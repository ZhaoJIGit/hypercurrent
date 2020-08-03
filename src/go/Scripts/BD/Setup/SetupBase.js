var SetupStepEnum = { OrgSetting: "OrgSetting", FinancialSetting: "FinancialSetting", TaxRateSetting: "TaxRateSetting", GLChartOfAccount: "GLChartOfAccount", GLOpeningBalance: "GLOpeningBalance", GLFinish: "GLFinish", GLSuccess: "GLSuccess" };
var SetupBase = {
    goServer: $("#hidGoServer").val(),
    tabSwitch: new BrowserTabSwitch(),
    showMask: function (delayUnmask) {
        //
        return false;
        var parentIfr = $("iframe:visible", parent.document);
        //parentIfr.parent().mask("");  
        top.$("body").find(".m-tab-content:visible").mask("");
        try {
            //parentIfr.contents().find("body").unmask();
            if (delayUnmask) {
                var interval = setInterval(function () {
                    var loading = parentIfr.contents().find(".loadmask-msg-i");
                    if (loading.length == 1) {
                        top.$("body").find(".m-tab-content:visible").mask("");
                        clearInterval(interval);
                    }
                }, 200);
            }
        }
        catch (ex) {

        }
    },
    showParentMask: function () {
        top.$("body").find(".m-tab-content:visible").mask("");
    },
    hideMask: function () {
        top.$("body").find(".m-tab-content:visible").unmask();
    },
    saveOrgDetail: function (toUrl, isSaveAndQuit) {
        if (OrgEdit.validateForm()) {
            var displayName = $("#txtDisplayName").val();
            var url = "";
            if (SetupBase.goServer) {
                url = SetupBase.goServer + "/BD/Organisation/IsOrgExist"
            } else {
                url = "/BD/Organisation/IsOrgExist"
            }

            mAjax.post(
                url,
                { displayName: displayName, excludeId: $("#hidOrgCode").val() },
                function (msg) {
                    if (msg === "true" || !$("#hidOrgCode").val() && $.trim(displayName) == "Demo Company") {
                        $.mDialog.confirm(HtmlLang.Write(LangModule.Org, "AlreadyExistOrg", "This Organisation Display Name already exist, are you continue anyway?"),
                        {
                            callback: function () {
                                SetupBase.submitOrgDetail(toUrl, isSaveAndQuit);
                            }
                        });
                    }
                    else {
                        SetupBase.submitOrgDetail(toUrl, isSaveAndQuit);
                    }
                });
        }
    },
    submitOrgDetail: function (toUrl, isSaveAndQuit) {
        if (toUrl != undefined) {
            //SetupBase.showMask();
        }
        var strUrl = toUrl != undefined ? "/BD/Setup/UpdateOrgDetail" : "/BD/Organisation/UpdateOrgDetail";
        $("body").mFormSubmit({
            url: strUrl, param: { model: {} }, callback: function (msg) {
                if (msg.Success) {
                    SetupBase.saveSysLang(toUrl, isSaveAndQuit);
                    //同步更新组织名
                    if (toUrl == undefined) {
                        var aCurOrg = $("#aOrgList", top.document);
                        var newOrgName = $("#txtDisplayName").val();
                        if (aCurOrg.text() != newOrgName) {
                            aCurOrg.text(newOrgName);
                        }
                    } else {
                        //更新当前组织信息和cookie
                        if (msg.ObjectID) {
                            SetupBase.tabSwitch.setBrowserTabOrgId(msg.ObjectID);
                        }


                        //$.cookie('MOrgID', result.ObjectID);
                    }
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    },
    goToUrl: function (toUrl, isSaveAndQuit) {
        if (toUrl) {
            if (isSaveAndQuit) {
                top.mWindow.reload(toUrl);
            }
            else {
                mWindow.reload(toUrl);
            }
        }
        else {
            $.mMsg(HtmlLang.Write(LangModule.Org, "UpdateSuccessful", "Update successfully."));
            mWindow.reload();
        }
    },
    saveSysLang: function (toUrl, isSaveAndQuit) {
        //不允许修改多语言
        if ($("#hidOrgCode").val()) {
            SetupBase.goToUrl(toUrl, isSaveAndQuit);
            return;
        }

        var checkSel = $('input[name="language"]:checked');
        var str = "";
        checkSel.each(function (i) {
            if (i == (checkSel.length - 1)) { str += $(this).val(); }
            else { str += $(this).val() + ","; }
        });
        $("#hidLangIds").val(str);
        $("#hidUpdateFields").val("MSystemLanguage");
        $("body").mFormSubmit({
            url: "/BD/Organisation/GlobalizationUpdate", param: { model: {} }, callback: function (msg) {
                if (msg.Success) {
                    if (toUrl == undefined) {

                        var title = HtmlLang.Write(LangModule.Common, "globalizationSaveSuccessfully", "更新成功,页面将会刷新，请确认所有数据已保存，您是否确认刷新页面?")
                        //
                        top.$.mDialog.confirm(title, function () {
                            //
                            top.window.location = top.mWindow.getOrigin();
                        });
                    } else {
                        SetupBase.showParentMask();
                        SetupBase.goToUrl(toUrl, isSaveAndQuit);
                    }
                    return;
                } else {
                    $.mDialog.alert(msg.Message);
                    SetupBase.hideMask();
                }
            }
        });
    },
    saveGlobalization: function (successCallBack) {
        var obj = {};
        obj.IsUpdate = true;
        obj.MSystemZone = 'China Standard Time';
        obj.MSystemDate = 'yyyy-MM-dd';
        obj.MSystemTime = 'HH:mm:ss';
        obj.MSystemDigitDot = '.';
        obj.MSystemDigitGroupingSymbol = ',';
        obj.MSystemDigitNegative = '-';
        obj.MIsShowCSymbol = true;

        $("body").mFormSubmit({
            url: "/BD/Organisation/GlobalizationUpdate", param: { model: obj }, callback: function (msg) {
                if (msg.Success) {
                    SetupBase.showParentMask();
                    if (successCallBack) {
                        successCallBack();
                    }
                    return;
                } else {
                    $.mDialog.alert(msg.Message);
                    SetupBase.hideMask();
                }
            }
        });
    },
    initOrgForm: function () {
        var orgCode = $("#hidOrgCode").val();
        if (orgCode == undefined || orgCode.length > 0) {
            $("body").mFormGet({
                url: "/BD/Organisation/GetOrgDetail", fill: true, callback: function (data) {
                    OrgEdit.loadProvinceList(data);
                    OrgEdit.initSysLang(data.GlobalizationModel);
                }
            });
        }
        if (orgCode != undefined && orgCode.length == 0) {
            OrgEdit.initDefaultValue();
        }
    },
    initGlobalization: function () {
        $("body").mFormGet({ url: "/BD/Organisation/GetOrgGlobalizationDetail" });
    },
    initFW: function () {
        if ($(".m-imain").length > 0) {
            var ifH = parent.$("body").find("iframe").height();
            var btnContainerH = 0;
            if ($(".m-toolbar-footer").length > 0) {
                btnContainerH = $(".m-toolbar-footer").outerHeight();
            }
            var h = ifH - btnContainerH - ($(".m-imain").outerHeight() - $(".m-imain").height());
            $(".m-imain").css({ "height": h + "px" });
        }
    },
    checkInitBalanceAndRedirect: function (toUrl) {
        mAjax.post(
            Setup.goServer + "/BD/BDAccount/CheckTrialInitBalance",
            null,
            function (response) {
                if (response.Success) {
                    mAjax.submit(
                        "/BD/InitSetting/GLSetupSuccess",
                        null,
                        function (msg) {
                            if (msg.Success) {
                                mWindow.reload(toUrl);
                            } else {
                                $.mDialog.alert(msg.Message);
                                //SetupBase.hideMask();
                            }
                        });
                }
                else {
                    //SetupBase.hideMask();
                    Megi.dialog({
                        title: HtmlLang.Write(LangModule.Acct, "TrialBalance", "Trial balance check"),
                        width: 500,
                        height: 230,
                        href: '/BD/BDAccount/TrialInitBalance'
                    });
                }
            });
    },
    checkCustomAcctIsFinish: function () {
        $.mAjax.Post("/BD/BDAccount/CheckCustomAccountIsFinish", {}, function (msg) {
            if (msg && msg.Success) {
                //检测成功后，看是否进行了匹配
                $.mAjax.post("/BD/BDAccount/CheckCustomAccountIsMatch", {}, function (msg) {
                    if (msg && msg.Success) {
                        Setup.save(Setup.getNextUrl());
                    } else {
                        //var msg = HtmlLang.Write(LangModule.BD, "CustomAccountNotMatch", "自定义科目还没有进行匹配，请先进行匹配");
                        $.mDialog.alert(msg.Message);
                        AccountList.accountMatchDialog();
                    }
                });
            } else {
                var msg = HtmlLang.Write(LangModule.BD, "NotFindAnyCustomAccount", "自定科目还没有生成，请先生成自定义科目");
                $.mDialog.alert(msg);

                AccountList.createAccountDialog();
            }
        }, "", true);
    },
    adjustGridHeight: function () {
        $(".setup-body").css("overflow-y", "hidden");
        var bodyH = $(".setup-body").height();
        var toolH = $(".m-toolbar").outerHeight();
        var tabH = $("#tabAccount").height();
        var tabToolH = $(".m-tab-toolbar").height();
        var actionH = $(".m-toolbar-footer").height();
        var gridH = bodyH - toolH - tabH - tabToolH - actionH;

        $(".datagrid-wrap").height(gridH);
        $(".datagrid-view").height(gridH);
        var headerH = $(".datagrid-view2 .datagrid-header").height();
        $(".datagrid-view2 .datagrid-body").height(gridH - headerH);
    }
}
