/*
   美记系统数据管理类，包括钱币的格式转化
*/
;
(function (Megi) {
    var mMath = (function () {
        var mMath = function () {

        };
        return mMath;
    })();
    //扩展静态方法
    $.extend(mMath, {
        toMoneyFormat: function (number, precision, minPrecision) {
            var newPrecision = precision || 2;
            if (minPrecision != undefined) {
                precision = minPrecision;
                if (number != null && number != "") {
                    var numberString = String(number);
                    if (numberString.indexOf(".") > -1) {
                        var arrNumber = numberString.split(".");
                        if (arrNumber.length == 1 || arrNumber[1].length <= minPrecision) {
                            precision = minPrecision;
                        } else if (arrNumber[1].length < newPrecision) {
                            precision = arrNumber[1].length
                        } else {
                            precision = newPrecision;
                        }
                    }
                }
            }
            return mMath.toMilliFormat(mMath.toDecimal(number, precision));
        },
        toMilliFormat: function (number) {
            var num = number + "";
            num = num.replace(new RegExp(",", "g"), "");
            var symble = "";
            if (/^([-+]).*$/.test(num)) {
                symble = num.replace(/^([-+]).*$/, "$1");
                num = num.replace(/^([-+])(.*)$/, "$2");
            }

            if (/^[0-9]+(\.[0-9]+)?$/.test(num)) {
                var num = num.replace(new RegExp("^[0]+", "g"), "");
                if (/^\./.test(num)) {
                    num = "0" + num;
                }

                var decimal = num.replace(/^[0-9]+(\.[0-9]+)?$/, "$1");
                var integer = num.replace(/^([0-9]+)(\.[0-9]+)?$/, "$1");

                var re = /(\d+)(\d{3})/;

                while (re.test(integer)) {
                    integer = integer.replace(re, "$1,$2");
                }
                return symble + integer + decimal;

            } else {
                return number;
            }
        },
        toDecimal: function (floatvar, num) {
            if (floatvar == undefined || floatvar == null || $.trim(floatvar) == "") {
                return "";
            }
            if (isNaN(floatvar)) {
                return floatvar;
            }
            var per = 100;
            if (num == undefined) {
                num = 2;
            }
            if (num == 0) {
                var numberString = String(floatvar);
                if (numberString.indexOf(".") > -1) {
                    return numberString.split(".")[0]
                } else {
                    return numberString;
                }
            } else if (num == 3) {
                per = 1000;
            } else if (num == 4) {
                per = 10000;
            } else if (num == 5) {
                per = 100000;
            } else if (num == 6) {
                per = 1000000;
            } else if (num == 7) {
                per = 10000000;
            } else if (num == 8) {
                per = 100000000;
            }
            var f_x = parseFloat(floatvar);
            if (isNaN(f_x)) {
                alert('function:changeTwoDecimal->parameter error');
                return false;
            }
            if (f_x > 0) {
                f_x = f_x + 0.0000000001;
            }
            var f_x = Math.round(f_x * per) / per;
            var s_x = f_x.toString();
            var pos_decimal = s_x.indexOf('.');
            if (pos_decimal < 0) {
                pos_decimal = s_x.length;
                s_x += '.';
            }
            while (s_x.length <= pos_decimal + num) {
                s_x += '0';
            }
            return s_x;
        }
    });
    //兼容目前版本
    $.mMath = window.mMath = Megi.Math = mMath;
})(Megi)