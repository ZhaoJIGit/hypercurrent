var AccountList = {
    IsRoad: false,
    IsClickEdit: false,
    IsClickDelete: false,
    AccountStandard: $("#hideAccountStandard").val(),
    IsCreateAccount: $("#hideIsCreateAccount").val(),
    IsMatchAccount: $("#hideIsMatchAccount").val(),
    hasBankChangeAuth: $("#hasBankChangeAuth").val(),
    AccountTreeData: null,
    selectId: null,
    tabswitch: new BrowserTabSwitch(),
    init: function () {
        AccountList.tabswitch.initSessionStorage();
        AccountList.initToolbar();
        AccountList.initMatchLogBtnWidth();
        //自定义科目，如果科目一次都没导入，弹窗对话框（不可关闭）
        if (AccountList.AccountStandard == 3 && !AccountList.IsCreateAccount) {
            AccountList.createAccountDialog();
        } else if (AccountList.AccountStandard == 3 && !AccountList.IsMatchAccount) {
            AccountList.accountMatchDialog();
        }

        //处理一下tabs的左右切换
        var tabLeftDom = $(".tabs-scroller-left");
        var tabRightDom = $(".tabs-scroller-right");

        if (tabLeftDom && tabRightDom) {
            tabLeftDom.css("display", "none");
            tabRightDom.css("display", "none");
        }

        AccountList.initUI();
    },
    initUI: function () {
        var isGuidPage = $("#hideIsSetup").val();

        if (!isGuidPage) {
            $("#btnAddTrack").css("display", "none");
        }
    },
    createAccountDialog: function () {
        $.mDialog.show({
            mWidth: 550,
            mHeight: 500,
            mShowbg: true,
            mContent: "id:divAccountCreateType",
            mShowTitle: false,
            closeFns: function () {
                AccountList.reload();
            }
        });
    },
    accountMatchDialog: function () {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.Acct, "CustomAccountMatch", "自定义科目匹配"),
            width: 800,
            height: 500,
            href: '/BD/BDAccount/CustomAccountMatch',
            closeCallback: function () {
                AccountList.reload();
            }
        });
    },
    initMatchLogBtnWidth: function () {
        if ($("#hidCurrentLangID", top.document).val() == "0x0009") {
            $("#divImport").width(250);
        }
    },
    IsPermisson: $("#hidePermisson").val(),
    initToolbar: function () {
        $("#aNewAccount").click(function () {
            //逻辑判断，如果用户没有选择一级科目，不允许新增
            var width = 670
            var height = AccountList.AccountStandard == 3 ? 550 : 500;

            Megi.dialog({
                title: HtmlLang.Write(LangModule.Acct, "AddAccount", "Add Account"),
                width: width,
                height: height,
                href: '/BD/BDAccount/AccountEdit?id=' + "&parentId=" + "&parentName=" + "&isLeaf=true" + "&isCombox=true",
                closeCallback: function () {
                    setTimeout(function () {
                        AccountList.reload(function () {
                            $("#btnSearch").trigger("click");
                        });
                    }, 100);
                }
            });
        });
        var bankHomeOj = new BDBankHome();
        $("#btnAddBankAccountTop").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[1].type, "", AccountList.reload) });
        //新增银行
        $("#btnAddBankAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[1].type, "", AccountList.reload) });
        //新增信用卡
        $("#btnAddCreditAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[2].type, "", AccountList.reload) });
        //新增现金帐户
        $("#btnAddCashAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[3].type, "", AccountList.reload) });
        //新增现金帐户
        $("#btnAddPayPalAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[4].type, "", AccountList.reload) });
        //新增现金帐户
        $("#btnAddAlipayAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[5].type, "", AccountList.reload) });

        $("#btnSearch").click(function () {
            var data = $("#tbAll").treegrid("getData");

            var keyword = $("#txtKeyword").val();

            if (!$.trim(keyword)) {
                $(".datagrid-row").show();
            } else {
                AccountList.search(data, keyword);
            }
        });



        $("#btnDelete").click(function () {
            AccountList.deleteAccount();
        });
        $("#btnArchive").click(function () {
            AccountList.archiveAccount();
        });

        $("#btnRestore").click(function () {
            AccountList.unArchiveAccount();
        });

        $("#aExport").click(function () {
            var queryParam = AccountList.getSearchParam();
            location.href = $("#hidGoServer").val() + '/BD/BDAccount/Export?jsonParam=' + escape($.toJSON(queryParam));
            $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });

        //$("#btnExport").off("click").on("click", function () {
        //    $("#aExport").trigger("click");
        //});

        $("#aPrint").click(function () {
            var title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Print", "Print"), HtmlLang.Write(LangModule.BD, "Account", "Account")]);
            var param = $.toJSON({ reportType: "Accounts", jsonParam: escape($.toJSON(AccountList.getSearchParam())) });
            Print.previewPrint(title, param, false);
        });

        $("#aImport").click(function () {
            if (AccountList.hasBankChangeAuth === "True") {
                ImportBase.showImportBox('/BD/Import/Import/Account', HtmlLang.Write(LangModule.IV, "ImportAccount", "Import Account"), 900, 520);
            }
            else {
                $("#aMatchLog").trigger("click");
            }
        });

        $("#btnImportAcct").click(function () {
            $("#aImport").trigger("click");
        });

        $("#btnImportInitBalance").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/OpeningBalance', $(this).text(), 900, 520);
        });

        //覆盖导入
        $("#btnReImport").click(function () {
            ImportBase.showImportBox('/BD/Import/Import?id=Account&isCover=true', HtmlLang.Write(LangModule.IV, "ReImportAccount", "导入会计科目(覆盖原有科目)"), 900, 520);
        });

        $("#btnCreateCustomAccount").click(function () {
            var checkValue = $('input:radio[name=rdoAccountCreateType]:checked').val();
            //根据系统默认进行生成
            if (checkValue == "0") {
                mAjax.submit("/BD/BDAccount/InsertDefaultAccount", {}, function (msg) {
                    if (msg && msg.Success) {
                        var message = HtmlLang.Write(LangModule.Acct, "AccountCreateSuccess", "科目生成成功！");
                        $.mDialog.message(message);
                        AccountList.reload();
                        $.mDialog.close();
                    } else {
                        var message = HtmlLang.Write(LangModule.Acct, "AccountCreateFail", "科目生成失败,请重试！");
                        $.mDialog.alert(message);
                    }

                }, "", true);
            } else if (checkValue == "1") {
                //导入
                $.mDialog.close();
                $("#btnReImport").trigger("click");

            }
        });

        $("#btnAccountMatch").click(function () {
            AccountList.accountMatchDialog();
        });

        $("#aMatchLog").click(function () {
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Acct, "ImportAccountMatchLog", "导入科目对照表"),
                width: 900,
                height: 520,
                href: '/BD/BDAccount/AccountMatchLog'
            });
        });

        $("#btnAddTrack").click(function () {
            var pageTitle = HtmlLang.Write(LangModule.BD, "Tracking", "Tracking");
            var url = $("#hidGoServer").val() + '/BD/Tracking/Index';
            $.mTab.addOrUpdate(pageTitle, url, true);
        });

        $("#tabAccount").tabs({
            onSelect: function (title , index) {
                AccountList.changeTabItem(title , index);
            }
        });

        //选择第一个tab
        $("#tabAccount").tabs("select", "");

    },
    editAccount: function (id, parentId, parentName, mdc, isLeaf) {
        //id不为空的时候
        var children = $("#tbAll").treegrid("getChildren", id);
        //是否叶子节点
        var isLeaf = children.length == 0 || isLeaf;

        if (id) {
            $("#tbAll").datagrid("selectRecord", id);
            var row = $("#tbAll").datagrid("getSelected");
            parentId = !parentId || parentId == "undefined" ? "0" : parentId;

            //AccountList.selectId = id;
        }
        var width = 680;
        var height = 530;
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddAccount", "Add Account") : HtmlLang.Write(LangModule.Acct, "EditAccount", "Edit Account");
        Megi.dialog({
            title: title,
            width: width,
            height: height,
            href: '/BD/BDAccount/AccountEdit?id=' + id + "&parentId=" + parentId + "&parentName=" + parentName + "&isLeaf=" + isLeaf + "&mdc=" + mdc,
            closeCallback: function () {
                setTimeout(function () {
                    AccountList.reload(function () {
                        $("#btnSearch").trigger("click");
                    });
                }, 100);

            }
        });
    },
    editBankAccount: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddBankAccount", "Add Bank Account") : HtmlLang.Write(LangModule.Acct, "EditBankAccount", "Edit Bank Account");
        $.mTab.addOrUpdate(title, '/BD/BDBank/BankAccountEdit/' + id);
    },
    editCreditCard: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddCreditCard", "Add Credit Card") : HtmlLang.Write(LangModule.Acct, "EditCreditCard", "Edit Credit Card");
        $.mTab.addOrUpdate(title, '/BD/BDBank/CreditCardEdit/' + id);
    },
    editCash: function (id) {
        var title = id == "" ? HtmlLang.Write(LangModule.Acct, "AddCash", "Add Cash") : HtmlLang.Write(LangModule.Acct, "EditCash", "Edit Cash");
        $.mTab.addOrUpdate(title, '/BD/BDBank/CashEdit/' + id);
    },
    deleteAccount: function (isSeeAll, id) {
        var specialAccount;
        if (id == undefined) {
            //获取所有选择项
            var rows = AccountList.getDisplaySelectionsRows();
            if (!rows || rows.length <= 0) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
                return;
            }
            var objParam = {};
            var keyIds = "";
            var haveSys = false;
            var haveSpecialAccount = false;
            for (var i = 0 ; i < rows.length; i++) {
                //银行科目不允许删除
                if (!rows[i].MIsSys && !(rows[i].MCode && (rows[i].MCode.indexOf("1001") == 0 || rows[i].MCode.indexOf("1002") == 0))) {
                    keyIds += rows[i].id + ",";
                }
                specialAccount = AccountList.isSpecialAccount(rows[i].MCode);
                if (!specialAccount) {
                    haveSpecialAccount = true;
                }
                if (rows[i].MIsSys) {
                    haveSys = true;
                }
            }
            if (haveSys || haveSpecialAccount) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "CanNotDeleteIncludeSysAccount", "所选科目包含系统默认科目、现金、银行，不能删除!"));
                return;
            }

            if (keyIds != "") {
                keyIds = keyIds.substring(0, keyIds.length - 1);
            } else {
                $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "HaveNotCanDeleteItem", "Have Not Can Delete Item,system default data or cash ,bank accounting can not delete!"));
                return;
            }

            objParam.KeyIDs = keyIds;
            objParam.IsDelete = true;

            mAjax.submit("/BD/BDAccount/IsCanDeleteOrInactive", { param: objParam }, function (response) {
                var alertMsg = BDQuote.GetQuoteMsg(response, isSeeAll);
                if (response.Success == true) {
                    //可删除的弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            mAjax.submit("/BD/BDAccount/DeleteAccount", { param: objParam }, function (delResponse) {
                                if (delResponse.Success == true) {
                                    $.mDialog.message(HtmlLang.Write(LangModule.Acct, "DeleteAccountSuccessfully", "Delete Account Successfully!"));
                                }
                                else {
                                    $.mDialog.message(delResponse.Message);
                                }
                                AccountList.reload(function () {
                                    $("#btnSearch").trigger("click");
                                });
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
                    AccountList.reload(function () {
                        $("#btnSearch").trigger("click");
                    });
                }
            });
        }
        else {
            var obj = {};
            obj.KeyIDs = id;
            obj.IsDelete = true;
            mAjax.submit("/BD/BDAccount/IsCanDeleteOrInactive", { param: obj }, function (response) {
                var alertMsg = BDQuote.GetQuoteMsg(response);
                if (response.Success == true) {
                    //可删除的弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            mAjax.submit("/BD/BDAccount/DeleteAccount", { param: obj }, function (delResponse) {
                                if (delResponse.Success == true) {
                                    $.mDialog.message(HtmlLang.Write(LangModule.Acct, "DeleteAccountSuccessfully", "Delete Account Successfully!"));
                                }
                                else {
                                    $.mDialog.message(delResponse.Message);
                                }
                                AccountList.reload(function () {
                                    $("#btnSearch").trigger("click");
                                });
                            });
                        }
                    });
                } else {
                    //不可删除弹出提示。
                    $.mDialog.alert(alertMsg);
                    AccountList.reload(function () {
                        $("#btnSearch").trigger("click");
                    });
                }
            });
        }
    },
    //获取可见且选中的Rows
    getDisplaySelectionsRows: function () {
        //获取选中行
        var rows = $("#tbAll").treegrid("getSelections");
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
    archiveAccount: function (isSeeAll) {
        var rows = AccountList.getDisplaySelectionsRows();
        if (!rows || rows.length <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
            return;
        }
        var objParam = {};
        var keyIds = "";
        var haveSpecialAccount = false;

        for (var i = 0 ; i < rows.length; i++) {
            if ($("tr[node-id='" + rows[i].id + "']").css("display") == "none") {
                continue;
            }

            keyIds += rows[i].id + ",";

            var specialAccount = AccountList.isSpecialAccount(rows[i].MCode);
            if (!specialAccount) {
                haveSpecialAccount = true;
            }
            var childrens = $("#tbAll").treegrid("getChildren", rows[i].id);
            if (childrens) {
                //选中所有的子节点
                for (var j = 0; j < childrens.length; j++) {
                    var childrenNode = childrens[j];

                    $("#tbAll").treegrid("select", childrenNode.id);
                    keyIds += childrenNode.id + ",";
                }
            }
        }


        if (haveSpecialAccount) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "CanNotArchiveIncludeSysAccount", "所选科目包含现金、银行、往来类科目，不能禁用!"));
            return;
        }

        if (keyIds != "") {
            keyIds = keyIds.substring(0, keyIds.length - 1);
        } else {
            $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "HaveNotCanArchiveItem", "Have not can delete item,system default data can not archive!"));
            return;
        }

        objParam.KeyIDs = keyIds;
        objParam.url = "/BD/BDAccount/ArchiveAccount";

        mAjax.submit("/BD/BDAccount/IsCanDeleteOrInactive", { param: objParam }, function (response) {
            var alertMsg = BDQuote.GetQuoteMsg(response, isSeeAll);
            if (response.Success == true) {
                //可禁用弹出提示框是否继续禁用
                $.mDialog.confirm(alertMsg, {
                    callback: function () {
                        mAjax.submit("/BD/BDAccount/ArchiveAccount", { param: objParam }, function (delResponse) {
                            if (delResponse.Success == true) {
                                $.mMsg(HtmlLang.Write(LangModule.Acct, "ArchiveAccountSuccessfully", "Archive Account Successfully!"));
                            }
                            else {
                                $.mDialog.message(delResponse.Message);
                            }
                            AccountList.reload(function () {
                                $("#btnSearch").trigger("click");
                            });
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
                AccountList.reload(function () {
                    $("#btnSearch").trigger("click");
                });
            }
        });

    },
    unArchiveAccount: function () {
        var rows = AccountList.getDisplaySelectionsRows();
        if (!rows || rows.length <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
            return;
        }
        var objParam = {};
        var keyIds = "";
        for (var i = 0 ; i < rows.length; i++) {

            if ($("tr[node-id='" + rows[i].id + "']").css("display") == "none") {
                continue;
            }

            keyIds += rows[i].id + ",";
        }

        if (keyIds != "") {
            keyIds = keyIds.substring(0, keyIds.length - 1);
        } else {
            $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "HaveNotCanArchiveItem", "Have not can delete Item,system default data can not archive!"));
            return;
        }

        objParam.KeyIDs = keyIds;
        objParam.url = "/BD/BDAccount/RestoreAccount";

        mAjax.submit(objParam.url, { param: objParam }, function (msg) {
            if (msg.Success) {
                msg.Message = HtmlLang.Write(LangModule.Acct, "UnArchiveAccountSuccessfully", "Restore Account Successfully!")
                $.mDialog.message(msg.Message);
                AccountList.reload(function () {

                    $("#btnSearch").trigger("click");
                });
            } else {
                $.mDialog.message(msg.Message);

            }
        });

    },
    changeTabItem: function (title, index) {
        $("#txtKeyword").val("");
        AccountList.IsRoad = true;

        var disableTabIndex = AccountList.AccountStandard == "1" ? 7 : 6;

        if (index == disableTabIndex) {
            $("#btnDelete").hide();
            $("#btnArchive").hide();
            $("#btnRestore").show();
        } else {
            $("#btnDelete").show();
            $("#btnArchive").show();
            $("#btnRestore").hide();
        }
        //清空所选项
        //AccountList.selectId = null;
        AccountList.bindGrid();
    },
    reload: function (callback) {
        $("#tbAll").treegrid("unselectAll");
        AccountList.bindGrid(callback);
    },
    reloadFromImport: function () {
        AccountList.reload(function () {
            $("#btnSearch").trigger("click");
        });
    },
    getSearchParam: function () {
        var objInfo = AccountList.getTabSelectedInfo();
        var keyword = Megi.encode($("#txtKeyword").val());
        return { Group: objInfo.Title, Keyword: keyword, IsActive: objInfo.IsActive, GroupID: objInfo.GroupID };
    },
    bindGrid: function (callback) {

        var params = AccountList.getSearchParam();

        if (callback && $.isFunction(callback)) {
            params.Keyword = "";
        }

        $("#tbAll").treegrid({
            url: "/BD/BDAccount/GetAccountList",
            queryParams: params ,
            idField: "id",
            treeField: 'text',
            checkbox: true,
            fitColumns: true,
            singleSelect: false,
            scrollY: true,
            height: $(".m-imain-content").height() - $("#tbAll").offset().top,
            lines: true,
            region: "center",
            columns: [[{
                field: 'id', width: 40, checkbox: true, formatter: function (value, rec, rowIndex) {
                    var isCanAddLowerAccount = AccountList.isCanAddLowerAccount(rec.MCode);
                    if (!rec.MIsSys && isCanAddLowerAccount) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\" disabled value=\"" + rec.MItemID + "\" >";
                    }
                }
            },
            {
                title: "MCode", field: 'MCode', width: 200, sortable: false, hidden: true
            },
             { title: HtmlLang.Write(LangModule.Common, "AccountCode", "Account Code"), field: 'MNumber', width: 50, sortable: false },
            {
                title: HtmlLang.Write(LangModule.Common, "AccountName", "Account Name"), field: 'text', width: 200, sortable: false, formatter: function (value, rec, rowIndex) {
                    return mText.encode(value);
                }
            },
            {
                title: LangKey.Type, field: 'MAcctTypeName', width: 120, align: 'center', sortable: false
            },

            {
                title: HtmlLang.Write(LangModule.Acct, "Direction", "Direction"), field: 'MDC', width: 50, align: 'center', sortable: false, formatter: function (value, rec, rowIndex) {
                    if (value && value == 1) {
                        return HtmlLang.Write(LangModule.Acct, "Debit", "Debit");
                    } else if (value && value == -1) {
                        return HtmlLang.Write(LangModule.Acct, "Credit", "Credit");
                    } else {
                        return '';
                    }
                }
            },
            {
                title: HtmlLang.Write(LangModule.Acct, "CheckType", "核算维度"), field: 'MCheckGroupName', width: 120, align: 'left', sortable: false, formatter: function (value, rec, rowIndex) {
                    if (rec.CheckGroupNameList && rec.CheckGroupNameList.length > 0) {
                        var enableRequiredString = HtmlLang.Write(LangModule.Acct, "CheckTypeRequired", "启用必录");
                        var enableOptionalString = HtmlLang.Write(LangModule.Acct, "CheckTypeOptional", "启用选录");
                        var html = "";
                        for (var i = 0 ; i < rec.CheckGroupNameList.length; i++) {
                            var checkType = rec.CheckGroupNameList[i];
                            if (checkType.MValue2 == "2") {
                                html += "<span style='padding-right:8px' title='" + enableRequiredString + "'><font color='red'>*</font>" + mText.encode(checkType.MName) + "</span>";
                            } else if (checkType.MValue2 == "1") {
                                html += "<span style='padding-right:8px' title='" + enableOptionalString + "'>" + mText.encode(checkType.MName) + "</span>";
                            }
                        }
                        return html;
                    } else {
                        return "";
                    }
                }
            },

            { title: "MIsSys", field: 'MIsSys', hidden: "true" },
            {
                title: HtmlLang.Write(LangModule.Acct, "Operation", "Operation"), field: 'Action', align: 'left', width: 70, sortable: false, formatter: function (value, rec, rowIndex) {
                    //如果是银行和现金库存科目，不允许增加三级科目
                    //if (rec.MCode.indexOf())

                    if (!AccountList.IsPermisson) {
                        return "";
                    }
                    var addTitle = HtmlLang.Write(LangModule.Acct, "AddLowerLevelAccount", "Add lower level account");
                    //禁用条件下不允许修改
                    if (AccountList.getSearchParam().IsActive) {
                        var html = "<div class='list-item-action'>";
                        var isCanAddLowerAccount = AccountList.isCanAddLowerAccount(rec.MCode);

                        if (isCanAddLowerAccount) {
                            html += "<a href='javascript:void(0);' title='" + addTitle + "' onclick=\"AccountList.IsClickEdit = true;AccountList.editAccount('','" + rec.id + "','" + mText.encode(rec.text) + "','" + rec.MDC + "','true')\" class='m-icon-add-row'></a>";
                        }
                        html += "<a href='javascript:void(0);' onclick=\"AccountList.IsClickEdit = true;AccountList.editAccount('" + rec.id + "','" + rec._parentId + "','')\" class='list-item-edit'></a>";
                        if (!rec.MIsSys && isCanAddLowerAccount) {
                            html += "<a href='javascript:void(0);' onclick=\"AccountList.IsClickDelete = true;AccountList.deleteAccount(false,'" + rec.id + "');\" class='list-item-del'></a>";
                        }
                        html += "</div>";
                    }
                    return html;
                }
            }]],
            onLoadSuccess: function (row, data) {
                //解决IE下重新加载，搜索框失效
                if (Megi.isIE()) {
                    $("#txtKeyword").focus();
                }

                if ($("#hidIsSetup").val() === "true") {
                    SetupBase.adjustGridHeight();
                } else {
                    AccountList.autoHeight();
                }


                //为所有的checkbox绑定点击事件
                $("input[type='checkbox']").bind("click", function () {
                    var dom = $(this);

                    var id = $(dom).attr("value");
                    setTimeout(function () {
                        $("#tbAll").treegrid('cascadeCheck', {
                            id: id, //节点ID  
                            deepCascade: false //深度级联  
                        });
                    }, 100);

                });

                if (callback && $.isFunction(callback)) {
                    callback();
                }

                $(this).treegrid("unselectAll");

                if (AccountList.selectId) {
                    $(this).treegrid("select", AccountList.selectId);
                }
            },
            onClickRow: function (row) {
                //$(this).treegrid('cascadeCheck', {
                //    id: row.id, //节点ID  
                //    deepCascade: true //深度级联  
                //});
            },
            onClickCell: function (field, index, value) {
                return false;
            }
        });
    },
    getTabSelectedInfo: function () {
        var obj = {};
        obj.Title = "";
        obj.IsActive = true;
        var selector = $("#tabAccount");
        //找到Id为tabAccount后面所有 find(".tabs>li")页签的长度
        var count = selector.find(".tabs>li").length;
        //遍历每一个页签li标签

        var selectedTab = selector.tabs('getSelected');
        var index = selector.tabs('getTabIndex', selectedTab);

        if (index > 0 && index < count - 1) {
            //给对象的Title赋值
            //obj.Title = $(this).find(".tabs-title").html();
            obj.GroupID = AccountList.getAccountGroupID(index);
        }
        if (index == count - 1) {
            obj.IsActive = false;
        }

        //$("#tabAccount").find(".tabs>li").each(function (i) {
        //    //谁被选中  hasClass() 谁class为tabs-selected
        //    if ($(this).hasClass("tabs-selected")) {
        //        if (i > 0 && i < count - 1) {
        //            //给对象的Title赋值
        //            obj.Title = $(this).find(".tabs-title").html();
        //        }
        //        if (i == count - 1) {
        //            obj.IsActive = false;
        //        }
        //        return;
        //    }
        //});
        return obj;
    },
    //找科目类别和tabIndex对应关系
    getAccountGroupID: function (tabIndex) {

        //小企业会计准则，没有共同类
        if (AccountList.AccountStandard == "2") {
            return tabIndex;
        }

        var result = "";
        switch (tabIndex) {
            case 3:
                result = 6
                break;
            case 4:
                result = 3;
                break;
            case 5:
                result = 4;
                break;
            case 6:
                result = 5;
                break;
            default:
                result = tabIndex;
                break;
        }

        return result;
    },
    autoHeight: function () {

        var selectorName = ".m-imain .m-imain-content";
        //grid的第一父元素
        var containner = $(selectorName);
        //
        $(containner).height($("body").height() - $(containner).offset().top - 5);
        //减去父元素的上级padding作为grid的高度 hideAutoHeight:配置的调整高度
        var gridWrapDivHegith = containner.height() - 20;// - $("#hideAutoHeight").val();
        $(".datagrid-wrap", selectorName).height(gridWrapDivHegith);

        var gridViewDiv = $(".datagrid-view", selectorName);
        gridViewDiv.height(gridWrapDivHegith);

        var gridBody = $(".datagrid-body", selectorName);
        //数据显示的高度需要减去表头占用高度
        var gridBodyHeight = gridViewDiv.height() - $(".datagrid-header", selectorName).height();
        gridBody.height(gridBodyHeight);

        //去除右侧多余空白
        $(".datagrid").width($(".datagrid-wrap", selectorName).width());
    },
    isCanAddLowerAccount: function (number) {
        //只算父级id
        if (number && (number.indexOf("1001") == 0 || number.indexOf("1002") == 0))
            //|| number.indexOf("1122") == 0 || number.indexOf("1123") == 0 || number.indexOf("1221") == 0
            //|| number.indexOf("2202") == 0 || number.indexOf("2203") == 0 || number.indexOf("2241") == 0)
        {
            return false;
        }
        return true;
    },
    isSpecialAccount: function (number) {
        //只算父级id
        if (number && ((number.indexOf("1001") == 0 || number.indexOf("1002") == 0)
            || number == "1122" || number == "1123" || number == "1221"
            || number == "2202" || number == "2203" || number == "2241")) {
            return false;
        }
        return true;
    },
    //数据前端查找
    search: function (data, keyword) {
        var length = data == null ? 0 : data.length;
        var hasMatch = false;
        if (length == 0) {
            return hasMatch;
        }
        for (var i = 0; i < length; i++) {
            var row = data[i];
            var match = row.MNumber.indexOf(keyword) >= 0 || row.text.indexOf(keyword) >= 0;
            var subMatch = false;
            //如果父级科目匹配，加载所有的数据
            var tr = $("tr[node-id='" + row.id + "']");
            if (!match) {
                var children = row.children;
                subMatch = AccountList.search(children, keyword);
            } else {
                //把它的所有子集都给显示出来
                var childTr = $(tr).next();
                if ($(childTr).hasClass("treegrid-tr-tree")) {
                    $(".datagrid-row", childTr).show();
                }

            }


            if (match || subMatch) {
                $(tr).show();
            } else {
                $(tr).hide()
            }

            hasMatch = match || subMatch || hasMatch;
        }

        return hasMatch;
    }
}
$(document).ready(function () {
    AccountList.init();
});