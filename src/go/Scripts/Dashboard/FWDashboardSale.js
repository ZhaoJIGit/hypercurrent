/*
    Dashboard销售
*/
(function () {
    //
    var FWDashboardSale = (function () {
        //获取销售信息数据的url
        var getDataUrl = "/FW/FWHome/GetSaleDashboardData";
        //销售数据的类型
        var dataType = "'Invoice_Sale','Invoice_Sale_Red'";
        //跳转到销售主页
        var saleUrl = "/IV/Invoice/InvoiceList/";
        //
        var _invoiceTitle = ".invoice-title";
        //
        var _invoiceNewOrder = ".invoice-new-order";
        //跳转到
        var all, due = 1, draft = 2, approval = 3, payment = 4, paid = 5;
        //样式
        var _saleDiv = ".sale";
        //样式
        var sale = "sale";
        //颜色
        var colors = [['#7c8490', '#7c8490', '#7c8490', '#7c8490'],
                       ['#c4e9c7', '#8ad68e', '#69bd6c', '#51ae56', '#26a72c']];
        //常量定义
        var FWDashboardSale = function (c) {
            //首先获取一个FWDashboardHome对象，供使用
            var home = new FWDashboardHome(sale);
            //当前的总div
            var current = $(_saleDiv);
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
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //传到父层
                home.initTable([
                    [data.WaitingApprovalAmount, title, saleUrl + approval],
                    [data.WaitingPaymentAmount, title, saleUrl + payment],
                    [data.DueAmount, title, saleUrl + due],
                    [data.PaidAmount, title, saleUrl + paid]
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
                    $(_invoiceNewOrder).remove();
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
        return FWDashboardSale;
    })();
    //
    window.FWDashboardSale = FWDashboardSale;
})()