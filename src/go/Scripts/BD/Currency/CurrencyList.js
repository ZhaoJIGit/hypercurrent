/// <reference path="D:\Development\1.Megi\Megi\Code\JieNor.Megi.Go\JieNor.Megi.Go.Web\intellisense/JieNor.Megi.Common.0x0009.js" />
//常用币种管理对象
var CurrencyList = {
    //是否有数据变更权限
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    //本位币ID
    GetCurrencyID: function () { return $("#hidCurrency").val(); },
    //获取当前组织代码
    GetOrgID: function () { return $("#hidOrgID").val(); },
    //初始化函数
    init: function () {
        //初始化事件
        CurrencyList.bindAction();
        //常用币种展示
        CurrencyList.bindGrid(0);
    },
    //展示常用币种
    bindGrid: function (selIndex) {
        //组织代码
        var orgID = CurrencyList.GetOrgID();
        //本位币
        var currencyId = CurrencyList.GetCurrencyID();
        //请求URL
        var url = "/BD/ExchangeRate/GetCurrencyList";
        //参数
        var param = { MSourceCurrencyID: currencyId };
        //单位
        var unitPer = 1 + currencyId + " = **" + HtmlLang.Write(LangModule.BD, "ForeignCurrency", "Foreign Currency");
        //单位
        var perUnit = 1 + HtmlLang.Write(LangModule.BD, "ForeignCurrency", "Foreign Currency") + " = " + "**" + currencyId;
        //汇率精确度
        var precision = CurrencyList.precision;
        //列头模型
        var colHeaderModel = [
            //常用币种
             {
                 title: HtmlLang.Write(LangModule.Acct, "Currency", "Currency"), field: 'MName', width: 100, align: 'center', sortable: true
             },
             //正汇率
             {
                 title: unitPer, field: 'MUserRate', width: 120, align: 'right', sortable: true, formatter: function (value, rowData, rowIndex) {
                     var userRate = Megi.Math.toDecimal(value, precision);
                     return (userRate && userRate != 0) ? userRate : "";
                 }
             },
             //反汇率
             {
                 title: perUnit, field: 'MRate', width: 120, align: 'right', sortable: true, formatter: function (value, rowData, rowIndex) {
                     var rate = Megi.Math.toDecimal(value, precision);
                     if (!rate) {
                         return "";
                     }
                     return (rate && rate != 0) ? rate : (1 / rowData.MUserRate).toFixed(6);
                 }
             },
             //生效日期
             {
                 title: HtmlLang.Write(LangModule.Acct, "EffectiveDate", "EffectiveDate"), align: 'center', field: 'MRateDate', width: 100, sortable: true, formatter: $.mDate.formatter
             },
             //操作
             {
                 title: HtmlLang.Write(LangModule.BD, "Operation", "Operation"), align: 'center', field: 'MItemID', width: 100, sortable: false, formatter: function (value, rowData, rowIndex) {
                     //rowData.MCurrencyID
                     if (CurrencyList.hasChangeAuth) {
                         return "<div class='list-item-action'><a href='javascript:void(0);' onclick=\"CurrencyList.removeCurrency('" + rowData.MItemID + "'," + rowIndex + ");\" class='list-item-del'></a></div>";
                     }
                 }
             }
        ];
        //列模型
        var colModel = [
            colHeaderModel
        ];
        //绑定表格数据
        Megi.grid('#tbCurrency', {
            //显示行数
            //rownumbers: true,
            //可伸展
            resizable: false,
            //单cell选择
            singleSelect: false,
            //auto
            auto: true,
            fitColumns: true,
            //url
            url: "/BD/Currency/GetBDCurrencyList",
            //param
            queryParams: param,
            //row click event
            onClickRow: CurrencyList.onCurrencyListClickRow,
            //onLoadSuccess
            onLoadSuccess: function (data) {
                //选中某行
                CurrencyList.onLoadCurrencyListSuccess(data, selIndex);
            },
            //data model
            columns: colModel
        });
    },
    onCurrencyListClickRow: function (rowIndex, rowData) {
        //调用币种汇率方法
        Megi.grid('#tbCurrency', "unselectAll");
        Megi.grid('#tbCurrency', "selectRow", rowIndex);
        ExchangeRateList.showExchangeRateByRowIndex(rowIndex, rowData);
    },
    //数据加载完成后调用的方法
    onLoadCurrencyListSuccess: function (data, selIndex) {
        //保存一份到当前列表
        CurrencyList.currencyList = data;
        //第一行选中
        Megi.grid('#tbCurrency', "selectRow", selIndex);
        //获取选中的行
        var row = Megi.grid("#tbCurrency", "getSelected");

        Megi.grid("#tbCurrency", "resize");
        //对应汇率表刷新
        CurrencyList.onCurrencyListClickRow(selIndex, row);
    },
    //删除常用币种的方法
    removeCurrency: function (itemID, rowIndex) {


        //itemID: 货币的ID
        //rowIndex :行数
        //删除确认信息 Are you sure to delete
        var delectConfirm = LangKey.AreYouSureToDelete;
        //弹出确认框
        $.mDialog.confirm(delectConfirm, {
            callback: function () {
                //异步删除的url
                var url = "/BD/Currency/RemoveBDCurrency";
                //参数
                var param = { MItemID: itemID };
                //回调函数
                var callback = function (msg) {
                    //是否正常
                    if (msg.Success) {
                        //移除本行
                        Megi.grid('#tbCurrency', "deleteRow", rowIndex);
                        //获取第一个行
                        CurrencyList.bindGrid(0);
                    }
                    else {
                        var currencyquote = HtmlLang.Write(LangModule.BD, "Thecurrencyhasbeenquoted", "该币别已经被引用")
                        $.mDialog.alert(currencyquote);
                    }

                };
                //异步提交
                mAjax.submit(url, { model: param }, callback);
            }
        });
    },
    //绑定事件 新增货币按钮
    bindAction: function () {
        $("#btnNewCurrency").click(function () {
            Megi.dialog({
                title: HtmlLang.Write(LangModule.BD, "NewCurrency", "New Currency"),
                width: 400,
                height: 330,
                href: '/BD/Currency/AddCurrency/'
            });
        });
    },
    currencyList: [],
    //精度
    precision: 6
}

$(document).ready(function () {
    CurrencyList.init();
});
