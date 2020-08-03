var VerificationEdit = {
    BillID: "",
    TotalAmount: 0,
    CurrencyID: "",
    ContactID: "",
    BizType: "",
    BizBillType: "",
    SrcBizType: "",
    SrcBizBillType: "",
    init: function () {
        VerificationEdit.CurrencyID = $("#hidMCurrencyID").val();
        VerificationEdit.BillID = $("#hidMBillID").val();
        VerificationEdit.BizType = $("#hidMBizType").val();
        VerificationEdit.BizBillType = $("#hidMBizBillType").val();
        VerificationEdit.SrcBizType = $("#hidSrcMBizType").val();
        VerificationEdit.SrcBizBillType = $("#hidSrcMBizBillType").val();
        VerificationEdit.ContactID = $("#hidMContactID").val();
        VerificationEdit.TotalAmount = Number($("#hidAmount").val());
        VerificationEdit.CalTotal();
        VerificationEdit.initGrid(true);
        $("#aSeach").click(function () {
            VerificationEdit.initGrid(false);
        });
        $("#btnSave").click(function () {
            VerificationEdit.CreditInvoice();
        });
    },
    initGrid: function (isInit) {
        var keyword = "";
        var amount = "";
        if (!isInit) {
            keyword = $("#txtKeyword").val();
            amount = $("#txtAmount").val();
        }
        Megi.grid("#myGrid", {
            resizable: true,
            auto: true,
            scrollY: true,
            fitColumns: true,
            height: 260,
            url: "/IV/Verification/GetVerificationList",
            queryParams: {
                MBizBillType: VerificationEdit.SrcBizBillType,
                MBillID: VerificationEdit.BillID,
                MBizType: VerificationEdit.BizType,
                MContactID: VerificationEdit.ContactID,
                MCurrencyID: VerificationEdit.CurrencyID,
                MKeyword: keyword,
                MAmount: amount
            },
            columns: [[
            {
                title: HtmlLang.Write(LangModule.Bank, "No_number", "No#"), field: 'MBillNo', width: 100, formatter: function (value, rec) {
                    var html = '';
                    if (rec.MBillNo != null && rec.MBillNo != "") {
                        html += "<div class='mg-bold'>" + rec.MBillNo + "</div>";
                    }
                    if (VerificationEdit.SrcBizBillType != "PayRun" && rec.MBizDate != null && rec.MBizDate != "") {
                        html += "<div>" + $.mDate.format(rec.MBizDate) + "</div>";
                    }
                    if (rec.MContactName != null && rec.MContactName != "") {
                        html += "<div>" + rec.MContactName + "</div>";
                    }
                    return html;
                }
            },
            {
                title: HtmlLang.Write(LangModule.Bank, "TransactionType", "Transaction Type"), field: 'MBizBillType', width: 100, align: 'center', formatter: function (value, row, index) {
                    if (value == "Payment" || value == "Receive") {
                        return eval("LangKey." + value);
                    }
                    return eval("LangKey." + row.MBizType);
                }
            },
            { title: LangKey.Reference, field: 'MReference', width: 80, align: 'left' },
            { title: HtmlLang.Write(LangModule.Bank, "Currency", "Currency"), field: 'MCurrencyName', width: 80, align: 'center' },
            {
                title: HtmlLang.Write(LangModule.Bank, "Total", "Total"), field: 'MAmountTotalFor', width: 60, align: 'right', formatter: function (value, rec) {
                    return Megi.Math.toDecimal(Math.abs(value));
                }
            },
            {
                title: HtmlLang.Write(LangModule.Bank, "ReconciledAmount", "Reconciled Amount"), field: 'MHaveVerificationAmtFor', width: 120, align: 'right', formatter: function (value, rec) {
                    return Megi.Math.toDecimal(Math.abs(value));
                }
            },
            {
                title: HtmlLang.Write(LangModule.Bank, "OutstandingAmount", "Outstanding Amount"), field: 'MNoVerificationAmtFor', width: 120, align: 'right', formatter: function (value, rec) {
                    var keyName = "MBillID,MBizType,MBizBillType";
                    var keyValue = mEasyUI.mGetKeyValueByKeyName(rec, keyName);
                    return "<span class='No-Verification-Amt-For' srcValue='" + Megi.Math.toDecimal(Math.abs(value)) + "'  keyname='" + keyName + "' keyvalue = '" + keyValue + "'>" + Megi.Math.toDecimal(Math.abs(value)) + "</span>";
                }
            },
            {
                title: HtmlLang.Write(LangModule.Bank, "AmountToBeReconciled", "Amount to be alloctated"), field: 'MVerificationAmt', width: 150,
                align: 'right',
                formatter: function (value, rec) {
                    return '<input type="text" class="datagrid-editable-input" style="min-width:135px;width:90%; height: 28px; line-height: 25px;"/>';
                }
                //editor: {
                //    type: 'numberbox', options: {
                //        precision: 2, min: 0, max: 9999999
                //    }
                //}
            }
            ]],
            onLoadSuccess: function () {
                var data = Megi.grid("#myGrid", "getRows");

                $(".datagrid-editable-input").css("text-align", "right");

                $(".datagrid-editable-input").keyup(function () {
                    var value = $(this).val();
                    $(this).val(value.replace("-",""));
                    var result = VerificationEdit.SetRowVerificationAmt(this);
                    VerificationEdit.CalTotal();
                });

                $(".datagrid-editable-input").blur(function () {
                    var value = $(this).val();
                    $(this).val(value.replace("-", ""));
                    var result = VerificationEdit.SetRowVerificationAmt(this);
                    if (!result) {
                        var tips = HtmlLang.Write(LangModule.IV, "AmountReconciledCanNotGreaterThanOutstandingAmount", "Amount to be reconciled can't be greater than outstanding amount!")
                        Megi.warning(tips);
                    }
                    VerificationEdit.CalTotal();
                });

                $(".datagrid-editable-input").numberbox({ min: 0, precision: 2 });

                VerificationEdit.setDefaultValue(data);
                Megi.regClickToSelectAllEvt();
                VerificationEdit.CalTotal();
            }
        });
    },
    setDefaultValue: function (rows) {
        if (rows == null || rows.length == 0 || rows.length > 1) {
            return;
        }
        var toBeVerificationAmt = $("#hidToBeVerificationAmount").val();
        toBeVerificationAmt = toBeVerificationAmt == "" ? 0 : Number(toBeVerificationAmt);
        var noVerificationAmtFor = rows[0].MNoVerificationAmtFor;
        if (toBeVerificationAmt == noVerificationAmtFor) {
            $(".datagrid-editable-input").numberbox("setValue", noVerificationAmtFor);
            VerificationEdit.SetRowVerificationAmt(".datagrid-editable-input");
        }
        //$(".check-key").trigger('click');
        Megi.grid("#myGrid", "selectRow", 0)
        VerificationEdit.CalTotal();
    },
    CalTotal: function (sender) {
        var srcAmt = $("#hidToBeVerificationAmount").val();
        var toBeReconciledTotal = VerificationEdit.GetToBeReconciledTotal();
        var outstandingAmount = parseFloat(srcAmt) - toBeReconciledTotal;
        $("#spToBeReconciledTotal").html(Megi.Math.toDecimal(toBeReconciledTotal) + "&nbsp;" + VerificationEdit.CurrencyID);
        $("#spOutstandingAmount").html(Megi.Math.toDecimal(outstandingAmount) + "&nbsp;" + VerificationEdit.CurrencyID);
        $("#spWaitingForReconciled").html(Megi.Math.toDecimal(srcAmt) + "&nbsp;" + VerificationEdit.CurrencyID);
    },
    GetToBeReconciledTotal: function () {
        var total = 0;
        $(".datagrid-editable-input").each(function () {
            var itemValue = $(this).val();
            itemValue = itemValue.length == 0 ? 0 : parseFloat(itemValue);
            total += itemValue;
        });
        return Megi.Math.toDecimal(total);
    },
    SetRowVerificationAmt: function (txtObj) {
        var value = $(txtObj).val();

        var noVerificationAmtForObj = $(txtObj).closest(".datagrid-row").find("td[field='MNoVerificationAmtFor']").find(".No-Verification-Amt-For");
        var srcNoVerificationAmtFor = Number($(noVerificationAmtForObj).attr("srcValue"));
        var toRecAmount = value == "" ? 0 : Number(value);
        if (srcNoVerificationAmtFor - toRecAmount < 0) {
            $(noVerificationAmtForObj).html(Megi.Math.toDecimal(srcNoVerificationAmtFor));
            $(txtObj).numberbox("setValue", "");
            return false;
        } else {
            $(noVerificationAmtForObj).html(Megi.Math.toDecimal(srcNoVerificationAmtFor - toRecAmount));
            return true;
        }
    },
    CreditInvoice: function () {
        var arr = new Array();
        $(".No-Verification-Amt-For").each(function () {
            var obj = {};
            var amtInput = $(this).closest("tr").find(".datagrid-editable-input");
            if (amtInput.length > 0) {
                var amt = amtInput.val();
                amt = amt.length == 0 ? 0 : Number(amt);
                if (amt > 0) {
                    var tgtData = mEasyUI.mGetDataGridSingleRowData("#myGrid", this);

                    obj.MSourceBillID = VerificationEdit.BillID;
                    obj.MSourceBillType = VerificationEdit.SrcBizBillType;
                    obj.MSourceBizType = VerificationEdit.SrcBizType;
                    obj.MTargetBillID = tgtData.MBillID;
                    obj.MTargetBillType = tgtData.MBizBillType;
                    obj.MTargetBizType = tgtData.MBizType;
                    obj.MAmount = amt;
                    arr.push(obj);
                }
            }

        });
        if (arr.length == 0) {

            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "NoItemsSelected", "No item selected."));
            return;
        }
        var total = VerificationEdit.GetToBeReconciledTotal();
        if (total <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "TotalMustGreaterThanZero", "Amount to be reconciled total must greater than zero."));
            return;
        }
        var toBeVerificationAmount = Number($("#hidToBeVerificationAmount").val());
        if (toBeVerificationAmount < total) {
            var message = HtmlLang.Write(LangModule.IV, "ReconciledTotalMustBeEqualOrLesserThan", "Amount to be reconciled total must be equal or lesser than ") + Megi.Math.toDecimal(toBeVerificationAmount) + $("#hidMCurrencyID").val() + ".";

            $.mDialog.alert(message)
            return;
        }
        mAjax.submit(
            "/IV/Verification/UpdateVerificationList",
            { list: arr },
            function (msg) {
                if (msg.Success == false) {
                    $.mAlert(msg.Message, {
                        callback: function () {
                            mWindow.reload();
                        }
                    });
                } else {
                    $.mDialog.callback();
                    $.mMsg(HtmlLang.Write(LangModule.IV, "ReconcileSuccessfully", "Reconcile successfully!"));
                    if (VerificationEdit.SrcBizBillType == "PayRun") {
                        parent.SalaryPaymentEdit.reload();
                    }
                    else {
                        parent.mWindow.reload(Megi.request("sv", "0", parent.window.location.href));
                    }
                    $.mDialog.close();
                }
            });
    }
}

$(document).ready(function () {
    VerificationEdit.init();
});