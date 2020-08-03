/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var TaxFillingList = {
    Sort: "asc",
    init: function () {
        TaxFillingList.bindGrid(true);
        TaxFillingList.bindAction();
    },
    bindGrid: function () {
        Megi.grid("#tbTaxFilingList", {
            pagination: true,
            url: "/Report/ETax/GetTaxFillList",
            sortOrder: TaxFillingList.Sort,
            queryParams: {},
            columns: [[
                {
                    title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    }, width: 40, align: 'center', sortable: false
                },
                { title: "Report Type Code", field: 'MCode', width: 200, align: 'left', sortable: true },
                { title: HtmlLang.Write(LangModule.BD, "ItemName", "商品名称"), field: 'MDesc', width: 300, sortable: true },
                { title: HtmlLang.Write(LangModule.BD, "CostPrice", "Cost Price"), field: 'MPurPrice', width: 140, align: 'right', sortable: true, formatter: function (value, rec, rowIndex) { return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 8, 2); } },
                { title: HtmlLang.Write(LangModule.BD, "SalePrice", "Sale Price"), field: 'MSalPrice', width: 140, align: 'right', sortable: true, formatter: function (value, rec, rowIndex) { return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 8, 2); } },
                {
                    title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), field: 'Action', align: 'center', width: 120, sortable: false, formatter: function (value, rec, rowIndex) {

                        if (TaxFillingList.hasChangeAuth) {
                            return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"$.mTab.addOrUpdate('纳税申报表','/Report/ETax/TaxFilingView')\" class='list-item-edit'></a><a href='javascript:void(0);' onclick=\"TaxFillingList.IsListActionClick = true;TaxFillingList.deleteItem('" + rec.MItemID + "');\" class='list-item-del'></a></div>";
                        } else {
                            return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"$.mTab.addOrUpdate('纳税申报表','/Report/ETax/TaxFilingView/" + rec.MID + "')\" class='list-item-edit'></a></div>";
                        }
                    }
                }]]
        });

    },

    bindAction: function () {
        $("#aGetReport").click(function () {
            TaxFillingBase.post("/Report/ETax/AddGetReportTaxTask", {}, function () {
                TaxFillingList.bindGrid();
            });
        });
    }
}

$(document).ready(function () {
    TaxFillingList.init();
});