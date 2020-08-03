/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var RIInspect = /** @class */ (function () {
    function RIInspect() {
        //获取巡检类型的url
        this.getCategoryListUrl = "/GL/GLVoucher/GetCategoryList";
        //开始巡检的url
        this.inspectUrl = "/GL/GLVoucher/StartInspect";
        //清除数据缓存
        this.clearDataPoolUrl = "/GL/GLVoucher/ClearDataPool";
        //具体的项目对应的li
        this.periodInfo = ".ri-period-span";
        this.closeMessage = ".ri-period-message";
        this.categoryLi = ".ri-accordion-li";
        this.categoryUl = ".ri-accordion-ul";
        this.categoryTop = ".ri-accordion-top";
        this.titleName = ".ri-title-name";
        this.titleDetail = ".ri-title-detail";
        this.accordionIcon = ".ri-accordion-icon";
        this.accordionBody = ".ri-accordion-body";
        this.accordionBodyUl = ".ri-accordion-body-ul";
        this.accordionBodyLi = ".ri-accordion-body-li";
        this.subCategory = ".ri-inpsect-sub-category";
        this.inspectSubResult = ".ri-inpsect-sub-result";
        this.inspectPassClass = "ri-inpsect-passed";
        this.inspectFailClass = "ri-inpsect-failed";
        this.inspectSubResultLink = ".ri-inpsect-detail-link";
        this.inspectButton = ".ri-inpsect-button";
        this.inspectProgressbar = ".ri-inspect-progressbar";
        this.closePeriodButton = ".ri-close-button";
        this.upIconClass = "ri-up-icon";
        this.downIconClass = "ri-down-icon";
        this.inspectPassed = false;
        this.isInspecting = false;
        this.topFailedClass = "ri-top-failed";
        this.inspectChildrenBody = ".ri-children-body";
        this.inspectChildrenUl = ".ri-children-ul";
        this.inspectChildrenLi = ".ri-children-li";
        this.inspectChildrenLink = ".ri-inpsect-children-link";
        this.childrenAccordionIcon = ".ri-children-accordion-icon";
        this.normalHrefClass = "ri-normal-href";
        this.currentIndex = 0;
        this.lastIndex = -1;
        this.currentIntervalId = 0;
        this.accountDisableMessage = HtmlLang.Write(LangModule.GL, "AccountDisale", "科目已被禁用，无法检查！");
        //从后台读取完成一次以后就一只保存使用
        this.CategoryList = [];
        //保存所有的结果
        this.ResultList = [];
        //顶部检查的数量
        this.topInspectCount = 0;
        //已经完成的顶部检查数量
        this.finishedTopInspectCount = 0;
    }
    /**
     * 开始
     */
    RIInspect.prototype.init = function () {
        var _this = this;
        this.voucherHome = new GLVoucherHome();
        this.year = +($("#year").val()), this.period = +($("#period").val());
        //展示巡检类型
        this.getCategoryList(function (data) {
            //展示
            _this.showCategory(data);
            //初始化事件
            _this.initEvent();
        });
    };
    /**
     * 初始化事件
     */
    RIInspect.prototype.initEvent = function () {
        var _this = this;
        //开始巡检
        $(this.inspectButton).off("click.ri").on("click.ri", function () { return _this.start(); });
        $(this.accordionIcon).off("click.ri").on("click.ri", function (event) { return _this.clickAccordion(event, _this.accordionBody, _this.categoryLi); });
        //结账按钮
        $(this.closePeriodButton).off("click.li").on("click.ri", function () { return _this.closePeriod(); });
        $(window).resize(function () {
            _this.initDomHeight();
        });
    };
    /**
     * 结账
     */
    RIInspect.prototype.closePeriod = function () {
        if (this.isInspecting === true)
            return mDialog.message(HtmlLang.Write(LangModule.GL, "PleaseWaitUntilInspectFinished", "巡检正在进行，请等待结束后再进行操作"));
        if (!this.inspectPassed)
            return mDialog.alert(HtmlLang.Write(LangModule.GL, "InspectFailed", "巡检结果有未通过的必查项，请先检查本期数据!"));
        this.voucherHome.closePeriod({
            MYear: this.year,
            MPeriod: this.period,
            MStatus: 1
        }, function () {
            mDialog.close();
        });
    };
    /**
     * 计算高度
     */
    RIInspect.prototype.initDomHeight = function () {
        $(this.categoryTop).height($(".m-imain").height() - 40);
        this.initDom();
    };
    /**
     * 点击收起和站点那个icon
     * @param target
     */
    RIInspect.prototype.clickAccordion = function (event, targetClass, liClass) {
        var _this = this;
        var target = $(event.target || event.srcElement);
        var li = target.parents(liClass);
        //如果点击收起，则只收起当前的，
        if (target.hasClass(this.upIconClass)) {
            target.addClass(this.downIconClass).removeClass(this.upIconClass);
            $(targetClass, li).fadeOut();
        }
        else {
            //如果点击展开，则需要收起其他的
            $("." + this.upIconClass + ":visible", li.parent()).addClass(this.downIconClass).removeClass(this.upIconClass);
            target.addClass(this.upIconClass).removeClass(this.downIconClass);
            $(targetClass).fadeOut();
            $(targetClass, li).fadeIn(500, function () { _this.initDom(); });
        }
        this.initDomHeight();
    };
    /**
     * 获取当前组织所有的巡检类型
     * @param callback
     */
    RIInspect.prototype.getCategoryList = function (callback) {
        mAjax.post(this.getCategoryListUrl, { year: this.year, period: this.period }, function (data) {
            $.isFunction(callback) && callback(data);
        }, null, true);
    };
    /**
     * 总巡检入口
     */
    RIInspect.prototype.start = function () {
        var _this = this;
        if (this.isInspecting === true)
            return mDialog.message(HtmlLang.Write(LangModule.GL, "PleaseWaitUntilInspectFinished", "巡检正在进行，请等待结束后再进行操作!"));
        this.isInspecting = true;
        $(this.inspectButton).addClass('easyui-linkbutton-yellow').removeClass('easyui-linkbutton-gray');
        $(this.closePeriodButton).addClass('easyui-linkbutton-gray').removeClass("easyui-linkbutton-yellow");
        $("li[finisedcount]").attr("finisedcount", 0);
        $("li[failedcount]").attr("failedcount", 0);
        this.finishedTopInspectCount = 0;
        $(this.titleDetail).hide();
        $(this.inspectSubResult).hide();
        $(this.inspectProgressbar).show();
        $(this.inspectChildrenLi + ":not(.demo)").remove();
        $(this.inspectProgressbar + ":visible").progressbar('setValue', 0);
        this.inspectPassed = false;
        this.ResultList = [];
        var insepectCategory = this.CategoryList.where("x.MIsParent === false");
        this.currentIndex = 0;
        this.lastIndex = -1;
        this.currentIntervalId = window.setInterval(function () {
            //结束条件
            if (_this.currentIndex == insepectCategory.length) {
                window.clearInterval(_this.currentIntervalId);
                return;
            }
            //如果当前执行的行就是i，则表示可以执行了
            if (_this.currentIndex == _this.lastIndex + 1) {
                _this.lastIndex = _this.currentIndex;
                var settingId = insepectCategory[_this.currentIndex].MSetting.MItemID;
                _this.inspect(settingId, function (result) {
                    return _this.showInspectResult(result);
                });
            }
        }, 100);
    };
    /**
     * 显示巡检的结果
     * @param result
     */
    RIInspect.prototype.showInspectResult = function (result) {
        var _this = this;
        var topLi = $("li[id='" + result.MParentID + "']" + this.categoryLi);
        //获取进度表div
        var progressBar = $(this.inspectProgressbar, topLi);
        //总的数量
        var totalCount = +topLi.attr("subcount");
        var finisedCount = +topLi.attr("finisedcount") + 1;
        var failedCount = +topLi.attr("failedcount");
        if (result.MPassed === false || result.MAccountIsDisable === true) {
            topLi.attr("failedcount", ++failedCount);
        }
        topLi.attr("finisedcount", finisedCount);
        progressBar.progressbar('setValue', +((finisedCount * 100 / totalCount).toFixed(1)));
        //找到对应的ul
        var ul = $("li[id='" + result.MParentID + "'] ul" + this.accordionBodyUl);
        //具体的li
        var li = $("li[settingid='" + result.MID + "']" + this.accordionBodyLi, ul);
        //显示结果的div
        var resultDiv = $(this.inspectSubResult, li).show();
        //把linkurl进行拆解
        var links = result.MLinkUrl.split(';');
        var title = HtmlLang.Write(mObject.getPropertyValue(LangModule, links[0]), links[1]);
        //显示点击跳转的链接
        var href = $(this.inspectSubResultLink, resultDiv);
        this.ResultList.push(result);
        //如果无需检查
        if (result.MNoNeedInspect === true) {
            href.text(HtmlLang.Write(LangModule.GL, "NoNeedInpsect", "无需检查"));
            href.removeClass(this.topFailedClass);
            $("span.ri-states-span", resultDiv).removeClass(this.inspectFailClass).addClass(this.inspectPassClass);
            href.addClass(this.normalHrefClass);
            $(this.childrenAccordionIcon, resultDiv).hide();
        }
        else {
            href.text(result.MAccountIsDisable ? this.accountDisableMessage : result.MMessage);
            href.tooltip({
                content: result.MAccountIsDisable ? this.accountDisableMessage : result.MMessage
            });
            if (result.MPassed && !result.MAccountIsDisable) {
                href.removeClass(this.topFailedClass);
                href.addClass(this.normalHrefClass);
            }
            else {
                href.addClass(this.topFailedClass);
                href.removeClass(this.normalHrefClass);
            }
            //根据通过与否，展示图标
            $("span.ri-states-span", resultDiv).removeClass(this.inspectFailClass).removeClass(this.inspectPassClass).addClass((result.MPassed && !result.MAccountIsDisable) ? this.inspectPassClass : this.inspectFailClass);
            var accordionIcon = $(this.childrenAccordionIcon, resultDiv);
            //如果有子项的话，就需要另外处理了
            if (result.children && result.children.length > 0) {
                accordionIcon.show();
                var childrenBody = $(this.inspectChildrenBody, resultDiv);
                childrenBody.hide();
                var childrenUl = $(this.inspectChildrenUl, childrenBody);
                var demoLi = $(this.inspectChildrenLi + ".demo", childrenUl);
                var lastLi = demoLi;
                var _loop_1 = function () {
                    var li_1 = demoLi.clone().removeClass('demo').insertAfter(lastLi);
                    var child = result.children[i];
                    var link = $(this_1.inspectChildrenLink, li_1);
                    //把linkurl进行拆解
                    ls = child.MLinkUrl.split(';');
                    t = HtmlLang.Write(mObject.getPropertyValue(LangModule, ls[0]), ls[1]);
                    if (child.MAccountIsDisable) {
                        child.MMessage = HtmlLang.Write(LangModule.GL, "AccountDisale", "科目已被禁用，无法检查！");
                    }
                    link.text(child.MMessage).tooltip({
                        content: child.MMessage
                    });
                    ;
                    var url = ls[2];
                    var title_1 = t;
                    if (!(child.MPassed && child.MNoLinkUrlIfPassed) && url) {
                        link.removeClass(this_1.normalHrefClass);
                        //如果是通过了并且通过就不需要跳转的话，就不跳转了
                        link.off("click.ri").on("click.ri", function () {
                            _this.showDialog(title_1, url);
                        });
                    }
                    else {
                        link.addClass(this_1.normalHrefClass);
                    }
                    lastLi = li_1;
                };
                var this_1 = this, ls, t;
                for (var i = 0; i < result.children.length; i++) {
                    _loop_1();
                }
                href.off("click.ri").on("click.ri", function () {
                    href.next("span").trigger("click.ri");
                });
            }
            else {
                accordionIcon.hide();
                //如果是凭证有断号的话，需要提示点击进行重排
                if (result.MItemID === '6300' && !result.MPassed) {
                    href.tooltip({ content: HtmlLang.Write(LangModule.GL, "Click2Reorder", '点击进行重排') });
                }
                else {
                    href.tooltip({
                        content: result.MMessage
                    });
                }
                var url = links[2];
                if (!(result.MPassed && result.MNoLinkUrlIfPassed) && (url || result.MItemID === '6300')) {
                    href.removeClass(this.normalHrefClass);
                    href.off("click.ri").on("click.ri", function () {
                        //凭证断号特殊处理
                        if (!result.MPassed && result.MItemID === '6300') {
                            _this.voucherHome.showVoucherNumberBroken(_this.year, _this.period, function () {
                                $("span.ri-states-span", resultDiv).removeClass(_this.inspectFailClass).removeClass(_this.inspectPassClass).addClass(_this.inspectPassClass);
                                href.removeClass(_this.topFailedClass).text(HtmlLang.Write(LangModule.GL, "VoucherNumberContinous", "凭证无断号"));
                                href.off("click.ri");
                                _this.ResultList.where('x.MItemID == "6300"')[0].MPassed = true;
                                _this.showTotalMessage();
                            });
                        }
                        else {
                            _this.showDialog(title, links[2]);
                        }
                    });
                }
            }
        }
        //如果顶部检查完成了，则继续往上
        if (finisedCount == totalCount) {
            //影藏进度条框
            progressBar.hide();
            var titleDetail = $(this.titleDetail, topLi);
            var message = failedCount > 0 ? result.MTopFailedMessage : result.MTopPassedMessage;
            titleDetail.show().text(mText.format(message, totalCount, failedCount));
            failedCount > 0 ? titleDetail.addClass(this.topFailedClass) : titleDetail.removeClass(this.topFailedClass);
            this.finishedTopInspectCount++;
            this.showTotalMessage();
            $(this.childrenAccordionIcon).off("click.ri").on("click.ri", function (event) { return _this.clickAccordion(event, _this.inspectChildrenBody, _this.accordionBodyLi); });
        }
    };
    /**
     * 打开详情窗口
     * @param title
     * @param url
     */
    RIInspect.prototype.showDialog = function (title, url) {
        if (!url)
            return;
        mDialog.show({
            mTitle: title,
            mContent: "iframe:" + url,
        });
        mDialog.max();
    };
    /**
     * 单个巡检
     * @param settingId
     */
    RIInspect.prototype.inspect = function (settingId, callback) {
        var _this = this;
        mAjax.post(this.inspectUrl, { settingId: settingId, year: this.year, period: this.period }, function (result) {
            $.isFunction(callback) && callback(result);
            _this.currentIndex++;
        });
    };
    /**
     *
     * @param list
     */
    RIInspect.prototype.showCategory = function (list) {
        this.CategoryList = list || this.CategoryList;
        var parents = this.CategoryList.where("!!x.MIsParent");
        this.topInspectCount = parents.length;
        var demoLi = $(this.categoryLi + ".demo");
        var lastLi = demoLi;
        for (var i = 0; i < parents.length; i++) {
            var cat = parents[i];
            var li = demoLi.clone();
            li.removeClass('demo').attr("id", cat.MItemID).insertAfter(lastLi).show();
            $(this.titleName, li).text(cat.MName);
            $(this.titleDetail, li).text('');
            $(this.inspectProgressbar, li).progressbar({ width: 400 }).hide();
            $(this.accordionBody, li).hide();
            var subs = this.CategoryList.where(" x.MParentID == '" + cat.MItemID + "'");
            li.attr("subcount", subs.length).attr("finisedcount", 0).attr("failedcount", 0);
            var subDemoLi = $(this.accordionBodyLi + ".demo", li);
            var lastSubLi = subDemoLi;
            for (var j = 0; j < subs.length; j++) {
                var subLi = subDemoLi.clone();
                subLi.removeClass('demo').insertAfter(lastSubLi).attr("settingid", subs[j].MSetting.MItemID).attr("parentid", subs[j].MParentID);
                $(this.subCategory, subLi).text(' - ' + subs[j].MName).tooltip({
                    content: subs[j].MName
                });
                lastSubLi = subLi;
            }
            lastLi = li;
        }
        this.initDomHeight();
    };
    /**
     * 清除数据缓存
     */
    RIInspect.prototype.clearDataPool = function (callback) {
        mAjax.post(this.clearDataPoolUrl, {}, function (data) {
            $.isFunction(callback) && callback();
        });
    };
    /**
     * 页面兼容性
     */
    RIInspect.prototype.initDom = function () {
        var _this = this;
        $(this.closeMessage).css({
            width: ($("body").width() - 330) + "px",
            "max-width": ($("body").width() - 330) + "px"
        });
        $(this.inspectSubResult).each(function (index, elem) {
            $(elem).width($(elem).closest("li").width() - 419);
        });
        $(this.inspectSubResultLink).each(function (index, elem) {
            $(elem).css({
                "width": ($(elem).closest(_this.inspectSubResult).width() - 61) + "px",
                "max-width": ($(elem).closest(_this.inspectSubResult).width() - 61) + "px"
            });
        });
    };
    /**
     * 显示总的结果，能否结账，如果能结账，则结账按钮变成红色，检查按钮变成蓝色
     */
    RIInspect.prototype.showTotalMessage = function () {
        var _this = this;
        //只有在最后的时刻才做检查
        if (this.finishedTopInspectCount !== this.topInspectCount)
            return;
        var text = mText.format(HtmlLang.Write(LangModule.GL, "PeriodInpectFinished", "{0}月份巡检完成"), this.period) + ", ";
        var requirePassResult = this.ResultList.where(' x.MRequirePass && !x.MPassed ');
        if (!!requirePassResult && requirePassResult.length > 0) {
            var messages = requirePassResult.select('MMessage');
            text += messages.join(',') + ", ";
            text += HtmlLang.Write(LangModule.GL, "CannotCloseThisPeriod", "无法结账!");
            $(this.inspectButton).addClass('easyui-linkbutton-yellow').removeClass('easyui-linkbutton-gray');
            $(this.closePeriodButton).addClass('easyui-linkbutton-gray').removeClass("easyui-linkbutton-yellow");
            $(this.closeMessage).addClass(this.topFailedClass).text(text).tooltip({ content: text });
        }
        else {
            text += HtmlLang.Write(LangModule.GL, "CanCloseThisPeriod", "可以结账!");
            $(this.inspectButton).removeClass('easyui-linkbutton-yellow').addClass('easyui-linkbutton-gray');
            $(this.closePeriodButton).removeClass('easyui-linkbutton-gray').addClass("easyui-linkbutton-yellow");
            this.inspectPassed = true;
            $(this.closeMessage).removeClass(this.topFailedClass).text(text).tooltip({ content: text });
        }
        this.initDom();
        /*
            检查完成以后，需要清空数据缓存区
        */
        this.clearDataPool(function () {
            _this.isInspecting = false;
        });
    };
    return RIInspect;
}());
//# sourceMappingURL=RIInspect.js.map