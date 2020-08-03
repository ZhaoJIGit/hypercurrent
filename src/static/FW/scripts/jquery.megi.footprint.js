window.FootPrintTable = window.FootPrintTable || {};

(function () {
    "use strict";
    var mFootprint = (function () {

        var mFootprint = function () {

            var that = this;

            var user = {};
            var traits = {};
            var initialized = false;

            //识别入口
            this.identify = function (options) {

                options = options || {};
                user = user || {};
                traits = traits || {};

                user.email = options.email || user.email;
                user.name = options.name || user.name;

                traits.organisation = options.organisation || traits.organisation;
                traits.org_edition = options.org_edition || traits.org_edition;
                traits.org_expiry = options.org_expiry || traits.org_expiry;
                traits.org_trial = options.org_trial || traits.org_trial;

                that.segmentIdentify();
            };

            //track总的入口
            this.track = function (options) {
                //segment入口
                that.segmentTrack(options);
            };

            //page的总入口
            this.page = function (options) {
                //segment入口
                that.segmentPage(options);
            };


            //初始化segment.io
            this.segmentIdentify = function () {
                if (!user.email)
                    return false;
                analytics.identify(user.email, user);
                return true;
            };

            //传输track数据到segment.io
            this.segmentTrack = function (options) {
                try {
                    if (!window.analytics) return;
                    analytics.track(options.event, _.extend({ name: user.name, email: user.email }, options.properties, traits));
                } catch (e) {
                    window.console && window.console.log(e);
                }

            };

            //传输page数据到segement.io
            this.segmentPage = function (options) {
                try {
                    if (!window.analytics) return;
                    analytics.page(options.category, options.name, _.extend({ name: user.name }, options.properties, traits));
                } catch (e) {
                    window.console && window.console.log(e);
                }
            };

            //初始化
            this.initialize = function () {

                if (initialized) return;
                //初始化用户
                var result = that.initializeIdentify();

                if (result === true) {
                    //标识已经初始化了
                    initialized = true;
                }
            };

            //初始化user
            this.initializeIdentify = function () {

                user = mFootprint.user || {};
                traits = mFootprint.traits || {};

                user.email = top.$("#hideUserEamil").val();
                user.name = top.$("#lblUserName").attr("title");

                traits.organisation = top.$("#hideOrgName").length > 0 ? top.$("#hideOrgName").val() : "";
                traits.org_edition = top.$("#hideOrgVersionID").length > 0 ? top.$("#hideOrgVersionID").val() === "0" ? "Standard" : "SmartLedger" : "";
                traits.org_expiry = top.$("#hideOrgExpireDate").length > 0 ? top.$("#hideOrgExpireDate").val() : "";
                traits.org_trial = top.$("a#aOrgList").length > 0 ? top.$("a#aOrgList").attr("ispaid") === "1" ? "YES" : "NO" : "NO";

                return user.email ? true : false;
            };
        };

        return mFootprint;
    })();

    //选项解析
    mFootprint.parseOptions = function (param, setting) {
        var options = {
            event: setting.event,
            category: setting.category,
            name: setting.event,
            properties: _.clone(setting.properties)
        };

        var extProperties = {};

        if (setting.extProperties && setting.extProperties.length > 0) {
            for (var i = 0; i < setting.extProperties.length; i++) {
                var prop = setting.extProperties[i];
                var name = typeof prop === "object" ? prop.name : prop;
                var alias = typeof prop === "object" ? prop.alias : undefined;
                alias = alias || name;
                var formatter = typeof prop === "object" ? prop.formatter : null;
                var value = mFootprint.getObjectValue(param, name);
                extProperties[alias] = formatter ? formatter(value) : value;
            }
        }

        var keys = Object.keys(options.properties);
        var extKeys = Object.keys(extProperties);

        for (i = 0; i < keys.length && extKeys.length > 0; i++) {

            var key = keys[i];
            value = options.properties[key];

            for (var j = 0; j < extKeys.length; j++) {

                options.properties[key] = options.properties[key].replace("{" + extKeys[j] + "}", extProperties[extKeys[j]]);
                options.event = options.event.replace("{" + extKeys[j] + "}", extProperties[extKeys[j]]);
                options.name = options.name.replace("{" + extKeys[j] + "}", extProperties[extKeys[j]]);
            }
        }

        options.properties = _.extend(options.properties, _.clone(extProperties));

        return options;
    };

    //获取对象的值
    mFootprint.getObjectValue = function (obj, name) {

        if (!name) return obj;

        if (name.indexOf('.') > 0) {

            var field = name.substring(0, name.indexOf('.'));
            var subField = name.substring(name.indexOf('.') + 1);

            return mFootprint.getObjectValue(obj[field], subField);
        }

        return obj[name];
    };

    //解析url字符串
    mFootprint.parseQueryString = function (str) {
        var p = {};
        var strs = str.split("&");
        for (var i = 0; i < strs.length; i++) {
            p[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
        }
        return p;
    };

    //判断是否需要跟踪
    mFootprint.needTrack = function (track, param, data) {

        if (track === true) return true;

        if (typeof track === "function" && track(param, data) === true) return true;

        return false;
    };

    //获取单例
    mFootprint.getInstance = function () {
        //如果顶部没有初始化过，则需要初始化
        if (top.mFootprint.instance === null) {
            top.mFootprint.instance = new mFootprint();
            top.mFootprint.instance.initialize();
        }
        return top.mFootprint.instance;
    };

    //静态方法
    mFootprint.track = function (url, param, data, timeout) {

        try {
            if (!window.mFootprint.onTrack) return;

            var trackFunc = function (url) {

                if (!url) return;

                url = url.replace(/http:/gi, "").replace(/iframe:/gi, "").replace("//", "");
                url = url.substring(url.indexOf('/'));
                var urlArry = url.split('?');
                url = urlArry[0];

                //如果有四个斜杠，最后一个斜杠表示参数/IV/Invoice/InvoiceEdit/adfadsfadsfadsfad
                var splits = url.split('/');
                if (splits.length === 5) {
                    param = param || {};
                    param.id = splits.pop();
                    url = splits.join('/');
                }

                var setting = window.FootPrintTable[url];

                if (setting) {

                    param = param || {};

                    if (urlArry.length > 1) {
                        param = _.extend(param, mFootprint.parseQueryString(urlArry[1]));
                    }

                    var options = mFootprint.parseOptions(param, setting);

                    if (!mFootprint.needTrack(setting.track, param, data)) return;

                    if (setting.identify && options.properties[setting.identify]) {
                        mFootprint.identify({ email: param[setting.identify], name: data.MUserName });
                    }
                    if (setting.type === "page") {
                        mFootprint.getInstance().page(options);
                    }
                    else if (setting.type === "track") {
                        mFootprint.getInstance().track(options);
                    }
                }
            };

            if (timeout === 0 || timeout === false)
                trackFunc(url);
            else {
                setTimeout(function () {
                    trackFunc(url);
                }, timeout || 100);
            }

        } catch (e) {
            window.console && window.console.log(e);
        }
    };

    //静态方法
    mFootprint.identify = function (options) {

        try {
            if (!window.mFootprint.onTrack) return;
            //如果顶部没有初始化过，则需要初始化
            return mFootprint.getInstance().identify(options);

        } catch (e) {
            window.console && window.console.log(e);
        }
    };

    //只有引入了这个js，才需要引入其他js
    mFootprint.load = function () {

        if (!window.mFootprint.onTrack) return;

        var scripts = [
            "/fw/footprint/jquery.megi.footprint.go.js",
            "/fw/footprint/jquery.megi.footprint.my.js",
            "/fw/footprint/jquery.megi.footprint.main.js",
            "/fw/footprint/jquery.megi.footprint.login.js",
            "/fw/scripts/jquery.megi.segment.js"
        ];
        var minJs = "/jquery.megi.min.js";
        var scriptSrc = $("script[src*='" + minJs + "']").attr("src");

        for (var i = 0; i < scripts.length; i++) {

            var n = document.createElement("script");
            n.type = "text/javascript";
            n.async = !0;
            n.src = scriptSrc.toLowerCase().replace("/fw/min" + minJs, scripts[i]);
            var a = document.getElementsByTagName("script")[0];
            a.parentNode.insertBefore(n, a);
        }
    };


    window.mFootprint = mFootprint;
    //单例
    window.mFootprint.instance = null;
    //开关
    window.mFootprint.onTrack = true;
})();

$(function () {
    try {
        top.window.mFootprint.load();
    } catch (e) {
        window.console && window.console.log(e);
    }
});
