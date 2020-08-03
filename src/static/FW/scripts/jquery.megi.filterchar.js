
//键盘字符过滤，options.filterChar：字符间用逗号隔开

(function ($) {
    $.fn.megifilterchar = function (options) {
        //默认给空格
        options = options ? options : {};
        options.filterChar = options.filterChar ? options.filterChar : [];
        options.isOnlyFirst = false;
        var htmlOptions = {};
        if ($(this).data("options")) {
            var temp = "{" + $(this).data("options") + "}";
            htmlOptions = eval("(" + temp + ")");
        }

        //如果设置了过滤字符，则按设置进行过滤,否则去默认的
        if (htmlOptions.filterChar) {
            //进行合并
            var array = htmlOptions.filterChar.split(',');
            options.filterChar = $.merge(options.filterChar, array);
        }

        if (htmlOptions.isOnlyFirst) {
            options.isOnlyFirst = true;
        }

        //没有指定，就返回
        if (!options.filterChar || options.filterChar.length <= 0) {
            return;
        }
        var selector = $(this);
        //给输入框绑定一个Keyup事件
        //$(this).keyup(function (e) {
        //    keyUpEvent(selector, options, e);
        //});

        $(this).off("keyup afterpaste paste").on("keyup afterpaste paste", function (e) {
            setTimeout(function () {
                keyUpEvent(selector, options, e);
            }, 100);
            
        });
    };

    function keyUpEvent(selector, options, e) {
        var value = $(selector).val();
        var length = value.length;
        //中转
        var temp = value;
        //从最后一个字符开始进行屏蔽，直到不是屏蔽的字符

        var cursorIndex = getCursorPos(selector);
        for (var i = length - 1; i >= 0; i--) {
            for (var y = 0 ; y < options.filterChar.length; y++) {
                if (options.isOnlyFirst && length > 1) {
                    //如果输入的为空格，前一个也为空格，就进行处理，其他情况不处理
                    if (value[i] == options.filterChar[y] && value == options.filterChar[y]) {
                        temp = value.substring(0, length - 1);
                        length = length - 1;
                    }
                } else {
                    //if (value[i] == options.filterChar[y]) {
                    //    //只屏蔽第一个
                    //    temp = value.substring(cursorIndex - 1, length - cursorIndex);
                    //    length = length - 1;
                    //}

                    temp = temp.split(options.filterChar[y]).join('');
                }
            }
        }

        var className = $(selector).attr("class");
        //多语言输入框
        if (className.indexOf("m-lang") >= 0) {
            var currentLang = $.cookie('MLocaleID');
            var langData = $(selector).getLangEditorData();
            if (langData && langData.MMultiLanguageField) {
                var langIsSame = false;
                //取第一个值，比较是否和其他两个值相等
                var langValue = langData.MMultiLanguageField[0].MValue;
                for (var x = 1; x < langData.MMultiLanguageField.length; x++) {
                    if (langValue != langData.MMultiLanguageField[x].MValue) {
                        langIsSame = false;
                        break;
                    } else {
                        langIsSame = true;
                    }
                }

                for (var j = 0; j < langData.MMultiLanguageField.length; j++) {
                    var lang = langData.MMultiLanguageField[j];
                    if (lang.MLocaleID == currentLang || langIsSame == true) {
                        lang.MValue = temp;
                    }
                }
            }

            $(selector).initLangEditor(langData);

        } else {
            //一般输入框
            $(selector).val(temp);
        }

        setCursorPos(selector, cursorIndex);
    };
    //获取光标位置
    function getCursorPos(selector) {
        if (navigator.userAgent.indexOf("MSIE") > -1) { // IE
            var range = document.selection.createRange();
            range.text = '';
            range.setEndPoint('StartToStart', selector.createTextRange());
            return range.text.length;
        } else {
            return selector[0].selectionStart;
        }
    };
    //设置光标位置
    function setCursorPos(selector, pos) {
        if (navigator.userAgent.indexOf("MSIE") > -1) {
            var range = document.selection.createRange();
            var textRange = selector.createTextRange();
            textRange.moveStart('character', pos);
            textRange.collapse();
            textRange.select();
        } else {
            selector[0].setSelectionRange(pos, pos);
        }
    }

    //遍历所有的控件，进行初始化
    $(function () {
        $(".easyui-filterchar", "body").megifilterchar();
    });
})(jQuery)