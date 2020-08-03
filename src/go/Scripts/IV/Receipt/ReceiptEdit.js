var ReceiptEdit = {
    init: function () {
        //保存收款单
        $("#aSaveReceipt").click(function () {
            ReceiptEditBase.saveReceipt(function (msg, model) {
                //保存收款单附件
                AssociateFiles.associateFilesTo($("#hidBizObject").val(), msg.ObjectID, undefined, function () {
                    //提示信息
                    $.mMsg(LangKey.SaveSuccessfully);
                    //页签标题
                    //var tabTitle = $.trim(model.MReference) != "" ? $.mIV.getTitle(mTitle_Pre_Receive, model.MReference) : HtmlLang.Write(LangModule.Bank, "EditReceipt", "Edit Receipt");
                    var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewReceipt", "Receive Money");
                    //收款类别
                    var payType = $('#selType').combobox('getValue');
                    if (payType == "Receive_SaleReturn" || payType == "Receive_OtherReturn") {
                        tabTitle = HtmlLang.Write(LangModule.Bank, "ViewRefund", "View Refund");
                    }
                    $.mTab.rename(tabTitle);
                    //跳转至查看页面
                    mWindow.reload("/IV/Receipt/ReceiptView/" + msg.ObjectID + "?acctid=" + ReceiptEditBase.BankID + "&sv=1");
                });
            });
        });
    },
}
//初始化页面
$(document).ready(function () {
    ReceiptEdit.init();
});