var BDBankRuleEdit = {
    init: function () {
        BDBankRuleEdit.initAction();
        BDBankRuleEdit.initForm();
    },
    initAction: function () {
        $("#aSave").click(function () {
            BDBankRuleEdit.saveBankRule();
        });
    },
    initForm: function () {
        var ruleId = $("#hidBankRuleID").val();

        //如果ID不为空，则去取数据。
        if (ruleId) {
            $("body").mFormGet({
                url: "/BD/BDBank/GetBDBankRuleEditModel"
            });
        }
    },
    saveBankRule: function () {
        $("body").mFormSubmit({
            url: "/BD/BDBank/UpdateBDBankRule", param: { model: {} }, callback: function (msg) {
                var bankRuleId = $("#hidBankRuleID").val();
                var bankRuleName = $("#txtBankRuleName").val();
                var addMsg = HtmlLang.Write(LangModule.Bank, "BankRuleAdded", "New bank rule added") + ": " + bankRuleName;
                var updateMsg = HtmlLang.Write(LangModule.Bank, "BankRuleUpdated", "Bank rule updated") + ": " + bankRuleName
                var successMsg = bankRuleId.length > 0 ? updateMsg : addMsg;
                if (msg.Success) {

                    //刷新界面
                    if (parent.BDBankRule != undefined) {
                        parent.BDBankRule.reload();
                    }
                    $.mMsg(successMsg);

                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    BDBankRuleEdit.init();
});