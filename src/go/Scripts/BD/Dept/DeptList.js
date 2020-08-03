/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var DeptList = {
    HasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    init: function () {
        DeptList.deptTreeGrid();
        DeptList.deptButtonAction();
        DeptList.empButtonAction();
        DeptList.empDataGrid("", "");
    },
    deptTreeGrid: function () {
        $("#tbDeptTree").treegrid({
            url: "/BD/Dept/GetOrgDeptList?type=1",
            idField: 'id',
            treeField: 'name',
            columns: [[
                { field: 'name', title: HtmlLang.Write(LangModule.Contact, "OrgDept", "Organization Department"), width: 180 }
            ]],
            onBeforeExpand: function (row) {
                if (row) {
                    $(this).treegrid('options').url = "/BD/Dept/GetOrgDeptList?type=2&orgId=" + row._topId + "&parentId=" + DeptList.getParentid(row, row.id);
                }
                return true;
            },
            onClickRow: function (row) {
                var orgid = row._topId;
                var deptid = row.type != "2" ? "" : row.id;
                DeptList.empDataGrid(orgid, deptid);
                return true;
            }
        });
    },
    empDataGrid: function (orgid, deptid) {
        Megi.grid("#tbEmpGrid", {
            resizable: true,
            auto: true,
            pagination: true,
            sortName: 'MFullName',
            sortOrder: 'asc',
            url: "/BD/Dept/GetDeptEmpPageList",
            queryParams: { orgid: orgid, deptid: deptid },
            columns: [[
                {
                    field: 'IsSelect', title: '<input type=\"checkbox\" >', formatter: function (value, rec, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    }, width: 25, align: 'center', sortable: true
                },
                {
                    field: 'MFullName', title: HtmlLang.Write(LangModule.Contact, "EmpName", "Employee Name"), width: 100, sortable: true
                },
                { field: 'MEmail', title: HtmlLang.Write(LangModule.Contact, "EmpEmail", "Employee Email"), width: 100, sortable: true }
                ,
                {
                    title: HtmlLang.Write(LangModule.Contact, "Operation", "Operation"), field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                        var actionHtml = "<div class='list-item-action'>";
                        if (DeptList.HasChangeAuth) {
                            actionHtml += "<a href=\"javascript:void(0);\" onclick=\"$('#tbEmpGrid').datagrid('selectRow'," + rowIndex + ");$('#RemoveEmp').trigger('click');\" class='list-item-del'></a></div>";
                        } else {
                            actionHtml += "</div>";
                        }

                        return actionHtml;
                    }
                }
            ]]
        });
    },
    deptButtonAction: function () {
        $("#NewDept").click(function () {
            var node = $('#tbDeptTree').treegrid('getSelected'); //选中节点
            if (node == null) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Contact, "selectOrganisationItem", "please select an organization item"));
                return false;
            }
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Contact, "AddDepartment", "Add A Department"),
                width: 400,
                height: 300,
                href: '/BD/Dept/DeptEdit?orgid=' + node._topId + "&parentid=" + node.id + "&op=add"//DeptList.getParentid(node, node.id)
            });
        });
        $("#AlterDept").click(function () {
            var node = $('#tbDeptTree').treegrid('getSelected'); //选中节点
            if (node == null || node.type != "2") {
                $.mDialog.alert(HtmlLang.Write(LangModule.Contact, "selectDeparentItem", "please select a department item"));
                return false;
            }
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Contact, "EditDepartment", "Edit Department"),
                width: 400,
                height: 300,
                href: '/BD/Dept/DeptEdit?orgid=' + node._topId + "&parentid=" + DeptList.getParentid(node, node._parentId) + "&deptid=" + node.id + "&op=up"
            });
        });
        $("#RemoveDept").click(function () {
            var node = $('#tbDeptTree').treegrid('getSelected'); //选中节点
            if (node == null || node.type != "2") {
                $.mDialog.alert(HtmlLang.Write(LangModule.Contact, "selectDeparentItem", "please select a department item"));
                return false;
            }
            var parentid = DeptList.getParentid(node, node._parentId);
            var orgid = node._topId;
            var deptid = node.id;
            var selid = parentid == "" || parentid == undefined ? orgid : parentid;
            $.mDialog.confirm(HtmlLang.Write(LangModule.Contact, "SureRemoveThisDepartment", "Are you sure you want to remove this department?"), {
                callback: function () {
                    var obj = {};
                    obj.KeyIDs = deptid;
                    mAjax.submit(
                        "/BD/Dept/DeptRemoveFromOrg", 
                        obj, 
                        function (msg) {
                            if (msg) {
                                $.mMsg(HtmlLang.Write(LangModule.BD, "DeleteDeparentSuccess", "Delete Deparent Successfully"));
                                DeptList.reloadTree(selid);

                            } else {
                                $.mDialog.alert(msg.Message);
                            }
                        });
                }
            });
        });
    },
    empButtonAction: function () {
        $("#NewEmp").click(function () {
            var node = $('#tbDeptTree').treegrid('getSelected'); //选中节点
            if (node == null) {
                $.mAlert(HtmlLang.Write(LangModule.Contact, "selectAnItem", "please select an item"));
                return false;
            }
            var orgid = node._topId;
            var deptid = node.type != "2" ? "" : node.id;
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Contact, "AddEmployee", "Add Employee"),
                width: 400,
                height: 200,
                href: '/BD/Dept/EmployeeEdit?orgid=' + orgid + "&deptid=" + deptid
            });
        });
        $("#AlterEmp").click(function () {
            var row = $('#tbEmpGrid').datagrid('getSelected'); //选中节点
            if (row == null) {
                $.mAlert(HtmlLang.Write(LangModule.Contact, "selectEmployeeItem", "please select an employee item"));
                return false;
            }
            var node = $('#tbDeptTree').treegrid('getSelected'); //选中节点
            var orgid = node._topId;
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Contact, "AlterEmployee", "Alter Employee"),
                width: 400,
                height: 200,
                href: '/BD/Dept/EmployeeEdit?orgid=' + orgid + "&empid=" + row.MItemID
            });
        });
        $("#RemoveEmp").click(function () {
            Megi.grid("#tbEmpGrid", "optSelected", {
                callback: function (ids) {
                    $.mDialog.confirm(HtmlLang.Write(LangModule.Contact, "SureRemoveThisEmployees", "Are you sure you want to remove this employees?"), function () {
                        var obj = {};
                        obj.KeyIDs = ids;
                        //部门
                        var node = $('#tbDeptTree').treegrid('getSelected'); //选中节点
                        obj.departId = node.id;
                        mAjax.submit(
                            "/BD/Dept/EmployeesRemoveOut",
                            obj,
                            function (msg) {
                                if (msg.Success) {
                                    $.mMsg(HtmlLang.Write(LangModule.BD, "RemoveEmployeesSuccess", "Removeb Employees Successfully"));
                                    DeptList.reloadData();
                                } else {
                                    $.mDialog.alert(msg.Message);
                                }
                            });
                    });
                }
            });
        });
        $("#MoveToEmp").click(function () {
            var node = $('#tbDeptTree').treegrid('getSelected'); //选中节点
            if (!node) {
                $.mAlert(HtmlLang.Write(LangModule.BD, "UnSelectEmployee", "Please Select an employee"));
                return false;
            }
            var orgid = node._topId;
            Megi.grid("#tbEmpGrid", "optSelected", {
                callback: function (ids) {
                    Megi.dialog({
                        title: HtmlLang.Write(LangModule.Contact, "EmployeesAddToDept", "Employees add to department"),
                        width: 400,
                        height: 200,
                        href: '/BD/Dept/EmployeesMoveTo?orgid=' + orgid + "&empids=" + ids
                    });
                }
            });
        });
    },
    getParentid: function (node, pid) {
        var _pid = pid;
        if (pid == node._topId) { _pid = ""; }
        return DeptList.undefined(_pid);
    },

    undefined: function (parentid) {
        if (parentid == undefined) {
            return "";
        }
        return parentid;
    },
    reloadTree: function (id) {
        var raloadId = id;
        var parentNode = $("#tbDeptTree").treegrid('getParent', id);
        if (parentNode != null) { raloadId = parentNode.id; }
        $("#tbDeptTree").treegrid('reload', raloadId);
    },
    reloadData: function () {
        //DeptList.empDataGrid(orgid, deptid);
        $("#tbEmpGrid").datagrid('reload');
    }
}

$(document).ready(function () {
    DeptList.init();
});