var AccountBalancesEdit = {
    init: function () {
        AccountBalancesEdit.initAction();
    },
    initAction:function(){
        $("#aCancel").click(function () {
            $.mDialog.close();
        });

        $("#aSave").click(function () {
            AccountBalancesEdit.saveBalance();
        });

        $("#tbxInitBalanceFor").keyup(function () {
            var exchange = $("#exchange").val();
            //如果为空，按照一比一处理
            exchange = exchange ? parseFloat(exchange) : 1;

            var money = $(this).val();

            $("#tbxInitBalance").numberbox("setValue", money * exchange);
        });

        $("#sltContact").combobox({
            url: "/BD/Contacts/GetContactList",
            valueField: 'MItemID',
            textField: 'MName',
            onChange: function (newValue, oldValue) {
                //当联系人的值改变,并且有值，这是后就要重新加载联系人的记录
                AccountBalancesEdit.loadContactInitBalance(newValue);
            }
        });
    },
    loadContactInitBalance:function(contactId){
        var url = "/BD/BDAccount/GetContactInitBalance";
        var oj = {};
        oj.contactId = contactId;
        oj.cyId = $("#mcyId").val();
        oj.accountId = $("#accountId").val();

        mAjax.post(
            url, 
            oj,
            function (data) {
                if (data) {
                    $("#tbxInitBalanceFor").numberbox("setValue", data.MInitBalanceFor);
                    $("#tbxInitBalance").numberbox("setValue", data.MInitBalance);
                    $("#tbxAccumulatedCredit").numberbox("setValue", data.MYtdCredit);
                    $("#tbxAccumulatedDebit").numberbox("setValue", data.MYtdDebit);
                } 
            });
    },
    saveBalance: function () {
       
        if (!$("#divEdit").mFormValidate()) {
            return;
        }
        var arr = new Array();
        var obj = {};
        obj.MAccountID = $("#accountId").val();
        obj.MItemID = $("#id").val();
        //0表示综合本分币，没有选用外币，此时所有的科目时不能进行编辑的
        obj.MCurrencyID = $("#mcyId").val() ? $("#mcyId").val() : "0";
        
        var balances = $("#tbxInitBalanceFor").val();
        obj.MInitBalanceFor = $("#tbxInitBalanceFor").numberbox("getValue");
        obj.MInitBalance = $("#tbxInitBalance").numberbox("getValue");
        obj.MYtdCredit = $("#tbxAccumulatedCredit").numberbox("getValue");
        obj.MYtdDebit = $("#tbxAccumulatedDebit").numberbox("getValue");
        var contactId = $("#sltContact").combobox("getValue");
        arr.push(obj);


        mAjax.submit(
            "/BD/BDAccount/UpdateInitBalance?contactId=" + contactId, 
            { modelList: arr }, 
            function (data) {
                if (data.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.Org, "UpdateSuccessfully", "Update successfully."));
                    parent.AccountBalances.bindGrid();
                    $.mDialog.close();
                } else {
                    $.mAlert(data.Message);
                }
            });
    }
}
$(document).ready(function () {
    AccountBalancesEdit.init();
});