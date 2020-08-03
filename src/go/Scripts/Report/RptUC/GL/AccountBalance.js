/// <reference path="../RptUC/UCReport.js" />
var AccountBalance = {
    currencyList: null,
    init: function () {
        AccountBalance.loadReportData();
        AccountBalance.changeCombo();

        AccountBalance.initAction();

        GLReportBase.reportType = "41";
    },
    //统一的参数
    baseParams: null,
    initAction: function () {
        $("#aReportUpdate").click(function (e, baseFilter) {
            if (!$("#cbxStartPeriod").combobox("getValue") ||
                !$("#cbxEndPeriod").combobox("getValue")) {
                return;
            }

            UCReport.reload();
        });
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
    loadReportData: function () {
        var opts = {};
        opts.url = "/Report/RptAccountBalance/GetReportData";
        opts.getFilter = function () {
            return AccountBalance.getFilter();
        };

        opts.callback = function () {
            $("#divReportDetail").find(".tb-header>td").eq(0).css("width", "150px");
            GLReportBase.initFilter();

            $("table", "#divReportDetail").mTableResize({
                forceFit: false
            });

            var filter = AccountBalance.getFilter();

            if (filter && filter.IncludeCheckType) {
                UCReport.ExpandAll();
            }


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

            $(".tb-item , .tb-subitem").each(function () {
                $(this).off("click").on("click", function (e) {

                    if (!$(e.srcElement).hasClass("exp-col")) {
                        //点击的+ -不做处理，UCReport.js有统一的处理
                        var isExpand = $(this).children(".item-tree-filed").find(".exp-col").hasClass("collapse");
                        var rowIndex = $(this).attr("rowindex");
                        //如果是展开，它的下级要隐藏
                        var childrenDom = $("#divReportDetail").find("tr[rowindex^='" + rowIndex + "']");
                        if (!childrenDom || childrenDom.length == 0) {
                            return;
                        }
                        if (isExpand) {
                            $(childrenDom).each(function (index) {
                                //自身不能操作
                                if (index != 0) {
                                    $(this).hide();
                                }
                                $(this).find(".exp-col").removeClass("collapse").addClass("expand");

                            });

                        } else {
                            //显示下级
                            $(childrenDom).each(function (index) {
                                if (index != 0) {
                                    $(this).show();
                                }
                                $(this).find(".exp-col").removeClass("expand").addClass("collapse");
                            });
                        }
                    }
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
    AccountBalance.init();
});
