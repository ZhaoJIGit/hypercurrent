/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPCoding = /** @class */ (function () {
    function FPCoding() {
        this.codingPartial = ".fp-coding-partial";
        this.codingTable = ".fp-coding-table";
        this.codingBody = ".fp-coding-body";
        this.codingTableHeader = ".fp-coding-body .datagrid-htable:visible";
        this.codingBodyMergedTd = ".fp-coding-body td.datagrid-td-merged:visible";
        this.pager = ".fp-coding-partial-pager";
        this.checkBox = ".fp-record-checkbox:visible";
        this.selectTd = "tr.datagrid-row td[field='MID']:visible";
        this.maskDiv = "#fpMaskDiv";
        this.selectDiv = '.fp-select-div';
        this.scrollSpeed = 1;
        this.scrollIntervalId = null;
        this.gridRowTr = "tr.datagrid-row";
        this.splitRowLeftClass = "fp-split-left";
        this.splitRowRightClass = "fp-split-right";
        this.toolbar = ".fp-coding-toolbar,.main-title";
        this.timeOutTime = 30;
        this.mergedTdClass = "fp-merged-td";
        this.gridCell = "tr.datagrid-row td .datagrid-cell:visible";
        this.viewFapiao = ".fp-view-fapiao";
        this.markBtn = "#markCodingStatus";
        this.checkAll = ".fp-record-checkbox-all:visible";
        this.saveSettingBtn = "#btnSaveSetting";
        this.settingDiv = "#fpSettingDiv";
        this.settingBodyDiv = ".fp-setting-body";
        this.resetBtn = "#resetCodingData";
        this.tipsDiv1 = "#fpTipsDiv1";
        this.tipsDiv2 = "#fpTipsDiv2";
        this.shakeClass = "fp-button-shake";
        this.fastCodeItemIDInput = "#fastCodeID";
        this.settingTable = ".fp-setting-table";
        this.settingTableDemoTr = ".fp-setting-tr.demo";
        this.settingTableFieldName = ".fp-setting-field-name";
        this.settingTableFieldValue = ".fp-setting-field-value input[type='checkbox']";
        this.settingCloseBtn = ".fp-coding-setting .fp-field-close";
        this.fastCodeCloseBtn = ".fp-fastcode-close";
        this.allSelectedClass = "fp-all-selected";
        this.partSelectedClass = "fp-part-selected";
        this.isShowAllColumn = false;
        this.allClass = "fp-all";
        this.partClass = "fp-part";
        this.setBtn = "#fpSet";
        this.selectedRowClass = "fp-seleced-row";
        this.disSelectedClass = "fp-disselect-row";
        this.fastCodePartialDiv = "#fastCodeDiv";
        this.fastCodeBody = ".fp-fastcode-body";
        this.fastCodeEdit = ".fp-fastcode-edit";
        this.fastCodeEditClass = "fp-fastcode-edit";
        this.fastCodeSelectedClass = "fp-fastcode-selected";
        this.fastCodeDemoDiv = ".fp-fastcode-partial";
        this.inventoryTitle = ".fp-inventory-title";
        this.splitTitle = ".fp-splite-title";
        this.psContactTitle = ".fp-pscontact-title";
        this.bizDateTitle = ".fp-bizdate-title";
        this.fapiaoNumberTitle = ".fp-fapiaonumber-title";
        this.fastCodeTitle = ".fp-fastcode-title";
        this.taxAccountTitle = ".fp-taxaccount-title";
        this.saveNCreateBtn = "#btnSaveNCreate";
        this.mergeCreate = "#btnMergeCreate";
        this.searchCodingBtn = "#bthSearchCoding";
        this.saveFastCodeBtn = ".fp-fastcode-save:visible";
        this.keywordInput = ".fp-coding-keyword";
        this.splitBtn = ".fp-split-l:visible";
        this.deleteBtn = ".fp-delete-row:visible";
        this.mergeFields = ["MID", "MFapiaoNumber", "MBizDate", "MPSContactName"];
        this.disableFields = ["MAmount", "MTaxRate", "MTotalAmount", "MTaxAmount"];
        this.isLoadingData = false;
        this.sort = "MBizDate";
        this.order = "desc";
        this.createVoucherWithRedConfirm = HtmlLang.Write(LangModule.FP, "CreditFapiaoWillGenerateIndividually", "选择的发票中存在红字发票, 红字发票会单独生成凭证, 是否确认生成凭证?");
        this.sortType = [
            {
                name: "MBizDate",
                value: "desc"
            },
            {
                name: "MPSContactName",
                value: "asc"
            },
            {
                name: "MFapiaoNumber",
                value: "asc"
            }
        ];
        this.editorTips = [
            {
                name: "MContactID",
                value: HtmlLang.Write(LangModule.FP, "InputNewContact2Save", "输入新的联系人名称后，点击生成凭证后可直接保存为基础资料")
            },
            {
                name: "MMerItemID",
                value: HtmlLang.Write(LangModule.FP, "InputNewItem2Save", "输入新的商品项目名称后（格式为[编号:描述])，点击生成凭证后可直接保存为基础资料")
            },
            {
                name: "MTrackItem1",
                value: HtmlLang.Write(LangModule.FP, "InputNewTackOptions2Save", "输入新的子项后，点击生成凭证后可直接保存为基础资料")
            },
            {
                name: "MTrackItem2",
                value: HtmlLang.Write(LangModule.FP, "InputNewTackOptions2Save", "输入新的子项后，点击生成凭证后可直接保存为基础资料")
            },
            {
                name: "MTrackItem3",
                value: HtmlLang.Write(LangModule.FP, "InputNewTackOptions2Save", "输入新的子项后，点击生成凭证后可直接保存为基础资料")
            },
            {
                name: "MTrackItem4",
                value: HtmlLang.Write(LangModule.FP, "InputNewTackOptions2Save", "输入新的子项后，点击生成凭证后可直接保存为基础资料")
            },
            {
                name: "MTrackItem5",
                value: HtmlLang.Write(LangModule.FP, "InputNewTackOptions2Save", "输入新的子项后，点击生成凭证后可直接保存为基础资料")
            }
        ];
        this.amountFeilds = ["MAmount", "MTaxRate", "MTotalAmount"];
        this.hiddenColumns = [];
        this.tempHiddenColumns = [];
        this.tempShownColumns = [];
        this.saveInRealTime = true;
        this.gridEditors = [];
        this.allColumns = [];
        this.gridData = [];
        this.sourceData = [];
        this.mouseDown = false;
        this.isSelect = false;
        this.lastCheckedBox = null;
        this.lastSelectedRow = null;
        this.page = 1;
        this.rows = 20;
        this.hidden = 0;
        this.show = 1;
        this.required = 2;
        this.editIndex = null;
    }
    /**
     * 初始化事件
     */
    FPCoding.prototype.init = function () {
        this.home = new FPReconcileHome();
        this.initEvent();
        this.initDom();
    };
    /**
     * 重置一些基础数据
     */
    FPCoding.prototype.resetData = function () {
        this.endEdit();
        this.gridEditors = [];
        this.gridData = [];
        this.sourceData = [];
        this.mouseDown = false;
        this.isSelect = false;
        this.lastCheckedBox = null;
        this.baseData = null;
        this.hiddenColumns = [];
        $(this.checkAll).removeAttr("checked");
        $(".tooltip-top").hide();
        this.hideCodingSetting();
        if (!this.isShowAllColumn) {
            $("." + this.allSelectedClass).addClass(this.allClass).removeClass(this.allSelectedClass);
            $("." + this.partClass).addClass(this.partSelectedClass).removeClass(this.partClass);
        }
    };
    /**
     * 保存完成后需要清除已经保存的项目
     * @param codings
     */
    FPCoding.prototype.clearSaveData = function (codings) {
    };
    /**
     * 刷新页面重试
     */
    FPCoding.prototype.refreshData = function (keepPage) {
        if (!keepPage)
            this.page = 1;
        $(this.pager).pagination("select", this.page);
    };
    /**
     * 获取当前浏览器最大数
     */
    FPCoding.prototype.getMaxEntryCount = function () {
        if (true)
            100000;
        if ($.browser.msie) {
            return 400;
        }
        else if ($.browser.webkit) {
            return 1000;
        }
        else if ($.browser.mozilla) {
            return 800;
        }
        else if ($.browser.opera) {
            return 1000;
        }
        else {
            return 800;
        }
    };
    /**
     * 获取数据
     */
    FPCoding.prototype.loadData = function (dates, firstTime) {
        var _this = this;
        this.resetData();
        /// add by 锦友 20180523 16:24:00
        ///修复点击搜索时，把选择显示字段弹出层给移除了
        $(".tooltip-top").hide();
        dates = dates || this.home.getPickedDate();
        //获取过滤
        var filter = {
            page: this.page,
            rows: this.rows,
            MStartDate: dates[0],
            MEndDate: dates[1],
            Sort: this.sort,
            Order: this.order,
            MKeyword: $(this.keywordInput).val(),
            MTotalAmount: +($(this.keywordInput).val()),
            MFapiaoCategory: this.home.getType(),
            MMaxEntryCount: this.getMaxEntryCount()
        };
        this.home.getCodingPageList(filter, function (data) {
            !data.Success && mDialog.alert(data.Message);
            _this.baseData = data.BaseData;
            _this.initializeBaseData();
            _this.showData(data.Codings);
            _this.initSetting();
        });
    };
    /**
     * 初始化基础资料
     */
    FPCoding.prototype.initializeBaseData = function () {
        //如果没有联系人或者没有商品项目，都需要讲其变成空数组
        this.baseData.MContact = this.baseData.MContact || [];
        this.baseData.MMerItem = this.baseData.MMerItem || [];
        //取出所有的跟踪项，如果跟踪项有大类，则子项则必须初始化为数组
        var list = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5];
        for (var i = 0; i < list.length; i++) {
            if (list[i] && list[i].MCheckTypeName) {
                list[i].MDataList = list[i].MDataList || [];
            }
        }
    };
    /**
     * 对数据进行编号
     */
    FPCoding.prototype.orderGridData = function () {
        for (var i = 0; i < this.gridData.length; i++) {
            this.gridData[i].MRowIndex = i;
        }
    };
    /**
     * 展示数据到面板
     */
    FPCoding.prototype.showData = function (data) {
        var _this = this;
        this.sourceData = this.home.cloneArray(data.rows);
        this.gridData = data.rows || [];
        this.orderGridData();
        this.allColumns = this.getFrozenColumns().concat(this.getComonColumns());
        this.isLoadingData = true;
        //搜索刷新表格后这个字段保存了上次刷新的值，需要清空
        this.lastSelectedRow = null;
        $(this.codingTable).datagrid({
            data: this.gridData,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            checkOnSelect: false,
            showText: true,
            remoteSort: false,
            width: this.home.getGridWith(),
            height: $(this.codingPartial).height() - 45,
            columns: [this.allColumns],
            onEndEdit: function (index, row, changes) {
                _this.beforeEndEdit(index, row, changes);
            },
            onBeforeEdit: function (index, row) {
                _this.hideTooltip();
            },
            onAfterEdit: function (index, row) {
                _this.editIndex = null;
                _this.hideTooltip();
            },
            onClickRow: function (index, row, evt) {
                _this.handleSelecetRow(index, row, evt);
            },
            onDblClickCell: function (index, field, value, evt) {
                if (field == "MFapiaoNumber" || field == "MBizDate" || field == "MPSContactName") {
                    var td = $(evt.srcElement || evt.target).closest("td[field='" + field + "']:visible");
                    $(_this.keywordInput).val(td.text()).focus();
                }
            },
            onClickCell: function (index, field, value, evt) {
                //处理点击列隐藏提示by huxiaofeng 2018-04-13
                $(".m-tip-top-div").hide();
                if (field == "MSplit" || field == "MID" || field == "MDelete")
                    return;
                //如果是合并行，则不需要做处理了
                if (evt.ctrlKey || evt.shiftKey) {
                    return;
                }
                //如果单独点击开票日期和购买方，表示用户需要选中次行
                if (field == "MInventoryName" || field == "MBizDate" || field == "MPSContactName" || field == "MFapiaoNumber") {
                    $("." + _this.selectedRowClass).removeClass(_this.selectedRowClass);
                    var $tr = _this.getGridRow(evt);
                    _this.selectRow($tr, true);
                    _this.lastSelectedRow = $tr;
                    return;
                }
                //当前行正在编辑，则不需要再编辑了
                if (index == _this.editIndex)
                    return;
                _this.beginEdit(index);
                //点的哪一行，就要聚焦哪一行
                _this.focusCell(field);
            },
            onLoadSuccess: function () {
                //合并行
                _this.initGridEvent();
                _this.initGridCellEvent();
                _this.initGridClass();
                _this.initShowColumnEvent();
                _this.isLoadingData = false;
                //初始化分页控件
                _this.initPage(data.total);
                $(_this.codingTable).datagrid("resize");
                $(".datagrid-body").scroll(function () {
                    $(".tooltip:visible").hide();
                });
            }
        });
    };
    /**
     * 聚焦到某一行
     * @param field
     */
    FPCoding.prototype.focusCell = function (field) {
        ///add by 锦友 20180508 11:12:00
        ///点击税科目时，如果是进项普通，则不用将焦点聚焦到当前列，避免出现下拉框
        if (field == "MTaxAccount") {
            var currentRowData = this.getRowByRowIndex();
            if (!(currentRowData.MInvoiceType == 1 && currentRowData.MType == 0)) {
                this.getRowEditorByField(field).editor.textbox().focus();
            }
        }
        else {
            this.getRowEditorByField(field).editor.textbox().focus();
        }
    };
    /**
     * 获取元素附近的tr
     * @param evt
     */
    FPCoding.prototype.getGridRow = function (evt, elem) {
        elem = !elem ? $(evt.target || evt.srcElement) : elem;
        return elem.closest("tr.datagrid-row");
    };
    /**
     *
     * @param index
     */
    FPCoding.prototype.getGridTrByIndex = function (index) {
        var tr = $("tr.datagrid-row[datagrid-row-index='" + index + "']");
        return tr;
    };
    /**
     * 处理行选择事件
     * @param index
     * @param row
     */
    FPCoding.prototype.handleSelecetRow = function (index, row, evt) {
        //如果没有按下ctrl键和shift键，则直接返回
        if (!evt.ctrlKey && !evt.shiftKey)
            return;
        //if (index == this.editIndex) return;
        //如果是按住了ctrlKey或者shfitKey，则表示要多选了操作了
        var $elem = $(evt.target || evt.srcElement);
        if ($elem.is("input") || $elem.hasClass("fp-record-checkbox"))
            return true;
        var $tr = this.getGridRow(evt);
        //处理单选
        if (evt.ctrlKey) {
            if ($tr.hasClass(this.selectedRowClass)) {
                $tr.removeClass(this.selectedRowClass);
                this.lastSelectedRow = null;
            }
            else {
                $tr.addClass(this.selectedRowClass);
                this.lastSelectedRow = $tr;
            }
        }
        else if (evt.shiftKey) {
            if (this.lastSelectedRow != null) {
                //不可组合使用
                var allRows = $(this.gridRowTr);
                var start = false, stop = false;
                var startElem = null;
                var selected = this.lastSelectedRow.hasClass(this.selectedRowClass);
                for (var index = 0; index < allRows.length; index++) {
                    var elem = allRows.eq(index);
                    if (!start && !stop) {
                        if ($(elem)[0] == $tr[0]) {
                            start = true;
                            startElem = $tr;
                        }
                        else if ($(elem)[0] == this.lastSelectedRow[0]) {
                            start = true;
                            startElem = this.lastSelectedRow;
                        }
                        if (start) {
                            this.selectRow($(elem), selected);
                        }
                    }
                    else if (start && !stop) {
                        if ($(elem)[0] == $($tr)[0] && startElem[0] == this.lastSelectedRow[0]) {
                            start = false;
                            stop = true;
                            this.lastSelectedRow = $(elem);
                            this.selectRow($(elem), selected);
                            this.clearSelectedText();
                        }
                        if ($(elem)[0] == this.lastSelectedRow[0] && startElem[0] == $($tr)[0]) {
                            start = false;
                            stop = true;
                            this.lastSelectedRow = $(elem);
                            this.selectRow($(elem), selected);
                            this.clearSelectedText();
                        }
                        !stop && this.selectRow($(elem), selected);
                    }
                }
                ;
                this.home.stopPropagation(evt);
            }
        }
    };
    /**
     * 选中某一行
     * @param row
     * @param selected
     */
    FPCoding.prototype.selectRow = function (row, selected) {
        if (selected) {
            $(row).addClass(this.selectedRowClass);
        }
        else {
            $(row).removeClass(this.selectedRowClass);
        }
    };
    /**
     * 清除选中的文本
     */
    FPCoding.prototype.clearSelectedText = function () {
        if (mObject.getPropertyValue(document, "selection")) {
            mObject.getPropertyValue(document, "selection").empty();
        }
        else if (window.getSelection) {
            window.getSelection().removeAllRanges();
        }
    };
    /**
     * 获取显示成灰色的格子
     * @param text
     */
    FPCoding.prototype.getGrayCell = function (text) {
        return "<div class='fp-gray-cell'>" + text + "</div>";
    };
    /**
     * 发票相关信息固定
     */
    FPCoding.prototype.getFrozenColumns = function () {
        var _this = this;
        var columns = [{
                field: 'MID', title: '<input type="checkbox" class="fp-record-checkbox-all"/>', fixwidth: 150, width: 150, align: 'left', formatter: function (value, row) {
                    return !row.MIsTop ? "" : "<div style='text-align:center'><input type='checkbox' class='fp-record-checkbox' mid='" + row.MID + "' " + (row.MChecked ? " checked='checked' " : "") + "/></div>";
                }
            },
            {
                field: 'MFapiaoNumber', hidden: this.getColumnHidden('MFapiaoNumber'), title: '<span class="fp-fapiaonumber-title" field="MFapiaoNumber">' + HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号") + '</div>', fixwidth: 300, width: 300, align: 'left', formatter: function (value, row) {
                    return !row.MIsTop ? _this.getGrayCell(value) : ("<a class='fp-view-fapiao' mid='" + row.MID + "'>" + value + "</a>");
                }
            },
            {
                field: 'MBizDate', hidden: this.getColumnHidden('MBizDate'), title: '<span class="fp-bizdate-title" field="MBizDate">' + HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期") + "</div>", fixwidth: 300, width: 300, align: 'center', formatter: function (value, row) {
                    return !row.MIsTop ? _this.getGrayCell(mDate.format(value)) : mDate.format(value);
                }
            },
            {
                field: 'MPSContactName', hidden: this.getColumnHidden('MPSContactName'), title: '<span class="fp-pscontact-title" field="MPSContactName">' + ((this.home.getType() == FPEnum.Sales ? HtmlLang.Write(LangModule.FP, "Buyer", "购买方") : HtmlLang.Write(LangModule.FP, "Saler", "销售方"))) + "</span>", fixwidth: 300, width: 300, align: 'left',
                formatter: function (value, row) {
                    return !row.MIsTop ? _this.getGrayCell(value) : value;
                }
            },
            {
                field: 'MSplit', title: '<span class="fp-splite-title">' + HtmlLang.Write(LangModule.FP, "Split", "拆分") + "</span>", fixwidth: 150, width: 150, align: 'center', formatter: function (value, row) {
                    return "<span class='fp-split-l' mid='" + row.MID + "' mentryid='" + row.MEntryID + "' mnumber='" + row.MFapiaoNumber + "' mindex='" + row.MIndex + "'>&nbsp;</span>";
                }
            },
            {
                field: 'MInventoryName', hidden: this.getColumnHidden('MInventoryName'), title: '<span class="fp-inventory-title">' + HtmlLang.Write(LangModule.FP, "Inventorys", "货物等") + "</span>", fixwidth: 300, width: 300, align: 'left'
            }];
        return columns;
    };
    /**
     * 获取其他的列
     */
    FPCoding.prototype.getComonColumns = function () {
        var leftColumns = [
            {
                field: 'MAmount', hidden: this.getColumnHidden('MAmount'), title: HtmlLang.Write(LangModule.FP, "Amount", "金额"), fixwidth: 300, width: 300, align: 'right', formatter: function (value) {
                    return mMath.toMoneyFormat(value);
                }, editor: this.getAmountEditor()
            },
            {
                field: 'MTaxRate', hidden: this.getColumnHidden('MTaxRate'), title: HtmlLang.Write(LangModule.FP, "TaxRate", "税率"), fixwidth: 150, width: 150, align: 'right', editor: this.getTaxRateEditor(), formatter: this.getTaxRateFormatter()
            },
            {
                field: 'MTaxAmount', hidden: this.getColumnHidden('MTaxAmount'), title: HtmlLang.Write(LangModule.FP, "FPTaxAmount", "税额"), fixwidth: 200, width: 200, align: 'right', formatter: function (value) {
                    return mMath.toMoneyFormat(value);
                }, editor: this.getAmountEditor()
            },
            {
                field: 'MTotalAmount', hidden: this.getColumnHidden('MTotalAmount'), title: HtmlLang.Write(LangModule.FP, "FPTotalAmount", "合计"), fixwidth: 300, width: 300, align: 'right', formatter: function (value) {
                    return mMath.toMoneyFormat(value);
                }, editor: this.getAmountEditor()
            },
            {
                field: 'MContactID', hidden: this.getColumnHidden('MContactID'), title: HtmlLang.Write(LangModule.Common, "Contact", "联系人"), fixwidth: 400, width: 400, align: 'left',
                editor: this.getContactEditor(),
                formatter: this.getContactFormatter()
            },
            {
                field: 'MFastCode', hidden: this.getColumnHidden('MFastCode'), title: '<span class="fp-fastcode-title">' + HtmlLang.Write(LangModule.FP, "MFastCode", "快速码") + '</span>', fixwidth: 240, width: 240, align: 'center', editor: this.getFastCodeEditor(), formatter: this.getFastCodeFormatter()
            },
            {
                field: 'MExplanation', hidden: this.getColumnHidden('MExplanation'), title: HtmlLang.Write(LangModule.FP, "Explanation", "摘要"), fixwidth: 300, width: 300, align: 'center', editor: this.getExplanationEditor()
            },
            {
                field: 'MMerItemID', hidden: this.getColumnHidden('MMerItemID'), title: HtmlLang.Write(LangModule.FP, "MerItem", "商品项目"), fixwidth: 300, width: 300, align: 'left', editor: this.getItemEditor(), formatter: this.getItemFormatter()
            }
        ];
        leftColumns = leftColumns.concat(this.getTrackItemColumn());
        var rightColumns = [
            {
                field: 'MDebitAccount', hidden: this.getColumnHidden('MDebitAccount'), title: HtmlLang.Write(LangModule.GL, "DebitAccount", "借方科目"), fixwidth: 300, width: 300, align: 'center', editor: this.getAccountEditor('MDebitAccount'), formatter: this.getAccountFormatter('MDebitAccount')
            },
            {
                field: 'MCreditAccount', hidden: this.getColumnHidden('MCreditAccount'), title: HtmlLang.Write(LangModule.GL, "CreditAccount", "贷方科目"), fixwidth: 300, width: 300, align: 'center', editor: this.getAccountEditor('MCreditAccount'), formatter: this.getAccountFormatter('MCreditAccount')
            },
            {
                field: 'MTaxAccount', hidden: this.getColumnHidden('MTaxAccount'), title: '<span class="fp-taxaccount-title">' + HtmlLang.Write(LangModule.GL, "TaxAccount", "税科目") + '</span>', fixwidth: 300, width: 300, align: 'center',
                editor: this.getAccountEditor('MTaxAccount'),
                formatter: this.getAccountFormatter('MTaxAccount')
            },
            {
                field: 'MDelete', title: HtmlLang.Write(LangModule.Common, "Operation", "操作"), fixwidth: 150, width: 150, align: 'center', formatter: function (value, row) {
                    return row.MIndex == 0 ? "" : "<span class='vc-delete-href fp-delete-row' >&nbsp;</span>";
                }
            }
        ];
        leftColumns = leftColumns.concat(rightColumns);
        return leftColumns;
    };
    /**
     * 获取跟踪项的title
     */
    FPCoding.prototype.getTrackItemColumn = function (noEditor) {
        var result = [];
        var list = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5];
        for (var i = 0; i < list.length; i++) {
            if (list[i] && list[i].MCheckTypeName) {
                result.push({
                    field: list[i].MCheckTypeColumnName,
                    title: list[i].MCheckTypeName,
                    width: (noEditor ? 100 : 300),
                    fixwidth: (noEditor ? 100 : 300),
                    align: 'left',
                    hidden: this.getColumnHidden(list[i].MCheckTypeColumnName),
                    editor: (noEditor ? null : this.getTrackItemEditor(list[i].MDataList, list[i].MCheckTypeColumnName, list[i].MCheckTypeGroupID)),
                    formatter: this.getTrackItemFormatter(list[i].MDataList, list[i].MCheckTypeColumnName, list[i].MCheckTypeGroupID)
                });
            }
        }
        return result;
    };
    /**
     * 获取设置需要展示的跟踪项的列
     */
    FPCoding.prototype.getTrackItemSettingList = function () {
        var result = [];
        var list = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5];
        var sList = [this.baseData.MSetting.MTrackItem1, this.baseData.MSetting.MTrackItem2, this.baseData.MSetting.MTrackItem3, this.baseData.MSetting.MTrackItem4, this.baseData.MSetting.MTrackItem5];
        for (var i = 0; i < list.length; i++) {
            if (list[i] && list[i].MCheckTypeName) {
                result.push({
                    name: list[i].MCheckTypeName,
                    value: sList[i],
                    field: list[i].MCheckTypeColumnName
                });
            }
        }
        return result;
    };
    /**
     * 拆分前面，货物等后面需要加上一条横线
     */
    FPCoding.prototype.initGridClass = function () {
        var _this = this;
        $("td[field='MSplit']:visible").each(function (index, elem) {
            //有删除的表示是子项
            if ($(elem).parent().find(_this.deleteBtn).length > 0)
                $(elem).addClass(_this.splitRowRightClass);
        });
    };
    /**
     * 初始化表格里面的点击时间
     */
    FPCoding.prototype.initGridEvent = function () {
        var _this = this;
        $(this.checkAll).off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            var checked = $elem.is(":checked");
            if (checked) {
                $(_this.checkBox + "[rstatus!='1']:not([disabled])").attr("checked", "checked");
                _this.lastCheckedBox = $(_this.checkBox).eq(0);
            }
            else {
                $(_this.checkBox + "[rstatus!='1']:not([disabled])").removeAttr("checked");
                _this.lastCheckedBox = null;
            }
            _this.setGridDataChecked(checked);
        });
        $(this.toolbar).off("click.fp").on("click.fp", function (evt) {
            if (!($(_this.codingBody).is(":visible")))
                return;
            if (_this.editIndex != null)
                _this.endEdit();
            _this.hideCodingSetting();
        });
        //点击排序
        $(this.bizDateTitle + "," + this.psContactTitle + "," + this.fapiaoNumberTitle).off("click").on("click", function (evt) {
            if (_this.editIndex != null)
                _this.endEdit();
            _this.sortColumn($(evt.target || evt.srcElement));
        }).tooltip({
            content: HtmlLang.Write(LangModule.FP, "Click2SortColumn", "点击进行排序"),
            position: "top"
        });
        this.mouseDrag2SelectRow();
    };
    /**
     * 初始化格子内部事件
     */
    FPCoding.prototype.initGridCellEvent = function () {
        var _this = this;
        $(this.viewFapiao).off("click").on("click", function (evt) {
            var mid = $(evt.srcElement || evt.target).attr("mid");
            _this.home.viewFapiao(mid, null);
            _this.home.stopPropagation(evt);
        });
        $(this.splitBtn).off("click").on("click", function (evt) {
            _this.splitRow($(evt.target || evt.srcElement), evt);
        });
        $(this.deleteBtn).off("click").on("click", function (evt) {
            _this.removeRow($(evt.target || evt.srcElement));
            evt.stopPropagation();
        });
        $(this.checkBox).off("click.fp").on("click.fp", function (evt) {
            var $elem = $(evt.target || evt.srcElement);
            var checked = $elem.is(":checked");
            var mid = $elem.attr("mid");
            var fapiao = _this.getGridRowData(mid, undefined, undefined, 0)[0];
            fapiao.MChecked = checked;
            //如果按下了shift键并且不是当前勾选框
            if (evt.shiftKey && _this.lastCheckedBox != null && _this.lastCheckedBox[0] != $elem[0]) {
                _this.shiftSelectRow($elem);
            }
            _this.lastCheckedBox = $elem;
            //如果全部勾选了
            _this.needCheckAll();
        });
        //每一个格子都有备注
        var func = function () {
            $(_this.gridCell).each(function (index, ele) {
                var text = $(ele).text();
                if (text.length > 0 && text != " ") {
                    $(ele).tooltip({
                        content: mText.encode(text)
                    });
                }
            });
        };
        window.setTimeout(func, this.timeOutTime);
    };
    /**
     * 设置编辑框里面的tooltip
     */
    FPCoding.prototype.setEditorRowTooltip = function () {
        var editors = this.getRowEditors();
        for (var i = 0; i < editors.length; i++) {
            if (!editors[i] || !editors[i].editor)
                continue;
            var textbox = editors[i].editor.textbox();
            if (!textbox || textbox.length == 0)
                continue;
            var text = textbox.val();
            var div = textbox.closest("td[field]").find(".datagrid-cell:visible");
            if (!div || div.length == 0)
                continue;
            if (text.length == 0)
                div.tooltip("destroy");
            else
                div.tooltip({
                    content: mText.encode(text)
                });
        }
    };
    /**
     * 按照列进行排序
     * @param ele
     */
    FPCoding.prototype.sortColumn = function (ele) {
        var field = ele.attr("field");
        var type = this.getSortByFeild(field);
        type.value = type.value == "desc" ? "asc" : "desc";
        var category = this.home.getType();
        this.sort = field == "MFapiaoNumber" ? "MNumber" : field;
        this.sort = this.sort == "MPSContactName" ? (category == 0 ? "MPContactName" : "MSContactName") : this.sort;
        this.order = type.value;
        this.loadData();
    };
    /**
     * 获取排序类型
     * @param field
     */
    FPCoding.prototype.getSortByFeild = function (field) {
        for (var i = 0; i < this.sortType.length; i++) {
            if (this.sortType[i].name == field)
                return this.sortType[i];
        }
        return null;
    };
    /**
     * 使用shift多选
     */
    FPCoding.prototype.shiftSelectRow = function (from) {
        var _this = this;
        if (this.lastCheckedBox == null)
            return;
        var checked = this.lastCheckedBox.is(":checked");
        var checkboxs = $(this.checkBox);
        var start = false;
        var stop = false;
        var startElem = null;
        checkboxs.each(function (index, elem) {
            if (!start && !stop) {
                if ($(elem)[0] == $(from)[0]) {
                    start = true;
                    startElem = $(from);
                }
                else if ($(elem)[0] == _this.lastCheckedBox[0]) {
                    start = true;
                    startElem = _this.lastCheckedBox;
                }
                if (start) {
                    checked ? $(elem).attr("checked", "checked") : $(elem).removeAttr("checked");
                }
            }
            else if (start && !stop) {
                if ($(elem)[0] == $(from)[0] && startElem[0] == _this.lastCheckedBox[0]) {
                    start = false;
                    stop = true;
                }
                if ($(elem)[0] == _this.lastCheckedBox[0] && startElem[0] == $(from)[0]) {
                    start = false;
                    stop = true;
                }
                checked ? $(elem).attr("checked", "checked") : $(elem).removeAttr("checked");
            }
        });
    };
    /**
     * 判断是否需要勾选全部
     */
    FPCoding.prototype.needCheckAll = function () {
        var checkboxs = $(this.checkBox);
        //如果全部勾选了
        if (checkboxs.filter(function (index, element) {
            return !$(element).is(":checked");
        }).length == 0) {
            $(this.checkAll).attr("checked", "checked");
        }
        else {
            $(this.checkAll).removeAttr("checked");
        }
    };
    /**
     * 设置数据的勾选状态
     * @param checked
     */
    FPCoding.prototype.setGridDataChecked = function (checked) {
        //设置勾选
        this.gridData.forEach(function (value, index) {
            value.MChecked = checked;
        });
    };
    /**
     * 初始化面板事件
     */
    FPCoding.prototype.initEvent = function () {
        var _this = this;
        //标记
        $(this.markBtn).off("click").on("click", function (evt) {
            var boxes = $(_this.checkBox + ":checked");
            var ids = [];
            boxes.each(function (index, elem) {
                ids.push($(elem).attr("mid"));
            });
            ids = ids.distinct();
            if (ids.length == 0) {
                return mDialog.message(HtmlLang.Write(LangModule.FP, "PleaseSelectFapiaos2BeMarkCoding", "请勾选需要标记的发票!"));
            }
            var filter = {
                MFapiaoIDs: ids,
                MCodingStatus: "2"
            };
            var saveFunc = function (ft) {
                _this.home.saveCodingStatus(ft, function (data) {
                    if (data.Success) {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                        _this.refreshData(true);
                    }
                    else {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                    }
                });
            };
            saveFunc(filter);
        });
        //合并生成
        $(this.mergeCreate).off("click").on("click", function () {
            _this.saveCoding(0);
        });
        //保存并全部生成
        $(this.saveNCreateBtn).off("click").on("click", function () {
            _this.saveCoding(1);
        });
        //重置
        $(this.resetBtn).off("click").on("click", function () {
            _this.resetCodingData();
        });
        //搜索
        $(this.searchCodingBtn).off("click").on("click", function () {
            _this.refreshData();
        });
        //输入框加上了回车搜索
        $(this.keywordInput).unbind("keydown.return").bind("keydown.return", "return", function (evt) {
            _this.page = 1;
            _this.loadData();
        });
        $(this.setBtn).tooltip({
            content: $('<div style="width:350px;"></div>'),
            position: "top",
            showEvent: "click",
            width: 360,
            onUpdate: function (content) {
                content.append($(_this.settingDiv).show());
                content.width(360);
            },
            onShow: function () {
                $(_this.setBtn).off("mouseleave");
                _this.initSettingEvent();
            }
        });
        $(this.keywordInput).off("dblclick").on("dblclick", function (evt) {
            $(evt.srcElement || evt.target).val("");
        });
    };
    /**
     * 晃动发票设置按钮
     */
    FPCoding.prototype.shakeSetButton = function () {
        $(this.setBtn).removeClass(this.shakeClass).addClass(this.shakeClass);
    };
    /**
     * 初始化显示列的事件
     */
    FPCoding.prototype.initShowColumnEvent = function () {
        var _this = this;
        $(".fp-select-div").find("span:eq(0)").tooltip({
            content: $('<div style="width:200px;"></div>'),
            position: "top",
            showEvent: "mouseover",
            width: 200,
            onUpdate: function (content) {
                var div = $(_this.tipsDiv1).clone();
                content.append(div.show());
                content.width(200);
            },
            onShow: function () {
                _this.shakeSetButton();
            }
        });
        $(".fp-select-div").find("span:eq(1)").tooltip({
            content: $('<div style="width:200px;"></div>'),
            position: "top",
            showEvent: "mouseover",
            width: 200,
            onUpdate: function (content) {
                var div = $(_this.tipsDiv2).clone();
                content.append(div.show());
                content.width(200);
            },
            onShow: function () {
                _this.shakeSetButton();
            }
        });
        this.initShowPartAllEvent();
    };
    /**
     * 初始化显示全部和显示部分的事件
     */
    FPCoding.prototype.initShowPartAllEvent = function () {
        var _this = this;
        //显示全部列
        $("." + this.allClass + ",." + this.partSelectedClass).off("click").on("click", function (evt) {
            _this.endEdit();
            _this.showAllColumn();
            _this.initShowColumnEvent();
        });
        //显示全部列
        $("." + this.allSelectedClass + ",." + this.partClass).off("click").on("click", function (evt) {
            _this.endEdit();
            _this.showPartColumn();
            _this.initShowColumnEvent();
        });
    };
    /**
     * 初始化元素  高度 宽度 滚动等
     */
    FPCoding.prototype.initDom = function () {
        $(this.codingPartial).height($("body").height() - 155);
        $(this.pager).css({
            top: ($("body").height() - 30) + "px"
        });
        $(this.selectDiv).css({
            top: ($("body").height() - 30) + "px",
            left: (($("body").width() - 75) / 2) + "px"
        });
    };
    /**
     * 展示快速码编辑页面
     */
    FPCoding.prototype.showFastCodeEdit = function (elem, fastCode) {
        var _this = this;
        $(this.fastCodePartialDiv).remove();
        var demo = $(this.fastCodeDemoDiv);
        var partial = demo.clone().removeClass("demo").attr("id", this.fastCodePartialDiv.replace('#', ''));
        partial.insertAfter(demo);
        mDialog.show({
            mContent: "id:" + this.fastCodePartialDiv.replace('#', ''),
            mShowbg: true,
            mHeight: 300,
            mWidth: 600,
            mShowTitle: false,
            mMax: false,
            mCloseCallback: function () {
            },
            mOpenCallback: function () {
                _this.initFastCodeEdit(fastCode);
                _this.initFastCodeEvent(elem);
                $(elem).combogrid("hidePanel");
            }
        });
    };
    /**
     * 初始化快速码设置界面
     */
    FPCoding.prototype.initFastCodeEdit = function (fastCode) {
        var partial = $(this.fastCodeBody + ":visible");
        !!fastCode && $(this.fastCodeItemIDInput, partial).val(fastCode.MID);
        //初始化快速码
        $("input[field='MFastCode']", partial).validatebox({
            required: true
        });
        //初始化描述
        $("input[field='MDescription']", partial).validatebox({
            required: false
        });
        //摘要
        $("input[field='MExplanation']", partial).validatebox({
            required: false
        });
        !!fastCode && $("input[field='MFastCode']", partial).val(fastCode.MFastCode);
        !!fastCode && $("input[field='MDescription']", partial).val(fastCode.MDescription);
        !!fastCode && $("input[field='MExplanation']", partial).val(fastCode.MExplanation);
        //商品项目
        $("input[field='MMerItemID']", partial).combobox({
            textField: "MText",
            valueField: "MItemID",
            srcRequired: true,
            height: 25,
            autoSizePanel: true,
            hideItemKey: "MIsActive",
            hideItemValue: false,
            data: this.baseData.MMerItem,
            formatter: this.getItemComboboxFormatter(),
            onSelect: function (data) {
            },
            onLoadSuccess: function () {
                !!fastCode && !!fastCode.MMerItemID && $("input[field='MMerItemID']", partial).combobox("setValue", fastCode.MMerItemID);
            }
        });
        //科目
        $("input[field='MDebitAccount']", partial).combobox({
            textField: "MFullName",
            valueField: "MItemID",
            srcRequired: true,
            height: 25,
            hideItemKey: "MIsActive",
            hideItemValue: false,
            autoSizePanel: true,
            data: this.baseData.MAccount,
            onSelect: function (data) {
            },
            onLoadSuccess: function () {
                !!fastCode && !!fastCode.MDebitAccount && $("input[field='MDebitAccount']", partial).combobox('setValue', fastCode.MDebitAccount);
            }
        });
        //科目
        $("input[field='MCreditAccount']", partial).combobox({
            textField: "MFullName",
            valueField: "MItemID",
            srcRequired: true,
            height: 25,
            hideItemKey: "MIsActive",
            hideItemValue: false,
            autoSizePanel: true,
            data: this.baseData.MAccount,
            onSelect: function (data) {
            },
            onLoadSuccess: function () {
                !!fastCode && !!fastCode.MCreditAccount && $("input[field='MCreditAccount']", partial).combobox('setValue', fastCode.MCreditAccount);
            }
        });
        //科目
        $("input[field='MTaxAccount']", partial).combobox({
            textField: "MFullName",
            valueField: "MItemID",
            srcRequired: true,
            height: 25,
            hideItemKey: "MIsActive",
            hideItemValue: false,
            autoSizePanel: true,
            data: this.baseData.MAccount,
            onSelect: function (data) {
            },
            onLoadSuccess: function () {
                !!fastCode && !!fastCode.MTaxAccount && $("input[field='MTaxAccount']", partial).combobox('setValue', fastCode.MTaxAccount);
            }
        });
        //跟踪项
        var tracks = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5];
        var _loop_1 = function () {
            if (!tracks[i])
                return "continue";
            var $textTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(2);
            var $valueTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(3);
            if (tracks[i].MCheckTypeName && tracks[i].MCheckTypeName.length > 0) {
                $textTd.text(mText.encode(tracks[i].MCheckTypeName));
                var $input_1 = $valueTd.find("input");
                var defaultValue_1 = mObject.getPropertyValue(fastCode, tracks[i].MCheckTypeColumnName);
                $input_1.show().attr("field", tracks[i].MCheckTypeColumnName).combobox({
                    textField: "text",
                    valueField: "id",
                    srcRequired: true,
                    height: 25,
                    hideItemKey: "MIsActive",
                    hideItemValue: false,
                    autoSizePanel: true,
                    formatter: this_1.getTrackComboboxFormatter(),
                    data: tracks[i].MDataList,
                    onLoadSuccess: function () {
                        !!defaultValue_1 && $input_1.combobox("setValue", defaultValue_1);
                    }
                });
            }
        };
        var this_1 = this;
        for (var i = 0; i < tracks.length; i++) {
            _loop_1();
        }
        //取消红色的提示
        !!fastCode && $(".validatebox-invalid", partial).removeClass("validatebox-invalid");
    };
    /**
     * 初始化设置面板的事件
     */
    FPCoding.prototype.initSettingEvent = function () {
        var _this = this;
        $(this.saveSettingBtn).off("click").on("click", function (evet) {
            _this.endEdit();
            var setting = _this.getCodindSetting();
            _this.home.saveCodingSetting(setting, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "SaveSuccessfully", "保存成功!"));
                    $(_this.setBtn).tooltip("hide");
                    //保存起来
                    _this.baseData.MSetting = setting;
                    _this.showPartColumn();
                    _this.initShowColumnEvent();
                }
                else {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "SaveFailed", "保存失败!"));
                }
            });
        });
        /*
        //checkbox点击的时候，底层动态显示，隐藏列
        var checkboxs = $(this.settingTableFieldValue + ":visible:not([disabled='disabled'])");

        checkboxs.each((index: number, elem: Element) => {
            $(elem).off("click").on("click", (evt: JQueryEventObject) => {
                var $elem = $(evt.target || evt.srcElement);

                var field = $elem.attr("field");

                if ($elem.is(":checked")) {
                    if (this.hiddenColumns.contains(field)) {
                        if (this.tempHiddenColumns.contains(field)) {
                            this.tempHiddenColumns = this.tempHiddenColumns.filter((value: string) => {
                                return value != field;
                            });
                        }

                        $(this.codingTable).datagrid("showColumn", field);

                        if (!this.tempShownColumns.contains(field)) {
                            this.tempShownColumns.push(field);
                        }
                    }
                }
                else {
                    if (!this.hiddenColumns.contains(field)) {
                        if (this.tempShownColumns.contains(field)) {
                            this.tempShownColumns = this.tempShownColumns.filter((value: string) => {
                                return value != field;
                            });
                        }

                        $(this.codingTable).datagrid("hideColumn", field);

                        if (!this.tempHiddenColumns.contains(field)) {
                            this.tempHiddenColumns.push(field);
                        }
                    }
                }

                this.autoMergeRow();
            });
        });
        */
        //关按钮
        $(this.settingCloseBtn).off("click").on("click", function () {
            $(_this.setBtn).tooltip("hide");
            //this.resetTempColumn();
        });
    };
    /**
     * 初始化快速码事件
     */
    FPCoding.prototype.initFastCodeEvent = function (elem) {
        var _this = this;
        $(this.saveFastCodeBtn).off("click").on("click", function () {
            var fastCode = _this.getFastCode();
            if (!!fastCode.MFastCode) {
                _this.home.saveFastCode(fastCode, function (data) {
                    if (data.Success) {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "SaveSuccessfully", "保存成功!"));
                        _this.handleSavedFastCode(fastCode, data.ObjectID, elem);
                        mDialog.close();
                    }
                    else {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "SaveFailed", "保存失败!"));
                    }
                });
            }
            else {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FastCodeIsEmpty", "快速码不可为空!"));
            }
        });
        $(this.fastCodeCloseBtn).off("click").on("click", function () {
            mDialog.close();
        });
    };
    /**
     * 保存快速码
     * @param fastCode
     */
    FPCoding.prototype.handleSavedFastCode = function (fastCode, id, elem) {
        if (!fastCode.MID) {
            fastCode.MID = id;
            //保存起来
            this.baseData.MFastCode.push(fastCode);
        }
        else {
            for (var i = 0; i < this.baseData.MFastCode.length; i++) {
                if (this.baseData.MFastCode[i].MID == fastCode.MID)
                    this.baseData.MFastCode[i] = fastCode;
            }
        }
        //商品项目是否有添加
        var item = this.getItemByFilter(fastCode.MMerItemID);
        if (item == null) {
            var editor = this.getRowEditorByField("MMerItemID");
            this.baseData.MMerItem.push({ MItemID: fastCode.MMerItemID, MText: fastCode.MMerItemIDName });
            editor && editor.editor && editor.editor.target.combobox("loadData", this.baseData.MMerItem);
        }
        var tracks = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5];
        for (var i = 0; i < tracks.length; i++) {
            var track = tracks[i];
            if (!track || !track.MCheckTypeName || !track.MCheckTypeColumnName)
                continue;
            var value = mObject.getPropertyValue(fastCode, track.MCheckTypeColumnName);
            var text = mObject.getPropertyValue(fastCode, track.MCheckTypeColumnName + "Name");
            var exists = this.getTrackByFilter(track.MCheckTypeColumnName, value);
            if (!exists) {
                var trackItem = mObject.getPropertyValue(this.baseData, track.MCheckTypeColumnName);
                trackItem.MDataList.push({
                    parentId: trackItem.MCheckTypeGroupID,
                    id: value,
                    text: text
                });
                var trackEditor = this.getRowEditorByField(track.MCheckTypeColumnName);
                trackEditor && trackEditor.editor && trackEditor.editor.target.combobox("loadData", trackItem.MDataList);
            }
        }
        elem.combogrid("grid").datagrid("loadData", this.baseData.MFastCode);
        this.fillGridWithFastCode(fastCode);
    };
    //显示所有列
    FPCoding.prototype.showAllColumn = function () {
        $(this.setBtn).tooltip("hide");
        $("." + this.allClass).addClass(this.allSelectedClass).removeClass(this.allClass);
        $("." + this.partSelectedClass).addClass(this.partClass).removeClass(this.partSelectedClass);
        var cloneColumns = this.home.cloneArray(this.hiddenColumns);
        //检查是否有隐藏的列
        for (var i = 0; i < cloneColumns.length; i++) {
            this.showColumn(cloneColumns[i]);
        }
        this.resizeColumnWidth();
        this.handleRowClassEvent();
        this.initShowPartAllEvent();
        this.isShowAllColumn = true;
        this.gridEditors = null;
    };
    //按照部分显示
    FPCoding.prototype.showPartColumn = function () {
        $(this.setBtn).tooltip("hide");
        $("." + this.allSelectedClass).addClass(this.allClass).removeClass(this.allSelectedClass);
        $("." + this.partClass).addClass(this.partSelectedClass).removeClass(this.partClass);
        //根据设置来展示列的隐藏与显示
        this.showColumnBySetting(this.baseData.MSetting.MFapiaoNumber, "MFapiaoNumber");
        this.showColumnBySetting(this.baseData.MSetting.MBizDate, "MBizDate");
        this.showColumnBySetting(this.baseData.MSetting.MInventoryName, "MInventoryName");
        this.showColumnBySetting(this.baseData.MSetting.MPSContactName, "MPSContactName");
        this.showColumnBySetting(this.baseData.MSetting.MContactID, "MContactID");
        this.showColumnBySetting(this.baseData.MSetting.MExplanation, "MExplanation");
        this.showColumnBySetting(this.baseData.MSetting.MMerItemID, "MMerItemID");
        this.showColumnBySetting(this.baseData.MSetting.MAmount, "MAmount");
        this.showColumnBySetting(this.baseData.MSetting.MTaxAmount, "MTaxAmount");
        this.showColumnBySetting(this.baseData.MSetting.MTaxRate, "MTaxRate");
        this.showColumnBySetting(this.baseData.MSetting.MTotalAmount, "MTotalAmount");
        this.showColumnBySetting(this.baseData.MSetting.MFastCode, "MFastCode");
        this.showColumnBySetting(this.baseData.MSetting.MTrackItem1, "MTrackItem1");
        this.showColumnBySetting(this.baseData.MSetting.MTrackItem2, "MTrackItem2");
        this.showColumnBySetting(this.baseData.MSetting.MTrackItem3, "MTrackItem3");
        this.showColumnBySetting(this.baseData.MSetting.MTrackItem4, "MTrackItem4");
        this.showColumnBySetting(this.baseData.MSetting.MTrackItem5, "MTrackItem5");
        this.resizeColumnWidth();
        this.handleRowClassEvent();
        this.initShowPartAllEvent();
        this.isShowAllColumn = false;
        this.gridEditors = null;
    };
    /**
     * 根据设置显示combogrid的列
     */
    FPCoding.prototype.showComboGridColumn = function (table) {
        var showAllColumnStatus = $("." + this.allSelectedClass + ":visible").length > 0;
        //先全部显示
        this.baseData.MSetting.MMerItemID == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MMerItemID") : "";
        this.baseData.MSetting.MExplanation == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MExplanation") : "";
        this.baseData.MTrackItem1 && this.baseData.MSetting.MTrackItem1 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem1") : "";
        this.baseData.MTrackItem2 && this.baseData.MSetting.MTrackItem2 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem2") : "";
        this.baseData.MTrackItem3 && this.baseData.MSetting.MTrackItem3 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem3") : "";
        this.baseData.MTrackItem4 && this.baseData.MSetting.MTrackItem4 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem4") : "";
        this.baseData.MTrackItem5 && this.baseData.MSetting.MTrackItem5 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem5") : "";
        $(table).datagrid("resize");
    };
    /**
     * 在做一些操作的时候需要对整个body进行一层遮罩，避免用户点击两次
     */
    FPCoding.prototype.mask = function () {
        $(this.maskDiv).show();
    };
    /**
     * 取消遮罩
     */
    FPCoding.prototype.unmask = function () {
        $(this.maskDiv).hide();
    };
    /**
     * 等待符号
     */
    FPCoding.prototype.wait = function () {
        //等待和取消等待，都调用这个函数
        document.body.style.cursor = "wait";
    };
    /**
    * 等待符号
    */
    FPCoding.prototype.unwait = function () {
        //等待和取消等待，都调用这个函数
        document.body.style.cursor = "default";
    };
    /**
     * 拆分行
     */
    FPCoding.prototype.splitRow = function (elem, event) {
        var rowIndex = this.getRowIndex(elem);
        //如果当前行正在编辑，则需要结束编辑
        if (this.editIndex == rowIndex)
            this.endEdit();
        var row = this.getCheckedRow(elem);
        var cloneRow = this.cloneRow(row);
        cloneRow.MIndex = row.MIndex + 1;
        this.appendRow(cloneRow, rowIndex);
        this.saveCodingRow(cloneRow);
        //合并行
        this.handleRowClassEvent();
    };
    /**
     * 是否包含
     * @param src
     * @param dest
     */
    FPCoding.prototype.contains = function (src, dest, field) {
        field = field || "MRowIndex";
        for (var i = 0; i < src.length; i++) {
            if (src[i] && mObject.getPropertyValue(src[i], field) == mObject.getPropertyValue(dest, field))
                return true;
        }
        return false;
    };
    /**
     * 拼接
     * @param src
     * @param dest
     */
    FPCoding.prototype.concat = function (src, dest) {
        if (!dest || dest.length == 0)
            return src;
        for (var i = 0; i < dest.length; i++) {
            if (!this.contains(src, dest[i]))
                src.push(dest[i]);
        }
        return src;
    };
    /**
     * 处理临时价税合计
     * @param currentRow
     */
    FPCoding.prototype.handleTempTotalAmount = function (currentRow) {
        if (currentRow.MHasTempTotalAmount) {
            currentRow.MTotalAmount = currentRow.MTempTotalAmount;
            currentRow.MHasTempTotalAmount = false;
            currentRow.MTempTotalAmount = null;
        }
    };
    /**
     * 结束之前
     */
    FPCoding.prototype.beforeEndEdit = function (index, row, changes) {
        var currentRow = this.getRowByRowIndex();
        this.handleTempTotalAmount(currentRow);
        var sourceRow = this.getSourceGridRowData(currentRow.MID, currentRow.MEntryID)[0];
        var refreshRow = [];
        //获取联系人和商品项目是否有用户输入的内容
        var contactEditor = this.getRowEditorByField("MContactID");
        var contactText = null;
        if (contactEditor != null) {
            var contactTextbox = contactEditor.editor.textbox();
            contactText = contactTextbox.val().trim();
        }
        var tContact;
        //如果用户没有任何文本，则表示要清空
        if (!contactText || contactText.length == 0) {
            currentRow.MContactID = null;
            currentRow.MContactIDName = null;
            tContact = null;
            //清空也需要同步联系人
            var xRows = this.selectContact(tContact, true);
            refreshRow = this.concat(refreshRow, xRows);
        }
        else {
            var contact = this.getContactByFilter(contactEditor.editor.getValue(), contactText);
            tContact = contact;
            //如果根据iD么有找到，或者找到了不是用户输入的，则表示用户输入的内容是需要新增的
            if (contact == null) {
                currentRow.MContactID = this.newGuid();
                currentRow.MContactIDName = contactText;
                var newContact = {
                    MItemID: currentRow.MContactID,
                    MName: contactText,
                    MContactName: contactText,
                    MIsNew: true
                };
                this.baseData.MContact.push(newContact);
                tContact = newContact;
                var xRows = this.selectContact(tContact, true);
                refreshRow = this.concat(refreshRow, xRows);
            }
            else if (contact.MName != contactText) {
                //这是一种情况，就是用户输入了一个存在的联系人，但是没有触发选择事件
                currentRow.MContactID = contact.MItemID;
                currentRow.MContactIDName = contact.MName;
                var xRows = this.selectContact(contact, true);
                refreshRow = this.concat(refreshRow, xRows);
            }
            else {
                currentRow.MContactID = contact.MItemID;
                currentRow.MContactIDName = contact.MName;
            }
        }
        var itemEditor = this.getRowEditorByField("MMerItemID");
        //如果用户没有显示商品项目，则不需要做编辑
        if (!!itemEditor) {
            var itemTextbox = itemEditor.editor.textbox();
            var itemText = itemTextbox.val().trim();
            var item;
            var tItem = item;
            if (!itemText || itemText.length == 0) {
                currentRow.MMerItemID = null;
                currentRow.MMerItemIDName = null;
                item = null;
                var yRows = this.selectMerItem(item, true);
                refreshRow = this.concat(refreshRow, yRows);
            }
            else {
                item = this.getItemByFilter(itemEditor.editor.getValue(), itemText);
                tItem = item;
                //如果根据iD么有找到，或者找到了不是用户输入的，则表示用户输入的内容是需要新增的
                if (item == null) {
                    currentRow.MMerItemID = this.newGuid();
                    currentRow.MMerItemIDName = itemText;
                    var newItem = {
                        MItemID: currentRow.MMerItemID,
                        MText: itemText,
                        MIsNew: true
                    };
                    this.baseData.MMerItem.push(newItem);
                    tItem = newItem;
                    var yRows = this.selectMerItem(tItem, true);
                    refreshRow = this.concat(refreshRow, yRows);
                }
                else if (item.MText != itemText) {
                    currentRow.MMerItemID = item.MItemID;
                    currentRow.MMerItemIDName = item.MText;
                    var yRows = this.selectMerItem(item, true);
                    refreshRow = this.concat(refreshRow, yRows);
                }
                else {
                    currentRow.MMerItemID = item.MItemID;
                    currentRow.MMerItemIDName = item.MText;
                }
            }
        }
        var trackItems = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, , this.baseData.MTrackItem5];
        for (var i = 0; i < trackItems.length; i++) {
            var track = trackItems[i];
            if (!track)
                continue;
            var editor = this.getRowEditorByField(track.MCheckTypeColumnName);
            if (!editor || !editor.editor)
                continue;
            var value = editor.editor.getValue();
            var text = editor.editor.textbox().val();
            var exists = void 0;
            var tTrack = void 0;
            if (!text || text.length == 0) {
                exists = null;
                tTrack = null;
                mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName, null);
                mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName + "Name", null);
                var zRows = this.selectTrackItem(tTrack, track.MCheckTypeColumnName, true);
                refreshRow = this.concat(refreshRow, zRows);
            }
            else {
                exists = this.getTrackByFilter(track.MCheckTypeColumnName, value, text);
                tTrack = exists;
                //如果不存在，或者有，但是文本对不上
                if (exists == null) {
                    var newTrack = {
                        id: this.newGuid(),
                        text: text,
                        MIsNew: true,
                        parentId: mObject.getPropertyValue(this.baseData, track.MCheckTypeColumnName).MCheckTypeGroupID
                    };
                    mObject.getPropertyValue(this.baseData, track.MCheckTypeColumnName).MDataList.push(newTrack);
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName, newTrack.id);
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName + "Name", text);
                    this.selectTrackItem(newTrack, track.MCheckTypeColumnName);
                    tTrack = newTrack;
                    var zRows = this.selectTrackItem(tTrack, track.MCheckTypeColumnName, true);
                    refreshRow = this.concat(refreshRow, zRows);
                }
                else if (text != exists.text) {
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName, exists.id);
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName + "Name", text);
                    var zRows = this.selectTrackItem(exists, track.MCheckTypeColumnName, true);
                    refreshRow = this.concat(refreshRow, zRows);
                }
                else {
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName, exists.id);
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName + "Name", text);
                }
            }
        }
        //科目选择
        var accounts = ["MDebitAccount", "MCreditAccount", "MTaxAccount"];
        for (var i = 0; i < accounts.length; i++) {
            var field = accounts[i];
            var editor = this.getRowEditorByField(field);
            var accountText = editor.editor.textbox().text();
            var accountId = editor.editor.getValue();
            var account = this.getAccountByFilter(accountId) || {};
            mObject.setPropertyValue(currentRow, field, account.MItemID);
            mObject.setPropertyValue(currentRow, field + "Name", account.MFullName);
        }
        var selectedRowIndex = this.getSelectedIndex();
        if (refreshRow.length > 0) {
            this.refreshRows(refreshRow, currentRow);
            this.setRowSelected(selectedRowIndex);
        }
        else {
            this.saveCodingRow(currentRow);
        }
    };
    /**
     * 保存临时行
     * @param row
     */
    FPCoding.prototype.saveCodingRow = function (row) {
        this.saveCodingRows([row]);
    };
    /**
     * 保存临时行
     * @param row
     */
    FPCoding.prototype.saveCodingRows = function (row) {
        var _this = this;
        if (!this.saveInRealTime)
            return;
        //保存当前行到数据库
        this.home.saveCodingRow(row, function (result) {
            if (result.Success && row.length == 1) {
                _this.setRowID(row, result.ObjectID);
            }
        });
    };
    /**
     * 设置每一行的codingID
     * @param rows
     * @param idString
     */
    FPCoding.prototype.setRowID = function (rows, idString) {
        var ids = idString.split(',');
        for (var i = 0; i < rows.length; i++) {
            rows[i].MItemID = ids[i];
        }
    };
    /**
     * 生成GUID
     */
    FPCoding.prototype.newGuid = function () {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
        }
        return guid;
    };
    /**
     * 隐藏tooltip
     */
    FPCoding.prototype.hideTooltip = function () {
        $(".tooltip-bottom:visible").hide();
    };
    /**
     *
     */
    FPCoding.prototype.hideCodingSetting = function () {
        $(this.setBtn).tooltip("hide");
    };
    /**
     * 开始bianjie
     * @param index
     */
    FPCoding.prototype.beginEdit = function (index) {
        if (this.editIndex == index) {
            return;
        }
        if (this.editIndex != null)
            this.endEdit();
        $(this.codingTable).datagrid("beginEdit", index == undefined ? this.editIndex : index);
        this.editIndex = index;
        //获取当前行
        var currentRow = this.getRowByRowIndex();
        ///在发票 – 进项 – 发票速记界面，对于普票（后台读取发票类型增值税普通发票）的发票数据，
        ///税金科目需默认为空，并置灰，不能进行编辑
        if (currentRow != null && currentRow.MInvoiceType == 1 && currentRow.MType == 0) {
            currentRow.MTaxAccount = null;
            this.setRowEditorDisable(["MTaxAccount"]);
        }
        //如果是第一行，则一些字段不可编辑
        if (currentRow.MIndex == 0) {
            this.setRowEditorDisable();
        }
        else {
            //价税合计不可编辑
            this.setRowEditorDisable(["MTotalAmount", "MTaxRate"]);
            //初始化输入框的输入事件
            this.setAmountInputEvent();
            //初始化联系人、商品项目、跟踪项、科目的keyup时间
        }
        this.setInputEvent();
        this.setRowEditorTips();
        this.initEditorsEvent();
        //合并行
        this.handleRowClassEvent();
    };
    /**
     * 结束编辑
     * @param index
     */
    FPCoding.prototype.endEdit = function () {
        if (this.editIndex == null) {
            return;
        }
        var currentRowSelected = $("tr.datagrid-row:visible[datagrid-row-index='" + this.editIndex + "']").hasClass(this.selectedRowClass);
        $(this.codingTable).datagrid("endEdit", this.editIndex);
        currentRowSelected && this.setRowSelected([this.editIndex]);
        this.editIndex = null;
        this.gridEditors = [];
        //合并行
        this.handleRowClassEvent();
        this.gridEditors = [];
    };
    /**
     * 获取编辑器里面所有的输入框，排除不可编辑的
     */
    FPCoding.prototype.getEditorInputs = function () {
        //获取所有的编辑器
        var editors = this.getRowEditors();
        var inputs = null;
        for (var i = 0; i < editors.length; i++) {
            var input = editors[i].editor.textbox();
            if (input.attr("disabled") == "disabled")
                continue;
            if (inputs == null)
                inputs = input;
            else
                inputs = inputs.add(input);
        }
        return inputs;
    };
    /**
     * 设置编辑行的事件
     */
    FPCoding.prototype.initEditorsEvent = function () {
        var _this = this;
        var inputs = this.getEditorInputs();
        //回车事件
        inputs.unbind("keydown.return").bind("keydown.return", "return", function (evt) {
            _this.goToNextCell(inputs, evt);
        });
        //tab事件
        inputs.unbind("keydown.tab").bind("keydown.tab", "tab", function (evt) {
            _this.goToNextCell(inputs, evt);
        });
        //left事件
        inputs.unbind("keydown.left").bind("keydown.left", "left", function (evt) {
            _this.goToPrevCell(inputs, evt);
        });
        //right事件
        inputs.unbind("keydown.right").bind("keydown.right", "right", function (evt) {
            _this.goToNextCell(inputs, evt);
        });
        //shift+return事件
        inputs.unbind("keydown.shiftreturn").bind("keydown.shiftreturn", "shift+return", function (evt) {
            _this.goToPrevCell(inputs, evt);
        });
        //shift+tab事件
        inputs.unbind("keydown.shifttab").bind("keydown.shifttab", "shift+tab", function (evt) {
            _this.goToPrevCell(inputs, evt);
        });
    };
    /**
     * 到下一个格子
     */
    FPCoding.prototype.goToNextCell = function (inputs, evt) {
        var input = $(evt.srcElement || evt.target);
        var nextCell = this.getNextCellInput(inputs, input);
        if (nextCell == null)
            this.goToNextRow(inputs, evt);
        else {
            nextCell.focus();
        }
        this.home.stopPropagation(evt);
    };
    /**
     * 到前一个格子
     */
    FPCoding.prototype.goToPrevCell = function (inputs, evt) {
        var input = $(evt.srcElement || evt.target);
        var prevCell = this.getPrevCellInput(inputs, input);
        if (prevCell == null)
            this.goToPrevRow(inputs, evt);
        else {
            prevCell.focus();
        }
        this.home.stopPropagation(evt);
    };
    /**
     * 到下一行
     */
    FPCoding.prototype.goToNextRow = function (inputs, evt) {
        if (this.editIndex == this.gridData.length)
            return;
        this.beginEdit(this.editIndex + 1);
        this.getEditorInputs().eq(0).focus();
        this.home.stopPropagation(evt);
    };
    /**
    * 到上一行
    */
    FPCoding.prototype.goToPrevRow = function (inputs, evt) {
        if (this.editIndex == 0)
            return;
        this.beginEdit(this.editIndex - 1);
        this.getEditorInputs().eq(0).focus();
        this.home.stopPropagation(evt);
    };
    /**
     * 获取下一个输入框
     * @param inputs
     * @param input
     */
    FPCoding.prototype.getNextCellInput = function (inputs, input) {
        for (var i = 0; i < inputs.length; i++) {
            if (inputs.eq(i)[0] == input[0])
                return i == inputs.length - 1 ? null : inputs.eq(i + 1);
        }
        return null;
    };
    /**
     * 获取上一个输入框
     * @param inputs
     * @param input
     */
    FPCoding.prototype.getPrevCellInput = function (inputs, input) {
        for (var i = 0; i < inputs.length; i++) {
            if (inputs.eq(i)[0] == input[0])
                return i == 0 ? null : inputs.eq(i - 1);
        }
        return null;
    };
    /**
     * 根据ID获取税率
     */
    FPCoding.prototype.getTaxRateByValue = function (id) {
        for (var i = 0; i < this.baseData.MTaxRate.length; i++) {
            if (this.baseData.MTaxRate[i].MItemID == id)
                return this.baseData.MTaxRate[i];
        }
        return null;
    };
    /**
     * 计算某一行的金额
     */
    FPCoding.prototype.calculateAmount = function (calcuateRate, $elem) {
        var amountEditor = this.getRowEditorByField("MAmount");
        var taxAmountEditor = this.getRowEditorByField("MTaxAmount");
        var taxRateEditor = this.getRowEditorByField("MTaxRate");
        var totalAmountEditor = this.getRowEditorByField("MTotalAmount");
        var amountTextbox = amountEditor.editor.textbox();
        var taxAmountTextbox = taxAmountEditor.editor.textbox();
        var amountTxt = amountTextbox.val();
        var taxTxt = taxAmountTextbox.val();
        var taxRateValue = taxRateEditor.editor.getValue();
        if (amountTxt == "-" || taxTxt == "-")
            return;
        var amount = +amountTxt, tax = +taxTxt;
        //如果是输入的金额，则需要计算税率
        if (calcuateRate && !!taxRateValue) {
            var taxRate = this.getTaxRateByValue(taxRateValue);
            if (taxRate != null) {
                tax = taxRate.MEffectiveTaxRateDecimal * amount;
            }
        }
        var currentRow = this.getRowByRowIndex();
        currentRow.MTaxAmount = tax;
        currentRow.MAmount = amount;
        currentRow.MTotalAmount = tax + amount;
        var allRows = this.getGridRowData(currentRow.MID, currentRow.MEntryID);
        var topRow = allRows.filter(function (value, index) {
            return value.MIndex == 0 && value.MEntryID == currentRow.MEntryID;
        })[0];
        var otherRows = allRows.filter(function (value, index) {
            return value.MIndex != 0 && value.MEntryID == currentRow.MEntryID;
        });
        var subSumAmount = otherRows.sum("MAmount");
        var subSumTaxAmount = otherRows.sum("MTaxAmount");
        var subSumTotalAmount = otherRows.sum("MTotalAmount");
        topRow.MAmount = topRow.MFixedAmount - +subSumAmount;
        topRow.MTaxAmount = topRow.MFixedTaxAmount - +subSumTaxAmount;
        topRow.MTotalAmount = topRow.MFixedTotalAmount - +subSumTotalAmount;
        ($elem == undefined || $elem[0] != amountTextbox[0]) && amountEditor.editor.setValue(currentRow.MAmount);
        ($elem == undefined || $elem[0] != taxAmountTextbox[0]) && taxAmountEditor.editor.setValue(currentRow.MTaxAmount);
        if (!totalAmountEditor) {
            currentRow.MTempTotalAmount = currentRow.MTotalAmount;
            currentRow.MHasTempTotalAmount = true;
        }
        else {
            totalAmountEditor.editor.setValue(currentRow.MTotalAmount);
            currentRow.MTempTotalAmount = null;
            currentRow.MHasTempTotalAmount = false;
        }
        this.refreshRow(topRow);
    };
    /**
     * 联系人输入框失去焦点
     * @param evt
     */
    FPCoding.prototype.contactBlur = function (evt) {
        if ($(evt.target || evt.srcElement).attr("select") == "1") {
            $(evt.target || evt.srcElement).removeAttr("select");
            return true;
        }
        var currentRow = this.getRowByRowIndex();
        var contactText = $(evt.target || evt.srcElement).val();
        var currentContact = this.getContactByFilter(undefined, contactText);
        var input = this.getRowEditorByField("MContactID").editor.target;
        var needSelect = false;
        if (currentContact == null) {
            if (contactText.length == 0) {
                currentContact = {};
            }
            else {
                currentContact = {
                    MItemID: this.newGuid(),
                    MName: contactText,
                    MIsNew: true
                };
                this.baseData.MContact.push(currentContact);
                input.combobox("loadData", this.baseData.MContact);
                input.combobox("setValue", currentContact.MItemID);
            }
            needSelect = true;
        }
        else if (currentContact.MItemID != currentRow.MContactID) {
            needSelect = true;
        }
        currentRow.MContactID = currentContact.MItemID;
        currentRow.MContactIDName = currentContact.MName;
        needSelect && this.selectContact(currentContact);
        return true;
    };
    /**
     * 商品项目blur
     * @param evt
     */
    FPCoding.prototype.itemBlur = function (evt) {
        if ($(evt.target || evt.srcElement).attr("select") == "1") {
            $(evt.target || evt.srcElement).removeAttr("select");
            return true;
        }
        var currentRow = this.getRowByRowIndex();
        var itemText = $(evt.target || evt.srcElement).val();
        var currentItem = this.getItemByFilter(undefined, itemText);
        var needSelect = false;
        if (currentItem == null) {
            if (itemText.length == 0) {
                currentItem = {};
            }
            else {
                currentItem = {
                    MItemID: this.newGuid(),
                    MText: itemText,
                    MIsNew: true
                };
                this.baseData.MMerItem.push(currentItem);
                var input = this.getRowEditorByField("MMerItemID").editor.target;
                input.combobox("loadData", this.baseData.MMerItem);
                input.combobox("setValue", currentItem.MItemID);
            }
            needSelect = true;
        }
        else if (currentItem.MItemID != currentRow.MMerItemID) {
            needSelect = true;
        }
        currentRow.MMerItemID = currentItem.MItemID;
        currentRow.MMerItemIDName = currentItem.MText;
        needSelect && this.selectMerItem(currentItem);
        return true;
    };
    /**
     * 跟踪项blur
     * @param evt
     */
    FPCoding.prototype.trackBlur = function (evt) {
        if ($(evt.target || evt.srcElement).attr("select") == "1") {
            $(evt.target || evt.srcElement).removeAttr("select");
            return true;
        }
        var text = $(evt.srcElement || evt.target).val();
        var columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");
        var currentRow = this.getRowByRowIndex();
        var exists = this.getTrackByFilter(columName, undefined, text);
        var needSelect = false;
        if (exists == null) {
            if (text.length == 0) {
                exists = {};
            }
            else {
                exists = {
                    id: this.newGuid(),
                    text: text,
                    MIsNew: true,
                    parentId: mObject.getPropertyValue(this.baseData, columName).MCheckTypeGroupID
                };
                mObject.getPropertyValue(this.baseData, columName).MDataList.push(exists);
                var input = this.getRowEditorByField(columName).editor.target;
                input.combobox("loadData", mObject.getPropertyValue(this.baseData, columName).MDataList);
                input.combobox("setValue", exists.id);
            }
            needSelect = true;
        }
        else if (exists.id != mObject.getPropertyValue(currentRow, columName)) {
            needSelect = true;
        }
        mObject.setPropertyValue(currentRow, columName, exists.id);
        mObject.setPropertyValue(currentRow, columName + "Name", text);
        needSelect && this.selectTrackItem(exists, columName);
    };
    /**
     * 科目失去焦点
     * @param evt
     */
    FPCoding.prototype.accountBlur = function (evt) {
        var text = $(evt.srcElement || evt.target).val();
        var columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");
        var currentRow = this.getRowByRowIndex();
        var exists = this.getAccountByFilter(undefined, text);
        var needSelect = false;
        if (exists == null) {
            exists = {};
            needSelect = true;
        }
        else if (exists.MItemID != mObject.getPropertyValue(currentRow, columName)) {
            needSelect = true;
        }
        mObject.setPropertyValue(currentRow, columName + "Name", text);
        mObject.setPropertyValue(currentRow, columName, exists.MItemID);
        needSelect && this.selectAccount(exists, columName);
    };
    /**
     * 联系人获取焦点
     * @param evt
     */
    FPCoding.prototype.contactFocus = function (evt) {
        var t = $(evt.srcElement || evt.target);
        var elem = this.getRowEditorByField("MContactID").editor.target;
        if (t.attr("inited") != "1") {
            var text = t.val();
            $(elem).combobox("loadData", this.baseData.MContact);
            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                var contact = this.getContactByFilter(null, text);
                if (contact != null) {
                    $(elem).combobox('setValue', contact.MItemID);
                }
            }
            $(elem).combobox("hidePanel");
            $(elem).combobox("showPanel");
            t.attr("inited", 1);
        }
    };
    /**
     * 商品项目获取焦点
     * @param evt
     */
    FPCoding.prototype.itemFocus = function (evt) {
        var t = $(evt.srcElement || evt.target);
        var elem = this.getRowEditorByField("MMerItemID").editor.target;
        if (t.attr("inited") != "1") {
            var text = t.val();
            $(elem).combobox("loadData", this.baseData.MMerItem);
            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                var item = this.getItemByFilter(null, text);
                if (item != null) {
                    $(elem).combobox('setValue', item.MItemID);
                }
            }
            $(elem).combobox("hidePanel");
            $(elem).combobox("showPanel");
            t.attr("inited", 1);
        }
    };
    /**
     * 跟踪项获取焦点
     * @param evt
     */
    FPCoding.prototype.trackFocus = function (evt) {
        var t = $(evt.srcElement || evt.target);
        var columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");
        var currentRow = this.getRowByRowIndex();
        var tracks = mObject.getPropertyValue(this.baseData, columName);
        var elem = this.getRowEditorByField(columName).editor.target;
        if (t.attr("inited") != "1") {
            var text = t.val();
            $(elem).combobox("loadData", tracks.MDataList);
            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                var item = this.getTrackByFilter(columName, null, text);
                if (item != null) {
                    $(elem).combobox('setValue', item.id);
                }
            }
            $(elem).combobox("hidePanel");
            $(elem).combobox("showPanel");
            t.attr("inited", 1);
        }
    };
    /**
     * 科目获取焦点
     * @param evt
     */
    FPCoding.prototype.accountFocus = function (evt) {
        var t = $(evt.srcElement || evt.target);
        var columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");
        var currentRow = this.getRowByRowIndex();
        var elem = this.getRowEditorByField(columName).editor.target;
        if (t.attr("inited") != "1") {
            var text = t.val();
            $(elem).combobox("loadData", this.baseData.MAccount);
            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                var account = this.getAccountByFilter(null, text);
                if (account != null) {
                    $(elem).combobox('setValue', account.MItemID);
                }
            }
            $(elem).combobox("hidePanel");
            $(elem).combobox("showPanel");
            t.attr("inited", 1);
        }
    };
    /**
     * 摘要输入框失去焦点的时候，需要同步
     */
    FPCoding.prototype.setInputEvent = function () {
        var _this = this;
        var explanationEditor = this.getRowEditorByField("MExplanation");
        if (!!explanationEditor) {
            var explanationTextbox = explanationEditor.editor.textbox();
            //金额输入框
            explanationTextbox.off("blur.fc").on("blur.fc", function (evt) {
                var selectedRowIndex = _this.getSelectedIndex();
                if (selectedRowIndex.length == 0)
                    return true;
                var currentRow = _this.getRowByRowIndex();
                var nowText = $(evt.target || evt.srcElement).val();
                if (currentRow.MExplanation != nowText) {
                    currentRow.MExplanation = nowText;
                    _this.inputExplanation(nowText);
                }
                return true;
            });
        }
        //联系人输入框
        var contactEditor = this.getRowEditorByField("MContactID");
        ///add by 锦友，如果不勾选联系人时，报为空错误
        if (contactEditor != null) {
            var contactTextbox = contactEditor.editor.textbox();
            // 没用到，所以注释了
            // let contactInput = contactEditor.editor.target;
            //联系人combobox加载和失去焦点
            contactTextbox.off("focus.fp").on("focus.fp", function (evt) {
                _this.contactFocus(evt);
                return true;
            }).off("blur.fp").on("blur.fp", function (evt) {
                setTimeout(function () {
                    _this.contactBlur(evt);
                }, 100);
                return true;
            });
        }
        //商品项目
        var merItemEditor = this.getRowEditorByField("MMerItemID");
        if (!!merItemEditor) {
            var merItemTextbox = merItemEditor.editor.textbox();
            merItemTextbox.off("focus.fp").on("focus.fp", function (evt) {
                _this.itemFocus(evt);
                return true;
            }).off("blur.fp").on("blur.fp", function (evt) {
                setTimeout(function () {
                    _this.itemBlur(evt);
                }, 100);
                return true;
            });
        }
        //跟踪项
        var trackItems = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, , this.baseData.MTrackItem5];
        for (var i = 0; i < trackItems.length; i++) {
            var track = trackItems[i];
            if (!track)
                continue;
            var editor = this.getRowEditorByField(track.MCheckTypeColumnName);
            if (!editor || !editor.editor)
                continue;
            editor.editor.textbox().off("focus.fp").on("focus.fp", function (evt) {
                _this.trackFocus(evt);
                return true;
            }).off("blur.fp").on("blur.fp", function (evt) {
                setTimeout(function () {
                    _this.trackBlur(evt);
                }, 100);
                return true;
            });
        }
        //科目选择
        var accounts = ["MDebitAccount", "MCreditAccount", "MTaxAccount"];
        //当前编辑行数据
        var currentRowData = this.getRowByRowIndex();
        for (var i = 0; i < accounts.length; i++) {
            var account = accounts[i];
            if (account == "MTaxAccount" && currentRowData.MInvoiceType == 1 && currentRowData.MType == 0) {
                continue;
            }
            var editor = this.getRowEditorByField(account);
            editor.editor.textbox().off("focus.fp").on("focus.fp", function (evt) {
                _this.accountFocus(evt);
                return true;
            }).off("blur.fp").on("blur.fp", function (evt) {
                _this.accountBlur(evt);
                return true;
            });
        }
        //快速码
        var fastCodeEditor = this.getRowEditorByField("MFastCode");
        if (!!fastCodeEditor) {
            fastCodeEditor.editor.textbox().off("focus.fp").on("focus.fp", function (evt) {
                var t = $(evt.srcElement || evt.target);
                var columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");
                var currentRow = _this.getRowByRowIndex();
                var elem = _this.getRowEditorByField(columName).editor.target;
                if (t.attr("inited") != "1") {
                    var text = t.val();
                    $(elem).combogrid("grid").datagrid("loadData", _this.baseData.MFastCode);
                    if (!text) {
                        $(elem).combogrid("setValue", null);
                    }
                    else {
                        var fastCode = _this.getFastCodeByFilter(null, text);
                        if (fastCode != null) {
                            $(elem).combobox('setValue', fastCode);
                        }
                    }
                    $(elem).combobox("hidePanel");
                    $(elem).combobox("showPanel");
                    t.attr("inited", 1);
                }
            });
        }
    };
    /**
     * 搜索快速码
     * @param evt
     */
    FPCoding.prototype.searchFastCode = function (evt) {
        var _this = this;
        var input = $(evt.target || evt.srcElement);
        var text = input.val();
        $("." + this.fastCodeSelectedClass).removeClass(this.fastCodeSelectedClass);
        if (!text)
            return;
        var hrefs = $(this.fastCodeEdit);
        hrefs.each(function (index, elem) {
            var t = $(elem).text();
            if (t.indexOf(text) >= 0) {
                $(elem).closest("tr").addClass(_this.fastCodeSelectedClass);
                return false;
            }
        });
    };
    /**
     * 当前编辑行里面的事件
     */
    FPCoding.prototype.setAmountInputEvent = function () {
        var _this = this;
        var amountEditor = this.getRowEditorByField("MAmount");
        var taxAmountEditor = this.getRowEditorByField("MTaxAmount");
        var amountTextbox = amountEditor.editor.textbox();
        var taxAmountTextbox = taxAmountEditor.editor.textbox();
        //金额输入框
        amountTextbox.off("keyup").on("keyup", _.debounce(function (evt) {
            _this.calculateAmount(true, $(evt.target || evt.srcElement));
        }, 250));
        //税额输入框
        taxAmountTextbox.off("keyup").on("keyup", _.debounce(function (evt) {
            _this.calculateAmount(false, $(evt.target || evt.srcElement));
        }, 250));
    };
    /**
     * 复制某一行
     */
    FPCoding.prototype.cloneRow = function (row) {
        var newRow = _.clone(row);
        //金额字段都设置为0
        newRow.MTotalAmount = 0;
        newRow.MTaxAmount = 0;
        newRow.MAmount = 0;
        newRow.MItemID = null;
        newRow.MIsTop = false;
        newRow.MIsSplit = true;
        return newRow;
    };
    /**
     * 插入一行
     * @param row
     * @param index
     */
    FPCoding.prototype.appendRow = function (row, currentIndex) {
        row.MRowIndex = currentIndex + 1;
        //插入一行之后，需要对index+1后面的数据进行编号 +1
        for (var i = 0; i < this.gridData.length; i++) {
            if (this.gridData[i].MRowIndex >= row.MRowIndex)
                this.gridData[i].MRowIndex = this.gridData[i].MRowIndex + 1;
            if (this.gridData[i].MID == row.MID && this.gridData[i].MIndex >= row.MIndex)
                this.gridData[i].MIndex = this.gridData[i].MIndex + 1;
        }
        //如果是最后一行，则调用appendRow
        if (row.MRowIndex == this.gridData.length) {
            $(this.codingTable).datagrid("appendRow", row);
        }
        else {
            $(this.codingTable).datagrid("insertRow", {
                index: row.MRowIndex,
                row: row
            });
        }
        //如果是在前面加一行，则需要将当前编辑的下标添加1
        if (this.editIndex >= row.MRowIndex)
            this.editIndex = this.editIndex + 1;
    };
    /**
     * 删除某一行
     * @param index
     */
    FPCoding.prototype.removeRow = function (elem, index) {
        var _this = this;
        if (index == undefined) {
            index = this.getRowIndex(elem);
        }
        this.mask();
        this.endEdit();
        var currentRow = this.getGridRowData(undefined, undefined, undefined, undefined, index)[0];
        var topRow = this.getGridRowData(currentRow.MID, currentRow.MEntryID, currentRow.MFapiaoNumber, 0)[0];
        var handleDeleteRow = function () {
            $(_this.codingTable).datagrid("deleteRow", index);
            var subCount = 0;
            //插入一行之后，需要对index+1后面的数据进行编号 +1
            for (var i = 0; i < _this.gridData.length; i++) {
                if (_this.gridData[i].MID == topRow.MID && _this.gridData[i].MIndex != 0)
                    subCount++;
                if (_this.gridData[i].MRowIndex > index)
                    _this.gridData[i].MRowIndex = _this.gridData[i].MRowIndex - 1;
                if (_this.gridData[i].MID == currentRow.MID && _this.gridData[i].MIndex > currentRow.MIndex)
                    _this.gridData[i].MIndex = _this.gridData[i].MIndex - 1;
            }
            //如果都删掉了，则恢复
            if (subCount == 0) {
                topRow.MAmount = +topRow.MFixedAmount;
                topRow.MTotalAmount = +topRow.MFixedTotalAmount;
                topRow.MTaxAmount = +topRow.MFixedTaxAmount;
            }
            else {
                topRow.MAmount += +currentRow.MAmount;
                topRow.MTotalAmount += +currentRow.MTotalAmount;
                topRow.MTaxAmount += +currentRow.MTaxAmount;
            }
            _this.refreshRow(topRow);
            //合并行
            _this.handleRowClassEvent();
            _this.hideTooltip();
            _this.unmask();
        };
        this.deleteCodingRow(currentRow, handleDeleteRow);
    };
    /**
     * 实时删除行
     * @param row
     */
    FPCoding.prototype.deleteCodingRow = function (row, callback) {
        var _this = this;
        //保存当前行到数据库
        this.home.deleteCodingRow(row, function (result) {
            if (result.Success) {
                $.isFunction(callback) && callback();
            }
            else {
                _this.unmask();
            }
        });
    };
    /**
     * 在新增行或者减少行的时候处理事件
     */
    FPCoding.prototype.handleRowClassEvent = function () {
        this.initGridCellEvent();
        this.initGridClass();
    };
    /**
     * 获取行号
     * @param elem
     */
    FPCoding.prototype.getRowIndex = function (elem) {
        var index = elem.attr("datagrid-row-index");
        if (index != undefined && index != null && index.length > 0)
            return +index;
        else
            return this.getRowIndex(($(elem).parents(".datagrid-row")));
    };
    /**
     * 获取选择的行
     */
    FPCoding.prototype.getCheckedRow = function (elem) {
        var mIndex = +($(elem).attr("mindex"));
        var fapiaoNumber = $(elem).attr("mnumber");
        var mid = $(elem).attr("mid");
        var entryid = $(elem).attr("mentryid");
        var data = this.getGridRowData(mid, entryid, fapiaoNumber, mIndex)[0];
        return data;
    };
    /**
     * 获取选择的行
     */
    FPCoding.prototype.getSelectedRow = function () {
        var _this = this;
        var rows = [];
        var $trs = this.getSelectedTrs();
        var ids = [];
        $trs.each(function (index, elem) {
            var i = +($(elem).attr("datagrid-row-index"));
            i !== _this.editIndex && ids.push(i);
        });
        ids = ids.sort();
        for (var i = 0; i < this.gridData.length; i++) {
            if (ids.contains(this.gridData[i].MRowIndex))
                rows.push(this.gridData[i]);
        }
        return rows;
    };
    /**
     * 获取选中的dom行
     */
    FPCoding.prototype.getSelectedTrs = function () {
        return $("." + this.selectedRowClass + ":visible");
    };
    /**
     * 查找编号
     * @param fapiaoNumber
     * @param index
     */
    FPCoding.prototype.getGridRowData = function (fapiaoID, entryID, fapiaoNumber, index, rowIndex) {
        var result = [];
        for (var i = 0; i < this.gridData.length; i++) {
            if ((!fapiaoID ? true : this.gridData[i].MID === fapiaoID)
                && (!entryID ? true : this.gridData[i].MEntryID === entryID)
                && (index == undefined ? true : this.gridData[i].MIndex === index)
                && (rowIndex == undefined ? true : this.gridData[i].MRowIndex === rowIndex)
                && (!fapiaoNumber ? true : this.gridData[i].MFapiaoNumber === fapiaoNumber)) {
                result.push(this.gridData[i]);
            }
        }
        return result;
    };
    /**
     * 查找编号
     * @param fapiaoNumber
     * @param index
     */
    FPCoding.prototype.getSourceGridRowData = function (fapiaoID, entryID, fapiaoNumber, index, rowIndex) {
        var result = [];
        for (var i = 0; i < this.sourceData.length; i++) {
            if ((!fapiaoID ? true : this.sourceData[i].MID === fapiaoID)
                && (!entryID ? true : this.sourceData[i].MEntryID === entryID)
                && (index == undefined ? true : this.sourceData[i].MIndex === index)
                && (rowIndex == undefined ? true : this.sourceData[i].MRowIndex === rowIndex)
                && (!fapiaoNumber ? true : this.sourceData[i].MFapiaoNumber === fapiaoNumber)) {
                result.push(this.sourceData[i]);
            }
        }
        return result;
    };
    /**
     * 根据列名获取某一列的值
     * @param fieldName
     */
    FPCoding.prototype.getColumnsByField = function (fieldName) {
        var fields = this.allColumns.filter(function (value, index) { return value.field == fieldName; });
        if (fields == null || fields.length == 0)
            return null;
        return fields;
    };
    /**
     * 封装显示列
     * @param fieldName
     */
    FPCoding.prototype.showColumn = function (fieldName, $table) {
        $table = $table || $(this.codingTable);
        if (this.getColumnsByField(fieldName) != null && this.hiddenColumns.contains(fieldName)) {
            $table.datagrid("showColumn", fieldName);
            this.hiddenColumns = this.hiddenColumns.filter(function (value, index, array) {
                return value != fieldName;
            });
            $table.datagrid("resize");
        }
    };
    /**
     * 封装隐藏列
     * @param fieldName
     */
    FPCoding.prototype.hideColumn = function (fieldName, $table) {
        $table = $table || $(this.codingTable);
        if (this.getColumnsByField(fieldName) != null && !this.hiddenColumns.contains(fieldName)) {
            $table.datagrid("hideColumn", fieldName);
            this.hiddenColumns.push(fieldName);
        }
    };
    /**
     * 重置列显示
     */
    FPCoding.prototype.resetTempColumn = function () {
        for (var i = 0; i < this.tempHiddenColumns.length; i++) {
            $(this.codingTable).datagrid("showColumn", this.tempHiddenColumns[i]);
        }
        for (var i = 0; i < this.tempShownColumns.length; i++) {
            $(this.codingBody).datagrid("hideColumn", this.tempShownColumns[i]);
        }
        this.tempHiddenColumns = [];
        this.tempShownColumns = [];
    };
    /**
     * 显示某一行
     * @param columnName
     * @param status
     */
    FPCoding.prototype.showColumnBySetting = function (status, columnName, $table) {
        if (status == 0) {
            this.hideColumn(columnName);
        }
        else {
            this.showColumn(columnName);
        }
    };
    /**
     * 获取codeSetting
     */
    FPCoding.prototype.getCodindSetting = function () {
        var checkboxs = $(this.settingTableFieldValue + ":visible");
        var model = {};
        checkboxs.each(function (index, elem) {
            var value = $(elem).is(":checked") ? 1 : 0;
            var value1 = ($(elem).attr("disabled") == "disabled") ? 1 : 0;
            var field = $(elem).attr("field");
            mObject.setPropertyValue(model, field, value + value1);
        });
        model.MID = this.baseData.MSetting.MID;
        return model;
    };
    /**
     * 获取用户设置的快速码信息
     */
    FPCoding.prototype.getFastCode = function () {
        var partial = $(this.fastCodeBody + ":visible");
        var fastCode = {
            MID: $(this.fastCodeItemIDInput, partial).val(),
            MFastCode: $("input[field='MFastCode']", partial).val(),
            MDescription: $("input[field='MDescription']", partial).val(),
            MExplanation: $("input[field='MExplanation']", partial).val(),
            MMerItemID: $("input[field='MMerItemID']", partial).combobox("getValue"),
            MMerItemIDName: $("input[field='MMerItemID']", partial).combobox("getText"),
            MDebitAccount: $("input[field='MDebitAccount']", partial).combobox("getValue"),
            MCreditAccount: $("input[field='MCreditAccount']", partial).combobox("getValue"),
            MTaxAccount: $("input[field='MTaxAccount']", partial).combobox("getValue"),
            MDebitAccountName: $("input[field='MDebitAccount']", partial).combobox("getText"),
            MCreditAccountName: $("input[field='MCreditAccount']", partial).combobox("getText"),
            MTaxAccountName: $("input[field='MTaxAccount']", partial).combobox("getText"),
        };
        fastCode.MMerItemID = fastCode.MMerItemIDName.length == 0 ? null : fastCode.MMerItemID;
        //跟踪项
        var tracks = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5];
        for (var i = 0; i < tracks.length; i++) {
            if (!tracks[i])
                continue;
            var $valueTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(3);
            if (tracks[i].MCheckTypeName && tracks[i].MCheckTypeName.length > 0) {
                mObject.setPropertyValue(fastCode, tracks[i].MCheckTypeColumnName, $valueTd.find("input[field='" + tracks[i].MCheckTypeColumnName + "']").combobox("getValue"));
                mObject.setPropertyValue(fastCode, tracks[i].MCheckTypeColumnName + "Name", $valueTd.find("input[field='" + tracks[i].MCheckTypeColumnName + "']").combobox("getText"));
            }
        }
        return fastCode;
    };
    /**
     * 显示控制面板
     */
    FPCoding.prototype.initSetting = function () {
        //展示设置字段的显示情况
        var $demoTr = $(this.settingTableDemoTr);
        var $table = $(this.settingTable);
        //显示左侧的数据
        var setting = this.baseData.MSetting;
        var leftPart = [setting.MFapiaoNumber, setting.MBizDate, setting.MPSContactName, setting.MInventoryName, setting.MContactID, setting.MMerItemID, setting.MAmount, setting.MTaxRate, setting.MTaxAmount, setting.MTotalAmount, setting.MFastCode];
        for (var i = 0; i < leftPart.length; i++) {
            var $td = $table.find("tr").eq(i).find("td").eq(1);
            this.setSettingDom($td, leftPart[i]);
        }
        //右边的就比较复杂了，要动态来显示
        //1.第一个显示摘要
        this.setSettingDom($table.find("tr").eq(0).find("td").eq(3), setting.MExplanation);
        //2.后面展示跟踪项
        var tracks = this.getTrackItemSettingList();
        var start = 1;
        for (; start < tracks.length + 1; start++) {
            var $td = $table.find("tr").eq(start).find("td").eq(3);
            this.setSettingDom($td, tracks[start - 1].value, tracks[start - 1].name, tracks[start - 1].field);
        }
        //借方科目
        this.setSettingDom($table.find("tr").eq(start++).find("td").eq(3), setting.MDebitAccount, HtmlLang.Write(LangModule.GL, "DebitAccount", "借方科目"), "MDebitAccount");
        //贷方科目
        this.setSettingDom($table.find("tr").eq(start++).find("td").eq(3), setting.MCreditAccount, HtmlLang.Write(LangModule.GL, "CreditAccount", "贷方科目"), "MCreditAccount");
        //税科目
        this.setSettingDom($table.find("tr").eq(start++).find("td").eq(3), setting.MTaxAccount, HtmlLang.Write(LangModule.GL, "TaxAccount", "税科目"), "MTaxAccount");
    };
    /**
     * 设置元素的值
     * @param $elem
     * @param status
     */
    FPCoding.prototype.setSettingDom = function ($elem, status, name, field) {
        var checkbox = $elem.find("input[type='checkbox']");
        checkbox.parent("td").prev("td").css({ "color": "#444" });
        if (status == 0) {
            checkbox.removeAttr("disabled").removeAttr("checked");
        }
        else if (status == 1) {
            checkbox.removeAttr("disabled").attr("checked", "checked");
        }
        else if (status == 2) {
            checkbox.attr("checked", "checked").attr("disabled", "disabled");
            checkbox.parent("td").prev("td").css({ "color": "#ccc" });
        }
        if (name != undefined) {
            $elem.prev("td").text(mText.encode(name));
        }
        if (field != undefined) {
            checkbox.attr("field", field);
        }
        checkbox.show();
    };
    /**
     * 重置发票coding的数据
     */
    FPCoding.prototype.resetCodingData = function () {
        var _this = this;
        //获取所有的勾选
        if (this.editIndex != null)
            this.endEdit();
        var ids = this.getCheckedFapiaoIds();
        if (ids.length == 0) {
            mDialog.alert(HtmlLang.Write(LangModule.FP, "NoFapiaoCheckedToBeReset", "请勾选要重置的发票!"));
            return false;
        }
        var filter = {
            MFapiaoIDs: ids,
            MFapiaoCategory: this.home.getType()
        };
        //需要确认提醒
        mDialog.confirm(HtmlLang.Write(LangModule.FP, "Sure2ResetCodingData", "您是否确认将所选发票速记数据重置成原始状态?"), function () {
            _this.home.resetCodingData(filter, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                    _this.refreshData();
                }
                else {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                }
            });
        });
    };
    /**
     * 判断是否相同的联系人里面有红字又有蓝字
     * @param codings
     */
    FPCoding.prototype.isCodingHasRedBlue = function (codings) {
        var hashTable = new HashTable();
        for (var i = 0; i < codings.length; i++) {
            var c = hashTable.getValue(codings[i].MContactID);
            //如果存在
            if (c != null) {
                c |= codings[i].MStatus == 4 ? 1 : 2;
                hashTable.add(codings[i].MContactID, c);
            }
            else {
                hashTable.add(codings[i].MContactID, codings[i].MStatus == 4 ? 1 : 2);
            }
        }
        return hashTable.getValues().length > 0 && hashTable.getValues().where("x == 3").length > 0;
    };
    /**
     * 保存发票勾兑
     */
    FPCoding.prototype.saveCoding = function (type) {
        var _this = this;
        type = type == undefined ? 0 : type;
        if (this.editIndex != null)
            this.endEdit();
        var codings = this.getCodingModels();
        if (!this.validateCodingModel(codings))
            return;
        var filter = {
            MCodings: codings,
            MSaveType: type,
            MFapiaoCategory: this.home.getType()
        };
        var func = function () {
            _this.home.saveCoding(filter, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "SaveSuccessfully", "保存成功!"));
                    _this.baseData = null;
                    _this.loadData();
                }
                else {
                    mDialog.confirm(data.Message, function () {
                        _this.baseData = null;
                        _this.refreshData();
                    });
                }
            });
        };
        //如果存在红字，并且用户点击的是合并生成，则需要提醒用户
        if (type == 0 && this.isCodingHasRedBlue(codings))
            mDialog.confirm(this.createVoucherWithRedConfirm, function () { func(); });
        else
            func();
    };
    /**
     * 获取勾选的发票id
     */
    FPCoding.prototype.getCheckedFapiaoIds = function () {
        //获取所有的checkbox
        var checkboxs = $("input.fp-record-checkbox[type='checkbox']:checked");
        var selectedCodings = [];
        var fapiaoIds = [];
        checkboxs.each(function (index, elem) {
            var id = $(elem).attr("mid");
            if (!!id)
                fapiaoIds.push(id);
        });
        return fapiaoIds;
    };
    /**
     * 获取目前表格中的所有需要保存的model
     */
    FPCoding.prototype.getCodingModels = function () {
        var selectedCodings = [];
        var fapiaoIds = this.getCheckedFapiaoIds();
        for (var i = 0; i < this.gridData.length && fapiaoIds.length > 0; i++) {
            if (fapiaoIds.contains(this.gridData[i].MID))
                selectedCodings.push(this.gridData[i]);
        }
        return selectedCodings;
    };
    /**
    * 初始化分页
    */
    FPCoding.prototype.initPage = function (total) {
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
        if (total > 0 && total <= this.rows * (this.page - 1)) {
            this.page = this.page - 1;
            this.loadData();
        }
    };
    /**
    * 选择科目
    */
    FPCoding.prototype.inputExplanation = function (text, noSaveRefresh) {
        var editors = this.getRowEditors();
        var editor = this.getRowEditor(editors, "MExplanation");
        var currentRow = this.getRowByRowIndex();
        var refreshRow = [];
        currentRow.MExplanation = text;
        var selectedRowIndex = this.getSelectedIndex();
        var obj = {
            MExplanation: text
        };
        refreshRow = refreshRow.concat(this.batchCopyRow(obj, ["MExplanation"]));
        if (refreshRow.length > 0 && noSaveRefresh !== true) {
            this.refreshRows(refreshRow);
            this.setRowSelected(selectedRowIndex);
        }
        return refreshRow;
    };
    /**
    * 选择科目
    */
    FPCoding.prototype.selectAccount = function (model, name, noSaveRefresh) {
        model = model || {};
        var editors = this.getRowEditors();
        var editor = this.getRowEditor(editors, name);
        var currentRow = this.getRowByRowIndex();
        var account = null;
        var refreshRow = [];
        var selectedRowIndex = this.getSelectedIndex();
        var obj = {};
        mObject.setPropertyValue(obj, name, model.MItemID);
        mObject.setPropertyValue(obj, name + "Name", model.MFullName);
        mObject.setPropertyValue(currentRow, name, model.MItemID);
        mObject.setPropertyValue(currentRow, name + "Name", model.MFullName);
        refreshRow = refreshRow.concat(this.batchCopyRow(obj, [name, name + "Name"]));
        if (refreshRow.length > 0 && noSaveRefresh !== true) {
            this.refreshRows(refreshRow);
            this.setRowSelected(selectedRowIndex);
        }
        this.setEditorRowTooltip();
        return refreshRow;
    };
    /**
     * 新增跟踪项的时候，需要选择+reload
     * @param model
     */
    FPCoding.prototype.addTrackItem = function (model, name) {
        if (!model)
            return;
        var editor = this.getRowEditorByField(name);
        var currentRow = this.getRowByRowIndex();
        var tracks = mObject.getPropertyValue(this.baseData, name);
        tracks.MDataList.push(model);
        tracks.MDataList.sort(function (a, b) {
            return a.text > b.text ? 1 : -1;
        });
        editor.editor.loadData(tracks.MDataList);
        editor.editor.target.combobox("select", model.id);
        editor.editor.target.combobox("hidePanel");
    };
    /**
     * 添加一个商品项目
     * @param model
     */
    FPCoding.prototype.addMerItem = function (model) {
        if (!model)
            return;
        var editor = this.getRowEditorByField("MMerItemID");
        var currentRow = this.getRowByRowIndex();
        this.baseData.MMerItem = this.baseData.MMerItem || [];
        this.baseData.MMerItem.push(model);
        this.baseData.MContact.sort(function (a, b) {
            return a.MText > b.MText ? 1 : -1;
        });
        editor.editor.loadData(this.baseData.MMerItem);
        editor.editor.target.combobox("select", model.MItemID);
        editor.editor.target.combobox("hidePanel");
    };
    /**
     *
     * @param model
     */
    FPCoding.prototype.addAccount = function (model, name) {
        if (!model)
            return;
        var editor = this.getRowEditorByField(name);
        var currentRow = this.getRowByRowIndex();
        this.baseData.MAccount.push(model);
        for (var i = 0; i < this.baseData.MAccount.length; i++) {
            if (this.baseData.MAccount[i].MItemID == model.MParentID)
                this.baseData.MAccount[i].MIsActive = false;
        }
        this.baseData.MAccount.sort(function (a, b) {
            return a.MCode > b.MCode ? 1 : -1;
        });
        editor.editor.loadData(this.baseData.MAccount);
        editor.editor.target.combobox("select", model.MItemID);
        editor.editor.target.combobox("hidePanel");
    };
    /**
     * 加入一个联系人
     * @param model
     */
    FPCoding.prototype.addContact = function (model) {
        if (!model)
            return;
        var editor = this.getRowEditorByField("MContactID");
        var currentRow = this.getRowByRowIndex();
        this.baseData.MContact.push(model);
        this.baseData.MContact.sort(function (a, b) {
            return a.MName > b.MName ? 1 : -1;
        });
        editor.editor.loadData(this.baseData.MContact);
        editor.editor.target.combobox("select", model.MItemID);
        editor.editor.target.combobox("hidePanel");
    };
    /**
    * 选择跟踪项
    */
    FPCoding.prototype.selectTrackItem = function (model, name, noSaveRefresh) {
        model = model || {};
        var editors = this.getRowEditors();
        var editor = this.getRowEditor(editors, name);
        var account = null;
        var refreshRow = [];
        var selectedRowIndex = this.getSelectedIndex();
        var obj = {};
        mObject.setPropertyValue(obj, name, model.id);
        mObject.setPropertyValue(obj, name + "Name", model.text);
        refreshRow = refreshRow.concat(this.batchCopyRow(obj, [name, name + "Name", "MIsNew"]));
        if (refreshRow.length > 0 && noSaveRefresh !== true) {
            this.refreshRows(refreshRow);
            this.setRowSelected(selectedRowIndex);
        }
        this.setEditorRowTooltip();
        return refreshRow;
    };
    /**
    * 选择联系人的时候需要更新同一条发票的所有联系人信息
    */
    FPCoding.prototype.selectMerItem = function (model, noSaveRefresh) {
        model = model || {};
        var editors = this.getRowEditors();
        var editor = this.getRowEditor(editors, "MMerItem");
        var currentRow = this.getRowByRowIndex();
        var account = null;
        //如果不是费用科目   进项  借方科目 -  库存科目  销项  贷方科目 -  收入科目
        if (!model.MIsExpenseItem) {
            var accounts = this.baseData.MAccount.where("x.MCode =='" + (this.home.getType() == 1 ? model.MInventoryAccountCode : model.MIncomeAccountCode) + "'");
            if (accounts != null && accounts.length > 0) {
                account = accounts[0];
                this.setEditRowValue(editors, this.home.getType() == 1 ? "MDebitAccount" : "MCreditAccount", account.MItemID, account.MFullName);
            }
        }
        else {
            //如果是费用科目  收入科目 贷方  进项发票  费用科目 到借方
            var accounts = this.baseData.MAccount.where("x.MCode =='" + (this.home.getType() == 1 ? model.MCostAccountCode : model.MIncomeAccountCode) + "'");
            if (accounts != null && accounts.length > 0) {
                account = accounts[0];
                this.setEditRowValue(editors, this.home.getType() == 1 ? "MDebitAccount" : "MCreditAccount", account.MItemID, account.MFullName);
            }
        }
        var refreshRow = [];
        var selectedRowIndex = this.getSelectedIndex();
        refreshRow = refreshRow.concat(this.batchCopyRow({ MMerItemID: model.MItemID, MMerItemIDName: model.MText }, ["MMerItemID", "MMerItemIDName", "MIsNew"]));
        if (refreshRow.length > 0 && noSaveRefresh !== true) {
            this.refreshRows(refreshRow);
            this.setRowSelected(selectedRowIndex);
        }
        this.setEditorRowTooltip();
        return refreshRow;
    };
    /**
     * 选择联系人的时候需要更新同一条发票的所有联系人信息
     */
    FPCoding.prototype.selectContact = function (model, noSaveRefresh) {
        model = model || {};
        var editors = this.getRowEditors();
        var editor = this.getRowEditor(editors, "MContactID");
        if (editor != null) {
            editor.editor.textbox().attr("select", "1");
        }
        var currentRow = this.getRowByRowIndex();
        //联系人设置
        this.setEditRowValue(editors, "MTrackItem1", model.MTrackItem1, model.MTrackItem1Name);
        this.setEditRowValue(editors, "MTrackItem2", model.MTrackItem2, model.MTrackItem2Name);
        this.setEditRowValue(editors, "MTrackItem3", model.MTrackItem3, model.MTrackItem3Name);
        this.setEditRowValue(editors, "MTrackItem4", model.MTrackItem4, model.MTrackItem4Name);
        this.setEditRowValue(editors, "MTrackItem5", model.MTrackItem5, model.MTrackItem5Name);
        var account = null;
        //往来科目
        if (!!model.MCCurrentAccountCode) {
            var accounts = this.baseData.MAccount.where("x.MCode =='" + model.MCCurrentAccountCode + "'");
            if (accounts != null && accounts.length > 0) {
                account = accounts[0];
                this.setEditRowValue(editors, this.home.getType() == 0 ? "MDebitAccount" : "MCreditAccount", account.MItemID, account.MFullName);
            }
        }
        var refreshRow = [];
        //获取所有的数据
        for (var i = 0; i < this.gridData.length; i++) {
            var row = this.gridData[i];
            if (row.MRowIndex == this.editIndex)
                continue;
            if (row.MFapiaoNumber == currentRow.MFapiaoNumber) {
                row.MContactID = model.MItemID;
                row.MContactIDName = model.MName;
                row.MTrackItem1 = !row.MTrackItem1 ? model.MTrackItem1 : row.MTrackItem1;
                row.MTrackItem2 = !row.MTrackItem2 ? model.MTrackItem2 : row.MTrackItem2;
                row.MTrackItem3 = !row.MTrackItem3 ? model.MTrackItem3 : row.MTrackItem3;
                row.MTrackItem4 = !row.MTrackItem4 ? model.MTrackItem4 : row.MTrackItem4;
                row.MTrackItem5 = !row.MTrackItem5 ? model.MTrackItem5 : row.MTrackItem5;
                if (account != null) {
                    if (this.home.getType() == 0 && !row.MDebitAccount) {
                        row.MDebitAccount = account.MItemID;
                    }
                    else if (this.home.getType() == 1 && !row.MCreditAccount) {
                        row.MCreditAccount = account.MItemID;
                    }
                }
                refreshRow.push(this.gridData[i]);
            }
        }
        var selectedRowIndex = this.getSelectedIndex();
        refreshRow = refreshRow.concat(this.batchCopyRow({ MContactID: model.MItemID, MContactIDName: model.MName }, ["MContactID", "MContactIDName", "MIsNew"]));
        if (refreshRow.length > 0 && noSaveRefresh !== true) {
            this.refreshRows(refreshRow);
            this.setRowSelected(selectedRowIndex);
        }
        this.setEditorRowTooltip();
        return refreshRow;
    };
    /**
     * 获取科目，根据代码
     * @param code
     */
    FPCoding.prototype.getAccountByCode = function (code) {
        if (code == null || code == undefined || code.length == 0)
            return null;
        var accounts = this.baseData.MAccount.where("x.MCode =='" + code + "'");
        return accounts == null || accounts.length == 0 ? null : accounts[0];
    };
    /**
     * 设置行选中
     * @param index
     */
    FPCoding.prototype.setRowSelected = function (indexs) {
        for (var i = 0; i < indexs.length; i++) {
            var $tr = this.getGridTrByIndex(indexs[i]);
            this.selectRow($tr, true);
        }
    };
    /**
     * 获取选中的发票下标
     */
    FPCoding.prototype.getSelectedIndex = function () {
        var _this = this;
        var indexs = [];
        var selectedRow = this.getSelectedTrs();
        selectedRow.each(function (index, elem) {
            indexs.push(_this.getRowIndex($(elem)));
        });
        return indexs;
    };
    /**
     * 同步数据
     * @param src
     */
    FPCoding.prototype.batchCopyRow = function (src, fields) {
        var codingRows = [];
        var rows = this.getSelectedRow();
        for (var i = 0; i < rows.length; i++) {
            if (this.equal(src, rows[i], fields))
                continue;
            this.copyRow(src, rows[i], fields);
            codingRows.push(rows[i]);
        }
        return codingRows;
    };
    /**
     *
     * @param src
     * @param desc
     * @param fields
     */
    FPCoding.prototype.equal = function (src, dest, fields) {
        if (src == dest)
            return true;
        var equal = true;
        for (var j = 0; j < fields.length; j++) {
            if (mObject.getPropertyValue(src, fields[j]) != mObject.getPropertyValue(dest, fields[j]))
                return false;
        }
        return true;
    };
    /**
     * 批量设置的时候，复制某一行的数据到选中的行
     * @param src
     * @param dest
     */
    FPCoding.prototype.copyRow = function (src, dest, fields) {
        for (var i = 0; i < fields.length; i++) {
            mObject.setPropertyValue(dest, fields[i], mObject.getPropertyValue(src, fields[i]));
        }
    };
    /**
     * 选中联系人以后需要带出跟踪项
     * @param editors
     * @param trackItemName
     * @param trackItemValue
     */
    FPCoding.prototype.setEditRowValue = function (editors, name, value, text) {
        var editor = this.getRowEditor(editors, name);
        if (editor != null && !!value && !editor.editor.getValue()) {
            editor.editor.setValue(value);
            editor.editor.setText && editor.editor.setText(text);
        }
    };
    /**
     * 选择税率的时候，需要计算税额和合计
     * @param rate
     */
    FPCoding.prototype.selectTaxRate = function (rate) {
        if (!rate)
            return;
        var currentRow = this.getRowByRowIndex();
        if (!rate || !rate.MText)
            return;
        var rateAmount = rate.MEffectiveTaxRateDecimal * currentRow.MAmount;
        currentRow.MTaxAmount = rateAmount;
        currentRow.MTotalAmount = rateAmount + currentRow.MAmount;
        //如果没有选择科目，并且税有带科目，则需要设置税的科目税科目
        var code = this.home.getType() == FPEnum.Sales ? rate.MSaleTaxAccountCode : rate.MPurchaseAccountCode;
        if (!currentRow.MTaxAccount && !!code) {
            currentRow.MTaxAccount = this.getTaxRateAccountID(rate);
            var taxEditor = this.getRowEditorByField("MTaxAccount");
            taxEditor.editor.setValue(currentRow.MTaxAccount);
        }
        this.calculateAmount(true);
    };
    /**
     * 根据一个税率，获取税的科目
     * @param rate
     */
    FPCoding.prototype.getTaxRateAccountID = function (rate) {
        //如果没有选择科目，并且税有带科目，则需要设置税的科目税科目
        var code = this.home.getType() == FPEnum.Sales ? rate.MSaleTaxAccountCode : rate.MPurchaseAccountCode;
        if (!code)
            return null;
        var account = this.getAccountByCode(code);
        return account == null ? null : account.MItemID;
    };
    /**
     * 重新刷数据
     * @param rows
     */
    FPCoding.prototype.refreshRows = function (rows, row, nounmask) {
        for (var i = 0; i < rows.length; i++) {
            var rowTr = $(this.codingTable).parent().find("tr[datagrid-row-index=" + rows[i].MRowIndex + "]");
            var srcChked = $(rowTr).find(".fp-record-checkbox").attr("checked");
            $(this.codingTable).datagrid("updateRow", {
                index: rows[i].MRowIndex,
                row: rows[i]
            });
            if (srcChked == "checked") {
                $(rowTr).find(".fp-record-checkbox").attr("checked", "checked");
            }
        }
        if (!!row)
            rows.push(row);
        this.saveCodingRows(rows);
        //合并行
        this.handleRowClassEvent();
    };
    /**
     * 更新某一行
     * @param index
     * @param row
     */
    FPCoding.prototype.refreshRow = function (row) {
        row = row == undefined ? {} : row;
        this.refreshRows([row]);
    };
    /**
     * 根据行号获取某一行的数据
     * @param index
     */
    FPCoding.prototype.getRowByRowIndex = function (index) {
        index = index == undefined ? this.editIndex : index;
        for (var i = 0; i < this.gridData.length; i++) {
            if (this.gridData[i].MRowIndex === index)
                return this.gridData[i];
        }
        return null;
    };
    //----编辑器---
    /**
     * 获取联系人的编辑器
     */
    FPCoding.prototype.getContactEditor = function () {
        var _this = this;
        var editor = {
            type: "addCombobox",
            options: {
                type: 'contact',
                dataOptions: {
                    textField: "MName",
                    valueField: "MItemID",
                    required: false,
                    srcRequired: true,
                    height: 34,
                    hideNoMatch: true,
                    autoSizePanel: true,
                    data: [],
                    formatter: this.getContactComboboxFormatter(),
                    onSelect: function (data) {
                        _this.selectContact(data);
                    },
                    onLoadSuccess: function (data, elem) {
                    }
                },
                addOptions: {
                    hasPermission: true,
                    isReload: false,
                    callback: function (model) {
                        _this.addContact(model);
                    }
                }
            }
        };
        return editor;
    };
    /**
     * 获取商品项目的编辑器
     */
    FPCoding.prototype.getItemEditor = function () {
        var _this = this;
        var editor = {
            type: "addCombobox",
            options: {
                type: 'inventory',
                dataOptions: {
                    textField: "MText",
                    valueField: "MItemID",
                    required: false,
                    srcRequired: true,
                    height: 34,
                    hideNoMatch: true,
                    autoSizePanel: true,
                    data: [],
                    formatter: this.getItemComboboxFormatter(),
                    onSelect: function (data) {
                        _this.selectMerItem(data);
                    },
                    onLoadSuccess: function () {
                    }
                },
                addOptions: {
                    hasPermission: true,
                    isReload: false,
                    callback: function (model) {
                        _this.addMerItem(model);
                    }
                }
            }
        };
        return editor;
    };
    /**
     * 获取税率的编辑框
     */
    FPCoding.prototype.getTaxRateEditor = function () {
        var _this = this;
        var editor = {
            type: "combobox",
            options: {
                textField: "MTaxText",
                valueField: "MItemID",
                required: false,
                srcRequired: true,
                height: 34,
                readonly: true,
                autoSizePanel: true,
                data: this.baseData.MTaxRate,
                onSelect: function (data) {
                    _this.selectTaxRate(data);
                },
                onLoadSuccess: function () {
                }
            }
        };
        return editor;
    };
    /**
     * 获取跟踪项的编辑框
     */
    FPCoding.prototype.getTrackItemEditor = function (tracks, trackName, parentId) {
        var _this = this;
        var editor = {
            type: "addCombobox",
            options: {
                type: 'trackOption',
                dataOptions: {
                    textField: "text",
                    valueField: "id",
                    required: false,
                    srcRequired: true,
                    height: 34,
                    hideNoMatch: true,
                    autoSizePanel: true,
                    data: [],
                    formatter: this.getTrackComboboxFormatter(),
                    onSelect: function (data) {
                        _this.selectTrackItem(data, trackName);
                    },
                    onLoadSuccess: function () {
                    }
                },
                addOptions: {
                    hasPermission: true,
                    url: "/BD/Tracking/CategoryOptionEdit?trackId=" + parentId,
                    isReload: false,
                    callback: function (model) {
                        if (!!model && !!model.id) {
                            _this.addTrackItem(model, trackName);
                        }
                    }
                }
            }
        };
        return editor;
    };
    /**
     * 获取科目编辑框
     */
    FPCoding.prototype.getAccountEditor = function (name) {
        var _this = this;
        var editor = {
            type: "addCombobox",
            options: {
                type: 'account',
                dataOptions: {
                    textField: "MFullName",
                    valueField: "MItemID",
                    required: false,
                    srcRequired: true,
                    height: 34,
                    autoSizePanel: true,
                    data: [],
                    onSelect: function (data) {
                        _this.selectAccount(data, name);
                    },
                    onLoadSuccess: function () {
                    }
                },
                addOptions: {
                    hasPermission: true,
                    IsReload: false,
                    callback: function (model) {
                        _this.addAccount(model, name);
                    }
                }
            }
        };
        return editor;
    };
    /**
     * 获取金额输入编辑框
     */
    FPCoding.prototype.getAmountEditor = function () {
        var editor = {
            type: "numberbox",
            options: {
                precision: 2
            }
        };
        return editor;
    };
    /**
     * 快速码编辑框
     */
    FPCoding.prototype.getFastCodeEditor = function () {
        var _this = this;
        var leftColumns = [
            {
                field: 'MFastCode', title: HtmlLang.Write(LangModule.FP, "MFastCode", "快速码"), width: 80, align: 'center',
                formatter: function (value, record) {
                    return "<a class='fp-fastcode-edit' mid='" + record.MID + "'>" + value + "</a>";
                }
            },
            {
                field: 'MExplanation', title: HtmlLang.Write(LangModule.FP, "Explanation", "摘要"), width: 100, align: 'center', formatter: function (value) { return value; }
            },
            {
                field: 'MMerItemID', title: HtmlLang.Write(LangModule.FP, "MerItem", "商品项目"), width: 100, align: 'center', formatter: this.getItemFormatter()
            }
        ];
        var rightColumns = [
            {
                field: 'MDebitAccount', title: HtmlLang.Write(LangModule.GL, "DebitAccount", "借方科目"), width: 100, align: 'center', formatter: this.getAccountFormatter("MDebitAccount")
            },
            {
                field: 'MCreditAccount', title: HtmlLang.Write(LangModule.GL, "CreditAccount", "贷方科目"), width: 100, align: 'center', formatter: this.getAccountFormatter("MCreditAccount")
            },
            {
                field: 'MTaxAccount', title: HtmlLang.Write(LangModule.GL, "TaxAccount", "税科目"), width: 100, align: 'center', formatter: this.getAccountFormatter("MTaxAccount")
            }
        ];
        var data = [];
        var editor = {
            type: 'addCombogrid',
            options: {
                addOptions: {
                    url: function (elem) {
                        _this.showFastCodeEdit(elem);
                    },
                    height: 300,
                    itemTitle: HtmlLang.Write(LangModule.Common, "NewFastCode", "New Fast Code"),
                    hasPermission: true,
                    callback: function (data) {
                    }
                },
                dataOptions: {
                    panelWidth: function () {
                        return _this.getCombogridPanelWidth();
                    },
                    scrollY: true,
                    resizable: true,
                    auto: true,
                    fitColumns: true,
                    collapsible: true,
                    idField: "MID",
                    textField: "MFastCode",
                    columns: [leftColumns.concat(this.getTrackItemColumn(true)).concat(rightColumns)],
                    data: data,
                    onSelect: function (index, record) {
                        _this.fillGridWithFastCode(record);
                    },
                    onShowPanel: function () {
                        var editor = _this.getRowEditorByField("MFastCode");
                        _this.showComboGridColumn(editor.editor.target.combogrid("grid"));
                        _this.initFastCodeCombogridEvent();
                    }
                }
            }
        };
        return editor;
    };
    /**
     * 联系人下拉框，根据是否是新增的显示不同的颜色
     */
    FPCoding.prototype.getContactComboboxFormatter = function () {
        var _this = this;
        return function (row) {
            return (row && row.MIsNew) ? _this.getNoMatchHtml(row.MName) : row.MName;
        };
    };
    /**
     * 商品项目下拉框，根据是否是新增的显示不同的颜色
     */
    FPCoding.prototype.getItemComboboxFormatter = function () {
        var _this = this;
        return function (row) {
            return (row && row.MIsNew) ? _this.getNoMatchHtml(row.MText) : row.MText;
        };
    };
    /**
      * 跟踪项下拉框，根据是否是新增的显示不同的颜色
      */
    FPCoding.prototype.getTrackComboboxFormatter = function () {
        var _this = this;
        return function (row) {
            return (row && row.MIsNew) ? _this.getNoMatchHtml(row.text) : row.text;
        };
    };
    /**
     * 初始化快速编辑事件
     */
    FPCoding.prototype.initFastCodeCombogridEvent = function () {
        var _this = this;
        $(this.fastCodeEdit).off("click").on("click", function (evt) {
            var $elem = $(evt.target || evt.srcElement);
            var mid = $elem.attr("mid");
            var textBox = _this.getRowEditorByField("MFastCode").editor.target;
            var fastCode = _this.getFastCodeByID(mid);
            _this.showFastCodeEdit(textBox, fastCode);
            _this.home.stopPropagation(evt);
        }).tooltip({ content: HtmlLang.Write(LangModule.FP, "Click2Edit", "点击编辑") });
    };
    /**
     * 获取数据
     * @param id
     */
    FPCoding.prototype.getFastCodeByID = function (id) {
        for (var i = 0; i < this.baseData.MFastCode.length; i++) {
            if (this.baseData.MFastCode[i].MID == id)
                return this.baseData.MFastCode[i];
        }
        return undefined;
    };
    /**
     * 获取快速码的宽度
     */
    FPCoding.prototype.getCombogridPanelWidth = function () {
        return $(this.taxAccountTitle).offset().left + $(this.taxAccountTitle).width() - $(this.fastCodeTitle).offset().left + 20 + 8;
    };
    /**
     * 获取摘要编辑框
     */
    FPCoding.prototype.getExplanationEditor = function () {
        var editor = {
            type: "validatebox",
            options: {
                height: 34
            }
        };
        return editor;
    };
    /**
     *
     * @param id
     * @param text
     */
    FPCoding.prototype.getContactByFilter = function (id, text) {
        for (var i = 0; i < this.baseData.MContact.length; i++) {
            if (id != undefined && id != null && id && id == this.baseData.MContact[i].MItemID) {
                return this.baseData.MContact[i];
            }
            if (text != undefined && text != null && text && text == this.baseData.MContact[i].MName) {
                return this.baseData.MContact[i];
            }
        }
        return null;
    };
    /**
     *
     * @param id
     * @param text
     */
    FPCoding.prototype.getItemByFilter = function (id, text) {
        for (var i = 0; i < this.baseData.MMerItem.length; i++) {
            if (id != undefined && id != null && id && id == this.baseData.MMerItem[i].MItemID) {
                return this.baseData.MMerItem[i];
            }
            if (text != undefined && text != null && text && text == this.baseData.MMerItem[i].MText) {
                return this.baseData.MMerItem[i];
            }
        }
        return null;
    };
    /**
     *
     * @param id
     * @param text
     */
    FPCoding.prototype.getTrackByFilter = function (columnName, id, text) {
        var track = mObject.getPropertyValue(this.baseData, columnName);
        if (!track)
            return null;
        var options = track.MDataList;
        if (!options || options.length == 0)
            return null;
        for (var i = 0; i < options.length; i++) {
            if (id != undefined && id != null && id && id == options[i].id) {
                return options[i];
            }
            if (text != undefined && text != null && text && text == options[i].text) {
                return options[i];
            }
        }
        return null;
    };
    /**
     * 获取科目
     * @param id
     * @param text
     */
    FPCoding.prototype.getAccountByFilter = function (id, text) {
        for (var i = 0; i < this.baseData.MAccount.length; i++) {
            if (id != undefined && id != null && id && id == this.baseData.MAccount[i].MItemID) {
                return this.baseData.MAccount[i];
            }
            if (text != undefined && text != null && text && text == this.baseData.MAccount[i].MFullName) {
                return this.baseData.MAccount[i];
            }
        }
        return null;
    };
    /**
     * 获取科目
     * @param id
     * @param text
     */
    FPCoding.prototype.getFastCodeByFilter = function (id, text) {
        for (var i = 0; i < this.baseData.MFastCode.length; i++) {
            if (id != undefined && id != null && id && id == this.baseData.MFastCode[i].MID) {
                return this.baseData.MFastCode[i];
            }
            if (text != undefined && text != null && text && text == this.baseData.MFastCode[i].MFastCode) {
                return this.baseData.MFastCode[i];
            }
        }
        return null;
    };
    //------格式化-----
    /**
     * 联系人格式化
     */
    FPCoding.prototype.getContactFormatter = function () {
        var _this = this;
        return function (value, row, index) {
            //先处理用户输入的联系人的情况
            var text = !!value ? row.MContactIDName : (_this.isLoadingData ? row.MPSContactName : "");
            var matches = _this.getContactByFilter(value, text);
            //如果有文本，列表中不存在
            if (matches == null) {
                if (!text)
                    return "";
                var newContact = {
                    MName: text,
                    MItemID: !!value ? value : _this.newGuid(),
                    MIsNew: true
                };
                _this.baseData.MContact.push(newContact);
                row.MContactID = newContact.MItemID;
                return _this.getNoMatchHtml(newContact.MName);
            }
            else {
                row.MContactID = matches.MItemID;
                row.MContactIDName = matches.MIsNew ? matches.MName : row.MContactIDName;
                return matches.MIsNew ? _this.getNoMatchHtml(matches.MName) : matches.MName;
            }
        };
    };
    /**
     * 快速码格式化
     */
    FPCoding.prototype.getFastCodeFormatter = function () {
        var _this = this;
        return function (value, row, index) {
            var text = !!value ? row.MMerItemIDName : (_this.isLoadingData ? row.MInventoryName : "");
            var matches = _this.baseData.MFastCode.filter(function (r, index) {
                return r.MID == value;
            });
            return matches != null && matches.length > 0 ? matches[0].MFastCode : "";
        };
    };
    /**
     * 获取未匹配上的文本
     * @param text
     */
    FPCoding.prototype.getNoMatchHtml = function (text) {
        return "<span class='fp-nomatch-text'>" + mText.encode(text || "") + "</span>";
    };
    /**
     * 联系人格式化
     */
    FPCoding.prototype.getItemFormatter = function () {
        var _this = this;
        return function (value, row, index) {
            var text = !!value ? row.MMerItemIDName : (_this.isLoadingData ? row.MInventoryName : "");
            var matches = _this.baseData.MMerItem.filter(function (r, index) {
                return (r.MItemID == value || text === r.MText || text === r.MDesc);
            });
            //如果有文本，列表中不存在
            if (matches.length == 0) {
                if (!text)
                    return "";
                var newItem = {
                    MText: text,
                    MItemID: !!value ? value : _this.newGuid(),
                    MIsNew: true
                };
                _this.baseData.MMerItem.push(newItem);
                row.MMerItemID = newItem.MItemID;
                return _this.getNoMatchHtml(text);
            }
            else {
                row.MMerItemID = matches[0].MItemID;
                row.MMerItemIDName = matches[0].MIsNew ? matches[0].MText : row.MMerItemIDName;
                return matches[0].MIsNew ? _this.getNoMatchHtml(matches[0].MText) : matches[0].MText;
            }
        };
    };
    /**
     * 科目格式化
     */
    FPCoding.prototype.getAccountFormatter = function (name) {
        var _this = this;
        return function (value, row, index) {
            value = value || mObject.getPropertyValue(row, name);
            var matches = _this.baseData.MAccount.filter(function (row, index) {
                return (row.MItemID == value);
            });
            return !!matches && matches.length > 0 ? matches[0].MFullName : "";
        };
    };
    /**
     * 格式化税率
     */
    FPCoding.prototype.getTaxRateFormatter = function () {
        var _this = this;
        return function (value, row, index) {
            var matches = _this.baseData.MTaxRate.filter(function (row, index) {
                return (row.MItemID === value);
            });
            //税率为0,税额不为0特殊处理显示为空针对发票不匹配税率的情况 by hxf
            if (row.MTaxPercent === 0 && row.MTaxAmount != 0) {
                row.MTaxRate = "";
                matches = [];
            }
            if (!!matches && matches.length > 0 && !row.MTaxAccount) {
                if (!(row.MInvoiceType == 1 && row.MType == 0)) {
                    row.MTaxAccount = _this.getTaxRateAccountID(matches[0]);
                }
            }
            return !!matches && matches.length > 0 ? matches[0].MTaxText : "";
        };
    };
    /**
     * 获取跟踪项的格式化
     * @param data
     */
    FPCoding.prototype.getTrackItemFormatter = function (data, trackName, parentId) {
        var _this = this;
        data = data || [];
        return function (value, row, index) {
            var matches = data.filter(function (row, index) {
                return (row.id == value);
            });
            var text = mObject.getPropertyValue(row, trackName + "Name");
            //如果有文本，列表中不存在
            if (matches.length == 0) {
                if (!text)
                    return "";
                var newTrack = {
                    id: value,
                    text: text,
                    MIsNew: true,
                    parentId: parentId
                };
                data.push(newTrack);
                mObject.setPropertyValue(row, trackName, newTrack.id);
                return _this.getNoMatchHtml(text);
            }
            else {
                mObject.setPropertyValue(row, trackName, matches[0].id);
                if (matches[0].MIsNew) {
                    mObject.setPropertyValue(row, trackName + "Name", matches[0].text);
                }
                return matches[0].MIsNew ? _this.getNoMatchHtml(text) : matches[0].text;
            }
        };
    };
    //----填充表格
    FPCoding.prototype.fillGridWithFastCode = function (data) {
        if (!data.MFastCode)
            return;
        var editors = this.getRowEditors();
        for (var i = 0; i < this.allColumns.length; i++) {
            var column = this.allColumns[i];
            var editor = this.getRowEditor(editors, column.field);
            if (mObject.hasOwnProperty(data, column.field) && column.field != "MFastCode") {
                var value = mObject.getPropertyValue(data, column.field);
                var text = mObject.getPropertyValue(data, column.field + "Name");
                editor != null && editor.editor.setValue && editor.editor.setValue(value);
                if (data && mObject.hasOwnProperty(data, column.field + "Name"))
                    editor != null && editor.editor.setText && editor.editor.setText(text);
            }
        }
        var tempCoding = {
            MExplanation: data.MExplanation,
            MMerItemID: data.MMerItemID,
            MMerItemIDName: data.MMerItemIDName,
            MDebitAccount: data.MDebitAccount,
            MCreditAccount: data.MCreditAccount,
            MTaxAccount: data.MTaxAccount,
            MTrackItem1: data.MTrackItem1,
            MTrackItem2: data.MTrackItem2,
            MTrackItem3: data.MTrackItem3,
            MTrackItem4: data.MTrackItem4,
            MTrackItem5: data.MTrackItem5,
        };
        var selectedRowIndex = this.getSelectedIndex();
        var rows = this.getSelectedRow();
        for (var i = 0; i < rows.length; i++) {
            this.copyRow(tempCoding, rows[i], [
                "MExplanation",
                "MMerItemID",
                "MMerItemIDName",
                "MDebitAccount",
                "MCreditAccount",
                "MTaxAccount",
                "MTrackItem1",
                "MTrackItem2",
                "MTrackItem3",
                "MTrackItem4",
                "MTrackItem5"
            ]);
        }
        if (rows.length > 0) {
            this.saveCodingRows(rows);
            this.refreshRows(rows);
            this.setRowSelected(selectedRowIndex);
        }
    };
    /**
     * 获取所有的行编辑
     */
    FPCoding.prototype.getRowEditors = function () {
        //
        var result = [];
        if (this.editIndex != null) {
            if (this.gridEditors != null && this.gridEditors.length > 0)
                return this.gridEditors;
            //找到各个编辑的框
            var editors = $(this.codingTable).datagrid('getEditors', this.editIndex);
            for (var i = 0; i < this.allColumns.length; i++) {
                var column = this.allColumns[i];
                var editor = {};
                if ((!column.hidden || this.isShowAllColumn) && column.editor) {
                    editor.name = column.field;
                    editor.editor = {
                        column: column,
                        type: column.editor.type.replace("addC", "c"),
                        target: $(editors.where("x.field =='" + column.field + "'")[0].target),
                        textbox: function () {
                            return (this.type == "numberbox" || this.type == "validatebox") ? this.target : this.target[this.type]("textbox");
                        },
                        disable: function () {
                            if (!this.target || this.target.length == 0) {
                                return;
                            }
                            if (this.type == "numberbox" || this.type == "validatebox") {
                                this.target.attr("disabled", "disabled");
                                //this.target.validatebox("disableValidation");
                            }
                            else {
                                this.target[this.type]("disable");
                                //this.target[this.type]("textbox").validatebox("disableValidation");
                            }
                        },
                        enable: function () {
                            if (!this.target || this.target.length == 0)
                                return;
                            if (this.type == "numberbox" || this.type == "validatebox") {
                                //this.target.removeAttr("disabled").validatebox("enableValidation");
                            }
                            else {
                                //this.target[this.type]("enable").validatebox("enableValidation");
                            }
                        },
                        validate: function () {
                            if (!this.target || this.target.length == 0)
                                return;
                            this.setValue(this.getValue());
                            if (!this.getValue())
                                this.setText("");
                            if (this.type == "numberbox" || this.type == "validatebox") {
                                return this.target.validatebox("isValid");
                            }
                            else {
                                return this.target[this.type]("isValid");
                            }
                        },
                        getValue: function () {
                            if (!this.target || this.target.length == 0)
                                return;
                            if (this.type == "validatebox")
                                return this.target.val();
                            return this.target[this.type]("getValue");
                        },
                        setValue: function (value) {
                            if (!this.target || this.target.length == 0)
                                return;
                            if (this.type == "validatebox")
                                return this.target.val(value);
                            this.target[this.type]("setValue", value);
                        },
                        setText: function (text) {
                            if (!this.target || this.target.length == 0)
                                return;
                            if (this.type == "numberbox" || this.type == "validatebox") {
                                return this.target.val(text);
                            }
                            else {
                                this.target[this.type]("setText", text);
                            }
                        },
                        setRequired: function (req) {
                            if (!this.target || this.target.length == 0)
                                return;
                            var value = this.getValue();
                            if (this.type == "numberbox" || this.type == "validatebox") {
                                this.target.validatebox({ required: req || this.target[this.type]("options").srcRequired });
                            }
                            else {
                                var data = _.clone(this.getData());
                                this.target[this.type]({ required: req || this.target[this.type]("options").srcRequired });
                                this.target[this.type]("resize", this.column.width);
                                this.loadData(data);
                            }
                            value && this.setValue(value);
                        },
                        loadData: function (data) {
                            if (!this.target || this.target.length == 0)
                                return;
                            this.target[this.type]("loadData", data);
                        },
                        getData: function () {
                            if (this.type == "combobox") {
                                return this.target.combobox("getData");
                            }
                            else if (this.type == "combotree") {
                                return this.target.combo("options").data;
                            }
                        },
                        reload: function () {
                            if (!this.target || this.target.length == 0)
                                return;
                            this.target[this.type]("reload");
                        }
                    };
                    result.push(editor);
                }
            }
            this.gridEditors = result;
        }
        return result;
    };
    /**
     * 获取具体某一个的编辑行
     * @param rows
     * @param field
     */
    FPCoding.prototype.getRowEditor = function (editors, field) {
        for (var i = 0; i < editors.length; i++) {
            if (editors[i].name === field)
                return editors[i];
        }
        return null;
    };
    /**
     * 获取某个编辑框
     * @param field
     */
    FPCoding.prototype.getRowEditorByField = function (field) {
        var editors = this.getRowEditors();
        return this.getRowEditor(editors, field);
    };
    /**
     * 更新个别字段的值
     * @param row
     * @param feilds
     */
    FPCoding.prototype.setRowEditorValue = function (row, fields) {
        var editors = this.getRowEditors();
        for (var i = 0; i < fields.length; i++) {
            var editor = this.getRowEditor(editors, fields[i]);
            editor.editor.setValue(mObject.getPropertyValue(row, fields[i]));
        }
    };
    /**
     * 输入框的设置提醒
     * @param values
     */
    FPCoding.prototype.setRowEditorTips = function (values) {
        this.editorTips = [];
        values = values == undefined ? this.editorTips : values;
        for (var i = 0; i < values.length; i++) {
            var editor = this.getRowEditorByField(values[i].name);
            editor && editor.editor && editor.editor.textbox().tooltip({
                content: values[i].value
            });
        }
    };
    /**
     * 设置某些列不可编辑
     * @param row
     * @param feilds
     */
    FPCoding.prototype.setRowEditorDisable = function (fields) {
        fields = fields == undefined ? this.disableFields : fields;
        var editors = this.getRowEditors();
        for (var i = 0; i < fields.length; i++) {
            var editor = this.getRowEditor(editors, fields[i]);
            !!editor && editor.editor.disable();
        }
    };
    /**
     * 判断一个字段是否隐藏
     * @param field
     */
    FPCoding.prototype.getColumnHidden = function (field) {
        //如果显示全部列
        if (this.isShowAllColumn)
            return false;
        var value = mObject.getPropertyValue(this.baseData.MSetting, field);
        if (+value == this.hidden) {
            if (!this.hiddenColumns.contains(field)) {
                this.hiddenColumns.push(field);
            }
            return true;
        }
        return false;
    };
    /**
     * 处理行宽度
     */
    FPCoding.prototype.resizeColumnWidth = function () {
        this.endEdit();
        $(this.codingTable).datagrid("resizeColumn");
    };
    /**
     *
     * @param rows
     */
    FPCoding.prototype.validateCodingModel = function (rows) {
        var _this = this;
        var result = [];
        //没有一行数据的时候，提醒用户没有可生成凭证的发票
        if (rows.length == 0) {
            mDialog.alert(HtmlLang.Write(LangModule.FP, "NoFapiaoCanBeCreateVoucher", "请勾选要生成凭证的发票"));
            return false;
        }
        else {
            //逐行校验
            for (var i = 0; i < rows.length; i++) {
                var model = {
                    index: rows[i].MRowIndex,
                    row: rows[i],
                    messages: []
                };
                //1.联系人必录
                if (!rows[i].MContactID && rows[i].MIndex == 0) {
                    model.messages.push(HtmlLang.Write(LangModule.FP, "ContactIsEmpty", "联系人不可为空"));
                }
                //合计不可为0 价税合计可以为0
                //if (rows[i].MTotalAmount == 0 && 1 == 2) {
                //    model.messages.push(HtmlLang.Write(LangModule.FP, "TotalAmountIsEmpty", "价税合计不可为空"));
                //}
                //借方科目不可为空
                if (!rows[i].MDebitAccount) {
                    model.messages.push(HtmlLang.Write(LangModule.FP, "DebitAccountIsEmpty", "借方科目不可为空"));
                }
                //贷方科目不可为空
                if (!rows[i].MCreditAccount) {
                    model.messages.push(HtmlLang.Write(LangModule.FP, "CreditAccountIsEmpty", "贷方科目不可为空"));
                }
                //税方科目不可为空
                if (!rows[i].MTaxAccount && rows[i].MTaxAmount > 0) {
                    if (!(rows[i].MInvoiceType == 1 && rows[i].MType == 0)) {
                        model.messages.push(HtmlLang.Write(LangModule.FP, "TaxAccountIsEmpty", "税科目不可为空"));
                    }
                }
                if (model.messages.length > 0)
                    result.push(model);
            }
        }
        //如果有不通过的项目，需要合并起来提醒用户，并且开始编辑对应的行
        if (result.length > 0) {
            var message = "";
            for (var i = 0; i < result.length; i++) {
                message += "<div class='fp-error-message'>" + HtmlLang.Write(LangModule.FP, "Row", "行") + (result[i].index + 1) + ":" + result[i].messages.join(',') + "</div>";
            }
            mDialog.alert(message, function () {
                _this.beginEdit(result[0].index);
            });
        }
        return result.length == 0;
    };
    /**
     * 获取相关的行
     * @param tr
     */
    FPCoding.prototype.getRelatedTrs = function (tr) {
        var rowspan = tr.find("td[field='MID']").attr("rowspan");
        //如果有多行
        if (!!rowspan && +(rowspan) > 0) {
            var num = $(this.splitBtn, tr).attr("mnumber");
            return tr.add(tr.nextAll(":lt(" + ((+rowspan - 1)) + ")"));
        }
        return tr;
    };
    /**
     * 拖拉选中
     */
    FPCoding.prototype.mouseDrag2SelectRow = function () {
        var _this = this;
        $(this.codingBody).off("mousedown.m2").on("mousedown.m2", function (evt) {
            var tableHeaderHeight = $(_this.codingTableHeader).height();
            if ($(evt.srcElement || evt.target)[0] == $(_this.checkAll)[0])
                return true;
            if ($(evt.srcElement || evt.target).hasClass("fp-record-checkbox"))
                return true;
            if (!$(_this.codingBody).is(":visible"))
                return;
            var selList = $(_this.selectTd);
            //checkbox界限
            var checkX = $(_this.checkAll).closest("td[field='MID']:visible").offset().left + $(_this.checkAll).closest("td[field='MID']:visible").width();
            var startX = (evt.clientX);
            var startY = (evt.clientY);
            var bodyX = $(_this.codingBody).offset().left;
            //Y轴的高度取决于表头,而不能写死
            var bodyY = $(_this.codingBody).offset().top + tableHeaderHeight;
            var inventoryX = $(_this.splitTitle).offset().left;
            if (startX > inventoryX)
                return;
            if (startX < bodyX || startY < bodyY)
                return;
            _this.mouseDown = true;
            _this.isSelect = true;
            var _x = null;
            var _y = null;
            var selectFunc = function (evt, x, y) {
                if (_this.isSelect && _this.mouseDown)
                    _this.clearSelectedText();
                _x = x || (evt.clientX);
                _y = y || (evt.clientY);
                if (Math.abs(_x - startX) <= 2 && Math.abs(_y - startY) <= 2)
                    return;
                if (_this.isSelect && _this.mouseDown && $(".select-div").length == 0) {
                    var bgDiv_1 = document.createElement("div");
                    bgDiv_1.style.cssText = "position:absolute;width:0px;height:0px;font-size:0px;margin:0px;padding:0px;background-color:#C3D5ED;z-index:999;opacity:0.0;left:0;top:0;display:none;";
                    bgDiv_1.className = "select-bg-div";
                    $(bgDiv_1).width($("body").width());
                    $(bgDiv_1).height($("body").width());
                    $(_this.codingBody).append(bgDiv_1);
                    var selDiv_1 = document.createElement("div");
                    selDiv_1.style.cssText = "position:absolute;width:0px;height:0px;font-size:0px;margin:0px;padding:0px;border:1px dashed #0099FF;background-color:#C3D5ED;z-index:1000;filter:alpha(opacity:60);opacity:0.6;display:none;";
                    selDiv_1.id = "selectDiv";
                    selDiv_1.className = "select-div";
                    $(_this.codingBody).append(selDiv_1);
                    selDiv_1.style.left = startX + "px";
                    selDiv_1.style.top = startY + "px";
                }
                var selDiv = $(".select-div");
                var bgDiv = $(".select-bg-div");
                if (_this.isSelect && _this.mouseDown) {
                    if (selDiv.is(":hidden")) {
                        selDiv.show();
                    }
                    if (bgDiv.is(":hidden")) {
                        bgDiv.show();
                    }
                    selDiv[0].style.left = Math.min(_x, startX) + "px";
                    selDiv[0].style.top = Math.min(_y, startY) + "px";
                    selDiv[0].style.width = Math.abs(_x - startX) + "px";
                    selDiv[0].style.height = Math.abs(_y - startY) + "px";
                    // ---------------- 关键算法 ---------------------
                    var _l = selDiv[0].offsetLeft, _t = selDiv[0].offsetTop;
                    var _w = selDiv[0].offsetWidth, _h = selDiv[0].offsetHeight;
                    for (var i = 0; i < selList.length; i++) {
                        var scrollTop = $(selList[i]).closest(".datagrid-body").scrollTop();
                        var sl = selList[i].offsetWidth + selList[i].offsetLeft + 20;
                        var st = selList[i].offsetHeight + selList[i].offsetTop + 170 - scrollTop;
                        if (startX < checkX) {
                            if ((sl > startX && 20 < _x && st + tableHeaderHeight > startY && st < _y) || (sl > _x && sl < startX && st < startY && st > _y)) {
                                selList.eq(i).find("input[type='checkbox']:visible").attr("checked", "checked");
                            }
                        }
                        else {
                            if ((sl < startX && sl < _x && st + tableHeaderHeight > startY && st < _y) || (sl < _x && sl < startX && st < startY && st > _y)) {
                                var trs = _this.getRelatedTrs(selList.eq(i).closest("tr.datagrid-row"));
                                trs.each(function (index, elem) {
                                    _this.selectRow($(elem), true);
                                });
                            }
                        }
                    }
                    _this.needCheckAll();
                }
                else {
                    selDiv.remove();
                    bgDiv.remove();
                }
            };
            var scrollInterval = function (evt) {
                var div = $(_this.codingTable).prev("div").find(".datagrid-body");
                if (div.length == 0)
                    return;
                //是否有滚动条
                var scrollHeight = div[0].scrollHeight;
                var height = div.height();
                //有滚动条
                if (scrollHeight > height) {
                    var cx = evt.clientX, cy = evt.clientY;
                    if (cx > div.offset().left && cy > (div.offset().top + height) && _this.scrollIntervalId == null) {
                        _this.scrollIntervalId = window.setInterval(function () {
                            if (div[0].scrollTop + height >= scrollHeight) {
                                window.clearInterval(_this.scrollIntervalId);
                                _this.scrollIntervalId = null;
                                return;
                            }
                            ;
                            div[0].scrollTop = div[0].scrollTop + _this.scrollSpeed * 5;
                            _this.scrollSpeed++;
                            selectFunc(evt);
                        }, 100);
                    }
                }
            };
            $(_this.codingBody).off("mousemove.m2").on("mousemove.m2", function (evt) {
                selectFunc(evt);
                _this.isSelect && _this.mouseDown && scrollInterval(evt);
            });
            $(document).off("keyup.m2").on("keyup.m2", function (evt) {
                if (evt.keyCode == 38 || evt.keyCode == 40 || evt.keyCode == 33 || evt.keyCode == 34) {
                    selectFunc(evt, _x, _y);
                }
            });
            $(document).off("mouseup.m2").on("mouseup.m2", function (evt) {
                _this.isSelect = false;
                _this.mouseDown = false;
                if (_this.scrollIntervalId != null) {
                    window.clearInterval(_this.scrollIntervalId);
                    _this.scrollIntervalId = null;
                }
                var selectDiv = $(".select-div");
                var bgDiv = $(".select-bg-div");
                selectDiv.remove();
                bgDiv.remove();
                selList = null, _x = null, _y = null, startX = null, startY = null;
            });
            return;
        });
    };
    return FPCoding;
}());
//# sourceMappingURL=FPCoding.js.map