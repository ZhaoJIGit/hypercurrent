/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="BillUrlHelper.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;    
var IVEditBase = {
    EditorHeight: "26px",
    InvoiceID: "",
    IVCurrentType: "sle",
    Selector: "#tbInvoiceDetail",
    Disabled: false,
    Verification: null,
    IsShowItem: true,
    DataSourceItem: [],
    //是否有 (发票，付款单，收款单) 编辑权限
    HasChangeAuth: false,
    //是否有 银行账号 查看权限
    IsCanBankAccountViewPermission: false,
    //是否有 银行账号 编辑权限
    IsCanBankAccountChangePermission: false,
    //是否有 基础资料 编辑权限
    IsCanSettingChangePermission: false,
    //是否有 联系人 编辑权限
    IsCanContactChangePermission: false,
    IsFirstLoad: true,//是否是第一次加载
    IsCurrencyChange: true,//是否切换币种 默认true,
    IsCoverExchangeRate: true,//是否覆盖汇率
    DataSourceEntry: [],
    DataSourceTaxRate: [],
    TrackDataSource: [],
    DepartmentDataSource: [],
    ContactInfoDataSource: [],
    ContactTrackType: "",
    BizType: "",//业务类型
    BizObject: "",//业务对象
    ContactType: "",//联系人类型
    RowIndex: -1,//当前编辑行索引
    MCyID: 'CNY', //币种
    ExchangeRate: 1,//当前税率
    MOToLRate: 1,//原币到本位币汇率 1原币=?本位币
    MLToORate: 1,//本位币到原币汇率 1本位币?原币
    CurrenCurrency: {},
    SourceBillExchangeRate: 1,//原单据的汇率
    SourceBillOToLRate: 1,
    SourceBillLToORate: 1,
    IsCurrencyDisabled: false,
    IVType: { Sale: "sle", Purchase: "pur" },
    CurrentStatus: 1,
    BizDate: '',
    init: function (opts) {
        //opts对象  此处注释代码不要删除
        //{
        //    IVType: IVEditBase.IVType.Sale, //类型，主要是判断是销售还是采购
        //    id: BillEditBase.InvoiceID, //单据ID
        //    oToLRate: msg.MOToLRate,  //原币到本位币的汇率
        //    lToORate: msg.MLToORate, //本位币到原币的汇率
        //    verificationList: msg.Verification, //核销列表
        //    entryList: msg.InvoiceEntry, //分录列表
        //    cyId: msg.MCyID, //币别
        //    taxId: msg.MTaxID, //税率类型
        //    disabled: isDisabled, //是否禁止编辑
        //    currencyDisabled: false, //币别是否允许编辑
        //    type: msg.MType, //单据类型
        //    status: msg.MStatus //单据状态
        //    callback: //回调函数
        //}


        IVEditBase.MOToLRate = opts.oToLRate;
        IVEditBase.MLToORate = opts.lToORate;

        IVEditBase.SourceBillExchangeRate = opts.oToLRate;
        IVEditBase.SourceBillOToLRate = opts.oToLRate;
        IVEditBase.SourceBillLToORate = opts.lToORate;
        IVEditBase.HasChangeAuth = $("#hidChangeAuth").val() == "1" ? true : false;
        IVEditBase.IsCanBankAccountViewPermission = $("#hidIsCanBankAccountViewPermission").val() == "1" ? true : false;
        IVEditBase.IsCanBankAccountChangePermission = $("#hidIsCanBankAccountChangePermission").val() == "1" ? true : false;
        IVEditBase.IsCanSettingChangePermission = $("#hidIsCanSettingChangePermission").val() == "1" ? true : false;
        IVEditBase.IsCanContactChangePermission = $("#hidIsCanContactChangePermission").val() == "1" ? true : false;

        if (opts.disabled == undefined) {
            opts.disabled = false;
        }
        IVEditBase.IVCurrentType = opts.IVType;
        IVEditBase.BizType = opts.type;
        IVEditBase.BizDate = opts.bizDate;
        IVEditBase.IsCurrencyDisabled = opts.currencyDisabled;
        //felson 2016.7.12
        //var track = $("#hidTrack").val();
        IVEditBase.TrackDataSource = mData.getNameValueTrackDataList();

        IVEditBase.Verification = opts.verificationList;
        Megi.ComboBox.setDefaultValue("#selCurrency");
        var localCyId = $('#selCurrency').combobox("getValue");
        Megi.ComboBox.setDefaultValue("#selTaxType");
        if (opts.currencyDisabled == true) {
            $('#selCurrency').combobox('disable');
        }

        IVEditBase.Disabled = opts.disabled;
        if (opts.disabled) {
            IVEditBase.IsCurrencyDisable = true;
            $('#selCurrency').combobox('disable');
            $('#selTaxType').combobox('disable');
            $('#aNewLine').remove();
            $(".form-invoice-total").addClass("form-invoice-total-disabled");
        }

        IVEditBase.InvoiceID = opts.id;
        IVEditBase.CurrentStatus = opts.status,

        IVEditBase.initTaxRate();

        //只有“付款单”和“付款单”的时候需要加载部门数据源
        IVEditBase.BizObject = $("#hidBizObject").val();

        if (opts.taxId != null) {
            $('#selTaxType').combobox('setValue', opts.taxId);
        }

        //初始化商品下拉框数据源
        IVEditBase.initItem(function () {
            //初始化税率下拉框数据源
            IVEditBase.initTaxRate(function () {
                //等以上数据源都加载完后，再初始化明细列表（防止初始化明细列表时，下拉框没有数据而导致数据不显示）
                IVEditBase.setDataSourceEntry(opts.entryList, true);
                IVEditBase.bindGrid();
                if (opts.cyId != null && opts.cyId != "-1") {
                    //币别是否存在，如果存在，则下拉选择，否则只显示
                    var isCyExists = IVEditBase.isCurrencyExists(opts.cyId);
                    
                    if (isCyExists == true) {
                        $('#selCurrency').combobox('select', opts.cyId);
                    }
                    else {
                        IVEditBase.getCurrencyInfo(opts.cyId, function (info) {
                            $('#selCurrency').combobox('setValue', info.MCurrencyID);
                            $('#selCurrency').combobox('setText', info.MCurrencyName);
                            $("#selCurrency").attr("billCyID", info.MCurrencyID);
                            $("#selCurrency").attr("billExchangeRate", opts.MOToLRate);
                            IVEditBase.changeCurrency({
                                MOToLRate: opts.MOToLRate,
                                MLToORate: opts.MLToORate,
                                CurrencyID: opts.cyId,
                                CurrencyName: info.MCurrencyName,
                                MLCurrencyID: localCyId,
                                MCyItemID: '',
                                IsLocalCy: opts.cyId == localCyId
                            });
                        });
                    }
                }
                IVEditBase.updateDataSourceEntry();
                if (opts.callback != undefined) {
                    opts.callback();
                }

                //明细列表自适应
                IVFW.setUI();

                //将当前页面设置为稳定状态
                $.mTab.setStable();
            });
        });
        //将当前页面设置为稳定状态
        $.mTab.setStable();
        //焦点移开分录结束编辑
        $(".m-imain,.m-toolbar-footer").off("click.endEdit").on("click.endEdit", function (e) {
            IVEditBase.endEditGrid();
        });
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
    valideInfo: function (invoiceType) {
        var result = true;
        var taxType = IVEditBase.getTaxType();
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            var item = IVEditBase.DataSourceEntry[i];
            //备注去左右空格
            item.MDesc = $.trim(item.MDesc);
            //如果所有列都为空则跳过
            if ((item.MItemID == undefined || item.MItemID == null || item.MItemID == "")
                && (item.MDesc == undefined || item.MDesc == null || item.MDesc == "")
                && item.MQty == ""
                && item.MPrice == ""
                && item.MDiscount == ""
                && (item.MTaxID == undefined || item.MTaxID == null || item.MTaxID == "" || item.MTaxID == "None")
                && (item.MDepartment == undefined || item.MDepartment == null || item.MDepartment == "")
                && (item.MTrackItem1 == undefined || item.MTrackItem1 == null || item.MTrackItem1 == "")
                && (item.MTrackItem2 == undefined || item.MTrackItem2 == null || item.MTrackItem2 == "")
                && (item.MTrackItem3 == undefined || item.MTrackItem3 == null || item.MTrackItem3 == "")
                && (item.MTrackItem4 == undefined || item.MTrackItem4 == null || item.MTrackItem4 == "")
                && (item.MTrackItem5 == undefined || item.MTrackItem5 == null || item.MTrackItem5 == "")) {
                continue;
            }
            if (item.MDesc == undefined || item.MDesc == null || item.MDesc == "") {
                $(IVEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MDesc']").addClass("row-error");
                result = false;
            }
            //价格可以为0， 保证总价>0就可以了
            if (item.MPrice == "" || item.MPrice == 0) {
                $(IVEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MPrice']").addClass("row-warning");
            }

            if (item.MQty == "" || item.MQty == 0) {
                $(IVEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MQty']").addClass("row-error");
                result = false;
            }
            //如果税率类型为含税，才需要验证税率是否为空，否则不需要验证
            if (taxType != 2) {
                if (invoiceType != undefined && (invoiceType == IVEditBase.IVType.Sale || invoiceType == IVEditBase.IVType.Purchase)) {
                    if (item.MTaxID == undefined || item.MTaxID == null || item.MTaxID == "" || item.MTaxID == "None") {
                        $(IVEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MTaxID']").addClass("row-error");
                        result = false;
                    }
                }
            }
        }
        return result;
    },
    getInfo: function (withId) {
        IVEditBase.endEditGrid();
        return IVEditBase.getViewInfo();
    },
    getDescList: function () {
        IVEditBase.endEditGrid();
        IVEditBase.updateDataSourceEntry();
        var arr = new Array();
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            var item = IVEditBase.DataSourceEntry[i];
            //备注去左右空格
            item.MDesc = $.trim(item.MDesc);
            //如果所有列都为空则跳过
            if ((item.MItemID == undefined || item.MItemID == null || item.MItemID == "")
                && (item.MDesc == undefined || item.MDesc == null || item.MDesc == "")
                && item.MQty == ""
                && item.MPrice == ""
                && item.MDiscount == ""
                && (item.MTaxID == undefined || item.MTaxID == null || item.MTaxID == "" || item.MTaxID == "None")
                && (item.MDepartment == undefined || item.MDepartment == null || item.MDepartment == "")
                && (item.MTrackItem1 == undefined || item.MTrackItem1 == null || item.MTrackItem1 == "")
                && (item.MTrackItem2 == undefined || item.MTrackItem2 == null || item.MTrackItem2 == "")
                && (item.MTrackItem3 == undefined || item.MTrackItem3 == null || item.MTrackItem3 == "")
                && (item.MTrackItem4 == undefined || item.MTrackItem4 == null || item.MTrackItem4 == "")
                && (item.MTrackItem5 == undefined || item.MTrackItem5 == null || item.MTrackItem5 == "")) {
                continue;
            }
            arr.push(item);
        }
        return arr;
    },
    getViewInfo: function (withId) {
        IVEditBase.updateDataSourceEntry();
        var taxId = $('#selTaxType').combobox('getValue');
        var currencyId = $('#selCurrency').combobox('getValue');

        var totalAmtFor = 0;
        var totalAmt = 0;
        var taxTotalAmtFor = 0;
        var taxTotalAmt = 0;
        var obj = {};
        var arr = new Array();
        var taxType = IVEditBase.getTaxType();
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            var item = IVEditBase.DataSourceEntry[i];
            //排序
            item.MSeq = i + 1;
            //备注去左右空格
            item.MDesc = $.trim(item.MDesc);
            //如果所有列都为空则跳过
            if ((item.MItemID == undefined || item.MItemID == null || item.MItemID == "")
                && (item.MDesc == undefined || item.MDesc == null || item.MDesc == "")
                && item.MQty == ""
                && item.MPrice == ""
                && item.MDiscount == ""
                && (item.MTaxID == undefined || item.MTaxID == null || item.MTaxID == "" || item.MTaxID == "None")
                && (item.MDepartment == undefined || item.MDepartment == null || item.MDepartment == "")
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
            if (!IVEditBase.isEmployees()) {
                item.MDepartment = "";
            }
            arr.push(item);
            totalAmtFor += parseFloat(Math.abs(item.MAmountFor));
            totalAmt += parseFloat(Math.abs(item.MAmount));
            if (taxType == 0) {
                taxTotalAmtFor += parseFloat(Math.abs(item.MTaxAmountFor));
                taxTotalAmt += parseFloat(Math.abs(item.MTaxAmount));
            }
            else {
                taxTotalAmtFor += parseFloat(Math.abs(item.MAmountFor));
                taxTotalAmt += parseFloat(Math.abs(item.MAmount));
            }
        }
        obj.InvoiceEntry = arr;
        obj.MCyID = currencyId;
        obj.MTaxID = taxId;
        obj.MTotalAmtFor = totalAmtFor;
        obj.MTotalAmt = totalAmt;
        obj.MTaxTotalAmtFor = taxTotalAmtFor;
        obj.MTaxTotalAmt = taxTotalAmt;
        obj.MExchangeRate = IVEditBase.ExchangeRate;
        obj.MOToLRate = IVEditBase.MOToLRate;
        obj.MLToORate = IVEditBase.MLToORate;

        return obj;
    },
    initItem: function (callback) {
        if (IVEditBase.isExpenseItem()) {
            IVEditBase.initExpense(callback);
        } else {
            IVEditBase.initInventory(callback);
        }
    },
    //初始化物料信息
    initInventory: function (callback) {
        var params = {};
        params.IncludeDisable = true;
        mAjax.post(
            "/BD/Item/GetItemList", { filter: params }, function (msg) {
                if (msg != null && msg.length > 0) {
                    for (var i = 0; i < msg.length; i++) {
                        msg[i].MText = msg[i].MNumber + ":" + msg[i].MDesc;
                    }
                }
                IVEditBase.DataSourceItem = msg;
                if (callback) {
                    callback();
                }
            });
    },
    initExpense: function (callback) {
        var params = {};
        params.IncludeDisable = true;
        mAjax.post(
            "/BD/ExpenseItem/GetExpenseItemListByTier",
            { includeDisable: true }, function (msg) {
                if (msg != null && msg.length > 0) {
                    for (var i = 0; i < msg.length; i++) {
                        //msg[i].MText = msg[i].MName + ":" + msg[i].MDesc;
                        msg[i].MText = msg[i].MName;
                    }
                }
                IVEditBase.DataSourceItem = msg;
                if (callback) {
                    callback();
                }
            });
    },
    removeDataSourceEntryItem: function () {
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            IVEditBase.DataSourceEntry[i].MItemID = "";
        }
    },
    //更新数据源
    updateDataSourceEntry: function (keyValue, value) {
        var taxType = IVEditBase.getTaxType();
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            if (taxType == 2) {
                IVEditBase.DataSourceEntry[i].MTaxID = "";
            }
            if (IVEditBase.DataSourceEntry[i].RowIndex == IVEditBase.RowIndex) {
                if (keyValue != undefined) {
                    if (keyValue == "MDesc" && value != undefined) {
                        IVEditBase.DataSourceEntry[i].MDesc = value;
                    } else {
                        eval("IVEditBase.DataSourceEntry[i]." + keyValue);
                    }
                }
            }
        }
        IVEditBase.updateSummaryInfo();
    },
    //获取行数据的汇总
    getAmtFor: function (rowIndex) {
        var vQty = IVEditBase.DataSourceEntry[rowIndex].MQty;
        var vPrice = IVEditBase.DataSourceEntry[rowIndex].MPrice;

        vQty = String(vQty).replace(new RegExp(",", "g"), "");
        vPrice = String(vPrice).replace(new RegExp(",", "g"), "");
        var vDiscount = IVEditBase.DataSourceEntry[rowIndex].MDiscount;
        var amtFor = 0;
        var isAbs = false;
        if (vQty != undefined && vQty != "" && vPrice != undefined && vPrice != "") {
            isAbs = parseFloat(vPrice) < 0 ? false : true;//价格为负数返回负数
            //前台的行金额逻辑有误，对于单价应该是先四舍五入再相乘得到行金额
            //数量4位，单价8位
            amtFor = Megi.Math.toDecimal(vQty, 4) * Megi.Math.toDecimal(vPrice, 8);
            if (vDiscount != undefined && vDiscount != 'undefined' && vDiscount != "") {
                amtFor = amtFor * (1 - parseFloat(vDiscount) / 100);
            }
        }
        amtFor = isAbs ? Math.abs(amtFor) : amtFor;
        amtFor = Megi.Math.toDecimal(amtFor, 2);
        return parseFloat(amtFor);
    },
    //更新合计信息
    updateSummaryInfo: function () {
        var amtFor = 0;
        var taxAmtFor = 0;
        var taxAmt = 0;

        var arrTax = new Array();

        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            var item = IVEditBase.DataSourceEntry[i];
            if (item.MAmountFor != "") {
                amtFor += parseFloat(item.MAmountFor);
                taxAmtFor += parseFloat(item.MTaxAmountFor);
                taxAmt += parseFloat(item.MTaxAmount);

                var taxRateName = IVEditBase.getTaxRateName(item.MTaxID);
                if ((item.MTaxID == undefined || item.MTaxID == null || item.MTaxID == "null" || item.MTaxID == "" || taxRateName == "")
                    && !item.MTaxAmtFor) {
                    continue;
                }
                var objTax = {};
                objTax.MTaxID = item.MTaxID;
                objTax.MTaxAmtFor = item.MTaxAmtFor;
                if (IVEditBase.isEmployees()) {
                    objTax.MName = "";
                } else {
                    objTax.MName = taxRateName;
                }
                if (arrTax.length == 0) {
                    arrTax.push(objTax);
                } else {
                    var isExist = false;
                    for (var k = 0; k < arrTax.length; k++) {
                        if (arrTax[k].MTaxID == item.MTaxID) {
                            arrTax[k].MTaxAmtFor = parseFloat(arrTax[k].MTaxAmtFor) + parseFloat(item.MTaxAmtFor);
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist) {
                        arrTax.push(objTax);
                    }
                }
            }
        }

        if (amtFor == 0) {
            amtFor = "0.00";
            taxAmtFor = "0.00";
        }
        taxAmtFor = Math.abs(taxAmtFor);

        $("#spSubTotal").html(Megi.Math.toMoneyFormat(amtFor, 2));
        var taxType = IVEditBase.getTaxType();
        switch (taxType) {
            case 0:
                IVEditBase.updateTaxSummaryInfo(arrTax);
                $("#spTotal").html(Megi.Math.toMoneyFormat(taxAmtFor, 2));
                IVEditBase.updateVerification(taxAmtFor);
                break;
            case 1:
                IVEditBase.updateTaxSummaryInfo(arrTax);
                $("#spTotal").html(Megi.Math.toMoneyFormat(amtFor, 2));
                IVEditBase.updateVerification(amtFor);
                break;
            case 2:
                IVEditBase.updateTaxSummaryInfo(null);
                $("#spTotal").html(Megi.Math.toMoneyFormat(amtFor, 2));
                IVEditBase.updateVerification(amtFor);
                break;
        }
        IVEditBase.setTotalAmtTip(taxAmt);
    },
    updateTaxSummaryInfo: function (arrTax) {
        var html = '';
        if (arrTax != null && arrTax.length > 0) {
            var taxType = IVEditBase.getTaxType();
            var taxTypeText = "";
            if (taxType == 0) {
                taxTypeText = HtmlLang.Write(LangModule.IV, "Total", "Total");
            } else if (taxType == 1) {
                taxTypeText = HtmlLang.Write(LangModule.IV, "Includes", "Includes");
            }
            for (var i = 0; i < arrTax.length; i++) {
                if (arrTax[i].MName != null) {
                    var taxFor = arrTax[i].MTaxAmtFor;
                    if (isNaN(taxFor) || taxFor == "" || taxFor == undefined) {
                        taxFor = 0;
                    }
                    var taxFor = Megi.Math.toMoneyFormat(Math.abs(taxFor), 2);
                    var taxName = mText.encode(arrTax[i].MName);

                    if (!IVEditBase.isEmployees() && taxName) {
                        taxName = "&nbsp;&nbsp;&nbsp;" + taxName;
                    }
                    html += '<div class="tax"><span class="mg-total-text">' + taxTypeText + taxName + '</span><span class="mg-total-value">' + taxFor + '</span></div>';
                }
            }
        }
        $("#divTax").html(html);
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
    updateVerification: function (amtFor) {
        if (IVEditBase.Verification == null || IVEditBase.Verification.length == 0) {
            $("#divCredit").html("");
            return;
        }
        var html = "";
        var totalVerificationAmt = 0;
        for (var i = 0; i < IVEditBase.Verification.length; i++) {



            var lastStyle = "";
            if (i == IVEditBase.Verification.length - 1) {
                lastStyle = "style='border-bottom:none;'";
            }
            var item = IVEditBase.Verification[i];
            var verifiText = HtmlLang.Write(LangModule.Common, item.MBizType, item.MBizType);

            if (item.MBizBillType == "Payment" || item.MBizBillType == "Receive") {
                verifiText = HtmlLang.Write(LangModule.Common, item.MBizBillType, item.MBizBillType);
            }
            var less = "<label style='color:#888;'>" + HtmlLang.Write(LangModule.IV, "Less", "Less") + "</label> ";
            verifiText = less + verifiText;

            if (item.MBizType == "Invoice_Sale" || item.MBizType == "Pay_Salary") {
                verifiText = verifiText + " " + item.MBillNo;
            }

            totalVerificationAmt += Math.abs(item.MHaveVerificationAmtFor);
            //是否有银行查看权限updateVerification
            if (IVEditBase.IsCanBankAccountViewPermission) {
                //是否有银行编辑权限
                if (IVEditBase.IsCanBankAccountChangePermission) {
                    if (item.IsMergePay) {
                        //html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span> <a href="javascript:void(0)" onclick="IVEditBase.deleteVerification(\'' + item.VerificationID + '\', this,\'' + item.IsMergePay + '\')" class="m-icon-delete">&nbsp;</a><div class="clear"></div></div>';
                        html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" billId=\'' + item.MBillID + '\' bizType=\'' + item.MBizType + '\' ref=\'' + $.mDate.format(item.MBizDate) + '\' onclick="IVEditBase.ViewVerificationDetail(\'' + item.MBillID + '\',\'MergePay\',\'' + $.mDate.format(item.MBizDate) + '\')"><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><a href="javascript:void(0)" onclick="IVEditBase.deleteVerification(\'' + item.VerificationID + '\', this,\'' + item.IsMergePay + '\')" class="m-icon-delete">&nbsp;</a><div class="clear"></div></div>';
                    } else {
                        html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" billId=\'' + item.MBillID + '\' bizType=\'' + item.MBizType + '\' onclick="IVEditBase.ViewVerificationDetail(\'' + item.MBillID + '\',\'' + item.MBizType + '\')"><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><a href="javascript:void(0)" onclick="IVEditBase.deleteVerification(\'' + item.VerificationID + '\', this,\'' + item.IsMergePay + '\')" class="m-icon-delete">&nbsp;</a><div class="clear"></div></div>';
                    }
                } else {
                    if (item.IsMergePay) {
                        //html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span><div class="clear"></div></div>';
                        html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" onclick="IVEditBase.ViewVerificationDetail(\'' + item.MBillID + '\',\'MergePay\',\'' + $.mDate.format(item.MBizDate) + '\')"><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><div class="clear"></div></div>';
                    } else {
                        html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" onclick="IVEditBase.ViewVerificationDetail(\'' + item.MBillID + '\',\'' + item.MBizType + '\')"><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><div class="clear"></div></div>';
                    }

                }
            } else {
                if (IVEditBase.IsCanBankAccountChangePermission) {
                    html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + HtmlLang.Write(LangModule.Common, item.MBizType, item.MBizType) + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span><a href="javascript:void(0)" onclick="IVEditBase.deleteVerification(\'' + item.VerificationID + '\', this,\'' + item.IsMergePay + '\')" class="m-icon-delete">&nbsp;</a><div class="clear"></div></div>';
                } else {
                    html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + HtmlLang.Write(LangModule.Common, item.MBizType, item.MBizType) + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span><div class="clear"></div></div>';
                }
            }
        }
        var amountDue = amtFor - totalVerificationAmt;
        html += ' <div class="due-total"><span class="mg-total-text">' + HtmlLang.Write(LangModule.IV, "AmountDue", "Amount Due") + '</span><span class="mg-total-value" id="spTotal">' + Megi.Math.toMoneyFormat(amountDue) + '</span><span class="mg-total-currency"></span></div>';

        $("#divCredit").html(html);

        //如果核销单据日期为空，则垂直居中显示
        if (!$.mDate.format(item.MBizDate)) {
            $("#divCredit .mg-total-text, #divCredit .mg-total-value").css({ "line-height": "40px" });
            $("#divCredit .m-icon-delete").height(35);
        }
    },
    //查看核销的详细信息
    ViewVerificationDetail: function (billId, bizType, ref) {
        BillUrl.open({ BillID: billId, BillType: bizType, Ref: ref });
    },
    deleteVerification: function (verificationId, sender, isMergePay) {
        var content = HtmlLang.Write(LangModule.IV, "DeleteReconcileRelationshipConfirm", "Are you sure to delete the reconcile relationship.");
        var prevA = $(sender).prev();
        var billId = prevA.attr("billId");
        var bizType = prevA.attr("bizType");
        $.mDialog.confirm(content, null, function () {
            mAjax.post(
                "/IV/Verification/DeleteVerification/",
                { verificationId: verificationId, isMergePay: isMergePay },
                function (msg) {
                    var messsage = '';
                    if (msg.Success) {
                        $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteReconcileRelationshipSuccessfully", "Delete reconcile relationship successfully!"));
                        mWindow.reload();
                        if (billId && bizType) {
                            BillUrl.open({ BillID: billId, BillType: bizType }, true);
                        }
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
        for (var i = 0; i < IVEditBase.DataSourceItem.length; i++) {
            if (IVEditBase.DataSourceItem[i].MItemID == itemId) {
                return IVEditBase.DataSourceItem[i].MNumber;
            }
        }
        return "";
    },
    //获取费用项目名称
    getExpenseName: function (itemId) {
        for (var i = 0; i < IVEditBase.DataSourceItem.length; i++) {
            if (IVEditBase.DataSourceItem[i].MItemID == itemId) {
                return IVEditBase.DataSourceItem[i].MName;
            }
        }
        return "";
    },
    //index只的那个tracking
    initTrackingOption: function (trackId, index, callback) {
        mAjax.post("/BD/Tracking/GetTrackOptionsById", { trackId: trackId }, function (msg) {
            IVEditBase.TrackDataSource[index].MChildren = msg;
            if (callback) {
                callback();
            }
        });
    },
    getTrackName: function (value) {
        if (IVEditBase.TrackDataSource == undefined || IVEditBase.TrackDataSource == null) {
            return "";
        }
        for (var i = 0; i < IVEditBase.TrackDataSource.length; i++) {
            if (IVEditBase.TrackDataSource[i].MChildren != null) {
                for (var k = 0; k < IVEditBase.TrackDataSource[i].MChildren.length; k++) {
                    if (value == IVEditBase.TrackDataSource[i].MChildren[k].MValue) {
                        return IVEditBase.TrackDataSource[i].MChildren[k].MName;
                    }
                }
            }
        }
        return "";
    },
    //初始化税率数据源
    initTaxRate: function (callback) {
        mAjax.post("/BD/TaxRate/GetTaxRateList", null, function (msg) {
            IVEditBase.DataSourceTaxRate = msg;
            if (callback) {
                callback();
            }
        });
    },
    //获取税率名称
    getTaxRateName: function (taxRateId) {
        for (var i = 0; i < IVEditBase.DataSourceTaxRate.length; i++) {
            if (IVEditBase.DataSourceTaxRate[i].MItemID == taxRateId) {
                return IVEditBase.DataSourceTaxRate[i].MText;
            }
        }
        return "";
    },
    //获取部门名称
    getDepartmentName: function (departmentId) {
        for (var i = 0; i < IVEditBase.DepartmentDataSource.length; i++) {
            if (IVEditBase.DepartmentDataSource[i].id == departmentId) {
                return IVEditBase.DepartmentDataSource[i].name;
            }
        }
        return "";
    },
    //获取税率
    getTaxRate: function (taxRateId) {
        if (taxRateId == null || taxRateId == "") {
            return 0;
        }
        for (var i = 0; i < IVEditBase.DataSourceTaxRate.length; i++) {
            if (IVEditBase.DataSourceTaxRate[i].MItemID == taxRateId) {
                return parseFloat(IVEditBase.DataSourceTaxRate[i].MEffectiveTaxRateDecimal);
            }
        }
        return 0;
    },
    //检查税率是否存在
    checkTaxRate: function (taxRateId) {
        if (!taxRateId) {
            return false;
        }
        var isExists = false;
        for (var i = 0; i < IVEditBase.DataSourceTaxRate.length; i++) {
            if (IVEditBase.DataSourceTaxRate[i].MItemID == taxRateId) {
                isExists = true;
                break;
            }
        }
        return isExists;
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
                var obj = IVEditBase.getEmptyEntry();
                obj.RowIndex = rowIndex;
                entryDataSource.push(obj);
                rowIndex += 1;
            }
        }
        //谭勇
        IVEditBase.DataSourceEntry = entryDataSource;
    },
    deleteDataSourceEntry: function (rowIndex) {
        if (IVEditBase.DataSourceEntry.length <= 1) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "AtLeastOneLineItem", "You must have at least 1 line item."));
            return false;
        }
        var arr = new Array();
        var index = 0;
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            if (i != rowIndex) {
                var obj = IVEditBase.DataSourceEntry[i];
                obj.RowIndex = index;
                arr.push(obj);
                index += 1;
            }
        }
        //谭勇
        IVEditBase.DataSourceEntry = arr;
        return true;
    },
    insertDataSourceEntry: function (rowIndex) {
        var newRow = null;
        var arr = new Array();
        var index = 0;
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            if (IVEditBase.DataSourceEntry[i].RowIndex == rowIndex) {
                newRow = IVEditBase.getEmptyEntry();
                newRow.RowIndex = index;
                arr.push(newRow);
                index += 1;
            }
            var obj = IVEditBase.DataSourceEntry[i];
            obj.RowIndex = index;
            arr.push(obj);
            index += 1;
        }
        if (rowIndex == IVEditBase.DataSourceEntry.length) {
            var lastRow = IVEditBase.getEmptyEntry();
            lastRow.RowIndex = rowIndex;
            arr.push(lastRow);
            IVEditBase.DataSourceEntry = arr;
            return lastRow;
        }
        IVEditBase.DataSourceEntry = arr;
        return newRow;
    },
    appendDataSourceEntry: function () {
        var index = IVEditBase.DataSourceEntry.length;
        var newRow = IVEditBase.getEmptyEntry();
        newRow.RowIndex = index;
        IVEditBase.DataSourceEntry.push(newRow);
        return newRow;
    },
    bindGrid: function () {
        //各项列名的绑定
        var columns = IVEditBase.Columns.getArray();
        Megi.grid(IVEditBase.Selector, {
            columns: columns,
            resizable: true,
            auto: true,
            data: IVEditBase.DataSourceEntry,
            singleSelect: true,
            onClickCell: function (rowIndex, field, value) {
                //重复发票用到 begin
                $("#aInsertPlaceholder").hide();
                $("#divInsertPlaceholder").hide();
                $("#aInsertPlaceholder2").hide();
                var repeatInvoice = $("#hidRepeatInvoice").val();
                //重复发票用到 end
                if (IVEditBase.Disabled || field == "MEntryID" || field == "MAddID") {
                    return;
                }
                IVEditBase.endEditGrid();
                var currencyId = $('#selCurrency').combobox('getValue');
                if (currencyId == null || currencyId == "") {
                    return;
                }
                IVEditBase.RowIndex = rowIndex;
                Megi.grid(IVEditBase.Selector, "beginEdit", rowIndex);
                var editor = $(IVEditBase.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    if (field == "MItemID") {
                        $(editor.target).combobox("showPanel").combobox("textbox").focus();
                    } else if (field == "MDepartment" || field == "MTrackItem1" || field == "MTrackItem2" || field == "MTrackItem3" || field == "MTrackItem4" || field == "MTrackItem5") {
                        //$(editor.target).combobox("showPanel").combobox("textbox").focus();
                    } else if (field == "MTaxID") {
                        //$(editor.target).combobox("textbox").focus();
                    }
                    else {
                        //重复发票用到 begin
                        if (repeatInvoice == "true") {
                            if (field == "MDesc") {
                                $("#aInsertPlaceholder2").show();
                                //editor.target.focus();
                            } else {
                                //editor.target.select();
                            }
                        } else {
                            //editor.target.select();
                        }
                        //重复发票用到 end
                    }

                    //$(".datagrid-body").find(".numberbox,.datagrid-editable-input,.validatebox-text").mSelect();

                    $(".datagrid-body").find(".numberbox,.datagrid-editable-input,.validatebox-text").click(function () {
                        return;
                        //重复发票用到 begin
                        //if (repeatInvoice == "true") {
                        //    if (field == "MDesc") {
                        //        $(this).focus();
                        //    }
                        //} else {
                        //    $(this).select();
                        //}
                        //重复发票用到 end
                    });
                }
            },
            onClickRow: function (rowIndex, rowData) {
                if (IVEditBase.Disabled) {
                    return;
                }
                IVEditBase.bindGridEditorEvent(rowData, rowIndex);
                IVEditBase.bindGridEditorTabEvent(rowData, rowIndex);
                if (rowIndex == IVEditBase.DataSourceEntry.length - 1) {
                    IVEditBase.AddItemByIndex(IVEditBase.DataSourceEntry.length);
                }
            },
            onAfterEdit: function () {
                IVEditBase.updateDataSourceEntry();
            },
            onLoadSuccess: function () {
                var taxType = IVEditBase.getTaxType();
                if (taxType == 2) {
                    Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxID");
                    Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxAmtFor");
                }
                if (IVEditBase.isEmployees()) {
                    Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxID");
                }
            }
        });

        //easyui leftWidth 计算有问题，会导致 leftWidth 值逐渐变大，然后出现滚动条。
        Megi.grid(IVEditBase.Selector, "resize");

        Megi.grid(IVEditBase.Selector, "selectRow", 0);
    },
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
                        if (rowIndex == IVEditBase.DataSourceEntry.length - 1) {
                            IVEditBase.AddItemByIndex(IVEditBase.DataSourceEntry.length);
                        }
                        //则结束当前编辑行，开始进行下一行编辑
                        IVEditBase.beginNextRowEdit(rowIndex);
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
                        if (rowIndex == IVEditBase.DataSourceEntry.length - 1) {
                            IVEditBase.AddItemByIndex(IVEditBase.DataSourceEntry.length);
                        }
                    }
                    var field = $(editing_controls[editing_controls_index]).closest("td[field]").attr("field");
                    var editor = $(IVEditBase.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                    if (editor != null) {
                        if (field == "MItemID" || field == "MTaxID" || field == "MDepartment" || field == "MTrackItem1" || field == "MTrackItem2" || field == "MTrackItem3" || field == "MTrackItem4" || field == "MTrackItem5") {
                            $(editor.target).combobox("showPanel");
                        } else {
                            var repeatInvoice = $("#hidRepeatInvoice").val();
                            if (repeatInvoice == "true") {
                                if (field == "MDesc") {
                                    $("#aInsertPlaceholder2").show();
                                } else {
                                    //editor.target.select();
                                }
                            } else {
                                //editor.target.select();
                            }
                        }
                    }
                }
            }).keydown(function (event) {
                if (event.which == 9) {
                    if ($(this).closest(".datagrid-editable").closest("td").next().attr("field") == "MAmountFor") {
                        IVEditBase.beginNextRowEdit(rowIndex);
                    }
                }
            }).focus(function (event) {
                //重复发票用到 begin
                $("#aInsertPlaceholder").hide();
                $("#divInsertPlaceholder").hide();
                $("#aInsertPlaceholder2").hide();
                //重复发票用到 end
                var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                var editing_controls_index = editing_controls.index(this);
                var field = $(editing_controls[editing_controls_index]).closest("td[field]").attr("field");
                var editor = $(IVEditBase.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    if (field == "MItemID" || field == "MTaxID" || field == "MDepartment" || field == "MTrackItem1" || field == "MTrackItem2" || field == "MTrackItem3" || field == "MTrackItem4" || field == "MTrackItem5") {
                        $(editor.target).combobox("showPanel");
                    } else {
                        //重复发票用到 begin
                        var repeatInvoice = $("#hidRepeatInvoice").val();
                        if (repeatInvoice == "true") {
                            if (field == "MDesc") {
                                $("#aInsertPlaceholder2").show();
                            } else {
                                //editor.target.select();
                            }
                        } else {
                            //editor.target.select();
                        }
                        //重复发票用到 end
                    }
                }
            }).blur(function () {
                var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                var editing_controls_index = editing_controls.index(this);
                var field = $(editing_controls[editing_controls_index]).closest("td[field]").attr("field");
                var editor = $(IVEditBase.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    if (field == "MDesc") {
                        var desc = $(editor.target).val();
                        if ($.trim(desc).length > 0) {
                            Megi.grid(IVEditBase.Selector, "selectRow", IVEditBase.RowIndex);
                            var rowObj = Megi.grid(IVEditBase.Selector, "getSelected");
                            // IVEditBase.setContactDefaultTrack(editor.target, rowObj);
                        }
                    }
                }
            });
        });
    },
    //开始下一行编辑
    beginNextRowEdit: function (rowIndex) {
        IVEditBase.endEditGrid();
        var newRowIndex = rowIndex + 1;
        IVEditBase.RowIndex = newRowIndex;
        Megi.grid(IVEditBase.Selector, "selectRow", newRowIndex);
        Megi.grid(IVEditBase.Selector, "beginEdit", newRowIndex);
        var newRowData = Megi.grid(IVEditBase.Selector, "getSelected");

        var newEditor = $(IVEditBase.Selector).datagrid('getEditor', { index: newRowIndex, field: "MItemID" });
        if (newEditor != null) {
            $(newEditor.target).combobox("showPanel");
            //设置前面的+为tab的焦点，这样combox才能获取焦点
            $(newEditor.target).closest("td[field='MItemID']").prev().find('.m-icon-add-row').focus();
        }
        IVEditBase.bindGridEditorTabEvent(newRowData, newRowIndex);
        IVEditBase.bindGridEditorEvent(newRowData, newRowIndex);
    },
    //Qty事件计算
    calQty: function (rowData, rowIndex) {
        var value = IVEditBase.getGridEditorValue(rowIndex, "MQty");
        if (value < 0) {
            value = 0;
        }
        IVEditBase.updateDataSourceEntry("MQty='" + value + "'");
        IVEditBase.Columns.updateMTaxAmtForValue("MQty", rowIndex);
        rowData.MAmountFor = IVEditBase.DataSourceEntry[rowIndex].MAmountFor;
    },
    //Price事件计算
    calPrice: function (rowData, rowIndex) {
        var value = IVEditBase.getGridEditorValue(rowIndex, "MPrice");
        if (value < 0 || isNaN(parseFloat(value))) {
            value = 0;
        }
        IVEditBase.updateDataSourceEntry("MPrice='" + value + "'");
        IVEditBase.Columns.updateMTaxAmtForValue("MPrice", rowIndex);
        rowData.MAmountFor = IVEditBase.DataSourceEntry[rowIndex].MAmountFor;
    },
    //Discount事件计算
    calDiscount: function (rowData, rowIndex) {
        var value = IVEditBase.getGridEditorValue(rowIndex, "MDiscount");
        if (value < 0 || isNaN(parseFloat(value))) {
            value = 0;
        }
        else if (value > 100) {
            value = 100;
        }
        IVEditBase.updateDataSourceEntry("MDiscount='" + value + "'");
        IVEditBase.Columns.updateMTaxAmtForValue("MDiscount", rowIndex);
        rowData.MAmountFor = IVEditBase.DataSourceEntry[rowIndex].MAmountFor;
    },
    //TaxAmtFor事件
    calTaxAmtFor: function (rowData, rowIndex) {
        var value = IVEditBase.getGridEditorValue(rowIndex, "MTaxAmtFor");
        if (value < 0 || isNaN(parseFloat(value))) {
            value = 0;
        }
        IVEditBase.Columns.updateMTaxAmtForValue("MTaxAmtFor", rowIndex, value);
        rowData.MAmountFor = IVEditBase.DataSourceEntry[rowIndex].MAmountFor;
    },
    bindGridEditorEvent: function (rowData, rowIndex) {
        var taxType = IVEditBase.getTaxType();
        var ed = $(IVEditBase.Selector).datagrid('getEditors', rowIndex);
        for (var i = 0; i < ed.length; i++) {
            var e = ed[i];
            switch (e.field) {
                case "MQty":
                    $(e.target).bind("keyup", function (event) {
                        IVEditBase.calQty(rowData, IVEditBase.RowIndex);
                    });
                    break;
                case "MPrice":
                    $(e.target).bind("keyup", function (event) {
                        IVEditBase.calPrice(rowData, IVEditBase.RowIndex);
                    });
                    break;
                case "MDiscount":
                    $(e.target).bind("keyup", function (event) {
                        IVEditBase.calDiscount(rowData, IVEditBase.RowIndex);

                    }).bind("blur", function (event) {
                        var value = IVEditBase.getGridEditorValue(IVEditBase.RowIndex, "MDiscount");
                        IVEditBase.updateDataSourceEntry("MDiscount='" + value + "'");
                        rowData.MAmountFor = IVEditBase.DataSourceEntry[IVEditBase.RowIndex].MAmountFor;
                    });

                    break;
                case "MTaxID":
                    if (taxType == 2) {
                        $(e.target).combobox('disable');
                        IVEditBase.updateDataSourceEntry("MTaxID=''");
                        rowData.MTaxID = "";
                    }
                    break;
                case "MTaxAmtFor":
                    $(e.target).bind("keyup", function (event) {
                        IVEditBase.calTaxAmtFor(rowData, IVEditBase.RowIndex);
                        IVEditBase.updateSummaryInfo();
                    });
                    break;
            }
        }
    },
    getGridEditorValue: function (rowIndex, field) {
        var rowObj = $(IVEditBase.Selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]");
        var value = $(rowObj).find("td[field='" + field + "']").find("input").val();
        return value;
    },
    setGridEditorValue: function (rowIndex, field, setValue) {
        var rowObj = $(IVEditBase.Selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]");
        $(rowObj).find("td[field='" + field + "']").find("input").val(setValue);
    },
    getEmptyEntry: function () {
        var obj = {};
        obj.MEntryID = "";
        obj.InvoiceID = IVEditBase.InvoiceID;
        obj.MItemID = "";
        obj.MIsNew = true;
        obj.MDesc = "";
        obj.MQty = "";
        obj.MPrice = "";
        obj.MDiscount = "";
        obj.MAcctID = "";
        obj.MTaxID = "";
        obj.MAmountFor = "";
        obj.MAmount = "";
        obj.MTaxAmountFor = "";
        obj.MTaxAmount = "";
        obj.MTaxAmtFor = "";
        obj.MTaxAmt = "";
        obj.MDepartment = "";
        obj.MTrackItem1 = "";
        obj.MTrackItem2 = "";
        obj.MTrackItem3 = "";
        obj.MTrackItem4 = "";
        obj.MTrackItem5 = "";
        return obj;
    },
    Columns: {
        getArray: function () {
            var arr = new Array();

            if (!IVEditBase.Disabled) {
                arr.push({
                    title: '', field: 'MAddID', width: 30, height: "26px", align: 'center', formatter: function (value, rowData, rowIndex) {
                        return "<a href='javascript:void(0)' class='m-icon-add-row' onclick='IVEditBase.AddItem(this);event.stopPropagation();'>&nbsp;</a>";
                    }
                });
            }
            var itemColumn = IVEditBase.Columns.getItemColumn();
            arr.push(itemColumn);
            arr.push({ title: HtmlLang.Write(LangModule.IV, "Description", "Description"), field: 'MDesc', width: 120, align: 'left', sortable: false, editor: { type: 'text' } });
            arr.push({ title: HtmlLang.Write(LangModule.IV, "Qty", "Qty"), field: 'MQty', width: 80, align: 'right', sortable: false, formatter: function (value, rowData, rowIndex) { return Megi.Math.toMoneyFormat(value, 4, 0) }, editor: { type: 'numberbox', options: { precision: 4, minPrecision: 0, min: 0, max: 999999999999 } } });
            arr.push({ title: HtmlLang.Write(LangModule.IV, "UnitPrice", "Unit Price"), field: 'MPrice', align: 'right', width: 80, height: IVEditBase.EditorHeight, formatter: function (value, rowData, rowIndex) { return Megi.Math.toMoneyFormat(value, 8, 2) }, sortable: false, editor: { type: 'numberbox', options: { precision: 8, minPrecision: 2, min: 0, max: 999999999999 } } });
            arr.push({
                title: HtmlLang.Write(LangModule.IV, "Disc_percent", "Disc %"), field: 'MDiscount', width: 80, height: IVEditBase.EditorHeight, align: 'right', formatter: function (value, rowData, rowIndex) {
                    if (value > 0) {
                        return Megi.Math.toMoneyFormat(value, 6, 0) + "%";
                    }
                    return "";
                }, sortable: false, editor: { type: 'numberbox', options: { precision: 6, minPrecision: 0, min: 0, max: 99.99999 } }
            });
            var taxRateColumn = IVEditBase.Columns.getTaxRateColumn();
            arr.push(taxRateColumn);

            //税额 Add By Eddie 2016-09-18
            arr.push({
                title: HtmlLang.Write(LangModule.IV, "TaxAmountFor", "Tax Amount"), field: 'MTaxAmtFor', align: 'right', width: 80, height: IVEditBase.EditorHeight,
                formatter: function (value, rowData, rowIndex) {
                    if (value > 0) {
                        return Megi.Math.toMoneyFormat(value, 8, 2);
                    }
                    return "";
                },
                sortable: false,
                editor: {
                    type: 'numberbox',
                    options: { precision: 2, min: 0, max: 999999999999 }
                }
            }); 0

            //跟踪项
            if (IVEditBase.TrackDataSource != null) {
                for (var i = 0; i < IVEditBase.TrackDataSource.length; i++) {
                    //查跟踪项的方法
                    var obj = IVEditBase.getTrackColumn(i);
                    arr.push(obj);
                }
            }
            arr.push({
                title: HtmlLang.Write(LangModule.IV, "Amount", "Amount"), field: 'MAmountFor', width: 80, align: 'right', sortable: false, formatter: function (value, rowData, rowIndex) {
                    return (value == null || value == "") ? "" : Megi.Math.toMoneyFormat(value);
                }
            });
            if (!IVEditBase.Disabled) {
                arr.push({
                    title: '', field: 'MEntryID', width: 30, height: "26px", align: 'center', formatter: function (value, rowData, rowIndex) {
                        return "<div class='list-item-action'><a href='javascript:void(0)' class='m-icon-delete-row' onclick='IVEditBase.DeleteItem(this," + rowIndex + ")'>&nbsp;</a></div>";
                    }
                });
            }
            var columns = new Array();
            columns.push(arr);
            return columns;
        },
        //系统自动算的税额
        calTaxValue: function (rowIndex, taxType, vTaxRate, amtFor) {
            var taxValue = 0;
            if (taxType == 0) {
                //税额
                taxValue = Megi.Math.toDecimal(vTaxRate * amtFor, 2);
            } else if (taxType == 1) {
                taxValue = Megi.Math.toDecimal(amtFor - amtFor / (1 + vTaxRate), 2);
            }
            if (amtFor < 0) {
                taxValue = 0;
            }

            //给DataSorce赋值
            IVEditBase.DataSourceEntry[rowIndex].MTaxAmtFor = taxValue;
            //给页面赋值
            IVEditBase.setGridEditorValue(rowIndex, "MTaxAmtFor", taxValue == 0 ? "" : taxValue);

            return taxValue;
        },
        //更新税额和总计等（数量、单价、折扣率、税率更改后）value:税额。
        updateMTaxAmtForValue: function (field, rowIndex, value) {

            //每行的原币金额
            var amtFor = IVEditBase.getAmtFor(rowIndex);;
            var amt = 0;
            var taxAmt = 0;
            var taxAmtFor = 0;
            var tax = 0;
            var taxFor = 0;

            //税率
            var vTaxRate = IVEditBase.getTaxRate(IVEditBase.DataSourceEntry[rowIndex].MTaxID);
            //价税类型
            var taxType = IVEditBase.getTaxType();
            //税额不存在，自动计算
            if (!value) {
                value = IVEditBase.Columns.calTaxValue(rowIndex, taxType, vTaxRate, amtFor);
            }

            if (value > amtFor && amtFor > 0) {
                var message = HtmlLang.Write(LangModule.IV, "CannotGreaterAmount", "The tax amount cannot be greater than the total amount");
                $.mDialog.message(message);
                //给出系统算的值
                value = IVEditBase.Columns.calTaxValue(rowIndex, taxType, vTaxRate, amtFor);
            }
            //税额
            taxFor = Number(Megi.Math.toDecimal(Number(value), 2));

            amt = Number(Megi.Math.toDecimal(amtFor * IVEditBase.ExchangeRate, 2));
            tax = Number(Megi.Math.toDecimal(taxFor * IVEditBase.ExchangeRate, 2));

            //给DataSorce赋值
            IVEditBase.DataSourceEntry[rowIndex].MTaxAmtFor = value;
            if (taxType == 0) {
                taxAmtFor = amtFor + taxFor;
                taxAmt = amt + tax;
            }
            else if (taxType == 1) {
                taxAmtFor = amtFor;
                taxAmt = amt;
            }
            else if (taxType == 2) {
                taxAmtFor = amtFor;
                taxAmt = amt;
                taxFor = 0;
                tax = 0;
            }

            IVEditBase.DataSourceEntry[rowIndex].MAmountFor = Megi.Math.toDecimal(amtFor, 2);
            IVEditBase.DataSourceEntry[rowIndex].MAmount = Megi.Math.toDecimal(amt, 2);
            IVEditBase.DataSourceEntry[rowIndex].MTaxAmountFor = Megi.Math.toDecimal(taxAmtFor, 2);
            IVEditBase.DataSourceEntry[rowIndex].MTaxAmount = Megi.Math.toDecimal(taxAmt, 2);
            IVEditBase.DataSourceEntry[rowIndex].MTaxAmtFor = Megi.Math.toDecimal(taxFor, 2);
            IVEditBase.DataSourceEntry[rowIndex].MTaxAmt = Megi.Math.toDecimal(tax, 2);
            $(IVEditBase.Selector).parent().find("tr[datagrid-row-index=" + IVEditBase.RowIndex + "]>td[field='MAmountFor']").children("div").css({ 'padding-right': '4px' }).text((amtFor > 0 ? Megi.Math.toMoneyFormat(Megi.Math.toDecimal(amtFor, 2)) : ""));
            IVEditBase.updateSummaryInfo();
        },
        updateEditorValue: function (cbox, field, value) {
            var rowObj = $(cbox).closest("td[field='MItemID']").parent();
            $(rowObj).find('td[field="' + field + '"]').find("input").val(value);
        },
        updateComboEditorValue: function (cbox, field, text, value) {
            var rowObj = $(cbox).closest(".datagrid-row");
            $(rowObj).find("td[field='" + field + "']").find(".combo-text").val(text);
            $(rowObj).find("td[field='" + field + "']").find(".combo-value").val(value);
        },
        //获取条目的Grid列对象
        getItemColumn: function () {
            var editorType = "inventory";
            if (IVEditBase.isExpenseItem()) {
                editorType = "expense";
            }
            var obj = {};
            obj.title = IVEditBase.isExpenseItem() ? HtmlLang.Write(LangModule.IV, "ExpenseItem", "Expense Item") : HtmlLang.Write(LangModule.IV, "Item", "Item");
            obj.field = 'MItemID';
            obj.width = 120;
            obj.sortable = false;
            obj.editor = {
                type: 'addCombobox',
                options: {
                    type: editorType,
                    addOptions: {
                        //是否有联系人编辑权限
                        hasPermission: IVEditBase.IsCanSettingChangePermission,
                        //弹出框关闭后的回调函数
                        callback: IVEditBase.initItem
                    },
                    dataOptions: {
                        height: IVEditBase.EditorHeight,
                        //数据加载成功后更新数据源
                        onLoadSuccess: function (msg) {
                            IVEditBase.DataSourceItem = msg;
                        },
                        onSelect: function (rec) {

                            if (rec.MDesc != undefined && rec.MDesc != "") {
                                rec.MDesc = rec.MDesc.replace(/\n/g, "");
                            }
                            //是否是费用报销
                            if (!IVEditBase.isExpenseItem()) {
                                IVEditBase.Columns.updateEditorValue(this, "MDesc", rec.MDesc);
                                IVEditBase.Columns.updateEditorValue(this, "MQty", "1");
                                IVEditBase.updateDataSourceEntry('MItemID="' + rec.MItemID + '"');
                                IVEditBase.updateDataSourceEntry('MDesc="' + mText.encode(rec.MDesc) + '"');
                                IVEditBase.updateDataSourceEntry("MQty='1'");

                                //带出商品价格的时候需要不能进行币别换算，这个要注意
                                var price;
                                if (IVEditBase.IVCurrentType == IVEditBase.IVType.Purchase) {
                                    price = rec.MPurPrice;
                                    if (isNaN(price) || price == 0) {
                                        price = '';
                                    }
                                    IVEditBase.Columns.updateEditorValue(this, "MPrice", price);
                                    IVEditBase.updateDataSourceEntry("MPrice='" + price + "'");
                                } else {
                                    price = rec.MSalPrice;
                                    if (isNaN(price) || price == 0) {
                                        price = '';
                                    }
                                    IVEditBase.Columns.updateEditorValue(this, "MPrice", price);
                                    IVEditBase.updateDataSourceEntry("MPrice='" + price + "'");
                                }

                                Megi.grid(IVEditBase.Selector, "selectRow", IVEditBase.RowIndex);
                                var rowObj = Megi.grid(IVEditBase.Selector, "getSelected");
                                rowObj.MItemID = rec.MItemID;
                                rowObj.MAmountFor = IVEditBase.DataSourceEntry[IVEditBase.RowIndex].MAmountFor;
                                rowObj.MPurTaxTypeID = rec.MPurTaxTypeID;//商品采购税率
                                rowObj.MSalTaxTypeID = rec.MSalTaxTypeID;//商品销售税率

                                IVEditBase.setContactDefaultTrack(this, rowObj);

                            } else {

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

                                        IVEditBase.Columns.updateEditorValue(comboboxItem, "MDesc", rec.MDesc);
                                        IVEditBase.Columns.updateEditorValue(comboboxItem, "MQty", "1");
                                        IVEditBase.updateDataSourceEntry("MItemID='" + rec.MItemID + "'");
                                        IVEditBase.updateDataSourceEntry('MDesc="' + rec.MDesc + '"');
                                        IVEditBase.updateDataSourceEntry("MQty='1'");

                                        Megi.grid(IVEditBase.Selector, "selectRow", IVEditBase.RowIndex);
                                        var rowObj = Megi.grid(IVEditBase.Selector, "getSelected");
                                        rowObj.MItemID = rec.MItemID;
                                        rowObj.MAmountFor = IVEditBase.DataSourceEntry[IVEditBase.RowIndex].MAmountFor;

                                        IVEditBase.setContactDefaultTrack(comboboxItem, rowObj);
                                    }
                                });
                            }
                        }
                    }
                }
            };
            obj.formatter = function (value, rowData, rowIndex) {
                if (IVEditBase.isExpenseItem()) {
                    return IVEditBase.getExpenseName(value);
                } else {
                    return IVEditBase.getInventoryCode(value);
                }
            }
            return obj;
        },
        getTaxRateColumn: function () {
            var obj = {};
            obj.title = HtmlLang.Write(LangModule.IV, "TaxRate", "Tax Rate");
            obj.field = 'MTaxID';
            obj.width = 120;
            obj.sortable = false;
            obj.editor = {
                type: 'addCombobox',
                options: {
                    type: "taxrate",
                    addOptions: {
                        //是否有基础资料编辑权限
                        hasPermission: IVEditBase.IsCanSettingChangePermission,
                        //关闭后的回调函数
                        callback: IVEditBase.initTaxRate
                    },
                    dataOptions: {
                        height: "24px",
                        //数据加载成功后更新数据源
                        onLoadSuccess: function (msg) {
                            IVEditBase.DataSourceTaxRate = msg;
                        },
                        onChange: function (newValue, oldValue) {
                            if (newValue == undefined || newValue == null) {
                                newValue = "";
                            }
                            IVEditBase.updateDataSourceEntry("MTaxID='" + newValue + "'");
                            //税额
                            var taxAmtValue = +IVEditBase.DataSourceEntry[IVEditBase.RowIndex].MTaxAmtFor;
                            //税额为空去算税额
                            if (taxAmtValue == 0) {
                                IVEditBase.Columns.updateMTaxAmtForValue("MTaxID", IVEditBase.RowIndex);
                            } else if (oldValue != "") {
                                //不是combox加载的数据更新数据
                                IVEditBase.Columns.updateMTaxAmtForValue("MTaxID", IVEditBase.RowIndex);
                            }

                        }
                    }
                }
            };
            obj.formatter = function (value, rowData, rowIndex) {
                return IVEditBase.getTaxRateName(value);
            }
            return obj;
        }
    },
    getTrackColumn: function (index) {
        var obj = {};
        obj.title = IVEditBase.TrackDataSource[index].MName;
        obj.field = "MTrackItem" + (index + 1);
        obj.width = 80;
        obj.sortable = false;
        var trackId = IVEditBase.TrackDataSource[index].MValue;
        var trackName = IVEditBase.TrackDataSource[index].MName;
        obj.editor = {
            type: 'addCombobox',
            options: {
                type: "trackOption" + (index),
                addOptions: {
                    //是否有基础资料编辑权限
                    hasPermission: IVEditBase.IsCanSettingChangePermission,
                    //关闭后的回调函数
                    callback: function () {
                        IVEditBase.initTrackingOption(trackId, index);
                    },
                    width: 400,
                    height: 400,
                    url: "/BD/Tracking/CategoryOptionEdit?trackId=" + trackId,
                    itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                    dialogTitle: HtmlLang.Write(LangModule.Common, "NewTrackOption", "New Tracking Option")
                },
                dataOptions: {
                    url: "/BD/Tracking/GetTrackOptionsById?trackId=" + trackId,
                    height: "24px",
                    //数据加载成功后更新数据源
                    onLoadSuccess: function (msg) {
                        IVEditBase.TrackDataSource[index].MChildren = msg;
                    },
                    onChange: function (newValue, oldValue) {
                        if (newValue == undefined || newValue == null) {
                            newValue = "";
                        }
                        IVEditBase.updateDataSourceEntry("MTrackItem" + (index + 1) + "='" + newValue + "'");
                    },
                    valueField: 'MValue',
                    textField: 'MName',
                    hideItemKey: "MValue1",
                    hideItemValue: "0"
                }
            }
        };

        obj.formatter = function (value, rowData, rowIndex) {
            return IVEditBase.getTrackName(value);
        }
        return obj;
    },
    DeleteItem: function (btnObj) {
        var rowIndex = $(btnObj).closest(".datagrid-row").attr("datagrid-row-index");
        var result = IVEditBase.deleteDataSourceEntry(rowIndex);
        if (!result) {
            return;
        }
        Megi.grid(IVEditBase.Selector, "deleteRow", rowIndex);
        //先算每行的 index 再endEditGrid(),否则在endEditGrid 的时候 index是错的。
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
            var tr_id = $(this).attr("id");
            $(this).attr("id", tr_id.substr(0, tr_id.lastIndexOf('-') - 1) + i);
        });
        IVEditBase.endEditGrid();

        IVEditBase.updateDataSourceEntry();
        //明细列表自适应
        IVFW.setUI();
    },
    AddItem: function (btnObj) {
        var rowIndex = Number($(btnObj).closest(".datagrid-row").attr("datagrid-row-index"));
        IVEditBase.AddItemByIndex(rowIndex);

        //明细列表自适应
        IVFW.setUI();
    },
    AddItemByIndex: function (rowIndex) {
        var isLastLine = rowIndex == IVEditBase.DataSourceEntry.length;
        if (!isLastLine) {
            IVEditBase.endEditGrid();
        }
        var newRow = IVEditBase.insertDataSourceEntry(rowIndex);
        if (newRow != null) {
            Megi.grid(IVEditBase.Selector, "insertRow", { index: rowIndex, row: newRow });
            if (!isLastLine) {
                IVEditBase.endEditGrid();
            }
        }
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
        });
        if (!isLastLine) {
            Megi.grid(IVEditBase.Selector, "beginEdit", rowIndex);
            Megi.grid(IVEditBase.Selector, "selectRow", rowIndex);
            IVEditBase.RowIndex = rowIndex;

            var rowData = Megi.grid(IVEditBase.Selector, "getSelected");
            IVEditBase.bindGridEditorTabEvent(rowData, rowIndex);
            IVEditBase.bindGridEditorEvent(rowData, rowIndex);
        }
    },
    selectCurrency: function (currencyId) {
        $("#selCurrency").combobox("select", currencyId);
    },
    verifyExchangeRate: function (bizDate, isShowAlter) {
        if ($("#spExchangeInfo").hasClass("mg-prompt-error")) {
            if (isShowAlter) {
                var message = HtmlLang.Write(LangModule.IV, "CantFindExchangeRate", "该币别在（{0}）没有有效的汇率，请在基础资料或单据上汇率处进行设置！");
                $.mDialog.alert(message.replace("{0}", bizDate));
            }
            return false;
        }
        return true;
    },
    //切换币别
    changeCurrency: function (rec) {
        IVEditBase.endEditGrid();
        
        if (rec == undefined) {
            IVEditBase.IsFirstLoad = false;
            return;
        }

        //处理币种变更时变更前币种等于变更后币种时，不修改汇率
        if (!IVEditBase.IsFirstLoad && IVEditBase.IsCurrencyChange && rec.CurrencyID == IVEditBase.MCyID) {
            return;
        }

        var exchangeInfo = "";
        var editExchangeInfo = "";
        var amountTitle = HtmlLang.Write(LangModule.IV, "Amount", "Amount");

        //是否覆盖汇率
        if (IVEditBase.IsCoverExchangeRate) {
            IVEditBase.ExchangeRate = isNaN(parseFloat(rec.MOToLRate)) ? 0: parseFloat(rec.MOToLRate);
            IVEditBase.MOToLRate = IVEditBase.ExchangeRate;
            IVEditBase.MLToORate = isNaN(parseFloat(rec.MLToORate)) ? 0 : parseFloat(rec.MLToORate);
        }

        if (!rec.IsLocalCy) {
            //既然不是本位币，那么就肯定不可能汇率等于1 
            //已经保存的单据，取保存的汇率
            
            if (IVEditBase.SourceBillExchangeRate != undefined
                && IVEditBase.SourceBillExchangeRate != 0
                && IVEditBase.SourceBillExchangeRate != 1
                && !Megi.request('isCopyCredit') //推红字单据时不复制汇率
                && IVEditBase.IsFirstLoad == true ) {
                IVEditBase.ExchangeRate = parseFloat(Megi.Math.toDecimal(IVEditBase.SourceBillExchangeRate, 6));
                IVEditBase.MOToLRate = parseFloat(Megi.Math.toDecimal(IVEditBase.SourceBillOToLRate, 6));
                IVEditBase.MLToORate = parseFloat(Megi.Math.toDecimal(IVEditBase.SourceBillLToORate, 6));
            }
            else {
                IVEditBase.ExchangeRate = parseFloat(Megi.Math.toDecimal(IVEditBase.MOToLRate, 6));
                IVEditBase.MOToLRate = parseFloat(Megi.Math.toDecimal(IVEditBase.MOToLRate, 6));
                IVEditBase.MLToORate = parseFloat(Megi.Math.toDecimal(IVEditBase.MLToORate, 6));
            }
            amountTitle = HtmlLang.Write(LangModule.IV, "Amount", "Amount") + " " + rec.CurrencyID;
            exchangeInfo = "1 " + rec.CurrencyID + " = " + Megi.Math.toDecimal(IVEditBase.ExchangeRate, 6) + " " + rec.MLCurrencyID;
            editExchangeInfo = '<a href="javascript:void(0)" cyItemId="' + rec.MCyItemID + '" cyId="' + rec.CurrencyID + '" class="m-exchange-edit m-icon-edit" onclick="IVEditBase.editExchangeRate(this)">&nbsp;</a>'
        }

        IVEditBase.MCyID = rec.CurrencyID;
        if (IVEditBase.ExchangeRate == 0) {
            IVEditBase.ExchangeRate = isNaN(parseFloat(rec.MOToLRate)) ? 0 : parseFloat(rec.MOToLRate);
            IVEditBase.MOToLRate = IVEditBase.ExchangeRate;
            IVEditBase.MLToORate = isNaN(parseFloat(rec.MLToORate)) ? 0 : parseFloat(rec.MLToORate);
        }

        //验证 汇率。 只有切换币种 或者 切换时间时，才提示错误。否则只标红
        $("#spExchangeInfo").removeClass("mg-prompt-error");
        if (IVEditBase.ExchangeRate == undefined || IVEditBase.ExchangeRate == "" || IVEditBase.ExchangeRate == 0) {
            $("#spExchangeInfo").addClass("mg-prompt-error");
            IVEditBase.verifyExchangeRate(IVEditBase.BizDate, true);
        }

        //设置汇率
        $("#spExchangeInfo").html(exchangeInfo);
        if (IVEditBase.IsCanSettingChangePermission && !IVEditBase.Disabled) {
            $("#spEditExchangeRate").html(editExchangeInfo);
        }
        $(".datagrid-header-row").find("td[field='MAmountFor']").find("span").eq(0).html(amountTitle);
        $("#spTotalCurrency").attr("localCyID", rec.MLCurrencyID);
        if (!rec.IsLocalCy) {
            $("#spTotalCurrency").html(IVEditBase.MCyID).show();
        } else {
            $("#spTotalCurrency").hide();
        }

        //更新明细数据源
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            IVEditBase.RowIndex = i;
            var item = IVEditBase.DataSourceEntry[i];
            IVEditBase.updateDataSourceEntry("MPrice='" + item.MPrice + "'");
            if (item.MPrice && !IVEditBase.IsCurrencyDisabled) {
                //bug 25530,取消beginEdit,endEdit.不知道这个操作的原始目的，但是它会导致所有行的数据绑定重新后台获取
                //进入编辑，关键！ 会去刷税额
                //Megi.grid(IVEditBase.Selector, "beginEdit", i);
                IVEditBase.Columns.updateMTaxAmtForValue("MPrice", IVEditBase.RowIndex, item.MTaxAmtFor);
                //Megi.grid(IVEditBase.Selector, "endEdit", i);
            }
        }

        IVEditBase.bindGrid();

        //重置回默认值
        IVEditBase.IsCurrencyChange = true;
        IVEditBase.IsFirstLoad = false;
        IVEditBase.IsCoverExchangeRate = true;
    },
    editExchangeRate: function (obj) {
        var cyId = $(obj).attr("cyId");
        IVEditBase.openExchangeRatePage(cyId, IVEditBase.ExchangeRate == 0 ? "" : IVEditBase.ExchangeRate);
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
                    IVEditBase.IsCurrencyChange = false;
                    IVEditBase.changeCurrency(ops);
                }
            }]
        });
    },
    resetCurrency: function (dateTime) {
        if (IVEditBase.BizDate == dateTime) {
            return;
        }
        IVEditBase.BizDate = dateTime;
        var obj = eval("(" + "{" + $("#selCurrency").attr("data-options") + "}" + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return;
        }
        var currencyId = $("#selCurrency").combobox("getValue");

        //当前币种为空时，不处理
        if (!currencyId) {
            return;
        }
        mAjax.Post(
            "/BD/Currency/GetCurrencyDataOpstion", { endDate: dateTime }, function (msg) {
                var opts = eval("({" + msg + "})");
                opts.onSelect = IVEditBase.changeCurrency;
                $("#selCurrency").combobox(opts);

                if (!IVEditBase.isCurrencyExists(currencyId)) {
                    $("#spExchangeInfo").html("");
                    $("#spEditExchangeRate").html("");
                }

                if (IVEditBase.IsCurrencyDisabled) {
                    $('#selCurrency').combobox('disable');
                }

                //设置非币种切换
                IVEditBase.IsCurrencyChange = false;
                //查找取回来的 汇率是否和单据上面的汇率相等
                if (IVEditBase.isExchangeRateChange(currencyId, IVEditBase.MLToORate)) {
                    var tips = HtmlLang.Write(LangModule.IV, "ConfirmCoverExchangeRate", "重新选择日期会影响汇率，是否需要重新获取汇率？");
                    $.mDialog.confirm(tips, function () {
                        IVEditBase.IsCoverExchangeRate = true;
                        $("#selCurrency").combobox("select", currencyId);
                    }, function () {
                        IVEditBase.IsCoverExchangeRate = false;
                        $("#selCurrency").combobox("select", currencyId);
                    });
                }
                else {
                    $("#selCurrency").combobox("select", currencyId);
                }
            });
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
    //切换税率
    changeTaxType: function (record) {
        IVEditBase.endEditGrid();
        //IVEditBase.updateDataSourceEntry();
        //更新明细数据源
        for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
            IVEditBase.RowIndex = i;
            var item = IVEditBase.DataSourceEntry[i];
            IVEditBase.updateDataSourceEntry("MTaxID='" + item.MTaxID + "'");
            if (item.MItemID) {
                //如果selTaxType为disabled状态，不去刷税率
                var taxTypeDisable = $('#selTaxType')[0].disabled;
                if (!taxTypeDisable) {
                    //进入编辑，关键！ 会去刷税额
                    Megi.grid(IVEditBase.Selector, "beginEdit", i);
                    //更新税率等数据
                    IVEditBase.Columns.updateMTaxAmtForValue("MTaxID", i);
                    Megi.grid(IVEditBase.Selector, "endEdit", i);
                }
            }
        }
        //重置RowIndex
        IVEditBase.RowIndex = -1;
        IVEditBase.bindGrid();

    },
    getTaxType: function () {
        var value = $('#selTaxType').combobox('getValue');
        if (value == "Tax_Exclusive") {
            return 0;
        }
        else if (value == "Tax_Inclusive") {
            return 1;
        } else {
            return 2;
        }
    },
    //联系人
    changeContact: function (type, contactId, callback) {
        IVEditBase.GetContactInfo(type, contactId, function (msg) {
            IVEditBase.setDefaultCurrency(msg.MDefaultCurrencyID);
            IVEditBase.setDefaultDate(msg);
            if (callback != undefined) {
                callback(msg);
            }
        });
    },
    GetContactInfo: function (type, contactId, callback) {
        IVEditBase.ContactTrackType = type;
        mAjax.post(
            "/Contacts/GetContactBasicInfo/",
            { contactId: contactId },
            function (msg) {
                IVEditBase.ContactInfoDataSource = msg;
                if (callback != undefined) {
                    callback(msg);
                }
            });
    },
    setDefaultDate: function (data) {
        var date = new Date();
        var dateVal = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();//默认当前日期
        var isDate = mDate.isValidDate($('#txtDate').datebox('getValue'));//检查账单日期是否输入正确的日期
        var txtdate = null;
        if (isDate) {
            txtdate = new Date($('#txtDate').datebox('getValue'));//账单日期
        }
        //根据单据类型来取dueDate和dueCondition
        var dueDate = IVEditBase.IVType == "sle" ? data.MSalDueDate : data.MPurDueDate;
        var dueCondition = IVEditBase.IVType == "sle" ? data.MSalDueCondition : data.MPurDueCondition;
        if (dueDate && dueCondition) {
            switch (dueCondition) {
                case "item0"://下一个月
                    dateVal = date.getFullYear() + "-" + (date.getMonth() + 2) + "-" + dueDate;
                    break;
                case "item1"://账单日期后的天数
                    if (txtdate) {
                        var purDueDate = txtdate.addDays(dueDate);
                        dateVal = purDueDate.getFullYear() + "-" + (purDueDate.getMonth() + 1) + "-" + purDueDate.getDate();
                    }
                    break;
                case "item2"://账单月末后的天数
                    if (txtdate) {
                        dateVal = txtdate.getFullYear() + "-" + (txtdate.getMonth() + 2) + "-" + dueDate;
                    }
                    break;
                case "item3"://当前月份
                    if (date.getDate() > dueDate) {
                        dateVal = date.getFullYear() + "-" + (date.getMonth() + 2) + "-" + dueDate;
                    } else {
                        dateVal = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + dueDate;
                    }
                    break;
            }
        }
        //给到期日设置值
        $('#txtDueDate').datebox('setValue', dateVal);

    },
    setDefaultCurrency: function (defaultCyId) {
        if (IVEditBase.IsCurrencyDisabled) {
            return;
        }
        var obj = $("#selCurrency").attr("data-options");
        obj = "{" + obj + "}";
        obj = eval("(" + obj + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return;
        }
        var myCurrencyID = obj.data[0].CurrencyID;
        var currencyCurrecyID = $("#selCurrency").combobox("getValue");
        //if (myCurrencyID != currencyCurrecyID) {
        //    return;
        //}

        for (var i = 0; i < obj.data.length; i++) {
            if (obj.data[i].CurrencyID == defaultCyId) {
                $("#selCurrency").combobox("select", defaultCyId);
                return;
            }
        }
        $("#selCurrency").combobox("select", obj.data[0].CurrencyID);
    },
    getTrackIndex: function (contactTrackItemId) {
        if (IVEditBase.TrackDataSource == null) {
            return -1;
        }
        var index = -1;
        for (var i = 0; i < IVEditBase.TrackDataSource.length; i++) {
            if (IVEditBase.TrackDataSource[i].MChildren != null) {
                for (var k = 0; k < IVEditBase.TrackDataSource[i].MChildren.length; k++) {
                    if (contactTrackItemId == IVEditBase.TrackDataSource[i].MChildren[k].MValue) {
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
    setContactDefaultTrack: function (cbox, rowObj) {
        if (IVEditBase.ContactInfoDataSource == null) {
            return;
        }
        var taxId = "";
        var discount = IVEditBase.ContactInfoDataSource.MDefaultDiscount;
        //if (discount == 0) {
        //    discount = "";
        //}
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
        //含税类型
        var taxType = IVEditBase.getTaxType();
        if (IVEditBase.ContactTrackType == "") {
            if (IVEditBase.IVType == IVEditBase.IVType.Purchase) {
                IVEditBase.ContactTrackType = "Pur";
            } else {
                IVEditBase.ContactTrackType = "Sale";
            }
        }
        if (IVEditBase.ContactTrackType == "Sale") {
            index1 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MSaleTrackItem1);
            index2 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MSaleTrackItem2);
            index3 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MSaleTrackItem3);
            index4 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MSaleTrackItem4);
            index5 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MSaleTrackItem5);
            value1 = IVEditBase.ContactInfoDataSource.MSaleTrackItem1;
            value2 = IVEditBase.ContactInfoDataSource.MSaleTrackItem2;
            value3 = IVEditBase.ContactInfoDataSource.MSaleTrackItem3;
            value4 = IVEditBase.ContactInfoDataSource.MSaleTrackItem4;
            value5 = IVEditBase.ContactInfoDataSource.MSaleTrackItem5;
            if (taxType == 2) {
                //taxType 为 2 标示不包括税
                taxId = "";
            } else {
                if (rowObj.MSalTaxTypeID && rowObj.MSalTaxTypeID != "None") {
                    //如果商品存在销售税率，则取商品的销售税率
                    taxId = rowObj.MSalTaxTypeID;
                } else {
                    //否则，取联系人设置的销售税率
                    taxId = IVEditBase.ContactInfoDataSource.MDefaultSaleTaxID;

                }
            }
        }
        else if (IVEditBase.ContactTrackType == "Pur") {
            index1 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MPurchaseTrackItem1);
            index2 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MPurchaseTrackItem2);
            index3 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MPurchaseTrackItem3);
            index4 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MPurchaseTrackItem4);
            index5 = IVEditBase.getTrackIndex(IVEditBase.ContactInfoDataSource.MPurchaseTrackItem5);
            value1 = IVEditBase.ContactInfoDataSource.MPurchaseTrackItem1;
            value2 = IVEditBase.ContactInfoDataSource.MPurchaseTrackItem2;
            value3 = IVEditBase.ContactInfoDataSource.MPurchaseTrackItem3;
            value4 = IVEditBase.ContactInfoDataSource.MPurchaseTrackItem4;
            value5 = IVEditBase.ContactInfoDataSource.MPurchaseTrackItem5;
            if (taxType == 2) {
                taxId = "";
            } else {
                if (rowObj.MPurTaxTypeID && rowObj.MPurTaxTypeID != "None") {
                    //如果商品存在采购税率，则取商品的采购税率
                    taxId = rowObj.MPurTaxTypeID;
                } else {
                    //取联系人设置的采购税率
                    taxId = IVEditBase.ContactInfoDataSource.MDefaultPurchaseTaxID;
                }
            }
        }
        for (var i = 1; i < 6; i++) {
            var index = eval("index" + i);
            if (index > -1) {
                eval("item" + index + "=value" + i);
            }
        }

        IVEditBase.updateDataSourceEntry("MTrackItem1='" + item1 + "'");
        IVEditBase.updateDataSourceEntry("MTrackItem2='" + item2 + "'");
        IVEditBase.updateDataSourceEntry("MTrackItem3='" + item3 + "'");
        IVEditBase.updateDataSourceEntry("MTrackItem4='" + item4 + "'");
        IVEditBase.updateDataSourceEntry("MTrackItem5='" + item5 + "'");
        IVEditBase.updateDataSourceEntry("MTaxID='" + taxId + "'");
        IVEditBase.updateDataSourceEntry("MDiscount='" + discount + "'");
        //更新税额
        IVEditBase.Columns.updateMTaxAmtForValue("MTaxID", IVEditBase.RowIndex);

        IVEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem1", IVEditBase.getTrackName(item1), item1);
        IVEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem2", IVEditBase.getTrackName(item2), item2);
        IVEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem3", IVEditBase.getTrackName(item3), item3);
        IVEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem4", IVEditBase.getTrackName(item4), item4);
        IVEditBase.Columns.updateComboEditorValue(cbox, "MTrackItem5", IVEditBase.getTrackName(item5), item5);
        IVEditBase.Columns.updateComboEditorValue(cbox, "MTaxID", IVEditBase.getTaxRateName(taxId), taxId);

        IVEditBase.Columns.updateEditorValue(cbox, "MDiscount", discount == 0 ? "" : discount);
    },
    //结束编辑列表  
    endEditGrid: function () {
        var records = Megi.grid(IVEditBase.Selector, "getRows");
        var recordLength = records.length;
        var oldRowIndex = IVEditBase.RowIndex;
        for (var i = 0; i < recordLength; i++) {
            var itemValue = $("tr[datagrid-row-index='" + i + "']>td[field='MItemID']>.datagrid-cell").text();
            if (!itemValue) {
                IVEditBase.RowIndex = i;
                IVEditBase.updateDataSourceEntry("MItemID=''");
            }
            var isEdit = $("tr[datagrid-row-index='" + i + "']").hasClass("datagrid-row-editing");
            if (isEdit) {
                var ed = $(IVEditBase.Selector).datagrid('getEditors', i);
                for (var j = 0; j < ed.length; j++) {
                    //取税额列去刷
                    if (ed[j].field == "MTaxAmtFor") {
                        IVEditBase.Columns.updateMTaxAmtForValue("MTaxAmtFor", i, ed[j].target[0].value);
                    }
                }
                Megi.grid(IVEditBase.Selector, "endEdit", i);
            }
        }
        IVEditBase.RowIndex = oldRowIndex;
    },

    //获取默认过期日期
    getDefaultDueDate: function (billDate) {
        if (billDate == "") {
            return "";
        }
        billDate = mDate.parse(billDate);
        if (IVEditBase.ContactInfoDataSource == null) {
            return "";
        }
        var dueDate = 0;
        var dateCondition = "";

        if (IVEditBase.ContactTrackType == "Sale") {
            dueDate = IVEditBase.ContactInfoDataSource.MSaleDueDate;
            dateCondition = IVEditBase.ContactInfoDataSource.MSaleDueCondition;
        }
        else if (IVEditBase.ContactTrackType == "Pur") {
            dueDate = IVEditBase.ContactInfoDataSource.MPurchaseDueDate;
            dateCondition = IVEditBase.ContactInfoDataSource.MPurchaseDueCondition;
        }
        if (dateCondition == null || dateCondition == "null" || dateCondition == "") {
            return "";
        }
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
    //业务类型改变事件
    changeBizBillType: function (contactType) {
        if (IVEditBase.Disabled) {
            return;
        }
        IVEditBase.endEditGrid();
        IVEditBase.ContactType = contactType;
        IVEditBase.initItem();

        Megi.grid(IVEditBase.Selector, "showColumn", "MTaxID");
        Megi.grid(IVEditBase.Selector, "showColumn", "MTaxAmtFor");
        if (!IVEditBase.Disabled) {
            $('#selTaxType').combobox("enable");
        }
        if (IVEditBase.isExpenseItem()) {
            $('#selTaxType').combobox("disable");
            $('#selTaxType').combobox('select', "Tax_Inclusive");

            Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxID");
        }
        else if (IVEditBase.ContactType == "Other") {
            $('#selTaxType').combobox("enable");
            //$('#selTaxType').combobox('select', "No_Tax");
            Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxID");
            Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxAmtFor");
        }

        if (!IVEditBase.isEmployees()) {
            //清空明细数据源（部门字段）
            for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
                IVEditBase.DataSourceEntry[i].MDepartment = "";
            }
        }

        IVEditBase.bindGrid();

    },
    //联系人类型改变时间
    changeContactType: function () {
        var contactType = $('#selContactType').combobox('getValue');

        IVEditBase.endEditGrid();
        IVEditBase.ContactType = contactType;
        IVEditBase.initItem();
        if (contactType == "Employees") {
            //隐藏税率列
            IVEditBase.ContactInfoDataSource = null;
            IVEditBase.endEditGrid();
            $('#selTaxType').combobox('select', "No_Tax");
            $('#selTaxType').combobox("disable");
            IVEditBase.bindGrid();
            Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxID");
        } else if (contactType == "Other") {
            $('#selTaxType').combobox('select', "No_Tax");
            $('#selTaxType').combobox("enable");
            IVEditBase.bindGrid();
            Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxID");
            Megi.grid(IVEditBase.Selector, "hideColumn", "MTaxAmtFor");
            //清空明细数据源（部门字段）
            for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
                IVEditBase.DataSourceEntry[i].MDepartment = "";
            }
        } else {
            IVEditBase.endEditGrid();
            //显示税率列
            $('#selTaxType').combobox('select', "Tax_Exclusive");
            $('#selTaxType').combobox("enable");
            Megi.grid(IVEditBase.Selector, "showColumn", "MTaxID");
            Megi.grid(IVEditBase.Selector, "showColumn", "MTaxAmtFor");
            //清空明细数据源（部门字段）
            for (var i = 0; i < IVEditBase.DataSourceEntry.length; i++) {
                IVEditBase.DataSourceEntry[i].MDepartment = "";
            }
            IVEditBase.bindGrid();
        }
        if (IVEditBase.Disabled) {
            $('#selTaxType').combobox("disable");
        }
    },
    isExpenseItem: function () {
        if (IVEditBase.ContactType == "Employees") {
            return true;
        }
        return false;
    },
    isEmployees: function () {
        if (IVEditBase.ContactType == "Employees") {
            return true;
        }
        return false;
    },
    getPrintInvoiceType: function (billType) {
        var result = '';
        switch (billType) {
            case IVBase.InvoiceType.Sale:
            case IVBase.InvoiceType.InvoiceSaleRed:
                result = "InvoiceListPrint";
                break;
            case IVBase.InvoiceType.Purchase:
            case IVBase.InvoiceType.InvoicePurchaseRed:
                result = "BillListPrint";
                break;
        }
        return result;
    },
    //打开打印对话框
    OpenPrintDialog: function (invoiceId, billType) {
        var paramObj = {};
        paramObj.MStatus = IVEditBase.CurrentStatus;
        paramObj.MType = billType;
        paramObj.SelectedIds = invoiceId;
        var title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Print", "Print"), IVBase.getPrintTitle(billType)]);
        var param = $.toJSON({ reportType: IVEditBase.getPrintInvoiceType(billType), jsonParam: escape($.toJSON(paramObj)) });
        Print.previewPrint(title, param);
    },
    //打开日志对话框
    OpenHistoryDialog: function (invoiceId, reportType, billType) {
        var title = $.trim(reportType.replace(/([A-Z])/g, ' $1'));
        Megi.dialog({
            title: HtmlLang.Write(LangModule.Common, "History", "History"),
            top: window.pageYOffset || document.documentElement.scrollTop,
            width: 800,
            height: 455,
            href: "/Log/Log/BusLogList?invoiceId=" + invoiceId + "&billType=" + billType
        });
    },
    emailInvoice: function (invoiceId, invoiceType, currentStatus, invoiceNumber) {
        var reportType = '';
        switch (invoiceType) {
            case IVBase.InvoiceType.Sale:
                reportType = "SaleInvoice";
                break;
            case IVBase.InvoiceType.InvoiceSaleRed:
                reportType = "SaleRedInvoice";
                break;
            case IVBase.InvoiceType.Purchase:
                reportType = "PurchaseInvoice";
                break;
            case IVBase.InvoiceType.InvoicePurchaseRed:
                reportType = "PurchaseRedInvoice";
                break;
        }
        var jsonParam = {};
        jsonParam.ReportId = invoiceId;
        jsonParam.BillType = invoiceType;
        jsonParam.InvoiceNumber = invoiceNumber;

        var paramStr = 'selectIds=' + invoiceId + "&status=" + currentStatus + "&reportType=" + reportType + "&rptJsonParam=" + escape($.toJSON(jsonParam));
        var param = $.toJSON({ type: "Invoice", param: paramStr });
        Print.selectPrintSetting(param);
    },
    //绑定联系人（从ContactView跳转过来）
    bindContactByContactView: function () {
        var contactId = Megi.getUrlParam("contactId");
        if (contactId != null) {
            $('#selContact').combobox('select', contactId);
        }
    },
    reloadFapiaoInfo: function () {
        mAjax.post("/IV/Invoice/IssueFapiaoInfo/", { id: $("#hidInvoiceID").val() }, function (result) {
            $("#divFapiao").html(result);
        });
    }
}
