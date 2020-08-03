/// <reference path="../RptUC/UCReport.js" />
var MyReport = {
    dateList: null,
    glBeginDate: $("#hideGLBeginDate").val(),
    init: function () {
        $("#divReportDetail").removeClass("report-content-gl").addClass("report-content-gl");
        var opts = {};
        opts.url = "/Report/RptIncomeStatement/GetReportData";
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

            var startDate = $("#selFromMonthlyDate").combobox("getText");

            var endDate = $("#selToMonth").combobox("getText");

            if (!startDate || ($("#selMType").combobox("getValue") == "TimeInterval" && !endDate)) {
                //var tips = HtmlLang.Write(LangModule.Report, "DateTiemIsRequired", "日期为必填项！");
                //$.mDialog.alert(tips);
                return;
            }

            UCReport.reload();
        });

        $("#selComopareSpan").combobox({
            onChange: function (newValue, oldValue) {
                //如果取值范围是本年数，需要控制一下日期，
                
                var startDateComboboxDate = $("#selFromMonthlyDate").combobox("getData");

                if (MyReport.dateList == null) {
                    MyReport.dateList = startDateComboboxDate;
                }

                if (!newValue || newValue == 0) {
                    $("#selFromMonthlyDate").combobox("loadData", MyReport.dateList);

                    $("#selToMonth").combobox("loadData", MyReport.dateList);

                    $("#selFromMonthlyDate").combobox("enable");
                    $("#selToMonth").combobox("enable");
                } else {
                    var compareDate = $.mDate.parse(MyReport.glBeginDate);

                    compareDate.setFullYear(compareDate.getFullYear()+1);

                    //过滤后的日期
                    var filterDateList = new Array();

                    for (var i = 0 ; i < MyReport.dateList.length; i++) {
                        
                        var tempDate = $.mDate.parse(MyReport.dateList[i].value);

                        if (tempDate < compareDate) {
                            continue;
                        }

                        filterDateList.push(MyReport.dateList[i]);
                    }

                    $("#selFromMonthlyDate").combobox("loadData", filterDateList);

                    $("#selToMonth").combobox("loadData", filterDateList);

                    if (!filterDateList || filterDateList.length == 0) {
                        $("#selFromMonthlyDate").combobox("disable");
                        $("#selToMonth").combobox("disable");
                    } else {
                        $("#selFromMonthlyDate").combobox("enable");
                        $("#selToMonth").combobox("enable");
                    }
                }

            }
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