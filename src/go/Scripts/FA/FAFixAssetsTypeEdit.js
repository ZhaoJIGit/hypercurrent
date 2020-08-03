/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
/**
 * 固定资产类别边界
 */
var FAFixAssetsTypeEdit = /** @class */ (function () {
    function FAFixAssetsTypeEdit() {
        //资产类别ID
        this.itemIDInput = "#itemID";
        //资产编号
        this.numberInput = '.tp-type-number';
        //资产名称
        this.nameInput = '.tp-type-name';
        //固定资产科目
        this.fixAssetAccountInput = '.tp-type-fixassetsaccount';
        //折旧科目
        this.depreciationAccountInput = '.tp-type-depreciationaccount';
        //折旧类型
        this.depreciateType = '.tp-type-depreciationtype';
        //核算维度编辑超链接
        this.checkTypeHref = '.tp-edit-checktype,.tp-edit-icon';
        //使用年限
        this.usefulYearInput = '.tp-type-year';
        //使用期
        this.usefulPeriodInput = '.tp-type-period';
        //备注
        this.remarkInput = '.tp-type-remark';
        //残值率
        this.rateOfSalvageInput = '.tp-type-rateofsalvage';
        //开始折旧方式
        this.depreciateFromInput = '.tp-type-depreciatefrom';
        //保存
        this.saveButton = '.tp-save-button';
        //保存页面数据
        this.sourceData = {};
        //核算维度设置
        this.divCheckGroupSetup = "#divCheckGroupSetup";
        //固定资产科目
        this.fixAssetsAccount = {};
        //累计折旧科目
        this.depreciationAccount = {};
        //所设置的核算维度值
        this.checkGroupValueModel = {};
        //当前页面就用一个科目列表数据
        this.accountData = [];
        this.glHome = new GLVoucherHome();
        this.home = new FAHome();
    }
    //初始化函数
    FAFixAssetsTypeEdit.prototype.init = function () {
        var _this = this;
        this.fixAssetsTypeID = $(this.itemIDInput).val();
        //加载数据
        this.home.getFixAssetTypeModel(function (data) {
            $("body").mask("");
            _this.loadData(data);
            //初始化页面事件
            _this.initEvent();
        }, this.fixAssetsTypeID);
    };
    //重新加载科目列表
    FAFixAssetsTypeEdit.prototype.reloadAccountList = function (d) {
        this.accountData = d;
        //重新加载
        $(this.fixAssetAccountInput).combobox("loadData", d);
        //重新加载
        $(this.depreciationAccountInput).combobox("loadData", d);
    };
    //加载一条数据
    FAFixAssetsTypeEdit.prototype.loadData = function (data) {
        var _this = this;
        this.sourceData = data;
        //资产类型编号
        $(this.numberInput).validatebox({ required: true, validType: ['length[1,50]'] });
        //名称是必填的
        $(this.nameInput).validatebox({ required: true, validType: ['length[1,200]'] });
        //资产名称
        $(this.nameInput).initLangEditor((data.MultiLanguage == null || data.MultiLanguage == undefined) ? null : data.MultiLanguage[0]);
        //备注最大500个字符
        $(this.remarkInput).validatebox({ validType: ['length[0,500]'] });
        //残值率最大是100
        $(this.rateOfSalvageInput).numberbox({ min: 0, max: 100, precision: 2 });
        //科目的数据
        this.accountData = this.home.getAccountList();
        //核算维度赋值
        this.checkGroupValueModel = data.MCheckGroupValueModel;
        //加载固定资产科目 与 累计折旧科目
        $(this.fixAssetAccountInput + "," + this.depreciationAccountInput).mAddCombobox("account", {
            width: 200,
            height: 26,
            data: this.accountData,
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
                $(_this.depreciationAccountInput).combobox("setValue", data.MDepAccountCode);
            }
        }, {
            hasPermission: 1,
            //关闭后需要重新加载摘要
            callback: function () {
                //重新获取摘要
                _this.glHome.getAccountData(function (d) {
                    _this.reloadAccountList(d);
                }, null, true);
            }
        });
        //折旧方法
        $(this.depreciateType).combobox({
            width: 200,
            height: 26,
            data: this.home.getDepreciationTypeArray(),
            textField: 'name',
            valueField: 'value',
            readonly: true,
            required: true,
            disabled: true,
            onLoadSuccess: function () {
                $(_this.depreciateType).combobox("setValue", (data.MDepreciationTypeID == null || data.MDepreciationTypeID == undefined) ? 0 : data.MDepreciationTypeID);
            }
        });
        //初始化年输入
        $(this.usefulYearInput).numberbox({
            width: 200,
            height: 26,
            min: 0,
            max: 9999,
            precision: 2,
            onChange: function (newValue, oldValue) {
                $(_this.usefulPeriodInput).numberbox("setValue", newValue * 12);
            }
        });
        //初始化期输入
        $(this.usefulPeriodInput).numberbox({
            width: 200,
            height: 26,
            min: 1,
            max: 999999,
            required: true,
            precision: 0,
        });
        //残值率
        $(this.rateOfSalvageInput).numberbox({
            width: 200,
            height: 26,
            min: 0,
            max: 100,
            required: true,
            precision: 2,
        });
        //折旧方式
        $(this.depreciateFromInput).combobox({
            width: 200,
            height: 26,
            data: this.home.getDepreciationFromArray(),
            textField: 'name',
            valueField: 'value',
            required: true,
            onLoadSuccess: function () {
                $(_this.depreciateFromInput).combobox("setValue", data.MDepreciationFromCurrentPeriod ? 1 : 0);
            }
        });
        //类别编号
        $(this.numberInput).val(data.MNumber);
        //类别名称
        $(this.nameInput).val(data.MName);
        //使用年限
        !!data.MUsefulPeriods && $(this.usefulYearInput).numberbox("setValue", data.MUsefulPeriods == undefined ? 0 : data.MUsefulPeriods / 12);
        //使用期
        !!data.MUsefulPeriods && $(this.usefulPeriodInput).numberbox("setValue", data.MUsefulPeriods == undefined ? 0 : data.MUsefulPeriods);
        //备注
        $(this.remarkInput).val(data.MRemark);
        //残值率
        $(this.rateOfSalvageInput).numberbox('setValue', data.MRateOfSalvage);
        //核算维度
        this.checkGroupValueModel = data.MCheckGroupValueModel;
    };
    /**
     * 初始化页面的事件
     */
    FAFixAssetsTypeEdit.prototype.initEvent = function () {
        var _this = this;
        //年限输入的时候，需要期跟着改变
        $(this.usefulYearInput).off("keyup").on("keyup", function (event) {
            $(_this.usefulPeriodInput).numberbox("setValue", $(_this.usefulYearInput).val() * 12);
        });
        //保存资产类别
        $(this.saveButton).off("click").on("click", function () { return _this.saveFixAssetsType(); });
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
    };
    /**
     * 保存资产类别
     */
    FAFixAssetsTypeEdit.prototype.saveFixAssetsType = function () {
        //先做页面校验
        if (!$('body').mValidateEasyUI())
            return;
        var model = this.getAssetsTypeModel();
        this.home.saveFixAssetsType(model, function (data) {
            if (data.Success) {
                mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessfully", "保存成功!"));
                mDialog.close();
            }
            else {
                mDialog.error(data.Message);
            }
        });
    };
    /**
     * 获取当前用户的资产类表实体
     */
    FAFixAssetsTypeEdit.prototype.getAssetsTypeModel = function () {
        var langData = $(this.nameInput).getLangEditorData();
        return {
            MItemID: $(this.itemIDInput).val(),
            MNumber: $(this.numberInput).val(),
            MName: $(this.nameInput).val(),
            MultiLanguage: [langData],
            MFixAccountCode: $(this.fixAssetAccountInput).combobox("getValue"),
            MDepAccountCode: $(this.depreciationAccountInput).combobox("getValue"),
            MDepreciationTypeID: $(this.depreciateType).combobox("getValue"),
            MUsefulPeriods: $(this.usefulPeriodInput).numberbox("getValue"),
            MCheckGroupValueModel: this.checkGroupValueModel,
            MRemark: $(this.remarkInput).val(),
            MRateOfSalvage: $(this.rateOfSalvageInput).numberbox("getValue"),
            MDepreciationFromCurrentPeriod: $(this.depreciateFromInput).combobox("getValue") == "1",
            MIsSys: this.sourceData.MIsSys
        };
    };
    /**
     * 获取用户设置好的核算维度值
     */
    FAFixAssetsTypeEdit.prototype.getCheckGroupModel = function () {
        //固定资产科目
        var fixAssetsAccount = this.home.getSelectedAccountModel($(this.fixAssetAccountInput), this.fixAssetsAccount);
        //累计折旧科目
        var depreciationAccount = this.home.getSelectedAccountModel($(this.depreciationAccountInput), this.depreciationAccount);
        //如果一个科目没选
        if (fixAssetsAccount == null && depreciationAccount == null) {
            return null;
        }
        //返回合并结果
        return this.home.mergeAccountCheckGroup(fixAssetsAccount, depreciationAccount);
    };
    return FAFixAssetsTypeEdit;
}());
//# sourceMappingURL=FAFixAssetsTypeEdit.js.map