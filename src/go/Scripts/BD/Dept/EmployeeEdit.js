/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var EmployeeEdit = {
    init: function () {
        EmployeeEdit.saveClick();
        EmployeeEdit.cancelClick();
        EmployeeEdit.deptDefault();
        $("body").mFormGet({
            url: "/BD/Dept/GetEmployeeModel", fill: true, callback: function () {
                EmployeeEdit.deptDefault();
            }
        });
    },
    deptDefault: function () {
        var deptDefaultID = $("#deptDefaultID").val();
        if (deptDefaultID != undefined && deptDefaultID != "") {
            $('#deptSelectID').combobox('setValues', deptDefaultID.split(','));
        }
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var obj = {};
            obj.MDeptIDS = $('#deptSelectID').combobox('getValues').toString();
            $("#divEmployeeEdit").mFormSubmit({
                url: "/BD/Dept/EmployeeUpdate", param: obj, callback: function (msg) {
                    if (msg.Success) {
                        Megi.closeDialog();
                        parent.DeptList.reloadData();
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            //var orgid = $("#orgid").val();
            //var deptid = $("#deptDefaultID").val();
            //var empid = $("#empid").val();
            Megi.closeDialog();
            parent.DeptList.reloadData();
            return false;
        });
    }

}


$(document).ready(function () {
    EmployeeEdit.init();
});