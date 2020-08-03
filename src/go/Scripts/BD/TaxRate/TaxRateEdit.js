
document.domain = $("#hidDomain").val();

var TaxRateEdit = {
    isEnableGL: $("#hideIsEnableGL").val(),
    //是否有权限修改或者删除
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    init: function () {
        TaxRateEdit.bindAction();
        TaxRateEdit.getTaxRate();
    },
    bindAction: function () {
        $("#aOK").click(function () {
            TaxRateEdit.saveTaxRate();
        });
        $("#aNewComponent").click(function () {
            TaxRateEdit.addCompoundItem();
        });
        TaxRateEdit.bindButtonEvent();
    },
    bindButtonEvent: function () {
        $('#txtTaxRateName').validatebox({ required: true });
        $(".m-icon-delete").unbind();
        $("input[name='IsCompound']").unbind();
        $(".m-icon-delete").click(function () {
            $(this).parent().parent().remove();
            TaxRateEdit.setDeleteItemButton();
        });
        $("input[name='IsCompound']").click(function () {
            TaxRateEdit.calTaxRate();
            var value = $(this).val();
            $("input[name='IsCompound']").val(false);
            if (value == "true") {
                $(this).attr("checked", false);
                $(this).val(false);
            } else {
                $(this).attr("checked", 'checked');
                $(this).val(true);
            }
            TaxRateEdit.bindTaxRate();
        });
    },
    bindTaxRate: function () {
        $(".component-percent").unbind();
        $('.component-percent').numberbox({
            min: 0,
            precision: 2,
            width: 45,
            height: 22,
            onChange: function () {
                TaxRateEdit.calTaxRate();
            }
        });
        $('.component-percent').keyup(function () {
            TaxRateEdit.calTaxRate();
        });
        TaxRateEdit.calTaxRate();
        TaxRateEdit.bindComponentName();
        Megi.regClickToSelectAllEvt();
    },
    bindComponentName: function () {
        $(".component-name").validatebox({
            required: true
        });
    },
    calTaxRate: function () {
        var total = 0;
        var effRate = 1;
        var totalEffRate = 0;

        $('.component-percent').each(function () {
            var value = $(this).val();
            var v = value == "" ? 0 : Number(value);
            total += v;
            effRate = v * effRate;
        });
        var isCcaEffRate = false;
        $("input[name='IsCompound']").each(function () {
            if ($(this).attr("checked") == "checked") {
                isCcaEffRate = true;
            }
        });

        totalEffRate = Megi.Math.toDecimal(effRate / 100 + total);
        total = Megi.Math.toDecimal(total);

        if (isCcaEffRate) {
            $("#divEffectiveTaxRate").show();
            $("#effectRate").html(totalEffRate + "%");
        } else {
            $("#divEffectiveTaxRate").hide();
            $("#effectRate").html("0.00%");
        }

        $("#totalRate").html(total + "%");
    },
    saveTaxRate: function () {
        if (!$(".m-imain").mFormValidate()) {
            return;
        }
        var name = $("#txtTaxRateName").val();
        var rateCode = $("#hidRateCode").val();
        var isSysData = !!$("#hideIsSysData").val()?$("#hideIsSysData").val():false;
        var detaillist = new Array();
        $(".m-tax-component>ul>li").each(function () {
            var detailObj = {};
            detailObj.MName = $(this).find(".component-name").val();
            detailObj.MIsCompound = $(this).find("input[type='radio']").attr("checked") == "checked" ? true : false;
            detailObj.MTaxRate = $(this).find(".component-percent").val();


            detaillist.push(detailObj);
        });


        for (var i = 0 ; i < detaillist.length; i++) {
            //var taxRate = detaillist[i].MTaxRate;

            //if (taxRate > 100) {
            //    var tips = HtmlLang.Write(LangModule.BD, "TaxRateGreateThan100", "税率不能大于100%");
            //    $.mDialog.alert(tips);
            //    return;
            //}

            //if (taxRate < 0) {
            //    var tips = HtmlLang.Write(LangModule.BD, "TaxRateLessThan0", "税率不能小于0%");
            //    $.mDialog.alert(tips);
            //    return;
            //}
        }

        var dataLang = $("#txtTaxRateName").getLangEditorData();

        var langArray = new Array();

        langArray.push(dataLang);

        var obj = {};
        obj.MItemID = rateCode;
        obj.TaxRateDetail = detaillist;
        obj.MultiLanguage = langArray;
        obj.MIsSysData = isSysData;
        if (TaxRateEdit.isEnableGL) {

            obj.MSaleTaxAccountCode = $("#cbxIA").combobox("getValue");;
            obj.MPurchaseAccountCode = $("#cbxInA").combobox("getValue");;
            obj.MPayDebitAccountCode = $("#cbxCA").combobox("getValue");;
        }
        $("body").mFormSubmit({
            url: "/BD/TaxRate/SaveTaxRate", validate: true, param: { paramModel: obj }, callback: function (msg) {
                var successMsg = rateCode.length > 0 ? HtmlLang.Write(LangModule.Bank, "TaxRateUpdated", "Tax Rate updated") + ": " + name : HtmlLang.Write(LangModule.Bank, "TaxRateAdded", "Tax Rate added") + ": " + name;
                if (msg.Success) {
                    $.mDialog.message(successMsg);
                    if (parent && parent.TaxRateSetting) {
                        parent.TaxRateSetting.reload();
                    }
                    if (!rateCode) {
                        obj.MItemID = msg.ObjectID;
                    }
                    $.mDialog.close(obj);
                }
                else {
                    if (!msg.Message) {
                        msg.Message = HtmlLang.Write(LangModule.Bank, "TaxRateAddFail", "Add taxrate fail , please check contents you input is correct!");
                    }
                    $.mDialog.alert(msg.Message);

                }
            }
        });
    },
    getTaxRate: function () {
        var rateCode = $("#hidRateCode").val();
        var detailHtml = '<li>'
        //detailHtml += '  <input class="easyui-validatebox textbox component-name" style="height:23px;display:none;" data-options="required:true"/>';
        detailHtml += '<span class="m-Compound" style="display:none">';
        detailHtml += ' <label style="display:none;">';
        detailHtml += '<input type="radio" name="IsCompound" value="false" />' + HtmlLang.Write(LangModule.BD, "CompoundApplyToTaxedSubtotal", "Compound Apply To Taxed Subtotal") + '&nbsp';
        detailHtml += '</label>';
        detailHtml += '</span>';
        detailHtml += '<span><input class="easyui-validatebox textbox component-percent" style="height:22px;line-height:22px" data-options="required:true"/>%</span><span><a href="javascript:void(0)" class="m-icon-delete" style="display:none;">\&nbsp;</a></span><br class="clear"/></li>';
        if (rateCode == "") {
            $(".m-tax-component>ul").html(detailHtml);
            TaxRateEdit.bindTaxRate();
            return;
        }
        var obj = {};
        obj.KeyIDs = $("#hidRateCode").val();
        mAjax.post(
            "/BD/TaxRate/GetTaxRate",
            { param: obj },
            function (msg) {
                if (msg != null) {
                    $("#txtTaxRateName").val(mText.htmlEncode(msg.MName));
                    var dataLang = msg.MultiLanguage;
                    $("#txtTaxRateName").initLangEditor(dataLang[0]);

                    //是否系统预设的税率
                    var isSysRate = msg.MIsSysData;

                    $("#hideIsSysData").val(isSysRate);
                    var detailName = "";
                    if (msg.TaxRateDetail.length > 0) {
                        detailHtml = '';
                        for (var i = 0; i < msg.TaxRateDetail.length; i++) {
                            var detailItem = msg.TaxRateDetail[i];
                            detailName = detailItem.MName;
                            //detailHtml += '<li><input style="height:24px" class="textbox component-name" value=""><span class="m-Compound" style="display:none"><label><input type="radio" name="IsCompound"' + (detailItem.MIsCompound == true ? ' checked="checked" value="true"' : 'value="false"') + '/>' + HtmlLang.Write(LangModule.Bank, "CompoundApplyToTaxedSubtotal", "Compound Apply To Taxed Subtotal") + '&nbsp;</label></span><span><input class="textbox component-percent" value="' + detailItem.MTaxRate + '" style="height: 22px;" />%</span><span><a href="javascript:void(0)" class="m-icon-delete">&nbsp;</a></span></li>';
                            detailHtml += '<li><span><input class="textbox component-percent" value="' + detailItem.MTaxRate + '" style="height: 22px;" />%</span><span><a href="javascript:void(0)" class="m-icon-delete">&nbsp;</a></span></li>';
                        }
                    }
                }
                $(".m-tax-component>ul").html(detailHtml);

                $(".component-name").each(function () {
                    $(this).val(detailName);
                });

                if (TaxRateEdit.isEnableGL) {
                    $("#cbxIA").combobox("setValue", msg.MSaleTaxAccountCode);
                    $("#cbxInA").combobox("setValue", msg.MPurchaseAccountCode);
                    $("#cbxCA").combobox("setValue", msg.MPayDebitAccountCode);
                }

                //如果是系统的税率，不允许更改名称
                if (isSysRate) {
                    $("#txtTaxRateName").attr("disabled", "disabled");

                    $("#MNameLangDiv").find("input:not(.m-lang-ok)").each(function () {
                        $(this).attr("disabled", "disabled");
                    });

                    $(".component-percent").attr("disabled", "disabled");
                }

                TaxRateEdit.setDeleteItemButton();
                TaxRateEdit.bindTaxRate();
                TaxRateEdit.bindButtonEvent();
            });
    },
    setDeleteItemButton: function () {
        if ($(".m-tax-component>ul>li").length == 1) {
            $(".m-tax-component").find(".m-icon-delete").hide();
            $(".m-tax-component").find("label").hide();
            $(".m-Compound>label").hide();
        } else {
            $(".m-tax-component").find(".m-icon-delete").show();
            $(".m-tax-component").find("label").show();
            $(".m-Compound>label").show();
        }
    },
    addCompoundItem: function () {
        var html = '<li><input class="textbox component-name" /><span class="m-Compound" style="display:none"><label><input type="radio" name="IsCompound" value="false" />' + HtmlLang.Write(LangModule.BD, "CompoundApplyToTaxedSubtotal", "Compound Apply To Taxed Subtotal") + '</label>&nbsp;</span><span><input class="textbox component-percent" style="hegith:22px;line-height:22px;"/>%</span><span><a href="javascript:void(0)" class="m-icon-delete">&nbsp;</a></span><br class="clear"/></li>';
        $(".m-tax-component>ul").append(html);
        TaxRateEdit.setDeleteItemButton();
        TaxRateEdit.bindButtonEvent();
        TaxRateEdit.bindTaxRate();

        $('.m-imain').scrollTop($('.m-imain')[0].scrollHeight);
    }
}

$(document).ready(function () {
    TaxRateEdit.init();
});