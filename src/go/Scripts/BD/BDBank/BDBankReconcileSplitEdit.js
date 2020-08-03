var BDBankRecSplitEdit = {
    init: function () {
        $("#txtPartPayment").blur(function () {
            BDBankRecSplitEdit.calAmt();
        }).keyup(function () {
            BDBankRecSplitEdit.calAmt();
        });
        $("#aSplit").click(function () {
            var rowIndex = $("#hidRowIndex").val();
            var splitAmtFor = $("#txtPartPayment").numberbox("getValue");
            parent.BDBankReconcileEdit.AfterSplitTran({ SplitAmtFor: splitAmtFor, RowIndex: rowIndex });
            $.mDialog.close();
        });
    },
    calAmt: function () {
        $("#spRemainAmt").html("");
        var totalAmt = Number($("#hidTotalAmtFor").val());
        var splitAmt = $("#txtPartPayment").val();
        var remainAmt = totalAmt - splitAmt;
        $("#spRemainAmt").html(Megi.Math.toMoneyFormat(remainAmt));
    }
}

$(document).ready(function () {
    BDBankRecSplitEdit.init();
});