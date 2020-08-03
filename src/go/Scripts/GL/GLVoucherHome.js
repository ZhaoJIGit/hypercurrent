
//凭证类型
var PeriodTransferType =
    {
        /// <summary>
        /// 结转销售成本
        /// </summary>
        TransferCost: 0,
        /// <summary>
        /// 计提工资
        /// </summary>
        WagesOnAccount: 1,
        /// <summary>
        /// 计提折旧
        /// </summary>
        Depreciation: 2,
        /// <summary>
        /// 摊销待摊费用
        /// </summary>
        AmortizationExpense: 3,
        /// <summary>
        /// 计提税金
        /// </summary>
        TaxOnAccount: 4,
        /// <summary>
        /// 结转未交增值税
        /// </summary>
        TransferVAT: 5,
        /// <summary>
        /// 期末调汇
        /// </summary>
        FinalTransfer: 9,
        /// <summary>
        /// 计提所得税
        /// </summary>
        IncomeTax: 6,
        /// <summary>
        /// 结转损益
        /// </summary>
        TransferProfitLoss: 7,
        /// <summary>
        /// 结转未分配利润 TransferNonDistributionProfits
        /// </summary>
        TransferNDP: 8,
    };

//核算维度类型枚举
var CheckTypeEnum = {
    //
    MContactID: 0,
    //
    MEmployeeID: 1,
    //
    MMerItemID: 2,
    //
    MExpItemID: 3,
    //
    MPaItemID: 4,
    //
    MTrackItem1: 5,
    //
    MTrackItem2: 6,
    //
    MTrackItem3: 7,
    //
    MTrackItem4: 8,
    //
    MTrackItem5: 9
};

