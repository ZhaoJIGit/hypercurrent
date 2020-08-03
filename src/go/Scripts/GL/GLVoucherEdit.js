(function () {
    var GLVoucher = (function () {
        //定义表格的宽度的比例
        var fastcodeWidth = 150,
        explanationWidth = 140,
        accountWidth = 200,
        currencyWidth = 160,
        checkGroupWidth = 250,
        debitWidth = 150,
        creditWidth = 150,
        //英文环境下 fastcodecombox 的最小宽度
        fastcodeComboxMinWidth = 165; 
        //
        var keydownEvent = false;
        //最基本宽度
        var baseWidth = 150 + 140 + 200 + 160 + 250 + 150 + 150;
        //
        var moduleTableBaseWith = 150 + 140 + 200 + 160 + 250 + 150 + 150;
        //操作对象
        var operateTarget = 0;
        //
        var VOUCHER = 0,
        MODULE = 1;
        //摘要的数据,科目数据，联系人数据,跟踪项信息
        var voucherModuleWithNoEntryData = [],
        explanationData = [],
        accountData = [],
        voucherModuleWithEntryData = [];
        var GLBeginDate;
        //审核状态
        var approved = "1",
        unapproved = "0",
        inited = "1",
        uninited = "0",
        draft = "-1",
        saved = "0";
        //保存一个凭证
        var voucherData = {};
        //凭证分录
        var voucherEntrysData = [];
        //基准宽度
        var allBaseWidth = 1260;
        //全部需要宽度
        var allWidth = 1260;
        //模板全部需要的宽度
        var allModuleWith = 1260;
        //默认显示的行数
        var defaultRowCount = 5;
        //
        var dataInitedCount = 0;

        var isSmartVersion = false;
        //默认的可用日期
        var defaultDate = "";
        //是否显示联系人
        var itemID, mNumber, oItemID, isCopy, isReverse, dir, fromPeriodTransfer, fromDocVoucher, docVoucherID, docID, isModule, entryAccountPair, sourceBillKey;

        //是否是从业务单据转凭证设置过来的
        var fromDocVoucherSetting = false;
        //
        var isDraftDocVoucher = false;
        //当前页面不需要取消校验，保持校验
        window.noCancelValidate = true;

        //是否是期末处理凭证
        var isPeriodTransfer;
        //是否是期末调汇的凭证
        var isFinalTransfer;
        //
        var advShowClass = "vc-adv-show",
        advHideClass = "vc-adv-hide";
        //
        var statusSpan = ".vc-status-span";
        //
        var body = ".vc-body";
        //
        var creater = ".vc-creator";
        //
        var bodyDiv = ".vc-body-div";
        //
        var selectVoucherDiv = ".vc-select-voucher";
        //
        var prevVoucher = ".vc-prev-voucher";
        //
        var nextVoucher = ".vc-next-voucher";
        //
        var toolBarCenter = ".m-toolbar-footer .center";
        //
        var statusVoucher = ".vc-status-voucher";
        //
        var checkGroupTable = ".vc-checkgroup-table";
        //
        var checkGroupTitleTd = "td.vc-checkgroup-title-td";
        //
        var checkGroupValueTb = "td.vc-checkgroup-value-td";
        //
        var checkGroupTitle = "div.vc-checkgroup-title";
        //
        var checkGroupValue = "input.vc-checkgroup-value";
        //样式定义
        var tableBody = ".vc-table-body";
        //
        var tableHeader = ".vc-table-th";
        //
        var entryRow = ".vc-entry-row";
        //
        var disableTdClass = "vc-disable-td";
        //
        var explanationDiv = ".vc-explanation-div";
        //
        var moduleEntryRow = "tr.vc-module-entry-row";

        var docViewHref = ".vc-doc-view";

        var reverseViewHref = ".vc-reverse-view";

        //会计科目勾选
        var accountSelect = ".vc-fastcode-account-select";
        //外币核算勾选
        var currencySelect = ".vc-fastcode-currency-select";
        //辅助核算勾选
        var checkGroupSelect = ".vc-fastcode-checkgroup-select";

        //
        var fastcodeContactDiv = ".vc-fastcode-contact-div";
        //
        var fastcodeAccountNameSpan = ".vc-fastcode-account-name";
        //
        var fastcodeExplanationDiv = ".vc-fastcode-explanation-div";
        //
        var fastcodeAccountDiv = ".vc-fastcode-account-div";
        //
        var fastcodeTrackDiv = ".vc-fastcode-track-div";
        //
        var fastcodeDebitDiv = ".vc-fastcode-debit-div";
        //
        var fastcodeCreditDiv = ".vc-fastcode-credit-div";
        //
        var currencyRateTr = ".tb-currency-rate-tr";
        //
        var currencyAmountTr = ".tb-currency-amount-tr";
        //
        var currencyAmountForTr = ".tb-currency-amount-tr";
        //
        var currencyDirectTr = ".tb-currency-direct-tr";

        //
        var fastcodeDebitTd = "td.vc-fastcode-debit";
        //
        var fastcodeCreditTd = "td.vc-fastcode-credit";
        //
        var fastcodeExplanationTd = "td.vc-fastcode-explanation";
        //
        var fastcodeContactTd = "td.vc-fastcode-contact";
        //
        var fastcodeAccountTd = "td.vc-fastcode-account";

        //
        var fastCodeTr = ".vc-fastcode";

        //
        var accountDiv = ".vc-account-div";
        //
        var fastcodeDiv = ".vc-fastcode-div";
        //
        var accountNameSpan = ".vc-account-name";
        //
        var creatorName = ".vc-creator-name";
        //
        var accountCurrencyHref = ".vc-account-currency";
        //
        var headerAttachment = ".vc-header-attachment";
        //
        var currencyDiv = ".vc-currency-div";
        //
        var checkGroupDiv = ".vc-checkgroup-div";
        //
        var trackDiv = ".vc-track-div";
        //
        var debitDiv = ".vc-debit-div";
        //
        var creditDiv = ".vc-credit-div";
        //
        var debitTd = ".vc-debit";
        //
        var creditTd = ".vc-credit";
        //
        var fastcodeTd = "td.vc-fastcode";
        //
        var explanationTd = "td.vc-explanation";
        //
        var contactTd = "td.vc-contact";
        //
        var currencyTd = "td.vc-currency";
        //
        var checkGroupTd = "td.vc-checkgroup";
        //
        var accountTd = "td.vc-account";
        //
        var trackTd = "td.track";
        //
        var debitTotalDiv = ".vc-debit-total-div";
        //
        var creditTotalDiv = ".vc-credit-total-div";
        //
        var currencySetupDiv = ".vc-currency-setup-div";
        //
        var checkGroupSetupDiv = ".vc-checkgroup-setup-div";
        //
        var fastcodeInputDiv = ".vc-fastcode-input-div";
        //
        var currencySetupDemoDiv = ".vc-currency-setup-demo-div";
        //
        var checkGroupSetupDemoDiv = ".vc-checkgroup-setup-demo-div";
        //
        var fastcodeSetupDiv = ".vc-fastcode-setup-div";
        //
        var fastcodeOperationDiv = ".vc-fastcode-operation-div";
        //
        var fastcodeTableDiv = ".vc-fastcode-table-div";
        //
        var fastcodeTitle = ".vc-fastcode-title";
        //
        var fastcodeTable = ".vc-fastcode-table";
        //
        var fastCodeTableTh = ".vc-fastcode-table-th";
        //
        var modulePreviewDiv = ".vc-module-preview-div";
        //
        var modulePreviewTable = ".vc-module-preview-table";
        //
        var modulePreviewTableTh = ".vc-preview-table-th";
        //
        var fastcodeInput = "input.vc-fast-code";
        //
        var tbFastcodeInput = "input.tb-fastcode-input";
        //
        var tbDescriptionInput = "input.tb-description-input";
        //
        var descriptionInput = "input.vc-description";
        //
        var creditInput = ".vc-credit-input";
        //
        var debitInput = ".vc-debit-input";
        //
        var totalSpan = ".vc-total-span";
        //
        var vcRedClass = "vc-red";
        //
        var tableTotal = ".vc-table-total";
        //
        var newDeleteDiv = ".vc-new-delete-div";
        //
        var newHref = ".vc-new-href";
        //
        var deleteHref = ".vc-delete-href";
        //
        var hotkeyDiv = ".vc-hotkey-div";
        //
        var hotkeyTableDiv = ".vc-hotkey-table-div";
        //
        var fastcodeTextareaText = "<div class='vc-hide'><textarea class='vc-hide vc-fastcode-textarea'></textarea></div>";
        //
        var explanationTextareaText = "<div class='vc-hide'><textarea class='vc-hide vc-explanation-textarea'></textarea></div>";
        //
        var accountTextareaText = "<div class='vc-hide'><textarea class='vc-hide vc-account-textarea m-account-combobox'></textarea></div>";
        //
        var trackInputText = "<div class='vc-hide'><input class='vc-hide vc-track-input'/></div>";
        //
        var debitInputText = "<div class='vc-hide'><input class='vc-debit-input vc-hide' type='text'/></div>";
        //
        var creditInputText = "<div class='vc-hide'><input class='vc-credit-input vc-hide' type='text'/></div>";
        //当前所处焦点的元素
        var focusItem = "vc-is-focus";
        //不能编辑的单元格
        var disabedClass = "vc-disabled";
        //失去焦点的事件名称
        var vcBlurEvent = "vcBlur.vc";
        //
        var currencyId = ".tb-currency-id";
        //
        var currencyRate = ".tb-currency-rate";
        //
        var currencyAmount = ".tb-currency-amount";
        //
        var currencyAmountFor = ".tb-currency-amount-for";
        //
        var advancedOptions = ".vc-adv-options";
        //
        var advanceDiv = ".vc-adv";
        //
        var advanceSwitchDiv = ".vc-adv-switch";
        //
        var advanceSwitch = ".vc-adv-switch a";
        //复制凭证
        var copyVoucher = ".vc-adv-copy";
        //审核凭证
        var approveVoucher = ".vc-adv-approve";
        //反审核凭证
        var unapproveVoucher = ".vc-adv-unapprove";
        //打印凭证
        var printVoucher = ".vc-adv-print";
        //
        var amountEditDiv = ".vc-amount-edit-div";
        //
        var amountForSpan = ".vc-amount-for span:first";
        //
        var amountForInput = ".vc-amount-for input:first";
        //
        var amountRateSpan = ".vc-amount-rate span:first";
        //
        var amountRateInput = ".vc-amount-rate input:first";
        //
        var amountBaseSpan = ".vc-amount-base span:first";
        //
        var amountBaseInput = ".vc-amount-base input:first";
        //
        var debitSelect = ".vc-debit-select";
        //
        var creditSelect = ".vc-credit-select";
        //
        var voucherOperate = ".vc-voucher-operate";
        //日志
        var aHistory = "#aHistory";
        //
        var stopEvent = false;

        var rCreateReverse = 0, rViewReverse = 1, rViewOriginal = 2;


        /*各类提示语*/
        //
        var GL = "GL";
        //
        var newLang = HtmlLang.Write(LangModule.Common, "New", "New");
        //
        var editLang = HtmlLang.Write(LangModule.Common, "Edit", "Edit");
        //
        var viewLang = HtmlLang.Write(LangModule.GL, "View", "View");
        //
        var lockedLang = HtmlLang.Write(LangModule.GL, "Locked", "Locked");
        //
        var savedLang = HtmlLang.Write(LangModule.Common, "Saved", "Saved");
        //
        var approvedLang = HtmlLang.Write(LangModule.GL, "Approved", "Approved");
        //
        var draftLang = HtmlLang.Write(LangModule.Common, "Draft", "Draft");
        //
        var moduleLang = HtmlLang.Write(LangModule.Common, "Module", "模板");
        //
        var numberIsUsedLang = HtmlLang.Write(LangModule.GL, "VoucherNumberIsDuplicated", "凭证编号已经重复了，是否使用系统自动编号?");
        //
        var accountIsNoCheckForCurrency = HtmlLang.Write(LangModule.GL, "AccountNoNeedCheckForCurrency", "您选择的科目无需按外币核算");
        //
        var accountHasNoCheckType = HtmlLang.Write(LangModule.GL, "AccountHasNoCheckType", "您选择的科目没有辅助核算维度");
        //
        var voucherModuleLang = HtmlLang.Write(LangModule.Common, "vouchermodule", "模板凭证");
        //
        var noCheckType2Select = HtmlLang.Write(LangModule.GL, "noCheckType2Select", "本科目没有可供选择的核算维度");
        //
        var haveToChooseASameCurrencyBankAccount = HtmlLang.Write(LangModule.GL, "haveToChooseASameCurrencyBankAccount", "只能选择一个与单据币别相同的银行科目或者非银行科目!");
        //
        var voucherLang = HtmlLang.Write(LangModule.Common, "Voucher", "Voucher");
        //
        var GLHome = HtmlLang.Write(LangModule.GL, "GeneralLedger", "General Ledger");
        //
        var debitCreditImbalance = HtmlLang.Write(LangModule.GL, "CreditAndDebitImbalance", "Credit and Debit imbalance");
        //摘要不可为空
        var explanationNotNull = HtmlLang.Write(LangModule.GL, "ExplanationNotNull", "Explanation Not Null!");
        //联系人不可为空
        var contactNotNull = HtmlLang.Write(LangModule.GL, "ContactIsNull", "Contact Is Null!");
        //科目不可为空
        var accountNotNull = HtmlLang.Write(LangModule.GL, "AccountIsNull", "Account Is Null!");
        //币别不可为空
        var currencyNotNull = HtmlLang.Write(LangModule.GL, "CurrencyCanNotBeEmpty", "外币核算币别不可为空");
        //外币核算不可为空
        var currencyNotNullOrDocIsNotForeign = HtmlLang.Write(LangModule.GL, "CurrencyCanNotBeEmptyOrDocIsNotForeign", "外币核算币别不可为空或者业务单据为非外币单据,请更换科目或者修改外币信息");
        //原币金额和本位币金额正负向不一致
        var plusminusNoEqual = HtmlLang.Write(LangModule.GL, "PlusMinusNoEqual", "原币金额和本位币金额正负不一致！");
        //辅助核算不可为空
        var checkGroupNotNull = HtmlLang.Write(LangModule.GL, "CheckOtherCanNotBeEmpty", "以下类型的辅助核算维度不可为空：");
        //借贷双方金额不可都为空
        var debitCreditNotNull = HtmlLang.Write(LangModule.GL, "DebitCreditIsNull", "Debit or Credit must have value!");
        //凭证字不可为空
        var voucherGroupNotNull = HtmlLang.Write(LangModule.GL, "VoucherGroupIsNull", "VoucherGroup Is Null!");
        //编号不可为空
        var voucherNumberNotNull = HtmlLang.Write(LangModule.GL, "VoucherNumberIsNull", "Voucher Number Is Null!");
        //凭证日期不可为空
        var voucherDateNotNull = HtmlLang.Write(LangModule.GL, "VoucherDateIsNull", "Voucher Date Is Null!");
        //凭证分录不能为空
        var voucherEntryNotNull = HtmlLang.Write(LangModule.GL, "VoucherEntryIsNull", "Voucher Entry Is Null!");
        //至少录入两条分录
        var voucherEntryMoreThanOne = HtmlLang.Write(LangModule.GL, "VoucherEntryMoreThanOne", "凭证至少有两行分录!");
        //
        var voucherModuleLang = HtmlLang.Write(LangModule.Common, "vouchermodule", "模板凭证");
        //
        var fastCodeLang = HtmlLang.Write(LangModule.Common, "FastCode", "快速码");
        //快速码设置
        var fastCodeSetupLang = HtmlLang.Write(LangModule.Common, "FastCodeSetup", "快速码设置");
        //描述
        var descriptionLang = LangKey.Description;
        //至少录入两条凭证
        var moduleFastcodeNotNull = HtmlLang.Write(LangModule.GL, "VoucherModuleFastCodeIsNull", "模板快速码不可为空!");
        //moduleFastcodeBeenUsed
        var moduleFastcodeBeenUsed = HtmlLang.Write(LangModule.GL, "VoucherModuleFastcodeBeenUsed", "模板快速码已经被使用!");
        //至少录入两条凭证
        var moduleDescriptionNotNull = HtmlLang.Write(LangModule.GL, "VoucherModuleDescriptionIsNull", "模板描述不可为空!");
        //至少录入两条凭证
        var pleaseSelectColumns2Save = HtmlLang.Write(LangModule.GL, "PleaseSelectColumns2Save", "请选择要保存的列!");
        //至少录入两条凭证
        var cellDisabledFromDocVoucher = HtmlLang.Write(LangModule.GL, "CellDisabledForFromBusinessSystem", "此凭证由业务系统单据转化而来，此列不可编辑!");
        //凭证从累计计提折旧创建的，不可编辑
        var cellDisabledFromDepreciation = HtmlLang.Write(LangModule.GL, "CellDisabledForFromDepreciation", "此列不可修改：凭证由固定资产模块计提创建，请在固定资产模块进行修改!");
        //totalLang
        var totalLang = HtmlLang.Write(LangModule.Common, "Total", "Total");
        //
        var currencyLang = HtmlLang.Write(LangModule.Common, "Currency", "Currency");
        //
        var newVoucherLang = HtmlLang.Write(LangModule.Common, "newvoucher", "New Voucher");
        //
        var newVoucherModuleLang = HtmlLang.Write(LangModule.GL, "newvoucherModule", "Create new template voucher");
        //
        var approveSuccessLang = HtmlLang.Write(LangModule.Common, "ApproveSuccessfully", "Approve Successfully!");
        //
        var unapproveSuccessLang = HtmlLang.Write(LangModule.Common, "UnapproveSuccessfully", "Unpprove Successfully!");
        //
        var operationFail = HtmlLang.Write(LangModule.Common, "operationfail", "Operation Failded!");
        //获取凭证的url
        var getVoucherUrl = "/GL/GLVoucher/GetVoucherEditModel";
        //获取凭证模板
        var getVoucherModuleUrl = "/FC/FCHome/GetVoucherModule";
        //新建一个凭证
        var editVoucherUrl = "/GL/GLVoucher/GLVoucherEdit";
        //凭证是否存在
        var existVoucherUrl = "/GL/GLVoucher/Exists";
        //凭证首页
        var voucherHomeUrl = "/GL/GLVoucher/GLVoucherHome";
        //获取下一个可用的凭证编号
        var getNextVoucherNumberUrl = "/GL/GLVoucher/GLGetNextVoucherNumber";
        //获取相对汇率的url
        var getCurrencyRateUrl = "/BD/ExchangeRate/GetBDExchangeRate";
        //
        var isNumberUsedUrl = "/GL/GLVoucher/IsMNumberUsed";
        //
        var accountHasNotCheckTypeLang = HtmlLang.Write(LangModule.GL, "TipOfAccountHasNotAssistCheck", "提示:本行分录所选科目没有启用辅助核算维度，如需添加辅助核算，请先修改科目!");
        //
        var accountHasNotForeignCurrencyLang = HtmlLang.Write(LangModule.GL, "TipOfAccountNotCheckForCurrency", "提示:本行分录所选科目为非外币核算科目，如需添加外币核算，请先修改科目!");

        //查看冲销凭证
        var viewReverseVoucherLang = HtmlLang.Write(LangModule.GL, "ViewReverseVoucher", "查看冲销凭证");
        var viewOriginalVoucherLang = HtmlLang.Write(LangModule.GL, "ViewOriginalVoucher", "查看原凭证");
        var createReverseVoucherLang = HtmlLang.Write(LangModule.GL, "CreateReverseVoucher", "创建冲销凭证");

        //其他应收应付code
        var otherPayReceiveAccountCode = ["1221", "2241"];
        //期末转结的类型
        var periodTransferModel = null;
        //凭证主类
        var GLVoucher = function () {
            //
            var that = this;
            //
            var home = new GLVoucherHome();
            //
            var checkType = new GLCheckType();
            /*初始化数据*/
            this.initVoucherOrModule = function (year, period, day) {

                $("body").mask("");
                //如果是凭证模板
                if (isModule) {
                    //获取凭证模板
                    that.getVoucherModule(that.initVoucherOrModuleData)
                } else {
                    //获取凭证后然后初始化表格，即使MItemID为空，也要从后台获取一张空的凭证，里面有最基本的日期和编号，编号必须是连续的
                    that.getVoucherData(year, period, day, that.initVoucherOrModuleData);
                }

            };
            //初始化一张凭证
            this.initVoucherOrModuleData = function (voucher) {
                //这个地方加载的有点慢，需要加上等待
                $("body").mask("");

                oItemID = voucher.MOVoucherID;
                //如果有MItemID
                if (voucher && voucher.MItemID) {
                    //保存起来
                    itemID = voucher.MItemID;
                    //
                    voucherData = voucher;
                    //
                    if (voucher.MStatus != draft && !isModule) {
                        //显示高级选项开关
                        that.showAdvancedOptionSwitch(true);
                        //显示高级选项
                        that.showAdvancedOptions(true, voucher);
                        //如果是期末结转的凭证
                        var newName = voucher.MTransferTypeName ? voucher.MTransferTypeName : (GL + "-" + GLVoucherHome.toVoucherNumber(voucher.MNumber));
                        // 
                        window.parent.$(".mCloseBox").length == 0 && $.mTab.rename(newName);
                        //
                        isPeriodTransfer = voucher.MTransferTypeID != -1;
                        isFinalTransfer = voucher.MTransferTypeID == PeriodTransferType.FinalTransfer;
                    }
                    //如果是模板凭证
                    if (isModule) {
                        //更新为模板凭证的名字
                        window.parent.$(".mCloseBox").length == 0 && $.mTab.rename(voucherModuleLang + "[" + voucher.MFastCode + "]");
                    }
                }
                //如果不是凭证模板
                if (!isModule) {
                    //业务凭证ID
                    docID = voucher.MDocID;
                    //
                    docVoucherID = voucher.MDocVoucherID;

                    docType = voucher.MDocType;
                    //是否从业务单据过来
                    fromDocVoucher = !!docID && !isSmartVersion;
                    //
                    sourceBillKey = voucher.MSourceBillKey;
                    //如果是从业务单据过来的，并且还是草稿凭证的话，隐藏，保存并审核，以及保存并新建按钮
                    if (fromDocVoucher) {
                        //快速码那一列不显示
                        $(fastCodeTr).remove();
                        //合计哪一行，需要减少一列
                        var totalTd = $(tableTotal).find("td").eq(0);
                        //减少一列
                        totalTd.attr("colspan", totalTd.attr("colspan") - 1);
                        //
                        that.handleDraftDocVoucher(voucher.MStatus == draft);
                        //
                        that.handleViewDoc(docID, docType, voucher.MItemID);
                    }
                    else {
                        //如果不是业务单据转过来的，并且没有凭证ID，则为新增凭证
                        if (!voucher.MItemID) {
                            $.mTab.rename(newVoucherLang);
                        }
                    }
                    if (sourceBillKey == "2" || fromDocVoucher || !!docID) {
                        $(reverseViewHref).parent().remove();
                    }

                    if (isPeriodTransfer || fromPeriodTransfer) {
                        $("#btnSaveAndAddAnother").hide();
                    }
                }
                //
                voucher.MStatus != approved && that.getBaseData();

                //初始化表头
                that.initTableHeader(voucher);
                //初始化其他的
                that.initVoucherOther(voucher);
                //初始化凭证表
                that.initEntryTable(voucher);
                //插入尾部
                that.initTableTrail(voucher);
                //显示单据状态
                that.handleVoucherStatus(voucher);
                //重新计算
                that.calculateTotal();
                //计算div的高度
                that.resizeBody();
                //重新计算宽度
                that.resizeTable();
                //调整高度
                that.adjustAdvOptionsDom();
                //将当前页面设置为稳定状态
                that.handleChange();
                //初始化tooltip
                that.tooltip(tableBody);
                //设置修改编辑的源数据
                $.mTab.setStable();
                //
                $("body").unmask("");
            };
            //处理业务单据过来设置核算维度的
            this.handleDraftDocVoucher = function (draft) {

                if (draft === true) {
                    //
                    $("#btnSaveAndAddAnother,#btnSaveAndApprove").hide();
                    //标志为草稿的业务单据凭证
                    isDraftDocVoucher = true;
                    //编号不可编辑
                    $("#MNumber").numberspinner("disable", true);
                    $("#MNumber").numberspinner("readonly", true);
                    //日期不可编辑
                    $("#MDate").datebox("disable", true);
                    $("#MDate").datebox("readonly", true);
                    //日志隐藏
                    $("#aRelatedAttach").hide();
                    //上传附件隐藏
                    $("#aHistory").hide();
                }
                else if (fromDocVoucherSetting) {
                    //日期不可编辑
                    $("#MDate").datebox("disable", true);
                    $("#MDate").datebox("readonly", true);
                    $("#btnSaveAndAddAnother").hide();
                }
            }
            //显示业务单据详情链接
            this.handleViewDoc = function (docID, docType, itemID) {

                //
                $(docViewHref).parent().show();

                $(docViewHref).off("click").on("click", function () {
                    var isExists = false;

                    //检查下凭证是否存在 同步提交
                    mAjax.post(existVoucherUrl, { pkID: itemID }, function (data) {
                        isExists = data;
                    }, false, false, false);

                    if (isExists) {
                        home.viewDoc(docID, docType);
                    }
                    else {
                        $.mDialog.alert(HtmlLang.Write(LangModule.Common, "VoucherDeleted", "凭证已经被删除!"));
                    }
                })
            }
            //处理显示单据状态
            this.handleVoucherStatus = function (voucher) {
                //如果是模板凭证
                if (isModule) {
                    operateLang = newLang;
                    statusLang = moduleLang;
                } else {
                    //
                    var operateLang = "";
                    //
                    var statusLang = "";
                    //
                    switch (voucher.MStatus) {
                        case -1:
                            operateLang = newLang;
                            statusLang = draftLang;
                            break;
                        case 0:
                            operateLang = editLang;
                            statusLang = savedLang;
                            break;
                        case 1:
                            operateLang = viewLang;
                            statusLang = approvedLang;
                            break;
                        default:
                            operateLang = newLang;
                            statusLang = draftLang;
                            break;
                    }
                }
                //如果是从业务单据过来的加上locked标志
                $(voucherOperate).text(operateLang);
                //
                var spilt = top.OrgLang[0].LangID == '0x0009' ? " " : "";
                //
                $(statusSpan).text(statusLang + (spilt));
                //
                $(statusVoucher).show();
                //不可编辑
                if (that.enableEditVoucher(voucher) || isModule) {
                    //注册事件
                    that.bindTableEvent();
                    //初始化document事件
                    that.bindDocumentEvent(voucher);
                }
                //绑定模板的事件
                that.bindModuleDocumentEvent();
            };
            //显示高级选项的开关
            this.showAdvancedOptionSwitch = function (show) {
                //
                show ? $(advanceSwitchDiv).show() : $(advanceSwitchDiv).hide();
            };
            //判断一个科目是否是其他应收和其他应付
            this.checkAccountIsOtherPayReceive = function (code) {
                //
                for (var i = 0; i < otherPayReceiveAccountCode.length ; i++) {
                    //
                    if (code && code.indexOf(otherPayReceiveAccountCode[i]) == 0) {
                        //如果
                        return true;
                    }
                }
                return false;
            }
            //显示高级选项
            this.showAdvancedOptions = function (show, voucher) {
                //如果是显示
                if (show) {
                    //先显示
                    $(advancedOptions).show();
                    //如果还没有加载过
                    if ($(advancedOptions).attr("inited") != inited) {
                        //初始化高级
                        that.initAdvancedOption(voucher);
                    }
                    //显示审核还还是反审核
                    that.showUnapproveHref(voucher.MStatus, voucher.MSettlementStatus);
                    //显示冲销相关
                    that.showReverseHref(voucher);
                    //计算宽度
                    that.resizeAdvanceOptionSize();
                    //把附件的margin-调整下
                } else {
                    //先显示
                    $(advancedOptions).hide();
                    //调整表格的宽度
                    $(tableBody).width($(bodyDiv).width());
                }
                //重新计算宽度
                that.resizeTable();
            };
            //重新计算高级选项的宽度
            this.resizeAdvanceOptionSize = function () {
                //
                if ($(advancedOptions).is(":visible")) {
                    //去除fixed样式
                    $(tableBody).removeClass("vc-fixed-table");
                    //
                    var tableBodyWidth = $(bodyDiv).width() - $(advancedOptions).outerWidth() - 18;
                    //调整表格的宽度
                    $(tableBody).width(tableBodyWidth);
                    //去除fixed样式
                    $(tableBody).addClass("vc-fixed-table");
                    //
                    $(headerAttachment).css({
                        "margin-right": ($(advancedOptions).outerWidth() + 18 - $(advanceSwitchDiv).width()) + "px"
                    });
                } else {
                    //margin为0
                    $(headerAttachment).css({
                        "margin-right": "0px"
                    });
                }
            };
            //决定是显示审核还是反审核
            this.showUnapproveHref = function (status, settled) {
                //
                status != approved && !settled ? $(approveVoucher).parent().show() : $(approveVoucher).parent().hide();
                //
                status == approved && !settled ? $(unapproveVoucher).parent().show() : $(unapproveVoucher).parent().hide();
            };
            //决定显示创建冲销凭证还是查看原凭证
            this.showReverseHref = function (voucher) {

                if (voucher.MSourceBillKey == "2" || fromDocVoucher || !!voucher.MDocID) return;

                if (!!voucher.MRVoucherID || !!voucher.MOVoucherID) {
                    !!voucher.MRVoucherID ? $(reverseViewHref).text(viewReverseVoucherLang).attr("operate", rViewReverse) : $(reverseViewHref).text(viewOriginalVoucherLang).attr("operate", rViewOriginal);
                    $(reverseViewHref).parent().show();
                }
                else if (voucher.MStatus == approved) {
                    $(reverseViewHref).parent().show()
                    $(reverseViewHref).text(createReverseVoucherLang).attr("operate", rCreateReverse);
                }
            }
            //初始化高级选项
            this.initAdvancedOption = function (voucher) {
                //
                $(advanceDiv).accordion({
                    animate: true,
                    collapsed: false
                });
                $(advancedOptions).attr("inited", 1);
                //打开和关闭高级选项
                $(advanceSwitch).off("click").on("click",
                function () {
                    //
                    if ($(this).hasClass(advShowClass)) {
                        //切换样式
                        $(this).removeClass(advShowClass).addClass(advHideClass);
                        //关闭高级选项
                        that.showAdvancedOptions(false, voucher);
                    } else {
                        //切换央视
                        $(this).removeClass(advHideClass).addClass(advShowClass);
                        //关闭高级选项
                        that.showAdvancedOptions(true, voucher);
                    }
                });
                //复制
                $(copyVoucher).off("click").on("click", that.copy);
                //审核
                $(approveVoucher).off("click").on("click",
                function () {
                    that.approveVoucher(approved);
                });
                //反审核
                $(unapproveVoucher).off("click").on("click",
                function () {
                    that.approveVoucher(unapproved);
                });
                //打印
                $(printVoucher).off("click").on("click", that.printVoucher);

                //日志的显示
                $(aHistory).off("click").on("click", function () {
                    //
                    var itemID = $("#hidInvoiceId").val();
                    //
                    !!itemID ? HistoryView.openDialog(itemID, "GL_Voucher") : "";

                });
                //上一张凭证
                $(prevVoucher).off("click").on("click", function () {
                    //
                    that.selectVoucher(-1);
                });
                //下一张凭证
                $(nextVoucher).off("click").on("click", function () {
                    //
                    that.selectVoucher(1);
                });

                $(reverseViewHref).off("click").on("click", function () {

                    var operate = +$(this).attr("operate");

                    if (operate == rCreateReverse) {
                        that.createReverse();
                    }
                    else if (operate == rViewOriginal) {
                        that.viewOriginalVoucher();
                    }
                    else if (operate == rViewReverse) {
                        that.viewReverseVoucher();
                    }
                });

            };
            //初始化凭证的其他类容
            this.initVoucherOther = function (voucher) {
                //如果是模板
                if (isModule) {
                    //
                    $(fastcodeInput).val(voucher.MFastCode);
                    //
                    $(descriptionInput).val(voucher.MDescription);
                } else {
                    //设置编号
                    $("#MNumber").numberspinner("setValue", voucher.MNumber);
                    //设置日期
                    $("#MDate").datebox("setValue", voucher.MDate);
                    //有时候用户是输入数字
                    $(document).off("blur.gl", ".datebox-input").on("blur.gl", ".datebox-input",
                    function () {
                        //如果日期面板是显示的，就不做下面的操作了
                        if ($("#MDate").datebox("panel").is(":visible")) {
                            return true;
                        }
                        //
                        var date = $(this).val();
                        //如果日期不正确
                        if (!mDate.parse(date)) {
                            //提醒用户
                            mDialog.message(HtmlLang.Write(LangModule.Common, "InvalidDate", "Invalid Date"));
                            //
                            return false;
                        }
                        else if (mDate.parse(date).getTime() < GLBeginDate.getTime()) {
                            //如果凭证日期小于总账启用日期，则重置为总账启用日期
                            mDialog.message(HtmlLang.Write(LangModule.Common, "VoucherDateMustGreaterThanTheGLBeginDate", "凭证日期必须大于总账启用日期"));
                            //
                            $("#MDate").datebox("setValue", mDate.parse(defaultDate));
                            //
                            return false;
                        }
                        else {
                            //格式化
                            $("#MDate").datebox("setValue", mDate.format(date));
                        }
                        //
                        date = mDate.parse(date);
                        //
                        if (!(voucherData && voucherData.MItemID && mDate.parse(voucherData.MDate).getFullYear() == date.getFullYear() && mDate.parse(voucherData.MDate).getMonth() == date.getMonth())) {
                            //
                            that.getNextVoucherNumber();
                        }
                    });
                    //设置附件数
                    $("#Attachments").numberspinner("setValue", voucher.MAttachments);
                    //默认的日期赋值，当出现用户选择了不可用日期之后，还原为默认日期
                    defaultDate = voucher.MDate;
                }
                //创建者
                $(creatorName).text(voucher.MCreatorName);
            };
            //初始化表头
            this.initTableHeader = function () {
                //重新计算一下高度
                that.resizeTable();
            };
            //初始化表格
            this.initEntryTable = function (voucher) {
                //先清空表格
                $(entryRow, tableBody).remove();
                //
                var row = $(tableHeader);
                //
                entrys = (isModule ? voucher.MVoucherModuleEntrys : voucher.MVoucherEntrys) || new Array(defaultRowCount);
                //
                for (var i = 0; i < entrys.length; i++) {
                    //插入默认行数
                    row = that.insertOneRow(voucher.MStatus || unapproved, voucher.MSettlementStatus || unapproved, entrys[i], row);
                }
            };
            //初始化表尾
            this.initTableTrail = function (totalData) {
                //先删除后面一行
                $(tableTotal, tableBody).remove();
                //找到th中td的个数
                var tdCount = $(tableHeader).find("th").not(":hidden").length;
                //
                var trailText = "<tr class='vc-table-total'>" + "<td colspan='" + (tdCount - 2) + "'><span class='vc-total-div'>" + totalLang + ":" + "</span><span class='vc-total-span'></span></td>" + " <td class='vc-debit'><div class='vc-debit-total-div'>" + (totalData.MTotalDebit || "") + "</div></td> " + " <td class='vc-credit'><div class='vc-credit-total-div'>" + (totalData.MTotalCredit || "") + "</div></td> ";
                //
                trailText += "</tr>";
                //插到最后一行去
                return $(trailText).insertAfter($("tr", tableBody).last());
            };

            //快速码输入框事件
            this.initFastcodeTd = function ($td, rowData, lock) {
                //注册事件
                !lock && $($td).off("click.vc").on("click.vc",
                function (event) {
                    //获取文本显示span
                    var textDiv = $(fastcodeDiv, $td);
                    //如果文本可见
                    if (textDiv.is(":visible")) {
                        //如果span是显示的
                        var func = function (data) {
                            //本身隐藏
                            textDiv.hide();
                            //
                            var nextDiv = textDiv.next("div");
                            //
                            nextDiv.width(10).show();
                            //
                            voucherModuleWithNoEntryData = data.length != 0 ? data : voucherModuleWithNoEntryData;
                            //附近的那个文本输入框
                            var textarea = nextDiv.find("textarea").eq(0);
                            //文本框显示
                            textarea.width(10).height(46).show().combobox({
                                width: $td.innerWidth() > fastcodeComboxMinWidth ? $td.innerWidth() : fastcodeComboxMinWidth,
                                hasDownArrow: false,
                                data: data,
                                onSelect: function (row) {
                                    //插入信息
                                    that.getVoucherModuleData(row.MItemID, [],
                                    function (data) {
                                        //
                                        that.insertModuleRows(data, $td);
                                    });
                                },
                                textField: "MFullName",
                                valueField: "MItemID",
                                formatter: that.fastCodeFormatter,
                                groupField: "group",
                                groupFormatter: that.fastCodeGroupFormatter,
                                navigation: function (selector, value) {
                                    //
                                    var panel = new window.mCombobox(selector).getPanel();
                                    //
                                    value && !$.isArray(value) && that.getVoucherModuleData(value, [], that.showVoucherModule, panel, $td);
                                },
                                onLoadSuccess: function () {
                                    var mCombo = new window.mCombobox(textarea);
                                    //comboInput
                                    var comboInput = mCombo.getInput();
                                    //
                                    var comboPanel = mCombo.getPanel();
                                    //重新计算宽度
                                    that.resizeCombobox($td, comboInput, comboPanel);

                                    //获取先前选择的值
                                    textarea.combobox("setValue", textDiv.data("value") || "");
                                    //获取先前选择的值
                                    textarea.combobox("setText", textDiv.text() || "");
                                    //聚焦
                                    comboInput.select().focus().off("focus.minput");
                                    //
                                    that.bindTableEvent();
                                }
                            });
                            //获取文本输入框
                            var $input = new window.mCombobox(textarea).getInput();
                            //将其他在焦点的元素置位不在焦点
                            that.vcBlurEvent();
                            //获取焦点，注册事件
                            that.vcFocusEvent($input,
                            function () {
                                //获取焦点，并且选中文本
                                $input.select().focus();
                            },
                            function () {
                                //获取文本输入框
                                var $input = new window.mCombobox(textarea).getInput();
                                //获取输入的值
                                var selectedValue = $input.val();
                                //选择的文本
                                var selectedText = $input.val();
                                //注销
                                $(textarea).combobox("destroy");
                                //
                                textDiv.next("div").remove();
                                //把值给span
                                textDiv.html(mText.encode(selectedText || "")).show().data("value", selectedValue ? selectedValue : "").attr("etitle", mText.encode(selectedText)).tooltip({ content: mText.encode(selectedText) });
                                //在span下面再插入一个textarea，供下次使用
                                $(explanationTextareaText).insertAfter(textDiv);
                                //
                                textDiv.height($td.height());
                                //
                                textDiv.width($td.width());
                            });
                        }
                        that.getVoucherModuleWithNoEntryData(false, func, false);
                    }

                    //组织冒泡
                    event.stopPropagation();
                    //
                    return false;
                });
            };
            //摘要输入框的事件
            this.initExplanationTd = function ($td, entryData, lock) {
                //
                $(explanationDiv, $td).data("value", entryData.MExplanation);
                //注册事件
                !lock && $($td).off("click.vc").on("click.vc",
                function (event) {
                    //获取文本显示span
                    var textDiv = $(explanationDiv, $td);
                    //如果文本可见
                    if (textDiv.is(":visible")) {
                        //如果span是显示的
                        var func = function (data) {
                            //
                            explanationData = explanationData.length == 0 ? data : explanationData;
                            //本身隐藏
                            textDiv.hide();
                            //
                            var nextDiv = textDiv.next("div");
                            //
                            nextDiv.width(10).show();
                            //附近的那个文本输入框
                            var textarea = nextDiv.find("textarea").eq(0);
                            //文本框显示
                            textarea.width(10).height(46).show().mAddCombobox("voucherExplanation", {
                                width: $td.innerWidth(),
                                height: $td.innerHeight() - 4,
                                hasDownArrow: false,
                                data: explanationData,
                                onSelect: that.switch2Cell,
                                onLoadSuccess: function () {
                                    //如果没有值
                                    if (textDiv.text() == "") {
                                        //复制上一个值
                                        that.copyPrevExplanation(textarea);
                                    }
                                    var mCombo = new window.mCombobox(textarea);
                                    //comboInput
                                    var comboInput = mCombo.getInput();
                                    //
                                    var comboPanel = mCombo.getPanel();
                                    //重新计算宽度
                                    that.resizeCombobox($td, comboInput, comboPanel);
                                    //获取先前选择的值
                                    textarea.combobox("setValue", textDiv.data("value") || "");
                                    //获取先前选择的值
                                    textarea.combobox("setText", textDiv.mHtml() || "");
                                    //聚焦
                                    comboInput.select().focus().off("focus.minput");
                                    //
                                    that.bindTableEvent();
                                }
                            },
                            {
                                //默认是有权限编辑
                                hasPermission: 1,
                                //关闭后需要重新加载摘要
                                callback: function () {
                                    //重新获取摘要
                                    that.getExplanationData(true, function (data) {
                                        //重新加载
                                        textarea.combobox("loadData", data);
                                    }, true);

                                }
                            });
                            //获取文本输入框
                            var $input = new window.mCombobox(textarea).getInput();
                            //将其他在焦点的元素置位不在焦点
                            that.vcBlurEvent();
                            //获取焦点，注册事件
                            that.vcFocusEvent($input,
                            function () {
                                //获取焦点，并且选中文本
                                $input.select().focus();
                            },
                            function () {
                                //获取文本输入框
                                var $input = new window.mCombobox(textarea).getInput();
                                //获取输入的值
                                var selectedValue = $input.val();
                                //选择的文本
                                var selectedText = $input.val();
                                //注销
                                $(textarea).combobox("destroy");
                                //
                                textDiv.next("div").remove();
                                //把值给span
                                textDiv.mHtml(mText.encode(selectedText || "")).show().data("value", selectedValue ? selectedValue : "").attr("etitle", mText.encode(selectedText)).tooltip({ content: mText.encode(selectedText) });
                                //在span下面再插入一个textarea，供下次使用
                                $(explanationTextareaText).insertAfter(textDiv);
                                //
                                textDiv.height($td.height());
                                //
                                textDiv.width($td.width());
                            });
                        }
                        //如果摘要数据为空，则更新摘要数据
                        if (explanationData.length == 0) {
                            //更新
                            that.getExplanationData(false, func);
                        } else {
                            //直接执行
                            func();
                        }
                    }

                    //组织冒泡
                    event.stopPropagation();
                    //
                    return false;
                });
            };
            //科目输入框的事件
            this.initAccountTd = function ($td, entryData, lock) {
                //
                that.showAccountData($(accountDiv, $td), entryData.MAccountModel);
                //注册事件
                !lock && $($td).off("click.vc").on("click.vc",
                function (event) {
                    //获取文本显示span
                    var textDiv = $(accountDiv, $td);
                    //如果文本可见
                    if (textDiv.is(":visible")) {
                        //如果span是显示的
                        var func = function (data) {
                            //本身隐藏
                            textDiv.hide();
                            //
                            var nextDiv = textDiv.next("div");
                            //
                            nextDiv.width(10).show();
                            //附近的那个文本输入框
                            var textarea = nextDiv.find("textarea").eq(0);
                            //
                            accountData = accountData.length == 0 ? data : accountData;
                            //文本框显示
                            textarea.removeAttr("destroyed").width(10).height(46).show().mAddCombobox("account", {
                                width: $td.innerWidth(),
                                height: $td.innerHeight() - 4,
                                data: data,
                                hasDownArrow: true,
                                onSelect: function (row) {
                                    //
                                    var event = that.getEvent();
                                    //
                                    if (event.type == "keydown") {
                                        //
                                        keydownEvent = true;
                                    }
                                    //
                                    var account = that.getAccountModel(textDiv);
                                    //
                                    if (fromDocVoucher) {
                                        //
                                        var sourceCurrency = that.getSourceCurrencyModel($(textDiv));
                                        //
                                        if (row && row.MIsBankAccount && row.MCurrencyID != sourceCurrency.MCurrencyID) {
                                            //
                                            mDialog.error(haveToChooseASameCurrencyBankAccount);
                                            //
                                            textarea.combobox("setValue", account.MItemID);
                                            //
                                            return that.stopPropagation(event);
                                        }
                                    }
                                    //如果是同一个科目，就不做处理了
                                    if (!account || account.MItemID != row.MItemID) {
                                        //
                                        var last = $.extend(true, {}, account);
                                        //
                                        that.setAccountModel(textDiv, row, fromDocVoucher);
                                        //处理是否显示币别列
                                        that.handleCurrencyTd($td, row, last);
                                        //初始核算维度是否可以选择
                                        that.handleCheckGroupTd($td, row, last);
                                        //
                                        that.handleAccountInfo(account, textDiv);
                                    }
                                    //跳到下一个单元格
                                    that.switch2Cell();
                                    //
                                    that.stopPropagation(event);
                                },
                                onLoadSuccess: function () {
                                    //
                                    var mCombo = new window.mCombobox(textarea);
                                    //comboInput
                                    var comboInput = mCombo.getInput();
                                    //
                                    var comboPanel = mCombo.getPanel();
                                    //重新计算宽度
                                    that.resizeCombobox($td, comboInput, comboPanel);
                                    //
                                    var account = that.getAccountModel(textDiv);
                                    //获取先前选择的值
                                    textarea.combobox("setValue", account.MItemID);
                                    //获取先前选择的值
                                    textarea.combobox("setText", account.MFullName);
                                    //聚焦
                                    mCombo.getInput().select().focus();
                                    //
                                    that.bindTableEvent();
                                }
                            },
                            {
                                hasPermission: 1,
                                //关闭后需要重新加载摘要
                                callback: function () {
                                    //重新获取摘要
                                    that.getAccountData(true, function (data) {
                                        //重新加载
                                        textarea.combobox("loadData", data);

                                        accountData = data;
                                    }, true);
                                }
                            });
                            //获取文本输入框
                            var $input = new window.mCombobox(textarea).getInput();
                            //将其他在焦点的元素置位不在焦点
                            that.vcBlurEvent();
                            //获取焦点，注册事件
                            that.vcFocusEvent($input,
                            function () {
                                //获取焦点，并且选中文本
                                $input.select().focus();
                                //
                                event.stopPropagation();
                            },
                            function () {
                                //获取输入的值
                                var selectedValue = textarea.combobox("getValue");
                                //选择的文本
                                var selectedText = textarea.combobox("getText");
                                //如果没有选中科目
                                if (!selectedText) {
                                    //
                                    var account = that.getAccountModel(textDiv);
                                    //
                                    account.MItemID = null;
                                    //
                                    account.MFullName = null;
                                }
                                //注销
                                $(textarea).combobox("destroy");

                                $(textarea).attr("destroyed", "1");
                                //
                                that.showAccountData(textDiv);
                                //把值给span
                                textDiv.show();
                                //在span下面再插入一个textarea，供下次使用
                                $(accountTextareaText).insertAfter(textDiv);
                                //
                                textDiv.height($td.height());
                                //
                                textDiv.width($td.width());
                            });
                        }
                        //更新
                        that.getAccountData(false, func, false);
                    }
                    else {
                        //
                        var nextDiv = textDiv.next("div");
                        //附近的那个文本输入框
                        var textarea = nextDiv.find("textarea").eq(0);
                        //
                        textarea.combobox("showPanel");
                    }
                    //组织冒泡
                    event.stopPropagation();
                    //
                    return false;
                });
            };
            //显示科目的信息
            this.showAccountData = function (div, account) {
                //
                account = account || that.getAccountModel(div);
                //
                that.setAccountModel(div, account);
                //
                div.html(account.MItemID ? account.MFullName : "").attr("etitle", mText.encode(account.MFullName)).tooltip({ content: mText.encode(account.MFullName) });
                //
                that.handleAccountInfo(account, div);
            };
            //处理科目如果非外币核算，在外币核算列展示一定的提示，如果没有核算维度，也在核算维度列展示一定的信息
            this.handleAccountInfo = function (account, textDiv) {
                //
                var tr = that.getParentNode(textDiv, "tr");
                //
                if ($(currencyDiv, tr).hasClass("tooltip-f")) {
                    //更新
                    $(currencyDiv, tr).tooltip("destroy");
                }
                //
                if ($(checkGroupDiv, tr).hasClass("tooltip-f")) {
                    //更新
                    $(checkGroupDiv, tr).tooltip("destroy");
                }
                //
                $(currencyDiv, tr).removeClass(disableTdClass).removeAttr("etitle");
                //
                $(checkGroupDiv, tr).removeClass(disableTdClass).removeAttr("etitle");
                //表示是正常的科目
                if (account && account.MItemID) {
                    //
                    if (!account.MIsCheckForCurrency) {
                        //
                        $(currencyDiv, tr).empty().addClass(disableTdClass).attr("etitle", accountHasNotForeignCurrencyLang);
                    }
                    //
                    if (account.MCheckGroupID == "0") {
                        //
                        $(checkGroupDiv, tr).empty().addClass(disableTdClass).attr("etitle", accountHasNotCheckTypeLang);
                    }
                }
            }
            this.getEvent = function (event) {
                return event || window.event || arguments.callee.caller.arguments[0];
            }
            this.getSourceCurrencyModel = function (div) {
                //
                return fromDocVoucher ? that.getParentNode(div, "tr").find(currencyDiv).data("MSourceCurrencyModel") : null;
            }
            this.setSourceCurrencyModel = function (div, currency) {
                //
                fromDocVoucher && that.getParentNode(div, "tr").find(currencyDiv).data("MSourceCurrencyModel", currency);
            }
            this.getSourceCheckGroupValueModel = function (div) {
                //
                return fromDocVoucher ? that.getParentNode(div, "tr").find(checkGroupDiv).data("MSourceCheckGroupValueModel") : null;
            }
            this.setSourceCheckGroupValueModel = function (div, checkGroupValue) {
                //
                fromDocVoucher && that.getParentNode(div, "tr").find(checkGroupDiv).data("MSourceCheckGroupValueModel", checkGroupValue);
            }
            //联系人输入框的事件
            this.initCurrencyTd = function ($td, entryData, lock) {
                //
                var account = that.getAccountModel($td);
                //
                var currency = account.MCurrencyDataModel;
                //
                if (!(!account.MItemID && isModule)) {
                    //
                    that.showCurrencyData($(currencyDiv, $td), currency);
                }
                //
                that.setSourceCurrencyModel($(currencyDiv, $td), currency);
                //注册事件
                !lock && $td.off("click.vc").on("click.vc",
                function (event) {
                    //获取文本显示span
                    var textDiv = $(currencyDiv, $td);

                    //如果已经在设置了,就不需要继续再设置了
                    if (!textDiv.hasClass(focusItem)) {
                        //将其他在焦点的元素置位不在焦点
                        that.vcBlurEvent();
                        //
                        var account = that.getAccountModel(textDiv);
                        //
                        if (!account || !account.MItemID) {
                            //
                            that.stopPropagation(event);
                            //
                            return that.switch2Cell(-1, $td);
                        }
                        //如果是从业务单据转过来的，则直接跳到下一个格子
                        if (fromDocVoucher) {
                            //
                            that.showCurrencyData(textDiv, that.getSourceCurrencyModel($(textDiv)));
                            //
                            that.stopPropagation(event);
                            //
                            return that.switch2Cell(1, $td);
                        }
                        //注销掉事件
                        $(currencySetupDiv).remove();
                        //
                        //如果需要有外币的话，才显示外币设置
                        if (account.MIsCheckForCurrency) {
                            //将其他在焦点的元素置位不在焦点
                            that.vcBlurEvent();
                            //克隆一个设置div
                            var setupDiv = $(currencySetupDemoDiv).clone();
                            //
                            setupDiv.appendTo("body");


                            that.locateCurrencySetupDiv($td, setupDiv);
                            //去除demo样式
                            setupDiv.removeClass(currencySetupDemoDiv.substring(1)).addClass(currencySetupDiv.substring(1));
                            //点击弹出
                            setupDiv.show();
                            //初始化里面的combobox 如果科目有币别，而且不等于本位币，那就是银行科目，必须限定不能选其他币别
                            that.initCurrencyDom(textDiv, setupDiv);
                            //初始化点击事件
                            that.bindCurrencyEvent(textDiv, setupDiv);
                            //获取焦点，注册事件
                            that.vcFocusEvent(textDiv,
                            function () {
                                //
                                textDiv.height($td.height() - 1);
                                //
                                textDiv.width($td.width() - 1);
                                //表格内部事件
                                that.bindTableEvent();
                            },
                            function () {
                                //
                                $(currencySetupDiv).remove();
                                //
                                textDiv.height($td.height() - 1);
                                //
                                textDiv.width($td.width() - 1);
                            });
                        }
                        else {
                            //获取焦点，注册事件
                            that.vcFocusEvent(textDiv,
                            function () {
                                //
                                textDiv.height($td.height() - 1);
                                //
                                textDiv.width($td.width() - 1);
                                //表格内部事件
                                that.bindTableEvent();
                            },
                            function () {
                                //
                                textDiv.height($td.height() - 1);
                                //
                                textDiv.width($td.width() - 1);
                            });
                            //直接跳到下一个
                            that.switch2Cell();
                        }

                        //失去焦点的时候
                        that.bindTableEvent();
                    }
                    //组织冒泡
                    event.stopPropagation();
                    //
                    return false;
                });
            };
            this.locateCurrencySetupDiv = function ($td, setupDiv) {

                var top = $td.offset().top + $td.height() + 2;

                if (top + setupDiv.height() > $("body").height()) {
                    top = $td.offset().top - setupDiv.height() - 2;
                }

                if (top < $(body).offset().top) {
                    setupDiv.hide();
                    setupDiv.data("hide", true);
                }
                else {
                    setupDiv.show();
                    setupDiv.css({
                        "left": $td.offset().left + 1 + "px",
                        "top": top + "px",
                        "width": $td.width() - 1 + "px"
                    });
                    setupDiv.data("hide", false);
                }
                setupDiv.data("td", $td);
            }
            //
            this.getAccountModel = function (dom, tr) {
                //
                return $(accountDiv, tr || that.getParentNode(dom, "tr")).data("account");
            }
            this.setAccountModel = function (dom, account, fromDoc) {
                //
                if (fromDoc) {
                    //
                    var lastAccount = that.getAccountModel(dom);
                    //
                    account.MCheckGroupValueModel = $.extend(true, {}, lastAccount.MCheckGroupValueModel);
                    //
                    account.MCurrencyDataModel = $.extend(true, {}, lastAccount.MCurrencyDataModel);
                    //如果是银行科目，或非外币核算的
                    if (account.MIsBankAccount || !account.MIsCheckForCurrency) {
                        //
                        account.MCurrencyDataModel.MCurrencyID = account.MCurrencyID;
                    }
                }
                //复制一个
                var newAccount = $.extend(true, {}, account);
                //
                $(accountDiv, that.getParentNode(dom, "tr")).data("account", newAccount);
            }
            //联系人输入框的事件
            this.initCheckGroupTd = function ($td, entryData, lock) {
                //
                var account = that.getAccountModel($td);
                //
                var checkGroupValueModel = account.MCheckGroupValueModel;
                //
                if (!(!account.MItemID && isModule)) {
                    //
                    that.showCheckGroupValueData($(checkGroupDiv, $td), account);
                }
                //
                that.setSourceCheckGroupValueModel($(checkGroupDiv, $td), checkGroupValueModel);
                //注册事件
                !lock && $td.off("click.vc").on("click.vc",
                function (event) {
                    //获取文本显示span
                    var textDiv = $(checkGroupDiv, $td);
                    //如果已经在设置了,就不需要继续再设置了
                    if (!textDiv.hasClass(focusItem)) {
                        //注销掉事件
                        $(currencySetupDiv).remove();
                        //
                        var account = that.getAccountModel(textDiv);
                        //
                        if (!account || !account.MItemID) {
                            //
                            that.stopPropagation(event);
                            //
                            return $(accountDiv, that.getParentNode($td, "tr")).trigger("click.vc");
                        }
                        //
                        //如果有核算维度的话才需要显示
                        if (account.MCheckGroupModel.MItemID != "0") {
                            //将其他在焦点的元素置位不在焦点
                            that.vcBlurEvent();
                            //克隆一个设置div
                            var setupDiv = $(checkGroupSetupDemoDiv).clone();
                            //
                            setupDiv = setupDiv.removeClass("vc-checkgroup-setup-demo-div").addClass("vc-checkgroup-setup-div");
                            //
                            setupDiv.appendTo("body");
                            //点击弹出
                            setupDiv.show().width($td.width() - 1);
                            //初始化里面的combobox 如果科目有币别，而且不等于本位币，那就是银行科目，必须限定不能选其他币别
                            that.initCheckGroupDom(textDiv, setupDiv);

                            //
                            that.showCheckGroupSetupDiv($td, setupDiv);
                            //
                            that.bindCheckGroupValue2Dom(textDiv, setupDiv);
                            //
                            that.selectCheckGroup(textDiv, setupDiv, null, []);
                            //
                            that.locateCheckgroupSetupDiv($td, setupDiv);
                            //表格内部事件
                            that.bindTableEvent();
                            //获取焦点，注册事件
                            that.vcFocusEvent(textDiv,
                            function () {
                                //
                                textDiv.height($td.height() - 1);
                                //
                                textDiv.width($td.width() - 1);
                                //表格内部事件
                                that.bindTableEvent();
                            },
                            function () {
                                //
                                if ($(checkGroupSetupDiv + ":visible").length > 0) {
                                    //
                                    that.selectCheckGroup(textDiv, $(checkGroupSetupDiv + ":visible"), null, []);
                                }
                                //
                                $(checkGroupSetupDiv).remove();
                                //
                                textDiv.height($td.height() - 1);
                                //
                                textDiv.width($td.width() - 1);
                            });
                        }
                        else {
                            //
                            that.vcBlurEvent();
                            //
                            if (!fromDocVoucher) {
                                //获取焦点，注册事件
                                that.vcFocusEvent(textDiv,
                                function () {
                                    //
                                    textDiv.width($td.width() - 1);
                                    //
                                    textDiv.height($td.height() - 1);
                                    //表格内部事件
                                    that.bindTableEvent();
                                },
                                function () {
                                    //
                                    textDiv.width($td.width() - 1);
                                    //
                                    textDiv.height($td.height() - 1);
                                });
                                //直接跳到下一个
                                that.switch2Cell(1, $td);
                            }
                        }

                        //失去焦点的时候
                        that.bindTableEvent();
                    }
                    //组织冒泡
                    event.stopPropagation();
                    //
                    return false;
                });
            };
            //
            this.showCheckGroupSetupDiv = function ($td, setupDiv) {
                //
                that.locateCheckgroupSetupDiv($td, setupDiv);
                //去除demo样式
                setupDiv.removeClass(checkGroupSetupDemoDiv.substring(1)).addClass(checkGroupSetupDiv.substring(1));

            }
            //
            this.locateCheckgroupSetupDiv = function ($td, setupDiv) {
                //
                var top = $td.offset().top + $td.height() + 2;
                //
                if (top + setupDiv.height() > $("body").height()) {
                    //
                    top = $td.offset().top - setupDiv.height() - 2;
                }

                if (top < $(body).offset().top) {
                    setupDiv.hide();
                    setupDiv.data("hide", true);
                }
                else {
                    setupDiv.show();
                    //
                    setupDiv.css({
                        "left": $td.offset().left + 1 + "px",
                        "top": top + "px"
                    });
                    setupDiv.data("hide", false);
                }
                //
                setupDiv.data("td", $td);
            }
            //借方金额输入框的事件
            this.initDebitTd = function ($td, entryData, lock) {
                //新建凭证模板时，点插入一行，MDebit为undefined，应设为0，避免反序列化报错 -- by linfq
                $(debitDiv, $td).data("value", entryData.MDebit || 0);
                //注册事件
                !fromDocVoucher && !lock && $($td).off("click.vc").on("click.vc",
                    function (event) {
                        //
                        if (that.getEventTarget(event).hasClass("vc-debit-input")) {
                            return false;
                        }
                        //获取文本显示span
                        var textDiv = $(debitDiv, $td);
                        //如果span是显示的
                        if (textDiv.is(":visible")) {
                            //
                            var account = that.getAccountModel(textDiv);
                            //
                            var currency = account.MCurrencyDataModel;
                            //
                            if (!account || !account.MItemID) {
                                //
                                that.stopPropagation(event);
                                //
                                return $(accountDiv, that.getParentNode($td, "tr")).trigger("click.vc");
                            }
                            //如果需要外币核算，并且没有选择币别，或者没有维护汇率
                            if (account && account.MItemID && account.MIsCheckForCurrency && (!currency.MCurrencyID || !currency.MExchangeRate)) {
                                //将其他在焦点的元素置位不在焦点
                                that.vcBlurEvent();
                                //
                                that.stopPropagation(event);
                                //
                                return $(currencyTd, that.getParentNode($td, "tr")).trigger("click.vc");
                            }
                            //本身隐藏
                            textDiv.hide();
                            //将其他在焦点的元素置位不在焦点
                            that.vcBlurEvent();
                            //
                            var nextDiv = textDiv.next("div");
                            //
                            nextDiv.width(10).show();
                            //附近的那个文本输入框
                            var input = nextDiv.find("input").eq(0);
                            //文本框显示
                            input.width(10).val(textDiv.data("value") || "").show().numberbox({
                                width: $td.width() - 2,
                                height: $td.height() - 2,
                                precision: 2
                            });
                            //调整宽度
                            that.resizeCombobox($td, input);
                            //获取文本输入框
                            var $input = input;
                            //获取焦点，注册事件
                            that.vcFocusEvent($input,
                            function () {
                                //
                                that.resizeTable();

                                try {
                                    //获取焦点，并且选中文本
                                    $input.select && $input.select();
                                } catch (e) {

                                }

                                $input.focus();
                                //
                                $input.click();
                            },
                            function () {
                                //获取输入的值
                                var value = $(this).val();
                                //
                                $(this).numberbox("destroy");
                                //注销
                                $(this).remove();
                                //显示
                                textDiv.show();
                                //如果不为空
                                if (value != "") {
                                    //把值给span
                                    textDiv.data("value", value);
                                    //
                                    that.handleOffsetValueShow(textDiv, value);
                                    //如果value的值大于零，则清空借方金额
                                    $(creditDiv, $td.parent()).data("value", null).empty();
                                } else {
                                    //把值给span
                                    textDiv.data("value", value).html("");
                                }
                                //在span下面再插入一个textarea，供下次使用
                                $(debitInputText).insertAfter(textDiv);
                                //
                                that.resizeTable();
                                //
                                that.handleUserInputAmount(textDiv, value);
                            });

                            //失去焦点的时候
                            that.bindTableEvent();
                        }
                        //组织冒泡
                        that.stopPropagation(event);
                        //
                        return false;
                    });
            };
            //
            this.handleUserInputAmount = function (textDiv, value) {
                //
                var account = that.getAccountModel(textDiv);
                //
                if (account.MIsCheckForCurrency) {
                    //
                    var currencyModel = account.MCurrencyDataModel;
                    //
                    if (currencyModel.MCurrencyID != GLVoucherHome.baseCurrencyID) {
                        //
                        account.MCurrencyDataModel.MAmount = value;
                    }
                    else {
                        //
                        account.MCurrencyDataModel.MAmountFor = value;
                        //
                        account.MCurrencyDataModel.MAmount = value;
                    }
                }
                //
                that.setAccountModel(textDiv, account);
            }
            //
            this.getEventTarget = function (e) {
                //
                return $(e.target || e.srcElement);
            }
            //贷方金额输入框的事件
            this.initCreditTd = function ($td, entryData, lock) {
                //新建凭证模板时，点插入一行，MCredit为undefined，应设为0，避免反序列化报错 -- by linfq
                $(creditDiv, $td).data("value", entryData.MCredit || 0);
                //注册事件
                !fromDocVoucher && !lock && $td.off("click.vc").on("click.vc",
                function (event) {
                    //
                    if (that.getEventTarget(event).hasClass("vc-credit-input")) {
                        return false;
                    }
                    //获取文本显示span
                    var textDiv = $(creditDiv, $td);
                    //如果span是显示的
                    if (textDiv.is(":visible")) {
                        //
                        var account = that.getAccountModel(textDiv);
                        //
                        var currency = account.MCurrencyDataModel;
                        //
                        if (!account || !account.MItemID) {
                            //
                            that.stopPropagation(event);
                            //
                            return $(accountDiv, that.getParentNode($td, "tr")).trigger("click.vc");
                        }
                        //如果需要外币核算，并且没有选择币别，或者没有维护汇率
                        if (account && account.MItemID && account.MIsCheckForCurrency && (!currency.MCurrencyID || !currency.MExchangeRate)) {
                            //
                            that.stopPropagation(event);
                            //
                            return $(currencyTd, that.getParentNode($td, "tr")).trigger("click.vc");
                        }
                        //本身隐藏
                        textDiv.hide();
                        //将其他在焦点的元素置位不在焦点
                        that.vcBlurEvent();
                        //
                        var nextDiv = textDiv.next("div");
                        //
                        nextDiv.width(10).show();
                        //附近的那个文本输入框
                        var input = nextDiv.find("input").eq(0);
                        //文本框显示
                        input.width(10).val(textDiv.data("value") || "").show().numberbox({
                            width: $td.width() - 2,
                            height: $td.height() - 2,
                            precision: 2
                        });
                        //调整宽度
                        that.resizeCombobox($td, input);
                        //获取文本输入框
                        var $input = input;
                        //获取焦点，注册事件
                        that.vcFocusEvent($input,
                        function () {
                            //
                            that.resizeTable();

                            try {
                                //获取焦点，并且选中文本
                                $input.select && $input.select();
                            } catch (e) {

                            }

                            $input.focus();
                            //
                            $input.click();
                        },
                        function () {
                            //获取输入的值
                            var value = $(this).val();
                            //
                            $(this).numberbox("destroy");
                            //注销
                            $(this).remove();
                            //显示
                            textDiv.show();
                            //如果不为空
                            if (value != "") {
                                //把值给span
                                textDiv.data("value", value);
                                //
                                that.handleOffsetValueShow(textDiv, value);
                                //如果value的值大于零，则清空借方金额
                                $(debitDiv, $td.parent()).data("value", null).empty();
                            } else {
                                //把值给span
                                textDiv.data("value", value).html("");
                            }
                            //在span下面再插入一个textarea，供下次使用
                            $(creditInputText).insertAfter(textDiv);
                            //
                            that.handleUserInputAmount(textDiv, value);
                        });
                        //失去焦点的时候
                        that.bindTableEvent();
                    }

                    //组织冒泡
                    that.stopPropagation(event);
                    //
                    return false;
                });
            };
            //
            this.fastCodeFormatter = function (data) {
                //
                return "<div class='vc-fastcode-combo'>" + "<div class='vc-fastcode-combo-code'>" + mText.encode(data.MFastCode) + "</div>" + "<div class='vc-fastcode-combo-desc'>" + mText.encode(data.MDescription) + "</div>" + "</div>";
            };
            //格式化那个group
            this.fastCodeGroupFormatter = function (data) {
                //
                return "<div class='vc-fastcode-combo-group-header vc-fastcode-combo'><div class='vc-fastcode-combo-group left'><span style='padding-left: 13px;'>" + fastCodeLang + "-" + descriptionLang + "</div></div>";
            };
            //金额输入框里面的事件
            this.initAmountInputDivEvent = function ($td) {
                //
                var func = function () {
                    var forValue = $(amountForInput, $td).val();
                    var rate = $(amountRateInput, $td).val();
                    var value = $(amountBaseInput, $td).val();
                    $(amountBaseInput, $td).numberbox("setValue", (forValue * rate).toFixed(6).toString());

                };
                //
                $(amountForInput, $td).numberbox({
                    min: 0,
                    height: 15,
                    width: $td.width() - 60,
                    precision: 6
                }).off("keyup.amt").on("keyup.amt", func);
                //
                $(amountRateInput, $td).numberbox({
                    min: 0,
                    height: 15,
                    width: $td.width() - 80,
                    precision: 6
                }).off("keyup.amt").on("keyup.amt", func);
                //
                $(amountBaseInput, $td).numberbox({
                    min: 0,
                    height: 15,
                    width: $td.width() - 80,
                    precision: 6
                });
            };
            //初始化每一行里面的具体细节
            this.initRowDetial = function ($row, rowData, lock) {
                //第0行
                that.initFastcodeTd($(fastcodeTd, $row), rowData, lock);
                //第一行的摘要 如果是从业务单据转的凭证，那么空行是不可以编辑的
                that.initExplanationTd($(explanationTd, $row), rowData, lock);
                //第二行科目
                that.initAccountTd($(accountTd, $row), rowData, lock);
                //第三行联系人
                that.initCurrencyTd($(currencyTd, $row), rowData, lock);
                //第四列 核算维度
                that.initCheckGroupTd($(checkGroupTd, $row), rowData, lock);
                //第十行借方金额 从业务单据转的凭证，金额不可以修改
                that.initDebitTd($(debitTd, $row), rowData, lock);
                //第一列贷方金额 从业务单据转的凭证，金额不可以修改
                that.initCreditTd($(creditTd, $row), rowData, lock);
                //绑定这一行的添加和删除事件
                !fromDocVoucher && !lock && $row.off("mouseover.vc").on("mouseover.vc", function () {
                    //显示删除
                    that.showNewDelete($row);
                    return true;
                });
            };
            //提醒用户单据是从业务系统而来不可编辑
            this.alertDocVoucher = function () {
                //
                if (fromDocVoucher) {
                    //提醒用户
                    mDialog.alert(cellDisabledFromDocVoucher);
                    //返回
                    return false;
                }
                return true;
            };
            //获取下一个可用的凭证编号，并且显示到编号输入框
            this.getNextVoucherNumber = function (date) {
                //
                date = date || $("#MDate").datebox("getValue");
                //
                var number = number || "";
                //
                var year = mDate.parse(date).getFullYear();
                //
                var period = mDate.parse(date).getMonth() + 1;
                //
                mAjax.post(getNextVoucherNumberUrl, {
                    year: year,
                    period: period
                },
                function (data) {
                    //如果是数字的话，那就是下一个合适的编号
                    if ($.isNumeric(data)) {
                        //
                        $("#MNumber").numberspinner("setValue", data);
                    } else {
                        //将其日修改成默认日期
                        $("#MDate").datebox("setValue", mDate.parse(defaultDate));
                        //期间不可在启用日期之前
                        var msg = data;
                        //提醒用户
                        mDialog.message(msg);
                    }
                })
            };

            /*绑定事件*/
            //绑定凭证模板表格里面的点击事件
            this.bindModuleTableEvent = function () {
                //会计科目勾选，其他的也勾选，取消，其他的也取消
                $(accountSelect).off("click.vc").on("click.vc", function () {
                    //
                    var checked = $(this).attr("checked");
                    //
                    if (!checked) {
                        //
                        $(this).removeAttr("checked");
                        //
                        $(this).removeAttr("value");
                        //
                        $(currencySelect).removeAttr("checked");
                        //
                        $(checkGroupSelect).removeAttr("checked");
                    }
                    else {
                        //
                        $(this).prop("checked", "true");
                        //
                        $(currencySelect).attr("checked", "checked");
                        //
                        $(checkGroupSelect).attr("checked", "checked");
                    }
                });
                //表格里面的单选框
                $("tr,td,th", fastcodeTableDiv).off("click.vc").on("click.vc", function (e) {
                    //
                    var src = e.target || e.srcElement;
                    //
                    if (!$(src).is("input")) {
                        return false;
                    }
                    else {
                        that.stopPropagation(e);
                    }
                });

            };
            //绑定表格内部键盘事件
            this.bindTableEvent = function () {
                //本身当有鼠标经过的时候当然要显示，离开的时候影藏
                $(newDeleteDiv).off("mouseover.table").on("mouseover.table",
                function () {
                    $(this).show();
                    return true;
                });

                /*以下是textarea 和input的事件*/
                //绑定键盘按键事件-回车
                $("textarea,input", tableBody).unbind("keydown.return").bind("keydown.return", "return",
                function (e) {
                    //
                    if (!keydownEvent) {
                        //
                        keydownEvent = true;
                        //
                        that.switch2Cell();
                    }
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });

                //绑定键盘按键事件-tab
                $("textarea,input", tableBody).unbind("keydown.tab").bind("keydown.tab", "tab",
                function (e) {
                    //
                    that.switch2Cell();
                    //
                    keydownEvent = true;
                    //
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });

                //绑定键盘按键事件-回车
                $("textarea,input", tableBody).unbind("keydown.shiftreturn").bind("keydown.shiftreturn", "shift+return",
                function (e) {
                    that.switch2Cell(-1);
                    //
                    keydownEvent = true;
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });

                //绑定键盘按键事件-tab
                $("textarea,input", tableBody).unbind("keydown.shifttab").bind("keydown.shifttab", "shift+tab",
                function (e) {
                    that.switch2Cell(-1);
                    //
                    keydownEvent = true;
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });

                //摘要内部的双斜杠事件
                $("textarea", ".vc-explanation").unbind("keyup.ss").bind("keyup.ss",
                function (e) {
                    //如果输入的事两个斜杠
                    if ($(this).val() == "//") {
                        //
                        that.copyPrevExplanation();
                    } else if ($(this).val() == "..") {
                        //
                        that.copyFirstExplanation();
                    }
                });
                //借贷方里面的空格事件
                $("input", debitTd + "," + creditTd).unbind("keyup.space").bind("keyup.space", "space",
                function (e) {
                    if (e.type == "postpaste") return true;
                    //交换数据
                    that.switchDebitCredit();
                });

                //给输入框加入数据，允许添加的符号
                $("input", debitTd + "," + creditTd).each(function () {

                    //获取已保存的属性
                    var allowChar = $(this).data("allowChar");

                    //如果为空则初始化，如果不为空则加入
                    if (!allowChar) allowChar = ['='];
                    else if (!allowChar.contains('=')) allowChar.push('=');

                    $(this).data("allowChar", allowChar);
                });

                //借贷方里面的=事件
                $("input", debitTd + "," + creditTd).off("keyup.equal").on("keyup.equal",
                function (e) {
                    if (e.type == "postpaste") return true;
                    if (!!this.value && this.value.indexOf("=") >= 0 || e.which === 61) {
                        //平衡借贷
                        that.balanceDebitCredit();
                    }
                });

                //借贷方里面的向上事件
                $("input", debitTd + "," + creditTd).unbind("keyup.up").bind("keyup.up", "up",
                function (e) {
                    if (e.type == "postpaste") return true;
                    //交换数据
                    that.upDownSwitchDebitCredit(1);
                });
                //借贷方里面的向下事件
                $("input", debitTd + "," + creditTd).unbind("keyup.down").bind("keyup.down", "down",
                function (e) {
                    if (e.type == "postpaste") return true;
                    //交换数据
                    that.upDownSwitchDebitCredit(-1);
                });
                //保存
                $("textarea,input").unbind("keydown.tablectrls").bind("keydown.tablectrls", "ctrl+s",
                function (e) {
                    if (e.type == "postpaste") return true;
                    //
                    if ($(this).hasClass(focusItem) || $(fastcodeTableDiv).is(":visible")) {
                        //保存，并且再次初始化
                        that.saveVoucherOrModule(that.refreshVoucherPage);
                    }
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });

                //保存并新增
                $("textarea,input", tableBody).unbind("keyup.tablef12").bind("keyup.tablef12", "F12",
                    function (e) {
                        if (e.type == "postpaste") return true;
                        //
                        if ($(this).hasClass(focusItem)) {
                            //
                            that.saveAndNewVoucherOrModule(null);
                        }
                        //阻止冒泡事件
                        return that.stopPropagation(e);
                    });
                //保存并审核
                $("textarea,input", tableBody).unbind("keyup.tablef11").bind("keyup.tablef11", "F11",
                function (e) {
                    if (e.type == "postpaste") return true;
                    //
                    if ($(this).hasClass(focusItem) && $("#btnSaveAndApprove").length > 0) {
                        //
                        that.saveAndApproveVoucher(that.refreshVoucherPage);
                    }
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });
                //保存为模板
                $("textarea,input", tableBody).unbind("keydown.tablef10").bind("keydown.tablef10", "F10",
                function (e) {
                    if (e.type == "postpaste") return true;
                    //
                    if ($(this).hasClass(focusItem)) {
                        //
                        $("#btnSaveAsModule").trigger("click.save");
                    }
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });
            };
            //将页面刷新
            this.refreshVoucherPage = function (voucher) {
                //禁用掉按钮
                that.enableEditVoucher({
                    MSettlementStatus: approved,
                    MStatus: approved,
                    MNoHideFastCode: true
                });
                //保存为模板也禁用掉
                $("#btnSaveAsModule").linkbutton("disable");
                //如果是从期末结转过来的，保存后就关闭窗口
                if (fromPeriodTransfer && window.parent.$(".mCloseBox").length != 0) {
                    //直接关闭
                    window.parent.$(".mCloseBox").trigger("click");
                }
                //如果是弹窗
                if (window.parent.$(".mCloseBox").length != 0) {
                    //直接重新加载次页面就行了
                    mWindow.reload()
                } else {
                    //
                    if (fromPeriodTransfer && fromDocVoucherSetting) {
                        //关闭当前页面
                        $.mTab.remove();
                    } else if (isModule) {
                        //跳转到编辑页面 
                        $.mTab.refresh(voucherModuleLang + "[" + voucher.MFastCode + "]", editVoucherUrl + "?MItemID=" + voucher.MItemID + "&IsModule=1", true);
                    } else {
                        //
                        $.mTab.refresh(GL + "-" + voucher.MNumber, editVoucherUrl + "?MItemID=" + (voucher.MItemID || ""), true);
                    }
                }
            };
            //绑定删除和添加的事件
            this.bindNewDeleteEvent = function ($tr) {
                //获取下一行
                var $nextTr = $tr.next("tr:not(" + tableTotal + ")");
                //上一行
                var prevTr = $tr.prev("tr");
                //
                $(newHref).data("tr", $tr);
                //添加事件
                $(newHref).off("click.new").on("click.new",
                function () {
                    //
                    that.insertOneRowByHref($tr);
                    //
                    event.stopPropagation();
                });
                //删除事件
                $(deleteHref).off("click.delete").on("click.delete",
                function (event) {
                    //
                    that.deleteOneRowByHref($tr, $nextTr, event);
                    //
                    $(newDeleteDiv).hide();
                    //
                    event.stopPropagation();
                });
            };
            //表格内部快捷键事件
            this.bindTableKeyEvent = function (event) {
                //如果是回车键
                switch (event.which) {
                    //单独的回车键
                    case 13:
                        that.switch2Cell(event);
                        //返回
                    default:
                        break;
                }
            };
            //回车事件
            this.bindTableKeyEvent_Enter = function (event) {
                //获取当前焦点的td
                var nextTd = that.getNextNode($("." + focusItem), "td");
                //触发点击事件
                nextTd.trigger("click.vc");
            };
            //初始化币别的点击事件
            this.bindCurrencyEvent = function (div, setupDiv) {
                //内部的金额输入框输入数字后，借贷方向的值跟着改变
                $(currencyAmount + "," + currencyRate + "," + currencyAmountFor, setupDiv).off("keyup.vc").on("keyup.vc",
                function () {
                    //必须要选择了币别
                    if ($(currencyId, setupDiv).combobox("getValue")) {
                        //
                        that.handleDebitCreditByRate(div, setupDiv, $(this));
                    }
                });
            };
            //
            this.refreshVoucherModulePage = function () {
                //
                mTab.refresh(HtmlLang.Write(LangModule.GL, "FastCode", "快速码管理"), '/FC/FCHome/FCHome', false, true);
            }
            //绑定与凭证状态无关的事件
            this.bindModuleDocumentEvent = function () {
                //保存为模板
                $(document).unbind("keydown.f10").bind("keydown.f10", "F10",
                function (e) {
                    //
                    that.showSaveAsModule(that.refreshVoucherPage);
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });
                //保存为模板
                $("#btnSaveAsModule").off("click.save").on("click.save",
                function () {
                    //保存并初始化
                    that.showSaveAsModule(that.refreshVoucherPage);
                });
                //再非模板状态下保存
                $("#btnModuleSave").off("click.save").on("click.save",
                function () {
                    //保存并初始化
                    that.saveModule(function () {
                        //隐藏就行
                        that.showFastCodeSetup(false);
                        //当前操作为凭证
                        operateTarget = VOUCHER;
                        //
                        that.refreshVoucherModulePage();
                    });
                });
                $("#btnModuleCancel").off("click.cancel").on("click.cancel",
                function () {
                    //保存并初始化
                    that.cancelShowSaveAsModule();
                });
                //取消
                $("textarea,input", fastcodeSetupDiv).unbind("keyup.tablealtc").bind("keyup.tablectrlc", "alt+c",
                function (e) {
                    if (e.altKey && e.key == "c") {
                        //取消
                        that.cancelShowSaveAsModule();
                        //阻止冒泡事件
                        return that.stopPropagation(e);
                    }
                });
                //绑定键盘按键事件-ctrl + c 取消保存为模板
                $(document).unbind("keyup.altc").bind("keyup.altc", "alt+c",
                function (e) {
                    if (e.altKey && e.key == "c") {
                        //取消
                        that.cancelShowSaveAsModule();
                        //阻止冒泡事件
                        return that.stopPropagation(e);
                    }
                });
            };
            //绑定document的事件
            this.bindDocumentEvent = function (voucher) {
                //
                var draftStatus = (voucher.MStatus != approved);
                /*以下是document的事件*/
                //绑定键盘按键事件-ctrl + s 保存 
                draftStatus && $(document).unbind("keydown.ctrls").bind("keydown.ctrls", "ctrl+s",
                function (e) {
                    //保存并初始化
                    that.saveVoucherOrModule(that.refreshVoucherPage);
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });
                //保存并新增
                draftStatus && !(isPeriodTransfer || fromPeriodTransfer) && $(document).unbind("keydown.f12").bind("keydown.f12", "F12",
                function (e) {
                    //
                    that.saveAndNewVoucherOrModule(null);
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });

                //保存并审核
                draftStatus && $(document).unbind("keydown.f11").bind("keydown.f11", "F11",
                function (e) {
                    //
                    $("#btnSaveAndApprove").length > 0 && $("#btnSaveAndApprove").trigger("click.save");
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });
                //Ctrl + F7 自动平衡借贷方金额
                draftStatus && $(document).unbind("keydown.ctrlf7").bind("keydown.ctrlf7", "ctrl+F7",
                function (e) {
                    //阻止冒泡事件
                    return that.stopPropagation(e);
                });
                //空格键 借贷方金额互换
                draftStatus && $(document).unbind("click.top").bind("click.top",
                function (e) {
                    //如果是新增某个类型的点击，则不需要触发失焦事件
                    var target = that.getEventTarget(e);
                    //
                    if ($(target).parents(currencySetupDiv + "," + checkGroupSetupDiv).length > 0
                        || $(target).hasClass(currencySetupDiv.trimStart('.'))
                        || $(target).hasClass("add-combobox-item")
                        || $(target).hasClass("add-combotree-item")
                        || $(target).hasClass("search-combobox-item")
                        || ($(target).parent() && $(target).parent().hasClass("add-combobox-item"))
                        || ($(target).parent() && $(target).parent().hasClass("add-combotree-item"))
                        || ($(target).parent() && $(target).parent().hasClass("search-combobox-item"))) {
                        //阻止冒泡事件
                        return that.stopPropagation(e);
                    }
                    //取消焦点事件
                    that.vcBlurEvent();
                });
                //
                draftStatus && $(document).off("keyup.m").on("keyup.m", function (e) {

                    //
                    if ($("." + focusItem).is(":visible") && $("." + focusItem).is("div")) {
                        //
                        var target = that.getEventTarget(e);
                        //
                        if (target.is("textarea") || target.hasClass("vc-debit-input") || target.hasClass("vc-credit-input ")) {
                            //
                            that.stopPropagation(e);
                            //
                            return false;
                        }
                        //
                        if (keydownEvent) {
                            //
                            keydownEvent = false;
                            //
                            return false;
                        }
                        if (e.keyCode == 37 || (e.keyCode == 13 && e.shiftKey) || (e.keyCode == 9 && e.shiftKey)) {
                            //
                            var target = that.getEventTarget(e);
                            //
                            if (target.is("input") && target.parents(".vc-currency-setup-div").length > 0) {
                                //阻止冒泡事件
                                return that.stopPropagation(e);
                            }
                            //left
                            that.switch2Cell(-1);
                        }
                            //right
                        else if (e.keyCode == 39 || e.keyCode == 13 || e.keyCode == 9) {
                            //
                            var target = that.getEventTarget(e);
                            //
                            if (target.is("input") && target.parents(".vc-currency-setup-div").length > 0) {
                                //阻止冒泡事件
                                return that.stopPropagation(e);
                            }
                            //
                            that.switch2Cell(1);
                        }

                        //
                        that.stopPropagation(e);
                    }

                    //
                    if (keydownEvent) {
                        //
                        keydownEvent = false;
                    }
                });
                //把xytipswindow去掉click事件
                $(document).off("click.top", ".XYTipsWindow").on("click.top", ".XYTipsWindow",
                function (e) {
                    //组织冒泡事件
                    return that.stopPropagation(e);
                })
                //保存
                draftStatus && $("textarea,input").unbind("keydown.docctrls").bind("keydown.docctrls", "ctrl+s",
                function (e) {
                    //保存并初始化
                    that.saveVoucherOrModule(that.refreshVoucherPage);
                    //阻止事件冒泡
                    return false;
                });
                //保存并新增
                draftStatus && !(isPeriodTransfer || fromPeriodTransfer) && $("textarea,input").unbind("keydown.docf12").bind("keydown.docf12", "F12",
                function (e) {
                    //
                    that.saveAndNewVoucherOrModule(null);
                    //组织冒泡
                    return false;
                });
                //保存并新增
                draftStatus && !(isPeriodTransfer || fromPeriodTransfer) && $("#btnSaveAndAddAnother").off("click.save").on("click.save",
                function () {
                    //
                    that.saveAndNewVoucherOrModule(null);
                });
                //保存并新增
                draftStatus && $("#btnSaveAndApprove").off("click.save").on("click.save",
                function () {
                    //保存
                    that.saveAndApproveVoucher(that.refreshVoucherPage);
                });
                //保存
                draftStatus && $("#btnSave").off("click.save").on("click.save",
                function () {
                    //保存并初始化
                    that.saveVoucherOrModule(that.refreshVoucherPage);
                });
                //键盘图片点击
                draftStatus && $(hotkeyDiv).mTip({
                    target: $(hotkeyTableDiv),
                    width: 340,
                    parent: $(hotkeyTableDiv).parent()
                });

                $(body).off("scroll.vc").on("scroll.vc", function () {
                    var currencySet1 = null;

                    if ($(currencySetupDiv).is(":visible")) {
                        currencySet1 = $(currencySetupDiv + ":visible");
                    }
                    else {
                        for (var i = 0; i < $(currencySetupDiv).length ; i++) {
                            if ($(currencySetupDiv).eq(i).data("hide")) {
                                currencySet1 = $(currencySetupDiv).eq(i);
                                break;
                            }
                        }
                    }
                    if (currencySet1) {

                        that.locateCurrencySetupDiv(currencySet1.data("td"), currencySet1);
                    }
                    var checkGroupSet1 = null;


                    if ($(checkGroupSetupDiv).is(":visible")) {
                        checkGroupSet1 = $(checkGroupSetupDiv + ":visible");
                    }
                    else {
                        for (var i = 0; i < $(checkGroupSetupDiv).length ; i++) {
                            if ($(checkGroupSetupDiv).eq(i).data("hide")) {
                                checkGroupSet1 = $(checkGroupSetupDiv).eq(i);
                                break;
                            }
                        }
                    }
                    if (checkGroupSet1) {

                        that.locateCheckgroupSetupDiv(checkGroupSet1.data("td"), checkGroupSet1);
                    }
                });
            };
            //组织冒泡事件
            this.stopPropagation = function (e) {
                //
                e = e || that.getEvent();
                //组织冒泡事件
                if (e.stopPropagation) {
                    // this code is for Mozilla and Opera 
                    e.stopPropagation();
                }
                else if (window.event) {
                    // this code is for IE 
                    window.event.cancelBubble = true;
                }
                //
                return false;
            };
            //取消其他的焦点
            this.vcBlurEvent = function (hideTip) {
                //获取元素
                $("." + focusItem).removeClass(focusItem).trigger(vcBlurEvent);
                //
                that.calculateTotal();
                //隐藏模板提醒
                $(modulePreviewDiv).hide();
            };
            //获取焦点
            this.vcFocusEvent = function (input, userFocus, userBlur) {
                $(".m-tip-top-div").hide();
                //绑定失去焦点
                input.addClass(focusItem);
                //如果focus有事件，则激活
                userFocus && $.isFunction(userFocus) && userFocus();
                //如果失去焦点又事件
                userBlur && $.isFunction(userBlur) && input.off(vcBlurEvent).on(vcBlurEvent, userBlur);
            };
            //显示下一张，或者上一张凭证
            this.selectVoucher = function (dir) {
                //默认是选择下一张
                dir = dir || 1;
                //
                mTab.addOrUpdate("GL-" + voucherData.MNumber, editVoucherUrl + "?MItemID=" + itemID + "&" + "MDir=" + dir + "&MNumber=" + voucherData.MNumber + "&year=" + voucherData.MYear + "&period=" + voucherData.MPeriod, false, true, true);
            };
            //查看原凭证或者冲销凭证
            this.viewReverseVoucher = function () {

                mTab.addOrUpdate("GL-" + voucherData.MNumber, editVoucherUrl + "?MItemID=" + voucherData.MRVoucherID);
            }
            /*表格内部操作*/
            //添加一行
            this.insertOneRow = function (status, settled, rowData, behindRow) {
                //如果没有数据则为空
                rowData = mText.encode(rowData || {});
                //如果没有bihindRow
                behindRow = behindRow || tableHeader;
                //
                var rowText = "<tr class='vc-entry-row' mentryid=" + (rowData.MEntryID || "") + ">";
                //第0列 快速码 
                var rowFastCode = fromDocVoucher ? "" : "<td class='vc-fastcode change-item' rowwidth='" + fastcodeWidth + "'><div class='vc-fastcode-div' value='" + (rowData.MFastCode || "") + "' etitle='" + mText.encode(rowData.MFastCode || "") + "'>" + mText.encode(rowData.MFastCode || "") + "</div>" + fastcodeTextareaText + "</td>"
                //第一列 摘要 MExplanation
                var rowExplanation = "<td class='vc-explanation change-item' rowwidth='" + explanationWidth + "'><div class='vc-explanation-div' value='" + mText.encode(rowData.MExplanation || "") + "' etitle='" + mText.encode(rowData.MExplanation || "") + "'>" + mText.encode(rowData.MExplanation || "").replace(/\n/gi, '<br>') + "</div>" + explanationTextareaText + "</td>";
                //第二列 科目
                var rowAccount = "<td class='vc-account' rowwidth='" + accountWidth + "' > <div class='vc-account-div'></div>" + accountTextareaText + "</td>";
                //第三列 币别列
                var rowCurrency = "<td class='vc-currency change-item' rowwidth='" + currencyWidth + "' ><div class='vc-currency-div'  etitle=''></div></td>";
                //第四列是核算维度列
                var rowCheckGroup = "<td class='vc-checkgroup change-item' rowwidth='" + checkGroupWidth + "' ><div class='vc-checkgroup-div'></div></td>";
                //
                var debit = rowData.MDC == 1 ? rowData.MDebit : "";
                //
                var credit = rowData.MDC == -1 ? rowData.MCredit : "";
                //第十借方金额
                var rowDebit = "<td class='vc-debit' rowwidth='" + debitWidth + "'><div class='vc-debit-div change-item' value='" + debit + "'>" + mMath.toMoneyFormat(debit) + "</div>" + debitInputText + "</td>";
                //第十一贷方金额
                var rowCredit = "<td class='vc-credit' rowwidth='" + creditWidth + "'><div class='vc-credit-div change-item' value='" + credit + "'>" + mMath.toMoneyFormat(credit) + "</div>" + creditInputText + "</td>";
                //拼接起来
                rowText += (rowFastCode + rowExplanation + rowAccount + rowCurrency + rowCheckGroup + rowDebit + rowCredit);
                //结尾
                rowText += "</tr>";
                //
                var $row = $(rowText).insertAfter(behindRow);
                //
                var lock = status == approved;
                //初始化行
                that.initRowDetial($row, rowData, lock);
                //
                return $row;
            };
            //
            this.getCheckGroupText = function (checkGroupValue) {
                //
                var html = "";
                //

            }
            //获取一行空的分录
            this.getEmptyEntry = function () {
                //
                return {
                    MAccountModel: {
                        MCurrencyDataModel: {
                            MCurrencyID: GLVoucherHome.baseCurrencyID
                        },
                        MCheckGroupModel: {},
                        MCheckGroupValueModel: {}
                    }
                }
            }
            //添加个空行
            this.insertOneRowByHref = function ($tr) {
                //
                var newTr = that.insertOneRow(unapproved, unapproved, that.getEmptyEntry(),
                $tr);
                //
                that.resizeAdvanceOptionSize();
                //
                that.resizeTable();
                //调整高度
                that.adjustAdvOptionsDom();
                //
                return newTr;
            };
            //删除一行
            this.deleteOneRowByHref = function ($tr, $nextTr, event) {
                //
                that.deleteOneRow($tr);
                //重新计算
                that.calculateTotal();
                //如果其下面有行，则重新绑定事件
                if ($nextTr.length == 1) {
                    //绑定
                    that.bindNewDeleteEvent($nextTr);
                }
                //
                that.resizeAdvanceOptionSize();
                //真个div的高度
                that.resizeBody();
                //
                event.stopPropagation();
            };
            //更新一行
            this.insertModuleRows = function (rows, $td) {
                //根据rowData的数量，如果是一行，就放入到本行，如果是多行，就在下方加入多行
                var $tr = $td.parent();
                //
                for (var i = 0; i < rows.MVoucherModuleEntrys.length; i++) {
                    //
                    that.updateOneRow(rows.MVoucherModuleEntrys[i], $tr);
                    //找到下一行
                    var nextTr = $tr.next("tr.vc-entry-row");
                    //如果是空行
                    if (nextTr.length > 0 && that.isRowEmpty(nextTr)) {
                        //直接拿来用
                        $tr = nextTr;
                    }
                        //添加一个空行,如果还有就再添加一行
                    else if (i != rows.MVoucherModuleEntrys.length - 1) {
                        //
                        $tr = that.insertOneRowByHref($tr);
                    }
                }
                //
                that.vcBlurEvent();
            };
            //更新一行
            this.updateOneRow = function (rowData, $tr) {
                //更新摘要
                $(explanationDiv, $tr).mHtml(rowData.MExplanation || "").data("value", rowData.MExplanation || "");
                //
                that.showAccountData($(accountDiv, $tr), rowData.MAccountModel);
                //更新币别信息
                rowData.MAccountID && that.showCurrencyData($(currencyDiv, $tr), rowData.MAccountModel.MCurrencyDataModel);
                //显示核算维度数据
                rowData.MAccountID && that.showCheckGroupValueData($(checkGroupDiv, $tr), rowData.MAccountModel);
                //更新借方
                $(debitDiv, $tr).text(rowData.MDC == -1 ? "" : mMath.toMoneyFormat(rowData.MDebit)).data("value", rowData.MDebit);
                //更新贷方
                $(creditDiv, $tr).text(rowData.MDC == 1 ? "" : mMath.toMoneyFormat(rowData.MCredit)).data("value", rowData.MCredit);
            }
            //删除一行
            this.deleteOneRow = function (tr) {
                //至少保证有一行
                var rowCount = $(tableBody + " tr").length;
                //如果多语1行
                if (rowCount > 3) {
                    //直接删除
                    $(tr).remove();
                    //
                    that.resizeAdvanceOptionSize();
                    //
                    that.resizeTable();
                    //调整高度
                    that.adjustAdvOptionsDom();
                }
            };
            //显示一个添加和删除按钮，在行的左侧，首先要保证，正在编辑的列不在本行
            this.showNewDelete = function (row) {
                //
                var editTr = $("." + focusItem).length > 0 ? that.getParentNode($("." + focusItem), "tr") : [];
                //先显示
                $(newDeleteDiv).show();
                //再定位
                that.locateNewDeleteSpan(row);
                //}
            };
            //定位删除和添加按钮的位置
            this.locateNewDeleteSpan = function ($tr) {
                //
                $(newDeleteDiv).css({
                    height: $tr.height() + "px",
                    top: ($tr.offset().top) + "px",
                    left: ($tr.offset().left - $(newDeleteDiv).width() + 4) + "px"
                });
                //删除按钮，要放到最底部
                $(deleteHref, newDeleteDiv).css({
                    "margin-top": ($(newDeleteDiv).height() - $(deleteHref, newDeleteDiv).height() * 2 - 1) + "px"
                });
                //需要定位的时候代表需要显示了，自然要绑定事件
                that.bindNewDeleteEvent($tr);
            };
            //初始借贷方冲销的问题
            this.handleOffsetValueShow = function (textDiv, value) {
                //
                textDiv.html(mMath.toMoneyFormat(Math.abs(value)));
                //
                textDiv.data("value", value);
                //如果值小于0，则显示成红色
                that.addRedClass(textDiv, value < 0);
            };
            //处理联系人是否可选
            this.handleCurrencyTd = function (td, account, last) {
                //
                if (!last || (last.MCurrencyDataModel.MCurrencyID != account.MCurrencyDataModel.MCurrencyID)) {
                    //先清理数据
                    $(currencyDiv, that.getParentNode(td, "tr")).empty();
                }
            };
            //处理联系人是否可选
            this.handleCheckGroupTd = function (td, account, last) {
                //
                if (!last || last.MCheckGroupID != account.MCheckGroupID) {
                    //先清理数据
                    $(checkGroupDiv, that.getParentNode(td, "tr")).empty();
                    //
                    that.showCheckGroupValueData($(checkGroupDiv, that.getParentNode(td, "tr")), account);
                }
            };

            //初始化币种与数额的方法
            this.initCurrencyDom = function (textDiv, div) {
                //
                var account = that.getAccountModel(textDiv);
                //
                var currency = that.getSourceCurrencyModel($(textDiv)) || account.MCurrencyDataModel;
                //
                var currencyID = currency.MCurrencyID;

                //初始化汇率输入
                $(currencyRate, div).numberbox({
                    min: 0,
                    width: 120,
                    height: 20,
                    precision: 6
                });

                //如果entryData有值
                $(currencyRate, div).numberbox("setValue", currency.MExchangeRate);
                //初始化金额输入
                $(currencyAmount, div).numberbox({
                    width: 120,
                    height: 20,
                    precision: 2
                });
                //如果entryData有值
                $(currencyAmount, div).numberbox("setValue", currency.MAmount);
                //初始化金额输入
                $(currencyAmountFor, div).numberbox({
                    width: 120,
                    height: 20,
                    precision: 2
                });
                //如果entryData有值
                $(currencyAmountFor, div).numberbox("setValue", currency.MAmountFor);
                //
                if (currencyID == GLVoucherHome.baseCurrencyID) {
                    //汇率不可编辑
                    $(currencyRate, div).numberbox("disable");
                    //原币不可编辑
                    $(currencyAmountFor, div).numberbox("disable");
                }
                else {
                    //汇率不可编辑
                    $(currencyRate, div).numberbox("enable");
                    //原币不可编辑
                    $(currencyAmountFor, div).numberbox("enable");
                }
                if (isFinalTransfer) {
                    //初始化汇率输入
                    $(currencyRateTr, div).hide();
                    //
                    $(currencyAmountTr, div).hide();
                }
                //初始化比别选择
                $(currencyId, div).combobox($.extend(eval("({" + $(currencyId, div).attr("data-values") + "})"), {
                    min: 0,
                    width: 120,
                    height: 29,
                    onSelect: function (row) {
                        //
                        var account = that.getAccountModel(textDiv);
                        //
                        var date = $("#MDate") && $("#MDate").length > 0 ? $("#MDate").datebox("getValue") : mDate.format(new Date());
                        //
                        that.getCurrencyRate(row.CurrencyID, GLVoucherHome.baseCurrencyID, date, textDiv);
                    },
                    //加载成功
                    onLoadSuccess: function () {
                        //在科目有币别并且不等于本位币的情况下，那肯定是银行科目的外币了
                        var isBankAccount = account.MIsBankAccount;
                        //对于已经选好的，则不需要再切换了
                        if (currency.MCurrencyID && currency.MExchangeRate > 0) {
                            //如果有币别
                            $(currencyId, div).combobox("setValue", currency.MCurrencyID);
                        }
                        else {
                            //
                            $(currencyId, div).combobox("select", currencyID || GLVoucherHome.baseCurrencyID);
                        }
                        //
                        if (isBankAccount) {
                            //
                            $(currencyId, div).combobox("disable");

                            //
                            $(currencyAmountFor, div).focus();
                        }
                        else {
                            $(currencyId, div).combobox("enable");
                            //
                            $(currencyAmount, div).focus();
                        }
                    }
                }));
            };
            //初始化币种与数额的方法
            this.initCheckGroupDom = function (textDiv, div, account) {
                //获取表格
                var table = $(checkGroupTable, div);
                //
                account = account || that.getAccountModel(textDiv);
                //
                var checkGroupModel = account.MCheckGroupModel;
                //当前核算维度的下标
                var checkGroupIndex = 0;
                //
                var checkGroupTitleDoms = $(checkGroupTitle, table);
                //
                var checkGroupValueDoms = $(checkGroupValue, table);
                //
                width = div.width() * 0.6 - 10;
                //
                var options = {
                    width: width,
                    height: 20,
                    onSelect: function (row, select) {

                        //
                        that.selectCheckGroup(textDiv, div, row, select);
                        //
                        return that.locateCheckgroupSetupDiv(that.getParentNode(textDiv, "td"), div);
                    }
                }
                //
                checkType.bindCheckTypes2Doms(checkGroupModel, checkGroupTitleDoms, checkGroupValueDoms, options);
                //把里面有值得td显示出来
                that.showThWithData(table);
            };
            //
            this.showCheckGroupValueData = function (div, account) {
                //
                var account = account || that.getAccountModel(div);
                //
                var checkGroupValueModel = that.getSourceCheckGroupValueModel(div) || account.MCheckGroupValueModel
                //
                var html = checkType.getCheckGroupValueHtml(account.MCheckGroupValueModel, account.MCheckGroupModel);
                //
                !div.hasClass(disableTdClass) && div.empty().html(html);
            }
            //选择了核算维度，表格里面的内容也要跟着变化
            this.selectCheckGroup = function (textDiv, div, row, select) {
                //把表格里面所有的选择框度拿出来
                var checkGroupValueDoms = $(checkGroupValue, div);
                //
                var checkGroupValueModel = {};
                //
                for (var i = 0 ; i < checkGroupValueDoms.length ; i++) {
                    //
                    var input = checkGroupValueDoms.eq(i)
                    //
                    data = input.data("data");
                    //
                    var id = select[0] == input[0] ? row.id : (data.MShowType == 2 ? input.combotree("getValue") : input.combobox("getValue"))
                    //
                    var text = select[0] == input[0] ? row.text : (data.MShowType == 2 ? input.combotree("getText") : input.combobox("getText"));
                    //
                    if (text.length == 0) {
                        //
                        id = "";
                    }
                    //
                    checkGroupValueModel[data.MCheckTypeColumnName] = id;
                    //
                    checkGroupValueModel[data.MCheckTypeColumnName.replace("ID", "") + "Name"] = text;
                    //
                    checkGroupValueModel[data.MCheckTypeColumnName.replace("ID", "") + "GroupName"] = data.MCheckTypeName;
                    //
                    checkGroupValueModel[data.MCheckTypeColumnName.replace("ID", "") + "GroupID"] = data.MCheckTypeGroupID;
                }

                //
                var account = that.getAccountModel(textDiv);
                //
                var html = checkType.getCheckGroupValueHtml(checkGroupValueModel, account.MCheckGroupModel);
                //
                textDiv.empty().html(html);
                //
                account.MCheckGroupValueModel = checkGroupValueModel;
                //
                that.setAccountModel(textDiv, account);
            }
            //根据核算维度的组的值，把对应的值一一放进去
            this.bindCheckGroupValue2Dom = function (textDiv, div) {
                //
                checkGroupValueModel = that.getSourceCheckGroupValueModel(textDiv) || that.getAccountModel(textDiv).MCheckGroupValueModel;
                //把表格里面所有的选择框度拿出来
                var checkGroupValueDoms = $(checkGroupValue, div);
                //
                for (var i = 0; i < checkGroupValueDoms.length ; i++) {
                    //
                    var input = checkGroupValueDoms.eq(i);
                    //
                    var data = input.data('data');
                    //
                    if (data.MShowType == 2) {
                        //
                        input.combotree("setValue", checkGroupValueModel[data.MCheckTypeColumnName]);
                    }
                    else {
                        //
                        input.combobox("setValue", checkGroupValueModel[data.MCheckTypeColumnName]);
                    }
                }
            }
            //
            this.showThWithData = function (table) {
                //
                $("tr", table).each(function () {
                    if ($(checkGroupTitle, this).eq(0).text().length > 0) {

                        $(this).show();
                    }
                    else {
                        $(this).remove();
                    }
                });

            };
            //显示汇率
            this.getCurrencyRate = function (fromCurrency, toCurrency, date, textDiv) {
                //异步获取汇率
                mAjax.post(
                    getCurrencyRateUrl,
                    {
                        filter: {
                            MSourceCurrencyID: fromCurrency,
                            MTargetCurrencyID: toCurrency,
                            MRateDate: date
                        }
                    },
                    function (data) {
                        //选中一行
                        that.showCurrencyRate(fromCurrency, data, textDiv);
                    });
            };
            //先择了币别
            this.showCurrencyRate = function (currencyID, rate, textDiv) {
                //
                if (!isFinalTransfer) {
                    //
                    rate = rate == 0 ? 1.0 : rate;
                    //显示到汇率
                    $(currencyRate, currencySetupDiv + ":visible").numberbox("setValue", rate);
                    //如果是本位币汇率，则汇率不可编辑，因为就是1：1
                    $(currencyRate, currencySetupDiv + ":visible").numberbox(currencyID == top.MegiGlobal.BaseCurrencyID ? "disable" : "enable", true);
                    //清空值
                    $(currencyAmount, currencySetupDiv + ":visible").numberbox("setValue", "");
                    //清空值
                    $(currencyAmountFor, currencySetupDiv + ":visible").numberbox("setValue", "");
                    //如果是本位币的话，则不可编辑
                    $(currencyAmountFor, currencySetupDiv + ":visible").numberbox(currencyID == top.MegiGlobal.BaseCurrencyID ? "disable" : "enable", true);
                    //清空借贷金额
                    that.clearDebitCredit(that.getParentNode(textDiv, "tr"));
                }
                else {
                    //选择的时候把CurrencyID绑定到输入框里面去
                    var cloneDiv = $("#" + $(textDiv).attr("tipId"));
                    //
                    var tr = that.getParentNode(textDiv, "tr");
                    //更新本行Debit的值
                    var valueDiv = $(debitDiv + "," + creditDiv, tr);
                    //
                    valueDiv.attr("currencyId", currencyID);
                }
            };
            //根据用户输入的税率和金额，计算借贷的金额 dir借贷方向
            this.handleDebitCreditByRate = function (div, setupDiv, target) {
                //
                var tr = that.getParentNode(div, "tr");
                //
                var account = that.getAccountModel(div);
                //
                var dir = account.MDC;
                //币别
                var id = $(currencyId, setupDiv).combobox("getValue");
                //获取税率
                var rate = $(currencyRate, setupDiv).val();
                //如果没有税率
                if (rate && rate > 0) {
                    //
                    var amount = amountFor = 0;
                    //本位币的输入不需要帮用户去算，直接取用户的输入值
                    if (target.hasClass(currencyAmountFor.substring(1))) {
                        //原币 = 本位币 / 汇率
                        amountFor = $(currencyAmountFor, setupDiv).val();
                        //
                        var amount = (amountFor * rate).toFixed(2);
                        //
                        $(currencyAmount, setupDiv).numberbox("setValue", amount);
                    }
                    else {
                        //
                        amount = $(currencyAmount, setupDiv).val();

                        //如果是本位币的话，则需要同步原币金额
                        if (id === GLVoucherHome.baseCurrencyID) {
                            //
                            amountFor = amount;
                            //
                            $(currencyAmountFor, setupDiv).numberbox("setValue", amountFor);
                        }
                        else {
                            //
                            amountFor = $(currencyAmountFor, setupDiv).numberbox("getValue");
                        }
                    }
                    //如果是正方向
                    //更新本行Debit的值
                    var valueDiv = $(((dir == 1) ? debitDiv : creditDiv), tr);
                    //需要清空的内容
                    var emptyDiv = $(((dir == -1) ? debitDiv : creditDiv), tr);
                    //清空
                    emptyDiv.empty().data("value", null);
                    //显示冲销的值
                    that.handleOffsetValueShow(valueDiv, amount);

                    //
                    account.MCurrencyDataModel = {
                        MCurrencyID: id,
                        MAmount: amount,
                        MAmountFor: amountFor,
                        MExchangeRate: rate
                    };
                    //
                    that.setAccountModel(div, account);
                    //
                    that.showCurrencyData(div);
                }
            };
            //把币别，汇率，原币金额展示到币别格子
            this.showCurrencyData = function (div, currency) {

                var account = that.getAccountModel(div);
                //
                var currency = that.getSourceCurrencyModel($(div)) || currency || account.MCurrencyDataModel;
                //
                !div.hasClass(disableTdClass) && div.empty().html(mText.htmlDecode(checkType.getCurrencyHtml(currency, false, account.MIsCheckForCurrency)));
            }
            //判断某一行是不是空行
            this.isRowEmpty = function (currentTr) {
                //科目
                var account = that.getAccountModel(null, currentTr);
                //必须是所有的内容都为空才表示这是一行空行
                return !account || !account.MItemID;
            };
            //计算贷方金额，和借方金额总和
            this.calculateTotal = function () {
                //
                var debitTotal = 0,
                creditTotal = 0;
                //贷方
                $(debitDiv).each(function () {
                    if ($(this).data("value")) {
                        debitTotal = new Number(debitTotal).add(+($(this).data("value")));
                    }
                });
                //借方
                $(creditDiv).each(function () {
                    if ($(this).data("value")) {
                        creditTotal = new Number(creditTotal).add(+($(this).data("value")));
                    }
                });

                //显示
                $(debitTotalDiv).text(mMath.toMoneyFormat(Math.abs(debitTotal)));
                //保存值
                that.addRedClass($(debitTotalDiv).data("value", debitTotal), debitTotal < 0);
                //显示
                $(creditTotalDiv).text(mMath.toMoneyFormat(Math.abs(creditTotal)));
                //保存值
                that.addRedClass($(creditTotalDiv).data("value", creditTotal), creditTotal < 0);
                //如果借贷平衡，则显示合计
                if (creditTotal.toFixed(2) === debitTotal.toFixed(2)) {
                    //正式的时候，将这里的 == 修改为 !=
                    var value = top.OrgLang[0].LangID != '0x0009' ? GLVoucherHome.Number2CN(Math.abs(debitTotal)) : mMath.toMoneyFormat(Math.abs(debitTotal));
                    //
                    $(totalSpan).text(value);
                    //并把值绑定
                    $(totalSpan).data("value", creditTotal);
                } else {
                    //置空
                    $(totalSpan).text("");
                    //并把值绑定
                    $(totalSpan).data("value", -1);
                }
            };
            //重新计算宽度
            this.resizeTable = function (table, width) {
                //
                table = table || tableBody;
                //
                allWidth = width || baseWidth;
                //高级选项的宽度
                var advWidth = $(advancedOptions).is(":visible") ? ($(advancedOptions).outerWidth() + 18) : 0;
                //调整表格的宽度
                $(tableBody).width($(bodyDiv).width() - advWidth);
                //
                var tableWidth = $(table).width();

                var allRowWidth = 0;

                $("th:visible", table).each(function () {
                    allRowWidth += +$(this).attr("rowwidth");
                });

                $("th:visible", table).each(function () {
                    //按照基数调整
                    $(this).attr("width", ($(this).attr("rowwidth") / allRowWidth * 100) + "%");
                });
            };
            //查看模板是否已经保存了
            this.isVoucherModuleFastCodeExists = function (fastcode) {
                //遍历
                for (var i = 0; i < voucherModuleWithEntryData.length; i++) {
                    //如果本身是模板，则剔除与本身的比较
                    if (isModule && itemID && voucherModuleWithEntryData[i].MItemID == itemID && voucherModuleWithEntryData[i].MFastCode == fastcode) {
                        //跳过
                        continue;
                    }
                    //根据ID查找
                    if (voucherModuleWithEntryData[i].MFastCode == fastcode) {
                        //返回
                        return true;
                    }
                }
                return false;
            }
            //根据模板ID获取模板凭证
            this.getVoucherModuleData = function (id, entryIds, func, panel, $td) {
                //
                if (!voucherModuleWithEntryData || voucherModuleWithEntryData.length == 0) {
                    //循环去取
                    voucherModuleWithEntryData = that.getVoucherModuleWithEntryData(false, null, false, {});
                }
                //
                var voucherModule = {};
                //遍历
                for (var i = 0; i < voucherModuleWithEntryData.length; i++) {
                    //根据ID查找
                    if (voucherModuleWithEntryData[i].MItemID == id) {
                        //
                        voucherModule = _.clone(voucherModuleWithEntryData[i]);
                        //
                        if (entryIds && entryIds.length > 0) {
                            //
                            var entrys = [];
                            //
                            for (var j = 0 ; j < voucherModule.MVoucherModuleEntrys.length ; j++) {
                                //
                                if (entryIds.contains(voucherModule.MVoucherModuleEntrys[j].MEntryID)) {
                                    //包含的话就加进去
                                    entrys.push(voucherModule.MVoucherModuleEntrys[j]);
                                }
                            }
                            //
                            voucherModule.MVoucherModuleEntrys = entrys;
                        }
                        //返回
                        return func(voucherModule, panel, $td);
                    }
                }
            };
            //显示模板凭证的相应信息
            this.showVoucherModule = function (data, panel, $td) {
                //显示右边的边框
                var moduleDiv = $(modulePreviewDiv);
                //获取table
                var moduleTable = $(modulePreviewTable);
                //
                var firstRow = $(modulePreviewTableTh);
                //先清除表格里面的非th行
                $("tr", moduleTable).not(modulePreviewTableTh).remove();
                //
                for (var i = 0; i < data.MVoucherModuleEntrys.length; i++) {
                    //插入一行
                    firstRow = that.insertOneModuleRow2Preview(data.MVoucherModuleEntrys[i], firstRow);
                }
                //先定位位置，再显示
                that.locateModuleDiv(moduleDiv, panel);
                //计算表格宽度
                that.resizeTable(moduleTable, allModuleWith);
                //显示表格
                moduleDiv.show();
                //
                that.bindModulePreviewTableEvent(data, $td);
            };
            //绑定凭证模板预览表格里面的点击事件
            this.bindModulePreviewTableEvent = function (data, $td) {
                //
                var trs = $(moduleEntryRow + ":visible", modulePreviewDiv);
                //
                trs.off("click").on("click", function (index) {
                    //
                    var entryId = $(this).attr("mentryid");
                    //插入信息
                    that.getVoucherModuleData(data.MItemID, [entryId],
                    function (data) {
                        //
                        that.insertModuleRows(data, $td);
                    });
                });
            };
            //显示模板凭证的相应信息
            this.showVoucherFastCodeSetup = function (data) {
                //获取table
                var setupTable = $(fastcodeTable);
                //
                var firstRow = $(fastCodeTableTh);
                //先清除表格里面的非th行
                $("tr", setupTable).not(fastCodeTableTh).remove();
                //
                for (var i = 0; i < data.MVoucherModuleEntrys.length; i++) {
                    //插入一行
                    firstRow = that.insertOneModuleRow2SetupTable(data.MVoucherModuleEntrys[i], firstRow);
                    //
                    firstRow.data("MModuleEntryData", data.MVoucherModuleEntrys[i]);
                }
                //计算各个div的位置
                that.resizeFastCodeSetupDiv();
                //计算表格宽度
                that.resizeTable(setupTable, allModuleWith);
                //绑定里面的事件
                that.bindModuleTableEvent();
                //初始化tooltip
                that.tooltip(setupTable);
            };
            //快速码设置div的高度设定
            this.resizeFastCodeSetupDiv = function () {
                //
                $(fastcodeTableDiv).css(
                    {
                        height: ($(fastcodeTableDiv).parent().outerHeight() - $(fastcodeInputDiv).outerHeight() - $(fastcodeOperationDiv).outerHeight() - $(fastcodeTitle).outerHeight() - 20) + "px"
                    });
            };
            //定位模板显示的位置
            this.locateModuleDiv = function (div, panel) {
                //
                var top = panel.offset().top;

                var divHeight = $(div).height() > 400 ? 400 : $(div).height();
                //
                if (top + divHeight > $("body").height()) {
                    //
                    top = $("body").height() - divHeight;
                }
                //
                div.css(
                    {
                        "left": (panel.offset().left + panel.width()) + "px",
                        "top": top + "px",
                        "max-height": 400 + "px",
                        "overflow-y": "auto",
                        "overflow-x": "hidden"
                    });
            };
            //添加一行
            this.insertOneModuleRow2Preview = function (rowData, behindRow) {
                //如果没有数据则为空
                rowData = mText.encode(rowData || {});
                //如果没有bihindRow
                behindRow = behindRow || tableHeader;
                //
                var rowText = "<tr class='vc-entry-row vc-module-entry-row' mentryid=" + (rowData.MEntryID || "") + ">";
                //第一列 摘要 MExplanation
                var rowExplanation = "<td class='vc-preview-explanation no-left' rowwidth='" + explanationWidth + "'><div class='vc-preview-explanation-div' value='" + (rowData.MExplanation || "") + "' etitle='" + mText.encode(rowData.MExplanation || "") + "'>" + (rowData.MExplanation || "").replace(/\n/gi, '<br>') + "</div>" + explanationTextareaText + "</td>";
                //第二列 科目
                var rowAccount = "<td class='vc-preview-account' rowwidth='" + accountWidth + "' > <div class='vc-preview-account-div'><span class='vc-account-name change-item'  value='" + (rowData.MAccountID || "") + "' etitle='" + mText.encode(rowData.MAccountName || "") + "'>" + (rowData.MAccountName || "") + "</span></div></td>";
                //第三列 外币核算
                var rowCurrency = "<td class='vc-fastcode-currency' rowwidth='" + currencyWidth + "' ><div class='vc-fastcode-currency-div'>" + (rowData.MAccountID ? checkType.getCurrencyHtml(rowData.MAccountModel.MCurrencyDataModel, true) : "") + "</div></td>";
                //第四列是核算维度列
                var rowCheckGroup = "<td class='vc-fastcode-checkgroup' rowwidth='" + checkGroupWidth + "' ><div class='vc-fastcode-checkgroup-div'> " + (rowData.MAccountID ? checkType.getCheckGroupValueHtml(rowData.MAccountModel.MCheckGroupValueModel, rowData.MAccountModel.MCheckGroupModel) : "") + "</div></td>";
                //
                var debit = rowData.MDC == 1 ? rowData.MDebit : "";
                //
                var credit = rowData.MDC == -1 ? rowData.MCredit : "";
                //第十借方金额
                var rowDebit = "<td class='vc-preview-debit text-right' rowwidth='" + debitWidth + "'><div class='vc-preview-debit-div' value='" + debit + "' currencyID='" + (rowData.MCurrencyID || "") + "' exchangeRate='" + (rowData.MExchangeRate || "") + "'>" + mMath.toMoneyFormat(debit) + "</div></td>";
                //第十一贷方金额
                var rowCredit = "<td class='vc-preview-credit text-right no-right' rowwidth='" + creditWidth + "'><div class='vc-preview-credit-div' value='" + credit + "' currencyID='" + (rowData.MCurrencyID || "") + "' exchangeRate='" + (rowData.MExchangeRate || "") + "'>" + mMath.toMoneyFormat(credit) + "</div></td>";
                //拼接起来
                rowText += (rowExplanation + rowAccount + rowCurrency + rowCheckGroup + rowDebit + rowCredit);
                //结尾
                rowText += "</tr>";
                //
                var $row = $(rowText).insertAfter(behindRow);
                //
                return $row;
            };
            //把所有的有etitle的控件都tooltip一下
            this.tooltip = function (selector) {
                //
                selector = selector || "body";
                //
                $("div[etitle],span[etitle]", selector).each(function (index) {

                    var title = $(this).attr("etitle") || $(this).text();
                    $(this).removeAttr('etitle').tooltip({
                        content: mText.encode(title)
                    })
                });
            };
            //添加一行到快速码设置表格
            this.insertOneModuleRow2SetupTable = function (rowData, behindRow) {
                //如果没有数据则为空
                rowData = mText.encode(rowData || {});
                //如果没有bihindRow
                behindRow = behindRow || tableHeader;
                //
                var rowText = "<tr class='vc-entry-row' mentryid=" + (rowData.MEntryID || "") + ">";

                //第一个各自是个单选框
                var checkBoxText = "<td class='vc-fastcode-checkbox'><input type='checkbox' checked='checked'/></td>";
                //第一列 摘要 MExplanation
                var rowExplanation = "<td class='vc-fastcode-explanation no-left' rowwidth='" + explanationWidth + "'><div class='vc-fastcode-explanation-div' value='" + (rowData.MExplanation || "") + "' etitle='" + mText.encode(rowData.MExplanation || "") + "'>" + (rowData.MExplanation || "").replace(/\n/gi, '<br>') + "</div>" + explanationTextareaText + "</td>";
                //第二列 科目
                var rowAccount = "<td class='vc-fastcode-account' rowwidth='" + accountWidth + "' > <div class='vc-fastcode-account-div'><span class='vc-fastcode-account-name change-item'  >" + rowData.MAccountName + "</span></div></td>";
                //第三列 外币核算
                var rowCurrency = "<td class='vc-fastcode-currency' rowwidth='" + currencyWidth + "' ><div class='vc-fastcode-currency-div'>" + (rowData.MAccountID ? checkType.getCurrencyHtml(rowData.MCurrencyDataModel, true) : "") + "</div></td>";
                //第四列是核算维度列
                var rowCheckGroup = "<td class='vc-fastcode-checkgroup' rowwidth='" + checkGroupWidth + "' ><div class='vc-fastcode-checkgroup-div'> " + (rowData.MAccountID ? checkType.getCheckGroupValueHtml(rowData.MAccountModel.MCheckGroupValueModel, rowData.MAccountModel.MCheckGroupModel) : "") + "</div></td>";
                //
                var debit = rowData.MDC == 1 ? rowData.MDebit : "";
                //
                var credit = rowData.MDC == -1 ? rowData.MCredit : "";
                //第十借方金额
                var rowDebit = "<td class='vc-fastcode-debit text-right' rowwidth='" + debitWidth + "'><div class='vc-fastcode-debit-div' value='" + debit + "' >" + mMath.toMoneyFormat(debit) + "</div></td>";
                //第十一贷方金额
                var rowCredit = "<td class='vc-fastcode-credit text-right' rowwidth='" + creditWidth + "'><div class='vc-fastcode-credit-div' value='" + credit + "' >" + mMath.toMoneyFormat(credit) + "</div></td>";
                //拼接起来
                rowText += (checkBoxText + rowExplanation + rowAccount + rowCurrency + rowCheckGroup + rowDebit + rowCredit);
                //结尾
                rowText += "</tr>";
                //
                var $row = $(rowText).insertAfter(behindRow);
                //
                return $row;
            };
            /*异步获取数据*/
            //异步获取凭证
            this.getVoucherData = function (year, period, day, func) {
                //
                var url = getVoucherUrl;
                //
                var params = {
                    MItemID: itemID,
                    MNumber: mNumber,
                    MIsCopy: (isCopy ? true : false),
                    MIsReverse: (isReverse ? true : false),
                    MYear: year,
                    MPeriod: period,
                    MDay: day,
                    MDir: dir,
                    MPeriodTransfer: periodTransferModel,
                    MDocVoucherID: docVoucherID,
                    MEntryAccountPair: entryAccountPair
                };
                //通过异步从后台获取凭证信息
                mAjax.post(url, { model: params },
                function (data) {
                    //调用回调函数
                    func && $.isFunction(func) && func(data);
                },
                "", true)
            };
            //获取凭证模板数据
            this.getVoucherModule = function (func) {
                //
                var url = getVoucherModuleUrl;
                //
                var params = {
                    MItemID: itemID
                };
                //通过异步从后台获取凭证信息
                mAjax.post(url, params,
                function (data) {
                    //调用回调函数
                    func && $.isFunction(func) && func(data);
                },
                "", true)
            };
            //获取快速码表
            this.getVoucherModuleWithNoEntryData = function (force, func, aysnc) {

                //
                aysnc = aysnc || $.isFunction(func);
                //如果强制获取的话
                if (force === true) {
                    //
                    voucherModuleWithNoEntryData = home.getVoucherModuleWithNoEntryData(func, {}, aysnc);
                }
                else if (voucherModuleWithNoEntryData.length == 0) {
                    //
                    return home.getVoucherModuleWithNoEntryData(func, {}, aysnc);
                }
                else {
                    $.isFunction(func) && func(voucherModuleWithNoEntryData);
                }
                //
                return voucherModuleWithNoEntryData;
            };
            //获取一个模板凭证详情
            this.getVoucherModuleWithEntryData = function (force, func, aysnc, param) {
                //
                aysnc = aysnc || $.isFunction(func);
                //如果强制获取的话
                if (force === true) {
                    //
                    voucherModuleWithEntryData = home.getVoucherModuleWithEntryData(func, param, aysnc);
                }
                else if (voucherModuleWithEntryData.length == 0) {
                    //
                    return home.getVoucherModuleWithEntryData(func, param, aysnc);
                }
                else {
                    $.isFunction(func) && func(voucherModuleWithEntryData);
                }
                //
                return voucherModuleWithEntryData;
            };
            //获取摘要表
            this.getExplanationData = function (force, func, aysnc) {
                //
                aysnc = aysnc || $.isFunction(func);
                //如果强制获取的话
                if (force === true) {
                    //
                    home.getExplanationData(func, {}, aysnc);
                }
                else if (explanationData.length == 0) {
                    //
                    return home.getExplanationData(func, {}, aysnc);
                }
                else {
                    $.isFunction(func) && func(explanationData);
                }
                //
                return explanationData;
            };
            //获取科目数据
            this.getAccountData = function (force, func, aysnc) {
                //
                aysnc = aysnc || $.isFunction(func);
                //如果强制获取的话
                if (force === true) {
                    //
                    home.getAccountData(func, null, aysnc);
                }
                else if (accountData.length == 0) {
                    //
                    return home.getAccountData(func, null, aysnc);
                }
                else {
                    $.isFunction(func) && func(accountData);
                }
                //
                return accountData;
            };


            //handleChange
            this.handleChange = function () {
                //设置为稳定状态
                $.mTab.setStable();
            };
            //跳转上一个格子还是下一个格子
            this.switch2Cell = function (dir, td) {
                //找到目标cell,td
                var targetTd = "";
                //默认是正向跳转
                if (dir != -1) {
                    //下一个td
                    targetTd = that.getNextNode(td || $("." + focusItem), "td");

                } else {
                    //反向跳转
                    targetTd = that.getPrevNode(td || $("." + focusItem), "td");
                    //
                    if (targetTd.hasClass(currencyTd.trimStart('td.'))) {
                        //
                        var account = that.getAccountModel(targetTd);
                        //
                        if (!account || !account.MIsCheckForCurrency) {
                            //
                            return that.switch2Cell(-1, targetTd);
                        }
                    }
                    else if (targetTd.hasClass(checkGroupTd.trimStart('td.'))) {
                        //
                        var account = that.getAccountModel(targetTd);
                        //
                        if (!account.MCheckGroupModel || account.MCheckGroupModel.MItemID == "0") {
                            //
                            return that.switch2Cell(-1, targetTd);
                        }
                    }
                }
                //触发点击事件
                targetTd.trigger("click.vc");
            };
            //表格内部点击回车事件
            //获取下一个元素
            this.getNextNode = function (input, nodeType) {
                //先找到其所在td
                var td = that.getParentNode(input, nodeType);
                //找到其相近的td
                var nextTd = td.next(nodeType);
                //如果没有找到
                if (nextTd.length == 0) {
                    //找到这一行
                    var tr = that.getParentNode(td, "tr");
                    //找到下一行
                    var nextTr = tr.next("tr.vc-entry-row");

                    //如果下面没有一行了需要自动新增一行
                    if (nextTr.length == 0) {
                        //添加一行新的
                        that.insertOneRowByHref(tr);
                        nextTr = tr.next("tr.vc-entry-row");
                    }
                    //找到第一个td
                    return nextTr.find(nodeType).first();
                } else {
                    //直接返回
                    return nextTd;
                }
            };
            //获取下一个元素
            this.getPrevNode = function (input, nodeType) {
                //先找到其所在td
                var td = that.getParentNode(input, nodeType);
                //找到其相近的td
                var prevTd = td.prev(nodeType);
                //如果没有找到
                if (prevTd.length == 0) {
                    //找到这一行
                    var tr = that.getParentNode(td, "tr");
                    //找到下一行
                    var prevTr = tr.prev("tr");
                    //找到第一个td
                    return prevTr.find(nodeType).last();
                } else {
                    //直接返回
                    return prevTd;
                }
            };
            //获取一个元素的父节点中为某种类型的节点
            this.getParentNode = function (input, nodeType) {
                //采用递归的方式查找
                return input.is(nodeType) ? input : that.getParentNode(input.parent(), nodeType);
            };
            //给一个node添加vc-red样式
            this.addRedClass = function (input, add) {
                //是否添加
                if (add) {
                    input.addClass(vcRedClass);
                } else {
                    input.removeClass(vcRedClass);
                }
            };

            /*高级选项里面的方法*/
            //复制凭证
            this.copy = function () {
                //跳转到编辑页面 
                mTab.addOrUpdate(newVoucherLang, editVoucherUrl + "?MItemID=" + itemID + "&" + "IsCopy=" + true, false, true, true);
            };
            //创建冲销凭证
            this.createReverse = function () {
                //跳转到编辑页面 
                mTab.addOrUpdate(newVoucherLang, editVoucherUrl + "?MItemID=" + itemID + "&" + "IsReverse=" + true, false, true, true);
            };
            //查看冲销凭证
            this.viewReverseVoucher = function () {
                //跳转到编辑页面 
                mTab.addOrUpdate("GL-" + voucherData.MNumber, editVoucherUrl + "?MItemID=" + voucherData.MRVoucherID, false, true, true);
            };
            //查看冲销凭证
            this.viewOriginalVoucher = function () {
                //跳转到编辑页面 
                mTab.addOrUpdate("GL-" + voucherData.MNumber, editVoucherUrl + "?MItemID=" + voucherData.MOVoucherID, false, true, true);
            };
            //审核凭证
            this.approveVoucher = function (status) {
                //
                return home.approveVoucher([itemID], status, function () {
                    that.refreshVoucherPage(voucherData);
                });
            };
            //打印凭证
            this.printVoucher = function () {
                //调用首页方法
                return home.printVoucher(itemID);
            };
            //调整高级选项的样式和高度
            this.adjustAdvOptionsDom = function () {
                //面板高度
                $("div.accordion").height($(tableBody).height() - 2);
                //
                $(".vc-adv .panel-header").css({
                    "height": "40px"
                });
                //头高度
                $(".vc-adv .panel-title").css({
                    "line-height": "40px",
                    "height": "40px"
                });
                //
                $("div.accordion-body").height($(tableBody).height() - 50 - 2);
            };
            /*下面是和数据相关的操作*/
            //复制第一行的摘要
            this.copyFirstExplanation = function (input) {
                //获取当前的输入框
                var $input = input || $("." + focusItem);
                //第一行
                var firstRow = $(tableBody + " tr:eq(1)");
                //获取其本身所在的tr
                var $tr = that.getParentNode($input, "tr");
                //如果本身就是第一行
                if (firstRow == $tr) {
                    //清空
                    $input.val("");
                } else {
                    //赋值
                    $input.val($(explanationDiv, firstRow).text());
                }
                //
                var textDiv = $(explanationDiv, $tr);
                //文本框也要赋值
                textDiv.text($input.val());
                //隐藏panel
                new mCombobox(textDiv.next("textarea")).hidePanel();
            };
            //从上一行复制摘要
            this.copyPrevExplanation = function (input) {
                //获取当前的输入框
                var $input = input || $("." + focusItem);
                //获取其本身所在的tr
                var $tr = that.getParentNode($input, "tr");
                //获取上一个tr
                var prevTr = $tr.prev();
                //如果上一行不存（行元素数量为0或者行元素没有摘要项）
                var prevHtml = prevTr.length == 0 || $(explanationDiv, prevTr).length == 0 ? "" : $(explanationDiv, prevTr).html().replace(/<br>/gi, '\n');
                //获取第一行的第一个摘要
                $input.val(prevHtml);
                //文本也要复制
                var textDiv = $(explanationDiv, $tr);
                //文本框也要赋值
                textDiv.mHtml(prevHtml);
                //
                var box = new mCombobox(textDiv.next("textarea"));
                //隐藏panel
                box.hidePanel();
            };
            //借贷对调，即在一方输入数字后，想调到另一方，输入空格键就行
            this.switchDebitCredit = function () {
                //如果焦点
                var input = $("." + focusItem);
                //获取当前行
                var tr = that.getParentNode(input, "tr");
                //借
                var dInput = $(debitInput, tr);
                //贷
                var cInput = $(creditInput, tr);
                //如果是从借到贷
                if (input[0] == dInput[0]) {
                    //贷方
                    $(creditDiv, tr).data("value", input.val());
                    //本身赋值为""
                    input.val("");
                    //
                    $(creditTd, tr).trigger("click");
                } else {
                    //贷方
                    $(debitDiv, tr).data("value", input.val());
                    //本身赋值为""
                    input.val("");
                    //
                    $(debitDiv, tr).trigger("click");
                }
            };
            //借贷平衡，即在借贷里面输入等于号，则自动实现借贷平衡
            this.balanceDebitCredit = function () {
                //
                var input = $("." + focusItem);
                //先设置为0
                input.numberbox("setValue", 0);
                //获取所在的行
                var tr = that.getParentNode(input, "tr");
                //借
                var dInput = $(debitInput, tr);
                //贷
                var cInput = $(creditInput, tr);
                //
                var isDebit = input.hasClass("vc-debit-input");
                //对面的那个输入框
                var oInput = isDebit ? cInput : dInput;
                //
                var oDiv = isDebit ? creditDiv : debitDiv;
                //
                var div = isDebit ? debitDiv : creditDiv;
                //清空对方的值
                $(oDiv, tr).data("value", 0).text("");
                //清空对方的值
                $(div, tr).data("value", 0).text("");
                //本身赋值为""
                oInput.val("");
                //重新计算一下合计
                that.calculateTotal();


                //获取借方目前的总金额
                var debitTotal = +($(debitTotalDiv).data("value") || 0);
                //获取贷方总金额
                var creditTotal = +($(creditTotalDiv).data("value") || 0);


                //差额
                var diff = (debitTotal - creditTotal) * (isDebit ? (-1) : 1);
                //
                var diffFloat = +(diff.toFixed(2));
                //如果是贷方
                input.numberbox("setValue", diffFloat);
            };
            //上下选择金额 dir : 1 up  -1 down
            this.upDownSwitchDebitCredit = function (dir) {
                //
                dir = dir || 1;
                //如果焦点
                var input = $("." + focusItem);
                //获取当前行
                var tr = that.getParentNode(input, "tr");
                //获取目标行
                var targetTr, targetTd;
                //
                if (dir == 1) {
                    //上一行
                    targetTr = tr.prev("tr" + entryRow);
                } else {
                    //上一行
                    targetTr = tr.next("tr" + entryRow);
                }
                //如果是借
                if (input.hasClass(debitInput.trimStart('.'))) {
                    //
                    targetTd = $(debitTd, targetTr);
                } else {
                    //
                    targetTd = $(creditTd, targetTr);
                }
                //
                if (targetTd && targetTd.length == 1) {
                    //
                    targetTd.trigger("click.vc");
                }
            };
            //清空借贷的值
            this.clearDebitCredit = function (tr) {
                //
                $(debitDiv, tr).text("").data("value", "");
                //
                $(creditDiv, tr).text("").data("value", "");
            };
            //保存并审核凭证
            this.saveAndApproveVoucher = function (callback) {
                //
                if (isDraftDocVoucher) {
                    //
                    return false;
                }
                //
                var voucher = that.getVoucher();
                //是否审核
                voucher.MStatus = 1;
                //
                return that.saveVoucher(callback, voucher, "F11");
            };
            //清空保存为模板的快速码和内容
            this.clearFastCodeSetup = function () {
                //清空快速码
                isModule ? $(fastcodeInput).val("") : $(tbFastcodeInput).val("");
                //清空描述
                isModule ? $(descriptionInput).val("") : $(tbDescriptionInput).val("");
            };
            //保存模板
            this.saveModule = function (callback, voucherModule) {
                //
                voucherModule = voucherModule || (isModule ? that.getVoucher(false) : that.getModule());
                //如果取不到凭证
                if (voucherModule === false) {
                    //应该直接返回
                    return false;
                }
                //
                var func1 = function (data) {
                    //提醒信息
                    var message = HtmlLang.Write(LangModule.Common, "SaveSuccessfully", "Save Successfully!");
                    //
                    mDialog.message(message);
                    //重新获取模信息
                    that.getVoucherModuleWithEntryData(true, null, false);
                    //重新获取模信息
                    that.getVoucherModuleWithNoEntryData(true, null, false);
                    //
                    that.refreshVoucherModulePage();

                    //
                    $(tbFastcodeInput).val("");
                    $(tbDescriptionInput).val("");

                    //调用回调函数
                    $.isFunction(callback) && callback(data);
                };
                //
                var func2 = function (data) {
                    //提醒错误信息，直接返回
                    return mDialog.error(data.ErrorMessage);
                }
                //
                home.saveVoucherModule(voucherModule, func1, func2);
            };
            //保存凭证或者模板
            this.saveVoucherOrModule = function (callback, t) {
                //
                operateTarget = t || operateTarget;
                //
                if (operateTarget == MODULE) {
                    //保存为凭证模板
                    isModule ? that.saveModule(callback || that.refreshVoucherPage) : that.saveModule(function () {
                        //隐藏就行
                        that.showFastCodeSetup(false);
                        //当前操作为凭证
                        operateTarget = VOUCHER;
                        //
                        that.refreshVoucherModulePage();
                    });
                } else {
                    //如果是业务单据转凭证设置页面，保存结果后，自动关闭当前页面
                    if (isDraftDocVoucher || fromDocVoucherSetting) {
                        callback = mDialog.close;
                    }
                    //保存为凭证
                    that.saveVoucher(callback || that.refreshVoucherPage);
                }
            };
            //保存并新增凭证或者模板
            this.saveAndNewVoucherOrModule = function (callback, t) {
                //
                if (isDraftDocVoucher) {
                    //
                    return false;
                }
                //
                operateTarget = t || operateTarget;
                //
                if (operateTarget == MODULE) {
                    //保存为凭证模板
                    that.saveAndNewVoucherModule(callback || that.refreshVoucherPage);
                } else {
                    //爆粗为凭证
                    that.saveAndNewVoucher(callback || that.refreshVoucherPage);
                }
            };
            //保存凭证
            this.saveVoucher = function (callback, voucher, hotKey) {
                //自我调用的函数
                var selfCallback = function () { };
                //
                switch (hotKey) {
                    case "F12":
                        selfCallback = function () { $("#btnSaveAndAddAnother").trigger("click.save") };
                        break;
                    case "F11":
                        selfCallback = function () { $("#btnSaveAndApprove").trigger("click.save") };
                        break;
                    default:
                        selfCallback = function () { $("#btnSave").trigger("click.save") };
                        break;
                }
                $("body").mask("");
                //获取到整个凭证
                var voucher = voucher || that.getVoucher();
                //如果取不到凭证
                if (voucher === false) {
                    $("body").unmask();
                    //应该直接返回
                    return false;
                }
                //是否从期末结算过来的，以及是否从业务单据过来的
                voucher = $.extend(voucher, {
                    FromPeriodTransfer: fromPeriodTransfer ? true : false
                });
                //期末转结
                var transfer = that.getPeriodTransferModel();
                //显示保存成功
                var func1 = function (data) {
                    //
                    if (!itemID && typeof (AssociateFiles) != undefined) {
                        //关联上传的附件
                        AssociateFiles.associateFilesTo($("#hidBizObject").val(), data.MItemID, undefined,
                        function () {
                            //处理
                            that.showSaveResult(data, voucher, callback, selfCallback);
                        });
                    } else {
                        //处理
                        that.showSaveResult(data, voucher, callback, selfCallback);
                    }
                };
                //显示保存失败
                var func2 = function () {
                    //
                    that.showSaveResult(false, voucher);
                };
                //
                voucher.MPeriodTransfer = transfer;
                //
                home.saveVoucher(voucher, func1, func2);
            };
            //获取期末转结的凭证
            this.getPeriodTransferModel = function () {
                //
                return fromPeriodTransfer ? periodTransferModel : null;
            };
            //保存失败和成功提醒
            this.showSaveResult = function (data, voucher, callback, selfCallback) {
                //如果有错误信息
                if (data.ErrorMessage) {
                    //如果凭证号不相等，
                    if (data.MNumber != voucher.MNumber) {
                        //把凭证号重新更新一下
                        $("#MNumber").numberspinner("setValue", data.MNumber);
                    }
                    //如果是警示性的，直接弹出提醒
                    if (data.ErrorCode == 0) {
                        //
                        mDialog.error(data.ErrorMessage);
                    }
                    else {
                        //如果是可修复性的，则确认后重新保存
                        mDialog.confirm(data.ErrorMessage, selfCallback);
                    }
                    //返回
                    return false;
                }
                //提醒信息
                var message = voucher.MStatus == approved ? HtmlLang.Write(LangModule.Common, "ApproveSuccessfully", "Approve Successfully!") : HtmlLang.Write(LangModule.Common, "SaveSuccessfully", "Save Successfully!");
                //
                mDialog.message(message);
                //保存成功后
                $.isFunction(callback) ? callback(data) : "";
            };
            //保存并新增凭证
            this.saveAndNewVoucher = function () {
                //日期
                var date = mDate.parse($("#MDate").datebox("getValue"));
                //
                var year = date.getFullYear();
                //
                var month = date.getMonth();
                //
                var day = date.getDate();
                //保存
                that.saveVoucher(function () {
                    //
                    $.mTab.addOrUpdate(newVoucherLang, editVoucherUrl + "?year=" + year + "&period=" + (month + 1) + "&day=" + day, true, true);
                }, "", "F12");
            };
            //保存并新增模板
            this.saveAndNewVoucherModule = function () {
                //保存
                this.saveVoucherOrModule(function () {
                    //
                    $.mTab.addOrUpdate(newVoucherModuleLang, editVoucherUrl + "?IsModule=1", true, true);
                    //

                },
                MODULE);
            }
            //保存为模板
            this.showSaveAsModule = function () {
                //
                var voucher = that.getVoucher(true);
                //
                if (voucher === false) {
                    //
                    return false;
                }
                //如果不是模板来的，则需要弹出框，让用户输入快速码和名称
                if (!isModule) {
                    //显示快速码设置
                    that.showFastCodeSetup(true, voucher);
                    //操作对象为模板
                    operateTarget = MODULE;
                }
            };
            //显示保存为模板的弹出框
            this.showFastCodeSetup = function (show, data) {
                //
                if (show && data && $(fastcodeTableDiv).is(":hidden")) {
                    //弹出显示
                    $.mDialog.show({
                        mTitle: fastCodeSetupLang,
                        mDrag: "mBoxTitle",
                        mContent: "id:fastcodeSetupDiv",
                        mShowbg: true,
                        mHeight: 400,
                        mWidth: 900,
                        mOpenCallback: function () {
                            //填充表格数据
                            that.showVoucherFastCodeSetup(data);
                        },
                        mCloseCallback: function () {
                            //当前操作为凭证
                            operateTarget = VOUCHER;
                        }
                    });
                } else if ($(fastcodeTableDiv).is(":visible")) {
                    //隐藏
                    $.mDialog.close();
                }
            };
            //取消模板保存
            this.cancelShowSaveAsModule = function () {
                //显示快速码设置
                that.showFastCodeSetup(false);
                //操作对象为模板
                operateTarget = isModule ? MODULE : VOUCHER;
            };
            //获取模板
            this.getModule = function () {
                //先结束编辑
                that.vcBlurEvent();
                //获取凭证
                var moduleModel = that.validateTableAndGetEntryFromModule();
                //如果返回的是一个数组，则表示出错了
                if ($.isArray(moduleModel)) {
                    //第一个是错误的对象
                    var targetNode = moduleModel[0];
                    //
                    targetNode && (targetNode.is("td") ? targetNode.trigger("click.vc") : targetNode.focus());
                    //
                    targetNode && (targetNode.is("a") ? targetNode.trigger("click.tip") : "");
                    //并弹出提醒
                    that.alertErrorMessge(moduleModel[1]);
                    //返回失败
                    return false;
                }
                //返回
                return moduleModel;
            };
            //获取一个凭证object
            this.getVoucher = function (asModule) {

                //
                $("body").mask("");
                //先结束编辑
                that.vcBlurEvent();
                //获取凭证
                var voucher = that.validateTableAndGetEntry(asModule);
                //如果返回的是一个数组，则表示出错了
                if ($.isArray(voucher)) {
                    //第一个是错误的对象
                    var targetNode = voucher[0];
                    //
                    targetNode && (targetNode.is("td") ? targetNode.trigger("click.vc") : targetNode.focus());;
                    //并弹出提醒
                    that.alertErrorMessge(voucher[1]);

                    $("body").unmask();
                    //返回失败
                    return false;
                }

                $("body").unmask();

                //返回
                return voucher;
            };
            //页面错误提醒方法
            this.alertErrorMessge = function (msg) {
                //弹出错误1秒就行
                $.mDialog.message(msg, 2000);
            };
            //校验表格是否合法并且获取分录项
            this.validateTableAndGetEntryFromModule = function () {
                //
                var voucher = {};
                //
                var fastcode = $(tbFastcodeInput).val();
                //如果没有快速码，而且是要保存为凭证模板
                if (!fastcode) {
                    //
                    return [$(tbFastcodeInput), moduleFastcodeNotNull];
                }
                //检测fastcode时候已经被占用了
                if (fastcode && that.isVoucherModuleFastCodeExists(fastcode.trim())) {
                    //
                    return [$(tbFastcodeInput), moduleFastcodeBeenUsed];
                }
                //描述
                var description = $(tbDescriptionInput).val();
                //如果没有快速码，而且是要保存为凭证模板
                if (!description) {
                    //
                    return [$(tbDescriptionInput), moduleDescriptionNotNull];
                }
                //对表格中的每一行进行遍历,需要有勾选的行
                var trs = $("tr:not(" + fastCodeTableTh + ")", fastcodeTable).filter(function () {
                    //
                    var checkbox = $("input[type='checkbox']", this);
                    //
                    return !!checkbox.attr("checked");
                });
                //对表格的第一个行的所有列进行遍历，需要有勾选的列
                var tds = $("tr" + fastCodeTableTh + " th", fastcodeTable).filter(function () {
                    //
                    //
                    var checkbox = $("input[type='checkbox']", this);
                    //
                    return !!checkbox.attr("checked");
                });
                //如果没有选择列
                if (tds.length == 0) {
                    //
                    return [$(""), pleaseSelectColumns2Save];
                }
                //
                var voucherEntrys = [];
                //去除第一行是表头
                for (var i = 0; i < trs.length; i++) {
                    //当前行
                    var currentTr = trs.eq(i);
                    //
                    var voucherEntry = currentTr.data("MModuleEntryData");
                    //
                    voucherEntry.MEntryID = null;
                    //entryid
                    voucherEntrys.push(voucherEntry);
                }
                //如果没有录入凭证
                if (voucherEntrys.length == 0) {
                    //提醒只要要录入一条凭证，并且焦点跳转到第一个格子
                    return [$(""), voucherEntryNotNull];
                }

                //快速码
                voucher.MFastCode = fastcode;
                //描述
                voucher.MDescription = description;
                //是单行模板还是多行模板
                voucher.MIsMulti = voucherEntrys.length > 1;
                //凭证分录
                voucher.MVoucherModuleEntrys = voucherEntrys;
                //根据列的勾选，清除一定的数据
                voucher = that.filterModuleTableWithNoChecked(voucher);
                //返回
                return voucher;
            };
            //将抬头没有勾选的列的值置为空
            this.filterModuleTableWithNoChecked = function (voucher) {
                //
                for (var i = 0; i < voucher.MVoucherModuleEntrys.length ; i++) {
                    //摘要
                    var expalanationChecked = $("input[type='checkbox']", "th.vc-fastcode-explanation").attr("checked");
                    //
                    !!expalanationChecked ? "" : (voucher.MVoucherModuleEntrys[i].MExplanation = null);
                    //科目
                    var accountChecked = $("input[type='checkbox']", "th.vc-fastcode-account").attr("checked");
                    //
                    !!accountChecked ? "" : (voucher.MVoucherModuleEntrys[i].MAccountID = null);
                    //
                    if (!accountChecked) {
                        //
                        voucher.MVoucherModuleEntrys[i].MAccountModel = null;
                        //
                        voucher.MVoucherModuleEntrys[i].MAccountID = null;
                        //
                        voucher.MVoucherModuleEntrys[i].MCurrencyDataModel = null;
                        //
                        voucher.MVoucherModuleEntrys[i].MCheckGroupValueModel = null;
                    }
                    else {
                        //币别
                        var currencyChecked = $("input[type='checkbox']", "th.vc-fastcode-currency").attr("checked");
                        //
                        if (!currencyChecked) {
                            //
                            voucher.MVoucherModuleEntrys[i].MCurrencyID = null;
                            //
                            voucher.MExchangeRate = 1.0;
                            //
                            voucher.MAmountFor = voucher.MAmount;
                        }
                        //核算维度
                        var checkGroupChecked = $("input[type='checkbox']", "th.vc-fastcode-checkgroup").attr("checked");
                        //
                        if (!checkGroupChecked) {
                            //
                            voucher.MVoucherModuleEntrys[i].MCurrencyID = null;
                            //
                            voucher.MVoucherModuleEntrys[i].MExchangeRate = 1.0;
                            //
                            voucher.MVoucherModuleEntrys[i].MAmountFor = voucher.MAmount;
                        }
                    }
                    //借方
                    var debitChecked = $("input[type='checkbox']", "th.vc-fastcode-debit").attr("checked");
                    //
                    !!debitChecked ? "" : (voucher.MVoucherModuleEntrys[i].MDebit = 0);
                    //贷方
                    var creditChecked = $("input[type='checkbox']", "th.vc-fastcode-credit").attr("checked");
                    //
                    !!creditChecked ? "" : (voucher.MVoucherModuleEntrys[i].MCredit = 0);
                }
                //
                return voucher;
            };
            //
            this.getCurrencyData = function (tr) {
                //
                var currency = $(currencyDiv, tr).data("currency");
                //
                return currency;
            }
            //
            this.validateCheckGroupValue = function (tr) {
                //
                var account = that.getAccountModel(null, tr);
                //
                return checkType.validateCheckGroup(account.MCheckGroupModel, account.MCheckGroupValueModel || {}, true);
            }
            //校验表格是否合法并且获取分录项
            this.validateTableAndGetEntry = function (asModule) {
                //借贷是否平衡
                var value = $(totalSpan).data("value");
                //默认取
                var voucher = {
                    MDocVoucherID: docVoucherID,
                    MDocID: docID,
                    MItemID: voucherData.MItemID,
                    MIsReverse: isReverse,
                    MSourceBillKey: sourceBillKey ? sourceBillKey : 0,
                    MOVoucherID: isReverse ? oItemID : null,
                    MTransferTypeID: voucherData.MTransferTypeID
                };
                //凭证编号
                var voucherNumber = isModule ? "" : $("#MNumber").numberspinner("getValue");
                //
                if (!voucherNumber && !isModule && !asModule && !isDraftDocVoucher) {
                    //
                    return [new mNumberSpinner($("#MNumber")).getInput(), voucherNumberNotNull];
                }
                //日期
                var date = isModule ? "" : $("#MDate").datebox("getValue");
                //
                if (!date && !isModule && !asModule && !isDraftDocVoucher) {
                    //
                    return [new mDatebox("#MDate").getInput(), voucherDateNotNull];
                }
                //附件数
                var attachmentAmount = isModule ? "" : $("#Attachments").numberspinner("getValue");
                //
                var fastcode = isModule ? $(fastcodeInput).val() : $(tbFastcodeInput).val();
                //如果没有快速码，而且是要保存为凭证模板
                if (!fastcode && isModule && !asModule && !isDraftDocVoucher) {
                    //
                    return [$(fastcodeInput), moduleFastcodeNotNull];
                }
                //检测fastcode时候已经被占用了
                if (isModule && !isDraftDocVoucher && !asModule && fastcode && that.isVoucherModuleFastCodeExists(fastcode.trim())) {
                    //
                    return [$(fastcodeInput), moduleFastcodeBeenUsed];
                }
                //描述
                var description = isModule ? $(descriptionInput).val() : $(tbDescriptionInput).val();
                //如果没有快速码，而且是要保存为凭证模板
                if (!description && isModule && !asModule && !isDraftDocVoucher) {
                    //
                    return [$(descriptionInput), moduleDescriptionNotNull];
                }
                //对表格中的每一行进行遍历
                var trs = $("tr", tableBody);
                //
                var voucherEntrys = [];

                //去除第一行和最后一行
                for (var i = 1; i < trs.length - 1; i++) {
                    //当前行
                    var currentTr = trs.eq(i);
                    //
                    var voucherEntry = {};
                    //entryid
                    var entryID = $(currentTr).attr("mentryid");
                    //摘要为必填项
                    var explanation = currentTr.find(explanationDiv).text().length > 0 ? currentTr.find(explanationDiv).data("value") : false;
                    //科目
                    var account = that.getAccountModel(null, currentTr);
                    //币别
                    var currency = account.MCurrencyDataModel;
                    //
                    var dir = currentTr.find(debitDiv).text().length > 0 ? 1 : -1;

                    //借方金额
                    var debit = dir == 1 ? currentTr.find(debitDiv).data("value") : null;
                    //贷方金额
                    var credit = dir == -1 ? currentTr.find(creditDiv).data("value") : null;
                    //
                    var debitValue = dir == 1 ? (+debit).toFixed(2) : null;
                    //
                    var creditValue = dir == -1 ? (+credit).toFixed(2) : null;
                    //如果这一行里面的都为空
                    if (that.isRowEmpty(currentTr)) {
                        //
                        continue;
                    }
                    //
                    if ((!explanation || explanation == "") && !isModule && !asModule && !isDraftDocVoucher) {
                        //如果为空
                        return [currentTr.find(explanationTd), explanationNotNull];
                    }
                    //
                    if ((!account || !account.MItemID) && !isModule && !asModule && !isDraftDocVoucher) {
                        //
                        return [currentTr.find(accountTd), accountNotNull];
                    }
                    //如果可以用，并且没有可以为的标志
                    if ((account.MIsCheckForCurrency && (!currency.MCurrencyID || !currency.MExchangeRate)) && !isModule && !asModule && !isDraftDocVoucher) {
                        //如果是业务单据转过来的，需要提醒的是
                        if (isDraftDocVoucher) {
                            //
                            return [currentTr.find(currencyTd), currencyNotNullOrDocIsNotForeign];
                        }
                        //
                        return [currentTr.find(currencyTd), currencyNotNull];
                    }
                    //如果可以用，并且没有可以为的标志 选了科目的话，必须做核算维度校验
                    if (!isModule && !asModule && account && account.MItemID) {
                        //
                        var validate = that.validateCheckGroupValue(currentTr);
                        //
                        if ($.isArray(validate)) {
                            //
                            return [currentTr.find(checkGroupTd), checkGroupNotNull + validate.join(',')];
                        }
                        //核算维度的model
                        var checkGroupValueModel = checkType.fillPaItemGroupID(validate);
                        //填充paItem的GroupID
                        voucherEntry.MCheckGroupValueModel = checkGroupValueModel;
                        //
                        voucherEntry.MCheckGroupValueID = null;
                    }
                    else if (account && account.MItemID) {
                        //
                        voucherEntry.MCheckGroupValueModel = account.MCheckGroupValueModel;
                    }

                    //验证借贷方是否填了值，可以填零，但是不可以不填
                    if (currentTr.find(creditDiv).text() == "" && currentTr.find(debitDiv).text() == "" && !isModule && !asModule) {
                        //
                        return [currentTr.find(debitTd), debitCreditNotNull];
                    }
                    voucherEntry.MID = voucher.MItemID;
                    //
                    voucherEntry.MEntryID = entryID;
                    //摘要
                    voucherEntry.MExplanation = explanation || "";
                    //科目
                    voucherEntry.MAccountID = account.MItemID || "";
                    //科目
                    voucherEntry.MAccountName = account.MFullName || "";
                    //
                    voucherEntry.MAccountModel = account;
                    //
                    voucherEntry.MCurrencyDataModel = currency;
                    //币种
                    voucherEntry.MCurrencyID = currency.MCurrencyID;
                    //借方金额
                    voucherEntry.MDebit = debitValue;
                    //贷方金额
                    voucherEntry.MCredit = creditValue;
                    //本位币金额
                    voucherEntry.MAmount = Math.abs(parseFloat(debitValue)) > 0 ? debitValue : creditValue;
                    //原币金额
                    voucherEntry.MAmountFor = (currency.MExchangeRate == 1 || currency.MExchangeRate == 0) ? voucherEntry.MAmount : currency.MAmountFor;

                    //判断金额正负向必须一致
                    if ((voucherEntry.MAmount > 0 && voucherEntry.MAmountFor < 0) || (voucherEntry.MAmount < 0 && voucherEntry.MAmountFor > 0)) {
                        if (Math.abs(parseFloat(debitValue)) > 0) {
                            return [currentTr.find(debitTd), plusminusNoEqual];
                        }
                        else {
                            return [currentTr.find(creditTd), plusminusNoEqual];
                        }
                    }

                    //汇率
                    voucherEntry.MExchangeRate = currency.MExchangeRate;
                    //序号
                    voucherEntry.MEntrySeq = voucherEntrys.length;
                    //方向
                    voucherEntry.MDC = dir;
                    //
                    voucherEntrys.push(voucherEntry);
                }
                //如果没有录入凭证
                if (voucherEntrys.length == 0 && !isDraftDocVoucher) {
                    //提醒只要要录入一条凭证，并且焦点跳转到第一个格子
                    return [$("td", tableBody).eq(1), voucherEntryNotNull];
                }

                if (voucherEntrys.length < 2 && !isDraftDocVoucher && !isModule) {
                    //凭证至少有两行分录，并且焦点跳转到第2行数据得备注
                    return [$(tableBody).find("tr").eq(2).find("td").eq(1), voucherEntryMoreThanOne];
                }

                //如果为-1
                if (value == "-1" && !isModule && !asModule && !isDraftDocVoucher) {
                    //返回需要提醒的信息
                    return [false, debitCreditImbalance];
                }
                //总金额
                voucher.MDebitTotal = voucher.MCreditTotal = +((+value).toFixed(2));
                //MItemID
                voucher.MItemID = voucherData.MItemID;

                //如果是保存为模板
                if (isModule || asModule) {
                    //快速码
                    voucher.MFastCode = fastcode;
                    //描述
                    voucher.MDescription = description;
                    //是单行模板还是多行模板
                    voucher.MIsMulti = voucherEntrys.length > 1;
                    //凭证分录
                    voucher.MVoucherModuleEntrys = voucherEntrys;
                } else {
                    //
                    voucher.MStatus = saved;
                    //编号
                    voucher.MNumber = GLVoucherHome.toVoucherNumber(voucherNumber);
                    //日期
                    voucher.MDate = date;
                    //附件数
                    voucher.MAttachments = attachmentAmount;
                    //凭证分录
                    voucher.MVoucherEntrys = voucherEntrys;
                }
                //返回
                return voucher;
            };
            //保存成功后把页面值为不可编辑状态
            this.saveSuccessCallback = function () {

            };
            //
            this.getBaseData = function () {
                //获取科目信息
                that.getAccountData(true, function (data) {
                    accountData = data;
                }, true);
                //获取联系人的核算维度信息
                checkType.getCheckTypeData(0, null, true, true);
            }
            //
            this.initDomSizeValue = function () {
                //
                if (fromDocVoucher) {
                    //
                    $(".vc-header").css({
                        "margin-top": 0 + "px"
                    });
                }
                //日期选择的时候
                $("#MDate").datebox({
                    width: 110,
                    required: true,
                    onSelect: function (date) {
                        //如果凭证日期小于总账启用日期，则重置为总账启用日期
                        if (date.getTime() < GLBeginDate.getTime()) {
                            //提醒用户
                            mDialog.message(HtmlLang.Write(LangModule.Common, "VoucherDateMustGreaterThanTheGLBeginDate", "凭证日期必须大于总账启用日期"));
                            //
                            $("#MDate").datebox("setValue", mDate.parse(defaultDate));
                        }
                        //如果是从业务单据转化来的，那日期不可以跨月
                        if (fromDocVoucher && (date.getFullYear() * 12 + date.getMonth() != mDate.parse(defaultDate).getFullYear() * 12 + mDate.parse(defaultDate).getMonth())) {
                            //提醒用户
                            mDialog.message(HtmlLang.Write(LangModule.GL, "DocVoucherCannotChangeDateToAnotherMonth", "业务单据生成的凭证日期不可修改为其他月份日期!"));
                            //
                            $("#MDate").datebox("setValue", mDate.parse(defaultDate));
                            //
                            return false;
                        }
                        //只有在新建凭证，并且凭证日期不等于本月的情况下采取获取下一个凭证号
                        if (!(voucherData && voucherData.MItemID && mDate.parse(voucherData.MDate).getFullYear() == date.getFullYear() && mDate.parse(voucherData.MDate).getMonth() == date.getMonth())) {
                            //获取下一个可以用的凭证号
                            that.getNextVoucherNumber(date);
                        }
                    }
                });
                $("#MNumber").numberspinner({
                    required: true,
                    width: 70,
                    min: 1,
                    max: +("9999999999".substring(0, +(mContext.getContext().MVoucherNumberLength))),
                    formatter: function (value) {
                        //
                        return GLVoucherHome.toVoucherNumber(value);
                        //检测次编号是否已经重复了
                    },
                    onChange: function (newValue, oldValue) {
                        //
                        oldValue = GLVoucherHome.toVoucherNumber(oldValue);
                        //
                        newValue = GLVoucherHome.toVoucherNumber(newValue);
                        //
                        var date = mDate.parse($("#MDate").datebox("getValue"));
                        //如果有更换，则需要去后台校验凭证编号是否合法
                        if (date) {
                            //对于已经保存的凭证，编码一致就不需要处理
                            if (voucherData && voucherData.MItemID) {
                                //
                                var vDate = mDate.parse(voucherData.MDate);
                                //
                                if (voucherData.MNumber && newValue == voucherData.MNumber && ((date.getFullYear() * 12 + date.getMonth()) == vDate.getFullYear() * 12 + vDate.getMonth())) {
                                    return;
                                }
                            }
                            //
                            mAjax.Post(isNumberUsedUrl, {
                                year: date.getFullYear(),
                                period: date.getMonth() + 1,
                                number: newValue
                            }, function (data) {
                                //
                                if (data) {
                                    //
                                    mDialog.confirm("[" + newValue + "]" + numberIsUsedLang, function () {
                                        //如果使用自动编号的话，就重新编号
                                        that.getNextVoucherNumber(date);
                                    }, function () {
                                        //不使用自动编号的话，就更新为原来的编号
                                        $("#MNumber").numberspinner("setValue", oldValue)
                                    });
                                }
                            });
                        }
                    }
                });
                //调整表格的宽度
                $(tableBody).width($(bodyDiv).width());
            };
            //整个页面不可编辑状态
            this.enableEditVoucher = function (voucher) {
                //如果已经结算或者审核了
                if (voucher.MStatus == approved) {
                    //
                    if (!voucher.MNoHideFastCode) {
                        //快速码那一列不显示
                        $(fastCodeTr).remove();
                        //合计哪一行，需要减少一列
                        var totalTd = $(tableTotal).find("td").eq(0);
                        //减少一列
                        totalTd.attr("colspan", $(tableHeader).find("th").not(":hidden").length - 2);
                    }
                    //编号不可编辑
                    $("#MNumber").numberspinner("disable", true);
                    $("#MNumber").numberspinner("readonly", true);
                    //日期不可编辑
                    $("#MDate").datebox("disable", true);
                    $("#MDate").datebox("readonly", true);
                    //附件数不可编辑
                    $("#Attachments").numberspinner("disable", true);
                    $("#Attachments").numberspinner("readonly", true);
                    //不可保存
                    $("#btnSave").linkbutton("disable");
                    //保存并新增
                    $("#btnSaveAndAddAnother").linkbutton("disable");
                    //保存并审核
                    $("#btnSaveAndApprove").linkbutton("disable");
                    //附件不可编辑
                    $("#divRelatedAttach .footer").remove();
                    //
                    return false;
                }
                //}
                return true;
            };
            //计算body的高度，滚动的时候不需要滚动头部和尾部
            this.resizeBody = function () {
                //
                $(body).height($(document).height() - $(body).offset().top - $(".m-toolbar-footer").outerHeight());
            };
            //处理从docVoucher过来的
            //重新计算某个combobox的宽度
            this.resizeCombobox = function ($td, $input, $panel) {
                //如果是combobox
                if ($panel) {
                    //
                    $input.parent().width($td.width() - 1);
                    //
                    $input.parent().height($td.height() - 2);
                    //
                    $panel.width($td.width() - 1);
                    //
                    $panel.parent().width($td.width() - 1);
                    //
                    $input.width($td.width() - 4);
                    //
                    $input.height($td.height() - 2);
                } else {
                    //如果是numberbox
                    $input.width($td.width());
                    //
                    $input.height($td.height() - 2);
                }

            };
            //
            this.bindEvent = function () {
                //
                $(window).resize(function () {
                    //
                    that.resizeTable();
                });
            };
            //初始化函数 showContact表示是否显示跟联系人，tracks表示跟踪项的数量
            this.init = function (_itemID, _number, _isCopy, _isReverse, _dir, _isModule, _fromDocVoucher, _docVoucherID, _entryAccountPair, _fromPeriodTransfer, transferTypeID, year, period, day, amount, percent0, percent1, percent2, versionID) {
                //是否是编辑
                itemID = _itemID;
                mNumber = _number;
                //
                isCopy = _isCopy == "1";

                isReverse = _isReverse == "1";
                //
                dir = _dir;
                //
                isSmartVersion = versionID == "1";
                //
                isModule = _isModule == "1";
                //
                operateTarget = isModule ? MODULE : VOUCHER;
                //
                fromPeriodTransfer = _fromPeriodTransfer == "1";
                //
                fromDocVoucher = _fromDocVoucher == "1";

                fromDocVoucherSetting = _fromDocVoucher == "1";
                //
                entryAccountPair = _entryAccountPair;
                //
                GLBeginDate = mDate.parse($("#hideGLBeginDate").val());
                //
                docVoucherID = _docVoucherID;
                //期末转结
                if (fromPeriodTransfer) {
                    //model数据，得以保存
                    periodTransferModel = {
                        MYear: year,
                        MPeriod: period,
                        MAmount: amount,
                        MTransferTypeID: transferTypeID,
                        MPercent0: percent0,
                        MPercent1: percent1,
                        MPercent2: percent2
                    }
                }
                //是否是期末调汇凭证
                isFinalTransfer = fromPeriodTransfer && (transferTypeID == PeriodTransferType.FinalTransfer);
                //
                that.initDomSizeValue();
                //初始化凭证
                that.initVoucherOrModule(year, period, day);
                //
                that.bindEvent();
            }
        };
        return GLVoucher;
    })();
    //
    window.GLVoucher = GLVoucher;
    //扩展一下GLVoucher里面的spinner-numberbox的数字显示格式
})()