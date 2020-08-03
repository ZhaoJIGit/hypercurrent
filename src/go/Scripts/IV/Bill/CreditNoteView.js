/// <reference path="../IVEditOptBase.js" />
/// <reference path="../IVBase.js" 
/// <reference path="../UC/MakePayment.js" />
/// <reference path="../IVEditBase.js" />
var CreditNoteView = {
    init: function () {
        $("#aPrint").click(function () {
            CreditNoteEditBase.printInvoice($(this).attr("billtype"), "PurchaseRedInvoice");
        });
        //操作记录
        $("#aHistory").click(function () {
            CreditNoteEditBase.viewHistory();
        });
        //发送Email
        $("#aEmail").click(function () {
            CreditNoteEditBase.emailInvoice();
        });
    }
}

$(document).ready(function () {
    CreditNoteView.init();
});