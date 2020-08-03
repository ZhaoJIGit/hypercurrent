var AccountMatchLog = {
    init: function () {
        AccountMatchLog.bindTree();

        $(window).resize(function () {
            AccountMatchLog.initUI();
        });
    },
    initUI: function () {
        try {
            $("#tbLogList").treegrid('resize', {
                width: $(".m-imain-content").width(),
                height: $(".m-imain").height() - 36
            });
        } catch (exc) { }
    },
    bindTree: function () {
        $("#tbLogList").treegrid({
            url: '/BD/BDAccount/GetAccountMatchLog',
            idField: 'MNumber',
            treeField: 'text',
            checkbox: true,
            fitColumns: true,
            singleSelect: false,
            scrollY: true,
            lines: true,
            region: "center",
            columns: [[
                { title: HtmlLang.Write(LangModule.Acct, "ImportAccountCode", "导入科目代码"), field: 'MNumber', width: 130, sortable: false },
                { title: HtmlLang.Write(LangModule.Acct, "ImportAccountName", "导入科目名称"), field: 'text', width: 208, sortable: false },
                { title: HtmlLang.Write(LangModule.Acct, "SystemAccount", "系统科目名称"), field: 'MMatchNumber', width: 218, sortable: false },
                { title: HtmlLang.Write(LangModule.Acct, "NewAccountCode", "新增科目代码"), field: 'MNewNumber', width: 180, sortable: false }
            ]],
            onLoadSuccess: function (row, data) {
                AccountMatchLog.initUI();
            },
            onClickCell: function (field, index, value) {
                $(this).treegrid("unselectAll");
            }
        });
    }
};

$(document).ready(function () {
    AccountMatchLog.init();
});