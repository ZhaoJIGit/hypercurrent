/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var AccountEdit = {
    tabswitch: new BrowserTabSwitch(),
    PasswordWarpper: new PassworWrapper(),
    lastEventTime: null,
    init: function () {

        AccountEdit.tabswitch.initSessionStorage();


        $("#btnUpdateEmail").click(function () {
            var toUrl = $(this).attr("href");
            var password = $("#Password").val();
            if (!password) {
                var msg = HtmlLang.Write(LangModule.My, "PasswordEmpty", "password is required , please input password!");
                $.mDialog.alert(msg);
                return;
            }
            var newEmail = $("#Email").val();
            $("#divEmailModify").mFormSubmit({
                url: "/Account/AcctUpdateEmail", param: { info: {} }, callback: function (data) {
                    if (data && data.Success) {
                        //修改成弹窗，更加明显
                        $.mDialog.message(data.Message);
                        $("#bEmail").html(newEmail);
                        //清空密码栏
                        $("#Password").val("");
                        window.location.reload(true);
                    } else {
                        if (data.ErrorMessageDetail == null) {
                            var errorMsg = HtmlLang.Write(LangModule.My, "passwordError", "email modify failed，please check your password!");
                            $.mDialog.alert(errorMsg);
                        } else {
                            $.mDialog.alert(data.ErrorMessageDetail);
                        }
                        $("#Password").val("");
                    }
                }
            });
            return false;
        });
        $("#btnCancelEmail").click(function () {
            $("#Password").val("");
            AccountEdit.EditOrClose("close1");
        });
        $("#OKPass").click(function () {

            var currentPassword = $("#CurPass").val();
            var newPassword = $("#NewPass").val();
            var confirmPassword = $("#ConfNewPass").val();

            var errMsg = null;
            if (currentPassword == "") {
                $.mDialog.alert(HtmlLang.Write(LangModule.My, "CurrentPasswordEmpty", "Current password is required!"));

                return false;
            }
            if (newPassword == "") {
                $.mDialog.alert(HtmlLang.Write(LangModule.My, "NewPasswordEmpty", "New password is required!"));

                return false;
            }

            if (confirmPassword == "") {
                $.mDialog.alert(HtmlLang.Write(LangModule.My, "confirmPasswordEmpty", "Confirm new password is required!"));

                return false;
            }

            if (confirmPassword != newPassword) {
                $.mDialog.alert(HtmlLang.Write(LangModule.My, "PasswordSame", "Two Password must be the same!"));
                return false;
            }

            if (!AccountEdit.PasswordWarpper.isValidate(newPassword)) {
                var message = HtmlLang.Write(LangModule.Common, "PasswordIsSimple", "密码必须包括大写字母，小写字母，数字和特殊字符！");
                $.mDialog.alert(message);
                return;
            }

            var toUrl = $(this).attr("href");
            $("#divPassModify").mFormSubmit({
                url: "/Account/AcctUpdatePass", param: { info: {} }, callback: function (data) {
                    if (data && data.Success) {
                        $.mDialog.message(HtmlLang.Write(LangModule.My, "PasswordModifySuccess", "The password modify success!"));
                        $("#CurPass").val("");
                        $("#NewPass").val("");
                        $("#ConfNewPass").val("");
                    } else {
                        $.mDialog.alert(data.ErrorMessageDetail);
                    }
                }
            });
            return false;
        });
        $("#CancelPass").click(function () {
            $("#CurPass").val("");
            $("#NewPass").val("");
            $("#ConfNewPass").val("");
            AccountEdit.EditOrClose("close2");
        });

        $("#OKOther").click(function () {
            var toUrl = $(this).attr("href");
            var obj = {};
            obj.MAppID = $("input[type='radio'][checked]", "#divOtherModify").val();

            $("#divOtherModify").mFormSubmit({
                validate: true,
                param: obj,
                url: "/Account/AcctUpdateOther", param: { info: {} }, callback: function (msg) {
                    if (msg && msg.Success == true) {
                        $.mDialog.message(HtmlLang.Write(LangModule.My, "LoginGotoPage", "设置成功"));
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });

        $("#radio1").click(function () {
            $(this).attr("checked", "checked");
            $("#radio2").removeAttr("checked");
        });

        $("#radio2").click(function () {
            $(this).attr("checked", "checked");
            $("#radio1").removeAttr("checked");
        });

        $("#CancelOther").click(function () {
            AccountEdit.EditOrClose("close3");
        });

        $("#NewPass").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            AccountEdit.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (AccountEdit.lastEventTime - e.timeStamp == 0) {
                    var password = $("#NewPass").val();

                    if (!AccountEdit.PasswordWarpper.isValidate(password)) {
                        $("#pwd_tips").css("display", "block");
                    } else {
                        $("#pwd_tips").css("display", "none");
                    }
                }

            }, 800);

        });

        $("#ConfNewPass").off("keyup.validate focus.validate").on("keyup.validate focus.validate", function (e) {
            AccountEdit.lastEventTime = e.timeStamp;
            setTimeout(function () {
                if (AccountEdit.lastEventTime - e.timeStamp == 0) {
                    var password = $("#ConfNewPass").val();

                    if (!AccountEdit.PasswordWarpper.isValidate(password)) {
                        $("#rpwd_tips").css("display", "block");
                    } else {
                        $("#rpwd_tips").css("display", "none");
                    }
                }

            }, 800);

        });
    },
    EditOrClose: function (id) {

        //先隐藏掉

        if (id.indexOf("edit") != -1) {
            var diffStr = id.substr(4);
            $("#edit" + diffStr).hide();
            $("#close" + diffStr).show();
        } else {
            var diffStr = id.substr(5);
            $("#edit" + diffStr).show();
            $("#close" + diffStr).hide();
        }
    }
}

$(document).ready(function () {
    AccountEdit.init();
});