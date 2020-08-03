var BalanceInput = {
    defaultAccountId: $("#hideDefaultId").val(),
    baseCurrencyId: $("#hideBaseCurrency").val(),
    //基本列的长度
    baseColumnLength: 5,
    combobxDefaultHeight: 35,
    currencyList: null,
    contactList: null,
    employeeList: null,
    merchandiseItemList: null,
    expenseItemList: null,
    paList: null, //工资项
    trackList: null,
    trackItem1Option: null,
    trackItem2Option: null,
    trackItem3Option: null,
    trackItem4Option: null,
    trackItem5Option: null,
    editorRowIndex: null,
    bankList: null,
    account: null,
    billType: null,
    //单据类型
    gridId: "#gridAccountBalance",
    forbitEdit: $("#hideForbitEdit").val().toLowerCase() == "true",
    balanceData: null,
    totalRows: null,
    //是否校验数据合法性
    isValidateData: true,
    glMonth: $("#hideGLMonth").val(),
    init: function () {
        //加载科目分组数据
        BalanceInput.loadAccountGroup();
        //初始化科目树
        BalanceInput.loadAccountTree();
        BalanceInput.initAction();
        BalanceInput.initUI();
        BalanceInput.initBillType();

    },
    initUI: function () {
        if (BalanceInput.forbitEdit) {
            $("#btnReInit").show();
            $("#btnSave").hide();
            $("#btnFinishInput").hide();
        } else {
            $("#btnSave").show();
            $("#btnFinishInput").show();
            $("#btnFinish").show();
        }
    },
    initBillType: function () {
        if (BalanceInput.billType == null) {
            BalanceInput.billType = [
            //销售发票
            {
                "id": "Invoice_Sale",
                "value": HtmlLang.Write(LangModule.Common, "InvoiceSale", "销售单")
            },
            //销售发票
            {
                "id": "Invoice_Sale_Red",
                "value": HtmlLang.Write(LangModule.Common, "InvoiceSaleRed", "红字销售单")
            },
            //采购单
            {
                "id": "Invoice_Purchase",
                "value": HtmlLang.Write(LangModule.Common, "InvoicePurchase", "采购单")
            },
            //红字采购单
            {
                "id": "Invoice_Purchase_Red",
                "value": HtmlLang.Write(LangModule.Common, "InvoicePurchaseRed", "红字采购单")
            },
            //预收单
            {
                "id": "Receive_Sale",
                "value": HtmlLang.Write(LangModule.Common, "ReceiveSale", "收款单")
            },
            //预付单
            {
                "id": "Pay_Purchase",
                "value": HtmlLang.Write(LangModule.Common, "PayPurchase", "付款单")
            },
            //费用报销单
            {
                "id": "Expense_Claims",
                "value": HtmlLang.Write(LangModule.Common, "Expense", "费用报销单")
            }];
        }
    },
    initAction: function () {
        $("#btnSave").click(function (ignorePrompt) {
            BalanceInput.saveData(ignorePrompt);
        });

        $(".searchbox-menu").click(function () {
            var accountGroupDiv = $(".bi-searchbox-content-div");
            if (accountGroupDiv.css("display") == "none") {
                accountGroupDiv.show();
            } else {
                accountGroupDiv.hide();
            }
        });

        $(".bi-searchbox-content-li").click(function () {
            var accountGroupId = $(this).attr("grouptype");
            var accountGroupName = $(this).text();

            $(".l-btn-text", ".searchbox-menu").text(accountGroupName).attr("groupid", accountGroupId);
            $(".bi-searchbox-content-div").hide();
            $(".searchbox-text").val("");

            $("#treeAccount").tree("doFilter", function (node) {

                return node.MAccountGroupID == accountGroupId || accountGroupId == "";
            });
        });

        $(".bi-menu-shrink").click(function () {

            var status = $(this).attr("status");
            if (status == "open") {
                $(this).removeClass("bi-menu-shrink-open");
                $(this).addClass("bi-menu-shrink-close");
                $(".bi-content-left").hide();
                $(this).attr("status", "close");
            } else if (status == "close") {
                $(this).removeClass("bi-menu-shrink-close");
                $(this).addClass("bi-menu-shrink-open");
                $(".bi-content-left").show();
                $(this).attr("status", "open");
            }
            BalanceInput.divAutoSize();

        });

        $(window).resize(function () {
            setTimeout(function () {
                BalanceInput.divAutoSize();
            }, 100);

        });

        $("#btnFinishInput").click(function () {
            BalanceInput.saveData(function () {
                var confirmMsg = HtmlLang.Write(LangModule.Acct, "ConfirmToInitDocumentPage", "是否进行期初单据的录入?");
                $.mDialog.confirm(confirmMsg, function () {
                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.BD, "DocInitBills", "科目单据初始化"), $("#hideGoServer").val() + '/BD/InitDocument/InitDocumentIndex', true, true, false, true);
                }, function () {
                    if ($("#btnFinish").css("display") == "none") {

                    } else {
                        $("#btnFinish").trigger("click");
                    }
                });
            }, true);
        });

        $("#btnAddAccount").click(function () {
            //逻辑判断，如果用户没有选择一级科目，不允许新增
            var width = 550;
            var height = 500;

            Megi.dialog({
                title: HtmlLang.Write(LangModule.Acct, "AddAccount", "Add Account"),
                width: width,
                height: height,
                href: '/BD/BDAccount/AccountEdit?id=' + "&parentId=" + "&parentName=" + "&isLeaf=true" + "&isCombox=true",
                closeCallback: function () {
                    BalanceInput.loadAccountTree();
                }
            });
        });

        $(".searchbox-button").click(function () {

            var groupId = $(".l-btn-text", ".searchbox-menu").attr("groupid");

            var keyword = $(".searchbox-text").val();
            if (groupId) {
                $("#treeAccount").tree("doFilter", function (node) {
                    return (node.MAccountGroupID == groupId && keyword == "") || (node.MAccountGroupID == groupId && node.text.indexOf(keyword) >= 0);
                });
            } else {
                $("#treeAccount").tree("doFilter", keyword);
            }

        });

        $(".searchbox-text").keyup(function (e) {
            $(".searchbox-button").trigger("click");
        });

        $("body").click(function (e) {

            if (!$(e.srcElement).hasClass("datagrid-body")) {
                return;
            }

            if (BalanceInput.validateEditRow()) {

                if (BalanceInput.editorRowIndex != null && !BalanceInput.checkAmountIsValid()) {
                    var tips = HtmlLang.Write(LangModule.Acct, "AmountNotEqual", "金额录入了原币金额没有录入本位币金额或者录入了本位币金额，没有原币金额，确定要结束编辑?");

                    $.mDialog.confirm(tips, function () {
                        BalanceInput.endGridRowEdit();
                        BalanceInput.editorRowIndex = null;
                    });
                } else {
                    BalanceInput.endGridRowEdit();
                    BalanceInput.editorRowIndex = null;
                }
            } else if ($("#hideEditPage").val() == true) {
                var tips = HtmlLang.Write(LangModule.Acct, "EditRowDataNotValidate", "编辑行数据不合法，无法结束编辑");
                $.mDialog.message(tips);
                return false;
            }
        });

        $("#ckbAutoAddInitDocument").click(function () {

            var url = "/BD/BDAccount/SetAccountCreateInitBill";

            var selectAccountNode = BalanceInput.getSelectedAccount();

            var accountId = selectAccountNode.id;
            var isCreateInitBill = $("#ckbAutoAddInitDocument").is(":checked");

            $.mAjax.submit(url, { accountId: accountId, createInitBill: isCreateInitBill }, function (data) {
                if (data && data.Success) {
                    var selectNode = BalanceInput.getSelectedAccount();
                    selectAccountNode.MCreateInitBill = isCreateInitBill;

                    if (BalanceInput.account != null) {
                        BalanceInput.account.MCreateInitBill = isCreateInitBill;
                    }

                    //保存一下当前数据
                    if (isCreateInitBill) {
                        $("#btnSave").trigger("click", true);
                    }
                    ;
                } else {

                    if (isCreateInitBill) {
                        $("#ckbAutoAddInitDocument").removeAttr("checked");
                    } else {
                        $("#ckbAutoAddInitDocument").attr("checked", "checked");
                    }

                    var message = data.Message ? data.Message : HtmlLang.Write(LangModule.Acct, "ChangeCreateInitBillStatusFailed", "无法更改是否生成期初单据状态！");

                    $.mDialog.alert(message);
                }

            });
        });
    },
    loadAccountGroup: function () {
        var url = "/BD/BDAccount/GetAccountGroupList"
        $.mAjax.post(url, null, function (data) {
            if (data && data.length > 0) {
                var searchContentTypeDiv = $(".bi-searchbox-content-div");

                $(searchContentTypeDiv).append("<div class='bi-searchbox-content-li menu-item' grouptype=''>" + HtmlLang.Write(LangModule.Acct, "AllAccount", "所有科目") + "</di>");

                for (var i = 0 ; i < data.length; i++) {
                    var accountTypeDom = "<div class='bi-searchbox-content-li menu-item' grouptype='" + data[i].MItemID + "'>" + data[i].MName + "</di>";
                    $(searchContentTypeDiv).append(accountTypeDom);
                }
            }
        }, false, true, false);
    },
    //加载科目树
    loadAccountTree: function () {
        var url = "/BD/BDAccount/GetAccountList?ShowNumber=true&IsActive=true";
        var treeId = "#treeAccount";
        mAjax.post(url, {}, function (data) {
            if (data) {
                $(treeId).tree({
                    data: data,
                    filter: function (keyword, node) {
                        return node.text.toLowerCase().indexOf(keyword.toLowerCase()) >= 0 || node.MAccountGroupID == keyword;
                    },
                    onSelect: function (node) {
                        if (!BalanceInput.isValidateData) {
                            BalanceInput.isValidateData = true;
                            return;
                        }

                        if (!BalanceInput.validateEditRow()) {
                            var tips = HtmlLang.Write(LangModule.Acct, "EditRowDataNotValidateChangeAccount", "数据录入未完成，是否放弃录入？");
                            $.mDialog.confirm(tips, function () {
                                $("#hideEditPage").val(false);
                                BalanceInput.changeAccount(node);
                            }, function () {
                                BalanceInput.isValidateData = false;
                                var preNode = $(treeId).tree("find", BalanceInput.account.MItemID);
                                $(treeId).tree("select", preNode.target);
                            });
                        } else {
                            BalanceInput.confirmSaveData(function () {
                                $("#hideEditPage").val(false);
                                BalanceInput.changeAccount(node);
                            }, true);
                        }
                    },
                    onBeforeSelect: function (node) {
                        if (BalanceInput.account && node.id == BalanceInput.account.MItemID) {
                            return false;
                        }
                        return true;
                    },
                    onLoadSuccess: function () {
                        if (BalanceInput.defaultAccountId) {
                            var node = $(this).tree("find", BalanceInput.defaultAccountId);
                            $(this).tree("select", node.target);
                            BalanceInput.changeAccount(node);
                        }
                    }
                });
            }
        }, false, false, true);
    },
    confirmSaveData: function (callback) {
        //如果正在编辑页面，提示用户是否保存当前数据
        var editPage = $("#hideEditPage").val();

        if (editPage && editPage == "true" && !BalanceInput.isEmptyRow()) {
            var tips = HtmlLang.Write(LangModule.Acct, "ConfirmSaveData", "当前页面处于编辑状态，是否保存当前数据?");
            $.mDialog.confirm(tips, function () {
                BalanceInput.saveData(callback);
            },
            function () {
                callback();
            })
        } else {
            callback();
        }
    },
    changeAccount: function (node, refresh) {

        if (!refresh && BalanceInput.account && node.id == BalanceInput.account.MItemID) {
            return;
        }

        $("body").mask("");
        //编辑每一行的时候，调用取基础资料的函数
        BalanceInput.initBaseData(function () {

            var direction = node.MDC == 1 ? HtmlLang.Write(LangModule.Acct, "Debit", "Debit") : HtmlLang.Write(LangModule.Acct, "Credit", "Credit");

            var accountName = node.MFullName + " [" + direction + "]";

            $(".bi-account-text", "#lblCurrentAccount").text(accountName);
            BalanceInput.bindBalanceData(node);
        });
    },
    bindBalanceData: function (node) {
        var isLeafNode = BalanceInput.isLeafAccount();

        var queryParam = {};
        queryParam.AccountID = node.id;
        queryParam.IncludeCheckTypeData = true;

        var gridData = {};

        var url = "/BD/BDAccountBalance/GetInitBalanceList";
        $.mAjax.post(url, { filter: queryParam }, function (data) {
            if (data && data.length > 0) {
                if (!isLeafNode) {
                    gridData = data.where("x.MCheckGroupValueID=='0'");
                } else {
                    gridData = BalanceInput.gridDataProcess(data);
                }
                BalanceInput.balanceData = data;
            } else {
                gridData = [];
                BalanceInput.balanceData = [];
            }
        }, false, true, false);

        //如果科目设置了自动生成单据
        if (node.MCreateInitBill) {
            $("#ckbAutoAddInitDocument").attr("checked", "checked");
        } else {
            $("#ckbAutoAddInitDocument").removeAttr("checked");
        }

        $("body").mask("");
        BalanceInput.getGridColumns(node.id, function (columns) {
            var gridId = "#gridAccountBalance";

            $(gridId).datagrid({
                data: gridData,
                resizable: true,
                auto: true,
                fitColumns: true,
                columns: columns,
                scrollY: true,
                onClickCell: function (rowIndex, field, value) {
                    var rowData = $(gridId).datagrid("getRowByIndex", rowIndex);
                    //这两个字段不触发编辑事件
                    if (field == "MItemID") {
                        if (BalanceInput.forbitEdit || !BalanceInput.isLeafAccount()) {
                            var tips = HtmlLang.Write(LangModule.Acct, "CanInputInitBalance", "不允许录入期初余额，科目已完成余额初始化或者没有编辑权限");
                            $.mDialog.message(tips);
                            return;
                        }
                        BalanceInput.insertEmptyRow(gridId, 1, rowIndex);
                        return;
                    }

                    if (field == "Op") {
                        if (BalanceInput.forbitEdit || !BalanceInput.isLeafAccount()) {
                            var tips = HtmlLang.Write(LangModule.Acct, "CanInputInitBalance", "不允许录入期初余额，科目已完成余额初始化或者没有编辑权限");
                            $.mDialog.message(tips);
                            return;
                        }
                        BalanceInput.deleteBalance(rowIndex);
                        return;
                    }

                    if ((rowData.MCheckGroupValueID != "0" && BalanceInput.validateEditRow())) {
                        $("#hideEditPage").val(true);

                        if (BalanceInput.editorRowIndex != null) {
                            BalanceInput.endGridRowEdit();
                        }

                        $(gridId).datagrid("beginEdit", rowIndex);

                        BalanceInput.editorRowIndex = rowIndex;

                        BalanceInput.bindNumberboxKeyupEvent();
                        //一些数据规则的设置
                        BalanceInput.setDataRule();
                    }
                },
                onBeforeEdit: function (rowIndex, rowData) {

                    if (BalanceInput.forbitEdit) {
                        var tips = HtmlLang.Write(LangModule.Acct, "CanInputInitBalance", "不允许录入期初余额，科目已完成余额初始化或者没有编辑权限");
                        $.mDialog.message(tips);
                        $("#hideEditPage").val(false);
                        return false;
                    }

                    //父级科目，合计行不允许编辑
                    if (!BalanceInput.isLeafAccount()) {
                        var tips = HtmlLang.Write(LangModule.Acct, "ParentAccountCanNotInputBalance", "父级科目不允许录入期初数据");
                        $.mDialog.message(tips);
                        $("#hideEditPage").val(false);
                        return false;
                    }

                    if (BalanceInput.validateEditRow()) {
                        //释放所有的必录属性
                        if (BalanceInput.editorRowIndex != null && !BalanceInput.checkAmountIsValid()) {
                            var tips = HtmlLang.Write(LangModule.Acct, "AmountNotEqual", "金额录入了原币金额没有录入本位币金额或者录入了本位币金额，没有原币金额，确定要结束编辑?");

                            $.mDialog.confirm(tips, function () {
                                BalanceInput.endGridRowEdit();

                                if (rowData.MCheckGroupValueID != "0") {
                                    BalanceInput.editorRowIndex = rowIndex;
                                } else {
                                    BalanceInput.editorRowIndex = null;
                                }
                            });
                        } else {
                            BalanceInput.endGridRowEdit();
                            if (rowData.MCheckGroupValueID != "0") {
                                BalanceInput.editorRowIndex = rowIndex;
                            } else {
                                BalanceInput.editorRowIndex = null;
                            }
                        }
                    } else {
                        var tips = HtmlLang.Write(LangModule.Acct, "EditRowDataNotValidate", "编辑行数据不合法，无法结束编辑");
                        $.mDialog.message(tips);
                        return false;
                    }
                },
                onBeforeLoad: function () {

                },
                onLoadSuccess: function () {
                    BalanceInput.reloadUI(gridId);

                    BalanceInput.initTotalRowData();
                    //开始初始合计模块的金额
                    BalanceInput.refreshTotalUI();
                    BalanceInput.divAutoSize();
                    $("body").unmask();
                }
            });
        });
    },
    initTotalRowData: function () {
        BalanceInput.totalRows = BalanceInput.balanceData.where("x.MCheckGroupValueID == '0'");
    },
    refreshTotalUI: function () {
        var data = BalanceInput.totalRows;

        //定义本位币金额
        var baseCurrencyTotalRow = {};
        baseCurrencyTotalRow.MCurrencyID = HtmlLang.Write(LangModule.Acct, "BaseCurrencyTotal", "综合本位币");
        baseCurrencyTotalRow.MInitBalanceFor = 0;
        baseCurrencyTotalRow.MInitBalance = 0;
        baseCurrencyTotalRow.MYtdDebitFor = 0;
        baseCurrencyTotalRow.MYtdDebit = 0;
        baseCurrencyTotalRow.MYtdCreditFor = 0;
        baseCurrencyTotalRow.MYtdCredit = 0;

        //记录合计行数是否变化
        var totalRowCountChanged = false;
        if ($(".bi-total-item") && BalanceInput.totalRows) {
            totalRowCountChanged = $(".bi-total-item").length != BalanceInput.totalRows.length;
        }
        

        $(".bi-total-item").remove();
        if (!data || data.length == 0) {
            //更新合计行
            BalanceInput.updateTotalRow($(".bi-total-all"), baseCurrencyTotalRow);
            BalanceInput.divAutoSize();
            return;
        } else {
            //$(".bi-balance-total").show();
        }
        $(".bi-total-item").remove();

        for (var i = 0 ; i < data.length; i++) {
            var row = data[i];
            if (row.MCheckGroupValueID != "0") {
                continue;
            }

            var copyDom = $(".bi-total-all").clone();
            $(copyDom).removeClass("bi-total-all").addClass("bi-total-item");
            BalanceInput.updateTotalRow(copyDom, row);

            baseCurrencyTotalRow.MInitBalanceFor += $.isNumeric(row.MInitBalance) ? row.MInitBalance : 0;
            baseCurrencyTotalRow.MInitBalance += $.isNumeric(row.MInitBalance) ? row.MInitBalance : 0;
            baseCurrencyTotalRow.MYtdDebitFor += $.isNumeric(row.MYtdDebit) ? row.MYtdDebit : 0;
            baseCurrencyTotalRow.MYtdDebit += $.isNumeric(row.MYtdDebit) ? row.MYtdDebit : 0;
            baseCurrencyTotalRow.MYtdCreditFor += $.isNumeric(row.MYtdCredit) ? row.MYtdCredit : 0;
            baseCurrencyTotalRow.MYtdCredit += $.isNumeric(row.MYtdCredit) ? row.MYtdCredit : 0;
        }

        //更新合计行
        BalanceInput.updateTotalRow($(".bi-total-all"), baseCurrencyTotalRow);

        //合计行数变化时才刷新布局
        if (totalRowCountChanged) {
            BalanceInput.divAutoSize();
        }
    },
    //更新一行合计数据
    updateTotalRow: function (selecotr, row) {
        var currency = row.MCurrencyID;

        var isTotalRow = selecotr.hasClass("bi-total-all");

        var initOriMoney = $.isNumeric(row.MInitBalanceFor) ? row.MInitBalanceFor : 0.00;
        var initBaseMoney = $.isNumeric(row.MInitBalance) ? row.MInitBalance : 0.00;
        var ytdDebitOriMoney = $.isNumeric(row.MYtdDebitFor) ? row.MYtdDebitFor : 0.00;
        var ytdDebitBaseMoney = $.isNumeric(row.MYtdDebit) ? row.MYtdDebit : 0.00;
        var ytdCreditOriMoney = $.isNumeric(row.MYtdCreditFor) ? row.MYtdCreditFor : 0.00;
        var ytdCreditBaseMoeny = $.isNumeric(row.MYtdCredit) ? row.MYtdCredit : 0.00;

        $(selecotr).attr("currency", currency);

        var currencyDom = $(".bi-total-currency", selecotr).find("div");
        currencyDom.text(currency);
        if (!isTotalRow) {
            $(".bi-money-init-ori", selecotr).text(Megi.Math.toMoneyFormat(initOriMoney));
            $(".bi-money-ytddebit-ori", selecotr).text(Megi.Math.toMoneyFormat(ytdDebitOriMoney));
            $(".bi-money-ytdcredit-ori", selecotr).text(Megi.Math.toMoneyFormat(ytdCreditOriMoney));
        } else {
            $(".bi-money-init-ori", selecotr).text(Megi.Math.toMoneyFormat(""));
            $(".bi-money-ytddebit-ori", selecotr).text(Megi.Math.toMoneyFormat(""));
            $(".bi-money-ytdcredit-ori", selecotr).text(Megi.Math.toMoneyFormat(""));
        }
        $(".bi-money-init-base", selecotr).text(Megi.Math.toMoneyFormat(initBaseMoney));
        $(".bi-money-ytddebit-base", selecotr).text(Megi.Math.toMoneyFormat(ytdDebitBaseMoney));
        $(".bi-money-ytdcredit-base", selecotr).text(Megi.Math.toMoneyFormat(ytdCreditBaseMoeny));
        if (!isTotalRow) {
            $(".bi-total-all", ".bi-balance-total").before(selecotr);
        }
    },
    endGridRowEdit: function () {
        if (BalanceInput.editorRowIndex != null) {
            //结束编辑
            $("#gridAccountBalance").datagrid("endEdit", BalanceInput.editorRowIndex);
        }
    },
    beginGridRowEdit: function (rowIndex) {
        $("#gridAccountBalance").datagrid("beginEdit", rowIndex);

        BalanceInput.bindNumberboxKeyupEvent();

        $("#hideEditPage").val(true);
    },
    getGridColumns: function (accountId, callback) {
        BalanceInput.getAccountInfo(accountId, function (accountInfo) {

            BalanceInput.account = accountInfo;

            var columns = new Array();
            var gridId = "#gridAccountBalance";
            var isLeaf = BalanceInput.isLeafAccount();
            columns.push({
                title: "", width: 40, align: 'right', field: "MItemID", formatter: function (val, rec, rowIndex) {
                    if (rec.MCheckGroupValueID != "0" && !BalanceInput.forbitEdit && isLeaf) {
                        return '<a href="javascript:void(0)" class="m-icon-add-row" rowindex="' + rowIndex + '">&nbsp;</a>'
                    } else {
                        return "";
                    }

                }
            });

            //获取科目的核算维度
            var checkGroupModel = accountInfo.MCheckGroupModel;
            //核算维度和核算维度名称
            var checkTypeNameRelationList = accountInfo.MCheckTypeNameRelationList;

            if (checkTypeNameRelationList && checkTypeNameRelationList.length > 0) {
                for (var i = 0 ; i < checkTypeNameRelationList.length; i++) {
                    var checkTypeNameRelation = checkTypeNameRelationList[i];
                    var column = { title: checkTypeNameRelation.MName, field: checkTypeNameRelation.MValue1, width: 100 };
                    column.editor = BalanceInput.getColumnEditor(checkGroupModel, checkTypeNameRelation.MValue);

                    var isTotalColum = i == checkTypeNameRelationList.length - 1;

                    column.formatter = BalanceInput.getCoumnFormatter(checkTypeNameRelation.MValue, isTotalColum);
                    columns.push(column);
                }
            }

            columns = columns.concat(BalanceInput.getBaseGridColumns());

            var result = [columns];

            if (callback && $.isFunction(callback)) {
                callback(result);
            }
        });
    },
    getCheckTypeBaseData: function (checkTypeEnum) {
        var data = [];
        if (checkTypeEnum == "0") {
            data = BalanceInput.contactList;
        } else if (checkTypeEnum == "1") {
            data = BalanceInput.employeeList;
        } else if (checkTypeEnum == "2") {
            data = BalanceInput.merchandiseItemList;
        } else if (checkTypeEnum == "3") {
            data = BalanceInput.expenseItemList;
        } else if (checkTypeEnum == "4") {
            data = BalanceInput.paList;
        } else if (checkTypeEnum == "5") {
            data = BalanceInput.trackItem1Option;
        } else if (checkTypeEnum == "6") {
            data = BalanceInput.trackItem2Option;
        } else if (checkTypeEnum == "7") {
            data = BalanceInput.trackItem3Option;
        } else if (checkTypeEnum == "8") {
            data = BalanceInput.trackItem4Option;
        } else if (checkTypeEnum == "9") {
            data = BalanceInput.trackItem5Option;
        } else {
            data = BalanceInput.currencyList;
        }

        return data;
    },
    //获取核算维度个数据
    getColumnEditor: function (checkGroupModel, checkTypeEnum) {
        //联系人
        var editor;
        var required;
        var data;
        //var url = ""
        if (checkTypeEnum) {
            var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=0";

            data = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum);
        } else {
            data = BalanceInput.getBaseDataByCheckTypeEnum();
        }

        if (checkTypeEnum == "0") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MContactID);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, true, null, null, function (node, selector) {
                var parentNode = selector.tree('getParent', node.target);
                var row = $("#gridAccountBalance").datagrid("getRowByIndex", BalanceInput.editorRowIndex);

                if (row) {
                    row.MContactTypeFromBill = parentNode.id;
                    row.MContactType = parentNode.id;
                }
            }, function (selector) {
                BalanceInput.setContactShowRule(selector);
            }, checkTypeEnum);
        } else if (checkTypeEnum == "1") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MEmployeeID);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, false, "MIsActive", false, function (row) {
                var row = $("#gridAccountBalance").datagrid("getRowByIndex", BalanceInput.editorRowIndex);
                row["MContactTypeFromBill"] = "Employees";
            }, function (data) {
                BalanceInput.employeeList = data;
            }, checkTypeEnum);
        } else if (checkTypeEnum == "2") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MMerItemID);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, false, "MIsActive", false, null, function (data) {
                BalanceInput.merchandiseItemList = data;
            }, checkTypeEnum);
        } else if (checkTypeEnum == "3") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MExpItemID);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, true, null, null, null, null, checkTypeEnum);
        } else if (checkTypeEnum == "4") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MPaItemID);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, true, null, null, null, null, checkTypeEnum);
        } else if (checkTypeEnum == "5") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MTrackItem1);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, false, "MIsActive", false, null, function (data) {
                BalanceInput.trackItem1Option = data;
            }, checkTypeEnum);
        } else if (checkTypeEnum == "6") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MTrackItem2);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, false, "MIsActive", false, null, function (data) {
                BalanceInput.trackItem2Option = data;
            }, checkTypeEnum);
        } else if (checkTypeEnum == "7") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MTrackItem3);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, false, "MIsActive", false, null, function (data) {
                BalanceInput.trackItem3Option = data;
            }, checkTypeEnum);
        } else if (checkTypeEnum == "8") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MTrackItem4);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, false, "MIsActive", false, null, function (data) {
                BalanceInput.trackItem4Option = data;
            }, checkTypeEnum);
        } else if (checkTypeEnum == "9") {
            required = BalanceInput.checkTypeIsRequired(checkGroupModel.MTrackItem5);
            editor = BalanceInput.getEditorOjbect("id", "text", data, required, false, "MIsActive", false, null, function (data) {
                BalanceInput.trackItem5Option = data;
            }, checkTypeEnum);
        }

        return editor;
    },
    setContactShowRule: function (selector) {
        //销售单只能选到客户，采购单只能选到供应商 ， 收款单客户和其他，付款单客户，其他员工，员工和联系人只能选择一个
        var accountCode = BalanceInput.account ? BalanceInput.account.MCode : null;
        var tree = selector.combotree("tree");
        var customerRootNode = tree.tree("find", "Customer");
        var supplierRootNode = tree.tree("find", "Supplier");
        var otherRootNode = tree.tree("find", "Other");

        ////应收账款对应销售单
        //if (accountCode && (accountCode.indexOf("1122") == 0 || accountCode.indexOf("1221")==0)) {
        if (accountCode && (accountCode.indexOf("1122") == 0)) {
            BalanceInput.hideTreeNode(selector, supplierRootNode);
            if (accountCode.indexOf("1122") == 0) {
                BalanceInput.hideTreeNode(selector, otherRootNode);
            }
        } else if (accountCode && (accountCode.indexOf("2202") == 0)) {
            BalanceInput.hideTreeNode(selector, customerRootNode);
            if (accountCode.indexOf("2202") == 0) {
                BalanceInput.hideTreeNode(selector, otherRootNode);
            }
        } else if (accountCode && accountCode.indexOf("2203") == 0) {
            BalanceInput.hideTreeNode(selector, supplierRootNode);
        } else if (accountCode && accountCode.indexOf("1123") == 0) {
            BalanceInput.hideTreeNode(selector, customerRootNode);
        }

    },
    getCoumnFormatter: function (checkTypeEnum, isTotalColumn) {
        var formatter;
        if (checkTypeEnum == "0") {
            BalanceInput.contactList = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                val = val ? val : rec.MContactID;
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.contactList);
                return value;
            }
        } else if (checkTypeEnum == "1") {
            BalanceInput.employeeList = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                isTotalColumn = isTotalColumn && rec.MCheckGroupValueID == "0";
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.employeeList);
                return value;
            }

        } else if (checkTypeEnum == "2") {
            BalanceInput.merchandiseItemList = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {

                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.merchandiseItemList);
                return value;
            }

        } else if (checkTypeEnum == "3") {
            BalanceInput.expenseItemList = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.expenseItemList);
                return value;
            }

        } else if (checkTypeEnum == "4") {
            BalanceInput.paList = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.paList);
                return value;
            }
        } else if (checkTypeEnum == "5") {
            BalanceInput.trackItem1Option = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.trackItem1Option);
                return value;
            }
        } else if (checkTypeEnum == "6") {
            BalanceInput.trackItem2Option = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                isTotalColumn = isTotalColumn && rec.MCheckGroupValueID == "0";
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.trackItem2Option);
                return value;
            }
        } else if (checkTypeEnum == "7") {
            BalanceInput.trackItem3Option = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.trackItem3Option);
                return value;
            }
        } else if (checkTypeEnum == "8") {
            BalanceInput.trackItem4Option = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.trackItem4Option);
                return value;
            }
        } else if (checkTypeEnum == "9") {
            BalanceInput.trackItem5Option = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.trackItem5Option);
                return value;
            }
        } else if (checkTypeEnum == "10") {
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("id", val, BalanceInput.billType, "value");
                return value;
            }
        } else if (checkTypeEnum == "11") {
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("MItemID", val, BalanceInput.bankList, "MBankName");
                return value;
            }
        } else {
            formatter = function (val, rec, rowIndex) {
                var value = BalanceInput.getFormatterResult("MCurrencyID", val, BalanceInput.currencyList);
                return value;
            }
        }

        return formatter;
    },
    getFormatterResult: function (macthKey, macthValue, data, textKey) {

        var result = "";
        if (data && data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                var tempData = data[i];
                if (macthValue && tempData[macthKey] == macthValue) {
                    if (textKey) {
                        result = tempData[textKey];
                    } else {
                        result = tempData.MName ? tempData.MName : tempData.text ? tempData.text : tempData.MNumber;
                    }
                    break;
                }

                if (typeof tempData.children != "undefined") {
                    result = BalanceInput.getFormatterResult(macthKey, macthValue, tempData.children, textKey);

                    if (result) {
                        break;
                    }
                }
            }
        }
        return result;

    },
    //检查维度是否必录
    checkTypeIsRequired: function (checkTypeStatus, checkTypeEnum) {
        if (checkTypeStatus == "2") {
            return true;
        }
        return false;
    },
    getEditorOjbect: function (valueField, textField, data,
        required, isTree, hideItemKey, hideItemValue, changeEvent, loadSuccessEvent, checkTypeEnum) {

        var editor = {};

        var addOptions = BalanceInput.getAddOptions(checkTypeEnum);
        if (addOptions == null) {
            editor.type = "combobox";
            editor.options = {
                valueField: valueField,
                textField: textField,
                required: required,
                hideItemKey: "MIsActive",
                hideItemValue: false,
                height: BalanceInput.combobxDefaultHeight,
                data: data,
                onSelect: function (data) {
                    if (changeEvent) { changeEvent(); }

                },
                onLoadSuccess: function () {
                    if (loadSuccessEvent) { loadSuccessEvent() };
                }
            }

            if (hideItemKey != undefined && hideItemValue != undefined) {
                editor.options.hideItemKey = hideItemKey;
                editor.options.hideItemValue = hideItemValue;
            }
        } else if (typeof addOptions == "string") {
            editor.type = 'addCombobox';
            editor.options = {};
            editor.options.type = addOptions;
            //跟踪项的新增
            if ($.isNumeric(checkTypeEnum) && checkTypeEnum > 4) {
                var addTrackOptionsUrl = BalanceInput.getAddTrackOptionUrl(checkTypeEnum);
                editor.options.addOptions = {
                    //是否有基础资料编辑权限
                    hasPermission: true,
                    isReload: false,
                    url: addTrackOptionsUrl,
                    //关闭后的回调函数
                    callback: function () {
                        var field = BalanceInput.getGridFieldByCheckTypeEnum(checkTypeEnum);
                        var newestData = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
                        if (newestData) {
                            var editor = BalanceInput.getEditor(field);
                            $(editor.target).combobox("loadData", newestData);
                        }
                    }
                }
            } else {
                editor.options.addOptions = {
                    //是否有基础资料编辑权限
                    hasPermission: true,
                    isReload: false,
                    //关闭后的回调函数
                    callback: function () {
                        var field = BalanceInput.getGridFieldByCheckTypeEnum(checkTypeEnum);
                        var newestData = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
                        if (newestData) {
                            var editor = BalanceInput.getEditor(field);
                            $(editor.target).combobox("loadData", newestData);
                        }

                    }
                }
            }

            editor.options.dataOptions = {
                valueField: valueField,
                textField: textField,
                required: required,
                hideItemKey: hideItemKey,
                hideItemValue: hideItemValue,
                height: BalanceInput.combobxDefaultHeight,
                url: "/GL/GLCheckType/GetCheckTreeTypeDataByType?type=" + checkTypeEnum,
                //data: data,
                onSelect: function (data) {
                    if (changeEvent) { changeEvent(); }

                },
                onLoadSuccess: function (data) {
                    BalanceInput.setBaseData(checkTypeEnum, data);

                    if (loadSuccessEvent) { loadSuccessEvent(data) };


                }
            };
        } else if (isTree && typeof addOptions == "object") {
            editor.type = "addCombotree";
            editor.options = {};
            editor.options.addOptions = addOptions;
            editor.options.dataOptions = {
                valueField: valueField,
                textField: textField,
                required: required,
                height: BalanceInput.combobxDefaultHeight,
                url: "/GL/GLCheckType/GetCheckTreeTypeDataByType?type=" + checkTypeEnum,
                onBeforeSelect: function (node) {
                    //不是叶子节点不允许选择
                    if (node == null || !$(this).tree('isLeaf', node.target)) {
                        return false;
                    }

                    //联系人标题不允许选择
                    if (node.id == "Customer" || node.id == "Supplier" || node.id == "Other") {
                        return false;
                    }
                },
                onSelect: function (node) {
                    if (changeEvent) { changeEvent(node, $(this)); }

                },
                onLoadSuccess: function (selector) {
                    var roots = selector.tree("getRoots");
                    var dataList = new Array();
                    if (roots && roots.length > 0) {
                        for (var i = 0; i < roots.length; i++) {
                            var data = $(tree).tree("getData", roots[i].target);

                            dataList.push(data);
                        }
                    }

                    BalanceInput.setBaseData(checkTypeEnum, dataList);
                },
                onShowPanel: function () {
                    //IE 环境下，点击表格，就会立马触发弹框，但是此时数据尚未加载出来。
                    //加个判断，如果没有子项，则先隐藏。
                    if ($(this).combotree("tree").tree("getChildren").length < 1) {
                        $(this).combo('resizeByItem');
                    }
                    if (loadSuccessEvent) {
                        loadSuccessEvent($(this));
                    }

                }
            }

        }
        return editor;
    },
    getBaseGridColumns: function () {
        var columns = new Array();
        var combobxHeight = 35;

        if (BalanceInput.isCurrenctAccount()) {
            columns.push({
                title: HtmlLang.Write(LangModule.Acct, "BillType", "单据类型"), field: 'MBillType', width: 100, hidden: true, editor: {
                    type: 'combobox',
                    options: {
                        valueField: 'id',
                        textField: 'value',
                        required: false,
                        height: BalanceInput.combobxDefaultHeight,
                        data: BalanceInput.getBillTypeByAccount(),
                        onSelect: function (record) {
                            var bankEditor = BalanceInput.getEditor("MBankID");
                            //只有收付款单才有银行账号
                            if (record.id == "Receive_Sale" || record.id == "Pay_Purchase") {
                                $(bankEditor.target).combobox("enable");

                            } else {
                                $(bankEditor.target).combobox("setValue", "");
                                $(bankEditor.target).combobox("disable");
                            }

                            //如果是其他应付科目，选择费用报销单的时候，员工和联系人只能选择一个，商品项目和费用项目只能选择一个
                            if (BalanceInput.account.MCode.indexOf("2241") == 0) {
                                var expenseItemEditor = BalanceInput.getEditor("MExpItemID");
                                var employeeEditor = BalanceInput.getEditor("MEmployeeID");

                                var contactEditor = BalanceInput.getEditor("MContactID");
                                var merItemEditor = BalanceInput.getEditor("MMerItemID");

                                //选择员工和费用项目
                                if (record.id == "Expense_Claims") {
                                    if (contactEditor) {
                                        $(contactEditor.target).combotree("setValue", "");
                                        $(contactEditor.target).combotree("disable");
                                    }

                                    if (merItemEditor) {
                                        $(merItemEditor.target).combobox("setValue", "");
                                        $(merItemEditor.target).combobox("disable");
                                    }

                                    if (employeeEditor) {
                                        $(employeeEditor.target).combobox("enable");
                                    }

                                    if (expenseItemEditor) {
                                        $(expenseItemEditor.target).combotree("enable");
                                    }
                                } else if (record.id == "Invoice_Purchase") {
                                    if (contactEditor) {
                                        $(contactEditor.target).combotree("enable");
                                    }

                                    if (merItemEditor) {
                                        $(merItemEditor.target).combobox("enable");
                                    }

                                    if (employeeEditor) {
                                        $(employeeEditor.target).combobox("setValue", "")
                                        $(employeeEditor.target).combobox("disable");
                                    }

                                    if (expenseItemEditor) {
                                        $(expenseItemEditor.target).combotree("setValue", "")
                                        $(expenseItemEditor.target).combotree("disable");
                                    }
                                }
                            }
                        }
                    }
                },
                formatter: BalanceInput.getCoumnFormatter("10")
            });

            columns.push({
                title: HtmlLang.Write(LangModule.Acct, "BankAccount", "银行账号"), field: 'MBankID', hidden: true, width: 100, editor: {
                    type: 'combobox',
                    options: {
                        valueField: 'MItemID',
                        textField: 'MBankName',
                        height: BalanceInput.combobxDefaultHeight,
                        data: BalanceInput.bankList,
                        onSelect: function (record) {
                            var currencyEditor = BalanceInput.getEditor("MCurrencyID");
                            var selectedCy = $(currencyEditor.target).combobox("getValue");

                            var bankCurrency = record.MCyID;

                            //如果选择了币别框，并且币别框值不等于银行的值，提示用户
                            if (selectedCy && selectedCy != bankCurrency) {
                                var tips = HtmlLang.Write(LangModule.Acct, "BankCyNotEqualCy", "所选银行的币别和所选择的余额币别不一致，不能进行保存!");
                                $.mDialog.alert(tips);

                                $(this).combobox("setValue", "");
                            } else if (!selectedCy) {
                                //如果没有选择币别，将银行的币别赋给币别框
                                $(currencyEditor.target).combobox("setValue", bankCurrency);
                            }
                        }
                    }
                },
                formatter: BalanceInput.getCoumnFormatter("11")
            });
            //联系人类型
            columns.push({ title: "", width: 150, field: "MBillID", hidden: true });
            columns.push({ title: "", width: 150, field: "MContactTypeFromBill", hidden: true });
        }
        columns.push({ title: "", width: 150, field: "MContactTypeFromInitBalance", hidden: true });
        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "Currency", "币别"), field: 'MCurrencyID', width: 100, editor: {
                type: 'combobox',
                options: {
                    valueField: 'MCurrencyID',
                    textField: 'MName',
                    required: true,
                    height: BalanceInput.combobxDefaultHeight,
                    data: BalanceInput.currencyList,
                    onSelect: function (data) {
                        if (BalanceInput.checkCurrencyIsUsed(data.MCurrencyID)) {
                            var tips = HtmlLang.Write(LangModule.Acct, "CurrencyIsUsed", "币别已经使用，请选择其他币别！");
                            $.mDialog.alert(tips);

                            $(this).combobox("setValue", "");
                        }
                        BalanceInput.disabledBaseCurrencyInput(data.MCurrencyID);
                    },
                    onChange: function (newValue, oldValue) {

                        var exchange = BalanceInput.getExchangeRate(newValue);

                        var initOriEditor = BalanceInput.getEditor("MInitBalanceFor");
                        var initOriValue = $(initOriEditor.target).numberbox("getValue");
                        if (initOriValue) {
                            var initValue = initOriValue * exchange;
                            var initBaseEditor = BalanceInput.getEditor("MInitBalance");
                            $(initBaseEditor.target).numberbox("setValue", initValue);
                        }


                        var debitOriEditor = BalanceInput.getEditor("MYtdDebitFor");
                        var debitOriValue = $(debitOriEditor.target).numberbox("getValue");
                        if (debitOriValue) {
                            var debitValue = debitOriValue * exchange;
                            var debitBaseEditor = BalanceInput.getEditor("MYtdDebit");
                            $(debitBaseEditor.target).numberbox("setValue", debitValue);
                        }


                        var creditOriEditor = BalanceInput.getEditor("MYtdCreditFor");
                        var creditOriValue = $(creditOriEditor.target).numberbox("getValue");
                        if (creditOriValue) {
                            var creditValue = creditOriValue * exchange;
                            var creditBaseEditor = BalanceInput.getEditor("MYtdCredit");
                            $(creditBaseEditor.target).numberbox("setValue", creditValue);
                        }

                    }
                }
            },
            formatter: BalanceInput.getCoumnFormatter()
        });
        columns.push({ title: "", width: 150, align: 'right', field: "MCheckGroupValueID", hidden: true });
        columns.push({ title: "", width: 150, align: 'right', field: "MAccountID", hidden: true });
        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "InitialBalanceOriginalCurrency", "期初余额原币"), width: 150, align: 'right', field: "MInitBalanceFor", editor: {
                type: 'numberbox',
                options: {
                    value: 0,
                    precision: 2,
                    required: false
                }
            }, formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        });
        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "InitialBalanceBaseCurrency", "期初余额本位币"), width: 150, align: 'right', field: "MInitBalance", editor: {
                type: 'numberbox',
                options: {
                    value: 0,
                    precision: 2,
                    required: false,
                }
            }, formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        });

        var isHideYtdField = BalanceInput.glMonth === "1";


        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "CumulativeDebitThisYearOriginalCurrency", "本年累计借方原币"), align: 'right', width: 150, hidden: isHideYtdField, field: "MYtdDebitFor", editor: {
                type: 'numberbox',
                options: {
                    value: 0,
                    precision: 2
                }
            }, formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        });
        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "CumulativeDebitThisYearBaseCurrency", "本年累计借方本位币"), align: 'right', width: 150, hidden: isHideYtdField, field: "MYtdDebit", editor: {
                type: 'numberbox',
                options: {
                    value: 0,
                    precision: 2
                }
            }, formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        });
        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "CumulativeCreditThisYearOriginalCurrency", "本年累计贷方原币"), align: 'right', width: 150, hidden: isHideYtdField, field: "MYtdCreditFor", editor: {
                type: 'numberbox',
                options: {
                    value: 0,
                    precision: 2
                }
            }, formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        });
        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "CumulativeCreditThisYearBaseCurrency", "本年累计贷方本位币"), align: 'right', width: 150, hidden: isHideYtdField, field: "MYtdCredit", editor: {
                type: 'numberbox',
                options: {
                    value: 0,
                    precision: 2
                }
            }, formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        });


        var isLeaf = BalanceInput.isLeafAccount();
        columns.push({
            title: HtmlLang.Write(LangModule.Acct, "Operation", "操作"), align: 'center', width: 100, field: "Op", formatter: function (val, rec, rowIndex) {
                if (rec.MCheckGroupValueID != "0" && !BalanceInput.forbitEdit && isLeaf) {
                    return "<div class='list-item-action'><a href='javascript:void(0);' row-index='" + rowIndex + "' class='list-item-del'></a></div>";
                } else {
                    return "";
                }
            }
        });

        return columns;
    },
    //是否往来科目
    isCurrenctAccount: function () {
        var code = BalanceInput.account.MCode;

        if (code.indexOf("2203") == 0 || code.indexOf("1221") == 0 ||
            code.indexOf("1123") == 0 || code.indexOf("2241") == 0 || code.indexOf("1122") == 0 || code.indexOf("2202") == 0) {
            return true;
        }

        return false;
    },
    getEditor: function (field) {
        var editor = $("#gridAccountBalance").datagrid('getEditor', { index: BalanceInput.editorRowIndex, field: field });

        return editor;
    },
    checkCurrencyIsUsed: function (currencyId) {
        //先判断是否其他行是否选择了币别
        var rows = $("#gridAccountBalance").datagrid("getRows");

        var contactId = BalanceInput.getColumnEditorSelectId("MContactID");
        var employeeId = BalanceInput.getColumnEditorSelectId("MEmployeeID");
        var expenseId = BalanceInput.getColumnEditorSelectId("MExpItemID");
        var merItemId = BalanceInput.getColumnEditorSelectId("MMerItemID");
        var paItemId = BalanceInput.getColumnEditorSelectId("MPaItemID");
        var trackItem1Id = BalanceInput.getColumnEditorSelectId("MTrackItem1");
        var trackItem2Id = BalanceInput.getColumnEditorSelectId("MTrackItem2");
        var trackItem3Id = BalanceInput.getColumnEditorSelectId("MTrackItem3");
        var trackItem4Id = BalanceInput.getColumnEditorSelectId("MTrackItem4");
        var trackItem5Id = BalanceInput.getColumnEditorSelectId("MTrackItem5");

        for (var i = 0; i < rows.length; i++) {
            //等于当前行，就忽略
            if (i == BalanceInput.editorRowIndex) {
                continue;
            }

            var row = rows[i];
            if (row.MCheckGroupValueID != "0" && row.MCurrencyID == currencyId &&
                row.MContactID == contactId && row.MEmployeeID == employeeId && row.MExpItemID == expenseId &&
                row.MMerItemID == merItemId && row.MPaItemID == paItemId &&
                row.MTrackItem1 == trackItem1Id && row.MTrackItem2 == trackItem2Id && row.MTrackItem3 == trackItem3Id &&
                row.MTrackItem4 == trackItem4Id && row.MTrackItem5 == trackItem5Id) {
                return true;
            }
        }
        return false;
    },
    getColumnEditorSelectId: function (field) {
        var editor = $("#gridAccountBalance").datagrid('getEditor', { index: BalanceInput.editorRowIndex, field: field });
        var id = null;
        if (editor) {
            id = $(editor.target).combo("getValue");
        }

        return id;
    },
    getAccountInfo: function (accountId, callback) {
        var url = '/BD/BDAccount/GetAccount?MItemID=' + accountId;
        mAjax.post(url, {}, function (data) {
            if (data) {
                if (callback && $.isFunction(callback)) {
                    callback(data);
                }
            }
        }, false, false, true);
    },
    reloadUI: function (gridId) {
        var data = Megi.grid(gridId, "getData");
        BalanceInput.isAutoCreateInitDocument(data);
        if (!data || !data.length == 0 || data.rows.length == 0) {
            BalanceInput.insertEmptyRow(gridId, 4);
        }
    },
    getBaseDataByCheckTypeEnum: function (checkTypeEnum, forceFresh) {
        if (checkTypeEnum == "0") {
            if (BalanceInput.contactList == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=0";
                BalanceInput.getBaseData(url, function (data) {
                    if (data) {
                        BalanceInput.contactList = data.MDataList;
                    }

                });
            }
            return BalanceInput.contactList;
        } else if (checkTypeEnum == "1") {
            if (BalanceInput.employeeList == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=1";
                BalanceInput.getBaseData(url, function (data) {
                    if (data) {
                        BalanceInput.employeeList = data.MDataList;
                    }
                });
            }
            return BalanceInput.employeeList;

        } else if (checkTypeEnum == "2") {
            if (BalanceInput.merchandiseItemList == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=2";
                BalanceInput.getBaseData(url, function (data) {
                    BalanceInput.merchandiseItemList = data.MDataList;
                });
            }

            return BalanceInput.merchandiseItemList;

        } else if (checkTypeEnum == "3") {
            if (BalanceInput.expenseItemList == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=3";
                BalanceInput.getBaseData(url, function (data) {
                    if (data) {
                        BalanceInput.expenseItemList = data.MDataList;
                    }

                });
            }
            return BalanceInput.expenseItemList;

        } else if (checkTypeEnum == "4") {

            if (BalanceInput.paList == null || forceFresh) {
                var url = "/PA/PayItem/GetSalaryItemTreeList";
                BalanceInput.getBaseData(url, function (data) {
                    //对工资项目进行特殊处理
                    var list = [];

                    for (var i = 0; i < data.length ; i++) {

                        if (!data[i].children || data[i].children.length == 0) {
                            list.push(data[i]);
                        }
                        else {
                            for (var j = 0; j < data[i].children.length; j++) {

                                data[i].children[j].text = data[i].text + "-" + data[i].children[j].text;
                                list.push(data[i].children[j]);
                            }
                        }
                    }
                    BalanceInput.paList = list;
                });
            }
            return BalanceInput.paList;
        } else if (checkTypeEnum == "5") {
            if (BalanceInput.trackItem1Option == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=5&includeDisable=true";
                BalanceInput.getBaseData(url, function (data) {
                    BalanceInput.trackItem1Option = data.MDataList;
                });
            }

            return BalanceInput.trackItem1Option;
        } else if (checkTypeEnum == "6") {
            if (BalanceInput.trackItem2Option == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=6&includeDisable=true";
                BalanceInput.getBaseData(url, function (data) {
                    BalanceInput.trackItem2Option = data.MDataList;
                });
            }
            return BalanceInput.trackItem2Option;
        } else if (checkTypeEnum == "7") {
            if (BalanceInput.trackItem3Option == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=7&includeDisable=true";
                BalanceInput.getBaseData(url, function (data) {
                    BalanceInput.trackItem3Option = data.MDataList;
                });
            }
            return BalanceInput.trackItem3Option;
        } else if (checkTypeEnum == "8") {
            if (BalanceInput.trackItem4Option == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=8&includeDisable=true";
                BalanceInput.getBaseData(url, function (data) {
                    BalanceInput.trackItem4Option = data.MDataList;
                });
            }
            return BalanceInput.trackItem4Option;
        } else if (checkTypeEnum == "9") {
            if (BalanceInput.trackItem5Option == null || forceFresh) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=9&includeDisable=true";
                BalanceInput.getBaseData(url, function (data) {
                    BalanceInput.trackItem5Option = data.MDataList;
                });
            }
            return BalanceInput.trackItem5Option;
        } else {
            return BalanceInput.currencyList;
        }
    },
    initBaseData: function (callback) {

        //银行信息
        if (BalanceInput.bankList == null) {
            var url = "/BD/BDBank/GetBDBankAccountViewList";
            BalanceInput.getBaseData(url, function (data) {
                BalanceInput.bankList = data;
            });
        }

        if (BalanceInput.currencyList == null) {
            var url = "/BD/Currency/GetBDCurrencyList?isIncludeBase=true";
            BalanceInput.getBaseData(url, function (data) {
                BalanceInput.currencyList = data;
            });
        }

        //跟踪项的获取
        BalanceInput.getTrackList();

        if (callback && $.isFunction(callback)) {
            callback();
        }
    },

    getTrackList: function (forceRefresh) {
        if (BalanceInput.trackList == null || forceRefresh) {
            var url = "/BD/Tracking/GetTrackBasicInfo";
            BalanceInput.getBaseData(url, function (data) {
                BalanceInput.trackList = data;
                //进行拆分
                if (data && data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var tempTrack = data[i];
                        var tempTrackOption = tempTrack.MChildren;

                        //转换一下，专程id，text形式
                        var tempArray = new Array();
                        if (tempTrackOption && tempTrackOption.length > 0) {

                            for (var j = 0; j < tempTrackOption.length; j++) {
                                var temp = {};

                                temp.id = tempTrackOption[j].MValue;
                                temp.text = tempTrackOption[j].MName;
                                temp.MIsActive = tempTrackOption[j].MValue1 === "1";

                                tempArray.push(temp);
                            }
                        }

                        switch (i) {
                            case 0:
                                BalanceInput.trackItem1Option = tempArray;
                                break;
                            case 1:
                                BalanceInput.trackItem2Option = tempArray;
                                break;
                            case 2:
                                BalanceInput.trackItem3Option = tempArray;
                                break;
                            case 3:
                                BalanceInput.trackItem4Option = tempArray;
                                break;
                            case 4:
                                BalanceInput.trackItem5Option = tempArray;
                                break;
                        }
                    }
                }

            });
        }
    },
    getBaseData: function (url, callback) {
        $.mAjax.post(url, null, function (data) {
            if (data && callback && $.isFunction(callback)) {
                callback(data);
            }
        }, false, true, false);
    },
    insertEmptyRow: function (gridId, rowCount, rowIndex) {
        var isLeafNode = BalanceInput.isLeafAccount();
        var rows = $(gridId).datagrid("getRows");
        for (var i = 0; i < rowCount; i++) {
            var emptyBalanceObject = BalanceInput.getEmptyData();

            //叶子节点与核算维度数据，需要插入在汇总行之前
            if (isLeafNode) {
                var data = $(gridId).datagrid("getData").rows;
                var summaryData = BalanceInput.getSummaryData(data);
                var totalRows = $(gridId).datagrid("getRows").length;
                var insertIndex = (totalRows - summaryData.length);

                $(gridId).datagrid("insertRow", { index: insertIndex, row: emptyBalanceObject });
            } else {
                $(gridId).datagrid("insertRow", { row: emptyBalanceObject });
            }
        }
        //如果是最后一行新增空行，滚到到最后一行,rowIndex从0开始
        if (rowIndex && rowIndex == rows.length - 2) {
            $(gridId).datagrid("scrollTo", rowIndex + 1);
        }
    },
    getEmptyData: function () {
        var emptyBalanceObject = {};

        emptyBalanceObject.MItemID = "";
        emptyBalanceObject.MCheckGroupValueID = "";
        emptyBalanceObject.MAccountID = BalanceInput.account.MItemID;
        emptyBalanceObject.MCurrencyID = "";
        emptyBalanceObject.MInitBalanceFor = "";
        emptyBalanceObject.MYtdDebitFor = "";
        emptyBalanceObject.MYtdDebit = "";
        emptyBalanceObject.MYtdCreditFor = "";
        emptyBalanceObject.MYtdCredit = "";

        return emptyBalanceObject;
    },
    initSummaryData: function (updateRowData, isDelete) {

        var rows = $("#gridAccountBalance").datagrid("getRows");

        for (var i = 0 ; i < rows.length; i++) {
            var row = rows[i];
            //表示汇总值
            if (row.MCheckGroupValueID == "0" && row.MCurrencyID == updateRowData.MCurrencyID) {
                if (isDelete) {
                    row.MInitBalanceFor += updateRowData.MInitBalanceFor * -1;
                    row.MInitBalance += updateRowData.MInitBalance * -1;
                    row.MYtdCredit += updateRowData.MYtdCredit * -1;
                    row.MYtdCreditFor += updateRowData.MYtdCreditFor * -1;
                    row.MYtdDebit += updateRowData.MYtdDebit * -1;
                    row.MYtdDebitFor += updateRowData.MYtdDebitFor * -1;
                }

                if (row.MInitBalanceFor == 0 && row.MInitBalance == 0 && row.MYtdCredit == 0
                    && row.MYtdCreditFor == 0 && row.MYtdDebit == 0 && row.MYtdDebitFor == 0) {
                    //删除改行
                    $("#gridAccountBalance").datagrid("deleteRow", i);
                } else {
                    $("#gridAccountBalance").datagrid("updateRow", {
                        index: i,
                        row: row
                    });
                }
            }
        }
    },
    deleteBalance: function (rowIndex) {

        if (BalanceInput.forbitEdit) {
            var tips = HtmlLang.Write(LangModule.Acct, "CanInputInitBalance", "不允许录入期初余额，科目已完成余额初始化或者没有编辑权限");
            $.mDialog.message(tips);
            return false;
        }

        var row = $("#gridAccountBalance").datagrid("getRowByIndex", +rowIndex);

        //不合法的行直接删除
        if (!BalanceInput.isValidateRow(row)) {
            BalanceInput.requestDeleteData(row, rowIndex);
        } else {
            var tips = HtmlLang.Write(LangModule.Acct, "ConfirmDeleteInitBalance", "是否删除该条余额记录?");
            $.mDialog.confirm(tips, function () {
                BalanceInput.requestDeleteData(row, rowIndex);
            });
        }

    },
    requestDeleteData: function (row, rowIndex) {
        if (row.MCheckGroupValueID == "") {
            $("#gridAccountBalance").datagrid("deleteRow", +rowIndex);
            BalanceInput.setTotalRowArray();
            BalanceInput.refreshTotalUI();

            BalanceInput.initGridRow();

            BalanceInput.editorRowIndex -= 1;

        } else {
            //请求后台删除数据
            var url = "/BD/BDAccountBalance/DeleteInitBalance";
            $.mAjax.submit(url, { initBalanceId: row.MItemID }, function (data) {
                if (data && data.Success) {
                    $("#gridAccountBalance").datagrid("deleteRow", +rowIndex);
                    BalanceInput.initSummaryData(row, true);
                    BalanceInput.initGridRow();

                    BalanceInput.setTotalRowArray();
                    BalanceInput.refreshTotalUI();

                    BalanceInput.editorRowIndex -= 1;
                } else {
                    var tips = data.Message ? data.Message : HtmlLang.Write(LangModule.Acct, "DeleteInitBalanceFail", "科目期初余额删除失败！");

                    $.mDialog.alert(tips);
                }
            });
        }
    },
    saveData: function (callback, ignorePrompt) {
        if (BalanceInput.forbitEdit) {
            var tips = HtmlLang.Write(LangModule.Acct, "CanInputInitBalance", "不允许录入期初余额，科目已完成余额初始化或者没有编辑权限");
            $.mDialog.message(tips);
            return false;
        }

        if (!BalanceInput.validateEditRow()) {
            return false;
        }

        if (BalanceInput.editorRowIndex != null && !BalanceInput.checkAmountIsValid()) {
            var tips = HtmlLang.Write(LangModule.Acct, "AmountNotEqual", "录入了原币金额没有录入本位币金额或者录入了本位币金额，没有原币金额，确定要结束编辑?");

            $.mDialog.confirm(tips, function () {
                BalanceInput.requestSaveData(callback, ignorePrompt);
            });
        } else {
            BalanceInput.requestSaveData(callback, ignorePrompt);
        }

    },
    requestSaveData: function (callback, ignorePrompt) {
        //结束编辑
        BalanceInput.endGridRowEdit();
        //组合数据模型
        var balanceRows = $("#gridAccountBalance").datagrid("getRows");

        var balanceList = new Array();
        for (var i = 0 ; i < balanceRows.length; i++) {
            var balanceRow = balanceRows[i];
            //验证数据是否要提交
            if (!BalanceInput.isValidateRow(balanceRow)) {
                continue;
            }

            //遍历做验证
            //获取科目的核算维度
            var checkGroupModel = BalanceInput.account.MCheckGroupModel;
            if (!checkGroupModel) {
                balanceList.push(balanceRow);
                continue;
            }
            //其他应付款的校验，其他应收款
            //2241其他应付款的数据校验
            if (BalanceInput.account.MCode.indexOf("2241") == 0 || BalanceInput.account.MCode.indexOf("1221") == 0) {
                var expenseId = balanceRow["MExpItemID"];
                var employeeId = balanceRow["MEmployeeID"];

                var contactId = balanceRow["MContactID"];
                var merItemId = balanceRow["MMerItemID"];
                var selectAccount = BalanceInput.getSelectedAccount();
                var tips = "";
                if (contactId && employeeId) {
                    tips += HtmlLang.Write(LangModule.Acct, "CanNotSelectEmpAndContact", "科目{0}不能同时选择联系人和员工").replace("{0}", selectAccount.text) + "</br>";

                }

                if (expenseId && merItemId) {
                    tips += HtmlLang.Write(LangModule.Acct, "CanNotSelectExpenseAndItem", "科目{0}不能同时选择费用项目和商品项目").replace("{0}", selectAccount.text) + "</br>";
                }

                if (contactId && expenseId) {
                    tips += HtmlLang.Write(LangModule.Acct, "CanNotSelectExpenseItemAndContact", "科目{0}联系人不能和费用项目同时选择！").replace("{0}", selectAccount.text) + "</br>";

                }

                if (employeeId && merItemId) {
                    tips += HtmlLang.Write(LangModule.Acct, "CanNotSelectItemAndEmployee", "科目{0}员工不能和商品项目同时选择！").replace("{0}", selectAccount.text) + "</br>";

                }
                if (tips) {
                    tips = "<div>" + tips + "</div>";
                    $.mDialog.alert(tips);
                    return;
                }


            }

            if (BalanceInput.account.MCreateInitBill) {
                BalanceInput.getInitBillModel(balanceRow);
            }



            //核算维度和核算维度名称

            var checkGroupValueModel = {};

            for (var checkTypeFeild in checkGroupModel) {

                if (checkTypeFeild == "MItemID") {
                    continue;
                }

                //科目某个核算维度的状态
                var checkTypeStatus = checkGroupModel[checkTypeFeild];
                //科目期初余额中核算维度的选中的具体值
                var balanceCheckTypeValue = balanceRow[checkTypeFeild];
                //余额没有这个字段,不校验
                if (balanceCheckTypeValue == undefined) {
                    continue;
                }

                var required = BalanceInput.checkTypeIsRequired(checkTypeStatus);

                if (balanceCheckTypeValue == "" && required) {
                    var fieldName = checkTypeFeild.replace("ID", "");
                    var tips = HtmlLang.Write(LangModule.Acct, "CheckType" + fieldName, fieldName) +
                        HtmlLang.Write(LangModule.Acct, "IsRequiedCheckType", "是必填的核算维度，请选择一个值！");
                    $.mDialog.alert(tips);

                    return;
                }

                checkGroupValueModel[checkTypeFeild] = balanceRow[checkTypeFeild];

                //如果是工资项核算维度字段，设置父级项目ID
                if (checkTypeFeild == "MPaItemID") {
                    var paItems = BalanceInput.paList.filter(function (item) { return item.id == balanceRow[checkTypeFeild] });
                    if (paItems.length > 0 && paItems[0]._parentId != '0') {
                        checkGroupValueModel["MPaItemGroupID"] = paItems[0]._parentId;
                    }
                }
            }

            balanceRow.MCheckGroupValueModel = checkGroupValueModel;
            balanceRow.MAccountCode = BalanceInput.account.MCode;
            balanceRow.MExchangeRate = BalanceInput.getExceptExchangeRate(balanceRow.MCurrencyID);
            balanceList.push(balanceRow);
        }

        if (!balanceList || balanceList.length == 0) {

            if (ignorePrompt) {
                if (callback && $.isFunction(callback)) {
                    callback();
                }
                return;
            } else {
                var tips = HtmlLang.Write(LangModule.Acct, "NoInitBalanceData", "没有需要保存的数据！");
                $.mDialog.alert(tips);

                var selectNode = BalanceInput.getSelectedAccount();
                BalanceInput.changeAccount(selectNode);
                $("#hideEditPage").val(false);
                return;
            }


        }

        var url = "/BD/BDAccountBalance/SaveInitBalance";
        $.mAjax.submit(url, { initBalanceList: balanceList }, function (data) {
            if (data && data.Success) {
                var hasCallBack = callback && $.isFunction(callback);
                if (!hasCallBack) {
                    var tips = HtmlLang.Write(LangModule.Acct, "InitBalanceSaveSuccessful", "科目期初余额保存成功");
                    $.mDialog.message(tips);
                }

                $("#hideEditPage").val(false);

                var selectedNode = BalanceInput.getSelectedAccount();

                BalanceInput.changeAccount(selectedNode, true);

                if (hasCallBack) {
                    callback();
                }
            } else {
                var tips = HtmlLang.Write(LangModule.Acct, "InitBalanceSaveFail", "科目期初余额保存失败");

                if (data && data.MessageList && data.MessageList.length > 0) {

                    var length = data.MessageList.length;

                    tips += "</br>"

                    for (var i = 0 ; i < length; i++) {
                        if (data.MessageList[i]) {
                            tips += data.MessageList[i] + "</br>"
                        }
                    }
                }

                $.mDialog.alert("<div>" + tips + "</div>");
            }
        });
    },
    getInitBillModel: function (balance) {
        var selectNode = BalanceInput.getSelectedAccount();

        if (!BalanceInput.isCurrenctAccount()) {
            return null;
        }

        if (balance.MInitBalanceFor <= 0 || balance.MInitBalance <= 0) {
            balance.MBillType = "";
            return;
        }

        //当有billid时，说明是编辑过来的，不在更改

        var accountCode = selectNode.MCode;

        if (accountCode.indexOf("1122") == 0) {
            //应收账款，默认生成销售单
            balance.MBillType = "Invoice_Sale";
        } else if (accountCode.indexOf("2202") == 0) {
            //应付账款，默认生成采购单
            balance.MBillType = "Invoice_Purchase";
        } else if (accountCode.indexOf("2203") == 0 || accountCode.indexOf("2241") == 0) {
            //预收账款,其他应付款默认生成收款单
            balance.MBillType = "Receive_Sale";

            if (balance.MContactType) {
                balance.MContactTypeFromBill = balance.MContactType;
            } else {
                balance.MContactTypeFromBill = balance.MContactTypeFromBill;
            }

            //收款单对应的供应商,员工或者没选联系人，是错误的逻辑，这个时候不生成单据
            if (balance.MContactTypeFromBill == "Supplier" || balance.MContactTypeFromBill == "Employees" || (!balance.MContactID && !balance.MEmployeeID)) {
                balance.MBillType = "";
                return;
            }

            if (((balance.MContactTypeFromBill == "Supplier" || balance.MContactTypeFromBill == "Customer" || balance.MContactTypeFromBill == "Other") && !balance.MContactID) ||
                (balance.MContactTypeFromBill == "Employees" && !balance.MEmployeeID)) {
                balance.MBillType = "";
                return;
            }

            //然后找一个相同币别的作为默认银行
            var bank = BalanceInput.getBankByCurrency(balance.MCurrencyID);

            if (bank) {
                balance.MBankID = bank.MItemID;
            } else {
                //银行账号不存在，默认也不生成单据
                balance.MBillType = "";
            }

        } else if (accountCode.indexOf("1123") == 0 || accountCode.indexOf("1221") == 0) {
            //预收账款,其他应收款默认生成收款单
            balance.MBillType = "Pay_Purchase";

            if (balance.MContactType) {
                balance.MContactTypeFromBill = balance.MContactType;
            } else {
                balance.MContactTypeFromBill = balance.MContactTypeFromBill;
            }

            if (balance.MContactTypeFromBill == "Customer" || (!balance.MContactID && !balance.MEmployeeID)) {
                balance.MBillType = "";
                //balance.MContactTypeFromBill = "";
                return;
            }
            
            //然后找一个相同币别的作为默认银行
            var bank = BalanceInput.getBankByCurrency(balance.MCurrencyID);

            if (bank) {
                balance.MBankID = bank.MItemID;
            } else {
                //银行账号不存在，默认也不生成单据
                balance.MBillType = "";
                //balance.MContactTypeFromBill = "";
            }
        }

    },
    getBankByCurrency: function (currencyId) {
        var bank = null;
        if (BalanceInput.bankList && currencyId) {
            for (var i = 0; i < BalanceInput.bankList.length; i++) {
                if (BalanceInput.bankList[i].MCyID == currencyId) {
                    bank = BalanceInput.bankList[i];
                    break;
                }
            }
        }
        //if (!bank) {
        //    bank = BalanceInput.bankList[0];
        //}

        return bank;
    },
    validateEditRow: function () {
        if (BalanceInput.editorRowIndex != null) {
            var editors = $("#gridAccountBalance").datagrid("getEditors", BalanceInput.editorRowIndex);

            if (editors == null) {
                return true;
            }

            //先检查是不是空数据行
            var isEmptyRow = true;
            for (var i = 0 ; i < editors.length; i++) {
                var editorValue = $(editors[i].target).val();

                if (editorValue) {
                    isEmptyRow = false;
                    break;
                }
            }
            //如果是空行数据，允许结束编辑
            if (isEmptyRow) {
                //禁用掉所有验证
                for (var i = 0 ; i < editors.length; i++) {
                    var validateDom = $(editors[i].target).next().find(".validatebox-invalid");
                    if (validateDom) {
                        $(validateDom).removeClass("validatebox-invalid").removeClass("validatebox-text");
                    }
                }

                return true;
            }


            //判断银行账号有没有填写
            var billTypeEditor = BalanceInput.getEditor("MBillType");
            if (billTypeEditor) {
                var billType = $(billTypeEditor.target).combobox("getValue");
                if (billType && (billType == "Receive_Sale" || billType == "Pay_Purchase")) {
                    var bankEditor = BalanceInput.getEditor("MBankID");
                    //没有填写银行账号
                    if (bankEditor && !$(bankEditor.target).combobox("getValue")) {
                        $(bankEditor.target).next().find("input").addClass("validatebox-text validatebox-invalid");
                        return false;
                    }
                }
            }


            for (var i = 0; i < editors.length; i++) {

                var editor = editors[i];

                var fieldOption = $("#gridAccountBalance").datagrid("getColumnOption", editor.field);
                //如果这一列隐藏了就不需要处理了
                if (!fieldOption.hidden) {
                    //如果是easyui的控件
                    if ($(editor.target).mIsEasyUIControl()) {
                        //
                        if (!$(editor.target).mValidateEasyUI()) {
                            return false;
                        }
                    }
                    else if ($(editor.target).hasClass("combotree-f")) {
                        if (!$(editor.target).combo("isValid")) {
                            return false;
                        }
                    } else if ($(editor.target).validatebox) {
                        //如果不是easyui组件，但是是validatebox组装件，则调用本身的validatebox方法
                        if (!$(editor.target).validatebox("isValid")) {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    },
    getSummaryData: function (data) {
        var result = new Array();

        if (!data || data.length == 0) {
            return result;
        }

        for (var i = 0 ; i < data.length; i++) {
            var row = data[i];
            if (row.MCheckGroupValueID == "0") {
                result.push(result);
            }
        }

        return result;
    },
    isValidateRow: function (row) {
        //没有选择币别，不提交改行数据
        if (!row.MCurrencyID) {
            return false;
        }

        //如果所有金额都没有填写，也不提交
        if (row.MInitBalance === "" && row.MInitBalanceFor === "" && row.MYtdCredit === "" &&
            row.MYtdCreditFor === "" && row.MYtdDebit === "" && row.MYtdDebitFor === "") {
            return false;
        }


        //如果是叶子节点，不提交汇总数据
        if (BalanceInput.isLeafAccount) {
            if (row.MCheckGroupValueID == "0") {
                return false;
            }
        }

        return true;

    },
    isLeafAccount: function () {
        var selectedTreeNode = BalanceInput.getSelectedAccount();

        var isLeafNode = $("#treeAccount").tree("isLeaf", selectedTreeNode.target);

        return isLeafNode;
    },
    getSelectedAccount: function () {
        var selectedTreeNode = $("#treeAccount").tree("getSelected");

        return selectedTreeNode;
    },
    gridDataProcess: function (data) {
        var tempDataArray = new Array();
        if (!data || data.length == 0) {
            return [];
        }
        for (var i = 0 ; i < data.length; i++) {

            if (data[i].MCheckGroupValueID == "0") {
                continue;
            }
            var tempData = {};

            tempData = $.extend(tempData, data[i]);

            var checkGroupValue = tempData.MCheckGroupValueModel;
            if (checkGroupValue) {
                for (var p in checkGroupValue) {
                    if (p == "MAccountID" || p == "MItemID" || p == "MCurrencyID" || p == "MContactType") {
                        continue;
                    }
                    tempData[p] = checkGroupValue[p];
                }
            }

            tempDataArray.push(tempData);
        }
        return tempDataArray;
    },
    bindNumberboxKeyupEvent: function () {
        var $dom = $("#gridAccountBalance");
        var editors = $dom.datagrid("getEditors", BalanceInput.editorRowIndex);

        for (var i = 0 ; i < editors.length; i++) {
            var editor = editors[i];
            if (editor.field == "MInitBalanceFor" || editor.field == "MYtdCreditFor" || editor.field == "MYtdDebitFor") {
                $(editor.target).off("keyup").on("keyup", function (e) {

                    //回车事件
                    BalanceInput.bindEnterEvent(e);
                    var dom = $(this);
                    setTimeout(function () {
                        //取不包括","的金额
                        var synchValue = dom.val().replace(/,/g, "");

                        var fieldName = dom.parents("[field]").attr("field");
                        var synchFieldName;

                        switch (fieldName) {
                            case "MInitBalanceFor":
                                synchFieldName = 'MInitBalance';
                                break;
                            case "MYtdCreditFor":
                                synchFieldName = 'MYtdCredit';
                                break;
                            case "MYtdDebitFor":
                                synchFieldName = 'MYtdDebit';
                                break;
                        }

                        var synchEditor = BalanceInput.getEditor(synchFieldName);
                        var currencyEditor = BalanceInput.getEditor("MCurrencyID");
                        var currencyId = $(currencyEditor.target).combobox("getValue");

                        var baseCurrencyValue = +synchValue * BalanceInput.getExchangeRate(currencyId);

                        $(synchEditor.target).numberbox("setValue", baseCurrencyValue);

                        $(synchEditor.target).validatebox("isValid");

                        //更新表格中的数据
                        BalanceInput.updateRowData(fieldName, synchValue, currencyId);
                        BalanceInput.updateRowData(synchFieldName, baseCurrencyValue, currencyId);
                        BalanceInput.refreshTotalUI();
                    }, 100);
                });
            }
            else if (editor.field == "MInitBalance" || editor.field == "MYtdCredit" || editor.field == "MYtdDebit") {
                $(editor.target).off("keyup").on("keyup", function (e) {

                    //回车事件
                    BalanceInput.bindEnterEvent(e);

                    var value = $(this).val();
                    var fieldName = $(this).parent().parent().parent().parent().parent().parent().attr("field");

                    var currencyEditor = BalanceInput.getEditor("MCurrencyID");
                    var currencyId = $(currencyEditor.target).combobox("getValue");

                    BalanceInput.updateRowData(fieldName, value, currencyId);
                    BalanceInput.refreshTotalUI();
                });
            } else {
                $(editor.target).off("keyup").on("keyup", function (e) {
                    //回车事件
                    BalanceInput.bindEnterEvent(e);
                });
            }
        }
    },
    bindEnterEvent: function (e) {
        if (e && e.keyCode == 13) {

            var field = $(e.target).parent().parent().parent().parent().parent().parent().attr("field");
            var editorIndex = BalanceInput.getColumnEditorIndex(field);
            var editors = BalanceInput.getColumnEditors();
            //如果超过了最大的editor，进入下一行
            if (editorIndex > editors.length) {
                if (!BalanceInput.validateEditRow()) {
                    var tips = HtmlLang.Write(LangModule.Acct, "EditRowDataNotValidate", "编辑行数据不合法,无法结束编辑!");
                    $.mDialog.message(tips);
                    return false;
                } else {
                    BalanceInput.endGridRowEdit();
                    BalanceInput.editorRowIndex++;

                    var rows = $("#gridAccountBalance").datagrid("getRows");

                    if (rows.length <= BalanceInput.editorRowIndex) {
                        BalanceInput.insertEmptyRow("#gridAccountBalance", 1);
                    }

                    BalanceInput.beginGridRowEdit(BalanceInput.editorRowIndex);
                }
            } else {
                //进入下一列
                var nextEditor = null;
                for (var i = editorIndex + 1; i < editors.length; i++) {
                    var tempNextEditor = editors[i];
                    if ($(tempNextEditor.target).attr("disabled") != "disabled") {
                        nextEditor = tempNextEditor;
                        break;
                    }
                }
                //如果没有找到，结束编辑，进入下一行
                if (nextEditor == null) {
                    if (!BalanceInput.validateEditRow()) {
                        var tips = HtmlLang.Write(LangModule.Acct, "EditRowDataNotValidate", "编辑行数据不合法,无法结束编辑!");
                        $.mDialog.message(tips);
                        return false;
                    } else {
                        BalanceInput.endGridRowEdit();
                        BalanceInput.editorRowIndex++;

                        var rows = $("#gridAccountBalance").datagrid("getRows");

                        if (rows.length <= BalanceInput.editorRowIndex) {
                            BalanceInput.insertEmptyRow("#gridAccountBalance", 1);
                        }

                        BalanceInput.beginGridRowEdit(BalanceInput.editorRowIndex);
                    }

                } else {
                    $(nextEditor.target).focus();
                }
            }
        }
    },
    //更新某行的数据
    updateRowData: function (field, newData, currencyId) {
        var gridDom = $("#gridAccountBalance");
        var row = gridDom.datagrid("getRowByIndex", BalanceInput.editorRowIndex);

        row[field] = newData;
        row["MCurrencyID"] = currencyId;

        BalanceInput.setTotalRowArray();
    },
    setTotalRowArray: function () {
        var rowsInGrid = $("#gridAccountBalance").datagrid("getData").rows;

        var totalRows = new Array();


        for (var i = 0; i < rowsInGrid.length; i++) {
            var rowInGrid = rowsInGrid[i];

            var currency = rowInGrid.MCurrencyID;
            if (!currency) {
                continue;
            }
            //空的话插入一条新的数据
            var totalRow = totalRows.where("x.MCurrencyID=='" + currency + "'")[0];
            var isExist = totalRow;
            if (!isExist) {
                totalRow = {};
                totalRow.MCurrencyID = currency;
                totalRow.MInitBalance = 0;
                totalRow.MInitBalanceFor = 0;
                totalRow.MYtdCredit = 0;
                totalRow.MYtdCreditFor = 0;
                totalRow.MYtdDebit = 0;
                totalRow.MYtdDebitFor = 0;
            }

            totalRow.MInitBalance += $.isNumeric(rowInGrid.MInitBalance) ? +rowInGrid.MInitBalance : 0;
            totalRow.MInitBalanceFor += $.isNumeric(rowInGrid.MInitBalanceFor) ? +rowInGrid.MInitBalanceFor : 0;
            totalRow.MYtdCredit += +rowInGrid.MYtdCredit;
            totalRow.MYtdCreditFor += +rowInGrid.MYtdCreditFor;
            totalRow.MYtdDebit += +rowInGrid.MYtdDebit;
            totalRow.MYtdDebitFor += +rowInGrid.MYtdDebitFor;
            totalRow.MCheckGroupValueID = "0";

            if (!isExist) {
                totalRows.push(totalRow);
            }

        }
        BalanceInput.totalRows = totalRows;
    },
    disableCurrenctyColumn: function (defaultCurrencyId) {
        var currencyEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: "MCurrencyID" });

        $(currencyEditor.target).combobox("setValue", defaultCurrencyId);

        $(currencyEditor.target).combobox("disable");
    },
    disableStartBalanceInput: function () {
        var intiBalanceForEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: "MInitBalanceFor" });
        $(intiBalanceForEditor.target).numberbox("disable");

        var intiBalanceEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: "MInitBalance" });
        $(intiBalanceEditor.target).numberbox("disable");
    },
    getExchangeRate: function (currencyId) {
        var currencyList = BalanceInput.currencyList;
        if (!currencyId || !currencyList || currencyList.length == 0 || currencyId == BalanceInput.baseCurrencyId) {
            return 1;
        }
        var exchangeRate = 1;
        for (var i = 0 ; i < currencyList.length; i++) {
            var currency = currencyList[i];
            if (currency.MCurrencyID == currencyId) {
                exchangeRate = currency.MRate;
                break;
            }
        }

        return exchangeRate;
    },
    //获取除法的汇率
    getExceptExchangeRate: function (currencyId) {
        var currencyList = BalanceInput.currencyList;
        if (!currencyId || !currencyList || currencyList.length == 0 || currencyId == BalanceInput.baseCurrencyId) {
            return 1;
        }
        var exchangeRate = 1;
        for (var i = 0 ; i < currencyList.length; i++) {
            var currency = currencyList[i];
            if (currency.MCurrencyID == currencyId) {
                exchangeRate = currency.MUserRate;
                break;
            }
        }

        return exchangeRate;
    },
    getBankAccountCurrency: function (bankId) {
        var defaultCurrencyId = "";
        for (var i = 0; i < BalanceInput.bankList.length; i++) {
            var bank = BalanceInput.bankList[i];

            if (bank.MItemID == bankId) {
                defaultCurrencyId = bank.MCyID;
            }
        }
        return defaultCurrencyId;
    },
    disabledBaseCurrencyInput: function (currencyId) {
        if (currencyId == BalanceInput.baseCurrencyId) {
            var initEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: 'MInitBalance' });
            $(initEditor.target).numberbox("disable");

            var ytdDebitEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: 'MYtdDebit' });
            $(ytdDebitEditor.target).numberbox("disable");

            var ytdCreditEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: 'MYtdCredit' });
            $(ytdCreditEditor.target).numberbox("disable");
        } else {
            var initEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: 'MInitBalance' });
            $(initEditor.target).numberbox("enable");

            var ytdDebitEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: 'MYtdDebit' });
            $(ytdDebitEditor.target).numberbox("enable");

            var ytdCreditEditor = $("#gridAccountBalance").datagrid("getEditor", { index: BalanceInput.editorRowIndex, field: 'MYtdCredit' });
            $(ytdCreditEditor.target).numberbox("enable");
        }

        //损益类科目不能录入期初余额
        //23727 在期初余额录入处，放开对成本类科目不能录入余额的控制，但损益类科目仍然不能录入期初余额
        // MAccountGroupID =4 的 5 开头可以录入，6开头的不可以录入。
        if (BalanceInput.account != null && ((BalanceInput.account.MAccountGroupID == "4" && BalanceInput.account.MCode.indexOf("6") == 0)
            || BalanceInput.account.MAccountGroupID == "5")) {

            BalanceInput.disableStartBalanceInput();
        }

    },
    reload: function () {
        mWindow.reload();
    },
    initGridRow: function () {
        var rows = $("#gridAccountBalance").datagrid("getRows");

        if (rows == 0) {
            BalanceInput.insertEmptyRow("#gridAccountBalance", 4);

            $("#ckbAutoAddInitDocument").removeAttr("disabled");

        }
    },
    divAutoSize: function () {
        var totalWidth = $(".m-imain").width();
        var leftWidth = $(".bi-left").width();

        var rightWidth = totalWidth - leftWidth - 10;

        $(".bi-content-right").css("width", rightWidth + "px");

        var totalHeight = $(".m-imain").height();

        //如果合计的table大于4行，不显示滚动条
        var tr = $("#tableTotal").find("tr");
        var tableHeight = 16;
        for (var i = 0 ; i < tr.length; i++) {
            tableHeight += $(tr[i]).height();
        }

        tableHeight = tableHeight < 64 ? 64 : tableHeight;
        tableHeight = tableHeight > 132 ? 132 : tableHeight;//调整高度避免出现滚动条

        $("#tableTotal").parent().height(tableHeight);

        var totalRowHeight = $(".bi-balance-total:visible").height();

        var gridHeight = totalHeight - totalRowHeight - $("#lblCurrentAccount").height();

        $("#gridAccountBalance").css("height", gridHeight);

        $("#gridAccountBalance").datagrid("resize");

        $(".datagrid-wrap", ".bi-content-right").css("height", gridHeight);

        //左侧树形框的高度
        var leftTotalHeight = $(".bi-content-left").height();
        var leftSearchHeight = $(".bi-contact-search-div").height();

        $(".bi-account-tree-div").height(leftTotalHeight - leftSearchHeight - 2);


    },
    //检查金额是否本位币和原币是否都录入了值
    checkAmountIsValid: function () {
        var initEditor = BalanceInput.getEditor("MInitBalance");
        var initEditorFor = BalanceInput.getEditor("MInitBalanceFor");

        if (!initEditor || !initEditorFor) {
            return true;
        }

        var initBalance = $(initEditor.target).numberbox("getValue");
        var initBalanceFor = $(initEditorFor.target).numberbox("getValue");

        if ((initBalance == 0 && initBalanceFor != 0) || (initBalance != 0 && initBalanceFor == 0)) {
            return false;
        }


        var ytdDebitEditor = BalanceInput.getEditor("MYtdDebit");
        var ytdDebitForEditor = BalanceInput.getEditor("MYtdDebitFor");

        if (!ytdDebitEditor || !ytdDebitForEditor) {
            return true;
        }

        var ytdDebit = $(ytdDebitEditor.target).numberbox("getValue");
        var ytdDebitFor = $(ytdDebitForEditor.target).numberbox("getValue");

        if ((ytdDebit == 0 && ytdDebitFor != 0) || (ytdDebit != 0 && ytdDebitFor == 0)) {
            return false;
        }

        var ytdCreditEditor = BalanceInput.getEditor("MYtdCredit");
        var ytdCreditForEditor = BalanceInput.getEditor("MYtdCreditFor");

        if (!ytdCreditEditor || !ytdCreditForEditor) {
            return true;
        }

        var ytdCredit = $(ytdCreditEditor.target).numberbox("getValue");
        var ytdCreditFor = $(ytdCreditForEditor.target).numberbox("getValue");

        if ((ytdCredit == 0 && ytdCreditFor != 0) || (ytdCredit != 0 && ytdCreditFor == 0)) {
            return false;
        }

        return true;
    },
    getBillTypeByAccount: function () {
        var code = BalanceInput.account.MCode;
        //应收账款，预收账款,其他应收款(销售单，红字销售单，收款单)
        var result = new Array();
        if (code.indexOf("1122") == 0 || code.indexOf("1221") == 0) {
            result.push(BalanceInput.billType[0]);
            //result.push(BalanceInput.billType[1]);
            //result.push(BalanceInput.billType[4]);
        } else if (code.indexOf("2203") == 0) {
            result.push(BalanceInput.billType[4]);
        } else if (code.indexOf("2202") == 0 || code.indexOf("2241") == 0) {
            //应付账款，预付账款，其他应付框(采购单，红字采购单，付款单，费用报销单)
            result.push(BalanceInput.billType[2]);
            //result.push(BalanceInput.billType[3]);
            //result.push(BalanceInput.billType[5]);

            if (code.indexOf("2241") == 0) {
                result.push(BalanceInput.billType[6]);
            }
        } else if (code.indexOf("1123") == 0) {
            result.push(BalanceInput.billType[5]);
        }

        return result;
    },
    isAutoCreateInitDocument: function (data) {
        if (!data || data.length == 0) {
            return;
        }

        //非往来科目不做处理
        if (!BalanceInput.isCurrenctAccount()) {
            $("#ckbAutoAddInitDocument").removeAttr("checked");
            $(".bi-auto-addinitdocument").hide();
            $(".bi-createbill-tips").mTitle("destory");
            return false;
        } else {
            $(".bi-auto-addinitdocument").show();
            $(".bi-createbill-tips").mTitle();
        }

        //for (var i = 0; i < data.total; i++) {
        //    var row = data.rows[i];

        //    if (row["MBillType"]) {
        //        $("#ckbAutoAddInitDocument").attr("checked", "checked");
        //        $("#ckbAutoAddInitDocument").attr("disabled", "disabled");
        //        break;
        //    }
        //}

        if (BalanceInput.forbitEdit) {
            $("#ckbAutoAddInitDocument").attr("disabled", "disabled");
        } else {
            $("#ckbAutoAddInitDocument").removeAttr("disabled");
        }
    },
    hideTreeNode: function (tree, node) {
        if (!node || !node.target) {
            return;
        }
        $(node.target).parent().empty();

    },
    getColumnEditors: function () {

        var editors = $("#gridAccountBalance").datagrid("getEditors", BalanceInput.editorRowIndex);
        return editors;
    },
    getColumnEditorIndex: function (field) {
        var editors = BalanceInput.getColumnEditors();
        var index = -1;
        if (editors && editors.length > 0) {
            for (var i = 0; i < editors.length; i++) {
                if (editors[i].field == field) {
                    index = i;
                    break;
                }
            }
        }
        return index;
    },
    setDataRule: function () {
        //银行科目，给默认币别
        var defaultCurrencyId = BalanceInput.baseCurrencyId;
        if (BalanceInput.account != null &&
            (BalanceInput.account.MCode.indexOf("1001") == 0 || BalanceInput.account.MCode.indexOf("1002") == 0)) {
            var defaultCurrencyId = BalanceInput.getBankAccountCurrency(BalanceInput.account.MItemID);

            BalanceInput.disableCurrenctyColumn(defaultCurrencyId);
            BalanceInput.disabledBaseCurrencyInput(defaultCurrencyId);
        }

        //损益类科目不能录入期初余额
        //23727 在期初余额录入处，放开对成本类科目不能录入余额的控制，但损益类科目仍然不能录入期初余额
        if (BalanceInput.account != null && ((BalanceInput.account.MAccountGroupID == "4" && BalanceInput.account.MCode.indexOf("6") == 0)
            || BalanceInput.account.MAccountGroupID == "5")) {
            BalanceInput.disableStartBalanceInput();
        }

        //非外币核算的科目不能选择外币
        if (BalanceInput.account.MIsCheckForCurrency == false) {
            BalanceInput.disableCurrenctyColumn(defaultCurrencyId);
            BalanceInput.disabledBaseCurrencyInput(defaultCurrencyId);
        }

        var editors = BalanceInput.getColumnEditors();

        if (!editors || editors.length == 0) {
            return;
        }

        for (var i = 0; i < editors.length; i++) {
            var editor = editors[i];

            if (editor.field == "MBillType") {
                var billType = $(editor.target).combobox("getValue");

                if (billType && billType != "Pay_Purchase" && billType != "Receive_Sale") {
                    var bankEditor = BalanceInput.getEditor("MBankID");

                    if (bankEditor) {
                        $(bankEditor.target).next().find("input").attr("disabled", "disabled");
                    }
                }
            } else if (editor.field == "MCurrencyID") {
                var currencyId = $(editor.target).combobox("getValue");

                //币别有值后不允许更改
                if (currencyId) {
                    $(editor.target).combobox("disable");
                } else {
                    $(editor.target).combobox("enable");
                }

                //本位币
                if (currencyId == BalanceInput.baseCurrencyId) {
                    var initBalanceEditor = BalanceInput.getEditor("MInitBalance");

                    if (initBalanceEditor) {
                        $(initBalanceEditor.target).numberbox("disable");
                    }

                    var ytdDebitEditor = BalanceInput.getEditor("MYtdDebit");
                    if (ytdDebitEditor) {
                        $(ytdDebitEditor.target).numberbox("disable");
                    }

                    var ytdCreditEditor = BalanceInput.getEditor("MYtdCredit");
                    if (ytdCreditEditor) {
                        $(ytdCreditEditor.target).numberbox("disable");
                    }
                }
            }

        }
        //重新注册一下单击全选
        Megi.regClickToSelectAllEvt();
    },
    //是否空白行，没有任何数据
    isEmptyRow: function () {
        var editors = $("#gridAccountBalance").datagrid("getEditors", BalanceInput.editorRowIndex);

        //先检查是不是空数据行
        var isEmptyRow = true;
        for (var i = 0 ; i < editors.length; i++) {
            var editorValue = $(editors[i].target).val();

            if (editorValue) {
                isEmptyRow = false;
                break;
            }
        }

        return isEmptyRow;
    },
    isEmptyRowData: function (row) {

    },
    getAddOptions: function (checkTypeEnum) {
        var addOptions = null;

        if (checkTypeEnum == "0") {
            addOptions = {
                hasPermission: true,
                itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                dialogTitle: HtmlLang.Write(LangModule.Common, "NewContact", "New Contact"),
                url: "/BD/Contacts/ContactsEdit/",
                width: 1080,
                height: 450,
                callback: function () {
                    var field = BalanceInput.getGridFieldByCheckTypeEnum(checkTypeEnum);
                    var newestData = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
                    var editor = BalanceInput.getEditor(field);
                    $(editor.target).combotree("loadData", newestData);

                    if (field == "MContactID") {
                        BalanceInput.setContactShowRule($(editor.target));
                    }
                }
            }
        } else if (checkTypeEnum == "1") {
            addOptions = "employee";

        } else if (checkTypeEnum == "2") {
            addOptions = "inventory";
        } else if (checkTypeEnum == "3") {
            var addOptions = {
                hasPermission: true,
                itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                dialogTitle: HtmlLang.Write(LangModule.Common, "NewExpenseItem", "New Expense Item"),
                url: "/BD/ExpenseItem/ExpenseItemEdit",
                width: 520,
                height: 400,
                callback: function () {
                    var field = BalanceInput.getGridFieldByCheckTypeEnum(checkTypeEnum);
                    var newestData = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
                    var editor = BalanceInput.getEditor(field);
                    $(editor.target).combotree("loadData", newestData);
                }
            }

        }
        //工资项目新增选项
        else if (checkTypeEnum == "4") {
            var addOptions = {
                hasPermission: true,
                itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                dialogTitle: HtmlLang.Write(LangModule.BD, "NewSalaryItem", "New Salary Item"),
                url: "/PA/PayItem/PayItemEdit?type=0",
                width: 420,
                height: 365,
                callback: function () {
                    var field = BalanceInput.getGridFieldByCheckTypeEnum(checkTypeEnum);
                    var newestData = BalanceInput.getBaseDataByCheckTypeEnum(checkTypeEnum, true);
                    var editor = BalanceInput.getEditor(field);
                    $(editor.target).combotree("loadData", newestData);
                }
            }
        } else if (checkTypeEnum == "5") {
            addOptions = "trackOption";
        } else if (checkTypeEnum == "6") {
            addOptions = "trackOption";
        } else if (checkTypeEnum == "7") {
            addOptions = "trackOption";

        } else if (checkTypeEnum == "8") {
            addOptions = "trackOption";
        } else if (checkTypeEnum == "9") {
            addOptions = "trackOption";
        }
        return addOptions;
    },
    getGridFieldByCheckTypeEnum: function (checkTypeEnum) {
        var field = "";
        switch (checkTypeEnum) {
            case "0":
                field = "MContactID";
                break;
            case "1":
                field = "MEmployeeID";
                break;
            case "2":
                field = "MMerItemID";
                break;
            case "3":
                field = "MExpItemID";
                break;
            case "4":
                field = "MPaItemID";
                break;
            case "5":
                field = "MTrackItem1";
                break;
            case "6":
                field = "MTrackItem2";
                break;
            case "7":
                field = "MTrackItem3";
                break;
            case "8":
                field = "MTrackItem4";
                break;
            case "9":
                field = "MTrackItem5";
                break;
        }
        return field;
    },
    getAddTrackOptionUrl: function (checkTypeEnum) {
        if (BalanceInput.trackList == null) {
            return "";
        }

        var trackIndex = +checkTypeEnum - 4;
        if (BalanceInput.trackList.length < trackIndex) {
            return "";
        }
        var trackId = BalanceInput.trackList[trackIndex - 1].MValue;
        var url = "/BD/Tracking/CategoryOptionEdit?trackId=" + trackId;

        return url;
    },
    setBaseData: function (checkType, data) {
        if (checkTypeEnum == "0") {
            BalanceInput.contactList = data;
        } else if (checkTypeEnum == "1") {

            BalanceInput.employeeList = data;

        } else if (checkTypeEnum == "2") {
            BalanceInput.merchandiseItemList = data;

        } else if (checkTypeEnum == "3") {
            BalanceInput.expenseItemList = data;

        } else if (checkTypeEnum == "4") {

            BalanceInput.paList = data;
        } else if (checkTypeEnum == "5") {
            BalanceInput.trackItem1Option = data;
        } else if (checkTypeEnum == "6") {
            BalanceInput.trackItem2Option = data;
        } else if (checkTypeEnum == "7") {
            BalanceInput.trackItem3Option = data;
        } else if (checkTypeEnum == "8") {
            BalanceInput.trackItem4Option = data;
        } else if (checkTypeEnum == "9") {
            BalanceInput.trackItem5Option = data;
        }
    }
}