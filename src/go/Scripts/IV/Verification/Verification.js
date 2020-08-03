var Verification = {
    init: function () {
        $(".btnCredit").click(function () {
            var url = '/IV/Verification/VerificationEdit/' + $("#hidInvoiceID").val();
            url = url + '?MBizBillType=' + $(this).attr("bizbilltype");
            url = url + '&MBizType=' + $(this).attr("biztype");
            url = url + '&MAmount=' + $(this).attr("amt");
            url = url + '&MSrcBizBillType=' + $("#hidSrcBizBillType").val();
            mWindow.reload(url);
        });
    }
}
$(document).ready(function () {
    Verification.init();
});