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
            $("body").mask("");
            try{
                $("#fileSelectForm").submit();
            }catch(e){
                $("body").unmask();
            }
        });
        $("#fileInput").change(function () {
            ImportBase.onFileChanged(this);

            var validateResult = FileBase.validateFile(this.value, 0, FileBase.excelExcludeCsvRegex);
            if (validateResult) {
                ImportBase.clearFile(this);
                $.mDialog.alert(validateResult);
            }
        });
    },
    downloadTemplate: function () {
        ImportBase.downloadTemplate('/BD/Employees/DownloadImportTemplate');
    }
}

$(document).ready(function () {
    Import.init();
    ImportBase.initAjaxForm("/BD/Employees/ConfirmImport");
});