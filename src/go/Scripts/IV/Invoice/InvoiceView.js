/// <reference path="InvoiceEditBase.js" />
var InvoiceView = {
    init: function () {
        //打印
        $("#aPrint").click(function () {
            InvoiceEditBase.printInvoice($(this).attr("billtype"), "SaleInvoice");
        });
        //操作记录
        $("#aHistory").click(function () {
            InvoiceEditBase.viewHistory();
        });
        //发送Email
        $("#aEmail").click(function () {
            InvoiceEditBase.emailInvoice();
        });
        //保存
        $("#btnSave").click(function () {
            //到期日期必须大于单据录入日期
            var bizDate = $('#txtDate').datebox('getValue');
            //预计付款日期必须大于等于单据录入日期
            var expectedDate = $('#txtExpectedDate').datebox('getValue');
            if (expectedDate) {
                //如果选择预计付款日期才需要验证
                if (expectedDate < bizDate) {
                    //$.mDialog.alert("Expected Payment date must be greater than or equal to date.", null, LangModule.IV, "ExpectedDateIsGreaterThanTheDate");
                    $.mDialog.alert(HtmlLang.Write(LangModule.IV, "ExpectedDateIsGreaterThanTheDate", "Expected Payment date must be greater than or equal to date."));
                    return;
                }
            }
            var invoiceId = $("#hidInvoiceID").val();
            var obj = {};
            obj.MID = invoiceId;
            obj.MExpectedDate = expectedDate;
            $("body").mFormSubmit({
                url: "/IV/IVInvoiceBase/UpdateInvoiceExpectedInfo?type=Sale", param: { model: obj }, validate: true, callback: function (msg) {
                    //提示信息
                    $.mMsg(LangKey.SaveSuccessfully);
                    //刷新列表页面
                    var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                    var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(InvoiceEditBase.CurrentStatus);
                    $.mTab.refresh(title, url, false);
                    //刷新当前页面
                    mWindow.reload(InvoiceEditBase.ViewUrl + "/" + invoiceId);
                }
            });
        });
        //增加红字发票
        $("#btnCopyCredit").click(function () {
            title = HtmlLang.Write(LangModule.IV, "NewCreditNote", "New Credit Note");
            var url = "/IV/Invoice/CreditNoteEdit?cpyId=" + InvoiceEditBase.InvoiceID + "&isCopyCredit=true";

            $.mTab.addOrUpdate(title, url, false);
        });
    }
}
$(document).ready(function () {
    InvoiceView.init();
});