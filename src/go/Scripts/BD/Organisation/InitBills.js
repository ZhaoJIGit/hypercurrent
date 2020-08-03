/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var InitBills = {
    initBalanceOver: $("#hideInitBalanceOver").val(),
    gridKey: "#gridInitData",
    gridDiv: ".datagrid-div",
    OrgId: $("#hidOrgCode").val(),
    CurrentType: 0,
    IsClick: true,
    url_InvoiceEdit: "/IV/Invoice/InvoiceInitEdit",
    url_InvCreditNoteEdit: "/IV/Invoice/CreditNoteInitEdit",
    url_BillsEdit: "/IV/Bill/BillInitEdit",
    url_BillsCreditNoteEdit: "/IV/Bill/CreditNoteInitEdit",
    url_ReceiveEdit: "/IV/Receipt/ReceiptInitEdit",
    url_SpendEdit: "/IV/Payment/PaymentInitEdit",
    url_ExpenseEdit: "/IV/Expense/ExpenseInitEdit",
    init: function () {
        InitBills.initTab();
        InitBills.initAction();
        InitBills.initToolbar();
        IVBase.bindEvent = function () {
            InitBills.bindGrid(InitBills.CurrentType);
        }
    },
    initTab: function () {
        var typeId = Number($("#hidInitType").val());
        $('#tabInit').tabs('select', typeId);
        InitBills.bindGrid(typeId);
        InitBills.CurrentType = typeId;
        $("#aAddInvoice").show();
        $("#aAddInvCreditNote").show();
        $("#aDeleteInvoice").show();
        $("#tabInit").tabs({
            onSelect: function (title, index) {
                InitBills.bindGrid(index);
                InitBills.CurrentType = index;
                $(window).resize();
            }
        });
    },
    initAction: function () {

        $("#aDeleteInvoice").click(function () {
            InitBills.activeFn(InitBills.invoiceDel);

        });
        $("#aDeleteBills").click(function () {
            InitBills.activeFn(InitBills.invoiceDel);


        });
        $("#aDeleteReceive").click(function () {
            InitBills.activeFn(InitBills.receiveDel);

        });
        $("#aDeleteSpend").click(function () {
            InitBills.activeFn(InitBills.spendDel);
        });
        $("#aDeleteExpense").click(function () {
            InitBills.activeFn(InitBills.expenseDel);
        });
    },
    initToolbar: function () {
        $("#aAddInvoice").click(function () {
            InitBills.activeFn(InitBills.AddInvoiceDialog);

        });
        $("#aAddInvCreditNote").click(function () {
            InitBills.activeFn(InitBills.AddInvCreditNoteDialog);

        });
        $("#aAddBills").click(function () {
            InitBills.activeFn(InitBills.AddBillsDialog);
        });
        $("#aAddBillsCreditNote").click(function () {
            InitBills.activeFn(InitBills.AddBillsCreditNoteDialog);

        });
        $("#aAddReceive").click(function () {
            InitBills.activeFn(InitBills.AddReceiveDialog);

        });
        $("#aAddSpend").click(function () {
            InitBills.activeFn(InitBills.AddSpendDialog);
        });
        $("#aAddExpense").click(function () {
            InitBills.activeFn(InitBills.AddExpenseDialog);
        });
    },
    activeFn: function (callback) {
        var msg = HtmlLang.Write(LangModule.IV, "InitBalanceIsOver", "The initial balance has been completed and is not allowed to initialize the document operation!")
        if (InitBills.initBalanceOver) {
            $.mDialog.alert(msg);
        } else {
            callback();
        }
    },
    invoiceDel: function () {
        var param = {};
        //表示初始化单据删除
        param.MIsInit = true;
        IVBase.deleteList(InitBills.gridKey, InitBills.deleteInitListCallBack, param
       )
    },
    deleteInitListCallBack: function (msg) {
        if (msg.Success) {
            InitBills.bindGrid(InitBills.CurrentType);
        } else {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Have cancel after verification, not allowed to delete"));
        }
    },
    receiveDel: function () {
        var param = {};
        //表示初始化单据删除
        param.MIsInit = true;

        Megi.grid(InitBills.gridKey, "deleteSelected", {
            url: "/IV/Receipt/DeleteReceiveList", callback: function () {
                InitBills.bindGrid(InitBills.CurrentType);
            }, param: param
        });
    },
    spendDel: function () {
        var param = {};
        //表示初始化单据删除
        param.MIsInit = true;
        Megi.grid(InitBills.gridKey, "deleteSelected", {
            url: "/IV/Payment/DeletePaymentList", callback: function () {
                InitBills.bindGrid(InitBills.CurrentType);
            }, param: param
        });
    },
    expenseDel: function () {
        var param = {};
        //表示初始化单据删除
        param.MIsInit = true;
        Megi.grid(InitBills.gridKey, "deleteSelected", {
            url: "/IV/Expense/DeleteExpenseList", callback: function () {
                InitBills.bindGrid(InitBills.CurrentType);
            }, param: param
        });
    },
    getPaymentReceiptColumns: function () {
        var columnConfigs = IVBase.columns;
        var arrColumnIndex = [0, 2, 3, 4, 8];
        var cols = new Array();
        for (var i = 0; i < arrColumnIndex.length; i++) {
            cols.push(columnConfigs[arrColumnIndex[i]]);
        }
        cols.push(
            {
                title: HtmlLang.Write(LangModule.IV, "Amount", "Amount"), field: 'MTaxTotalAmtFor2', width: 100, align: "right", sortable: true, formatter: function (value, rec, rowIndex) {
                    var result = Math.abs(rec.MTaxTotalAmtFor);
                    if (rec.MOrgCyID == rec.MCyID) {
                        return Megi.Math.toMoneyFormat(result, 2);
                    } else {
                        return "<span class='iv-cy'  onmouseover=\"InitBills.showLocalCurrency(this," + rec.MTaxTotalAmt + ",'" + rec.MOrgCyID + "');\">" + rec.MCyID + "</span>" + Megi.Math.toMoneyFormat(result, 2);
                    }
                }
            }
        );

        cols.push(columnConfigs[12]);
        cols.push(columnConfigs[13]);
        var result = new Array();
        result.push(cols);

        return result;
    },
    showLocalCurrency: function (selector, totalTaxAmt, localCyId) {
        $(selector).mLocalCyTooltip(totalTaxAmt, localCyId);
        $(selector).tooltip("show")
    },
    getExpenseColumns: function () {
        var columnConfigs = ExpenseList.columns;
        var arrColumnIndex = [0, 1, 2, 3, 4, 5, 6, 7, 10, 11];
        var cols = new Array();
        for (var i = 0; i < arrColumnIndex.length; i++) {
            cols.push(columnConfigs[arrColumnIndex[i]]);
        }
        var result = new Array();
        result.push(cols);

        return result;
    },
    bindGrid: function (type) {
        var status = IVBase.Status.AwaitingPayment;
        var url;
        var billType;
        var columnList;
        switch (type) {
            case 0:
                url = IVBase.url_getlist;
                billType = IVBase.InvoiceType.Sale;
                IVBase.columns[2].title = HtmlLang.Write(LangModule.IV, "IVTo", "To");
                if (!InitBills.initBalanceOver) {
                    columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 1, 2, 3, 4, 5, 8, 12]);
                } else {
                    columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 1, 2, 3, 4, 5, 8, 12]);
                }

                break;
            case 1:
                $("#aAddBills").show();
                $("#aAddBillsCreditNote").show();
                $("#aDeleteBills").show();
                url = IVBase.url_getlist;
                billType = IVBase.InvoiceType.Purchase;
                IVBase.columns[2].title = HtmlLang.Write(LangModule.IV, "IVFrom", "From");
                if (!InitBills.initBalanceOver) {
                    columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 2, 3, 4, 5, 8, 12]);
                } else {
                    columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 2, 3, 4, 5, 8, 12]);
                }
                break;
            case 2:
                IVBase.columns[2].title = HtmlLang.Write(LangModule.IV, "IVTo", "To");
                $("#aAddReceive").show();
                $("#aDeleteReceive").show();
                url = "/IV/Receipt/GetInitReceiveListByPage";
                columnList = InitBills.getPaymentReceiptColumns();
                break;
            case 3:
                IVBase.columns[2].title = HtmlLang.Write(LangModule.IV, "IVFrom", "From");
                $("#aAddSpend").show();
                $("#aDeleteSpend").show();
                url = "/IV/Payment/GetInitPaymentListByPage";
                columnList = InitBills.getPaymentReceiptColumns();
                break;
            case 4:
                IVBase.columns[2].title = HtmlLang.Write(LangModule.IV, "IVTo", "To");
                $("#aAddExpense").show();
                $("#aDeleteExpense").show();
                url = "/IV/Expense/GetInitExpenseListByPage";
                columnList = InitBills.getExpenseColumns();
                break;
        }
        var queryParam = {}
        queryParam.MStatus = 0;
        queryParam.MType = billType;
        queryParam.MIsOnlyInitData = true;
        queryParam.MConversionDate = $("#hidConversionDate").val();

        Megi.grid(InitBills.gridKey, {
            url: url,
            pagination: true,
            resizable: true,
            auto: true,
            fitColumns: true,
            pagination: true,
            collapsible: true,
            scrollY: true,
            width: $(InitBills.gridDiv).width() - 5,
            height: ($("body").height() - $(InitBills.gridDiv).offset().top),
            queryParams: queryParam,
            columns: columnList
        });
    },
    AddInvoiceDialog: function (mid) {
        if (mid == undefined) { mid = ""; }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "InitSalesInvoice", "Init Sales Invoice"),
            width: 900,
            height: 499,
            href: InitBills.url_InvoiceEdit + "/" + mid + "?showAdvance=false",
            closeFns: InitBills.dialogCloseFn
        });
    },
    AddInvCreditNoteDialog: function (mid) {
        if (mid == undefined) { mid = ""; }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "InitSalesCreditNote", "Init Sales Credit Note"),
            width: 900,
            height: 499,
            href: InitBills.url_InvCreditNoteEdit + "/" + mid + "?showAdvance=false",
            closeFns: InitBills.dialogCloseFn
        });
    },
    AddBillsDialog: function (mid) {
        if (mid == undefined) { mid = ""; }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "InitBills", "Init Bills"),
            width: 900,
            height: 499,
            href: InitBills.url_BillsEdit + "/" + mid + "?showAdvance=false",
            closeFns: InitBills.dialogCloseFn
        });
    },
    AddBillsCreditNoteDialog: function (mid) {
        if (mid == undefined) { mid = ""; }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "InitBillsCreditNote", "Init Bills Credit Note"),
            width: 900,
            height: 499,
            href: InitBills.url_BillsCreditNoteEdit + "/" + mid + "?showAdvance=false",
            closeFns: InitBills.dialogCloseFn
        });
    },
    AddReceiveDialog: function (mid, bankId) {
        if (mid == undefined) { mid = ""; }
        if (!bankId) {
            bankId = "";
        }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "InitReceive", "Init Receive"),
            width: 900,
            height: 499,
            href: InitBills.url_ReceiveEdit + "/" + mid + "?showAdvance=false&showBnkAcct=true&acctid=" + bankId,
            closeFns: InitBills.dialogCloseFn
        });
    },
    AddSpendDialog: function (mid, bankId) {
        if (!bankId) {
            bankId = "";
        }
        if (mid == undefined) { mid = ""; }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "InitSpend", "Init Spend"),
            width: 900,
            height: 499,
            href: InitBills.url_SpendEdit + "/" + mid + "?showAdvance=false&showBnkAcct=true&acctid=" + bankId,
            closeFns: InitBills.dialogCloseFn
        });
    },
    AddExpenseDialog: function (mid) {
        if (mid == undefined) { mid = ""; }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "InitExpense", "初始化费用报销单"),
            width: 900,
            height: 499,
            href: InitBills.url_ExpenseEdit + "/" + mid + "?showAdvance=false",
            closeFns: InitBills.dialogCloseFn
        });
    },
    dialogCloseFn: function () {
        var title = HtmlLang.Write(LangModule.BD, "AccountBalances", "Account Balances");
        $.mTab.refresh(title, '/BD/BDAccount/AccountBalances', false);

        //var titleInit = HtmlLang.Write(LangModule.My, " initializewizard", "Initialize Wizard");
        //var url = $("#hideGoServer").val();
        //$.mTab.refresh(1, url + '/BD/Setup/GLOpeningBalance', false);
    },
    reloadData: function () {
        $(InitBills.gridKey).datagrid('reload');
    },
    //控制单据日期小于启用日期
    ControlBizDate: function (orgid, bizdate, callback) {
        mAjax.post("/BD/Organisation/GetBasicInfo", null, function (data) {
            if (data != null) {
                //var conversionDate= $.mDate.format(data.MConversionDate);
                var conversionDate = MegiDate.parser(data.MConversionDate);
                if (new Date(bizdate) >= new Date(conversionDate)) {
                    $.mDialog.alert(HtmlLang.Write(LangModule.BD, "DateBeforeConversionDate", "The Date must be before your conversion date") + "(" + conversionDate + ")");
                    return;
                }
            }
            callback();
        })
    }
}

$(document).ready(function () {
    InitBills.init();
});