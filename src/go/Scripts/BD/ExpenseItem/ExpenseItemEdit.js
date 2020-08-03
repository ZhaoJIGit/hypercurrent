/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ExpenseItemEdit = {
    isEnableGL: $("#hideIsEnableGL").val() == "true" ? true : false,
    expenseItemList: null,
    init: function () {
        ExpenseItemEdit.saveClick();
        ExpenseItemEdit.cancelClick();
        ExpenseItemEdit.rideoClick();

        ExpenseItemEdit.initParentExpenseItem(ExpenseItemEdit.initExpenseItemInfo);
    },
    //初始化顶级的费用报销项目
    initParentExpenseItem: function (callback) {
        var none = HtmlLang.Write(LangModule.BD, "None", "None");

        $("#cbxMparent").combobox({
            valueField: 'MItemID',
            textField: 'MName',
            onChange: function (newValue, oldValue) {
                if (!ExpenseItemEdit.isEnableGL) {
                    return;
                }
                var parentItem = ExpenseItemEdit.getParentItem(newValue);
                if (!parentItem || !parentItem.MAccountCode) {
                    $("#cbxExpenseAccount").combobox("setValue", "");
                    return;
                }

                $("#cbxExpenseAccount").combobox("setValue", parentItem.MAccountCode);
            },
            onLoadSuccess: function () {
                $("#cbxMparent").combobox("setValue", "");

                if (ExpenseItemEdit.isEnableGL) {
                    //特殊字符#8570
                    mAjax.post("/BD/BDAccount/GetBDAccountListByCode?IsActive", {}, function (data) {
                        if (data) {
                            var jsonData = [];
                            if (data.length > 0) {
                                for (var i = 0 ; i < data.length; i++) {
                                    var temp = data[i];
                                    jsonData.push({ MCode: temp.MCode, MFullName: mText.encode(temp.MFullName) });
                                }
                            }
                            var opt = {};
                            opt.valueField = "MCode";
                            opt.textField = "MFullName";
                            opt.data = jsonData;
                            opt.onLoadSuccess = function () {
                                if (callback && $.isFunction(callback)) {
                                    callback();
                                }
                            }
                            $("#cbxExpenseAccount").combobox(opt);
                        }
                    });
                } else {
                    if (callback && $.isFunction(callback)) {
                        callback();
                    }
                }

            }
        });

        var url = "/BD/ExpenseItem/GetParentExpenseItemList?itemId=" + $("#hidItemID").val();

        mAjax.post(url, {}, function (msg) {
            if (msg) {
                ExpenseItemEdit.expenseItemList = msg;
                var data = msg;
                if (data) {
                    var jsonData = [];
                    jsonData.push({ MItemID: 0, MName: none });
                    if (data && data.length > 0) {
                        for (var i = 0 ; i < data.length; i++) {
                            var temp = data[i];
                            jsonData.push({ MItemID: temp.MItemID, MName: mText.encode(temp.MName) });
                        }
                    }

                    data = jsonData;
                }

                $("#cbxMparent").combobox("loadData", data);

            }
        });
    },
    initExpenseItemInfo: function () {
        var id = $("#hidItemID").val();
        if (id) {
            $("body").mFormGet({
                url: "/BD/ExpenseItem/GetEditInfo/", fill: true, callback: function (msg) {
                    if (msg.MIsIncludeAccount) {
                        $("#Expenseitemtable").hide();
                        $("#rideoselectAll").attr("checked", true);
                    } else {
                        $("#rideoselectOne").attr("checked", true);
                        $("#selectAll").hide();
                        $("#Expenseitemtable").show();
                    }

                    if (ExpenseItemEdit.isEnableGL) {
                        $("#cbxExpenseAccount").combobox("setValue", msg.MAccountCode);
                    }

                    if (msg.EntryList == null || msg.EntryList.length == 0) {
                        return;
                    }
                }
            });
        }
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var arr = new Array();
            //去科目
            var model = {};
            if (ExpenseItemEdit.isEnableGL) {
                model.MAccountCode = $("#cbxExpenseAccount").combobox("getValue");
                model.MAccountCode = model.MAccountCode == "0" ? "" : model.MAccountCode;
            }

            $("#divItemEdit").mFormSubmit({
                url: "/BD/ExpenseItem/UpdateExpenseItem", param: { expenseitem: model }, callback: function (msg) {
                    var successMsg = $("#hidItemID").val().length > 0 ? HtmlLang.Write(LangModule.BD,
                        "ExpenseInfoUpd", "Expense Expense Item updated") : HtmlLang.Write(LangModule.BD, "ExpenseItemAdded", "Expense Item added");
                    if (msg.Success) {
                        $.mDialog.message(successMsg);
                        if (parent && parent.GoExpenseItemList) {
                            parent.GoExpenseItemList.reload();
                        }
                        $.mDialog.close();
                    } else {
                        $.mDialog.warning(msg.Message);
                    }
                }
            });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            var toUrl = $(this).attr("href");
            $.mDialog.close();
            parent.GoExpenseItemList.reload();
            return false;
        });
    },
    rideoClick: function () {
        $("#rideoselectAll").attr("checked", true);
        $("#Expenseitemtable").hide();
        $("#rideoselectAll").click(function () {
            $("#Expenseitemtable").hide();
            $("#selectAll").show();
        }),
        $("#rideoselectOne").click(function () {
            $("#Expenseitemtable").show();
            $("#selectAll").hide();
        });
    },
    getParentItem: function (id) {
        if (ExpenseItemEdit.expenseItemList && ExpenseItemEdit.expenseItemList.length > 0) {
            for (var i = 0 ; i < ExpenseItemEdit.expenseItemList.length; i++) {
                var item = ExpenseItemEdit.expenseItemList[i];

                if (item.MItemID == id) {
                    return item;
                }
            }
        }
        return null;
    }

}

$(document).ready(function () {
    ExpenseItemEdit.init();
});