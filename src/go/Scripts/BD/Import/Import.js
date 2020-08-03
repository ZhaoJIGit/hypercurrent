/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="ImportBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var Import = {
    type: $("#hidType").val(),
    isCover: $("#hideIsCover").val(),
    init: function () {
        $.mDialog.min();
        //设置下拉按钮弹出层宽度跟主按钮宽度一致
        $("#divSalaryTmplList").width($("#aSalaryTmplList").outerWidth() - 2);
        Import.bindAction();
    },
    bindAction: function () {
        $("#aImport").click(function () {
            if (!ImportBase.validateFile()) {
                return;
            }
            if (Import.type == ImportType.Account) {
                var msg = HtmlLang.Write(LangModule.BD, "ImportAcountTips", "通过导出科目表，在导出的科目表里对科目进行新增的方式准备科目表会比较便捷。尽量不要直接在空白模板上粘贴原科目表，否则就需要一一进行匹配操作。");
                $.mDialog.alert(msg, function () {
                    Import.doImport();
                }, 0, false, true);
                $("#popup_ok", parent.document).val(HtmlLang.Write(LangModule.Common, "IKnow", "我知道了"));
                return;
            } else if (Import.type == ImportType.OpeningBalance) {
                //如果是科目余额，需要先校验是否存在自动推送单据存在核销记录

                var url = "/GL/GLInitBalance/CheckAutoCreateBillHadVerifiyRecord";

                $.mAjax.post(url, {}, function (data) {
                    //如果存在核销记录，需要提示
                    if (data && data.Success) {
                        var tips = HtmlLang.Write(LangModule.BD, "ImportAccountBalanceTips", "科目期初余额推送的单据存在核销记录，如果导入，将会清除掉这些核算关系，是否继续？");

                        if (data.MessageList && data.MessageList.length > 0) {
                            for (var i = 0 ; i < data.length; i++) {
                                tips += "</br>" + mText.encode(data.MessageList[i]);
                            }
                            tips = "<div>" + tips + "</div>";
                        }

                        $.mDialog.confirm(tips, function () {
                            Import.doImport();
                        });
                    } else {
                        Import.doImport();
                    }
                }, false, true);
                return;
            }

            Import.doImport();
        });

        $("#aCancel").click(function () {
            $.mDialog.close();
        });
        $("#fileInput").change(function () {
            ImportBase.onFileChanged(this);

            var validateResult = FileBase.validateFile(this.value, 0, FileBase.excelExcludeCsvRegex);
            if (validateResult) {
                ImportBase.clearFile(this);
                $.mDialog.alert(validateResult);
            }
        });
        $("#divSalaryTmplList .menu-item").live('click', function () {
            var period = this.tagName == "a" ? $(this).attr("data-id") : $(this).find("a").attr("data-id");
            if (period) {
                Import.downloadTemplate(period);
            }
        });
    },
    doImport: function () {
        $(".m-imain").mask("");
        $("#fileSelectForm").submit();
    },
    appendParamByType: function (url) {
        if (Import.type == ImportType.ReceiveSale || Import.type == ImportType.PayPurchase) {
            var urlParamSpliter = url.lastIndexOf('?') != -1 ? '&' : '?';
            url += urlParamSpliter + 'accountId=' + $("#hidAccountId").val() + '&contactType=' + $("#hidContactType").val();
        }
        return url;
    },
    downloadTemplate: function (period) {
        var url = '/BD/Import/DownloadImportTemplate?id=' + Import.type + "&isCover=" + Import.isCover + "&period=" + period;
        ImportBase.downloadTemplate(Import.appendParamByType(url));
    },
    showConfirmDialog: function (msg, toUrl, isAlertAndContinue) {
        if (isAlertAndContinue) {
            //弹出提示后，点确定继续执行
            $.mDialog.alert(msg, function () {
                mWindow.reload(toUrl);
            }, 0, false, true);
        }
        else {
            $.mDialog.confirm(msg,
            {
                callback: function () {
                    mWindow.reload(toUrl);
                }
            });
        }
    }
}

$(document).ready(function () {
    Import.init();
    var url = "/BD/Import/ConfirmImport?id=" + Import.type + "&isCover=" + Import.isCover;
    var callBack;
    if (Import.type == ImportType.PayRun || Import.type == ImportType.Contact) {
        callBack = function (response, toUrl) {
            if (response) {
                var msg = response.Message;
                var tag = response.Tag;
                var importCnt = response.ImportCnt;
                var success = response.Success;
                var decodeMsg = mText.decode(response.Message);
                var isJson = ImportBase.isJsonStr(decodeMsg);
                if (isJson || tag == "Exist") {
                    if (isJson) {
                        response = JSON.parse(decodeMsg);
                        msg = response.Message.Message;
                        tag = response.Message.Tag;
                        success = response.Message.Success;
                        importCnt = response.ImportCnt;
                        toUrl += importCnt;
                    }
                    
                    //msg = msg.replace(/(\r\n|\n|\r)/gm, "");
                    if (tag == "Exist" || tag == "AlertAndContinue") {
                        Import.showConfirmDialog(msg, toUrl, tag == "AlertAndContinue");
                    }
                    else if (!success) {
                        ImportBase.clearFile();
                        $.mDialog.alert(msg);
                    }
                }
                else if (response.Success) {
                    mWindow.reload(toUrl);
                }
                else {
                    ImportBase.clearFile();
                    msg = mText.encode(msg);
                    $.mDialog.alert("<div>" + msg.replace(/\n|\r\n/g, "<br>") + "</div>", undefined, 0, true, true);
                }
            }
            $("body").unmask();
        };
    } else {

    }
    ImportBase.initAjaxForm(Import.appendParamByType(url), callBack);
});