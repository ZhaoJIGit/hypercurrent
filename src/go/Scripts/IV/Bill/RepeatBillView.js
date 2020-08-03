/// <reference path="InvoiceEditBase.js" />
var RepeatBillView = {
    init: function () {
        //操作记录
        $("#aHistory").click(function () {
            RepeatBillEditBase.viewHistory();
        });
    }
}
$(document).ready(function () {
    RepeatBillView.init();
});