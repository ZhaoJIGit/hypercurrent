var pageError = {
    init: function () {
        pageError.initAction();
    },
    initAction: function () {
        //反馈按钮
        $("#btnFeedBack").click(function () {
            var url = $("#hideFeedBackUrl").val();
            $.mDialog.show({ title: '我要反馈', width: 650, height: 450, href: url });
        });

        $("#btnRefresh").click(function () {
            var refreshUrl = $("#hideRefreshUrl").val();
            var referer = $("#hideReferer").val();
            var iframe = $(parent.document).find("iframe[src='" + refreshUrl + "']");
            
            
            if (iframe && iframe.length > 0) {
                //var fullUrl = referer + refreshUrl + "?" + Math.random();
                iframe.attr("src", refreshUrl);
            } else {
                window.document.location.href = referer + refreshUrl;
            }
        })
    }

}

$(document).ready(function () {
    pageError.init();
});