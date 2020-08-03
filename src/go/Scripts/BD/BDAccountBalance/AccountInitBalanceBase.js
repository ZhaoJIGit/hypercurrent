AccountInitBalanceBase = {
    accountStandard: $("#hideAccountStandard").val(),
    init:function(){
        AccountInitBalanceBase.initAction();
    },
    initAction:function(){
        $("#btnTrialBalance").click(function () {
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Acct, "TrialBalance", "Trial balance check"),
                width: 500,
                height: 230,
                href: '/BD/BDAccount/TrialInitBalance'
            });
        });

        //结束初始化
        $("#btnFinish").click(function () {
            
            if (typeof(BalanceInput) =="Object") {
                BalanceInput.saveData(true);
            }

            var func = function () {
                //先判断试算是否平衡
                mAjax.post(
                    "/BD/BDAccount/CheckInitBalance", {}, function (msg) {
                        if (msg && msg.Success) {
                            //平衡时做其他操作
                            mAjax.submit(
                                "/BD/BDAccount/InitBalanceFinish",
                                "",
                                function (msg) {
                                    if (msg && msg.Success) {
                                        //平衡时做其他操作
                                        var message = HtmlLang.Write(LangModule.Acct, "FinishInitSuccess", "Finish Init Balance Successfully");
                                        $.mDialog.message(message);
                                        AccountInitBalanceBase.reload();
                                    } else {
                                        var message = msg && msg.Message ? msg.Message : HtmlLang.Write(LangModule.Acct, "FinishInitFail", "Finish Init Balance Fail,please try again!");

                                        $.mDialog.alert(message, function () {
                                            AccountInitBalanceBase.reload();
                                        });

                                    }
                                });
                        } else {
                            //显示试算平衡值
                            $("#btnTrialBalance").trigger("click");
                        }
                    }
                );
            }

            if (AccountInitBalanceBase.accountStandard == 3) {
                AccountInitBalanceBase.checkCustomAccountIsMatch(function () {
                    AccountInitBalanceBase.checkBalanceEqualWithBill(func);
                })
            } else {
                AccountInitBalanceBase.checkBalanceEqualWithBill(func);
            }
        });

        //重新初始化
        $("#btnReInit").click(function () {
            var confirmMsg = HtmlLang.Write(LangModule.Acct, "ReInitBalanceConfirm", "The re initialization of the balance will be the reverse audit all vouchers, and will affect the balance of accounts, you are sure to re - the initial balance?");
            $.mDialog.confirm(confirmMsg, function () {
                mAjax.submit(
                    "/BD/BDAccount/ReInitBalance",
                    "",
                    function (msg) {
                        if (msg && msg.Success) {
                            var message = HtmlLang.Write(LangModule.Acct, "ReInitBalanceSuccess", "Operation successfully ,You can re-enter the balance");
                            $.mDialog.message(message);
                            AccountInitBalanceBase.reload();
                            
                        } else {
                            var message = msg && msg.Message ? msg.Message : HtmlLang.Write(LangModule.Acct, "ReInitBalanceFail", "Operation Fail ,please try again!");
                            $.mDialog.alert(message);
                        }
                    });
            });
        });

        $("#aExport").click(function () {
            location.href = $("#hidGoServer").val() + '/BD/BDAccount/ExportOpeningBalances';
            $.mDialog.message(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });

        $("#btnClearInitBalance").click(function () {
            var tipMessage = HtmlLang.Write(LangModule.Acct, "SureToClearInitBalance", "所有科目的期初余额将会被清空，您确定需要这样做吗？")
            $.mDialog.confirm(tipMessage, function () {
                $.mAjax.submit("/GL/GLInitBalance/ClearInitBalance", {}, function (msg) {
                    if (msg && msg.Success) {
                        var message = HtmlLang.Write(LangModule.Acct, "InitBalanceClearSuccess", "已成功清空所有科目期初余额！");
                        $.mDialog.message(message);
                        AccountInitBalanceBase.reload();
                        
                    } else {
                        var message = "";
                        if (msg.Message) {
                            message = msg.Message;
                        } else {
                            message = HtmlLang.Write(LangModule.Acct, "InitBalanceClearFail", "科目期初余额清空失败！");
                        }
                        $.mDialog.alert(message);
                    }

                });
            });
        });
    },
    checkCustomAccountIsMatch: function (callback) {
        var url = "/BD/BDAccount/CheckCustomAccountIsMatch";
        $.mAjax.post(url, {}, function (data) {
            if (data && data.Success) {
                if (callback) {
                    callback();
                }
            } else {
                $.mDialog.alert(data.Message);
            }
        }, "", true);
    },
    checkBalanceEqualWithBill: function (callback) {
        //
        var url = "/BD/BDAccount/CheckBalanceEqualWithBill";
        //
        var sure2SynchronizeLang = HtmlLang.Write(LangModule.Acct, "NotFinishInit", "无法完成初始化，请核对后重新录入");
        //
        mAjax.post(url, {}, function (data) {
            //
            if (data) {
                if (!data.Success && data.Message) {
                    var errorMessage = "<div style='text-align:left;margin-left: 10px;'>";
                    var messages = data.Message.trimStart(';').trimEnd(';').split(';');
                    for (var i = 0; i < messages.length ; i++) {
                        errorMessage += (i + 1) + "." + messages[i] + ";<br>";
                    }
                    errorMessage += sure2SynchronizeLang + "</div>";
                    $.mDialog.alert(errorMessage);

                }
                else {
                    $.isFunction(callback) && callback();
                }
            }
        }, "", true);
    },
    reload: function () {
        if (typeof AccountBalances != "undefined") {
            AccountBalances.reload(true);
        } else if (typeof BalanceInput != "undefined") {
            BalanceInput.reload(true);
        }
    }
}


