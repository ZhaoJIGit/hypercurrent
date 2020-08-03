;
var BDBankAccountEdit = {};
//执行匿名函数
(function () {
    //
    var BDBankAccountEdit = (function () {
        //保存银行帐户url
        var saveBDBankAccountUrl = "/BD/BDBank/SaveBDBankAccount";
        //银行账户选择div
        var bankBankTypeInputDiv = "#bankBankTypeInputDiv";
        //银行账户类型，账号输入的div名称
        var bankBankNoInputDivSelector = "#bankBankNoInputDiv";
        //信用卡账户类型，账号输入的div名称
        var creditBankNoInputDivSelector = "#creditBankNoInputDiv";
        //
        var BDBankAccountEdit = function (bankType) {
            //
            var that = this;
            //处理dom隐藏和显示
            this.showDom = function () {
                //
                switch (bankType) {
                    case BankTypeEnum[1].type:
                        $("#MIsNeedReconcile").attr("checked", true);
                        $("#MIsNeedReconcile").attr("disabled", "disabled");
                        //信用卡号输入框移除
                        $(creditBankNoInputDivSelector).remove();
                        break;
                    case BankTypeEnum[2].type:
                        $("#MIsNeedReconcile").attr("checked", true);
                        $("#MIsNeedReconcile").attr("disabled", "disabled");
                        //支票账户
                        $(bankBankNoInputDivSelector).remove();
                        break;
                    case BankTypeEnum[3].type:
                        //银行类型移除
                        $(bankBankTypeInputDiv).remove();
                        //都移除
                        $(creditBankNoInputDivSelector).remove();
                        //
                        $(bankBankNoInputDivSelector).remove();
                        break;
                    case BankTypeEnum[4].type:
                    case BankTypeEnum[5].type:
                        //银行类型移除
                        $(bankBankTypeInputDiv).remove();
                        //信用卡移除
                        $(creditBankNoInputDivSelector).remove();
                        break;
                    default:
                        break;
                }
            };
            //保存账户
            this.saveBankAccount = function () {
                //如果是新的帐户，需要激活的话，则标明
                if (!$(keyFieldSelector).val()) {
                    $("input[name='MIsActive']").val("true");
                }

                //传过去的参数
                var model = { MBankAccountType: bankType, MIsCheckExists: true };
                //现金账号时加上cash
                if (bankType == 3) {
                    var model = { MBankAccountType: bankType, MBankTypeID: "Cash", MIsCheckExists: true };
                }

                //成功后的回调函数
                var saveBanckAccountCallback = function (msg) {
                    if (!msg.Success) {
                        $.mDialog.alert(msg.Message);
                        return;
                    }

                    //账户id
                    var hidAccountID = $("#hidAccountID").val();
                    //成功信息
                    var successMsg = hidAccountID.length > 0 ? HtmlLang.Write(LangModule.Bank, bankType + "AccountUpdated", BankTypeEnum[bankType].name.toUpperChar() + " Account Updated") : HtmlLang.Write(LangModule.Bank, "NewBankAccountAdded", "New " + BankTypeEnum[bankType].name.toUpperChar() + " Account Added");
                    //展示信息
                    mMsg(successMsg);

                    //关闭窗口
                    $.mDialog.close(0);

                }
                //异步提交
                $("body").mFormSubmit({ url: saveBDBankAccountUrl, param: { model: model }, callback: saveBanckAccountCallback });
            };
            //初始化事件
            this.initEvent = function () {
                //取消事件
                $("#btnCancel").off("click").on("click", $.mDialog.close);
                //保存事件
                $("#btnSave").off("click").on("click", that.saveBankAccount);
            };
            //获取模型数据
            this.getModelData = function () {
                //获取数据
                $("body").mFormGet({
                    url: "/BD/BDBank/GetBDBankAccountEditModel", callback: function (msg) {
                        that.showDom();
                        if (msg.MIsNeedReconcile && msg.MHasBankBillData) {
                            $("#MIsNeedReconcile").attr("disabled", "disabled");
                        }
                        if (msg.MBankIsUse) {
                            $("#selCurrencyCode").combobox("disable");
                            //
                            $("#MBankTypeID").combobox("disable");
                            //隐藏下拉列表的 面板
                            $("#MBankTypeID").combobox("hidePanel");
                        }
                    }
                });
            };
            //
            this.init = function () {
                //最先处理显示以及隐藏信息
                this.showDom();
                //注册事件
                this.initEvent();
                //获取数据
                this.getModelData();
            };
        };
        //
        return BDBankAccountEdit;
    })();
    //注册到window
    window.BDBankAccountEdit = BDBankAccountEdit;
})()