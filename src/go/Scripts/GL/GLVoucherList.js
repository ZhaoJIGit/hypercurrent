
(function (w) {
    //
    var GLVoucherList = (function () {

        //常量定义
        var pageIndex = 1;
        //单页显示的条数
        var pageSize = GLVoucherHome.pageSize;
        //总记录数
        var totalCount = 0;

        /*操作类型*/
        var operateDelete = 0, operateApprove = 1, operateUnapprove = 2, operatePrint = 3;
        //
        var draft = "-1", saved = "0", approved = "1", unsettled = "0", settled = "1";
        //
        var checkType = new GLCheckType();
        //
        var frozenItemID = "";
        //
        var settlementInfo = {};
        /*异步请求的一些url*/
        //获取凭证列表
        var getVoucherListUrl = "/GL/GLVoucher/GLGetVoucherPageList";
        //编辑凭证
        var editVoucherUrl = "/GL/GLVoucher/GLVoucherEdit";
        //
        var GL = "GL";
        //
        var draftLang = HtmlLang.Write(LangModule.Common, "Draft", "Draft");
        //
        var savedLang = HtmlLang.Write(LangModule.Common, "Saved", "Saved");
        //
        var noVoucher2Operate = HtmlLang.Write(LangModule.Common, "NoVoucher2Operate", "No Voucher to Operate");
        //
        var approvedLang = HtmlLang.Write(LangModule.Common, "Approved", "Approved");
        //
        var newVoucherLang = HtmlLang.Write(LangModule.Common, "newvoucher", "New Voucher");
        //
        var operationFail = HtmlLang.Write(LangModule.Common, "operationfail", "Operation Failded!");
        //
        var sureToSettle = HtmlLang.Write(LangModule.Common, "Areyousuretosettlethisperiod", "Are you sure to settle this period?");
        //
        var periodIsNotPassed = HtmlLang.Write(LangModule.Common, "periodIsNotPassed", "The period is not passed,");

        /*按钮的selector*/
        //新建
        var btnNew = ".vl-new-button";
        //审核
        var btnApprove = ".vl-approve-button ";
        //打印div
        var functionDiv = ".vl-function-div";
        //反审核
        var btnUnapprove = ".vl-unapprove-button";
        //打印
        var btnPrint = ".vl-print-button";

        //导出
        var btnExport = ".vl-export-buttonimg";
        //导入
        var btnImport = ".vl-import-buttonimg";

        //查询 div
        var divSearch = ".m-adv-search-voucher";
        var btnSearchdiv = ".m-adv-search-btn";
        var btnSearchclose = ".m-adv-search-close";
        
        //删除
        var btnDelete = ".vl-delete-button";
        //查找
        var btnSearch = "#aSearchVoucher";
        //清除
        var btnClear = "#aClearSearchFilter";
        //全选
        var btnCheckall = ".vl-checkall-input";
        //关键字
        var txtKeyword = ".vl-keyword-input";
        //金额
        var txtAmount = ".vl-Amount-input";
        //凭证编号
        var txtNumber = ".vl-number-input";
        //凭证期间
        var txtPeriod = "#txtPeriod";
        //凭证期间
        var txtPeriodEnd = "#txtPeriodEnd";
        //是否审核
        var txtStatus = ".vl-status-input";
        //排序选项
        var txtSortInput = ".vl-sort-input";
        //
        var txtFrom = ".vl-from-input";
        //表格
        var voucherTable = ".vl-voucher-table";
        //悬浮层
        var functionLayer = ".vl-function-layer";
        //
        var operateTop = ".vl-operate-top";
        //
        var entryList = ".vl-entry-list";
        //
        var advanceHref = ".vl-advance-href";
        //
        var advanceOption = ".vl-advance-option";
        //
        var noRecordRowClass = "vl-norecord-row";
        //
        var voucherTableHeader = ".vl-voucher-table-header";
        //
        var voucherHeaderDemoClass = "vl-v-h-demo";
        //
        var voucherEntryDemoClass = "v-v-e-demo";
        //
        var voucherTotalDemoClass = "vl-voucher-total-demo";
        //
        var voucherHeaderClass = "vl-v-h";
        //
        var voucherEntryClass = "v-v-e";
        //
        var voucherTotalClass = "vl-v-t";
        //
        var voucherDate = ".v-v-d";
        //
        var voucherNumber = ".v-v-n";
        //
        var voucherStatus = ".v-v-s";
        //
        var voucherTransferTitle = ".v-v-t-t";
        //
        var voucherTransfer = ".v-v-t";
        //
        var voucherSelect = ".m-v-s input[type='checkbox']";
        //
        var entryDiv = ".vl-entry-div";
        //
        var entryReference = ".m-e-r";
        //
        var entryAccount = ".m-e-a";
        //
        var entryCurrency = ".m-e-c";
        //
        var entryCheckGroup = ".m-e-ck";
        //
        var entryDebit = ".m-e-d";
        //
        var entryCredit = ".m-e-cd";

        var _pager = ".vc-pagenation-div";
        //
        var voucherHeaderDiv = ".m-v-c-d";
        //
        var voucherTotalDiv = ".vl-voucher-total-div";
        //
        var voucherTotalAmount = ".m-v-t-a";
        //隐藏
        var hide = "vc-hide";
        //鼠标移动到上面就显示编辑曾
        var voucherCol = "td.m-v-c";
        //
        var functionButton = ".vl-function-button";
        //审核
        var functionApprove = ".vl-function-approve";
        //反审核
        var functionUnapprove = ".vl-function-unapprove";
        //查看
        var functionView = ".vl-function-view";
        //编辑
        var functionEdit = ".vl-function-edit";
        //删除
        var functionDelete = ".vl-function-delete";
        //新增
        var functionNew = ".vl-function-new";
        //打印
        var functionPrint = ".vl-function-print";

        var functionDraft = ".vl-function-draft";
        var functionApproved = ".vl-function-approved";
        var functionSettled = ".vl-function-settled";
        var functionUnsettled = ".vl-function-unsettled";

        var radioSortByType = "input[name='sortByType']";
        var comboDateSortType = "#dateSortType";
        var comboNumberSortType = "#numberSortType";

        //跟踪项
        var trackDataScoure = null;
        //
        var GLVoucherList = function (home) {
            //
            var that = this;
            //0 按凭证号排序 1 按凭证日期排序  默认0
            this.sortByType = "0";
            //
            this.getYearPeriod = function () {
                //当前用户选择的日期
                var date = $(txtPeriod).val();
                var enddate = $(txtPeriodEnd).val();

                if (!date && !enddate) {
                    mDialog.message(HtmlLang.Write(LangModule.GL, "PleaseSelectAPeriod", "请选择一个会计期间"));
                    return false;
                }

                //默认为空
                var year = period = endYear = endPeriod = "";
                //如果不是日期就不说了
                if (date && mDate.parse(date + "-01")) {
                    //
                    year = date.split('-')[0];
                    //
                    period = date.split('-')[1];
                }

                if (enddate && mDate.parse(enddate + "-01")) {
                    //
                    endYear = enddate.split('-')[0];
                    //
                    endPeriod = enddate.split('-')[1];
                }

                return [year, period, endYear, endPeriod];
            };
            this.getFilterSet = function () {

                var yearPeriod = that.getYearPeriod();

                if (yearPeriod === false) return;
                //
                var number = $.isNumeric($(txtNumber).val()) ? GLVoucherHome.toVoucherNumber($(txtNumber).val()) : $(txtNumber).val();

                var keyWord = $(txtKeyword).val();
                var decimalKeyWord = $(txtAmount).val();

                if ($(txtStatus).attr("filter")) {
                    $(txtStatus).combobox("setValue", $(txtStatus).attr("filter"))
                }
                if ($(txtFrom).attr("filter")) {
                    $(txtFrom).combobox("setValue", $(txtFrom).attr("filter"))
                }

                that.showSortType();

                var sortType = "";
                if (that.sortByType == 0) {
                    sortType = $(numberSortType).combobox("getValue");
                }
                else {
                    sortType = $(comboDateSortType).combobox("getValue");
                }

                var filter = {
                    page: pageIndex,
                    rows: pageSize,
                    Year: yearPeriod[0],
                    Period: yearPeriod[1],
                    EndYear: yearPeriod[2],
                    EndPeriod: yearPeriod[3],
                    KeyWord: keyWord,
                    DecimalKeyWord: decimalKeyWord,
                    MNumber: number ? number : "",
                    Status: $(txtStatus).combobox("getValue"),
                    From: $(txtFrom).combobox("getValue"),
                    SortByType: that.sortByType,
                    SortType: sortType
                };

                return filter;
            };
            //获取凭证数据
            this.getVoucherListData = function (func) {

                $(btnCheckall).removeAttr("checked");

                mAjax.post(getVoucherListUrl, {
                    filter: that.getFilterSet()
                }, function (data) {
                    //
                    if (data.Success) {
                        //如果分页取不到数据，需要重新取一次
                        var total = data.Voucher.total;
                        if (total && total > 0 && pageSize != 0) {

                            if (total <= (pageIndex - 1) * pageSize) {
                                pageIndex -= 1;
                                that.getVoucherListData(func);
                                return;
                            }
                        }
                        //
                        var vouchers = data.Voucher;
                        //
                        var settlement = data.Settlement;
                        //调用回调函数
                        func && $.isFunction(func) && func(vouchers, settlement);
                    }
                }, "", true);
            };
            //初始化凭证列表
            this.initVoucherList = function (data, settlement) {
                //获取跟踪项
                var ds = mData.getNameValueTrackDataList();
                trackDataScoure = ds;
                //
                totalCount = data.total;
                //
                var rows = data.rows;
                //
                settlementInfo = settlement;
                //
                that.initSettlement();
                //总共的行数
                that.initPagerEvent();
                //获取table里面的body
                var $body = $("tbody", voucherTable);
                //先移除里面的没有记录的行
                $("." + noRecordRowClass).remove();
                //先清除里面的凭证
                $body.find("tr").not("tr[class*='-demo']").remove();


                var obj = data.obj;
                //如果没有凭证
                if (!obj) {
                    //加入空行
                    $body.append(that.getNoRecordRow());
                }
                else {
                    $body.append(obj);

                    that.initFuncionLayerEvent();
                }
                that.initFuncionLayerEvent();
                //初始化高度
                that.initEntryDiv();
                //滚动到最顶层
                GLVoucherHome.scrollToTop($("tbody", voucherTable));
                //
                $(_pager).show();
            };
            //显示审核反审核的按钮
            this.initSettlement = function () {
                $(btnApprove).show();
                $(btnUnapprove).show();
                $(btnDelete).show();
            };
            //创建一个凭证的html并加到表格里面
            this.createVouherHtml = function (voucher, tbody, isLast) {
                //
                var html = "";
                //获取头的demotr
                var $header = $("." + voucherHeaderDemoClass).clone();
                //去掉类
                $header.removeClass(voucherHeaderDemoClass).removeClass(hide).addClass(voucherHeaderClass).attr("VoucherID", voucher.MItemID);
                //显示日期
                $(voucherDate, $header).text(mDate.format(voucher.MDate));
                //显示凭证号
                $(voucherNumber, $header).text(voucher.MNumber);
                //显示凭证当前状态
                $(voucherStatus, $header).text(voucher.MStatus == approved ? approvedLang : savedLang);
                //是否是结转类型
                if (voucher.MTransferTypeID >= 0) {
                    //显示标题
                    $(voucherTransferTitle, $header).show();
                    //显示计提类型
                    $(voucherTransfer, $header).text(voucher.MTransferTypeName);
                }
                //
                tbody.append($header);
                //计算需要显示checkbox的行
                var checkboxRow = Math.floor((voucher.MVoucherEntrys.length + 1) / 2) - 1;
                //
                for (var i = 0; i < voucher.MVoucherEntrys.length ; i++) {
                    //
                    var entry = voucher.MVoucherEntrys[i];
                    //
                    var $entry = $("." + voucherEntryDemoClass).clone();
                    //去除class
                    $entry.removeClass(voucherEntryDemoClass).removeClass(hide).addClass(voucherEntryClass).show().attr("VoucherID", voucher.MItemID);
                    //如果本行需要显示checkbox
                    (i == checkboxRow) && $(voucherSelect, $entry).show().attr("VoucherID", voucher.MItemID);
                    //摘要
                    var expolantion = mText.encode(entry.MExplanation);
                    $(entryReference, $entry).mHtml(expolantion);
                    //科目
                    var account = mText.encode(entry.MAccountName);
                    $(entryAccount, $entry).text(account);
                    //外币核算显示
                    $(entryCurrency, $entry).html(checkType.getCurrencyHtml(entry.MAccountModel.MCurrencyDataModel, true));
                    //核算维度个数
                    var checkGroupCount = 0;
                    //核算维度html拼接
                    var checkGroupHtml = checkType.getCheckGroupValueHtml(entry.MAccountModel.MCheckGroupValueModel, entry.MAccountModel.MCheckGroupModel);
                    //
                    $(entryCheckGroup, $entry).html(checkGroupHtml);
                    //高度
                    var checkGroupHeight = checkGroupCount * 18;
                    $(entryCheckGroup, $entry).css("height", checkGroupHeight + "px");
                    //借方、贷方金额都为0
                    if (entry.MDebit == 0 && entry.MCredit == 0) {
                        if (entry.MDC == -1) {
                            $(entryCredit, $entry).text("0.00");
                        } else {
                            $(entryDebit, $entry).text("0.00");
                        }
                    } else {
                        //借方金额
                        entry.MDebit && entry.MDebit != 0 && $(entryDebit, $entry).text(mMath.toMoneyFormat(entry.MDebit));
                        //贷方金额
                        entry.MCredit && entry.MCredit != 0 && $(entryCredit, $entry).text(mMath.toMoneyFormat(entry.MCredit));
                    }

                    //加入
                    tbody.append($entry);
                    //
                }
                //
                var $total = $("." + voucherTotalDemoClass).clone();
                //
                $total.removeClass(voucherTotalDemoClass).removeClass(hide).addClass(voucherTotalClass).show().attr("VoucherID", voucher.MItemID);
                //显示数字
                $(voucherTotalAmount, $total).find("span").text(mMath.toMoneyFormat(voucher.MCreditTotal));
                //
                tbody.append($total);
                //如果不是最后一项，则每两个凭证之间加一个空行
                !isLast && tbody.append("<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>")
            };

            //根据跟踪项id获取跟踪项名
            this.GetMtrackItemName = function (track) {
                //没有数据返回
                if (track == null || track == "") {
                    return "";
                }
                //获取跟踪项数据
                var tds = trackDataScoure;
                if (tds != null && tds.length > 0) {
                    //遍历跟踪项
                    for (var i = 0; i < tds.length; i++) {
                        //获取孩子节点
                        var ds = tds[i].MChildren;
                        //遍历孩子节点
                        for (var j = 0; j < ds.length; j++) {
                            if (ds[j].MValue == track) {
                                //值相等返回名字
                                return tds[i].MName;
                            }
                        }
                    }
                }
                return "";
            };
            //根据核算维度获取核算维度的名字
            this.getCheckTypeName = function (type) {
                //
                switch (type) {
                    case "MContactID":
                        return HtmlLang.Write(LangModule.BD, "Contact", "联系人");
                    case "MEmployeeID":
                        return HtmlLang.Write(LangModule.BD, "Employee", "员工");
                    case "MMerItemID":
                        return HtmlLang.Write(LangModule.BD, "MerItem", "商品项目");
                    case "MExpItemID":
                        return HtmlLang.Write(LangModule.BD, "ExpenseItem", "费用项目");
                    case "MPaItemID":
                        return HtmlLang.Write(LangModule.BD, "PayItem", "工资项目");
                    default:
                        return "";
                }
            };
            //初始化页面的默认值
            this.initDomValue = function () {
                $(txtAmount).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Common, "Amount", "Amount")).initHint();
                //默认为当前日期
                $(txtPeriod).val(GLVoucherHome.avaliablePeriod());
                $(txtPeriodEnd).val(GLVoucherHome.avaliablePeriod());

                //是否审核
                $(txtStatus).combobox({
                    width: 110,
                    textField: 'text',
                    valueField: 'value',
                    data: [
                        {
                            text: HtmlLang.Write(LangModule.Common, 'Saved', "Saved"),
                            value: 0
                        },
                        {
                            text: HtmlLang.Write(LangModule.GL, 'Approved', "已审核"),
                            value: 1
                        }
                    ],
                    onSelect:function(){
                        $(txtStatus).attr("filter", null);
                    },
                    onLoadSuccess: function () {
                        //
                        var value = $(txtStatus).attr("filter") || "";
                        $(txtStatus).combobox("setValue", value);
                        !value && $(txtStatus).combobox("setText", "");
                    }
                });
                //凭证来源
                $(txtFrom).combobox({
                    width: 110,
                    textField: 'text',
                    valueField: 'value',
                    data: [
                        {
                            text: HtmlLang.Write(LangModule.Common, 'UserInput', "手工录入"),
                            value: 0
                        },
                        {
                            text: HtmlLang.Write(LangModule.Common, 'UserImport', "用户导入"),
                            value: 1
                        },
                        {
                            text: HtmlLang.Write(LangModule.Common, 'DocVoucher', "业务系统"),
                            value: 2
                        }
                        ,
                        {
                            text: HtmlLang.Write(LangModule.Common, 'FromApp', "其他应用导入"),
                            value: 3
                        }
                    ],
                    onSelect: function () {
                        $(txtFrom).attr("filter", null);
                    },
                    onLoadSuccess: function () {
                        //
                        var value = $(txtFrom).attr("filter") || "";
                        $(txtFrom).combobox("setValue", value);
                        !value && $(txtFrom).combobox("setText", "");
                    }
                });
                //升降序
                $(txtSortInput).combobox({
                    width: 75,
                    textField: 'text',
                    valueField: 'value',
                    data: [
                        {
                            text: HtmlLang.Write(LangModule.Common, 'Desc', "降序"),
                            value: "0"
                        },
                        {
                            text: HtmlLang.Write(LangModule.Common, 'Asc', "升序"),
                            value: "1"
                        }
                    ]
                });

                //默认降序
                $(comboDateSortType).combobox("setValue", "0");
                $(comboNumberSortType).combobox("setValue", "0");
            };
            //添加空行
            this.getNoRecordRow = function () {
                //
                return "<tr class='" + noRecordRowClass + "'><td class='dv-norecord-td' colspan='7'>" + HtmlLang.Write(LangModule.Common, "NoRecords", "No Records.") + "</td></tr>";
            };
            //初始化凭证鼠标移动到上面显示功能操作层的事件
            this.initFuncionLayerEvent = function () {
                //获取单个td
                var $td = $("." + voucherEntryClass + ",." + voucherTotalClass).find("td");
                //绑定事件
                $td.off("mouseover").on("mouseover", function () {
                    //显示功能层
                    that.showFunctionLayer($(this));
                }).off("dblclick").on("dblclick", function () {
                    //
                    frozenItemID = $(this).parent().attr("voucherID");

                    //如果只有一条的情况下，双击需要再次显示
                    if ($(functionLayer).is(":hidden")) {
                        //
                        frozenItemID = "";
                        //不进行下方的内容
                        $(functionLayer).show();
                    }
                });
                //
                $(functionLayer).off("mouseleave").on("mouseleave", function () {
                    //隐藏
                    $(this).hide();
                    //
                    $(functionLayer).attr("voucherID", "");
                });
                $(functionLayer).off("dblclick").on("dblclick", function () {
                    //隐藏
                    $(this).hide();
                    //
                    frozenItemID = $(this).attr("voucherID");
                });
            };
            this.checkIsSettled = function (year, period) {
                return parseInt(settlementInfo.CurrentPeriod) > parseInt(year) * 100 + parseInt(period);
            };
            //显示功能层
            this.showFunctionLayer = function ($td) {
                //获取本行
                var $tr = $td.parent();
                //凭证ID
                var itemID = $tr.attr("voucherID");
                var status = $tr.attr("status");
                var year = $tr.attr("year");
                var period = $tr.attr("period");

                //如果用户已经双击过的，则不需要再显示了
                if (itemID == frozenItemID) {
                    //如果用户已经双击过的，则不需要再显示了
                    return false;
                }
                else {
                    //晴空
                    frozenItemID = "";
                }
                //
                $(functionLayer).attr("voucherID", itemID);
                //获取到当前的voucher
                var voucher = { MItemID: itemID, MStatus: status, MYear: year, MPeriod: period  };
                //
                var $firstTr = $tr.hasClass(voucherHeaderClass) ? $tr : $tr.prevAll("." + voucherHeaderClass).first();
                //获取Number
                var itemNumber = $(voucherNumber, $firstTr).text();
                //初始化位置
                that.initFunctionLayerDom($tr);
                //
                $(functionLayer).mScroll($("tbody", voucherTable));
                //如果已经结算了，则只能查看
                if (that.checkIsSettled(voucher.MYear, voucher.MPeriod)) {
                    $(functionUnsettled).hide();
                    $(functionSettled).show();
                }
                else {
                    $(functionUnsettled).show();
                    $(functionSettled).show();
                    //如果已经审核
                    if (voucher.MStatus == approved) {
                        //隐藏
                        $(functionDraft).hide();
                        //
                        $(functionApproved).show();
                    }
                    else {
                        //隐藏
                        $(functionDraft).show();
                        //
                        $(functionApproved).hide();
                    }
                }
                //如果有业务单据关联，删除键去掉
                //voucher.MDocID && $(functionDelete).hide();
                //审核
                $(functionApprove).off("click").on("click", function () {
                    //
                    that.approveVoucher(approved, itemID);
                });
                //反审核
                $(functionUnapprove).off("click").on("click", function () {
                    //
                    that.approveVoucher(saved, itemID);
                });
                //编辑
                $(functionEdit).off("click").on("click", function () {
                    //
                    that.editVoucher(itemID, itemNumber)
                });
                //编辑
                $(functionView).off("click").on("click", function () {
                    //
                    that.editVoucher(itemID, itemNumber)
                });
                //删除
                $(functionDelete).off("click").on("click", function () {
                    //
                    that.deleteVoucher(itemID, itemNumber);
                });
                //打印
                $(functionPrint).off("click").on("click", function () {
                    //
                    that.printVoucher(itemID);
                })
            };
            //初始化功能遮罩层的位置大小
            this.initFunctionLayerDom = function ($tr) {
                //获取第一行
                var $firstTr = $tr.hasClass(voucherHeaderClass) ? $tr : $tr.prevAll("." + voucherHeaderClass).first();
                //找到本凭证的最后一行
                var $lastTr = $tr.hasClass(voucherTotalClass) ? $tr : $tr.nextAll("." + voucherTotalClass).first();
                //
                $(functionLayer).stop();
                //
                var scrollTop = $("tbody", entryDiv).scrollTop();
                //
                var headerOffsetTop = $(operateTop).offset().top + $(operateTop).height() + 10 + $("thead", voucherTable).height();
                if ($(divSearch).is(':visible')) {
                    headerOffsetTop += $(divSearch).outerHeight() + 15;  //margin-top 5  margin-bottom 10
                }
                //
                var diff = ($firstTr.offset().top + $firstTr.height()) < headerOffsetTop ? ($firstTr.offset().top + $firstTr.height() - headerOffsetTop) : 0;
                //
                var buttomOffset = 0;
                //
                var height = ($lastTr.offset().top - $firstTr.offset().top + $lastTr.outerHeight() - $firstTr.outerHeight() + diff);
                //如果上面没有遮挡，看下方是否有遮挡
                //
                var buttomOffset = $lastTr.offset().top + $lastTr.height() - $(entryDiv).height() - $(entryDiv).offset().top;
                //如果底下显示不全
                buttomOffset = buttomOffset > 0 ? buttomOffset : 0;
                //
                $(functionLayer).animate({
                    //宽度
                    width: $(voucherCol, $firstTr).outerWidth() + "px",
                    //高度
                    height: (height - buttomOffset) + "px",
                    //位置
                    left: $(voucherCol, $firstTr).offset().left + "px",
                    //顶部
                    top: ($firstTr.offset().top + $firstTr.outerHeight() - diff) + "px"
                }, 200, "swing", function () {
                    $(functionButton).css({
                        "margin-top": (((height - buttomOffset - $(functionButton).height()) / 2)) + "px"
                    });
                }).show();
                //计算图片的位置

            };
            //初始化entrydiv的高度
            this.initEntryDiv = function () {
                //
                var tBody = $("tbody", voucherTable);
                //计算高度
                $(entryDiv).height($(window).height() - $(entryDiv).offset().top - $(_pager).height() - 4);
                //
                tBody.css({
                    "height": ($(entryDiv).height() - $(voucherTableHeader).height()) + "px",
                    "margin-top": $(voucherTableHeader).height()
                });
                //其滚动的时候隐藏悬浮层
                $(tBody).scroll(function () {
                    //显示虚线
                    if (this.scrollTop > 0) {
                        $(this).addClass(dashedLineClassName);
                    }
                    else {
                        $(this).removeClass(dashedLineClassName);
                    }

                    //隐藏
                    $(functionLayer).hide();
                });
                //底部打印按钮的位置
                that.initFunctionDivLocation();
                //
                $("tbody", voucherTable).mScroll();
            };

            //固定导出按钮的位置
            this.initFunctionDivLocation = function () {
                //底部打印按钮的位置
                $(functionDiv).css({
                    top: ($(window).height() - $(functionDiv).height() - 3) + "px"
                });
            }
            //初始化按钮的点击事件
            this.initButtonEvent = function () {
                //新增
                $(btnNew).off("click").on("click", that.newVoucher);
                //审核
                $(btnApprove).off("click").on("click", function () {
                    that.approveVoucher(approved, null);
                });
                //反审核
                $(btnUnapprove).off("click").on("click", function () {
                    that.approveVoucher(saved, null);
                });
                //导入
                $(btnImport).off("click").on("click", that.importVoucher);
                //打印
                $(btnPrint).off("click").on("click", function () {
                    var itemIds = that.getSelectedVoucher(true, operatePrint);
                    if (itemIds.length > 0) {
                        that.printVoucher(itemIds.join());
                    }
                    else {
                        $.mDialog.warning(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
                    }
                });
                //导出
                $(btnExport).off("click").on("click", that.exportVoucher);
                //删除
                $(btnDelete).off("click").on("click", function () {
                    that.deleteVoucher();
                });
                //搜索
                $(btnSearch).off("click").on("click", function () {
                    pageIndex = 1;
                    that.refresh();
                });

                //清空
                $(btnClear).off("click").on("click", that.clear);

                //显示搜索div
                $(btnSearchdiv).off("click").on("click", function () {
                    $(btnSearchdiv).hide();
                    $(divSearch).show();
                    that.initEntryDiv();
                });

                //隐藏搜索div
                $(btnSearchclose).off("click").on("click", function () {
                    $(btnSearchdiv).show();
                    $(divSearch).hide();
                    that.initEntryDiv();
                });

                $(radioSortByType).off("click").on("click", function () {
                    that.sortByType = $(radioSortByType + ":checked").val();
                    that.showSortType();
                });

                $(btnCheckall).off("click").on("click", that.selectAll);
                //高级选项
                $(advanceHref).mTip(
                    {
                        target: $(advanceOption),
                        width: 235,
                        parent: $(advanceOption).parent()
                    });

                //默认展开
                $(btnSearchdiv).trigger("click");

                
            };
            //设置 排序相关的显示&隐藏
            this.showSortType = function () {
                $(comboDateSortType).next(".combo").hide();
                $(comboNumberSortType).next(".combo").hide();
                if (that.sortByType == "1") {
                    $('#dateSort').prop("checked", true)
                    $(comboDateSortType).next(".combo").show();
                }
                else if (that.sortByType == "0") {
                    $('#numberSort').prop("checked", true)
                    $(comboNumberSortType).next(".combo").show();
                }
            };
            //审核
            this.approveVoucher = function (status, itemID) {
                if (!itemID) {
                    //是批量操作 检查是否跨期
                    if (that.checkIsSpanPeriod()) {
                        $.mDialog.warning(HtmlLang.Write(LangModule.Common, "VoucherSpanApprove", "不支持跨期批量操作!"));
                        return;
                    }
                }

                //获取勾选到的凭证数
                var itemIDs = itemID ? [itemID] : that.getSelectedVoucher(true, status == 1 ? operateApprove : operateUnapprove);
                //
                if (itemIDs.length > 0) {
                    //调用首页的方法
                    home.approveVoucher(itemIDs, status, that.refresh);
                }
                else {
                    //
                    home.showNoVouchers2Operate();
                }
            };
            //
            this.refresh = function () {
                //
                var yearPeriod = that.getYearPeriod();

                if (yearPeriod === false) return;

                //弹出框隐藏
                $(functionLayer).hide();
                //重新获取数据
                that.getVoucherListData(that.initVoucherList);

                GLVoucherHome.clearQueryDate();
            };
            //全选与取消全选
            this.selectAll = function () {
                //判断是否勾选
                ($(btnCheckall).attr("checked") == "checked") ? $(voucherSelect).attr("checked", true) : $(voucherSelect).attr("checked", false);
            };
            //导入
            this.importVoucher = function () {
                //调用home的
                home.importVoucher();
            };
            //导出查询参数
            this.getSearchParam = function () {
                var filter = that.getFilterSet();
                filter.page = undefined;
                filter.rows = undefined;
                return filter;
            },
            //打印凭证
            this.printVoucher = function (itemID) {
                //调用首页
                home.printVoucher(itemID);
            },
            //导出
            this.exportVoucher = function () {
                //调用home的方法
                home.exportVoucher(that.getSearchParam());
            };
            //删除
            this.deleteVoucher = function (itemID) {
                if (!itemID) {
                    //是批量操作 检查是否跨期
                    if (that.checkIsSpanPeriod()) {
                        $.mDialog.warning(HtmlLang.Write(LangModule.Common, "VoucherSpanApprove", "不支持跨期批量操作!"));
                        return;
                    }
                }

                //获取勾选的凭证
                var itemIDs = itemID ? [itemID] : that.getSelectedVoucher(true, operateDelete);
                //必须有勾选在做删除
                if (itemIDs.length > 0) {
                    //删除
                    home.deleteVoucher(itemIDs, that.refresh);
                }
                else {
                    //
                    home.showNoVouchers2Operate();
                }
            };
            //编辑凭证
            this.editVoucher = function (itemID, itemNumber) {
                //跳转到编辑页面 
                mTab.addOrUpdate(GL + "-" + itemNumber, editVoucherUrl + "?MItemID=" + itemID, false, true, true, true);
            };
            //新建凭证
            this.newVoucher = function () {
                //
                var date = that.getYearPeriod();

                if (date === false) return;
                //跳转到编辑页面 
                mTab.addOrUpdate(newVoucherLang, editVoucherUrl + "?year=" + date[0] + "&period=" + date[1], true, true, true, true);
            };
            //清除查找条件
            this.clear = function () {
                //
                $(txtKeyword).val("");
                $(txtAmount).numberbox("setValue","");
                //
                $(txtNumber).val("");
                //
                $(txtStatus).combobox("setValue", "");
                //
                $(txtFrom).combobox("setValue", "");
                //
                $(txtStatus).combobox("setText", "");
                //
                $(txtStatus).removeAttr("filter");
                //
                $(txtFrom).combobox("setText", "");
                //
                $(txtFrom).removeAttr("filter");

                //默认为当前日期
                $(txtPeriod).val(GLVoucherHome.avaliablePeriod());
                $(txtPeriodEnd).val(GLVoucherHome.avaliablePeriod());

                that.sortByType = "0";
                that.showSortType();

                $(comboDateSortType).combobox("setValue", "0");
                $(comboNumberSortType).combobox("setValue", "0");
            };
            //获取勾选上的凭证列表
            this.getSelectedVoucher = function (ids, operateType) {
                //
                var selectedVouchers = [];
                //
                var selectedIds = [];
                //获取
                $(voucherSelect).not(":hidden").filter(function () { return $(this).attr("checked") == "checked"; }).each(function () {
                    //
                    var selectedVoucher =
                    {
                        MItemID: $(this).parent().parent().attr("VoucherID"),
                        MStatus: $(this).parent().parent().attr("status")
                    };

                    //下面的逻辑故意分开写的，请不要合并

                    //审核的时候，只能审核草稿状态的
                    if (operateType == operateApprove && selectedVoucher.MStatus == saved) {
                        //加入到已经选择的里面
                        selectedVouchers.push(selectedVoucher);
                        //
                        selectedIds.push(selectedVoucher.MItemID);
                    }

                    //反审核的时候，只能反审核已经审核的
                    if (operateType == operateUnapprove && selectedVoucher.MStatus == approved) {
                        //加入到已经选择的里面
                        selectedVouchers.push(selectedVoucher);
                        //
                        selectedIds.push(selectedVoucher.MItemID);
                    }

                    //打印能打印所有的
                    if (operateType == operatePrint) {
                        //加入到已经选择的里面
                        selectedVouchers.push(selectedVoucher);
                        //
                        selectedIds.push(selectedVoucher.MItemID);
                    }

                    //删除只能删除未审核的
                    if (operateType == operateDelete && selectedVoucher.MStatus == saved) {
                        //加入到已经选择的里面
                        selectedVouchers.push(selectedVoucher);
                        //
                        selectedIds.push(selectedVoucher.MItemID);
                    }
                });
                //返回是id的列表还是凭证的列表
                return ids ? selectedIds : selectedVouchers;
            };
            //检查选择是否有跨期数据
            this.checkIsSpanPeriod = function () {
                var year = "";
                var period = "";
                var spanPeriod = false;

                //获取
                $(voucherSelect).not(":hidden").filter(function () { return $(this).attr("checked") == "checked"; }).each(function () {
                    var selectedVoucher =
                    {
                        MYear: $(this).parent().parent().attr("year"),
                        MPeriod: $(this).parent().parent().attr("period"),
                    };

                    if (year.length == 0 || period.length == 0) {
                        //记录首条的期间年份
                        year = selectedVoucher.MYear;
                        period = selectedVoucher.MPeriod;
                    }
                    else {
                        //判断是否于首条数据的期间年份 是否一致，不一致就要提示不允许跨期操作
                        if (selectedVoucher.MYear != year ||
                            selectedVoucher.MPeriod != period) {
                            spanPeriod = true;
                            return;
                        }
                    }
                });
                return spanPeriod;
            };
            //设置参数
            this.setFilter = function (status, from) {
                //
                $(txtStatus).attr("filter", status);
                //
                $(txtFrom).attr("filter", from);
            };
            //重置分页参数
            this.resetPager = function () {
                //初始化的时候，要把参数都置位原始值
                totalCount = 0;
                //
                pageIndex = 1;
                //
                pageSize = GLVoucherHome.pageSize;

            };
            //初始化翻页事件
            this.initPagerEvent = function () {
                //调用easyui组件
                $(_pager).pagination({
                    total: totalCount,
                    pageSize: pageSize,
                    pageNumber: pageIndex,
                    pageList: GLVoucherHome.pageList,
                    onSelectPage: function (number, size) {
                        pageIndex = number;
                        pageSize = GLVoucherHome.pageSize = size;
                        that.getVoucherListData(that.initVoucherList);
                    }
                });
            };
            //
            this.reset = function () {

                this.clear();
            };
            //
            this.init = function () {
                //初始化页面上的时间
                that.initDomValue();
                //重置参数
                that.resetPager();
                //初始化导出按钮的位置
                that.initFunctionDivLocation();
                //获取凭证数据
                that.getVoucherListData(that.initVoucherList);
                //初始化事件
                that.initButtonEvent();
                //初始化分页事件
                that.initPagerEvent();
            };
        };
        //
        return GLVoucherList;
    })();
    //
    w.GLVoucherList = GLVoucherList;
})(window)