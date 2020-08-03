/// <reference path="../IVEditOptBase.js" />
/// <reference path="../IVBase.js" 
/// <reference path="../IVEditBase.js" />
var ReceiptView = {
    IsShowVerifConfirm: function () {
        var isShowVerif = $("#hidIsShowReceiptVerif").val() == "True" ? true : false;
        var receiveId = $("#hidReceiptID").val();
        if (receiveId != undefined && isShowVerif && Megi.request("sv") == "1") {
            mAjax.post(
                "/IV/Receipt/GetVerificationById",
                { receiveId: receiveId },
                function (data) {
                    if (data != null && data.length > 0) {
                        var currency = data[0].MCurrencyID;
                        var totalAmount = 0;
                        $(data).each(function (i) {
                            totalAmount += data[i].Amount;
                        });
                        var title = HtmlLang.Write(LangModule.IV, "ThereIs", " There is") + " " + "<b>" +
                                    Megi.Math.toMoneyFormat(totalAmount, 2) + " " + currency + "</b>" + " " +
                                    HtmlLang.Write(LangModule.IV, "InOutstandingCredit", "in outstanding credit.") + " " +
                                    HtmlLang.Write(LangModule.IV, "AllocatecreditToReceipt", "Would you like to allocate credit to this receipt?");
                        $.mDialog.confirm(title, function (sure) {
                            if (sure) {
                                Verification.open(receiveId, "Receive");
                            }
                        }, "", true);
                    }
                });
        }
    }
}
//初始化页面
$(document).ready(function () {
    ReceiptView.IsShowVerifConfirm();
});