(function () {
    var BDBankReconcileHome = (function () {
        //整个页面
        var _main = ".bank-reconcile-main";
        //header
        var _mainHeader = ".main-header";
        //bottom
        var _mainButtom = ".main-buttom";
        //银行名称
        var _bankNameText = ".bank-name-text";
        //账户信息
        var _accountCuid = ".account-cuid";
        //账户管理
        var _accountInfo = ".account-info";
        //银行余额
        var _bankBalance = ".balance-text.bank";
        //美记余额
        var _megiBalance = ".balance-text.megi";
        //对账单数量
        var _headerReconcileCount = ".header-reconcile-count";
        //点击对账
        var _reconcile = "#reconcile";
        //对账分布视图
        var _reconcilePartial = ".reconcile-partial";
        //
        var icoDate = ".m-icon-date";
        //点击银行记录
        var _statement = "#statement";
        //银行分布视图
        var _statementPartial = ".statement-partial";
        //点击交易记录
        var _transaction = "#transaction";
        //交易分布视图
        var _transactionPartial = ".transaction-partial";
        //cash coding
        var _cashCodeing = "#cashCodeing";
        //cash coding分布视图
        var _cashCodinglPartial = ".cash-coding-partial";

        //statement detail
        var _statementDetail = "#statementDetail";
        //statement detail分布视图
        var _statementDetailPartial = ".statement-detail-partial";

        //编辑/删除功能
        var _accountCuidSubDiv = ".account-cuid-sub-div";
        //账户信息功能
        var _accountInfoSubDiv = ".account-info-sub-div";
        //编辑账户
        var _editAccount = ".edit-account";
        //删除账户
        var _deleteAccount = ".delete-account";
        //银行操作功能
        var _accountManageButton = ".account-manage-button";
        //
        var _accountManageSubDiv = ".account-manage-sub-div";
        //银行内的各个操作
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
        //对账报表
        var _reconcileReport = ".reconcile-report";
        //各个对账记录
        var _reconciles = ".reconcile";
        //头部选中的样式
        var _mainHeaderSelectedClass = "main-header-selected";
        //获取银行信息url
        var _getBankInfoUrl = "/BD/BDBank/GetBDBankAccountInfoByDate";
        //日期选择控件
        var _dateRangePicker = ".daterangepicker-span";
        //日期选择控件
        var _dateRangePickerTrigger = ".daterangepicker-span";
        //
        var _dateRangePickerArrow = ".daterangepicker-arrow,.m-icon-date";
        //是否初始化tab的属性
        var inited = "inited";
        //类定义
        var BDBankReconcileHome = function (type, r, id, index, bankType, isNeedReconcile) {
            isNeedReconcile = isNeedReconcile == "True" ? true : false;

            //
            var that = this;
            //点击对账单的事件
            this.reconcileClicked = function () {
                //切换选中样式
                that.switchHeaderClass(this);
                //展示视图
                $(_reconcilePartial).show().siblings().hide();
                new BDBankReconcile(r).InitReconcileList(id);
                $(window).resize();
            };
            //点击对账单的事件
            this.statementClicked = function () {
                //切换选中样式
                that.switchHeaderClass(this);
                //展示视图
                $(_statementPartial).show().siblings().hide();
                window.BDBankStatement.InitStatementList(id);
                $(window).resize();
            };
            //点击交易记录事件
            this.transactionClicked = function () {
                //切换选中样式
                that.switchHeaderClass(this);
                //展示视图
                $(_transactionPartial).show().siblings().hide();
                window.BDBankTransaction.InitTransactionList(id, that, isNeedReconcile);
                $(window).resize();
            };
            //点击cash coding 事件
            this.cashCodingClicked = function () {
                //切换选中样式
                that.switchHeaderClass(this);
                //展示视图
                $(_cashCodinglPartial).show().siblings().hide();
                window.BDBankCashCoding.InitCashCodingList(id);

                $(window).resize();
            };
            //点击对账单的事件
            this.statementDetailClicked = function () {
                //切换选中样式
                that.switchHeaderClass(this);
                //展示视图
                $(_statementDetailPartial).show().siblings().hide();
                window.BDBankStatementDetail.init(id);
                $(window).resize();
            };
            //切换样式
            this.switchHeaderClass = function (selector) {
                //先掉所有的选中央视
                $("span", _mainHeader).removeClass(_mainHeaderSelectedClass);
                //本身添加选中样式，其他的去掉样式
                $(selector).addClass(_mainHeaderSelectedClass)
            };
            this.reloadPage = function () {
                mWindow.reload();
            }
            //初始化银行账户编辑/一行账户信息点击事件
            this.initAccountManageEvent = function () {
                //
                var bankHome = new BDBankHome();
                //账户设置
                $(_accountCuid).mTip({
                    target: $(_accountCuidSubDiv),
                    width: 120,
                    parent: $(_accountCuidSubDiv).parent()
                });
                //账户信息
                $(_accountInfo).mTip({
                    target: $(_accountInfoSubDiv),
                    width: 240,
                    parent: $(_accountInfoSubDiv).parent()
                });
                //账户编辑信息
                if ($(_accountManageButton).length > 0) {
                    //绑定弹出信息
                    $(_accountManageButton).mTip(
                        {
                            target: $(_accountManageSubDiv),
                            width: 305,
                            parent: $(_accountManageSubDiv).parent()
                        });
                }
                //删除账户
                $(_deleteAccount).off("click").on("click", function () {
                    //调用公用方法
                    return bankHome.deleteBankAccount(type, id);
                });
                //编辑账户
                $(_editAccount).off("click").on("click", function () {
                    //
                    return bankHome.editBankAccount(type, id, that.reloadPage);
                });
                //转账
                $(_transfer).off("click").on("click", function () {
                    //调用公用方法
                    return bankHome.transferBusiness(id);
                });
                //付款
                $(_payment).off("click").on("click", function () {
                    //调用公用方法
                    return bankHome.paymentBusiness(id);
                });
                //收款
                $(_receipt).off("click").on("click", function () {
                    //调用公用方法
                    return bankHome.receiptBusiness(id);
                });
                //导入对账单
                $(_import).off("click").on("click", function () {
                    //调用公用方法
                    return bankHome.importBusiness(type, bankType, id);
                });
                //获取银行对账单
                $(_bankFeeds).off("click").on("click", function () {
                    //调用公用方法
                    return bankHome.bankFeedsBusiness(bankType, id);
                });
                //勾对报表
                $(_reconcileReport).off("click").on("click", function () {
                    //调用公用方法
                    return bankHome.reconcileReportBusiness(id);
                });
            };
            //初始化tab点击的事件
            this.initTabClickEvent = function () {
                //
                $(_reconcile).off("click").on("click", that.reconcileClicked);
                //
                $(_statement).off("click").on("click", that.statementClicked);
                //
                $(_transaction).off("click").on("click", that.transactionClicked);
                //
                $(_cashCodeing).off("click").on("click", that.cashCodingClicked);
                //
                $(_statementDetail).off("click").on("click", that.statementDetailClicked);
            };
            //初始化时间选择事件
            this.initDatePickerEvent = function () {
                //初始化
                $(_dateRangePickerTrigger).mDaterangepicker({
                    onChange: function () { that.Reload("") }
                });
                //
                $(_dateRangePickerArrow).off("click").on("click", function (event) {
                    //
                    $(_dateRangePickerTrigger).trigger("click");
                    //
                    event.stopPropagation();
                });
            };
            //将所有的tab置为未初始化
            this.resetTabInit = function () {
                //reconciel
                $(_reconcilePartial).attr(inited, "0");
                //statement
                $(_statementPartial).attr(inited, "0");
                //transaction
                $(_transactionPartial).attr(inited, "0");
                //cash coding
                $(_cashCodinglPartial).attr(inited, "0");
                //对账单数量置空
                that.ShowReconcileCount(0);
            };
            //将某个tab置为未初始化
            this.resetSingleTabInit = function (tabIndex) {
                var tabPartial = $(".main-buttom div[class$=-partial]");
                $(tabPartial[tabIndex]).attr(inited, "0");
            };
            //获取日期
            this.getUserSelectedDate = function () {
                //
                return $(_dateRangePicker).mDaterangepicker("getRangeDate");
            };
            //get form
            this.getForm = function () {
                //获取日期
                var dates = that.getUserSelectedDate();
                id = id == undefined ? Megi.request("acctid") : id;
                //异步获取
                $("body").mFormGet({
                    url: _getBankInfoUrl,
                    param: {
                        acctid: id,
                        beginDate: dates[0],
                        endDate: dates[1]
                    },
                    callback: function (account) {
                        //帐户余额
                        $(".bank-balance-data-span:visible").html(mMath.toMoneyFormat(account.MBankStatement));
                        //美记帐户余额
                        $(".bank-balance-data-span:visible").html(mMath.toMoneyFormat(account.MMegiBalance));
                    }
                });
            },
            //reload
            this.Reload = function (i, dateRange, callback) {
                //清空初始化状态
                this.resetTabInit();
                //
                var defaultStartDate = new Date(mDate.DateNow.getFullYear(), mDate.DateNow.getMonth(), 1);
                //
                var defaultEndDate = new Date(mDate.DateNow.getFullYear(), mDate.DateNow.getMonth(), 1);
                //
                defaultStartDate = defaultStartDate.addMonths(-5);
                //
                defaultEndDate = defaultEndDate.addMonths(1).addDays(-1);
                //传过来的日期期间或者默认
                dateRange = dateRange != undefined ? [dateRange[0], dateRange[1]] : [defaultStartDate, defaultEndDate];
                //传过来的index
                switch (i) {
                    case "1":
                        //日期为启用日期到今天
                        $(_dateRangePicker).mDaterangepicker("setRangeDate", dateRange);
                        //
                        $(_reconcile).click();
                        break;
                    case "2":
                        //日期当前月
                        $(_dateRangePicker).mDaterangepicker("setRangeDate", dateRange);
                        //
                        $(_statement).click();
                        break;
                    case "3":
                        //日期当前月
                        $(_dateRangePicker).mDaterangepicker("setRangeDate", dateRange);
                        //
                        $(_transaction).click();
                        break;
                    case "4":
                        //日期当前月
                        $(_dateRangePicker).mDaterangepicker("setRangeDate", dateRange);
                        //
                        $(_cashCodeing).click();
                        break;
                    default:
                        //当前tab页点击
                        $("." + _mainHeaderSelectedClass).click();
                }
                //获取账户信息
                that.getForm();
                //回调函数
                if (callback && typeof (callback) == "function") {
                    callback();
                }
            };
            //显示对账单的数量
            this.ShowReconcileCount = function (count) {
                //
                $(_headerReconcileCount).text(count ? count : "0");
            };
            //根据账户类型处理一些逻辑
            this.initAccountType = function () {
                //如果是现金账户
                if (type === "cash") {
                    //如果是cash账户，隐藏掉Reconcile Bank Statement 和 CashCoding
                    $(_reconcile).off("click").remove();
                    //
                    $(_statement).off("click").remove();
                    //
                    $(_cashCodeing).off("click").remove();
                    //显示对账单数量的span
                    $(_headerReconcileCount).remove();
                }
            };
            //提交数据到后台获取银行信息
            //初始化所有的数据,统一入口
            //type表示账户类型，id表示银行账户id
            this.InitAll = function () {
                //初始化时间选择
                that.initDatePickerEvent();
                //初始化tab隐藏显示
                that.initAccountType();
                //初始化事件
                that.initTabClickEvent();
                //初始化银行编辑事件
                that.initAccountManageEvent();
                //计算整个页面的高度，使得头部不滚动
                $("body").css("overflow", "hidden");
                //整体的高度
                $(_main).height($(document).height() - 20);
                //bottom的高度
                $(_mainButtom).height($(_main).height() - $(_mainHeader).height() - $(_mainHeader).offset().top);
                //相当于reload一次
                that.Reload(index);
                if (!isNeedReconcile) {
                    $(_transaction).click();
                }
            };
        };
        //返回
        return BDBankReconcileHome;
    })();
    //有必要就复制到windows
    window.BDBankReconcileHome = BDBankReconcileHome;
})()