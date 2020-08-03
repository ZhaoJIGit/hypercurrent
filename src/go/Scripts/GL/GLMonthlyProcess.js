(function () {
    //
    var GLMonthlyProcess = (function () {
        //
        var GLMonthlyProcess = function (home) {
            //
            var that = this;
            var draft = "-1", saved = "0", approved = "1", unsettled = "0", settled = "1";
            //
            var periodSettled = false;
            //获取单个期末转结项url
            var getPeriodTranserUrl = "/GL/GLVoucher/GLGetPeriodTransfer";
            //测算的url
            var calculatePeriodTransferUrl = "/GL/GLVoucher/GLCalculatePeriodTransfer";
            //创建凭证
            var createVoucherUrl = "/GL/GLVoucher/GLCreateVoucher";
            //编辑凭证
            var viewVoucherUrl = "/GL/GLVoucher/GLVoucherEdit";
            //获取本月底的汇率
            var getMonthlyExchangeRateListUrl = "/BD/ExchangeRate/GetMonthlyExchangeRateList";
            //
            var updateExchangeRateListUrl = "/BD/ExchangeRate/UpdateExchangeRateList";
            //获取结算信息
            var getSettlementUrl = "/GL/GLVoucher/GetSettlement";
            //测试按钮
            var btnMeasure = ".mp-button-measure";
            //结账
            var btnSettlement = ".vl-settlement-button";
            //
            var btnUnsettlement = ".vl-unsettlement-button"
            //期选择控件
            var txtPeriod = ".mp-date-input";
            //查询按钮
            var btnSearch = ".mp-button-search";
            //
            var transferProfitLossVoucher = [];
            //
            var settlementInfo = {};
            //顶部
            var barlineTop = ".mp-barline-top";
            //遮罩
            var barlineMask = ".mp-mask-div";
            //期末调汇汇率调节
            var rateSetupDiv = ".mp-rate-setup-div";
            //
            var currencyRateSetupTable = ".mp-rate-setup-table";
            //
            var currencyRateSetupTableDiv = ".mp-rate-setup-table-div";
            //显示百分比的文本
            var barlinePercent = ".mp-barline-percent";
            //显示百分比长度的div
            var barlineProcess = ".mp-barline-process";
            //
            var rateSaveButton = ".mp-rate-save";
            //目前的百分比
            var currentStep = 0;
            //总共的步数
            var stepTotal = 7;

            //
            var newVoucherLang = HtmlLang.Write(LangModule.Common, "newvoucher", "New Voucher");
            //
            var unnecessaryCreateVoucher = HtmlLang.Write(LangModule.Common, "UnnecessaryCreateVoucher", "无需创建凭证");
            //
            var currencyRateSetupLang = HtmlLang.Write(LangModule.GL, "MonthlyCurrencyRateSetup", "期末汇率设置");
            //
            var numberDiscontinuous = HtmlLang.Write(LangModule.Common, "voucherNumberDiscontinuous", "The vouchers' number of this period is discontinuous, sure to reorder them before settlement?");
            //本位币
            var baseCyID = top.MegiGlobal.BaseCurrencyID;

            var unitPer = 1 + baseCyID + " = **" + HtmlLang.Write(LangModule.BD, "ForeignCurrency", "Foreign Currency");
            //单位
            var perUnit = 1 + HtmlLang.Write(LangModule.BD, "ForeignCurrency", "Foreign Currency") + " = " + "**" + baseCyID;

            var GL = "GL";
            //
            var approvedLang = HtmlLang.Write(LangModule.Common, "Approved", "Approved");
            //
            var unapprovedLang = HtmlLang.Write(LangModule.Common, "Unapproved", "Unapproved");

            //
            var topDiv = ".mp-top-div";
            //
            var buttomDiv = ".mp-buttom-div";
            //
            var transferDiv = ".mp-transfer-div";
            //
            var transferRecord = ".mp-transfer-record";
            //
            var transferTable = ".mp-transfer-table";
            //
            var transferRecord = ".mp-transfer-record";
            //
            var transferName = ".mp-transfer-name";
            //
            var transferAmount = ".mp-transfer-amount";
            //
            var transferDetail = ".mp-transfer-detail";
            //
            var voucherNumber = ".mp-voucher-number";
            //悬浮功能层
            var functionLayer = ".mp-function-layer";
            //
            var functionButton = ".mp-function-button";
            //
            var functionCreate = ".mp-function-create";
            //
            var functionView = ".mp-function-view";
            //
            var functionDelete = ".mp-function-delete";
            //
            var notneedCreate = ".mp-notneed-create";
            //
            var functionApprove = ".mp-function-approve";
            //
            var functionUnapprove = ".mp-function-unapprove";
            //
            var functionPrint = ".mp-function-print";
            //编辑
            var functionEdit = ".mp-function-edit";
            //测算
            var functionCalculate = ".mp-function-calculate";
            //创建状态
            var createStatus = ".mp-create-status";
            //
            var percentNumberbox = ".mp-percent-numberbox";
            //
            var editConfirm = ".mp-edit-confirm";
            //
            var transferDemoDiv = ".mp-transfer-demo-div";
            //
            var frozenTransferTypeID = -1;

            var faBegun = false;
            var faBegunYear = 0;
            var faBegunPeriod = 0;
            //
            var home = new GLVoucherHome();
            //从头到尾初始化凭证
            this.initAllPeriodTransfer = function (init) {

                //当前用户选择的日期
                var date = that.getPeriod();

                if (date === false) return;

                //显示遮罩
                that.showBarline(true);
                //默认为空
                var year = date.getFullYear(), period = date.getMonth() + 1;

                settlementInfo.MYear = year;
                settlementInfo.MPeriod = period;

                var fa = faBegun && (year * 12 + period >= faBegunYear * 12 + faBegunPeriod);
                //如果是初始化，则清除所有的dom
                $(transferDiv).remove();
                //结转销售成本
                that.initPeriodTransferDom(PeriodTransferType.TransferCost, '', init);
                //结算计提工资，暂时做隐藏
                //that.initPeriodTransferDom(PeriodTransferType.WagesOnAccount, '', init);
                //计提折旧
                !fa && that.initPeriodTransferDom(PeriodTransferType.Depreciation, '', init);
                //计提摊销
                that.initPeriodTransferDom(PeriodTransferType.AmortizationExpense, '', init);
                //计提税金
                //that.initPeriodTransferDom(PeriodTransferType.TaxOnAccount, '', init);
                //期末调汇
                that.initPeriodTransferDom(PeriodTransferType.FinalTransfer, '', init);
                //结转未交增值税
                that.initPeriodTransferDom(PeriodTransferType.TransferVAT, '', init);
                //所得税
                //结转季所得税
                that.initPeriodTransferDom(PeriodTransferType.IncomeTax, '', init);
                //结转损益
                that.initPeriodTransferDom(PeriodTransferType.TransferProfitLoss, '', init);
                //未分配利润
                that.initPeriodTransferDom(PeriodTransferType.TransferNDP, '', init);
                //年末才做结转未分配利润
                //查询完以后决定是否显示结算按钮
                that.checkSettlement(date);

                //更新期末处理页签显示
                home.updateMonthlyProcessStatus(date);
            };

            //获取所有的期末结转的数据
            this.initPeriodTransferDom = function (type, div, init) {
                //
                var $transferDiv = div || $(transferDiv + "-" + type + ":visible");
                //
                $transferDiv = $transferDiv.length == 0 ? $(transferDemoDiv + "-" + type).clone().appendTo($(buttomDiv)) : $transferDiv;
                //显示
                $transferDiv.show().removeClass((transferDemoDiv + "-" + type).trimStart('.')).addClass(transferDiv.trimStart('.') + "-" + type).addClass(transferDiv.trimStart('.')).attr("transferType", type);
                //
                that.initTransferDivSize($transferDiv);
                //当前用户选择的日期
                var date = that.getPeriod();
                //默认为空
                var year = date.getFullYear(), period = date.getMonth() + 1;

                var fa = $("#faBegun").val() == "1" && (year * 12 + period >= faBegunYear * 12 + faBegunPeriod);

                //如果是计提折旧，并且已经启用固定资产了，则此出弹出的是计提设置界面
                if (type == PeriodTransferType.Depreciation && !init && fa) {

                    return home.depreciatePeriodEdit({
                        Year: year,
                        Period: period
                    }, function () {
                        that.initPeriodTransferDom(type, div, true);
                    });
                }

                //一个一个从后台获取数据
                mAjax.post(init ? getPeriodTranserUrl : calculatePeriodTransferUrl,
                    {
                        model: {
                            MTransferTypeID: type,
                            MYear: year,
                            MPeriod: period
                        }
                    }, function (data) {
                        //成功
                        if (data) {
                            //数据
                            that.bindData2TransferDiv($transferDiv, data, init);
                        }
                        //先隐藏遮罩层
                        $(functionLayer).hide();
                    });

                GLVoucherHome.clearQueryDate();
            };
            //把期末计提的数据显示到div
            this.bindData2TransferDiv = function ($transferDiv, data, init) {
                //首先给table加上ID以及是否有编辑的内容
                $transferDiv.attr("MItemID", data.MItemID).attr("MVoucherID", data.MVoucherID || "");
                //显示基本的数据
                that.handleDataBaseInfo($transferDiv, data, init);
                //凭证编号
                that.showVoucherHref($transferDiv, data);
                //根据不同的结转类型，后面的内容才不相同
                !init && that.initPeriodTransferByType($transferDiv, data);
                //悬浮框
                that.initFunctionLayer($transferDiv, data, init);
                //
                $(transferDiv).width($(buttomDiv)[0].clientWidth - 2);
                //显示成功
                data.MTransferTypeID != PeriodTransferType.TransferNDP && that.increasePrecent();
            };
            //处理是否显示结转未分配利润的问题
            this.handleTransferNDP = function () {
                //需要结转损益凭证生成并审核 transferProfitLossVoucher.length > 0 && transferProfitLossVoucher[0].MVoucherStatus == 1 &&
                if (that.getPeriod().getMonth() == 11) {
                    //显示结转未分配利润的div
                    $(transferDiv + "-" + PeriodTransferType.TransferNDP).show();
                }
                else {
                    //显示结转未分配利润的div
                    $(transferDiv + "-" + PeriodTransferType.TransferNDP).hide();
                }
            };
            //处理表头显示已创建 未创建 已计算 显示计提名字 显示金额等
            this.handleDataBaseInfo = function ($transferDiv, data, init) {
                //结算名称
                $(transferName, $transferDiv).text(data.MTransferTypeName);
                //显示金额
                $(transferAmount, $transferDiv).attr("amount", data.MAmount || 0).text(mMath.toMoneyFormat(data.MAmount || 0));
                //
                var created = init && data.MVoucherID;
                //
                var uncreated = init && !data.MVoucherID;
                //
                var calculated = !init;
                //
                created ? $(createStatus, $transferDiv).find(".create").show() : $(createStatus, $transferDiv).find(".create").hide();
                //
                uncreated ? $(createStatus, $transferDiv).find(".uncreate").show() : $(createStatus, $transferDiv).find(".uncreate").hide();
                //
                calculated ? $(createStatus, $transferDiv).find(".calculated").show() : $(createStatus, $transferDiv).find(".calculated").hide();
                //
                if (data.MTransferTypeID == PeriodTransferType.TransferProfitLoss) {
                    //
                    transferProfitLossVoucher = data.MVoucherID ? [data] : [];
                }
            };
            //处理凭证编码可点击查看凭证信息的链接
            this.showVoucherHref = function ($div, data) {
                //
                var hrefHtml = "";
                //
                if (data.MVoucherID) {
                    hrefHtml += "<a href='###' onclick='mTab.addOrUpdate(\"" + data.MTransferTypeName + "\",\"" + viewVoucherUrl + "?MItemID=" + data.MVoucherID + "\", false, true, true, true)'>" + ("GL-" + data.MVoucherNumber) + "[" + (data.MVoucherStatus == 1 ? approvedLang : unapprovedLang) + "]" + "</a>,";
                }
                //
                hrefHtml = hrefHtml.length > 0 ? hrefHtml.trimEnd(',') : hrefHtml;
                //提示文本
                hrefHtml += "<span class='" + notneedCreate.trimStart(".") + "' style='display:" + (!data.MVoucherID && data.MNotNeedCreateVoucher ? "block" : "none") + "'>[" + unnecessaryCreateVoucher + "]</span>";
                //
                if (data.MErrorMessage) {
                    //
                    hrefHtml += "<span style='color:red;'>[" + data.MErrorMessage + "]</span>";
                }
                //
                $(voucherNumber, $div).empty().append(hrefHtml.trimEnd(','));
            };
            //根据结转类型的不同，显示到表格内部的内容也不同
            this.initPeriodTransferByType = function ($transferDiv, data) {
                //
                var detailHtml = "";
                //
                switch (parseInt(data.MTransferTypeID)) {
                    //结转销售成本
                    case PeriodTransferType.TransferCost:
                        //第一行显示本期营业收入
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //第二行显示结转百分比
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[1].MName + ":" + "<input type='text' class='" + percentNumberbox.trimStart('.') + "' value='" + data.MNameValueModels[1].MValue + "'/>%</div>";
                        //第三行，显示库存商品余额
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[2].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[2].MValue);
                        //加一行用来确定
                        detailHtml += "<a href='###' class='easyui-linkbutton easyui-linkbutton-yellow " + editConfirm.trimStart('.') + " vc-hide'>" + HtmlLang.Write(LangModule.Common, "Apply", "Apply") + "</a></div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        //确认按钮，点击确认的时候
                        $(editConfirm, $transferDiv).off("click").on("click", function () {
                            //获取结转百分比
                            var percent = $(percentNumberbox, $transferDiv).numberbox("getValue");
                            //主营业务收入
                            var value = data.MNameValueModels[0].MValue;
                            //结转金额
                            var amount = value * percent / 100.00;
                            //计算金额
                            $(transferAmount, $transferDiv).text(mMath.toMoneyFormat(amount)).attr("amount", amount);
                            //如果金额等于零
                            amount == 0 ? $(notneedCreate, $transferDiv).show() : $(notneedCreate, $transferDiv).hide();
                            //隐藏按钮
                            $(this).hide();
                            //标记为编辑结束
                            $(transferRecord, $transferDiv).attr("editing", 0);
                            //numberbox都可以输入
                            $(percentNumberbox, $transferDiv).numberbox("disable");
                        });
                        break;
                        //结算计提工资
                    case PeriodTransferType.WagesOnAccount:
                        //第一行显示应付职工薪酬余额
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        break;
                        //计提折旧
                    case PeriodTransferType.Depreciation:
                        //第一行显示上期累计折旧
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //第一行显示应付职工薪酬余额
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[1].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[1].MValue) + "</div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        break;
                        //计提摊销
                    case PeriodTransferType.AmortizationExpense:
                        //第一行显示上期待摊费用余额
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //第二行显示上期摊销费用
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[1].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[1].MValue) + "</div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        break;
                        //计提税金
                    case PeriodTransferType.TaxOnAccount:
                        //第一行显示本期应交增值税
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //第二行显示应交城市维护建设税税率
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[1].MName + ":" + "<input type='text' class='" + percentNumberbox.trimStart('.') + "' value='" + data.MNameValueModels[1].MValue + "'/>%</div>";
                        //第三行，显示教育费附加税率
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[2].MName + ":" + "<input type='text' class='" + percentNumberbox.trimStart('.') + "' value='" + data.MNameValueModels[2].MValue + "'/>%";
                        //加一行用来确定
                        detailHtml += "<a href='###' class='easyui-linkbutton  easyui-linkbutton-yellow " + editConfirm.trimStart('.') + " vc-hide'>" + HtmlLang.Write(LangModule.Common, "Apply", "Apply") + "</a></div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        //确认按钮，点击确认的时候
                        $(editConfirm, $transferDiv).off("click").on("click", function () {
                            //获取应交城市维护建设税税率
                            var percent1 = $(percentNumberbox, $transferDiv).eq(0).numberbox("getValue");
                            //获取教育费附加税率
                            var percent2 = $(percentNumberbox, $transferDiv).eq(1).numberbox("getValue");
                            //主营业务收入
                            var value = data.MNameValueModels[0].MValue;
                            //计算金额
                            $(transferAmount, $transferDiv).text(mMath.toMoneyFormat(value * percent1 / 100.00)).attr("amount", value);
                            //隐藏按钮
                            $(this).hide();
                            //标记为编辑结束
                            $(transferRecord, $transferDiv).attr("editing", 0);
                            //numberbox都可以输入
                            $(percentNumberbox, $transferDiv).numberbox("disable");
                        });
                        break;
                        //期末调汇
                    case PeriodTransferType.FinalTransfer:
                        //第一行显示 
                        //detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //
                        //$(transferDetail, $transferDiv).empty().append(detailHtml);
                        break;
                        //结转未交增值税
                    case PeriodTransferType.TransferVAT:
                        //第一行显示本期应缴增值税贷方余额
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue);
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        break;
                        //所得税
                    case PeriodTransferType.IncomeTax:
                        //第一行显示本年累计利润总额
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //第二行显示所得税税率
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[1].MName + ":" + "<input type='text' class='" + percentNumberbox.trimStart('.') + "' value='" + data.MNameValueModels[1].MValue + "'/>%</div>";
                        //第三行显示所得税贷方累计
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[2].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[2].MValue) + "";
                        //加一行用来确定
                        detailHtml += "<a href='###' class='easyui-linkbutton  easyui-linkbutton-yellow " + editConfirm.trimStart('.') + " vc-hide'>" + HtmlLang.Write(LangModule.Common, "Apply", "Apply") + "</a></div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        //确认按钮，点击确认的时候
                        $(editConfirm, $transferDiv).off("click").on("click", function () {
                            //获取所得税税率
                            var percent1 = $(percentNumberbox, $transferDiv).eq(0).numberbox("getValue");
                            //本年累计利润总额
                            var value = data.MNameValueModels[0].MValue;
                            //本年已交所得税
                            var value2 = data.MNameValueModels[2].MValue;
                            //计算应交
                            var payableValue = value * percent1 / 100 - value2;
                            //如果小于零则需要交钱
                            payableValue = payableValue < 0 ? 0 : payableValue;
                            //计算金额
                            $(transferAmount, $transferDiv).text(mMath.toMoneyFormat(payableValue)).attr("amount", payableValue);
                            //如果金额等于零
                            payableValue == 0 ? $(notneedCreate, $transferDiv).show() : $(notneedCreate, $transferDiv).hide();
                            //隐藏按钮
                            $(this).hide();
                            //标记为编辑结束
                            $(transferRecord, $transferDiv).attr("editing", 0);
                            //numberbox都可以输入
                            $(percentNumberbox, $transferDiv).numberbox("disable");
                        });
                        break;
                        //结转损益
                    case PeriodTransferType.TransferProfitLoss:
                        //第一行显示本期收入合计
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //第二行显示本期成本合计
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[1].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[1].MValue) + "</div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        break;
                        //未分配利润
                    case PeriodTransferType.TransferNDP:
                        //第一行显示本年利润统计
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[0].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[0].MValue) + "</div>";
                        //第二行显示结转未分配利润
                        detailHtml += "<div class='mp-detail-div'>" + data.MNameValueModels[1].MName + ":" + mMath.toMoneyFormat(data.MNameValueModels[1].MValue) + "</div>";
                        //
                        $(transferDetail, $transferDiv).empty().append(detailHtml);
                        break;
                    default:
                        break;
                }
                //初始化numberbox
                $(percentNumberbox, $transferDiv).numberbox({
                    min: 0,
                    precision: 2,
                    readonly: true,
                    disable: true,
                    width: 50,
                    onChange: function (newValue, oldValue) {
                        var $transferDiv = $(transferDiv + "-" + parseInt(data.MTransferTypeID) + ":visible");
                        //超过100%显示提示信息
                        if (newValue > 100) {
                            var warningMsg = HtmlLang.Write(LangModule.GL, "TransferPercentMoreThan100PercentWarning", "结转比例不可超过100%");
                            var parcentTr = $(transferDetail, $transferDiv).find(percentNumberbox).closest(".mp-detail-div");
                            parcentTr.find(".percent-warning").remove();
                            $("<label class='percent-warning' style='color:red;padding-left:8px;'>" + warningMsg + "</label>").appendTo(parcentTr);
                        }
                        else {
                            $(transferDetail, $transferDiv).find(".percent-warning").remove();
                        }
                    }
                });
                $(percentNumberbox, $transferDiv).numberbox("disable");
                //隐藏
                $(editConfirm, $transferDiv).hide();
            };
            //初始化鼠标移动上去的时候显示的编辑和生成凭证
            this.initFunctionLayer = function ($transferDiv, data, init) {
                //绑定事件
                $(transferRecord, $transferDiv).off("mouseover").on("mouseover", function () {
                    //如果不处于编辑状态显示功能层,而且必须有测算过，如果没有的话，鼠标放上不显示功能层
                    $(this).attr("editing") != "1" && that.showFunctionLayer($transferDiv, data, init);
                    //如果出于编辑的话，其他的隐藏
                    $(this).attr("editing") == "1" && $(functionLayer).hide();
                });
                //
                $(functionLayer).off("mouseleave").on("mouseleave", function () {
                    //隐藏
                    $(this).hide();
                    //
                    $(this).attr("transferType", "");
                });
                $(functionLayer).off("dblclick").on("dblclick", function () {
                    //隐藏
                    $(this).hide();
                    //
                    frozenTransferTypeID = $(this).attr("transferType");
                });
            };
            //测算金额
            this.calculateAmount = function () {
                //获取所有的
                $("div[class^='" + transferDiv.trimStart('.') + "']").each(function (index) {
                    //
                    var $div = $(this);
                    //
                    var type = $div.attr("transferType");

                    //当前用户选择的日期
                    var date = that.getPeriod();

                    if (date === false) return;
                    //默认为空
                    var year = date.getFullYear(), period = date.getMonth() + 1;

                    var fa = faBegun && (year * 12 + period >= faBegunYear * 12 + faBegunPeriod);

                    if (!(type == "2" && fa)) {
                        //
                        that.initPeriodTransferDom(type, $div, false);
                    }
                });
            };
            //显示功能层 div用来定位，data用来确定里面显示的编辑，生成和重新生成按钮
            this.showFunctionLayer = function ($div, data, init) {
                //如果里面有disable样式，则表示不能使用了
                if ($(functionLayer).hasClass("diasble") || data.MTransferTypeID == frozenTransferTypeID) {
                    //直接返回
                    return false;
                }
                else {
                    frozenTransferTypeID = -1;
                }
                //
                $(functionLayer).attr("transferType", data.MTransferTypeID);
                //可审核
                var canCreate = !init && (data.MNotNeedCreateVoucher === true ? false : true) && !periodSettled;
                //
                var canEdit = (data.MNeedEdit || data.MTransferTypeID == PeriodTransferType.FinalTransfer) && !periodSettled;
                //
                var canCalculate = !periodSettled;
                //
                var canView = data.MVoucherID ? true : false;
                //
                var canApprove = !periodSettled && canView && data.MVoucherStatus == 0;
                //
                var canDelete = !periodSettled && canView && data.MVoucherStatus == 0;
                //
                var canUnapprove = !periodSettled && data.MVoucherStatus == 1;
                //
                var canPrint = data.MVoucherID ? true : false;
                //
                canView ? $(functionView).show() : $(functionView).hide();
                //
                canEdit ? $(functionEdit).show() : $(functionEdit).hide();
                //
                canDelete ? $(functionDelete).show() : $(functionDelete).hide();
                //
                canApprove ? $(functionApprove).show() : $(functionApprove).hide();
                //
                canUnapprove ? $(functionUnapprove).show() : $(functionUnapprove).hide();
                //
                canCreate ? $(functionCreate).attr("MErrorMessage", data.MErrorMessage).show() : $(functionCreate).hide();
                //
                canCalculate ? $(functionCalculate).show() : $(functionCalculate).hide();
                //
                canPrint ? $(functionPrint).show() : $(functionPrint).hide();
                //如果啥都不能看，就不用显示遮罩层了
                if (!canView && !canEdit && !canCreate && !canDelete && !canApprove && !canUnapprove && !canPrint && !canCalculate) {
                    //直接返回吧
                    return false;
                }
                //编辑事件
                $(functionEdit).off("click").on("click", function () {
                    //隐藏
                    $(functionLayer).hide();
                    //如果是期末调汇的话，弹出的就是汇率维护界面
                    if (data.MTransferTypeID == PeriodTransferType.FinalTransfer) {
                        //
                        $.mDialog.show({
                            mTitle: currencyRateSetupLang,
                            mDrag: "mBoxTitle",
                            mContent: "id:currencyRateSetupDiv",
                            //mShowbg: false,
                            //mShowTitle: false,
                            mHeight: 400,
                            mWidth: 600,
                            mOpenCallback: function () {
                                //填充表格数据
                                that.getMonthlyExchangeRateData();
                                //
                                that.resizeCurrencyRateDivSize();
                                //
                                $(rateSaveButton).off("click").on("click", function (event) {
                                    //
                                    that.saveCurrencyExchangeRate();
                                });
                            }
                        });
                    }
                    else {
                        //标记为已编辑
                        $(transferRecord, $div).attr("editing", 1);
                        //显示确认按钮
                        $(editConfirm, $div).show();
                        //numberbox都可以输入
                        $(percentNumberbox, $div).numberbox("enable");
                    }
                });
                //生成凭证事件
                $(functionCreate).off("click").on("click", function () {
                    //有错误则提示
                    var errorMsg = $(this).attr("MErrorMessage");
                    if (errorMsg) {
                        mDialog.alert(errorMsg);
                        return;
                    }

                    //获取百分比
                    var numberbox = $(percentNumberbox, $div);
                    //如果是第一个
                    numberbox.length > 0 ? (data.MPercent0 = numberbox.eq(0).numberbox("getValue")) : "";
                    //如果有第二个
                    numberbox.length > 1 ? (data.MPercent1 = numberbox.eq(1).numberbox("getValue")) : "";
                    //
                    var amount = $(transferAmount, $div).attr("amount") || 0;
                    //
                    var func = function () {
                        //名字
                        var title = $(transferName, $div).text();
                        //提交
                        //跳转到编辑页面 
                        //mTab.addOrUpdate(title, createVoucherUrl + "?MItemID=" + (data.MItemID || "") + "&MTransferTypeID=" + data.MTransferTypeID + "&MPercent0=" + data.MPercent0 + "&MPercent1=" + data.MPercent1 + "&MYear=" + data.MYear + "&MPeriod=" + data.MPeriod + "&MAmount=" + amount, false, true, true, true);
                        $.mDialog.show({
                            mTitle: title,
                            mDrag: "mBoxTitle",
                            mShowbg: true,
                            mContent: "iframe:" + createVoucherUrl + "?MItemID=" + (data.MItemID || "") + "&MTransferTypeID=" + data.MTransferTypeID + "&MPercent0=" + data.MPercent0 + "&MPercent1=" + data.MPercent1 + "&MYear=" + data.MYear + "&MPeriod=" + data.MPeriod + "&MAmount=" + amount,
                            mCloseCallback: [function () {
                                //
                                $(transferDetail, $(transferDiv + "-" + data.MTransferTypeID)).empty();
                                //刷新
                                that.initPeriodTransferDom(data.MTransferTypeID, "", true);
                            }]
                        });
                        //最大化
                        $.mDialog.max();
                    };
                    //如果是结转销售成本，百分比高于100，则需要先提醒用户
                    if (data.MTransferTypeID == PeriodTransferType.TransferCost && data.MPercent0 > 100) {
                        //
                        mDialog.confirm(HtmlLang.Write(LangModule.GL, "TransferPercentMoreThan100Percent", "The percent of transfer has over 100% ,are you sure to create?"), func);
                        //
                        return false;
                    }
                    func();
                });
                //查看凭证事件
                $(functionView).off("click").on("click", function () {
                    //跳转到编辑页面 
                    mTab.addOrUpdate(data.MTransferTypeName, viewVoucherUrl + "?MItemID=" + (data.MVoucherID || ""), false, true, true, true);
                });
                //删除凭证
                $(functionDelete).off("click").on("click", function () {
                    //
                    home.deleteVoucher([data.MVoucherID], function () {
                        //
                        that.initPeriodTransferDom(data.MTransferTypeID, $div, init);
                    });
                });
                //审核凭证
                $(functionApprove).off("click").on("click", function () {
                    //
                    home.approveVoucher([data.MVoucherID], 1, function () {
                        //
                        that.initPeriodTransferDom(data.MTransferTypeID, $div, init);
                    });
                });
                //反审核
                $(functionUnapprove).off("click").on("click", function () {
                    //
                    home.approveVoucher([data.MVoucherID], 0, function () {
                        //
                        that.initPeriodTransferDom(data.MTransferTypeID, $div, init);
                    });
                });
                //打印
                $(functionPrint).off("click").on("click", function () {
                    //
                    home.printVoucher([data.MVoucherID].join(), function () {
                        //重新全部加载
                        that.initAllPeriodTransfer(init);
                    });
                });
                //重新测算
                $(functionCalculate).off("click").on("click", function () {
                    //
                    that.checkBeforeCalculate(function () {
                        that.initPeriodTransferDom(data.MTransferTypeID, $div, false);
                    }, data.MTransferTypeID == PeriodTransferType.TransferNDP);
                });
                //
                var $transferDiv = $(transferRecord, $div);
                //
                var headerOffsetTop = $(topDiv).offset().top + $(topDiv).height() + 10;
                //
                var diff = ($transferDiv.offset().top) < headerOffsetTop ? ($transferDiv.offset().top - headerOffsetTop) : 0;
                //
                var buttomOffset = 0;
                //
                var height = $transferDiv.outerHeight() + diff;
                //如果上面没有遮挡，看下方是否有遮挡
                if (diff == 0) {
                    //
                    var buttomOffset = $transferDiv.offset().top + $transferDiv.height() - $(buttomDiv).height() - $(buttomDiv).offset().top;
                    //如果底下显示不全
                    buttomOffset = buttomOffset > 0 ? buttomOffset : 0;
                }
                //
                $(functionLayer).stop();
                //
                $(functionLayer).animate({
                    //宽度
                    width: $div.outerWidth() + "px",
                    //高度
                    height: (height - buttomOffset) + "px",
                    //位置
                    left: $div.offset().left + "px",
                    //顶部
                    top: ($transferDiv.offset().top - diff) + "px"
                }, 200, "swing", function () {
                    //
                    $(functionButton).css({
                        "margin-top": diff + "px"
                    });
                }).show();
                //
                $(functionLayer).mScroll($(buttomDiv));
            };
            //获取单个期末转结
            this.getPeriodTransferData = function (type) {
                //

            };
            //显示本组织所有的币种以及有效期为当月月底的汇率
            this.getMonthlyExchangeRateData = function (callback) {

                var date = that.getPeriod();

                if (date === false) return;
                //
                mAjax.post(getMonthlyExchangeRateListUrl, { date: date }, function (data) {
                    //
                    if (data) {
                        //
                        that.initMonthlyCurrencyRate2Table(data);
                    }
                }, true);
            };
            //把本月的期末调汇的汇率显示到表格里面
            this.initMonthlyCurrencyRate2Table = function (data) {
                //编辑下标
                that.editIndex = null;
                //初始化数据
                $(currencyRateSetupTable).datagrid({
                    data: data,
                    resizable: true,
                    auto: true,
                    fitColumns: true,
                    pagination: false,
                    width: 580,
                    columns: [[
                         //常用币种
                         {
                             title: HtmlLang.Write(LangModule.Acct, "Currency", "Currency"), field: 'MTargetCurrencyID', width: 80, align: 'center', sortable: true
                         },
                         //正汇率
                         {
                             title: unitPer, field: 'MUserRate', width: 120, align: 'right', sortable: true, editor: {
                                 type: 'numberbox',
                                 options: {
                                     required: true,
                                     precision: 6,
                                     min: 0
                                 }
                             },
                             formatter: function (value) {
                                 return parseFloat(value || 0).toFixed(6);
                             }
                         },
                         //反汇率
                         {
                             title: perUnit, field: 'MRate', width: 120, align: 'right', sortable: true, editor: {
                                 type: 'numberbox',
                                 options: {
                                     required: true,
                                     precision: 6,
                                     min: 0
                                 }
                             },
                             formatter: function (value) {
                                 return parseFloat(value || 0).toFixed(6);
                             }
                         },
                         //生效日期
                         {
                             title: HtmlLang.Write(LangModule.Acct, "EffectiveDate", "EffectiveDate"), align: 'center', field: 'MRateDate', width: 100, sortable: true, formatter: $.mDate.formatter
                         }
                    ]],
                    onLoadSuccess: function () {

                    },
                    onClickCell: function (rowIndex, rowData, event) {

                        if (that.editIndex == rowIndex) return null;

                        if (that.editIndex != null) $(currencyRateSetupTable).datagrid("endEdit");

                        //结束编辑
                        $(currencyRateSetupTable).datagrid("beginEdit", rowIndex);

                        that.editIndex = rowIndex;

                        that.bindTableRowRateInputEvent(rowIndex);
                    },
                    onDblClickRow: function (rowIndex, rowData) {
                    },
                    onBeforeEdit: function (rowIndex, rowData) {
                        //
                        if (that.editIndex != null) {
                            //结束编辑
                            $(currencyRateSetupTable).datagrid("endEdit", that.editIndex);
                        }
                    },
                    onAfterEdit: function (rowIndex, rowData, changes) {
                        //结束编辑
                        that.editIndex = null;
                    }
                });
            };
            //计算底部的footer div
            this.resizeCurrencyRateDivSize = function () {
                //
                $(currencyRateSetupTableDiv).height($(currencyRateSetupTableDiv).parent().height() - $(".m-toolbar-footer", $(currencyRateSetupTableDiv).parent()).outerHeight() - 10);
            };
            //汇率输入的时候，正反之间的换算
            this.bindTableRowRateInputEvent = function (rowIndex) {
                //找到各个编辑的框
                var editors = $(currencyRateSetupTable).datagrid('getEditors', that.editIndex);
                //金额输入的框
                var userRateInputEditor = editors.where("x.field =='MUserRate'")[0].target;
                //外币金额输入的框
                var rateInputEditor = editors.where("x.field =='MRate'")[0].target;
                //
                $(userRateInputEditor).off("keyup.id").on("keyup.id", function (e) {
                    //
                    if (e.keyCode == 13) {
                        //
                        that.endEditAndGoToNextRow();
                    }
                    else {
                        //如果是金额输入框则需要更新差额
                        that.calculateRate($(this), $(rateInputEditor), 1);
                    }
                });
                //
                $(rateInputEditor).off("keyup.id").on("keyup.id", function (e) {
                    //
                    if (e.keyCode == 13) {
                        //
                        that.endEditAndGoToNextRow();
                    }
                    else {
                        //如果是金额输入框则需要更新差额
                        that.calculateRate($(this), $(userRateInputEditor), 0);
                    }
                });
            };
            //提交保存的汇率
            this.saveCurrencyExchangeRate = function () {
                //
                var rows = that.getCurrencyRateRows();

                var date = that.getPeriod();

                if (date === false) return;
                //
                if (rows && rows.length > 0) {
                    //
                    $(currencyRateSetupTableDiv).parent().mask("");
                    //
                    mAjax.submit(updateExchangeRateListUrl, { list: rows, date: date }, function (data) {
                        //
                        $(currencyRateSetupTableDiv).parent().unmask();
                        //
                        if (data && data.Success) {
                            //
                            mDialog.message(LangKey.SaveSuccessfully);
                            //关闭当前窗口
                            $.mDialog.close();
                        }
                    }, true);
                }
            };
            //获取汇率提交的每一行
            this.getCurrencyRateRows = function () {
                //
                if (that.editIndex != null) {
                    //先结束编辑
                    $(currencyRateSetupTable).datagrid("endEdit", that.editIndex);
                }
                //
                var rows = $(currencyRateSetupTable).datagrid("getRows");
                //
                for (var i = 0; i < rows.length; i++) {
                    //汇率不可为0
                    if (!rows[i].MUserRate && !rows[i].MRate) {
                        //提醒用户汇率不可为0
                        mDialog.error(HtmlLang.Write(LangModule.BD, "ExchangeMustMoreThanZero", "Exchange rate must greater than zero!"));
                        //
                        return false;
                    }
                }
                //
                return rows;
            };
            //计算汇率和返回率
            this.calculateRate = function (srcInput, targetInput, dir) {
                //
                var srcValue = srcInput.val();
                //
                var targetValue = targetInput.val();
                //如果是正向汇率 targetValue = 1 / srcValue;
                //
                targetValue = srcValue == 0 ? 0 : 1.0 / srcValue;
                //
                targetInput.numberbox("setValue", targetValue);
            };
            //结束编辑，并且跳转到下一行
            this.endEditAndGoToNextRow = function () {
                //
                var index = that.editIndex;
                //
                $(currencyRateSetupTable).datagrid("endEdit", index);
                //下一行开始编辑
                $(currencyRateSetupTable).datagrid("beginEdit", index + 1);
                //
                that.bindTableRowRateInputEvent(index + 1);
            };

            //初始化宽度
            this.initTransferDivSize = function ($transferDiv) {
                //
                $transferDiv = $transferDiv || $("div[class^='" + transferDiv.trimStart('.') + "']");
                //
                $transferDiv.width($(buttomDiv)[0].clientWidth - 2);
            };
            //在测算前需要检查的逻辑
            /*
            a)	所有银行科目都勾对了（月底）。如果没有，提醒用户要去勾对。
            b)	是否还有应该生成凭证的业务单据没有审核或生成凭证。如果有，提醒用户去管理凭证界面生成凭证。
            c)	凭证字号是否有断号。在点击结帐时如果凭证号不连续需要弹出框提示说“该期间的凭证号不连续，系统将自动进入整理，点击“是”进行整理，点击“否”手动进行。
            */
            this.checkBeforeCalculate = function (func, isNDP) {
                //如果结转损益凭证已经生成，需要先反审核
                if (!isNDP && transferProfitLossVoucher.length > 0) {
                    //提醒用户需要先反审核
                    mDialog.alert(HtmlLang.Write(LangModule.GL, "ProfitAndLossVoucherHasApproved", "该期结转损益凭证已经生成，请先删除该凭证."));
                    //
                    return false;
                }
                //如果是未分配利润，则需要结转损益凭证已经审核
                if (isNDP && (transferProfitLossVoucher.length == 0 || transferProfitLossVoucher[0].MVoucherStatus !== 1)) {
                    //提醒用户需要先反审核
                    mDialog.alert(HtmlLang.Write(LangModule.GL, "YouMustCreateAndApproveProfitAndLossVoucherFirst", "请先创建并审核结转损益凭证!"));
                    //
                    return false;
                }
                //获取当前期
                var date = mDate.parse($(txtPeriod).val() + "-01");
                //
                home.calculatePeriod(date, func);
            };
            //
            this.initDomSizeValue = function () {
                //默认为当前日期
                $(txtPeriod).val(GLVoucherHome.avaliablePeriod());
                //
                $(buttomDiv).width($(topDiv).width());
                //整体高度
                $(buttomDiv).height($("body").height() - $(buttomDiv).offset().top - 1);
                //其滚动的时候隐藏悬浮层
                $(buttomDiv).scroll(function () {
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
                //
                $(document).off("click").on("click", function () {
                    //隐藏
                    $(functionLayer).hide();
                });
                //
                $(buttomDiv).mScroll();
            };
            //初始化
            this.initPeriodTransfer = function (data) {

            };
            //生成凭证
            this.createVoucher = function () {

            };
            //刷新
            this.refresh = function () {
                that.initAllPeriodTransfer(true);
            }
            //结算
            this.settlePeriod = function (settlement) {
                //调用首页的方法
                home.settlePeriod(settlement, that.refresh, that.refresh);
            };
            //显示结算和反结算的按钮
            this.initSettlement = function () {
                //如果没有年和期，则隐藏结算按钮
                if (!settlementInfo.MYear || !settlementInfo.MPeriod) {
                    //
                    $(btnSettlement).hide();
                    //
                    $(btnUnsettlement).hide();
                }
                else if (settlementInfo.MStatus == settled) {
                    //
                    $(btnSettlement).hide();
                    //
                    $(btnUnsettlement).show();
                }
                else {
                    //
                    $(btnSettlement).show();
                    //
                    $(btnUnsettlement).hide();
                }
                //
                $(btnSettlement).off("click").on("click", function () {
                    //结算
                    that.settlePeriod(
                        {
                            MYear: settlementInfo.MYear,
                            MPeriod: settlementInfo.MPeriod,
                            MStatus: approved
                        });
                });
                //
                $(btnUnsettlement).off("click").on("click", function () {
                    //反结算
                    that.settlePeriod(
                        {
                            MYear: settlementInfo.MYear,
                            MPeriod: settlementInfo.MPeriod,
                            MStatus: saved
                        });
                });
            };

            //按钮的事件
            this.bindEvent = function () {
                //测算按钮
                $(btnMeasure).off("click").on("click", function () {
                    //
                    !periodSettled && that.checkBeforeCalculate(function () {
                        //测算
                        that.initAllPeriodTransfer(false);
                    });
                });
                //查询
                $(btnSearch).off("click").on("click", function () {
                    //
                    that.initAllPeriodTransfer(true);
                });
            };
            //编辑参数
            this.editParam = function () {

            };

            //检测本期是否已经结算，如果已经结算，不允许做测算操作
            this.checkSettlement = function (date) {
                //
                home.checkPeriodSettled(date, function (issettled) {
                    //
                    periodSettled = issettled;
                    //
                    issettled ? $(btnMeasure).linkbutton("disable") : $(btnMeasure).linkbutton("enable");
                    //设置结算 按钮状态
                    settlementInfo.MStatus = issettled ? settled : unsettled;
                    that.initSettlement();
                })
            };

            //展示百分比
            this.showBarline = function (show) {
                //如果是显示
                if (show) {
                    //
                    var srcWidth = $(barlineMask).attr("srcWidth") || $(barlineMask).width();
                    //遮罩层太宽了，需要去掉右边的20
                    $(barlineMask).width(srcWidth - 40);
                    //
                    $(barlineMask).attr("srcWidth", srcWidth);
                    //显示
                    $(barlineMask).show();
                    //先定位进度条的left
                    $(barlineTop).css({
                        left: (($(barlineMask).width() - $(barlineTop).width()) / 2) + "px",
                        top: ((($(barlineMask).height() - $(barlineMask).offset().top - $(barlineTop).height()) / 2) + $(barlineMask).offset().top) + "px"
                    });
                    $(barlineTop).show();

                }
                else {
                    $(barlineMask).hide();
                    $(barlineTop).hide();
                    //
                    $(barlineProcess).attr("step", 0);
                    //
                    $(barlineProcess).width(0);
                    //
                    $(barlinePercent).text("");
                }
                //先展示背景层
            }

            //初始化div的位置
            this.increasePrecent = function () {
                //进度条
                $(barlineProcess).stop();
                //
                var step = parseInt($(barlineProcess).attr("step") || 0) + 1;
                //百分数
                $(barlinePercent).text(step + "/" + stepTotal);
                //位置
                $(barlinePercent).css({
                    left: (step / stepTotal / 2.0 * $(barlineTop).width()).toFixed(0) + "px"
                });
                //进度条
                $(barlineProcess).animate({
                    width: (step / stepTotal * $(barlineTop).width()).toFixed(0) + "px"
                }, 100, "swing", function () {
                    if ($(barlineProcess).is(":hidden")) {
                        $(barlineProcess).width(0);
                    }
                });
                //
                $(barlineProcess).attr("step", step);
                //
                if (step == stepTotal) {
                    //表示完成了，取消遮罩
                    this.showBarline(false);
                    //处理结转未分配利润的
                    that.handleTransferNDP();
                    //初始是否滚动到某一条
                    that.handleScroll();
                }

            };
            //处理是否需要滚动到定位的位置
            this.handleScroll = function ($div) {
                //
                $div = $div || $("div[scroll]:visible");
                //如果有
                if ($div) {
                    //
                    var typeID = $div.attr("scroll") || 0;
                    //
                    var scrollDiv = $(transferDiv + "-" + typeID);
                    //定位到
                    $(buttomDiv).animate({ scrollTop: scrollDiv.offset().top - $(buttomDiv).offset().top });
                }
            };
            //获取当前的期
            this.getPeriod = function () {
                //当前用户选择的日期
                var date = $(txtPeriod).val();

                if (!date) {
                    mDialog.message(HtmlLang.Write(LangModule.GL, "PleaseSelectAPeriod", "请选择一个会计期间"));
                    return false;
                }
                //默认为空
                var year = mDate.DateNow.getFullYear(), period = mDate.DateNow.getMonth() + 1;
                //如果不是日期就不说了
                if (date && mDate.parse(date + "-01")) {
                    //
                    year = date.split('-')[0];
                    //
                    period = date.split('-')[1];
                }
                //返回一个日期
                return new Date(year, period - 1, 1);
            };
            //
            this.reset = function () {
                //
                $(barlineProcess).attr("step", "");
            };
            //设置参数
            this.setFilter = function (transferTypeID) {
                //
                $(transferDemoDiv + "-" + transferTypeID).attr("scroll", transferTypeID);
            };
            this.init = function () {

                faBegun = $("#faBegun").val() == "1";
                faBegunYear = +$("#faBegunYear").val();
                faBegunPeriod = +$("#faBegunPeriod").val();

                //当前用户选择的日期
                var date = that.getPeriod();

                if (date === false) return;
                //默认为空
                var year = date.getFullYear(), period = date.getMonth() + 1;

                var fa = faBegun && (year * 12 + period >= faBegunYear * 12 + faBegunPeriod);

                if (fa) stepTotal--;
                //
                that.reset();
                //
                that.initDomSizeValue();
                //
                that.initAllPeriodTransfer(true);
                //
                that.bindEvent();
            };
        };
        return GLMonthlyProcess;
    })();
    window.GLMonthlyProcess = GLMonthlyProcess;
})()