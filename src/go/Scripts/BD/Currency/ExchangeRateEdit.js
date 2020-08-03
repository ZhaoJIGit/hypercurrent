var ExchangeRateEdit = {
    //初始化方法
    init: function () {
        ExchangeRateEdit.initExchangeRateData();
        //绑定事件
        ExchangeRateEdit.bindAction(0);
    },

    initExchangeRateData: function () {
        var mExchangeRate = parseFloat($("#hidMExchangeRate").val());
        $("#tbxUnCurrency").numberbox("setValue", mExchangeRate);
        $("#tbxTargetCurrency").numberbox("setValue", (1 / mExchangeRate).toFixed(6));
    },
    //绑定开始默认事件
    bindAction: function () {
        //添加常用币种事件
        $("#aSave").off("click").on("click", function () {
            //添加币种
            ExchangeRateEdit.edit();
        });

        $("#tbxTargetCurrency").bind("keyup", function (e) {
            if (e.keyCode == 9) {
                return;
            }
            var currency = $(this).val();
            currency = ExchangeRateEdit.currencySubstring(currency);
            if (ExchangeRateEdit.getDecimalVigits($(this).val()) > 6) {
                $(this).numberbox("setValue", currency);
            }
            if (currency && new Number(currency) > 0) {
                var unCurrency = (1 / parseFloat(currency)).toFixed(6);
                $("#tbxUnCurrency").numberbox("clear");
                $("#tbxUnCurrency").numberbox("setValue", unCurrency);

                $("#tbxUnCurrency").validatebox("isValid");
            }

        });

        $("#tbxUnCurrency").bind("keyup", function (e) {
            if (e.keyCode == 9) {
                return;
            }
            var currency = $(this).val();
            currency = ExchangeRateEdit.currencySubstring(currency);
            if (ExchangeRateEdit.getDecimalVigits($(this).val()) > 6) {
                $(this).numberbox("setValue", currency);

            }
            if (currency && new Number(currency) > 0) {
                currency = (1 / parseFloat(currency)).toFixed(6);
                $("#tbxTargetCurrency").numberbox("clear");
                $("#tbxTargetCurrency").numberbox("setValue", currency);

                $("#tbxTargetCurrency").validatebox("isValid");
            }
        });
    },
    //汇率修改
    edit: function () {

        var ops = {
            MOToLRate: parseFloat($("#tbxUnCurrency").numberbox("getValue")),
            MLToORate: parseFloat($("#tbxTargetCurrency").numberbox("getValue")),
            CurrencyID: $("#hidEditCurrency").val(),
            MLCurrencyID: $("#hidCurrency").val(),
            MCyItemID: '',
            MExchangeRateID: '',
        };

        //提示汇率必须大于零
        if (ops.MOToLRate <= 0 || ops.MLToORate <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "ExchangeMustMoreThanZero", "Exchange rate must greater than zero!"));
            return;
        }

        ops.CurrencyName = ops.CurrencyID;
        ops.IsLocalCy = ops.CurrencyID == ops.MLCurrencyID;

        $.mDialog.close(ops);
    },
    currencySubstring: function (value) {
        if (!value) {
            return value;
        }
        var result = value;
        var decimalVigits = value.split('.');
        //如果是数组
        if (decimalVigits instanceof Array && decimalVigits.length == 2) {
            var decimal = decimalVigits[1];

            if (decimal.length > 6) {
                decimal = decimal.substring(0, 6);
                var temp = decimalVigits[0] + "." + decimal;
                result = parseFloat(temp).toFixed(6);
            } else {
                result = value;
            }
        }

        return result;
    },
    //获取小数位数
    getDecimalVigits: function (value) {
        var result = 0;
        var decimalVigits = value.split('.');
        //如果是数组
        if (decimalVigits instanceof Array && decimalVigits.length == 2) {
            var decimal = decimalVigits[1];

            result = decimal.length;
        }

        return result;
    }

}
//部分视图初始化
$(document).ready(function () {
    ExchangeRateEdit.init();
});