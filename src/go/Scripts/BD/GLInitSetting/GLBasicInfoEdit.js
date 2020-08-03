var GLBasicInfoEdit = {
    isSetup: location.href.indexOf('/Setup/') != -1,
    oriAcctStandardValue: '',
    oriAcctStandardText: '',
    init: function () {
        GLBasicInfoEdit.initAction();
        GLBasicInfoEdit.initForm();
    },
    initAction: function () {
        $("#aUpdate").click(function () {
            GLBasicInfoEdit.save();
            return false;
        });
        $("#selMonth,#selYear").combobox({
            onSelect: function () {
                GLBasicInfoEdit.showConversionDayLastDate();
            }
        });
        $("#selAcctStandard").combobox({
            onSelect: function (item) {
                if (GLBasicInfoEdit.oriAcctStandardValue == 1 && item.value == 2) {
                    $("#selAcctStandard").combobox('setValue', 1);
                    $.mDialog.alert(HtmlLang.Write(LangModule.Org, "TaxPayerUpdateToLowerMsg", "Accounting standard can not update from " + GLBasicInfoEdit.oriAcctStandardText + " to " + item.text + "."));
                }
            }
        });
    },
    showConversionDayLastDate: function () {
        var year = parseInt($("#selYear").combobox("getValue"));
        var month = parseInt($("#selMonth").combobox("getValue"));
        var conversionDate = new Date();
        conversionDate.setFullYear(year, month - 1, 1);//本月第一天
        var preDate = new Date(conversionDate.getTime() - 24 * 60 * 60 * 1000);//上月最后一天
        var preMonth = preDate.getMonth() + 1;//上个月
        var fullDate = preDate.getFullYear() + "-" + preMonth + "-" + preDate.getDate();
        $("#lblConversionDate").text(fullDate);
    },
    initDefaultValue: function () {
        $("#selAcctStandard").combobox('setValue', 1);
        var date = new Date();
        $('#selYear').combobox("setValue", date.getFullYear());
        $('#selMonth').combobox("setValue", date.getMonth() + 1);
    },
    initForm: function () {
        Megi.Form.get({
            url: "/BD/GLInitSetting/GetGLBasicInfo", fill: true, callback: function (data) {
                if (data == null || data.MItemID == null) {
                    GLBasicInfoEdit.initDefaultValue();
                }
                else {
                    GLBasicInfoEdit.setConversionDate(data.MConversionDate);
                }
                GLBasicInfoEdit.showConversionDayLastDate();

                if (!GLBasicInfoEdit.isSetup) {
                    $('#selYear').combobox("disable");
                    $('#selMonth').combobox("disable");
                    $('#selAcctStandard').combobox("disable");
                    GLBasicInfoEdit.oriAcctStandardValue = $("#selAcctStandard").combobox("getValue");
                    GLBasicInfoEdit.oriAcctStandardText = $("#selAcctStandard").combobox("getText");
                }
            }
        });
    },
    validateForm: function(){
        var selTaxPlayer = $("#selAcctStandard");
        if (selTaxPlayer.combobox("getValue") == '') {
            selTaxPlayer.addClass('validatebox-invalid').focus();
            return false;
        }
        return true;
    },
    save: function (toUrl, isSaveAndQuit) {
        if (!GLBasicInfoEdit.validateForm()) {
            SetupBase.hideMask();
            return false;
        }

        var obj = Megi.Form.getForm();
        obj.MConversionDate = GLBasicInfoEdit.getConversionDate();

        Megi.Form.submit({
            url: "/BD/GLInitSetting/UpdateGLBasicInfo", param: obj, callback: function (msg) {
                var isSetup = toUrl != undefined;
                if (msg.Success) {
                    if (!isSetup) {
                        $.mMsg(HtmlLang.Write(LangModule.Org, "UpdateSuccessfully", "Update successfully."));
                        location.reload();
                    } else {
                        if (isSaveAndQuit) {
                            top.location = toUrl;
                        }
                        else {
                            location = toUrl;
                        }
                    }
                } else {
                    $.mDialog.alert(msg.Message);
                    if (isSetup) {
                        SetupBase.hideMask();
                    }
                }
            }
        });
    },
    getConversionDate: function () {
        var year = $('#selYear').combobox("getValue");
        var month = parseInt($('#selMonth').combobox("getValue"));
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
    GLBasicInfoEdit.init();
});