/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    opts: {},
    init: function () {
        MyReport.initTab();
        MyReport.clickAcion();
    },
    initTab: function () {
        MyReport.initUrl();
        $("#tabSalesTax").tabs({
            onSelect: function (title, index) {
                MyReport.initUrl(index);
                UCReport.reload();
            }
        });
    },
    clickAcion: function () {
        $("#aReportUpdate").click(function () {
            UCReport.reload();
        });
    },
    initUrl: function (index) {
        MyReport.opts.url = (index == undefined || index == 0) ? "/Report/RptSalesTaxReport/GetReportData"
                                                               : "/Report/RptSalesTaxAuditReport/GetReportData";
        UCReport.init(MyReport.opts);
    }
}

$(document).ready(function () {
    MyReport.init();
});