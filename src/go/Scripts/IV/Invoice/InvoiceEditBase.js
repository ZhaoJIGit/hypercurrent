/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var InvoiceEditBase = {
    InvoiceID: $("#hidInvoiceID").val(),
    //发票模型
    InvoiceModel: null,
    InvoiceType: '',
    CurrentStatus: 1,
    ListUrl: "/IV/Invoice/InvoiceList",
    EditUrl: "/IV/Invoice/InvoiceEdit",
    InitEditUrl: "/IV/Invoice/InvoiceInitEdit",
    ViewUrl: "/IV/Invoice/InvoiceView",
    IsInit: false,
    IsEdit: true,
    IsFirstLoad: false,
    //初始化
    init: function () {
        InvoiceEditBase.IsFirstLoad = true;
        //解决手动修改日期，到期日没跟着变问题
        $("#txtDate").datebox({
            onChange: function (newValue, oldValue) {
                if ($("#txtDate").datebox("panel").is(":visible")) {
                    return;
                }
                if (mDate.isDateString(newValue)) {
                    InvoiceEditBase.changeDate();
                }
            },
            onSelect: function (date) {
                InvoiceEditBase.changeDate();
            }
        });

        var msg = mText.getObject("#hidInvoiceModel");

        //联系人ID, 如果联系人为空，从url获取，用来设置默认值
        var contactId = msg.MContactID;
        contactId = (msg.MContactID == null || msg.MContactID == "") ? Megi.getUrlParam("contactId") : contactId;

        InvoiceEditBase.initContactList(contactId, function () {
            InvoiceEditBase.getModel();
            InvoiceEditBase.IsFirstLoad = false;
        });


        $(".m-icon-customer").click(function () {
            var contactDom = $("#selContact");

            var contactId = contactDom.combobox("getValue");

            if (!contactId) {
                return;
            }
            $.mTab.addOrUpdate(HtmlLang.Write(LangModule.Contact, "ViewContact", "View Contact"), '/BD/Contacts/ContactView/' + contactId);
        });

    },

    initContactList: function (contactId, callback) {
        $("#selContact").mAddCombobox("contact", {
            url: "/BD/Contacts/GetContactsListByContactType?contactType=1&IncludeDisable=true",
            valueField: 'MItemID',
            textField: 'MContactName',
            hideItemKey: 'MIsActive',
            hideItemValue: false,
            mode: "remote",
            onSelect: InvoiceEditBase.changeContact,
            queryParams: { v: contactId },
            onLoadSuccess: function () {
                if (InvoiceEditBase.IsFirstLoad && callback && $.isFunction(callback)) {
                    callback();
                }
            }
        },
         {
             url: "/BD/Contacts/ContactsEdit?contactType=1",
             //是否有联系人编辑权限
             hasPermission: $("#hidIsCanContactChangePermission").val() == "1",
             //弹出框关闭后的回调函数
             callback: InvoiceEditBase.afterAddContact
         });
    },
    saveInvoice: function (callback) {
        InvoiceEditBase.saveModel(InvoiceEditBase.CurrentStatus, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    saveInvoiceAsDraft: function (callback) {
        InvoiceEditBase.saveModel(IVBase.Status.Draft, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    saveInvoiceAndSubmitForApproval: function (callback) {
        InvoiceEditBase.saveModel(IVBase.Status.WaitingApproval, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    saveInvoiceAndApprove: function (callback) {
        InvoiceEditBase.saveModel(IVBase.Status.AwaitingPayment, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    printInvoice: function (billType) {
        IVEditBase.OpenPrintDialog(InvoiceEditBase.InvoiceID, billType);
    },
    emailInvoice: function () {
        IVEditBase.emailInvoice(InvoiceEditBase.InvoiceID, InvoiceEditBase.InvoiceType, InvoiceEditBase.CurrentStatus, $("#txtInvoiceNo").val());
    },
    viewHistory: function () {
        IVEditBase.OpenHistoryDialog(InvoiceEditBase.InvoiceID, "History", "Invoice_Sale");
    },
    changeContact: function (rec) {
        if (!rec) {
            return;
        }
        var value = rec.MItemID;
        IVEditBase.changeContact("Sale", value, function (msg) {
            InvoiceEditBase.setDefaultDueDate();
        });
    },
    setDefaultDueDate: function () {
        if (InvoiceEditBase.IsInit) {
            var defaultBizDate = $('#hidDefaultBizDate').val();
            if (defaultBizDate) {
                var defaultDate = new Date(defaultBizDate).addMonths(1);
                var nextMonthFirstDay = new Date(defaultDate.getFullYear(), defaultDate.getMonth()+1, 1);
                var oneDay = 1000 * 60 * 60 * 24;
                $('#txtDueDate').datebox('setValue', nextMonthFirstDay - oneDay);
            }
        } else {
            var billDate = $('#txtDate').datebox('getValue');
            var value = IVEditBase.getDefaultDueDate(billDate);
            $('#txtDueDate').datebox('setValue', value);
        }

    },
    afterAddContact: function (param) {
        if (param) {
            $('#selContact').combobox('setValue', param);
            $.mTab.setStable();
        }
    },
    changeDate: function () {
        var billDate = $('#txtDate').datebox('getValue');
        InvoiceEditBase.setDefaultDueDate();
        var billDate = $('#txtDate').datebox('getValue');
        if (!InvoiceEditBase.IsFirstLoad) {
            IVEditBase.resetCurrency(billDate);
        }
    },
    afterVerification: function () {
        var url = InvoiceEditBase.ListUrl + "?id=5";
        $.mTab.refresh("", url, false, true);
    },
    //初始化发票信息
    getModel: function () {
        //发票信息模型
        var msg = mText.getObject("#hidInvoiceModel");
        InvoiceEditBase.InvoiceModel = msg;
        //联系人ID, 如果联系人为空，从url获取，用来设置默认值
        var contactId = msg.MContactID;
        contactId = (msg.MContactID == null || msg.MContactID == "") ? Megi.getUrlParam("contactId") : contactId;
        //发票号
        $("#txtInvoiceNo").val(msg.MNumber);
        //录入日期
        if (InvoiceEditBase.InvoiceID != "") {
            //如果 InvoiceID 不为空，则说明有录入日期
            $('#txtDate').datebox('setText', $.mDate.format(msg.MBizDate));
            $('#txtDueDate').datebox('setValue', $.mDate.format(msg.MDueDate));
        } else {
            //默认Due日期
            InvoiceEditBase.setDefaultDueDate();
        }

        $('#txtExpectedDate').datebox('setValue', $.mDate.format(msg.MExpectedDate));

        $("#txtRef").val(msg.MReference);
        if (msg.MBranding != null) {
            $('#selBranding').combobox('setValue', msg.MBranding);
        }
        $("#txtInvoiceStatus").val(IVBase.GetStatus(msg.MStatus));

        InvoiceEditBase.CurrentStatus = msg.MStatus;
        InvoiceEditBase.InvoiceType = msg.MType;
        var isDisabled = $("#hidIsEdit").val() == "True" ? false : true;
        IVEditBase.init({
            IVType: IVEditBase.IVType.Sale, //类型，主要是判断是销售还是采购
            id: InvoiceEditBase.InvoiceID, //单据ID
            oToLRate: msg.MOToLRate,  //原币到本位币的汇率
            lToORate: msg.MLToORate, //本位币到原币的汇率
            verificationList: msg.Verification, //核销列表
            entryList: msg.InvoiceEntry, //分录列表
            cyId: msg.MCyID, //币别
            taxId: msg.MTaxID, //税率类型
            disabled: isDisabled, //是否禁止编辑
            currencyDisabled: false, //币别是否允许编辑
            type: msg.MType, //单据类型
            status: msg.MStatus, //单据状态
            bizDate: $('#txtDate').datebox('getValue')  //单据日期
        });

        if ((msg.MID == null || msg.MID == "") && contactId != null && contactId != "" && Megi.request("cpyId")=="") {
            IVEditBase.GetContactInfo("Sale", contactId, function () {
                $('#selContact').combobox('select', contactId);
                $.mTab.setStable();
            });
        } else {
            $('#selContact').combobox('setValue', msg.MContactID);
        }
        InvoiceEditBase.IsShowVerifConfirm(InvoiceEditBase.InvoiceID);
        //将当前页面设置为稳定状态
        $.mTab.setStable();
    },
    IsShowVerifConfirm: function (invoiceId) {
        var isShowVerif = ($("#hidIsShowVerif").val() == "True" && Megi.request("sv") == "1") ? true : false;
        if (invoiceId != "" && isShowVerif) {
            mAjax.post("/IV/IVInvoiceBase/GetVerificationById", { invoiceId: invoiceId }, function (data) {
                if (data != null && data.length > 0) {
                    var currency = data[0].MCurrencyID;
                    var totalAmount = 0;
                    $(data).each(function (i) {
                        totalAmount += data[i].Amount;
                    });
                    var title = HtmlLang.Write(LangModule.IV, "ThereIs", " There is") + " " + "<b>" +
                                Megi.Math.toMoneyFormat(totalAmount, 2) + " " + currency + "</b>" + " " +
                                HtmlLang.Write(LangModule.IV, "InOutstandingCredit", "in outstanding credit.") + " " +
                                HtmlLang.Write(LangModule.IV, "AllocatecreditToInvoice", "Would you like to allocate credit to this invoice?");
                    $.mDialog.confirm(title, function (sure) {
                        if (sure) {
                            Verification.open(invoiceId, "Invoice", InvoiceEditBase.afterVerification);
                        }
                    }, "", true);
                }
            });
        }
    },
    saveModel: function (status, callback) {
        var bizDate = $('#txtDate').datebox('getValue');
        var dueDate = $('#txtDueDate').datebox('getValue');
        
        if (!IVEditBase.verifyExchangeRate(bizDate)) {
            return;
        }
        if (status < 1) {
            status = 1;
        }
        var entryInfo = IVEditBase.getInfo();
        entryResult = IVEditBase.valideInfo(IVEditBase.IVType.Sale);
        var result = $(".m-form-icon").mFormValidate();
        if (!result || !entryResult) {
            return;
        }

        var contactId = $("#selContact").combobox("getValue");
        var contactName = $("#selContact").combobox("getText")
        if (!contactId || contactId == contactName) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "ContatNotExist", "联系人不存在，请重新选择联系人"));
            return;
        }

        //到期日期必须大于单据录入日期
        if (dueDate < bizDate) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "DueDateIsGreaterThanTheDate", "Due date must be greater than or equal to the date."));
            return;
        }
        //预计付款日期必须大于等于单据录入日期
        var expectedDate = $('#txtExpectedDate').datebox('getValue');
        if (expectedDate) {
            //如果选择预计收款日期才需要验证
            if (expectedDate < bizDate) {
                $.mDialog.alert(HtmlLang.Write(LangModule.IV, "ReceiptDateGreaterThanDate", "Expected payment date must be greater than or equal to invoice issued date."));
                return;
            }
        }
        //至少要有一条明细
        if (entryInfo.InvoiceEntry == null || entryInfo.InvoiceEntry.length < 1) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "AtLeastOneLineItem", "You must have at least 1 line item."));
            return;
        }

        //汇总金额必须大于0
        var total = parseFloat($("#spTotal").text());
        if (!total || total <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "TotalAmountNotToZero", "Aggregate amount must be greater than zero."));
            return;
        }

        //单条税额不能大于单条总金额
        for (var i = 0; i < entryInfo.InvoiceEntry.length; i++) {
            var item = entryInfo.InvoiceEntry[i];
            var check = Megi.Math.toDecimal(parseFloat(item.MTaxAmtFor), 2) - Megi.Math.toDecimal(parseFloat(item.MAmountFor), 2);
            if (check>0) {
                var message = HtmlLang.Write(LangModule.IV, "CannotGreaterAmount", "The tax amount cannot be greater than the total amount");
                $.mDialog.alert(message);
                return;
            }
        }

        var invoiceId = $("#hidInvoiceID").val();
        var contactId = $('#selContact').combobox('getValue');
        var number = $("#txtInvoiceNo").val();
        var ref = $("#txtRef").val();
        var branding = $("#selBranding").combobox('getValue');

        var obj = {};
        obj.MID = invoiceId;
        obj.MContactID = contactId;
        obj.MBizDate = bizDate;
        obj.MDueDate = dueDate;
        obj.MExpectedDate = expectedDate;
        obj.MNumber = number;
        obj.MType = IVBase.InvoiceType.Sale;
        obj.MStatus = status;
        obj.MReference = ref;
        obj.MBranding = branding;
        var dbModel = mText.getObject("#hidInvoiceModel");
        obj.MCurrentAccountCode = dbModel.MCurrentAccountCode;


        obj.MTotalAmtFor = entryInfo.MTotalAmtFor;
        obj.MTotalAmt = entryInfo.MTotalAmt;
        obj.MTaxTotalAmtFor = entryInfo.MTaxTotalAmtFor;
        obj.MTaxTotalAmt = entryInfo.MTaxTotalAmt;
        obj.MTaxID = entryInfo.MTaxID;
        obj.MCyID = entryInfo.MCyID;
        obj.MExchangeRate = entryInfo.MExchangeRate;
        obj.MOToLRate = entryInfo.MOToLRate;
        obj.MLToORate = entryInfo.MLToORate;
        obj.InvoiceEntry = entryInfo.InvoiceEntry;

        IVBase.saveInvoice(obj, function (msg) {
            if (msg.Code == "1000") {
                $.mDialog.alert(msg.Message, {
                    callback: function () {
                        mWindow.reload();
                    }
                });
                return;
            } 

            if (!InvoiceEditBase.InvoiceID && typeof (AssociateFiles) != undefined && typeof (AssociateFiles) != 'undefined') {
                AssociateFiles.associateFilesTo($("#hidBizObject").val(), msg.ObjectID, undefined, function () {
                    if (callback != undefined) {
                        callback(msg);
                    }
                });
            }
            else if (callback != undefined) {
                callback(msg);
            }
        });
    }
}

$(document).ready(function () {
    InvoiceEditBase.init();
});