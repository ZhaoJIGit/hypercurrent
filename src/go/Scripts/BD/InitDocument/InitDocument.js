(function () {
    var InitDocument = (function () {
        //业务系统启用日期，总账启用日期
        var BeginDate, GLBeginDate, MaxBillDate;
        //目前展示的维度
        var currentType = ACCOUNT = 0, BILLTYPE = 1;
        //左侧的编辑默认是展开的，用户在编辑表格的时候会自动收起来
        var shrinkStatus = 1;
        //当前页面不需要取消校验，保持校验
        window.noCancelValidate = true;
        //
        var tableColumns = [];
        //
        var lastFunc = null;
        //当前用户所选择的科目
        var accountType = {};
        //
        var isSettingValue = false;
        //单据类型
        var allBillType = -1, invoiceSale = 0, invoicePurchase = 1, receive = 2, payment = 3, expense = 4;
        //当前单据类型
        var currentBillType = allBillType;
        //
        var lastTabIndex = 0;
        //是否启用了业务系统,是否可以录入单据
        var IsBusinessSystemInited = false;
        //
        var mWidth = 900, mHeight = 500;
        //是否在一月启用的总账
        var IsGLBeginInJan = false;
        //外币和金额的列表
        var InitBillAmountFor = [];
        //
        var OrignalType = 0, BalanceType = 1;
        //是否完成了初始化
        var isInitOver = false;
        //小数点位数
        var precise = 2;
        //
        var checkTypeDal = new GLCheckType();
        //本位币
        var baseCyID = MegiGlobal.BaseCurrencyID;
        //联系人的类型
        var checkType = -1, allType = -1,
            checkTypeContact = 0,
            checkTypeEmployee = 1,
            checkTypeMerItem = 2,
            checkTypeExpItem = 3,
            checkTypePaItem = 4,
            checkTypeTrackItem1 = 5,
            checkTypeTrackItem2 = 6,
            checkTypeTrackItem3 = 7,
            checkTypeTrackItem4 = 8,
            checkTypeTrackItem5 = 9;
        //业务类型
        var mTypeLang = HtmlLang.Write(LangModule.Common, "DocumentType", "业务类型");
        //差额不为空，不可以保存
        var differNotEqualZeroSureToSave = HtmlLang.Write(LangModule.Common, "DifferNotEqualZeroSureToSave", "差额不为0,确认是否进行保存!");
        //
        var pleaseSelectAContact = HtmlLang.Write(LangModule.Common, "PleaseSelectAContactOrEmployee", "请选择一个联系人或员工!");
        //
        var pleaseSelectAnAccount = HtmlLang.Write(LangModule.Common, "PleaseSelectAnAccount", "请选择一个科目!");
        //删除失败
        var deleteFailed = HtmlLang.Write(LangModule.Common, "DeleteFailed", "删除失败!");
        //删除成功
        var deleteSuccessfully = HtmlLang.Write(LangModule.Common, "DeleteSuccessfully", "删除成功!");
        //更新单据往来科目成功
        var updateBillCurrentAccountSuccessfully = HtmlLang.Write(LangModule.GL, "updateBillCurrentAccountSuccessfully", "更新单据往来科目成功!");
        //
        var leaveWithUnsaveDataConfirm = HtmlLang.Write(LangModule.Common, "leaveWithUnsaveDataConfirm", "表格中有未保存的初始化单据，是否确认离开?");
        //初始化已经完成
        var balanceInitIsOverBillUpdateIsForbidden = HtmlLang.Write(LangModule.Common, "balanceInitIsOverBillUpdateIsForbidden", "初始化已经完成，单据不可以再进行修改!");
        //提醒用户没有勾选生成初始化单据的科目不可编辑初始化单据
        var cannotEditAccountInitBill = HtmlLang.Write(LangModule.BD, "cannotEditAccountInitBill", "当前科目没有设置生成业务单据，无法修改或者创建业务单据，如需修改，请在科目初始余额界面选中对应科目勾选【生成初始化单据】!");
        //单据类型列表
        var BillTypeListData = [
            {
                "id": "All_Type",
                "value": HtmlLang.Write(LangModule.Common, "AllType", "全部类型")
            },
            [
            //销售发票
            {
                "id": "Invoice_Sale",
                "value": HtmlLang.Write(LangModule.Common, "InvoiceSale", "销售单")
            },
            //销售发票
            {
                "id": "Invoice_Sale_Red",
                "value": HtmlLang.Write(LangModule.Common, "InvoiceSaleRed", "红字销售单")
            }],
            //采购单
            [{
                "id": "Invoice_Purchase",
                "value": HtmlLang.Write(LangModule.Common, "InvoicePurchase", "采购单")
            },
            //红字采购单
            {
                "id": "Invoice_Purchase_Red",
                "value": HtmlLang.Write(LangModule.Common, "InvoicePurchaseRed", "红字采购单")
            }],
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
        //科目列表
        var AccountListData = [
            //应收账款
            {
                MCode: "1122",
                MItemID: "",
                MName: HtmlLang.Write(LangModule.Common, "AccountReceivable", "应收账款"),
                MInitBalanceModel: [{
                    MInitBalance: 0,
                    MInitBalanceFor: 0
                }],
                MCheckGroupModel: {

                },
                MSubAccounts: []
            },
            //预收账款
             {
                 MCode: "2203",
                 MItemID: "",
                 MName: HtmlLang.Write(LangModule.Common, "ReceiveInAdvance", "预收账款"),
                 MInitBalanceModel: [{
                     MInitBalance: 0,
                     MInitBalanceFor: 0
                 }],
                 MCheckGroupModel: {

                 },
                 MSubAccounts: []
             },
            //其他应收
             {
                 MCode: "1221",
                 MItemID: "",
                 MName: HtmlLang.Write(LangModule.Common, "OtherReceivable", "其他应收"),
                 MInitBalanceModel: [{
                     MInitBalance: 0,
                     MInitBalanceFor: 0
                 }],
                 MCheckGroupModel: {

                 },
                 MSubAccounts: []
             },
            //应付账款
            {
                MCode: "2202",
                MItemID: "",
                MName: HtmlLang.Write(LangModule.Common, "AccountPayable", "应付账款"),
                MInitBalanceModel: [{
                    MInitBalance: 0,
                    MInitBalanceFor: 0
                }],
                MCheckGroupModel: {

                },
                MSubAccounts: []
            },
            //预付账款
           {
               MCode: "1123",
               MItemID: "",
               MName: HtmlLang.Write(LangModule.Common, "Prepayment", "预付账款"),
               MInitBalanceModel: [{
                   MInitBalance: 0,
                   MInitBalanceFor: 0
               }],
               MCheckGroupModel: {

               },
               MSubAccounts: []
           },
            //其他应付
            {
                MCode: "2241",
                MItemID: "",
                MName: HtmlLang.Write(LangModule.Common, "OtherPayable", "其他应付"),
                MInitBalanceModel: [{
                    MInitBalance: 0,
                    MInitBalanceFor: 0
                }],
                MCheckGroupModel: {

                },
                MSubAccounts: []
            }
        ];
        //
        var BankAccountListData = [];
        //联系人搜索输入框
        var checkTypeSerachInputDemo = ".id-checktype-search-input-demo";
        //联系人搜索输入框
        var checkTypeSerachInput = ".id-checktype-search-input";
        //联系人列表
        var checkTypeList = ".id-checktype-list";
        //
        var checkTypeListDiv = ".id-checktype-list-div";
        //联系人列表
        var checkTypeListDemo = ".id-checktype-list-demo";
        //
        var checkTypeListDivDemo = ".id-checktype-list-div-demo";
        //
        var checkTypeDiv = ".id-checktype-div";
        //
        var checkTypeDivDemo = ".id-checktype-div-demo";
        //科目选择的combo
        var accountCombo = ".id-account-combo";
        //设定科目的文本
        var setAccountLable = ".id-account-combo-label";
        //科目选择combo的label
        var accountComboLabel = ".id-account-combo-label";
        //
        var searchButton = ".id-checktype-data-search";
        //
        var subAccountSelect = ".id-sub-account-select";
        //
        var subAccountUl = ".id-sub-account-ul";
        //
        var clearCheckTypeDiv = ".id-clear-checktype-div";
        //
        var subAccountLi = ".id-sub-account-li";
        //客户
        var customerDiv = ".id-customer-list";
        //供应商
        var supplierDiv = ".id-supplier-list";
        //员工
        var employeeDiv = ".id-employee-list";
        //
        var otherDiv = ".id-other-list";
        //
        var billtypeUl = ".id-billtype-ul";
        //
        var accountUl = ".id-account-ul";
        //
        var typeTitle = ".id-type-title";
        //
        var contentTop = ".id-content-top";
        //
        var contentBottom = ".id-content-bottom";
        //
        var subTypeTitle = ".id-sub-title";
        //
        var typeChange = ".id-type-change";
        //
        var accountArrowSpan = ".id-account-arrow-span";
        //
        var allTypeDiv = ".id-all-type";
        //
        var totalSpan = ".id-total-span";
        //
        var totalDiv = ".id-total-div";
        //
        var accountArrow = ".id-account-arrow";
        //
        var customerTypeDiv = ".id-customer-type";
        //
        var supplierTypeDiv = ".id-supplier-type";
        //
        var employeeTypeDiv = ".id-employee-type";
        //
        var otherTypeDiv = ".id-other-type";
        //
        var gridTableDiv = ".id-gridtable-div";
        //
        var contentMain = ".id-content-main";
        //
        var contentRight = ".id-content-right";
        //
        var contentLeft = ".id-content-left";
        //
        var contentTop = ".id-content-top";
        //
        var contentBottom = ".id-content-bottom";
        //
        var shrinkOpen = ".m-menu-shrink-open";
        //
        var shrinkClose = ".m-menu-shrink-close";
        //
        var tableBody = "#gridTable";
        //保存按钮
        var saveButton = "#btnSave";
        //保存单据
        var saveBillButton = "#btnBillSave";
        //
        var homeDiv = ".id-home-div";
        //底部的外部计算div
        var balanceInfoTable = ".id-balance-info-table";
        //
        var originalBalanceInfoTable = ".id-original-balance-info-table";
        //
        var accountBalanceInfoTr = ".demo.id-account-balance-info-tr";
        //
        var docTotalInfoTr = ".demo.id-doc-total-info-tr";
        //币别名称
        var forLiCurrency = ".id-for-li-currency";
        //业务单据统计div
        var forDiv = ".id-for-div";
        //科目原始初始化余额
        var originalDiv = ".id-original-div";
        //币别金额
        var forLiValue = ".id-for-li-value";
        //
        var newLineButton = ".new-line-button";
        //
        var removeLineButton = ".remove-line-button";
        //
        var removeBillButton = ".remove-bill-button";
        //编辑初始化单据
        var editBillButton = ".edit-bill-button";
        //
        var applyEditButton = ".apply-edit-button";
        //期初余额（综合本位币）
        var initBalanceBaseInput = ".id-init-balance-base";
        //期初余额（本位币）
        var initBalanceSpan = ".id-init-balance-span";
        //
        var docSpan = "span.id-doc-span";
        //
        var docForSpan = "span.id-doc-for-span";
        //期初余额（本位币）
        var initBalanceForSpan = ".id-init-balance-for-span";
        //总额
        var totalAmountInput = ".id-total-amount";
        //差额
        var differenceAmountInput = ".id-difference-amount";
        //
        var differenceAmountLabel = ".id-difference-label";
        //相等的样式
        var equalClass = "id-equal-value";
        //
        var notEqualClass = "id-notequal-value";
        //
        var balanceInfo = ".id-balance-info";
        //
        var statisticInfo = ".id-statistic-info";
        //
        var checkTypeSelectHtml = "<div class='id-checktype-selected'>&nbsp;</div>";
        //
        var checkTypeSelectedClass = "id-checktype-selected";
        //
        var checkTypeSearchDiv = ".id-checktype-search-div";

        //销售单统计行
        var docTotalInfoFor = "tr.id-doc-total-info-tr";
        //加载初始化数据
        var getInitDocumnetUrl = "/BD/InitDocument/GetInitDocument";
        //删除初始化单据
        var removeInitBillUrl = "/BD/InitDocument/RemoveInitBill";
        //获取科目的数据
        var getCurrentAccountUrl = '/BD/BDAccount/GetCurrentAccountBaseData';
        //获取科目维度或者联系人维度的信息
        var getInitDocumentBaseDataUrl = '/BD/InitDocument/GetInitDocumentData';
        //保存初始化单据
        var saveInitDocumentUrl = "/BD/InitDocument/SaveInitDocument";
        //获取一行账户信息
        var getBankAccountUrl = "/BD/BDBank/GetAllBDBankAccountInfo";
        //初始化发票
        var invoiceInitEditUrl = "/IV/Invoice/InvoiceInitEdit";
        //初始化红字发票
        var invoiceCreditNoteInitEditUrl = "/IV/Invoice/CreditNoteInitEdit";
        //初始化采购发票
        var purchaseInitEditUrl = "/IV/Bill/BillInitEdit";
        //初始化采购发票
        var purchaseInitCreditNoteEditUrl = "/IV/Bill/CreditNoteInitEdit";
        //初始化收款单发票
        var receiveInitEditUrl = "/IV/Receipt/ReceiptInitEdit";
        //初始化付款单发票
        var paymentInitEditUrl = "/IV/Payment/PaymentInitEdit";
        //初始化费用报销单发票
        var expenseInitEditUrl = "/IV/Expense/ExpenseInitEdit";
        //编辑发票url
        var invoiceEditUrl = "/IV/Invoice/InvoiceView/";
        //编辑采购单
        var purchaseEditUrl = "/IV/Bill/BillView/";
        //编辑费用报销
        var expenseEditUrl = "/IV/Expense/ExpenseEdit/";
        //编辑收款单
        var receiveEditUrl = "/IV/Receipt/ReceiptView/";
        //编辑付款单
        var paymentEditUrl = "/IV/Payment/PaymentView/";
        //更新单据的往来科目
        var updateDocCurrentAccountCodeUrl = "/BD/InitDocument/UpdateDocCurrentAccountCode";
        //
        var expenseInitEdit = "";
        //
        var accountLang = HtmlLang.Write(LangModule.BD, "AccountName", "会计科目");
        //
        var contactLang = HtmlLang.Write(LangModule.BD, "Contact", "联系人");
        //
        var billTypeLang = HtmlLang.Write(LangModule.BD, "DocumentTypes", "单据类型");
        //单据号
        var bankLang = HtmlLang.Write(LangModule.Common, "BankName", "银行账号");
        //单据号
        var numberLang = HtmlLang.Write(LangModule.Common, "BillNumber", "单据号");
        //备注
        var referenceLang = HtmlLang.Write(LangModule.Common, "Reference", "备注");
        //业务日期
        var bizDateLang = HtmlLang.Write(LangModule.Common, "BizDate", "业务日期");
        //到期日
        var dueDateLang = HtmlLang.Write(LangModule.Common, "DueDate", "到期日");
        //金额
        var amountLang = HtmlLang.Write(LangModule.Common, "Amount", "金额");
        //币别
        var currencyLang = HtmlLang.Write(LangModule.Common, "Currency", "币别");
        //币别
        var currenctAccountLang = HtmlLang.Write(LangModule.Common, "currentAccount", "往来科目");
        //外币金额
        var forCurrencyLang = HtmlLang.Write(LangModule.Common, "ForeignCurrency", "原币金额");
        //操作
        var operateLang = HtmlLang.Write(LangModule.Common, "Operation", "操作");
        //初始化单据不可修改
        var initBillCannotModify = HtmlLang.Write(LangModule.Common, "InitBillCannotModify", "业务系统初始化单据不可修改");
        //请在业务系统修改单据金额
        var pleaseModifyInBusinessSystem = HtmlLang.Write(LangModule.Common, "PleaseModifyInBusinessSystem", "单据已经生成，请在业务系统修改单据详情");
        //有重复的单据编号
        var existsDuplicatedNumber = HtmlLang.Write(LangModule.Common, "ExistsDuplicatedNumber", "存在重复的单据编号");
        //提醒用户输入原币金额
        var pleaseInputForeignCurrencyAmount = HtmlLang.Write(LangModule.Common, "PleaseInputForeignCurrencyAmount", "请输入原币金额");
        //提醒用户输入本位币金额
        var pleaseInputBaseCurrencyAmount = HtmlLang.Write(LangModule.Common, "PleaseInputBaseCurrencyAmount", "请输入本位币金额");
        //业务日期不可大于到期日
        var businessDateMustLargerThanDueDate = HtmlLang.Write(LangModule.Common, "BusinessDateMustLargerThanDueDate", "业务日期必须大于到期日");
        //单据的业务到日期是必填的
        var dueDateCannotBeNull = HtmlLang.Write(LangModule.Common, "InvoiceExpenseDueDateCannotBeNull", "销售单/采购单/费用报销单的到期日为必填!");
        //收付款单需要选择银行账户
        var bankShouldBeSelectedWithReceiptOrPayment = HtmlLang.Write(LangModule.Common, "bankShouldBeSelectedWithReceiptOrPayment", "收付款单必须选择银行");
        //是否确定永久性删除初始化单据
        var areYouSureToDeleteInitBillPermanently = HtmlLang.Write(LangModule.Common, "areYouSureToDeleteInitBillPermanently", "是否确定永久性删除初始化单据?");
        //
        var contactLang = HtmlLang.Write(LangModule.BD, "Contact", "联系人");
        //
        var employeeLang = HtmlLang.Write(LangModule.BD, "Employee", "员工");
        //
        var merItemLang = HtmlLang.Write(LangModule.BD, "MerItem", "商品项目");
        //
        var expItemLang = HtmlLang.Write(LangModule.BD, "ExpItem", "费用项目");
        //
        var paItemLang = HtmlLang.Write(LangModule.BD, "PaItem", "工资项目");
        //
        var allTypeLang = HtmlLang.Write(LangModule.Common, "AllType", "全部类型");
        //
        var noNeedShowClass = "no-need-show";
        //
        var InitDocument = function () {
            //
            var that = this;
            //编辑的行和列
            this.editIndex = this.editRow = null;
            //初始化事件
            this.initEvent = function () {
                //新建联系人
                $("#btnNewContact,#btnNewTop").off("click").on("click", that.addNewContact);
                //导入联系人
                $("#btnImportContact,#btnImportTop").off("click").on("click", that.importContact);
                //新建员工
                $("#btnNewEmployee").off("click").on("click", that.addNewEmployee);
                //导入员工
                $("#btnImportEmployee").off("click").on("click", that.importEmployee);
                //切换联系人和科目维度
                $(typeChange).off("click").on("click", function () {
                    //
                    that.checkDataGridUnsaveBill(that.switchAccountBillType);
                });
                //保存初始化账单
                $(saveButton).off("click").on("click", function () {
                    //
                    that.saveInitDocumentModel();
                });
                //div点击结束编辑的事件
                that.intEndGridEditEvent();
                //
                $(document).off("click", shrinkOpen).on("click", shrinkOpen, function () {
                    //
                    shrinkStatus = that.shrink(-1);
                });
                //
                $(document).off("click", shrinkClose).on("click", shrinkClose, function () {
                    //
                    shrinkStatus = that.shrink(1);
                });
                //
                $(searchButton).off("click").on("click", that.updateData);
                //
                window.onresize = that.resize;
            };
            //点击查询数据
            this.updateData = function () {
                //
                that.getInitDocumentData(that.initDocumentData);
            }
            //
            this.shrink = function (dir) {
                //
                var status = -1;
                //
                dir = dir || 1;
                //隐藏左边
                if (dir === 1) {
                    //显示左边
                    $(contentLeft).show();
                    //
                    status = 1;
                    //
                    $(shrinkClose).removeClass(shrinkClose.trimStart('.')).addClass(shrinkOpen.trimStart('.'));
                    //
                    $.isFunction(lastFunc) && lastFunc();
                    //
                    lastFunc = null;

                }
                else {
                    //
                    status = -1;
                    //
                    $(contentLeft).hide();
                    //
                    $(shrinkOpen).removeClass(shrinkOpen.trimStart('.')).addClass(shrinkClose.trimStart('.'));
                }
                //
                that.resizeFrame();
                //
                that.resizeTable();
                //
                return status;
            }
            //点击其他地方结束编辑的事件
            this.intEndGridEditEvent = function () {
                //
                $(document).off("click.end", contentTop).on("click.end", contentTop, that.endEdit);
                //
                $(document).off("click.end", contentBottom).on("click.end", contentBottom, that.endEdit);
            };
            //结束编辑事件
            this.endEdit = function () {
                //
                if (that.editIndex != null) {
                    //结束当前编辑行的编辑状态
                    if (that.validateEditRow()) {
                        //先结束编辑
                        $(tableBody).datagrid("endEdit", that.editIndex);
                    }
                }
            };
            //
            this.saveInitDocumentModel = function () {
                //
                if (that.editIndex != null) {
                    //
                    if (that.validateEditRow()) {
                        //先结束编辑
                        $(tableBody).datagrid("endEdit", that.editIndex);
                    }
                    else {
                        return false;
                    }
                }
                //
                if (that.validateTableData(alert)) {
                    //保存并且刷新整个页面
                    that.saveInitDocument(function () {
                        //更新tab页的值
                        that.updateTabValue();
                        //
                        that.updateData();
                    });
                }
            };
            //更新tab上的值
            this.updateTabValue = function () {
                //
                if (currentType == ACCOUNT) {
                    //
                    that.initAccountBaseData(that.showAccountBaseData);
                }
                else {
                    //更新金额
                    that.getInitDocumentBaseData(currentType, that.showInitDocumentBaseData);
                }
                //
                $.mTab.refresh(HtmlLang.Write(LangModule.BD, "AccountBalancesFinancial", "科目初始余额"), '/BD/BDAccount/AccountBalances', false);
            }
            //初始化tab type = 0 表示是accountType= 1表示为contact
            this.initTab = function (index) {
                //
                var tab = currentType == BILLTYPE ? billtypeUl : accountUl;
                //隐藏其他的
                $(tab).show().siblings().hide();
                //初始化
                $(tab).tabsExtend({
                    //默认的显示标签
                    initTabIndex: index || 1,
                    //选择标签函数
                    onSelect: function (index) {
                        //
                        index > 0 && that.checkDataGridUnsaveBill(function () {
                            //
                            that.showTab(index - 1);
                            //
                            lastTabIndex = index - 1;
                        }, function () {
                            //
                            var tab = currentType == BILLTYPE ? billtypeUl : accountUl;
                            //加上class
                            $(tab).find("li:eq(" + lastTabIndex + ")").addClass("current").siblings().removeClass("current");
                        });
                    }
                });
            };
            //展示tab下相应的内容
            this.showTab = function (index) {
                //
                var tab = currentType == BILLTYPE ? billtypeUl : accountUl;
                //加上class
                $(tab).find("li:eq(" + index + 1 + ")").addClass("current").siblings().removeClass("current");
                //联系人类型
                checkType = allType;
                //重新初始化联系人选择
                that.showCheckTypePanel(allType);
                //先清除表头的数据
                that.clearTableHeaderData();
                //只有选了联系人才做
                //针对于联系人的时候，tab对应的是科目
                if (currentType == ACCOUNT) {
                    //
                    account = AccountListData[index];
                    //
                    if (that.isLeftShow()) {
                        //重新加载联系人的信息
                        that.initCheckTypeList(account);
                        //
                        lastFunc = null;
                    }
                    else {
                        //
                        lastFunc = function () {
                            //重新加载联系人的信息
                            that.initCheckTypeList(account);
                        }
                    }
                    //
                    accountType = index;
                    //
                    that.enableDifference();
                }
                else {
                    //
                    currentBillType = index;
                    //
                    that.enableDifference();
                    //
                    if (that.isLeftShow()) {
                        //重新加载联系人的信息
                        that.initCheckTypeList();
                        //
                        lastFunc = null;
                    }
                    else {
                        //
                        lastFunc = function () {
                            //重新加载联系人的信息
                            that.initCheckTypeList();
                        }
                    }
                }
                //
                that.getInitDocumentData(that.initDocumentData);

                that.updateTabValue();

            };
            //判断左边是否有显示
            this.isLeftShow = function () {
                //
                return $(contentLeft).is(":visible");
            }
            //根据单据类型显示核算维度
            //根据科目类型显示科目的期初和期末
            this.showAccountBalanceInfo = function (data) {
                //初始化
                $(initBalanceBaseInput).numberbox("setValue", data);
                //
                return true;
            };
            //绑定往来科目右边的点击出现的子科目
            this.bindTabAccountEvent = function ($arrow, accounts, code) {
                //
                that.initSubAccountDiv(accounts, code);
                //
                var $div = $(subAccountSelect + "[mcode='" + code + "']")
                //
                $arrow.mTip({
                    target: $div,
                    width: 230,
                    parent: $div.parent()
                });
                //
                that.bindSubAccountEvent($div);
            };
            //初始化科目选择里面的点击事件
            this.bindSubAccountEvent = function ($div) {
                //
                $("li:not(.demo)", $div).off('click').on("click", function () {
                    //
                    contactType = allType;
                    //
                    var code = $(this).attr("mcode");
                    //当前的科目类型
                    accountType = that.getAccountTypeByCode(code);
                    //如果当前的li不是选中状态则不做处理
                    var currentLi = $("li[mcode^='" + code.substring(0, 4) + "']", accountUl);
                    //
                    currentLi.siblings(".current").removeClass("current");
                    //
                    currentLi.addClass("current");
                    //
                    $(".m-tip-top-div").hide();
                    //肯定只有一个的
                    subAccount = that.getAccountDataByCode(code);
                    //
                    that.bindAccount2AccountListData(subAccount);
                    //
                    that.showAccountInfo2Li(currentLi, subAccount);
                    //
                    that.updateData();
                });
            };
            //加一个面板到页面，里面是科目的选择
            this.initSubAccountDiv = function (accounts, code) {
                //先删除相同的
                $(subAccountSelect + "[mcode='" + code + "']").remove();
                //
                var demoDiv = $(".demo" + subAccountSelect).clone();
                //
                demoDiv.removeClass("demo");
                //
                demoDiv.attr("mcode", code);
                //
                demoDiv.appendTo("body");
                //
                that.bindSubAccounts2Div(accounts, demoDiv);
            };
            //把自科目放到li里面去
            this.bindSubAccounts2Div = function (accounts, div) {
                //
                var html = "";
                //
                var demoLi = $("li.demo", div);
                //先删除那些不要的
                demoLi.siblings().remove();
                //
                for (var i = 0; i < accounts.length ; i++) {
                    //
                    var li = demoLi.clone().removeClass("demo");
                    //
                    li.attr("mcode", accounts[i].MCode).text(accounts[i].MFullName);
                    //
                    $(subAccountUl, div).append(li);
                }
            };
            //由科目转化为联系人
            this.switchAccountBillType = function (type, tabIndex) {
                //遮罩起来
                $("body").mask("");
                //把左侧展示出来
                that.shrink(1);
                //如果指定了type就按照指定的来，如果没有指定，就切换
                currentType = type || (currentType == BILLTYPE ? ACCOUNT : BILLTYPE);
                //
                that.clearTableHeaderData();
                //
                if (currentType == BILLTYPE) {
                    //
                    $(typeTitle).text(billTypeLang);
                    //
                    $(subTypeTitle).text(accountLang);
                    //联系人类型
                    contactType = allType;
                    //
                    $(contentTop).hide();
                    //隐藏期初余额本位币
                    $(balanceInfo).hide();
                    //隐藏合计 差额
                    $(statisticInfo).hide();
                    //科目期初余额隐藏
                    $(originalDiv).hide();
                }
                else {
                    //
                    $(typeTitle).text(accountLang);
                    //
                    $(subTypeTitle).text(billTypeLang);
                    //
                    $(contentTop).show();
                    //隐藏期初余额本位币
                    $(balanceInfo).show();
                    //隐藏合计 差额
                    $(statisticInfo).show();
                    //科目期初余额隐藏
                    $(originalDiv).show();
                }
                //
                that.resize();
                //不管是联系人还是科目都是取第一个页签，而不是第0个页签
                that.initTab(tabIndex || 1);
                //
                that.updateTabValue(true);
                //重新计算tab的宽度
                that.resizeTab();
            };
            //初始化总计，numberbox
            this.initNumberbox = function () {
                //
            };
            //更新表格的金额，需要考虑里面有编辑行的情况
            this.updateDifferenceWithEditRow = function (value, valueBase, valueFor) {
                //
                if (that.editIndex != null) {
                    //
                    var editor = that.getRowEditor();
                    //
                    var mtype = editor.MType.getValue();
                    //设置为空
                    if (mtype) {
                        //
                        that.updateDifference(value, {
                            MType: mtype,
                            MCurrentAccountCode: editor.MCurrentAccountCode.getValue(),
                            MTaxTotalAmt: valueBase || editor.MTaxTotalAmt.getValue() || 0,
                            MTaxTotalAmtFor: valueFor || editor.MTaxTotalAmtFor.getValue() || 0,
                            MCyID: editor.MCyID.getValue()
                        });
                    }
                }
                else {
                    //先更新
                    that.updateDifference(value);
                }
            };
            //获取科目的基本信息
            this.initAccountBaseData = function (callback) {
                //
                mAjax.post(getCurrentAccountUrl, {}, function (data) {
                    //
                    if (data) {
                        //
                        $.isFunction(callback) && callback(data);
                    }

                }, false, false, false);
            };
            //把一个科目的信息保存到list
            this.bindAccount2AccountListData = function (account) {
                //
                var topCode = account.MCode.substring(0, 4);
                //
                for (var i = 0; i < AccountListData.length ; i++) {
                    //
                    if (AccountListData[i].MCode.substring(0, 4) == topCode) {
                        //
                        AccountListData[i] = $.extend(true, {}, AccountListData[i], account);
                    }
                }
            }
            //把一个li的科目名称，余额等等显示出来
            this.showAccountInfo2Li = function ($li, account) {
                //科目ID
                $li.attr("mitemid", account.MItemID);
                //科目ID
                $li.attr("mcode", account.MCode);
                //名称
                $li.find("div").eq(0).text(account.MName);
                //
                if ($li.find("div").eq(0).hasClass("tooltip-f")) {
                    //更新
                    $li.find("div").eq(0).attr("etitle", account.MFullName).tooltip("update", account.MFullName);
                }
                else {
                    //更新
                    $li.find("div").eq(0).attr("etitle", account.MFullName).tooltip();
                }
                //
                $("div.title", $li).text(mMath.toMoneyFormat(account.MInitBalanceModels.sum("MInitBalance")));
            };
            //初始化搜索按钮
            this.initSerachTextBox = function () {
                //
                $(checkTypeSerachInput).searchbox("destroy");
                //
                var newSearchInput = $(checkTypeSerachInputDemo).clone();
                //
                newSearchInput.insertAfter($(checkTypeSerachInputDemo));
                //
                newSearchInput.removeClass(checkTypeSerachInputDemo.trimStart('.')).addClass(checkTypeSerachInput.trimStart('.')).show();
                //
                $(checkTypeSerachInput).searchbox({
                    //
                    searcher: function (value, name) {
                        //显示
                        that.searchCheckType(value, mText.decode(name));
                    },
                    height: 25,
                    width: 160,
                    menu: $(checkTypeDiv),
                    prompt: HtmlLang.Write(LangModule.Common, "Searching", '搜索...')
                });
            };
            //根据当前类型，获取联系人数据
            this.getContactDataType = function (type) {
                //
                type = type == undefined ? currentBillType : type;
                //
                switch (type) {
                    case invoiceSale:
                        return [[contactCustomer, CustomerListData]];
                    case invoicePurchase:
                        return [[contactSupplier, SupplierListData]];
                    case receive:
                        return [[contactCustomer, CustomerListData], [contactOther, OtherListData]];
                    case payment:
                        return [[contactSupplier, SupplierListData], [contactEmployee, EmployeeListData], [contactOther, OtherListData]];
                    case expense:
                        return [[contactEmployee, EmployeeListData]];
                    case allType:
                        return [
                            [contactCustomer, CustomerListData],
                            [contactSupplier, SupplierListData],
                            [contactEmployee, EmployeeListData],
                            [contactOther, OtherListData]];
                    default:
                        return [[]];
                }
            };
            //根据当前类型获取div
            this.getContactDiv = function (type) {
                //
                type = type || contactType;
                //
                var targetDiv;
                //
                switch (type) {
                    case contactCustomer:
                        targetDiv = $(customerDiv);
                        break;
                    case contactSupplier:
                        targetDiv = $(supplierDiv);
                        break;
                    case contactEmployee:
                        targetDiv = $(employeeDiv);
                        break;
                    case contactOther:
                        targetDiv = $(otherDiv);
                        break;
                    default:
                        break;
                }
                return targetDiv;
            };
            //根据类型隐藏相应的accordin
            this.showCheckTypePanel = function (billType) {
                //
                var showPanels = hidePanels = [];

                //显示和隐藏
                for (var i = 0; i < showPanels.length ; i++) {
                    //显示
                    $(showPanels[i]).parent().show();
                };
                //
                //显示和隐藏
                for (var i = 0; i < hidePanels.length ; i++) {
                    //显示
                    $(hidePanels[i]).parent().hide();
                };
            };
            //
            this.contains = function (array, item) {
                //
                if (array && array.length > 0) {
                    for (var i = 0; i < array.length ; i++) {
                        if (array[i] && item && array[i].indexOf(item) == 0) {
                            return true;
                        }
                    }
                }
                return false;
            };
            //搜索核算维度的详细值
            this.searchCheckType = function (keyword, checkTypeName) {
                //
                that.initCheckTypeList(AccountListData[0], checkTypeName, keyword, true);
            };
            //获取一个显示所有核算维度的科目
            this.getCheckTypeModelByBillType = function () {
                //
                return {
                    MContactID: currentBillType != expense ? 1 : 0,
                    MEmployeeID: (currentBillType == expense || currentBillType == payment) ? 1 : 0,
                    MMerItemID: currentBillType != expense ? 1 : 0,
                    MExpItemID: (currentBillType == expense || currentBillType == payment) ? 1 : 0,
                    MPaItemID: 0,
                    MTrackItem1: 1,
                    MTrackItem2: 1,
                    MTrackItem3: 1,
                    MTrackItem4: 1,
                    MTrackItem5: 1
                }
            }
            //
            this.initAccordionSearchBox = function (account, checkTypeName, filter, isSearch) {
                //
                $(checkTypeList).remove();
                //
                var newCheckTypeList = $(checkTypeListDemo).clone();
                //
                newCheckTypeList
                    .removeClass(checkTypeListDemo.trimStart('.'))
                    .addClass(checkTypeList.trimStart('.'));
                //
                newCheckTypeList.insertAfter($(checkTypeListDemo)).show();
                //
                if (!isSearch) {
                    //
                    $(checkTypeDiv).remove();
                    //
                    var newCheckTypeDiv = $(checkTypeDivDemo).clone();
                    //全部需要显示
                    newCheckTypeDiv
                        .removeClass(checkTypeDivDemo.trimStart('.'))
                        .addClass(checkTypeDiv.trimStart('.'));
                    //
                    newCheckTypeDiv.insertAfter($(checkTypeDivDemo)).hide();
                }
                //
                var checkGroupModel = account ? account.MCheckGroupModel : that.getCheckTypeModelByBillType();
                //
                for (var i = 0; i < GLCheckType.CheckTypeList.length; i++) {
                    //
                    var value = checkGroupModel[GLCheckType.CheckTypeList[i].column];
                    //
                    var type = GLCheckType.CheckTypeList[i].type;
                    //
                    var div = $("div[ltype=" + type + "]", checkTypeList);
                    //  
                    var searchDiv = $("div[ntype=" + type + "]", checkTypeDiv);
                    //
                    if (type != 4) {
                        //
                        var data = checkTypeDal.getCheckTypeData(GLCheckType.CheckTypeList[i].type);
                        //
                        if (!data.MCheckTypeName || (checkTypeName && checkTypeName !== data.MCheckTypeName)) {
                            //
                            div.empty().attr("title", "").hide();
                            //
                            continue;
                        }
                        //
                        div.empty().attr("title", mText.encode(data.MCheckTypeName)).show();
                        //
                        !isSearch && searchDiv.empty().text(mText.encode(data.MCheckTypeName));
                        //
                        that.bindCheckTypeData2Div(data, div, filter);
                    }
                    else {
                        //
                        div.remove();
                        //
                        !isSearch && searchDiv.remove();
                    }
                }
            };
            //初始化dataList
            this.initCheckTypeList = function (account, checkTypeName, filter, isSearch) {
                //
                var checkType = !!checkTypeName && checkTypeName.toLowerCase() == allTypeLang.toLowerCase() ? false : checkTypeName;
                //有mtype的div先都隐藏，然后再做显示
                that.initAccordionSearchBox(account, checkType, filter, isSearch);
                //
                if (!isSearch) {
                    //搜索框
                    that.initSerachTextBox();
                }
                //
                that.initAccordion();
            };
            //
            this.initAccordion = function () {
                //用折叠的形式展示
                $(checkTypeList).accordion({
                    animate: true,
                    multiple: false,
                    height: $(contentRight).height() - $(checkTypeSearchDiv).height() - 5,
                    width: 220
                });
                //
                that.expandAccordion();
            }
            //获取联系人的其他div
            //重新计算accordion的高度
            this.expandAccordion = function () {
                //
                var firstPanel = $(".accordion-body:visible:eq(0)", checkTypeList);
                //
                if (!firstPanel.attr("title") || !firstPanel.text()) {
                    //
                    $(".accordion-body", checkTypeList).filter(function () {
                        return !!$(this).attr("title") && !!$(this).text();
                    }).eq(0).parent().find(".accordion-collapse").trigger("click");
                }
            };
            //enable期初余额和差额统计功能
            this.enableDifference = function () {

            };
            //禁用期初余额、本年累计的输入
            this.enableBalanceInput = function (enable) {
                //
                var o = enable ? "enable" : "disable";
                //
                $(initBalanceBaseInput).numberbox(o);
                //
                $("input:visible", balanceInfoTable).numberbox(o);
            };
            //把联系人信息展示到div里面去 使用 ul li的形式展示
            this.bindCheckTypeData2Div = function (data, div, filter) {
                //
                data.MDataList = data.MDataList || [];
                //
                div.data("MCheckGroupModel", data).attr("title", mText.encode(data.MCheckTypeName));
                //
                var ulDiv = $("<div class='id-checktype-list-div'></div>");
                //
                ulDiv.appendTo(div);
                //按照li的形式展示
                if (data.MShowType == 0) {
                    //
                    var html = "<ul>";
                    //
                    for (var i = 0; i < data.MDataList.length; i++) {
                        //
                        if (!filter || data.MDataList[i].text.toLowerCase().indexOf(filter.toLowerCase()) >= 0) {
                            //
                            html += "<li id='" + data.MDataList[i].id + "' class='id-panel-li'><span>" + mText.encode(data.MDataList[i].text) + "</span></li>";
                        }
                    }
                    //
                    html += "</ul>";
                    //
                    $(html).appendTo(ulDiv);
                    //
                    $("li", ulDiv).off("click").on("click", function () {
                        //
                        that.showSelectedCheckType2Div(div, $(this).attr("id"), $("span", $(this)).text());
                    });
                }
                else if (data.MShowType == 2 || data.MShowType == 1) {
                    //
                    var dataList = filter ? checkTypeDal.filterTreeData(data.MDataList, filter) : data.MDataList;
                    //按照树的形式展示
                    $(ulDiv).tree({
                        data: dataList,
                        onBeforeSelect: function (node) {
                            if (!$(this).tree('isLeaf', node.target)) {
                                return false;
                            }
                        },
                        onSelect: function (node) {
                            //
                            that.showSelectedCheckType2Div(div, node.id, node.text);
                        }
                    });
                }
            };
            //
            this.shakeButton = function () {

                //
                $(searchButton).removeClass("m-button-shake");

                $(searchButton).addClass("m-button-shake");
            }
            //把用户选择的内容显示到div上面
            this.showSelectedCheckType2Div = function (div, id, text) {
                //
                that.shakeButton();
                //
                var titleDiv = div.prev(".panel-header").find(".panel-title");
                //
                var clearDiv = $("<div class='id-clear-checktype-div'>&nbsp;</div>");
                //
                $(clearCheckTypeDiv, titleDiv.parent()).remove();
                //
                var titleText = titleDiv.text();
                //
                if (titleDiv.attr("id")) {
                    //
                    titleText = titleText.replace(/\[.*\]/, "");
                }
                //
                if (id) {
                    //
                    titleDiv.html(titleText + "[" + mText.encode(text) + "]");
                    //
                    $(titleDiv).after(clearDiv);
                    //
                    $(clearDiv).off("click").on("click", function (e) {
                        //
                        that.showSelectedCheckType2Div(div, "", "");
                        //
                        $(this).remove();
                        //
                        checkTypeDal.stopPropagation(e);
                    });
                }
                else {
                    //
                    titleDiv.html(titleText);
                }
                //
                titleDiv.attr("id", id);
            }
            //初始化dom的值以及
            this.initDomValue = function (tabIndex) {
                //获取银行账户信息
                that.getBankAccountData();
                ////获取科目的基本信息
                //that.initAccountBaseData(function (data) {
                //    //显示基本信息
                //    that.showAccountBaseData(data);
                //});
                //初始化金额numberbox
                that.initNumberbox();
                //
                that.resizeFrame();
            };
            //计算tab页的宽度
            this.resizeTab = function () {
                //
                var $tab = currentType == BILLTYPE ? $(billtypeUl) : $(accountUl);
                //获取第一个li，这个li的宽度是不可以改变的
                var $li0 = $tab.find("li").eq(0);
                //计算剩下的宽度
                var allWith = $tab.width() - $li0.outerWidth();
                //每一个li的width
                var eachWith = allWith / ($tab.find("li").length - 1);
                //剩下的每一个宽度都要
                $tab.find("li").each(function (index) {
                    //
                    if (index != 0) {
                        //
                        $(this).width(eachWith - 20);
                        //重新定义宽度
                        $(".statistics", this).width(eachWith - 20 - $(accountArrowSpan).outerWidth());
                    }
                });
            };
            //显示科目的基本信息
            this.showAccountBaseData = function (data) {
                //
                for (var i = 0; i < AccountListData.length ; i++) {
                    //
                    var topCode = AccountListData[i].MCode.substring(0, 4);
                    //找出所有的子科目
                    var subAccounts = data.where("x.MCode.substring(0,4) == '" + topCode + "'");
                    //排序
                    subAccounts = subAccounts.length > 0 ? subAccounts.OrderByAsc(function (a) { return a.MFullName; }) : subAccounts;
                    //
                    AccountListData[i].MSubAccounts = subAccounts;
                    //
                    var $li = $("li[mcode^='" + topCode + "']", accountUl);
                    //
                    var account2Show = $li.attr("mcode").length > 4 ? subAccounts.where("x.MCode == '" + $li.attr("mcode") + "'")[0] : subAccounts[0];
                    //
                    that.showAccountInfo2Li($li, account2Show);
                    //如果只有一个自科目，则不能选择其他自科目
                    if (subAccounts.length == 1) {
                        //
                        $(accountArrow, $li).parent().remove();
                        //如果移除了 箭头，就重新算下 每个li 的宽度
                        that.resizeTab();
                    }
                    else {
                        //绑定事件
                        that.bindTabAccountEvent($(accountArrow, $li), subAccounts, account2Show.MCode.substring(0, 4));
                    }
                    //
                    that.bindAccount2AccountListData(account2Show);
                }
            };
            //定义框架的高度和宽度
            this.resizeFrame = function () {
                //
                $(".m-imain").height($(window).height() - $(".m-toolbar").outerHeight() - $(".m-toolbar-footer").outerHeight());
                //
                $(homeDiv).height($(".m-imain").height() - 15);
                //
                $(contentMain).height($(homeDiv).height() - $(contentMain).offset().top + $(".m-toolbar").outerHeight() + 15);
                //右侧的宽度
                $(contentRight).width($(contentMain).width() - ($(contentLeft).is(":visible") ? $(contentLeft).width() : 0) - 10);
                //右侧的宽度
                $(contentRight).height($(contentMain).height());
                //
                $(originalBalanceInfoTable).width($(contentRight).width() - $(totalSpan, originalDiv).outerWidth() - 40);
                //
                $(balanceInfoTable).width($(contentRight).width() - $(totalSpan, forDiv).outerWidth() - 40);
                //
                that.resizeTableDiv();

            };
            //重新计算table的高度
            this.resizeTableDiv = function () {
                //
                $(gridTableDiv).height($(contentRight + ":visible").outerHeight() - $(totalDiv + ":visible").outerHeight() - $(contentTop + ":visible").outerHeight() - 5);
            };
            //
            this.resizeTable = function () {
                $(tableBody).datagrid('resize', {
                    width: $(contentRight).width() - 10,
                    height: $(gridTableDiv).height() - 3
                })
            };
            //
            this.getAllAccount = function () {
                //
                var accounts = [];
                //
                for (var i = 0; i < AccountListData.length; i++) {
                    //
                    accounts = accounts.concat(AccountListData[i].MSubAccounts);
                }
                //
                return accounts;
            }
            //根据科目code，获取科目信息
            this.getAccountDataByCode = function (code) {
                //
                var allAccounts = that.getAllAccount();
                //
                var accounts = allAccounts.where("x.MCode == '" + code + "'");
                //
                return accounts.length > 0 ? accounts[0] : {};
            };
            //根据科目code，获取科目信息
            this.getAccountDataByTopCode = function (topCode) {
                //
                var allAccounts = that.getAllAccount();
                //
                var accounts = allAccounts.where(" x.MCode.substring(0,4) == '" + topCode + "'");
                //
                return accounts.length > 0 ? accounts[0] : {};
            };
            //根据科目code，获取科目的type
            this.getAccountTypeByCode = function (code) {
                //
                for (var i = 0; i < AccountListData.length && code ; i++) {
                    //
                    if (AccountListData[i].MCode == code) {
                        //返回
                        return i;
                    }
                    //
                    if (AccountListData[i].MSubAccounts && AccountListData[i].MSubAccounts.length > 0) {
                        //
                        for (var j = 0; j < AccountListData[i].MSubAccounts.length; j++) {
                            //
                            if (AccountListData[i].MSubAccounts[j].MCode === code) {
                                //返回
                                return i;
                            }
                        }
                    }
                }
                //返回默认的 1 吧
                return 1;
            }
            //获取科目选择，根据联系人不同，只能选择相应的科目
            this.getAccountSubData = function () {

                var accountList = [];
                //
                if (currentType == BILLTYPE) {
                    //
                    switch (currentBillType) {
                        //销售单 收款单 应收 预收 其他应收
                        case invoiceSale:
                            accountList = AccountListData[0].MSubAccounts
                                .concat(AccountListData[1].MSubAccounts)
                                .concat(AccountListData[2].MSubAccounts);
                            break;
                        case receive:
                            accountList = AccountListData[0].MSubAccounts
                                .concat(AccountListData[1].MSubAccounts)
                                .concat(AccountListData[2].MSubAccounts)
                                .concat(AccountListData[5].MSubAccounts);
                            break;
                            //采购单 付款单 应付、预付、其他应付
                        case invoicePurchase:
                            accountList = AccountListData[3].MSubAccounts
                               .concat(AccountListData[4].MSubAccounts)
                               .concat(AccountListData[5].MSubAccounts);
                            break;
                        case payment:
                            accountList = AccountListData[3].MSubAccounts
                                .concat(AccountListData[2].MSubAccounts)
                                .concat(AccountListData[4].MSubAccounts)
                                .concat(AccountListData[5].MSubAccounts);
                            break;
                            //
                        case expense:
                            accountList = AccountListData[5].MSubAccounts
                    }
                }
                else {
                    //不需要根据联系人来获取科目，统一放开，任何联系人可以挂任何往来科目
                    accountList = AccountListData[accountType].MSubAccounts;
                }

                return accountList ? accountList.where('!!x.MCreateInitBill') : [];
            };

            //根据维度以及往来科目不同，获取单据类型0,1,2,3,4
            this.getIntBillTypeList = function () {
                //如果是科目维度的，则是全部的单据类型
                if (currentType == ACCOUNT) {
                    //
                    return [invoiceSale, invoicePurchase, receive, payment, expense];
                }
                else {
                    //
                    return [currentBillType];
                }
            };
            //根据维度以及往来科目的不同，获取单据类型
            this.getBillTypeList = function () {
                //如果是科目维度的，则是全部的单据类型
                if (currentType == ACCOUNT) {
                    //
                    switch (accountType) {
                        case 0:
                        case 1:
                            return [BillTypeListData[1][0], BillTypeListData[1][1]].
                            concat(BillTypeListData[3]);
                        case 2:
                            return [BillTypeListData[1][0], BillTypeListData[1][1]].
                            concat(BillTypeListData[3]).concat(BillTypeListData[4]);
                        case 3:
                        case 4:
                            return [BillTypeListData[2][0], BillTypeListData[2][1]].
                            concat(BillTypeListData[4]);
                        case 5:
                            return [BillTypeListData[2][0], BillTypeListData[2][1]].concat(BillTypeListData[3])
                           .concat(BillTypeListData[4]).concat(BillTypeListData[5]);
                        default:
                            break;
                    }
                }
                else {
                    //
                    switch (currentBillType) {
                        //客户只能选择应收和红字发票，收款单
                        case invoiceSale:
                            return [BillTypeListData[1][0], BillTypeListData[1][1]];
                            //供应商只能选择采购单，红字采购单和付款单
                        case invoicePurchase:
                            return [BillTypeListData[2][0], BillTypeListData[2][1]];
                            //员工和费用报销单和付款单
                        case receive:
                            return [BillTypeListData[3]];
                            //其他只能选择收付款单
                        case payment:
                            return [BillTypeListData[4]];
                            //
                        case expense:
                            return [BillTypeListData[5]];
                        default:
                            return BillTypeListData;
                    }
                }
            };

            //帮一个联系人的信息绑定到表格以及上面的总览
            this.initDocumentData = function (data) {
                //清空表格数据
                that.clearTableRows();
                //清空下面的内容
                that.clearTotalData();
                //把里面的各种单据的信息，插入到表格里面
                //销售发票
                that.bindBillModel2Table(data.InitBillList);
                //如果表格里面没有数据，就插入一行空的
                if (data.InitBillList.length == 0) {
                    //加入空行
                    that.insertRow();
                }
                //在表格下方显示差额
                InitBillAmountFor = data.InitBillAmountFor;
                //
                (currentType == ACCOUNT) ? $(originalDiv).show() : $(originalDiv).hide();
                //如果不是联系人维度的，则显示下方的统计
                if (currentType == ACCOUNT && InitBillAmountFor && InitBillAmountFor.length > 0) {
                    //
                    var sum = InitBillAmountFor.sum("MInitBalance");
                    //如果是员工的话，则不显示对于的初始化余额
                    that.showAccountBalanceInfo(sum);
                    //
                    that.showAmountForTotal(InitBillAmountFor, OrignalType);
                }
                //计算差额
                that.updateDifference(undefined, undefined);
                //
                that.resizeTableDiv();
                //
                $("body").unmask();
            };
            //绑定各种单据的数据到表格
            this.bindBillModel2Table = function (models) {
                //循环加入行
                for (var i = 0; i < models.length; i++) {
                    //
                    var model = models[i];
                    //绑定到表格的行
                    model = $.extend(true, {}, model, {
                        MContactID: model.MCheckGroupValueModel.MContactID,
                        MEmployeeID: model.MCheckGroupValueModel.MEmployeeID,
                        MExpItemID: model.MCheckGroupValueModel.MExpItemID,
                        MMerItemID: model.MCheckGroupValueModel.MMerItemID,
                        MPaItemID: model.MCheckGroupValueModel.MPaItemID,
                        MTrackItem1: model.MCheckGroupValueModel.MTrackItem1,
                        MTrackItem2: model.MCheckGroupValueModel.MTrackItem2,
                        MTrackItem3: model.MCheckGroupValueModel.MTrackItem3,
                        MTrackItem4: model.MCheckGroupValueModel.MTrackItem4,
                        MTrackItem5: model.MCheckGroupValueModel.MTrackItem5
                    });
                    //处理红字的问题
                    if (model.MType && model.MType.indexOf("Red") >= 0) {
                        //
                        model.MTaxTotalAmt = -1.0 * model.MTaxTotalAmt;
                        //
                        model.MTaxTotalAmtFor = -1.0 * model.MTaxTotalAmtFor;
                    }
                    //插入到表格
                    that.insertRow(null, model);
                }
            };
            //初始化表格里面的事件
            this.initTableEvent = function () {
                //
                $(newLineButton).off("click").on("click", function () {

                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    if (!that.checkAccountCreateBill()) {
                        return;
                    }
                    //
                    that.editIndex != null && $(tableBody).datagrid("endEdit", index);
                    //获取当前所在的行的下标
                    var index = that.getRowIndex(this);
                    //点击添加的时候
                    that.insertRow(index + 1);

                    if (that.editIndex != null && index < that.editIndex) {

                        that.editIndex++;
                    }
                });
                //
                $(removeLineButton).off("click").on("click", function () {

                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    if (!that.checkAccountCreateBill()) {
                        return;
                    }
                    //
                    var index = that.getRowIndex(this);
                    //点击添加的时候
                    that.deleteRow(index, $(this).attr("mid"), true);
                    //如果删除了当前行
                    if (index == that.editIndex) {
                        //则还是没有编辑的行
                        that.editIndex = null;
                    }
                    //
                    that.updateDifferenceWithEditRow();
                });
                //删除初始化单据的事件
                $(editBillButton).off("click").on("click", function () {

                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    if (!that.checkAccountCreateBill()) {
                        return;
                    }
                    //点击添加的时候
                    var mid = $(this).attr("mid");
                    //业务单据类型
                    var mtype = $(this).attr("mtype");
                    //银行账号
                    var bankID = $(this).attr("bankid");
                    //是否是初始化单据
                    var isInitBill = $(this).attr("isinit") == "1";
                    //如果是初始化单据
                    if (isInitBill) {
                        //
                        that.editInitBillItem(mtype, mid, bankID);
                    }
                    else {
                        //
                        that.editBillItem(mtype, mid, bankID);
                    }
                });
                //删除初始化单据的事件
                $(removeBillButton).off("click").on("click", function () {

                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                    if (!that.checkAccountCreateBill()) {
                        return;
                    }
                    //点击添加的时候
                    var mid = $(this).attr("mid");
                    //业务单据类型
                    var mtype = $(this).attr("mtype");
                    //
                    var index = that.getRowIndex(this);
                    //
                    var deleteCallback = function () {
                        //
                        $(tableBody).datagrid("deleteRow", index);
                        //如果删除了当前行
                        if (index == that.editIndex) {
                            //则还是没有编辑的行
                            that.editIndex = null;
                        }
                        else if (that.editIndex != null) {
                            //如果删除的行在编辑行上面，那么，编辑行指标要减一
                            if (index < that.editIndex) {

                                that.editIndex--;
                            }
                        }
                        //
                        !that.getRows().length ? that.insertRow() : "";
                        //
                        var baseBalance = $(initBalanceBaseInput).val();
                        //选中的联系人重现点击
                        that.updateTabValue();
                        //恢复用户输入的期初余额
                        $(initBalanceBaseInput).val(baseBalance);
                        //更新差额
                        that.updateDifferenceWithEditRow();
                    };
                    //提醒用户是否永久性删除初始化单据
                    mDialog.confirm(areYouSureToDeleteInitBillPermanently, function () {
                        //
                        mAjax.submit(removeInitBillUrl, { MID: mid, MType: mtype }, function (data) {
                            //
                            if (data.Success) {
                                //
                                mDialog.message(deleteSuccessfully);
                                //
                                deleteCallback();
                            }
                            else {
                                //提醒用户删除失败
                                mDialog.error(deleteFailed);
                            }
                        });
                    });

                });
            };
            //编辑初始化单据
            this.editInitBillItem = function (mtype, mid, bankID) {
                //弹框的url
                var url = "";
                //弹框的title
                var title = "";
                //针对于销售发票
                if (mtype == "Invoice_Sale") {
                    //
                    title = HtmlLang.Write(LangModule.IV, "InitSalesInvoice", "初始化销售单");
                    //
                    url = invoiceInitEditUrl + "/" + mid + "?showAdvance=false";
                }
                else if (mtype == "Invoice_Sale_Red") {
                    //编辑红字发票
                    title = HtmlLang.Write(LangModule.IV, "InitSalesCreditNote", "初始化红字销售单");
                    //
                    url = invoiceCreditNoteInitEditUrl + "/" + mid + "?showAdvance=false";
                }
                else if (mtype == "Invoice_Purchase") {
                    //编辑采购单
                    title = HtmlLang.Write(LangModule.IV, "InitBills", "初始化采购单");
                    //
                    url = purchaseInitEditUrl + "/" + mid + "?showAdvance=false";
                }
                else if (mtype == "Invoice_Purchase_Red") {
                    //编辑采购单
                    title = HtmlLang.Write(LangModule.IV, "InitRedBills", "初始化红字采购单");
                    //
                    url = purchaseInitCreditNoteEditUrl + "/" + mid + "?showAdvance=false";
                }
                else if (mtype.indexOf("Pay") == 0) {
                    //编辑付款单
                    title = HtmlLang.Write(LangModule.IV, "InitSpend", "初始化付款单");
                    //
                    url = paymentInitEditUrl + "/" + mid + "?showAdvance=false&showBnkAcct=true&acctid=" + bankID;
                }
                else if (mtype.indexOf("Receive") == 0) {
                    //编辑收款单
                    title = HtmlLang.Write(LangModule.IV, "InitReceive", "初始化收款单")
                    //
                    url = receiveInitEditUrl + "/" + mid + "?showAdvance=false&showBnkAcct=true&acctid=" + bankID;
                }
                else if (mtype == "Expense_Claims") {
                    //编辑收款单
                    title = HtmlLang.Write(LangModule.IV, "InitExpense", "初始化费用报销单")
                    //
                    url = expenseInitEditUrl + "/" + mid + "?showAdvance=false";
                }
                //弹框
                $.mDialog.show({
                    mTitle: title,
                    mWidth: mWidth,
                    mHeight: mHeight,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + url,
                    mCloseCallback: [that.billEditColseCallback]
                });
            };
            //单据编辑后返回时候需要刷新页面
            this.billEditColseCallback = function () {
                //更新各种联系人的数据
                //更新金额
                currentType == ACCOUNT && that.getInitDocumentBaseData(currentType, that.showInitDocumentBaseData);
                //tab页重新点击
                $(".current", currentType == ACCOUNT ? accountUl : billtypeUl).trigger("click");
            };
            //编辑普通的单据
            this.editBillItem = function (mtype, mid, bankID) {
                //弹框的url
                var url = "";
                //弹框的title
                var title = "";
                //针对于销售发票
                if (mtype == "Invoice_Sale") {
                    //
                    title = HtmlLang.Write(LangModule.IV, "InvoiceDetail", "销售单详情");
                    //
                    url = invoiceEditUrl + mid;
                }
                else if (mtype == "Invoice_Sale_Red") {
                    //
                    title = HtmlLang.Write(LangModule.IV, "InvoiceRedDetail", "红字销售单详情");
                    //
                    url = invoiceEditUrl + mid;
                }
                else if (mtype == "Invoice_Purchase") {
                    //编辑采购单
                    title = HtmlLang.Write(LangModule.IV, "PurchaseDetail", "采购单详情");
                    //
                    url = purchaseEditUrl + mid;
                }
                else if (mtype == "Invoice_Purchase_Red") {
                    //编辑采购单
                    title = HtmlLang.Write(LangModule.IV, "PurchaseRedDetail", "红字采购单详情");
                    //
                    url = purchaseEditUrl + mid;
                }
                else if (mtype.indexOf("Pay") == 0) {
                    //编辑付款单
                    title = HtmlLang.Write(LangModule.IV, "PaymentDetail", "付款单详情");
                    //
                    url = paymentEditUrl + mid;
                }
                else if (mtype.indexOf("Receive") == 0) {
                    //编辑收款单
                    title = HtmlLang.Write(LangModule.IV, "ReceiptDetail", "收款单详情");
                    //
                    url = receiveEditUrl + mid;
                }
                else if (mtype == "Expense_Claims") {
                    //编辑收款单
                    title = HtmlLang.Write(LangModule.IV, "ExpenseDetail", "费用报销单详情")
                    //
                    url = expenseEditUrl + mid;
                }
                //弹框
                $.mDialog.show({
                    mTitle: title,
                    mDrag: "mBoxTitle",
                    mWidth: mWidth,
                    mHeight: mHeight,
                    mShowbg: true,
                    mContent: "iframe:" + url,
                    mCloseCallback: [that.billEditColseCallback]
                });
            };
            //根据item获取所在行
            this.GetGridRowIndexByCellItem = function (table, item) {
                //
                var view = $(tableBody).datagrid("getPanel");
                //
                var table = $(".datagrid-btable", view);
                //获取里面的行
                var rows = $(".datagrid-row", table);
                //
                for (var i = 0; i < rows.length; i++) {
                    //
                    if ($.contains(rows.eq(i), item)) {
                        //
                        return i;
                    }
                }
            };
            //重现编号
            this.reorderTableRow = function () {
                //
                var tables = $(".datagrid-body").filter(function () {
                    return $(this).width() > 0;
                });
                //
                for (var i = 0; i < tables.length ; i++) {
                    //
                    var rows = tables.eq(i).filter(function () {
                        return $(this).width() > 0;
                    }).find(".datagrid-row:visible");
                    //
                    rows.each(function (i) {
                        $(this).attr("datagrid-row-index", i);
                        var tr_id = $(this).attr("id");
                        $(this).attr("id", tr_id.substr(0, tr_id.lastIndexOf('-') - 1) + i);
                    });
                }
            };
            //清空表格里面的数据
            this.clearTableRows = function (contactType) {
                //
                that.initTableData([], true, contactType);
            };
            //清空表头数据
            that.clearTableHeaderData = function () {
                //期初
                $(initBalanceBaseInput).numberbox("setValue", 0);
                //合计
                $(totalAmountInput).val("");
                //差额
                $(differenceAmountInput).val("");
                //如果已经初始化了
                if ($(accountCombo).attr("inited") == "1") {
                    //把内容清空
                    $(accountCombo).combobox("setValue", "");
                    //
                    $(accountCombo).combobox("setText", "");
                }
            };
            //清空合计的数据
            that.clearTotalData = function () {
                //清空本位币和外币的金额显示
                $("tr:not(.demo)", balanceInfoTable).remove();
                //
                $("tr:not(.demo)", originalBalanceInfoTable).remove();
            };
            //
            this.getRowIndex = function (obj) {
                //获取行
                return parseInt($(obj).closest(".datagrid-row").attr("datagrid-row-index"));
            };
            //获取里面所有的行
            this.getRows = function () {
                //
                return $(".datagrid-body").eq(0).filter(function () {
                    return $(this).width() > 0;
                }).find(".datagrid-row:visible");
            };
            //是否隐藏联系人
            this.shouldHideEmployeeExpItem = function () {
                return (currentType == BILLTYPE && currentBillType != expense && currentBillType != payment)
                || (currentType == ACCOUNT && (accountType == 0 || accountType == 4));
            }
            //是否隐藏联系人和商品项目
            this.shouldHideContactMerItem = function () {
                //只有币别类型以及费用报销单才隐藏
                return currentType == BILLTYPE && currentBillType == expense;
            }
            //根据单据类型过滤联系人类型
            this.getContactDataByBillType = function (data, type) {

                switch (type) {
                    case BillTypeListData[1][0].id:
                    case BillTypeListData[1][1].id:
                        return data.where('x.id == "Customer"');
                    case BillTypeListData[2][0].id:
                    case BillTypeListData[2][1].id:
                        return data.where('x.id == "Supplier"');
                    case BillTypeListData[4].id:
                        return data.where('x.id == "Supplier" || x.id == "Other"');
                    case BillTypeListData[3].id:
                        return data.where('x.id == "Customer" || x.id == "Other"');
                    default:
                        return data;
                }
                return data;
            }
            //根据科目获取科目的核算维度列
            //联系人和员工也都显示(是否必录根据科目来确定）
            //跟踪项必须要显示（是否必录根据科目来确定）
            //工资项目都不显示，
            //商品项目和费用项目也都显示（是否必录根据科目来确定），
            this.getAccountCheckTypeColumn = function () {
                //
                var account = AccountListData[accountType];
                //
                var checkGroupModel = currentType == BILLTYPE ? that.getCheckTypeModelByBillType() : account.MCheckGroupModel;
                //如果当前是单据维度，并且是费用报销单，那么联系人不需要显示
                var hideContactMerItem = that.shouldHideContactMerItem();
                //是否隐藏员工
                var hideEmployeeExpItem = that.shouldHideEmployeeExpItem();
                //
                var columns = [];
                //
                for (var i = 0; i < GLCheckType.CheckTypeList.length; i++) {
                    //
                    var value = checkGroupModel[GLCheckType.CheckTypeList[i].column];
                    //
                    var type = GLCheckType.CheckTypeList[i].type;
                    //费用报销单不显示联系人
                    if ((type == 0 && hideContactMerItem)
                        //除了付款单和费用报销单，其他的都隐藏员工
                        || (type == 1 && hideEmployeeExpItem)
                        //费用报销单不显示商品项目
                        || (type == 2 && hideContactMerItem)
                        //除了费用报销单和付款单，单据都不显示费用项目
                        || (type == 3 && hideEmployeeExpItem)
                        //工资项目统一不显示
                        || (type == 4)
                        ) {
                        //
                        continue;
                    }
                    //
                    var data = checkTypeDal.getCheckTypeData(GLCheckType.CheckTypeList[i].type);
                    //
                    if (!data.MCheckTypeName) {
                        //
                        continue;
                    }
                    //
                    var column = {};
                    var required = value == 2;
                    var field = GLCheckType.CheckTypeList[i].column;
                    //如果按照combox的显示
                    if ((data.MShowType == 0 || data.MShowType == 1)) {
                        //
                        column = {
                            field: field, width: 150, title: data.MCheckTypeName, align: 'left',
                            formatter: function (id, row, index, field) {
                                //
                                var type = checkTypeDal.getCheckTypeByColumnName(field);
                                //
                                return !id ? "" : checkTypeDal.filterCheckTypeData(type.type, id).text;
                            }
                        }

                        column.editor = that.getColumnEditor(GLCheckType.CheckTypeList[i], required, false, field);
                    }
                    else {
                        //
                        column = {
                            field: field, width: 150, title: data.MCheckTypeName, align: 'left',
                            formatter: function (id, row, index, field) {
                                //
                                var type = checkTypeDal.getCheckTypeByColumnName(field);
                                //
                                return !id ? "" : checkTypeDal.filterCheckTypeData(type.type, id).text;
                            }
                        }

                        column.editor = that.getColumnEditor(GLCheckType.CheckTypeList[i], required, true, field);
                    }
                    //
                    columns.push(column);
                }
                //
                return columns;
            };

            this.getColumnEditor = function (checkTypeData, required, isTree, field) {
                var checkTypeEnum = checkTypeData.data.MCheckType;
                var editor = {};
                var addOptions = that.getAddOptions(checkTypeEnum, field);

                if (isTree != undefined && !isTree) {
                    //不是树形结构
                    editor.type = 'addCombobox';
                    editor.options = {};
                    editor.options.type = addOptions;

                    if ($.isNumeric(checkTypeEnum) && checkTypeEnum > 4) {
                        var addTrackOptionsUrl = that.getAddTrackOptionUrl(checkTypeEnum);
                        editor.options.addOptions = {
                            //是否有基础资料编辑权限
                            hasPermission: true,
                            isReload: false,
                            url: addTrackOptionsUrl,
                            //关闭后的回调函数
                            callback: function () {
                                var newestData = checkTypeDal.getCheckTypeData(checkTypeEnum, null, true);
                                if (newestData) {
                                    var editor = that.getEditorByField(field);
                                    $(editor.target).combobox("loadData", newestData.MDataList);
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
                                var newestData = checkTypeDal.getCheckTypeData(checkTypeEnum, null, true);
                                if (newestData) {
                                    var editor = that.getEditorByField(field);
                                    $(editor.target).combobox("loadData", newestData.MDataList);
                                }
                            }
                        }
                    }

                    editor.options.dataOptions = {
                        textField: "text",
                        valueField: "id",
                        required: required,
                        srcRequired: required,
                        hideItemKey: "MIsActive",
                        hideItemValue: false,
                        data: checkTypeData.data.MDataList,
                        onSelect: function (data) {

                            var id = data.id;

                        },
                        onLoadSuccess: function () {
                            //if (loadSuccessEvent) { loadSuccessEvent() };
                        }
                    };

                    return editor;
                }

                if (isTree != undefined && isTree) {
                    editor.type = "addCombotree";
                    editor.options = {};
                    editor.options.addOptions = addOptions;
                    editor.options.dataOptions = {
                        textField: "text",
                        valueField: "id",
                        required: required,
                        srcRequired: required,
                        hideItemKey: "MIsActive",
                        hideItemValue: false,
                        data: checkTypeData.data.MDataList,
                        onBeforeSelect: function (node) {
                            //不是叶子节点不允许选择
                            if (!$(this).tree('isLeaf', node.target)) {
                                return false;
                            }

                            //联系人标题不允许选择
                            if (node.id == "Customer" || node.id == "Supplier" || node.id == "Other") {
                                return false;
                            }
                        },
                        onSelect: function (node) {

                            if (["Supplier", "Customer", "Other"].contains(node.parentId) && !that.isSettingValue) {
                                $(tableBody).datagrid('getRows')[that.editIndex].MContactType = node.parentId;
                            }

                        },
                        onLoadSuccess: function () {
                            //if (loadSuccessEvent) { loadSuccessEvent($(this)) };
                        }
                    }

                    return editor;
                }

                //默认就是combobox
                editor = {
                    type: 'combobox',
                    options: {
                        data: checkTypeData.data.MDataList,
                        hasDownArrow: true,
                        required: required,
                        srcRequired: required,
                        textField: "text",
                        valueField: "id",
                        groupField: checkTypeData.MShowType == 0 ? null : "parentId",
                        onSelect: function (row) {
                            //

                        },
                        onLoadSuccess: function () {
                        }
                    }
                }

                return editor;
            };

            this.getAddOptions = function (checkTypeEnum, field) {
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
                            var newestData = checkTypeDal.getCheckTypeData(checkTypeEnum, null, true);
                            if (newestData) {
                                var editor = that.getEditorByField(field);
                                $(editor.target).combotree("loadData", newestData.MDataList);
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
                            var newestData = checkTypeDal.getCheckTypeData(checkTypeEnum, null, true);
                            if (newestData) {
                                var editor = that.getEditorByField(field);
                                $(editor.target).combotree("loadData", newestData.MDataList);
                            }

                        }
                    }

                } else if (checkTypeEnum == "4") {
                    //工资项没有新增
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
            };

            //获取新增跟踪项的核算维度
            this.getAddTrackOptionUrl = function (checkTypeEnum) {

                var trackData = checkTypeDal.getCheckTypeData(checkTypeEnum);
                if (trackData == null) {
                    return "";
                }

                var trackId = trackData.MCheckTypeGroupID;
                var url = "/BD/Tracking/CategoryOptionEdit?trackId=" + trackId;

                return url;

            };

            this.getEditorByField = function (field) {
                var editor = $(tableBody).datagrid('getEditor', { index: that.editIndex, field: field });

                return editor;
            }

            //
            this.getCommonColumn = function () {
                //是否隐藏银行账号
                var isHideBank = (currentType == BILLTYPE) && (currentBillType == invoiceSale || currentBillType == invoicePurchase || currentBillType == expense)
                //是否隐藏number
                var isHideNumber = (currentType == BILLTYPE) && (currentBillType != invoiceSale);
                //是否隐藏到日期 收付款单，是不需要有到期日的
                var isHideDueDate = (currentType == BILLTYPE) && (currentBillType == payment || currentBillType == receive);
                //
                return [
                          {
                              field: 'MID', width: 80, title: operateLang, align: 'center', resizable: false, formatter: function (val, rec, rowIndex) {
                                  //如果是业务单据 必须是没有完成初始化的情况下
                                  var text = '<div class="list-item-action">';
                                  //
                                  if (!isInitOver) {
                                      //
                                      text += '<a href="javascript:void(0)" class=" m-icon-add-row ' + newLineButton.trimStart('.') + '" rowindex="' + rowIndex + '">&nbsp;</a>';
                                  }
                                  //编辑按钮一致需要
                                  if (rec.MID) {
                                      //而且可以编辑
                                      text += '<a href="javascript:void(0)" class="list-item-edit ' + editBillButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '" mtype="' + rec.MType + '" bankid="' + rec.MBankID + '" isinit="1">&nbsp;</a>';
                                  }
                                  //如果是初始化单据，就可以编辑删除
                                  if (rec.MIsInitBill && rec.MID && !isInitOver) {
                                      //
                                      text += '<a href="javascript:void(0)" class="list-item-del ' + removeBillButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '" mtype="' + rec.MType + '">&nbsp;</a>';
                                  }
                                  else if (!rec.MID && !isInitOver) {
                                      //如果不是初始化单据也不是业务单据，就表示是用户临时添加的，可以减号减掉
                                      text += '<a href="javascript:void(0)" class="m-icon-remove-row ' + removeLineButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MID + '">&nbsp;</a>';
                                  }
                                  //
                                  text += '</div>';
                                  //
                                  return text;
                              }
                          },
                          {
                              field: 'MEntryID', title: '', align: 'center', hidden: true
                          },
                           {
                               field: 'MContactType', title: '', align: 'center', hidden: true
                           },
                          {
                              field: 'MIsInitBill', title: '', align: 'center', hidden: true
                          },
                          {
                              field: 'MCurrentAccountCode', textField: 'MCurrentAccountName', width: 90, title: currenctAccountLang, align: 'left', editor: {
                                  type: 'combobox',
                                  options: {
                                      valueField: 'MCode',
                                      textField: 'MFullName',
                                      required: true,
                                      data: that.getAccountSubData(),
                                      onSelect: function (data) {

                                          data && data.MCode && that.updateDocCurrentAccountCode(data.MCode);
                                          //更新行里面的必选和非必选
                                          data && data.MCode && that.changeAccountType(data.MCode);
                                      }
                                  }
                              },
                              formatter: function (value, row) {
                                  //
                                  if (value) {
                                      //
                                      var v = that.getAccountSubData().where("x.MCode == '" + value + "'");
                                      //
                                      return v.length > 0 ? v[0].MFullName : "";
                                  }
                                  //
                                  return "";
                              }
                          },
                            {
                                field: 'MType', width: 80, title: mTypeLang, align: 'left', resizable: false, editor: {
                                    type: 'combobox',
                                    options: {
                                        valueField: 'id',
                                        textField: 'value',
                                        data: that.getBillTypeList(),
                                        required: true,
                                        onSelect: function (data) {
                                            that.changeMType(data.id);
                                        }
                                    }
                                }, formatter: function (value, rec) {
                                    var result = "";
                                    switch (value) {
                                        case "Invoice_Sale":
                                            result = BillTypeListData[1][0].value;
                                            break;
                                        case "Invoice_Sale_Red":
                                            result = BillTypeListData[1][1].value;
                                            break;
                                        case "Invoice_Purchase":
                                            result = BillTypeListData[2][0].value;
                                            break;
                                        case "Invoice_Purchase_Red":
                                            result = BillTypeListData[2][1].value;
                                            break;
                                        case "Receive_Sale":
                                        case "Receive_Other":
                                        case "Receive_SaleReturn":
                                        case "Receive_Adjustment":
                                        case "Receive_BankFee":
                                            result = BillTypeListData[3].value;
                                            break;
                                        case "Pay_Purchase":
                                        case "Pay_Other":
                                        case "Pay_PurReturn":
                                        case "Pay_BankFee":
                                        case "Pay_OtherReturn":
                                        case "Pay_Adjustment":
                                            result = BillTypeListData[4].value;
                                            break;
                                        case "Expense_Claims":
                                            result = BillTypeListData[5].value;
                                            break;
                                    }
                                    return result;
                                }
                            },
                            {
                                field: 'MBankID', textField: 'MBankName', hidden: isHideBank, width: 80, title: bankLang, align: 'left', editor: {
                                    type: 'combobox',
                                    options: {
                                        valueField: 'MBankID',
                                        textField: 'MBankName',
                                        required: true && !isHideBank,
                                        data: BankAccountListData,
                                        onSelect: function (data) {
                                            //
                                            that.changeBankAccount(data.MCyID);
                                        },
                                        formatter: function (data) {
                                            //
                                            return data.MBankName + "(" + data.MCyID + ")";
                                        }
                                    }
                                },
                                formatter: function (value, row) {
                                    //
                                    if (value) {
                                        //
                                        var bank = BankAccountListData.where("x.MBankID =='" + value + "'")[0];
                                        //
                                        return bank.MBankName + "(" + bank.MCyID + ")";
                                    }
                                    else {
                                        return "";
                                    }
                                }
                            },
                          {
                              field: 'MNumber', width: 70, hidden: isHideNumber, title: numberLang, align: 'left', editor: {
                                  type: 'validatebox',
                                  options: {
                                      required: true && !isHideNumber
                                  }
                              },
                              formatter: function (value, row) {
                                  //
                                  if (row.MType == "Invoice_Purchase" || row.MType == "Invoice_Purchase_Red") {
                                      return "";
                                  }

                                  return value;
                              }
                          },
                          {
                              field: 'MReference', width: 100, title: referenceLang, align: 'left', editor: {
                                  type: 'validatebox',
                                  options: {
                                      required: true,
                                      height: 26
                                  }
                              }
                          },

                          {
                              field: 'MBizDate', width: 80, title: bizDateLang, align: 'left', resizable: false, editor: {
                                  type: 'datebox',
                                  options: {
                                      required: true,
                                      validType: ["maxDate['" + mDate.format(MaxBillDate) + "']"],
                                      current: mDate.parse(BeginDate)
                                  }
                              }, formatter: function (value) {
                                  return $.mDate.format(value);
                              }
                          },
                          {
                              field: 'MDueDate', width: 80, hidden: isHideDueDate, title: dueDateLang, align: 'left', resizable: false, editor: {
                                  type: 'datebox',
                                  options: {
                                      required: true && !isHideDueDate,
                                      current: mDate.parse(BeginDate)
                                  }
                              }, formatter: function (value) {
                                  return $.mDate.format(value);
                              }
                          },
                          {
                              field: 'MCyID', width: 60, title: currencyLang, align: 'left', resizable: false, editor: {
                                  type: 'addCombobox',
                                  options: {
                                      type: "currency",
                                      dataOptions:
                                          {
                                              required: true,
                                              onSelect: function (data) {
                                                  that.changeMCyID(data.MCurrencyID);
                                              }
                                          },
                                      addOptions: {
                                          //是否有联系人编辑权限
                                          hasPermission: true
                                      },
                                  }
                              }
                          },
                          {
                              field: 'MTaxTotalAmt', width: 80, title: amountLang, align: 'right', editor: {
                                  type: 'numberbox',
                                  options: {
                                      required: true,
                                      precision: 2,
                                      validType: ['notEqual[0]']
                                  }
                              },
                              formatter: function (value) {
                                  return !value ? "" : parseFloat(value).toFixed(2);
                              }
                          },
                          {
                              field: 'MTaxTotalAmtFor', width: 80, title: forCurrencyLang, align: 'center', editor: {
                                  type: 'numberbox',
                                  options: {
                                      required: true,
                                      precision: 2,
                                      validType: ['notEqual[0]']
                                  }
                              },
                              formatter: function (value, rec) {
                                  //如果是本位币，则不要显示
                                  if (rec.MCyID == baseCyID) {
                                      return "";
                                  }
                                  else {
                                      return !value ? "" : parseFloat(value).toFixed(2);
                                  }
                              }
                          }
                ];
            };
            //加载表格数据 empty表示是否保持空的，不加入空行
            this.initTableData = function (data, empty) {
                //数据
                var data = data || [that.getEmptyRow()];
                //编辑下标
                that.editIndex = null;
                //编辑的行
                that.editRow = null;
                //
                var checkTypeColumn = that.getAccountCheckTypeColumn();
                //
                var commonColumn = that.getCommonColumn();
                //
                tableColumns = commonColumn.concat(checkTypeColumn);
                //初始化数据
                $(tableBody).datagrid({
                    data: data,
                    pagination: false,
                    width: $(contentRight).width() - 10,
                    height: $(gridTableDiv).height() - 3,
                    scrollY: true,
                    frozenColumns: [commonColumn],
                    columns: [checkTypeColumn],
                    onResizeColumn: function (field, width) {
                        that.resizeFrozenColumn(field, width);
                    },
                    onLoadSuccess: function () {
                        //如果没有数据则添加一个空行，供用户编辑
                        if ((!data || data.length == 0) && !empty) {
                            //添加一个空行，用于用户编辑
                            that.insertRow();
                        }
                        //
                        that.initTableEvent();
                    },
                    onClickRow: function (rowIndex, rowData, event) {
                        //如果已经完成了初始化，则直接提醒用户
                        if (isInitOver) {
                            //
                            mDialog.alert(balanceInitIsOverBillUpdateIsForbidden);
                            //
                            return false;
                        }
                        //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                        if (!that.checkAccountCreateBill()) {
                            return;
                        }
                        //
                        if (rowData.MID) {
                            //如果已经编辑好的，则不能再编辑了
                            return false;
                        }
                        //如果点击的是添加和删除按钮，则不进行编辑
                        if (!$(event.srcElement || event.target).is("a")) {
                            //
                            $(this).datagrid("unselectAll");
                            //如果校验不通过
                            if (!that.validateEditRow()) {
                                //直接返回
                                return false;
                            }
                            //结束编辑
                            $(this).datagrid("beginEdit", rowIndex);
                            //
                            that.handleNewSavedBillEditor(rowData.MID);
                            //如果是已经保存的初始化单据，则只能编辑往来科目
                            if (!rowData.MID) {
                                //
                                that.bindTableRowAmountInputEvent(rowIndex);
                                //找到各个编辑的框
                                that.handleMTypeMCyID(true);
                            }
                        }
                    },
                    onDblClickRow: function (rowIndex, rowData) {
                    },
                    onBeforeEdit: function (rowIndex, rowData) {
                        //
                        if (that.editIndex != null) {
                            //结束编辑
                            $(this).datagrid("endEdit", that.editIndex);
                        }
                        that.editIndex = rowIndex;
                        //
                        that.shrink(-1);
                    },
                    onAfterEdit: function (rowIndex, rowData, changes) {
                        //
                        that.initTableEvent();
                        //结束编辑
                        that.editIndex = null;
                        //
                        that.shrink(shrinkStatus);
                    }
                });
            };

            this.resizeFrozenColumn = function (field, width) {
                /*
                    bug 20064，
                    原因：easyui datagrid 控件的bug，由于它将固定列和非固定列做两个表格来处理，并且滚动条只显示在非固定列的表格上
                    这个bug是用户将非固定列表宽度拖拽长度过宽，导致将非固定列表格挤出去了，使得滚动条看不到
                    解决方案：easyui datagrid 官方没找到相应的解决方案。 所以通过以下方案解决，以后如果有发现更好的方式可以直接替换。 
                    1.固定列里面指定6个字段可以拖拽（和李丹确认过）。往来科目，银行名称，单据号，备注，金额，原币金额
                    2.限定 6个字段拖拽后的总宽带 < 表格总宽度 - 380(其他固定列总宽带和) - 180（预留给右边非固定列表格）
                    实现逻辑：
                    1.如果拖拽后的 6个字段总宽带 超过限制。将当前拖拽的字段的宽带设置为它可以有的最大宽度。
                    2.easyui datagrid 是通过动态生成 css，以每个字段列的class 作为选择器来指定宽带,
                    所以，这边也通过每个列的class 作为选择器来指定宽带
                    */

                //只处理这6个字段
                if (field === "MCurrentAccountCode" || field === "MBankID" || field === "MNumber" || field === "MReference" || field === "MTaxTotalAmt" || field === "MTaxTotalAmtFor")
                {
                    //找到每个字段的 class,然后取宽带（不要用getColumnOption 里面的 width 去取，拖拽时它是不准确的）
                    var MCurrentAccountCodeWidth = $('.' + $(tableBody).datagrid("getColumnOption", "MCurrentAccountCode").cellClass).width();
                    var MBankIDWidth = $('.' + $(tableBody).datagrid("getColumnOption", "MBankID").cellClass).width();
                    var MNumberWidth = $('.' + $(tableBody).datagrid("getColumnOption", "MNumber").cellClass).width();
                    var MReferenceWidth = $('.' + $(tableBody).datagrid("getColumnOption", "MReference").cellClass).width();
                    var MTaxTotalAmtWidth = $('.' + $(tableBody).datagrid("getColumnOption", "MTaxTotalAmt").cellClass).width();
                    var MTaxTotalAmtForWidth = $('.' + $(tableBody).datagrid("getColumnOption", "MTaxTotalAmtFor").cellClass).width();

                    //取6个字段可以有的最大宽带
                    var maxWidth = $(gridTableDiv).width() - 380 - 180;

                    if (MCurrentAccountCodeWidth + MBankIDWidth + MNumberWidth + MReferenceWidth + MTaxTotalAmtWidth + MTaxTotalAmtForWidth > maxWidth) {
                        //取当前拖拽的字段它可以有的最大宽度
                        if (field === "MCurrentAccountCode") {
                            width = maxWidth - (MBankIDWidth + MNumberWidth + MReferenceWidth + MTaxTotalAmtWidth + MTaxTotalAmtForWidth);
                        }
                        else if (field === "MBankID") {
                            width = maxWidth - (MCurrentAccountCodeWidth + MNumberWidth + MReferenceWidth + MTaxTotalAmtWidth + MTaxTotalAmtForWidth);
                        }
                        else if (field === "MNumber") {
                            width = maxWidth - (MCurrentAccountCodeWidth + MBankIDWidth + MReferenceWidth + MTaxTotalAmtWidth + MTaxTotalAmtForWidth);
                        }
                        else if (field === "MReference") {
                            width = maxWidth - (MCurrentAccountCodeWidth + MBankIDWidth + MNumberWidth + MTaxTotalAmtWidth + MTaxTotalAmtForWidth);
                        }
                        else if (field === "MTaxTotalAmt") {
                            width = maxWidth - (MCurrentAccountCodeWidth + MBankIDWidth + MNumberWidth + MReferenceWidth + MTaxTotalAmtForWidth);
                        }
                        else if (field === "MTaxTotalAmtFor") {
                            width = maxWidth - (MCurrentAccountCodeWidth + MBankIDWidth + MNumberWidth + MReferenceWidth + MTaxTotalAmtWidth);
                        }
                    }
                    //通过每个列的class 作为选择器来指定宽带
                    $('.' + $(tableBody).datagrid("getColumnOption", field).cellClass).width(width);
                }
                
            };
            //
            this.checkAccountCreateBill = function () {
                //如果是科目维度，则需要判断科目是否勾选了生成业务单据按钮
                if (currentType == ACCOUNT) {
                    //获取当前科目
                    var account = AccountListData[accountType];
                    //
                    if (!account.MCreateInitBill) {
                        //
                        mDialog.alert(cannotEditAccountInitBill);
                        //
                        return false;
                    }
                }
                return true;
            }
            //处理不同单据类型的时候，银行账户不可填，本位币的时候外币金额不可填的事件
            this.handleMTypeMCyID = function (keepSrcValue) {
                //
                var editor = that.getRowEditor();
                //
                if (that.editIndex != null) {
                    //如果输入类型是销售单等，则银行账号不可选择
                    that.changeMType(editor.MType.getValue());
                    //如果外币输入框的内容是本位币，则外币金额输入框不可编辑
                    that.changeMCyID(editor.MCyID.getValue(), keepSrcValue);
                }
            };
            //
            this.getRowEditor = function () {
                //
                var editor = null;
                //
                //禁用所有的输入框，除了往来科目输入框
                if (that.editIndex != null) {
                    //找到各个编辑的框
                    var editors = $(tableBody).datagrid('getEditors', that.editIndex);
                    //
                    editor = {};
                    //
                    for (var i = 0; i < tableColumns.length; i++) {
                        //
                        var column = tableColumns[i];
                        //
                        if (!column.hidden && column.editor) {
                            //
                            editor[column.field] = {
                                column: column,
                                type: column.editor.type.replace("addC", "c"),
                                target: $(editors.where("x.field =='" + column.field + "'")[0].target),
                                textbox: function () {
                                    //
                                    return (this.type == "numberbox" || this.type == "validatebox") ? this.target : this.target[this.type]("textbox");
                                },
                                disable: function () {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    //
                                    if (this.type == "numberbox" || this.type == "validatebox") {
                                        //
                                        this.target.attr("disabled", "disabled");
                                        //
                                        this.target.validatebox("disableValidation");
                                    }
                                    else {
                                        //
                                        this.target[this.type]("disable");
                                        //
                                        this.target[this.type]("textbox").validatebox("disableValidation");
                                    }
                                },
                                enable: function () {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    //
                                    if (this.type == "numberbox" || this.type == "validatebox") {
                                        //
                                        this.target.removeAttr("disabled");
                                        //
                                        this.target.validatebox("enableValidation");
                                    }
                                    else {
                                        //
                                        this.target[this.type]("enable");
                                        //
                                        this.target[this.type]("textbox").validatebox("enableValidation");
                                    }
                                },
                                validate: function () {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    that.isSettingValue = true;
                                    //先设置一下
                                    this.setValue(this.getValue());

                                    that.isSettingValue = false;

                                    //
                                    if (!this.getValue()) {
                                        //
                                        this.setText("");
                                    }
                                    //
                                    if (this.type == "numberbox" || this.type == "validatebox") {
                                        //
                                        return this.target.validatebox("isValid");
                                    }
                                    else {
                                        //
                                        return this.target[this.type]("isValid");
                                    }
                                },
                                getValue: function () {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    if (this.type == "validatebox") {
                                        return this.target.val();
                                    }
                                    //
                                    return this.target[this.type]("getValue");
                                },
                                setValue: function (value) {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    if (this.type == "validatebox") {
                                        //
                                        return this.target.val(value);
                                    }
                                    //
                                    this.target[this.type]("setValue", value);
                                },
                                setText: function (text) {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    if (this.type == "numberbox" || this.type == "validatebox") {
                                        //
                                        return this.target.val(text);
                                    }
                                    else {
                                        //
                                        this.target[this.type]("setText", text);
                                    }
                                },
                                //设置为必录的
                                setRequired: function (req) {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    //
                                    var value = this.getValue();
                                    //
                                    if (this.type == "numberbox" || this.type == "validatebox") {
                                        //
                                        this.target.validatebox({ required: req || this.target[this.type]("options").srcRequired });
                                    }
                                    else {

                                        var data = _.clone(this.getData());
                                        //
                                        this.target[this.type]({ required: req || this.target[this.type]("options").srcRequired });
                                        this.target[this.type]("resize", this.column.width);

                                        this.loadData(data);
                                    }
                                    //
                                    if (value) {
                                        this.setValue(value);
                                    }

                                },
                                loadData: function (data) {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    //
                                    this.target[this.type]("loadData", data);
                                },
                                getData: function () {
                                    if (this.type == "combobox") {
                                        return this.target.combobox("getData");
                                    }
                                    else if (this.type == "combotree") {
                                        return this.target.combo("options").data;
                                    }
                                },
                                reload: function () {
                                    //
                                    if (!this.target || this.target.length == 0) {
                                        //
                                        return;
                                    }
                                    //
                                    this.target[this.type]("reload");
                                }
                            }
                        }
                    }
                }
                //
                return editor;
            }
            //对于已经保存的初始化单据，只能修改往来科目
            this.handleNewSavedBillEditor = function (mid) {
                //禁用所有的输入框，除了往来科目输入框
                if (that.editIndex != null) {
                    //找到各个编辑的框
                    var editor = that.getRowEditor();
                    //如果不是新增的话，则需要做禁用
                    if (mid) {
                        //
                        editor.MType.disable();
                        //
                        editor.MBankID.disable();;

                        //
                        editor.MNumber.disable();;

                        //
                        editor.MReference.disable();

                        //
                        editor.MBizDate.disable();;

                        //
                        editor.MDueDate.disable();;

                        //
                        editor.MCyID.disable();;

                        //如果是本位币，则外币不可输入
                        editor.MTaxTotalAmtFor.disable();;

                        //如果是本位币，则外币不可输入
                        editor.MTaxTotalAmt.disable();;
                    }
                    else {
                        //如果是新增的话，科目维度的话，只能选对应的科目，单据类型科目可以任选
                        if (currentType == ACCOUNT) {
                            //
                            editor.MCurrentAccountCode.disable();;
                        }
                        else {
                            //
                            editor.MCurrentAccountCode.loadData(that.getAccountSubData());
                            //如果是 收付款单，费用报销单，则单据类型不可以选择
                            if (currentBillType == payment || currentBillType == receive || currentBillType == expense) {
                                //
                                editor.MType.disable();
                            }
                        }
                        //
                        editor.MCurrentAccountCode.reload();
                    }
                }
            };
            //获取银行账号的信息
            this.getBankAccountData = function () {
                //
                mAjax.post(getBankAccountUrl, {}, function (data) {
                    //账户信息
                    if (data) {
                        //保存到
                        BankAccountListData = data;
                        //
                        for (var i = 0; i < BankAccountListData.length ; i++) {
                            //
                            BankAccountListData[i].MBankID = BankAccountListData[i].MItemID;
                        }
                    }
                }, false, false, false)
            };
            //切换银行账号
            this.changeBankAccount = function (cyID) {
                //
                if (cyID) {
                    //
                    var editor = that.getRowEditor();
                    //不可以选择
                    editor.MCyID.setValue(cyID);
                    //
                    editor.MCyID.disable();
                    //
                    that.handleMTypeMCyID();
                    //
                    that.updateDifferenceWithEditRow();
                }
            };
            //更换科目维度，需要设置哪些为必录的哪些为选录的
            this.changeAccountType = function (code) {
                //
                var account = that.getAccountDataByCode(code);
                //
                var checkGroupModel = account.MCheckGroupModel;
                //
                for (var i = 0; i < GLCheckType.CheckTypeList.length; i++) {
                    //
                    var value = checkGroupModel[GLCheckType.CheckTypeList[i].column];
                    //
                    var type = GLCheckType.CheckTypeList[i].type;
                    //
                    var editors = that.getRowEditor();
                    //整片没有工资项目，所以不需要对工资项目进行设置
                    if (type == 4 || !editors[GLCheckType.CheckTypeList[i].column]) continue;
                    //
                    if (value == 2) {
                        //
                        editors[GLCheckType.CheckTypeList[i].column].setRequired(true);
                    }
                    else {
                        //
                        editors[GLCheckType.CheckTypeList[i].column].setRequired(false);
                    }
                }
            }
            //清空某个格子的数据
            this.changeMType = function (mtype) {
                //设置为空
                if (mtype) {
                    //
                    var editor = that.getRowEditor();
                    //
                    that.updateDifferenceWithEditRow();
                    //只有是 收付款单的时候，才能选择银行账户
                    if (mtype == BillTypeListData[3].id || mtype == BillTypeListData[4].id) {
                        //选择框可以选择
                        editor.MBankID && editor.MBankID.enable();
                        //
                        editor.MDueDate && editor.MDueDate.setValue("");
                        //
                        editor.MDueDate && editor.MDueDate.disable();
                    }
                    else {
                        //不可以选择
                        editor.MBankID && editor.MBankID.setValue("");
                        //不可以选择
                        editor.MBankID && editor.MBankID.setText("");
                        //可以选择
                        editor.MBankID && editor.MBankID.disable();
                        //可以选择
                        editor.MBankID && editor.MBankID.setValue("");
                        //
                        editor.MDueDate && editor.MDueDate.enable();
                    }
                    //只有销售单的发票号是必填的
                    if (mtype == BillTypeListData[1][0].id || mtype == BillTypeListData[1][1].id) {
                        //
                        editor.MNumber && editor.MNumber.enable();
                        //联系人必录
                    }
                    else {
                        //
                        editor.MNumber && editor.MNumber.disable();
                        //
                        editor.MNumber && editor.MNumber.setValue("");
                    }
                    //销售单，采购单、付款单联系人必录
                    if (mtype == BillTypeListData[1][0].id
                        || mtype == BillTypeListData[1][1].id
                        || mtype == BillTypeListData[2][0].id
                        || mtype == BillTypeListData[2][1].id) {
                        //联系人必录
                        editor.MContactID && editor.MContactID.setRequired(true);
                    }
                    else {
                        editor.MContactID && editor.MContactID.setRequired(false);
                    }
                    //费用报销单的员工是必录的
                    if (mtype == BillTypeListData[5].id) {
                        //联系人必录
                        editor.MEmployeeID && editor.MEmployeeID.setRequired(true);
                        //费用项目也是必录的
                        editor.MExpItemID && editor.MExpItemID.setRequired(true);
                        //不需要录入联系人
                        editor.MContactID && editor.MContactID.setRequired(false);
                        //
                        editor.MContactID && editor.MContactID.disable();
                        //
                        editor.MContactID && editor.MContactID.setValue("");
                        //商品项目也是不需要录入的
                        editor.MMerItemID && editor.MMerItemID.disable();
                        //商品项目也是不需要录入的
                        editor.MMerItemID && editor.MMerItemID.setValue("");
                    }
                    else {
                        //
                        editor.MEmployeeID && editor.MEmployeeID.setRequired(false);
                        //费用项目不是必录的
                        editor.MExpItemID && editor.MExpItemID.setRequired(false);
                        //不需要录入联系人
                        //editor.MContactID && editor.MContactID.setRequired(false);
                        //
                        editor.MContactID && editor.MContactID.enable();
                        //不需要录入联系人
                        editor.MMerItemID && editor.MMerItemID.setRequired(false);
                        //
                        editor.MMerItemID && editor.MMerItemID.enable();

                        editor.MContactID.loadData(that.getContactDataByBillType(checkTypeDal.getCheckTypeData(0).MDataList, mtype));

                        editor.MContactID.setValue(editor.MContactID.getValue());
                    }
                    //只有付款单和费用报销单才可以选择员工以及费用项目
                    if (mtype == BillTypeListData[5].id || mtype == BillTypeListData[4].id) {
                        //可以选择
                        editor.MEmployeeID && editor.MEmployeeID.enable();
                        //可以选择
                        editor.MExpItemID && editor.MExpItemID.enable();
                    }
                    else {
                        //不可以选择
                        editor.MEmployeeID && editor.MEmployeeID.setValue("");
                        //不可以选择
                        editor.MEmployeeID && editor.MEmployeeID.setText("");
                        //可以选择
                        editor.MEmployeeID && editor.MEmployeeID.disable();
                        //可以选择
                        editor.MEmployeeID && editor.MEmployeeID.setValue("");

                        //不可以选择
                        editor.MExpItemID && editor.MExpItemID.setValue("");
                        //不可以选择
                        editor.MExpItemID && editor.MExpItemID.setText("");
                        //可以选择
                        editor.MExpItemID && editor.MExpItemID.disable();
                        //可以选择
                        editor.MExpItemID && editor.MExpItemID.setValue("");
                    }
                }
            };
            //切换币种
            this.changeMCyID = function (type, keepSrcValue) {
                //找到各个编辑的框
                var editor = that.getRowEditor();
                //
                var isBase = type == baseCyID;
                //
                if (isBase) {
                    //
                    editor.MTaxTotalAmtFor.setValue("");
                    //
                    editor.MTaxTotalAmtFor.disable();
                }
                else {
                    //
                    editor.MTaxTotalAmtFor.setValue(keepSrcValue ? editor.MTaxTotalAmtFor.getValue() : 0);
                    //
                    editor.MTaxTotalAmtFor.enable();
                }
                //
                that.updateDifferenceWithEditRow(null, null, 0);
            };
            //绑定输入框里面的keyup事件
            this.bindTableRowAmountInputEvent = function () {
                //找到各个编辑的框
                var editor = that.getRowEditor();
                //
                var fields = Object.keys(editor);
                //
                for (var i = 0; i < fields.length ; i++) {
                    //
                    editor[fields[i]].textbox().off("keyup.id").on("keyup.id", function (e) {
                        //
                        if (e.keyCode == 13) {
                            //
                            that.endEditAndGoToNextRow();
                        }
                    });
                }
                //
                editor.MTaxTotalAmt.textbox().off("keyup.id").on("keyup.id", function (e) {
                    //
                    if (e.keyCode == 13) {
                        //
                        $(this).numberbox("setValue", +$(this).val());
                        //
                        that.endEditAndGoToNextRow();
                    }
                    else {
                        //如果是金额输入框则需要更新差额
                        that.updateDifferenceWithEditRow(undefined, $(this).val());
                    }
                });
                //
                editor.MTaxTotalAmtFor.textbox().off("keyup.id").on("keyup.id", function (e) {
                    //
                    if (e.keyCode == 13) {
                        //
                        $(this).numberbox("setValue", +$(this).val());
                        //
                        that.endEditAndGoToNextRow();
                    }
                    else {
                        //如果是金额输入框则需要更新差额
                        that.updateDifferenceWithEditRow(undefined, undefined, $(this).val());
                    }
                });

            };
            //校验某一行是否合法
            this.validateEditRow = function () {
                //
                if (that.editIndex != null) {
                    //
                    var editor = that.getRowEditor();
                    //
                    var fields = Object.keys(editor);
                    //
                    for (var i = 0; i < fields.length ; i++) {
                        //
                        if (!editor[fields[i]].validate()) {
                            return false;
                        }
                    }
                }
                return true;
            };
            //获取用户对核算维度的筛选
            this.getQueryData = function () {
                //
                var titles = $(checkTypeList).find(".panel").find(".panel-title");
                //获取当前的科目
                var account = currentType == ACCOUNT ? AccountListData[accountType] : {};
                //
                var query = {
                    MTypeList: that.getIntBillTypeList(),
                    MCurrentAccountCode: account.MCode,
                    MAccountID: account.MItemID
                };
                //
                var checkGroupValueModel = {};
                //
                var hasValue = false;
                //
                for (var i = 0; i < titles.length ; i++) {
                    //
                    var id = titles.eq(i).attr("id");
                    //
                    if (id) {
                        //
                        hasValue = true;
                        //
                        checkGroupValueModel[titles.eq(i).parent().next("div").attr("column")] = id;
                    }
                }
                //
                query.MCheckGroupValueModel = hasValue ? checkGroupValueModel : null;
                //
                return query;
            };
            //获取联系人的信息
            this.getInitDocumentData = function (callback) {
                //
                var query = that.getQueryData();
                //直接请求
                $.mAjax.post(getInitDocumnetUrl, { query: query }, function (data) {
                    //
                    if (data) {
                        //
                        $.isFunction(callback) && callback(data);
                    }
                }, "", true);
            };
            //获取总的数据
            this.getInitDocumentBaseData = function (type, callback) {
                //
                type = type == undefined ? currentType : type;
                //直接请求
                $.mAjax.post(getInitDocumentBaseDataUrl, { type: type }, function (data) {
                    //
                    if (data) {
                        //
                        $.isFunction(callback) && callback(data, type);
                    }
                }, "", true);
            };
            //展示总得数据
            this.showInitDocumentBaseData = function (data, type) {
                //
                type = type == undefined ? currentType : type;
                //
                if (type == BILLTYPE) {
                    //先清空
                    that.resetTabData();
                    //显示各种科目的数据
                    for (var i = 0 ; i < data.length ; i++) {
                        //
                        var li = $("li[mtype='" + data[i].MName + "']", billtypeUl);
                        //把后台传过来的 期初 借方 贷方都保存起来
                        var balances = data[i].MValue.split(',');
                        //
                        $("div.title", li).text(mMath.toMoneyFormat(balances[0]));
                    }
                }
            };
            //将表头的金额都设置为0
            this.resetTabData = function () {
                //联系人的
                $("div.title", $("li[mtype]", billtypeUl)).text("0.00");
            };
            //
            this.getEmptyRow = function () {
                //空行的数据
                var emptyRow = {
                    MID: "",
                    MEntryID: "",
                    MType: currentType == ACCOUNT ? "" : BillTypeListData[currentBillType + 1].id,
                    MCurrentAccountCode: currentType == ACCOUNT ? AccountListData[accountType].MCode : "",
                    MBankID: "",
                    MNumber: "",
                    MReference: "",
                    MContactID: "",
                    MContactType: "",
                    MEmployeeID: "",
                    MMerItemID: "",
                    MExpItemID: "",
                    MTrackItem1: "",
                    MTrackItem2: "",
                    MTrackItem3: "",
                    MTrackItem4: "",
                    MTrackItem5: "",
                    MBizDate: mDate.parse(MaxBillDate).addDays(-1),
                    MDueDate: "",
                    MTaxTotalAmt: "",
                    MCyID: "",
                    MTaxTotalAmtFor: ""
                };
                //
                return emptyRow;
            }
            //插入一行
            this.insertRow = function (index, row) {
                //
                row = row || that.getEmptyRow();
                //对于空行
                row = row || $.extend(true, {}, emptyRow);
                //插入到数据中
                $(tableBody).datagrid("insertRow", { index: index, row: row });
                //重新编号
                that.reorderTableRow();
                //
                that.initTableEvent();
                //
                that.resizeTableDiv();
                //
                that.resizeTable();
            };
            //删除一行
            this.deleteRow = function (index, mid, force) {
                //
                var rowLength = that.getRows().length;
                //如果只有一行了，就不然删除
                var row = that.getRowByMID(mid);
                //如果有这一行但是不是初始化单据，或者没有这一行
                if (!row || (row && force)) {
                    //
                    $(tableBody).datagrid("deleteRow", index);
                    //重新编号
                    that.reorderTableRow();
                    //
                    that.initTableEvent();
                    //
                    that.resizeTableDiv();
                    //
                    that.resizeTable();
                }
                //
                rowLength == 1 ? that.insertRow() : "";
            };
            //获取所有有效的行
            this.getGridRowsWithData = function (filterEditRow) {
                //
                var rows = $(tableBody).datagrid("getRows");
                //如果需要剔除编辑的行，则作如下处理
                if (filterEditRow && that.editIndex != null) {
                    //
                    var newRows = [];
                    //
                    for (var i = 0; i < rows.length ; i++) {
                        //
                        if (i != that.editIndex) {
                            //
                            newRows.push(rows[i]);
                        }
                    }
                    //
                    rows = newRows;
                }
                //返回
                var rows = that.filterRowByBillType(rows.where("!!x.MType && !!x.MCurrentAccountCode && !!x.MCyID"));
                //
                return rows;
            };
            //获取有意义的行，根据不同类型的单据，对不同单元格有不同的要求
            this.filterRowByBillType = function (rows) {
                //
                var newRows = [];
                //
                if (rows && rows.length > 0) {
                    //
                    for (var i = 0; i < rows.length ; i++) {
                        //
                        var type = rows[i].MType;
                        //对于销售单，必须有销售单号
                        if (type.toLowerCase().indexOf("invoice_sale") >= 0) {
                            //
                            if (rows[i].MNumber && rows[i].MNumber.length > 0 && rows[i].MCyID && rows[i].MBizDate && rows[i].MDueDate) {
                                //
                                newRows.push(rows[i]);
                            }
                        }
                        else if (type.toLowerCase().indexOf("invoice_purchase") >= 0 || type.toLowerCase().indexOf("expense") >= 0) {
                            //对于采购单，不需要编号
                            if (rows[i].MCyID && rows[i].MBizDate && rows[i].MDueDate) {
                                //
                                newRows.push(rows[i]);
                            }
                        }
                        else if (type.toLowerCase().indexOf("pay") >= 0 || type.toLowerCase().indexOf("receive") >= 0) {
                            //对于收付款单，则银行账户是必须的
                            if (rows[i].MBankID && rows[i].MBankID.length > 0 && rows[i].MCyID) {
                                //
                                newRows.push(rows[i]);
                            }
                        }
                    }
                }
                //
                return newRows;
            }
            //根据一个MID或者到某一行的数据
            this.getRowByMID = function (mid) {
                //
                var rows = that.getGridRowsWithData();
                //
                for (var i = 0; i < rows.length ; i++) {
                    //比较MID
                    if (rows[i].MID === mid) {
                        //返回
                        return rows[i];
                    }
                }
                return false;
            };
            //
            this.getRowByIndex = function (index) {
                //
                return $(tableBody).datagrid("getRows")[index];
            };
            //更新单据的往来科目
            this.updateDocCurrentAccountCode = function (code) {
                //获取当前编辑的行
                var row = that.getRowByIndex(that.editIndex);
                //直接在后台更新就好了，不需要对用户做任何提示,只要不是新建的行，都需要直接更新
                row.MID && mAjax.post(updateDocCurrentAccountCodeUrl, {
                    docType: row.MType,
                    docId: row.MID,
                    accountCode: code
                }, function () {
                    //如果当前是联系人维度，并且本行的accountCode不等于accoutCode的话，则需要隐藏这行
                    if (currentType == ACCOUNT && code != AccountListData[accountType].MCode) {
                        //
                        mDialog.message(updateBillCurrentAccountSuccessfully);
                        //
                        that.deleteRow(that.editIndex, row.MID, true);
                        //
                        that.editIndex = null;
                        //
                        that.updateDifferenceWithEditRow();
                    }
                    else {
                        //结束编辑
                        that.endEdit();
                    }
                    //选中的联系人重现点击
                    that.updateTabValue();
                });
            };
            //对最终保存到数据库的业务单据进行校验
            this.validateInitDocument = function (doc) {
                //
                var message = [];
                //对于每一张单据都要进行校验
                for (var i = 0; i < doc.InitBillList.length ; i++) {
                    //
                    var code = doc.InitBillList[i].MCurrentAccountCode;
                    //
                    var checkGroupValueModel = doc.InitBillList[i].MCheckGroupValueModel;
                    //如果用户选择了科目，需要对科目的必录进行校验
                    if (code) {
                        //
                        var account = that.getAccountDataByCode(doc.InitBillList[i].MCurrentAccountCode);
                        //
                        var checkGroupModel = account.MCheckGroupModel;
                        //
                        doc.InitBillList[i].MCheckGroupModel = checkGroupModel;
                        //
                        var keys = Object.keys(checkGroupModel);
                        //
                        for (var j = 0; j < keys.length ; j++) {
                            //如果对于科目是必录的，但是单据没有录那是不行 
                            if (checkGroupModel[keys[j]] === 2 && !checkGroupValueModel[keys[j]]) {
                                //
                                var msg = HtmlLang.Write(LangModule.GL, "Account", "科目")
                                + account.MFullName + HtmlLang.Write(LangModule.GL, "CheckType", "核算维度:")
                                + checkTypeDal.getCheckTypeByColumnName(keys[j]).data.MCheckTypeName
                                + HtmlLang.Write(LangModule.GL, "IsRequired", "是必须录入的!");
                                //
                                message.push(msg);
                            }
                        }
                    }

                    //校验单据的准确性
                    //联系人和员工不可同时都有值
                    if (checkGroupValueModel.MContactID && checkGroupValueModel.MEmployeeID) {
                        //
                        var msg = HtmlLang.Write(LangModule.GL, "ContactAndEmployeeCannotHasValueAtSameTime", "联系人和员工不可同时都有值");
                        //
                        message.push(msg);
                    }

                    //联系人和员工不可同时都有值
                    if (!checkGroupValueModel.MContactID && !checkGroupValueModel.MEmployeeID) {
                        //
                        var msg = HtmlLang.Write(LangModule.GL, "ContactAndEmployeeCannotHasNoValueAtSameTime", "业务单据的联系人和员工不可同时都没有值");
                        //
                        message.push(msg);
                    }


                    //对于付款单，必须是其他类型的联系人或者员工，如果选的是其他类型联系人，则不能选择商品项目，
                    if (doc.InitBillList.MType == BillTypeListData[4].id) {
                        //
                        if (checkGroupValueModel.MContactType == "Other" && checkGroupValueModel.MExpItemID) {
                            //联系人付款单不可选择费用项目
                            //
                            var msg = HtmlLang.Write(LangModule.GL, "ContactPaymentCannotHasExpItem", "联系人的付款单不可有费用项目");
                            //
                            message.push(msg);
                        }
                        //
                        if (checkGroupValueModel.MEmployeeID && checkGroupValueModel.MMerItemID) {
                            //联系人付款单不可选择费用项目
                            //
                            var msg = HtmlLang.Write(LangModule.GL, "EmployeePaymentCannotHasMerItem", "员工付款单不可有商品项目");
                            //
                            message.push(msg);
                        }
                        //
                        if (checkGroupValueModel.MMerItemID && checkGroupValueModel.MExpItemID) {
                            //联系人付款单不可选择费用项目
                            //
                            var msg = HtmlLang.Write(LangModule.GL, "MerItemAndExpItemCannotAllExists", "商品项目和费用项目不可同时录入");
                            //
                            message.push(msg);
                        }

                    }

                    if (doc.InitBillList[i].MTaxTotalAmt < 0 || doc.InitBillList[i].MTaxTotalAmt < 0) {
                        var msg = HtmlLang.Write(LangModule.IV, "BillAmountMustGreatZore", "单据金额必须大于0");
                        //
                        message.push(msg);
                    }

                }
                return message.length > 0 ? message : doc;
            }
            //保存初始化账单的数据
            this.saveInitDocument = function (callback) {
                //先校验
                if (that.validateTableData()) {
                    //更新差额
                    that.updateDifference();
                    //获取单据
                    var doc = that.getInitDocumentModel();
                    //
                    if (doc !== false) {
                        //再经过一次校验
                        var validate = that.validateInitDocument(doc);
                        //
                        if ($.isArray(validate)) {
                            //
                            return mDialog.error("<div>" + validate.join('</div><div>') + "</div>", null, true);
                        }
                        //
                        var func = function () {
                            //请求
                            $.mAjax.submit(saveInitDocumentUrl, { model: doc }, function (data) {
                                //保存成功
                                if (data && data.Success) {
                                    //
                                    $.mDialog.message(LangKey.SaveSuccessfully);
                                    //
                                    $.isFunction(callback) && callback();
                                }
                            });
                        }
                        //如果业务系统已经启用，但是否差额不为0
                        if (currentType == ACCOUNT && $(differenceAmountInput).val() != 0) {
                            //提醒用户，差额不为0 ，不可以保存
                            mDialog.confirm(differNotEqualZeroSureToSave, func);
                        }
                        else {
                            //
                            func();
                        }
                    }
                }
            };
            //获取初始化单据的数据
            this.getInitDocumentModel = function () {
                //先检查
                if (!that.validateTableData(true)) {
                    //
                    return false;
                }
                //
                that.endEdit();
                //获取每一行
                var entryList = that.getGridRowsWithData();
                //
                var doc = {};
                //具体的分录
                doc.InitBillList = checkTypeDal.setCheckGroupValueModel(entryList || []);
                //只有是科目维度的才需要获取用户的本年累计的一些情况
                if (currentType == ACCOUNT) {
                    //科目ID
                    doc.MAccountID = AccountListData[accountType].MItemID;
                    //
                    doc.InitBillAmountFor = that.getAmountForTotal();
                    //针对于没有数据的情况，其实就是清空科目余额
                    if (doc.InitBillAmountFor.length == 0) {
                        //
                        var amountFor = that.getAmountForTotal($(originalBalanceInfoTable));
                        //
                        for (var i = 0; i < amountFor.length ; i++) {
                            //期初
                            amountFor[i].MInitBalance = 0;
                            //期初（外币）
                            amountFor[i].MInitBalanceFor = 0;
                        }
                        //
                        doc.InitBillAmountFor = amountFor;
                    }
                }
                //
                return doc;
            };
            //新增联系人
            this.addNewContact = function () {
                //传页签过去
                $.mDialog.show({
                    mTitle: HtmlLang.Write(LangModule.Contact, "AddContact", "Add Contact"),
                    mContent: "iframe:" + '/BD/Contacts/ContactsEdit?Origin=AccountTransaction',
                    mWidth: 1110,
                    mHeight: 450,
                    mShowbg: true,
                    mShowTitle: true,
                    mDrag: "mBoxTitle",
                    mCloseCallback: function () {
                        //直接重新加载次页面就行了
                        mWindow.reload();
                    }
                });
            };
            //导入联系人
            this.importContact = function () {
                //
                ImportBase.showImportBox("/BD/Import/Import/Contact", HtmlLang.Write(LangModule.Contact, "ImportContacts", "Import Contacts"), 800, 500);
            }
            //新增联系人
            this.addNewEmployee = function () {
                $.mDialog.show({
                    mTitle: HtmlLang.Write(LangModule.Contact, "AddEmployee", "Add Employee"),
                    mContent: "iframe:" + '/BD/Employees/EmployeesEdit?Origin=AccountTransaction',
                    mWidth: 1000,
                    mHeight: 450,
                    mShowbg: true,
                    mShowTitle: true,
                    mDrag: "mBoxTitle",
                    mCloseCallback: function () {
                        //直接重新加载次页面就行了
                        mWindow.reload();
                    }
                });
            };
            //导入联系人
            this.importEmployee = function () {
                //
                ImportBase.showImportBox("/BD/Import/Import/Employees", HtmlLang.Write(LangModule.Contact, "ImportEmployees", "Import Employees"), 830, 525);
            }
            //更新差额
            this.updateDifference = function (initBalance, editRow, notShow) {
                //获取所有的行
                var rows = that.getGridRowsWithData(true);
                //
                editRow ? (rows.push(editRow)) : "";
                //把没有选择币别 没有设置往来科目的过滤出去
                rows = rows.where("x.MCyID && x.MCyID.length > 0 && x.MCurrentAccountCode && x.MCurrentAccountCode.length > 0 ");
                //如果没有行
                if (rows.length == 0) {
                    //合计
                    $(totalAmountInput).val("");
                    //差额
                    $(differenceAmountInput).val("");
                    //把下面的所有行都清除
                    $("tr:not(.demo)", balanceInfoTable).remove();
                    //
                    return;
                }
                //计算里面的金额
                var value = 0;
                //
                InitBillAmountFor = [];
                //获取所有的币别(本位币排在第一,必须保证有本位币的情况）
                var cyIDs = (rows.select("MCyID").contains(baseCyID) ? [baseCyID] : []).concat(rows.select("MCyID").distinct().where("x !='" + baseCyID + "'"));
                //遍历
                for (var i = 0; i < cyIDs.length ; i++) {
                    //每一种币别都添加进去
                    InitBillAmountFor.push({ MCyID: cyIDs[i], MInitBalance: 0 });
                }

                //获取所有的收款单
                var receiveSaleList = rows.where("x.MType=='Receive_Sale' || x.MType=='Receive_Other' || x.MType=='Receive_SaleReturn' || x.MType=='Receive_Purchase' || x.MType=='Receive_Adjustment' || x.MType=='Receive_BankFee'");
                //获取所有的销售单
                var invoiceSaleList = rows.where("x.MType=='Invoice_Sale'");
                //获取所有的红字销售单
                var invoiceSaleRedList = rows.where("x.MType=='Invoice_Sale_Red'");
                //获取所有的采购单
                var invoicePurchaseList = rows.where("x.MType=='Invoice_Purchase'");
                //获取所有的红字采购单
                var invoicePurchaseRedList = rows.where("x.MType=='Invoice_Purchase_Red'");
                //获取所有的付款单
                var payPurchaseList = rows.where("x.MType=='Pay_Purchase' || x.MType=='Pay_Other' || x.MType=='Pay_PurReturn' || x.MType=='Pay_BankFee' || x.MType=='Pay_OtherReturn' || x.MType=='Pay_Adjustment'");
                //获取所有的费用报销单
                var expenseClaimsList = rows.where("x.MType=='Expense_Claims'");

                //针对于联系人维度
                if (currentType == ACCOUNT) {
                    //如果是 应收账款 预付 其他应收（资产类科目），则金额 = 销售单 - 红字销售单 + 付款单 - 收款单 - 采购单 + 红字采购单 - 费用报销单
                    var dir = (accountType == 1 || accountType == 3 || accountType == 5) ? -1 : 1;
                    //
                    for (var i = 0; i < InitBillAmountFor.length ; i++) {
                        //
                        var cyIDFilter = "x.MCyID == '" + InitBillAmountFor[i].MCyID + "'";
                        //获取对应币别的行
                        InitBillAmountFor[i].MInitBalanceFor = (invoiceSaleList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise)
                        - invoiceSaleRedList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise)
                        + payPurchaseList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise)
                        - receiveSaleList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise)
                        - invoicePurchaseList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise)
                        + invoicePurchaseRedList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise)
                        - expenseClaimsList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise)) * dir;
                        //获取对应币别的行（本位币）
                        InitBillAmountFor[i].MInitBalance = (invoiceSaleList.where(cyIDFilter).sum("MTaxTotalAmt", precise)
                        - invoiceSaleRedList.where(cyIDFilter).sum("MTaxTotalAmt", precise)
                        + payPurchaseList.where(cyIDFilter).sum("MTaxTotalAmt", precise)
                        - receiveSaleList.where(cyIDFilter).sum("MTaxTotalAmt", precise)
                        - invoicePurchaseList.where(cyIDFilter).sum("MTaxTotalAmt", precise)
                        + invoicePurchaseRedList.where(cyIDFilter).sum("MTaxTotalAmt", precise)
                        - expenseClaimsList.where(cyIDFilter).sum("MTaxTotalAmt", precise)) * dir;
                        //综合本位币金额
                        value += InitBillAmountFor[i].MInitBalance;
                    }
                    value = value.toFixed(precise);
                    //合计
                    $(totalAmountInput).val(value);
                    //更新初始化的值
                    var differ = Math.abs(+value - (initBalance != undefined ? (+initBalance) : (+$(initBalanceBaseInput).numberbox("getValue"))));
                    //把差额，缩小到两位数
                    differ = differ.toFixed(precise);
                    //
                    $(differenceAmountInput).val(differ);
                    //表示有差额，则把字体改为红色的
                    differ > 0 ? $(differenceAmountInput + "," + differenceAmountLabel).removeClass(equalClass).addClass(notEqualClass)
                        : $(differenceAmountInput + "," + differenceAmountLabel).removeClass(notEqualClass).addClass(equalClass);
                    //如果forValues有值
                    !notShow && InitBillAmountFor.length > 0 && that.showAmountForTotal(InitBillAmountFor, BalanceType);
                }
                else {
                    //如果是科目维度的
                    //如果是应付账款 预付账款 其他应付（负债类科目），则金额 = -销售单 - 红字销售单 - 付款单 + 收款单 + 采购单 - 红字采购单 + 费用报销单
                    for (var i = 0; i < InitBillAmountFor.length ; i++) {
                        //
                        var cyIDFilter = "x.MCyID == '" + InitBillAmountFor[i].MCyID + "'";
                        //如果当前维度是销售单
                        if (currentBillType == invoiceSale) {
                            //获取对应币别的行
                            InitBillAmountFor[i]["MTaxTotalAmtFor"] = invoiceSaleList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise) - invoiceSaleRedList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise);
                            //获取对应币别的本位币行
                            InitBillAmountFor[i]["MTaxTotalAmt"] = invoiceSaleList.where(cyIDFilter).sum("MTaxTotalAmt", precise) - invoiceSaleRedList.where(cyIDFilter).sum("MTaxTotalAmt", precise);
                        }
                        else if (currentBillType == invoicePurchase) {
                            //获取对应币别的行
                            InitBillAmountFor[i]["MTaxTotalAmtFor"] = invoicePurchaseList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise) - invoicePurchaseRedList.where(cyIDFilter).sum("MTaxTotalAmtFor", precise);
                            //获取对应币别的本位币行
                            InitBillAmountFor[i]["MTaxTotalAmt"] = invoicePurchaseList.where(cyIDFilter).sum("MTaxTotalAmt", precise) - invoicePurchaseRedList.where(cyIDFilter).sum("MTaxTotalAmt", precise);
                        }
                        else {
                            InitBillAmountFor[i]["MTaxTotalAmtFor"] = rows.where(cyIDFilter).sum("MTaxTotalAmtFor", precise);
                            //获取对应币别的本位币行
                            InitBillAmountFor[i]["MTaxTotalAmt"] = rows.where(cyIDFilter).sum("MTaxTotalAmt", precise);
                        }
                    }
                    //
                    //如果forValues有值
                    !notShow && InitBillAmountFor.length > 0 && that.showDocAmountForTotal(InitBillAmountFor);
                }
            };
            //获取总币别的情况
            this.getAmountForTotal = function ($table) {
                //获取ul
                var table = $table || $(balanceInfoTable);
                //把里面的行都去掉
                var trs = $("tr:not(.demo)", table);
                //
                InitBillAmountFor = [];
                //
                for (var i = 0; i < trs.length ; i++) {
                    //是否是本位币
                    var isBase = $(forLiCurrency, trs[i]).attr("value") == baseCyID;
                    //每一种币别都加进去
                    InitBillAmountFor.push({
                        //币别
                        MCyID: $(forLiCurrency, trs[i]).attr("value"),
                        //期初
                        MInitBalance: $(initBalanceSpan, trs[i]).attr("value"),
                        //期初（外币）
                        MInitBalanceFor: isBase ? $(initBalanceSpan, trs[i]).attr("value") : $(initBalanceForSpan, trs[i]).attr("value")
                    });
                }
                //如果没有统计，则直接保存一个本年累计
                if ((InitBillAmountFor.length == 0 || InitBillAmountFor.where("x.MCyID == '" + baseCyID + "'").length == 0) && !IsGLBeginInJan) {
                    //
                    InitBillAmountFor.push({
                        //币别
                        MCyID: baseCyID,
                        //期初
                        MInitBalance: 0,
                        //期初（外币）
                        MInitBalanceFor: 0
                    });
                }
                //
                return InitBillAmountFor;
            };
            //显示业务单据的合计情况
            this.showDocAmountForTotal = function (data) {
                //
                var table = $(balanceInfoTable);
                //
                var cyIDs = data.select("MCyID");
                //把里面的没有的币种的行都去掉
                $(docTotalInfoFor + ":not(.demo)", table).filter(function () {
                    //
                    return !cyIDs.contains($(this).attr("MCyID"));
                }).remove();
                //每一行显示一下
                for (var i = 0; i < data.length; i++) {
                    //先找一下有没有这一行
                    var existsTr = $(docTotalInfoFor + "[MCyID='" + data[i].MCyID + "']", table);
                    //如果没有的话，则克隆一条
                    if (existsTr.length == 0) {
                        //加入行
                        existsTr = $(docTotalInfoFor + ".demo", table).clone().removeClass("demo");
                        //
                        table.append(existsTr);
                        //
                        existsTr.attr("MCyID", data[i].MCyID);
                    }
                    //里面的币别
                    $(forLiCurrency, existsTr).text(data[i].MCyID).attr("value", data[i].MCyID);
                    //金额 本位币金额
                    $(docSpan, existsTr).text(mMath.toMoneyFormat(data[i].MTaxTotalAmt)).attr("value", data[i].MTaxTotalAmt);
                    //原币金额
                    var forValue = data[i].MCyID == baseCyID ? data[i].MTaxTotalAmt : data[i].MTaxTotalAmtFor;
                    //金额 原币金额
                    $(docForSpan, existsTr).text(mMath.toMoneyFormat(forValue)).attr("value", forValue);
                    //显示
                    existsTr.show();
                }
                //把本位币的那一行移动到第一
                $(docTotalInfoFor + "[MCyID='" + baseCyID + "']", table).insertAfter($(docTotalInfoFor + ".demo", table));
                //
                that.resizeTableDiv();
                //
                that.resizeTable();
            };
            //显示外币的情况
            this.showAmountForTotal = function (data, type) {
                //
                var table = $(type == OrignalType ? originalBalanceInfoTable : balanceInfoTable);
                //获取所有的币别
                var cyIDs = data.select("MCyID");
                //把里面的没有的币种的行都去掉
                $("tr:not(.demo)", table).filter(function () {
                    //
                    return !cyIDs.contains($(this).attr("MCyID"));
                }).remove();
                //每一行显示一下
                for (var i = 0; i < data.length; i++) {
                    //先找一下有没有这一行
                    var existsTr = $("tr[MCyID='" + data[i].MCyID + "']", table);
                    //如果没有的话，则克隆一条
                    if (existsTr.length == 0) {
                        //加入行
                        existsTr = $(accountBalanceInfoTr + ".demo", table).clone().removeClass("demo");
                        //
                        table.append(existsTr);
                        //
                        existsTr.attr("MCyID", data[i].MCyID);
                    }
                    //里面的币别
                    $(forLiCurrency, existsTr).text(data[i].MCyID).attr("value", data[i].MCyID);
                    //金额 本位币金额
                    $(initBalanceSpan, existsTr).text(mMath.toMoneyFormat(data[i].MInitBalance)).attr("value", data[i].MInitBalance);
                    //原币金额
                    var forValue = data[i].MCyID == baseCyID ? data[i].MInitBalance : data[i].MInitBalanceFor;
                    //金额 原币金额
                    $(initBalanceForSpan, existsTr).text(mMath.toMoneyFormat(forValue)).attr("value", forValue);
                    //显示
                    existsTr.show();
                }
                //把本位币的那一行移动到第一
                $("tr[MCyID='" + baseCyID + "']", table).insertAfter($(accountBalanceInfoTr, table));
                //
                that.resizeTableDiv();
                //
                that.resizeTable();
            };
            //结束编辑，并且跳转到下一行
            this.endEditAndGoToNextRow = function () {
                //
                var index = that.editIndex;
                //
                if (that.validateEditRow()) {
                    //
                    $(tableBody).datagrid("endEdit", index);
                    //如果下面有一行
                    if ($(tableBody).datagrid("getRows").length == (index + 1)) {
                        //下面加一行
                        that.insertRow(index + 1);
                    }
                    //下一行开始编辑
                    $(tableBody).datagrid("beginEdit", index + 1);
                    //
                    that.bindTableRowAmountInputEvent(index + 1);
                }
            };
            //校验表格里面的内容是否合法
            this.validateTableData = function (alert) {
                //如果表格里面有编辑的行，校验编辑的行是否合法
                if (!that.validateEditRow()) {
                    //表格内容有问题
                    return false;
                }
                //是否有重复的单据编号
                var rows = that.getGridRowsWithData();
                //如果有行
                if (rows.length > 0) {
                    //
                    var numbers = rows.where("x.MType=='Invoice_Sale' || x.MType=='Invoice_Sale_Red'").select("MNumber");
                    //如果有重复的编号就提醒用户
                    if (numbers.length > numbers.distinct().length && alert) {
                        //
                        mDialog.message(existsDuplicatedNumber);
                        //
                        return false;
                    }
                }

                //单据金额必须大于0

                //
                return true;
            };
            //检查表格中是否有没有保存的数据，如果有的话提醒用户并且
            this.checkDataGridUnsaveBill = function (callback, cancelCallback) {
                //
                if (that.editIndex != null || (that.getGridRowsWithData().length > that.getGridRowsWithData().where('!!x.MID').length)) {
                    //提醒用户本界面有为保存的数据
                    mDialog.confirm(leaveWithUnsaveDataConfirm, function () {
                        //
                        that.editIndex = null;
                        //
                        $.isFunction(callback) && callback();
                    }, cancelCallback);
                }
                else {
                    //直接调用回掉函数
                    callback();
                }
            };
            //编辑表格的某一行一级某一个单元格
            this.editDataGridAndFocusCell = function (rowIndex, fieldName) {
                //
                $(tableBody).datagrid("beginEdit", rowIndex);
                //
                var editor = that.getRowEditor()[fieldName].textbox().focus();
                //处理银行是否可选，外币金额是否需要输入的问题
                that.handleMTypeMCyID();
            };
            //
            this.resize = function () {
                //调整tab
                that.resizeFrame();
                //
                that.resizeTab();
                //
                that.resizeTable();
            }
            //
            this.init = function (initOver, accountCode) {
                //
                isInitOver = initOver == "1" || initOver == "True";
                //
                BeginDate = $("#hideBeginDate").val();
                //
                GLBeginDate = $("#hideGLBeginDate").val();
                //是否启用了业务系统(如果业务系统比总账先启用，则不能在此界面维护初始化单据）
                IsBusinessSystemInited = mDate.parse(BeginDate).getTime() > mDate.parse('1901-01-01').getTime();
                //是否在一月启用的总账
                IsGLBeginInJan = mDate.parse(GLBeginDate).getMonth() == 0;
                //最大的单据业务日期控制，如果没启用业务系统，则单据必须在总账之前，如果启用了业务系统就，单据日期必须在业务日期之前
                MaxBillDate = IsBusinessSystemInited ? BeginDate : GLBeginDate;
                //
                currentType = ACCOUNT;
                //
                accountType = that.getAccountTypeByCode(accountCode);
                //
                that.initDomValue(accountType);
                //
                that.initEvent();
                //
                that.initTab(accountType);
                //初始化表格里面的数据
                that.initTableData();
                //
                that.resizeTab();
            }
        }
        return InitDocument;
    })();
    //
    window.InitDocument = InitDocument;
})()