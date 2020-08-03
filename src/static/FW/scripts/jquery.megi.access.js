
//请求验证是否过期的url;这里为了解决edge取缓存的问题 chenpan
var accessRequestUrl = "/FW/FWHome/CheckToken?r=" + Math.random();
//登陆请求
var loginRequestUrl = "/FW/FWHome/LoginBoxSignIn";
//MAccessTokenCookie的名字
var MAccessTokenCookie = "MAccessToken";
//MAccessTokenCookie的名字
var MUserEmailCookie = "MUserEmail";
//
var MLocaleID = "MLocaleID";
//cookie的域
var cookieDomain = ".megi.local";

//把登陆弹出放在最外层的Iframe上
window.showLoginDialog = function (type, callback) {
    //获取当前的email
    var emailInPage = top.$("#hideUserEamil").val();
    var email = emailInPage ? emailInPage : $.cookie(MUserEmailCookie);
    //获取当前的组织id
    var orgid = $("#aOrgList").attr("orgid") || "";
    //如果没有email
    //弹出登陆窗口
    top.$(".m-login-box-home").show();
    //
    new mLoginBox(email, orgid, type, callback).init();
}
//出事login 状态
window.handleLoginState = function (type, callback) {
    //只有等于0的时候才正常，其他的都需要登陆
    if (type === 0) {
        //调用回调函数
        $.isFunction(callback) && callback();
    }
    else if (type == 2) {
        //取消页面刷新的时候，直接到登陆页面
        window.goLoginState();
    }
    else if (type == 1 || type == 3) {
        //显示弹出框
        window.showLoginDialog(type, callback)
    }
    else if (type == 4) {
        //提醒用户，当前组织已经过期，需要重新进入my站点进行组织选择
        window.goMyState();
    }
    else if (type == 5) {
        //多语言切换了，需要提醒用户整个顶部需要重新刷新
        window.goTopRefresh();
    }
}
//把登陆弹出放在最外层的Iframe上
window.closeLoginDialog = function () {
    //弹出登陆窗口
    top.$(".m-login-box-home").hide();
}
//请求后台回话是否过期
window.accessRequest = function (callback, async) {
    //获取当前的email
    var emailInPage = top.$("#hideUserEamil").val();
    var email = emailInPage ? emailInPage : $.cookie(MUserEmailCookie);
    //获取当前的token
    var token = $.cookie(MAccessTokenCookie);
    //
    var localeID = new BrowserTabSwitch().getBrowserTabLang();

    //这里逻辑更改，优先使用sessionstorage里面的内容
    var localeID = localeID ? localeID : $.cookie(MLocaleID);
    //获取当前的组织id
    var orgid = $("#aOrgList").attr("orgid") || "";
    //异步请求是否超时
    $.mAjax.Post(accessRequestUrl, { _tmt: token, _eme: email, _omo: orgid, _lml: localeID }, function (data) {
        //根据data的值判断是否过期 data:0 合法 1过期 2未登陆 3 被迫下线
        var type = data.type;
        //只有等于0的时候才正常，其他的都需要登陆
        window.handleLoginState(type, callback);
    }, "", false, async);
}
//跳到登录页面
window.goLoginState = function () {
    //取消页面刷新的时候，弹出的确认框
    top.onbeforeunload = null;
    //登陆站点
    //top.location = top.LoginServer + "?RedirectUrl=" + top.location;
    top.location = top.LoginServer;
}
//跳到登录页面
window.goMyState = function () {
    //取消页面刷新的时候，弹出的确认框
    top.onbeforeunload = null;
    //
    $.mDialog.warning(HtmlLang.Write(LangModule.Common, "YourOrganisationIsExpired", "您的组织已经过期，请使用新的组织或者将此组织延期!"), function () {
        //登陆站点
        top.location = top.MyServer + "?RedirectUrl=" + top.location;
    });
}
//获取用户信息
window.getUserInfo = function () {
    //暂时这样处理吧
    return top.$(".m-user-content>p").text();
}
//
window.goTopRefresh = function () {
    //取消页面刷新的时候，弹出的确认框
    top.onbeforeunload = null;
    //
    $.mDialog.warning(HtmlLang.Write(LangModule.Common, "LanguageOfMegiIsChangedUnexpected", "美记系统的语言被意外切换了，您需要刷新整个系统!"), function () {
        //登陆站点
        top.mWindow.reload();
    });
}