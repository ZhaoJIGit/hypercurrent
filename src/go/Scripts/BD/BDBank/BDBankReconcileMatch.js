/// <reference path="../../IV/BillUrlHelper.js" />
/// <reference path="../../IV/BillUrlHelper.js" />
var BDBankReconcileMatch = {
    recMain: ".recmatch-main",
    saveUpateUrl: "/BD/BDBank/UpdateReconcileMatch",
    reconcleUrl: "/BD/BDBank/UpdateBDBankBillReconcile",
    searchUrl: "/BD/BDBank/BDBankReconcileEdit",
    init: function () {
        BDBankReconcileMatch.bindAction();
    },
    bindAction: function () {
        $('#aSaveUpdate').off("click").on("click", BDBankReconcileMatch.saveUpdate);
        $('#aSaveAndRec').off("click").on("click", BDBankReconcileMatch.saveReconcile);
        $('.match-line').off("click").on("click", function () {
            $(this).find("input[name='chooseBill']").attr("checked", "checked");
        });
        $('.chooseOther').off("click").on("click", function () {
            var rectr = $('.rec-line', BDBankReconcileMatch.recMain);
            mWindow.reload(BDBankReconcileMatch.searchUrl + "?acctid=" + rectr.attr("data-MBankBillEntryID") + "&bankId=" + rectr.attr("data-MBankID"));
        });
    },
    saveUpdate: function () {
        mAjax.submit(BDBankReconcileMatch.saveUpateUrl, {
            entryID: $('.rec-line', BDBankReconcileMatch.recMain).attr("data-MBankBillEntryID"),
            matchBillID: $("input[name='chooseBill']:checked", BDBankReconcileMatch.recMain).val()
        }, function (msg) {
            if (msg.Success) {
                mMsg(HtmlLang.Write(LangModule.Common, "SaveSuccessful", "保存成功！"));
                $.mDialog.close(true);
            } else {
                $.mDialog.alert(msg.Message);
            }
        });
    },
    saveReconcile: function () {
        //后台传过去的参数
        var rectr = $('.rec-line', BDBankReconcileMatch.recMain);
        var obj = {};
        obj.MBankBillEntryID = rectr.attr("data-MBankBillEntryID");
        obj.MSpentAmtFor = rectr.attr("data-MSpentAmtFor");
        obj.MBankID = rectr.attr("data-MBankID");
        obj.MReceiveAmtFor = rectr.attr("data-MReceiveAmtFor");

        var matchtr = $("input[name='chooseBill']:checked", BDBankReconcileMatch.recMain).parent().parent();
        var entry = {};
        entry.MTargetBillID = $("input[name='chooseBill']:checked").val();
        entry.MSpentAmtFor = matchtr.attr("data-MSpentAmtFor");
        entry.MReceiveAmtFor = matchtr.attr("data-MReceiveAmtFor");
        entry.MTargetBillType = matchtr.attr("data-MTargetBillType");
        entry.MDate = mDate.format(matchtr.attr("data-MDate"));
        var arr = [entry];
        //美记账单
        obj.RecEntryList = arr;

        mAjax.submit(BDBankReconcileMatch.reconcleUrl, { model: [obj] }, function (msg) {
            if (msg.Success) {
                mMsg(HtmlLang.Write(LangModule.Bank, "Reconciled", "Reconciled"));
                $.mDialog.close(true);
            } else {
                $.mDialog.alert(msg.Message);
            }
        });
    }
}

$(document).ready(function () {
    BDBankReconcileMatch.init();
});