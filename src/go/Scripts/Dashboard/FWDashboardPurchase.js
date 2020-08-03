/*
    Dashboard销售
*/
(function () {
    //
    var FWDashboardPurchase = (function () {
        //获取销售信息数据的url
        var getDataUrl = "/FW/FWHome/GetPurchaseDashboardData";
        //销售数据的类型
        var dataType = "'Invoice_Purchase','Invoice_Purchase_Red'";
        //跳转到销售主页
        var purchaseUrl = "/IV/Bill/BillList/";
        //跳转到Draft: 1,WaitingApproval: 2,AwaitingPayment: 3,Paid: 4
        var all, due = 1, draft = 2, approval = 3, payment = 4, paid = 5;
        //
        var _purchseTitle = ".purchse-title";
        //
        var _purchaseNewOrder = ".purchase-new-order";
        //样式
        var _purchaseDiv = ".purchase";
        //样式
        var purchase = "purchase";
        //颜色
        var colors = [['#f96422', '#f3956d', '#7c8490', '#f0c0aa'],
                      ['#f0c0aa', '#f2a07c', '#f38a5b', '#fa7b43', '#f96422']];
        //常量定义
        var FWDashboardPurchase = function (c) {
            //首先获取一个FWDashboardHome对象，供使用
            var home = new FWDashboardHome(purchase);
            //当前的总div
            var current = $(_purchaseDiv);
            //
            var that = this;
            //获取银行信息的数据
            this.getData = function (dateRange, chartType) {
                //遮罩
                current.mask("");
                //发送请求
                mAjax.post(getDataUrl, { dataType: dataType, startDate: dateRange[0], endDate: dateRange[1], chartType: chartType }, function (data) {
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
            //将获取的数据绑定到图表
            this.initChart = function (data, chartType) {
                //返回到home处理
                home.initChart(chartType, data, current, colors);
            };
            //将获取的数据绑定到表格
            this.initTable = function (data) {
                //
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //传到父层
                home.initTable([
                    [data.WaitingApprovalAmount, title, purchaseUrl + approval],
                    [data.WaitingPaymentAmount, title, purchaseUrl + payment],
                    [data.DueAmount, title, purchaseUrl + due],
                    [data.PaidAmount, title, purchaseUrl + paid]
                ]);
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
                    //不可新采购但
                    $(_purchaseNewOrder).remove();
                }
            };
            //
            this.InitAll = function () {
                //处理权限
                that.grantPermission();
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
        return FWDashboardPurchase;
    })();
    //
    window.FWDashboardPurchase = FWDashboardPurchase;
})()