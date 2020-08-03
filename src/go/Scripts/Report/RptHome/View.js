/// <reference path="UCReport.js" />
var Report = {
    init: function () {
        var opts = {};
        //总账报表新增样式
        var reportType = $("#hidReportTypeID").val();
        if (reportType == "41" || reportType == "42" || reportType == "33"
            || reportType == "23" || reportType == "39" || reportType == "40"
            || reportType == "501" || reportType == "502" || reportType == "503" || reportType == "504") {
            $("#divReportDetail").removeClass("report-content-gl").addClass("report-content-gl");

            opts.autoFillCell = false;
        }

        
        opts.url = "/Report/RptManager/GetReportData";
        opts.isReadonly = true;
        UCReport.init(opts);
    }
}
$(document).ready(function () {
    Report.init();
});

