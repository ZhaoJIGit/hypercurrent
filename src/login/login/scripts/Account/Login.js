/// <reference path="../../../jq/jquery.megi.common.js" />
$.support.cors = true;

var MegiLogin = {
    tabswitch: new BrowserTabSwitch(),
    //验证码加载标志
    CodeLoaded: false,
    PasswordWarpper: new PassworWrapper(),
    AllowLoginErrorAccount: $("#hideAllowLoginErrorCount").val(),
    init: function () {
        //上次登陆的邮箱
        var inviteEamil = $("#hideInviteEmail").val();

        if (inviteEamil) {
            $("#txtEmail").val(decodeURIComponent(inviteEamil));
        } else {
            var lastLoginEmail = $("#hidEmail").val();
            if (lastLoginEmail) {
                //从cookie中获取用户上次登陆的email
                $("#txtEmail").val(decodeURIComponent(lastLoginEmail));
            }
            var he = $("#hidHE").val();
            //
            if (he && he.length > 0) {
                //
                var emails = mText.Base64Decode(he).split(',');
                //
                var emailList = $("#userEmailList");
                //
                var optionHtml = "";
                //
                for (var i = 0; i < emails.length ; i++) {
                    //
                    optionHtml += "<option value='" + decodeURIComponent(emails[i]) + "'>";
                }
                //
                emailList.empty().append(optionHtml);
            }
        }

        $("#userNameHidden").val($("#txtEmail").val());


        //密码输入框的特殊处理
        window.focusSwitch(".password-text", ".password-password");
        $("#aLogin").click(function () {
            MegiLogin.login();
            return false;
        });

        $("#Sendlink").click(function () {
            var toUrl = $(this).attr("href");
            $("#divSendlink").mFormSubmit({
                url: "/Password/ForgotPwdAndSendMail", param: { model: {} }, callback: function (msg) {
                    if (msg.Success) {
                        window.location = toUrl;
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });
        //整个页面的的input（text 和button 都响应回车事件)
        $("input").on("keyup", function (event) {
            //如果是回车
            if (event.keyCode == 13) {
                //登陆
                MegiLogin.login();
                //返回失败
                return false;
            }
        });

        //载入页面是是否加载验证码
        var loginErrorAccount = $("#hideLoginErrorCount").val();

        if (loginErrorAccount && loginErrorAccount > MegiLogin.AllowLoginErrorAccount) {
            $("#divCode").show();
            $("#tbxCode").trigger("click");
            MegiLogin.CodeLoaded = true;
        }


        //重新获取图片
        $("#validateCodeImage").bind("click", function () {
            MegiLogin.refreshValidateCode();
        });

        $("#btnChangeImage").click(function () {
            MegiLogin.refreshValidateCode();
        });

        //当点击tbxcode的时候开始加载验证码
        $("#tbxCode").click(function () {
            MegiLogin.showValidateCode();
        });

        //实时验证验是否正确
        $("#tbxCode").off("keyup").on("keyup", function () {
            var code = $(this).val();
            //只有等于四位的时候，才真正请求后台进行验证
            var dom = $("#isCorrent");
            if (code.length != 4) {
                if (code.length > 0) {
                    dom.removeClass().addClass("m-validate-incorrent");
                } else {
                    dom.removeClass();
                }
                dom.attr("code", "false");
                return;
            }
            var email = $("#txtEmail").val();
            $.mAjax.post("/Account/ValidateCodeIsCorrect", { email: email, code: code }, function (msg) {
                if (msg.IsSuccess) {
                    dom.removeClass().addClass("m-validate-corrent");
                    //标示一下
                    dom.attr("code", "true");
                    $(".error-msg").html("");
                } else {
                    dom.removeClass().addClass("m-validate-incorrent");
                    dom.attr("code", "false");
                }
            })

        });
        $("#txtEmail").keyup(function (e) {
            if (e.keyCode == 32) {
                $("#txtEmail").val($.trim($("#txtEmail").val()));
            }
        });

        //一旦密码框获取了焦点，检查是否需要显示验证码
        $("#txtEmail").focusout(function () {
            MegiLogin.getLoginErrorCount();
        });

        $("#txtPwd").focus(function () {
            MegiLogin.getLoginErrorCount();
        });


        //加载页面的时候提示IE9一下的浏览器不可用
        if (Megi.isLowVersionIE9()) {
            var tips = HtmlLang.Write(LangModule.Login, "NoSupportIE8Tips", "系统不支持IE8及其以下版本浏览器，请更新浏览器版本!");
            alert(tips);
            return;
        }

        // 检查refer，如果来自于go或者my就发送跟踪消息"Logged out"

    },
    showValidateCode: function () {
        if (!MegiLogin.CodeLoaded) {
            $("#divCode").show();
            $("#divCodeImage").show();
            MegiLogin.refreshValidateCode();
            MegiLogin.CodeLoaded = true;
        }
    },
    getLoginErrorCount: function () {
        var email = $("#txtEmail").val();
        if (email) {
            $.mAjax.post("/Account/GetLoginErrorCount", { email: email }, function (msg) {
                if (msg.errorCount > msg.allowErrorCount) {
                    MegiLogin.showValidateCode();
                }
            }, false, false, true);
        }
    },
    login: function () {

        //加载页面的时候提示IE9一下的浏览器不可用
        if (Megi.isLowVersionIE9()) {
            var tips = HtmlLang.Write(LangModule.Login, "NoSupportLowVersionIE", "系统不支持IE8以及以下浏览器，请更新浏览器版本!");
            alert(tips);
            return;
        }

        $("#txtEmail").removeClass("error");
        $("#txtPwd").removeClass("error");
        $(".error-msg").html("");

        $("#txtEmail").val($.trim($("#txtEmail").val()));

        if (!$("#txtEmail").validatebox("isValid")) {
            return $("#txtEmail").focus();
        }
        if (!$("#txtPwd").validatebox("isValid")) {
            MegiLogin.loginError();
            return $("#txtPwd").focus();
        }


        var validateCode = $("#tbxCode").val();
        var hasValidateCode = $("#divCode").css("display") == "block";

        //只有在需要显示验证码和验证码不存在时进行校验
        if (hasValidateCode && (!validateCode || $("#isCorrent").attr("code") == "false")) {
            var msg = HtmlLang.Write(LangModule.Login, "validateCodeError", "validate code is empty or incorrect");
            $("#isCorrent").removeClass().addClass("m-validate-incorrent");
            $(".error-msg").html(msg);

            $("#tbxCode").focus();

            $("#tbxCode")
            return false;
        }

        $("html").mask(HtmlLang.Write(LangModule.Common, "Login", "Login..."));
        var emailCookieName = 'MUserEmail';
        var email = $.trim($("#txtEmail").val());
        var pwd = $("#txtPwd").val();
        var userId = $("#userId").val();
        var orgId = $("#orgId").val();
        var sendLinkID = $("#sendLinkID").val();
        var redirectUrl = $("#redirectUrl").val();
        var relogin = $("#relogin").val();

        var obj = {};
        obj.Email = email;
        obj.Password = pwd;
        obj.UserId = userId;
        obj.OrgId = orgId;
        obj.SendLinkID = sendLinkID;
        obj.RedirectUrl = redirectUrl;
        obj.Relogin = relogin ? true : false;
        obj.ValidateCode = hasValidateCode ? validateCode : "";
        obj.MLCID = MegiLogin.tabswitch.getBrowserTabLang();

        $("#userNameHidden").val($("#txtEmail").val());

        mAjax.submit("/Account/SignIn", obj, function (msg) {
            var data = msg;
            if (data && data.IsSuccess) {

                if (!MegiLogin.PasswordWarpper.isValidate(obj.Password)) {
                    $("html").unmask();
                    var tipMessage = HtmlLang.Write(LangModule.Common, "PasswordIsSimpleTips", "To ensure your data safety, HyperCurrent upgraded data security policy. Please reset your password according to our new policy. A link has been sent to your registration mailing address which will let you reset your password. If  you don't receive an email shortly, check your 'bulk email' or 'junk email' folders. To make sure you receive email from Megi in the future, add the  'hypercu.cn'  domain to your email safe list.  Thanks for your support. ");

                    $.mDialog.alert(tipMessage, function () {
                        MegiLogin.PasswordWarpper.sendResetLink(obj.Email);
                    },1);

                    return;
                }

                MegiLogin.tabswitch.setBrowserTabIndex(data.MBrowserTabIndex);

                var langId = data.MLocaleID;

                MegiLogin.tabswitch.setBrowserTabLang(langId);

                //如果是页面失效后，重新登陆的话，登陆成功则直接关闭页面
                if (data.MRelogin) {
                    //父页面置空
                    self.opener = null;
                    //关闭页面
                    self.close();
                }
                else if (data.MRedirectURL) {
                    //直接跳到跳转的url
                    var url = data.MRedirectURL.trimEnd('/');
                    mWindow.reload(url, true);

                    //window.location.href = data.MRedirectURL.trimEnd('/');
                }
                    //如果是邀请用户
                else if (data.MIsUserInvite) {
                    var url = $("#hidMyServer").val() + "/FW/FWHome/OrgSelect?MOrgID=" + data.MWhenILogIn + "&RedirectUrl=" + $("#hidGoServer").val().trimEnd('/') + "/";

                    mWindow.reload(url, true);
                    //window.location.href = $("#hidMyServer").val() + "/FW/FWHome/OrgSelect?MOrgID=" + data.MWhenILogIn + "&RedirectUrl=" + url;
                }
                else {
                    if (data.MWhenILogIn == "" || data.MWhenILogIn == "1") {
                        var url = $("#hidMyServer").val();
                        mWindow.reload(url, true);
                    }
                    else {
                        var url = $("#hidGoServer").val();
                        mWindow.reload(url, true);
                    }
                }
                //清除原来的cookie
                $.removeCookie(emailCookieName)
                //登陆成功，都保存用户输入的用户名到cookie里面,1年有效期
                $.cookie(emailCookieName, email, 365);
            } else {
                if (data.MLoginErrorCount > MegiLogin.AllowLoginErrorAccount) {
                    //显示验证码
                    $("#divCode").show();
                    //刷验证码
                    MegiLogin.refreshValidateCode();
                }
                MegiLogin.loginError(msg);
            }

        },
        function (msg) {
            //刷验证码
            MegiLogin.refreshValidateCode();
            MegiLogin.loginError(msg);
        }, true);

    },
    loginError: function (msg) {
        msg = msg ? msg : {};

        //没有信息给个默认值
        if (!msg.Message) {
            var errorMsg = HtmlLang.Write(LangModule.Login, "EmailOrPasswordError", "Email or password error!");
            msg.Message = errorMsg;
        }

        $("#txtEmail").addClass("error");
        $("#txtPwd").addClass("error");
        $(".error-msg").html(errorMsg);
        if ($("#txtEmail").val()) {
            $("#txtEmail").focus();
        } else {
            $("#txtPwd").focus();
        }
        $("html").unmask();
    },
    refreshValidateCode: function () {
        //IE特殊处理

        $("#validateCodeImage").remove();
        var html = '<img id="validateCodeImage" src="/Account/CreateValidateCodeImage?' + Math.random() + '" style="cursor: pointer;"  alt="验证码" width="70" height="30"></img>';
        $("#btnChangeImage").before(html);

        //刷的时候将验证码是否正确标示改为false
        $("#isCorrent").attr("code", "false");
        $("#isCorrent").removeAttr("class");
        $("#tbxCode").val("");
    }
}
$(document).ready(function () {
    MegiLogin.init();
});

