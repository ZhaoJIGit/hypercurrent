
var IssueFapiaoInfo = {
    invoiceId: $("#hidInvoiceID2").val(),
    invoiceType: $("#hidInvoiceType").val(),
    taxId: $("#hidTaxId").val(),
    init: function () {
        IssueFapiaoInfo.getTableView();
    },
    getTableView: function () {
        mAjax.post(
            "/FP/FPHome/GetTableViewModelByInvoiceID",
            { invoiceId: IssueFapiaoInfo.invoiceId },
            function (msg) {
                    IssueFapiaoInfo.initData(msg);
            });
    },
    initData: function (model) {
        if (model.MItemID) {
            $("#divIssueFapiao").hide();
            $(".issued-info").show();
            var statusClass = 'partly-issued';
            var isSale = IssueFapiaoInfo.invoiceType === "0";

            var lblPartlyIssued = isSale ? HtmlLang.Write(LangModule.FP, "partlyIssued", "部分开票") : HtmlLang.Write(LangModule.FP, "partlyCollected", "部分收集");
            var lblIssued = isSale ? HtmlLang.Write(LangModule.FP, "Issued", "完全开票") : HtmlLang.Write(LangModule.FP, "Collected", "完全收集");
            var lblWaitingIssue = isSale ? HtmlLang.Write(LangModule.FP, "notIssued", "等待开票") : HtmlLang.Write(LangModule.FP, "notCollected", "等待收集");

            var statusTitle = lblWaitingIssue;
            switch (model.MIssueStatus) {
                case 1:
                    statusClass = "partly-issued";
                    statusTitle = lblPartlyIssued;
                    break;
                case 2:
                    statusClass = "issued";
                    statusTitle = lblIssued;
                    break;
            }

            $("#aIssue").off('click');
            $("#aIssue").linkbutton("disable");
            $("#liUnApprove").hide();

            $("#divStatusTitle").html("<span class='" + statusClass + "'></span>" + statusTitle);

            var aNumber = "<a id='aNumber' href='javascript:void(0)' onclick=\"IssueFapiao.single('" + model.MItemID + "', '" + IssueFapiaoInfo.invoiceId + "','" + IssueFapiaoInfo.invoiceType + "')\">" + IssueFapiao.GetFullTableNumber(model.MNumber, IssueFapiaoInfo.invoiceType) + "</a>";
            $("#divNumber").html(HtmlLang.Write(LangModule.FP, "TableNumber", "开票单号") + "：" + aNumber);
            $("#divDate").html(HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期") + "：" + $.mDate.format(model.MBizDate));
            $("#divIssuedAmount").html(HtmlLang.Write(LangModule.FP, "IssuedAmount", "已开票金额") + "：" + Megi.Math.toMoneyFormat(model.IssuedAmount, 2));
        }
        else {
            $(".issued-info").hide();
            $("#aIssue").linkbutton("enable").off('click').on('click', function () {
                IssueFapiao.single('', IssueFapiaoInfo.invoiceId, IssueFapiaoInfo.invoiceType, IssueFapiaoInfo.taxId == "No_Tax", function () { IssueFapiaoInfo.init(); });
            });
            $("#spStatusImg").attr("class", "");
            $("#divNumber,#divDate,#divIssuedAmount, #divStatusTitle").html('');
            $("#liUnApprove").show();
        }
    }
}

$(document).ready(function () {
    IssueFapiaoInfo.init();
});