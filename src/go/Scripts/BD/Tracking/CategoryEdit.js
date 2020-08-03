var cateEdit = {
    tabIndex: $("#hideTabIndex").val(),
    init: function () {
        cateEdit.okClick();
        cateEdit.cancelClick();
        cateEdit.getTrackCategory();
    },
    okClick: function () {
        $("#aSave").click(function () {
            if (!$(".m-imain").mFormValidate()) {
                return;
            }
            var toUrl = $(this).attr("href");
            var newTabTitle = $('input[name="MName"]').val();

            var dataLang = $("#tbxMName").getLangEditorData();
            var langArray = new Array();
            langArray.push(dataLang);

            var obj = {};
            obj.MItemID = $("#itemId").val();
            obj.MultiLanguage = langArray;

            var result = $("body").mFormValidate();

            if (!result) return;

            mAjax.submit("/BD/Tracking/TrackingUpd", { info: obj }, function (msg) {
                if (msg && msg.Success) {
                    parent.Tracking.reload(cateEdit.tabIndex);
                    $.mMsg(HtmlLang.Write(LangModule.BD, "TrackAddSuccess", "Operation Successfully"));
                    Megi.closeDialog();

                } else {
                    $.mDialog.alert(msg.Message);
                }
            });

            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            var toUrl = $(this).attr("href");
            var selTabTitle = $("#curTitle").val();
            Megi.closeDialog();
            parent.Tracking.reload(selTabTitle);
            return false;
        });
    },
    getTrackCategory: function () {
        //id不为空表示编辑
        var trackId = $("#itemId").val();

        if (trackId) {
            $.mAjax.Post("/BD/Tracking/GetTrackById", { trackId: trackId }, function (msg) {
                if (msg != null) {
                    $("#tbxMName").val(msg.MName);
                    var dataLang = msg.MultiLanguage;
                    $("#tbxMName").initLangEditor(dataLang[0]);
                } else {
                    $.mAlert(msg.Message);
                }
            }, "", true);
        }
    }
}


$(document).ready(function () {
    cateEdit.init();
});