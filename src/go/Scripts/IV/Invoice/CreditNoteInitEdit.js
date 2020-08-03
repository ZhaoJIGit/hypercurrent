/// <reference path="../IVEditOptBase.js" />
/// <reference path="../IVBase.js" 
/// <reference path="../UC/MakePayment.js" />
/// <reference path="../IVEditBase.js" />
var CreditNoteEdit = {
    init: function () {
        CreditNoteEditBase.IsInit = true;
        $("#btnSave").click(function () {
            CreditNoteEditBase.saveInvoiceAndApprove(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                parent.InitBills.reloadData();
                $.mDialog.close();
            });
        });
        $("#btnSaveAndAddAnother").click(function () {
            CreditNoteEditBase.saveInvoiceAndApprove(function () {
                $.mMsg(LangKey.SaveSuccessfully);
                parent.InitBills.reloadData();
            });
        });

        var invoiceId = $("#hidInvoiceID").val();
        if (invoiceId) {
            $("#selContact").combobox("disable");
        }
    }
}

$(document).ready(function () {
    CreditNoteEdit.init();
});