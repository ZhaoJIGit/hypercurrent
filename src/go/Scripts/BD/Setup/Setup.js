var SetupProgress = { OrgSetting: "OrgSetting", FinancialSetting: "FinancialSetting", TaxRateSetting: "TaxRateSetting", Finish: "Finish" };
var GLSetupProgress = { GLBasicInfo: "GLBasicInfo", GLChartOfAccount: "GLChartOfAccount", GLOpeningBalance: "GLOpeningBalance", GLFinish: "GLFinish" };
var Module = { Sales: 1, GL: 2 };
var Setup = {
    progress: $("#hidProgress").val(),
    progressVal: parseInt($("#hidProgressVal").val()),
    goServer: $("#hidGoServer").val(),
    myServer: $("#hidMyServer").val(),
    moduleId: parseInt($("#hidModuleId").val()),
    interval: null,
    init: function () {

        if ($.browser.msie && parseInt($.browser.version) === 8 || $.browser.safari) {
            $('.setup-content').css('width', '100%').css('width', '-=210px');
        }
        if (Setup.moduleId == Module.GL) {
            $(".m-setup .setup-step").width(169);
            $('.setup-content').css('width', '100%').css('width', '-=182px');
        }
        Setup.initUI();
        SetupBase.hideMask();
        Setup.initProgressButton(Setup.moduleId);
        Setup.initAction();
        if (Setup.progress == SetupProgress.Finish) {
            SetupBase.initGlobalization();
        }
        switch (Setup.progress) {
            case SetupProgress.OrgSetting:
            case GLSetupProgress.GLBasicInfo:
                $("#aPrevious").hide();
                break;
            case GLSetupProgress.GLChartOfAccount:
                $("#txtKeyword").css({"height": "25px", "line-height": "25px"});
                $("#aPrevious").show();
                //var msg = HtmlLang.Write(LangModule.Acct, "ImportAcctWarning1", "1).银行科目不能直接导入，需在银行账户页面手动新增！<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarning2", "2).初始化完成后，已有的会计科目不能再次导入，如需修改，只能在会计科目表里手动操作！<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarning3", "3).每个科目都可以设置辅助核算维度，这个维度包括联系人、员工、商品项目、费用项目、工资项目和跟踪项。以下为各个科目的默认维度，不能取消：<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarninga", "a) 应收默认选择维度联系人，不能出现员工、费用项目和工资项目<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarningb", "b) 预收默认选择维度联系人，不能出现员工、费用项目和工资项目<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarningc", "c) 应付默认选择维度联系人，不能出现员工、费用项目和工资项目<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarningd", "d) 预付默认选择维度联系人，不能出现员工、费用项目和工资项目<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarninge", "e) 其他应收默认选择联系人，可取消<br>");
                //msg += HtmlLang.Write(LangModule.Acct, "ImportAcctWarningf", "f)其他应付默认选择联系人和员工，可取消");



                //$.mDialog.alert(msg, undefined, 0, true);
                $("#popup_container").width(425);
                $("#popup_message").css("text-align", "left");
                $("#popup_container .alert").css("background-position", "180px 0");
                break;
            default:
                $("#aPrevious").show();
                break;
        }

        //if (isHide) {
        //    containner.hide();
        //}
    },
    initUI: function () {
        var w = $(".setup-body").width();
        switch (Setup.progress) {
            case SetupProgress.TaxRateSetting:
            case GLSetupProgress.GLChartOfAccount:
            case GLSetupProgress.GLOpeningBalance:
                $(".m-setup-content").css("padding-left", "0px");
                break;
        }
        if (Setup.moduleId == Module.GL) {
            $(".m-setup-content").width(w - 10);
            $(".m-toolbar").width(w - 70);
        }
        switch (Setup.progress) {
            case SetupProgress.TaxRateSetting:
                try {
                    $("#tbTax").datagrid('resize', {
                        width: 710
                    });
                } catch (exc) { }
                $(".m-setup-content").width(730);
                break;
            case GLSetupProgress.GLChartOfAccount:
                $(".easyui-tabs .tabs-wrap").width(w);
                $(".m-tab-toolbar").width(w - 60);
                $(".tool-bar-search").width(253);
                try {
                    $("#tbAll").datagrid('resize', {
                        width: w - 50
                    });
                } catch (exc) { }
                break;
            case GLSetupProgress.GLOpeningBalance:
                w = w - 50;
                $(".tool-bar-action").width(w);
                try {
                    $("#tbAccountBalance").datagrid('resize', {
                        width: w
                    });
                } catch (exc) { }
                break;
        }
    },
    initAction: function () {
        $("#aSaveAndQuit").click(function () {
            Setup.save(Setup.myServer);
        });
        $("#aNext").click(function () {
            //这里做一些判断
            //如果在科目阶段，自定义科目没有导入，后者，没有进行匹配，不允许调过这一步
            //先检查是否完成了导入
            if (Setup.progress == GLSetupProgress.GLChartOfAccount && AccountList.AccountStandard == 3) {
                SetupBase.checkCustomAcctIsFinish();
            } else {
                Setup.save(Setup.getNextUrl());
            }
        });
        $("#aPrevious").click(function () {
            mWindow.reload(Setup.getPreviousUrl());
        });
        $("#aSkip").click(function () {
            mWindow.reload(Setup.getNextUrl());
        });
        $("#aGoDashboard").click(function () {
            MegiTrack.MixPanel.track('Started Trial');
            MegiTrack.Intercom.track('Started Trial');
            Setup.save(Setup.getNextUrl());
        });
        $("#aEnableGL").click(function () {
            $.mDialog.confirm(HtmlLang.Write(LangModule.BD, "EnableGLConfirm", "To enable general ledger，you will need to complete its setting first．By click \"Yes\" we will take you to the GL setting page."),
            {
                callback: function () {
                    MegiTrack.MixPanel.track('Started Trial');
                    MegiTrack.Intercom.track('Started Trial');
                    Setup.save(Setup.getNextUrl(true), Module.GL);
                }
            });
        });
        $(".setup-step ul li a").click(function () {
            //SetupBase.showMask();
        });
    },
    save: function (toUrl, targetModuleId) {
        var isSaveAndQuit = toUrl == Setup.myServer;
        if (targetModuleId == undefined && Setup.moduleId == Module.Sales) {
            if (Setup.progress == SetupProgress.OrgSetting) {
                SetupBase.saveOrgDetail(toUrl, isSaveAndQuit);
            }
            else if (Setup.progress == SetupProgress.FinancialSetting) {
                //SetupBase.showMask();
                FinancialEdit.saveFinancial(toUrl, isSaveAndQuit);
            }
            else if (Setup.progress == SetupProgress.TaxRateSetting) {
                //SetupBase.showMask();
                if (isSaveAndQuit) {
                    top.mWindow.reload(toUrl);
                }
                else {
                    mWindow.reload(toUrl);
                }
            }
            else if (Setup.progress == SetupProgress.Finish) {
                MegiTrack.MixPanel.track('Started Trial');
                MegiTrack.Intercom.track('Started Trial');
                //SetupBase.showMask();
                SetupBase.finish();
            }
        }
        else if (targetModuleId == Module.GL || Setup.moduleId == Module.GL) {
            if (Setup.progress != GLSetupProgress.GLBasicInfo) {
                //SetupBase.showMask();
            }
            if (targetModuleId == Module.GL) {
                SetupBase.finish(true);
            }
            else {
                if (Setup.progress == GLSetupProgress.GLBasicInfo) {
                    GLBasicInfoEdit.save(toUrl, isSaveAndQuit);
                }
                else if (Setup.progress == GLSetupProgress.GLOpeningBalance) {
                    MegiTrack.MixPanel.track('Completed GL Setup');
                    mAjax.submit(
                        "/BD/InitSetting/GLSetupSuccess",
                        null,
                        function (msg) {
                            if (msg.Success) {
                                SetupBase.showParentMask();
                                top.mWindow.reload(toUrl);
                            } else {
                                $.mDialog.alert(msg.Message);
                                SetupBase.hideMask();
                            }
                        });
                }
                else {
                    SetupBase.showParentMask();
                    location = Setup.getNextUrl();
                }
            }
        }
    },
    getModuleProgress: function (moduleId) {
        switch (moduleId) {
            case Module.Sales:
                return [SetupProgress.OrgSetting, SetupProgress.FinancialSetting, SetupProgress.TaxRateSetting, SetupProgress.Finish];
            case Module.GL:
                return [GLSetupProgress.GLBasicInfo, GLSetupProgress.GLChartOfAccount, GLSetupProgress.GLOpeningBalance];//,GLSetupProgress.GLFinish
        }
    },
    initProgressButton: function (moduleId) {
        var steps = $(".setup-step ul li");
        var curProgressVal = Setup.progressVal % 10;
        if (curProgressVal <= steps.length) {
            steps.removeClass("current");
            $(steps.get(curProgressVal - 1)).addClass("current");
        }
        var arrProgress = Setup.getModuleProgress(moduleId);
        var totalStep = arrProgress.length;
        if (curProgressVal > 1) {
            $.each(steps, function (idx, val) {
                if (idx < curProgressVal - 1) {
                    $(steps.get(idx)).addClass("done");
                    var lbl = $(steps.get(idx)).find("span");
                    lbl.html("<a href='" + Setup.getProcessUrl(arrProgress[idx]) + "'>" + lbl.text() + "</a>");
                }
            });
        }
        var curProgress = curProgressVal == totalStep ? 99 : Math.floor(curProgressVal / totalStep * 100);
        if (Setup.moduleId == Module.Sales) {
            $(".setup-progress").append(curProgress + '%');
        }
        var title = $(".setup-step li.current").text();
        if (Setup.progress == SetupProgress.Finish) {
            title = HtmlLang.Write(LangModule.BD, "AlmostDone", "Almost done!");
        }
        $(".m-imain-title").text(title);
    },
    getProcessUrl: function (progress) {
        if (Setup.moduleId == Module.Sales) {
            switch (progress) {
                case SetupProgress.OrgSetting:
                    return "/BD/Setup/OrgSetting";
                case SetupProgress.FinancialSetting:
                    return "/BD/Setup/FinancialSetting";
                case SetupProgress.TaxRateSetting:
                    return "/BD/Setup/TaxRateSetting";
            }
        }
        else if (Setup.moduleId == Module.GL) {
            return "/BD/Setup/" + progress;
        }
    },
    getPreviousUrl: function () {
        switch (Setup.progress) {
            case SetupProgress.FinancialSetting:
                url = "/BD/Setup/OrgSetting";
                break;
            case SetupProgress.TaxRateSetting:
                url = "/BD/Setup/FinancialSetting";
                break;
            case SetupProgress.Finish:
                url = "/BD/Setup/TaxRateSetting";
                break;

            case GLSetupProgress.GLChartOfAccount:
                url = "/BD/Setup/" + GLSetupProgress.GLBasicInfo;
                break;
            case GLSetupProgress.GLOpeningBalance:
                url = "/BD/Setup/" + GLSetupProgress.GLChartOfAccount;
                break;
        }
        return Setup.goServer + url;
    },
    getNextUrl: function (isEnableGL) {
        var url = '';
        switch (Setup.progress) {
            case SetupProgress.OrgSetting:
                url = "/BD/Setup/FinancialSetting";
                break;
            case SetupProgress.FinancialSetting:
                url = "/BD/Setup/TaxRateSetting";
                break;
            case SetupProgress.TaxRateSetting:
                url = "/BD/Setup/Finish";
                break;
            case SetupProgress.Finish:
                url = isEnableGL ? "/BD/Setup/GLBasicInfo" : "/BD/Setup/CreateOrgSuccess";
                break;
                /*总帐*/
            case GLSetupProgress.GLBasicInfo:
                url = "/BD/Setup/GLChartOfAccount";
                break;
            case GLSetupProgress.GLChartOfAccount:
                url = "/BD/Setup/GLOpeningBalance";
                break;
            case GLSetupProgress.GLOpeningBalance:
                url = "/BD/Setup/GLSuccess";
                break;
        }
        return Setup.goServer + url;
    }
}
$(document).ready(function () {
    Setup.init();
});