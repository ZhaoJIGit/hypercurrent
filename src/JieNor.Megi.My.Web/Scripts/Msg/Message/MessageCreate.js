/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var MsgCreate = {
    receiverList: null,
    getSenderInfo: function () {
        var senderInfo = $("#hidSenderInfo").val();
        if (senderInfo != undefined && senderInfo != null && senderInfo.length > 0) {
            return eval("(" + senderInfo + ")");
        }
        return null;
    },
    getMsgID: function () {
        return $("#hidMsgID").val();
    },
    init: function () {
        MsgCreate.receiverList = new Array();
        MsgCreate.initAction();
        var senderInfo = MsgCreate.getSenderInfo();
        if (senderInfo != null) {
            MsgCreate.addReceiver(senderInfo)
        }
        var objMsgBoard = document.getElementById("divMsgBoard");
        if (objMsgBoard != undefined) {
            objMsgBoard.scrollTop = objMsgBoard.scrollHeight;
        }
    },
    initAction: function () {
        $("#divUserList").find(".mg-msg-user").click(function () {
            var model = eval("(" + $(this).attr("data-model") + ")");
            var isSelected = $(this).hasClass("mg-msg-user-selected");
            if (isSelected) {
                MsgCreate.removeReceiver(model.MUserID);
                $(this).removeClass("mg-msg-user-selected");
            } else {
                MsgCreate.addReceiver(model);
                $(this).addClass("mg-msg-user-selected");
            }
        });
        $("#aSend").click(function () {
            MsgCreate.sendMsg();
        });
    },
    addReceiver: function (model) {
        var isExists = false;
        for (var i = 0; i < MsgCreate.receiverList.length; i++) {
            if (MsgCreate.receiverList[i].MUserID == model.MUserID) {
                isExists = true;
                break;
            }
        }
        if (!isExists) {
            MsgCreate.receiverList.push(model);
        }
        MsgCreate.bindReceiver();
    },
    removeReceiver: function (userId) {
        var arr = new Array();
        for (var i = 0; i < MsgCreate.receiverList.length; i++) {
            if (MsgCreate.receiverList[i].MUserID != userId) {
                arr.push(MsgCreate.receiverList[i]);
            }
        }
        MsgCreate.receiverList = arr;
        MsgCreate.bindReceiver();
    },
    bindReceiver: function () {
        var html = '';
        for (var i = 0; i < MsgCreate.receiverList.length; i++) {
            var item = MsgCreate.receiverList[i];
            html += '<a  href="javascript:void(0)" data-Id="' + item.MUserID + '"><lable>' + item.MUserName + '</lable><lable class="email">&lt;' + item.MEmail + '&gt;</lable>;</a>';
        }
        $("#divReceiverList").html(html);

        $("#divReceiverList").find("a").click(function () {
            var userId = $(this).attr("data-Id");
            MsgCreate.removeReceiver(userId);
            MsgCreate.removeSelectedReceiverStyle(userId);
        });
    },
    removeSelectedReceiverStyle: function (userId) {
        $("#divUserList").find(".mg-msg-user").each(function () {
            var model = eval("(" + $(this).attr("data-model") + ")");
            if (model.MUserID == userId) {
                $(this).removeClass("mg-msg-user-selected");
            }
        });
    },
    sendMsg: function () {
        var title = $("#txtTitle").val();
        var content = $("#txtContent").val();
        var isSendEmail = $("#txtIsSendEmail").attr("checked");
        var obj = {};
        obj.MReplyID = MsgCreate.getMsgID();
        obj.MTitle = title;
        obj.MContent = content;
        obj.MReceiverList = MsgCreate.receiverList;
        obj.MIsSendEmail = isSendEmail;
        mAjax.submit("/Message/SendMessage", { msgModel: obj }, function (msg) {
            parent.MsgList.afterCreateMsg(obj.MReplyID);
        });
    }
}

$(document).ready(function () {
    MsgCreate.init();
});