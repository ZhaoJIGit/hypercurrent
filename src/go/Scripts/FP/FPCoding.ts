/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />

class FPCoding implements iBusiness {

    private codingPartial: string = ".fp-coding-partial";
    private codingTable: string = ".fp-coding-table";
    private codingBody: string = ".fp-coding-body";
    private codingBodyMergedTd: string = ".fp-coding-body td.datagrid-td-merged:visible";
    private pager: string = ".fp-coding-partial-pager";
    private checkBox: string = ".fp-record-checkbox:visible";

    private selectTd = "tr.datagrid-row td[field='MID']:visible";

    private maskDiv: string = "#fpMaskDiv";

    private selectDiv: string = '.fp-select-div';


    private gridRowTr: string = "tr.datagrid-row";

    private splitRowLeftClass: string = "fp-split-left";
    private splitRowRightClass: string = "fp-split-right";

    private toolbar = ".fp-coding-toolbar,.main-title";

    private timeOutTime: number = 30;

    private mergedTdClass = "fp-merged-td";

    private gridCell = "tr.datagrid-row td .datagrid-cell:visible";

    private viewFapiao: string = ".fp-view-fapiao";
    private markBtn: string = "#markCodingStatus";
    private checkAll: string = ".fp-record-checkbox-all";

    private saveSettingBtn: string = "#btnSaveSetting";
    private settingDiv: string = "#fpSettingDiv";
    private settingBodyDiv: string = ".fp-setting-body";

    private tipsDiv1 = "#fpTipsDiv1";
    private tipsDiv2 = "#fpTipsDiv2";

    private shakeClass = "fp-button-shake";

    private fastCodeItemIDInput = "#fastCodeID";

    private settingTable: string = ".fp-setting-table";
    private settingTableDemoTr: string = ".fp-setting-tr.demo";
    private settingTableFieldName: string = ".fp-setting-field-name";
    private settingTableFieldValue: string = ".fp-setting-field-value input[type='checkbox']";

    private settingCloseBtn: string = ".fp-coding-setting .fp-field-close";
    private fastCodeCloseBtn: string = ".fp-fastcode-close";

    private allSelectedClass: string = "fp-all-selected";
    private partSelectedClass: string = "fp-part-selected";

    private allClass: string = "fp-all";
    private partClass: string = "fp-part";

    private setBtn: string = "#fpSet";

    private selectedRowClass: string = "fp-seleced-row";

    private disSelectedClass: string = "fp-disselect-row";

    private fastCodePartialDiv = "#fastCodeDiv";
    private fastCodeBody = ".fp-fastcode-body";

    private fastCodeEdit = ".fp-fastcode-edit";
    private fastCodeEditClass = "fp-fastcode-edit";

    private fastCodeSelectedClass = "fp-fastcode-selected";

    private fastCodeDemoDiv = ".fp-fastcode-partial";
    private inventoryTitle = ".fp-inventory-title";
    private psContactTitle = ".fp-pscontact-title";
    private bizDateTitle = ".fp-bizdate-title";
    private fapiaoNumberTitle = ".fp-fapiaonumber-title";
    private fastCodeTitle = ".fp-fastcode-title";
    private taxAccountTitle = ".fp-taxaccount-title";

    private saveNCreateBtn: string = "#btnSaveNCreate";
    private mergeCreate: string = "#btnMergeCreate";
    private searchCodingBtn: string = "#bthSearchCoding";

    private saveFastCodeBtn: string = ".fp-fastcode-save:visible";

    private keywordInput = ".fp-coding-keyword";

    private splitBtn = ".fp-split-l:visible";
    private deleteBtn = ".fp-delete-row:visible";

    private mergeFields: string[] = ["MID", "MFapiaoNumber", "MBizDate", "MPSContactName"];

    private disableFields: string[] = ["MAmount", "MTaxRate", "MTotalAmount", "MTaxAmount"];

    private isLoadingData: boolean = false;


