(function () {

    var mSelect = (function () {

        var selector;

        var dynamic;

        var className = "click2select";

        var clickEventName = "click.click2select";

        var blurEventName = "blur.click2select";

        var inputSelect = "input:text:not(.click-no-select), input:password, textarea:not(.click-no-select)";

        var mSelect = function (sel, dyn) {

            var that = this;

            selector = sel == document ? inputSelect : sel;

            dynamic = dyn;

            //文本是否选中
            this.isTextSelected = function (input) {
                if (typeof input.selectionStart == "number") {
                    return input.selectionEnd - input.selectionStart > 0;
                } else if (typeof document.selection != "undefined") {
                    return document.selection.createRange().text.length > 0;
                }
            }

            //点击选中事件
            this.clickEvent = function (evt) {
                var input = $(evt.srcElement || evt.target)[0];

                !$(input).hasClass(className) && !that.isTextSelected(input) && $(input).selectAll(evt);

                $("." + className).removeClass(className);

                $(input).addClass(className);

                //window.console && window.console.log("selecting...");
            }

            //失去焦点事件
            this.blurEvent = function (evt) {
                var input = $(evt.srcElement || evt.target)[0];

                $(input).removeClass(className);
            }

            //初始化
            this.init = function () {

                if (dynamic) {
                    $(document).off(clickEventName, selector).on(clickEventName, selector, function (evt) {

                        that.clickEvent(evt);
                    }).off(blurEventName, selector).on(blurEventName, selector, function (evt) {

                        that.blurEvent(evt);
                    });
                }
                else {
                    $(selector).off(clickEventName).on(clickEventName, function (evt) {

                        that.clickEvent(evt);
                    }).off(blurEventName).on(blurEventName, function (evt) {

                        that.blurEvent(evt);
                    });
                }
            }
        }

        return mSelect;
    })();

    $.fn.mSelect = function (isDynamic) {

        return new mSelect(this, !!isDynamic).init();
    }

    window.mSelect = mSelect;
})()