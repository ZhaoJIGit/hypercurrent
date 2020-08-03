
var PayRunList = {
    isIE9OrFFRefresh: (location.href.lastIndexOf("?refresh=true") != -1 && ($.browser.mozilla || $.browser.msie != undefined && $.browser.msie && parseInt($.browser.version) <= 9)),
    currentUrl: "/PA/SalaryPayment/PayRunList",
    CurrentType: 0,
    init: function () {
        PayRunBase.dgId = "#payRunList";
        $(".iv2-tab-links li").css("width", "18%");
        PayRunList.initSearchControl();
        PayRunList.initTab();
        PayRunList.initChartWidth();
        PayRunList.bindAction();
    },
    initTab: function (idx) {
        if (!idx) {
            idx = 0;
        }
        $(".m-extend-tabs").tabsExtend({
            initTabIndex: idx,
            onSelect: function (index) {
                PayRunList.CurrentType = index;
                PayRunList.bindGrid(index);
                //刷新Tab合计数据
                //IVBase.tabSelRefreshTitle(1);
                $(window).resize();
            }
        });
    },
    initSearchControl: function () {
        var filter = Megi.request("filter");
        if (filter != "") {
            $(".m-adv-search-btn").hide();
            $(".m-adv-search").show();
            filter = eval("(" + filter + ")");
            $("body").mFormSetForm(filter);
        }
    },
    bindAction: function () {
        $("#divCopy").off('click').on('click', function () {
            PayRunList.selectSalaryMonth(1);
        });
        $("#aCreateBy,#divNew").off('click').on('click', function () {
            PayRunList.selectSalaryMonth(2);
        });
        $("#aImport").off('click').on('click', function () {
            ImportBase.showImportBox("/BD/Import/Import/PayRun", HtmlLang.Write(LangModule.PA, "ImportSalaryList", "Import Salary List"), 830, 500);
        });
        $("#aSearch").off('click').on('click', function () {
            PayRunList.bindGrid(PayRunList.CurrentType);
        });
    },
    selectSalaryMonth: function (from) {
        SalaryMonthSelect.initData();
        $("#hidFrom").val(from);
        var isCopy = from == 1;
        $.mDialog.show({
            mTitle: (isCopy ? HtmlLang.Write(LangModule.PA, "CopyPayRunFromPrevious", "Copy Pay Run from previous") : HtmlLang.Write(LangModule.PA, "NewPayRun", "New Pay Run")),
            mWidth: 438,
            mHeight: 280,
            mDrag: "mBoxTitle",
            mShowbg: true,
            mContent: "id:divSelect"
        });
    },
    reload: function () {
        mWindow.reload("/PA/SalaryPayment/PayRunList/" + PayRunList.CurrentType);
    },
    setListWidth: function () {
        var w = $(".m-imain-content").width();
        if (w <= 0) {
            w = sessionStorage.ListWidth;
        }
        else {
            sessionStorage.ListWidth = w;
        }
        $("#divPAList").css({ "width": w + "px" });
        try {
            $("#payRunList").datagrid('resize', {
                width: $("#divPAList").width()
            });
        } catch (exc) { }
    },
    reloadByRefresh: function () {
        if (PayRunList.isIE9OrFFRefresh) {
            var curIframe = $.mTab.getCurrentIframe();
            var dgHeader = $("#divPAList .datagrid-view2 .datagrid-header");
            if (curIframe && curIframe[0].src.lastIndexOf(PayRunList.currentUrl) != -1 && (dgHeader.height() == 0 || $(".loadmask", curIframe.contents()).is(":visible"))) {
                $('#payRunList').datagrid('reload');
                window.clearInterval(PayRunBase.interval);
                PayRunBase.interval = null;

                PayRunList.isIE9OrFFRefresh = false;
                if (typeof (history.pushState) != "undefined") {
                    var obj = { Title: HtmlLang.Write(LangModule.PA, "PayRun", "Pay Run"), Url: PayRunList.currentUrl };
                    history.pushState(obj, obj.Title, obj.Url);
                }
            }
        }
    },
    bindGrid: function (type) {

        switch (type) {
            case 0:
                $('.m-tab-toolbar .left .m-tool-bar-btn').hide();
                $('#divSearch').css("display", "none");
                $('#divHome').show();
                $('#divPAList').hide();
                if (PayRunList.isIE9OrFFRefresh && PayRunBase.interval == null) {
                    window.clearInterval(PayRunBase.interval);
                    PayRunBase.interval = setInterval(PayRunList.reloadByRefresh, 500);
                }
                PayRunList.InitPayrunChart();
                break;
            default:
                $('#divHome').hide();
                $('#divPAList').show();
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#divSearch').css("display", "block");
                var columnList = [{
                    title: HtmlLang.Write(LangModule.PA, "Month", "Month"), field: 'MDate', width: 80, align: 'center', sortable: true, formatter: function (value, row, index) {
                        return "<a href=\"javascript:void(0);\" onclick=\"PayRunList.editPayRun('" + row.MID + "','" + row.MFormatDate + "');" + "\" >" + row.MFormatDate + "</a>"
                    }
                }];
                columnList = columnList.concat(PayRunBase.salaryItemColumnList());
                var removeIdx = -1;
                $.each(columnList, function (idx, obj) {
                    if (obj.field == "SSWithHFEmployee") {
                        obj.title = HtmlLang.Write(LangModule.PA, "TotalSSAndHF", "Total SS & HF");
                        obj.field = "SSWithHFTotal";
                        obj.width = 106;
                    }
                    if (obj.field == "SSWithHFEmployer") {
                        removeIdx = idx;
                    }
                });
                columnList.splice(removeIdx, 1);
                columnList.push({
                    title: HtmlLang.Write(LangModule.PA, "Status", "Status"), field: 'MStatus', width: 80, sortable: true, align: 'center', formatter: function (value, row, index) {
                        var status = Number(value);
                        switch (status) {
                            case PayRunBase.Status.Draft:
                                return HtmlLang.Write(LangModule.PA, "Draft", "Draft");
                            case PayRunBase.Status.AwaitingPayment:
                                return "<div style='white-space: normal; line-height: 18px;'>" + HtmlLang.Write(LangModule.IV, "AwaitingPayment", "Awaiting Payment") + "</div>";
                            case PayRunBase.Status.Paid:
                                return HtmlLang.Write(LangModule.IV, "Paid", "Paid");
                        }
                    }
                });
                var allFields = [];
                $.each(columnList, function (i, column) {
                    allFields.push(column.field);
                });
                var status = (type < 3 ? type - 1 : type);

                //是否有未分配的宽度
                var haveUnAssignWidth = PayRunBase.haveUnAssignWidth(columnList);

                var queryParam = {}
                if ($(".m-adv-search").css("display") == "block") {
                    queryParam = $("body").mFormGetForm();
                }
                queryParam.status = status;
                queryParam.StartDate = $("#txtStartDate").val();
                queryParam.EndDate = $("#txtEndDate").val();

                //横向滚动条左边固定列
                var leftFixedFields = "MDate,BaseSalary".split(',');//,SalaryAfterPIT,TotalSalary
                var leftFrozenColumns = [];
                var removeIdx = [];

                //设置左边固定列
                var allFields = [];
                $.each(columnList, function (i, column) {
                    allFields.push(column.field);
                    if ($.inArray(column.field, leftFixedFields) != -1) {
                        leftFrozenColumns.push(column);
                        removeIdx.push(i);
                    }
                });
                //设置右边滚动列
                removeIdx = removeIdx.reverse();
                $.each(removeIdx, function (i, idx) {
                    columnList.splice(idx, 1);
                });
                
                var listBodyHeight = $(".m-imain").height() - $(".m-extend-tabs").outerHeight() - $(".m-tab-toolbar").outerHeight() - 50;
                Megi.grid("#payRunList", {
                    resizable: true,
                    //显示纵向滚动条
                    scrollY: true,
                    lines: true,
                    //如果有未分配宽度，则自动分配列宽
                    fitColumns: haveUnAssignWidth,
                    auto: haveUnAssignWidth,
                    //设置表体高度
                    height: listBodyHeight,
                    url: "/PA/SalaryPayment/GetPayRunListPage",
                    queryParams: queryParam,
                    frozenColumns: [leftFrozenColumns],
                    pagination: true,
                    columns: [columnList],
                    //onBeforeLoad: function (param) {
                    //    var tmp = param;
                    //},
                    onLoadSuccess: function (data) {
                        PayRunBase.hideDisableColumn("payRunList", data, allFields);
                    }
                });
                break;
        }

    },
    editPayRun: function (id, date) {
        $.mTab.addOrUpdate(Megi.getCombineTitle([date, HtmlLang.Write(LangModule.PA, "SalaryList", "Salary List")]), "/PA/SalaryPayment/SalaryPaymentList/" + id)
    },
    InitPayrunChart: function (data) {
        //动态数据：data,labels,tip,coordinate
        mAjax.post(
            "/PA/SalaryPayment/GetPayRunChartData",
            { payRunListData: data },
            function (jsonData) {
                if (jsonData != null && jsonData != "") {
                    PayRunList.fillStackedData(JSON.parse(jsonData));
                }
            });
    },
    initChartWidth: function () {
        //var w = $(".m-imain").innerWidth() - 60;
        //var h = $(".m-imain").height() - $("#divPAList").height() - 100;
        //var chart = $("#payRunChart");
        //chart.push("column_width", null);//设置为null则每次重新计算柱子宽度
        //chart.resize(w, h);
    },
    /*给每个data的color赋值*/
    setDataColor: function (data) {
        /*用来做渐变的颜色*/

        var chartColors = ['#f69679', '#F5AF9A', '#F4BFAF', '#F4D2C9', '#FDFAF9'];
        //生成渐变色
        var colors = mColor.Gradient(chartColors[0], chartColors[chartColors.length - 1], data.length);
        for (var i = 0; i < data.length ; i++) {
            data[i].color = colors[i];
        }
        return data;
    },
    fillStackedData: function (jsonData) {
        if (jsonData.data.length == 0) {
            return;
        }
        var data = jsonData.data;
        new iChart.ColumnStacked2D({
            render: 'payRunChart',
            data: PayRunList.setDataColor(data),
            sub_option: {
                label: false
            },
            labels: jsonData.labels,
            border: 0,
            decimalsnum: 2,
            width: 500,
            height: 300,
            label: { color: '#7c8490', font: 'Arial', fontsize: 12 },
            default_mouseover_css: false,
            background_color: null,
            tip: {
                enable: true,
                listeners: {
                    //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                    parseText: function (tip, name, value, text, i) {
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].name == name) {
                                for (var k = 0; k < data[j].value.length; k++) {
                                    if (data[j].value[k] == value) {
                                        return "<span class=\"m-chart-font-blue14\">" + mText.encode(name) + "</span><br />" +
                                    "<span class=\"m-chart-font-blueBold16\">" + Megi.Math.toMoneyFormat(value) + "</span>";
                                    }
                                }
                            }
                        }

                    }
                }
            },
            coordinate: {
                width: "100%",
                height: "85%",
                background_color: null,
                axis: {
                    width: [0, 0, 1, 0],
                    color: '#666666'
                },
                scale: {
                    scale_enable: false,
                    scale_share: 0,
                }
            },

        }).draw();

        //设置柱状图容器宽度
        $("#payRunChart div:first").width(560);
    },

    fillPieData: function (jsonData) {
        if (jsonData.length == 0) {
            return;
        }
        var data = jsonData;
        new iChart.Pie2D({
            render: 'divCustomerMostChart',
            data: InvoiceList.setDataColor(data),
            decimalsnum: 2,
            width: 280,
            height: 150,
            radius: 140,
            border: 0,
            default_mouseover_css: false,
            align: 'left',
            background_color: null,
            tip: {
                enable: true,
                listeners: {
                    //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                    parseText: function (tip, name, value, text, i) {
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].name == name) {
                                for (var k = 0; k < data[j].value.length; k++) {
                                    if (data[j].value[k] == value) {
                                        return "<span class=\"m-chart-font-blue14\">" + mText.encode(name) + "</span><br />" +
                                    "<span class=\"m-chart-font-blueBold16\">" + Megi.Math.toMoneyFormat(value) + "</span>";
                                    }
                                }
                            }
                        }

                    }
                }
            }

        }).draw();
    }


}

$(document).ready(function () {
    PayRunList.init();

    //$(window).resize(function () {
    //    PayRunList.setListWidth();
    //    PayRunList.initChartWidth();
    //});
});