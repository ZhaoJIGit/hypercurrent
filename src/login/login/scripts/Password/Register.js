var AccountEdit = {
    PasswordWarpper: new PassworWrapper(),
    lastEventTime: null,
    init: function () {

        $("#aCreatePassword").click(function () {

            $('input.easyui-validatebox').validatebox('enableValidation').validatebox('validate');
            //
            if (!$("#pwd").validatebox("isValid")) {
                return $("#pwd").focus();
            }
            if (!$("#rpwd").validatebox("isValid")) {
                return $("#rpwd").focus();
            }
            if ($("#pwd").val() != $("#rpwd").val()) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Login, "PasswordnotMatch", "Password not Match"));
                return false;
            }

            if (!AccountEdit.PasswordWarpper.isValidate($("#pwd").val())) {
                var message = HtmlLang.Write(LangModule.Common, "PasswordIsSimple", "密码必须包括大写字母，小写字母，数字和特殊字符,长度在8-20字符！");
                $.mDialog.alert(message);
                return;
            }

            var param = $("body").mFormGetForm();

            var toUrl = $(this).attr("href");
            //
            mAjax.submit("/Password/SureRegister", param, function (msg) {
                if (msg.Success) {
                    window.location = "/Password/CreateSuccess?email="+$("#hidEmail").val();
                } else {
                    $.mDialog.alert(msg.Message);
                }
            });
            return false;
        });
        $("#ResetPwd").click(function () {
            if ($("#pwd").attr("value").length < 8) {
                var message = HtmlLang.Write(LangModule.Login, "PasswordLength", "Password must be at least 8 characters");
                $.mDialog.alert(message, null, LangModule.Login, "PasswordMust");
                return false;
            }
            if ($("#rpwd").attr("value").length < 8) {
                var message = HtmlLang.Write(LangModule.Login, "ConfirmPasswordLength", "Comfirm Password must be at least 8 characters!");
                $.mDialog.alert(message, null, LangModule.Login, "ComfirmPasswordMust");
                return false;
            }
            if ($("#pwd").attr("value") != $("#rpwd").attr("value")) {
                var message = HtmlLang.Write(LangModule.Login, "PasswordDonotMatch", "Two password that your Input must be the same!");
                $.mDialog.alert(message, null, LangModule.Login, "TwoPasswordThat");
                return false;
            }
            var toUrl = $(this).attr("href");
            $("#divResetPwd").mFormSubmit({
                url: "/Password/PutNewPwd", callback: function (msg) {
                    if (msg.Success) {
                        window.location = toUrl;
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });

        //绑定回车事件
        $("body").off("keyup").on("keyup", function (event) {
            //如果是回车
            if (event.keyCode == 13) {
                //设置密码
                //$("#Sure").trigger("click");
                //if ($("#aCreatePassword") && !$("#popup_content")) {
                $("#aCreatePassword").trigger("click");
                //}
                //返回失败
                return false;
            }
          }
        );

        $("#pwd").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            AccountEdit.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (AccountEdit.lastEventTime - e.timeStamp == 0) {
                    var password = $("#pwd").val();

                    if (!AccountEdit.PasswordWarpper.isValidate(password)) {
                        $("#pwd_tips").css("display", "block");
                    } else {
                        $("#pwd_tips").css("display", "none");
                    }
                }

            }, 800);

        });

        $("#rpwd").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            AccountEdit.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (AccountEdit.lastEventTime - e.timeStamp == 0) {
                    var password = $("#rpwd").val();

                    if (!AccountEdit.PasswordWarpper.isValidate(password)) {
                        $("#rpwd_tips").css("display", "block");
                    } else {
                        $("#rpwd_tips").css("display", "none");
                    }
                }

            }, 800);

        });
    }
}

$(document).ready(function () {

    $('input.easyui-validatebox').validatebox('disableValidation')
           .focus(function () { $(this).validatebox('enableValidation'); })
           .blur(function () { $(this).validatebox('validate') });
    AccountEdit.init();
});