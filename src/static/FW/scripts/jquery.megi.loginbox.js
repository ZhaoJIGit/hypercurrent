//弹出登陆框
(function () {
    //
    var mLoginBox = (function () {
        //
        var _loginTitle = ".login-title";
        //
        var mLoginBox = function (email, orgid, type, callback) {
            //
            var that = this;
            //登陆完成
            this.loginComplete = function (msg) {
                //取消遮罩
                $(".m-login-box").unmask();
                //返回结果
                if (msg.data === 1) {
                    //隐藏登陆框
                    top.closeLoginDialog();

                    //增加浏览器页签标识
                    if (msg.bti) {

                        var tabSwitch = new BrowserTabSwitch();

                        //设置参数
                        var topOlderBti = tabSwitch.getBrowserTabIndex();

                        tabSwitch.setBrowserTabIndex(msg.bti);
                        
                        var olderUrl = top.window.location.href;
                        var newUrl = olderUrl.replace(eval("/" + topOlderBti + "/g"), msg.bti);

                        if (msg.lang) {
                            var olderLangId = tabSwitch.getBrowserTabLang();
                            tabSwitch.setBrowserTabLang(msg.lang);

                            newUrl = newUrl.replace(eval("/" + olderLangId + "/g"), msg.lang);
                        }
                        //替换原来的网址

                        if (!top.window.history.pushState) {
                            top.location.href = newUrl;
                        } else {
                            top.window.history.pushState("", "", newUrl);
                        }
                    }


                    if (that.isRefrshPage(msg) && msg.setupProgress && msg.setupUrl) {
                        top.window.location.href = msg.setupUrl;
                    }

                    //去掉密码
                    $(".password-text,.password-password").val("");
                    //依次调用回调函数
                    for (var i = 0; i < top.loginCallback.length ; i++) {
                        var func = top.loginCallback[i];
                        //调用回调函数
                        $.isFunction(func) && func();
                    }
                    //清空top的loginCallBack
                    top.loginCallback = [];

                }
                else {
                    //显示错误信息
                    $("#errorMessage").show();
                    //显示错误
                    $(".password-text,.password-password").addClass("m-login-error-border");
                    //清空密码框
                    $("#password").val("");
                }
                //取消遮罩
            };
            this.GetSetupPageFrame = function () {
                var regFrame = null;
                var setupUrlPrefix = "/BD/Setup";

                var tabs = $(".m-tab-content");

                if (tabs.length == 0) {
                    return regFrame;
                }

                for (var i = 0 ; i < tabs.length; i++) {
                    var dataUrl = $(tabs[i]).attr("data-url");
                    if (!!dataUrl) {
                        //
                        dataUrl = dataUrl.toLowerCase();
                        //向导只用一个页签显示
                        if (dataUrl.indexOf(setupUrlPrefix.toLocaleLowerCase()) > -1) {
                            //
                            regFrame = $(tabs[i]).find("iframe");
                            break;
                        }
                    }
                }


                return $(regFrame)[0];
            },
            //是否刷新当前页面
            this.isRefrshPage = function (msg) {
                var _this = this;

                //向导的iframe
                var regFrame = _this.GetSetupPageFrame();

                if (!regFrame) {
                    return false;
                }

                //如果是在向导页面，获取向导的组织ID，如果有组织id就刷新向导

                var orgid = top.$("#aOrgList").attr("orgid");

                //如果组织存在或者组织是被删除了，在my站点都需要重新刷新
                if (orgid || msg.isDeletedOrgID) {
                    return true;
                } else {
                    return false;
                }
            },
                //设置密码输入框的边框
            this.setPasswordClass = function (password) {
                //
                if (!password || password.length < 8 || password.length > 28) {
                    $(".password-text,.password-password").addClass("m-login-error-border");
                    //返回失败
                    return false;
                }
                else {
                    $(".password-text,.password-password").removeClass("m-login-error-border");
            }
                //
                return true;
            };
                //登陆事件
            this.login = function () {
                //获取参数
                var password = $("#password").val();
                //检测password是否正确
                if (that.setPasswordClass(password)) {

                    var tabSwitch = new BrowserTabSwitch();

                    //登陆框遮罩
                    $(".m-login-box").mask(HtmlLang.Write(LangModule.Common, "Login", "Login..."));
                    //orgid
                    var orgId = top.$("#aOrgList").attr("orgid") || "";

                    var currentTabLang = tabSwitch.getBrowserTabLang();
                    
                    //可能存在用户清空cookie的情况，这个时候sessionstorage和cookie里面的内容都应该是没有的,这个时候就直接去url里面的
                    var topUrl = top.window.location.href;
                    if (!currentTabLang) {
                        currentTabLang = Megi.getQueryString("lang");
                    }

                    //这里逻辑更改，优先使用sessionstorage里面的内容
                    var localeID = currentTabLang ? currentTabLang : $.cookie(MLocaleID);
                    
                    

                    


                    //var localeID =  $.cookie(MLocaleID);
                    //提交
                    $.mAjax.Post(loginRequestUrl, { _pmp: password, _eme: email, _omo: orgid, _lml: localeID }, that.loginComplete);
            }
            };
                //
            this.init = function () {
                //将回调函数加入到数组
                top.loginCallback.push(callback);
                //
                var userName = top.getUserInfo();
                //
                var title = "";
                //根据type展示原因
                title = (type === 3) ? (userName + ", " + HtmlLang.Write(LangModule.Common, "forcetorelogin", "your account Logined in other device, please login again!"))
                    : (userName + ", " + HtmlLang.Write(LangModule.Common, "sessionexpired", "session expired, please login again!"));
                //显示提醒
                top.$(".m-login-message").text(title);
                //显示错误信息
                $("#errorMessage").hide();
                //首先确认位置
                var t = (($(window).height() / 2) - ($(".m-login-box").outerHeight() / 2));
                var l = (($(window).width() / 2) - ($(".m-login-box").outerWidth() / 2));
                //调整位置
                $(".m-login-box").css({
                        left: l,
                        top: t
                })
                //先清除token对应的cookie值
                $.removeCookie(MAccessTokenCookie, { domain: cookieDomain });
                //密码输入框的特殊处理
                window.focusSwitch(".password-text", ".password-password");
                //注册登陆事件
                $("#login_popup_ok").off("click").on("click", that.login);
                //退录事件
                $("#login_popup_cancel").off("click").on("click", function () {
                    //取消页面刷新的时候，直接到登陆页面
                    window.goLoginState();
                });
                //回车提交
                $("#login_popup_ok,#password").off("keyup").on("keyup", function (event) {
                    //如果是回车
                    if (event.keyCode == 13) {
                        //登陆
                        that.login();
                }
                });
            }
        }
        return mLoginBox;
    })();
    //
    window.mLoginBox = mLoginBox;
})()