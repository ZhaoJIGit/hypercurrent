/*常量定义*/

//可弹出新增功能的combobox的公共央视
var addComboboxAttrName = "add-combobox-type";
//保存添加url的属性名称
var addComboboxOptionsAtrrName = "add-options";

//可下拉可编辑combobox的数据项目
var addComboboxDataOptions = {
    //银行
    "bank": {

    },
    //银行帐号
    "bankaccount": {

    },
    //联系人
    "contact": {
        url: '',
        mode: 'local',
        //后台 有ComboboxMaxCount 控制，前端做这个多余了
        //ComboboxMaxCount: 20,
        valueField: 'MItemID',
        textField: 'MName',
        hideItemKey: 'MIsActive',
        hideItemValue: false,
    },
    //科目
    "account": {
        url: '',
        valueField: 'MItemID',
        textField: 'MFullName',
        hideItemKey: "MIsActive",
        hideItemValue: false,
    },
    //员工
    "employee": {
        url: '/BD/Employees/GetEmployees?isDefaultOrg=true&includeDisable=true',
        valueField: 'MItemID',
        textField: 'MFullName',
        hideItemKey: "MIsActive",
        hideItemValue: false,
    },
    //inventory items
    "inventory": {
        url: "/BD/Item/GetItemList",
        queryParams: { IncludeDisable: true },
        valueField: "MItemID",
        textField: "MText",
        hideItemKey: 'MIsActive',
        hideItemValue: false
    },
    //expense items
    "expense": {
        url: "/BD/ExpenseItem/GetExpenseItemListByTier?includeDisable=true",
        valueField: "MItemID",
        textField: "MText",
        groupField: "MGroupName",
        hideItemKey: "MIsActive",
        hideItemValue: false,
        groupFormatter: function (group) {
            if (group != null && group != "") {
                return group;
            }
        },
    },
    //currency
    "currency": {
        url: "/BD/Currency/GetBDCurrencyList?isIncludeBase=true",
        valueField: "MCurrencyID",
        textField: "MCurrencyID"
    },
    //税率
    "taxrate": {
        url: " /BD/TaxRate/GetTaxRateList",
        valueField: "MItemID",
        textField: "MText",
    },
    "trackOption": {
        url: '/BD/Tracking/GetTrackOptionsById',
        valueField: 'MValue',
        textField: 'MName',
        hideItemKey: 'MValue1',
        hideItemValue: '0'
    },
    "voucherExplanation": {
        url: '/GL/GLVoucher/GetVoucherExplanationList',
        valueField: 'MItemID',
        textField: 'MContent'
    }
};

