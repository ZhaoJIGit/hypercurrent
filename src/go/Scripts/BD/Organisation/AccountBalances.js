var AccountBalances = {
    init: function () {
        AccountBalances.initAction();
    },
    initAction: function () {
        $("#aNext").click(function () {
            var toUrl = $(this).attr("href");
            AccountBalances.saveBalance(toUrl);
            return false;
        });
        $("#aSave").click(function () {
            var toUrl = $(this).attr("href");
            AccountBalances.saveBalance(toUrl);
            return false;
        });
        $("#aUpdate").click(function () {
            AccountBalances.saveBalance();
            return false;
        });
    },
    saveBalance: function (toUrl) {
        var orgId = $("#hidOrgCode").val();
        var arr = new Array();
        $("#balanceTable").find(".beginBalance").each(function () {
            var banlAcctId = $(this).attr("bankAcctID");
            var beginBalance = $(this).val();
            var bankCurrId = $(this).attr("bankCurrID");
            var obj = {};
            obj.MAcctID = banlAcctId;
            obj.MCyID = bankCurrId;
            obj.MBeginBalance = beginBalance;
            obj.MBeginBalanceFor = beginBalance;
            obj.MEndBalance = beginBalance;
            obj.MEndBalanceFor = beginBalance;
            arr.push(obj);
        });
        mAjax.post(
            "/Account/UpdateBankInitBalance", 
            { modelList: arr }, 
            function (data) {
                if (data.Success) {
                    if (toUrl == undefined) {
                        Megi.displaySuccess("#divMessage", HtmlLang.Write(LangModule.Org, "UpdateSuccessfully", "Update successfully."));
                    } else {
                        mWindow.reload(toUrl);
                    }
                } else {
                    $.mDialog.alert(data.Message);
                }
            });
    }
}
$(document).ready(function () {
    AccountBalances.init();
});