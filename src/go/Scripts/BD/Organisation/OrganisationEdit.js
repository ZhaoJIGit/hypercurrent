var OrgEdit = {
    tabswitch: new BrowserTabSwitch(),
    init: function () {
        OrgEdit.tabswitch.initSessionStorage();
        OrgEdit.initAction();
        SetupBase.initOrgForm();
        
    },
    initAction: function () {
        $("#aUpdate").click(function () {
            SetupBase.saveOrgDetail();
            return false;
        });
        $("#selCountry").combobox({
            onSelect: function (rec) {
                OrgEdit.switchProvinceInputCtrl(rec.id, true);
            }
        });
        $("#selProvince").combobox({
            onSelect: function (rec) {
                $("#txtProvince").val(rec.id);
                $("#txtCity").val('');
                $("#txtStreet").val('');
                $("#txtPostalNo").val('');
            }
        });
        $('input[name="language"]').off("click").on("click", function () {
            var isSelected = false;
            $('input[name="language"]').each(function () {
                if (this.checked) {
                    isSelected = true;
                }
            });
            if (isSelected) {
                $(".reg-error-image").hide();
            }
            else {
                $(".reg-error-image").show();
            }
        });
    },
    initDefaultValue: function(){
        $("#selCountry").combobox('setValue', '106');
        var model = {};
        model.MSystemLanguage = '0x7804,0x0009';
        OrgEdit.initSysLang(model);
    },
    validateForm: function () {
        var validateSuccess = true;
        var arrObj = [];
        if ($("#txtDisplayName").val() == '') {
            arrObj.push($("#txtDisplayName"));
            $("#txtDisplayName").addClass('validatebox-invalid');
            validateSuccess = false;
        }
        if ($("#txtLagalTrading").val() == '') {
            arrObj.push($("#txtLagalTrading"));
            $("#txtLagalTrading").addClass('validatebox-invalid');
            validateSuccess = false;
        }
        var isSelected = false;
        $('input[name="language"]').each(function () {
            if (this.checked) {
                isSelected = true;
            }
        });
        if (!isSelected) {
            arrObj.push($('input[name="language"]')[0]);
            $(".reg-error-image").show();
            validateSuccess = false;
        }
        if (arrObj.length > 0) {
            arrObj[0].focus();
        }
        return validateSuccess;
    },
    switchProvinceInputCtrl: function(countryId, isOnCountryChange){
        if (countryId == '106') {
            $("#selProvince").next(".combo").show();    
            $("#txtProvince").val($("#selProvince").combobox("getValue")).hide();
            if (isOnCountryChange) {
                $("#selProvince").combobox('setValue', '');
            }
        }
        else {
            $("#txtProvince").show();
            $("#selProvince").next(".combo").hide();
            if (isOnCountryChange) {
                $("#txtProvince").val('');
            }
        }
        if (isOnCountryChange) {
            $("#txtCity").val('');
            $("#txtStreet").val('');
            $("#txtPostalNo").val('');
        }
    },
    initSysLang: function (model) {
        var itemId = '';
        var langIds = '';
        if (model != null) {
            itemId = model.MItemID;
            langIds = model.MSystemLanguage == null ? '' : model.MSystemLanguage;
        }
        $("#hidItemId").val(itemId);
        $("#langIds").val(langIds);
        $('input[name="language"]').each(function (i) {
            if (langIds.indexOf($(this).val()) != -1) {
                $(this).attr("checked", true);
            }
        })
    },
    loadProvinceList: function (data) {
        OrgEdit.switchProvinceInputCtrl(data.MCountryID);
    }
}
$(document).ready(function () {
    OrgEdit.init();
});