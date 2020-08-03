$(document).ready(function () {
    $(".m-feedback-link").css("top", $(window).height() / 2);
    $(window).resize(function () {
        $(".m-feedback-link").css("top", $(window).height() / 2);
    });
});