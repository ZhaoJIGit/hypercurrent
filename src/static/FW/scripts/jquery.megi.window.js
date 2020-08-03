/*
这个类主要是处理window对象本身有的方法，
做函数截取
*/
(function (w) {
    var mWindow = (function () {
        //
        var mWindow = function () {

        };
        //
        return mWindow;
    })();
    //
    w.mWindow = mWindow;
    //
    $.extend(w.mWindow, {
        //点击打开窗口事件，比如下载excel，下载模板等
        open: function (href, alias) {
            window.mFilter.doFilter("track", [alias || href]);
            window.open(href);
        },
        //window重新加载
        reload: function (href, isNotValidateToken, isNotRemoveContent) {
            //既然页面要做更新，就需要把页面里面所有的input禁用，a禁用，button，禁用
            w.mWindow.offEvent(w);
            //检查下是否需要记录
            window.mFilter.doFilter("track", [(href || window.location.href), undefined, 0]);

            var tabswitch = new BrowserTabSwitch(),

                href = tabswitch.intercept(href);

            if (isNotValidateToken) {
                //然后再调用原先的reload事件
                if (href) {
                    //
                    if (window.frameElement && window.frameElement.src) {
                        //
                        if (!isNotRemoveContent) {
                            $("body", window.document).empty();
                        }
                        $(window.frameElement).addClass("m-page-loading");
                        //直接更行iframe的值
                        window.frameElement.src = href;

                    }
                    else {
                        window.location = href;
                    }
                }
                else {
                    //
                    if (!isNotRemoveContent) {
                        $("body", window.document).empty();
                    }
                    //如果是iframe的话则需要做一下处理
                    if (window.frameElement) {
                        //加上等待图标
                        $(window.frameElement).addClass("m-page-loading");
                        //针对IE需要做特殊处理，因为带有post提交的iframe刷新的时候，会弹出提醒，是否确定重复提交
                        if (mWindow.GetBrowser().indexOf("ie") >= 0) {
                            //找到对应的那个form
                            var form = $("form[id='f_" + $(window.frameElement).attr("id") + "']", $(window.frameElement).parent());
                            //如果有请求的数据
                            if (form.length > 0) {
                                //再次提交一下就ok啦
                                form.submit();
                                //
                                return;
                            }
                        }
                    }
                    //
                    window.location.reload(true);
                }
                return;
            }

            top.window.accessRequest(function () {

                //然后再调用原先的reload事件
                if (href) {
                    //
                    if (window.frameElement && window.frameElement.src) {
                        //
                        if (!isNotRemoveContent) {
                            $("body", window.document).empty();
                        }
                        //
                        $(window.frameElement).addClass("m-page-loading");
                        //直接更行iframe的值
                        window.frameElement.src = href;
                    }
                    else {
                        window.location = href;
                    }
                }
                else {
                    //
                    if (!isNotRemoveContent) {
                        $("body", window.document).empty();
                    }
                    //如果是iframe的话则需要做一下处理
                    if (window.frameElement) {
                        //加上等待图标
                        $(window.frameElement).addClass("m-page-loading");
                        //针对IE需要做特殊处理，因为带有post提交的iframe刷新的时候，会弹出提醒，是否确定重复提交
                        if (mWindow.GetBrowser().indexOf("ie") >= 0) {
                            //找到对应的那个form
                            var form = $("form[id='f_" + $(window.frameElement).attr("id") + "']", $(window.frameElement).parent());
                            //如果有请求的数据
                            if (form.length > 0) {
                                //再次提交一下就ok啦
                                form.submit();
                                //
                                return;
                            }
                        }
                    }
                    //
                    window.location.reload(true);
                }
            });
        },
        //取消widow连input和其他的事件
        offEvent: function (win) {
            //
            win = win || w;
            //
            $("input,a,div,span,li,textarea", win.document).off("click").off("keyup").off("mouseover").off("mouseleave").off("mousedown").off("mouseup").off("mouseout").off("keydown").off("dblclick").off("focus").off("blur");
        },
        //获取浏览器的版本信息
        GetBrowser: function () {
            var agent = navigator.userAgent.toLowerCase();
            var regStr_ie = /msie [\d.]+/gi;
            var regStr_ff = /firefox\/[\d.]+/gi
            var regStr_chrome = /chrome\/[\d.]+/gi;
            var regStr_saf = /safari\/[\d.]+/gi;
            var browserNV = "";
            //IE
            if (agent.indexOf("msie") > 0) {
                browserNV = agent.match(regStr_ie);
            }
            //firefox
            if (agent.indexOf("firefox") > 0) {
                browserNV = agent.match(regStr_ff);
            }
            //Chrome
            if (agent.indexOf("chrome") > 0) {
                browserNV = agent.match(regStr_chrome);
            }
            //Safari
            if (agent.indexOf("safari") > 0 && agent.indexOf("chrome") < 0) {
                browserNV = agent.match(regStr_saf);
            }
            browserNV = browserNV.toString();
            //other
            if ("" == browserNV) {
                browserNV = "Is not a standard browser";
            }
            //Here does not display "/"
            if (browserNV.indexOf('firefox') != -1 || browserNV.indexOf('chrome') != -1) {
                browserNV = browserNV.replace("/", "");
            }
            //Here does not display space
            if (browserNV.indexOf('msie') != -1) {
                //msie replace IE & trim space
                browserNV = browserNV.replace("msie", "ie").replace(/\s/g, "");
            }
            if (browserNV == "Is not a standard browser") {
                if (mWindow.IsIE()) {
                    browserNV = "ie11.0";
                }
            }
            //return eg:ie9.0 firefox34.0 chrome37.0
            return browserNV;
        },
        //判断一个浏览器是否是IE
        IsIE: function () { //IE
            if (!!window.ActiveXObject || "ActiveXObject" in window)
                return true;
            else
                return false;
        },
        getOrigin: function () {
            if (window["context"] == undefined) {
                if (!window.location.origin) {
                    window.location.origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '');
                }
                window["context"] = location.origin + "/V6.0";
            }

            return window.location.origin;
        },
        //取消选中的文本
        clearSelectedText: function () {
            if (document.selection) {
                document.selection.empty();
            }
            else if (window.getSelection) {
                window.getSelection().removeAllRanges();
            }
        }
    });
})(window)



