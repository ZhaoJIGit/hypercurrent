var PTSalaryList = {
    canEdit: $("#hidCanEditPayRun").val() === "True",
    init: function () {
        if (PTSalaryList.canEdit) {
            PTBase.initSort("divSalaryList", "/PT/PTSalaryList/SortPT");
        }
        PTSalaryList.initUI();
        PTSalaryList.initAction();
    },
    initUI: function () {
        var totalW = $("#tabInit").width() - 34 - 15;
        var logoW = 210 + 35;
        $(".ps-list .settings").css({ "width": totalW - logoW });
    },
    initAction: function () {
        $("#divSalaryList .edit").off("click").on("click", function () {
            PTSalaryList.openDialog(this, HtmlLang.Write(LangModule.Common, "EditSalaryPT", "编辑工资单打印模板"), "/PT/PTSalaryList/PTSalaryEdit");
        });
        $("#divSalaryList .copy").off("click").on("click", function () {
            PTSalaryList.openDialog(this, HtmlLang.Write(LangModule.Common, "CopySalaryPT", "复制工资单打印模板"), "/PT/PTSalaryList/PTSalaryCopy", 400, 345);
        });
        $("#divSalaryList .upload-logo").off("click").on("click", function () {
            if (PTSalaryList.canEdit) {
                var title = $(this).closest(".upload-logo").find("img").length == 1
                    ? HtmlLang.Write(LangModule.Common, "ChangeLogo", "Change Logo")
                    : HtmlLang.Write(LangModule.Common, "UploadLogo", "Upload Logo");
                PTSalaryList.uploadOrChangeLogo(this, title);
            }
        });
        $("#divSalaryList .change-logo").off("click").on("click", function () {
            PTSalaryList.uploadOrChangeLogo(this, HtmlLang.Write(LangModule.Common, "ChangeLogo", "Change Logo"));
        });
        $("#divSalaryList .remove-logo").off("click").on("click", function () {
            PTSalaryList.removeLogo(this);
        });
        $("#divSalaryList .delete").off("click").on("click", function () {
            PTSalaryList.deleteSetting(this);
        });
    },
    reload: function () {
        PTBase.reload(PTTabType.SalaryList);
    },
    getEditId: function (sender) {
        var id = $(sender).closest(".print-tmpl").attr("id");
        if (!id) {
            var idx = parseInt($(sender).parent().attr("id").replace("divSalaryFolderOptions", ""));
            id = $("#divSalaryOptions" + idx).closest(".print-tmpl").attr("id");
        }
        return id;
    },
    uploadOrChangeLogo: function (sender, title) {
        PTSalaryList.openDialog(sender, title, "/PT/PTSalaryList/PTSalaryLogoUpload", 410, 270);
    },
    openDialog: function (sender, title, url, w, h, isNew) {
        if (!isNew) {
            url = url + '/' + PTSalaryList.getEditId(sender);
        }
        Megi.dialog({
            title: title,
            width: w,
            height: h,
            href: url
        });
    },
    removeLogo: function (sender) {
        mAjax.submit(
            "/PT/PTSalaryList/RemovePTLogo",
            { id: PTSalaryList.getEditId(sender) },
            function (msg) {
                if (msg.Success) {
                    PTSalaryList.reload();
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
                var id = PTSalaryList.getEditId(sender);
                mAjax.submit(
                    "/PT/PTSalaryList/DeletePT",
                    { id: id },
                    function (msg) {
                        if (msg.Success) {
                            $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                            PTSalaryList.reload();
                            //$(".print-tmpl[id='" + id + "']").remove();
                        } else {
                            $.mDialog.alert(msg.Message);
                        }
                    });
            }
        });
    }
}
$(document).ready(function () {
    PTSalaryList.init();
});