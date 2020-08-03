var BillUrl = {
    open: function (opts, isRefresh) {
        var billId = opts.BillID;
        var bizType = opts.BillType;
        var ref = opts.Ref;
        var url = "";
        var titlePre = "";
        var title = "";
        var defaultTitle = "";
        switch (bizType) {
            case "Invoice_Purchase":
                defaultTitle = HtmlLang.Write(LangModule.IV, "ViewBill", "View Bill");
                url = "/IV/Bill/BillView/" + billId;
                //title = BillUrl.getTitle("Bill", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Invoice_Purchase_Red":
                defaultTitle = HtmlLang.Write(LangModule.IV, "ViewBillCreditNote", "View Bill Credit Note");
                url = "/IV/Bill/CreditNoteView/" + billId;
                //title = BillUrl.getTitle("BCN", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Invoice_Sale":
                defaultTitle = HtmlLang.Write(LangModule.IV, "ViewInvoice", "View Invoice");
                url = "/IV/Invoice/InvoiceView/" + billId;
                //title = BillUrl.getTitle("IV", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Invoice_Sale_Red":
                //红字销售单
                defaultTitle = HtmlLang.Write(LangModule.IV, "CreditNote", "View Credit Note");
                url = "/IV/Invoice/CreditNoteView/" + billId;
                //title = BillUrl.getTitle("CN", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Pay_Purchase":
            case "Pay_Other":
            case "Pay_Adjustment":
            case "Pay_BankFee":
            case "Pay_BankInterest":
                defaultTitle = HtmlLang.Write(LangModule.IV, "ViewPayment", "View Payment");
                url = "/IV/Payment/PaymentView/" + billId;
                title = defaultTitle;
                break;
            case "Pay_PurReturn":
            case "Pay_OtherReturn":
                defaultTitle = HtmlLang.Write(LangModule.IV, "Refund", "Refund");
                url = "/IV/Payment/PaymentView/" + billId;
                //title = BillUrl.getTitle("Payment", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Receive_Sale":
            case "Receive_Other":
            case "Receive_Adjustment":
            case "Receive_BankFee":
            case "Receive_BankInterest":
                defaultTitle = HtmlLang.Write(LangModule.IV, "ViewReceive", "View Receive");
                url = "/IV/Receipt/ReceiptView/" + billId;
                //title = BillUrl.getTitle("Receive", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Receive_SaleReturn":
            case "Receive_OtherReturn":
                defaultTitle = HtmlLang.Write(LangModule.IV, "Refund", "Refund");
                url = "/IV/Receipt/ReceiptView/" + billId;
                //title = BillUrl.getTitle("Receive", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Expense_Claims":
                defaultTitle = HtmlLang.Write(LangModule.IV, "ViewExpense", "View Expense");
                url = "/IV/Expense/ExpenseEdit/" + billId;
                //title = BillUrl.getTitle("Expense", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Transfer":
                defaultTitle = HtmlLang.Write(LangModule.IV, "ViewTransfer", "View Transfer");
                url = "/IV/IVTransfer/IVTransferHome?MID=" + billId;
                //title = BillUrl.getTitle("Transfer", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "Pay_Salary":
                defaultTitle = HtmlLang.Write(LangModule.PA, "ViewSalaryList", "View Salary List");
                url = "/PA/SalaryPayment/SalaryPaymentEdit/" + billId;
                //title = BillUrl.getTitle("Expense", ref, defaultTitle);
                title = defaultTitle;
                break;
            case "MergePay":
                $.mTab.addOrUpdate(Megi.getCombineTitle([ref, HtmlLang.Write(LangModule.PA, "SalaryList", "Salary List")]),
                    "/PA/SalaryPayment/GetSalaryPaymentListByVerificationId/" + billId);
                return;
        }
        //删除核销关系时，刷新关联单据页面
        if (isRefresh) {
            $.mTab.refresh(title, url, false, true);
        }
        else {
            $.mTab.addOrUpdate(title, url);
        }
    },
    getTitle: function (titlePre,ref,defaultTitle) {
        if (ref != null && ref != "null" && ref != "")
        {
            return titlePre + ":" + ref;
        }
        return defaultTitle;
    }
}