ReportExpanseClaimsDetail = {
    Department: "1",
    Employee: "2",
    Expense: "3",
    init: function () {
        ReportExpanseClaimsDetail.initAction();
        ReportExpanseClaimsDetail.intiSortByOptions();
        ReportExpanseClaimsDetail.initTrack(function () {
            ReportExpanseClaimsDetail.setTrackValue();
        });

        ReportExpanseClaimsDetail.initReportData();
        

    },
    initAction: function () {

        $('#sltSortBy').combobox({
            onChange: function (record) {
                var sortByValue = record;
                ReportExpanseClaimsDetail.loadSortByOption(sortByValue);
            }
        });

        $("#aReportUpdate").click(function () {
            UCReport.reload();
        });

        $("#aBackToReport").click(function () {
            UCReport.backToParentReport();
            return false;
        });
    },
    //初始化跟踪项
    initTrack: function (callback) {
        mAjax.post("/Report/RptExpenseClaims/GetTrack", {}, function (msg) {
            if (msg.length > 0) {

                var dom = $("#divUpdate");
                for (var i = 0 ; i < msg.length; i++) {
                    //获取标题
                    var temp = msg[i];
                    //跟踪项标题
                    var title = temp.MName;

                    var html = "<div class='m-track m-form-item' >";
                    html += "<ul>";
                    html += "<li class='m-bold'>" + HtmlLang.Write(LangModule.Report, "TrackBy", "Filter by track:") + mText.encode(title) + "</li>";
                    html += "<li>";
                    html += "<select class='m-track-option easyui-combobox'  style='width:120px; height:22px;' id='" + temp.MPKID + "'>";
                    html += "<option value='0'>" + HtmlLang.Write(LangModule.Report, "DoNotFilter", "Do not filter") + "</option>"; //不过滤
                    html += "<option value='1'>" + HtmlLang.Write(LangModule.Report, "Unassigned", "Unassigned") + "</option>";    //空值
                    var options = temp.Options;
                    if (options.length > 0) {
                        for (var j = 0; j < options.length; j++) {
                            var option = options[j];
                            html += "<option value='" + option.MEntryID + "'>" + mText.encode(option.MEntryName) + "</option>";
                        }
                    }
                    html += "</select>";
                    html += "</li>";
                    html += "</ul>";
                    html += "</div>";

                    //插入
                    dom.before(html);
                }
                //初始化combobox
                $(".m-track-option").combobox();
            }

            if (callback && $.isFunction(callback)) {
                callback();
            }

        }, false, false, false);
    },
    //初始sortby的子项
    intiSortByOptions: function () {
        var sortByValue = $("#sltSortBy").val();

        ReportExpanseClaimsDetail.loadSortByOption(sortByValue);

    },
    //加载sortby的子项，联动
    loadSortByOption: function (value) {
        var sltDom = "sltSortByOptions";
        //需要把原来的信息清空掉
        $(sltDom).combobox("clear");
        //获取url的逻辑
        var url = "";
        switch (value) {
            case ReportExpanseClaimsDetail.Employee:
                //employess需要做特殊处理
                url = "/BD/Employees/GetBDEmployeesList?filter=";
                $("#" + sltDom).combobox({
                    valueField: 'MItemID',
                    textField: 'MName'
                });
                ReportExpanseClaimsDetail.initComboboxData(sltDom, url, value);
                break;
            case ReportExpanseClaimsDetail.Expense:
                url = "/BD/ExpenseItem/GetExpenseItemList";
                $("#" + sltDom).combobox({
                    valueField: 'MItemID',
                    textField: 'MName',
                    onLoadSuccess: function () {
                        //$(this).combobox('setText', all);
                    }
                });
                ReportExpanseClaimsDetail.initComboboxData(sltDom, url, value);
                break;
        }
    },
    //初始化子下拉框，selector：下拉框ID ， url:数据地址，type：类型
    initComboboxData: function (selector, url, type) {
        var all = HtmlLang.Write(LangModule.Report, "ALL", "All");
        mAjax.post(url, {}, function (msg) {
            if (msg) {
                var data = msg;
                if (type == ReportExpanseClaimsDetail.Employee || type == ReportExpanseClaimsDetail.Expense) {
                    var itemArray = new Array();

                    var itemAll = {};
                    itemAll.MItemID = "";
                    itemAll.MName = all;
                    itemArray.push(itemAll);

                    if (data && data.length > 0) {
                        for (var i = 0 ; i < data.length; i++) {
                            var item = {};
                            item.MItemID = data[i].MItemID;
                            item.MName = data[i].MName;
                            itemArray.push(item);
                        }
                    }
                    data = itemArray;
                }

                $("#" + selector).combobox("loadData", data);
            }
        }, null, true, true);
    },
    initReportData: function () {
        var opts = {};
        opts.url = "/Report/RptExpenseClaimDetail/GetReportData";
        opts.initFilter = function () {
            
            
        };

        opts.getFilter = function () {
            var obj = {};
            obj.StartDate = $("#StartDate").datebox('getValue');
            obj.EndDate = $("#EndDate").datebox('getValue');
            obj.StatisticsType = $("#sltSortBy").combobox('getValue');
			obj.StatisticsFieldOptionIds = $("#sltSortByOptions").combobox("getValue").split(',');

            var trackIds = new Array();

            $(".m-track").each(function () {
                var trackId = $(this).attr("id");
                var trackOptionId = $(".m-track-option", this).combobox("getValue");
                trackIds.push(trackId + "-" + trackOptionId);
            });

            obj.TrackIds = trackIds;

            obj.MReportID = UCReport.ReportID;

            obj.IsReload = UCReport.options.IsReload;

            return obj;
        }
        UCReport.init(opts);
    },
    getParantFilter: function () {
        var parentFilterString = $("#hidReportFilter").val();

        if (!parentFilterString) {
            return;
        }

        var parentFilter = eval("(" + parentFilterString + ")");

        return parentFilter;
    },
    setTrackValue: function () {
        var parentFilter = ReportExpanseClaimsDetail.getParantFilter();

        if (parentFilter && parentFilter.TrackIds) {
            var trackIdList = parentFilter.TrackIds.split(',');

            for (var i = 0 ; i < trackIdList.length; i++) {
                var trackIdString = trackIdList[i];

                if (trackIdString) {
                    var trackKeyList = trackIdString.split('-');
                    //小于2的只有跟踪大项，没有跟踪子项的
                    if (!trackKeyList || trackKeyList.length < 2) {
                        continue;
                    }

                    var trackId = trackKeyList[0];
                    var trackOptionId = trackKeyList[1];

                    $("#" + trackId).combobox("setValue", trackOptionId);
                }
            }
        }
    }
}

$(document).ready(function () {
    ReportExpanseClaimsDetail.init();
});