(function () {

    var GLVoucherHome = (function () {

        var GLVoucherHome = function () {

            //tab
            var tab = ".gl-main-tabs";
            //
            var isSmartVersion = false;
            //定义各个tab对应的index
            var home = 0, summary = 1, docVoucher = 2, monthlyProcess = 3, report = 4, tax = 5;

            //定义各个tab对应的index
            var sale = 0, purchase = 1, expense = 2, receive = 3, payment = 4, transfer = 5, salary = 6;

            var _voucherList, _docVoucherList, _monthlyProcess, _dashboard;

            //结算的URL
            var settlePeriodUrl = "/GL/GLVoucher/GLSettlePeriod";
            //预结算的URL
            var preSettlePeriodUrl = "/GL/GLVoucher/GLPreSettlePeriod";
            //重新编排
            var reorderVoucherNumberUrl = "/GL/GLVoucher/GLVoucherNumberReorder";
            //审核凭证
            var approveVoucherUrl = "/GL/GLVoucher/ApproveVoucher";
            //删除凭证
            var deleteVoucherUrl = "/GL/GLVoucher/GLDeleteVoucher";
            //导出凭证
            var exportVoucherUrl = "/GL/GLVoucher/Export?jsonParam=";
            //获取结算信息
            var getSettlementUrl = "/GL/GLVoucher/GetSettlement";
            //保存凭证
            var updateVoucherUrl = "/GL/GLVoucher/UpdateVoucher";
            //保存凭证模板
            var updateVoucherModuleUrl = "/FC/FCHome/UpdateVoucherModule";
            //保存凭证
            var updateVoucherByPeriodTransfer = "/GL/GLVoucher/GLUpdateVoucherByPeriodTransfer";
            //业务单据生成凭证
            var createDocVoucherUrl = "/GL/GLVoucher/CreateDocVoucher";
            //重新生成凭证
            var resetDocVoucherUrl = "/GL/GLVoucher/ResetDocVoucher";
            //删除业务单据生成凭证
            var deleteDocVoucherUrl = "/GL/GLVoucher/DeleteDocVoucher";
            //编辑业务单据生成凭证规则
            var docVoucherRuleEditUrl = "/GL/GLVoucher/GLDocVoucherRuleEdit";
            //查看凭证
            var editDocVoucherUrl = "/GL/GLVoucher/GLDocVoucherEdit";
            //获取映射信息的url
            var getDocVoucherUrl = "/GL/GLVoucher/GetDocVoucherModelList";
            //获取首页数据的URL
            var getGLDashboardDataUrl = "/GL/GLVoucher/GetGLDashboardData";

            var getRelatedVoucher2DeleteUrl = "/GL/GLVoucher/GetRelateDeleteVoucherList";

            //编辑凭证
            var editVoucherUrl = "/GL/GLVoucher/GLVoucherEdit";


            //获取销售单和采购单具体的单据类型
            var getDocMTypeUrl = "/GL/GLVoucher/GetDocMType";

            //获取科目的url
            var getAccountUrl = "/BD/BDAccount/GetAccountListWithCheckType";
            //获取摘要的url
            var getExplanationUrl = "/GL/GLVoucher/GetVoucherExplanationList?size=200";

            //获取快速码的url
            var getVoucherModuleListDataWithNoEntryUrl = "/FC/FCHome/GetVoucherModuleListDataWithNoEntry";
            //获取快速码的url
            var getVoucherModuleListDataWithEntryUrl = "/FC/FCHome/GetVoucherModuleListDataWithEntry";


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

            //获取巡检内容
            var inspectUrl = "/GL/GLVoucher/RoutineInpect";



            //
            var draft = "-1", saved = "0", approved = "1", unsettled = "0", settled = "1";

            //多语言信息
            var previewPrintTitle = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Print", "Print"), HtmlLang.Write(LangModule.GL, "Voucher", "Voucher")]);
            //
            var sureToDelete = HtmlLang.Write(LangModule.Common, "AreYouSureToDelete", "Are you sure to delete");
            //
            var sureToUnsettle = HtmlLang.Write(LangModule.GL, "AreyousuretoUnsettleThisPeriod", "<div style='text-align: left;font-weight: bold;'>反结账当前期间将会对后面期间产生以下影响，请先确认<br>1.后面已结账期间会被反结账<br>2.后面期间已审核凭证会被反审核<br>您是否确认反结账当前期间?</div>");

            var sureToSettle = HtmlLang.Write(LangModule.GL, "AreyousuretoCloseThisPeriod", "结帐后业务系统本期数据将被锁定, 不可对本期任何单据进行修改, 确认需要结账吗?");
            //
            var exportLang = HtmlLang.Write(LangModule.Common, "Exporting", "Exporting...");
            //
            var importLang = HtmlLang.Write(LangModule.GL, "ImportVoucher", "Import Voucher");
            //
            var existErrorLang = HtmlLang.Write(LangModule.GL, "ExistsErrorAsBelow", "There are the following questions：");
            //
            var warningLang = HtmlLang.Write(LangModule.Common, "Warning", "Tips:");
            //
            var approveSuccessLang = HtmlLang.Write(LangModule.Common, "ApproveSuccessfully", "Approve Successfully!");
            //
            var voucherNumberReorderSuccessfully = HtmlLang.Write(LangModule.Common, "VoucherNumberReorderSuccessfully", "Voucher number reordering successfully!");
            //
            var unapproveSuccessLang = HtmlLang.Write(LangModule.Common, "UnapproveSuccessfully", "Unpprove Successfully!");
            //
            var settleSuccessfully = HtmlLang.Write(LangModule.Common, "SettlementSuccessfully", "Settlement Successfully!");
            //
            var unsettleSuccessfully = HtmlLang.Write(LangModule.Common, "UnsettlementSuccessfully", "反结账成功!");
            //
            var numberDiscontinuous = HtmlLang.Write(LangModule.Common, "voucherNumberDiscontinuous", "The vouchers' number of this period is discontinuous, sure to reorder them before settlement?");
            //
            var operationFail = HtmlLang.Write(LangModule.Common, "operationfail", "Operation Failded!");
            //科目不可为空
            var debitAccountNotNull = HtmlLang.Write(LangModule.GL, "DebitAccountIsNull", "Debit Account Is Null!");
            //科目不可为空
            var creditAccountNotNull = HtmlLang.Write(LangModule.GL, "CreditAccountIsNull", "Credit Account Is Null!");
            //科目不可为空
            var taxAccountNotNull = HtmlLang.Write(LangModule.GL, "TaxAccountIsNull", "Tax Account Is Null!");
            //科目不可为空
            var pleaseDeleteVoucherFirst = HtmlLang.Write(LangModule.GL, "DeleteVoucherBeforeSetAccountEmpty", "凭证已经生成，不可清空相应科目，请先删除凭证!");
            //提醒
            var ExistsUnApprovedVouchers = HtmlLang.Write(LangModule.Common, "ExistsUnApprovedVouchers", "Exists unapproved vouchers");
            //提醒没有可操作的凭证
            var NoVouchers2Operate = HtmlLang.Write(LangModule.GL, "NoVoucher2Operate", "没有可操作的凭证!");
            //等待验证
            var confirmAwaiting = HtmlLang.Write(LangModule.GL, "ComfirmAwaiting", "Awaiting Comfirm");
            //已经结账
            var accountClosed = HtmlLang.Write(LangModule.GL, "AccountClosed", "Account Closed");
            //
            var processFinished = HtmlLang.Write(LangModule.Common, "Finished", "完成");
            //
            var processNotFinished = HtmlLang.Write(LangModule.Common, "notFinished", "未完成");
            //
            var createVoucherConfirm = HtmlLang.Write(LangModule.GL, "createVoucherConfirm", "已经生成凭证的业务单据，再次修改科目可能会导致原凭证的删除，是否确认保存?");
            //
            var periodInput = ".hp-period-input";
            //
            var that = this;

            //初始化Tab
            this.initTab = function (index, subIndex) {
                //初始化
                $(tab).tabsExtend({
                    //默认的显示标签
                    initTabIndex: index,
                    //选择标签函数
                    onSelect: function (index) {
                        //
                        that.showTab(index, subIndex);
                    }
                });
            };
            //查看业务单据 type指业务单据的大类，mType指业务单据的小类
            this.viewDoc = function (docID, type) {
                //
                var editDocUrl = "";
                //弹窗的名称
                var editDocTitle = "";

                var mType = "";

                if (type == sale || type == purchase) {
                    //获取单据对应的类型
                    mAjax.post(getDocMTypeUrl, { docID: docID }, function (data) {

                        mType = data;
                    }, false, false, false);
                }
                //
                var v = "View/";
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
                    case salary:
                        //编辑工资单
                        editDocUrl = receiveEditUrl + docID;
                        //名称
                        editDocTitle = HtmlLang.Write(LangModule.Common, "SalaryDetail", "Salary Detail");
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
                    mContent: "iframe:" + editDocUrl
                });
                $.mDialog.max();
            };
            //显示tab
            this.showTab = function (index, subIndex) {
                //加上class
                $("ul.tab-links").find("li:eq(" + index + ")").addClass("current").siblings().removeClass("current");

                //当前需要显示的内容
                var currentPartial = $(".gl-partial-" + index);
                //
                if (isSmartVersion && index == docVoucher) {
                    index++;
                }
                //根据index的不同，控制partial的显示
                currentPartial.show().siblings().hide();
                //初始化
                switch (parseInt(index || 0)) {
                    //先控制显示
                    case home:
                        if (!_dashboard) {
                            _dashboard = new GLDashboard(that);
                            _dashboard.init(isSmartVersion);
                        }
                        else {
                            _dashboard.update(isSmartVersion);
                        }
                        //加上初始化标志
                        currentPartial.attr("inited", 1);

                        break;
                    case summary:
                        //如果没有初始化，则初始化
                        if (!_voucherList) {
                            _voucherList = new GLVoucherList(that);
                            _voucherList.init();
                        }
                        else {
                            _voucherList.refresh();
                        }
                        //加上初始化标志
                        currentPartial.attr("inited", 1);
                        //
                        break;
                    case docVoucher:
                        if (!_docVoucherList) {
                            _docVoucherList = new GLDocVoucher(that);
                            _docVoucherList.init(subIndex);
                        }
                        else {
                            _docVoucherList.refresh();
                        }
                        //加上初始化标志
                        currentPartial.attr("inited", 1);
                        //
                        break;
                    case monthlyProcess:
                        //
                        _monthlyProcess = _monthlyProcess || new GLMonthlyProcess(that);
                        //如果没有初始化，则初始化
                        _monthlyProcess.init();
                        //加上初始化标志
                        currentPartial.attr("inited", 1);
                        //
                        _monthlyProcess.initTransferDivSize();
                        break;
                    case report:
                        break;
                    default:
                        break;
                }
            };
            //提醒用户没有可操作的凭证
            this.showNoVouchers2Operate = function () {
                //
                mDialog.message(NoVouchers2Operate);
            };
            //更新业务单据生成凭证的数量
            this.updateDoc2VoucherNumber = function (date) {
                //
                if (date) {
                    //异步请求
                    mAjax.post(getGLDashboardDataUrl, {
                        year: date.getFullYear(),
                        period: date.getMonth() + 1,
                        type: 2
                    }, function (data) {
                        //展示到页面上
                        data = data;
                        //C.业务单据生成凭证
                        that.initTabTitle(docVoucher, data.CreatedDocVoucherCount + "/" + (data.UncreatedDocVoucherCount + data.CreatedDocVoucherCount));
                    });
                }
            };
            //更新期末结转是否已经完成
            this.updateMonthlyProcessStatus = function (date) {
                //异步请求
                mAjax.post(getGLDashboardDataUrl, {
                    year: date.getFullYear(),
                    period: date.getMonth() + 1,
                    type: 3
                }, function (data) {
                    //展示到页面上
                    data = data;
                    //C.业务单据生成凭证
                    that.initTabTitle(monthlyProcess, (data.MonthProcessFinished || data.Settled) ? processFinished : processNotFinished);
                }, "", true);
            };

            //重置每个tab
            this.resetAllTab = function (v) {
                //
                isSmartVersion = isSmartVersion || v;
                //凭证列表
                that.resetTab(summary);
                //业务单据转化为凭证
                !isSmartVersion && that.resetTab(docVoucher);
                //期末月结
                that.resetTab(isSmartVersion ? docVoucher : monthlyProcess);

                _dashboard = undefined;
                _voucherList = undefined;
                _docVoucherList = undefined;
                _monthlyProcess = undefined;
            };

            //重置每个tab
            this.resetTab = function (index) {

                //
                if (isSmartVersion && index == docVoucher)
                    return;
                //当前需要显示的内容
                var currentPartial = $(".gl-partial-" + index);

                _voucherList = null; _docVoucherList = null;
                //初始化
                switch (parseInt(index || 0)) {
                    //列表
                    case summary:
                        //如果没有初始化，则初始化
                        new GLVoucherList().reset();

                        //加上初始化标志
                        currentPartial.attr("inited", 0);
                        break;
                    case docVoucher:
                        //如果没有初始化，则初始化
                        new GLDocVoucher().reset();

                        //加上初始化标志
                        currentPartial.attr("inited", 0);
                        break;
                    case monthlyProcess:
                        if (GLMonthlyProcess != undefined) {
                            //如果没有初始化，则初始化
                            new GLMonthlyProcess().reset();
                            //加上初始化标志
                            currentPartial.attr("inited", 0);
                        }
                        break;
                    case report:
                        break;
                    default:
                        break;
                }
            };
            //获取科目数据
            this.getAccountData = function (func, itemId, aysnc) {
                //
                var accountData = [];
                //异步获取
                mAjax.post(getAccountUrl, { itemID: itemId }, function (data) {
                    //
                    accountData = data || [];
                    //
                    accountData && $.isFunction(func) && func(accountData);
                }, "", "", aysnc);
                //只有在同步的情况下才返回数据
                if (!aysnc) {
                    //
                    return accountData;
                }
            };

            //获取摘要表
            this.getExplanationData = function (func, param, aysnc) {
                //
                var explanationData = [];
                //异步获取
                mAjax.post(getExplanationUrl, param, function (data) {
                    //
                    explanationData = data || [];
                    //
                    explanationData && $.isFunction(func) && func(explanationData);
                }, "", "", aysnc);
                //只有在同步的情况下才返回数据
                if (!aysnc) {
                    //
                    return explanationData;
                }
            };
            //获取快速码数据，没有分录的
            this.getVoucherModuleWithNoEntryData = function (func, param, aysnc) {
                //
                var voucherModuleData = [];
                //异步获取
                mAjax.post(getVoucherModuleListDataWithNoEntryUrl, param,
                function (data) {
                    //
                    voucherModuleData = data || [];
                    //加上一个多语言信息
                    for (var i = 0; i < voucherModuleData.length; i++) {
                        //加上一个行属性
                        voucherModuleData[i].group = HtmlLang.Write(LangModule.GL, "FastCode_Description", "快速码-描述");
                        voucherModuleData[i].MFullName = voucherModuleData[i].MFastCode + voucherModuleData[i].MDescription;
                    }
                    //
                    func && $.isFunction(func) && func(voucherModuleData);
                }, "", "", aysnc);
                //
                if (!aysnc) {
                    return voucherModuleData;
                }
            };
            //获取快速码数据，没有分录的
            this.getVoucherModuleWithEntryData = function (func, param, aysnc) {
                //
                var voucherModuleData = [];
                //异步获取
                mAjax.post(getVoucherModuleListDataWithEntryUrl, { pkIDS: param },
                function (data) {
                    //
                    voucherModuleData = data || [];
                    //
                    func && $.isFunction(func) && func(voucherModuleData);
                }, "", "", aysnc);
                //
                if (!aysnc) {
                    return voucherModuleData;
                }
            }
            //获取数据
            this.getDocVoucher = function (filter, func) {
                //获取数据
                mAjax.Post(getDocVoucherUrl, { filter: filter }, function (data) {
                    //
                    var docs = data.docs;
                    //
                    var settlement = data.settlement;
                    //成功回调
                    data && $.isFunction(func) && func(docs, settlement);
                }, "", true);
            };
            //删除
            this.deleteDocVoucher = function (docVouchers, func) {

                //先提醒用户是否确定删除
                mDialog.confirm(sureToDelete, function () {
                    //删除
                    mAjax.submit(deleteDocVoucherUrl, { list: docVouchers }, function (data) {
                        //
                        $.isFunction(func) && func(data);
                    });
                });
            };

            //重新生成草稿凭证
            this.resetDocVoucher = function (docIDs, docType, func) {

                mDialog.confirm(HtmlLang.Write(LangModule.GL, "SureToResetDoc2Voucher", "系统将会以默认规则重新创建凭证，已保存凭证信息将会被清除，是否确认将单据重新生成凭证?"), function () {

                    mAjax.submit(resetDocVoucherUrl, { docIDs: docIDs, docType: docType }, function (data) {

                        if (data.Success) {
                            mDialog.message(HtmlLang.Write(LangModule.GL, "ResetSuccessfully", "重置成功!"));
                            $.isFunction(func) && func(data);
                        }
                        else {
                            data.Message && mDialog.error(data.Message);
                        }
                    });
                });
            }

            //
            var existsCreatedVoucher = false;
            //业务单据生成凭证
            this.createDocVoucher = function (docVouchers, create, func) {

                //过滤掉已经审核的
                docVouchers = that.filterGetApprovedVoucher(docVouchers, false);


                //把里面所有的的create字段都附到model上面去,如果是需要创建凭证，则科目不可以为空
                for (var i = 0; i < docVouchers.length ; i++) {
                    //
                    var docVoucher = docVouchers[i];
                    //
                    docVouchers[i].Create = create == true;
                    //如果没有选择借方科目
                    if (!docVoucher.MCreditAccountFullName && (create || docVouchers[i].MVoucherNumber)) {
                        //提醒用户
                        mDialog.message(creditAccountNotNull);
                        //
                        return false;
                    }
                    //如果没有选择借方科目
                    if (!docVoucher.MDebitAccountFullName && (create || docVouchers[i].MVoucherNumber)) {
                        //提醒用户
                        mDialog.message(debitAccountNotNull);
                        //
                        return false;
                    }
                    //如果凭证已经创建，但是出现某种把科目选空的情况，则是不允许的，需要先删除凭证
                    if (docVoucher.MVoucherNumber && (
                        !docVoucher.MDebitAccountID
                        || !docVoucher.MCreditAccountID
                        || (docVoucher.MTaxEntryID && !docVoucher.MTaxAccountID))) {
                        //提醒用户
                        mDialog.message(pleaseDeleteVoucherFirst);
                        //
                        return false;
                    }
                    //如果没有选择借方科目
                    if (docVoucher.MTaxAmt != 0
                        //不是首付款单
                        && (docVoucher.MDocType != 3 && docVoucher.MDocType != 4)
                        //没有选税科目
                        && !docVoucher.MTaxAccountID
                        //是创建或者凭证保存
                        && (create || docVouchers[i].MVoucherNumber)) {
                        //提醒用
                        mDialog.message(taxAccountNotNull);
                        //
                        return false;
                    }
                    //
                    existsCreatedVoucher = existsCreatedVoucher || !!docVouchers[i].MVoucherNumber;
                }
                //
                var saveFunc = function () {  //异步提交
                    mAjax.submit(createDocVoucherUrl,
                        {
                            list: docVouchers,
                            create: create
                        }, function (data) {
                            //回调函数执行
                            $.isFunction(func) && func(data);
                        }, "");
                };
                //
                if (existsCreatedVoucher && !create) {
                    //
                    mDialog.confirm(createVoucherConfirm, saveFunc);
                }
                else {
                    //
                    saveFunc();
                }

            };

            //计提折旧
            this.depreciatePeriodEdit = function (filter, callback) {

                var title = HtmlLang.Write(LangModule.FA, "SetupDepreciation", "卡片折旧设置");
                //直接弹窗
                mDialog.show({
                    mTitle: title,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + "/FA/FAFixAssets/DepreciatePeriodEdit" + "?year=" + filter.Year + "&period=" + filter.Period,
                    mCloseCallback: callback
                });

                mDialog.max();
            }

            //编辑业务单据生成凭证规则
            this.editDocVoucherRule = function (docIDs, docType, closeCallback, filter) {

                //如果是固定资产，则另做处理
                if (docType == 6) {
                    var title = HtmlLang.Write(LangModule.FA, "BatchSetupDepreciation", "批量设置卡片折旧");
                    //直接弹窗
                    mDialog.show({
                        mTitle: title,
                        mDrag: "mBoxTitle",
                        mShowbg: true,
                        mContent: "iframe:" + "/FA/FAFixAssets/BatchSetupDepreciation" + "?year=" + filter.Year + '&period=' + filter.Period,
                        mCloseCallback: [closeCallback],
                        mPostData: { "itemIDs": docIDs }
                    });
                }
                else {

                    //弹窗
                    $.mDialog.show({
                        mTitle: HtmlLang.Write(LangModule.Common, "VoucherCreateDetails", "Voucher Create Details"),
                        mDrag: "mBoxTitle",
                        mShowbg: true,
                        mContent: "iframe:" + docVoucherRuleEditUrl + "?Type=" + docType,
                        mCloseCallback: [closeCallback],
                        mPostData: { "MDocIDs": docIDs }
                    });
                }
                $.mDialog.max();
            };
            //取有凭证编号，和没有凭证编号的，因为删除时候只能删除有凭证的，生成凭证只能对没有生成的再次生成，或者业务单据的可以生成凭证字段为true
            this.filterDocVoucherByHasVoucher = function (docVouchers, hasVoucher) {
                //新的一个
                var newDocVoucher = [];
                //遍历
                for (var i = 0; i < docVouchers.length ; i++) {
                    //需要凭证，并且有凭证编号，
                    if (hasVoucher && docVouchers[i].MVoucherNumber) {
                        newDocVoucher.push(docVouchers[i]);
                    }
                    else if (!hasVoucher && !docVouchers[i].MVoucherNumber) {
                        //没有凭证，并且没有凭证编号
                        newDocVoucher.push(docVouchers[i]);
                    }
                }
                //
                return newDocVoucher;
            };
            //获取可以是否可以创建凭证的
            this.filterCanCreateVoucher = function (docVouchers, canCreate) {
                //
                canCreate = canCreate == undefined ? true : canCreate;
                //新的一个
                var newDocVoucher = [];
                //
                for (var i = 0; i < docVouchers.length ; i++) {
                    //
                    if ((canCreate && docVouchers[i].CanCreateVoucher)
                    || (!canCreate && !docVouchers[i].CanCreateVoucher)) {
                        //装进去
                        newDocVoucher.push(docVouchers[i]);
                    }
                }
                //
                return newDocVoucher;
            };
            //获取已经审核的凭证和未审核的凭证
            this.filterGetApprovedVoucher = function (docVouchers, isApproved) {
                //
                isApproved = isApproved == undefined ? true : isApproved;
                //
                var newDocVoucher = [];
                //
                for (var i = 0; i < docVouchers.length ; i++) {
                    //
                    if ((isApproved && docVouchers[i].MVoucherStatus == 1)
                    || (!isApproved && docVouchers[i].MVoucherStatus != 1)) {
                        //装进去
                        newDocVoucher.push(docVouchers[i]);
                    }
                }
                return newDocVoucher;
            };
            //编辑业务单据生成的凭证
            this.editDocVoucher = function (jsonString, callback) {
                //弹窗
                $.mDialog.show({
                    mTitle: HtmlLang.Write(LangModule.Common, "VoucherDetails", "Voucher Details"),
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + editDocVoucherUrl + jsonString,
                    mCloseCallback: [callback]
                });
                $.mDialog.max();
            };
            //修改tab的名字
            this.initTabTitle = function (index, title) {
                //记账版 没有单据生成凭证页面
                if (that.isSmartVersion() && index > docVoucher) {
                    index--;
                }
                $(".gl-main-tabs li:eq(" + index + ") .title").text(title);
            };
            //初始化
            this.init = function (index, subIndex, version) {
                //
                isSmartVersion = version == 1;
                //初始化Tab
                that.initTab(index, subIndex);
                //
                window.lastQueryDate = mDate.DateNow;
            };



            //反结账
            this.openPeriod = function (settlement, func0) {

                //直接反结算
                mDialog.confirm(sureToUnsettle, function () {

                    //预结算请求
                    mAjax.submit(settlePeriodUrl, { model: settlement }, function (data) {
                        //提醒用户结算成功
                        mDialog.message(unsettleSuccessfully);
                        //整个页面更新
                        $.isFunction(func0) && func0(data);
                    }, "");
                });
            }



            //预结账
            this.preClosePeriod = function (settlement, func) {

                //先预结算
                mAjax.post(preSettlePeriodUrl, { date: new Date(settlement.MYear, settlement.MPeriod - 1, 1) }, function (data) {
                    //如果失败
                    if (!data.Success) {
                        //
                        var errorMessage = "<div style='text-align:left;margin-left: 10px;'>" + existErrorLang + "<br>" + data.Message + "</div>";
                        //显示失败的内容
                        mDialog.error(errorMessage);
                    }
                    else {
                        var title = HtmlLang.Write(LangModule.GL, "RoutineInspect", "结账巡检");
                        //直接弹窗
                        mDialog.show({
                            mTitle: title,
                            mDrag: "mBoxTitle",
                            mShowbg: true,
                            mContent: "iframe:" + inspectUrl + "?year=" + settlement.MYear + "&period=" + settlement.MPeriod,
                            mCloseCallback: func,
                            mWidth: 840,
                            mHeight: 500
                        });
                    }
                }, null, true);
            }



            //结账
            this.closePeriod = function (settlement, func0) {

                //直接反结算
                mDialog.confirm(sureToSettle, function () {

                    //预结算请求
                    mAjax.submit(settlePeriodUrl, { model: settlement }, function (data) {
                        //提醒用户结算成功
                        mDialog.message(settleSuccessfully);
                        //整个页面更新
                        $.isFunction(func0) && func0(data);
                    }, "");
                });

            }





            //结算 func0表示结算成功后要做的事情，func1表示如果遇到重新编排凭证编号需要做的事情
            this.settlePeriod = function (settlement, func0, func1) {
                //如果是结算，则需要先去检查是否符合结算的要求，，如果是反结算，则直接提醒用户确认就行
                if (settlement.MStatus == saved) {
                    //直接反结算
                    that.openPeriod(settlement, func0);
                }
                else {
                    //先提醒用户是否确定结算
                    this.preClosePeriod(settlement, func0);
                }
            };




            //期末计提测算的时候，需要按照结算校验的提醒来
            this.calculatePeriod = function (date, func0, func1) {
                //做结算前的检查
                that.preSettlePeriod(date, func0, func1, true);
            };

            //获取用户选的期
            this.getPeriod = function () {
                //
                var period = $(periodInput).val();

                if (!period) {
                    mDialog.message(HtmlLang.Write(LangModule.GL, "PleaseSelectAPeriod", "请选择一个会计期间"));
                    return false;
                }
                //
                var date = mDate.parse(period + "-01");

                return date;
            };

            //预结算,func0 是结算的方法 第一个表示重新编号后执行的方法
            this.preSettlePeriod = function (date, func0, func1, calculate) {
                //先遮罩整个页面
                $("body").mask("");
                //先预结算
                mAjax.post(preSettlePeriodUrl, { date: date, isCalculate: !!calculate }, function (data) {
                    //先遮罩整个页面
                    $("body").unmask();
                    //如果失败
                    if (!data.Success) {
                        //如果有断号
                        if (data.ObjectID == "1") {
                            //提醒用户有断号，需要重新编号
                            that.showVoucherNumberBroken(date.getFullYear(), date.getMonth() + 1, func1, data.Message);
                        }
                        else {
                            //
                            var errorMessage = "<div style='text-align:left;margin-left: 10px;'>" + existErrorLang + "<br>" + data.Message + "</div>";
                            //显示失败的内容
                            mDialog.error(errorMessage);
                        }
                    }
                    else {
                        //
                        if (data.Message) {
                            //
                            var confirmMessage = "<div style='text-align:left;margin-left: 10px;'>" + warningLang + "<br>" + data.Message + "</div>";
                            //预结算成功了,那还是要确认的
                            mDialog.confirm(confirmMessage, func0, true);
                        }
                        else {
                            $.isFunction(func0) && func0();
                        }
                    }
                });
            };

            //保存凭证
            this.saveVoucher = function (voucher, callback1, callback2) {
                //
                var url = updateVoucherUrl;

                voucher && mAjax.submit(url, { model: voucher }, callback1, callback2);
            };

            //保存凭证模板
            this.saveVoucherModule = function (module, callback1, callback2) {
                //
                var url = updateVoucherModuleUrl;
                //
                module && mAjax.submit(url, { model: module }, callback1, callback2);
            }

            //审核凭证
            this.approveVoucher = function (ids, status, func1) {
                //审核
                mAjax.submit(approveVoucherUrl, { ids: ids, status: status }, function (data) {
                    //
                    if (data.Success) {
                        //如果是审核,提醒审核成功
                        mDialog.message(status == saved ? unapproveSuccessLang : approveSuccessLang);
                        //
                        $.isFunction(func1) && func1(data);
                    }
                    else {
                        //
                        var failedMsg = HtmlLang.Write(LangModule.Common, "approvefialed", "Approve Failed") + " : " + (data.Message || "");
                        //提醒失败
                        mDialog.error(failedMsg);
                    }
                }, "");
            };
            //删除凭证的时候查看一下是否有其他需要删除的凭证
            this.getRelatedVoucher = function (itemIDs) {

                var relatedVouchers = [];

                //同步去获取，牺牲一点性能
                mAjax.Post(getRelatedVoucher2DeleteUrl, { pkIDS: itemIDs }, function (data) {

                    relatedVouchers = data || [];

                }, null, false, false);

                return relatedVouchers;
            };

            //初始删除凭证，预删除
            this.handleDeleteVouchers = function (itemIDs) {

                var confirmMessage = sureToDelete;

                //先查看是否有相关联的凭证是否需要删除
                var relatedVouchers = that.getRelatedVoucher(itemIDs);

                //如果有关联的凭证，需要做特殊处理
                if (relatedVouchers && relatedVouchers.length > 0) {

                    var approvedVouchers = [], vouchers = [];

                    for (var i = 0; i < relatedVouchers.length; i++) {

                        vouchers.push("GL-" + relatedVouchers[i].MNumber);
                        //将已经审核的凭证挑出来
                        if (relatedVouchers[i].MStatus == approved) {

                            approvedVouchers.push("GL-" + relatedVouchers[i].MNumber);
                        }
                    }

                    confirmMessage = HtmlLang.Write(LangModule.GL, "VouchersAsFollowWillBeDeleted", "如果删除当前凭证，以下凭证将会被关联删除:<br>") + vouchers.join(',');

                    //如果没有已经审核的凭证，则直接提醒确认就ok了
                    if (approvedVouchers.length == 0) {

                        return ["<div>" + confirmMessage + "<br>" + sureToDelete + "</div>", itemIDs.concat(relatedVouchers.select("MItemID"))];
                    }
                    else {
                        //如果存在已经审核的凭证，则直接不让其审核
                        confirmMessage += "<br>" + HtmlLang.Write(LangModule.GL, "VouchersAsFollowAreApproved", "以下凭证已经审核，无法被删除,请先反审核凭证:");

                        mDialog.alert("<div>" + confirmMessage + "<br>" + approvedVouchers.join(',') + "</div>");

                        return [false];
                    }

                }
                return [confirmMessage, itemIDs];
            }

            //删除凭证
            this.deleteVoucher = function (itemIDs, func) {
                //必须有勾选在做删除
                if (itemIDs.length > 0) {

                    var confirmMessage = that.handleDeleteVouchers(itemIDs);

                    if (confirmMessage[0] === false)
                        return;

                    //先提醒用户是否确定删除
                    mDialog.confirm(confirmMessage[0], function () {
                        //删除
                        mAjax.submit(deleteVoucherUrl, {
                            ids: confirmMessage[1]
                        }, function (data) {
                            //
                            if (data.Success) {
                                //提醒用户删除成功
                                mDialog.message(LangKey.DeleteSuccessfully);
                                //整个页面更新
                                $.isFunction(func) && func(data);
                            }
                            else {
                                //提醒用户删除成功
                                mDialog.error(HtmlLang.Write(LangModule.Common, "DeleteFailed", "Delete Failed!"));
                            }
                        }, "");
                    });
                }
            };

            //导出
            this.exportVoucher = function (params) {
                //
                location.href = exportVoucherUrl + escape($.toJSON(params));
                //显示正在导出
                mDialog.message(exportLang);
            };

            //导入
            this.importVoucher = function () {
                ImportBase.showImportBox('/BD/Import/Import/Voucher', importLang, 900, 520, function () {

                    //更新业务单据生成凭证数量
                    if ($(".gl-partial-3").length > 0)
                        that.updateDoc2VoucherNumber(that.getPeriod());
                    //
                    var voucherList = new GLVoucherList();
                    //重新获取数据
                    voucherList.getVoucherListData(voucherList.initVoucherList);
                });
            };

            //编辑凭证
            this.editVoucher = function (itemID, closeCallback) {
                //弹窗
                $.mDialog.show({
                    mTitle: HtmlLang.Write(LangModule.GL, "ViewVoucher", "Voucher Create Details"),
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + editVoucherUrl + "?MItemID=" + itemID,
                    mCloseCallback: [closeCallback]
                });

                $.mDialog.max();

            };

            //打印凭证
            this.printVoucher = function (itemID) {
                that.OpenPrintDialog(HtmlLang.Write(LangModule.GL, "Voucher", "Voucher"), $.toJSON({ MItemID: itemID }), "VoucherPrint")
            };

            this.OpenPrintDialog = function (title, queryParam, reportType) {
                title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Print", "Print"), title]);
                var param = $.toJSON({ reportType: reportType, jsonParam: escape(queryParam) });
                Print.previewPrint(title, param);
            };

            //检测是否有断号的凭证，本期
            this.checkVoucherNumberAndUnapproved = function (date, func) {
                //
                var year = date.getFullYear();
                //
                var period = date.getMonth() + 1;
                //
                mAjax.post(getSettlementUrl, {
                    model: {
                        MYear: year,
                        MPeriod: period
                    }
                }, function (data) {
                    //如果中间有断号
                    if (data) {
                        //是否有未审核的凭证
                        if (data.HasUnapprovedVoucher) {
                            //提醒有未审核的凭证，不让测算
                            mDialog.alert(ExistsUnApprovedVouchers);
                            //
                            return false;
                        }
                        //如果有断号
                        if (data.IsNumberBroken) {
                            //
                            that.showVoucherNumberBroken(year, period, func);
                            //
                            return false;
                        }
                        //如果没有断号就执行吧
                        $.isFunction(func) && func();
                    }
                });
            };
            //检测是否有断号
            this.showVoucherNumberBroken = function (year, period, func, message) {
                //提醒用户凭证中间有断号，是否需要重新整理
                mDialog.confirm(message ? ("<div>" + message + "</div>") : numberDiscontinuous, function () {
                    //计算请求
                    mAjax.post(reorderVoucherNumberUrl, { year: year, period: period }, function (data) {
                        //提醒用户重新编排成功
                        mDialog.message(voucherNumberReorderSuccessfully);
                        //整个页面更新
                        $.isFunction(func) && func(data);
                    }, "", true);
                });
            };
            //根据
            //检测某一期是否已经结算
            this.checkPeriodSettled = function (date, func) {
                //年
                var year = date.getFullYear();
                //期
                var period = date.getMonth() + 1;
                //
                mAjax.post(getSettlementUrl, {
                    model: {
                        MYear: year,
                        MPeriod: period
                    }
                }, function (data) {
                    //成功
                    $.isFunction(func) && func(data && data.MStatus == approved);
                });
            };
            this.isSmartVersion = function () {
                return $("#orgVersion").val() == "1";
            };
        };

        //返回
        return GLVoucherHome;
    })();
    //
    window.GLVoucherHome = GLVoucherHome;

    $.extend(GLVoucherHome, {
        ctx: null,
        Number2CN: function DX(n) {
            if (n == 0) {
                return "零元整";
            }
            if (!/^(0|[1-9]\d*)(\.\d+)?$/.test(n))
                return "数据非法";
            var unit = "仟佰拾亿仟佰拾万仟佰拾元角分", str = "";
            n += "00";
            var p = n.indexOf('.');
            if (p >= 0)
                n = n.substring(0, p) + n.substr(p + 1, 2);
            unit = unit.substr(unit.length - n.length);
            for (var i = 0; i < n.length; i++)
                str += '零壹贰叁肆伍陆柒捌玖'.charAt(n.charAt(i)) + unit.charAt(i);
            //return str.replace(/零(仟|佰|拾|角)/g, "零").replace(/(零)+/g, "零").replace(/零(万|亿|元)/g, "$1").replace(/(亿)万|壹(拾)/g, "$1$2").replace(/^元零?|零分/g, "").replace(/元$/g, "元整");
            return str.replace(/零(仟|佰|拾|角)/g, "零").replace(/(零)+/g, "零").replace(/零(万|亿|元)/g, "$1").replace(/^元零?|零分/g, "").replace(/元$/g, "元整");
        },
        toVoucherNumber: function (number) {

            var ctx = GLVoucherHome.ctx || mContext.getContext();
            var count = ctx.MVoucherNumberLength;
            var fillWithChar = ctx.MVoucherNumberFilledChar;

            if (!number || !$.isNumeric(number)) {
                return "";
            }

            number = parseInt(number.trimStart("0"));

            if (number.length > count) number = number.substring(0, count);

            if (!fillWithChar) {
                return number;
            }
            else {
                return "0000000000".substr(0, count - number.toString().length) + number;
            }
        },
        avaliablePeriod: function () {
            return $(".gl-avaliable-period").text();
        },
        avaliableDate: function () {
            //
            var text = $(".gl-avaliable-period").text();
            //
            var date = mDate.parse(text + "-01");
            //
            return date ? date : mDate.DateNow;
        },
        //滚动到顶层
        scrollToTop: function (selector) {
            $(selector)[0].scrollTop = 0;
        },
        //清除查询时间
        clearQueryDate:function(){
            
            window.lastQueryDate = new Date();
        },
        //表格分页数量
        pageSize: 20,
        //每页显示的数量
        pageList: [10, 20, 50, 100, 200],
        //获取业务表格的最后更新时间
        getUpdatedDocTableUrl: "/GL/GLVoucher/GetUpdatedDocTable",
        //检查业务系统数据是否更新
        checkDocTableUpdate: function () {
            //如果第一次过来
            if (window.lastQueryDate) {
                //
                mAjax.post(GLVoucherHome.getUpdatedDocTableUrl, { lastQueryDate: window.lastQueryDate }, function (data) {
                    //如果有新更新的数据
                    if (data && data.length > 0) {
                        //最新更新的数据
                        var docTable = data;
                        //
                        var confirmMsg = HtmlLang.Write(LangModule.Common, "BusinessTableAsBlowHasChangedYouWouldBetterRefreshPage", "以下的资金系统数据已经更新，您最好刷新此页面:</br>") + docTable.join(",");
                        //
                        mDialog.confirm(confirmMsg, function () {
                            //表示已经更新过了
                            window.lastQueryDate = "";
                            //
                            var updateButton = $(".hp-button-update:visible");
                            //
                            if (updateButton && updateButton.length > 0) {
                                //
                                updateButton.trigger("click");
                            }
                            else {
                                $(".gl-main-tabs li.current").trigger("click");
                            }
                            
                            GLVoucherHome.clearQueryDate();
                        }, function () {
                            //如果用户点击了取消，也表示更新过了
                            GLVoucherHome.clearQueryDate();
                        }, true);
                    }
                });
            }
            else {
                //当前更新时间
                window.lastQueryDate = new Date();
            }
        },
        lastQueryDate: "",
        //本位币
        baseCurrencyID: top.MegiGlobal.BaseCurrencyID
    });
})()