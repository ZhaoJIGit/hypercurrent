/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ItemEdit = {
    isEnableGL: $("#hideIsEnableGL").val(),
    isUpdate: $("#hidItemID").val().length > 0,
    init: function () {
        ItemEdit.initAction();
        ItemEdit.saveClick();
        ItemEdit.cancelClick();



        if (ItemEdit.isUpdate) {
            $("body").mFormGet({
                url: "/BD/Item/GetEditInfo/", callback: function (data) {
                    if (data) {
                        if (ItemEdit.isEnableGL) {
                            var code1 = data.MInventoryAccountCode ? data.MInventoryAccountCode : "";
                            $("#cbxIA").combobox("setValue", code1);

                            var code2 = data.MIncomeAccountCode ? data.MIncomeAccountCode : "";
                            $("#cbxInA").combobox("setValue", code2);

                            var code3 = data.MCostAccountCode ? data.MCostAccountCode : "";
                            $("#cbxCA").combobox("setValue", code3);

                            var accountDoms = $(".inventory-account");
                            if (data.MIsExpenseItem) {

                                accountDoms.each(function () {
                                    var expenseAccount = HtmlLang.Write(LangModule.Acct, "ExpenseAccount", "Expense Account");
                                    $("#lblSpecial").text(expenseAccount);
                                    $(this).hide();
                                });

                                $("#ckbExpenseItem").attr("checked", "checked");
                            } else {
                                accountDoms.each(function () {
                                    var costAccount = HtmlLang.Write(LangModule.Acct, "CostAccount", "Cost Account");
                                    $("#lblSpecial").text(costAccount);
                                    $(this).show();
                                });

                                $("#ckbExpenseItem").removeAttr("checked");
                            }
                        }
                    }
                }
            });
        } else {
            $("#MPurTaxTypeID").combobox("setValue", "0");
            $("#MSalTaxTypeID").combobox("setValue", "0");
        }


        if (ItemEdit.isEnableGL) {
            ItemEdit.loadAccountList();
        }
    },
    initAction: function () {
        if (ItemEdit.isEnableGL) {
            var accountDoms = $(".inventory-account");
            $("#ckbExpenseItem").click(function () {
                if ($(this).attr("checked") == "checked") {
                    accountDoms.each(function () {
                        $(this).hide();
                        //费用科目
                        var expenseAccount = HtmlLang.Write(LangModule.Acct, "ExpenseAccount", "Expense Account");
                        $("#lblSpecial").text(expenseAccount);
                    });
                } else {
                    accountDoms.each(function () {
                        var costAccount = HtmlLang.Write(LangModule.Acct, "CostAccount", "Cost Account");
                        $("#lblSpecial").text(costAccount);
                        $(this).show();
                    });
                }
            });
        }
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var toUrl = $(this).attr("href");
            var param = {};

            param.MDesc = $("#txtMDesc").val();
            param.MNumber = $("#txtMNumber").val();
            param.MText = param.MNumber + ":" + param.MDesc;

            if (ItemEdit.isEnableGL) {
                param.MInventoryAccountCode = $("#cbxIA").combobox("getValue");
                param.MIncomeAccountCode = $("#cbxInA").combobox("getValue");
                param.MCostAccountCode = $("#cbxCA").combobox("getValue");
                param.MIsExpenseItem = $("#ckbExpenseItem").attr("checked") == "checked";
            }
            $("#divItemEdit").mFormSubmit({
                url: "/BD/Item/ItemInfoUpd", param: { item: param }, callback: function (msg) {
                    var successMsg = ItemEdit.isUpdate ? HtmlLang.Write(LangModule.Acct, "ItemUpdated", "Item updated") + ": " + $("#txtMNumber").val() : HtmlLang.Write(LangModule.Acct, "ItemAdded", "Item added") + ": " + $("#txtMNumber").val();
                    if (msg.Success) {
                        $.mMsg(successMsg);
                        if (parent && parent.GoItemList) {
                            parent.GoItemList.bindGrid(true);
                        }
                        param.MItemID = msg.ObjectID;

                        $.mDialog.close(0, param);
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            var toUrl = $(this).attr("href");
            $.mDialog.close(0);
            if (parent && parent.GoItemList) {
                parent.GoItemList.reload();
            }

            return false;
        });
    },
    loadAccountList: function () {
    }
}

$(document).ready(function () {
    ItemEdit.init();
});