(function () {

    /*
        这个方法主要处理从后台返回到前端的异常，后台返回异常编码，前端展示异常内容
        felson 206.9.18
    */
    var mError = (function () {

        //
        var mError = function () {
            //
            var that = this;
            //
            this.getErroeModel = function (code) {
                //
                var keys = Object.keys(MActionResultCodeEnum);
                //
                for (var i = 0; i < keys.length ; i++) {
                    //
                    if (MActionResultCodeEnum[keys[i]].Code == code) {
                        //
                        return MActionResultCodeEnum[keys[i]];
                    }
                }
            }
            //
            this.checkResultValid = function (result) {
                //表示有异常报出了
                if (result.Success === false || result.Codes) {
                    //
                    var html = "";

                    var isSystemException = false;
                    //
                    for (var i = 0; i < result.Codes.length ; i++) {
                        //
                        if (result.Codes[i] != MActionResultCodeEnum.Success.Code) {
                            //
                            var error = that.getErroeModel(result.Codes[i]);
                            //
                            html += "<div class='m-error-code-text' >" + (error.Message && (error.Message.indexOf('<') == 0) ? error.Message : mText.encode(error.Message)) + "</div>";

                            isSystemException = isSystemException || (result.Codes[i] == MActionResultCodeEnum.ExceptionExist.Code);
                        }
                    }
                    //
                    if (result.Messages && result.Messages.length > 0 && !isSystemException) {

                        for (var j = 0; j < result.Messages.length; j++)
                            //
                            html += "<div class='m-error-message-text' >" + (result.Messages[j] && (result.Messages[j].indexOf('<') == 0) ? result.Messages[j] : mText.encode(result.Messages[j])) + "</div>";
                    }
                    //
                    if (html.length > 0) {
                        //
                        mDialog.error(html, null, true);
                        //
                        return false;
                    }
                }
                //
                return true;
            }
        };
        //
        return mError;
    })();
    //
    window.mError = mError;
})()

