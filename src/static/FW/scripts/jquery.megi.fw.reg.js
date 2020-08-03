/// <reference path="jquery-1.8.2.min.js" />
var Reg = {
    init: function () {
        var docW = $("body").width();
        var docH = $("body").height();
        //计算".m-reg-box"的样式
        var $regBox = $(".m-reg-box");
        var t = (docH - $regBox.height() - $(".m-reg-footer").height()) / 2;
        $regBox.css({
            "margin-top": t+ "px"
        });
        var boxH = $regBox.offset().top + $regBox.height();

        if (docH - boxH > $(".m-reg-footer").height()) {
            var marginTop = (docH - boxH - $(".m-reg-footer").height());
            marginTop = marginTop < 0 ? 0 : marginTop;
            $(".m-reg-footer").css({ "margin-top": marginTop + "px" });
        } else {
            $(".m-reg-footer").css({ "margin-top": "20px" });
        }

        //$(".m-reg-footer") 可能跑出了窗口，重新调整一下 chenpan
        if ($regBox.offset().top < 0) {
            $regBox.css({
                "margin-top": "0px"
            });
        }
    }
}
$(document).ready(function () {
    Reg.init();
    $(window).resize(function () {
        Reg.init();
    });
});
