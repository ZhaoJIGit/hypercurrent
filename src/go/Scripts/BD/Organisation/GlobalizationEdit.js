/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var GlobalizationEdit = {
    init: function () {
        GlobalizationEdit.initAction();
        GlobalizationEdit.initForm();
    },
    initAction: function () {
        $("#aUpdate").click(function () {
            GlobalizationEdit.saveOrgGlobalizationDetail();
            return false;
        });
    },
    saveOrgGlobalizationDetail: function (toUrl) {
        $("body").mFormSubmit({
            url: "/BD/Organisation/GlobalizationUpdate", param: { model: {} }, callback: function (msg) {
                if (msg.Success) {
                    if (toUrl == undefined) {
                        var title = HtmlLang.Write(LangModule.Common, "globalizationSaveSuccessfully", "更新成功,页面将会刷新，请确认所有数据已保存，您是否确认刷新页面?")
                        //
                        $.mDialog.confirm(title, function () {
                            //
                            top.window.location = top.mWindow.getOrigin();
                        });

                    } else {
                        mWindow.reload(toUrl);
                    }
                    return;
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    },
    initForm: function () {
        var orgid = $("#setupOrgId").val() == undefined ? "" : $("#setupOrgId").val();
        $("body").mFormGet({
            url: "/BD/Organisation/GetOrgGlobalizationDetail/" + orgid, fill: true, callback: function (data) {
                var result = data;
            }
        });
    }
}
$(document).ready(function () {
    GlobalizationEdit.init();
});