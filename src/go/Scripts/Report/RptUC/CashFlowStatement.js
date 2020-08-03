
var MyReport = {
    init: function () {
        $("#divReportDetail").removeClass("report-content-gl").addClass("report-content-gl");
        var opts = {};
        opts.url = "/Report/RptCashFlowStatement/GetReportData";
        opts.callback = function (msg) {

            if (msg.Filter.MType == "Monthly") {
                $("#liFrom").hide();
                $("#divToDate").hide();
                $("#liDate").show();
            } else if (msg.Filter.MType == "TimeInterval") {
                $("#liDate").hide();
                $("#liFrom").show();
                $("#divToDate").show();
            }
        };
        UCReport.init(opts);

        $("#aReportUpdate").click(function () {
            var type = $('#selMType').val();
            if (type == "Monthly") {
                if (!$("#selFromMonthlyDate").combobox("getValue")) {
                    return;
                }
            }
            else if (type == "TimeInterval") {
                if (!$("#selFromMonthlyDate").combobox("getValue") ||
                    !$("#selToMonth").combobox("getValue")) {
                    return;
                }
            }
            UCReport.reload();
        });
    },
    changeType: function (rec) {
        if (rec.value == "Monthly") {
            $("#liFrom").hide();
            $("#divToDate").hide();
            $("#liDate").show();
        } else if (rec.value == "TimeInterval") {
            $("#liDate").hide();
            $("#liFrom").show();
            $("#divToDate").show();
        }
    }
}

$(document).ready(function () {
    MyReport.init();
});