/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var FeedBack = {
    init: function () {
        FeedBack.submitClick();

        $("#tbxEmail").keyup(function (e) {
            if (e.keyCode == 32) {
                $("#tbxEmail").val($.trim($("#tbxEmail").val()));
            }
        });
    },
    submitClick: function () {
        $("#aSubmit").click(function () {
            $("#divFeed").mFormSubmit({
                url: "/Feed/FeedBack/SendUserFeedBack", param: { model: {} }, callback: function (data) {
                    var successMsg = HtmlLang.Write(LangModule.My, "FeedBackSubmitSuccessfully", "FeedBack submit successfully");
                    if (data.Success) {
                        $.mMsg(successMsg);
                        $.mDialog.close();
                    } else {
                        $.mDialog.alert(data.Message);
                    }
                }
            });
            return false;
        });
    }
}

$(document).ready(function () {
    FeedBack.init();
});