var PTIndex = {
    CurrentType: 0,
    init: function () {
        //PTIndex.initUI();
        PTIndex.initTab();
        PTIndex.initAction();
    },    
    getTabPanelH: function () {
        var contentH = $(".m-imain").height();
        var tabHeaderH = $(".tabs-header").outerHeight();
        return contentH - tabHeaderH - 10;
    },
    initTab: function (idx) {
        if (!idx) {
            idx = 0;
        }
        PTIndex.CurrentType = idx;
        $('#tabInit').tabs('select', idx);
        PTIndex.bindList(Number(idx) + 1);

        $("#tabInit").tabs({
            height: PTIndex.getTabPanelH(),
            onSelect: function (title, index) {
                PTIndex.CurrentType = index;
                PTIndex.bindList(index + 1);
            },
            onLoad: function (panel) {
                //加载完事件
                $('#tabInit').tabs('resize', {
                    width: PTIndex.getTabPanelH()
                });
                if (PTIndex.CurrentType != 1) {
                    PTIndex.loadLogo();
                }
            }
        });
    },
    loadLogo: function () {
        var psList = $(".print-tmpl:visible");
        $.each(psList, function (idx, item) {
            var psLogo = $(item).find(".ps-logo");
            if (psLogo.length == 1) {
                $.mAjax.Post("/PT/PTBiz/ShowImage", { id: psLogo.attr("id") }, function (img) {
                    if (img) {
                        psLogo.html(img);
                    }
                })
            }
        });
    },
    bindList: function (index) {
        var tabType;
        switch (index)
        {
        	case 1:
        	    tabType = PTTabType.Biz;
        	    break;
            case 2:
                tabType = PTTabType.Voucher;
                break;
            case 3:
                tabType = PTTabType.SalaryList;
                break;
        }
        PTBase.reload(tabType);
    },
    initAction: function () {
        $("#aNewTmpl, #divNewBizTmpl").off("click").on("click", function () {
            PTBase.openDialog(this, HtmlLang.Write(LangModule.Common, "NewBizPrintTmpl", "新增业务单据打印模板"), "/PT/PTBiz/PTBizEdit", true);
        });
        $("#divNewVoucherTmpl").off("click").on("click", function () {
            PTBase.openDialog(this, HtmlLang.Write(LangModule.Common, "NewVoucherPrintTmpl", "新增凭证打印模板"), "/PT/PTVoucher/PTVoucherEdit", true, 900, 420);
        });
        $("#divNewSalaryListTmpl").off("click").on("click", function () {
            PTBase.openDialog(this, HtmlLang.Write(LangModule.Common, "NewSalaryListPrintTmpl", "新增工资单打印模板"), "/PT/PTSalaryList/PTSalaryEdit", true);
        });
    }
}
$(document).ready(function () {
    PTIndex.init();
});