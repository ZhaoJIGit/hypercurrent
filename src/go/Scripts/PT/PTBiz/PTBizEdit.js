var PTBizEdit = {
    itemID: $("#hidItemID").val(),
    isSmartVersion: $("#hidIsSmartVersion").val() === "True",
    init: function () {
        PTBizEdit.initUI();
        PTBizEdit.initSetting();
        PTBizEdit.initAction();
    },
    initAction: function () {
        $("#aSave").click(function () {
            PTBizEdit.save();
        });
    },
    initUI: function () {
        var curLang = $("#hidCurrentLangID", top.document).val();
        var isChrome = /chrome/i.test(navigator.userAgent);
        var isSafari = /^((?!chrome).)*safari/i.test(navigator.userAgent);
        if (isChrome) {
            $("#divMeasureIn").css("margin-left", (curLang != '0x0009' ? "20px" : "16px"));
        }
        else if (isSafari) {
            $("#divMeasureIn").css("margin-left", "0px");
        }

        if (curLang != '0x0009') {
            $(".contact-detail").width(245);
            $("#MContactDetails").height(142).width(243);
        }

        if (PTBizEdit.isSmartVersion) {
            var logoAlign = $("#logoAlignment").clone();
            $("#logoAlignment").remove();
            logoAlign.show().appendTo("#divLogoAlign");
            $("<br/>").appendTo("#divLogoAlign");
            $("#logoAlignment").css({ "padding-left": "17px" });
            $("#MShowLogo").change(function () {
                PTBizEdit.switchCtrlStatus(this.checked, $("#divLogoAlign input:radio"));
            });

            $(".branding-options").width(320);
            $(".print-tmpl-edit .right").width(300).height(128);
            $("#logoAlignment .field").css({ "float": "left", "margin-left": "10px" });
            if (curLang != '0x0009') {
                $(".print-tmpl-edit .right").css({ "padding-left": "20px" });
            }
            $(".print-tmpl-edit").height(220);
        }
    },
    switchCtrlStatus: function (chkStatus, chkObj) {
        if (chkStatus) {
            chkObj.removeAttr("disabled");
        }
        else {
            chkObj.attr("disabled", true);
        }
    },
    initSetting: function () {
        $("body").mFormGet({
            url: "/PT/PTBiz/GetPT/" + PTBizEdit.itemID, fill: true, callback: function (data) {
                if (!PTBizEdit.itemID) {
                    $("#MName").focus();
                }
                $("#MContactDetails").html(data.MContactDetails);
                $("#MTermsAndPayAdvice").html(data.MTermsAndPayAdvice);
                $("#MShowTaxNumber").attr("checked", data.MShowTaxNumber);
                $("#MShowHeading").attr("checked", data.MShowHeading);
                $("#MShowUnitPriceAndQuantity").attr("checked", data.MShowUnitPriceAndQuantity);
                $("#MShowTaxColumn").attr("checked", data.MShowTaxColumn);
                $("#MShowHeading").attr("checked", data.MShowHeading);
                $("#MShowRegAddress").attr("checked", data.MShowRegAddress);
                $("#MShowLogo").attr("checked", data.MShowLogo);
                $("#MShowLogo").trigger("change");
                $("#MShowTracking").attr("checked", data.MShowTracking);
                $("#MHideDiscount").attr("checked", data.MHideDiscount);
                PTBizEdit.setRdoVal("MMeasureIn", data.MMeasureIn);
                PTBizEdit.setRdoVal("MLogoAlignment", data.MLogoAlignment);
                if (data.MShowTaxType == null) {
                    data.MShowTaxType = "";
                }
                PTBizEdit.setRdoVal("MShowTaxType", data.MShowTaxType);
            }
        });
    },
    setRdoVal: function (name, val) {
        $('input[name="' + name + '"]').each(function (i) {
            if ($(this).val() == val) {
                $(this).attr("checked", true);
            }
        });
    },
    getRdoVal: function (name) {
        return $("input:radio:checked[name='" + name + "']").val();
    },
    save: function () {
        if (!$(".required-field").mFormValidate()) {
            return;
        }
        var obj = {};
        obj.MMeasureIn = PTBizEdit.getRdoVal("MMeasureIn");
        obj.MLogoAlignment = PTBizEdit.getRdoVal("MLogoAlignment");
        obj.MShowTaxType = PTBizEdit.getRdoVal("MShowTaxType");
        obj.MShowTaxNumber = $("#MShowTaxNumber").attr("checked") ? 1 : 0;
        obj.MShowUnitPriceAndQuantity = $("#MShowUnitPriceAndQuantity").attr("checked") ? 1 : 0;
        obj.MShowTaxColumn = $("#MShowTaxColumn").attr("checked") ? 1 : 0;
        obj.MShowRegAddress = $("#MShowRegAddress").attr("checked") ? 1 : 0;
        obj.MShowLogo = $("#MShowLogo").attr("checked") ? 1 : 0;
        obj.MShowTracking = $("#MShowTracking").attr("checked") ? 1 : 0;
        obj.MHideDiscount = $("#MHideDiscount").attr("checked") ? 1 : 0;
        obj.MContactDetails = $("#MContactDetails").val();
        obj.MTermsAndPayAdvice = $("#MTermsAndPayAdvice").val();

        if (PTBizEdit.isSmartVersion) {
            obj.MShowHeading = 1;
        }
        else {
            obj.MShowHeading = $("#MShowHeading").attr("checked") ? 1 : 0;
        }

        $("body").mFormSubmit({
            url: "/PT/PTBiz/UpdatePT", param: { model: obj }, callback: function (msg) {
                if (msg.Success) {
                    var successMsg = PTBizEdit.itemID ? HtmlLang.Write(LangModule.Common, "PrintTemplateUpdated", "更新成功！") : HtmlLang.Write(LangModule.Common, "PrintTemplateAdded", "新增成功！");
                    $.mMsg(successMsg, undefined, true);
                    parent.PTBase.reload("Biz");
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    PTBizEdit.init();
});