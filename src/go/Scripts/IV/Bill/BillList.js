/// <reference path="../jquery/jquery.megi.js" />
/// <reference path="../../../fw/scripts/jquery-1.8.2.min.js" />
/// <reference path="../IVBase.js" />
/// <reference path="../megi.go.common.js" />
var BillList = {
    CurrentType: 0,
    url_List: "/IV/Bill/BillList/",
    //采购发票编辑地址
    url_Edit: "/IV/Bill/BillEdit",
    //采购红字发票编辑地址
    url_CreditNoteEdit: "/IV/Bill/CreditNoteEdit",
    //采购发票查看地址
    url_View: "/IV/Bill/BillView",
    //采购红字发票查看地址
    url_CreditNoteView: "/IV/Bill/CreditNoteView",
    //是否有权限进行 编辑 和 删除
    HasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    Status: 0,
    //初始化页面
    init: function () {
        BillList.initSearchControl();
        BillList.initTab();
        BillList.initAction();
        BillList.InitSpanIcon();
        //初始化父类的绑定事件
        IVBase.bindEvent = function () {
            BillList.bindGrid(BillList.CurrentType);
        }
    },
    //初始化查询控件
    initSearchControl: function () {
        var filter = Megi.request("filter");
        if (filter != "") {
            $(".m-adv-search-btn").hide();
            $(".m-adv-search").show();
            filter = eval("(" + filter + ")");
            $("body").mFormSetForm(filter);
        }
    },
    //初始化 Tab 选项卡
    initTab: function () {
        var typeId = Number($("#hidInvoiceType").val());
        $(".m-extend-tabs").tabsExtend({
            initTabIndex: typeId,
            onSelect: function (index) {
                BillList.bindGrid(index);
                BillList.CurrentType = index;
                //刷新Tab合计数据
                IVBase.tabSelRefreshTitle(2);
                //切换日期查询范围类型
                IVBase.changeSearchWithin(index);

                if (index == 6) {
                    $("#aExport").linkbutton("disable");
                }
                else {
                    $("#aExport").linkbutton("enable");
                }

                $(window).resize();
            }
        });
    },
    //初始化内页工具栏
    initAction: function () {
        //批量删除
        $("#btnDelete").click(function () {
            IVBase.deleteList("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                mWindow.reload(BillList.url_List + BillList.CurrentType);
            })
        });
        //批量提交并审核
        $("#btnSbmForAppr").click(function () {
            IVBase.submitForApproval("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "SubmitForApprovalSuccessfully", "Submit for approval Successfully!"));

                mWindow.reload(BillList.url_List + BillList.CurrentType);
                //window.location = BillList.url_List + BillList.CurrentType;
            })
        });
        //批量审核
        $("#btnApproval").click(function () {
            IVBase.approval("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                mWindow.reload(BillList.url_List + BillList.CurrentType);
            })
        });
        //查询
        $("#aSearchInvoice").click(function () {
            BillList.bindGrid(BillList.CurrentType);
        });
        //发送Email
        $("#btnEmail").click(function () {
            Megi.grid("#gridInvoice", "optSelected", {
                callback: function (ids) {
                    var param = 'selectIds=' + ids + "&status=" + BillList.CurrentType + "&type=" + IVBase.InvoiceType.Purchase;
                    Print.openSendEmailDialog(HtmlLang.Write(LangModule.IV, "SendInvoice", "Send Invoice"), param);
                }
            });
        });
        $("#show_pie").click(function () {
            BillList.GetOwingMostChart();
            $("#show_pie").addClass("active");
            $("#show_list").removeClass("active");
            $("#divSupplierMostChart").show();
            $("td a span").show();
            $('td[class$="red"]').hide();
            $("tbody tr td").css("padding", "0 0 0 20px");
            $("#divSupplierMostChart").css("width", "30%");
            $("#divSupplierMostChart").css("float", "left");
            $("#divSupplierMostList").css("width", "60%");
            $("#divSupplierMostList").css("float", "right");
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
        $("#aExport").click(function () {
            //mWindow.reload有问题，会刷新当前页面
            if (!$(this).hasClass("l-btn-disabled")) {
                location.href = '/IV/Invoice/Export?jsonParam=' + escape($.toJSON(BillList.getQueryParam()));
                $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
            }
        });
        //打印
        $("#aPrint").click(function () {
            Megi.grid("#gridInvoice", "optSelected", {
                callback: function (ids) {
                    IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "Bills", "Bills"), $.toJSON(BillList.getQueryParam(ids)), "BillListPrint");
                }
            });
        });
        //重复发票：修改为草稿状态
        $("#btnSavedAsDraft").click(function () {
            IVBase.savedAsDraft("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "SavedAsDraftSuccessfully", "Saved As Draft Successfully!"));
                mWindow.reload(BillList.url_List + BillList.CurrentType);
            })
        });
        //重复发票：修改为审核状态
        $("#btnApprovalRepeat").click(function () {
            IVBase.approvalRepeat("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApprovalSuccessfully", "Approval Successfully!"));
                mWindow.reload(BillList.url_List + BillList.CurrentType);
            })
        });
        //重复发票：删除
        $("#btnDeleteRepeat").click(function () {
            IVBase.deleteRepeatList("#gridInvoice", function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                mWindow.reload(BillList.url_List + BillList.CurrentType);
            })
        });
        //导入采购发票
        $("#aImport, #divImportBill").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/Invoice_Purchase', HtmlLang.Write(LangModule.IV, "ImportBill", "Import Bill"), 900, 520);
        });
        //导入红字采购发票
        $("#divImportBillRed").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/Invoice_Purchase_Red', HtmlLang.Write(LangModule.IV, "ImportBillCreditNote", "Import Credit Note"), 900, 520);
        });
        //批量支付
        $("#btnBatchPay").click(function () {
            Megi.grid("#gridInvoice", "optSelected", {
                callback: function (ids) {
                    mAjax.submit(
                        "/IV/UC/IsSuccessBatch", 
                        { KeyIDs: ids, selectObj: "Invoice_Purchases" },
                        function (res) {
                            if (res.Success) {
                                $.mDialog.show({
                                    mTitle: HtmlLang.Write(LangModule.IV, "NewBatchPayment", "New Batch Payment"),
                                    mWidth: 700,
                                    mHeight: 400,
                                    mShowbg: true,
                                    mShowTitle: true,
                                    mDrag: "mBoxTitle",
                                    mContent: "iframe:/IV/UC/BatchPayment",
                                    mPostData: { selectIds: ids, obj: "Invoice_Purchases" }
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
        queryParam.MStatus = BillList.Status;
        queryParam.MType = IVBase.InvoiceType.Purchase;
        if (selectedIds) {
            queryParam.SelectedIds = selectedIds;
        }
        return queryParam;
    },
    contBillView: function (contId) {
        //选中待付款页签
        $(".m-extend-tabs ul li").removeClass("current");
        $(".m-extend-tabs ul li").eq(4).addClass("current");
        var queryParam = {};
        queryParam.MContactID = contId;
        BillList.bindGrid(4, queryParam);
    },
    //绑定数据列表
    bindGrid: function (type, param) {
        //状态（每个Tab选项卡对应一个状态值）
        var status = 0;
        //列表总列数
        var columnList = null;
        //隐藏内页工具栏中的所有按钮（出查询按钮外）
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
        switch (type) {
            case 0://采购发票主页（柱状图 和 饼状图）
                $('#divHome').show();
                $('#divList').hide();
                if ($.trim($("#divUpcomingChart").html()) === '') {
                    BillList.InitIncomingChart();
                }
                break;
            case 1://所有
                columnList = Megi.getDataGridColumnList(IVBase.columns, [2, 3, 4, 5, 7, 8, 9, 11, 12]);
                break;
            case 2://草稿
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 2, 3, 4, 5, 8, 11, 12]);
                status = IVBase.Status.Draft;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $("#btnPayment").hide();
                $("#btnSchedulePayment").hide();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnDeleteRepeat').hide();
                $('#btnBatchPay').hide();
                break;
            case 3://等待审核
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 2, 3, 4, 5, 8, 11, 12]);
                status = IVBase.Status.WaitingApproval;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $("#btnPayment").hide();
                $("#btnSchedulePayment").hide();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnDeleteRepeat').hide();
                $('#btnBatchPay').hide();
                break;
            case 4://等待支付
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 2, 3, 4, 5, 7, 8, 11, 12]);
                status = IVBase.Status.AwaitingPayment;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnApproval').hide();
                $('#btnDelete').hide();
                $("#btnPayment").show();
                $("#btnSchedulePayment").show();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnDeleteRepeat').hide();
                break;
            case 5://已支付
                columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 2, 3, 4, 5, 7, 8, 11, 12]);
                status = IVBase.Status.Paid;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnApproval').hide();
                $('#btnDelete').hide();
                $("#btnPayment").hide();
                $("#btnSchedulePayment").hide();
                $('#btnSavedAsDraft').hide();
                $('#btnApprovalRepeat').hide();
                $('#btnDeleteRepeat').hide();
                $('#btnBatchPay').hide();
                break;
            case 6:
                columnList = Megi.getDataGridColumnList(IVBase.repeatInvoiceColumns, [0, 1, 2, 3, 4, 6, 7, 9, 10, 11]);
                $('#btnSavedAsDraft').show();
                $('#btnApprovalRepeat').show();
                $('#btnDeleteRepeat').show();
                $('#btnBatchPay').hide();
                break;
        }
        BillList.Status = status;
        //如果点击的不是主页Tab则需要查询列表
        if (type > 0) {
            $('#divList').show();
            //查询参数列表
            var queryParam = (param == undefined || param == null) ? {} : param;
            //获取查询参数
            if ($(".m-adv-search").css("display") == "block") {
                queryParam = $("body").mFormGetForm();
            }
            //状态
            queryParam.MStatus = status;
            //发票类型
            queryParam.MType = IVBase.InvoiceType.Purchase;
            //开始查询数据并绑定数据列表
            Megi.grid("#gridInvoice", {
                url: type == 6 ? "/IV/Bill/GetRepeatBillList" : IVBase.url_getlist,
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
            "/IV/Bill/GetInComingChartData",
            { Type: "'Invoice_Purchase','Invoice_Purchase_Red'" },
            function (jsonData) {
                if (jsonData != null) {
                    BillList.fillStackedData(JSON.parse(jsonData));
                }
            });
    },
    GetOwingMostChart: function () {
        mAjax.post(
            "/IV/Invoice/GetOwingMostChartData",
            { Type: "'Invoice_Purchase','Invoice_Purchase_Red'" },
            function (jsonData) {
                if (jsonData != null) {
                    BillList.fillPieData(jsonData);
                }
            });
    },
    /*用来做渐变的颜色*/
    chartColors: ['#f0c0aa', '#f2a07c', '#f38a5b', '#fa7b43', '#f96422'],
    /*获取渐变色的数组*/
    getGradientColors: function (dataCount) {
        //声称渐变色
        return mColor.Gradient(BillList.chartColors[0], BillList.chartColors[BillList.chartColors.length - 1], dataCount);
    },
    /*给每个data的color赋值*/
    setDataColor: function (data) {
        //
        var colors = BillList.getGradientColors(data.length);
        //每个data都赋值
        for (var i = 0; i < data.length ; i++) {
            //赋值
            data[i].color = colors[i];
        }
        return data;
    },
    fillStackedData: function (jsonData) {
        if (jsonData.data.length == 0) {
            return;
        }

        //等待支付的多语言
        var owingL = HtmlLang.Write(LangModule.IV, "owing", "owing");
        //逾期的多语言
        var dueL = HtmlLang.Write(LangModule.IV, "Due", "due");


        data = jsonData.data;
        labels = jsonData.labels;
        scalSpace = jsonData.scalSpace;
        new iChart.ColumnStacked2D({
            render: 'divUpcomingChart',
            data: BillList.setDataColor(data),
            sub_option: {
                label: false,
                listeners: {
                    click: function (r, e, m) {
                        //全部的结果
                        var nameObj = $.parseJSON(r.options.name);
                        BillList.contBillView(nameObj.MContactID);
                    }
                }
            },
            labels: labels,
            border: 0,
            decimalsnum: 2,
            margin: "0px auto",
            width: 520,
            height: 160,
            //offsetx: 35,
            //label: { color: '#7c8490', font: 'Arial', fontsize: 12 },
            default_mouseover_css: false,
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
                        return "<div onclick='BillList.contBillView(\"" + nameObj.MContactID + "\")'> " + result + "</div>";
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
                //scale: [{
                //    position: 'left',
                //    start_scale: 0,
                //    scale_space: scalSpace,
                //    end_scale: scalSpace * 3,
                //    listeners: {
                //        parseText: function (t, x, y) {
                //            return { text: t }
                //        }
                //    }
                //}]
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
            data: BillList.setDataColor(data),
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
                        BillList.contBillView(r.get('MContactID'));
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
                                    re += "<br /><span class=\"m-chart-font-red\">" + data[j].MOverDue + " " + HtmlLang.Write(LangModule.IV, "Overdue", "overdue") + "</span>";
                                }
                                return "<div onclick='BillList.contBillView(\"" + data[j].MContactID + "\")'> " + re + "</div>";
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
    },
    reload: function (type) {
        BillList.bindGrid(type);
        //刷新Tab合计数据
        IVBase.tabSelRefreshTitle(2);
    }
}
//初始化页面
$(document).ready(function () {
    IVBase.columns[2].title = HtmlLang.Write(LangModule.IV, "From", "From");
    BillList.init();
});