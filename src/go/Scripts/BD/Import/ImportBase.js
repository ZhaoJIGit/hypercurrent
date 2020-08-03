/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ImportType = {
    InvoiceSale: "Invoice_Sale",
    InvoiceSaleRed: "Invoice_Sale_Red",
    InvoicePurchase: "Invoice_Purchase",
    InvoicePurchaseRed: "Invoice_Purchase_Red",
    PayPurchase: "Pay_Purchase",
    ReceiveSale: "Receive_Sale",
    Item: "Item",
    Contact: "Contact",
    Employees: "Employees",
    Account: "Account",
    OpeningBalance: "OpeningBalance",
    Voucher: "Voucher",
    PayRun: "PayRun",
    ExpenseItem: "ExpenseItem",
    InFaPiao: "InFaPiao",
    OutFaPiao:"OutFaPiao"
}
var ImportTypeExt = { Voucher: 1, Sale: 2, Purchase: 3, Payment: 4, Receive: 5, Expense: 6, InFaPiao: 7, OutFaPiao: 8 };
var ImportBase = {
    isIE9Previous: $.browser.msie != undefined && $.browser.msie && parseInt($.browser.version) <= 9,
    onFileChanged: function (sender) {
        var defaultText = HtmlLang.Write(LangModule.Common, "UploadLabel", "Browse...");
        var displayText = sender.value.substring(sender.value.lastIndexOf('\\') + 1);
        if (displayText == "") {
            displayText = defaultText;
        }
        var uploadLabel = $(sender).next();
        if (!uploadLabel) {
            uploadLabel = $(sender).parents(".upload-icon").next();
        }
        uploadLabel.html(displayText);//.css('white-space', 'nowrap')
    },
    restoreDefaultText: function (sender) {
        var defaultText = HtmlLang.Write(LangModule.Common, "UploadLabel", "Browse...");
        $(sender).html(defaultText);
    },
    clearFile: function (sender) {
        if (sender == undefined) {
            sender = $("#fileInput")[0];
        }
        var lbl = $(sender).next();
        //For IE
        $('#' + sender.id).replaceWith($(sender).clone(true));
        //For other browsers
        $('#' + sender.id).val("");
        //解决文件格式校验失败时文件没清空掉问题
        document.getElementById(sender.id).value = "";
        ImportBase.restoreDefaultText(lbl);
    },
    isSelectFile: function () {
        var isSelectFile = false;

        var file = document.getElementById("fileInput");
        if (file) {
            if (window.navigator.userAgent.indexOf("MSIE") >= 1) {
                isSelectFile = file.value != "";
            } else {
                isSelectFile = file.files.length != 0
            }
        }
        return isSelectFile;
    },
    selectFileCount: function () {
        var count = 0;
        var file = document.getElementById("fileInput");
        if (file) {
            if (ImportBase.isIE9Previous && file.value != '') {
                count = 1;
            }
            else {
                count = file.files.length;
            }
        }
        return count;
    },
    minDialog: function () {
        $.mDialog.min();
        $(".m-imain").height($("body").height() - $(".m-toolbar-footer").outerHeight());
    },
    validateFile: function (msg) {
        if (!ImportBase.isSelectFile()) {
            if (!msg) {
                msg = HtmlLang.Write(LangModule.Common, "UnSelectImportFile", "Please select the file you wish to import.");
            }
            $.mDialog.alert(msg);
            return false;
        }
        return true;
    },
    validateConfirm: function () {
        var count = parseInt($("#hidImportingCount").val());
        return count > 0;
    },
    //显示导入弹出框的方法
    showImportBox: function (url, title, width, height, callback) {
        if (!width) {
            width = 800;
        }

        if (!height) {
            height = 452;
        }

        $.mDialog.show({
            mTitle: title,
            mWidth: width,
            mHeight: height,
            mDrag: "mBoxTitle",
            mShowbg: true,
            mContent: "iframe:" + url,
            mCloseCallback: callback
        });
    },
    closeImportBox: function () {
        $.mDialog.close();
    },
    downloadTemplate: function (downLoadUrl) {
        $.mMsg(HtmlLang.Write(LangModule.Common, "Downloading", "Downloading..."));

        //mWindow.reload(downLoadUrl , false , true);
        window.location.href = downLoadUrl;
    },
    getFileName: function (fileId) {
        var result = '';
        if (ImportBase.isIE9Previous) {
            result = document.getElementById(fileId).value;
            result = result.substring(result.lastIndexOf('\\') + 1);
        }
        else if (document.getElementById(fileId).files.length > 0) {
            result = document.getElementById(fileId).files[0].name;
        }
        return result;
    },
    attachPropertyChangeEvent: function (obj, callBack) {
        if ($.browser.msie) {
            obj.onpropertychange = callBack;
        } else {
            obj.addEventListener("input", callBack, false);
        }
    },
    initAjaxForm: function (toUrl, callBack) {
        $("form").ajaxForm({
            dataType: ImportBase.isIE9Previous ? null : "json",
            success: function (response) {

                //判断是否被逼下线 
                if (response && response.accessDenied === 1) {
                    $("body").unmask();

                    top.showLoginDialog(response.type, function () {});

                    return;
                }

                response = response.Data || response;
                if (response == undefined || response.Success == undefined) {
                    $.getJSON("/BD/Docs/GetUploadResult?id=" + (new Date()).toString(), function (response) {
                        ImportBase.execUploadCallBack(response, toUrl, callBack);
                    });
                }
                else {
                    ImportBase.execUploadCallBack(response, toUrl, callBack);
                }
            },
            fail: function (event, data) {
                ImportBase.clearFile();
                $.mDialog.alert(data);
                $("body").unmask();
            }
        });
    },
    execUploadCallBack: function (response, toUrl, callBack) {
        var urlParamSpliter = toUrl.lastIndexOf('?') != -1 ? '&' : '?';
        if (response.ImportCnt == undefined) {
            response.ImportCnt = '';
        }
        toUrl = toUrl + urlParamSpliter + "fileName=" + encodeURIComponent(response.FileName) + "&importCnt=" + response.ImportCnt;
        if (callBack != undefined) {
            callBack(response, toUrl);
            return;
        }
        if (response && !response.Success) {
            ImportBase.clearFile();

            if (response.FileUrl) {
                $.mDialog.confirm(response.Message, {
                    callback: function () {
                        window.location.href = response.FileUrl;
                    }
                });
            } else {
                response.Message = response.Message.replace(/(&#92;n)/gm, "");
                $.mDialog.alert(response.Message, undefined, 0, true);
            }

            $("body").unmask();
        } else {
            mWindow.reload(toUrl);
        }
    },
    confirmImport: function (url, params, successCallBack, reload) {
        if (!ImportBase.validateConfirm()) {
            return;
        }

        $("body").mask("");
        if (!params) {
            params = {};
        }
        params.fileName = $("#hidNewFileName").val();

        mAjax.submit(url, params, function (response) {
            var resultData = response;
            if (resultData) {
                if (resultData.Success) {
                    if (successCallBack != undefined) {
                        successCallBack(response);
                    }
                }
                else {
                    var msg = resultData.Message || resultData.ErrorMessageDetail;
                    if(!msg && resultData.VerificationInfor && resultData.VerificationInfor.length > 0){
                        msg = resultData.VerificationInfor[0].Message;
                    }
                    msg = mText.encode(msg);
                    $.mDialog.alert("<div>" + msg.replace(/\n|\r\n/g, "<br>") + "</div>", undefined, 0, true, true);
                }
            }
            else if (response.ResultCodeString) {
                $.mDialog.alert(response.ResultCodeString);
            }
        });
    },
    isJsonStr: function (str) {
        try {
            JSON.parse(str);
        } catch (e) {
            return false;
        }
        return true;
    },
    showSuccessMsg: function (type, importedCount, addMsgList) {
        var msg = '';
        switch (type) {
            case ImportType.InvoiceSale:
            case ImportType.InvoiceSaleRed:
            case ImportTypeExt.Sale.toString():
                msg = HtmlLang.Write(LangModule.IV, "ImportedInvoiceMsg", "{0} invoices were added.").replace("{0}", importedCount);
                break;
            case ImportType.InvoicePurchase:
            case ImportType.InvoicePurchaseRed:
            case ImportTypeExt.Purchase.toString():
                msg = HtmlLang.Write(LangModule.IV, "ImportedBillMsg", "{0} bills were added.").replace("{0}", importedCount);
                break;
            case ImportType.PayPurchase:
            case ImportTypeExt.Payment.toString():
                msg = HtmlLang.Write(LangModule.Bank, "ImportedSpendMoneyMsg", "{0} Spend Money were added.").replace("{0}", importedCount);
                break;
            case ImportType.ReceiveSale:
            case ImportTypeExt.Receive.toString():
                msg = HtmlLang.Write(LangModule.Bank, "ImportedReceiveMoneyMsg", "{0} Receive Money were added.").replace("{0}", importedCount);
                break;
            case ImportType.Item:
                msg = HtmlLang.Write(LangModule.Acct, "ImportedInventoryMsg", "{0} inventory items were added.").replace("{0}", importedCount);
                break;
            case ImportType.ExpenseItem:
                msg = HtmlLang.Write(LangModule.Acct, "ImportedExpenseItemMsg", "{0} expense items were added.").replace("{0}", importedCount);
                break;
            case ImportType.Contact:
                //msg = HtmlLang.Write(LangModule.Acct, "ImportContactsNotifyMsg", "{0} contacts were added.").replace("{0}", importedCount);
                msg = HtmlLang.Write(LangModule.Acct, "ImportedContactsNotifyMsg", "{0} contacts were imported.").replace("{0}", importedCount);
                break;
            case ImportType.Employees:
                msg = HtmlLang.Write(LangModule.Acct, "ImportEmployeesNotifyMsg", "{0} employees were added.").replace("{0}", importedCount);
                break;
            case ImportType.Account:
                msg = HtmlLang.Write(LangModule.Acct, "ImportedAccountMsg", "{0} Accounts were added.").replace("{0}", importedCount);
                break;
            case ImportType.OpeningBalance:
                msg = HtmlLang.Write(LangModule.Acct, "ImportedOpeningBalanceMsg", "{0} opening balances were added.").replace("{0}", importedCount);
                break;
            case ImportType.Voucher:
            case ImportTypeExt.Voucher.toString():
                msg = HtmlLang.Write(LangModule.Acct, "ImportedVoucherMsg", "{0} Voucher were added.").replace("{0}", importedCount);
                break;
            case ImportType.PayRun:
                msg = HtmlLang.Write(LangModule.PA, "ImportedSalaryListMsg", "{0} Salary List were added.").replace("{0}", importedCount);
                break;
            case ImportType.InFaPiao:
                msg = HtmlLang.Write(LangModule.FP, "ImportedFaPiaoListMsg", "{0} FaPiao List were added.").replace("{0}", importedCount);
                break;
            case ImportType.OutFaPiao:
                msg = HtmlLang.Write(LangModule.FP, "ImportedFaPiaoListMsg", "{0} FaPiao List were added.").replace("{0}", importedCount);
                break;
        }
        if (msg) {
            var timeout = 3000;
            if (addMsgList && addMsgList.length > 0) {
                msg += "\n (" + addMsgList.join("; ") + ")";
                timeout = 6000;
            }
            $.mMsg(msg, timeout);
        }
    }
}