//错误编码内容
var MActionResultCodeEnum = {

    /// <summary>
    /// 成功
    /// </summary>
    Success: {
        Code: 200,
        Message: HtmlLang.Write(LangModule.Common, "Success", "成功")
    },
    /// <summary>
    /// 丢失了Token
    /// </summary>
    AccessTokenMissing: {
        Code: 400,
        Message: HtmlLang.Write(LangModule.Common, "AccessTokenMissing", "登录令牌丢失")
    },
    /// <summary>
    /// 丢失了上下文
    /// </summary>
    ContextMissing: {
        Code: 401,
        Message: HtmlLang.Write(LangModule.Common, "ContactInvalid", "登录信息丢失")
    },
    /// <summary>
    /// 上下文过期
    /// </summary>
    ContextExpired: {
        Code: 402,
        Message: HtmlLang.Write(LangModule.Common, "ContextExpired", "登录已经过期")
    },
    /// <summary>
    /// 地址无法获取
    /// </summary>
    UnTracableAddress: {
        Code: 403,
        Message: HtmlLang.Write(LangModule.Common, "UnTracableAddress", "地址不可达")
    },
    /// <summary>
    /// IP地址不匹配
    /// </summary>
    AddressNotMatch: {
        Code: 404,
        Message: HtmlLang.Write(LangModule.Common, "AddressNotMatch", "登录所在的IP与系统IP不一致")
    },
    /// <summary>
    /// 上下文过期
    /// </summary>
    OrgExpired: {
        Code: 405,
        Message: HtmlLang.Write(LangModule.Common, "OrgExpired", "组织过期")
    },
    /// <summary>
    /// 当前组织其他用户正在进行操作，请稍后重试
    /// </summary>
    SyncBusy: {
        Code: 406,
        Message: HtmlLang.Write(LangModule.Common, "SyncBusy", "当前组织其他用户正在进行操作，请稍后重试！")
    },
    /// <summary>
    /// 没有权限
    /// </summary>
    AccessDenied: {
        Code: 501,
        Message: HtmlLang.Write(LangModule.Common, "AccessDenied", "没有权限")
    },
    /// <summary>
    /// 出现异常
    /// </summary>
    ExceptionExist: {
        Code: 502,
        Message: HtmlLang.Write(LangModule.Common, "ExceptionExistPelaseRetry", "系统出现异常, 请关闭此窗口后重试!")
    },

    /// <summary>
    /// 联系人被删除或者禁用
    /// </summary>
    MContactInvalid: {
        Code: 900,
        Message: HtmlLang.Write(LangModule.Common, "ContactInvalid", "联系人被删除或者禁用")
    },
    /// <summary>
    /// 员工被删除或者禁用
    /// </summary>
    MEmployeeInvalid: {
        Code: 901,
        Message: HtmlLang.Write(LangModule.Common, "EmployeeInvalid", "员工被删除或者禁用")
    },
    /// <summary>
    /// 商品项目被删除或者禁用
    /// </summary>
    MMerItemInvalid: {
        Code: 902,
        Message: HtmlLang.Write(LangModule.Common, "MerItemInvalid", "商品项目被删除或者禁用")
    },
    /// <summary>
    /// 费用项目被删除或者禁用
    /// </summary>
    MExpItemInvalid: {
        Code: 903,
        Message: HtmlLang.Write(LangModule.Common, "ExpItemInvalid", "费用项目被删除或者禁用")
    },
    /// <summary>
    /// 工资项目被删除或者禁用
    /// </summary>
    MPaItemInvalid: {
        Code: 904,
        Message: HtmlLang.Write(LangModule.Common, "PaItemInvalid", "工资项目被删除或者禁用")
    },
    /// <summary>
    /// 跟踪项被删除或者禁用
    /// </summary>
    MTrackItemInvalid: {
        Code: 905,
        Message: HtmlLang.Write(LangModule.Common, "TrackItemInvalid", "跟踪项被删除或者禁用")
    },
    /// <summary>
    /// 编号重复或者不合法
    /// </summary>
    MNumberInvalid: {
        Code: 906,
        Message: HtmlLang.Write(LangModule.Common, "NumberInvalid", "编号重复或者不合法")
    },
    /// <summary>
    /// 科目被删除或者禁用
    /// </summary>
    MAccountInvalid: {
        Code: 907,
        Message: HtmlLang.Write(LangModule.Common, "AccountInvalid", "科目被删除或者禁用")
    },
    /// <summary>
    /// 科目被删除或者禁用
    /// </summary>
    MCurrencyInvalid: {
        Code: 908,
        Message: HtmlLang.Write(LangModule.Common, "CurrencyInvalid", "币别被删除或者禁用")
    },

    /// <summary>
    /// 期间已经结账了
    /// </summary>
    MPeriodClosed: {
        Code: 909,
        Message: HtmlLang.Write(LangModule.Common, "PeriodClosed", "选择的期间已经结账了")
    },

    /// <summary>
    /// 期间在总账启用之前
    /// </summary>
    MPeriodBeforeStart: {
        Code: 910,
        Message: HtmlLang.Write(LangModule.Common, "PeriodBeforeStart", "选择的期在总账启用之前")
    },

    /// <summary>
    /// 凭证已经审核不可更新
    /// </summary>
    MVoucherApproved: {
        Code: 911,
        Message: HtmlLang.Write(LangModule.Common, "VoucherApproved", "凭证已审核")
    },

    /// <summary>
    /// 凭证未审核，不需要再反审核
    /// </summary>
    MVoucherUnapprove: {
        Code: 912,
        Message: HtmlLang.Write(LangModule.Common, "VoucherUnapprove", "凭证未审核")
    },

    /// <summary>
    /// 本期的期末结转类型已经存在了，需要删除前面一张
    /// </summary>
    MPeriodTransferExists: {
        Code: 913,
        Message: HtmlLang.Write(LangModule.Common, "PeriodTransferExists", "本期的期末结转类型已经存在了，请先删除")
    },

    /// <summary>
    /// 未完成初始化
    /// </summary>
    MInitBalanceOver: {
        Code: 914,
        Message: HtmlLang.Write(LangModule.Common, "InitBalanceOver", "总账未完成初始化")
    },
    /// <summary>
    /// 跟踪项分组不合法
    /// </summary>
    MTrackGroupInvalid: {
        Code: 915,
        Message: HtmlLang.Write(LangModule.Common, "TrackGroupInvalid", "跟踪项分组已经被禁用或者删除")
    },
    /// <summary>
    /// 科目含有子科目
    /// </summary>
    MAccountHasSub: {
        Code: 916,
        Message: HtmlLang.Write(LangModule.Common, "AccountHasSub", "选择的科目含有子科目")
    },
    /// <summary>
    /// 费用项目含有子项目
    /// </summary>
    MExpItemHasSub: {
        Code: 917,
        Message: HtmlLang.Write(LangModule.Common, "ExpItemHasSub", "选择的费用项目含有子项目")
    },
    /// <summary>
    /// 核算维度的值与科目的核算维度不匹配
    /// </summary>
    MCheckGroupValueNotMatchWithAccount: {
        Code: 918,
        Message: HtmlLang.Write(LangModule.Common, "CheckGroupValueNotMatchWithAccount", "核算维度的值与科目的核算维度不匹配")
    },
    /// <summary>
    /// 凭证分录为空
    /// </summary>
    MVoucherHasNotEntry: {
        Code: 919,
        Message: HtmlLang.Write(LangModule.Common, "VoucherHasNotEntry", "凭证分录为空")
    },
    /// <summary>
    /// 借贷不凭证
    /// </summary>
    MCreditDebitImbalance: {
        Code: 920,
        Message: HtmlLang.Write(LangModule.Common, "CreditDebitImbalance", "凭证分录借贷方不平衡")
    },
    /// <summary>
    /// 借贷不凭证
    /// </summary>
    MVoucherEntryHasNotAccountOrAccountNotMatchCheckGroup: {
        Code: 921,
        Message: HtmlLang.Write(LangModule.Common, "VoucherEntry", "凭证分录没有选择科目或者科目与核算维度值不匹配")
    },
    /// <summary>
    /// 凭证模板快速码重复
    /// </summary>
    MVoucherModuleFastCodeInvalid: {
        Code: 922,
        Message: HtmlLang.Write(LangModule.Common, "VoucherModuleFastCodeInvalid", "凭证模板快速码重复了")
    },
    /// <summary>
    /// 非外币核算的科目录入了外币 MCheckGroupValueNotMatchWithAccount
    /// </summary>
    MCurrencyNotMatchAccount: {
        Code: 923,
        Message: HtmlLang.Write(LangModule.Common, "CurrencyNotMatchAccount", "非外币核算的科目录入了外币")
    },
    /// <summary>
    /// 一张业务单据生成多多张非法凭证
    /// </summary>
    MOneDocCreatedMoreThanOneVoucher: {
        Code: 924,
        Message: HtmlLang.Write(LangModule.Common, "OneDocCreatedMoreThanOneVoucher", "一张业务单据生成多多张非法凭证")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MHasUnsettledPeriodBefore: {
        Code: 925,
        Message: HtmlLang.Write(LangModule.Common, "HasUnsettledPeriodBefore", "本期之前有未结账的期")
    },
    /// <summary>
    /// 存在不合法的业务单据
    /// </summary>
    MInvalidDocExsits: {
        Code: 926,
        Message: HtmlLang.Write(LangModule.Common, "InvalidDocExsits", "存在不合法的业务单据")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MAccountNumberDuplicated: {
        Code: 927,
        Message: HtmlLang.Write(LangModule.Common, "AccountNumberDuplicated", "科目编号重复了")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MAccountNameDuplicated: {
        Code: 928,
        Message: HtmlLang.Write(LangModule.Common, "AccountNameDuplicated", "科目名称重复了")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MAccountParentInvalid: {
        Code: 929,
        Message: HtmlLang.Write(LangModule.Common, "AccountParentInvalid", "科目父级科目不合法")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MAccountNewCheckGroupNotMathWithOldData: {
        Code: 930,
        Message: HtmlLang.Write(LangModule.Common, "AccountNewCheckGroupNotMathWithOldData", "科目新的核算维度与历史数据不符")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MAccountDetailIncomplete: {
        Code: 931,
        Message: HtmlLang.Write(LangModule.Common, "AccountDetailIncomplete", "科目基本信息不全")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MVoucherExplanationDuplicated: {
        Code: 932,
        Message: HtmlLang.Write(LangModule.Common, "VoucherExplanationDuplicated", "凭证摘要重复")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MVoucherExplanationInvalid: {
        Code: 933,
        Message: HtmlLang.Write(LangModule.Common, "VoucherExplanationInvalid", "凭证摘要不合法")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MVoucherInvalid: {
        Code: 934,
        Message: HtmlLang.Write(LangModule.Common, "VoucherInvalid", "凭证不合法")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MBillHasAlreadyCreatedVoucher: {
        Code: 935,
        Message: HtmlLang.Write(LangModule.Common, "BillHasAlreadyCreatedVoucher", "单据已经生成了凭证，不可重复生成!")
    },
    /// <summary>
    /// 本期之前有未结账的期
    /// </summary>
    MBankAccountInvalid: {
        Code: 936,
        Message: HtmlLang.Write(LangModule.Common, "BankAccountInvalid", "银行账号不合法!")
    },
    /// <summary>
    /// 资产类型编码重复
    /// </summary>
    MFixAssetsTypeNumberDuplicated: {
        Code: 937,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsTypeNumberDuplicated", "资产类型编码重复!")
    },
    /// <summary>
    /// 资产类型名称重复
    /// </summary>
    MFixAssetsTypeNameDuplicated: {
        Code: 938,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsNameDuplicated", "资产类型名称重复!")
    },

    /// <summary>
    /// 资产类型编码重复
    /// </summary>
    MFixAssetsNumberDuplicated: {
        Code: 939,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsNumberDuplicated", "资产编码重复!")
    },
    /// <summary>
    /// 资产类型名称重复
    /// </summary>
    MFixAssetsNameDuplicated: {
        Code: 940,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsNameDuplicated", "资产名称重复!")
    },
    /// <summary>
    /// 资产类型被引用
    /// </summary>
    MFixAssetsTypeRelatedFixAssets: {
        Code: 941,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsTypeRelatedFixAssets", "资产类型被引用!")
    },
    /// <summary>
    /// 资产已经开始折旧
    /// </summary>
    MFixAssetsAlreadyDepreciated: {
        Code: 942,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsAlreadyDepreciated", "资产已经开始折旧!")
    },
    /// <summary>
    /// 资产已经开始折旧
    /// </summary>
    MFixAssetsAlreadyDisposed: {
        Code: 943,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsAlreadyDisposed", "资产卡片已经被处置，不可重复处置!")
    },
    /// <summary>
    /// 资产已经开始折旧
    /// </summary>
    MFixAssetsIsNormal: {
        Code: 944,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsIsNormal", "资产卡片处于正常状态，不可撤销处置!")
    },
    /// <summary>
    /// 资产已经开始折旧
    /// </summary>
    MDeprecationVoucherCreated: {
        Code: 945,
        Message: HtmlLang.Write(LangModule.Common, "DeprecationVoucherCreated", "本期计提凭证已经生成，请删除后重试!")
    },
    /// <summary>
    /// 资产已经开始折旧
    /// </summary>
    MFixAssetsInvalid: {
        Code: 946,
        Message: HtmlLang.Write(LangModule.Common, "FixAssetsInvalid", "资产卡片不合法!")
    },
    /// <summary>
    /// 资产已经开始折旧
    /// </summary>
    MAccountAreRequiredForDepreciation: {
        Code: 947,
        Message: HtmlLang.Write(LangModule.Common, "AccountAreRequiredForDepreciation", "请设置计提折旧需要的科目!")
    },
    /// <summary>
    /// 资产已经开始折旧
    /// </summary>
    MDepreciatedVoucherHasBeenCreatedDuringItsHandledPeriod: {
        Code: 948,
        Message: HtmlLang.Write(LangModule.Common, "DepreciatedVoucherHasBeenCreatedDuringItsHandledPeriod", "资产处置的期间已经开始计提折旧了，请删除折旧凭证后再做撤销处置操作!")
    },
    /// <summary>
    ///  存在当前期间以后的已计提折旧的凭证，请先删除!
    /// </summary>
    MExistsCreatedDepreciationVoucherAfterThisPeriod: {
        Code: 949,
        Message: HtmlLang.Write(LangModule.Common, "ExistsCreatedDepreciationVoucherAfterThisPeriod", "当前期间以后期间存在计提折旧凭证，请先删除!")
    },
    /// <summary>
    /// 当前期之前存在未计提的期间，请先计提!
    /// </summary>
    MExistsUnDepreciatedPeriodBefore: {
        Code: 950,
        Message: HtmlLang.Write(LangModule.Common, "ExistsUndepreciatedPeriodBeforeCurrent", "当前期之前存在未计提的期间，请先计提!")
    },
    // <summary>
    /// 当前期计提生成的凭证已经审核，请先反审核凭证
    /// </summary>
    MDepreciationVoucherIsApproved: {
        Code: 951,
        Message: HtmlLang.Write(LangModule.Common, "DepreciationVoucherIsApproved", "当前期计提生成的凭证已经审核，请先反审核凭证!")
    },
    // <summary>
    /// 当前期计提生成的凭证已经审核，请先反审核凭证
    /// </summary>
    MExistsChangeAfterThisPeriod: {
        Code: 952,
        Message: HtmlLang.Write(LangModule.Common, "ExistsChangeAfterThisPeriod", "当前期间以后期间存在影响折旧的变更，不可当期删除计提折旧凭证或重新生成计提折旧凭证!")
    },
    // <summary>
    /// 请求已经提交，请不要重复操作
    /// </summary>
    MResubmitException: {
        Code: 953,
        Message: HtmlLang.Write(LangModule.Common, "ResubmitException", "请求已经提交，请不要重复操作!")
    },
    // <summary>
    /// 登陆信息已经丢失，请刷新页面重试
    /// </summary>
    MLoginInfoLost: {
        Code: 954,
        Message: HtmlLang.Write(LangModule.Common, "LoginInfoLost", "登陆信息已经丢失，请刷新页面重试!")
    },
    // <summary>
    /// 发票不存在或者已作废
    /// </summary>
    MFapiaoDeletedOrObsolete: {
        Code: 955,
        Message: HtmlLang.Write(LangModule.Common, "FapiaoDeletedOrObsolete", "发票不存在或者已作废!")
    },
    // <summary>
    /// 登陆信息已经丢失，请刷新页面重试
    /// </summary>
    MTableNotExists: {
        Code: 956,
        Message: HtmlLang.Write(LangModule.Common, "TableNotExists", "开票单不存在!")
    },

    // <summary>
    /// 登陆信息已经丢失，请刷新页面重试
    /// </summary>
    MFapiaoAmountCannotLargerThanTableAmount: {
        Code: 957,
        Message: HtmlLang.Write(LangModule.Common, "FapiaoAmountCannotLargerThanTableAmount", "发票金额不可大于开票单金额!")
    },
    // <summary>
    /// 登陆信息已经丢失，请刷新页面重试
    /// </summary>
    MExistsFapiaoInReconciledStatus: {
        Code: 958,
        Message: HtmlLang.Write(LangModule.Common, "ExistsFapiaoInReconciledStatus", " 存在已经勾兑的发票，无法为其设置勾兑状态!")
    },
    // <summary>
    /// 登陆信息已经丢失，请刷新页面重试
    /// </summary>
    MExistsFapiaoInNoReconciledStatus: {
        Code: 959,
        Message: HtmlLang.Write(LangModule.Common, "ExistsFapiaoInNoReconciledStatus", "存在已经设置为无需勾兑的发票，无法为其设置勾兑状态!")
    },
    // <summary>
    /// 存在已经设置为无需生成凭证的发票，无法为其设置生成凭证状态。
    /// </summary>
    MExistsFapiaoInNoCodingStatus: {
        Code: 960,
        Message: HtmlLang.Write(LangModule.Common, "ExistsFapiaoInNoCodingStatus", "存在已经设置为无需生成凭证的发票，无法为其设置生成凭证状态!")
    },
    // <summary>
    /// 存在已经生成凭证的发票，如需重新生成，请先删除凭证。
    /// </summary>
    MExistsFapiaoCreatedFapiao: {
        Code: 961,
        Message: HtmlLang.Write(LangModule.Common, "ExistsFapiaoCreatedFapiao", "存在已经生成凭证的发票，如需重新生成，请先删除凭证!")
    },
    // <summary>
    /// 存在联系人为空的发票，请先选择或者录入生成凭证的联系人
    /// </summary>
    MFapiaoCreateVoucherWithNoContact: {
        Code: 962,
        Message: HtmlLang.Write(LangModule.Common, "FapiaoCreateVoucherWithNoContact", "存在联系人为空的发票，请先选择或者录入生成凭证的联系人!")
    },
    // <summary>
    /// 存在借方科目为空的发票，请先选择借方科目。
    /// </summary>
    MFapiaoCreateVoucherWithNoDebitAccount: {
        Code: 963,
        Message: HtmlLang.Write(LangModule.Common, "FapiaoCreateVoucherWithNoDebitAccount", "存在借方科目为空的发票，请先选择借方科目!")
    },
    // <summary>
    /// 存在贷方科目为空的发票，请先选择贷方科目
    /// </summary>
    MFapiaoCreateVoucherWithNoCreditAccount: {
        Code: 964,
        Message: HtmlLang.Write(LangModule.Common, "FapiaoCreateVoucherWithNoCreditAccount", "存在贷方科目为空的发票，请先选择贷方科目!")
    },
    // <summary>
    /// 存在税科目为空的发票，请先选择税科目
    /// </summary>
    MFapiaoCreateVoucherWithNoTaxAccount: {
        Code: 965,
        Message: HtmlLang.Write(LangModule.Common, "FapiaoCreateVoucherWithNoTaxAccount", "存在税金额不为0但是税科目为空的发票，请先选择税科目!")
    },
    // <summary>
    /// 存在税科目为空的发票，请先选择税科目
    /// </summary>
    MFapiaoModuleFastCodeInvalid: {
        Code: 966,
        Message: HtmlLang.Write(LangModule.Common, "FapiaoModuleFastCodeInvalid", "发票模板编码重复!")
    },
    // <summary>
    /// 销项发票生成凭证对应期间已经结账，请先反结账该期间!
    /// </summary>
    MOutFapiaoPeriodIsClosed: {
        Code: 967,
        Message: HtmlLang.Write(LangModule.Common, "OutFapiaoPeriodIsClosed", "销项发票生成凭证对应期间已经结账，请先反结账该期间!")
    },
    // <summary>
    /// 销项发票中存在启用日期之前的数据，不能生成凭证！
    /// </summary>
    MOutFapiaoPeriodStartGL: {
        Code: 968,
        Message: HtmlLang.Write(LangModule.Common, "OutFapiaoPeriodStartGL", "销项发票中存在启用日期之前的数据，不能生成凭证！")
    },
    // <summary>
    /// 登陆信息已经丢失，请刷新页面重试
    /// </summary>
    MExistsReconciledFapiao: {
        Code: 980,
        Message: HtmlLang.Write(LangModule.Common, "TableExistsReconciledFapiao", "开票单存在已经勾对的发票，如需删除，请先删除发票勾对关系!")
    },
    // <summary>
    /// 系统存在超过所设置最大位数编码的凭证编号，请修改凭证最大位数
    /// </summary>
    MExistsVoucherNumberLagerThanSetLength: {
        Code: 981,
        Message: HtmlLang.Write(LangModule.Common, "MExistsVoucherNumberLagerThanSetLength", "系统存在超过所设置最大位数编码的凭证编号，请修改凭证最大位数!")
    },
    // <summary>
    /// 凭证编号的位数已达到系统设置的最大值，请【用户在设置-总账设置-凭证设置-凭证编号设置】进行调整
    /// </summary>
    MVoucherNumberNotMeetDemand: {
        Code: 982,
        Message: HtmlLang.Write(LangModule.Common, "MVoucherNumberNotMeetDemand", "凭证编号的位数已达到系统设置的最大值，请【用户在设置-总账设置-凭证设置-凭证编号设置】进行调整!")
    },
    // <summary>
    /// 凭证至少有两行分录!
    /// </summary>
    MVoucherEntryMoreThanOne: {
        Code: 983,
        Message: HtmlLang.Write(LangModule.Common, "VoucherEntryMoreThanOne", "凭证至少有两行分录!")
    },
    // <summary>
    /// 凭证已经被删除!
    /// </summary>
    MVoucherDeleted: {
        Code: 984,
        Message: HtmlLang.Write(LangModule.Common, "VoucherDeleted", "凭证已经被删除!")
    },
    // <summary>
    /// 不支持跨期审核!
    /// </summary>
    MVoucherSpanApprove: {
        Code: 985,
        Message: HtmlLang.Write(LangModule.Common, "VoucherSpanApprove", "不支持跨期批量操作!")
    },
    // <summary>
    /// 存在已经生成凭证的发票，请刷新发票速记列表后重新操作.
    /// </summary>
    MExistsFapiaoCreatedVoucher: {
        Code: 1000,
        Message: HtmlLang.Write(LangModule.Common, "MExistsFapiaoCreatedVoucher", "存在已经生成凭证的发票，请刷新发票速记列表后重新操作.")
    }
}