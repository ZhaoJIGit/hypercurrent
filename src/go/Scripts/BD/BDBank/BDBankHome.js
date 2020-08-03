//初始化银行首页
(function (window) {
    //
    var BDBankHome = (function () {
        //
        var _mBankBoxLeft = ".m-bank-box-left";
        //主div
        var _mImainContent = ".m-imain-content";
        //工具栏
        var _mToolbar = ".m-toolbar";
        //m-imain
        var _mImain = ".m-imain";
        //银行标题
        var _headerTitleTxt = ".header-title-txt";
        //一级标题
        var _firstTitleTxt = ".first-title-txt";
        //linkbutton获取文字的样式
        var _linkButtonText = ".l-btn-text";
        //二级标题
        var _secondTitle = ".second-title";
        //银行名称
        var _accountBank = ".account-bank";
        //
        var divBankInfo = "#divBankInfo";
        //需要对账
        var _needReconcile = ".need-reconcile";
        //提醒用户倒入对账单
        var _importBankFeed = ".import-bank-feed";
        //更新日期
        var _acountUpdateDate = ".account-update-date";
        //编辑，删除帐户的点击按钮
        var _headerCuidButton = ".header-cuid-button";
        //包含/删除/编辑帐户按钮的父div
        var _headerCuidSubDiv = ".header-cuid-sub-div";
        //
        var updateDate = HtmlLang.Write(LangModule.Bank, "UpdateDate", "Update Date") + ":";
        //
        var reconcileName = HtmlLang.Write(LangModule.Bank, "Reconcile", "Reconcile");
        //编辑帐户
        var _editAccount = ".edit-account";
        //删除帐户
        var _deleteAccount = ".delete-account";
        //帐户管理
        var _headerManageButton = ".header-manage-button";
        //
        var _headerManageSubDiv = ".header-manage-sub-div";
        //支出
        var _payment = ".payment";
        //收入
        var _receipt = ".receipt";
        //转账
        var _transfer = ".transfer";
        //导入对账单
        var _import = ".import";
        //直连银行导入对账单
        var _bankFeeds = ".bank-feeds";
        //交易单记录
        var _transaction = ".transaction";

        var _cashCoding = ".transaction-cashCodeing";

        //银行转账记录
        var _statement = ".statement";
        //现金对账单
        var _cashStatement = ".cash-statement";
        //银行对账单
        var _bankStatement = ".bank-statement";
        //对账单
        var _reconcile = ".reconcile";
        //对账账户
        var _reconcileAccount = ".reconcile-account";
        //对账报表
        var _reconcileReport = ".reconcile-report";
        //帐户图表
        var _mainChart = ".main-chart";
        //银行帐户余额
        var _bankBalanceDataSpan = ".bank-balance-data-span";
        //美记帐户余额
        var _megiBalanceDataSpan = ".megi-balance-data-span";
        //对账单div
        var _reconcileDiv = ".reconcile-div";
        //需要对账的项目数，文字
        var _reconcileCount = ".reconcile-count";
        //不需要对账
        var _noReconcile = ".no-reconcile";
        //底部
        var _mBoxFooter = ".m-box-footer";
        //定义10个背景颜色的样式
        /*
        0：area区域的颜色
        1：边框的颜色
        2：组件中字体颜色
        3：labe中字体的颜色
        4：数字提示框内部字体的颜色
        5：十字线的颜色
        6：背景区域的颜色
        7:坐标轴的颜色
        8：整个图片数据部分的背景颜色，最好是白色
        9：坐标网格的颜色
        */
        var _chartBgColors = [
            ["#99ccf6", "#e3eaed", "#ffffff", "#f4f9fb", "#f4f9fb", "#145d79", "#ffffff", "#449be0", "#ffffff", "#ffffff"],
            ["#f2e5ca", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#fab122", "#ffffff", "#ffffff"],
            ["#a5c8d6", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#0579a5", "#ffffff", "#ffffff"],
            ["#f2e5ca", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#fab122", "#ffffff", "#ffffff"],
            ["#a5c8d6", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#0579a5", "#ffffff", "#ffffff"],
            ["#f2e5ca", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#fab122", "#ffffff", "#ffffff"],
            ["#a5c8d6", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#0579a5", "#ffffff", "#ffffff"],
            ["#f2e5ca", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#fab122", "#ffffff", "#ffffff"],
            ["#a5c8d6", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#0579a5", "#ffffff", "#ffffff"],
            ["#f2e5ca", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#fab122", "#ffffff", "#ffffff"]
        ];
        //
        var BDBankHome = function (acctId, c, r, p) {
            //需要用的常量
            //先通过异步请求，获取用户所有的帐户信息
            this.url = "/BD/BDBank/GetBDBankAccountViewList";
            //各个样式的名字
            //新增帐户的url
            this.editBankAccountUrl = "/BD/BDBank/BDBankAccountEdit";
            //删除帐户的url
            this.deleteAccountUrl = "/BD/BDBank/BDBankAccountDelete";
            //银行转账的url
            this.transferUrl = "/IV/IVTransfer/IVTransferHome";
            //bank rules
            this.bankRuleUrl = "/BD/BDBank/BDBankRuleHome";
            //收入
            this.receiptUrl = "/IV/Receipt/ReceiptEdit";
            //支出
            this.paymentUrl = "/IV/Payment/PaymentEdit";
            //银行对账单
            this.reconcileUrl = "/BD/BDBank/BDBankReconcileHome";
            //导入对账单
            this.importUrl = "/BD/BDBank/Import";
            //直连银行导入对账单
            this.bankFeedsUrl = "/BD/BDBank/ImportByBankFeeds";
            //交易列表
            this.transactionUrl = "/BD/BDBank/BDBankReconcileHome";
            //对账单报表
            this.reconcileReportUrl = "/Report/Report2/11";
            //
            var that = this;
            //获取图形宽度
            this.getChartWidth = function () {
                //
                var width = $(document).width() - 250;
                return width;
            };
            //做一个没有数据的图形
            this.initEmptyChart = function (selector, colors) {
                var title = HtmlLang.Write(LangModule.Common, "nodata", "No data");
                var data = [
                    {
                        name: '',
                        value: [0],
                        color: colors[0],
                        line_width: 1
                    }
                ];
                var chart = new iChart.Area2D(
                    {
                        render: selector,
                        data: data,
                        border: {
                            enable: true,
                            width: [0, 0, 0, 0],
                            color: colors[1]
                        },
                        title: title,
                        align: "left",
                        width: that.getChartWidth(),
                        height: 200,
                        offsetx: 35,
                        color: colors[2],
                        label: { color: colors[3], fontsize: 12 },
                        labels: ["", "", "", "", ""],
                        tip: {
                            enable: true
                        },
                        crosshair: {
                            enable: true,
                            gradient: true,
                            line_color: colors[5],
                            line_width: 1
                        },
                        sub_option: {
                            hollow_inside: false,
                            label: false,
                            point_size: 8
                        },
                        background_color: colors[6],
                        coordinate: {
                            axis: {
                                width: [0, 0, 2, 2],
                                color: colors[7]
                            },
                            background_color: colors[8],
                            height: '80%',
                            grid_color: colors[9],
                            scale: {
                                scale_enable: false,
                                scale_share: 0
                            }
                        }
                    });
                chart.draw();
            };
            //调用第三方插件，生成图像
            this.initChart = function (selector, chartData, colors) {
                //如果没有数据
                if (!chartData) {
                    //绘制空白图形
                    return that.initEmptyChart(selector, colors);
                }
                var data = [
                    {
                        name: null,
                        value: chartData.MValue,
                        color: colors[0],
                        line_width: 1
                    }];

                var chart = new iChart.Area2D(
                    {
                        render: selector,
                        data: data,
                        border: {
                            enable: true,
                            width: [0, 0, 0, 0],
                            color: colors[1]
                        },
                        width: that.getChartWidth(),
                        height: 200,
                        offsetx: 35,
                        align: "left",
                        color: colors[2],
                        label: { color: colors[3], font: 'Microsoft Yahei', fontsize: 12 },
                        tip: {
                            enable: true,
                            listeners: {
                                //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                                parseText: function (tip, name, value, text, i) {
                                    var lable = chartData.MTipLabels[i];
                                    return "<div class='m-chart-tip'><p class='tip-lable'>" + lable + "</p><p class='tip-value' style='color:'" + colors[4] + "'>" + mMath.toMoneyFormat(chartData.MValue[i]) + "</p></div>";
                                }
                            }
                        },
                        crosshair: {
                            enable: true,
                            gradient: true,
                            line_color: colors[5],
                            line_width: 1
                        },
                        sub_option: {
                            hollow_inside: false,
                            label: false,
                            point_size: 8
                        },
                        background_color: colors[6],
                        coordinate: {
                            axis: {
                                width: [0, 0, 2, 2],
                                color: colors[7]
                            },
                            background_color: colors[8],
                            height: '80%',
                            grid_color: colors[9],
                            scale: {
                                scale_enable: false,
                                scale_share: 0
                            }
                        }
                    });
                chart.draw();
            }
            //新增银行帐户
            this.editBankAccount = function (type, id, onCloseFns, origin) {
                //标题
                var title = id ? HtmlLang.Write(LangModule.Bank, "Edit" + BankTypeEnum[type].baseName.replace(" ", "") + "Account", "Edit " + BankTypeEnum[type].name.toUpperChar() + " Account") : HtmlLang.Write(LangModule.Bank, "add" + type + "account", "Add " + BankTypeEnum[type].name.toUpperChar() + " Account");
                //默认高度
                var height = BankTypeEnum[type].height;
                //弹窗
                $.mDialog.show({
                    mTitle: title,
                    mWidth: 450,
                    mHeight: height,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + that.editBankAccountUrl + "?type=" + type + "&id=" + id + '&Origin=' + (origin || ""),
                    mCloseCallback: [onCloseFns]
                });
            };
            //转账
            this.transferBusiness = function (acctid) {
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "TransferBusiness", "Transfer");
                //弹窗
                $.mTab.addOrUpdate(title, that.transferUrl + "?acctId=" + acctid, true);
            };
            //收入
            this.receiptBusiness = function (acctid) {
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "ReceiveMoney", "Receive Money");
                //弹窗
                $.mTab.addOrUpdate(title, that.receiptUrl + "?acctid=" + acctid, true);
            };
            //支出
            this.paymentBusiness = function (acctid) {
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "SpendMoney", "Spend Money");
                //弹窗
                $.mTab.addOrUpdate(title, that.paymentUrl + "?acctid=" + acctid, true);
            };
            //Bank Rules
            this.bankRulesBusiness = function () {
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "BankRules", "Bank Rules");
                //弹窗
                $.mTab.addOrUpdate(title, that.bankRuleUrl, true);
            };
            //对账
            this.reconcileBusiness = function (acctid, type, index, bankType, bankName) {
                //弹窗
                $.mTab.addOrUpdate(bankName, that.reconcileUrl + "?acctid=" + acctid + "&index=" + index + "&type=" + type + "&bankType=" + bankType, true);
            };
            //bank statement 
            this.statementBusiness = function (acctid, type, index, bankType, bankName) {
                //弹窗
                $.mTab.addOrUpdate(bankName, that.reconcileUrl + "?acctid=" + acctid + "&index=" + index + "&type=" + type + "&bankType=" + bankType, true);
            };
            //交易信息
            this.transactionBusiness = function (acctid, type, index, bankType, bankName) { //标题
                //弹窗
                $.mTab.addOrUpdate(bankName, that.reconcileUrl + "?acctid=" + acctid + "&index=" + index + "&type=" + type + "&bankType=" + bankType, true);

            };
            ///单个账户信息管理
            //删除银行帐户
            this.deleteBankAccount = function (type, id, callback, bankName) {
                //弹出确认框确认
                $.mDialog.confirm(LangKey.AreYouSureToDelete, LangKey.Confirm, function (sure) {
                    //确认
                    if (sure) {
                        //异步删除
                        mAjax.submit(that.deleteAccountUrl, { MItemID: id }, function (msg) {
                            //展示结果
                            if (msg.Success) {
                                //1.5秒显示操作成功信息
                                mMsg(LangKey.DeleteSuccessfully);
                                $.mTab.remove(bankName);
                            }
                            else {

                                $.mDialog.alert(msg.Message);

                            }
                            //回调函数
                            if (callback && (typeof callback == "function")) {
                                callback();
                            }
                        }, function (msg) {
                            //1.5秒显示操作失败信息
                            mMsg(LangKey.Error);
                        });
                    }
                });
            }
            //导入对账单
            this.importBusiness = function (type, bankType, id) {
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "ImportStatement", "Import a Statement");

                //非银行
                if (type != "1" && type != "2") {
                    bankType = BankTypeEnum[type].baseName;
                }

                //弹窗
                $.mDialog.show({
                    mTitle: title,
                    mWidth: 990,
                    mHeight: 500,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + that.importUrl + "?type=" + bankType + "&id=" + id
                });
            };
            this.bankFeedsBusiness = function (bankType, id) {
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "GetBankFeeds", "Get Bank Feeds");

                //弹窗
                $.mDialog.show({
                    mTitle: title,
                    mWidth: 500,
                    mHeight: 380,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + that.bankFeedsUrl + "?type=" + bankType + "&id=" + id
                });
            },
                //进入对账单报表
                this.reconcileReportBusiness = function (id) {
                    //
                    var title = HtmlLang.Write(LangModule.Bank, "bankreconcilereport", "Bank Reconcile Report", true);
                    //弹窗
                    $.mTab.addOrUpdate(title, that.reconcileReportUrl + "?filter={'MBankAccountID':'" + id + "','MDate':'/Date(" + mDate.DateNow.getTime() + ")/'}", true);
                };
            //初始化新建帐户/收入/支出/bankrule的点击事件
            this.initTooBarEvent = function () {
                //新增银行 默认
                $("#aNewBankAccountTop").off("click").on("click", function () {
                    that.editBankAccount(BankTypeEnum[1].type, "", that.InitAll, "Banking")
                });
                //新增银行
                $("#aNewBankAccount").off("click").on("click", function () {
                    that.editBankAccount(BankTypeEnum[1].type, "", that.InitAll, "Banking")
                });
                //新增信用卡
                $("#aNewCreditAccount").off("click").on("click", function () {
                    that.editBankAccount(BankTypeEnum[2].type, "", that.InitAll, "Banking")
                });
                //新增现金帐户
                $("#aNewCashAccount").off("click").on("click", function () {
                    that.editBankAccount(BankTypeEnum[3].type, "", that.InitAll, "Banking")
                });
                //新增Paypal帐户
                $("#aNewPayPalAccount").off("click").on("click", function () {
                    that.editBankAccount(BankTypeEnum[4].type, "", that.InitAll, "Banking")
                });
                //新增aplipay帐户
                $("#aNewAlipayAccount").off("click").on("click", function () {
                    that.editBankAccount(BankTypeEnum[5].type, "", that.InitAll, "Banking")
                });
                //转账
                $("#aTransfer").off("click").on("click", function () { that.transferBusiness("") });
                //收入
                $("#aReceipt").off("click").on("click", function () { that.receiptBusiness(""); });
                //支出
                $("#aPayment").off("click").on("click", function () { that.paymentBusiness(""); });
                //Bank rules
                $("#aBankRules").off("click").on("click", function () { that.bankRulesBusiness(""); });
            };
            //初始化各个银行帐户信息里面的按钮点击事件 selector 本帐户div的选择器
            this.initBankAccountEvent = function (selector, type, id, bankType, bankName) {
                //编辑按钮
                $(_editAccount, selector).off("click").on("click", function () {
                    //编辑
                    that.editBankAccount(type, id, that.InitAll);
                });
                //删除帐户
                $(_deleteAccount, selector).off("click").on("click", function () {
                    that.deleteBankAccount(type, id, that.InitAll, bankName);
                });
                //账户设置
                $(_headerCuidButton, selector).mTip({
                    target: $(_headerCuidSubDiv, selector),
                    width: 135,
                    parent: $(_headerCuidSubDiv, selector).parent()
                });
                //账户管理
                $(_headerManageButton, selector).mTip({
                    target: $(_headerManageSubDiv, selector),
                    width: 305,
                    parent: $(_headerManageSubDiv, selector).parent()
                });
                //receipt edit
                $(_receipt, selector).off("click").on("click", function () {
                    return that.receiptBusiness(id);
                });
                //payment
                $(_payment, selector).off("click").on("click", function () {
                    return that.paymentBusiness(id);
                });
                //transfer
                $(_transfer, selector).off("click").on("click", function () {
                    //
                    return that.transferBusiness(id);
                });
                //import
                $(_import, selector).off("click").on("click", function () {
                    return that.importBusiness(type, bankType, id);
                });
                //bank feeds
                $(_bankFeeds, selector).off("click").on("click", function () {
                    //调用公用方法
                    return that.bankFeedsBusiness(bankType, id);
                });
                //reconcile
                $(_needReconcile, selector).off("click").on("click", function () {
                    return that.reconcileBusiness(id, type, 1, bankType, bankName);
                });
                $(_reconcileAccount, selector).off("click").on("click", function () {
                    return that.reconcileBusiness(id, type, 1, bankType, bankName);
                });
                //no-reconcile
                $(_noReconcile, selector).off("click").on("click", function () {
                    return that.reconcileBusiness(id, type, 3, bankType, bankName);
                });
                //statement
                $(_statement, selector).off("click").on("click", function () {
                    return that.statementBusiness(id, type, 2, bankType, bankName);
                });
                //transaction
                //账户交易明细单击事件的绑定
                $(_transaction, selector).off("click").on("click", function () {
                    return that.transactionBusiness(id, type, 3, bankType, bankName);
                });

                $(_cashCoding, selector).off("click").on("click", function () {
                    return that.transactionBusiness(id, type, 4, bankType, bankName);
                });

                //reconcile report
                $(_reconcileReport, selector).off("click").on("click", function () {
                    return that.reconcileReportBusiness(id);
                });
                //倒入对账单
                $(_importBankFeed, selector).off("click").on("click", function () {
                    return that.importBusiness(type, bankType, id);
                });
                //
                if (c !== "True") {
                    //
                    $(_headerCuidButton, selector).remove();
                    //不可新建帐号
                    $(_editAccount, selector).remove();
                    //不可删除帐号
                    $(_deleteAccount, selector).remove();
                    //转账
                    $(_transfer, selector).remove();
                    //付款
                    $(_payment, selector).remove();
                    //收款
                    $(_receipt, selector).remove();
                    //
                }
                if (r !== "True") {
                    //不可勾对
                    $(_reconcileAccount, selector).remove();
                    //
                    $(_reconcileDiv, selector).remove();

                    //不可查看帐单记录
                    $(_statement, selector).remove();
                    //不可导入对账单
                    $(_import, selector).remove();
                    //不可获取银行对账单
                    $(_bankFeeds, selector).remove();

                    $(_reconcileReport, selector).remove();
                }
                //
                if (p !== "True") {
                    //不可查看勾对报表
                    $(_reconcileReport, selector).remove();
                }
                //如果三个权限都没有
                if (r !== "True" && p !== "True" && c !== "True") {
                    //直接隐藏点击蓝
                    $(_headerManageButton, selector).remove();
                }
                $(".m-bank-box-top").each(function () {
                    if ($(this).find(".manage-table>li").length == 0) {
                        $(this).find(_headerManageButton).remove();
                    }
                });
            }
            //初始化页面图像
            this.initBankHome = function (msg) {
                //如果请求数据失败
                if (!msg) {
                    return false;
                }
                //获取数据
                var data = msg;
                //顶层box
                var $top = $(".m-imain-content");

                var isSmartVersion = $("#hideVersion").val();

                //遍历获取的银行信息
                for (var i = 0; i < data.length; i++) {
                    //
                    var account = data[i];
                    //添加一个模版的div到body
                    var $bankDiv = $(divBankInfo).clone()
                    //修改id
                    $bankDiv.attr("id", account.MItemID);
                    //append
                    $bankDiv.appendTo($top);
                    //再根据id来获取
                    $bankDiv = $("#" + account.MItemID);

                    //总账版，如果账号没有选择是否勾兑，隐藏整个导航
                    if (isSmartVersion && !account.MIsNeedReconcile) {
                        $(".header-manage-button", $bankDiv).hide();
                    }


                    //显示
                    $bankDiv.show();
                    //进行遮罩
                    $bankDiv.mask("");
                    //
                    var bankName = account.MBankName;
                    //帐户名称
                    $(_firstTitleTxt, $bankDiv).text(account.MBankName || " ").append("<span style='font-size:20px'>[" + account.MCyID + "]</span>");
                    //银行名称
                    $(_accountBank, $bankDiv).text(BankTypeEnum[account.MBankAccountType].name + (account.MBankTypeName ? (":" + account.MBankTypeName) : ""));
                    //最后更新日期
                    $(_acountUpdateDate, $bankDiv).text(updateDate + mDate.formatter(account.MLastUpdateDate, 'yyyy-MM-dd'));
                    //添加表明的字样

                    //帐户余额
                    $(_bankBalanceDataSpan, $bankDiv).html(mMath.toMoneyFormat(account.MBankStatement));
                    //$(_bankBalanceDataSpan, $bankDiv).html('<span style="font-size:20px,font-color:red">' + account.MCyID + ''+mMath.toMoneyFormat(account.MBankStatement)+'</span>');
                    //美记帐户余额
                    $(_megiBalanceDataSpan, $bankDiv).html(mMath.toMoneyFormat(account.MMegiBalance));

                    //如果美记帐户有余额，银行账户没有余额，且没有对账内容
                    if (account.MMegiBalance != 0 && account.MBankStatement == 0 && account.MReconcileCount == 0 && !account.MIsCash && $(_importBankFeed, $bankDiv).length > 0) {
                        //提醒倒入对账单
                        $(_importBankFeed, $bankDiv).show();
                        //显示勾勾
                        $(_noReconcile, $bankDiv).hide();
                        //
                        $(_needReconcile, $bankDiv).hide();
                    }
                    else if (account.MBankAccountType == BankTypeEnum[3].type && account.MReconcileCount == 0) {//对于非现金账户,获取没有需要对账的
                        //显示勾勾
                        $(_noReconcile, $bankDiv).show();
                        //
                        $(_needReconcile, $bankDiv).hide();
                        //
                        $(_importBankFeed, $bankDiv).hide();
                    }
                    else {
                        var $reconclie = $(_needReconcile, $bankDiv);
                        //显示按钮
                        $reconclie.show();
                        //
                        $(_noReconcile, $bankDiv).hide();
                        //
                        $reconclie.text($reconclie.text() + "[" + account.MReconcileCount + "]");
                        //
                        $(_importBankFeed, $bankDiv).hide();
                    }
                    //
                    var chartId = account.MItemID + "_chart"
                    //给图像区域赋值一个id
                    $(_mainChart, $bankDiv).attr("id", chartId);
                    //声称图像
                    that.initChart(chartId, account.MBankChartInfo, _chartBgColors[i % _chartBgColors.length]);
                    //初始化事件
                    that.initBankAccountEvent($bankDiv, account.MBankAccountType, account.MItemID, account.MBankTypeID, account.MBankName);
                    //如果不需要勾对
                    if (!account.MIsNeedReconcile) {
                        that.hideReconcilePart($bankDiv);
                    }
                    //非银行账户
                    if (account.MBankAccountType != BankTypeEnum[1].type) {
                        $(_cashStatement, $bankDiv).show();
                        $(_bankStatement, $bankDiv).hide();
                    }
                    $("#spBankBalance", $bankDiv).hide();
                    $("#spStatementBalance", $bankDiv).hide();
                    if (account.MBankAccountType == BankTypeEnum[1].type || account.MBankAccountType == BankTypeEnum[2].type) {
                        $("#spBankBalance", $bankDiv).show();
                    } else {
                        $(_bankFeeds, $bankDiv).remove();
                        $("#spStatementBalance", $bankDiv).show();
                    }
                    //加载完成就取消遮罩
                    $bankDiv.unmask();
                }
                //如果有定位逻辑
                if (acctId) {
                    //需要定位的div
                    var $scrollToDiv = $("#" + acctId);
                    //定位到
                    $(_mImain).animate({ scrollTop: $scrollToDiv.offset().top - $(_mImain).offset().top });
                }

            };
            //处理权限问题
            this.initPermission = function () {
                //
            };
            //移除现金账户的部分功能
            this.hideReconcilePart = function ($bankDiv) {
                $(_import, $bankDiv).remove();
                $(_bankFeeds, $bankDiv).remove();
                $(_statement, $bankDiv).remove();
                $(_reconcileAccount, $bankDiv).remove();
                $(_reconcileReport, $bankDiv).remove();
                $(_reconcileDiv, $bankDiv).remove();
                $(".bank-balance", $bankDiv).remove();
            };
            //初始化高度
            this.initDomSize = function () {
                //
                $(_mImainContent).height($(_mImain).outerHeight() - $(_mToolbar).outerHeight() - 20);
            };
            //清空账户信息
            this.clearAccountInfo = function () {
                //
                $(divBankInfo).siblings().remove();
            };
            //初始化所有
            this.InitAll = function () {
                //初始化工具栏
                that.initTooBarEvent();
                //
                that.clearAccountInfo();
                //异步
                $.mAjax.post(that.url, "", that.initBankHome);
                //
                that.initDomSize();
            };
        };
        //
        return BDBankHome;
    })();
    //
    window.BDBankHome = BDBankHome;
})(window)