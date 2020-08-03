var SalaryItemList = {
    hasChangeAuth: $("#hidChangeAuth").val() == "True" ? true : false,
    treegridId: "#tbSalaryItem",
    //不允许禁用的工资项（基本工资、税前工资、净工资、总工资）
    unAllowDisableItemTypes: [1000, 3000, 3005, 3015],
    //不允许删除的工资项目（社保、公积金项及个税）
    unSupportDelItemTypes: [1035, 1055, 2000, 2015, 3010],
    init: function () {

        SalaryItemList.bindGrid();
        SalaryItemList.bindAction();
    },
    bindGrid: function () {
        var goServer = $("#aGoServer").val();
        var keyword = Megi.encode($("#itemKeyword").val());

        $(SalaryItemList.treegridId).treegrid({
            url: '/PA/PayItem/GetSalaryItemTreeList',
            idField: "id",
            treeField: 'text',
            lines: true,
            checkbox: true,
            singleSelect: false,
            columns: SalaryItemList.Columns(),
            onClickCell: function () {
                return false;
            },
            onLoadSuccess: function () {
                $(SalaryItemList.treegridId).treegrid("collapseAll");
                $(SalaryItemList.treegridId).treegrid("unselectAll");

                var treePanel = $(SalaryItemList.treegridId).treegrid('getPanel');
                treePanel.find('div.datagrid-header input:checkbox').hide();
                var data = $(SalaryItemList.treegridId).treegrid("getData");
                $.each(data, function (i, item) {
                    if ($.inArray(item.ItemType, SalaryItemList.unAllowDisableItemTypes) != -1) {
                        treePanel.find("div.datagrid-body input:checkbox[value='" + item.id + "']").hide();
                    }
                });

                //为所有的checkbox绑定点击事件 
                //2018.6.5 去掉了 visible，并且把change改成click时间
                treePanel.find("div.datagrid-body input:checkbox").bind("click", function () {
                    var dom = $(this);
                    var id = $(dom).attr("value");
                    $(SalaryItemList.treegridId).treegrid("expand", id);

                    setTimeout(function () {
                        $(SalaryItemList.treegridId).treegrid('cascadeCheck', {
                            id: id, //节点ID  
                            deepCascade: false //深度级联  
                        });
                    }, 100);
                });
            },
            onClickRow: function (row) {
                SalaryItemList.switchBtnStatus(row, true);
            },
            onSelect: function (row) {
                SalaryItemList.switchBtnStatus(row);
            }
        });
    },
    switchBtnStatus: function (row, isFromClick) {
        var chk = $(SalaryItemList.treegridId).treegrid('getPanel').find("div.datagrid-body input:checkbox[value='" + row.id + "']");
        if (chk.is(":hidden")) {
            $(SalaryItemList.treegridId).treegrid("unselect", row.id);
            return;
        }

        //触发级联
        if (isFromClick) {
            chk.trigger("click");
        }

        var rows = $(SalaryItemList.treegridId).treegrid("getSelections");
        var statusList = SalaryItemList.getSelectionsStatus(rows);

        if (statusList.length == 1) {
            if (statusList[0]) {
                $("#EnableExpItems").linkbutton("disable");
                $("#DisableItems").linkbutton("enable");
            } else {
                $("#EnableExpItems").linkbutton("enable");
                $("#DisableItems").linkbutton("disable");
            }
        }
        else {
            $("#EnableExpItems").linkbutton("enable");
            $("#DisableItems").linkbutton("enable");
        }
    },
    getSelectionsStatus: function (rows) {
        var statusList = [];
        $.each(rows, function (i, item) {
            if ($.inArray(item.IsActive, statusList) == -1) {
                statusList.push(item.IsActive);
            }
        });

        return statusList;
    },
    //列集合
    Columns: function () {
        var arr = new Array();
        arr.push({
            field: 'id', width: 40, hegiht: 22, checkbox: true,
        });
        arr.push({
            title: HtmlLang.Write(LangModule.BD, "SalaryItemName", "Item Name"), field: 'text', width: 260, align: 'left', sortable: false, formatter: function (value, rec, rowIndex) {
                return mText.encode(value);
            }
        });
        arr.push({ title: HtmlLang.Write(LangModule.BD, "SalaryItemGroupID", "MGroupID"), field: 'MGroupID', hidden: true });
        arr.push({
            title: HtmlLang.Write(LangModule.BD, "Status", "Status"), width: 100, field: 'IsActive', align: 'center', hidden: false, formatter: function (value, rec, rowIndex) {
                if (value == "1") {
                    return HtmlLang.Write(LangModule.BD, "Enabled", "Enabled");

                } else {
                    return HtmlLang.Write(LangModule.BD, "Disable", "Disable");
                }
            }
        });
        if (SalaryItemList.hasChangeAuth) {
            arr.push({
                title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), field: 'IsActive1', align: 'center', width: 120, sortable: false, formatter: function (value, rec, rowIndex) {
                    if (rec.IsActive) {
                        return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"SalaryItemList.IsListActionClick = true;SalaryItemList.editItemInfo('" + rec.id + "');" + "\" class=\"list-item-edit\"></a></div>";
                    }
                }
            });
        }
        var columns = new Array();
        columns.push(arr);
        return columns;
    },
    bindAction: function () {
        $("#btnNew").off("click").on("click", function () {
            SalaryItemList.editItemInfo("");
        });
        $("#EnableExpItems").off("click").on("click", function () {
            SalaryItemList.disableItem(true);
        });
        $("#DisableItems").off("click").on("click", function () {
            SalaryItemList.disableItem(false);
        });
        $("#DeleteItems").off("click").on("click", function () {
            SalaryItemList.deleteItem();
        });
    },
    disableItem: function (isEnable, isSeeAll) {
        var rows = $(SalaryItemList.treegridId).treegrid("getSelections");
        if (rows.length <= 0) {
            var msg = HtmlLang.Write(LangModule.BD, "NoSelectRows", "Not choose any line!")
            $.mDialog.alert(msg);
            return;
        }

        var statusList = SalaryItemList.getSelectionsStatus(rows);
        if (statusList.length == 2) {
            var msg = HtmlLang.Write(LangModule.BD, "ForbiddenPayItemStatusError", "同时存在禁用和启用的工资项目，不能进行此操作")
            $.mDialog.alert(msg);
            return;
        }

        var ids = [];
        for (var i = 0; i < rows.length; i++) {
            //如果是禁用操作，只需要添加父级工资项目id，二级工资项目在后台代码会自动禁用；如果是恢复操作，需要添加所有选中的工资项目id
            if ((!isEnable && $.inArray(rows[i]._parentId, ids) == -1) || (isEnable && $.inArray(rows[i].id, ids) == -1)) {
                ids.push(rows[i].id);
            }
            
            //启用下级工资项目时，上级也要启用
            if (isEnable) {
                //如果有父级
                if (rows[i]._parentId) {
                    //父级对象
                    var parentNode = $(SalaryItemList.treegridId).treegrid("getParent", rows[i].id);
                    //如果父级是禁用的，并且父级id不在列表，则把父级id加到列表
                    if (!parentNode.MIsActive && $.inArray(rows[i]._parentId, ids) == -1) {
                        ids.push(rows[i]._parentId);
                    }
                }
            }
        }
        var id = ids.join(',');
        if (!isEnable) {
            var url = '/PA/PayItem/IsCanDeleteOrInactive';
            var param = {};
            param.KeyIDs = id;

            $("body").mFormSubmit({
                param: param,
                url: url, callback: function (msg) {
                    var alertMsg = BDQuote.GetQuoteMsg(msg, isSeeAll);
                    if (msg) {
                        $.mDialog.confirm(alertMsg, {
                            callback: function () {
                                SalaryItemList.forbiddenItem(id, isEnable);
                            }
                        });
                        if (isSeeAll) {
                            $("#popup_message").css("max-height", "300px");
                        } else {
                            $("#popup_message").css("max-height", "200px");
                        }
                    }
                }
            });

        } else {
            SalaryItemList.forbiddenItem(id, isEnable);
        }

    },
    //删除校验
    validateDelete: function (selRows) {
        //是否有勾选
        if (!selRows || selRows.length <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
            return false;
        }

        var disabledNames = [];
        var unsupportDelNames = [];
        $.each(selRows, function (i, item) {
            //所选项是否包含禁用项
            if (!item.IsActive) {
                disabledNames.push(item.text);
            }
            //所选项是否包含社保和公积金项
            if ($.inArray(item.ItemType, SalaryItemList.unSupportDelItemTypes) != -1 || item._parentId) {
                unsupportDelNames.push(item.text);
            }
        });
        if (disabledNames.length > 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "DelDisabledPayItemMsg", "所选的工资项目中包含禁用的工资项目，请先恢复后再进行操作。"));
            return false;
        }
        if (unsupportDelNames.length > 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "DelSSOrHFPayItemMsg", "所选的工资项目中包含社保或公积金，不能进行删除。"));
            return false;
        }

        return true;
    },
    deleteItem: function (isSeeAll) {
        //没传id时判断是否有选中
        var selRows = $(SalaryItemList.treegridId).treegrid("getSelections");

        //删除校验
        if (!SalaryItemList.validateDelete(selRows)) {
            return false;
        }

        //设置要删除的id
        var arrId = [];
        $.each(selRows, function (i, item) {
            arrId.push(item.id);
        });

        var param = {};
        param.IsDelete = true;
        param.KeyIDs = arrId.toString();

        mAjax.submit("/PA/PayItem/IsCanDeleteOrInactive", { param: param }, function (response) {
            var alertMsg = BDQuote.GetQuoteMsg(response, isSeeAll);
            if (response.Success == true) {
                //可禁用弹出提示框是否继续禁用
                $.mDialog.confirm(alertMsg, {
                    callback: function () {
                        mAjax.submit("/PA/PayItem/Delete", { param: param }, function (delResponse) {
                            if (delResponse.Success == true) {
                                SalaryItemList.reload();
                                $.mMsg(HtmlLang.Write(LangModule.BD, "DeletePayItemSuccess", "工资项目删除成功。"));
                            }
                            else {
                                $.mDialog.alert("<div>" + delResponse.Message.replace(/\n|\r\n/g, "<br>") + "</div>", undefined, 0, true, true);
                            }
                        });
                    }
                });
                if (isSeeAll) {
                    $("#popup_message").css("max-height", "300px");
                } else {
                    $("#popup_message").css("max-height", "200px");
                }
            } else {
                //不可禁用弹出提示。
                $.mDialog.alert(alertMsg);
                if (isSeeAll) {
                    $("#popup_message").css("max-height", "300px");
                } else {
                    $("#popup_message").css("max-height", "200px");
                }
                SalaryItemList.reload();
            }
        });
    },
    editItemInfo: function (id) {
        //默认为一级工资项
        var type = "0";
        var parentNode = $(SalaryItemList.treegridId).treegrid("getParent", id);
        if (parentNode && parentNode.children && parentNode.children.length > 0) {
            type = "1";
        }

        Megi.dialog({
            title: id == "" ? HtmlLang.Write(LangModule.BD, "NewSalaryItem", "New Salary Item") : HtmlLang.Write(LangModule.BD, "EidtSalaryItem", "Edit Salary Item"),
            width: 420,
            height: 365,
            href: '/PA/PayItem/PayItemEdit/?id=' + id + "&type=" + type
        });
    },
    //禁用
    forbiddenItem: function (id, isEnable) {
        var url = '/PA/PayItem/ForbiddenItem?ids=' + id;
        $("body").mFormSubmit({
            param: { ids: id },
            url: url, callback: function (msg) {
                if (msg && msg.Success) {
                    var msg = isEnable ? HtmlLang.Write(LangModule.BD, "EnableSalaryItemSuccess", "Enable Salary item success!") : HtmlLang.Write(LangModule.BD, "DisableSalaryItemSuccess", "Disable Salary item success!");
                    $.mDialog.message(msg);
                    SalaryItemList.reload();
                } else {
                    var failMsg = isEnable ? HtmlLang.Write(LangModule.BD, "EnableSalaryItemFail", "Enable Salary item Fail!") : HtmlLang.Write(LangModule.BD, "DisableSalaryItemFail", "Disable Salary item Fail!");
                    $.mDialog.alert(failMsg);
                }
            }
        });
    },
    reload: function () {
        SalaryItemList.bindGrid();
        //重新加载后反选所有项，防止getSelections获取到已删除的项目
        $(SalaryItemList.treegridId).treegrid("unselectAll");
    }
}

$(document).ready(function () {
    SalaryItemList.init();
});