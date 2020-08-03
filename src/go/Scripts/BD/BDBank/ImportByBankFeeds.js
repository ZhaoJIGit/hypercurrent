/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ImportByBankFeeds = {
    curStep: 0,
    bankId: $("#hidBankId").val(),
    bankTypeId: $("#hidBankTypeId").val(),
    init: function () {
        //银行ID为空时禁止导入
        if (!ImportByBankFeeds.bankId) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "bankIdIsEmpty", "Not find bank account,please select a bank account"));
            return false;
        }
        ImportByBankFeeds.initStep(ImportByBankFeeds.bankTypeId);
        localStorage.clear();
        ImportByBankFeeds.bindAction();
    },
    initStep: function (bankTypeId) {
        var step = 1;
        if (bankTypeId == "BOS") {
            step = 2;
        }
        ImportByBankFeeds.goStep(step);
    },
    goStep: function (step) {
        var label;
        ImportByBankFeeds.curStep = step;
        if (step == 1) {
            $("#divLogin").show();
            $("#divDatePeriod").hide();
            label = HtmlLang.Write(LangModule.Bank, "Next", "Next");
        }
        else {
            $("#divDatePeriod").show();
            $("#divLogin").hide();
            label = HtmlLang.Write(LangModule.Bank, "Finish", "Finish");
        }
        $("#aImport .l-btn-text").removeClass("l-btn-empty").text(label);
    },
    validateLogin: function () {
        if (!$("#divLogin").mFormValidate()) {
            return;
        }

        $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "UnsupportImportStatmentOnline", "This bank has not support importing statement online!"));
        //校验银行登录信息
        //for test
        //setTimeout(function () {
        //    ImportByBankFeeds.goStep(2);
        //}, 1000);
    },
    getDataByDateRange: function(){
        var startDate = $('#StartDate').datebox('getValue');
        var endDate = $('#EndDate').datebox('getValue');
        if (!$("#divDatePeriod").mFormValidate()) {
            return;
        }
        if (ImportByBankFeeds.dateDiff(startDate) <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "StartdateMustLessThanToday", "Start date must be less than today."));
            return;
        }
        if (ImportByBankFeeds.dateDiff(endDate) <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "EnddateMustLessThanToday", "End date must be less than today."));
            return;
        }
        if (endDate < startDate) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "EnddateMustGreaterThanStartdate", "End date must be greater than or equal to Start date."));
            return;
        }
        if (ImportByBankFeeds.dateDiff(startDate, endDate) > 31) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "DatediffCannotMorethan31Days", "The time period for the end date and the start date cannot more than 31 days."));
            return;
        }
        var obj = {};
        obj.AcctID = ImportByBankFeeds.bankId;
        obj.StartDate = startDate;
        obj.EndDate = endDate;
        mAjax.post(
            "/BD/BDBank/GetBankFeeds",
            { feedModel: obj },
            function (data) {
                if (data.Success) {
                    var msg = HtmlLang.Write(LangModule.Bank, "ImportStatementSuccess", "The statement import success!");
                    $.mMsg(msg);
                    parent.mWindow.reload();
                    $.mDialog.close();
                } else {
                    $.mDialog.alert(data.Message.replace('\r\n', ''));
                }
            }, "", true);
    },
    bindAction: function () {
        $("#aImport").click(function () {
            if (ImportByBankFeeds.curStep == 1) {
                ImportByBankFeeds.validateLogin();
            }
            else {
                ImportByBankFeeds.getDataByDateRange();
            }
        });

        $("#aCancel").click(function () {
            ImportBase.closeImportBox();
        });
    },
    dateDiff: function (sdt, eDt) {
        var endDate = eDt == undefined ? $.mDate.format(new Date()) : eDt;
        return (Date.parse(endDate) - Date.parse(sdt)) / (1000 * 24 * 60 * 60);
    }
}

$(document).ready(function () {
    ImportByBankFeeds.init();
});