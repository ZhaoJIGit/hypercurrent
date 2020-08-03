/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var ConfirmInvite = {
    init: function () {
        ConfirmInvite.clickAction();
    },
    clickAction: function () {
        $("#aCreateLogin").click(function () {
            $("#step1").hide();
            $("#step2A").show();
            return false;
        });
        $("#aLoginNow").click(function () {
            var toUrl = $("#NowLoginServer").val();
            window.location = toUrl;
            return false;
        });
        $("#aCreate").click(function () {
            if ($("#pwd").attr("value").length < 8) {
                $.mDialog.alert("Password must be at least 8 characters！", null, LangModule.Login, "PasswordMust");
                return false;
            }
            if ($("#rpwd").attr("value").length < 8) {
                $.mDialog.alert("Comfirm Password must be at least 8 characters！", null, LangModule.Login, "ComfirmPasswordMust");
                return false;
            }
            if ($("#pwd").attr("value") != $("#rpwd").attr("value")) {
                $.mDialog.alert("Two Password must be at least 8 characters！", null, LangModule.Login, "TwoPasswordMust");
                return false;
            }

            if ($("#ckAgreeTerm").attr("checked") != "checked") {
                $(".reg-error-image").show();
                return false;
            }

            var toUrl = $("#CreateLoginServer").val();

            $("#divUserRegister").mFormSubmit({
                url: "/Invite/AcceptInvite", param: { model: {} }, callback: function (data) {
                    if (data.Success) {
                        if (Megi.trim($("#defaultEmail").val()).toLowerCase() == Megi.trim($("#newEmail").val()).toLowerCase()) {
                            window.location = toUrl;
                        }
                        else {
                            $("#diffEmail").html(Megi.trim($("#newEmail").val()));
                            $("#step1").hide();
                            $("#step2A").hide();
                            $("#step3").show();
                        }
                    } else {
                        $.mDialog.alert(data.Message);
                    }
                }
            });
            return false;
        });
        $("#aCancel").click(function () {
            $("#step1").show();
            $("#step2A").hide();
            return false;
        });

        $("#ckAgreeTerm").off("click").on("click", function () {
            if ($(this).attr("checked") == "checked") {

                $(".reg-error-image").hide();
            }
        });
    }
}



$(document).ready(function () {
    ConfirmInvite.init();
})