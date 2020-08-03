var AccountBalances = {
    dbClickRow: null,
    dbClickField:null,
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    //包含累计
    includeYTD: $("#hideGLBeginMonth").val() == 1,
    //用于其他页签刷新这个页签
    isHide: false,
    containner:null,
    currencyList: null,
    canSelectCurrencyList: null,
    baseCurrency: $("#hideBaseCy").val(),
    go: $("#hideGoService").val(),
    permisson: $("#hidePermission").val(),
    init: function () {
        var url = "/BD/BDAccount/AccountBalances";
        AccountBalances.containner = $(parent.document).find("div[data-url='" + url + "']");
        if (!AccountBalances.containner || AccountBalances.containner.length == 0) {
            url = AccountBalances.go + "/BD/Setup/GLOpeningBalance";
            AccountBalances.containner = $(parent.document).find("div[data-url='" + url + "']")
        }

        //var iframe = AccountBalances.containner.find("iframe");
        AccountBalances.isHide = AccountBalances.containner.css("display") == "none";


        if (AccountBalances.isHide) {
            //var src = iframe.attr("src");
            //iframe.attr("src", src);
            AccountBalances.containner.show();
        }


        AccountBalances.initUI();
        AccountBalances.initAction();
        AccountBalances.initCurrency();

        if (AccountBalances.isHide) {
            AccountBalances.containner.hide();
        }
    },
    initUI: function () {
        var forbidEdit = $("#hideForbidEdit").val() == "1";
        if (forbidEdit) {
            $("#btnReInit").show();
        } else {
            $("#btnFinish").show();
        }
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

       
        $("#btnTrialBalance").click(function () {
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Acct, "TrialBalance", "Trial balance check"),
                width: 500,
                height: 230,
                href: '/BD/BDAccount/TrialInitBalance'
            });
        });

        //结束初始化
        $("#btnFinish").click(function () {
            //先判断试算是否平衡



            Megi.Form.post({
                param: "",
                url: "/BD/BDAccount/CheckInitBalance", callback: function (msg) {
                    if (msg && msg.Success) {
                        //平衡时做其他操作
                        Megi.Form.post({
                            param: "",
                            url: "/BD/BDAccount/InitBalanceFinish", callback: function (msg) {
                                if (msg && msg.Success) {
                                    //平衡时做其他操作
                                    var message = HtmlLang.Write(LangModule.Acct, "FinishInitSuccess", "Finish Init Balance Successfully");
                                    $.mDialog.message(message);
                                    AccountBalances.reload(true);
                                } else {
                                    var message = msg.Message ? msg.Message : HtmlLang.Write(LangModule.Acct, "FinishInitFail", "Finish Init Balance Fail,please try again!");

                                    $.mDialog.alert(message);
                                }
                            }
                        });
                    } else {
                        //显示试算平衡值
                        $("#btnTrialBalance").trigger("click");
                    }
                }
            });
        });

        //重新初始化
        $("#btnReInit").click(function () {
            var confirmMsg = HtmlLang.Write(LangModule.Acct, "ReInitBalanceConfirm", "The re initialization of the balance will be the reverse audit all vouchers, and will affect the balance of accounts, you are sure to re - the initial balance?");
            $.mDialog.confirm(confirmMsg, function () {
                Megi.Form.post({
                    param: "",
                    url: "/BD/BDAccount/ReInitBalance", callback: function (msg) {
                        if (msg && msg.Success) {
                            var message = HtmlLang.Write(LangModule.Acct, "ReInitBalanceSuccess", "Operation successfully ,You can re-enter the balance");
                            $.mDialog.message(message);
                            AccountBalances.reload(true);
                        } else {
                            var message = msg.Message ? msg.Message : HtmlLang.Write(LangModule.Acct, "ReInitBalanceFail", "Operation Fail ,please try again!");
                            $.mDialog.alert(message);
                        }
                    }
                });
            });
        });

        $("#aExport").click(function () {
            window.location.href = '/BD/BDAccount/ExportOpeningBalances';
            $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
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
        Megi.Form.post({
            param: { isIncludeBase: true },
            url: "/BD/Currency/GetBDCurrencyList", callback: function (msg) {
                if (msg) {
                    //保存为全局变量
                    AccountBalances.currencyList = msg;
                    AccountBalances.canSelectCurrencyList = AccountBalances.copyArray(msg);
                    AccountBalances.bindGrid();
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
            url: "/BD/BDAccount/GetAccountInitBalanceTreeList?cyId=0&includeContact=true&t"+Math.random,
            queryParams: { group: objInfo.Title, searchFilter: searchFilter },
            idField: "id",
            treeField: 'text',
            fitColumns: false,
            scrollY: true,
            lines: true,
            columns: [[

            { title: LangKey.Code, field: 'MNumber', width: 100, align: 'left', sortable: true },
            {
                title: LangKey.Name, field: 'text', width: 300, sortable: true, formatter: function (value, rec, rowIndex) {

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
                        
                        if (!forbidEdit  && (!rec.children || rec.children.length == 0 || canAdd) && rec.MNumber && rec.IsCanRelateContact) {
                            //return "<a href='javascript:void(0);' style='line-height:18px;'>"+value+"</a>";
                            return value;
                        }
                    }
                    return value;
                }
            },

            {
                title: HtmlLang.Write(LangModule.Acct, "Direction", "Direction"), field: 'MDC', align: 'center', width: 100, sortable: true, formatter: function (value, rec, rowIndex) {
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
                title: HtmlLang.Write(LangModule.Acct, "InitialBalance", "Initial balance"), width: 200, align: 'right', field: "MInitBalanceFor", formatter: function (value, rec, rowIndex) {
                    var list = rec.Balances;
                    //是否可编辑
                    var canEdit = AccountBalances.permisson && !forbidEdit && (rec.children == null || rec.children.length == 0) && AccountBalances.isCanEditBalance(rec.MAccountGroupID, "MInitBalanceFor");
                    //银行科目应该也是不能出编辑框的
                    return AccountBalances.createBalanceTable(list, "MInitBalanceFor", canEdit, "MInitBalance");
                }
            },


            {
                title: HtmlLang.Write(LangModule.Acct, "CumulativeDebitThisYear", "Cumulative debit this year"),hidden:AccountBalances.includeYTD, align: 'right', width: 200, field: "MYtdDebitFor", formatter: function (value, rec, rowIndex) {
                    var list = rec.Balances;
                    var canEdit = AccountBalances.permisson && !forbidEdit && (rec.children == null || rec.children.length == 0) && AccountBalances.isCanEditBalance(rec.MAccountGroupID, "MYtdDebit");
                    return AccountBalances.createBalanceTable(list, "MYtdDebitFor", canEdit, "MYtdDebit");
                }
            },
             {
                 title: HtmlLang.Write(LangModule.Acct, "CumulativeCreditThisYear", "Cumulative credit this year"), hidden: AccountBalances.includeYTD, align: 'right', width: 200, field: "MYtdCreditFor", formatter: function (value, rec, rowIndex) {
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
                    Setup.adjustGridHeight();
                } else {
                    AccountBalances.autoHeight();
                }

                
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
                                domCurrencyEdit.css("top", top - domCurrencyEdit .height() - height + "px");
                                domCurrencyEdit.attr("dec", "up");
                            }
                            
                        }
                    }
                   
                });
            },
            onClickCell: function (field, row) {
                //如果是银行和现金科目，直接调整到银行初始录入界面

                if (!AccountBalances.isCanClickFiled(field)) {
                    return;
                }

                var node = row;
                if (!row.MNumber) {
                    node = $("#tbAccountBalance").treegrid("getParent", row.id);
                    $("#divbalance").hide();
                }

                if ((node.MCode.indexOf("1001") == 0 || node.MCode.indexOf("1002") == 0) && field == "MInitBalanceFor") {
                    var canEdit = $("#hideCanEditBankBalance").val();
                    if (!canEdit) {
                        return;
                    }
                    $.mTab.addOrUpdate(HtmlLang.Write(LangModule.BD, "BankAccountBalances", "BankAccount Balances"), AccountBalances.go+'/BD/BDBank/BankAccountBalances', true);
                } else if (( AccountBalances.specialAccountCodes.contains( node.MCode.substring(0,4))) && (field == "MInitBalanceFor" || field == "MYtdDebitFor" || field == "MYtdCreditFor")) {
                    //应收账款 应付账款 其他应收款
                    $("#divbalance").hide();
                    //
                    var titleLang = HtmlLang.Write(LangModule.Common, "AccountRelatedBillInit", "科目单据初始化");
                    $.mTab.addOrUpdate(titleLang, AccountBalances.go + '/BD/InitDocument/InitDocumentIndex', true);
                }else {
                    //进行定位，然后弹出框
                    AccountBalances.dbClickRow = row;
                    AccountBalances.dbClickField = field;
                    var node_id = row.id;
                    //先找到tr,在找td,在找div
                    var locationDiv = $(".datagrid-btable").find("tr[node-id='" + node_id + "']").find("td[field='" + field + "']").find(".div-balance");
                    if (!locationDiv || locationDiv.length == 0) {
                        //提示用户费用和损益不能录入初始余额
                        $("#divbalance").hide();

                        var tips = "";
                        if ($("#hideForbidEdit").val() == "1") {
                            tips = HtmlLang.Write(LangModule.Acct, "InitBalanceFinish", "Initialization is complete, can't input the balance!");
                            $.mDialog.message(tips);
                        } else {
                            if (!AccountBalances.permisson) {
                                tips = HtmlLang.Write(LangModule.Acct, "NoEditPermisson", "No edit permissions");
                            } else {
                                tips = HtmlLang.Write(LangModule.Acct, "CanNotInputInitBalance", "Parent Account ,expenses and Revenue can not enter the balance!");
                                $.mDialog.message(tips);
                            }
                        }
                        
                       
                        return;
                    }
                    var top = locationDiv.offset().top;


                    var left = locationDiv.offset().left;
                    var height = locationDiv.height();
                    var dom = $("#divbalance");

                    var baseCurrencyField = field.replace("For", "")

                    AccountBalances.createEditBalance(dom, row, field, baseCurrencyField);


                    //表格高度
                    var gridHeight = $(".datagrid-btable").height();
                    //如果表格高度-点击行位置的剩余高度小于币别编辑框高度，此时，币别编辑框应该向上显示
                    var viewH = $(".datagrid-body", ".datagrid-view2").height();//可见高度  
                    var contentH = $(".datagrid-body", ".datagrid-view2").get(0).scrollHeight;//内容高度
                    var scrollTop = $(".datagrid-body", ".datagrid-view2").scrollTop();//滚动高度 
                    //到达底部100px时,加载新内容 
                    if (contentH - viewH - scrollTop <= 0 && (viewH - top) < dom.height()) {
                        dom.css("position", "fixed");
                        dom.css("left", left + "px");
                        dom.css("top", top - dom.height() + "px");
                        dom.attr("dec", "down");
                    } else {
                        dom.css("position", "fixed");
                        dom.css("left", left + "px");
                        dom.css("top", top + height + "px");
                        dom.attr("dec", "down");
                    }
                    //滚动条滚动
                    //点击行到表头
                    var scrollTop = top - $(".datagrid-body", ".datagrid-view2").offset().top + $(".datagrid-body", ".datagrid-view2").scrollTop();
                    $(".datagrid-body", ".datagrid-view2").animate({ scrollTop: scrollTop }, 600);

                    dom.show();
                }

               
            }
        });
    },
    specialAccountCodes: ['1122', '2203', '2202', '1123', '1221', '2241'],
    fnArray: function () {
        AccountBalances.bindGrid();
        //$.mDialog.close();
    },
    reload: function () {
        window.location.reload(true);
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
    saveBalance: function (list,contactId) {
        $("#divbalance").mask();
        Megi.Ajax.post("/BD/BDAccount/UpdateInitBalance?contactId="+contactId, { modelList: list }, function (data) {
            if (!data.Success) {
                $.mAlert(data.Message);
            } else {
                $("#btnCancel").trigger("click");
                AccountBalances.bindGrid();
            }
            $("#divbalance").unmask();
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
    createBalanceTable: function (list, field,  canEdit,baseCurrencyField) {
        var count = 0;
        var html = "";
        if (canEdit) {
            var clickTip = HtmlLang.Write(LangModule.Acct, "ClickTip", "Click to edit");
            html ="<div class='div-balance' title='"+clickTip+"'>";
        } else {
            html = "<div>";
        }
        if (list && list.length > 0) {
            html += "<table border='0' cellpadding='0' cellspacing='0' width='100%' style='padding:5px'>";
            var oldCurrency = "";
            //币别类型
            var currencyType =0;
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
                //如果本位币有值，原币没有值，将本位值进行计算
                var exchange = AccountBalances.getExchange(currency);
                if (baseMoney && !money) {
                    money = exchange * baseMoney;
                }

                html += "<tr>";
                html += "<td style='border:0px;text-align:left;  width:10%'><span style='background-color: #61ABE4; color:white;'>" + currency + "</span></td><td  style='border:0px;text-align:right;color:#7EB8EA'>" + Megi.Math.toMoneyFormat(money) + "</td>";
                html += "</tr>";
                //取不到汇率，且没有传入baseMoney
                if (!exchange && baseMoney==0) {
                    continue;
                }
                //如果存在本位币，就取本位币
                count += baseMoney == 0 ? money / exchange : baseMoney;
            }
            //本位币不显示总计
            if (currencyType > 1 || oldCurrency!=AccountBalances.baseCurrency) {
                html += "<tr><td style='border:0px; text-align:left; width:10%;font-size:13px'>" + AccountBalances.baseCurrency + "(Total)</td><td style='border:0px;text-align:right;font-size:13px'>" + Megi.Math.toMoneyFormat(count) + "</td></tr>";
            }
            
            html += "</table>";
        } else {
            html += "&nbsp;";
        }
        html+="</div>"
        return html;
    },
    createEditBalance: function (selector, row, field,baseCurrencyField) {
        var list = row.Balances;
        var html = "<div id='currency-content' style='max-height:220px;overflow-y: auto;overflow-x:hidden;'>";
        var count = 0;
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
                if (baseMoney && !money) {
                    money = exchange * baseMoney;
                }

                html += AccountBalances.createCurrencyDiv(currency, money, false, row.MIsCheckForCurrency, baseMoney);

                count += baseMoney;
            }
        } else {
            //没有默认插入一条空的
            html += AccountBalances.createCurrencyDiv(AccountBalances.baseCurrency, 0, true, row.MIsCheckForCurrency,0);
        }
        
        html += "</div>";
        //当前选的币别等于组织的币别，不显示增加外币
        var sysCurrencyList = AccountBalances.currencyList;

        if (list && list.length < sysCurrencyList.length && sysCurrencyList.length > 1 && row.MIsCheckForCurrency) {
            html += "<a id='btnAddCurrency' href='javascript:void(0);' style='padding-left:5px'>" + HtmlLang.Write(LangModule.Acct, "AddAnotherCurrency", "+ Add another currency") + "</a>";
        } else {
            html += "<a id='btnAddCurrency' href='javascript:void(0);' style='padding-left:5px;display:none'>" + HtmlLang.Write(LangModule.Acct, "AddAnotherCurrency", "+ Add another currency") + "</a>";
        }
        
        html += '<hr style=" border: 1px solid #E7E7E7;width:90%;"/>';
        html += '<div style="padding:5px;text-align:right">';
        html += '        <span>' + HtmlLang.Write(LangModule.Acct, "AmountIn", "Amount in ") + AccountBalances.baseCurrency + ':</span></br>';
        html += '       <label id="lblBaseTotal">' + Megi.Math.toMoneyFormat(count) + '</label>';
        html += '   </div>';
        html += '<input id="btnCancel" class="m-lang-ok l-btn easyui-linkbutton-gray " style="float:left;height:24px;" type="button" value="' + HtmlLang.Write(LangModule.Acct, "Cancel", "Cancel ") + '">';
        html += '<input id="btnOK" class="m-lang-ok l-btn easyui-linkbutton-yellow " style="float:right;height:24px;" type="button" value="' + HtmlLang.Write(LangModule.Acct, "OK", "OK ") + '">';
       
        $(selector).empty();
        $(selector).attr("accountId", row.id);
        $(selector).attr("field",field);
        $(selector).append(html);
        
        AccountBalances.initCurrencyCbx();



        $("#btnAddCurrency").click(function () {
            var domCurrencyEdit = $("#divbalance");
            //币别编辑个数
            var divEditCount = $(".div-currency","#divbalance").length;
            //大于系统外币时不允许新增
            if (divEditCount < AccountBalances.currencyList.length) {
                
                var html = AccountBalances.createCurrencyDiv("", 0 , false , row.MIsCheckForCurrency);
                $("#currency-content").append(html);
                AccountBalances.initCurrencyCbx();
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
            AccountBalances.canSelectCurrencyList =  AccountBalances.copyArray(AccountBalances.currencyList);
        });

        $("#btnOK").click(function () {
            //先找到编辑的行和编辑的列
            var accountId = $("#divbalance").attr("accountId");
            var field = $("#divbalance").attr("field");

            var row = $("#tbAccountBalance").treegrid("find",accountId);
            var updateList = new Array();

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
                    if (field == "MInitBalanceFor") {
                        obj.MInitBalanceFor = $(this).val();
                        //更新本位币字段
                       
                        if (baseCyDom && baseCyDom.length > 0) {
                            obj.MInitBalance = $(baseCyDom).val();
                            
                        } else {
                            obj.MInitBalance = $(this).val();
                        }
                        obj.MYtdCredit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCredit");
                        obj.MYtdCreditFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCreditFor");
                        obj.MYtdDebit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebit");
                        obj.MYtdDebitFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebitFor");
                    } else if (field == "MYtdCreditFor") {
                        obj.MContactID = row.MContactID;
                        obj.MInitBalanceFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalanceFor");
                        obj.MInitBalance = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalance");
                        obj.MYtdCreditFor = $(this).val();

                        if (baseCyDom && baseCyDom.length > 0) {
                            obj.MYtdCredit = $(baseCyDom).val();

                        } else {
                            obj.MYtdCredit = $(this).val();
                        }

                        obj.MYtdDebit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebit");
                        obj.MYtdDebitFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdDebitFor");
                    } else if (field == "MYtdDebitFor") {
                        obj.MContactID = row.MContactID;
                        obj.MInitBalanceFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalanceFor");
                        obj.MInitBalance = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MInitBalance");
                        obj.MYtdCredit = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCredit");
                        obj.MYtdCreditFor = AccountBalances.getBalance(obj.MCurrencyID, row.Balances, "MYtdCreditFor");
                        obj.MYtdDebitFor = $(this).val();
                        if (baseCyDom && baseCyDom.length > 0) {
                            obj.MYtdDebit = $(baseCyDom).val();

                        } else {
                            obj.MYtdDebit = $(this).val();
                        }
                    }
                    updateList.push(obj);
                }
               
            });
            var contactId = row.MContactID ? row.MContactID : "";
            AccountBalances.saveBalance(updateList, contactId);
        });
    },
    //币别 ，金额，是否从系统后台获取的值，是否进行外币核算
    createCurrencyDiv: function (currency , money , isDefualt,isCheckForCurrency,baseMoney) {
        var html = "";
        html += '<div class="div-currency" style="padding:5px">';
        html += '<span>' + HtmlLang.Write(LangModule.Acct, "Currency", "Currency") + '</span></br>';
        var rand = Math.random();
        if ((currency && !isDefualt)|| !isCheckForCurrency) {
            html += '<input class="balance"  cy="' + currency + '" disabled="disabled"/></br>';
            var index = AccountBalances.getArrayIndexByValue(AccountBalances.canSelectCurrencyList, currency);
            //AccountBalances.canSelectCurrencyList.splice(index, 1)
        } else {
            html += '<input class="balance" rand="' + rand + '" cy="' + currency + '"/></br>';
        }
        
        html += '<span>' + HtmlLang.Write(LangModule.Acct, "Amount", "Amount") + '</span></br>';
         

        html += '<input class="easyui-numberbox balance-money" style="text-align:right; height:20px;line-height:20" cy="' + currency + '" rand="'+rand+'" value="' + parseFloat(money).toFixed(2) + '"/>';
        //本位币金额
        if (currency != AccountBalances.baseCurrency) {
            html += '</br><span>' + HtmlLang.Write(LangModule.Acct, "AmountBaseCurrency", "Amount(Base currency)") + '</span></br>';
            html += '<input class="easyui-numberbox base-currency" style="text-align:right; height:18px;line-height:18" cy="' + AccountBalances.baseCurrency + '" value="' + parseFloat(baseMoney).toFixed(2) + '"/>';
        }
     
        html += '</div>';
        return html;
    },
    //初始化币别下拉框
    initCurrencyCbx:function(){
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
                    $(this).attr("cy", newValue);

                    $(this).parent().find('.balance-money').attr("cy", newValue);
                    //可选择的combobox,如果当前框选中的币别在其他框存在，就清除他
                    var rand = $(this).attr("rand");
                    var currencyId = $(this).combobox("getValue");
                    if (rand) {
                        var otherCurrencyEidtDom = $(".balance[rand!='" + rand + "']");
                        if (otherCurrencyEidtDom && otherCurrencyEidtDom.length > 0) {
                            for (var i = 0; i < otherCurrencyEidtDom.length; i++) {
                                var dom = otherCurrencyEidtDom[i];
                                if ($(dom).hasClass("combobox-f") && $(dom).combobox("getValue") == currencyId) {
                                    $(dom).combobox("clear");
                                    $(dom).parent().find('.balance-money').attr("cy", "");
                                    $(dom).parent().find('.balance-money').val("0");
                                }
                            }
                        }
                    }
                    AccountBalances.setBaseCurrencyTotal(0,rand);
                },
                onLoadSuccess: function () {
                    //加载原来的选项
                    $(this).combobox("setValue", $(this).attr("cy"));
                }
            });
        });

        $(".balance-money").each(function () {
            $(this).numberbox({
                precision: 2
            });
            $(this).css("height", "20px");
            $(this).css("line-height","20px");
            $(this).keyup(function () {
                var rand = $(this).attr("rand");
                //alert(rand);
                AccountBalances.setBaseCurrencyTotal(0 , rand);
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
    },
    //根据币别获取汇率
    getExchange:function(currencyId){
        for(var i=0; i<AccountBalances.currencyList.length;i++){
            var oj = AccountBalances.currencyList[i];
            if(oj.MCurrencyID == currencyId){
                return oj.MUserRate;
            }
        }
    },
    //根据币别获取余额
    getBalance: function (currencyId, balanceList,field) {
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
    getArrayIndexByValue:function(currencyList, currencyId){
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
    isCanEditBalance: function (accountType , field) {
        if ((accountType == "4" || accountType == "5") && field == "MInitBalanceFor") {
            return false;
        }

        return true;
    },
    //设置本位币金额,修改balance-money input 索引
    setBaseCurrencyTotal: function (type ,index) {
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
    isCanClickFiled:function(field){
        var matchString = "MNumber ,text,MDC";
        //如果匹配到，不允许点击
        if (matchString.indexOf(field) >= 0) {
            return false;
        }
        return true;
    }
   
}
$(document).ready(function () {
    AccountBalances.init();
});