//银行
var addCombboxTypeOptions = {
    //科目
    "account": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "newaccount", "New Account"),
        url: "/BD/BDAccount/AccountEdit?id=&parentId=&parentName=&isCombox=true&Origin=Dropdown",
        width: 670,
        height: 500,
        showSearchBox: true,
        searchColumns: [
            {
                field: "MNumber",
                title: HtmlLang.Write(LangModule.Common, "AccountCode", "科目代码"),
                width: "20%",
            },
            {
                field: "MName",
                title: HtmlLang.Write(LangModule.Common, "AccountName", "科目名称"),
                width: "30%",
            },
            {
                field: "MFullName",
                title: HtmlLang.Write(LangModule.Common, "AccountfullName", "科目全称"),
                width: "40%",
            }
        ]
    },
    //添加银行
    "bank": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "newbanktype", "New Bank Type"),
        url: "/BD/BDBank/BDBankTypeEdit",
        width: 350,
        height: 280
    },
    //添加银行帐号
    "bankaccount": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "newbankaccount", "New Bank Account"),
        url: "/BD/BDBank/BDBankAccountEdit",
        width: 450,
        height: 420
    },
    //添加联系人
    "contact": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "NewContact", "New Contact"),
        url: "/BD/Contacts/ContactsEdit",
        width: 1080,
        height: 450,
        isReload: true
    },
    //添加员工
    "employee": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "NewEmployee", "New Employee"),
        url: "/BD/Employees/EmployeesEdit",
        width: 1100,
        height: 450
    },
    //添加inventory items
    "inventory": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "NewInventoryItem", "New Inventory Item"),
        url: '/BD/Item/ItemEdit',
        width: 620,
        height: 450
    },
    //添加Expense Items
    "expense": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "NewExpenseItem", "New Expense Item"),
        url: "/BD/ExpenseItem/ExpenseItemEdit",
        width: 520,
        height: 400
    },
    //添加外币
    "currency": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "NewCurrency", "新增货币"),
        url: "/BD/Currency/AddCurrency/",
        width: 400,
        height: 330
    },
    //添加税率
    "taxrate": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "NewTaxRate", "New Tax Rate"),
        url: '/BD/TaxRate/TaxRateEdit/',
        width: 620,
        height: 330
    },
    "trackOption": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "NewTrackOption", "New Tracking Option"),
        url: "/BD/Tracking/CategoryOptionEdit",
        width: 400,
        height: 400
    },
    //凭证的摘要
    "voucherExplanation": {
        itemTitle: HtmlLang.Write(LangModule.Common, "Management", "管理"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "ExplanationManagement", "摘要管理"),
        url: "/GL/GLVoucher/GLExplanationEdit",
        width: 600,
        height: 400
    },
    //过滤方案
    "filterScheme": {
        itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
        dialogTitle: HtmlLang.Write(LangModule.Common, "AddFilterScheme", "新增过滤方案"),
        url: "/GL/GLVoucher/GLExplanationEdit",
        width: 600,
        height: 420
    }
}
/*
    一个下拉框，里面除了包含下拉框的内容，还需要添加一个new Item的类似功能
    使用方式有三种
    1.$(selector).mAddCombobox() selector必须有 data-options/add-options的属性
    2.$(selector).mAddCombobox(dataoptions,addOptions)
    3.给selector添加add-combobox样式，并且具有data-options/add-options的属性

    add-options样例：add-options = 
*/
;
(function () {
    //第一层，闭包
    var mAddCombobox = (function () {
        //第二层闭包
        var mAddCombobox = function (selector, dataOptions, a) {
            var addOptions = $.extend(true, {}, a);
            //
            var that = this;
            //
            var searchLang = HtmlLang.Write(LangModule.Common, "AdvanceSearch", "高级搜索");
            //
            var indexLang = HtmlLang.Write(LangModule.Common, "Index", "序号");
            //item的title
            addOptions.itemTitle = addOptions.itemTitle || HtmlLang.Write(LangModule.Common, "newitem", "New Item");
            //弹出框的title
            addOptions.dialogTitle = addOptions.dialogTitle || addOptions.itemTitle;
            //
            this.init = function () {
                //
                var loadSuccess = dataOptions.onLoadSuccess;
                //如果有回调函数
                loadSuccess = $.isFunction(loadSuccess) ? loadSuccess : function () { };
                //
                var loadSuccessAdd = (addOptions.url && addOptions.hasPermission) ? that.loadSuccess : function () { };
                //添加一个loadsuccess事件
                dataOptions.onLoadSuccess = function () {
                    //
                    try {
                        loadSuccess($(selector).combobox("getData"));
                    } catch (msg) {

                    }

                    //
                    loadSuccessAdd();
                };
                //如果有data这置空url 
                if (dataOptions.data) {
                    //置空，已现有数据为准
                    dataOptions.url = null;
                }
                //直接调用easyui初始化
                $(selector).combobox(dataOptions);
            };
            //加载完成后添加一个div
            this.loadSuccess = function () {
                //获取panel
                var $panel = $(selector).combobox("panel");
                //如果已经存在了，就不需要加了
                if ($(".add-combobox-item", $panel.parent()).length === 0) {
                    //需要添加的div
                    var addDiv = $("<div class='combobox-item add-combobox-item'><span class='combobox-add'>&nbsp;</span><a href='javascript:void(0);'>" + addOptions.itemTitle + "</a></div>");
                    //加入到第一层
                    $panel.parent().append(addDiv);
                    //div的点击事件
                    addDiv.off("click.add").on("click.add", that.openAddDialog);
                }
                //如果需要显示查询按钮
                if (addOptions.showSearchBox) {
                    //
                    if ($(".search-combobox-item", $panel.parent()).length === 0) {
                        //需要添加的div
                        var searchDiv = $("<div class='combobox-item search-combobox-item'><span class='combobox-search'>&nbsp;</span><a href='javascript:void(0);'>" + searchLang + "</a></div>");
                        //加入到第一层
                        $panel.parent().append(searchDiv);
                        //div的点击事件
                        searchDiv.off("click.search").on("click.search", that.openSearchDialog);
                    }
                }

                if (addOptions.ButtonList && addOptions.ButtonList.length > 0) {
                    for (var i = 0; i < addOptions.ButtonList.length; i++) {
                        var itemClass = "button-combobox-item" + i;
                        var buttonItem = addOptions.ButtonList[i];

                        if ($("." + itemClass, $panel.parent()).length === 0) {
                            //需要添加的div
                            var searchDiv = $("<div class='combobox-item add-combobox-item button-combobox-itme " + itemClass + "'><span class='" + buttonItem.buttonClass + "'>&nbsp;</span><a href='javascript:void(0);'>" + buttonItem.buttonName + "</a></div>");
                            //加入到第一层
                            $panel.parent().append(searchDiv);
                            //div的点击事件
                            searchDiv.off("click").on("click", buttonItem.buttonEvent);
                        }
                    }
                }

            };
            //点击添加弹出事件
            this.openAddDialog = function () {
                var url = "";
                var $panel = $(selector).combobox("panel");
                if ($(".add-combobox-item", $panel.parent()).length > 0) {
                    url = $(".add-combobox-item", $panel.parent()).attr("url");
                }
                if (url == undefined || url == "" || url == null) {
                    url = addOptions.url;
                }
                //如果没有来源，则自动加上来源
                if (url.indexOf("Origin=") < 0) {
                    url = url.indexOf('?') > 0 ? (url + '&Origin=Dropdown') : (url + '?Origin=Dropdown');
                }
                //直接弹框
                $.mDialog.show({
                    mTitle: addOptions.dialogTitle,
                    mWidth: addOptions.width || 450,
                    mHeight: addOptions.height || 350,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + url,
                    mCloseCallback: that.closeCallback
                });
            };
            //点击添加弹出事件
            this.openSearchDialog = function () {

                $(selector).combobox("hidePanel");
                //
                if ($(".search-combobox-dialog", "body").length > 0) {
                    //如果有，就直接删除
                    $(".search-combobox-dialog", "body").remove();
                }
                //
                var html = "<div class='search-combobox-dialog' id='searchComboboxDialog' style='display:none;'>"
                            + "<div class='search-combobox-search-div'>"
                                + "<input class='search-combobox-search-input'>"
                                + "<span class='search-combobox-search-span searchbox-button'>&nbsp;&nbsp;</span>"
                                + "<span class='search-combobox-close-box mCloseBox'>&nbsp;&nbsp;</span>"
                              + "</div>"
                              + "<div class='search-combobox-table-div'>"
                                + "<table class='search-combobox-table'>"
                                + "</table>"
                              + "</div>"
                            + "</div>"
                          + "</div>";
                //
                $("body").append(html);
                //直接弹框
                $.mDialog.show({
                    mTitle: searchLang,
                    mWidth: addOptions.swidth || 600,
                    mHeight: addOptions.sheight || 400,
                    mDrag: "search-combobox-search-div",
                    mShowbg: false,
                    mShowTitle: false,
                    mContent: "id:searchComboboxDialog",
                    mOpenCallback: that.initSearchContent
                });
            };
            //显示搜索的内容
            this.initSearchContent = function () {
                //
                $(".search-combobox-search-input").focus();
                //
                var func = function (data) {
                    //
                    that.showData2SearchTable(data, "", $(selector).combobox("getValue"));
                }
                //
                if ($(selector).combobox("getData")) {
                    //
                    func($(selector).combobox("getData"));
                }
                else {
                    //
                    mAjax.Post(dataOptions.url, {}, function (data) {
                        //
                        func(data);
                    });
                }

            };
            //
            this.showData2SearchTable = function (data, keyword, value) {
                //
                var table = $(".search-combobox-table");
                //
                if (!data) {
                    //
                    data = table.data("data");
                }
                else {
                    //
                    table.data("data", data);
                }
                //
                var matchTr = "";
                //
                table.empty();
                //
                var header = "<tr class='search-combobox-table-header'><th style='width:50px'>" + indexLang + "</th>";
                //
                var averageWith = (table.width() - 50) / (addOptions.searchColumns.length);
                //
                for (var i = 0; i < addOptions.searchColumns.length ; i++) {
                    var searchColumn = addOptions.searchColumns[i];
                    //
                    header += "<th style='width:" + (searchColumn.width || averageWith) + "'>" + searchColumn.title + "</th>";
                }
                //
                header + "</tr>";
                //
                table.append(header);
                //
                for (var i = 0; i < data.length ; i++) {
                    //
                    var matchData = data[i];
                    //
                    var fond = false;
                    //
                    var tr = "<tr " + dataOptions.valueField + "='" + matchData[dataOptions.valueField] + "' class='search-combobox-table-tr'><td class='search-combobox-table-td-index'>" + (i + 1) + "</td>";
                    //
                    for (var j = 0; j < addOptions.searchColumns.length; j++) {
                        //
                        if (keyword) {
                            //
                            if (matchData[addOptions.searchColumns[j].field] && matchData[addOptions.searchColumns[j].field].indexOf(keyword) >= 0) {
                                //
                                fond = true;
                            }
                        }
                        else {
                            fond = true;
                        }
                        //
                        tr += "<td>" + mText.encode(matchData[addOptions.searchColumns[j].field]) + "</td>";
                    }
                    //
                    tr += "</tr>";
                    //
                    if (fond) {
                        //
                        $tr = $(tr);
                        //
                        table.append($tr);
                        //
                        $tr.data("addData", matchData);
                    }
                }
                //第一行加上被选中的样式
                var tr = value ? $("tr.search-combobox-table-tr[" + dataOptions.valueField + "='" + value + "']", table) : $("tr.search-combobox-table-tr:eq(0)", table);
                //
                $("td", tr).addClass("combobox-item-selected");
                //
                $(".search-combobox-table-div").height($(".search-combobox-table-div").parent().height() -
                    $(".search-combobox-search-div").height() - 22);
                //
                that.scroll2Tr(tr);
                //
                that.initSearchBoxEvent();
            };
            //初始化查找框里面的事件
            this.initSearchBoxEvent = function () {
                //
                var table = $(".search-combobox-table");
                //初始化行的点击事件
                $("tr:gt(0)", table).each(function (index) {
                    //
                    var $tr = $(this);
                    //
                    $tr.off("click.add").on("click.add", function (e) {
                        //
                        //$(selector).combobox("setValue", $(this).attr(dataOptions.valueField));
                        //
                        $(".search-combobox-close-box").trigger("click");
                        //关闭弹出层
                        //$.XYTipsWindow.closeBox();
                        //
                        $(selector).combobox("select", $(this).attr(dataOptions.valueField));
                        //
                        e.stopPropagation();
                        //
                        return false;
                    });
                });
                //输入框里面的事件
                $(".search-combobox-search-input").off("keyup.add").on("keyup.add", function (e) {
                    //如果是回车键
                    if (e.keyCode == 13) {
                        //找出表格里面的第一行
                        that.getNextPrevRow(table).trigger("click");
                    }
                    else if (e.keyCode == 38 || e.keyCode == 39) {
                        //向上选择
                        //找到当前选中的那一行
                        var tr = that.getNextPrevRow(table, -1);
                        //
                        if (tr.length > 0) {
                            //
                            $("td.combobox-item-selected").removeClass("combobox-item-selected"),
                            //加上选择的样式
                            $("td", tr).addClass("combobox-item-selected");
                            //
                            that.scroll2Tr(tr);
                        }
                        //
                        that.stopPropagation(e);
                    }
                    else if (e.keyCode == 40 || e.keyCode == 37) {
                        //向下选择
                        //找到当前选中的那一行
                        var tr = that.getNextPrevRow(table, 1);
                        //
                        if (tr.length > 0) {
                            //
                            $("td.combobox-item-selected").removeClass("combobox-item-selected"),
                            //加上选择的样式
                            $("td", tr).addClass("combobox-item-selected");
                            //
                            that.scroll2Tr(tr);
                        }
                        //
                        that.stopPropagation(e);
                    }
                    else {
                        //
                        that.showData2SearchTable(false, $(this).val());
                        //
                        that.stopPropagation(e);
                    }
                });

            };
            //
            this.scroll2Tr = function (tr) {
                //
                var div = $(".search-combobox-table-div");
                //
                if ((tr.offset().top > (div.height() + div.offset().top - tr.height()))
                || (tr.offset().top < div.offset().top)) {
                    //滚动
                    $(div).scrollTop(tr.offset().top + $(div).scrollTop() - $(div).offset().top);
                }
            };
            //
            this.getNextPrevRow = function (table, dir) {
                //
                switch (dir) {
                    case 1:
                        return $("td.combobox-item-selected:eq(0)").parent().next("tr.search-combobox-table-tr");
                    case -1:
                        return $("td.combobox-item-selected:eq(0)").parent().prev("tr.search-combobox-table-tr");
                    default:
                        return $("td.combobox-item-selected:eq(0)").parent();
                }
            };
            //组织冒泡事件
            this.stopPropagation = function (e) {
                //组织冒泡事件
                if (e.stopPropagation) {
                    // this code is for Mozilla and Opera 
                    e.stopPropagation();
                } else if (window.event) {
                    // this code is for IE 
                    window.event.cancelBubble = true;
                }
                return false;
            };
            //添加完成后需要刷新页面
            this.closeCallback = function (param) {
                //如果有传过来的回调函数
                addOptions.callback && $.isFunction(addOptions.callback) && addOptions.callback(param);
                //combobox必须要重新加载
                if (addOptions.isReload !== false) {
                    //重新加载，默认为重新加载
                    $(selector).combobox("reload");
                    //如果有保存参数，则设置参数
                    if (param) {

                        param = typeof param == "object" ? param[addOptions.keyField || "MItemID"] : param;
                        //
                        var valueField = $(selector).combobox("options").valueField;
                        //
                        if ($(selector).combobox("getData") && $(selector).combobox("getData").where('x.' + valueField + " == '" + param + "'").length > 0) {
                            //combobox赋值
                            $(selector).combobox("select", param);
                        }
                    }

                }
                //是否在弹出框关闭的时候隐藏panel
                addOptions.hideWhenClose && $(selector).combobox("hidePanel");
            };
        };
        //返回
        return mAddCombobox;
    })()
    //扩展到$
    $.fn.mAddCombobox = function (type, dataOptions, addOptions) {
        //类型
        type = type || $(this).attr(addComboboxAttrName);
        //获取dataoptions
        var dataAttr = $(this).attr("data-options");
        //有data-options属性
        if (dataAttr) {
            dataOptions = $.extend(eval("({" + dataAttr + "})"), dataOptions || {});
        }
        //如果有配置
        if (addComboboxDataOptions[type]) {
            dataOptions = $.extend(addComboboxDataOptions[type], dataOptions || {});
        }
        //获取addOptions
        var addAttr = $(this).attr("add-options");
        //如果有配置
        if (addAttr) {
            addOptions = $.extend(eval("({" + addAttr + "})"), addOptions || {});
        }
        //如果有传参数
        if (addCombboxTypeOptions[type]) {
            addOptions = $.extend(addCombboxTypeOptions[type], addOptions || {});
        }
        //初始化吧
        return new mAddCombobox(this, dataOptions, addOptions).init();
    };
})()

