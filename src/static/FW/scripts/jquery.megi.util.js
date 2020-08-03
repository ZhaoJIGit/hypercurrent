
/*

*/
(function (w) {

    var mUtil = (function () {
        //
        var mUtil = function () {

        }
        //
        return mUtil;
    })();
    //
    w.mUtil = mUtil;
    /*
        lang表示对应的多语言ID 0x7804 0x0009 等等
        url 表示切换语言的时候需要调用后台内容
        type 站点类型 是否在登录页面，如果在登录页面的话，只提醒用户，系统正在切换，如果不是的话，需要提熊用户数据会被重新覆盖，并且恢复用户打开的tab页
    */
    mUtil.ChangeLang = function (lang, type, url , uncheckToken) {
        //默认的url
        url = url || "/Framework/ChangeLang";
        //
        var changLangFunc = function () {
            //提交请求到后台修改cookie内部的值以及用户多语言ID

            var tabSwitch = new BrowserTabSwitch();

            $.ajax({
                type: "POST",
                url: url,
                data: { langId: lang },
                success: function (msg) {
                    //
                    (type == 0 || type == 3) && $("#loginBox").unmask();
                    

                    var olderUrl = top.window.location.href;

                    //需要去掉#号
                    olderUrl = olderUrl.replace(eval("/#/g"), "");

                    var olderLangId = tabSwitch.getBrowserTabLang();
                    tabSwitch.setBrowserTabLang(lang);

                    var newUrl = olderUrl.replace(eval("/" + olderLangId + "/g"), lang);

                    window.mFilter.doFilter('track', ['ChangeLanguage', { lang: lang }]);
                    if (uncheckToken) {
                        window.mWindow.reload(newUrl, true);
                    } else {
                        msg && window.mWindow.reload(newUrl);
                    }

                   
                }
            });
        }
        //如果是login站点 和 main站点，则直接切换
        if (type == 0 || type == 3) {
            //
            $("#loginBox").mask(HtmlLang.Write(LangModule.Common, "waitChanglang", "switching..."));
            //
            changLangFunc();
        }
        else {
            //
            var title = HtmlLang.Write(LangModule.Common, "AreYouSureToChangeLang", "切换语言后页面将会刷新，请确认所有数据已保存，您是否确认要切换语言?")
            //
            window.onbeforeunload = null;
            //
            $.mDialog.confirm(title, function () {
                //
                changLangFunc();
            });
        }
    };

    mUtil.GetOrgUrl = function (progress) {

        switch (progress)
        {
            case 1:
                return "/BD/Setup/OrgSetting";
            case 2:
                return "/BD/Setup/FinancialSetting";
            case 3:
                return "/BD/Setup/TaxRateSetting";
            case 4:
                return "/BD/Setup/Finish";
            case 5:
                return "/";
            case 11:
            case 12:
            case 13:
            case 14:
            case 15:
                return "/";
            default:
                return "/BD/Setup/OrgSetting";
        }
    }
})(window)


window.mIV = $.mIV = {};

$.mIV.getTitle = function (titlePre, ref) {
    if (ref == null || ref == 'null' || ref == "") {
        return titlePre;
    }
    return titlePre + ":" + ref;
}
