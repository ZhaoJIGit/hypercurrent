/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RptBankStatement/GetReportData";
        UCReport.init(opts);
        $("#aReportUpdate").click(function () {
            UCReport.reload();
        });
    }
}

$(document).ready(function () {
    MyReport.init();
});