var AddAccountingProject = {
    init: function () {
        AddAccountingProject.initAction();
    },
    initAction: function () {
        $("#aCancel").click(function () {
            $.mDialog.close();
        });

        $("#aSave").click(function () {
            AddAccountingProject.saveBalance();
        });

        $("#tbxInitBalanceFor").keyup(function () {
            var exchange = $("#exchange").val();
            //如果为空，按照一比一处理
            exchange = exchange ? parseFloat(exchange) : 1;

            var money = $(this).val();

            $("#tbxInitBalance").numberbox("setValue", money * exchange);
        });

        $("#sltContact").mAddCombobox("contact", {
            url: "/BD/Contacts/GetContactList",
            valueField: 'MItemID',
            textField: 'MName',
            panelHeight: 140,
            onChange: function (newValue, oldValue) {
                //当联系人的值改变,并且有值，这是后就要重新加载联系人的记录
                AddAccountingProject.loadContactInitBalance(newValue);
            }
        },
            {
                url: "/BD/Contacts/ContactsEdit?contactType",
                //是否有联系人编辑权限
                hasPermission: true,
                //弹出框关闭后的回调函数
                callback: null
            });
    },
    loadContactInitBalance: function (contactId) {
        $("#itemId").val();
        var url = "/BD/BDAccount/GetContactInitBalance";
        var oj = {};
        oj.contactId = contactId;
        oj.cyId = $("#mcyId").val();
        oj.accountId = $("#accountId").val();

        mAjax.post(url, oj, function (data) {
            if (data) {
                $("#tbxInitBalanceFor").val(data.MInitBalanceFor);
                $("#tbxInitBalance").val(data.MInitBalance);
                $("#tbxAccumulatedCredit").val(data.MYtdCredit);
                $("#tbxAccumulatedDebit").val(data.MYtdDebit);

                $("#itemId").val(data.MItemID);
            }
        });
    },
    saveBalance: function () {

        var contact = $("#sltContact").combobox("getValue");
        if (!contact) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "ContactIsEmpty", "Please select a contact!"))
            return;
        }

        if ($("#itemId").val()) {
            var msg = HtmlLang.Write(LangModule.Acct, "ContactIsExist", "This account already contains this contact!");
            $.mDialog.alert(msg);
            return;
        }

        //var arr = new Array();
        var obj = {};
        obj.MAccountID = $("#accountId").val();
        obj.MItemID = $("#id").val();
        //0表示综合本分币，没有选用外币，此时所有的科目时不能进行编辑的
        obj.MCurrencyID = $("#mcyId").val() ? $("#mcyId").val() : "0";

        obj.MInitBalanceFor = $("#tbxInitBalanceFor").val();
        obj.MInitBalance = $("#tbxInitBalance").val();
        obj.MYtdCredit = $("#tbxAccumulatedCredit").val();
        obj.MYtdDebit = $("#tbxAccumulatedDebit").val();
        obj.MContactID = $("#sltContact").combobox("getValue");
        var contactId = $("#sltContact").combobox("getValue");

        mAjax.submit("/BD/BDAccount/UpdateAccountingProject", obj, function (data) {
            if (data.Success) {
                $.mMsg(HtmlLang.Write(LangModule.Acct, "AddSuccessfully", "Add successfully."));
                parent.AccountBalances.bindGrid();
                $.mDialog.close();
            } else {
                $.mAlert(data.Message);
            }
        });
    }
}

$(document).ready(function () {
    AddAccountingProject.init();
});