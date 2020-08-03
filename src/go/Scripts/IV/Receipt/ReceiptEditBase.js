/// <reference path="../IVEditOptBase.js" />
/// <reference path="../IVBase.js" 
/// <reference path="../IVEditBase.js" />
var ReceiptEditBase = {
    ReceiptID: $("#hidReceiptID").val(),
    ReceiptModel: null,
    CurrentStatus: 1,
    url_delete: "/IV/Receipt/DeleteReceiveList",
    BankID: $("#hidAccountID").val(),
    BankName: $("#hidBankName").val(),
    IsFirstLoad: false,
    //初始化
    init: function () {

        $("#txtDate").datebox({
            onChange: function (newValue, oldValue) {
                if ($("#txtDate").datebox("panel").is(":visible")) {
                    return;
                }
                if (mDate.isDateString(newValue)) {
                    ReceiptEditBase.changeDate();
                }
            },
            onSelect: function (date) {
                ReceiptEditBase.changeDate();
            }
        });

        ReceiptEditBase.IsFirstLoad = true;
        ReceiptEditBase.initReceiptModel();
        ReceiptEditBase.initOperation();
        //选中银行账号
        var bankId = $.trim($("#hidAccountID").val());
        if (bankId == "") {
            if (ReceiptEditBase.ReceiptModel) {
                bankId = ReceiptEditBase.ReceiptModel.MBankID;
            }
        }
        $('#selBankAcctID').combobox('setValue', bankId);
        ReceiptEditBase.BankID = bankId;
        ReceiptEditBase.initCombobox();
        ReceiptEditBase.IsFirstLoad = false;
    },
    //初始化操作
    initOperation: function () {
        //退款
        $("#aRefund").click(function () {
            var promptMessage = HtmlLang.Write(LangModule.Bank, "SwitchToTheRefundBefore", "By switching to refund, you may lose some or all data on this page. Are you sure you want to switch?");
            $.mDialog.confirm(promptMessage, function (sure) {
                if (sure) {
                    if ($.trim($("#hidShowAdvance").val()) != "") {
                        $.mTab.rename(HtmlLang.Write(LangModule.Bank, "NewRefund", "New Refund"));
                    }
                    mWindow.reload("/IV/Receipt/ReceiptEdit?acctid=" + $("#hidAccountID").val() + "&showBnkAcct=" + $("#hidShowBnkAcct").val() + "&paymentType=Receive_SaleReturn");
                }
            });
        });
    },
    afterAddContact: function (param) {
        if (param) {
            $('#selContact').combobox('setValue', param);
        }
    },
    //支付类别
    changePaymentType: function () {
        IVEditBase.removeDataSourceEntryItem();
        var contactType = $('#selContactType').combobox('getValue');
        IVEditBase.changeBizBillType(contactType);
    },
    changeBankAccount: function (rec) {
        $("#hidReceiptCurrencyID").val(rec.currency);
        $("#hidAccountID").val(rec.id);
        $("#hidBankName").val(rec.bankName);
        ReceiptEditBase.BankName = rec.bankName;
        IVEditBase.selectCurrency(rec.currency);
    },
    changeDate: function () {
        var billDate = $('#txtDate').datebox('getValue');
        if (!ReceiptEditBase.IsFirstLoad) {
            IVEditBase.resetCurrency(billDate);
        }
    },
    //查看银行勾对
    showBankReconcileView: function (billType, billId) {

        Megi.dialog({
            title: HtmlLang.Write(LangModule.Bank, "BankReconcileView", "BankReconcileView"),
            width: 900,
            height: 450,
            href: "/BD/BDBank/BDBankReconcileView?billType=" + billType + "&billId=" + billId,
        });

    },
    //删除银行勾对记录
    deleteBankReconcile: function (billType, billId, bankId) {
        $.mDialog.confirm(HtmlLang.Write(LangModule.Bank, "ReconcileDeleteTip", "This action will cancel all the reconciliation records with this business transaction, are you sure want to cancel it?"),
        {
            callback: function () {
                mAjax.submit(
                    "/BD/BDBank/DeleteBankReconcile?billType=" + billType + "&billId=" + billId,
                    {},
                    function (msg) {
                        if (msg.Success) {
                            var message = HtmlLang.Write(LangModule.Bank, "DeleteSuccessfully", "Delete Successfully!");
                            $.mMsg(message);
                            //刷新页面
                            location.reload();
                            BDBank.refreshTransPage(bankId, ReceiptEditBase.BankName, true);
                        } else {
                            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Deletefailed"));
                        }
                    });
            }
        });
    },
    UpdateReconcileStatu: function (receiveId, statu, bankId) {
        mAjax.submit(
            "/IV/Receipt/UpdateReconcileStatu", { receiveId: receiveId, statu: statu },
            function (msg) {
                if (msg.Success) {
                    if (statu == 204) {
                        var message = HtmlLang.Write(LangModule.Bank, "MarkAsReconciledSuccess", "Mark as reconciled successful!");
                    } else {
                        var message = HtmlLang.Write(LangModule.Bank, "MarkAsUnReconciledSuccess", "Mark as unreconciled successful!");
                    }
                    $.mMsg(message);
                    BDBank.refreshTransPage(bankId, ReceiptEditBase.BankName, true);
                    location.reload();
                } else {
                    $.mDialog.alert(msg.Message, function () {
                        mWindow.reload();
                    });
                }
            });
    },
    showOrHideIsAdvances: function () {
        var contactType = $('#selContactType').combobox('getValue');
        if (contactType == "Employees") {
            //隐藏供应商，显示员工
            $("#selContact").combobox('setValue', "");
            $("#divContact").hide();
            $("#divEmployee").show();
        } else {
            //显示供应商，隐藏员工
            $("#divContact").show();
            $("#divEmployee").hide();
            ReceiptEditBase.loadContact(false);
        }
    },
    //联系人类型
    changeContactType: function () {
        //是否显示“借支”
        ReceiptEditBase.showOrHideIsAdvances();
        //是否显示“部门”和“税率”
        IVEditBase.changeContactType();
    },
    //选择客户时，设置明细列表的一些列表自动填充（收款单 和 采购发票 一致）
    changeContact: function (rec) {
        if (!rec) {
            return;
        }
        var value = rec.MItemID;
        var type2 = "Pur";
        var contactType = $('#selContactType').combobox('getValue');
        if (contactType == "Supplier") {
            type2 = "Pur";
        } else if (contactType == "Customer") {
            type2 = "Sale";
        }
        IVEditBase.changeContact(type2, value, function (msg) {
        });
    },
    //加载联系人下拉框数据
    loadContact: function (isInit) {
        var contactTypeValue = 1;
        var contactType = $('#selContactType').combobox('getValue');
        if (contactType == "Customer") {
            contactTypeValue = 1;
        } else if (contactType == "Supplier") {
            contactTypeValue = 2;
        } else if (contactType == "Other") {
            contactTypeValue = 4;
        }
        $('#selContact').mAddCombobox("contact",
            {
                valueField: 'MItemID',
                textField: 'MContactName',
                hideItemKey: 'MIsActive',
                hideItemValue: false,
                mode: "remote",
                url: '/BD/Contacts/GetContactsListByContactType?contactType=' + contactTypeValue + "&IncludeDisable=true",
                //数据加载成功后更新数据源
                onLoadSuccess: function (msg) {
                    //选中客户
                    if (isInit) {
                        if (ReceiptEditBase.ReceiptModel && ReceiptEditBase.ReceiptModel.MContactID) {
                            $('#selContact').combobox('setValue', ReceiptEditBase.ReceiptModel.MContactID);
                        }
                        //将当前页面设置为稳定状态
                        $.mTab.setStable();
                    }
                },
                onSelect: function (rec) {
                    ReceiptEditBase.changeContact(rec);
                }
            },
            {
                url: "/BD/Contacts/ContactsEdit?contactType=" + contactTypeValue,
                //是否有联系人编辑权限
                hasPermission: IVEditBase.IsCanContactChangePermission,
                //弹出框关闭后的回调函数
                callback: function (param) {
                    if (param) {
                        $('#selContact').combobox('setValue', param);
                    }
                }
            }
        );
    },
    //初始化下拉框
    initCombobox: function () {
        //联系人下拉框
        ReceiptEditBase.loadContact(true);

        //员工下拉框
        $('#selEmployee').mAddCombobox("employee",
            {
                //数据加载成功后更新数据源
                onLoadSuccess: function (msg) {
                    //提示文字
                    $("#selEmployee").initHint();
                    //选中员工
                    if (ReceiptEditBase.ReceiptModel && ReceiptEditBase.ReceiptModel.MContactID) {
                        $('#selEmployee').combobox('setValue', ReceiptEditBase.ReceiptModel.MContactID);
                        //将当前页面设置为稳定状态
                        $.mTab.setStable();
                    }
                }
            },
            {
                //是否有联系人编辑权限
                hasPermission: IVEditBase.IsCanSettingChangePermission,
                //弹出框关闭后的回调函数
                callback: function (param) {
                    if (param) {
                        $('#selEmployee').combobox('setValue', param);
                    }
                }
            }
        );
    },
    //获取tab选项卡标题
    getTabTitle: function () {
        var ref = $.trim($("#txtRef").val());
        return ref != "" ? $.mIV.getTitle(mTitle_Pre_Receive, ref) : HtmlLang.Write(LangModule.Bank, "EditReceipt", "Edit Receipt");
    },
    //收款单删除（加删除标记）
    deleteItem: function (id, bankId) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete,
        {
            callback: function () {
                var param = {};
                param.KeyIDs = id;
                mAjax.submit(
                    ReceiptEditBase.url_delete,
                    { param: param },
                    function (msg) {
                        if (msg.Success) {
                            var message = HtmlLang.Write(LangModule.Bank, "DeleteSuccessfully", "Delete Successfully!");
                            $.mMsg(message);
                            BDBank.refreshTransPage(bankId, ReceiptEditBase.BankName, false, function () {
                                $.mTab.remove();
                            }, function () {
                                $.mDialog.close();
                            });
                        } else {
                            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Have cancel after verification, not allowed to delete"), function () {
                                window.location = window.location;
                            });
                        }
                    });
            }
        });
    },
    //初始化收款单模型
    initReceiptModel: function () {
        var msg = mText.getObject("#hidReceiptModel");
        ReceiptEditBase.ReceiptModel = msg;

        //收款类型：销售收款 或者 销售退款
        if (msg.MType == "Receive_SaleReturn" || msg.MType == "Receive_OtherReturn") {
            $("#divPageTitle").text(HtmlLang.Write(LangModule.IV, "Refund", "Refund"));
            $("#divRefund").hide();
        } else {
            $("#divPageTitle").text(HtmlLang.Write(LangModule.IV, "ReceiveMoney", "Receive Money"));
            $("#divRefund").show();
        }
        if (msg.MType != null) {
            $('#selType').combobox('setValue', msg.MType);
        }
        //业务日期
        if (ReceiptEditBase.ReceiptID != "") {
            $('#txtDate').datebox('setText', $.mDate.format(msg.MBizDate));
        }
        //联系人类型
        if (msg.MContactType != null) {
            $('#selContactType').combobox('setValue', msg.MContactType);
        }
        //摘要
        $("#txtRef").val(msg.MReference);
        //银行汇率ID
        var cyId = $("#hidReceiptCurrencyID").val();

        //初始化收款单明细列表控件
        var isDisabled = $("#hidIsEdit").val() == "True" ? false : true;
        var contactType = $('#selContactType').combobox('getValue');
        IVEditBase.ContactType = contactType;

        var type1 = IVEditBase.IVType.Purchase;
        var type2 = "Pur";
        if (contactType == "Supplier") {
            type1 = IVEditBase.IVType.Purchase;
            type2 = "Pur";
        } else if (contactType == "Customer") {
            type1 = IVEditBase.IVType.Sale;
            type2 = "Sale";
        }


        IVEditBase.init({
            IVType: type1, //类型，主要是判断是销售还是采购
            id: ReceiptEditBase.ReceiptID, //单据ID
            oToLRate: msg.MOToLRate,  //原币到本位币的汇率
            lToORate: msg.MLToORate, //本位币到原币的汇率
            verificationList: msg.Verification, //核销列表
            entryList: msg.ReceiveEntry, //分录列表
            cyId: cyId, //币别
            taxId: msg.MTaxID, //税率类型
            disabled: isDisabled, //是否禁止编辑
            currencyDisabled: true, //币别是否允许编辑
            type: msg.MType, //单据类型
            status: null, //单据状态
            bizDate: $('#txtDate').datebox('getValue'),  //单据日期
            callback: function () {
                IVEditBase.changeBizBillType(msg.MContactType);
            }
        });

        if (msg.MContactID != null && msg.MContactID != "") {
            if (!msg.MContactType || msg.MContactType == "Employees") {
                //隐藏供应商，显示员工
                $("#divContact").hide();
                $("#divEmployee").show();
                //将当前页面设置为稳定状态
                $.mTab.setStable();
            } else {
                IVEditBase.GetContactInfo(type2, msg.MContactID, function (msg) {
                    //显示供应商，隐藏员工
                    $("#divContact").show();
                    $("#divEmployee").hide();
                    //将当前页面设置为稳定状态
                    $.mTab.setStable();
                });
            }
        }
        //将当前页面设置为稳定状态

        $.mTab.setStable();
    },
    //保存收款单
    saveReceipt: function (callback, refreshHome) {
        var bizDate = $('#txtDate').datebox('getValue');
        if (!IVEditBase.verifyExchangeRate(bizDate)) {
            return;
        }
        //联系人类型
        var contactType = $('#selContactType').combobox('getValue');
        var contactID = "";
        //需要验证付款客户
        if (contactType == "Employees") {
            $("#selContact").combobox('disableValidation');
            $("#selEmployee").combobox('enableValidation');
            $("#selContact").addClass("no-validate");
            contactID = $('#selEmployee').combobox('getValue');
        } else {
            $("#selContact").combobox('enableValidation');
            $("#selEmployee").combobox('disableValidation');
            contactID = $('#selContact').combobox('getValue');
        }


        if (contactType == "Other") {
            $("#selContact").combobox('disableValidation');
        } else {
            $("#selContact").combobox('enableValidation');
        }

        //收款类别
        var payType = $('#selType').combobox('getValue');
        //如果联系人类型为“员工”或者“其他”，那么支付类别为“其他付款”或者“其他退款”
        if (contactType == "Employees" || contactType == "Other") {
            if (payType == "Receive_Sale") {
                payType = "Receive_Other";
            } else if (payType == "Receive_SaleReturn") {
                payType = "Receive_OtherReturn";
            }
        } else {
            if (payType == "Receive_Other") {
                payType = "Receive_Sale";
            } else if (payType == "Receive_OtherReturn") {
                payType = "Receive_SaleReturn";
            }
        }

        //验证收款单
        var result = $(".m-form-icon").mFormValidate();
        if (!result) {
            //验证不通过，则终止该操作
            return;
        }

        //获取收款单明细列表
        var entryInfo = IVEditBase.getInfo();
        //至少要有一条收款单明细
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
            if (check > 0) {
                var message = HtmlLang.Write(LangModule.IV, "CannotGreaterAmount", "The tax amount cannot be greater than the total amount");
                $.mDialog.alert(message);
                return;
            }
        }
        //要保存的收款单模型
        var obj = {};
        //收款单ID
        obj.MID = ReceiptEditBase.ReceiptID;
        //收款类别
        obj.MType = payType;
        //联系人类型
        obj.MContactType = contactType;
        //被收款人ID（可以理解为付款人ID）
        obj.MContactID = contactID;
        //业务日期
        obj.MBizDate = bizDate;
        //摘要
        obj.MReference = $("#txtRef").val();
        //银行账号ID
        obj.MBankID = $("#hidAccountID").val();
        //金额（不含税、原币）
        obj.MTotalAmtFor = entryInfo.MTotalAmtFor;
        //金额（不含税、本位币）
        obj.MTotalAmt = entryInfo.MTotalAmt;
        //金额（含税、原币）
        obj.MTaxTotalAmtFor = entryInfo.MTaxTotalAmtFor;
        //金额（含税、本位币）
        obj.MTaxTotalAmt = entryInfo.MTaxTotalAmt;
        //税制ID
        obj.MTaxID = entryInfo.MTaxID;
        //币别ID
        obj.MCyID = entryInfo.MCyID;
        var dbModel = mText.getObject("#hidReceiptModel");
        obj.MCurrentAccountCode = dbModel.MCurrentAccountCode;
        //汇率
        obj.MExchangeRate = entryInfo.MExchangeRate;
        obj.MOToLRate = entryInfo.MOToLRate;
        obj.MLToORate = entryInfo.MLToORate;
        //收款单明细列表
        obj.ReceiveEntry = entryInfo.InvoiceEntry;
        //提交保存
        $("body").mFormSubmit({
            url: "/IV/Receipt/UpdateReceipt",
            param: { model: obj },
            validate: true,
            extValidate: IVEditBase.valideInfo,
            callback: function (msg) {
                if (msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                    return;
                }
                if (refreshHome != false) {
                    BDBank.refreshTransPage(obj.MBankID, ReceiptEditBase.BankName, true);
                }
                if (callback != undefined) {
                    callback(msg, obj);
                }
            }
        });
    }
}
//初始化页面
$(document).ready(function () {
    ReceiptEditBase.init();
});