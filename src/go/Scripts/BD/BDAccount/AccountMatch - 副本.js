var MatchResultEnum = { None: 0, Matched: 1, AutoAdd: 2, ManualMatch: 3 };
var MatchMode = { Match: 1, Preview: 2 };
var i = 0;
var AccountMatch = {
    type: null,
    fileName: null,
    isCover: false,
    systemAccountList: null,
    unMatchSystemAccountList: null,
    matchResultList: null,
    selector: "#tbAcctList",
    previewSelector: "#tbPreviewList",
    txtNewNumberClass: ".new-number-txt",
    cbUnMatchSysAcctClass: ".unmatch-sys-account-cb",
    cbSysAcctClass: ".all-sys-account-cb",
    matchNumberKey: "MMatchNumber",
    newNumberKey: "MNewNumber",
    currentMode: MatchMode.Match,
    previewText: HtmlLang.Write(LangModule.Common, "Preview", "预览"),
    hasUnmatchData: false,
    changeByInitMatchLog: false,
    changeByInitNewLog: false,
    changeByInitDefaultMatch: false,
    isMatchChanged: false,
    newAcctOptText: HtmlLang.Write(LangModule.Common, "newaccount", "New Account"),
    //初始化是否完成
    isInited: false,
    //是否有数据需要转移
    isNeedTransfer: false,
    onSelectNewAccount: false,
    clearRelation: true,
    inconsistentMap: {},
    curCombo: null,
    isLoaded: false,
    cbCurrentSys: null,
    init: function () {
        AccountMatch.isCover = $("#hideIsCover").val().toUpperCase() === "TRUE";
        AccountMatch.type = $("#hidType").val();
        AccountMatch.fileName = $("#hidNewFileName").val();
        if (!AccountMatch.isCover && AccountMatch.type == "Account") {
            AccountMatch.bindGrid();
            AccountMatch.changeBtnText(AccountMatch.previewText);
            $.mDialog.min();
        }
        else {
            AccountMatch.showConfirmInfo(true);
        }
        AccountMatch.initAction();
    },
    initAction: function () {
        $(".mCloseBox", parent.document).click(function (e) {
            AccountMatch.confirmSaveMatchLog(function () {
                $.mDialog.close();
            });
            if (e.stopPropagation) {
                // this code is for Mozilla and Opera 
                e.stopPropagation();
            } else if (window.event) {
                // this code is for IE 
                window.event.cancelBubble = true;
            }
            return false;
        });

        $(window).resize(function () {
            AccountMatch.initUI();
        });
    },
    changeBtnText: function (text) {
        $("#aCompleteImport .l-btn-text").text(text);
    },
    switchMode: function (mode) {
        if (mode == MatchMode.Match) {
            AccountMatch.currentMode = MatchMode.Match;
            $("#divPreview").hide();
            if (AccountMatch.hasUnmatchData) {
                $("#divMatch").show();
            }
            else {
                $("#divConfirm").show();
            }
            AccountMatch.changeBtnText(AccountMatch.previewText);
            $.mDialog.min();
        }
        else {
            //检查断号
            if (AccountMatch.checkBrokenNumber()) {
                $.mDialog.confirm(HtmlLang.Write(LangModule.Acct, "ConfirmSortNewNumber", "新增的科目存在断号，是否重排?"),
                     function () {
                         //重排
                         AccountMatch.checkBrokenNumber(true);
                     },
                    function () {
                        //点“否”则直接预览
                        AccountMatch.previewMatch();
                    }
                 , false, true);
            }
            else {
                //不存在断号
                //移除重排标记
                $.each($(AccountMatch.txtNewNumberClass + ":visible"), function () {
                    AccountMatch.switchNewNumSortMark(this, false);
                });

                //检查父子级
                var msgList = AccountMatch.getInconsistentMatchErrMsgList();
                if (msgList.length > 0) {
                    var msg = msgList.toString() + "\n" + HtmlLang.Write(LangModule.Common, "AreYouContinue", "是否继续？");
                    $.mDialog.confirm(msg, function () {
                        AccountMatch.previewMatch();
                    }, undefined, undefined, false, true);
                }
                else {
                    AccountMatch.previewMatch();
                }
            }
        }
    },
    getInconsistentMatchErrMsgList: function () {
        AccountMatch.inconsistentMap = {};
        var cbMatchList = $(AccountMatch.cbUnMatchSysAcctClass + "[pnumber='undefined']");
        $.each(cbMatchList, function (i, cb) {
            var pUserNum = $(this).attr("number");
            var pNum = AccountMatch.getMatchOrNewVal($(this));
            var pChildList = $(AccountMatch.cbUnMatchSysAcctClass + "[pnumber='" + pUserNum + "']");
            AccountMatch.checkMatchRelation(pNum, pChildList);
        });

        var msgList = [];
        var msg = HtmlLang.Write(LangModule.Acct, "ChildCannotMatchOnceParentNew", "该科目：{0}无法与上级科目：{1}构成父子级关系");
        $.each(AccountMatch.inconsistentMap, function (pNum, cNums) {
            msgList.push(msg.replace("{0}", cNums).replace("{1}", pNum));
        });
        return msgList;
    },
    checkMatchRelation: function (pNum, pChildList) {
        var arrInconsistent = [];
        $.each(pChildList, function (j, cbC) {
            var cUserNum = $(cbC).attr("number");
            var cNum = AccountMatch.getMatchOrNewVal($(cbC));
            if (cNum.indexOf(pNum) == -1) {
                arrInconsistent.push(cNum);
            }

            var cChildList = $(AccountMatch.cbUnMatchSysAcctClass + "[pnumber='" + cUserNum + "']");
            if (cChildList.length > 0) {
                AccountMatch.checkMatchRelation(cNum, cChildList);
            }
        });
        if (arrInconsistent.length > 0) {
            AccountMatch.inconsistentMap[pNum] = arrInconsistent.join("、");
        }
    },
    confirmSaveMatchLog: function (callback) {
        if (AccountMatch.isMatchChanged) {
            $.mDialog.confirm(HtmlLang.Write(LangModule.Acct, "ConfirmSaveMatchLog", "科目匹配已经修改，是否保存后再离开?"),
                 function () {
                     AccountMatch.rebuildMatchData();
                     mAjax.submit(
                         "/BD/BDAccount/SaveAccountMatchLog",
                         { acctList: AccountMatch.matchResultList },
                         function (response) {
                             if (response.Success) {
                                 $.mMsg(HtmlLang.Write(LangModule.Acct, "SaveMatchSuccess", "科目匹配保存成功！"));
                                 AccountMatch.showMatchLogBtn();
                                 AccountMatch.isMatchChanged = false;
                                 if (callback != undefined) {
                                     callback();
                                 }
                             } else {
                                 $.mDialog.alert(response.Message);
                             }
                         });
                 },
                function () {
                    if (callback != undefined) {
                        callback();
                    }
                }
             , false, true);
        }
        else {
            if (callback != undefined) {
                callback();
            }
        }
    },
    rebuildMatchData: function () {
        $.each(AccountMatch.matchResultList, function (i, item) {
            if (item.MatchResult == MatchResultEnum.ManualMatch) {
                var userNumber = item.MNumber;
                var cbMatch = $(AccountMatch.cbUnMatchSysAcctClass + "[number='" + userNumber + "']");
                var newNumber = $(AccountMatch.txtNewNumberClass + "[number='" + userNumber + "']").val();
                var matchNumber = cbMatch.attr("loaded") == undefined ? (newNumber || $.trim(cbMatch.val()) == AccountMatch.newAcctOptText ? "" : cbMatch.val().split(' ')[0]) : cbMatch.combobox("getValue");

                if (matchNumber == "-1") {
                    matchNumber = "";
                }
                if (matchNumber) {
                    item.MMatchNumber = matchNumber;
                    item.MNewNumber = '';
                }
                else if (newNumber) {
                    item.MNewNumber = newNumber;
                    item.MMatchNumber = '';
                }
            }
        });
    },
    showUnMatchMsg: function (sender) {
        $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "UnCompleteMatch", "科目没完成匹配！"), function () {
            $(sender).combo("showPanel");
            $(sender).trigger("click");
        });
    },
    save: function () {
        var hasValidateError = false;
        if (AccountMatch.currentMode == MatchMode.Match) {
            //手动匹配
            var unMatchList = $(AccountMatch.cbUnMatchSysAcctClass);
            $.each(unMatchList, function (i, obj) {
                var matchVal = AccountMatch.getMatchVal($(this));
                var txtNew = $(this).closest("tr").find(AccountMatch.txtNewNumberClass);
                if (!matchVal && (txtNew.is(":hidden") || !txtNew.val())) {
                    AccountMatch.showUnMatchMsg(this);
                    hasValidateError = true;
                    return false;
                }
            });
            //新增的科目
            $.each($(AccountMatch.txtNewNumberClass), function () {
                AccountMatch.validateNewNumberEmpty(this);
                if ($(this).hasClass("validatebox-invalid")) {
                    AccountMatch.showUnMatchMsg(this);
                    hasValidateError = true;
                    return false;
                }
                else if (!AccountMatch.validateNewNumber(this)) {
                    hasValidateError = true;
                    return false;
                }
            });
            if (hasValidateError) {
                return;
            }
            if (!$("#divMatchList").mFormValidate()) {
                return;
            }
        }

        if (AccountMatch.currentMode == MatchMode.Match) {
            AccountMatch.switchMode(MatchMode.Preview);
            return;
        }

        AccountMatch.rebuildMatchData();

        if (AccountMatch.isNeedTransfer) {
            $.mDialog.confirm(HtmlLang.Write(LangModule.Acct, "confirmImportAccount", "You are adding first lower level account,system will put all data transferred to the first new child account.Are you continue?"),
            {
                callback: function () {
                    AccountMatch.executeSave();
                }
            });
        }
        else {
            AccountMatch.executeSave();
        }
    },
    executeSave: function () {
        mAjax.submit(
           "/BD/BDAccount/SaveAccountMatchResult",
           { list: AccountMatch.matchResultList },
           function (response) {
               if (response.Success) {
                   $.mMsg(HtmlLang.Write(LangModule.Common, "ImportSuccess", "导入成功！"));
                   if (parent && parent.AccountList) {
                       parent.AccountList.reloadFromImport();
                   }
                   if (AccountMatch.hasUnmatchData) {
                       AccountMatch.showMatchLogBtn();
                   }
                   $.mDialog.close();
               } else {
                   $.mDialog.alert(response.Message);
               }
           });
    },
    showMatchLogBtn: function () {
        //显示对照表按钮
        var item = $("#aMatchLog", parent.document).closest(".menu-item");
        if (item[0].style.display == "none") {
            item.show();
            var divWrapper = $("#divImport", parent.document);
            divWrapper.height(divWrapper.height() + 30);
        }
    },
    validateNewNumberEmpty: function (sender) {
        var val = $(sender).val();
        if (val || $(sender).is(":hidden")) {
            $(sender).removeClass("validatebox-invalid");
        }
        else {
            $(sender).addClass("validatebox-invalid");
        }

        if (!val && $(sender).next().attr("loaded") != undefined) {
            $(sender).next().combobox("setValue", "");
        }
    },
    //手动改变新科目编码时
    onNewNumberManualChanged: function (sender) {
        //当前新增科目代码
        setTimeout(function () {
            var newNumber = sender.value;
            if (newNumber && AccountMatch.validateNewNumber(sender, false)) {
                var userNumber = $(sender).attr("number");
                var parentNumber = AccountMatch.getParentNo(newNumber);
                var existNumberList = AccountMatch.getExistChildrenNumberList(parentNumber);
                //没值才处理
                //AccountMatch.setNewNumberVal(sender, newNumber);
                //校验重复（自动新增科目与系统科目）
                var filterList = existNumberList.filter(function (v) {
                    return v == newNumber;
                });
                if (filterList.length > 1) {
                    $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "MatchNumberDuplicate", "匹配不能重复！" + "(" + filterList[0] + ")"));
                    return;
                }
                //
                AccountMatch.updateMatchResult(userNumber, AccountMatch.newNumberKey, newNumber);
                //1.自动填充当前修改科目下的子级科目
                var curChildren = $(AccountMatch.txtNewNumberClass + "[pnumber='" + userNumber + "']");
                if (curChildren.length > 0) {
                    //自动填充所有子科目
                    AccountMatch.autoFillChildNewNumber(curChildren, undefined, newNumber);
                }
            }
        }, 300);
    },
    getExistChildrenNumberList: function (parentNumber, excludeNewNumber, isGetNewNumber) {
        //1.查找当前要下挂的系统科目的最大子级科目
        var sysNumberList = AccountMatch.getExistSysNumberList(parentNumber);
        //2.从自动新增的科目代码查找
        var autoNumberList = AccountMatch.getAutoNewNumberList(parentNumber);
        //3.从已新增的科目代码查找
        var newNumberList = isGetNewNumber == undefined || isGetNewNumber == true ? AccountMatch.getNewNumberList(parentNumber, excludeNewNumber) : [];

        return sysNumberList.concat(autoNumberList, newNumberList);
    },
    //更新新增科目下的所有子级科目
    autoFillChildNewNumber: function (curChildrenList, childrenList, parentNumber) {
        var list = childrenList || curChildrenList;
        $.each(list, function (i, obj) {
            i++;
            var oriVal = this.value;
            //如果没值就continue
            if (!oriVal) {
                return;
            }

            var parentSplit = parentNumber.split('.');
            var oriSplit = oriVal.split('.');
            //删除父级科目代码
            oriSplit.splice(0, parentSplit.length);
            var newNumber = parentNumber + '.' + oriSplit.join('.');
            AccountMatch.setNewNumberVal(this, newNumber);
            var userNumber = $(this).attr("number");
            var subChildrenList = $(AccountMatch.txtNewNumberClass + "[pnumber='" + userNumber + "']");

            if (subChildrenList.length > 0) {
                AccountMatch.autoFillChildNewNumber(curChildrenList, subChildrenList, parentNumber);
            }
        });
    },
    switchTxtValidateStatus: function (sender, isManualEdit) {
        AccountMatch.validateNewNumberEmpty(sender);
        var userNumber = $(sender).attr("number");
        var newNumber = $(sender).val();
        AccountMatch.updateMatchResult(userNumber, AccountMatch.newNumberKey, newNumber);

        //手动清空文本框时，清空选择
        if (isManualEdit && !newNumber) {
            var cbSys = $(AccountMatch.cbSysAcctClass + "[number='" + userNumber + "']");
            if (cbSys.attr("loaded") != undefined) {
                cbSys.combobox("setValue", "");
            }
        }
        if (isManualEdit) {
            AccountMatch.loadSysAcctList($(sender).next(), false, function () {
                var cbSys = $(sender).next();
                //如果有选择过，需要先清除选择，否则滚动条会滚到最底部
                if (cbSys.combobox("getValue")) {
                    AccountMatch.clearRelation = false;
                    cbSys.combobox("setValue", "");
                }
                cbSys.combo("showPanel");
                var panel = cbSys.combo("panel");
                panel.find(".no-match-records").remove();
                var comboItems = panel.find(".combobox-item");
                comboItems.show();
                var hideComboItems = comboItems.filter(function () {
                    return $(this).attr("title").indexOf(newNumber) == -1;
                });
                hideComboItems.hide();
                if (hideComboItems.length == comboItems.length) {
                    panel.append("<div class=\"no-match-records\" style=\"display: block;\">&nbsp;&nbsp;" + HtmlLang.Write(LangModule.Common, "NoMatchRecords", "No Matched Records!") + "</div>");
                }
            });
        }
    },
    bindGrid: function () {
        $("body").mask("");
        $(AccountMatch.selector).treegrid({
            url: '/BD/Import/GetImportAccountMatchResult',
            queryParams: { type: AccountMatch.type, fileName: AccountMatch.fileName },
            idField: 'MNumber',
            treeField: 'text',
            checkbox: true,
            fitColumns: true,
            singleSelect: false,
            scrollY: true,
            lines: true,
            region: "center",
            columns: [[
                { title: HtmlLang.Write(LangModule.Acct, "ImportAccountCode", "导入科目代码"), field: 'MNumber', width: 130, sortable: false },
                {
                    title: HtmlLang.Write(LangModule.Acct, "ImportAccountName", "导入科目名称"), field: 'text', width: 238, sortable: false,
                    formatter: function (value, rec, rowIndex) {
                        return mText.encode(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Acct, "SystemAccount", "系统科目名称"), field: 'MItemID', sortable: false,
                    formatter: function (value, row, index) {
                        if (row.MNumber) {
                            return "<input type='text' class='unmatch-sys-account-cb easyui-combobox' data-options='required:true' onfocus='AccountMatch.loadUnMatchAcctList(this, true);' pnumber='" + row._parentId + "' number='" + row.MNumber + "' initval='" + row.MMatchNumber + "' style='width:230px;height:25px;border:1px solid #c2cbd3;'/>";
                        }
                        return "";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Acct, "NewAccountCode", "新增科目代码"), field: 'MNewNumber', width: 238, sortable: false,
                    formatter: function (value, row, index) {
                        if (row.MNumber) {
                            //去掉了keyup事件，写在代码里面
                            //return "<div class='new-number-wrapper' number='" + row.MNumber + "' style='display:none;'><input number='" + row.MNumber + "' initval='" + row.MNewNumber + "' pnumber='" + row._parentId + "' class='easyui-textbox new-number-txt' data-options='required:true' onkeyup='AccountMatch.switchTxtValidateStatus(this, true);' onblur='AccountMatch.onNewNumberManualChanged(this);' style='width:185px;padding-right:3px;height:25px;border:1px solid #c2cbd3;'/><input class='all-sys-account-cb' style='width:0px;opacity:0;height: 27px;border:1px solid #c2cbd3;border-left-width:0px;' defaultWidth='268' number='" + row.MNumber + "'/><a class='m-search-icon' href='javascript:void(0)' style='width:16px;height:16px;display:inline-block;vertical-align:middle;' onclick='AccountMatch.onSearchClick(this, true);'></a></div>";
                            return "<div class='new-number-wrapper' number='" + row.MNumber + "' style='display:none;'><input number='" + row.MNumber + "' initval='" + row.MNewNumber + "' pnumber='" + row._parentId + "' class='easyui-textbox new-number-txt' data-options='required:true'  style='width:185px;padding-right:3px;height:25px;border:1px solid #c2cbd3;'/><input class='all-sys-account-cb' style='width:0px;opacity:0;height: 27px;border:1px solid #c2cbd3;border-left-width:0px;' defaultWidth='268' number='" + row.MNumber + "'/><a class='m-search-icon' href='javascript:void(0)' style='width:16px;height:16px;display:inline-block;vertical-align:middle;' ></a></div>";
                        }
                        return "";
                    }
                }
            ]],
            onLoadSuccess: function (row, data) {

                // felson 2018.6.5 把new-number-txt输入框的keyup事件移到这里
                $(AccountMatch.txtNewNumberClass).off("keyup.am").on("keyup.am", function () {

                    AccountMatch.switchTxtValidateStatus(this, true)
                }).off("blur.am").on("blur.am", function () {
                    AccountMatch.onNewNumberManualChanged(this);
                });

                $(".m-search-icon").off("click.am").on("click.am", function () {
                    AccountMatch.onSearchClick(this, true);
                });

                var cbUnMatch = $(AccountMatch.cbUnMatchSysAcctClass).filter(function () {
                    var initVal = $(this).attr("initval");
                    return !initVal || initVal == 'null';
                });
                $.each(cbUnMatch, function () {
                    AccountMatch.bindCombobox(this, {
                        width: 252
                    }, false);
                });
                $(".combo input[class*='validatebox-invalid']").removeClass("validatebox-invalid");
                $("body").unmask();
                AccountMatch.matchResultList = data[0].MatchResultList;
                AccountMatch.systemAccountList = data[0].SystemAccountList;

                var matchedList = AccountMatch.getMatchResultList(MatchResultEnum.Matched);
                var manualMatchList = AccountMatch.getMatchResultList(MatchResultEnum.ManualMatch);
                AccountMatch.hasUnmatchData = manualMatchList.length > 0 && data[0].MNumber;
                AccountMatch.showConfirmInfo(!AccountMatch.hasUnmatchData);

                var arrMatchedUserNumber = [];
                $.each(matchedList, function (i, userAcct) {
                    arrMatchedUserNumber.push(AccountMatch.getStandardNo(userAcct.MNumber));
                });
                AccountMatch.unMatchSystemAccountList = AccountMatch.systemAccountList.filter(function (el) {
                    return $.inArray(el.MNumber, arrMatchedUserNumber) == -1;
                });
                AccountMatch.hidComboPanelByScroll();
                if (!AccountMatch.hasUnmatchData) {
                    return;
                }

                var autoAddCount = AccountMatch.getMatchResultList(MatchResultEnum.AutoAdd).length;
                $("#spMatchMsg").text(HtmlLang.Write(LangModule.Acct, "ImportAccountMatchTip", "已经匹配到{0}个科目，{1}个科目可自动新增，{2}个未匹配到的科目需要手动匹配：")
                    .replace("{0}", matchedList.length).replace("{1}", autoAddCount).replace("{2}", manualMatchList.length));

                //一级科目有70%匹配不上则提示是否使用自定义会计准则
                AccountMatch.showTipByFirstLevelUnMatchCount();

                //加载预设数据
                AccountMatch.loadInitMatchList();
                Megi.regClickToSelectAllEvt();
            },
            onClickRow: function (row) {
                $(this).treegrid('cascadeCheck', {
                    id: row.id, //节点ID  
                    deepCascade: true //深度级联  
                });
            },
            onClickCell: function (field, index, value) {
                $(this).treegrid("unselectAll");
            },
            rowStyler: function (row, Index) {
                i = i + 1;
                if (i % 2 == 0) {
                    i = 0;
                    return 'background-color:rgb(229,240,245);';
                }
            },
            onBeforeCollapse: function (row) {
                //阻止折叠，不然:visible取不到
                return false;
            }
        });
    },
    onSearchClick: function (sender, isManual) {
        var cbSys = $(sender).closest("td").find(AccountMatch.cbSysAcctClass)[0];
        AccountMatch.loadSysAcctList(cbSys, isManual);
    },
    hidComboPanelByScroll: function () {
        $(".datagrid-body").scroll(function () {
            if (AccountMatch.curCombo && AccountMatch.curCombo.length == 1) {
                AccountMatch.curCombo.combo("hidePanel");
            }
        });
    },
    //新增科目代码的下拉框样式
    setNewAcctNumberComboUI: function (cbSys) {
        var container = $(cbSys).parent();
        container.find(".combo").css({ "width": "0px", "border-left": "0px", "opacity": "0" });
        container.find(".combo .combo-text").css({ "display": "none", "width": "0px", "padding-right": "0px" });
    },
    //一级科目有70%匹配不上则提示是否使用自定义会计准则
    showTipByFirstLevelUnMatchCount: function () {
        var firstLevelAcctList = AccountMatch.matchResultList.filter(function (el) {
            return el.MatchResult == MatchResultEnum.ManualMatch && el.MNumber.length == 4;
        });
        if (firstLevelAcctList.length / AccountMatch.matchResultList.length >= 0.7) {
            $.mDialog.confirm(HtmlLang.Write(LangModule.Acct, "UseCustomiseAccountWarning", "导入的科目表有{0}个一级科目不能与系统进行匹配，是否需要使用自定义式会计准则？").replace("{0}", firstLevelAcctList.length),
            {
                callback: function () {
                    parent.location.href = "/BD/Setup/FinancialSetting"
                }
            });
        }
    },
    setNewNumberVal: function (sender, val) {
        $(sender).closest(".new-number-wrapper").show();
        $(sender).val(val);
        var userNumber = $(sender).attr("number");
        var cbUnMatch = $(AccountMatch.cbUnMatchSysAcctClass + "[number='" + userNumber + "']");
        if (cbUnMatch.attr("loaded") === undefined) {
            var comboTxt = cbUnMatch.closest("td").find(".combo-text");
            if (comboTxt.length == 1) {
                comboTxt.val(AccountMatch.newAcctOptText);
            }
            else {
                cbUnMatch.val(AccountMatch.newAcctOptText);
            }
            //combo.find("combo-value").val(-1);
        }
        else {
            cbUnMatch.combobox("setValue", -1);
        }
        //Megi.regClickToSelectAllEvt();
    },
    loadInitMatchList: function () {
        var txtNewNumbers = $(AccountMatch.txtNewNumberClass);
        txtNewNumbers.each(function (i, txt) {
            var initVal = $(this).attr("initval");
            var userNumber = $(this).attr("number");
            var cbUnMatch = $(AccountMatch.cbUnMatchSysAcctClass + "[number='" + userNumber + "']");
            if (initVal && initVal != "null") {
                AccountMatch.setNewNumberVal(this, initVal);
            }
            else {
                if (cbUnMatch.attr("loaded") == undefined) {
                    var initMatch = cbUnMatch.attr("initval");
                    if (initMatch && initMatch != "null") {
                        var standNo = AccountMatch.getStandardNo(initMatch);
                        //"1122":只有名称匹配的预设值; "1122 应收账款"：数据库保存的匹配记录
                        if (initMatch.length == standNo.length) {
                            AccountMatch.changeByInitDefaultMatch = true;
                            AccountMatch.loadUnMatchAcctList(cbUnMatch, false, undefined, false);
                        }
                        else {
                            cbUnMatch.val(initMatch);
                        }
                    }
                }
            }
            if (i == txtNewNumbers.length - 1) {
                AccountMatch.isInited = true;
            }
        });
    },
    loadUnMatchData: function (sender) {
        $(sender).hide();
        $(sender).prev().trigger('click');
    },
    loadUnMatchAcctList: function (cbObj, showPanel, callback, isInited) {
        if (AccountMatch.cbCurrentSys != null) {
            AccountMatch.cbCurrentSys.combo("hidePanel");
        }
        if ($(cbObj).attr("loaded") == undefined) {
            var oldVal;
            var newOption = [{ MNumber: "-1", MFullName: AccountMatch.newAcctOptText }];
            var unMatchData = newOption.concat(AccountMatch.unMatchSystemAccountList);
            AccountMatch.bindCombobox(cbObj, {
                width: 252,
                data: unMatchData,
                onSelect: function (record) {
                    if (isInited != undefined && !AccountMatch.isLoaded) {
                        AccountMatch.isInited = isInited;
                    }
                    AccountMatch.onManualMatchSelect(this, oldVal);
                },
                //手动修改或加载预设值
                onChange: function (newValue, oldValue) {
                    if (isInited != undefined && !AccountMatch.isLoaded) {
                        AccountMatch.isInited = isInited;
                    }
                    if (AccountMatch.changeByInitNewLog || AccountMatch.changeByInitDefaultMatch) {
                        AccountMatch.onManualMatchSelect(this);
                        AccountMatch.changeByInitNewLog = false;
                        AccountMatch.changeByInitDefaultMatch = false;
                    }
                    else {
                        AccountMatch.isMatchChanged = true;
                    }
                },
                onLoadSuccess: function () {
                    var initVal = $(this).attr("initval");
                    if (initVal && initVal != "null") {
                        AccountMatch.changeByInitMatchLog = true;
                        $(this).combobox("setValue", initVal.split(' ')[0]);
                        AccountMatch.changeByInitMatchLog = false;
                    }
                    else {
                        var userNumber = $(this).attr("number");
                        var newNumber = $(AccountMatch.txtNewNumberClass + "[number='" + userNumber + "']").val();
                        if (newNumber) {
                            $(this).combobox("setValue", -1);
                        }
                    }
                    $(this).attr("loaded", 1);
                    //$(this).next().next(".combo-arrow").hide();
                    if (showPanel) {
                        $(this).combo("showPanel");
                        $(this).parent().find(".combo-text").focus();
                    }
                    if (callback) {
                        callback();
                    }
                    AccountMatch.isInited = true;
                },
                onShowPanel: function () {
                    AccountMatch.setInitedFlag();
                    oldVal = $(this).combobox("getValue");
                    if (oldVal == "-1") {
                        oldVal = $(this).closest("tr").find(AccountMatch.txtNewNumberClass).val();
                    }
                    AccountMatch.curCombo = $(this);
                    AccountMatch.setComboPanelShadow($(this));
                }
            });
        }
        else {
            if (callback) {
                callback();
            }
        }
    },
    setInitedFlag: function () {
        if (!AccountMatch.isInited) {
            AccountMatch.isInited = true;
        }
        AccountMatch.isLoaded = true;
    },
    showComboPanel: function (sender) {
        var comboPanel = $(sender).combo("panel");
        if (comboPanel.is(":hidden")) {
            $(sender).combo("showPanel");
        }
    },
    setComboPanelShadow: function (cb) {
        cb.combo("panel").parent().css({ "-moz-box-shadow": "0px 0px 5px #333333", "-webkit-box-shadow": "0px 0px 5px #333333", "box-shadow": "0px 0px 5px #333333" });
    },
    loadSysAcctList: function (cbObj, showPanel, callback) {
        if ($(cbObj).attr("loaded") == undefined) {
            AccountMatch.bindCombobox(cbObj, {
                left: 500,
                data: AccountMatch.systemAccountList,
                hasDownArrow: false,
                onChange: function (newValue, oldValue) {
                    AccountMatch.onAppendMatchSelect(this);
                    AccountMatch.clearRelation = true;
                },
                onLoadSuccess: function () {
                    AccountMatch.adjustSysAcctComboWidth($(this));
                    $(this).attr("loaded", 1);
                    AccountMatch.setNewAcctNumberComboUI(this);
                    //$(this).closest(".new-number-wrapper").find(".combo-arrow:eq(0)").hide();
                    if (showPanel) {
                        $(this).combo("showPanel");
                    }
                    if (callback != undefined) {
                        callback();
                    }
                    AccountMatch.hidComboPanelByScroll();
                },
                onShowPanel: function () {
                    AccountMatch.cbCurrentSys = $(this);
                    AccountMatch.adjustSysAcctComboWidth($(this));
                    //有选择，但是文本框没值
                    //var txtNewNumber = $(this).closest(".new-number-wrapper").find(AccountMatch.txtNewNumberClass);
                    if ($(this).combobox("getValue")) {//!txtNewNumber.val() && 
                        AccountMatch.clearRelation = false;
                        $(this).combobox("setValue", "");
                    }
                    $(this).combo("panel").height(198);
                    AccountMatch.curCombo = $(this);
                    AccountMatch.setComboPanelShadow($(this));
                }
            });
        }
        else if (callback != undefined) {
            callback();
        }
        else if (showPanel) {
            $(cbObj).combo("showPanel");
        }
    },
    adjustSysAcctComboWidth: function (cb) {
        //cb.combo("panel").width(273);
    },
    getMatchResultList: function (matchResult) {
        return AccountMatch.matchResultList.filter(function (el) {
            return el.MatchResult == matchResult;
        });
    },
    //检查断号
    checkBrokenNumber: function (isReSort, isMark, parentNumber) {
        var existBroke = false;
        var iteratedParentList = [];
        var txtNumbers = $(AccountMatch.txtNewNumberClass + ":visible");
        if (parentNumber) {
            txtNumbers = txtNumbers.filter(function () {
                return $(this).val().indexOf(parentNumber) == 0;
            });
        }
        $.each(txtNumbers, function (i, txt) {
            var val = $(this).val();
            var pNum = AccountMatch.getParentNo(val);
            //父级相同则跳过
            if ($.inArray(pNum, iteratedParentList) != -1) {
                return;
            }

            //检查与当前科目同级科目/同级的下级科目是否断号
            //所有同级科目
            var currentChildNumList = AccountMatch.getExistChildrenNumberList(pNum);
            var visibleChildNumList = AccountMatch.getNewNumberList(pNum);
            //找到最大的末级编码
            var arrSubNo = AccountMatch.getCurrentSubNoList(currentChildNumList);
            //检查断号
            if (AccountMatch.existBrokeNumByParent(visibleChildNumList, arrSubNo)) {
                if (isReSort) {
                    AccountMatch.sortNewNumber(pNum, isMark);
                }
                else {
                    existBroke = true;
                    return false;
                }
            }
            else {
                //移除标记
                AccountMatch.switchNewNumSortMark(this, false);
            }
            //记录已遍历的科目代码
            iteratedParentList.push(pNum);
        });
        if (isReSort && isMark != false) {
            $.mMsg(HtmlLang.Write(LangModule.Acct, "SortSuccess", "重排成功！"));
        }
        return existBroke;
    },
    existBrokeNumByParent: function (visibleChildNumList, currentSubNumList) {
        var existBroke = false;
        var maxSubNo = AccountMatch.getMaxSubNo(visibleChildNumList);
        for (var i = maxSubNo; i > 0; i--) {
            if ($.inArray(i, currentSubNumList) == -1) {
                existBroke = true;
                break;
            }
        }
        return existBroke;
    },
    getCurrentSubNoList: function (currentChildNumList) {
        var arrSubNo = [];
        $.each(currentChildNumList, function (i, v) {
            var arrSplit = v.split('.');
            var subNo = arrSplit[arrSplit.length - 1];
            arrSubNo.push(+subNo);
        });
        return arrSubNo;
    },
    getMaxSubNo: function (currentChildNumList) {
        var arrSubNo = AccountMatch.getCurrentSubNoList(currentChildNumList);
        return Math.max.apply(null, arrSubNo);
    },
    //获取指定科目的末级编码，默认转整型
    getSubNo: function (number, getOriVal, subNoIdx) {
        var arrSplit = number.split('.');
        var idx = subNoIdx != undefined ? subNoIdx : arrSplit.length - 1;
        var subNo = arrSplit[idx];
        //是否取原始
        if (getOriVal) {
            return subNo;
        }
        return +subNo;
    },
    getAutoNewNumber: function (pNumber, subNo) {
        var subNoStr = subNo < 10 ? "0" + subNo : subNo.toString();
        return pNumber.concat("." + subNoStr);
    },
    setSubNo: function (sender, newSubNo, subNoIdx) {
        var number = $(sender).val();
        var arrSplit = number.split('.');
        var idx = subNoIdx != undefined ? subNoIdx : arrSplit.length - 1;
        var curSubNo = arrSplit[idx];

        var arrPad = [];
        for (var i = 0; i < curSubNo.length; i++) {
            arrPad.push('0');
        }

        var newSubNoStr = (arrPad.toString() + newSubNo).slice(-arrPad.length);

        arrSplit[idx] = newSubNoStr;
        $(sender).val(arrSplit.join('.'));
    },
    //获取指定科目下的所有子科目的末级编码列表
    getSubNoList: function (pNumber, includeNewNumber) {
        var subNoList = [];
        var currentChildNumList = AccountMatch.getExistChildrenNumberList(pNumber, undefined, includeNewNumber);
        $.each(currentChildNumList, function (i, v) {
            subNoList.push(AccountMatch.getSubNo(v));
        });
        return subNoList;
    },
    getNextSubNoByParent: function (pNumber, includeNewNumber) {
        var existSubNoList = AccountMatch.getSubNoList(pNumber, includeNewNumber);
        var maxSubNo = existSubNoList.length == 0 ? 0 : Math.max.apply(null, existSubNoList);
        return maxSubNo + 1;
    },
    //重排
    sortNewNumber: function (pNumber, isMark) {
        //所有下一级子科目
        var txtChildren = $(AccountMatch.txtNewNumberClass + ":visible").filter(function () {
            return AccountMatch.getParentNo($(this).val()) == pNumber;
        });
        if (txtChildren.length > 0) {
            var subNoList = AccountMatch.getSubNoList(pNumber);
            $.each(txtChildren, function (i, txt) {
                var val = $(this).val();
                var curSubNo = AccountMatch.getSubNo(val, true);
                var isMarkTmp = false;
                //当前末级科目位数
                var arrPad = [];
                for (var i = 0; i < curSubNo.length; i++) {
                    arrPad.push('0');
                }

                for (var i = 1; i < +curSubNo; i++) {
                    //有断号
                    if ($.inArray(i, subNoList) == -1) {
                        var newSubNo = (arrPad.toString() + i).slice(-arrPad.length);
                        var newVal = pNumber.concat("." + newSubNo);
                        $(this).val(newVal);
                        //触发所有子科目联动
                        $(this).trigger("blur");
                        //标记重排
                        isMarkTmp = true;

                        //重排后，重新获取末级编码列表
                        subNoList = AccountMatch.getSubNoList(pNumber);
                        break;
                    }
                }

                if (isMark === false) {
                    isMarkTmp = false;
                }
                AccountMatch.switchNewNumSortMark(this, isMarkTmp);
            });
        }
    },
    //切换排序标记
    switchNewNumSortMark: function (sender, isMark) {
        var color = isMark ? "green" : "#E9E9E9";
        var val = $(sender).val();
        var txtChildList = $(AccountMatch.txtNewNumberClass + ":visible").filter(function () {
            return $(this).val().indexOf(val) == 0;
        });

        $.each(txtChildList, function (i, txt) {
            $(this).closest("td").css({ "border": "1px solid " + color });
        });
    },
    previewMatch: function () {
        $("body").mask("");
        AccountMatch.rebuildMatchData();
        $(AccountMatch.previewSelector).treegrid({
            url: '/BD/BDAccount/PreviewAccountMatch',
            queryParams: { json: encodeURIComponent($.toJSON(AccountMatch.matchResultList)) },
            idField: 'MNumber',
            treeField: 'text',
            checkbox: true,
            fitColumns: true,
            singleSelect: false,
            scrollY: true,
            lines: true,
            region: "center",
            columns: [[
            { title: LangKey.Code, field: 'MNumber', width: 50, sortable: false },
            {
                title: LangKey.Name, field: 'text', width: 200, sortable: false, formatter: function (value, rec, rowIndex) {
                    return value;
                }
            },

            { title: LangKey.Type, field: 'MAccountTypeID', width: 120, align: 'center', sortable: false },

            {
                title: HtmlLang.Write(LangModule.Acct, "Direction", "Direction"), field: 'MDC', width: 100, align: 'center', sortable: false, formatter: function (value, rec, rowIndex) {
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
                title: HtmlLang.Write(LangModule.Acct, "CurrencyAccountingWay", "外币核算方式"), field: 'MIsCheckForCurrency', width: 100, align: 'center', sortable: false, formatter: function (value, rec, rowIndex) {
                    if (value) {
                        return HtmlLang.Write(LangModule.Acct, "AccountForeignCurrency", "Account foreign currency");
                    }
                    else {
                        return HtmlLang.Write(LangModule.Acct, "NoAccountForeignCurrency", "Non-currency accounting");
                    }
                }
            },
            {
                title: HtmlLang.Write(LangModule.Acct, "CheckType", "核算维度"), field: 'MCheckGroupNames', width: 100, align: 'left', sortable: false, formatter: function (value, rec, rowIndex) {
                    return mText.encode(value);
                }
            }
            ]],
            onLoadSuccess: function (row, data) {
                $("body").unmask();
                AccountMatch.isNeedTransfer = data[0].IsNeedTransfer;
                if (data[0].Message) {
                    $.mDialog.alert(data[0].Message);
                }
                else {
                    AccountMatch.currentMode = MatchMode.Preview;
                    $("#divPreview").show();
                    $("#divMatch").hide();
                    $("#divConfirm").hide();
                    AccountMatch.changeBtnText(HtmlLang.Write(LangModule.Common, "Save", "Save"));
                    $.mDialog.max();
                    AccountMatch.initUI(AccountMatch.previewSelector);
                }
            },
            onClickCell: function (field, index, value) {
                $(this).treegrid("unselectAll");
            }
        });
    },
    //校验新增科目格式
    validateNewNumber: function (sender, showMsg) {
        var val = $.trim(sender.value);
        if ($(sender).is(":hidden") || !val) {
            return true;
        }
        if (showMsg == undefined) {
            showMsg = true;
        }

        var formatterErrorTips = HtmlLang.Write(LangModule.Acct, "AccountNumberFormatterError", "新增的科目代码:{0}格式不正确！");
        formatterErrorTips = formatterErrorTips.replace("{0}", val);
        //校验最后一个字符是不是数字
        var lastChar = val.charAt(val.length - 1);
        if (isNaN(lastChar)) {
            if (showMsg) {
                $.mDialog.alert(formatterErrorTips, function () {
                    $(sender).focus();
                });
            }
            return false;
        }

        //校验去掉.分隔符后是否是数字
        var tempNumber = val.replace(/\./g, "");

        if (isNaN(tempNumber)) {
            if (showMsg) {
                $.mDialog.alert(formatterErrorTips, function () {
                    $(sender).focus();
                });
            }
            return false;
        }

        if (val.indexOf('.') == -1) {
            if (showMsg) {
                $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "NewNumberWithoutDot", "新增的科目代码格式不正确，请使用点号(.)分隔！"), function () {
                    $(sender).focus();
                });
            }
            return false;
        }
        else {
            var arrSplit = val.split('.');
            var isError = false;
            if (arrSplit[0].length != 4) {
                isError = true;
            }
            if (isError) {
                if (showMsg) {
                    $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "NewNumberFormatError", "新增的科目代码格式请使用4+2+2...格式！"), function () {
                        $(sender).focus();
                    });
                }
                return false;
            }
        }
        return true;
    },
    //是否存在重复的编码
    hasDuplicateNumber: function (arrNumber, detectNumber) {
        //唯一的匹配
        var uniqMatch = [];
        var duplicateMatch = [];
        $.each(arrNumber, function (i, val) {
            if ($.inArray(val, uniqMatch) == -1) {
                uniqMatch.push(val);
            }
            else {
                duplicateMatch.push(val);
            }
        });

        return $.inArray(detectNumber, duplicateMatch) != -1;
    },
    //显示或隐藏新增代码输入框
    switchNewNumberVisible: function (sender, matchNumber) {
        var curTr = $(sender).closest("tr");
        if (matchNumber == "-1") {
            curTr.find(".new-number-wrapper").show();
            if (!AccountMatch.changeByInitNewLog) {
                curTr.find(AccountMatch.txtNewNumberClass).focus();
            }
            //curTr.find(AccountMatch.cbSysAcctClass).combo("showPanel");
        }
        else {
            curTr.find(".new-number-wrapper").hide();
            //curTr.find(AccountMatch.cbSysAcctClass).combobox("setValue", "");
        }
    },
    autoFillChildNewUserNumber: function (curUserNumber, userNumber, newUserNumber) {
        var childList = $(AccountMatch.txtNewNumberClass + "[pnumber='" + curUserNumber + "']");

        if (childList.length > 0) {
            $.each(childList, function (i, txt) {
                var tmpUserNumber = $(this).attr("number");
                AccountMatch.setNewNumberVal(this, tmpUserNumber.replace(userNumber, newUserNumber));
                AccountMatch.autoFillChildNewUserNumber(tmpUserNumber, userNumber, newUserNumber);
            });
        }
    },
    getMatchVal: function (cb) {
        var val = '';
        if (cb && cb.length > 0) {
            if (cb.attr("loaded") == undefined) {
                val = cb.val();
            }
            else {
                val = cb.combobox("getValue");
            }
            if (val == AccountMatch.newAcctOptText || val == -1) {
                val = '';
            }
        }
        return val;
    },
    getMatchVal: function (cb) {
        var val = '';
        if (cb && cb.length > 0) {
            if (cb.attr("loaded") == undefined) {
                val = cb.val();
            }
            else {
                val = cb.combobox("getValue");
            }
            if (val == AccountMatch.newAcctOptText || val == -1) {
                val = '';
            }
        }
        return val;
    },
    getMatchOrNewVal: function (cb) {
        var val = AccountMatch.getMatchVal(cb);
        if (!val) {
            var txtNew = $(AccountMatch.txtNewNumberClass + "[number='" + cb.attr("number") + "']");
            if (txtNew.is(":visible")) {
                val = txtNew.val();
            }
        }
        return val;
    },
    //填充新增科目
    fillNewNumberList: function (userNumber) {
        var txtNumber = $(AccountMatch.txtNewNumberClass + "[number='" + userNumber + "']");
        //新增科目有值则不填充
        if (txtNumber.val()) {
            return;
        }
        var parentUserNumber = AccountMatch.getParentNo(userNumber);
        var cbMatch = $(AccountMatch.cbUnMatchSysAcctClass + "[number='" + parentUserNumber + "']");
        var parentVal = AccountMatch.getMatchVal(cbMatch);
        if (!parentVal) {
            var txtPNewNumber = $(AccountMatch.txtNewNumberClass + "[number='" + parentUserNumber + "']");
            if (txtPNewNumber.length > 0) {
                parentVal = txtPNewNumber.val();
            }
        }
        //父级有匹配或新增，则自动填充
        if (parentVal) {
            var nextSubNo = AccountMatch.getNextSubNoByParent(parentVal, true);
            var newNumber = AccountMatch.getAutoNewNumber(parentVal, nextSubNo);
            txtNumber.val(newNumber);
            AccountMatch.checkBrokenNumber(true, false, parentVal);
        }
        else {
            //导入的科目没有上级科目，但系统存在上级科目，则自动填充
            var cbSysAcct = $(AccountMatch.cbSysAcctClass + "[number='" + userNumber + "']")[0];
            AccountMatch.loadSysAcctList(cbSysAcct, false, function () {
                //新科目父级必须是系统科目
                var pSysAcct = AccountMatch.systemAccountList.filter(function (acct) {
                    return acct.MNumber == parentUserNumber;
                });
                if (pSysAcct.length > 0) {
                    if ($(cbSysAcct).combobox("getValue") == parentUserNumber) {
                        $(cbSysAcct).combobox("setValue", "");
                    }
                    AccountMatch.onSelectNewAccount = true;
                    $(cbSysAcct).combobox("setValue", parentUserNumber);
                    AccountMatch.checkBrokenNumber(true, false, parentUserNumber);
                }
            });
        }
    },
    //已预设匹配的用户科目
    getInitedMatchUserNumberList: function () {
        var numList = [];
        var initMatchList = $(AccountMatch.cbUnMatchSysAcctClass + "[initval!='null']");
        $.each(initMatchList, function () {
            var userNumber = $(this).attr("number");
            if (AccountMatch.getMatchVal($(this))) {
                numList.push(userNumber);
            }
        });
        return numList;
    },
    //手动匹配改变时
    onManualMatchSelect: function (sender, oldVal) {
        var arrMatchNumber = [];
        var userNumber = $(sender).attr("number");
        var matchNumber = $(sender).combobox("getValue");
        if (!matchNumber) {
            return;
        }
        var txtNewNumber = $(AccountMatch.txtNewNumberClass + "[number='" + userNumber + "']");
        var isHidden = txtNewNumber.is(":hidden");
        AccountMatch.switchNewNumberVisible(sender, matchNumber);
        //选择新增科目
        if (matchNumber == "-1") {
            AccountMatch.updateMatchResult(userNumber, AccountMatch.matchNumberKey, "");
            if (oldVal && isHidden) {
                txtNewNumber.val('');
            }
            if (userNumber.length > 4) {
                AccountMatch.fillNewNumberList(userNumber);
            }
            if (txtNewNumber.val()) {
                txtNewNumber.removeClass("validatebox-invalid");
            }
            return;
        }
        else {
            //校验匹配是否重复
            if (AccountMatch.isInited) {
                $(AccountMatch.cbUnMatchSysAcctClass).each(function () {
                    if ($(this).attr("loaded") == undefined) {
                        var val = $.trim($(this).val());
                        if (val && val != AccountMatch.newAcctOptText) {
                            arrMatchNumber.push($.trim($(this).val()).split(' ')[0]);
                        }
                    }
                    else {
                        var selNumber = $(this).combobox("getValue");
                        if (selNumber && selNumber != "-1") {
                            arrMatchNumber.push(selNumber);
                        }
                    }
                });
                if (AccountMatch.hasDuplicateNumber(arrMatchNumber, matchNumber)) {
                    $(sender).combobox("setValue", "");
                    txtNewNumber.val('');
                    var sysAcct = $(AccountMatch.cbSysAcctClass + "[number='" + userNumber + "']");
                    if (sysAcct.attr("loaded") != undefined) {
                        sysAcct.combobox("setValue", "");
                    }
                    $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "MatchNumberDuplicate", "匹配不能重复！"), function () {
                        $(sender).combo("showPanel");
                    });
                    return;
                }
            }

            var sysMaxNo = AccountMatch.getSysChildMaxNo(matchNumber);
            //var matchedChildren = AccountMatch.getExistSysNumberList(matchNumber);
            var initNumList = AccountMatch.getInitedMatchUserNumberList();
            var childrenList = $(AccountMatch.txtNewNumberClass + "[pnumber='" + userNumber + "']").filter(function () {
                return $.inArray($(this).attr("number"), initNumList) == -1;
            });
            //当前匹配的系统科目不包含子级，就将用户科目下所有子级自动新增
            if (childrenList.length > 0) {//matchedChildren.length == 0 && 
                //替换掉之前的填充
                if (oldVal && oldVal != matchNumber) {
                    var txtNumbers = $(AccountMatch.txtNewNumberClass + ":visible").filter(function () {
                        return $(this).val() && $(this).val().indexOf(oldVal) == 0 && $(this).attr("number").indexOf(userNumber) == 0;
                    });
                    var nextSubNo = AccountMatch.getNextSubNoByParent(matchNumber, false);
                    var deduce = 0;
                    $.each(txtNumbers, function (i, txt) {
                        var val = $(this).val();
                        var subNo = AccountMatch.getSubNo(val, false, oldVal.split('.').length);
                        if (i == 0) {
                            deduce = nextSubNo - subNo;
                        }
                        var newVal = val.replace(oldVal, matchNumber);
                        $(this).val(newVal);
                        AccountMatch.setSubNo(this, subNo + deduce, matchNumber.split('.').length);
                    });

                    AccountMatch.checkBrokenNumber(true, false, matchNumber);
                }

                AccountMatch.autoNewChildrenNumber(childrenList, sysMaxNo, userNumber, matchNumber);
            }
        }

        AccountMatch.updateMatchResult(userNumber, AccountMatch.matchNumberKey, matchNumber);
    },
    getSysChildMaxNo: function (matchNumber) {
        var sysChildrenList = AccountMatch.systemAccountList.filter(function (el) {
            return AccountMatch.getParentNo(el.MNumber) == matchNumber;
        });
        var maxNo = 0;
        $.each(sysChildrenList, function (i, sysAcct) {
            var numberSpit = sysAcct.MNumber.split('.');
            maxNo = Math.max(maxNo, +numberSpit[numberSpit.length - 1]);
        });
        return maxNo;
    },
    //更新新增科目的子级
    //updateNewNumberSubNo: function () {

    //},
    //获取系统科目的下一级的最大编号
    generateChildAcctSubNo: function (parentNo) {
        var result = AccountMatch.systemAccountList.filter(function (el) {
            return AccountMatch.getParentNo(el.MNumber) == parentNo;
        });

        var maxNo = 0;
        $.each(result, function (i, item) {
            var arrNumber = item.MNumber.split('.');
            maxNo = Math.max(maxNo, arrNumber[arrNumber.length - 1]);
        });
        return maxNo;
    },
    //获取指定科目代码的父级代码
    getParentNo: function (str) {
        var arrSplit = str.split('.');
        arrSplit.splice(arrSplit.length - 1, 1);
        return arrSplit.join('.');
    },
    //获取标准的科目代码
    getStandardNo: function (str) {
        result = str;
        if (str.indexOf('.') == -1 && str.length > 4) {
            var splitChar = $.trim(str.replace(/\d/g, ""))[0];
            if (splitChar) {
                return str.replace(/\D/g, '.');
            }

            result = str.replace(/[^\w]/g, '');
            var arrTmp = [];
            arrTmp.push(result.substr(0, 4));

            for (var i = 4; i < result.length; i += 2) {
                arrTmp.push(result.substr(i, 2));
            }
            result = arrTmp.join('.');
        }
        return result;
    },
    //校验新增的匹配
    validateAppendMatch: function (txtSender, cbSender) {
        var userNumber = $(txtSender).attr("number");
        var newNumber = $(txtSender).val();
        var parentNumber = AccountMatch.getParentNo(newNumber);
        var arrMatchNumber = AccountMatch.getExistChildrenNumberList(parentNumber);
        var isValid = true;
        var duplicateNo;
        if (AccountMatch.hasDuplicateNumber(arrMatchNumber, newNumber)) {
            isValid = false;
            if (cbSender) {
                $(txtSender).val("");
            }
            else {
                duplicateNo = "（" + $(txtSender).val() + "）";
            }
            $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "MatchNumberDuplicate", "匹配不能重复！") + duplicateNo, function () {
                if (cbSender) {
                    $(cbSender).combo("showPanel");
                }
            });
            return;
        }
        if (!cbSender) {
            return isValid;
        }
        AccountMatch.updateMatchResult(userNumber, AccountMatch.newNumberKey, newNumber);
    },
    //已经存在的系统科目
    getExistSysNumberList: function (pnumberFilter) {
        var arrExistNumber = [];
        $(AccountMatch.systemAccountList).each(function (i, item) {
            if (AccountMatch.getParentNo(item.MNumber) == pnumberFilter) {
                arrExistNumber.push(item.MNumber);
            }
        });
        return arrExistNumber;
    },
    //获取指定科目的下一级自动新增科目代码
    getAutoNewNumberList: function (pnumberFilter) {
        var arrAutoNewNumber = [];
        $(AccountMatch.matchResultList).each(function (i, item) {
            if (item.MatchResult == MatchResultEnum.AutoAdd && AccountMatch.getParentNo(item.MNumber) == pnumberFilter) {
                arrAutoNewNumber.push(item.MNewNumber || item.MNumber);
            }
        });
        return arrAutoNewNumber;
    },
    getNewNumberList: function (pnumberFilter, excludeNewNumber) {
        var arrNewNumber = [];
        $(AccountMatch.txtNewNumberClass + ":visible").each(function (i, txt) {
            if (this.value && (!pnumberFilter || AccountMatch.getParentNo(this.value) == pnumberFilter) && (excludeNewNumber == undefined || !excludeNewNumber || excludeNewNumber != this.value)) {
                arrNewNumber.push(this.value);
            }
        });
        return arrNewNumber;
    },
    //从新增科目里，获取某科目下的子科目的最大子编码
    getChildNewNumberMaxSubNo: function (parentNumber, childBeginNo, excludeNumber) {
        if (childBeginNo == undefined) {
            childBeginNo = 0;
        }
        var childNewNumberList = AccountMatch.getNewNumberList(parentNumber, excludeNumber);
        $.each(childNewNumberList, function (i, number) {
            var numberSpit = number.split('.');
            childBeginNo = Math.max(childBeginNo, +numberSpit[numberSpit.length - 1]);
        });
        return childBeginNo;
    },
    //获取指定科目的下一级自动新增科目代码的最大子编码
    getAutoAddNumberMaxSubNo: function (parentNumber, childBeginNo) {
        if (childBeginNo == undefined) {
            childBeginNo = 0;
        }

        var autoNumberList = AccountMatch.getAutoNewNumberList(parentNumber);
        $.each(autoNumberList, function (i, number) {
            var numberSpit = number.split('.');
            childBeginNo = Math.max(childBeginNo, +numberSpit[numberSpit.length - 1]);
        });
        return childBeginNo;
    },
    isNumberExistInSys: function (number) {
        var result = AccountMatch.systemAccountList.filter(function (el) {
            return el.MNumber == number;
        });
        return result.length == 1;
    },
    getNextSubNo: function (numberList, parentNumber, excludeNumberList) {
        var nextSubNo = 0;
        if (!numberList) {
            numberList = AccountMatch.getExistChildrenNumberList(parentNumber);
        }
        var newNumberList = [];
        if (excludeNumberList && excludeNumberList.length > 0) {
            $.each(numberList, function (i, num) {
                if ($.inArray(num, excludeNumberList) == -1) {
                    newNumberList.push(num);
                }
            });
        }
        else {
            newNumberList = numberList;
        }
        var arrSubNoList = [];
        $.each(newNumberList, function (i, number) {
            var arrNumber = number.split('.');
            arrSubNoList.push(+arrNumber[arrNumber.length - 1]);
        });
        if (arrSubNoList.length > 0) {
            nextSubNo = Math.max.apply(null, arrSubNoList);
        }
        return nextSubNo + 1;
    },
    //新增匹配选择时，找不到的一级科目，点下拉箭头，选择一项时，自动填充新增的科目编码
    onAppendMatchSelect: function (sender) {
        var txtNewNumber = $(sender).closest("td").find(AccountMatch.txtNewNumberClass);
        //if (!$(sender).combobox("getValue") || txtNewNumber.val()) {
        //    return;
        //}
        if (!AccountMatch.clearRelation) {
            return;
        }
        var userNumber = txtNewNumber.attr("number");
        var appendNumber = $(sender).combobox("getValue");

        var existList = AccountMatch.getExistChildrenNumberList(appendNumber, txtNewNumber.val());
        var txtNewNumbers = $(AccountMatch.txtNewNumberClass + ":visible");
        var childList = [];
        $.each(txtNewNumbers, function (i, txt) {
            if ($(this).val() && $(this).attr("number").indexOf(userNumber) == 0) {
                childList.push($(this).val());
            }
        });
        var childBeginNo = AccountMatch.getNextSubNo(existList, undefined, childList);
        var newNumber = appendNumber ? appendNumber + '.' + (childBeginNo < 10 ? '0' + childBeginNo : childBeginNo) : '';

        //点新增科目时，如果用户科目没有使用过，则从新科目和用户科目中取小的那个
        if (AccountMatch.onSelectNewAccount) {
            var arrSplit = userNumber.split('.');
            //子科目小于测算的新科目
            if (arrSplit.length > 1 && +arrSplit[arrSplit.length - 1] < childBeginNo) {
                if (AccountMatch.isNumberExistInSys(userNumber) && $.inArray(userNumber, existList) == -1) {
                    newNumber = AccountMatch.getStandardNo(Math.min(+arrSplit.join(''), +newNumber.split('.').join('')).toString());
                }
            }
        }

        //没值才处理
        AccountMatch.setNewNumberVal(txtNewNumber[0], newNumber);
        //设置文本框状态
        AccountMatch.switchTxtValidateStatus(txtNewNumber[0]);
        //校验重复
        //AccountMatch.validateAppendMatch(txtNewNumber[0], sender);

        var curChildren = $(AccountMatch.txtNewNumberClass + "[pnumber='" + userNumber + "']");
        if (curChildren.length > 0) {
            //自动填充所有子科目
            AccountMatch.updateAppendSelectChildren(curChildren, undefined, newNumber);
        }
        AccountMatch.onSelectNewAccount = false;
        AccountMatch.checkBrokenNumber(true, false, appendNumber);
    },
    //更新匹配结果
    updateMatchResult: function (userNumber, key, number, name) {
        //var curLocaleId = $("#hidCurrentLangID", top.document).val();
        $.each(AccountMatch.matchResultList, function (i, item) {
            if (item.MNumber == userNumber) {
                item[key] = number;
                if (number) {
                    if (key == AccountMatch.matchNumberKey) {
                        item.MNewNumber = '';
                    }
                    else {
                        item.MMatchNumber = '';
                    }
                }
                if (!AccountMatch.changeByInitMatchLog && !AccountMatch.changeByInitNewLog) {
                    AccountMatch.isMatchChanged = true;
                }

                return false;
            }
        });
    },
    //更新当前新增匹配下面的所有子级科目
    updateAppendSelectChildren: function (curChildrenList, childrenList, parentNumber) {
        var list = childrenList || curChildrenList;
        var idx = 0;
        $.each(list, function (i, obj) {
            var userNumber = $(this).attr("number");
            var cbMatch = $(AccountMatch.cbUnMatchSysAcctClass + "[number='" + userNumber + "']");
            if (AccountMatch.getMatchVal(cbMatch)) {
                return;//continue
            }
            idx++;
            var newNumber = parentNumber ? parentNumber + '.' + (idx < 10 ? '0' + idx : idx) : '';
            AccountMatch.setNewNumberVal(this, newNumber);
            AccountMatch.switchTxtValidateStatus(this);
            //var cbAppendMatch = $(AccountMatch.cbSysAcctClass + "[number='" + userNumber + "']")[0];
            //AccountMatch.validateAppendMatch(this, cbAppendMatch);
            var subChildrenList = $(AccountMatch.txtNewNumberClass + "[pnumber='" + userNumber + "']");

            if (subChildrenList.length > 0) {
                AccountMatch.updateAppendSelectChildren(curChildrenList, subChildrenList, newNumber);
            }
        });
    },
    getNewNumber: function (number, subNo) {
        var arrNum = number.split('.');
        arrNum[arrNum.length - 1] = subNo < 10 ? '0' + subNo : subNo;
        return arrNum.join('.');
    },
    getInitedUserNumList: function (pNumber) {
        var result = [];
        var cbMatchs = $(AccountMatch.cbUnMatchSysAcctClass + "[pnumber='" + pNumber + "'][initval!='null']");
        $.each(cbMatchs, function () {
            result.push($(this).attr("number"));
        });
        return result;
    },
    getNewNumber: function (number, subNo) {
        var arrNum = number.split('.');
        arrNum[arrNum.length - 1] = subNo < 10 ? '0' + subNo : subNo;
        return arrNum.join('.');
    },
    //上一级手动匹配，匹配的系统科目没有子级，则将用户科目下的所有子级自动新增
    autoNewChildrenNumber: function (curChildrenList, maxNo, parentUserNumber, parentMatchNumber, childrenList) {
        var list = childrenList || curChildrenList;
        $.each(list, function (i, obj) {
            var userNumber = $(this).attr("number");

            var cbMatch = $(AccountMatch.cbUnMatchSysAcctClass + "[number='" + userNumber + "']");
            var txtNewNumber = $(AccountMatch.txtNewNumberClass + "[number='" + userNumber + "']");
            //如果匹配有值，continue
            var matchVal = AccountMatch.getMatchVal(cbMatch);
            var newVal = txtNewNumber.val();
            var initedChildUserNumList = AccountMatch.getInitedUserNumList(parentUserNumber);
            //有匹配，或有新增，或用户科目有预设匹配（防止断号）
            if (matchVal || (newVal && newVal.indexOf(parentMatchNumber) == 0) || (!AccountMatch.isInited && $.inArray(userNumber, initedChildUserNumList) != -1)) {
                return;
            }

            var newVal = userNumber.replace(parentUserNumber, parentMatchNumber);
            var pNumber = AccountMatch.getParentNo(newVal);

            var nextSubNo = AccountMatch.getNextSubNo(undefined, pNumber);
            newVal = AccountMatch.getNewNumber(newVal, nextSubNo);

            AccountMatch.setNewNumberVal(this, newVal);
            AccountMatch.switchTxtValidateStatus(txtNewNumber[0]);

            //下级子节点
            var subChildrenList = $(AccountMatch.txtNewNumberClass + "[pnumber='" + userNumber + "']");
            if (subChildrenList.length > 0) {
                AccountMatch.autoNewChildrenNumber(curChildrenList, 0, userNumber, newVal, subChildrenList);
            }
        });
    },
    initUI: function (selector) {
        try {
            selector = selector || AccountMatch.selector;
            $(selector).treegrid('resize', {
                width: $(".m-imain-content").width(),
                height: (AccountMatch.currentMode == MatchMode.Match ? $(".m-imain").height() - 66 : $(".m-main", top.document).height() - 150)
            });
        } catch (exc) { }
    },
    showConfirmInfo: function (isShow) {
        if (isShow) {
            $("#divConfirm").show();
            $("#divMatch").hide();
        }
        else {
            AccountMatch.initUI();
            $("#divMatch").show();
            $("#divConfirm").hide();
        }
    },
    bindCombobox: function (sender, options, isInited) {
        var opt = {
            valueField: 'MNumber',
            textField: 'MFullName',
            height: 30
        };
        if (isInited === false) {
            opt.onShowPanel = function () {
                AccountMatch.loadUnMatchAcctList(sender, true);
                AccountMatch.curCombo = $(sender);
                AccountMatch.setComboPanelShadow($(this));
                AccountMatch.setInitedFlag();
            };
        }
        $.extend(opt, options);
        $(sender).combobox(opt);
    }
};

$(document).ready(function () {
    AccountMatch.init();
});