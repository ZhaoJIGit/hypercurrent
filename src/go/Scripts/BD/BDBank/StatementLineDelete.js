/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var StatementLineDelete = {
    init: function () {
        StatementLineDelete.saveClick();
        StatementLineDelete.cancelClick();
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var selectIds = $("#selectIds").val();
            mAjax.submit(
                "/BD/BDBank/StatementStatusUpdate", 
                { selectIds: selectIds, directType: 1 }, 
                function (jsonData) {
                    if (jsonData >= 0) {
                        Megi.closeDialog();
                        parent.BankStatementView.reloadViewData();
                    } else {
                        $.mDialog.alert(jsonData.Message);
                    }
                });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            Megi.closeDialog();
            parent.BankStatementView.reloadViewData();
            return false;
        });
    }

}


$(document).ready(function () {
    StatementLineDelete.init();
});