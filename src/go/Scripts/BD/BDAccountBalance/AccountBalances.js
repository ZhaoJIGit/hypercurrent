var AccountBalances = {
    dbClickRow: null,
    dbClickField: null,
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    //包含累计
    includeYTD: $("#hideGLBeginMonth").val() == 1,
    //用于其他页签刷新这个页签
    isHide: false,
    containner: null,
    currencyList: null,
    canSelectCurrencyList: null,
    baseCurrency: $("#hideBaseCy").val(),
    go: $("#hideGoService").val(),
    permisson: $("#hidePermission").val(),
    accountStandard: $("#hideAccountStandard").val(),
    bankList: null,
    isGuidPage: $("#hideIsSetup").val(),
    tabSwitch: new BrowserTabSwitch(),
    init: function () {
        AccountBalances.tabSwitch.initSessionStorage();
        AccountBalances.initUI();
        AccountBalances.initAction();
        AccountBalances.initCurrency();
        AccountBalances.initBankList();
    },
    initUI: function () {
        var forbidEdit = $("#hideForbidEdit").val() == "1";
        if (forbidEdit) {
            $("#btnReInit").show();
            $("#btnImport").hide();
            $("#btnClearInitBalance").hide();
        } else {
            $("#btnFinish").show();
            $("#btnImport").show();
            $("#btnClearInitBalance").show();
            //同时检查初始化单据的金额是否等于科目的期初余额 Felson 2016.3.1
            if (AccountBalances.accountStandard == 3) {
                AccountBalances.checkCustomAccountIsMatch();
            }
        }

        if (!$("#hideIsSetup").val()) {
            $("#btnAddCurrency").css("display", "none");
        }
    },
    checkBalanceEqualWithBill: function (callback) {
        //
        var url = "/BD/BDAccount/CheckBalanceEqualWithBill";
        //
        var sure2SynchronizeLang = HtmlLang.Write(LangModule.Acct, "NotFinishInit", "无法完成初始化，请核对后重新录入");
        //
        mAjax.post(url, {}, function (data) {
            //
            if (data && data.Success) {
                //如果不相等，则提醒用户
                if (!data.Success && data.Message) {
                    var errorMessage = "<div style='text-align:left;margin-left: 10px;'>";
                    var messages = data.Message.trimStart(';').trimEnd(';').split(';');
                    for (var i = 0; i < messages.length ; i++) {
                        errorMessage += (i + 1) + "." + messages[i] + ";<br>";
                    }
                    errorMessage += sure2SynchronizeLang + "</div>";
                    $.mDialog.alert(errorMessage);
                 
                }
                else {
                    $.isFunction(callback) && callback();
                }
            }
        }, "", true);
    },
    //将业务单据的数据同步到科目初始化余额
    synchronizeAccountBalanceWithBill: function () {
        //
        var url = "/BD/BDAccount/SynchronizeAccountBalanceWithBill";
        //
        mAjax.submit(url, {}, function (data) {
            //
            if (data && data.Success) {
                //当前页面刷新
                mWindow.reload();
            }
        });
    },
    initAction: function () {
        $("#aUpdate").click(function () {
            AccountBalances.saveBalance();
            return false;
        });
        var bankHomeOj = new BDBankHome();

        $("#btnAddBankAccountTop").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[1].type, "", AccountBalances.reload) });
        //新增银行
        $("#btnAddBankAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[1].type, "", AccountBalances.reload) });
        //新增信用卡
        $("#btnAddCreditAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[2].type, "", AccountBalances.reload) });
        //新增现金帐户
        $("#btnAddCashAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[3].type, "", AccountBalances.reload) });
        //新增现金帐户
        $("#btnAddPayPalAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[4].type, "", AccountBalances.reload) });
        //新增现金帐户
        $("#btnAddAlipayAccount").off("click").on("click", function () { bankHomeOj.editBankAccount(BankTypeEnum[5].type, "", AccountBalances.reload) });

        

        $("#btnSynchBankAmount").click(function () {
            mAjax.submit(
                "/BD/BDAccount/UpdateBankAccountInitBalance",
                {},
                function (msg) {
                    if (msg) {
                        if (msg && msg.Success) {
                            var message = HtmlLang.Write(LangModule.Acct, "UpdateBankAccountInitBalanceSuccess", "Update bank account init balance successfully!");
                            $.mDialog.alert(message);
                            AccountBalances.bindGrid();
                        } else {
                            var message = HtmlLang.Write(LangModule.Acct, "UpdateBankAccountInitBalanceFail", "Update bank account init balance Fail!");
                            $.mDialog.alert(message);
                        }
                    }
                });
        });
        
        $("#btnAddCurrency").click(function () {
            var pageTitle = HtmlLang.Write(LangModule.BD, "Currency", "Currency");
            var url = $("#hidGoServer").val() + '/BD/Currency/Currency';
            $.mTab.addOrUpdate(pageTitle, url, true);
        });

        $("#btnImport").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/OpeningBalance', HtmlLang.Write(LangModule.Acct, "ImportOpeningBalance", "导入科目期初余额"), 900, 520);
        });
    },
    getOptionExchange: function (cyId) {
        var data = AccountBalances.currencyList;
        if (data) {
            for (var i = 0; i < data.length; i++) {
                if (data[i].MCurrencyID == cyId) {
                    var temp = data[i].MUserRate ? parseFloat(data[i].MUserRate) : null;
                    if (temp) {
                        return temp.toFixed(6);
                    } else {
                        //汇率无法换算成小数，但是也要停止循环
                        break;
                    }

                }
            }
        }
        return "";
    },
    ///初始化用户币别
    initCurrency: function () {
        mAjax.post(
            "/BD/Currency/GetBDCurrencyList",
            { isIncludeBase: true },
            function (msg) {
                if (msg) {
                    //保存为全局变量
                    AccountBalances.currencyList = msg;
                    AccountBalances.canSelectCurrencyList = AccountBalances.copyArray(msg);
                    AccountBalances.bindGrid();
                }
            });
    },
    initBankList: function () {
        mAjax.post(
            "/BD/BDBank/GetBDBankAccountViewList",
            { isIncludeBase: true },
            function (msg) {
                if (msg) {
                    //保存为全局变量
                    if (msg) {
                        AccountBalances.bankList = msg;
                    }
                }
            });
    },

    bindGrid: function () {
        var searchFilter = "0";

        var objInfo = AccountBalances.getTabSelectedInfo();

        var forbidEdit = $("#hideForbidEdit").val() == "1";
        //根据cyId初始InitialBalance列的colspan

        $("#tbAccountBalance").treegrid({
            url: "/BD/BDAccount/GetAccountInitBalanceTreeList?cyId=0&includeContact=true",
            queryParams: { group: objInfo.Title, searchFilter: searchFilter },
            idField: "id",
            treeField: 'text',
            fitColumns: true,
            scrollY: true,
            lines: true,
            columns: [[

            { title: LangKey.Code, field: 'MNumber', width: 80, align: 'left', sortable: true },
            {
                title: LangKey.Name, field: 'text', width: 200, sortable: true, formatter: function (value, rec, rowIndex) {

                    //如果是核算项目
                    if (!forbidEdit && !rec.MNumber) {
                        return value + "<a href='javascript:void(0)'  class='del-contact' onclick='AccountBalances.addAccountingProject(\"" + rec.id + "\")' title='Add contact'></a>";
                    } else {
                        //如果有子项，但是子项的MNumber为0，则还是可以新增联系人的初始余额
                        //遍历所有的子项，如果子项的rec.MNumber为空，则表示是余额子表中的项，可以继续新增联系人核算科目
                        var canAdd = true;
                        if (rec.children && rec.children.length > 0) {
                            for (var i = 0 ; i < rec.children.length; i++) {
                                var child = rec.children[i];

                                if (child.MNumber) {
                                    canAdd = false;
                                    break;
                                }
                            }
                        }
                        if (!forbidEdit && (!rec.children || rec.children.length == 0 || canAdd) && rec.MNumber && rec.IsCanRelateContact) {
                            //return "<a href='javascript:void(0);' style='line-height:18px;'>"+value+"</a>";
                            return value;
                        }
                    }
                    return  mText.encode(value);
                }
            },

            {
                title: HtmlLang.Write(LangModule.Acct, "Direction", "Direction"), field: 'MDC', align: 'center', width: 50, sortable: true, formatter: function (value, rec, rowIndex) {
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
                title: HtmlLang.Write(LangModule.Acct, "InitialBalance", "Initial balance"), width: 190, align: 'right', field: "MInitBalanceFor", formatter: function (value, rec, rowIndex) {
                    var list = rec.Balances;
                    //是否可编辑
                    var canEdit = AccountBalances.permisson && !forbidEdit && (rec.children == null || rec.children.length == 0) && AccountBalances.isCanEditBalance(rec.MAccountGroupID, "MInitBalanceFor");
                    //银行科目应该也是不能出编辑框的
                    return AccountBalances.createBalanceTable(list, "MInitBalanceFor", canEdit, "MInitBalance");
                }
            },


            {
                title: HtmlLang.Write(LangModule.Acct, "CumulativeDebitThisYear", "Cumulative debit this year"), hidden: AccountBalances.includeYTD, align: 'right', width: 190, field: "MYtdDebitFor", formatter: function (value, rec, rowIndex) {
                    var list = rec.Balances;
                    var canEdit = AccountBalances.permisson && !forbidEdit && (rec.children == null || rec.children.length == 0) && AccountBalances.isCanEditBalance(rec.MAccountGroupID, "MYtdDebit");
                    return AccountBalances.createBalanceTable(list, "MYtdDebitFor", canEdit, "MYtdDebit");
                }
            },
             {
                 title: HtmlLang.Write(LangModule.Acct, "CumulativeCreditThisYear", "Cumulative credit this year"), hidden: AccountBalances.includeYTD, align: 'right', width: 190, field: "MYtdCreditFor", formatter: function (value, rec, rowIndex) {
                     var list = rec.Balances;
                     var canEdit = AccountBalances.permisson && !forbidEdit && (rec.children == null || rec.children.length == 0) && AccountBalances.isCanEditBalance(rec.MAccountGroupID, "MYtdCredit");
                     return AccountBalances.createBalanceTable(list, "MYtdCreditFor", canEdit, "MYtdCredit");
                 }
             }

            ]],
            onLoadSuccess: function () {
                $(".datagrid-body").find(".easyui-numberbox").each(function () {
                    $(this).numberbox({
                        min: 0,
                        precision: 2
                    });
                })
                if ($("#hidIsSetup").val() === "true") {
                    Setup.initUI();
                    SetupBase.adjustGridHeight();
                } else {
                    AccountBalances.autoHeight();
                }

                AccountBalances.updateFirstGradeBackGroundColor();

                $(".datagrid-body").scroll(function () {
                    if (AccountBalances.dbClickRow) {
                        var node_id = AccountBalances.dbClickRow.id;
                        var locationDiv = $(".datagrid-btable").find("tr[node-id='" + node_id + "']").find("td[field='" + AccountBalances.dbClickField + "']").find(".div-balance");
                        var top = locationDiv.offset().top;
                        //如果点击行的高度小于一个表格的高度，隐藏币别编辑框
                        var domCurrencyEdit = $("#divbalance");
                        var gridTop = $(".datagrid-htable").offset().top + $(".datagrid-htable").height();
                        if (top < gridTop) {
                            domCurrencyEdit.hide();
                        } else {
                            var left = locationDiv.offset().left;
                            var height = locationDiv.height();
                            if (domCurrencyEdit.attr("dec") == "down") {
                                domCurrencyEdit.css("position", "fixed");
                                domCurrencyEdit.css("left", left + "px");
                                domCurrencyEdit.css("top", top + height + "px");
                                domCurrencyEdit.show();
                            } else {
                                domCurrencyEdit.css("position", "fixed");
                                domCurrencyEdit.css("left", left + "px");
                                domCurrencyEdit.css("top", top - domCurrencyEdit.height() - height + "px");
                                domCurrencyEdit.attr("dec", "up");
                            }

                        }
                    }

                    Megi.regClickToSelectAllEvt();
                });
                if (AccountBalances.isHide) {
                    AccountBalances.containner.hide();
                    AccountBalances.isHide = false;
                }

                $(".datagrid-row").mouseover(function () {
                    
                    $(this).find("td").css("color", "white");
                }).mouseout(function () {
                    $(this).find("td").css("color", "#7EB8EA");
                });
            },
            onClickCell: function (field, row) {
                //如果是银行和现金科目，直接调整到银行初始录入界面
                if (!AccountBalances.isCanClickFiled(field)) {
                    return;
                }
                var title = HtmlLang.Write(LangModule.Acct, "AccountInitBalanceInput", "科目初始余额录入");

                var isGuid = $("#hideIsSetup").val()=="true" ? true : false;

                $.mTab.addOrUpdate(title, AccountBalances.go + '/BD/BDAccountBalance/BalanceInput?defaultId=' + row.id + "&isGuide=" + isGuid, true, true, false, true);
            }
        });
    },
    //更新一级科目行的背景色
    updateFirstGradeBackGroundColor: function () {
        //
        //$(".datagrid-body > .datagrid-btable > tbody > tr.datagrid-row").css({
        //    "background-color": "#f8fcff"
        //});
    },
    specialAccountCodes: ['1122', '2203', '2202', '1123', '1221', '2241'],
    fnArray: function () {
        AccountBalances.bindGrid();
        //$.mDialog.close();
    },
    reload: function () {
        mWindow.reload();
    },
    changeTabItem: function (title, index) {
        $("#txtKeyword").val("");
        AccountBalances.bindGrid();
    },
    getTabSelectedInfo: function () {
        var obj = {};
        obj.Title = "";
        obj.IsActive = true;

        var count = $("#tabAccount").find(".tabs>li").length;
        $("#tabAccount").find(".tabs>li").each(function (i) {
            if ($(this).hasClass("tabs-selected")) {
                if (i > 0 && i < count - 1) {
                    obj.Title = $(this).find(".tabs-title").html();
                }
                if (i == count - 1) {
                    obj.IsActive = false;
                }
                return;
            }
        });
        return obj;
    },
    //现在核算项目
    addAccountingProject: function (id) {
        $("#tbAccountBalance").treegrid("unselectAll");
        $("#tbAccountBalance").treegrid("select", id);

        var row = $("#tbAccountBalance").treegrid("getSelected");
        Megi.dialog({
            title: HtmlLang.Write(LangModule.BD, "AddAccountingProject", "Add Accounting Project"),
            width: 350,
            height: 300,
            href: '/BD/BDAccount/AddAccountingProject?accountId=' + row.id + "&accountName=" + row.text + "&cyId="
        });
    },
    delAccountingProject: function (id) {

    },
    saveBalance: function (list, contactId) {
        mAjax.submit("/BD/BDAccount/UpdateInitBalance?contactId=" + contactId, { modelList: list }, function (data) {
            if (!data.Success) {
                $.mAlert(data.Message);
            } else {
                $("#btnCancel").trigger("click");
                AccountBalances.bindGrid();
            }
        });
    },
    autoHeight: function () {
        var selectorName = "body";
        //grid的第一父元素
        var containner = $(selectorName);
        //减去父元素的上级padding作为grid的高度
        var gridWrapDivHegith = containner.height() - $("#hideAutoHeight").val();
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
    //余额list ， grid字段 ， 本位币字段，是否可以编辑
    createBalanceTable: function (list, field, canEdit, baseCurrencyField) {
        var count = 0;
        var html = "";
        if (canEdit) {
            var clickTip = HtmlLang.Write(LangModule.Acct, "ClickTip", "Click to edit");
            html = "<div class='div-balance account-balance-edit' title='" + clickTip + "'>";
        } else {
            html = "<div>";
        }
        if (list && list.length > 0) {
            html += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='padding:5px'>";
            var oldCurrency = "";
            //币别类型
            var currencyType = 0;
            for (var i = 0 ; i < list.length; i++) {
                var json = list[i];
                var currency = json.MCurrencyID;
                if (oldCurrency != currency) {
                    currencyType++;
                    oldCurrency = currency;
                }
                var money = 0;
                var baseMoney = 0;

                for (var key in json) {
                    if (key == field) {
                        money = json[key];
                    }
                    //找原币
                    if (baseCurrencyField && key == baseCurrencyField) {
                        baseMoney = json[key];
                    }
                }
                //如果本位币有值，原币没有值，将本位值进行计算（因为存在特殊情况，只录本位币，不录原币）
                var exchange = AccountBalances.getExchange(currency);
                //if (baseMoney && !money) {
                //    money = exchange * baseMoney;
                //}

                html += "<tr>";
                html += "<td style='border:0px;text-align:left;  width:20%'><div class='id-for-li-currency'>" + currency + "</div></td><td  style='border:0px;text-align:right;color:#7EB8EA'>" + Megi.Math.toMoneyFormat(money) + "</td>";
                html += "</tr>";
                //取不到汇率，且没有传入baseMoney
                if (!exchange && baseMoney == 0) {
                    continue;
                }
                //如果存在本位币，就取本位币
                //count += baseMoney == 0 && exchange != 0 ? money / exchange : baseMoney;
                count += baseMoney;
            }
            //本位币不显示总计
            if (currencyType > 1 || oldCurrency != AccountBalances.baseCurrency) {
                html += "<tr><td style='border:0px; text-align:left; width:100px;color:#7EB8EA' nowrap>" + AccountBalances.baseCurrency + "(" + HtmlLang.Write(LangModule.GL, "Total", "合计") + ")</td><td style='border:0px;text-align:right;font-size:12px;color:#7EB8EA'>" + Megi.Math.toMoneyFormat(count) + "</td></tr>";
            }

            html += "</table>";
        } else {
            html += "&nbsp;";
        }
        html += "</div>"
        return html;
    },
    createEditBalance: function (selector, row, field, baseCurrencyField) {
        var list = row.Balances;
        var html = "<div id='currency-content' style='max-height:220px;overflow-y: auto;overflow-x:hidden;'>";
        var count = 0;
        //var isYtd = field == "MYtdDebitFor" || field == "MYtdCreditFor";
        //银行和现金,本年累计字段不能录入外币
        var isBankAccount = row.MCode != null && (row.MCode.indexOf("1001") == 0 || row.MCode.indexOf("1002") == 0);
        if (isBankAccount) {
            row.TempIsCheckForCurrency = false;
        } else {
            row.TempIsCheckForCurrency = row.MIsCheckForCurrency;
        }
        var currencyHtml = "";
        if (list && list.length > 0) {
            
            for (var i = 0 ; i < list.length; i++) {
                var json = list[i];
                var json = list[i];
                var currency = json.MCurrencyID;
                var money = 0;
                var baseMoney = 0;
                for (var key in json) {
                    if (key == field) {
                        money = json[key];
                    }

                    if (baseCurrencyField && key == baseCurrencyField) {
                        baseMoney = json[key];
                    }
                }

                //如果本位币有值，原币没有值，将本位值进行计算
                var exchange = AccountBalances.getExchange(currency);
                //if (baseMoney && !money) {
                //    money = exchange * baseMoney;
                //}
                //如果不是外币核算，不显示其他币别
                if (!isBankAccount && !row.TempIsCheckForCurrency && currency!=AccountBalances.baseCurrency) {
                    continue;
                }
                currencyHtml += AccountBalances.createCurrencyDiv(currency, money, false, row.TempIsCheckForCurrency, baseMoney);

                count += baseMoney;
            }
        } else {
            //没有默认插入一条空的
            var defalutCyID = isBankAccount ? AccountBalances.getBankCyId(row.id) : AccountBalances.baseCurrency
            currencyHtml += AccountBalances.createCurrencyDiv(defalutCyID, 0, true, row.TempIsCheckForCurrency, 0);
        }

        if (!currencyHtml) {
            var defalutCyID = isBankAccount ? AccountBalances.getBankCyId(row.id) : AccountBalances.baseCurrency
            currencyHtml += AccountBalances.createCurrencyDiv(defalutCyID, 0, true, row.TempIsCheckForCurrency, 0);
        }
        html += currencyHtml;
        html += "</div>";
        //当前选的币别等于组织的币别，不显示增加外币
        var sysCurrencyList = AccountBalances.currencyList;
        
        if (list && list.length < sysCurrencyList.length && sysCurrencyList.length > 1 && row.TempIsCheckForCurrency) {
            html += "<a id='btnAddCurrency' href='javascript:void(0);' style='padding-left:5px'>" + HtmlLang.Write(LangModule.Acct, "AddAnotherCurrency", "+ Add another currency") + "</a>";
        } else {
            html += "<a id='btnAddCurrency' href='javascript:void(0);' style='padding-left:5px;display:none'>" + HtmlLang.Write(LangModule.Acct, "AddAnotherCurrency", "+ Add another currency") + "</a>";
        }

        html += '<hr style=" border: 1px solid #E7E7E7;width:90%;"/>';
        html += '<div style="padding:5px;text-align:right">';
        html += '        <span>' + HtmlLang.Write(LangModule.Acct, "AmountBaseCurrency", "Amount(Base currency)") + ':</span></br>';
        html += '       <label id="lblBaseTotal">' + Megi.Math.toMoneyFormat(count) + '</label>';
        html += '   </div>';
        html += '<input id="btnCancel" class="m-lang-ok l-btn easyui-linkbutton-gray " style="float:left;height:24px;" type="button" value="' + HtmlLang.Write(LangModule.Acct, "Cancel", "Cancel ") + '">';
        html += '<input id="btnOK" class="m-lang-ok l-btn easyui-linkbutton-yellow " style="float:right;height:24px;" type="button" value="' + HtmlLang.Write(LangModule.Acct, "OK", "OK ") + '">';

        $(selector).empty();
        $(selector).attr("accountId", row.id);
        $(selector).attr("field", field);
        $(selector).append(html);

        AccountBalances.initCurrencyCbx();



        $("#btnAddCurrency").click(function () {
            var domCurrencyEdit = $("#divbalance");
            //币别编辑个数
            var divEditCount = $(".div-currency", "#divbalance").length;
            //大于系统外币时不允许新增
            if (divEditCount < AccountBalances.currencyList.length) {

                var html = AccountBalances.createCurrencyDiv("", 0, false, row.TempIsCheckForCurrency);
                $("#currency-content").append(html);
                AccountBalances.initCurrencyCbx();
                Megi.regClickToSelectAllEvt();
                //如果是向上显示的，需要调整币别编辑框的位置
                if (domCurrencyEdit.attr("dec") == "up") {
                    //小于两个币别编辑内容时执行
                    if (divEditCount < 2) {
                        var top = domCurrencyEdit.offset().top;
                        domCurrencyEdit.css("top", top - 110 + "px");
                    }
                }

                if (divEditCount + 1 == AccountBalances.currencyList.length) {
                    $("#btnAddCurrency").hide();
                }
            }

        });

        $("#btnCancel").click(function () {
            var dom = $("#divbalance");
            dom.empty();
            dom.hide();
            AccountBalances.dbClickRow = null;
            AccountBalances.dbClickField = null;
            AccountBalances.canSelectCurrencyList = AccountBalances.copyArray(AccountBalances.currencyList);
        });

        $("#btnOK").click(function () {
            //先找到编辑的行和编辑的列
            var accountId = $("#divbalance").attr("accountId");
            var field = $("#divbalance").attr("field");

            var row = $("#tbAccountBalance").treegrid("find", accountId);
            var updateList = new Array();
            var illegalBalanceList = new Array();

            $(".balance-money").each(function () {
                //选择了币别的才算数
                if ($(this).attr("cy")) {
                    var obj = {};

                    //如果MNumber为空,表示为联系人核算项目
                    if (!row.MNumber) {
                        obj.MAccountID = row._parentId;
                    }

                    obj.MAccountID = !row.MNumber ? row._parentId : row.id;

                    //0表示综合本分币，没有选用外币，此时所有的科目时不能进行编辑的
                    obj.MCurrencyID = $(this).attr("cy");
                    obj.MItemID = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MItemID");
                    if (!obj.MAccountID || obj.MAccountID == "0") {
                        return;
                    }
                    var baseCyDom = $(this).parent().find(".base-currency");
                    obj.MContactID = row.MContactID;

                    var originalBalance  =  $(this).val();
                    if (+originalBalance == 0 && +($(baseCyDom).val()) != 0) {

                        AccountBalances.getAccountBalanceObject(field, obj, row , $(this), baseCyDom);
                        illegalBalanceList.push(obj);
                    } else {
                        AccountBalances.getAccountBalanceObject(field, obj, row ,$(this), baseCyDom);
                        updateList.push(obj);
                    }
                }

            });

            if (illegalBalanceList.length > 0) {
                var tips = HtmlLang.Write(LangModule.Acct, "NotOriginalCurrency", "检测到没有填写原币金额，是否继续保存！");
                $.mDialog.confirm(tips, function () {
                    updateList = $.extend(updateList, illegalBalanceList);
                    var contactId = row.MContactID ? row.MContactID : "";
                    AccountBalances.saveBalance(updateList, contactId);

                });
            } else {
                var contactId = row.MContactID ? row.MContactID : "";
                AccountBalances.saveBalance(updateList, contactId);
            }
        });
    },
    //获取完整的余额对象：需要处理的列，余额对象，grid表格行，当前原币输入框dom对象, 本位币dom对象
    getAccountBalanceObject: function (field, balanceObject, row , balanceDom , baseCyDom) {
        var obj = balanceObject;
        if (field == "MInitBalanceFor") {
            obj.MInitBalanceFor = balanceDom.val();
            //更新本位币字段
            if (baseCyDom && baseCyDom.length > 0) {
                obj.MInitBalance = $(baseCyDom).val();

            } else {
                obj.MInitBalance = balanceDom.val();
            }
            obj.MYtdCredit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCredit");
            obj.MYtdCreditFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCreditFor");
            obj.MYtdDebit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebit");
            obj.MYtdDebitFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebitFor");
        } else if (field == "MYtdCreditFor") {
            obj.MContactID = row.MContactID;
            obj.MInitBalanceFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalanceFor");
            obj.MInitBalance = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalance");
            obj.MYtdCreditFor = balanceDom.val();

            if (baseCyDom && baseCyDom.length > 0) {
                obj.MYtdCredit = $(baseCyDom).val();

            } else {
                obj.MYtdCredit = balanceDom.val();
            }

            obj.MYtdDebit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebit");
            obj.MYtdDebitFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebitFor");
        } else if (field == "MYtdDebitFor") {
            obj.MContactID = row.MContactID;
            obj.MInitBalanceFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalanceFor");
            obj.MInitBalance = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalance");
            obj.MYtdCredit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCredit");
            obj.MYtdCreditFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCreditFor");
            obj.MYtdDebitFor = balanceDom.val();
            if (baseCyDom && baseCyDom.length > 0) {
                obj.MYtdDebit = $(baseCyDom).val();

            } else {
                obj.MYtdDebit = balanceDom.val();
            }
        }

        return obj;
    },
    //币别 ，金额，是否从系统后台获取的值，是否进行外币核算
    createCurrencyDiv: function (currency, money, isDefualt, isCheckForCurrency, baseMoney) {
        var html = "";
        html += '<div class="div-currency" style="padding:5px">';
        html += '<span>' + HtmlLang.Write(LangModule.Acct, "Currency", "Currency") + '</span></br>';
        var rand = Math.random();
        if ((currency && !isDefualt) || !isCheckForCurrency) {
            html += '<input class="balance"  cy="' + currency + '" disabled="disabled"/></br>';
            var index = AccountBalances.getArrayIndexByValue(AccountBalances.canSelectCurrencyList, currency);
            //AccountBalances.canSelectCurrencyList.splice(index, 1)
        } else {
            html += '<input class="balance" rand="' + rand + '" cy="' + currency + '"/></br>';
        }

        html += '<span>' + HtmlLang.Write(LangModule.Acct, "Amount", "Amount") + '</span></br>';


        html += '<input class="easyui-numberbox balance-money" style="text-align:right; height:20px;line-height:20" cy="' + currency + '" rand="' + rand + '" value="' + parseFloat(money).toFixed(2) + '"/>';
        //本位币金额
        if (currency != AccountBalances.baseCurrency) {
            html += '</br><span class="span-base-curreny">' + HtmlLang.Write(LangModule.Acct, "AmountBaseCurrency", "Amount(Base currency)") + '</span></br>';
            html += '<input class="easyui-numberbox base-currency" style="text-align:right; height:18px;line-height:18" cy="' + AccountBalances.baseCurrency + '" value="' + parseFloat(baseMoney).toFixed(2) + '"/>';
        } else {
            html += '</br><span class="span-base-curreny">' + HtmlLang.Write(LangModule.Acct, "AmountBaseCurrency", "Amount(Base currency)") + '</span></br>';
            html += '<input class="easyui-numberbox base-currency" style="text-align:right; height:18px;line-height:18" cy="' + AccountBalances.baseCurrency + '" value="' + parseFloat(money).toFixed(2) + '" data-options="disabled:true"/>';
        }

        html += '</div>';
        return html;
    },
    //初始化币别下拉框
    initCurrencyCbx: function () {

        $(".balance-money").each(function () {
            $(this).numberbox({
                precision: 2
            });
            $(this).css("height", "20px");
            $(this).css("line-height", "20px");
            $(this).keyup(function () {
                var rand = $(this).attr("rand");
                //alert(rand);
                AccountBalances.setBaseCurrencyTotal(0, rand);
            });
        });

        $(".base-currency").each(function () {
            $(this).numberbox({
                precision: 2
            });
            $(this).css("height", "20px");
            $(this).css("line-height", "20px");
            $(this).keyup(function () {
                AccountBalances.setBaseCurrencyTotal(1);
            });
        });

        $(".balance").each(function () {
            var bindData = null;
            if ($(this).attr("disabled") && $(this).attr("disabled") == "disabled") {
                bindData = AccountBalances.currencyList;
            } else {
                bindData = AccountBalances.canSelectCurrencyList;
            }
            $(this).combobox({
                textField: "MName",
                valueField: "MCurrencyID",
                data: bindData,
                //重新赋值币别
                onChange: function (newValue, oldValue) {
                    var currentDom = $(this);
                    $(this).attr("cy", newValue);

                    $(this).parent().find('.balance-money').attr("cy", newValue);
                    var baseCyDom = $(this).parent().find('.base-currency');
                    //如果不等于外币，增加本位币输入input
                    if (newValue != AccountBalances.baseCurrency) {
                        $(baseCyDom).numberbox("enable");
                    } else {
                        $(baseCyDom).numberbox("disable");
                    }

                    //可选择的combobox,如果当前框选中的币别在其他框存在，如果是不能输入了，就提示，如果还是可以输入就删除
                    var rand = $(this).attr("rand");
                    var currencyId = $(this).combobox("getValue");
                    if (rand) {
                        var otherCurrencyEidtDom = $(".balance[rand!='" + rand + "']");
                        if (otherCurrencyEidtDom && otherCurrencyEidtDom.length > 0) {
                            for (var i = 0; i < otherCurrencyEidtDom.length; i++) {
                                var dom = otherCurrencyEidtDom[i];
                                if ($(dom).hasClass("combobox-f") && $(dom).combobox("getValue") == currencyId) {

                                    if ($(dom).attr("disabled") == "disabled") {
                                        var tips = HtmlLang.Write(LangModule.Acct, "CurrecyExist", "币别已经存在,请选择其他币别");
                                        setTimeout(function () {
                                            $(currentDom).combobox("setValue", "");
                                            $(currentDom).attr("cy", "");
                                            $(currentDom).parent().find('.balance-money').attr("cy", "");
                                            $(currentDom).parent().find('.balance-money').numberbox("setValue", "0");
                                            $(currentDom).parent().find('.base-currency').numberbox("setValue", "0");
                                        }, 200);
                                        $.mDialog.message(tips);
                                    } else {
                                        $(dom).combobox("clear");
                                        $(dom).parent().find('.balance-money').attr("cy", "");
                                        $(dom).parent().find('.balance-money').numberbox("setValue","0");
                                        $(dom).parent().find('.base-currency').attr("cy", "");
                                        $(dom).parent().find('.base-currency').numberbox("setValue", "0");
                                    }

                                    
                                }
                            }
                        }
                    }
                    AccountBalances.setBaseCurrencyTotal(0, rand);
                },
                onLoadSuccess: function () {
                    //加载原来的选项
                    $(this).combobox("setValue", $(this).attr("cy"));
                }
            });
        });


    },
    //根据币别获取汇率
    getExchange: function (currencyId) {
        for (var i = 0; i < AccountBalances.currencyList.length; i++) {
            var oj = AccountBalances.currencyList[i];
            if (oj.MCurrencyID == currencyId) {
                return oj.MUserRate;
            }
        }
    },
    //根据币别获取余额
    getBalance: function (currencyId, balanceList, field) {
        for (var i = 0; i < balanceList.length; i++) {
            var oj = balanceList[i];
            if (oj.MCurrencyID == currencyId) {
                for (var key in oj) {
                    if (key == field) {
                        return oj[key];
                    }
                }
            }
        }
        return null;
    },
    getArrayIndexByValue: function (currencyList, currencyId) {
        if (currencyList && currencyList.length > 0) {
            for (var i = 0 ; i < currencyList.length; i++) {
                var currency = currencyList[i];
                if (currency.MCurrencyID == currencyId) {
                    return i;
                }
            }
        }
        return -1;
    },
    copyArray: function (array) {
        var temp = new Array();
        if (array) {
            for (var i = 0; i < array.length; i++) {
                temp.push(array[i]);
            }
        }
        return temp;
    },
    isCanEditBalance: function (accountType, field) {
        if ((accountType == "4" || accountType == "5") && field == "MInitBalanceFor") {
            return false;
        }

        return true;
    },
    //设置本位币金额,修改balance-money input 索引
    setBaseCurrencyTotal: function (type, index) {
        var total = 0.00;
        $(".balance-money").each(function () {
            var currencyId = $(this).attr("cy");
            var rand = $(this).attr("rand");
            if (currencyId) {
                var money = $(this).val() ? $(this).val() : 0.00;

                //对应的本位金额,用户可收到更改
                var baseCyDom = $(this).parent().find('.base-currency');

                var baseMoney = parseFloat(money) / AccountBalances.getExchange(currencyId)
                if (baseCyDom && baseCyDom.length > 0) {
                    if (type != 1 && index && rand == index) {
                        $(baseCyDom).numberbox("setValue", baseMoney);
                    } else {
                        baseMoney = parseFloat(baseCyDom.val());
                    }
                }

                total += baseMoney;
            }
        });
        $("#lblBaseTotal").text(total.toFixed(2));
    },
    isCanClickFiled: function (field) {
        var matchString = "MNumber ,text,MDC";
        //如果匹配到，不允许点击
        if (matchString.indexOf(field) >= 0) {
            return false;
        }
        return true;
    },
    //获取银行的币别
    getBankCyId: function (id) {
        var cyId = AccountBalances.baseCurrency;
        if (AccountBalances.bankList) {
            for (var i = 0; i < AccountBalances.bankList.length; i++) {
                var bankRow = AccountBalances.bankList[i];
                if (bankRow.MItemID == id) {
                    cyId = bankRow.MCyID;
                    break;
                }
            }
        }
        return cyId;
    },
    checkCustomAccountIsMatch: function (callback) {
        var url = "/BD/BDAccount/CheckCustomAccountIsMatch";
        $.mAjax.post(url, {}, function (data) {
            if (data && data.Success) {
                if (callback) {
                    callback();
                }
            } else {
                $.mDialog.alert(data.Message);
            }
        }, "", true);
    }

}
$(document).ready(function () {
    AccountBalances.init();

    var isSetup = $("#hideIsSetup").val();

    if (isSetup) {
        window.MReady = mWindow.reload;
    } else {
        window.MReady = AccountBalances.bindGrid;
    }
});