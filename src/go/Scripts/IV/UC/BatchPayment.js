var BatchPayment = {
    Selector: "#batchGrid",
    SelectIds: $("#hidIds").val(),
    SelectObj: $("#hidObj").val(),
    IsMergePay: $("#hidIsMergePay").val() == "" ? false : true,
    rundId: $("#hidrunId").val(),
	IsPayMoreThanDue: false,
	IsPayAmountEmpty: false,
    DataSourceEntry: [],
    init: function () {
        BatchPayment.InitSetting();
        BatchPayment.payGrid();
        BatchPayment.clickAction();
    },
    InitSetting: function () {
        //支付账号默认选中第一个
        var arrBank = $("#payBank").combobox("getData");
        if (arrBank != null && arrBank.length > 0) {
            $("#payBank").combobox("select", arrBank[0].id);
        }
    },
    showLocalCurrency: function (selector, totalTaxAmt, localCyId) {
        $(selector).mLocalCyTooltip(totalTaxAmt, localCyId);
        $(selector).tooltip("show")
    },
    payGrid: function () {
        var lblTo = HtmlLang.Write(LangModule.IV, "To", "To");
        var lblNumber = HtmlLang.Write(LangModule.IV, "Number", "Number");
        var lblAmountDue = HtmlLang.Write(LangModule.IV, "AmountDue", "Amount Due");
        var isPayRun = BatchPayment.SelectObj == "PayRun";
        if (isPayRun) {
            lblNumber = HtmlLang.Write(LangModule.PA, "PayMonth", "支付月份");
            lblAmountDue = HtmlLang.Write(LangModule.PA, "RemainingAmount", "待支付工资");
        }
        if (isPayRun || BatchPayment.SelectObj == "Expense") {
            lblTo = HtmlLang.Write(LangModule.Common, "Employee", "Employee");
        }
        else if (BatchPayment.SelectObj == "Invoice_Purchases") {
            lblTo = HtmlLang.Write(LangModule.Common, "supplier", "Supplier");
        }
        else if (BatchPayment.SelectObj == "Invoice_Sales") {
            lblTo = HtmlLang.Write(LangModule.BD, "Customers", "Customers");
        }

        var columns = [{ title: lblTo, field: 'MContactName', width: 100, sortable: true }];
        if (BatchPayment.SelectObj == "Invoice_Sales" || isPayRun) {
            columns.push({ title: lblNumber, field: 'MNumber', width: 80, align: isPayRun ? 'center' : 'left', sortable: true });
        }
        if (!isPayRun) {
            columns.push({ title: HtmlLang.Write(LangModule.IV, "DueDate", "Due Date"), field: 'MDueDate', width: 80, align: 'center', sortable: true, formatter: $.mDate.formatter });
        }
        columns = columns.concat([{
            title: lblAmountDue, field: 'MNoVerifyAmtFor', width: 100, align: "right", sortable: true, formatter: function (value, rec, rowIndex) {
                if (rec.MOrgCyID == rec.MCyID) {
                    return "<span class='amt'>" + Megi.Math.toMoneyFormat(value, 2)+"</span>";
                } else {
                    return "<span class='iv-cy'  onmouseover=\"BatchPayment.showLocalCurrency(this," + rec.MNoVerifyAmt + ",'" + rec.MOrgCyID + "');\">" + rec.MCyID + "</span><span class='amt'>" + Megi.Math.toMoneyFormat(value, 2)+"</span>";
                }
            }
        },
        {
            title: HtmlLang.Write(LangModule.IV, "Amount", "Amount"), field: 'MPayAmount', width: 100, align: "right", sortable: true,
            formatter: function (value) {
                return Megi.Math.toMoneyFormat(value, 2);
            },
            editor: { type: 'numberbox', options: { precision: 2, minPrecision: 2, min: 0, max: 999999999999 } }

        },
            { title: HtmlLang.Write(LangModule.IV, "Reference", "Reference"), field: 'MReference', width: 120, align: 'left', sortable: true, editor: { type: 'text' } },
            {
                title: '', field: 'MEntryID', width: 20, height: "26px", align: 'center', formatter: function (value, rowData, rowIndex) {
                    return "<div class='list-item-action'><a href='javascript:void(0)' class='m-icon-delete-row' onclick='BatchPayment.DeleteItem(this," + rowIndex + ")'>&nbsp;</a></div>";
                }
            }]);
        Megi.grid(BatchPayment.Selector, {
            url: "/IV/UC/GetBatchPaymentList",
            queryParams: { para: { KeyIDs: BatchPayment.SelectIds }, selectObj: BatchPayment.SelectObj, isMergePay: BatchPayment.IsMergePay },
            columns: [columns],
            onClickCell: function (rowIndex, field, value) {
                BatchPayment.endEditGrid();
                if (field == "MContactName" || field == "MNumber" || field == "MDueDate" || field == "MNoVerifyAmtFor" || field == "MEntryID") {
                    return;
                }
                if (BatchPayment.IsMergePay) {
                    return;
                }
                Megi.grid(BatchPayment.Selector, "beginEdit", rowIndex);
                var editor = $(BatchPayment.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) { $(editor.target).focus().select(); }
                Megi.regClickToSelectAllEvt();
            },
            onBeforeEdit: function (rowIndex, rowData){
                BatchPayment.currentEditRowIndex = rowIndex;
            },
            onAfterEdit: function (rowIndex, rowData, changes) {
                BatchPayment.updateDataSourceEntry(rowIndex, changes);
            },
            onLoadSuccess: function (data) {
                //设置表格数据
                BatchPayment.setDataSourceEntry(data.rows);
                //更新表格数据
                BatchPayment.updateDataSourceEntry();
            }
        });
    },
    //更新数据源
    updateDataSourceEntry: function (index, changes) {
        if (index != undefined && changes) {
            for (var i = 0; i < BatchPayment.DataSourceEntry.length; i++) {
                if (BatchPayment.DataSourceEntry[i].RowIndex == index) {
                    if (changes["MReference"] != undefined) {
                        BatchPayment.DataSourceEntry[i].MReference = changes["MReference"];
                    }

                    if (changes["MPayAmount"] != undefined) {
                        var value = Megi.Math.toDecimal(changes["MPayAmount"], 2);
                        var dueAmt = Megi.Math.toDecimal(BatchPayment.getGridValue(index, "MNoVerifyAmtFor"), 2);
                        if (parseFloat(BatchPayment.EmptyToIntZero(value)) > parseFloat(BatchPayment.EmptyToIntZero(dueAmt))) {
                            value = dueAmt;
                            IsPayMoreThanDue = true;
                            $(BatchPayment.Selector).datagrid("updateRow", { index: index, row: { "MPayAmount": value } });
                            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "payAmtCannotLargethanDueAmt", "payment amount cannot large than due amount."));
                        }
                        BatchPayment.DataSourceEntry[i].MPayAmount = value;
                    }
                    break;
                }
            }
        }
        BatchPayment.updateSummaryInfo();
    },
    //更新合计信息
    updateSummaryInfo: function () {
        var amt = 0;
        var arrTax = new Array();
        for (var i = 0; i < BatchPayment.DataSourceEntry.length; i++) {
            var item = BatchPayment.DataSourceEntry[i];
            if (item.MPayAmount != "") {
                amt += parseFloat(BatchPayment.EmptyToIntZero(item.MPayAmount));
            }
        }
        if (amt == 0) { amt = "0.00";}
        //总金额
        $("#spTotal").html(Megi.Math.toMoneyFormat(amt, 2));
    },
    //重新设置数据源
    setDataSourceEntry: function (recordData) {
        var entryDataSource = new Array();
        var rowIndex = 0;
        for (var i = 0; i < recordData.length; i++) {
            var obj = BatchPayment.getOneEntry(recordData[i]);
            obj.RowIndex = rowIndex;
            entryDataSource.push(obj);
            rowIndex += 1;
        }
        BatchPayment.DataSourceEntry = entryDataSource;
    },
    //获取一个空的明细对象
    getEmptyEntry: function () {
        var obj = {};
        obj.MID = "";
        obj.MReference = "";
        obj.MPayAmount = "";
        return obj;
    },
    getOneEntry: function (data) {
        var obj = BatchPayment.getEmptyEntry();
        obj.MID = data.MID;
        obj.MReference = data.MReference;
        obj.MPayAmount = data.MPayAmount;
        return obj;
    },
    DeleteItem: function (btnObj) {
        var rowIndex = $(btnObj).closest(".datagrid-row").attr("datagrid-row-index");
        var result = BatchPayment.deleteDataSourceEntry(rowIndex);
        if (!result) {
            return;
        }
        Megi.grid(BatchPayment.Selector, "deleteRow", rowIndex);
        BatchPayment.endEditGrid();
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
            var tr_id = $(this).attr("id");
            $(this).attr("id", tr_id.substr(0, tr_id.lastIndexOf('-') - 1) + i);
        });
        BatchPayment.updateDataSourceEntry();
    },
    deleteDataSourceEntry: function (rowIndex) {
        if (BatchPayment.DataSourceEntry.length <= 1) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "AtLeastOneLineItem", "You must have at least 1 line item."));
            return false;
        }
        var arr = new Array();
        for (var i = 0; i < BatchPayment.DataSourceEntry.length; i++) {
            if (i != rowIndex) {
                var obj = BatchPayment.DataSourceEntry[i];
                obj.RowIndex = i;
                arr.push(obj);
            }
        }
        BatchPayment.DataSourceEntry = arr;
        return true;
    },
    EmptyToIntZero: function (value) {
        if (value == "") { value = 0; }
        //将金额格式中数字和.之外的字符都去掉
        if (typeof (value) == "string") {
            return value.replace(/[^0-9\.]+/g, "");
        }
        return value;
    },
    getGridValue: function (rowIndex, field) {
        var rowObj = $(BatchPayment.Selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]");
        var value = $(rowObj).find("td[field='" + field + "']").find("div").find(".amt").html();
        return value;
    },
    //结束编辑列表
    endEditGrid: function () {
        if (BatchPayment.currentEditRowIndex) {
            Megi.grid(BatchPayment.Selector, "endEdit", BatchPayment.currentEditRowIndex)
        }
        else {
            var recordLength = Megi.grid(BatchPayment.Selector, "getRows").length;
            for (var i = 0; i < recordLength; i++) {
                Megi.grid(BatchPayment.Selector, "endEdit", i);
            }
        }
    },
    //验证信息
    valideInfo: function () {
        var result = true;
        var isPayRun = BatchPayment.SelectObj == "PayRun";
        for (var i = 0; i < BatchPayment.DataSourceEntry.length; i++) {
            var item = BatchPayment.DataSourceEntry[i];
            if (parseFloat(item.MPayAmount) <= 0) {
                var dueAmt = Megi.Math.toDecimal(BatchPayment.getGridValue(i, "MNoVerifyAmtFor"), 2);
                if (!(isPayRun && parseFloat(dueAmt) <= 0)) {
                    $(BatchPayment.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MPayAmount']").addClass("row-error");
                    result = false;
                }
            }
            if (!isPayRun && $.trim(item.MReference) == '') {
                $(BatchPayment.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MReference']").addClass("row-error");
                result = false;
            }
        }
        return result;
    },
    //获取明细实体列表
    getViewInfo: function () {
        BatchPayment.endEditGrid();
        BatchPayment.updateDataSourceEntry();
        var obj = {};
        var arr = new Array();
        var isPayRun = BatchPayment.SelectObj == "PayRun";
        for (var i = 0; i < BatchPayment.DataSourceEntry.length; i++) {
            var item = BatchPayment.DataSourceEntry[i];
            var dueAmt = Megi.Math.toDecimal(BatchPayment.getGridValue(i, "MNoVerifyAmtFor"), 2);
            var isPayRunEmpty = isPayRun && parseFloat(BatchPayment.EmptyToIntZero(item.MPayAmount)) == parseFloat(BatchPayment.EmptyToIntZero(dueAmt));
			if (item.MPayAmount == "" && !isPayRunEmpty) {
			    BatchPayment.IsPayAmountEmpty = true;
                continue;
            }
            arr.push(item);
        }
        obj.PaymentEntry = arr;
        return obj;
    },
    clickAction: function () {
        $("#aSave").click(function () {
            BatchPayment.batchUpdate();
        });
    },
    batchUpdate: function () {
		IsPayMoreThanDue = false;
		IsPayAmountEmpty = false;
        BatchPayment.endEditGrid();
        var entryResult = BatchPayment.valideInfo();
        var payDateControl = $('#payDate');
        /// add by 锦友20180516 14:13:00
        ///判断收款日期是否在结算期内
        var payDateValid = payDateControl.datebox('isValid');
        if (!entryResult || !payDateValid) { return; }
        var payDate = payDateControl.datebox('getValue');
        var payBank = $('#payBank').combobox('getValue');
        if (payDate == "" || payBank == "") {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "DataAndAccountMustInput", "date and account must input."));
            return;
		}
		var entryInfo = BatchPayment.getViewInfo();

		//付款金额为空报错
		if (BatchPayment.IsPayAmountEmpty) {
			$.mDialog.alert(HtmlLang.Write(LangModule.IV, "AmountCanNotEmpty", "金额不能为空！"));
			return;
		}

        //至少要有一条明细
        if (entryInfo.PaymentEntry == null || entryInfo.PaymentEntry.length < 1) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "AtLeastOneLineItem", "You must have at least 1 line item."));
            return;
        }
        //付款金额大于代核销金额不能保存
        if (IsPayMoreThanDue)
        {
            return;
        }
        var obj = {};
        obj.SelectObj = BatchPayment.SelectObj;
        obj.MPayDate = payDate;
        obj.MPayBank = payBank;
        obj.PaymentEntry = entryInfo.PaymentEntry;
        obj.PayRunID = BatchPayment.rundId;
        obj.SalaryPaymentIDLists = BatchPayment.SelectIds;
        if (BatchPayment.IsMergePay) {
            obj.IsPayRun = true;
        }

        mAjax.submit(
            "/IV/UC/BatchPaymentUpdate",
            { headModel: obj },
            function (data) {
                    var msg = "";
                    switch (BatchPayment.SelectObj) {
                        case "Invoice_Sales":
                            if (!data.Success) {
                                $.mDialog.alert(data.Message);
                            } else {
                                msg = HtmlLang.Write(LangModule.IV, "ReceivePaymentSuccessful", "Receipt created successful!");
                                $.mMsg(msg);
                                parent.InvoiceList.reload(4);
                                $.mDialog.close();
                            }
                            break;
                        case "Invoice_Purchases":
                            if (!data.Success) {
                                $.mDialog.alert(data.Message);
                            } else {
                                msg = HtmlLang.Write(LangModule.IV, "Paymentsuccess", "Payment Successfully!");
                                $.mMsg(msg);
                                parent.BillList.reload(4);
                                $.mDialog.close();
                            }
                            break;
                        case "Expense":
                            if (!data.Success) {
                                $.mDialog.alert(data.Message);
                            }
                            else {
                                msg = HtmlLang.Write(LangModule.IV, "Paymentsuccess", "Payment Successfully!");
                                $.mMsg(msg);
                                parent.ExpenseList.reload(3);
                                $.mDialog.close();
                            }
                            break;
                        case "PayRun":
                            if (!data.Success) {
                                $.mDialog.alert(data.Message);
                            }
                            else {
                                msg = HtmlLang.Write(LangModule.IV, "SuccessfulPayment", "Payment successful!");
                                $.mMsg(msg);
                                if (Megi.getUrlParam("IsFromEdit") === "true") {
                                    parent.SalaryPaymentEdit.reload();
                                }
                                else {
                                    parent.SalaryPaymentList.reload();
                                }
                                $.mDialog.close();
                            }
                            break;
                    }
            });
    }
}

$(document).ready(function () {
    BatchPayment.init();
});