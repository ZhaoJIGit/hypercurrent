/*
    Dashboard 银行帐号信息
*/
(function () {
    //
    var FWDashboardBank = (function () {
        //常量定义
        //获取银行信息数据的url
        var getDataUrl = "/FW/FWHome/GetBankDashbardData";
        //获取银行账单信息

        var bank = "bank";
        //当前bank的div
        var _bankDiv = ".bank";
        //管理按钮
        var _bankManage = ".bank-manage";
        //银行标题，点击可进入相应角色
        var _bankTitle = ".bank-title";
        //管理子功能
        var _bankManageSubDiv = ".bank-manage-sub-div";
        //
        var _reconcileTransaction = "a.reconcile-transaction";
        //
        var _reconcielCount = "a.reconcile-count";
        //
        var _showBankAccountInfo = "a.bank-account-info";
        //
        var showBankData = [];
        //
        //不同形状的图，定义的不同颜色
        var colors = [
             ["#99ccf6", "#e3eaed", "#ffffff", "#f4f9fb", "#f4f9fb", "#145d79", "#ffffff", "#449be0", "#ffffff", "#ffffff"],
            ["#a5c8d6", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#0579a5", "#ffffff", "#ffffff"],
            ["#a5c8d6", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#0579a5", "#ffffff", "#ffffff"],
            ["#a5c8d6", "#0579a5", "#ffffff", "#996633", "#9933ff", "#145d79", "#ffffff", "#0579a5", "#ffffff", "#ffffff"]];
        //银行账户信息展示对应的表格
        var _bankInfoTable = ".bank-info-table";
        //定义类
        var FWDashboardBank = function (c, r, v) {

            var isSmart = v == "1";
            //首先获取一个FWDashboardHome对象，供使用
            var home = new FWDashboardHome(bank);
            //当前的总div
            var current = $(_bankDiv);
            //
            var that = this;
            //获取银行信息的数据
            this.getData = function (dateRange, chartType) {
                //先遮罩整个div
                current.mask("");
                //异步获取数据,将用户选择的日期段以及图表类型传到后台，便于过滤
                mAjax.post(getDataUrl, { startDate: dateRange[0], endDate: dateRange[1], chartType: chartType }, function (data) {
                    //初始化右侧表格
                    that.initTable(data, chartType);
                    //取消遮罩
                    current.unmask();
                }, function () {
                    //取消遮罩
                    current.unmask();
                });
            };
            //将获取的数据绑定到图表
            this.initChart = function (data, chartType, title) {
                //调用统一接口生成相应图像
                home.initChart(chartType, data, current, colors[chartType], title);
            };
            //初始化银行子功能
            this.initBankManageEvent = function () {
                //账户设置
                $(_bankManage).mTip({
                    target: $(_bankManageSubDiv),
                    width: 120,
                    parent: $(_bankManageSubDiv).parent()
                });
                //初始化事件 
                $("#aNewAccount").off("click").on("click", function () {
                    new BDBankHome().editBankAccount(BankTypeEnum[1].type, '', $.mTab.addOrUpdate,'Mission_Control');
                });
                $("#aTransfer").off("click").on("click", function () {
                    new BDBankHome().transferBusiness('');
                });
                $("#aBankRules").off("click").on("click", function () {
                    new BDBankHome().bankRulesBusiness('');
                });
            };
            //初始化表格里面的额事件
            this.bindTableEvent = function () {
                //对账单的url
                var reconcileUrl = new BDBankHome().reconcileUrl;
                //给仪表盘名称添加
                $(_showBankAccountInfo).off("click").on("click", function (index) {
                    //
                    var row = showBankData.where("x.MItemID =='" + $(this).attr("mitemid") + "'")[0];
                    //
                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.Bank, "bankAccount", "银行账户"), "/BD/BDBank/BDBankHome?acctId=" + row.MItemID, true);
                });
                //
                $(_reconcileTransaction).off("click").on("click", function (index) {
                    //
                    var row = showBankData.where("x.MItemID =='" + $(this).attr("mitemid") + "'")[0];
                    //
                    $.mTab.addOrUpdate(row.MBankName, reconcileUrl + "?acctid=" + row.MItemID + "&type=" + row.MBankAccountType + "&index=3&bankType=" + row.MBankTypeID, true);
                });
                //
                $(_reconcielCount).off("click").on("click", function (index) {
                    //
                    var row = showBankData.where("x.MItemID =='" + $(this).attr("mitemid") + "'")[0];
                    //
                    $.mTab.addOrUpdate(row.MBankName, reconcileUrl + "?acctid=" + row.MItemID + "&type=" + row.MBankAccountType + "&index=" + (row.MReconcileCount == 0 ? 3 : 1) + "&bankType=" + row.MBankTypeID, true)
                });
            };
            //将获取的数据绑定到表格
            this.initTable = function (data, chartType) {
                //手动创建一个table，不是用easyui的datagrid
                var title = HtmlLang.Write(LangModule.Bank, "AwaitReconcile", "Reconcile");
                //标题
                var transactionTitle = HtmlLang.Write(LangModule.Bank, "AccountTransaction", "Account Transaction");
                //清空内部的内容
                $(_bankInfoTable).find("tbody").empty();
                //清空内部的内容
                $(_bankInfoTable).parent().css({ "overflow-y": "auto" });
                //标题
                var name = HtmlLang.Write(LangModule.Common, "Name", "Name");
                //标题
                var statementBalance = HtmlLang.Write(LangModule.Bank, "statementbalance", "Statement Balance");
                //表格标题
                var balanceInMegi = HtmlLang.Write(LangModule.Bank, "BalanceInMegi", "Balance in Hypercurrent");
                //绘制图形
                that.loadSuccess(data, chartType);
                //去掉第一行以及第五行以后的
                showBankData = [];
                //
                for (var i = 1; i < data.length && showBankData.length < 4; i++) {
                    //如果展示在首页
                    data[i].MIsShowInHome ? showBankData.push(data[i]) : "";
                }
                //创建表格
                $(_bankInfoTable).datagrid({
                    width: ((top.$("body").width() - 63 - 100 - 10 - 2) * 0.48 - 20 - 2) * 0.95,
                    //
                    onLoadSuccess: function () {
                        //禁用tr的hover事件
                        $("tr", ".datagrid-btable").off("hover").off("mouseover");
                        //
                        $('.datagrid-cell-c1-MBankName', ".datagrid-btable").each(function () {
                            //
                            $(this).tooltip({
                                content: mText.encode($("a", this).text())
                            });
                            $(this).addClass("m-ellipsis");
                        });
                        //
                        that.bindTableEvent();
                    },
                    //数据
                    data: showBankData,
                    //列
                    columns: [
                        [
                        {
                            field: 'MBankName', title: name, align: "left", width: 135,
                            formatter: function (value, row, index) {
                                return "<a href='javascript:void(0);' class='bank-account-info' mitemid = '" + row.MItemID + "'>"
                                + mText.encode(value) + "</a>";
                            }
                        },
                        {
                            field: 'MBankStatement', title: statementBalance, align: "right", width: 115, hidden: isSmart,
                            formatter: function (value, row, index) {
                                //账户类型
                                return "<span style='font-size:12px' >" + ((row.MBankAccountType == BankTypeEnum[3].type) ? "0.00" : mMath.toMoneyFormat(value)) + "</span>";
                            }
                        },
                        {
                            field: 'MMegiBalance', title: balanceInMegi, align: "right", width: 135,
                            formatter: function (value, row, index) {
                                return "<a href='javascript:void(0)' class='reconcile-btn reconcile-transaction' mitemid='" + row.MItemID + "'>" + mMath.toMoneyFormat(value) + "[" + row.MCyID + "]</a>";
                            }
                        },
                        {
                            field: 'MReconcileCount', title: '', align: "left", width: 120, hidden: isSmart,
                            formatter: function (value, row, index) {
                                //账户类型
                                return (row.MIsNeedReconcile == false || r !== 'True') ? "" : "<a href='javascript:void(0)' class='reconcile-btn reconcile-count' mitemid='" + row.MItemID + "'>" + (title + "[" + row.MReconcileCount) + "]" + "</a>";
                            }
                        }
                        ],
                    ]
                });
            };
            //加载表格完成后的操作
            this.loadSuccess = function (data, chartType) {
                //标题
                var allTile = HtmlLang.Write(LangModule.Bank, "AllCount", "All Account");
                //如果有行数
                if (data && data.length > 0) {
                    //加载第一行对应的图表
                    that.initChart(data[0].MBankChartInfo, chartType, allTile);
                }
                else {
                    //画一张空图
                    that.initChart("", chartType, "");
                }
                //有数据以后再展示按钮
                that.locateMange();
            };
            //
            //计算操作按钮的位置
            this.locateMange = function () {
                //获取图表的宽度
                var width = home.getChartWidth();
                //
                $(_bankManage).css({ "margin-left": (width - 110) + "px" });
                //设置按钮功能
                that.initBankManageEvent();
            };
            //初始化右侧的账户管理功能
            this.initManage = function () {

            };
            //获取当前选择的时间段
            this.getDateRange = function () {
                //
            };
            //reload函数
            this.reload = function () {
                //获取用户选择的日期类型
                var dateRange = home.getDateRange(current);
                //获取用户选择的图形
                var chartType = home.getChartType(current);
                //获取数据
                that.getData(dateRange, chartType);
            };
            //处理权限问题
            this.grantPermission = function () {
                //如果没有编辑权限
                if (c !== "True") {
                    //不可新建账号
                    $("#aNewAccount").remove();
                    //不可转账
                    $("#aTransfer").remove();
                }
            };
            //
            this.InitAll = function () {
                //处理权限问题
                that.grantPermission();
                //首先清除不属于自己的内容
                home.clearSiblings(current);
                //隐藏
                $(_bankManageSubDiv).hide();
                //
                that.reload();
                //调用home注册事件
                home.initDateSelectEvent(current, that.reload);
                //注册
                home.initChartSelectEvent(current, that.reload);
            };
        };
        //
        return FWDashboardBank;
    })();
    //
    window.FWDashboardBank = FWDashboardBank;
})()