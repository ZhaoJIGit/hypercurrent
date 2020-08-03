var PTSalaryCopy = {
    itemID: $("#hidItemID").val(),
    init: function () {
        $("#MName").focus();
        PTSalaryCopy.initAction();
    },
    initAction: function () {
        $("#aSave").click(function () {
            PTSalaryCopy.save();
        });
    },
    save: function () {
        $("#divTitle").mFormSubmit({
            url: "/PT/PTSalaryList/CopyPT", param: { model: {}, isCopyTmpl: true }, callback: function (msg) {
                if (msg.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.Common, "PrintSettingCopyed", "The print setting <strong>{0}</strong> was added.").replace('{0}', msg.Tag), undefined, true);
                    parent.PTSalaryList.reload();
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    }
}
$(document).ready(function () {
    PTSalaryCopy.init();
});