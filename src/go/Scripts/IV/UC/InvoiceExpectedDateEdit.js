/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var InvoiceExpected = {
    invoiceId: "",
    mtype: "",
    callBack: function () {

    },
    show: function (invoiceId, contactId, mtype, callBack) {
        $("#aContactName").html("").attr("href", "javascript:void(0)");
        $("#spContactPerson").html("");
        $("#spPhone").html("");
        $("#spMobile").html("");
        InvoiceExpected.invoiceId = invoiceId;
        InvoiceExpected.mtype = mtype;
        $(".mg-expected-date").show("slow");
        $("#txtNote").select();
        InvoiceExpected.callBack = callBack;
        mAjax.post(
            "/IV/IVBase/GetContactInfo", 
            { contactId: contactId },
            function (msg) {
                if (msg != null) {
                    $("#aContactName").html(msg.MName).attr("href", "/Contacts/ContactView/" + contactId);
                    $("#spContactPerson").html(msg.MFirstName + " " + msg.MLastName);
                    $("#spPhone").html(msg.MPhone);
                    $("#spMobile").html(msg.MMobile);
                }
            });
    },
    init: function () {
        $("#aSaveInvoiceExpectedDate").click(function () {
            var obj = $(".mg-expected-date").mFormGetForm();
            obj.MID = InvoiceExpected.invoiceId;
            obj.MType = InvoiceExpected.mtype;
            mAjax.submit(
                "/IV/IVBase/UpdateInvoiceExpectedInfo", { model: obj }, function (msg) {
                    $(".mg-expected-date").hide();
                    if (InvoiceExpected.callBack != undefined) {
                        InvoiceExpected.callBack();
                    }
                });
        });
        $("#aCancelEditInvoiceExpectedDate").click(function () {
            $(".mg-expected-date").hide();
        });
    }
}
$(document).ready(function () {
    InvoiceExpected.init();
});