/**
 ** 加法函数，用来得到精确的加法结果
 ** 说明：javascript的加法结果会有误差，在两个浮点数相加的时候会比较明显。这个函数返回较为精确的加法结果。
 ** 调用：accAdd(arg1,arg2)
 ** 返回值：arg1加上arg2的精确结果
 **/
function accAdd(arg1, arg2) {
    var r1, r2, m, c;
    try {
        r1 = arg1.toString().split(".")[1].length;
    }
    catch (e) {
        r1 = 0;
    }
    try {
        r2 = arg2.toString().split(".")[1].length;
    }
    catch (e) {
        r2 = 0;
    }
    c = Math.abs(r1 - r2);
    m = Math.pow(10, Math.max(r1, r2));
    if (c > 0) {
        var cm = Math.pow(10, c);
        if (r1 > r2) {
            arg1 = Number(arg1.toString().replace(".", ""));
            arg2 = Number(arg2.toString().replace(".", "")) * cm;
        } else {
            arg1 = Number(arg1.toString().replace(".", "")) * cm;
            arg2 = Number(arg2.toString().replace(".", ""));
        }
    } else {
        arg1 = Number(arg1.toString().replace(".", ""));
        arg2 = Number(arg2.toString().replace(".", ""));
    }
    return +((arg1 + arg2) / m);
}


//给Number类型增加一个add方法，调用起来更加方便。
Number.prototype.add = function (arg) {
    return accAdd(this, arg);
};

/**
 ** 减法函数，用来得到精确的减法结果
 ** 说明：javascript的减法结果会有误差，在两个浮点数相减的时候会比较明显。这个函数返回较为精确的减法结果。
 ** 调用：accSub(arg1,arg2)
 ** 返回值：arg1加上arg2的精确结果
 **/
function accSub(arg1, arg2) {
    var r1, r2, m, n;
    try {
        r1 = arg1.toString().split(".")[1].length;
    }
    catch (e) {
        r1 = 0;
    }
    try {
        r2 = arg2.toString().split(".")[1].length;
    }
    catch (e) {
        r2 = 0;
    }
    m = Math.pow(10, Math.max(r1, r2)); //last modify by deeka //动态控制精度长度
    n = (r1 >= r2) ? r1 : r2;
    return +(((arg1 * m - arg2 * m) / m).toFixed(n));
}

// 给Number类型增加一个mul方法，调用起来更加方便。
Number.prototype.sub = function (arg) {
    return accSub(this, arg);
};

/**
 ** 乘法函数，用来得到精确的乘法结果
 ** 说明：javascript的乘法结果会有误差，在两个浮点数相乘的时候会比较明显。这个函数返回较为精确的乘法结果。
 ** 调用：accMul(arg1,arg2)
 ** 返回值：arg1乘以 arg2的精确结果
 **/
