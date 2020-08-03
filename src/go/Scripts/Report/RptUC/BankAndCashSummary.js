/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RptBankAndCashSummary/GetReportData";
        UCReport.init(opts);
        $("#aReportUpdate").click(function () {
            if (!$('#selFromDate').combobox("getValue")) {
                return;
            }
            UCReport.reload();
        });
    }
}

$(document).ready(function () {
    MyReport.init();
});