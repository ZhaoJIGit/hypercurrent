//页面初始化的时候加载
(function ($) {
    var minSize = { width: 1280, height: 662 };

    var options = {
        "min-width": minSize.width,
        "min-height": minSize.height,
        "overflow-x": "auto",
        "overflow-y": "auto",
    };



    $("body").css(options);

    $(window).resize(options, function (event) {

        var min = event.data;
        var $dom = $(this);
        //if ($dom.width() < min.width || $dom.height() < min.height) {
        //    //纵向滚动条
        //    if ($dom.height() < min.height) {
        //        options["overflow-y"] = "auto",
        //        options["min-height"] = min.height;
        //    }

        //    //横向滚动条
        //    if ($dom.width() < min.width) {
        //        options["overflow-x"] = "auto",
        //        options["min-width"] = min.width;
        //    }
        //}

        $("body").css(options);
    })
})(jQuery);