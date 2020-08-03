var PTSalaryLogoUpload = {
    init: function () {
        PTSalaryLogoUpload.initAction();
    },
    initAction: function () {
        $("#fileInput").change(function () {
            PTLogoUploadBase.onFileChange(this);
        });
        $("#aSave").click(function () {
            PTLogoUploadBase.uploadLogo("/PT/PTSalaryList/UploadPTLogo", function () {
                parent.PTSalaryList.reload();
                $.mDialog.close();
            });
        });
    }
}
$(document).ready(function () {
    PTSalaryLogoUpload.init();
});