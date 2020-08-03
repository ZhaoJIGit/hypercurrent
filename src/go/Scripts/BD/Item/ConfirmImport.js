/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ConfirmImport = {
    init: function () {
        ConfirmImport.bindAction();
    },
    bindAction: function () {
        $("#aCompleteImport").click(function () {
            ImportBase.confirmImport("/BD/Item/CompleteImport", null, function () {
                $.mMsg(HtmlLang.Write(LangModule.Acct, "ImportedInventoryMsg", "{0} inventory items were added.").replace("{0}", $("#hidImportingCount").val()));
            });
        });

        $("#aGoBack").click(function () {
            window.location.href = "/BD/Item/Import";
        });
    }
}

$(document).ready(function () {
    ConfirmImport.init();
});