/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPStatement = /** @class */ (function () {
    function FPStatement() {
        this.checkAll = ".fp-record-checkbox-all";
        this.statementPartial = ".fp-statement-partial";
        this.statementTable = ".fp-statement-table";
        this.pager = ".fp-statement-partial-pager";
        this.showDetail = ".fp-import-detail";
        this.btnDeleteFPImportDiv = "#btnDeleteFPImport";
        this.checkBox = ".fp-record-checkbox:visible";
        this.page = 1;
        this.rows = 20;
    }
    /**
     * 初始化事件
     */
    FPStatement.prototype.init = function () {
        this.home = new FPReconcileHome();
        this.initDom();
        this.initEvent();
    };
    /**
     * 获取数据
     */
    FPStatement.prototype.loadData = function (dates) {
        var _this = this;
        /// add by 锦友 20180529 15:54:00
        ///修复切换到发票导入添加时把发票速记的选择显示字段弹出层给移除了
        $(".tooltip-top").hide();
        dates = dates || this.home.getPickedDate();
        //获取过滤
        var filter = {
            page: this.page,
            rows: this.rows,
            MStartDate: dates[0],
            MEndDate: dates[1],
            MFapiaoCategory: this.home.getType()
        };
        this.home.getStatementList(filter, function (data) {
            if (data.rows.length === 0) {
                filter.page = filter.page > 1 ? filter.page - 1 : 1;
                _this.home.getStatementList(filter, function (data) {
                    _this.showData(data);
                });
            }
            else {
                _this.showData(data);
            }
        });
    };
    /**
     * 展示数据到面板
     */
    FPStatement.prototype.showData = function (data) {
        var _this = this;
        $(this.statementTable).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: this.home.getGridWith(),
            height: $(this.statementPartial).height() - 45,
            checkOnSelect: false,
            selectOnCheck: false,
            columns: [[
                    {
                        field: 'MID', title: '<input type="checkbox" class="fp-record-checkbox-all"/>', fixwidth: 30, width: 30, align: 'left',
                        formatter: function (value, row) {
                            return "<div style='text-align:center'><input type='checkbox' class='fp-record-checkbox' mid='" + row.MID + "' " + "/></div>";
                        }
                    },
                    {
                        field: 'MDate', title: HtmlLang.Write(LangModule.FP, "ImportDate", "导入日期"), width: 80, align: 'center',
                        formatter: function (value, rec) {
                            return "<a href='javascript:void(0)' class='fp-import-detail' mid='" + rec.MID + "'>" + mDate.format(value) + "</a>";
                        }
                    },
                    {
                        field: 'MStartDate', title: HtmlLang.Write(LangModule.FP, "StartDate", "开始日期"), width: 80, align: 'center', formatter: function (value) {
                            return mDate.format(value);
                        }
                    },
                    {
                        field: 'MEndDate', title: HtmlLang.Write(LangModule.FP, "EndDate", "结束日期"), width: 80, align: 'center', formatter: function (value) {
                            return mDate.format(value);
                        }
                    },
                    {
                        field: 'MCount', title: HtmlLang.Write(LangModule.FP, "Quantity", "数量"), width: 80, align: 'right'
                    },
                    {
                        field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 80, align: 'right', formatter: function (value) {
                            return mMath.toMoneyFormat(value);
                        }
                    },
                    {
                        field: 'MOperator', title: HtmlLang.Write(LangModule.FP, "Operator", "操作员"), width: 80, align: 'center'
                    },
                    {
                        field: 'MFileName', title: HtmlLang.Write(LangModule.FP, "FileName", "文件名"), width: 150, align: 'center'
                    },
                    {
                        field: 'MSource', title: HtmlLang.Write(LangModule.FP, "Source", "来源"), width: 80, align: 'center', formatter: function (source) {
                            return _this.home.getFapiaoSourceName(source);
                        }
                    }
                ]],
            onLoadSuccess: function () {
                //初始化分页控件
                _this.initPage(data.total);
                $(_this.statementTable).datagrid("resize");
                _this.initGridEvent();
            }
        });
    };
    /**
     * 初始化面板事件
     */
    FPStatement.prototype.initEvent = function () {
        var _this = this;
        /*
        删除发票导入清单
        */
        $(this.btnDeleteFPImportDiv).off("click").on("click", function (evt) {
            var boxes = $(_this.checkBox + ":checked");
            var ids = [];
            boxes.each(function (index, elem) {
                ids.push($(elem).attr("mid"));
            });
            ids = ids.distinct();
            if (ids.length == 0) {
                return mDialog.message(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "请选择一个或多个项目！"));
            }
            //弹出删除确认提醒
            mDialog.confirm(HtmlLang.Write(LangModule.FP, "AreYourSureDeleteFPImportData", "确认删除选中的发票清单吗？"), function () {
                var filter = {
                    IdList: ids
                };
                _this.home.deleteFPImportByImportIds(filter, function (data) {
                    if (data.Success) {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                        _this.loadData();
                    }
                    else {
                        if (data.Message != null && data.Message != "") {
                            mDialog.message(data.Message);
                        }
                        else {
                            mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                        }
                    }
                });
            });
        });
    };
    /**
    * 初始化面板事件
    */
    FPStatement.prototype.initGridEvent = function () {
        var _this = this;
        //全选
        $(this.checkAll).off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            if (_this.home.isSmartVersion()) {
                if ($elem.is(":checked")) {
                    $(_this.checkBox + "[rstatus!='1']:not([disabled])").attr("checked", "checked");
                }
                else {
                    $(_this.checkBox + "[rstatus!='1']:not([disabled])").removeAttr("checked");
                }
            }
            else {
                if ($elem.is(":checked")) {
                    $(_this.checkBox + "[cstatus!='1']:not([disabled])").attr("checked", "checked");
                }
                else {
                    $(_this.checkBox + "[cstatus!='1']:not([disabled])").removeAttr("checked");
                }
            }
        });
        $(this.checkBox).off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            if (_this.home.isSmartVersion()) {
                if ($elem.is(":checked")) {
                    if ($(_this.checkBox + "[rstatus!='1']:not([disabled]):checked").length == $(_this.checkBox + "[rstatus!='1']:not([disabled])").length) {
                        $(_this.checkAll).attr("checked", "checked");
                    }
                }
                else {
                    $(_this.checkAll).removeAttr("checked");
                }
            }
            else {
                if ($elem.is(":checked")) {
                    if ($(_this.checkBox + "[cstatus!='1']:not([disabled]):checked").length == $(_this.checkBox + "[cstatus!='1']:not([disabled])").length) {
                        $(_this.checkAll).attr("checked", "checked");
                    }
                }
                else {
                    $(_this.checkAll).removeAttr("checked");
                }
            }
        });
        /*
        查看发票清单详情
        */
        $(this.showDetail).off("click").on("click", function (evt) {
            var mid = $(evt.target || evt.srcElement).attr("mid");
            mDialog.show({
                mTitle: HtmlLang.Write(LangModule.FP, "FapiaoImportDetail", "发票导入详情"),
                mDrag: "mBoxTitle",
                mShowbg: true,
                mContent: "iframe:" + _this.home.showTransactionDetailUrl + "?MImportID=" + mid + "&MFapiaoCategory=" + _this.home.getType() + "&OrgVersion=" + _this.home.orgVersion(),
                mCloseCallback: function () {
                    $.isFunction(_this.loadData()) && _this.loadData();
                }
            });
        });
    };
    /**
     * 初始化元素  高度 宽度 滚动等
     */
    FPStatement.prototype.initDom = function () {
        $(this.statementPartial).height($("body").height() - 155);
        $(this.pager).css({
            top: ($("body").height() - 30) + "px"
        });
    };
    /**
    * 初始化分页
    */
    FPStatement.prototype.initPage = function (total) {
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
    return FPStatement;
}());
//# sourceMappingURL=FPStatement.js.map