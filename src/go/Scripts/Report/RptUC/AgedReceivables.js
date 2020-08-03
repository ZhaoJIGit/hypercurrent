/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RptAgedReceivable/GetReportData";
        UCReport.init(opts);

        $("#aReportUpdate").click(function () {
            if (!$("[comboname='MEndDateExt']").combobox("getValue") ||
                !$("[comboname='AgedShowType']").combobox("getValue") ||
                !$("[comboname='AgedByField']").combobox("getValue")) {
                return;
            }

            UCReport.reload();
        });
    }
}

$(document).ready(function () {
    MyReport.init();
});