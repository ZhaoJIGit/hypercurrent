

(function () {

    var GLDocVoucher = (function () {
        var GLDocVoucher = function () {
            //
            var that = this;

            var Draft = 1, WaitingApproval = 2, WaitingPayment = 3, Paid = 4;
            //
            var tab = "#tabDocVoucherTab";
            //
            var keyword = ".dv-keyword-input:visible";
            //
            var number = ".dv-number-input:visible";
            //
            var status = ".dv-status-input";
            //
            var period = ".dv-date-input:visible";
            //
            var search = ".dv-search-button";
            //
            var clear = ".dv-clear-button";
            //
            var edit = ".dv-edit-button";
            //
            var create = ".dv-create-button";
            //
            var batchSetup = ".dv-batch-setup-button";

            var depreciateButton = ".dv-depreciate-button";
            //
            var del = ".dv-delete-button";

            var reset = ".dv-reset-button";
            //
            var docPart = ".dv-doc-part";
            //
            var voucherPart = ".dv-voucher-part";
            //展开
            var collapsedownButton = ".dv-collapsedown-button";
            //折叠
            var collapseupButton = ".dv-collapseup-button";
            //
            var functionCreate = ".dv-create";
            //
            var functionDelete = ".dv-delete";
            //
            var functionView = ".dv-view";
            //
            var _pager = ".dv-pagination";
            //
            var dvBorder = ".dv-border";
            //
            var dvEmpty = ".dv-empty";
            //
            var dvTr = ".dv-tr";
            //
            var settlementInfo = {};
            //
            var noRecordRowClass = "dv-norecord-row";
            //
            var absoluteHeader = "dv-absolute-header";

            var getDocVoucherUrl = "/GL/GLVoucher/GetDoc2Voucher";
            //删除凭证
            var deleteDocVoucherUrl = "/GL/GLVoucher/GLDeleteDocVoucher";
            //查看凭证
            var editDocVoucherUrl = "/GL/GLVoucher/GLDocVoucherEdit";
            //编辑发票url
            var invoiceEditUrl = "/IV/Invoice/Invoice";
            //
            var redInvoiceEditUrl = "/IV/Invoice/CreditNote";
            //编辑采购单
            var purchaseEditUrl = "/IV/Bill/Bill";
            //
            var redPurchaseEditUrl = "/IV/Bill/CreditNote";
            //编辑费用报销
            var expenseEditUrl = "/IV/Expense/Expense";
            //编辑收款单
            var receiveEditUrl = "/IV/Receipt/Receipt";
            //编辑付款单
            var paymentEditUrl = "/IV/Payment/Payment";
            //编辑转帐单
            var transferEditUrl = "/IV/IVTransfer/IVTransferHome?dialog=1&MID=";
            //
            var invAmountHint = HtmlLang.Write(LangModule.GL, "InvAmount", "单据编号 ＃ 金额");

            var amountHint = HtmlLang.Write(LangModule.Common, "Amount", "Amount");
            //保存凭证生成原则
            var saveDocVoucherRuleUrl = "/GL/GLVoucher/SaveDocVoucherRule";
            //当前编辑的行DocID，便于编辑完成以后再滚动到当前行
            var currentEditEntryID = "";

            //
            var home = new GLVoucherHome();

            //
            var newVoucherLang = HtmlLang.Write(LangModule.Common, "newvoucher", "New Voucher");
            //
            var GL = "GL";
            //
            var sureToDelte = HtmlLang.Write(LangModule.Common, "Areyousuretodelete", "Are you sure to delete");
            //
            var operationFail = HtmlLang.Write(LangModule.Common, "operationfail", "Operation Failded!");
            //
            var createSuccessLang = HtmlLang.Write(LangModule.Common, "CreateSuccessfully", "Create Successfully!");
            //
            var multipleLang = HtmlLang.Write(LangModule.Common, "Multipile", "Multipile...");
            //
            var noUnapprovedVouchersToDelete = HtmlLang.Write(LangModule.Common, "NoUnapprovedVouchersToDelete", "No unapproved vouchers to be delete!");
            //
            var noVoucherCanBeCreate = HtmlLang.Write(LangModule.Common, "NoVoucherCanBeCreate", "No voucher can be create!");
            //
            var pleaseSelectRow = HtmlLang.Write(LangModule.Common, "PleaseSelectRowsBeforeOperate", "请选择需要操作的行!");
            //
            var noDocument2Operate = HtmlLang.Write(LangModule.Common, "NoDocument2Operate", "没有可以操作的单据!");
            //提醒用户在批量生成前需要用户确认的内容
            var createDocVoucherConfirm = "<div style='text-align:left;margin-left: 10px;'>" + HtmlLang.Write(LangModule.Common, "ConfirmCreateDocVoucher", "请在生成凭证前做以下确认:<br>1.所选单据如果是发票/费用报销单，请先确认其已经审核，其他单据未处于归档状态;<br>2.生成凭证所对应的科目已经设置(含税单据，税金科目必须设置);<br>3.单据未生成凭证.<br>确定要生成凭证吗?") + "</div>";
            //
            var saleTable = ".dv-sale-table";
            //
            var purchaseTable = ".dv-purchase-table";
            //
            var expenseTable = ".dv-expense-table";
            //
            var paymentTable = ".dv-payment-table";
            //
            var receiveTable = ".dv-receive-table";
            //
            var transferTable = ".dv-transfer-table";
            //
            var fixAssetsTable = ".dv-fixassets-table";
            //
            var docDiv = ".dv-doc-div";
            //
            var docCheck = ".dv-doc-check";
            //
            var tableDiv = ".dv-table-div";
            //
            var demoTrClass = "demo-tr";
            //
            var dvPartial = ".dv-partial";
            //
            var dvFunctionTd = ".dv-doc-td,.dv-voucher-td";
            //
            var functionLayer = ".dv-function-layer";
            //
            var docEdit = ".dv-doc-edit";
            //
            var docCollspan = ".dv-doc-collspan";
            //
            var docExpand = ".dv-doc-expand";
            //
            var voucherView = ".dv-voucher-view";
            //
            var voucherEdit = ".dv-voucher-edit";
            //
            var voucherRuleEdit = ".dv-voucher-rule-edit";
            //
            var voucherCreate = ".dv-voucher-create";
            //
            var voucherDelete = ".dv-voucher-delete";
            //
            var functionButton = ".dv-function-button";
            //
            var functionIco = ".dv-function-ico";
            //
            var partialDemo = ".dv-partial-demo";
            //各个分页控件的值
            var typeParams = [
                { pageIndex: 1, totalCount: 0, colspan: 14, mergeCol: 5, leftPart: 8, rightPart: 4, showCollapse: true, tableClass: saleTable, docVoucherData: [], frozenItemID: "" },
                { pageIndex: 1, totalCount: 0, colspan: 14, mergeCol: 5, leftPart: 8, rightPart: 4, showCollapse: true, tableClass: purchaseTable, docVoucherData: [], frozenItemID: "" },
                { pageIndex: 1, totalCount: 0, colspan: 13, mergeCol: 4, leftPart: 7, rightPart: 4, showCollapse: true, tableClass: expenseTable, docVoucherData: [], frozenItemID: "" },
                { pageIndex: 1, totalCount: 0, colspan: 13, mergeCol: 4, leftPart: 7, rightPart: 4, showCollapse: true, tableClass: receiveTable, docVoucherData: [], frozenItemID: "" },
                { pageIndex: 1, totalCount: 0, colspan: 13, mergeCol: 4, leftPart: 7, rightPart: 4, showCollapse: true, tableClass: paymentTable, docVoucherData: [], frozenItemID: "" },
                { pageIndex: 1, totalCount: 0, colspan: 12, mergeCol: 0, leftPart: 5, rightPart: 4, showCollapse: false, tableClass: transferTable, docVoucherData: [], frozenItemID: "" },
                { pageIndex: 1, totalCount: 0, colspan: 12, mergeCol: 0, leftPart: 8, rightPart: 1, showCollapse: false, tableClass: fixAssetsTable, docVoucherData: [], frozenItemID: "" }
            ];

            //定义各个tab对应的index
            var sale = 0, purchase = 1, expense = 2, receive = 3, payment = 4, transfer = 5, fixassets = 6;
            //初始化Tab
            this.initTab = function (index) {
                //初始化
                $(tab).tabs({
                    //默认的显示标签
                    selected: index,
                    //选择标签函数
                    onSelect: function (title, index) {
                        //
                        that.showTab(index);
                    }
                });
            };

            //显示tab
            this.showTab = function (index) {
                //当前需要显示的内容
                var div = $(".dv-partial-" + index);
                //是否已经初始化
                var inited = div.attr("inited") == "1";
                //根据index的不同，控制partial的显示
                div.show().siblings().hide();
                //隐藏所有的悬浮框
                $(functionLayer).hide();
                //
                $("li:eq(" + index + ")", tab).addClass("tabs-selected").siblings().removeClass("tabs-selected");

                if (index == 5) {
                    $(reset, div).remove();
                }
                //如果没有初始化过
                if (!inited) {
                    //初始化各个值
                    that.initDomValue(div);
                    //初始化事件
                    that.initEvent(div);                    
                }
                //初始化
                that.searchDocVoucher(div);
                //
                div.attr("inited", "1");
            };
            //显示按钮的隐藏
            this.hideShowButtonBySettlement = function ($div) {
                //
                var settled = settlementInfo.MStatus == "1";
                //
                settled ? $(batchSetup, $div).hide() : $(batchSetup, $div).show();
                //
                settled ? $(create, $div).hide() : $(create, $div).show();
                //
                settled ? $(del, $div).hide() : $(del, $div).show();
                //
                settled ? $(reset, $div).hide() : $(reset, $div).show();
                //
                settled ? $(depreciateButton, $div).hide() : $(depreciateButton, $div).show();
            }
            //获取设置的条件
            this.getFilterSet = function ($div) {
                var date = that.getSelectedDate($div);

                if (date === false) return false;

                var year = date.getFullYear();
                var month = date.getMonth() + 1;
                //
                var type = parseInt($div.attr("docType"));

                //
                if ($(status, $div).attr("filter")) {
                    //
                    $(status, $div).combobox("setValue", $(status, $div).attr("filter"));
                    //进入到页面之后，需要晴空filter里面的值
                    //$(status, $div).attr("filter", null);
                }
                //
                var filter = {
                    Keyword: $(keyword, $div).val(),
                    DecimalKeyword: $(keyword, $div).val().toNullableNumber(),
                    DatetimeKeyword: mDate.isDateString($(keyword, $div).val()) ? $(keyword, $div).val() : null,
                    Number: $(number, $div).val(),
                    Status: type == fixassets ? "" : $(status, $div).combobox("getValue"),
                    Year: year,
                    Period: month,
                    Type: type,
                    rows: GLVoucherHome.pageSize,
                    page: typeParams[type].pageIndex
                };
                //返回
                return filter;
            };
            //初始化各个dom的值
            this.initDomValue = function ($div) {
                //
                $(keyword, $div).val("");
                //
                $(number, $div).val("");
                //
                if ($div.hasClass("dv-partial-0")) {
                    //
                    $(number, $div).removeAttr("hint").attr("hint", invAmountHint).initHint();
                }
                else {

                    if (!$(number, $div).attr("hint")) {
                        $(number, $div).removeAttr("hint").attr("hint", amountHint).initHint();
                    }
                }
                //默认为当前日期
                $(period, $div).val(GLVoucherHome.avaliablePeriod());
                //
                $(status, $div).val("");
                //是否审核
                $(status, $div).combobox({
                    width: 80,
                    textField: 'text',
                    valueField: 'value',
                    data: [
                        {
                            text: HtmlLang.Write(LangModule.Common, 'Created', "Created"),
                            value: 0
                        },
                        {
                            text: HtmlLang.Write(LangModule.Common, 'Uncreated', "Uncreated"),
                            value: -1
                        }
                    ],
                    onSelect: function () {
                        $(status, $div).attr("filter", null);
                    },
                    onLoadSuccess: function () {
                        //
                        var value = $(status, $div).attr("filter") || "";
                        $(status, $div).combobox("setValue", "");
                        !value && $(status, $div).combobox("setText", "");
                    }
                });
            };
            //获取数据
            this.getDocVoucher = function (type, func) {
                //
                var $div = $("div[docType='" + type + "']");
                //
                that.initMainDivSize($div);
                //
                var filter = that.getFilterSet($div);

                if (filter === false) return;

                //获取数据
                home.getDocVoucher(filter, function (data, settlement) {
                    //
                    settlementInfo = settlement;
                    //处理结算新xi
                    that.hideShowButtonBySettlement($div);
                    //处理前面的
                    var $tableBody = that.prehandleInitTable(type, data);
                    //处理没有数据时候的表格宽度
                    that.handleTableThWithIE($("tbody", typeParams[type].tableClass).parent());
                    //获取表格
                    if ($tableBody) {
                        //展示表格
                        $tableBody = func($div, $tableBody, type);
                        //处理后面的
                        that.afterhandleInitTable($div, $tableBody, type);
                        //
                        $tableBody.mScroll();
                        //
                        //GLVoucherHome.scrollToTop($tableBody);
                    }
                    else {
                        //
                        that.initMainDivSize($div);
                    }
                });
            };
            //查询事件
            this.searchDocVoucher = function ($div) {
                //隐藏遮罩层
                $(functionLayer, $div).hide();
                //重置翻页
                var type = parseInt($div.attr("docType"));

                $("input[type='checkbox']", $div).removeAttr("checked");

                //不同类型
                switch (type) {
                    case sale:
                    case purchase:
                        that.getDocVoucher(type, that.initInvoiceDocVoucher);
                        break;
                    case expense:
                        that.getDocVoucher(type, that.initExpenseDocVoucher);
                        break;
                    case receive:
                    case payment:
                        that.getDocVoucher(type, that.initReceivePaymentDocVoucher);
                        break;
                    case transfer:
                        that.getDocVoucher(type, that.initTransferDocVoucher);
                        break;
                    case fixassets:
                        that.getDocVoucher(type, that.initFixAssetsDocVoucher);
                        break;
                    default:
                        break;
                }
                var date = that.getSelectedDate($div);

                if (date === false) return;

                //处理业务单据转凭证的数量
                home.updateDoc2VoucherNumber(date);
            };
            //获取用户选的期
            this.getSelectedDate = function ($div) {
                //
                var date = $(period, $div).val();

                if (!date) {
                    mDialog.message(HtmlLang.Write(LangModule.GL, "PleaseSelectAPeriod", "请选择一个会计期间"));
                    return false;
                }
                //
                if (date && mDate.parse(date + "-01")) {
                    //
                    return mDate.parse(date + "-01");
                }
                return false;
            };
            //根据业务单来获取信息
            this.findDocVoucher = function (docID, entryID, type) {
                //
                var result = [];
                //
                for (var i = 0; i < typeParams[type].docVoucherData.length ; i++) {
                    if (type == fixassets && typeParams[type].docVoucherData[i].MID == docID) {
                        result.push(typeParams[type].docVoucherData[i]);
                    }
                        //
                    else if (typeParams[type].docVoucherData[i].MDocID == docID && (!entryID || typeParams[type].docVoucherData[i].MEntryID == entryID)) {
                        result.push(typeParams[type].docVoucherData[i]);
                    }

                }
                //
                return result;
            };
            //处理表格初始化前的事物
            this.prehandleInitTable = function (type, data) {
                //
                typeParams[type].docVoucherData = data.rows;
                //
                typeParams[type].totalCount = data.total;
                //获取tablebody
                var $tableBody = $("tbody", typeParams[type].tableClass);
                //除了demo行，其余的都移除
                $("tr:not(." + demoTrClass + ")", $tableBody).remove();
                //
                var $div = that.getDivByType(type);
                //显示合并按钮
                if ($(collapsedownButton, $div).is(":hidden") && $(collapseupButton, $div).is(":hidden") && typeParams[type].showCollapse) {
                    //默认显示折叠
                    $(collapsedownButton, $div).show();
                    //
                    $(collapseupButton, $div).hide();
                }
                //如果是空的
                if (!data.rows || data.rows.length == 0) {
                    //加入一行空行
                    ($("." + noRecordRowClass, $tableBody).length == 0) && $tableBody.append(that.getNoRecordRow(type));
                    //
                    that.initPagerEvent($div);
                    //
                    return false;
                }
                //然会tablebody
                return $tableBody;
            };
            this.handleTableThWithIE = function ($tableBody) {
                //
                if (navigator.userAgent.indexOf("MSIE") > 0 && navigator.userAgent.indexOf("MSIE 9.0") > 0) {

                    var headerWidth = $("thead", $tableBody).width();

                    var ths = $("thead th:visible", $tableBody);

                    var headerWidth = $("thead", $tableBody).width();
                    for (var i = 0 ; i < 5 ; i++) {

                        var widthString = ths.eq(i).attr("style").replace("width:", "");

                        var width = 0;
                        //第一次加载的时候使用百分号的情况
                        if (widthString && widthString.indexOf("%") > 0 && i !== ths.length - 1) {

                            percentStyle = true;

                            width = +widthString.replace("%;", '') / 100 * headerWidth;

                            ths.eq(i).width(width);
                        }
                    }
                }
            }
            //
            this.handleTableTdWidth = function ($tableBody) {
                //
                var ths = $tableBody.parent().find("thead tr th:visible");
                //
                var firstTrTds = $(dvTr + ":visible", $tableBody).eq(0).find("td:visible");
                //
                var needBreakTdIndex = [];
                //
                for (var i = 0; i < firstTrTds.length ; i++) {
                    //
                    var td = firstTrTds.eq(i);
                    //
                    var $div = td.find("div");
                    //
                    var ajust = td.hasClass("dv-td-right") || td.hasClass("dv-td-left") ? 1 : 0;
                    //
                    ajust = td.hasClass("dv-empty") ? -10 : ajust;
                    //
                    if ($div) {
                        //
                        var width = ths.eq(i).width() - (i == 0 ? 0 : 10) - ajust
                        //
                        $div.width(width);
                        //如果名字太长了，硬生生的撑宽了，则需要加上breakall
                        if (td.width() > ths.eq(i).width()) {
                            //
                            needBreakTdIndex.push([i, width]);
                        }
                    }
                }
                //
                for (var i = 0; i < needBreakTdIndex.length ; i++) {
                    //
                    $(dvTr + ":visible", $tableBody).each(function () {
                        //
                        var td = $(this).find("td:eq(" + needBreakTdIndex[i][0] + ")");
                        //
                        var div = td.find("div");
                        //
                        if (td.width() > ths.eq(needBreakTdIndex[i][0]).width()) {
                            div.css({
                                "white-space": "nowrap",
                                "overflow": "hidden",
                                "text-overflow": "ellipsis",
                                "width": needBreakTdIndex[i][1] + "px"
                            }).attr("title", mText.encode(div.text()));
                        }
                    });
                }
            }
            //处理表格初始化后的事物
            this.afterhandleInitTable = function ($div, $tableBody, type) {
                //处理相同的行
                typeParams[type].showCollapse && that.handleSameDocRow($tableBody, typeParams[type].mergeCol, type);
                //
                typeParams[type].showCollapse && that.showMergedRow($div, $(collapsedownButton, $div).is(":visible"));
                //body显示
                $tableBody.show();
                //
                that.handleTableTdWidth($tableBody);
                //处理里面的行悬浮框事件
                $(dvTr, $tableBody).each(function (index) {
                    //
                    that.initFunctionLayer($div, $(this), type);
                });
                //
                typeParams[type].rows = $("tr[merged!=-1]:not(.demo-tr)", $tableBody).length;
                //总共的行数
                that.initPagerEvent($div);
                //
                that.initMainDivSize($div);
                //
                if (currentEditEntryID) {
                    //找到需要滚动到的行
                    var empty = $(dvEmpty + "[MEntryID='" + currentEditEntryID + "']", $div);
                    //
                    if (empty.length > 0 && empty.is(":visible")) {
                        //找到父行
                        var parentTr = empty.eq(0).parent();
                        //
                        $(docDiv, $div).animate({ scrollTop: parentTr.offset().top - $(docDiv, $div).offset().top }, 200, "swing", function () {
                            //
                            that.showFunctionLayer(parentTr, $div, type);
                        });
                    }
                    //滚动完成以后，置空
                    currentEditEntryID = "";
                }
            }
            //获取发票的业务单据 销售 采购 
            this.initInvoiceDocVoucher = function ($div, $tableBody, type) {
                //
                var totalHtml = "";
                //demo行
                var demoTr = $("tr." + demoTrClass, $tableBody);
                //demo行也要隐藏
                demoTr.hide();
                //先隐藏起来，把里面的合并完以后再现实
                $tableBody.hide();
                //每一行数据遍历
                for (var i = 0; i < typeParams[type].docVoucherData.length ; i++) {
                    //
                    var invoiceEntry = typeParams[type].docVoucherData[i];
                    //
                    var tr = demoTr.clone().removeClass(demoTrClass);
                    //联系人
                    $("td:eq(1) div", tr).text(mText.encode(invoiceEntry.MContactName || ""));
                    //发票号
                    $("td:eq(2) div", tr).text(mText.encode(invoiceEntry.MNumber || ""));
                    //描述
                    $("td:eq(3) div", tr).text(mText.encode(invoiceEntry.MReference || ""));
                    //日期
                    $("td:eq(4) div", tr).text(mDate.format(invoiceEntry.MBizDate) || "");
                    //物料
                    $("td:eq(5) div", tr).text(mText.encode(invoiceEntry.MItemName || ""));
                    //描述
                    $("td:eq(6) div", tr).text(mText.encode(invoiceEntry.MDesc || ""));
                    //金额
                    $("td:eq(7) div", tr).text(mMath.toMoneyFormat(invoiceEntry.MAmount || ""));
                    //税金
                    $("td:eq(8) div", tr).text(mMath.toMoneyFormat(invoiceEntry.MTaxAmt || ""));
                    //应收款科目
                    $("td:eq(10) div", tr).text(invoiceEntry.MDebitAccountName || "");
                    //税金科目
                    $("td:eq(11) div", tr).text((type == sale ? invoiceEntry.MCreditAccountName : invoiceEntry.MTaxAccountName) || "");
                    //收入科目
                    $("td:eq(12) div", tr).text((type == sale ? invoiceEntry.MTaxAccountName : invoiceEntry.MCreditAccountName) || "");
                    //凭证号
                    $("td:eq(13) div", tr).html(invoiceEntry.MVoucherNumber ? ("GL-" + invoiceEntry.MVoucherNumber) : "");
                    //
                    $tableBody.append(tr);
                    //
                    tr.show();
                    //
                    that.addAttribute2Tr(tr, invoiceEntry);
                    //
                    $tableBody.append(that.getEmptyRow(type));
                }
                //
                return $tableBody;
            };
            //费用报销
            this.initExpenseDocVoucher = function ($div, $tableBody, type) {
                //
                var totalHtml = "";
                //demo行
                var demoTr = $("tr." + demoTrClass, $tableBody);
                //demo行也要隐藏
                demoTr.hide();
                //先隐藏起来，把里面的合并完以后再现实
                $tableBody.hide();
                //
                for (var i = 0; i < typeParams[type].docVoucherData.length ; i++) {
                    //
                    var expenseEntry = typeParams[type].docVoucherData[i];
                    //
                    var tr = demoTr.clone().removeClass(demoTrClass);
                    //联系人
                    $("td:eq(1) div", tr).text(mText.encode(expenseEntry.MEmployeeName || ""));
                    //描述
                    $("td:eq(2) div", tr).text(mText.encode(expenseEntry.MReference || ""));
                    //日期
                    $("td:eq(3) div", tr).text(mDate.format(expenseEntry.MBizDate) || "");
                    //费用项目
                    $("td:eq(4) div", tr).text(mText.encode(expenseEntry.MItemName || ""));
                    //费用项目描述
                    $("td:eq(5) div", tr).text(mText.encode(expenseEntry.MDesc || ""));
                    //金额
                    $("td:eq(6) div", tr).text(mMath.toMoneyFormat((+expenseEntry.MAmount)));
                    //税额
                    $("td:eq(7) div", tr).text(mMath.toMoneyFormat(expenseEntry.MTaxAmt || ""));
                    //应收款科目
                    $("td:eq(9) div", tr).text(expenseEntry.MDebitAccountName || "");
                    //应收款科目
                    $("td:eq(10) div", tr).text(expenseEntry.MTaxAccountName || "");
                    //收入科目
                    $("td:eq(11) div", tr).text(expenseEntry.MCreditAccountName || "");
                    //凭证号
                    $("td:eq(12) div", tr).text(expenseEntry.MVoucherNumber ? ("GL-" + expenseEntry.MVoucherNumber) : "");
                    //
                    $tableBody.append(tr);
                    //
                    tr.show();
                    //
                    that.addAttribute2Tr(tr, expenseEntry);
                    //
                    $tableBody.append(that.getEmptyRow(type));
                }
                //
                return $tableBody;
            };
            //获取发票的收付款
            this.initReceivePaymentDocVoucher = function ($div, $tableBody, type) {
                //
                var totalHtml = "";
                //demo行
                var demoTr = $("tr." + demoTrClass, $tableBody);
                //demo行也要隐藏
                demoTr.hide();
                //先隐藏起来，把里面的合并完以后再现实
                $tableBody.hide();
                //每一行数据遍历
                for (var i = 0; i < typeParams[type].docVoucherData.length ; i++) {
                    //
                    var entry = typeParams[type].docVoucherData[i];
                    //
                    var tr = demoTr.clone().removeClass(demoTrClass);
                    //联系人
                    $("td:eq(1) div", tr).text(entry.MContactName || "");
                    //描述
                    $("td:eq(2) div", tr).text(mText.encode(entry.MReference || ""));
                    //日期
                    $("td:eq(3) div", tr).text(mDate.format(entry.MBizDate) || "");
                    //物料
                    $("td:eq(4) div", tr).text(mText.encode(entry.MItemName || ""));
                    //描述
                    $("td:eq(5) div", tr).text(mText.encode(entry.MDesc || ""));
                    //金额
                    $("td:eq(6) div", tr).text(mMath.toMoneyFormat(entry.MAmount || ""));
                    //税金
                    $("td:eq(7) div", tr).text(mMath.toMoneyFormat(entry.MTaxAmt || ""));
                    //应收款科目
                    $("td:eq(9) div", tr).text(entry.MDebitAccountName || "");
                    //
                    if (type == payment) {
                        //税金科目
                        $("td:eq(10) div", tr).text(entry.MTaxAccountName || "");
                        //应收款科目
                        $("td:eq(11) div", tr).text(entry.MCreditAccountName || "");
                    }
                    else {
                        //应收款科目
                        $("td:eq(10) div", tr).text(entry.MCreditAccountName || "");
                        //税金科目
                        $("td:eq(11) div", tr).text(entry.MTaxAccountName || "");
                    }
                    //凭证号
                    $("td:eq(12) div", tr).text(entry.MVoucherNumber ? ("GL-" + entry.MVoucherNumber) : "");
                    //
                    $tableBody.append(tr);
                    //
                    tr.show();
                    //
                    that.addAttribute2Tr(tr, entry);
                    //
                    $tableBody.append(that.getEmptyRow(type));
                }
                //
                return $tableBody;
            };
            //转帐单
            this.initTransferDocVoucher = function ($div, $tableBody, type) {
                //
                var totalHtml = "";
                //demo行
                var demoTr = $("tr." + demoTrClass, $tableBody);
                //demo行也要隐藏
                demoTr.hide();
                //先隐藏起来，把里面的合并完以后再现实
                $tableBody.hide();
                //每一行数据遍历
                for (var i = 0; i < typeParams[type].docVoucherData.length ; i++) {
                    //
                    var transfer = typeParams[type].docVoucherData[i];
                    //
                    var tr = demoTr.clone().removeClass(demoTrClass);
                    //
                    var tr = demoTr.clone().removeClass(demoTrClass);
                    //日期
                    $("td:eq(1) div", tr).text(mDate.format(transfer.MBizDate) || "");
                    //描述
                    $("td:eq(2) div", tr).text(mText.encode(transfer.MDesc || ""));
                    //转出账户
                    $("td:eq(3) div", tr).text(transfer.MFromAccountName || "");
                    //转入账户
                    $("td:eq(4) div", tr).text(transfer.MToAccountName || "");
                    //金额
                    $("td:eq(5) div", tr).text(mMath.toMoneyFormat(transfer.MAmount || ""));
                    //应收款科目
                    $("td:eq(7) div", tr).text(transfer.MDebitAccountName || "");
                    //应收款科目
                    $("td:eq(8) div", tr).text(transfer.MTaxAccountName || "");
                    //收入科目
                    $("td:eq(9) div", tr).text(transfer.MCreditAccountName || "");
                    //凭证号
                    $("td:eq(10) div", tr).html(transfer.MVoucherNumber ? ("GL-" + transfer.MVoucherNumber) : "");
                    //
                    $tableBody.append(tr);
                    //
                    tr.show();
                    //
                    that.addAttribute2Tr(tr, transfer);
                    //
                    $tableBody.append(that.getEmptyRow(type));
                }
                //
                return $tableBody;
            };
            //固定资产
            this.initFixAssetsDocVoucher = function ($div, $tableBody, type) {
                //
                var totalHtml = "";
                //demo行
                var demoTr = $("tr." + demoTrClass, $tableBody);
                //demo行也要隐藏
                demoTr.hide();
                //先隐藏起来，把里面的合并完以后再现实
                $tableBody.hide();

                //如果有创建了凭证，则显示凭证删除按钮
                if (home.filterDocVoucherByHasVoucher(typeParams[type].docVoucherData, true).length == 0) {

                    $(del, $div).hide();
                    $(batchSetup, $div).show();
                    $(depreciateButton, $div).show();
                }
                else {
                    $(del, $div).show();
                    $(batchSetup, $div).hide();
                    $(depreciateButton, $div).hide();
                }
                //每一行数据遍历
                for (var i = 0; i < typeParams[type].docVoucherData.length ; i++) {
                    //
                    var dep = typeParams[type].docVoucherData[i];
                    //
                    var tr = demoTr.clone().removeClass(demoTrClass);
                    //
                    var tr = demoTr.clone().removeClass(demoTrClass);
                    //编码
                    $("td:eq(1) div", tr).text(dep.MFixAssetsNumber || "");
                    //名称
                    $("td:eq(2) div", tr).text(mText.encode(dep.MFixAssetsName || ""));
                    //类别名称
                    $("td:eq(3) div", tr).text(mText.encode(dep.MFATypeIDName || ""));
                    //原值
                    $("td:eq(4) div", tr).text(mMath.toMoneyFormat(dep.MOriginalAmount));
                    //月折旧额
                    $("td:eq(5) div", tr).text(!dep.MItemID ? '' : mMath.toMoneyFormat(dep.MPeriodDepreciatedAmount + dep.MLastAdjustAmount));
                    //本年累计折旧
                    $("td:eq(6) div", tr).text(mMath.toMoneyFormat(dep.MDepreciatedAmountOfYear));
                    //减值准备
                    $("td:eq(7) div", tr).text(mMath.toMoneyFormat(dep.MPrepareForDecreaseAmount));
                    //期末净值
                    $("td:eq(8) div", tr).text(mMath.toMoneyFormat(dep.MNetAmount));

                    ////应收款科目
                    //$("td:eq(10) div", tr).text(mText.encode(dep.MExpAccountFullName || ""));
                    ////收入科目
                    //$("td:eq(11) div", tr).text(mText.encode(dep.MDepAccountFullName || ""));
                    //凭证号
                    $("td:eq(10) div", tr).html(dep.MVoucherNumber ? ("GL-" + dep.MVoucherNumber) : "");
                    //
                    $tableBody.append(tr);
                    //
                    tr.show();
                    //
                    that.addAttribute2Tr(tr, dep);
                    //
                    $tableBody.append(that.getEmptyRow(type));
                }
                //
                return $tableBody;
            };
            //出事tbody里面的相同业务单据的行,需要合并的列的数量
            this.handleSameDocRow = function (tbody, colCount, type) {
                //所有的行
                var allRows = tbody.find("tr:not(.demo-tr)");
                //当前行
                var currentEntryRow = allRows.eq(0);
                //当前行的空格行
                var currentEmptyRow = allRows.eq(1);
                //需要合并的行
                var sameEntryRowList = [currentEntryRow];
                //
                var sameEmptyRowList = [currentEmptyRow];
                //遍历,从2 开始，因为第0行为currentRow，第1行是空格行
                for (var i = 2; i < allRows.length ; i += 2) {
                    //
                    var entryRow = allRows.eq(i);
                    //
                    var emtryTd = $(dvEmpty, entryRow);
                    //
                    var emptyRow = allRows.eq(i + 1);
                    //
                    var voucherID = emtryTd.attr("MVoucherID");
                    //
                    var docVoucherID = emtryTd.attr("MDocVoucherID");
                    //
                    var entryID = emtryTd.attr("MEntryID");
                    //
                    var docID = emtryTd.attr("MDocID");
                    //
                    var voucherNumber = emtryTd.attr("MVoucherNumber");
                    //
                    var totalAmt = emtryTd.attr("MTotalAmt");
                    //
                    var taxTotalAmt = emtryTd.attr("MTaxTotalAmt");
                    //只有在相同业务单据ID才做合并，不管其是否生成了凭证
                    if (docID && docID == $(dvEmpty, currentEntryRow).attr("MDocID")) {
                        //
                        sameEntryRowList.push(entryRow);
                        //
                        sameEmptyRowList.push(emptyRow);
                    }
                    else {
                        //合并行
                        that.mergeRow(sameEntryRowList, sameEmptyRowList, colCount, type);
                        //
                        currentEntryRow = entryRow;
                        //
                        currentEmptyRow = emptyRow;
                        //
                        sameEntryRowList = [currentEntryRow];
                        //
                        sameEmptyRowList = [currentEmptyRow];
                    }
                }
                //最后还是要做合并的
                that.mergeRow(sameEntryRowList, sameEmptyRowList, colCount, type);
            };
            //合并行
            this.mergeRow = function (entryRowList, emptyRowList, colCount, type) {
                //如果只有一行，则不需要合并
                if (entryRowList && entryRowList.length > 1) {
                    //先去掉，中间的空格行
                    for (var i = 0; i < emptyRowList.length - 1 ; i++) {
                        //
                        emptyRowList[i].remove();
                    }
                    //在和并列的第一列加上一行合并列
                    if (type == expense) {
                        that.addExpenseMergedRow(entryRowList[0])
                    }
                    else if (type == sale || type == purchase) {
                        that.addInvoiceMergedRow(entryRowList[0]);
                    }
                    else if (type == receive || type == payment) {
                        that.addReceivePaymentMergedRow(entryRowList[0]);
                    }
                    //需要合并的行
                    //1.第一行的第0-colCount,td都加上 rowspan = xxx
                    //2.其他的行，0-colCount的td都去掉
                    for (var i = 0; i < entryRowList.length ; i++) {
                        //
                        entryRowList[i].attr("merged", -1);
                        //0-coCount
                        for (var j = colCount - 1; j >= 0 ; j--) {
                            //第一行
                            if (i == 0) {
                                //
                                entryRowList[i].find("td").eq(j).attr("rowspan", entryRowList.length);
                                //
                                entryRowList[i].attr("topRow", 1);
                            }
                            else {
                                //去掉此行
                                var td = entryRowList[i].find("td").eq(j);
                                //移除
                                td.remove();
                            }
                        }
                    }
                }
            };
            //展示合并或者非合并的行
            this.showMergedRow = function ($div, show, aRow, bRow) {
                //
                var $tableBody = $("tbody", $div);
                //
                aRow = aRow ? aRow : $("tr[merged=1]:not(.demo-tr)", $tableBody);
                //
                bRow = bRow ? bRow : $("tr[merged=-1]:not(.demo-tr)", $tableBody).hide();
                //
                show ? aRow.show() : aRow.hide();
                //
                !show ? bRow.show() : bRow.hide();
            };
            //给tr加上一些基本的属性 
            this.addAttribute2Tr = function ($tr, row) {
                //把所有的信息都放到中间的td-empty上面去
                var emptry = $(dvEmpty, $tr);
                //
                emptry.attr("MVoucherID", row.MVoucherID)
                    .attr("MDocVoucherID", row.MDocVoucherID)
                    .attr("MEntryID", row.MEntryID)
                    .attr("MDocID", row.MDocID)
                    .attr("MDocStatus", row.MDocStatus)
                    .attr("MVoucherNumber", row.MVoucherNumber)
                    .attr("MTotalAmt", row.MTotalAmt)
                    .attr("MTaxTotalAmt", row.MTaxTotalAmt)
                    .attr("MType", row.MType)
                    .attr("MTaxAmt", row.MTax)
                    .attr("MID", row.MID);
            };
            //
            this.initMainDivSize = function (div) {

                $(docDiv, div).height($(window).height() - $(docDiv, div).offset().top - $(_pager, div).height() - 3);

                $(tableDiv, div).width($(docDiv.div).width - 2);

                $(tableDiv, div).find("tbody").height($(docDiv, div).height() - $(tableDiv, div).find("thead").height());
            };



            //在一系列需要合并的行上面家一行合并后的行
            this.addInvoiceMergedRow = function (entryRow) {
                //
                var emtryTd = $(dvEmpty, entryRow);
                //
                var voucherID = emtryTd.attr("MVoucherID");
                //
                var docVoucherID = emtryTd.attr("MDocVoucherID");
                //
                var entryID = emtryTd.attr("MEntryID");
                //
                var docID = emtryTd.attr("MDocID");
                //
                var voucherNumber = emtryTd.attr("MVoucherNumber");
                //
                var totalAmt = emtryTd.attr("MTotalAmt");
                //
                var taxTotalAmt = emtryTd.attr("MTaxTotalAmt");
                //
                var taxAmt = emtryTd.attr("MTaxAmt");
                //复制一行
                var mergedRow = entryRow.clone();
                //加上一个属性表示是合并的行
                mergedRow.attr("merged", 1);
                //前面的都不变，只是那个item变为 Multiple...
                //item
                mergedRow.find("td:eq(5) div").text(multipleLang);
                //item
                mergedRow.find("td:eq(6) div").text(multipleLang);
                //金额，显示为合并后的金额
                mergedRow.find("td:eq(7) div").text(mMath.toMoneyFormat(totalAmt));
                //税额
                mergedRow.find("td:eq(8) div").text(mMath.toMoneyFormat(taxAmt));
                //Debit
                mergedRow.find("td:eq(10) div").text(multipleLang);
                //Tax
                mergedRow.find("td:eq(11) div").text(multipleLang);
                //Credit
                mergedRow.find("td:eq(12) div").text(multipleLang);
                //Voucher
                mergedRow.find("td:eq(13) div").text(voucherNumber ? (GL + "-" + voucherNumber) : "");
                //把这一行插入到前面
                entryRow.before(mergedRow);
            };

            //在一系列需要合并的行上面家一行合并后的行
            this.addReceivePaymentMergedRow = function (entryRow) {
                //
                var emtryTd = $(dvEmpty, entryRow);
                //
                var voucherID = emtryTd.attr("MVoucherID");
                //
                var docVoucherID = emtryTd.attr("MDocVoucherID");
                //
                var entryID = emtryTd.attr("MEntryID");
                //
                var docID = emtryTd.attr("MDocID");
                //
                var voucherNumber = emtryTd.attr("MVoucherNumber");
                //
                var totalAmt = emtryTd.attr("MTotalAmt");
                //
                var taxTotalAmt = emtryTd.attr("MTaxTotalAmt");
                //
                var taxAmt = emtryTd.attr("MTaxAmt");
                //复制一行
                var mergedRow = entryRow.clone();
                //加上一个属性表示是合并的行
                mergedRow.attr("merged", 1);
                //前面的都不变，只是那个item变为 Multiple...
                //item
                mergedRow.find("td:eq(4) div").text(multipleLang);
                //item
                mergedRow.find("td:eq(5) div").text(multipleLang);
                //金额，显示为合并后的金额
                mergedRow.find("td:eq(6) div").text(mMath.toMoneyFormat(totalAmt));
                //税额
                mergedRow.find("td:eq(7) div").text(mMath.toMoneyFormat(taxAmt));
                //Debit
                mergedRow.find("td:eq(9) div").text(multipleLang);
                //Debit
                mergedRow.find("td:eq(10) div").text(multipleLang);
                //Tax
                mergedRow.find("td:eq(11) div").text(multipleLang);
                //Voucher
                mergedRow.find("td:eq(12) div").text(voucherNumber ? (GL + "-" + voucherNumber) : "");
                //把这一行插入到前面
                entryRow.before(mergedRow);
            };
            //在一系列需要合并的行上面家一行合并后的行
            this.addExpenseMergedRow = function (entryRow) {
                //
                var emtryTd = $(dvEmpty, entryRow);
                //
                var voucherID = emtryTd.attr("MVoucherID");
                //
                var docVoucherID = emtryTd.attr("MDocVoucherID");
                //
                var entryID = emtryTd.attr("MEntryID");
                //
                var docID = emtryTd.attr("MDocID");
                //
                var voucherNumber = emtryTd.attr("MVoucherNumber");
                //
                var totalAmt = emtryTd.attr("MTotalAmt");
                //
                var taxTotalAmt = emtryTd.attr("MTaxTotalAmt");
                //
                var taxAmt = emtryTd.attr("MTaxAmt");
                //复制一行
                var mergedRow = entryRow.clone();
                //加上一个属性表示是合并的行
                mergedRow.attr("merged", 1);
                //前面的都不变，只是那个item变为 Multiple...
                //物料
                mergedRow.find("td:eq(4) div").text(multipleLang);
                //金额，显示为合并后的金额
                mergedRow.find("td:eq(5) div").text(multipleLang);
                //金额，显示为合并后的金额
                mergedRow.find("td:eq(6) div").text(mMath.toMoneyFormat(totalAmt));
                //金额，显示为合并后的金额
                mergedRow.find("td:eq(7) div").text(mMath.toMoneyFormat(taxAmt));
                //Debit
                mergedRow.find("td:eq(9) div").text(multipleLang);
                //Tax
                mergedRow.find("td:eq(10) div").text(multipleLang);
                //Tax
                mergedRow.find("td:eq(11) div").text(multipleLang);
                //Voucher
                mergedRow.find("td:eq(12) div").text(voucherNumber ? (GL + "-" + voucherNumber) : "");
                //把这一行插入到前面
                entryRow.before(mergedRow);
            };


            //获取空行
            this.getEmptyRow = function (type) {
                return "<tr class='dv-empty-row'><td class='dv-empty-td' colspan='" + typeParams[type].colspan + "'>&nbsp;</td></tr>";
            };
            //获取没有记录的行
            this.getNoRecordRow = function (type) {
                return "<tr class='" + noRecordRowClass + "'><td class='dv-norecord-td' colspan='" + typeParams[type].colspan + "'>" + HtmlLang.Write(LangModule.Common, "NoRecords", "No Records.") + "</td></tr>";
            }
            //
            this.getDivByType = function (type) {
                return $("div[doctype='" + type + "']");
            };
            //重置分页参数
            this.resetPager = function () {
                //
                for (var i = 0; i < typeParams.length ; i++) {
                    //初始化的时候，要把参数都置位原始值
                    typeParams[i].totalCount = 0;
                    //
                    typeParams[i].pageIndex = 1;
                }
            };
            //初始化翻页事件
            this.initPagerEvent = function ($div) {
                //
                var type = parseInt($div.attr("doctype"));
                //调用easyui组件
                $(_pager, $div).pagination({
                    total: typeParams[type].totalCount,
                    pageSize: GLVoucherHome.pageSize,
                    pageList: GLVoucherHome.pageList,
                    pageNumber: typeParams[type].pageIndex,
                    onSelectPage: function (number, size) {
                        typeParams[type].pageIndex = number;
                        GLVoucherHome.pageSize = size;
                        that.searchDocVoucher($div);
                    }
                });
            };
            this.refresh = function () {

                var div = $("div[docType]:visible");

                that.searchDocVoucher(div);

                GLVoucherHome.clearQueryDate();
            },
            //初始化各个事件
            this.initEvent = function ($div) {
                //生成凭证事件
                $(create, $div).off("click").on("click", function () {
                    //
                    that.createDocVoucher($div);
                });
                //批量设置
                $(batchSetup, $div).off("click").on("click", function () {
                    //
                    that.editDocVoucherRule($div, null);
                });
                //批量设置
                $(depreciateButton, $div).off("click").on("click", function () {
                    //
                    that.depreciatePeriod($div, null);
                });
                //查询事件
                $(search, $div).off("click").on("click", function () {

                    var type = parseInt($div.attr("docType"));

                    typeParams[type].pageIndex = 1;

                    that.searchDocVoucher($div);
                });
                //清除条件
                $(clear, $div).off("click").on("click", function () {

                    that.clear($div);
                    //
                    that.initDomValue($div);
                });
                //删除事件
                $(del, $div).off("click").on("click", function () {
                    that.deleteDocVoucher($div);
                });
                //重置事件
                $(reset, $div).off("click").on("click", function () {

                    that.resetDocVoucher($div);
                });
                //折叠事件
                $(collapsedownButton, $div).off("click").on("click", function () {
                    //
                    that.showMergedRow($div, false);
                    //
                    $(collapseupButton, $div).show();
                    //
                    $(this).hide();
                });
                //折叠事件
                $(collapseupButton, $div).off("click").on("click", function () {
                    //
                    that.showMergedRow($div, true);
                    //
                    $(collapsedownButton, $div).show();
                    //
                    $(this).hide();
                });
                //全部勾选
                $("thead " + docCheck + " input", $div).off("click").on("click", function () {
                    that.selectAll($(this), $div);
                });
            };
            this.clear = function ($div) {

                if ($div == undefined) {
                    $(status).attr("filter", "");
                }
                else {
                    $(status, $div).attr("filter", "");
                }
            };
            //初始化高级选项事件
            this.initFunctionLayer = function ($div, $tr, type) {
                //
                $tr.find(dvFunctionTd).off("mouseover").on("mouseover", function () {
                    //
                    that.showFunctionLayer($(this).parent(), $div, type);
                }).off("dblclick").on("dblclick", function () {
                    //如果只有一条的情况下，双击需要再次显示
                    if ($(functionLayer, $div).is(":hidden")) {
                        //
                        typeParams[type].frozenItemID = "";
                        //不进行下方的内容
                        return $(functionLayer, $div).show();
                    }
                });
                $(functionLayer, $div).off("mouseleave").on("mouseleave", function () {
                    //
                    $(functionLayer, $div).hide();
                });
                $(functionLayer, $div).off("dblclick").on("dblclick", function () {

                    //隐藏
                    $(this).hide();
                    //
                    typeParams[type].frozenItemID = type == fixassets ? $(this).attr("MID") : $(this).attr("MEntryID");
                });
            };
            //
            this.initFunctionLayerEvent = function ($div, $tr, type) {
                //删除凭证事件
                $(voucherDelete, $div).off("click").on("click", function () {
                    //
                    that.deleteDocVoucher($div, $tr, type);
                });

                //查看凭证
                $(voucherView, $div).off("click").on("click", function () {
                    //
                    currentEditEntryID = $(dvEmpty, $tr).attr("MEntryID");
                    //
                    that.editDocVoucher($div, $tr, type);
                });

                //查看业务单据
                $(docEdit, $div).off("click").on("click", function () {
                    //
                    currentEditEntryID = $(dvEmpty, $tr).attr("MEntryID");
                    //
                    that.editDoc($div, $tr, type);
                });

                //编辑凭证规则事件
                $(voucherRuleEdit, $div).off("click").on("click", function () {
                    //
                    currentEditEntryID = $(dvEmpty, $tr).attr("MEntryID");
                    //
                    that.editDocVoucherRule($div, $tr);
                });
                //生成凭证事件
                $(voucherCreate, $div).off("click").on("click", function () {
                    //
                    currentEditEntryID = $(dvEmpty, $tr).attr("MEntryID");
                    //
                    that.createDocVoucher($div, $tr);
                });
                //展开
                $(docExpand, $div).off("click").on("click", function () {
                    //
                    that.showMergedRow($div, true, that.getSubRow($tr), $tr);
                    //
                    $(functionLayer, $div).hide();
                });
                //合并
                $(docCollspan, $div).off("click").on("click", function () {
                    //
                    that.showMergedRow($div, true, that.getMergedRow($tr), that.getSubRow(that.getMergedRow($tr)));
                    //
                    $(functionLayer, $div).hide();
                });
            };
            //
            this.showFunctionLayer = function ($tr, $div, type) {
                //用来保存数据的td
                var emtryTd = $(dvEmpty, $tr);
                //业务单据分录ID
                var entryID = emtryTd.attr("MEntryID");
                //凭证ID
                var voucherID = emtryTd.attr("MVoucherID");
                //业务单据ID
                var docID = emtryTd.attr("MDocID");
                //映射ID
                var docVoucherID = emtryTd.attr("MDocVoucherID");
                //凭证号
                var voucherNumber = emtryTd.attr("MVoucherNumber");

                var mid = type == fixassets ? emtryTd.attr("MID") : null;

                //如果用户已经双击过的，则不需要再显示了
                if ((entryID || mid) == typeParams[type].frozenItemID) {
                    //如果用户已经双击过的，则不需要再显示了
                    return false;
                }
                else {
                    //清空
                    typeParams[type].frozenItemID = "";
                }

                //如果里面有没有生成的凭证
                var docVouchers = that.getDocVoucherByTr($tr, type);
                //是否已经结算了，如果结算了，不能再生成凭证
                var settled = settlementInfo.MStatus == "1";
                //生成凭证的
                var voucherCreated = home.filterDocVoucherByHasVoucher(docVouchers, true);
                //没有生成凭证的
                var voucherUncreated = home.filterDocVoucherByHasVoucher(docVouchers, false);
                //可以生成凭证的
                var canCreateVouchers = home.filterCanCreateVoucher(docVouchers, true);
                //获取已经审核的凭证
                var unapprovedVouchers = home.filterGetApprovedVoucher(voucherCreated, false);
                //是否是折叠后的行
                var isCollapseRow = $tr.attr("merged") == "1";
                //是否是明细行
                var isDetailRow = $tr.attr("merged") == "-1";
                //是否是原生行，非折叠行，也非明细行
                var isOriginRow = !isCollapseRow && !isDetailRow;
                //可查看凭证，必须在不是折叠行，并且本行有凭证号
                var canView = (voucherCreated.length == docVouchers.length);
                //都可以编辑
                var canEdit = true;
                //可以创建凭证，没有结算，并且内部有没有创建的凭证
                var canCreate = type != fixassets && !settled && (canCreateVouchers.length > 0) && voucherCreated.length != docVouchers.length;
                //是否可删除，必须在内部有凭证的情况下
                var canDelete = (voucherCreated.length > 0 && unapprovedVouchers.length > 0 && !settled);
                //显示与隐藏
                that.showButton($div, canView, canCreate, canEdit, canDelete, isCollapseRow, isDetailRow, isOriginRow);
                //左边
                var leftWidth = 0, rightWidth = 0;
                //获取需要遮罩显示的行
                var layerTr = that.getFunctionLayerTr($tr);
                //
                var $topRow = $(layerTr[0]);
                //
                var $lastRow = $(layerTr[layerTr.length - 1]);
                //
                var layerHeight = 0;
                //
                for (var i = 0; i < layerTr.length ; i++) {
                    //
                    layerHeight += $(layerTr[i]).outerHeight();
                }
                //
                for (var i = 1; i <= typeParams[type].leftPart ; i++) {
                    leftWidth += $topRow.find("td").eq(i).outerWidth();
                }
                //
                for (var i = typeParams[type].leftPart + 2 ; i < typeParams[type].leftPart + 2 + typeParams[type].rightPart; i++) {
                    rightWidth += $topRow.find("td").eq(i).outerWidth();
                }
                //
                $(functionLayer, $div).find(docPart).width(leftWidth + 1);
                //
                $(functionLayer, $div).find(voucherPart).width(rightWidth);
                //
                var $firstTr = $(tableDiv + " tbody", $div).find("tr:visible:eq(0)");
                //
                var headerOffsetTop = $(partialDemo, $div).offset().top + $(partialDemo, $div).height() + $(tableDiv + " thead", $div).height();
                //
                var diff = ($topRow.offset().top) < headerOffsetTop ? ($topRow.offset().top - headerOffsetTop) : 0;
                //
                var buttomOffset = 0;


                var height = layerHeight + 2 + diff;
                //看下方是否有遮挡
                var buttomOffset = $topRow.offset().top + height - $(_pager, $div).offset().top;
                //如果底下显示不全
                buttomOffset = buttomOffset > 0 ? buttomOffset : 0;


                //
                $(functionLayer, $div).stop();
                //
                var width = $firstTr.width() - $(docCheck, $firstTr).width() - $(dvBorder, $firstTr).width() + 5;
                //
                $(functionLayer, $div).animate({
                    //宽度
                    width: width + "px",
                    //高度
                    height: (height - buttomOffset) + "px",
                    //位置
                    left: $firstTr.find("td:eq(1)").offset().left + "px",
                    //顶部
                    top: ($topRow.offset().top - diff) + "px"
                }, 200, "swing", function () {
                    //
                    $(functionButton, $div).css({
                        "margin-top": 0 + "px",
                        "height": $(functionLayer, $div).height() + "px"
                    });
                    //
                    $(functionIco, $div).css({
                        "margin-top": (($(functionLayer, $div).height() - $(functionIco, $div).height()) / 2) + "px"
                    });
                }).show();
                //
                //其滚动的时候隐藏悬浮层
                $(docDiv, $div).find("tbody").scroll(function () {
                    //显示虚线
                    if (this.scrollTop > 0) {
                        $(this).addClass(dashedLineClassName);
                    }
                    else {
                        $(this).removeClass(dashedLineClassName);
                    }
                    //隐藏
                    $(functionLayer, $div).hide();
                });
                //
                $(functionLayer, $div).attr("MEntryID", (entryID || mid));
                //
                var $tableBody = $("tbody", typeParams[type].tableClass);
                //
                $(functionLayer, $div).mScroll($tableBody);
                //初始化里面的点击事件
                that.initFunctionLayerEvent($div, $tr, type);
            };
            //显示里面的按钮
            this.showButton = function ($div, view, create, edit, del, isCollapseRow, isDetailRow, isOriginRow) {
                //
                view ? $(voucherView, $div).show() : $(voucherView, $div).hide();
                //
                create ? $(voucherCreate, $div).show() : $(voucherCreate, $div).hide();
                //
                edit ? $(voucherRuleEdit, $div).show() : $(voucherRuleEdit, $div).hide();
                //
                del ? $(voucherDelete, $div).show() : $(voucherDelete, $div).hide();
                //如果是折叠行
                isCollapseRow ? $(docExpand, $div).show() : $(docExpand, $div).hide();
                //
                isDetailRow ? $(docCollspan, $div).show() : $(docCollspan, $div).hide();
                //
                isOriginRow ? (($(docCollspan, $div).hide() && false) || $(docExpand, $div).hide()) : "";
            };
            //查看业务单据
            this.editDoc = function ($div, $tr, type) {
                //
                var docID = $(dvEmpty, $tr).attr("MDocID");
                //
                var status = $(dvEmpty, $tr).attr("MDocStatus");
                //
                var mType = $(dvEmpty, $tr).attr("MType");
                //
                var editDocUrl = "";
                //弹窗的名称
                var editDocTitle = "";
                //
                var v = that.getEditViewFromStatus(status);
                //如果是发票
                switch (type) {
                    case sale:
                        //
                        var url = mType.indexOf("Red") > 0 ? redInvoiceEditUrl : invoiceEditUrl;
                        //编辑发票
                        editDocUrl = url + v + docID;
                        //名称
                        editDocTitle = HtmlLang.Write(LangModule.Common, "InvoiceDetail", "Invoice Detail");
                        //
                        break;
                    case purchase:
                        //
                        var url = mType.indexOf("Red") > 0 ? redPurchaseEditUrl : purchaseEditUrl;
                        //编辑发票
                        editDocUrl = url + v + docID;
                        //名称
                        editDocTitle = HtmlLang.Write(LangModule.Common, "PurchaseDetail", "Purchase Detail");
                        break;
                    case expense:
                        //编辑费用报销单
                        editDocUrl = expenseEditUrl + v + docID;
                        //名称
                        editDocTitle = HtmlLang.Write(LangModule.Common, "ExpenseDetail", "Expense Detail");
                        break;
                    case receive:
                        //编辑费用报销单
                        editDocUrl = receiveEditUrl + v + docID;
                        //名称
                        editDocTitle = HtmlLang.Write(LangModule.Common, "ReceiptDetail", "Receipt Detail");
                        break;
                    case payment:
                        //编辑付款单
                        editDocUrl = paymentEditUrl + v + docID;
                        //名称
                        editDocTitle = HtmlLang.Write(LangModule.Common, "PaymentDetail", "Payment Detail");
                        break;
                    case transfer:
                        //编辑转帐单
                        editDocUrl = transferEditUrl + docID;
                        //名称
                        editDocTitle = HtmlLang.Write(LangModule.Common, "TransferDetail", "Transfer Detail");
                        break;
                    default:
                        break;
                }
                //弹框，但是需要把框100%打开
                //弹窗
                $.mDialog.show({
                    mTitle: editDocTitle,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + editDocUrl,
                    mCloseCallback: [function (obj) {
                        //刷新
                        that.searchDocVoucher($div);
                    }]
                });
                $.mDialog.max();
            };
            //
            this.getEditViewFromStatus = function (status) {
                //
                return status >= WaitingPayment ? "View/" : "Edit/";
            }
            //将整个业务单据生成凭证
            this.createDocVoucher = function ($div, $tr) {
                //
                var type = $div.attr("doctype");
                //获取整个业务单据的信息
                var docVouchers = $tr ? that.getDocVoucherByTr($tr, type) : that.getSelectedDocVoucher($div, type);
                //如果没有选择行
                if (docVouchers.length == 0) {
                    //
                    mDialog.message(pleaseSelectRow);
                    //
                    return false;
                }
                //去除已经生成的,和不能生成凭证的
                docVouchers = home.filterCanCreateVoucher(docVouchers, true);
                //
                if (docVouchers.length > 0) {
                    //
                    mDialog.confirm(createDocVoucherConfirm, function () {

                        //调用首页的来执行
                        home.createDocVoucher(docVouchers, true, function (data) {
                            //如果生成成功
                            if (data.Success) {
                                //显示生成成功
                                mDialog.message(createSuccessLang);
                                //本身刷新
                                that.searchDocVoucher($div);
                            }
                            else {
                                mDialog.alert(data.Message);
                            }
                        });

                    })
                }
                else {
                    //
                    mDialog.alert(noDocument2Operate);
                }
            };
            //编辑业务单据生成的凭证
            this.editDocVoucher = function ($div, $tr, type) {
                //弹窗
                home.editDocVoucher(that.convertObj2Json($tr, type), function () {
                    //刷新
                    that.searchDocVoucher($div);
                });
            };
            //编辑业务单据生成凭证的规则
            this.editDocVoucherRule = function ($div, $tr) {
                //
                var type = $div.attr("doctype");
                //
                var docID = ($tr && type == transfer) ? $(dvEmpty, $tr).attr("MDocID") : "";

                docID = ($tr && type == fixassets) ? $(dvEmpty, $tr).attr("MID") : docID;

                var filter = that.getFilterSet($div);

                //需要编辑的EntryIDs
                var docIDs = [];
                //如果是某一行
                //
                var docVouchers = !$tr ? that.getSelectedDocVoucher($div, type) : that.getDocVoucherByTr($tr, type);
                //只取一个
                for (var i = 0; i < docVouchers.length ; i++) {
                    //每一个
                    docIDs.push(type == fixassets ? docVouchers[i].MID : docVouchers[i].MDocID);
                }
                if (docIDs.length > 0 || docID) {
                    //
                    home.editDocVoucherRule(docIDs, type, function () {
                        //重新刷新
                        that.searchDocVoucher($div);
                    }, filter);
                }
                else {
                    //
                    mDialog.alert(noDocument2Operate);
                }
            }
            //编辑业务单据生成凭证的规则
            this.depreciatePeriod = function ($div, $tr) {
                //
                var type = $div.attr("doctype");

                var filter = that.getFilterSet($div);

                if (filter === false) return;
                //需要编辑的EntryIDs
                //
                home.depreciatePeriodEdit(filter, function () {
                    //重新刷新
                    that.searchDocVoucher($div);
                });
            }

            //重置凭证
            this.resetDocVoucher = function ($div) {
                //
                var type = $div.attr("doctype");

                var docVouchers = that.getSelectedDocVoucher($div, type);

                docVouchers = home.filterGetApprovedVoucher(docVouchers, false);

                //必须有勾选在做删除
                if (docVouchers.length > 0) {
                    //先提醒用户是否确定删除
                    home.resetDocVoucher(docVouchers.select("MDocID"), type, function (data) {

                        that.searchDocVoucher($div);
                    });
                }
                else {
                    //提醒用户没有未审核的凭证供删除
                    mDialog.message(noUnapprovedVouchersToDelete);
                }
            };

            //删除
            this.deleteDocVoucher = function ($div, $tr) {
                //
                var type = $div.attr("doctype");
                //隐藏遮罩
                $(functionLayer, $div).hide();
                //获取勾选的凭证获取单个的凭证
                var docVouchers = $tr ? that.getDocVoucherByTr($tr, type) : that.getSelectedDocVoucher($div, type);
                //去除没有生成的
                docVouchers = home.filterDocVoucherByHasVoucher(docVouchers, true);
                //去除已经审核的
                docVouchers = home.filterGetApprovedVoucher(docVouchers, false);
                //必须有勾选在做删除
                if (docVouchers.length > 0) {
                    //先提醒用户是否确定删除
                    home.deleteDocVoucher(docVouchers, function (data) {
                        //提醒用户删除成功
                        mDialog.message(LangKey.DeleteSuccessfully);
                        //整个页面更新
                        that.searchDocVoucher($div);
                    });
                }
                else {
                    //提醒用户没有未审核的凭证供删除
                    mDialog.message(noUnapprovedVouchersToDelete);
                }
            };
            //全选与取消全选
            this.selectAll = function (input, $div) {
                //判断是否勾选
                (input.attr("checked") == "checked") ? $("tbody " + docCheck + " input", $div).attr("checked", true) : $("tbody " + docCheck + " input", $div).attr("checked", false);
            };
            //获取勾选到的docVoucher
            this.getSelectedDocVoucher = function ($div, type) {
                //
                var selectedDocVouchers = [];
                //
                $("tbody tr:visible", $div).each(function (index) {
                    //
                    var emptyTd = $(emptyTd, $(this));
                    //
                    var $topRow = that.getTopRow($(this));
                    //如果toprow勾选了，则本身也勾选了
                    if ($("td:eq(0) input", $topRow).attr("checked") == "checked") {

                        if (!($(this).attr("merged") == "-1" && $(this).attr("topRow") != "1"))
                            //
                            selectedDocVouchers = selectedDocVouchers.concat(that.getDocVoucherByTr($(this), type));
                    }
                });
                //
                return selectedDocVouchers;
            };
            //
            this.getTopRow = function ($tr) {
                return ($tr.attr("merged") == "-1" && $tr.attr("topRow") != "1") ? $tr.prevAll("[topRow=1]").first() : $tr;
            }
            //
            this.getMergedRow = function ($tr) {
                return $tr.prevAll("[merged=1]").first();
            };
            //获取遮罩层需要遮罩的tr
            this.getFunctionLayerTr = function ($tr) {
                //如果是分开的行
                if ($tr.attr("merged") == "-1") {
                    //对于分开的行
                    return that.getSubRow($tr);
                }
                //返回本身就行
                return [$tr];
            };
            //
            this.getSubRow = function ($tr) {
                //
                var docID = $(dvEmpty, $tr).attr("MDocID");
                //根据docID来查找
                var subRow = $("tr[merged='-1']", $tr.parent()).filter(function () {
                    //
                    return $(dvEmpty, $(this)).attr("MDocID") == docID;
                });
                //
                return subRow;
            };
            //根据某一行来取其应该包含的docVoucher
            this.getDocVoucherByTr = function ($tr, type) {

                //
                var docVouchers = [];
                //
                var dataTd = $(dvEmpty, $tr);

                //如果是一条总的
                docVouchers = that.findDocVoucher(type == fixassets ? dataTd.attr("MID") : dataTd.attr("MDocID"), '', type);
                //
                return docVouchers;
            };
            //讲后台需要用到的几个基本值传到后台
            this.convertObj2Json = function ($tr, type) {
                //
                var emptyTd = $(dvEmpty, $tr);
                //
                return "?MDocVoucherID=" + (emptyTd.attr("MDocVoucherID") || "") + "&MID=" + (emptyTd.attr("MID") || "") + "&MDocID=" + (emptyTd.attr("MDocID") || "") + "&MVoucherID=" + (emptyTd.attr("MVoucherID") || "") + "&MEntryID=" + (emptyTd.attr("MEntryID") || "") + "&MDocType=" + type;
            };
            //
            this.reset = function () {
                //
                $(".dv-partial-" + sale).attr("inited", 0);
                //
                $(".dv-partial-" + purchase).attr("inited", 0);
                //
                $(".dv-partial-" + expense).attr("inited", 0);
                //
                $(".dv-partial-" + receive).attr("inited", 0);
                //
                $(".dv-partial-" + payment).attr("inited", 0);
                //
                $(".dv-partial-" + transfer).attr("inited", 0);
                //
                $(".dv-partial-" + fixassets).attr("inited", 0);

                $(".dv-doc-div").each(function () {

                    $(clear, this).trigger("click");
                });
            };
            //设置参数
            this.setFilter = function (s) {
                //
                $(status).attr("filter", s);
            };
            //初始化入口
            this.init = function (index) {
                //
                index = index || 0;

                $(number).val("");
                //
                that.resetPager();
                //
                that.initTab(index);
                //
                $(tab).tabs("select", parseInt(index));
                //
                that.showTab(index);
            };
        };
        return GLDocVoucher;
    })();

    window.GLDocVoucher = GLDocVoucher;
})();