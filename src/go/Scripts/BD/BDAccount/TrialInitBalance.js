var TrialInitBalance = {
    glMonth:$("#hideGLMonth").val(),
    init: function () {
        TrialInitBalance.initForm();
    },
    initForm: function () {
        //直接拼个json

        $("#tbAll").datagrid({
            fitColumns: true,
            columns: [[
            { title: " ", field: 'Item', width: 100 },
            {
                title: HtmlLang.Write(LangModule.Acct, "DebitTotalAmount", "借方总额"), field: 'DebitAmount', width: 140, align: 'right', formatter: function (value, rec, rowIndex) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            {
                title: HtmlLang.Write(LangModule.Acct, "CreditTotalAmount", "贷方总额"), field: 'CreditAmount', width: 140, align: 'right', formatter: function (value, rec, rowIndex) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            {
                title: HtmlLang.Write(LangModule.Acct, "Difference", "Difference"), field: 'Difference', width: 120, align: 'right', formatter: function (value, rec, rowIndex) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            ]]
        });

        mAjax.post(
            "/BD/BDAccount/CheckInitBalance",
            "", 
            function (msg) {
                if (msg) {
                    var text = "";
                    if (msg.Success) {
                        text = HtmlLang.Write(LangModule.Acct, "TrialBalanceSuccess", "Congratulations, you input the initial balance is the balance!");
                        $("#lblMessage").attr("style", "color: #689800;");
                        $("#lblMessage").text(text);

                    } else {
                        text = HtmlLang.Write(LangModule.Acct, "TrialBalanceFail", "Sorry, you input the initial balance is not balanced, please input again!");
                        $("#lblMessage").attr("style", "color: red;");
                        $("#lblMessage").text(text);
                    }
                    
                    var initBalanceTitle = HtmlLang.Write(LangModule.Acct, "InitialBalance", "Opening balance");

                    var data = new Array();

                    var initBalanceItem = { Item: initBalanceTitle, DebitAmount: msg.DebitAmount, CreditAmount: msg.CreditAmount, Difference: (msg.DebitAmount - msg.CreditAmount).toFixed(2) };
                    data.push(initBalanceItem);

                    //1月不现实本年累计
                    if (TrialInitBalance.glMonth !== "1") {
                        var ytdBalanceItem = {
                            Item: HtmlLang.Write(LangModule.Acct, "CumulativeAmount", "本年累计合计"), DebitAmount: msg.YtdDebit, CreditAmount: msg.YtdCredit,
                            Difference: (msg.YtdDebit - msg.YtdCredit).toFixed(2)
                        };

                        data.push(ytdBalanceItem);
                    }

                    $("#tbAll").datagrid("loadData", data);

                }
            });
    }
}

$(document).ready(function () {
    TrialInitBalance.init();
});