if (IssueFapiao == undefined) {
    var IssueFapiao = {};
}
var IssueFapiao = {
    single: function (tableId, invoiceIds, invoiceType, isNoTax, closeCallBack) {
        //本位币不是人民币，不让开票
        if ($("#hidCurrencyId").val().toUpperCase() != "CNY") {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "issueFapiaoError4Currency", "只有本位币是人民币才能开发票！"));
            return;
        }
        //无税不需要开票
        if (isNoTax === true || isNoTax && isNoTax.toLowerCase() === "true") {

            var tips = invoiceType == "1" ? HtmlLang.Write(LangModule.IV, "issueFapiaoError41NoTax", "无税的采购单不需要开发票！") :
                HtmlLang.Write(LangModule.IV, "issueFapiaoError4NoTax", "无税的销售单不需要开发票！");

            $.mDialog.alert(tips);
            return;
        }
        $.mDialog.show({
            mTitle: HtmlLang.Write(LangModule.FP, "Edit" + (invoiceType == "1" ? "Purchase" : "Sale") + "Table", "编辑" + (invoiceType == "1" ? "采购" : "销售") + "开票单"),
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle",
            mContent: "iframe:" + '/FP/FPHome/FPEditTable',
            mPostData: { "tableId": tableId || "", "invoiceIds": invoiceIds || "", "invoiceType": invoiceType },
            mCloseCallback: closeCallBack
        });
    },
    //多个开票申请
    multiple: function (invoiceIds, invoiceType, gridId, closeCallBack) {
        gridId = gridId || "#gridInvoice";
        var noTaxRowIds = [];
        var selRows = $(gridId).datagrid('getSelections');
        var selContactIds = [];
        var selTableIds = [];
        var issuedRowIds = [];
        //遍历选中的行，记录选择的联系人ID、无税的单据ID、已开票的单据ID
        $.each(selRows, function (i, row) {
            if (row.MTableID) {
                selTableIds.push(row.MTableID);
            }
            if ($.inArray(row.MContactID, selContactIds) == -1) {
                selContactIds.push(row.MContactID);
            }
            if (row.MTaxID == IVBase.TaxType.No_Tax) {
                noTaxRowIds.push(row.MID);
            }
            if (row.FPTableView && rec.FPTableView.MIssueStatus > 0) {
                issuedRowIds.push(row.MID);
            }
        });
        IssueFapiao.setRowStyleForError(selRows.length, gridId, issuedRowIds);
        //判断是否已开票
        if (issuedRowIds.length > 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "issueFapiaoError4Issued", "存在已开票的单据！"));
            return;
        }
        //判断是否为同一客户
        if (selContactIds.length > 1) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "issueFapiaoError4ContactDiff", "必须是同一个客户才能够合并开发票！"));
            return;
        }
        IssueFapiao.setRowStyleForError(selRows.length, gridId, noTaxRowIds);
        IssueFapiao.single(selTableIds.toString(), invoiceIds, invoiceType, noTaxRowIds.length > 0, closeCallBack);
    },
    //选择了多行记录时，给有错误的行边框设置为红色
    setRowStyleForError: function (selCount, gridId, arrErrorId) {
        if (selCount > 1 && arrErrorId.length > 0) {
            var allRows = $(gridId).datagrid('getRows');
            $.each(allRows, function (i, row) {
                if ($.inArray(row.MID, arrErrorId) != -1) {
                    var errTd = $(gridId).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MID']");
                    errTd.css({ "border": "1px solid red" });
                    errTd.find("input:checkbox").on('change', function () {
                        if (!this.checked) {
                            $(this).closest("td").css({ "border": "" });
                        }
                    });
                }
            });
            $(gridId).parent().find(".datagrid-header-row td[field=MID] input:checkbox").on('click', function () {
                if (!this.checked) {
                    $(gridId).parent().find("tr[class*='datagrid-row'] td[field=MID] input:checkbox").trigger("change");
                }
            });
        }
    },
    //把开票单的编号转化 相应的号码
    GetFullTableNumber: function (number, type) {
        //
        if (!number) {
            //
            return "";
        }
        //
        var prefix = type == IVBase.InvoiceType.Sale || type == IVBase.InvoiceType.InvoiceSaleRed || type === "0" ? "SFP" : "PFP";
        //
        return prefix + ("0000".substring(0, 4 - number.toString().length) + number.toString());
    }
}