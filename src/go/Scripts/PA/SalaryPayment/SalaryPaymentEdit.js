/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var SalaryPaymentEdit = {
    SalaryPayID: $("#hidSalaryPayID").val(),
    //发票模型
    InvoiceModel: null,
    InvoiceType: '',
    CurrentStatus: 1,
    ListUrl: "/PA/SalaryPayment/SalaryPaymentList",
    EditUrl: "/PA/SalaryPayment/SalaryPaymentEdit",
    UpdUrl: "/PA/SalaryPayment/SalaryPaymentUpdate",
    UnApproveUrl: "/PA/SalaryPayment/UnApproveSalaryPayment",
    IsInit: false,
    IsEdit: true,
    //初始化
    init: function () {
        SalaryPaymentEdit.getModel();
    },
    //初始化发票信息
    getModel: function () {
        SPEditBase.init(SalaryPaymentEdit.SalaryPayID, $("#hidTaxAmt").val());
        //将当前页面设置为稳定状态
        $.mTab.setStable();
        SalaryPaymentEdit.clickAction();
    },
    clickAction: function () {
        $("#aSave").off('click').on('click', function () {
            SalaryPaymentEdit.saveModel();
        });
    },
    unApproveToDraft: function () {
        $.mDialog.confirm(HtmlLang.Write(LangModule.IV, "AreYouSureToUnAuditToDraft", "Are you sure to UnAudit To Draft?"), {
            callback: function () {
                $.mAjax.submit(SalaryPaymentEdit.UnApproveUrl, { ids: SalaryPaymentEdit.SalaryPayID }, function (data) {
                    //提示信息
                    $.mMsg(HtmlLang.Write(LangModule.IV, "UnAuditToDraftSuccessfully", "UnAudit To Draft Successfully!"));

                    parent.SalaryPaymentList.reload();
                    $.mDialog.close(0);
                });
            }
        });
    },
    saveModel: function () {
        var entryInfo = SPEditBase.getViewInfo();
        //至少要有一条明细
        if (entryInfo.SalaryPaymentEntry == null || entryInfo.SalaryPaymentEntry.length < 1) {
            $.mDialog.alert("You must have at least 1 line item.", null, LangModule.IV, "AtLeastOneLineItem");
            return;
        }
        var obj = {};
        obj.MID = $("#hidSalaryPayID").val();
        obj.MTaxSalary = entryInfo.MTaxSalary;
        obj.MNetSalary = entryInfo.MNetSalary;
        obj.SalaryPaymentEntry = entryInfo.SalaryPaymentEntry;
        mAjax.submit(
            SalaryPaymentEdit.UpdUrl,
            { spModel: obj },
            function (data) {
                if (data.Success) {
                    //刷新的Url地址
                    parent.SalaryPaymentList.reload();
                    $.mDialog.close(0);
                } else {
                    $.mDialog.alert(data.Message);
                }
            });
    },
    reload: function () {
        if (parent.SalaryPaymentList) {
            parent.SalaryPaymentList.reload();
        }
        mWindow.reload();
    }
}

$(document).ready(function () {
    SalaryPaymentEdit.init();
});