/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ConfirmImport = {
    type: $("#hidtype").val(),
    solutionID: $("#hidSolutionID").val(),
    fileName: $("#hidNewFileName").val(),
    init: function () {
        ConfirmImport.setTitle();
        ImportBase.minDialog();
        ConfirmImport.bindAction();
    },
    setTitle: function () {
        var title = '';
        switch (parseInt(ConfirmImport.type)) {
            case ImportTypeExt.InFaPiao:
                title = HtmlLang.Write(LangModule.FP, "ImportPurchaseFapiao", "导入进项发票");
                break;
            case ImportTypeExt.OutFaPiao:
                title = HtmlLang.Write(LangModule.FP, "ImportSaleFapiao", "导入销项发票");
                break;
        }
        if (title) {
            $("div.boxTitle>h3", parent.document).text(title);
        }
    },
    getConfirmModel: function () {
        var model = {};
        model.SolutionID = ConfirmImport.solutionID;
        model.FileName = ConfirmImport.fileName;
        model.IsSaveData = true;
        switch (parseInt(ConfirmImport.type)) {
            case ImportTypeExt.OutFaPiao:
                model.FaPiaoType = "0";
                model.TemplateType = ConfirmImport.type;
                break;
            case ImportTypeExt.InFaPiao:
                model.FaPiaoType = "1";
                model.TemplateType = ConfirmImport.type;
                break;
        }
        return model;
    },
    bindAction: function () {
        $("#aCompleteImport").click(function () {
            var model = ConfirmImport.getConfirmModel();

            mAjax.submit(
                "/IO/ImportBySolution/Save",
                { model: model },
                function (msg) {

                    var addMsgList = [];
                    var importedCount = $("#hidImportingCount").val();

                    switch (parseInt(ConfirmImport.type)) {
                        case ImportTypeExt.OutFaPiao:
                            ImportBase.showSuccessMsg(ImportType.OutFaPiao, importedCount, addMsgList);
                            break;
                        case ImportTypeExt.InFaPiao:
                            ImportBase.showSuccessMsg(ImportType.InFaPiao, importedCount, addMsgList);
                            break;
                    }

                    if (parseInt(ConfirmImport.type) == ImportTypeExt.OutFaPiao
                        || parseInt(ConfirmImport.type) == ImportTypeExt.InFaPiao) {
                        $.mDialog.close(1, "");
                    }
                    else {
                        $.mDialog.close();
                    }
                });
        });

        $("#aGoBack").click(function () {
            var url = "/IO/ImportBySolution/ImportOptions/" + ConfirmImport.solutionID + "?type=" + ConfirmImport.type
                + "&fileName=" + encodeURIComponent(ConfirmImport.fileName);
            mWindow.reload(url);
        });
    }
}

$(document).ready(function () {
    ConfirmImport.init();
});