var PTBizList = {
    isSmartVersion: $("#hidIsSmartVersion").val() === "True",
    CurrentType: 0,
    canEdit: $("#hidCanEditSetting").val() === "True",
    init: function () {
        if (PTBizList.canEdit) {
            PTBase.initSort("divBiz", "/PT/PTBiz/SortPT");
        }
        PTBizList.initUI();
        PTBizList.initAction();
    },
    initUI: function () {
        var totalW = $(".m-imain-content").width();
        var centerMaxW = totalW - $(".settings").width() - $(".logo-wrapper").width() - 45;
        $(".center-wrapper").css("max-width", centerMaxW);
        if (PTBizList.isSmartVersion) {
            $("#divBiz .print-tmpl ul.show-list").width(300);
        }
    },
    getDialogOption: function () {
        var opt = {};
        opt.w = undefined;
        opt.h = undefined;
        if (PTBizList.isSmartVersion) {
            opt.w = 785;
            opt.h = 310;
        }
        return opt;
    },
    initAction: function () {
        $("#divBiz .edit").off("click").on("click", function () {
            var opt = PTBizList.getDialogOption();
            PTBizList.openDialog(this, HtmlLang.Write(LangModule.Common, "EditBizPT", "编辑单据和报表打印模板"), "/PT/PTBiz/PTBizEdit", opt.w, opt.h);
        });
        $("#divBiz .copy").off("click").on("click", function () {
            PTBizList.openDialog(this, HtmlLang.Write(LangModule.Common, "CopyBizPT", "复制单据和报表打印模板"), "/PT/PTBiz/PTBizCopy", 500, 345);
        });
        $("#divBiz .upload-logo").off("click").on("click", function () {
            if (PTBizList.canEdit) {
                var title = $(this).closest(".upload-logo").find("img").length == 1
                    ? HtmlLang.Write(LangModule.Common, "ChangeLogo", "Change Logo")
                    : HtmlLang.Write(LangModule.Common, "UploadLogo", "Upload Logo");
                PTBizList.uploadOrChangeLogo(this, title);
            }
        });
        $("#divBiz .change-logo").off("click").on("click", function () {
            PTBizList.uploadOrChangeLogo(this, HtmlLang.Write(LangModule.Common, "ChangeLogo", "Change Logo"));
        });
        $("#divBiz .remove-logo").off("click").on("click", function () {
            PTBizList.removeLogo(this);
        });
        $("#divBiz .delete").off("click").on("click", function () {
            PTBizList.deleteSetting(this);
        });
    },
    reload: function () {
        PTBase.reload(PTTabType.Biz);
    },
    getEditId: function (sender) {
        var id = $(sender).closest(".print-tmpl").attr("id");
        if (!id) {
            var idx = parseInt($(sender).parent().attr("id").replace("divBizFolderOptions", ""));
            id = $("#divBizOptions" + idx).closest(".print-tmpl").attr("id");
        }
        return id;
    },
    uploadOrChangeLogo: function (sender, title) {
        PTBizList.openDialog(sender, title, "/PT/PTBiz/PTBizLogoUpload", 410, 270);
    },
    openDialog: function (sender, title, url, w, h, isNew) {
        if (!isNew) {
            url = url + '/' + PTBizList.getEditId(sender)
        }
        var param = {
            title: title,
            href: url
        };
        if (w && h) {
            param.width = w;
            param.height = h;
        }

        Megi.dialog(param);
    },
    removeLogo: function (sender) {
        mAjax.submit(
            "/PT/PTBiz/RemovePTLogo",
            { id: PTBizList.getEditId(sender) },
            function (msg) {
                if (msg.Success) {
                    PTBizList.reload();
                }
                else {
                    $.mDialog.alert(msg.Message);
                }
            });
    },
    deleteSetting: function (sender) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete,
        {
            callback: function () {
                var id = PTBizList.getEditId(sender);
                mAjax.submit(
                    "/PT/PTBiz/DeletePT",
                    { id: id },
                    function (msg) {
                        if (msg.Success) {
                            $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                            PTBizList.reload();
                        } else {
                            $.mDialog.alert(msg.Message);
                        }
                    });
            }
        });
    }
}
$(document).ready(function () {
    PTBizList.init();
});