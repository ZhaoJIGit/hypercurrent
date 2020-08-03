var CustomAccountMatch = {
    editorNode: null,
    //需要进行匹配的银行账号
    matchBankAccountList: new Array(),
    macthCashAccountList: new Array(),
    init: function () {
        CustomAccountMatch.initCashAndBankAccountList();
        CustomAccountMatch.initAtion();
        CustomAccountMatch.initAccountList();
    },
    initAtion: function () {
        $("#btnFinishMatch").click(function () {
            var data = $("#tbAll").treegrid("getData");
            //排除掉所有没有MCode的科目，并对现金科目和银行科目进行处理
            var saveData = new Array();
            for (var i = 0 ; i < data.length; i++) {
                var account = data[i];

                CustomAccountMatch.getMatchAccountRecursion(account, saveData);
            }
            var confirmMsg = HtmlLang.Write(LangModule.BD, "CustomMatchConfirmMessage", "现金科目和银行科目匹配后，我们将自动同步您创建的银行账号。您确认要继续吗？");
            $.mDialog.confirm(confirmMsg, function () {

                mAjax.submit("/BD/BDAccount/CustomAccountFinishMatch", { list: saveData }, function (msg) {
                    if (msg && msg.Success) {
                        var message = HtmlLang.Write(LangModule.BD, "AccountMatchSuccess", "科目匹配成功！");
                        $.mDialog.message(message);
                        $.mDialog.close();
                    } else {
                        var message = HtmlLang.Write(LangModule.BD, "AccountMatchFail", "科目匹配失败！")
                        if (msg && msg.Message) {
                            message = msg.Message;
                        }
                        $.mDialog.alert(message);
                    }
                }, "", true);
            });

        });
    },
    getMatchAccountRecursion: function (parentData, container) {
        var childrenAccount = parentData.children;
        var text = $("#" + parentData.id).combobox("getText");
        var code = $("#" + parentData.id).combobox("getValue");

        if (code && code != "null" && text) {
            parentData.MCode = code;
            var accountModel = CustomAccountMatch.convertAccountModel(parentData);
            container.push(accountModel);
        }


        if (!childrenAccount || childrenAccount.length == 0) {
            return container;
        }

        if (childrenAccount && childrenAccount.length > 0) {
            for (var j = 0 ; j < childrenAccount.length; j++) {
                var child = childrenAccount[j];
                CustomAccountMatch.getMatchAccountRecursion(child, container);
            }
        }

    },
    initAccountList: function () {
        $("#tbAll").treegrid({
            url: "/BD/BDAccount/GetAccountList",
            queryParams: { IsActive: true },
            idField: "id",
            treeField: 'text',
            checkbox: true,
            fitColumns: false,
            singleSelect: false,
            scrollY: true,
            lines: true,
            region: "center",
            columns: [[{
                field: 'id', width: 40, hidden: true,
            },
            {
                title: LangKey.Name, field: 'text', width: 200, sortable: false
            },
            { title: LangKey.Code, field: 'MNumber', width: 120, sortable: false },
            {
                title: HtmlLang.Write(LangModule.BD, "MatchAccount", "科目匹配"), field: 'MCode', width: 160, sortable: false,
                formatter: function (value, row, index) {

                    return "<input id='" + row.id + "' class='match-account easyui-combobox' account-code='" + value + "'/>";

                    //var parentNode = $("#tbAll").treegrid('getParent', row.id);
                    //if (!parentNode) {
                    //    return "<input id='"+row.id+"' class='match-account easyui-combobox' account-code='"+value+"'/>";
                    //} else {
                    //    return "<input id='" + row.id + "' class='match-bank easyui-combobox' bank-id='" + row.id + "' new-bank-id=''/>";
                    //}
                }
            }
            ]],
            onBeforeLoad: function () {

            },
            onLoadSuccess: function () {
                $(this).treegrid("collapseAll");
                //控件渲染
                $(".match-account").each(function () {
                    $(this).combobox({
                        data: CustomAccountMatch.matchAccountList,
                        valueField: 'id',
                        textField: 'text',
                        width: 150,
                        onChange: function (newValue, oldValue) {
                            //如果选择了现金科目和银行科目，则他的子级科目加载银行账号匹配
                            var id = $(this).attr("id");
                            if (newValue == "1001") {

                                $("#tbAll").treegrid("expandAll", id);
                                var childNode = $("#tbAll").treegrid("getChildren", id);
                                CustomAccountMatch.initBankCombobox(childNode, CustomAccountMatch.macthCashAccountList);

                            } else if (newValue == "1002") {
                                $("#tbAll").treegrid("expandAll", id);
                                var childNode = $("#tbAll").treegrid("getChildren", id);

                                CustomAccountMatch.initBankCombobox(childNode, CustomAccountMatch.matchBankAccountList);
                            }

                            //清空掉其他选择这个值的combobox

                            $(".match-account[id!='" + id + "']").each(function () {
                                if ($(this).combobox("getValue") == newValue) {
                                    $(this).combobox("setValue", "");
                                }
                            });
                        }
                    });
                });

                //渲染完成后，加载默认值
                $(".match-account").each(function () {
                    var code = $(this).attr("account-code");
                    if (code) {
                        $(this).combobox("setValue", code);
                    }
                });

                $(".match-bank").each(function () {
                    $(this).combobox({
                        valueField: 'MItemID',
                        textField: 'MBankName',
                        width: 150,
                        onLoadSuccess: function () {
                            var bankId = $(this).attr("bank-id");
                            if (bankId) {
                                $(this).combobox("setValue", bankId);
                            }
                        },
                        onChange: function (newValue, oldValue) {
                            var id = $(this).attr("id");
                            $(this).attr("new-bank-id", newValue);

                            $(".match-bank[id!='" + id + "']").each(function () {
                                if ($(this).combobox("getValue") == newValue) {
                                    $(this).combobox("setValue", "");
                                }
                            });
                        }
                    });
                });
                $(".match-bank").each(function () {
                    var bankId = $(this).attr("bank-id");
                    if (bankId) {
                        $(this).combobox("setValue", bankId);
                    }
                })

            },
            onClickRow: function (row) {
                $(this).treegrid("unselectAll");
            }

        });
    },

    initBankCombobox: function (nodes, data) {
        //没有子项，不做任何事情
        if (!nodes || nodes.length == 0) {
            return;
        }
        //将子项的combobox加载银行账号科目
        for (var i = 0; i < nodes.length; i++) {
            $("#" + nodes[i].id).combobox({
                valueField: 'MItemID',
                textField: 'MBankName',
                data: data,
                width: 150,
                onLoadSuccess: function () {
                    var bankId = $(this).attr("bank-id");
                    if (bankId) {
                        $(this).combobox("setValue", bankId);
                    }
                },
                onChange: function (newValue, oldValue) {
                    var id = $(this).attr("id");
                    $(this).attr("new-bank-id", newValue);

                    $(".match-bank[id!='" + id + "']").each(function () {
                        if ($(this).combobox("getValue") == newValue) {
                            $(this).combobox("setValue", "");
                        }
                    });
                }
            })
        }
    },

    matchAccountList: [
        { "id": "1001", "text": HtmlLang.Write(LangModule.Acct, "CashInHand", "Cash in Hand") },
        { "id": "1002", "text": HtmlLang.Write(LangModule.Acct, "Bank", "Bank") },
        { "id": "1122", "text": HtmlLang.Write(LangModule.Acct, "AccountsReceivable", "Accounts Receivable") },
        { "id": "1123", "text": HtmlLang.Write(LangModule.Acct, "Prepayment", "Prepayment") },
        { "id": "2202", "text": HtmlLang.Write(LangModule.Acct, "AccountsPayable", "Accounts Payable") },
        { "id": "2203", "text": HtmlLang.Write(LangModule.Acct, "AdvanceCustomers", "Advance from customers") },
        { "id": "1221", "text": HtmlLang.Write(LangModule.Acct, "OtherReceivable", "Other Receivable") },
        { "id": "2241", "text": HtmlLang.Write(LangModule.Acct, "OtherPayables", "Other Payables") },
        { "id": "22210101", "text": HtmlLang.Write(LangModule.Acct, "VATIN", "进项税额") },
        { "id": "22210105", "text": HtmlLang.Write(LangModule.Acct, "VATOUT", "销项税额") },
        { "id": "1405", "text": HtmlLang.Write(LangModule.Acct, "FinishedGoods", "库存商品") },
        { "id": "660303", "text": HtmlLang.Write(LangModule.Acct, "BankCharges", "银行手续费") },
        { "id": "660301", "text": HtmlLang.Write(LangModule.Acct, "InterestExpenses", "利息费用") },
        { "id": "6401", "text": HtmlLang.Write(LangModule.Acct, "PrimeOperatingCosts", "主营业务成本") }
    ],
    initCashAndBankAccountList: function () {
        $.mAjax.post("/BD/BDBank/GetBDBankAccountViewList", {}, function (msg) {
            if (msg) {

                var data = msg;
                if (data && data.length > 0) {
                    for (var i = 0 ; i < data.length; i++) {
                        if (data[i].MBankAccountType == 3) {
                            CustomAccountMatch.macthCashAccountList.push(data[i]);
                        } else {
                            CustomAccountMatch.matchBankAccountList.push(data[i]);
                        }
                    }
                }
            }
        })
    },
    convertAccountModel: function (accountTree) {
        var account = {};
        account.MItemID = accountTree.id;
        account.MCode = accountTree.MCode;
        account.MNumber = accountTree.MNumber;
        account.MParentID = accountTree._parentId ? accountTree._parentId : "0";
        return account;
    }
};

$(document).ready(function () {
    CustomAccountMatch.init();
});