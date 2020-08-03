var PTVoucherEdit = {
    itemID: $("#hidItemID").val(),
    model: null,
    init: function () {
        PTVoucherEdit.initUI();
        PTVoucherEdit.initAction();
        PTVoucherEdit.initSetting();
    },
    initUI: function () {
        //$("#MTopMargin,#MBottomMargin").css({ "width": 136 });
    },
    initAction: function () {
        $("#aSave").click(function () {
            PTVoucherEdit.save();
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
            url: "/PT/PTVoucher/GetPT/" + PTVoucherEdit.itemID, fill: true, callback: function (data) {
                PTVoucherEdit.model = data;
                if (!PTVoucherEdit.itemID) {
                    $("#MName").focus();
                }
                $("#divPreviewImage").addClass(data.MPreviewImage);
                if (data.MIsSys) {
                    //预设模板禁用字段
                    $("#MName").attr("disabled", "disabled");
                    $("#MTemplateType").combobox({ disabled: true });
                    $("#MPaperType").combobox({ disabled: true });
                    PTVoucherEdit.switchCtrlStatus(false, $("#divPaperDirection input:radio"));
                }
                var chkObjs = $("input:checkbox");
                $.each(chkObjs, function (i, chk) {
                    $(chk).attr("checked", data[chk.id]);
                });
                PTVoucherEdit.setRdoVal("MPaperDirection", data.MPaperDirection);
                PTVoucherEdit.setRdoVal("MIsPrintLine", data.MIsPrintLine);
                PTVoucherEdit.setComboVal($("#MTemplateType"), data.TemplateTypeList, data.MTemplateType);
                PTVoucherEdit.setComboVal($("#MPaperType"), data.PaperTypeList, data.MPaperType);
            }
        });
    },
    setComboVal: function (obj, list, selItem) {
        obj.combobox("loadData", list);
        obj.combobox("setValue", selItem);
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
        var obj = PTVoucherEdit.model;
        var chkObjs = $("input:checkbox");
        $.each(chkObjs, function (i, chk) {
            obj[chk.id] = $(chk).attr("checked") ? 1 : 0;
        });
        obj.MPaperDirection = PTVoucherEdit.getRdoVal("MPaperDirection");
        obj.MIsPrintLine = PTVoucherEdit.getRdoVal("MIsPrintLine") === "1";

        $("body").mFormSubmit({
            url: "/PT/PTVoucher/UpdatePT", param: { model: obj }, callback: function (msg) {
                if (msg.Success) {
                    var successMsg = PTVoucherEdit.itemID ? HtmlLang.Write(LangModule.Common, "PrintTemplateUpdated", "更新成功！") : HtmlLang.Write(LangModule.Common, "PrintTemplateAdded", "新增成功！");
                    $.mMsg(successMsg, undefined, true);
                    parent.PTVoucherList.reload();
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    PTVoucherEdit.init();
});