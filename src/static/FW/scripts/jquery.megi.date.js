//扩展Date对象
//使用方式  
//1.var date = new mDate() //默认获取用户当前时间的当时日期
//2.var date = new mDate("2015-1-1", "YYYY-MM-DD")
//3.var date = new mDate("/Date 2344234234/","YYYY-MM-DD");
//4.var date = new mDate(123123213,"YYYY-MM-DD");
//5.还有很多的静态函数供使用
//6.调用mDate静态函数 format  参数是一个Date类型的
//目前支持的日期格式 yyyy/YYYY（yy/YY)  MM/mm DD/dd的任意组合形式
//调用对象的.format()方法，能获取其对应转化后的字符串

; (function (Megi) {
    //检测Megi是否初始化
    Megi = Megi || {};
    //新建mDate
    var mDate = (function () {
        //新建一个mDate function对象
        var mDate = function (value, format) {
            //把this赋值给that
            var that = this;
            //获取格式化
            this.InitFormat = function (format) {
                //如果没有初始化
                that.DateFormat = format;
                //如果还是没有值
                if (!that.DateFormat) {
                    //获取日期格式，默认为YYYY-MM-DD
                    that.DateFormat = top.MegiGlobal ? top.MegiGlobal.MDateFormat : "";
                    //再去默认日期
                    that.DateFormat = that.DateFormat || LangKey.DateFormat;
                    //如果还为空
                    that.DateFormat = that.DateFormat || "YYYY-MM-DD";
                }
                //转化为大写
                that.DateFormat.toUpperCase();
                //否则直接返回
                return that.DateFormat;
            }
            //初始化值
            this.InitValue = function (value) {
                //如果value为空，则默认为用户当前时区的日期
                value = value || that.DateNow;
                //赋值
                that.DateValue = value;
                //返回
                return that.DateValue;
            };
            //初始化格式
            this.InitFormat(format);
            //初始化值
            this.InitValue(value);
        }
        //返回
        return mDate;
    })();

    //扩展mDate的静态函数
    $.extend(mDate, {
        //默认的日期格式
        DateFormat: (function () {
            //
            return (top.MegiGlobal ? top.MegiGlobal.MDateFormat : "") || "YYYY-MM-DD";
        })(),
        //获取分隔符
        Spliter: (function () {
            //获取第一个分隔符就好了
            return ((top.MegiGlobal ? top.MegiGlobal.MDateFormat : "") || "YYYY-MM-DD").match(/\W/g)[0];
        })(),
        //默认的时间格式
        TimeFormat: (function () {
            var timeFormat = (typeof (MegiGlobal) != "undefined" && MegiGlobal.MTimeFormat != undefined) ? MegiGlobal.MTimeFormat : "";
            if (timeFormat == null || timeFormat == "") {
                timeFormat = "HH:mm:ss";
            }
            return timeFormat;
        })(),
        //默认日期
        DateValue: (function () {
            //如果value为空，则默认为用户当前时区的日期
            var value = mDate.DateValue || mDate.DateNow;
            //赋值
            mDate.DateValue = value;
            //返回
            return mDate.DateValue;
        })(),
        //当前日期
        DateNow: (function () {
            //获取今天 默认 用 new Date 后期改成获取用户当前时区的时间
            return new Date();
        })(),
        //判断一个字符串是否是一个日期格式的，现在支持 yyyy/mm/dd 以及yyyy-mm-dd格式的
        isDateString: function (str) {
            if (!/^(\d{4})\/(\d{1,2})\/(\d{1,2})$/.test(str) && !/^(\d{4})\-(\d{1,2})\-(\d{1,2})$/.test(str)) {
                return false;
            }
            var year = RegExp.$1 - 0;
            var month = RegExp.$2 - 1;
            var date = RegExp.$3 - 0;
            var obj = new Date(year, month, date);
            return !!(obj.getFullYear() == year && obj.getMonth() == month && obj.getDate() == date);
        },
        //判断一个日期是是否是一个合法的日期
        isValidDate: function (str) {
            //
            if (!str) {
                return false;
            }
            else {
                var date = mDate.parse(str);
                //
                if (date && date.getTime() > new Date(1900, 0, 1).getTime()) {
                    //
                    return true;
                }
            }
            return false;
        },
        //总的格式化函数
        format: function (value, format) {
            try {
                //如果用户传入了格式
                format = format || mDate.DateFormat;
                //转化为大写
                format = format.toUpperCase();
                //如果用户传入了值
                value = value || mDate.DateValue;
                //根据 DateValue来判断调用哪个函数
                if (typeof value == "number") {
                    //返回纯数字的转化
                    return mDate.formatNumber(value, format);
                }
                else if (typeof value == "string" && value.indexOf("/Date(") > -1) {
                    //调用直接的eval解析方式
                    return mDate.formatValue(value, format);
                }
                else if (typeof value == "string") {
                    //调用 YYYY-MM-DD格式的转化
                    return mDate.formatString(value, format);
                }
                else if (typeof value == "object" && value instanceof Date) {
                    //调用formatDate函数
                    return mDate.formatDate(value, format);
                }
                else {
                    //返回undefined
                    return "";
                }
            } catch (e) {
                //返回未定义
                return "";
            }

        },
        //格式器
        formatter: function (date, format) {
            //formatter必须是字符串
            format = (format && (typeof format == "string")) ? format : undefined;
            //如果没有传入date，则直接返回""
            if (!date) {
                return "";
            }
            //返回
            return mDate.format(date, format);
        },
        //总的格式化函数
        parse: function (value, format) {
            try {
                //如果用户传入了格式
                format = format || mDate.DateFormat;
                //转化为大写
                format = format.toUpperCase();
                //如果用户传入了值
                value = value || mDate.DateValue;
                //根据 DateValue来判断调用哪个函数
                if (typeof value == "number") {
                    //返回纯数字的转化
                    return mDate.parseNumber(value, format);
                }
                else if (typeof value == "string" && value.indexOf("/Date(") > -1) {
                    //调用直接的eval解析方式
                    return mDate.parseValue(value, format);
                }
                else if (typeof value == "string") {
                    //调用 YYYY-MM-DD格式的转化
                    return mDate.parseString(value, format);
                }
                else if (typeof value == "object" && value instanceof Date) {
                    //调用formatDate函数
                    return value;
                }
                else {
                    //返回undefined
                    return undefined;
                }
            } catch (e) {
                //返回未定义
                return undefined;
            }
        },
        //定义日期转化函数，把string，转化为date类型的
        parser: function (value, format) {
            //如果没有值，就默认为今天
            if (!value) {
                //
                return mDate.DateNow;
            }
            //formatter必须是字符串
            format = (format && (typeof format == "string")) ? format : undefined;
            //
            return mDate.parse(value, format);
        },
        //将/Date 2423423/格式转化为日期格式
        parseValue: function (value, format) {
            //异常处理
            try {
                //根据传入参数来给value 和 format赋值
                switch (arguments.length) {
                    //没有传入参数
                    case 0:
                        value = mDate.DateValue;
                        format = mDate.DateFormat;
                        break;
                    case 1:
                        //如果只有一个参数，表示只传入了value，则format取系统默认
                        format = mDate.DateFormat;
                        break;
                    default:
                        break;
                }
                //一般传入的格式是/Date 2312312312/格式的
                var stringValue = value.replace(/\\|\/|\"|Date|\(|\)/gi, '');
                //现在只剩下数字了
                var dateValue = stringValue < 0 ? new Date(1900, 0, 1) : new Date(parseInt(stringValue));
                //如果小于1900，表示传入的是一个无效的值
                if (dateValue.getFullYear() < 1900) {
                    //返回空
                    throw e;
                }
                //返回格式
                return dateValue;
            } catch (e) {
                //转化异常，则返回空
                throw e;
            }
        },
        //将/Date 2423423/格式转化为日期格式
        formatValue: function (value, format) {

            //异常处理
            try {
                //根据传入参数来给value 和 format赋值
                switch (arguments.length) {
                    //没有传入参数
                    case 0:
                        value = mDate.DateValue;
                        format = mDate.DateFormat;
                        break;
                    case 1:
                        //如果只有一个参数，表示只传入了value，则format取系统默认
                        format = mDate.DateFormat;
                        break;
                    default:
                        break;
                }
                //直接调用
                return mDate.formatDate(mDate.parseValue(value, format), format);
            } catch (e) {
                //转化异常，则返回空
                throw e;
            }
        },
        //将数值类型的转化为日期格式
        formatNumber: function (value, format) {
            //异常捕获
            try {
                //根据传入参数来给value 和 format赋值
                switch (arguments.length) {
                    //没有传入参数
                    case 0:
                        value = mDate.DateValue;
                        format = mDate.DateFormat;
                        break;
                    case 1:
                        //如果只有一个参数，表示只传入了value，则format取系统默认
                        format = mDate.DateFormat;
                        break;
                    default:
                        break;
                }
                //直接转化就行
                return mDate.formatDate(new Date(value), format);
            } catch (e) {
                //返回错误
                throw e;
            }
        },
        //将数值类型的转化为日期格式
        parseNumber: function (value) {
            //异常捕获
            try {
                //根据传入参数来给value 和 format赋值
                switch (arguments.length) {
                    //没有传入参数
                    case 0:
                        value = mDate.DateValue;
                        break;
                    default:
                        break;
                }
                //直接转化就行
                return new Date(value);
            } catch (e) {
                //返回错误
                throw e;
            }
        },
        //将文本类型的转化为日期格式
        formatString: function (value, format) {
            //返回转化为日期格式后的字符串
            try {
                //根据传入参数来给value 和 format赋值
                switch (arguments.length) {
                    //没有传入参数
                    case 0:
                        value = mDate.DateValue;
                        format = mDate.DateFormat;
                        break;
                    case 1:
                        //如果只有一个参数，表示只传入了value，则format取系统默认
                        format = mDate.DateFormat;
                        break;
                    default:
                        break;
                }

                //先转化为日期，再转回为字符串
                return mDate.formatDate(mDate.parseString(value, format), format);
            } catch (e) {
                //返回错误
                throw e;
            }
        },
        //将文本类型的转化为日期格式
        parseString: function (value, format) {
            //异常捕获
            try {
                //根据传入参数来给value 和 format赋值
                switch (arguments.length) {
                    //没有传入参数
                    case 0:
                        value = mDate.DateValue;
                        format = mDate.DateFormat;
                        break;
                    case 1:
                        //如果只有一个参数，表示只传入了value，则format取系统默认
                        format = mDate.DateFormat;
                        break;
                    default:
                        break;
                }
                //获取所有分隔符
                var spliters = format.match(/\W/g);
                //先全部转化为以-为分隔符的格式
                var newFormat = format.replace(/\W/g, '-');
                //然后分开为数组形式 例如  yyyy-MM-dd 变成['yyyy','MM','dd']
                var formats = newFormat.split('-');
                //将值也转化为'-'做分割
                var newValue = value.replace(/\W/g, '-');
                //将值也变成数组形式例如 2012-1-1 变成[2012,1,1];
                var values = newValue.split('-');
                //定义三个变量，获取年、月、日
                var year = 1900, month = 0, day = 1, hour = 0, minute = 0, second = 0;
                //根据格式从数值中获取相应的年、月、日
                for (var i = 0; i < formats.length ; i++) {
                    //老老实实，进行 年月日比较
                    switch (formats[i].substr(0, 1).toUpperCase()) {
                        //年份
                        case "Y":
                            //判断位数
                            if (formats[i].length == 2) {
                                //针对那种 yy-MM-dd的形式
                                year = mDate.DateNow.getFullYear().toString().substr(2, 2);
                            }
                            else {
                                //获取年份
                                year = values[i];
                            }
                            //结束
                            break;
                            //月份
                        case "M":
                            //获取月份
                            month = values[i] || month;
                            //结束
                            break;
                            //获取天
                        case "D":
                            //日期
                            day = values[i] || day;
                            break;
                        case "H":
                            //日期
                            hour = values[i] || hour;
                            break;
                        case "m":
                            //日期
                            minute = values[i] || minute;
                            break;
                        case "s":
                            //日期
                            second = values[i] || second;
                            break;
                        default:
                            //如果啥都不是，那肯定是有问题的
                            throw e;
                    }
                }
                //转化为日期
                var date = new Date(year, month, day, hour, minute, second);
                //如果date是不合法日期,抛异常
                if (date.toString() == 'Invalid Date') {
                    throw e;
                }
                //
                var maxDay = new Date(year, month, 0).getDate();
                //
                day = day > maxDay ? maxDay : day;
                //如果正常，在将月份减少1
                date = new Date(year, month - 1 < 0 ? 0 : month - 1, day);
                //如果到这里都还没有报错了那就是没问题的日期了
                return date;
            } catch (e) {
                //返回错误
                throw e;
            }
        },

        formatDateTime: function (value) {
            var date = Date.parse(value);
            return date.format(mDate.DateFormat + " HH:mm:ss");
        },
        //直接转化Date对象的类型
        formatDate: function (date, format) {
            try {
                //根据传入参数来给value 和 format赋值
                switch (arguments.length) {
                    //没有传入参数
                    case 0:
                        value = mDate.DateValue;
                        format = mDate.DateFormat;
                        break;
                    case 1:
                        //如果只有一个参数，表示只传入了value，则format取系统默认
                        format = mDate.DateFormat;
                        break;
                    default:
                        break;
                }
                //获取年月日
                var year = date.getFullYear(), month = date.getMonth(), day = date.getDate();
                //如果是1900的数据，则返回空字符串
                if (year <= 1901) {
                    return "";
                }

                //字符串最后的结果
                var result = format;
                //把format转化为大写
                result = result.toUpperCase();
                //取出年对应的字符串
                var y = result.match(/Y/g).join('');
                //代入年份
                result = result.replace(y, year.toString().substr(year.toString().length - y.length, y.length));
                //获取月份对应的字符串
                var m = result.match(/M/g).join('');
                //代入月份
                result = result.replace(m, (month + 1) < 10 ? ('0' + (month + 1).toString()) : (month + 1));
                //获取日期
                var d = result.match(/D/g).join('');
                //代入日期
                result = result.replace(d, day < 10 ? ('0' + day.toString()) : day);
                //直接调用 formatNumber的形式
                return result;
            } catch (e) {
                //直接抛出
                throw e;
            }
        },
        //重写toString方法
        toString: function (format) {
            //返回格式化的字符串
            return mDate.format(undefined, format);
        },
        compare: function (value1, value2) {
            return mDate.parse(value1) > mDate.parse(value2);
        }
    });
    //将MegiDate赋值到Megi
    Megi.MegiDate = window.MegiDate = $.MegiDate = mDate;
    //
    $.mDate = window.mDate = mDate;
    //
})(Megi);

//使用函数劫持的方式把combobox的setText函数进行重写
//$.fn.datebox.defaults.formatter = $.mDate.formatter;

//同时要劫持 combo.parser函数
//$.fn.datebox.defaults.parser = $.mDate.parser;