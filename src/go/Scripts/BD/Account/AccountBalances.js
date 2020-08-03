var AccountBalances = {
    editRowIndex: -1,
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    init: function () {
        AccountBalances.initAction();
        AccountBalances.getAccountListByPage();
    },
    initAction: function () {
        $("#aUpdate").click(function () {
            AccountBalances.saveBalance();
            return false;
        });
        var bankHomeOj = new BDBankHome();



        $("#btnAddBankAccountTop").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[1].type, "", AccountBalances.reload) });
        //新增银行
        $("#btnAddBankAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[1].type, "", AccountBalances.reload) });
        //新增信用卡
        $("#btnAddCreditAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[2].type, "", AccountBalances.reload) });
        //新增现金帐户
        $("#btnAddCashAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[3].type, "", AccountBalances.reload) });
        //新增现金帐户
        $("#btnAddPayPalAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[4].type, "", AccountBalances.reload) });
        //新增现金帐户
        $("#btnAddAlipayAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[5].type, "", AccountBalances.reload) });
    },
    
    getAccountListByPage: function () {
        var searchFilter = "0";

        Megi.grid("#tbAccountBalance", {
            width: 1000,
            resizable: true,
            auto: false,
            pagination: true,
            url: "/BD/Account/GetAccountBalancesByPage",
            queryParams: { searchFilter: searchFilter },
            columns: [[
                    {title: '<input type=\"checkbox\" >', field: '-1', width: 25, align: 'center', sortable: true,hidden:false ,formatter: function (value, rec, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    }
                    },
                    { title: 'MAcctID', field: 'MAcctID', width: 100, align: 'left', sortable: true, hidden: true },
                    { title: HtmlLang.Write(LangModule.Bank, "AccountNumber", "Account Number"), field: 'MBankNo', width: 100, align: 'left', sortable: true },
                    { title: HtmlLang.Write(LangModule.Bank, "Account", "Account"), field: 'MName', width: 100, align: 'left', sortable: true },
                    {
                        title: HtmlLang.Write(LangModule.Bank, "Balance", "Balance"), field: 'MBeginBalanceFor', width: 90, sortable: true, align: 'right', formatter: function (value, rec, rowIndex) {
                            return Megi.Math.toMoneyFormat(value, 2);;
                        }
                    },
                    { title: HtmlLang.Write(LangModule.Bank, "BankTypeName", "Bank Name"), field: 'MBankTypeName', width: 150, align: 'left', sortable: true },
                    { title: "MCyID", field: 'MCyID', width: 90, sortable: false, hidden: true },

                    {
                        title: HtmlLang.Write(LangModule.Bank, "Operation", "Operation"), field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                            if (!AccountBalances.hasChangeAuth) {
                                return "<div>&nbsp;</div>";
                            } else {
                                return "<div class='list-item-action'><a href='javascript:void(0);' onclick='AccountBalances.editBtn(\"" + rec.MAcctID + "\");' class='list-item-edit'></a></div>";
                            }
                        }
                    },
                    { title: '', field: 'MIsCash', width: 100, align: 'left',hidden:'true'},
            ]],
         
            onLoadSuccess: function (data) {
                //加载完成后,判读是否有编辑权限
                if (!AccountBalances.hasChangeAuth) {
                    var col = $(this).datagrid('getColumnOption', "MBeginBalanceFor");

                    //将编辑权限置为空
                    col.editor = null;
                }
                
            }
        });
    },
    
    editBtn: function (accountId) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "UpdateAccountBalances", "Update account balances"),
            width: 400,
            height: 200,
            href: '/BD/Account/AccountBalancesEdit?accountId=' + accountId
        });
    },
    fnArray: function () {
        AccountBalances.getAccountListByPage();
        //$.mDialog.close();
    },
    reload: function () {
        window.location.reload(true);
    }
}
$(document).ready(function () {
    AccountBalances.init();
});