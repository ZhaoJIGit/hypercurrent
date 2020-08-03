(function (w) {

    var FPEditFapiao = (function () {
        //
        var FPEditFapiao = function (mid, tableId, invoiceType, contactId, number, explanation, date, tableNumber, maxAmount, maxTaxAmount) {
            //
            var that = this;
            //
            date = mDate.parse(date);
            //
            contactId = contactId || "";
            //
            number = number || "";
            //
            maxAmount = +maxAmount || 0;
            //
            maxTaxAmount = +maxTaxAmount || 0;
            //
            var hasTax = maxTaxAmount != 0;
            //
            explanation = explanation || "";
            //
            var tableBody = ".ef-fapiao-table";
            //
            var datagridDiv = ".ef-fapiao-div";
            //
            var totalDiv = ".ef-total-div";
            //
            var topDiv = ".ef-top-div";
            //
            var cancelButton = ".ef-cancel-button";
            //
            var saveButton = ".ef-save-button";
            //
            var txtContact = "#txtContact";
            //
            var txtNumber = "#txtNumber";
            //
            var txtTableNumber = "#txtTableNumber";
            //
            var txtExplanation = "#txtExplanation";
            //
            var txtAjustAmount = "#txtAjustAmount";
            //
            var txtDeductionDate = "#txtDeductionDate";
            //
            var txtDate = "#txtDate";
            //
            var addEntryButton = ".add-entry-button";
            //
            var removeEntryButton = ".remove-entry-button";
            //
            var notTaxAmount = "tr.ef-notax-amount td:eq(1)";
            //
            var taxAmount = "tr.ef-tax-amount td:eq(1)";
            //
            var totalAmount = "tr.ef-total-amount td:eq(1)";
            //
            var getFapiaoUrl = "/FP/FPHome/GetFapiao";
            //
            var saveFapiaoUrl = "/FP/FPhome/FPSaveFapiao";
            //编辑发票的界面
            var editFapiaoUrl = "/FP/FPHome/FPEditFapiao";
            //
            var isFapiaoUpdated = false;
            //
            var fapiaoData = {};
            //
            this.editIndex = null;
            //日志历史按钮
            var aHistory = "#aHistory";
            //
            var hidInvoiceId = "#hidInvoiceId";
            //
            var DataSourceTaxRate = [];
            //空行的数据
            var emptyRow = {
                MEntryID: "",
                MItemName: "",
                MItemType: "",
                MUnit: "",
                MQuantity: "",
                MPrice: "",
                MAmount: "",
                MTaxID: "",
                MTaxAmount: "",
                MTax: ""
            };
            //
            var fapiaoData = {};
            //
            this.init = function () {
                //初始化各个控件
                that.initDomValue();
                //
                that.getFapiaoData();
                //
                that.initEvent();
            };

            //初始化各个事件
            this.initEvent = function () {
                //取消
                $(cancelButton).off("click").on("click", function () {
                    //如果发票保存过了，则需要把发票的全部信息提交到底层
                    mDialog.close();
                });
                //保存按钮
                $(saveButton).off("click").on("click", function () {
                    //
                    that.saveFapiao();
                });

                //显示日志的功能
                $(aHistory).off("click").on("click", function () {
                    //获取查看日志的ID
                    var faPiaomId = $(hidInvoiceId).val();

                    !!faPiaomId ? HistoryView.openDialog(faPiaomId, "Sale_FaPiao") : "";
                });
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
            //校验发票里面的数据是否正确
            this.validateFapiaoRow = function (fapiao) {
                //
                for (var i = 0; i < fapiao.MFapiaoEntrys.length ; i++) {
                    //
                    var entry = fapiao.MFapiaoEntrys[i];
                    //
                    if (entry.MPrice == 0) {
                        //
                        //
                        mDialog.message(HtmlLang.Write(LangModule.FP, "PriceOfFaiaoEntryCannotBeZero", "发票分录的单价不可为0!"));
                        //
                        that.editRow(undefined, entry);
                        //
                        return false;
                    }
                    //
                    if (entry.MQuantity <= 0) {
                        //
                        //
                        mDialog.message(HtmlLang.Write(LangModule.FP, "QuantityOfFaiaoEntryCannotBeZero", "发票分录的数量必须大于0!"));
                        //
                        that.editRow(undefined, entry);
                        //
                        return false;
                    }
                }
                //
                return true;
            };
            //编辑某一行
            this.editRow = function (index, row) {
                //
                index = index == undefined ? $(tableBody).datagrid("getRowIndex", row) : index;
                //
                $(tableBody).datagrid("beginEdit", index);
            }
            //保存发票
            this.saveFapiao = function () {
                //
                if (!that.validate()) {
                    //
                    return false;
                }
                //如果正在编辑，则先结束编辑
                if (that.editIndex != null) {
                    //
                    $(tableBody).datagrid("endEdit", that.editIndex);
                }
                //
                var fapiaoData = that.getFapiao();
                //如果没有分录，则不能保存
                if (!fapiaoData.MFapiaoEntrys || fapiaoData.MFapiaoEntrys.length == 0) {
                    //
                    mDialog.error(HtmlLang.Write(LangModule.FP, "OneValidEntryAtMostNeedForFapiao", "一张发票至少需要一条有效分录!"))
                    //
                    return false;
                }
                //
                if (!that.validateFapiaoRow(fapiaoData)) {
                    //
                    return false;
                }
                //
                if (maxAmount != 0 && Math.abs(fapiaoData.MTotalAmount) > Math.abs(maxAmount)) {
                    //
                    mDialog.error(HtmlLang.Write(LangModule.FP, "MaxAmountOfThisFapiaoIs", "本张发票的最大开票金额为:") + mMath.toMoneyFormat(maxAmount));
                    //
                    return false;
                }
                //
                if (maxTaxAmount != 0 && Math.abs(fapiaoData.MTaxAmount) > Math.abs(maxTaxAmount)) {
                    //
                    mDialog.error(HtmlLang.Write(LangModule.FP, "MaxTaxAmountOfThisFapiaoIs", "本张发票的最大开票税额为:") + mMath.toMoneyFormat(maxTaxAmount));
                    //
                    return false;
                }
                //抵扣月份必须大于发票日期所在月份
                if (fapiaoData.MDeductionDate && mDate.parse(fapiaoData.MDeductionDate)) {
                    //
                    var deductionDate = mDate.parse(fapiaoData.MDeductionDate);
                    //发票所在月份
                    var fapiaoDate = mDate.parse(fapiaoData.MBizDate);
                    //
                    if ((deductionDate.getFullYear() * 12 + deductionDate.getMonth()) < (fapiaoDate.getFullYear() * 12 + fapiaoDate.getMonth())) {
                        //
                        mDialog.error(HtmlLang.Write(LangModule.FP, "DeductionDateMustLargeThanFapiaoDate", "发票的抵扣月份必须大于发票本身月份"));
                        //
                        return false;
                    }
                }
                //
                mAjax.submit(saveFapiaoUrl, { fapiao: fapiaoData }, function (data) {
                    //
                    if (data) {
                        //
                        if (!data.MNumber) {
                            //
                            mDialog.error(HtmlLang.Write(LangModule.FP, "FapiaoNumberDuplicated", "发票号已经被使用了，请重新输入"));
                        }
                        else {
                            //先提醒用户保存成功
                            mDialog.message(LangKey.SaveSuccessfully);
                            //
                            fapiaoData = data;
                            //如果发票保存过了，则需要把发票的全部信息提交到底层
                            mDialog.setParam(0, fapiaoData);
                            //
                            var param = "mid=" + fapiaoData.MID + "&invoiceType=" + invoiceType + "&tableId=" + fapiaoData.MTableID + "&contactId=" + fapiaoData.MContactID + "&maxAmount=" + maxAmount + "&maxTaxAmount=" + maxTaxAmount + "&tableNumber=" + tableNumber;
                            //
                            mWindow.reload(editFapiaoUrl + "?" + param);
                        }
                    }
                });
            };
            //获取发票页面上的信息
            this.getFapiao = function () {
                //
                fapiaoData.MNumber = $(txtNumber).val();
                //开票日期
                fapiaoData.MBizDate = $(txtDate).datebox("getValue");
                //备注
                fapiaoData.MExplanation = $(txtExplanation).val();
                //
                fapiaoData.MInvoiceType = invoiceType;
                //调整金额
                fapiaoData.MFapiaoEntrys = that.getFapiaoEntrys();
                //
                fapiaoData.MAmount = fapiaoData.MFapiaoEntrys.sum("MAmount");
                //
                fapiaoData.MTaxAmount = fapiaoData.MFapiaoEntrys.sum("MTaxAmount");
                //
                fapiaoData.MTotalAmount = fapiaoData.MAmount + fapiaoData.MTaxAmount;
                //
                fapiaoData.MDeductionDate = mDate.format(FPHome.ParseYearMonth2Date($(txtDeductionDate).val()));

                fapiaoData.MVerifyDate = fapiaoData.MDeductionDate;
                //
                fapiaoData.MHasDetail = true;
                //
                fapiaoData.MTableID = tableId;
                //
                return fapiaoData;
            };
            //初始化控件
            this.initDomValue = function () {
                //联系人不需要选择
                //发票号可以修改
                //到期日默认为当天
                $(txtDate).datebox({}).datebox("setValue", new Date());
                //备注不需要填
                $(txtTableNumber).val(FPHome.GetFullTableNumber(tableNumber, invoiceType));
            }

            //
            this.getFapiaoData = function () {
                //
                mAjax.post(getFapiaoUrl, {
                    fapiao:
                        {
                            MID: mid, MTableId: tableId, MContactID: contactId, MBizDate: date, MExplanation: explanation, MNumber: number, MMaxAmount: maxAmount
                        }
                }, function (data) {
                    //
                    if (data) {
                        //
                        fapiaoData = data;
                        //主表
                        that.bindFapiaoBaseInfo(data);
                        //分录
                        that.bindGridData(data.MFapiaoEntrys);
                    }
                }, "", true);
            };
            //
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
                    height: ($("body").height() - $(datagridDiv).offset().top - $(totalDiv).height() - 40 - (invoiceType == "1" ? 20 : 0) - 27),
                    columns: [[
                          {
                              field: 'MEntryID', title: '', width: 20, align: 'left', formatter: function (rec) {
                                  //
                                  return "<a href='javascript:void(0);' class='m-icon-add-row add-entry-button' id='" + rec + "' style='text-align:center'>&nbsp;</a>";
                              }
                          },
                           {
                               field: 'MItemID', width: 120, title: HtmlLang.Write(LangModule.FP, "ItemID", "商品/服务"), align: 'left',
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
                                           required: true,
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
                               field: 'MItemType', width: 70, title: HtmlLang.Write(LangModule.FP, "ItemType", "规格型号"), align: 'left',
                               editor: {
                                   type: "validatebox",
                                   options: { required: false }
                               }
                           },
                           {
                               field: 'MUnit', width: 30, title: HtmlLang.Write(LangModule.FP, "ItemUnit", "单位"), align: 'left',
                               editor: {
                                   type: "validatebox",
                                   options: { required: false }
                               }
                           },
                           {
                               field: 'MQuantity', width: 30, align: 'right', title: HtmlLang.Write(LangModule.FP, "Quantity", "数量"), align: 'right',
                               editor: {
                                   type: "numberbox",
                                   options: { precision: 4, minPrecision: 0, required: true }
                               }
                           },
                          {
                              field: 'MPrice', width: 30, align: 'right', title: HtmlLang.Write(LangModule.FP, "Price", "单价"), align: 'right', formatter: function (value) {
                                  return mMath.toMoneyFormat(value);
                              },
                              editor: {
                                  type: "numberbox",
                                  options: { precision: 8, minPrecision: 0, required: true }
                              }
                          },
                          {
                              field: 'MAmount', width: 50, align: 'right', title: HtmlLang.Write(LangModule.FP, "MAmountWithoutTax", "未含税金额"), align: 'right', formatter: function (value) {
                                  return mMath.toMoneyFormat(value);
                              },
                              editor: {
                                  type: "numberbox",
                                  options: { precision: 4, minPrecision: 0, required: false }
                              }
                          },

                          {
                              field: 'MTaxID', width: 80, title: HtmlLang.Write(LangModule.FP, "Tax", "税率"), align: 'left',
                              editor: !hasTax ? null : {
                                  type: 'addCombobox',
                                  options: {
                                      type: "taxrate",
                                      addOptions: {
                                          //是否有基础资料编辑权限
                                          hasPermission: true,
                                          //关闭后的回调函数
                                          callback: function () { }
                                      },
                                      dataOptions: {
                                          height: "24px",
                                          //
                                          required: true,
                                          //数据加载成功后更新数据源
                                          onLoadSuccess: function (msg) {

                                          },
                                          onSelect: function (newValue, oldValue) {
                                              //先更新本行的值
                                              that.updateRowAmount();
                                              //如果是金额输入框则需要更新差额
                                              that.updateTotalAmount();
                                          },
                                          formatter: function (row) {
                                              var opts = $(this).combobox('options');
                                              return row[opts.textField];
                                          }
                                      }
                                  }
                              },
                              formatter: function (value, rowData, rowIndex) {
                                  //获取税率的名称
                                  return !value ? "" : that.getTaxRateName(value);
                              }
                          },
                         {
                             field: 'MTaxAmount', width: 50, align: 'right', title: HtmlLang.Write(LangModule.FP, "TaxAmount", "税金额"), align: 'right', formatter: function (value) {
                                 return mMath.toMoneyFormat(value);
                             }, editor: {
                                 type: "numberbox",
                                 options: { precision: 2, minPrecision: 0, required: false }
                             }
                         },
                          {
                              field: 'Operation', width: 30, title: HtmlLang.Write(LangModule.Common, "Operation", "操作"), align: 'center', formatter: function (val, rec, rowIndex) {
                                  //如果是业务单据 必须是没有完成初始化的情况下
                                  var text = '<div class="list-item-action">';
                                  //
                                  text += '<a href="javascript:void(0)" class="m-icon-remove-row ' + removeEntryButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MTableID + '" mnumber="' + rec.MTableNumber + '">&nbsp;</a>';
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
                            $(this).datagrid("selectRow", that.editIndex);
                            //
                            that.bindTableRowAmountInputEvent(rowIndex);
                            //
                            that.initTableEvent();
                        }
                    },
                    onDblClickRow: function (rowIndex, rowData) {
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
                                $.extend(true, rowData, editRow);
                                //
                                $(tableBody).datagrid("updateRow", {
                                    index: that.editIndex,
                                    row: rowData
                                });
                            }
                            //结束编辑
                            //$(this).datagrid("endEdit", that.editIndex);
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
            //绑定输入框里面的keyup事件
            this.bindTableRowAmountInputEvent = function (rowIndex) {
                //找到各个编辑的框
                var editors = $(tableBody).datagrid('getEditors', that.editIndex);
                //未含税金额字段不可编辑
                $(editors.where("x.field=='MTaxAmount'")[0].target).attr('readonly', true);
                //税金额字段不可编辑
                $(editors.where("x.field=='MAmount'")[0].target).attr('readonly', true);
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
                    if (editors[i].field == 'MQuantity') {
                        //
                        $(targetInput).off("keyup.id").on("keyup.id", function (e) {
                            //
                            if (e.keyCode == FPHome.EnterKey) {
                                //
                                FPHome.EndEditAndGoToNextRow(tableBody, that.EditIndex, that.getEmptyRowData());
                            }
                            else {
                                //先更新本行的值
                                that.updateRowAmount();
                                //如果是金额输入框则需要更新差额
                                that.updateTotalAmount();
                            }
                        });
                    }
                    else if (editors[i].field == 'MPrice') {
                        //
                        $(targetInput).off("keyup.id").on("keyup.id", function (e) {
                            //
                            if (e.keyCode == FPHome.EnterKey) {
                                //
                                FPHome.EndEditAndGoToNextRow(tableBody, that.EditIndex, that.getEmptyRowData());
                            }
                            else {
                                //先更新本行的值
                                that.updateRowAmount();
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
                                FPHome.EndEditAndGoToNextRow();
                            }
                        });
                    }
                }
            };
            //跟新本行的为含税金额和税金额
            this.updateRowAmount = function () {
                //
                if (that.editIndex != null) {
                    //
                    //找到各个编辑的框
                    var editors = $(tableBody).datagrid('getEditors', that.editIndex);
                    //
                    var quantityInput = $(editors.where("x.field=='MQuantity'")[0].target);
                    //
                    var priceInput = $(editors.where("x.field=='MPrice'")[0].target);
                    //
                    var taxAmountInput = $(editors.where("x.field=='MTaxAmount'")[0].target);
                    //
                    var amountInput = $(editors.where("x.field=='MAmount'")[0].target);
                    //获取本行
                    var quantity = quantityInput.val() || 0;
                    //
                    var price = priceInput.val() || 0;
                    //
                    amountInput.numberbox("setValue", price * quantity);

                    //税率
                    var taxInput = hasTax ? $(editors.where("x.field=='MTaxID'")[0].target) : null;
                    //
                    var tax = hasTax ? (that.getTaxRate(taxInput.combobox("getValue")) || 0) : 0;
                    //
                    taxAmountInput.numberbox("setValue", price * quantity * tax);
                }
            };

            //更新下面的金额
            this.updateTotalAmount = function () {
                //
                var rows = that.getRowData();
                //
                var taxAmountValue = 0, notaxAmountValue = 0, totalAmountValue = 0;
                //
                for (var i = 0; i < rows.length ; i++) {
                    //
                    taxAmountValue += rows[i].MPrice * rows[i].MQuantity * (hasTax ? that.getTaxRate(rows[i].MTaxID) : 0);
                    //
                    notaxAmountValue += rows[i].MPrice * rows[i].MQuantity;
                    //
                    totalAmountValue += (taxAmountValue + notaxAmountValue);
                }
                //
                $(notTaxAmount).empty().text(mMath.toMoneyFormat(notaxAmountValue));
                //
                $(taxAmount).empty().text(mMath.toMoneyFormat(taxAmountValue));
                //
                $(totalAmount).empty().text(mMath.toMoneyFormat(totalAmountValue));
            };
            //获取汇率的名称
            this.getTaxRateName = function (taxRateId) {
                //
                return taxRateId ? FPHome.TaxRates.where("x.MItemID == '" + taxRateId + "'")[0].MText : "";
            }
            //获取汇率的值
            this.getTaxRate = function (taxRateId) {
                //
                return taxRateId ? parseFloat(FPHome.TaxRates.where("x.MItemID == '" + taxRateId + "'")[0].MEffectiveTaxRateDecimal) : 0;
            };
            //初始化发票单的基本信息
            this.bindFapiaoBaseInfo = function (data) {
                //开票单号
                $(txtTableNumber).val(data.MTableNumber || $(txtTableNumber).val());
                //联系人
                var contactName = mText.htmlDecode(data.MContactName || "");
                $(txtContact).val(contactName);
                //
                $(txtContact).attr("etitle", contactName).tooltip({ content: contactName })
                //发票号
                $(txtNumber).val(data.MNumber || "");
                //开票日期
                $(txtDate).datebox("setValue", data.MBizDate || new Date());
                //备注
                $(txtExplanation).val(data.MExplanation || "");
                //调整金额
                $(txtAjustAmount).numberbox("setValue", data.txtAjustAmount);
                //
                $(txtDeductionDate).val(FPHome.ParseDate2YearMonth(data.MDeductionDate));
            };
            //获取发票分录信息
            this.getFapiaoEntrys = function () {
                //
                var rows = that.getRowData();
                //
                if (that.editIndex != null) {
                    //
                    $(tableBody).datagrid("endEdit");
                }
                //
                return rows;
            };
            //
            this.resizeTableHeight = function () {

            };
            //
            this.initTableEvent = function () {
                //删除
                $(removeEntryButton).off("click").on("click", function () {
                    //
                    var index = FPHome.GetGridRowIndexByCellItem(tableBody, $(this));
                    //
                    $(tableBody).datagrid("deleteRow", index);
                    //
                    if (index == that.editIndex) {
                        //刚好删除编辑的哪一行
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
                        that.insertEmptyRow(0);
                    }
                    //更新下面的金额
                    that.updateTotalAmount();
                });
                //添加一行
                $(addEntryButton).off("click").on("click", function () {
                    //
                    var index = FPHome.GetGridRowIndexByCellItem(tableBody, $(this));
                    //
                    that.insertEmptyRow(index + 1);

                    //如果是在编辑行上方加一行，那编辑行的序号要加一
                    if (that.editIndex != null && index + 1 <= that.editIndex) {
                        //
                        that.editIndex = that.editIndex + 1;
                    }
                });
            };

            //
            this.getEmptyRowData = function () {
                //
                return $.extend(true, {}, emptyRow);
            };

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
                //至少需要金额单价 数量 三个数
                return rows.where("!!x.MAmount && !!x.MPrice && !!x.MQuantity " + (hasTax ? "&& !!x.MTaxID" : ""))
            };
            //获取编辑的哪一行的金额
            this.getEditRowData = function () {
                //
                if (that.editIndex != null) {
                    //找到各个编辑的框
                    var editors = $(tableBody).datagrid('getEditors', that.editIndex);
                    //获取本行
                    var editRow = {
                        MItemID: $(editors.where("x.field=='MItemID'")[0].target).combobox("getValue"),
                        MItemType: $(editors.where("x.field=='MItemType'")[0].target).val(),
                        MUnit: $(editors.where("x.field=='MUnit'")[0].target).val(),
                        MQuantity: $(editors.where("x.field=='MQuantity'")[0].target).val(),
                        MPrice: $(editors.where("x.field=='MPrice'")[0].target).val(),
                        MAmount: $(editors.where("x.field=='MAmount'")[0].target).val(),
                        MTaxID: !hasTax ? "" : $(editors.where("x.field=='MTaxID'")[0].target).combobox("getValue"),
                        MTax: !hasTax ? 0 : that.getTaxRate($(editors.where("x.field=='MTaxID'")[0].target).combobox("getValue")),
                        MTaxAmount: $(editors.where("x.field=='MTaxAmount'")[0].target).val(),
                    }
                    //
                    return editRow;
                }
                //
                return that.getEmptyRowData();
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
        };
        //
        return FPEditFapiao;
    })()
    //
    w.FPEditFapiao = FPEditFapiao;
})(window)