var UCReportHeader = {
    editor: null,
    isReadonly: false,
    DataObject: {},
    getValue: function () {
        return UCReportHeader.DataObject;
    },
    init: function (opts) {
        UCReportHeader.initData(opts);
        if (!opts.isReload) {
            UCReportHeader.initEditor();
            UCReportHeader.initButton();
            UCReportHeader.updateControlContent();
            UCReportHeader.setVisible();
            if (!UCReportHeader.isReadonly && UCReportHeader.DataObject.hasChangeAuth) {
                $(".view-detail").mouseover(function () {
                    $("#divReportEditButton").show();
                    $(this).addClass("hover");
                });
                $(".view-detail").mouseout(function () {
                    $("#divReportEditButton").hide();
                    $(this).removeClass("hover");
                });
                $(".view-detail").click(function () {
                    UCReportHeader.updateControlContent();
                    $("#divReportHeaderView").hide();
                    $("#divReportHeaderEdit").show();
                });
            } else {
                $(".view-close").hide();
            }
        }
    },
    initData: function (opts) {
        UCReportHeader.DataObject = { title: HtmlLang.Write(LangModule.Report, "Summary", "Summary"), content: '', isNew: true };
        if (opts == undefined) {
            opts = {};
        }
        if (opts.title == undefined) {
            opts.title = HtmlLang.Write(LangModule.Report, "Summary", "Summary");
        }
        if (opts.content == undefined) {
            opts.content = "";
        }
        if (opts.isNew == undefined) {
            opts.isNew = true;
        }
        UCReportHeader.DataObject.title = opts.title;
        UCReportHeader.DataObject.content = opts.content;
        UCReportHeader.DataObject.isNew = opts.isNew;
        UCReportHeader.DataObject.hasChangeAuth = opts.hasChangeAuth;
        UCReportHeader.isReadonly = opts.isReadonly;
    },
    initEditor: function () {
        if (UCReportHeader.editor == null) {
            UCReportHeader.editor = window.KindEditor.create('#txtHeaderContent', {
                items: ['bold', 'italic', 'underline', 'removeformat', '|', 'table', 'insertorderedlist', 'insertunorderedlist', 'wordpaste']
            });
            UCReportHeader.editor.html(UCReportHeader.DataObject.content);
        } else {
            UCReportHeader.editor.html(UCReportHeader.DataObject.content);
        }
    },
    initButton: function () {
        $("#aAddReportHeader").click(function () {
            $("#aAddReportHeader").hide();
            $("#divReportHeaderEdit").show();
        });
        $("#aReportHeaderSave").click(function () {
            var title = $("#txtHeaderTitle").val();
            var content = UCReportHeader.editor.html();
            UCReportHeader.DataObject.title = title;
            UCReportHeader.DataObject.content = content;
            UCReportHeader.DataObject.isNew = false;
            UCReportHeader.updateControlContent();

            $("#divReportHeaderEdit").hide();
            $("#divReportHeaderView").show();
        });
        $("#aReportHeaderCancel").click(function () {
            UCReportHeader.setVisible();
        });
        $("#aReportDeleteHeaderContent").click(function () {
            UCReportHeader.DataObject.content = "";
            UCReportHeader.DataObject.isNew = true;
            UCReportHeader.updateControlContent();
            UCReportHeader.setVisible();
        });
    },
    setVisible: function () {
        if (UCReportHeader.DataObject.isNew && UCReportHeader.DataObject.hasChangeAuth == true) {
            $("#divReportHeaderEdit").hide();
            $("#divReportHeaderView").hide();
            $("#aAddReportHeader").show();
        } else {
            $("#aAddReportHeader").hide();
            $("#divReportHeaderEdit").hide();
            $("#divReportHeaderView").show();
        }
    },
    updateControlContent: function () {
        var title = HtmlLang.Write(LangModule.Report, "AddDescription", "Add Description") ;
        //UCReportHeader.DataObject.title;
        $("#aAddReportHeader").find(".l-btn-text").html(title);
        $("#txtHeaderTitle").val(HtmlLang.Write(LangModule.Report, "Description", "Description"));
        $("#divReportViewTitle").html(UCReportHeader.DataObject.title);
        if (UCReportHeader.editor != null) {
            UCReportHeader.editor.html(UCReportHeader.DataObject.content);
        }
        $("#divReportViewContent").html(UCReportHeader.DataObject.content);
    }
}

$(document).ready(function () {

    UCReportHeader.initEditor();
});