var Coupon = {
    tabswitch: new BrowserTabSwitch(),
    init: function () {
        Coupon.tabswitch.initSessionStorage();
        Coupon.initAction();
    },
    initAction: function () {
        $("#txtCouponCode").keyup(function () {
            Coupon.setCouponStyle();
        }).focus(function () {
            Coupon.setCouponStyle();
        }).blur(function () {
            Coupon.setCouponStyle();
        });
    },
    setCouponStyle: function () {
        var value = $.trim($("#txtCouponCode").val());
        $("#aApplyCoupon").off("click");
        if (value.length > 0) {
            $("#aApplyCoupon").removeClass("easyui-linkbutton-gray").addClass("easyui-linkbutton-yellow");
            $("#aApplyCoupon").click(function () {
                Coupon.ApplyCoupon();
                return false;
            });
        } else {
            $("#aApplyCoupon").removeClass("easyui-linkbutton-yellow").addClass("easyui-linkbutton-gray");
        }
    },
    ApplyCoupon: function () {
        var code = $("#txtCouponCode").val();
        $(".coupon-msg").html("");
        $('.coupon-msg').removeClass("coupon-correct").removeClass("coupon-error");
        mAjax.submit(
            '/SYS/Coupon/ApplyCoupon',
            { code: code },
            function (msg) {
                $(".coupon-msg").html(msg.Message);
                if (msg.Success) {
                    $('.coupon-msg').addClass("coupon-correct");
                } else {
                    $('.coupon-msg').addClass("coupon-error");
                }
            });
    }
}
$(document).ready(function () {
    Coupon.init();
});