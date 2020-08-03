/// <reference path="../IVEditOptBase.js" />
/// <reference path="../IVBase.js" 
/// <reference path="../IVEditBase.js" />
var PaymentEdit = {
    init: function () {
        $("#aSavePayment").click(function () {
            PaymentEditBase.savePayment(function (msg, model) {

                //保存付款单附件
                AssociateFiles.associateFilesTo($("#hidBizObject").val(), msg.ObjectID, undefined, function () {
                    //提示信息
                    $.mMsg(LangKey.SaveSuccessfully);
                    //页签标题
                    //var tabTitle = $.trim(model.MReference) != "" ? $.mIV.getTitle(mTitle_Pre_Payment, model.MReference) : HtmlLang.Write(LangModule.Bank, "EditPayment", "Edit Payment");
                    var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewPayment", "View Payment");
                    //付款类别
                    var payType = $('#selType').combobox('getValue');
                    if (payType == "Pay_PurReturn" || payType == "Pay_OtherReturn") {
                        tabTitle = HtmlLang.Write(LangModule.Bank, "ViewRefund", "View Refund");
                    }
                    $.mTab.rename(tabTitle);
                    //跳转至查看页面
                    mWindow.reload("/IV/Payment/PaymentView/" + msg.ObjectID + "?acctid=" + PaymentEditBase.BankID + "&sv=1");
                });
            });
        });
    }
}
//初始化页面
$(document).ready(function () {
    PaymentEdit.init();
});