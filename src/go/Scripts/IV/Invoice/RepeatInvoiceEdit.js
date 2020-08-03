/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="RepeatRepeatInvoiceEditBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var RepeatInvoiceEdit = {
    init: function () {
        //保存
        $("#btnSave").click(function () {
            RepeatInvoiceEditBase.saveInvoice(function (msg) {
                //提示信息
                $.mMsg(LangKey.SaveSuccessfully);

                var tabTitle = HtmlLang.Write(LangModule.IV, "EditRepeatingInvoice", "Edit Repeating Invoice");
                $.mTab.rename(tabTitle);
                //销售发票列表tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = RepeatInvoiceEditBase.ListUrl + "?id=6";
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //刷新当前页面
                mWindow.reload(RepeatInvoiceEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //操作记录
        $("#aHistory").click(function () {
            RepeatInvoiceEditBase.viewHistory();
        });
    }
}

$(document).ready(function () {
    RepeatInvoiceEdit.init();
});