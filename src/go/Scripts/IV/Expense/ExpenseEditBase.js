/// <reference path="/Scripts/IV/IVBase.js" />
var ExpenseEditBase = {
    EditorHeight: "26px",
    InvoiceID: "",
    IVCurrentType: "sle",
    Selector: "#tbInvoiceDetail",
    url_delete: "/IV/Expense/DeleteExpenseList",
    url_unAuditToDraft: "/IV/Expense/ExpenseUnAuditToDraft",
    url_Edit: "/IV/Expense/ExpenseEdit",
    url_ExpenseList: "/IV/Expense/ExpenseList",
    Disabled: false,
    Verification: null,
    DataSourceInventory: [],
    //是否有 (费用报销单，付款单，收款单) 编辑权限
    HasChangeAuth: false,
    //是否有 银行账号 查看权限
    IsCanBankAccountViewPermission: false,
    //是否有 银行账号 编辑权限
    IsCanBankAccountChangePermission: false,
    //是否有 基础资料 编辑权限
    IsCanSettingChangePermission: false,
    //是否有 联系人 编辑权限
    IsCanContactChangePermission: false,
    IsCurrencyDisabled: false,
    IsFirstLoad: true,//是否是第一次加载
    IsCurrencyChange: true,//是否切换币种 默认true,
    IsCoverExchangeRate: true,//是否覆盖汇率
    DataSourceEntry: [],
    DataSourceTaxRate: [],
    TrackDataSource: [],
    ContactInfoDataSource: [],
    ContactTrackType: "",
    IsInit: false,
    RowIndex: -1,//当前编辑行索引
    MCyID: 'CNY', //币种
    ExchangeRate: 1,//当前税率
    SourceBillExchangeRate: 1,//原单据的汇率
    MOToLRate: 1,//原币到本位币汇率 1原币=?本位币
    MLToORate: 1,//本位币到原币汇率 1本位币?原币
    BizDate: '',
    SourceBillOToLRate: 1,
    SourceBillLToORate: 1,
    IVType: { Sale: "sle", Purchase: "pur" },
    init: function (type, invoiceId, srcOToLRate, srcLToORate, verification, entryDataSource, currencyId, status, disabled, currencyDisabled, bizDate) {
        ExpenseEditBase.SourceBillExchangeRate = srcOToLRate;
        ExpenseEditBase.SourceBillOToLRate = srcOToLRate;
        ExpenseEditBase.SourceBillLToORate = srcLToORate;
        ExpenseEditBase.BizDate = bizDate;
        ExpenseEditBase.IsInit = true;
        ExpenseEditBase.HasChangeAuth = $("#hidChangeAuth").val() == "1" ? true : false;
        ExpenseEditBase.IsCanBankAccountViewPermission = $("#hidIsCanBankAccountViewPermission").val() == "1" ? true : false;
        ExpenseEditBase.IsCanBankAccountChangePermission = $("#hidIsCanBankAccountChangePermission").val() == "1" ? true : false;
        ExpenseEditBase.IsCanSettingChangePermission = $("#hidIsCanSettingChangePermission").val() == "1" ? true : false;
        ExpenseEditBase.IsCanContactChangePermission = $("#hidIsCanContactChangePermission").val() == "1" ? true : false;

        if (disabled == undefined) {
            disabled = false;
        }
        ExpenseEditBase.IVCurrentType = type;
        ExpenseEditBase.IsCurrencyDisabled = currencyDisabled;
        //felson 2016.7.12
        //var track = $("#hidTrack").val();
        ExpenseEditBase.TrackDataSource = mData.getNameValueTrackDataList();

        ExpenseEditBase.Verification = verification;
        Megi.ComboBox.setDefaultValue("#selCurrency");
        Megi.ComboBox.setDefaultValue("#selTaxType");
        if (currencyDisabled == true) {
            $('#selCurrency').combobox('disable');
        }

        ExpenseEditBase.Disabled = disabled;
        if (disabled) {
            ExpenseEditBase.IsCurrencyDisabled = true;
            $('#selCurrency').combobox('disable');
            $('#selTaxType').combobox('disable');
            $(".form-invoice-total").addClass("form-invoice-total-disabled");
        }

        ExpenseEditBase.InvoiceID = invoiceId;


        //初始化费用报销项目
        ExpenseEditBase.initInventory(function () {

            //设置明细列表数据源
            ExpenseEditBase.setDataSourceEntry(entryDataSource, true);

            //绑定明细列表
            ExpenseEditBase.bindGrid(status);

            //选中指定的货币类型
            if (currencyId != null && currencyId != "-1") {
                var localCyId = $('#selCurrency').combobox("getValue");
                //币别是否存在，如果存在，则下拉选择，否则只显示
                var isCyExists = ExpenseEditBase.isCurrencyExists(currencyId);
                if (isCyExists == true) {
                    $('#selCurrency').combobox('select', currencyId);
                }
                else {
                    ExpenseEditBase.getCurrencyInfo(currencyId, function (info) {
                        $('#selCurrency').combobox('setValue', info.MCurrencyID);
                        $('#selCurrency').combobox('setText', info.MCurrencyName);
                        $("#selCurrency").attr("billCyID", info.MCurrencyID);
                        $("#selCurrency").attr("billExchangeRate", srcOToLRate);
                        ExpenseEditBase.changeCurrency({
                            MOToLRate: srcOToLRate,
                            MLToORate: srcLToORate,
                            CurrencyID: currencyId,
                            CurrencyName: info.MCurrencyName,
                            MLCurrencyID: localCyId,
                            MCyItemID: '',
                            IsLocalCy: currencyId == localCyId
                        });
                    });
                }
            }

            //更新明细列表数据源
            ExpenseEditBase.updateDataSourceEntry();

            //明细列表自适应
            IVFW.setUI();

            //将当前页面设置为稳定状态
            $.mTab.setStable();
        });
        //焦点移开分录结束编辑
        $(document).off("click.endEdit").on("click.endEdit", function (e) {
            IVBase.endEditEntry(e, ExpenseEditBase.Selector);
        });

        if (!ExpenseEditBase.InvoiceID) {
            //默认Due日期
            ExpenseEditBase.setDefaultDueDate();
        }
        ExpenseEditBase.IsInit = false;
    },

    //币别是否存在
    isCurrencyExists: function (cyId) {
        var curList = $('#selCurrency').combobox('getData');
        var isCyExists = false;
        for (var i = 0; i < curList.length; i++) {
            if (curList[i].CurrencyID == cyId) {
                isCyExists = true;
                break;
            }
        }
        return isCyExists;
    },
    //汇率是否不同
    isExchangeRateChange: function (cyId, mLToORate) {
        var curList = $('#selCurrency').combobox('getData');
        var isChange = true;
        for (var i = 0; i < curList.length; i++) {
            if (curList[i].CurrencyID == cyId) {
                if (curList[i].MLToORate == mLToORate) {
                    isChange = false;
                }
                break;
            }
        }
        return isChange;
    },
    getCurrencyInfo: function (cyId, callback) {
        mAjax.Post("/BD/Currency/GetBDCurrencyList", null, function (msg) {
            for (var i = 0; i < msg.length; i++) {
                if (msg[i].MCurrencyID == cyId) {
                    var obj = msg[i];
                    obj.MCurrencyName = obj.MCurrencyID + " " + obj.MName;
                    callback(obj);
                    return;
                }
            }
            callback({ MItemID: '', MCurrencyID: cyId, MCurrencyName: cyId });
        });
    },
    setDefaultDueDate: function () {
        if (ExpenseEditBase.IsInit) {
            if (ExpenseEditBase.BizDate) {
                var defaultDate = new Date(ExpenseEditBase.BizDate).addMonths(1);
                var nextMonthFirstDay = new Date(defaultDate.getFullYear(), defaultDate.getMonth() + 1, 1);
                var oneDay = 1000 * 60 * 60 * 24;
                $('#txtDueDate').datebox('setValue', nextMonthFirstDay - oneDay);
            }
        } else {
            var billDate = $('#txtDate').datebox('getValue');
            var value = ExpenseEditBase.getDefaultDueDate(billDate);
            $('#txtDueDate').datebox('setValue', value);
        }
    },


    //获取默认过期日期
    getDefaultDueDate: function (billDate) {
        if (billDate == "") {
            return "";
        }
        billDate = mDate.parse(billDate);
        if (ExpenseEditBase.ContactInfoDataSource == null) {
            return "";
        }
        var dueDate = 0;
        var dateCondition = "";

        dueDate = ExpenseEditBase.ContactInfoDataSource.MPurDueDate;
        dateCondition = ExpenseEditBase.ContactInfoDataSource.MPurDueCondition;
        var date = new Date(billDate);
        switch (dateCondition) {
            case "item0":
                date.setMonth(date.getMonth() + 1);
                date.setDate(dueDate);
                break;
            case "item1":
                date.setDate(date.getDate() + dueDate);
                break;
            case "item2":
                date = new Date(date.getFullYear(), date.getMonth(), 1);
                date.setMonth(date.getMonth() + 1);
                date.setDate(date.getDate() + dueDate - 1);
                break;
            case "item3":
                date.setMonth(date.getMonth());
                date.setDate(dueDate);
                break;
        }
        return date.format(top.MegiGlobal.MDateFormat);
    },

    //费用报销单支付
    mergePay: function (id) {
        mAjax.post(
            "/IV/Expense/IsSuccessMerge",
            { expids: id },
            function (res) {
                if (res.Success) {
                    Megi.dialog({
                        title: HtmlLang.Write(LangModule.IV, "MakeAPayment", "Make a payment"),
                        width: 500,
                        height: 345,
                        href: '/IV/Expense/ExpensesMerge?expids=' + id + "&payfrom=editpage"
                    });
                }
                else {
                    $.mDialog.alert(res.Message);
                }
            });
    },
    afterVerification: function () {
        var url = ExpenseEditBase.url_ExpenseList + "?id=4";
        $.mTab.addOrUpdate("", url, false, false);
    },
    //费用报销单删除（加删除标记）
    deleteItem: function (id, redirectUrl) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete,
            {
                callback: function () {
                    var param = {};
                    param.KeyIDs = id;
                    mAjax.submit(
                        ExpenseEditBase.url_delete,
                        { param: param },
                        function (msg) {
                            if (msg.Success) {
                                //tab选项卡标题
                                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                                //提示信息
                                $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                                //在指定的tab选项卡中刷新

                                if (window.parent.$(".mCloseBox").length == 0) {
                                    $.mTab.refresh(title, redirectUrl, false, true);

                                    //关闭当前选项卡
                                    $.mTab.remove();
                                } else {
                                    parent.$.mTab.refresh(title, redirectUrl, false, true);
                                    $.mDialog.close();
                                }
                            } else {
                                $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Delete failed"));
                            }
                        });
                }
            });
    },
    //反审核到草稿状态
    unAuditToDraft: function (id) {
        $.mDialog.confirm(HtmlLang.Write(LangModule.IV, "AreYouSureToUnAuditToDraft", "Are you sure to UnAudit To Draft?"), {
            callback: function () {
                var objParam = {};
                objParam.KeyIDs = id;
                objParam.MOperationID = IVBase.Status.Draft;

                mAjax.post(ExpenseEditBase.url_unAuditToDraft, { expenseId: id }, function (data) {
                    if (data.Success) {
                        //提示信息
                        $.mMsg(HtmlLang.Write(LangModule.IV, "UnAuditToDraftSuccessfully", "UnAudit To Draft Successfully!"));
                        //刷新费用报销列表页面
                        var listTitle = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                        var listUrl = ExpenseEditBase.url_ExpenseList + "?id=" + IVBase.Status.Draft;
                        $.mTab.refresh(listTitle, listUrl, false, true);
                        //刷新当前页面
                        mWindow.reload(ExpenseEditBase.url_Edit + "/" + id);
                    } else {
                        $.mAlert(data.Message, function () {
                            mWindow.reload();
                        });
                    }
                });
            }
        });
    },
    //获取tab选项卡标题
    getTabTitle: function (type) {
        if (type) {
            switch (type) {
                case "NewExpense":
                    return HtmlLang.Write(LangModule.IV, "NewExpense", "New Expense");
                case "CopyExpense":
                    return HtmlLang.Write(LangModule.IV, "CopyExpense", "Copy Expense");
                default:
                    return "";
            }
        } else {
            //var ref = $.trim($("#txtRef").val());
            //return ref != "" ? $.mIV.getTitle(mTitle_Pre_Expense, ref) : HtmlLang.Write(LangModule.IV, "EditExpense", "Edit Expense");
            return HtmlLang.Write(LangModule.IV, "EditExpense", "Edit Expense");
        }
    },
    //验证信息
    valideInfo: function () {
        var result = true;
        for (var i = 0; i < ExpenseEditBase.DataSourceEntry.length; i++) {
            var item = ExpenseEditBase.DataSourceEntry[i];
            //备注去左右空格
            item.MDesc = $.trim(item.MDesc);
            //如果所有列都为空则跳过
            if ((item.MItemID == undefined || item.MItemID == null || item.MItemID == "")
                && (item.MDesc == undefined || item.MDesc == null || item.MDesc == "")
                && item.MQty == ""
                && item.MPrice == ""
                && (item.MTrackItem1 == undefined || item.MTrackItem1 == null || item.MTrackItem1 == "")
                && (item.MTrackItem2 == undefined || item.MTrackItem2 == null || item.MTrackItem2 == "")
                && (item.MTrackItem3 == undefined || item.MTrackItem3 == null || item.MTrackItem3 == "")
                && (item.MTrackItem4 == undefined || item.MTrackItem4 == null || item.MTrackItem4 == "")
                && (item.MTrackItem5 == undefined || item.MTrackItem5 == null || item.MTrackItem5 == "")) {
                continue;
            }
            if (item.MItemID == undefined || item.MItemID == null || item.MItemID == "") {
                $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MItemID']").addClass("row-error");
                $(".datagrid-editable-input").css("height", "28px");
                result = false;
            }
            if (item.MDesc == "") {
                $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MDesc']").addClass("row-error");
                $(".datagrid-editable-input").css("height", "28px");
                result = false;
            }
            if (item.MPrice == "") {
                $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MPrice']").addClass("row-error");
                $(".datagrid-editable-input").css("height", "28px");
                result = false;
            }
            if (item.MQty == "") {
                $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MQty']").addClass("row-error");
                $(".datagrid-editable-input").css("height", "28px");
                result = false;
            }
        }
        return result;
    },
    //获取明细实体列表
    getViewInfo: function (withId) {
        ExpenseEditBase.endEditGrid();
        ExpenseEditBase.updateDataSourceEntry();

        var currencyId = $('#selCurrency').combobox('getValue');

        var totalAmtFor = 0;
        var totalAmt = 0;
        var taxTotalAmtFor = 0;
        var taxTotalAmt = 0;
        var obj = {};
        var arr = new Array();
        for (var i = 0; i < ExpenseEditBase.DataSourceEntry.length; i++) {
            var item = ExpenseEditBase.DataSourceEntry[i];
            //排序
            item.MSeq = i + 1;
            //备注去左右空格
            item.MDesc = $.trim(item.MDesc);
            //如果所有列都为空则跳过
            if ((item.MItemID == undefined || item.MItemID == null || item.MItemID == "")
                && (item.MDesc == undefined || item.MDesc == null || item.MDesc == "")
                && item.MQty == ""
                && item.MPrice == ""
                && (item.MTrackItem1 == undefined || item.MTrackItem1 == null || item.MTrackItem1 == "")
                && (item.MTrackItem2 == undefined || item.MTrackItem2 == null || item.MTrackItem2 == "")
                && (item.MTrackItem3 == undefined || item.MTrackItem3 == null || item.MTrackItem3 == "")
                && (item.MTrackItem4 == undefined || item.MTrackItem4 == null || item.MTrackItem4 == "")
                && (item.MTrackItem5 == undefined || item.MTrackItem5 == null || item.MTrackItem5 == "")) {
                continue;
            }
            if (withId != undefined && withId == false) {
                item.MEntryID = "";
            }
            arr.push(item);
            totalAmtFor += parseFloat(Math.abs(item.MAmountFor));
            totalAmt += parseFloat(Math.abs(item.MAmount));
            taxTotalAmtFor += parseFloat(Math.abs(item.MTaxAmountFor));
            taxTotalAmt += parseFloat(Math.abs(item.MTaxAmount));
        }
        obj.ExpenseEntry = arr;
        obj.MCyID = currencyId;
        obj.MTotalAmtFor = totalAmtFor;
        obj.MTotalAmt = totalAmt;
        obj.MTaxTotalAmtFor = taxTotalAmtFor;
        obj.MTaxTotalAmt = taxTotalAmt;
        obj.MExchangeRate = ExpenseEditBase.ExchangeRate;
        obj.MOToLRate = ExpenseEditBase.MOToLRate;
        obj.MLToORate = ExpenseEditBase.MLToORate;
        return obj;
    },
    //初始化物料信息（这里改成获取费用项目）
    initInventory: function (callback) {
        var params = { includeDisable: true };
        mAjax.post("/BD/ExpenseItem/GetExpenseItemListByTier", params, function (msg) {
            ExpenseEditBase.DataSourceInventory = msg;
            if (callback && $.isFunction(callback)) {
                callback();
            }
        })
    },
    verifyExchangeRate: function (bizDate,isShowAlter) {
        if ($("#spExchangeInfo").hasClass("mg-prompt-error")) {
            if (isShowAlter) {
                var message = HtmlLang.Write(LangModule.IV, "CantFindExchangeRate", "该币别在（{0}）没有有效的汇率，请在基础资料或单据上汇率处进行设置！");
                $.mDialog.alert(message.replace("{0}", bizDate));
            }
            return false;
        }
        return true;
    },
    //更新数据源
    updateDataSourceEntry: function (keyValue) {
        for (var i = 0; i < ExpenseEditBase.DataSourceEntry.length; i++) {
            //找到当前行
            if (i == ExpenseEditBase.RowIndex) {
                if (!!keyValue) {
                    eval("ExpenseEditBase.DataSourceEntry[i]." + keyValue);
                }

                var amtFor = ExpenseEditBase.getAmount(i);
                var amt = "";
                var taxAmt = "";
                var taxAmtFor = "";
                var tax = "";
                var taxFor = ExpenseEditBase.getTaxFor(i);


                amt = amtFor * ExpenseEditBase.MOToLRate;
                taxAmtFor = amtFor;
                taxAmt = amt;
                tax = taxFor * ExpenseEditBase.MOToLRate;


                ExpenseEditBase.DataSourceEntry[i].MAmountFor = Megi.Math.toDecimal(amtFor, 2);
                ExpenseEditBase.DataSourceEntry[i].MAmount = Megi.Math.toDecimal(amt, 2);
                ExpenseEditBase.DataSourceEntry[i].MApproveAmtFor = Megi.Math.toDecimal(amtFor, 2);
                ExpenseEditBase.DataSourceEntry[i].MApproveAmt = Megi.Math.toDecimal(amt, 2);
                ExpenseEditBase.DataSourceEntry[i].MTaxAmountFor = Megi.Math.toDecimal(taxAmtFor, 2);
                ExpenseEditBase.DataSourceEntry[i].MTaxAmount = Megi.Math.toDecimal(taxAmt, 2);
                ExpenseEditBase.DataSourceEntry[i].MTaxAmtFor = Megi.Math.toDecimal(taxFor, 2);
                ExpenseEditBase.DataSourceEntry[i].MTaxAmt = Megi.Math.toDecimal(tax, 2);

                $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + ExpenseEditBase.RowIndex + "]>td[field='MTaxAmountFor']").children("div").css({ 'padding-right': '4px' }).text((amtFor > 0 ? Megi.Math.toMoneyFormat(Math.abs(amtFor)) : ""));
                $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + ExpenseEditBase.RowIndex + "]>td[field='MApproveAmtFor']").html("<div style='text-align:right;padding-right:4px;'>" + amtFor + "</div>");
            }
        }
        ExpenseEditBase.updateSummaryInfo();
    },
    //获取行数据的汇总
    getAmount: function (rowIndex) {
        var vQty = ExpenseEditBase.DataSourceEntry[rowIndex].MQty;
        var vPrice = ExpenseEditBase.DataSourceEntry[rowIndex].MPrice;
        vQty = String(vQty).replace(new RegExp(",", "g"), "");
        vPrice = String(vPrice).replace(new RegExp(",", "g"), "");
        var amount = 0;
        if (vQty != undefined && vQty != "" && vPrice != undefined && vPrice != "") {
            amount = Megi.Math.toDecimal(vQty, 4) * Megi.Math.toDecimal(vPrice, 8);
        }
        return parseFloat(Megi.Math.toDecimal(amount, 2));
    },
    //获取用户输入的税金额
    getTaxFor: function (rowIndex) {
        var taxFor = ExpenseEditBase.DataSourceEntry[rowIndex].MTaxAmtFor;
        taxFor = String(taxFor).replace(new RegExp(",", "g"), "");
        if (!taxFor) {
            taxFor = 0;
        }
        return parseFloat(Megi.Math.toDecimal(taxFor, 2));
    },
    //更新合计信息
    updateSummaryInfo: function () {
        var amountFor = 0;
        var taxAmountFor = 0;
        var taxAmtFor = 0;
        var taxAmt = 0;

        var arrTax = new Array();

        for (var i = 0; i < ExpenseEditBase.DataSourceEntry.length; i++) {
            var item = ExpenseEditBase.DataSourceEntry[i];
            if (item.MAmountFor != "") {
                amountFor += +item.MAmountFor;
                taxAmountFor += +item.MTaxAmountFor;
                taxAmtFor += +item.MTaxAmtFor;
                taxAmt += parseFloat(item.MTaxAmount);
            }
        }

        $("#spSubTotal").html(Megi.Math.toDecimal(taxAmountFor, 2));
        ExpenseEditBase.updateTaxSummaryInfo(taxAmtFor);
        $("#spTotal").html(Megi.Math.toDecimal(taxAmountFor, 2));
        ExpenseEditBase.updateCreditInfo(taxAmountFor);

        ExpenseEditBase.setTotalAmtTip(taxAmt);
    },
    setTotalAmtTip: function (totalTaxAmt) {
        var currencyId = $('#selCurrency').combobox('getValue');
        var localCyId = $("#spTotalCurrency").attr("localCyID");
        //如果是本位币，不需要提示
        if (localCyId == currencyId) {
            return;
        }
        $("#spTotalCurrency").mLocalCyTooltip(totalTaxAmt, localCyId);
    },
    //更新页面上税率总计信息
    updateTaxSummaryInfo: function (taxAmtFor) {
        //税率暂时隐藏
        var html = '';
        html += '<div class="tax"><span class="mg-total-text">' + HtmlLang.Write(LangModule.IV, "TaxAmountFor", "税额") + '</span><span class="mg-total-value">' + Megi.Math.toDecimal(taxAmtFor, 2) + '</span></div>';
        $("#divTax").html(html);
    },
    //更新页面上核销总计信息
    updateCreditInfo: function (amtFor) {
        if (ExpenseEditBase.Verification == null || ExpenseEditBase.Verification.length == 0) {
            $("#divCredit").html("");
            return;
        }
        var html = "";
        var totalVerificationAmt = 0;
        for (var i = 0; i < ExpenseEditBase.Verification.length; i++) {
            var lastStyle = "";
            if (i == ExpenseEditBase.Verification.length - 1) {
                lastStyle = "style='border-bottom:none;'";
            }
            var item = ExpenseEditBase.Verification[i];
            var verifiText = HtmlLang.Write(LangModule.Common, item.MBizType, item.MBizType);
            var less = "<label style='color:#888;'>" + HtmlLang.Write(LangModule.IV, "Less", "Less") + "</label> ";
            verifiText = less + verifiText;

            totalVerificationAmt += Math.abs(item.MHaveVerificationAmtFor);

            //是否有银行查看权限
            if (ExpenseEditBase.IsCanBankAccountViewPermission) {
                //是否有银行编辑权限
                if (ExpenseEditBase.IsCanBankAccountChangePermission) {
                    html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" onclick=\'ExpenseEditBase.ViewVerificationDetail("' + item.MBillID + '","' + item.MBizType + '","")\'><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toDecimal(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><a href="javascript:void(0)" onclick="ExpenseEditBase.deleteVerification(\'' + item.VerificationID + '\')" class="m-icon-delete">&nbsp;</a><div class="clear"></div></div>';
                } else {
                    html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" onclick=\'ExpenseEditBase.ViewVerificationDetail("' + item.MBillID + '","' + item.MBizType + '","")\'><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toDecimal(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><div class="clear"></div></div>';
                }
            } else {
                if (ExpenseEditBase.IsCanBankAccountChangePermission) {
                    html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toDecimal(Math.abs(item.MHaveVerificationAmtFor)) + '</span><a href="javascript:void(0)" onclick="ExpenseEditBase.deleteVerification(\'' + item.VerificationID + '\')" class="m-icon-delete">&nbsp;</a><div class="clear"></div></div>';
                } else {
                    html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toDecimal(Math.abs(item.MHaveVerificationAmtFor)) + '</span><div class="clear"></div></div>';
                }
            }
        }
        var amountDue = amtFor - totalVerificationAmt;
        html += ' <div class="due-total"><span class="mg-total-text">' + HtmlLang.Write(LangModule.IV, "AmountDue", "Amount Due") + '</span><span class="mg-total-value" id="spTotal">' + Megi.Math.toDecimal(amountDue) + '</span><span class="mg-total-currency"></span></div>';
        $("#divCredit").html(html);
    },
    //查看核销的详细信息
    ViewVerificationDetail: function (billId, bizType, ref) {
        var url = "/IV/Payment/PaymentView/" + billId;
        //var titlePre ="Pay";
        //$.mTab.addOrUpdate($.mIV.getTitle(titlePre, ref), url); 
        $.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, "ViewPayment", "View Payment"), url);
    },
    //删除核销记录
    deleteVerification: function (verificationId) {
        var content = HtmlLang.Write(LangModule.IV, "DeleteReconcileRelationshipConfirm", "Are you sure to delete the reconcile relationship.");
        $.mDialog.confirm(content, null, function () {
            mAjax.submit(
                "/IV/Verification/DeleteVerification/",
                { verificationId: verificationId },
                function (msg) {
                    var messsage = '';
                    if (msg.Success) {
                        messsage = HtmlLang.Write(LangModule.IV, "DeleteReconcileRelationshipSuccessfully", "Delete reconcile relationship successfully!");
                        $.mMsg(messsage);
                        mWindow.reload();
                    }
                    else {
                        messsage = msg.Message;
                        $.mAlert(messsage, function () {
                            mWindow.reload();
                        });
                    }
                });
        });
    },
    //获取物料编码
    getInventoryCode: function (itemId) {
        if (ExpenseEditBase.DataSourceInventory) {
            for (var i = 0; i < ExpenseEditBase.DataSourceInventory.length; i++) {
                if (ExpenseEditBase.DataSourceInventory[i].MItemID == itemId) {
                    return ExpenseEditBase.DataSourceInventory[i].MName;
                }
            }
        }
        //return itemId;
        return "";
    },
    //获取跟踪项名称
    getTrackName: function (value) {
        for (var i = 0; i < ExpenseEditBase.TrackDataSource.length; i++) {
            if (ExpenseEditBase.TrackDataSource[i].MChildren != null) {
                for (var k = 0; k < ExpenseEditBase.TrackDataSource[i].MChildren.length; k++) {
                    if (value == ExpenseEditBase.TrackDataSource[i].MChildren[k].MValue) {
                        return ExpenseEditBase.TrackDataSource[i].MChildren[k].MName;
                    }
                }
            }
        }
        return "";
    },
    //初始化税率数据源
    initTaxRate: function () {
        mAjax.post("/BD/TaxRate/GetTaxRateList", null, function (msg) {
            ExpenseEditBase.DataSourceTaxRate = msg;
        });
    },
    //获取税率名称
    getTaxRateName: function (taxRateId) {
        for (var i = 0; i < ExpenseEditBase.DataSourceTaxRate.length; i++) {
            if (ExpenseEditBase.DataSourceTaxRate[i].MItemID == taxRateId) {
                return ExpenseEditBase.DataSourceTaxRate[i].MText;
            }
        }
        return taxRateId;
    },
    //获取税率
    getTaxRate: function (taxRateId) {
        if (taxRateId == null || taxRateId == "") {
            return 0;
        }
        for (var i = 0; i < ExpenseEditBase.DataSourceTaxRate.length; i++) {
            if (ExpenseEditBase.DataSourceTaxRate[i].MItemID == taxRateId) {
                return parseFloat(ExpenseEditBase.DataSourceTaxRate[i].MEffectiveTaxRateDecimal);
            }
        }
        return 0;
    },
    //重新设置数据源
    setDataSourceEntry: function (entryDataSource, addDefaultRow) {
        var defaultRow = 4;
        if (entryDataSource == null) {
            entryDataSource = new Array();
        }
        var rowIndex = 0;
        for (var i = 0; i < entryDataSource.length; i++) {
            entryDataSource[i].RowIndex = rowIndex;
            rowIndex += 1;
        }
        if (addDefaultRow == undefined || addDefaultRow == true) {
            var dataLength = entryDataSource.length;
            var emptyRow = 1;
            if (dataLength < defaultRow) {
                emptyRow = defaultRow - 1 - dataLength + emptyRow;
            }
            for (var i = 0; i < emptyRow; i++) {
                var obj = ExpenseEditBase.getEmptyEntry();
                obj.RowIndex = rowIndex;
                entryDataSource.push(obj);
                rowIndex += 1;
            }
        }
        ExpenseEditBase.DataSourceEntry = entryDataSource;
    },
    //删除数据源中指定的记录（根据索引删除）
    deleteDataSourceEntry: function (rowIndex) {
        if (ExpenseEditBase.DataSourceEntry.length <= 1) {
            $.mDialog.alert("You must have at least 1 line item.", null, LangModule.IV, "AtLeastOneLineItem");
            return false;
        }
        var arr = new Array();
        var index = 0;
        for (var i = 0; i < ExpenseEditBase.DataSourceEntry.length; i++) {
            if (i != rowIndex) {
                var obj = ExpenseEditBase.DataSourceEntry[i];
                obj.RowIndex = index;
                arr.push(obj);
                index += 1;
            }
        }
        ExpenseEditBase.DataSourceEntry = arr;
        return true;
    },
    //插入一条记录到数据源中（根据索引插入）
    insertDataSourceEntry: function (rowIndex) {
        var newRow = null;
        var arr = new Array();
        var index = 0;
        for (var i = 0; i < ExpenseEditBase.DataSourceEntry.length; i++) {
            if (ExpenseEditBase.DataSourceEntry[i].RowIndex == rowIndex) {
                newRow = ExpenseEditBase.getEmptyEntry();
                newRow.RowIndex = index;
                arr.push(newRow);
                index += 1;
            }
            var obj = ExpenseEditBase.DataSourceEntry[i];
            obj.RowIndex = index;
            arr.push(obj);
            index += 1;
        }
        if (rowIndex == ExpenseEditBase.DataSourceEntry.length) {
            var lastRow = ExpenseEditBase.getEmptyEntry();
            lastRow.RowIndex = rowIndex;
            arr.push(lastRow);
            ExpenseEditBase.DataSourceEntry = arr;
            return lastRow;
        }
        ExpenseEditBase.DataSourceEntry = arr;
        return newRow;
    },
    //追加一条记录到数据源中（追加到最后）
    appendDataSourceEntry: function () {
        var index = ExpenseEditBase.DataSourceEntry.length;
        var newRow = ExpenseEditBase.getEmptyEntry();
        newRow.RowIndex = index;
        ExpenseEditBase.DataSourceEntry.push(newRow);
        return newRow;
    },
    //数据源增加Description列
    addDataSourceEntryDescItem: function (entryDataSource) {
        if (entryDataSource.length > 0) {
            for (var i = 0; i < entryDataSource.length; i++) {
                var desc = "";
                for (var k = 0; k < ExpenseEditBase.DataSourceInventory.length; k++) {
                    if (entryDataSource[i].MItemID == ExpenseEditBase.DataSourceInventory[k].MItemID) {
                        desc = ExpenseEditBase.DataSourceInventory[k].MDesc;
                        break;
                    }
                }
                entryDataSource[i].MDesc = desc;
            }
        }
    },
    //绑定明细列表
    bindGrid: function (status) {
        var columns = ExpenseEditBase.Columns.getArray(status);
        Megi.grid(ExpenseEditBase.Selector, {
            columns: columns,
            resizable: true,
            auto: true,
            data: ExpenseEditBase.DataSourceEntry,
            singleSelect: true,
            //单据单元格时，设置当前双击的行为可编辑状态
            onClickCell: function (rowIndex, field, value) {
                if (ExpenseEditBase.Disabled || field == "MEntryID" || field == "MAddID") {
                    return;
                }
                ExpenseEditBase.endEditGrid();
                var currencyId = $('#selCurrency').combobox('getValue');
                if (currencyId == null || currencyId == "") {
                    return;
                }
                ExpenseEditBase.RowIndex = rowIndex;
                Megi.grid(ExpenseEditBase.Selector, "beginEdit", rowIndex);
                var editor = $(ExpenseEditBase.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    if (field == "MItemID") {
                        $(editor.target).combobox("showPanel");
                    } else if (field == "MTrackItem1" || field == "MTrackItem2" || field == "MTrackItem3" || field == "MTrackItem4" || field == "MTrackItem5") {
                        $(editor.target).combobox("showPanel");
                    }
                    else {
                        //editor.target.select();
                    }
                    //$(".datagrid-body").find(".numberbox,.datagrid-editable-input,.validatebox-text").click(function () {
                    //    $(this).select();
                    //});
                }
            },
            //单击列表行时，设置列表为不可编辑状态
            onClickRow: function (rowIndex, rowData) {
                if (ExpenseEditBase.Disabled) {
                    return;
                }
                ExpenseEditBase.bindGridEditorEvent(rowData, rowIndex);
                ExpenseEditBase.bindGridEditorTabEvent(rowData, rowIndex);
                if (rowIndex == ExpenseEditBase.DataSourceEntry.length - 1) {
                    ExpenseEditBase.AddItemByIndex(ExpenseEditBase.DataSourceEntry.length);
                }
            },
            onAfterEdit: function () {
                ExpenseEditBase.updateDataSourceEntry();
            }
        });
        //easyui leftWidth 计算有问题，会导致 leftWidth 值逐渐变大，然后出现滚动条。
        Megi.grid(ExpenseEditBase.Selector, "resize");

        Megi.grid(ExpenseEditBase.Selector, "selectRow", 0);
    },
    //给明细列表控件绑定 Tab 事件
    bindGridEditorTabEvent: function (rowData, rowIndex) {
        $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input").each(function () {
            $(this).keyup(function (event) {
                if (event.which == 13) {
                    //当前编辑行的所有控件
                    var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                    //当前正在编辑的控件的索引
                    var editing_controls_index = editing_controls.index(this);
                    //如果当前正在编辑的控件是该编辑行的最后一个控件
                    if (editing_controls_index == editing_controls.length - 1) {
                        //移动到最后一行最后一个控件时，自动增加一行
                        if (rowIndex == ExpenseEditBase.DataSourceEntry.length - 1) {
                            ExpenseEditBase.AddItemByIndex(ExpenseEditBase.DataSourceEntry.length);
                        }
                        //则结束当前编辑行，开始进行下一行编辑
                        ExpenseEditBase.beginNextRowEdit(rowIndex);
                        editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                        if (editing_controls.length > 0) {
                            //设置下一编辑行的第一个控件焦点
                            editing_controls[0].focus();
                        }
                    } else {
                        //将焦点移动到下一个控件
                        editing_controls[editing_controls_index + 1].focus();
                    }
                }
                if (event.which == 9) {
                    var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                    var editing_controls_index = editing_controls.index(this);
                    //如果当前正在编辑的控件是该编辑行的最后一个控件
                    if (editing_controls_index == editing_controls.length - 1) {
                        //移动到最后一行最后一个控件时，自动增加一行
                        if (rowIndex == ExpenseEditBase.DataSourceEntry.length - 1) {
                            ExpenseEditBase.AddItemByIndex(ExpenseEditBase.DataSourceEntry.length);
                        }
                    }
                    var field = $(editing_controls[editing_controls_index]).closest("td[field]").attr("field");
                    var editor = $(ExpenseEditBase.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                    if (editor != null) {
                        if (field == "MItemID" || field == "MTrackItem1" || field == "MTrackItem2" || field == "MTrackItem3" || field == "MTrackItem4" || field == "MTrackItem5") {
                            $(editor.target).combobox("showPanel");
                        } else {
                            //editor.target.select();
                        }
                    }

                }
            }).keydown(function (event) {
                if (event.which == 9) {
                    if ($(this).closest(".datagrid-editable").closest("td").next().attr("field") == "MAmountFor") {
                        ExpenseEditBase.beginNextRowEdit(rowIndex);
                    }
                }
            }).focus(function (event) {
                var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                var editing_controls_index = editing_controls.index(this);
                var field = $(editing_controls[editing_controls_index]).closest("td[field]").attr("field");
                var editor = $(ExpenseEditBase.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    if (field == "MItemID" || field == "MTrackItem1" || field == "MTrackItem2" || field == "MTrackItem3" || field == "MTrackItem4" || field == "MTrackItem5") {
                        $(editor.target).combobox("showPanel");
                    } else {
                        //editor.target.select();
                    }
                }
            });
        });
    },
    //开始下一行编辑
    beginNextRowEdit: function (rowIndex) {
        ExpenseEditBase.endEditGrid();
        var newRowIndex = rowIndex + 1;
        ExpenseEditBase.RowIndex = newRowIndex;
        Megi.grid(ExpenseEditBase.Selector, "selectRow", newRowIndex);
        Megi.grid(ExpenseEditBase.Selector, "beginEdit", newRowIndex);
        var newRowData = Megi.grid(ExpenseEditBase.Selector, "getSelected");

        var newEditor = $(ExpenseEditBase.Selector).datagrid('getEditor', { index: newRowIndex, field: "MItemID" });
        if (newEditor != null) {
            $(newEditor.target).combobox("showPanel");
            //设置前面的+为tab的焦点，这样combox才能获取焦点
            $(newEditor.target).closest("td[field='MItemID']").prev().find('.m-icon-add-row').focus();
        }
        ExpenseEditBase.bindGridEditorTabEvent(newRowData, newRowIndex);
        ExpenseEditBase.bindGridEditorEvent(newRowData, newRowIndex);
    },
    //给明细列表控件绑定 KeyUp 事件
    bindGridEditorEvent: function (rowData, rowIndex) {
        var ed = $(ExpenseEditBase.Selector).datagrid('getEditors', rowIndex);
        for (var i = 0; i < ed.length; i++) {
            var e = ed[i];
            switch (e.field) {
                case "MQty":
                    $(e.target).bind("keyup", function (event) {
                        var value = ExpenseEditBase.getGridEditorValue(ExpenseEditBase.RowIndex, "MQty");
                        if (value < 0 || isNaN(parseFloat(value))) {
                            value = 0;
                        }
                        ExpenseEditBase.updateDataSourceEntry("MQty='" + value + "'");
                        rowData.MAmountFor = ExpenseEditBase.DataSourceEntry[ExpenseEditBase.RowIndex].MAmountFor;
                        //检查税额是否大于总额
                        var taxAmoutFor = ExpenseEditBase.getGridEditorValue(rowIndex, "MTaxAmtFor");
                        ExpenseEditBase.checkTaxAmtForValue(rowIndex, taxAmoutFor);
                    });
                    break;
                case "MPrice":
                    $(e.target).bind("keyup", function (event) {
                        var value = ExpenseEditBase.getGridEditorValue(ExpenseEditBase.RowIndex, "MPrice");
                        if (value < 0 || isNaN(parseFloat(value))) {
                            value = 0;
                        }
                        ExpenseEditBase.updateDataSourceEntry("MPrice='" + value + "'");
                        rowData.MAmountFor = ExpenseEditBase.DataSourceEntry[ExpenseEditBase.RowIndex].MAmountFor;
                        //检查税额是否大于总额
                        var taxAmoutFor = ExpenseEditBase.getGridEditorValue(rowIndex, "MTaxAmtFor");
                        ExpenseEditBase.checkTaxAmtForValue(rowIndex, taxAmoutFor);
                    });
                    break;
                case "MTaxAmtFor":
                    $(e.target).bind("keyup", function (event) {
                        var value = ExpenseEditBase.getGridEditorValue(ExpenseEditBase.RowIndex, "MTaxAmtFor");
                        if (value < 0 || isNaN(parseFloat(value))) {
                            value = 0;
                        }
                        ExpenseEditBase.updateDataSourceEntry("MTaxAmtFor='" + value + "'");
                        rowData.MAmountFor = ExpenseEditBase.DataSourceEntry[ExpenseEditBase.RowIndex].MAmountFor;
                        //检查税额是否大于总额
                        ExpenseEditBase.checkTaxAmtForValue(rowIndex, value);
                    });
                    break;
            }
        }
    },
    checkTaxAmtForValue: function (rowIndex, taxAmtValue) {
        //税额
        var value = +Megi.Math.toDecimal(parseFloat(taxAmtValue), 2);
        //获取一行的总额
        var amount = ExpenseEditBase.getAmount(rowIndex);
        var check = value - (+Megi.Math.toDecimal(parseFloat(amount), 2));
        //税额大于总额
        if (check > 0) {
            var message = HtmlLang.Write(LangModule.IV, "CannotGreaterAmount", "The tax amount cannot be greater than the total amount");
            $.mDialog.message(message);
            //给税额赋0
            ExpenseEditBase.setGridEditorValue(rowIndex, "MTaxAmtFor", 0);
        }
    },
    setGridEditorValue: function (rowIndex, field, setValue) {
        var rowObj = $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]");
        $(rowObj).find("td[field='" + field + "']").find("input").val(setValue);
    },
    //获取明细列表编辑状态下控件的值
    getGridEditorValue: function (rowIndex, field) {
        var rowObj = $(ExpenseEditBase.Selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]");
        var value = $(rowObj).find("td[field='" + field + "']").find("input").val();
        return value;
    },
    //获取一个空的明细对象
    getEmptyEntry: function () {
        var obj = {};
        obj.MEntryID = "";
        obj.InvoiceID = ExpenseEditBase.InvoiceID;
        obj.MItemID = "";
        obj.MIsNew = true;
        obj.MDesc = "";
        obj.MQty = "";
        obj.MPrice = "";
        //obj.MDiscount = "";
        obj.MAcctID = "";
        obj.MAmountFor = "";
        obj.MAmount = "";
        obj.MApproveAmtFor = "";
        obj.MApproveAmt = "";
        obj.MTaxAmountFor = "";
        obj.MTaxAmount = "";
        obj.MTaxAmtFor = "";
        obj.MTaxAmt = "";
        obj.MTrackItem1 = "";
        obj.MTrackItem2 = "";
        obj.MTrackItem3 = "";
        obj.MTrackItem4 = "";
        obj.MTrackItem5 = "";
        return obj;
    },
    //明细列表的列数组
    Columns: {
        getArray: function (status) {
            var arr = new Array();

            //新增行
            if (!ExpenseEditBase.Disabled) {
                arr.push({
                    title: '', field: 'MAddID', width: 30, height: "26px", align: 'center', formatter: function (value, rowData, rowIndex) {
                        return "<a href='javascript:void(0)' class='m-icon-add-row' onclick='ExpenseEditBase.AddItem(this)'>&nbsp;</a>";
                    }
                });
            }
            //费用报销项
            var itemColumn = ExpenseEditBase.Columns.getItemColumn();
            arr.push(itemColumn);

            arr.push({ title: HtmlLang.Write(LangModule.IV, "Description", "Description"), field: 'MDesc', width: 120, align: 'left', sortable: false, editor: { type: 'text' } });
            arr.push({ title: HtmlLang.Write(LangModule.IV, "Qty", "Qty"), field: 'MQty', width: 80, align: 'right', sortable: false, formatter: function (value, rowData, rowIndex) { return Megi.Math.toMoneyFormat(value, 4, 0) }, editor: { type: 'numberbox', options: { precision: 4, minPrecision: 0, min: 0, max: 999999999999 } } });
            arr.push({ title: HtmlLang.Write(LangModule.IV, "UnitPrice", "Unit Price"), field: 'MPrice', width: 80, height: ExpenseEditBase.EditorHeight, align: 'right', sortable: false, formatter: function (value, rowData, rowIndex) { return Megi.Math.toMoneyFormat(value, 4, 2) }, editor: { type: 'numberbox', options: { precision: 4, minPrecision: 2, min: 0, max: 999999999999 } } });
            arr.push({
                title: HtmlLang.Write(LangModule.IV, "TaxAmountFor", "税额"), field: 'MTaxAmtFor', width: 80, height: ExpenseEditBase.EditorHeight, align: 'right', sortable: false, formatter: function (value, rowData, rowIndex) {
                    if (!rowData.MQty || !rowData.MPrice) {
                        return "";
                    }
                    return Megi.Math.toMoneyFormat(value, 2, 2)
                },
                editor: { type: 'numberbox', options: { precision: 2, minPrecision: 2, min: 0, max: 999999999999 } }
            });

            //税率 暂时隐藏掉
            //var taxRateColumn = ExpenseEditBase.Columns.getTaxRateColumn();
            //arr.push(taxRateColumn);

            //跟踪项
            if (ExpenseEditBase.TrackDataSource != null) {
                for (var i = 0; i < ExpenseEditBase.TrackDataSource.length; i++) {
                    var obj = ExpenseEditBase.getTrackColumn(i);
                    arr.push(obj);
                }
            }
            //金额
            arr.push({
                title: HtmlLang.Write(LangModule.IV, "Amount", "Amount"), field: 'MTaxAmountFor', width: 80, align: 'right', sortable: false, formatter: function (value, rowData, rowIndex) {
                    if (!rowData.MQty || !rowData.MPrice) {
                        return "";
                    }
                    return (value == null || value == "") ? "" : Megi.Math.toMoneyFormat(Math.abs(value));
                }
            });
            //删除行
            if (!ExpenseEditBase.Disabled) {
                arr.push({
                    title: '', field: 'MEntryID', width: 30, height: ExpenseEditBase.EditorHeight, align: 'center', sortable: false, formatter: function (value, rowData, rowIndex) {
                        return "<div class='list-item-action'><a href='javascript:void(0)' class='m-icon-delete-row' onclick='ExpenseEditBase.DeleteItem(this)'>&nbsp;</a></div>";
                    }
                });
            }
            var columns = new Array();
            columns.push(arr);
            return columns;
        },
        updateEditorValue: function (cbox, field, value) {
            var rowObj = $(cbox).closest("td[field='MItemID']").parent();
            $(rowObj).find("td[field='" + field + "']").find("input").val(value);
        },
        updateComboEditorValue: function (cbox, field, text, value) {
            var rowObj = $(cbox).closest("td[field='MItemID']").parent();
            $(rowObj).find("td[field='" + field + "']").find(".combo-text").val(text);
            $(rowObj).find("td[field='" + field + "']").find(".combo-value").val(value);
        },
        //获取费用报销单项列
        getItemColumn: function () {
            var obj = {};
            obj.title = HtmlLang.Write(LangModule.IV, "ExpenseItem", "Expense Item");
            obj.field = 'MItemID';
            obj.width = 100;

            obj.sortable = false;
            obj.editor = {
                type: 'addCombobox',
                options: {
                    type: "expense",
                    addOptions: {
                        //是否有联系人编辑权限
                        hasPermission: ExpenseEditBase.IsCanSettingChangePermission,
                        //弹出框关闭后的回调函数
                        callback: ExpenseEditBase.initInventory
                    },
                    dataOptions: {
                        height: ExpenseEditBase.EditorHeight,
                        //数据加载成功后更新数据源
                        onLoadSuccess: function (msg) {
                            ExpenseEditBase.DataSourceInventory = msg;
                        },
                        onSelect: function (rec) {

                            var comboboxItem = this;

                            //选择的时候判断是否选择的是一级项目
                            mAjax.post("/BD/ExpenseItem/IsParentExpenseItem", { itemId: rec.MItemID }, function (msg) {
                                //如果是一级项目，提示用户不能选择
                                if (msg.Success == true) {
                                    //提示信息
                                    var message = HtmlLang.Write(LangModule.IV, "CanSelectFirstLevelItem", "Sorry,can't choose the first level of expense item");
                                    $.mDialog.alert(message);
                                    //找到当前的编辑框，并将内容清空
                                    var comboValue = $("input[value='" + rec.MItemID + "']");
                                    if (comboValue) {
                                        var comboText = $(comboValue).parent().children("input")[0];
                                        $(comboValue).removeAttr("value");
                                        $(comboText).removeAttr("srcvalue");
                                    }

                                } else {

                                    ExpenseEditBase.Columns.updateEditorValue(comboboxItem, "MDesc", rec.MDesc);
                                    ExpenseEditBase.Columns.updateEditorValue(comboboxItem, "MQty", "1");
                                    ExpenseEditBase.updateDataSourceEntry("MItemID='" + rec.MItemID + "'");
                                    ExpenseEditBase.updateDataSourceEntry('MDesc="' + rec.MDesc + '"');
                                    ExpenseEditBase.updateDataSourceEntry("MQty='1'");

                                    Megi.grid(ExpenseEditBase.Selector, "selectRow", ExpenseEditBase.RowIndex);
                                    var rowObj = Megi.grid(ExpenseEditBase.Selector, "getSelected");
                                    rowObj.MItemID = rec.MItemID;
                                    rowObj.MAmountFor = ExpenseEditBase.DataSourceEntry[ExpenseEditBase.RowIndex].MAmountFor;
                                    rowObj.MApproveAmtFor = ExpenseEditBase.DataSourceEntry[ExpenseEditBase.RowIndex].MApproveAmtFor;

                                    ExpenseEditBase.setContactDefaultTrack(comboboxItem, rowObj);
                                }
                            });
                        }
                    }
                }
            };
            obj.formatter = function (value, rowData, rowIndex) {
                return ExpenseEditBase.getInventoryCode(value);
            }
            return obj;
        },
    },
    //获取跟踪项列
    getTrackColumn: function (index) {
        var obj = {};
        obj.title = ExpenseEditBase.TrackDataSource[index].MName;
        obj.field = "MTrackItem" + (index + 1);
        obj.width = 80;
        obj.sortable = false;
        var trackId = ExpenseEditBase.TrackDataSource[index].MValue;
        var trackName = ExpenseEditBase.TrackDataSource[index].MName;
        obj.editor = {
            type: 'addCombobox',
            options: {
                type: "trackOption" + (index) + Math.random(),
                addOptions: {
                    //是否有基础资料编辑权限
                    hasPermission: ExpenseEditBase.IsCanSettingChangePermission,
                    //关闭后的回调函数
                    callback: function () {
                        ExpenseEditBase.initTrackingOption(trackId, index);
                    },
                    width: 400,
                    height: 400,
					url: "/BD/Tracking/CategoryOptionEdit?trackId=" + trackId + "&tabTitle=" + mDES.encrypt(trackName),
                    itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                    dialogTitle: HtmlLang.Write(LangModule.Common, "NewTrackOption", "New Tracking Option")
                },
                dataOptions: {
                    url: "/BD/Tracking/GetTrackOptionsById?trackId=" + trackId,
                    height: "24px",
                    hideItemKey: "MValue1",
                    hideItemValue: "0",
                    //数据加载成功后更新数据源
                    onLoadSuccess: function (msg) {
                        ExpenseEditBase.TrackDataSource[index].MChildren = msg;
                    },
                    onChange: function (newValue, oldValue) {
                        if (newValue == undefined || newValue == null) {
                            newValue = "";
                        }
                        ExpenseEditBase.updateDataSourceEntry("MTrackItem" + index + "='" + newValue + "'");
                    },
                    valueField: 'MValue',
                    textField: 'MName'

                }
            }
        };
        obj.formatter = function (value, rowData, rowIndex) {
            return ExpenseEditBase.getTrackName(value);
        }
        return obj;
    },
    //index只的那个tracking
    initTrackingOption: function (trackId, index, callback) {
        mAjax.post(
            "/BD/Tracking/GetTrackOptionsById",
            { trackId: trackId },
            function (msg) {
                ExpenseEditBase.TrackDataSource[index].MChildren = msg;
                if (callback) {
                    callback();
                }
            });
    },
    //删除一条明细（根据索引）
    DeleteItem: function (btnObj) {
        var rowIndex = $(btnObj).closest(".datagrid-row").attr("datagrid-row-index");
        var result = ExpenseEditBase.deleteDataSourceEntry(rowIndex);
        if (!result) {
            return;
        }
        Megi.grid(ExpenseEditBase.Selector, "deleteRow", rowIndex);
        ExpenseEditBase.endEditGrid();
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
            var tr_id = $(this).attr("id");
            $(this).attr("id", tr_id.substr(0, tr_id.lastIndexOf('-') - 1) + i);
        });
        ExpenseEditBase.updateDataSourceEntry();
        //明细列表自适应
        IVFW.setUI();
    },
    //添加一条明细
    AddItem: function (btnObj) {
        var rowIndex = Number($(btnObj).closest(".datagrid-row").attr("datagrid-row-index"));
        ExpenseEditBase.AddItemByIndex(rowIndex);
        //明细列表自适应
        IVFW.setUI();
    },
    //插入一条明细（根据索引）
    AddItemByIndex: function (rowIndex) {
        var isLastLine = rowIndex == ExpenseEditBase.DataSourceEntry.length;
        if (!isLastLine) {
            ExpenseEditBase.endEditGrid();
        }
        var newRow = ExpenseEditBase.insertDataSourceEntry(rowIndex);
        if (newRow != null) {
            Megi.grid(ExpenseEditBase.Selector, "insertRow", { index: rowIndex, row: newRow });
            if (!isLastLine) {
                ExpenseEditBase.endEditGrid();
            }
        }
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
        });
        if (!isLastLine) {
            Megi.grid(ExpenseEditBase.Selector, "beginEdit", rowIndex);
            Megi.grid(ExpenseEditBase.Selector, "selectRow", rowIndex);
            ExpenseEditBase.RowIndex = rowIndex;

            var rowData = Megi.grid(ExpenseEditBase.Selector, "getSelected");
            ExpenseEditBase.bindGridEditorTabEvent(rowData, rowIndex);
            ExpenseEditBase.bindGridEditorEvent(rowData, rowIndex);
        }
    },
    //切换币别
    changeCurrency: function (rec) {
        ExpenseEditBase.endEditGrid();
        
        if (rec == undefined) {
            ExpenseEditBase.IsFirstLoad = false;
            return;
        }

        //处理币种变更时变更前币种等于变更后币种时，不修改汇率
        if (!ExpenseEditBase.IsFirstLoad && ExpenseEditBase.IsCurrencyChange && rec.CurrencyID == ExpenseEditBase.MCyID) {
            return;
        }

        var exchangeInfo = "";
        var editExchangeInfo = "";
        var amountTitle = HtmlLang.Write(LangModule.IV, "Amount", "Amount");
        var approveAmountTitle = HtmlLang.Write(LangModule.IV, "ApproveAmount", "审核金额");

        //是否覆盖汇率
        if (ExpenseEditBase.IsCoverExchangeRate) {
            ExpenseEditBase.ExchangeRate = isNaN(parseFloat(rec.MOToLRate)) ? 0 : parseFloat(rec.MOToLRate);
            ExpenseEditBase.MOToLRate = ExpenseEditBase.ExchangeRate;
            ExpenseEditBase.MLToORate = isNaN(parseFloat(rec.MLToORate)) ? 0 : parseFloat(rec.MLToORate);
        }

        if (!rec.IsLocalCy) {
            if (ExpenseEditBase.SourceBillExchangeRate != undefined
                && ExpenseEditBase.SourceBillExchangeRate != 0
                && ExpenseEditBase.SourceBillExchangeRate != 1
                && ExpenseEditBase.IsFirstLoad == true) {
                ExpenseEditBase.ExchangeRate = parseFloat(Megi.Math.toDecimal(ExpenseEditBase.SourceBillExchangeRate, 6));
                ExpenseEditBase.MOToLRate = parseFloat(Megi.Math.toDecimal(ExpenseEditBase.SourceBillOToLRate, 6));
                ExpenseEditBase.MLToORate = parseFloat(Megi.Math.toDecimal(ExpenseEditBase.SourceBillLToORate, 6));
            }
            amountTitle = HtmlLang.Write(LangModule.IV, "Amount", "Amount") + rec.CurrencyID;
            exchangeInfo = "1 " + rec.CurrencyID + " = " + Megi.Math.toDecimal(ExpenseEditBase.ExchangeRate, 6) + " " + rec.MLCurrencyID;
            approveAmountTitle = HtmlLang.Write(LangModule.IV, "ApproveAmount", "审核金额") + rec.CurrencyID;
            editExchangeInfo = '<a href="javascript:void(0)" cyItemId="' + rec.MCyItemID + '" cyId="' + rec.CurrencyID + '" class="m-exchange-edit m-icon-edit" onclick="ExpenseEditBase.editExchangeRate(this)">&nbsp;</a>'
        }

        ExpenseEditBase.MCyID = rec.CurrencyID;
        if (ExpenseEditBase.ExchangeRate == 0) {
            ExpenseEditBase.ExchangeRate = isNaN(parseFloat(rec.MOToLRate)) ? 0 : parseFloat(rec.MOToLRate);
            ExpenseEditBase.MOToLRate = ExpenseEditBase.ExchangeRate;
            ExpenseEditBase.MLToORate = isNaN(parseFloat(rec.MLToORate)) ? 0 : parseFloat(rec.MLToORate);
        }

        //验证 汇率。 只有切换币种 或者 切换时间时，才提示错误。否则只标红
        $("#spExchangeInfo").removeClass("mg-prompt-error");
        if (ExpenseEditBase.ExchangeRate == undefined || ExpenseEditBase.ExchangeRate == "" || ExpenseEditBase.ExchangeRate == 0) {
            $("#spExchangeInfo").addClass("mg-prompt-error");
            ExpenseEditBase.verifyExchangeRate(ExpenseEditBase.BizDate, true);
        }

        $("#spExchangeInfo").html(exchangeInfo);
        if (ExpenseEditBase.IsCanSettingChangePermission && !ExpenseEditBase.Disabled) {
            $("#spEditExchangeRate").html(editExchangeInfo);
        }
        $(".datagrid-header-row").find("td[field='MAmountFor']").find("span").eq(0).html(amountTitle);
        $(".datagrid-header-row").find("td[field='MApproveAmtFor']").find("span").eq(0).html(approveAmountTitle);
        $("#spTotalCurrency").attr("localCyID", rec.MLCurrencyID);
        if (!rec.IsLocalCy) {
            $("#spTotalCurrency").html(ExpenseEditBase.MCyID).show();
        } else {
            $("#spTotalCurrency").hide();
        }

        //更新明细数据源
        for (var i = 0; i < ExpenseEditBase.DataSourceEntry.length; i++) {
            ExpenseEditBase.RowIndex = i;
            var item = ExpenseEditBase.DataSourceEntry[i];
            ExpenseEditBase.updateDataSourceEntry("MPrice='" + item.MPrice + "'");
        }

        ExpenseEditBase.bindGrid();

        //重置回默认值
        ExpenseEditBase.IsCurrencyChange = true;
        ExpenseEditBase.IsFirstLoad = false;
        ExpenseEditBase.IsCoverExchangeRate = true;
    },
    editExchangeRate: function (obj) {
        var cyId = $(obj).attr("cyId");
        ExpenseEditBase.openExchangeRatePage(cyId, ExpenseEditBase.ExchangeRate == 0 ? "" : ExpenseEditBase.ExchangeRate);
    },
    openExchangeRatePage: function (cyId, exchangeRate) {
        var url = '/BD/Currency/ExchangeRateEdit?currencyId=' + cyId + "&exchangeRate=" + exchangeRate;
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "EditExchangeRate", "编辑汇率"),
            width: 400,
            height: 280,
            href: url,
            closeCallback: [function (ops) {
                if (ops) {
                    //设置非币种切换
                    ExpenseEditBase.IsCurrencyChange = false;
                    ExpenseEditBase.changeCurrency(ops);
                }
            }]
        });
    },
    resetCurrency: function (dateTime) {
        if (ExpenseEditBase.BizDate == dateTime) {
            return;
        }
        ExpenseEditBase.BizDate = dateTime;
        var obj = eval("(" + "{" + $("#selCurrency").attr("data-options") + "}" + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return;
        }
        var currencyId = $("#selCurrency").combobox("getValue");
        //当前币种为空时，不处理
        if (!currencyId) {
            return;
        }
        mAjax.post(
            "/BD/Currency/GetCurrencyDataOpstion",
            { endDate: dateTime },
            function (msg) {
                var opts = eval("({" + msg + "})");
                opts.onSelect = ExpenseEditBase.changeCurrency;
                $("#selCurrency").combobox(opts);

                if (!ExpenseEditBase.isCurrencyExists(currencyId)) {
                    $("#spExchangeInfo").html("");
                    $("#spEditExchangeRate").html("");
                }
                if (ExpenseEditBase.IsCurrencyDisabled) {
                    $('#selCurrency').combobox('disable');
                }

                //设置非币种切换
                ExpenseEditBase.IsCurrencyChange = false;
                //查找取回来的 汇率是否和单据上面的汇率相等
                if (ExpenseEditBase.isExchangeRateChange(currencyId, ExpenseEditBase.MLToORate)) {
                    var tips = HtmlLang.Write(LangModule.IV, "ConfirmCoverExchangeRate", "重新选择日期会影响汇率，是否需要重新获取汇率？");
                    $.mDialog.confirm(tips, function () {
                        ExpenseEditBase.IsCoverExchangeRate = true;
                        $("#selCurrency").combobox("select", currencyId);
                    }, function () {
                        ExpenseEditBase.IsCoverExchangeRate = false;
                        $("#selCurrency").combobox("select", currencyId);
                    });
                }
                else {
                    $("#selCurrency").combobox("select", currencyId);
                }
            });
    },
    //获取本位币ID
    getOrgCurrencyID: function () {
        var obj = eval("(" + "{" + $("#selCurrency").attr("data-options") + "}" + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return "";
        }
        return obj.data[0].SrcCurrencyID;
    },
    //获取当前币别信息
    getCurrency: function () {
        var obj = eval("(" + "{" + $("#selCurrency").attr("data-options") + "}" + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return "";
        }
        var selectedValue = $("#selCurrency").combobox("getValue");
        for (var i = 0; i < obj.data.length; i++) {
            if (obj.data[i].CurrencyID == selectedValue) {
                return obj.data[i];
            }
        }
        return null;
    },
    //切换联系人
    changeContact: function (contactId, callback) {
        ExpenseEditBase.GetContactInfo(contactId, function (msg) {
            ExpenseEditBase.setDefaultCurrency(msg.MDefaultCyID);
            ExpenseEditBase.setDefaultDueDate();
            if (callback != undefined) {
                callback(msg);
            }
        });
    },
    //获取联系人并设置联系人数据源
    GetContactInfo: function (contactId, callback) {
        mAjax.post(
            "/BD/Employees/GetEmployeeInfo/",
            { employeeId: contactId },
            function (msg) {
                ExpenseEditBase.ContactInfoDataSource = msg;
                if (callback != undefined) {
                    callback(msg);
                }
            });
    },
    //设置默认币别
    setDefaultCurrency: function (defaultCyId) {
        var obj = $("#selCurrency").attr("data-options");
        obj = "{" + obj + "}";
        obj = eval("(" + obj + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return;
        }
        for (var i = 0; i < obj.data.length; i++) {
            if (obj.data[i].CurrencyID == defaultCyId) {
                $("#selCurrency").combobox("select", defaultCyId);
                return;
            }
        }
        $("#selCurrency").combobox("select", obj.data[0].CurrencyID);
    },
    //获取跟踪项索引
    getTrackIndex: function (contactTrackItemId) {
        if (ExpenseEditBase.TrackDataSource == null) {
            return -1;
        }
        var index = -1;
        for (var i = 0; i < ExpenseEditBase.TrackDataSource.length; i++) {
            if (ExpenseEditBase.TrackDataSource[i].MChildren != null) {
                for (var k = 0; k < ExpenseEditBase.TrackDataSource[i].MChildren.length; k++) {
                    if (contactTrackItemId == ExpenseEditBase.TrackDataSource[i].MChildren[k].MValue) {
                        index = i + 1;
                        break;
                    }
                }
            }
            if (index > -1) {
                break;
            }
        }
        return index;
    },
    //设置联系人默认的跟踪项
    setContactDefaultTrack: function (cbox, rowObj) {
        if (ExpenseEditBase.ContactInfoDataSource == null) {
            return;
        }
        var item1 = "";
        var item2 = "";
        var item3 = "";
        var item4 = "";
        var item5 = "";
        var index1 = -1;
        var index2 = -1;
        var index3 = -1;
        var index4 = -1;
        var index5 = -1;
        var value1 = "";
        var value2 = "";
        var value3 = "";
        var value4 = "";
        var value5 = "";
        if (ExpenseEditBase.ContactTrackType == "Sale") {
            index1 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem1);
            index2 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem2);
            index3 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem3);
            index4 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem4);
            index5 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem5);
            value1 = ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem1;
            value2 = ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem2;
            value3 = ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem3;
            value4 = ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem4;
            value5 = ExpenseEditBase.ContactInfoDataSource.MSaleTrackItem5;
        }
        else if (ExpenseEditBase.ContactTrackType == "Pur") {
            index1 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem1);
            index2 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem2);
            index3 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem3);
            index4 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem4);
            index5 = ExpenseEditBase.getTrackIndex(ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem5);
            value1 = ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem1;
            value2 = ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem2;
            value3 = ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem3;
            value4 = ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem4;
            value5 = ExpenseEditBase.ContactInfoDataSource.MPurchaseTrackItem5;
        }
        for (var i = 1; i < 6; i++) {
            var index = eval("index" + i);
            if (index > -1) {
                eval("item" + index + "=value" + i);
            }
        }

        ExpenseEditBase.updateDataSourceEntry("MTrackItem1='" + item1 + "'");
        ExpenseEditBase.updateDataSourceEntry("MTrackItem2='" + item2 + "'");
        ExpenseEditBase.updateDataSourceEntry("MTrackItem3='" + item3 + "'");
        ExpenseEditBase.updateDataSourceEntry("MTrackItem4='" + item4 + "'");
        ExpenseEditBase.updateDataSourceEntry("MTrackItem5='" + item5 + "'");


        ExpenseEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem1", ExpenseEditBase.getTrackName(item1), item1);
        ExpenseEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem2", ExpenseEditBase.getTrackName(item2), item2);
        ExpenseEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem3", ExpenseEditBase.getTrackName(item3), item3);
        ExpenseEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem4", ExpenseEditBase.getTrackName(item4), item4);
        ExpenseEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem5", ExpenseEditBase.getTrackName(item5), item5);
    },
    //结束编辑列表
    endEditGrid: function () {
        var recordLength = Megi.grid(ExpenseEditBase.Selector, "getRows").length;
        for (var i = 0; i < recordLength; i++) {
            Megi.grid(ExpenseEditBase.Selector, "endEdit", i);
        }
    },
    //打开日志对话框
    OpenHistoryDialog: function (invoiceId, reportType) {
        var title = $.trim(reportType.replace(/([A-Z])/g, ' $1'));
        Megi.dialog({
            title: HtmlLang.Write(LangModule.Common, "History", "History"),
            top: window.pageYOffset || document.documentElement.scrollTop,
            width: 800,
            height: 455,
            href: "/Log/Log/BusLogList?invoiceId=" + invoiceId + "&billType=Expense_Claims"
        });
    },
    changeDate: function () {
        var billDate = $('#txtDate').datebox('getValue');
        var billDate = $('#txtDate').datebox('getValue');
        if (!ExpenseEditBase.IsFirstLoad) {
            ExpenseEditBase.resetCurrency(billDate);
        }
    }
}