var ProfileEdit = {
    tabswitch: new BrowserTabSwitch(),
    init: function () {
        ProfileEdit.tabswitch.initSessionStorage();
        ProfileEdit.initAction();
        ProfileEdit.initHeaderImage();
    },
    initAction: function () {
        $("#aUpdate").click(function () {
            ProfileEdit.submitData();
        });

        $("#btnUploadImage").change(function () {
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



    },
    submitData: function () {

        if (!$("#txtFirstName").mFormValidate() || !$("#txtLastName").mFormValidate()) {
            return;
        }
        var lModel = {};
        lModel.MPKID = $("#hideMPKID").val();
        lModel.MParentID = $("#hideMParentID").val();
        if ($("#txtFirstName").val() == "" ) {
            $("#txtFirstName").addClass("validatebox-invalid");
            return;
        }
        if ($("#txtLastName").val() == "") {
            $("#txtLastName").addClass("validatebox-invalid");
            return;
        }
        lModel.MFristName = $("#txtFirstName").val();
        lModel.MLastName = $("#txtLastName").val();
        lModel.MJobTitle = $("#txtMJobTitle").val();
        lModel.MBriefBio = $("#txtBriefBio").val();
        lModel.ItemId = $("#hideMParentID").val();
        lModel.MProfileImage = $("#hideImageID").val();
        lModel.MMobilePhone = $("#txtPhoneNumber").val();
        //model.SECUserLModel = lModel;
        mAjax.submit("/Profile/UploadHeaderImage",
            {
                lModel: lModel
            },
            function (response) {
                //IE无法解析，直接提示正确

                if (response.Success) {

                    //更改小图标签信息
                    //更新top上的信息
                    //图片
                    //var headImageUrl = $("#hideImageUrl", window.parent.document).val() + "?r=" + Math.random();
                    //$("#divUserInfo", window.parent.document).find("#imageHeaderImage").attr("src", headImageUrl);
                    //更新名称
                    var url = $("#aMySite", window.parent.document).val() + "/FW/FWHome/GetUserName";
                    mAjax.post(url, "", function (msg) {
                        $("#divUserInfo", window.parent.document).find("#lblUserName").text(msg);
                        $.mMsg(HtmlLang.Write(LangModule.Org, "ProfileEditSuccess", "Profile save successfully"));
                        mWindow.reload();
                        //window.location.reload(true);
                    }, "", true, false);

                } else {
                    $.mDialog.alert(response.Message);
                }
            }, "", true, false);
    },
    initHeaderImage: function () {
        $("#imageHeader").attr("src", $("#hideDefaultImageUrl").val());

    }
};
$(document).ready(function () {
    ProfileEdit.init();
});