var PaySettingEdit = {
    init: function () {
        PaySettingEdit.bindAction();
        //PaySettingEdit.initSalaryTree();
        PaySettingEdit.initData();
    },
    bindAction: function () {
        $("#aSave").click(function () {
            PaySettingEdit.saveData();
        });
    },
    //初始化树
    initSalaryTree: function () {

        $("#divSalaryTree").tree({
            url: "/PA/PayItem/GetSalaryItemTreeList",
            checkbox: true,
            onLoadSuccess: function (node, data) {
                //数加载成功后，加载salary部分的数据
                PaySettingEdit.initData();
            }
        });
    },
    initData: function () {
        //只是初始化Salary部分
        $("#MStartPayMonth").combobox("setValue", $("#hideMStartPayMonth").val());
        $("#MStartPayDay").combobox("setValue", $("#hideMStartPayDay").val());
        $("#MEndPayMonth").combobox("setValue", $("#hideMEndPayMonth").val());
        $("#MEndPayDay").combobox("setValue", $("#hideMEndPayDay").val());
        $("#MPaymentMonth").combobox("setValue", $("#hideMPaymentMonth").val());
        $("#MPaymentDay").combobox("setValue", $("#hideMPaymentDay").val());

        //工资项打勾勾
        //var payItemIds = $("#hidePayItemIDs").val();
        //if(payItemIds){
        //    var array = payItemIds.split(',');
        //    for (var i = 0 ; i < array.length; i++) {
        //        var id = array[i];
        //        var node = $('#divSalaryTree').tree('find', id);
        //        $('#divSalaryTree').tree('check', node.target);
        //    }
        //}


    },
    saveData: function () {

        //var result = $(".m-imain").mFormValidate();

        var v1 = $("#MStartPayDay").combobox("getText");
        var v2 = $("#MEndPayDay").combobox("getText");
        var v3 = $("#MPaymentDay").combobox("getText")
        if (v1 > 31 || v1 < 0 || v2 > 31 || v2 < 0 || v3 > 31 || v3 < 0) {
            return;
        }

        var model = {};
        model.MItemID = $("#hideMitemID").val();
        //公司
        model.MSocialSecurityAccount = $("#MSocialSecurityAccount").val();
        model.MRetirementSecurityPer = $("#MRetirementSecurityPer").val();
        model.MMedicalInsurancePer = $("#MMedicalInsurancePer").val();
        model.MUmemploymentInsurancePer = $("#MUmemploymentInsurancePer").val();
        model.MMaternityInsurancePer = $("#MMaternityInsurancePer").val();
        model.MIndustrialInjuryPer = $("#MIndustrialInjuryPer").val();
        model.MSeriousIiinessInjuryPer = $("#MSeriousIiinessInjuryPer").val();
        model.MOtherPer = $("#MOtherPer").val();
        //员工
        model.MEmpRetirementSecurityPer = $("#MEmpRetirementSecurityPer").val();
        model.MEmpMedicalInsurancePer = $("#MEmpMedicalInsurancePer").val();
        model.MEmpUmemploymentInsurancePer = $("#MEmpUmemploymentInsurancePer").val();

        //住房公积金
        model.MProvidentFundAccount = $("#MProvidentFundAccount").val();
        model.MProvidentFundPer = $("#MProvidentFundPer").val();
        model.MAddProvidentFundPer = $("#MAddProvidentFundPer").val();

        model.MStartPayMonth = $("#MStartPayMonth").combobox("getValue");
        model.MStartPayDay = $("#MStartPayDay").combobox("getValue");
        model.MEndPayMonth = $("#MEndPayMonth").combobox("getValue");
        model.MEndPayDay = $("#MEndPayDay").combobox("getValue");
        model.MPaymentMonth = $("#MPaymentMonth").combobox("getValue");
        model.MPaymentDay = $("#MPaymentDay").combobox("getValue");

        //公司所属的工资项
        //var nodes = $('#divSalaryTree').tree('getChecked');
        //if (nodes && nodes.length>0) {
        //    model.MPayItemIDs = "";
        //    for (var i = 0 ; i < nodes.length; i++) {
        //        var node = nodes[i];
        //        model.MPayItemIDs += node.id + ",";
        //    }
        //}
        //if (model.MPayItemIDs) {
        //    model.MPayItemIDs = model.MPayItemIDs.substring(0, model.MPayItemIDs.length - 1);
        //}

        var url = "/PA/PayrollBasic/UpdatePaySetting";

        //$("body").mask();
        $("body").mFormSubmit({
            param: { model: model },
            url: url, callback: function (msg) {
                //$("body").unmask();
                if (msg.Success) {
                    $.mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessful", "Save successfully!"));
                    $.mDialog.close();
                }
                else {
                    $.mDialog.warning(msg.Message);
                }
            }
        });
        return false;
    }
}

$(document).ready(function () {
    PaySettingEdit.init();
});