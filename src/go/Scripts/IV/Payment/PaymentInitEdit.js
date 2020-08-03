
var PaymentEdit = {
    PaymentID:$("#hidPaymentID").val(),
    init: function () {
        $("#aSavePayment").click(function (msg) {
            PaymentEditBase.savePayment(function (msg) {
                if (msg && msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                } else {
                    $.mMsg(LangKey.SaveSuccessfully);
                    parent.InitBills && parent.InitBills.reloadData();
                    $.mDialog.close();
                }
             
            });
        });

        var invoiceId = $("#hidPaymentID").val();
        if (invoiceId) {
            $("#selContactType").combobox("disable");
            $("#selContact").combobox("disable");
        }
        $(".form-invoice-toolbar").hide();
    }

}
//初始化页面
$(document).ready(function () {
    PaymentEdit.init();
});

