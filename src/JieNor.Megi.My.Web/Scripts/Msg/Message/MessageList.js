/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var MsgList = {
    init: function () {
        MsgList.initAction();
    },
    initAction: function () {
        $("#aSendMessage").click(function () {
            MsgList.createMessage()
        });
    },
    createMessage: function () {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.My, "SendMessage", "Send Message"),
            width: 720,
            height: 420,
            href: "/Message/MessageCreate",
            onClose: function () {
                var tab = $('#tabMsg').tabs('getSelected');
                var index = $('#tabMsg').tabs('getTabIndex', tab);
                if (index != 1) {
                    $("#tabMsg").tabs("select", 1);
                } else {
                    MsgList.bindSentList();
                }
            }
        });
    },
    replyMessage: function (id,type) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.My, "ReplyMessage", "Reply Message!"),
            width: 720,
            height: 560,
            href: "/Message/MessageReply/" + id+"?type="+type,
            onClose: function () {
                var tab = $('#tabMsg').tabs('getSelected');
                var index = $('#tabMsg').tabs('getTabIndex', tab);
                if (index == 0) {
                    MsgList.bindReceiveList();
                }
                else {
                    MsgList.bindSentList();
                }
            }
        });
    },
    afterCreateMsg:function(replyId){
        Megi.closeDialog();
        if (replyId != undefined && replyId.length > 0) {
            Megi.displaySuccess("#divMessage", HtmlLang.Write(LangModule.My, "ReplySuccessully", "Reply Successully!"));
        } else {
            Megi.displaySuccess("#divMessage", HtmlLang.Write(LangModule.My, "SendMessageSuccessully", "Send Message Successully!"));
        }
    },
    ViewMsgList: function (title, index) {
        if (index == 0) {
            MsgList.bindReceiveList();
        } else {
            MsgList.bindSentList();
        }
    },
    getMsgTypeIcon:function(msgType){
        if (msgType == 100) {
            return "<span class='mg-msg-basic'>&nbsp;</span>";
        }
        return "<span class='mg-msg-task'>&nbsp;</span>";
    },
    bindReceiveList: function () {
        Megi.grid("#tbReceiveMsg", {
            resizable: true,
            auto: true,
            pagination: true,
            url: "/Message/GetReceiveMessageList",
            columns: [[
            {
                title: HtmlLang.Write(LangModule.My, "Subject", "Subject"), field: 'MTitle', width: 200, sortable: true, formatter: function (value, rowData, rowIndex) {
                    return MsgList.getMsgTypeIcon(rowData.MType) + MsgList.setGridRowStyle(value, rowData.MIsRead);
                }
            },
            {
                title: HtmlLang.Write(LangModule.My, "From", "From"), field: 'MSenderInfo', width: 50, sortable: true, formatter: function (value, rowData, rowIndex) {
                    return MsgList.setGridRowStyle(rowData.MSenderInfo.MUserName, rowData.MIsRead);
                }
            },
            {
                title: LangKey.Date, field: 'MCreateDate', width: 50, sortable: true, formatter: function (value, rowData, rowIndex) {
                    return MsgList.setGridRowStyle($.mDate.format(value), rowData.MIsRead);
                }
            }]],
            onDblClickRow: function (rowIndex, rowData) {
                MsgList.replyMessage(rowData.MID,"reply");
            }
        });
    },
    bindSentList: function () {
        Megi.grid("#tbSentMsg", {
            resizable: true,
            auto: true,
            pagination: true,
            url: "/Message/GetSentMessageList",
            columns: [[
            {
                title: HtmlLang.Write(LangModule.My, "Subject", "Subject"), field: 'MTitle', width: 200, sortable: true, formatter: function (value, rowData, rowIndex) {
                    return MsgList.getMsgTypeIcon(rowData.MType) + value;
                }
            },
            {
                title: HtmlLang.Write(LangModule.My, "To", "To"), field: 'MReceiverInfo', width: 50, sortable: true, formatter: function (value, rowData, rowIndex) {
                    return rowData.MReceiverInfo.MUserName;
                }
            },
            {
                title: LangKey.Date, field: 'MCreateDate', width: 50, sortable: true, formatter: $.mDate.formatter
            }]],
            onDblClickRow: function (rowIndex, rowData) {
                MsgList.replyMessage(rowData.MID,"addMsg");
            }
        });
    },
    setGridRowStyle: function (value,isRead) {
        if (isRead) {
            return value;
        }
        return "<div class='mg-bold'>"+value+"</div>";
    }
}

$(document).ready(function () {
    MsgList.init();
});