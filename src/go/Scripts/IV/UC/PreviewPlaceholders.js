/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var RepeatPreviewPlaceholders = {
    init: function () {
        RepeatPreviewPlaceholders.bindList();
    },
    bindList: function () {
        var billType = $("#hidBillType").val();
        var rppList = null;
        if (billType == "Invoice") {
            rppList = parent.RepeatInvoiceEditBase.PreviewPlaceholderDatas;
        } else if (billType == "Bill") {
            rppList = parent.RepeatBillEditBase.PreviewPlaceholderDatas;
        }
        mAjax.post(
            "/IV/UC/FormatPlaceholders",
            { param: rppList },
            function (data) {
                var preview_placeholder_list = "";
                for (var i = 0; i < data.length; i++) {
                    if (i == 0) {
                        preview_placeholder_list += "\
                                <div class=\"preview-placeholder-list-row\" style=\"padding-bottom:10px;\">\
                                    <div class=\"preview-placeholder-list-row-cell-1\">" + mText.encode(data[i].Title) + "</div>\
                                    <div class=\"preview-placeholder-list-row-cell-2\">" + mText.encode(data[i].Content) + "</div>\
                                </div>";
                    } else {
                        preview_placeholder_list += "\
                                <div class=\"preview-placeholder-list-row\">\
                                    <div class=\"preview-placeholder-list-row-cell-1\">" +  mText.encode(data[i].Title) + "</div>\
                                    <div class=\"preview-placeholder-list-row-cell-2\">" +  mText.encode(data[i].Content) + "</div>\
                                </div>";
                    }
                }
                $(".preview-placeholder-list").html(preview_placeholder_list);
            });
    }
}
//初始化页面
$(document).ready(function () {
    RepeatPreviewPlaceholders.init();
});