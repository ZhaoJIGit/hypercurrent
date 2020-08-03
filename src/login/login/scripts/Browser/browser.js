if (mBrowser == undefined) {
    var mBrowser = {};
}

mBrowser.getBrowserInfo=function() {
    var agent = navigator.userAgent.toLowerCase();

    var regStr_ie = /msie [\d.]+;/gi;
    var regStr_ff = /firefox\/[\d.]+/gi
    var regStr_chrome = /chrome\/[\d.]+/gi;
    var regStr_saf = /safari\/[\d.]+/gi;
    //IE
    if (agent.indexOf("msie") > 0) {
        return agent.match(regStr_ie);
    }

    //firefox
    if (agent.indexOf("firefox") > 0) {
        return agent.match(regStr_ff);
    }

    //Chrome
    if (agent.indexOf("chrome") > 0) {
        return agent.match(regStr_chrome);
    }

    //Safari
    if (agent.indexOf("safari") > 0 && agent.indexOf("chrome") < 0) {
        return agent.match(regStr_saf);
    }
}

var mUserAgentInfo = mBrowser.getBrowserInfo();
mBrowser.info = {
    version: (mUserAgentInfo + "").replace(/[^0-9.]/ig, ""),
    safari: /webkit/.test(mUserAgentInfo),
    opera: /opera/.test(mUserAgentInfo),
    msie: /msie/.test(mUserAgentInfo),
    mozilla: /mozilla/.test(mUserAgentInfo),
    chrome: /chrome/.test(mUserAgentInfo)
};