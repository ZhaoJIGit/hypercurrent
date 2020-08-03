/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ExpensesMerge = {
    payFrom: $("#payfrom").val(),
    init: function () {
        var ids = $("#expids").val();
        if (ids.length == 0) {
            return;
        }
        var idArray = ids.split(',');

        //支付账号默认第一个选中
        $("#selPaidTo").combobox('select', $('#selPaidTo').combobox('getData')[0].id);

        //保存
        $("#aSave").click(function () {
            //验证支付信息
            var result = $(".m-form").mFormValidate();
            if (!result) {
                //验证不通过，则终止该操作
                return;
            }
            //费用报销单总金额
            var totalAmt = parseFloat($("#totamount").val());
            //当前要支付的金额
            var paymentAmt = parseFloat($('#txtAmountPaid').numberbox('getValue'));
            //支付金额必须大于0
            if (!paymentAmt || paymentAmt <= 0) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "PaymentAmountNotToZero", "Payment amount must be greater than zero."));
                return;
            }
            //支付金额不能大于费用报销单总金额
            if (paymentAmt > totalAmt) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "CannotBeGreaterThanTheTotalAmountPaid", "Payment amount cannot be greater than the total amount of reimbursement list."));
                return;
            }

            var bankId = $('#selPaidTo').combobox('getValue');
            var paidAmt = $('#txtAmountPaid').numberbox('getValue');
            var paidDate = $('#txtPaidDate').datebox('getValue');
            var ref = $("#txtPaymentRef").val();

            var arr = new Array();
            for (var i = 0; i < idArray.length; i++) {
                var obj = {};
                obj.MObjectID = idArray[i];
                obj.MPaidAmount = paidAmt;
                obj.MBankID = bankId;
                obj.MPaidDate = paidDate;
                obj.MRef = ref;
                arr.push(obj);
            }

            //MPaidAmount获取的参数错误.
            mAjax.submit(
                "/IV/Expense/AddExpensePayment", 
                { modelList: arr }, function (msg) {
                    if (msg.Success == false) {
                        $.mDialog.alert(msg.Message);
                        return;
                    }
                    //提示信息
                    //$.mMsg(HtmlLang.Write(LangModule.IV, "PaymentSuccessfully", "Payment Successfully!"));
                    $.mMsg(HtmlLang.Write(LangModule.IV, "Paymentsuccess", "付款成功!"));
                    if (ExpensesMerge.payFrom == "listpage") {
                        //如果是从列表页面支付的，支付成功后刷新列表
                        parent.ExpenseList.reloadData();
                    } else {
                        $.mTab.rename(HtmlLang.Write(LangModule.IV, "ViewExpense", "View Expense"));
                        //如果是从编辑页面支付的，支付成功后跳转至查看页面
                        parent.mWindow.reload("/IV/Expense/ExpenseView/" + ids);
                    }
                    Megi.closeDialog();
                });
        });

        //取消
        $("#aCancel").click(function () {
            Megi.closeDialog();
        });
    }
}
//初始化页面
$(document).ready(function () {
    ExpensesMerge.init();
});