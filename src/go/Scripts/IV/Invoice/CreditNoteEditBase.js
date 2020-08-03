/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var CreditNoteEditBase = {
    InvoiceID: $("#hidInvoiceID").val(),
    //发票模型
    InvoiceModel: null,
    CurrentStatus: 1,
    InvoiceType: '',
    ListUrl: "/IV/Invoice/InvoiceList",
    EditUrl: "/IV/Invoice/CreditNoteEdit",
    ViewUrl: "/IV/Invoice/CreditNoteView",
    IsInit: false,
    IsEdit: true,
    IsFirstLoad: false,
    //初始化
    init: function () {
        CreditNoteEditBase.IsFirstLoad = true;
        //解决手动修改日期，到期日没跟着变问题
        $("#txtDate").datebox({
            onChange: function (newValue, oldValue) {
                if ($("#txtDate").datebox("panel").is(":visible")) {
                    return;
                }
                if (mDate.isDateString(newValue)) {
                    CreditNoteEditBase.changeDate();
                }
            },
            onSelect: function (date) {
                CreditNoteEditBase.changeDate();
            }
        });
        //获取传入的联系人信息
        var msg = mText.getObject("#hidInvoiceModel");

        //联系人ID, 如果联系人为空，从url获取，用来设置默认值
        var contactId = msg.MContactID;
        contactId = (msg.MContactID == null || msg.MContactID == "") ? Megi.getUrlParam("contactId") : contactId;

        CreditNoteEditBase.initContactList(contactId, function () {
            CreditNoteEditBase.getModel();
            CreditNoteEditBase.IsFirstLoad = false;
        });


        //联系人页面的跳转 点击事件
        $(".m-icon-customer").click(function () {
            //找到下拉框的对象
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
            onSelect: CreditNoteEditBase.changeContact,
            queryParams: { v: contactId },
            onLoadSuccess: function () {
                if (CreditNoteEditBase.IsFirstLoad && callback && $.isFunction(callback)) {
                    callback();
                }
            }
        },
         {
             url: "/BD/Contacts/ContactsEdit?contactType=1",
             //是否有联系人编辑权限
             hasPermission: $("#hidIsCanContactChangePermission").val() == "1",
             //弹出框关闭后的回调函数
             callback: CreditNoteEditBase.afterAddContact
         });
    },
    saveInvoice: function (callback) {
        CreditNoteEditBase.saveModel(CreditNoteEditBase.CurrentStatus, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    saveInvoiceAsDraft: function (callback) {
        CreditNoteEditBase.saveModel(IVBase.Status.Draft, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    afterAddContact: function (param) {
        if (param) {
            $('#selContact').combobox('setValue', param);
        }
    },
    saveInvoiceAndSubmitForApproval: function (callback) {
        CreditNoteEditBase.saveModel(IVBase.Status.WaitingApproval, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    saveInvoiceAndApprove: function (callback) {
        CreditNoteEditBase.saveModel(IVBase.Status.AwaitingPayment, function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    printInvoice: function (billType) {
        IVEditBase.OpenPrintDialog(CreditNoteEditBase.InvoiceID, billType);
    },
    emailInvoice: function () {
        IVEditBase.emailInvoice(CreditNoteEditBase.InvoiceID, CreditNoteEditBase.InvoiceType, CreditNoteEditBase.CurrentStatus, $("#txtInvoiceNo").val());
    },
    viewHistory: function () {
        IVEditBase.OpenHistoryDialog(CreditNoteEditBase.InvoiceID, "History", "Invoice_Sale_Red");
    },
    changeContact: function (rec) {
        var value = rec.MItemID;
        IVEditBase.changeContact("Sale", value, function (msg) {
            CreditNoteEditBase.setDefaultDueDate();
        });
    },
    setDefaultDueDate: function () {
        if (CreditNoteEditBase.IsInit) {
            $('#txtDueDate').datebox('setValue', $('#hidDefaultBizDate').val());
        } else {
            var billDate = $('#txtDate').datebox('getValue');
            var value = IVEditBase.getDefaultDueDate(billDate);
            $('#txtDueDate').datebox('setValue', value);
        }
    },
    changeDate: function () {
        CreditNoteEditBase.setDefaultDueDate();
        var billDate = $('#txtDate').datebox('getValue');
        if (!CreditNoteEditBase.IsFirstLoad) {
            IVEditBase.resetCurrency(billDate);
        }
    },
    //初始化发票信息
    getModel: function () {
        //发票信息模型
        var msg = mText.getObject("#hidInvoiceModel");
        CreditNoteEditBase.InvoiceModel = msg;

        //联系人ID, 如果联系人为空，从url获取，用来设置默认值
        var contactId = msg.MContactID;
        contactId = (msg.MContactID == null || msg.MContactID == "") ? Megi.getUrlParam("contactId") : contactId;

        //发票号
        $("#txtInvoiceNo").val(msg.MNumber);
        //录入日期
        if (CreditNoteEditBase.InvoiceID != "") {
            //如果 InvoiceID 不为空，则说明有录入日期
            $('#txtDate').datebox('setText', $.mDate.format(msg.MBizDate));
        }

        $("#txtRef").val(msg.MReference);
        if (msg.MBranding != null) {
            $('#selBranding').combobox('setValue', msg.MBranding);
        }
        $("#txtInvoiceStatus").val(IVBase.GetStatus(msg.MStatus));

        CreditNoteEditBase.CurrentStatus = msg.MStatus;
        CreditNoteEditBase.InvoiceType = msg.MType;
        var isDisabled = $("#hidIsEdit").val() == "True" ? false : true;
        IVEditBase.init({
            IVType: IVEditBase.IVType.Sale, //类型，主要是判断是销售还是采购
            id: CreditNoteEditBase.InvoiceID, //单据ID
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
            bizDate: $('#txtDate').datebox('getValue')   //单据日期
        });

        if ((msg.MID == null || msg.MID == "") && contactId != null && contactId != "" && Megi.request("cpyId") == "") {
            IVEditBase.GetContactInfo("Sale", contactId, function () {
                $('#selContact').combobox('select', contactId);
                //将当前页面设置为稳定状态
                $.mTab.setStable();
            });
        } else {
            $('#selContact').combobox('setValue', msg.MContactID);
        }
        CreditNoteEditBase.IsShowVerifConfirm(CreditNoteEditBase.InvoiceID);
        //将当前页面设置为稳定状态
        $.mTab.setStable();
    },
    IsShowVerifConfirm: function (invoiceId) {
        var isShowVerif = ($("#hidIsShowVerif").val() == "True" && Megi.request("sv") == "1") ? true : false;
        if (invoiceId != "" && isShowVerif) {
            $.mAjax.post("/IV/IVInvoiceBase/GetVerificationById", { invoiceId: invoiceId }, function (data) {
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
                            Verification.open(invoiceId, "Invoice", CreditNoteEditBase.afterVerification);
                        }
                    }, "", true);
                }
            });
        }
    },
    afterVerification: function () {
        var url = CreditNoteEditBase.ListUrl + "?id=5";
        $.mTab.addOrUpdate("", url, false, false);
    },
    saveModel: function (status, callback) {
        var bizDate = $('#txtDate').datebox('getValue');
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

        var invoiceId = $("#hidInvoiceID").val();
        var contactId = $('#selContact').combobox('getValue');
        var number = $("#txtInvoiceNo").val();
        var ref = $("#txtRef").val();
        var branding = $("#selBranding").combobox('getValue');
        var isCopyCredit = $("#hidIsCopyCredit").val() == "True" ? true : false;
        var invCopyID = $("#hidInvCopyID").val();

        var obj = {};
        obj.MID = invoiceId;
        obj.MContactID = contactId;
        obj.MBizDate = bizDate;
        obj.MNumber = number;
        obj.MType = IVBase.InvoiceType.InvoiceSaleRed;
        obj.MStatus = status;
        obj.MReference = ref;
        obj.MBranding = branding;
        obj.MIsCopyCredit = isCopyCredit;
        obj.MInvCopyID = invCopyID;
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
            if (!CreditNoteEditBase.InvoiceID && typeof (AssociateFiles) != undefined && typeof (AssociateFiles) != 'undefined') {
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
    CreditNoteEditBase.init();
});