/// <reference path="UCReport.js" />
var Report = {
    init: function () {
        $("#aPopInOut").click(function () {
            if ($(this).hasClass("pop-out")) {
                $(this).removeClass("pop-out").addClass("pop-in");
                $(".mg-pop-wrapper-flag").removeClass("mg-wrapper").addClass("mg-pop-wrapper");
                $(this).html("Standard view");
            } else {
                $(this).removeClass("pop-in").addClass("pop-out");
                $(".mg-pop-wrapper-flag").removeClass("mg-pop-wrapper").addClass("mg-wrapper");
                $(this).html("Wide view");
            }
        });
    }
}
$(document).ready(function () {
    Report.init();
});