/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var SaleIndex = {
    init: function () {
        SaleIndex.initToolbar();
        SaleIndex.InitIncomingChart();
        SaleIndex.clickAction();
        SaleIndex.InitSpanIcon();
    },
    initToolbar: function () {
        $("#aNewInvoice").click(function () {
            mWindow.reload("/IV/Invoice/InvoiceEdit");
        });
        $("#aSendStatements").click(function () {
            mWindow.reload("/IV/Sale/Statements");
            return false;
        });
        $("#aSearchInvoice").click(function () {
            var param = $(".m-adv-search").mFormGetForm();
            var filter = $.toJSON(param);
            mWindow.reload("/IV/Invoice/InvoiceList?filter=" + filter);
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
            { Type: "'Invoice_Sale','Invoice_Sale_Red'" },
            function (jsonData) {
                if (jsonData != null) {
                    SaleIndex.fillStackedData(jsonData);
                }
            });
    },
    /*用来做渐变的颜色*/
    chartColors: ['#c4e9c7', '#8ad68e', '#69bd6c', '#51ae56', '#26a72c'],
    /*获取渐变色的数组*/
    getGradientColors: function (dataCount) {
        //声称渐变色
        return mColor.Gradient(SaleIndex.chartColors[0], SaleIndex.chartColors[SaleIndex.chartColors.length - 1], dataCount);
    },
    /*给每个data的color赋值*/
    setDataColor: function (data) {
        //
        var colors = SaleIndex.getGradientColors(data.length);
        //每个data都赋值
        for (var i = 0; i < data.length ; i++) {
            //赋值
            data[i].color = colors[i];
        }
        return data;
    },
    GetOwingMostChart: function () {
        mAjax.post(
            "/IV/Invoice/GetOwingMostChartData", 
            { Type: "'Invoice_Sale','Invoice_Sale_Red'" }, 
            function (jsonData) {
                if (jsonData != null) {
                    SaleIndex.fillPieData(jsonData);
                }
            });
    },
    clickAction: function () {
        $("#show_pie").click(function () {
            SaleIndex.GetOwingMostChart();
            $("#show_pie").addClass("active");
            $("#show_list").removeClass("active");
            $("#divCustomerMostChart").show();
            $("td a span").show();
            $('td[class$="red"]').hide();
            $("tbody tr td").css("padding", "0 0 0 20px");
            $("#divCustomerMostChart").css("width", "160px");
            $("#divCustomerMostChart").css("float", "left");
            $("#divCustomerMostList").css("width", "330px");
            $("#divCustomerMostList").css("float", "left");
        });
        $("#show_list").click(function () {
            $("#show_list").addClass("active");
            $("#show_pie").removeClass("active");
            $("#divCustomerMostChart").hide();
            $("td a span").hide();
            $('td[class$="red"]').show();
            $("tbody tr td").css("padding", "0 0 0 5px");
            $("#divCustomerMostChart").css("float", "none");
            $("#divCustomerMostList").css("width", "100%");
            $("#divCustomerMostList").css("float", "none");
        });
    },
    fillStackedData: function (jsonData) {
        if (jsonData.data.length == 0) {
            return;
        }
        var data = jsonData.data;
        var labels = jsonData.labels;
        var scalSpace = jsonData.scalSpace;
        new iChart.ColumnStacked2D({
            render: 'divIncomingChart',
            data: SaleIndex.setDataColor(data),
            sub_option: {
                label: false,
                listeners: {
                    click: function (r, e, m) {
                        mWindow.reload("/IV/Sale/ViewStatement?statementType=Outstanding&statementContactID=" + r.get('MContactID'));
                    }
                }
            },
            //labels: ['Older', 'Oct', 'Nov', 'Dec', 'Jan', 'Feb'],
            labels: labels,
            border: 0,
            decimalsnum: 2,
            width: 480,
            height: 150,
            offsetx: 35,
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
                                        return
                                        "<div onclick='mWindow.reload(\"/IV/Sale/ViewStatement?statementType=Outstanding&statementContactID=" + data[j].MContactID + "\")'>"
                                        "<span class=\"m-chart-font-blue14\">" + mText.encode(name) + "</span><br />" +
                                        "<span class=\"m-chart-font-gray\">" + mText.encode(data[j].MChartFirstName) + " " + mText.encode(data[j].MChartLastName) + "</span><br />" +
                                        "<span class=\"m-chart-font-blueBold16\">" + Megi.Math.toMoneyFormat(value) + " " + data[j].MChartDueOrOwing[k]; + "</span>"
                                        + "</div>";
                                    }
                                }
                            }
                        }

                    }
                }
            },
            coordinate: {
                background_color: null,
                scale: [{
                    position: 'left',
                    start_scale: 0,
                    scale_space: scalSpace,
                    end_scale: scalSpace * 3,
                    listeners: {
                        parseText: function (t, x, y) {
                            return { text: Megi.Math.toMoneyFormat(t) }
                        }
                    }
                }]
            },

        }).draw();
    },
    fillPieData: function (jsonData) {
        if (jsonData.length == 0) {
            return;
        }
        var data = jsonData;
        new iChart.Pie2D({
            render: 'divCustomerMostChart',
            data: SaleIndex.setDataColor(data),
            decimalsnum: 2,
            width: 150,
            height: 150,
            radius: 140,
            border: 0,
            default_mouseover_css: false,
            align: 'left',
            background_color: null,
            sub_option: {
                label: false,
                listeners: {
                    click: function (r, e, m) {
                        mWindow.reload("/IV/Sale/ViewStatement?statementType=Outstanding&statementContactID=" + r.get('MContactID'));
                    }
                }
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
                                        "<span class=\"m-chart-font-blueBold16\">" + value + HtmlLang.Write(LangModule.Report, "Due", "Due") + "</span>";
                                if (data[j].MOverDue > 0) {
                                    re += "<br /><span class=\"m-chart-font-red\">" + data[j].MOverDue + HtmlLang.Write(LangModule.Report, "Overdue", "overdue") + " </span>";
                                }
                                return "<div onclick='mWindow.reload(\"/IV/Sale/ViewStatement?statementType=Outstanding&statementContactID=" + data[j].MContactID + "\")'>" +
                                    re + "</div>";
                            }
                        }

                    }
                }
            }

        }).draw();
    }
}
$(document).ready(function () {
    SaleIndex.init();
});