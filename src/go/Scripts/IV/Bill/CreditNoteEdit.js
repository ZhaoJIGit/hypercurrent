/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="CreditNoteEditBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var CreditNoteEdit = {
    CopyID: "",
    refreshInvoice: function () {
        if (CreditNoteEdit.CopyID == undefined || CreditNoteEdit.CopyID == "") {
            return;
        }
        var tabTitle = HtmlLang.Write(LangModule.IV, "ViewInvoice", "View Invoice");
        $.mTab.refresh(tabTitle, IVBase.url_View + "/" + CreditNoteEdit.CopyID, false);
    },
    init: function () {
        CreditNoteEdit.CopyID = Megi.request('cpyId');
        $("#btnSave").click(function () {
            CreditNoteEditBase.saveInvoice(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                //var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var status = CreditNoteEditBase.CurrentStatus == 0 ? IVBase.Status.Draft : CreditNoteEditBase.CurrentStatus;
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(status);
                //在指定的tab选项卡中刷新
                $.mTab.refresh("", url, false, true);
                if (CreditNoteEditBase.CurrentStatus == IVBase.Status.AwaitingPayment || CreditNoteEditBase.CurrentStatus == IVBase.Status.Paid) {
                    mWindow.reload(CreditNoteEditBase.ViewUrl + "/" + msg.ObjectID);
                } else {
                    mWindow.reload(CreditNoteEditBase.EditUrl + "/" + msg.ObjectID);
                }
            });
        });
        //保存为草稿
        $("#btnSaveAsDraft").click(function () {
            CreditNoteEditBase.saveInvoiceAsDraft(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(CreditNoteEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后继续编辑
        $("#btnSaveAndContinue").click(function () {
            CreditNoteEditBase.saveInvoice(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(CreditNoteEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后提交审核
        $("#btnSaveAndSubmitForApproval").click(function () {
            CreditNoteEditBase.saveInvoiceAndSubmitForApproval(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.WaitingApproval);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(CreditNoteEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后重新添加一个采购红字发票
        $("#btnSaveAndAddAnother").click(function () {
            var newIVID = $(this).attr("ivId");
            var newIVType = $(this).attr("ivType");
            CreditNoteEditBase.saveInvoice(function () {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle("NewCreditNote"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(CreditNoteEditBase.EditUrl);
            });
        });
        //审核采购红字发票
        $("#aApproveInvoice").click(function () {
            CreditNoteEditBase.saveInvoiceAndApprove(function (msg) {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                //$.mTab.rename(IVBase.getTabTitle());
                //ViewDebitNote
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "ViewDebitNote", "View Credit Note"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(CreditNoteEditBase.ViewUrl + "/" + msg.ObjectID + "?sv=1");
            });
        });
        //审核后重新添加一个采购红字发票
        $("#aApproveAndAddAnother").click(function () {
            CreditNoteEditBase.saveInvoiceAndApprove(function () {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                $.mTab.rename(IVBase.getTabTitle("NewCreditNote"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(CreditNoteEditBase.EditUrl);
            });
        });
        //审核后查看下一个
        $("#aApproveAndViewNext").click(function () {
            var newIVID = $(this).attr("ivId");
            var newIVType = $(this).attr("ivType");
            CreditNoteEditBase.saveInvoiceAndApprove(function () {
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);

                if (newIVType == "Invoice_Purchase") {
                    var tabTitle = HtmlLang.Write(LangModule.IV, "EditBill", "Edit Bill");
                    $.mTab.rename(tabTitle);
                    mWindow.reload("/IV/Bill/BillEdit/" + newIVID);
                } else {
                    var tabTitle = HtmlLang.Write(LangModule.IV, "EditCreditNote", "Edit Credit Note");
                    $.mTab.rename(tabTitle);
                    mWindow.reload("/IV/Bill/CreditNoteEdit/" + newIVID);
                }
            });
        });
        //审核后打印
        $("#aApproveAndPrint").click(function () {
            CreditNoteEditBase.saveInvoiceAndApprove(function (response) {
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //打印
                var invoiceId = $.trim($("#hidInvoiceID").val());
                invoiceId = invoiceId == "" ? response.ObjectID : invoiceId;
                var printParam = {};
                printParam.MStatus = CreditNoteEditBase.CurrentStatus;
                printParam.MType = IVBase.InvoiceType.InvoicePurchaseRed;
                printParam.SelectedIds = invoiceId;
                IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "Bills", "Bills"), $.toJSON(printParam), "BillListPrint");

                $.mTab.refresh(title, url, false, true);
                mWindow.reload(CreditNoteEditBase.ViewUrl + "/" + invoiceId + "?sv=1");
            });
        });
        //打印
        $("#aPrint").click(function () {
            CreditNoteEditBase.printInvoice($(this).attr("billtype"));
        });
        //操作记录
        $("#aHistory").click(function () {
            CreditNoteEditBase.viewHistory();
        });
    }
}

$(document).ready(function () {
    CreditNoteEdit.init();
});