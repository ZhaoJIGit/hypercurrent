var CreditCardEdit = {
    init: function () {
        CreditCardEdit.initAction();
        CreditCardEdit.initForm();
    },
    initAction:function(){
        $("#aStartTrial").click(function () {
            CreditCardEdit.saveAccount();
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
            url: "/Bank/SaveCreditCard", callback: function (msg) {
                var successMsg = $("#hidAccountID").val().length > 0 ? HtmlLang.Write(LangModule.Bank, "CreditCardUpdated", "Credit card updated") + ": " + $("#txtMNumber").val() : HtmlLang.Write(LangModule.Bank, "CreditCardAdded", "Credit card added")+ ": " + $("#txtMNumber").val();
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
    CreditCardEdit.init();
});