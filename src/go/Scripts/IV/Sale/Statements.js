var Statements = {
    init: function () {
        Statements.initDefaultDate();
        Statements.InitFilter();
        Statements.reloadData();
        Statements.updateAction();
    },
    initDefaultDate: function () {
        $('#dateBetweenInput').datebox('setValue', $.mDate.format($("#FirstDayOfMonth").val()));
    },
    InitFilter: function () {
        var StatementType = $("#StatementType").combobox("getValue");
        if (StatementType == "Outstanding") {
            $('#dateEndInput').datebox('setValue', $.mDate.format(new Date()));
            $("#dateBetweenDiv").hide();
            $("#dateEndSpan").html(HtmlLang.Write(LangModule.Acct, "Asat", "as at"));
        }
        else if (StatementType == "Activity") {
            $('#dateEndInput').datebox('setValue', $("#LastDayOfMonth").val());
            $("#dateBetweenDiv").show();
            $("#dateEndSpan").html(HtmlLang.Write(LangModule.Acct, "And", "and"));
            Statements.firstSelectActivity = false;
        }
    },
    updateAction: function () {
        $("#aUpdate").click(function () {
            Statements.reloadData();
            return false;
        });
    },
    bindGrid: function (StatementType, endDate, startDate) {
        var searchText = $("#searchText").val() == HtmlLang.Write(LangModule.Acct, "ConEmailPosAddress", "Contact, email or postal address") ? "" : $("#searchText").val();
        Megi.grid('#StatementsData', {
            resizable: true,
            auto: true,
            url: "/IV/Sale/GetStatementData",
            queryParams: { EndDate: endDate.replace(/\//g, "-"), Filter: searchText },
            columns: [[
                {
                    title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    }, width: 20, align: 'center', sortable: true
                },
                {
                    title: HtmlLang.Write(LangModule.Common, "Name", "Name"), field: 'MName', width: 100, align: 'center', sortable: true
                    , formatter: function (value, row, index) {
                        return "<a href=\"/IV/Sale/ViewStatement?"
                                + "statementType=" + StatementType + "&"
                                + "startDate=" + startDate + "&"
                                + "endDate=" + endDate + "&"
                                + "statementContactID=" + row.MItemID + "\" >" + value + "</a>";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Common, "Email", "Email"), field: 'MEmail', width: 100, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                        if (value == "") { value = " Edit address details"; }
                        return "<a href=\"javascript:void(0)\" onclick=\"Statements.InputAddr('" + rec.MItemID + "')\">" + value + "</a>";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Common, "Address", "Address"), field: 'MAddress', width: 100, sortable: true, formatter: function (value, rec, rowIndex) {
                        if (value=="") {
                            return "<a href=\"javascript:void(0)\" onclick=\"Statements.InputAddr('" + rec.MItemID + "')\">" + "Add address" + "</a>";
                        }
                        else {
                            return value;
                        }
                    }
                },
                    { title: HtmlLang.Write(LangModule.Bank, "OutstandingBalance", "Outstanding Balance"), field: 'MBalance', width: 100, sortable: true },
                    { title: HtmlLang.Write(LangModule.Bank, "Overdue", "Overdue"), field: 'MOverdue', width: 100, sortable: true }

            ]]
        });
    },
    InputAddr: function (contactID) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "DetailAddress", "详细地址"),
            width: 500,
            height: 450,
            href: "/IV/Sale/EditContactAddr/" + contactID + "/Statements"
        });
    },
    reloadData: function () {
        var startDate = $("#dateBetweenInput").datebox("getValue");
        var endDate = $("#dateEndInput").datebox("getValue");
        if (endDate == undefined || endDate == "") {
            return false;
        }
        var StatementType = $("#StatementType").combobox("getValue");
        if (StatementType == "Outstanding") {
            if (startDate == undefined || startDate == "") {
                startDate = (new Date().getMonth() + 1) + "/" + new Date().getDate() + "/" + new Date().getFullYear();
            }
            if (new Date(startDate) > new Date(endDate)) {
                startDate = endDate;
            }
            Statements.bindGrid(StatementType, endDate, startDate);
        }
        else if (StatementType == "Activity") {
            if (startDate == undefined || startDate == "") {
                return false;
            }
            else {
                Statements.bindGrid(StatementType, endDate, startDate);
            }
        }
    }
}

$(document).ready(function () {
    Statements.init();
});