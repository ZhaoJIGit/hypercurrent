if (MakePayment == undefined) {
    var MakePayment = {};
}
MakePayment.open = function (invoiceId, title,callback) {
    var url = "/IV/UC/MakePayment/" + invoiceId;
    $.mDialog.show({
        mTitle: title,
        mDrag: "mBoxTitle",
        mWidth: 500,
        mHeight: 380,
        mShowbg: true,
        mContent: "iframe:" + url,
        mCallback: function () {
            if (callback != undefined) {
                callback();
            }
        }
    });
}