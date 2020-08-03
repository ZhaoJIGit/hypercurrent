var SelectPrintSetting = {
    type: $("#hidType").val(),
    reportType: $("#hidReportType").val(),
    init: function () {
        SelectPrintSetting.initData();
        SelectPrintSetting.initAction();
    },
    initData: function () {
        $("#selBranding").combobox({
            required: true,
            textField: 'Value',
            valueField: 'Key',
            url: '/PT/PTBiz/GetPTList',
            queryParams: { bizObject: $('#hidBizObject').val() },
            onLoadSuccess: function () {
                var data = $(this).combobox("getData");
                if (data.length > 0) {
                    $(this).combobox("setValue", data[0].Key);
                }
            }
        });
    },
    initAction: function () {
        $("#aSave").click(function () {
            if (!$("#selBranding").mFormValidate()) {
                return;
            }
            var title = '';
            var param = $("#hidParam")[0].value + '&printSettingId=' + $("#selBranding").combobox("getValue");
            switch (SelectPrintSetting.type) {
                case "Invoices":
                case "Invoice":
                    title = HtmlLang.Write(LangModule.IV, "SendInvoice", "Send Invoice");
                    break;
                case "Statements":
                case "Statement":
                    title = HtmlLang.Write(LangModule.IV, "SendStatement", "Send Statement"); 
                    break;
                case "SalaryPrint":
                    title = HtmlLang.Write(LangModule.PA, "SendPayslip", "Send Payslip");
                    break;
            }
            if (parent.Print) {
                parent.Print.openSendEmailDialog(title, param);
            }
            $.mDialog.close();
        });
    }
}

$(document).ready(function () {
    SelectPrintSetting.init();
});