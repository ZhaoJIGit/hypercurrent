/// <reference path="jquery-1.8.2.min.js" />
(function ($) {
    var curSrcSelector;
    var curHideCallBack;
    $.fn.extend({
        /* initTabIndex:0,
         * setTabIndex:0,
         * onSelect: function(tabIndex){}
         */
        tabsExtend: function (options) {
            var tabLinks = $(this).find("li");
            tabLinks.click(function () {
                tabLinks.removeClass("current");
                $(this).closest("li").addClass("current");
                var tabContents = $(".m-extend-tabs-panel .tab-content");
                var tabIndex = tabLinks.index($(this));

                var url = location.href;
                if (url.lastIndexOf('#') != -1) {
                    url = url.split('#')[0];
                }
                //location.href = url + '#' + tabIndex;

                if (tabContents.length == tabLinks.length) {
                    tabContents.hide();
                    $(tabContents.get(tabIndex)).show();
                }
                if (options.onSelect != undefined) {
                    options.onSelect(tabIndex);
                }
            });
            tabLinks.mouseover(function () {
                tabLinks.removeClass("over");
                $(this).closest("li").addClass("over");
            }).mouseout(function () {
                $(this).closest("li").removeClass("over");
            });
            $(function () {
                var tabIndex = options.initTabIndex;
                //if (window.location.hash) {
                //    tabIndex = window.location.hash.substring(1);
                //}
                if (options.setTabIndex != undefined) {
                    tabIndex = options.setTabIndex;
                }
                if (tabIndex != undefined) {
                    $(tabLinks.get(tabIndex)).trigger("click");
                }
            });
        },
        popup: function (options) {
            var tgtSelector = options.selector;
            var srcSelector = $(this).selector;
            $(this).attr("srcSelector", srcSelector).attr("tgtSelector", tgtSelector);
            $(tgtSelector).attr("srcSelector", srcSelector).attr("tgtSelector", tgtSelector);

            function getClickCoords(event) {
                var totalOffsetX = 0;
                var totalOffsetY = 0;
                var canvasX = 0;
                var canvasY = 0;
                var currentElement = event.srcElement;
                //var currentElement = this;

                do {
                    totalOffsetX += currentElement.offsetLeft - currentElement.scrollLeft;
                    totalOffsetY += currentElement.offsetTop - currentElement.scrollTop;
                }
                while (currentElement = currentElement.offsetParent)

                canvasX = event.pageX - totalOffsetX;
                canvasY = event.pageY - totalOffsetY;

                return { x: canvasX, y: canvasY };
            }
            function setPosition(srcSelector, tgtSelector) {
                if (srcSelector == undefined || tgtSelector == undefined) {
                    return;
                }
                var paddingBottom = options.paddingBottom == undefined ? 10 : options.paddingBottom;
                var srcOffset = $(srcSelector).offset();
                var srcPosition = $(srcSelector).position();
                //兼容IE浏览器（点击New Tab时，position会变成relative，导致弹出框位置不对）
                //if (srcPosition.top == 0 && srcPosition.left == 0) {
                //if(true){
                //    var po = getClickCoords(options.event);
                //    srcOffset.left = po.x;
                //    srcOffset.top = po.y + 10;
                //}
                var targetTop;
                var targetLeft;
                var srcSelWidth = $(srcSelector).width();
                var srcSelHeight = $(srcSelector).outerHeight();
                if (options.position == undefined) {
                    options.position = "left-bottom";
                }
                if (options.position.indexOf("left") >= 0) {
                    targetLeft = srcOffset.left;
                    $(tgtSelector).find(".popup-arrow").css("left", 20);
                }
                else {
                    targetLeft = srcOffset.left - 11;
                    $(tgtSelector).find(".popup-arrow").css("right", srcSelWidth / 2);
                }
                if (options.position.lastIndexOf('bottom') != -1) {
                    targetTop = srcOffset.top + srcSelHeight + paddingBottom;
                }
                else {
                    targetTop = srcOffset.top - srcSelHeight - $(tgtSelector).outerHeight() - paddingBottom;
                }
                if (options.left != undefined) {
                    targetLeft = targetLeft + options.left;
                }

                //保证弹出框不超出边界
                if ($(tgtSelector).outerWidth() + targetLeft > document.body.offsetWidth - 10) {
                    targetLeft = document.body.offsetWidth - 10 - $(tgtSelector).outerWidth();
                }
                if ($(tgtSelector).outerHeight() + targetTop > document.body.offsetHeight - 10) {
                    targetTop = document.body.offsetHeight - 10 - $(tgtSelector).outerHeight();
                }

                $(tgtSelector).css({ position: "fixed", top: targetTop, left: targetLeft, "z-index": 88888 });
            }
            $(document).off("click.popup").on("click.popup", function (e) {
                var target = e.srcElement || e.target;
                srcSelector = $(target).attr("srcSelector") || $(target).parent().attr("srcSelector");
                tgtSelector = $(target).attr("tgtSelector") || $(target).parent().attr("tgtSelector");
                if (srcSelector == undefined && curSrcSelector != undefined) {
                    srcSelector = $(curSrcSelector).attr("srcSelector");
                    tgtSelector = $(curSrcSelector).attr("tgtSelector");
                }
                var isClickInPopup = target != undefined && (target.id == $(tgtSelector).attr("id") || $(target).parents(tgtSelector).length > 0 || $(target).hasClass("popup-action"));
                var isClickSrcSelector = target != undefined && (target.id == $(srcSelector).attr("id") || $(target).parents(srcSelector).length == 1);
                var isClickInBoxTitle = target != undefined && ($(target).hasClass("boxTitle") || $(target).parents(".boxTitle").length > 0) || $(target).parents("#popup_container").length > 0;
                if (isClickSrcSelector && !$(tgtSelector).is(":visible")) {
                    if (curSrcSelector != undefined) {
                        $($(curSrcSelector).attr("tgtSelector")).hide();
                    }
                    curSrcSelector = srcSelector;
                    curHideCallBack = options.hideCallBack;
                    setPosition(srcSelector, tgtSelector);
                    $(tgtSelector).show("fast", function () {
                        if (options.showCallBack != undefined) {
                            options.showCallBack(srcSelector);
                        }
                    });
                } else if (!isClickInPopup && !isClickSrcSelector && !isClickInBoxTitle) {
                    $($(curSrcSelector).attr("tgtSelector")).hide();
                    if (options.hideCallBack != undefined) {
                        options.hideCallBack();
                    }
                }
            });
            $(window).resize(function () {
                setPosition(srcSelector, tgtSelector);
            });
            if (options.scrollObj && options.scrollObj.length > 0) {
                $(options.scrollObj).scroll(function () {
                    setPosition(srcSelector, tgtSelector);
                });
            }
            $("iframe").mouseover(function (e) {
                if (curSrcSelector != undefined) {
                    $($(curSrcSelector).attr("tgtSelector")).hide();
                    if (curHideCallBack != undefined) {
                        curHideCallBack();
                    }
                }
                if (e && e.stopPropagation) {
                    e.stopPropagation();
                }
                else {
                    window.event.cancelBubble = true;
                }
            });
        },
        grid: function (options) {
            var defaults = { selector: "", position: "left-bottom" };
            var opts = $.extend(defaults, options);
        },
        selectAll: function (e) {

            if ($(this).hasClass("combo-text")) {
                $(this).get(0).setSelectionRange(0, $(this).val().length);
                //e && e.stopPropagation();
            }
            else {
                $(this).select();
            }
        }
    });
})(jQuery);


$(function () {

    new mSelect(document, true).init();

});
