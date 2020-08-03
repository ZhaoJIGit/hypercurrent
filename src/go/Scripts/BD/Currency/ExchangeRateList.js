//扩展 easyui 的editCell方法
$.extend($.fn.datagrid.methods, {
    editCell: function (jq, param) {
        return jq.each(function () {
            var opts = $(this).datagrid('options');
            var fields = $(this).datagrid('getColumnFields', true).concat($(this).datagrid('getColumnFields'));
            for (var i = 0; i < fields.length - 1; i++) {
                var col = $(this).datagrid('getColumnOption', fields[i]);
                col.editor1 = col.editor;
                if (fields[i] != param.field) {
                    col.editor = null;
                }
            }
            try {
                //正常操作
                $(param.selector).datagrid('beginEdit', param.index);
                for (var i = 0; i < fields.length - 1; i++) {
                    var col = $(this).datagrid('getColumnOption', fields[i]);
                    col.editor = col.editor1;
                }
            }
            catch (e) {
                //碰到异常列的属性需要还原
                for (var i = 0; i < fields.length - 1; i++) {
                    var col = $(this).datagrid('getColumnOption', fields[i]);
                    col.editor = col.editor1;
                }
            }

        });
    }
});


//币种汇率显示
var ExchangeRateList = {
    init: function () {
        $("#btnNewExchange").click(function () {
            var title = HtmlLang.Write(LangModule.BD, "NewExchange", "New exchange rate");
            ExchangeRateList.openCurrenyEditDialog(null, title);
        });
    },
    //默认显示常用币种列表第一列的数据
    showExchangeRateByRowIndex: function (index, rowData) {
        //获取本位币
        var currencyId = CurrencyList.GetCurrencyID();
        //获取选中行的币种 
        var targetCurrencyId = rowData ? rowData.MCurrencyID : "";
        //保存参数
        ExchangeRateList.currencySelectedIndex = index;
        //如果没有传过来数据
        if (!rowData) {
            //修改表头 
            $("#dvCurrencyRates").text(HtmlLang.Write(LangModule.Acct, "ExchangeRate", "Exchange Rate"));
            //显示 
            ExchangeRateList.showExchangeRate(currencyId, targetCurrencyId, "");
        }
        else {
            //修改表头 
            $("#dvCurrencyRates").text(HtmlLang.Write(LangModule.Acct, "ExchangeRate", "Exchange Rate") + ":" + rowData.MCurrencyID + " " + rowData.MName);
            //显示 
            ExchangeRateList.showExchangeRate(currencyId, targetCurrencyId, rowData.MName);
        }
    },
    //显示某一行的数据
    showExchangeRate: function (sourceCurrencyId, targetCurrencyId, targetCurrencyName) {
        //组装查询参数

        $("#flag").next(".datagrid").remove();

        $("#flag").after('<div id="tbCurrencyExchangeRate" fitcolumns="true"></div>');

        var param = { MSourceCurrencyID: sourceCurrencyId, MTargetCurrencyID: targetCurrencyId };
        //组装url
        var url = "/BD/ExchangeRate/GetBDExchangeRateList";
        //回调函数
        var callback = function () { };
        //单位
        var unitPer = 1 + sourceCurrencyId + " = **" + HtmlLang.Write(LangModule.BD, "ForeignCurrency", "Foreign Currency");
        //单位
        var perUnit = 1 + HtmlLang.Write(LangModule.BD, "ForeignCurrency", "Foreign Currency") + " = " + "**" + sourceCurrencyId;
        //汇率精确度
        var precision = CurrencyList.precision;
        //列头模型
        var colHeaderModel = [
            //外币名称
             {
                 title: HtmlLang.Write(LangModule.BD, "ForeignCurrency", "Foreign Currency"), field: "MTargetCurrencyName", width: 100, align: 'center', sortable: false
             },
             //正汇率
             {
                 title: unitPer, field: 'MUserRate', width: 120, align: 'right', sortable: true, formatter: function (value, rowData, rowIndex) {
                     return Megi.Math.toDecimal(rowData.MUserRate, precision);
                 }

             },
             //反汇率
             {
                 title: perUnit, field: 'MRate', width: 120, align: 'right', formatter: function (value, rowData, rowIndex) {
                     var rate = Megi.Math.toDecimal(value, precision);
                     return (rate && rate != 0) ? rate : (1 / rowData.MUserRate).toFixed(6);
                 }
             },
             //生效日期
             {
                 title: HtmlLang.Write(LangModule.Acct, "EffectiveDate", "Effective Date"), align: 'center', field: 'MRateDate', width: 100, sortable: true, formatter: $.mDate.formatter
             },
             //操作
             {
                 title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), align: 'center', field: 'MItemID', width: 100, sortable: false, formatter: function (value, rowData, rowIndex) {
                     if (CurrencyList.hasChangeAuth) {
                         return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"ExchangeRateList.editExchangeRate('" + rowData.MItemID + "','" + rowIndex + "');" + "\" class='list-item-edit'></a><a href='javascript:void(0);' onclick=\"ExchangeRateList.removeExchangeRate('" + rowIndex + "','" + value + "');\" class='list-item-del'></a></div>";
                     }
                 }
             }
        ];
        //列模型
        var colModel = [colHeaderModel];
        //绑定表格数据
        Megi.grid('#tbCurrencyExchangeRate', {
            //显示行数
            //rownumbers: true,
            //可伸展
            resizable: false,
            //单cell选择
            singleSelect: true,
            //auto
            auto: false,
            //url
            url: url,
            sortName: 'MRateDate',
            sortOrder: 'desc',
            //param
            queryParams: param,
            //data model
            columns: colModel,

            //第一个汇率可编辑
            //onClickCell: ExchangeRateList.exchangeRateCellClick,
            //分页
            pagination: true,
            onAfterEdit: function (rowIndex, rowData, changes) {

                //如果可编辑的列为MUserRate

                if (ExchangeRateList.exchangeRateEditField == "MUserRate") {
                    var userRate = Megi.Math.toDecimal(rowData.MUserRate, precision);
                    if (userRate && userRate != 0) {
                        rowData.MRate = Megi.Math.toDecimal(1 / userRate, precision);
                    }
                }

                if (ExchangeRateList.exchangeRateEditField == "MRate") {
                    var rate = Megi.Math.toDecimal(rowData.MRate, precision);
                    if (rate && rate != 0) {
                        rowData.MUserRate = Megi.Math.toDecimal(1 / rate, precision);
                    }
                }
                var param = {};
                param.index = rowIndex;
                param.row = rowData;
                Megi.grid("#tbCurrencyExchangeRate", "updateRow", param);
            }
        });
    },
    editExchangeRate: function (id) {
        ExchangeRateList.openCurrenyEditDialog(id);
    },

    openCurrenyEditDialog: function (exchangeId, dialogTitle) {
        //先判断是否中了外币
        var row = $("#tbCurrency").datagrid("getSelected");

        //如果没选择提示用户选择
        if (!row) {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "NotSelectAnyCurrenry", "Please select a currency"));
            return;
        }
        //获取rowIndex
        var rowIndex = $("#tbCurrency").datagrid("getRowIndex", row);

        var url = '/BD/Currency/AddCurrency?currencyId=' + row.MCurrencyID + "&rowIndex=" + rowIndex + "&MItemID=" + row.MItemID + "";
        if (exchangeId) {
            url += "&exchangeRateId=" + exchangeId;
        }
        Megi.dialog({
            title: dialogTitle ? dialogTitle : HtmlLang.Write(LangModule.BD, "EditCurrency", "Eidt Currency"),
            width: 400,
            height: 330,
            href: url
        });
    },

    //保存用户修改或者新增的汇率
    saveExchangeRate: function (rowIndex, MItemID) {
        //结束行编辑
        ExchangeRateList.exchangeRateEndEditing();
        //设置本行选中
        Megi.grid("#tbCurrencyExchangeRate", "selectRow", rowIndex);
        //获取选中的行
        var row = Megi.grid("#tbCurrencyExchangeRate", "getSelected");
        //获取生效日期值
        var date = row.MRateDate;
        //获取正汇率
        var rate = row.MUserRate;
        //如果用户没有输入
        if (rate == undefined || rate.length == 0 || rate == 0) {
            //弹出提醒
            $.mDialog.alert(HtmlLang.Write(LangModule.Acct, "InputExchangeRate", "Please Input ExchangeRate"));
            //返回
            return false;
        }
        //验证用户的汇率是否大于0
        if (rate < 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "addCurrencyExchangeRate", "The value of the currency must be greater than zero"));
            return false;
        }
        //新增逻辑
        var url = "/BD/ExchangeRate/AddBDExchangeRate";
        //参数
        var param = {
            MItemID: row.MItemID,
            MRateDate: date,
            MUserRate: rate,
            MTargetCurrencyID: row.MTargetCurrencyID,
            MSourceCurrencyID: row.MSourceCurrencyID,
            RowIndex: rowIndex
        };

        //修改不进行判断
        if (param.MItemID) {
            ExchangeRateList.updateExchangeRate(url, param);
        } else {
            //检测该汇率是否存在同一天的记录
            mAjax.post("/BD/ExchangeRate/CheckExchangeRateIsExist", { model: param }, function (msg) {
                if (msg.Success) {
                    $.mDialog.max();
                    var content = HtmlLang.Write(LangModule.Acct, "ExchangeRatealreadyexistingatdate", "Exchange Rate already existing at " + $.mDate.format(param.MRateDate) + ",Are you sure to update");
                    $.mDialog.confirm(content, {
                        callback: function () {
                            //加上更新标志
                            url += "?isUpdate=true";
                            ExchangeRateList.updateExchangeRate(url, param);
                        }
                    });
                } else {
                    ExchangeRateList.updateExchangeRate(url, param);
                }
            });
        }
    },

    updateExchangeRate: function (url, param) {
        //回调函数
        var callback = function (msg) {
            //提醒用户
            //常用币种表更新

            if (msg && msg.Success) {
                $.mMsg(HtmlLang.Write(LangModule.Acct, "OperationSuccess", "Operation Success"));
                parent.CurrencyList.bindGrid(param.RowIndex);
            } else {
                if (msg && msg.Message) {
                    $.mDialog.alert(msg.Message);
                } else {
                    var tips = HtmlLang.Write(LangModule.BD, "OperationFail", "Operation Fail");

                    $.mDialog.alert(tips);
                }
            }


        };
        //异步提交
        mAjax.submit(url, { model: param }, callback);
    },

    //删除用户新增的汇率
    removeExchangeRate: function (rowIndex, MItemID) {
        //设置本行选中
        Megi.grid("#tbCurrencyExchangeRate", "selectRow", rowIndex);
        //获取选中的行
        var row = Megi.grid("#tbCurrencyExchangeRate", "getSelected");
        //如果是第一行，则是清空用户输入，还原为系统默认值
        //if (rowIndex == 0) {
        //    //清空逻辑
        //    Megi.grid("#tbCurrencyExchangeRate", "updateRow", {
        //        index: rowIndex,
        //        row: {
        //            "MUserRate": row.MRate,
        //            "MRateDate": ""
        //        }
        //    });
        //    //返回
        //    return true;
        //}
        //提醒用户是否删除
        var delectConfirm = LangKey.AreYouSureToDelete;
        //弹出确认框
        $.mDialog.confirm(delectConfirm, {
            callback: function () {
                //删除逻辑
                var url = "/BD/ExchangeRate/RemoveBDExchangeRate";
                //参数
                var param = {
                    MItemID: row.MItemID,
                    MSourceCurrencyID: row.MSourceCurrencyID
                };
                //回调函数
                var callback = function (msg) {
                    //提醒用户

                    //删除本行
                    Megi.grid("#tbCurrencyExchangeRate", "deleteRow", rowIndex);
                    $.mMsg(HtmlLang.Write(LangModule.BD, "addCurrencyExchangeRateSucces", "删除货币汇率成功"));
                    //常用币种表更新
                    parent.CurrencyList.bindGrid(ExchangeRateList.currencySelectedIndex);
                };
                //异步提交
                mAjax.submit(url, { model: param }, callback);
            }
        });
    },
    //可编辑
    exchangeRateCellClick: function (index, field) {
        //结束其他的编辑
        if (ExchangeRateList.exchangeRateEndEditing()) {
            //选中行并且编辑格子
            var row = Megi.grid('#tbCurrencyExchangeRate', 'selectRow', index);
            //编辑
            Megi.grid(row, 'editCell', { index: index, field: field, selector: '#tbCurrencyExchangeRate' });
            //Megi.grid(row, "beginEdit", index);
            //下标
            ExchangeRateList.exchangeRateEditIndex = index;
            ExchangeRateList.exchangeRateEditField = field;
        }
    },
    //编辑完成
    exchangeRateEndEditing: function () {
        //如果是第一次编辑
        if (ExchangeRateList.exchangeRateEditIndex == undefined) {
            return true;
        }


        //先校验格式
        if (Megi.grid('#tbCurrencyExchangeRate', 'validateRow', ExchangeRateList.exchangeRateEditIndex)) {

            //结束编辑状态
            Megi.grid('#tbCurrencyExchangeRate', 'endEdit', ExchangeRateList.exchangeRateEditIndex);
            //重置
            ExchangeRateList.exchangeRateEditIndex = undefined;
            //返回
            return true;
        } else {
            //失败
            return false;
        }
    },
    //编辑的行
    exchangeRateEditIndex: undefined,
    //编辑的列
    exchangeRateEditField: undefined,
    //从常用币种传过来的选中行
    currencySelectedIndex: undefined

}

$(document).ready(function () {
    ExchangeRateList.init();
});