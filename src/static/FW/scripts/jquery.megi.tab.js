//切换dashboard时候，是否提醒用户确认
var switchDashboardConfirm = false;
//系统的主页数组
var dashboardTitles = [
    //主页
    {
        title: HtmlLang.Write(LangModule.Common, "dashboard", "Dashboard"),
        url: "/FW/FWHome/FWDashboard?type=0&ver=" + window.top.VersionNumber
    },
    //总账主页
    {
        title: HtmlLang.Write(LangModule.GL, "GeneralLedger", "General Ledger"),
        url: "/GL/GLVoucher/GLVoucherHome?ver=" + window.top.VersionNumber
    },
    //银行主页
    {
        title: HtmlLang.Write(LangModule.Common, "bankaccounts", "Bank Accounts"),
        url: "/BD/BDBank/BDBankHome?ver=" + window.top.VersionNumber
    },
    //采购主页
    {
        title: HtmlLang.Write(LangModule.Common, "purchase", "Purchase"),
        url: "/IV/Bill/BillList?ver=" + window.top.VersionNumber
    },
    //销售主页
    {
        title: HtmlLang.Write(LangModule.Common, "sales", "Sales"),
        url: "/IV/Invoice/InvoiceList?ver=" + window.top.VersionNumber
    },
     //费用报销主页
    {
        title: HtmlLang.Write(LangModule.Common, "expenseclaims", "Expense Claims"),
        url: "/IV/Expense/ExpenseList?ver=" + window.top.VersionNumber
    },
    //报表首页
    {
        title: HtmlLang.Write(LangModule.Common, "allreports", "All Reports"),
        url: "/Report/?ver=" + window.top.VersionNumber,
        ignoreUrl: "/Report/Report2/"
    },
    //发票主页
    {
        title: HtmlLang.Write(LangModule.FP, "FapiaoHome", "发票"),
        url: "/FP/FPHome/FPDashboard"
    }]

