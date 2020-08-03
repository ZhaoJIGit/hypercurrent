/// <reference path="../include/hiAlert/jquery.alert.js" />
/*
    美记UI组件的管理类

*/

//弹出框
window.mDialog = $.mDialog = {};
//打开弹出框 {mWidth,mHeight,mTitle,href} jquery.megi.ui.js
$.mDialog.show = Megi.dialog = function (options) {

    window.dialog = window.mDialog || {};

    var pageIdInput = $("input[name='hiddenToken']");

    var pageId = "";

    if (pageIdInput.length > 0) {
        pageId = pageIdInput.attr("pageid");
        //如果当前页面有弹出框，就不能再弹出了
        if (window.dialog[pageId] === true) return;
        window.dialog[pageId] = true;
    }

    var tabSwitch = new BrowserTabSwitch();

    options.mTitle = !options.mTitle ? "" : mText.encode(options.mTitle);
    var newOptions = {};
    //兼容老代码
    if (options.href) {
        options.href = tabSwitch.intercept(options.href);
        newOptions.mContent = "iframe:" + Megi.getVersionUrl(options.href);
        newOptions.mWidth = options.width;
        newOptions.mHeight = options.height;
        newOptions.mTitle = mText.encode(options.title);
        newOptions.mShowbg = options.modal;
        newOptions.mDrag = "mBoxTitle";
        newOptions.mOpenCallback = options.openCallback;
        newOptions.mCloseCallback = options.closeCallback;
    }
    else {
        if (options.mContent != undefined && String(options.mContent).indexOf("iframe:") > -1) {

            options.mContent = Megi.getVersionUrl(options.mContent);

            options.mContent = tabSwitch.intercept(options.mContent);
        }
        newOptions = options;
    }

    //记录当前层的document
    options.document = $(document);
    var win = top.window == window ? window : $.mTab.getCurrentIframe()[0].contentWindow;
    //权限验证
    if (options.mNoAuth) {
        window.mFilter.doFilter("track", [newOptions.mContent, newOptions.mPostData]);
        window.$.XYTipsWindow(newOptions);
        if (!!pageId) window.dialog[pageId] = false;
    } else {
        top.accessRequest(function () {

            window.mFilter.doFilter("track", [newOptions.mContent, newOptions.mPostData]);
            //调用本身
            window.$.XYTipsWindow(newOptions);
            if (!!pageId) window.dialog[pageId] = false;
        });
    }

    //弹出窗口

};
/* 
    如果只传一个参数过来的话，则默认index 为0 传过来的只有param
    调用方式
    $.mDialog.setParam(param);
    $.mDialog.setParam(index, param);
*/
$.mDialog.setParam = function (index, param) {
    //
    if (arguments.length == 1) {
        //
        param = index;
        //
        index = 0;
    }
    //找到关闭的按钮
    var $tipsWindow = $(".XYTipsWindow").length > 0 ? $(".XYTipsWindow") : $(".XYTipsWindow", window.parent.document);
    //
    index = index || 0;
    //
    if (param) {
        //
        if (typeof param == "object") {
            param = JSON.stringify(param);
        }
        //
        $tipsWindow.attr("param" + "_" + index, param);
    }
};
//
$.mDialog.setCallbackIndex = function (index) {
    //找到关闭的按钮
    var $tipsWindow = $(".XYTipsWindow").length > 0 ? $(".XYTipsWindow") : $(".XYTipsWindow", window.parent.document);
    //绑定参数
    $tipsWindow.attr("index", index);
}
//关闭弹出框 index表示关闭的时候是需要调用成功的回调函数的下标，其值数值，param是需要回传的参数
$.mDialog.close = Megi.closeDialog = function (index, param) {
    //
    if (arguments.length == 1) {
        //
        param = index;
        //
        index = 0;
    }
    //找到关闭的按钮
    var $tipsWindow = $(".XYTipsWindow").length > 0 ? $(".XYTipsWindow") : $(".XYTipsWindow", window.parent.document);
    //绑定参数
    if (typeof index == "number" && index >= 0) {
        //
        $.mDialog.setParam(index, param);
        //
        $.mDialog.setCallbackIndex(index);
    }
    //
    var mCloseBox = $(".XYTipsWindow").length > 0 ? $(".mCloseBox", $tipsWindow).last() : window.parent.$(".mCloseBox").last();
    //针对有关闭按钮的
    if (mCloseBox.length > 0) {
        //触发点击事件
        mCloseBox.trigger("click");
    }
    else {
        //直接调用closeBox事件
        $(".XYTipsWindow").length > 0 ? $.XYTipsWindow.closeBox() : window.parent.$.XYTipsWindow.closeBox();
    }
    //触发点击事件
};
$.mDialog.callback = function () {
    $(".XYTipsWindow").length > 0 ? $.XYTipsWindow.callback() : window.parent.$.XYTipsWindow.callback();
}
//最大化当前弹出框，只是当前
$.mDialog.max = function () {
    //找到关闭的按钮
    var $tipsWindow = $(".XYTipsWindow").length > 0 ? $(".XYTipsWindow") : $(".XYTipsWindow", window.parent.document);
    //触发点击事件
    var maxObj = $(".mMaxBox", $tipsWindow).not(":hidden");
    //Safari无法最大化
    if (Megi.isSafari() && maxObj.length > 0) {
        var evt = document.createEvent("HTMLEvents");
        evt.initEvent("click", true, true);
        maxObj[0].dispatchEvent(evt);
    }
    else {
        maxObj.trigger("click");
        //IE11最大化时不能自动触发resize事件
        if (Megi.isIE()) {
            $(window).trigger("resize");
        }
    }
};
//还原当前弹出框，只是当前
$.mDialog.min = function () {
    //找到关闭的按钮
    var $tipsWindow = $(".XYTipsWindow").length > 0 ? $(".XYTipsWindow") : $(".XYTipsWindow", window.parent.document);
    //触发点击事件
    $(".mMinBox", $tipsWindow).not(":hidden").trigger("click");
};
//提醒框 content：内容，
/*
callback:点击确认后的回调函数，支持以下形式
    1.{callback:function(){}},
    2.function(){}
type：提醒框的类型，
    0（默认）表示提醒，图标为圆形背景色感叹号
    1 表示警告，图标为三角形背景感叹号
    2 表示失败，图形为哭脸
*/
//isL 表示对话框文字是否左对齐
$.mDialog.alert = $.mAlert = function (content, callback, type, noEncode, isL) {
    //如果callback是{}形式
    if (callback && (typeof callback === "object")) {
        //
        callback = callback.callback;
    }
    //
    type = type || 0;
    //
    var win = top.window == window ? window : $.mTab.getCurrentIframe()[0].contentWindow;
    //调用本身
    win.hiAlert($.mDialog.handleContent(content, noEncode), callback, type, isL);
}
//提示建设当中
$.mDialog.built = function (content, callback, type, isL) {
    //如果callback是{}形式
    if (callback && (typeof callback === "object")) {
        //
        callback = callback.callback;
    }
    //
    type = type || 0;
    var win = top.window == window ? window : $.mTab.getCurrentIframe()[0].contentWindow;
    //调用本身
    win.hiBuilt(content, callback, type, isL);
}
//提醒
$.mDialog.warning = $.mWarning = function (content, callback, noEncode, isL) {
    $.mDialog.alert(content, callback, 1, noEncode, isL)
}
//失败
$.mDialog.error = $.mError = function (content, callback, noEncode, isL) {
    $.mDialog.alert(content, callback, 2, noEncode, isL)
}

