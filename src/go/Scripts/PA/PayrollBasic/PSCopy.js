﻿var PSCopy = {
    itemID: $("#hidItemID").val(),
    init: function () {
        $("#MName").focus();
        PSCopy.initAction();
    },
    initAction: function () {
        $("#aSave").click(function () {
            PSCopy.save();
        });
    },
    save: function () {
        $("#divTitle").mFormSubmit({
            url: "/PA/PayrollBasic/CopyPrintSetting", param: { isCopyTmpl: true }, callback: function (msg) {
                if (msg.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.Common, "PrintSettingCopyed", "The print setting <strong>{0}</strong> was added.").replace('{0}', msg.Tag), undefined, true);
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
    PSCopy.init();
});