/// <reference path="../IVBase.js" />
/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var MakePayment = {
    type: $("#hidInvoiceType").val(),
    currencyId: $("#hidCurrencyID").val(),
    init: function () {
        var ivModel = mText.getObject("#hidInvoiceModel");;

        //支付账号默认第一个选中
        var arrBankAcct = $('#selPaidTo').combobox('getData');
        if (arrBankAcct != null && arrBankAcct.length > 0) {
            $("#selPaidTo").combobox('select', arrBankAcct[0].id);
        }

        $("#aAddPayment").click(function () {
            MakePayment.createPayment();
        });
        var amt = Megi.Math.toDecimal((Math.abs(ivModel.MTaxTotalAmtFor) - Math.abs(ivModel.MVerificationAmt)), 2);
        $('#txtAmountPaid').numberbox({
            min: 0,
            precision: 2,
            required: true,
            max: Number(amt)
        });
        $("#txtAmountPaid").removeClass("validatebox-invalid");//去掉错误信息提示
        $('#txtAmountPaid').numberbox('setValue', amt);

        $("#selPaidTo").combobox({
            onShowPanel: function () {
                var panel = $(this).data("combo").panel;
                var items = panel.find(".combobox-item:visible,.combobox-group:visible");
                if (items.length == 0) {
                    mAjax.post(
                        "/BD/Currency/GetCurrencyName",
                        { currencyId: MakePayment.currencyId },
                        function (response) {
                            if (response) {
                                $.mDialog.alert(HtmlLang.Write(LangModule.Common, "NotFoundSpecificCurrencyBankAccount", "请先创建{0}银行账户!").replace("{0}", response));
                            }
                        })
                };
            }
        });
    },
    createPayment: function () {
        //验证支付信息
        var result = $(".m-form").mFormValidate();
        if (!result) {
            //验证不通过，则终止该操作
            return;
        }
        var amtPaid = $('#txtAmountPaid').numberbox('getValue');
        var acctId = $('#selPaidTo').combobox('getValue');
        var bizDate = $('#txtPaidDate').datebox('getValue');
        var ref = $("#txtPaymentRef").val();

        var ivModel = mText.getObject("#hidInvoiceModel");;

        var obj = {};
        obj.MObjectID = ivModel.MID;
        obj.MBankID = acctId;
        obj.MPaidAmount = amtPaid;
        obj.MPaidDate = bizDate;
        obj.MRef = ref;

        var url = "/IV/Invoice/AddInvoiceReceive";
        if (ivModel.MType == IVBase.InvoiceType.Sale || ivModel.MType == IVBase.InvoiceType.Invoice_Sale_Red) {
            url = "/IV/Invoice/AddInvoiceReceive";
        } else if (ivModel.MType == IVBase.InvoiceType.Purchase || ivModel.MType == IVBase.InvoiceType.InvoicePurchaseRed) {
            url = "/IV/Invoice/AddInvoicePayment";
        }

        mAjax.submit(
            url,
            { model: obj },
            function (callbackData) {
                if (!callbackData.Success) {
                    $.mDialog.alert(callbackData.Message);
                    return;
                }
                $.mDialog.callback();
                //提示信息
                var msg;
                switch (MakePayment.type) {
                    case IVBase.InvoiceType.Sale:
                        msg = HtmlLang.Write(LangModule.IV, "ReceivePaymentSuccessful", "Receive payment successful!");
                        break;
                    case IVBase.InvoiceType.InvoiceSaleRed:
                    case IVBase.InvoiceType.InvoicePurchaseRed:
                        msg = HtmlLang.Write(LangModule.IV, "RefundSuccessful", "Refund successful!");
                        break;
                    case IVBase.InvoiceType.Purchase:
                        msg = HtmlLang.Write(LangModule.IV, "SuccessfulPayment", "Payment successful!");
                        break;
                }
                $.mMsg(msg);
                //更改页签标题
                if (ivModel.MType == IVBase.InvoiceType.Sale) {
                    $.mTab.rename(HtmlLang.Write(LangModule.IV, "ViewInvoice", "View Invoice"));
                }
                else if (ivModel.MType == IVBase.InvoiceType.Purchase) {
                    $.mTab.rename(HtmlLang.Write(LangModule.IV, "ViewBill", "View Bill"));
                }
                else {
                    $.mTab.rename(HtmlLang.Write(LangModule.IV, "ViewCreditNote", "View Credit Note"));
                }
                parent.mWindow.reload(Megi.request("sv", "0", parent.window.location.href));
                Megi.closeDialog();
            });
    }
}
function changeURLPar(destiny, par, par_value) {
    var pattern = par + '=([^&]*)';
    var replaceText = par + '=' + par_value;
    if (destiny.match(pattern)) {
        var tmp = '/\\' + par + '=[^&]*/';
        tmp = destiny.replace(eval(tmp), replaceText);
        return (tmp);
    }
    else {
        if (destiny.match('[\?]')) {
            return destiny + '&' + replaceText;
        }
        else {
            return destiny + '?' + replaceText;
        }
    }
    return destiny + '\n' + par + '\n' + par_value;
}

$(document).ready(function () {
    MakePayment.init();
});