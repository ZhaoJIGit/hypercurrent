var AccountList = {
    IsClickEdit: false,
    IsClickDelete: false,
    init: function () {
        AccountList.initToolbar();
        //测试有没有获取
    },
    initToolbar: function () {
        $("#aNewAccount").click(function () {
            AccountList.editAccount("");
        });
        $("#aNewBankAccount,#aNewBankAccount1").click(function () {
            AccountList.editBankAccount("");
        });
        $("#aNewCreditCard").click(function () {
            AccountList.editCreditCard("");
        });
        $("#btnSearch").click(function () {
            AccountList.bindGrid();
        });

        $("#btnDelete").click(function () {
            AccountList.deleteAccount();
        });
        $("#btnArchive").click(function () {
            AccountList.archiveAccount();
        });
        $("#aNewCash").click(function () {
            AccountList.editCash("");
        });
    },
    editAccount: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddAccount", "Add Account") : HtmlLang.Write(LangModule.Acct, "EditAccount", "Edit Account");
        Megi.dialog({
            title: title,
            width: 550,
            height: 550,
            href: '/BD/Account/AccountEdit/' + id
        });
    },
    editBankAccount: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddBankAccount", "Add Bank Account") : HtmlLang.Write(LangModule.Acct, "EditBankAccount", "Edit Bank Account");
        $.mTab.addOrUpdate(title, '/BD/BDBank/BankAccountEdit/' + id);
    },
    editCreditCard: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddCreditCard", "Add Credit Card") : HtmlLang.Write(LangModule.Acct, "EditCreditCard", "Edit Credit Card");
        $.mTab.addOrUpdate(title, '/BD/BDBank/CreditCardEdit/' + id);
    },
    editCash: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddCash", "Add Cash") : HtmlLang.Write(LangModule.Acct, "EditCash", "Edit Cash");
        $.mTab.addOrUpdate(title, '/BD/BDBank/CashEdit/' + id);
    },
    deleteAccount: function (id) {
        if (id == undefined) {
            Megi.grid("#tabAccount", "deleteSelected", {
                url: "/BD/BDAccount/DeleteAccount", callback: function () {
                    AccountList.reload();
                }
            });
        }
        else {
            Megi.confirm(LangKey.AreYouSureToDelete, function () {
                var obj = {};
                obj.KeyIDs = id;
                mAjax.submit("/BD/BDAccount/DeleteAccount", obj, function (response) {
                    if (response.Success == true) {
                        AccountList.reload();
                    } else {
                        $.mDialog.alert(response.Message);
                    }
                });
            });
        }
    },
    archiveAccount: function () {
        Megi.grid("#tabAccount", "optSelected", {
            url: "/BD/BDAccount/ArchiveAccount", msg: HtmlLang.Write(LangModule.Acct, "SureToArchive", "Are you sure to archive?"), callback: function () {
                AccountList.reload();
            }
        });
    },
    changeTabItem: function (title, index) {
        $("#txtKeyword").val("");
        AccountList.bindGrid();
    },
    reload: function () {
        AccountList.bindGrid();
    },
    afterEdit: function (msg) {
        Megi.displaySuccess("#divMessage", msg);
        AccountList.bindGrid();
    },
    bindGrid: function () {
        var objInfo = AccountList.getTabSelectedInfo();
        var keyword = Megi.encode($("#txtKeyword").val());
        Megi.grid("#tbAll", {
            resizable: true,
            auto: true,
            url: "/BD/BDAccount/GetAccountList",
            queryParams: { group: objInfo.Title, keyword: keyword, isActive: objInfo.IsActive },
            columns: [[{
                title: '<input type=\"checkbox\" >', field: 'MItemID', formatter: function (value, rec, rowIndex) {
                    return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" >";
                }, width: 20, align: 'center', sortable: false
            },
            { title: LangKey.Code, field: 'MNumber', width: 50, sortable: true },
            { title: LangKey.Name, field: 'MName', width: 100, sortable: true },
            { title: LangKey.Type, field: 'MAcctTypeName', width: 50, sortable: true },
            { title: HtmlLang.Write(LangModule.Acct, "TaxRate", "Tax Rate"), field: 'MTaxTypeName', width: 60, sortable: true },
            { title: HtmlLang.Write(LangModule.Acct, "YTD", "YTD"), field: 'ExpectedDate', width: 60, sortable: true, formatter: $.mDate.formatter },
            {
                title: HtmlLang.Write(LangModule.Acct, "Operation", "Operation"), field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                    return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"AccountList.IsClickEdit = true;" + "\" class='list-item-edit'></a><a href='javascript:void(0);' onclick=\"AccountList.IsClickDelete = true;AccountList.deleteAccount('" + rec.MItemID + "');\" class='list-item-del'></a></div>";
                }
            }]],
            onSelect: function (rowIndex, rowData) {

            },
            onClickRow: function (rowIndex, rowData) {
                if (AccountList.IsClickEdit) {
                    if (rowData.MIsBank) {
                        if (rowData.MIsCreditCard) {
                            AccountList.editCreditCard(rowData.MItemID);
                        } else if (rowData.MIsCash) {
                            AccountList.editCash(rowData.MItemID);
                        }
                        else {
                            AccountList.editBankAccount(rowData.MItemID);
                        }
                    } else {
                        AccountList.editAccount(rowData.MItemID);
                    }
                }
                if (AccountList.IsClickEdit || AccountList.IsClickDelete) {
                    $(this).datagrid('unselectRow', rowIndex);
                    AccountList.IsClickEdit = false;
                    AccountList.IsClickDelete = false;
                }
            }
        });
    },
    getTabSelectedInfo: function () {
        var obj = {};
        obj.Title = "";
        obj.IsActive = true;

        var count = $("#tabAccount").find(".tabs>li").length;
        $("#tabAccount").find(".tabs>li").each(function (i) {
            if ($(this).hasClass("tabs-selected")) {
                if (i > 0 && i < count - 1) {
                    obj.Title = $(this).find(".tabs-title").html();
                }
                if (i == count - 1) {
                    obj.IsActive = false;
                }
                return;
            }
        });
        return obj;
    }
}
$(document).ready(function () {
    AccountList.init();
});