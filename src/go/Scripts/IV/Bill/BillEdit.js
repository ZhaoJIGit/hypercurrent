/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="BillEditBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var BillEdit = {
    init: function () {
        //保存
        $("#btnSave").click(function () {
            BillEditBase.saveInvoice(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var status = BillEditBase.CurrentStatus == 0 ? IVBase.Status.Draft : BillEditBase.CurrentStatus;
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(status);
                //在指定的tab选项卡中刷新
                $.mTab.refresh("", url, false, true);
                if (BillEditBase.CurrentStatus == IVBase.Status.AwaitingPayment || BillEditBase.CurrentStatus == IVBase.Status.Paid) {
                    mWindow.reload(BillEditBase.ViewUrl + "/" + msg.ObjectID);
                } else {
                    mWindow.reload(BillEditBase.EditUrl + "/" + msg.ObjectID);
                }
            });
        });
        //保存为草稿
        $("#btnSaveAsDraft").click(function () {
            BillEditBase.saveInvoiceAsDraft(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(BillEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后继续编辑
        $("#btnSaveAndContinue").click(function () {
            BillEditBase.saveInvoice(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(BillEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后提交审核
        $("#btnSaveAndSubmitForApproval").click(function () {
            BillEditBase.saveInvoiceAndSubmitForApproval(function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle());
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.WaitingApproval);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(BillEditBase.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后重新添加一个采购发票
        $("#btnSaveAndAddAnother").click(function () {
            BillEditBase.saveInvoice(function () {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(IVBase.getTabTitle("NewBill"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.Draft);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(BillEditBase.EditUrl);
            });
        });
        //审核采购发票
        $("#aApproveInvoice").click(function () {
            BillEditBase.saveInvoiceAndApprove(function (msg) {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                //$.mTab.rename(IVBase.getTabTitle());
                $.mTab.rename(HtmlLang.Write(LangModule.IV, "ViewBill", "View Bill"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(BillEditBase.ViewUrl + "/" + msg.ObjectID + "?sv=1");
            });
        });
        //审核后重新添加一个采购发票
        $("#aApproveAndAddAnother").click(function () {
            BillEditBase.saveInvoiceAndApprove(function () {
                var message = HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!");
                $.mMsg(message);
                $.mTab.rename(IVBase.getTabTitle("NewBill"));
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(BillEditBase.EditUrl);
            });
        });
        //审核后查看下一个
        $("#aApproveAndViewNext").click(function () {
            var newIVID = $(this).attr("ivId");
            var newIVType = $(this).attr("ivType");
            BillEditBase.saveInvoiceAndApprove(function () {
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                
                $.mTab.refresh("", url, false, false);
                //关闭当前选项卡

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
            BillEditBase.saveInvoiceAndApprove(function (response) {
                //tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "Purchase", "Purchase");
                //提示信息
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                //刷新的Url地址
                var url = BillEditBase.ListUrl + "?id=" + IVBase.StatusPlusOne(IVBase.Status.AwaitingPayment);
                //打印
                var invoiceId = $.trim($("#hidInvoiceID").val());
                invoiceId = invoiceId == "" ? response.ObjectID : invoiceId;
                var printParam = {};
                printParam.MStatus = BillEditBase.CurrentStatus;
                printParam.MType = IVBase.InvoiceType.Purchase;
                printParam.SelectedIds = invoiceId;
                IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "Bills", "Bills"), $.toJSON(printParam), "BillListPrint");

                $.mTab.refresh(title, url, false, true);
                mWindow.reload(BillEditBase.ViewUrl + "/" + invoiceId + "?sv=1");
            });
        });
        //打印
        $("#aPrint").click(function () {
            BillEditBase.printInvoice($(this).attr("billtype"));
        });
        //操作记录
        $("#aHistory").click(function () {
            BillEditBase.viewHistory();
        });
    }
}

$(document).ready(function () {
    BillEdit.init();
});