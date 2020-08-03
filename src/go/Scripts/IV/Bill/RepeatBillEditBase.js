﻿/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var RepeatBillEditBase = {
    InvoiceID: $("#hidInvoiceID").val(),
    //发票模型
    InvoiceModel: null,
    ListUrl: "/IV/Bill/BillList",
    EditUrl: "/IV/Bill/RepeatBillEdit",
    ViewUrl: "/IV/Bill/RepeatBillView",
    SaveUrl: "/IV/Bill/UpdateRepeatBill",
    IsEdit: true,
    PreviewPlaceholderDatas: [],
    IsFirstLoad: false,
    //初始化
    init: function () {
        RepeatBillEditBase.IsFirstLoad = true;
        RepeatBillEditBase.getModel();
        RepeatBillEditBase.initCombobox();
        RepeatBillEditBase.initOperation();

        $("#txtBizDate").datebox({
            onChange: function (newValue, oldValue) {
                if ($("#txtBizDate").datebox("panel").is(":visible")) {
                    return;
                }
                if (mDate.isDateString(newValue)) {
                    RepeatBillEditBase.changeDate();
                }
            },
            onSelect: function (date) {
                RepeatBillEditBase.changeDate();
            }
        });

        RepeatBillEditBase.IsFirstLoad = false;
    },
    //初始化下拉框
    initCombobox: function () {
        //重复类型下拉框
        $("#selRepeatType").combobox({
            valueField: 'value', textField: 'text',
            data: [
                { value: "Week", text: HtmlLang.Write(LangModule.IV, "Week", "Week(s)") },
                { value: "Month", text: HtmlLang.Write(LangModule.IV, "Month", "Month(s)") }
            ],
            onSelect: function (record) {
                $("#selDueDateType").combobox({
                    valueField: 'value', textField: 'text',
                    data: RepeatBillEditBase.getDueDateTypeDataSource(),
                    onLoadSuccess: function () {
                        RepeatBillEditBase.changeDueDateType();
                    }
                });
                RepeatBillEditBase.astrictDueDateNumber();
            },
            onLoadSuccess: function () {
                //提示文字
                $("#selRepeatType").initHint();
                //选中
                if (RepeatBillEditBase.InvoiceModel && RepeatBillEditBase.InvoiceModel.MRepeatType) {
                    $('#selRepeatType').combobox('setValue', RepeatBillEditBase.InvoiceModel.MRepeatType);
                } else {
                    $('#selRepeatType').combobox('setValue', "Month");
                }
            }
        });
        //到期日期类型下拉框
        $("#selDueDateType").combobox({
            valueField: 'value', textField: 'text',
            data: RepeatBillEditBase.getDueDateTypeDataSource(),
            onSelect: function (record) {
                RepeatBillEditBase.astrictDueDateNumber();
            },
            onLoadSuccess: function () {
                //提示文字
                $("#selDueDateType").initHint();
                //选中
                if (RepeatBillEditBase.InvoiceModel && RepeatBillEditBase.InvoiceModel.MDueDateType) {
                    $('#selDueDateType').combobox('setValue', RepeatBillEditBase.InvoiceModel.MDueDateType);
                } else {
                    RepeatBillEditBase.changeDueDateType();
                }
                //加载到期日期数字
                if (RepeatBillEditBase.InvoiceModel) {
                    $("#txtDueDateNumber").numberbox('setValue', RepeatBillEditBase.InvoiceModel.MDueDateNumber);
                }
                RepeatBillEditBase.astrictDueDateNumber();
            }
        });

        var statusData = [];
        if ($("#hidHasApprovePermission").val() == "True") {
            statusData = [{ value: "1", text: HtmlLang.Write(LangModule.IV, "SaveAsDraft", "Save as Draft") },
            { value: "2", text: HtmlLang.Write(LangModule.IV, "Approve", "Approve") }];
        } else {
            statusData = [{ value: "1", text: HtmlLang.Write(LangModule.IV, "SaveAsDraft", "Save as Draft") }]
        }
        //状态下拉框
        $("#selStatus").combobox({
            valueField: 'value', textField: 'text',
            data: statusData,
            onLoadSuccess: function () {
                //提示文字
                $("#selStatus").initHint();
                //选中
                if (RepeatBillEditBase.InvoiceModel && RepeatBillEditBase.InvoiceModel.MStatus) {
                    $('#selStatus').combobox('setValue', RepeatBillEditBase.InvoiceModel.MStatus);
                }
            }
        });
    },
    //获取到期日期类型数据源
    getDueDateTypeDataSource: function () {
        var dueDateType_Data = [];
        //到期日期类型
        var repeatType = $('#selRepeatType').combobox('getValue');
        if (repeatType == "Week") {
            dueDateType_Data = [
                { value: "item0", text: HtmlLang.Write(LangModule.IV, "OfTheFollowingMonth", "of the following month") },
                { value: "item1", text: HtmlLang.Write(LangModule.IV, "DayAfterTheInvoiceDate", "day(s) after the invoice date") },
            ];
        } else if (repeatType == "Month") {
            dueDateType_Data = [
                { value: "item0", text: HtmlLang.Write(LangModule.IV, "OfTheFollowingMonth", "of the following month") },
                { value: "item1", text: HtmlLang.Write(LangModule.IV, "DayAfterTheInvoiceDate", "day(s) after the invoice date") },
                //{ value: "item2", text: HtmlLang.Write(LangModule.IV, "DayAfterTheEndOfTheInvoiceMonth", "day(s) after the end of the invoice month") },//此选项暂时隐藏掉，因为和其他项所表示的意思类似
                { value: "item3", text: HtmlLang.Write(LangModule.IV, "OfTheCurrentMonth", "of the current month") }
            ];
        }
        return dueDateType_Data;
    },
    //初始化操作
    initOperation: function () {
        //备注插入
        $("#aInsertPlaceholder").click(function () {
            var left = $(this).offset().left;
            var top = $(this).offset().top;
            var divInsertPlaceholder = $("#divInsertPlaceholder");
            if (divInsertPlaceholder.is(":visible")) {
                divInsertPlaceholder.hide();
            } else {
                divInsertPlaceholder.css({ left: left + "px", top: (top + 20) + "px", "z-index": 99999 }).show();
            }
        });
        //Megi.popup("#aInsertPlaceholder", { selector: "#divInsertPlaceholder" });
        $("#txtRef").focus(function () {
            $("#aInsertPlaceholder").show();
            //隐藏（明细）插入弹出层
            $("#aInsertPlaceholder2").hide();
            $("#divInsertPlaceholder2").hide();
        });
        $("#ul-insert-placeholder a").click(function () {
            var insert_ref = "[" + $(this).text() + "]";
            //在光标处插入内容
            $("#txtRef").insertAtCaret(insert_ref);
            $("#txtRef").blur();
            //隐藏插入弹出层
            $("#aInsertPlaceholder").hide();
            $("#divInsertPlaceholder").hide();
        });

        //明细备注插入
        $("#aInsertPlaceholder2").click(function () {
            var left = $(this).offset().left;
            var top = $(this).offset().top;
            var divInsertPlaceholder = $("#divInsertPlaceholder2");
            if (divInsertPlaceholder.is(":visible")) {
                divInsertPlaceholder.hide();
            } else {
                divInsertPlaceholder.css({ left: left + "px", top: (top + 20) + "px", "z-index": 99999 }).show();
            }
        });
        $("#ul-insert-placeholder2 a").click(function () {
            var insert_ref = "[" + $(this).text() + "]";
            var editor = $(IVEditBase.Selector).datagrid('getEditor', { index: IVEditBase.RowIndex, field: "MDesc" });
            if (editor != null) {
                editor.target.insertAtCaret(insert_ref);
                editor.target.blur();
            }
            $("#aInsertPlaceholder2").hide();
            $("#divInsertPlaceholder2").hide();
        });

        //预览占位符
        $("#aPreviewPlaceholders").click(function () {
            var ppdArray = new Array();
            var obj = {};
            obj.Title = HtmlLang.Write(LangModule.IV, "Description", "Description");
            obj.Content = $.trim($("#txtRef").val());
            if (obj.Content == "") {
                obj.Content = "(" + HtmlLang.Write(LangModule.IV, "empty", "empty") + ")";
            }
            ppdArray.push(obj);

            var descList = IVEditBase.getDescList();
            for (var i = 0; i < descList.length; i++) {
                var obj2 = {};
                obj2.Title = HtmlLang.Write(LangModule.IV, "Line", "Line") + " " + (i + 1) + " " + HtmlLang.Write(LangModule.IV, "Description", "description");
                obj2.Content = descList[i].MDesc;
                ppdArray.push(obj2);
            }

            RepeatBillEditBase.PreviewPlaceholderDatas = ppdArray;

            Megi.dialog({
                title: HtmlLang.Write(LangModule.IV, "PreviewPlaceholders", "Preview Placeholders"),
                top: window.pageYOffset || document.documentElement.scrollTop,
                width: 550,
                height: 320,
                href: "/IV/UC/PreviewPlaceholders?billType=Bill"
            });
        });
    },
    //限制到期日期数字
    astrictDueDateNumber: function () {
        var dueDateType = $('#selDueDateType').combobox('getValue');
        var txtDueDateNumber = $("#txtDueDateNumber");
        if (dueDateType == "item1") {
            txtDueDateNumber.attr("maxlength", "3");
        } else {
            txtDueDateNumber.attr("maxlength", "2");
            var number = $('#txtDueDateNumber').numberbox('getValue');
            if (dueDateType == "item0") {
                if (parseInt(number) > 31) {
                    $('#txtDueDateNumber').numberbox('setValue', 31);
                } else if (parseInt(number) < 1) {
                    $('#txtDueDateNumber').numberbox('setValue', 1);
                }
            } else if (dueDateType == "item2") {
                if (number.length > 2) {
                    $('#txtDueDateNumber').numberbox('setValue', number.substr(0, 2));
                }
            } else if (dueDateType == "item3") {
                var bizDate = $('#txtBizDate').datebox('getValue');
                if (bizDate) {
                    var bizDate_D = new Date(bizDate);
                    var monthLastDay = RepeatBillEditBase.getLastDay(bizDate_D.getFullYear(), bizDate_D.getMonth() + 1);
                    //如果输入的数字大于某月的最大天数，则默认为某月的最大天数
                    if (parseInt(number) > monthLastDay) {
                        $("#txtDueDateNumber").numberbox('setValue', monthLastDay);
                    } else {
                        var date_num = parseInt(bizDate_D.getDate());
                        if (parseInt(number) < date_num) {
                            var dueDateType = $('#selDueDateType').combobox('getValue');
                            if (dueDateType == "item3") {
                                $("#txtDueDateNumber").numberbox('setValue', date_num);
                            }
                        }
                    }
                }
            }
        }
    },
    //获得某月的最后一天 
    getLastDay: function (year, month) {
        var new_year = year;//取当前的年份
        var new_month = month++;//取下一个月的第一天，方便计算（最后一天不固定）
        if (month > 12) {
            new_month -= 12;//月份减         
            new_year++;//年份增         
        }
        var new_date = new Date(new_year, new_month, 1);//取当年当月中的第一天         
        return (new Date(new_date.getTime() - 1000 * 60 * 60 * 24)).getDate();//获取当月最后一天日期         
    },
    //过期时间类型改变事件
    changeDueDateType: function () {
        var repeatType = $('#selRepeatType').combobox('getValue');
        if (repeatType == "Week") {
            $("#selDueDateType").combobox("setValue", "item1");
        } else if (repeatType == "Month") {
            $("#selDueDateType").combobox("setValue", "item0");
        }
    },
    //保存发票
    saveInvoice: function (callback) {
        RepeatBillEditBase.saveModel(function (msg) {
            if (msg.Success == false) {
                $.mDialog.alert(msg.Message);
                return;
            }
            callback(msg);
        });
    },
    //查看操作日志
    viewHistory: function () {
        IVEditBase.OpenHistoryDialog(RepeatBillEditBase.InvoiceID, "History", "Invoice_Purchase");
    },
    //联系人改变事件
    changeContact: function (rec) {
        var value = rec.MItemID;
        IVEditBase.changeContact("Pur", value, function (msg) {

        });
    },
    afterAddContact: function (param) {
        if (param) {
            $('#selContact').combobox('setValue', param);
        }
    },
    //业务日期改变事件
    changeDate: function () {
        var bizDate = $('#txtBizDate').datebox('getValue');
        if (bizDate) {
            var dueDateType = $('#selDueDateType').combobox('getValue');
            if (dueDateType == "item3") {
                $("#txtDueDateNumber").numberbox('setValue', new Date(bizDate).getDate());
            }
        }
        if (!RepeatBillEditBase.IsFirstLoad) {
            IVEditBase.resetCurrency(bizDate);
        }
        ////当前系统日期
        //var dateNow = $.mDate.format($('#hidDateNow').val());
        ////业务日期必须大于当前系统日期
        //if (bizDate <= dateNow) {
        //    $.mDialog.alert(HtmlLang.Write(LangModule.IV, "RepeatBillBizDateMsg", "The Next Bill Date you entered has already occurred. \
        //        Bills prior to and including today will be generated when you Save."));
        //    return;
        //}
    },
    //验证结束日期
    validateEndDate: function () {
        //业务日期
        var bizDate = $('#txtBizDate').datebox('getValue');
        //结束日期
        var endDate = $('#txtEndDate').datebox('getValue');
        //结束日期不为空的情况下
        if (endDate != "") {
            if (bizDate > endDate) {
                $.mDialog.alert(HtmlLang.Write(LangModule.IV, "EndDateMBOOATNextBillDate", "End Date must be on or after the Next Bill Date."));
                return false;
            }
        }
        return true;
    },
    //初始化发票信息
    getModel: function () {
        //发票信息模型
        var msg = mText.getObject("#hidInvoiceModel");
        RepeatBillEditBase.InvoiceModel = msg;
        //业务日期
        if (RepeatBillEditBase.InvoiceID != "") {
            //如果 InvoiceID 不为空，则说明有录入日期
            $('#txtBizDate').datebox('setText', $.mDate.format(msg.MBizDate));
        }
        $("#txtRepeatNumber").numberbox('setValue', parseInt(msg.MRepeatNumber) <= 0 ? 1 : msg.MRepeatNumber);
        $('#txtEndDate').datebox('setValue', $.mDate.format(msg.MEndDate));
        $('#txtExpectedDate').datebox('setValue', $.mDate.format(msg.MExpectedDate));
        $("#txtRef").val(msg.MReference);

        var isDisabled = $("#hidIsEdit").val() == "True" ? false : true;

        IVEditBase.init({
            IVType: IVEditBase.IVType.Purchase, //类型，主要是判断是销售还是采购
            id: RepeatBillEditBase.InvoiceID, //单据ID
            oToLRate: msg.MOToLRate,  //原币到本位币的汇率
            lToORate: msg.MLToORate, //本位币到原币的汇率
            verificationList: msg.Verification, //核销列表
            entryList: msg.RepeatInvoiceEntry, //分录列表
            cyId: msg.MCyID, //币别
            taxId: msg.MTaxID, //税率类型
            disabled: isDisabled, //是否禁止编辑
            currencyDisabled: false, //币别是否允许编辑
            type: msg.MType, //单据类型
            status: msg.MStatus, //单据状态
            bizDate: $('#txtBizDate').datebox('getValue')  //单据日期
        });

        if (msg.MContactID != null && msg.MContactID != "") {
            IVEditBase.GetContactInfo("Pur", msg.MContactID, function (msg) {
                //这里需要重新获取一下，否则拿不到数据
                msg = mText.getObject("#hidInvoiceModel");
                //联系人(等联系人下拉框数据加载完成后再设置选中的值，否则会出现内码ID)
                $('#selContact').combobox('setValue', msg.MContactID);
                //将当前页面设置为稳定状态
                $.mTab.setStable();
            });
        }
        else {
            IVEditBase.bindContactByContactView();
        }
        //将当前页面设置为稳定状态
        $.mTab.setStable();
    },
    saveModel: function (callback) {
        //业务日期
        var bizDate = $('#txtBizDate').datebox('getValue');
        if (!IVEditBase.verifyExchangeRate(bizDate)) {
            return;
        }
        var entryInfo = IVEditBase.getInfo();
        entryResult = IVEditBase.valideInfo(IVEditBase.IVType.Purchase);
        var result = $(".m-form-icon").mFormValidate();
        if (!result || !entryResult) {
            return;
        }
        //结束日期
        var endDate = $('#txtEndDate').datebox('getValue');
        //发票日期不能大于到期日期
        var dueDateNumber = parseInt($("#txtDueDateNumber").numberbox('getValue'));
        var dueDateType = $('#selDueDateType').combobox('getValue');
        if (dueDateType == "item3") {
            var date = new Date(bizDate);
            if (date.getDate() > dueDateNumber) {
                $.mDialog.alert(HtmlLang.Write(LangModule.IV, "TheDueDateCannotPrecedeTheBillDate", "The Due Date cannot precede the Bill Date."));
                return;
            }
        }
        //验证结束日期
        var isEndDateOk = RepeatBillEditBase.validateEndDate();
        if (!isEndDateOk) {
            return;
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

        var contactId = $("#selContact").combobox("getValue");
        var contactName = $("#selContact").combobox("getText")
        if (!contactId || contactId == contactName) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "ContatNotExist", "联系人不存在，请重新选择联系人"));
            return;
        }

        var obj = {};
        obj.MID = $("#hidInvoiceID").val();
        obj.MType = IVBase.InvoiceType.Purchase;
        obj.MRepeatNumber = $("#txtRepeatNumber").numberbox('getValue');
        obj.MRepeatType = $('#selRepeatType').combobox('getValue');
        obj.MContactID = $('#selContact').combobox('getValue');
        obj.MBizDate = bizDate;
        obj.MDueDateNumber = dueDateNumber;
        obj.MDueDateType = dueDateType;
        obj.MEndDate = endDate;
        obj.MExpectedDate = "";
        obj.MBranding = $("#selBranding").combobox('getValue');
        obj.MStatus = $("#selStatus").combobox('getValue');
        obj.MReference = $("#txtRef").val();

        obj.MTotalAmtFor = entryInfo.MTotalAmtFor;
        obj.MTotalAmt = entryInfo.MTotalAmt;
        obj.MTaxTotalAmtFor = entryInfo.MTaxTotalAmtFor;
        obj.MTaxTotalAmt = entryInfo.MTaxTotalAmt;
        obj.MTaxID = entryInfo.MTaxID;
        obj.MCyID = entryInfo.MCyID;
        obj.MExchangeRate = entryInfo.MExchangeRate;
        obj.MOToLRate = entryInfo.MOToLRate;
        obj.MLToORate = entryInfo.MLToORate;
        obj.RepeatInvoiceEntry = entryInfo.InvoiceEntry;

        $("body").mFormSubmit({
            url: RepeatBillEditBase.SaveUrl, param: { model: obj }, validate: true, callback: function (msg) {
                if (!RepeatBillEditBase.InvoiceID && typeof (AssociateFiles) != undefined && typeof (AssociateFiles) != 'undefined') {
                    AssociateFiles.associateFilesTo($("#hidBizObject").val(), msg.ObjectID, undefined, function () {
                        if (callback != undefined) {
                            callback(msg);
                        }
                    });
                }
                else if (callback != undefined) {
                    callback(msg);
                }
            }
        });
    }
}
//页面加载事件
$(document).ready(function () {
    RepeatBillEditBase.init();
});