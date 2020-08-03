var ResetPassword = {
    PasswordWarpper: new PassworWrapper(),
    lastEventTime: null,
    init:function(){
        ResetPassword.bindAction();
    },
    bindAction:function(){
        $("#btnCancel").click(function(){
            var url = $(this).attr("url");
            window.location.href=url;
        });

        $("#ResetPwd").click(ResetPassword.sendResetPasswordRequest);

        $("#pwd").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            ResetPassword.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (ResetPassword.lastEventTime - e.timeStamp == 0) {
                    var password = $("#pwd").val();

                    if (!ResetPassword.PasswordWarpper.isValidate(password)) {
                        $("#pwd_tips").css("display","block");
                    } else {
                        $("#pwd_tips").css("display","none");
                    }
                }

            }, 800);

        });

        $("#rpwd").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            ResetPassword.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (ResetPassword.lastEventTime - e.timeStamp == 0) {
                    var password = $("#rpwd").val();

                    if (!ResetPassword.PasswordWarpper.isValidate(password)) {
                        $("#rpwd_tips").css("display", "block");
                    } else {
                        $("#rpwd_tips").css("display", "none");
                    }
                }

            }, 800);

        });
        
    },
    sendResetPasswordRequest: function () {

        $('input.easyui-validatebox').validatebox('enableValidation').validatebox('validate');
        //验证没通过，不执行方法
        if (!$("#divResetPwd").form('validate')) {
            return;
        }
        var newPassword = $("#pwd").val();
        var rNewPassword = $("#rpwd").val();

        if (newPassword != rNewPassword) {
            var message = HtmlLang.Write(LangModule.Login, "PasswordnotMatch", "Two password that your Input must be the same!");
            $.mDialog.alert(message);
            return;
        }

        if (!ResetPassword.PasswordWarpper.isValidate(newPassword)) {
            var message = HtmlLang.Write(LangModule.Common, "PasswordIsSimple", "密码必须包括大写字母，小写字母，数字和特殊字符！");
            $.mDialog.alert(message);
            return;
        }

        //$("#resetPassword").mask("processing...");

        //提交表单
        $("#divResetPwd").mFormPost({
            validate: true,
            url: "/Password/PutNewPwd", callback: function (data) {
                if (data.Success) {
                    window.location.href = "/Password/ResetPwdSuccess?email=" + $("#hidEmail").val();

                } else {
                    if (data.ErrorMessageDetail == null) {
                        var errorMsg = HtmlLang.Write(LangModule.My, "passwordError", "email modify failed，please check your password!");
                        $.mDialog.alert(errorMsg);
                    } else {
                        $.mDialog.alert(data.ErrorMessageDetail);
                    }
                    $("#pwd").val("");
                    $("#rpwd").val("");
                }
                //$("#resetPassword").unmask();
            }
        }, true);
        
    }
}

$(function(){
    ResetPassword.init();
    $('input.easyui-validatebox').validatebox('disableValidation')
           .focus(function () { $(this).validatebox('enableValidation'); })
           .blur(function () { $(this).validatebox('validate') });
})