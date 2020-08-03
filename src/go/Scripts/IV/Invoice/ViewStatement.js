var ViewStatement = {
    init: function () {
        ViewStatement.dataDefault();
        ViewStatement.InitFilter();
        ViewStatement.updateAction();
        ViewStatement.addressClick();
    },
    dataDefault: function () {
        $("#StateType").combobox('setValue', $("#statementType").val());
        $('#dateBetweenInput').datebox('setValue', $.mDate.format($("#startDate").val()));
        $('#dateEndInput').datebox('setValue', $.mDate.format($("#endDate").val()));
        var dt = new Date($("#endDate").val());
        $("#statementDate").html($.mDate.format(dt));
    },
    InitFilter: function () {
        var StatementType = $("#StateType").combobox("getValue");
        if (StatementType == "Outstanding") {
            $("#dateBetweenDiv").hide();
            $("#dateEndSpan").html(HtmlLang.Write(LangModule.IV, "asat", "as at"));
        }
        else if (StatementType == "Activity") {
            $("#dateBetweenDiv").show();
            $("#dateEndSpan").html(HtmlLang.Write(LangModule.IV, "end", "end"));
        }
    },
    updateAction: function () {
        $("#aUpdate").click(function () {
            var contactID = $("#statementContactID").val();
            ViewStatement.reload(contactID);
            return false;
        });
        //发送Email
        $("#btnEmail").click(function () {
            var contactID = $("#statementContactID").val();
            var StatementType = $("#StateType").combobox("getValue");
            var beginDate = (StatementType == "Activity") ? $("#dateBetweenInput").datebox("getValue") : "";
            var endDate = $("#dateEndInput").datebox("getValue");
            var jsonParam = {};
            jsonParam.ObjectIds = contactID;
            jsonParam.StatementType = StatementType;
            jsonParam.BeginDate = beginDate;
            jsonParam.EndDate = endDate;

            var paramStr = 'selectIds=' + contactID + "&sendType=2" + "&endDate=" + endDate + "&beginDate=" + beginDate + "&type=" + StatementType + "&reportType=Statements&rptJsonParam=" + escape($.toJSON(jsonParam));
            var param = $.toJSON({ type: "Statement", param: paramStr });
            Print.selectPrintSetting(param);
        });
        //打印
        $("#aPrint").click(function () {
            var statementType = $("#StateType").combobox("getValue");
            var beginDate = $("#dateBetweenInput").datebox("getValue");
            var endDate = $("#dateEndInput").datebox("getValue");
            var contactID = $("#statementContactID").val();
            var json = "{\"ObjectIds\":\"" + contactID + "\", \"StatementType\":\"" + statementType + "\", \"BeginDate\":\"" + beginDate.replace(/\//g, "-") + "\", \"EndDate\":\"" + endDate.replace(/\//g, "-") + "\"}";

            var title = HtmlLang.Write(LangModule.IV, "PrintStatement", "Print Statement");
            var param = $.toJSON({ reportType: "Statements", jsonParam: escape(json) });
            Print.previewPrint(title, param);
        });
    },
    addressClick: function () {
        $("#EditAddress").click(function () {
            ViewStatement.editAddress();
        });
        $("#AddAddress").click(function () {
            ViewStatement.editAddress();
        });
    },
    editAddress: function () {
        var contactID = $("#statementContactID").val();
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "DetailAddress", "详细地址"),
            width: 500,
            height: 450,
            href: "/IV/Invoice/EditContactAddr/" + contactID + "/ViewStatement"
        });
    },
    reload: function (contactID) {
        var startDate = $("#dateBetweenInput").datebox("getValue");
        var endDate = $("#dateEndInput").datebox("getValue");
        if (endDate == undefined || endDate == "") {
            return false;
        }
        var StatementType = $("#StateType").combobox("getValue");
        if (StatementType == "Outstanding") {
            if (startDate == undefined || startDate == "") {
                startDate = (new Date().getMonth() + 1) + "/" + new Date().getDate() + "/" + new Date().getFullYear();
            }
            if (new Date(startDate) > new Date(endDate)) {
                startDate = endDate;
            }
            ViewStatement.reloadData(contactID, StatementType, endDate, startDate);
        }
        else if (StatementType == "Activity") {
            if (startDate == undefined || startDate == "") {
                return false;
            }
            else {
                ViewStatement.reloadData(contactID, StatementType, endDate, startDate);
            }
        }
    },
    reloadData: function (contactID, StatementType, endDate, startDate) {
        mWindow.reload("/IV/Invoice/ViewStatement?"
                           + "statementType=" + StatementType + "&"
                           + "startDate=" + startDate + "&"
                           + "endDate=" + endDate + "&"
                           + "statementContactID=" + contactID);
    }
}

$(document).ready(function () {
    ViewStatement.init();
});