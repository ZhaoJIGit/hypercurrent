var IVSettingCopy = {
    itemID: $("#hidItemID").val(),
    init: function () {
        $("#MName").focus();
        IVSettingCopy.initAction();
    },
    initAction: function () {
        $("#aSave").click(function () {
            IVSettingCopy.save();
        });
    },
    save: function () {
        $("#divTitle").mFormSubmit({
            url: "/BD/PrintSetting/CopySetting", param: { isCopyTmpl: true }, callback: function (msg) {
                if (msg.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.Common, "PrintSettingCopyed", "The print setting <strong>{0}</strong> was added.").replace('{0}', msg.Tag), undefined, true);
                    parent.IVSetting.reload();
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    IVSettingCopy.init();
});