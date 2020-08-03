var AccountBalancesEdit = {
    currencyList: null,
    baseCurrency: $("#hideBaseCy").val(),
    init: function () {

        //如果是本位币，禁用原币输入框

        var currency = $("#mcyId").val();

        if (currency == AccountBalancesEdit.baseCurrency) {
            $("#tbxBalances").numberbox("disable");
        }

        AccountBalancesEdit.initCurrency();
        AccountBalancesEdit.initAction();
    },
    initAction: function () {
        $("#aCancel").click(function () {
            $.mDialog.close();
        });

        $("#aSave").click(function () {
            AccountBalancesEdit.saveBalance();
        });

        $("#tbxBalancesFor").keyup(function () {
            var value = $(this).val();
            var currency = $("#mcyId").val();
            var value2 = 0;
            if (currency == AccountBalancesEdit.baseCurrency) {
                value2 = value;
            } else {
                value2 = (value * AccountBalancesEdit.getExchange(currency)).toFixed(2)
            }
            $("#tbxBalances").numberbox("setValue", value2);
        });
    },
    initCurrency: function () {
        mAjax.post(
            "/BD/Currency/GetBDCurrencyList",
            { isIncludeBase: true },
            function (msg) {
                if (msg) {
                    //保存为全局变量
                    AccountBalancesEdit.currencyList = msg;
                }
            });
    },
    //根据币别获取汇率
    getExchange: function (currencyId) {
        for (var i = 0; i < AccountBalancesEdit.currencyList.length; i++) {
            var currency = AccountBalancesEdit.currencyList[i];
            if (currency.MCurrencyID == currencyId) {
                return currency.MRate;
            }
        }
    },

    saveBalance: function () {

        if (!$("#divEdit").mFormValidate()) {
            return;
        }

        var obj = {};
        obj.MAccountID = $("#accountId").val();
        obj.MCyID = $("#mcyId").val();
        obj.MID = $("#mId").val();
        var balances = $("#tbxBalancesFor").numberbox("getValue");
        obj.MBeginBalanceFor = balances;
        obj.MBeginBalance = $("#tbxBalances").numberbox("getValue"); // (balances / AccountBalancesEdit.getExchange(obj.MCurrencyID)).toFixed(2);

        //这种特殊情况，需要提示用户是否保存
        if (+obj.MBeginBalanceFor == 0 && +obj.MBeginBalance > 0) {
            var tips = HtmlLang.Write(LangModule.BD, "NotOriginalCurrency", "检测到没有填写原币金额，是否继续保存！");
            $.mDialog.confirm(tips, function () {
                AccountBalancesEdit.saveBankInitBalance(obj);
            });
        } else {
            AccountBalancesEdit.saveBankInitBalance(obj)
        }
    },
    saveBankInitBalance: function (bankBalanceObject) {
        mAjax.submit(
          "/BD/BDBank/UpdateBankInitBalance",
          { model: bankBalanceObject },
          function (data) {
              if (data.Success) {
                  $.mDialog.message(HtmlLang.Write(LangModule.Org, "UpdateSuccessfully", "Update successfully."));
                  parent.AccountBalances.getAccountListByPage();
                  $.mDialog.close();
              } else {
                  $.mDialog.max();
                  $.mDialog.alert(data.Message, function () {
                      $.mDialog.min();
                  });
              }
          });
    }

}
$(document).ready(function () {
    AccountBalancesEdit.init();
});