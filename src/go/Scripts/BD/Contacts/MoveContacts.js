var GoMoveContacts = {
    init: function () {
        GoMoveContacts.saveClick();
        GoMoveContacts.cancelClick();
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var model = {};
            //要移动的联系人Id
            model.ContactIds = $("#selectIds").val();
            //新分组多语言对象
            model.NewGroupMultiLangModel = $("#NewType").getLangEditorData();
            //源分组Id
            model.MoveFromGroupId = $("#typeId").val();
            //分组是否存在
            model.IsGroupExist = false;
            //当前分组名
            var curLangTypeName = $.trim($('#NewType').val());

            //没有输入新分组名，则取选择的分组
            if (curLangTypeName == "") {
                model.GroupId = $("#SelectType").find("option:selected").val();
                curLangTypeName = $("#SelectType").find("option:selected").text();
                model.IsGroupExist = true;
            }

            mAjax.submit(
                "/BD/Contacts/ContactGroupMoveFromTo", { model: model }, function (msg) {
                    if (msg.Success == true) {
                        var successMsg = HtmlLang.Write(LangModule.Contact, "MoveContactToGroup", "Contact moved to group") + ": " + curLangTypeName;
                        $.mMsg(successMsg);
                        //不存在的时候需要刷新整个页面
                        parent.GoContactsList.reload(curLangTypeName, !model.IsGroupExist);
                        Megi.closeDialog();
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            var selTabTitle = $("#curTitle").val();
            Megi.closeDialog();
            parent.GoContactsList.reload(selTabTitle);
            return false;
        });
    }

}


$(document).ready(function () {
    GoMoveContacts.init();
});