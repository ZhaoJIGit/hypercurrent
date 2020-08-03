var PSEdit = {
    itemID: $("#hidItemID").val(),
    init: function () {
        PSEdit.initUI();
        PSEdit.initAction();
        PSEdit.initSetting(); 
    },
    initUI: function () {
        $("#MTopMargin,#MBottomMargin").css({ "width": 136 });
    },
    initAction: function () {
        $("#aSave").click(function () {
            PSEdit.save();
        });
        $("#MShowEmployeeName").change(function () {
            PSEdit.switchCtrlStatus(this.checked, $("#MShowCnEnName"));
        });
        $("#MShowAdditionalInfo").change(function () {
            PSEdit.switchCtrlStatus(this.checked, $("#divAdditionalInfo input:checkbox"));
        });
        $("#MShowLogo").change(function () {
            PSEdit.switchCtrlStatus(this.checked, $("#divLogoAlign input:radio"));
        });
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
            url: "/PA/PayrollBasic/GetPrintSetting/" + PSEdit.itemID, fill: true, callback: function (data) {
                if (!PSEdit.itemID) {
                    $("#MName").focus();
                }
                PSEdit.loadGroupItemName(data.MPayItemFieldMapping, data.MPayItemGroupList)
                var chkObjs = $("input:checkbox");
                $.each(chkObjs, function (i, chk) {
                    $(chk).attr("checked", data[chk.id]);
                });
                PSEdit.setRdoVal("MMeasureIn", data.MMeasureIn);
                PSEdit.setRdoVal("MLogoAlignment", data.MLogoAlignment);
                $("#MShowLogo, #MShowAdditionalInfo, #MShowEmployeeName").trigger("change");
            }
        });
    },
    loadGroupItemName: function (mapping, groupList) {
        var arrHtml = [];
        $.each(mapping, function (field, itemType) {
            var payItem = groupList.filter(function (item) {
                return item.MItemType == itemType;
            });
            arrHtml.push("<div class='field checkbox'>\
                <input type='checkbox' class='radio' id='" + field + "' name='" + field + "' />\
                <label for='" + field + "'>" + mText.encode(payItem[0].MName) + "</label></div>");
        });
        $("#divGroupList").html(arrHtml.join(''));
    },
    setRdoVal: function (name, val) {
        $('input[name="' + name + '"]').each(function (i) {
            if ($(this).val() == val) {
                $(this).attr("checked", true);
            }
        });
    },
    getRdoVal: function(name){
        return $("input:radio:checked[name='" + name + "']").val();
    },
    save: function () {
        if (!$(".required-field").mFormValidate()) {
            return;
        }
        var obj = {};
        var chkObjs = $("input:checkbox");
        $.each(chkObjs, function (i, chk) {
            obj[chk.id] = $(chk).attr("checked") ? 1 : 0;
        });
        obj.MMeasureIn = PSEdit.getRdoVal("MMeasureIn"); 
        obj.MLogoAlignment = PSEdit.getRdoVal("MLogoAlignment");

        $("body").mFormSubmit({
            url: "/PA/PayrollBasic/UpdatePrintSetting", param: obj, callback: function (msg) {
                if (msg.Success) {
                    var successMsg = PSEdit.itemID ? HtmlLang.Write(LangModule.Common, "PrintSettingUpdated", "Your changes to <strong>{0}</strong> were saved.").replace("{0}", msg.Tag) : HtmlLang.Write(LangModule.Common, "PrintSettingAdded", "The new print setting <strong>{0}</strong> has been saved.").replace("{0}", msg.Tag);
                    $.mMsg(successMsg, undefined, true);
                    parent.PSList.reload();
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    PSEdit.init();
});