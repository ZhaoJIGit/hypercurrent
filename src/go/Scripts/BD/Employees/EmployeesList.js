var GoEmployeesList = {
    IsListActionClick: false,
    HasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    typeId: "0",
    searchFilter: "0",
    init: function () {
        GoEmployeesList.tabKeySelected();
        GoEmployeesList.AllSelect();
        GoEmployeesList.tabSelect();
        GoEmployeesList.clickAction();
    },

    tabKeySelected: function () {
        var index = GoEmployeesList.getSelectTabIndex();
        var tabkey = $("#tabKeySel").val();
        var gridId = "#tbGrid" + tabkey;
        $('#tbEmployeesGroups').tabs('select', tabkey);
        typeKey = tabkey + "typeId";
        GoEmployeesList.typeId = $("#" + tabkey).find('input[id="' + typeKey + '"]').val();
        if (index == 0) {
            GoEmployeesList.gridFill(gridId, {}, "1");
        } else {
            GoEmployeesList.gridFill(gridId, {}, "0");
        }
    },
    getTabTitle: function () {
        return $('#tbEmployeesGroups').tabs('getSelected').panel('options').title;
    },
    AllSelect: function () {
        var index = GoEmployeesList.getSelectTabIndex();
        var gridId = "#tbGrid" + GoEmployeesList.getTabTitle();
        if (index == 0) {
            GoEmployeesList.gridFill(gridId, {}, "1");
        } else {
            GoEmployeesList.gridFill(gridId, {}, "0");
        }
    },

    tabSelect: function () {
        $('#tbEmployeesGroups').tabs({
            onSelect: function (title, index) {
                var gridId = index == 0 ? "#tbGrid" + title : "#girdArchive";

                var searchFilter = {};
                var isActive = "1";
                //如果是归档页签
                if (index == 1) {
                    isActive = "0";
                }
                GoEmployeesList.gridFill(gridId, searchFilter, isActive);
            }
        });
    },

    gridFill: function (gridId, searchFilter, isActive) {
        if (searchFilter == undefined) searchFilter = "0";
        if (isActive == undefined) {
            isActive = "1";
        }

        var toolbar = $(".m-tab-toolbar:visible");
        var t = toolbar.length > 0 ? toolbar.offset().top : 0;
        var h = toolbar.length > 0 ? toolbar.height() : 0;

        Megi.grid(gridId, {
            resizable: true,
            auto: true,
            pagination: true,
            sortName: 'MName',
            sortOrder: 'asc',
            scrollY: true,
            height: $("body").height() - t - h - 20,
            url: "/BD/Employees/GetEmployeesList",
            queryParams: { searchFilter: searchFilter, IsActive: isActive },
            columns: [[
                    {
                        title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                            return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                        }, width: 25, align: 'center', sortable: false, hidden: !GoEmployeesList.HasChangeAuth
                    },
                    { title: HtmlLang.Write(LangModule.Contact, "EmployeeName", "Employee Name"), field: 'MName', width: 100, align: 'center', sortable: true },
                    { title: HtmlLang.Write(LangModule.Contact, "AccountNo", "Account No."), field: 'MBankAcctNo', width: 90, sortable: true },
                    { title: HtmlLang.Write(LangModule.Contact, "EmailAddress", "Email Address"), field: 'MEmail', width: 110, sortable: true },
                    { title: HtmlLang.Write(LangModule.Contact, "PhoneNumbers", "Phone Numbers"), field: 'MPhone', width: 100, sortable: true },
                    {
                        title: HtmlLang.Write(LangModule.Contact, "Sex", "Sex"), field: 'MSex', width: 90, sortable: true, align: 'center', formatter: function (value, rec, rowIndex) {
                            if (value && value != 0) {
                                if (value == "Woman") {
                                    value = "Female";
                                }
                                else if (value == "Man") {
                                    value = "Male";
                                }
                                value = HtmlLang.Write(LangModule.Contact, value, value);
                            }
                            return value == 0 ? "" : value;
                        }
                    },
                    {
                        title: HtmlLang.Write(LangModule.Contact, "Status", "Status"), field: 'MStatus', width: 90, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                            return value == 0 ? "" : HtmlLang.Write(LangModule.Contact, value, value);
                        }
                    },
                    {
                        title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), hidden: !GoEmployeesList.HasChangeAuth, field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {


                            if (isActive == 1) {
                                var actionHtml = "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"GoEmployeesList.IsListActionClick = true;GoEmployeesList.editEmployeeInfo('" + rec.MItemID + "');\" class='list-item-edit'></a>";
                                if (GoEmployeesList.HasChangeAuth) {
                                    actionHtml += "<a href=\"javascript:void(0);\" onclick=\"GoEmployeesList.IsListActionClick = true;GoEmployeesList.deleteEmployee(false," + rowIndex + ");\" class='list-item-del'></a></div>";
                                } else {
                                    actionHtml += "</div>";
                                }
                            }
                            return actionHtml;
                        }
                    }
            ]],
            onClickRow: function (rowIndex, rowData) {
                if (GoEmployeesList.IsListActionClick) {
                    $(this).datagrid('unselectRow', rowIndex);
                    GoEmployeesList.IsListActionClick = false;
                }
            }
        });
    },
    editEmployeeInfo: function (id) {
        $.mDialog.show({
            mTitle: id == "" ? HtmlLang.Write(LangModule.Contact, "AddEmployee", "Add Employee") : HtmlLang.Write(LangModule.Contact, "EditEmployee", "Edit Employee"),
            mContent: "iframe:" + '/BD/Employees/EmployeesEdit?id=' + id,
            mWidth: 1100,
            mHeight: 450,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle"

        });
    },
    clickAction: function () {
        $("#NewEmployee").click(function () {
            GoEmployeesList.editEmployeeInfo('');
        });

        $("#AllArchive").click(function () {
            GoEmployeesList.archiveEmployee();
        });

        $('a[id*="Search"]').click(function () {
            var gridId = "#tbGrid" + GoEmployeesList.getTabTitle();
            var searchId = $(this).attr("id");
            var searchFilter = searchId.substring((searchId.indexOf("Search") + 6));
            GoEmployeesList.searchFilter = searchFilter;
            GoEmployeesList.gridFill(gridId, searchFilter);
        });

        $("#btnDelete").click(function () {
            GoEmployeesList.deleteEmployee();
        });

        //导入
        $("#aImport").click(function () {
            ImportBase.showImportBox("/BD/Import/Import/Employees", HtmlLang.Write(LangModule.Contact, "ImportEmployees", "Import Employees"), 830, 525);
        });
        $("#aExport").click(function () {
            var queryParam = { typeId: GoEmployeesList.typeId, searchFilter: GoEmployeesList.searchFilter };
            location.href = '/BD/Employees/Export?jsonParam=' + escape($.toJSON(queryParam));
            $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });

        $("#btnUnArchive").click(function () {
            GoEmployeesList.unArcheiveEmployee();
        });
    },
    //禁用按钮
    archiveEmployee: function (isSeeAll) {
        //先去检查是否可禁用
        Megi.grid("#tbGrid" + GoEmployeesList.getTabTitle(), "dbSelected", {
            url: "/BD/Employees/IsCanDeleteOrInactive", callback: function (data) {
                var alertMsg = BDQuote.GetQuoteMsg(data, isSeeAll);
                if (data.Success) {
                    //可禁用弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            //执行禁用操作
                            Megi.grid("#tbGrid" + GoEmployeesList.getTabTitle(), "dbSelected", {
                                url: "/BD/Employees/EmployeeMoveToArchived",
                                callback: function (retdata) {
                                    if (retdata.Success) {
                                        $.mMsg(HtmlLang.Write(LangModule.BD, "EmployeeArchivedSuccess", "员工禁用成功！"));
                                        GoEmployeesList.reload();
                                    }
                                }
                            });
                        }
                    });
                    if (isSeeAll) {
                        $("#popup_message").css("max-height", "300px");
                    }
                    else {
                        $("#popup_message").css("max-height", "200px");
                    }
                } else {
                    //不可删除弹出提示。
                    $.mDialog.alert(alertMsg);
                    if (isSeeAll) {
                        $("#popup_message").css("max-height", "300px");
                    }
                    else {
                        $("#popup_message").css("max-height", "200px");
                    }
                }
            }
        });
    },
    //恢复按钮
    unArcheiveEmployee: function () {
        Megi.grid("#girdArchive", "optSelected", {
            url: "/BD/Employees/RestoreEmployee", msg: HtmlLang.Write(LangModule.Contact, "SureToRestoreEmp", "确定要恢复选中的员工吗?"), callback: function () {
                GoEmployeesList.reload();
            }
        });
    },
    reload: function (tab) {
        var index = GoEmployeesList.getSelectTabIndex();

        var gridId = index == 0 ? "#tbGrid" + GoEmployeesList.getTabTitle() : "#girdArchive";

        var searchFilter = {};
        var isActive = "1";
        //如果是归档页签
        if (index == 1) {
            isActive = "0";
        }
        GoEmployeesList.gridFill(gridId, searchFilter, isActive);
    },
    deleteEmployee: function (isSeeAll, rowIndex) {
        //先选中该行,判断时注意0行这个特殊值
        if (rowIndex != undefined) {
            Megi.grid("#tbGrid" + GoEmployeesList.getTabTitle(), "unselectAll");
            Megi.grid("#tbGrid" + GoEmployeesList.getTabTitle(), "selectRow", rowIndex);
        }

        //Megi.grid("#tbGrid" + GoEmployeesList.getTabTitle(), "deleteSelected", {
        //    url: "/BD/Employees/DeleteEmployess",
        //    callback: function (response) {
        //        if (response.Success) {
        //            var tips =  HtmlLang.Write(LangModule.BD, "DeleteEmployeSuccess", "Delete Employe Successfully");
        //            $.mDialog.message(tips);
        //            GoEmployeesList.reload();
        //        } else {
        //            $.mDialog.alert(response.Message);
        //        }
        //    }
        //});

        var param = {};
        param.IsDelete = true;
        //先去检查是否可删除
        Megi.grid("#tbGrid" + GoEmployeesList.getTabTitle(), "dbSelected", {
            url: "/BD/Employees/IsCanDeleteOrInactive", param: param, callback: function (data) {
                var alertMsg = BDQuote.GetQuoteMsg(data, isSeeAll);
                if (data.Success) {
                    //可删除弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            //执行删除操作
                            Megi.grid("#tbGrid" + GoEmployeesList.getTabTitle(), "dbSelected", {
                                url: "/BD/Employees/DeleteEmployess",
                                param: param,
                                callback: function (retdata) {
                                    //if (retdata.Success) {
                                    $.mMsg(HtmlLang.Write(LangModule.BD, "DeleteEmployeSuccess", "Delete Employe Successfully"));
                                    GoEmployeesList.reload();
                                    //}
                                }
                            });
                        }
                    });
                    if (isSeeAll) {
                        $("#popup_message").css("max-height", "300px");
                    }
                    else {
                        $("#popup_message").css("max-height", "200px");
                    }
                } else {
                    //不可删除弹出提示。
                    $.mDialog.alert(alertMsg);
                    if (isSeeAll) {
                        $("#popup_message").css("max-height", "300px");
                    }
                    else {
                        $("#popup_message").css("max-height", "200px");
                    }
                }
            }
        });

    },
    getSelectTabIndex: function () {
        var tab = $('#tbEmployeesGroups').tabs('getSelected');
        var index = $('#tbEmployeesGroups').tabs('getTabIndex', tab);

        return index;
    }
}


$(document).ready(function () {
    GoEmployeesList.init();
});