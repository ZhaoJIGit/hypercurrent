
var IVBase = {
    url_getlist: "/IV/IVInvoiceBase/GetInvoiceList",
    url_delete: "/IV/IVInvoiceBase/DeleteInvoiceList",
    url_updateStatus: "/IV/IVInvoiceBase/UpdateInvoiceStatus",
    url_approve: "/IV/IVInvoiceBase/ApproveInvoice",
    url_unApprove: "/IV/IVInvoiceBase/UnApproveInvoice",
    url_repeat_invoice_updateStatus: "/IV/Invoice/UpdateRepeatIVStatus",
    url_repeat_bill_updateStatus: "/IV/Bill/UpdateRepeatBillStatus",
    url_repeat_invoice_delete: "/IV/Invoice/DeleteRepeatIVList",
    url_repeat_bill_delete: "/IV/Bill/DeleteRepeatBillList",
    url_getModel: "/IV/IVInvoiceBase/GetInvoiceEditModel",
    url_update: "/IV/IVInvoiceBase/UpdateInvoice",
    url_Edit: "/IV/Invoice/InvoiceEdit",
    url_View: "/IV/Invoice/InvoiceView",
    url_CreditNoteEdit: "/IV/Invoice/CreditNoteEdit",
    url_CreditNoteView: "/IV/Invoice/CreditNoteView",
    url_BillEdit: "/IV/Bill/BillEdit",
    url_BillCreditNoteEdit: "/IV/Bill/CreditNoteEdit",
    url_BillView: "/IV/Bill/BillView",
    url_BillCreditNoteView: "/IV/Bill/CreditNoteView",
    url_InvoiceList: "/IV/Invoice/InvoiceList",
    url_BillList: "/IV/Bill/BillList",
    HasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    BillType: $("#hidType").val(),
    IsClick: true,
    IsClickEdit: false,
    IsClickDelete: false,
    enableNumberLink: false,
    columns: [
        //复选框
        {
            title: '<input type=\"checkbox\" >', field: 'MID', width: 25, align: 'center', formatter: function (value, rec, rowIndex) {
                return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" /><input name='type' type='hidden' value='" + value + "," + rec.MNumber + "," + rec.MType + "' />";
            }
        },
        //发票号
        {
            title: HtmlLang.Write(LangModule.IV, "Number", "Number"), field: 'MNumber', width: 80, align: 'left', sortable: true, formatter: function (value, rec, rowIndex) {
                if (IVBase.enableNumberLink) {
                    return "<a href=\"javascript:void(0);\" onclick=\"IVBase.IsClickEdit=true;IVBase.editItem(" + rec.MStatus + ", '" + rec.MType + "', '" + rec.MID + "','');" + "\" >" + value + "</a>";
                }
                else {
                    if (value) {
                        //判断是否销售单 
                        if (rec.MType == IVBase.InvoiceType.InvoiceSaleRed) {
                            return '<span class="icon-credit-note">&nbsp;</span>' + value;
                        }
                        else {
                            return '<span class="icon-invoice-add">&nbsp;</span>' + value;
                        }

                    } else {
                        return "";
                    }
                }
            }
        },
        //客户
        {
            title: HtmlLang.Write(LangModule.IV, "To", "To"), field: 'MContactName', width: 100, sortable: true, formatter: function (value, rec, rowIndex) {

                if (rec.MType == IVBase.InvoiceType.Purchase) {
                    return '<span class="icon-invoice-add">&nbsp;</span>' + value;
                }
                else if (rec.MType == IVBase.InvoiceType.InvoicePurchaseRed) {
                    return '<span class="icon-credit-note">&nbsp;</span>' + value;
                }
                else {
                    return value;
                }

            }
        },
        //摘要
        {
            title: HtmlLang.Write(LangModule.IV, "Ref", "Ref"), field: 'MReference', width: 120, align: 'left', sortable: true, formatter: function (value, rec, rowIndex) {
                if (value) {
                    return value;
                } else {
                    return "";
                }
            }
        },
        //业务日期
        { title: HtmlLang.Write(LangModule.IV, "Date", "Date"), field: 'MBizDate', width: 80, align: 'center', sortable: true, formatter: $.mDate.formatter },
        //到期日期
        {
            title: HtmlLang.Write(LangModule.IV, "DueDate", "Due Date"), field: 'MDueDate', width: 80, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                var dueDate = $.mDate.format(value);
                if (dueDate == "") {
                    return "";
                }
                var now = new Date();
                var tempDate = new Date(dueDate);
                tempDate.setDate(tempDate.getDate() + 1);
                if (tempDate < now) {
                    return "<span class='red'>" + dueDate + "</span>";
                }
                return dueDate;
            }
        },
        //预计付款日期
        {
            title: HtmlLang.Write(LangModule.IV, "ExpectedDate", "Expected Date"), field: 'MExpectedDate', width: 80, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                var expDate = $.mDate.format(value);
                return expDate;
                //if (expDate == "") {
                //    return "<a class=\"\" style=\"display:block;cursor:pointer; width:100%; height:20px\" onmouseover='IVBase.showExpectedDateTip(this)' onmouseout='IVBase.hideExpectedDateTip(this)' onclick='IVBase.editExpectedDate(this,\"" + rec.MID + "\",\"" + rec.MContactID + "\",\""+rec.MType+"\")'>&nbsp;</a>";
                //}
                //var now = new Date();
                //var tempDate = new Date(expDate);
                //tempDate.setDate(tempDate.getDate() + 1);
                //var cls = "";
                //if (tempDate < now) {
                //    cls = "red";
                //}
                //return "<a href='javascript:void(0)' class='"+cls+"' onclick='IVBase.editExpectedDate(this,\""+rec.MID+"\",\""+rec.MContactID+"\")'>"+ expDate +"</a>";
            }
        },
        //已核销金额
        {
            title: HtmlLang.Write(LangModule.IV, "Paid", "Paid"), field: 'MVerifyAmtFor', width: 100, align: "right", sortable: true, formatter: function (value, rec, rowIndex) {
                value = Math.abs(value);
                if (rec.MOrgCyID == rec.MCyID) {
                    return Megi.Math.toMoneyFormat(value, 2);
                } else {
                    return "<span class='iv-cy'  onmouseover=\"IVBase.showLocalCurrency(this," + rec.MVerifyAmt + ",'" + rec.MOrgCyID + "');\">" + rec.MCyID + "</span>" + Megi.Math.toMoneyFormat(value, 2);
                }
            }
        },
        //发票金额（含税、原币）
        {
            title: HtmlLang.Write(LangModule.IV, "owing", "owing"), field: 'MTaxTotalAmtFor', width: 100, align: "right", sortable: true, formatter: function (value, rec, rowIndex) {

                var result = Math.abs(value) - Math.abs(rec.MVerifyAmtFor);

                var noVerifyAmt = Math.abs(rec.MTaxTotalAmt) - Math.abs(rec.MVerifyAmt);
                if (result == 0) {
                    noVerifyAmt = 0;
                }

                if (rec.MOrgCyID == rec.MCyID) {
                    return Megi.Math.toMoneyFormat(result, 2);
                } else {
                    return "<span class='iv-cy' onmouseover=\"IVBase.showLocalCurrency(this," + noVerifyAmt + ",'" + rec.MOrgCyID + "');\">" + rec.MCyID + "</span>" + Megi.Math.toMoneyFormat(result, 2);
                }
            }
        },
        //状态
        {
            title: HtmlLang.Write(LangModule.IV, "Status", "Status"), field: 'MStatus', width: 120, fixed: true, sortable: true, formatter: function (value, rec, rowIndex) {
                var status = Number(value);
                var isSale = rec.MType == "Invoice_Sale";//"Invoice_Sale_Red"
                var result = "";
                switch (status) {
                    case IVBase.Status.Draft:
                        result = HtmlLang.Write(LangModule.IV, "Draft", "Draft");
                        break;
                    case IVBase.Status.WaitingApproval:
                        result = HtmlLang.Write(LangModule.IV, "AwaitingApproval", "Awaiting Approval");
                        break;
                    case IVBase.Status.AwaitingPayment:
                        result = isSale ? HtmlLang.Write(LangModule.IV, "AwaitingReceive", "Awaiting Receive") : HtmlLang.Write(LangModule.IV, "AwaitingPayment", "Awaiting Payment");
                        break;
                    case IVBase.Status.Paid:
                        result = isSale ? HtmlLang.Write(LangModule.IV, "Received", "Received") : HtmlLang.Write(LangModule.IV, "Paid", "Paid");
                        break;
                }

                return result;
            }
        },
        //是否已发送过电子邮件
        {
            title: HtmlLang.Write(LangModule.IV, "Sent", "Sent"), field: 'MIsSent', width: 50, fixed: true, sortable: true, align: 'center', formatter: function (value, rec, rowIndex) {
                return value == true ? "<span class=\"icon-flag-done\"></span>" : "";
            }
        },
        //附件
        {
            title: HtmlLang.Write(LangModule.Common, "Attachment", "Attachment"), field: 'Attach', align: 'center', width: 50, sortable: false, formatter: function (value, rec, rowIndex) {
                var curAttachId = '';
                var attachCount = '';
                var attachIconClass = '';
                if (rec.MAttachIDs != null && rec.MAttachIDs != '') {
                    var arrAttachId = rec.MAttachIDs.split(',');
                    curAttachId = arrAttachId[0];
                    attachCount = arrAttachId.length;
                    attachIconClass = "m-list-attachment";
                }
                return "<a href='javascript:void(0);' onclick=\"IVBase.viewFileInfo('" + curAttachId + "', '" + rec.MAttachIDs + "', '');\" class='" + attachIconClass + "'><span>" + attachCount + "</span></a>";
            }
        },
        //编辑 和 删除
        {
            title: HtmlLang.Write(LangModule.IV, "Operation", "Operation"), field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                //将 双引号 和 单引号 替换一下，避免在方法中作为参数传递出现错误
                if (rec.MReference) {
                    rec.MReference = rec.MReference.replace(/"/g, "").replace(/'/g, "");
                }
                //验证是否有权限进行 编辑 和 删除（这个验证只是为了方便用户操作，其实后台服务器做了双重权限验证）
                if (IVBase.HasChangeAuth) {
                    //1.有权限，则显示 编辑 和 删除
                    //2.为了和批量删除按钮的显示隐藏保持一致，这里需要判断一下当前列表行是否需要显示删除操作，列表编号为 2 和 3（Draft，Awaiting Approval）的需要显示删除操作
                    var gct = IVBase.getCurrentType();

                    //bankid只在初始化收付款单时有用
                    var bankId = rec.MBankID ? rec.MBankID : "";

                    if (gct == 2 || gct == 3) {
                        return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick='IVBase.IsClickEdit=true;IVBase.editItem(" + rec.MStatus + ", \"" + rec.MType + "\", \"" + rec.MID + "\",\"" + bankId + "\");' class='list-item-edit'>&nbsp;</a><a href=\"javascript:void(0);\" onclick=\"IVBase.IsClickDelete=true;IVBase.deleteItem('" + rec.MID + "');\" class='list-item-del'>&nbsp;</a></div>";
                    } else {
                        //其他列表行只显示编辑操作
                        return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick='IVBase.IsClickEdit=true;IVBase.editItem(" + rec.MStatus + ", \"" + rec.MType + "\", \"" + rec.MID + "\",\"" + bankId + "\");' class='list-item-edit'>&nbsp;</a></div>";
                    }
                } else {
                    //没权限，则显示 编辑，因为编辑页面会对该操作进行权限验证，如果没有权限，则显示为只读
                    return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick='IVBase.IsClickEdit=true;IVBase.editItem(" + rec.MStatus + ", \"" + rec.MType + "\", \"" + rec.MID + "\",\"" + bankId + "\");' class='list-item-edit'>&nbsp;</a></div>";
                }
            }
        },
        //银行ID隐藏列，在初始化收付款单使用
        {
            title: 'MBankID', field: 'MBankID', align: 'center', width: 60, sortable: false, hidden: 'true'
        },
        //关联开票单
        {
            title: HtmlLang.Write(LangModule.FP, "FPTable", "开票单"), field: 'MTableNumber', width: 110, fixed: true, sortable: true, align: 'center', hidden: ($("#hidViewFapiaoAuth").val() == "1" ? false : true), formatter: function (value, rec, rowIndex) {
                if (rec.MTableID) {
                    return IssueFapiao.GetFullTableNumber(value, rec.MType);
                }
                else {
                    return "";
                }
            }
        },
        //开票状态
        {
            title: ($("#hidInvoiceType").val() == "0" ? HtmlLang.Write(LangModule.FP, "IssueStatus", "开票状态") : HtmlLang.Write(LangModule.FP, "CollectStatus", "收集状态")), field: 'MIssueStatus', width: 110, fixed: true, sortable: true, align: 'center', hidden: ($("#hidViewFapiaoAuth").val() == "1" ? false : true), formatter: function (value, rec, rowIndex) {
                var result = '';
                var isSale = rec.MType == "Invoice_Sale" || rec.MType == "Invoice_Sale_Red";
                var lblPartlyIssued = isSale ? HtmlLang.Write(LangModule.FP, "partlyIssued", "部分开票") : HtmlLang.Write(LangModule.FP, "partlyCollected", "部分收集");
                var lblIssued = isSale ? HtmlLang.Write(LangModule.FP, "Issued", "完全开票") : HtmlLang.Write(LangModule.FP, "Collected", "完全收集");
                var lblWaitingIssue = isSale ? HtmlLang.Write(LangModule.FP, "notIssued", "等待开票") : HtmlLang.Write(LangModule.FP, "notCollected", "等待收集");

                switch (rec.MIssueStatus) {
                    case IVBase.IssueStatus.PartlyIssued:
                        result = lblPartlyIssued;
                        break;
                    case IVBase.IssueStatus.Issued:
                        result = lblIssued;
                        break;
                    default:
                        result = lblWaitingIssue;
                        break;
                }
                return result;
            }
        }
    ],
    //重复发票列数组
    repeatInvoiceColumns: [
        //复选框
        {
            title: '<input type=\"checkbox\" >', field: 'MID', width: 25, align: 'center', formatter: function (value, rec, rowIndex) {
                return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" /><input name='type' type='hidden' value='" + value + "," + rec.MNumber + "," + rec.MType + "' />";
            }
        },
        //客户
        {
            title: HtmlLang.Write(LangModule.IV, "Name", "Name"), field: 'MContactName', width: 100, sortable: true, formatter: function (value, rec, rowIndex) {
                if (value) {
                    return value;
                } else {
                    return "";
                }
            }
        },
        //摘要
        {
            title: HtmlLang.Write(LangModule.IV, "Reference", "Reference"), field: 'MReference', width: 120, align: 'left', sortable: true, formatter: function (value, rec, rowIndex) {
                if (value) {
                    return value;
                } else {
                    return "";
                }
            }
        },
        //发票金额（含税、原币）
        {
            title: HtmlLang.Write(LangModule.IV, "Amount", "Amount"), field: 'MTaxTotalAmtFor', width: 100, align: "right", sortable: true, formatter: function (value, rec, rowIndex) {
                var result = Math.abs(value);
                if (rec.MOrgCyID == rec.MCyID) {
                    return Megi.Math.toMoneyFormat(result, 2);
                } else {
                    return "<span class='iv-cy'>" + rec.MCyID + "</span>" + Megi.Math.toMoneyFormat(result, 2);
                }
            }
        },
        //重复周期
        {
            title: HtmlLang.Write(LangModule.IV, "Repeats", "Repeats"), field: 'MRepeats', width: 80, align: 'left', sortable: false, formatter: function (value, rec, rowIndex) {
                return HtmlLang.Write(LangModule.IV, "Every", "Every") + " " + rec.MRepeatNumber + " " + HtmlLang.Write(LangModule.IV, rec.MRepeatType, rec.MRepeatType);
            }
        },
        //业务日期（销售发票）
        { title: HtmlLang.Write(LangModule.IV, "NextInvoiceDate", "Next Invoice Date"), field: 'MBizDate', width: 80, align: 'center', sortable: true, formatter: $.mDate.formatter },
        //业务日期（采购发票）
        { title: HtmlLang.Write(LangModule.IV, "NextBillDate", "Next Bill Date"), field: 'MBizDate', width: 80, align: 'center', sortable: true, formatter: $.mDate.formatter },
        //结束日期
        {
            title: HtmlLang.Write(LangModule.IV, "EndDate", "End Date"), field: 'MEndDate', width: 80, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                var endDate = $.mDate.format(value);
                if (endDate == "") {
                    return "";
                }
                var now = new Date();
                var tempDate = new Date(endDate);
                tempDate.setDate(tempDate.getDate() + 1);
                if (tempDate < now) {
                    return "<span class='red'>" + endDate + "</span>";
                }
                return endDate;
            }
        },
        //状态（销售发票）
        {
            title: HtmlLang.Write(LangModule.IV, "InvoiceWillBe", "Invoice Will Be"), field: 'MStatus', width: 120, fixed: true, sortable: true, formatter: function (value, rec, rowIndex) {
                var status = Number(value);
                switch (status) {
                    case IVBase.Status.Draft:
                        return HtmlLang.Write(LangModule.IV, "SaveAsDraft", "Save as Draft");
                    case IVBase.Status.WaitingApproval:
                        return HtmlLang.Write(LangModule.IV, "Approve", "Approve");
                    case IVBase.Status.AwaitingPayment:
                        return HtmlLang.Write(LangModule.IV, "ApproveForSending", "Approve for Sending");
                }
            }
        },
        //状态（采购发票）
        {
            title: HtmlLang.Write(LangModule.IV, "BillWillBe", "Bill Will Be"), field: 'MStatus', width: 120, fixed: true, sortable: true, formatter: function (value, rec, rowIndex) {
                var status = Number(value);
                switch (status) {
                    case IVBase.Status.Draft:
                        return HtmlLang.Write(LangModule.IV, "SaveAsDraft", "Save as Draft");
                    case IVBase.Status.WaitingApproval:
                        return HtmlLang.Write(LangModule.IV, "Approve", "Approve");
                    case IVBase.Status.AwaitingPayment:
                        return HtmlLang.Write(LangModule.IV, "ApproveForSending", "Approve for Sending");
                }
            }
        },
        //附件
        {
            title: HtmlLang.Write(LangModule.Common, "Attachment", "Attachment"), field: 'Attach', align: 'center', width: 50, sortable: false, formatter: function (value, rec, rowIndex) {
                var hasAttach = rec.MAttachIDs != null && rec.MAttachIDs != '';
                var curAttachId = hasAttach ? rec.MAttachIDs.split(',')[0] : '';
                var attachCount = hasAttach ? rec.MAttachIDs.split(',').length : '';
                var attachIconClass = hasAttach ? "m-list-attachment" : "";
                return "<a href='javascript:void(0);' onclick=\"IVBase.viewFileInfo('" + curAttachId + "', '" + rec.MAttachIDs + "', '');\" class='" + attachIconClass + "'><span>" + attachCount + "</span></a>";
            }
        },
        //编辑 和 删除
        {
            title: HtmlLang.Write(LangModule.IV, "Operation", "Operation"), field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                //将 双引号 和 单引号 替换一下，避免在方法中作为参数传递出现错误
                if (rec.MReference) {
                    rec.MReference = rec.MReference.replace(/"/g, "").replace(/'/g, "");
                }
                //验证是否有权限进行 编辑 和 删除（这个验证只是为了方便用户操作，其实后台服务器做了双重权限验证）
                if (IVBase.HasChangeAuth) {
                    return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"IVBase.IsClickEdit=true;IVBase.editRepeatInvoice('" + rec.MID + "','" + rec.MReference + "');\" class='list-item-edit'>&nbsp;</a><a href=\"javascript:void(0);\" onclick=\"IVBase.IsClickDelete=true;IVBase.deleteRepeatInvoice('" + rec.MID + "','" + rec.MType + "');\" class='list-item-del'>&nbsp;</a></div>";
                } else {

                    //没权限，则显示 编辑，因为编辑页面会对该操作进行权限验证，如果没有权限，则显示为只读
                    return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"IVBase.IsClickEdit=true;IVBase.editRepeatInvoice('" + rec.MID + "','" + rec.MReference + "');\" class='list-item-edit'>&nbsp;</a></div>";
                }
            }
        },
        //编辑 和 删除
        {
            title: HtmlLang.Write(LangModule.IV, "Operation", "Operation"), field: 'Action2', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                return "<div class='list-item-action'>&nbsp</div>";
            }
        }
    ],
    Status: {
        Draft: 1,
        WaitingApproval: 2,
        AwaitingPayment: 3,
        Paid: 4
    },
    IssueStatus: {
        NotIssued: 0,
        PartlyIssued: 1,
        Issued: 2
    },
    TaxType: {
        No_Tax: "No_Tax",
        Tax_Exclusive: "Tax_Exclusive",
        Tax_Inclusive: "Tax_Inclusive"
    },
    StatusPlusOne: function (status) {
        return parseInt(status) + 1;
    },
    InvoiceType: {
        //销售
        Sale: "Invoice_Sale",
        //采购
        Purchase: "Invoice_Purchase",
        InvoiceSaleRed: "Invoice_Sale_Red",
        InvoicePurchaseRed: "Invoice_Purchase_Red",


        PayPurchase: "Pay_Purchase",
        PayPurReturn: "Pay_PurReturn",
        PayOther: "Pay_Other",
        PayOtherReturn: "Pay_OtherReturn",
        ReceiveSale: "Receive_Sale",
        ReceiveOther: "Receive_Other",
        ReceiveOtherReturn: "Receive_OtherReturn"
    },
    showLocalCurrency: function (selector, totalTaxAmt, localCyId) {
        $(selector).mLocalCyTooltip(totalTaxAmt, localCyId);
        $(selector).tooltip("show")
    },
    GetStatus: function (statusId) {
        switch (statusId) {
            case IVBase.Status.Draft:
                return HtmlLang.Write(LangModule.IV, "Draft", "Draft");
            case IVBase.Status.WaitingApproval:
                return HtmlLang.Write(LangModule.IV, "AwaitingApproval", "Awaiting Approval");
            case IVBase.Status.AwaitingPayment:
                return HtmlLang.Write(LangModule.IV, "AwaitingPayment", "Awaiting Payment");
            case IVBase.Status.Paid:
                return HtmlLang.Write(LangModule.IV, "Paid", "Paid");
            default:
                return HtmlLang.Write(LangModule.IV, "Draft", "Draft");
        }
    },
    bindEvent: null,
    editExpectedDate: function (obj, id, contactId, mtype) {
        if (!IVBase.HasChangeAuth) {
            return;
        }
        var width = $(".mg-wrapper").width() - 22;
        var top = $(obj).offset().top + 20;
        $(".mg-expected-date").css({ "position": "absolute", top: top, width: width });
        InvoiceExpected.show(id, contactId, mtype, function () {
            if (IVBase.bindEvent != undefined && IVBase.bindEvent != null) {
                IVBase.bindEvent();
            }
        });
    },
    showExpectedDateTip: function (obj) {
        if (!IVBase.HasChangeAuth) {
            return;
        }
        $(obj).html(HtmlLang.Write(LangModule.IV, "AddDateAndNotes", "Add Date&Notes"));
    },
    hideExpectedDateTip: function (obj) {
        $(obj).html("");
    },
    deleteList: function (gridSelector, callback, param) {
        Megi.grid(gridSelector, "deleteSelected", {
            url: IVBase.url_delete, callback: function (msg) {
                if (msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                    return;
                }
                callback(msg);
            }, param: param
        });
    },
    approval: function (gridSelector, callback) {
        Megi.grid(gridSelector, "optSelected", {
            url: IVBase.url_approve, msg: HtmlLang.Write(LangModule.IV, "AreYouSureToApproval", "Are you sure to approval?"), param: { MOperationID: IVBase.Status.AwaitingPayment }, callback: function (msg) {
                if (msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                    return;
                }
                callback(msg);
            }
        });
    },
    submitForApproval: function (gridSelector, callback) {
        Megi.grid(gridSelector, "optSelected", {
            url: IVBase.url_updateStatus, msg: HtmlLang.Write(LangModule.IV, "AreYouSureToSubmitForApproval", "Are you sure to submit for approval?"), param: { MOperationID: IVBase.Status.WaitingApproval }, callback: function (msg) {
                if (msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                    return;
                }
                callback(msg);
            }
        });
    },
    savedAsDraft: function (gridSelector, callback) {
        var url_update_repeat_status = IVBase.BillType == "Bill" ? IVBase.url_repeat_bill_updateStatus : IVBase.url_repeat_invoice_updateStatus;
        Megi.grid(gridSelector, "optSelected", {
            url: url_update_repeat_status, msg: HtmlLang.Write(LangModule.IV, "AreYouSureToSavedAsDraft", "Are you sure to saved as draft?"), param: { MOperationID: IVBase.Status.Draft }, callback: function (msg) {
                if (msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                    return;
                }
                callback(msg);
            }
        });
    },
    approvalRepeat: function (gridSelector, callback) {
        var url_update_repeat_status = IVBase.BillType == "Bill" ? IVBase.url_repeat_bill_updateStatus : IVBase.url_repeat_invoice_updateStatus;
        Megi.grid(gridSelector, "optSelected", {
            url: url_update_repeat_status, msg: HtmlLang.Write(LangModule.IV, "AreYouSureToApproval", "Are you sure to approval?"), param: { MOperationID: IVBase.Status.WaitingApproval }, callback: function (msg) {
                if (msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                    return;
                }
                callback(msg);
            }
        });
    },
    approveForSending: function (gridSelector, callback) {
        var url_update_repeat_status = IVBase.BillType == "Bill" ? IVBase.url_repeat_bill_updateStatus : IVBase.url_repeat_invoice_updateStatus;
        Megi.grid(gridSelector, "optSelected", {
            url: url_update_repeat_status, msg: HtmlLang.Write(LangModule.IV, "AreYouSureToApproveForSending", "Are you sure to approve for sending?"), param: { MOperationID: IVBase.Status.AwaitingPayment }, callback: callback
        });
    },
    deleteRepeatList: function (gridSelector, callBack) {
        var url_delete_repeat = IVBase.BillType == "Bill" ? IVBase.url_repeat_bill_delete : IVBase.url_repeat_invoice_delete;
        Megi.grid(gridSelector, "deleteSelected", {
            url: url_delete_repeat, callback: callBack
        });
    },
    unAuditToDraft: function (invId, type) {
        $.mDialog.confirm(HtmlLang.Write(LangModule.IV, "AreYouSureToUnAuditToDraft", "Are you sure to UnAudit To Draft?"), {
            callback: function () {
                var objParam = {};
                objParam.KeyIDs = invId;
                objParam.MOperationID = IVBase.Status.Draft;
                mAjax.submit(
                    IVBase.url_unApprove,
                    { invoiceId: invId },
                    function (data) {
                        if (data.Success) {
                            //提示信息
                            $.mMsg(HtmlLang.Write(LangModule.IV, "UnAuditToDraftSuccessfully", "UnAudit To Draft Successfully!"));
                            if (window.parent.$(".mCloseBox").length <= 0) {
                                //刷新发票列表页面
                                var listTabTitle = IVBase.getListTabTitle();
                                var listUrl = "";
                                if (type == IVBase.InvoiceType.Sale || type == IVBase.InvoiceType.InvoiceSaleRed) {
                                    listUrl = IVBase.url_InvoiceList + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                                }
                                else if (type == IVBase.InvoiceType.Purchase || type == IVBase.InvoiceType.InvoicePurchaseRed) {
                                    listUrl = IVBase.url_BillList + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                                }
                                $.mTab.refresh(listTabTitle, listUrl, false, true);

                            }
                            //刷新当前页面
                            if (type == IVBase.InvoiceType.Sale) {
                                $.mTab.rename(HtmlLang.Write(LangModule.IV, "EditInvoice", "Edit Invoice"));
                            }
                            else if (type == IVBase.InvoiceType.Purchase) {
                                $.mTab.rename(HtmlLang.Write(LangModule.IV, "EditBill", "Edit Bill"));
                            }
                            else if (type == IVBase.InvoiceType.InvoicePurchaseRed) {
                                $.mTab.rename(HtmlLang.Write(LangModule.IV, "Invoice_Purchase_Red", "Debit Note"));
                            }
                            else if (type == IVBase.InvoiceType.InvoiceSaleRed) {
                                $.mTab.rename(HtmlLang.Write(LangModule.IV, "CreditNote", "Credit Note"));
                            }
                            else {
                                $.mTab.rename(HtmlLang.Write(LangModule.IV, "EditCreditNote", "Edit Credit Note"));
                            }
                            switch (type) {
                                case IVBase.InvoiceType.Sale:
                                    mWindow.reload(IVBase.url_Edit + "/" + invId);
                                    break;
                                case IVBase.InvoiceType.InvoiceSaleRed:
                                    mWindow.reload(IVBase.url_CreditNoteEdit + "/" + invId);
                                    break;
                                case IVBase.InvoiceType.Purchase:
                                    mWindow.reload(IVBase.url_BillEdit + "/" + invId);
                                    break;
                                case IVBase.InvoiceType.InvoicePurchaseRed:
                                    mWindow.reload(IVBase.url_BillCreditNoteEdit + "/" + invId);
                                    break;
                                default:
                                    break;
                            }
                        }
                        else {
                            $.mAlert(data.Message, function () {
                                mWindow.reload();
                            });
                        }
                    });
            }
        });
    },
    RedirectToListPage: function () {

    },
    saveInvoice: function (param, callback) {
        var type = param.MType.indexOf('Purchase') > 0 ? "Bill" : "Sale";
        $("body").mFormSubmit({
            url: IVBase.url_update + "?type=" + type, param: { model: param }, validate: true, callback: function (msg) {
                callback(msg);
            }
        });
    },
    getInvoiceModel: function (invoiceId, callback) {
        mAjax.post(
            IVBase.url_getModel,
            { id: invoiceId },
            callback
        );
    },
    //获取列表分类Tab编号
    getCurrentType: function () {
        var hidType = $("#hidType").val();
        var currentType = 0;
        if (hidType == "Invoice") {
            //销售发票
            currentType = InvoiceList.CurrentType;
        } else if (hidType == "Bill") {
            //采购发票
            currentType = BillList.CurrentType;
        }
        return currentType;
    },
    //获取tab选项卡标题
    getTabTitle: function (type) {
        //发票类别
        var invoiceType = $("#hidInvoiceType").val();
        //摘要
        var ref = $.trim($("#txtRef").val());
        //销售发票
        if (invoiceType == IVBase.InvoiceType.Sale) {
            if (type) {
                switch (type) {
                    case "NewInvoice":
                        return HtmlLang.Write(LangModule.IV, "NewInvoice", "New Invoice");
                    case "CopyInvoice":
                        return HtmlLang.Write(LangModule.IV, "CopyInvoice", "Copy Invoice");
                    default:
                        return "";
                }
            } else {
                //如果没有备注，则显示发票号
                //return $.mIV.getTitle(mTitle_Pre_Invoice, ref ? ref : $("#txtInvoiceNo").val());
                return HtmlLang.Write(LangModule.IV, "EditInvoice", "Edit Invoice");
            }
        }
        //销售发票（红字）
        if (invoiceType == IVBase.InvoiceType.InvoiceSaleRed) {
            if (type) {
                switch (type) {
                    case "NewCreditNote":
                        return HtmlLang.Write(LangModule.IV, "NewCreditNote", "New Credit Note");
                    case "CopyCreditNote":
                        return HtmlLang.Write(LangModule.IV, "CopyCreditNote", "Copy Credit Note");
                    default:
                        return HtmlLang.Write(LangModule.IV, "Invoice_Sale_Red", "Credit Note");
                }
            } else {
                return HtmlLang.Write(LangModule.IV, "EditCreditNote", "Edit Credit Note");
            }
        }
        //采购发票
        if (invoiceType == IVBase.InvoiceType.Purchase) {
            if (type) {
                switch (type) {
                    case "NewBill":
                        return HtmlLang.Write(LangModule.IV, "NewBill", "New Bill");
                    case "CopyBill":
                        return HtmlLang.Write(LangModule.IV, "CopyBill", "Copy Bill");
                    default:
                        return "";
                }
            } else {
                return HtmlLang.Write(LangModule.IV, "EditBill", "Edit Bill");
            }
        }
        //采购发票（红字）
        if (invoiceType == IVBase.InvoiceType.InvoicePurchaseRed) {
            if (type) {
                switch (type) {
                    case "NewCreditNote":
                        return HtmlLang.Write(LangModule.IV, "NewCreditNote", "New Credit Note");
                    case "CopyCreditNote":
                        return HtmlLang.Write(LangModule.IV, "CopyCreditNote", "Copy Credit Note");
                    default:
                        return "";
                }
            } else {
                //return ref != "" ? $.mIV.getTitle(mTitle_Pre_Bill_Red, ref) : HtmlLang.Write(LangModule.IV, "EditCreditNote", "Edit Credit Note");
                return HtmlLang.Write(LangModule.IV, "EditCreditNote", "Edit Credit Note");
            }
        }
    },
    //获取发票列表tab选项卡标题（销售发票 或者 采购发票）
    getListTabTitle: function () {
        //tab选项卡标题
        var title = "";
        //发票模型
        var msg = mText.getObject("#hidInvoiceModel");;
        //销售发票
        if (msg.MType == IVBase.InvoiceType.Sale || msg.MType == IVBase.InvoiceType.InvoiceSaleRed) {
            title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
        }
        //采购发票
        if (msg.MType == IVBase.InvoiceType.Purchase || msg.MType == IVBase.InvoiceType.InvoicePurchaseRed) {
            title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
        }
        return title;
    },
    //发票删除（加删除标记）
    deleteItem: function (id, redirectUrl) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete, {
            callback: function () {
                var param = {};
                param.KeyIDs = id;
                mAjax.submit(
                    IVBase.url_delete,
                    { param: param },
                    function (msg) {
                        if (msg.Success) {
                            //提示信息
                            $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                            //
                            if (window.parent.$(".mCloseBox").length == 0) {
                                //如果存在 redirectUrl 则说明是通过 编辑页面 或者 查看页面 进行删除的
                                if (redirectUrl) {
                                    //在指定的tab选项卡中刷新
                                    $.mTab.addOrUpdate(IVBase.getListTabTitle(), redirectUrl);
                                    //关闭当前选项卡
                                    $.mTab.remove();

                                } else {
                                    //如果不存在 redirectUrl 则说明是通过 列表页面 进行删除的
                                    var hidType = $("#hidType").val();
                                    if (hidType == "Invoice") {
                                        //销售发票
                                        window.location = InvoiceList.url_List + InvoiceList.CurrentType;
                                    } else if (hidType == "Bill") {
                                        //采购发票
                                        window.location = BillList.url_List + BillList.CurrentType;
                                    }
                                }
                            }
                            else {
                                $.mDialog.close();
                            }
                        } else {
                            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Have cancel after verification, not allowed to delete"));
                        }
                    });
            }
        });
    },
    editItem: function (MStatus, MType, MID, bankId) {
        //InitBills对象不存在，说明不是初始化单据
        var type = typeof InitBills;
        if (type == "undefined") {
            if (IVBase.IsClick) {
                var hidType = $("#hidType").val();
                var list = null;
                if (hidType == "Invoice") {//销售发票

                    list = typeof (InvoiceList) != 'undefined' ? InvoiceList : { CurrentType: 0 };
                    if (!IVBase.HasChangeAuth || MStatus == IVBase.Status.AwaitingPayment || MStatus == IVBase.Status.Paid) {
                        if (MType == IVBase.InvoiceType.Sale) {
                            var tabTitle = HtmlLang.Write(LangModule.IV, "ViewInvoice", "View Invoice");
                            if (MStatus == IVBase.Status.AwaitingPayment) {
                                tabTitle = HtmlLang.Write(LangModule.IV, "EditInvoice", "Edit Invoice");
                            }
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_View + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        } else if (MType == IVBase.InvoiceType.InvoiceSaleRed) {
                            //红字销售单 
                            var tabTitle = HtmlLang.Write(LangModule.IV, "CreditNote", "View Credit Note");
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_CreditNoteView + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        }
                    } else {
                        if (MType == IVBase.InvoiceType.Sale) {
                            var tabTitle = HtmlLang.Write(LangModule.IV, "EditInvoice", "Edit Invoice");
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_Edit + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        } else if (MType == IVBase.InvoiceType.InvoiceSaleRed) {
                            var tabTitle = HtmlLang.Write(LangModule.IV, "Invoice_Sale_Red", "View Credit Note");
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_CreditNoteEdit + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        }
                    }

                } else if (hidType == "Bill") {//采购发票

                    list = typeof (BillList) != 'undefined' ? BillList : { CurrentType: 0 };
                    if (!IVBase.HasChangeAuth || MStatus == IVBase.Status.AwaitingPayment || MStatus == IVBase.Status.Paid) {
                        if (MType == IVBase.InvoiceType.Purchase) {
                            var tabTitle = HtmlLang.Write(LangModule.IV, "ViewBill", "View Bill");
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_BillView + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        } else if (MType == IVBase.InvoiceType.InvoicePurchaseRed) {
                            //采购单  
                            var tabTitle = HtmlLang.Write(LangModule.IV, "ViewDebitNote", "View Credit Note");
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_BillCreditNoteView + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        }
                    } else {
                        if (MType == IVBase.InvoiceType.Purchase) {
                            var tabTitle = HtmlLang.Write(LangModule.IV, "EditBill", "Edit Bill");
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_BillEdit + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        } else if (MType == IVBase.InvoiceType.InvoicePurchaseRed) {
                            var tabTitle = HtmlLang.Write(LangModule.IV, "EditCreditNote", "Edit Credit Note");
                            $.mTab.addOrUpdate(tabTitle, IVBase.url_BillCreditNoteEdit + "/" + MID + "?tabIndex=" + list.CurrentType + "&sv=1");
                        }
                    }
                }
            }
        } else {
            //初始化单据的逻辑
            if (IVBase.IsClickEdit) {
                switch (MType) {
                    case IVBase.InvoiceType.Sale:
                        InitBills.AddInvoiceDialog(MID);
                        break;
                    case IVBase.InvoiceType.InvoiceSaleRed:
                        InitBills.AddInvCreditNoteDialog(MID);
                        break;
                    case IVBase.InvoiceType.Purchase:
                        InitBills.AddBillsDialog(MID);
                        break;
                    case IVBase.InvoiceType.InvoicePurchaseRed:
                        InitBills.AddBillsCreditNoteDialog(MID);
                        break;
                    case IVBase.InvoiceType.ReceiveSale:
                    case IVBase.InvoiceType.ReceiveOther:
                    case IVBase.InvoiceType.ReceiveOtherReturn:
                        InitBills.AddReceiveDialog(MID, bankId);
                        break;
                    case IVBase.InvoiceType.PayPurchase:
                    case IVBase.InvoiceType.PayPurReturn:
                    case IVBase.InvoiceType.PayOther:
                    case IVBase.InvoiceType.PayOtherReturn:
                        InitBills.AddSpendDialog(MID, bankId);
                        break;
                    default:
                        break;
                }
            }
        }
    },
    //删除重复发票（加删除标记）
    deleteRepeatInvoice: function (id, billType, redirectUrl) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete, {
            callback: function () {
                var param = {};
                param.KeyIDs = id;
                var ref = $.trim($("#txtRef").val());
                var delete_repeatInvoice_url = "/IV/Invoice/DeleteRepeatInvoiceList";
                if (billType == IVBase.InvoiceType.Purchase) {
                    delete_repeatInvoice_url = "/IV/Bill/DeleteRepeatBillList";
                }
                mAjax.submit(
                    delete_repeatInvoice_url,
                    { param: param },
                    function (msg) {
                        if (msg.Success) {
                            //提示信息
                            $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                            //如果存在 redirectUrl 则说明是通过 编辑页面 或者 查看页面 进行删除的
                            if (redirectUrl) {
                                //在指定的tab选项卡中刷新
                                $.mTab.addOrUpdate(IVBase.getListTabTitle(), redirectUrl);
                                //关闭当前选项卡
                                $.mTab.remove();
                            } else {
                                //如果不存在 redirectUrl 则说明是通过 列表页面 进行删除的
                                var hidType = $("#hidType").val();
                                if (hidType == "Invoice") {
                                    //销售发票
                                    window.location = InvoiceList.url_List + InvoiceList.CurrentType;
                                } else if (hidType == "Bill") {
                                    //采购发票
                                    window.location = BillList.url_List + BillList.CurrentType;
                                }
                            }
                        } else {
                            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Deletefailed"));
                        }
                    });
            }
        });
    },
    //编辑重复发票
    editRepeatInvoice: function (MID, ref) {
        var ref = ref == "null" ? null : $.trim(ref) == "" ? null : ref;
        if (IVBase.IsClick) {
            var hidType = $("#hidType").val();
            var list = null;
            if (hidType == "Invoice") {//销售发票
                list = InvoiceList;
                //var tabTitle = $.mIV.getTitle(mTitle_Pre_Invoice, ref != null ? ref : HtmlLang.Write(LangModule.IV, "EditRepeatingInvoice", "Edit Repeating Invoice"));
                if (!IVBase.HasChangeAuth) {
                    var tabTitle = HtmlLang.Write(LangModule.IV, "ViewRepeatingInvoice", "View Repeating Invoice");
                    $.mTab.addOrUpdate(tabTitle, "/IV/Invoice/RepeatInvoiceView/" + MID + "?tabIndex=" + InvoiceList.CurrentType);
                } else {
                    var tabTitle = HtmlLang.Write(LangModule.IV, "EditRepeatingInvoice", "Edit Repeating Invoice");
                    $.mTab.addOrUpdate(tabTitle, "/IV/Invoice/RepeatInvoiceEdit/" + MID + "?tabIndex=" + InvoiceList.CurrentType);
                }

            } else if (hidType == "Bill") {//采购发票
                list = BillList;
                //var tabTitle = $.mIV.getTitle(mTitle_Pre_Bill, ref != null ? ref : HtmlLang.Write(LangModule.IV, "EditRepeatingBill", "Edit Repeating Bill"));
                if (!IVBase.HasChangeAuth) {
                    var tabTitle = HtmlLang.Write(LangModule.IV, "ViewRepeatingBill", "View Repeating Bill");
                    $.mTab.addOrUpdate(tabTitle, "/IV/Bill/RepeatBillView/" + MID + "?tabIndex=" + BillList.CurrentType);
                } else {
                    //显示的是编辑重复销售单
                    //var tabTitle = HtmlLang.Write(LangModule.IV, "EditRepeatingBill", "Edit Repeating Bill");
                    var tabTitle = HtmlLang.Write(LangModule.IV, "EditRepeatingPurchase", "编辑重复采购单");
                    $.mTab.addOrUpdate(tabTitle, "/IV/Bill/RepeatBillEdit/" + MID + "?tabIndex=" + BillList.CurrentType);
                }
            }
        }
    },
    OptType: {
        Save: "1",
        Delete: "2",
        Void: "3",
        Approval: "4",
        Payment: "5",
        Reconcile: "6"
    },
    setOptInfo: function () {
        var opt = Megi.request("opt");
        var msg = "";
        switch (opt) {
            case IVBase.OptType.Save:
                msg = "Save Successfully!";
                break;
            case IVBase.OptType.Delete:
                msg = "Delete Successfully!";
                break;
            case IVBase.OptType.Void:
                msg = "Void Successfully!";
                break;
            case IVBase.OptType.Approval:
                msg = "Approval Successfully!";
                break;
            case IVBase.OptType.Payment:
                msg = "Make a payment Successfully!";
                break;
            case IVBase.OptType.Reconcile:
                msg = "Reconcile Successfully!";
                break;
        }
        if (msg != "") {
            $.mMsg(msg);
        }
    },
    viewFileInfo: function (fileId, fileIds) {
        Megi.openDialog('/BD/Docs/FileView', '', 'curFileId=' + fileId + '&fileIds=' + fileIds, 560, 460);
    },
    OpenPrintDialog: function (title, queryParam, reportType) {
        title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Print", "Print"), title]);
        var param = $.toJSON({ reportType: reportType, jsonParam: escape(queryParam) });
        Print.previewPrint(title, param);
    },
    getPrintTitle: function (billType) {
        var title = '';
        switch (billType) {
            case IVBase.InvoiceType.Sale:
                title = HtmlLang.Write(LangModule.IV, "Invoice", "Invoice");
                break;
            case IVBase.InvoiceType.InvoiceSaleRed:
                title = HtmlLang.Write(LangModule.IV, "CreditNote", "Credit Note");
                break;
            case IVBase.InvoiceType.Purchase:
                title = HtmlLang.Write(LangModule.IV, "Bills", "Bill");
                break;
            case IVBase.InvoiceType.InvoicePurchaseRed:
                title = HtmlLang.Write(LangModule.IV, "Invoice_Purchase_Red", "Debit Note");
                break;
        }
        return title;
    },
    //打开打印对话框
    OpenIVEditPrintDialog: function (invoiceId, billType, reportType) {
        var json = "{\"ReportId\":\"" + invoiceId + "\", \"BillType\":\"" + billType + "\"}";
        var title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Print", "Print"), IVBase.getPrintTitle(billType)]);
        var param = $.toJSON({ reportType: reportType, jsonParam: escape(json) });
        Print.previewPrint(title, param);
    },
    initAdvSearch: function () {
        $(".m-adv-search-btn").click(function () {
            $(".m-adv-search").show();
            $(this).hide();
            if ($(".m-tab-toolbar").find("a:visible").length == 0) {
                $(".m-tab-toolbar").hide()
            }
        });
        $(".m-adv-search>.m-adv-search-close>a").click(function () {
            $(".m-adv-search").hide();
            $(".m-adv-search-btn").show();
            $(".m-tab-toolbar").show();
        });
        $(".m-adv-search").find("#aClearSearchFilter").click(function () {
            $("body").mFormClearForm();
            $("#selSearchWithin").combobox("setValue", "1");
        });
    },
    tabSelRefreshTitle: function (type, id) {
        var reqUrl = "";
        switch (type) {
            case 1:
                reqUrl = "/IV/IVInvoiceBase/GetSummaryModel?type=" + IVBase.InvoiceType.Sale;
                break;
            case 2:
                reqUrl = "/IV/IVInvoiceBase/GetSummaryModel?type=" + IVBase.InvoiceType.Purchase;
                break;
            case 3:
                reqUrl = "/IV/Expense/GetExpenseSummaryModel";
                break;
            case 4:
                reqUrl = "/PA/SalaryPayment/GetSalaryPaymentSummaryModel/" + id;
                break;
        }
        mAjax.post(reqUrl, null, function (data) {
            $(".m-extend-tabs").find("ul>li>.sub-title").each(function (i) {
                switch (i) {
                    case 0:
                        $(this).html("(" + data.AllCount + " " + IVBase.ShowCountUnit(data.AllCount) + ")");
                        $(this).next().html(Megi.Math.toMoneyFormat(data.AllAmount));
                        break;
                    case 1:
                        $(this).html("(" + data.DraftCount + " " + IVBase.ShowCountUnit(data.DraftCount) + ")");
                        $(this).next().html(Megi.Math.toMoneyFormat(data.DraftAmount));
                        break;
                    case 2:
                        $(this).html("(" + data.WaitingApprovalCount + " " + IVBase.ShowCountUnit(data.WaitingApprovalCount) + ")");
                        $(this).next().html(Megi.Math.toMoneyFormat(data.WaitingApprovalAmount));
                        break;
                    case 3:
                        $(this).html("(" + data.WaitingPaymentCount + " " + IVBase.ShowCountUnit(data.WaitingPaymentCount) + ")");
                        $(this).next().html(Megi.Math.toMoneyFormat(data.WaitingPaymentAmount));
                        break;
                    case 4:
                        $(this).html("(" + data.PaidCount + " " + IVBase.ShowCountUnit(data.PaidCount) + ")");
                        $(this).next().html(Megi.Math.toMoneyFormat(data.PaidAmount));
                        break;
                    case 5:
                        $(this).html("(" + data.RepeatingCount + " " + IVBase.ShowCountUnit(data.RepeatingCount) + ")");
                        $(this).next().html(Megi.Math.toMoneyFormat(data.RepeatingAmount));
                        break;
                }
            });
        });
    },
    ShowCountUnit: function (count) {
        return count > 1 ? HtmlLang.Write(LangModule.Common, "items", "items") : HtmlLang.Write(LangModule.Common, "item", "item");
    },
    changeSearchWithin: function (type) {
        var sw_combobox_data = [];
        if (type == 6) {
            sw_combobox_data = [
                { value: "1", text: HtmlLang.Write(LangModule.IV, "Anydate", "Any date") },
                { value: "5", text: IVBase.BillType == "Invoice" ? HtmlLang.Write(LangModule.IV, "NextInvoiceDate", "Next invoice date") : HtmlLang.Write(LangModule.IV, "NextBillDate", "Next bill date") },
                { value: "6", text: HtmlLang.Write(LangModule.IV, "EndDate", "End date") }
            ];
            $("#divUnsentOnly").hide();
            $("#divUnsentOnly .mg-data").attr("checked", false);
        } else {
            sw_combobox_data = [
                { value: "1", text: HtmlLang.Write(LangModule.IV, "Anydate", "Any date") },
                { value: "2", text: HtmlLang.Write(LangModule.IV, "TransactionDate", "Transaction date") },
                { value: "3", text: HtmlLang.Write(LangModule.IV, "DueDate", "Due date") },
                { value: "4", text: HtmlLang.Write(LangModule.IV, "ExpectedDate", "Expected date") }
            ];
            if (type == 1 || type == 4 || type == 5) {
                $("#divUnsentOnly").show();
            } else {
                $("#divUnsentOnly").hide();
                $("#divUnsentOnly .mg-data").attr("checked", false);
            }
        }
        $("#selSearchWithin").combobox({
            valueField: 'value', textField: 'text',
            data: sw_combobox_data,
            onLoadSuccess: function () {
                $("#selSearchWithin").combobox("setValue", "1");
            }
        });
        if (type == 6) {
            //如果type是6点击的是重复销售单
            $("#aSearch").hide();
            //隐藏整个搜索框
            $("#SearchBar").hide();
        }
        else {
            //bug26203
            if (type != 0) {
                $(".m-tab-toolbar").show();
                $("#aSearch").show();
            }
        }
    },
    //结束编辑分录
    endEditEntry: function (e, selector) {
        var divEntry = $(e.target).closest(".m-iv-entry");
        var divAddItem = $(e.target).closest(".add-combobox-item");
        var divDialog = $(e.target).closest(".XYTipsWindow");
        if ($(e.target).hasClass("m-iv-entry") || divEntry.length == 0 && divAddItem.length == 0 && divDialog.length == 0) {
            var activeRow = $(".m-iv-entry .datagrid-btable").find(".datagrid-row-editing");
            if (activeRow.length == 1) {
                var rowIdx = activeRow.attr("datagrid-row-index");
                Megi.grid(selector, "endEdit", rowIdx);
            }
        }
    }
}

$(document).ready(function () {
    IVBase.setOptInfo();
    IVBase.initAdvSearch();
});