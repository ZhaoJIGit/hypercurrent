/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var GoItemList = {
    IsListActionClick: false,
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    Sort: "asc",
    init: function () {
        GoItemList.bindGrid(true);
        GoItemList.bindAction();
    },
    bindGrid: function (isActive) {
        var goServer = $("#aGoServer").val();
        //var keyword = Megi.encode($("#itemKeyword").val());

        if (isActive == undefined) {
            isActive = true;
        }

        var selector = isActive ? '#tbItem' : '#tbArchiveItem';

        Megi.grid(selector, {
            pagination: true,
            url: "/BD/Item/GetPageItemList",
            sortName: 'MNumber',
            sortOrder: GoItemList.Sort,
            queryParams: { Keyword: Megi.encode($("#itemKeyword").val()), IsActive: isActive },
            columns: [[
                {
                    title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                    }, width: 40, align: 'center', sortable: false
                },
                { title: HtmlLang.Write(LangModule.BD, "ItemCode", "Item Code"), field: 'MNumber', width: 200, align: 'left', sortable: true },
                { title: HtmlLang.Write(LangModule.BD, "ItemName", "商品名称"), field: 'MDesc', width: 300, sortable: true },
                { title: HtmlLang.Write(LangModule.BD, "CostPrice", "Cost Price"), field: 'MPurPrice', width: 140, align: 'right', sortable: true, formatter: function (value, rec, rowIndex) { return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 8, 2); } },
                { title: HtmlLang.Write(LangModule.BD, "SalePrice", "Sale Price"), field: 'MSalPrice', width: 140, align: 'right', sortable: true, formatter: function (value, rec, rowIndex) { return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 8, 2); } },
                {
                    title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), field: 'Action', align: 'center', width: 120, sortable: false, formatter: function (value, rec, rowIndex) {
                        if (!isActive) {
                            return;
                        }
                        if (GoItemList.hasChangeAuth) {
                            return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"GoItemList.IsListActionClick = true;GoItemList.editItemInfo('" + rec.MItemID + "');" + "\" class='list-item-edit'></a><a href='javascript:void(0);' onclick=\"GoItemList.IsListActionClick = true;GoItemList.deleteItem('" + rec.MItemID + "');\" class='list-item-del'></a></div>";
                        } else {
                            return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"GoItemList.IsListActionClick = true;GoItemList.editItemInfo('" + rec.MItemID + "');" + "\" class='list-item-edit'></a></div>";
                        }
                    }
                }]],
            onClickRow: function (rowIndex, rowData) {
                if (GoItemList.IsListActionClick) {
                    $(this).datagrid('unselectRow', rowIndex);
                    GoItemList.IsListActionClick = false;
                }
            },
            onSortColumn: function (sort, order) {
                GoItemList.Sort = order;
            },
            onBeforeLoad: function () {

            }
        });

        var pageObject = $(selector).datagrid("getPager");

        if (pageObject) {
            $(pageObject).pagination({
                onBeforeRefresh: function (pageNumber, pageSize) {
                    var queryParams = $(selector).datagrid('options').queryParams;

                    queryParams = !queryParams ? {} : queryParams;

                    queryParams.Keyword = Megi.encode($("#itemKeyword").val());
                    queryParams.IsActive = isActive;

                }
            });
        }

    },

    deleteClick: function (isSeeAll) {
        var param = {};
        param.IsDelete = true;
        //先去检查是否可删除
        Megi.grid("#tbItem", "dbSelected", {
            url: "/BD/Item/IsCanDeleteOrInactive", param: param, callback: function (data) {
                var alertMsg = BDQuote.GetQuoteMsg(data, isSeeAll);
                if (data.Success) {
                    //可删除弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            //执行删除操作
                            Megi.grid("#tbItem", "dbSelected", {
                                url: "/BD/Item/DeleteItemList",
                                callback: function (retdata) {
                                    //if (retdata.Success) {
                                    $.mMsg(HtmlLang.Write(LangModule.BD, "DeleteItemSuccess", "Delete Inventory Item Successfully"));
                                    GoItemList.bindGrid(true);
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

    bindAction: function () {
        $("#NewItem").click(function () {
            GoItemList.editItemInfo("");
        });
        $("#DeleteItems").click(function () {
            GoItemList.deleteClick();
        });
        $("#itemSearch").click(function () {
            var index = $('#tt').tabs('getTabIndex', $('#tt').tabs('getSelected'));//获取选中tab的index
            GoItemList.bindGrid(index == 0);
        });
        $("#aImport").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/Item', HtmlLang.Write(LangModule.BD, "ImportInventoryItems", "Import Inventory Items"), 830, 500);
        });
        $("#divExportTypes .menu-item").click(function () {
            GoItemList.exportData(this);
        });
        $("#aExport").click(function () {
            GoItemList.exportData(this, "Xls");
        });
        $('#itemKeyword').keydown(function (e) {
            if (e.keyCode == 13) {
                GoItemList.bindGrid();
            }
        });

        $('#btnRestore').click(function () {
            GoItemList.archiveItem(true);
        });

        $("#btnArchive").click(function () {
            GoItemList.archiveItem(false);
        });

        $('#tt').tabs({
            onSelect: function (title, index) {
                if (index == 0) {
                    GoItemList.bindGrid(true);
                } else if (index == 1) {
                    GoItemList.bindGrid(false);
                }
            }
        });
    },
    exportData: function (sender, exportType) {
        if (exportType == undefined) {
            exportType = $(sender).text();
        }
        location.href = '/BD/Item/Export/' + exportType;
        $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
    },
    deleteItem: function (id) {
        var obj = {};
        obj.KeyIDs = id;
        obj.IsDelete = true;
        mAjax.submit("/BD/Item/IsCanDeleteOrInactive", { param: obj }, function (response) {
            var alertMsg = BDQuote.GetQuoteMsg(response);
            if (response.Success) {
                //可删除弹出提示框是否继续删除
                $.mDialog.confirm(alertMsg, {
                    callback: function () {
                        mAjax.submit("/BD/Item/DeleteItemList", { param: obj }, function (deleteReponse) {
                            if (deleteReponse.Success) {
                                $.mMsg(HtmlLang.Write(LangModule.BD, "DeleteItemSuccess", "Delete Inventory Item Successfully"));
                                GoItemList.bindGrid();
                            }
                        });
                    }
                });
            } else {
                $.mDialog.alert(alertMsg);
            }
        });
    },
    afterEdit: function (msg) {
        $.mMsg(msg);
        GoItemList.bindGrid();
    },
    editItemInfo: function (id) {
        Megi.dialog({
            title: id == "" ? HtmlLang.Write(LangModule.BD, "NewInventoryItem", "New Inventory Item") : HtmlLang.Write(LangModule.BD, "EditInventoryItem", "Edit Inventory Item"),
            width: 620,
            height: 440,
            href: '/BD/Item/ItemEdit?id=' + id,
            mCloseCallback: [GoItemList.closeDialog, GoItemList.reload]
        });
    },
    closeDialog: function () { $.mDialog.close(); },
    reload: function () {
        mWindow.reload("/BD/Item/ItemList");
        //window.location = "/BD/Item/ItemList";
    },
    archiveItem: function (isRestore, isSeeAll) {
        var selector = isRestore ? '#tbArchiveItem' : '#tbItem';
        if (isRestore) {
            var tips = HtmlLang.Write(LangModule.BD, "ConfirmRestoreItem", "您确定要启用这些项目吗？");
            Megi.grid(selector, "optSelected", {
                callback: function (ids) {
                    $.mDialog.confirm(tips,
                    {
                        callback: function () {
                            mAjax.submit(
                                "/BD/Item/ArchiveItem",
                                { KeyIDs: ids, isRestore: isRestore },
                                function (msg) {
                                    if (msg.Success == true) {
                                        var tip = isRestore ? HtmlLang.Write(LangModule.BD, "RestoreItemSuceess", "Restore Inventory Item Successfully") : HtmlLang.Write(LangModule.BD, "ArchiveItemSuceess", "Archive Inventory Item Successfully");
                                        $.mMsg(tip);
                                        GoItemList.bindGrid(!isRestore);
                                    } else {
                                        var tip = isRestore ? HtmlLang.Write(LangModule.BD, "RestoreItemFail", "Restore inventory item fail") : HtmlLang.Write(LangModule.BD, "ArchiveItemFail", "Archive Inventory Item fail");
                                        $.mDialog.alert(tip);
                                    }
                                });
                        }
                    });
                }
            });
        } else {
            //先去检查是否可禁用
            Megi.grid("#tbItem", "dbSelected", {
                url: "/BD/Item/IsCanDeleteOrInactive", callback: function (data) {
                    var alertMsg = BDQuote.GetQuoteMsg(data, isSeeAll);
                    if (data.Success) {
                        //可禁用弹出提示框是否继续禁用
                        $.mDialog.confirm(alertMsg, {
                            callback: function () {
                                //执行禁用操作
                                Megi.grid("#tbItem", "dbSelected", {
                                    url: "/BD/Item/ArchiveItem",
                                    callback: function (retdata) {
                                        if (retdata.Success) {
                                            $.mMsg(HtmlLang.Write(LangModule.BD, "ArchiveItemSuceess", "Archive Inventory Item Successfully"));
                                        }
                                        GoItemList.bindGrid(true);
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
                        //不可禁用弹出提示。
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
        }
    }

}

$(document).ready(function () {
    GoItemList.init();
});