$(function () {
    //整个页面都扫一遍
    $("input,select").filter(function () { return $(this).attr(addComboboxAttrName); }).each(function () {
        //初始化可添加选择框
        $(this).mAddCombobox();
    });
})

//扩展Easyui datagrid editor
$.extend($.fn.datagrid.defaults.editors, {
    addCombobox: {
        init: function (container, options) {
            //
            var $input = $("<input type='text' class='" + ((options.addOptions && options.addOptions.addClass) ? (options.addOptions.addClass) : "") + "'>").appendTo(container);
            //
            $input.mAddCombobox(options.type, options.dataOptions, options.addOptions, options.callback);
            //返回
            return $input;
        },
        getValue: function (target) {
            var opts = $(target).combobox("options");
            if (opts.multiple) {
                return $(target).combobox("getValues").join(opts.separator);
            } else {
                return $(target).combobox("getValue");
            }
        },
        setValue: function (target, value) {
            var opts = $(target).combobox("options");
            if (opts.multiple) {
                if (value) {
                    $(target).combobox("setValues", value.split(opts.separator));
                } else {
                    $(target).combobox("clear");
                }
            } else {
                $(target).combobox("setValue", value);
            }
        },
        resize: function (target, width) {
            $(target).combobox("resize", width);
        },
        setText: function (a, b) {
            $(a).combobox("setText", b)
        },
    }
});
