BrowserTabSwitch = (function () {
    //构造函数
    function BrowserTabSwitch(options) {
        this.urlKey = "bti";
        this.currentLangKey = "lang";
        this.sessionStrorageKey = "browser-tab-index";
        this.sessionStrorageLangKey = "browser-tab-lang";
    };
    //浏览器页签切换事件，兼容IE10一下浏览器
    BrowserTabSwitch.prototype.bindSwitchEvent = function (callback) {

        $(window.self).off("focus.tabswitch").on("focus.tabswitch", function () {
            top.accessRequest(function () {

            }, true);
        });

        //top.window.addEventListener("focus", function () {
        //    top.accessRequest(function () {

        //    }, false);
        //}, true);

       
    };
    BrowserTabSwitch.prototype.visibilityChangeEvent = function () {
        var hidden = "hidden";

        // h5标准浏览器
        if (hidden in document)
            document.addEventListener("visibilitychange", onchange);
        else if ((hidden = "mozHidden") in document)
            document.addEventListener("mozvisibilitychange", onchange);
        else if ((hidden = "webkitHidden") in document)
            document.addEventListener("webkitvisibilitychange", onchange);
        else if ((hidden = "msHidden") in document)
            document.addEventListener("msvisibilitychange", onchange);

        function onchange(evt) {
            var v = "visible", h = "hidden",
                evtMap = {
                    focus: v, focusin: v, pageshow: v, blur: h, focusout: h, pagehide: h
                };

            evt = evt || window.event;
            var isVisible;

            if (evt.type in evtMap) {
                isVisible = evtMap[evt.type];
            }
            else {
                isVisible = this[hidden] ? "hidden" : "visible";
            }

            //如果为显示，校验一次登录状态
            if (isVisible) {
                top.accessRequest(function () {

                }, false);
            }

        }

        if (document[hidden] !== undefined) {
            onchange({ type: document[hidden] ? "blur" : "focus" });
        }
           
    };
    //初始化
	BrowserTabSwitch.prototype.initSessionStorage = function () {

	    var _this = this;

	    var browserTabIndex = Megi.getQueryString(_this.urlKey);
	    var browserTabIndexOlde = _this.getBrowserTabIndex();
	    var browserLang = Megi.getQueryString(_this.currentLangKey);

	    //只有原来没有，并且url中有参数，才更新，其他都在登录站点更新
	    if (browserTabIndex) {
	        _this.setBrowserTabIndex(browserTabIndex);
	    }

	    if (browserLang) {
	        _this.setBrowserTabLang(browserLang);
	    }
	};
    BrowserTabSwitch.prototype.intercept = function (url) {
        var _this = this;

        if (!url || (url.indexOf(_this.urlKey) >= 0 && url.indexOf(_this.currentLangKey) > 0)) {
            return url;
        }


        //表示原来已经有参数
        if (url.split("?").length > 1) {
            url = url + "&" + _this.urlKey + "=" + _this.getBrowserTabIndex() + "&" + _this.currentLangKey + "=" + _this.getBrowserTabLang();
        } else {
            url = url + "?" + _this.urlKey + "=" + _this.getBrowserTabIndex() + "&" + _this.currentLangKey + "=" + _this.getBrowserTabLang();
        }

        return url;
    }

    BrowserTabSwitch.prototype.getBrowserTabIndex = function () {

        var _this = this;

        return window.sessionStorage.getItem(_this.sessionStrorageKey);
    };

    BrowserTabSwitch.prototype.setBrowserTabIndex = function (val) {
        var _this = this;

        window.sessionStorage.setItem(_this.sessionStrorageKey, val);
    };

    BrowserTabSwitch.prototype.setBrowserTabLang = function (val) {
        var _this = this;

        window.sessionStorage.setItem(_this.sessionStrorageLangKey, val);
    };

    BrowserTabSwitch.prototype.getBrowserTabLang = function () {
        var _this = this;

        return window.sessionStorage.getItem(_this.sessionStrorageLangKey);
    };

    //设置组织id信息
    BrowserTabSwitch.prototype.setBrowserTabOrgId = function (val) {
        top.$("#aOrgList").attr("orgid", val);
    };

    return BrowserTabSwitch;
}());

