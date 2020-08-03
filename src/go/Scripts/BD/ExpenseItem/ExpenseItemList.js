
var GoExpenseItemList = {

    IsListActionClick: false,
    //权限标志
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    init: function () {
        GoExpenseItemList.bindGrid(true);
        GoExpenseItemList.bindAction();
    },
    bindGrid: function (isActive) {
        var goServer = $("#aGoServer").val();
        var keyword = Megi.encode($("#itemKeyword").val());
        var selector = isActive ? $("#tbExpItem") : $("#tbArchiveItem");
        mAjax.post("/BD/ExpenseItem/GetExpenseItemList", { isActive: isActive }, function (msg) {
            var options = {
                valueField: "MItemID",
                textField: "MName",
                parentField: "MParentItemID",
                idField: 'id',
                treeField: "MName",
            };
            var data = GoExpenseItemList.initData(msg, options);
            //给哪个Table 绑定数据 
            selector.treegrid("loadData", data);
        },
            function () {

            }
        );

        var adjuge = isActive ? 40 : 180;

        var height = $("body").height() - $(".m-tab-toolbar").offset().top - adjuge;

        selector.treegrid({
            //计算高度 body的高度-显示框的高度 然后进行微调的高度
            height: height,
            resizable: true,
            auto: true,
            pagination: false,
            singleSelect: false,
            parentField: "MParentItemID",
            idField: 'id',
            //出现竖向的滚动条
            scrollY: true,
            treeField: "MName",

            columns: [[
                {
                    field: 'id', width: 40, hegiht: 22, checkbox: true,
                },
                {
                    title: HtmlLang.Write(LangModule.BD, "ExpesneLevel2Name", "item name"), field: 'MName', width: 230, align: 'left', sortable: false, formatter: function (value, rec, rowIndex) {
                        return mText.encode(value);
                    }
                },

                {
                    title: LangKey.Description, field: 'MDesc', width: 300, align: 'left', sortable: false, formatter: function (value, rec, rowIndex) {
                        return mText.encode(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), field: 'Action', align: 'center', width: 120, sortable: false, formatter: function (value, rec, rowIndex) {
                        if (isActive) {
                            if (GoExpenseItemList.hasChangeAuth) {
                                return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"GoExpenseItemList.IsListActionClick = true;GoExpenseItemList.editItemInfo('" + rec.id + "');" + "\" class='list-item-edit'></a><a href=\"javascript:void(0);\" onclick=\"GoExpenseItemList.IsListActionClick = true;GoExpenseItemList.deleteItem('" + rec.id + "');\" class='list-item-del'></a></div>";
                            } else {
                                return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"GoExpenseItemList.IsListActionClick = true;GoExpenseItemList.editItemInfo('" + rec.id + "');" + "\" class='list-item-edit'></a></div>";
                            }
                        }
                    }
                }
            ]],
            onClickCell: function () {
                return false;
            },
            onLoadSuccess: function () {
                //为所有的checkbox绑定点击事件
                $("input[type='checkbox']").bind("click", function () {
                    var dom = $(this);

                    var id = $(dom).attr("value");
                    setTimeout(function () {
                        $(selector).treegrid('cascadeCheck', {
                            id: id, //节点ID  
                            deepCascade: false //深度级联  
                        });
                    }, 100);

                });
            }
        });

    },
    requestItemData: function () {

    },
    deleteClick: function (isSeeAll) {
        var nodes = GoExpenseItemList.getDisplaySelectionsRows($("#tbExpItem"));
        if (!nodes || nodes.length == 0) {
            var tips = HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!");
            $.mDialog.alert(tips);
            return;
        }

        var ids = "";
        for (var j = 0 ; j < nodes.length; j++) {
            //获取子节点
            var node = nodes[j];
            ids += node.id + ",";
            var childrens = $("#tbExpItem").treegrid("getChildren", node.id);
            if (childrens) {
                //选中所有的子节点
                for (var i = 0; i < childrens.length; i++) {
                    var childrenNode = childrens[i];

                    $("#tbExpItem").treegrid("select", childrenNode.id);
                    ids += childrenNode.id + ",";
                }
            }
        }

        if (ids) {
            ids = ids.substring(0, ids.length - 1);
        }
        var obj = {};
        obj.KeyIDs = ids;
        obj.IsDelete = true;

        mAjax.submit("/BD/ExpenseItem/IsCanDeleteOrInactive", { param: obj }, function (response) {
            var alertMsg = BDQuote.GetQuoteMsg(response, isSeeAll);
            if (response.Success == true) {
                //可删除弹出提示框是否继续删除
                $.mDialog.confirm(alertMsg, {
                    callback: function () {

                        mAjax.submit("/BD/ExpenseItem/DeleteExpItem", { param: obj }, function (delResponse) {
                            //if (delResponse.Success == true) {
                            $.mDialog.message(HtmlLang.Write(LangModule.BD, "DeleteExpenseItemSuccess", "Delete Expense Item Successfully"));
                            GoExpenseItemList.bindGrid(true);
                            $("#tbExpItem").treegrid("unselectAll");
                            //}
                            //else {
                            //    $.mDialog.message(delResponse.Message);
                            //    GoExpenseItemList.bindGrid(true);
                            //}
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
                GoExpenseItemList.bindGrid(true);
            }

        });
    },
    bindAction: function () {
        $("#NewExpenseItem").click(function () {
            GoExpenseItemList.editItemInfo("");
        });
        $("#DeleteExpItems").click(function () {
            GoExpenseItemList.deleteClick();
        });

        $("#btnArchive").click(function () {
            GoExpenseItemList.archiveItem(false);
        });

        $("#btnRestore").click(function () {
            GoExpenseItemList.archiveItem(true);
        });

        $("#itemSearch").click(function () {
            GoExpenseItemList.bindGrid();
        });

        $("#aImport").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/ExpenseItem', HtmlLang.Write(LangModule.BD, "ImportExpenseItems", "Import Expense Items"), 830, 500);
        });

        $("#aExport").click(function () {
            location.href = $("#aGoServer").val() + '/BD/ExpenseItem/Export';
            $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });

        $('#tt').tabs({
            onSelect: function (title, index) {
                if (index == 0) {
                    GoExpenseItemList.bindGrid(true);
                } else if (index == 1) {
                    GoExpenseItemList.bindGrid(false);
                }
            }
        });
    },
    afterEdit: function (msg) {
        $.mDialog.message(HtmlLang.Write(LangModule.BD, "UpdateExpenseItemSuccess", "Update Expense Item Successfully"));
        GoExpenseItemList.bindGrid(true);
    },
    editItemInfo: function (id) {
        Megi.dialog({
            title: id == "" ? HtmlLang.Write(LangModule.BD, "NewExpenseItem", "New Expense Item") : HtmlLang.Write(LangModule.BD, "EditExpenseItem", "Edit Expense Item"),
            width: 400,
            height: 400,
            href: '/BD/ExpenseItem/ExpenseItemEdit/' + id
        });
    },
    reload: function () {
        mWindow.reload("/BD/ExpenseItem/ExpenseItemList");
    },
    deleteItem: function (id) {
        //获取子节点
        var childrens = $("#tbExpItem").megitreegrid("getChildren", id);
        if (childrens) {
            //选中所有的子节点
            for (var i = 0 ; i < childrens.length; i++) {
                var childrenNode = childrens[i];

                id += "," + childrenNode.id;
            }

        }

        var obj = {};
        obj.KeyIDs = id;
        obj.IsDelete = true;

        mAjax.submit("/BD/ExpenseItem/IsCanDeleteOrInactive", { param: obj }, function (response) {
            var alertMsg = BDQuote.GetQuoteMsg(response);
            if (response.Success == true) {
                //可删除弹出提示框是否继续删除
                $.mDialog.confirm(alertMsg, {
                    callback: function () {

                        mAjax.submit("/BD/ExpenseItem/DeleteExpItem", { param: obj }, function (delResponse) {
                            if (delResponse.Success == true) {
                                $.mDialog.message(HtmlLang.Write(LangModule.BD, "DeleteExpenseItemSuccess", "Delete Expense Item Successfully"));
                                GoExpenseItemList.bindGrid(true);
                            }
                            else {
                                $.mDialog.message(delResponse.Message);
                                GoExpenseItemList.bindGrid(true);
                            }
                        });
                    }
                });
            } else {
                //不可删除弹出提示。
                $.mDialog.alert(alertMsg);
                GoExpenseItemList.bindGrid(true);
            }
        });
    },
    initData: function (data, options) {
        //主键容器：用于循环了主键
        var keyContainer = new Array();
        var treeContainner = new Array();

        for (var i = 0; i < data.length; i++) {
            var row = data[i];
            if (!GoExpenseItemList.isParentNode(row, data, options)) {
                if (GoExpenseItemList.isExistParentItem(row, data, options)) {
                    continue;
                }
                else {
                    var treeNode = GoExpenseItemList.convertToJsonObject(row, null, options);
                    treeContainner.push(treeNode);
                    keyContainer.splice(0, 0, row[options.valueField]);

                }
            }
            if (GoExpenseItemList.valueIsExist(keyContainer, row[options.valueField])) {
                continue;
            }

            var childrenNode = GoExpenseItemList.getChildNodes(row[options.valueField], data, options);

            var treeNode = GoExpenseItemList.convertToJsonObject(row, childrenNode, options);

            treeContainner.push(treeNode);

            keyContainer.push(row[options.valueField]);

        }

        //将json字符串转换成json格式数据
        //json = $.toJSON(treeContainner);

        return treeContainner;
    },
    convertToJsonObject: function (row, chilrenRow, options) {
        var jsonObject = {};

        jsonObject.id = row[options.valueField];

        $.extend(jsonObject, row);

        jsonObject.children = null;

        if (chilrenRow && chilrenRow.length > 0) {
            jsonObject.children = new Array();
            for (var i = 0; i < chilrenRow.length; i++) {
                var childNode = chilrenRow[i];
                var childJsonObject = GoExpenseItemList.convertToJsonObject(childNode, null, options);

                jsonObject.children.push(childJsonObject);
            }
        }

        return jsonObject;
    },
    //判断数组是否在容器中
    valueIsExist: function (container, value) {
        for (var i = 0; i < container.length; i++) {
            if (container[i] == value) {
                return true;
            }
        }

        return false;
    },//查找该节点是否有父节点
    isParentNode: function (node, nodes, options) {
        for (var i = 0 ; i < nodes.length ; i++) {
            var temp = nodes[i];
            if (node[options.parentField] == 0 || !node[options.parentField] || node[options.valueField] == temp[options.parentField]) {
                return true;
            }
        }

        return false;
    },
    isExistParentItem: function (node, nodes, options) {
        var parentNodes = GoExpenseItemList.getParentNodes(nodes, options);

        if (!parentNodes || parentNodes.length == 0) {
            return false;
        }

        for (var i = 0; i < parentNodes.length; i++) {
            var parentNode = parentNodes[i];

            if (parentNode[options.valueField] == node[options.parentField]) {
                return true;
            }
        }
        return false;
    },
    getParentNodes: function (nodes, options) {
        var parentNodes = new Array();

        for (var i = 0 ; i < nodes.length ; i++) {
            var node = nodes[i];
            if (node[options.parentField] == 0 || !node[options.parentField]) {
                parentNodes.push(node);
            }
        }

        return parentNodes;
    },
    getChildNodes: function (parentId, nodes, options) {
        var childrenNode = new Array();
        for (var i = 0; i < nodes.length; i++) {
            var childNode = nodes[i];
            if (childNode[options.parentField] == parentId) {
                childrenNode.push(childNode);
            }
        }

        return childrenNode;
    },
    //获取可见且选中的Rows
    getDisplaySelectionsRows: function (selector) {
        //获取选中行
        var rows = selector.treegrid("getSelections");
        //返回的行
        var retRows = [];
        if (rows && rows.length > 0) {
            $(".datagrid-row").each(function (a, b) {
                //是否显示
                var display = $(b).is(":visible");
                if (display) {
                    //获取行ID
                    var nodeId = $(b).attr("node-id");
                    //查找选中并可见的行加到返回数据中
                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i].id == nodeId) {
                            retRows.push(rows[i]);
                        }
                    }
                }
            });
        }
        return retRows;
    },
    archiveItem: function (isRestore, isSeeAll) {

        var selector = !isRestore ? $("#tbExpItem") : $("#tbArchiveItem");

        var nodes = GoExpenseItemList.getDisplaySelectionsRows(selector);

        if (!nodes || nodes.length == 0) {
            var tips = HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!");
            $.mDialog.alert(tips);
            return;
        }

        var ids = "";
        for (var j = 0 ; j < nodes.length; j++) {
            //获取子节点
            var node = nodes[j];
            ids += node.id + ",";
            var childrens = selector.treegrid("getChildren", node.id);
            if (childrens) {
                //选中所有的子节点
                for (var i = 0 ; i < childrens.length; i++) {
                    var childrenNode = childrens[i];

                    selector.treegrid("select", childrenNode.id);

                    ids += childrenNode.id + ",";
                }
            }
            //恢复 如果下级恢复 上级也要恢复
            if (isRestore) {
                if (nodes[j].MParentItemID) {
                    ids += nodes[j].MParentItemID + ",";
                }
            }
        }

        if (ids) {
            ids = ids.substring(0, ids.length - 1);
        }

        var obj = {};
        obj.itemids = ids;
        obj.KeyIDs = ids;
        obj.isRestore = isRestore;

        if (isRestore) {
            var confirmTips = HtmlLang.Write(LangModule.BD, "ConfirmRestoreItem", "您确定要启用这些项目吗？");
            $.mDialog.confirm(confirmTips, {
                callback: function () {
                    mAjax.submit("/BD/ExpenseItem/AchiveExpenseItem", obj, function (response) {
                        if (response.Success == true) {
                            var tips = isRestore ? HtmlLang.Write(LangModule.BD, "RestoreExpenseItemSuccess", "Restore expense item successfully") : HtmlLang.Write(LangModule.BD, "ArchiveExpenseItemSuccess", "Archive expense item successfully");
                            $.mDialog.message(tips);
                            GoExpenseItemList.bindGrid(!isRestore);

                            selector.treegrid("unselectAll");
                        } else {
                            var tips = isRestore ? HtmlLang.Write(LangModule.BD, "RestoreExpenseItemFail", "Restore expense item fail") : HtmlLang.Write(LangModule.BD, "ArchiveExpenseItemFail", "Archive expense item fail");
                            $.mDialog.alert(tips);
                            //GoExpenseItemList.bindGrid();
                        }
                    });
                }
            });
        } else {
            //先去检查是否可禁用
            mAjax.submit("/BD/ExpenseItem/IsCanDeleteOrInactive", { param: obj }, function (response) {
                var alertMsg = BDQuote.GetQuoteMsg(response, isSeeAll);
                if (response.Success == true) {
                    //可禁用弹出提示框是否继续禁用
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            mAjax.submit("/BD/ExpenseItem/AchiveExpenseItem", obj, function (delResponse) {
                                if (delResponse.Success == true) {
                                    $.mDialog.message(HtmlLang.Write(LangModule.BD, "ArchiveExpenseItemSuccess", "Archive expense item successfully"));
                                    GoExpenseItemList.bindGrid(true);
                                }
                                else {
                                    $.mDialog.message(HtmlLang.Write(LangModule.BD, "ArchiveExpenseItemFail", "Archive expense item fail"));
                                    GoExpenseItemList.bindGrid(true);
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
                    GoExpenseItemList.bindGrid(true);
                }
            });
        }

    }
}
$(document).ready(function () {
    GoExpenseItemList.init();
});