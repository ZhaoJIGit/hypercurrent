var AccountBalancesEdit = {
    init: function () {
        AccountBalancesEdit.initAction();
    },
    initAction:function(){
        $("#aCancel").click(function () {
            $.mDialog.close();
        });

        $("#aSave").click(function () {
            AccountBalancesEdit.saveBalance();
        });
    },
    saveBalance: function () {
       
        if (!$("#divEdit").mFormValidate()) {
            return;
        }
        var arr = new Array();
        var obj = {};
        obj.MAcctID = $("#accountId").val();
        obj.MCyID = $("#mcyId").val();

        var balances = $("#balances").val();
        obj.MBeginBalance = balances;
        obj.MBeginBalanceFor = balances;
        obj.MEndBalance = balances;
        obj.MEndBalanceFor = balances;
        arr.push(obj);


        Megi.Form.post({
            url: "/BD/Account/UpdateBankInitBalance", param: { modelList: arr }, callback: function (data) {
                if (data.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.Org, "UpdateSuccessfully", "Update successfully."));
                    parent.AccountBalances.getAccountListByPage();
                    $.mDialog.close();
                } else {
                    $.mAlert(data.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    AccountBalancesEdit.init();
});