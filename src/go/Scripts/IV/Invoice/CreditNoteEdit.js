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
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "Invoice_Sale_Red", "Credit Note"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var status = CreditNoteEditBase.CurrentStatus == 0 ? IVBase.Status.Draft : CreditNoteEditBase.CurrentStatus;
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(status);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                if (CreditNoteEditBase.CurrentStatus == IVBase.Status.AwaitingPayment || CreditNoteEditBase.CurrentStatus == IVBase.Status.Paid) {
                    wmWindow.reload(CreditNoteEditBase.ViewUrl + "/" + msg.ObjectID);
                } else {
                    mWindow.reload(CreditNoteEditBase.EditUrl + "/" + msg.ObjectID);
                }
            });
        });
        //保存为草稿
        $("#btnSaveAsDraft").click(function () {
            CreditNoteEditBase.saveInvoiceAsDraft(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "Invoice_Sale_Red", "Credit Note"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
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
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "Invoice_Sale_Red", "Credit Note"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                window.location = CreditNoteEditBase.EditUrl + "/" + msg.ObjectID ;
            });
        });
        //保存后提交审核
        $("#btnSaveAndSubmitForApproval").click(function () {
            CreditNoteEditBase.saveInvoiceAndSubmitForApproval(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "Invoice_Sale_Red", "Credit Note"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.WaitingApproval);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                CreditNoteEdit.refreshInvoice();
                window.location = CreditNoteEditBase.EditUrl + "/" + msg.ObjectID ;
            });
        });
        //保存后重新添加一个销售发票
        $("#btnSaveAndAddAnother").click(function () {
            CreditNoteEditBase.saveInvoice(function () {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle("NewCreditNote"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                CreditNoteEdit.refreshInvoice();
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(CreditNoteEditBase.EditUrl);
            });
        });
        //审核销售发票
        $("#aApproveInvoice").click(function () {
            CreditNoteEditBase.saveInvoiceAndApprove(function (msg) {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                //$.mTab.rename(IVBase.getTabTitle());
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "ViewCreditNote", "View Credit Note"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                CreditNoteEdit.refreshInvoice();
                mWindow.reload(CreditNoteEditBase.ViewUrl + "/" + msg.ObjectID + "?sv=1");
            });
        });
        //审核后重新添加一个销售发票
        $("#aApproveAndAddAnother").click(function () {
            CreditNoteEditBase.saveInvoiceAndApprove(function () {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                $.mTab.rename(IVBase.getTabTitle("NewCreditNote"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                CreditNoteEdit.refreshInvoice();
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
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
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
            CreditNoteEditBase.saveInvoiceAndApprove(function (response) {
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Sales", "Sales");
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = CreditNoteEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //打印
                var invoiceId = $.trim($("#hidInvoiceID").val());
                invoiceId = invoiceId == "" ? response.ObjectID : invoiceId;
                var printParam = {};
                printParam.MStatus = CreditNoteEditBase.CurrentStatus;
                printParam.MType = IVBase.InvoiceType.InvoiceSaleRed;
                printParam.SelectedIds = invoiceId;
                IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "invoices", "Invoices"), $.toJSON(printParam), "InvoiceListPrint");

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
        //发送Email
        $("#aEmail").click(function () {
            CreditNoteEditBase.emailInvoice();
        });
    }
}

$(document).ready(function () {
    CreditNoteEdit.init();
});