/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.business.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPTransactionDetail = /** @class */ (function () {
    function FPTransactionDetail() {
        this.checkAll = ".fp-record-checkbox-all";
        this.checkBox = ".fp-record-checkbox:visible";
        this.btnDeleteFP = "#btnDeleteFP";
        this.transactionPartial = ".fp-transaction-detail";
        this.transactionTable = ".fp-transaction-table";
        this.pager = ".fp-transaction-detail-pager";
        this.viewVoucherUrl = "/GL/GLVoucher/GLVoucherEdit";
        this.viewFapiao = ".fp-view-fapiao";
        this.importID = "";
        this.page = 1;
        this.rows = 20;
        this.type = 0;
        this.orgVersion = "";
    }
    /**
     * 初始化事件
     */
    FPTransactionDetail.prototype.init = function (imprtID, type, orgVersion) {
        this.importID = imprtID;
        this.type = type;
        this.orgVersion = orgVersion;
        this.home = new FPReconcileHome();
        this.loadData();
        this.initEvent();
    };
    /**
     * 获取数据
     *add by 锦友 2018-04-12：11:05:00，
     *添加了一个是否删除的参数，表示是否是删除成功后，重新加载数据，
     *以做判断当从接口返回的发票数据为0时，是否提示发票已被删除，是否查看
     *isDelete:1表示是，0不是
     */
    FPTransactionDetail.prototype.loadData = function (isDelete) {
        var _this = this;
        if (isDelete === void 0) { isDelete = 0; }
        //获取过滤
        var filter = {
            page: this.page,
            rows: this.rows,
            MFapiaoCategory: this.type,
            MImportID: this.importID
        };
        this.home.getTransactionList(filter, function (data) {
            //如果没有
            if (data.total === 0 && isDelete != 1) {
                return mDialog.alert(HtmlLang.Write(LangModule.FP, "FapiaoNotExistsOrReplaced", "此次导入的发票已经被删除或者被后来导入的发票覆盖, 无法查看详情."), function () {
                    mDialog.close();
                });
            }
            _this.showData(data);
        });
    };
    /**
     * 展示数据到面板
     */
    FPTransactionDetail.prototype.showData = function (data) {
        var _this = this;
        $(this.transactionTable).datagrid({
            data: data.rows,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $(this.transactionPartial).width() - 20,
            height: $("body").height() - 80,
            columns: [[
                    {
                        field: 'MID', title: '<input type="checkbox" class="fp-record-checkbox-all"/>', fixwidth: 30, width: 30, align: 'left',
                        formatter: function (value, row) {
                            return "<div style='text-align:center'><input type='checkbox' data-msource='" + row.MSource + "' class='fp-record-checkbox' mid='" + row.MID + "' " + "/></div>";
                        }
                    },
                    {
                        field: 'MNumber', title: HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号"), width: 100, align: 'left', formatter: function (value, record) {
                            return "<a class='fp-view-fapiao' mid='" + record.MID + "'>" + value + "</a>";
                        }
                    },
                    {
                        field: 'MBizDate', title: HtmlLang.Write(LangModule.FP, "FapiaoDate", "开票日期"), width: 100, align: 'center', formatter: function (value) {
                            return mDate.format(value);
                        }
                    },
                    {
                        field: 'MContactName', title: HtmlLang.Write(LangModule.FP, "Company", "公司"), width: 300, align: 'left', formatter: function (value, record) {
                            return record.MInvoiceType == FPEnum.Sales ? record.MPContactName : record.MSContactName;
                        }
                    },
                    {
                        field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 100, align: 'right', formatter: function (value) {
                            return mMath.toMoneyFormat(value);
                        }
                    },
                    {
                        field: 'MVerifyType', hidden: this.type == FPEnum.Sales, title: HtmlLang.Write(LangModule.FP, "VerifyType", "认证状态"), width: 100, align: 'center', formatter: function (value) {
                            return _this.home.getVerifyTypeName(value);
                        }
                    },
                    {
                        field: 'MVerifyDate', hidden: this.type == FPEnum.Sales, title: HtmlLang.Write(LangModule.FP, "VerifyDate", "认证月份"), width: 100, align: 'center', formatter: function (value) {
                            var date = mDate.parse(value);
                            return date.getFullYear() <= 1901 ? "" : mDate.parse(value).format("yyyy-MM");
                        }
                    },
                    {
                        field: 'MStatus', title: HtmlLang.Write(LangModule.FP, "FapiaoStatus", "状态"), width: 100, align: 'center', formatter: function (value) {
                            return _this.home.getFapiaoStatusName(value, true);
                        }
                    },
                    //标准版显示勾兑状态
                    {
                        field: 'MReconcileStatus', title: HtmlLang.Write(LangModule.FP, "ReconcileStatus", "勾兑状态"), hidden: this.orgVersion == "1", width: 100, align: 'center', formatter: function (value) {
                            return _this.home.getReconcileStatusName(value, true);
                        }
                    },
                    //记账版显示凭证状态
                    {
                        field: 'MCodingStatus', title: HtmlLang.Write(LangModule.Bank, "VoucherStatus", "凭证状态"), hidden: this.orgVersion == "0", width: 100, align: 'center', formatter: function (value, record) {
                            ///add by 锦友 2018-04-05 19:39:00 保持和发票明细一致
                            //   return this.home.getCodingStatusName(value, true);
                            //如果有凭证，则优先显示凭证
                            if (record.MVoucherID) {
                                var voucherNumber = "GL-" + record.MVoucherNumber;
                                return "<a href='javascript:void(0)' onclick='mTab.addOrUpdate(\"" + voucherNumber + "\",\"" + _this.viewVoucherUrl + "?MItemID=" + record.MVoucherID + "\", false, true, true, true)'>" + voucherNumber + "</a>";
                            }
                            return record.MStatus == FPFapiaoStatusEnum.Obsolete ? "" : _this.home.getCodingStatusName(value, true);
                        }
                    }
                ]],
            onLoadSuccess: function () {
                //初始化分页控件
                _this.initPage(data.total);
                _this.initGridEvent();
                $(_this.transactionTable).datagrid("resize");
            }
        });
    };
    FPTransactionDetail.prototype.getCodingStatusTitle = function () {
        var title = HtmlLang.Write(LangModule.Bank, "VoucherStatus", "凭证状态");
        return title;
    };
    /**
     * 初始化表格里面的点击时间
     */
    FPTransactionDetail.prototype.initGridEvent = function () {
        var _this = this;
        $(this.viewFapiao).off("click").on("click", function (evt) {
            var mid = $(evt.srcElement || evt.target).attr("mid");
            _this.home.viewFapiao(mid, null);
        });
        //全选
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
    };
    /**
     * 初始化面板事件
     */
    FPTransactionDetail.prototype.initEvent = function () {
        var _this = this;
        /*
      删除发票
      */
        $(this.btnDeleteFP).off("click").on("click", function (evt) {
            var boxes = $(_this.checkBox + ":checked");
            var ids = [];
            var isExistAutoData = 0;
            boxes.each(function (index, elem) {
                ids.push($(elem).attr("mid"));
                var msource = $(elem).attr("data-msource");
                if (msource == "1") {
                    isExistAutoData = 1;
                    return false;
                }
            });
            ids = ids.distinct();
            if (ids.length == 0) {
                return mDialog.message(HtmlLang.Write(LangModule.FP, "PleaseSelectDelectFP", "请勾选需要删除的发票！"));
            }
            // 判断是否存在自动获取发票
            if (isExistAutoData == 1) {
                var f = mDialog.message(HtmlLang.Write(LangModule.FP, "FlagExistFapionData", "存在自动获取的发票，不能删除"));
                return false;
            }
            //弹出删除确认提醒
            mDialog.confirm(HtmlLang.Write(LangModule.FP, "AreYourSureDeleteFPData", "确认删除选中的发票吗？"), function () {
                var filter = {
                    MFapiaoIDs: ids
                };
                _this.home.deleteFapiaoByFapiaoIds(filter, function (data) {
                    if (data.Success) {
                        mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                        var pageNumber = _this.page;
                        if (pageNumber > 1) {
                            var currentPageItemCount = $(_this.transactionTable).datagrid('getRows').length;
                            if (currentPageItemCount == ids.length) {
                                _this.page = pageNumber - 1;
                            }
                        }
                        _this.loadData(1);
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
    FPTransactionDetail.prototype.initDom = function () {
    };
    /**
    * 初始化分页
    */
    FPTransactionDetail.prototype.initPage = function (total) {
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
    return FPTransactionDetail;
}());
//# sourceMappingURL=FPTransactionDetail.js.map