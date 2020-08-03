//这个js专门处理文本，解码，编码等等
(function () {

    var mText = (function () {
        //
        var mText = function () { };
        //
        return mText;
    })();
    //
    window.mText = mText;

    //html解码
    mText.htmlDecode = _.htmlDecode;
    //
    mText.htmlEncode = _.htmlEncode;
    //
    mText.urlDecode = _.urlDecode;
    //
    mText.urlEncode = _.urlEncode;


    /*
        对象的编码
    */
    //把一个对象里面的所有属性进行encode一次
    mText.encodeObject = function (obj, encodeFunc) {
        //
        if (arguments.length == 1 || !encodeFunc || !_.isFunction(encodeFunc)) {
            //
            encodeFunc = _.encode;
        }
        //对于不是object对象，或者无效的值，直接返回
        if (!obj && typeof obj !== "object") {
            return obj;
        }
        //获取所有的key
        var keys = Object.keys(obj);
        //遍历所有的key
        for (var i = 0; i < keys.length ; i++) {
            //对于字符串类型的，则编码一次
            obj[keys[i]] = mText.encode(obj[keys[i]], encodeFunc);
        }
        //返回
        return obj;
    }
    //对一个数据进行编码
    mText.encodeArray = function (arr, encodeFunc) {
        //
        if (arguments.length == 1 || !encodeFunc || !_.isFunction(encodeFunc)) {
            //
            encodeFunc = mText.encode;
        }
        //对于不是array对象，或者无效的值，直接返回
        if (!arr && _.isArray(arr)) {
            //必须是有效值
            for (var i = 0; i < arr.length ; i++) {
                //直接对数组内容进行编码
                arr[i] = mText.encode(arr[i], encodeFunc);
            }
        }
        //
        return arr;
    }
    //把underscore里面的函数都转到mText里面来 encodeFunc可以为空
    mText.encode = function (obj, encodeFunc) {
        //
        if (!obj) {
            return obj;
        }
        //如果是数组
        if (_.isArray(obj)) {
            //
            return mText.encodeArray(obj, encodeFunc);
        }
        //如果是字符串
        if (typeof obj === "string") {
            //
            if (arguments.length == 1 || !encodeFunc || !_.isFunction(encodeFunc)) {
                //直接编码返回
                return _.encode(obj);
            }
            return encodeFunc(obj);
        }
        //如果是object
        if (typeof obj === "object") {
            //
            return mText.encodeObject(obj, encodeFunc);
        }
        //其他的就直接返回
        return obj;
    };


    /*
        对象的解码函数
    */
    //把一个对象里面的所有属性进行encode一次
    mText.decodeObject = function (obj, decodeFunc) {
        //对于不是object对象，或者无效的值，直接返回
        if (!obj && typeof obj !== "object") {
            return obj;
        }
        //
        if (arguments.length == 1 || !decodeFunc || !_.isFunction(decodeFunc)) {
            //
            decodeFunc = _.decode;
        }
        //获取所有的key
        var keys = Object.keys(obj);
        //遍历所有的key
        for (var i = 0; i < keys.length ; i++) {
            //对于字符串类型的，则编码一次
            obj[keys[i]] = mText.decode(obj[keys[i]], decodeFunc);
        }
        //返回
        return obj;
    }
    //对一个数据进行编码
    mText.decodeArray = function (arr, decodeFunc) {
        //
        if (arguments.length == 1 || !decodeFunc || !_.isFunction(decodeFunc)) {
            //
            decodeFunc = _.decode;
        }
        //对于不是array对象，或者无效的值，直接返回
        if (!!arr && _.isArray(arr)) {
            //必须是有效值
            for (var i = 0; i < arr.length ; i++) {
                //直接对数组内容进行编码
                arr[i] = mText.decode(arr[i], decodeFunc);
            }
        }
        //
        return arr;
    }
    //把underscore里面的函数都转到mText里面来
    mText.decode = function (obj, decodeFunc) {
        //
        if (!obj) {
            return obj;
        }
        //如果是数组
        if (_.isArray(obj)) {
            //
            return mText.decodeArray(obj, decodeFunc);
        }
        //如果是字符串
        if (typeof obj === "string") {
            //
            //
            if (arguments.length == 1 || !decodeFunc || !_.isFunction(decodeFunc)) {
                //直接编码返回
                return _.decode(obj);
            }
            return decodeFunc(obj);
        }
        //如果是object
        if (typeof obj === "object") {
            //
            return mText.decodeObject(obj, decodeFunc);
        }
        //其他的就直接返回
        return obj;
    };

    /*
        Json处理的函数
    */
    //将一个对象的所有字符串属性进行编码，确保解码出来的时候不会有问题
    mText.jsonEncode = function (obj) {
        //
        return mText.encode(obj, _.jsonDecode);
    }
    //将一个对象的所有字符串属性进行编码，确保解码出来的时候不会有问题
    mText.jsonDecode = function (obj) {
        //
        return mText.decode(obj, _.jsonDecode);
    }
    //将一个对象转化成一个json格式的string，里面所有字符串属性都会被JsonEcode
    mText.toJson = function (obj) {
        //
        return JSON.stringify(mText.jsonEncode(obj));
    }
    //将一个json格式的string转化成一个对象，然后将里面所有的属性都JsonDecode
    mText.parseJson = function (obj) {
        //
        return mText.jsonDecode(!!obj ? JSON.parse(obj) : obj);
    }

    //64base64编码
    mText.Base64 = function () {

        // private property
        _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

        // public method for encoding
        this.encode = function (input) {
            var output = "";
            var chr1, chr2, chr3, enc1, enc2, enc3, enc4;
            var i = 0;
            input = _utf8_encode(input);
            while (i < input.length) {
                chr1 = input.charCodeAt(i++);
                chr2 = input.charCodeAt(i++);
                chr3 = input.charCodeAt(i++);
                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;
                if (isNaN(chr2)) {
                    enc3 = enc4 = 64;
                } else if (isNaN(chr3)) {
                    enc4 = 64;
                }
                output = output +
                _keyStr.charAt(enc1) + _keyStr.charAt(enc2) +
                _keyStr.charAt(enc3) + _keyStr.charAt(enc4);
            }
            return output;
        }

        // public method for decoding
        this.decode = function (input) {
            var output = "";
            var chr1, chr2, chr3;
            var enc1, enc2, enc3, enc4;
            var i = 0;
            input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");
            while (i < input.length) {
                enc1 = _keyStr.indexOf(input.charAt(i++));
                enc2 = _keyStr.indexOf(input.charAt(i++));
                enc3 = _keyStr.indexOf(input.charAt(i++));
                enc4 = _keyStr.indexOf(input.charAt(i++));
                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;
                output = output + String.fromCharCode(chr1);
                if (enc3 != 64) {
                    output = output + String.fromCharCode(chr2);
                }
                if (enc4 != 64) {
                    output = output + String.fromCharCode(chr3);
                }
            }
            output = _utf8_decode(output);
            return output;
        }

        // private method for UTF-8 encoding
        _utf8_encode = function (string) {
            string = string.replace(/\r\n/g, "\n");
            var utftext = "";
            for (var n = 0; n < string.length; n++) {
                var c = string.charCodeAt(n);
                if (c < 128) {
                    utftext += String.fromCharCode(c);
                } else if ((c > 127) && (c < 2048)) {
                    utftext += String.fromCharCode((c >> 6) | 192);
                    utftext += String.fromCharCode((c & 63) | 128);
                } else {
                    utftext += String.fromCharCode((c >> 12) | 224);
                    utftext += String.fromCharCode(((c >> 6) & 63) | 128);
                    utftext += String.fromCharCode((c & 63) | 128);
                }

            }
            return utftext;
        }

        // private method for UTF-8 decoding
        _utf8_decode = function (utftext) {
            var string = "";
            var i = 0;
            var c = c1 = c2 = 0;
            while (i < utftext.length) {
                c = utftext.charCodeAt(i);
                if (c < 128) {
                    string += String.fromCharCode(c);
                    i++;
                } else if ((c > 191) && (c < 224)) {
                    c2 = utftext.charCodeAt(i + 1);
                    string += String.fromCharCode(((c & 31) << 6) | (c2 & 63));
                    i += 2;
                } else {
                    c2 = utftext.charCodeAt(i + 1);
                    c3 = utftext.charCodeAt(i + 2);
                    string += String.fromCharCode(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63));
                    i += 3;
                }
            }
            return string;
        }
    };
    //
    mText.Base64Encode = function (str) {
        //
        var encodeValue = (!!str && typeof str == "string") ? new mText.Base64().encode(str) : str;
        //
        return encodeValue || str;
    }
    //
    mText.Base64Decode = function (str) {
        //
        var decodeValue = (!!str && typeof str == "string") ? new mText.Base64().decode(str) : str;
        //
        return decodeValue || str;
    }

    mText.format = function () {

        if (arguments.length > 0) {

            var result = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({)" + (i - 1) + "(})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }

            return result;
        }
        return undefined;
    }
})();