function accMul(arg1, arg2) {
    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
    try {
        m += s1.split(".")[1].length;
    }
    catch (e) {
    }
    try {
        m += s2.split(".")[1].length;
    }
    catch (e) {
    }
    return +(Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m));
}

// 给Number类型增加一个mul方法，调用起来更加方便。
Number.prototype.mul = function (arg) {
    return accMul(this, arg);
};


/** 
 ** 除法函数，用来得到精确的除法结果
 ** 说明：javascript的除法结果会有误差，在两个浮点数相除的时候会比较明显。这个函数返回较为精确的除法结果。
 ** 调用：accDiv(arg1,arg2)
 ** 返回值：arg1除以arg2的精确结果
 **/
function accDiv(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2;
    try {
        t1 = arg1.toString().split(".")[1].length;
    }
    catch (e) {
    }
    try {
        t2 = arg2.toString().split(".")[1].length;
    }
    catch (e) {
    }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""));
        r2 = Number(arg2.toString().replace(".", ""));
        return +((r1 / r2) * pow(10, t2 - t1));
    }
}

//给Number类型增加一个div方法，调用起来更加方便。
Number.prototype.div = function (arg) {
    return accDiv(this, arg);
};

Date.prototype.format = function (format) {
    var date = new Date();
    var o = {
        "M+": this.getMonth() + 1,
        "d+": this.getDate(),
        "h+": this.getHours() % 12 == 0 ? 12 : this.getHours() % 12,
        "H+": this.getHours(),
        "t+": this.getHours() <= 12 ? LangKey.AM : LangKey.PM,
        "m+": this.getMinutes(),
        "s+": this.getSeconds(),
        "q+": Math.floor((this.getMonth() + 3) / 3),
        "S": this.getMilliseconds()
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
        (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
            RegExp.$1.length == 1 ? o[k] :
                ("00" + o[k]).substr(("" + o[k]).length));
    return format;
}

//将字符串转成对应的数字
String.prototype.toNullableNumber = function () {

    if (this === "" || this.trim() === "") return undefined;

    //目前最大支持10位吧，因为数据库字符串转decimal类型的方法最大支持10位
    if (isNaN(this)) return undefined;

    if (this.length > 20) {
        return undefined;
    }

    var num = +this;
    //转成字符串
    var str = new String(num);
    //去掉小数点和正负号
    str = str.replace('.', '').replace('+', '').replace('-');
    //数值最大位数20位
    if (str.length > 20) return undefined;


    return num;
}

String.prototype.format = function (args) {
    if (arguments.length > 0) {
        var result = this;
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (key == null) {
                    continue;
                }
                var reg = new RegExp("({" + key + "})", "g");
                result = result.replace(reg, args[key]);
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                var value = arguments[i];
                if (value == undefined || value == null) {
                    continue;
                }
                if (value.toString().indexOf("/Date(") > -1) {
                    value = $.mDate.format(value);
                }
                var reg = new RegExp("({[" + i + "]})", "g");
                result = result.replace(reg, value);
            }
        }
        return result;
    }
    else {
        return this;
    }
}

Array.prototype.distinct = function () {

    if (this.length < 2) return this;
    var ret = [];

    for (var i = 0; i < this.length; i++) {

        if (!ret.contains(this[i])) ret.push(this[i]);
    }

    return ret;
}


Array.prototype.select = function (field) {
    //
    var result = [];
    //
    for (var i = 0; i < this.length; i++) {
        //
        result.push(this[i][field]);
    }
    return result;
};

Array.prototype.contains = function (value) {
    //
    for (var i = 0; i < this.length; i++) {
        //
        if (this[i] === value) {
            return true;
        }
    }
    return false;
};

Array.prototype.sum = function (field, fixed) {
    //
    var result = 0;
    //
    for (var i = 0; i < this.length; i++) {
        //
        result = result.add(+(this[i][field] || 0));
    }
    return fixed != undefined ? +(result.toFixed(fixed)) : result;
};

Array.prototype.where = function (condition) {
    //
    var result = [];
    //
    for (var i = 0; i < this.length; i++) {
        //
        var x = this[i];
        //
        if (condition != null && condition != undefined && eval(condition) === true) {
            result.push(x);
        }
    }
    return result;
};

Array.prototype.OrderByAsc = function (func) {
    var m = {};
    for (var i = 0; i < this.length; i++) {
        for (var k = 0; k < this.length; k++) {
            if (func(this[i]) < func(this[k])) {
                m = this[k];
                this[k] = this[i];
                this[i] = m;
            }
        }
    }
    return this;
}

