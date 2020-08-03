var ForgetPassword = {
    Init: function () {
        ForgetPassword.bindAction();
    },
    bindAction: function () {
        $("#Sendlink").bind("click", ForgetPassword.sendResetPasswordLink);

        $("#btnCancel").click(function () {
            var url = $(this).attr("url");

            window.location.href = url;
        });
        $("#tbxEmail").keyup(function (e) {
            if (e.keyCode == 32) {
                $("#tbxEmail").val($.trim($("#tbxEmail").val()));
            }
        });
    },
    sendResetPasswordLink: function () {

        $('input.easyui-validatebox').validatebox('enableValidation').validatebox('validate');
        //去邮箱的空格
        $("#tbxEmail").val($.trim($("#tbxEmail").val()));

        //邮箱验证不通过
        if (!$("#divSendlink").form('validate')) {
            return;
        }
        $("#main-reg").mask("processing...");
        //发送请求
        mAjax.submit("/Password/ForgotPwdAndSendMail", { MEmailAddress: $("#tbxEmail").val() }, function (response) {
            $("#main-reg").unmask();
            if (response.Success == true) {
                //成功跳转到提示页面
                window.location.href = "/Password/OperateSuccess";
            } else {
                var msg = response.Message
                if (!response.Message) {
                    msg = HtmlLang.Write(LangModule.Login, "ForgetPasswordSendFail", "Message send failed , please try again!");
                }

                $.mDialog.alert(msg);
            }


        }, "", true);
    }
}

$(function () {
    ForgetPassword.Init();

    $('input.easyui-validatebox').validatebox('disableValidation')
           .focus(function () { $(this).validatebox('enableValidation'); })
           .blur(function () { $(this).validatebox('validate') });
})