//确认框 content：内容，title：对话框标题，callback：回调函数，cancelCallback,用户点击取消的时候的回调函数，若无则不写,options可直接写回调函数函数 haveCancel是否有取消
$.mDialog.confirm = $.mConfirm = function (content, title, callback, cancelCallback, noEncode, haveCancel, isL) {
    var options = {};
    //如果是 confirm(content,callback,cancelCallback)
    if ($.isFunction(title)) {
        options.callback = title;
        options.cancelCallback = callback;
        options.noEncode = cancelCallback;
        options.haveCancel = noEncode;
        options.isL = haveCancel;
    }
    else {
        //兼容老版本, content title callback
        if (arguments.length >= 3) {
            options.callback = callback;
            options.cancelCallback = cancelCallback;
        }
        else {
            //content option方式
            options = title || options;
        }
    }
    var win = top.window == window ? window : $.mTab.getCurrentIframe()[0].contentWindow;
    //
    window.hiConfirm($.mDialog.handleContent(content, options.noEncode), "", function (r) {
        if (r) {
            options.callback(true);
        }
        else {
            options.cancelCallback && $.isFunction(options.cancelCallback) && options.cancelCallback(false);
        }
    }, options.haveCancel, options.isL);
};
//
$.mDialog.handleContent = function (content, noEncode) {
    //
    noEncode = noEncode || (content && content.indexOf('<') == 0);
    //
    return noEncode ? content : mText.encode(content);
}
//hiOverAlert用来展示信息提示条效果，类似本站文章jNotify：操作结果信息提示条中的效果，它提供了两个参数，content：提示内容，timeout：提示时间，默认为3000，即3秒
$.mDialog.message = window.mMsg = $.mMsg = $.overAlert = function (content, timeout, noEncode) {
    //
    timeout = timeout || 3000;
    //
    return top.hiOverAlert($.mDialog.handleContent(content, noEncode), timeout);
};
//tab相关
window.mTab = $.mTab = {};
//添加一个tab标签
$.mTab.add = Megi.addTab = function (title, url, unique, change) {
    return top.accessRequest(function () { top.MegiTab.add(title, url, unique, change); });
};
//将一个tab里面的iframe设置为稳定状态
$.mTab.setStable = function (iframe) {
    return MegiTab.setStable(iframe);
};
//显示某个tab页
$.mTab.showCurrent = function (index) {
    return top.MegiTab.showCurrent(index);
};
//修改当前选中tab的标签名称
$.mTab.rename = $.mTabTitle = function (title) {
    //找到目前正在显示的那个div,并且获取其index
    //var index = top.$(".m-tab-content:visible").attr("data-index");
    //不能使用上面的方法，因为有时候用户会切换tab页，导致找到的不正确
    var index = $.mTab.getCurrentIframe().parent().attr("data-index");
    //修改其对应的名字
    index && top.$(".m-tab-item[data-index='" + index + "']").text(title);
};
//或取当前展示页的title
$.mTab.title = function () {
    //找到目前正在显示的那个div,并且获取其index
    //var index = top.$(".m-tab-content:visible").attr("data-index");
    //不能使用上面的方法，因为有时候用户会切换tab页，导致找到的不正确
    var index = $.mTab.getCurrentIframe().parent().attr("data-index");
    //修改其对应的名字
    return top.$(".m-tab-item[data-index='" + index + "']").text();
};
//刷新某个页面 fireWhenShow表示只有在tab页显示的时候才会执行刷新
$.mTab.refresh = function (titleOrIndex, url, show, fireWhenShow) {
    //如果传过来的整形
    //如果是数字
    if ((parseInt(titleOrIndex) >= 0 && parseInt(titleOrIndex) <= 10) || top.MegiTab.getTabIndex(url, true)) {
        //过滤
        return top.accessRequest(function () {
            window.mFilter.doFilter("track", url);
            top.MegiTab.addOrUpdate(titleOrIndex, url, "", show, "", "", fireWhenShow);
        }, false);
    }
};

