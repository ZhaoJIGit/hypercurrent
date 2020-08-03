

(function () {

    var FCFapiaoModuleList = (function () {
        //常量定义
        var pageIndex = 1;
        //单页显示的条数
        var pageSize = FCHome.pageSize;
        //总记录数
        var totalCount = 0;
        //当前页所有的凭证
        var pageFapiaoModuleData = [];
        //
        var frozenItemID = "";

        var fastCodeBody = ".fp-fastcode-body:visible";
        /*异步请求的一些url*/
        //获取凭证列表
        var getFapiaoListUrl = "/FC/FCHome/GetFapiaoModulePageList";
        //删除凭证模板
        var deleteFapiaoModuleUrl = "/FC/FCHome/DeleteFapiaoModules";

        var saveFastCodeBtn = ".fp-fastcode-save:visible";

        var fastCodeCloseBtn = ".fp-fastcode-close";

        var fastCodePartialDiv = "#fcFapiaoFastCodeDiv";

        var fastCodeDemoDiv = ".fp-fastcode-partial.demo";

        var baseData = null;

        var fastCodeList = [];
        //
        var newFapiaoLang = HtmlLang.Write(LangModule.FP, "newfapiaomodule", "新建发票模板");
        //
        var fapiaoModuleLang = HtmlLang.Write(LangModule.FP, "fapiaomodule", "发票模板");
        //
        var operationFail = HtmlLang.Write(LangModule.Common, "operationfail", "Operation Failded!");
        //
        var sureToDelete = HtmlLang.Write(LangModule.Common, "AreYouSureToDelete", "Are you sure to delete");
        /*按钮的selector*/
        //新建
        var btnNew = ".fcf-new-button";
        //删除
        var btnDelete = ".fcf-delete-button";
        //查找
        var btnSearch = ".fcf-search-button";
        //清除
        var btnClear = ".fcf-clear-button";
        //全选
        var btnCheckall = ".fcf-checkall-input";
        //关键字
        var txtKeyword = ".fcf-keyword-input";
        //凭证编号
        var txtCode = ".fcf-code-input";

        var entryDiv = ".fcf-entry-div";
        //
        var txtType = "#txtType";
        //表格
        var fapiaoTable = ".fcf-fapiao-table";
        var fapiaoOperation = ".fcf-fapiao-operation";
        var operateTop = ".fcf-operate-top";
        var entryList = ".fcf-entry-list";

        var noRecordRowClass = "fcf-norecord-row";
        var fapiaoTableHeader = ".fcf-fapiao-table-header";
        var fapiaoHeaderDemoClass = "fcf-fapiao-header-demo";
        var fapiaoEntryDemoClass = "fcf-fapiao-entry-demo";
        var fapiaoHeaderClass = "fcf-fapiao-header";
        var fapiaoEntryClass = "fcf-fapiao-entry";
        var fapiaoEmptyClass = "fcf-fapiao-empty";
        var fapiaoDescription = ".fcf-entry-description";
        var fapiaoMerItem = ".fcf-entry-meritem";
        var fapiaoFastcode = ".fcf-entry-fastcode";
        var fapiaoSelect = ".fcf-entry-select input[type='checkbox']";

        var fapiaoDiv = ".fcf-entry-div";

        var entryExplanation = ".fcf-entry-explanation";
        var entryDescription = ".fcf-entry-description";
        var entryMerItem = ".fcf-entry-meritem";
        var entryDebitAccount = ".fcf-entry-debitaccount";
        var entryCreditAccount = ".fcf-entry-creditaccount";
        var entryTaxAccount = ".fcf-entry-taxaccount"

        var _pager1 = ".fcf-pagenation-div";

        var fapiaoHeaderDiv = ".fcf-fapiao-col-div";
        //隐藏
        var hide = "fcf-hide";
        //编辑
        var fapiaoEdit = ".fcf-fapiao-edit";
        //删除
        var fapiaoDelete = ".fcf-fapiao-delete";
        //
        var FCFapiaoModuleList = function () {
            //
            var that = this;
            //调用home的方法
            var home = new FCHome();
            //
            var checkType = new GLCheckType();
            //获取凭证数据
            this.getFapiaoModuleListData = function (func) {
                //
                var code = $(txtCode).val();
                //
                mAjax.post(getFapiaoListUrl, {
                    filter: {
                        page: pageIndex,
                        rows: pageSize,
                        KeyWord: $(txtKeyword).val(),
                        MIsMulti: $(txtType).combobox("getValue"),
                        MFastCode: code ? code : ""
                    }
                }, function (data) {
                    var fapiaos = data;
                    //调用回调函数
                    func && $.isFunction(func) && func(fapiaos);
                }, "", true);
            };
            //删除凭证模板
            this.deleteFapiaoModules = function (moduleID, func) {
                //
                var pkIDs = moduleID ? [moduleID] : that.getSelectedFapiao();
                //必须有勾选在做删除
                if (pkIDs.length > 0) {
                    //先提醒用户是否确定删除
                    mDialog.confirm(sureToDelete, function () {
                        //
                        mAjax.submit(deleteFapiaoModuleUrl, {
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
            this.getSelectedFapiao = function () {
                //
                var selectedIds = [];
                //获取
                $(fapiaoSelect).not(":hidden").filter(function () { return $(this).attr("checked") == "checked"; }).each(function () {
                    //
                    var selectedID = $(this).closest("tr").attr("mid");
                    //
                    !!selectedID && selectedIds.push(selectedID);
                });
                //返回是id的列表还是凭证的列表
                return selectedIds;
            };
            //初始化凭证列表
            this.initFapiaoModuleList = function (data) {

                fastCodeList = data || [];

                if (baseData == null) {
                    baseData = that.getBaseData();
                }

                var tracks = [baseData.MTrackItem1, baseData.MTrackItem2, baseData.MTrackItem3, baseData.MTrackItem4, baseData.MTrackItem5]

                var trackCount = 0;
                //获取跟踪项
                for (var i = 0; i < tracks.length; i++) {

                    if (tracks[i] && tracks[i].MCheckTypeName && tracks[i].MCheckTypeName.length > 0) {
                        //界面显示跟踪项
                        $(".fcf-track-header" + (i + 1)).show();
                        $(".fcf-entry-track" + (i + 1)).show();
                        //跟踪项标题
                        $(".fcf-track-header" + (i + 1)).html(mText.encode(tracks[i].MCheckTypeName));
                        trackCount++;
                    }
                }


                //
                totalCount = data.total;
                //
                var rows = data.rows;
                //
                pageFapiaoModuleData = rows;
                //总共的行数
                that.initPagerEvent();
                //获取table里面的body
                var $body = $("tbody", fapiaoTable);
                //先移除里面的没有记录的行
                $("." + noRecordRowClass).remove();
                //先清除里面的凭证
                $body.find("tr").not("tr[class*='-demo']").remove();
                //如果没有凭证
                if (rows.length == 0) {
                    //加入空行
                    $body.append(that.getNoRecordRow(trackCount + 9));
                }
                else {
                    //遍历每一个凭证
                    for (var i = 0; i < rows.length ; i++) {
                        //当前凭证
                        var fapiaoModule = rows[i];
                        //一行一行加入到table中
                        that.createVouherModuleHtml(fapiaoModule, $body, i == (rows.length - 1));
                    }
                }
                //初始化高度
                that.initEntryDiv();
                //初始化事件
                that.initTableEvent();
                //
                $(fapiaoTable).each(function () {
                    //
                    $(this).mTableResize({
                        forceFit: false
                    });
                });
            };

            this.getFastCodeByID = function (id) {

                for (var i = 0; i < fastCodeList.length ; i++) {
                    if (fastCodeList[i].MID == id)
                        return fastCodeList[i];
                }

                return null;
            }

            //创建一个凭证的html并加到表格里面
            this.createVouherModuleHtml = function (fapiao, tbody, isLast) {
                //
                var html = "";
                //第一行显示的是一个概况
                var $header = $("." + fapiaoEntryDemoClass).clone();
                //去除class
                $header.removeClass(fapiaoEntryDemoClass).removeClass(hide).addClass(fapiaoEntryClass).show().attr("MID", fapiao.MID);
                //加入
                tbody.append($header);
                //如果本行需要显示checkbox
                $(fapiaoSelect, $header).show().attr("MID", fapiao.MID);
                //快速码
                var fastCodeTd = $(fapiaoFastcode, $header);
                //如果有多行
                fastCodeTd.append(mText.encode(fapiao.MFastCode));
                //描述
                $(fapiaoDescription, $header).text(mText.encode(fapiao.MDescription || ""));
                //摘要
                $(entryExplanation, $header).text(mText.encode(fapiao.MExplanation || ""));
                //商品项目
                $(entryMerItem, $header).text(mText.encode(fapiao.MMerItemIDName || ""));

                fapiao.MTrackItem1Name && $(".fcf-entry-track1", $header).text(mText.encode(fapiao.MTrackItem1Name));
                fapiao.MTrackItem2Name && $(".fcf-entry-track2", $header).text(mText.encode(fapiao.MTrackItem2Name));
                fapiao.MTrackItem3Name && $(".fcf-entry-track3", $header).text(mText.encode(fapiao.MTrackItem3Name));
                fapiao.MTrackItem4Name && $(".fcf-entry-track4", $header).text(mText.encode(fapiao.MTrackItem4Name));
                fapiao.MTrackItem5Name && $(".fcf-entry-track5", $header).text(mText.encode(fapiao.MTrackItem5Name));

                //借方总金额
                $(entryDebitAccount, $header).text(mText.encode(fapiao.MDebitAccountName || ""));
                //贷方总金额
                $(entryCreditAccount, $header).text(mText.encode(fapiao.MCreditAccountName || ""));
                //贷方总金额
                $(entryTaxAccount, $header).text(mText.encode(fapiao.MTaxAccountName || ""));
            };
            //添加空行
            this.getNoRecordRow = function (colspan) {
                //
                return "<tr class='" + noRecordRowClass + "'><td class='dv-norecord-td' colspan='" + colspan + "'>" + HtmlLang.Write(LangModule.Common, "NoRecords", "No Records.") + "</td></tr>";
            };
            //
            this.getFapiaoByItemID = function (id) {
                for (var i = 0; i < pageFapiaoModuleData.length ; i++) {
                    if (pageFapiaoModuleData[i].MID == id) {
                        return pageFapiaoModuleData[i];
                    }
                }
            };
            //初始化表格里面的事件
            this.initTableEvent = function () {
                //
                $(fapiaoDelete, fapiaoTable).off("click").on("click", function () {
                    //找到行
                    var $tr = $(this).parent().parent().parent();
                    //
                    that.deleteFapiaoModules($tr.attr("MID"), that.refresh);
                });
                //编辑
                $(fapiaoEdit, fapiaoTable).off("click").on("click", function () {
                    //找到行
                    var $tr = $(this).parent().parent().parent();
                    //
                    that.editFapiaoModule($tr.attr("MID"));
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
            };
            //初始化按钮的点击事件
            this.initButtonEvent = function () {
                //新增
                $(btnNew).off("click").on("click", function () {
                    that.editFapiaoModule();
                });
                //删除
                $(btnDelete).off("click").on("click", function () {
                    //
                    that.deleteFapiaoModules(null, that.refresh);
                });
                //搜索
                $(btnSearch).off("click").on("click", that.refresh);
                //清空
                $(btnClear).off("click").on("click", that.clear);
                //
                $(btnCheckall).off("click").on("click", that.selectAll);

            };
            //全选与取消全选
            this.selectAll = function () {
                //判断是否勾选
                ($(btnCheckall).attr("checked") == "checked") ? $(fapiaoSelect).attr("checked", true) : $(fapiaoSelect).attr("checked", false);
            };
            //
            this.refresh = function () {

                baseData = that.getBaseData();
                //重新获取数据
                that.getFapiaoModuleListData(that.initFapiaoModuleList);
                //首页也要更新
                home.updateModuleInfo();
            };
            //编辑凭证
            this.editFapiaoModule = function (itemID) {

                var fastCode = that.getFapiaoByItemID(itemID);


                $(fastCodePartialDiv).remove();

                var demo = $(fastCodeDemoDiv);

                var partial = demo.clone().removeClass("demo").attr("id", fastCodePartialDiv.replace('#', ''));

                partial.insertAfter(demo);

                mDialog.show({
                    mContent: "id:fcFapiaoFastCodeDiv",
                    mShowbg: true,
                    mHeight: 300,
                    mWidth: 600,
                    mShowTitle: false,
                    mMax: false,
                    mCloseCallback: function () {

                        that.refresh();
                    },
                    mOpenCallback: function () {
                        that.initFapiaoFastCodeEdit(fastCode);
                        that.initFastCodeEvent();
                    }
                });

            };

            this.getBaseData = function () {
                var result = null;
                mAjax.post("/FP/FPHome/GetBaseData", {}, function (data) {

                    result = data;
                }, null, true, false);

                return result;
            }

            this.initFapiaoFastCodeEdit = function (fastCode) {

                fastCode = fastCode || {};

                if (baseData == null) {
                    baseData = that.getBaseData();
                }

                var partial = $(fastCodeBody);

                !!fastCode && $("#fastCodeID", partial).val(fastCode.MID);
                //初始化快速码
                $("input[field='MFastCode']", partial).validatebox({
                    required: true
                });

                //初始化描述
                $("input[field='MDescription']", partial).validatebox({
                    required: false
                });

                //摘要
                $("input[field='MExplanation']", partial).validatebox({
                    required: false
                });

                !!fastCode && $("input[field='MFastCode']", partial).val(fastCode.MFastCode);
                !!fastCode && $("input[field='MDescription']", partial).val(fastCode.MDescription);
                !!fastCode && $("input[field='MExplanation']", partial).val(fastCode.MExplanation);

                //商品项目
                $("input[field='MMerItemID']", partial).combobox({
                    textField: "MText",
                    valueField: "MItemID",
                    srcRequired: true,
                    height: 25,
                    hideItemKey: "MIsActive",
                    hideItemValue: false,
                    autoSizePanel: true,
                    data: baseData.MMerItem,
                    onLoadSuccess: function () {
                        !!fastCode && !!fastCode.MMerItemID && $("input[field='MMerItemID']", partial).combobox("setValue", fastCode.MMerItemID);
                    }
                });

                //科目
                $("input[field='MDebitAccount']", partial).combobox(
                    {
                        textField: "MFullName",
                        valueField: "MItemID",
                        srcRequired: true,
                        height: 25,
                        hideItemKey: "MIsActive",
                        hideItemValue: false,
                        autoSizePanel: true,
                        data: baseData.MAccount,
                        onLoadSuccess: function () {
                            !!fastCode && !!fastCode.MDebitAccount && $("input[field='MDebitAccount']", partial).combobox('setValue', fastCode.MDebitAccount);
                        }
                    });

                //科目
                $("input[field='MCreditAccount']", partial).combobox(
                    {
                        textField: "MFullName",
                        valueField: "MItemID",
                        srcRequired: true,
                        height: 25,
                        hideItemKey: "MIsActive",
                        hideItemValue: false,
                        autoSizePanel: true,
                        data: baseData.MAccount,
                        onLoadSuccess: function () {
                            !!fastCode && !!fastCode.MCreditAccount && $("input[field='MCreditAccount']", partial).combobox('setValue', fastCode.MCreditAccount);
                        }
                    });


                //科目
                $("input[field='MTaxAccount']", partial).combobox(
                    {
                        textField: "MFullName",
                        valueField: "MItemID",
                        srcRequired: true,
                        height: 25,
                        hideItemKey: "MIsActive",
                        hideItemValue: false,
                        autoSizePanel: true,
                        data: baseData.MAccount,
                        onLoadSuccess: function () {
                            !!fastCode && !!fastCode.MTaxAccount && $("input[field='MTaxAccount']", partial).combobox('setValue', fastCode.MTaxAccount);
                        }
                    });


                //跟踪项
                var tracks = [baseData.MTrackItem1, baseData.MTrackItem2, baseData.MTrackItem3, baseData.MTrackItem4, baseData.MTrackItem5]

                for (var i = 0; i < tracks.length; i++) {

                    if (!tracks[i]) continue;

                    var $textTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(2);

                    var $valueTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(3);

                    if (tracks[i].MCheckTypeName && tracks[i].MCheckTypeName.length > 0) {
                        $textTd.text(mText.encode(tracks[i].MCheckTypeName));
                        var $input = $valueTd.find("input");
                        var defaultValue = mObject.getPropertyValue(fastCode, tracks[i].MCheckTypeColumnName);
                        $input.show().attr("field", tracks[i].MCheckTypeColumnName).combobox(
                            {
                                textField: "text",
                                valueField: "id",
                                srcRequired: true,
                                height: 25,
                                hideItemKey: "MIsActive",
                                hideItemValue: false,
                                autoSizePanel: true,
                                data: tracks[i].MDataList,
                                onLoadSuccess: function () {
                                    !!defaultValue && $input.combobox("setValue", defaultValue);
                                }
                            });
                    }
                }

                //取消红色的提示
                !!fastCode && $(".validatebox-invalid", partial).removeClass("validatebox-invalid");
            }

            /**
            * 保存发票快速码设置
            */
            this.saveFastCode = function (model, callback) {

                mAjax.submit("/FC/FCHome/SaveFapiaoModule", { model: model }, function (data) {

                    $.isFunction(callback) && callback(data);
                });

            }

            /**
             * 获取用户设置的快速码信息
             */
            this.getFastCode = function () {


                var partial = $(fastCodeBody);

                var fastCode = {
                    MID: $("#fastCodeID", partial).val(),
                    MFastCode: $("input[field='MFastCode']", partial).val(),
                    MDescription: $("input[field='MDescription']", partial).val(),
                    MExplanation: $("input[field='MExplanation']", partial).val(),
                    MMerItemID: $("input[field='MMerItemID']", partial).combobox("getValue"),
                    MMerItemIDName: $("input[field='MMerItemID']", partial).combobox("getText"),
                    MDebitAccount: $("input[field='MDebitAccount']", partial).combobox("getValue"),
                    MCreditAccount: $("input[field='MCreditAccount']", partial).combobox("getValue"),
                    MTaxAccount: $("input[field='MTaxAccount']", partial).combobox("getValue"),

                };

                //跟踪项
                var tracks = [baseData.MTrackItem1, baseData.MTrackItem2, baseData.MTrackItem3, baseData.MTrackItem4, baseData.MTrackItem5]

                for (var i = 0; i < tracks.length; i++) {

                    if (!tracks[i]) continue;

                    var $valueTd = $(partial).find("table").find("tr").eq(i + 1).find("td").eq(3);

                    if (tracks[i].MCheckTypeName && tracks[i].MCheckTypeName.length > 0) {

                        mObject.setPropertyValue(fastCode, tracks[i].MCheckTypeColumnName, $valueTd.find("input[field='" + tracks[i].MCheckTypeColumnName + "']").combobox("getValue"));
                        mObject.setPropertyValue(fastCode, tracks[i].MCheckTypeColumnName + "Name", $valueTd.find("input[field='" + tracks[i].MCheckTypeColumnName + "']").combobox("getText"));
                    }
                }

                return fastCode;
            }

            /**
            * 初始化快速码事件
            */
            this.initFastCodeEvent = function (elem) {

                $(saveFastCodeBtn).off("click").on("click", function () {
                    var fastCode = that.getFastCode();

                    if (!!fastCode.MFastCode) {

                        that.saveFastCode(fastCode, function (data) {

                            if (data.Success) {

                                mDialog.message(HtmlLang.Write(LangModule.Common, "SaveSuccessful", "保存成功!"));

                                mDialog.close();
                            }
                            else {
                                mDialog.message(HtmlLang.Write(LangModule.FP, "SetFailed", "保存失败!"));
                            }
                        });
                    }
                    else {

                        mDialog.message(HtmlLang.Write(LangModule.FP, "FastCodeIsEmpty", "快速码不可为空!"));
                    }
                });

                $(fastCodeCloseBtn).off("click").on("click", function () {

                    mDialog.close();
                });
            }


            //新建凭证
            this.newFapiaoModule = function () {

                that.editFapiaoModule();

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
                    onSelectPage: function (number, size) {
                        pageIndex = number;
                        pageSize = FCHome.pageSize = size;
                        that.getFapiaoModuleListData(that.initFapiaoModuleList);
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
                that.getFapiaoModuleListData(that.initFapiaoModuleList);
                //初始化事件
                that.initButtonEvent();
                //初始化分页事件
                that.initPagerEvent();
            };

            // #凭证模板 开始

            //导出凭证模板
            this.exportFapiaoModule = function (params) {
                //
                mWindow.reload(exportFapiaoModuleUrl + escape($.toJSON(params)));
                //显示正在导出
                mDialog.message(exportLang);
            };

            //导入凭证模板
            this.importFapiaoModule = function () {
                ImportBase.showImportBox('/BD/Import/Import/FapiaoModule', importLang, 900, 520);
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
            this.printFapiaoModule = function (itemID) {
                //
                var param = { MItemID: itemID };
                //弹出框
                Megi.dialog({
                    title: fapiaoModuleLang,
                    top: window.pageYOffset || document.documentElement.scrollTop,
                    width: 1060,
                    height: 560,
                    href: printFapiaoModuleUrl + escape($.toJSON(param))
                });
            };

            // #凭证模板 结束
        };
        //返回
        return FCFapiaoModuleList;
    })();
    //
    window.FCFapiaoModuleList = FCFapiaoModuleList;
})()