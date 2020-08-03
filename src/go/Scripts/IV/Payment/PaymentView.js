/// <reference path="../IVEditOptBase.js" />
/// <reference path="../IVBase.js" 
/// <reference path="../IVEditBase.js" />
var PaymentView = {
    IsShowVerifConfirm: function () {
        var isShowVerif = $("#hidIsShowPaymentVerif").val() == "True" ? true : false;
        var paymentId = $("#hidPaymentID").val();
        if (paymentId != undefined && isShowVerif && Megi.request("sv") == "1") {
            mAjax.post(
                "/IV/Payment/GetVerificationById",
                { paymentId: paymentId }, function (data) {
                    if (data != null && data.length > 0) {
                        var currency = data[0].MCurrencyID;
                        var totalAmount = 0;
                        $(data).each(function (i) {
                            totalAmount += data[i].Amount;
                        });
                        var title = HtmlLang.Write(LangModule.IV, "ThereIs", " There is") + " " + "<b>" +
                                    Megi.Math.toMoneyFormat(totalAmount, 2) + " " + currency + "</b>" + " " +
                                    HtmlLang.Write(LangModule.IV, "InOutstandingCredit", "in outstanding credit.") + " " +
                                    HtmlLang.Write(LangModule.IV, "AllocatecreditToPayment", "Would you like to allocate credit to this payment?");
                        $.mDialog.confirm(title, function (sure) {
                            if (sure) {
                                Verification.open(paymentId, "Payment");
                            }
                        }, "", true);
                    }
                });
        }
    }
}
//初始化页面
$(document).ready(function () {
    PaymentView.IsShowVerifConfirm();
});