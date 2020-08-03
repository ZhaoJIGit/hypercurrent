

(function () {

    var FCCashCodingModuleList = (function () {
        //常量定义
        var pageIndex = 1;
        //单页显示的条数
        var pageSize = FCHome.pageSize;
        //总记录数
        var totalCount = 0;
        //当前页所有的凭证
        var pageCashCodingData = [];
        /*异步请求的一些url*/
        //获取快速码列表
        var getCashCodingModuleListUrl = "/FC/FCCashCodingModule/GetCashCodingByPageList";
        //编辑快速码
        var editCashCodingModuleUrl = "/FC/FCCashCodingModule/FCCashCodingEdit";
        //删除快速码
        var deleteCashCodingModuleUrl = "/FC/FCCashCodingModule/DeleteCashCodingModule";
        //
        var sureToDelete = HtmlLang.Write(LangModule.Common, "AreYouSureToDelete", "Are you sure to delete");
        /*按钮的selector*/
        //新建
        var btnNew = ".fcv-new-button";
        //删除
        var btnDelete = ".fcv-delete-button";
        //查找
        var btnSearch = ".fcv-cashcodingsearch-button";
        //清除
        var btnClear = ".fcv-cashcodingclear-button";
        //全选
        var btnCheckall = ".fcv-checkalls-input";
        //关键字
        var txtKeyword = ".fcv-cashcodingkeyword-input";
        //快速码
        var txtCode = ".fcv-cashcodingcode-input";
        //表格
        var cashcodingTable = ".fcv-cashcoding-table";
        //
        var noRecordRowClass = "fcv-norecord-row";
        //
        var cashcodingEntryDemoClass = "fcv-cashcoding-entry-demo";
        //
        var cashcodingEntryClass = "fcv-cashcoding-entry";
        //
        var cashcodingDescription = ".fcv-entry-cachcodinmref";
        //
        var cashcodingMName = ".fcv-entry-cachcodinmname";
        //
        var cachcodingMDesc = ".fcv-entry-cachcodinmdesc";
        //
        var cashcodingFastcode = ".fcv-entry-cachcodinfastcode";

        var cashcodingTax = ".fcv-entry-cachcodintax";
        //
        var cashcodingSelect = ".fcv-entry-cachcodingselect input[type='checkbox']";
        //
        var entryDiv = ".fcv-entry-cashcoding-div";
        //
        var entryAccount = ".fcv-entry-cachcodinaccount";
        //
        var entryContact = ".fcv-entry-cachcodincontact";
        //跟踪项目1-5
        var entryTrack1 = ".fcv-entry-track1";
        var entryTrack2 = ".fcv-entry-track2";
        var entryTrack3 = ".fcv-entry-track3";
        var entryTrack4 = ".fcv-entry-track4";
        var entryTrack5 = ".fcv-entry-track5";

        var _pager1 = ".fcv-pagenations-div";
        //隐藏
        var hide = "fcv-hide";
        //编辑
        var cashcodingEdit = ".fcv-cashcoding-edit";
        //删除
        var cashcodingDelete = ".fcv-cashcoding-delete";
        //
        var FCCashCodingModuleList = function () {

            var that = this;
            //调用home的方法
            var home = new FCHome();
            //跟踪项数据源
            var trackDataScoure = null;
            //列的个数 默认的列数为th个数减去5个跟踪项
            var columnCount = 0;
            //获取CashCoding数据
            this.getCashCodingModuleListData = function (func) {
                columnCount = $(cashcodingTable).find("th").length - 5;
                //
                var code = $(txtCode).val();
                //
                mAjax.post(getCashCodingModuleListUrl, {
                    page: pageIndex,
                    rows: pageSize,
                    KeyWord: $(txtKeyword).val(),
                    MCode: code ? code : "",
                    Sort: "MCode"
                }, function (data) {

                    if (data.Success) {
                        var cashcodings = data.Data;
                        //调用回调函数
                        func && $.isFunction(func) && func(cashcodings);
                    }
                }, "", true);
            };
            //删除
            this.deleteCashCodingModules = function (moduleID, func) {
                //
                var pkIDs = moduleID ? [moduleID] : that.getSelectedCashCoding();
                //必须有勾选在做删除
                if (pkIDs.length > 0) {
                    //先提醒用户是否确定删除
                    mDialog.confirm(sureToDelete, function () {
                        //
                        mAjax.submit(deleteCashCodingModuleUrl, {
                            pkIDs: pkIDs
                        }, function (data) {
                            //
                            if (data.Success) {
                                //提醒用户删除成功
                                mDialog.message(LangKey.DeleteSuccessfully);
                                //整个页面更新
                                $.isFunction(func) && func(data.Data);
                            } else {
                                //提醒用户删除成功
                                mDialog.error(HtmlLang.Write(LangModule.Common, "DeleteFailed", "Delete Failed!"));
                            }
                        }, "", true);
                    });
                } else {
                    //没有选择行
                    mDialog.message(HtmlLang.Write(LangModule.Common, "NotSelectedAnyRow", "Please select a row!"));
                }
            }
            //获取勾选上的列表
            this.getSelectedCashCoding = function () {
                //
                var selectedIds = [];
                //获取
                $(cashcodingSelect).not(":hidden").filter(function () { return $(this).attr("checked") == "checked"; }).each(function () {
                    //
                    var selectedID = $(this).attr("MID");
                    //
                    selectedIds.push(selectedID);
                });
                //返回是id的列表还是列表
                return selectedIds;
            };
            //初始化列表
            this.initCashCodingModuleList = function (data) {

                //获取跟踪项
                var ds = mData.getNameValueTrackDataList();
                trackDataScoure = ds;
                //列的个数9+跟踪项
                columnCount = columnCount + ds.length;
                //跟踪项有数据
                if (ds != null && ds.length > 0) {
                    for (var i = 0; i < ds.length; i++) {
                        //界面显示跟踪项
                        $(".vlc-track-header" + (i + 1)).css('display', 'table-cell');
                        $(".fcv-entry-track" + (i + 1)).css('display', 'table-cell');
                        //跟踪项标题
                        $(".vlc-track-header" + (i + 1)).html(mText.encode(ds[i].MName));
                    }
                }

                totalCount = data.total;

                var rows = data.rows;
                //
                pageCashCodingData = rows;
                //总共的行数
                that.initPagerEvent();
                //获取table里面的body
                var $body = $("tbody", cashcodingTable);
                //先移除里面的没有记录的行
                $("." + noRecordRowClass).remove();
                //先清除里面的数据
                $body.find("tr").not("tr[class*='-demo']").remove();
                //如果没有数据
                if (rows.length == 0) {
                    //加入空行
                    $body.append(that.getNoRecordRow());
                }
                else {
                    //遍历
                    for (var i = 0; i < rows.length ; i++) {
                        //当前cashcoding
                        var cashcodingModule = rows[i];
                        //一行一行加入到table中
                        that.createCashCodingModuleHtml(cashcodingModule, $body, i == (rows.length - 1));
                    }
                }


                //初始化高度
                that.initEntryDiv();
                //初始化事件
                that.initTableEvent();
                //
                $(cashcodingTable).each(function () {
                    $(this).mTableResize({
                        forceFit: false
                    });
                });
            };
            //创建一个凭证的html并加到表格里面
            this.createCashCodingModuleHtml = function (cashcoding, tbody, isLast) {
                //
                var html = "";
                //第一行显示的是一个概况
                var $header = $("." + cashcodingEntryDemoClass).clone();
                //去除class
                $header.removeClass(cashcodingEntryDemoClass).removeClass(hide).addClass(cashcodingEntryClass).show().attr("MID", cashcoding.MID).attr("topRow", 1).attr("MCode", cashcoding.MCode);
                //如果本行需要显示checkbox
                $(cashcodingSelect, $header).show().attr("MID", cashcoding.MID);
                //快速码
                $(cashcodingFastcode, $header).text(cashcoding.MCode);
                //联系人
                $(entryContact, $header).text(cashcoding.MContactName == null ? "" : cashcoding.MContactName);
                //备注
                $(cashcodingDescription, $header).text(cashcoding.MRef == null ? "" : cashcoding.MRef);
                //名字
                $(cashcodingMName, $header).text(cashcoding.MName == null ? "" : cashcoding.MName);
                //描述
                $(cachcodingMDesc, $header).text(cashcoding.MDesc == null ? "" : cashcoding.MDesc);
                //科目
                $(entryAccount, $header).text(cashcoding.MAccountName == null ? "" : cashcoding.MAccountName);
                //税率
                $(cashcodingTax, $header).text(cashcoding.MTaxName == null ? "" : cashcoding.MTaxName);
                //跟踪项1-5
                $(entryTrack1, $header).text(that.GetMtrackItemName(cashcoding.MTrackItem1));
                $(entryTrack2, $header).text(that.GetMtrackItemName(cashcoding.MTrackItem2));
                $(entryTrack3, $header).text(that.GetMtrackItemName(cashcoding.MTrackItem3));
                $(entryTrack4, $header).text(that.GetMtrackItemName(cashcoding.MTrackItem4));
                $(entryTrack5, $header).text(that.GetMtrackItemName(cashcoding.MTrackItem5));
                //加入
                tbody.append($header);
            };
            //根据跟踪项id获取跟踪项名
            this.GetMtrackItemName = function (track) {
                //没有数据返回
                if (track == null || track == "") {
                    return "";
                }
                //获取跟踪项数据
                var tds = trackDataScoure;
                if (tds != null && tds.length > 0) {
                    //遍历跟踪项
                    for (var i = 0; i < tds.length; i++) {
                        //获取孩子节点
                        var ds = tds[i].MChildren;
                        //遍历孩子节点
                        for (var j = 0; j < ds.length; j++) {
                            if (ds[j].MValue == track) {
                                //值相等返回名字
                                return ds[j].MName;
                            }
                        }
                    }
                }
                return "";
            };
            //添加空行
            this.getNoRecordRow = function () {
                //
                return "<tr class='" + noRecordRowClass + "'><td class='dv-norecord-td' colspan='" + columnCount + "'>" + HtmlLang.Write(LangModule.Common, "NoRecords", "No Records.") + "</td></tr>";
            };
            //初始化表格里面的事件
            this.initTableEvent = function () {
                //
                $(cashcodingDelete, cashcodingTable).off("click").on("click", function () {
                    //找到行
                    var $tr = $(this).parent().parent().parent();
                    //
                    that.deleteCashCodingModules($tr.attr("MID"), that.refresh);
                });
                //编辑
                $(cashcodingEdit, cashcodingTable).off("click").on("click", function () {
                    //找到行
                    var $tr = $(this).parent().parent().parent();
                    //
                    that.newCashCodingModule($tr.attr("MID"), that.init, HtmlLang.Write(LangModule.GL, "EditTemplateFastCode", "Edit Template's Fast Code"));
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
                    that.newCashCodingModule("", that.init, HtmlLang.Write(LangModule.GL, "NewTemplateFastCode", "New Template's Fast Code"));
                });
                //删除
                $(btnDelete).off("click").on("click", function () {
                    that.deleteCashCodingModules(null, that.refresh);
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
                ($(btnCheckall).attr("checked") == "checked") ? $(cashcodingSelect).attr("checked", true) : $(cashcodingSelect).attr("checked", false);
            };
            //
            this.refresh = function () {
                //重新获取数据
                that.getCashCodingModuleListData(that.initCashCodingModuleList);
                //首页也要更新
                home.updateModuleInfo();
            };
            //新建凭证
            this.newCashCodingModule = function (id, onCloseFns, mTitle) {
                //标题
                var title = mTitle;
                //默认高度
                var height = 325;
                //跟踪项的个数
                var trackCount = columnCount - ($(cashcodingTable).find("th").length - 5);
                if ($("#haveGLPermission").val()) {
                    height = 420;
                } else {
                    //设置高度
                    height = height + 50 * (trackCount / 2);
                }
               
                //弹窗
                $.mDialog.show({
                    mTitle: title,
                    mWidth: 400,
                    mHeight: height,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + editCashCodingModuleUrl + "?mid=" + id,
                    mCloseCallback: [onCloseFns]
                });
            };
            //清除查找条件
            this.clear = function () {
                //
                $(txtKeyword).val("");
                //
                $(txtCode).val("");
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
                        that.getCashCodingModuleListData(that.initCashCodingModuleList);
                    },
                    onRefresh: function (number, size) {
                        pageIndex = number;
                        pageSize = FCHome.pageSize = size;
                        that.getCashCodingModuleListData(that.initCashCodingModuleList);
                    },
                    onSelectPage: function (number, size) {
                        pageIndex = number;
                        pageSize = FCHome.pageSize = size;
                        that.getCashCodingModuleListData(that.initCashCodingModuleList);
                    }
                });
            };
            //
            this.init = function () {
                //重置参数
                that.resetPager();
                //获取数据
                that.getCashCodingModuleListData(that.initCashCodingModuleList);
                //初始化事件
                that.initButtonEvent();
                //初始化分页事件
                that.initPagerEvent();
                //更新表头数据
                home.updateModuleInfo();
            };

            // # 结束
        };
        //返回
        return FCCashCodingModuleList;
    })();
    //
    window.FCCashCodingModuleList = FCCashCodingModuleList;
})()