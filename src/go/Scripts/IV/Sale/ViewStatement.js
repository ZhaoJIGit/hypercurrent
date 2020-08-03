var ViewStatement = {
    init: function () {
        ViewStatement.dataDefault();
        ViewStatement.InitFilter();
        ViewStatement.updateAction();
        ViewStatement.addressClick();
    },
    dataDefault: function () {
        $("#StateType").combobox('setValue', $("#statementType").val());
        $('#dateBetweenInput').datebox('setValue', $("#startDate").val());
        $('#dateEndInput').datebox('setValue', $("#endDate").val());
        var dt = new Date($("#endDate").val());
        $("#statementDate").html((dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear());
    },
    InitFilter: function () {
        var StatementType = $("#StateType").combobox("getValue");
        if (StatementType == "Outstanding") {
            $("#dateBetweenDiv").hide();
            $("#dateEndSpan").html(HtmlLang.Write(LangModule.IV, "asat", "as at"));
        }
        else if (StatementType == "Activity") {
            $("#dateBetweenDiv").show();
            $("#dateEndSpan").html("and");
        }
    },
    updateAction: function () {
        $("#aUpdate").click(function () {
            var contactID = $("#statementContactID").val();
            ViewStatement.reload(contactID);
            return false;
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
            href: "/IV/Sale/EditContactAddr/" + contactID + "/ViewStatement"
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
        mWindow.reload("/IV/Sale/ViewStatement?"
                           + "statementType=" + StatementType + "&"
                           //+ "startDate=" + startDate.replace(/\//g, "-") + "&"
                           + "startDate=" + startDate + "&"
                           + "endDate=" + endDate + "&"
                           + "statementContactID=" + contactID);
    }
}

$(document).ready(function () {
    ViewStatement.init();
});