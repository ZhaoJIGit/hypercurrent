var ContactType = { Customer: 'Customer', Supplier: 'Supplier', Other: 'Other' };
var GoContactEdit = {
    isEnableGL: $("#hideIsEnableGL").val(),
    isUpdate: $("#ContactID").val().length > 0,
    arrContactType: [],
    isQuote: $("#hideIsQuote").val() == "true" ? true : false,
    init: function () {
        GoContactEdit.initTab();
        GoContactEdit.saveClick();
        //邮件地址和物理地址赋值
        GoContactEdit.copyPostal2Physical();
        //Sales Tax 的默认值置为空 Felson 2015.1.12
        GoContactEdit.clearComboboxValue($("#SelMSalTaxTypeID"));
        //Purchase Tax的默认值置为空 Felson 2015.1.12
        GoContactEdit.clearComboboxValue($("#SelMPurTaxTypeID"));
        GoContactEdit.initTrackEntry();
        GoContactEdit.initContactType();

        GoContactEdit.initCurrenAccount();
        GoContactEdit.clearComboboxValue($("#MSalDueCondition"));
        GoContactEdit.clearComboboxValue($("#MPurDueCondition"));

        GoContactEdit.initAction();

        if (GoContactEdit.isUpdate) {
            mAjax.post("/BD/Contacts/GetContactEditInfo/", { model: { MItemID: $("#ContactID").val() } }, function (data) {

                $("body").mFormSetForm(data);

                if (GoContactEdit.isEnableGL && data) {
                    $("#cbxCCurrentMoney").combobox("setValue", data.MCCurrentAccountCode);
                };
                //对国家赋值
                $("#selMPCountryID").combobox("setValue", data.MPCountryID);
                $("#selMRealCountryID").combobox("setValue", data.MRealCountryID);
            });
        }
        else {
            GoContactEdit.clearComboboxValue($("select[id^=sal_MSalTrackEntry]"));
            GoContactEdit.clearComboboxValue($("select[id^=pur_MPurTrackEntry]"));
        }
    },
    initAction: function () {
        $("#cbCustomer,#cbSupplier,#cbOther").click(function () {
            GoContactEdit.arrContactType = [];
            if ($("#cbCustomer").attr("checked")) {
                GoContactEdit.arrContactType.push(ContactType.Customer);
            }
            if ($("#cbSupplier").attr("checked")) {
                GoContactEdit.arrContactType.push(ContactType.Supplier);
            }
            if ($("#cbOther").attr("checked")) {
                GoContactEdit.arrContactType.push(ContactType.Other);
            }

            var isAll = ($("#cbCustomer").attr("checked") && $("#cbSupplier").attr("checked")) || $("#cbOther").attr("checked");

            if (!isAll && $("#cbCustomer").attr("checked")) {
                //只选择了客户去掉采购的跟踪项
                $("#divPurTracking").hide();
                $("#divPurDueDate").hide();

                $("#divSaleTracking").show();
                $("#divSaleDueDate").show();

                $("#divSaleTax").show();
                $("#divPurTax").hide();

                //去掉之前录入的数据
                GoContactEdit.clearCtrlValue("#divPurDueDate,#divPurTax,#divPurTracking");
            } else if (!isAll && $("#cbSupplier").attr("checked")) {
                $("#divPurTracking").show();
                $("#divPurDueDate").show();
                $("#divSaleTracking").hide();
                $("#divSaleDueDate").hide();

                $("#divSaleTax").hide();
                $("#divPurTax").show();

                //去掉之前录入的数据
                GoContactEdit.clearCtrlValue("#divSaleDueDate,#divSaleTax,#divSaleTracking");
            } else {
                //既是供应商也是客户
                $("#divPurTracking").show();
                $("#divPurDueDate").show();
                $("#divSaleTracking").show();
                $("#divSaleDueDate").show();

                $("#divSaleTax").show();
                $("#divPurTax").show();
            }
        });
    },
    initLog: function () {
        if (GoContactEdit.isUpdate) {
            BusLog.bindGrid(contactID);
        }
    },
    switchTabContent: function (idx) {
        if (idx == 0) {
            $("#divContactInfo").show();
            $("#divFinDetail").hide();
        }
        else {
            $("#divContactInfo").hide();
            $("#divFinDetail").show();
        }
    },
    clearCtrlValue: function (ids) {
        var arrInput = $(ids).find("input").not("input[id^=MTrackHead]");
        var arrSelect = $(ids).find("select");
        if (arrInput.length > 0) {
            $.each(arrInput, function (idx, obj) {
                $(obj).val('');
            });
        }
        if (arrSelect.length > 0) {
            $.each(arrSelect, function (idx, obj) {
                $(obj).combobox("setValue", '');
            });
        }
    },
    switchCtrlByContactType: function () {
        switch (GoContactEdit.arrContactType.toString()) {
            case ContactType.Customer:
                //隐藏采购字段
                $("#divIVSetting").show();
                $("#divSaleDueDate").show();
                $("#divPurDueDate").hide();

                $("#divSaleTax").show();
                $("#divPurTax").hide();

                $("#divSaleTracking").show();
                $("#divPurTracking").hide();

                GoContactEdit.clearCtrlValue("#divPurDueDate,#divPurTax,#divPurTracking");
                break;
            case ContactType.Supplier:
                //隐藏销售字段
                $("#divIVSetting").show();
                $("#divSaleDueDate").hide();
                $("#divPurDueDate").show();

                $("#divSaleTax").hide();
                $("#divPurTax").show();

                $("#divSaleTracking").hide();
                $("#divPurTracking").show();

                GoContactEdit.clearCtrlValue("#divSaleDueDate,#divSaleTax,#divSaleTracking");
                break;
            case ContactType.Other:
                //隐藏采购及销售字段
                $("#divIVSetting").hide();
                $("#divSaleDueDate").hide();
                $("#divPurDueDate").hide();

                $("#divSaleTax").hide();
                $("#divPurTax").hide();

                $("#divSaleTracking").hide();
                $("#divPurTracking").hide();
                GoContactEdit.clearCtrlValue("#divPurDueDate,#divPurTax,#divPurTracking,#divSaleDueDate,#divSaleTax,#divSaleTracking");
                break;
            default:
                //都显示
                $("#divIVSetting").show();
                $("#divSaleDueDate").show();
                $("#divPurDueDate").show();

                $("#divSaleTax").show();
                $("#divPurTax").show();

                $("#divSaleTracking").show();
                $("#divPurTracking").show();
                break;
        }
    },
    initContactType: function () {
        //初始化“联系人类型”
        var isCustomer = $("#hideIsCustomer").val() == "true" ? true : false;
        var isSupplier = $("#hideIsSupplier").val() == "true" ? true : false;
        var isOther = $("#hideIsOther").val() == "true" ? true : false;

        if (isCustomer && !isSupplier && !isOther) {
            //打勾并禁用“客户”复选框 被引用了才不允许修改 chenpan
            $("#cbCustomer").attr("checked", true);
            if (GoContactEdit.isQuote) {
                $("#cbCustomer").attr("disabled", true);
                $("#divCustomer").addClass("checkbox_disabled");
            }
            GoContactEdit.arrContactType = ContactType.Customer;


        } else if (!isCustomer && isSupplier && !isOther) {
            //打勾并禁用“供应商”复选框
            $("#cbSupplier").attr("checked", true);
            if (GoContactEdit.isQuote) {
                $("#cbSupplier").attr("disabled", true);
                $("#divSupplier").addClass("checkbox_disabled");
            }
            GoContactEdit.arrContactType = ContactType.Supplier;
        } else if (!isCustomer && !isSupplier && isOther) {
            $("#cbOther").attr("checked", true);
            if (GoContactEdit.isQuote) {
                $("#cbOther").attr("disabled", true);
                $("#divOther").addClass("checkbox_disabled");
            }
        } else if (isCustomer && isSupplier && !isOther) {
            $("#cbCustomer").attr("checked", true);
            $("#cbSupplier").attr("checked", true);
            if (GoContactEdit.isQuote) {
                $("#cbCustomer").attr("disabled", true);
                $("#cbSupplier").attr("disabled", true);
                $("#divCustomer").addClass("checkbox_disabled");
                $("#divSupplier").addClass("checkbox_disabled");
            }
            GoContactEdit.arrContactType = [ContactType.Customer, ContactType.Supplier];
        } else if (isCustomer && !isSupplier && isOther) {
            $("#cbCustomer").attr("checked", true);
            $("#cbOther").attr("checked", true);
            if (GoContactEdit.isQuote) {
                $("#cbCustomer").attr("disabled", true);
                $("#cbOther").attr("disabled", true);
                $("#divCustomer").addClass("checkbox_disabled");
                $("#divOther").addClass("checkbox_disabled");
            }
            GoContactEdit.arrContactType = [ContactType.Customer, ContactType.Other];
        } else if (!isCustomer && isSupplier && isOther) {
            $("#cbOther").attr("checked", true);
            $("#cbSupplier").attr("checked", true);
            if (GoContactEdit.isQuote) {
                $("#cbOther").attr("disabled", true);
                $("#cbSupplier").attr("disabled", true);
                $("#divOther").addClass("checkbox_disabled");
                $("#divSupplier").addClass("checkbox_disabled");
            }
            GoContactEdit.arrContactType = [ContactType.Supplier, ContactType.Other];
        } else if (isCustomer && isSupplier && isOther) {
            $("#cbOther").attr("checked", true);
            $("#cbCustomer").attr("checked", true);
            $("#cbSupplier").attr("checked", true);

            if (GoContactEdit.isQuote) {
                $("#cbCustomer").attr("disabled", true);
                $("#divCustomer").addClass("checkbox_disabled");
                $("#divOther").addClass("checkbox_disabled");
                $("#cbOther").attr("disabled", true);
                $("#cbSupplier").attr("disabled", true);
                $("#divSupplier").addClass("checkbox_disabled");
            }
        }

        GoContactEdit.initCurrenAccount("1");
        GoContactEdit.switchCtrlByContactType();
    },
    initTab: function () {
        var idx = 0;
        if (window.location.search) {
            var tabIndex = Megi.getUrlParam("tabIndex");

            idx = parseInt(tabIndex);
            idx = isNaN(idx) ? 0 : idx;

            //idx = parseInt(window.location.hash.substring(1));
        }
        $('#tabContact').tabs({
            onSelect: function (title, index) {
                GoContactEdit.switchTabContent(index);
            }
        });
        $("#tabContact").tabs("select", idx);
        GoContactEdit.switchTabContent(idx);
        window.location.hash = '';
    },
    saveContact: function () {

        var toUrl = $(this).attr("href");
        var trackLength = $('input[id^="sal_MTrackHead"]').length;
        var obj = {};

        //联系人类型
        var isCustomer = $("#cbCustomer").is(":checked");
        var isSupplier = $("#cbSupplier").is(":checked");
        var isOther = $("#cbOther").is(":checked");
        if (!isCustomer && !isSupplier && !isOther) {
            $("#spanContactTypeError").show();
            $('#tabContact').tabs("select", 0);
            return;
        }
        obj.MIsCustomer = isCustomer;
        obj.MIsSupplier = isSupplier;
        obj.MIsOther = isOther;
        obj.MIsValidateName = true;
        $("#spanContactTypeError").hide();

        //科目
        if (GoContactEdit.isEnableGL) {

            obj.MCCurrentAccountCode = $("#cbxCCurrentMoney").combobox("getValue");
        }

        obj.MName = $("#txtMName").val();

        for (var i = 1; i <= trackLength; i++) {

            var salTrackValue = $("#sal_MTrackHead" + i).val();
            var purTrackValue = $("#pur_MTrackHead" + i).val();

            var trackId = salTrackValue ? salTrackValue : purTrackValue;

            var salTrackOptionValue = $("#sal_MSalTrackEntry" + i).combobox("getValue");
            var purTrackOptionValue = $("#pur_MPurTrackEntry" + i).combobox("getValue");

            obj["MTrackHead" + i] = trackId;
            obj["MSalTrackEntry" + i] = salTrackOptionValue;
            obj["MPurTrackEntry" + i] = purTrackOptionValue;
        }

        $("body").mFormSubmit({
            url: "/BD/Contacts/ContactsUpdate", param: { model: obj }, callback: function (response) {
                if (response.Success) {
                    var successMsg = GoContactEdit.isUpdate ? HtmlLang.Write(LangModule.Acct, "ContactUpdated", "Customer updated") + ": " + $("#txtMName").val() : HtmlLang.Write(LangModule.Acct, "ContactAdded", "Customer added") + ": " + $("#txtMName").val();
                    $.mMsg(successMsg);
                    if (parent && parent.GoContactsList) {
                        var tabTitle = parent.GoContactsList.getSelectTabIndex();
                        parent.GoContactsList.reload(tabTitle);
                    }
                    else if (parent && parent.ContactView) {
                        parent.ContactView.reload();
                    }
                    else {

                        obj.MItemID = response.ObjectID;

                        parent.$.mDialog.close(0, obj);
                    }
                    $.mDialog.close(0);
                } else {
                    $.mDialog.alert(response.Message);
                }
            }
        });
    },
    saveClick: function () {
        $("#aSave").click(function () {
            GoContactEdit.saveContact(true);
            return false;
        });
    },
    //将combox的选中值默认为空
    clearComboboxValue: function (selector) {
        //调用easyui
        $(selector).combobox("setValue", "");
    },
    //将邮件地址复制到的物理地址的方法
    copyPostal2Physical: function () {
        //绑定click方法
        $("#aCopyPostal").off("click").on("click", function () {
            //目前有6个字段
            var fieldCount = 6;
            //post field class prefix
            var postalClass = "postal";
            //physical field calss prefx
            var pyhsicalClass = "physical";
            //遍历赋值
            for (var i = 1; i <= fieldCount; i++) {
                //postal input
                var $postalInput = $("." + postalClass + i);
                //physical input
                var $physicalInput = $("." + pyhsicalClass + i);
                //多语言信息
                var langData = $postalInput.getLangEditorData();
                if (langData) {
                    //替换FieldName
                    var langDataTemp = jQuery.extend(true, {}, langData);
                    var fieldName = $physicalInput.attr("name");
                    langDataTemp.MFieldName = fieldName;

                    $physicalInput.initLangEditor(langDataTemp);
                }
                else {
                    //赋值
                    $physicalInput.val($postalInput.val());
                }
            }

            //对国家赋值
            var mpCountryId =  $("#selMPCountryID").combobox("getValue");
            $("#selMRealCountryID").combobox("setValue", mpCountryId);
        });
    },
    initCurrenAccount: function (contactType) {

        if (!GoContactEdit.isEnableGL) {
            return;
        }
        $("#cbxCCurrentMoney").combobox("clear");
        var code = "1122,2203,2202,1123,1221,2241";
        /*往来科目的绑定*/
        $("#cbxCCurrentMoney").combobox({
            valueField: 'MCode',
            textField: 'MFullName',
            url: '/BD/BDAccount/GetBDAccountListByCode?IsActive=true&ParentCodes=' + code,
        });
    },
    initTrackEntry: function () {
        var trackEntryDoms = $(".track-entry");

        if (!trackEntryDoms && trackEntryDoms.length <= 0) {
            return;
        }
        var index = 0;
        for (var i = 0 ; i < trackEntryDoms.length; i++) {
            var trackEntryDom = trackEntryDoms[i];
            //父节点
            var parentDom = $(trackEntryDom).parent();

            var trackList = mText.getObject(parentDom.find(".track-data"));
            if (!trackList) {
                continue
            }

            $(trackEntryDom).combobox({
                data: trackList,
                textField: "MEntryName",
                valueField: "MEntryID",
                hideItemKey: "MIsActive",
                hideItemValue: "0",
                onLoadSuccess: function () {
                    var selectedTrackId = parentDom.find(".track-select-optionid").val();
                    if (selectedTrackId) {
                        $(trackEntryDom).combobox("setValue", selectedTrackId);
                    } else {
                        $(trackEntryDom).combobox("setValue", "");
                    }
                }
            });
        }
    }
}

$(document).ready(function () {
    GoContactEdit.init();
});