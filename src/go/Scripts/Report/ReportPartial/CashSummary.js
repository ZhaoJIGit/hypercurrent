(function () {
    //
    var CashSummary = (function () {
        //
        var CashSummary = function () {
            //
            var that = this;
            //获取数据的url
            this.loadDataUrl = "/Report/ReportHome/GetCashSummaryReportData";
            //重新查询的触发对象
            this.reloadSelector = "#aReportUpdate";
        };
        //
        return CashSummary;
    })();
    window.CashSummary = CashSummary;
})();