var BusLog = {
    //发票Id
    InvoiceId: $("#hidInvoiceId").val(),
    //初始化页面操作
    init: function () {
        BusLog.bindGrid();
        BusLog.clickAction();
    },
    //绑定列表
    bindGrid: function () {
        var noteHeight = $("#divNotes");
        Megi.grid('#gridBusLog', {
            resizable: true,
            auto: true,
            scrollY: noteHeight.length > 0,
            pagination: true,
            height: BusLog.getGridHeight(),
            url: "/Log/Log/GetBussinessLogList",
            queryParams: { MPKID: BusLog.InvoiceId },
            columns: [[
                //操作类型
                { title: LangKey.Operation, field: 'MAction', width: 120, sortable: false },
                //操作日期
                {
                    title: LangKey.Date, field: 'MCreateDate', width: 120, align: 'center', sortable: false, formatter: function (value, row, index) {
                        return $.mDate.formatDateTime(row.MCreateDate);
                    }
                },
                //操作用户
                { title: LangKey.User, field: 'MUserName', width: 100, sortable: false },
                //操作明细
                {
                    title: LangKey.Details, field: 'MNote', width: 320, sortable: false, formatter: function (value, row, index) {
                        if (!value) {
                            return "";
                        }
                        var logValueText = value.format(row.MValue1, row.MValue2, row.MValue3, row.MValue4, row.MValue5, row.MValue6, row.MValue7, row.MValue8, row.MValue9, row.MValue10);
                        return valueLog = "<span    title='" + mText.encode(logValueText) + "' style='text-overflow: ellipsis;white-space: nowrap;overflow: hidden;width: 320px;'>" + mText.encode(logValueText) + "</span>";

                        //return value.format(row.MValue1, row.MValue2, row.MValue3, row.MValue4, row.MValue5, row.MValue6, row.MValue7, row.MValue8, row.MValue9, row.MValue10);
                    }
                }
            ]]
        });
    },
    getGridHeight: function () {
        var contentH = $(".m-imain").outerHeight();
        var noteH = $("#divNotes").outerHeight();
        return contentH - noteH - 15;
    },
    //初始化按钮操作
    clickAction: function () {
        //保存
        $("#aSave").click(function () {
            //保存的地址
            var save_url = "";
            //获取发票类型
            var billType = $.trim($("#hidBillType").val());
            if (billType == "Invoice_Sale" || billType == "Invoice_Sale_Red") {
                save_url = "/IV/Invoice/AddSaleNoteLog";
            }
            else if (billType == "Invoice_Purchase" || billType == "Invoice_Purchase_Red") {
                save_url = "/IV/Bill/AddBillNoteLog";
            }
            else if (billType == "Expense_Claims") {
                save_url = "/IV/Expense/AddExpenseNoteLog";
            } else if (billType == "Sale_FaPiao") {
                save_url = "/FP/FPHome/FPAddLog";
                billType = 0;
            } else if (billType == "Sale_FaPiao_Table") {
                save_url = "/FP/FPHome/FPAddLog";
                billType = 1;
            } else if (billType == "GL_Voucher") {
                save_url = "/FP/FPHome/FPAddLog";
                billType = 2;
            }
            //备注
            var txtNote = $("#txtNote");
            var note = $.trim(txtNote.val());
            if (note == "") {
                txtNote.focus();
                return;
            }
            //提交
            $("#divNotes").mFormSubmit({
                url: save_url,
                param: { model: { MID: BusLog.InvoiceId, MDesc: note, MType: billType } },
                callback: function (msg) {
                    if (msg.Success == true) {
                        //提示信息
                        $.mMsg(LangKey.SaveSuccessfully);
                        //保存成功，刷新列表
                        BusLog.bindGrid(BusLog.CurrentType);
                        //清空备注
                        txtNote.val("");
                    } else {
                        //保存失败，提示错误
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
        });
    }
}

//页面加载
$(document).ready(function () {
    BusLog.init();
});