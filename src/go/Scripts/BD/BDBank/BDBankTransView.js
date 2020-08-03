/// <reference path="../IVEditOptBase.js" />
/// <reference path="../IVBase.js" 
/// <reference path="../IVEditBase.js" />
/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var BDBankTransactionView = {
    ObjectID: $("#hidObjectID").val(),
    Type: $("#hidTransType").val(),
    init: function () {
        BDBankTransactionView.getModel();
    },
    getModel: function () {
        var url = "/IV/Payment/GetPaymentViewModel";
        if (BDBankTransactionView.Type == "Receipt") {
            url = "/IV/Receipt/GetReceiptViewModel"
        }
        mAjax.post(
            url, 
            { id: BDBankTransactionView.ObjectID },
            function (msg) {
                $("#spPaymentDate").html($.mDate.format(msg.MBizDate));
                $("#spReference").html(msg.MReference);
                $("#spTotal").html(msg.MTaxTotalAmtFor);
                BDBankTransactionView.bindGrid(msg);
            });
    },
    bindGrid: function (msg) {
        var data = BDBankTransactionView.getGridData(msg);
        Megi.grid("#myGrid", {
            columns: [[{ title: LangKey.Contact, field: 'MContactName', width: 100, align: 'center' },
                    { title: LangKey.Inv, field: 'MNumber', width: 100, align: 'center' },
                    { title: LangKey.Date, field: 'MCreateDate', width: 100, formatter: $.mDate.formatter },
                    { title: LangKey.DueDate, field: 'MDueDate', width: 100, formatter: $.mDate.formatter },
                    { title: LangKey.Total, field: 'MVerificationAmt', width: 100 },
                    { title: LangKey.PaymentAmount, field: 'MTaxTotalAmtFor', width: 100 }]],
            resizable: true,
            auto: true,
            data:data
        });
    },
    getGridData: function (msg) {
        var arr = new Array();
        var obj = {};
        obj.MContactName = msg.MContactName;
        obj.MNumber = msg.MNumber;
        obj.MCreateDate = msg.MCreateDate;
        obj.MDueDate = msg.MCreateDate;
        obj.MVerificationAmt = msg.MVerificationAmt;
        obj.MTaxTotalAmtFor = msg.MTaxTotalAmtFor;
        arr.push(obj);
        return arr;
    }
}

$(document).ready(function () {

    BDBankTransactionView.init();
});