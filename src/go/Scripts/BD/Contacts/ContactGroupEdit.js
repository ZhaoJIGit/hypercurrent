var GoContactGroupEdit = {
    init: function () {
        GoContactGroupEdit.saveClick();
        GoContactGroupEdit.cancelClick();
    },
    saveClick: function () {
        $("#aSave").click(function () {
            $("#divContactGroupEdit").mFormSubmit({
                url: "/BD/Contacts/ContactsGroupUpdate", param: { model: {} }, callback: function (msg) {
                    if (msg.Success) {
                        var groupTitle = $("#txtMName").val();
                        var successMsg = HtmlLang.Write(LangModule.Acct, "GroupAdded", "Group added") + ": " + $("#txtMName").val();
                        $.mMsg(successMsg);
                        parent.GoContactsList.reload(groupTitle, true);
                        Megi.closeDialog();
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            Megi.closeDialog();
            parent.GoContactsList.reload();
            return false;
        });
    }

}


$(document).ready(function () {
    GoContactGroupEdit.init();
});