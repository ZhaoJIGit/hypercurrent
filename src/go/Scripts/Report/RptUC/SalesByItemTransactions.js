/// <reference path="../RptUC/UCReport.js" />
var MyInvoiceReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RptSalesByItemTransactions/GetReportData";
        UCReport.init(opts);

        $("#aReportUpdate").click(function () {
            UCReport.reload();
        });

        $("#aBackToReport").click(function () {
            UCReport.backToParentReport();
            return false;
        });
    }
}

$(document).ready(function () {
    MyInvoiceReport.init();
});