/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
/**
 * 固定资产类别边界
 */
var FAFixAssetsEdit = /** @class */ (function () {
    function FAFixAssetsEdit() {
        //新增div
        this.newButtonDiv = ".fa-new-div";
        //变更div
        this.changeButtonDiv = ".fa-change-div";
        //处置div
        this.handleDiv = ".fa-handle-div";
        //日志按钮
        this.divHistory = "#aHistory";
        //资产类别ID
        this.itemIDInput = "#itemID";
        //是否是复制
        this.isCopyInput = "#isCopy";
        //卡片状态
        this.cardStatusDiv = ".fa-cards-status";
        //资产编号前缀
        this.prefixInput = '.fa-fixassets-prefix';
        //资产编号
        this.numberInput = '.fa-fixassets-number';
        //资产名称
        this.nameInput = '.fa-fixassets-name';
        //采购日期
        this.purchseDateInput = '.fa-purchase-date';
        //资产类别
        this.typeInput = '.fa-fixassets-type';
        //清理期间
        this.disposePeriodInput = '.fa-dispose-period';
        //数量
        this.quantityInput = ".fa-fixassets-quantity";
        //美记开始计提期间
        this.depreciateFromTips = ".fa-depereciate-from-tips";
        //--------------------------------------------------------
        //折旧类型
        this.depreciateTypeInput = '.fa-depreciation-type';
        //开始折旧方式
        this.depreciateFromInput = '.fa-depreciation-from';
        //预计使用期数
        this.usefulPeriodsInput = '.fa-useful-periods';
        //固定资产科目
        this.fixAssetAccountInput = '.fa-fixassets-account';
        //折旧科目
        this.depreciationAccountInput = '.fa-depreciation-account';
        //费用科目
        this.expenseAccountInput = '.fa-expense-account';
        //核算维度编辑超链接
        this.checkTypeHref = '.fa-edit-checktype,.fa-edit-icon';
        //设置为默认科目
        this.setDefaultAccountInput = '.fa-setdefault-account';
        //-------------------------------------------------------------------------
        //原值
        this.originalValueInput = ".fa-original-value";
        //月折旧额
        this.periodDepreciationInput = ".fa-period-depreciation";
        //备注
        this.remarkInput = '.fa-fixassets-remark';
        //残值率
        this.rateOfSalvageInput = '.fa-fixassets-rateofsalvage';
        //预计残值
        this.expectedSalvageInput = '.fa-expected-salvage';
        //残值率
        this.prepareDecreaseInput = '.fa-prepare-decrease';
        //已折旧期间
        this.depreciatedPeriodsInput = '.fa-depreciated-periods';
        //累计折旧
        this.depreciatedValueInput = '.fa-depreciated-value';
        //已折旧期间
        this.netValueInput = '.fa-net-value';
        //变更需要显示的内容
        this.changeDiv = '.fa-change-info';
        //变更从当期开始印象
        this.changeFromInput = '.fa-change-from';
        //追溯调整
        this.backAdjustInput = '.fa-back-adjust';
        this.readonly = false;
        //保存
        this.saveButton = '#aSave';
        //保存并复制
        this.saveAndCopyButton = '#aSaveAndCopy';
        //保存并新增
        this.saveAndNewButton = '#aSaveAndNew';
        //变更
        this.changeButton = '#aChange';
        //是否是变更
        this.isChange = false;
        //处置
        this.handleButton = '#aHandle';
        //售出
        this.sellButton = '#aSell';
        //报废
        this.disposeButton = '#aDispose';
        //处置的顶部
        this.handleTopButton = '#aHandleTop';
        //撤销处置
        this.unhandleButton = '#aUnhandle';
        //核算维度设置
        this.divCheckGroupSetup = "#divCheckGroupSetup";
        //核算维度展示
        this.divCheckGroupTips = '#divCheckGroupTips';
        //固定资产科目
        this.fixAssetsAccount = {};
        //累计折旧科目
        this.depreciationAccount = {};
        //费用折旧科目
        this.expenseAccount = {};
        //原始的卡片model，用来判断是否变更了
        this.srcMode = {};
        //所设置的核算维度值
        this.checkGroupValueModel = {};
        //当前页面就用一个科目列表数据
        this.accountData = [];
        this.glHome = new GLVoucherHome();
        this.home = new FAHome();
    }
    //初始化函数
    FAFixAssetsEdit.prototype.init = function () {
        var _this = this;
        this.fixAssetsID = $(this.itemIDInput).val();
        this.isCopy = $(this.isCopyInput).val() == '1';
        this.minDepreciateDate = mDate.parse($("#minDepreciateDate").val());
        //加载数据
        this.home.getFixAssetModel(function (data) {
            $("body").mask("");
            _this.loadData(data);
            _this.initEventAfterLoadData();
            _this.initDom();
            //初始化页面事件
            _this.initEvent();
        }, this.fixAssetsID, this.isCopy);
        //如果是复制的话，需要把id置空
        if (this.isCopy) {
            this.fixAssetsID = null;
            $(this.itemIDInput).val('');
        }
    };
    //初始化页面的内容
    FAFixAssetsEdit.prototype.initDom = function () {
        $(this.handleTopButton).splitbutton({
            menu: $('#divHandle')
        });
        $(this.depreciateFromTips).mTitle();
    };
    //重新加载科目列表
    FAFixAssetsEdit.prototype.reloadAccountList = function (d) {
        this.accountData = d;
        //重新加载
        $(this.fixAssetAccountInput).combobox("loadData", d);
        $(this.depreciationAccountInput).combobox("loadData", d);
        $(this.expenseAccountInput).combobox("loadData", d);
    };
    //加载一条数据
    FAFixAssetsEdit.prototype.loadData = function (data) {
        var _this = this;
        this.readonly = data.MStatus > 0;
        //原始数据保存起来
        this.srcMode = data;
        //如果卡片是已经保存的，则需要显示卡片的状态
        if (data.MItemID != null && data.MItemID != undefined && data.MItemID.length > 0) {
            //显示卡片的基本信息 正常 已处置 已报废等
            $(this.cardStatusDiv + " div").eq(0).text(HtmlLang.Write(LangModule.FA, "BaseInfoAndStatusOfFixAssets", "基本信息") + " [ " + this.home.getFixAssetsStatus(data.MStatus, data) + " ] ");
        }
        //资产类型编号
        $(this.numberInput).numberspinner({
            required: true,
            min: 1,
            max: 99999999999999999999,
            validType: ['length[1,20]'],
            height: 26,
            readonly: this.readonly,
            formatter: function (val) {
                val = val == undefined || val == null || val == '' ? 1 : val;
                return '00000000000000000000'.substring(0, data.MPrefixModel.MNumberCount - val.toString().length) + val;
            }
        });
        //资产编号前缀
        $(this.prefixInput).validatebox({ required: true, validType: ['length[1,50]'] });
        //资产名称
        $(this.nameInput).initLangEditor((data.MultiLanguage == null || data.MultiLanguage == undefined || data.MultiLanguage.length == 0) ? null : $.extend(true, {}, data.MultiLanguage[0]));
        //名称是必填的
        $(this.nameInput).validatebox({ required: true, validType: ['length[1,200]'] });
        //备注最大是500个字符
        $(this.remarkInput).validatebox({ validType: ['length[0,500]'] });
        //预计使用期数不可为空
        $(this.usefulPeriodsInput).numberspinner({
            required: true,
            min: 1,
            max: 99999999999999999999,
            height: 26,
            readonly: this.readonly,
            onChange: function (val) {
                $(_this.rateOfSalvageInput).trigger("keyup.fa");
            }
        });
        //原值
        $(this.originalValueInput).numberbox({
            min: 0,
            required: true
        });
        //月折旧额
        $(this.periodDepreciationInput).numberbox({
            //min: 0,
            required: true,
            validType: ["ge [0]"]
        });
        //累计折旧
        $(this.depreciatedValueInput).numberbox({
            min: 0,
            required: true
        });
        //净值
        $(this.netValueInput).numberbox({
            min: 0,
            required: true
        });
        //减值准备
        $(this.prepareDecreaseInput).numberbox({
            min: 0,
            required: true
        });
        //数量
        $(this.quantityInput).numberspinner({
            min: 1,
            required: true
        });
        //采购日期是必填的
        $(this.purchseDateInput).datebox({
            required: true,
            readonly: this.readonly,
            onSelect: function (val) {
                _this.calculaDepreciateFromPeriod();
                _this.calculateDepreciatedPeriods(val, null);
            }
        });
        //默认为当天
        $(this.purchseDateInput).datebox('setValue', mDate.DateNow);
        //资产类别
        this.home.getFixAssetTypeList(function (typeList) {
            for (var i = 0; i < typeList.length; i++) {
                typeList[i].MName = mText.encode(typeList[i].MNumber + "_" + typeList[i].MName);
            }
            $(_this.typeInput).combobox({
                data: typeList,
                required: true,
                readonly: _this.readonly,
                textField: 'MName',
                valueField: 'MItemID',
                onLoadSuccess: function () {
                    $(_this.typeInput).combobox('setValue', data.MFATypeID);
                },
                onSelect: function (val) {
                    _this.bindFixAssetDataByType(val);
                }
            });
        });
        //数量
        $(this.quantityInput).numberbox('setValue', data.MQuantity);
        //上次的折旧开始方式
        this.setDepreciateFromCurrentPeriod(data.MDepreciationFromCurrentPeriod);
        //折旧方式
        $(this.depreciateTypeInput).combobox({
            width: 200,
            height: 26,
            data: this.home.getDepreciationTypeArray(),
            textField: 'name',
            valueField: 'value',
            readonly: true,
            disabled: true,
            required: true,
            onLoadSuccess: function () {
                $(_this.depreciateTypeInput).combobox("setValue", (data.MDepreciationTypeID == null || data.MDepreciationTypeID == undefined) ? 0 : data.MDepreciationTypeID);
            }
        });
        //折旧开始期间
        $(this.depreciateFromInput).validatebox({ required: true });
        //科目的数据
        this.accountData = this.home.getAccountList();
        //核算维度赋值
        this.checkGroupValueModel = $.extend(true, {}, data.MCheckGroupValueModel);
        //加载固定资产科目 与 累计折旧科目
        $(this.fixAssetAccountInput + "," + this.depreciationAccountInput).mAddCombobox("account", {
            width: 200,
            height: 26,
            data: this.accountData,
            //required: true,
            readonly: this.readonly,
            valueField: 'MCode',
            hasDownArrow: true,
            formatter: function (row) {
                return mText.encode(row.MFullName);
            },
            onSelect: function (row) {
                _this.fixAssetsAccount = row;
                //获取用户选择的科目信息
                var checkGroup = _this.getCheckGroupModel();
                //每一次选择的时候都需要根据用户选择的内容，对核算维度进行过滤显示
                _this.home.showCheckGroupValueTips(_this.checkGroupValueModel, $(_this.checkTypeHref), null, checkGroup);
            },
            onLoadSuccess: function () {
                $("body").unmask();
                $(_this.fixAssetAccountInput).combobox("setValue", data.MFixAccountCode);
            }
        }, {
            hasPermission: 1,
            //关闭后需要重新加载摘要
            callback: function () {
                //重新获取摘要
                _this.glHome.getAccountData(function (d) {
                    //重新加载
                    _this.reloadAccountList(d);
                }, null, true);
            }
        });
        //加载固定资产科目 与 累计折旧科目
        $(this.depreciationAccountInput).mAddCombobox("account", {
            width: 200,
            height: 26,
            data: this.accountData,
            valueField: 'MCode',
            hasDownArrow: true,
            readonly: this.readonly,
            formatter: function (row) {
                return mText.encode(row.MFullName);
            },
            //required: true,
            onSelect: function (row) {
                _this.depreciationAccount = row;
                //获取用户选择的科目信息
                var checkGroup = _this.getCheckGroupModel();
                //每一次选择的时候都需要根据用户选择的内容，对核算维度进行过滤显示
                _this.home.showCheckGroupValueTips(_this.checkGroupValueModel, $(_this.checkTypeHref), null, checkGroup);
            },
            onLoadSuccess: function () {
                $("body").unmask();
                $(_this.depreciationAccountInput).combobox("setValue", data.MDepAccountCode);
            }
        }, {
            hasPermission: 1,
            //关闭后需要重新加载摘要
            callback: function () {
                //重新获取摘要
                _this.glHome.getAccountData(function (d) {
                    //重新加载
                    _this.reloadAccountList(d);
                }, null, true);
            }
        });
        //加载固定资产科目 与 累计折旧科目
        $(this.expenseAccountInput).mAddCombobox("account", {
            width: 200,
            height: 26,
            data: this.accountData,
            valueField: 'MCode',
            hasDownArrow: true,
            //required: true,
            readonly: this.readonly,
            formatter: function (row) {
                return mText.encode(row.MFullName);
            },
            onSelect: function (row) {
                _this.depreciationAccount = row;
                //获取用户选择的科目信息
                var checkGroup = _this.getCheckGroupModel();
                //每一次选择的时候都需要根据用户选择的内容，对核算维度进行过滤显示
                _this.home.showCheckGroupValueTips(_this.checkGroupValueModel, $(_this.checkTypeHref), null, checkGroup);
            },
            onLoadSuccess: function () {
                $("body").unmask();
                $(_this.expenseAccountInput).combobox("setValue", data.MExpAccountCode);
            }
        }, {
            hasPermission: 1,
            //关闭后需要重新加载摘要
            callback: function () {
                //重新获取摘要
                _this.glHome.getAccountData(function (d) {
                    //重新加载
                    _this.reloadAccountList(d);
                }, null, true);
            }
        });
        //原值 和 残值率 减值准备 累计折旧 输入的时候，需要动态计算预计残值 净值
        $(this.originalValueInput + "," + this.rateOfSalvageInput + "," + this.prepareDecreaseInput + "," + this.depreciatedValueInput + "," + this.usefulPeriodsInput).off("keyup.fa blur.fa").on("keyup.fa blur.fa", function () {
            //如果是变更的话，不需要算
            if (_this.isChange)
                return;
            var orginalValue = +$(_this.originalValueInput).val().replace(/,/g, ''), rate = +$(_this.rateOfSalvageInput).val(), prepareValue = +$(_this.prepareDecreaseInput).val().replace(/,/g, ''), depreciatedValue = +$(_this.depreciatedValueInput).val().replace(/,/g, ''), deprecatedPeriod = +$(_this.depreciatedPeriodsInput).val(), usefulPeriod = +$(_this.usefulPeriodsInput).val();
            //预计残值 =  原值 * 残值率 
            $(_this.expectedSalvageInput).numberbox('setValue', orginalValue * rate / 100);
            //净值 =  原值 - 减值准备 - 累计折旧
            $(_this.netValueInput).numberbox('setValue', orginalValue - prepareValue - depreciatedValue);
            //月折旧额 = 期初净值*（1-残值率）/（预计使用期数-已折旧期数）
            if (rate >= 0 && rate <= 100 && deprecatedPeriod >= 0 && deprecatedPeriod < usefulPeriod) {
                var v = (orginalValue - prepareValue - depreciatedValue - orginalValue * rate / 100) / (usefulPeriod - deprecatedPeriod);
                $(_this.periodDepreciationInput).numberbox('setValue', v == 0 ? '' : v);
            }
            else if (deprecatedPeriod == usefulPeriod) {
                $(_this.periodDepreciationInput).numberbox('setValue', '');
            }
        });
        //--------------------------------------------------------------------------------------------
        //前缀
        $(this.prefixInput);
        //类别名称
        $(this.nameInput).val(data.MName);
        //类别编号
        $(this.numberInput).numberbox('setValue', data.MNumber);
        //采购日期
        if (data.MPurchaseDate != null && data.MPurchaseDate != undefined && mDate.parse(data.MPurchaseDate).getFullYear() > 1901)
            $(this.purchseDateInput).datebox('setValue', data.MPurchaseDate);
        //清理期间
        if (data.MHandledDate != null && data.MHandledDate != undefined && mDate.parse(data.MHandledDate).getFullYear() > 1901)
            $(this.disposePeriodInput).val(mDate.parse(data.MHandledDate).getFullYear() + "-" + ((mDate.parse(data.MHandledDate).getMonth() + 1) < 10 ? ('0' + (mDate.parse(data.MHandledDate).getMonth() + 1)) : (mDate.parse(data.MHandledDate).getMonth() + 1)));
        //数量
        $(this.quantityInput).numberbox('setValue', data.MQuantity);
        //开始折旧期数
        $(this.depreciateFromInput).val(this.home.formateDate(data.MDepreciationFromPeriod));
        //预计使用期数
        data.MUsefulPeriods && $(this.usefulPeriodsInput).numberspinner('setValue', data.MUsefulPeriods);
        //前缀
        $(this.prefixInput).val(data.MPrefix);
        //原值
        $(this.originalValueInput).numberbox('setValue', data.MOriginalAmount);
        //累计折旧
        $(this.depreciatedValueInput).numberbox('setValue', data.MDepreciatedAmount);
        //备注
        $(this.remarkInput).val(data.MRemark);
        //残值率
        $(this.rateOfSalvageInput).numberbox('setValue', data.MRateOfSalvage);
        //预计残值
        $(this.expectedSalvageInput).numberbox('setValue', data.MSalvageAmount);
        //减值准备
        $(this.prepareDecreaseInput).numberbox('setValue', data.MPrepareForDecreaseAmount);
        //净值
        $(this.netValueInput).numberbox('setValue', data.MNetAmount);
        //核算维度
        this.checkGroupValueModel = $.extend(true, {}, data.MCheckGroupValueModel);
        //如果是正常状态，再来考虑保存和变更
        if (data.MStatus == 0) {
            //如果上期计提折旧大于2000年，则表示已经正常计提了
            if (data.MLastDepreciatedYear > 2000) {
                $(this.changeButtonDiv).show();
                $(this.changeDiv).show();
                $(this.newButtonDiv).remove();
                $(this.originalValueInput).width(100);
                $(this.changeFromInput).val('');
                this.isChange = true;
            }
            else {
                $(this.newButtonDiv).show();
                $(this.changeButtonDiv).remove();
                $(this.changeDiv).hide();
                this.isChange = false;
            }
        }
        else {
            //显示处置和撤销处置
            $(this.handleDiv).show().siblings().remove();
            $(this.changeDiv).hide();
        }
        //已折旧期数修改的时候，也需要重新算月折旧额
        $(this.depreciatedPeriodsInput).numberspinner({
            min: 0,
            disabled: this.isChange,
            max: this.isChange ? 10000 : this.caculateDatePeriodDiff($(this.purchseDateInput).datebox('getValue'), $(this.depreciateFromInput).val() + "-01", this.getDepreciateFromCurrentPeriod() ? 0 : 1),
            onChange: function (val) {
                $(_this.rateOfSalvageInput).trigger("keyup.fa");
            }
        });
        //已折旧期间
        $(this.depreciatedPeriodsInput).numberbox('setValue', data.MDepreciatedPeriods);
        //月折旧额
        $(this.periodDepreciationInput).numberbox('setValue', (data.MPeriodDepreciatedAmount == 0 && !data.MItemID) ? '' : data.MPeriodDepreciatedAmount);
        //如果是变更类型 资产类别 折旧金额 累计折旧 已折旧期数不能修改
        if (this.isChange) {
            //资产类别
            $(this.typeInput).combobox("disable");
            //折旧开始方式
            $(this.depreciateFromInput).attr("disabled", "disabled");
            //已折旧期数
            $(this.depreciatedPeriodsInput).attr("disabled", "disabled");
            //累计折旧
            $(this.depreciatedValueInput).attr("disabled", "disabled");
            //月折旧额
            $(this.periodDepreciationInput).attr("disabled", "disabled");
        }
        //隐藏设置为折旧费用科目
        if (this.readonly) {
            //
            $(this.setDefaultAccountInput).hide().siblings().hide();
            //设置核算维度取消点击事件
            $(this.checkTypeHref).off("click").on("click", function () { });
            //所有的可见input都disable
            $("input:visible,textarea:visible").attr("disabled", "disabled");
        }
    };
    /**
     * 设置是否从当期开始折旧
     * @param current
     */
    FAFixAssetsEdit.prototype.setDepreciateFromCurrentPeriod = function (current) {
        $(this.depreciateFromInput).attr("fromCurrent", current ? "1" : "0");
    };
    /**
     * 获取是否当期采购当期折旧
     */
    FAFixAssetsEdit.prototype.getDepreciateFromCurrentPeriod = function () {
        return $(this.depreciateFromInput).attr("fromCurrent") == "1";
    };
    /**
     * 计算两个日期之间的差
     * @param from
     * @param to
     */
    FAFixAssetsEdit.prototype.caculateDatePeriodDiff = function (from, to, ajust) {
        if (ajust === void 0) { ajust = 0; }
        var f = mDate.parse(from);
        var t = mDate.parse(to);
        var d = Math.abs(f.getFullYear() * 12 + f.getMonth() - t.getFullYear() * 12 - t.getMonth()) + ajust;
        return d;
    };
    /**
     * 计算已折旧期间
     * @param purchaseDate
     * @param typeID
     */
    FAFixAssetsEdit.prototype.calculateDepreciatedPeriods = function (purchaseDate, depreciateDate) {
        var _this = this;
        //如果是变更的话，不需要算
        if (this.isChange) {
            //如果是修改采购日期，这需要调整已折旧期数)
            if (purchaseDate != null && purchaseDate != undefined) {
                var i, j;
                //找出最原始的日期
                var srcPurchasePeriod = (i = mDate.parse(this.srcMode.MPurchaseDate)).getFullYear() * 12 + i.getMonth() + 1;
                var purchasePeriod = (j = mDate.parse(purchaseDate)).getFullYear() * 12 + j.getMonth() + 1;
                var differ = srcPurchasePeriod - purchasePeriod;
                if (differ === 0)
                    return;
                var v = this.srcMode.MDepreciatedPeriods + differ;
                $(this.depreciatedPeriodsInput).numberspinner({
                    min: 0,
                    max: this.isChange ? 1000000 : v,
                    onChange: function (val) {
                        $(_this.rateOfSalvageInput).trigger("keyup.fa");
                    }
                });
                //期间调增或者调减
                $(this.depreciatedPeriodsInput).numberbox('setValue', v);
                return;
            }
        }
        ;
        purchaseDate = (purchaseDate == undefined || purchaseDate == null) ? $(this.purchseDateInput).datebox('getValue') : purchaseDate;
        depreciateDate = (depreciateDate == undefined || depreciateDate == null) ? ($(this.depreciateFromInput).val() + "-01") : depreciateDate;
        //只有在采购日期和折旧开发方式都有值的时候才会去计算已折旧期间
        if (purchaseDate != null && purchaseDate != undefined && depreciateDate != null && depreciateDate != undefined) {
            //如果折旧日期大于采购日期，则修改折旧日期为采购日期 +-1
            var p = mDate.parse(purchaseDate), d = mDate.parse(depreciateDate);
            d = d.getTime() < p.getTime() ? p : d;
            var v = !d ? 0 : ((d.getFullYear() * 12 + d.getMonth()) - (p.getFullYear() * 12 + p.getMonth())) - (this.getDepreciateFromCurrentPeriod() ? 0 : 1);
            $(this.depreciatedPeriodsInput).numberspinner({
                min: 0,
                max: this.isChange ? 1000000 : (v + (this.getDepreciateFromCurrentPeriod() ? 0 : 1)),
                onChange: function (val) {
                    $(_this.rateOfSalvageInput).trigger("keyup.fa");
                }
            });
            $(this.depreciatedPeriodsInput).numberbox('setValue', v);
        }
    };
    /**
     * 修改日期的时候计算开始折旧期间
     */
    FAFixAssetsEdit.prototype.calculaDepreciateFromPeriod = function () {
        //如果是变更的话，不需要算
        if (this.isChange)
            return;
        var purchaseDate = $(this.purchseDateInput).datebox('getValue');
        if (mDate.parse(purchaseDate)) {
            var date = mDate.parse(this.home.formateDate(purchaseDate, this.getDepreciateFromCurrentPeriod() ? 0 : 1) + "-01");
            date = date.getTime() < this.minDepreciateDate.getTime() ? this.minDepreciateDate : date;
            //开始折旧日期
            return $(this.depreciateFromInput).val(this.home.formateDate(date));
        }
        $(this.depreciateFromInput).val('');
    };
    /**
     * 绑定数据到元素上面
     * @param data
     */
    FAFixAssetsEdit.prototype.bindFixAssetDataByType = function (rec) {
        var _this = this;
        //选择的时候动态去获取一下
        this.home.getFixAssetTypeModel(function (data) {
            //折旧方法
            $(_this.depreciateTypeInput).combobox('setValue', data.MDepreciationTypeID);
            //是否当期采购当期折旧
            _this.setDepreciateFromCurrentPeriod(data.MDepreciationFromCurrentPeriod);
            //计算开始折旧期间
            _this.calculaDepreciateFromPeriod();
            //预计使用期限
            $(_this.usefulPeriodsInput).numberspinner('setValue', data.MUsefulPeriods);
            //固定资产科目
            $(_this.fixAssetAccountInput).combobox('setValue', data.MFixAccountCode);
            //累计折旧科目
            $(_this.depreciationAccountInput).combobox('setValue', data.MDepAccountCode);
            //费用科目
            $(_this.expenseAccountInput).combobox('setValue', data.MExpAccountCode);
            //残值率
            $(_this.rateOfSalvageInput).numberbox('setValue', data.MRateOfSalvage);
            //核算维度
            _this.checkGroupValueModel = data.MCheckGroupValueModel;
            //触发残值率的keyup事件
            $(_this.rateOfSalvageInput).trigger("keyup.fa");
            //核算维度超链接鼠标放置显示
            _this.home.showCheckGroupValueTips(_this.checkGroupValueModel, $(_this.checkTypeHref), null, {});
            //计算已折旧期数 如果变更，则不需要重新计算已折旧期数
            !_this.isChange && _this.calculateDepreciatedPeriods(null, null);
        }, rec.MItemID);
    };
    FAFixAssetsEdit.prototype.initEventAfterLoadData = function () {
        var _this = this;
        $(".m-imain").off("scroll").on("scroll", function (event) {
            event.stopPropagation();
        });
        //有时候用户是输入数字
        $(".datebox-input:visible").off("keyup.fa blur.fa").on("keyup.fa blur.fa", function (event) {
            _this.calculaDepreciateFromPeriod();
            //如果日期面板是显示的，就不做下面的操作了
            if ($(_this.purchseDateInput).datebox("panel").is(":visible"))
                return;
            var date = $(_this.purchseDateInput).datebox('textbox').val();
            !!mDate.parse(date) && _this.calculateDepreciatedPeriods(mDate.format(date), null);
        });
    };
    /**
     * 初始化页面的事件
     */
    FAFixAssetsEdit.prototype.initEvent = function () {
        var _this = this;
        //处置
        $(this.handleTopButton + "," + this.sellButton + "," + this.disposeButton).off("click").on("click", function (event) {
            var type = $(event.target || event.srcElement).attr("newstatus") == undefined ? $(event.target || event.srcElement).parents("[newstatus]").attr("newstatus") : $(event.target || event.srcElement).attr("newstatus");
            _this.home.handleFixAssets([$(_this.itemIDInput).val()], +type, function () {
                mWindow.reload();
            });
        });
        //撤销处置
        $(this.unhandleButton).off("click").on("click", function (event) {
            _this.home.handleFixAssets([$(_this.itemIDInput).val()], parseInt($(_this.unhandleButton).attr("newstatus")), function () {
                mWindow.reload();
            });
        });
        //保存资产
        $(this.saveButton + "," + this.saveAndCopyButton + "," + this.saveAndNewButton + "," + this.changeButton).off("click").on("click", function (event) {
            var type = $(event.target || event.srcElement).attr("type") == undefined ? $(event.target || event.srcElement).parents("[type]").attr("type") : $(event.target || event.srcElement).attr("type");
            //调用统一的入口方法
            return _this.saveFixAssets(+type);
        });
        //核算维度超链接鼠标放置显示
        this.home.showCheckGroupValueTips(this.checkGroupValueModel, $(this.checkTypeHref), null, {});
        //设置核算维度
        $(this.checkTypeHref).off("click").on("click", function () {
            $("body").mask("");
            //先弹出框框，然后设置显示的内容
            //获取用户选择的科目信息
            var checkGroup = _this.getCheckGroupModel();
            _this.home.showCheckGroupSetupDiv(checkGroup, _this.checkGroupValueModel, _this.divCheckGroupSetup, null, function (d) {
                _this.checkGroupValueModel = d;
                _this.home.showCheckGroupValueTips(_this.checkGroupValueModel, $(_this.checkTypeHref), null, _this.getCheckGroupModel());
            });
        });
        //设置我默认的核算维度
        $(this.setDefaultAccountInput).off("click").on("click", function () {
            var accountCode = $(_this.expenseAccountInput).combobox('getValue');
            if (accountCode == null || accountCode == undefined || accountCode.length == 0) {
                return mDialog.alert(HtmlLang.Write(LangModule.FA, "PleaseChooesOneExpenseDepreciationAccount", "请选择一个折旧费用科目"));
            }
            _this.home.setDefaultExpenseAccount(true, accountCode, function (data) {
                var typeList = $(_this.typeInput).combobox('getData');
                var selectedValue = $(_this.typeInput).combobox('getValue');
                if (typeList != null && typeList.length > 0) {
                    typeList.forEach(function (value, index, array) {
                        value.MExpAccountCode = accountCode;
                    });
                    $(_this.typeInput).combobox('loadData', typeList);
                    $(_this.typeInput).combobox('setValue', selectedValue);
                }
            });
        });
        //日志
        $(this.divHistory).off("click").on("click", function () {
            _this.home.showLogDialog(_this.fixAssetsID, null);
        });
    };
    /**
     * 保存资产类别
     */
    FAFixAssetsEdit.prototype.saveFixAssets = function (type) {
        //先做页面校验
        if (!$('body').mValidateEasyUI())
            return;
        var model = this.getAssetsModel();
        //数值准确性校验
        if (!this.validateFixAssetsModel(model))
            return;
        //是否有做过修改
        var equal = this.equal(this.srcMode, model);
        //判断是否有做过修改
        if (model.MIsChange && equal)
            return mDialog.alert(HtmlLang.Write(LangModule.FA, "FixAssetsHasNoChanged", "资产卡片没有做任何修改，无需变更!"));
        //如果有修改，则需要后台保存修改信息
        model.MIsChange = !equal;
        //保存
        this.home.saveFixAssets(model, type, this.isChange);
    };
    /**
     * 校验固定资产卡片是否合法
     * @param model
     */
    FAFixAssetsEdit.prototype.validateFixAssetsModel = function (model) {
        //错误信息集合
        var message = [];
        //2.减值准备不可大于 期初-期初累计折旧
        if (model.MPrepareForDecreaseAmount > model.MOriginalAmount - model.MDepreciatedAmount) {
            message.push(HtmlLang.Write(LangModule.FA, "PrepareDecreaseCannotLargerThanCurrentValue", '减值准备不可大于原值与累计折旧的差额!'));
        }
        //3.已折旧期数不可大于预计使用期数
        if (model.MDepreciatedPeriods > model.MUsefulPeriods) {
            message.push(HtmlLang.Write(LangModule.FA, "DepreciatedPeriodCannotLargerThanUsefulPeriods", '已折旧期数不可大于预计使用期数!'));
        }
        //4.月折旧额 不可大于
        if (model.MPeriodDepreciatedAmount > model.MOriginalAmount) {
            message.push(HtmlLang.Write(LangModule.FA, "PeriodDepreciatedAmontCannotLargerThanOriginalAmount", '余额折旧额不可大于原值!'));
        }
        var i, j;
        //5.折旧开始期间，不可小于采购期间
        if (((i = mDate.parse(model.MDepreciationFromPeriod)).getFullYear() * 12 + i.getMonth()) <
            (j = mDate.parse(model.MPurchaseDate)).getFullYear() * 12 + j.getMonth()) {
            message.push(HtmlLang.Write(LangModule.FA, "DepreciateFromPeriodCannotEarlierThanPurchasePeriod", '折旧开始期间不可早于采购期间!'));
        }
        //6.如果是变更话，必须填入折旧影响期间
        if (this.isChange && this.home.GetChangeType(model, this.srcMode) >= FixAssetsChangeTypeEnum.Strategy && mDate.parse(model.MChangeFromPeriod).getFullYear() <= 1901) {
            message.push(HtmlLang.Write(LangModule.FA, "ChangeApplyFromPeriodCannotBeNull", '变更开始影响期间不可为空!'));
        }
        //7.如果是变更的话，减值准备不可调小，只允许调大
        if (this.isChange && +this.srcMode.MPrepareForDecreaseAmount > +model.MPrepareForDecreaseAmount) {
            message.push(HtmlLang.Write(LangModule.FA, "PrepareForDecreaseCannotChangeToLess", '已折旧资产的减值准备不可调减!'));
        }
        //如果有提示信息，则表示失败了
        if (message.length > 0) {
            var messageStr_1 = '';
            message.forEach(function (value, index, array) {
                messageStr_1 += (index + 1) + "." + value + "<br>";
            });
            mDialog.alert('<div>' + messageStr_1 + '</div>');
            return false;
        }
        return true;
    };
    /**
     * 比较两个实体是否相等
     * @param src
     * @param dest
     */
    FAFixAssetsEdit.prototype.equal = function (src, dest) {
        //把核算维度值的id取消掉
        if (src.MCheckGroupValueModel != null && src.MCheckGroupValueModel != undefined)
            src.MCheckGroupValueModel.MItemID = null;
        var i, j;
        //返回比较接口
        var isEqual = src.MNumber == dest.MNumber
            && src.MPrefix == dest.MPrefix
            && src.MName == dest.MName
            && src.MQuantity == dest.MQuantity
            && ((src.MRemark == null || src.MRemark == undefined) ? '' : src.MRemark) == dest.MRemark
            && (src.MFixAccountCode || "") == dest.MFixAccountCode
            && (src.MDepAccountCode || "") == dest.MDepAccountCode
            && (src.MExpAccountCode || "") == dest.MExpAccountCode
            && _.isEqual(src.MCheckGroupValueModel, dest.MCheckGroupValueModel)
            && mDate.parse(src.MDepreciationFromPeriod).getTime() == dest.MDepreciationFromPeriod.getTime()
            && src.MOriginalAmount == dest.MOriginalAmount
            && src.MPeriodDepreciatedAmount == dest.MPeriodDepreciatedAmount
            && src.MRateOfSalvage == dest.MRateOfSalvage
            && src.MSalvageAmount == dest.MSalvageAmount
            && src.MPrepareForDecreaseAmount == dest.MPrepareForDecreaseAmount
            && src.MNetAmount == dest.MNetAmount
            && src.MDepreciatedAmount == dest.MDepreciatedAmount
            && (((i = mDate.parse(src.MPurchaseDate)).getFullYear() == (j = mDate.parse(dest.MPurchaseDate)).getFullYear()) && (i.getMonth() == j.getMonth()) && (i.getDate() == j.getDate()))
            && src.MUsefulPeriods == dest.MUsefulPeriods
            && src.MDepreciatedPeriods == dest.MDepreciatedPeriods
            && _.isEqual(src.MultiLanguage, dest.MultiLanguage);
        return isEqual;
    };
    /**
     * 获取当前用户的资产类表实体
     */
    FAFixAssetsEdit.prototype.getAssetsModel = function () {
        var langData = $(this.nameInput).getLangEditorData();
        var model = {
            MItemID: $(this.itemIDInput).val(),
            MNumber: $(this.numberInput).val(),
            MPrefix: $(this.prefixInput).val(),
            MName: $(this.nameInput).val(),
            MultiLanguage: [langData],
            MPurchaseDate: mDate.parse($(this.purchseDateInput).datebox('getValue')),
            MFATypeID: $(this.typeInput).combobox('getValue'),
            MQuantity: +$(this.quantityInput).numberbox('getValue'),
            MDepreciationTypeID: $(this.depreciateTypeInput).combobox('getValue'),
            MUsefulPeriods: +$(this.usefulPeriodsInput).numberspinner("getValue"),
            MDepreciationFromPeriod: mDate.parse($(this.depreciateFromInput).val() + "-01"),
            MDepAccountCode: $(this.depreciationAccountInput).combobox("getValue"),
            MFixAccountCode: $(this.fixAssetAccountInput).combobox("getValue"),
            MExpAccountCode: $(this.expenseAccountInput).combobox('getValue'),
            MOriginalAmount: +$(this.originalValueInput).numberbox('getValue'),
            MPeriodDepreciatedAmount: +$(this.periodDepreciationInput).numberbox('getValue'),
            MRateOfSalvage: +$(this.rateOfSalvageInput).numberbox("getValue"),
            MRemark: $(this.remarkInput).val(),
            MPrepareForDecreaseAmount: +$(this.prepareDecreaseInput).numberbox('getValue'),
            MDepreciatedAmount: +$(this.depreciatedValueInput).numberbox('getValue'),
            MDepreciatedPeriods: +$(this.depreciatedPeriodsInput).numberbox('getValue'),
            MCheckGroupValueModel: this.checkGroupValueModel,
            MChangeFromPeriod: mDate.parse($(this.changeFromInput).val() + "-01"),
            MBackAdjust: $(this.backAdjustInput).is(":checked"),
            MIsChange: this.isChange
        };
        //如果是变更的话，需要讲一些参数物归原主
        if (this.isChange) {
            //model.MDepreciatedPeriods = this.srcMode.MSrcModel.MDepreciatedPeriods;
            model.MPeriodDepreciatedAmount = this.srcMode.MSrcModel.MPeriodDepreciatedAmount;
            model.MNetAmount = this.srcMode.MSrcModel.MNetAmount;
            model.MDepreciatedAmount = this.srcMode.MSrcModel.MDepreciatedAmount;
        }
        else {
            //净值 = 原值 - 减值准备 - 累计折旧
            model.MNetAmount = model.MOriginalAmount - model.MPrepareForDecreaseAmount - model.MDepreciatedAmount;
            //预计残值 = 原值 * 残值率 / 100;
            model.MSalvageAmount = +((model.MOriginalAmount * model.MRateOfSalvage / 100.00).toFixed(2));
            model.MChangeFromPeriod = null;
        }
        return model;
    };
    /**
     * 获取用户设置好的核算维度值
     */
    FAFixAssetsEdit.prototype.getCheckGroupModel = function () {
        //固定资产科目
        var fixAssetsAccount = this.home.getSelectedAccountModel($(this.fixAssetAccountInput), this.fixAssetsAccount);
        //累计折旧科目
        var depreciationAccount = this.home.getSelectedAccountModel($(this.depreciationAccountInput), this.depreciationAccount);
        //折旧费用科目
        var expenseAccount = this.home.getSelectedAccountModel($(this.expenseAccountInput), this.expenseAccount);
        //如果一个科目没选
        if (fixAssetsAccount == null && depreciationAccount == null && expenseAccount == null) {
            return null;
        }
        //返回合并结果
        return this.home.mergeAccountCheckGroup(fixAssetsAccount, depreciationAccount, expenseAccount);
    };
    return FAFixAssetsEdit;
}());
//# sourceMappingURL=FAFixAssetsEdit.js.map