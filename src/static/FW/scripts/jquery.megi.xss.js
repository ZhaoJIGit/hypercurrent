/*
这个js专门用来处理脚本攻击的事件
felson 2016.7.20
*/
(function () {

    var mXss = (function () {
        //
        var mXss = function () { };
        //
        return mXss;
    })();

    //默认检测xxs需要编码的整个表达式
    var xssEncodeReg = /document\s*\.|window\s*\.|javascript|<(\s*|\/)script\s+?>|<(\s*|\/)img\s+?>|<(\s*|\/)applet\s+?>|<(\s*|\/)embed\s+?>|<(\s*|\/)a\s+?>|<(\s*|\/)meta\s+?>|<(\s*|\/)xml\s+?>|<(\s*|\/)html\s+?>|<(\s*|\/)body\s+?>|<(\s*|\/)head\s+?>|<(\s*|\/)div\s+?>|<(\s*|\/)ul\s+?>|<(\s*|\/)li\s+?>|<(\s*|\/)style\s+?>|<(\s*|\/)base\s+?>|<(\s*|\/)link\s+?>|<(\s*|\/)iframe\s+?>|<(\s*|\/)frameset\s+?>|<(\s*|\/)bgsound\s+?>|<(\s*|\/)object\s+?>|\s+style\s*=|\s+href\s*=|\s+rel\s*=|\s+type\s*=|\s+src\s*=|\s+backgroud\s*=|\s+alert\s*\([^()]*\)|\s+url\s*\([^()]*\)|\s+eval\s*\([^()]*\)|\s+escape\s*\([^()]*\)|\s+unescape\s*\([^()]*\)|\s+execscript\s*\([^()]*\)|\s+msgbox\s*\([^()]*\)|\s+confirm\s*\([^()]*\)|prompt\s*\([^()]*\)|data\s*:/gim;;
    //取消无效的字符串 回车换行等
    var xssInvalidCharReg = "\\t|\\n|\\r|\\f";
    //第一次判断里面是否有特殊字符
    var xssScriptCharReg = "\\\\|&|#|%|<|>|:|\\(|\\)";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.xssFilter = function (s) {
        //如果是空字符串
        if (!s || typeof s != "string") {
            return s;
        }
        else {
            //
            var s0 = _.clone(s);
            //先判断是否有一些可能攻击的字符
            if (mXss.hasXssEscapeChar(s0)) {
                //把里面的换行 回车那些给去掉
                s0 = mXss.replaceInvalidChar(s0);
                //然后进行全部解码
                s0 = mXss.XssDecode(s0);
            }
            //判断里面是否有攻击脚本
            if (mXss.hasXssScript(s0)) {
                //如果有的话，需要把里面的脚本全部替换
                return mXss.xssFilter(mXss.filterXssScript(s0));
            }
        }
        //
        return s;
    }

    /// <summary>
    /// 是否有可能攻击的字符判断里面是否有  # & \ % > < :这些可能的特殊字符
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.hasXssEscapeChar = function (s) {
        //直接校验
        return new RegExp(xssScriptCharReg, "i").test(s);
    }

    /// <summary>
    /// 去掉里面的制表符 换行 回车 Escape 竖向制表符
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.replaceInvalidChar = function (s) {
        //去掉无效字符
        var reg = new RegExp(xssInvalidCharReg, "i");
        //
        return s.replace(reg, "");
    }

    /// <summary>
    /// 是否有xss脚本
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.hasXssScript = function (s) {
        //是否有攻击性脚本
        return xssEncodeReg.test(s);
    }

    /// <summary>
    /// 总的入口 把字符串进行解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.XssDecode = function (s) {
        //如果是空字符串就直接返回好了
        return !!s ? mXss.urlDecode(mXss.jsUnicode(mXss.jsAsciiToString(mXss.asciiToString(s)))) : s;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    mXss.filterXssScript = function (value) {
        //
        return value.replace(xssEncodeReg, "");
    }

    /// <summary>
    /// jsunicode Ascii转 正常字符
    /// 将 \u0061\u006c\u0065\u0072\u0074(1) 转化为正常字符床alert(1)  \152\141\166 \x3c
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.jsUnicode = function (s) {
        //
        var reg = /\\[uU]([0-9a-f]{4})/gi;
        //分组替换
        s = s.replace(reg, function (m, p1, p2, p3) {
            return String.fromCharCode(parseInt(p1, 16));
        });
        //返回
        return s;
    }


    /// <summary>
    /// 将url编码后进行解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.urlDecode = function (s) {
        //
        var reg = /%([0-9a-f]{1,4})/ig;
        //分组替换
        s = s.replace(reg, function (m, p1, p2, p3) {
            return String.fromCharCode(parseInt(p1, 16));
        });
        //返回
        return s;
    }
    /// <summary>
    /// 将js编码过的十进制和十六进制进行解码
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.jsAsciiToString = function (s) {
        //
        return mXss.jsAsciiToString10(mXss.jsAsciiToString16(s));
    }

    /// <summary>
    /// 将js 16进制进行解码 &#x6A;&#x61;&#x76;&#x61;&#x73;&#x63;&#x72;&#x69;&#x70;&#x74; javascript
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.jsAsciiToString16 = function (s) {
        //
        var reg = /\&#[xX]([0-9a-f]{1,4});/ig;
        //分组替换
        s = s.replace(reg, function (m, p1, p2, p3) {
            return String.fromCharCode(parseInt(p1, 16));
        });
        //返回
        return s;
    }

    /// <summary>
    /// 将js 10进制进行解码 &#97&#108&#101&#114&#116&#40&#34&#72&#101&#108&#108&#111&#32&#119&#111&#114&#108&#100&#34&#41&#59 javascript
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.jsAsciiToString10 = function (s) {
        //
        var reg = /&#([0-9]{1,4})/ig;
        //分组替换
        s = s.replace(reg, function (m, p1, p2, p3) {
            return String.fromCharCode(parseInt(p1, 10));
        });
        //返回
        return s;
    }


    /// <summary>
    /// Ascii转 正常字符
    ///
    mXss.asciiToString = function (s) {
        //进过16进制和8进制解码
        return mXss.asciiToString16(mXss.asciiToString8(s));
    }


    /// <summary>
    /// Ascii转 正常字符
    /// 将 \x3c\x69\x6d\x67\x20\x73\x72\x63\x3d\x22\x78\x22\x20\x6f\x6e\x65\x72\x72\x6f\x72\x3d\x22\x61\x6c\x65\x72\x74\x28\x31\x29\x22\x3e
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.asciiToString8 = function (s) {
        //
        var reg = /\\([0-7]{1,4})/ig;
        //分组替换
        s = s.replace(reg, function (m, p1, p2, p3) {
            return String.fromCharCode(parseInt(p1, 8));
        });
        //返回
        return s;
    }

    /// <summary>
    /// Ascii转 正常字符
    /// 将 \x3c\x69\x6d\x67\x20\x73\x72\x63\x3d\x22\x78\x22\x20\x6f\x6e\x65\x72\x72\x6f\x72\x3d\x22\x61\x6c\x65\x72\x74\x28\x31\x29\x22\x3e
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    mXss.asciiToString16 = function (s) {
        //
        var reg = /\\[xX]([0-9a-f]{1,4})/ig;
        //分组替换
        s = s.replace(reg, function (m, p1, p2, p3) {
            return String.fromCharCode(parseInt(p1, 16));
        });
        //返回
        return s;
    }


    //
    window.mXss = mXss;
})()