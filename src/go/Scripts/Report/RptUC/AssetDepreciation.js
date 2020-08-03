var MyReport = {
    init: function () {
        var opts = {};
        opts.url = "/Report/RPTAssetDepreciation/GetReportData";
        opts.getFilter = function () {
            return MyReport.getFilter();
        };
        opts.callback = function () {
            $("#divReportDetail").find(".tb-header>td").eq(0).css("width", "150px");
            $("table", "#divReportDetail").mTableResize({
                forceFit: false
            });
        };
        UCReport.init(opts);
        $("#aReportUpdate").click(function () {
            UCReport.reload();
        });
    },
    getFilter: function () {
        var param = {};
        param = $.extend(param, DepreciationReportBase.getFilter());
        return param;
    }
}

$(document).ready(function () {
    MyReport.init();
});