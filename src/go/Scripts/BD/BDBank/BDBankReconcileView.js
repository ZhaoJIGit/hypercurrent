var BDBankReconcileView = {
    init: function () {
        BDBankReconcileView.bindAction();
        BDBankReconcileView.bindBankReconcileList();
    },
    bindAction: function () {
        $(".bankinfo").find(".more-info").click(function () {
            $(".statement-detail").hide();
            var left = $(this).offset().left;
            var top = $(this).offset().top;
            var height = $(this).closest(".bankinfo").find(".statement-detail").height();
            //$(this).closest(".bankinfo").find(".statement-detail").css({ left: left + "px", top: (top + 15) + "px", "z-index": 99999 }).show();
            Megi.popup("#aMore", { selector: "#divMoreContent" });
        });
        $(".statement-detail").find(".m-icon-close").click(function () {
            $(this).closest(".statement-detail").hide();
        });
    },
    bindBankReconcileList: function () {
        var data = eval("(" + $("#hidIVBankBillReconcileEntryModelList").val() + ")");
        Megi.grid("#tbBankBillReconcileList", {
            resizable: true,
            auto: true,
            height: 378,
            scrollY: true,
            data: data,
            columns: [[
                { title: LangKey.Date, field: 'MCreateDate', width: 50, sortable: false, formatter: $.mDate.formatter },
                {
                    title: LangKey.Reference, field: 'MDesc', width: 100, sortable: false },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Spent", "Spent"), align: 'right', field: 'MSpentAmtFor', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                        return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 2);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Received", "Received"), align: 'right', field: 'MReceiveAmtFor', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                        return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 2);
                    }
                }
            ]]
        });
    }
}

$(document).ready(function () {
    BDBankReconcileView.init();
});