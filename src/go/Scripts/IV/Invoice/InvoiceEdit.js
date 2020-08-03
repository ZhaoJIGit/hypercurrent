/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="InvoiceEditBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var InvoiceEdit = {
    init: function () {
        //保存
        $("#btnSave").click(function () {
            InvoiceEditBase.saveInvoice(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var status = InvoiceEditBase.CurrentStatus == 0 ? IVBase.Status.Draft : InvoiceEditBase.CurrentStatus;
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(status);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                if (InvoiceEditBase.CurrentStatus == IVBase.Status.AwaitingPayment || InvoiceEditBase.CurrentStatus == IVBase.Status.Paid) {
                    mWindow.reload(InvoiceEditBase.ViewUrl + "/" + msg.ObjectID);
                } else {
                    mWindow.reload(InvoiceEditBase.EditUrl + "/" + msg.ObjectID);
                }
            });
        });
        //保存为草稿
        $("#btnSaveAsDraft").click(function () {
            InvoiceEditBase.saveInvoiceAsDraft(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(InvoiceEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后继续编辑
        $("#btnSaveAndContinue").click(function () {
            InvoiceEditBase.saveInvoice(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(InvoiceEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后提交审核
        $("#btnSaveAndSubmitForApproval").click(function () {
            InvoiceEditBase.saveInvoiceAndSubmitForApproval(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.WaitingApproval);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(InvoiceEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后重新添加一个销售发票
        $("#btnSaveAndAddAnother").click(function () {
            InvoiceEditBase.saveInvoice(function () {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "NewInvoice", "NewInvoice"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(InvoiceEditBase.EditUrl);
            });
        });
        //审核销售发票
        $("#aApproveInvoice").click(function () {
            InvoiceEditBase.saveInvoiceAndApprove(function (msg) {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(InvoiceEditBase.ViewUrl + "/" + msg.ObjectID + "?sv=1");
            });
        });
        //审核后重新添加一个销售发票
        $("#aApproveAndAddAnother").click(function () {
            InvoiceEditBase.saveInvoiceAndApprove(function () {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                $.mTab.rename(IVBase.getTabTitle("NewInvoice"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(InvoiceEditBase.EditUrl);
            });
        });
        //审核后查看下一个
        $("#aApproveAndViewNext").click(function () {
            var newIVID = $(this).attr("ivId");
            var newIVType = $(this).attr("ivType");
            InvoiceEditBase.saveInvoiceAndApprove(function () {
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh("", url, false, true);
                //关闭当前选项卡

                if (newIVType == "Invoice_Sale") {
                    var tabTitle = HtmlLang.Write(LangModule.IV, "EditInvoice", "Edit Invoice");
                    $.mTab.rename(tabTitle);
                    mWindow.reload("/IV/Invoice/InvoiceEdit/" + newIVID);
                } else {
                    var tabTitle = HtmlLang.Write(LangModule.IV, "EditSaleCreditNote", "Edit Credit Note");
                    $.mTab.rename(tabTitle);
                    mWindow.reload("/IV/Invoice/CreditNoteEdit/" + newIVID);
                }
            });
        });
        //审核后打印
        $("#aApproveAndPrint").click(function () {
            InvoiceEditBase.saveInvoiceAndApprove(function (response) {
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = InvoiceEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //打印
                var invoiceId = $.trim($("#hidInvoiceID").val());
                invoiceId = invoiceId == "" ? response.ObjectID : invoiceId;
                var printParam = {};
                printParam.MStatus = InvoiceEditBase.CurrentStatus;
                printParam.MType = IVBase.InvoiceType.Sale;
                printParam.SelectedIds = invoiceId;
                IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "invoices", "Invoices"), $.toJSON(printParam), "InvoiceListPrint");

                $.mTab.refresh(title, url, false, true);
                mWindow.reload(InvoiceEditBase.ViewUrl + "/" + invoiceId + "?sv=1");
            });
        });
        //打印
        $("#aPrint").click(function () {
            InvoiceEditBase.printInvoice($(this).attr("billtype"));
        });
        //操作记录
        $("#aHistory").click(function () {
            InvoiceEditBase.viewHistory();
        });
        //发送Email
        $("#aEmail").click(function () {
            InvoiceEditBase.emailInvoice();
        });
    }
}

$(document).ready(function () {
    InvoiceEdit.init();
});