/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
/**
 * 固定资产首页类
 */
var FAFixAssetsHome = /** @class */ (function () {
    //构造函数
    function FAFixAssetsHome() {
        //tab总
        this.tab = ".fa-main-tabs";
        //新增按钮
        this.newFixAssetsButton = "#divNewFixAssets";
        //折旧汇总和卡片列表的两个按钮
        this.cardsButton = ".fa-cards-btn";
        this.depreciationButton = ".fa-depreciation-btn";
        //折旧汇总和卡片列表两个div
        this.cardsDiv = ".fa-cards-div";
        this.depreciationDiv = ".fa-depreciation-div";
        //核算维度和核算维度值
        this.checkGroupInput = ".fa-checkgroup-input";
        this.checkGroupValueInput = ".fa-checkgroupvalue-input";
        this.numberInput = ".fa-number-input";
        this.keywordInput = ".fa-keyword-input";
        //折旧列表的关键字和编号
        this.depNumberInput = ".fa-dep-number-input";
        this.depKeywordInput = ".fa-dep-keyword-input";
        this.table = ".fa-fixassets-grid";
        this.depreciateTable = ".fa-depreciate-grid";
        this.periodInput = ".fa-period-input";
        //操作按钮
        this.editTableButton = ".fa-edit-btn";
        this.removeTableButton = ".fa-remove-btn";
        this.changeTableButton = ".fa-change-btn";
        this.disposeButton = "#aDispose";
        this.sellButton = '#aSell';
        this.handleTopButton = '#aHandleTop';
        this.deleteButton = "#aDeleteFixAssets";
        this.unhandleButton = "#aUnhandle";
        this.searchButton = "#aSearchFixAssets";
        this.clearButton = '#aClear';
        this.batchSetupButton = "#aBatchSetup";
        this.depreciateButton = "#aDepreciate";
        this.searchDepreciateButton = "#aSearchDepreciation";
        this.importButton = "#btnImport";
        this.aExport = "#aExport";
        //查看凭证
        this.viewVoucherButton = ".bs-view-voucher";
        //分页控制
        this.pageParams = [
            { index: 1, size: 20, total: 0 },
            { index: 1, size: 20, total: 0 },
            { index: 1, size: 20, total: 0 }
        ];
        //折旧列表的分页控制
        this.depPageParam = { index: 1, size: 20, total: 0 };
        //分页空间
        this.pagerDiv = ".fa-fixassets-pager";
        this.depPagerDiv = ".fa-depreciation-pager";
        this.checkType = new GLCheckType();
        this.home = new FAHome();
        this.voucherHome = new GLVoucherHome();
    }
    /**
     * 初始化
     */
    FAFixAssetsHome.prototype.init = function () {
        //初始化页面元素
        this.initDom(0);
        //初始化事件
        this.initEvent();
    };
    /**
     * 初始化页面的事件
     */
    FAFixAssetsHome.prototype.initEvent = function () {
        var _this = this;
        //切换卡片列表和折旧汇总
        $(this.cardsButton + "," + this.depreciationButton).off("click").on("click", function (event) {
            var currentDom = $(event.target || event.srcElement);
            //本身如果再次点击的话，不做任何处理
            if (currentDom.hasClass("current"))
                return;
            //按钮切换颜色
            currentDom.addClass("current").siblings().removeClass('current');
            //下面div对应的隐藏和显示
            $(currentDom.attr("mtarget")).show().siblings().hide();
            if (currentDom[0] == $(_this.depreciationButton)[0]) {
                //如果折旧汇总还没有加载过数据，则直接加载数据
                _this.loadDepreciationData();
            }
            //如果卡片页，则需要刷新卡片数据
            if (currentDom[0] == $(_this.cardsButton)[0]) {
                _this.loadFixAssetsData();
            }
        });
        //批量设置
        $(this.batchSetupButton).off("click").on("click", function () {
            _this.batchSetupDepreciation();
        });
        //计提折旧
        $(this.depreciateButton).off("click").on("click", function () {
            _this.depreciatePeriod();
        });
        //搜索
        $(this.searchDepreciateButton).off("click").on("click", function () {
            return _this.loadDepreciationData();
        });
        //新增
        $(this.newFixAssetsButton).off("click").on("click", function () {
            _this.home.showEditFixAssets(null, function () {
                //更新tab
                _this.home.initFixAssetsTabTitle($("div[statuscount]"));
                //如果当前tab是卡片列表，这需要刷新显示
                if ($(_this.cardsButton).hasClass("current"))
                    _this.loadFixAssetsData();
            });
        });
        //导出
        $(this.importButton).off("click").on("click", function () {
            ImportBase.showImportBox('/BD/Import/Import/Fixed_Assets', HtmlLang.Write(LangModule.FA, "ImportFixedAssetCard", "导入固定资产卡片"), 900, 520);
            ;
        });
        $(this.aExport).off("click").on("click", function () {
            var queryParam = _this.getQueryFilter();
            location.href = '/FA/FAFixAssets/Export?jsonParam=' + JSON.stringify(queryParam);
            mDialog.message(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });
    };
    //初始化分页的事件
    //初始化翻页事件
    FAFixAssetsHome.prototype.initPagerEvent = function () {
        var _this = this;
        var index = +this.partial.attr("index");
        //调用easyui组件
        $(this.pagerDiv, this.partial).pagination({
            total: this.pageParams[index].total,
            pageSize: this.pageParams[index].size,
            pageList: this.home.getPageList(),
            onChangePageSize: function (size) {
                _this.pageParams[index].size = size;
                _this.loadFixAssetsData();
            },
            onRefresh: function (i, size) {
                _this.pageParams[index].index = i;
                _this.pageParams[index].size = size;
                _this.loadFixAssetsData();
            },
            onSelectPage: function (i, size) {
                _this.pageParams[index].index = i;
                _this.pageParams[index].size = size;
                _this.loadFixAssetsData();
            }
        });
    };
    ;
    //初始化翻页事件
    FAFixAssetsHome.prototype.initDepPagerEvent = function () {
        var _this = this;
        //调用easyui组件
        $(this.depPagerDiv).pagination({
            total: this.depPageParam.total,
            pageSize: this.depPageParam.size,
            pageList: this.home.getPageList(),
            onChangePageSize: function (size) {
                _this.depPageParam.size = size;
                _this.loadDepreciationData();
            },
            onRefresh: function (index, size) {
                _this.depPageParam.index = index;
                _this.depPageParam.size = size;
                _this.loadDepreciationData();
            },
            onSelectPage: function (index, size) {
                _this.depPageParam.index = index;
                _this.depPageParam.size = size;
                _this.loadDepreciationData();
            }
        });
    };
    ;
    /**
     * 初始化页面元素
     */
    FAFixAssetsHome.prototype.initDom = function (index) {
        var _this = this;
        //初始化
        $(this.tab).tabsExtend({
            //默认的显示标签
            initTabIndex: index,
            //选择标签函数
            onSelect: function (index) {
                //
                _this.showTab(index);
            }
        });
    };
    /**
     * 初始化某一块的元素
     * @param index
     */
    FAFixAssetsHome.prototype.initPartialDom = function () {
        var _this = this;
        $(this.keywordInput + "," + this.numberInput).initHint();
        $(this.handleTopButton, this.partial).splitbutton({
            menu: $('#divHandle', this.partial)
        });
        var isSmart = $("#smartVersion").val() == "1";
        //核算维度类型下拉选择框
        $(this.checkGroupInput, this.partial).combobox({
            textField: 'name',
            valueField: 'value',
            data: this.home.getCheckGroupNameArray(isSmart),
            onSelect: function (row) {
                if (row.value == null || row.value == undefined)
                    return;
                var lastInput = $(_this.checkGroupInput, _this.partial).data("lastinput");
                //如果并没有切换，则不需要进行任何操作
                if (!!lastInput && lastInput.attr("typeid") == row.value)
                    return;
                $(_this.checkGroupValueInput + "[typeid='-1']", _this.partial).hide();
                $(_this.checkGroupValueInput + "[typeid='-1']", _this.partial).combobox("textbox").parent().hide();
                var input = $(_this.checkGroupValueInput + ".demo[typeid='" + row.value + "']", _this.partial).clone().removeClass("demo").insertAfter($(_this.checkGroupValueInput + "[typeid='" + row.value + "']", _this.partial));
                if (lastInput != null && lastInput != undefined && lastInput.attr("typeid") == input.attr("typeid")) {
                    return (lastInput.hasClass('combotree-f') ? input.combotree('textbox').parent().show() : input.combobox('textbox').parent().show());
                }
                if (lastInput != null)
                    $(lastInput).combobox("destroy");
                //核算维度值下来选择
                _this.checkType.bindCheckType2Dom(1, row.value, $(""), input, 0, { height: 300, width: 200, onSelect: function () { } });
                //背景颜色
                input.attr("hint", row.name).initHint();
                $(_this.checkGroupInput, _this.partial).data("lastinput", input);
            },
            onLoadSuccess: function () {
                $(_this.checkGroupInput, _this.partial).combobox("setValue", '');
                $(_this.checkGroupInput, _this.partial).combobox("setText", '');
            }
        });
    };
    /**
     * 显示某个tab页
     * @param index
     */
    FAFixAssetsHome.prototype.showTab = function (index) {
        //如果为空，则默认显示为0
        index = index == null || index == undefined ? 0 : index;
        //加上class
        $("ul.tab-links").find("li:eq(" + index + ")").addClass("current").siblings().removeClass("current");
        //当前需要显示的内容
        this.partial = $(".fa-partial-" + index);
        //根据index的不同，控制partial的显示
        this.partial.show().siblings().hide();
        //如果还没有内容，则需要从democopy一个过来
        if (this.partial.attr("inited") !== "1") {
            //显示展示和隐藏
            this.handleShowPartial(index);
            //默认初始化某一块的数据
            this.initPartialDom();
            //初始化事件
            this.intPartialEvent();
            //标记为已经初始化
            this.partial.attr("inited", 1);
        }
        //tab需要刷新
        this.home.initFixAssetsTabTitle($("div[statuscount]"));
        //初始化数据 每次切换都要重新加载数据，避免，有数据不同步的问题出现
        this.loadFixAssetsData();
    };
    /**
     *
     */
    FAFixAssetsHome.prototype.intPartialEvent = function () {
        var _this = this;
        //处置
        $(this.handleTopButton + "," + this.sellButton + "," + this.disposeButton).off("click").on("click", function (event) {
            //获取用户勾选的所有可处置的对象
            var rows = _this.getCheckRows($(_this.table, _this.partial), 'x.MStatus == "0"');
            if (rows != null) {
                var type = $(event.target || event.srcElement).attr("newstatus") == undefined ? $(event.target || event.srcElement).parents("[newstatus]").attr("newstatus") : $(event.target || event.srcElement).attr("newstatus");
                _this.home.handleFixAssets(rows.select('MItemID'), +type, function () {
                    _this.home.initFixAssetsTabTitle($("div[statuscount]"));
                    _this.loadFixAssetsData();
                });
            }
        });
        //撤销处置
        $(this.unhandleButton, this.partial).off("click").on("click", function (event) {
            //获取用户勾选的所有可处置的对象
            var rows = _this.getCheckRows($(_this.table, _this.partial), 'x.MStatus != 0');
            if (rows != null)
                _this.home.handleFixAssets(rows.select('MItemID'), parseInt($(_this.unhandleButton, _this.partial).attr("newstatus")), function () {
                    _this.home.initFixAssetsTabTitle($("div[statuscount]"));
                    _this.loadFixAssetsData();
                });
        });
        //删除
        $(this.deleteButton, this.partial).off("click").on("click", function (event) {
            //获取用户勾选的所有可处置的对象
            var rows = _this.getCheckRows($(_this.table, _this.partial), 'mDate.parse(x.MLastDepreciatedDate).getFullYear() <= 1901 && x.MStatus == 0 ');
            if (rows != null)
                _this.home.deleteFixAssets(rows.select('MItemID'), function () {
                    _this.home.initFixAssetsTabTitle($("div[statuscount]"));
                    _this.loadFixAssetsData();
                });
        });
        //过滤查询
        $(this.searchButton, this.partial).off("click").on("click", function (event) {
            _this.loadFixAssetsData();
        });
        //过滤查询
        $(this.clearButton, this.partial).off("click").on("click", function (event) {
            _this.clearFilter();
        });
    };
    /**
     * 初始化表格里面的事件
     */
    FAFixAssetsHome.prototype.initFixAssetsGridEvent = function (table) {
        var _this = this;
        //删除
        $(this.removeTableButton, this.partial).off("click").on("click", function (event) {
            _this.home.deleteFixAssets([$(event.target || event.srcElement).attr("mid")], function () {
                _this.home.initFixAssetsTabTitle($("div[statuscount]"));
                _this.loadFixAssetsData();
            });
        }).tooltip({
            content: HtmlLang.Write(LangModule.FA, "Click2DeleteCards", "点击删除卡片")
        });
        //编辑和变更
        $(this.editTableButton + "," + this.changeTableButton, this.partial).off("click").on("click", function (event) {
            _this.home.showEditFixAssets($(event.target || event.srcElement).attr("mid"), function () {
                _this.home.initFixAssetsTabTitle($("div[statuscount]"));
                _this.loadFixAssetsData();
            });
        });
        //编辑
        $(this.editTableButton, this.partial).tooltip({
            content: HtmlLang.Write(LangModule.FA, "Click2ViewEditCards", "点击查看/编辑卡片")
        });
        //变更
        $(this.changeTableButton, this.partial).tooltip({
            content: HtmlLang.Write(LangModule.FA, "Click2ViewEditCards", "点击查看/编辑卡片")
        });
        //查看凭证
        $(this.viewVoucherButton, this.partial).off("click.bs").on("click.bs", function (event) {
            _this.voucherHome.editVoucher($(event.target || event.srcElement).attr("MVoucherID"), null);
        });
    };
    /**
     * 初始化表格里面的事件
     */
    FAFixAssetsHome.prototype.initDepreciationGridEvent = function (table) {
        var _this = this;
        //查看凭证
        $(this.viewVoucherButton).off("click.bs").on("click.bs", function (event) {
            _this.voucherHome.editVoucher($(event.target || event.srcElement).attr("MVoucherID"), null);
        });
    };
    /**
     * 获取所有勾选的行
     * @param filter
     */
    FAFixAssetsHome.prototype.getCheckRows = function (table, filter) {
        //获取用户勾选的所有可处置的对象
        var rows = $(table).datagrid("getChecked").where(filter);
        if (rows == null || rows.length == 0) {
            mDialog.alert(HtmlLang.Write(LangModule.FA, "NoAvaliableFixAssetsRecords2Operate", "没有可操作的资产卡片，请选择卡片并检查所选卡片的状态!"));
            return null;
        }
        return rows;
    };
    /**
     * 批量设置计提折旧科目或者金额等
     */
    FAFixAssetsHome.prototype.batchSetupDepreciation = function () {
        var _this = this;
        var rows = this.getCheckRows($(this.depreciateTable), '1==1');
        var query = this.getDepQueryFilter();
        if (rows != null)
            this.home.batchSetupDepreciation(query, rows.select('MID'), function () {
                return _this.loadDepreciationData();
            });
        else {
            HtmlLang.Write(LangModule.FA, 'PleaseChooseRow2Operate', '请选择要操作的行!');
        }
    };
    /**
     * 计提折旧
     */
    FAFixAssetsHome.prototype.depreciatePeriod = function () {
        var _this = this;
        var query = this.getDepQueryFilter();
        this.home.depreciatePeriodEdit(query, function () {
            return _this.loadDepreciationData();
        });
    };
    /**
     *
     * @param partial
     * @param index

     */
    FAFixAssetsHome.prototype.handleShowPartial = function (index) {
        //如果有需要显示的下标，则显示，没有就隐藏
        $("a[showindex],div[showindex]", this.partial).each(function () {
            if ($(this).attr("showindex").split(',').contains(index.toString())) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        });
    };
    /**
     * 清空查询条件
     */
    FAFixAssetsHome.prototype.clearFilter = function () {
        $(this.numberInput, this.partial).val('');
        $(this.keywordInput, this.partial).val('');
        $(this.checkGroupInput, this.partial).combobox("setValue", '');
        $(this.checkGroupInput, this.partial).combobox("setText", '');
        $(this.checkGroupValueInput + "[typeid='-1']", this.partial).combobox("textbox").parent().show();
        var lastInput = $(this.checkGroupInput, this.partial).data("lastinput");
        //清空选择的核算维度类型
        if (lastInput != null && lastInput != undefined) {
            //如果是树控件
            if (lastInput.hasClass('combotree-f'))
                lastInput.combotree('destroy');
            else
                lastInput.combobox('destroy');
            //既然是清空那就需要清掉所有的内容
            $(this.checkGroupInput, this.partial).data("lastinput", null);
        }
    };
    /**
     * 加载卡片数据
     */
    FAFixAssetsHome.prototype.loadFixAssetsData = function () {
        var _this = this;
        //获取数据并加载
        this.home.getFixAssetList(this.getQueryFilter(), function (data) {
            _this.showFixAssetsData(data);
        });
    };
    /**
     * 加载折旧汇总列表
     */
    FAFixAssetsHome.prototype.loadDepreciationData = function () {
        var _this = this;
        //获取数据并加载
        this.home.GetDepreciationPageList(this.getDepQueryFilter(), function (data) {
            _this.showDepreciationData(data);
        });
    };
    /**
     * 加载查询条件
     */
    FAFixAssetsHome.prototype.getDepQueryFilter = function () {
        var period = mDate.parse($(this.periodInput).val() + "-01");
        return {
            Year: period.getFullYear(),
            Period: period.getMonth() + 1,
            page: this.depPageParam.index,
            rows: this.depPageParam.size,
            Keyword: $(this.depKeywordInput).val(),
            MKeyword: +($(this.depKeywordInput).val()),
            Number: $(this.depNumberInput).val()
        };
    };
    /**
     * 获取用户选择的核算维度值
     */
    FAFixAssetsHome.prototype.getQueryFilter = function () {
        var checkGroupValue = "";
        var checkGroupID = $(this.checkGroupInput, this.partial).combobox("getValue");
        var checkGroupText = $(this.checkGroupInput, this.partial).combobox("getText");
        //上一个核算维度选择的input
        var lastInput = $(this.checkGroupInput, this.partial).data("lastinput");
        //如果有input，证明用户选择过核算维度
        if (lastInput != null && lastInput != undefined && lastInput.length != 0 && checkGroupID != null && checkGroupID != undefined && checkGroupText != null && checkGroupText != undefined && checkGroupText.length > 0) {
            var value = lastInput.data("selectedvalue");
            if (value != null && value != undefined) {
                checkGroupValue = value.id;
            }
        }
        var pageParam = this.pageParams[+$(this.partial).attr("index")];
        return {
            CheckGroup: this.checkType.getCheckTypeColumnByType(+checkGroupID),
            CheckGroupValue: checkGroupValue,
            Keyword: $(this.keywordInput, this.partial).val(),
            MKeyword: +($(this.keywordInput, this.partial).val()),
            Number: $(this.numberInput, this.partial).val(),
            Status: +this.partial.attr("status"),
            Date: this.getPeriod(),
            page: pageParam.index,
            rows: pageParam.size
        };
    };
    /**
     * 展示数据到div下面
     * @param data
     */
    FAFixAssetsHome.prototype.showFixAssetsData = function (data) {
        var _this = this;
        var tableDiv = $(this.table, this.partial);
        this.pageParams[+this.partial.attr("index")].total = data.total;
        //初始化分页数据
        this.initPagerEvent();
        //如果已经初始化了，则直接加载数据就行
        if (tableDiv.attr("inited") == "1") {
            $(tableDiv).datagrid("loadData", data.rows);
            return this.initFixAssetsGridEvent(tableDiv);
        }
        //初始化数据
        $(tableDiv).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: tableDiv.width() - 5,
            height: ($("body").height() - tableDiv.offset().top - 32),
            columns: [[
                    {
                        field: 'MItemID', title: '', width: 40, checkbox: true, align: 'left', formatter: function (data, rec) { return data; }
                    },
                    {
                        field: 'MFullNumber', width: 60, title: HtmlLang.Write(LangModule.FA, "FixAssetNumber", "编码"), align: 'left', formatter: function (data, rec, rowIndex) {
                            return '<a href="javascript:void(0)" class="' + _this.changeTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '">' + data + '</a>';
                        }
                    },
                    {
                        field: 'MName', width: 80, title: HtmlLang.Write(LangModule.FA, "FixAssetName", "名称"), align: 'left', formatter: function (val, rec) {
                            return "<div title='" + mText.encode(val) + "'>" + mText.encode(val) + "</div>";
                        }
                    },
                    {
                        field: 'MFATypeIDName', width: 100, title: HtmlLang.Write(LangModule.FA, "FixAssetTypeName", "类别名称"), align: 'left', formatter: function (val, rec) {
                            return "<div title='" + mText.encode(val) + "'>" + mText.encode(val) + "</div>";
                        }
                    },
                    {
                        field: 'MPurchaseDate', width: 60, title: HtmlLang.Write(LangModule.FA, "PurchaseDate", "采购日期"), align: 'center', formatter: function (val, rec) {
                            return mDate.format(val, 'YYYY-MM-DD');
                        }
                    },
                    {
                        field: 'MDepreciationFromPeriod', width: 80, title: HtmlLang.Write(LangModule.FA, "DepreciationFromPeriod", "折旧开始期间"), align: 'center', formatter: function (data, rec) {
                            return _this.home.formateDate(data);
                        }
                    },
                    {
                        field: 'MOriginalAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "OriginalAmount", "原值"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MDepreciatedAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "CurrentDepreciatedAmount", "累计折旧"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MPeriodDepreciatedAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "PeriodDepreciateAmount", "月折旧"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MUsefulPeriods', width: 50, title: HtmlLang.Write(LangModule.FA, "UsefulPeriods", "使用期数"), align: 'right'
                    },
                    {
                        field: 'MDepreciatedPeriods', width: 60, title: HtmlLang.Write(LangModule.FA, "DepreciatedPeriods", "已折旧期数"), align: 'right'
                    },
                    {
                        field: 'MLastDepreciatedDate', width: 60, title: HtmlLang.Write(LangModule.FA, "LastDepreciatedDate", "最后计提期间"), align: 'center', formatter: function (val, rec) {
                            return rec.MLastDepreciatedYear < 2000 ? '' : (mDate.parse(val).getFullYear() + "-" + (mDate.parse(val).getMonth() < 9 ? ('0' + (mDate.parse(val).getMonth() + 1)) : +(mDate.parse(val).getMonth() + 1)));
                        }
                    }
                ].concat([{
                        field: 'MStatus', width: 40, title: HtmlLang.Write(LangModule.FA, "MStatus", "状态"), align: 'center', formatter: function (val, row) {
                            return _this.home.getFixAssetsStatus(val, row);
                        }
                    }]).concat([
                    {
                        field: 'Operation', width: 40, title: HtmlLang.Write(LangModule.Common, "Operation", "操作"), align: 'left', formatter: function (val, rec, rowIndex) {
                            var text = '<div class="list-item-action">';
                            //显示编辑、变更、删除按钮的控制
                            var showEdit = true, showChange = !(rec.MLastDepreciatedDate == null || mDate.parse(rec.MLastDepreciatedDate).getFullYear() < 2000), showDelete = (rec.MLastDepreciatedDate == null || mDate.parse(rec.MLastDepreciatedDate).getFullYear() < 2000) && rec.MStatus == 0;
                            text += showChange ? ('<a href="javascript:void(0)" class="list-item-edit ' + _this.changeTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>') : '';
                            text += showEdit && !showChange ? ('<a href="javascript:void(0)" style="margin-right:10px" class="list-item-edit ' + _this.editTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>') : '';
                            text += showDelete ? ('<a href="javascript:void(0)" class="m-icon-delete-row ' + _this.removeTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>') : '';
                            text += '</div>';
                            return text;
                        }
                    }
                ])],
            onClickRow: function () {
                return false;
            },
            onLoadSuccess: function () {
                _this.home.clearGridCheckbox(tableDiv);
            }
        });
        $(tableDiv).attr("inited", 1);
        return this.initFixAssetsGridEvent(this.partial);
    };
    /**
     * 获取选择的月份
     */
    FAFixAssetsHome.prototype.getPeriod = function () {
        //
        return mDate.parse($(this.periodInput).val() + "-01");
    };
    /**
     * 展示数据到div下面
     * @param data
     */
    FAFixAssetsHome.prototype.showDepreciationData = function (data) {
        var _this = this;
        var tableDiv = $(this.depreciateTable);
        this.depPageParam.total = data.total;
        this.initDepPagerEvent();
        var period = this.getPeriod();
        //如果已经初始化了，则直接加载数据就行
        if (tableDiv.attr("inited") == "1") {
            $(tableDiv).datagrid('getColumnOption', 'MPeriodDepreciatedAmount').title = HtmlLang.Write(LangModule.FA, "PeriodDepreciateAmount", "月折旧额") + "\n" + "[" + $(this.periodInput).val() + "]";
            $(tableDiv).datagrid();
            $(tableDiv).datagrid("loadData", data.rows);
            return this.initDepreciationGridEvent(tableDiv);
        }
        //初始化分页数据
        this.initPagerEvent();
        //初始化数据
        $(tableDiv).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: tableDiv.width() - 5,
            height: ($("body").height() - tableDiv.offset().top - 32),
            columns: [[
                    {
                        field: 'MItemID', title: '', width: 40, checkbox: true, align: 'left'
                    },
                    {
                        field: 'MFixAssetsNumber', width: 60, title: HtmlLang.Write(LangModule.FA, "FixAssetNumber", "编码"), align: 'left'
                    },
                    {
                        field: 'MFixAssetsName', width: 80, title: HtmlLang.Write(LangModule.FA, "FixAssetName", "名称"), align: 'left', formatter: function (val, rec) {
                            return "<div title='" + mText.encode(val) + "'>" + mText.encode(val) + "</div>";
                        }
                    },
                    {
                        field: 'MFATypeIDName', width: 100, title: HtmlLang.Write(LangModule.FA, "FixAssetTypeName", "类别名称"), align: 'left', formatter: function (val, rec) {
                            return "<div title='" + mText.encode(val) + "'>" + mText.encode(val) + "</div>";
                        }
                    },
                    {
                        field: 'MPurchaseDate', width: 60, title: HtmlLang.Write(LangModule.FA, "PurchaseDate", "采购日期"), align: 'center', formatter: function (val, rec) {
                            return mDate.format(val, 'YYYY-MM-DD');
                        }
                    },
                    {
                        field: 'MDepreciationFromPeriod', width: 80, title: HtmlLang.Write(LangModule.FA, "DepreciationFromPeriod", "折旧开始期间"), align: 'center', formatter: function (data, rec) {
                            return data ? _this.home.formateDate(data) : '';
                        }
                    },
                    {
                        field: 'MOriginalAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "OriginalAmount", "原值"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MPeriodDepreciatedAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "PeriodDepreciateAmount", "月折旧") + "\n" + "[" + $(this.periodInput).val() + "]", align: 'right', formatter: function (val, rec) {
                            return !rec.MItemID ? '' : mMath.toMoneyFormat(val + rec.MLastAdjustAmount);
                        }
                    },
                    {
                        field: 'MDepreciatedAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "DepreciatedAmount", "期末累计折旧"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MDepreciatedAmountOfYear', width: 60, title: HtmlLang.Write(LangModule.FA, "DepreciatedAmountOfYear", "本年累计折旧"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MPrepareForDecreaseAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "PrepareForDecreaseAmount", "减值准备"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MNetAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "PeriodNetAmount", "期末净值"), align: 'right', formatter: function (val, rec) {
                            //if (!rec.MItemID) {
                            return mMath.toMoneyFormat(rec.MOriginalAmount - rec.MPrepareForDecreaseAmount - rec.MDepreciatedAmount);
                            //}
                            //else {
                            //    return mMath.toMoneyFormat(val);
                            //}
                        }
                    },
                    {
                        field: 'MDepreciationProgress', width: 60, title: HtmlLang.Write(LangModule.FA, "DepreciationProgress", "折旧进度"), align: 'right', formatter: function (val, rec) {
                            return (rec.MVoucherNumber != null && rec.MVoucherNumber != undefined && rec.MVoucherNumber.length > 0)
                                ? ('<a href="###" MVoucherID="' + rec.MVoucherID + '" class="bs-view-voucher"  style="color: rgb(4, 143, 194);">GL-' + rec.MVoucherNumber + '</a>') : HtmlLang.Write(LangModule.FA, 'Undepreciated', '未折旧');
                        }
                    }
                ]],
            onClickRow: function () {
                return false;
            },
            onLoadSuccess: function () {
                _this.home.clearGridCheckbox(tableDiv);
            }
        });
        $(tableDiv).attr("inited", 1);
        return this.initDepreciationGridEvent(tableDiv);
    };
    return FAFixAssetsHome;
}());
//# sourceMappingURL=FAFixAssetsHome.js.map