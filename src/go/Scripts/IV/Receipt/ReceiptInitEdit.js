var ReceiptEdit = {
    init: function () {
        //保存收款单
        $("#aSaveReceipt").click(function () {
            ReceiptEditBase.saveReceipt(function (msg) {
                if (msg && msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                } else {
                    $.mMsg(LangKey.SaveSuccessfully);
                    parent.InitBills && parent.InitBills.reloadData();
                    $.mDialog.close();
                }
            });
        });

        var invoiceId = $("#hidReceiptID").val();
        if (invoiceId) {
            $("#selContactType").combobox("disable");
            $("#selContact").combobox("disable");
        }
        $(".form-invoice-toolbar").hide();
    },
}
//初始化页面
$(document).ready(function () {
    ReceiptEdit.init();
});