

(function () {

    var FPFapiao = (function () {

        var FPFapiao = function (index, invoiceType) {

            //tab
            var tab = ".fp-main-tabs";
            //
            var fapiaoButton = ".fp-table-btn";
            //
            var invoiceButton = ".fp-invoice-button";
            //
            var fapiaoDiv = ".fp-fapiao-div";
            //
            var invoiceDiv = ".fp-invoice-div";
            //
            var allCountDiv = ".fp-all-count";
            //
            var notIssedCountDiv = ".fp-notissued-count";
            //
            var partlyIssuedDiv = ".fp-partlyissued-count";
            //
            var allIssuedDiv = ".fp-allissued-count";
            //定义各个tab对应的index
            var home = 0, all = "", notIssued = 0, partlyIssued = 1, allIssued = 2;
            //
            var item = HtmlLang.Write(LangModule.FP, "Item", "项");
            //
            var items = HtmlLang.Write(LangModule.FP, "Items", "项");
            //
            var tableBaseInfo = {};

            var tabInited = false;
            //
            var that = this;
            //获取发票的基本信息  全部多少张  未开票多少张  部分开票多少张 完全开票多少张
            var getTableHomDataUrl = "/FP/FPHome/GetTableHomeData";
            //
            var deleteTableUrl = "/FP/FPHome/DeleteTable";
            //初始化Tab
            this.initTab = function () {

                if (tabInited) return;
                //初始化
                $(tab).tabsExtend({
                    //默认的显示标签
                    initTabIndex: index,
                    //选择标签函数
                    onSelect: function (index) {
                        //
                        that.showTab(index);
                    }
                });

                tabInited = true;
            };
            //显示tab
            this.showTab = function (index) {

                index = index == undefined ? (+($(".fp-main-tabs li.current").attr("index"))) : index;
                //加上class
                $("ul.tab-links").find("li:eq(" + index + ")").addClass("current").siblings().removeClass("current");
                //当前需要显示的内容
                var currentPartial = $(".fp-partial-" + index);
                //根据index的不同，控制partial的显示
                currentPartial.show().siblings().hide();
                //初始化
                switch (parseInt(index || 0)) {
                    //先控制显示
                    case 0:
                        break;
                    case 1:
                        //
                        new FPTableList(currentPartial, all, invoiceType).init();
                        break;
                    case 2:
                        //
                        new FPTableList(currentPartial, notIssued, invoiceType).init();
                        break;
                    case 3:
                        //
                        new FPTableList(currentPartial, partlyIssued, invoiceType).init();
                        break;

                    case 4:
                        //
                        new FPTableList(currentPartial, allIssued, invoiceType).init();
                        break;
                }
            };
            //获取模板的数量
            this.getTableBaseInfo = function () {
                //
                mAjax.post(getModuleInfoUrl, {}, function (data) {
                    //
                    for (var i = 0; i < data.length ; i++) {
                        //
                        that.initTabTitle(i, data[i] > 1 ? (data[i] + " " + items) : (data[i] + " " + item));
                    }
                });
            };
            //获取科目数据
            this.getTabeBaseInfoData = function (func) {
                //异步获取
                mAjax.post(getTableBaseInfoUrl, {}, function (data) {
                    //
                    tableBaseInfo = data;
                    //
                    tableBaseInfo && $.isFunction(func) && func(tableBaseInfo);
                });
            };
            //修改tab的名字
            this.initTabTitle = function (index, title) {
                //
                $(".fp-main-tabs li:eq(" + index + ") .title").text(title);
            };
            //初始化
            this.init = function () {
                //初始化Tab
                that.initTab();
                //
                that.showTab();
                //更新模板数
                that.updateTableHomeData();
            };
            //开票
            this.issueFapiao = function () {

            };

            //更新表头的开票和未开票的数量
            this.updateTableHomeData = function () {
                //
                mAjax.post(getTableHomDataUrl, { invoiceType: invoiceType }, function (data) {
                    //
                    that.showTableHomeData(data);
                });
            };
            //显示主页数据
            this.showTableHomeData = function (data) {
                //
                var notIssedCount = data.where("x.MName =='" + notIssued + "'")[0].MValue;
                //
                var partlyIssuedCount = data.where("x.MName =='" + partlyIssued + "'")[0].MValue;
                //
                var allIssedCount = data.where("x.MName =='" + allIssued + "'")[0].MValue;
                //
                var totalCount = (+partlyIssuedCount) + (+allIssedCount) + (+notIssedCount);
                //
                $(allCountDiv).text("(" + totalCount + (totalCount > 1 ? items : item) + ")");
                //
                $(notIssedCountDiv).text("(" + notIssedCount + (notIssedCount > 1 ? items : item) + ")");
                //
                $(partlyIssuedDiv).text("(" + partlyIssuedCount + (partlyIssuedCount > 1 ? items : item) + ")");
                //
                $(allIssuedDiv).text("(" + allIssedCount + (allIssedCount > 1 ? items : item) + ")");
            }
            //删除开票单
            this.deleteTable = function (itemIDs, func) {
                //必须有勾选在做删除
                if (itemIDs.length > 0) {
                    //先提醒用户是否确定删除
                    mDialog.confirm(LangKey.AreYouSureToDelete, function () {
                        //删除
                        mAjax.submit(deleteTableUrl, { ids: itemIDs }, function (data) {
                            //提醒用户删除成功
                            mDialog.message(LangKey.DeleteSuccessfully);
                            //整个页面更新
                            $.isFunction(func) && func(data);
                        });
                    });
                }
            };

            //编辑开票单
            this.editTable = function (id) {

            };

        };

        //返回
        return FPFapiao;
    })();
    //
    window.FPFapiao = FPFapiao;
})()