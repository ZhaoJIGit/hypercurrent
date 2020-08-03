
var TaxFillingView = {
    init: function () {
        TaxFillingView.bindAction();
    },

    bindAction: function () {
        var reportId = $("#hidReportID").val();
        $("#aPostReport").click(function () {
            TaxFillingBase.post("/Report/ETax/AddPostReportTaxTask", { MReportID: reportId }, function () {

            });
        });
    }
}

$(document).ready(function () {
    TaxFillingView.init();
});