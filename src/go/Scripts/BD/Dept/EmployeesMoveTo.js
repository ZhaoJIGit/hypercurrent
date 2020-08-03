var EmployeesMoveTo = {
    init: function () {
        EmployeesMoveTo.saveClick();
        EmployeesMoveTo.cancelClick();
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var empids = $("#empids").val();
            var deptids = $('#deptSelectID').combobox('getValues').toString();
            mAjax.submit(
                "/BD/Dept/EmpsAddToDepts", 
                { empids: empids, deptids: deptids }, 
                function (msg) {
                    if (msg) {
                        $.mMsg(HtmlLang.Write(LangModule.BD, "EmpsAddToDepts", "Move Employess Successfully"));
                        if (parent.DeptList) {
                            parent.DeptList.reloadData();
                        }
                        Megi.closeDialog();
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            Megi.closeDialog();
            
            return false;
        });
    }

}


$(document).ready(function () {
    EmployeesMoveTo.init();
});