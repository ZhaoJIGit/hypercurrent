/*
这个js是针对一些输入框，假如提示性语言，文本框聚焦后，提示性语言消失，失去焦点后，如果文本框内的内容如果为空，则继续展示提示语言
需要考虑的点如下
1.普通的input比较简单，只需要加入hint属性就可以了，hint属性的值
2.但是针对easyui的numberbox里面不可以输入文本,如何解决；
3.combobox里面的input不再是原input，如何解决；
4.datebox里面的input也不再是原input，如何解决；
5.如果input 有validate功能，如何解决
使用方式：
$(input).mHint(hint);
$.mHint(hint,selector)
window.mHimt(hint,selector)
*/
(function () {
    //
    var mHint = (function () {

        var mHint = function (selector) {
            //
            var that = this;
            //
            selector = selector || "body";
            //
            this.initHint = function () {
                //获取hint对象
                var hints = that.findHintControl();
                //是否是easyui对象
                for (var i = 0; i < hints.length ; i++) {
                    //
                    var $hint = hints.eq(i);
                    //
                    var hint = $hint.attr("hint");
                    //
                    var value = hint;
                    //
                    if (value) {
                        //是否是easyui对象
                        var easyuiType = $hint.mIsEasyUIControl();
                        //如果是
                        if (easyuiType) {
                            easyuiType.setHint(value);
                        }
                        else {
                            //失去焦点的时候，给自己赋一下原先的值，jquery里面有处理
                            $hint.off("blur.hint").on("blur.hint", function () {
                                mHint.blurFunc(this);
                            }).off("focus.hint").on("focus.hint", function () {
                                mHint.focusFunc(this, value);
                            }).blur();
                        }
                    }
                }
            };
            //找到所有的hint属性的对象
            this.findHintControl = function () {
                //目前只有input,select,textarea
                inputs = $("input,select,textarea", selector).filter(function () { return $(this).attr("hint") && !$(this).hasClass("has-hint"); }) || [];
                //如果本身就是的
                if (inputs.length == 0 && $(selector).attr("hint") && !$(selector).hasClass("has-hint")) {
                    //加入
                    inputs = $(selector);
                }
                return inputs;
            };
        };
        return mHint;
    })();
    //扩展到$
    $.fn.initHint = function (selector) {
        selector = selector || this;
        new mHint(selector).initHint();
    };
    window.mHint = $.mHint = mHint;
    //扩展静态函数
    $.extend(mHint, {
        blurFunc: function (selector) {
            //如果已经加载
            if (!$(selector).hasClass("hint-blur") ) {
                $(selector).addClass("hint-blur");
                $(selector).val($(selector).val());
            }
        },
        focusFunc: function (selector, value) {
            //如果已经加载
            if ($(selector).hasClass("hint-blur") ) {
                $(selector).removeClass("hint-blur");
                $(selector).val($(selector).val());

            }
        }
    });
})();
$(function () {
    $("body").initHint();
});


//针对于密码框和文本框的切换问题
window.focusSwitch = function (src, target) {
    $(src).val("");
    $(target).val("");
    //密码输入文本框，获取焦点的时候，本身隐藏，由真正的密码输入框来实现功能
    $(src).off("focus.switch").on("focus.switch", function () {
        //附近的另外一个来显示
        $(this).hide();
        //密码输入框显示
        $(target).show().focus();
    }).keyup(function () {
        if ($(this).attr("hint") != $(this).val()) {
            $(target).val($(this).val());
        }
    }).blur(function () {
        if ($(this).attr("hint") != $(this).val()) {
            $(target).val($(this).val());
        }
    });
    //密码输入文本框，获取焦点的时候，本身隐藏，由真正的密码输入框来实现功能
    $(target).off("blur.switch").on("blur.switch", function () {
        //如果里面没有内容
        if ($(this).val().length == 0) {
            //附近的另外一个来显示
            $(this).hide();
            //文本框显示
            $(src).show();
        }
    }).keyup(function () {
        $(src).val($(this).val());
    }).blur(function () {
        $(src).val($(this).val());
    });;
}