var BillEdit = {
    init: function () {
        BillEditBase.IsInit = true;
        $("#btnSave").click(function () {
            BillEditBase.saveInvoiceAndApprove(function (msg) {
                if (msg.Success) {
                    $.mMsg(LangKey.SaveSuccessfully);

                    var title = HtmlLang.Write(LangModule.BD, "AccountBalances", "Account Balances");
                    $.mTab.refresh(title, '/BD/BDAccount/AccountBalances', true);

                    parent.InitBills && parent.InitBills.reloadData();
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
                
            });
        });
        $("#btnSaveAndAddAnother").click(function () {
            BillEditBase.saveInvoiceAndApprove(function () {
                $.mMsg(LangKey.SaveSuccessfully);
                parent.InitBills && parent.InitBills.reloadData();
            });
        });

        var invoiceId = $("#hidInvoiceID").val();
        if (invoiceId) {
            $("#selContact").combobox("disable");
        }
    }
}

$(document).ready(function () {
    BillEdit.init();
});