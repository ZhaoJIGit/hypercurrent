/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
/**
 * 固定资产首页类
 */
var FADepreciationBatchSetup = /** @class */ (function () {
    //构造函数
    function FADepreciationBatchSetup() {
        //表格
        this.depreciateTable = ".bs-depreciate-grid";
        //保存
        this.saveButton = "#aSave";
        //保存并计提
        this.depreciateButton = '.bs-depreciate-button';
        //重新计提
        this.reDepreciateButton = '.bs-re-depreciate-button';
        //年
        this.year = 0;
        //期
        this.period = 0;
        this.originalData = [];
        //编辑
        this.setCheckGroupButton = ".bs-set-checkgroup";
        //查看凭证
        this.viewVoucherButton = ".bs-view-voucher";
        this.isCreate = false;
        //保存当前编辑的行
        this.editIndex = null;
        this.editField = '';
        this.comboboxInput = "input.combobox-f:hidden";
        //核算维度设置
        this.divCheckGroupSetup = "#divCheckGroupSetup";
        //核算维度展示
        this.divCheckGroupTips = '#divCheckGroupTips';
        //鼠标变成等待的样式
        this.waitClass = "fa-wait-cursor";
        //批量设置的两列
        this.batchSetupColumnFields = ['MDepAccountCode', 'MExpAccountCode'];
        this.checkType = new GLCheckType();
        this.home = new FAHome();
        this.voucherHome = new GLVoucherHome();
    }
    /**
     * 初始化
     */
    FADepreciationBatchSetup.prototype.init = function () {
        //获取所有的id
        this.itemIDs = $("#itemIDs").val().length == 0 ? [] : $("#itemIDs").val().split(',');
        //年期
        this.year = +$("#yearInput").val(), this.period = $("#periodInput").val();
        //是否是创建页面
        this.isCreate = this.itemIDs.length == 0;
        //初始化事件
        this.initEvent();
        //加载数据
        this.loadDepreciationData();
    };
    /**
     * 初始化页面的事件
     */
    FADepreciationBatchSetup.prototype.initEvent = function () {
        var _this = this;
        //保存按钮
        $(this.saveButton).off("click").on("click", function () {
            if (_this.editIndex != null) {
                $(_this.depreciateTable).datagrid('endEdit', _this.editIndex);
                _this.mergeGridCell();
            }
            var rows = $(_this.depreciateTable).datagrid('getRows');
            rows = rows.slice(1, rows.length);
            _this.home.saveDepreciationList({
                DepreciationModels: rows,
                Year: _this.year,
                Period: _this.period
            }, function () {
                _this.loadDepreciationData();
            });
        });
        //计提按钮
        $(this.depreciateButton).off("click").on("click", function () {
            if (_this.editIndex != null) {
                $(_this.depreciateTable).datagrid('endEdit', _this.editIndex);
                _this.mergeGridCell();
            }
            var rows = $(_this.depreciateTable).datagrid('getRows');
            rows = rows.slice(1, rows.length);
            _this.home.depreciatePeriod({
                DepreciationModels: _this.equalDepreciationArray(rows, _this.originalData) ? [] : rows,
                Year: _this.year,
                Period: _this.period
            }, function () {
                _this.loadDepreciationData();
            });
        });
        //计提按钮
        $(this.reDepreciateButton).off("click").on("click", function () {
            if (_this.editIndex != null) {
                $(_this.depreciateTable).datagrid('endEdit', _this.editIndex);
                _this.mergeGridCell();
            }
            mDialog.confirm(HtmlLang.Write(LangModule.FA, "AreYouSureToReDepreciate", "重新计提将会删除已创建凭证并重新生成凭证, 是否确认重新计提"), function () {
                var rows = $(_this.depreciateTable).datagrid('getRows');
                rows = rows.slice(1, rows.length);
                _this.home.depreciatePeriod({
                    DepreciationModels: _this.equalDepreciationArray(rows, _this.originalData) ? [] : rows,
                    Year: _this.year,
                    Period: _this.period,
                    IsRedepreciate: true
                }, function () {
                    _this.loadDepreciationData();
                });
            });
        });
        //结束编辑
        $(document).off("click.bs").on("click.bs", function (event) {
            if (_this.editIndex != null && !_this.isComboHrefClick(event)) {
                $(_this.depreciateTable).datagrid('endEdit', _this.editIndex);
                _this.mergeGridCell();
            }
        });
    };
    /**
     * 是否bobobox的超链接点击
     * @param event
     */
    FADepreciationBatchSetup.prototype.isComboHrefClick = function (event) {
        var target = $(event.target || event.srcElement);
        return (target.hasClass("add-combobox-item")
            || target.hasClass("search-combobox-item")
            || target.parent(".add-combobox-item, .search-combobox-item").length > 0
            || (target.hasClass('combo-p') && target.find('add-combobox-item, search-combobox-item').length > 0)
            || (target.parents(".XYTipsWindow").length > 0)
            || (target.parents(".search-combobox-table-tr").length > 0));
    };
    //复制一个数组
    FADepreciationBatchSetup.prototype.cloneDepreciationArray = function (array) {
        var arr = [];
        for (var i = 0; i < array.length; i++) {
            arr.push(_.clone(array[i]));
        }
        return arr;
    };
    //复制一个数组
    FADepreciationBatchSetup.prototype.equalDepreciationArray = function (a, b) {
        if (a == b)
            return true;
        if ((a || []) == (b || []))
            return true;
        if ((a || []).length != (b || []).length)
            return false;
        for (var i = 0; i < a.length; i++) {
            if (!_.isEqual(a[i], b[i]))
                return false;
        }
        return true;
    };
    /**
     * 合并批量设置哪一行
     */
    FADepreciationBatchSetup.prototype.mergeGridCell = function () {
        $(this.depreciateTable).datagrid('mergeCells', {
            index: 0,
            field: 'MFixAssetsNumber',
            colspan: 7,
            type: 'body'
        });
    };
    /**
     * 初始化表格里面的事件
     */
    FADepreciationBatchSetup.prototype.initDepreciationGridEvent = function () {
        var _this = this;
        //核算维度展示
        $(this.setCheckGroupButton).off("mouseover.bs").on("mouseover.bs", function (event) {
            //先结束编辑
            if (_this.editIndex != null) {
                $(_this.depreciateTable).datagrid('endEdit', _this.editIndex);
                _this.initDepreciationGridEvent();
                _this.mergeGridCell();
            }
            //获取当前正在编辑的那一行
            var row = $(_this.depreciateTable).datagrid('getRows')[$(event.target || event.srcElement).attr("index")];
            //初始话tips的事件
            var initTips = function (data, first, checkGroup) {
                if (first === void 0) { first = false; }
                row.MCheckGroupValueModel = data;
                //显示tips
                _this.home.showCheckGroupValueTips(data, $(event.target || event.srcElement), null, {});
                $(event.target || event.srcElement).removeClass(_this.waitClass);
                row.MIsLoadedCheckGroup = true;
                if (first)
                    $(event.target || event.srcElement).trigger("mouseover.tip");
            };
            //获取选择的科目
            var depAccount = _this.home.getAccount(row.MDepAccountCode);
            var expAccount = _this.home.getAccount(row.MExpAccountCode);
            var checkGroup = _this.home.mergeAccountCheckGroup(depAccount, expAccount);
            //如果已经有了核算维度的数据，则直接加载就好了
            if (row.MIsLoadedCheckGroup)
                return initTips(row.MCheckGroupValueModel, false, checkGroup);
            //如果没有选择科目，或者科目的核算维度为0，表示没有启用核算维度
            if ((depAccount == null || depAccount.MCheckGroupModel.MItemID == '0') && (expAccount == null || expAccount.MCheckGroupModel.MItemID == '0'))
                return;
            //如果有选择科目，但是核算维度ID，都是为空的
            //鼠标变成等待的样式
            $(event.target || event.srcElement).addClass(_this.waitClass);
            //保存核算维度值ID
            var idList = [];
            if (row.MDepCheckGroupValueID != null && row.MDepCheckGroupValueID != undefined && row.MDepCheckGroupValueID.length > 0)
                idList.push(row.MDepCheckGroupValueID);
            if (row.MExpCheckGroupValueID != null && row.MExpCheckGroupValueID != undefined && row.MExpCheckGroupValueID.length > 0)
                idList.push(row.MExpCheckGroupValueID);
            //如果有id
            if (idList.length > 0) {
                //从后台读取一次，然后再加载一次
                _this.home.getMergeCheckGroupValueModel(idList, function (data) { return initTips(data, true); });
            }
        }).off("mouseleave.bs").on("mouseleave.bs", function (event) {
            //获取当前正在编辑的那一行
            $(event.target || event.srcElement).removeClass(_this.waitClass);
        });
        //编辑核算维度
        $(this.setCheckGroupButton).off("click").on("click", function (event) {
            //如果还在加载，则先等待加载成功吧
            if ($(event.target || event.srcElement).hasClass(_this.waitClass))
                return;
            //先结束编辑
            if (_this.editIndex != null) {
                $(_this.depreciateTable).datagrid('endEdit', _this.editIndex);
                _this.mergeGridCell();
            }
            //获取当前正在编辑的那一行
            var row = $(_this.depreciateTable).datagrid('getRows')[$(event.target || event.srcElement).attr("index")];
            var depAccount = _this.home.getAccount(row.MDepAccountCode);
            var expAccount = _this.home.getAccount(row.MExpAccountCode);
            var checkGroup = _this.home.mergeAccountCheckGroup(depAccount, expAccount);
            //弹出设置框
            _this.home.showCheckGroupSetupDiv(checkGroup, row.MCheckGroupValueModel, _this.divCheckGroupSetup, null, function (d) {
                row.MCheckGroupValueModel = d;
                _this.home.showCheckGroupValueTips(row.MCheckGroupValueModel, $(event.target || event.srcElement), null, checkGroup);
            });
        });
        //查看凭证
        $(this.viewVoucherButton).off("click.bs").on("click.bs", function (event) {
            _this.voucherHome.editVoucher($(event.target || event.srcElement).attr("MVoucherID"), null);
        });
    };
    /**
     * 加载卡片数据
     */
    FADepreciationBatchSetup.prototype.loadDepreciationData = function () {
        var _this = this;
        //获取数据并加载
        this.home.GetDepreciationList({
            ItemIDs: this.itemIDs,
            NeedPage: false,
            Year: this.year,
            Period: this.period,
            IsCalculate: this.isCreate
        }, function (data) {
            if (data == null || data == undefined || data.length == 0) {
                mDialog.alert(HtmlLang.Write(LangModule.FA, "NoAssetsCards2Depreciate", "本期没有可计提的卡片"), function () {
                    //没有卡片可以编辑的话，就关闭当前页面
                    mDialog.close();
                });
                $(_this.saveButton).hide();
                $(_this.depreciateButton).hide();
                return;
            }
            _this.showDepreciationData(data);
        });
    };
    /**
     * 批量设置科目
     */
    FADepreciationBatchSetup.prototype.batchSetup = function (d) {
        var _this = this;
        $(this.comboboxInput).combobox('hidePanel');
        var rows = $(this.depreciateTable).datagrid('getRows');
        //每一行的对应字段的值都需要更新
        rows.forEach(function (value, index, array) {
            mObject.setPropertyValue(value, _this.editField, d.MCode);
        });
        //重新填充字段
        $(this.depreciateTable).datagrid('loadData', rows);
        this.mergeGridCell();
        this.initDepreciationGridEvent();
    };
    /**
     * 插入一条批量设置的行
     */
    FADepreciationBatchSetup.prototype.getBatchSetupRow = function () {
        return {
            MIsBatchSetup: true
        };
    };
    /**
     * 展示数据到div下面
     * @param data
     */
    FADepreciationBatchSetup.prototype.showDepreciationData = function (data) {
        var _this = this;
        var tableDiv = $(this.depreciateTable);
        this.originalData = this.cloneDepreciationArray(data);
        //如果已经计提了，则只有一个按钮 重新计提
        if (data.where('!!x.MVoucherID').length > 0) {
            $(this.saveButton).hide();
            $(this.depreciateButton).hide();
            $(this.reDepreciateButton).show();
        }
        else {
            $(this.saveButton).show();
            $(this.depreciateButton).show();
            $(this.reDepreciateButton).hide();
        }
        //如果已经初始化了，则直接加载数据就行
        if (tableDiv.attr("inited") == "1") {
            $(tableDiv).datagrid('loadData', [this.getBatchSetupRow()].concat(data));
            this.mergeGridCell();
            $(tableDiv).datagrid("beginEdit", 0);
            return this.initDepreciationGridEvent();
        }
        var accountData = this.home.getAccountList(true);
        //初始化数据
        $(tableDiv).datagrid({
            data: [],
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            scrollX: false,
            width: tableDiv.width() - 10,
            height: ($("body").height() - tableDiv.offset().top - 57),
            columns: [[
                    {
                        field: 'MItemID', title: '', width: 40, hidden: true, align: 'left'
                    },
                    {
                        field: 'MID', title: '', width: 10, align: 'left', hidden: true
                    },
                    {
                        field: 'MFixAssetsNumber', width: 60, title: HtmlLang.Write(LangModule.FA, "FixAssetNumber", "编码"), align: 'left', formatter: function (val, row) {
                            return row.MIsBatchSetup ? ('<span style="float:right;font-size:12px;font-weight:bold" >' + HtmlLang.Write(LangModule.GL, 'BatchSetup', '批量设置') + '</span>') : val;
                        }
                    },
                    {
                        field: 'MFixAssetsName', width: 60, title: HtmlLang.Write(LangModule.FA, "FixAssetName", "名称"), align: 'left'
                    },
                    {
                        field: 'MFATypeIDName', width: 60, title: HtmlLang.Write(LangModule.FA, "FixAssetTypeName", "类别名称"), align: 'left'
                    },
                    {
                        field: 'MTempPeriodDepreciatedAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "OriginalCalculatedAmount", "原始测算金额"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MAdjustAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "AdjustAmount", "调整额"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }
                    },
                    {
                        field: 'MPeriodDepreciatedAmount', width: 60, title: HtmlLang.Write(LangModule.FA, "MFinalDepreciateAmount", "最终折旧金额"), align: 'right', formatter: function (val, rec) {
                            return mMath.toMoneyFormat(val);
                        }, editor: {
                            type: 'numberbox',
                            options: {
                                precision: 2
                            }
                        }
                    },
                    {
                        field: 'MVoucherID', width: 20, title: '', align: 'right', formatter: function (val) { return ''; }
                    },
                    {
                        field: 'MExpAccountCode', width: 60, title: HtmlLang.Write(LangModule.FA, "DepreciateExpenseAccount", "折旧费用科目"), formatter: function (val) {
                            return (val != null && val != undefined && val.length > 0) ? accountData.where('x.MCode == "' + val + '"')[0].MFullName : '';
                        }, align: 'center',
                        editor: {
                            type: "addCombobox",
                            options: {
                                type: 'account',
                                dataOptions: {
                                    data: this.home.getAccountList(),
                                    height: 30,
                                    required: false,
                                    autoSizePanel: true,
                                    textField: 'MFullName',
                                    valueField: 'MCode',
                                    onSelect: function (d) {
                                        //如果当前编辑的字段就是批量设置字段，并且当前编辑的是在第一行
                                        if (_this.editIndex === 0) {
                                            _this.editField = _this.batchSetupColumnFields[1];
                                            _this.batchSetup(d);
                                        }
                                    },
                                    onLoadSuccess: function () {
                                    }
                                },
                                addOptions: {
                                    hasPermission: 1,
                                    //关闭后需要重新加载摘要
                                    callback: function () {
                                        //重新获取摘要
                                        var accountList = _this.home.getAccountList(true);
                                        $(".combobox-f").combobox("loadData", accountList);
                                        $(".combobox-f").combobox("reload");
                                        $(tableDiv).datagrid("getColumnOption", "MExpAccountCode").editor.options.dataOptions.data = accountList;
                                    }
                                }
                            }
                        }
                    },
                    {
                        field: 'MDepAccountCode', width: 60, title: HtmlLang.Write(LangModule.FA, "DepreciateAccount", "累计折旧科目"), formatter: function (val) {
                            return (val != null && val != undefined && val.length > 0) ? accountData.where('x.MCode == "' + val + '"')[0].MFullName : '';
                        }, align: 'center', editor: {
                            type: "addCombobox",
                            options: {
                                type: 'account',
                                dataOptions: {
                                    data: this.home.getAccountList(),
                                    height: 30,
                                    required: false,
                                    autoSizePanel: true,
                                    textField: 'MFullName',
                                    valueField: 'MCode',
                                    onSelect: function (d) {
                                        //如果当前编辑的字段就是批量设置字段，并且当前编辑的是在第一行
                                        if (_this.editIndex === 0) {
                                            _this.editField = _this.batchSetupColumnFields[0];
                                            _this.batchSetup(d);
                                        }
                                    },
                                    onLoadSuccess: function () {
                                    }
                                },
                                addOptions: {
                                    hasPermission: 1,
                                    //关闭后需要重新加载摘要
                                    callback: function () {
                                        //重新获取摘要
                                        var accountList = _this.home.getAccountList(true);
                                        $(".combobox-f").combobox("loadData", accountList);
                                        $(".combobox-f").combobox("reload");
                                        $(tableDiv).datagrid("getColumnOption", "MDepAccountCode").editor.options.dataOptions.data = accountList;
                                    }
                                }
                            }
                        }
                    },
                    {
                        field: 'Operation', width: 60, title: HtmlLang.Write(LangModule.FA, "CreatedVoucher", "凭证"), align: 'left', formatter: function (val, rec, index) {
                            return rec.MIsBatchSetup ? '' : ((rec.MVoucherNumber != null && rec.MVoucherNumber != undefined && rec.MVoucherNumber.length > 0)
                                ? ('<a href="###" MVoucherID="' + rec.MVoucherID + '" class="bs-view-voucher"  style="color: rgb(4, 143, 194);">GL-' + rec.MVoucherNumber + '</a>') : ('<a href="###" class= "bs-set-checkgroup" style="color: rgb(4, 143, 194);" index="' + index + '">' + HtmlLang.Write(LangModule.FA, 'ViewSetCheckGroupValue', '查看/设置核算维度') + '</a>'));
                        }
                    }
                ]],
            onBeforeEdit: function (index, row) {
                //
                if (_this.editIndex != null) {
                    //结束编辑
                    $(tableDiv).datagrid("endEdit", _this.editIndex);
                    _this.mergeGridCell();
                }
                _this.editIndex = index;
            },
            onAfterEdit: function (index, row) {
                //结束编辑
                _this.editIndex = null;
                _this.editField = null;
                //结束编辑以后要重新绑定事件
                _this.initDepreciationGridEvent();
            },
            onClickRow: function (index, row) {
                //如果点击的是添加和删除按钮，则不进行编辑
                if (!$(event.srcElement || event.target).is("a")) {
                    $(tableDiv).datagrid("unselectAll");
                    //结束编辑
                    $(tableDiv).datagrid("beginEdit", index);
                }
            },
            onClickCell: function (index, field, value) {
                _this.editField = field;
            },
            onLoadSuccess: function () {
            }
        });
        $(tableDiv).datagrid('loadData', [this.getBatchSetupRow()].concat(data));
        this.mergeGridCell();
        $(tableDiv).datagrid("beginEdit", 0);
        this.initDepreciationGridEvent();
        $(tableDiv).attr("inited", 1);
    };
    return FADepreciationBatchSetup;
}());
//# sourceMappingURL=FADepreciationBatchSetup.js.map