/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ConfirmImport = {
    init: function () {
        ConfirmImport.bindAction();
    },
    bindAction: function () {
        $("#aCompleteImport").click(function () {
            ImportBase.confirmImport("/BD/ExpenseItem/CompleteImport", null, function () {
                $.mMsg(HtmlLang.Write(LangModule.BD, "ImportExpenseItemsNotifyMsg", "{0} Expense Items were added.").replace("{0}", $("#hidImportingCount").val()));
            });
        });
    }
}

$(document).ready(function () {
    ConfirmImport.init();
});