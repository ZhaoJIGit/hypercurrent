var FPDashboard = {
    vatBarChartDom: $(".vat-canvas"),
    vatBarChartClass: ".vat-canvas",
    vatPieChartClass: ".vatpie-canvas",
    incomeBarChartCalss: ".incomeFapiao-canvas",
    incomePieChartClass: ".incomepie-canvas",
    ichartOj: new FWDashboardHome("vat"),
    vatFapiaoEnum: 0,
    incomeFapiaoEnum: 1,
    isSmartVersion: $("#hideVersion").val() == "True",
    barChartColor: [['#f96422', '#f3956d', '#7c8490', '#f0c0aa'], ['#78B3E1', '#F5AB8D']],
    pieChartColor: ['#78B3E1', '#EBE9E9'],
    init: function () {
        FPDashboard.initAction();

        FPDashboard.initVatChart();
        FPDashboard.initInputChart();

        FPDashboard.initTableData();
    },
    initVatChart: function () {
        var selector = $(".div-vat");
        FPDashboard.getData(selector, FPDashboard.vatFapiaoEnum, function (data) {

            var jsonData = data ? $.parseJSON(data) : {};

            var barChartData = jsonData.BarChartData;

            FPDashboard.initVATBarChart(barChartData);


            var pieChartData = jsonData.PieChartData;

            FPDashboard.initVATPieChart(pieChartData);

        });
    },
    //销项柱状图
    initVATBarChart: function (data) {
        var selector = FPDashboard.vatBarChartClass;
        var ichartOj = FPDashboard.ichartOj;
        var chartType = "1";

        var color = FPDashboard.barChartColor;
        var navToUrl = "";

        var prefixs = FPDashboard.isSmartVersion ? "vatFapiaoSmart" : "vatFapiao";

        new FWDashboardHome(prefixs).initChart(chartType, data, selector, color, "", navToUrl, false);
    },
    //销项饼状图
    initVATPieChart: function (data) {
        //总账版不显示饼状图
        if (!FPDashboard.isSmartVersion) {
            var selector = FPDashboard.vatPieChartClass;
            var chartType = "3";

            var color = FPDashboard.pieChartColor;
            var navToUrl = "";
            var title = HtmlLang.Write(LangModule.FP, "ThisYearBillingProportion", "本年开票比例");

            var pieChartData = data ? data.data : [];

            //如果是两个数据都为空
            if (pieChartData.length == 2) {
                if (pieChartData[0].MTotalAmount === 0 && pieChartData[1].MTotalAmount === 0) {
                    pieChartData[0].value = "-1";
                }
            }

            new FWDashboardHome("vatPie").initChart(chartType, pieChartData, selector, color, title, navToUrl);
        }
        //图标下方的总金额
        var totalAmount = Megi.Math.toMoneyFormat(data.totalAmount);

        $("#outputFapiaoTotalAmount").text(totalAmount);


    },
    initInputChart: function () {
        var selector = $(".div-input");
        FPDashboard.getData(selector, FPDashboard.incomeFapiaoEnum, function (data) {

            var jsonData = data ? $.parseJSON(data) : {};

            var barChartData = jsonData.BarChartData;

            FPDashboard.initInputBarChart(barChartData);



            var pieChartData = jsonData.PieChartData;

            FPDashboard.initInputPieChart(pieChartData);


        });
    },
    initInputBarChart: function (data) {
        var selector = FPDashboard.incomeBarChartCalss;
        var ichartOj = FPDashboard.ichartOj;
        var chartType = "1";

        var color = FPDashboard.barChartColor;
        var navToUrl = "";

        var prefixs = FPDashboard.isSmartVersion ? "incomeFapiaoSmart" : "incomeFapiao";

        new FWDashboardHome(prefixs).initChart(chartType, data, selector, color, "", navToUrl, false);
    },
    initInputPieChart: function (data) {
        //总账版不显示饼状图
        if (!FPDashboard.isSmartVersion) {
            //图标
            var selector = FPDashboard.incomePieChartClass;

            var chartType = "3";

            var color = FPDashboard.pieChartColor;
            var navToUrl = "";
            var title = HtmlLang.Write(LangModule.FP, "ThisYearReceiveProportion", "本年已收票比例");

            var pieChartData = data ? data.data : [];

            //如果是两个数据都为空
            if (pieChartData.length == 2) {
                if (pieChartData[0].MTotalAmount === 0 && pieChartData[1].MTotalAmount === 0) {
                    pieChartData[0].value = "-1";
                }
            }

            new FWDashboardHome("incomePie").initChart(chartType, pieChartData, selector, color, title, navToUrl);
        }
        //图标下方的总金额
        var totalAmount = data ? Megi.Math.toMoneyFormat(data.totalAmount) : 0;

        $("#inputFapiaoTotalAmount").text(totalAmount);
    },
    initAction: function () {
        $("#btnVatMange").mTip({
            target: $("#divVatSub"),
            width: 200,
            parent: $("#divVatSub").parent()
        });

        $("#btnIncome").mTip({
            target: $("#divIncomeSub"),
            width: 200,
            parent: $("#divIncomeSub").parent()
        });

        $("#btnOutputFPImportList").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, 1, false);
        });

        $("#btnOutputFPTransaction").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, 2, false);
        });

        $("#btnOutputFPHook").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, 0, false);
        });

        $("#btnOutPutFPAutoRecevieLog").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, 4, false);
        });


        $("#btnOutputFPTable").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, 1, true);
        });
        $("#btnOutputFPTable_2").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, 1, true);
        });

        $("#btnOutputFPCoding").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, 3, false);
        });

        $("#btnInputFPImportList").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.incomeFapiaoEnum, 1, false);
        });

        $("#btnInputFPTransaction").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.incomeFapiaoEnum, 2, false);
        });

        $("#btnInputFPCoding").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.incomeFapiaoEnum, 3, false);
        });

        $("#btnInputFPHook").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.incomeFapiaoEnum, 0, false);
        });

        $("#btnInputFPAutoRecevieLog").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.incomeFapiaoEnum, 4, false);
        });

        $("#btnInputFPTable").off("click").on("click", function () {
            FPDashboard.navPage(FPDashboard.incomeFapiaoEnum, 1, true);
        })

        $("#btnInputFPTitle").off("click").on("click", function () {
            var isSmart = $("#hideVersion").val();
            var tabIndex = isSmart == "False" ? 0 : 2;
            FPDashboard.navPage(FPDashboard.incomeFapiaoEnum, tabIndex, false);
        });

        $("#btnOutputFPTitle").off("click").on("click", function () {
            var isSmart = $("#hideVersion").val();
            var tabIndex = isSmart == "False" ? 0 : 2;
            FPDashboard.navPage(FPDashboard.vatFapiaoEnum, tabIndex, false);
        });
    },
    //url获取 isTable：是否开票单
    navPage: function (fapiaoType, tabIndex, isTable) {
        var title;
        var url;
        if (fapiaoType == FPDashboard.incomeFapiaoEnum) {
            //进项
            if (isTable) {
                title = HtmlLang.Write(LangModule.FP, "FPTable", "开票单");
                url = '/FP/FPHome/FPHome?invoiceType=1&index=' + tabIndex;
            } else {
                title = HtmlLang.Write(LangModule.FP, "PurchaseFapiaoHome", "进项发票");
                url = "/FP/FPHome/FPReconcileHome?type=1&index=" + tabIndex;
            }
        } else if (fapiaoType == FPDashboard.vatFapiaoEnum) {
            //销项
            if (isTable) {
                title = HtmlLang.Write(LangModule.FP, "FPTable", "开票单");
                url = '/FP/FPHome/FPHome?invoiceType=0&index=' + tabIndex;
            } else {
                title = HtmlLang.Write(LangModule.FP, "VATFapiao", "销项发票");
                url = "/FP/FPHome/FPReconcileHome?type=0&index=" + tabIndex;
            }
        }
        $.mTab.addOrUpdate(title, url, true);
    },
    getData: function (selector, type, callback) {
        var url = "/FW/FWHome/GetFapiaoDashboardData";
        selector.mask("");
        //发送请求
        mAjax.post(url, { type: type }, function (data) {
            if (callback && $.isFunction(callback)) {
                callback(data);
            }
            //取消遮罩
            selector.unmask();
        }, function () {
            //取消遮罩
            selector.unmask();
        });
    },
    initTableData: function () {
        var selector = $(".dashboard-table");
        var url = "/FP/FPHome/GetDashboardTableData";
        selector.mask("");
        //发送请求
        mAjax.post(url, {}, function (data) {

            if (data) {
                var jsonData = $.parseJSON(data);

                //先处理进销项差额
                var taxJsonData = jsonData.TaxData;
                var inputTaxAmount = taxJsonData.InputTax ? taxJsonData.InputTax : 0.00;
                var outputTaxAmount = taxJsonData.OutputTax ? taxJsonData.OutputTax : 0.00;

                var balance = outputTaxAmount - inputTaxAmount;

                $(".incomeAmount").text(Megi.Math.toMoneyFormat(inputTaxAmount));
                $(".ounputAmount").text(Megi.Math.toMoneyFormat(outputTaxAmount));

                $(".balance").text(Megi.Math.toMoneyFormat(balance));

                //在处理供应商未收票记录

                var supplierDataList = jsonData.SupplierData;

                if (supplierDataList && supplierDataList.length > 0) {
                    var trHtml = "";
                    for (var i = 0 ; i < supplierDataList.length; i++) {
                        var supplierData = supplierDataList[i];
                        var contactName = supplierData.ContactName;
                        var amount = supplierData.Amount ? Megi.Math.toMoneyFormat(supplierData.Amount) : 0.00;
                        trHtml += "<tr>";
                        trHtml += "   <td>" + contactName + "</td>";
                        trHtml += "   <td class='fp-td-amount'>" + amount + "</td>";
                        trHtml += "</tr>";
                    }

                    $(".supplier-table").append(trHtml);
                }

            }

            selector.unmask();
        }, function () {
            selector.unmask();
        });
    }
};



