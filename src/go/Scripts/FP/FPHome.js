

(function () {

    var FPHome = (function () {

        var FPHome = function (fapiaoIndex, invoiceType) {
            //
            var BeginDate = $("#hideBeginDate").val();
            //默认是进入主页
            fapiaoIndex = fapiaoIndex || 0;
            //默认是销售列表
            invoiceType = invoiceType || 0;
            //
            var fapiaoButton = ".fp-fapiao-btn";
            //
            var invoiceButton = ".fp-invoice-btn";
            //
            var fapiaoDiv = ".fp-fapiao-div";
            //
            var invoiceDiv = ".fp-invoice-div";

            var lockViewFapiao = false;

            var _fapiao = null;
            //
            var newTable = "#aNewTable";
            //
            var fastNewTable = "#aFastNewTable";
            //
            var manualNewTable = "#aManualNewTable";

            //查看发票的界面
            var viewFapiaoUrl = "/FP/FPHome/FPViewFapiao";
            //
            var tableBaseInfo = {};
            //
            var that = this;
            //点击美记Fapiao 和销售采购单切换的事件
            this.initEvent = function () {
                //
                $(fapiaoButton).off("click").on("click", function () {
                    //
                    $(this).addClass("current").siblings().removeClass("current");
                    //显示美记Fapiao主页
                    $(fapiaoDiv).show().siblings().hide();

                    if (_fapiao == null) {

                        _fapiao = new FPFapiao(fapiaoIndex, invoiceType);
                        _fapiao.init();
                    }
                    else {
                        _fapiao.showTab();
                    }
                });
                //
                $(invoiceButton).off("click").on("click", function () {
                    //
                    $(this).addClass("current").siblings().removeClass("current");
                    //显示美记Fapiao主页
                    $(invoiceDiv).show().siblings().hide();
                    //
                    new FPInvoice(invoiceType).init();
                });
                //快速新增开票单
                $(newTable + "," + fastNewTable).off("click").on("click", function () {
                    //
                    that.editTable("", function () {
                        //
                        mWindow.reload();
                    });
                });

            };

            //查看发票
            this.viewFapiao = function (fapiaoID, callback) {

                lockViewFapiao = true;
                //
                top.mDialog.show({
                    mShowbg: true,
                    mDrag: "mBoxTitle",
                    mContent: "iframe:" + viewFapiaoUrl + "?fapiaoID=" + fapiaoID,
                    mCloseCallback: function () {
                        $.isFunction(callback) && callback();
                        lockViewFapiao = false;
                    },
                    mWidth: 1090,
                    mHeight: 600,
                    mShowTitle: false,
                    mMax: false
                });
            };

            //编辑开票单
            this.editTable = function (tableID, mCloseCallback) {
                //
                var title = HtmlLang.Write(LangModule.FP, "EditSaleTable", "编辑销售开票单");
                //
                if (invoiceType == 1) {
                    //
                    title = HtmlLang.Write(LangModule.FP, "EditPurchaseTable", "编辑采购开票单");
                }
                //
                mDialog.show({
                    mTitle: title,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + FPHome.editTableUrl + "?tableId=" + tableID + "&invoiceType=" + invoiceType,
                    mCloseCallback: mCloseCallback
                });
                $.mDialog.max();
            };

            //初始化
            this.init = function () {
                //
                that.initEvent();
                //
                new FPFapiao(fapiaoIndex, invoiceType).init();
                //更新模板数
                //that.getTabeBaseInfoData();
            };
        };

        //返回
        return FPHome;
    })();
    //
    $.extend(FPHome, {
        //常量的定义
        editTableUrl: "/FP/FPHome/FPEditTable",
        pageSize: 20,
        pageList: [10, 20, 50, 100, 200],
        //把001这样的数字完全转化成0001这样四位数的
        GetNumber: function (number) {
            //
            if (!number) {
                //
                return "";
            }
            //
            return ("0000".substring(0, 4 - number.toString().length) + number.toString());
        },
        //把开票单的编号转化 相应的号码
        GetFullTableNumber: function (number, type) {
            //
            if (!number) {
                //
                return "";
            }
            //
            var prefix = (+type) == 0 ? "SFP" : "PFP";
            //
            if (number instanceof Array) {
                //
                var result = "";
                //
                for (var i = 0; i < number.length ; i++) {

                    //
                    result += prefix + ("0000".substring(0, 4 - number[i].toString().length) + number[i].toString()) + ",";
                }
                //
                return result.trimEnd(",");
            }
            //
            return prefix + ("0000".substring(0, 4 - number.toString().length) + number.toString());
        },
        //
        GetTableNumber: function (number) {
            //
            if (number && number.length > 0) {
                //
                return number.toString().replace("SFP", "").replace("PFP", "");
            }
            //
            return number;
        },
        //
        ParseDate2YearMonth: function (date) {
            //
            var date = mDate.parse(date);
            //
            if (date && date.getFullYear() > 1901) {
                //
                return date.getFullYear() + "-" + (date.getMonth() + 1);
            }
            //
            return "";
        },
        //
        ParseYearMonth2Date: function (dateString) {
            //
            if (dateString) {
                //
                var date = mDate.parse(dateString + "-01");
                //
                return date;
            }
            //
            return "";
        },
        //切换美记发票和销售采购
        SwitchFapiaoInvoice: function (type) {
            //
            if (!type) {
                //
                $(".fp-fapiao-btn").trigger("click");
            }
            else {
                $(".fp-invoice-btn").trigger("click");
            }
        },
        //获取表格中勾选中的那些行
        GetGridSelectedCheckbox: function (tableBody, attrNames) {
            //
            var view = $(tableBody).datagrid("getPanel");
            //
            var table = $(".datagrid-body > table", view);
            //获取里面的行
            var rows = $(".datagrid-row", table);
            //
            var attrArray = [];
            //
            $("input[type='checkbox']", rows).each(function (index) {
                //
                if ($(this).attr('checked') == "checked") {
                    //
                    var obj = { target: $(this), index: index };
                    //
                    for (var i = 0 ; i < attrNames.length ; i++) {
                        //
                        if (attrNames[i].attrName && attrNames[i].fieldName) {
                            //
                            obj[attrNames[i].fieldName] = $(this).attr(attrNames[i].attrName);
                        }
                    }
                    //
                    attrArray.push(obj);
                }
            });
            //
            return attrArray;
        },
        //
        //获取所在的Index
        GetGridRowIndexByCellItem: function (tableBody, item) {
            //
            var view = $(tableBody).datagrid("getPanel");
            //
            var table = $(".datagrid-body > table", view);
            //获取里面的行
            var rows = $(".datagrid-row", table);
            //
            for (var i = 0; i < rows.length; i++) {
                //
                if ($.contains(rows[i], item[0])) {
                    //
                    return i;
                }
            }
            //
            return false;
        },
        //初始化发票类型选择框
        InitFapiaoTypeCombobx: function (selector, onSelect, onLoadSuccess, defaultValue, required, needAll) {
            //
            defaultValue = defaultValue || -1;
            //
            var data = [
                 {
                     id: -1,
                     name: HtmlLang.Write(LangModule.FP, "AllFapiaoType", "全部发票类型")
                 },
                {
                    id: 0,
                    name: HtmlLang.Write(LangModule.FP, "NormalFapiao", "增值税普通发票")
                },
                {
                    id: 1,
                    name: HtmlLang.Write(LangModule.FP, "SpecialFapiao", "增值税专用发票")
                }];
            //
            if (needAll !== true) {
                //
                data = data.slice(1);
            }
            //
            $(selector).combobox({
                textField: "name",
                valueField: "id",
                data: data,
                width: $(selector).width(),
                onSelect: onSelect || function () { },
                required: required !== false ? true : false,
                onLoadSuccess: function () {
                    //
                    $(selector).combobox("setValue", defaultValue);
                    //
                    $.isFunction(onLoadSuccess) && onLoadSuccess();
                }
            });
        },
        //判断一个数值是否超出了范围
        IsOuterOfRange: function (src, cmp) {
            //
            return src >= 0 ? src < cmp : Math.abs(src) < Math.abs(cmp);
        },
        //发票状态
        FapiaoStatus: [
                {
                    id: "1",
                    name: HtmlLang.Write(LangModule.FP, "NormalFapiao", "普通发票")
                },
                {
                    id: "-1",
                    name: HtmlLang.Write(LangModule.FP, "CreditFapiao", "红字发票")
                }],
        //初始化发票状态  0作废 -1红字 1正常  
        InitFapiaoStatusCombobox: function (selector, onSelect, onLoadSuccess, defaultValue) {
            //默认为普通发票
            defaultValue = defaultValue == undefined ? 1 : defaultValue;
            //
            var data = FPHome.FapiaoStatus;
            //
            $(selector).combobox({
                textField: "name",
                valueField: "id",
                data: data,
                required: true,
                width: $(selector).width(),
                onSelect: onSelect,
                onLoadSuccess: function () {
                    //
                    $(selector).combobox("setValue", defaultValue);
                    //
                    $.isFunction(onLoadSuccess) && onLoadSuccess();
                }
            });
        },
        //校验某一行是否合法
        ValidateEditRow: function (tableBody, index) {
            //
            if (index != null) {
                //开始编辑之前的操作，获取所有的编辑器
                var editors = $(tableBody).datagrid("getEditors", index);
                //每一个编辑器都要校验
                for (var i = 0; i < editors.length; i++) {
                    //当前编辑器
                    var editor = editors[i];
                    //如果是easyui的控件
                    if ($(editor.target).mIsEasyUIControl()) {
                        //
                        if (!$(editor.target).mValidateEasyUI()) {
                            return false;
                        }
                    }
                    else if ($(editor.target).validatebox) {
                        //如果不是easyui组件，但是是validatebox组装件，则调用本身的validatebox方法
                        if (!$(editor.target).validatebox("isValid")) {
                            return false;
                        }
                    }
                }
            }
            return true;
        },
        //结束编辑，并且跳转到下一行
        EndEditAndGoToNextRow: function (tableBody, index, emptyRow) {
            //
            if (FPHome.ValidateEditRow(tableBody, index)) {
                //
                $(tableBody).datagrid("endEdit", index);
                //如果下面有一行
                if ($(tableBody).datagrid("getRows").length == (index + 1)) {
                    //插入到数据中
                    $(tableBody).datagrid("insertRow", { index: index + 1, row: emptyRow });
                }
                //下一行开始编辑
                $(tableBody).datagrid("beginEdit", index + 1);
                //
                return index + 1;
            }
            //
            return null;
        },
        //获取所有有效的行 正在编辑的那一行不能算
        GetGridRowsWithData: function (tableBody, editIndex, filter) {
            //
            var rows = $(tableBody).datagrid("getRows");
            //如果需要剔除编辑的行，则作如下处理
            //
            var newRows = [];
            //
            for (var i = 0; i < rows.length ; i++) {
                //
                if (i !== editIndex) {
                    //
                    newRows.push(rows[i]);
                }
            }
            //
            if (filter && filter.length > 0) {
                //
                return newRows.where(filter);
            }
            //返回
            return newRows;
        },
        TaxRates: [],
        //
        GetTaxRates: function () {
            //
            if (FPHome.TaxRates.length == 0) {
                //
                mAjax.post("/BD/TaxRate/GetTaxRateList", {}, function (data) {
                    //
                    FPHome.TaxRates = data;
                    //
                }, "", false, false);
            }
            else {
                return FPHome.TaxRates;
            }
        },
        EnterKey: 1300,
        //是否有商品
        Items: [],
        //这个需要做一个同步请求，必须保证所有的item都返回来了
        GetItemList: function () {
            //
            if (FPHome.Items.length == 0) {
                //
                mAjax.post("/BD/Item/GetItemList", { filter: { IncludeDisable: true } }, function (data) {
                    //
                    FPHome.Items = data;
                }, "", false, false);
            }
            else {
                return FPHome.Items;
            }
        },
        InitContact: function (selector, value, contactType, onSelect) {
            //
            $(selector).mAddCombobox("contact", {
                required: true,
                valueField: 'MItemID',
                textField: 'MContactName',
                onSelect: onSelect,
                mode: "remote",
                url: '/BD/Contacts/GetContactsListByContactType?contactType=' + contactType,
                onLoadSuccess: function () {
                    //
                    !!value && $(selector).combobox("setValue", value);
                }
            },
            {
                hasPermission: true,
                url: '/BD/Contacts/ContactsEdit?contactType=' + contactType
            });
        }
    });
    //
    window.FPHome = FPHome;
})()

$(document).ready(function () {
    FPHome.GetItemList();
    //
    FPHome.GetTaxRates();
});