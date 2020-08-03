var Setup = {
    progress: $("#hidProgress").val(),
    goServer: $("#hidGoServer").val(),
    myServer: $("#hidMyServer").val(),
    interval: null,
    tabSwitch: new BrowserTabSwitch(),
    arrStep: [SetupStepEnum.OrgSetting, SetupStepEnum.FinancialSetting, SetupStepEnum.TaxRateSetting, SetupStepEnum.GLChartOfAccount, SetupStepEnum.GLOpeningBalance, SetupStepEnum.GLFinish, SetupStepEnum.GLSuccess],
    init: function () {
        if ($.browser.msie && parseInt($.browser.version) === 8 || $.browser.safari) {
            $('.setup-content').css('width', '100%').css('width', '-=210px');
        }
        //初始化组织ID
        Setup.initOrgId();

        Setup.initUI();
        SetupBase.hideMask();
        Setup.initProgressButton();
        Setup.initAction();
        if (Setup.progress == SetupStepEnum.GLFinish) {
            SetupBase.initGlobalization();
        }
        switch (Setup.progress) {
            case SetupStepEnum.OrgSetting:
                $("#aPrevious").hide();
                break;
            case SetupStepEnum.GLChartOfAccount:
                $("#txtKeyword").css({ "height": "25px", "line-height": "25px" });
                $("#aPrevious").show();
                $("#popup_container").width(425);
                $("#popup_message").css("text-align", "left");
                $("#popup_container .alert").css("background-position", "180px 0");
                break;
            default:
                $("#aPrevious").show();
                break;
        }

        //绑定页签变化事件
        Setup.tabSwitch.bindSwitchEvent();
    },
    initUI: function () {
        var w = $(".setup-body").width();
        switch (Setup.progress) {
            case SetupStepEnum.TaxRateSetting:
                $(".m-setup-content").css("padding-left", "0px");
                //try {
                //    $("#tbTax").datagrid('resize', {
                //        width: 710
                //    });
                //} catch (exc) { }
                //$(".m-setup-content").width(730);
                break;
            case SetupStepEnum.GLChartOfAccount:
                Setup.adjustUI(w);

                $(".easyui-tabs .tabs-wrap").width(w);
                $(".m-tab-toolbar").width(w - 60);
                $(".tool-bar-search").width(253);
                try {
                    $("#tbAll").datagrid('resize', {
                        width: w - 50
                    });
                } catch (exc) { }
                break;
            case SetupStepEnum.GLOpeningBalance:
                Setup.adjustUI(w);

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
    adjustUI: function (w) {
        $(".m-setup-content").css("padding-left", "0px");
        $(".m-setup-content").width(w - 10);
        $(".m-toolbar").width(w - 70);
    },
    //向导页面加载都需要清掉组织id
    initOrgId: function () {

        var currentOrgId = $("#hidOrgCode").val();

        Setup.tabSwitch.setBrowserTabOrgId(currentOrgId);
    },
    initAction: function () {
        $("#aSaveAndQuit").click(function () {
            Setup.save(Setup.myServer);
        });
        $("#aNext").click(function () {
            //这里做一些判断
            //如果在科目阶段，自定义科目没有导入，后者，没有进行匹配，不允许调过这一步
            //先检查是否完成了导入
            if (Setup.progress == SetupStepEnum.GLChartOfAccount && AccountList.AccountStandard == 3) {
                SetupBase.checkCustomAcctIsFinish();
            } else {
                Setup.save(Setup.getNextUrl());
            }
        })
        $("#aPrevious").click(function () {
            mWindow.reload(Setup.getPreviousUrl());
        });
        $("#aSkip").click(function () {
            mWindow.reload(Setup.getNextUrl());
        });
        $(".setup-step ul li a").click(function () {
            //SetupBase.showMask();
        });
    },
    save: function (toUrl) {
        var isSaveAndQuit = toUrl == Setup.myServer;
        switch (Setup.progress) {
            case SetupStepEnum.OrgSetting:
                SetupBase.saveOrgDetail(toUrl, isSaveAndQuit);
                break;
            case SetupStepEnum.FinancialSetting:
                //SetupBase.showMask();
                FinancialEdit.saveFinancial(toUrl, isSaveAndQuit);
                break;
            case SetupStepEnum.TaxRateSetting:
                //SetupBase.showMask();
                if (isSaveAndQuit) {
                    top.mWindow.reload(toUrl);
                }
                else {
                    mWindow.reload(toUrl);
                }
                break;
            case SetupStepEnum.GLChartOfAccount:
            case SetupStepEnum.GLOpeningBalance:
                mWindow.reload(toUrl);
                break;
            case SetupStepEnum.GLFinish:
                //SetupBase.showMask();
                SetupBase.saveGlobalization(function () {
                    top.mWindow.reload(toUrl);
                });
                break;
        }
    },
    initProgressButton: function () {
        var steps = $(".setup-step ul li");
        var currentStepIdx = Setup.arrStep.indexOf(Setup.progress);
        if (currentStepIdx < steps.length) {
            steps.removeClass("current");
            $(steps.get(currentStepIdx)).addClass("current");
        }
        if (currentStepIdx > 0) {
            $.each(steps, function (idx, val) {
                if (idx < currentStepIdx) {
                    $(steps.get(idx)).addClass("done");
                    var lbl = $(steps.get(idx)).find("span");
                    lbl.html("<a href='javascript:void(0)'>" + lbl.text() + "</a>");
                }

                $("a", this).off("click").on("click", function () {
                    var url = Setup.getProcessUrl(Setup.arrStep[idx]);
                    mWindow.reload(url);
                })
            });
        }

        var divProgress = $(".setup-progress");
        divProgress.text(divProgress.text().replace("{0}", currentStepIdx + 1).replace("{1}", steps.length));
        divProgress.show();

        var title = $(".setup-step li.current").text();
        if (Setup.progress == SetupStepEnum.GLFinish) {
            title = HtmlLang.Write(LangModule.BD, "AlmostDone", "Almost done!");
        }
        $(".m-imain-title").text(title);
    },
    getProcessUrl: function (progress) {
        return Setup.goServer + "/BD/Setup/" + progress;
    },
    getPreviousUrl: function () {
        var stepIdx = Setup.arrStep.indexOf(Setup.progress);
        return Setup.goServer + "/BD/Setup/" + Setup.arrStep[stepIdx - 1];
    },
    getNextUrl: function () {
        var stepIdx = Setup.arrStep.indexOf(Setup.progress);
        return Setup.goServer + "/BD/Setup/" + Setup.arrStep[stepIdx + 1];
    }
}
$(document).ready(function () {
    Setup.init();
});