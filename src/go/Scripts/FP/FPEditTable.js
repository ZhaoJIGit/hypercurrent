(function (w) {

    var FPEditTable = (function () {
        //
        var FPEditTable = function (tableId, invoiceIds, invoiceType) {
            //
            tableId = tableId || "";
            //
            invoiceIds = invoiceIds || "";
            //
            var tableBody = ".et-fapiao-table";
            //
            var datagridDiv = ".et-fapiao-div";
            //
            var totalDiv = ".ef-total-div";
            //
            var topDiv = ".et-top-div";
            //
            var numberPrefix = ".et-number-prefix";
            //
            var tableData = {};
            //
            var removeFapiaoButton = ".remove-fapiao-button";
            //
            var deleteFapiaoButton = ".delete-fapiao-button";
            //编辑初始化单据
            var editFapiaoButton = ".edit-fapiao-button";
            //查看
            var viewFapiaoButton = ".view-fapiao-button";
            var sendFapiaoButton = ".send-bw-fapiao-button";

            
            var removeReconcileButton = ".remove-reconcile-button";
            //
            var addFapiaoButton = ".add-fapiao-button";
            //
            var getFapiaoListUrl = "/FP/FPHome/GetFapiaoList";
            //获取开票单的基本信息
            var getTableBaseInfoUrl = "/FP/FPHome/GetTableBaseInfo";
            //编辑发票的界面
            var editFapiaoUrl = "/FP/FPHome/FPEditFapiao";
            //
            var editTableUrl = "/FP/FPHome/FPEditTable";
            //
            var saveTableUrl = "/FP/FPHome/FPSaveTable";
            //
            var removeFapiaoListUrl = "/FP/FPHome/DeleteFapiaoByFapiaoIds";

            var removeReconcileUrl = "/FP/FPHome/RemoveReconcile";

            //发送发票的界面
            var FPSendBWFapiao = "/FP/FPHome/FPSendBWFapiao";
            //
            var taxAmount = "tr.ef-tax-amount td:eq(1)";
            //
            var notaxAmount = "tr.ef-notax-amount td:eq(1)";
            //
            var totalAmount = "tr.ef-total-amount td:eq(1)";
            //
            var unIssuedAmount = "tr.ef-unissued-amount td:eq(1)";
            //
            var saveButton = ".et-save-button";
            //
            var cancelButton = ".et-cancel-button"
            //
            var txtType = "#txtType";
            //
            var txtContact = "#txtContact";

            var txtContactTaxCode = "#txtContactTaxCode";
            //
            var txtNumber = "#txtNumber";
            //
            var txtExplanation = "#txtExplanation";
            //
            var txtAjustAmount = "#txtAjustAmount";
            //
            var txtTaxAmount = "#txtTaxAmount";
            //
            var txtTotalAmount = "#txtTotalAmount";
            //
            var txtDate = "#txtDate";
            //
            var that = this;

            var home = new FPHome();

            this.editIndex = null;
            //日志按钮
            var aHistory = "#aHistory";
            //开票单Id
            var SaleSinglebilling = "#hidInvoiceId";
            //空行的数据
            var emptyRow = {
                MID: "",
                MContactName: "",
                MContactID: "",
                MBizDate: "",
                MType: "",
                MTotalAmount: "",
                MTaxAmount: "",
                MAmount: "",
                MExplanation: ""
            };
            //
            this.init = function () {
                //
                that.initDomValue();
                //
                that.bindEvent();
                //
                that.iniContact();
                //
                that.getTableBaseInfo();
            };
            //
            this.initDomValue = function () {
                //初始化发票类型
                FPHome.InitFapiaoTypeCombobx($(txtType));
                //
                $(txtNumber).numberbox({
                    required: true,
                    min: 0,
                    max: 9999,
                    formatter: function (value) {
                        //
                        return FPHome.GetNumber(value);
                    }
                });
            };
            //初始化联系人选择
            this.iniContact = function () {
                //这种情况就是从直接新建开票单过来的，需要用户去选择联系人
                if (!tableId && !invoiceIds) {
                    //
                    FPHome.InitContact(txtContact, "", invoiceType, function (data) {

                        if ($(txtContactTaxCode).is(":visible")) {
                            $(txtContactTaxCode).val(data.MTaxNo || "");
                        }
                    });
                }
            }
            //页面上的事件
            this.bindEvent = function () {
                //添加日志的点击事件
                $(aHistory).off("click").on("click", function () {
                    //获取到当前页面的那个开票单的ID
                    var SaleSinglebillingID = $(SaleSinglebilling).val();
                    //在js当中 0 undifind NaN Null 判断的时候都是false
                    !!SaleSinglebillingID ? HistoryView.openDialog(SaleSinglebillingID, "Sale_FaPiao_Table") : "";
                }
                )
                //
                $(saveButton).off("click").on("click", function () {
                    //
                    that.saveTable();
                });
                //
                $(cancelButton).off("click").on("click", function () {
                    //
                    $.mDialog.close();
                });
                //
                $(txtAjustAmount + "," + txtTotalAmount).off("keyup").on("keyup", function () {
                    //
                    that.updateTotalAmount();
                });
            }
            //获取表格中的某一行
            this.getRowData = function (index) {
                //
                var rows = FPHome.GetGridRowsWithData(tableBody, that.editIndex);
                //先结束编辑
                if (that.editIndex != null) {
                    //先结束编辑
                    rows.push(that.getEditRowData());
                }
                //
                if (index != undefined && index >= 0 && rows.length > 0 && index < rows.length) {
                    //
                    return rows[index];
                }
                //把月份转化为日期格式
                for (var i = 0; i < rows.length ; i++) {
                    //
                    rows[i].MDeductionDate = mDate.format(rows[i].MDeductionDate);
                }
                //
                return rows.where("!!x.MTotalAmount || !!x.MTaxAmount || !!x.MBizDate || !!x.MNumber || !!x.MExplanaton || !!x.MItemID");
            };
            //获取编辑的哪一行的金额
            this.getEditRowData = function () {
                //
                if (that.editIndex != null) {
                    //找到各个编辑的框
                    var editors = $(tableBody).datagrid('getEditors', that.editIndex);
                    //获取本行
                    var editRow = {
                        MTaxAmount: $(editors.where("x.field=='MTaxAmount'")[0].target).val(),
                        MTotalAmount: $(editors.where("x.field=='MTotalAmount'")[0].target).val(),
                        MBizDate: $(editors.where("x.field=='MBizDate'")[0].target).datebox("getValue"),
                        MDeductionDate: $(editors.where("x.field=='MDeductionDate'")[0].target).val(),
                        MNumber: $(editors.where("x.field=='MNumber'")[0].target).val(),
                        MItemID: $(editors.where("x.field=='MItemID'")[0].target).combobox("getValue"),
                        MExplanation: $(editors.where("x.field=='MExplanation'")[0].target).val()
                    }
                    //
                    return editRow;
                }
                //
                return that.getEmptyRowData();
            };
            //
            this.bindGridData = function (data) {
                //数据
                var data = data || [];
                //
                $(tableBody).datagrid({
                    data: data,
                    resizable: true,
                    auto: true,
                    fitColumns: true,
                    collapsible: true,
                    scrollY: true,
                    width: $(datagridDiv).width() - 5,
                    //销售单 
                    height: ($("body").height() - $(datagridDiv).offset().top - $(totalDiv).height() - 40 - 35),
                    columns: [[
                          {
                              field: 'MID', title: '', width: 10, align: 'left', formatter: function (rec) {
                                  //
                                  return "<a href='javascript:void(0);' class='m-icon-add-row " + addFapiaoButton.trimStart('.') + "' id='" + rec + "' style='text-align:center'>&nbsp;</a>";
                              }
                          },
                          {
                              field: 'MHasDetail', title: '', hidden: true
                          },
                          {
                              field: 'MTotalAmount', width: 50, title: HtmlLang.Write(LangModule.FP, "FapiaoTotalAmount", "含税金额"), align: 'right', formatter: function (value) {
                                  return mMath.toMoneyFormat(value);
                              },
                              editor: {
                                  type: "numberbox",
                                  required: true,
                                  options: { precision: 2, minPrecision: 0, required: true }
                              }
                          },
                          {
                              field: 'MTaxAmount', width: 50, title: HtmlLang.Write(LangModule.FP, "FapiaoTaxAmount", "税额"), align: 'right', formatter: function (value) {
                                  return mMath.toMoneyFormat(value);
                              },
                              editor: {
                                  type: "numberbox",
                                  options: { precision: 2, minPrecision: 0, required: true }
                              }
                          },
                          {
                              field: 'MBizDate', width: 40, title: HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期"), align: 'center', formatter: function (value) {
                                  return value ? mDate.format(value) : value;
                              }, editor: {
                                  type: "datebox",
                                  options: {
                                      required: true,
                                      height: "24px",
                                      formatter: function (value) {
                                          //
                                          return value ? mDate.format(value) : value;
                                      },
                                      onSelect: function (value) {
                                          //
                                          that.validateDeducationDate(value, "");
                                      }
                                  }
                              }
                          },
                          {
                              field: 'MDeductionDate', width: 40, title: HtmlLang.Write(LangModule.FP, "VerifyDate", "认证月份"), align: 'center', hidden: invoiceType == 0 ? true : false,
                              formatter: function (value, rec) {
                                  //
                                  return FPHome.ParseDate2YearMonth(value);
                              }, editor: {
                                  type: "monthpicker",
                                  options: {
                                      readonly: true,
                                      required: false,
                                      height: 30,
                                      lang: $("#hideLang").val(),
                                      minDate: $("#hideBeginDate").val(),
                                      formatter: function (value) {
                                          //
                                          return FPHome.ParseDate2YearMonth(mDate.parse(value));
                                      },
                                      onpicked: function (value) {
                                          //
                                          that.validateDeducationDate("", value);
                                      }
                                  }
                              }
                          },
                          {
                              field: 'MNumber', width: 60, title: HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号(选填)"), align: 'right',

                              editor: {
                                  type: "numberbox",
                                  options: {
                                      required: false,
                                      keepText: true,
                                      align: "right",
                                      formatter: function (value) {
                                          return $(this).val() || value;
                                      }
                                  }
                              }
                          },
                           {
                               field: 'MExplanation', width: 80, title: HtmlLang.Write(LangModule.Common, "Explanation", "备注"), align: 'left',
                               editor: {
                                   type: "validatebox",
                                   options: { required: false }
                               }
                           },
                          {
                              field: 'MItemID', width: 80, title: HtmlLang.Write(LangModule.FP, "ItemName", "商品/服务"), align: 'left',
                              formatter: function (value) {
                                  //
                                  return value ? FPHome.Items.where("x.MItemID =='" + value + "'")[0].MText : "";
                              },
                              editor: {
                                  type: 'addCombobox',
                                  options: {
                                      type: "inventory",
                                      addOptions: {
                                          //是否有联系人编辑权限
                                          hasPermission: true,
                                          //弹出框关闭后的回调函数
                                          callback: function () { }
                                      },
                                      dataOptions: {
                                          height: "24px",
                                          required: false,
                                          textField: "MText",
                                          data: FPHome.Items,
                                          //数据加载成功后更新数据源
                                          onLoadSuccess: function (msg) {

                                          },
                                          onSelect: function (rec) { },
                                          formatter: function (row) {
                                              var opts = $(this).combobox('options');
                                              return row[opts.textField];
                                          }
                                      }
                                  }

                              }
                          },
                          {
                              field: 'Operation', width: 40, title: HtmlLang.Write(LangModule.Common, "Operation", "操作"), align: 'center', formatter: function (val, rec, rowIndex) {
                                  //如果是业务单据 必须是没有完成初始化的情况下
                                  var text = '<div class="list-item-action">';

                                  //发票查看
                                  text += !rec.MID ? '' : '<a href="javascript:void(0)" style="margin-right:5px" class="' + viewFapiaoButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '">&nbsp;</a>';
                                  //发送发票
                                  text += !rec.MID ? '' : '<a href="javascript:void(0)" class="' + sendFapiaoButton.trimStart('.')+'" style="margin-right:5px ;color: black;font-weight: 500;font-size: large;"   rowindex="' + rowIndex + '" mid="' + rec.MID + '">B</a>';

                                  
                                  //删除勾兑关系
                                  text += !rec.MID ? '' : '<a href="javascript:void(0)" style="margin-right:5px" class="' + removeReconcileButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '">&nbsp;</a>';

                                  //而且可以编辑
                                  text += rec.MSource == 0 ? ('<a href="javascript:void(0)" style="margin-right:5px" class="list-item-edit ' + editFapiaoButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>') : "";
                                  //
                                  if (rec.MID) {
                                      ///add by 锦友 20180502 16:14:00
                                      ///去掉删除按钮，原因：
                                      ///1、已勾对的发票不能删除；
                                      ///2、删除发票与开票单的勾对关系，在当前列表页面该发票数据就不显示了，是发票明细页面中，该数据就是未勾对状态，可以进行删除
                                      //if (rec.MSource == 0) {
                                      //    text += '<a href="javascript:void(0)" class="m-icon-delete-row ' + deleteFapiaoButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>';
                                      //}
                                  }
                                  else {
                                      text += '<a href="javascript:void(0)" class="m-icon-remove-row ' + removeFapiaoButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>';
                                  }
                                  //
                                  text += '</div>';
                                  //
                                  return text;
                              }
                          }
                    ]],
                    onClickRow: function (rowIndex, rowData, event) {
                        //如果点击的是添加和删除按钮，则不进行编辑
                        if (!$(event.srcElement || event.target).is("a")) {
                            //如果校验不通过
                            if (!FPHome.ValidateEditRow($(this), that.editIndex)) {
                                //直接返回
                                return false;
                            }
                            //如果是导入的发票，则不能进行编辑
                            if (rowData.MSource == 1 || rowData.MSource == 2) return;
                            //如果是多行的，只能在弹出框里面进行编辑
                            if (rowData.MHasDetail && rowData.MID) {
                                //
                                $(editFapiaoButton + "[mid='" + rowData.MID + "']").trigger("click");
                                //
                                return false;
                            }
                            //如果校验不通过
                            if (!FPHome.ValidateEditRow(tableBody, that.editIndex)) {
                                //直接返回
                                return false;
                            }
                            //
                            if (that.editIndex != null) {
                                //
                                $(this).datagrid("unselectAll");
                                //
                                $(this).datagrid("selectRow", that.editIndex);
                            }
                            //结束编辑
                            $(this).datagrid("beginEdit", rowIndex);
                            //全选
                            Megi.regClickToSelectAllEvt();
                            //
                            $(this).datagrid("unselectAll");
                            //
                            that.editIndex = rowIndex;
                            //
                            that.bindTableRowAmountInputEvent(rowIndex);
                            //
                            $(this).datagrid("selectRow", that.editIndex);
                            //
                            that.initTableEvent();
                        }
                    },
                    onBeforeEdit: function (rowIndex, rowData) {
                        //
                        if (that.editIndex != null) {
                            //
                            var rowData = $(tableBody).datagrid("getSelected");
                            //
                            if (rowData) {
                                //
                                var editRow = that.getEditRowData();
                                //
                                var newData = $.extend(true, {}, rowData, editRow);
                                //
                                $(tableBody).datagrid("updateRow", {
                                    index: that.editIndex,
                                    row: newData
                                });
                            }
                        }
                    },
                    onAfterEdit: function (rowIndex, rowData, changes) {
                        //
                        that.initTableEvent();
                        //结束编辑
                        that.editIndex = null;
                    },
                    onLoadSuccess: function () {
                        //如果没有数据则添加一个空行，供用户编辑
                        if ((!data || data.length == 0)) {
                            //添加一个空行，用于用户编辑
                            that.insertEmptyRow();
                        }
                        that.updateTotalAmount();
                        //
                        that.resizeTableHeight();
                        //
                        that.initTableEvent();
                    }
                });
            };
            //校验抵扣月份的问题
            this.validateDeducationDate = function (bizDate, deductionDate) {
                //只校验采购发票
                if (invoiceType == 1 && that.editIndex != null) {
                    //
                    var fromDeductionDate = !!deductionDate;
                    //
                    var deductionDateEditor = $($(tableBody).datagrid("getEditors", that.editIndex).where("x.field == 'MDeductionDate'")[0].target);
                    //
                    var bizDateEditor = $($(tableBody).datagrid("getEditors", that.editIndex).where("x.field == 'MBizDate'")[0].target);
                    //
                    bizDate = mDate.parse(bizDate ? bizDate : bizDateEditor.datebox("getValue"));
                    //
                    deductionDate = FPHome.ParseYearMonth2Date(deductionDate ? deductionDate : deductionDateEditor.val());
                    //
                    if (deductionDate && (deductionDate.getFullYear() * 12 + deductionDate.getMonth()) < (bizDate.getFullYear() * 12 + bizDate.getMonth())) {
                        //
                        fromDeductionDate && mDialog.message(HtmlLang.Write(LangModule.FP, "VefrifyDateMustLargeThanFapiaoDate", "发票的认证月份必须大于或等于发票开票月份！"));
                        //
                        deductionDateEditor.val("");
                        //
                        return false;
                    }
                }
            }
            //
            //绑定输入框里面的keyup事件
            this.bindTableRowAmountInputEvent = function (rowIndex) {
                //找到各个编辑的框
                var editors = $(tableBody).datagrid('getEditors', that.editIndex);
                //
                for (var i = 0; i < editors.length ; i++) {
                    //
                    var easyUIControl = $(editors[i].target).mIsEasyUIControl();
                    //
                    var targetInput = editors[i].target;
                    //
                    if (easyUIControl) {
                        //
                        targetInput = easyUIControl.getInput();
                    }
                    //
                    if (editors[i].field == 'MTotalAmount') {
                        //
                        $(targetInput).off("keyup.id").on("keyup.id", function (e) {
                            //
                            if (e.keyCode == FPHome.EnterKey) {
                                //
                                that.editIndex = FPHome.EndEditAndGoToNextRow(tableBody, that.editIndex, that.getEmptyRowData());
                                //
                                that.bindTableRowAmountInputEvent();
                                //
                                that.initTableEvent();
                                //
                                that.resizeTableHeight();
                            }
                            else {
                                //如果是金额输入框则需要更新差额
                                that.updateTotalAmount();
                            }
                        });
                    }
                    else if (editors[i].field == 'MTaxAmount') {
                        //
                        $(targetInput).off("keyup.id").on("keyup.id", function (e) {
                            //
                            if (e.keyCode == FPHome.EnterKey) {
                                //
                                that.editIndex = FPHome.EndEditAndGoToNextRow(tableBody, that.editIndex, that.getEmptyRowData());
                                //
                                that.bindTableRowAmountInputEvent()
                                //
                                that.initTableEvent();
                                //
                                that.resizeTableHeight();
                            }
                            else {
                                //如果是金额输入框则需要更新差额
                                that.updateTotalAmount();
                            }
                        });
                    }
                    else {
                        //
                        $(targetInput).off("keyup.id").on("keyup.id", function (e) {
                            //
                            if (e.keyCode == FPHome.EnterKey) {
                                //
                                that.editIndex = FPHome.EndEditAndGoToNextRow(tableBody, that.editIndex, that.getEmptyRowData());
                                //
                                that.bindTableRowAmountInputEvent();
                                //
                                that.initTableEvent();
                                //
                                that.resizeTableHeight();
                            }
                        });
                    }
                }
            };
            //更新下面的金额
            this.updateTotalAmount = function () {
                //
                var rows = that.getRowData();
                //
                var taxAmountValue = 0, totalAmountValue = 0;
                //
                var tableAmount = $(txtTotalAmount).val();
                //
                var ajustAmount = $(txtAjustAmount).val();
                //
                for (var i = 0; i < rows.length ; i++) {
                    //
                    taxAmountValue += (+rows[i].MTaxAmount);
                    //
                    totalAmountValue += (+rows[i].MTotalAmount);
                }
                //
                $(taxAmount).empty().text(mMath.toMoneyFormat(taxAmountValue));
                //
                $(totalAmount).empty().text(mMath.toMoneyFormat(totalAmountValue));
                //
                $(notaxAmount).empty().text(mMath.toMoneyFormat(totalAmountValue - taxAmountValue));
                //
                $(unIssuedAmount).empty().text(mMath.toMoneyFormat(tableAmount - totalAmountValue - ajustAmount));
            };
            //初始化开票单的基本信息
            this.bindTableBaseInfo = function (data) {
                //
                if (data.MContactID) {
                    //联系人
                    var contactName = mText.htmlDecode(data.MContactName || "");
                    $(txtContact).val(contactName).attr("MContactID", data.MContactID);
                    //
                    $(txtContact).attr("etitle", contactName).tooltip({ content: contactName || "" });
                }
                //税号
                $(txtContactTaxCode).val(data.MContactTaxCode || "");

                //发票号
                $(txtNumber).numberbox("setValue", data.MNumber);
                //发票类型
                $(txtType).combobox("setValue", data.MType || "");
                $("#selPaidTo").combobox("setValue", data.MBankId || "");

                //开票日期
                $(txtDate).datebox("setValue", data.MBizDate || new Date());

                //备注
                $(txtExplanation).val(data.MExplanation || "");
                //总金额
                $(txtTotalAmount).val(data.MTotalAmount);
                //总金额
                $(txtTaxAmount).val(data.MTaxAmount);
                //调整金额
                $(txtAjustAmount).val(data.MAjustAmount);
                //
                //that.updateTotalAmount();
            };
            //获取开票单的基本信息
            this.getTableBaseInfo = function () {
                //
                mAjax.Post(getTableBaseInfoUrl, { tableId: tableId, invoiceIds: invoiceIds, invoiceType: invoiceType }, function (data) {
                    //
                    tableData = data;
                    //
                    that.bindTableBaseInfo(data);
                    that.getFapiaoList();
                }, "", true);
            };
            //获取开票单下面的发票信息
            this.getFapiaoList = function () {
                //
                mAjax.post(getFapiaoListUrl, { tableId: tableId, invoiceIds: invoiceIds }, function (data) {
                    //
                    that.bindGridData(data);
                }, "", true);
            }
            //
            this.resizeTableHeight = function () {

            };
            //
            this.initTableEvent = function () {
                //
                $(editFapiaoButton).off("click").on("click", function () {
                    //
                    var index = FPHome.GetGridRowIndexByCellItem(tableBody, $(this));
                    //
                    var row = that.getRowData(index);
                    //
                    row = $.extend(true, {}, row, {
                        MID: $(this).attr("mid") || "",
                        MTableID: tableData.MItemID || ""
                    });
                    //编辑发票
                    that.editFapiao(index, row);
                    //
                }).tooltip({ content: HtmlLang.Write(LangModule.FP, "Click2EditFapiao", "点击编辑发票") });
                //查看
                $(viewFapiaoButton).off("click").on("click", function () {

                    home.viewFapiao($(this).attr("mid"));
                }).tooltip({ content: HtmlLang.Write(LangModule.FP, "Click2ViewFapiao", "点击查看发票") });


                //发送发票
                $(sendFapiaoButton).off("click").on("click", function () {
                    var mid = $(this).attr("mid");
                    mAjax.Post(FPSendBWFapiao +"?fapiaoId="+mid,{}, function (data) {
                        if (data.result) {
                            mDialog.message(HtmlLang.Write(LangModule.FP, "SendSuccessful", "发送成功") );
                        }
                        else {
                            mDialog.error(data.msg);
                        }
                    })

                    //home.viewFapiao();
                }).tooltip({ content: HtmlLang.Write(LangModule.FP, "Click2SendBWFapiao", "点击发送至百旺云") });



                //删除勾兑关系
                $(removeReconcileButton).off("click").on("click", function () {
                    //
                    var index = FPHome.GetGridRowIndexByCellItem(tableBody, $(this));
                    //
                    var row = that.getRowData(index);
                    //
                    that.removeFapiaoReconcile([{ MID: row.MID, MNumber: row.MNumber, index: index }]);
                }).tooltip({ content: HtmlLang.Write(LangModule.FP, "Click2DeleteFapiaoReconcile", "点击删除发票与开票单勾兑关系") });
                //删除
                $(deleteFapiaoButton).off("click").on("click", function () {
                    //
                    var index = FPHome.GetGridRowIndexByCellItem(tableBody, $(this));
                    //
                    var row = that.getRowData(index);
                    //
                    that.deleteFapiao([{ MID: row.MID, MNumber: row.MNumber, index: index }]);
                }).tooltip({ content: HtmlLang.Write(LangModule.FP, "Click2DeleteFapiao", "点击删除发票") });
                //删除发票
                $(removeFapiaoButton).off("click").on("click", function () {
                    //
                    var index = FPHome.GetGridRowIndexByCellItem(tableBody, $(this));
                    //
                    $(tableBody).datagrid("deleteRow", index);
                    //
                    if (index == that.editIndex) {
                        //
                        that.editIndex = null;
                    }
                    //
                    if (that.editIndex != null && index < that.editIndex) {
                        //
                        that.editIndex = that.editIndex - 1;
                    }
                    //如果没有一行了，则添加一个空行
                    if (that.getRowData().length == 0) {
                        //
                        that.insertEmptyRow(0, that.getEmptyRowData());
                    }
                    //
                    that.updateTotalAmount();
                });
                //添加一行
                $(addFapiaoButton).off("click").on("click", function () {
                    //
                    var index = FPHome.GetGridRowIndexByCellItem(tableBody, $(this));
                    //
                    var row = that.getRowData(index);
                    //复制这一行
                    var copyRow = {
                        MTaxAmount: row.MTaxAmount,
                        MTotalAmount: row.MTotalAmount,
                        MBizDate: row.MBizDate,
                        MExplanation: row.MExplanation,
                        MItemID: row.MItemID
                    }
                    //
                    that.insertEmptyRow(index + 1, copyRow);
                    //如果是在编辑行上方加一行，那编辑行的序号要加一
                    if (that.editIndex != null && index + 1 <= that.editIndex) {
                        //
                        that.editIndex = that.editIndex + 1;
                    }
                    //
                    that.updateTotalAmount();
                    //
                    that.initTableEvent();
                });
            };
            //删除开票单
            this.deleteFapiao = function (fapiaos) {
                //
                var fapiaoNumbers = fapiaos.select("MNumber").join(',');
                //
                var sureTitle = fapiaoNumbers ? (HtmlLang.Write(LangModule.FP, "AreYouSureToDeleteFapiaosAsFollow", "您是否确认删除以下发票:") + fapiaoNumbers) : (HtmlLang.Write(LangModule.FP, "AreYouSureToDeleteFapiaos", "您是否确认删除发票?"));
                //
                fapiaos && fapiaos.length > 0 && mDialog.confirm(sureTitle, function () {
                    //
                    mAjax.submit(removeFapiaoListUrl, { fapiaoIds: fapiaos.select("MID").join(",") }, function (data) {
                        //提示保存成功
                        mDialog.message(LangKey.DeleteSuccessfully);
                        //
                        $(tableBody).datagrid("deleteRow", fapiaos[0].index);
                        //
                        that.updateTotalAmount();
                        //如果没有一行了，则添加一个空行
                        if (that.getRowData().length == 0) {
                            //
                            that.insertEmptyRow(0, that.getEmptyRowData());
                        }
                    }, "", true);
                });
            };

            //删除开票单
            this.removeFapiaoReconcile = function (fapiaos) {

                if (!tableId) {
                    return;
                }
                //
                var fapiaoNumbers = fapiaos.select("MNumber").join(',');
                //
                var sureTitle = HtmlLang.Write(LangModule.FP, "AreYouSureToRemoveFapiaoReoncile", "您是否确认删除发票与开票单的勾兑关系?");
                //
                fapiaos && fapiaos.length > 0 && mDialog.confirm(sureTitle, function () {
                    //
                    mAjax.submit(removeReconcileUrl, {
                        model: {
                            MTable: {
                                MItemID: tableId
                            },
                            MFapiaoList: fapiaos
                        }
                    }, function (data) {
                        //提示保存成功
                        mDialog.message(LangKey.DeleteSuccessfully);
                        //
                        $(tableBody).datagrid("deleteRow", fapiaos[0].index);
                        //
                        that.updateTotalAmount();
                        //如果没有一行了，则添加一个空行
                        if (that.getRowData().length == 0) {
                            //
                            that.insertEmptyRow(0, that.getEmptyRowData());
                        }
                    }, "", true);
                });
            };

            //编辑发票
            this.editFapiao = function (index, row) {
                //
                var title = HtmlLang.Write(LangModule.FP, "EditSaleFapiao", "编辑销售发票");
                //
                if (invoiceType == 1) {
                    //
                    title = HtmlLang.Write(LangModule.FP, "EditPurchaseFapiao", "编辑采购发票");
                }
                //
                var totalAmont = that.getRowData().where("!!x.MID").sum("MTotalAmount");
                //
                var totalTaxAmont = that.getRowData().where("!!x.MID").sum("MTaxAmount");
                //需要剔除本行
                var currentRow = that.getRowData(index);
                //
                if (currentRow.MID) {
                    //
                    totalAmont = totalAmont.sub(currentRow.MTotalAmount);
                    //
                    totalTaxAmont = totalTaxAmont.sub(currentRow.MTaxAmount);
                }
                //
                var maxAmount = (+tableData.MTotalAmount).sub(totalAmont);
                //
                var maxTaxAmount = (+$(txtTaxAmount).val()).sub(totalTaxAmont);
                //
                var param = "mid=" + (row.MID || "") + "&tableId=" + (tableData.MItemID || "") + "&invoiceType=" + invoiceType + "&contactId=" + (tableData.MContactID || "") + "&number=" + (row.MNumber || "") + "&date=" + row.MBizDate + "&explanation=" + (row.MExplanation || "") + "&maxAmount=" + maxAmount + "&maxTaxAmount=" + maxTaxAmount + "&tableNumber=" + $(txtNumber).val();
                //
                param = encodeURI(param);
                //
                mDialog.show({
                    mTitle: title,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + editFapiaoUrl + "?" + param,
                    mCloseCallback: function (fapiao) {
                        //
                        that.updateGridRow(index, fapiao);
                    }
                });
                $.mDialog.max();
            };
            //更新某一行的数据,当用户从发票列表保存成功后，重新过来更新本行的数据
            this.updateGridRow = function (index, fapiao) {
                //
                if (fapiao && fapiao.MID) {
                    //
                    $(tableBody).datagrid("updateRow", {
                        index: index,
                        row: {
                            MID: fapiao.MID,
                            MFapiaoEntrysCount: fapiao.MFapiaoEntrys.length,
                            MNumber: fapiao.MNumber,
                            MTaxAmount: fapiao.MTaxAmount,
                            MBizDate: fapiao.MBizDate,
                            MItemID: fapiao.MFapiaoEntrys[0].MItemID,
                            MTotalAmount: fapiao.MTotalAmount,
                            MHasDetail: true,
                            MDeductionDate: fapiao.MDeductionDate,
                            MExplanation: fapiao.MExplanation,
                            MOperation: "",
                            MStatus: fapiao.MTotalAmount > 0 ? 1 : -1
                        }
                    });
                    //如果正在当前编辑行的话，会结束编辑
                    if (index == that.editIndex) {
                        //
                        that.editIndex = null;
                    }
                    //
                    that.updateTotalAmount();
                    //
                    that.initTableEvent();
                }
            };
            //
            this.validate = function () {
                //
                if (!$(topDiv).mValidateEasyUI()) {
                    return false;
                }
                //
                if (!FPHome.ValidateEditRow(tableBody, that.editIndex)) {
                    return false;
                }
                //
                return true;
            };
            //处理时间的问题
            this.handleDatetime = function (fapiaoList) {
                //
                for (var i = 0; i < fapiaoList.length ; i++) {
                    //转化一次
                    fapiaoList[i].MBizDate = mDate.format(fapiaoList[i].MBizDate);
                }
            };
            //校验每一行的发票是否合法
            this.validateFapiao = function (fapiaos) {
                if (!fapiaos || fapiaos.length == 0) return true;

                for (var i = 0; i < fapiaos.length ; i++) {

                    //发票总金额不可为0
                    if (fapiaos[i].MTotalAmount == 0) {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "FapiaoTotalAmountCannotEqualWith0", "发票总金额不可为0"));
                        return false;
                    }

                    //发票税金不可大于等于总金额
                    if (+(fapiaos[i].MTaxAmount) * +(fapiaos[i].MTotalAmount) >= +(fapiaos[i].MTotalAmount) * +(fapiaos[i].MTotalAmount)) {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "FapiaoTaxAmountCannotLargerThanTableAmount", "发票税额金额必须小于总金额"));
                        return false;
                    }
                }

                return true;
            },
            //保存开票单以及发票信息
            this.saveTable = function () {
                //
                if (!that.validate()) {
                    //
                    return false;
                }
                //如果正在编辑，则先结束编辑
                if (that.editIndex != null) {
                    //
                    $(tableBody).datagrid("endEdit", that.editIndex);
                    //
                    that.initTableEvent();
                }
                //如果没有联系人的话
                if (!tableData.MContactID) {
                    //获取值
                    tableData.MContactID = $(txtContact).combobox("getValue");
                }
                //开票单号
                tableData.MContactTaxCode = $(txtContactTaxCode).val();
                //先获取开票单的基本信息
                tableData.MNumber = parseInt($(txtNumber).numberbox("getValue"));
                //
                tableData.MBizDate = $(txtDate).datebox("getValue");
                //
                tableData.MExplanation = $(txtExplanation).val();
                //是增值税专用发票还是普通发票
                tableData.MType = $(txtType).combobox("getValue");
                //
                tableData.MBankId = $("#selPaidTo").combobox("getValue");
                
                //
                tableData.MInvoiceType = invoiceType;
                //总额
                tableData.MAjustAmount = +$(txtAjustAmount).val();
                //总额
                tableData.MTaxAmount = +$(txtTaxAmount).val();
                //总额
                tableData.MTotalAmount = +$(txtTotalAmount).val();
                //发票分录
                tableData.fapiaoList = that.getRowData();
                //处理时间显示问题
                that.handleDatetime(tableData.fapiaoList);

                if (!this.validateFapiao(tableData.fapiaoList)) return false;
                //
                tableData.invoiceIdList = invoiceIds.split(',');
                //调整金额不可以大于开票金额
                if (FPHome.IsOuterOfRange(tableData.MTotalAmount, tableData.MAjustAmount)) {
                    //提醒用户，调整金额的值不可超过总金额
                    mDialog.message(HtmlLang.Write(LangModule.FP, "AdjustAmountCannotLargerThanTableAmount", "调整金额不可超过总金额"));
                    //
                    return false;
                };
                //税额也不可超过开票金额
                if (FPHome.IsOuterOfRange(tableData.MTotalAmount, tableData.MTaxAmount)) {
                    //提醒用户，税额也不可超过开票金额
                    mDialog.message(HtmlLang.Write(LangModule.FP, "TaxAmountCannotLargerThanTableAmount", "税额金额不可超过总金额"));
                    //
                    return false;
                };
                //税额必须同开票单金额相同，因为要么是红字开票单要么是正常开票单
                if (tableData.MTotalAmount * tableData.MTaxAmount < 0) {
                    //提醒用户，税额也不可超过开票金额
                    mDialog.message(HtmlLang.Write(LangModule.FP, "TaxAmountMustSamePositiveOrNegativeWithTableAmount", "税额金额必须和开票单金额同为正数或者负数"));
                    //
                    return false;
                }
                //
                //总额 - 调整金额，不可以小于 列表的金额
                if (Math.abs(tableData.MTotalAmount - tableData.MAjustAmount) < Math.abs(tableData.fapiaoList.sum("MTotalAmount"))) {
                    //
                    mDialog.message(HtmlLang.Write(LangModule.FP, "FapiaoAmountCannotLargerThanTableAmount", "发票总金额不可大于开票单的总金额"));
                    //
                    return false;
                }
                //总额 - 调整金额，不可以小于 列表的金额
                if (Math.abs(tableData.MTaxAmount - tableData.MAjustAmount) < Math.abs(tableData.fapiaoList.sum("MTaxAmount"))) {
                    //
                    mDialog.message(HtmlLang.Write(LangModule.FP, "FapiaoTaxAmountCannotLargerThanTableTaxAmount", "发票总税额不可大于开票单的总税额"));
                    //
                    return false;
                }
                //判断是否存在重复编号
                var numbers = tableData.fapiaoList.where("!!x.MNumber").select("MNumber");
                //
                if (numbers.length > numbers.distinct().length) {
                    //
                    mDialog.message(HtmlLang.Write(LangModule.FP, "ExistsDuplicatedFapiaoNumber", "存在重复的发票编号!"));
                    //
                    return false;
                }
                //提交
                mAjax.submit(saveTableUrl, { table: tableData }, function (data) {
                    //
                    mDialog.message(LangKey.SaveSuccessfully);
                    //
                    mWindow.reload(FPHome.editTableUrl + "?tableId=" + data.ObjectID + "&invoiceType=" + invoiceType);
                });
            };
            //
            this.insertEmptyRow = function (index, row) {
                //对于空行
                row = row || that.getEmptyRowData();
                //插入到数据中
                $(tableBody).datagrid("insertRow", { index: index, row: row });
                //
                that.initTableEvent();
                //
                that.resizeTableHeight();
            };
            //
            this.getEmptyRowData = function () {
                //
                return $.extend(true, {}, emptyRow);
            };
        };
        return FPEditTable;
    })();
    //
    w.FPEditTable = FPEditTable;
})(window)