    private sortType: NameValueModel[] = [
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

    private editorTips: NameValueModel[] = [
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

    private amountFeilds: string[] = ["MAmount", "MTaxRate", "MTotalAmount"];

    private hiddenColumns: string[] = [];

    private tempHiddenColumns: string[] = [];
    private tempShownColumns: string[] = [];

    private saveInRealTime: boolean = true;

    private gridEditors: DGEditors<FPCodingModel>[] = [];


    private allColumns: DGColumn<FPCodingModel>[] = [];

    private gridData: FPCodingModel[] = [];

    private sourceData: FPCodingModel[] = [];

    private mouseDown: boolean = false;

    private isSelect: boolean = false;

    private lastCheckedBox: JQuery = null;

    private lastSelectedRow: JQuery = null;


    private baseData: FPBaseDataModel;



    public page: number = 1;
    public rows: number = 20;


    private editIndex: number = null;

    private home: FPReconcileHome;
    /**
     * 初始化事件
     */
    public init() {

        this.home = new FPReconcileHome();

        this.initEvent();

        this.initDom();

        window.console && window.console.log("start");
    }

    /**
     * 重置一些基础数据
     */
    public resetData() {

        this.editIndex = null;
        this.gridEditors = [];
        this.gridData = [];
        this.sourceData = [];
        this.mouseDown = false;
        this.isSelect = false;
        this.lastCheckedBox = null;
        this.baseData = null;
        this.hiddenColumns = [];
        $(this.checkAll).removeAttr("checked");
        this.hideCodingSetting();
        $("." + this.allClass).addClass(this.allSelectedClass).removeClass(this.allClass);
        $("." + this.partSelectedClass).addClass(this.partClass).removeClass(this.partSelectedClass);
    }

    /**
     * 保存完成后需要清除已经保存的项目
     * @param codings
     */
    public clearSaveData(codings: FPCodingModel[]) {

    }

    /**
     * 获取数据
     */
    public loadData(dates?: Date[], firstTime?: boolean) {

        this.resetData();

        dates = dates || this.home.getPickedDate();
        //获取过滤
        var filter: FPFapiaoFilterModel = {
            page: this.page,
            rows: this.rows,
            MStartDate: dates[0],
            MEndDate: dates[1],
            MKeyword: $(this.keywordInput).val(),
            MTotalAmount: $(this.keywordInput).val(),
            MFapiaoCategory: this.home.getType()
        };

        this.home.getCodingPageList(filter, (data: FPCodingPageModel) => {

            this.baseData = data.BaseData;

            this.showData(data.Codings);

            this.initSetting();

        });
    }

    /**
     * 对数据进行编号
     */
    public orderGridData() {

        for (var i = 0; i < this.gridData.length; i++) {

            this.gridData[i].MRowIndex = i;
        }
    }


    /**
     * 展示数据到面板
     */
    public showData(data: DataGridJson<FPCodingModel>) {

        this.sourceData = this.home.cloneArray(data.rows);

        this.gridData = data.rows || [];

        this.orderGridData();

        this.allColumns = this.getFrozenColumns().concat(this.getComonColumns());

        this.isLoadingData = true;

        $(this.codingTable).datagrid<FPCodingModel>({
            data: this.gridData,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            //noSelectClass: true,
            checkOnSelect: false,
            showText: true,
            remoteSort: false,
            width: this.home.getGridWith(),
            height: $(this.codingPartial).height() - 45,
            columns: [this.allColumns],
            onEndEdit: (index: number, row: FPCodingModel, changes: FPCodingModel): void => {

                this.beforeEndEdit(index, row, changes);
            },
            onBeforeEdit: (index: number, row: FPCodingModel): void => {

                this.hideTooltip();
            },
            onAfterEdit: (index: number, row: FPCodingModel): void => {

                this.editIndex = null;

                this.hideTooltip();
            },
            onClickRow: (index: number, row: FPCodingModel, evt?: JQueryEventObject): void => {

                this.handleSelecetRow(index, row, evt);
            },
            onDblClickCell: (index: number, field: string, value: any, evt: JQueryEventObject): void => {

                if (field == "MFapiaoNumber" || field == "MBizDate" || field == "MPSContactName") {
                    var td = $(evt.srcElement || evt.target).closest("td[field='" + field + "']:visible");

                    $(this.keywordInput).val(td.text()).focus();
                }
            },
            onClickCell: (index: number, field: string, value: any, evt: JQueryEventObject): void => {

                if (field == "MSplit" || field == "MID" || field == "MDelete") return;

                //如果是合并行，则不需要做处理了
                if (evt.ctrlKey || evt.shiftKey) {
                    return;
                }

                //如果单独点击开票日期和购买方，表示用户需要选中次行
                if (field == "MInventoryName" || field == "MBizDate" || field == "MPSContactName" || field == "MFapiaoNumber") {

                    $("." + this.selectedRowClass).removeClass(this.selectedRowClass);

                    var $tr = this.getGridRow(evt);

                    this.selectRow($tr, true);

                    this.lastSelectedRow = $tr;

                    return;
                }

                //当前行正在编辑，则不需要再编辑了
                if (index == this.editIndex) return;

                this.beginEdit(index);
            },
            onLoadSuccess: () => {

                //合并行
                this.initGridEvent();

                this.initGridCellEvent();

                this.initGridClass();

                this.initShowColumnEvent();

                this.isLoadingData = false;

                //初始化分页控件
                this.initPage(data.total);

                $(this.codingTable).datagrid("resize");
            }
        });
    }

    /**
     * 获取元素附近的tr
     * @param evt
     */
    private getGridRow(evt: JQueryEventObject, elem?: JQuery): JQuery {

        elem = !elem ? $(evt.target || evt.srcElement) : elem;

        return elem.closest("tr.datagrid-row");
    }

    /**
     * 
     * @param index
     */
    private getGridTrByIndex(index: number): JQuery {

        var tr = $("tr.datagrid-row[datagrid-row-index='" + index + "']");

        return tr;
    }

    /**
     * 处理行选择事件
     * @param index
     * @param row
     */
    public handleSelecetRow(index: number, row: FPCodingModel, evt: JQueryEventObject) {

        //如果没有按下ctrl键和shift键，则直接返回
        if (!evt.ctrlKey && !evt.shiftKey) return;

        //if (index == this.editIndex) return;

        //如果是按住了ctrlKey或者shfitKey，则表示要多选了操作了

        var $elem = $(evt.target || evt.srcElement);

        if ($elem.is("input") || $elem.hasClass("fp-record-checkbox")) return true;

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
                var startElem: JQuery = null;

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
                };


                this.home.stopPropagation(evt);

            }
        }

    }
    /**
     * 选中某一行
     * @param row
     * @param selected
     */
    private selectRow(row: JQuery, selected: boolean) {

        if (selected) {
            $(row).addClass(this.selectedRowClass);
        }
        else {
            $(row).removeClass(this.selectedRowClass);
        }
    }

    /**
     * 清除选中的文本
     */
    private clearSelectedText() {

        if (mObject.getPropertyValue(document, "selection")) {
            mObject.getPropertyValue(document, "selection").empty();
        } else if (window.getSelection) {
            window.getSelection().removeAllRanges();
        }
    }

    /**
     * 获取显示成灰色的格子
     * @param text
     */
    private getGrayCell(text: string) {


        return "<div class='fp-gray-cell'>" + text + "</div>";
    }

    /**
     * 发票相关信息固定
     */
    public getFrozenColumns(): DGColumn<FPCodingModel>[] {

        var columns: DGColumn<FPCodingModel>[] = [{
            field: 'MID', title: '<input type="checkbox" class="fp-record-checkbox-all"/>', width: 150, align: 'left', formatter: (value: string, row: FPCodingModel): string => {
                return !row.MIsTop ? "" : "<div style='text-align:center'><input type='checkbox' class='fp-record-checkbox' mid='" + row.MID + "' " + (row.MChecked ? " checked='checked' " : "") + "/></div>";
            }
        },
        {
            field: 'MFapiaoNumber', title: '<span class="fp-fapiaonumber-title" field="MFapiaoNumber">' + HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号") + '</div>', width: 300, align: 'left', formatter: (value: string, row: FPCodingModel): string => {

                return !row.MIsTop ? this.getGrayCell(value) : ("<a class='fp-view-fapiao' mid='" + row.MID + "'>" + value + "</a>");
            }
        },
        {
            field: 'MBizDate', title: '<span class="fp-bizdate-title" field="MBizDate">' + HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期") + "</div>", width: 300, align: 'center', formatter: (value: Date, row: FPCodingModel): string => {

                return !row.MIsTop ? this.getGrayCell(mDate.format(value)) : mDate.format(value);
            }
        },
        {
            field: 'MPSContactName', title: '<span class="fp-pscontact-title" field="MPSContactName">' + ((this.home.getType() == FPEnum.Sales ? HtmlLang.Write(LangModule.FP, "Buyer", "购买方") : HtmlLang.Write(LangModule.FP, "Saler", "销售方"))) + "</span>", width: 300, align: 'center',
            formatter: (value: string, row: FPCodingModel): string => {

                return !row.MIsTop ? this.getGrayCell(value) : value;
            }
        },
        {
            field: 'MSplit', title: HtmlLang.Write(LangModule.FP, "Split", "拆分"), width: 150, align: 'center', formatter: (value: string, row: FPCodingModel): string => {

                return "<span class='fp-split-l' mid='" + row.MID + "' mentryid='" + row.MEntryID + "' mnumber='" + row.MFapiaoNumber + "' mindex='" + row.MIndex + "'>&nbsp;</span>";
            }
        },
        {
            field: 'MInventoryName', title: '<span class="fp-inventory-title">' + HtmlLang.Write(LangModule.FP, "Inventorys", "货物等") + "</span>", width: 300, align: 'left'
        }];

        return columns;
    }

    /**
     * 获取其他的列
     */
    public getComonColumns(): DGColumn<FPCodingModel>[] {

        var leftColumns: DGColumn<FPCodingModel>[] = [
            {
                field: 'MAmount', title: HtmlLang.Write(LangModule.FP, "Amount", "金额"), width: 300, align: 'right', formatter: (value: number): string => {

                    return mMath.toMoneyFormat(value);
                }, editor: this.getAmountEditor()
            },
            {
                field: 'MTaxRate', title: HtmlLang.Write(LangModule.FP, "TaxRate", "税率"), width: 150, align: 'right', editor: this.getTaxRateEditor(), formatter: this.getTaxRateFormatter()
            },
            {
                field: 'MTaxAmount', title: HtmlLang.Write(LangModule.FP, "FPTaxAmount", "税额"), width: 200, align: 'right', formatter: (value: number): string => {

                    return mMath.toMoneyFormat(value);
                }, editor: this.getAmountEditor()
            },
            {
                field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "FPTotalAmount", "合计"), width: 300, align: 'right', formatter: (value: number): string => {

                    return mMath.toMoneyFormat(value);
                }, editor: this.getAmountEditor()
            },
            {
                field: 'MContactID', title: HtmlLang.Write(LangModule.Common, "Contact", "联系人"), width: 400, align: 'left',
                editor: this.getContactEditor(),
                formatter: this.getContactFormatter()
            },
            {
                field: 'MFastCode', title: '<span class="fp-fastcode-title">' + HtmlLang.Write(LangModule.FP, "MFastCode", "快速码") + '</span>', width: 240, align: 'center', editor: this.getFastCodeEditor(), formatter: this.getFastCodeFormatter()
            },
            {
                field: 'MExplanation', title: HtmlLang.Write(LangModule.FP, "Explanation", "摘要"), width: 300, align: 'center', editor: this.getExplanationEditor()
            },

            {
                field: 'MMerItemID', title: HtmlLang.Write(LangModule.FP, "MerItem", "商品项目"), width: 300, align: 'left', editor: this.getItemEditor(), formatter: this.getItemFormatter()
            }];

        leftColumns = leftColumns.concat(this.getTrackItemColumn());

        var rightColumns: DGColumn<FPCodingModel>[] = [
            {
                field: 'MDebitAccount', title: HtmlLang.Write(LangModule.GL, "DebitAccount", "借方科目"), width: 300, align: 'center', editor: this.getAccountEditor('MDebitAccount'), formatter: this.getAccountFormatter('MDebitAccount')
            },
            {
                field: 'MCreditAccount', title: HtmlLang.Write(LangModule.GL, "CreditAccount", "贷方科目"), width: 300, align: 'center', editor: this.getAccountEditor('MCreditAccount'), formatter: this.getAccountFormatter('MCreditAccount')
            },
            {
                field: 'MTaxAccount', title: '<span class="fp-taxaccount-title">' + HtmlLang.Write(LangModule.GL, "TaxAccount", "税科目") + '</span>', width: 300, align: 'center', editor: this.getAccountEditor('MTaxAccount'), formatter: this.getAccountFormatter('MTaxAccount')
            },
            {
                field: 'MDelete', title: HtmlLang.Write(LangModule.Common, "Operation", "操作"), width: 150, align: 'center', formatter: (value: string, row: FPCodingModel): string => {
                    return row.MIndex == 0 ? "" : "<span class='vc-delete-href fp-delete-row' >&nbsp;</span>";
                }
            }
        ];

        leftColumns = leftColumns.concat(rightColumns);

        return leftColumns;
    }


    /**
     * 获取跟踪项的title
     */
    public getTrackItemColumn(noEditor?: boolean): DGColumn<FPCodingModel>[] {

        var result: DGColumn<FPCodingModel>[] = [];

        var list: GLCheckTypeDataModel[] = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5]

        for (var i = 0; i < list.length; i++) {
            if (list[i] && list[i].MCheckTypeName) {
                result.push({
                    field: list[i].MCheckTypeColumnName,
                    title: list[i].MCheckTypeName,
                    width: (noEditor ? 100 : 300),
                    align: 'left',
                    editor: (noEditor ? null : this.getTrackItemEditor(list[i].MDataList, list[i].MCheckTypeColumnName, list[i].MCheckTypeGroupID)),
                    formatter: this.getTrackItemFormatter(list[i].MDataList, list[i].MCheckTypeColumnName, list[i].MCheckTypeGroupID)
                })
            }
        }

        return result;
    }

    /**
     * 获取设置需要展示的跟踪项的列
     */
    public getTrackItemSettingList(): any[] {

        var result: any = [];

        var list: GLCheckTypeDataModel[] = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5]
        var sList: number[] = [this.baseData.MSetting.MTrackItem1, this.baseData.MSetting.MTrackItem2, this.baseData.MSetting.MTrackItem3, this.baseData.MSetting.MTrackItem4, this.baseData.MSetting.MTrackItem5];

        for (var i = 0; i < list.length; i++) {
            if (list[i] && list[i].MCheckTypeName) {
                result.push({
                    name: list[i].MCheckTypeName,
                    value: sList[i],
                    field: list[i].MCheckTypeColumnName
                })
            }
        }
        return result;
    }

    /**
     * 拆分前面，货物等后面需要加上一条横线
     */
    public initGridClass() {

        $("td[field='MInventoryName']:visible").each((index: number, elem: Element) => {
            //有删除的表示是子项
            if ($(elem).parent().find(this.deleteBtn).length > 0)
                $(elem).addClass(this.splitRowLeftClass);
        });
    }



    /**
     * 初始化表格里面的点击时间
     */
    public initGridEvent() {


        $(this.checkAll).off("click").on("click", (evt: JQueryEventObject) => {
            var $elem = $(evt.srcElement || evt.target);
            var checked = $elem.is(":checked");

            if (checked) {
                $(this.checkBox + "[rstatus!='1']:not([disabled])").attr("checked", "checked");
                this.lastCheckedBox = $(this.checkBox).eq(0);
            }
            else {
                $(this.checkBox + "[rstatus!='1']:not([disabled])").removeAttr("checked");
                this.lastCheckedBox = null;
            }

            this.setGridDataChecked(checked);
        });



        $(this.toolbar).off("click.fp").on("click.fp", (evt: JQueryEventObject) => {

            if (!($(this.codingBody).is(":visible"))) return;

            if (this.editIndex != null)
                this.endEdit();

            this.hideCodingSetting();

        });

        //点击排序
        $(this.bizDateTitle + "," + this.psContactTitle + "," + this.fapiaoNumberTitle).off("click").on("click", (evt: JQueryEventObject) => {

            if (this.editIndex != null) this.endEdit();

            this.sortColumn($(evt.target || evt.srcElement));


        }).tooltip({
            content: HtmlLang.Write(LangModule.FP, "Click2SortColumn", "点击进行排序"),
            position: "top"
        });

        this.mouseDrag2SelectRow();
    }

    /**
     * 初始化格子内部事件
     */
    private initGridCellEvent() {

        $(this.viewFapiao).off("click").on("click", (evt: JQueryEventObject) => {

            var mid = $(evt.srcElement || evt.target).attr("mid");

            this.home.viewFapiao(mid, null);

            this.home.stopPropagation(evt);
        });

        $(this.splitBtn).off("click").on("click", (evt: JQueryEventObject) => {

            this.splitRow($(evt.target || evt.srcElement), evt);
        });

        $(this.deleteBtn).off("click").on("click", (evt: JQueryEventObject) => {

            this.removeRow($(evt.target || evt.srcElement));

            evt.stopPropagation();
        });

        $(this.checkBox).off("click.fp").on("click.fp", (evt: JQueryEventObject) => {

            var $elem = $(evt.target || evt.srcElement);

            var checked = $elem.is(":checked");

            var mid = $elem.attr("mid");

            var fapiao = this.getGridRowData(mid, undefined, undefined, 0)[0];

            fapiao.MChecked = checked;

            //如果按下了shift键并且不是当前勾选框
            if (evt.shiftKey && this.lastCheckedBox != null && this.lastCheckedBox[0] != $elem[0]) {

                this.shiftSelectRow($elem);
            }

            this.lastCheckedBox = $elem;

            //如果全部勾选了
            this.needCheckAll();

        });

        //每一个格子都有备注
        var func = () => {

            $(this.gridCell).each((index: number, ele: Element) => {

                var text = $(ele).text();

                if (text.length > 0 && text != " ") {

                    $(ele).tooltip({
                        content: mText.encode(text)
                    });
                }
            });
        }

        window.setTimeout(func, this.timeOutTime);
    }


    /**
     * 设置编辑框里面的tooltip
     */
    private setEditorRowTooltip() {

        var editors = this.getRowEditors();

        for (var i = 0; i < editors.length; i++) {

            if (!editors[i] || !editors[i].editor) continue;

            let textbox = editors[i].editor.textbox();

            if (!textbox || textbox.length == 0) continue;

            let text = textbox.val();

            var div = textbox.closest("td[field]").find(".datagrid-cell:visible");

            if (!div || div.length == 0) continue;

            if (text.length == 0)
                div.tooltip("destroy");
            else div.tooltip({
                content: mText.encode(text)
            });
        }
    }

    /**
     * 按照列进行排序
     * @param ele
     */
    private sortColumn(ele: JQuery) {

        var field = ele.attr("field");

        var type = this.getSortByFeild(field);

        var dir = type.value == "desc" ? 1 : -1;

        //排序
        this.gridData.sort((a: FPCodingModel, b: FPCodingModel): number => {

            let aName: string, bName: string;

            if (field == "MBizDate") {
                aName = mDate.parse(a.MBizDate).getTime().toString() + a.MFapiaoNumber.toString() + a.MPSContactName;
                bName = mDate.parse(b.MBizDate).getTime().toString() + b.MFapiaoNumber.toString() + b.MPSContactName;
            }
            else if (field == "MFapiaoNumber") {
                aName = a.MFapiaoNumber.toString() + mDate.parse(a.MBizDate).getTime().toString() + a.MPSContactName;
                bName = b.MFapiaoNumber.toString() + mDate.parse(b.MBizDate).getTime().toString() + b.MPSContactName;
            }
            else if (field == "MPSContactName") {
                aName = a.MPSContactName + mDate.parse(a.MBizDate).getTime().toString() + a.MFapiaoNumber.toString();
                bName = b.MPSContactName + mDate.parse(b.MBizDate).getTime().toString() + b.MFapiaoNumber.toString();
            }

            if (aName == bName) return a.MIndex > b.MIndex ? 1 : -1;

            return aName > bName ? 1 * dir : -1 * dir;

        });

        this.orderGridData();

        $(this.codingTable).datagrid("loadData", this.gridData);

        type.value = type.value == "desc" ? "asc" : "desc";

        this.handleRowClassEvent();
    }

    /**
     * 获取排序类型
     * @param field
     */
    private getSortByFeild(field: string): NameValueModel {

        for (var i = 0; i < this.sortType.length; i++) {
            if (this.sortType[i].name == field) return this.sortType[i];
        }
        return null;
    }


    /**
     * 使用shift多选
     */
    private shiftSelectRow(from: JQuery) {

        if (this.lastCheckedBox == null) return;

        var checked = this.lastCheckedBox.is(":checked");

        var checkboxs = $(this.checkBox);

        var start = false;
        var stop = false
        var startElem: JQuery = null;

        checkboxs.each((index: number, elem: Element) => {

            if (!start && !stop) {
                if ($(elem)[0] == $(from)[0]) {
                    start = true;
                    startElem = $(from);
                }

                else if ($(elem)[0] == this.lastCheckedBox[0]) {
                    start = true;
                    startElem = this.lastCheckedBox;
                }

                if (start) {
                    checked ? $(elem).attr("checked", "checked") : $(elem).removeAttr("checked");
                }
            }
            else if (start && !stop) {
                if ($(elem)[0] == $(from)[0] && startElem[0] == this.lastCheckedBox[0]) {
                    start = false;
                    stop = true;
                }

                if ($(elem)[0] == this.lastCheckedBox[0] && startElem[0] == $(from)[0]) {
                    start = false;
                    stop = true;
                }

                checked ? $(elem).attr("checked", "checked") : $(elem).removeAttr("checked");
            }
        });
    }

    /**
     * 判断是否需要勾选全部
     */
    private needCheckAll() {
        var checkboxs = $(this.checkBox);

        //如果全部勾选了
        if (checkboxs.filter((index: number, element: Element) => {
            return !$(element).is(":checked");
        }).length == 0) {

            $(this.checkAll).attr("checked", "checked");
        }
        else {
            $(this.checkAll).removeAttr("checked");
        }
    }

    /**
     * 设置数据的勾选状态
     * @param checked
     */
    private setGridDataChecked(checked: boolean) {

        //设置勾选
        this.gridData.forEach((value: FPCodingModel, index: number) => {

            value.MChecked = checked;
        });
    }



    /**
     * 初始化面板事件
     */
    public initEvent() {

        //标记
        $(this.markBtn).off("click").on("click", (evt: JQueryEventObject) => {

            var boxes = $(this.checkBox + ":checked");

            var ids: string[] = [];

            boxes.each((index: number, elem: Element) => {

                ids.push($(elem).attr("mid"));
            });

            ids = ids.distinct();

            if (ids.length == 0) {
                return mDialog.message(HtmlLang.Write(LangModule.FP, "PleaseSelectFapiaos2BeMarkCoding", "请勾选需要标记的发票!"));
            }

            var filter: FPFapiaoFilterModel = {
                MFapiaoIDs: ids,
                MCodingStatus: "2"
            };

            var saveFunc = (ft: FPFapiaoFilterModel) => {

                this.home.saveCodingStatus(ft, (data: OperationResult): void => {

                    if (data.Success) {

                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));

                        this.loadData();
                    }
                    else {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                    }
                });
            }

            saveFunc(filter);
        });

        //合并生成
        $(this.mergeCreate).off("click").on("click", () => {

            this.saveCoding(0);
        });

        //保存并全部生成
        $(this.saveNCreateBtn).off("click").on("click", () => {

            this.saveCoding(1);
        });

        //搜索
        $(this.searchCodingBtn).off("click").on("click", () => {

            this.page = 1;

            this.loadData();
        });

        //输入框加上了回车搜索
        $(this.keywordInput).unbind("keydown.return").bind("keydown.return", "return", (evt: JQueryEventObject) => {

            this.page = 1;

            this.loadData();
        });

        $(this.setBtn).tooltip({
            content: $('<div style="width:350px;"></div>'),
            position: "top",
            showEvent: "click",
            width: 360,
            onUpdate: (content: JQuery) => {
                content.append($(this.settingDiv).show());
                content.width(360);
            },
            onShow: () => {
                $(this.setBtn).off("mouseleave");
                this.initSettingEvent();
            }
        });

        $(this.keywordInput).off("dblclick").on("dblclick", (evt: JQueryEventObject) => {

            $(evt.srcElement || evt.target).val("");
        });
    }

    /**
     * 晃动发票设置按钮
     */
    private shakeSetButton() {

        $(this.setBtn).removeClass(this.shakeClass).addClass(this.shakeClass);
    }

    /**
     * 初始化显示列的事件
     */
    public initShowColumnEvent() {

        $(".fp-select-div").find("span:eq(0)").tooltip({
            content: $('<div style="width:200px;"></div>'),
            position: "top",
            showEvent: "mouseover",
            width: 200,
            onUpdate: (content: JQuery) => {
                var div = $(this.tipsDiv1).clone();
                content.append(div.show());
                content.width(200);
            },
            onShow: () => {
                this.shakeSetButton();
            }
        });

        $(".fp-select-div").find("span:eq(1)").tooltip({
            content: $('<div style="width:200px;"></div>'),
            position: "top",
            showEvent: "mouseover",
            width: 200,
            onUpdate: (content: JQuery) => {
                var div = $(this.tipsDiv2).clone();
                content.append(div.show());
                content.width(200);
            },
            onShow: () => {
                this.shakeSetButton();
            }
        });


        this.initShowPartAllEvent();
    }

    /**
     * 初始化显示全部和显示部分的事件
     */
    private initShowPartAllEvent() {
        //显示全部列
        $("." + this.allClass + ",." + this.partSelectedClass).off("click").on("click", (evt: JQueryEventObject) => {

            this.endEdit();

            this.showAllColumn();

            this.initShowColumnEvent();
        });

        //显示全部列
        $("." + this.allSelectedClass + ",." + this.partClass).off("click").on("click", (evt: JQueryEventObject) => {

            this.endEdit();

            this.showPartColumn();

            this.initShowColumnEvent();
        });
    }

    /**
     * 初始化元素  高度 宽度 滚动等
     */
    public initDom() {

        $(this.codingPartial).height($("body").height() - 155);
        $(this.pager).css({
            top: ($("body").height() - 30) + "px"
        });
        $(this.selectDiv).css({
            top: ($("body").height() - 30) + "px",
            left: (($("body").width() - 75) / 2) + "px"
        })
    }

    /**
     * 展示快速码编辑页面
     */
    public showFastCodeEdit(elem: JQuery, fastCode?: FCFapiaoModuleModel) {

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
            mCloseCallback: (): void => {

            },
            mOpenCallback: (): void => {
                this.initFastCodeEdit(fastCode);
                this.initFastCodeEvent(elem);
                $(elem).combogrid("hidePanel");
            }
        });
    }


    /**
     * 初始化快速码设置界面
     */
    public initFastCodeEdit(fastCode?: FCFapiaoModuleModel) {

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
            onSelect: (data: BDContactModel) => {

            },
            onLoadSuccess: () => {
                !!fastCode && !!fastCode.MMerItemID && $("input[field='MMerItemID']", partial).combobox("setValue", fastCode.MMerItemID);
            }
        });

        //科目
        $("input[field='MDebitAccount']", partial).combobox(
            {
                textField: "MFullName",
                valueField: "MItemID",
                srcRequired: true,
                height: 25,
                hideItemKey: "MIsActive",
                hideItemValue: false,
                autoSizePanel: true,
                data: this.baseData.MAccount,
                onSelect: (data: BDContactModel) => {

                },
                onLoadSuccess: () => {
                    !!fastCode && !!fastCode.MDebitAccount && $("input[field='MDebitAccount']", partial).combobox('setValue', fastCode.MDebitAccount);
                }
            });

        //科目
        $("input[field='MCreditAccount']", partial).combobox(
            {
                textField: "MFullName",
                valueField: "MItemID",
                srcRequired: true,
                height: 25,
                hideItemKey: "MIsActive",
                hideItemValue: false,
                autoSizePanel: true,
                data: this.baseData.MAccount,
                onSelect: (data: BDContactModel) => {

                },
                onLoadSuccess: () => {
                    !!fastCode && !!fastCode.MCreditAccount && $("input[field='MCreditAccount']", partial).combobox('setValue', fastCode.MCreditAccount);
                }
            });


        //科目
        $("input[field='MTaxAccount']", partial).combobox(
            {
                textField: "MFullName",
                valueField: "MItemID",
                srcRequired: true,
                height: 25,
                hideItemKey: "MIsActive",
                hideItemValue: false,
                autoSizePanel: true,
                data: this.baseData.MAccount,
                onSelect: (data: BDContactModel) => {

                },
                onLoadSuccess: () => {
                    !!fastCode && !!fastCode.MTaxAccount && $("input[field='MTaxAccount']", partial).combobox('setValue', fastCode.MTaxAccount);
                }
            });


        //跟踪项
        var tracks = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5]

        for (var i = 0; i < tracks.length; i++) {

            if (!tracks[i]) continue;

            let $textTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(2);

            let $valueTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(3);

            if (tracks[i].MCheckTypeName && tracks[i].MCheckTypeName.length > 0) {
                $textTd.text(mText.encode(tracks[i].MCheckTypeName));
                let $input = $valueTd.find("input");
                let defaultValue = mObject.getPropertyValue(fastCode, tracks[i].MCheckTypeColumnName);
                $input.show().attr("field", tracks[i].MCheckTypeColumnName).combobox(
                    {
                        textField: "text",
                        valueField: "id",
                        srcRequired: true,
                        height: 25,
                        hideItemKey: "MIsActive",
                        hideItemValue: false,
                        autoSizePanel: true,
                        formatter: this.getTrackComboboxFormatter(),
                        data: tracks[i].MDataList,
                        onLoadSuccess: () => {
                            !!defaultValue && $input.combobox("setValue", defaultValue);
                        }
                    });
            }
        }

        //取消红色的提示
        !!fastCode && $(".validatebox-invalid", partial).removeClass("validatebox-invalid");
    }


    /**
     * 初始化设置面板的事件
     */
    public initSettingEvent() {

        $(this.saveSettingBtn).off("click").on("click", (evet: JQueryEventObject) => {

            this.endEdit();

            var setting = this.getCodindSetting();

            this.home.saveCodingSetting(setting, (data: OperationResult) => {

                if (data.Success) {

                    mDialog.message(HtmlLang.Write(LangModule.FP, "SaveSuccessfully", "保存成功!"));

                    $(this.setBtn).tooltip("hide");
                    //保存起来
                    this.baseData.MSetting = setting;

                    this.showPartColumn();

                    this.initShowColumnEvent();
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
        $(this.settingCloseBtn).off("click").on("click", () => {

            $(this.setBtn).tooltip("hide");

            //this.resetTempColumn();
        });
    }


    /**
     * 初始化快速码事件
     */
    public initFastCodeEvent(elem: JQuery) {

        $(this.saveFastCodeBtn).off("click").on("click", () => {
            var fastCode = this.getFastCode();

            if (!!fastCode.MFastCode) {

                this.home.saveFastCode(fastCode, (data: OperationResult) => {

                    if (data.Success) {

                        mDialog.message(HtmlLang.Write(LangModule.FP, "SaveSuccessfully", "保存成功!"));

                        this.handleSavedFastCode(fastCode, data.ObjectID, elem);

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

        $(this.fastCodeCloseBtn).off("click").on("click", () => {

            mDialog.close();
        });
    }

    /**
     * 保存快速码
     * @param fastCode
     */
    private handleSavedFastCode(fastCode: FCFapiaoModuleModel, id: string, elem: JQuery) {

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

            if (!track || !track.MCheckTypeName || !track.MCheckTypeColumnName) continue;

            var value: string = mObject.getPropertyValue(fastCode, track.MCheckTypeColumnName);
            var text: string = mObject.getPropertyValue(fastCode, track.MCheckTypeColumnName + "Name");

            var exists = this.getTrackByFilter(track.MCheckTypeColumnName, value);

            if (!exists) {

                var trackItem: GLCheckTypeDataModel = mObject.getPropertyValue(this.baseData, track.MCheckTypeColumnName);

                trackItem.MDataList.push({
                    parentId: trackItem.MCheckTypeGroupID,
                    id: value,
                    text: text
                });

                let trackEditor = this.getRowEditorByField(track.MCheckTypeColumnName);

                trackEditor && trackEditor.editor && trackEditor.editor.target.combobox("loadData", trackItem.MDataList);
            }
        }

        elem.combogrid("grid").datagrid("loadData", this.baseData.MFastCode);

        this.fillGridWithFastCode(fastCode);
    }

    //显示所有列
    private showAllColumn(): void {


        $(this.setBtn).tooltip("hide");

        $("." + this.allClass).addClass(this.allSelectedClass).removeClass(this.allClass);

        $("." + this.partSelectedClass).addClass(this.partClass).removeClass(this.partSelectedClass);


        var cloneColumns: string[] = this.home.cloneArray(this.hiddenColumns);

        //检查是否有隐藏的列
        for (var i = 0; i < cloneColumns.length; i++) {

            this.showColumn(cloneColumns[i]);
        }

        $(this.codingTable).datagrid("fixColumnSize");

        this.handleRowClassEvent();

        this.initShowPartAllEvent();

    }

    //按照部分显示
    private showPartColumn(): void {


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

        this.handleRowClassEvent();

        this.initShowPartAllEvent();
    }

    /**
     * 根据设置显示combogrid的列
     */
    private showComboGridColumn(table?: JQuery): void {

        var showAllColumnStatus: boolean = $("." + this.allSelectedClass + ":visible").length > 0;

        //先全部显示
        this.baseData.MSetting.MMerItemID == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MMerItemID") : "";
        this.baseData.MSetting.MExplanation == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MExplanation") : "";
        this.baseData.MTrackItem1 && this.baseData.MSetting.MTrackItem1 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem1") : "";
        this.baseData.MTrackItem2 && this.baseData.MSetting.MTrackItem2 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem2") : "";
        this.baseData.MTrackItem3 && this.baseData.MSetting.MTrackItem3 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem3") : "";
        this.baseData.MTrackItem4 && this.baseData.MSetting.MTrackItem4 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem4") : "";
        this.baseData.MTrackItem5 && this.baseData.MSetting.MTrackItem5 == 0 && !showAllColumnStatus ? table.datagrid("hideColumn", "MTrackItem5") : "";

        $(table).datagrid("resize");

    }

    /**
     * 在做一些操作的时候需要对整个body进行一层遮罩，避免用户点击两次
     */
    private mask() {

        $(this.maskDiv).show();
    }

    /**
     * 取消遮罩
     */
    private unmask() {
        $(this.maskDiv).hide();
    }

    /**
     * 等待符号
     */
    private wait() {
        //等待和取消等待，都调用这个函数
        document.body.style.cursor = "wait";
    }

    /**
    * 等待符号
    */
    private unwait() {
        //等待和取消等待，都调用这个函数
        document.body.style.cursor = "default";
    }


    /**
     * 拆分行
     */
    private splitRow(elem: JQuery, event: Event): void {


        var rowIndex = this.getRowIndex(elem);

        //如果当前行正在编辑，则需要结束编辑
        if (this.editIndex == rowIndex)
            this.endEdit();

        var row = this.getCheckedRow(elem);

        var cloneRow: FPCodingModel = this.cloneRow(row);

        cloneRow.MIndex = row.MIndex + 1;

        this.appendRow(cloneRow, rowIndex);

        this.saveCodingRow(cloneRow);

        //合并行
        this.handleRowClassEvent();
    }

    /**
     * 是否包含
     * @param src
     * @param dest
     */
    private contains(src: FPCodingModel[], dest: FPCodingModel, field?: string) {

        field = field || "MRowIndex";

        for (var i = 0; i < src.length; i++) {
            if (src[i] && mObject.getPropertyValue(src[i], field) == mObject.getPropertyValue(dest, field)) return true;
        }
        return false;
    }

    /**
     * 拼接
     * @param src
     * @param dest
     */
    private concat(src: FPCodingModel[], dest: FPCodingModel[]) {

        if (!dest || dest.length == 0) return src;

        for (var i = 0; i < dest.length; i++) {
            if (!this.contains(src, dest[i]))
                src.push(dest[i]);
        }

        return src;
    }

    /**
     * 日志
     * @param text
     */
    private log(text: string) {
        window.console && window.console.log("行数(" + this.gridData.length + "):" + text + ":" + (new Date().getTime() % 100000 / 1000));
    }


    /**
     * 结束之前
     */
    private beforeEndEdit(index: number, row: FPCodingModel, changes: FPCodingModel) {

        this.log("结束编辑前准备");

        var currentRow = this.getRowByRowIndex();

        var sourceRow = this.getSourceGridRowData(currentRow.MID, currentRow.MEntryID)[0];

        var refreshRow: FPCodingModel[] = [];


        //获取联系人和商品项目是否有用户输入的内容
        let contactEditor = this.getRowEditorByField("MContactID");
        let contactTextbox = contactEditor.editor.textbox();
        let contactText: string = contactTextbox.val().trim();
        let tContact: BDContactModel;


        //如果用户没有任何文本，则表示要清空
        if (!contactText || contactText.length == 0) {
            currentRow.MContactID = null;
            currentRow.MContactIDName = null;
            tContact = null;
            //清空也需要同步联系人
            let xRows = this.selectContact(tContact, true);

            refreshRow = this.concat(refreshRow, xRows);
        }
        else {

            var contact = this.getContactByFilter(contactEditor.editor.getValue(), contactText);

            tContact = contact;

            //如果根据iD么有找到，或者找到了不是用户输入的，则表示用户输入的内容是需要新增的
            if (contact == null) {
                currentRow.MContactID = this.newGuid();
                currentRow.MContactIDName = contactText;

                var newContact: BDContactModel = {
                    MItemID: currentRow.MContactID,
                    MName: contactText,
                    MContactName: contactText,
                    MIsNew: true
                };

                this.baseData.MContact.push(newContact);

                tContact = newContact;

                let xRows = this.selectContact(tContact, true);

                refreshRow = this.concat(refreshRow, xRows);

            }
            else if (contact.MName != contactText) {
                //这是一种情况，就是用户输入了一个存在的联系人，但是没有触发选择事件
                currentRow.MContactID = contact.MItemID;
                currentRow.MContactIDName = contact.MName;

                let xRows = this.selectContact(contact, true);

                refreshRow = this.concat(refreshRow, xRows);

            } else {
                currentRow.MContactID = contact.MItemID;
                currentRow.MContactIDName = contact.MName;
            }
        }


        var itemEditor = this.getRowEditorByField("MMerItemID");
        //如果用户没有显示商品项目，则不需要做编辑
        if (!!itemEditor) {

            var itemTextbox = itemEditor.editor.textbox();
            var itemText: string = itemTextbox.val().trim();

            var item: BDItemModel;

            let tItem: BDItemModel = item;

            if (!itemText || itemText.length == 0) {

                currentRow.MMerItemID = null;
                currentRow.MMerItemIDName = null;
                item = null;

                let yRows = this.selectMerItem(item, true);
                refreshRow = this.concat(refreshRow, yRows);
            }
            else {

                item = this.getItemByFilter(itemEditor.editor.getValue(), itemText);

                tItem = item;

                //如果根据iD么有找到，或者找到了不是用户输入的，则表示用户输入的内容是需要新增的
                if (item == null) {

                    currentRow.MMerItemID = this.newGuid();
                    currentRow.MMerItemIDName = itemText;
                    var newItem: BDItemModel = {
                        MItemID: currentRow.MMerItemID,
                        MText: itemText,
                        MIsNew: true
                    };

                    this.baseData.MMerItem.push(newItem);

                    tItem = newItem;


                    let yRows = this.selectMerItem(tItem, true);
                    refreshRow = this.concat(refreshRow, yRows);

                }
                else if (item.MText != itemText) {
                    currentRow.MMerItemID = item.MItemID;
                    currentRow.MMerItemIDName = item.MText;


                    let yRows = this.selectMerItem(item, true);
                    refreshRow = this.concat(refreshRow, yRows);

                } else {
                    currentRow.MMerItemID = item.MItemID;
                    currentRow.MMerItemIDName = item.MText;
                }
            }

        }


        var trackItems = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, , this.baseData.MTrackItem5];

        for (var i = 0; i < trackItems.length; i++) {

            var track = trackItems[i];

            if (!track) continue;

            let editor = this.getRowEditorByField(track.MCheckTypeColumnName);

            if (!editor || !editor.editor) continue;

            var value = editor.editor.getValue();
            var text: string = editor.editor.textbox().val();

            let exists: GLTreeModel;

            let tTrack: GLTreeModel;

            if (!text || text.length == 0) {

                exists = null;
                tTrack = null;

                let zRows = this.selectTrackItem(tTrack, track.MCheckTypeColumnName, true);

                refreshRow = this.concat(refreshRow, zRows);
            }
            else {

                exists = this.getTrackByFilter(track.MCheckTypeColumnName, value, text);

                tTrack = exists;

                //如果不存在，或者有，但是文本对不上
                if (exists == null) {

                    var newTrack: GLTreeModel = {
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

                    let zRows = this.selectTrackItem(tTrack, track.MCheckTypeColumnName, true);

                    refreshRow = this.concat(refreshRow, zRows);

                } else if (text != exists.text) {
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName, exists.id);
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName + "Name", text);

                    let zRows = this.selectTrackItem(exists, track.MCheckTypeColumnName, true);

                    refreshRow = this.concat(refreshRow, zRows);

                } else {
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName, exists.id);
                    mObject.setPropertyValue(currentRow, track.MCheckTypeColumnName + "Name", text);
                }
            }
        }

        var selectedRowIndex: number[] = this.getSelectedIndex();

        if (refreshRow.length > 0) {

            this.refreshRows(refreshRow, currentRow);

            this.setRowSelected(selectedRowIndex);
        }
        else {

            this.saveCodingRow(currentRow);
        }

        this.log("完成结束编辑准备");
    }

    /**
     * 保存临时行
     * @param row
     */
    private saveCodingRow(row: FPCodingModel): void {

        this.saveCodingRows([row]);
    }

    /**
     * 保存临时行
     * @param row
     */
    private saveCodingRows(row: FPCodingModel[]): void {

        if (!this.saveInRealTime) return;

        //保存当前行到数据库
        this.home.saveCodingRow(row, (result: OperationResult): any => {

            if (result.Success && row.length == 1) {

                this.setRowID(row, result.ObjectID);
            }
        });
    }

    /**
     * 设置每一行的codingID
     * @param rows
     * @param idString
     */
    private setRowID(rows: FPCodingModel[], idString: string) {

        var ids = idString.split(',');

        for (var i = 0; i < rows.length; i++) {
            rows[i].MItemID = ids[i];
        }
    }



    /**
     * 生成GUID
     */
    private newGuid(): string {
        var guid = "";
        for (var i = 1; i <= 32; i++) {
            var n = Math.floor(Math.random() * 16.0).toString(16);
            guid += n;
        }
        return guid;
    }

    /**
     * 隐藏tooltip
     */
    private hideTooltip() {

        $(".tooltip-bottom:visible").hide();
    }

    /**
     * 
     */
    private hideCodingSetting() {
        $(this.setBtn).tooltip("hide");
    }

    /**
     * 开始bianjie
     * @param index
     */
    private beginEdit(index: number) {

        this.log("开始编辑准备");


        if (this.editIndex == index) {
            return;
        }

        if (this.editIndex != null)
            this.endEdit();


        this.log("开始编辑行");

        $(this.codingTable).datagrid("beginEdit", index == undefined ? this.editIndex : index);

        this.log("编辑行完成");

        this.editIndex = index;

        //获取当前行
        var currentRow = this.getRowByRowIndex();

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

        this.log("编辑行结束");
    }

    /**
     * 结束编辑
     * @param index
     */
    private endEdit() {

        this.log("表格结束编辑");

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

        this.log("表格结束编辑完成");

    }

    /**
     * 获取编辑器里面所有的输入框，排除不可编辑的
     */
    private getEditorInputs(): JQuery {
        //获取所有的编辑器
        var editors = this.getRowEditors();

        var inputs: JQuery = null;

        for (var i = 0; i < editors.length; i++) {

            let input = editors[i].editor.textbox();

            if (input.attr("disabled") == "disabled") continue;

            if (inputs == null) inputs = input;
            else inputs = inputs.add(input);
        }

        return inputs;
    }

    /**
     * 设置编辑行的事件
     */
    private initEditorsEvent() {

        var inputs: JQuery = this.getEditorInputs();

        //回车事件
        inputs.unbind("keydown.return").bind("keydown.return", "return", (evt: JQueryEventObject) => {

            this.goToNextCell(inputs, evt);
        });

        //tab事件
        inputs.unbind("keydown.tab").bind("keydown.tab", "tab", (evt: JQueryEventObject) => {

            this.goToNextCell(inputs, evt);
        });

        //left事件
        inputs.unbind("keydown.left").bind("keydown.left", "left", (evt: JQueryEventObject) => {

            this.goToPrevCell(inputs, evt);
        });

        //right事件
        inputs.unbind("keydown.right").bind("keydown.right", "right", (evt: JQueryEventObject) => {

            this.goToNextCell(inputs, evt);
        });

        //shift+return事件
        inputs.unbind("keydown.shiftreturn").bind("keydown.shiftreturn", "shift+return", (evt: JQueryEventObject) => {

            this.goToPrevCell(inputs, evt);
        });

        //shift+tab事件
        inputs.unbind("keydown.shifttab").bind("keydown.shifttab", "shift+tab", (evt: JQueryEventObject) => {

            this.goToPrevCell(inputs, evt);
        });
    }

    /**
     * 到下一个格子
     */
    private goToNextCell(inputs: JQuery, evt: JQueryEventObject) {

        var input = $(evt.srcElement || evt.target);

        var nextCell = this.getNextCellInput(inputs, input);

        if (nextCell == null)
            this.goToNextRow(inputs, evt);
        else {
            nextCell.focus();
        }
        this.home.stopPropagation(evt);
    }

    /**
     * 到前一个格子
     */
    private goToPrevCell(inputs: JQuery, evt: JQueryEventObject) {

        var input = $(evt.srcElement || evt.target);

        var prevCell = this.getPrevCellInput(inputs, input);

        if (prevCell == null)
            this.goToPrevRow(inputs, evt);
        else {
            prevCell.focus();
        }
        this.home.stopPropagation(evt);
    }

    /**
     * 到下一行
     */
    private goToNextRow(inputs: JQuery, evt: JQueryEventObject) {

        if (this.editIndex == this.gridData.length) return;

        this.beginEdit(this.editIndex + 1);

        this.getEditorInputs().eq(0).focus();

        this.home.stopPropagation(evt);


    }

    /**
    * 到上一行
    */
    private goToPrevRow(inputs: JQuery, evt: JQueryEventObject) {

        if (this.editIndex == 0) return;

        this.beginEdit(this.editIndex - 1);

        this.getEditorInputs().eq(0).focus();

        this.home.stopPropagation(evt);
    }

    /**
     * 获取下一个输入框
     * @param inputs
     * @param input
     */
    private getNextCellInput(inputs: JQuery, input: JQuery) {

        for (var i = 0; i < inputs.length; i++) {

            if (inputs.eq(i)[0] == input[0])
                return i == inputs.length - 1 ? null : inputs.eq(i + 1);
        }
        return null;
    }

    /**
     * 获取上一个输入框
     * @param inputs
     * @param input
     */
    private getPrevCellInput(inputs: JQuery, input: JQuery) {

        for (var i = 0; i < inputs.length; i++) {

            if (inputs.eq(i)[0] == input[0])
                return i == 0 ? null : inputs.eq(i - 1);
        }
        return null;
    }


    /**
     * 根据ID获取税率
     */
    public getTaxRateByValue(id: string): REGTaxRateModel {

        for (var i = 0; i < this.baseData.MTaxRate.length; i++) {
            if (this.baseData.MTaxRate[i].MItemID == id) return this.baseData.MTaxRate[i];
        }
        return null;
    }


    /**
     * 计算某一行的金额
     */
    private calculateAmount(calcuateRate?: boolean, $elem?: JQuery) {

        var amountEditor = this.getRowEditorByField("MAmount");
        var taxAmountEditor = this.getRowEditorByField("MTaxAmount");
        var taxRateEditor = this.getRowEditorByField("MTaxRate");
        var totalAmountEditor = this.getRowEditorByField("MTotalAmount");

        var amountTextbox = amountEditor.editor.textbox();
        var taxAmountTextbox = taxAmountEditor.editor.textbox();

        var amountTxt = amountTextbox.val();
        var taxTxt = taxAmountTextbox.val();

        var taxRateValue = taxRateEditor.editor.getValue();

        if (amountTxt == "-" || taxTxt == "-") return;

        var amount: number = +amountTxt, tax: number = + taxTxt;

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
        var topRow = allRows.filter((value: FPCodingModel, index: number) => {

            return value.MIndex == 0 && value.MEntryID == currentRow.MEntryID;
        })[0];

        var otherRows = allRows.filter((value: FPCodingModel, index: number) => {

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
        totalAmountEditor.editor.setValue(currentRow.MTotalAmount);

        this.refreshRow(topRow);

    }

    /**
     * 联系人输入框失去焦点
     * @param evt
     */
    private contactBlur(evt: JQueryEventObject): boolean {

        var currentRow = this.getRowByRowIndex();

        var contactText = $(evt.target || evt.srcElement).val();

        var currentContact: BDContactModel = this.getContactByFilter(undefined, contactText);

        let input = this.getRowEditorByField("MContactID").editor.target;

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
    }

    /**
     * 商品项目blur
     * @param evt
     */
    private itemBlur(evt: JQueryEventObject) {

        var currentRow = this.getRowByRowIndex();

        var itemText = $(evt.target || evt.srcElement).val();

        var currentItem = this.getItemByFilter(undefined, itemText);

        var needSelect = false;

        if (currentItem == null) {

            if (itemText.length == 0) {
                currentItem = {};
            } else {

                currentItem = {
                    MItemID: this.newGuid(),
                    MText: itemText,
                    MIsNew: true
                };

                this.baseData.MMerItem.push(currentItem);

                let input = this.getRowEditorByField("MMerItemID").editor.target;
                input.combobox("loadData", this.baseData.MMerItem);
                input.combobox("setValue", currentItem.MItemID);
            }

            needSelect = true;
        } else if (currentItem.MItemID != currentRow.MMerItemID) {

            needSelect = true;
        }


        currentRow.MMerItemID = currentItem.MItemID;
        currentRow.MMerItemIDName = currentItem.MText;

        needSelect && this.selectMerItem(currentItem);

        return true;
    }

    /**
     * 跟踪项blur
     * @param evt
     */
    private trackBlur(evt: JQueryEventObject) {
        var text = $(evt.srcElement || evt.target).val();

        let columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");

        var currentRow = this.getRowByRowIndex();

        let exists = this.getTrackByFilter(columName, undefined, text);

        var needSelect = false;

        if (exists == null) {

            if (text.length == 0) {
                exists = {};
            } else {

                exists = {
                    id: this.newGuid(),
                    text: text,
                    MIsNew: true,
                    parentId: mObject.getPropertyValue(this.baseData, columName).MCheckTypeGroupID
                };

                mObject.getPropertyValue(this.baseData, columName).MDataList.push(exists);

                let input = this.getRowEditorByField(columName).editor.target;
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
    }


    /**
     * 科目失去焦点
     * @param evt
     */
    private accountBlur(evt: JQueryEventObject) {

        var text = $(evt.srcElement || evt.target).val();

        let columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");

        var currentRow = this.getRowByRowIndex();

        let exists: BDAccountModel = this.getAccountByFilter(undefined, text);

        let needSelect = false;

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
    }


    /**
     * 联系人获取焦点
     * @param evt
     */
    private contactFocus(evt: JQueryEventObject) {

        let t = $(evt.srcElement || evt.target);

        let elem = this.getRowEditorByField("MContactID").editor.target;

        if (t.attr("inited") != "1") {

            var text = t.val();

            $(elem).combobox("loadData", this.baseData.MContact);

            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                let contact = this.getContactByFilter(null, text);

                if (contact != null) {
                    $(elem).combobox('setValue', contact.MItemID);
                }
            }

            $(elem).combobox("hidePanel");

            $(elem).combobox("showPanel");

            t.attr("inited", 1);
        }
    }

    /**
     * 商品项目获取焦点
     * @param evt
     */
    private itemFocus(evt: JQueryEventObject) {

        let t = $(evt.srcElement || evt.target);

        let elem = this.getRowEditorByField("MMerItemID").editor.target;

        if (t.attr("inited") != "1") {

            var text = t.val();

            $(elem).combobox("loadData", this.baseData.MMerItem);

            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                let item = this.getItemByFilter(null, text);

                if (item != null) {
                    $(elem).combobox('setValue', item.MItemID);
                }
            }

            $(elem).combobox("hidePanel");

            $(elem).combobox("showPanel");

            t.attr("inited", 1);
        }
    }

    /**
     * 跟踪项获取焦点
     * @param evt
     */
    private trackFocus(evt: JQueryEventObject) {

        let t = $(evt.srcElement || evt.target);

        let columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");

        var currentRow = this.getRowByRowIndex();

        let tracks: GLCheckTypeDataModel = mObject.getPropertyValue(this.baseData, columName);

        let elem = this.getRowEditorByField(columName).editor.target;

        if (t.attr("inited") != "1") {

            var text = t.val();

            $(elem).combobox("loadData", tracks.MDataList);

            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                let item = this.getTrackByFilter(columName, null, text);

                if (item != null) {
                    $(elem).combobox('setValue', item.id);
                }
            }

            $(elem).combobox("hidePanel");

            $(elem).combobox("showPanel");

            t.attr("inited", 1);
        }
    }

    /**
     * 科目获取焦点
     * @param evt
     */
    private accountFocus(evt: JQueryEventObject) {

        let t = $(evt.srcElement || evt.target);

        let columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");

        var currentRow = this.getRowByRowIndex();

        let elem = this.getRowEditorByField(columName).editor.target;

        if (t.attr("inited") != "1") {

            var text = t.val();

            $(elem).combobox("loadData", this.baseData.MAccount);

            if (!text) {
                $(elem).combobox("setValue", null);
            }
            else {
                let account = this.getAccountByFilter(null, text);

                if (account != null) {
                    $(elem).combobox('setValue', account.MItemID);
                }
            }

            $(elem).combobox("hidePanel");

            $(elem).combobox("showPanel");

            t.attr("inited", 1);
        }
    }

    /**
     * 摘要输入框失去焦点的时候，需要同步
     */
    private setInputEvent() {

        var explanationEditor = this.getRowEditorByField("MExplanation");

        var explanationTextbox = explanationEditor.editor.textbox();

        //金额输入框
        explanationTextbox.off("blur.fc").on("blur.fc", (evt: JQueryEventObject) => {

            var selectedRowIndex: number[] = this.getSelectedIndex();

            if (selectedRowIndex.length == 0) return true;

            var currentRow = this.getRowByRowIndex();

            var nowText = $(evt.target || evt.srcElement).val();

            if (currentRow.MExplanation != nowText) {

                currentRow.MExplanation = nowText;

                this.inputExplanation(nowText);
            }

            return true;
        });


        //联系人输入框
        let contactEditor = this.getRowEditorByField("MContactID");
        let contactTextbox = contactEditor.editor.textbox();
        let contactInput = contactEditor.editor.target;

        //联系人combobox加载和失去焦点
        contactTextbox.off("focus.fp").on("focus.fp", (evt: JQueryEventObject) => {

            this.contactFocus(evt);

            return true;
        }).off("blur.fp").on("blur.fp", (evt: JQueryEventObject) => {

            this.contactBlur(evt);

            return true;
        });

        //商品项目
        var merItemEditor = this.getRowEditorByField("MMerItemID");

        if (!!merItemEditor) {
            var merItemTextbox = merItemEditor.editor.textbox();
            merItemTextbox.off("focus.fp").on("focus.fp", (evt: JQueryEventObject) => {

                this.itemFocus(evt);
                return true;
            }).off("blur.fp").on("blur.fp", (evt: JQueryEventObject) => {

                this.itemBlur(evt);

                return true;
            });
        }

        //跟踪项
        var trackItems = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, , this.baseData.MTrackItem5];

        for (var i = 0; i < trackItems.length; i++) {

            let track = trackItems[i];

            if (!track) continue;

            let editor = this.getRowEditorByField(track.MCheckTypeColumnName);

            if (!editor || !editor.editor) continue;

            editor.editor.textbox().off("focus.fp").on("focus.fp", (evt: JQueryEventObject) => {

                this.trackFocus(evt);

                return true;
            }).off("blur.fp").on("blur.fp", (evt: JQueryEventObject) => {

                this.trackBlur(evt);

                return true;

            });
        }


        //科目选择
        var accounts = ["MDebitAccount", "MCreditAccount", "MTaxAccount"];

        for (var i = 0; i < accounts.length; i++) {

            let account = accounts[i];

            let editor = this.getRowEditorByField(account);

            editor.editor.textbox().off("focus.fp").on("focus.fp", (evt: JQueryEventObject) => {

                this.accountFocus(evt);

                return true;
            }).off("blur.fp").on("blur.fp", (evt: JQueryEventObject) => {

                this.accountBlur(evt);

                return true;

            });
        }

        //快速码

        let fastCodeEditor = this.getRowEditorByField("MFastCode");

        if (!!fastCodeEditor) {

            fastCodeEditor.editor.textbox().off("focus.fp").on("focus.fp", (evt: JQueryEventObject) => {

                let t = $(evt.srcElement || evt.target);

                let columName = $(evt.srcElement || evt.target).closest("td[field]").attr("field");

                var currentRow = this.getRowByRowIndex();

                let elem = this.getRowEditorByField(columName).editor.target;

                if (t.attr("inited") != "1") {

                    var text = t.val();

                    $(elem).combogrid("grid").datagrid("loadData", this.baseData.MFastCode);

                    if (!text) {
                        $(elem).combogrid("setValue", null);
                    }
                    else {
                        let fastCode = this.getFastCodeByFilter(null, text);

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


    }

    /**
     * 搜索快速码
     * @param evt
     */
    private searchFastCode(evt: JQueryEventObject) {

        var input = $(evt.target || evt.srcElement);

        var text = input.val();

        $("." + this.fastCodeSelectedClass).removeClass(this.fastCodeSelectedClass);

        if (!text) return;

        var hrefs = $(this.fastCodeEdit);

        hrefs.each((index: number, elem: Element) => {

            let t = $(elem).text();

            if (t.indexOf(text) >= 0) {

                $(elem).closest("tr").addClass(this.fastCodeSelectedClass);

                return false;
            }
        });
    }


    /**
     * 当前编辑行里面的事件
     */
    private setAmountInputEvent() {

        var amountEditor = this.getRowEditorByField("MAmount");
        var taxAmountEditor = this.getRowEditorByField("MTaxAmount");

        var amountTextbox = amountEditor.editor.textbox();
        var taxAmountTextbox = taxAmountEditor.editor.textbox();

        //金额输入框
        amountTextbox.off("keyup afterpaste").on("keyup afterpaste", _.debounce((evt: JQueryEventObject) => {
            this.calculateAmount(true, $(evt.target || evt.srcElement))
        }, 250));

        //税额输入框
        taxAmountTextbox.off("keyup afterpaste").on("keyup afterpaste", _.debounce((evt: JQueryEventObject) => {
            this.calculateAmount(false, $(evt.target || evt.srcElement))
        }, 250));
    }

    /**
     * 复制某一行
     */
    private cloneRow(row: FPCodingModel) {

        var newRow: FPCodingModel = _.clone(row);

        //金额字段都设置为0
        newRow.MTotalAmount = 0;
        newRow.MTaxAmount = 0;
        newRow.MAmount = 0;
        newRow.MItemID = null;
        newRow.MIsTop = false;
        newRow.MIsSplit = true;

        return newRow;
    }

    /**
     * 插入一行
     * @param row
     * @param index
     */
    public appendRow(row: FPCodingModel, currentIndex: number) {

        row.MRowIndex = currentIndex + 1;

        //插入一行之后，需要对index+1后面的数据进行编号 +1 
        for (var i = 0; i < this.gridData.length; i++) {

            if (this.gridData[i].MRowIndex >= row.MRowIndex) this.gridData[i].MRowIndex = this.gridData[i].MRowIndex + 1;

            if (this.gridData[i].MID == row.MID && this.gridData[i].MIndex >= row.MIndex) this.gridData[i].MIndex = this.gridData[i].MIndex + 1;
        }

        //如果是最后一行，则调用appendRow
        if (row.MRowIndex == this.gridData.length) {

            $(this.codingTable).datagrid("appendRow", row);
        }
        else {
            $(this.codingTable).datagrid("insertRow",
                {
                    index: row.MRowIndex,
                    row: row
                });
        }

        //如果是在前面加一行，则需要将当前编辑的下标添加1
        if (this.editIndex >= row.MRowIndex) this.editIndex = this.editIndex + 1;
    }

    /**
     * 删除某一行
     * @param index
     */
    public removeRow(elem: JQuery, index?: number) {

        if (index == undefined) {
            index = this.getRowIndex(elem);
        }

        this.mask();

        this.endEdit();

        var currentRow: FPCodingModel = this.getGridRowData(undefined, undefined, undefined, undefined, index)[0];

        var topRow: FPCodingModel = this.getGridRowData(currentRow.MID, currentRow.MEntryID, currentRow.MFapiaoNumber, 0)[0];

        var handleDeleteRow = () => {

            $(this.codingTable).datagrid("deleteRow", index);

            var subCount = 0;
            //插入一行之后，需要对index+1后面的数据进行编号 +1 
            for (var i = 0; i < this.gridData.length; i++) {

                if (this.gridData[i].MID == topRow.MID && this.gridData[i].MIndex != 0) subCount++;

                if (this.gridData[i].MRowIndex > index) this.gridData[i].MRowIndex = this.gridData[i].MRowIndex - 1;

                if (this.gridData[i].MID == currentRow.MID && this.gridData[i].MIndex > currentRow.MIndex) this.gridData[i].MIndex = this.gridData[i].MIndex - 1;
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
            this.refreshRow(topRow);

            //合并行
            this.handleRowClassEvent();

            this.hideTooltip();

            this.unmask();
        };

        this.deleteCodingRow(currentRow, handleDeleteRow);

    }

    /**
     * 实时删除行
     * @param row
     */
    private deleteCodingRow(row: FPCodingModel, callback: () => any) {

        //保存当前行到数据库
        this.home.deleteCodingRow(row, (result: OperationResult): any => {

            if (result.Success) {

                $.isFunction(callback) && callback();
            }
            else {
                this.unmask();
            }
        });
    }

    /**
     * 在新增行或者减少行的时候处理事件
     */
    public handleRowClassEvent() {

        this.log("开始合并行");

        this.initGridCellEvent();

        this.initGridClass();

        this.log("完成合并行");
    }

    /**
     * 获取行号
     * @param elem
     */
    public getRowIndex(elem: JQuery): number {
        var index = elem.attr("datagrid-row-index");

        if (index != undefined && index != null && index.length > 0) return +index;

        else return this.getRowIndex(($(elem).parents(".datagrid-row")));
    }

    /**
     * 获取选择的行
     */
    private getCheckedRow(elem: JQuery): FPCodingModel {


        var mIndex = +($(elem).attr("mindex"));

        var fapiaoNumber = $(elem).attr("mnumber");

        var mid = $(elem).attr("mid");

        var entryid = $(elem).attr("mentryid");

        var data = this.getGridRowData(mid, entryid, fapiaoNumber, mIndex)[0];

        return data;
    }

    /**
     * 获取选择的行
     */
    private getSelectedRow(): FPCodingModel[] {

        var rows: FPCodingModel[] = [];

        var $trs = this.getSelectedTrs();

        var ids: number[] = [];

        $trs.each((index: number, elem: Element) => {

            let i = +($(elem).attr("datagrid-row-index"));

            i !== this.editIndex && ids.push(i);
        });

        ids = ids.sort();

        for (var i = 0; i < this.gridData.length; i++) {

            if (ids.contains(this.gridData[i].MRowIndex))
                rows.push(this.gridData[i]);

        }

        return rows;
    }

    /**
     * 获取选中的dom行
     */
    private getSelectedTrs(): JQuery {
        return $("." + this.selectedRowClass + ":visible");
    }

    /**
     * 查找编号
     * @param fapiaoNumber
     * @param index
     */
    private getGridRowData(
        fapiaoID?: string,
        entryID?: string,
        fapiaoNumber?: string,
        index?: number,
        rowIndex?: number): FPCodingModel[] {

        var result: FPCodingModel[] = [];

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
    }

    /**
     * 查找编号
     * @param fapiaoNumber
     * @param index
     */
    private getSourceGridRowData(
        fapiaoID?: string,
        entryID?: string,
        fapiaoNumber?: string,
        index?: number,
        rowIndex?: number): FPCodingModel[] {

        var result: FPCodingModel[] = [];

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
    }

    /**
     * 根据列名获取某一列的值
     * @param fieldName
     */
    public getColumnsByField(fieldName: string) {

        var fields = this.allColumns.filter((value: DGColumn<FPCodingModel>, index: number) => { return value.field == fieldName });

        if (fields == null || fields.length == 0) return null;

        return fields;
    }

    /**
     * 封装显示列
     * @param fieldName
     */
    private showColumn(fieldName: string, $table?: JQuery) {

        $table = $table || $(this.codingTable);

        if (this.getColumnsByField(fieldName) != null && this.hiddenColumns.contains(fieldName)) {

            $table.datagrid("showColumn", fieldName);

            this.hiddenColumns = this.hiddenColumns.filter((value: string, index: number, array: string[]) => {

                return value != fieldName;
            });
            $table.datagrid("resize");
        }
    }

    /**
     * 封装隐藏列
     * @param fieldName
     */
    private hideColumn(fieldName: string, $table?: JQuery) {

        $table = $table || $(this.codingTable);

        if (this.getColumnsByField(fieldName) != null && !this.hiddenColumns.contains(fieldName)) {
            $table.datagrid("hideColumn", fieldName);
            this.hiddenColumns.push(fieldName);
        }
    }


    /**
     * 重置列显示
     */
    private resetTempColumn() {

        for (var i = 0; i < this.tempHiddenColumns.length; i++) {

            $(this.codingTable).datagrid("showColumn", this.tempHiddenColumns[i]);
        }

        for (var i = 0; i < this.tempShownColumns.length; i++) {
            $(this.codingBody).datagrid("hideColumn", this.tempShownColumns[i]);
        }

        this.tempHiddenColumns = [];
        this.tempShownColumns = [];

    }


    /**
     * 显示某一行
     * @param columnName
     * @param status
     */
    private showColumnBySetting(status: number, columnName: string, $table?: JQuery) {

        if (status == 0) {
            this.hideColumn(columnName);
        }
        else {
            this.showColumn(columnName);
        }
    }

    /**
     * 获取codeSetting
     */
    private getCodindSetting(): FPCodingSettingModel {

        var checkboxs = $(this.settingTableFieldValue + ":visible");

        var model: FPCodingSettingModel = {};

        checkboxs.each((index: number, elem: Element) => {

            var value = $(elem).is(":checked") ? 1 : 0;
            var value1 = ($(elem).attr("disabled") == "disabled") ? 1 : 0;
            var field = $(elem).attr("field");

            mObject.setPropertyValue(model, field, value + value1);
        });

        model.MID = this.baseData.MSetting.MID;

        return model;
    }

    /**
     * 获取用户设置的快速码信息
     */
    private getFastCode(): FCFapiaoModuleModel {


        var partial = $(this.fastCodeBody + ":visible");

        var fastCode: FCFapiaoModuleModel = {
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
        var tracks = [this.baseData.MTrackItem1, this.baseData.MTrackItem2, this.baseData.MTrackItem3, this.baseData.MTrackItem4, this.baseData.MTrackItem5]

        for (var i = 0; i < tracks.length; i++) {

            if (!tracks[i]) continue;

            var $valueTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(3);

            if (tracks[i].MCheckTypeName && tracks[i].MCheckTypeName.length > 0) {

                mObject.setPropertyValue(fastCode, tracks[i].MCheckTypeColumnName, $valueTd.find("input[field='" + tracks[i].MCheckTypeColumnName + "']").combobox("getValue"));
                mObject.setPropertyValue(fastCode, tracks[i].MCheckTypeColumnName + "Name", $valueTd.find("input[field='" + tracks[i].MCheckTypeColumnName + "']").combobox("getText"));
            }
        }

        return fastCode;
    }

    /**
     * 显示控制面板
     */
    public initSetting() {

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
    }

    /**
     * 设置元素的值
     * @param $elem
     * @param status
     */
    public setSettingDom($elem: JQuery, status: number, name?: string, field?: string): void {

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
    }

    /**
     * 保存发票勾兑
     */
    public saveCoding(type?: number) {

        type = type == undefined ? 0 : type;

        if (this.editIndex != null)
            this.endEdit();

        var codings: FPCodingModel[] = this.getCodingModels();

        if (!this.validateCodingModel(codings)) return;

        var filter: FPFapiaoFilterModel = {
            MCodings: codings,
            MSaveType: type,
            MFapiaoCategory: this.home.getType()
        }

        this.home.saveCoding(filter, (data: OperationResult): any => {

            if (data.Success) {

                mDialog.message(HtmlLang.Write(LangModule.FP, "SaveSuccessfully", "保存成功!"));

                this.baseData = null;

                this.loadData();
            }
            else {
                mDialog.message(HtmlLang.Write(LangModule.FP, "SaveFailed", "保存失败!"));
            }
        });
    }

    /**
     * 获取目前表格中的所有需要保存的model
     */
    public getCodingModels(): FPCodingModel[] {

        //获取所有的checkbox
        var checkboxs = $("input.fp-record-checkbox[type='checkbox']:checked");

        var selectedCodings: FPCodingModel[] = [];

        var fapiaoIds: string[] = [];

        checkboxs.each((index: number, elem: Element) => {

            let id = $(elem).attr("mid");

            if (!!id) fapiaoIds.push(id);
        });

        for (var i = 0; i < this.gridData.length && fapiaoIds.length > 0; i++) {

            if (fapiaoIds.contains(this.gridData[i].MID)) selectedCodings.push(this.gridData[i]);
        }

        return selectedCodings;
    }

    /**
    * 初始化分页
    */
    public initPage(total: number) {
        //调用easyui组件
        $(this.pager).pagination({
            total: total,
            pageSize: this.rows,
            pageList: this.home.pageList,
            onSelectPage: (page: number, size: number) => {
                this.page = page;
                this.rows = size;
                this.loadData();
            }
        });
    }

    /**
    * 选择科目
    */
    public inputExplanation(text: string, noSaveRefresh?: boolean): FPCodingModel[] {

        var editors: DGEditors<FPCodingModel>[] = this.getRowEditors();

        var editor: DGEditors<FPCodingModel> = this.getRowEditor(editors, "MExplanation");

        var currentRow: FPCodingModel = this.getRowByRowIndex();

        var refreshRow: FPCodingModel[] = [];

        currentRow.MExplanation = text;

        var selectedRowIndex: number[] = this.getSelectedIndex();

        var obj: FPCodingModel = {
            MExplanation: text
        };

        refreshRow = refreshRow.concat(this.batchCopyRow(obj, ["MExplanation"]));

        if (refreshRow.length > 0 && noSaveRefresh !== true) {

            this.refreshRows(refreshRow);

            this.setRowSelected(selectedRowIndex);
        }

        return refreshRow;
    }


    /**
    * 选择科目
    */
    public selectAccount(model: BDAccountModel, name: string, noSaveRefresh?: boolean): FPCodingModel[] {

        model = model || {};

        var editors: DGEditors<FPCodingModel>[] = this.getRowEditors();

        var editor: DGEditors<FPCodingModel> = this.getRowEditor(editors, name);

        var currentRow: FPCodingModel = this.getRowByRowIndex();

        var account: BDAccountModel = null;

        var refreshRow: FPCodingModel[] = [];

        var selectedRowIndex: number[] = this.getSelectedIndex();

        var obj: FPCodingModel = {};

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
    }

    /**
     * 新增跟踪项的时候，需要选择+reload
     * @param model
     */
    public addTrackItem(model: GLTreeModel, name: string) {


        if (!model) return;

        var editor: DGEditors<FPCodingModel> = this.getRowEditorByField(name);

        var currentRow = this.getRowByRowIndex();

        var tracks: GLCheckTypeDataModel = mObject.getPropertyValue(this.baseData, name);

        tracks.MDataList.push(model);

        tracks.MDataList.sort((a: GLTreeModel, b: GLTreeModel): number => {

            return a.text > b.text ? 1 : -1;
        });

        editor.editor.loadData(tracks.MDataList);

        editor.editor.target.combobox("select", model.id);

        editor.editor.target.combobox("hidePanel");
    }

    /**
     * 添加一个商品项目
     * @param model
     */
    public addMerItem(model: BDItemModel) {


        if (!model) return;

        var editor: DGEditors<FPCodingModel> = this.getRowEditorByField("MMerItemID");

        var currentRow = this.getRowByRowIndex();

        this.baseData.MMerItem.push(model);

        this.baseData.MContact.sort((a: BDItemModel, b: BDItemModel): number => {

            return a.MText > b.MText ? 1 : -1;
        });

        editor.editor.loadData(this.baseData.MMerItem);

        editor.editor.target.combobox("select", model.MItemID);

        editor.editor.target.combobox("hidePanel");
    }

    /**
     * 
     * @param model
     */
    public addAccount(model: BDAccountModel, name: string) {

        if (!model) return;

        var editor: DGEditors<FPCodingModel> = this.getRowEditorByField(name);

        var currentRow = this.getRowByRowIndex();

        this.baseData.MAccount.push(model);

        for (var i = 0; i < this.baseData.MAccount.length; i++) {

            if (this.baseData.MAccount[i].MItemID == model.MParentID)
                this.baseData.MAccount[i].MIsActive = false;
        }

        this.baseData.MAccount.sort((a: BDAccountModel, b: BDAccountModel): number => {

            return a.MCode > b.MCode ? 1 : -1;
        });

        editor.editor.loadData(this.baseData.MAccount);

        editor.editor.target.combobox("select", model.MItemID);

        editor.editor.target.combobox("hidePanel");

    }

    /**
     * 加入一个联系人
     * @param model
     */
    public addContact(model: BDContactModel) {


        if (!model) return;

        var editor: DGEditors<FPCodingModel> = this.getRowEditorByField("MContactID");

        var currentRow = this.getRowByRowIndex();

        this.baseData.MContact.push(model);

        this.baseData.MContact.sort((a: BDContactModel, b: BDContactModel): number => {

            return a.MName > b.MName ? 1 : -1;
        });

        editor.editor.loadData(this.baseData.MContact);

        editor.editor.target.combobox("select", model.MItemID);

        editor.editor.target.combobox("hidePanel");
    }


    /**
    * 选择跟踪项
    */
    public selectTrackItem(model: GLTreeModel, name: string, noSaveRefresh?: boolean): FPCodingModel[] {

        model = model || {};

        var editors: DGEditors<FPCodingModel>[] = this.getRowEditors();

        var editor: DGEditors<FPCodingModel> = this.getRowEditor(editors, name);

        var account: BDAccountModel = null;

        var refreshRow: FPCodingModel[] = [];

        var selectedRowIndex: number[] = this.getSelectedIndex();

        var obj: FPCodingModel = {};

        mObject.setPropertyValue(obj, name, model.id);
        mObject.setPropertyValue(obj, name + "Name", model.text);

        refreshRow = refreshRow.concat(this.batchCopyRow(obj, [name, name + "Name", "MIsNew"]));


        if (refreshRow.length > 0 && noSaveRefresh !== true) {

            this.refreshRows(refreshRow);

            this.setRowSelected(selectedRowIndex);
        }


        this.setEditorRowTooltip();

        return refreshRow;
    }

    /**
    * 选择联系人的时候需要更新同一条发票的所有联系人信息
    */
    public selectMerItem(model: BDItemModel, noSaveRefresh?: boolean): FPCodingModel[] {

        model = model || {};

        var editors: DGEditors<FPCodingModel>[] = this.getRowEditors();

        var editor: DGEditors<FPCodingModel> = this.getRowEditor(editors, "MMerItem");

        var currentRow: FPCodingModel = this.getRowByRowIndex();

        var account: BDAccountModel = null;

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

        var refreshRow: FPCodingModel[] = [];

        var selectedRowIndex: number[] = this.getSelectedIndex();

        refreshRow = refreshRow.concat(this.batchCopyRow({ MMerItemID: model.MItemID, MMerItemIDName: model.MText }, ["MMerItemID", "MMerItemIDName", "MIsNew"]));


        if (refreshRow.length > 0 && noSaveRefresh !== true) {

            this.refreshRows(refreshRow);

            this.setRowSelected(selectedRowIndex);
        }


        this.setEditorRowTooltip();

        return refreshRow;
    }

    /**
     * 选择联系人的时候需要更新同一条发票的所有联系人信息
     */
    public selectContact(model: BDContactModel, noSaveRefresh?: boolean): FPCodingModel[] {

        model = model || {};

        var editors: DGEditors<FPCodingModel>[] = this.getRowEditors();

        var editor: DGEditors<FPCodingModel> = this.getRowEditor(editors, "MContactID");

        var currentRow: FPCodingModel = this.getRowByRowIndex();

        //联系人设置
        this.setEditRowValue(editors, "MTrackItem1", model.MTrackItem1, model.MTrackItem1Name);
        this.setEditRowValue(editors, "MTrackItem2", model.MTrackItem2, model.MTrackItem2Name);
        this.setEditRowValue(editors, "MTrackItem3", model.MTrackItem3, model.MTrackItem3Name);
        this.setEditRowValue(editors, "MTrackItem4", model.MTrackItem4, model.MTrackItem4Name);
        this.setEditRowValue(editors, "MTrackItem5", model.MTrackItem5, model.MTrackItem5Name);

        var account: BDAccountModel = null;

        //往来科目
        if (!!model.MCCurrentAccountCode) {
            var accounts = this.baseData.MAccount.where("x.MCode =='" + model.MCCurrentAccountCode + "'");

            if (accounts != null && accounts.length > 0) {

                account = accounts[0];

                this.setEditRowValue(editors, this.home.getType() == 0 ? "MDebitAccount" : "MCreditAccount", account.MItemID, account.MFullName);
            }
        }

        var refreshRow: FPCodingModel[] = [];

        //获取所有的数据
        for (var i = 0; i < this.gridData.length; i++) {

            var row = this.gridData[i];

            if (row.MRowIndex == this.editIndex) continue;

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
                    } else if (this.home.getType() == 1 && !row.MCreditAccount) {
                        row.MCreditAccount = account.MItemID;
                    }
                }

                refreshRow.push(this.gridData[i]);
            }
        }
        var selectedRowIndex: number[] = this.getSelectedIndex();

        refreshRow = refreshRow.concat(this.batchCopyRow({ MContactID: model.MItemID, MContactIDName: model.MName }, ["MContactID", "MContactIDName", "MIsNew"]));

        if (refreshRow.length > 0 && noSaveRefresh !== true) {

            this.refreshRows(refreshRow);

            this.setRowSelected(selectedRowIndex);
        }

        this.setEditorRowTooltip();

        return refreshRow;
    }

    /**
     * 获取科目，根据代码
     * @param code
     */
    private getAccountByCode(code: string): BDContactModel {

        if (code == null || code == undefined || code.length == 0) return null;

        let accounts: BDAccountModel[] = this.baseData.MAccount.where("x.MCode =='" + code + "'");

        return accounts == null || accounts.length == 0 ? null : accounts[0];
    }


    /**
     * 设置行选中
     * @param index
     */
    private setRowSelected(indexs: number[]) {

        for (var i = 0; i < indexs.length; i++) {

            var $tr = this.getGridTrByIndex(indexs[i]);

            this.selectRow($tr, true);
        }
    }

    /**
     * 获取选中的发票下标
     */
    private getSelectedIndex(): number[] {

        var indexs: number[] = [];

        var selectedRow = this.getSelectedTrs();

        selectedRow.each((index: number, elem: Element) => {

            indexs.push(this.getRowIndex($(elem)));
        });

        return indexs;
    }

    /**
     * 同步数据
     * @param src
     */
    private batchCopyRow(src: FPCodingModel, fields: string[]): FPCodingModel[] {

        var codingRows: FPCodingModel[] = [];

        var rows = this.getSelectedRow();

        for (var i = 0; i < rows.length; i++) {

            if (this.equal(src, rows[i], fields)) continue;

            this.copyRow(src, rows[i], fields);

            codingRows.push(rows[i]);
        }

        return codingRows;
    }

    /**
     * 
     * @param src
     * @param desc
     * @param fields
     */
    private equal(src: FPCodingModel, dest: FPCodingModel, fields: string[]) {

        if (src == dest) return true;

        var equal = true;
        for (var j = 0; j < fields.length; j++) {
            if (mObject.getPropertyValue(src, fields[j]) != mObject.getPropertyValue(dest, fields[j]))
                return false;
        }

        return true;
    }

    /**
     * 批量设置的时候，复制某一行的数据到选中的行
     * @param src
     * @param dest
     */
    private copyRow(src: FPCodingModel, dest: FPCodingModel, fields: string[]): void {

        for (var i = 0; i < fields.length; i++) {

            mObject.setPropertyValue(dest, fields[i], mObject.getPropertyValue(src, fields[i]));
        }
    }


    /**
     * 选中联系人以后需要带出跟踪项
     * @param editors
     * @param trackItemName
     * @param trackItemValue
     */
    private setEditRowValue(editors: DGEditor<FPCodingModel>[], name: string, value: string, text: string): void {

        var editor = this.getRowEditor(editors, name);
        if (editor != null && !!value && !editor.editor.getValue()) {
            editor.editor.setValue(value);
            editor.editor.setText && editor.editor.setText(text);
        }
    }


    /**
     * 选择税率的时候，需要计算税额和合计
     * @param rate
     */
    public selectTaxRate(rate: REGTaxRateModel) {

        if (!rate) return;

        var currentRow: FPCodingModel = this.getRowByRowIndex();

        if (!rate || !rate.MText) return;

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
    }

    /**
     * 根据一个税率，获取税的科目
     * @param rate
     */
    private getTaxRateAccountID(rate: REGTaxRateModel) {
        //如果没有选择科目，并且税有带科目，则需要设置税的科目税科目
        var code = this.home.getType() == FPEnum.Sales ? rate.MSaleTaxAccountCode : rate.MPurchaseAccountCode;

        if (!code) return null;

        let account = this.getAccountByCode(code);

        return account == null ? null : account.MItemID;
    }

    /**
     * 重新刷数据
     * @param rows
     */
    public refreshRows(rows: FPCodingModel[], row?: FPCodingModel, nounmask?: boolean) {

        for (var i = 0; i < rows.length; i++) {

            $(this.codingTable).datagrid("updateRow", {
                index: rows[i].MRowIndex,
                row: rows[i]
            });
        }

        if (!!row)
            rows.push(row);

        this.saveCodingRows(rows);

        //合并行
        this.handleRowClassEvent();
    }

    /**
     * 更新某一行
     * @param index
     * @param row
     */
    public refreshRow(row: FPCodingModel) {

        row = row == undefined ? {} : row;

        this.refreshRows([row]);
    }


    /**
     * 根据行号获取某一行的数据
     * @param index
     */
    public getRowByRowIndex(index?: number): FPCodingModel {

        index = index == undefined ? this.editIndex : index;

        for (var i = 0; i < this.gridData.length; i++) {
            if (this.gridData[i].MRowIndex === index) return this.gridData[i];
        }
        return null;
    }



    //----编辑器---
    /**
     * 获取联系人的编辑器
     */
    public getContactEditor(): any {

        var editor: any = {
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
                    onSelect: (data: BDContactModel) => {

                        this.selectContact(data);
                    },
                    onLoadSuccess: (data: BDContactModel[], elem: JQuery) => {

                    }
                },
                addOptions: {
                    hasPermission: true,
                    isReload: false,
                    callback: (model: BDContactModel) => {

                        this.addContact(model);
                    }
                }
            }
        };
        return editor;
    }



    /**
     * 获取商品项目的编辑器
     */
    public getItemEditor() {
        var editor: any = {
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
                    onSelect: (data: BDItemModel) => {

                        this.selectMerItem(data);
                    },
                    onLoadSuccess: function () {

                    }
                },
                addOptions: {
                    hasPermission: true,
                    isReload: false,
                    callback: (model: BDItemModel) => {

                        this.addMerItem(model);
                    }
                }
            }
        };
        return editor;
    }

    /**
     * 获取税率的编辑框
     */
    public getTaxRateEditor() {
        var editor: any = {
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
                onSelect: (data: REGTaxRateModel) => {

                    this.selectTaxRate(data);
                },
                onLoadSuccess: () => {

                }
            }
        };
        return editor;
    }

    /**
     * 获取跟踪项的编辑框
     */
    public getTrackItemEditor(tracks: GLTreeModel[], trackName: string, parentId: string) {

        var editor: any = {
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
                    onSelect: (data: GLTreeModel) => {

                        this.selectTrackItem(data, trackName);
                    },
                    onLoadSuccess: function () {

                    }
                },
                addOptions: {
                    hasPermission: true,
                    url: "/BD/Tracking/CategoryOptionEdit?trackId=" + parentId,
                    isReload: false,
                    callback: (model: GLTreeModel) => {

                        if (!!model && !!model.id) {

                            this.addTrackItem(model, trackName);
                        }
                    }
                }
            }
        };
        return editor;
    }

    /**
     * 获取科目编辑框
     */
    public getAccountEditor(name: string) {
        var editor: any = {
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
                    onSelect: (data: BDAccountModel) => {

                        this.selectAccount(data, name)
                    },
                    onLoadSuccess: function () {

                    }
                },
                addOptions: {
                    hasPermission: true,
                    IsReload: false,
                    callback: (model: BDAccountModel) => {

                        this.addAccount(model, name);
                    }
                }
            }
        };
        return editor;
    }

    /**
     * 获取金额输入编辑框
     */
    public getAmountEditor() {

        var editor = {
            type: "numberbox",
            options: {
                precision: 2
            }
        };

        return editor;
    }

    /**
     * 快速码编辑框
     */
    public getFastCodeEditor() {

        var leftColumns: DGColumn<FCFapiaoModuleModel>[] = [
            {
                field: 'MFastCode', title: HtmlLang.Write(LangModule.FP, "MFastCode", "快速码"), width: 80, align: 'center',
                formatter: (value: string, record: FCFapiaoModuleModel) => {
                    return "<a class='fp-fastcode-edit' mid='" + record.MID + "'>" + value + "</a>";
                }
            },
            {
                field: 'MExplanation', title: HtmlLang.Write(LangModule.FP, "Explanation", "摘要"), width: 100, align: 'center', formatter: (value: string): string => { return value; }
            },
            {
                field: 'MMerItemID', title: HtmlLang.Write(LangModule.FP, "MerItem", "商品项目"), width: 100, align: 'center', formatter: this.getItemFormatter()
            }];

        var rightColumns: DGColumn<FCFapiaoModuleModel>[] = [
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

        let data: FCFapiaoModuleModel[] = [];

        var editor = {
            type: 'addCombogrid',
            options: {
                addOptions: {
                    url: (elem: JQuery) => {
                        this.showFastCodeEdit(elem)
                    },
                    height: 300,
                    itemTitle: HtmlLang.Write(LangModule.Common, "NewFastCode", "New Fast Code"),
                    hasPermission: true,
                    callback: (data: any) => {

                    }
                },
                dataOptions: {
                    panelWidth: (): number => {
                        return this.getCombogridPanelWidth();
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
                    onSelect: (index: number, record: FCFapiaoModuleModel) => {
                        this.fillGridWithFastCode(record);
                    },
                    onShowPanel: () => {
                        var editor = this.getRowEditorByField("MFastCode");

                        this.showComboGridColumn(editor.editor.target.combogrid("grid"));

                        this.initFastCodeCombogridEvent();
                    }
                }
            }
        };

        return editor;
    }

    /**
     * 联系人下拉框，根据是否是新增的显示不同的颜色
     */
    public getContactComboboxFormatter() {

        return (row: BDContactModel) => {

            return (row && row.MIsNew) ? this.getNoMatchHtml(row.MName) : row.MName;
        }
    }

    /**
     * 商品项目下拉框，根据是否是新增的显示不同的颜色
     */
    public getItemComboboxFormatter() {

        return (row: BDItemModel) => {

            return (row && row.MIsNew) ? this.getNoMatchHtml(row.MText) : row.MText;
        }
    }


    /**
      * 跟踪项下拉框，根据是否是新增的显示不同的颜色
      */
    public getTrackComboboxFormatter() {

        return (row: GLTreeModel) => {

            return (row && row.MIsNew) ? this.getNoMatchHtml(row.text) : row.text;
        }
    }



    /**
     * 初始化快速编辑事件
     */
    public initFastCodeCombogridEvent() {

        $(this.fastCodeEdit).off("click").on("click", (evt: JQueryEventObject) => {

            var $elem = $(evt.target || evt.srcElement);

            var mid = $elem.attr("mid");

            var textBox = this.getRowEditorByField("MFastCode").editor.target;

            var fastCode = this.getFastCodeByID(mid);

            this.showFastCodeEdit(textBox, fastCode);

            this.home.stopPropagation(evt);

        }).tooltip({ content: HtmlLang.Write(LangModule.FP, "Click2Edit", "点击编辑") });
    }

    /**
     * 获取数据
     * @param id
     */
    public getFastCodeByID(id: string) {

        for (var i = 0; i < this.baseData.MFastCode.length; i++) {
            if (this.baseData.MFastCode[i].MID == id) return this.baseData.MFastCode[i];
        }
        return undefined;
    }

    /**
     * 获取快速码的宽度
     */
    public getCombogridPanelWidth(): number {
        return $(this.taxAccountTitle).offset().left + $(this.taxAccountTitle).width() - $(this.fastCodeTitle).offset().left + 20 + 8;
    }

    /**
     * 获取摘要编辑框
     */
    public getExplanationEditor() {
        var editor = {
            type: "validatebox",
            options: {
                height: 34
            }
        };

        return editor;
    }

    /**
     * 
     * @param id
     * @param text
     */
    public getContactByFilter(id?: string, text?: string) {

        for (var i = 0; i < this.baseData.MContact.length; i++) {

            if (id != undefined && id != null && id && id == this.baseData.MContact[i].MItemID) {
                return this.baseData.MContact[i];
            }

            if (text != undefined && text != null && text && text == this.baseData.MContact[i].MName) {
                return this.baseData.MContact[i];
            }
        }
        return null;
    }

    /**
     * 
     * @param id
     * @param text
     */
    public getItemByFilter(id?: string, text?: string) {

        for (var i = 0; i < this.baseData.MMerItem.length; i++) {

            if (id != undefined && id != null && id && id == this.baseData.MMerItem[i].MItemID) {
                return this.baseData.MMerItem[i];
            }

            if (text != undefined && text != null && text && text == this.baseData.MMerItem[i].MText) {
                return this.baseData.MMerItem[i];
            }
        }
        return null;
    }

    /**
     * 
     * @param id
     * @param text
     */
    public getTrackByFilter(columnName: string, id?: string, text?: string): GLTreeModel {

        var track: GLCheckTypeDataModel = mObject.getPropertyValue(this.baseData, columnName);

        if (!track) return null;

        var options = track.MDataList;

        if (!options || options.length == 0) return null;

        for (var i = 0; i < options.length; i++) {

            if (id != undefined && id != null && id && id == options[i].id) {
                return options[i];
            }

            if (text != undefined && text != null && text && text == options[i].text) {
                return options[i];
            }
        }
        return null;
    }

    /**
     * 获取科目
     * @param id
     * @param text
     */
    public getAccountByFilter(id?: string, text?: string) {

        for (var i = 0; i < this.baseData.MAccount.length; i++) {

            if (id != undefined && id != null && id && id == this.baseData.MAccount[i].MItemID) {
                return this.baseData.MAccount[i];
            }

            if (text != undefined && text != null && text && text == this.baseData.MAccount[i].MFullName) {
                return this.baseData.MAccount[i];
            }
        }
        return null;
    }

    /**
     * 获取科目
     * @param id
     * @param text
     */
    public getFastCodeByFilter(id?: string, text?: string): FCFapiaoModuleModel {

        for (var i = 0; i < this.baseData.MFastCode.length; i++) {

            if (id != undefined && id != null && id && id == this.baseData.MFastCode[i].MID) {
                return this.baseData.MFastCode[i];
            }

            if (text != undefined && text != null && text && text == this.baseData.MFastCode[i].MFastCode) {
                return this.baseData.MFastCode[i];
            }
        }
        return null;
    }




    //------格式化-----
    /**
     * 联系人格式化
     */
    public getContactFormatter(): any {

        return (value: string, row: FPCodingModel, index: number): string => {

            //先处理用户输入的联系人的情况
            let text = !!value ? row.MContactIDName : (this.isLoadingData ? row.MPSContactName : "");

            var matches = this.getContactByFilter(value, text);

            //如果有文本，列表中不存在
            if (matches == null) {

                if (!text) return "";

                let newContact: BDContactModel = {
                    MName: text,
                    MItemID: !!value ? value : this.newGuid(),
                    MIsNew: true
                };

                this.baseData.MContact.push(newContact);

                row.MContactID = newContact.MItemID;

                return this.getNoMatchHtml(newContact.MName);
            }
            else {
                row.MContactID = matches.MItemID;
                row.MContactIDName = matches.MIsNew ? matches.MName : row.MContactIDName;
                return matches.MIsNew ? this.getNoMatchHtml(matches.MName) : matches.MName;
            }
        };
    }

    /**
     * 快速码格式化
     */
    public getFastCodeFormatter(): any {

        return (value: string, row: FPCodingModel, index: number): string => {

            var text = !!value ? row.MMerItemIDName : (this.isLoadingData ? row.MInventoryName : "");

            var matches = this.baseData.MFastCode.filter((r: FCFapiaoModuleModel, index: number) => {

                return r.MID == value;
            });

            return matches != null && matches.length > 0 ? matches[0].MFastCode : "";
        };
    }

    /**
     * 获取未匹配上的文本
     * @param text
     */
    private getNoMatchHtml(text: string) {

        return "<span class='fp-nomatch-text'>" + mText.encode(text || "") + "</span>";
    }

    /**
     * 联系人格式化
     */
    public getItemFormatter(): any {

        return (value: string, row: FPCodingModel, index: number): string => {


            var text = !!value ? row.MMerItemIDName : (this.isLoadingData ? row.MInventoryName : "");

            var matches = this.baseData.MMerItem.filter((r: BDItemModel, index: number) => {

                return (r.MItemID == value || text === r.MText || text === r.MDesc);
            });

            //如果有文本，列表中不存在
            if (matches.length == 0) {

                if (!text) return "";

                let newItem: BDItemModel = {
                    MText: text,
                    MItemID: !!value ? value : this.newGuid(),
                    MIsNew: true
                };

                this.baseData.MMerItem.push(newItem);

                row.MMerItemID = newItem.MItemID;

                return this.getNoMatchHtml(text);
            }
            else {
                row.MMerItemID = matches[0].MItemID;
                row.MMerItemIDName = matches[0].MIsNew ? matches[0].MText : row.MMerItemIDName;
                return matches[0].MIsNew ? this.getNoMatchHtml(matches[0].MText) : matches[0].MText;
            }
        };
    }

    /**
     * 联系人格式化
     */
    public getAccountFormatter(name: string): any {

        return (value: string, row: FPCodingModel, index: number): string => {

            value = value || mObject.getPropertyValue(row, name);

            var matches = this.baseData.MAccount.filter((row: BDAccountModel, index: number) => {

                return (row.MItemID == value)
            });

            return !!matches && matches.length > 0 ? matches[0].MFullName : "";
        };
    }

    /**
     * 联系人格式化
     */
    public getTaxRateFormatter(): any {

        return (value: string, row: FPCodingModel, index: number): string => {

            var matches = this.baseData.MTaxRate.filter((row: REGTaxRateModel, index: number) => {

                return (row.MItemID == value)
            });

            if (!!matches && matches.length > 0 && !row.MTaxAccount) {

                row.MTaxAccount = this.getTaxRateAccountID(matches[0]);
            }

            return !!matches && matches.length > 0 ? matches[0].MTaxText : "";
        };
    }
    /**
     * 获取跟踪项的格式化
     * @param data
     */
    public getTrackItemFormatter(data: GLTreeModel[], trackName: string, parentId: string): any {

        return (value: string, row: FPCodingModel, index: number): string => {

            var matches = data.filter((row: GLTreeModel, index: number) => {

                return (row.id == value)
            });

            var text = mObject.getPropertyValue(row, trackName + "Name");

            //如果有文本，列表中不存在
            if (matches.length == 0) {

                if (!text) return "";

                let newTrack: GLTreeModel = {
                    id: value,
                    text: text,
                    MIsNew: true,
                    parentId: parentId
                }

                data.push(newTrack);

                mObject.setPropertyValue(row, trackName, newTrack.id);

                return this.getNoMatchHtml(text);
            }
            else {

                mObject.setPropertyValue(row, trackName, matches[0].id);

                if (matches[0].MIsNew) {
                    mObject.setPropertyValue(row, trackName + "Name", matches[0].text);
                }

                return matches[0].MIsNew ? this.getNoMatchHtml(text) : matches[0].text;
            }
        };
    }


    //----填充表格
    public fillGridWithFastCode(data: FCFapiaoModuleModel) {

        if (!data.MFastCode) return;

        var editors: DGEditors<FPCodingModel>[] = this.getRowEditors();

        for (var i = 0; i < this.allColumns.length; i++) {

            var column = this.allColumns[i];

            var editor = this.getRowEditor(editors, column.field);

            if (mObject.hasOwnProperty(data, column.field) && column.field != "MFastCode") {

                var value: any = mObject.getPropertyValue(data, column.field);
                var text: any = mObject.getPropertyValue(data, column.field + "Name");

                editor != null && editor.editor.setValue && editor.editor.setValue(value);
                editor != null && editor.editor.setText && editor.editor.setText(text);
            }
        }


        var tempCoding: FPCodingModel = {
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
        }

        var selectedRowIndex: number[] = this.getSelectedIndex();

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
                "MTrackItem5"]);
        }

        if (rows.length > 0) {

            this.saveCodingRow(rows);

            this.refreshRows(rows);

            this.setRowSelected(selectedRowIndex);
        }
    }


    /**
     * 获取所有的行编辑
     */
    public getRowEditors(): DGEditors<FPCodingModel>[] {
        //
        var result: DGEditors<FPCodingModel>[] = [];

        if (this.editIndex != null) {

            if (this.gridEditors != null && this.gridEditors.length > 0) return this.gridEditors;

            //找到各个编辑的框
            var editors = $(this.codingTable).datagrid('getEditors', this.editIndex);


            for (var i = 0; i < this.allColumns.length; i++) {

                var column = this.allColumns[i];


                let editor: DGEditors<FPCodingModel> = {};

                if (!column.hidden && column.editor) {

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
                            if (!this.target || this.target.length == 0) return;

                            if (this.type == "numberbox" || this.type == "validatebox") {
                                //this.target.removeAttr("disabled").validatebox("enableValidation");
                            }
                            else {
                                //this.target[this.type]("enable").validatebox("enableValidation");
                            }
                        },
                        validate: function () {

                            if (!this.target || this.target.length == 0) return;

                            this.setValue(this.getValue());

                            if (!this.getValue()) this.setText("");

                            if (this.type == "numberbox" || this.type == "validatebox") {
                                return this.target.validatebox("isValid");
                            }
                            else {
                                return this.target[this.type]("isValid");
                            }
                        },
                        getValue: function () {

                            if (!this.target || this.target.length == 0) return;

                            if (this.type == "validatebox") return this.target.val();

                            return this.target[this.type]("getValue");
                        },
                        setValue: function (value: string) {

                            if (!this.target || this.target.length == 0) return;

                            if (this.type == "validatebox") return this.target.val(value);

                            this.target[this.type]("setValue", value);
                        },
                        setText: function (text: string) {

                            if (!this.target || this.target.length == 0) return;

                            if (this.type == "numberbox" || this.type == "validatebox") {
                                return this.target.val(text);
                            }
                            else {
                                this.target[this.type]("setText", text);
                            }
                        },
                        setRequired: function (req: boolean) {

                            if (!this.target || this.target.length == 0) return;

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
                        loadData: function (data: any) {

                            if (!this.target || this.target.length == 0) return;

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

                            if (!this.target || this.target.length == 0) return;

                            this.target[this.type]("reload");
                        }
                    }

                    result.push(editor);
                }
            }


            this.gridEditors = result;
        }

        return result;
    }

    /**
     * 获取具体某一个的编辑行
     * @param rows
     * @param field
     */
    public getRowEditor(rows: DGEditors<FPCodingModel>[], field: string): DGEditors<FPCodingModel> {

        for (var i = 0; i < rows.length; i++) {

            if (rows[i].name === field) return rows[i];
        }

        return null;
    }

    /**
     * 获取某个编辑框
     * @param field
     */
    public getRowEditorByField(field: string): DGEditors<FPCodingModel> {

        var editors = this.getRowEditors();

        return this.getRowEditor(editors, field);
    }

    /**
     * 更新个别字段的值
     * @param row
     * @param feilds
     */
    public setRowEditorValue(row: FPCodingModel, fields: string[]) {

        var editors = this.getRowEditors();

        for (var i = 0; i < fields.length; i++) {

            let editor = this.getRowEditor(editors, fields[i]);

            editor.editor.setValue(mObject.getPropertyValue(row, fields[i]));
        }
    }

    /**
     * 输入框的设置提醒
     * @param values
     */
    public setRowEditorTips(values?: NameValueModel[]): void {

        this.editorTips = [];

        values = values == undefined ? this.editorTips : values;

        for (var i = 0; i < values.length; i++) {
            let editor = this.getRowEditorByField(values[i].name);
            editor && editor.editor && editor.editor.textbox().tooltip({
                content: values[i].value
            });
        }
    }


    /**
     * 设置某些列不可编辑
     * @param row
     * @param feilds
     */
    public setRowEditorDisable(fields?: string[]) {

        fields = fields == undefined ? this.disableFields : fields;

        var editors = this.getRowEditors();

        for (var i = 0; i < fields.length; i++) {

            let editor = this.getRowEditor(editors, fields[i]);

            editor.editor.disable();
        }
    }



    /**
     * 
     * @param rows
     */
    public validateCodingModel(rows: FPCodingModel[]): boolean {

        var result: FPValidateModel[] = [];
        //没有一行数据的时候，提醒用户没有可生成凭证的发票
        if (rows.length == 0) {
            mDialog.alert(HtmlLang.Write(LangModule.FP, "NoFapiaoCanBeCreateVoucher", "请勾选要生成凭证的发票"));
            return false;
        }
        else {


            //逐行校验
            for (var i = 0; i < rows.length; i++) {

                let model: FPValidateModel = {
                    index: rows[i].MRowIndex,
                    row: rows[i],
                    messages: []
                };


                //1.联系人必录
                if (!rows[i].MContactID && rows[i].MIndex == 0) {

                    model.messages.push(HtmlLang.Write(LangModule.FP, "ContactIsEmpty", "联系人不可为空"));
                }

                //合计不可为0
                if (rows[i].MTotalAmount == 0) {

                    model.messages.push(HtmlLang.Write(LangModule.FP, "TotalAmountIsEmpty", "价税合计不可为空"));
                }

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

                    model.messages.push(HtmlLang.Write(LangModule.FP, "TaxAccountIsEmpty", "税科目不可为空"));
                }

                if (model.messages.length > 0) result.push(model);
            }
        }

        //如果有不通过的项目，需要合并起来提醒用户，并且开始编辑对应的行
        if (result.length > 0) {

            var message: string = "";

            for (var i = 0; i < result.length; i++) {
                message += "<div class='fp-error-message'>" + HtmlLang.Write(LangModule.FP, "Row", "行") + (result[i].index + 1) + ":" + result[i].messages.join(',') + "</div>";
            }

            mDialog.alert(message, () => {

                this.beginEdit(result[0].index);
            });
        }

        return result.length == 0;
    }

    /**
     * 获取相关的行
     * @param tr
     */
    private getRelatedTrs(tr: JQuery): JQuery {

        var rowspan = tr.find("td[field='MID']").attr("rowspan");
        //如果有多行
        if (!!rowspan && +(rowspan) > 0) {

            var num = $(this.splitBtn, tr).attr("mnumber");

            return tr.add(tr.nextAll(":lt(" + ((+rowspan - 1)) + ")"));
        }
        return tr;
    }


    /**
     * 拖拉选中
     */
    public mouseDrag2SelectRow() {

        $(this.codingBody).off("mousedown.m2").on("mousedown.m2", (evt: JQueryEventObject) => {

            if ($(evt.srcElement || evt.target)[0] == $(this.checkAll)[0]) return true;

            if ($(evt.srcElement || evt.target).hasClass("fp-record-checkbox")) return true;

            if (!$(this.codingBody).is(":visible")) return;

            var selList: JQuery = $(this.selectTd);

            //checkbox界限
            var checkX = $(this.checkAll).closest("td[field='MID']:visible").offset().left + $(this.checkAll).closest("td[field='MID']:visible").width();

            var startX = (evt.clientX);
            var startY = (evt.clientY);

            var bodyX = $(this.codingBody).offset().left;
            var bodyY = $(this.codingBody).offset().top + 35;

            var inventoryX = $(this.psContactTitle).offset().left + $(this.psContactTitle).width();

            if (startX > inventoryX) return;

            if (startX < bodyX || startY < bodyY) return;



            this.mouseDown = true;

            this.isSelect = true;



            var _x: number = null;

            var _y: number = null;
            $(this.codingBody).off("mousemove.m2").on("mousemove.m2", (evt: JQueryEventObject) => {

                if (this.isSelect && this.mouseDown)
                    this.clearSelectedText();

                _x = (evt.clientX);

                _y = (evt.clientY);

                if (Math.abs(_x - startX) <= 2 && Math.abs(_y - startY) <= 2) return;

                if (this.isSelect && this.mouseDown && $(".select-div").length == 0) {

                    let bgDiv = document.createElement("div");

                    bgDiv.style.cssText = "position:absolute;width:0px;height:0px;font-size:0px;margin:0px;padding:0px;background-color:#C3D5ED;z-index:999;opacity:0.0;left:0;top:0;display:none;";
                    bgDiv.className = "select-bg-div";
                    $(bgDiv).width($("body").width());
                    $(bgDiv).height($("body").width());
                    $(this.codingBody).append(bgDiv);

                    let selDiv = document.createElement("div");
                    selDiv.style.cssText = "position:absolute;width:0px;height:0px;font-size:0px;margin:0px;padding:0px;border:1px dashed #0099FF;background-color:#C3D5ED;z-index:1000;filter:alpha(opacity:60);opacity:0.6;display:none;";
                    selDiv.id = "selectDiv";
                    selDiv.className = "select-div";

                    $(this.codingBody).append(selDiv);

                    selDiv.style.left = startX + "px";

                    selDiv.style.top = startY + "px";
                }


                var selDiv = $(".select-div");
                var bgDiv = $(".select-bg-div");

                if (this.isSelect && this.mouseDown) {

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

                            if ((sl > startX && 20 < _x && st + 60 > startY && st < _y) || (sl > _x && sl < startX && st < startY && st > _y)) {
                                selList.eq(i).find("input[type='checkbox']:visible").attr("checked", "checked");
                            }
                        }
                        else {
                            if ((sl < startX && sl < _x && st + 60 > startY && st < _y) || (sl < _x && sl < startX && st < startY && st > _y)) {

                                var trs = this.getRelatedTrs(selList.eq(i).closest("tr.datagrid-row"));

                                trs.each((index: number, elem: Element) => {


                                    this.selectRow($(elem), true);
                                });
                            }
                        }
                    }

                    this.needCheckAll();

                }
                else {
                    selDiv.remove();
                    bgDiv.remove();
                }
            });


            $(document).off("mouseup.m2").on("mouseup.m2", (evt: JQueryEventObject) => {

                this.isSelect = false;
                this.mouseDown = false;

                var selectDiv = $(".select-div");
                var bgDiv = $(".select-bg-div");

                selectDiv.remove();

                bgDiv.remove();

                selList = null, _x = null, _y = null, startX = null, startY = null;

            });

            return;
        });
    }
}