/*
    Dashboard 首页
*/
(function () {
    //常量定义
    var FWDashboardHome = (function () {
        //常量定义
        //银行/销售/采购/费用对应的样式前缀
        var prefixs = ["bank", "sale", "purchase", "expense", "vatFapiao", "incomeFapiao", "vatFapiaoSmart", "incomeFapiaoSmart"];
        //选中日期段类型样式
        var _dateUseSelected = ".date-use-selected";
        //日期类型
        var week = "1", month = "2", quarter = "3", halfYear = "4";
        //用户选择的图类型
        var _chartUseSelected = ".chart-use-selected";
        //日期选择div
        var _bspeDateSelect = ".bspe-date-select";
        //图表类型选择div
        var _bspeChartSelect = ".bspe-chart-select";
        //绘制图形区域
        var _bspeChart = "ChartDiv";
        //数据中心
        var _dataHome = ".data-home";
        //点击数据中心按钮
        var _userDataBtn = ".user-data-btn";
        //点击消息中心
        var _userMsgBtn = ".user-msg-btn";
        //消息中心
        var _messageHome = ".message-home";
        //选中样式
        var _funcSelected = "current";
        //未选中样式
        var _funcUnselected = "";
        //显示数据中心的标题
        var _dataHomeTitle = ".data-home-title";
        //显示消息中心的标题
        var _messageHomeTitle = ".message-home-title";
        //图的类型
        var trendType = "0", columnType = "1", dataType = "2", pieType = "3";

        var textColor = "#444";

        //定义类
        var FWDashboardHome = function (prefix) {
            //
            var that = this;
            //当前的前缀
            this.prefixName = prefix;
            //
            var fontFamily = (top.OrgLang[0].LangID == '0x0009') ? "Arial" : "Microsoft Yahei";
            //计算图形的宽度
            this.getChartWidth = function (selector) {

                if (selector) {
                    var width = $(selector).width();
                    var countWidth = (top.$("body").width() - 63 - 100 - 10 - 1) / 2;
                    return width < countWidth ? countWidth : width;
                } else {
                    var width = $(".dashboard-home").width() - 530;
                    return width > 580 ? 580 : width;
                }
            };
            //计算图形的高度
            this.getChartHeight = function (selector) {
                if (selector && $(selector).attr("height")) {
                    var height = $(selector).attr("height")
                    return height;
                }
                return 180;
            };
            //清除不应该显示的内容
            this.clearSiblings = function (selector) {
                //remove所有不存在的内容
                for (var i = 0; i < prefixs.length ; i++) {
                    //当前前缀
                    var p = prefixs[i];
                    //除了当前的不去掉
                    if (p != this.prefixName) {
                        //remove掉
                        $("div[class^=" + p + "],span[class^=" + p + "],table[class^=" + p + "],a[class^=" + p + "]", selector).remove();
                    }
                    else {
                        //显示
                        $("div[class^=" + p + "],span[class^=" + p + "],table[class^=" + p + "],a[class^=" + p + "]", selector).show();
                    }
                }
            };
            //获取用户选择的日期类型
            this.getDateRange = function (selector) {
                //获取选中的值
                var index = $(_dateUseSelected, selector).attr("index");
                //当天日期
                var today = new Date(mDate.DateNow.getTime());
                //
                var today1 = new Date(mDate.DateNow);
                //根据用户选择的类型
                switch (index) {
                    case week:
                        return [today.addDays(-7).addDays(1), today1];
                    case month:
                        return [today.addMonths(-1).addDays(1), today1];
                    case quarter:
                        return [today.addMonths(-3).addDays(1), today1];
                    case halfYear:
                        return [today.addMonths(-6).addDays(1), today1];
                    default:
                        return [mDate.parse(top.MegiGlobal.MBeginDate), today1];
                }
            };
            //做一个没有数据的图形
            this.initEmptyChart = function (selector, colors, title) {
                var title = title ? title : "" + "[" + HtmlLang.Write(LangModule.Common, "nodata", "No data") + "]";
                var data = [
                            {
                                name: 'Megi',
                                value: [0],
                                color: colors[0],
                                line_width: 1
                            }
                ];

                var chart = new iChart.Area2D(
                    {
                        render: selector,
                        data: data,
                        align: "left",
                        border: {
                            enable: true,
                            width: [0, 0, 0, 0],
                            color: colors[1]
                        },
                        title: {
                            text: title,
                            font: fontFamily,
                            color: textColor
                        },
                        width: that.getChartWidth("#" + selector),
                        height: that.getChartHeight("#" + selector),
                        offsetx: 35,
                        color: colors[2],
                        tip: {
                            enable: true
                        },
                        crosshair: {
                            enable: true,
                            gradient: true,
                            line_color: colors[5],
                            line_width: 1
                        },
                        sub_option: {
                            hollow_inside: false,
                            label: false,
                            point_size: 8
                        },
                        background_color: colors[6],
                        coordinate: {
                            axis: {
                                width: [0, 0, 2, 2],
                                color: colors[7]
                            },
                            background_color: colors[8],
                            height: '80%',
                            grid_color: colors[9],
                            scale: {
                                scale_enable: false,
                                scale_share: 0
                            }
                        }
                    });

                chart.draw();
            };
            //获取用户选择的图表类型
            this.getChartType = function (selector) {
                //获取选中的值
                return $(_chartUseSelected, selector).attr("index");
            };
            //生成趋势图
            this.initTrendChart = function (chartData, selector, colors, title) {
                title = title ? title : "";
                if (!chartData) {
                    //绘制空的图形
                    return that.initEmptyChart(selector, colors, title);
                }
                var data = [
                    {
                        name: null,
                        value: chartData.MValue,
                        color: colors[0],
                        line_width: 1
                    }];

                var chart = new iChart.Area2D(
                    {
                        render: selector,
                        data: data,
                        align: "left",
                        title: {
                            text: title ? title : "",
                            font: fontFamily,
                            color: textColor
                        },
                        border: {
                            enable: true,
                            width: [0, 0, 0, 0],
                            color: colors[1]
                        },
                        width: that.getChartWidth("#" + selector),
                        height: that.getChartHeight("#" + selector),
                        offsetx: 35,
                        color: colors[2],
                        label: { color: colors[3], font: fontFamily, fontsize: 12 },
                        tip: {
                            enable: true,
                            listeners: {
                                //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                                parseText: function (tip, name, value, text, i) {
                                    var lable = chartData.MTipLabels[i];
                                    return "<div class='m-chart-tip'><p class='tip-lable' style=\"font-family:'" + fontFamily + "'\">" + mText.encode(lable) + "</p><p class='tip-value' style=\"color:'" + colors[4] + "';font-family:'" + fontFamily + "'\">" + mMath.toMoneyFormat(chartData.MValue[i]) + "</p></div>";
                                }
                            }
                        },
                        crosshair: {
                            enable: true,
                            gradient: true,
                            line_color: colors[5],
                            line_width: 1
                        },
                        sub_option: {
                            hollow_inside: false,
                            label: false,
                            point_size: 8
                        },
                        background_color: colors[6],
                        coordinate: {
                            axis: {
                                width: [0, 0, 2, 2],
                                color: colors[7]
                            },
                            background_color: colors[8],
                            height: '90%',
                            grid_color: colors[9],
                            scale: {
                                scale_enable: false,
                                scale_share: 0
                            }
                        }
                    });
                chart.draw();
            };
            //生成柱状图
            this.initColumnChart = function (chartData, selector, colors, title, url, isGradient) {
                //边线的color集合
                var lineColor = colors[0];
                //数据的color
                var dataColor = colors[1];
                //如果没有数据
                if (!chartData || !chartData.data || chartData.data.length == 0) {
                    //绘制空的图形
                    return that.initEmptyChart(selector, lineColor, title);
                }
                var data = chartData.data;
                //声称渐变色
                if (isGradient == undefined || isGradient) {
                    dataColor = mColor.Gradient(dataColor[0], dataColor[dataColor.length - 1], data.length);
                }

                //对每一层赋值颜色
                for (var i = 0; i < data.length ; i++) {
                    data[i].color = dataColor[i];
                }
                var labels = chartData.labels;
                var scalSpace = chartData.scalSpace;

                //等待支付的多语言
                var owingL = HtmlLang.Write(LangModule.IV, "owing", "owing");
                //逾期的多语言
                var dueL = HtmlLang.Write(LangModule.IV, "Due", "due");

                new iChart.ColumnStacked2D({
                    render: selector,
                    data: data,
                    align: "left",
                    font: fontFamily,
                    sub_option: {
                        label: false,
                        listeners: {
                            click: function (r, e, m) {
                                //全部的结果
                                var nameObj = $.parseJSON(r.options.name);
                                //如果“图表类型”是费用报销单
                                if (prefixs[7] == prefix) {
                                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, "PurchaseFapiaoHome", "进项发票"), "/FP/FPHome/FPReconcileHome?type=1&index=2");
                                } else if (prefixs[6] == prefix) {
                                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, "VATFapiao", "销项发票"), "/FP/FPHome/FPReconcileHome?type=0&index=2");
                                } else if (prefixs[5] == prefix) {
                                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, "PurchaseFapiaoHome", "进项发票"), "/FP/FPHome/FPReconcileHome?type=1&index=0");
                                } else if (prefixs[4] == prefix) {
                                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, "VATFapiao", "销项发票"), "/FP/FPHome/FPReconcileHome?type=0&index=0");
                                } else if (prefixs[3] == prefix) {
                                    //跳转至“费用报销单”列表
                                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, "ExpenseClaim", "Expense Claims"), "/IV/Expense/ExpenseList/");
                                } else {
                                    //如果“图表类型”是采购发票
                                    if (prefixs[2] == prefix) {
                                        //跳转至“采购”列表
                                        $.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, "Purchase", "Purchase"), "/IV/Bill/BillList/4/");
                                    } else {
                                        //跳转至“发票对账单”列表
                                        $.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, "ViewStatement", "View Statement"), "/IV/Invoice/ViewStatement?statementType=Outstanding&statementContactID=" + nameObj.MContactID);
                                    }
                                }
                            }
                        }
                    },
                    labels: labels,
                    border: {
                        enable: false,
                        width: [0, 0, 0, 0],
                        color: colors[1]
                    },
                    decimalsnum: 2,
                    width: that.getChartWidth("#" + selector),
                    height: that.getChartHeight("#" + selector),
                    offsetx: 35,
                    label: { color: colors[0], fontsize: 12 },
                    default_mouseover_css: false,
                    background_color: null,
                    column_width: 50,
                    coordinate: {
                        axis: {
                            width: [0, 0, 2, 0],
                            color: colors[7]
                        },
                        background_color: colors[8],
                        height: '80%',
                        grid_color: colors[9],
                        scale: {
                            scale_enable: false,
                            scale_share: 0
                        }
                    },
                    tip: {
                        enable: true,
                        listeners: {
                            //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                            parseText: function (tip, name, value, text, i, j) {
                                //全部的结果
                                var nameObj = $.parseJSON(name);
                                if (!nameObj) {
                                    return;
                                }
                                //真正的名字
                                var name = nameObj.name;
                                //是否逾期
                                var dueOwing = nameObj.MChartDueOrOwing.split(',')[j];
                                //转化
                                dueOwing = dueOwing == "1" ? owingL : (dueOwing == "-1" ? dueL : "");
                                //返回结果
                                var result =
                                        "<span class=\"m-chart-font-blue14\">" + mText.encode(nameObj.name) + "</span><br />" +
                                        "<span class=\"m-chart-font-gray\">" + mText.encode(nameObj.MChartFirstName) + " " + mText.encode(nameObj.MChartLastName) + "</span><br />" +
                                        "<span class=\"m-chart-font-blueBold16\">" + mMath.toMoneyFormat(value) + " " + dueOwing + "</span>";
                                //加上点击事件
                                //如果“图表类型”是费用报销单
                                if (prefixs[7] == prefix) {
                                    return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, \"PurchaseFapiaoHome\", \"进项发票\"), \"/FP/FPHome/FPReconcileHome?type=1&index=2\")'>" + result + "</div>";
                                } else if (prefixs[6] == prefix) {
                                    return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, \"VATFapiao\", \"销项发票\"), \"/FP/FPHome/FPReconcileHome?type=0&index=2\")'>" + result + "</div>";
                                } else if (prefixs[5] == prefix) {
                                    return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, \"PurchaseFapiaoHome\", \"进项发票\"), \"/FP/FPHome/FPReconcileHome?type=1&index=0\")'>" + result + "</div>";
                                } else if (prefixs[4] == prefix) {
                                    return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.FP, \"VATFapiao\", \"销项发票\"), \"/FP/FPHome/FPReconcileHome?type=0&index=0\")'>" + result + "</div>";
                                } else if (prefixs[3] == prefix) {
                                    //跳转至“费用报销单”列表
                                    return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, \"ExpenseClaim\", \"Expense Claims\"), \"/IV/Expense/ExpenseList/\")'>" + result + "</div>";
                                } else {
                                    //如果“图表类型”是采购发票
                                    if (prefixs[2] == prefix) {
                                        //跳转至“采购”列表
                                        return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, \"Purchase\", \"Purchase\"), \"/IV/Bill/BillList/4/\")'>" + result + "</div>";
                                    } else {
                                        //跳转至“发票对账单”列表
                                        return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, \"ViewStatement\", \"View Statement\"), \"/IV/Invoice/ViewStatement?statementType=Outstanding&statementContactID=" + nameObj.MContactID + "\")'>" + result + "</div>";
                                    }
                                }
                            }
                        }
                    }

                }).draw();
            };
            //生成数据图
            this.initDataChart = function (chartData, selector, colors, title)
            { };
            //生成饼状图
            this.initPieChart = function (chartData, selector, colors, title, centerText) {
                var centerText = "--";
                if (chartData && chartData.length > 0) {
                    for (var i = 0 ; i < chartData.length; i++) {
                        var data = chartData[i];

                        data.color = colors[i];

                        //默认以第一条数据作为中间显示比例值
                        if (i == 0 && data.value != -1) {
                            centerText = data.value + "%";
                        }

                        data.value = data.value > 100 ? 100 : data.value;

                        data.value = data.value < 0 ? 0 : data.value;
                    }
                } else {
                    chartData = [
                        { name: '', value: 0, color: colors[0] },
                        { name: '', value: 100, color: colors[1] }
                    ];
                }

                var chart = new iChart.Donut2D({
                    render: selector,
                    title: {
                        text: title,
                        color: '#636363',
                        fontsize: 12,
                    },
                    center: {
                        text: centerText,
                        shadow: false,
                        fontsize: 16,
                        shadow_offsetx: 0,
                        shadow_offsety: 2,
                        shadow_blur: 2,
                        shadow_color: '#b7b7b7',
                        color: '#6f6f6f'
                    },
                    data: chartData,
                    shadow: false,
                    shadow_offsetx: 0,
                    shadow_offsety: 2,
                    shadow_blur: 10,
                    shadow_color: '#676767',
                    gradient: false,//开启渐变背景
                    background_color: '#FFFFFF',
                    separate_angle: 0,//分离角度
                    increment: 0,//弹出距离
                    sub_option: {
                        gradient: false,
                        color_factor: 0.08,
                        gradient_mode: 'RadialGradientInOut',
                        label: false
                    },
                    border: {
                        enable: false,
                        width: [0, 0, 0, 0],
                        color: colors[1]
                    },
                    showpercent: true,
                    decimalsnum: 0,
                    width: 250,
                    height: 170,
                    radius: 140,
                    offset_angle: 90//逆时针偏移120度
                });

                chart.draw();
            };
            //生成图表 url：点击图片柱状图点击地址 isGradient 渐变色
            this.initChart = function (chartType, data, selector, colors, title, url, isGradient) {
                //
                selector = that.prefixName + _bspeChart;
                //柱状图
                switch (chartType) {
                    case trendType:
                        //趋势图
                        that.initTrendChart(data, selector, colors, title);
                        break;
                    case columnType:
                        //柱状图
                        that.initColumnChart(data, selector, colors, title, url, isGradient);
                        break;
                    case dataType:
                        that.initDataChart(data, selector, colors, title);
                        //数据图
                    case pieType:
                        that.initPieChart(data, selector, colors, title);
                        //饼状图
                        break;
                    default:
                        //趋势图
                        that.initTrendChart(data, selector, colors, title);
                        break;

                }
            };
            //注册日期切换选择的时间
            this.initDateSelectEvent = function (selector, callback) {
                //切换样式
                $(_bspeDateSelect + ">span", selector).off("click").on("click", function () {
                    $(this).siblings().removeClass(_dateUseSelected.trimStart("."));
                    $(this).addClass(_dateUseSelected.trimStart("."));
                    callback();
                });
            };
            //注册图形类型切换选择的时间
            this.initChartSelectEvent = function (selector, callback) {
                //切换样式
                $(_bspeChartSelect + ">span", selector).off("click").on("click", function () {
                    $(this).siblings().removeClass(_chartUseSelected.trimStart("."));
                    $(this).addClass(_chartUseSelected.trimStart("."));
                    callback();
                });
            };
            //初始化表格 针对采购，销售，费用报销
            this.initTable = function (data) {
                //表格后缀
                var tablePrefix = "-info-table";
                //获取表格
                var $trs = $("." + that.prefixName + tablePrefix).find("tr");
                //设置每一行的第二列
                for (var i = 0; i < 4; i++) {
                    //
                    var span = $trs.eq(i).find("td").eq(1).find("a");
                    //赋值
                    span.text(mMath.toMoneyFormat(data[i][0]));
                    //
                    var title = "" + data[i][1]
                    //
                    span.attr("url", data[i][2]);
                    //点击跳转到相应的页面
                    span.off("click").on("click", function () {
                        $.mTab.addOrUpdate(title, $(this).attr("url"), true, true);
                    });
                }
            };
            //点击MyData 以及 Message事件
            this.initTabEvent = function () {
                //点击数据中心
                $(_userDataBtn).off("click").on("click", function () {
                    //显示数据图表
                    $(_dataHome).show();
                    //
                    $(_messageHome).hide();
                    //切换选中样式
                    $(this).addClass(_funcSelected).removeClass(_funcUnselected);
                    //切换
                    $(_userMsgBtn).addClass(_funcUnselected).removeClass(_funcSelected);
                    //切换标题
                    $(_dataHomeTitle).show();
                    //切换
                    $(_messageHomeTitle).hide();
                });
                //点击消息中心
                $(_userMsgBtn).off("click").on("click", function () {
                    //隐藏数据图表
                    $(_dataHome).hide();
                    //
                    $(_messageHome).show();
                    //切换选中样式
                    $(this).addClass(_funcSelected).removeClass(_funcUnselected);
                    //切换
                    $(_userDataBtn).addClass(_funcUnselected).removeClass(_funcSelected);
                    //切换标题
                    $(_dataHomeTitle).hide();
                    //切换
                    $(_messageHomeTitle).show();
                });
                $(".m-imain").scroll(function () {
                    //显示虚线
                    if (this.scrollTop > 0) {
                        $(this).addClass(dashedLineClassName);
                    }
                    else {
                        $(this).removeClass(dashedLineClassName);
                    }
                });
            };
            //
            this.InitAll = function () {
                //
                that.initTabEvent();
            };
        };
        //
        return FWDashboardHome;
    })();
    //
    window.FWDashboardHome = FWDashboardHome;
})()