/// <reference path="../../../intellisense/JieNor.Megi.Common.0x0009.js" />
var MyReportPublish = {
    init: function () {
        MyReportPublish.initAction();
        MyReportPublish.initForm();
    },
    initAction: function () {
        $("#aSaveAsDraft").click(function () {
            MyReportPublish.SaveReportAsDraft();
            return false;
        });
        $("#aPublish").click(function () {
            MyReportPublish.SaveReportAsPublished();
        });
    },
    SaveReportAsDraft: function () {
        $("body").mFormSubmit({
            url: "/Report/RptManager/SaveReportAsDraft", param: { model: {} }, callback: function (msg) {
                $("#hidReportID").val(msg.ObjectID);
                $.mMsg(LangKey.SaveSuccessfully);
            }
        });
    },
    SaveReportAsPublished: function () {
        $("body").mFormSubmit({
            url: "/Report/RptManager/SaveReportAsPublished", param: { model: {} }, callback: function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mDialog.close(0, "1");
            }
        });
    },
    initForm: function () {
        $("body").mFormGet({ url: "/Report/RptManager/GetReportModel" });
    }
}
$(document).ready(function () {
    MyReportPublish.init();
});