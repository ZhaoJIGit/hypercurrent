var PayRunSourceEnum = { Copy: "Copy", New: "New" };
var SalaryMonthSelect = {
    init: function () {
        SalaryMonthSelect.bindAction();
        SalaryMonthSelect.initData();
    },
    initData: function () {
        $("#txtPeriod").val($("#hidCurDate").val());
    },
    bindAction: function () {
        $("#aStart").off('click.pa').on('click.pa', function () {
            var selYearMonth = $("#txtPeriod").val();
            if (!selYearMonth) {
                mDialog.message(HtmlLang.Write(LangModule.PA, "PleaseSelectASalaryPeriod", "请选择一个工资期间"));
                return false;
            }

            var source;
            if ($("#hidFrom").val() == "1") {
                source = PayRunSourceEnum.Copy;
            }
            else {
                source = PayRunSourceEnum.New;
            }

            SalaryMonthSelect.validateAction(selYearMonth, source, function () {
                var actionUrl = source == PayRunSourceEnum.Copy ? "/PA/SalaryPayment/PayRunCopy" : "/PA/SalaryPayment/PayRunNew";
                mAjax.submit(
                    actionUrl,
                    { yearMonth: selYearMonth },
                    function (response) {
                        if (response.Success) {
                            PayRunList.reload();
                            $.mDialog.close();
                        } else {
                            $.mDialog.alert(response.Message);
                        }
                    });
                });
        });
    },
    validateAction: function (selYearMonth, source, callBack) {
        mAjax.post(
            "/PA/SalaryPayment/ValidatePayRunAction",
            { yearMonth: selYearMonth, source: source },
            function (response) {
                var isExist = response.Tag === "Exist";
                var isAlertAndContinue = response.Tag === "AlertAndContinue";
                if (response.Success || isExist || isAlertAndContinue) {
                    if (isExist) {
                        $.mDialog.confirm(response.Message,
                        {
                            callback: function () {
                                if (callBack != undefined) {
                                    callBack();
                                }
                            }
                        });
                    }
                    else if (isAlertAndContinue) {
                        //弹出提示后，点确定继续执行
                        $.mDialog.alert(response.Message, function () {
                            callBack();
                        }, 0, false, true);
                    }
                    else {
                        if (callBack != undefined) {
                            callBack();
                        }
                    }
                } else {
                    $.mDialog.alert(response.Message);
                }
            }, "", true);
    }
}

$(document).ready(function () {
    SalaryMonthSelect.init();
});