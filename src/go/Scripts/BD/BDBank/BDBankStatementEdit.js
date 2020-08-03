/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;


var BDBankStatementEdit = {
    reconcileState: [
        HtmlLang.Write(LangModule.Bank, "unreconciled", "Unreconciled"),
        HtmlLang.Write(LangModule.Bank, "Reconciled", "Reconciled"),
        HtmlLang.Write(LangModule.Common, "deleted", "Deleted")
    ],
    init: function () {
        BDBankStatementEdit.saveClick();
        BDBankStatementEdit.cancelClick();
        BDBankStatementEdit.gridBind();
        $("body").mFormGet({ url: "/BD/BDBank/GetBDBankStatementModel/" });
    },
    saveClick: function () {
        $("#aSave").click(function () {
            BDBankStatementEdit.endEdit();
            var statementId = $("#statementId").val();
            var rows = $("#tbStatementDetail").datagrid("getRows");
            mAjax.submit(
                "/BD/BDBank/StatementUpdate",
                { viewModels: rows },
                function (result) {
                    if (result.Result) {
                        mMsg(LangKey.SaveSuccessfully);
                        //更新
                        $.mTab.addOrUpdate();
                    }
                });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            //数据重做
            $.mTab.addOrUpdate();
        });
    },
    gridBind: function () {
        var statementId = $("#statementId").val();
        Megi.grid("#tbStatementDetail", {
            resizable: true,
            auto: true,
            url: "/BD/BDBank/GetBDStatementDetails/" + statementId,
            columns: [[
                { title: HtmlLang.Write(LangModule.Bank, "Date", "Date"), field: 'MDate', width: 50, formatter: mDate.formatter },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Type", "Type"), field: 'MTransType', width: 50, editor: {
                        type: 'combobox', options: {
                            valueField: 'label',
                            textField: 'value',
                            data: [{
                                label: 'other',
                                value: 'other'
                            }, {
                                label: 'debit',
                                value: 'debit'
                            }]
                        }
                    }
                },
                { title: HtmlLang.Write(LangModule.Bank, "Payee", "Payee"), field: 'MTransAcctName', width: 50, editor: { type: 'text' } },
                { title: HtmlLang.Write(LangModule.Bank, "Particulars", "Particulars"), field: 'MTransAcctNo', width: 50, editor: { type: 'text' } },
                { title: HtmlLang.Write(LangModule.Bank, "Code", "Code"), field: 'MCode', width: 50, editor: { type: 'text' }, hidden: true },
                { title: HtmlLang.Write(LangModule.Bank, "Reference", "Reference"), field: 'MDesc', width: 50, editor: { type: 'text' } },
                { title: HtmlLang.Write(LangModule.Bank, "AnalysisCode", "Analysis Code"), field: 'MAnalysisCode', width: 50, editor: { type: 'text' }, hidden: true },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Spent", "Spent"), field: 'MSpentAmt', width: 50, align: "right", formatter: function (value) {
                        return mMath.toMoneyFormat(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Received", "Received"), field: 'MReceivedAmt', width: 50, align: "right", formatter: function (value) {
                        return mMath.toMoneyFormat(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Balance", "Balance"), field: 'MBalance', width: 50, align: "right", formatter: function (value) {
                        return mMath.toMoneyFormat(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Status", "Status"), field: 'MCheckState', width: 50, align: "center", formatter: function (value, row, index) {
                        switch (value) {
                            case "1":
                                return "<span class='mark Check' >" + BDBankStatementEdit.reconcileState[value] + "</span>";
                            case "0":
                                return "<span class='mark UnCheck' >" + BDBankStatementEdit.reconcileState[value] + "</span>";
                            case "2":
                                return "<span class='mark Deleted' >" + BDBankStatementEdit.reconcileState[value] + "</span>";
                            default:
                                break;
                        }
                    }
                },
                { title: '', field: 'MEntryID', hidden: true },
            ]],
            onClickCell: function (index, field, value) {
                switch (field) {
                    case "MDate":
                    case "MSpentAmt":
                    case "MReceivedAmt":
                    case "MBalance":
                    case "MStatus":
                        BDBankStatementEdit.endEdit();
                        return false;
                    default:
                        break;
                }
                $(this).datagrid('beginEdit', index);
                var ed = $(this).datagrid('getEditor', { index: index, field: field });
                if (ed != null) {
                    $(ed.target).focus();
                }
            }
        });
    },
    endEdit: function () {
        var recordLength = $("#tbStatementDetail").datagrid("getRows").length;
        for (var i = 0; i < recordLength; i++) {
            $("#tbStatementDetail").datagrid("endEdit", i);
        }
    }
}


$(document).ready(function () {
    BDBankStatementEdit.init();
});