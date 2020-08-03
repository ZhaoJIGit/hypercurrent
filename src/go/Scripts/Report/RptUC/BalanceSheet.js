/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    init: function () {
        $("#divReportDetail").removeClass("report-content-gl").addClass("report-content-gl");
        var opts = {};
        opts.url = "/Report/RptBalanceSheet/GetReportData";
        UCReport.init(opts);

        $("#aReportUpdate").click(function () {
            if (!$("[comboname='MDateString']").combobox("getValue")) {
                return;
            }

            UCReport.reload();
        });
    },
}

$(document).ready(function () {
    MyReport.init();
});