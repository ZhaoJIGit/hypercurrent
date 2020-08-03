var Statements = {
    isInit: false,
    init: function () {
        Statements.isInit = true;
        Statements.initDefaultDate();
        Statements.InitFilter();
        Statements.reloadData();
        Statements.updateAction();
        Statements.isInit = false;
    },
    initDefaultDate: function () {
        $('#dateBetweenInput').datebox('setValue', $.mDate.format($("#FirstDayOfMonth").val()));
    },
    InitFilter: function () {
        var StatementType = $("#StatementType").combobox("getValue");
        if (StatementType == "Outstanding") {
            $('#dateEndInput').datebox('setValue', $.mDate.format(new Date()));
            $("#dateBetweenDiv").hide();
        }
        else if (StatementType == "Activity") {
            $('#dateEndInput').datebox('setValue', $.mDate.format($("#LastDayOfMonth").val()));
            $("#dateBetweenDiv").show();
        }
    },
    updateAction: function () {
        $("#aUpdate").click(function () {
            Statements.reloadData();
            return false;
        });
        //发送Email
        $("#btnEmail").click(function () {
            var StatementType = $("#StatementType").combobox("getValue");
            var beginDate = (StatementType == "Activity") ? $("#dateBetweenInput").datebox("getValue") : "";
            var endDate = $("#dateEndInput").datebox("getValue");
            var jsonParam = {};
            jsonParam.StatementType = StatementType;
            jsonParam.BeginDate = beginDate;
            jsonParam.EndDate = endDate;
            jsonParam.Filter = Statements.getSearchText();
            Megi.grid("#StatementsData", "optSelected", {
                callback: function (ids) {
                    jsonParam.ObjectIds = ids;

                    //var param = 'selectIds=' + ids + "&sendType=2" + "&beginDate=" + beginDate + "&endDate=" + endDate + "&type=" + StatementType + "&reportType=Statements&rptJsonParam=" + escape($.toJSON(jsonParam));
                    //Megi.dialog({
                    //    title: HtmlLang.Write(LangModule.IV, "SelectPrintSetting", "Select Print Setting"),
                    //    width: 450,
                    //    height: 270,
                    //    href: '/IV/Invoice/SelectPrintSetting?type=Statements&param=' + param
                    //});

                    var paramStr = 'selectIds=' + ids + "&sendType=2" + "&beginDate=" + beginDate + "&endDate=" + endDate + "&type=" + StatementType + "&reportType=Statements&rptJsonParam=" + escape($.toJSON(jsonParam));
                    var param = $.toJSON({ type: "Statements", param: paramStr });
                    Print.selectPrintSetting(param);
                }
            });
        });
        //打印
        $("#aPrint").click(function () {
            var beginDate = $("#dateBetweenInput").datebox("getValue");
            var endDate = $("#dateEndInput").datebox("getValue");
            var statementType = $("#StatementType").combobox("getValue");
            Megi.grid("#StatementsData", "optSelected", {
                callback: function (ids) {
                    var paramObj = {};
                    paramObj.StatementType = statementType;
                    paramObj.ObjectIds = ids;
                    paramObj.BeginDate = beginDate.replace(/\//g, "-");
                    paramObj.EndDate = endDate.replace(/\//g, "-");
                    paramObj.Filter = Statements.getSearchText();

                    var title = HtmlLang.Write(LangModule.IV, "PrintStatements", "Print Statements");
                    var param = $.toJSON({ reportType: "Statements", jsonParam: escape($.toJSON([paramObj])) });
                    Print.previewPrint(title, param);
                }
            });
        });
    },
    bindGrid: function (StatementType, endDate, startDate) {
        var searchText = Statements.getSearchText();
        Megi.grid('#StatementsData', {
            resizable: true,
            auto: true,
            url: "/IV/Invoice/GetStatementData",
            queryParams: { StartDate:startDate.replace(/\//g, "-"), EndDate: endDate.replace(/\//g, "-"), Filter: searchText, StatementType: StatementType },
            columns: [[
                {
                    title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    }, width: 20, align: 'center', sortable: false
                },
                {
                    title: HtmlLang.Write(LangModule.Common, "Name", "Name"), field: 'MName', width: 100, align: 'left', sortable: true
                    , formatter: function (value, row, index) {
                        return "<a href='javascript:void(0)' onclick='$.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, \"ViewStatement\", \"View Statement\"),\"/IV/Invoice/ViewStatement?"
                                + "statementType=" + StatementType + "&"
                                + "startDate=" + startDate + "&"
                                + "endDate=" + endDate + "&"
                                + "statementContactID=" + row.MItemID + "\")' >" + value + "</a>";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Common, "Email", "Email"), field: 'MEmail', width: 100, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                        if (!rec.MIsActive)
                        {
                            return value;
                        }
                        else
                        {
                            if (value == "")
                            {
                                value = HtmlLang.Write(LangModule.IV, "EditAddressDetails", "Edit address details");
                            }
                            return "<a href=\"javascript:void(0)\" onclick=\"Statements.InputAddr('" + rec.MItemID + "')\">" + value + "</a>";
                        }
                        
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Common, "Address", "Address"), field: 'MAddress', width: 100, sortable: true, formatter: function (value, rec, rowIndex) {
                        if (value=="") {
                            return "<a href=\"javascript:void(0)\" onclick=\"Statements.InputAddr('" + rec.MItemID + "')\">" + HtmlLang.Write(LangModule.IV, "AddAddress", "Add address") + "</a>";
                        }
                        else {
                            return value.trimStart(',');
                        }
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "OutstandingBalance", "Outstanding Balance"), field: 'MBalance', width: 100, sortable: true, formatter: function (value, rec, rowIndex) {
                        return "<span style='float:right'>" + Megi.Math.toMoneyFormat(value) + "</span>";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Overdue", "Overdue"), field: 'MOverdue', width: 100, sortable: true, formatter: function (value, rec, rowIndex) {
                        return "<span style='float:right'>" + Megi.Math.toMoneyFormat(value) + "</span>";
                    }
                }
            ]],
            onLoadSuccess: function (data) {
                //初始化时增加100ms延时，否则滚动条还没出现
                var timeOut = Statements.isInit ? 100 : 0;
                setTimeout(function () {
                    Statements.resizeGrid()
                }, timeOut);
            }
        });
    },
    resizeGrid: function () {
        try {
            $("#StatementsData").datagrid('resize', {
                width: $(".m-tab-toolbar").width()
            });
        } catch (exc) { }
    },
    getSearchText: function() {
        return $("#searchText").val() == HtmlLang.Write(LangModule.Acct, "ConEmailPosAddress", "Contact, email or postal address") ? "" : $("#searchText").val();
    },
    InputAddr: function (contactID) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "DetailAddress", "详细地址"),
            width: 500,
            height: 450,
            href: "/IV/Invoice/EditContactAddr/" + contactID + "/Statements"
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