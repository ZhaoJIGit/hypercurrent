/// <reference path="/Scripts/IV/IVBase.js" />
/// <reference path="/Scripts/IV/Expense/ExpenseEditBase.js" />
var ExpenseView = {
    //费用报销单ID
    ExpenseID: $("#hidExpenseID").val(),
    //费用报销单模型
    ExpenseModel: null,
    //费用报销单列表地址
    ListUrl: "/IV/Expense/ExpenseList/",
    IsShowVerifConfirm: function () {
        var isShowVerif = $("#hidIsShowVerif").val() == "True" ? true : false;
        var expenseId = $("#hidExpenseID").val();
        if (expenseId != undefined && isShowVerif && Megi.request("sv") == "1") {
            mAjax.post("/IV/Expense/GetVerificationById", { expenseId: expenseId }, function (data) {
                if (data != null && data.length > 0) {
                    var currency = data[0].MCurrencyID;
                    var totalAmount = 0;
                    $(data).each(function (i) {
                        totalAmount += data[i].Amount;
                    });
                    var title = HtmlLang.Write(LangModule.IV, "ThereIs", " There is") + " " + "<b>" +
                                Megi.Math.toMoneyFormat(totalAmount, 2) + " " + currency + "</b>" + " " +
                                HtmlLang.Write(LangModule.IV, "InOutstandingCredit", "in outstanding credit.") + " " +
                                HtmlLang.Write(LangModule.IV, "AllocatecreditToReceipt", "Would you like to allocate credit to this receipt?");
                    $.mDialog.confirm(title, function (sure) {
                        if (sure) {
                            Verification.open(expenseId, "Expense");
                        }
                    }, "", true);
                }
            });
        }
    },
    //初始化
    init: function () {
        //解决手动修改日期，到期日没跟着变问题
        $("#txtDate").datebox({
            onChange: function (newValue, oldValue) {
                if (newValue && oldValue && mDate.isDateString(newValue) && $.mDate.format(new Date(newValue)) != $.mDate.format(new Date(oldValue))) {
                    ExpenseEditBase.changeDate();
                }
            }
        });
        ExpenseView.IsShowVerifConfirm();
        ExpenseView.initOperate();
        ExpenseView.initExpenseModel();
        ExpenseView.initCombobox();
        //将当前页面设置为稳定状态
        $.mTab.setStable();
    },
    //初始化相关操作
    initOperate: function () {
        //操作记录
        $("#aHistory").click(function () {
            ExpenseEditBase.OpenHistoryDialog(ExpenseView.ExpenseID, "History");
        });
        //修改预计付款日期
        $("#btnSave").click(function () {
            //到期日期必须大于单据录入日期
            var bizDate = $('#txtDate').datebox('getValue');
            //预计付款日期必须大于等于单据录入日期
            var expectedDate = $('#txtExpectedDate').datebox('getValue');
            if (expectedDate) {
                //如果选择预计付款日期才需要验证
                if (expectedDate < bizDate) {
                    $.mDialog.alert(HtmlLang.Write(LangModule.IV, "ExpectedDateIsGreaterThanTheDate", "Expected Payment date must be greater than or equal to date."));
                    return;
                }
            }
            var obj = {};
            obj.MID = ExpenseView.ExpenseID;
            obj.MExpectedDate = expectedDate;
            $("body").mFormSubmit({
                url: "/IV/Expense/UpdateExpenseExpectedInfo", param: { model: obj }, validate: true, callback: function (msg) {
                    //提示信息
                    $.mMsg(LangKey.SaveSuccessfully);
                    //刷新列表页面
                    var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                    var url = ExpenseView.ListUrl + "?id=" + ExpenseView.ExpenseModel.MStatus;
                    $.mTab.refresh(title, url, false, true);
                    //刷新当前页面
                    mWindow.reload("/IV/Expense/ExpenseView/" + ExpenseView.ExpenseID);
                }
            });
        });
        //打印
        $("#aPrint").click(function () {
            IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims"), $.toJSON({ SelectedIds: $("#hidExpenseID").val() }), "ExpenseClaimsPrint");
        });
    },
    //初始化下拉框
    initCombobox: function () {
        //员工下拉框
        $('#selEmployee').combobox({
            url: '/BD/Employees/GetEmployees?includeDisable=true',
            valueField: 'MItemID',
            textField: 'MFullName',
            onLoadSuccess: function () {
                //选中员工
                if (ExpenseView.ExpenseModel) {
                    $('#selEmployee').combobox('setValue', ExpenseView.ExpenseModel.MEmployee);
                }
                //将当前页面设置为稳定状态
                $.mTab.setStable();
            }
        });
    },
    //初始化费用报销单模型
    initExpenseModel: function () {
        //费用报销单模型
        var msg = mText.getObject("#hidExpenseModel");
        ExpenseView.ExpenseModel = msg;
        //状态
        $("#divExpenseStatus").html(IVBase.GetStatus(msg.MStatus));
        //摘要
        $("#txtRef").val(msg.MReference);
        //录入日期
        $('#txtDate').datebox('setValue', $.mDate.format(msg.MBizDate));
        //到期日期
        $('#txtDueDate').datebox('setValue', $.mDate.format(msg.MDueDate));
        //预计付款日期
        $('#txtExpectedDate').datebox('setValue', $.mDate.format(msg.MExpectedDate));
        //初始化费用报销单明细列表控件
        ExpenseEditBase.init(ExpenseEditBase.IVType.Sale, ExpenseView.ExpenseID, msg.MOToLRate, msg.MLToORate, msg.Verification, msg.ExpenseEntry, msg.MCyID, msg.MTaxID, msg.MStatus, true);
    }
}
//初始化页面
$(document).ready(function () {
    ExpenseView.init();
});