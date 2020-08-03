(function () {

    var mGLDatebox = (function () {
        var mGLDatebox = function (selector, options) {

            var that = this;

            var isPeriodSettledUrl = "/GL/GLDashboard/IsPeirodSettled";
            var getPeriodSettledUrl = "/GL/GLDashboard/GetSettledPeriod";

            this.glSelect = function (date, func) {

                if (date) {

                    mAjax.Post(isPeriodSettledUrl, {
                        date: date
                    }, function (data) {

                        if (data == true || data == "true") {
                            $.isFunction(func) && func(dateStr);
                        }
                        else {

                            mDialog.message(HtmlLang.Write(LangModule.GL, "Period", "期间") + date.getFullYear() + "-" + (date.getMonth() + 1) + HtmlLang.Write(LangModule.Common, "IsClosedInGeneralLedger", "在总账已经结账，请先反结账"));
                            var datebox = new mDatebox(selector);

                            datebox.setValue("");

                            datebox.getInput().focus();

                            return false;
                        }
                    }, function () { }, false, true);
                }
            };
            this.initCalendar = function () {
                var disabled = $(selector).attr("disabled");
                mAjax.Post(getPeriodSettledUrl, null, function (msg) {
                    if (msg.Success == false) {
                        return;
                    }

                    //提示给用户的最小值或者最大值，默认取开始日期，因为结账日期会在开始日期之后
                    var borderDate = msg.BeginDate;
                    //置灰显示的日期
                    var maskDate = msg.OpenDate;

                    var vt = options.validType;

                    var minValid = true;

                    //如果没有校验类型，则默认是总账可用期间，
                    if (!vt) {
                        minValid = true;
                        options.validType = " minDate ['" + borderDate + "'] ";
                    }
                    else {

                        //如果有设置校验类型，分两种，一种设置了minDate，一种设置了maxDate

                        //设置了minDate 
                        if (vt.indexOf("minDate") >= 0) {

                            minValid = true;

                            var reg = new RegExp("minDate\\s\\['(.*?)'\\]", "ig");

                            //找出本身设置的最小值
                            var matches = reg.exec(vt);

                            //如果匹配则，取出来，然后做比较
                            if (!!matches) {
                                var setMinDate = matches[1];
                                //如果设置的最小值，小于总账当前期间，则已设置的值为准
                                if (mDate.parse(setMinDate) < mDate.parse(borderDate)) {
                                    borderDate = setMinDate;
                                }
                                options.validType = vt.replace(reg, "minDate ['" + borderDate + "']");
                            }
                        }
                        else if (vt.indexOf("maxDate") >= 0) {

                            minValid = false;

                            var reg = new RegExp("maxDate\\s\\['(.*?)'\\]", "ig");

                            //找出本身设置的最小值
                            var matches = reg.exec(vt);

                            //如果匹配则，取出来，然后做比较
                            if (!!matches) {
                                borderDate = matches[1];
                                maskDate = borderDate;
                            }
                        }

                    }

                    $(selector).datebox(options);
                    new mDatebox(selector);

                    var c = $(selector).datebox('calendar');

                    c.calendar({
                        validator: function (date) {
                            if (disabled == "disabled") {
                                return true;
                            }
                            var year = date.getFullYear();
                            var month = date.getMonth() + 1;

                            var arr = maskDate.split('-');
                            if (minValid && ((year * 100 + month) < (parseInt(arr[0]) * 100 + parseInt(arr[1])))) {
                                return false;
                            }

                            if (!minValid && ((year * 100 + month) >= (parseInt(arr[0]) * 100 + parseInt(arr[1])))) {
                                return false;
                            }

                            return true;
                        }
                    });

                    //$(selector).datebox("setValue", $(selector).val());
                }, function () { }, false, true);
            };
            //
            this.init = function () {
                //获取onselect事件
                var selectFunc = options.onSelect;
                //如果select为空
                if (!$.isFunction(selectFunc)) {
                    //
                    selectFunc = that.glSelect;
                }
                else {
                    selectFunc = function (date) {
                        //如果通过了校验
                        that.glSelect(date, selectFunc);
                    }
                }

                that.initCalendar();
            }
        }
        return mGLDatebox;
    })();
    //
    $.fn.mGLDatebox = function (options) {
        //
        return new mGLDatebox(this, options).init();
    }
})();

$(function () {

    $("input.gl-easyui-datebox").each(function () {
        //
        var dataOptions = {};
        //获取dataoptions
        var dataAttr = $(this).attr("data-options");
        //有data-options属性
        if (dataAttr) {
            dataOptions = $.extend(eval("({" + dataAttr + "})"), dataOptions || {});
        }
        $(this).mGLDatebox(dataOptions);
    });
});


