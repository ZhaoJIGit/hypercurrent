/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;

var BDBankTransaction = {
    IsNeedReconcile: true,
    //查找相关
    functoolbar: ".transaction",
    btnSearchdiv: ".btnSearchDiv",
    btnreset: '.search-reset',
    btnSearchclose: ".m-adv-search-close",
    divSearch: ".m-adv-search-transactions",
    txtAmountFrom: ".search-amount-from",
    txtAmountTo: ".search-amount-to",
    txtPayer: ".search-payer",
    txtDate: ".search-date",
    txtRef: ".search-ref",
    txtFrom: ".search-from",
    toFlag: ".search-toflag",
    exactAmount: ".search-exact",
    btnSearch: ".search-do",
    InitTransactionList: function (bankID, home, isNeedReconcile) { 
        BDBankTransaction.IsNeedReconcile = isNeedReconcile;
        BDBankTransaction.initDom();
        BDBankTransaction.initToolbar(bankID, home);
        BDBankTransaction.initTab(bankID);
        BDBankTransaction.tabSelect(bankID);

    },
    //初始化界面元素
    initDom : function () {
        $(BDBankTransaction.txtPayer, BDBankTransaction.divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Bank, "Payee", "收/付款人")).initHint();
        //支出\收入
        $(BDBankTransaction.txtFrom, BDBankTransaction.divSearch).combobox({
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
    },
    initToolbar: function (bankID, home) {
        //删除
        $("#btnDeleteTransaction").off("click").on("click", function () {


            var checkedRows = $("#gridTransaction").datagrid("getSelections");
            var url = "/BD/BDBank/DeleteTransactions";

            if (!checkedRows || checkedRows.length == 0) return;

            var confirmMessage = HtmlLang.Write(LangModule.Common, "AreYouSureToDelete", "Are you sure to delete?");

            mDialog.confirm(confirmMessage, function () {
                mAjax.post(url, { list: checkedRows }, function (data) {
                    if (data.Success) {
                        if (data.Message == 0) {
                            mDialog.alert(HtmlLang.Write(LangModule.Common,"NoDocument2Operate","没有可操作的单据!"));
                        }
                        else {
                            var message = HtmlLang.Write(LangModule.Bank, "TheQuantityOfDeletedSuccessfully", "成功删除的条数为:") + data.Message;
                            mDialog.message(message);
                            BDBankTransaction.bindTransaction(bankID);
                        }                        
                    } else {
                        mDialog.alert(data.Message);
                    }
                });
            });            
        });

        //显示搜索div
        $(BDBankTransaction.btnSearchdiv, BDBankTransaction.functoolbar).off("click").on("click", function () {
            $(BDBankTransaction.btnSearchdiv, BDBankTransaction.functoolbar).hide();
            $(BDBankTransaction.divSearch).show();
            BDBankTransaction.resize();
        });

        //隐藏搜索div
        $(BDBankTransaction.btnSearchclose, BDBankTransaction.divSearch).off("click").on("click", function () {
            $(BDBankTransaction.btnSearchdiv, BDBankTransaction.functoolbar).show();
            $(BDBankTransaction.divSearch).hide();
            BDBankTransaction.resize();
        });

        $(BDBankTransaction.exactAmount, BDBankTransaction.divSearch).off("change").on("change", BDBankTransaction.exactAmountShow);

        //添加搜索的功能
        $(BDBankTransaction.btnSearch, BDBankTransaction.divSearch).off("click").on("click", function () {
            var filter = BDBankTransaction.getFiter();
            filter.MBankID = bankID;
            var cols = BDBankTransaction.getTransactionColumns(bankID);

            Megi.grid("#gridTransaction", {
                url: "/BD/BDBank/GetBDBankTransactionList",
                //设置传入得参数
                queryParams: filter,
                //设置显示的列名
                columns: cols,
            })
        });

        $(BDBankTransaction.btnreset, BDBankTransaction.divSearch).off("click").on("click", function () {
            $(BDBankTransaction.txtDate, BDBankTransaction.divSearch).datebox('setValue', '');
            $(BDBankTransaction.txtPayer, BDBankTransaction.divSearch).val('');
            $(BDBankTransaction.txtRef, BDBankTransaction.divSearch).val('');
            $(BDBankTransaction.txtFrom, BDBankTransaction.divSearch).combobox("setValue", '');
            $(BDBankTransaction.txtAmountFrom, BDBankTransaction.divSearch).numberbox("setValue", "");
            $(BDBankTransaction.txtAmountTo, BDBankTransaction.divSearch).numberbox("setValue", "");
            $(BDBankTransaction.exactAmount, BDBankTransaction.divSearch).prop("checked", false);
            $(BDBankTransaction.exactAmount, BDBankTransaction.divSearch).trigger("change");
        });

        $("#aMarkAsRec,#divMarkAsUnRec").off("click").on("click", function () {
            var statu = $(this).attr("status");
            Megi.grid("#gridTransaction", "optSelected", {
                url: "/BD/BDBank/UpdateReconcileStatu", msg: "", param: { MBankID: bankID, MStatu: statu }, callback: function (msg) {
                    if (msg.Success) {
                        if (statu == 204) {
                            var message = HtmlLang.Write(LangModule.Bank, "MarkAsReconciledSuccess", "Mark as reconciled successful!");
                        } else {
                            var message = HtmlLang.Write(LangModule.Bank, "MarkAsUnReconciledSuccess", "Mark as unreconciled successful!");
                        }
                        $.mMsg(message);
                        BDBankTransaction.bindTransaction(bankID);
                    } else {
                        $.mAlert(msg.Message, function () {
                            BDBankTransaction.bindTransaction(bankID);
                        }, 0, true);
                    }
                }
            });
        });
        //导入付款单
        $("#aImport,#divImportSpend").off("click").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/Pay_Purchase?contactType=Supplier&accountId=' + bankID, HtmlLang.Write(LangModule.Bank, "ImportSpendMoney", "Import Spend Money"), 900, 448);
        });
        //导入收款单
        $("#divImportReceive").off("click").click(function () {
            ImportBase.showImportBox('/BD/Import/Import/Receive_Sale?contactType=Customer&accountId=' + bankID, HtmlLang.Write(LangModule.Bank, "ImportReceiveMoney", "Import Receive Money"), 900, 448);
        });
        //导出收付款单         
        $("#aExport").click(function () {
            BDBankTransaction.exportData(bankID, "");
        });
        //导出付款单         
        $("#divExportSpend").click(function () {
            BDBankTransaction.exportData(bankID, "Payment");
        });
        //导出收款单
        $("#divExportReceive").click(function () {
            BDBankTransaction.exportData(bankID, "Receive");
        });
    },

    //显示&隐藏 金额过滤项
    exactAmountShow: function () {
        if ($(BDBankTransaction.exactAmount, BDBankTransaction.divSearch).is(":checked")) {
            $(BDBankTransaction.txtAmountTo, BDBankTransaction.divSearch).hide();
            $(BDBankTransaction.toFlag, BDBankTransaction.divSearch).hide();
            $(BDBankTransaction.txtAmountFrom, BDBankTransaction.divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Common, "Amount", "Amount")).removeClass("has-hint").initHint();
        }
        else {
            $(BDBankTransaction.txtAmountTo, BDBankTransaction.divSearch).show();
            $(BDBankTransaction.toFlag, BDBankTransaction.divSearch).show();
            $(BDBankTransaction.txtAmountFrom, BDBankTransaction.divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Common, "MinAmount", "最小金额")).removeClass("has-hint").initHint();
        }
    },
    getFiter: function () {
        var dates = new BDBankReconcileHome().getUserSelectedDate();
        var filter = {
            ExactDate: $(BDBankTransaction.txtDate, BDBankTransaction.divSearch).datebox("getValue"),
            TransAcctName: $(BDBankTransaction.txtPayer, BDBankTransaction.divSearch).val(),
            MDesc: $(BDBankTransaction.txtRef, BDBankTransaction.divSearch).val(),
            SrcFrom: $(BDBankTransaction.txtFrom, BDBankTransaction.divSearch).combobox("getValue"),
            AmountFrom: $(BDBankTransaction.txtAmountFrom, BDBankTransaction.divSearch).val(),
            AmountTo: $(BDBankTransaction.txtAmountTo, BDBankTransaction.divSearch).val(),
            IsExactAmount: $(BDBankTransaction.exactAmount, BDBankTransaction.divSearch).is(":checked"),
            StartDate: dates[0],
            EndDate: dates[1]
        };
        return filter;
    },
    getFormatDate: function (strDate) {
        var date = new Date(strDate);
        return date.getFullYear() + '/' + (date.getMonth() + 1) + '/' + date.getDate();
    },
    exportData: function (bankID, type) {
        var filter = BDBankTransaction.getFiter();
        filter.MBankID = bankID;
        filter.TransactionType = type;
        filter.Sort = "MBizDate";
        filter.Order = "desc";
        if (!filter.SrcFrom) {
            filter.SrcFrom = "0";
        }
        var queryParam = filter;
        location.href = '/BD/BDBank/Export?jsonParam=' + escape($.toJSON(queryParam));
        $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
    },
    initTab: function (bankID) {
        var type = Megi.request("type");
        if (type != "0" && type != "1") {
            type = 2;
        }
        type = Number(type);
        $('#tabTransaction').tabs('select', type);
        BDBankTransaction.bindTransaction(bankID);
    },
    tabSelect: function (bankID) {
        $('#tabTransaction').tabs({
            onSelect: function (title, index) {
                switch (index) {
                    case 0:
                        break;
                    case 1:
                        BDBankTransaction.StatementsTransaction(bankID);
                        break;
                    case 2:
                        BDBankTransaction.bindTransaction(bankID);
                        break;
                    default:
                        break;
                }
            }
        });
    },
    viewFileInfo: function (fileId, fileIds) {
        Megi.openDialog('/BD/Docs/FileView', '', 'curFileId=' + fileId + '&fileIds=' + fileIds, 560, 460);
    },
	getTransactionColumns: function (bankID) {
		//获取启用时间
		var beginTime = $.mDate.parse($("#hideGLBeginDate").val()).getTime();
        var arr = [
                //复选框
                {
                    title: '<input type=\"checkbox\" >', field: 'MID', width: 10, align: 'center', formatter: function (value, rec, rowIndex) {
						//如果为完全勾兑状态或者启用时间大于单据日期(期初单据)则禁用按钮
						var html = rec.MReconcileStatu == "203" || beginTime > $.mDate.parse(rec.MBizDate).getTime() ? "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MID + "\" disabled='disabled'>" :
                            "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MID + "\" >";
                        return html;
                    }
                },
                //业务日期
                {
                    title: BDBankCashCoding.getSortColumnTitle(LangKey.Date), field: 'MBizDate', align: 'left', width: 30, sortable: true, formatter: function (value, rec, rowIndex) {

                        //return "<a href=\"javascript:void(0);\" onclick='BDBankTransaction.viewRow(\"" + bankID + "\", \"" + rec.MID + "\", \"" + rec.MType + "\", \"" + rec.MReference + "\");' >" + mDate.format(rec.MBizDate) + "</a>"
                        return "<a href=\"javascript:void(0);\" onclick='BDBankTransaction.viewRow(\"" + bankID + "\", \"" + rec.MID + "\", \"" + rec.MType + "\");' >" + mDate.format(rec.MBizDate) + "</a>";
                    }
                },
                //备注
                {
                    title: LangKey.Description, field: 'MDescription', width: 80, formatter: function (value, rec, rowIndex) {
                        if (value == null || value.length == 0) {
                            return "";
                        }
                        value = value.replace("Receive:", HtmlLang.Write(LangModule.Common, "Receive_Sale", "Receive") + " : ");
                        value = value.replace("Payment:", HtmlLang.Write(LangModule.Common, "Pay_Purchase", "Payment") + " : ");
                        value = value.replace("Interest:", HtmlLang.Write(LangModule.Bank, "Interest", "Interest") + " : ");
                        value = value.replace("Bank fee:", HtmlLang.Write(LangModule.Bank, "BankFees", "Bank fee") + " : ");
                        value = value.replace("Reconciliation adjustment:", HtmlLang.Write(LangModule.Bank, "ReconciliationAdjustment", "Reconciliation adjustment") + " : ");
                        return value;
                    }
                },
                //摘要
                { title: LangKey.Reference, field: 'MReference', width: 60 },
                //付款金额
                {
                    title: LangKey.Spent, field: 'MSpent', align: 'right', width: 40, formatter: function (value, rec, rowIndex) {
                        //return Megi.Math.toDecimal(rec.MSpent, 2);
                        return mMath.toMoneyFormat(rec.MSpent)
                    }
                },
                //收款金额
                {
                    //当付款金额等于空的话直接返回 否则后面添加上[币别]的标示
                    title: LangKey.Received, field: 'MReceived', align: 'right', width: 40, formatter: function (value, rec, rowIndex) {
                        //return Megi.Math.toDecimal(rec.MReceived, 2);
                        return mMath.toMoneyFormat(rec.MReceived)
                    }
                },
                //附件
                {
                    title: HtmlLang.Write(LangModule.Common, "Attachment", "Attachment"), field: 'MAttachIDs', align: 'center', width: 30, formatter: function (value, rec, rowIndex) {
                        var hasAttach = rec.MAttachIDs != null && rec.MAttachIDs != '';
                        var curAttachId = hasAttach ? rec.MAttachIDs.split(',')[0] : '';
                        var attachCount = hasAttach ? rec.MAttachIDs.split(',').length : '';
                        var attachIconClass = hasAttach ? "m-list-attachment" : "";
                        return "<a href='javascript:void(0);' onclick=\"BDBankTransaction.viewFileInfo('" + curAttachId + "', '" + rec.MAttachIDs + "', '');\" class='" + attachIconClass + "'><span>" + attachCount + "</span></a>";
                    }
                }
        ];
        if (BDBankTransaction.IsNeedReconcile) {
            arr.push({
                title: BDBankCashCoding.getSortColumnTitle(HtmlLang.Write(LangModule.Bank, "Status", "Status")), field: 'MReconcileStatu', width: 30, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                    if (value == 203) {
                        return "<span class='m-green'>" + HtmlLang.Write(LangModule.Bank, "Reconciled", "Reconclied") + "</span>"
                    } else if (value == 204) {
                        return "<span class='m-black'>" + HtmlLang.Write(LangModule.Bank, "Reconciled", "Reconclied") + "</span>"
                    } else {
                        return "<span class='m-yellow'>" + HtmlLang.Write(LangModule.Bank, "unreconciled", "UnReconclied") + "</span>"
                    }
                }
            });
        }
        var cols = [];
        cols.push(arr);
        return cols;
    },
    resize: function () {
        var height = $("body").height() - $("#gridTransactionDiv").offset().top - 10;
        Megi.grid("#gridTransaction", "resize", {
            height: height
        });
    },
    bindTransaction: function (bankID) {
        //获取选择的日期
        var filter = BDBankTransaction.getFiter();
        filter.MBankID = bankID;
        var cols = BDBankTransaction.getTransactionColumns(bankID);
        Megi.grid("#gridTransaction", {
            resizable: true,
            auto: true,
            checkOnSelect: false,
            pagination: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $("#gridTransactionDiv").width() - 5,
            height: ($("body").height() - $("#gridTransactionDiv").offset().top - 10),
            sortName: 'MBizDate',
            sortOrder: 'desc',
            url: "/BD/BDBank/GetBDBankTransactionList",
            queryParams: filter,
            columns: cols,
            onLoadSuccess: function () {
                $(window).resize();
                BDBankCashCoding.initTitleTooltip();
                new BDBankReconcileHome().getForm();
            }
        });
    },
    //viewRow: function (bankID, MID, MType, MReference) {
    viewRow: function (bankID, MID, MType) {
        //var ref = MReference == null ? null : $.trim(MReference) == "" ? null : MReference;
        switch (MType) {
            case "Pay_Purchase":
            case "Pay_Adjustment":
            case "Pay_BankFee":
            case "Pay_Other":
            case "Pay_BankInterest":
                var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewPayment", "View Payment");
                $.mTab.addOrUpdate(tabTitle, '/IV/Payment/PaymentView/' + MID + '?acctId=' + bankID + "&sv=1", true);
                break;
            case "Pay_PurReturn":
            case "Pay_OtherReturn":
                var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewRefund", "View Refund");
                $.mTab.addOrUpdate(tabTitle, '/IV/Payment/PaymentView/' + MID + '?acctId=' + bankID + "&sv=1", true);
                break;
            case "Receive_Sale":
            case "Receive_Adjustment":
            case "Receive_BankFee":
            case "Receive_Other":
            case "Receive_BankInterest":
                var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewReceipt", "View Receive");
                $.mTab.addOrUpdate(tabTitle, '/IV/Receipt/ReceiptView/' + MID + '?acctId=' + bankID + "&sv=1", true);
                break;
            case "Receive_SaleReturn":
            case "Receive_OtherReturn":
                var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewRefund", "View Refund");
                $.mTab.addOrUpdate(tabTitle, '/IV/Receipt/ReceiptView/' + MID + '?acctId=' + bankID + "&sv=1", true);
                break;
            case "Transfer":
                var tabTitle = HtmlLang.Write(LangModule.Bank, "ViewTransfer", "View Transfer");
                $.mTab.addOrUpdate(tabTitle, '/IV/IVTransfer/IVTransferHome?MID=' + MID + "&bankId=" + bankID, true);
                break;
        }
    },
    StatementsTransaction: function (bankID) {
        var acctId = bankID;
        Megi.grid("#gridBankStatements", {
            resizable: true,
            auto: true,
            url: "/IV/Transactions/GetBankStatementsList",
            queryParams: { MBankID: acctId },
            columns: [[
                {
                    title: HtmlLang.Write(LangModule.Bank, "ImportDate", "Import Date"), field: 'MImportDate', width: 50, formatter: function (value, row, index) {
                        return "<a href=\"/BD/BDBank/BDBankStatementView/" + row.MID + "?acctId=" + acctId + "\" >" + value + "</a>"
                    }
                },
                { title: HtmlLang.Write(LangModule.Bank, "StartDate", "Start Date"), field: 'MStartDate', width: 50 },
                { title: HtmlLang.Write(LangModule.Bank, "EndDate", "End Date"), field: 'MEndDate', width: 50 },
                { title: HtmlLang.Write(LangModule.Bank, "StartBalance", "Start Balance"), field: 'MStartBalance', width: 50 },
                { title: HtmlLang.Write(LangModule.Bank, "EndBalance", "End Balance"), field: 'MEndBalance', width: 50 },
                { title: HtmlLang.Write(LangModule.Bank, "Status", "Status"), field: 'MStatus', width: 50 },
                { title: HtmlLang.Write(LangModule.Bank, "User", "User"), field: 'MUser', width: 50 },
                { title: HtmlLang.Write(LangModule.Bank, "ImportedFile", "Imported File"), field: 'MFileName', width: 50 },
            ]],
            onClickRow: function (rowIndex, rowData) {
                mWindow.reload("/BD/BDBank/BDBankStatementView/" + rowData.MID + "?acctId=" + acctId);
            },
        });
    }
}