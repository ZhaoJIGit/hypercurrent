
(function () {

    var GLVoucherQuarter = (function () {


        var GLVoucherQuarter = function () {

            //
            var that = this;
            //
            var dateInput = ".hp-date-input";
            //
            var periodInput = ".hp-period-input";
            //初始化页面的默认值
            this.initDomValue = function () {
                //
                var year = new Date().getFullYear();
                //
                var quarter = (new Date().getMonth()) / 3;
                //
                var yearList = [];
                //默认为当前日期
                for (var i = year; i > 2000; i--) {
                    //把年份加入
                    yearList.push({
                        text: i,
                        value: i
                    });
                }
                //年度
                $(dateInput).combobox({
                    width: 60,
                    textField: 'text',
                    valueField: 'value',
                    data: yearList,
                    onLoadSuccess: function () {
                        $(dateInput).combobox("setValue", year);
                    }
                });
                //默认日期
                $(periodInput).val(GLVoucherHome.avaliablePeriod());
            };
            //
            this.init = function () {
                //
                that.initDomValue();
            }

        };

        //
        return GLVoucherQuarter;
    })();

    window.GLVoucherQuarter = GLVoucherQuarter;
})()