/*
    Dashboard 费用帐号信息
*/
(function () {
    //
    var FWDashboardExpense = (function () {
        //常量定义
        //获取银行信息数据的url
        var getDataUrl = "/FW/FWHome/GetExpenseDashboardData";
        //
        var expense = "expense";
        //跳转到费用报销报表主页
        var expenseUrl = "/IV/Expense/ExpenseList/";

        var reportUrl = "Report/Report2/38";

        var all = 0, draft = 1, approval = 2, payment = 3, paid = 4;

        //
        var _expenseDiv = ".expense";
        //颜色
        var colors = [['#f96422', '#f3956d', '#7c8490', '#f0c0aa'],
                      ['#fab743', '#f6cf5f', '#fbdc82', '#fbe299', '#fbe8b0']];
        //定义类
        var FWDashboardExpense = function (c) {
            //首先获取一个FWDashboardHome对象，供使用
            var home = new FWDashboardHome(expense);
            //
            var that = this;
            //当前的总div
            var current = $(_expenseDiv);
            //获取银行信息的数据
            this.getData = function (dateRange, chartType) {
                //遮罩
                current.mask("");
                //发送请求
                mAjax.post(getDataUrl, { Type: "1", startDate: dateRange[0], endDate: dateRange[1], chartType: chartType }, function (data) {
                    //展示图形
                    that.initChart($.parseJSON(data.ChartData), chartType);
                    //显示数据
                    that.initTable(data.TableData);
                    //取消遮罩
                    current.unmask();
                }, function () {
                    //取消遮罩
                    current.unmask();
                });
            };
            //将获取的数据绑定到图表A
            this.initChart = function (data, chartType) {
                //返回到home处理
                home.initChart(chartType, data, current, colors, "", reportUrl);
            };
            //将获取的数据绑定到表格
            this.initTable = function (data) {
                //
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaim", "Expense Claims");
                //传到父层
                home.initTable([
                    [data.AllAmount, title, expenseUrl + all],
                    [data.WaitingApprovalAmount, title, expenseUrl + approval],
                    [data.WaitingPaymentAmount, title, expenseUrl + payment],
                    [data.PaidAmount, title, expenseUrl + paid], ]);
            };
            //初始化右侧的账户管理功能
            this.initAccountManage = function () {

            };
            //初始化最近一周
            //reload函数
            this.reload = function () {
                //获取当前日期
                var dateRange = home.getDateRange(current);
                //获取图标类型
                var chartType = home.getChartType(current);
                //
                that.getData(dateRange, chartType);
            };
            //处理权限问题
            this.grantPermission = function () {
                //如果没有编辑权限
                if (c !== "True") {
                    //
                }
            };
            //
            this.InitAll = function () {
                //首先清除不属于自己的内容
                home.clearSiblings(current);
                //
                that.reload();
                //调用home注册事件
                home.initDateSelectEvent(current, that.reload);
                //注册
                home.initChartSelectEvent(current, that.reload);
            };
        };
        //
        return FWDashboardExpense;
    })();
    //
    window.FWDashboardExpense = FWDashboardExpense;
})()