/*常量定义*/

//可弹出新增功能的combogrid的公共央视
var addCombogridAttrName = "add-combogrid-type";
//保存添加url的属性名称
var addCombogridOptionsAtrrName = "add-options";


/*
    一个下拉框，里面除了包含下拉框的内容，还需要添加一个new Item的类似功能
    使用方式有三种
    1.$(selector).mAddCombogrid() selector必须有 data-options/add-options的属性
    2.$(selector).mAddCombogrid(dataoptions,addOptions)
    3.给selector添加add-combogrid样式，并且具有data-options/add-options的属性

    add-options样例：add-options = 
*/
;
(function () {
    //第一层，闭包
    var mAddCombogrid = (function () {
        //第二层闭包
        var mAddCombogrid = function (selector, dataOptions, addOptions) {
            //
            var that = this;
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
                        loadSuccess($(selector).combogrid("getData"));
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
                $(selector).combogrid(dataOptions);
            };
            //加载完成后添加一个div
            this.loadSuccess = function () {
                //获取panel
                var $panel = $(selector).combogrid("panel");
                //如果已经存在了，就不需要加了
                if ($(".add-combogrid-item", $panel.parent()).length === 0) {
                    //需要添加的div
                    var addDiv = $("<div class='combogrid-item add-combogrid-item'><span class=''>+&nbsp;</span><a href='javascript:void(0);'>" + addOptions.itemTitle + "</a></div>");
                    //加入到第一层
                    $panel.parent().append(addDiv);
                    //div的点击事件
                    addDiv.off("click.add").on("click.add", that.openAddDialog);
                }
            };
            //点击添加弹出事件
            this.openAddDialog = function () {
                var url = "";
                var $panel = $(selector).combogrid("panel");
                if ($(".add-combogrid-item", $panel.parent()).length > 0) {
                    url = $(".add-combogrid-item", $panel.parent()).attr("url");
                }
                if (url == undefined || url == "") {
                    url = addOptions.url;
                }

                if ($.isFunction(url)) {
                    url($(selector));
                }
                else {
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
                ////combogrid必须要重新加载
                //if (addOptions.isReload !== false) {
                //    //重新加载，默认为重新加载
                //    $(selector).combogrid("reload");
                //    //如果有保存参数，则设置参数
                //    if (param) {
                //        //
                //        //var idField = $(selector).combogrid("options").idField;
                //        ////
                //        //if ($(selector).combogrid("getData") && $(selector).combogrid("getData").where('x.' + idField + " == '" + param + "'").length > 0) {
                //        //    //combogrid赋值
                //        //    $(selector).combogrid("select", param);
                //        //}
                //    }
                //}
            };
        };
        //返回
        return mAddCombogrid;
    })()
    //扩展到$
    $.fn.mAddCombogrid = function (type, dataOptions, addOptions) {
        //类型
        type = type || $(this).attr(addCombogridAttrName);
        //获取dataoptions
        var dataAttr = $(this).attr("data-options");
        //有data-options属性
        if (dataAttr) {
            dataOptions = $.extend(eval("({" + dataAttr + "})"), dataOptions || {});
        }

        //获取addOptions
        var addAttr = $(this).attr("add-options");
        //如果有配置
        if (addAttr) {
            addOptions = $.extend(eval("({" + addAttr + "})"), addOptions || {});
        }

        //初始化吧
        return new mAddCombogrid(this, dataOptions, addOptions).init();
    };
})()

$(function () {
    //整个页面都扫一遍
    $("input,select").filter(function () { return $(this).attr(addCombogridAttrName); }).each(function () {
        //初始化可添加选择框
        $(this).mAddCombogrid();
    });
})

//扩展Easyui datagrid editor
$.extend($.fn.datagrid.defaults.editors, {
    addCombogrid: {
        init: function (container, options) {
            //
            var $input = $("<input type='text' class='" + ((options.addOptions && options.addOptions.addClass) ? (options.addOptions.addClass) : "") + "'>").appendTo(container);
            //
            $input.mAddCombogrid(options.type, options.dataOptions, options.addOptions, options.callback);
            //返回
            return $input;
        },
        getValue: function (target) {
            var opts = $(target).combogrid("options");
            if (opts.multiple) {
                return $(target).combogrid("getValues").join(opts.separator);
            } else {
                return $(target).combogrid("getValue");
            }
        },
        setValue: function (target, value) {
            var opts = $(target).combogrid("options");
            if (opts.multiple) {
                if (value) {
                    $(target).combogrid("setValues", value.split(opts.separator));
                } else {
                    $(target).combogrid("clear");
                }
            } else {
                $(target).combogrid("setValue", value);
            }
        },
        setText: function (target, value) {
            $(target).combogrid("setText", value);
        },
        showPanel: function () {
            $(target).combogrid("showPanel");
        },
        resize: function (target, width) {
            $(target).combogrid("resize", width);
        }
    }
});
