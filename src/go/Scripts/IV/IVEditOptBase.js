var IVEditOptBase = {
    init: function (options) {

        $("#btnSave").click(function () {
            if (options.save != null) {
                options.save();
            }
        });

        $("#btnSaveAsDraft").click(function () {
            if (options.saveAsDraft != null) {
                options.saveAsDraft();
            }
        });
        $("#btnSaveAndContinue").click(function () {
            if (options.saveAndContinue != null) {
                options.saveAndContinue();
            }
        });
        $("#btnSaveAndSubmitForApproval").click(function () {
            if (options.saveAndSubmitForApproval != null) {
                options.saveAndSubmitForApproval();
            }
        });
        $("#btnSaveAndAddAnother").click(function () {
            if (options.saveAndAddAnother != null) {
                options.saveAndAddAnother();
            }
        });

        $("#aApproveInvoice").click(function () {
            if (options.approve != null) {
                options.approve();
            }
        });
        $("#aApproveAndAddAnother").click(function () {
            if (options.approveAndAddAnother != null) {
                options.approveAndAddAnother();
            }
        });
        $("#aApproveAndViewNext").click(function () {
            if (options.approveAndViewNext != null) {
                options.approveAndViewNext();
            }
        });
        $("#aApproveAndPrint").click(function () {
            if (options.approveAndPrint != null) {
                options.approveAndPrint();
            }
        });
    }

}