var SalaryItemEdit = {
    isEnableGL: $("#hideIsEnableGL").val(),
    unEditableTypes: [1000,1035,1040,1045,1050,1055,1060,1065,2000,2005,2010,2015,2020,2025,2030,2035,2040,2045,2050,3000,3005,3010,3015],
    init: function () {
        SalaryItemEdit.initAction();
        SalaryItemEdit.initForm();
    },
    initAction: function () {
        $("#aSave").off("click").on("click", function () {
            SalaryItemEdit.saveData();
        });
        $("#aCancel").off("click").on("click", function () {
            var toUrl = $(this).attr("href");
            $.mDialog.close();
            parent.SalaryItemList.reload();
            return false;
        });
    },
    initForm: function () {
        var id = $("#hidItemID").val();
        var editType = $("#hideEditType").val();
        if (id) {
            $("body").mask("");
            $("body").mFormGet({
                url: "/PA/PayItem/GetEditInfo?id=" + id + "&type=" + editType, fill: true, callback: function (model) {
                    if (model) {
                        $("#txtMNumber").val(model.MName);
                        if ($.inArray(model.MItemType, SalaryItemEdit.unEditableTypes) != -1) {
                            $("#txtMNumber").attr("disabled", "disabled");
                        }
                        var dataLang = model.MultiLanguage;
                        $("#txtMNumber").initLangEditor(dataLang[0]);

                        //$("#cbxMparent").combobox("setValue", model.MGroupID);

                        $("input[name=MCoefficient][value=" + model.MCoefficient + "]").prop("checked", true);
                        //是否为预设工资项（不是用户新增项1023，也不是用户扣除项1067）
                        var isDefaultItem = model.MItemType != 1023 && model.MItemType != 1067;
                        //预设、被引用的工资项置灰
                        if (isDefaultItem || !model.MIsCanEdit) {
                            SalaryItemEdit.switchCtrlStatus(this.checked, $("#divCalculationLogic input:radio"));
                        }

                        if (SalaryItemEdit.isEnableGL) {
                            $("#cbxAccount").combobox("setValue", model.MAccountCode);
                        }
                    }

                    $("body").unmask();
                }
            });
        }
    },
    //切换控件状态
    switchCtrlStatus: function (chkStatus, chkObj) {
        if (chkStatus) {
            chkObj.removeAttr("disabled");
        }
        else {
            chkObj.attr("disabled", true);
        }
    },
    initSalaryGroupItem: function (callback) {
        $("#cbxMparent").combobox({
            valueField: 'MItemID',
            textField: 'MName',
            onLoadSuccess: function () {
                $("#cbxMparent").combobox("setValue", "");
            }
        });

        var url = "/PA/PayItem/GetSalaryGroupItemList";
        $("body").mask("");
        mAjax.post(url, {}, function (models) {
            if (models) {
                var data = models;
                if (data) {
                    var jsonData = [];
                    //jsonString += "{\"MItemID\":\"\",\"MName\":\"" + none + "\"},";
                    if (data.length > 0) {
                        for (var i = 0 ; i < data.length; i++) {
                            var temp = data[i];
                            jsonData.push({ MItemID: temp.MItemID, MName: mText.encode(temp.MName) });
                        }
                    }

                    data = jsonData;
                }

                $("#cbxMparent").combobox("loadData", data);
            }

            if (callback) {
                callback();
            }
            $("body").unmask();
        });
    },
    saveData: function () {
        if (!$(".m-imain").mFormValidate()) {
            return;
        }

        var isNew = !$("#hidItemID").val();
        var coefficient = $("input:radio:checked[name='MCoefficient']").val();
        //新增时计算逻辑必录
        if (isNew && !coefficient) {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "CalculationLogicRequired", "请选择计算逻辑"));
            return;
        }

        var model = {};
        model.MCoefficient = coefficient;
        model.MItemID = $("#hidItemID").val();
        //model.MGroupID = $("#cbxMparent").combobox("getValue");
        model.MultiLanguage = new Array();
        if (SalaryItemEdit.isEnableGL) {
            model.MAccountCode = $("#cbxAccount").combobox("getValue");
        }

        var nameLang = $("#txtMNumber").getLangEditorData();
        model.MultiLanguage.push(nameLang);

        var editType = $("#hideEditType").val();

        var url = "/PA/PayItem/SalaryItemUpdate";
        if (editType == "0") {
            url = "/PA/PayItem/SalaryGroupItemUpdate";
        }

        mAjax.submit(url, { model: model },function (msg) {
            if (msg.Success) {

                $.mDialog.message(HtmlLang.Write(LangModule.BD, "SalaryUpdateSuccess", "operation successfully"));

                if (parent.SalaryItemList) {
                    parent.SalaryItemList.bindGrid();
                }
                $.mDialog.close();
            }
            else {
                $.mDialog.warning(msg.Message);
            }
        });
    }
}

$(document).ready(function () {
    SalaryItemEdit.init();
});