var PTLogoUploadBase = {
    itemID: $("#hidItemID").val(),
    onFileChange: function (sender) {
        var lbl = $(sender).next();
        ImportBase.onFileChanged(sender);
        var validateResult = FileBase.validateFile(sender.value, 0, FileBase.imgRegex);
        if (validateResult) {
            ImportBase.clearFile(sender);
            $.mDialog.alert(validateResult);
        }
    },
    uploadLogo: function (url, successCallBack) {
        if (!ImportBase.validateFile(HtmlLang.Write(LangModule.Common, "UnSelectLogo", "Please select the Logo you wish to upload."))) {
            return;
        }

        PTLogoUploadBase.bindSubmitAction(PTLogoUploadBase.itemID, url, successCallBack);
        $("#logoUpload").submit();
    },
    bindSubmitAction: function (id, url, successCallBack) {
        $("body").mask("");
        $("form").ajaxForm({
            url: url,
            data: { id: id },
            dataType: ImportBase.isIE9Previous ? null : "json",
            type: "POST",
            success: function (response) {
                if (response && !ImportBase.isIE9Previous && !response.Success) {
                    var msg = typeof (response.VerificationInfor) != 'undefined' ? response.VerificationInfor[0].Message : response.Message;
                    $.mDialog.alert(msg);
                } else {
                    var name = $(".print-tmpl[id='" + id + "']", parent.document).find(".name p").text();
                    $.mMsg(HtmlLang.Write(LangModule.IV, "LogoUploaded", "Your logo for <strong>{0}</strong> was uploaded.").replace("{0}", name), undefined, true);

                    if (successCallBack != undefined) {
                        successCallBack();
                    }

                    $.mDialog.close();
                }
                $("body").unmask();
            },
            fail: function (event, data) {
                $("body").unmask();
                $.mDialog.alert(data);
            }
        });
    }
}