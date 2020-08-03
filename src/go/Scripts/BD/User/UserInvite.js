var UserInvite = {
    //用户ID
    ItemId: $("#hidItemID").val(),
    //初始化页面数据
    init: function () {
        UserInvite.clickAction();
    },
    //初始化按钮操作
    clickAction: function () {
        //发送邀请
        $("#aSendInvite").click(function () {
            var email = $("#hidEmail").val();
            var orgId = $("#hidOrgId").val();
            var orgName = $("#hidOrgName").val();
            var message = $("#txtMessage").val();
            $("#DivUserInvite").mFormSubmit({
                url: "/BD/User/UserInviteSendMeg",
                param: { UserId: UserInvite.ItemId, Email: email, OrgId: orgId, OrgName: orgName, Message: message },
                callback: function (msg) {
                    if (msg == true) {
                        //发送成功，刷新用户编辑页面，关闭对话框
                        UserInvite.reloadAndClose(true);
                    } else {
                        //发送失败，提示错误
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
        });
        //取消
        $("#aCancel").click(function () {
            var params = { MItemID: UserInvite.ItemId };
            if (!$("#hidIsResent").val()) {
                //同步去删除
                mAjax.post("/BD/User/UserLinkInfoDelete/", { model: params }, function (msg) {
                }, "", "", false);
            }
            UserInvite.reloadAndClose(false);
        });
    },
    //刷新用户编辑页面，关闭对话框
    reloadAndClose: function (isTip) {
        if (isTip) {
            var msg = HtmlLang.Write(LangModule.User, "InviteSuccessfully", "Invite Successfully!");
            $.mMsg(msg);
            parent.mWindow.reload("/BD/User/UserEdit/" + UserInvite.ItemId);
        }
        Megi.closeDialog();
    }
}
//初始化页面数据
$(document).ready(function () {
    UserInvite.init();
});