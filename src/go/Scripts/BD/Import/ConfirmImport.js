/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="ImportBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ConfirmImport = {
    type: $("#hidType").val(),
    isCover: $("#hideIsCover").val(),
    init: function () {
        ConfirmImport.bindAction();
    },
    complete: function () {
        switch (ConfirmImport.type) {
            case ImportType.Account:
                if (AccountMatch) {
                    AccountMatch.save();
                }
                return false;
                break;
        }
        return true;
    },
    bindAction: function () {
        $("#aCompleteImport").click(function () {
            //非自定义科目的保存
            if (ConfirmImport.isCover.toUpperCase() != "TRUE" && !ConfirmImport.complete()) {
                return;
            }

            var param = {};
            param.type = ConfirmImport.type;
            param.isCover = ConfirmImport.isCover;
            ImportBase.confirmImport("/BD/Import/CompleteImport?id=" + param.type, ConfirmImport.appendParamByType(param), function (response) {

                var addMsgList = [];
                var importedCount = $("#hidImportingCount").val();
                var arrIVType = [ImportType.InvoiceSale, ImportType.InvoiceSaleRed, ImportType.InvoicePurchase, ImportType.InvoicePurchaseRed];
                var isImportInvoice = $.inArray(ConfirmImport.type, arrIVType) != -1;
                if ((isImportInvoice || ConfirmImport.type == ImportType.PayPurchase || ConfirmImport.type == ImportType.ReceiveSale || ConfirmImport.type == ImportType.ExpenseItem) && response && response.Tag) {
                    if (ImportBase.isJsonStr(response.Tag)) {
                        var newContactNames = JSON.parse(response.Tag).NewContact;
                        if (newContactNames) {
                            addMsgList.push(HtmlLang.Write(LangModule.Common, "Import_NewContactAutoAdded", "联系人:{0}已自动新增!").replace("{0}", newContactNames));
                        }
                        var newEmpNames = JSON.parse(response.Tag).NewEmp;
                        if (newEmpNames) {
                            addMsgList.push(HtmlLang.Write(LangModule.Common, "Import_NewEmpAutoAdded", "员工:{0}已自动新增!").replace("{0}", newEmpNames));
                        }
                    }
                    else {
                        switch (ConfirmImport.type) {
                            case ImportType.ExpenseItem:
                                addMsgList.push(HtmlLang.Write(LangModule.Common, "Import_NewExpItemAutoAdded", "父级费用项目:{0}已自动新增!").replace("{0}", response.Tag));
                                break;
                        }
                    }
                }
                ImportBase.showSuccessMsg(ConfirmImport.type, importedCount, addMsgList);

                //提示
                if (isImportInvoice) {
                    var arrUrl = parent.location.href.split('?');
                    var urlSec = arrUrl[0].split('/');
                    var lastUrlSec = urlSec[urlSec.length - 1].split('#')[0];
                    if (isNaN(Number(lastUrlSec))) {
                        urlSec.push(2);
                    }
                    else {
                        urlSec[urlSec.length - 1] = 2;
                    }
                    arrUrl[0] = urlSec.join('/');
                    var url = arrUrl.join('?');

                    parent.mWindow.reload(url);
                    return;
                }

                if (parent) {
                    //刷新页面
                    if (ConfirmImport.type == ImportType.Voucher) {
                        $.mDialog.close();
                        //var arrUrl = [];
                        //arrUrl.push(parent.location.href.split('#')[0]);
                        //arrUrl.push(arrUrl.toString().indexOf('?') != -1 ? '&' : '?');
                        //arrUrl.push("Index=1");
                        //parent.mWindow.reload(arrUrl.join(''));
                    }
                    else if (ConfirmImport.type == ImportType.PayPurchase || ConfirmImport.type == ImportType.ReceiveSale) {
                        var responseTag = JSON.parse(response.Tag);
                        new parent.BDBankReconcileHome().Reload("3", [responseTag.StartDate, responseTag.EndDate]);
                        $.mDialog.close();
                    }
                    else if (ConfirmImport.type == ImportType.Account && parent.AccountList) {
                        parent.AccountList.reload();
                        $.mDialog.close();
                    }
                    else {
                        parent.mWindow.reload();
                    }
                }
            });
        });

        $("#aGoBack").click(function () {
            var isCover = ConfirmImport.isCover.toUpperCase() == "TRUE";
            var isImportAcct = !isCover && ConfirmImport.type == ImportType.Account;
            if (AccountMatch && AccountMatch.currentMode == MatchMode.Preview && isImportAcct) {
                AccountMatch.switchMode(MatchMode.Match);
            }
            else {
                if (isImportAcct && AccountMatch) {
                    AccountMatch.confirmSaveMatchLog(function () {
                        mWindow.reload(ConfirmImport.appendParamByType("/BD/Import/Import/" + ConfirmImport.type));
                    });
                }
                else {
                    var url = "/BD/Import/Import/" + ConfirmImport.type;
                    if (isCover) {
                        url += "?isCover=true";
                    }
                    mWindow.reload(ConfirmImport.appendParamByType(url));
                }
            }
        });
    },
    appendParamByType: function (obj) {
        if (ConfirmImport.type == ImportType.ReceiveSale || ConfirmImport.type == ImportType.PayPurchase) {
            var accountId = $("#hidAccountId").val();
            var contactType = $("#hidContactType").val();
            if (typeof obj == 'object') {
                obj.accountId = accountId;
                obj.contactType = contactType;
            }
            else {
                obj += '?accountId=' + accountId + '&contactType=' + contactType;
            }
        }
        return obj;
    }
}

$(document).ready(function () {
    ConfirmImport.init();
});