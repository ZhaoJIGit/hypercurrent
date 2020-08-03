PublishMessage = {
    setMsg: function (msg) {
        $("body").append("<div class='m-notice'>" + msg + "</div>");
        var bodyW = $("body").width();
        var msgW = $(".m-notice").width();
        $(".m-notice").css("left", (bodyW - msgW) / 2 + "px");
    },
    setNoticeMsg:function(data){
        var msg = HtmlLang.Write(LangModule.Common, "SystemUpdatePromt", "System will be update between {0} and {1} ").format(data.MValue1, data.MValue2);
        if (data.MValue4) {
            msg = HtmlLang.Write(LangModule.Common, "SystemUpdatePromtDay", "Notice：the system will be upgrade between {1} to {2} on {0}, please do not use it during this period. ").format(data.MValue4, data.MValue1, data.MValue2);
        }
        PublishMessage.setMsg(msg);
    },
    show: function (data) {
        $(".m-notice").remove();
        switch (data.MStatu) {
            case 1:
                PublishMessage.setNoticeMsg(data);
                break;
            case 2:
                PublishMessage.setNoticeMsg(data);
                $.mDialog.alert(HtmlLang.Write(LangModule.Common, "SystemUpdateAlert", "System will be update after {0} minutes.").format(data.MValue3));
                break;
            case 3:
                var msg = HtmlLang.Write(LangModule.Common, "SystemUpdating", "System is updating...");
                PublishMessage.setMsg(msg);
                break;
            case 4:
                var msg = HtmlLang.Write(LangModule.Common, "SystemUpgradeCompleted", "System upgrade completed!");
                PublishMessage.setMsg(msg);
                break;
            case 5:
                break;
        }

    }
}

$(function () {
    $(function () {
        $.connection.hub.url = $("#hidHubServer").val();
        //日志
        //$.connection.hub.logging = true;
        var chat = $.connection.ServerBroadcastHub;
        if (!chat) {
            return;
        }

        //服务器返回的信息调用消息处理
        chat.client.broadcastMessage = function (message) {
            if (message) {
                data = eval("(" + message + ")");
                PublishMessage.show(data);
                return;

                if (data.Success) {

                    var pushMsg = eval("("+data.Message+")");
                    var showMsg = "";

                    //找出与本地语言相关的推送消息
                    if (pushMsg && pushMsg.length > 0) {
                        var local = $.cookie('MLocaleID');
                        if (!local) {
                            local = "0x0009";
                        }
                        for (var i = 0 ; i < pushMsg.length; i++) {
                            var row = pushMsg[i];
                            if (row.MLocaleID == local) {
                                showMsg = row.MContent;
                                break;
                            }
                        }

                        $.messager.show({
                            title: HtmlLang.Write(LangModule.BD, "ImportantNotice", "Important notice"),
                            msg: showMsg,
                            timeout: 20000,
                            showType: 'slide'
                        });
                    }

                   
                }

            }

        };

        $.connection.hub.start().done(function () {
            chat.server.send("haha", "hahh");
        });
    });
});