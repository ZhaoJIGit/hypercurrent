if (Verification == undefined) {
    var Verification = {};
}
Verification.open = function (id, srcBizBillType, callback) {
    var url = '/IV/Verification/VerificationEdit/' + id + "?srcBizBillType=" + srcBizBillType;
    $.mDialog.show({
        mTitle:HtmlLang.Write(LangModule.IV, "Allocate", "Allocate"),
        mDrag: "mBoxTitle",
        mWidth: 900,
        mHeight: 520,
        mShowbg: true,
        mContent: "iframe:" + url,
        mCallback: function () {
            if (callback!=undefined)
            {
                callback();
            }
        }
    });
}