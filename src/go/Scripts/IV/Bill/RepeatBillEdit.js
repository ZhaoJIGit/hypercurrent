/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="RepeatBillEditBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var RepeatBillEdit = {
    init: function () {
        //保存
        $("#btnSave").click(function () {
            RepeatBillEditBase.saveInvoice(function (msg) {
                //提示信息
                $.mMsg(LangKey.SaveSuccessfully);

                //修改当前tab选项卡标题
                var tabTitle = HtmlLang.Write(LangModule.IV, "EditRepeatingPurchase","编辑重复采购单");
                $.mTab.rename(tabTitle);
                //销售发票列表tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = RepeatBillEditBase.ListUrl + "?id=6";
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //刷新当前页面
                mWindow.reload(RepeatBillEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //操作记录
        $("#aHistory").click(function () {
            RepeatBillEditBase.viewHistory();
        });
    }
}

$(document).ready(function () {
    RepeatBillEdit.init();
});