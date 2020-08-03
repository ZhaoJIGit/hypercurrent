var PTBizLogoUpload = {
    init: function () {
        PTBizLogoUpload.initAction();
    },
    initAction: function () {
        $("#fileInput").change(function () {
            PTLogoUploadBase.onFileChange(this);
        });
        $("#aSave").click(function () {
            PTLogoUploadBase.uploadLogo("/PT/PTBiz/UploadPTLogo", function () {
                parent.PTBizList.reload();
                $.mDialog.close();
            });
        });
    }
}
$(document).ready(function () {
    PTBizLogoUpload.init();
});