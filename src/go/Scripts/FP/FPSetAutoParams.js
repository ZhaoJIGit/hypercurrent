var FPSetAutoParams = {
    Init: function () {
        $("#dataMonthAgo").combobox("setValue", $("#monthAgo").val());
        FPSetAutoParams.BindAction();
    },
    BindAction: function () {
        $("#aSure").off("click").on("click", function () {
            var obj = {};
            obj.MAccountNo = $("#txtAccountNo").val();
            obj.MPassword = $("#password").val();
            obj.MItemID = $("#itemId").val();
            obj.MMonthAgo = $("#dataMonthAgo").combobox("getValue");
            mAjax.submit("/FP/FPSetting/SaveSetAutoParams", { model: obj },
            function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessful", "保存成功！"));
                    $.mDialog.close();
                } else {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "SaveFailed", "保存失败！"));
                }
            });
        });

    }
}

$(document).ready(function () {
    FPSetAutoParams.Init();
});