(function (window) {
    var BDBankReconcile = (function () {
        //常量定义
        var pageIndex = 1;
        //单页显示的条数
        var pageSize = 20;
        //总记录数
        var totalCount = 0;
        //是否对账
        var _matchCondition = ".match-condition";
        //有对上的那把锁
        var _reconcileMatchLock = ".reconcile-match-lock";
        //交易分布视图
        var _transactionPartial = ".transaction-partial";
        //勾兑交易视图
        var _reconcileMain = ".reconcile-main";
        //对账单表格
        var _matchRecord = ".match-record";
        //省略字符 类
        var _hidetext = "hidetext";
        //对上的样式
        var _matchClass = "match-record";
        //未对上样式
        var _unmatchClass = "unmatch-record";
        //银行账单
        var _bankRecord = ".bank-record";
        //美记账单
        var _megiRecord = ".megi-record";
        //
        var _record = ".record";
        //交易日期
        var _recordTableDate = ".record-table-date";
        //交易号
        var _recordTableTransactionNo = ".record-table-transaction-no";
        //借款额
        var _recordTableSpent = ".record-table-spent";
        //收款额
        var _recordTableReceived = ".record-table-received";
        //功能层 查找/对账
        var _reconcileFunctionLayer = ".reconcile-function-layer";
        //匹配
        var _reconcileMatch = ".reconcile-match";
        //展开
        var _reconcileExpand = ".reconcile-expand";
        //查找
        var _reconcileSearch = ".reconcile-search";
        //ok
        var _reconcileApply = ".reconcile-apply";
        //翻页控件
        var _pager = ".bank-reconcile-pager";
        //
        var _reconcileBottom = ".bank-reconcile-bottom";
        //账单模版
        var _reconcileModel = "#reconcileModel";
        //获取对账单信息
        var _getBankReconcileUrl = "/BD/BDBank/GetBDBankReconcileList";
        //查找url
        var _searchUrl = "/BD/BDBank/BDBankReconcileEdit";
        //确认对账的url
        var _confirmUrl = "/BD/BDBank/UpdateBDBankBillReconcile";
        //匹配url
        var _matchUrl = "/BD/BDBank/BDBankReconcileMatch";
        
        var _reconcile = ".reconcile";

        var functoolbar = ".reconcile-toolbar";
        //查找相关
        var btnSearchdiv = ".btnSearchDiv";
        var btnreset = '.search-reset';
        var btnSearchclose = ".m-adv-search-close";
        var divSearch = ".m-adv-search-reconclie";
        var txtAmountFrom = ".search-amount-from";
        var txtAmountTo = ".search-amount-to";
        var txtPayer = ".search-payer";
        var txtDate = ".search-date";
        var txtRef = ".search-ref";
        var txtFrom = ".search-from";
        var toFlag = ".search-toflag";
        var exactAmount = ".search-exact";
        var btnSearch = ".search-do";

        //显示系统能匹配的
        var _chkShowSysMatch = "#showSysMatch";

        //全部收起 展开
        var _btnExpand = "#btnExpand";
        var _btnCollspan = "#btnCollspan";
        var _btnReconclie = "#btnReconclie"
        var _chkAll = "#chkAll";

        //排序
        var _headerDate = ".header-date";
        var _headerSpent = ".header-spent";
        var _headerRecevied = ".header-recevied";
        var _sort = "MDate";
        var _order = "desc";
        var _bankID = "";

        //当前的账单ID
        var _currentEntryID = "";
        //当前匹配的BankBillID
        var _matchBillID = "";
        //设置匹配用的临时变量，每次刷新数据后设置完成匹配，需要清空它
        var _tmpmatchUsed = [];
        //是否初始化tab的属性
        var inited = "inited";
        //class constructor
        var BDBankReconcile = function (r) {
            //
            var that = this;
            //显示功能层
            this.showFunctionLayer = function (selector, matchNum) {
                //首先根据selector调整位置
                var top = (selector.offset().top + $(_matchCondition, selector).height());
                //
                $(_reconcileFunctionLayer).stop();
                //如果top小于ScrollTop，则不予以显示
                if (($(_reconcileMain).offset().top > top) ||
                    (top + $(_reconcileFunctionLayer).height() + $(_pager).height() > $("body").height())) {
                    return $(_reconcileFunctionLayer).hide();
                }
                //调整位置
                $(_reconcileFunctionLayer).animate({
                    top: top + "px",
                    left: (selector.offset().left + 24) + "px",
                    width: $(selector).width() - 25 + "px",
                    height: $(selector).height() + "px"
                }, 200, "swing").show();
                //展示多个按钮
                if (matchNum >0 ) {
                    //显示ok按钮
                    $(_reconcileApply, _reconcileFunctionLayer).show();
                    //
                    $(_reconcileSearch).css({ "margin-right": "30px" });
                    if (matchNum > 1) {
                        $(".match-count", _reconcileFunctionLayer).html(matchNum);
                        $(_reconcileMatch, _reconcileFunctionLayer).show();

                    }
                    else {
                        $(_reconcileMatch, _reconcileFunctionLayer).hide();
                    }
                }
                else {
                    //隐藏OK按钮
                    $(_reconcileApply, _reconcileFunctionLayer).hide();
                    $(_reconcileMatch, _reconcileFunctionLayer).hide();
                    //
                    $(_reconcileSearch).css({ "margin-right": "0px" });
                }
                //浮层 垂直居中
                var mtop = ($(selector).height() - $('.reconcile-function-button').height()) / 2;
                if (mtop <= 0) {
                    mtop = 0;
                }
                $('.reconcile-function-button', _reconcileFunctionLayer).css({
                    "margin-top": mtop + "px"
                });
                //展开 还原 图片处理
                $(_reconcileExpand, _reconcileFunctionLayer).removeClass("expand-ico");
                $(_reconcileExpand, _reconcileFunctionLayer).removeClass("collspan-ico");
                if ($(selector).find(_recordTableTransactionNo).hasClass(_hidetext)) {
                    $(_reconcileExpand, _reconcileFunctionLayer).addClass("expand-ico");
                    $(_reconcileExpand, _reconcileFunctionLayer).find(".ico-span").html(HtmlLang.Write(LangModule.Common, "CollapseDown", "展开"));
                }
                else {
                    $(_reconcileExpand, _reconcileFunctionLayer).addClass("collspan-ico");
                    $(_reconcileExpand, _reconcileFunctionLayer).find(".ico-span").html(HtmlLang.Write(LangModule.Common, "CollapseUp", "收起"));
                }
                //当前的账单id
                _currentEntryID = selector.attr("id");
                _matchBillID = selector.attr("mathbillid");
            };
            //鼠标移上去展示功能div
            this.initRecordMouseEvent = function (selector, matchNum) {
                selector.off("mouseover").on("mouseover",function () {
                    return that.showFunctionLayer($(this), matchNum);
                });
            };
            //显示&隐藏 金额过滤项
            this.exactAmountShow = function () {
                if ($(exactAmount, divSearch).is(":checked")) {
                    $(txtAmountTo, divSearch).hide();
                    $(toFlag, divSearch).hide();
                    $(txtAmountFrom, divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Common, "Amount", "Amount")).removeClass("has-hint").initHint();
                }
                else {
                    $(txtAmountTo, divSearch).show();
                    $(toFlag, divSearch).show();
                    $(txtAmountFrom, divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Common, "MinAmount", "最小金额")).removeClass("has-hint").initHint();
                }
            };
            //点击匹配
            this.matchBusiness = function () {
                $(_reconcileFunctionLayer).hide();
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "QuickChooseItem", "快捷选择系统匹配项");
                //弹窗
                $.mDialog.show({
                    mTitle: title,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mWidth: 1000,
                    mHeight: 420,
                    mContent: "iframe:" + _matchUrl + "/" + _currentEntryID + "?bankid=" + _bankID + "&mathid=" + _matchBillID,
                    mCloseCallback: [function (needRefresh) {
                        if (needRefresh) {
                            //重新初始化勾对列表
                            that.InitReconcileList();
                            //将交易列表置位未初始化
                            $(_transactionPartial).attr(inited, "0");
                        }
                    }]
                });
            };
            //全部展开
            this.expandAllBusiness = function () {
                $(_record, _reconcileMain).not(":hidden").each(function (index, selector) {
                    if ($(selector).find(_recordTableTransactionNo).hasClass(_hidetext)) {
                        that.doExpand(selector, true);
                    }
                });
                //计算锁的位置
                that.initLockPostion();
                $(_btnCollspan).show();
                $(_btnExpand).hide();
            };
            //全部收起
            this.collspanAllBusiness = function () {
                $(_record, _reconcileMain).not(":hidden").each(function (index, selector) {
                    if (!$(selector).find(_recordTableTransactionNo).hasClass(_hidetext)) {
                        that.doExpand(selector, false);
                    }
                })
                //计算锁的位置
                that.initLockPostion();
                $(_btnExpand).show();
                $(_btnCollspan).hide();
            };
            //点击展开事件
            this.expandBusiness = function () {
                $(_reconcileFunctionLayer).hide();
                var selector = "#" + _currentEntryID;

                if ($(selector).find(_recordTableTransactionNo).hasClass(_hidetext)) {
                    that.doExpand(selector, true);
                }
                else {
                    that.doExpand(selector, false);
                }
                //计算锁的位置
                that.initLockPostion();
            };
            //执行展开 或者收起
            this.doExpand = function (selector, isExpand) {
                var data = $(selector).data("data");
                var matchedRows = data.MMatchList || [];
                if (isExpand) {
                    $(_bankRecord + " " + _recordTableTransactionNo, selector).html(that.assembleTransAccount(data)
                        + "<p>" + that.assembleBankRef(data) + "</p>");
                    if (matchedRows.length > 0) {
                        var matchrow = that.getMatchRow(data.MMatchBillID, matchedRows);
                        $(_megiRecord + " " + _recordTableTransactionNo, selector).html(that.assembleMegiAccount(matchrow)
                            + "<p>" + that.assembleMegiRef(matchrow) + "</p>");
                    }

                    $(selector).find(_recordTableTransactionNo).removeClass(_hidetext);
                    var bankRecordHeight = $(_bankRecord, selector).outerHeight();
                    var megiRecordHeight = $(_megiRecord, selector).outerHeight();
                    $(selector).height(bankRecordHeight >= megiRecordHeight ? bankRecordHeight : megiRecordHeight);
                }
                else {
                    $(_bankRecord + " " + _recordTableTransactionNo, selector).html(that.assembleTransAccount(data));
                    if (matchedRows.length > 0) {
                        var matchrow = that.getMatchRow(data.MMatchBillID, matchedRows);
                        $(_megiRecord + " " + _recordTableTransactionNo, selector).html(that.assembleMegiAccount(matchrow));
                    }
                    $(selector).find(_recordTableTransactionNo).addClass(_hidetext)
                    $(selector).height(35);
                }
            };
            //如果有选择过的匹配行，挑出来，否则第一条
            this.getMatchRow = function (matchid, matchRows) {
                if (matchRows.length < 1) {
                    return {};
                }
                else {
                    for (var i = 0; i < matchRows.length; i++) {
                        if (matchid == matchRows[i].MBillID) {
                            return matchRows[i]
                        }
                    }
                }
                return matchRows[0];
            };
            //点击查找事件
            this.searchBusiness = function () {
                $(_reconcileFunctionLayer).hide();
                //标题
                var title = HtmlLang.Write(LangModule.Bank, "ReconcileEdit", "Reconcile Edit");
                //弹窗
                $.mDialog.show({
                    mTitle: title,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + _searchUrl + "?acctid=" + _currentEntryID + "&bankId=" + _bankID + "&mathList=" + _matchBillID,
                    mCloseCallback: [function (needRefresh) {
                        if (needRefresh) {
                            //重新初始化勾对列表
                            that.InitReconcileList();
                            //将交易列表置位未初始化
                            $(_transactionPartial).attr(inited, "0");
                        }
                    }]
                });
            };
            //点击确认勾兑事件
            this.applyReconcileBusiness = function () {
                $(_reconcileFunctionLayer).hide();

                //获取数据
                var data = $("#" + _currentEntryID).data("data");
                var obj = that.buildParaObj(data);

                mAjax.submit(_confirmUrl, { model: [obj] }, function (msg) {
                    if (msg.Success) {
                        mMsg(HtmlLang.Write(LangModule.Bank, "Reconciled", "Reconciled"));
                        that.InitReconcileList();
                    } else {
                        $.mDialog.alert(msg.Message, function () {
                            that.InitReconcileList();
                        });
                    }
                });
            };
            this.buildParaObj = function (data) {
                //转化格式
                var mathItem = that.getMatchRow(data.MMatchBillID, data.MMatchList);
                //后台传过去的参数
                var obj = {};
                obj.MBankBillEntryID = data.MEntryID;
                obj.MSpentAmtFor = data.MSpentAmt;
                obj.MBankID = _bankID;
                obj.MReceiveAmtFor = data.MReceivedAmt;
                var entry = {};
                entry.MTargetBillID = mathItem.MBillID;
                entry.MSpentAmtFor = mathItem.MSpentAmtFor;
                entry.MReceiveAmtFor = mathItem.MReceiveAmtFor;
                entry.MTargetBillType = mathItem.MTargetBillType;
                entry.MDate = mDate.format(data.MDate);
                var arr = [entry];
                //美记账单
                obj.RecEntryList = arr;
                return obj;
            };
            //批量勾对
            this.reconcileAll = function () {
                var para = [];
                if ($('input:checkbox:checked', _reconcileMain).length < 1) {
                    $.mDialog.warning(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
                    return;
                }

                //判断是否有多对一的 匹配。
                var inUsedList = [];
                _tmpmatchUsed = [];
                $('input:checkbox:checked', _reconcileMain).each(function (index, selector) {
                    //获取数据
                    var data = $(selector).parent().data("data");
                    var obj = that.buildParaObj(data);

                    //是否被占用
                    if ($.inArray(obj.RecEntryList[0].MTargetBillID, _tmpmatchUsed) >= 0) {
                        inUsedList.push(that.getMatchRow(obj.RecEntryList[0].MTargetBillID, data.MMatchList));
                    }
                    _tmpmatchUsed.push(obj.RecEntryList[0].MTargetBillID);
                    para.push(obj);
                });

                if (inUsedList.length > 0) {
                    //组装提示信息
                    var altermsg = HtmlLang.Write(LangModule.Bank, "MatchConflict", "以下业务单据同时匹配了多条银行交易，请确认业务单据对应的银行交易后再完成勾对：")
                    for (var i = 0; i < inUsedList.length; i++) {
                        var data = inUsedList[i];
                        var accountname = data.MBankAccountName ? data.MBankAccountName : "-";
                        var number = data.MNumber ? data.MNumber : "-";
                        var ref = data.MReference ? data.MReference : "-";
                        var amount = data.MSpentAmtFor ? mMath.toMoneyFormat(data.MSpentAmtFor) : mMath.toMoneyFormat(data.MReceiveAmtFor);

                        altermsg += "</br>"
                        altermsg += HtmlLang.Write(LangModule.Common, "Date", "日期") + "[" + mDate.format(data.MBizDate) + "]";
                        altermsg += HtmlLang.Write(LangModule.Bank, "Payee", "收/付款人") + "[" + accountname + "]";
                        altermsg += HtmlLang.Write(LangModule.Common, "BillNumber", "单据号") + "[" + number + "]";
                        altermsg += HtmlLang.Write(LangModule.Common, "Explanation", "备注") + "[" + ref + "]";
                        altermsg += HtmlLang.Write(LangModule.Common, "Amount", "金额") + "[" + amount + "]"
                    }
                    $.mDialog.alert("<div class='popup-list-msg'>" + altermsg + "</div>");
                }
                else {
                    mAjax.submit(_confirmUrl, { model: para }, function (msg) {
                        if (msg.Success) {
                            mMsg(HtmlLang.Write(LangModule.Bank, "Reconciled", "Reconciled"));
                            that.InitReconcileList();
                        } else {
                            $.mDialog.alert(msg.Message, function () {
                                that.InitReconcileList();
                            });
                        }
                    });
                }

                _tmpmatchUsed = [];
            };
            //将一条数据初始化到银行信息上
            this.bindDataToBankTable = function (selector, data) {
                //日期
                $(_recordTableDate, selector).text(mDate.format(data.MDate));
                //消费
                var spent = (data.MSpentAmt) ? mMath.toMoneyFormat(data.MSpentAmt) : "";
                $(_recordTableSpent, selector).text(spent);
                //收入
                var received = (data.MReceivedAmt) ? mMath.toMoneyFormat(data.MReceivedAmt) : "";
                $(_recordTableReceived, selector).text(received);
                //收/付款人 账号
                $(_recordTableTransactionNo, selector).text(that.assembleTransAccount(data));
            };
            //将对方账户的信息组装起来 按照 name（no）-transno
            this.assembleTransAccount = function (data) {
                //
                var name = "";
                //如果有对方账号的名称
                if (data.MTransAcctName) {
                    //
                    name += data.MTransAcctName;
                }
                //如果有对方账号的账号
                if (data.MTransAcctNo) {
                    //
                    name += data.MTransAcctName ? ("(" + data.MTransAcctNo + ")") : data.MTransAcctNo;
                }
                //如果有交易号
                if (data.MTransNo) {
                    name += name ? (" " + data.MTransNo) : data.MTransNo;
                }
                //
                return name;
            };
            //获取银行数据备注
            this.assembleBankRef = function (data) {
                if (data) {
                    return data.MDesc || "-";
                }
                else {
                    return "";
                }
            };
            //获取美记数据 账号
            this.assembleMegiAccount = function (data) {
                if (data) {
                    data.MDescription = data.MDescription == null ? "" : data.MDescription;
                    return data.MBankAccountName == null ? data.MDescription : data.MBankAccountName;
                }
                else {
                    return "";
                }
            };
            //获取美记数据备注
            this.assembleMegiRef = function (data) {
                if (data) {
                    var ref = data.MReference;
                    if (data.MTargetBillType == "Invoice") {
                        ref = "[ " + data.MNumber + " ] " + ref;
                    }
                    return ref;
                }
                else {
                    return "";
                }
            };
            
            //将一条数据初始化到美记信息上
            this.bindDataToMegiTable = function (selector, data) {
                //如果data为空，则清空内部的文本
                if (data) {
                    //匹配id
                    $(".record-math-ids", selector).val(data.MBillID);
                    //日期
                    $(_recordTableDate, selector).text(mDate.format(data.MBizDate));
                    //消费
                    var spent = (data.MSpentAmtFor) ? mMath.toMoneyFormat(data.MSpentAmtFor) : "";
                    $(_recordTableSpent, selector).text(spent);
                    //收入
                    var received = (data.MReceiveAmtFor) ? mMath.toMoneyFormat(data.MReceiveAmtFor) : "";
                    //收入
                    $(_recordTableReceived, selector).text(received);
                    $(_recordTableTransactionNo, selector).text(that.assembleMegiAccount(data));
                }
                else {
                    //主要是span
                    $("span,td", selector).text("");
                }
            };
            //清除所有的对账单信息
            this.clearAllReconcile = function () {
                //
                $(_reconcile).not(":hidden").remove();
            };
            //初始化每个账单表格信息
            this.initReconcileTable = function (data) {
                //导航栏
                var $bottom = $(_reconcileBottom);
                //如果没有数据
                that.clearAllReconcile();
                if (!data || !data.rows) {
                    //清空所有数据
                    return;
                }

                //针对数据库已经存在匹配id的先处理一轮：
                // 1).匹配列表中没有这条数据的，清空匹配id。
                // 2).已经被其他有匹配id的占用了，清空匹配id。
                for (var i = 0; i < data.rows.length; i++) {
                    var row = data.rows[i];
                    var matchedRows = row.MMatchList || [];
                    if (row.MMatchBillID) {
                        //已经被其他有匹配id的占用了
                        if ($.inArray(row.MMatchBillID, _tmpmatchUsed) >= 0) {
                            row.MMatchBillID = "";
                            continue;
                        }

                        //查找是否有这条数据
                        var findflag = false;
                        for (var j = 0; j < matchedRows.length; j++) {
                            if (row.MMatchBillID == matchedRows[j].MBillID) {
                                findflag = true;
                                _tmpmatchUsed.push(row.MMatchBillID);
                                break;
                            }
                        }
                        if (!findflag) {
                            row.MMatchBillID = "";
                        }
                    }
                }

                //初始化表格
                for (var i = 0; i < data.rows.length; i++) {
                    //某一个账单
                    var row = data.rows[i];
                    //获取ID
                    //匹配上的数据
                    var matchedRows = row.MMatchList || [];
                    //新建一个div，从模板中获取
                    var $reconcileDiv = $(_reconcileModel).clone();
                    //加入到父节点
                    $reconcileDiv.insertBefore($bottom).show();
                    //
                    var $record = $(_record, $reconcileDiv);
                    //绑定id
                    $record.attr("id", row.MEntryID);

                    //设置匹配的id，记录到data中。
                    if (matchedRows.length > 0) {
                        if (!row.MMatchBillID) {
                            for (var j = 0; j < matchedRows.length; j++) {
                                if ($.inArray(matchedRows[j].MBillID, _tmpmatchUsed) >= 0) {
                                    //最后一个 直接匹配第一个，为了和匹配更新页面一致
                                    if (j == matchedRows.length - 1) {
                                        row.MMatchBillID = matchedRows[0].MBillID;
                                        break;
                                    }
                                    else {
                                        continue;
                                    }
                                }
                                else {
                                    row.MMatchBillID = matchedRows[j].MBillID;
                                    break;
                                }
                            }
                        }
                        _tmpmatchUsed.push(row.MMatchBillID);
                    }

                    //绑定数据
                    $record.data("data", row);
                    //
                    var bankRecord = $(_bankRecord, $reconcileDiv);
                    //先初始化银行数据
                    that.bindDataToBankTable(bankRecord, row);
                    //匹配的数据
                    var matchedRow = {};
                    //如果匹配数据为0条
                    if (matchedRows.length == 0) {
                        //更换背景样式
                        $(_matchRecord, $reconcileDiv).removeClass(_matchClass).addClass(_unmatchClass);
                        //显示那把锁
                        $(_reconcileMatchLock, $reconcileDiv).remove();

                        $('input[type=checkbox]', $reconcileDiv).attr("disabled", "disabled");
                    }
                    else {
                        //有多条
                        matchedRow = that.getMatchRow(row.MMatchBillID, matchedRows);

                        //绑定银行BankBillID
                        $record.attr("mathBillID", matchedRow.MBillID);
                        //显示那把锁
                        $(_reconcileMatchLock, $reconcileDiv).show();
                    }
                    //
                    var megiRecord = $(_megiRecord, $reconcileDiv);
                    //初始化美记数据
                    that.bindDataToMegiTable(megiRecord, matchedRow);
                    //初始化鼠标移上去的事件
                    r === "True" && that.initRecordMouseEvent($record, matchedRows.length);
                }

                //初始化完匹配数据，清空临时数据
                _tmpmatchUsed = [];
            };
            //计算勾兑上的那个链接的符号的位置
            this.initLockPostion = function () {
                //弹出层隐藏
                $(_reconcileFunctionLayer).hide();
                //
                $(_reconcileMatchLock).each(function () {
                    var reconcileDiv = $(this).parent().parent();
                    //计算top
                    var top = reconcileDiv.offset().top + (reconcileDiv.outerHeight() - $(this).height() - 10) / 2;
                    //如果top不在显示范围之内
                    if (top <= $(_reconcileMain).offset().top
                        || top >= $(_pager).offset().top) {
                        //超过就隐藏
                        $(this).hide();
                    }
                    else {
                        $(this).show();
                        $(this).css({
                            "top": (top + "px")
                        });
                    }
                });
            };
            //获取过滤项输入
            this.getFilter = function () {
                var filter = {
                    page: pageIndex,
                    rows: pageSize,
                    ExactDate: $(txtDate, divSearch).datebox("getValue"),
                    TransAcctName: $(txtPayer, divSearch).val(),
                    MDesc: $(txtRef, divSearch).val(),
                    SrcFrom: $(txtFrom, divSearch).combobox("getValue"),
                    AmountFrom: $(txtAmountFrom, divSearch).val(),
                    AmountTo: $(txtAmountTo, divSearch).val(),
                    IsExactAmount: $(exactAmount, divSearch).is(":checked"),
                    OnlyShowMatched: $(_chkShowSysMatch, functoolbar).is(":checked"),
                    Sort: _sort,
                    Order: _order
                };
                return filter;
            };
            this.doSearch = function () {
                //重新初始化勾对列表
                that.InitReconcileList();
            };
            //获取对账单信息
            this.InitReconcileList = function (acctid) {
                //银行账号
                if (acctid) {
                    _bankID = acctid
                }
                //
                var home = new BDBankReconcileHome();
                //获取选择的日期
                var dates = home.getUserSelectedDate();

                var filter = that.getFilter();
                filter.StartDate = dates[0];
                filter.EndDate = dates[1];
                filter.MBankID = _bankID;
                //异步提交
                mAjax.post(_getBankReconcileUrl, {
                    filter: filter
                }, function (msg) {
                    //初始化数据

                    //这种情况说明页数不够，需要重新请求，减掉一页
                    if (msg && msg.total > 0 && msg.rows.length == 0) {
                        pageIndex = pageIndex - 1;

                        that.InitReconcileList(acctid);

                        return;
                    }

                    that.initReconcileTable(msg);
                    //总条数（已银行为基准)
                    totalCount = msg.total;
                    //初始化导航
                    that.initPagerEvent(acctid);
                    //显示总条数
                    home.ShowReconcileCount(totalCount);
                    that.resizePage();

                    //换页后展开
                    if ($(_btnCollspan, functoolbar).is(":visible")) {
                        that.expandAllBusiness();
                    }

                }, "", true);
            };
            //初始化翻页事件
            this.initPagerEvent = function () {
                //调用easyui组件
                $(_pager).pagination({
                    total: totalCount,
                    pageSize: pageSize,
                    onSelectPage: function (number, size) {
                        pageIndex = number;
                        pageSize = size;
                        that.InitReconcileList();
                    }
                });
            };
            //初始化表头样式
            this.initHeaderPostion = function () {
                $('.reconclie-header').width($(_reconcileMain).width());
                //24 是 中间锁的预留，25是checkbox的预留
                var width = ($('.reconclie-header').width() - 24 - 25) / 2
                $('.bank-header').width(width);
                $('.megi-header').width(width);
            };
            //初始化所有
            this.InitAll = function () {
                that.initDom();
                that.initButtonEvent();
                that.resizePage();
                new BDBankReconcileHome().getForm();
            };
            //初始化界面元素
            this.initDom = function () {
                $(txtPayer, divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Bank, "Payee", "收/付款人")).initHint();
                //支出\收入
                $(txtFrom, divSearch).combobox({
                    width: 110,
                    textField: 'text',
                    valueField: 'value',
                    data: [
                        {
                            text: HtmlLang.Write(LangModule.Common, 'Received', "收入"),
                            value: "1"
                        },
                        {
                            text: HtmlLang.Write(LangModule.Common, 'Spent', "支出"),
                            value: "2"
                        }
                    ]
                });
            }
            //初始化功能事件
            this.initButtonEvent = function () {
                //功能层的失去焦点事件
                $(_reconcileFunctionLayer).off("mouseleave").on("mouseleave", function () {
                    $(this).hide();
                });
                $(_reconcileFunctionLayer).mScroll($(_reconcileMain));
                //
                $(document).on("click", function () {
                    $(_reconcileFunctionLayer).hide();
                });
                //匹配选择的事件
                $(_reconcileMatch).off("click").on("click", that.matchBusiness);
                //展开的事件
                $(_reconcileExpand).off("click").on("click", that.expandBusiness);
                //查找的事件
                $(_reconcileSearch).off("click").on("click", that.searchBusiness);
                //确认的事件
                $(_reconcileApply).off("click").on("click", that.applyReconcileBusiness);
                //全部展开
                $(_btnExpand).off("click").on("click", that.expandAllBusiness);
                //全部收起s
                $(_btnCollspan).off("click").on("click", that.collspanAllBusiness);
                $(_btnReconclie).off("click").on("click", that.reconcileAll);
                //checkbox
                $(exactAmount, divSearch).off("change").on("change", that.exactAmountShow);
                $(_chkShowSysMatch, functoolbar).off("change").on("change", that.doSearch);
                $(btnSearch, divSearch).off("click").on("click", that.doSearch);
                $(btnreset, divSearch).off("click").on("click", function () {
                    $(txtDate, divSearch).datebox('setValue', '');
                    $(txtPayer, divSearch).val('');
                    $(txtRef, divSearch).val('');
                    $(txtFrom, divSearch).combobox("setValue", '');
                    $(txtAmountFrom, divSearch).numberbox("setValue", "");
                    $(txtAmountTo, divSearch).numberbox("setValue", "");
                    $(exactAmount, divSearch).prop("checked", false);
                    $(exactAmount, divSearch).trigger("change");
                    $(_chkShowSysMatch, functoolbar).prop("checked", false);
                    _sort = "MDate";
                    _order = "desc";
                });

                $(_chkAll, '.reconclie-header').off("change").on("change", function () {
                    $('input[type=checkbox]:visible', _reconcileMain).not(':disabled').prop("checked", $(_chkAll, '.reconclie-header').is(":checked"));
                });

                $(_headerDate).off('click').on("click", function () {
                    _sort = "MDate";
                    _order = _order == "desc" ? "asc" : "desc";
                    that.doSearch();
                });

                $(_headerSpent).off('click').on("click", function () {
                    _sort = "Spent";
                    that.doSearch();
                });

                $(_headerRecevied).off('click').on("click", function () {
                    _sort = "Recevied";
                    that.doSearch();
                });

                //显示搜索div
                $(btnSearchdiv, functoolbar).off("click").on("click", function () {
                    $(btnSearchdiv, functoolbar).hide();
                    $(divSearch).show();
                    that.resizePage();
                });

                //隐藏搜索div
                $(btnSearchclose, divSearch).off("click").on("click", function () {
                    $(btnSearchdiv, functoolbar).show();
                    $(divSearch).hide();
                    that.resizePage();
                });

                //js 控制 reconcile-match hover
                $('.reconcile-match', _reconcileMain).hover(function () {
                    $(this).find('.match-text').css({ "opacity": 1, "width": "inherit" });
                    $(this).find('.match-count').css({ "display": "none" });
                    $(this).css({ "background": "none", "background-color": "#2981C6", "border-radius": "31px" });
                },function () {
                    $(this).find('.match-text').css({ "opacity": 0, "width": "10px" });
                    $(this).find('.match-count').css({ "display": "" });
                    $(this).css({ "background": "", "background-color": "", "border-radius": "" });
                });
            };
            //初始化表格高度 位置
            this.resizePage = function () {
                var _mainHeight = $(".main-buttom").height() - $(_pager).outerHeight() - $('.bank-header').outerHeight() - $(functoolbar).outerHeight() - 20;
                if ($(divSearch).is(":visible")) {
                    _mainHeight = _mainHeight - $(divSearch).outerHeight() - 10;
                }
                $(_reconcileMain).height(_mainHeight);
                that.initHeaderPostion();
                //页面滚动的时候
                $(_reconcileMain).off("scroll.bankrec").on("scroll.bankrec", that.initLockPostion);
                //计算宽度
                $(_record, _reconcileMain).not(":hidden").each(function () {
                    //统一宽带，对齐
                    $(_bankRecord, $(this)).width($('.bank-header').width());
                    $(_megiRecord, $(this)).width($('.megi-header').width());
                });

                //计算锁的宽度
                $(_record, _reconcileMain).not(":hidden").each(function () {
                    var bankRight = $(_bankRecord, $(this)).offset().left + $(_bankRecord, $(this)).width();
                    var mid = $(_megiRecord, $(this)).offset().left - bankRight;
                    //20 为最外层的 pading 20
                    $(_reconcileMatchLock, $(this)).css({ "margin-left": bankRight - 20 - ($(_reconcileMatchLock, $(this)).width() - mid) / 2 + "px" });
                });

                //计算锁的位置
                that.initLockPostion();

                //出现滚动条后需要重新布局下 megi-header
                var recordwidth = $(_record, _reconcileMain).not(":hidden").width();
                var addcol = $('.reconclie-header').width() - recordwidth;
                if (addcol > 0 && recordwidth > 0) {
                    $('.megi-header').css({ "margin-right": addcol + "px" });
                }
                else {
                    $('.megi-header').css({ "margin-right": "0px" });
                }
            }
        };
        //返回
        return BDBankReconcile;
    })();
    //
    window.BDBankReconcile = BDBankReconcile;
})(window);
