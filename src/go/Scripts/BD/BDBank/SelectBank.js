var SelectBank = {
    type: $("#hidType").val(),
    init: function () {
        SelectBank.initAction();
    },
    initAction: function () {
        $("#aSave").click(function () {
            if (!$("#divBanks").mFormValidate()) {
                return;
            }

            if (parent.Index) {
                parent.Index.newTransactionTab(SelectBank.type, $("#selBank").combobox("getValue"));
            }
            $.mDialog.close();
        });
    }
}

$(document).ready(function () {
    SelectBank.init();
});