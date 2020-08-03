/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
///固定资产类型管理首页
var FAFixAssetsTypeHome = /** @class */ (function () {
    function FAFixAssetsTypeHome() {
        var _this = this;
        this.gridBody = ".fa-tp-grid";
        this.gridDiv = ".fa-tp-gridDiv";
        this.editTableButton = ".fp-tp-edit";
        this.removeTableButton = ".fp-tp-remove";
        this.deleteButton = "#divDeleteFixAssetsType";
        this.newButton = "#divNewFixAssetsType";
        this.checkbox = ".fa-tp-checkbox input:checked";
        //初始化页面的删除事件和新增事件
        this.initEvent = function () {
            //新增和编辑是一起的
            $(_this.newButton).off("click").on("click", function () {
                _this.editFixAssetsType(null);
            });
            //批量删除
            $(_this.deleteButton).off("click").on("click", function () {
                var rows = [];
                //把表格中勾选的都筛选出来
                $(_this.checkbox).each(function () {
                    rows.push({ MNumber: $(this).attr("mnumber"), MItemID: $(this).attr("mid") });
                });
                //把非系统预设的拿出来，不让用户进行删除
                var idList = rows.select("MItemID").distinct();
                var numberList = rows.select("MNumber").distinct();
                if (idList == null || idList.length == 0) {
                    return mDialog.alert(HtmlLang.Write(LangModule.FA, "PleaseSelectAnyRecords", "请选择要操作的资产类型!"));
                }
                else {
                    //弹出确认提醒，需要显示编号
                    mDialog.confirm(HtmlLang.Write(LangModule.FA, "AreYourSure2DeleteFixAssetsType", "确认删除资产类型:") + numberList.join(',') + "?", function () {
                        _this.home.deleteFixAssetsType(idList, function (data) {
                            if (data.Success) {
                                mDialog.message(HtmlLang.Write(LangModule.FA, "DeleteSccuessFully", "删除成功!"));
                                _this.loadData();
                            }
                            else {
                                mDialog.alert(data.Message);
                            }
                        });
                    });
                }
            });
        };
        //加载资产类别列表
        this.loadData = function () {
            _this.home.getFixAssetTypeList(_this.showData);
        };
        //初始化表格里面的操作事件
        this.initTableEvent = function () {
            //里面的删除事件
            $(_this.removeTableButton).off("click").on("click", function (event) {
                var button = $(event.target || event.srcElement);
                var typeId = $(button).attr("mid");
                var tableNumber = $(button).attr("mnumber");
                //弹出确认提醒，需要显示编号
                mDialog.confirm(HtmlLang.Write(LangModule.FA, "AreYourSure2DeleteFixAssetsType", "确认删除资产类型:") + tableNumber + "?", function () {
                    _this.home.deleteFixAssetsType([typeId], function (data) {
                        if (data.Success) {
                            mDialog.message(HtmlLang.Write(LangModule.FA, "DeleteSccuessFully", "删除成功!"));
                            _this.loadData();
                        }
                        else {
                            mDialog.alert(data.Message);
                        }
                    });
                });
            }).tooltip({
                content: HtmlLang.Write(LangModule.FA, "Click2DeleteFixAssetType", "点击删除卡片类型")
            });
            //表格里面的编辑事件
            $(_this.editTableButton).off("click").on("click", function (event) {
                var button = $(event.target || event.srcElement);
                _this.editFixAssetsType(button.attr("mid"));
            }).tooltip({
                content: HtmlLang.Write(LangModule.FA, "Click2EditFixAssetType", "点击编辑卡片类型")
            });
        };
        //展示资产类表数据
        this.showData = function (data) {
            $(_this.gridBody).datagrid({
                data: data,
                resizable: true,
                auto: true,
                fitColumns: true,
                collapsible: true,
                scrollY: true,
                width: $(_this.gridDiv).width() - 5,
                height: ($("body").height() - $(_this.gridDiv).offset().top - 2),
                columns: [[
                        {
                            field: 'MItemID', title: '', width: 40, align: 'left', formatter: function (value, rec) {
                                //
                                return "<div class='fa-tp-checkbox' id='" + value + "' style='text-align:center'><input type='checkbox' mid='" + value + "' mnumber='" + rec.MNumber + (rec.MIsSys ? "' disabled = 'disabled' " : "'") + "></div>";
                            }
                        },
                        {
                            field: 'MNumber', width: 80, title: HtmlLang.Write(LangModule.FA, "FixAssetTypeNumber", "类别编码"), align: 'left'
                        },
                        {
                            field: 'MName', width: 80, title: HtmlLang.Write(LangModule.FA, "FixAssetTypeName", "类别名称"), align: 'left'
                        },
                        {
                            field: 'MDepreciationTypeID', width: 80, title: HtmlLang.Write(LangModule.FA, "DepreciationType", "折旧方法"), align: 'center', formatter: function (data, rec) {
                                return _this.home.getDepreciationTypeArray().where('x.value == "' + data + '"')[0].name;
                            }
                        },
                        {
                            field: 'MDepreciationFromCurrentPeriod', width: 80, title: HtmlLang.Write(LangModule.FA, "DepreciationFromPeriod", "折旧开始期间"), align: 'center', formatter: function (data, rec) {
                                return data ? _this.home.getDepreciationFromArray()[1].name : _this.home.getDepreciationFromArray()[0].name;
                            }
                        },
                        {
                            field: 'MUsefulYears', width: 40, title: HtmlLang.Write(LangModule.FA, "UsefulYears", "使用年数"), align: 'center'
                        },
                        {
                            field: 'MUsefulPeriods', hidden: true, width: 40, title: HtmlLang.Write(LangModule.FA, "UsefulPeriods", "使用期数"), align: 'center'
                        },
                        {
                            field: 'MRateOfSalvage', width: 40, align: 'right', title: HtmlLang.Write(LangModule.FA, "RateOfSalvage", "残值率"), formatter: function (data, rec) {
                                return data + "%";
                            }
                        },
                        {
                            field: 'MFixAccountFullName', width: 80, title: HtmlLang.Write(LangModule.FA, "FixAccountName", "固定资产科目"), align: 'left'
                        },
                        {
                            field: 'MDepAccountFullName', width: 80, title: HtmlLang.Write(LangModule.FA, "DepreciationAccountName", "累计折旧科目"), align: 'left'
                        },
                        {
                            field: 'MRemark', width: 80, title: HtmlLang.Write(LangModule.Common, "Description", "描述"), align: 'left'
                        },
                        {
                            field: 'Operation', width: 40, title: HtmlLang.Write(LangModule.Common, "Operation", "操作"), align: 'left', formatter: function (val, rec, rowIndex) {
                                var text = '<div class="list-item-action">';
                                text += '<a href="javascript:void(0)" style="margin-right:10px" class="list-item-edit ' + _this.editTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>';
                                //如果是系统预设的则不能删除
                                text += rec.MIsSys ? '' : '<a href="javascript:void(0)" class="m-icon-delete-row ' + _this.removeTableButton.trimStart('.') + '" rowindex="' + rowIndex + '" mid="' + rec.MItemID + '" mnumber="' + rec.MNumber + '">&nbsp;</a>';
                                text += '</div>';
                                return text;
                            }
                        }
                    ]],
                onClickRow: function () {
                    return false;
                },
                onLoadSuccess: function () {
                    //如果没有数据则添加一个空行，供用户编辑
                    if ((data && data.length > 0)) {
                        _this.resizeTableHeight();
                        _this.initTableEvent();
                    }
                }
            });
        };
    }
    //初始化
    FAFixAssetsTypeHome.prototype.init = function () {
        this.home = new FAHome();
        this.initEvent();
        this.loadData();
    };
    //表格高度重新计算
    FAFixAssetsTypeHome.prototype.resizeTableHeight = function () {
    };
    //编辑资产类型
    FAFixAssetsTypeHome.prototype.editFixAssetsType = function (typeId) {
        this.home.showEditFixAssetsType(typeId, this.loadData);
    };
    return FAFixAssetsTypeHome;
}());
//# sourceMappingURL=FAFixAssetsTypeHome.js.map