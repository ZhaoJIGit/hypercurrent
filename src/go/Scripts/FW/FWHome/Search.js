(function () {
    var mSearch = (function () {
        //
        var searchBth = "#btnSearch";
        //
        var bizObjectSel = "#bizObjectSel";
        //
        var bizColumnSel = "#bizColumnSel";
        //
        var operateSel = "#operateSel";
        //
        var keywordInput = "#keywordInput";

        //
        var dateInput = "#dateInput";

        var isBizDateColumn = false;

        var searchDateInputClass = "m-search-date";

        var searchDateInput = "<input class='" + searchDateInputClass + "' />";

        //
        var bizObjectList = [
            { text: HtmlLang.Write(LangModule.Common, "All", "全部"), value: "" },
             { text: HtmlLang.Write(LangModule.Common, "Invoice_Sale", "销售单"), value: "Invoice_Sales" },
              { text: HtmlLang.Write(LangModule.Common, "Invoice_Purchase", "采购单"), value: "Invoice_Purchases" },
               { text: HtmlLang.Write(LangModule.Common, "Expense_Claims", "费用报销"), value: "Expense" },
                { text: HtmlLang.Write(LangModule.Common, "Receipts", "收款单"), value: "Receive" },
                 { text: HtmlLang.Write(LangModule.Common, "Payments", "付款单"), value: "Payment" },
                  { text: HtmlLang.Write(LangModule.Common, "Voucher", "凭证"), value: "Voucher" },
                   { text: HtmlLang.Write(LangModule.Common, "Pay_Salary", "工资单"), value: "Pay_Salary" },
                    { text: HtmlLang.Write(LangModule.FP, "MegiFapiao", "开票单"), value: "MegiFapiao" },
                     { text: HtmlLang.Write(LangModule.Common, "Transferbill", "转账单"), value: "Transfer" },
        ];
        //
        var bizColumnList = [
            { text: HtmlLang.Write(LangModule.Common, "All", "全部"), value: "" },
             { text: HtmlLang.Write(LangModule.Common, "Description", "描述"), value: "MDesc" },
             { text: HtmlLang.Write(LangModule.Common, "Explanation", "MReference"), value: "MReference" },
              { text: HtmlLang.Write(LangModule.IV, "Number", "单据号"), value: "MNumber" },
               { text: HtmlLang.Write(LangModule.IV, "Amount", "金额"), value: "MAmount" },
                { text: HtmlLang.Write(LangModule.Contact, "ContactName", "联系人"), value: "MContactName" },
                 { text: HtmlLang.Write(LangModule.Common, "BizDate", "业务日期"), value: "MBizDate" },
        ];
        //
        var operateList = [
            { text: HtmlLang.Write(LangModule.Common, "All", "全部"), value: "" },
            { text: HtmlLang.Write(LangModule.Common, "Contain", "包含"), value: 5 },
            { text: HtmlLang.Write(LangModule.Common, "NotContains", "不包含"), value: 6 },
            { text: HtmlLang.Write(LangModule.Common, "LessThan", "小于"), value: 1 },
            { text: HtmlLang.Write(LangModule.Common, "Equal", "等于"), value: 2 },
             { text: HtmlLang.Write(LangModule.Common, "NotEqual", "不等于"), value: 9 },
            { text: HtmlLang.Write(LangModule.Common, "GreaterThan", "大于"), value: 0 },
        ];
        //
        var IVStatus = {
            Draft: { value: 1, text: HtmlLang.Write(LangModule.IV, "Draft", "草稿") },
            WaitingApproval: { value: 2, text: HtmlLang.Write(LangModule.IV, "WaitingApproval", "等待审核") },
            AwaitingPayment: { value: 3, text: HtmlLang.Write(LangModule.IV, "AwaitingPayment", "等待付款") },
            Paid: { value: 4, text: HtmlLang.Write(LangModule.IV, "Paid", "已付款") }
        };
        var IssueStatus = {
            NotIssued: { value: 0, text: HtmlLang.Write(LangModule.IV, "NotIssued", "等待开票") },
            PartlyIssued: { value: 1, text: HtmlLang.Write(LangModule.IV, "PartlyIssued", "部分开票") },
            Issued: { value: 2, text: HtmlLang.Write(LangModule.IV, "Issued", "完全开票") },
        };
        //
        var VoucherStatus = {
            Draft: { value: 0, text: HtmlLang.Write(LangModule.IV, "Draft", "草稿") },
            Approved: { value: 1, text: HtmlLang.Write(LangModule.IV, "Approved", "已审核") },
        };
        //
        var mSearch = function () {
            //
            var that = this;

            //
            this.init = function () {
                //
                this.initEvent();
                //
                this.initCombobox();
                //展示一个空的表格
                this.bindGrid([]);
            }
            //
            this.initEvent = function () {
                //
                $("#btnSearch").off("click.search").on("click.search", function () {
                    //
                    that.bindGrid();
                    //
                    $("#dataGrid").attr("inited", 1);
                });
            };
            //初始化combobox
            this.initCombobox = function () {
                //初始化业务单据类型
                $(bizObjectSel).combobox({
                    textField: "text",
                    valueFeild: "value",
                    editable: false,
                    data: bizObjectList,
                    onSelect: function (value) {
                        //
                        that.handleSelectDisable();
                    }
                });
                //初始化查询列
                $(bizColumnSel).combobox({
                    textField: "text",
                    valueFeild: "value",
                    editable: false,
                    data: bizColumnList,
                    onSelect: function (row) {
                        //如果选了全部的列，则不能添加筛选条件了
                        if (row.value) {
                            //如果是业务日期
                            if (row.value == "MBizDate") {
                                //
                                isBizDateColumn = true;
                                //
                                $(keywordInput).val("").hide();
                                //
                                if ($("." + searchDateInputClass).length == 0) {
                                    //
                                    $(searchDateInput).insertAfter($(keywordInput));
                                }
                                //初始化日期选择
                                $("." + searchDateInputClass).attr("inited", 1).datebox();
                            }
                            else {
                                //
                                isBizDateColumn = false;
                                //
                                $("." + searchDateInputClass).attr("inited") == 1 && $("." + searchDateInputClass).datebox("destroy");
                                //
                                $(keywordInput).val("").show();
                            }
                        }
                        //
                        that.handleSelectDisable();
                    }
                });
                //初始化比较负
                $(operateSel).combobox({
                    textField: "text",
                    valueFeild: "value",
                    editable: false,
                    data: operateList,
                    onSelect: function (row) {
                        //
                        that.handleSelectDisable();
                    }
                });
                //加载完成之后，需要统一算一下
                that.handleSelectDisable();
            };
            //处理统一的column operate keyword是否可用
            this.handleSelectDisable = function () {
                //
                isBizDateColumn = $(bizColumnSel).combobox("getValue") == "MBizDate";
                //操作符
                var isAllOperate = !$(operateSel).combobox("getValue");
                //是否是所有的列
                var isAllColumn = !$(bizColumnSel).combobox("getValue");
                //
                if (isBizDateColumn) {
                    //输入框不可输入
                    $(keywordInput).hide();
                    //
                    $("." + searchDateInputClass).length > 0 && $("." + searchDateInputClass).datebox("enable");
                }
                else {
                    //输入框不可输入
                    $(keywordInput).show();
                    //
                    $("." + searchDateInputClass).length > 0 && $("." + searchDateInputClass).datebox("distroy");
                }
                //
                if (isAllColumn) {
                    //比较符号选择清空
                    $(operateSel).combobox("setValue", "").combobox("disable");
                }
                else {
                    //比较符号选择清空
                    $(operateSel).combobox("enable");
                }
                //如果选择了全部的操作符
                if (isAllOperate) {
                    //输入框不可输入
                    $(keywordInput).val("").attr("disabled", "disabled");
                    //
                    $("." + searchDateInputClass).length > 0 && $("." + searchDateInputClass).datebox("setValue", "").datebox("disable")
                }
                else {
                    //输入框不可输入
                    $(keywordInput).removeAttr("disabled");
                    //
                    $("." + searchDateInputClass).length > 0 && $("." + searchDateInputClass).datebox("enable");
                }
            }
            //获取对象的链接
            this.triggerBizObejctLink = function (row) {
                //
                var title = url = "";
                //
                switch (row.MType) {
                    //
                    case "Invoice_Sale":
                        var tabTitle = HtmlLang.Write(LangModule.IV, "ViewInvoice", "View Invoice");
                        if (row.MValue1 == IVStatus.AwaitingPayment.value) {
                            tabTitle = HtmlLang.Write(LangModule.IV, "EditInvoice", "Edit Invoice");
                        }
                        $.mTab.addOrUpdate(tabTitle, "/IV/Invoice/InvoiceView/" + row.MID + "?sv=1");
                        break;
                    case "Invoice_Sale_Red":
                        var tabTitle = HtmlLang.Write(LangModule.IV, "CreditNote", "View Credit Note");
                        $.mTab.addOrUpdate(tabTitle, "/IV/Invoice/CreditNoteView/" + row.MID + "?sv=1");
                        break;
                    case "Invoice_Purchase":
                        var tabTitle = HtmlLang.Write(LangModule.IV, "ViewBill", "View Bill");
                        if (row.MValue1 == IVStatus.AwaitingPayment.value) {
                            tabTitle = HtmlLang.Write(LangModule.IV, "EditBill", "Edit Bill");
                        }
                        $.mTab.addOrUpdate(tabTitle, "/IV/Bill/BillEdit/" + row.MID + "?sv=1");
                        break;
                    case "Invoice_Purchase_Red":
                        var tabTitle = HtmlLang.Write(LangModule.IV, "ViewDebitNote", "View Credit Note");
                        $.mTab.addOrUpdate(tabTitle, "/IV/Bill/CreditNoteView/" + row.MID + "?sv=1");
                        break;
                    case "Expense":
                        title = "";
                        url = "/IV/Expense/ExpenseView/" + row.MID;
                        var tabTitle = HtmlLang.Write(LangModule.IV, "ViewExpense", "View Expense");
                        //等待付款，还可以改预计付款日期，所以标题是编辑：Edit Expense
                        if (row.MStatus == IVStatus.AwaitingPayment.value) {
                            tabTitle = HtmlLang.Write(LangModule.IV, "EditExpense", "Edit Expense");
                        }
                        $.mTab.addOrUpdate(tabTitle, "/IV/Expense/ExpenseView/" + row.MID + "?sv=1");
                        break;
                    case "Receive":
                        var tabTitle = HtmlLang.Write(LangModule.Bank, "ReceiveMoney", "Receive Money");
                        $.mTab.addOrUpdate(tabTitle, '/IV/Receipt/ReceiptView/' + row.MID + '?acctId=' + row.MBankID + "&sv=1", true);
                        break;
                    case "Payment":
                        var tabTitle = HtmlLang.Write(LangModule.Bank, "SpendMoney", "Spend Money");
                        $.mTab.addOrUpdate(tabTitle, '/IV/Payment/PaymentView/' + row.MID + '?acctId=' + row.MBankID + "&sv=1", true);
                        break;
                    case "Transfer":
                        var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewTransfer", "View Transfer");
                        $.mTab.addOrUpdate(tabTitle, '/IV/IVTransfer/IVTransferHome?MID=' + row.MID + "&bankId=" + row.MBankID, true);
                        break;
                    case "Voucher":
                        var tabTitle = HtmlLang.Write(LangModule.GL, "GL", "总账");
                        mTab.addOrUpdate(tabTitle + "-" + row.MNumber, "/GL/GLVoucher/GLVoucherEdit?MItemID=" + row.MID, false, true, true, true);
                        break;
                    case "Pay_Salary":
                        var tabTitle = HtmlLang.Write(LangModule.PA, "ViewSalaryList", "查看工资单");
                        mTab.addOrUpdate(tabTitle, "/PA/SalaryPayment/SalaryPaymentEdit/" + row.MID, false, true, true, true);
                        break;
                    case "MegiFapiao":
                        //
                        var title = HtmlLang.Write(LangModule.FP, "EditSaleTable", "编辑销售开票单");
                        //
                        if (row.MValue2 == 1) {
                            //
                            title = HtmlLang.Write(LangModule.FP, "EditPurchaseTable", "编辑采购开票单");
                        }
                        mTab.addOrUpdate(title, '/FP/FPHome/FPEditTable?tableId=' + row.MID + '&invoiceType=' + row.MValue2, false, true, true, true);
                        break;
                    default:
                        return "";
                }
            };
            //根据mType获取类型的多语言
            this.getTypeName = function (type) {
                //
                for (var i = 1; i < bizObjectList.length ; i++) {
                    //比较名字
                    if (type.indexOf(bizObjectList[i].value.substring(0, bizObjectList[i].value.length - 1)) == 0) {
                        //获取多语言值
                        return bizObjectList[i].text;
                    }
                }
            }
            //根据类型显示单据编号
            this.getNumberByType = function (number, row) {
                //如果是凭证，则需要加上GL-
                if (row.MType == bizObjectList[6].value) {
                    //
                    return "GL-" + row.MNumber;
                }
                else if (row.MType == bizObjectList[8].value) {
                    //开票单
                    return FPHome.GetFullTableNumber(row.MNumber, row.MValue2);
                }
                else {
                    return row.MNumber;
                }
            }
            //绑定数据
            this.bindGrid = function (data) {
                //
                var param = that.getParam();
                //如果已经初始化了，不能再显示空表格了
                data = $("#dataGrid").attr("inited") == "1" ? false : data;
                //
                $("#dataGrid").datagrid({
                    pagination: true,
                    resizable: true,
                    auto: true,
                    fitColumns: true,
                    pagination: true,
                    collapsible: true,
                    scrollY: true,
                    width: $('#dataGridDiv').width() - 5,
                    height: ($(".m-search-div").parent().height() - $('#dataGridDiv').offset().top + $(".m-search-div").parent().offset().top - 5),
                    pagination: true,
                    url: data ? "" : "/FW/FWHome/GetSearchBizData",
                    data: data ? data : "",
                    queryParams: param,
                    columns: [[
                         /*1分类*/
                         {
                             title: HtmlLang.Write(LangModule.My, "Category", "Category"), field: 'MType', width: 50, align: 'center', sortable: true, formatter: function (value, rec) {
                                 //
                                 return that.getTypeName(value);
                             }
                         },
                         /*2业务日期*/
                         { title: HtmlLang.Write(LangModule.My, "LastUpdDate", "业务日期"), field: 'MBizDate', width: 50, align: 'center', sortable: true, formatter: mDate.formatter },
                         /*2业务日期*/
                         { title: HtmlLang.Write(LangModule.IV, "Number", "编号"), field: 'MNumber', width: 50, align: 'center', sortable: true, formatter: that.getNumberByType },
                         /*3.描述*/
                         { title: HtmlLang.Write(LangModule.Common, "Description", "描述"), field: 'MDesc', width: 90, align: 'center', sortable: true },
                         /*4联系人*/
                         { title: HtmlLang.Write(LangModule.My, "EmployeeOrContact", "联系人/员工"), field: 'MContactName', width: 120, align: 'left', sortable: true },
                         /*5备注*/
                         { title: HtmlLang.Write(LangModule.Common, "Explanation", "MReference"), field: 'MReference', width: 90, align: 'left', sortable: true },
                         /*6金额*/
                         {
                             title: HtmlLang.Write(LangModule.PA, "Amount", "金额"), field: 'MAmount', width: 50, align: 'right', sortable: true, formatter: function (value, rec) {
                                 //
                                 return mMath.toMoneyFormat(value) + "[" + rec.MCyID + "]";
                             }
                         },
                         {
                             //操作
                             title: HtmlLang.Write(LangModule.IV, "Operation", "Operation"), field: 'Action', align: 'center', width: 50, sortable: false, formatter: function (value, rec, rowIndex) {
                                 //
                                 return "<div class='list-item-action'><a href=\"javascript:void(0);\" keyname='MID' keyvalue='" + rec.MID + "' class='list-item-edit m-search-grid-edit'></a></div>";
                             }
                         }
                    ]],
                    onLoadSuccess: function () {
                        //resize一下
                        $(window).resize();
                        //绑定表格里面的事件
                        that.bindGridEvent();
                    }
                });
            };
            //初始化表格里面的参数
            this.bindGridEvent = function () {
                //获取所有的编辑按钮
                $(".m-search-grid-edit").each(function () {
                    //
                    var x = this;
                    //点击的时候需要跳转
                    $(this).off("click").on("click", function () {
                        //
                        var row = mEasyUI.mGetDataGridSingleRowData("#dataGrid", x);
                        //触发事件
                        that.triggerBizObejctLink(row);
                    });
                });
            }
            //获取所有的参数
            this.getParam = function () {
                //
                var filter = {};
                //单据类型
                filter.BizObjectList = !!$(bizObjectSel).combobox("getValue") ? [$(bizObjectSel).combobox("getValue")] : [];
                //列
                filter.BizColumn = $(bizColumnSel).combobox("getValue");
                //运算符
                filter.SqlOperator = $(operateSel).combobox("getValue");
                //关键字
                filter.KeyWord = isBizDateColumn ? $("." + searchDateInputClass).combobox("getValue") : $(keywordInput).val();
                //
                return filter;
            }
        };
        return mSearch;
    })();
    window.mSearch = mSearch;
})();