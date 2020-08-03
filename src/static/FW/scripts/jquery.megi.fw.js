/// <reference path="jquery.megi.common.js" />
/// <reference path="jquery-1.8.2.min.js" />

var clearMsgTimeoutId = null;
var isTopWindowInited = false;
var FW = {
    //获取窗体宽度
    getWinWidth: function () {
        return $(".m-wrapper").outerWidth();
    },
    //获取窗体宽度
    getWinHeight: function () {
        return $(window).outerHeight();
    },
    //获取左边宽度
    getLeftWidth: function () {
        if ($(".m-nav").is(":hidden")) {
            return 0;
        } else {
            return $(".m-nav").outerWidth();
        }
    },
    //获取主体宽度
    getMainWidth: function () {
        var w = FW.getWinWidth() - FW.getLeftWidth() - 1;
        return w;
    },
    //获取主体高度
    getMainHeight: function () {
        var h = FW.getWinHeight() - $(".m-header").height();
        if (h < 320) {
            return 320;
        }
        return h;
    },
    //左边菜单
    initLeft: function () {
        var w = FW.getLeftWidth();
        var h = FW.getMainHeight();
        $(".m-nav").css({ "height": h + "px" });
        $(".m-nav>ul>li>.m-sub-nav-container").css({ "height": (h - 20) + "px", "left": w + "px" });

        hideBtnH = $(".m-nav>.bottom").height();
        var liCount = $(".m-nav>.top>li").length;
        var mTop = h - liCount * 55 - hideBtnH;
        if (mTop < 0) {
            mTop = 0;
        }
        $(".m-nav>.bottom").css("margin-top", mTop + "px");
        $(".m-sub-nav-container").hide();
    },
    //左边菜单事件
    initLeftEvent: function () {
        $(".m-nav-shrink").unbind().click(function () {
            $(".m-nav").hide();
            $(".m-menu-show>.btn-show").css("display", "block");
            FW.resize();
        });
    },
    //主体
    initMain: function () {
        var w = FW.getMainWidth();
        var h = FW.getMainHeight();
        $(".m-main").css({ "width": w + "px" });
        $(".m-main").css({ "height": h + "px" });
        $(".m-main").find("iframe").attr({ "width": w + "px", "height": h + "px" });
    },
    initInnerMain: function () {
        var h = FW.getMainHeight();
        $(".m-imain").css({ "height": h + "px" });
    },
    //悬浮菜单
    initShrinkMenu: function () {
        $(".m-nav-pop>.nav-main>.item>.item-link").mouseover(function () {
            $(".m-nav-pop>.nav-main>li").removeClass("current").removeClass("arrow");
            $(".n-sub-nav-pop").hide();
            var module = $(this).attr("data-module");
            $(this).closest("li").addClass("current");
            if (module != undefined && module.length > 0) {
                $(this).closest("li").addClass("arrow");
                var curObj = $(".n-sub-nav-pop[data-module='" + module + "']");
                $(curObj).show();
                var curObjH = $(curObj).height();
                var popH = $(".m-nav-pop").height();
                if (curObjH < popH) {
                    curObjH = popH;
                    $(curObj).css("height", curObjH + "px");
                }
            }
        });
    },
    //悬浮菜单事件
    initShrinkMenuEvent: function () {
        $(".m-menu-show>.btn-show").unbind().mouseover(function () {
            $(".m-nav-pop").show(300);
        }).mouseout(function () {
            $(".m-nav-pop").hide();
        });
        $(".m-nav-pop").unbind().mouseover(function () {
            $(".m-nav-pop").show();
        }).mouseout(function () {
            $(".m-nav-pop").hide();
        });
    },
    resize: function () {
        FW.initLeft();
        FW.initMain();
    },
    addTab: function (tab, content) {
        FW.resize();
        var h = FW.getMainHeight();
        content.find("iframe").attr("height", h).addClass("m-page-loading");
        $(".m-tab-loading", tab).addClass("m-img-loading");
    },
    initFW: function () {
        FW.resize();
        FW.initInnerMain();
        FW.initLeftEvent();
        FW.initShrinkMenu();
        FW.initShrinkMenuEvent();
        FW.initMenu();
        isTopWindowInited = true;
    },
    msgEvent: function () {
        if (clearMsgTimeoutId != null) {
            clearTimeout(clearMsgTimeoutId);
        }
        clearMsgTimeoutId = setTimeout(FW.removeMsg, 15000);
    },
    initMenu: function () {
        //current样式
        var current = "current";
        //container样式
        var container = "m-sub-nav-container";
        //iframe遮罩样式
        var iframeTopBg = "iframe-top-bg";
        //
        var $bg = $("." + iframeTopBg);
        //
        var showBg = function () {

            //
            var $main = $(".m-main");
            //显示并且调节样式
            $bg.show().css({
                width: $main.outerWidth(),
                height: $main.outerHeight(),
                left: $main.offset().left,
                top: $main.offset().top
            });
        };
        //隐藏
        var hideBg = function () {
            //隐藏
            $bg.hide();
        };
        //初始化事件
        var initEvent = function () {
            $(document).off("click.menu").on("click.menu", function () {
                $(".m-nav ul li." + current).trigger("click");
            });
        };
        //显示一层
        $(".m-nav>ul>li").off("click").on("click", function () {
            //
            var that = this;
            //
            var $container = $("." + container, $(that));
            //如果已经打开
            if ($container.is(":visible")) {
                //则将其关闭
                $container.hide();
                //去掉current样式
                $(that).removeClass(current);
                //
                hideBg();
            }
            else {
                showBg();
                initEvent();
                //兄弟节点去掉current样式
                $(that).addClass(current);
                $(".m-nav>ul>li").not(that).each(function () {
                    //去掉current样式
                    $(this).removeClass(current);
                    //
                    $("." + container, $(this)).hide();

                });
                //
                $("." + container, $(that)).show();

                var subCurrentTop = 13;

                var left = $(this).offset().left;
                var w = $(this).width();
                var h = $(window).height() - 75;
                if ($container.length > 0) {
                    var subNavHeight = $container.find("p,tr").length * 32;
                    //排除最底部（系统设置）菜单 Features 的高度
                    subNavHeight = subNavHeight - $container.find("p[myattr='right']").length * 32;

                    if ($(".arrow-content-cell-title", $container).is(":visible")) {
                        subNavHeight += 28;
                    }
                    var subNavTop = $(that).offset().top;
                    if ($(that).offset().top + subNavHeight > $(window).height()) {
                        subNavTop = $(that).offset().top - subNavHeight + $(that).height();
                        subCurrentTop = subCurrentTop + subNavHeight - $(that).height();
                    }
                    $container.find(".arrow-left,.arrow-left-border").css("margin-top", (subCurrentTop) + "px");
                    $container.css({ "left": (left + w) + "px", top: subNavTop, height: subNavHeight }).show();
                    return false;
                }

            }

        });


        document.onkeydown = function (e) {
            if (window.event && window.event.keyCode == 13) {
                //为了防止新建tab的快捷菜单位置失效，屏蔽掉回车键 chenpan
                var dom = e.srcElement;
                if ($(dom).attr("id") == "aNewTab") {
                    window.event.returnValue = false;
                }
            }
        }

        $("#aUser").click(function () {
            Megi.popup("#aUser", {
                selector: "#divUserInfo", paddingBottom: 10, position: "right-bottom", showCallBack: function () { $("#aUser").addClass("current"); }, hideCallBack: function () { $("#aUser").removeClass("current"); }
            });
        });

        $("#aNewTab").click(function (e) {
            //MegiTab.resetLiStyle();
            //MegiTab.show($(this));
            Megi.popup("#aNewTab", {
                selector: "#divNewTabList", event: e
            });
        });

        $("#divNewTabList a").click(function () {
            $('#divNewTabList').hide();
        });

        $("#aOrgList").click(function () {
            Megi.popup("#aOrgList", {
                selector: "#divOrgList", paddingBottom: 5, left: -20
            });
        });

    }
}
$(document).ready(function () {
    FW.initFW();
    $(window).resize(function () {
        FW.resize();
        setTimeout(FW.resize, 300)
    });
});
