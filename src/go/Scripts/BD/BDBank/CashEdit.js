var CashEdit = {
    init: function () {
        CashEdit.initAction();
        CashEdit.initForm();
    },
    initAction:function(){
        $("#aStartTrial").click(function () {
            CashEdit.saveAccount();
        });
    },
    initForm: function () {
        Megi.Form.get({
            url: "/Account/GetAccount"
        });
        if ($("#hidAccountID").val().length > 0) {
            $("#selCurrencyCode").next().find(".combo-arrow").unbind();
            $("#selCurrencyCode").next().find(".combo-text").attr("disabled","disabled");
        }
    },
    saveAccount: function () {
        if ($(".mg-form-key").val() == "") {
            $("input[name='MIsActive']").val("true");
        }
        Megi.Form.submit({
            url: "/Bank/SaveCash", callback: function (msg) {
                var successMsg = $("#hidAccountID").val().length > 0 ? HtmlLang.Write(LangModule.Bank, "CashUpdated", "Cash updated") + ": " + $("#txtMNumber").val() : HtmlLang.Write(LangModule.Bank, "CashAdded", "Cash added")+ ": " + $("#txtMNumber").val();
                if (msg.Success) {
                    if (parent.AccountList != undefined) {
                        parent.AccountList.afterEdit(successMsg);
                    }
                    else if (parent.BankAccountList != undefined) {
                        parent.BankAccountList.reload();
                    }
                    Megi.closeDialog();
                } else {
                    Megi.alert(msg.Message);
                }
            }
        });
    },
}
$(document).ready(function () {
    CashEdit.init();
});