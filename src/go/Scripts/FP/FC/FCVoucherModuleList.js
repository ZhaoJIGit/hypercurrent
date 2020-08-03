

(function () {

    var FCVoucherModuleList = (function () {
        //常量定义
        var pageIndex = 1;
        //单页显示的条数
        var pageSize = FCHome.pageSize;
        //总记录数
        var totalCount = 0;
        //当前页所有的凭证
        var pageVoucherModuleData = [];
        //
        var frozenItemID = "";
        /*异步请求的一些url*/
        //获取凭证列表
        var getVoucherListUrl = "/FC/FCHome/GetVoucherModulePageList";
        //编辑凭证
        var editVoucherModuleUrl = "/GL/GLVoucher/GLVoucherEdit";
        //删除凭证模板
        var deleteVoucherModuleUrl = "/FC/FCHome/DeleteVoucherModules";
        //
        var GL = "GL";
        //
        var newVoucherLang = HtmlLang.Write(LangModule.Common, "newvouchermodule", "新建模板凭证");
        //
        var voucherModuleLang = HtmlLang.Write(LangModule.Common, "vouchermodule", "模板凭证");
        //
        var operationFail = HtmlLang.Write(LangModule.Common, "operationfail", "Operation Failded!");
        //
        var sureToDelete = HtmlLang.Write(LangModule.Common, "AreYouSureToDelete", "Are you sure to delete");
        /*按钮的selector*/
        //新建
        var btnNew = ".fcv-new-button";
        //打印div
        var functionDiv = ".fcv-function-div";
        //打印
        var btnPrint = ".fcv-print-button";
        //导出
        var btnExport = ".fcv-export-button";
        //导入
        var btnImport = ".fcv-import-button";
        //删除
        var btnDelete = ".fcv-delete-button";
        //查找
        var btnSearch = ".fcv-search-button";
        //清除
        var btnClear = ".fcv-clear-button";
        //全选
        var btnCheckall = ".fcv-checkall-input";
        //关键字
        var txtKeyword = ".fcv-keyword-input";
        //凭证编号
        var txtCode = ".fcv-code-input";
        //
        var expandButton = ".fcv-expand-button";
        //
        var expandedClass = "tree-folder-open";
        //
        var collapsedClass = "tree-folder-closed";
        //
        var collapseAllButton = ".fcv-collapsedown-button";
        //
        var expandAllButton = ".fcv-collapseup-button";
        //
        var txtType = "#txtType";
        //表格
        var voucherTable = ".fcv-voucher-table";
        //
        var voucherOperation = ".fcv-voucher-operation";
        //
        var operateTop = ".fcv-operate-top";
        //
        var entryList = ".fcv-entry-list";
        //
        var noRecordRowClass = "fcv-norecord-row";
        //
        var voucherTableHeader = ".fcv-voucher-table-header";
        //
        var voucherHeaderDemoClass = "fcv-voucher-header-demo";
        //
        var voucherEntryDemoClass = "fcv-voucher-entry-demo";
        //
        var voucherHeaderClass = "fcv-voucher-header";
        //
        var voucherEntryClass = "fcv-voucher-entry";
        //
        var voucherEmptyClass = "fcv-voucher-empty";
        //
        var voucherDescription = ".fcv-entry-description";
        //
        var voucherFastcode = ".fcv-entry-fastcode";
        //
        var voucherSelect = ".fcv-entry-select input[type='checkbox']";
        //
        var entrySelect = ".fcv-entry-select";
        //
        var entryDiv = ".fcv-entry-div";
        //
        var entryExplanation = ".fcv-entry-explanation";
        //
        var entryAccount = ".fcv-entry-account";
        //
        var entryCurrency = ".fcv-entry-currency";
        //
        var entryCheckGroup = ".fcv-entry-checkgroup";
        //
        var entryDebit = ".fcv-entry-debit";
        //
        var entryCredit = ".fcv-entry-credit";

        var _pager1 = ".fcv-pagenation-div";
        //
        var voucherHeaderDiv = ".fcv-voucher-col-div";
        //编辑
        var voucherEidt = ".fcv-voucher-edit";
        //
        var voucherDelete = ".fcv-voucher-delete";
        //隐藏
        var hide = "fcv-hide";
        //编辑
        var voucherEdit = ".fcv-voucher-edit";
        //删除
        var voucherDelete = ".fcv-voucher-delete";
        //打印
        var voucherPrint = ".vl-function-print";
        //
        var FCVoucherModuleList = function () {
            //
            var that = this;
            //调用home的方法
            var home = new FCHome();
            //
            var checkType = new GLCheckType();
            //获取凭证数据
            this.getVoucherModuleListData = function (func) {
                //
                var code = $(txtCode).val();
                //
                mAjax.post(getVoucherListUrl, {
                    page: pageIndex,
                    rows: pageSize,
                    KeyWord: $(txtKeyword).val(),
                    MIsMulti: $(txtType).combobox("getValue"),
                    MFastCode: code ? code : ""
                }, function (data) {
                    var vouchers = data;
                    //调用回调函数
                    func && $.isFunction(func) && func(vouchers);
                }, "", true);
            };
            //删除凭证模板
            this.deleteVoucherModules = function (moduleID, func) {
                //
                var pkIDs = moduleID ? [moduleID] : that.getSelectedVoucher();
                //必须有勾选在做删除
                if (pkIDs.length > 0) {
                    //先提醒用户是否确定删除
                    mDialog.confirm(sureToDelete, function () {
                        //
                        mAjax.submit(deleteVoucherModuleUrl, {
                            pkIDs: pkIDs
                        }, function (data) {
                            //提醒用户删除成功
                            mDialog.message(LangKey.DeleteSuccessfully);
                            //整个页面更新
                            $.isFunction(func) && func(data);
                        }, "", true);
                    });
                }
                else {
                    //没有选择行
                    mDialog.message(HtmlLang.Write(LangModule.Common, "NotSelectedAnyRow", "Please select a row!"));
                }
            }
            //获取勾选上的凭证列表
            this.getSelectedVoucher = function () {
                //
                var selectedIds = [];
                //获取
                $(voucherSelect).not(":hidden").filter(function () { return $(this).attr("checked") == "checked"; }).each(function () {
                    //
                    var selectedID = $(this).attr("MItemID");
                    //
                    selectedIds.push(selectedID);
                });
                //返回是id的列表还是凭证的列表
                return selectedIds;
            };
            //初始化凭证列表
            this.initVoucherModuleList = function (data) {
                //
                totalCount = data.total;
                //
                var rows = data.rows;
                //
                pageVoucherData = rows;
                //总共的行数
                that.initPagerEvent();
                //获取table里面的body
                var $body = $("tbody", voucherTable);
                //先移除里面的没有记录的行
                $("." + noRecordRowClass).remove();
                //先清除里面的凭证
                $body.find("tr").not("tr[class*='-demo']").remove();
                //如果没有凭证
                if (rows.length == 0) {
                    //加入空行
                    $body.append(that.getNoRecordRow());
                }
                else {
                    //遍历每一个凭证
                    for (var i = 0; i < rows.length ; i++) {
                        //当前凭证
                        var voucherModule = rows[i];
                        //一行一行加入到table中
                        that.createVouherModuleHtml(voucherModule, $body, i == (rows.length - 1));
                    }
                }
                //初始化高度
                that.initEntryDiv();
                //初始化事件
                that.initTableEvent();
                //
                $(voucherTable).each(function () {
                    //
                    $(this).mTableResize({
                        forceFit: false
                    });
                });
            };
            //创建一个凭证的html并加到表格里面
            this.createVouherModuleHtml = function (voucher, tbody, isLast) {
                //
                var html = "";
                //第一行显示的是一个概况
                var $header = $("." + voucherEntryDemoClass).clone();
                //去除class
                $header.removeClass(voucherEntryDemoClass).removeClass(hide).addClass(voucherEntryClass).show().attr("MItemID", voucher.MItemID).attr("topRow", 1).attr("MFastCode", voucher.MFastCode);
                //如果本行需要显示checkbox
                $(voucherSelect, $header).show().attr("MItemID", voucher.MItemID);
                //快速码
                var fastCodeTd = $(voucherFastcode, $header);
                //
                if (voucher.MIsMulti) {
                    //如果有多行
                    fastCodeTd.append((voucher.MIsMulti ? "<div class='fcv-expand-button tree-folder tree-folder-closed ' mitemid='" + voucher.MItemID + "'>&nbsp;</div>" : "") + mText.encode(voucher.MFastCode) + (voucher.MIsMulti ? ("(" + voucher.MVoucherModuleEntrys.length + ")") : ""));
                    //描述
                    $(voucherDescription, $header).text(mText.encode(voucher.MDescription));
                    //借方总金额
                    voucher.MDebitTotal && voucher.MDebitTotal != 0 && $(entryDebit, $header).text(mMath.toMoneyFormat(voucher.MDebitTotal));
                    //贷方总金额
                    voucher.MCreditTotal && voucher.MCreditTotal != 0 && $(entryCredit, $header).text(mMath.toMoneyFormat(voucher.MCreditTotal));
                    //加入
                    tbody.append($header);
                    //
                    for (var i = 0; voucher.MIsMulti && i < voucher.MVoucherModuleEntrys.length ; i++) {
                        //
                        var entry = voucher.MVoucherModuleEntrys[i];
                        //
                        var $entry = $("." + voucherEntryDemoClass).clone();
                        //处理边框的问题
                        if (i == 0) {
                            $(entrySelect, $entry).attr("colspan", 3).attr("rowspan", voucher.MVoucherModuleEntrys.length).addClass("fcv-no-hover");
                        }
                        else {
                            //选择的
                            $(entrySelect, $entry).remove();
                        }
                        //描述
                        $(voucherDescription, $entry).remove();
                        //快速码
                        $(voucherFastcode, $entry).remove();
                        //去除class
                        $entry.removeClass(voucherEntryDemoClass).removeClass(hide).addClass(voucherEntryClass).attr("topRow", "0").attr("mitemid", voucher.MItemID).hide();
                        //摘要
                        $(entryExplanation, $entry).text(mText.encode(entry.MExplanation || ""));
                        //科目
                        $(entryAccount, $entry).text(mText.encode(entry.MAccountName || ""));
                        //
                        if (entry.MAccountModel && entry.MAccountModel.MItemID && entry.MAccountModel.MCurrencyDataModel) {
                            //联系人
                            $(entryCurrency, $entry).html(checkType.getCurrencyHtml(entry.MAccountModel.MCurrencyDataModel, true));
                        }
                        else {
                            $(entryCurrency, $entry).html("&nbsp;");
                        }
                        //核算维度
                        if (entry.MAccountModel && entry.MAccountModel.MItemID && entry.MAccountModel.MCheckGroupValueModel) {
                            //联系人
                            $(entryCheckGroup, $entry).html(entry.MAccountID ? checkType.getCheckGroupValueHtml(entry.MAccountModel.MCheckGroupValueModel, entry.MAccountModel.MCheckGroupModel) : "");
                        }
                        else {
                            $(entryCheckGroup, $entry).html("&nbsp;");
                        }
                        //借方金额
                        entry.MDC == 1 && $(entryDebit, $entry).text(mMath.toMoneyFormat(entry.MDebit));
                        //贷方金额
                        entry.MDC == -1 && entry.MCredit && entry.MCredit != 0 && $(entryCredit, $entry).text(mMath.toMoneyFormat(entry.MCredit));
                        //不需要操作按钮
                        $(voucherOperation, $entry).remove();
                        //加入
                        tbody.append($entry);
                        //
                    }
                }
                else {
                    //如果有多行
                    fastCodeTd.append(mText.encode(voucher.MFastCode));
                    //描述
                    $(voucherDescription, $header).text(mText.encode(voucher.MDescription));
                    //
                    var entry = voucher.MVoucherModuleEntrys[0]
                    //摘要
                    $(entryExplanation, $header).text(mText.encode(entry.MExplanation || ""));
                    //科目
                    $(entryAccount, $header).text(mText.encode(entry.MAccountName || ""));
                    //显示科目
                    if (entry.MAccountModel && entry.MAccountModel.MItemID && entry.MAccountModel.MCurrencyDataModel) {
                        //联系人
                        $(entryCurrency, $header).html(checkType.getCurrencyHtml(entry.MAccountModel.MCurrencyDataModel, true));
                    }
                    else {
                        $(entryCurrency, $header).html("&nbsp;");
                    }
                    //核算维度
                    if (entry.MAccountModel && entry.MAccountModel.MItemID && entry.MAccountModel.MCheckGroupValueModel) {
                        //联系人
                        $(entryCheckGroup, $header).html(entry.MAccountID ? checkType.getCheckGroupValueHtml(entry.MAccountModel.MCheckGroupValueModel, entry.MAccountModel.MCheckGroupModel) : "");
                    }
                    else {
                        $(entryCheckGroup, $header).html("&nbsp;");
                    }
                    //借方总金额
                    entry.MDC == 1 && $(entryDebit, $header).text(mMath.toMoneyFormat(entry.MDebit));
                    //贷方总金额
                    entry.MDC == -1 && $(entryCredit, $header).text(mMath.toMoneyFormat(entry.MCredit));
                    //加入
                    tbody.append($header);
                }
            };
            //添加空行
            this.getNoRecordRow = function () {
                //
                return "<tr class='" + noRecordRowClass + "'><td class='dv-norecord-td' colspan='10'>" + HtmlLang.Write(LangModule.Common, "NoRecords", "No Records.") + "</td></tr>";
            };
            //
            this.getVoucherByItemID = function (itemID) {
                for (var i = 0; i < pageVoucherModuleData.length ; i++) {
                    if (pageVoucherModuleData[i].MItemID == itemID) {
                        return pageVoucherModuleData[i];
                    }
                }
            };
            //初始化表格里面的事件
            this.initTableEvent = function () {
                //展开与合起
                $(expandButton, voucherTable).off("click").on("click", function () {
                    //只展开本条对应的
                    that.expandVoucherModule(this, $(this).attr("mitemid"));
                });
                //
                $(voucherDelete, voucherTable).off("click").on("click", function () {
                    //找到行
                    var $tr = $(this).parent().parent().parent();
                    //
                    that.deleteVoucherModules($tr.attr("MItemID"), that.refresh);
                });
                //编辑
                $(voucherEdit, voucherTable).off("click").on("click", function () {
                    //找到行
                    var $tr = $(this).parent().parent().parent();
                    //
                    that.editVoucherModule($tr.attr("MItemID"), $tr.attr("MFastCode"));
                });
            };
            //初始化entrydiv的高度
            this.initEntryDiv = function () {
                //计算高度
                $(entryDiv).height($(window).height() - $(entryDiv).offset().top - $(_pager1).height() - 4);
                //其滚动的时候隐藏悬浮层
                $(entryDiv).scroll(function () {
                    //显示虚线
                    if (this.scrollTop > 0) {
                        $(this).addClass(dashedLineClassName);
                    }
                    else {
                        $(this).removeClass(dashedLineClassName);
                    }
                });
                //底部打印按钮的位置
                $(functionDiv).css({
                    top: ($(window).height() - $(functionDiv).height() - 3) + "px"
                });
            };
            //初始化按钮的点击事件
            this.initButtonEvent = function () {
                //新增
                $(btnNew).off("click").on("click", that.newVoucherModule);
                //导入
                $(btnImport).off("click").on("click", that.importVoucherModule);
                //打印
                $(btnPrint).off("click").on("click", function () {
                    var itemIds = that.getSelectedVoucherModule(true, operatePrint);
                    if (itemIds.length > 0) {
                        that.printVoucher(itemIds.join());
                    }
                    else {
                        $.mDialog.warning(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
                    }
                });
                //导出
                $(btnExport).off("click").on("click", that.exportVoucherModule);
                //删除
                $(btnDelete).off("click").on("click", function () {
                    //
                    that.deleteVoucherModules(null, that.refresh);
                });
                //搜索
                $(btnSearch).off("click").on("click", that.refresh);
                //清空
                $(btnClear).off("click").on("click", that.clear);
                //
                $(btnCheckall).off("click").on("click", that.selectAll);
                //展开
                $(expandAllButton).off("click").on("click", function () {
                    //找到表格中所有的展开按钮，触发点击事件
                    $("." + expandedClass + ":visible", voucherTable).trigger("click");
                });
                //收起
                $(collapseAllButton).off("click").on("click", function () {
                    //找到表格中所有的展开按钮，触发点击事件
                    $("." + collapsedClass + ":visible", voucherTable).trigger("click");
                });
            };
            //展开和收起凭证模板
            this.expandVoucherModule = function (button, mitemid) {
                //
                var rows = $("tr[topRow=0]", voucherTable).filter(function () {
                    //
                    return $(this).attr("mitemid") == mitemid;
                });
                //
                if ($(button).hasClass(collapsedClass)) {
                    //展开的操作
                    $(button).removeClass(collapsedClass).addClass(expandedClass);
                    //把详细行显示出来
                    rows.show();
                }
                else {
                    //收起的操作
                    $(button).removeClass(expandedClass).addClass(collapsedClass);
                    //把详细行显示出来
                    rows.hide();
                }
            };
            //全选与取消全选
            this.selectAll = function () {
                //判断是否勾选
                ($(btnCheckall).attr("checked") == "checked") ? $(voucherSelect).attr("checked", true) : $(voucherSelect).attr("checked", false);
            };
            //
            this.refresh = function () {
                //重新获取数据
                that.getVoucherModuleListData(that.initVoucherModuleList);
                //首页也要更新
                home.updateModuleInfo();
            };
            //打印凭证
            this.printVoucherModule = function (itemID) {
                //调用首页
                home.printVoucherModule(itemID);
            },
            //导出
            this.exportVoucherModule = function () {
                //调用home的方法
                home.exportVoucherModule(that.getSearchParam());
            };
            //编辑凭证
            this.editVoucherModule = function (itemID, fastcode) {
                //跳转到编辑页面 
                mTab.addOrUpdate(voucherModuleLang + "[" + fastcode + "]", editVoucherModuleUrl + "?MItemID=" + itemID + "&IsModule=1", false, true, true);
            };
            //新建凭证
            this.newVoucherModule = function () {
                //跳转到编辑页面 
                mTab.addOrUpdate(newVoucherLang, editVoucherModuleUrl + "?IsModule=1", true, true, true);
            };
            //清除查找条件
            this.clear = function () {
                //
                $(txtKeyword).val("");
                //
                $(txtCode).val("");
                //
                $(txtType).combobox("setValue", "");
                //
                $(txtType).combobox("setText", "");
            };
            //重置分页参数
            this.resetPager = function () {
                //初始化的时候，要把参数都置位原始值
                totalCount = 0;
                //
                pageIndex = 1;
                //
                pageSize = FCHome.pageSize;
            };
            //初始化翻页事件
            this.initPagerEvent = function () {
                //调用easyui组件
                $(_pager1).pagination({
                    total: totalCount,
                    pageSize: pageSize,
                    onChangePageSize: function (size) {
                        pageSize = FCHome.pageSize = size;
                        that.getVoucherModuleListData(that.initVoucherModuleList);
                    },
                    onRefresh: function (number, size) {
                        pageIndex = number;
                        pageSize = FCHome.pageSize = size;
                        that.getVoucherModuleListData(that.initVoucherModuleList);
                    },
                    onSelectPage: function (number, size) {
                        pageIndex = number;
                        pageSize = FCHome.pageSize = size;
                        that.getVoucherModuleListData(that.initVoucherModuleList);
                    }
                });
            };
            //
            this.reset = function () {

            };
            //
            this.init = function () {
                //重置参数
                that.resetPager();
                //初始化domValue
                that.initDomValue();
                //获取凭证数据
                that.getVoucherModuleListData(that.initVoucherModuleList);
                //初始化事件
                that.initButtonEvent();
                //初始化分页事件
                that.initPagerEvent();
            };

            // #凭证模板 开始

            //导出凭证模板
            this.exportVoucherModule = function (params) {
                //
                mWindow.reload(exportVoucherModuleUrl + escape($.toJSON(params)));
                //显示正在导出
                mDialog.message(exportLang);
            };

            //导入凭证模板
            this.importVoucherModule = function () {
                ImportBase.showImportBox('/BD/Import/Import/VoucherModule', importLang, 900, 520);
            };
            //
            this.initDomValue = function () {
                //是否审核
                $(txtType).combobox({
                    width: 110,
                    textField: 'text',
                    valueField: 'value',
                    data: [
                         {
                             text: HtmlLang.Write(LangModule.Common, 'SingleRow', "单行"),
                             value: 0
                         },
                        {
                            text: HtmlLang.Write(LangModule.Common, 'MultiRow', "多行"),
                            value: 1
                        }

                    ],
                    onLoadSuccess: function () {
                        //初始化为空
                        $(txtType).combobox("setValue", "");
                        $(txtType).combobox("setText", "");
                    }
                });
            }
            //打印凭证模板
            this.printVoucherModule = function (itemID) {
                //
                var param = { MItemID: itemID };
                //弹出框
                Megi.dialog({
                    title: voucherModuleLang,
                    top: window.pageYOffset || document.documentElement.scrollTop,
                    width: 1060,
                    height: 560,
                    href: printVoucherModuleUrl + escape($.toJSON(param))
                });
            };

            // #凭证模板 结束
        };
        //返回
        return FCVoucherModuleList;
    })();
    //
    window.FCVoucherModuleList = FCVoucherModuleList;
})()