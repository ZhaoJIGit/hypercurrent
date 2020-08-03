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
    },
    downloadTemplate:function() {
        ImportBase.downloadTemplate('/BD/Item/DownloadImportTemplate');
    }
}

$(document).ready(function () {
    Import.init();
    $("form").ajaxForm({
        dataType: ImportBase.isIE9Previous ? null : "json",
        success: function(response) {
            if (response && !ImportBase.isIE9Previous && !response.Success) {
                var msg = typeof (response.VerificationInfor) != 'undefined' ? response.VerificationInfor[0].Message : response.Message;
                $.mDialog.alert(msg);
                $(".m-imain").unmask();
            } else {
                window.location.href = "/BD/ExpenseItem/ConfirmImport";
            }
        },
        fail: function(event, data) {
            $.mDialog.alert(data);
        }
    });
});