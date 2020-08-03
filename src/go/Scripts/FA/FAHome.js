/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="model/FAModels.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
/*
    这种类用来统一处理固定资产模块的 数据加载  数据删除  等通用性操作
*/
var FAHome = /** @class */ (function () {
    function FAHome() {
        this.normal = 0;
        this.sold = 1;
        this.disposed = 2;
        //获取资产列表
        this.getFixAssetsUrl = "/FA/FAFixAssets/GetFixAssetsPageList";
        //获取一个资产，需要获取详情
        this.getFixAssetsModelUrl = "/FA/FAFixAssets/GetFixAssetsModel";
        //删除具体资产url
        this.deleteFixAssetsUrl = "/FA/FAFixAssets/DeleteFixAssets";
        //保存资产卡片
        this.saveFixAssetsUrl = "/FA/FAFixAssets/SaveFixAssets";
        //编辑资产url
        this.editFixAssetsUrl = "/FA/FAFixAssets/EditFixAssets";
        //处置资产
        this.handleFixAssetsUrl = "/FA/FAFixAssets/HandleFixAssets";
        //获取资产数量
        this.getFixAssetsTabInfo = "/FA/FAFixAssets/GetFixAssetsTabInfo";
        //设置费用折旧科目为默认
        this.setExpenseAccountDefaultUrl = "/FA/FAFixAssets/SetExpenseAccountDefault";
        //获取合并后的核算维度值
        this.getMergeCheckGroupValueModelUrl = "/FA/FAFixAssets/GetMergeCheckGroupValueModel";
        //获取资产类别的url
        this.getFixAssetsTypeUrl = "/FA/FAFixAssets/GetFixAssetsTypeList";
        //获取一个资产类别，需要获取详情
        this.getFixAssetsTypeModelUrl = "/FA/FAFixAssets/GetFixAssetsTypeModel";
        //删除资产类别的url
        this.deleteFixAssetTypeUrl = "/FA/FAFixAssets/DeleteFixAssetsType";
        //保存资产类别url
        this.saveFixAssetsTypeUrl = "/FA/FAFixAssets/SaveFixAssetsType";
        //编辑资产类别url
        this.editFixAssetsTypeUrl = "/FA/FAFixAssets/EditFixAssetsType";
        //获取折旧列表
        this.getDepreciationPageListUrl = "/FA/FAFixAssets/GetSummaryDepreciationPageList";
        //获取折旧列表
        this.getDepreciationListUrl = "/FA/FAFixAssets/GetDetailDepreciationList";
        //计提折旧
        this.depreciateUrl = "/FA/FAFixAssets/DepreciatePeriod";
        //批量设置
        this.batchSetupUrl = "/FA/FAFixAssets/BatchSetupDepreciation";
        //保存科目设置url
        this.saveDepreciationListUrl = "/FA/FAFixAssets/SaveDepreciationList";
        //计提折旧链接
        this.depreciatePeriodEditUrl = "/FA/FAFixAssets/DepreciatePeriodEdit";
        this.noCheckGroupValueDiv = ".fa-no-checkgroupdvalue";
        //保存的默认值
        this.SAVE = 0;
        this.SAVE_NEW = 1;
        this.SAVE_COPY = 2;
        this.SAVE_REFRESH = 3;
        //必录
        this.required = 2;
        //可选
        this.optional = 1;
        //禁用
        this.disabled = 0;
        //核算维度设置的table
        this.checkGroupSetupTable = ".fa-checkgroupvalue-table";
        this.checkGroupTipsTable = ".fa-checkgroupvalue-tips-table";
        this.divCheckGroupTips = "#divCheckGroupTips";
        this.saveCheckGroupValueButton = '.fa-checkgroupvalue-save';
        //核算维度设置的公共类
        this.checkType = new GLCheckType();
        //用来保存用户设置的核算维度
        this.checkGroupValueModel = {};
        this.glHome = new GLVoucherHome();
    }
    //获取折旧方法的函数
    FAHome.prototype.getDepreciationTypeArray = function () {
        return [{ name: HtmlLang.Write(LangModule.FA, 'CompositeLifeMethod', '平均年限法'), value: '0' }];
    };
    //获取折旧方法的函数
    FAHome.prototype.getDepreciationFromArray = function () {
        return [
            { name: HtmlLang.Write(LangModule.FA, 'DepreciateFromNextMonth', '当期采购下期折旧'), value: '0' },
            { name: HtmlLang.Write(LangModule.FA, 'DepreciateFromCurrentMonth', '当期采购当期折旧'), value: '1' }
        ];
    };
    /*
        获取固定资产类别
    */
    FAHome.prototype.getFixAssetTypeList = function (func, itemID) {
        mAjax.post(this.getFixAssetsTypeUrl, null, function (d) {
            return func(d);
        }, null);
    };
    /*
        获取固定资产类别
    */
    FAHome.prototype.getFixAssetTypeModel = function (func, itemID) {
        mAjax.post(this.getFixAssetsTypeModelUrl, { itemID: itemID }, function (d) {
            return func(d);
        }, null, true);
    };
    /*
        删除资产类别
    */
    FAHome.prototype.deleteFixAssetsType = function (idList, func) {
        mAjax.submit(this.deleteFixAssetTypeUrl, { itemIDs: idList }, func);
    };
    /*
        保存资产类别
    */
    FAHome.prototype.saveFixAssetsType = function (data, func) {
        mAjax.submit(this.saveFixAssetsTypeUrl, { model: data }, func);
    };
    //编辑资产类型
    FAHome.prototype.showEditFixAssetsType = function (itemID, func) {
        var title = itemID == null || itemID.length == 0 ? HtmlLang.Write(LangModule.Common, "New", "新增") : HtmlLang.Write(LangModule.Common, "Edit", "编辑");
        //直接弹窗
        mDialog.show({
            mTitle: title + HtmlLang.Write(LangModule.FA, 'FixAssetsType', '资产类别'),
            mDrag: "mBoxTitle",
            mShowbg: true,
            mContent: "iframe:" + this.editFixAssetsTypeUrl + "?itemID=" + (itemID == null ? '' : itemID),
            mCloseCallback: func,
            mWidth: 840,
            mHeight: 350
        });
        mDialog.max();
    };
    //编辑资产卡片
    FAHome.prototype.showEditFixAssets = function (itemID, func) {
        var title = itemID == null || itemID.length == 0 ? HtmlLang.Write(LangModule.FA, "NewFixAssets", "新增资产卡片") : HtmlLang.Write(LangModule.FA, "EditFixAssets", "编辑资产卡片");
        //直接弹窗
        mDialog.show({
            mTitle: title,
            mDrag: "mBoxTitle",
            mShowbg: true,
            mMax: false,
            mContent: "iframe:" + this.editFixAssetsUrl + "?itemID=" + (itemID == null ? '' : itemID),
            mCloseCallback: func
        });
        mDialog.max();
    };
    //批量设置折旧情况
    FAHome.prototype.batchSetupDepreciation = function (filter, itemIDs, func) {
        var title = HtmlLang.Write(LangModule.FA, "BatchSetupDepreciation", "批量设置卡片折旧");
        //直接弹窗
        mDialog.show({
            mTitle: title,
            mDrag: "mBoxTitle",
            mShowbg: true,
            mMax: false,
            mContent: "iframe:" + this.batchSetupUrl + "?year=" + filter.Year + '&period=' + filter.Period,
            mCloseCallback: func,
            mPostData: { "itemIDs": itemIDs }
        });
        mDialog.max();
    };
    //计提折旧
    FAHome.prototype.depreciatePeriodEdit = function (filter, func) {
        var title = HtmlLang.Write(LangModule.FA, "SetupDepreciation", "卡片折旧设置");
        //直接弹窗
        mDialog.show({
            mTitle: title,
            mDrag: "mBoxTitle",
            mShowbg: true,
            mMax: false,
            mContent: "iframe:" + this.depreciatePeriodEditUrl + "?year=" + filter.Year + "&period=" + filter.Period,
            mCloseCallback: func
        });
        mDialog.max();
    };
    /**
     * 对某一期进行计提折旧
     */
    FAHome.prototype.depreciatePeriod = function (filter, func) {
        mAjax.submit(this.depreciateUrl, { filter: filter }, function (data) {
            if (data.Success) {
                mDialog.message(HtmlLang.Write(LangModule.FA, "DepreciateSuccessfully", "计提成功"));
                if ($.isFunction(func))
                    func(data);
            }
            else {
                mDialog.error(data.Message);
            }
        });
    };
    /*
        获取固定资产
    */
    FAHome.prototype.getFixAssetList = function (param, func) {
        mAjax.post(this.getFixAssetsUrl, { filter: param }, func, null, true);
    };
    /*
        获取固定资产卡片（单张）
    */
    FAHome.prototype.getFixAssetModel = function (func, itemID, isCopy) {
        mAjax.post(this.getFixAssetsModelUrl, { itemID: itemID, isCopy: isCopy }, function (d) {
            return func(d);
        }, null, true);
    };
    /*
        删除资产
    */
    FAHome.prototype.deleteFixAssets = function (idList, func) {
        var _this = this;
        mDialog.confirm(HtmlLang.Write(LangModule.FA, "Sure2DeleteFixAssets", "确定删除资产卡片吗?"), function () {
            mAjax.submit(_this.deleteFixAssetsUrl, { itemIDs: idList }, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.Common, "DeleteSuccessfully", "删除成功"));
                    if ($.isFunction(func))
                        func();
                }
                else {
                    mDialog.alert(data.Message);
                }
            });
        });
    };
    /**
     * 处置固定资产
     * @param idList
     * @param newStatus
     */
    FAHome.prototype.handleFixAssets = function (idList, newStatus, func) {
        var _this = this;
        mDialog.confirm(newStatus == this.normal ?
            HtmlLang.Write(LangModule.FA, "Sure2UnhandleFixAssets", "确定撤销处置资产卡片吗?") :
            (newStatus == this.sold ? HtmlLang.Write(LangModule.FA, "Sure2SellFixAssets", "确定售出所选资产卡片吗?")
                : HtmlLang.Write(LangModule.FA, "Sure2DisposeFixAssets", "确定作废所选资产卡片吗?")), function () {
            mAjax.submit(_this.handleFixAssetsUrl, { itemIDs: idList, type: newStatus }, function (data) {
                if (data.Success) {
                    mDialog.message(newStatus == _this.normal ? HtmlLang.Write(LangModule.FA, "UnHandleSuccessfully", "资产处置成功")
                        : HtmlLang.Write(LangModule.FA, "HandleSuccessfully", "资产处置成功"));
                    if ($.isFunction(func))
                        func();
                }
                else {
                    mDialog.alert(data.Message);
                }
            });
        });
    };
    /*
        保存资产
    */
    FAHome.prototype.saveFixAssets = function (data, refreshType, isChange) {
        var _this = this;
        //如果是变更，则需要弹出提醒
        var save = function (d, r) {
            mAjax.submit(_this.saveFixAssetsUrl, { model: d }, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessfully", "保存成功!"));
                    switch (r) {
                        //如果是保存并新增，则，保存完以后，刷新页面
                        case _this.SAVE_NEW:
                            $(".mBoxTitle h3", parent.document).text(HtmlLang.Write(LangModule.FA, 'NewFixAssets', '新增资产卡片'));
                            return mWindow.reload(_this.editFixAssetsUrl);
                        //复制的话，需要把ID传过去
                        case _this.SAVE_COPY:
                            $(".mBoxTitle h3", parent.document).text(HtmlLang.Write(LangModule.FA, 'NewFixAssets', '新增资产卡片'));
                            return mWindow.reload(_this.editFixAssetsUrl + "?itemID=" + data.ObjectID + "&isCopy=true");
                        //保存并重新加载页面
                        case _this.SAVE_REFRESH:
                            return mWindow.reload(_this.editFixAssetsUrl + "?itemID=" + data.ObjectID + "&isCopy=false");
                        //保存并关闭页面
                        default:
                            return mDialog.close();
                            ;
                    }
                }
                else {
                    mDialog.error(data.Message);
                }
            });
        };
        //如果是变更，则需要用户点击确认
        if (isChange === true) {
            mDialog.confirm(HtmlLang.Write(LangModule.FA, "ConfirmToChangeFixAssets", "变更系统自动计算调整影响金额，变更后会自动更新卡片信息，系统不会生成相关凭证，需要手动进行记账处理，请确认变更?"), function () {
                return save(data, refreshType);
            });
        }
        else {
            save(data, refreshType);
        }
    };
    /*
       获取固定资产
   */
    FAHome.prototype.GetDepreciationPageList = function (param, func) {
        mAjax.post(this.getDepreciationPageListUrl, { filter: param }, func, null, true);
    };
    /*
       获取固定资产 不分页
   */
    FAHome.prototype.GetDepreciationList = function (param, func) {
        mAjax.post(this.getDepreciationListUrl, { filter: param }, func, null, true);
    };
    /**
     * 核算维度的启用状况
     * @param checkType1
     * @param checkType2
     */
    FAHome.prototype.mergeCheckGroup = function (checkType1, checkType2) {
        return (checkType1 == this.required || checkType2 == this.required) ? this.required : ((checkType1 == this.optional || checkType2 == this.optional) ? this.optional : this.disabled);
    };
    /**
     * 合并多个科目的核算维度
     * @param account
     */
    FAHome.prototype.mergeAccountCheckGroup = function (account) {
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        account = account == null ? {} : account;
        var result = $.extend(true, {}, account.MCheckGroupModel);
        for (var i = 0; args != null && args != undefined && i < args.length; i++) {
            if (args[i] == null)
                continue;
            result.MContactID = this.mergeCheckGroup(result.MContactID, args[i].MCheckGroupModel.MContactID);
            result.MEmployeeID = this.mergeCheckGroup(result.MEmployeeID, args[i].MCheckGroupModel.MEmployeeID);
            result.MMerItemID = this.mergeCheckGroup(result.MMerItemID, args[i].MCheckGroupModel.MMerItemID);
            result.MExpItemID = this.mergeCheckGroup(result.MExpItemID, args[i].MCheckGroupModel.MExpItemID);
            result.MPaItemID = this.mergeCheckGroup(result.MPaItemID, args[i].MCheckGroupModel.MPaItemID);
            result.MTrackItem1 = this.mergeCheckGroup(result.MTrackItem1, args[i].MCheckGroupModel.MTrackItem1);
            result.MTrackItem2 = this.mergeCheckGroup(result.MTrackItem2, args[i].MCheckGroupModel.MTrackItem2);
            result.MTrackItem3 = this.mergeCheckGroup(result.MTrackItem3, args[i].MCheckGroupModel.MTrackItem3);
            result.MTrackItem4 = this.mergeCheckGroup(result.MTrackItem4, args[i].MCheckGroupModel.MTrackItem4);
            result.MTrackItem5 = this.mergeCheckGroup(result.MTrackItem5, args[i].MCheckGroupModel.MTrackItem5);
        }
        return result;
    };
    /**
    * 检查一个核算维度是否有内容
    * @param checkGroup
    */
    FAHome.prototype.hasEnabledCheckGroupValue = function (checkGroupValue) {
        return checkGroupValue != null && checkGroupValue != undefined && ((checkGroupValue.MContactID != null && checkGroupValue.MContactID != undefined)
            || (checkGroupValue.MEmployeeID != null || checkGroupValue.MEmployeeID != undefined)
            || (checkGroupValue.MMerItemID != null || checkGroupValue.MMerItemID != undefined)
            || (checkGroupValue.MExpItemID != null || checkGroupValue.MExpItemID != undefined)
            || (checkGroupValue.MPaItemID != null || checkGroupValue.MPaItemID != undefined)
            || (checkGroupValue.MTrackItem1 != null || checkGroupValue.MTrackItem1 != undefined)
            || (checkGroupValue.MTrackItem2 != null || checkGroupValue.MTrackItem2 != undefined)
            || (checkGroupValue.MTrackItem3 != null || checkGroupValue.MTrackItem3 != undefined)
            || (checkGroupValue.MTrackItem4 != null || checkGroupValue.MTrackItem4 != undefined)
            || (checkGroupValue.MTrackItem5 != null || checkGroupValue.MTrackItem5 != undefined));
    };
    /**
   * 检查一个核算维度是否有内容
   * @param checkGroup
   */
    FAHome.prototype.hasEnabledCheckGroup = function (checkGroup) {
        return checkGroup != null && checkGroup != undefined && ((checkGroup.MContactID == this.required || checkGroup.MContactID == this.optional || checkGroup.MContactID == undefined || checkGroup.MContactID == null)
            || (checkGroup.MEmployeeID == this.required || checkGroup.MEmployeeID == this.optional || checkGroup.MEmployeeID == undefined || checkGroup.MEmployeeID == null)
            || (checkGroup.MMerItemID == this.required || checkGroup.MMerItemID == this.optional || checkGroup.MMerItemID == undefined || checkGroup.MMerItemID == null)
            || (checkGroup.MExpItemID == this.required || checkGroup.MExpItemID == this.optional || checkGroup.MExpItemID == undefined || checkGroup.MExpItemID == null)
            || (checkGroup.MPaItemID == this.required || checkGroup.MPaItemID == this.optional || checkGroup.MPaItemID == undefined || checkGroup.MPaItemID == null)
            || (checkGroup.MTrackItem1 == this.required || checkGroup.MTrackItem1 == this.optional || checkGroup.MTrackItem1 == undefined || checkGroup.MTrackItem1 == null)
            || (checkGroup.MTrackItem2 == this.required || checkGroup.MTrackItem2 == this.optional || checkGroup.MTrackItem2 == undefined || checkGroup.MTrackItem2 == null)
            || (checkGroup.MTrackItem3 == this.required || checkGroup.MTrackItem3 == this.optional || checkGroup.MTrackItem3 == undefined || checkGroup.MTrackItem3 == null)
            || (checkGroup.MTrackItem4 == this.required || checkGroup.MTrackItem4 == this.optional || checkGroup.MTrackItem4 == undefined || checkGroup.MTrackItem4 == null)
            || (checkGroup.MTrackItem5 == this.required || checkGroup.MTrackItem5 == this.optional || checkGroup.MTrackItem5 == undefined || checkGroup.MTrackItem5 == null));
    };
    /**
     * 将用户设置的核算维度显示到tips里面
     * @param checkGroupValue
     * @param href
     */
    FAHome.prototype.showCheckGroupValueTips = function (checkGroupValue, href, callback, checkGroup) {
        //如果没有合法的值，则直接不初始化
        if (!this.hasEnabledCheckGroupValue(checkGroupValue) || !this.hasEnabledCheckGroup(checkGroup)) {
            $(href).off("mouseover");
            return;
        }
        ;
        //先弹出框
        $(href).mTip({
            target: $(this.divCheckGroupTips),
            width: 200,
            parent: $(this.divCheckGroupTips).parent(),
            callback: callback,
            trigger: "mouseover"
        });
        //填充内部的内容
        var checkGroupValues = [
            this.getCheckGroupValue(checkGroupValue.MContactIDTitle, checkGroupValue.MContactIDName, checkGroup.MContactID),
            this.getCheckGroupValue(checkGroupValue.MEmployeeIDTitle, checkGroupValue.MEmployeeIDName, checkGroup.MEmployeeID),
            this.getCheckGroupValue(checkGroupValue.MMerItemIDTitle, checkGroupValue.MMerItemIDName, checkGroup.MMerItemID),
            this.getCheckGroupValue(checkGroupValue.MExpItemIDTitle, checkGroupValue.MExpItemIDName, checkGroup.MExpItemID),
            this.getCheckGroupValue(checkGroupValue.MPaItemIDTitle, checkGroupValue.MPaItemIDName, checkGroup.MPaItemID),
            this.getCheckGroupValue(checkGroupValue.MTrackItem1Title, checkGroupValue.MTrackItem1Name, checkGroup.MTrackItem1),
            this.getCheckGroupValue(checkGroupValue.MTrackItem2Title, checkGroupValue.MTrackItem2Name, checkGroup.MTrackItem2),
            this.getCheckGroupValue(checkGroupValue.MTrackItem3Title, checkGroupValue.MTrackItem3Name, checkGroup.MTrackItem3),
            this.getCheckGroupValue(checkGroupValue.MTrackItem4Title, checkGroupValue.MTrackItem4Name, checkGroup.MTrackItem4),
            this.getCheckGroupValue(checkGroupValue.MTrackItem5Title, checkGroupValue.MTrackItem5Name, checkGroup.MTrackItem5)
        ];
        $("tr:not(.demo)", this.checkGroupTipsTable).remove();
        var demoTr = $("tr.demo", this.checkGroupTipsTable);
        var count = 0;
        for (var i = 0; i < checkGroupValues.length; i++) {
            if (checkGroupValues[i].value == null || checkGroupValues[i].value == undefined || checkGroupValues[i].value.length == 0)
                continue;
            var tr = demoTr.clone().insertBefore(demoTr).show().removeClass("demo");
            $("td:eq(0)", tr).html(mText.encode(checkGroupValues[i].name) + ":");
            $("td:eq(1)", tr).html(mText.encode(checkGroupValues[i].value));
            count++;
        }
        if (count == 0)
            $(this.noCheckGroupValueDiv).show();
        else
            $(this.noCheckGroupValueDiv).hide();
    };
    /**
     * 判断一个维度是否启用
     * @param value
     */
    FAHome.prototype.getCheckGroupValue = function (title, name, value) {
        return (value == 1 || value == 2 || value == undefined || value == null) ? { name: title, value: name } : { name: null, value: null };
    };
    /**
     * 弹出核算维度设置div
     * @param checkGroup
     * @param div
     * @param func
     */
    FAHome.prototype.showCheckGroupSetupDiv = function (checkGroup, checkGroupValue, id, open, close) {
        var _this = this;
        //如果核算维度为空，需要提醒用户选择科目
        if (checkGroup == null) {
            $("body").unmask();
            return mDialog.alert(HtmlLang.Write(LangModule.FA, 'PleaseSelectAnyAccountWithAssitanceCheck', '请选择一个带有核算维度的科目!'));
        }
        //如果核算维度为空，需要提醒用户选择科目
        if (!this.hasEnabledCheckGroup(checkGroup)) {
            $("body").unmask();
            return mDialog.alert(HtmlLang.Write(LangModule.FA, 'AccountSelectedHasNoAssitanceCheck', '所选科目没有设置核算维度!'));
        }
        //赋默认值
        checkGroupValue = checkGroupValue == undefined || checkGroupValue == null ? {} : checkGroupValue;
        this.checkGroupValueModel = checkGroupValue;
        //先弹出框
        mDialog.show({
            mTitle: HtmlLang.Write(LangModule.FA, "AssistanceCheckSetup", "核算维度设置"),
            mDrag: "mBoxTitle",
            mContent: "id:" + id.replace('#', ''),
            mShowbg: true,
            mHeight: 300,
            mWidth: 400,
            mMax: true,
            mCloseCallback: function () {
                //先执行回调
                close(_this.checkGroupValueModel);
            },
            //mNoMaxParent: true,
            mOpenCallback: function () {
                //将元素绑定到对应的列表
                _this.bindCheckGroup2Dom({
                    name: checkGroup.MContactID,
                    value: checkGroupValue.MContactID
                }, {
                    name: checkGroup.MEmployeeID,
                    value: checkGroupValue.MEmployeeID
                }, {
                    name: checkGroup.MMerItemID,
                    value: checkGroupValue.MMerItemID
                }, {
                    name: checkGroup.MExpItemID,
                    value: checkGroupValue.MExpItemID
                }, {
                    name: checkGroup.MPaItemID,
                    value: checkGroupValue.MPaItemID
                }, {
                    name: checkGroup.MTrackItem1,
                    value: checkGroupValue.MTrackItem1
                }, {
                    name: checkGroup.MTrackItem2,
                    value: checkGroupValue.MTrackItem2
                }, {
                    name: checkGroup.MTrackItem3,
                    value: checkGroupValue.MTrackItem3
                }, {
                    name: checkGroup.MTrackItem4,
                    value: checkGroupValue.MTrackItem4
                }, {
                    name: checkGroup.MTrackItem5,
                    value: checkGroupValue.MTrackItem5
                });
                $("body").unmask();
                //初始化保存的事件
                $(_this.saveCheckGroupValueButton).off("click").on("click", function (event) {
                    //重新从弹出框里面取值
                    _this.getCheckGroupValueModelFromDom($(id));
                    //关闭弹出框
                    mDialog.close();
                });
            }
        });
    };
    /**
     * 从一个dom里面获取核算维度设置的值
     */
    FAHome.prototype.getCheckGroupValueModelFromDom = function (select) {
        var inputs = $(".fa-checkgroupvalue-input");
        for (var i = 0; i < inputs.length; i++) {
            var input = inputs.eq(i);
            var data = input.data("data");
            if (!!data) {
                var showType = data.MShowType;
                var columnName = data.MCheckTypeColumnName;
                var id = showType == 0 ? input.combobox("getValue") : input.combotree("getValue");
                var text = showType == 0 ? input.combobox("getText") : input.combotree("getText");
                this.selectCheckGroupValue({ id: id, text: text }, input, null);
            }
        }
        return this.checkGroupValueModel;
    };
    /**
     * 把核算维度绑定到元素上
     * @param value
     * @param args
     */
    FAHome.prototype.bindCheckGroup2Dom = function (nameValue) {
        var _this = this;
        var args = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            args[_i - 1] = arguments[_i];
        }
        var table = $(this.checkGroupSetupTable);
        //demo行
        var demoTr = $("tr.demo", table);
        //删除未起作用的行
        $("tr:not(.demo)", table).remove();
        var all = [nameValue].concat(args != null && args != undefined && args.length > 0 ? args : []);
        for (var i = 0; i < all.length; i++) {
            //联系人
            if (all[i].name == this.required || all[i].name == this.optional) {
                //复制一行
                var copyTr = demoTr.clone();
                //追加并显示
                copyTr.insertBefore(demoTr).show().removeClass("demo");
                //绑定元素
                this.checkType.bindCheckType2Dom(all[i].name, i, $(".fa-checkgroupvalue-name", copyTr), $(".fa-checkgroupvalue-input", copyTr), 0, {
                    height: 26,
                    width: 200,
                    hideWhenClose: true,
                    onSelect: function (row, valueDom, titleDom) { return _this.selectCheckGroupValue(row, valueDom, titleDom); }
                }, all[i].value, true);
            }
        }
    };
    /**
     * 核算维度设置界面选择核算维度触发的事件
     * @param row
     * @param input
     */
    FAHome.prototype.selectCheckGroupValue = function (row, valueDom, titleDom) {
        //绑定的所有相应核算维度对应的值
        var data = valueDom.data("data");
        if (data == null || data == undefined)
            return;
        //列名
        var columName = data.MCheckTypeColumnName;
        //只有有了文本才能取其值
        row.id = !!row.text ? row.id : null;
        //设置核算维度 ContactID= '11111'
        this.checkGroupValueModel = mObject.setPropertyValue(this.checkGroupValueModel, columName, row.id);
        //设置显示的维度对应的值 ContactID 罗群益
        this.checkGroupValueModel = mObject.setPropertyValue(this.checkGroupValueModel, columName + 'Name', row.text);
        //设置维度的类型名字
        this.checkGroupValueModel = mObject.setPropertyValue(this.checkGroupValueModel, columName + 'Title', data.MCheckTypeName);
    };
    /**
     * 获取核算维度名字和对应的ID
     */
    FAHome.prototype.getCheckGroupNameArray = function (isSmart) {
        var result = [
            { name: this.checkType.getCheckTypeName(0), value: 0 },
            { name: this.checkType.getCheckTypeName(1), value: 1 },
            { name: this.checkType.getCheckTypeName(2), value: 2 },
            { name: this.checkType.getCheckTypeName(3), value: 3 },
            { name: this.checkType.getCheckTypeName(4), value: 4 },
            { name: this.checkType.getCheckTypeData(5, null, false, false).MCheckTypeName, value: 5 },
            { name: this.checkType.getCheckTypeData(6, null, false, false).MCheckTypeName, value: 6 },
            { name: this.checkType.getCheckTypeData(7, null, false, false).MCheckTypeName, value: 7 },
            { name: this.checkType.getCheckTypeData(8, null, false, false).MCheckTypeName, value: 8 },
            { name: this.checkType.getCheckTypeData(9, null, false, false).MCheckTypeName, value: 9 },
        ];
        return result.where('x.name != null && x.name != undefined && x.name.length > 0' + (isSmart ? ' && x.value != 4 ' : ''));
    };
    /**
     * 获取科目列表
     */
    FAHome.prototype.getAccountList = function (force) {
        if (force || this._accountList == null || this._accountList == undefined || this._accountList.length == 0)
            this._accountList = this.glHome.getAccountData(null, null, false);
        return this._accountList;
    };
    /**
     * 获取一个科目
     * @param code
     */
    FAHome.prototype.getAccount = function (code) {
        var account = code == null || code == undefined || code == '' ? null : this.getAccountList().where('x.MCode == "' + code + '"');
        return account == null || account.length == 0 ? null : account[0];
    };
    /**
     * 获取选择的科目
     */
    FAHome.prototype.getSelectedAccountModel = function (input, value) {
        //用户选择的科目代码
        var selectedAccountCode = $(input).combobox('getValue');
        var selectedAccuontText = $(input).combobox('getText');
        //必须有文本才代表有值
        selectedAccountCode = !!selectedAccuontText ? selectedAccountCode : null;
        //如果选的code和默认的值相同，则直接返回value
        if (value != null && value != undefined && value.MCode === selectedAccountCode && selectedAccuontText == value.MFullName)
            return value;
        if (selectedAccountCode == null || selectedAccountCode == undefined || selectedAccountCode.length == 0) {
            return (value = null);
        }
        //从列表里面找到那一条，然后返回
        var account = this.getAccountList().where('x.MCode ==="' + selectedAccountCode + '"')[0];
        if (account.MFullName !== selectedAccuontText)
            return (value = null);
        return account;
    };
    /**
     * 初始化固定资产类表各项资产的数量
     * @param tabs
     */
    FAHome.prototype.initFixAssetsTabTitle = function (tabs) {
        mAjax.post(this.getFixAssetsTabInfo, {}, function (data) {
            var undisposed = data.where('x.MName == "0"').sum('MValue');
            var disposed = data.where('x.MName == "1" || x.MName == "2"').sum('MValue');
            var all = undisposed + disposed;
            tabs.eq(0).text(all + ' ' + HtmlLang.Write(LangModule.FA, "FixAssetsItems", "项"));
            tabs.eq(1).text(undisposed + ' ' + HtmlLang.Write(LangModule.FA, "FixAssetsItems", "项"));
            tabs.eq(2).text(disposed + ' ' + HtmlLang.Write(LangModule.FA, "FixAssetsItems", "项"));
        });
    };
    /**
     * 设置折旧费用科目为默认
     * @param checked
     * @param accountCode
     */
    FAHome.prototype.setDefaultExpenseAccount = function (checked, accountCode, func) {
        //获取折旧费用科目
        mAjax.submit(this.setExpenseAccountDefaultUrl, { check: checked, accountCode: accountCode }, function (data) {
            if (data.Success) {
                mDialog.message(checked ? HtmlLang.Write(LangModule.FA, "SetDefaultSuccessfully", "设置默认折旧费用科目成功!")
                    : HtmlLang.Write(LangModule.FA, "ClearDefaultSuccessfully", "取消默认折旧费用科目成功!"));
                if ($.isFunction(func))
                    func(data);
            }
            else {
                mDialog.message(data.Message);
            }
        });
    };
    /**
     * 获取资产的状态
     * @param status
     */
    FAHome.prototype.getFixAssetsStatus = function (status, row) {
        switch (status) {
            case this.normal:
                if ((row.MDepreciatedAmount + row.MPrepareForDecreaseAmount + row.MSalvageAmount >= row.MOriginalAmount)
                    || row.MDepreciatedPeriods == row.MUsefulPeriods)
                    return HtmlLang.Write(LangModule.FA, "ReadyForHandle", "待处置");
                return HtmlLang.Write(LangModule.FA, "Normal", "正常");
            case this.sold:
                return HtmlLang.Write(LangModule.FA, "Sold", "已售");
            case this.disposed:
                return HtmlLang.Write(LangModule.FA, "Disposed", "报废");
            default:
                return '';
        }
    };
    /**
     * 获取资产变更类型
     * @param changeModel
     * @param srcModel
     */
    FAHome.prototype.GetChangeType = function (changeModel, srcModel) {
        var changeType = 0;
        //基本变更类型
        changeType += (changeModel.MName != srcModel.MName || changeModel.MNumber != srcModel.MNumber || changeModel.MPrefix != srcModel.MPrefix || changeModel.MQuantity != srcModel.MQuantity) ? FixAssetsChangeTypeEnum.Basic : 0;
        //其他变更
        changeType += (changeModel.MDepAccountCode != srcModel.MDepAccountCode || changeModel.MFixAccountCode != srcModel.MFixAccountCode || changeModel.MExpAccountCode != srcModel.MExpAccountCode || changeModel.MDepreciatedPeriods != srcModel.MDepreciatedPeriods || changeModel.MDepreciatedAmount != srcModel.MDepreciatedAmount || changeModel.MPeriodDepreciatedAmount != srcModel.MPeriodDepreciatedAmount) ? FixAssetsChangeTypeEnum.Other : 0;
        //折旧策略变更
        changeType += (changeModel.MDepreciationTypeID != srcModel.MDepreciationTypeID || changeModel.MUsefulPeriods != srcModel.MUsefulPeriods || changeModel.MRateOfSalvage != srcModel.MRateOfSalvage) ? FixAssetsChangeTypeEnum.Strategy : 0;
        //原值变更
        changeType += (changeModel.MOriginalAmount != srcModel.MOriginalAmount || (mDate.parse(changeModel.MPurchaseDate).getFullYear() * 12 + mDate.parse(changeModel.MPurchaseDate).getMonth()) != (mDate.parse(srcModel.MPurchaseDate).getFullYear() * 12 + mDate.parse(srcModel.MPurchaseDate).getMonth()) || changeModel.MPrepareForDecreaseAmount != srcModel.MPrepareForDecreaseAmount) ? FixAssetsChangeTypeEnum.Original : 0;
        //处置变更 不会存在处置的情况
        //changeType += (changeModel.MStatus != srcModel.MStatus) ? FixAssetsChangeTypeEnum.Handle : 0;
        return changeType == 0 ? FixAssetsChangeTypeEnum.Other : changeType;
    };
    /**
     * 根据变更的类型，获取名字
     * @param type
     */
    FAHome.prototype.getChangeTypeName = function (type) {
        var name = [];
        //基本信息
        if ((type & 1) == 1) {
            name.push(HtmlLang.Write(LangModule.FA, 'BasicInfoChange', '基本信息（名称、编码等）'));
        }
        //其他
        if ((type & 2) == 2) {
            name.push(HtmlLang.Write(LangModule.FA, 'OtherChange', '其他(科目、核算维度等）'));
        }
        //折旧策略
        if ((type & 4) == 4) {
            name.push(HtmlLang.Write(LangModule.FA, 'DepreciateStrategyChange', '折旧策略（折旧方式、残值率等）'));
        }
        //原值
        if ((type & 8) == 8) {
            name.push(HtmlLang.Write(LangModule.FA, 'OriginalValuePurchaseDateChange', '原值/采购日期'));
        }
        //处置变更
        if ((type & 16) == 16) {
            name.push(HtmlLang.Write(LangModule.FA, 'HandleChange', '处置变更'));
        }
        return name.join(',');
    };
    /**
     * 保存折旧信息
     */
    FAHome.prototype.saveDepreciationList = function (filter, func) {
        mAjax.submit(this.saveDepreciationListUrl, { filter: filter }, function (d) {
            if (d.Success) {
                mDialog.message(HtmlLang.Write(LangModule.FA, 'SaveDerepciationInfoSuccessfully', '保存折旧设置信息成功!'));
                if ($.isFunction(func))
                    func(d);
            }
            else {
                mDialog.message(d.Message);
            }
        });
    };
    /**
     * 根据核算维度的ID，获取对应合并后的model
     * @param Array
     * @param func
     */
    FAHome.prototype.getMergeCheckGroupValueModel = function (checkGroupValueIDs, func) {
        mAjax.post(this.getMergeCheckGroupValueModelUrl, { checkGroupValueIDs: checkGroupValueIDs }, function (data) {
            if ($.isFunction(func))
                func(data);
        });
    };
    /**
     * 日志显示
     * @param itemID
     * @param callback
     */
    FAHome.prototype.showLogDialog = function (itemID, callback) {
        mDialog.show({
            mTitle: HtmlLang.Write(LangModule.Common, "History", "History"),
            mWidth: 800,
            mHeight: 455,
            mDrag: "mBoxTitle",
            mContent: "iframe:/Log/Log/FALogList?ItemID=" + itemID
        });
    };
    /**
     * 固定资产默认的分页数
     */
    FAHome.prototype.getPageList = function () {
        return [10, 20, 50, 100, 200];
    };
    FAHome.prototype.clearGridCheckbox = function (table) {
        $("div.datagrid-header-check input[type='checkbox']:visible").prop("checked", false).attr("value", "");
    };
    /**
     * 将日期转成期间格式的
     * @param date
     */
    FAHome.prototype.formateDate = function (date, addMonth) {
        if (mDate.parse(date)) {
            var d = mDate.parse(date);
            if (addMonth != undefined && addMonth !== 0)
                d = new Date(parseInt(((d.getFullYear() * 12 + d.getMonth() + addMonth) / 12) + ""), parseInt(((d.getFullYear() * 12 + d.getMonth() + addMonth) % 12) + ""), 1);
            return d.getFullYear() < 2000 ? '' : (d.getFullYear() + "-" + (d.getMonth() < 9 ? ('0' + (d.getMonth() + 1)) : (d.getMonth() + 1)));
        }
        return "";
    };
    return FAHome;
}());
//# sourceMappingURL=FAHome.js.map