var GoContactsList = {
    IsListActionClick: false,
    //是否有权限进行编辑和删除 test test
    HasChangeAuth: false,
    typeId: "0",
    searchFilter: "0",
    ViewSaleAuth: $("#hidViewSaleAuth").val(),
    ViewPurchaseAuth: $("#hasViewPurchaseAuth").val(),
    init: function () {
        GoContactsList.HasChangeAuth = $("#hidChangeAuth").val() == "1" ? true : false;
        $(".contact-tab-div").show();
        GoContactsList.tabKeySelected();
        GoContactsList.tabSelect();
        GoContactsList.clickAction();
    },
    tabKeySelected: function () {
        var tabkey = unescape($("#tabKeySel").val());
        if (!isNaN(tabkey)) {
            tabkey = +tabkey;
        }
        $('#tbContactGroups').tabs('select', tabkey);
        if (tabkey != "0") {
            var selectedTabIndex = GoContactsList.getSelectTabIndex();
            var gridId = "#tbGrid" + selectedTabIndex;
            typeKey = "#typeId" + selectedTabIndex;
            GoContactsList.typeId = $(typeKey).val();
            var showInvoice = $("input[id*='ShowInvoice']:visible").is(":checked");
            GoContactsList.gridFill(gridId, GoContactsList.typeId, "0", null, showInvoice);
        } else {
            var gridId = "#tbGrid" + 0;
            GoContactsList.gridFill(gridId, "0", "0");
        }
    },
    tabSelect: function () {
        $('#tbContactGroups').tabs({
            onSelect: function (title, index) {
                var htmlTabId = "#tabId" + index;
                var htmlGridId = "#tbGrid" + index;
                var htmlTypeId = "#typeId" + index;
                var typeId = $(htmlTypeId).val();
                GoContactsList.typeId = typeId;
                var showInvoice = $("input[id*='ShowInvoice']:visible").is(":checked");
                GoContactsList.gridFill(htmlGridId, typeId, "0", null, showInvoice);

                $(window).resize();
            }
        });
    },

    gridFill: function (gridId, typeId, searchFilter, keyword, showInvoice) {
        if (typeId == undefined) typeId = "0";
        if (searchFilter == undefined) searchFilter = "0";
        //隐藏的列
        var isVersion = $("#hidVersion").val();

        var toolbar = $(".m-tab-toolbar:visible");
        var t = toolbar.length > 0 ? toolbar.offset().top : 0;
        var h = toolbar.length > 0 ? toolbar.height() : 0;

        Megi.grid(gridId, {
            resizable: true,
            auto: true,
            pagination: true,
            sortName: 'MName',
            sortOrder: 'asc',
            height: $("body").height() - t - h - 20,
            scrollY: true,
            url: "/BD/Contacts/GetPageContactsList",
            queryParams: { typeId: typeId, searchFilter: searchFilter, keyword: keyword, showInvoice: showInvoice },
            columns: [[
                    {
                        title: '<input type=\"checkbox\" >', field: 'IsSelect', formatter: function (value, rec, rowIndex) {
                            return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                        }, width: 25, align: 'center', sortable: false, hidden: !GoContactsList.HasChangeAuth
                    },
                    {
                        title: HtmlLang.Write(LangModule.Common, "Name", "Name"), field: 'MName', width: 100, sortable: true, formatter: function (value, row, index) {
                            return "<a href=\"javascript:void(0);\" key='" + row.MItemID + "' class='a-viewcontact'></a>";
                        }
                    },
                    //{ title: HtmlLang.Write(LangModule.Contact, "ContactName", "Contact Name"), field: 'MName', width: 100, sortable: false },
                    {
                        title: HtmlLang.Write(LangModule.Contact, "PrimaryPerson", "Primary Person"), field: 'MContactName', width: 90, sortable: false, formatter: function (value, row, index) {
                            return "<span class='span-primaryperson' key='" + row.MItemID + "'><span>";
                        }
                    },
                    { title: HtmlLang.Write(LangModule.Contact, "EmailAddress", "Email Address"), field: 'MEmail', width: 110, sortable: false },
                    { title: HtmlLang.Write(LangModule.Contact, "PhoneNumbers", "Phone Numbers"), field: 'MPhone', width: 100, sortable: false, hidden: (GoContactsList.ViewPurchaseAuth == "1" ? false : true) },
                    {
                        //1：未付款采购单
                        title: HtmlLang.Write(LangModule.Contact, "BillsDue", "Bills Due"), field: 'MBillDueAmt', align: 'right', width: 90, sortable: false, hidden: (isVersion == "False" ? false : true) || !showInvoice, formatter: function (value, rec, rowIndex) {
                            return value == 0 ? "" : value;
                        }
                    },
                    {
                        //2:逾期采购单
                        title: HtmlLang.Write(LangModule.Contact, "OverdueBills", "Overdue Bills"), field: 'MBillOverDueAmt', align: 'right', width: 90, sortable: false, hidden: (GoContactsList.ViewPurchaseAuth == "1" && isVersion == "False" ? false : true) || !showInvoice, formatter: function (value, rec, rowIndex) {
                            return value == 0 ? "" : "<span class=\"red\">" + value + "</span>";
                        }
                    },
                    {
                        //3:到期销售单
                        title: HtmlLang.Write(LangModule.Contact, "SalesInvoicesDue", "Sales Invoices Due"), field: 'MSaleDueAmt', align: 'right', width: 110, sortable: false, hidden: (GoContactsList.ViewSaleAuth == "1" && isVersion == "False" ? false : true) || !showInvoice, formatter: function (value, rec, rowIndex) {
                            return value == 0 ? "" : value;
                        }
                    },
                    {
                        //4:逾期销售单
                        title: HtmlLang.Write(LangModule.Contact, "OverdueSalesInvoices", "Overdue Sales Invoices"), field: 'MSaleOverDueAmt', align: 'right', width: 110, sortable: false, hidden: (GoContactsList.ViewSaleAuth == "1" && isVersion == "False" ? false : true) || !showInvoice, formatter: function (value, rec, rowIndex) {
                            return value == 0 ? "" : "<span class=\"red\">" + value + "</span>";
                        }
                    },
                    {
                        title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), field: 'Action', align: 'center', hidden: !GoContactsList.HasChangeAuth, width: 40, sortable: false, formatter: function (value, rec, rowIndex) {


                            if (GoContactsList.HasChangeAuth && typeId != 3) {
                                var contactName = mText.encode(rec.MName);
                                var html = "<div class='list-item-action'><a href=\"javascript:void(0);\" key='" + rec.MItemID + "' class='list-item-edit'></a>" +
                                    "<a href=\"javascript:void(0);\" key='" + rec.MItemID + "' class='list-item-del'></a></div>";
                                return html;
                            } else {
                                return "";
                            }
                        }
                    }
            ]],
            onClickRow: function (rowIndex, rowData) {
                if (GoContactsList.IsListActionClick) {
                    $(gridId).datagrid('unselectRow', rowIndex);
                    GoContactsList.IsListActionClick = false;
                }
            },
            onLoadSuccess: function () {
                //给删除按钮绑定事件
                $(".list-item-del").each(function () {
                    var paramFields = ["MItemID", "MName"];
                    Megi.gridOperatorEventBind(gridId, $(this), GoContactsList.deleteContact, paramFields);
                });

                $(".list-item-edit").each(function () {
                    var paramFields = ["MItemID"];
                    Megi.gridOperatorEventBind(gridId, $(this), GoContactsList.editContactInfo, paramFields);
                });

                $(".a-viewcontact").each(function () {
                    var title = HtmlLang.Write(LangModule.Contact, "ViewContact", "View Contact");
                    var contactId = $(this).attr("key");
                    var url = "/BD/Contacts/ContactView/" + contactId;
                    //函数参数
                    var args = [title, url];
                    Megi.gridEventBind(gridId, $(this), $.mTab.addOrUpdate, args, "MName");

                });

                $(".span-primaryperson").each(function () {
                    Megi.gridEventBind(gridId, $(this), null, null, "MContactName");
                });
            }
        });
        $(gridId).resize();
    },
    editContactInfo: function (id) {
        //传页签过去
        var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
        $.mDialog.show({
            mTitle: id == "" ? HtmlLang.Write(LangModule.Contact, "AddContact", "Add Contact") : HtmlLang.Write(LangModule.BD, "EditContact", "Edit Contact"),
            mContent: "iframe:" + '/BD/Contacts/ContactsEdit?id=' + id + '&tabIndex=0',
            mWidth: 1100,
            mHeight: 450,
            mShowbg: true,
            mShowTitle: true,
            mCloseCallback: function () {
                $("#btnSearch").trigger("click");
            },
            mDrag: "mBoxTitle"
        });
    },
    archiveClick: function (isSeeAll) {
        var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
        var selectTabIndex = GoContactsList.getSelectTabIndex();
        var gridId = "#tbGrid" + selectTabIndex;
        var param = { isActive: false };
        //先去检查是否可禁用
        Megi.grid(gridId, "dbSelected", {
            url: "/BD/Contacts/IsCanDeleteOrInactive", callback: function (data) {
                var alertMsg = BDQuote.GetQuoteMsg(data, isSeeAll);
                if (data.Success) {
                    //可删除弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            //执行删除操作
                            Megi.grid(gridId, "dbSelected", {
                                url: "/BD/Contacts/ArchiveContact",
                                param: param,
                                callback: function (retdata) {
                                    if (retdata.Success) {
                                        $.mMsg(HtmlLang.Write(LangModule.BD, "ContactArchivedSuccess", "联系人禁用成功！"));
                                        GoContactsList.reload(tabTitle);
                                    } else {
                                        $.mDialog.alert(msg.Message);
                                    }

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
                    //不可删除弹出提示。
                    $.mDialog.alert(alertMsg);
                    if (isSeeAll) {
                        $("#popup_message").css("max-height", "300px");
                    } else {
                        $("#popup_message").css("max-height", "200px");
                    }
                }
            }
        });
    },
    deleteClick: function (isSeeAll) {
        var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
        var selectedTabIndex = GoContactsList.getSelectTabIndex();
        var gridId = "#tbGrid" + selectedTabIndex;

        var param = {};
        param.IsDelete = true;
        //先去检查是否可删除
        Megi.grid(gridId, "dbSelected", {
            url: "/BD/Contacts/IsCanDeleteOrInactive", param: param, callback: function (data) {
                var alertMsg = BDQuote.GetQuoteMsg(data, isSeeAll);
                if (data.Success) {
                    //可删除弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            //执行删除操作
                            Megi.grid(gridId, "dbSelected", {
                                url: "/BD/Contacts/DeleteContact",
                                param: param,
                                callback: function (retdata) {
                                    //if (retdata.Success) {
                                    $.mMsg(LangKey.DeleteSuccessfully);
                                    //}
                                    GoContactsList.reload(tabTitle);
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
    clickAction: function () {
        $("#NewContact").click(function () {
            GoContactsList.editContactInfo('');
        });
        $("#aImport").click(function () {
            ImportBase.showImportBox("/BD/Import/Import/Contact", HtmlLang.Write(LangModule.Contact, "ImportContacts", "Import Contacts"), 800, 500);
        });
        $("#NewGroup").click(function () {
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Contact, "AddGroup", "Add Group"),
                width: 400,
                height: 300,
                href: '/BD/Contacts/ContactGroupEdit',
                //mCloseBack
            });
        });

        $("#btnDelete").click(function () {
            GoContactsList.deleteClick();
        });
        $('a[id$="AddToGroup"]').click(function () {

            var selectTabIndex = GoContactsList.getSelectTabIndex();

            var gridId = "#tbGrid" + selectTabIndex;
            Megi.grid(gridId, "optSelected", {
                callback: function (ids) {
                    $.mDialog.show({
                        mTitle: HtmlLang.Write(LangModule.Contact, "AddToGroup", "Add to Group"),
                        //mContent: "iframe:" + '/BD/Contacts/AddToGroup/' + ids,
                        mWidth: 400,
                        mHeight: 300,
                        mShowbg: true,
                        mShowTitle: true,
                        mDrag: "mBoxTitle",
                        mContent: "iframe:" + '/BD/Contacts/AddToGroup/',
                        mPostData: { id: ids }
                    });
                }
            });
        });
        $('a[id$="Archive"]').click(function (obj) {
            GoContactsList.archiveClick();
        });

        $('a[id$="MoveToGroup"]').click(function () {
            var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
            var selectedTabIndex = GoContactsList.getSelectTabIndex();
            var gridId = "#tbGrid" + selectedTabIndex;
            typeKey = "#typeId" + selectedTabIndex;

            var typeid = $(typeKey).val();
            Megi.grid(gridId, "optSelected", {
                callback: function (ids) {
                    $.mDialog.show({
                        mTitle: HtmlLang.Write(LangModule.Contact, "MoveContacts", "Move Contacts"),
                        mWidth: 400,
                        mHeight: 300,
                        mShowbg: true,
                        mShowTitle: true,
                        mDrag: "mBoxTitle",
                        mContent: "iframe:" + '/BD/Contacts/MoveContacts',
                        mPostData: { id: ids, number: typeid, title: "" }
                    });
                }
            });
        });

        //删除分组
        $('a[id$="RemoveFromGroup"]').click(function () {
            var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
            var selectedTabIndex = GoContactsList.getSelectTabIndex();
            var gridId = "#tbGrid" + selectedTabIndex;
            typeKey = "#typeId" + selectedTabIndex;
            var typeid = $(typeKey).val();
            Megi.grid(gridId, "optSelected", {
                callback: function (ids) {
                    $.mDialog.confirm(HtmlLang.Write(LangModule.Contact, "SureRemoveThisContact", "Are you sure you want to remove this contact from this group?"),
                    {
                        height: 160,
                        callback: function () {
                            mAjax.submit(
                                "/BD/Contacts/ContactMoveOutGroup",
                                { selIds: ids, moveFromTypeId: typeid },
                                function (msg) {
                                    if (msg == true) {
                                        var successMsg = HtmlLang.Write(LangModule.Acct, "ContactRemoveFromGroup", "Contact removed from group") + ": " + tabTitle;
                                        $.mMsg(successMsg);
                                        GoContactsList.reload(tabTitle);
                                    } else {
                                        $.mDialog.alert(msg.Message);
                                    }
                                });
                        }
                    });
                }
            });
        });
        $('a[id$="DeleteFromGroup"]').click(function () {
            var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
            var selectedTabIndex = GoContactsList.getSelectTabIndex();
            var gridId = "#tbGrid" + selectedTabIndex;
            typeKey = "#typeId" + selectedTabIndex;
            var typeid = $(typeKey).val();

            if (typeid == "1" || typeid == "2" || typeid == "3") {
                var tips = HtmlLang.Write(LangModule.Contact, "CanNotDeleteGroup", "This is system default group ,can not be delete!");
                $.mDialog.alert(tips);
                return;
            } else if (!typeid || typeid == "") {
                var tips = HtmlLang.Write(LangModule.Contact, "NotSelectGroup", "请先选择要删除的分组！");
                $.mDialog.alert(tips);
                return;
            }

            $.mDialog.confirm(HtmlLang.Write(LangModule.Contact, "SureDeleteThisGroup", "Are you sure you want to delete this group? (Deleting a group does not delete any contacts) "),
            {
                height: 160,
                callback: function () {
                    mAjax.submit(
                        "/BD/Contacts/DelGroupAndLink",
                        { typeId: typeid },
                        function (msg) {
                            if (msg && msg.Success) {
                                var successMsg = HtmlLang.Write(LangModule.Acct, "GroupDeleted", "Group deleted") + ": " + tabTitle;
                                $.mMsg(successMsg);
                                //删除成功的话，要将tabkeysel恢复到第一个tab
                                $("#tabKeySel").val("0");
                                GoContactsList.reload(null, true);
                            } else {
                                $.mDialog.alert(msg.Message);
                            }
                        });
                }
            });
        });
        //恢复禁用联系人
        $('a[id$="Restore"]').click(function () {
            var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
            var selectedTabIndex = GoContactsList.getSelectTabIndex();
            var gridId = "#tbGrid" + selectedTabIndex;
            typeKey = "#typeId" + selectedTabIndex;
            var typeid = $(typeKey).val();
            Megi.grid(gridId, "optSelected", {
                callback: function (ids) {
                    $.mDialog.confirm(HtmlLang.Write(LangModule.Contact, "SureRestoreThisContact", "Are you sure you want to restore this contact?"),
                    {
                        callback: function () {
                            mAjax.submit(
                                "/BD/Contacts/ArchiveContact",
                                { keyIDs: ids, isActive: true },
                                function (msg) {
                                    if (msg.Success) {
                                        GoContactsList.reload(tabTitle);
                                    } else {
                                        $.mDialog.alert(msg.Message);
                                    }
                                });
                        }
                    });
                }
            });
        });

        $('input[id*="ShowInvoice"]').off("click").on("click", function () {

            if ($(this).is(":checked")) {
                $('input[id*="ShowInvoice"').attr("checked", "checked");
            }
            else {
                $('input[id*="ShowInvoice"').removeAttr("checked");
            }
            $('#btnSearch:visible').trigger("click");

        });

        $('a[id*="Search"]').click(function () {
            var tabTitle = $('#tbContactGroups').tabs('getSelected').panel('options').title;
            var selectedTabIndex = GoContactsList.getSelectTabIndex();
            var gridId = "#tbGrid" + selectedTabIndex;
            var typeId = $("#typeId" + selectedTabIndex).val();
            var searchId = $(this).attr("id");
            var searchFilter = searchId.substring((searchId.indexOf("Search") + 6));
            GoContactsList.searchFilter = searchFilter;
            var keyword = '';
            if (this.id == 'btnSearch') {
                keyword = Megi.encode($('#txtKey').val());
            }
            var showInvoice = $("input[id*='ShowInvoice']:visible").is(":checked");
            GoContactsList.gridFill(gridId, typeId, searchFilter, keyword, showInvoice);
        });
        $('#txtKey').keydown(function (e) {
            if (e.keyCode == 13) {
                $('#btnSearch').click();
            }
        });
        $("#aExport").click(function () {
            var queryParam = { typeId: GoContactsList.typeId, searchFilter: GoContactsList.searchFilter, keyword: $('#txtKey').val() };
            location.href = '/BD/Contacts/Export?jsonParam=' + escape($.toJSON(queryParam));
            $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });
    },
    reload: function (tabTitel, refreshPage) {
        if (refreshPage) {
            if (tabTitel == undefined) { tabTitel = "0"; }
            tabTitel = escape(tabTitel);
            mWindow.reload("/BD/Contacts/ContactsList?id=" + encodeURIComponent(tabTitel));
        } else {
            $("#btnSearch").trigger("click");
        }
    },
    deleteContact: function (id) {
        var obj = {};
        obj.KeyIDs = id;
        obj.IsDelete = true;
        mAjax.submit("/BD/Contacts/IsCanDeleteOrInactive", { param: obj }, function (response) {
            var alertMsg = BDQuote.GetQuoteMsg(response);
            if (response.Success) {
                //可删除弹出提示框是否继续删除
                $.mDialog.confirm(alertMsg, {
                    callback: function () {
                        //执行删除操作
                        mAjax.submit("/BD/Contacts/DeleteContact", { param: obj }, function (retResponse) {
                            if (retResponse.Success) {
                                $.mMsg(LangKey.DeleteSuccessfully);
                            } else {
                                $.mDialog.alert(retResponse.Message);
                            }
                            GoContactsList.reload();
                        });
                    }
                });

            } else {
                //不可删除弹出提示。
                $.mDialog.alert(alertMsg);
            }
        });
    },
    getSelectTabIndex: function () {
        var selectedTab = $("#tbContactGroups").tabs("getSelected");
        var selectTabIndex = $("#tbContactGroups").tabs("getTabIndex", selectedTab);

        return selectTabIndex;
    }
}

$(document).ready(function () {
    GoContactsList.init();
});