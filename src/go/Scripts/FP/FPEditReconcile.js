/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPEditReconcile = /** @class */ (function () {
    function FPEditReconcile() {
        this.detailTable = ".fp-rc-table";
        this.showAllCbx = ".fp-rc-showall";
        this.clearBtn = ".fp-rc-clear";
        this.amountFilterInput = ".fp-rc-amount-filter";
        this.companyFilterInput = ".fp-rc-company-filter";
        this.btnSearch = "#btnSearch";
        this.sourceDiv = ".fp-rc-source";
        this.sourceTable = ".fp-rc-source-table";
        this.destDiv = ".fp-rc-dest";
        this.destTable = ".fp-rc-dest-table";
        this.adjustInput = ".fp-rc-adjust";
        this.totalDiv = ".fp-rc-total";
        this.diffDiv = ".fp-rc-diff";
        this.reconcileBtn = "#aReconcile";
        this.fapiaoCheckbox = ".fp-record-checkbox";
        this.equalClass = "fp-rc-equal";
        this.notEqualClass = "fp-rc-notequal";
        this.tableDetail = ".fp-rc-table-detail";
    }
    /**
     * 初始化事件
     */
    FPEditReconcile.prototype.init = function (tableID, type) {
        this.srcFapiaos = [];
        this.destFapiaos = [];
        this.difference = 0;
        this.home = new FPReconcileHome();
        this.tableID = tableID;
        this.type = type;
        this.initDom();
        this.loadData(false);
        this.initEvent();
    };
    /**
     * 获取数据
     */
    FPEditReconcile.prototype.loadData = function (onlyFapiao) {
        var _this = this;
        this.home.getReconcileList(this.getQueryFilter(onlyFapiao), function (data) {
            _this.showData(data, onlyFapiao);
        });
    };
    /**
     * 获取过滤条件
     */
    FPEditReconcile.prototype.getQueryFilter = function (onlyFapiao) {
        var amount = $(this.amountFilterInput).val();
        var filter = {
            MTableID: this.tableID,
            MShowAll: $(this.showAllCbx).is(":checked"),
            MKeyword: $(this.companyFilterInput).val(),
            MTotalAmount: amount.toNullableNumber(),
            MFindFapiao: true,
            MFapiaoCategory: this.type,
            MOnlyFapiao: onlyFapiao
        };
        return filter;
    };
    /**
     * 获取当前页面的勾兑模型
     */
    FPEditReconcile.prototype.getReconcileModel = function () {
        var model = {
            MTable: {
                MItemID: this.tableID,
                MAjustAmount: $(this.adjustInput).numberbox("getValue"),
            },
            MFapiaoList: this.destFapiaos
        };
        return model;
    };
    /**
     * 保存勾兑模板
     */
    FPEditReconcile.prototype.saveReconcileModel = function () {
        //如果没有任何发票，则提示选择要勾兑的发票
        if (!this.destFapiaos || this.destFapiaos.length == 0) {
            return mDialog.alert(HtmlLang.Write(LangModule.FP, "PleaseSelectFapiao2Reconicle", "请选择要勾兑的发票到选中区!"));
        }
        //如果未开票金额<0，则表示多勾兑了
        if (this.difference < 0) {
            return mDialog.alert(HtmlLang.Write(LangModule.FP, "ReconcileAmountCannotLargerThanTableAmount", "勾兑金额不可大于开票单金额!"));
        }
        var model = this.getReconcileModel();
        this.home.saveReconcile(model, function (result) {
            if (result.Success) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "ReconcileSuccessfully", "勾兑成功!"));
                //1s后关闭页面
                setInterval(function () { mDialog.close(); }, 1000);
            }
            else {
                mDialog.message(HtmlLang.Write(LangModule.FP, "ReconcileFailed", "勾兑失败!"));
            }
        });
    };
    /**
     * 计算差额以及合计金额
     */
    FPEditReconcile.prototype.calculateAmount = function (adjust) {
        var tableAmount = !this.table ? 0 : this.table.MTotalAmount;
        var reconcileAmount = !this.table ? 0 : this.destFapiaos.sum("MTotalAmount");
        var adjustAmount = +(adjust != undefined ? adjust : $(this.adjustInput).numberbox("getValue"));
        this.difference = tableAmount.sub(adjustAmount).sub(reconcileAmount);
        $(this.diffDiv).text(mMath.toMoneyFormat(this.difference));
        if (this.difference > 0) {
            $(this.diffDiv).removeClass(this.equalClass).addClass(this.notEqualClass);
        }
        else {
            $(this.diffDiv).removeClass(this.notEqualClass).addClass(this.equalClass);
        }
        $(this.totalDiv).text(mMath.toMoneyFormat(reconcileAmount + adjustAmount));
    };
    /**
     * 处理调整输入框是否可以输入
     */
    FPEditReconcile.prototype.handleAdjustAmountInput = function () {
        var tableAmount = this.table.MTotalAmount;
        var reconcileAmount = this.destFapiaos.sum("MTotalAmount");
        //如果表格金额=开票单金额
        if (tableAmount == reconcileAmount) {
            $(this.adjustInput).numberbox("setValue", 0);
            $(this.adjustInput).numberbox("disable");
        }
        else {
            $(this.adjustInput).numberbox("enable");
        }
    };
    /**
     * 展示数据到面板
     */
    FPEditReconcile.prototype.showData = function (data, onlyFapiao) {
        this.reconcileModel = data.rows && data.rows.length > 0 ? data.rows[0] : null;
        this.table = this.reconcileModel ? this.reconcileModel.MTable : null;
        this.srcFapiaos = this.reconcileModel && this.reconcileModel.MFapiaoList ? this.reconcileModel.MFapiaoList : [];
        //如果不是搜索发票，则需要加载开票单信息
        if (onlyFapiao !== true) {
            this.showTableData(this.table);
        }
        //被选区需要根据发票号和发票日期进行排序
        this.srcFapiaos = this.srcFapiaos.sort(function (a, b) {
            return (a.MBizDate < b.MBizDate || (a.MBizDate == b.MBizDate && a.MNumber < b.MNumber)) ? 1 : -1;
        });
        this.showSourceData();
        if (!onlyFapiao) {
            this.destFapiaos = this.reconcileModel && this.reconcileModel.MReconciledFapiaoList ? this.reconcileModel.MReconciledFapiaoList : [];
            //被选区需要根据发票号和发票日期进行排序
            this.destFapiaos = this.destFapiaos.sort(function (a, b) {
                return (a.MBizDate < b.MBizDate || (a.MBizDate == b.MBizDate && a.MNumber < b.MNumber)) ? 1 : -1;
            });
            this.showDestData();
        }
        this.calculateAmount();
    };
    /**
     * 显示开票单信息
     * @param table
     */
    FPEditReconcile.prototype.showTableData = function (table) {
        $(this.tableDetail).find("td:eq(1)").text(this.home.GetFullTableNumber(table.MNumber, this.type));
        $(this.tableDetail).find("td:eq(3)").text(mDate.format(table.MBizDate));
        $(this.tableDetail).find("td:eq(5)").text(table.MContactName + "(" + (table.MContactTaxCode || "--") + ")");
        $(this.tableDetail).find("td:eq(7)").text(mMath.toMoneyFormat(table.MTotalAmount));
        table.MAjustAmount != 0 && $(this.adjustInput).numberbox("setValue", table.MAjustAmount);
    };
    /**
     * 显示到备选表格
     * @param fapiaos
     */
    FPEditReconcile.prototype.showSourceData = function (fapiaos) {
        var _this = this;
        var src = _.clone(fapiaos || this.srcFapiaos || []);
        $(this.sourceTable).datagrid({
            data: (!src || src.length == 0) ? [] : src,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $(this.sourceDiv).width(),
            height: $(this.sourceDiv).height(),
            columns: [[
                    {
                        field: 'MID', title: '', width: 20, align: 'left', formatter: function (value, record, index) {
                            return "<div style='text-align:center'><input type='checkbox' target='1' class='fp-record-checkbox' mid='" + record.MID + "' index='" + index + "'/></div>";
                        }
                    },
                    {
                        field: 'MNumber', title: HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号"), width: 100, align: 'center'
                    },
                    {
                        field: 'MBizDate', title: HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期"), width: 100, align: 'center', formatter: function (value) {
                            return mDate.format(value);
                        }
                    },
                    {
                        field: 'MContactName', title: HtmlLang.Write(LangModule.FP, "Company", "公司"), width: 300, align: 'left', formatter: function (value, record) {
                            return _this.type == FPEnum.Sales ? record.MPContactName : record.MSContactName;
                        }
                    },
                    {
                        field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 100, align: 'right', formatter: function (value) {
                            return mMath.toMoneyFormat(value);
                        }
                    }
                ]],
            onLoadSuccess: function () {
                $(_this.sourceTable).datagrid("resize");
                _this.initGridEvent();
            }
        });
    };
    /**
     * 显示到备选表格
     * @param fapiaos
     */
    FPEditReconcile.prototype.showDestData = function (fapiaos) {
        var _this = this;
        var dest = _.clone(fapiaos || this.destFapiaos || []);
        //显示空数据
        $(this.destTable).datagrid({
            data: (!dest || dest.length == 0) ? [] : dest,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $(this.sourceDiv).width(),
            height: $(this.sourceDiv).height(),
            columns: [[
                    {
                        field: 'MID', title: '', width: 20, align: 'left', formatter: function (value, record, index) {
                            return "<div style='text-align:center'><input type='checkbox' target='0' checked='checked' class='fp-record-checkbox' mid='" + record.MID + "' index='" + index + "'/></div>";
                        }
                    },
                    {
                        field: 'MNumber', title: HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号"), width: 100, align: 'center'
                    },
                    {
                        field: 'MBizDate', title: HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期"), width: 100, align: 'center', formatter: function (value) {
                            return mDate.format(value);
                        }
                    },
                    {
                        field: 'MContactName', title: HtmlLang.Write(LangModule.FP, "Company", "公司"), width: 300, align: 'left', formatter: function (value, record) {
                            return _this.type == FPEnum.Sales ? record.MPContactName : record.MSContactName;
                        }
                    },
                    {
                        field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 100, align: 'right', formatter: function (value) {
                            return mMath.toMoneyFormat(value);
                        }
                    }
                ]],
            onLoadSuccess: function () {
                $(_this.destTable).datagrid("resize");
                _this.initGridEvent();
            }
        });
    };
    /**
     * 初始化表格里面checkbox的点击事件
     */
    FPEditReconcile.prototype.initGridEvent = function () {
        var _this = this;
        //勾选点击的时候做的操作
        $(this.fapiaoCheckbox).off("click").on("click", function (evt) {
            var $element = $(evt.target || evt.srcElement);
            var target = $element.attr("target");
            var mid = $element.attr("mid");
            var index = +($element.attr("index"));
            if (!mid)
                return;
            //如果是从备选区选到已选区
            if (target == "1") {
                //先看下是否在已选区已经有了
                var src = _this.srcFapiaos.where('x.MID == "' + mid + '"')[0];
                var i = _this.getFapiaoIndex(_this.destFapiaos, src);
                var append = i == _this.destFapiaos.length;
                if (_this.destFapiaos.where('x.MID == "' + mid + '"').length == 0) {
                    _this.destFapiaos.push(src);
                }
                //从备选区移除
                _this.srcFapiaos = _this.srcFapiaos.filter(function (value, index, array) {
                    return value.MID != mid;
                });
                $(_this.sourceTable).datagrid("deleteRow", index);
                //如果需要插入到最后，则直接用appendRow
                if (append) {
                    $(_this.destTable).datagrid("appendRow", src);
                }
                else {
                    $(_this.destTable).datagrid("insertRow", {
                        index: i,
                        row: src
                    });
                }
            }
            else {
                //如果从备选区移到已选区
                var dest = _this.destFapiaos.where('x.MID == "' + mid + '"')[0];
                var i = _this.getFapiaoIndex(_this.srcFapiaos, dest);
                var append = i == _this.srcFapiaos.length;
                if (_this.srcFapiaos.where('x.MID == "' + mid + '"').length == 0) {
                    _this.srcFapiaos.push(dest);
                }
                $(_this.destTable).datagrid("deleteRow", index);
                //如果需要插入到最后，则直接用appendRow
                if (append) {
                    $(_this.sourceTable).datagrid("appendRow", dest);
                }
                else {
                    $(_this.sourceTable).datagrid("insertRow", {
                        index: i,
                        row: dest
                    });
                }
                //从已选取移除
                _this.destFapiaos = _this.destFapiaos.filter(function (value, index, array) {
                    return value.MID != mid;
                });
            }
            _this.reorderGridCheckIndex();
            _this.initGridEvent();
            _this.handleAdjustAmountInput();
            _this.calculateAmount();
            _this.orderFapiaos();
        });
    };
    /**
     * 重新给发票列表排序
     */
    FPEditReconcile.prototype.orderFapiaos = function () {
        this.srcFapiaos = this.srcFapiaos.sort(function (a, b) {
            return (a.MBizDate < b.MBizDate || (a.MBizDate == b.MBizDate && a.MNumber < b.MNumber)) ? 1 : -1;
        });
        this.destFapiaos = this.destFapiaos.sort(function (a, b) {
            return (a.MBizDate < b.MBizDate || (a.MBizDate == b.MBizDate && a.MNumber < b.MNumber)) ? 1 : -1;
        });
    };
    /**
     * 获取一张发票的正常位置
     * @param fapiao
     */
    FPEditReconcile.prototype.getFapiaoIndex = function (fapiaos, fapiao) {
        if (!fapiaos || fapiaos.length == 0)
            return 0;
        for (var i = 0; i < fapiaos.length; i++) {
            //找到它后面那一条
            if (fapiaos[i].MBizDate < fapiao.MBizDate) {
                return i;
            }
        }
        //插到最后
        return fapiaos.length;
    };
    /**
     * 初始化面板事件
     */
    FPEditReconcile.prototype.initEvent = function () {
        var _this = this;
        //显示全部
        $(this.showAllCbx).off("click").on("click", function (evt) {
            $(_this.btnSearch).trigger("click");
        });
        //搜索
        $(this.btnSearch).off("click").on("click", function (evt) {
            _this.loadData(true);
        });
        //勾兑按钮
        $(this.reconcileBtn).off("click").on("click", function (evt) {
            _this.saveReconcileModel();
        });
        //调整金额输入框
        $(this.adjustInput).off("keyup").on("keyup", function (evt) {
            var value = $(evt.target || evt.srcElement).val();
            if (value != "-") {
                _this.calculateAmount(+value);
            }
        });
        //清空
        $(this.clearBtn).off("click").on("click", function () {
            $(_this.companyFilterInput).val("");
            $(_this.amountFilterInput).numberbox("clear");
            $(_this.amountFilterInput).val("");
        });
        $("body").off("keyup").on("keyup", function (evt) {
            if (evt.keyCode == 13) {
                $(_this.btnSearch).trigger("click");
            }
        });
    };
    /**
     * 重排checkbox的index
     */
    FPEditReconcile.prototype.reorderGridCheckIndex = function () {
        $(this.sourceDiv).find(this.fapiaoCheckbox).each(function (index, elem) {
            $(elem).attr("index", index);
        });
        $(this.destDiv).find(this.fapiaoCheckbox).each(function (index, elem) {
            $(elem).attr("index", index);
        });
    };
    /**
     * 初始化元素  高度 宽度 滚动等
     */
    FPEditReconcile.prototype.initDom = function () {
        $(this.sourceDiv).height(($("body").height() - 315) / 2);
        $(this.destDiv).height(($("body").height() - 315) / 2);
    };
    /**
     * 初始化分页
     */
    FPEditReconcile.prototype.initPage = function () {
    };
    return FPEditReconcile;
}());
//# sourceMappingURL=FPEditReconcile.js.map