/*
这是一个时间区间选择框，依赖于以下js以及css
jquery.daterangepicker.js
jquery.daterangepicker.custom.js
jquery.daterangepicker.css
jquery.daterangepicker.custom.css
//多语言信息
jquery.megi.date.js
//
使用方式
1.新建选择器
$(selector).mDaterangepicker({
    //相当于回调函数
	onChange: function(){},
	locale:"0x0009"
});
2.注销选择器
$(selector).mDaterangepicker("destory");
*/
(function (daterangepicker) {
    //判断是否存在
    if (!daterangepicker) {
        return false;
    }
    //
    var mDaterangepicker = (function () {
        //
        var _region = [];
        //区域代码
        var en_us = "0x0009", zh_cn = "0x7804", zh_hk = "0x7c04";
        //英文
        _region[en_us] = {
            closeText: "Done",
            prevText: "Prev",
            nextText: "Next",
            currentText: "Today",
            rangeStartTitle: 'Start Date',
            rangeEndTitle: 'End Date',
            nextLinkText: 'Next Month',
            prevLinkText: 'Last Month',
            doneButtonText: 'OK',
            cancelButtonText: 'Cancel',
            monthNames: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
            monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
            dayNamesShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
            dayNamesMin: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
            firstDay: 0,
            isRTL: false
        };
        //中文简体
        _region[zh_cn] = {
            closeText: "完成",
            prevText: "上一页",
            nextText: "下一页",
            currentText: "今天",
            rangeStartTitle: "开始日期",
            rangeEndTitle: "截止日期",
            nextLinkText: "下一月",
            prevLinkText: "上一月",
            doneButtonText: "完成",
            cancelButtonText: "取消",
            monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            monthNamesShort: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            dayNames: ["星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天"],
            dayNamesShort: ["一", "二", "三", "四", "五", "六", "日"],
            dayNamesMin: ["一", "二", "三", "四", "五", "六", "日"],
            firstDay: 0,
            isRTL: false
        };
        //中文繁体 其实在日期上面，繁体和中文简体差别不大
        _region[zh_hk] = {
            closeText: "完成",
            prevText: "上一页",
            nextText: "下一页",
            currentText: "今天",
            rangeStartTitle: "开始日期",
            rangeEndTitle: "截止日期",
            nextLinkText: "下一月",
            prevLinkText: "上一月",
            doneButtonText: "完成",
            cancelButtonText: '取消',
            monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            monthNamesShort: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            dayNames: ["星期一", "星期二", "星期三", "星期四", "星期五", "星期六", "星期天"],
            dayNamesShort: ["一", "二", "三", "四", "五", "六", "日"],
            dayNamesMin: ["一", "二", "三", "四", "五", "六", "日"],
            firstDay: 0,
            isRTL: false
        };
        //需要显示的内容
        var _presetRanges = [];
        //英文
        _presetRanges[en_us] = [
			 { text: 'Last 7 Days', dateStart: 'yesterday-7days', dateEnd: 'yesterday' },
             { text: 'Recent a Month', dateStart: 'yesterday-1month', dateEnd: 'yesterday' },
             { text: 'Recent a Year', dateStart: 'yesterday-1year', dateEnd: 'yesterday' }
        ];
        //中文
        _presetRanges[zh_cn] = [
			 { text: '最近七天', dateStart: 'yesterday-7days', dateEnd: 'yesterday' },
             { text: '最近一月', dateStart: 'yesterday-1month', dateEnd: 'yesterday' },
             { text: '最近一年', dateStart: 'yesterday-1year', dateEnd: 'yesterday' }
        ];
        //繁体
        _presetRanges[zh_hk] = [
			 { text: '最近七天', dateStart: 'yesterday-7days', dateEnd: 'yesterday' },
             { text: '最近壹月', dateStart: 'yesterday-1month', dateEnd: 'yesterday' },
             { text: '最近壹年', dateStart: 'yesterday-1year', dateEnd: 'yesterday' }
        ];
        //
        var _presets = [];
        //英文
        _presets[en_us] =
            {
                specificDate: 'Specific Date',
                allDatesBefore: 'All Dates Before',
                allDatesAfter: 'All Dates After',
                dateRange: 'Date Range'

            };
        //中文
        _presets[zh_cn] =
            {
                specificDate: '单独某天',
                allDatesBefore: '某天之前',
                allDatesAfter: '某天之后',
                dateRange: '日期区间'

            };
        //繁体
        _presets[zh_hk] =
            {
                specificDate: '單獨某天',
                allDatesBefore: '某天之前',
                allDatesAfter: '某天之後',
                dateRange: '日期區間'

            };
        //分隔符
        var rangeSplitter = "~";
        //默认的功能
        var _default = {
            earliestDate: Date.parse('-15years'),
            latestDate: Date.parse('today'),
            constrainDates: true,
            rangeSplitter: rangeSplitter,
            closeOnSelect: false
        };
        //定义daterangepicker的 defaults
        var _defaults = [];
        //中文
        _defaults[en_us] =
            {
                rangeStartTitle: 'Start date',
                rangeEndTitle: 'End date',
                nextLinkText: 'Next',
                prevLinkText: 'Prev',
                doneButtonText: 'Done'
            };
        //中文
        _defaults[zh_cn] =
            {
                rangeStartTitle: '开始日期',
                rangeEndTitle: '截止日期',
                nextLinkText: '下一页',
                prevLinkText: '上一页',
                doneButtonText: '完成'
            };
        //中文
        _defaults[zh_hk] =
            {
                rangeStartTitle: '开始日期',
                rangeEndTitle: '截止日期',
                nextLinkText: '下一頁',
                prevLinkText: '上一頁',
                doneButtonText: '完成'
            };
        //多语言定义
        var mDaterangepicker = function (selector, options, value) {
            //
            var that = this;
            //获取选择日期
            this.getRangeDate = function () {
                //获取用户选择的值
                var value = ($(selector).is("input") ? $(selector).val() : $(selector).text());
                //
                var dates = value.split(rangeSplitter);
                //单个日期情况，则开始日期和截止日期为同一天
                if (dates.length == 1) {
                    //返回一个数组
                    return [mDate.parse(dates[0].trim()), mDate.parse(dates[0].trim())];
                }
                else if (dates.length == 2) {
                    //返回一个数组
                    return [mDate.parse(dates[0].trim()), mDate.parse(dates[1].trimStart())];
                }
                return [, ];
            };
            //设置selector的值
            this.setSelectorRangeDate = function (dateValueString) {
                //如果是input
                if ($(selector).is("input")) {
                    $(selector).val(dateValueString);
                }
                else {
                    $(selector).text(dateValueString);
                }
            };
            //设置日期
            this.setRangeDate = function (dateValue) {
                //需要设置的值
                var dateValueString = "";
                //首先判断是单个日期还是多个日期
                if (typeof (dateValue) == "string") {
                    //根据~来区分
                    var dates = value.split(rangeSplitter);
                    //单个日期情况，则开始日期和截止日期为同一天
                    if (dates.length == 1) {
                        //返回一个数组
                        dateValueString = mDate.format(dates[0].trim());
                    }
                    else if (dates.length == 2) {
                        //返回一个数组
                        dateValueString = mDate.format(dates[0].trim()) + rangeSplitter + mDate.format(dates[1].trim());
                    }
                }
                else if (typeof (dateValue) == "object" && dateValue instanceof Array) {
                    //如果传了一个数组过来，则是设置两个日期
                    if (dateValue.length == 1) {
                        return that.setRangeDate(dateValue[0]);
                    }
                    else if (dateValue.length == 2) {
                        //开始日期截至日期
                        dateValueString = mDate.format(dateValue[0]) + rangeSplitter + mDate.format(dateValue[1]);
                    }
                }
                else if ((typeof (dateValue) == "object") && dateValue instanceof Date) {
                    //如果只传了一个日期值过来，则开始日期和截止日期相同
                    dateValueString = mDate.format(dateValue[0]);
                }
                //
                that.setSelectorRangeDate(dateValueString);
            };
            //如果是获取日期
            if (options === "getRangeDate") {
                //
                return that.getRangeDate();
            }
            else if (options === "setRangeDate") {
                //
                return that.setRangeDate(value);
            }
            else {
                //获取多语言信息
                var locale = options.locale || top.OrgLang[0].LangID;
                //region
                var region = _region[locale];
                //根据多语言获取内容
                var presetRanges = _presetRanges[locale];
                //需要显示的内容
                options.presetRanges = presetRanges;
                //预设的功能
                var presets = _presets[locale];
                //预设
                options.presets = presets;
                //日期格式
                region.dateFormat = mDate.DateFormat.toLowerCase().replace('yyyy', 'yy');
                //日期格式
                options.dateFormat = region.dateFormat;
                //扩展多语言信息
                options.datepickerOptions = region;
                //默认的
                _default = $.extend(_default, _defaults[locale]);
                //继续扩展
                options = $.extend(options, _default);
                //调用封装好的函数
                $(selector).daterangepicker(options);
            }
        };
        //返回
        return mDaterangepicker;
    })();
    //同样赋值给$.fn
    $.fn.mDaterangepicker = function (options, value) {
        //
        return new mDaterangepicker(this, options, value);
    };
})($.fn.daterangepicker)