//根据url找到一个tab，然后找到里面的iframe，设置对应window的MReady事件
$.mTab.setMReady = function (url, ready) {
    //
    var iframe = top.MegiTab.getIframeByUrl(url);
    //
    if (iframe) {
        //设置初始化事件
        iframe.contentWindow.MReady = ready;
    }
};
//获取某个页面的index
$.mTab.getTabIndex = function (url) {
    //
    return top.MegiTab.getTabIndex(url);
};
//获取当前展示的iframe
$.mTab.getCurrentIframe = function () {
    //return $("iframe", top.$(".m-tab-content:visible"));
    //不能使用上面的方法，因为有时候用户会切换tab页，导致找到的不正确
    if (window.frameElement && window.frameElement.parentElement) {
        //
        return $(window.frameElement);
    }
    else {
        return $("iframe", top.$(".m-tab-content:visible"));
    }
};

//刷新或者添加一个tab，title为空，则刷新当前tab，tile不为空，url为空，则刷新，如果url不为空，则按照新的url刷新,show为false的时候表示不需要切换到新建或者刷新的tab标签
$.mTab.addOrUpdate = function (title, url, isUnique, show, change, lock) {
    //如果没有传过来参数
    if (!title) {
        //获取当前的index
        title = top.MegiTab.getTabIndex();
    }
    //megi解码下 chenpan
    title = _.urlDecode(title);
    //过滤
    return top.accessRequest(function () {
        window.mFilter.doFilter("track", url);
        top.MegiTab.addOrUpdate(title, url, isUnique, show, change, lock);
    });
};
//删除某个tab标签，如果title为空，则删除当前标签
$.mTab.remove = $.mTabDelete = function (title) {
    return top.MegiTab.removeTab(title);
}

/*
    自定义html函数
*/
$.fn.mHtml = function (value) {
    //如果是取值
    if (value == undefined) {

        var v = $(this).html();

        return !!v ? v.replace(/<br>/gi, "\n") : v;
    }
    //如果是赋值
    if (value && typeof value == "string" && value.length > 0) {
        value = value.replace(/\n/gi, '<br>');
    }
    return $(this).html(value);
}

//popup 扩展
$.fn.mPopup = function (options) {
    //
    options.selector = options.selector ? options.selector : $(this);
    //调用现有接口
    Megi.popup(options);
};

$.fn.mLocalCyTooltip = function (amt, localCyId) {
    $(this).tooltip({
        position: 'top',
        content: '<span class="m-tooltip-currecy">' + Megi.Math.toMoneyFormat(Math.abs(amt), 2) + ' <span class="cy">' + localCyId + '</span></span>',
        onShow: function () {
            $(this).tooltip('tip').css({ backgroundColor: '#fff', borderColor: '#666' });
        }
    });
};

//日常使用隐藏的input获取对象里面的字符串，然后转化为对象
if (typeof (mText) != 'undefined') {
    mText.getObject = function (selector) {
        //
        var input = $(selector);
        //
        return input ? mText.parseJson(input[0].value) : {};
    };
}


