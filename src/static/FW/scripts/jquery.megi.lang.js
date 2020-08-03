/*
新版本的多语言
*/
(function () {
    var mLang = (function () {
        //
        var mLang = function (selector, langData) {
            //
            selector = selector || ("." + window.mLanguageClassName);
            //
            langData = langData || "";
            //
            var that = this;
            //
            this.langDiv = "";
            //
            this.divId = "";
            //是否在div中
            this.inDiv = false;
            //
            this.langConfigBtn = "";
            //用户当前选择的语言
            this.currentLang = mLang.getLang()[0];

            //是否禁用
            this.disabled = false;

            //设置初始化的值
            this.setLangEditorData = function () {
                //
                var langFeilds = langData.MMultiLanguageField;
                //遍历
                for (var i = 0; i < langFeilds.length ; i++) {
                    //
                    var value = langFeilds[i].MValue;
                    //设置值
                    $("#" + langFeilds[i].MLocaleID, that.langDiv).val(value ? value : "");
                    //给selector赋值
                    if (langFeilds[i].MLocaleID == that.currentLang.LangID) {
                        //给selector赋值
                        $(selector).val(value ? value : "");
                    }

                }
            };
            //
            this.initLangEditor = function () {
                //divid
                that.divId = that.fieldName + "LangDiv";
                //
                that.langDiv = $("#" + that.divId);
                //
                $(selector).data(langDataName, langData);
                //
                $(selector).data(langDivId, that.divId);
                //如果已经存在了
                if ($("#" + that.divId).length == 0) {
                    //绑定数据
                    //初始化内容
                    that.initLangDiv();
                    //添加到body
                    that.initLangEditorDiv();
                    //事件
                    that.initLangDivEvent();
                    //设置定时器
                    window.setInterval(that.postionInterval, 100);
                }
                //设置值
                if (langData) {
                    that.setLangEditorData();
                }
            };
            //输入框keyup,keup的时候，把多语言里面为空的字段补充称文本框里面的字段
            this.selectorKeyup = function () {
                //
                var value = $(selector).val();
                //判断三个框里面的内容是否一致
                var currentValue = $("#" + mLang.getLang()[0].LangID, that.langDiv).val();
                //遍历
                for (var i = 0; i < mLang.getLang().length ; i++) {
                    //如果恒不相等
                    if (currentValue !== $("#" + mLang.getLang()[i].LangID, that.langDiv).val()) {
                        //
                        currentValue = false;
                        break;
                    }
                    else {
                        currentValue = $("#" + mLang.getLang()[i].LangID, that.langDiv).val()
                    }
                }
                //如果三个值不相等
                if (currentValue === false) {
                    //只修改当前语言的值
                    $("#" + that.currentLang.LangID, that.langDiv).val(value);
                }
                else {
                    $("input[type='text']", that.langDiv).val(value);
                }
            };
            //点击ok
            this.langOkClick = function () {
                //隐藏
                that.hideLangDiv();
                //同步值
                var result = $("#" + that.currentLang.LangID, that.langDiv).val();
                $(selector).val(result);
                //验证下
                $(selector).validatebox('isValid');


            };
            //点击取消
            this.langCancelClick = function () {
                //隐藏
                that.hideLangDiv();
            };
            //隐藏所有的多语言
            this.hideAllLangDiv = function () {
                //
                $("." + mLanguageDivClassName).not(that.langDiv).hide();
            };
            //显示
            this.showLangDiv = function () {
                //是否禁用
                that.disabled = $(selector).attr("disabled");
                var orgLang = mLang.getLang();
                //是否禁用
                for (var i = 0; i < orgLang.length ; i++) {

                    if (that.disabled == "disabled") {
                        $("#" + orgLang[i].LangID, "#" + that.divId).attr("disabled", "disabled");
                    } else {
                        $("#" + orgLang[i].LangID, "#" + that.divId).removeAttr("disabled");
                    }
                }
                //
                that.langDiv.show();
            };
            //隐藏
            this.hideLangDiv = function () {
                //
                that.langDiv.hide();
                //隐藏的时候去检查值
                that.checkLangValue();
            };
            this.checkLangValue = function () {
                var tempValue = "";
                var isNullIndex = [];
                //遍历
                for (var i = 0; i < mLang.getLang().length ; i++) {
                    //获取多语言的值
                    var langValue = $("#" + mLang.getLang()[i].LangID, that.langDiv).val();
                    if (langValue == "") {
                        //如果没有值把Index存起来
                        isNullIndex.push(i);
                    }
                    else if (tempValue == "") {
                        //如果有赋值并且tempValue没值赋值给tempValue
                        tempValue = langValue;
                    }
                }
                //遍历赋值给没有值的项
                for (var i = 0; i < isNullIndex.length; i++) {
                    $("#" + mLang.getLang()[isNullIndex[i]].LangID, that.langDiv).val(tempValue);
                }

            };
            //toggle
            this.toggleLangDiv = function () {
                //
                that.hideAllLangDiv();
                //
                if (that.langDiv.is(":hidden")) {
                    //初始化位置
                    that.initLangDivPostion();
                    //显示
                    that.showLangDiv();
                }
                else {
                    //隐藏
                    that.hideLangDiv();
                }


            };
            //当前selector的name
            this.fieldName = $(selector).attr("name");
            //获取html,用一个div 里面放置一个 ul 里面在放置li,最后放置一个save 以及 cancel的按钮
            this.initLangDiv = function () {
                //
                var divHtml = "<div id='" + that.divId + "' class='" + mLanguageDivClassName + "'>";
                //
                divHtml += "<ul>";
                //用户设置的多语言
                var orgLang = mLang.getLang();
                //遍历
                for (var i = 0; i < orgLang.length ; i++) {
                    //第一行显示名字
                    var li0 = "<li>" + orgLang[i].LangName + "</li>";
                    //第二行显示输入框
                    var li1 = "<li><input type='text' style='line-height:16px;height:16px' id='" + orgLang[i].LangID + "' value=''/>";

                    //加入到div
                    divHtml += li0 + li1;
                }
                //加上确认取消功能
                divHtml += "<li class='m-lang-ok-cancel'>"
                    //+ "<input class='m-lang-cancel l-btn' type='button' value='" + LangKey.Cancel + "'/>"
                    + "<input class='m-lang-ok l-btn easyui-linkbutton-yellow ' type='button' value='" + LangKey.OK + "'/>" + "</li>";
                divHtml += "</ul>";
                //加入确认取消功能
                divHtml += "</div>";
                //加入到body
                that.langDiv = $(divHtml);
            };
            //设置一个定时器用户判断lang div的位置是否还在 selector的附近
            this.postionInterval = function () {
                //如果是可见的
                if (that.langDiv.is(":visible")) {
                    //获取其位置
                    var offset = that.langDiv.offset;
                    //
                    if (offset.left != $(selector).offset().left) {
                        //重新计算位置
                        that.initLangDivPostion();
                    }
                }
            };
            //获取用户设置的lang数据
            this.getLangData = function () {

            };
            //实时计算langdiv的位置
            this.initLangDivPostion = function () {
                //如果下方放不下了
                var top = $(selector).offset().top + $(selector).outerHeight() + that.langDiv.outerHeight();
                //内容框
                var contentHeight = $(".m-imain-content").length > 0 ? $(window).outerHeight() : ($("body").outerHeight() - $(".m-fixed-bottom").outerHeight());
                //如果高于content的高度，则放置在上方
                var top = (top >= contentHeight) ? ($(selector).offset().top - that.langDiv.outerHeight()) : ($(selector).offset().top + $(selector).outerHeight());
                that.langDiv.css({
                    "left": $(selector).offset().left + "px",
                    "top": top + "px"
                });
            };
            //多语言点击事件
            this.initLangDivEvent = function () {
                //点击事件
                that.langConfigBtn.off("click.lang").on("click.lang", function (event) {
                    //点击隐藏以及显示
                    that.toggleLangDiv();
                    //组织冒泡事件
                    if (event.stopPropagation) {
                        // this code is for Mozilla and Opera 
                        event.stopPropagation();
                    }
                    else if (window.event) {
                        // this code is for IE 
                        window.event.cancelBubble = true;
                    }
                });
                //本身点击不触犯冒泡事件
                that.langDiv.off("click.lang").on("click.lang", function (event) {
                    //组织冒泡事件
                    if (event.stopPropagation) {
                        // this code is for Mozilla and Opera 
                        event.stopPropagation();
                    }
                    else if (window.event) {
                        // this code is for IE 
                        window.event.cancelBubble = true;
                    }
                });
                //本身失去焦点的时候，隐藏
                //$(selector).blur(function () {
                //    $("." + langOkClassName, that.langDiv).trigger("click.lang");
                //});
                //ok按钮
                $("." + langOkClassName, that.langDiv).off("click.lang").on("click.lang", that.langOkClick);
                //cancel按钮
                $("." + langCancelClassName, that.langDiv).off("click.lang").on("click.lang", that.langCancelClick);
                //selector key up事件
                $(selector).on("blur.lang", that.selectorKeyup);
            };
            //初始化div的位置,大小等
            this.initLangEditorDiv = function () {
                //在selector的尾部加上一个多语言点击按钮
                var langConfigBtn = "<span class='" + mLangBtnClassName + "'></span>";
                //
                that.langConfigBtn = $(langConfigBtn);
                //加入到input
                $(selector).after(that.langConfigBtn);
                //宽度
                that.langDiv.width($(selector).outerWidth() - 2);
                //selector宽度变窄
                $(selector).css({
                    "width": $(selector).width() - that.langConfigBtn.width() + "px"
                });
                //添加样式
                $(selector).addClass(langInputClassName);
                //计算位置
                that.langConfigBtn.css({
                    "top": ($(selector).offset().top + $(selector).outerHeight()) - (that.langConfigBtn.offset().top + that.langConfigBtn.height()) - 1 + "px",
                    "right": 1 + "px"
                });
                //
                that.langDiv.appendTo("body");
                //里面的input宽度
                $("input", that.langDiv).not("input[type='button']").width((that.langDiv.width() - 15) + "px");
                //调节selector的宽度
                $(selector).width($(selector).width());

                $("input", that.langDiv).not("input[type='button']").mSelect();
            };
        };
        return mLang;
    })();
    window.mLang = $.mLang = jQuery.mLang = mLang;
    //初始化
    $.fn.initLangEditor = function (langData) {
        return new mLang(this, langData).initLangEditor();
    };
    //获取用户的多语言
    mLang.getLang = function () {
        return top.OrgLang || parent.OrgLang || window.OrgLang;
    }
    //获取值
    $.fn.getLangEditorData = function () {
        //如果不是多语言字段
        if (!$(this).hasClass(langFieldClassName)) {
            //返回失败
            return undefined;
        }
        //
        var langData = $(this).data(langDataName);
        //如果langData没有的话，则组装一个对象
        if (!langData) {
            var lang = mLang.getLang();
            //
            langData = {};
            //多语言字段
            langData.MMultiLanguageField = [];
            //如果是复制多语言信息的话，则可能涉及到fieldName不相同
            langData.MFieldName = $(this).attr("name");
            //
            for (var i = 0; i < lang.length ; i++) {
                langData.MMultiLanguageField.push({
                    MLocaleID: lang[i].LangID,
                    MValue: ""
                });
            }
        }
        //
        var $langDiv = $("#" + $(this).data(langDivId));
        //
        var langFeilds = langData.MMultiLanguageField;
        //
        for (var i = 0; i < langFeilds.length ; i++) {
            langFeilds[i].MValue = $("#" + langFeilds[i].MLocaleID, $langDiv).val();
        }
        return langData;
    };
})()

$(function () {
    $("." + langFieldClassName).not(":hidden").each(function () {
        $(this).initLangEditor();
    });

    //整个document的点击事件
    $(document).off("click.lang").on("click.lang", function () {
        //所有的多语言框架

        $("." + mLanguageDivClassName).not(":hidden").each(function () {
            $("." + langOkClassName, $(this)).trigger("click.lang");
        });
    });
});