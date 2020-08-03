/*
转账主界面
*/
(function () {
    var IVTransferHome = (function () {
        //常量定义
        //顶端
        var _transferTop = ".transfer-top";
        //主div
        var _mImainContent = ".m-imain-content";
        //
        var transferContent = ".m-transfer-content";
        //工具栏
        var _mToolbar = ".m-toolbar";
        //
        var fromAccount, toAccount;
        //
        var acctName = "";
        //
        var acctListData = [];
        //黄色按钮央视
        var yellowAnchor = "easyui-linkbutton-yellow";
        //取消
        var _cancelTransfer = ".cancel-transfer";
        //本位币
        var baseCyID = top.MegiGlobal.BaseCurrencyID;
        //
        var balanceAfterTr = "tr.balance-after-tr";
        //确认
        var _submitTransfer = ".submit-transfer";
        //转出账户，和目标账户
        var from = 0, to = 1;
        //
        var _fromAccount = ".from-account";
        //
        var _transferIcoRate = ".transfer-ico-rate";
        //转出账户选择
        var _fromAccountSelect = ".from-account-select";
        //转入账户选择
        var _toAccountSelect = ".to-account-select";
        //当前余额
        var _currentBalance = ".current-balance";
        //
        var emptyTd = "td.empty-td";
        //
        var _exchangeLossInput = ".exchange-loss-input";
        //
        var diffCurrencySpan = '.diff-currency-span';
        //
        var exchangeLossDiv = ".exchange-loss-div";
        //左边的账户table
        var _fromAccountInfoTable = ".from-account-info-table";
        //右边的账户table
        var _toAccountInfoTable = ".to-account-info-table";
        //日期选择
        var _tranferDateSelect = ".tranfer-date-select";
        //数值输入
        var _fromAmountForInput = ".from-amount-for-input";
        //
        var _fromAmountInput = ".from-amount-input";
        //
        var _toAmountInput = ".to-amount-input";
        //数值输入
        var _toAmountForInput = ".to-amount-for-input";
        //reference
        var _referenceInput = ".reference-input";
        //汇率输入
        var _beginExchangeRateInput = ".begin-exchangerate-input";
        //汇率输入
        var _currentExchangeRateInput = ".current-exchangerate-input";
        //高级选项
        var _advanceOptionDiv = ".m-iv-adv";
        //转出账户币别
        var _fromAccountCurrency = ".from-account-currency";
        //转入账户币别
        var _toAccountCurrency = ".to-account-currency";
        //参考汇率div
        var _referenceRateDiv = ".reference-rate-div";
        //当前汇率
        var _rate = 0;
        //
        var valueField = "MItemID";
        //
        var textField = "MBankName";
        //转账后余额
        var _balanceAfterTransfer = ".balance-after-transfer";
        //初始化form的url
        var getFormUrl = "/IV/IVTransfer/GetTransferEditModel";
        //获取银行账户信息
        var getAccountUrl = "/BD/BDBank/GetAllBDBankAccountInfo";
        //汇率显示精度
        var precision = 6;
        //获取相对汇率的url
        var getCurrencyRateUrl = "/BD/ExchangeRate/GetBDExchangeRate";
        //提交
        var submitTransferUrl = "/IV/IVTransfer/SubmitTransfer";
        //保存的ID
        var MItemID = null;
        //左边用户的余额数量
        var leftMoney = 0;
        //右边用户的余额数量
        var rightMoney = 0;

        var IVTransferHome = function (mid, acctId, dialog) {
            var that = this;
            //初始化combobox
            this.InitAccountCombobox = function () {
                //
                mAjax.post(getAccountUrl, "", that.InitAccountComboboxByData, "", false, false);
            };
            //初始化日期选择控件
            this.initDateCombobox = function () {
                //
                $(_tranferDateSelect).datebox({
                    required: true,
                    validType: "minDate ['" + $("#beginDate").val() + "']",
                    minDate: $("#beginDate").val(),
                    onSelect: function (date) {
                        //
                        if (fromAccount.MCyID && toAccount.MCyID) {
                            //
                            that.getCurrencyRate(fromAccount.MCyID, toAccount.MCyID, date);
                        }
                    }
                });
            };
            //根据获取的数据初始化combobox
            this.InitAccountComboboxByData = function (data) {
                //
                acctListData = data;
                //转出
                $(_fromAccountSelect).mAddCombobox(
                    "bankaccount",
                    {
                        data: data,
                        valueField: valueField,
                        textField: textField,
                        width: 200,
                        onSelect: function (row) {
                            //转出科目=0     _fromAccountSelect:转出账户的选择
                            that.changeAccount(from, row);
                        }
                    },
                    {
                        hasPermission: $(_submitTransfer).is(":visible"),
                        callback: function () {
                            //先clear
                            $(_fromAccountSelect).combobox("clear");
                            //
                            $(_toAccountSelect).combobox("clear");
                            //初始化
                            that.InitAccountCombobox();
                            //
                            $.mTab.setStable();
                        }
                    });
                //转入账户
                $(_toAccountSelect).mAddCombobox(
                    "bankaccount",
                    {
                        data: data,
                        valueField: valueField,
                        textField: textField,
                        width: 200,
                        onSelect: function (row) {
                            that.changeAccount(to, row)
                        }
                    },
                    {
                        hasPermission: $(_submitTransfer).is(":visible"),
                        callback: function () {
                            //先clear
                            $(_fromAccountSelect).combobox("clear");
                            //
                            $(_toAccountSelect).combobox("clear");
                            //初始化
                            that.InitAccountCombobox();
                            //
                            $.mTab.setStable();
                        }
                    }
                );
                //初始化转出账户的值
                if (acctId) {
                    //IE下面，上面的不方法不管用 
                    $(_fromAccountSelect).combobox("select", acctId);
                }
            };

            //获取一张银行转账单的信息
            this.getTransferData = function () {
                //如果有mid
                $(balanceAfterTr).hide();
                //
                mAjax.post(getFormUrl, { MID: mid }, function (data) {
                    //
                    if (data) {
                        //显示出来
                        that.showTransferData(data);
                    }
                }, "", true);
            };
            //把一张银行转账单的信息显示出来
            this.showTransferData = function (data) {
                //转账日期
                $(_tranferDateSelect).datebox("setValue", data.MBizDate);
                //用途说明
                $(_referenceInput).val(data.MReference);
                //转出账户,选择完账户之后，就不用管了
                $(_fromAccountSelect).combobox("setValue", data.MFromAcctID);
                //转入账户
                $(_toAccountSelect).combobox("setValue", data.MToAcctID);

                var f = $(_fromAccountSelect).combobox("options").data.where("x.MItemID =='" + data.MFromAcctID + "'")[0];

                var t = $(_toAccountSelect).combobox("options").data.where("x.MItemID =='" + data.MToAcctID + "'")[0];
                //
                f && that.changeAccount(from, f);
                //
                t && that.changeAccount(to, t);

                //默认 的银行账号就是转出账户
                acctId = data.MFromAcctID;
                //
                acctName = acctListData.where("x." + valueField + " == '" + acctId + "'")[0][textField];
                //
                if (data.MFromCyID != data.MToCyID) {
                    if (data.MBeginExchangeRate) {
                        //显示期初汇率
                        $(_beginExchangeRateInput).numberbox("setValue", data.MBeginExchangeRate);
                    }
                    //如果有当前汇率
                    if (data.MExchangeRate) {
                        //显示汇率
                        $(_currentExchangeRateInput).numberbox("setValue", data.MExchangeRate);
                    };
                } else {
                    $(_beginExchangeRateInput).parent().parent().hide();
                    $(_currentExchangeRateInput).parent().parent().hide();
                }

                //转出原币金额
                $(_fromAmountForInput).numberbox("setValue", data.MFromTotalAmtFor);
                //转出本位币金额
                $(_fromAmountInput).numberbox("setValue", data.MFromTotalAmt);
                //转入原币金额
                $(_toAmountForInput).numberbox("setValue", data.MToTotalAmtFor);
                //转入本位币金额
                $(_toAmountInput).numberbox("setValue", data.MToTotalAmt);
                //计算汇兑损益
                that.calculateExchangeLoss();

                //设置为稳定状态
                $.mTab.setStable();
            };

            //检查相同账户之间转账和都是外币之间的转账
            this.checkAccountValid = function (src) {
                //from account = 
                fromAccount = $(_fromAccountSelect).mGetComboboxObject(valueField);
                //
                toAccount = $(_toAccountSelect).mGetComboboxObject(valueField);
                //
                //不能在相同账户之间转账
                if (fromAccount[valueField] && fromAccount[valueField] == toAccount[valueField]) {
                    //弹出提醒
                    var title = HtmlLang.Write(LangModule.IV, "CannotTranferBetweenAAccount", "You can't Tranfer Between the same account");
                    //
                    $.mDialog.error(title);
                    //
                    $(src).combobox("setValue", "");
                    //返回
                    return false;
                }
                //不能在相同账户之间转账
                if (fromAccount["MCyID"] && toAccount["MCyID"] && fromAccount["MCyID"] != baseCyID && toAccount["MCyID"] != baseCyID) {
                    //弹出提醒
                    var title = HtmlLang.Write(LangModule.IV, "CannotTranferBetweenTwoForeignAccount", "不能在两个外币账户间进行转账");
                    //
                    $.mDialog.error(title);
                    //
                    $(src).combobox("setValue", "");
                    //返回
                    return false;
                }
                return true;
            };

            //处理左右表格中没有显示的行
            this.handleTableEmtpyRow = function () {
                //转出原币金额是否显示
                if ($(_fromAmountForInput).parent().parent().find(emptyTd).is(":visible")
                    && $(_toAmountForInput).parent().parent().find(emptyTd).is(":visible")) {
                    $(_fromAmountForInput).parent().parent().find(emptyTd).hide();
                    $(_toAmountForInput).parent().parent().find(emptyTd).hide();
                }
                //汇率
                if ($(_beginExchangeRateInput).parent().parent().find(emptyTd).is(":visible")
                    && $(_currentExchangeRateInput).parent().parent().find(emptyTd).is(":visible")) {
                    $(_beginExchangeRateInput).parent().parent().find(emptyTd).hide();
                    $(_currentExchangeRateInput).parent().parent().find(emptyTd).hide();
                };
            };
            //切换账号的时候，需要清空下面的金额
            this.clearAccountInput = function (accountType) {
                //
                if (accountType == from) {
                    //
                    $(_fromAmountForInput).numberbox("setValue", 0);
                    //
                    $(_fromAmountInput).numberbox("setValue", 0);
                    //
                    $(_beginExchangeRateInput).numberbox("setValue", 0);
                }
                else {
                    //
                    $(_toAmountForInput).numberbox("setValue", 0);
                    //
                    $(_toAmountInput).numberbox("setValue", 0);
                    //
                    $(_exchangeLossInput).numberbox("setValue", 0);
                }
            };

            //选择转出账户，如果是本位币转出的话，那期初汇率和转出本位币金额就不需要显示，反之则需要显示
            this.changeAccount = function (accountType, row) {
                //
                var src = accountType == from ? _fromAccountSelect : _toAccountSelect;
                //
                if (that.checkAccountValid(src)) {
                    //
                    that.clearAccountInput(accountType);
                    //转出账户是外币 则显示
                    var showFromAmountFor = fromAccount.MCyID != baseCyID;
                    //转出账户是外币 则显示
                    var showFromExchangeRate = fromAccount.MCyID != baseCyID;
                    //不然如何都显示
                    var showFromAmount = true;
                    //转出账户是本位币，但是转入账户是外币  
                    var showToAmountFor = fromAccount.MCyID == baseCyID && toAccount.MCyID != baseCyID;
                    //转出账户不是本位币
                    var showToExchangeRate = fromAccount.MCyID != baseCyID || toAccount.MCyID != baseCyID;
                    //
                    $(emptyTd).hide();
                    //
                    var showToAmount = true;
                    //
                    if (showFromAmountFor) {
                        //
                        $(_fromAmountForInput).parent().parent().find("td").not(emptyTd).show();
                        //
                        $(_fromAmountForInput).validatebox("enableValidation");
                    }
                    else {
                        //
                        $(_fromAmountForInput).parent().parent().find("td").hide();
                        //
                        $(_fromAmountForInput).parent().parent().find(emptyTd).show();
                        //
                        $(_fromAmountForInput).validatebox("disableValidation");
                    }
                    //控制期初汇率的显示
                    if (showFromExchangeRate) {
                        //
                        $(_beginExchangeRateInput).parent().parent().find("td").not(emptyTd).parent().show();
                        $(_beginExchangeRateInput).parent().parent().find("td").not(emptyTd).show();
                        //
                        $(_beginExchangeRateInput).validatebox("enableValidation");
                    }
                    else {
                        //
                        $(_beginExchangeRateInput).parent().parent().find("td").hide();
                        //
                        $(_beginExchangeRateInput).parent().parent().find(emptyTd).show();
                        //
                        $(_beginExchangeRateInput).validatebox("disableValidation");
                    }
                    //
                    if (showFromAmount) {
                        //
                        $(_fromAmountInput).parent().parent().find("td").not(emptyTd).show();
                        //
                        $(_fromAmountInput).validatebox("enableValidation");
                    }
                    else {
                        //
                        $(_fromAmountInput).parent().parent().find("td").hide();
                        //
                        $(_fromAmountInput).parent().parent().find(emptyTd).show();
                        //
                        $(_fromAmountInput).validatebox("disableValidation");
                    }
                    //
                    if (showToAmountFor) {
                        //
                        $(_toAmountForInput).parent().parent().find("td").not(emptyTd).show();
                        //
                        $(_toAmountForInput).validatebox("enableValidation");
                    }
                    else {
                        //
                        $(_toAmountForInput).parent().parent().find("td").hide();
                        //
                        $(_toAmountForInput).parent().parent().find(emptyTd).show();
                        //
                        $(_toAmountForInput).validatebox("disableValidation");
                    }
                    //
                    if (showToExchangeRate) {
                        $(_currentExchangeRateInput).parent().parent().find("td").not(emptyTd).parent().show();
                        //
                        $(_currentExchangeRateInput).parent().parent().find("td").not(emptyTd).show();
                        //
                        $(_currentExchangeRateInput).validatebox("enableValidation");
                    }
                    else {
                        //
                        $(_currentExchangeRateInput).parent().parent().find("td").hide();
                        //
                        $(_currentExchangeRateInput).parent().parent().find(emptyTd).show();
                        //
                        $(_currentExchangeRateInput).validatebox("disableValidation");
                    }
                    //
                    if (showToAmount) {
                        //
                        $(_toAmountInput).parent().parent().find("td").not(emptyTd).show();
                        //
                        $(_toAmountInput).validatebox("enableValidation");
                    }
                    else {
                        //
                        $(_toAmountInput).parent().parent().find("td").hide();
                        //
                        $(_toAmountInput).parent().parent().find(emptyTd).show();
                        //
                        $(_toAmountInput).validatebox("disableValidation");
                    }
                    //如果从外币账户转出的，那本位币金额是不可以输入的
                    if (fromAccount.MCyID != baseCyID) {
                        //
                        $(_fromAmountInput).attr("readonly", "readonly");
                    }
                    else {
                        //
                        $(_fromAmountInput).removeAttr("readonly");
                        //如果本位币账户转给本位币账户
                        if (toAccount.MCyID == baseCyID) {
                            //
                            if (accountType == to) {
                                //同步金额
                                $(_toAmountInput).numberbox("setValue", $(_fromAmountInput).numberbox("getValue"));
                                //计算汇兑损益
                                that.calculateExchangeLoss();
                            }
                        }
                    }
                    //
                    that.toAmountForInputKeyUp({});
                    //
                    that.fromAmountForInputKeyUp({});
                    //
                    var selector = accountType == from ? _fromAccountInfoTable : _toAccountInfoTable;


                    //显示当前余额
                    that.showBalance($(_currentBalance, selector), row.MMegiBalance, row.MCyID)
                    //显示转账后余额
                    that.showBalance($(_balanceAfterTransfer, selector), row.MMegiBalance, row.MCyID);

                    //如果是转出账户并且不是本位币账户，则需要显示期初
                    if (fromAccount.MCyID && toAccount.MCyID && fromAccount.MCyID != baseCyID && showFromExchangeRate) {
                        //显示期初汇率
                        that.getCurrencyRate(fromAccount.MCyID, toAccount.MCyID);
                    }
                    //如果是转出账户，并且是外币账户，则需要显示 汇率
                    if (accountType == to && toAccount.MCyID && toAccount.MCyID != baseCyID && showToExchangeRate) {
                        //显示一个单元
                        $(_fromAccountCurrency, _toAccountInfoTable).text("1" + toAccount.MCyID + "=");
                        //显示转入账户币种
                        $(_toAccountCurrency, _toAccountInfoTable).text(baseCyID);
                    }
                    //
                    that.handleTableEmtpyRow();
                }
            };
            //获取combobox的某一项的data数据
            //显示汇率
            this.getCurrencyRate = function (fromCurrency, toCurrency, date) {
                //异步获取汇率
                mAjax.post(
                    getCurrencyRateUrl,
                    {
                        filter: {
                            MSourceCurrencyID: fromCurrency,
                            MTargetCurrencyID: toCurrency,
                            MRateDate: date || $(_tranferDateSelect).combobox("getValue")
                        }
                    },
                    function (data) {
                        //显示汇率
                        that.showCurrencyRate(data, fromCurrency, toCurrency);
                    }, false, false, false);
            };
            //显示汇率以及差额
            this.showCurrencyRate = function (data, fromCurrency, toCurrency) {
                //
                _rate = data;
                //显示一个单元
                $(_fromAccountCurrency).text("1" + fromCurrency + "=");
                //显示汇率
                $(_beginExchangeRateInput).numberbox("setValue", (_rate == 0 ? "" : _rate));
                //显示转入账户币种
                $(_toAccountCurrency).text(toCurrency);
            };
            //清空转出账户的日期，数量，以及reference
            this.clearFromAccount = function () {
                //清空日期
                $(_tranferDateSelect).datebox("setValue", mDate.format(mDate.DateNow));
                //清空amount
                $(_fromAmountForInput).numberbox("setValue", "");
            };

            this.refreshListUrl = function (ready, normalCallback, popCallback) {
                BDBank.refreshTransPage(acctId, acctName, ready, normalCallback, popCallback);
            },

            //初始化点击事件
            this.initEvent = function () {
                //确认功能
                $(_submitTransfer).off("click").on("click", that.submitTransfer);
                //转账输入输入框响应事件 汇率输入框里面的输入响应事件
                $(_fromAmountForInput + "," + _fromAmountInput + "," + _beginExchangeRateInput).off("keyup", that.fromAmountForInputKeyUp).on("keyup", that.fromAmountForInputKeyUp);
                //转账输入输入框响应事件 汇率输入框里面的输入响应事件
                $(_toAmountForInput + "," + _currentExchangeRateInput).off("keyup", that.toAmountForInputKeyUp).on("keyup", that.toAmountForInputKeyUp);
                //查看银行勾对
                $("#aShowBankReconcileView").off("click").on("click", function () {
                    var billType = $("#hidBillType").val();
                    var billId = $("#hidBillId").val();
                    $.mDialog.show({
                        mTitle: HtmlLang.Write(LangModule.Bank, "BankReconcileView", "BankReconcileView"),
                        mWidth: "900",
                        mHeigth: "450",
                        mDrag: "mBoxTitle",
                        mShowbg: true,
                        mContent: "iframe:" + "/BD/BDBank/BDBankReconcileView?billType=" + billType + "&billId=" + billId,
                        mCloseCallback: [function () {

                        }]
                    });
                });
                //删除银行勾对记录
                $("#aDeleteBankReconcile").off("click").on("click", function () {
                    var billType = $("#hidBillType").val();
                    var billId = $("#hidBillId").val();
                    $.mDialog.confirm(LangKey.AreYouSureToDelete, {
                        callback: function () {
                            mAjax.submit(
                                "/BD/BDBank/DeleteBankReconcile?billType=" + billType + "&billId=" + billId,
                                {},
                                function (msg) {
                                    if (msg.Success) {
                                        var message = HtmlLang.Write(LangModule.Bank, "DeleteSuccessfully", "Delete Successfully!");
                                        $.mMsg(message);
                                        //
                                        that.refreshListUrl(true);
                                        //刷新页面
                                        mWindow.reload();
                                    } else {
                                        $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Deletefailed"));
                                        that.refreshListUrl(true);
                                    }
                                });
                        }
                    });
                });
                $(".markstatus").off("click").on("click", function () {
                    var billId = $("#hidBillId").val();
                    var statu = $(this).attr("status");
                    that.UpdateReconcileStatu(billId, statu);
                });
            };
            this.initUI = function () {
                if (window.screen && window.screen.width <= 1024) {
                    $(_beginExchangeRateInput).width(80);
                    $(_advanceOptionDiv).width(170);
                    $(".transfer-top .from-account,.transfer-top .to-account,.transfer-top .transfer-ico-rate").width(290);
                    $(".transfer-top .transfer-rate").css({ "margin-left": 50 });
                }
            };

            this.UpdateReconcileStatu = function (transferId, statu) {
                mAjax.submit(
                    "/IV/IVTransfer/UpdateReconcileStatu?transferId=" + transferId + "&statu=" + statu,
                    {},
                    function (msg) {
                        if (msg.Success) {
                            if (statu == "204") {
                                var message = HtmlLang.Write(LangModule.Bank, "MarkAsReconciledSuccess", "Mark as reconciled successful!");
                            } else {
                                var message = HtmlLang.Write(LangModule.Bank, "MarkAsUnReconciledSuccess", "Mark as unreconciled successful!");
                            }
                            $.mDialog.message(message);
                            //
                            that.refreshListUrl(true);
                            //刷新页面
                            mWindow.reload();
                        } else {
                            $.mDialog.alert(msg.Message, function () {
                                mWindow.reload();
                            });
                        }
                    });
            };
            //
            this.fromAmountForInputKeyUp = function (event) {
                //需要选择了输出账户才有效
                if (fromAccount && fromAccount[valueField]) {
                    //获取当前汇率
                    _rate = 1;
                    //
                    var value = 0;
                    //对于本位币转外币 或者本位币转本位币
                    if (fromAccount.MCyID == baseCyID) {
                        //
                        value = $(_fromAmountInput).val();
                        //如果转入账户也是本位币，则需要同步金额
                        if (toAccount.MCyID == baseCyID) {
                            //
                            $(_toAmountInput).numberbox("setValue", value);
                            //
                            that.toAmountForInputKeyUp({});
                        }
                        //下面的金额是不需要任何计算的，只需要计算汇兑损益
                        that.calculateExchangeLoss();
                    }
                    else {
                        //
                        _rate = $(_beginExchangeRateInput).val();
                        //对于外币转本位币
                        if ((event.srcElement || event.target) == $(_fromAmountForInput)[0]) {
                            //转出本位币金额 = 汇率 * 转出原币金额
                            $(_fromAmountInput).numberbox("setValue", $(_fromAmountForInput).val() * _rate);
                            //如果外币有汇率
                            var currentExchangeRate = $(_currentExchangeRateInput).numberbox("getValue");
                            //
                            $(_toAmountInput).numberbox("setValue", currentExchangeRate * $(_fromAmountForInput).val());
                            //计算汇兑损益
                            that.calculateExchangeLoss();
                        }
                        else if ((event.srcElement || event.target) == $(_beginExchangeRateInput)[0]) {
                            //转出本位币金额 = 汇率 * 转出原币金额
                            $(_fromAmountInput).numberbox("setValue", $(_fromAmountForInput).val() * _rate);
                            //如果外币有汇率
                            var currentExchangeRate = $(_currentExchangeRateInput).numberbox("getValue");
                            //
                            $(_toAmountInput).numberbox("setValue", currentExchangeRate * $(_fromAmountForInput).val());
                            //计算汇兑损益
                            that.calculateExchangeLoss();
                        }
                        else {
                            //转出原币金额 = 转出原币金额 / 汇率
                            $(_fromAmountForInput).numberbox("setValue", $(_fromAmountInput).val() / _rate);
                            //计算汇兑损益
                            that.calculateExchangeLoss();
                        }
                        //
                        value = $(_fromAmountForInput).val();
                    }
                    //转出账户
                    var fromCurrent = $(_currentBalance, _fromAccountInfoTable);
                    //
                    var fromAfter = $(_balanceAfterTransfer, _fromAccountInfoTable);
                    //转出账户当前余额减少
                    var balance = fromCurrent.data("balance");
                    //
                    that.showBalance(fromCurrent, balance, fromCurrent.data("currencyID"));
                    //
                    balance = balance - value;
                    //
                    that.showBalance(fromAfter, balance, fromCurrent.data("currencyID"));
                }
            };
            //
            this.toAmountForInputKeyUp = function (event) {
                //需要选择了输出账户才有效
                if (toAccount && toAccount[valueField]) {
                    //获取当前汇率
                    _rate = 1;
                    //
                    var value = 0;
                    //对于本位币转外币 或者本位币转本位币
                    if (toAccount.MCyID == baseCyID && fromAccount.MCyID == baseCyID) {
                        //
                        value = +($(_toAmountInput).val());
                        //下面的金额是不需要任何计算的，只需要计算汇兑损益
                        that.calculateExchangeLoss();
                    }
                    else if (toAccount.MCyID != baseCyID && fromAccount.MCyID == baseCyID) {
                        //针对于本位币转外币的情况，就是一个购汇了 原币 汇率 和 转入本位币金额之间没有一个必然的关系
                        //转入本位币金额 = 使用当前汇率 * 转入原币金额
                        _rate = +($(_currentExchangeRateInput).val());
                        //转入原币金额
                        var toAmountForValue = +($(_toAmountForInput).val());
                        //
                        value = toAmountForValue;
                        //转入本位币金额
                        var toAmountValue = toAmountForValue * _rate;
                        //
                        $(_toAmountInput).numberbox("setValue", toAmountValue);
                        //计算汇兑损益
                        that.calculateExchangeLoss();
                    }
                    else if (toAccount.MCyID == baseCyID && fromAccount.MCyID != baseCyID) {
                        //针对于，外币转本位币的情况
                        //获取转出账户的原币金额
                        var fromAmountFor = +($(_fromAmountForInput).val());
                        //获取当前汇率
                        _rate = +($(_currentExchangeRateInput).val());
                        //计算本位币金额
                        var toAmountValue = fromAmountFor * _rate;
                        //
                        value = toAmountValue;
                        //
                        $(_toAmountInput).numberbox("setValue", toAmountValue);
                        //计算汇兑损益
                        that.calculateExchangeLoss();
                    }
                    //转出账户
                    var toCurrent = $(_currentBalance, _toAccountInfoTable);
                    //
                    var toAfter = $(_balanceAfterTransfer, _toAccountInfoTable);
                    //转出账户当前余额减少
                    var balance = toCurrent.data("balance");
                    //
                    //为undifind
                    if (balance) {
                        that.showBalance(toCurrent, balance, toCurrent.data("currencyID"));
                    }

                    //
                    balance = balance + value;
                    //
                    //chenpan , 这里余额为0的时候，没有进去，导致最后一次keyup没有根据转账后余额
                    if (balance != undefined) {
                        that.showBalance(toAfter, balance, toCurrent.data("currencyID"));
                    }
                }
            };
            //计算汇兑损益
            this.calculateExchangeLoss = function () {
                //
                var fromAmount = +($(_fromAmountInput).val());
                //
                var toAmount = +($(_toAmountInput).val());
                //
                var diff = fromAmount - toAmount;
                //
                $(_exchangeLossInput).numberbox("setValue", diff);
                //
                $(diffCurrencySpan).text("[" + baseCyID + "]");

                diff = diff < 0.01 && diff > -0.01 ? 0 : diff;
                //
                if (diff != 0) {
                    //显示成红色的
                    $("span,div,input", exchangeLossDiv).css({
                        color: "red"
                    });
                }
                else {
                    //显示成绿色的
                    $("span,div,input", exchangeLossDiv).css({
                        color: "green"
                    });
                }
            };

            //确认提交转账
            this.submitTransfer = function () {
                //先校验是否通过
                if (!$("body").mFormValidate()) {
                    return false;
                }
                //转账的金额必须大于目前的余额
                var afterBalance = $(_balanceAfterTransfer, _fromAccountInfoTable).data("balance");
                //
                var currentBalance = $(_balanceAfterTransfer, _fromAccountInfoTable).data("balance");
                //
                if (currentBalance < afterBalance && fasle) {
                    //弹出确定取消的提示框
                    var title = HtmlLang.Write(LangModule.Bank, "Turnouttheamountisgreaterthanthecurrentbalance", "转出的总额大于目前余额")
                    //谭勇
                    $.mDialog.confirm(title, function (e) {
                        //如果用户点击确定则直接进行转账
                        that.transfer();
                    });
                }
                else {
                    that.transfer();
                }
            };
            //提交转账
            this.transfer = function () {
                //转账日期
                var bizDate = $(_tranferDateSelect).datebox("getValue");
                //使用描述
                var reference = $(_referenceInput).val();
                //获取当前转账的金额
                var fromAmount = $(_fromAmountInput).numberbox("getValue");
                //获取当前汇率
                var beginExchangeRate = $(_beginExchangeRateInput).numberbox("getValue");
                //获取转出原币金额
                var fromAmountFor = $(_fromAmountForInput).numberbox("getValue");
                //获取转入金额
                var toAmount = $(_toAmountInput).numberbox("getValue");
                //获取转入外币金额
                var toAmountFor = $(_toAmountForInput).numberbox("getValue");
                //获取当前汇率
                var currentExchangeRate = $(_currentExchangeRateInput).numberbox("getValue");
                //转出账户
                var fromAccountId = $(_fromAccountSelect).combobox("getValue");
                //转入账户
                var toAccountId = $(_toAccountSelect).combobox("getValue");
                //转出币别
                var fromCyID = fromAccount.MCyID;
                //抓入币别
                var toCyID = toAccount.MCyID;
                //获取转账ID 没有则为空
                var BillId = $("#hidBillId").val();
                //提交参数
                mAjax.submit(
                    submitTransferUrl, {
                        model: {
                            MID: BillId,
                            MFromAcctID: fromAccountId,
                            MToAcctID: toAccountId,
                            MBizDate: bizDate,
                            MFromCyID: fromCyID,
                            MToCyID: toCyID,
                            MExchangeRate: currentExchangeRate,
                            MBeginExchangeRate: beginExchangeRate,
                            MFromTotalAmtFor: fromAmountFor,
                            MFromTotalAmt: fromAmount,
                            MToTotalAmt: toAmount,
                            MToTotalAmtFor: toAmountFor,
                            MReference: reference
                        }
                    }, function (data) {
                        //成功显示
                        if (data.Success) {
                            //
                            var success = HtmlLang.Write(LangModule.IV, "TransferSuccessed", "Transfer Successed");
                            if (BillId) {
                                success = HtmlLang.Write(LangModule.Common, "SaveSuccessfully", "Save Successed");
                            }
                            //显示成功
                            mDialog.message(success);
                            //如果是dialog过来的
                            if (dialog == 1) {
                                //关闭dialog
                                $.mDialog.close(0, data.Data);
                            }
                            else {
                                //标题
                                var title = HtmlLang.Write(LangModule.Bank, "TransferBusiness", "Transfer");
                                //弹窗
                                mWindow.reload("/IV/IVTransfer/IVTransferHome?MID=" + data.ObjectID);
                            }
                            $.mTab.setStable();
                        }
                        else {
                            $.mDialog.alert(data.Message);
                        }
                    });
            };
            //显示某种余额数量
            this.showBalance = function (selector, balance, currencyID) {
                //显示币种以及余额
                $(selector).val(mMath.toMoneyFormat(balance) + "[" + currencyID + "]");
                //
                $(selector).data("balance", balance);
                //
                $(selector).data("currencyID", currencyID);
            };
            //计算各个组件的高度宽度等
            this.initDomSize = function () {
                //
                $(_mImainContent).height($(_transferTop).outerHeight() - $(_mToolbar).outerHeight() - 42);
                //
                $(transferContent).css({
                    "width": "850px"
                });

            };
            //初始化
            this.InitAll = function () {
                //如果是dialog，_mToolbar不现实
                if (dialog == 1) {
                    //不要工具栏
                    $(_mToolbar).remove();
                    //窗口大小调整一下
                    $(window).resize();
                }
                //初始化日期选择空间
                that.initDateCombobox();
                //初始化combobox
                that.InitAccountCombobox();
                //
                mid && that.getTransferData();
                //
                that.calculateExchangeLoss();
                //clear一下
                that.clearFromAccount();
                //初始化高度
                that.initDomSize();
                //初始化点击事件
                that.initEvent();
                //初始化UI
                that.initUI();
            };
        };
        //
        return IVTransferHome;
    })();
    //
    window.IVTransferHome = IVTransferHome;
})();