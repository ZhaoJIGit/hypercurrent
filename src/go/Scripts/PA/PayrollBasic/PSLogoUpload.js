﻿var PSLogoUpload = {
    itemID: $("#hidItemID").val(),
    init: function () {
        PSLogoUpload.initAction();
    },
    initAction: function () {
        $("#fileInput").change(function () {
            var lbl = $(this).next();
            ImportBase.onFileChanged(this);
            var validateResult = FileBase.validateFile(this.value, 0, FileBase.imgRegex);
            if (validateResult) {
                //For IE
                $(this).replaceWith($(this).clone(true));
                //For other browsers
                $(this).val("");
                ImportBase.restoreDefaultText(lbl);
                $.mDialog.alert(validateResult);
            }
        });
        $("#aSave").click(function () {
            if (!ImportBase.validateFile(HtmlLang.Write(LangModule.Common, "UnSelectLogo", "Please select the Logo you wish to upload."))) {
                return;
            }
            PSLogoUpload.save();
        });
    },
    save: function () {
        var obj = {};
        obj.id = PSLogoUpload.itemID;
        PSLogoUpload.bindSubmitAction(obj);
        $("#logoUpload").submit();
    },
    bindSubmitAction: function (obj) {
        $("body").mask("");
        $("form").ajaxForm({
            url: "/PA/PayrollBasic/UploadPrintSettingLogo",
            data: obj,
            dataType: ImportBase.isIE9Previous ? null : "json",
            type: "POST",
            success: function (response) {
                if (response && !ImportBase.isIE9Previous && !response.Success) {
                    var msg = typeof (response.VerificationInfor) != 'undefined' ? response.VerificationInfor[0].Message : response.Message;
                    $.mDialog.alert(msg);
                } else {
                    var name = $(".print-tmpl[id='" + PSLogoUpload.itemID + "']", parent.document).find(".name p").text();
                    $.mMsg(HtmlLang.Write(LangModule.IV, "LogoUploaded", "Your logo for <strong>{0}</strong> was uploaded.").replace("{0}", name), undefined, true);
                    parent.PSList.reload();
                    $.mDialog.close();
                }
                $("body").unmask();
            },
            fail: function (event, data) {
                $("body").unmask();
                $.mDialog.alert(data);
            }
        });
    },
}
$(document).ready(function () {
    PSLogoUpload.init();
});