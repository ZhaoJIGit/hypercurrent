/// <reference path="InvoiceEditBase.js" />
var RepeatInvoiceView = {
    init: function () {
        //操作记录
        $("#aHistory").click(function () {
            RepeatInvoiceEditBase.viewHistory();
        });
    }
}
$(document).ready(function () {
    RepeatInvoiceView.init();
});