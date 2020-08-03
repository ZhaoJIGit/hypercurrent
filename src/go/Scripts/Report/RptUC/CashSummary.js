/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RptCashSummary/GetReportData";
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
    MyReport.init();
});