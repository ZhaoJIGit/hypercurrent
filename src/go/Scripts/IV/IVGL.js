if (IVGL == undefined) {
    var IVGL = {};
}
IVGL.open = function (docId, typeId) {
    var url = "/GL/GLVoucher/GLDocVoucherRuleEdit?Type=" + typeId;
    $.mDialog.show({
        mTitle: HtmlLang.Write(LangModule.IV, "VoucherCreateDetails", "Voucher Create Details"),
        mDrag: "mBoxTitle",
        mShowbg: true,
        mContent: "iframe:" + url,
        mPostData: { "MDocIDs": [docId] }
    });

}