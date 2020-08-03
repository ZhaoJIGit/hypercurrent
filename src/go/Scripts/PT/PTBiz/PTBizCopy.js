var PTBizCopy = {
    itemID: $("#hidItemID").val(),
    init: function () {
        $("#MName").focus();
        PTBizCopy.initAction();
    },
    initAction: function () {
        $("#aSave").click(function () {
            PTBizCopy.save();
        });
    },
    save: function () {
        $("#divTitle").mFormSubmit({
            url: "/PT/PTBiz/CopyPT", param: { model: {}, isCopyTmpl: true }, callback: function (msg) {
                if (msg.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.Common, "PrintSettingCopyed", "The print setting <strong>{0}</strong> was added.").replace('{0}', msg.Tag), undefined, true);
                    parent.PTBizList.reload();
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    PTBizCopy.init();
});