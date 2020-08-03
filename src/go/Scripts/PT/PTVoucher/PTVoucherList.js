var PTVoucherList = {
    init: function () {
        if ($("#hidCanEditVoucher").val() === "True") {
            PTBase.initSort("divVoucher", "/PT/PTVoucher/SortPT");
        }
        PTVoucherList.initAction();
    },
    reload: function () {
        PTBase.reload(PTTabType.Voucher);
    },
    initAction: function () {
        $("#divVoucher .edit").off("click").on("click", function () {
            PTVoucherList.openDialog(this, HtmlLang.Write(LangModule.Common, "EditVoucherPrintTemplate", "编辑凭证打印模板"), "/PT/PTVoucher/PTVoucherEdit", 900, 420);
        });
        $("#divVoucher .delete").off("click").on("click", function () {
            PTVoucherList.deleteSetting(this);
        });
    },
    getEditId: function (sender) {
        var id = $(sender).closest(".print-tmpl").attr("id");
        if (!id) {
            var idx = parseInt($(sender).parent().attr("id").replace("divVoucherFolderOptions", ""));
            id = $("#divVoucherOptions" + idx).closest(".print-tmpl").attr("id");
        }
        return id;
    },
    openDialog: function (sender, title, url, w, h, isNew) {
        if (!isNew) {
            url = url + '/' + PTVoucherList.getEditId(sender)
        }
        Megi.dialog({
            title: title,
            width: w,
            height: h,
            href: url
        });
    },
    deleteSetting: function (sender) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete,
        {
            callback: function () {
                var id = PTVoucherList.getEditId(sender);
                mAjax.submit(
                    "/PT/PTVoucher/DeletePT",
                    { id: id },
                    function (msg) {
                        if (msg.Success) {
                            $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                            PTBase.reload(PTTabType.Voucher);
                            //$(".print-tmpl[id='" + id + "']").remove();
                        } else {
                            $.mDialog.alert(msg.Message);
                        }
                    });
            }
        });
    }
}
$(document).ready(function () {
    PTVoucherList.init();
});