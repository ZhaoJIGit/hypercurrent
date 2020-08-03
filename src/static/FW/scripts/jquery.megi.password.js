var PassworWrapper = (function(){
	function PassworWrapper() {
        this.regText = '(?=.*[0-9])(?=.*[a-zA-Z])(?=.*[^a-zA-Z0-9]).{8,30}';
	}

	PassworWrapper.prototype.isValidate = function (pw) {
		var _this = this;

		var reg = new RegExp(_this.regText);

		return reg.test(pw);
    }

    PassworWrapper.prototype.sendResetLink = function(email) {

        email = $.trim(email);

        mAjax.submit("/Password/ForgotPwdAndSendMail", { MEmailAddress: email }, function (response) {
            $("#main-reg").unmask();
            if (!response.Success) {
                var msg = response.Message
                if (!response.Message) {
                    msg = HtmlLang.Write(LangModule.Login, "ForgetPasswordSendFail", "Message send failed , please try again!");
                }

                $.mDialog.alert(msg);
            }
        }, "", true);
    }

	return PassworWrapper;
}())