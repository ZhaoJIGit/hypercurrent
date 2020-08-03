/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />

class FPStatement implements iBusiness {


    private statementPartial: string = ".fp-statement-partial";
    private statementTable: string = ".fp-statement-table";
    private pager: string = ".fp-statement-partial-pager";
    private showDetail: string = ".fp-import-detail";

    private page: number = 1;
    private rows: number = 20;

    private home: FPReconcileHome;
    /**
     * 初始化事件
     */
    public init() {

        this.home = new FPReconcileHome();
    }

    /**
     * 获取数据
     */
    public loadData(dates?: Date[]) {

        dates = dates || this.home.getPickedDate();
        //获取过滤
        var filter: FPFapiaoFilterModel = {
            page: this.page,
            rows: this.rows,
            MStartDate: dates[0],
            MEndDate: dates[1],
            MFapiaoCategory: this.home.getType()
        };

        this.home.getStatementList(filter, (data: DataGridJson<FPImportModel>) => {

            this.showData(data);
        });
    }

    /**
     * 展示数据到面板
     */
    public showData(data: DataGridJson<FPImportModel>) {

        $(this.statementTable).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $(this.statementPartial).width(),
            height: $(this.statementPartial).height() - 45,
            columns: [[
                {
                    field: 'MID', title: '', width: 40, align: 'left', hidden: true
                },
                {
                    field: 'MDate', title: HtmlLang.Write(LangModule.FP, "ImportDate", "导入日期"), width: 100, align: 'center',
                    formatter: (value: Date, rec: FPImportModel): string => {

                        return "<a href='javascript:void(0)' class='fp-import-detail' mid='" + rec.MID + "'>" + mDate.format(value) + "</a>";
                    }
                },
                {
                    field: 'MStartDate', title: HtmlLang.Write(LangModule.FP, "StartDate", "开始日期"), width: 150, align: 'center', formatter: (value: Date): string => {

                        return mDate.format(value);
                    }
                },
                {
                    field: 'MEndDate', title: HtmlLang.Write(LangModule.FP, "EndDate", "结束日期"), width: 150, align: 'center', formatter: (value: Date): string => {

                        return mDate.format(value);
                    }
                },
                {
                    field: 'MCount', title: HtmlLang.Write(LangModule.FP, "Quantity", "数量"), width: 150, align: 'center'
                },
                {
                    field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 150, align: 'center', formatter: (value: number): string => {

                        return mMath.toMoneyFormat(value);
                    }
                },
                {
                    field: 'MOperator', title: HtmlLang.Write(LangModule.FP, "Operator", "操作员"), width: 150, align: 'center'
                },
                {
                    field: 'MFileName', title: HtmlLang.Write(LangModule.FP, "FileName", "文件名"), width: 150, align: 'center'
                },
                {
                    field: 'MSource', title: HtmlLang.Write(LangModule.FP, "Source", "来源"), width: 150, align: 'center', formatter: (source: number): string => {
                        return this.home.getFapiaoSourceName(source);
                    }
                }
            ]],
            onLoadSuccess: () => {

                //初始化分页控件
                this.initPage(data.total);

                $(this.statementTable).datagrid("resize");

                this.initGridEvent();
            }
        });

    }

    /**
     * 初始化面板事件
     */
    public initEvent() {

    }

    /**
    * 初始化面板事件
    */
    public initGridEvent() {

        $(this.showDetail).off("click").on("click", (evt: JQueryEventObject) => {

            var mid = $(evt.target || evt.srcElement).attr("mid");

            this.home.showTransactionDetail(mid, null);
        });
    }

    /**
     * 初始化元素  高度 宽度 滚动等
     */
    public initDom() {

    }

    /**
    * 初始化分页
    */
    public initPage(total: number) {
        //调用easyui组件
        $(this.pager).pagination({
            total: total,
            pageSize: this.home.pageSize,
            pageList: this.home.pageList,
            onSelectPage: (page: number, size: number) => {
                this.page = page;
                this.rows = size;
                this.loadData();
            }
        });
    }
}