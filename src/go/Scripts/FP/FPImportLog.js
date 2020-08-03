/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPImportLog = /** @class */ (function () {
    function FPImportLog() {
        this.importLogPartial = ".fp-importlog-partial";
        this.importLogTable = ".fp-importlog-table";
        this.pager = ".fp-importlog-partial-pager";
        this.btnRefresh = "#btnRefreshLog";
        this.lastTime = ".fp-importlog-value";
        this.page = 1;
        this.rows = 20;
    }
    /**
     * 初始化事件
     */
    FPImportLog.prototype.init = function () {
        this.home = new FPReconcileHome();
        this.initDom();
        this.initEvent();
    };
    /**
     * 获取数据
     */
    FPImportLog.prototype.loadData = function (dates) {
        var _this = this;
        /// add by 锦友 20180529 15:54:00
        ///修复切换到发票导入添加时把发票速记的选择显示字段弹出层给移除了
        $(".tooltip-top").hide();
        dates = dates || this.home.getPickedDate();
        //获取过滤
        var filter = {
            MType: $("#typeInput").val(),
            page: this.page,
            rows: this.rows,
            MStartDate: dates[0],
            MEndDate: dates[1]
        };
        this.home.getFapiaoLogList(filter, function (data) {
            _this.showData(data);
        });
    };
    /**
     * 展示数据到面板
     */
    FPImportLog.prototype.showData = function (data) {
        var _this = this;
        var date = new Date(Date.parse(data.obj));
        //最后更新时间显示
        $(this.lastTime).text(date.getFullYear() <= 1901 ? "0000-00-00 00:00:00" : mDate.formatDateTime(data.obj));
        $(this.importLogTable).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: this.home.getGridWith(),
            height: $(this.importLogPartial).height() - 45,
            columns: [[
                    {
                        field: 'MDate', title: HtmlLang.Write(LangModule.FP, "CollectTime", "获取时间"), width: 150, align: 'center', formatter: function (value) {
                            return mDate.formatDateTime(value);
                        }
                    },
                    {
                        field: 'MQty', title: HtmlLang.Write(LangModule.FP, "FapiaoQuantity", "发票数量"), width: 150, align: 'right'
                    },
                    {
                        field: 'MAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 150, align: 'right', formatter: function (value) {
                            return mMath.toMoneyFormat(value);
                        }
                    },
                    {
                        field: 'MMessage', title: HtmlLang.Write(LangModule.FP, "Remark", "备注"), width: 220, align: 'left', formatter: function (value, record) {
                            return value;
                        }
                    },
                    {
                        field: 'MStatus', title: HtmlLang.Write(LangModule.FP, "Status", "状态"), width: 80, align: 'center', formatter: function (value) {
                            return value == 1 ? "<span style='color:green;'>" + HtmlLang.Write(LangModule.FP, "FPCollectSuccess", "获取成功") + "</span>"
                                : "<span style='color:red;'>" + HtmlLang.Write(LangModule.FP, "FPCollectFail", "获取失败") + "</span>";
                        }
                    }
                ]],
            onLoadSuccess: function () {
                //初始化分页控件
                _this.initPage(data.total);
                $(_this.importLogTable).datagrid("resize");
            }
        });
    };
    /**
     * 初始化面板事件
     */
    FPImportLog.prototype.initEvent = function () {
        var _this = this;
        $(this.btnRefresh).off("click").on("click", function (evt) {
            _this.loadData();
        });
    };
    /**
     * 初始化元素  高度 宽度 滚动等
     */
    FPImportLog.prototype.initDom = function () {
        $(this.importLogPartial).height($("body").height() - 155);
        $(this.pager).css({
            top: ($("body").height() - 30) + "px"
        });
    };
    /**
    * 初始化分页
    */
    FPImportLog.prototype.initPage = function (total) {
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
    return FPImportLog;
}());
//# sourceMappingURL=FPImportLog.js.map