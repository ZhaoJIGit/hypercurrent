(function () {

    var GLDashboard = (function () {
        //
        var GLDashboard = function (voucherHome) {
            //
            var that = this;
            //定义各个tab对应的index
            var home = 0, summary = 1, docVoucher = 2, monthlyProcess = 3, report = 4, tax = 5;
            //
            var draft = -1, saved = 0, approved = 1, imported = 1, fromApp = 3;
            //
            var periodInput = ".hp-period-input";
            //刷新按钮
            var buttonUpdate = ".hp-button-update";

            //1.本期状态，是否结账
            var periodStatus = ".hp-period-status";
            //2.期初余额是否初始化
            var beginBalanceInited = ".hp-balance-inited";
            //3.结转损益凭证是否生成
            var profitLossVoucherCreated = ".hp-profit-loss-voucher";
            //4.凭证数
            var journalEntry = ".hp-journal-entry";
            //5.未审核凭证书
            var unapprovedVoucher = ".hp-unapproved-voucher";
            //6.业务单据已经转化为凭证数
            var docVoucherCreated = ".hp-has-transfered";
            //7.没有转化为凭证的业务单据数
            var docVoucherUncreated = ".hp-not-transfered";
            //8.导入的凭证数
            var importedVoucher = ".hp-imported-entry";
            //其他应用导入凭证
            var fromappimportedVoucher = ".hp-fromappimported-entry";
            //9.业务系统是否完成勾对
            var reconcileFinished = ".hp-reconcile-finished";
            //10.结账至期间
            var closingPeriod = ".hp-ClosingPeriod";
            var closingPeriodDiv = "#closingPeriodDiv"
            //
            var accountSummary = ".gl-account-summary";

            //获取首页数据的URL
            var getGLDashboardDataUrl = "/GL/GLVoucher/GetGLDashboardData";
            //初始化余额界面的url
            var accountBalanceUrl = "/BD/BDAccount/AccountBalances";
            //
            var item = HtmlLang.Write(LangModule.Common, "item", "item");
            //
            var items = HtmlLang.Write(LangModule.Common, "items", "items");
            //已经结算
            var settled = HtmlLang.Write(LangModule.Common, "Settled", "Settled");
            //未结算
            var unsettled = HtmlLang.Write(LangModule.Common, "Unsettled", "Unsettled");
            //已经结算
            var inited = HtmlLang.Write(LangModule.Common, "Inited", "Inited");
            //未结算
            var uninited = HtmlLang.Write(LangModule.Common, "Uninited", "Uninited");
            //已经生成
            var created = HtmlLang.Write(LangModule.Common, "Created", "Created");
            //未生成
            var uncreated = HtmlLang.Write(LangModule.Common, "Uncreated", "Uncreated");
            //未完成
            var finished = HtmlLang.Write(LangModule.Common, "Finished", "Finished");
            //未完成
            var unfinished = HtmlLang.Write(LangModule.Common, "Unfinished", "Unfinished");
            //等待验证
            var confirmAwaiting = HtmlLang.Write(LangModule.GL, "ComfirmAwaiting", "Awaiting Comfirm");
            //已经结账
            var accountClosed = HtmlLang.Write(LangModule.GL, "AccountClosed", "Account Closed");
            //
            var processFinished = HtmlLang.Write(LangModule.Common, "Finished", "完成");
            //
            var processNotFinished = HtmlLang.Write(LangModule.Common, "notFinished", "未完成");
            //更新
            this.update = function (isSmartVersion,func) {
                //
                that.getDashbaordData(function (data) {
                    //显示数据
                    that.showDashboardData(data, isSmartVersion);
                    //加载事件
                    that.initDashboardEvent(data, isSmartVersion);

                    $.isFunction(func) && func();
                });

            };
            //获取首页的数据
            this.getDashbaordData = function (func) {
                //获取日期
                var periodDate = that.getPeriod();

                if (periodDate === false) return;
                //异步请求
                mAjax.post(getGLDashboardDataUrl, {
                    year: periodDate.getFullYear(),
                    period: periodDate.getMonth() + 1
                }, function (data) {
                    //展示到页面上
                    $.isFunction(func) && func(data);
                }, "", true);
            };
            //展示结果数据
            this.showDashboardData = function (data, isSmartVersion) {
                //A.年期修改
                voucherHome.initTabTitle(home, data.Year + "-" + (data.Period < 10 ? ("0" + data.Period) : data.Period));

                //C.业务单据生成凭证
                !isSmartVersion && voucherHome.initTabTitle(docVoucher, data.CreatedDocVoucherCount + "/" + (data.UncreatedDocVoucherCount + data.CreatedDocVoucherCount));
                //D.是否完成初始化
                voucherHome.initTabTitle(isSmartVersion ? monthlyProcess - 1 : monthlyProcess, (data.MonthProcessFinished || data.Settled) ? processFinished : processNotFinished);

                //1.本期状态，是否结账
                $(periodStatus).text(data.Settled ? settled : unsettled);
                //2.期初余额是否初始化
                $(beginBalanceInited).text(data.PeriodBalanceInited ? inited : uninited);
                //3.结转损益凭证是否生成
                $(profitLossVoucherCreated).text(data.MonthProcessFinished ? created : uncreated);
                //4.凭证数
                $(journalEntry).text(data.VoucherCount + " " + (data.VoucherCount > 1 ? items : item));
                //5.未审核凭证书
                $(unapprovedVoucher).text(data.VoucherSavedCount + " " + (data.VoucherSavedCount > 1 ? items : item));
                //6.业务单据已经转化为凭证数
                $(docVoucherCreated).text(data.CreatedDocVoucherCount + " " + (data.CreatedDocVoucherCount > 1 ? items : item));
                //7.没有转化为凭证的业务单据数
                $(docVoucherUncreated).text(data.UncreatedDocVoucherCount + " " + (data.UncreatedDocVoucherCount > 1 ? items : item));
                //8.导入的凭证数
                $(importedVoucher).text(data.ImportedVoucherCount + " " + (data.ImportedVoucherCount > 1 ? items : item));
                //其他应用导入凭证数
                $(fromappimportedVoucher).text(data.FromAppImportedVoucherCount + " " + (data.FromAppImportedVoucherCount > 1 ? items : item));
                //9.业务系统是否完成勾对
                //$(reconcileFinished).text(data.ReconcileFinished ? finished : unfinished);
                //10.结账至期间
                $(closingPeriod).text(data.ClosingPeriod);
                //没有结账期间时，设置为灰色
                if (data.ClosingPeriod.length < 6) {
                    $(closingPeriodDiv).removeClass("hp-link");
                    $(closingPeriodDiv).addClass("hp-ClosingPeriodDiv");
                }
                else {
                    $(closingPeriodDiv).removeClass("hp-ClosingPeriodDiv");
                    $(closingPeriodDiv).addClass("hp-link");
                }

            };
            //初始化点击事件
            this.initDashboardEvent = function (data, isSmartVersion) {
                //1.本期状态，是否结账
                $(periodStatus).off("click").on("click", function () {
                    //
                    that.redirect2VoucherList("", "");
                });
                //2.期初余额是否初始化
                $(beginBalanceInited).off("click").on("click", function () {
                    that.redirect2AccountBalancce()
                });
                //3.结转损益凭证是否生成
                $(profitLossVoucherCreated).off("click").on("click", function () {
                    //
                    that.redirect2MonthlyProcess(PeriodTransferType.TransferProfitLoss, isSmartVersion);
                });
                //4.凭证数
                $(journalEntry).off("click").on("click", function () {
                    //
                    that.redirect2VoucherList("", "");
                });
                //5.未审核凭证书
                $(unapprovedVoucher).off("click").on("click", function () {
                    //未审核
                    that.redirect2VoucherList(saved);
                });
                //6.业务单据已经转化为凭证数
                $(docVoucherCreated).off("click").on("click", function () {
                    //未审核
                    that.redirect2DocVoucher(saved);
                });
                //7.没有转化为凭证的业务单据数
                $(docVoucherUncreated).off("click").on("click", function () {
                    //未审核
                    that.redirect2DocVoucher(draft);
                });
                //8.导入的凭证数
                $(importedVoucher).off("click").on("click", function () {
                    //未审核
                    that.redirect2VoucherList("", imported);
                });
                //9.其他应用导入的凭证数
                $(fromappimportedVoucher).off("click").on("click", function () {
                    that.redirect2VoucherList("", fromApp);
                });
                //10.结账至期间
                $(closingPeriod).off("click").on("click", function () {
                    if (data.ClosingPeriod.length > 6) {
                        //设置期间 
                        $(periodInput).val(data.ClosingPeriod);
                        //更新
                        that.update(isSmartVersion, function () {
                            //跳转到汇总页
                            that.redirect2VoucherList("", "");
                        });
                        //更新每一个tab页为为初始化的值
                        voucherHome.resetAllTab(isSmartVersion);
                    }
                });
            };
            //跳转到凭证列表
            this.redirect2VoucherList = function (status, from) {
                //先设置好参数
                var voucherList = new GLVoucherList();

                voucherList.clear();
                //
                voucherList.setFilter(status, from);
                //首先切换到凭证列表
                voucherHome.showTab(1);
            };
            //跳转到初始化余额页面
            this.redirect2AccountBalancce = function () {
                //
                $.mTab.addOrUpdate(HtmlLang.Write(LangModule.Report, "AccountBalanceReport", "Account balance Report"), '/Report/Report2/41', true);
            };
            //跳转到期末转结列表，别滚动到结转损益凭证
            this.redirect2MonthlyProcess = function (transferTypeID, isSmartVersion) {
                //先设置好参数
                var monthlyProcess = new GLMonthlyProcess();
                //
                monthlyProcess.setFilter(transferTypeID);
                //首先切换到期末转结页面
                voucherHome.showTab(isSmartVersion ? 2 : 3);
            };
            //跳转到业务单据转到凭证的界面，
            this.redirect2DocVoucher = function (status) {
                //先把参数给设置好
                var docVoucher = new GLDocVoucher();

                docVoucher.clear();
                //设置参数
                docVoucher.setFilter(status);
                //首先切换到业务单据转凭证
                voucherHome.showTab(2);
            };
            //获取用户选的期
            this.getPeriod = function () {
                //
                var period = $(periodInput).val();

                if (!period) {
                    mDialog.message(HtmlLang.Write(LangModule.GL, "PleaseSelectAPeriod", "请选择一个会计期间"));
                    return false;
                }
                //
                return mDate.parse(period + "-01");
            };
            //初始化点击
            this.bindEvent = function (isSmartVersion) {
                //刷新按钮
                $(buttonUpdate).off("click").on("click", function () {
                    that.update(isSmartVersion);
                    //更新每一个tab页为为初始化的值
                    voucherHome.resetAllTab(isSmartVersion);
                });
            };
            //
            this.init = function (isSmartVersion) {
                //绑定事件
                that.bindEvent(isSmartVersion);
                //刷新一下
                that.update(isSmartVersion);
            };
        }
        return GLDashboard;
    })();

    window.GLDashboard = GLDashboard;
})()