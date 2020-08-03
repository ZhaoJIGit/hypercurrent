/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var BDBankStatementView = {
    init: function (bankID) {
        BDBankStatementView.InitStatus();
        BDBankStatementView.ClickAction(bankID);
    },
    InitStatus: function () {
        $('td[class="mark"]').each(function () {
            switch ($(this).attr("state")) {
                case "1":
                    $(this).addClass("Check");
                    break;
                case "0":
                    $(this).addClass("UnCheck");
                    break;
                case "2":
                    $(this).parent().addClass("Deleted");
                    break;
                default:
                    break;
            }
        });
    },
    //更新statement的状态
    UpdateStatement: function (directType) {
        var array = [];
        //获取参与更新的行
        //var $trs = directType == 1 ? $("tbody tr").not(".Deleted") : $("tbody tr.Deleted");
        var $trs = $("tbody tr");
        var lines = $('td input[type="checkbox"]:checked', $trs);
        //是否有勾对的
        var hasReconcile = false;
        if (lines.length == 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "NoItemsSelected", "No item selected."));
            return;
        }
        for (var i = 0; i < lines.length; i++) {
            array.push("'" + lines.eq(i).val() + "'");
            //
            hasReconcile = hasReconcile || lines.eq(i).parent().parent().find(".Check").length > 0;
        }
        //
        var restoreTitle = HtmlLang.Write(LangModule.Common, "suretorestore", "Are you sure to restore?");
        var deleteWithReconciel = HtmlLang.Write(LangModule.Common, "suretodeletewithreconcile", "Are you sure to delete the records with their reconciled records?");
        var restoreSuccessTitle = HtmlLang.Write(LangModule.Common, "restoresuccessfully", "Restore Successfully !")
        //弹出框内容
        var content = directType == 1 ? (hasReconcile ? deleteWithReconciel : LangKey.AreYouSureToDelete) : restoreTitle;
        //弹出确认
        $.mDialog.confirm(content, {
            callback: function () {
                //
                mAjax.submit('/BD/BDBank/StatementStatusUpdate', { selectIds: array.join(","), directType: directType }, function () {
                    mMsg(directType == 1 ? LangKey.DeleteSuccessfully : restoreSuccessTitle);
                    $.mTab.addOrUpdate();
                });
            }
        });
    },
    ClickAction: function (bankID) {
        $("#tbDelete").click(function () {
            BDBankStatementView.UpdateStatement(1);
        });
        $("#tbRestore").click(function () {
            BDBankStatementView.UpdateStatement(2);
        });
        //编辑账单
        $("#EditBankStatement").off("click").on("click", function () {
            //跳转到账单编辑页面
            var url = "/BD/BDBank/BDBankStatementEdit?MID=" + $("#MID").val() + "&MBanKID=" + $("#MBankID").val();
            //页头
            var title = HtmlLang.Write(LangModule.Bank, "statementedit", "Statement Edit");
            //新增或者更新
            $.mTab.addOrUpdate(title, url, true);
        });
    },
    checkAll: function () {
        if ($("#CheckAll").attr("checked") == "checked") {
            $('input[type="checkbox"]', "tbody").each(function () {
                if ($(this).attr("disabled") != "disabled") {
                    $(this).attr("checked", "checked");
                }
            });
        }
        else {
            $('input[type="checkbox"]', "tbody").each(function () {
                if ($(this).attr("disabled") != "disabled") {
                    $(this).removeAttr("checked", "checked");
                }
            });
        }
    }
}