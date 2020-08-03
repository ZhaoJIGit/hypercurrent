
var PurchaseIndex = {
    init: function () {
        PurchaseIndex.initToolbar();
        PurchaseIndex.InitIncomingChart();
        PurchaseIndex.clickAction();
        PurchaseIndex.InitSpanIcon();
    },
    initToolbar: function () {
        $("#aNewInvoice").click(function () {
            mWindow.reload("/IV/Bill/BillEdit");
        });

        $("#aSearchInvoice").click(function () {
            var param = $(".m-adv-search").mFormGetForm();
            var filter = $.toJSON(param);
            mWindow.reload("/IV/Bill/BillList?filter=" + filter);
            return false;
        });
    },
    InitSpanIcon: function () {
        $("td a span").hide();
    },
    InitIncomingChart: function () {
        //动态数据：data,labels,tip,coordinate
        mAjax.post(
            "/IV/Invoice/GetInComingChartData",
            { Type: "'Invoice_Purchase','Invoice_Purchase_Red'" },
            function (jsonData) {
                if (jsonData != null) {
                    PurchaseIndex.fillStackedData(jsonData);
                }
            });
    },
    GetOwingMostChart: function () {
        mAjax.post(
            "/IV/Invoice/GetOwingMostChartData",
            { Type: "'Invoice_Purchase','Invoice_Purchase_Red'" },
            function (jsonData) {
                if (jsonData != null) {
                    PurchaseIndex.fillPieData(jsonData);
                }
            });
    },
    /*用来做渐变的颜色*/
    chartColors: ['#f0c0aa', '#f2a07c', '#f38a5b', '#fa7b43', '#f96422'],
    /*获取渐变色的数组*/
    getGradientColors: function (dataCount) {
        //声称渐变色
        return mColor.Gradient(PurchaseIndex.chartColors[0], PurchaseIndex.chartColors[PurchaseIndex.chartColors.length - 1], dataCount);
    },
    /*给每个data的color赋值*/
    setDataColor: function (data) {
        //
        var colors = PurchaseIndex.getGradientColors(data.length);
        //每个data都赋值
        for (var i = 0; i < data.length ; i++) {
            //赋值
            data[i].color = colors[i];
        }
        return data;
    },
    clickAction: function () {
        $("#show_pie").click(function () {
            PurchaseIndex.GetOwingMostChart();
            $("#show_pie").addClass("active");
            $("#show_list").removeClass("active");
            $("#divSupplierMostChart").show();
            $("td a span").show();
            $('td[class$="red"]').hide();
            $("tbody tr td").css("padding", "0 0 0 20px");
            $("#divSupplierMostChart").css("width", "160px");
            $("#divSupplierMostChart").css("float", "left");
            $("#divSupplierMostList").css("width", "330px");
            $("#divSupplierMostList").css("float", "left");
        });
        $("#show_list").click(function () {
            $("#show_list").addClass("active");
            $("#show_pie").removeClass("active");
            $("#divSupplierMostChart").hide();
            $("td a span").hide();
            $('td[class$="red"]').show();
            $("tbody tr td").css("padding", "0 0 0 5px");
            $("#divSupplierMostChart").css("float", "none");
            $("#divSupplierMostList").css("width", "100%");
            $("#divSupplierMostList").css("float", "none");
        });
    },
    fillStackedData: function (jsonData) {
        if (jsonData.data.length == 0) {
            return;
        }
        data = jsonData.data;
        labels = jsonData.labels;
        scalSpace = jsonData.scalSpace;
        new iChart.ColumnStacked2D({
            render: 'divUpcomingChart',
            data: PurchaseIndex.setDataColor(data),
            sub_option: {
                label: false
            },
            labels: labels,
            border: 0,
            decimalsnum: 2,
            margin: "0px auto",
            width: 450,
            height: 150,
            label: { color: '#7c8490', font: 'Arial', fontsize: 12 },
            default_mouseover_css: false,
            tip: {
                enable: true,
                listeners: {
                    //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                    parseText: function (tip, name, value, text, i) {
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].name == name) {
                                return "<span class=\"m-chart-font-blue14\">" + mText.encode(name) + "</span><br />" +
                                        "<span class=\"m-chart-font-gray\">" + mText.encode(data[j].MChartFirstName) + " " + mText.encode(data[j].MChartLastName) + "</span><br />" +
                                        "<span class=\"m-chart-font-blueBold16\">" + value + " due</span>";
                            }
                        }

                    }
                }
            },
            coordinate: {
                background_color: '#fefefe',
                scale: [{
                    position: 'left',
                    start_scale: 0,
                    scale_space: scalSpace,
                    end_scale: scalSpace * 3,
                    listeners: {
                        parseText: function (t, x, y) {
                            return { text: t }
                        }
                    }
                }]
            }
        }).draw();
    },
    fillPieData: function (jsonData) {
        if (jsonData.length == 0) {
            return;
        }
        data = jsonData;
        new iChart.Pie2D({
            render: 'divSupplierMostChart',
            data: PurchaseIndex.setDataColor(data),
            decimalsnum: 2,
            width: 150,
            height: 150,
            radius: 140,
            border: 0,
            default_mouseover_css: false,
            align: 'left',
            background_color: null,
            sub_option: {
                label: false
            },
            tip: {
                enable: true,
                listeners: {
                    //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                    parseText: function (tip, name, value, text, i) {
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].name == name) {
                                var re =
                                    "<span class=\"m-chart-font-blue14\">" + mText.encode(name) + "</span><br />" +
                                    "<span class=\"m-chart-font-blue14\">" + mText.encode(data[j].MChartFirstName) + " " + mText.encode(data[j].MChartLastName) + "</span><br />" +
                                    "<span class=\"m-chart-font-blueBold16\">" + mText.encode(value) + " due</span>";
                                if (data[j].MOverDue > 0) {
                                    re += "<br /><span class=\"m-chart-font-red\">" + data[j].MOverDue + " overdue</span>";
                                }
                                return re;
                            }
                        }

                    }
                }
            },
            listeners: {
                mouseover: function (th, e) {

                }
            }

        }).draw();
    }
}
$(document).ready(function () {
    PurchaseIndex.init();
});