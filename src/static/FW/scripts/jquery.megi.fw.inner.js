/// <reference path="jquery.megi.common.js" />
/// <reference path="jquery-1.8.2.min.js" />
var FWInner = {
    getMainHeight: function () {
        var h = $(window).outerHeight() - $(".m-header").height();
        if (h < 320) {
            return 320;
        }
        return h;
    },
    initFW: function () {
        parent.$("body").find("iframe").removeClass("m-page-loading");
        var dataIndex = $(window.frameElement).parent().attr("data-index") || 0;
        $(".m-tab-item[data-index='" + dataIndex + "']", window.parent.$("body")).attr("locked", "0");
        $(".m-tab-item[data-index='" + dataIndex + "']", window.parent.$("body")).prev().removeClass("m-img-loading");
        if ($(".m-imain").length > 0) {
            var ifH = parent.$("body").find(".mBoxContent").find("iframe").height();


            if (ifH == null || ifH < 0) {
                ifH = parent.$("body").find("iframe").height();
            }
            ifH = (ifH == null || ifH < 0) ? FWInner.getMainHeight() : ifH;
            //针对那种弹出框的样式
            var visualHeight = $(window.parent).height() - ((window.parent.$(".mBoxTitle").length) > 0 ? window.parent.$(".mBoxTitle").outerHeight() : 0);
            //如果超出那个高度
            ifH = ifH > visualHeight ? visualHeight : ifH;
            $(".m-imain").attr("nn", ifH);
            var btnContainerH = 0;
            var toolbarH = 0;
            if ($(".m-toolbar-footer:visible").length > 0) {
                btnContainerH = $(".m-toolbar-footer:visible").outerHeight();
            }
            if ($(".m-ititle,.m-toolbar").length > 0) {
                toolbarH = $(".m-ititle,.m-toolbar").outerHeight();
            }
            var h = ifH - toolbarH - btnContainerH - ($(".m-imain").outerHeight() - $(".m-imain").height());

            $(".m-imain").css({ "height": h + "px" });
        }
        //$(".m-toolbar-footer").show();
    }
}

$(document).ready(function () {
    FWInner.initFW();
    $(window).resize(function () {
        FWInner.initFW();
    });
});