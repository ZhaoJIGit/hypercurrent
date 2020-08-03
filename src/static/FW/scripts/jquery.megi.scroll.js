/*
    这个js是为了解决在某个div上面滚动滑轮的时候，div能够进行滚动
*/
(function (w) {

    var mScroll = (function () {

        var mScroll = function (selector, target) {
            //
            var that = this;
            //
            this.init = function () {
                //
                target = target || ($(selector).attr("target") ? $($(selector).attr("target")) : (selector));
                //
                $(selector).off("mousewheel").on("mousewheel", function (event, delta, deltaX, deltaY) {
                    //
                    $(target).scrollTop($(target).scrollTop() - delta * event.deltaFactor);
                });
            }
        };

        return mScroll;
    })();
    //
    w.mScroll = mScroll;
    //
    $.fn.mScroll = function (target) {
        new mScroll(this, target).init();
    }
    //
})(window)

$(function () {
    //
    var selectors = $(".m-scroll-div");

    selectors.each(function () {
        //
        new mScroll(this).init();
    });
})