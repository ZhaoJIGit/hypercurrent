var DeptEdit = {
    init: function () {
        DeptEdit.saveClick();
        DeptEdit.cancelClick();
        DeptEdit.loadData();
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var orgid = $("#orgid").val();
            var parentid = $("#parentid").val();
            var selid = parentid == "" || parentid == undefined ? orgid : parentid;


            $("#divDeptEdit").mFormSubmit({
                url: "/BD/Dept/DeptUpdate", callback: function (msg) {
                    if (msg.Success) {
                        if (parent.DeptList) {
                            parent.DeptList.reloadTree(selid);
                        }
                        $.mMsg(HtmlLang.Write(LangModule.BD, "OperationSuccessful", "Operation Successfully"));
                        $.mDialog.close();
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            var orgid = $("#orgid").val();
            var parentid = $("#parentid").val();
            var selid = parentid == "" || parentid == undefined ? orgid : parentid;

            $.mDialog.close();
        });
    },
    loadData: function () {
        var op = $("#op").val();
        //如果不是新增，则加载需要编辑的部门信息
        if (op != "add") {
            $("body").mFormGet({ url: "/BD/Dept/GetDeptModel" });
        }
    }
}


$(document).ready(function () {
    DeptEdit.init();
});