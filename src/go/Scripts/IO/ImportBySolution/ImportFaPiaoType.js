var ImportFaPiaoType = {
    init: function () {
        ImportFaPiaoType.bindAction();
    },
    bindAction: function () {
        $("#SpecialFaPiao").click(function() {
            $("#divNotices").css("display", "block");
        });

        $("#ProfessionalFaPiao").click(function () {
            $("#divNotices").css("display", "none");
        });

        $("#aNext").click(function () {
            var fpType = $("#FPType").val();
            var type = 0;
            if ($("#ProfessionalFaPiao").get(0).checked) {
                type = $("#ProfessionalFaPiao").val();
            }
            if ($("#SpecialFaPiao").get(0).checked) {
                type = $("#SpecialFaPiao").val();
            }
            if (type != 0 && type != 1) {
                $.mDialog.alert(HtmlLang.Write(LangModule.FP, "ImportFPType", "请选择您要上传的发票类型"));
                return;
            }
            var url = "/IO/ImportBySolution/ImportFaPiao?type=" + type + "&fpType=" + fpType;
            mWindow.reload(url);
        });
    }
}

$(document).ready(function () {
    ImportFaPiaoType.init();
});