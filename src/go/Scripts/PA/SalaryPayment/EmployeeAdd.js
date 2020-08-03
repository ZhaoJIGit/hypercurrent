var EmployeeAdd = {
    init: function () {
        EmployeeAdd.bindAction();
        EmployeeAdd.bindGrid();
    },
    bindAction: function () {
        $("#aSave").off('click').on('click', function () {
            Megi.grid("#empList", "optSelected", {
                callback: function (ids) {
                    mAjax.submit(
                        "/PA/SalaryPayment/SalaryPaymentListUpdate",
                        { runId: $("#hidRunId").val(), empIds: ids },
                        function (response) {
                            if (response.Success) {
                                parent.SalaryPaymentList.reload();
                                $.mDialog.close(0);
                            } else {
                                $.mDialog.alert(response.Message);
                            }
                        });
                }
            });
        });
    },
    bindGrid: function () {
        var columnList = [
            {
                title: '<input type=\"checkbox\" >', field: 'MItemID', width: 25, align: 'center', formatter: function (value, rec, rowIndex) {
                    return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" />";
                }
            },
            {
                title: HtmlLang.Write(LangModule.PA, "EmployeeName", "Employee Name"), field: 'MName', width: 80, align: 'left', sortable: false
            }];

        var queryParam = {};
        queryParam.runId = $("#hidRunId").val();
        Megi.grid("#empList", {
            resizable: true,
            auto: true,
            url: "/PA/SalaryPayment/GetUnPayEmployeeList",
            queryParams: queryParam,
            columns: [columnList]
        });
    }
}

$(document).ready(function () {
    EmployeeAdd.init();
});