/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var Import = {
    id: Megi.getUrlParam("id"),
    type: Megi.getUrlParam("type"),
    init: function () {
        Import.setTitle();
        ImportBase.minDialog();
        Import.bindAction();
        Import.initUI();
    },
    setTitle: function () {
        $("div.boxTitle>h3", parent.document).text(HtmlLang.Write(LangModule.Common, "Import", "Import"));
    },
    initUI: function () {
        if (Import.type) {
            Import.initSelect();
        }
        var msg = $("#hidMessage").val();
        if (msg) {
            $.mDialog.alert(_.htmlDecode(msg));
            return;
        }
    },
    initSelect: function () {
        $("#selType").combobox("select", Import.type);
    },
    bindAction: function () {
        $("#aImport").click(function () {
            if (!$("#divImport").mFormValidate()) {
                return;
            }
            if (ImportBase.validateFile()) {
                $(".m-imain").mask("");
                Import.bindSubmit();
                $("#fileSelectForm").submit();
            }
        });
        if (Megi.isIE9Previous()) {
            var type = "change";
            var file = document.getElementById("fileInput");
            if (typeof (document.createEvent) != 'undefined') {
                e = document.createEvent('HTMLEvents');
                e.initEvent(type, true, true);
                file.dispatchEvent(e);
            } else if (typeof (document.createEventObject) != 'undefined') {
                try {
                    e = document.createEventObject();
                    file.fireEvent('on' + type.toLowerCase(), e);
                } catch (err) { }
            }
        }
        $("#fileInput").change(function () {
            ImportBase.onFileChanged(this);

            var validateResult = FileBase.validateFile(this.value, 0, FileBase.excelIncludeCsvRegex);
            if (validateResult) {
                ImportBase.clearFile(this);
                $.mDialog.alert(validateResult);
            }
        });

        $("#aCancel").click(function () {
            ImportBase.closeImportBox();
        });

        Megi.attachPropChangeEvent(document.getElementById("MName"), function () {
            Import.switchValidate(this.value);
        });
    },
    switchValidate: function (val) {
        $("#MName").validatebox(val ? "disableValidation" : "enableValidation");
    },
    changeType: function (item) {
        if (item) {
            Import.bindSolutionList(item.Key, Import.parentId);
        }
    },
    changeSolution: function (item) {
        if (item) {
            if (item.MItemID) {
                $("#MName").val(item.MName);
            }
            else {
                $("#MName").val('');
                $("#MName").focus();
            }
            Import.switchValidate(item.MItemID);
        }
    },
    bindSolutionList: function (type) {
        mAjax.post(
            "/IO/ImportBySolution/GetSolutionList", 
            { type: type }, 
            function (msg) {
                var opt = {};
                opt.data = msg;
                opt.onSelect = Import.changeSolution;
                opt.valueField = "MItemID";
                opt.textField = "MName";
                opt.required = true;
                $("#selSolution").combobox(opt);
                var defaultSelect = msg[0].MItemID;
                if (Import.id) {
                    defaultSelect = Import.id;
                }
                else if (msg.length > 1) {
                    defaultSelect = msg[1].MItemID;
                }
                $("#selSolution").combobox("select", defaultSelect);
            });
    },
    getModel: function () {
        var model = {};
        model.SolutionID = $("#selSolution").combobox("getValue");
        model.TemplateType = $("#selType").combobox("getValue");
        model.MSolutionName =  mText.decode($("#MName").val());
        return model;
    },
    bindSubmit: function () {
        $("form").ajaxForm({
            data: Import.getModel(),
            dataType: ImportBase.isIE9Previous ? null : "json",
            success: function (response) {
                response = response.Data;
                if (response && !ImportBase.isIE9Previous && !response.Success) {
                    $.mDialog.alert(response.Message);
                    $(".m-imain").unmask();
                } else {
                    var fileName = response.MFileName;
                    var type = $("#selType").combobox("getValue");
                    var url = "/IO/ImportBySolution/ImportOptions/" + response.ObjectID + "?type=" + type + "&fileName=" + encodeURIComponent(fileName);
                    mWindow.reload(url);
                }
            },
            fail: function (event, data) {
                $.mDialog.alert(data);
            }
        });
    }
}

$(document).ready(function () {
    $("#MName").validatebox("disableValidation");
    Import.init();
});