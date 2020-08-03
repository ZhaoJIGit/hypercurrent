//常用币种添加方法集 felson 2015.1.6
var alertLedger = HtmlLang.Write(LangModule.BD, "Addforeigncurrencysuccess", "您已成功添加外币，请至会计科目表处设置需要外币核算的科目!")

var ratesuccessfully = HtmlLang.Write(LangModule.BD, "addedexchangeratesuccessfully", "保存汇率成功")

var Addcurrency = null;
var CurrencyAdd = {
    //初始化方法
    init: function () {
        //初始化下拉框
        CurrencyAdd.initCombobox();
        //绑定事件
        CurrencyAdd.bindAction(0);
        if (!CurrencyList.hasChangeAuth) {
            $("#aAddCurrency").hide();
        }

    },
    initCombobox: function () {
        $("#selCurrency").combobox({
            url: "/Currency/GetBasCurrencyList?containFlag=false",
            textField: "MLocalName",
            valueField: "MCurrencyID",
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                var text = row[opts.textField] ? row[opts.textField].toLowerCase() : "";
                var value = row[opts.valueField] ? row[opts.valueField].toLowerCase() : "";
                q = q.toLowerCase();

                if (text.indexOf(q) >= 0 || value.indexOf(q) >= 0) {
                    return true;
                } else {
                    return false;
                }
            },
            onLoadSuccess: function () {
                //是否有编辑的币别
                var editCurrency = $("#hideEditCurrency").val();
                if (editCurrency) {
                    $("#selCurrency").combobox("setValue", editCurrency);
                    $("#selCurrency").combobox('disable');
                }

                CurrencyAdd.initExchangeRateData();
            },
            onBeforeLoad: function () {
                $('#selCurrency').combobox('disableValidation')
            },
            onShowPanel: function () {
                $("#selCurrency").combobox("enableValidation");
            }
        });
    },
    initExchangeRateData: function () {
        var id = $("#hideExchangeRateId").val();
        if (id) {
            //异步请求URL
            var url = "/BD/ExchangeRate/GetExchangeRateById";
            //参数
            var param = { id: id };
            //异步提交常用币种信息
            mAjax.post(url, param, function (msg) {
                if (msg) {
                    $("#tbxEffectiveData").datebox("setValue", msg.MRateDate);
                    $("#tbxTargetCurrency").numberbox("setValue", msg.MUserRate);

                    if (!msg.MRate) {
                        $("#tbxUnCurrency").numberbox("setValue", (1 / msg.MUserRate).toFixed(6))
                    } else {
                        $("#tbxUnCurrency").numberbox("setValue", msg.MRate);
                    }

                }
            });
        } else {
            $("#tbxEffectiveData").datebox("setValue", $("#hidDefaultDate").val());
        }
    },
    //绑定开始默认事件
    bindAction: function () {
        //添加常用币种事件
        $("#aSave").off("click").on("click", function () {
            //添加币种
            CurrencyAdd.addCurrency();
        });

        //加载默认选择的外币汇率
        $("#selCurrency").combobox({
            onSelect: function (record) {
                var baseCurrency = $("#hidCurrency").val();
                var targetCurrency = record.MCurrencyID;

                $("#lblTargetCurrencyName").text(targetCurrency);

                $("#lblUnCurrencyName").text("1 " + targetCurrency + " = ");
            }
        });

        $("#tbxTargetCurrency").bind("keyup", function (e) {
            if (e.keyCode == 9) {
                return;
            }
            var currency = $(this).val();
            currency = CurrencyAdd.currencySubstring(currency);
            if (CurrencyAdd.getDecimalVigits($(this).val()) > 6) {
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
            currency = CurrencyAdd.currencySubstring(currency);
            if (CurrencyAdd.getDecimalVigits($(this).val()) > 6) {
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
    //加载币种成功事件
    locadCurrencySuccess: function () {
        //初始化Combobox easyui-combobox，默认值选择为空
        $("#selCurrency").combobox("setValue", "");

    },
    //添加常用币种事件
    addCurrency: function () {
        $("#selCurrency").combobox("enableValidation");
        $('input.easyui-validatebox').validatebox('enableValidation');
        if (!$(".m-imain-content").mFormValidate()) {
            return false;
        }

        //选中的币种
        var selectedCurrencyID = $("#selCurrency").combobox("getValue");
        //判断是否为空
        if (!selectedCurrencyID) {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "noSelectCurreng", "Please select a currency!"));
            return false;
        }
        //生效时间
        var effDate = $("#tbxEffectiveData").datebox("getValue");

        var currentDate = $.mDate.formatter(new Date());

        if ($.mDate.compare(effDate, currentDate)) {
            //生效日期要小于当前日期
            var effDateErrorTips = HtmlLang.Write(LangModule.BD, "EffectDateLessCurrentDate", "生效日期要小于等于当前日期");
            $.mDialog.alert(effDateErrorTips);
            return;
        }

        //目标货币
        var rate = $("#tbxTargetCurrency").numberbox("getValue");
        var userRate = $("#tbxTargetCurrency").numberbox("getValue");
        if (!effDate || !rate) {
            return;
        }
        //提示汇率必须大于零
        if (rate <= 0 || userRate <= 0 || rate == "Infinity" || userRate == "Infinity") {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "ExchangeMustMoreThanZero", "Exchange rate must greater than zero!"));
            return;
        }



        //异步请求URL
        var url = "/BD/Currency/AddBDCurrency";
        //参数
        var model = {
            MCurrencyID: selectedCurrencyID,
            MItemID: $("#hidMItemID").val()
        }
        //异步提交常用币种信息
        mAjax.submit(url, { model: model }, CurrencyAdd.addCurrencySucces);
    },
    //添加常用币种成功后的回调函数
    addCurrencySucces: function (msg) {

        //先保存外币
        if (msg.Success) {
            Addcurrency = msg.Success;
            //获取返回的ID
            var id = msg.ObjectID;
            //再进行汇率的保存
            var url = "/BD/ExchangeRate/AddBDExchangeRate";
            //参数
            var effDate = $("#tbxEffectiveData").datebox("getValue");
            var rate = $("#tbxTargetCurrency").numberbox("getValue");



            var param = {
                MItemID: $("#hideExchangeRateId").val(),
                MRateDate: effDate,
                MUserRate: rate,
                MRate: $("#tbxUnCurrency").numberbox("getValue"),
                MTargetCurrencyID: $("#selCurrency").combobox("getValue"),
                MSourceCurrencyID: $("#hidCurrency").val(),
                RowIndex: $("#hideRowIndex").val()
            };

            //修改不进行判断
            if (param.MItemID) {
                CurrencyAdd.updateExchangeRate(url, param, "1");
            } else {
                //检测该汇率是否存在同一天的记录
                mAjax.post("/BD/ExchangeRate/CheckExchangeRateIsExist", { model: param }, function (msg) {
                    if (msg.Success) {
                        parent.$.mDialog.max();
                        //存在 确定要更新的提示
                        var content = HtmlLang.Write(LangModule.Acct, "ExchangeRatealreadyexistingatdate", "Exchange Rate already existing at {0},Are you sure to update");
                        content = content.replace("{0}", $.mDate.format(param.MRateDate));
                        $.mDialog.confirm(content, {
                            callback: function () {
                                //加上更新标志  更新的时候
                                url += "?isUpdate=true";
                                CurrencyAdd.updateExchangeRate(url, param, "2");
                            },
                            cancelCallback: function () {
                                parent.$.mDialog.min()
                            }
                        });
                    } else {
                        //添加货币 和  添加汇率的时候
                        CurrencyAdd.updateExchangeRate(url, param, "3");
                    }
                });
            }
        } else {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "addCurrencyFail", "Add currency fail：") + msg.Message);
        }
    },
    updateExchangeRate: function (url, param, Item) {
        //回调函数
        var callback = function (msg) {
            //提醒用户
            //常用币种表更新
            if (msg && msg.Success) {

                //操作成功!
                //$.mMsg(HtmlLang.Write(LangModule.Acct, "OperationSuccess", "Operation Success"));

                //总账开启 添加外币的提示
                var ledgerMarking = $("#hidGLBegin").val();
                //
                var Curreny = $("#hideEditCurrency").val();

                if (ledgerMarking == "1" && Addcurrency) {
                    if (Item == "1") {
                        $.mMsg(ratesuccessfully);
                    } else if (Item == "2") {
                        $.mMsg(ratesuccessfully);
                    } else {
                        //添加货币 和  添加汇率
                        if (Curreny == "") {
                            $.mMsg(alertLedger);
                        } else {
                            //汇率
                            $.mMsg(ratesuccessfully);
                        }
                    }
                }
                parent.CurrencyList && parent.CurrencyList.bindGrid && parent.CurrencyList.bindGrid(param.RowIndex);
                //关闭
                $.mDialog.close(msg.ObjectID);
            } else {
                if (msg && msg.Message) {
                    $.mDialog.alert(msg.Message);
                } else {
                    var tips = HtmlLang.Write(LangModule.BD, "OperationFail", "Operation Fail");
                    $.mDialog.alert(tips);
                }
            }

        };
        //异步提交
        mAjax.submit(url, { model: param }, callback);
        parent.$.mDialog.min();

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
    CurrencyAdd.init();
});