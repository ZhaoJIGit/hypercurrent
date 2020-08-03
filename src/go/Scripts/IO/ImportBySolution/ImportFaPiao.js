var ImportFaPiao = {
    type:$("#inOrOutType").val() == 0 ? ImportTypeExt.OutFaPiao : ImportTypeExt.InFaPiao,
    init: function () {
        var title = HtmlLang.Write(LangModule.FP, "UploadFaPiao", "上传您的发票");
        $("div.boxTitle>h3", parent.document).text(title);
        ImportBase.minDialog();
        ImportFaPiao.bindAction();
        ImportFaPiao.bindSolutionList(ImportFaPiao.type);
        ImportFaPiao.initUI();
    },
    initUI: function () {
        var msg = $("#hidMessage").val();
        if (msg) {
            $.mDialog.alert(_.htmlDecode(msg));
            return;
        }
    },
    bindAction: function () {
        $("#aImport").click(function () {
            if (!$("#divImport").mFormValidate()) {
                return;
            }
            if (ImportBase.validateFile()) {
                $(".m-imain").mask("");
                ImportFaPiao.bindSubmit();
                $("#fileSelectForm").submit();
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

        Megi.attachPropChangeEvent(document.getElementById("MName"), function () {
            ImportFaPiao.switchValidate(this.value);
        });
    },
    switchValidate: function (val) {
        $("#MName").validatebox(val ? "disableValidation" : "enableValidation");
    },
    changeSolution: function (item) {
        if (item) {
            if (item.MItemID) {
                $("#MName").val(item.MName);
            }
            else {
                $("#MName").val('');
                $("#MName").focus();
            }
            //航天默认模板不可编辑
            if (item.MItemID == "999999") {
                $('#MName').attr('readonly', true);
            } else {
                $('#MName').attr('readonly', false);
            }

            ImportFaPiao.switchValidate(item.MItemID);
        }
    },
    bindSolutionList: function (type) {
        mAjax.post(
            "/IO/ImportBySolution/GetSolutionList",
            { type: type },
            function (msg) {
                var opt = {};
                opt.data = msg;
                opt.onSelect = ImportFaPiao.changeSolution;
                opt.valueField = "MItemID";
                opt.textField = "MName";
                opt.required = true;
                $("#selSolution").combobox(opt);
                var defaultSelect = msg[0].MItemID;
                //if (msg.length > 1) {
                //    defaultSelect = msg[1].MItemID;
                //}
                $("#selSolution").combobox("select", defaultSelect);
            });
    },
    getModel: function () {
        var model = {};
        model.SolutionID = $("#selSolution").combobox("getValue");
        model.TemplateType = ImportFaPiao.type;
        model.MSolutionName = $("#MName").val();
        return model;
    },
    bindSubmit: function () {
        $("form").ajaxForm({
            data: ImportFaPiao.getModel(),
            dataType: ImportBase.isIE9Previous ? null : "json",
            success: function (response) {
                response = response.Data;
                //解决IE9没返回Json结果
			    if (response == undefined || response.Success == undefined) {
				    $.getJSON("/BD/Docs/GetUploadResult?id=" + (new Date()).toString(), function (response) {
				        ImportFaPiao.doUploadCallBack(response);
				    });
			    }
			    else {
			        ImportFaPiao.doUploadCallBack(response);
			    }    
                
            },
            fail: function (event, data) {
                $.mDialog.alert(data);
            }
        });
    },
    doUploadCallBack: function (response) {
        if (response && !ImportBase.isIE9Previous && !response.Success)
        {
            $.mDialog.alert(response.Message);
            $(".m-imain").unmask();
        } else {
            var fileName = response.MFileName;
            var type = ImportFaPiao.type;
            //进项 销项
            var inOrOutType = $("#inOrOutType").val();
            var url = "/IO/ImportBySolution/ImportOptions/" + response.ObjectID + "?type=" + type + "&fileName=" + encodeURIComponent(fileName) + "&inOrOutType=" + inOrOutType;
            mWindow.reload(url);
        }
    }

}

$(document).ready(function () {
    $("#MName").validatebox("disableValidation");
    ImportFaPiao.init();
});