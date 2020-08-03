

(function () {

    var FPTableList = (function () {
        //常量定义
        var pageIndex = 1;
        //单页显示的条数
        var pageSize = FPHome.pageSize;
        //总记录数
        var totalCount = 0;
        //
        var FPTableList = function (selector, type, invoiceType) {
            //
            var that = this;
            //
            var datagridBody = ".fp-datagrid-body";
            //
            var datagridDiv = ".fp-datagrid-div";
            var pager = ".fp-pager-div";
            //
            var getTableListUrl = "/FP/FPHome/GetTableViewModelPageList";
            //删除开票单
            var removeTableListUrl = "/FP/FPHome/DeleteTableByTableIds";
            //
            var editTableUrl = "/FP/FPHome/FPEditTable";
            //
            var removeTableButton = ".remove-table-button";
            //
            var deleteButton = ".fp-delete-button";
            //
            var txtNumber = ".dv-number-input";
            //
            var txtKeyword = ".dv-keyword-input";
            //
            var txtType = ".dv-type-input";
            //
            var txtDate = ".dv-date-input";
            //
            var btnSearch = ".fp-search-button";
            //
            var btnClear = ".dv-clear-button";
            //编辑初始化单据
            var editTableButton = ".edit-table-button";
            //
            var fapiaoDetail = ".fp-fapiao-detail";
            //
            var invoiceDetail = ".fp-invoice-detail";
            //
            var specialFapiaoLang = HtmlLang.Write(LangModule.FP, "SpecialFapiao", "专用发票");
            //
            var commonFapiaoLang = HtmlLang.Write(LangModule.FP, "commonFapiao", "普通发票");
            //
            var partlyIssued = HtmlLang.Write(LangModule.FP, "partlyIssued", "部分开票");
            //
            var allIssued = HtmlLang.Write(LangModule.FP, "allIssued", "完全开票");
            //
            var notIssued = HtmlLang.Write(LangModule.FP, "notIssued", "等待开票");
            //
            this.init = function () {
                //
                selector = selector;

                that.initDomSize();
                //
                that.initDomValue();
                //
                that.bindEvent();
                //
                that.refreshData();
            };
            //
            this.bindEvent = function () {
                //
                $(btnSearch, selector).off("click").on("click", that.refreshData);
                //
                $(btnClear, selector).off("click").on("click", function () {
                    //
                    $(txtNumber, selector).val("");
                    //
                    $(txtKeyword, selector).val("");
                    //
                    $(txtType, selector).combobox("setValue", "");
                    //
                    $(txtType, selector).combobox("setText", "");
                    //
                    $(txtDate, selector).val("")
                });
                //
                $(deleteButton, selector).off("click").on("click", function () {
                    //先获取所有勾选的
                    var selectTables = FPHome.GetGridSelectedCheckbox($(datagridBody, selector), [
                        {
                            attrName: "mid",
                            fieldName: "MItemID"
                        },
                        {
                            attrName: "mnumber",
                            fieldName: "MNumber"
                        }
                    ]);
                    //
                    that.deleteTables(selectTables);
                });
            };
            //
            this.refreshData = function () {
                //更新表头的数据
                new FPFapiao("", invoiceType).updateTableHomeData();
                //
                that.getTableListData();
            };
            //
            this.getTableListData = function () {
                //
                mAjax.post(getTableListUrl, { filter: that.getFilter() }, function (data) {
                    //
                    that.bindDataGrid(data);
                }, "", true);
            };
            this.getPeriod = function () {
                //
                var period = $(txtDate, selector).val();

                if (!period) {
                    return null;
                }
                //
                var date = mDate.parse(period + "-01");

                return date;
            }
            //获取查询条件
            this.getFilter = function () {

                var date = that.getPeriod();
                //
                return {
                    MIssueStatus: type,
                    MInvoiceType: invoiceType,
                    Keyword: FPHome.GetTableNumber($(txtKeyword, selector).val().trim()),
                    MFapiaoType: $(txtType, selector).combobox("getValue"),
                    MTableDate: date,
                    page: pageIndex,
                    rows: pageSize,
                    NumberAmount: FPHome.GetTableNumber($(txtNumber, selector).val().trim())
                }
            };
            //调用home的方法
            this.bindDataGrid = function (data) {
                //数据
                var data = data || [];
                //初始化数据
                $(datagridBody, selector).datagrid({
                    data: data.rows,
                    resizable: true,
                    auto: true,
                    fitColumns: true,
                    collapsible: true,
                    scrollY: true,
                    width: $(datagridDiv, selector).width() - 5,
                    height: ($("body").height() - $(datagridDiv, selector).offset().top - 70),
                    columns: [[
                          {
                              field: 'MItemID', title: '', width: 30, align: 'left', width: 30, formatter: function (value, rec) {
                                  //
                                  return "<div class='' id='" + value + "' style='text-align:center'><input type='checkbox' mid='" + value + "' mnumber='" + rec.MNumber + "'></div>";
                              }
                          },
                          {
                              field: 'MContacID', title: '', align: 'center', hidden: true
                          },
                          {
                              field: 'MNumber', width: 80, title: HtmlLang.Write(LangModule.FP, "TableNumber", "开票单号"), align: 'left',
                              formatter: function (value) {
                                  //
                                  return FPHome.GetFullTableNumber(value, invoiceType);
                              }
                          },
                          {
                              field: 'MContactName', width: 150, title: HtmlLang.Write(LangModule.FP, "ContacntName", "联系人"), align: 'left'

                          },
                          {
                              field: 'MType', width: 80, title: HtmlLang.Write(LangModule.FP, "TableType", "开票类型"), align: 'center', formatter: function (rec, data) {
                                  //
                                  switch (rec) {
                                      case 0:
                                          return commonFapiaoLang;
                                      case 1:
                                          return specialFapiaoLang;
                                      default:
                                          return "";
                                  }
                              }
                          },
                          {
                              field: 'MBizDate', width: 80, title: HtmlLang.Write(LangModule.FP, "TableDate", "日期"), align: 'center', formatter: function (value) {
                                  return $.mDate.format(value);
                              }
                          },
                          {
                              field: 'MExplanation', width: 100, title: HtmlLang.Write(LangModule.Common, "Explanation", "备注"), align: 'left'

                          },
                          {
                              field: 'MFapiaoCount', width: 50, title: HtmlLang.Write(LangModule.Common, "FapiaoCount", "发票数量"), align: 'center',
                              formatter: function (value, rec) {
                                  //
                                  return "<a class='fp-fapiao-detail' mid='" + rec.MItemID + "'>" + (!!rec.fapiaoList ? rec.fapiaoList.length : 0) + "</a>"
                              }

                          },
                          {
                              field: 'MInvoiceCount', width: 70, title: HtmlLang.Write(LangModule.Common, "InvoiceCount", "业务单据数量"), align: 'center',
                              formatter: function (value, rec) {
                                  //
                                  return "<a class='fp-invoice-detail' mid='" + rec.MItemID + "' mnumber='" + rec.MNumber + "'>" + (!!rec.invoiceList ? rec.invoiceList.length : 0) + "</a>"
                              }

                          },
                          {
                              field: 'MTotalAmount', width: 80, title: HtmlLang.Write(LangModule.FP, "TableTotalAmount", "金额"), align: 'right', editor: {
                                  type: 'numberbox',
                                  options: {
                                      required: true,
                                      precision: 2
                                  }
                              },
                              formatter: function (value) {
                                  return !value ? "" : parseFloat(value).toFixed(2);
                              }
                          },
                          {
                              field: 'MAjustAmount', width: 80, title: HtmlLang.Write(LangModule.FP, "AjustAmount", "调整金额"), align: 'right', editor: {
                                  type: 'numberbox',
                                  options: {
                                      required: true,
                                      precision: 2
                                  }
                              },
                              formatter: function (value) {
                                  return !value ? "" : parseFloat(value).toFixed(2);
                              }
                          },
                          {
                              field: 'MIssuedAmount', width: 80, title: HtmlLang.Write(LangModule.FP, "IssuedAmount", "已开票金额"), align: 'right', editor: {
                                  type: 'numberbox',
                                  options: {
                                      required: true,
                                      precision: 2
                                  }
                              },
                              formatter: function (value) {
                                  return !value ? "" : parseFloat(value).toFixed(2);
                              }
                          },
                          {
                              field: 'MIssueStatus', width: 80, title: HtmlLang.Write(LangModule.FP, "IssuedStatus", "开票状态"), align: 'center',
                              formatter: function (value) {
                                  //
                                  switch (value) {
                                      case 0:
                                          return notIssued;
                                      case 1:
                                          return partlyIssued;
                                      case 2:
                                          return allIssued;
                                      default:
                                          return "";
                                  }
                              }
                          },
                          {
                              field: 'Operation', width: 50, title: HtmlLang.Write(LangModule.Common, "Operation", "操作"), align: 'center', formatter: function (val, rec, rowIndex) {
                                  //如果是业务单据 必须是没有完成初始化的情况下
                                  var text = '<div class="list-item-action">';

                                  //而且可以编辑
                                  text += '<a href="javascript:void(0)" style="margin-right:10px" class="list-item-edit ' + editTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>';
                                  //
                                  text += '<a href="javascript:void(0)" class="m-icon-delete-row ' + removeTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>';
                                  //
                                  text += '</div>';
                                  //
                                  return text;
                              }
                          }
                    ]],
                    onClickRow: function () {
                        return false;
                    },
                    onLoadSuccess: function () {
                        //如果没有数据则添加一个空行，供用户编辑
                        if (data.rows && data.rows.length > 0) {
                            //
                            that.resizeTableHeight();
                            //
                            that.initTableEvent();
                        }
                    }
                });

                //分页
                $(pager, selector).pagination({
                    total: data.total,
                    pageSize: pageSize,
                    pageList: FPHome.pageList,
                    onSelectPage: function (number, size) {
                        pageIndex = number;
                        pageSize = size;
                        that.refreshData();
                    }
                });
            };
            this.initDomSize = function () {

                var totalWidth = $(".iv-tab-links:visible").width();

                $(".fp-home-li").width((totalWidth - 300) * 0.12);

                $("ul.tab-links li:gt(0)").width((totalWidth - 300) * 0.22 - 5);
            }
            //
            this.initDomValue = function () {
                //单据类型 普通发票 还是专用发票
                FPHome.InitFapiaoTypeCombobx($(txtType, selector), "", "", "", false, true);
            };
            //插入一行
            this.insertEmptyRow = function (index, row) {
                //空行的数据
                var emptyRow = {
                    MTableID: "",
                    MTableName: "",
                    MContactName: "",
                    MContactID: "",
                    MTableDate: "",
                    MTableType: "",
                    MTableTotalAmount: "",
                    MTableExplanation: ""
                };
                //对于空行
                row = row || $.extend(true, {}, emptyRow);
                //插入到数据中
                $(datagridBody, selector).datagrid("insertRow", { index: index, row: row });
                //
                that.initTableEvent();
                //
                that.resizeTableHeight();
            };
            //删除开票单
            this.deleteTables = function (tables) {
                //
                var tableNames = FPHome.GetFullTableNumber(tables.select("MNumber"), invoiceType);
                //
                var sureTitle = HtmlLang.Write(LangModule.FP, "AreYouSureToDeleteTablesAsFollow", "您是否确认删除以下开票单:") + tableNames;
                //
                tables && tables.length > 0 && mDialog.confirm(sureTitle, function () {
                    //
                    mAjax.submit(removeTableListUrl, { tableIds: tables.select("MItemID").join(',') }, function (data) {
                        //
                        mDialog.message(LangKey.DeleteSuccessfully);
                        //
                        if (tables.length == 1 && tables[0].index >= 0) {
                            //直接删除这一行就好了，不用整个页面刷新
                            $(datagridBody, selector).datagrid("deleteRow", tables[0].index);
                        }
                        else {
                            //
                            $(btnSearch, selector).trigger("click");
                        }
                        //更新表头的数据
                        new FPFapiao("", invoiceType).updateTableHomeData();
                    }, "", true);
                });
            };
            //打开销售单列表
            this.showInvoice = function (number) {
                $("#txtKeyword").val(number);
                $("#hidCurrentStatus").val(0);
                //先切换到业务单据页面
                FPHome.SwitchFapiaoInvoice(1);
                //
            }

            //编辑开票单
            this.editTable = function (tableID) {
                //
                new new FPHome("", invoiceType).editTable(tableID, that.refreshData);
            };

            //初始化表格中的事件
            this.initTableEvent = function () {
                //里面的删除事件
                $(removeTableButton, selector).off("click").on("click", function () {
                    //
                    var tableId = $(this).attr("mid");
                    //
                    var tableNumber = $(this).attr("mnumber");
                    //
                    var rowIndex = FPHome.GetGridRowIndexByCellItem($(datagridBody, selector), $(this));
                    //
                    that.deleteTables([{ MItemID: tableId, MNumber: tableNumber, index: rowIndex }]);
                });
                //表格里面的编辑事件
                $(editTableButton, selector).off("click").on("click", function () {
                    //
                    that.editTable($(this).attr("mid"));
                });
                //表格里面的编辑事件
                $(fapiaoDetail, selector).off("click").on("click", function () {
                    //
                    that.editTable($(this).attr("mid"));
                });
                //表格里面的编辑事件
                $(invoiceDetail, selector).off("click").on("click", function () {
                    //
                    that.showInvoice(FPHome.GetFullTableNumber($(this).attr("mnumber"), invoiceType));
                });
            };
            //重新计算表格的高度
            this.resizeTableHeight = function () {

            };
            // #凭证模板 结束
        };
        //返回
        return FPTableList;
    })();
    //
    window.FPTableList = FPTableList;
})()