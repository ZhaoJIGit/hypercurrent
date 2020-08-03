var UserList = {
    //用于标示是否需要选中某一行
    IsSelectRow: false,
    //是否有权限修改或者删除
    HasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    //时间格式化
    TimeFormatString: $("#hidTimeFormat").val(),
    //初始化页面数据
    Init: function () {
        UserList.InitUserGrid();
        UserList.InitActivityGrid();
        UserList.InitAction();


    },
    InitAction: function () {
        $('#btnArchive').linkbutton({
            onClick: function () {
                UserList.ArchiveUser();
            }
        });
        $('#btnRestore').linkbutton({
            onClick: function () {
                UserList.ArchiveUser();
            }
        });


        //切换tab的时候，重新调整和布局列表
        $(".easyui-tabs").tabs({
            onSelect: function (title, index) {
                switch (index) {
                    case 0:
                        UserList.InitUserGrid();
                        $('#tbUsers').datagrid('resize');
                        break;
                    case 1:
                        UserList.InitActivityGrid();
                        $("#tbRecentActivity").datagrid("resize");
                        break;
                    default:
                }
            }
        });
    },
    ArchiveUser: function () {
        var rows = Megi.grid('#tbUsers', "getSelections");
        if (!rows || rows.length == 0) {
            var error = HtmlLang.Write(LangModule.User, "NotSelectedAnyRow", "Please select a row");
            $.mDialog.alert(error);
            return;
        }
        var ids = "";
        for (var i = 0; i < rows.length; i++) {
            ids += rows[i].MItemID + ",";
        }
        if (ids) {
            ids = ids.substring(0, ids.length - 1);
        }

        mAjax.submit(
            "/BD/User/ArchiveUser",
            { userId: ids },
            function (msg) {
                if (msg && msg.Success) {
                    var tips = msg.Message;
                    $.mDialog.message(tips, null, true);
                    UserList.InitUserGrid();
                } else {
                    if (msg && msg.Message) {
                        var tips = "<div>" + msg.Message + "<div>";
                        $.mDialog.alert(tips, null, true);
                    } else {
                        var error = HtmlLang.Write(LangModule.User, "OperationFail", "Operation fail!");
                        $.mDialog.alert(error, null, true);
                    }
                }
            });
    },
    //初始化用户列表
    InitUserGrid: function () {
        Megi.grid('#tbUsers', {
            resizable: true,
            auto: true,
            showHeader: true,
            pagination: true,
            sortName: "MFullName",
            sortOrder: "asc",
            scrollY: true,
            height: $("body").height() - $(".tabs-panels").offset().top,
            url: "/BD/User/GetUserPermissionList",
            columns: [[
                {
                    //title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                    //    return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    //}, width: 30, align: 'center', sortable: false
                    title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    }, width: 20, align: 'center', sortable: false
                }
                ,
                //用户名(姓 和 名)
                {
                    title: HtmlLang.Write(LangModule.User, "Name", "Name"), field: 'MFullName', width: 80, sortable: true, formatter: function (value, row, index) {
                        return "<a href=\"javascript:void(0);\" onclick=\"UserList.IsSelectRow = true;$.mTab.addOrUpdate('" + HtmlLang.Write(LangModule.User, "EditUser", "Edit User") + "','/BD/User/UserEdit/" + row.MItemID + "')" + "\" >" + value + "</a>"
                    }
                },
                //电子邮箱
                { title: HtmlLang.Write(LangModule.User, "EmailAddress", "Email Address"), field: 'MEmail', width: 100, sortable: true },
                //岗位
                {
                    title: HtmlLang.Write(LangModule.User, "Position", "Position"), field: 'MPosition', width: 100, sortable: true, formatter: function (value, row, index) {
                        //如果有多个岗位，特殊处理
                        if (value.indexOf(',') != -1) {
                            var positionTest = "";
                            var values = value.split(',');
                            for (var i = 0; i < values.length; i++) {
                                if (i == values.length - 1) {
                                    positionTest += HtmlLang.Write(LangModule.User, values[i], values[i]);
                                } else {
                                    positionTest += HtmlLang.Write(LangModule.User, values[i], values[i]) + ",";
                                }
                            }
                            return positionTest;
                        } else {
                            return HtmlLang.Write(LangModule.User, value, value);
                        }
                    }
                },
                //角色
                {
                    title: HtmlLang.Write(LangModule.User, "Role", "Role"), field: 'MRole', width: 80, sortable: true, formatter: function (value, row, index) {
                        return HtmlLang.Write(LangModule.User, value, value);
                    }
                },
                //状态
                {
                    title: HtmlLang.Write(LangModule.User, "Status", "Status"), field: 'MPermStatus', width: 35, align: 'center', sortable: true, formatter: function (value, row, index) {
                        if (value == "Pending") {
                            return "<div style=\"color: #777;\" >" + HtmlLang.Write(LangModule.User, "Pending", "Pending") + "</div>";
                        } else {
                            //如果等于1，表示已归档
                            if (row.MIsArchive == 1) {
                                return "<div style=\"color: #777;\" >" + HtmlLang.Write(LangModule.User, "Archive", "Archive") + "</div>";
                            }
                            return "<div style=\"color: #57a400;\" >" + HtmlLang.Write(LangModule.User, "Active", "Active") + "</div>";
                        }
                    }
                },
                //编辑 和 删除
                {
                    title: HtmlLang.Write(LangModule.User, "Operation", "Operation"), field: 'Action', align: 'center', width: 40, sortable: false, formatter: function (value, row, index) {
                        if (UserList.HasChangeAuth) {
                            return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"UserList.IsSelectRow = true;UserList.editItem('" + row.MItemID + "');\" class='list-item-edit'></a><a href='javascript:void(0);' onclick=\"UserList.IsSelectRow = true;UserList.deleteItem('" + row.MItemID + "');\" class='list-item-del'></a></div>";
                        } else {
                            return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"UserList.IsSelectRow = true;UserList.editItem('" + row.MItemID + "');\" class='list-item-edit'></a></div>";
                        }
                    }
                }
            ]],
            //当点击 编辑 和 删除 的时候当前所属的行不要选中
            onClickRow: function (rowIndex, rowData) {
                //if (rowData.MIsArchive == 1) {
                //    $("#btnArchive").linkbutton("disable");
                //    $("#btnRestore").linkbutton("enable");
                //} else {
                //    $("#btnArchive").linkbutton("enable");
                //    $("#btnRestore").linkbutton("disable");
                //}

                //$("#tbUsers").datagrid('unselectAll');
                //$("#tbUsers").datagrid('selectRow', rowIndex);

            },
            onLoadSuccess: function () {
                $(window).resize();
            }
        });
    },
    //初始化用户登陆记录列表
    InitActivityGrid: function () {
        Megi.grid('#tbRecentActivity', {
            resizable: true,
            auto: true,
            showHeader: true,
            pagination: true,
            scrollY: true,
            pageSize: 20,
            pageList: [10, 20, 50, 100, 200],
            height: $("body").height() - $(".tabs-panels").offset().top,
            url: "/BD/User/GetUserActivityList",
            columns: [[

                //用户名(姓 和 名)
                { title: HtmlLang.Write(LangModule.User, "Name", "Name"), field: 'MFullName', width: 200, sortable: true },
                //最后登陆日期
                {
                    title: HtmlLang.Write(LangModule.User, "LastLoginDate", "Last Login Date"), field: 'MLoginDate', width: 300, align: 'center', formatter: function (value, row, index) {
                        return row.MLoginDate;
                    }
                }
            ]],
            onLoadSuccess: function () {
                $(window).resize();
            }
        });
    },
    //时间格式化
    TimeFormat: function (jsonDate, format) {
        try {
            var date = new Date(parseInt(jsonDate.replace("/Date(", "").replace(")/", ""), 10));
            var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
            var day = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
            var hours = date.getHours();
            var minutes = date.getMinutes();
            var seconds = date.getSeconds();
            var timePrefix = "";
            var isZeroize = false;
            if (format) {
                if (format.indexOf('tt ') > -1) {
                    //分早晚
                    if (hours > 0 && hours <= 12) {
                        //早上
                        timePrefix = " " + HtmlLang.Write(LangModule.User, "Morning", "Morning");
                    } else {
                        //下午
                        timePrefix = " " + HtmlLang.Write(LangModule.User, "Afternoon", "Afternoon");
                    }
                    if (format.indexOf('hh') > -1) {
                        isZeroize = true;
                    }
                } else {
                    if (format.indexOf('HH') > -1) {
                        isZeroize = true;
                    }
                }
            } else {
                isZeroize = true;
            }
            if (isZeroize) {
                //不足10的，前面补0
                hours = hours < 10 ? "0" + hours : hours;
                minutes = minutes < 10 ? "0" + minutes : minutes;
                seconds = seconds < 10 ? "0" + seconds : seconds;
                return timePrefix + " " + hours + ":" + minutes + ":" + seconds;
            } else {
                return timePrefix + " " + hours + ":" + minutes + ":" + seconds;
            }
        } catch (ex) {
            return "";
        }
    },
    //编辑用户
    editItem: function (itemId) {
        $.mTab.addOrUpdate(HtmlLang.Write(LangModule.User, "EditUser", "Edit User"), "/BD/User/UserEdit/" + itemId);
    },
    //删除用户
    deleteItem: function (itemId) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete, {
            callback: function () {
                var params = { MItemID: itemId };
                mAjax.submit("/BD/User/UserLinkInfoDelete/", { model: params }, function (msg) {
                    if (msg == true) {
                        UserList.InitUserGrid();
                    } else {
                        $.mDialog.alert(HtmlLang.Write(LangModule.User, "AtLeastOneSystemManager", "every organisation must have at least one system manager."));
                    }
                });
            }
        });
    },
    refresh: function () {
        var tabSelectIndex = UserList.getSelectTabIndex();
        tabSelectIndex == 0 ? UserList.InitUserGrid() : UserList.InitActivityGrid();
    },
    getSelectTabIndex: function () {
        var tab = $(".easyui-tabs").tabs('getSelected');
        var index = $(".easyui-tabs").tabs('getTabIndex', tab);
        return index;
    }
}
//初始化页面数据
$(document).ready(function () {
    UserList.Init();
    window.MReady = UserList.refresh;
});