Array.prototype.OrderByDesc = function (func) {
    var m = {};
    for (var i = 0; i < this.length; i++) {
        for (var k = 0; k < this.length; k++) {
            if (func(this[i]) > func(this[k])) {
                m = this[k];
                this[k] = this[i];
                this[i] = m;
            }
        }
    }
    return this;
}

Array.prototype.removeItem = function (obj) {
    var m = [];
    for (var i = 0; i < this.length; i++) {
        //
        if (this[i] !== obj) {
            m.push(this[i]);
        }
    }
    return m;
}

Array.prototype.OrderByAsc = function (func) {
    var m = {};
    for (var i = 0; i < this.length; i++) {
        for (var k = 0; k < this.length; k++) {
            if (func(this[i]) < func(this[k])) {
                m = this[k];
                this[k] = this[i];
                this[i] = m;
            }
        }
    }
    return this;
}
Array.prototype.OrderByDesc = function (func) {
    var m = {};
    for (var i = 0; i < this.length; i++) {
        for (var k = 0; k < this.length; k++) {
            if (func(this[i]) > func(this[k])) {
                m = this[k];
                this[k] = this[i];
                this[i] = m;
            }
        }
    }
    return this;
}

//字符串类型的trimStart函数 默认是 取前后空格
String.prototype.trimStart = function (trimStr) {
    trimStr = trimStr ? trimStr : " ";
    var temp = this;
    while (true) {
        if (temp.substr(0, trimStr.length) != trimStr) {
            break;
        }
        temp = temp.substr(trimStr.length);
    }
    return temp.toString();
};

//字符串类型的trimEnd函数
String.prototype.trimEnd = function (trimStr) {
    trimStr = trimStr ? trimStr : " ";
    var temp = this;
    while (true) {
        if (temp.substr(temp.length - trimStr.length, trimStr.length) != trimStr) {
            break;
        }
        temp = temp.substr(0, temp.length - trimStr.length);
    }
    return temp.toString();
};
//字符串类型的trim函数
String.prototype.trim = function (trimStr) {
    trimStr = trimStr ? trimStr : " ";
    return this.trimStart(trimStr).trimEnd(trimStr).toString();
};
//字符串类型的trim函数
String.prototype.toUpperChar = function () {
    return this.substr(0, 1).toUpperCase() + this.substr(1);
};
//已某个开始
String.prototype.startWith = function (str) {
    var reg = new RegExp("^" + str);
    return reg.test(this);
}
//已某个结束
String.prototype.endWith = function (str) {
    var reg = new RegExp(str + "$");
    return reg.test(this);
}
//将数据库字段的里面含单引号的处理下
String.prototype.qoute = function () {
    if (!this || this.length == 0) {
        return "";
    }
    return this.replace(/&#39;/ig, "\'").replace(/&quot;/ig, '"').replace(/&amp;/ig, '&');
};

//模拟 
String.isNullOrWhitespace = function (str) {

    return str == undefined || str == null || str == '';
};


Number.prototype.toFixed = function (n) {

    if (n > 20 || n < 0) {
        throw new RangeError('toFixed() digits argument must be between 0 and 20');
    }

    //如果本身就是number类型的，并且数字小于0.xxx[n]5，则表示结果肯定是被四舍五入掉变成0了
    if (typeof (differ) === "number" && Math.abs(differ) < (+("0." + "00000000000000000000".substring(0, n) + "5"))) {
        //返回0;
        return "0." + "00000000000000000000".substring(0, n);
    }

    var number = this;
    if (isNaN(number) || number >= Math.pow(10, 21)) {
        return number.toString();
    }
    if (typeof (n) == 'undefined' || n == 0) {
        return (Math.round(number)).toString();
    }

    var result = number.toString();
    var arr = result.split('.');

    // 整数的情况
    if (arr.length < 2) {
        result += '.';
        for (var i = 0; i < n; i += 1) {
            result += '0';
        }
        return result;
    }

    var integer = arr[0];
    var decimal = arr[1];
    if (decimal.length == n) {
        return result;
    }
    if (decimal.length < n) {
        for (var i = 0; i < n - decimal.length; i += 1) {
            result += '0';
        }
        return result;
    }
    result = integer + '.' + decimal.substr(0, n);
    var last = decimal.substr(n, 1);

    // 四舍五入，转换为整数再处理，避免浮点数精度的损失
    if (parseInt(last, 10) >= 5) {
        var x = Math.pow(10, n);

        var convertValue = parseFloat(result) * x;

        var adjustmentDirection = convertValue >= 0 ? 1 : -1;

        result = (Math.round((convertValue)) + adjustmentDirection) / x;
        result = result.toFixed(n);
    }

    return result;
}
