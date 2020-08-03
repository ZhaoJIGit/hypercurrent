/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var Import = {
    init: function () {
        Import.bindAction();
    },
    bindAction: function () {
        $("#aImport").click(function () {
            if (!ImportBase.validateFile()) {
                return;
            }
            $(".m-imain").mask("");
            $("#fileSelectForm").submit();
        });
        $("#fileInput").change(function () {
            ImportBase.onFileChanged(this);

            var validateResult = FileBase.validateFile(this.value, 0, FileBase.excelExcludeCsvRegex);
            if (validateResult) {
                ImportBase.clearFile(this);
                $.mDialog.alert(validateResult);
            }
        });

        $("#aCancel").click(function () {
            $.mDialog.close();
        });
    },
    downloadTemplate: function () {
        ImportBase.downloadTemplate('/BD/Contacts/DownloadImportTemplate');
    }
}

$(document).ready(function () {
    Import.init();
    ImportBase.initAjaxForm("/BD/Contacts/ConfirmImport");
});