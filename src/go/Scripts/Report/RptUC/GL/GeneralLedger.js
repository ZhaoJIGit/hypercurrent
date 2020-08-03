/// <reference path="../RptUC/UCReport.js" />
var GeneralLedger = {
    baseParams: null,
    currencyList: null,
    init: function () {
        GeneralLedger.initAction();
        GeneralLedger.loadReportData();

        GeneralLedger.changeCombo();
    },
    changeCombo: function () {

        $("#cbxStartPeriod").combobox({
            onChange: function (n, o) {
                Megi.dateCompare(n, $("#cbxEndPeriod"), 1);
            }
        });
        $("#cbxEndPeriod").combobox({
            onChange: function (n, o) {
                Megi.dateCompare(n, $("#cbxStartPeriod"), 2);
            }
        });

    },
    initAction: function () {
        $("#aReportUpdate").click(function (e, baseFilter) {
            if (!$("#cbxStartPeriod").combobox("getValue") ||
                !$("#cbxEndPeriod").combobox("getValue")) {
                return;
            }

            UCReport.reload();
        });
    },
    loadReportData: function () {
        var opts = {};
        opts.autoFillCell = false;
        opts.url = "/Report/RptGeneralLedger/GetReportData";

        opts.getFilter = function () {
            return GeneralLedger.getFilter();
        };
        opts.callback = function () {
            GLReportBase.initFilter();

            $("table", "#divReportDetail").mTableResize({
                forceFit: false
            });

            //进入科目余额表事件
            $(".click-a").each(function () {
                $(this).off("click").on("click", function () {
                    var reportId = $("#hidReportID").val();
                    var url = $(this).attr("url");

                    var index = url.indexOf("?");

                    var paramString = url.substring(index);
                    var urlString = url.substring(0, index);

                    var reg = new RegExp("(^|&)accountId=([^&]*)(&|$)", "i");
                    var r = paramString.substr(1).match(reg);
                    var accountId = unescape(r[2]);

                    var reportTitle = $(this).attr("reportname");

                    url = urlString + "?filter=" + encodeURIComponent('{"MAccountID":"' + accountId + '","NavReportID":"' + reportId + '"}');

                    $.mTab.addOrUpdate(reportTitle, url, false, true, false, true);

                });
            });
        };
        UCReport.init(opts);
    },
    getFilter: function () {
        var param = {};
        param.MReportID = UCReport.ReportID;
        param.IsReload = UCReport.options.IsReload;

        param = $.extend(param, GLReportBase.getFilter());

        return param;
    }
}

$(document).ready(function () {
    GeneralLedger.init();
});