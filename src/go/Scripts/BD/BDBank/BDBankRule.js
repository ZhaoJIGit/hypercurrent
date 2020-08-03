/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var BDBankRule = {
    IsClick: false,
    init: function () {
        BDBankRule.bindGrid();
        BDBankRule.initAction();
        if (Megi.request("type") == "new") {
            BDBankRule.editRule("");
        }
    },
    initAction: function () {
        $("#lbtnAddRule").click(function () {
            BDBankRule.editRule("");
        });

        $("#aDeleteBankRule").click(function () {
            Megi.grid("#tbBankRule", "deleteSelected", {
                url: "/BD/BDBank/DeleteBDBankRule", callback: function () {
                    BDBankRule.bindGrid();
                }
            });
        });
    },
    bindGrid: function () {
        Megi.grid('#tbBankRule', {
            resizable: true,
            auto: true,
            url: "/BD/BDBank/GetBDBankRuleList",
            columns: [[{
                title: '<input type=\"checkbox\" >', field: 'MItemID', width: 40, align: 'center', formatter: function (value, rec, rowIndex) {
                    if (rec.MBankName) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" >";
                    } else {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" disabled=\"disabled\" >";
                    }
                }
            },
                {
                    title: LangKey.Name, field: 'MName', width: 300, align: 'left', sortable: true, formatter: function (value, rec, rowIndex) {
                        return "<a href='javascript:void(0)' onclick='BDBankRule.editRule(\"" + rec.MItemID + "\");'>" + value + "</a>";
                    }
                },
                 /*添加银行账户的字段  银行账户 */
                 {
                     title: HtmlLang.Write(LangModule.Bank, "bankAccount", "银行账户"), field: 'MBankName', width: 150, align: 'left', sortable: true, formatter: function (value, rec, rowIndex) {

                         var mBankName = HtmlLang.Write(LangModule.Bank, "AllBankAccounts", "所有银行账户");
                         if (!value) {
                             value = mBankName;
                         } 
                         return "<a href='javascript:void(0)'>" + value + "</a>";
                     }
                 },

                {
                    title: HtmlLang.Write(LangModule.Bank, "Amount", "Amount"), field: 'MChkAmount', width: 120, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                        var chk = value ? "checked='checked'" : "";
                        return "<input type=\"checkbox\" disabled='disabled' " + chk + " >";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Payee", "Payee"), field: 'MChkPayee', width: 120, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                        var chk = value ? "checked='checked'" : "";
                        return "<input type=\"checkbox\" disabled='disabled' " + chk + " >";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Reference", "Reference"), field: 'MChkRef', width: 120, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                        var chk = value ? "checked='checked'" : "";
                        return "<input type=\"checkbox\" disabled='disabled' " + chk + " >";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "TransitionDate", "Transition Date"), field: 'MChkTransDate', width: 120, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                        var chk = value ? "checked='checked'" : "";
                        return "<input type=\"checkbox\" disabled='disabled' " + chk + " >";
                    }
                }
            ]]
        });
    },
    editRule: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Bank, "CreateRule", "Create Rule") : HtmlLang.Write(LangModule.Bank, "EditRule", "Edit Rule")
        Megi.dialog({
            title: title,
            width: 500,
            height: 350,
            href: '/BD/BDBank/BDBankRuleEdit/' + id
        });
    },
    viewRule: function (id) {
        var title = HtmlLang.Write(LangModule.Bank, "ViewRule", "View Rule");
        Megi.dialog({
            title: title,
            width: 500,
            height: 350,
            href: '/BD/BDBank/BDBankRuleView/' + id
        });
    },
    reload: function () {
        BDBankRule.bindGrid();
    },
    afterEdit: function (msg) {
        Megi.displaySuccess("#divMessage", msg);
        BDBankRule.bindGrid();
    }
}
$(document).ready(function () {
    BDBankRule.init();
});