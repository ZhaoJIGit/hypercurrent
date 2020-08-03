/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPReconcile = /** @class */ (function () {
    function FPReconcile() {
        this.page = 1;
        this.rows = 20;
        this.demoDiv = ".fp-reconcile-demo";
        this.matchText = ".match-text";
        this.icoDiv = ".match-condition .ico";
        this.matchClass = "match-ico";
        this.unmatchClass = "unmatch-ico";
        this.tableRecordDiv = "div.fp-table-record:visible";
        this.fapiaoRecordDiv = "div.fp-fapiao-record:visible";
        this.tableTable = "div.fp-table-record table";
        this.fapiaoTable = "div.fp-fapiao-record table";
        this.functionLayer = ".fp-reconcile-function-layer";
        this.funcitonButton = ".fp-reconcile-function-button";
        this.searchButton = ".fp-reconcile-search";
        this.applyButton = ".fp-reconcile-apply";
        this.reconcilePartialDiv = ".fp-reconcile-partial";
        this.pager = ".fp-reconcile-partial-pager";
        this.bottomDiv = ".fp-reconcile-partial-bottom";
        this.reconcileDivClass = "fp-reconcile-partial-div";
        this.lockDiv = ".fp-reconcile-match-lock";
        this.recordDiv = ".record";
        this.reconileCount = ".header-reconcile-count";
        this.matchCount = ".match-count";
        this.emptyDiv = ".fp-reconcile-empty";
        this.matchLang = HtmlLang.Write(LangModule.Common, "match", "匹配");
        this.unmatchLang = HtmlLang.Write(LangModule.Common, "unmatch", "未匹配");
    }
    /**
     * 初始化事件
     */
    FPReconcile.prototype.init = function () {
        this.home = new FPReconcileHome();
        this.initDom();
        //初始化事件
        this.initEvent();
    };
    /**
     * 获取数据
     */
    FPReconcile.prototype.loadData = function (dates) {
        var _this = this;
        dates = dates || this.home.getPickedDate();
        //获取过滤
        var filter = {
            page: this.page,
            rows: this.rows,
            MStartDate: dates[0],
            MEndDate: dates[1],
            MFapiaoCategory: this.home.getType()
        };
        //获取数据
        this.home.getReconcileList(filter, function (data) {
            _this.showData(data);
        });
    };
    /**
     * 展示数据到面板
     */
    FPReconcile.prototype.showData = function (data) {
        //先去除所有的div
        $("." + this.reconcileDivClass).remove();
        $(this.emptyDiv).hide();
        $(this.reconileCount).text(data.total);
        //获取demo
        var demo = $(this.demoDiv);
        var lastDiv = demo;
        //循环显示数据
        for (var i = 0; i < data.rows.length; i++) {
            var div = demo.clone().removeClass(this.demoDiv.trimStart('.')).addClass(this.reconcileDivClass);
            var match = !!data.rows[i].MFapiaoList && data.rows[i].MFapiaoList.length > 0;
            var record = div.find(this.recordDiv);
            div.find(this.icoDiv).removeClass(this.unmatchClass).removeClass(this.matchClass).addClass(match ? this.matchClass : this.unmatchClass);
            div.find(this.matchText).text(match ? this.matchLang : this.unmatchLang);
            match ? $(this.lockDiv, div).show() : $(this.lockDiv, div).remove();
            div.find(this.matchCount).text((match && data.rows[i].MFapiaoList.length > 1) ? "[" + data.rows[i].MFapiaoList.length + "]" : "");
            var table = $(this.tableTable, div);
            table.find("tr:eq(0)").find("td:eq(1)").text(this.home.GetFullTableNumber(data.rows[i].MTable.MNumber, this.home.getType()));
            var maxWidth = (($(this.reconcilePartialDiv).width() - 20) / 2 - 30) * 0.45 - 10;
            table.find("tr:eq(0)").find("td:eq(3)").find("span").css({
                "max-width": maxWidth + "px",
                "width": (maxWidth - 10) + "px",
                "display": "block"
            });
            table.find("tr:eq(0)").find("td:eq(3)").css({
                "max-width": (maxWidth + 10) + "px",
                "width": (maxWidth + 10) + "px"
            });
            table.find("tr:eq(0)").find("td:eq(3)").find("span").text(data.rows[i].MTable.MContactName);
            table.find("tr:eq(1)").find("td:eq(1)").text(mDate.format(data.rows[i].MTable.MBizDate));
            table.find("tr:eq(1)").find("td:eq(3)").text(mMath.toMoneyFormat(data.rows[i].MTable.MTotalAmount - data.rows[i].MTable.MRTotalAmount - data.rows[i].MTable.MAjustAmount));
            var fapiao = $(this.fapiaoTable, div);
            if (match) {
                fapiao.find("tr:eq(0)").find("td:eq(1)").text(data.rows[i].MFapiaoList[0].MNumber);
                fapiao.find("tr:eq(0)").find("td:eq(3)").find("span").css({
                    "max-width": maxWidth + "px",
                    "width": (maxWidth - 10) + "px"
                });
                fapiao.find("tr:eq(0)").find("td:eq(3)").css({
                    "max-width": (maxWidth + 10) + "px",
                    "width": (maxWidth + 10) + "px"
                });
                fapiao.find("tr:eq(0)").find("td:eq(3)").find("span").text(this.home.getType() == FPEnum.Purchase ? data.rows[i].MFapiaoList[0].MSContactName : data.rows[i].MFapiaoList[0].MPContactName);
                fapiao.find("tr:eq(1)").find("td:eq(1)").text(mDate.format(data.rows[i].MFapiaoList[0].MBizDate));
                fapiao.find("tr:eq(1)").find("td:eq(3)").text(mMath.toMoneyFormat(data.rows[i].MFapiaoList[0].MTotalAmount));
            }
            else {
                fapiao.find("td").html("&nbsp;");
            }
            div.insertAfter(lastDiv).show();
            lastDiv = div;
            $.data(record[0], "data", data.rows[i]);
        }
        this.resize();
        //初始化遮罩层
        this.initFuctionLayer();
        //初始化翻页控件
        this.initPage(data.total);
        data.total === 0 && $(this.emptyDiv).show();
    };
    /**
     * 苹果浏览器下面，宽度计算有问题
     */
    FPReconcile.prototype.resize = function () {
        var divWidth = ($("." + this.reconcileDivClass).width() - 20) / 2;
        $(this.tableRecordDiv).each(function (index, elem) {
            $(elem).width(divWidth);
        });
        $(this.fapiaoRecordDiv).each(function (index, elem) {
            $(elem).width(divWidth);
        });
        $(this.lockDiv + ":visible").each(function (index, elem) {
            $(elem).css({
                'margin-left': (($(elem).parent().width() - 48) / 2) + 'px'
            });
        });
    };
    /**
     * 初始化遮罩层
     */
    FPReconcile.prototype.initFuctionLayer = function () {
        var _this = this;
        $(this.recordDiv).off("mouseenter").on("mouseenter", function (evt) {
            var div = $(evt.srcElement || evt.target).parents(".record");
            var data = $.data(div[0], "data");
            //如果超出最上层，则不需要显示遮罩层
            if (!data || div.offset().top < 125)
                return;
            //如果超出最下层也不要显示遮罩层
            if (!data || div.offset().top > ($(_this.reconcilePartialDiv).height() + $(_this.reconcilePartialDiv).offset().top - 75))
                return;
            $(_this.functionLayer).animate({
                top: div.offset().top + "px",
                width: div.width() - 1 + "px",
            }, 200, "swing").show();
            !!data.MFapiaoList && data.MFapiaoList.length > 0 ? $(_this.applyButton).show() : $(_this.applyButton).hide();
            //确认勾兑
            $(_this.applyButton).off("click").on("click", function () {
                //只取第一条
                data.MFapiaoList = [data.MFapiaoList[0]];
                _this.home.saveReconcile(data, function (data) {
                    if (data.Success) {
                        mDialog.message(HtmlLang.Write(LangModule.Common, "ReconcileSuccessfully", "勾兑成功!"));
                        _this.loadData();
                    }
                    else {
                        mDialog.message(HtmlLang.Write(LangModule.Common, "ReconcileFailed", "勾兑失败!"));
                    }
                });
            });
            //查找，就是打开勾兑
            $(_this.searchButton).off("click").on("click", function () {
                _this.home.showEditReconcile(data.MTable.MItemID, _this.home.getType(), function () { _this.loadData(); });
            });
            //功能层的失去焦点事件
            $(_this.functionLayer).off("mouseleave").on("mouseleave", function (evt) {
                $(_this.functionLayer).hide();
            });
            //如果用户点击任何地方，则需要隐藏
            $(document).on("click", function () {
                $(_this.functionLayer).hide();
            });
        });
    };
    /**
     * 在页面滚动的时候初始化锁的位置
     */
    FPReconcile.prototype.initLockPosition = function () {
        var _this = this;
        //
        $(this.lockDiv).each(function (index, elem) {
            var $lock = $(elem);
            //找到整个div
            var $reconcileDiv = $lock.parent();
            //计算top
            var top = $reconcileDiv.offset().top + ($reconcileDiv.outerHeight() - $lock.height()) / 2 - 30;
            //如果top不在显示范围之内
            (top <= 95) ? $lock.hide() : $lock.show().css({ "top": (top + "px") });
            $(_this.functionLayer).hide();
        });
    };
    /**
     * 初始化面板事件
     */
    FPReconcile.prototype.initEvent = function () {
        var _this = this;
        //页面滚动的时候需要重新定位锁的位置
        $(this.reconcilePartialDiv).scroll(function () {
            _this.initLockPosition();
        });
    };
    /**
     * 初始化元素  高度 宽度 滚动等
     */
    FPReconcile.prototype.initDom = function () {
        $(this.reconcilePartialDiv).height($("body").height() - 155);
        $(this.pager).css({
            top: ($("body").height() - 30) + "px"
        });
    };
    /**
     * 初始化翻页
     */
    FPReconcile.prototype.initPage = function (total) {
        var _this = this;
        //调用easyui组件
        $(this.pager).pagination({
            total: total,
            pageSize: this.rows,
            pageList: this.home.pageList,
            onSelectPage: function (page, size) {
                _this.page = page;
                _this.rows = size;
                _this.loadData();
            }
        });
    };
    return FPReconcile;
}());
//# sourceMappingURL=FPReconcile.js.map