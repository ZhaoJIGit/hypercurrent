/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPTransaction = /** @class */ (function () {
    function FPTransaction() {
        this.transactionPartial = ".fp-transaction-partial";
        this.transactionTable = ".fp-transaction-table";
        this.transactionBody = ".fp-transaction-body";
        this.pager = ".fp-transaction-partial-pager";
        this.checkBox = ".fp-record-checkbox:visible";
        this.viewFapiao = ".fp-view-fapiao";
        this.markButtonDiv = "#divMark";
        this.markDiv = ".fp-transaction-mark";
        this.checkAll = ".fp-record-checkbox-all";
        this.markCodingButtonDiv = "#divCodingMark";
        this.markCodingDiv = ".fp-coding-mark";
        this.reconcileStatusSelect = ".fp-transation-reconcile";
        this.codingStatusSelect = ".fp-transation-coding";
        this.keywordInput = ".fp-transaction-keyword";
        this.queryButton = "#bthSearchTransaction";
        this.viewVoucherUrl = "/GL/GLVoucher/GLVoucherEdit";
        this.markTop = "#aMarkAsRec";
        this.markCodingTop = "#aCodingMarkAsRec";
        this.fpStatusTop = "#btnChangeFpStatus";
        this.fapiaoStatusSelect = ".fp-transation-status";
        this.fapiaoCertStatusSelect = ".fp-transation-certstatus";
        this.fapiaoCertPeriodSelect = ".fp-transation-certperiod";
        this.clearFilterButton = "#btnClear";
        this.exportButton = "#btnExport";
        this.psContactTitle = ".fp-pscontact-title";
        this.bizDateTitle = ".fp-bizdate-title";
        this.fapiaoNumberTitle = ".fp-fapiaonumber-title";
        this.btnDeleteFP = "#btnDeleteFP";
        this.btnShowSearchDiv = "#bthShowSearchDiv";
        this.fpTransactionQuery = ".fp-transaction-query";
        this.btnQueryClose = "#aTransactionQueryClose";
        this.changeStatusTo = ".fp-status-mark";
        this.changeVerifyStatus = "#btnChangeVerifyStatus";
        this.editVerifyStatusUrl = "/FP/FPHome/FPEditVerifyStatus";
        this.spanChangeVerifyStatus = "#btnChangeVerifyStatus span .l-btn-text";
        this.sort = "MBizDate";
        this.order = "asc";
        this.sortType = [
            {
                name: "MBizDate",
                value: "desc"
            },
            {
                name: "MPSContactName",
                value: "asc"
            },
            {
                name: "MFapiaoNumber",
                value: "asc"
            }
        ];
        this.allColumns = [];
        this.page = 1;
        this.rows = 20;
    }
    /**
     * 初始化事件
     */
    FPTransaction.prototype.init = function () {
        this.home = new FPReconcileHome();
        this.initDom();
        this.initEvent();
    };
    /**
     * 刷新页面重试
     */
    FPTransaction.prototype.refreshData = function (keepPage) {
        if (!keepPage)
            this.page = 1;
        $(this.pager).pagination("select", this.page);
    };
    /**
     * 获取数据
     */
    FPTransaction.prototype.loadData = function (dates) {
        var _this = this;
        /// add by 锦友 20180524 15:54:00
        ///修复切换到发票明细把发票速记的选择显示字段弹出层给移除了
        $(".tooltip-top").hide();
        var filter = this.getQueryFilter(dates);
        this.home.getTransactionList(filter, function (data) {
            if (data.rows.length === 0) {
                filter.page = filter.page > 1 ? filter.page - 1 : 1;
                _this.home.getTransactionList(filter, function (data) {
                    _this.showData(data);
                });
            }
            else {
                _this.showData(data);
            }
        });
    };
    /**
     *
     * @param dates
     */
    FPTransaction.prototype.getQueryFilter = function (dates) {
        dates = dates || this.home.getPickedDate();
        //获取过滤
        var filter = {
            page: this.page,
            rows: this.rows,
            MStartDate: dates[0],
            MEndDate: dates[1],
            Sort: this.sort,
            Order: this.order,
            MKeyword: $(this.keywordInput).val(),
            MCodingStatus: $(this.codingStatusSelect).length > 0 ? $(this.codingStatusSelect).combobox("getValue") : "",
            MReconcileStatus: $(this.reconcileStatusSelect).length > 0 ? $(this.reconcileStatusSelect).combobox("getValue") : "",
            MFapiaoCategory: this.home.getType(),
            MTotalAmount: $(this.keywordInput).val().toNullableNumber(),
            MStatus: $(this.fapiaoStatusSelect).length > 0 ? $(this.fapiaoStatusSelect).combobox("getValue") : "",
            MVerifyDate: $(this.fapiaoCertPeriodSelect).length > 0 ? $(this.fapiaoCertPeriodSelect).val() + "-01" : "1900-01-01 00:00:00"
        };
        var verifyStatusArray = $(this.fapiaoCertStatusSelect).length > 0 ? $(this.fapiaoCertStatusSelect).combotree("getValues") : [];
        if (verifyStatusArray.length > 0) {
            filter.MVerifyStatus = verifyStatusArray.join(',');
        }
        var selectedVerifyDate = $(this.fapiaoCertPeriodSelect).length > 0 ? $(this.fapiaoCertPeriodSelect).val() : "";
        if (selectedVerifyDate != "") {
            filter.MVerifyDate = selectedVerifyDate + "-01";
        }
        else {
            filter.MVerifyDate = "1900-01-01 00:00:00";
        }
        return filter;
    };
    FPTransaction.prototype.ResetFilter = function () {
        $(this.fapiaoStatusSelect).length > 0 ? $(this.fapiaoStatusSelect).combobox("setValue", "") : "";
        $(this.fapiaoCertStatusSelect).length > 0 ? $(this.fapiaoCertStatusSelect).combobox("setValue", "") : "";
        $(this.codingStatusSelect).length > 0 ? $(this.codingStatusSelect).combobox("setValue", "") : "";
        $(this.fapiaoCertPeriodSelect).length > 0 ? $("#txtPeriod").val("") : "";
        $(this.reconcileStatusSelect).length > 0 ? $(this.reconcileStatusSelect).combobox("setValue", "") : "";
        $(this.keywordInput).val("");
    };
    /**
     * 获取所有的列
     */
    FPTransaction.prototype.getAllColumns = function () {
        var _this = this;
        return [
            {
                field: 'MID', title: '<input type="checkbox" class="fp-record-checkbox-all"/>', fixwidth: 30, width: 30, align: 'left', formatter: function (value, record) {
                    return "<div style='text-align:center'><input type='checkbox' class='fp-record-checkbox' mid='" + record.MID + "' rstatus='" + record.MReconcileStatus + "' cstatus='" + record.MCodingStatus + "' data-msource ='" + record.MSource + "' fstatus='" + record.MStatus + "' fapiaotype='" + record.MType + "'/></div>";
                }
            },
            {
                field: 'MNumber', title: '<span class="fp-fapiaonumber-title" field="MFapiaoNumber">' + HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号") + "</div>", fixwidth: 100, width: 100, align: 'left', formatter: function (value, record) {
                    return "<a class='fp-view-fapiao' mid='" + record.MID + "'>" + (value || "") + "</a>";
                }
            },
            {
                field: 'MBizDate', title: '<span class="fp-bizdate-title" field="MBizDate">' + HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期") + "</div>", fixwidth: 100, width: 100, align: 'center', formatter: function (value) {
                    return mDate.format(value);
                }
            },
            {
                field: 'MContactName', title: '<span class="fp-pscontact-title" field="MPSContactName">' + HtmlLang.Write(LangModule.FP, "Company", "公司") + "</div>", fixwidth: 300, width: 300, align: 'left', formatter: function (value, record) {
                    return _this.home.getType() == FPEnum.Purchase ? record.MSContactName : record.MPContactName;
                }
            },
            {
                field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), fixwidth: 100, width: 100, align: 'right', formatter: function (value) {
                    return mMath.toMoneyFormat(value);
                }
            },
            {
                field: 'MVerifyType', hidden: this.home.getType() == FPEnum.Sales, title: HtmlLang.Write(LangModule.FP, "VerifyType", "认证状态"), fixwidth: 100, width: 100, align: 'center', formatter: function (value) {
                    return _this.home.getVerifyTypeName(value);
                }
            },
            {
                field: 'MVerifyDate', hidden: this.home.getType() == FPEnum.Sales, title: HtmlLang.Write(LangModule.FP, "VerifyDate", "认证月份"), fixwidth: 100, width: 100, align: 'center', formatter: function (value) {
                    var date = mDate.parse(value);
                    return date.getFullYear() <= 1901 ? "" : mDate.parse(value).format("yyyy-MM");
                }
            },
            {
                field: 'MStatus', title: HtmlLang.Write(LangModule.FP, "FapiaoStatus", "状态"), fixwidth: 100, width: 100, align: 'center', formatter: function (value) {
                    return _this.home.getFapiaoStatusName(value, true);
                }
            },
            {
                field: 'MReconcileStatus', title: HtmlLang.Write(LangModule.FP, "ReconcileStatus", "勾兑状态"), hidden: this.home.isSmartVersion(), fixwidth: 100, width: 100, align: 'center', formatter: function (value, record) {
                    return record.MStatus == FPFapiaoStatusEnum.Obsolete ? "" : _this.home.getReconcileStatusName(value, true);
                }
            },
            {
                field: 'MCodingStatus', title: HtmlLang.Write(LangModule.Bank, "VoucherStatus", "凭证状态"), hidden: !this.home.isSmartVersion(), fixwidth: 100, width: 100, align: 'center', formatter: function (value, record) {
                    //如果有凭证，则优先显示凭证
                    if (record.MVoucherID) {
                        var voucherNumber = "GL-" + record.MVoucherNumber;
                        return "<a href='###' onclick='mTab.addOrUpdate(\"" + voucherNumber + "\",\"" + _this.viewVoucherUrl + "?MItemID=" + record.MVoucherID + "\", false, true, true, true)'>" + voucherNumber + "</a>";
                    }
                    return record.MStatus == FPFapiaoStatusEnum.Obsolete ? "" : _this.home.getCodingStatusName(value, true);
                }
            },
        ];
    };
    /**
     * 展示数据到面板
     */
    FPTransaction.prototype.showData = function (data) {
        var _this = this;
        $(this.transactionPartial).show();
        this.allColumns = this.getAllColumns();
        $(this.transactionTable).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: this.home.getGridWith(),
            height: this.getHeight(),
            columns: [this.allColumns],
            checkOnSelect: false,
            selectOnCheck: false,
            onLoadSuccess: function () {
                //初始化分页控件
                _this.initPage(data.total);
                _this.initGridEvent();
                $(_this.transactionTable).datagrid("resizeColumn");
            }
        });
    };
    /**
     * 获取datagrid高度
     */
    FPTransaction.prototype.getHeight = function () {
        if ($(this.fpTransactionQuery).is(":visible")) {
            return $(this.transactionPartial).height() - 170;
        }
        else
            return $(this.transactionPartial).height() - 45;
    };
    /**
     * 初始化表格里面的点击时间
     */
    FPTransaction.prototype.initGridEvent = function () {
        var _this = this;
        $(this.checkAll).off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            if (_this.home.isSmartVersion()) {
                if ($elem.is(":checked")) {
                    $(_this.checkBox + "[rstatus!='1']:not([disabled])").attr("checked", "checked");
                }
                else {
                    $(_this.checkBox + "[rstatus!='1']:not([disabled])").removeAttr("checked");
                }
            }
            else {
                if ($elem.is(":checked")) {
                    $(_this.checkBox + "[cstatus!='1']:not([disabled])").attr("checked", "checked");
                }
                else {
                    $(_this.checkBox + "[cstatus!='1']:not([disabled])").removeAttr("checked");
                }
            }
        });
        $(this.checkBox).off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            if (_this.home.isSmartVersion()) {
                if ($elem.is(":checked")) {
                    if ($(_this.checkBox + "[rstatus!='1']:not([disabled]):checked").length == $(_this.checkBox + "[rstatus!='1']:not([disabled])").length) {
                        $(_this.checkAll).attr("checked", "checked");
                    }
                }
                else {
                    $(_this.checkAll).removeAttr("checked");
                }
            }
            else {
                if ($elem.is(":checked")) {
                    if ($(_this.checkBox + "[cstatus!='1']:not([disabled]):checked").length == $(_this.checkBox + "[cstatus!='1']:not([disabled])").length) {
                        $(_this.checkAll).attr("checked", "checked");
                    }
                }
                else {
                    $(_this.checkAll).removeAttr("checked");
                }
            }
        });
        $(this.viewFapiao).off("click").on("click", function (evt) {
            var mid = $(evt.srcElement || evt.target).attr("mid");
            _this.home.viewFapiao(mid, null);
        });
        //点击排序
        $(this.bizDateTitle + "," + this.fapiaoNumberTitle + "," + this.psContactTitle).off("click").on("click", function (evt) {
            _this.sortColumn($(evt.target || evt.srcElement));
        }).tooltip({
            content: HtmlLang.Write(LangModule.FP, "Click2SortColumn", "点击进行排序"),
            position: "top"
        });
    };
    /**
     * 获取排序类型
     * @param field
     */
    FPTransaction.prototype.getSortByFeild = function (field) {
        for (var i = 0; i < this.sortType.length; i++) {
            if (this.sortType[i].name == field)
                return this.sortType[i];
        }
        return null;
    };
    /**
     * 按照列进行排序
     * @param ele
     */
    FPTransaction.prototype.sortColumn = function (ele) {
        var field = ele.attr("field");
        var type = this.getSortByFeild(field);
        type.value = type.value == "desc" ? "asc" : "desc";
        var category = this.home.getType();
        this.sort = field == "MFapiaoNumber" ? "MNumber" : field;
        this.sort = this.sort == "MPSContactName" ? (category == 0 ? "MPContactName" : "MSContactName") : this.sort;
        this.order = type.value;
        this.refreshData(true);
    };
    /**
     * 初始化面板事件
     */
    FPTransaction.prototype.initEvent = function () {
        var _this = this;
        //初始化修改认证状态的宽度
        $(this.spanChangeVerifyStatus).css("width", "140px");
        $(this.btnShowSearchDiv).off("click").on("click", function () {
            $(_this.btnShowSearchDiv).hide();
            $(_this.fpTransactionQuery).show();
            $(_this.transactionTable).datagrid('resize', {
                height: $(_this.transactionPartial).height() - 170
            });
        });
        $(this.btnQueryClose).off("click").on("click", function () {
            $(_this.fpTransactionQuery).hide();
            $(_this.btnShowSearchDiv).show();
            $(_this.transactionTable).datagrid('resize', {
                height: $(_this.transactionPartial).height() - 45
            });
        });
        $(this.queryButton).off("click").on("click", function () {
            _this.refreshData();
        });
        /*
        * 下拉按钮点击事件,触发时应只触发当前按钮下.m-btn-line样式的click事件
        * 如果页面存在多个.m-btn-line不触发当前按钮下的会有bug
        */
        $(this.markTop).off("click").on("click", function (evt) {
            $(_this.markTop + " .m-btn-line").trigger("click");
        });
        $(this.markCodingTop).off("click").on("click", function (evt) {
            $(_this.markCodingTop + " .m-btn-line").trigger("click");
        });
        $(this.fpStatusTop).off("click").on("click", function (evt) {
            $(_this.fpStatusTop + " .m-btn-line").trigger("click");
        });
        //标记
        $(this.markCodingDiv).off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            if (!$elem.hasClass(_this.markCodingDiv.trimStart('.'))) {
                $elem = $elem.parent(_this.markCodingDiv);
            }
            var status = ($elem.attr("status"));
            var boxes = $(_this.checkBox + ":checked[cstatus='" + (status == "0" ? 2 : 0) + "']");
            var ids = [];
            boxes.each(function (index, elem) {
                ids.push($(elem).attr("mid"));
            });
            ids = ids.distinct();
            if (ids.length == 0) {
                return mDialog.message(HtmlLang.Write(LangModule.FP, "PleaseSelectFapiaos2BeMarkCoding", "请勾选需要标记的发票!"));
            }
            var filter = {
                MFapiaoIDs: ids,
                MCodingStatus: status
            };
            _this.home.saveCodingStatus(filter, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                    _this.refreshData(true);
                }
                else {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                }
            });
        });
        $(this.markDiv).off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            if (!$elem.hasClass(_this.markDiv.trimStart('.'))) {
                $elem = $elem.parent(_this.markDiv);
            }
            var status = ($elem.attr("status"));
            var boxes = $(_this.checkBox + ":checked[rstatus='" + (status == "0" ? 2 : 0) + "']");
            var ids = [];
            boxes.each(function (index, elem) {
                ids.push($(elem).attr("mid"));
            });
            ids = ids.distinct();
            var filter = {
                MFapiaoIDs: ids,
                MReconcileStatus: status
            };
            _this.home.setReconcileStatus(filter, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                    _this.refreshData(true);
                }
                else {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                }
            });
        });
        //点击修改发票状态
        $(this.changeStatusTo).off("click").on("click", function (evt) {
            //获取校验结果
            var result = _this.getCheckDataResult();
            if (!result.success) {
                mDialog.message(result.message);
                return;
            }
            // 判断是否存在自动获取发票
            if (result.isExistAutoData) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FlagExistAutoData", "存在自动获取的发票，不能修改!"));
                return;
            }
            //获取要修改的状态
            var $elem = $(evt.srcElement || evt.target);
            if (!$elem.hasClass(_this.changeStatusTo.trimStart('.'))) {
                $elem = $elem.parent(_this.changeStatusTo);
            }
            var fstatus = $elem.attr("status");
            //记账版存在已生成凭证发票不能修改状态
            if (_this.home.isSmartVersion() && result.isExistReconciledVoucher) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FlagExistVoucherData", "存在已生成凭证的发票,不能修改发票状态!"));
                return;
            }
            //存在红冲发票修改为正常,不允许修改，提示
            if (result.isExistRedFlush && fstatus === FPFapiaoStatusEnum.Normal.toString()) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FlagExistRedData", "存在红冲状态的发票,不能修改为正常状态!"));
                return;
            }
            //设置要传的对象
            var filter = {
                MFapiaoIDs: result.objectIds,
                MStatus: fstatus
            };
            //修改发票状态
            _this.changeFaPiaoStatus(filter);
        });
        //点击修改发票认证状态
        $(this.changeVerifyStatus).off("click").on("click", function (evt) {
            //获取校验结果
            var result = _this.getCheckDataResult();
            if (!result.success) {
                mDialog.message(result.message);
                return;
            }
            // 判断是否存在自动获取发票
            if (result.isExistAutoData) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FlagExistAutoData", "存在自动获取的发票，不能修改!"));
                return;
            }
            //如存在普票时，提示语为：存在增值税普票，不能修改认证状态！
            if (result.isExistCommonFaPiao) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FlagCommonFapiaoType", "存在增值税普票，不能修改!"));
                return;
            }
            //如存在发票状态不是正常状态（红冲状态除外）时，提示语为：存在发票状态不是正常状态的发票，不能修改认证状态！
            if (result.isExistFaPiaoStatusIsNormal) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FlagNotNormalData", "存在发票状态不是正常状态的发票，不能修改！"));
                return;
            }
            //设置认证状态
            mDialog.show({
                mTitle: HtmlLang.Write(LangModule.FP, "ButtonsModifyFPRZStatus", "修改认证状态"),
                mDrag: "mBoxTitle",
                mShowbg: true,
                mContent: "iframe:" + _this.editVerifyStatusUrl + "?selectIds=" + result.objectIds,
                mCloseCallback: function () {
                    $.isFunction(_this.refreshData) && _this.refreshData(true);
                }
            });
        });
        $(this.clearFilterButton).off("click").on("click", function (evt) {
            _this.ResetFilter();
        });
        $(this.exportButton).off("click").on("click", function (evt) {
            var filter = _this.getQueryFilter();
            if (typeof filter.MTotalAmount != "number") {
                filter.MTotalAmount = null;
            }
            var filterJsonString = JSON.stringify(filter);
            var url = mObject.getPropertyValue(window, "escape")(filterJsonString);
            location.href = '/FP/FPHome/Export?jsonParam=' + url;
            mDialog.message(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });
        /*
         删除发票
       */
        $(this.btnDeleteFP).off("click").on("click", function (evt) {
            //获取校验结果
            var result = _this.getCheckDataResult();
            if (!result.success) {
                mDialog.message(result.message);
                return;
            }
            // 判断是否存在自动获取发票
            if (result.isExistAutoData) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "FlagFapiaoIsExistAutoData", "存在自动获取的发票，不能删除!"));
                return;
            }
            //弹出删除确认提醒
            mDialog.confirm(HtmlLang.Write(LangModule.FP, "AreYourSureDeleteFPData", "确认删除选中的发票吗？"), function () {
                // var idStrs = result.objectIds.join(',');
                var filter = {
                    MFapiaoIDs: result.objectIds
                };
                _this.home.deleteFapiaoByFapiaoIds(filter, function (data) {
                    if (data.Success) {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                        var pageNumber = _this.page;
                        if (pageNumber > 1) {
                            var currentPageItemCount = $(_this.transactionTable).datagrid('getRows').length;
                            if (currentPageItemCount == result.objectIds.length) {
                                _this.page = pageNumber - 1;
                            }
                        }
                        _this.refreshData(true);
                    }
                    else {
                        if (data.Message != null && data.Message != "") {
                            mDialog.message(data.Message);
                        }
                        else {
                            mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                        }
                    }
                });
            });
        });
    };
    /**
     * 初始化元素  高度 宽度 滚动等
     */
    FPTransaction.prototype.initDom = function () {
        $(this.transactionPartial).height($("body").height() - 155);
        $(this.pager).css({
            top: ($("body").height() - 30) + "px"
        });
        this.home.initCodingStatusCombo($(this.codingStatusSelect));
        this.home.initReconcileStatusCombo($(this.reconcileStatusSelect));
        this.home.initFapiaoStatusCombo($(this.fapiaoStatusSelect));
        this.home.initFapiaoCertStatusCombo($(this.fapiaoCertStatusSelect));
    };
    /**
     * 处理行宽度
     */
    FPTransaction.prototype.resizeColumnWidth = function () {
        var allCheckoxCell = $(this.checkAll).closest(".datagrid-cell");
        var prefix = allCheckoxCell.attr("class").replace("datagrid-cell", "").trim().replace("MID", "");
        var total = 0;
        var totalWidth = this.home.getGridWith() - 20;
        //先计算出总的宽度
        for (var i = 0; i < this.allColumns.length; i++) {
            if (this.allColumns[i].hidden)
                continue;
            total += this.allColumns[i].fixwidth;
        }
        var html = "";
        for (var i = 0; i < this.allColumns.length; i++) {
            if (this.allColumns[i].hidden)
                continue;
            //有padding
            var w = +this.allColumns[i].fixwidth / total * totalWidth - 10;
            html += "." + prefix + this.allColumns[i].field + "{ width:" + w + "px }" + "\n";
        }
        html += "";
        $("style[easyui]", this.transactionPartial).html(html);
    };
    /**
    * 初始化分页
    */
    FPTransaction.prototype.initPage = function (total) {
        var _this = this;
        //调用easyui组件
        $(this.pager).pagination({
            total: total,
            pageSize: this.rows,
            pageList: this.home.pageList,
            onSelectPage: function (page, size) {
                _this.page = page;
                _this.rows = size;
                _this.loadData();
            }
        });
    };
    /**
    *发票公共基本校验,校验通过返回对象
    */
    FPTransaction.prototype.getCheckDataResult = function () {
        var _this = this;
        var boxes = $(this.checkBox + ":checked");
        var ids = [];
        var message = "";
        //构造返回结果
        var result = {
            success: true,
            message: message,
            isExistAutoData: false,
            isExistReconciled: false,
            isExistReconciledVoucher: false,
            isExistRedFlush: false,
            objectIds: ids,
            isExistFaPiaoStatusIsNormal: false,
            isExistCommonFaPiao: false
        };
        boxes.each(function (index, elem) {
            ids.push($(elem).attr("mid"));
            //判断数据源是否为自动获取,已经赋值了不再判断
            var msource = $(elem).attr("data-msource");
            if (msource === FapiaoSourceEnum.Import.toString() && !result.isExistAutoData) {
                result.isExistAutoData = true;
            }
            //标准版判断是否存在已勾兑数据,已经赋值了不再判断
            var rstatus = $(elem).attr("rstatus");
            if (!_this.home.isSmartVersion() && rstatus === ReconcileStatusEnum.Reconciled.toString() && !result.isExistReconciled) {
                result.isExistReconciled = true;
            }
            //记账版判断是否存在已生成凭证状态,已经赋值了不再判断
            var cstatus = $(elem).attr("cstatus");
            if (_this.home.isSmartVersion() && cstatus === ReconcileStatusEnum.Reconciled.toString() && !result.isExistReconciledVoucher) {
                result.isExistReconciledVoucher = true;
            }
            //是否存在红冲发票,已经赋值了不再判断
            var fstatus = $(elem).attr("fstatus");
            if (fstatus === FPFapiaoStatusEnum.Credit.toString() && !result.isExistRedFlush) {
                result.isExistRedFlush = true;
            }
            //是否存在非正常状态发票(红冲除外),已经赋值了不再判断
            if (fstatus !== FPFapiaoStatusEnum.Credit.toString() && !result.isExistFaPiaoStatusIsNormal && fstatus !== FPFapiaoStatusEnum.Normal.toString()) {
                result.isExistFaPiaoStatusIsNormal = true;
            }
            //是否存在普票,已经赋值了不再判断
            var fapiaotype = $(elem).attr("fapiaotype");
            if (fapiaotype === FPFapiaoTypeEnum.Common.toString() && !result.isExistCommonFaPiao) {
                result.isExistCommonFaPiao = true;
            }
        });
        ids = ids.distinct();
        //是否勾选项
        if (ids.length === 0) {
            message = HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "请选择一个或多个项目！");
            result.success = false;
            result.message = message;
            result.objectIds = ids;
            return result;
        }
        result.success = true;
        result.objectIds = ids;
        return result;
    };
    /**
     * 修改发票状态
     * @param filter
     */
    FPTransaction.prototype.changeFaPiaoStatus = function (filter) {
        var _this = this;
        this.home.setFaPiaoStatus(filter, function (data) {
            if (data.Success && (data.ObjectID == null || data.ObjectID.length < 1)) {
                mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                _this.refreshData(true);
            }
            else {
                if (data.ObjectID) {
                    //弹出确认提醒
                    mDialog.confirm(data.Message, function () {
                        filter.ConfirmPara = data.ObjectID;
                        _this.home.setFaPiaoStatus(filter, function (data) {
                            if (data.Success) {
                                mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                                _this.refreshData(true);
                            }
                            else {
                                if (data.Message) {
                                    mDialog.message(data.Message);
                                }
                                else {
                                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                                }
                            }
                        });
                    });
                }
                else {
                    if (data.Message) {
                        mDialog.message(data.Message);
                    }
                    else {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationFailed", "操作失败!"));
                    }
                }
            }
        });
    };
    return FPTransaction;
}());
//# sourceMappingURL=FPTransaction.js.map