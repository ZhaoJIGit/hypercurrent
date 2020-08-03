var FinancialEdit = {
    isSetup: location.href.indexOf('/Setup/') != -1,
    oriTaxPlayerValue: '',
    oriTaxPlayerText: '',
    initYear: '',
    initMonth: '',
    isSmartVersion: $("#hidVersion").val() === "1",
    oriAcctStandardValue: '',
    isChangeTaxType: false,
    tabswitch: new BrowserTabSwitch(),
    //原始的财务设置对象，用于判断是否需要重置科目
    oriEditModel: null,
    //是否重置科目
    isResetAccount: false,
    init: function () {
        FinancialEdit.tabswitch.initSessionStorage();
        FinancialEdit.initAction();
        FinancialEdit.initForm();
    },
    initAction: function () {
        $("#aUpdate").click(function () {
            FinancialEdit.saveFinancial();
            return false;
        });
        $("#fileTaxRegCertCopy, #fileLocalTaxRegCertCopy").change(function () {
            var lbl = $(this).next();
            ImportBase.onFileChanged(this);
            var validateResult = FileBase.validateFile(this.value, 0, FileBase.imgRegex);
            if (validateResult) {
                ImportBase.clearFile(this);
                $.mDialog.alert(validateResult);
            }
        });
        $("#selMonth,#selYear").combobox({
            onSelect: function () {
                FinancialEdit.showConversionDayLastDate();
            }
        });
        $("#selMTaxPayer").combobox({
            //放开 一般纳税人到小规模纳税人的控制
            onSelect: function (item) {
                if (FinancialEdit.oriTaxPlayerValue && FinancialEdit.oriTaxPlayerValue != item.value) {
                    FinancialEdit.isChangeTaxType = true;
                } else {
                    FinancialEdit.isChangeTaxType = false;
                }
            }
        });
        $("#selCurrencyCode").combobox({
            url: "/BD/Currency/GetSystemCurrencyList",
            textField: "MLocalName",
            valueField: "MCurrencyID",
            filter: function (q, row) {
                var opts = $(this).combobox('options');
                var text = row[opts.textField] ? row[opts.textField].toLowerCase() : "";
                var value = row[opts.valueField] ? row[opts.valueField].toLowerCase() : "";
                q = q.toLowerCase();

                if (text.indexOf(q) >= 0 || value.indexOf(q) >= 0) {
                    return true;
                } else {
                    return false;
                }
            },
            onLoadSuccess: function () {
                var isEnable = $("#hidIsCurrencyEnabled").val() === "True";
                if (!isEnable) {
                    $(this).combobox("disable");
                }
            }
        });

        $("#selAcctStandard").combobox({
            onSelect: function (item) {
                if (FinancialEdit.oriAcctStandardValue == 1 && item.value == 2) {
                    $("#selAcctStandard").combobox('setValue', 1);
                    $.mDialog.alert(HtmlLang.Write(LangModule.Org, "TaxPayerUpdateToLowerMsg", "Accounting standard can not update from " + FinancialEdit.oriAcctStandardText + " to " + item.text + "."));
                }
            }
        });
    },
    showConversionDayLastDate: function () {
        var year = parseInt($("#selYear").combobox("getValue"));
        if (!year) {
            year = FinancialEdit.initYear;
        }
        var month = parseInt($("#selMonth").combobox("getValue"));
        if (!month) {
            month = FinancialEdit.initMonth;
        }

        var conversionDate = new Date();
        conversionDate.setFullYear(year, month - 1, 1);//本月第一天
        var preDate = new Date(conversionDate.getTime() - 24 * 60 * 60 * 1000);//上月最后一天
        var preMonth = preDate.getMonth() + 1;//上个月
        var fullDate = preDate.getFullYear() + "-" + preMonth + "-" + preDate.getDate();
        $("#lblConversionDate").text(fullDate);
        FinancialEdit.initYear = year;
        FinancialEdit.initMonth = month;
    },
    initDefaultValue: function () {
        $("#selCurrencyCode").combobox('setValue', 'CNY');
        $("#selMTaxPayer").combobox('setValue', 1);
        //$("#selMYearEndDay").combobox('setValue', 31);
        //$("#selMYearEndMonth").combobox('setValue', 12);

        var date = new Date();
        $('#selYear').combobox("setValue", date.getFullYear());
        $('#selMonth').combobox("setValue", date.getMonth() + 1);
    },
    initForm: function () {
        $("body").mFormGet({
            url: "/BD/Financial/GetFinancial", fill: true, callback: function (data) {
                if (data.MItemID == null) {
                    FinancialEdit.initDefaultValue();
                    $("#selAcctStandard").combobox('setValue', 1);
                }
                else {
                    FinancialEdit.oriEditModel = data;
                    $("#selAcctStandard").combobox('setValue', data.MAccountingStandard);
                    if (!FinancialEdit.isSetup) {
                        $('#selAcctStandard').combobox("disable");
                    }
                }
                $("#hidTaxRegCertAttachId").val(data.MTaxRegCertCopyAttachId);
                $("#hidLocalTaxRegCertAttachId").val(data.MLocalTaxRegCertCopyAttachId);
                var fileIds = '';//data.MTaxRegCertCopyAttachId + ',' + data.MLocalTaxRegCertCopyAttachId;
                if (data.MTaxRegCertCopyAttachModel != null) {
                    $("#aTaxRegCert").text(data.MTaxRegCertCopyAttachModel.MName).click(function () {
                        FinancialEdit.viewFileInfo(data.MTaxRegCertCopyAttachId, fileIds, data.MTaxRegCertCopyAttachModel.MName);
                    }).closest(".content").height(53);
                }
                if (data.MLocalTaxRegCertCopyAttachModel != null) {
                    $("#aLocalTaxRegCert").text(data.MLocalTaxRegCertCopyAttachModel.MName).click(function () {
                        FinancialEdit.viewFileInfo(data.MLocalTaxRegCertCopyAttachId, fileIds, data.MLocalTaxRegCertCopyAttachModel.MName);
                    }).closest(".content").height(53);
                }
                FinancialEdit.setConversionDate(data.MConversionDate);
                FinancialEdit.showConversionDayLastDate();

                if (!FinancialEdit.isSetup) {
                    $('#selYear').combobox("disable");
                    $('#selMonth').combobox("disable");
                    FinancialEdit.oriTaxPlayerValue = $("#selMTaxPayer").combobox("getValue");
                    FinancialEdit.oriTaxPlayerText = $("#selMTaxPayer").combobox("getText");
                }
                else {
                    FinancialEdit.initYear = $('#selYear').combobox("getValue");
                    FinancialEdit.initMonth = $('#selMonth').combobox("getValue");
                    $(".year-month").find("input").attr("class", "combo-text").attr("readonly", "readonly");//给设置年月的combox中的input设成只读。
                }
            }
        });
    },
    viewFileInfo: function (fileId, fileIds, fileName) {
        var param = 'curFileId=' + fileId + '&fileIds=' + fileIds;
        if ($("#hidIsSetup").val() === 'true') {
            param += '&isSetup=true';
        }
        Megi.openDialog('/BD/Docs/FileView', fileName, param, 560, 460);
    },
    validateForm: function () {
        var arrSelId = ["#selCurrencyCode", "#selMTaxPayer", "#selYear", "#selMonth"];
        arrSelId.splice(1, 0, "#selAcctStandard");
        if (!FinancialEdit.validateCombo(arrSelId.toString())) {
            return false;
        }
        if (!$("#IdMTaxNo").validatebox("isValid")) {
            return false;
        }
        return true;
    },
    validateCombo: function (ids) {
        var isValid = true;
        var selCtrls = $(ids);
        $.each(selCtrls, function (i, sel) {
            if (!$(sel).combobox("isValid")) {
                $(sel).addClass('validatebox-invalid').focus();
                isValid = false;
                return false;
            }
        });
        return isValid;
    },
    //判断是否重置科目
    isResetAccount: function () {
        var model = FinancialEdit.oriEditModel;
        var selAcctStandard = $("#selAcctStandard").combobox("getValue");
        var selCurrency = $("#selCurrencyCode").combobox("getValue");
        var conversionDate = FinancialEdit.getConversionDate();

        //如果是新增，则不重置
        if (!model) {
            return false;
        }

        //如果会计准则、启用日期或币别字段中，有修改任何一个，则需要重置科目
        if (model.MAccountingStandard != selAcctStandard
            || $.mDate.format(model.MConversionDate) != $.mDate.format(conversionDate)
            || model.MCurrencyID != selCurrency) {
            return true;
        }

        return false;
    },
    saveFinancial: function (toUrl, isSaveAndQuit) {
        //校验字段
        if (!FinancialEdit.validateForm()) {
            SetupBase.hideMask();
            return false;
        }

        //判断是否重置科目
        if (FinancialEdit.isResetAccount()) {
            $.mDialog.confirm(HtmlLang.Write(LangModule.Org, "ResetAccountConfirm", "修改启用月份/会计准则/币别会重置会计科目，期初余额的数据将会被清除，请确认是否继续？"),
            {
                callback: function () {
                    FinancialEdit.isResetAccount = true;
                    FinancialEdit.submitData(toUrl, isSaveAndQuit);
                }
            });
        }
        else {
            FinancialEdit.isResetAccount = false;
            FinancialEdit.submitData(toUrl, isSaveAndQuit);
        }        
    },
    //提交数据
    submitData: function (toUrl, isSaveAndQuit) {
        var isTaxRegCertSelectFile = ImportBase.getFileName("fileTaxRegCertCopy").length > 0;
        var isLocalTaxRegCertSelectFile = ImportBase.getFileName("fileLocalTaxRegCertCopy").length > 0;

        var obj = $("body").mFormGetForm();
        obj.MTaxRegCertCopyAttachId = $("#hidTaxRegCertAttachId").val();
        obj.MLocalTaxRegCertCopyAttachId = $("#hidLocalTaxRegCertAttachId").val();
        $("#hidLocalTaxRegCertAttachId").val();
        if (isTaxRegCertSelectFile && !isLocalTaxRegCertSelectFile) {
            obj.IsUpdateTaxRegCert = true;
        }
        else if (!isTaxRegCertSelectFile && isLocalTaxRegCertSelectFile) {
            obj.IsUpdateLocalTaxRegCert = true;
        }
        obj.MTaxNo = mText.encode(obj.MTaxNo);
        obj.MConversionDate = FinancialEdit.getConversionDate();
        obj.MAccountingStandard = $("#selAcctStandard").combobox("getValue");
        FinancialEdit.bindSubmitAction(obj, toUrl, isSaveAndQuit);
        $("#fileFinancialCert").submit();
    },
    getUpdateTaxInfo:function(callback){
        //需要提示用户新增的税率信息
        var url = "/BD/TaxRate/GetUpdateTaxInfo";

        var params = {
            changeTaxType : $("#selMTaxPayer").combobox("getValue")
        };

        $.mAjax.post(url, params, function (data) {
            //如果成功，则表明需要提示用户升级后的一些税率信息
            if (data.Success) {
                var taxRateList = data.VerificationInfor;

                if (taxRateList && taxRateList.length) {
                    var needAddTrList = "";
                    var existTrList = "";
                    for (var i = 0 ; i < taxRateList.length; i++) {
                        var taxRate = taxRateList[i];
                        //需要新增
                        if (taxRate.Id == "1") {
                            needAddTrList += "<tr><td width='70%'>" + taxRate.Message + "</td><td width='30%' align='right'>" + taxRate.ExtendField + "</td></tr>";
                        } else {
                            existTrList += "<tr><td width='70%'>" + taxRate.Message + "</td><td width='30%' align='right'>" + taxRate.ExtendField + "</td></tr>";
                        }
                    }

                    $("#tbExistRate").empty().append(existTrList);
                    $("#tbAddRate").empty().append(needAddTrList);
                }
                $.mDialog.show({
                    mTitle: "",
                    mWidth: 650,
                    mHeight: 450,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mShowTitle: false,
                    mContent: "id:panel-tax-update",
                });

                //绑定确定升级事件
                $("#btnUpdateTax").off("click").on("click", function () {
                    if (callback && $.isFunction(callback)) {
                        callback();
                    }

                    $.mDialog.hide();
                });


            } else {
                if (callback && $.isFunction(callback)) {
                    callback();
                }
            }
        }, true, true);

    },
    bindSubmitAction: function (obj, toUrl, isSaveAndQuit) {
        var isSetup = toUrl != undefined;
        $("body").mask("");
        $("form").ajaxForm({
            url: "/BD/Financial/UploadFinancialCert",
            data: obj,
            dataType: ImportBase.isIE9Previous ? null : "json",
            type: "POST",
            success: function (response) {
                $("body").unmask();

                //判断是否有错误信息
                if (response && response.Data && response.Data.Message) {
                    $.mDialog.alert(response.Data.Message);
                    return;
                }

                //判断是否被逼下线 
                if (response && response.accessDenied === 1) {
                    $("body").unmask();

                    top.showLoginDialog(response.type, function () { });

                    return;
                }

                //保存失败
                if (response && !ImportBase.isIE9Previous && !response.Success) {
                    var msg = typeof (response.VerificationInfor) != 'undefined' ? response.VerificationInfor[0].Message : response.Message;
                    $.mDialog.alert(msg);
                }
                //保存成功
                else {
                    //在Go站点保存
                    if (!isSetup) {
                        var tips = HtmlLang.Write(LangModule.Org, "UpdateSuccessful", "Update successfully.");

                        if (FinancialEdit.isChangeTaxType) {
                            tips += "<br>" + HtmlLang.Write(LangModule.Org, "UpdateSuccessTips", "纳税人类型的修改，系统不会进行税率升级，请手动至税率界面进行新增");
                            $.mDialog.alert("<div>" + tips + "</div>", function () {
                                location.reload();
                            });
                        } else {
                            $.mDialog.message(tips);
                            location.reload();
                        }
                        
                    }
                    //在向导中保存
                    else {
                        //重置科目后关闭科目初始余额录入页面和科目单据初始化页面
                        if (FinancialEdit.isResetAccount) {
                            $.mTab.remove(HtmlLang.Write(LangModule.Acct, "AccountInitBalanceInput", "科目初始余额录入"));
                            $.mTab.remove(HtmlLang.Write(LangModule.BD, "DocInitBills", "科目单据初始化"));
                        }

                        mAjax.submit(
                            "/BD/InitSetting/GLSetupSuccess",
                            null,
                            function (msg) {
                                if (msg.Success) {
                                    SetupBase.showParentMask();
                                    if (isSaveAndQuit) {
                                        top.mWindow.reload(toUrl);
                                    }
                                    else {
                                        mWindow.reload(toUrl);
                                    }
                                } else {
                                    $.mDialog.alert(msg.Message);
                                    SetupBase.hideMask();
                                }
                            });
                    }
                    return;
                }
            },
            fail: function (event, data) {
                if (!isSetup) {
                    $("body").unmask();
                }
                $.mDialog.alert(data);
            }
        });
    },
    getConversionDate: function () {
        var year = FinancialEdit.initYear;
        var month = FinancialEdit.initMonth;
        return year + "-" + month + "-1";
    },
    setConversionDate: function (date) {
        if (date != undefined && date != null) {
            var cDate = $.mDate.format(date);
            if (cDate != "") {
                var year = cDate.substring(0, 4);
                var month = cDate.substring(5, 7);
                if (month.length == 2 && month.substring(0, 1) == "0") {
                    month = month.substring(1, 2);
                }
                $('#selYear').combobox("setValue", year);
                $('#selMonth').combobox("setValue", month);
            }
        }
    }
}
$(document).ready(function () {
    FinancialEdit.init();
});