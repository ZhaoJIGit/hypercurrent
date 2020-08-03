/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var Import = {
    importWay: 0,
    init: function () {
        ImportBase.minDialog();
        Import.bindAction();

        //银行ID为空时禁止导入
        var bankId = $("#hidBankId").val();
        if (!bankId) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "bankIdIsEmpty", "Not find bank account,please select a bank account"));
            return;
        }

        var cbStyle = { "font-size": "16px" };
        $("#selSolution").parent().find(".combo-text").css(cbStyle);
        var panel = $("#selSolution").combo("panel");
        panel.find(".combobox-item").css(cbStyle);

        if ($("#hidBankTypeId").val() != '') {
            var data = $("#selSolution").combobox('getData');
            if (data.length > 1) {
                $("#selSolution").combobox('setValue', data[1].Key);
            }
        }
        localStorage.clear();

        var msg = $("#hidMessage").val();
        if (msg) {
            $.mDialog.alert(msg);
            return;
        }
    },
    bindAction: function () {
        $("#aImport").click(function () {
            var solutionId = $("#selSolution").combobox("getValue");
            if (solutionId == undefined) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Common, "SolutionNotExist", "导入方案不存在，请重新选择"));
                $("#selSolution").combobox().next('span').find('input').focus();
                return;
            }

            if (ImportBase.isSelectFile()) {
                $(".m-imain").mask("");
                $("#fileSelectForm").mSubmit();
            }
            else {
                $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "SelectStatementFile", "Please select a statement file!"));
            }
        });
        if (Megi.isIE9Previous()) {
            var type = "change";
            var file = document.getElementById("fileInput");
            if (typeof (document.createEvent) != 'undefined') {
                e = document.createEvent('HTMLEvents');
                e.initEvent(type, true, true);
                file.dispatchEvent(e);
            } else if (typeof (document.createEventObject) != 'undefined') {
                try {
                    e = document.createEventObject();
                    file.fireEvent('on' + type.toLowerCase(), e);
                } catch (err) { }
            }
        }
        $("#fileInput").change(function () {
            ImportBase.onFileChanged(this);

            var validateResult = FileBase.validateFile(this.value, 0, FileBase.excelIncludeCsvRegex);
            if (validateResult) {
                ImportBase.clearFile(this);
                $.mDialog.alert(validateResult);
            }
        });

        $("#aCancel").click(function () {
            ImportBase.closeImportBox();
        });
    },
    doUploadCallBack: function (response) {
        if (response && !ImportBase.isIE9Previous && !response.Success) {
            $.mDialog.alert(response.Message);
            $(".m-imain").unmask();
        } else {
            var fileName = response.FileName;

            //隐藏第一个弹窗框，确保一个页面不出现两个框
            //$("#firstImportBox", window.parent.document).css("display", "none");

            var url = "/BD/BDBank/ImportOptions/" + $("#hidBankId").val() + "?solutionId=" + $("#selSolution").combobox("getValue") + "&type=" + $("#hidBankTypeId").val() + "&fileName=" + encodeURIComponent(fileName);
            mWindow.reload(url);
        }
    }
}

$(document).ready(function () {
    Import.init();
    $("form").ajaxForm({
        dataType: ImportBase.isIE9Previous ? null : "json",
        success: function (response) {
            response = response.Data || response;
			//解决IE9没返回Json结果
			if (response == undefined || response.Success == undefined) {
				$.getJSON("/BD/Docs/GetUploadResult?id=" + (new Date()).toString(), function (response) {
				    Import.doUploadCallBack(response);
				});
			}
			else {
			    Import.doUploadCallBack(response);
			}            
        },
        fail: function (event, data) {
            $.mDialog.alert(data);
        }
    });
});