/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="../IVBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var InvoiceList = {
    CurrentType: 0,
    url_List: "/IV/Invoice/InvoiceList/",
    url_Edit: "/IV/Invoice/InvoiceEdit",
    url_CreditNoteEdit: "/IV/Invoice/CreditNoteEdit",
    url_View: "/IV/Invoice/InvoiceView",
    url_CreditNoteEdit: "/IV/Invoice/CreditNoteEdit",
    url_CreditNoteView: "/IV/Invoice/CreditNoteView",
    HasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    Status: 0,
    init: function () {
        InvoiceList.initSearchControl();
        InvoiceList.initTab();
        InvoiceList.initAction();
        InvoiceList.InitSpanIcon();
        //初始化父类的绑定事件
        IVBase.bindEvent = function () {
            InvoiceList.bindGrid(InvoiceList.CurrentType);
        }
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
    initTab: function () {
        var typeId = Number($("#hidInvoiceType").val());
        $(".m-extend-tabs").tabsExtend({
            initTabIndex: typeId,
            onSelect: function (index) {
                InvoiceList.CurrentType = index;
                InvoiceList.bindGrid(index);
                //刷新Tab合计数据
                IVBase.tabSelRefreshTitle(1);
                //切换日期查询范围类型
                IVBase.changeSearchWithin(index);

				if(index == 6){
					$("#aExport").linkbutton("disable");
				}
				else{
					$("#aExport").linkbutton("enable");
				}

                $(window).resize();
            }
        });
    },
    initAction: function () {
        //批量删除
        $("#btnDelete").click(function () {
            IVBase.deleteList("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                mWindow.reload(InvoiceList.url_List + InvoiceList.CurrentType);
            })
        });
        //批量提交并审核
        $("#btnSbmForAppr").click(function () {
            IVBase.submitForApproval("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "SubmitForApprovalSuccessfully", "Submit for approval Successfully!"));
                mWindow.reload(InvoiceList.url_List + InvoiceList.CurrentType);
            })
        });
        //批量审核
        $("#btnApproval").click(function () {
            IVBase.approval("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                mWindow.reload(InvoiceList.url_List + InvoiceList.CurrentType);
            })
        });
        //查询
        $("#aSearchInvoice").click(function () {
            InvoiceList.bindGrid(InvoiceList.CurrentType);
        });
        //发送Email
        $("#btnEmail").click(function () {
            var jsonParam = {};
            Megi.grid("#gridInvoice", "optSelected", {
                callback: function (ids) {
                    jsonParam.ReportId = ids;
                    var arrNumber = [];
                    var arrType = [];
                    var hidTypes = $("#divList .datagrid-view2 .datagrid-body table tr td[field=MID] input[name=type]");
                    $.each(hidTypes, function (i, obj) {
                        var arrVal = obj.value.split(',');
                        if (ids.indexOf(arrVal[0]) != -1) {
                            arrNumber.push(arrVal[1]);
                            arrType.push(arrVal[2] == IVBase.InvoiceType.Sale ? "SaleInvoice" : "SaleRedInvoice");
                        }
                    });
                    jsonParam.BillType = arrType.toString();
                    jsonParam.InvoiceNumber = arrNumber.toString();

                    var paramStr = 'selectIds=' + ids + "&status=" + (InvoiceList.CurrentType - 1) + "&type=" + IVBase.InvoiceType.Sale + "&reportType=" + jsonParam.BillType + "&rptJsonParam=" + escape($.toJSON(jsonParam));
                    var param = $.toJSON({ type: "Invoices", param: paramStr });
                    Print.selectPrintSetting(param);
                }
            });
        });
        $("#show_pie").click(function () {
            InvoiceList.GetOwingMostChart();
            $("#show_pie").addClass("active");
            $("#show_list").removeClass("active");
            $("#divCustomerMostChart").show();
            $("td a span").show();
            $('td[class$="red"]').hide();
            $("tbody tr td").css("padding", "0 0 0 20px");
            $("#divCustomerMostChart").css("width", "30%");
            $("#divCustomerMostChart").css("float", "left");
            $("#divCustomerMostList").css("width", "60%");
            $("#divCustomerMostList").css("float", "right");
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
        $("#aExport").click(function () {
            //mWindow.reload有问题，会刷新当前页面
			if(!$(this).hasClass("l-btn-disabled")){
				location.href = '/IV/Invoice/Export?jsonParam=' + escape($.toJSON(InvoiceList.getQueryParam()));
				$.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
			}
        });
        //打印
        $("#aPrint").click(function () {
            Megi.grid("#gridInvoice", "optSelected", {
                callback: function (ids) {
                    IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "invoices", "Invoices"), $.toJSON(InvoiceList.getQueryParam(ids)), "InvoiceListPrint");
                }
            });
        });
        //重复发票：修改为草稿状态
        $("#btnSavedAsDraft").click(function () {
            IVBase.savedAsDraft("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "SavedAsDraftSuccessfully", "Saved As Draft Successfully!"));
                mWindow.reload(InvoiceList.url_List + InvoiceList.CurrentType);
            })
        });
        //重复发票：修改为审核状态
        $("#btnApprovalRepeat").click(function () {
            IVBase.approvalRepeat("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApprovalSuccessfully", "Approval Successfully!"));
                mWindow.reload(InvoiceList.url_List + InvoiceList.CurrentType);
            })
        });
        //重复发票：修改为审核并发送状态
        $("#btnApproveForSending").click(function () {
            var ids = "";
            $("#gridInvoice").parent().find(".row-key-checkbox").each(function () {
                if ($(this).attr("checked") == "checked") {
                    ids = ids + $(this).val() + ",";
                }
            });
            if (ids.length == 0) {
                $.mDialog.alert(
                        HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"),
                        { title: HtmlLang.Write(LangModule.Common, "NoItemsSelected", "No items selected") });
                return;
            }
            var param = 'selectIds=' + ids + "&sendType=3&status=3&type=" + IVBase.InvoiceType.Sale;
            Print.openSendEmailDialog(HtmlLang.Write(LangModule.IV, "EditMessage", "Edit Message"), param);
        });
        //重复发票：删除
        $("#btnDeleteRepeat").click(function () {
            IVBase.deleteRepeatList("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                mWindow.reload(InvoiceList.url_List + InvoiceList.CurrentType);
            })
        });
        //导入发票
        $("#aImport, #divImportIv").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/Invoice_Sale', HtmlLang.Write(LangModule.IV, "ImportInvoice", "Import Invoice"), 900, 520);
        });
        //导入红字发票
        $("#divImportIvRed").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/Invoice_Sale_Red', HtmlLang.Write(LangModule.IV, "ImportCreditNote", "Import Credit Note"), 900, 520);
        });
        //批量支付
        $("#btnBatchPay").click(function () {
            Megi.grid("#gridInvoice", "optSelected", {
                callback: function (ids) {
                    mAjax.submit(
                        "/IV/UC/IsSuccessBatch",
                        { KeyIDs: ids, selectObj: "Invoice_Sales" },
                        function (res) {
                            if (res.Success) {
                                $.mDialog.show({
                                    mTitle: HtmlLang.Write(LangModule.IV, "NewBatchReceiveMoney", "New Batch Receive Money"),
                                    mWidth: 700,
                                    mHeight: 400,
                                    mShowbg: true,
                                    mShowTitle: true,
                                    mDrag: "mBoxTitle",
                                    mContent: "iframe:/IV/UC/BatchPayment",
                                    mPostData: { selectIds: ids, obj: "Invoice_Sales" }
                                });
                            }
                            else {
                                $.mDialog.alert(res.Message);
                            }
                        });
                }
            });
            return false;
        });
    },
    getQueryParam: function (selectedIds) {
        var queryParam = {}
        if ($(".m-adv-search").css("display") == "block") {
            queryParam = $("body").mFormGetForm();
        }
        queryParam.MStatus = InvoiceList.Status;
        queryParam.MType = IVBase.InvoiceType.Sale;
        if (selectedIds) {
            queryParam.SelectedIds = selectedIds;
        }
        return queryParam;
    },
    bindGrid: function (type) {
        var status = 0;
        var columnList = null;
        $('.m-tab-toolbar .left .m-tool-bar-btn').hide();
        $('#divHome').hide();
        if (type == 0) {
            $(".m-adv-search-invoice").hide();
            $(".m-adv-search-btn").hide();
        } else {
            if (!$(".m-adv-search-invoice").is(":visible")) {
                $(".m-adv-search-btn").show();
            }
        }
        IVBase.columns[7].title = HtmlLang.Write(LangModule.IV, "Received", "Received");
        IVBase.columns[8].title = HtmlLang.Write(LangModule.IV, "UnReceived", "Unreceived");
        switch (type) {
            case 0:
                $('#divHome').show();
                $('#divList').hide();
                if ($.trim($("#divIncomingChart").html()) == '') {
                    InvoiceList.InitIncomingChart();
                }
                break;
            case 1:
                columnList = Megi.getDataGridColumnList(IVBase.columns, [1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12]);
                break;
            case 2:
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 1, 2, 3, 4, 5, 8, 11, 12]);
                status = IVBase.Status.Draft;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnApproveForSending').hide();
                $('#btnDeleteRepeat').hide();
                $('#btnBatchPay').hide();
                break;
            case 3:
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 1, 2, 3, 4, 5, 8, 11, 12]);
                status = IVBase.Status.WaitingApproval;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnApproveForSending').hide();
                $('#btnDeleteRepeat').hide();
                $('#btnBatchPay').hide();
                break;
            case 4:
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12]);
                status = IVBase.Status.AwaitingPayment;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnApproval').hide();
                $('#btnDelete').hide();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnApproveForSending').hide();
                $('#btnDeleteRepeat').hide();
                break;
            case 5:
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 1, 2, 3, 4, 5, 7, 8, 10, 11, 12]);
                status = IVBase.Status.Paid;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnApproval').hide();
                $('#btnDelete').hide();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnApproveForSending').hide();
                $('#btnDeleteRepeat').hide();
                $('#btnBatchPay').hide();
                break;
            case 6:
                columnList = Megi.getDataGridColumnList(IVBase.repeatInvoiceColumns, [0, 1, 2, 3, 4, 5, 7, 8, 10, 11]);
                $('#btnSavedAsDraft').show();
                $('#btnApprovalRepeat').show();
                $('#btnApproveForSending').show();
                $('#btnDeleteRepeat').show();
                $('#btnBatchPay').hide();
                break;
        }
        InvoiceList.Status = status;
        if (type > 0) {
            $('#divList').show();
            var queryParam = {}
            if ($(".m-adv-search").css("display") == "block") {
                queryParam = $("body").mFormGetForm();
            }
            queryParam.MStatus = status;
            queryParam.MType = IVBase.InvoiceType.Sale;
            Megi.grid("#gridInvoice", {
                url: type == 6 ? "/IV/Invoice/GetRepeatIVList" : IVBase.url_getlist,
                sortOrder: "desc",//默认降序
                pagination: true,
                queryParams: queryParam,
                columns: columnList,
                onLoadSuccess: function () {
                    $("#gridInvoice").datagrid('resizeColumn');
                }
            });
        }
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
                    InvoiceList.fillStackedData(JSON.parse(jsonData));
                }
            });
    },
    /*用来做渐变的颜色*/
    chartColors: ['#c4e9c7', '#8ad68e', '#69bd6c', '#51ae56', '#26a72c'],
    /*获取渐变色的数组*/
    getGradientColors: function (dataCount) {
        //声称渐变色
        return mColor.Gradient(InvoiceList.chartColors[0], InvoiceList.chartColors[InvoiceList.chartColors.length - 1], dataCount);
    },
    /*给每个data的color赋值*/
    setDataColor: function (data) {
        //
        var colors = InvoiceList.getGradientColors(data.length);
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
                    InvoiceList.fillPieData(jsonData);
                }
            });
    },
    fillStackedData: function (jsonData) {
        if (jsonData.data.length == 0) {
            return;
        }
        //等待支付的多语言
        var owingL = HtmlLang.Write(LangModule.IV, "owing", "owing");
        //逾期的多语言
        var dueL = HtmlLang.Write(LangModule.IV, "Due", "due");


        var data = jsonData.data;
        var labels = jsonData.labels;
        var scalSpace = jsonData.scalSpace;
        new iChart.ColumnStacked2D({
            render: 'divIncomingChart',
            data: InvoiceList.setDataColor(data),
            sub_option: {
                label: false,
                listeners: {
                    click: function (r, e, m) {
                        //全部的结果
                        var nameObj = $.parseJSON(r.options.name);
                        $.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, "ViewStatement", "View Statement"), "/IV/Invoice/ViewStatement?statementType=Outstanding&statementContactID=" + nameObj.MContactID);
                    }
                }
            },
            //labels: ['Older', 'Oct', 'Nov', 'Dec', 'Jan', 'Feb'],
            labels: labels,
            border: 0,
            decimalsnum: 2,
            width: 520,
            height: 160,
            //offsetx: 35,
            //label: { color: '#7c8490', font: 'Arial', fontsize: 12 },
            default_mouseover_css: false,
            background_color: null,
            tip: {
                enable: true,
                listeners: {
                    //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                    parseText: function (tip, name, value, text, i, j) {
                        //全部的结果
                        var nameObj = $.parseJSON(name);
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
                        return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, \"ViewStatement\", \"View Statement\"), \"/IV/Invoice/ViewStatement?statementType=Outstanding&statementContactID=" + nameObj.MContactID + "\")'>" + result + "</div>";
                    }
                }
            },
            coordinate: {
                background_color: null,
                axis: {
                    width: [0, 0, 1, 0],
                    color: '#666666'
                },
                scale: {
                    scale_enable: false,
                    scale_share: 0,
                },
                height: '80%'
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
            data: InvoiceList.setDataColor(data),
            decimalsnum: 2,
            width: 280,
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
                        $.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, "ViewStatement", "View Statement"), "/IV/Invoice/ViewStatement?statementType=Outstanding&statementContactID=" + r.get('MContactID'));
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
                                        "<span class=\"m-chart-font-blueBold16\">" + mText.encode(value) + " " + HtmlLang.Write(LangModule.IV, "Due", "Due") + "</span>";
                                if (data[j].MOverDue > 0) {
                                    re += "<br /><span class=\"m-chart-font-red\">" + data[j].MOverDue + " " + HtmlLang.Write(LangModule.IV, "Overdue", "overdue") + " </span>";
                                }
                                return "<div onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, \"ViewStatement\", \"View Statement\"), \"/IV/Invoice/ViewStatement?statementType=Outstanding&statementContactID=" + data[j].MContactID + "\")'> " + re + "</div>";
                            }
                        }

                    }
                }
            }

        }).draw();
    },
    reload: function (type) {
        InvoiceList.bindGrid(type);
        //刷新Tab合计数据
        IVBase.tabSelRefreshTitle(1);
        //$(window).resize();
    }
}

$(document).ready(function () {
    InvoiceList.init();
});