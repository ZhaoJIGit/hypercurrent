(function () {
    var GLDocVoucherRuleEdit = (function () {
        //
        var GLDocVoucherRuleEdit = function () {
            //
            var that = this;
            //操作失败
            var operationFail = HtmlLang.Write(LangModule.Common, "operationfail", "Operation Failded!");
            //创建成功
            var createSuccessLang = HtmlLang.Write(LangModule.Common, "CreateSuccessfully", "Create Successfully!");
            //
            var docType = 0;
            //定义各个tab对应的index
            var sale = 0, purchase = 1, expense = 2, receive = 3, payment = 4, transfer = 5, salary = 6;
            //
            var bankAccountType = 1, cashAccountType = 3;

            var periodClosed = false;
            //各种需要隐藏的列
            var colsHide = [
                [10],
                [12],
                [1, 12],
                [1, 10],
                [1, 12],
                [0, 1, 7, 12],
                []
            ];
            //
            var canCreate = canSave = false;
            //
            var home = new GLVoucherHome();
            //
            var GL = "GL";
            //以下是一些列的科目类型
            //1.往来科目
            var currentAccount = HtmlLang.Write(LangModule.GL, "currentAccount", "Current Account");
            //2.收入科目
            var incomeAccount = HtmlLang.Write(LangModule.GL, "incomeAccount", "Income Account");
            //3.成本科目
            var costAccount = HtmlLang.Write(LangModule.GL, "costAccount", "Cost Account");
            //4.进项税科目 
            var inputVATAccount = HtmlLang.Write(LangModule.GL, "inputVATAccount", "Input VAT Account");
            //5.销项税科目
            var outputVATAccount = HtmlLang.Write(LangModule.GL, "outputVATAccount", "Output VAT Account");
            //6.费用科目
            var expenseAccount = HtmlLang.Write(LangModule.GL, "expenseAccount", "Expense Account");
            //7.资金科目
            var cashAccount = HtmlLang.Write(LangModule.GL, "cashAccount", "Cash Account");
            //8.固定资产
            var fixedAssetsAccout = HtmlLang.Write(LangModule.GL, "fixedAssetsAccout", "Salary Payable Account");
            //9.应付薪酬科目
            var salaryPayableAccount = HtmlLang.Write(LangModule.GL, "salaryPayableAccount", "Salary Payable Account");
            //10.应交税金科目
            var taxPayableAccount = HtmlLang.Write(LangModule.GL, "taxPayableAccount", "Tax Payable Account");
            //11.借方科目
            var debitAccount = HtmlLang.Write(LangModule.GL, "DebitAccount", "Debit Account");
            //12.贷方科目
            var creditAccount = HtmlLang.Write(LangModule.GL, "CreditAccount", "贷方科目");
            //13.税科目
            var taxAccount = HtmlLang.Write(LangModule.GL, "TaxAccount", "税科目");
            //14.汇兑损益
            var exchangeLoss = HtmlLang.Write(LangModule.GL, "ExchangeLoss", "汇兑损益");
            //15.设置核算维度
            var setVoucherDetail = HtmlLang.Write(LangModule.GL, "SetVoucherDetail", "设置凭证详情");
            //
            var merItem = HtmlLang.Write(LangModule.Common, "MerItem", "商品项目");
            //
            var merExpItem = HtmlLang.Write(LangModule.Common, "MerExpItem", "商品/费用项目");
            //
            var expItem = HtmlLang.Write(LangModule.Common, "ExpItem", "费用项目");
            //
            var approvedLang = HtmlLang.Write(LangModule.GL, "Audited", "Approved");
            //
            var unapprovedLang = HtmlLang.Write(LangModule.GL, "Unapproved", "Unapproved");
            //
            var customer = HtmlLang.Write(LangModule.GL, "customer", "Customer");
            //
            var supplier = HtmlLang.Write(LangModule.GL, "supplier", "Supplier");
            //员工
            var employee = HtmlLang.Write(LangModule.GL, "Employee", "Employee");
            //
            var supplierExployee = HtmlLang.Write(LangModule.GL, "supplierEmployee", "供应商/员工");
            //
            var editVoucherTitle = HtmlLang.Write(LangModule.GL, "EditVoucherFromDoc", "编辑业务单据转凭证");
            //
            var from = HtmlLang.Write(LangModule.IV, "FromAccount", "转出账户");
            //
            var to = HtmlLang.Write(LangModule.IV, "ToAccount", "转入账户");

            var payer = HtmlLang.Write(LangModule.IV, "Payer", "收款方");

            //
            var payee = HtmlLang.Write(LangModule.IV, "Payee", "付款方");
            //
            var taxInfoClass = "de-tax-info";
            //
            var taxAmountInfo = ".de-taxamount-info";
            //表格主体
            var tableBody = ".de-table-body";
            //表头行
            var headerRow = ".de-head-tr";
            //表体行
            var bodyRow = ".de-body-tr";
            //需要进行科目选择的TD
            var accountTd = ".de-account-td";
            //debit info
            var debitInfo = ".de-debit-info";
            //
            var creditInfo = ".de-credit-info"
            //
            var taxInfo = ".de-tax-info";
            //
            var editVoucher = ".dv-edit-voucher";
            //
            var accountInputClass = "de-account-input";
            //
            var saveButton = ".de-save-button";
            //
            var createButton = ".de-create-button";
            //
            var voucherInfo = ".de-voucher-info";
            //
            var docIDs, docVoucherID;
            //
            var batchSetupClass = "de-batch-setup";
            //
            var docVoucherData = [];
            //
            var accountData = [];
            //异步获取数据
            this.initDocVoucher2Table = function (data) {
                //
                accountData = data || accountData;
                //
                home.getDocVoucher({
                    MDocVoucherID: docVoucherID,
                    Type: docType,
                    MDocIDs: docIDs
                }, function (data) {
                    //
                    docVoucherData = data.rows;

                    periodClosed = data.rows[0].MSettleStatus == 1;
                    //绑定事件
                    that.initEvent();
                    //初始化表格
                    that.initTableData();
                    //初始化表格里面的事件
                    that.initTableEvent();
                });
            };
            //计算表格各个列的宽度
            this.resizeTable = function () {
                //获取当前表格的宽度
                var tableWidth = $(tableBody).outerWidth();
                //总体的列需要的宽度
                var thWidth = 0;
                //表头的每一列
                $("th:visible", headerRow).each(function () {
                    //
                    thWidth += parseInt($(this).attr("mwidth"));
                });
                //然后计算宽度
                $("th:visible", headerRow).each(function () {
                    //逐列计算宽度
                    $(this).width(parseInt($(this).attr("mwidth")) * tableWidth / thWidth);
                });
            };
            //初始化表格的数据
            this.initTableData = function () {
                //
                $("tr:not(.demo)", tableBody).remove();
                //先隐藏不需要的列
                that.hideUnusedColumn();
                //针对不同的类型，隐藏不同的列
                switch (parseInt(docType)) {
                    case sale:
                    case purchase:
                        //发票
                        that.initInvoiceTable();
                        break;
                    case expense:
                        //
                        that.initExpenseTable();
                        break;
                    case receive:
                    case payment:
                        //
                        that.initReceivePaymentTable();
                        break;
                    case transfer:
                        //
                        that.initTransferTable();
                        break;
                    default:
                        break;
                }
                //把表格重新算宽度
                that.resizeTable();
                //加上可以拖拽调整宽度的函数
                $(tableBody).mTableResize();
            };
            //移除不需要的列
            this.hideUnusedColumn = function () {
                //
                for (var i = 0; i < colsHide[docType].length ; i++) {
                    //
                    $(tableBody).find("td").eq(colsHide[docType][i]).attr("nouse", 1).hide();
                    //
                    $(headerRow).find("th").eq(colsHide[docType][i]).attr("nouse", 1).hide();
                }
            };
            //初始化发票类型
            this.initInvoiceTable = function () {
                //获取到表头，需要对其重命名
                var header = $(headerRow, tableBody).eq(0);
                //第11列起名销项税
                if (docType == sale) {
                    header.find("th:eq(0)").text(customer);
                    header.find("th:eq(9)").text(debitAccount);
                    header.find("th:eq(11)").text(creditAccount);
                    header.find("th:eq(12)").text(taxAccount);
                }
                else {
                    header.find("th:eq(0)").text(supplier);
                    header.find("th:eq(9)").text(debitAccount);
                    header.find("th:eq(10)").text(taxAccount);
                    header.find("th:eq(11)").text(creditAccount);
                }
                //表体每一行的模版
                var demoTr = $(bodyRow, tableBody);
                //
                var tbody = $("tbody", tableBody);
                //先隐藏
                tbody.hide();
                //
                var rowspan = 4;
                //
                var lastDocID = "";
                //
                for (var i = 0; i < docVoucherData.length ; i++) {
                    //
                    var entry = docVoucherData[i];
                    //复制行
                    var currentTr = demoTr.clone();
                    //
                    currentTr.attr("MEntryID", entry.MEntryID);
                    //
                    currentTr.attr("MTaxEntryID", entry.MTaxEntryID);
                    //
                    currentTr.attr("approved", entry.MVoucherStatus);
                    //
                    currentTr.removeClass("demo");
                    //如果是第一行,且不止多行，则需要将前面的合并
                    if (lastDocID != entry.MDocID) {
                        //
                        var sameDocVouchers = that.getDocVoucherByDocID(entry.MDocID);
                        //客户名称
                        var contactName = mText.htmlDecode(entry.MContactName || "");
                        currentTr.find("td[nouse!=1]:eq(0)").attr("rowspan", sameDocVouchers.length).text(contactName);
                        //发票号
                        currentTr.find("td[nouse!=1]:eq(1)").attr("rowspan", sameDocVouchers.length).text(entry.MNumber || "");
                        //业务日期
                        currentTr.find("td[nouse!=1]:eq(2)").attr("rowspan", sameDocVouchers.length).text(mDate.format(entry.MBizDate));
                        //Reference
                        currentTr.find("td[nouse!=1]:eq(3)").attr("rowspan", sameDocVouchers.length).text(entry.MReference || "");
                        //
                        rowspan = 0;
                    }
                    else {
                        //移除不需要的td
                        currentTr.find("td[nouse!=1]:lt(" + rowspan + ")").remove();
                    }
                    //第4列item
                    currentTr.find("td[nouse!=1]").eq(4 - rowspan).text(entry.MItemName || "");
                    //第5列摘要
                    currentTr.find("td[nouse!=1]").eq(5 - rowspan).text(entry.MDesc || "");
                    //第6列金额
                    currentTr.find("td[nouse!=1]").eq(6 - rowspan).text(mMath.toMoneyFormat(entry.MAmount));
                    //第7列税金额
                    currentTr.find("td[nouse!=1]").eq(7 - rowspan).text(mMath.toMoneyFormat(entry.MTaxAmt || ""));

                    //第9列
                    //
                    var creditTd = currentTr.find("td[nouse!=1]").eq(9 - rowspan);
                    //初始化combobox
                    creditTd.attr("accountID", entry.MDebitAccountID).text(entry.MDebitAccountFullName);
                    //第10列
                    var taxOrCreditTd = currentTr.find("td[nouse!=1]").eq(10 - rowspan);
                    //
                    taxOrCreditTd.attr("accountID", docType == sale ? entry.MCreditAccountID : entry.MTaxAccountID).text((docType == sale ? entry.MCreditAccountFullName : entry.MTaxAccountFullName) || "");
                    //第11列
                    var creditOrTaxTd = currentTr.find("td[nouse!=1]").eq(11 - rowspan);
                    //
                    creditOrTaxTd.attr("accountID", docType == purchase ? entry.MCreditAccountID : entry.MTaxAccountID).text((docType == purchase ? entry.MCreditAccountFullName : entry.MTaxAccountFullName) || "");
                    //第12列
                    currentTr.find("td[nouse!=1]").eq(12 - rowspan).html(that.getVoucherNumberCell(entry));
                    //把列插入到tablebody里面
                    tbody.append(currentTr);
                    //
                    rowspan = 4;
                    //
                    lastDocID = entry.MDocID;
                }
                //加上批量设置行
                canSave && docVoucherData.length > 1 && that.addBatchSetupTr(13, 3);
                //显示
                tbody.show();
                //表格显示
                $(tableBody).show();
                //初始化combobox
                that.initAccountCombo();
            };
            //
            this.getVoucherNumberCell = function (entry) {
                //
                if (entry.MVoucherNumber) {
                    //
                    return "<a href='###' class='dv-edit-voucher' entryID='" + entry.MEntryID + "'>" + (GL + "-" + entry.MVoucherNumber + "[" + (entry.MVoucherStatus == 1 ? approvedLang : unapprovedLang) + "]") + "</a>";
                }
                else {
                    //
                    return "<a href='###' class='dv-edit-voucher' entryID='" + entry.MEntryID + "'>" + setVoucherDetail + "</a>";
                }
            }
            //初始化表格里面的事件
            this.initTableEvent = function () {
                //
                $(editVoucher).off("click").on("click", function () {
                    //新建一个凭证
                    var editDocVoucherUrl = "/GL/GLVoucher/GLDocVoucherEdit";
                    //
                    var entryID = $(this).attr("entryid");
                    //
                    var entry = that.getTableData(entryID);
                    //弹出凭证编辑界面
                    $.mDialog.show({
                        mTitle: editVoucherTitle,
                        mDrag: "mBoxTitle",
                        mShowbg: true,
                        mContent: "iframe:" + editDocVoucherUrl
                            + "?MVoucherID=" + (entry.MVoucherID || "")
                            + "&MDocVoucherID=" + (entry.MDocVoucherID || "")
                            + "&MDebitEntryID=" + (entry.MDebitEntryID || "")
                            + "&MTaxEntryID=" + (entry.MTaxEntryID || "")
                            + "&MCreditEntryID=" + (entry.MCreditEntryID || "")
                            + "&MDebitAccountID=" + (entry.MDebitAccountID || "")
                            + "&MTaxAccountID=" + (entry.MTaxAccountID || "")
                            + "&MCreditAccountID=" + (entry.MCreditAccountID || ""),
                        mCloseCallback: [function () {
                            //刷新页面
                            that.initDocVoucher2Table();
                        }]
                    });
                });
            }
            //初始化费用报销
            this.initExpenseTable = function () {
                //获取到表头，需要对其重命名
                var header = $(headerRow, tableBody).eq(0);
                //第11列起名销项税
                header.find("th:eq(0)").text(employee);
                header.find("th:eq(4)").text(expItem);
                header.find("th:eq(9)").text(debitAccount);
                header.find("th:eq(10)").text(taxAccount);
                header.find("th:eq(11)").text(creditAccount);
                //表体每一行的模版
                var demoTr = $(bodyRow, tableBody);
                //
                var tbody = $("tbody", tableBody);
                //先隐藏
                tbody.hide();
                //
                var rowspan = 3;
                //
                var lastDocID = "";
                //
                for (var i = 0; i < docVoucherData.length ; i++) {
                    //
                    var entry = docVoucherData[i];
                    //复制行
                    var currentTr = demoTr.clone();
                    //
                    currentTr.attr("MEntryID", entry.MEntryID);
                    //
                    currentTr.attr("MTaxEntryID", entry.MTaxEntryID);
                    //
                    currentTr.attr("approved", entry.MVoucherStatus);
                    //
                    currentTr.removeClass("demo");
                    //如果是第一行,且不止多行，则需要将前面的合并
                    if (lastDocID != entry.MDocID) {
                        //
                        var sameDocVouchers = that.getDocVoucherByDocID(entry.MDocID);
                        //客户名称
                        currentTr.find("td[nouse!=1]:eq(0)").attr("rowspan", sameDocVouchers.length).text(entry.MEmployeeName || "");
                        //业务日期
                        currentTr.find("td[nouse!=1]:eq(1)").attr("rowspan", sameDocVouchers.length).text(mDate.format(entry.MBizDate));
                        //Reference
                        currentTr.find("td[nouse!=1]:eq(2)").attr("rowspan", sameDocVouchers.length).text(entry.MReference || "");
                        //
                        rowspan = 0;
                    }
                    else {
                        //移除不需要的td
                        currentTr.find("td[nouse!=1]:lt(" + rowspan + ")").remove();
                    }
                    //第4列item
                    currentTr.find("td[nouse!=1]").eq(3 - rowspan).text(entry.MItemName || "");
                    //第5列摘要
                    currentTr.find("td[nouse!=1]").eq(4 - rowspan).text(entry.MDesc || "");
                    //第6列金额
                    currentTr.find("td[nouse!=1]").eq(5 - rowspan).text(mMath.toMoneyFormat(entry.MAmount));
                    //第7列税金额
                    currentTr.find("td[nouse!=1]").eq(6 - rowspan).text(mMath.toMoneyFormat(entry.MTaxAmt || ""));

                    //第9列
                    //
                    var debitTd = currentTr.find("td[nouse!=1]").eq(8 - rowspan);
                    //初始化combobox
                    debitTd.attr("accountID", entry.MDebitAccountID).text(entry.MDebitAccountFullName);
                    //第10列
                    var taxTd = currentTr.find("td[nouse!=1]").eq(9 - rowspan);
                    //
                    taxTd.attr("accountID", entry.MTaxAccountID).text(entry.MTaxAccountFullName || "");
                    //第11列
                    var creditTd = currentTr.find("td[nouse!=1]").eq(10 - rowspan);
                    //
                    creditTd.attr("accountID", entry.MCreditAccountID).text(entry.MCreditAccountFullName);

                    //第12列
                    currentTr.find("td[nouse!=1]").eq(11 - rowspan).html(that.getVoucherNumberCell(entry));
                    //把列插入到tablebody里面
                    tbody.append(currentTr);
                    //
                    rowspan = 3;
                    //
                    lastDocID = entry.MDocID;
                }
                //加上批量设置行
                canSave && docVoucherData.length > 1 && that.addBatchSetupTr(12, 3);
                //显示
                tbody.show();
                //表格显示
                $(tableBody).show();
                //初始化combobox
                that.initAccountCombo();
            };
            //初始化发票类型
            this.initReceivePaymentTable = function () {
                //获取到表头，需要对其重命名
                var header = $(headerRow, tableBody).eq(0);
                //第11列起名销项税
                if (docType == receive) {
                    header.find("th:eq(0)").text(payee);
                    header.find("th:eq(9)").text(debitAccount);
                    header.find("th:eq(11)").text(creditAccount);
                    header.find("th:eq(12)").text(taxAccount);
                }
                else {
                    header.find("th:eq(0)").text(payer);
                    header.find("th:eq(4)").text(merExpItem);
                    header.find("th:eq(9)").text(debitAccount);
                    header.find("th:eq(10)").text(taxAccount)
                    header.find("th:eq(11)").text(creditAccount)
                }
                //表体每一行的模版
                var demoTr = $(bodyRow, tableBody);
                //
                var tbody = $("tbody", tableBody);
                //先隐藏
                tbody.hide();
                //
                var rowspan = 3;
                //
                var lastDocID = "";
                //
                for (var i = 0; i < docVoucherData.length ; i++) {
                    //
                    var entry = docVoucherData[i];
                    //复制行
                    var currentTr = demoTr.clone();
                    //
                    currentTr.attr("MEntryID", entry.MEntryID);
                    //
                    currentTr.attr("MTaxEntryID", entry.MTaxEntryID);
                    //
                    currentTr.attr("approved", entry.MVoucherStatus);
                    //
                    currentTr.removeClass("demo");
                    //如果是第一行,且不止多行，则需要将前面的合并
                    if (lastDocID != entry.MDocID) {
                        //
                        var sameDocVouchers = that.getDocVoucherByDocID(entry.MDocID);
                        //客户名称
                        currentTr.find("td[nouse!=1]:eq(0)").attr("rowspan", sameDocVouchers.length).text(entry.MContactName || "");
                        //业务日期
                        currentTr.find("td[nouse!=1]:eq(1)").attr("rowspan", sameDocVouchers.length).text(mDate.format(entry.MBizDate));
                        //Reference
                        currentTr.find("td[nouse!=1]:eq(2)").attr("rowspan", sameDocVouchers.length).text(entry.MReference || "");
                        //
                        rowspan = 0;
                    }
                    else {
                        //移除不需要的td
                        currentTr.find("td[nouse!=1]:lt(" + rowspan + ")").remove();
                    }
                    //第4列item
                    currentTr.find("td[nouse!=1]").eq(3 - rowspan).text(entry.MItemName || "");
                    //第5列摘要
                    currentTr.find("td[nouse!=1]").eq(4 - rowspan).text(entry.MDesc || "");
                    //第6列金额
                    currentTr.find("td[nouse!=1]").eq(5 - rowspan).text(mMath.toMoneyFormat(entry.MAmount));
                    //第7列税额
                    currentTr.find("td[nouse!=1]").eq(6 - rowspan).text(mMath.toMoneyFormat(entry.MTaxAmt || ""));
                    //第9列
                    var creditTd = currentTr.find("td[nouse!=1]").eq(8 - rowspan);
                    //初始化combobox
                    creditTd.attr("accountID", entry.MDebitAccountID).text(entry.MDebitAccountFullName);
                    //第10列
                    var taxOrCreditTd = currentTr.find("td[nouse!=1]").eq(9 - rowspan);
                    //
                    taxOrCreditTd.attr("accountID", docType == receive ? entry.MCreditAccountID : entry.MTaxAccountID).text((docType == receive ? entry.MCreditAccountFullName : entry.MTaxAccountFullName) || "");
                    //第11列
                    var creditOrTaxTd = currentTr.find("td[nouse!=1]").eq(10 - rowspan);
                    //
                    creditOrTaxTd.attr("accountID", docType == payment ? entry.MCreditAccountID : entry.MTaxAccountID).text((docType == payment ? entry.MCreditAccountFullName : entry.MTaxAccountFullName) || "");
                    //第12列
                    currentTr.find("td[nouse!=1]").eq(11 - rowspan).html(that.getVoucherNumberCell(entry));
                    //把列插入到tablebody里面
                    tbody.append(currentTr);
                    //
                    rowspan = 3;
                    //
                    lastDocID = entry.MDocID;
                }
                //加上批量设置行
                canSave && docVoucherData.length > 1 && that.addBatchSetupTr(12, 3);
                //显示
                tbody.show();
                //表格显示
                $(tableBody).show();
                //初始化combobox
                that.initAccountCombo();
            };
            //初始化转账单
            this.initTransferTable = function () {
                //获取到表头，需要对其重命名
                var header = $(headerRow, tableBody).eq(0);
                header.find("th:eq(3)").text(from);
                header.find("th:eq(4)").text(to);
                header.find("th:eq(9)").text(debitAccount);
                header.find("th:eq(10)").text(exchangeLoss);
                header.find("th:eq(11)").text(creditAccount);
                //表体每一行的模版
                var demoTr = $(bodyRow, tableBody);
                //
                var tbody = $("tbody", tableBody);
                //先隐藏
                tbody.hide();
                //
                for (var i = 0; i < docVoucherData.length ; i++) {
                    //
                    var entry = docVoucherData[i];
                    //复制行
                    var currentTr = demoTr.clone();
                    //
                    currentTr.attr("MEntryID", entry.MEntryID);
                    //
                    currentTr.attr("MTaxEntryID", entry.MTaxEntryID);
                    //
                    currentTr.attr("approved", entry.MVoucherStatus);
                    //业务日期
                    currentTr.find("td[nouse!=1]:eq(0)").text(mDate.format(entry.MBizDate));
                    //转出账户
                    currentTr.find("td[nouse!=1]:eq(1)").text(entry.MFromAccountName || "");
                    //转如账户
                    currentTr.find("td[nouse!=1]:eq(2)").text(entry.MToAccountName || "");
                    //摘要
                    currentTr.find("td[nouse!=1]:eq(3)").text(entry.MDesc || "");
                    //金额
                    currentTr.find("td[nouse!=1]:eq(4)").text(mMath.toMoneyFormat(entry.MAmount));

                    //
                    currentTr.removeClass("demo");
                    //第9列
                    var debitTd = currentTr.find("td[nouse!=1]:eq(6)");
                    //初始化combobox
                    debitTd.attr("accountID", entry.MDebitAccountID).text(entry.MDebitAccountFullName);
                    //第10列
                    var exchangeLossTd = currentTr.find("td[nouse!=1]:eq(7)");
                    //初始化combobox
                    exchangeLossTd.attr("accountID", entry.MTaxAccountID || "").text(entry.MTaxAccountFullName || "");
                    //第11列
                    var creditTd = currentTr.find("td[nouse!=1]:eq(8)");
                    //
                    creditTd.attr("accountID", entry.MCreditAccountID).text(entry.MCreditAccountFullName);
                    //第12列
                    currentTr.find("td[nouse!=1]:eq(9)").html(that.getVoucherNumberCell(entry));
                    //把列插入到tablebody里面
                    tbody.append(currentTr);
                }
                //加上批量设置行
                canSave && docVoucherData.length > 1 && that.addBatchSetupTr(10, 3);
                //显示
                tbody.show();
                //表格显示
                $(tableBody).show();
                //初始化combobox
                that.initAccountCombo();
            };
            //初始化combobox选择
            this.initAccountCombo = function () {
                //每一个合适的td
                $(accountTd + ":visible", tableBody).each(function (index) {
                    //先获取里面的id
                    var accountId = $(this).attr("accountID");
                    //
                    var td = $(this);
                    //
                    var tr = td.parent();
                    //
                    var approved = tr.attr("approved") == "1";
                    //
                    var taxEntryID = tr.attr("MTaxEntryID");
                    //如果是税科目的话，则需要有税分录ID，如果不是的话就不用管
                    if ((td.hasClass(taxInfoClass) && taxEntryID) || !td.hasClass(taxInfoClass)) {
                        //
                        var $input = $("input", this);
                        //
                        if ($input.length == 0) {
                            $input = $("<input class='" + accountInputClass + "' />");
                            //
                            $(this).text("").append($input);
                        }
                        //
                        $input.addClass("m-account-combobox");
                        //输入框
                        $input.mAddCombobox("account", {
                            data: accountData,
                            height: 25,
                            required: !$input.parent().parent().hasClass(batchSetupClass),
                            autoSizePanel: true,
                            onSelect: function (data) {
                                //
                                that.batchSetup($input, data);
                            },
                            onLoadSuccess: function () {
                                //如果
                                $input.combobox("setValue", accountId);
                                //如果已经审核，本行就不能再选科目了
                                approved && new mCombo($input).createInstance().readonly();

                                //如果是批量设置的话，需要显示底纹
                                if ($input.parent().parent().hasClass(batchSetupClass)) {
                                    $input.combobox('textbox').attr("hint", $input.attr("hint")).initHint();
                                }
                            }
                        }, {
                            hasPermission: 1,
                            //关闭后需要重新加载摘要
                            callback: function () {
                                //重新获取摘要
                                that.getAccountData(true, function (data) {

                                    that.reloadAccountData(data);

                                    accountData = data;
                                }, true);
                            }
                        });
                    }
                });
                //全选
                Megi.regClickToSelectAllEvt();
            };
            //重新设置科目
            this.reloadAccountData = function (data) {

                $(".m-account-combobox").each(function () {

                    var value = $(this).combobox("getValue");
                    //重新加载
                    $(this).combobox("loadData", data);
                    //重新加载
                    $(this).combobox("reload");

                    $(this).combobox("setValue", value);
                });
            };
            //批量同步
            this.batchSetup = function ($input, data) {
                //获取其index
                var index = $input.attr("colIndex");
                //
                if (index) {
                    //把整个表格里面的combo都同步一下
                    $("tr:visible:gt(1)", tableBody).filter(function () {
                        //需要没有创建凭证的行
                        return $(this).attr("approved") != 1;
                    }).each(function (i) {
                        //
                        var comboInput = $(".de-account-td:visible:eq(" + index + ") input." + accountInputClass, $(this));
                        //
                        if (comboInput.length > 0) {
                            //
                            comboInput.combobox("setValue", data.MItemID);
                            //
                            comboInput.combobox("setText", data.MFullName);
                        }
                    });
                }
            };
            //根据一个docID获取所有的相应项
            this.getDocVoucherByDocID = function (docID, entryID) {
                //
                var result = [];
                //
                for (var i = 0; i < docVoucherData.length ; i++) {
                    //如果有docID，或者
                    if ((docID && !entryID && docVoucherData[i].MDocID == docID)
                        || (docID && entryID && docVoucherData[i].MDocID == docID && docVoucherData[i].MEntryID == entryID)) {
                        //
                        result.push(docVoucherData[i]);
                    }
                }
                //
                return result;
            };
            //获取批量设置的行
            this.addBatchSetupTr = function (colCount, accountCount) {
                //
                var batchTr = "<tr class='" + batchSetupClass + "'><td colspan='" + (colCount - accountCount - 2) + "' class='de-batch-title'>" + "</td>"
                + "<td class='de-empty-cell'>&nbsp;</td>";
                for (var i = 0; i < accountCount ; i++) {
                    batchTr += "<td class='de-account-td'><input type='text' hint='" + HtmlLang.Write(LangModule.GL, "BatchSetup", "批量设置") + "' class='de-account-select' colIndex='" + i + "'></td>"
                }
                //凭证列
                batchTr += "<td>&nbsp;</td></tr>";
                //加入到第一行
                $("tr:eq(1)", tableBody).before(batchTr);
            };
            //保存规则
            this.save = function (create) {
                //先获取数据
                //先获取数据
                that.getTableData();
                //
                var newData = [];
                //
                $.extend(newData, docVoucherData);
                //
                home.createDocVoucher(newData, create, function (data) {
                    //
                    if (data.Success) {
                        //保存成功
                        $.mDialog.message(create ? createSuccessLang : LangKey.SaveSuccessfully);
                        //刷新页面
                        that.initDocVoucher2Table();
                    }
                    else {
                        //操作失败
                        mDialog.error(data.Message);
                    }
                }, "", true);
            };
            //获取科目数据
            this.getAccountData = function (force, func, aysnc) {
                //
                aysnc = aysnc || $.isFunction(func);
                //如果强制获取的话
                if (force === true) {
                    //
                    home.getAccountData(func, {}, aysnc);
                }
                else if (accountData.length == 0) {
                    //
                    return home.getAccountData(func, {}, aysnc);
                }
                else {
                    $.isFunction(func) && func(accountData);
                }
                //
                return accountData;
            };
            //获取表格中的数据并且组装成一个obj
            this.getTableData = function (entryID) {
                //获取每一行
                for (var i = 0; i < docVoucherData.length ; i++) {
                    //
                    var entry = docVoucherData[i];
                    //找到对应的行
                    var tr = $("tr[MEntryID='" + entry.MEntryID + "']", tableBody);
                    //
                    var debitInput = $(debitInfo, tr).find("input." + accountInputClass);
                    //
                    var taxInput = $(taxInfo + ":visible", tr).find("input." + accountInputClass);
                    //
                    var creditInput = $(creditInfo, tr).find("input." + accountInputClass);
                    //取Debit科目
                    entry.MDebitAccountID = debitInput.length > 0 ? debitInput.combobox("getValue") : "";
                    //取tax科目
                    entry.MTaxAccountID = taxInput.length > 0 ? taxInput.combobox("getValue") : "";
                    //取Credit科目
                    entry.MCreditAccountID = creditInput.length > 0 ? creditInput.combobox("getValue") : "";
                    //取Debit科目
                    entry.MDebitAccountFullName = debitInput.length > 0 ? debitInput.combobox("getText") : "";
                    //取tax科目
                    entry.MTaxAccountFullName = taxInput.length > 0 ? taxInput.combobox("getText") : "";
                    //取Credit科目
                    entry.MCreditAccountFullName = creditInput.length > 0 ? creditInput.combobox("getText") : "";
                    //如果只需要取一行的话，则直接返回本行
                    if (entryID && entryID == entry.MEntryID) {
                        //
                        return entry;
                    }
                }
            };
            //事件的初始化
            this.initEvent = function () {
                //是否可以保存:已经审核的凭证数 == 分录数,并且本期没有结账
                canSave = home.filterGetApprovedVoucher(docVoucherData, true).length != docVoucherData.length && !periodClosed;
                //是否可以创建取决于，先可以保存，然后有可以创建的凭证,并且不是所有的都生成凭证了
                canCreate = canSave && home.filterCanCreateVoucher(docVoucherData, true).length > 0 && home.filterDocVoucherByHasVoucher(docVoucherData, true).length < docVoucherData.length;
                //禁用保存
                canSave ? $(saveButton).show() : $(saveButton).hide()

                if (canCreate) {
                    $(createButton).show();
                    $(saveButton).removeClass("easyui-linkbutton-yellow");
                }
                else {
                    $(createButton).hide()
                }
                //保存
                $(saveButton).off("click").on("click", function () {

                    //如果有已经创建的凭证，点击保存的时候提醒用户
                    if (home.filterDocVoucherByHasVoucher(docVoucherData, true).length > 0) {

                        mDialog.confirm(HtmlLang.Write(LangModule.GL, "SaveAgainMayLeadsToDeleteCreatedVoucher", "重新保存可能会导致已经创建的凭证被删除，是否确认保存?"), function () {

                            //
                            that.save(false);
                        });
                    }
                    else {

                        //
                        that.save(false);
                    }
                });
                //声称
                $(createButton).off("click").on("click", function () {
                    //
                    that.save(true);
                })
            };
            //初始化
            this.init = function (_docIDs, _docType) {
                //
                docType = _docType || "";
                //
                docIDs = _docIDs ? _docIDs.split(',') : [];
                //获取科目数据
                that.getAccountData(true, this.initDocVoucher2Table);
            };
        };
        //
        return GLDocVoucherRuleEdit;
    })();
    //
    window.GLDocVoucherRuleEdit = GLDocVoucherRuleEdit;
})()