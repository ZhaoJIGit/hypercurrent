var BillView = {
    init: function () {
        $("#aPrint").click(function () {
            BillEditBase.printInvoice($(this).attr("billtype"), "PurchaseInvoice");
        });
        //操作记录
        $("#aHistory").click(function () {
            BillEditBase.viewHistory();
        });
        //发送Email
        $("#aEmail").click(function () {
            BillEditBase.emailInvoice();
        });
        //增加红字发票
        $("#btnCopyCredit").click(function () {
            var title= HtmlLang.Write(LangModule.IV, "NewBillCreditNote", "New Credit Note");
            var url= "/IV/Bill/CreditNoteEdit?cpyId=" + BillEditBase.InvoiceID + "&isCopyCredit=true";
            $.mTab.addOrUpdate(title, url, false);
        });
    }
}
$(document).ready(function () {
    BillView.init();
});