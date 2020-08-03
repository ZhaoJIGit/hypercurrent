var cateOptEdit = {
    trackName: null,
    trackEntryName: null,
    tabIndex: $("#hideTabIndex").val(),
    init: function () {
        cateOptEdit.okClick();
        cateOptEdit.cancelClick();
        cateOptEdit.getOption();
    },
    okClick: function () {
        $("#aSave").click(function () {
            if (!$(".m-imain").mFormValidate()) {
                return;
            }
            var toUrl = $(this).attr("href");
            var selTabTitle = $("#curTitle").val();

            var dataLang = $("#MEntryName").getLangEditorData();
            var langArray = new Array();
            langArray.push(dataLang);

            var obj = {};
            obj.MEntryID = $("#MEntryID").val();

            //跟踪项的子项
            var optionModels = new Array();
            var optionModel = {};
            optionModel.MItemID = $("#trackId").val();
            optionModel.MEntryID = obj.MEntryID;
            optionModel.MultiLanguage = new Array();

            var lang = $("#MEntryName").getLangEditorData();
            lang.MFieldName = "MName";
            optionModel.MultiLanguage.push(lang);
            if (!obj.MEntryID) {
                optionModel.MIsActive = true;
            }

           

            optionModels.push(optionModel);


            //新增一个option
            if (!obj.MEntryID) {

                var model = {};
                model.MItemID = $("#trackId").val();

                var closeModel = {
                    id: "",
                    text: $("#MEntryName").val(),
                    parentId: $("#trackId").val()
                }

                mAjax.submit(
                    "/BD/Tracking/SaveTrackingInfo", 
                    { model: model, optionsModels: optionModels },
                    function (msg) {
                        if (msg && msg.Success) {
                            if (parent.Tracking) {
                                parent.Tracking.reload(selTabTitle);
                            }

                            closeModel.id = msg.ObjectID;

                            $.mMsg(HtmlLang.Write(LangModule.BD, "TrackAddSuccess", "Operation Successfully"));

                            Megi.closeDialog(closeModel);
                        } else {
                            $.mDialog.alert(msg.Message);
                        }
                    });
                return false;
            }

            obj.MultiLanguage = langArray;
            obj.MItemID = $("#trackId").val();
            $("body").mFormSubmit({
                validate: true,
                param: { track: obj, entryList: optionModels },
                url: "/BD/Tracking/TrackingOptUpd", callback: function (msg) {
                    if (msg && msg.Success) {
                        if (parent.Tracking) {
                            parent.Tracking.reload(cateOptEdit.tabIndex);
                        }

                        $.mMsg(HtmlLang.Write(LangModule.BD, "TrackAddSuccess", "Operation Successfully"));
                        Megi.closeDialog();
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
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
    getOption: function () {
        var optionId = $("#MEntryID").val();
        var trackId = $("#trackId").val();

        var obj = {};
        obj.trackId = trackId;
        obj.optionId = optionId;

        if (optionId && trackId) {
            $.mAjax.post("/BD/Tracking/GetTrackOptionById", obj, function (msg) {
                if (msg) {
                    cateOptEdit.trackName = "";
                    cateOptEdit.trackEntryName = "";
                    $("#MEntryName").val(msg.MEntryName);
                    var dataLang = msg.MultiLanguage;
                    $("#MEntryName").initLangEditor(dataLang[0]);
                } else {
                    $.mAlert(msg.Message);
                }
            }, "", true);
        }
    }
}


$(document).ready(function () {
    cateOptEdit.init();
});