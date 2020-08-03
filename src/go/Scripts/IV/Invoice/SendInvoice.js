/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var EmailSendType = { None: "None", Invoice: "Invoice", Statement: "Statement", RepeatingInvoice: "RepeatingInvoice", Payslip: "Payslip" };
var ActionType = { Send: 'Send', ChangeTmpl: "ChangeTmpl", SaveRepeatTmpl: "SaveRepeatTmpl" };

var SendInv = {
    Selector: "#gridInvSend",
    //区分发送发票列表/客户对账单
    sendType: $("#sendType").val(),
    //发票列表Tab索引号
    status: parseInt($("#status").val()),
    //发票类别
    MType: $("#hidMType").val(),
    emailTmpl: null,
    focusObj: null,
    isPreview: false,
    previewTmplData: null,
    placeHolder: eval('(' + $("#hidPlaceHolder").val() + ')'),
    previewIndex: 0,
    isChanged: false,
    lastTmplId: null,
    prevTmplId: null,
    isCancelAdd: false,
    init: function () {
        $.mDialog.max();
        if (Megi.isIE()) {
            FWInner.initFW();
        }
        SendInv.initUI();
        SendInv.sendGrid();
        SendInv.clickAction();
        SendInv.bindTmpl();
    },
    initUI: function () {
        $("#EmailTemplate_value").find(".combo-panel").css({ "max-height": "238px" });
        if (SendInv.sendType == EmailSendType.Payslip) {
            $("#divTmpl").hide();
            $("#includePDF").attr("checked", true);
        }
        else if (SendInv.sendType == EmailSendType.Invoice) {
            $("#includePDF").attr("checked", true);
        }
    },
    sendGrid: function () {
        //发票ID串
        var selectIds = $("#selIds").val();
        var contactId = $("#hidContactId").val();
        //开始查询数据并绑定数据列表
        Megi.grid(SendInv.Selector, {
            resizable: true,
            auto: true,
            url: "/IV/Invoice/GetSendInvoiceList",
            queryParams: { KeyIDs: selectIds, SendType: SendInv.sendType, ContactID: contactId },
            columns: SendInv.getGridColumnList(),
            onLoadSuccess: function () {
                //初始化发送的总行数
                SendInv.inputEntryCount();
                Megi.regClickToSelectAllEvt();
            },
        });
    },
    //删除一行
    DeleteItem: function (btnObj) {
        var arr = Megi.grid(SendInv.Selector, "getRows");
        if (arr.length <= 1) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "AtLeastOneLineItem", "You must have at least 1 line item."));
            return;
        }
        var rowIndex = $(btnObj).closest(".datagrid-row").prevAll().length;
        Megi.grid(SendInv.Selector, "deleteRow", rowIndex);
        SendInv.inputEntryCount();
    },
    //发送的总行数
    inputEntryCount: function () {
        var arr = Megi.grid(SendInv.Selector, "getRows");
        var str = arr.length.toString();
        if (SendInv.sendType == EmailSendType.Invoice || SendInv.sendType == EmailSendType.RepeatingInvoice) {
            if (arr.length <= 1) {
                str += " " + HtmlLang.Write(LangModule.IV, "Invoice", "Invoice");
            } else {
                str += " " + HtmlLang.Write(LangModule.IV, "invoices", "invoices");
            }
        } else if (SendInv.sendType == EmailSendType.Statement) {
            if (arr.length <= 1) {
                str += " " + HtmlLang.Write(LangModule.IV, "statement", "statement");
            } else {
                str += " " + HtmlLang.Write(LangModule.IV, "statements", "statements");
            }
            
        } else if (SendInv.sendType == EmailSendType.Payslip) {
            if (arr.length <= 1) {
                str += " " + HtmlLang.Write(LangModule.IV, "payslip", "payslip");
            } else {
                str += " " + HtmlLang.Write(LangModule.IV, "Payslips", "payslips");
            }
        }
        //if (arr.length <= 1) {
        //    str = str.trimEnd('s');
        //}
        $("#entryCount").html(str);

        //全选自动选中
        var isAllSelect = true;
        for (var i = 0; i < arr.length; i++) {
            var checkbox = $('tr[class*="datagrid-row"] td[field="MIsSent"]').eq(i).find('input[class="row-key-checkbox"]');
            if (checkbox.length > 0) {
                if (!checkbox.prop("checked")) {
                    isAllSelect = false;
                    break;
                }
            }
        }
        $(".datagrid-header-row").find("td[field='MIsSent']").find("span").eq(0).find('input[type="checkbox"]').attr("checked", isAllSelect);

        //重复发票邮件信息编辑
        if (SendInv.sendType == EmailSendType.RepeatingInvoice) {
            if (arr.length == 1) {
                if (arr[0].MID) {
                    $("#includePDF").attr("checked", arr[0].MIsIncludePDFAttachment);
                    $("#SendMeACopy").attr("checked", arr[0].MIsSendMeACopy);
                } else {
                    var totalAmount = $.trim($("#hidTotalAmount").val());
                    if (totalAmount != "") {
                        $('tr[class*="datagrid-row"] td[field="MAmount"]').eq(0).find('div').text(totalAmount);
                    }
                    $('tr[class*="datagrid-row"] td[field="MIsSent"]').eq(0).find('input[class="row-key-checkbox"]').attr("checked", parent.RepeatInvoiceEditBase.MIsMarkAsSent);
                    $("#includePDF").attr("checked", parent.RepeatInvoiceEditBase.MIsIncludePDFAttachment);
                    $("#SendMeACopy").attr("checked", parent.RepeatInvoiceEditBase.MIsSendMeACopy);
                }
            }
        }
    },
    //校验邮箱有效性
    validEmail: function (callback) {
        var emailArr = $('input[class*="email-text"]');
        for (var i = 0; i < emailArr.length; i++) {
            var emailText = emailArr[i].value;
            emailArr[i].value = $.trim(emailText).replace(/(^,)|(,$)|(^;)|(;$)/g, "");
            
            //正则过于简单，替换成easyUI的正则
            //var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{1,20})$/;
            //mEmailReg 存在系统变量里面
            var reg = mEmailReg;
            var arrEmail = emailArr[i].value.split(/,|;/);
            var isValid = true;
            $.each(arrEmail, function (i, email) {
                if (!reg.test(email)) {
                    isValid = false;
                    return false;
                }
            });

            if (!isValid) {
                $.mDialog.alert(HtmlLang.Write(LangModule.IV, "EmailAddressesIsMissingOrInvalid", "One or more email addresses is missing or invalid."));
                return;
            }
        }
        callback();
    },
    getParam: function () {
        var obj = {};
        obj.MReplyEmail = $("#replyEmail").val();
        obj.MFromUserName = $("#fromUserName").val();
        obj.MSubject = $("#MsgSubject").val();
        obj.MContent = $("#MsgContent").val().replace(/\n|\r\n/g, "<br>");
        if (SendInv.sendType != EmailSendType.Payslip) {
            obj.MEmailTemplate = $("#EmailTemplate_value").combobox("getValue");
        }
        obj.MSendMeACopy = $("#SendMeACopy").prop("checked");
        obj.MSendType = SendInv.sendType;
        var includePDF = $("#includePDF").prop("checked");
        if (SendInv.sendType == EmailSendType.Invoice) {
            obj.MIncludePDF = includePDF;
        }
        else if (SendInv.sendType == EmailSendType.Statement) {
            obj.MIncludePDF = true;
            obj.MBeginDate = $("#beginDate").val();
            obj.MEndDate = $("#endDate").val();
        } else if (SendInv.sendType == EmailSendType.Payslip) {
            obj.MSalaryPeriod = $("input:hidden[name=period]", parent.document).val();//Megi.getUrlParam("period");
            obj.MIncludePDF = includePDF;
        }
        var arr = new Array();
        var emailOrigArr = $('input[id="OriginalContactEmail"]');
        var emailArr = $('input[class*="email-text"]');

        //获取选中的行
        var rows = Megi.grid(SendInv.Selector, "getRows");
        //附件路径集合
        var dicIdPath = eval("(" + $("#dicIdPath")[0].value + ")");
        //遍历选中的行
        $(rows).each(function (i) {
            var entry = {};
            //发票或者工资
            if (SendInv.sendType == EmailSendType.Invoice || SendInv.sendType == EmailSendType.Payslip) {
                if (SendInv.sendType == 1) { entry.MContactNetwork = rows[i].MContactNetwork; }
                entry.MID = rows[i].MID;//发票ID
                entry.MInvNumber = rows[i].MInvNumber;//发票号

                var checkbox = $('tr[class*="datagrid-row"] td[field="MIsSent"]').eq(i).find('input[class="row-key-checkbox"]');
                if (checkbox.length > 0) {
                    entry.MIsSent = checkbox.prop("checked");
                }
                entry.MFilePath = dicIdPath == null ? "" : dicIdPath[rows[i].MID];
            }
            else if (SendInv.sendType == EmailSendType.Statement) {
                entry.MFilePath = dicIdPath[rows[i].MContactID];
            }
            entry.MContactID = rows[i].MContactID;//联系人ID
            entry.MContactName = rows[i].MContactName;//联系人名称
            entry.MContactPrimaryPerson = rows[i].MContactPrimaryPerson;//主要联系人
            if (emailOrigArr.length > i) {
                entry.MOriginalContactEmail = emailOrigArr[i].value;
            }
            if (emailArr.length > i) {
                entry.MContactEmail = emailArr[i].value;
            }

            arr.push(entry);
        });
        //要发送的邮件列表
        obj.SendEntryList = arr;
        return obj;
    },
    clickAction: function () {
        //发送
        $("#aSend").click(function () {
            if (!$(".required-field").mFormValidate()) {
                return;
            }
            SendInv.checkTmplChange(ActionType.Send, function () {
                SendInv.validEmail(function () {
                    //开始发送
                    mAjax.submit("/IV/Invoice/SendInvoiceList", { model: SendInv.getParam() }, function (data) {
                        //发送成功
                        if (data.Success) {
                            //提示邮件已经提交服务器发送
                            var successMsg = HtmlLang.Write(LangModule.IV, "EmailsSubmittedToServer", "the emails have been submitted to the server to send.");
                            $.mMsg(successMsg);
                            //如果是销售发票列表点击发送的，需要刷新列表（后台status=3代表等待付款，前端status=4代表等待付款）
                            if (SendInv.MType == "Invoice_Sale") {
                                parent.InvoiceList.bindGrid(SendInv.status + 1);
                            }
                            //如果是采购发票列表点击发送的，需要刷新列表
                            if (SendInv.MType == "Invoice_Purchase") {
                                parent.BillList.bindGrid(SendInv.status + 1);
                            }
                            //如果是工资条发送
                            if (SendInv.sendType == EmailSendType.Payslip) {
                                parent.SalaryPaymentList.reload();
                            }
                            //关闭对话
                            Megi.closeDialog();
                        } else {
                            //发送失败，提示错误信息
                            $.mDialog.alert(data.Message);
                        }
                    });
                });
            });
        });
        //保存
        $("#aSave").click(function () {
            SendInv.checkTmplChange(ActionType.SaveRepeatTmpl, function () {
                SendInv.validEmail(function () {
                    var obj = {};
                    obj.MReplyEmail = $("#replyEmail").val();
                    obj.MFromUserName = $("#fromUserName").val();
                    obj.MEmailTemplate = $("#EmailTemplate_value").combobox("getValue");
                    obj.MSendMeACopy = $("#SendMeACopy").prop("checked");
                    obj.MSendType = SendInv.sendType;
                    obj.MIncludePDF = $("#includePDF").prop("checked");
                    var MIsMarkAsSent = true;

                    var arr = new Array();
                    var emailOrigArr = $('input[id="OriginalContactEmail"]');
                    var emailArr = $('input[class*="email-text"]');

                    //获取选中的行
                    var rows = Megi.grid(SendInv.Selector, "getRows");
                    //附件路径集合
                    var dicIdPath = eval("(" + $("#dicIdPath")[0].value + ")");
                    //遍历选中的行
                    $(rows).each(function (i) {
                        var entry = {};
                        entry.MID = rows[i].MID;//发票ID
                        entry.MContactNetwork = rows[i].MContactNetwork;
                        var checkbox = $('tr[class*="datagrid-row"] td[field="MIsSent"]').eq(i).find('input[class="row-key-checkbox"]');
                        if (checkbox.length > 0) {
                            entry.MIsSent = checkbox.prop("checked");
                            MIsMarkAsSent = entry.MIsSent;
                        }
                        entry.MContactID = rows[i].MContactID;//联系人ID
                        entry.MContactName = rows[i].MContactName;//联系人姓名
                        if (emailOrigArr.length > i) {
                            entry.MOriginalContactEmail = emailOrigArr[i].value;
                        }
                        if (emailArr.length > i) {
                            entry.MContactEmail = emailArr[i].value;
                        }
                        arr.push(entry);
                    });
                    obj.SendEntryList = arr;
                    mAjax.submit("/IV/Invoice/UpdateRepeatInvoiceMessage", { model: obj }, function (data) {
                        //发送成功
                        if (data.Success) {
                            if (SendInv.MType == "Invoice_Sale") {
                                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveForSendingSuccessfully", "Approve For Sending Successfully!"));
                                parent.InvoiceList.bindGrid(6);
                            } else {
                                parent.RepeatInvoiceEditBase.MIsMarkAsSent = MIsMarkAsSent;
                                parent.RepeatInvoiceEditBase.MIsIncludePDFAttachment = obj.MIncludePDF;
                                parent.RepeatInvoiceEditBase.MIsSendMeACopy = obj.MSendMeACopy;
                                $.mMsg(LangKey.SaveSuccessfully);
                            }
                            //关闭对话
                            Megi.closeDialog();
                        } else {
                            //发送失败，提示错误信息
                            $.mDialog.alert(data.Message);
                        }
                    });
                });
            });
        });
        //取消
        $("#aCancel").click(function () {
            Megi.closeDialog();
        });
        $("#aNew, #aCopy").click(function () {
            SendInv.isPreview = false;
            SendInv.previewTmpl(false);
            SendInv.lastTmplId = $("#EmailTemplate_value").combobox("getValue");
            SendInv.addTmpl(this);
            if (this.id == "aNew") {
                $("#divMsgSubject").html('');
                $("#divMsgContent").html('');
            }
        });
        $("#aDelete").click(function () {
            var isSys = $("#hidIsSys").val();
            if (isSys != "1" && isSys.toLowerCase() != "true") {
                var tmplId = $("#EmailTemplate_value").combobox("getValue");
                SendInv.deleteTmpl(tmplId);
            }
        });
        $("#aSaveTmpl").click(function () {
            SendInv.saveTmpl();
        });
        $("#aCancelTmpl").click(function () {
            $("#divTmplName").hide();
            $("#txtTmplName").val('');
            $("#EmailTemplate_value").combobox("enable");
            $("#aNew, #aCopy").show();
            SendInv.isCancelAdd = true;
            SendInv.resetTmpl();
            SendInv.isCancelAdd = false;
        });

        $("#aInsertPlaceholder").click(function (e) {
            Megi.popup("#aInsertPlaceholder", { selector: "#divInsertPlaceholder", event: e, scrollObj: $(".m-imain") });
        });
        $("#ul-insert-placeholder a").click(function () {
            if (SendInv.focusObj && $(SendInv.focusObj).is(":visible")) {
                var key = $(this).attr("key");
                //在光标处插入内容
                $(SendInv.focusObj).insertAtCaret(key);
                $(SendInv.focusObj).blur();
                SendInv.bindPHLabel(SendInv.focusObj.id, $(SendInv.focusObj).val());
            }
            $("#divInsertPlaceholder").hide();
        });
        $("#MsgSubject,#MsgContent").click(function () {
            SendInv.focusObj = this;
        });
        $(".email-tmpl-ph").live("click", function () {
            var parentId = $(this).parent().attr("id").replace("div", "");
            SendInv.focusObj = $("#" + parentId)[0];
            if (SendInv.focusObj) {
                var phWapper = $(this).find("em");
                var key = phWapper.attr("key");
                var val = phWapper.text();
                var matchedIdx = /\(([^)]+)\)/.exec(val);
                var idx;
                
                var tmplVal;
                var objVal = $(SendInv.focusObj).val();
                if (matchedIdx == null) {
                    tmplVal = objVal.replace(key, "");
                }
                else {
                    idx = parseInt(matchedIdx[1]);
                    tmplVal = SendInv.replaceIndex(objVal, key.replace("[", "\\[").replace("]", "\\]"), idx);
                }
                $(SendInv.focusObj).val(tmplVal);
                SendInv.isChanged = true;
                SendInv.bindPHLabel(SendInv.focusObj.id, tmplVal);//$(this).remove();
            }
        }).live("mouseover", function () {
            $(this).find("i").attr('class', 'email-tmpl-ph-del-over');
        }).live("mouseout", function () {
            $(this).find("i").attr('class', 'email-tmpl-ph-del');
        });
        $("#aPreview").click(function () {
            SendInv.isPreview = !SendInv.isPreview;
            SendInv.previewIndex = 0;
            SendInv.previewTmpl(SendInv.isPreview);
        });
        $("#aPrevious, #aNext").click(function () {
            if (SendInv.previewTmplData && SendInv.previewTmplData.length > 1) {
                var isNext = this.id == "aNext";
                var total = SendInv.previewTmplData.length;
                if (isNext && SendInv.previewIndex < total - 1) {
                    SendInv.previewIndex++;
                }
                else if (!isNext && SendInv.previewIndex > 0) {
                    SendInv.previewIndex--;
                }
                SendInv.navgateTmplData();
            }
        });
        Megi.attachPropChangeEvent(document.getElementById("MsgSubject"), function () {
            SendInv.bindPHLabel(this.id, this.value);
            SendInv.isChanged = true;
        });
        Megi.attachPropChangeEvent(document.getElementById("MsgContent"), function () {
            SendInv.bindPHLabel(this.id, this.value);
            SendInv.isChanged = true;
        });
        Megi.attachPropChangeEvent(document.getElementById("txtTmplName"), function () {
            SendInv.isChanged = true;
        });
    },
    isSysTmpl: function () {
	    var isSys = $("#hidIsSys").val();
        return isSys == "1" || isSys.toLowerCase() == "true";
	},
    replaceIndex: function (str, regexStr, n) {
        var regex = new RegExp(regexStr, "g");
        var idx = 0;
        return str.replace(regex, function (match, i) {
            idx++;
            if (idx === n) return '';
            return match;
        });
    },
    checkTmplChange: function (actionType, callBack, cancelCallBack) {
        if (actionType == ActionType.ChangeTmpl) {
            //如果模板已改变，则提示
            if (SendInv.isChanged && !SendInv.isCancelAdd) {
                $.mDialog.confirm(HtmlLang.Write(LangModule.IV, "EmailTemplateChangedConfirm", "The Email template have unsaved data, are you continue anyway?"),
                {
                    callback: function () {
                        if (callBack) {
                            callBack();
                        }
                    },
                    cancelCallback: function () {
                        if (cancelCallBack) {
                            cancelCallBack();
                        }
                    }
                });
            }
            else if (callBack) {
                callBack();
            }
        }
        else if (actionType == ActionType.Send || actionType == ActionType.SaveRepeatTmpl) {
            //新增、拷贝、模板已改变，则自动保存
            if (!$("#EmailTemplate_value").combobox("getValue") || SendInv.isChanged && !SendInv.isCancelAdd) {
                SendInv.saveTmpl(callBack);
            }
            else if (callBack) {
                callBack();
            }
        }
    },
    previewTmpl: function (isPreview) {
        if (!isPreview) {
            $("#MsgSubject,#MsgContent").show();
            $("#previewMsgSubject,#previewMsgContent").hide();
            if (SendInv.isSysTmpl()) {
				$("#aInsertPlaceholder").hide();
				$("#divMsgSubject,#divMsgContent").hide();
			}
            else {
                $("#aInsertPlaceholder").show();
				$("#divMsgSubject,#divMsgContent").show();
			}
			$("#aPreview").text(HtmlLang.Write(LangModule.Common, "Preview", "Preview"));
            $("#divPreviewNav").hide();
            return;
        }
        var obj = SendInv.getParam();
        mAjax.submit("/IV/Invoice/PreviewEmailTmpl", { model: obj }, function (response) {
            if (response && response.length > 0) {
                SendInv.previewTmplData = response;
                SendInv.navgateTmplData(true);
            }
        });
    },
    navgateTmplData: function (isFirst) {        
        if (SendInv.previewTmplData && SendInv.previewTmplData.length > 0) {
            var total = SendInv.previewTmplData.length;
            if (isFirst) {
                if (total > 1) {
                    $("#divPreviewNav").show();
                    $("#aPrevious").linkbutton("disable");
                    $("#aNext").linkbutton("enable");
                }
                $("#aPreview").text(HtmlLang.Write(LangModule.Common, "EndPreview", "End Preview"));
                $("#MsgSubject,#MsgContent").hide();
                $("#divMsgSubject,#divMsgContent").hide();
                $("#previewMsgSubject,#previewMsgContent").show();
                $("#aInsertPlaceholder").hide();
            }

            $("#previewMsgSubject").html(SendInv.previewTmplData[SendInv.previewIndex].MSubjectPreview);
            $("#previewMsgContent").html(SendInv.previewTmplData[SendInv.previewIndex].MContentPreview);

            if (SendInv.previewIndex > 0) {
                $("#aPrevious").linkbutton("enable");
            }
            else {
                $("#aPrevious").linkbutton("disable");
            }
            if (SendInv.previewIndex < total - 1) {
                $("#aNext").linkbutton("enable");
            }
            else {
                $("#aNext").linkbutton("disable");
            }
        }
    },
    bindPHLabel: function (objId, tmplStr) {
        if (objId && tmplStr && SendInv.placeHolder) {
            var container = $("#div" + objId);
            container.html('');
            var regex = /(\[.*?\])/g;
            var matchedPHs = tmplStr.match(regex);
            if (matchedPHs) {
                var arrPHCount = matchedPHs.reduce(function (prev, cur) {
                    prev[cur] = (prev[cur] || 0) + 1;
                    return prev;
                }, {});
                var arrPHDisplayed = [];
                var phIdx = '';
                $.each(matchedPHs, function (i, matchedPH) {
                    $.each(SendInv.placeHolder, function (j, ph) {
                        if (matchedPH === ph.Key) {
                            var newPH = $("#aPH").clone();
                            newPH.show().removeAttr("id").appendTo(container);
                            if (parseInt(arrPHCount[matchedPH]) > 1) {
                                if (!arrPHDisplayed[matchedPH]) {
                                    arrPHDisplayed[matchedPH] = 0;
                                }
                                arrPHDisplayed[matchedPH] = parseInt(arrPHDisplayed[matchedPH]) + 1;
                                phIdx = "(" + arrPHDisplayed[matchedPH] + ")";
                            }
                            else {
                                phIdx = '';
                            }
                            newPH.find("em").attr("key", ph.Key).attr("title", ph.Value).text(ph.Key + phIdx);
                            return false;
                        }
                    });
                });
            }
            if (container.find("a").length > 0) {
                container.show();
            }
            else {
                container.hide();
            }
        }
    },
    saveTmpl: function (callBack) {
        if (!$(".required-field").mFormValidate()) {
            return;
        }
        var result = false;
        var obj = {};
        obj.MItemID = $("#EmailTemplate_value").combobox("getValue");
        obj.MName = $("#txtTmplName").val();
        obj.MSubject = $("#MsgSubject").val();
        obj.MContent = $("#MsgContent").val();
        obj.MType = SendInv.sendType;
        mAjax.post("/IV/Invoice/EmailTmplAdd", { model: obj }, function (response) {
            if (response.Success) {
                SendInv.isChanged = false;
                if (callBack != undefined) {
                    callBack();
                }
                else {
                    $.mMsg(LangKey.SaveSuccessfully);
                    $("#aNew, #aCopy").show();
                }
                result = true;
                SendInv.bindTmpl(response.ObjectID);
            }
            else {
                if (response) {
                    $.mDialog.alert(response.Message);
                } else {
                    $.mDialog.alert(response.Message);
                }
            }
        }, "", true);
        return result;
    },
    addTmpl: function (sender) {
        $("#aNew, #aCopy").hide();
        $("#divTmplName,#aCancelTmpl").show();
        var data = SendInv.emailTmpl;
        var newItem = null;
        if (sender.id == "aCopy") {
            var selectedItemId = $("#EmailTemplate_value").combobox("getValue");
            $.each(SendInv.emailTmpl, function (i, item) {
                if (item.MItemID === selectedItemId) {
                    newItem = { MItemID: '', MName: item.MName + '_' + HtmlLang.Write(LangModule.Common, "Copy", "Copy"), MSubject: item.MSubject, MContent: item.MContent };
                    return false;
                }
            });
        }
        else {
            var newName = HtmlLang.Write(LangModule.Common, "NewTemplate", "New Template");
            newItem = { MItemID: '', MName: newName, MSubject: '', MContent: '' };
        }
        data.push(newItem);
        $("#txtTmplName").val(newItem.MName).focus();
        $("#EmailTemplate_value").combobox({ data: data });
        $("#EmailTemplate_value").combobox("select", "");
        $("#EmailTemplate_value").combobox("disable");
    },
    resetTmpl: function () {
        var oriEmailTmp = [];
        var lastTmplId = SendInv.lastTmplId;
        $.each(SendInv.emailTmpl, function (i, item) {
            if (item.MItemID.length > 0) {
                oriEmailTmp.push(item);
            }
        });
        SendInv.emailTmpl = oriEmailTmp;
        $("#EmailTemplate_value").combobox({ data: SendInv.emailTmpl });
        if (!lastTmplId) {
            lastTmplId = SendInv.emailTmpl[0].MItemID;
        }
        $("#EmailTemplate_value").combobox("select", lastTmplId);
    },
    clearPHLabel: function () {
        $("#divMsgSubject, #divMsgContent").html('');
    },
    htmlDecode: function (str) {
        var div = document.getElementById("divHidden");
        div.innerHTML = str;
        return div.innerHTML;
    },
    changeTmpl: function (item) {
        SendInv.checkTmplChange(ActionType.ChangeTmpl, function () {
            SendInv.isChanged = false;
            if (SendInv.emailTmpl && item) {
                $.each(SendInv.emailTmpl, function (i, obj) {
                    if (obj.MItemID == item.MItemID) {
                        if (item.MItemID && item.MItemID != "") {
                            SendInv.lastTmplId = item.MItemID;
                        }
                        $("#hidIsSys").val(obj.MIsSys);
                        var subject = SendInv.htmlDecode(obj.MSubject);
                        var content = SendInv.htmlDecode(obj.MContent);
                        $("#MsgSubject").val(subject.replace(/&amp;/g, '&'));
                        $("#MsgContent").val(content.replace(/&amp;/g, '&'));
                        if (Megi.trim(subject) != "") {
                            $("#MsgSubject").removeClass("validatebox-invalid");
                        }
                        if (Megi.trim(content) != "") {
                            $("#MsgContent").removeClass("validatebox-invalid");
                        }
                        $("#aDelete").linkbutton("disable");
                        if (obj.MIsSys) {
                            $("#MsgSubject").attr("disabled", true);
                            $("#MsgContent").attr("disabled", true);
                            SendInv.clearPHLabel();
                            $("#txtTmplName").val(obj.MName);
                            $("#divTmplName").hide();
                            $("#aNew, #aCopy").show();
                            $("#aInsertPlaceholder").hide();
                        }
                        else {
                            if (obj.MItemID) {
                                $("#aDelete").linkbutton("enable");
                                var tmplName = $("#EmailTemplate_value").combobox("getText");
                                $("#txtTmplName").val(tmplName);
                                $("#divTmplName").show();
                                $("#aCancelTmpl").hide();
                            }
                            $("#MsgSubject").removeAttr("disabled");
                            $("#MsgContent").removeAttr("disabled");
                            SendInv.bindPHLabel("MsgSubject", obj.MSubject);
                            SendInv.bindPHLabel("MsgContent", obj.MContent);
                            if (!SendInv.isPreview) {
                                $("#aInsertPlaceholder").show();
                            }
                        }
                        if (SendInv.isPreview) {
                            SendInv.previewTmpl(true);
                        }
                        return false;
                    }
                });
            }
            else {
                $("#hidIsSys").val('');
                $("#MsgSubject").removeAttr("disabled").val('');
                $("#MsgContent").removeAttr("disabled").val('');
                $("#aDelete").linkbutton("disable");
                $("#aInsertPlaceholder").show();
                SendInv.clearPHLabel();
            }
        }, function () {
            $("#EmailTemplate_value").combobox("select", SendInv.prevTmplId);
            $("#popup_overlay, #popup_container").remove();
        });
    },
    bindTmpl: function (tmplId) {
        mAjax.post("/IV/Invoice/GetEmailTmplList", {
            sendType: SendInv.sendType
        }, function (msg) {
            SendInv.emailTmpl = msg;
            var opt = {};
            opt.data = msg;
            opt.onSelect = SendInv.changeTmpl;
            opt.valueField = "MItemID";
            opt.textField = "MName";
            opt.panelHeight = 200;
            opt.onShowPanel = function () {
                SendInv.prevTmplId = $(this).combobox("getValue");
                var $panel = $(this).data("combo").panel;
                var panelHeight = $(this).data("combo").options.panelHeight;
                $panel.height("inherit");
                if ($panel.outerHeight() >= panelHeight) {
                    $panel.outerHeight(panelHeight);
                }
            };
            $("#EmailTemplate_value").combobox(opt);
            var defaultSelect = msg[0].MItemID;
            if (tmplId) {
                defaultSelect = tmplId;
            }
            $("#EmailTemplate_value").combobox("select", defaultSelect);
        }, "", true);
    },
    deleteTmpl: function (id) {
        if (!id) {
            return;
        }
        $.mDialog.confirm(LangKey.AreYouSureToDelete, function () {
            mAjax.submit("/IV/Invoice/DeleteEmailTmpl", {
                id: id
            }, function (msg) {
                if (msg.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                    SendInv.bindTmpl();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            });
        });
    },
    getGridColumnList: function () {
        var arrColumnIndex;
        if (SendInv.sendType == EmailSendType.Invoice) {
            arrColumnIndex = [0, 1, 2, 3, 4, 5];
        }
        else if (SendInv.sendType == EmailSendType.Statement) {
            arrColumnIndex = [1, 2, 3, 5];
        }
        else if (SendInv.sendType == EmailSendType.RepeatingInvoice) {
            arrColumnIndex = [7, 1, 2, 3, 4, 5, 7, 8];
        }
        else if (SendInv.sendType == EmailSendType.Payslip) {
            arrColumnIndex = [1, 2, 3, 4, 5];
        }
        var cols = new Array();
        for (var i = 0; i < arrColumnIndex.length; i++) {
            cols.push(SendInv.columns[arrColumnIndex[i]]);
        }
        var result = new Array();
        result.push(cols);
        return result;
    },
    columns: [
            //0-发票号
            { title: HtmlLang.Write(LangModule.IV, "Invoice", "Invoice"), field: 'MInvNumber', width: 70, sortable: false },
            //1-联系人ID
            { title: "", field: 'MContactID', width: 0, hidden: true, sortable: false },
            //2-联系人姓名
            { title: ($("#sendType").val() == EmailSendType.Payslip ? HtmlLang.Write(LangModule.Common, "Employee", "Employee") : HtmlLang.Write(LangModule.IV, "To", "To")), field: 'MContactName', width: 80, sortable: false },
            //3-联系人Email
            {
                title: HtmlLang.Write(LangModule.IV, "Email", "Email"), field: 'MContactEmail', width: 130, sortable: false,
                formatter: function (value, rec, rowIndex) {
                    if (value == null || value == "null") {
                        value = "";
                    }
                    return "<input type='hidden' id='OriginalContactEmail' value='" + value + "' /><input type='text' class='easyui-validatebox email-text' data-options=\"required:true,validType:'email'\" style='width:99%;height:25px;line-height:25px;' value='" + value + "' />";
                }
            },
            //4-该发票是否已经发送过Email
            {
                title: '<input id="chkMarkAsSend" type="checkbox" >&nbsp;<label for="chkMarkAsSend">' + HtmlLang.Write(LangModule.IV, "MarkAsSent", "Mark as sent") + '</label>', width: 100, align: 'center', sortable: false,
                field: 'MIsSent', formatter: function (value, rec, rowIndex) {
                    if (SendInv.sendType == EmailSendType.RepeatingInvoice) {
                        if (rec.MIsSent) {
                            return "<input type='checkbox' class='row-key-checkbox' checked='true'>";
                        } else {
                            return "<input type='checkbox' class='row-key-checkbox'>";
                        }
                    }
                    else {
                        if ((SendInv.sendType == EmailSendType.Invoice && SendInv.status != 3 && SendInv.status != 4) || rec.MIsSent) {
                            return "<span class='icon-flag-done'></span>";
                        }
                        else {
                            return "<input type='checkbox' class='row-key-checkbox' checked='true'>";
                        }
                    }
                }
            },
            //5-
            //{ title: HtmlLang.Write(LangModule.IV, "MegiNetwork", "Megi Network"), field: 'MContactNetwork', width: 80, sortable: false },
            //6-删除
            {
                title: "", width: 35, align: 'center', sortable: false,
                field: 'MEntryID', formatter: function (value, rec, rowIndex) {
                    return "<a class=\"mg-icon-delete\" href=\"javascript:void(0)\" onclick=\"SendInv.DeleteItem(this)\">&nbsp;</a>";
                }
            },
            //7-金额
            {
                title: HtmlLang.Write(LangModule.IV, "Amount", "Amount"), field: 'MAmount', width: 70, sortable: false, formatter: function (value, rec, rowIndex) {
                    return Megi.Math.toMoneyFormat(Math.abs(value), 2);
                }
            },
            //8-
            { title: "", field: 'MIsIncludePDFAttachment', width: 0, hidden: true, sortable: false },
            //9-
            { title: "", field: 'MIsSendMeACopy', width: 0, hidden: true, sortable: false },
            //10-工资号
            { title: HtmlLang.Write(LangModule.PA, "Payslip", "Payslip"), field: 'MInvNumber', width: 70, sortable: false }
        ],
}
//初始化页面
$(document).ready(function () {
    SendInv.init();
});