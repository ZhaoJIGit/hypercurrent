/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RptIncomeByContact/GetReportData";
        UCReport.init(opts);

        $("#aReportUpdate").click(function () {
            if (!$("[comboname='MEndDate']").combobox("getValue")) {
                return;
            }

            UCReport.reload();
        });
    }
}

$(document).ready(function () {
    MyReport.init();
});