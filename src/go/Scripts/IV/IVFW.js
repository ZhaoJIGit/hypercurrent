//高级选项切换
var IVFW = {
    init: function () {
        IVFW.initAdvSearch();
        IVFW.autoTabWidth();
        IVFW.initBank();
        if ($(".m-iv").length > 0) {
            IVFW.bindSwitchEvent();
            $(document).ready(function () {
                //初始化列表 和 高级选项 UI
                IVFW.setUI();
            });
            $(window).resize(function () {
                IVFW.setUI();
            });
        }
    },
    autoTabWidth: function () {
        var divWrapper = $(".m-extend-tabs");
        var wraperW = $(".m-extend-tabs").width();
        var totalW = 0;
        var divTabs = $(".m-extend-tabs .tab-links").parent();
        var lis = $(".m-extend-tabs li");
        $.each(lis, function (i, li) {
            totalW += $(this).outerWidth();
        });
        if (totalW > wraperW) {
            totalW += 10;
            divTabs.width(totalW);
            divWrapper.css({ "overflow-x": "scroll" });
        }
        else {
            divTabs.css({ "width": "100%" });
            divWrapper.css({ "overflow-x": "hidden" });
        }
        $("#divAmountsAre").css("right", (0 - $(".m-iv-adv-switch:visible").width())+"px");
    },
    initBank:function(){
        $(".form-invoice-toolbar").find(".bank-all").mouseover(function () {
            $(this).addClass("bank-more");
        }).mouseleave(function () {
            $(this).removeClass("bank-more");
        });
    },
    initAdvSearch: function () {
        $(".m-adv-search-btn").click(function () {
            $(".m-adv-search").show();
            $(this).hide();
        });
        $(".m-adv-search>.m-adv-search-close>a").click(function () {
            $(".m-adv-search").hide();
            $(".m-adv-search-btn").show();
        });
        $(".m-adv-search").find("#aClearSearchFilter").click(function () {
            $("body").mFormClearForm();
        });
    },
    setUI: function (wp) {
        var w = $(".m-imain").innerWidth() - 60;
        var advW = 0;
        if ($(".m-iv-adv").length > 0 && $(".m-iv-adv").css("display") != "none") {
            advW = $(".m-iv-adv").outerWidth();
        }
        if ($(".m-iv-adv").length <= 0) {
            //如果高级选择不存在，则明细列表宽度+20
            $(".m-iv-entry").css({ "width": ((w - advW) + 20) + "px" });
        } else {
            $(".m-iv-entry").css({ "width": (w - advW) + "px" });
        }
        
        //判断 m-imain div 是否出现了滚动条
        if ($(".m-imain").get(0).scrollHeight > $(".m-imain").height()) {
            //有滚动条
            $(".m-iv-entry").width($(".m-iv-entry").width() - 17);
        }
        try{
            $("#tbInvoiceDetail").datagrid('resize', {
                width: $(".m-iv-entry").width()
            });
        } catch (exc) { }

        //高级选项高度自适应
        IVFW.advanceHeightAuto();
    },
    //高级选项高度自适应
    advanceHeightAuto: function () {
        //明细列表高度 =（列表记录数 * 35）+ 列表头高度 + 底部汇总高度 + 20像素边距
        var rowsCount = 5;
        try {
            rowsCount = $('#tbInvoiceDetail').datagrid('getRows').length;
        } catch (exc) { }
        var itemLeftHeight = (rowsCount * 35) + 35 + $(".form-invoice-total").height() + 20;
        //高级选项对象
        var advanceDiv = $("#aa");
        //如果明细记录不足 5 ，则取最小高度
        if (rowsCount < 5) {
            var minHeight = parseInt(advanceDiv.css("min-height"));
            advanceDiv.height(minHeight);
        } else {
            advanceDiv.height(itemLeftHeight);
        }

        //jquery.accordion.js line15 
        //高度设置为0了 重设下高度
        advanceDiv.find(">div.panel>div.accordion-header").css("height", "auto");
    },
    getContainerWidth: function () {
        var containerWidth = $("#tbInvoiceDetail").closest(".datagrid").parent().width();
        if (containerWidth < 200) {
            containerWidth = 200;
        }
        return containerWidth;
    },
    bindSwitchEvent: function () {
        $(".m-iv-adv-switch>a").unbind().click(function () {
            if ($(this).hasClass("show")) {
                $(this).removeClass("show").addClass("hide");
                $(".m-iv-adv").hide();
            } else {
                $(this).removeClass("hide").addClass("show");
                $(".m-iv-adv").show();
            }
            IVFW.setUI();
            //如果高级选择不存在，则明细列表宽度+20
            if(!$(this).hasClass("show")){
                $(".m-iv-entry").width($(".m-iv-entry").width() + 20);
            }
            
            $(window).resize();
        });
    }
}
$(document).ready(function () {
    IVFW.init();
    $(window).resize(function () {
        IVFW.autoTabWidth();
    });
});