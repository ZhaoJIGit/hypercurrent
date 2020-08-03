/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RptBankRecSummary/GetReportData";
        UCReport.init(opts);
        $("#aReportUpdate").click(function () {
            if (!$("#selAccount").combobox("getValue")) {
                return;
            }
            UCReport.reload();
        });
    },
    setLoadValid: function () {
        $("#selAccount").next().find("input").addClass("easyui-load-valid")
    }
}

$(document).ready(function () {
    MyReport.init();
});