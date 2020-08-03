/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var BDVoucherSetting = /** @class */ (function () {
    function BDVoucherSetting() {
        //获取设置
        this.getVoucherSettingUrl = "/BD/BDVoucherSetting/GetVoucherSetting";
        //保存设置
        this.saveVoucherSettingUrl = "/BD/BDVoucherSetting/SaveVoucherSetting";
        this.tabSelectedClass = "main-header-selected";
        this.vsAction = ".vs-action";
        this.advanceSettingButton = ".vs-advance-setting-button";
        this.advanceSetting = "#advanceSetting";
        this.saveButton = ".vs-save-button";
        this.confirmButton = ".vs-confirm-button";
        this.grid = ".vs-setting-table";
        this.gridDiv = ".vs-setting-grid";
        this.settingUl = ".vs-setting-ul.vs-demo";
        this.settingDiv = ".vs-setting-div";
        this.settingItemIDName = "settingitemid";
        this.srcValueName = "srcvalue";
        this.isCheckboxName = "ischeckbox";
        this.modifiedClassName = "vs-modified";
        this.createDiv = ".vs-create-div";
        this.numberDiv = ".vs-number-div";
        this.mainHeader = ".main-header";
        this.mainBottom = ".main-bottom";
        this.numberCount = ".vs-number-count";
        this.fillWithChar = ".vs-number-filledwithchar";
        this.numberExample = ".vs-number-example";
        this.currentIndex = 0;
        //显示的哪些列
        this.settingIdList = [0];
    }
    /**
     * 初始化数据
     */
    BDVoucherSetting.prototype.init = function () {
        this.prefixSetting = new BDPrefixSetting();
        this.initTab();
        this.initEvent();
    };
    /**
     * 初始化tab
     */
    BDVoucherSetting.prototype.initTab = function () {
        var _this = this;
        $(this.mainHeader).tabs({
            selected: this.currentIndex,
            onSelect: function (title, index) {
                _this.showTab(index);
            }
        });
        this.showTab(this.currentIndex);
    };
    /**
     * 显示tab
     * @param index
     */
    BDVoucherSetting.prototype.showTab = function (index) {
        var div = $(this.mainBottom + " > div[index='" + index + "']");
        div.show().siblings().hide();
        var tabDiv = $(this.mainHeader + ">div[index=" + index + "]");
        tabDiv.find("span").addClass(this.tabSelectedClass);
        tabDiv.siblings().find("span").removeClass(this.tabSelectedClass);
        this.currentIndex = +index;
        switch (+index) {
            case 0:
                $(this.vsAction).show();
                this.loadSettingData();
                break;
            case 1:
                $(this.vsAction).hide();
                this.loadPrefixSettingData();
                break;
        }
    };
    /**
     * 加载凭证的前缀设置
     */
    BDVoucherSetting.prototype.loadPrefixSettingData = function () {
        var _this = this;
        this.prefixSetting.loadPrefixSettingData("GeneralLedger", function (model) {
            model.MNumberCount = model.MNumberCount === 0 ? 3 : model.MNumberCount;
            model.MFillBlankChar = !model.MItemID ? '0' : model.MFillBlankChar;
            _this.showPrefixSettingData(model);
        });
    };
    /**
     * 展示数据
     * @param data
     */
    BDVoucherSetting.prototype.showPrefixSettingData = function (model) {
        var _this = this;
        $(this.numberCount).numberspinner({
            min: 3,
            max: 9,
            onChange: function (newValue, oldValue) {
                _this.showVoucherNumberRange(_this.getPrefixSettingModel());
            }
        });
        $(this.fillWithChar).off("click").on("click", function () {
            _this.showVoucherNumberRange(_this.getPrefixSettingModel());
        });
        $(this.numberCount).numberspinner("setValue", model.MNumberCount);
        !!model.MFillBlankChar ? $(this.fillWithChar).attr("checked", "checked") : $(this.fillWithChar).removeAttr("checked");
        this.showVoucherNumberRange(model);
    };
    /**
     * 展示凭证号的最大期间
     * @param model
     */
    BDVoucherSetting.prototype.showVoucherNumberRange = function (model) {
        var max = "9999999999".substr(0, model.MNumberCount);
        var text = "GL-" + this.toVoucherNumber("1", model.MNumberCount, model.MFillBlankChar) + " ~ " + "GL-" + this.toVoucherNumber(max, model.MNumberCount, model.MFillBlankChar);
        $(this.numberExample).text(text);
    };
    /**
     * 将一个编号转化成固定格式的
     * @param number
     * @param count
     * @param fillWithChar
     */
    BDVoucherSetting.prototype.toVoucherNumber = function (number, count, fillWithChar) {
        if (!number || !$.isNumeric(number))
            return "";
        return fillWithChar !== '0' ? number : ("0000000000".substring(0, count - number.length) + number);
    };
    /**
     * 获取用户设置的model
     */
    BDVoucherSetting.prototype.getPrefixSettingModel = function () {
        return {
            MPrefixModule: "GeneralLedger",
            MNumberCount: $(this.numberCount).numberspinner("getValue"),
            MFillBlankChar: $(this.fillWithChar).is(":checked") ? "0" : ""
        };
    };
    /**
     * 保存
     */
    BDVoucherSetting.prototype.savePrefixSettingModel = function () {
        var _this = this;
        var model = this.getPrefixSettingModel();
        if (model.MNumberCount < 3 || model.MNumberCount > 10) {
            mDialog.alert(HtmlLang.Write(LangModule.BD, "VoucherNummberCountRange", "凭证编号位数必须在[3,9]之间"));
            return;
        }
        mDialog.confirm(HtmlLang.Write(LangModule.BD, "VoucherNumberSettingSaveConfirm", "凭证编号规则修改后，系统将会更新所有的凭证编号显示格式，是否确认修改?"), function () {
            _this.prefixSetting.savePrefixSettingData(model, function (result) {
                if (result.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessfully", "保存成功!"));
                    _this.loadPrefixSettingData();
                }
                else {
                    mDialog.message(HtmlLang.Write(LangModule.Common, "SaveFailed", "保存失败!"));
                }
            });
        });
    };
    /**
     * 加载数据
     */
    BDVoucherSetting.prototype.loadSettingData = function () {
        var _this = this;
        mAjax.post(this.getVoucherSettingUrl, {}, function (data) {
            _this.showSettingData(data);
        }, null, true);
    };
    /**
     * 显示设置数据
     */
    BDVoucherSetting.prototype.showSettingData = function (data) {
        var _this = this;
        data = data || [];
        $(this.grid).datagrid({
            data: data,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $(".vs-imain").width() - 25,
            height: ($(".vs-imain").height() - 78),
            columns: this.getSetColumnList(),
            onLoadSuccess: function () {
                _this.initCheckBoxEvent();
                $(_this.grid).datagrid("resize");
            }
        });
    };
    BDVoucherSetting.prototype.getSetColumnList = function () {
        var _this = this;
        var single = !this.isSettingShow(VoucherSettingEnum.ExplanationSet);
        if (single) {
            return [[
                    {
                        field: 'MItemID', title: '', width: 40, align: 'left', hidden: true
                    },
                    {
                        field: 'MModuleID', title: HtmlLang.Write(LangModule.BD, "ModuleName", "模块"), width: 100, align: 'center',
                        formatter: function (value) {
                            return _this.getModuleName(+value).replace(/\s*$|\r|\n/gi, '').replace(/^\s*/g, "");
                        }
                    },
                    {
                        field: 'MEntryMergeSet', title: HtmlLang.Write(LangModule.BD, "EntryMergeSet", "分录合并设置"), width: 300, align: 'center', formatter: function (value, row) {
                            var items = row.MSettingList.where('x.MColumnID == 0');
                            return _this.getSettingItemHtml(items).replace(/\s*$|\r|\n/gi, '').replace(/^\s*/g, "");
                        }
                    },
                ]];
        }
        else {
            return [[
                    {
                        field: 'MItemID', title: '', width: 40, align: 'left', hidden: true, rowspan: 2
                    },
                    {
                        field: 'MModuleID', title: HtmlLang.Write(LangModule.BD, "ModuleName", "模块"), width: 100, align: 'center', rowspan: 2,
                        formatter: function (value) {
                            return _this.getModuleName(+value);
                        }
                    },
                    {
                        field: 'MEntryMergeSet', title: HtmlLang.Write(LangModule.BD, "EntryMergeSet", "分录合并设置"), width: 150, align: 'center', rowspan: 2, formatter: function (value, row) {
                            var items = row.MSettingList.where('x.MColumnID == 0');
                            return _this.getSettingItemHtml(items).replace(/\s*$|\r|\n/gi, '').replace(/^\s*/g, "");
                        }
                    },
                    {
                        title: HtmlLang.Write(LangModule.BD, "ExplanationMergeSet", "摘要设置"), width: 150, align: 'center', colspan: 2, hidden: !this.isSettingShow(VoucherSettingEnum.ExplanationSet)
                    }
                ],
                [
                    {
                        field: 'MDebit', title: HtmlLang.Write(LangModule.BD, "Debit", "借方"), width: 75, align: 'center', hidden: !this.isSettingShow(VoucherSettingEnum.ExplanationSet), formatter: function (value, row) {
                            var items = row.MSettingList.where('x.MColumnID == 1 && x.MDC == 1 ');
                            return _this.getSettingItemHtml(items).replace(/\s*$|\r|\n/gi, '').replace(/^\s*/g, "");
                        }
                    },
                    {
                        field: 'MCredit', title: HtmlLang.Write(LangModule.BD, "Credit", "贷方"), width: 75, align: 'center', hidden: !this.isSettingShow(VoucherSettingEnum.ExplanationSet), formatter: function (value, row) {
                            var items = row.MSettingList.where('x.MColumnID == 1 && x.MDC == -1 ');
                            return _this.getSettingItemHtml(items).replace(/\s*$|\r|\n/gi, '').replace(/^\s*/g, "");
                        }
                    }
                ]];
        }
    };
    /**
     * 获取模块的名字
     * @param id
     */
    BDVoucherSetting.prototype.getModuleName = function (id) {
        switch (id) {
            case 0:
                return HtmlLang.Write(LangModule.BD, "SalesModule", "销售");
            case 1:
                return HtmlLang.Write(LangModule.BD, "PurchaseModule", "采购");
            case 2:
                return HtmlLang.Write(LangModule.BD, "ExpenseModule", "费用报销");
            case 4:
                return HtmlLang.Write(LangModule.FP, "SaleFapiao", "销项发票");
            case 5:
                return HtmlLang.Write(LangModule.FP, "PurchaseFapiaoHome", "进项发票");
            case 6:
                return HtmlLang.Write(LangModule.Common, "Receive_Other", "收款");
            case 7:
                return HtmlLang.Write(LangModule.Common, "Pay_Other", "付款");
            case 8:
                return HtmlLang.Write(LangModule.Common, "Transfer", "转账");
            default:
                return '';
        }
    };
    /**
     * 显示具体项
     * @param item
     */
    BDVoucherSetting.prototype.getSettingItemHtml = function (items) {
        var ul = $(this.settingUl).clone();
        ul.removeClass("vs-demo");
        var demoli = $("li.vs-demo", ul);
        var lastLi = demoli;
        for (var i = 0; i < items.length; i++) {
            var li = demoli.clone();
            li.removeClass("vs-demo");
            li.insertAfter(lastLi);
            //保存ID以及原始的值
            li.attr(this.settingItemIDName, items[i].MItemID)
                .attr(this.srcValueName, items[i].MStatus ? 1 : 0)
                .attr(this.isCheckboxName, items[i].MIsCheckBox ? 1 : 0);
            var span = $("span", li);
            var checkbox = $("input[type='checkbox']", li);
            items[i].MStatus ? checkbox.attr("checked", "checked") : checkbox.removeAttr("checked");
            span.text(mText.encode(this.getTypeNameByID(items[i].MTypeID)));
            lastLi = li;
        }
        return ul.prop("outerHTML");
    };
    /**
     * 获取设置的类型
     */
    BDVoucherSetting.prototype.getTypeNameByID = function (id) {
        switch (id) {
            case 0:
                return HtmlLang.Write(LangModule.BD, 'EntryMergeSet', '分录合并设置');
            case 1:
                return HtmlLang.Write(LangModule.BD, 'ExplanationSet', '摘要设置');
            case 2:
                return HtmlLang.Write(LangModule.BD, 'AccountCheckTypeSame', '科目与核算维度相同');
            case 3:
                return HtmlLang.Write(LangModule.BD, 'AccountDescCheckTypeSame', '科目、摘要与核算维度相同');
            case 4:
                return HtmlLang.Write(LangModule.BD, 'DocType', '单据类型');
            case 5:
                return HtmlLang.Write(LangModule.BD, 'DocNumber', '单据号');
            case 6:
                return HtmlLang.Write(LangModule.BD, 'Contact', '联系人');
            case 7:
                return HtmlLang.Write(LangModule.IV, 'Ref', '备注');
            case 8:
                return HtmlLang.Write(LangModule.BD, 'MerItemNumber', '商品编号');
            case 9:
                return HtmlLang.Write(LangModule.IV, 'Description', '描述');
            case 10:
                return HtmlLang.Write(LangModule.BD, 'ExpenseItem', '费用项目');
            case 11:
                return HtmlLang.Write(LangModule.BD, 'Employee', '员工');
            case 12:
                return HtmlLang.Write(LangModule.BD, 'ExpenseItemMerItemNumber', '费用项目/商品编号');
            case 13:
                return HtmlLang.Write(LangModule.BD, 'ContactEmployee', '联系人/员工');
            case 14:
                return HtmlLang.Write(LangModule.FP, 'FapiaoType', '发票类型');
            case 15:
                return HtmlLang.Write(LangModule.FP, 'FapiaoNumber', '发票编号');
            case 16:
                return HtmlLang.Write(LangModule.FP, 'Explanation', '摘要');
            default:
                return '';
        }
    };
    /**
     * 对于有互斥类型的，点击一个，其他的都要取消
     */
    BDVoucherSetting.prototype.initCheckBoxEvent = function () {
        var _this = this;
        //找到页面所有可见的li
        $("li:visible[" + this.settingItemIDName + "][" + this.isCheckboxName + "=0]").find("input[type='checkbox']").off("click.vs").on("click.vs", function (event) {
            var checkbox = $(event.target || event.srcElement);
            //取消不做处理
            if (!checkbox.is(":checked"))
                return;
            //同级的要取消
            var siblings = checkbox.parent().siblings().find("input[type='checkbox']");
            siblings.each(function (index, elem) {
                if ($(elem).is(":checked")) {
                    $(elem).removeAttr("checked");
                    _this.handleCheckboxModified($(elem));
                }
            });
        });
        $("li:visible[" + this.settingItemIDName + "]").find("input[type='checkbox']").off("click.vs1").on("click.vs1", function (event) {
            var checkbox = $(event.target || event.srcElement);
            _this.handleCheckboxModified(checkbox);
        });
    };
    /**
     * 处理checkbox是否做了修改
     * @param checkbox
     */
    BDVoucherSetting.prototype.handleCheckboxModified = function (checkbox) {
        var srcValue = +(checkbox.parent().attr(this.srcValueName));
        var nowValue = checkbox.is(":checked") ? 1 : 0;
        var span = checkbox.parent().find("span");
        span.removeClass(this.modifiedClassName);
        if (srcValue != nowValue)
            span.addClass(this.modifiedClassName);
    };
    /**
     * 获取设置数据
     */
    BDVoucherSetting.prototype.getSettingData = function () {
        //找到页面所有可见的li
        var liList = $("li:visible[" + this.settingItemIDName + "]");
        var settingList = [];
        for (var i = 0; i < liList.length; i++) {
            var srcValue = +(liList.eq(i).attr(this.srcValueName));
            var nowValue = $("input[type='checkbox']", liList.eq(i)).eq(0).is(":checked") ? 1 : 0;
            if (srcValue != nowValue) {
                settingList.push({
                    MItemID: liList.eq(i).attr(this.settingItemIDName),
                    MStatus: nowValue ? true : false
                });
            }
        }
        return settingList;
    };
    /**
     * 保存设置数据
     */
    BDVoucherSetting.prototype.saveSettingData = function () {
        var _this = this;
        var data = this.getSettingData();
        if (data.length == 0) {
            mDialog.alert(HtmlLang.Write(LangModule.BD, "NoSettingChanged", "没有可保存的修改项"));
            return;
        }
        mDialog.confirm(HtmlLang.Write(LangModule.BD, "SaveVoucherSettingConfirm", "修改凭证设置后，新建业务单据将会以新的规则创建凭证，是否确认修改?"), function () {
            mAjax.submit(_this.saveVoucherSettingUrl, { list: data }, function (result) {
                if (result.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessfully", "保存成功!"));
                    _this.loadSettingData();
                }
                else {
                    if (result.Message) {
                        mDialog.alert("<div class='popup-list-msg'>" + result.Message + "</div>", null, 1, true);
                    }
                    else {
                        mDialog.message(HtmlLang.Write(LangModule.Common, "SaveFailed", "保存失败!"));
                    }
                }
            });
        });
    };
    /**
     * 获取一个setting是否显示
     * @param id
     */
    BDVoucherSetting.prototype.isSettingShow = function (id) {
        this.getSettingIdList();
        return this.settingIdList.contains(id);
    };
    /**
     * 获取可显示的项目
     */
    BDVoucherSetting.prototype.getSettingIdList = function () {
        this.settingIdList = [];
        var list = $("input[type='checkbox'][settingId]:checked");
        for (var i = 0; i < list.length; i++) {
            this.settingIdList.push(+(list.eq(i).attr("settingId")));
        }
        return this.settingIdList;
    };
    /**
     * 初始化事件
     */
    BDVoucherSetting.prototype.initEvent = function () {
        var _this = this;
        //高级设置按钮
        $(this.advanceSettingButton).mTip({
            target: $(this.advanceSetting),
            width: 200,
            parent: $(this.advanceSetting).parent()
        });
        //保存按钮
        $(this.saveButton).off("click.vs").on("click.vs", function () {
            if (_this.currentIndex == 0)
                _this.saveSettingData();
            else
                _this.savePrefixSettingModel();
        });
        //确认按钮
        $(this.confirmButton).off("click.vs").on("click.vs", function () {
            $(".m-tip-top-div").hide();
            _this.getSettingIdList();
            _this.loadSettingData();
        });
    };
    return BDVoucherSetting;
}());
//# sourceMappingURL=BDVoucherSetting.js.map