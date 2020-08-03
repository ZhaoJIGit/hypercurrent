var FaInit = {
    init: function () {
        FaInit.initForm();
        FaInit.bindAction();
    },
    initForm: function () {
        var type = $("#FAInitType").val();
        var haveChange = $("#HaveChangePermission").val();

        if (type == 0) {
            $("#txtPrefixName").val("FAN");
            $("#txtStartNumber").val("0001");
            var dateNow = Date.now();
            $('#selMonth').combobox("setValue", dateNow.getMonth() + 1);
            $('#selYear').combobox("setValue", dateNow.getFullYear());
        } else {
            $("body").mFormGet({
                url: "/FA/FAInit/GetFAInfo/",
                callback: function (data) {
                    $("#txtPrefixName").val(data.MPrefixName);
                    var startIndex = data.MStartIndex;
                    var numberCount = data.MNumberCount;
                    var startNumber = (Array(numberCount).join('0') + startIndex).slice(-numberCount);
                    $('#selYear').combobox("setValue", data.MConversionYear);
                    $('#selMonth').combobox("setValue", data.MConversionMonth);
                    $("#txtStartNumber").val(startNumber);
                }
            });

            $("#selMonth").combobox('disable');
            $("#selYear").combobox('disable');
            if (!haveChange) {
                $("#txtPrefixName").attr("disabled", true);
                $("#txtStartNumber").attr("disabled", true);
            }
        }

    },
    bindAction: function () {
        $("#aSure").click(function () {
            if (!$("#divFAInit").mFormValidate()) {
                return;
            }
            var obj = {};

            obj.MConversionYear = parseInt($('#selYear').combobox("getValue"));
            obj.MConversionMonth = parseInt($('#selMonth').combobox("getValue"));
            obj.MPrefixName = $('#txtPrefixName').val().trim();
            var startIndex = $('#txtStartNumber').val().trim();
            obj.MNumberCount = startIndex.length;
            obj.MStartIndex = parseInt(startIndex);
            obj.ActionType = $("#FAInitType").val().trim();

            $("body").mFormSubmit({
                url: "/FA/FAInit/SaveFAInit", validate: true, param: { model: obj }, callback: function (msg) {
                    var successMsg = HtmlLang.Write(LangModule.FA, "BeginFASuccess", "启用成功,重刷系统！");
                    if (msg.Success) {
                        top.$.mDialog.alert(successMsg);
                        top.mWindow.reload();
                    }
                    else {
                        if (!msg.Message) {
                            msg.Message = HtmlLang.Write(LangModule.FA, "BeginFAFail", "启用固定资产失败！");
                        }
                        top.$.mDialog.alert(msg.Message);
                    }
                }
            });
        });
    }
};

$(document).ready(function () {
    FaInit.init();
});