/// <reference path="jquery-1.8.2.min.js" />
//美记框架的Tab页添加方法
var MegiTab = {
    tabswitch: new BrowserTabSwitch(),
    //添加tab的标签
    addTitle: function (title, index, locked) {
        var html = '<li ><span class="m-tab-inner">';
        html += '<span class="m-tab-loading"></span>';
        html += '<a href="###" onclick="MegiTab.showCurrent(' + index + ', true)" class="m-tab-item" locked="' + (locked ? "1" : "") + '" data-index="' + index + '">';
        html += title;
        html += '</a>';
        //加上显示保存状态的*号
        //html += '<span class="edit-state">*</span>';
        html += '<a href="###"  data-index="' + index + '" class="close">&nbsp;</a>';
        html += '</span></li>';
        var tab = $(html);
        $(".m-tab").find("ul").find("li").last().before(tab);
        var obj = $(".m-tab").find("ul").find("li").last().prev();
        $(obj).css("max-width", ($(obj).width() + 5) + "px");
        return tab;
    },
    addContent: function (url, index, change, title) {
        //
        var content = $(".m-tab-content[data-index='" + index + "']");
        //
        content.attr("data-url", url);
        //获取iframe
        var iframe = $("iframe", content)[0];
        //重新加载
        url = Megi.getVersionUrl(url);

        url = Megi.addTabTitle(url, title);

        iframe.src = url;
        //添加一个属性表示已经使用
        $(iframe).attr("used", "used");
        //如果有监控change事件
        if (change === true) {
            //给iframe添加一个加载完成事件
            //如果是IE
            if ($.browser.msie) {
                iframe.onreadystatechange = function () {
                    //如果已经完成
                    if (iframe.readyState == "complete") {
                        //如果需要监听change事件
                        $(iframe).attr("change", "change");
                        //绑定事件
                        MegiTab.initChangeInput(iframe);
                    }
                };
            }
            else {
                //不是IE浏览器，监控onload就行了
                iframe.onload = function () {
                    //如果需要监听change事件
                    $(iframe).attr("change", "change");
                    //绑定事件
                    MegiTab.initChangeInput(iframe);
                }
            }
        }
        //
        return content;
    },
    resetLiStyle: function () {
        $(".m-tab-item").closest("li").each(function () {
            var cls = $(this).attr("data-cls");
            $(this).removeClass("current");
            if (cls != undefined) {
                $(this).addClass(cls);
            }
        });
    },
    //刷新一个iframe的title以及url，update表示在相同url的时候，是否也刷新,默认是url相同的时候不刷新
    update: function (index, title, url, update) {
        //如果index = 0
        //index = index == 0 ? index : (index || MegiTab.getTabIndex(title));
        index = index == 0 ? index : (index || MegiTab.getTabIndex(url));
        $(".m-tab-content[data-index='" + index + "']").attr("data-url", url);
        //找到那个iframe
        var iframe = $("iframe", ".m-tab-content[data-index='" + index + "']")[0];
        //获取url
        var oldUrl = $(iframe).attr("src");
        //获取window对象
        var win = iframe.contentWindow;
        //更新的时候需要把ready事件去掉；
        win.MReady = null;
        //
        var desTitle = "&_tabTitle_=" + mDES.encrypt(title);
        //如果url不相等
        if (oldUrl && oldUrl.toLowerCase() != url.toLowerCase()) {
            //刷新url
            var refreshIframe = function () {
                //刷新
                url = Megi.getVersionUrl(url);

                url = Megi.addTabTitle(url, title);
                //
                win.mWindow.reload(url);
                //跟换名字
                $(".m-tab-item[data-index='" + index + "']").text(title);
            };
            //如果需要确认
            if (switchDashboardConfirm === true) {
                //弹出提醒，确认是否刷新
                win.$.mDialog.confirm(HtmlLang.Write(LangModule.Common, "suretorefresh", " Sure to switch dashboard to " + title + "?"), function () {
                    //刷新
                    refreshIframe();
                });
            }
            else {
                //直接刷新
                refreshIframe();
            }
        }
        else if (update === true) {
            //需要更新url
            url = Megi.getVersionUrl(url);

            url = Megi.addTabTitle(url, title);
            //
            win.mWindow.reload(url);
        }
    },
    //根据一个index，展示当前的iframe。isExecuteReadyFunc表示是否执行ready函数 -- by linfq 2019-2-13
    showCurrent: function (index, isExecuteReadyFunc) {
        //找到现在正在显示的那个a
        var currentA = $("li.current", ".m-tab").find(".m-tab-item");
        //找到现在正在显示的那个，看其是否有lock标志，如果有，就不让其点击切换其他页面
        var locked = currentA.attr("data-index") != index && currentA.attr("locked") == "1";
        //如果锁定了，不然其切换
        if (locked) {
            //在这里可以加入一些提醒的内容
            return false;
        }
        MegiTab.resetLiStyle();
        $(".m-tab-content,.m-tab-ctrl").hide();
        $(".m-tab-item[data-index='" + index + "']").closest("li").addClass("current");
        $(".m-tab-content[data-index='" + index + "']").show();
        //
        try {
            //是否有绑定ready函数
            var readyFunc = $.mTab.getCurrentIframe()[0].contentWindow.MReady;
            //手动点击tab或关闭页签显示上一个tab时，才会执行ready函数 -- by linfq 2019-2-13
            isExecuteReadyFunc && $.isFunction(readyFunc) && readyFunc();
        } catch (e) {

        }

        $(window).resize();
    },
    //判断某个title是都存在
    isTabExsits: function (title) {
        //
        var index = false;
        //
        if (title) {
            //
            $(".m-tab").find(".m-tab-item").each(function () {
                var tabTitle = $(this).html();
                //用比较文本的方式确定两个tab是否相同
                if (title.toLowerCase() === tabTitle.toLowerCase()) {
                    //获取下标
                    index = $(this).closest(".m-tab-item").attr("data-index");
                }
            });
        }
        //返回index
        return index;
    },

    //根据title获取某个tab的index(先保留，后期移除tab根据url来移除)
    getTabIndexByTitle: function (title) {
        var index = false;
        //如果没有传过来title，就获取当前的
        if (!title) {
            index = $(".m-tab-item", ".current").attr("data-index");
        }
        else {
            //判断是否是dasoboard的title
            var dashboard = MegiTab.isDashboardTitle(title);
            //如果是在其中
            if (dashboard) {
                //直接返回对象
                return dashboard;
            }
            //
            $(".m-tab").find(".m-tab-item").each(function () {
                var tabTitle = $(this).html();
                //用比较文本的方式确定两个tab是否相同
                if (title.toLowerCase() === tabTitle.toLowerCase()) {
                    //获取下标
                    index = $(this).closest(".m-tab-item").attr("data-index");
                }
            });
        }
        return index;
    },
    //判断title是否是dashboard
    isDashboardTitle: function (title) {
        //如果title无效
        if (title) {
            //
            for (var i = 0; i < dashboardTitles.length ; i++) {
                //
                if (dashboardTitles[i].title.toLowerCase() == title.toLowerCase().trimEnd('/')) {
                    //返回一个对象
                    return dashboardTitles[i];
                }
            }
        }
        return false;
    },
    getIframeByUrl: function (url) {
        //
        var index = MegiTab.getTabIndex(url, true);
        //找到那个iframe
        return (index >= "0" && index <= "10") ? $("iframe", ".m-tab-content[data-index='" + index + "']")[0] : index;
    },
    //根据url获取某个tab的index ignoreDashboard的意思是是否不需要管dashboard的url
    //就是说，如果你传一个dashbaord的url过来，但是其并未显示的话，也不能返回回去
    getTabIndex: function (url, ignoreDashboard) {
        var index = false;
        //如果没有传过来title，就获取当前的
        if (!url) {
            //
            index = $(".m-tab-item", ".current").attr("data-index");
        }
        else {
            //
            if (ignoreDashboard !== true) {
                //判断是否是dasoboard的title
                var dashboard = MegiTab.isDashboardUrl(url);
                //如果是在其中
                if (dashboard) {
                    //直接返回对象
                    return dashboard;
                }
            }
            //
            var relUrl = url.split("?")[0].toLowerCase();
            //
            var setupUrlPrefix = "/BD/Setup";
            //
            var isFromSetup = url.indexOf(setupUrlPrefix) > -1;
            //
            $(".m-tab-content").each(function () {
                //
                var dataUrl = $(this).attr("data-url");
                //
                var oriDataUrl = dataUrl;
                //
                if (!!dataUrl) {
                    //
                    dataUrl = dataUrl.split("?")[0].toLowerCase();
                    //向导只用一个页签显示
                    //if (relUrl.trimStart('/') == dataUrl.trimStart('/')) {
                    if (relUrl.trim('/') == dataUrl.trim('/') || isFromSetup && oriDataUrl.indexOf(setupUrlPrefix) > -1) {
                        //
                        index = $(this).attr("data-index");
                    }
                }
            });
        }
        return index;
    },
    //判断url是否是dashboard
    isDashboardUrl: function (url) {
        var url = url.split("?")[0];
        //如果title无效
        if (url) {
            //
            for (var i = 0; i < dashboardTitles.length ; i++) {
                //
                var requestUrl = url.toLowerCase().trimEnd('/');
                //
                var dashboardUrl = dashboardTitles[i].url.split("?")[0].toLowerCase().trimEnd('/');
                //
                var ignoreUrl = dashboardTitles[i].ignoreUrl ? dashboardTitles[i].ignoreUrl.split("?")[0].toLowerCase() : "########";
                //
                if (requestUrl.indexOf(dashboardUrl) > -1 && requestUrl.indexOf(ignoreUrl) == -1) {
                    //返回一个对象
                    return dashboardTitles[i];
                }
            }
        }
        return false;
    },
    //移除一个tab
    removeTab: function (title) {
        var index = MegiTab.getTabIndexByTitle(title);
        if (index !== false) {
            MegiTab.remove(index);
        }
    },
    //change: true false表示是否监控input框的change事件
    add: function (title, url, unique, change, lock) {
        //获取当前没有显示的tab标签
        var hiddenIframeDiv = $(".m-tab-content:hidden", ".m-main").filter(function () {
            return $(this).attr("data-index") != 0 && !($("iframe", $(this)).attr("src"));
        });
        //
        //var tabIndex = MegiTab.getTabIndex(title);
        var tabIndex = MegiTab.getTabIndex(url);
        //如果是object对象，证明是dashboard
        if (typeof tabIndex == "object") {
            //
            MegiTab.showCurrent(0);
            //先更新这个tab
            MegiTab.update(0, tabIndex.title || title, url || tabIndex.url, true);
        }
        else {
            //是否唯一
            var index = (unique === true && MegiTab.getTabIndex(title)) || false;
            //如果不存在
            //取第一个div
            index = index || (hiddenIframeDiv.length > 0 ? hiddenIframeDiv.attr("data-index") : false);
            //如果index 依然不存在,表示没有合适的tab了
            if (index == false) {
                //弹出提醒框
                var title = HtmlLang.Write(LangModule.Common, "tabcountoverflow", "Your can add maximum 10 tabs!");
                //
                $.mDialog.alert(title);
                //返回
                return false;
            }
            if (unique != true && tabIndex != false) {
                index = tabIndex;
            }
            //如果大于0表示，非dashboard的正常iframe
            var $iframe = $("iframe", ".m-tab-content[data-index='" + index + "']");
            //如果iframe里面没有表示已经使用
            if (!($iframe.attr("used"))) {
                //加一个tab标签
                var tab = MegiTab.addTitle(title, index, lock);
                //加内容
                var content = MegiTab.addContent(url, index, change, title);
                //展示
                MegiTab.showCurrent(index);
                //
                FW.addTab(tab, content);
                //初始化宽度
                MegiTab.setTabItemWidth();
                //初始化tab标签
                MegiTab.bindTitleAction();
            }
            //返回当前的tab
            return index;
        }
    },
    //更新name
    renameTab: function (index, title) {
        //更新名字
        $(".m-tab .m-tab-item[data-index='" + index + "']").text(title);
    },
    //刷新或者添加某个tab，url表示其新的地址，并且是否让其显示 show 为 true 或者 false8
    addOrUpdate: function (title, url, unique, show, change, lock, fireWhenShow) {
        //先找到这个tab,页可以直接传整形的title过来，比如说首页
        var index = (!$.isNumeric(title) || typeof (title) == "string") ? MegiTab.getTabIndex(url) : title;

        var orginUrl = url;

        url = MegiTab.tabswitch.intercept(url);

        //如果是object对象，证明是dashboard
        if (typeof index == "object") {
            //找到这个iframe
            var iframe = $("iframe", ".m-tab-content[data-index='" + 0 + "']")[0];
            //找到上一个url
            var oldUrl = $(iframe).attr("src");

            //如果只是在现实的时候才执行的话，则给contnetWindow绑定一个刷新的事件就行了
            if (fireWhenShow === true && oldUrl.split("?")[0] == (url || index.url).split("?")[0]) {
                //
                iframe.contentWindow.MReady = function () {
                    //
                    $(".m-tab-item[data-index='0']").prev().addClass("m-img-loading");
                    //
                    MegiTab.update(0, index.title, url || index.url, true);
                };
            }
            else {
                //
                $(".m-tab-item[data-index='0']").prev().addClass("m-img-loading");
                //
                MegiTab.update(0, index.title, url || index.url, true);
            }
            //如果显示
            show === false ? "" : MegiTab.showCurrent(0);
        }
            //如果存在就刷新
        else if (index !== false) {
            //找到这个iframe
            var iframe = $("iframe", ".m-tab-content[data-index='" + index + "']")[0];
            //
            lock = lock || $(".m-tab-item[data-index='" + index + "']").attr("locked") == "1";
            //设置lock
            $(".m-tab-item[data-index='" + index + "']").attr("locked", lock ? "1" : "0");

            url = Megi.addTabTitle(url, title);

            //如果只是在现实的时候才执行的话，则给contnetWindow绑定一个刷新的事件就行了
            if (fireWhenShow === true) {
                //
                iframe.contentWindow.MReady = function () {
                    //
                    iframe.contentWindow.mWindow.reload(url || iframe.src);
                };
            }
            else {
                //
                $(".m-tab-item[data-index='" + index + "']").prev().addClass("m-img-loading");
                //
                iframe.contentWindow.mWindow.reload(url || iframe.src);
            }
            //如果需要展示这个tab
            show === false ? "" : MegiTab.showCurrent(index);
            //更新名字
            title && !$.isNumeric(title) && MegiTab.renameTab(index, title);
        }
        else {
            //添加一个
            MegiTab.add(title, url, unique, change, lock);
        }
    },
    //设置当前tab为稳定版本的tab了，就是当前的input,select,textarea 的值为最原始的值
    setStable: function (iframe, selector) {
        //
        iframe = iframe || window.frameElement;
        //默认的selector为没有标记为hidden的input/textarea/select
        selector = selector || "input[type!='hidden'][nocheckchange!='1'],textarea[nocheckchange!='1'],.change-item";
        //
        $(selector, iframe.contentDocument).each(function () {
            //
            var value = ($(this).is("input") || $(this).is("textarea")) ? $(this).val() : $(this).text();
            //记录其本身的值
            $(this).attr("srcValue", value);
            //打上检查change的标签
            $(this).attr("checkChange", "checkChange");
        });
        //直接打上标签，所有调用此方法的iframe都必须有change = change属性
        $(iframe).attr("change", "change");

        $("body").unmask();
    },
    //清除没有使用的iframe的src为空
    clearUnusedIframe: function () {
        //获取当前没有显示的tab标签
        $(".m-tab-content[data-index != 0]:hidden iframe[used != 'used']", ".m-main").each(function () {
            //
            this.src = "";
        });
    },
    //删除一个tab标签
    remove: function (index, iframe) {
        $(".m-tab-content[data-index='" + index + "']").attr("data-url", "");
        //清除其他没有使用的iframe
        MegiTab.clearUnusedIframe();
        //当前对应的iframe
        iframe = iframe || $("iframe", ".m-tab-content[data-index='" + index + "']")[0];
        //去除change属性
        $(iframe).attr("change") && $(iframe).removeAttr("change");
        //置空onload方法
        iframe.onload = iframe.onreadystatechange = null;
        //把src置为空
        $(iframe).attr("used", "");
        //避免其他页面打开的时候，显示以前页面的值
        try { //页面报错无法关掉
            $("body", iframe.contentWindow.document).empty();
        }
        catch (ex) { };
        //下一个index
        var nextIndex = $(".m-tab-item[data-index='" + index + "']").closest("li").next().find(".m-tab-item").attr("data-index");
        //上一个index
        var prevIndex = $(".m-tab-item[data-index='" + index + "']").closest("li").prev().find(".m-tab-item").attr("data-index");
        //tab标签移除
        $(".m-tab-item[data-index='" + index + "']").closest("li").remove();
        //content div隐藏
        $(".m-tab-content[data-index='" + index + "']").hide();
        //查看目前是否已经有显示的tab标签，如果有的话，就不显示上一个标签了
        if ($(".current", ".m-tab").length == 0) {
            //显示上一个tab
            MegiTab.showCurrent(prevIndex, true);
        }
        //初始化一次宽度
        MegiTab.setTabItemWidth();
    },
    //点击tab标签的事件
    bindTitleAction: function () {
        $(".close", ".m-tab").unbind().click(function () {
            //获取下标
            var index = $(this).attr("data-index");
            //内部的iframe.src 设置为空
            var iframe = $("iframe", ".m-tab-content[data-index='" + index + "']")[0];
            //
            //判断iframe是否监控了change 如果是正在加载的iframe，可以直接关闭
            if ($(iframe).attr("change") === "change" && MegiTab.isValueChange(iframe) && !$(".m-tab-item[data-index='" + index + "']").prev().hasClass("m-img-loading")) {
                //
                var title = HtmlLang.Write(LangModule.Common, "suretodeleteunsavedtab", "You have edited this page ,are your sure to close it?");
                //弹出提醒框
                $.mDialog.confirm(title, function () {
                    //
                    MegiTab.remove(index, iframe);
                });
            }
            else {
                //直接关闭
                MegiTab.remove(index, iframe);
            }
            return false;
        });
        //绑定右键事件
        $(".m-tab-ul li").not("#aNewTab").each(function () {

            //my站点不实现这个功能
            if ($("#divDataGridContainer").length > 0) return;
            //初始化右键的点击事件
            var initMenuClickEvent = function (event) {
                //
                event = event || window.event;
                //获取到a标签
                var selector = event.target || event.srcElement;
                //
                var dataIndex = $(selector).attr("data-index");

                //
                var iframe = $("iframe", ".m-tab-content[data-index='" + dataIndex + "']")[0];
                //
                if (iframe) {
                    //iframe遮罩样式
                    var iframeTopBg = $(".iframe-top-bg");
                    //刷新事件
                    $(".home-refresh").off("click.menu").on("click.menu", function () {
                        //
                        iframeTopBg.trigger("click");

                        window.accessRequest(function () {

                            iframe.contentWindow.mWindow.reload();
                        })
                    });
                    //关闭
                    $(".home-close").off("click.menu").on("click.menu", function () {
                        //
                        iframeTopBg.trigger("click");
                        //
                        $(selector).next(".close").trigger("click");
                    });
                    //关闭所有
                    $(".home-close-all").off("click.menu").on("click.menu", function () {
                        //
                        iframeTopBg.trigger("click");
                        //
                        $(".m-tab").find(".close").trigger("click");
                    });
                    //关闭右侧标签
                    $(".home-close-right").off("click.menu").on("click.menu", function () {
                        //
                        iframeTopBg.trigger("click");
                        //
                        $(selector).parent().parent().nextAll("li").find(".close").trigger("click");
                    });
                    //关闭其他
                    $(".home-close-others").off("click.menu").on("click.menu", function () {
                        //
                        iframeTopBg.trigger("click");
                        //首先要点击一下
                        MegiTab.showCurrent(dataIndex, true);
                        //
                        $(selector).parent().parent().siblings("li").find(".close").trigger("click");
                    });
                }
            };
            //
            $(this).mTip({
                target: $(".home-context-menu"),
                width: 150,
                height: 180,
                parent: $(".home-context-menu").parent(),
                trigger: "contextmenu",
                callback: initMenuClickEvent
            });
        });
        //先取消拖拽
        $(".m-tab-ul").sortable("destroy");
        //tab可拖拽事件
        $(".m-tab-ul").sortable({ items: ":not(.stable)" });
    },
    //调整tab标签的宽度
    setTabItemWidth: function () {
        var length = $(".m-tab-item").length;
        var w = $(".m-tab").width();
        var totalLiW = 0;
        $(".m-tab>ul>li").each(function () {
            totalLiW += $(this).width();
        });
        var avrWidth = w / length;

        $(".m-tab>ul>li").each(function (i) {
            if ($(this).hasClass("last")) {
                $(this).css("width", "88px");
            } else if (avrWidth > 150) {
                $(this).css("width", "150px");
            } else {
                $(this).css("width", (avrWidth - 2) + "px");
            }
        });
    },
    init: function () {
        MegiTab.bindTitleAction();
        MegiTab.setTabItemWidth();
    },
    //添加一个展示dashboard的较临时方法，后期同事可以用其他方式实现
    //Felson 
    //type 0.home 1.sale 2 purchase 3.expense 4 report
    showDashboard: function (title, type) {
        //
        var homeUrl = "/FW/FWHome/FWDashboard?type=" + (type ? type : 0) + "&ver=" + window.top.VersionNumber;
        //刷新
        top.$("#dashboard").src = homeUrl;
        //跳转到tab
        Megi.showCurrent(0);
        //替换名字
        $.mTabTitle(title);
    },
    //判断页面是否有值修改
    isValueChange: function (iframe, selector) {
        //
        iframe = iframe || window.frameElement;
        //默认的selector为没有标记为hidden的input/textarea/select
        selector = selector || "input[type!='hidden'][nocheckchange!='1'],textarea[nocheckchange!='1'],.change-item";
        //结果
        var result = false;
        //检查所有的input内容
        var ifrContent = $(iframe.contentDocument) || $(iframe).contents();
        $(selector, ifrContent).each(function () {
            //
            var value = ($(this).is("input") || $(this).is("textarea")) ? $(this).val() : $(this).text();
            //如果有checkChange属性，并且其值不等于其本身的值
            if ($(this).attr("checkChange") === "checkChange" && $(this).attr("srcValue") !== value) {
                //
                result = true;
                //返回
                return;
            }
        });
        return result;
    },
    //给iframe内部的input绑定focus时间
    initChangeInput: function (iframe, selector) {
        //
        iframe = iframe || window.frameElement;
        //默认的selector为没有标记为hidden的input/textarea/select
        selector = selector || "input[type!='hidden'][nocheckchange!='1'],textarea[nocheckchange!='1'],.change-item";
        //检查所有的input内容
        $(selector, $(iframe.contentDocument)).each(function () {
            //加上checkChange属性
            $(this).attr("checkChange", "checkChange");
            //保存初始值
            $(this).attr("srcValue", $(this).val());
        });
    }
}
$(document).ready(function () {
    MegiTab.init();
    $(window).resize(function () {
        MegiTab.setTabItemWidth();
    });
});