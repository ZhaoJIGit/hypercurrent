//var MgBox = {
//    alert: function (msg, options, langModule, langKey) {
//        MgBox.createDialog(msg, options, "Error", "error", false, langModule, langKey);
//        MgBox.closeDialogEvent();
//    },
//    warning: function (msg, options, langModule, langKey) {
//        MgBox.createDialog(msg, options, "Warning", "warning", false, langModule, langKey);
//        MgBox.closeDialogEvent();
//    },
//    confirm: function (msg, callback, options, langModule, langKey) {
//        MgBox.createDialog(msg, options, "Confirm", "confirm", true, langModule, langKey);
//        $("#divMessageContainer").find(".action>.m-btn-green").click(function () {
//            $("#divMessageContainer").dialog("close");
//            if (callback != null) {
//                callback();
//            }
//        });
//        $("#divMessageContainer").find(".action>.m-btn-gray").click(function () {
//            $("#divMessageContainer").dialog("close");
//        });
//    },
//    closeDialogEvent: function () {
//        $("#divMessageContainer").find(".action>.m-btn-green").click(function () {
//            $("#divMessageContainer").dialog("close");
//        });
//    },
//    createDialog: function (msg, options, title, iconCls, showCancel, langModule, langKey) {
//        if (langModule != undefined && langKey != undefined) {
//            msg = HtmlLang.Write(langModule, langKey, msg);
//        }
//        $("#divMessageContainer").remove();
//        if (options == undefined) {
//            options = {};
//        }
//        if (options.width == undefined) {
//            options.width = 350;
//        }
//        if (options.height == undefined) {
//            options.height = 190;
//        }
//        if (options.title == undefined) {
//            options.title = title;
//        }
//        var contentHeight = options.height - 150
//        var html = "<div id='divMessageContainer' class='m-dialog'>";
//        html += "<div class='content " + iconCls + "' style='height:" + contentHeight + "px'>" + msg + "</div>";
//        html += "<div class='action'>";
//        if (showCancel) {
//            html += "<a href='javascript:void(0)' class='m-btn m-btn-gray'>Cancel</a>";
//        }
//        html += "<a href='javascript:void(0)' class='m-btn m-btn-green'>OK</a></div>";
//        html += "</div>";
//        $(html).appendTo('body');
//        options.modal = true;
//        $('#divMessageContainer').dialog(options);
//    }
//}
//$.mDialog.alert = function (msg, options) {
//    MgBox.alert(msg, options);
//};
//$.mConfirm = function (msg, options) {
//    MgBox.confirm(msg, options)
//}
//$.mConfirmDelete = function (callBack) {

//}