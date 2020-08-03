var SettingFW = {
    init: function () {
        //parent.$("body").find("iframe").removeClass("m-page-loading");
        if ($(".m-imain").length > 0) {
            var ifH = parent.$("body").find("iframe").height();
            ifH = ifH ==null? $(window).height() : ifH;
            $(".m-imain").css({ "height": ifH + "px" });
            var btnContainerH = 0;
            if ($(".setup-content > .m-toolbar-footer").length > 0) {
                btnContainerH = $(".setup-content > .m-toolbar-footer").outerHeight();
            }
            var h = ifH - btnContainerH - ($(".m-imain").outerHeight() - $(".m-imain").height()) - $(".m-imain-title").height() - $(".setup-progress").height();
            $(".setup-body").css({ "height": h + "px" });
        }
    }
}
$(document).ready(function () {
    SettingFW.init();
    $(window).resize(function () {
        SettingFW.init();
    });
});