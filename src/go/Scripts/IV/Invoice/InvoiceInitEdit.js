/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
/// <reference path="InvoiceEditBase.js" />
var InvoiceEdit = {
    init: function () {
        InvoiceEditBase.IsInit = true;
        //保存初始化各种单据
        $("#btnSave").click(function () {
            InvoiceEditBase.saveInvoiceAndApprove(function (msg) {
                if (msg && !msg.Success) {
                    $.mDialog.alert(msg.Message);
                } else {
                    $.mMsg(LangKey.SaveSuccessfully);
                    parent.InitBills && parent.InitBills.reloadData();
                    $.mDialog.close();
                }
            
            });
        });
        $("#btnSaveAndAddAnother").click(function () {
            InvoiceEditBase.saveInvoiceAndApprove(function () {
                $.mMsg(LangKey.SaveSuccessfully);
                parent.InitBills && parent.InitBills.reloadData();
            });
        });

        var invoiceId = $("#hidInvoiceID").val();
        if (invoiceId) {
            $("#selContact").combobox("disable");
        }
    }
}

$(document).ready(function () {
    InvoiceEdit.init();
});