//高级选项切换
var IVTransferFW = {
    init: function () {
        if ($(".m-iv-adv").length > 0) {
            IVTransferFW.bindSwitchEvent();
            $(document).ready(function () {
                //初始化列表 和 高级选项 UI
                IVTransferFW.setUI();
                //初始化删除
                IVTransferFW.transterDelete();
            });
            $(window).resize(function () {
                IVTransferFW.setUI();
            });
        }
    },
    setUI: function (wp) {
        var w = $(".m-imain").innerWidth() - 60;
        var advW = 0;
        if ($(".m-iv-adv").length > 0 && $(".m-iv-adv").css("display") != "none") {
            advW = $(".m-iv-adv").outerWidth();
        }

        //判断 m-imain div 是否出现了滚动条
        $(".m-imain").scrollTop(5);
        if ($(".m-imain").scrollTop() > 0) {
            //
            //有滚动条
            //$(".m-transfer-content").width($(".m-transfer-content").width() - 17);
        }
        $(".m-imain").scrollTop(0);

        //高级选项高度自适应
        IVTransferFW.advanceHeightAuto();
    },
    //转账单的删除
    transterDelete: function () {
        //ID
        var transferID = $("#hidBillId").val();
        //删除
        $("#idDelete").off("click").on("click", function () {
            $.mDialog.confirm(LangKey.AreYouSureToDelete, function () {
                //传参数ID
                var param = {};
                param.MID = transferID;
                mAjax.submit(
                    "/IV/IVTransfer/deleteTransfer", //URL
                    { model: param },
                    function (msg) {
                        if (msg.Success) {
                            //删除成功
                            var message = HtmlLang.Write(LangModule.Bank, "DeleteSuccessfully", "Delete Successfully!");
                            $.mMsg(message);
                            if (window.parent.$(".mCloseBox").length > 0) {
                                $.mDialog.close();
                            }
                            else {

                                //关闭当前选项卡
                                $.mTab.remove();
                            }

                        } else {
                            //删除失败
                            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Have cancel after verification, not allowed to delete"));
                        }
                    });
            }
            );
        });
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
            IVTransferFW.setUI();
            //如果高级选择不存在，则明细列表宽度+20
            if (!$(this).hasClass("show")) {
                $(".m-iv-entry").width($(".m-iv-entry").width() + 20);
            }
            $("#tbInvoiceDetail").datagrid('resize', {
                width: $(".m-iv-entry").width()
            });
        });
    }
}
$(document).ready(function () {
    IVTransferFW.init();
});