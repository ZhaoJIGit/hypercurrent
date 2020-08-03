var ExpenseInitEdit = {
    init: function () {
        ExpenseEditBase.IsInit = true;
        $("#btnSave").off("click").on("click", function () {
            ExpenseEdit.saveExpense(IVBase.Status.AwaitingPayment, function (msg) {
                if (msg && msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                } else {
                    $.mMsg(LangKey.SaveSuccessfully);
                    parent.InitBills && parent.InitBills.reloadData();
                    $.mDialog.close();
                }
            });
        });
        $("#btnSaveAndAddAnother").off("click").on("click", function () {
            ExpenseEdit.saveExpense(IVBase.Status.AwaitingPayment, function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                $.mTab.rename(ExpenseEditBase.getTabTitle("NewExpense"));
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.AwaitingPayment;
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(ExpenseEdit.InitEditUrl);
            });
        });

        var expenseId = $("#hidExpenseID").val();
        if (expenseId) {
            $("#selEmployee").combobox("disable");
        }
    }
}

$(document).ready(function () {
    ExpenseInitEdit.init();
});