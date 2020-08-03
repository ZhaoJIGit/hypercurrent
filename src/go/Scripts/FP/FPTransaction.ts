/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />

class FPTransaction implements iBusiness {

    private transactionPartial: string = ".fp-transaction-partial";
    private transactionTable: string = ".fp-transaction-table";
    private pager: string = ".fp-transaction-partial-pager";
    private checkBox: string = ".fp-record-checkbox:visible";

    private viewFapiao: string = ".fp-view-fapiao";
    private markButtonDiv: string = "#divMark";
    private markDiv: string = ".fp-transaction-mark";

    private markTop: string = "#aMarkAsRec";



    private page: number = 1;
    private rows: number = 20;

    private home: FPReconcileHome;
    /**
     * 初始化事件
     */
    public init() {

        this.home = new FPReconcileHome();

        this.initEvent();
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

        this.home.getTransactionList(filter, (data: DataGridJson<FPFapiaoModel>) => {

            this.showData(data);
        });
    }

    /**
     * 展示数据到面板
     */
    public showData(data: DataGridJson<FPFapiaoModel>) {

        $(this.transactionTable).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $(this.transactionPartial).width(),
            height: $(this.transactionPartial).height() - 65,
            columns: [[
                {
                    field: 'MID', title: '', width: 40, align: 'left', formatter: (value: string, record: FPFapiaoModel): string => {
                        var disabled = (record.MReconcileStatus == ReconcileStatusEnum.Reconciled) ? " disabled " : "";
                        return "<div style='text-align:center'><input " + disabled + " type='checkbox' class='fp-record-checkbox' mid='" + record.MID + "' rstatus='" + record.MReconcileStatus + "'/></div>";
                    }
                },
                {
                    field: 'MNumber', title: HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号"), width: 100, align: 'left', formatter: (value: string, record: FPFapiaoModel): string => {

                        return "<a class='fp-view-fapiao' mid='" + record.MID + "'>" + value + "</a>";
                    }
                },
                {
                    field: 'MBizDate', title: HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期"), width: 100, align: 'center', formatter: (value: Date): string => {

                        return mDate.format(value);
                    }
                },
                {
                    field: 'MContactName', title: HtmlLang.Write(LangModule.FP, "Company", "公司"), width: 300, align: 'left', formatter: (value: string, record: FPFapiaoModel): string => {

                        return this.home.getType() == FPEnum.Purchase ? record.MSContactName : record.MPContactName
                    }
                },
                {
                    field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 100, align: 'right', formatter: (value: number): string => {

                        return mMath.toMoneyFormat(value);
                    }
                },
                {
                    field: 'MStatus', title: HtmlLang.Write(LangModule.FP, "FapiaoStatus", "状态"), width: 100, align: 'center', formatter: (value: number): string => {
                        return this.home.getFapiaoStatusName(value);
                    }
                },
                {
                    field: 'MReconcileStatus', title: HtmlLang.Write(LangModule.FP, "ReconcileStatus", "勾兑状态"), width: 100, align: 'center', formatter: (value: number): string => {
                        return this.home.getReconcileStatusName(value, true);
                    }
                },
            ]],
            onLoadSuccess: () => {

                //初始化分页控件
                this.initPage(data.total);

                this.initGridEvent();

                $(this.transactionTable).datagrid("resize");
            }
        });

    }


    /**
     * 初始化表格里面的点击时间
     */
    public initGridEvent() {

        $(this.viewFapiao).off("click").on("click", (evt: JQueryEventObject) => {

            var mid = $(evt.srcElement || evt.target).attr("mid");

            this.home.viewFapiao(mid, null);
        });
    }

    /**
     * 初始化面板事件
     */
    public initEvent() {

        $(this.markTop).off("click").on("click", (evt: JQueryEventObject) => {

            $(".m-btn-line").trigger("click");
        });

        $(this.markDiv).off("click").on("click", (evt: JQueryEventObject) => {

            var $elem = $(evt.srcElement || evt.target);

            if (!$elem.hasClass(this.markDiv.trimStart('.'))) {
                $elem = $elem.parent(this.markDiv);
            }

            var status = ($elem.attr("status"));

            var boxes = $(this.checkBox + ":checked[rstatus!='1']");

            var ids: string[] = [];

            boxes.each((index: number, elem: Element) => {

                ids.push($(elem).attr("mid"));
            });

            ids = ids.distinct();

            var filter: FPFapiaoFilterModel = {
                MFapiaoIDs: ids,
                MReconcileStatus: status
            };

            this.home.setReconcileStatus(filter, (data: OperationResult) => {

                if (data.Success) {

                    mDialog.message(HtmlLang.Write(LangModule.FP, "SetSuccessfully", "标记成功!"));

                    this.loadData();
                }
                else {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "SetFailed", "标记失败!"));
                }
            });
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