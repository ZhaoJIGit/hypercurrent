/*
这是一个帮助类，其目的在于实现，某个按钮或者dom，响应鼠标事件的时候，弹出可操作的对话提醒框，
框内可是完全的文本信息，也可以是带有操作功能的按钮
1.框内的内容为用户自定义的一个selector
2.click
options 格式
{
    target :"#id",
    width:100,
    heigth:100,
    parent:parent,
    trigger:"click"/events类型,
    callback:function(event)//点击显示的时候，触发的事件
}
*/
(function () {
    //最外层的div样式
    var tipTopDivClass = "m-tip-top-div";
    //显示箭头的样式
    var tipArrowClass = "m-tip-arrow-up";
    //显示箭头的样式
    var tipArrowBgClass = "m-tip-arrow-bg";
    //是否是滚动隐藏
    var scrollToHide = "scrollToHide";
    //options的名称
    var dataOptions = "tip-options";
    //current样式
    var current = "current";
    //container样式
    var container = "m-sub-nav-container";
    //iframe遮罩样式
    var iframeTopBg = "iframe-top-bg";
    //常量定义
    var mTip = (function () {
        //
        var getRandomId = function () {
            var Str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (var i = 0, r = ""; i < 10; i++) {
                r += Str.charAt(Math.floor(Math.random() * 62));
            };
            return r;
        };
        //类定义
        var mTip = function (selector, options) {
            //当前div
            var $div = "";
            //
            var that = this;
            //
            options = options || $(selector).attr(dataOptions);
            //显示数据
            this.init = function () {
                //获取对象，需要展示的内容
                var target = options.target;
                //事件，用户是否是点击selector后才展示tip
                var trigger = options.trigger || "click";
                //
                var hideTrigger = options.trigger == "mouseover" ? "mouseleave" : null;
                //父节点，将弹出框插入到的节点位置
                var parent = options.parent || "body";
                //先判断一下，是否已经有包含target的div了，如果有了就公用一个
                if ($(parent).hasClass(tipTopDivClass) && $(parent).find(target).length > 0) {
                    //共用
                    that.$div = $(parent);
                };
                //如果没有找到
                if (!that.$div) {
                    //宽度
                    var width = options.width || "300";
                    //高度
                    //var height = options.height || "200";
                    //新建一个id
                    var randomId = getRandomId();
                    //把randomID给selector的一个属性
                    $(selector).attr("tipId", randomId);
                    //html
                    //添加一个div
                    var topDiv = "<div id='" + randomId + "' class='" + tipTopDivClass + "' style='width:" + width + "px; " + (options.height ? ("height:" + options.height + "px;") : "") + "'>";
                    //
                    if (!options.hideArrow) {
                        //加一个箭头
                        var arrowBgSpan = "<span class='" + tipArrowBgClass + "'></span>";
                        //
                        topDiv += arrowBgSpan;
                    }
                    //加一个箭头
                    var arrowSpan = "<span class='" + tipArrowClass + "'></span>";
                    //
                    topDiv += arrowSpan;
                    //内容
                    topDiv += "</div>";
                    //加到body上
                    that.$div = $(topDiv);
                    //加入
                    that.$div.appendTo(parent);
                    //将用户的内容加到内部
                    $(target).appendTo(that.$div).show();
                }
                //注册事件
                that.event(trigger, hideTrigger);

            };
            //显示背景
            this.showBackground = function () {
                //iframe的背景
                var $iframeBg = $("." + iframeTopBg);
                //main
                var $main = $(".m-main");
                //显示并且调节样式
                $iframeBg.length > 0 && $main.length > 0 && $iframeBg.show().css({
                    width: $main.outerWidth(),
                    height: $main.outerHeight(),
                    left: $main.offset().left,
                    top: $main.offset().top
                });
                //
                return true;
            };
            //隐藏背景
            this.hideBackground = function () {
                //iframe的背景
                var $iframeBg = $("." + iframeTopBg);
                //隐藏
                $iframeBg.hide();
                //
                return true;
            };
            //关闭的时候调用的事件
            this.showCallback = function (event) {
                //如果显示，并且有回调函数
                that.$div.is(":visible") && that.showBackground() && options.callback && $.isFunction(options.callback) && options.callback(event, selector, that.$div);

            };
            this.hideCallback = function (event) {
                //如果显示，并且有回调函数
                that.$div.is(":hidden") && that.hideBackground() && options.closeCallback && $.isFunction(options.closeCallback) && options.closeCallback(event, selector, that.$div);
            };
            //事件
            this.event = function (trigger, hideTrigger) {
                //current样式
                var current = "current";
                //container样式
                var container = "m-sub-nav-container";
                //iframe遮罩样式
                var iframeTopBg = "iframe-top-bg";
                //什么时候显示
                $(selector).off(trigger + ".tip").on(trigger + ".tip", function (event) {
                    //其他的隐藏
                    $("." + tipTopDivClass).not(that.$div).hide();
                    //将滚动隐藏设置为false
                    $("." + tipTopDivClass).data(scrollToHide, false);
                    //处理回调函数
                    var handleCallback = function () {
                        //
                        that.showCallback(event);
                        //处理隐藏
                        that.hideCallback(event);
                    };
                    //显示与隐藏,如果是右键，则一直显示
                    trigger == "contextmenu" ? that.$div.fadeIn("normal", handleCallback) : that.$div.toggle("linear", handleCallback);
                    //组织冒泡事件
                    if (event.stopPropagation) {
                        // this code is for Mozilla and Opera 
                        event.stopPropagation();
                    }
                    else if (window.event) {
                        // this code is for IE 
                        window.event.cancelBubble = true;
                    }
                    //定位位置
                    that.locate("", trigger, event);
                    //
                    return false;
                });
                //给body注册事件，点击其他地方的时候隐藏所有的div，
                $(document).off("click.tip").on("click.tip", function (event) {


                    if (that.checkStopEvent(event))
                        return false;
                    //如果点击的对象是div，则不做处理
                    var target = $.browser.msie ? event.srcElement : event.target;
                    //
                    $("." + tipTopDivClass).hide();
                    //隐藏导航栏
                    $("." + container).hide();
                    //去掉导航栏当前选中的样式
                    $(".m-nav").find("li").removeClass("current");
                    //加上一个属性，表示其实因为滚动才隐藏的
                    that.$div.data(scrollToHide, false);
                    //
                    that.hideCallback(event);
                });
                //隐藏函数

                //给body注册事件，点击其他地方的时候隐藏所有的div，
                $("." + tipTopDivClass).off("click.tip").on("click.tip", function (event) {
                    //如果提供了事件对象，则这是一个非IE浏览器 
                    if (event && event.stopPropagation)
                        //因此它支持W3C的stopPropagation()方法 
                        event.stopPropagation();
                    else {
                        //否则，我们需要使用IE的方式来取消事件冒泡 
                        window.event.cancelBubble = true;
                    }
                });
                //给body注册事件，点击其他地方的时候隐藏所有的div，
                $(top).off("click.tip").on("click.tip", function (event) {
                    //
                    $("." + tipTopDivClass).hide();
                    //
                    that.hideCallback(event);
                    //加上一个属性，表示其实因为滚动才隐藏的
                    that.$div.data(scrollToHide, false);
                });
                //滚动事件
                $(mainScroll).on("scroll.tip", function () {
                    //重新定位
                    that.locate($(this).scrollTop());
                });
                //如果有隐藏的事件定义
                if (hideTrigger) {
                    //什么时候显示
                    $(selector).off(hideTrigger + ".tip").on(hideTrigger + ".tip", function (event) {
                        //
                        $("." + tipTopDivClass).hide();
                        //
                        that.hideCallback(event);
                        //加上一个属性，表示其实因为滚动才隐藏的
                        that.$div.data(scrollToHide, false);
                    });
                }
            };
            //对于一些combobox的点击事件，以及本身div的点击，就不需要隐藏了
            this.checkStopEvent = function (event) {
                var target = $(event.target || event.srcElement);
                if (target && (
                    target.parent("." + tipTopDivClass).length > 0
                    || target.hasClass(tipTopDivClass)
                    || target.parents(".combo-p").length > 0
                    )) {
                    //如果提供了事件对象，则这是一个非IE浏览器 
                    if (event && event.stopPropagation)
                        //因此它支持W3C的stopPropagation()方法 
                        event.stopPropagation();
                    else {
                        //否则，我们需要使用IE的方式来取消事件冒泡 
                        window.event.cancelBubble = true;
                    }

                    return true;
                }
                return false;
            }
            //计算位置
            this.locate = function (scrollTop, trigger, event) {
                //对应topdiv的位置
                var offset = $(selector).offset();
                //
                var left = 0, top = 0, arrowLeft = 0;
                //先定位整体的位置
                left = (offset.left - options.width * 0.686);
                //默认离开10个像素
                top = (offset.top + $(selector).height() + 20);

                var ajust = $(selector).width() / 2;
                //
                arrowLeft = options.width * 0.686 - 10 + ajust;
                //如果是右键点击的，就需要微调
                if (trigger == "contextmenu" && event) {
                    //光标位置
                    var clientX = event ? (event.clientX + document.body.scrollLeft - document.body.clientLeft) : 0;
                    //
                    var clientY = event ? (event.clientY + document.body.scrollTop - document.body.clientTop) : 0;
                    //
                    left = clientX - options.width * 0.686;
                    //
                    top = clientY + 10;
                    //
                    arrowLeft = options.width * 0.686 - 10;
                }
                //left最小为10
                var newLeft = left < 10 ? 10 : left;
                //如果右边有超出
                if ((newLeft + options.width + 10) > $("body").outerWidth()) {
                    //靠右
                    newLeft = $("body").outerWidth() - options.width - 10;
                }
                //
                arrowLeft = arrowLeft + (left - newLeft);
                //调整位置
                that.$div.css({
                    //黄金分割点
                    left: options.left + newLeft + "px",
                    top: options.top + top + "px"
                });

                arrowLeft = arrowLeft > options.width * 0.8 ? options.width * 0.8 : arrowLeft;

                //箭头的位置
                $("." + tipArrowClass + ",." + tipArrowBgClass, that.$div).css({
                    left: options.left + arrowLeft + "px"
                });
                //如果是从滚动过来触发的事件
                if (scrollTop || scrollTop === 0) {
                    //如果看不到主按钮的了，则隐藏tip框
                    if (scrollTop > offset.top && that.$div.is(":visible")) {
                        //
                        that.$div.hide();
                        //加上一个属性，表示其实因为滚动才隐藏的
                        that.$div.data(scrollToHide, true);
                    }
                    else if (that.$div.data(scrollToHide) === true) {
                        //恢复其隐藏
                        that.$div.show();
                    }
                }
            }
        };
        //返回
        return mTip;
    })();
    //扩展到jquery对象
    $.fn.mTip = function (options) {
        //初始化一些数据
        options.top = options.top || 0;
        //
        options.left = options.left || 0;
        //
        return new mTip(this, options).init();
    };
})();