var PTSalaryEdit = {
    itemID: $("#hidItemID").val(),
    init: function () {
        PTSalaryEdit.initUI();
        PTSalaryEdit.initAction();
        PTSalaryEdit.initSetting();
    },
    initUI: function () {
        $("#MTopMargin,#MBottomMargin").css({ "width": 136 });
    },
    initAction: function () {
        $("#aSave").click(function () {
            PTSalaryEdit.save();
        });
        $("#MShowEmployeeName").change(function () {
            PTSalaryEdit.switchCtrlStatus(this.checked, $("#MShowCnEnName"));
        });
        $("#MShowAdditionalInfo").change(function () {
            PTSalaryEdit.switchCtrlStatus(this.checked, $("#divAdditionalInfo input:checkbox"));
        });
        $("#MShowLogo").change(function () {
            PTSalaryEdit.switchCtrlStatus(this.checked, $("#divLogoAlign input:radio"));
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
            url: "/PT/PTSalaryList/GetPT/" + PTSalaryEdit.itemID, fill: true, callback: function (data) {
                if (!PTSalaryEdit.itemID) {
                    $("#MName").focus();
                }
                PTSalaryEdit.loadGroupItemName(data.MEntryList)
                var chkObjs = $("input:checkbox");
                //给复选框赋值
                $.each(chkObjs, function (i, chk) {
                    //分录项
                    if ($(chk).attr("payItemId")) {
                        $(chk).attr("checked", $(chk).attr("value") === "true");
                    }
                    //非分录项
                    else {
                        $(chk).attr("checked", data[chk.id]);
                    }
                });
                PTSalaryEdit.setRdoVal("MMeasureIn", data.MMeasureIn);
                PTSalaryEdit.setRdoVal("MLogoAlignment", data.MLogoAlignment);
                $("#MShowLogo, #MShowAdditionalInfo, #MShowEmployeeName").trigger("change");
            }
        });
    },
    //加载一级工资项目
    loadGroupItemName: function (entryList) {
        var arrHtml = [];
        $.each(entryList, function (i, entry) {
            var entryId = !entry.MEntryID ? "" : entry.MEntryID;
            arrHtml.push("<div class='field checkbox'>\
                <input type='checkbox' class='radio' entryId='" + entryId + "' payItemId='" + entry.MPayItemID + "' value='" + entry.MIsShow + "' id='MShow" + entry.MPayItemID + "' name='MShow" + entryId + "' />\
                <label for='MShow" + entry.MPayItemID + "'>" + mText.encode(entry.MPayItemName) + "</label></div>");
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
    getRdoVal: function (name) {
        return $("input:radio:checked[name='" + name + "']").val();
    },
    save: function () {
        if (!$(".required-field").mFormValidate()) {
            return;
        }
        var obj = {};
        var entryList = [];
        var chkObjs = $("input:checkbox");
        $.each(chkObjs, function (i, chk) {
            var entryId = $(chk).attr("entryId");
            var isShow = $(chk).attr("checked") ? 1 : 0;
            var payItemId = $(chk).attr("payItemId");

            //如果payItemId属性有值，为分录项
            if (payItemId) {
                entryList.push({ MEntryID: entryId, MIsShow: isShow, MPayItemID: payItemId });
            }
            //非分录项
            else {
                obj[chk.id] = isShow;
            }
        });
        obj.MEntryList = entryList;
        obj.MMeasureIn = PTSalaryEdit.getRdoVal("MMeasureIn");
        obj.MLogoAlignment = PTSalaryEdit.getRdoVal("MLogoAlignment");

        $("body").mFormSubmit({
            url: "/PT/PTSalaryList/UpdatePT", param: { model: obj }, callback: function (msg) {
                if (msg.Success) {
                    var successMsg = PTSalaryEdit.itemID ? HtmlLang.Write(LangModule.Common, "PrintTemplateUpdated", "更新成功！") : HtmlLang.Write(LangModule.Common, "PrintTemplateAdded", "新增成功！");
                    $.mMsg(successMsg, undefined, true);
                    parent.PTBase.reload("SalaryList");
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    PTSalaryEdit.init();
});