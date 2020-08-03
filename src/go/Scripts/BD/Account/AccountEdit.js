
var AccountEdit = {
    init: function () {
        AccountEdit.initAction();
        AccountEdit.initForm();
        $("body").find("input[type='text']")[0].focus();
    },
    initAction:function(){
        $("#aStartTrial").click(function () {
            AccountEdit.saveAccount();
        });
    },
    initForm: function () {
        Megi.Form.get({
            url: "/BD/Account/GetAccount"
        });
        //HtmlLang.Write(LangModule.Acct, "", "aaaa");
        var name = LangKey.Name;
    },
    saveAccount: function () {
        if ($(".m-form-key").val() == "") {
            $("input[name='MIsActive']").val("true");
        }
        Megi.Form.submit({
            url: "/BD/Account/SaveAccount", callback: function (msg) {
                var successMsg = $("#hidAccountID").val().length > 0 ? HtmlLang.Write(LangModule.Acct, "AccountUpdated", "Account updated") + ": " + $("#txtMNumber").val() : HtmlLang.Write(LangModule.Acct, "AccountAdded", "Account added") + ": " + $("#txtMNumber").val();
                if (msg.Success) {
                   
                    parent.AccountList.afterEdit(successMsg);
                    $.mDialog.close(); // Megi.closeDialog();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    },
}
$(document).ready(function () {
    AccountEdit.init();
});
