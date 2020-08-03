ReportExpanseClaims = {
    Department: "1",
    Employee: "2",
    Expense: "3",
    init: function () {
        ReportExpanseClaims.initAction();
        ReportExpanseClaims.intiSortByOptions(function () {
        });
        ReportExpanseClaims.initTrack(function () {
            ReportExpanseClaims.setTrackValue();
        });
        ReportExpanseClaims.initReportData();

    },
    initAction: function () {

        $('#sltSortBy').combobox({
            onChange: function (record) {
                var sortByValue = record;
                ReportExpanseClaims.loadSortByOption(sortByValue);
            }
        });

        $("#aReportUpdate").click(function () {
            if (!$("[comboname='MEndDate']").combobox("getValue") ||
                !$("[comboname='SortBy']").combobox("getValue")) {
                return;
            }
            UCReport.reload();
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

                    var html = "<div class='m-track m-form-item' id='" + temp.MPKID + "'>";
                    html += "<ul>";
                    html += "<li class='m-bold'>" + HtmlLang.Write(LangModule.Report, "TrackBy", "Filter by track:") + mText.encode(title) + "</li>";
                    html += "<li>";
                    html += "<select class='m-track-option easyui-combobox' style='width:120px; height:22px;' >";
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

                $(".m-track-option").combobox();

                if (callback && $.isFunction(callback)) {
                    callback();
                }

            }
        }, null, true, true);
    },
    //初始sortby的子项
    intiSortByOptions: function (callback) {
        var sortByValue = $("#sltSortBy").val();

        ReportExpanseClaims.loadSortByOption(sortByValue);

        if (callback && $.isFunction(callback)) {
            callback();
        }

    },
    //加载sortby的子项，联动
    loadSortByOption: function (value) {
        var sltDom = "sltSortByOptions";
        //需要把原来的信息清空掉
        $(sltDom).combobox("clear");

        //获取url的逻辑
        var url = "";

        switch (value) {
            case ReportExpanseClaims.Employee:
                //employess需要做特殊处理
                url = "/BD/Employees/GetBDEmployeesList?filter=";
                $("#" + sltDom).combobox({
                    valueField: 'MItemID',
                    textField: 'MName'
                });
                ReportExpanseClaims.initComboboxData(sltDom, url, value);
                break;
            case ReportExpanseClaims.Expense:
                url = "/BD/ExpenseItem/GetParentExpenseItemList";
                $("#" + sltDom).combobox({
                    valueField: 'MItemID',
                    textField: 'MName',
                    onLoadSuccess: function () {
                        //$(this).combobox('setText', all);
                    }
                });

                ReportExpanseClaims.initComboboxData(sltDom, url, value);
                break;
        }
    },
    //初始化子下拉框，selector：下拉框ID ， url:数据地址，type：类型
    initComboboxData: function (selector, url, type) {
        var all = HtmlLang.Write(LangModule.Report, "ALL", "All");
        mAjax.post(url, {}, function (msg) {
            if (msg) {
                var data = msg;
                if (type == ReportExpanseClaims.Employee || type == ReportExpanseClaims.Expense) {
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
        }, null , true , true);
    },
    initReportData: function () {
        var opts = {};
        opts.hasSubReport = true;

        opts.url = "/Report/RptExpenseClaims/GetReportData";
        opts.getFilter = function () {
            var expenseObj = {};

            expenseObj.MEndDate = $("#sltDate").combobox('getValue');
			expenseObj.SortBy = $("#sltSortBy").combobox('getValue');
			expenseObj.SortByValue = $("#sltSortByOptions").combobox("getValue").split(',');
            expenseObj.MonthShowType = $("#sltMonthFamtter").combobox("getValue");

            var trackIds = ReportExpanseClaims.getTrack();

            expenseObj.TrackIds = trackIds;

            expenseObj.MReportID = UCReport.ReportID;

            expenseObj.IsReload = UCReport.options.IsReload;

            return expenseObj;
        };

        opts.callback = function (msg) {

            if (!msg.Filter) {
                return;
            }

            ReportExpanseClaims.setTrackValue(msg.Filter);
        }

        UCReport.init(opts);
    },
    getTrack: function () {
        var trackIds = new Array();

        $(".m-track").each(function () {
            var trackId = $(this).attr("id");
            var trackOptionId = $(".m-track-option", this).combobox("getValue");
            trackIds.push(trackId + "-" + trackOptionId);
        });

        return trackIds;
    },
    setTrackValue: function (filter) {
        //var filter = ReportExpanseClaims.getFilter();

        if (filter && filter.TrackIds && filter.TrackIds.length > 0) {
            var trackIdList = filter.TrackIds;

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

                    $("#" + trackId).find(".m-track-option").combobox("setValue", trackOptionId);
                }
            }
        }
    },
    getFilter: function () {
        var parentFilterString = $("#hidReportFilter").val();

        if (!parentFilterString) {
            return;
        }

        var parentFilter = eval("(" + parentFilterString + ")");

        return parentFilter;
    },
}

$(document).ready(function () {
    ReportExpanseClaims.init();
});