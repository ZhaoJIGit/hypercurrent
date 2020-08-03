/// <reference path="../IVBase.js" />
(function () {
    var FPInvoice = (function () {
        var FPInvoice = function (invoiceType) {
            //
            var sale = 0, purchase = 1;
            //
            var that = this;
            //
            var tableBody = "#gridInvoice";
            //
            var hasChangeFapiaoAuth = $("#hidChangeFapiaoAuth").val() === "1";
            //
            var issueFapaioBtn = "#btnIssueFapiao";
            //
            var delFapaioTableBtn = "#btnDelFapiaoTable";
            //
            var searchBtn = "#btnSearchInvoice";
            //
            var clearBtn = "#btnClearInvoice";
            //
            var selDateType = "#selSearchWithin";
            //
            var selIssueStatus = "#selIssueStatus";
            //
            var invoiceButton = ".fp-invoice-btn";
            //
            var allCountDiv = ".fp-all-count";
            //
            var notIssedCountDiv = ".fp-notissued-count";
            //
            var partlyIssuedDiv = ".fp-partlyissued-count";
            //
            var allIssuedDiv = ".fp-allissued-count";
            //
            var tabDiv = ".fp-invoice-tabs";
            //
            var item = HtmlLang.Write(LangModule.FP, "Item", "项");
            //
            var items = HtmlLang.Write(LangModule.FP, "Items", "项");
            //定义各个tab对应的index
            var all = 0, notIssued = 0, partlyIssued = 1, allIssued = 2;
            //
            //var currentStatus = 0;
            //获取销售采购列表的数据
            //this.getInvoiceData = function () {

            //};

            this.initData = function () {
                //日期类型
                $(selDateType).combobox({
                    valueField: 'value', textField: 'text',
                    data: [{ value: "1", text: HtmlLang.Write(LangModule.IV, "Anydate", "Any date") },
                        { value: "2", text: HtmlLang.Write(LangModule.IV, "TransactionDate", "Transaction date") },
                        { value: "3", text: HtmlLang.Write(LangModule.IV, "DueDate", "Due date") },
                        { value: "4", text: HtmlLang.Write(LangModule.IV, "ExpectedDate", "Expected date") }],
                    onLoadSuccess: function () {
                        $(selDateType).combobox("setValue", "1");
                    }
                });
                //发票类型
                $(selIssueStatus).combobox({
                    valueField: 'value', textField: 'text',
                    data: [{ value: "-1", text: HtmlLang.Write(LangModule.FP, "AllIssueStatus", "所有开票状态") },
                        { value: "0", text: HtmlLang.Write(LangModule.FP, "NotIssued", "等待开票") },
                        { value: "1", text: HtmlLang.Write(LangModule.FP, "PartlyIssued", "部分开票") },
                        { value: "2", text: HtmlLang.Write(LangModule.FP, "Issued", "完全开票") }],
                    onLoadSuccess: function () {
                        $(selIssueStatus).combobox("setValue", "-1");
                    }
                });
            };
            //绑定 删除、开票事件
            this.bindInvoiceEvent = function () {
                //开票
                $(issueFapaioBtn).off("click").on("click", function () {
                    Megi.grid(tableBody, "optSelected", {
                        callback: function (ids) {
                            IssueFapiao.multiple(ids, invoiceType, '', function () {
                                new FPInvoice('', invoiceType).reload();
                            });
                        }
                    });
                });
                //删除开票关系
                $(delFapaioTableBtn).off("click").on("click", function () {
                    Megi.grid(tableBody, "optSelected", {
                        callback: function (ids) {
                            var issuedIds = [];
                            var selRows = $(tableBody).datagrid('getSelections');
                            $.each(selRows, function (i, row) {
                                if (row.MTableID) {
                                    issuedIds.push(row.MID);
                                }
                            });
                            if (issuedIds.length > 0) {
                                that.deleteFapiaoTable(issuedIds.toString());
                            }
                            else {
                                $.mDialog.alert(HtmlLang.Write(LangModule.IV, "NoIssueFapiao", "没有可删除的开票关系！"));
                            }
                        }
                    });
                });
                //查询事件
                $(searchBtn).off("click").on("click", function () {
                    that.bindInvoiceGrid();
                });
                //清空查询条件
                $(clearBtn).off("click").on("click", function () {
                    $("body").mFormClearForm();
                    $(selDateType).combobox("setValue", "1");
                    $(selIssueStatus).combobox("setValue", "-1");
                });
            };
            //初始化销售采购单列表
            this.bindInvoiceGrid = function () {
                var columnList = null;
                invoiceType = invoiceType || $("#hidInvoiceType").val();

                switch (parseInt(invoiceType)) {
                    case sale:
                        IVBase.columns[7].title = HtmlLang.Write(LangModule.IV, "Received", "Received");
                        IVBase.columns[8].title = HtmlLang.Write(LangModule.IV, "UnReceived", "Unreceived");
                        columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 1, 2, 3, 4, 5, 6, 7, 8, 14, 15]);//, 10, 11
                        break;
                    case purchase:
                        IVBase.columns[2].title = HtmlLang.Write(LangModule.Common, "From", "From");
                        columnList = Megi.getDataGridColumnList(IVBase.columns, [0, 2, 3, 4, 5, 7, 8, 14, 15]);//, 11
                        break;
                }
                if (hasChangeFapiaoAuth) {
                    columnList[0].push({
                        title: HtmlLang.Write(LangModule.IV, "Operation", "Operation"), field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                            var actionHtml = "<div class='list-item-action' style='text-align:left;margin-left:10px'>";

                            //
                            actionHtml += "<a href=\"javascript:void(0);\" onclick=\"IssueFapiao.single('" + (rec.MTableID || "") + "', '" + (rec.MID || "") + "','" + invoiceType + "'," + (rec.MTaxID == "No_Tax") + ", function(){ new FPInvoice('','" + invoiceType + "').reload(); });\" class='fp-create-table'></a>";
                            if (rec.MTableID) {
                                actionHtml += "<a href=\"javascript:void(0);\" onclick=\"new FPInvoice().deleteFapiaoTable('" + (rec.MID || "") + "');\" class='remove-reconcile-button'></a>";
                            }
                            //
                            actionHtml += "</div>";
                            return actionHtml;
                        }
                    });
                }
                var queryParam = that.getParam();
                Megi.grid(tableBody, {
                    url: IVBase.url_getlist,
                    sortName: "MCreateDate",
                    sortOrder: "desc",
                    pagination: true,
                    pageList: FPHome.pageList,
                    queryParams: queryParam,
                    columns: columnList,
                    fitColumns: true,
                    resizable: true,
                    scrollY: true,
                    lines: true,
                    onLoadSuccess: function (data) {
                        that.autoHeight();
                        //
                        that.bindInvoiceEvent();

                        that.setCheckBoxStatus(data);
                    }
                });
            };
            //已开票的行复选框置灰，不可选中
            this.setCheckBoxStatus = function (data) {
                //已开票的复选框置灰
                for (var i = 0; i < data.rows.length; i++) {
                    if (data.rows[i].MTableNumber) {
                        $("#divList .datagrid-row[datagrid-row-index=" + i + "] input[type='checkbox']")[0].disabled = true;
                    }
                }

                //已开票的复选框不能选中
                $(tableBody).datagrid('getPanel').find('div.datagrid-header input[type=checkbox]').off('change.all').on('change.all', function () {
                    if (this.checked) {
                        for (var i = 0; i < data.rows.length; i++) {
                            if ($("#divList .datagrid-row[datagrid-row-index=" + i + "] input[type='checkbox']")[0].disabled) {
                                $(tableBody).datagrid("unselectRow", i);
                            }
                        }
                    }
                });
            };

            //开票,用户点击勾选，或者销售采购单后面的列表，进入开票单页面
            //this.issueFapiao = function (ids) {

            //};
            //删除与开票单之间的关系
            this.deleteFapiaoTable = function (ids) {
                $.mDialog.confirm(HtmlLang.Write(LangModule.FP, "AreYouSureToDeleteIssueRelation", "您确定要删除开票关系？"), {
                    callback: function () {
                        mAjax.submit(
                            "/FP/FPHome/DeleteTableByInvoiceIds",
                            { invoiceIds: ids },
                            function (response) {
                                if (response.Success) {
                                    that.reload();
                                }
                            });
                    }
                });
            };
            //初始化销售采购单的列表
            this.init = function () {
                that.initData();
                //
                that.showSummaryData();
                //
                that.initTab();
            };
            this.reload = function () {
                if ($(invoiceButton).hasClass("current")) {
                    that.showSummaryData();
                    that.initTab();
                }
            };
            this.autoHeight = function () {
                var selectorName = ".m-imain";
                var gridWrapDivHegith = $(selectorName).height() - $("#divTab").outerHeight() - $("#divAction").outerHeight() - 13;//15
                $(".datagrid-wrap", selectorName).height(gridWrapDivHegith);

                var gridViewDiv = $(".datagrid-view", selectorName);
                gridViewDiv.height(gridWrapDivHegith);

                var gridBody = $(".datagrid-body", selectorName);
                //数据显示的高度需要减去表头占用高度
                var gridBodyHeight = gridViewDiv.height();

                try {
                    $(tableBody).datagrid('resize', {
                        width: $("#divList").width(),
                        height: gridBodyHeight
                    });
                } catch (exc) { }
            };
            //初始化 Tab 选项卡
            this.initTab = function () {
                $(tabDiv).tabsExtend({
                    initTabIndex: Number($("#hidCurrentStatus").val()),
                    onSelect: function (index) {
                        //currentStatus = index;
                        $("#hidCurrentStatus").val(index);
                        switch (index) {
                            case 1:
                                $(issueFapaioBtn).show();
                                $(delFapaioTableBtn).hide();
                                break;
                            case 2:
                            case 3:
                                $(issueFapaioBtn).hide();
                                $(delFapaioTableBtn).show();
                                break;
                            default:
                                $(issueFapaioBtn).show();
                                $(delFapaioTableBtn).show();
                                break;
                        }
                        that.bindInvoiceGrid();
                    }
                });
            };
            this.getParam = function (isSummary) {
                var queryParam = {};
                if (isSummary) {
                    queryParam.MIssueStatus = -1;
                }
                else {
                    queryParam = $("body").mFormGetForm();
                    queryParam.MIssueStatus = parseInt($("#hidCurrentStatus").val()) - 1;
                }
                queryParam.MStatus = IVBase.Status.AwaitingPayment;
                invoiceType = invoiceType || $("#hidInvoiceType").val();
                queryParam.MType = invoiceType == sale ? "Invoice_Sale" : "Invoice_Purchase";
                queryParam.IsFromFapiao = true;
                return queryParam;
            };
            this.showSummaryData = function () {
                var url = "/IV/IVInvoiceBase/GetFPInvoiceSummary";
                var param = that.getParam(true);
                mAjax.post(url, { param: param }, function (response) {
                    var data = response;
                    //
                    var notIssedCount = data.where("x.MName =='" + notIssued + "'")[0].MValue;
                    //
                    var partlyIssuedCount = data.where("x.MName =='" + partlyIssued + "'")[0].MValue;
                    //
                    var allIssedCount = data.where("x.MName =='" + allIssued + "'")[0].MValue;
                    //
                    var totalCount = data.where("x.MName =='-1'")[0].MValue;

                    var divTab = $(tabDiv);
                    //
                    $(allCountDiv, divTab).text("(" + totalCount + (totalCount > 1 ? items : item) + ")");
                    //
                    $(notIssedCountDiv, divTab).text("(" + notIssedCount + (notIssedCount > 1 ? items : item) + ")");
                    //
                    $(partlyIssuedDiv, divTab).text("(" + partlyIssuedCount + (partlyIssuedCount > 1 ? items : item) + ")");
                    //
                    $(allIssuedDiv, divTab).text("(" + allIssedCount + (allIssedCount > 1 ? items : item) + ")");
                });
            };
        };

        return FPInvoice;
    })();
    //
    window.FPInvoice = FPInvoice;
})()