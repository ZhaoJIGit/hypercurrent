/// <reference path="../../Scripts/TS/jquery/jquery.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.d.ts" />
/// <reference path="../../Scripts/TS/jquery/jquery.megi.model.ts" />
var FPReconcileHome = /** @class */ (function () {
    function FPReconcileHome() {
        this.tabHeader = ".main-header";
        this.tabSelectedClass = "main-header-selected";
        this.tabPrefix = ".div-partial-";
        this.datePicker = ".daterangepicker-span";
        this.datePickerIcon = ".m-icon-date,.daterangepicker-arrow";
        this.typeInput = "#typeInput";
        this.manageButton = ".account-manage-button";
        this.importFapiaoBtn = "#importFapiao";
        this.fpTableBtn = "#btnOutputFPTable";
        this.currentIndex = 0;
        this.getReconcileListUrl = "/FP/FPHome/GetReconcileList";
        this.getStatementListUrl = "/FP/FPHome/GetStatementList";
        this.getTransactionListUrl = "/FP/FPHome/GetTransactionList";
        this.getFapiaoLogListUrl = "/FP/FPHome/GetFapiaoLogList";
        this.getFapiaoImportDetailUrl = "/FP/FPHome/GetFapiaoImportDetail";
        this.getCodingListUrl = "/FP/FPHome/GetCodingList";
        this.saveReconcileUrl = "/FP/FPHome/SaveReconcile";
        this.editReconcileUrl = "/FP/FPHome/FPEditReconcile";
        this.showTransactionDetailUrl = "/FP/FPHome/FPTransactionDetail";
        this.setReconcileStatusUrl = "/FP/FPHome/SetReconcileStatus";
        this.batchUpdateFpStatusUrl = "/FP/FPHome/BatchUpdateFPStatusByIds";
        this.batchUpdateFpVerifyTypeUrl = "/FP/FPHome/BatchUpdateFPVerifyType";
        this.removeReconcileUrl = "/FP/FPHome/RemoveReconcile";
        this.getCodingPageListUrl = "/FP/FPHome/GetCodingPageList";
        this.saveCodingStatusUrl = "/FP/FPHome/SaveCodingStatus";
        this.saveCodingUrl = "/FP/FPHome/SaveCoding";
        this.saveCodingByZipStringUrl = "/FP/FPHome/SaveCodingByZipString";
        this.getBaseDataUrl = "/FP/FPHome/GetBaseData";
        this.getCodingSettingUrl = "/FP/FPHome/GetCodingSetting";
        this.saveCodingSettingUrl = "/FP/FPHome/SaveCodingSetting";
        this.saveFastCodeUrl = "/FC/FCHome/SaveFapiaoModule";
        this.resetCodingDataUrl = "/FP/FPHome/ResetCodingData";
        this.saveCodingRowUrl = "/FP/FPHome/SaveCodingRow";
        this.deleteCodingRowUrl = "/FP/FPHome/DeleteCodingRow";
        this.viewFapiaoUrl = "/FP/FPHome/FPViewFapiao";
        this.deleteFapiaoUrl = "/FP/FPHome/DeleteFapiaoByFapiaoIds";
        this.deleteFPImportUrl = "/FP/FPHome/DeleteFPImportByImportIds";
        this.orgVersionInput = "#orgVersion";
        this.pageSize = 20;
        this.pageList = [10, 20, 50, 100, 200];
        this.type = 0;
    }
    //?????????
    FPReconcileHome.prototype.init = function (type, index) {
        this.type = type;
        this.initEvent();
        //?????????tab
        this.initTab(index);
    };
    /**
     * ???????????? ?????? ?????? ??????
     */
    FPReconcileHome.prototype.getType = function () {
        return +($(this.typeInput).val());
    };
    /**
     * ???????????????
     */
    FPReconcileHome.prototype.getGridWith = function () {
        return $("body").width() - 40;
    };
    /**
     * ??????????????????
     */
    FPReconcileHome.prototype.isSmartVersion = function () {
        return $(this.orgVersionInput).val() == "1";
    };
    /**
     * ??????
     */
    FPReconcileHome.prototype.orgVersion = function () {
        return $(this.orgVersionInput).val();
    };
    /**
     * ???????????????
     */
    FPReconcileHome.prototype.initEvent = function () {
        var _this = this;
        //????????????
        this.initDataPicker();
        $(this.manageButton).mTip({
            target: $(".account-manage-sub-div"),
            width: 150,
            parent: $(".account-manage-sub-div").parent()
        });
        //?????????
        $(this.fpTableBtn).off("click").on("click", function () {
            var title = HtmlLang.Write(LangModule.FP, "FPTable", "?????????");
            var url = '/FP/FPHome/FPHome?invoiceType=' + _this.getType() + '&index=1';
            mTab.addOrUpdate(title, url, true);
        });
        //????????????
        $(this.importFapiaoBtn).off("click").on("click", function () {
            IOImport.fpOpen(_this.getType(), function () {
                _this.showTab(ReconcileTabEnum.Statement);
            });
        });
    };
    /**
     * ?????????????????????
     */
    FPReconcileHome.prototype.initDataPicker = function () {
        var _this = this;
        //?????????????????????
        $(this.datePicker).mDaterangepicker({
            onChange: function () {
                _this.reloadData();
            }
        });
        var defaultStartDate = new Date(mDate.DateNow.getFullYear(), mDate.DateNow.getMonth(), 1).addMonths(-5);
        var defaultEndDate = new Date(mDate.DateNow.getFullYear(), mDate.DateNow.getMonth(), 1).addMonths(1).addDays(-1);
        $(this.datePicker).mDaterangepicker("setRangeDate", [defaultStartDate, defaultEndDate]);
        //??????????????????????????????????????????????????????????????????
        $(this.datePickerIcon).off("click").on("click", function (evt) {
            $(_this.datePicker).trigger("click");
            evt.stopPropagation();
        });
    };
    /**
     * ???????????????????????????
     */
    FPReconcileHome.prototype.getPickedDate = function () {
        var date = $(this.datePicker).mDaterangepicker("getRangeDate");
        if (date != null && date.length == 2) {
            date[1] = new Date(date[1].getFullYear(), date[1].getMonth(), date[1].getDate(), 23, 59, 59);
        }
        return date;
    };
    /**
     * ????????????
     * @param any
     */
    FPReconcileHome.prototype.cloneArray = function (arr) {
        var a = [];
        for (var i = 0; i < arr.length; i++) {
            if (typeof arr[i] == "string" || typeof arr[i] == "number") {
                a.push(arr[i]);
            }
            else {
                a.push(_.extend({}, arr[i]));
            }
        }
        return a;
    };
    /**
     * ??????????????????????????????
     * @param a
     * @param b
     */
    FPReconcileHome.prototype.arrayEqual = function (a, b) {
        if ((a || []) == (b || []))
            return true;
        if (a.length != b.length)
            return false;
        for (var i = 0; i < a.length; i++) {
            if (!this.equal(a[i], b[i]))
                return false;
        }
        return true;
    };
    /**
     * ??????????????????codingmodel????????????
     * @param a
     * @param b
     */
    FPReconcileHome.prototype.equal = function (a, b) {
        if (a == b || (a == null && b == null))
            return true;
        if ((a == undefined || a == null) && (b == undefined || b == null))
            return true;
        if ((typeof a == "string" || typeof b == "string")) {
            return ((a || "") == (b || ""));
        }
        var keys = Object.keys(a);
        for (var i = 0; i < keys.length; i++) {
            if (!this.equal(mObject.getPropertyValue(a, keys[i]), mObject.getPropertyValue(b, keys[i])))
                return false;
        }
        return true;
    };
    /**
     * ??????????????????
     */
    FPReconcileHome.prototype.reloadData = function () {
        //???????????????????????????tab??????????????????????????????????????????????????????????????????????????????????????????tab??????
        this.showTab(this.currentIndex);
    };
    //?????????Tab
    FPReconcileHome.prototype.initTab = function (index) {
        var _this = this;
        //?????????
        $(this.tabHeader + ">div").off("click").on("click", function (evt) {
            var index = +($(evt.srcElement || evt.target).attr("index"));
            _this.showTab(index);
        });
        this.showTab(index);
    };
    ;
    //??????tab
    FPReconcileHome.prototype.showTab = function (index) {
        this.currentIndex = index;
        var div = $(this.tabPrefix + index);
        div.show().siblings().hide();
        var tabDiv = $(this.tabHeader + ">div[index=" + index + "]");
        tabDiv.find("span").addClass(this.tabSelectedClass);
        tabDiv.siblings().find("span").removeClass(this.tabSelectedClass);
        var dates = this.getPickedDate();
        switch (index) {
            //????????????
            case ReconcileTabEnum.Reconcile:
                if (!this.reconcile) {
                    this.reconcile = new FPReconcile();
                    this.reconcile.page = 1;
                    this.reconcile.init();
                }
                this.reconcile.loadData(dates);
                break;
            case ReconcileTabEnum.Statement:
                if (!this.statement) {
                    this.statement = new FPStatement();
                    this.statement.page = 1;
                    this.statement.init();
                }
                this.statement.loadData(dates);
                break;
            case ReconcileTabEnum.Transaction:
                if (!this.transaction) {
                    this.transaction = new FPTransaction();
                    this.transaction.page = 1;
                    this.transaction.init();
                }
                this.transaction.loadData(dates);
                break;
            case ReconcileTabEnum.Coding:
                if (!this.coding) {
                    this.coding = new FPCoding();
                    this.coding.page = 1;
                    this.coding.init();
                }
                this.coding.loadData(dates, true);
                break;
            case ReconcileTabEnum.ImportLog:
                if (!this.importLog) {
                    this.importLog = new FPImportLog();
                    this.importLog.page = 1;
                    this.importLog.init();
                }
                this.importLog.loadData(dates);
                break;
        }
    };
    ;
    /**
     * ??????????????????
     * @param date
     */
    FPReconcileHome.prototype.getReconcileList = function (filter, callback) {
        mAjax.post(this.getReconcileListUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        }, null, true);
    };
    /**
     * ??????????????????
     * @param date
     */
    FPReconcileHome.prototype.getStatementList = function (filter, callback) {
        mAjax.post(this.getStatementListUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        }, null, true);
    };
    /**
     * ??????????????????
     * @param date
     */
    FPReconcileHome.prototype.getTransactionList = function (filter, callback) {
        mAjax.post(this.getTransactionListUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        }, null, true);
    };
    /**
     * ??????????????????
     * @param date
     */
    FPReconcileHome.prototype.getFapiaoLogList = function (filter, callback) {
        mAjax.post(this.getFapiaoLogListUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        }, null, true);
    };
    /**
     * ??????????????????
     * @param model
     */
    FPReconcileHome.prototype.saveReconcile = function (model, callback) {
        mAjax.submit(this.saveReconcileUrl, { model: model }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
     * ??????????????????
     * @param model
     * @param callback
     */
    FPReconcileHome.prototype.removeReconcile = function (model, callback) {
        mAjax.submit(this.removeReconcileUrl, { model: model }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /*
     *add by ?????? 20180420
     * ????????????
     * @param filter
     * @param callback
     */
    FPReconcileHome.prototype.deleteFapiaoByFapiaoIds = function (filter, callback) {
        mAjax.submit(this.deleteFapiaoUrl, { model: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /*
     *add by ?????? 20180420
     * ????????????????????????
     * @param filter
     * @param callback
     */
    FPReconcileHome.prototype.deleteFPImportByImportIds = function (filter, callback) {
        mAjax.submit(this.deleteFPImportUrl, { model: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
     * ?????????????????? ???????????????????????????
     * @param filter
     * @param callback
     */
    FPReconcileHome.prototype.setReconcileStatus = function (filter, callback) {
        if (!filter || !filter.MFapiaoIDs || filter.MFapiaoIDs.length == 0) {
            return mDialog.message(HtmlLang.Write(LangModule.FP, "PleaseSelectFapiao2Operate", "???????????????????????????"));
        }
        mAjax.submit(this.setReconcileStatusUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ??????????????????
    * @param filter
    * @param callback
    */
    FPReconcileHome.prototype.setFaPiaoStatus = function (filter, callback) {
        mAjax.submit(this.batchUpdateFpStatusUrl, { model: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ????????????????????????
    * @param filter
    * @param callback
    */
    FPReconcileHome.prototype.setFaPiaoVerifyStatus = function (data, callback) {
        mAjax.submit(this.batchUpdateFpVerifyTypeUrl, { model: data }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
     * ????????????????????????
     * @param tableID
     */
    FPReconcileHome.prototype.showEditReconcile = function (tableID, type, callback) {
        mDialog.show({
            mTitle: HtmlLang.Write(LangModule.FP, "FapiaoReconcile", "????????????"),
            mDrag: "mBoxTitle",
            mShowbg: true,
            mContent: "iframe:" + this.editReconcileUrl + "?MTableID=" + tableID + "&MFapiaoCategory=" + type,
            mCloseCallback: function () {
                $.isFunction(callback) && callback();
            }
        });
    };
    /**
     * ???????????????????????????????????????
     */
    FPReconcileHome.prototype.handleGridWidth = function (parent, callback) {
        var style = $(parent).find("style[easyui]");
        if (style.length > 0 && style.text().length == 0) {
            callback();
        }
    };
    /**
     *
     * @param input
     */
    FPReconcileHome.prototype.initReconcileStatusCombo = function (input) {
        var reconcileStatus = [
            { id: "0", text: HtmlLang.Write(LangModule.FP, "NotReconciled", "?????????") },
            { id: "1", text: HtmlLang.Write(LangModule.FP, "Reconciled", "?????????") },
            { id: "2", text: HtmlLang.Write(LangModule.FP, "NoReconcile", "????????????") }
        ];
        input.combobox({
            data: reconcileStatus,
            valueField: "id",
            textField: "text"
        });
    };
    FPReconcileHome.prototype.initFapiaoStatusCombo = function (selector) {
        var statusList = [
            { id: "1", text: HtmlLang.Write(LangModule.FP, "Normal", "??????") },
            { id: "0", text: HtmlLang.Write(LangModule.FP, "Obsolete", "??????") },
            { id: "4", text: HtmlLang.Write(LangModule.FP, "RedFlush", "??????") },
            { id: "2", text: HtmlLang.Write(LangModule.FP, "OutOfControl", "??????") },
            { id: "3", text: HtmlLang.Write(LangModule.FP, "Abnormal", "??????") }
        ];
        selector.combobox({
            data: statusList,
            valueField: "id",
            textField: "text"
        });
    };
    FPReconcileHome.prototype.initFapiaoCertStatusCombo = function (selector) {
        var data = [
            { id: "3", text: HtmlLang.Write(LangModule.FP, "NoNeedVerify", "????????????") },
            { id: "2", text: HtmlLang.Write(LangModule.FP, "CheckAuth", "????????????") },
            { id: "1", text: HtmlLang.Write(LangModule.FP, "ScanAuth", "????????????") },
            { id: "0", text: HtmlLang.Write(LangModule.FP, "NotCertified", "?????????") }
        ];
        selector.combobox({
            data: data,
            valueField: "id",
            textField: "text",
            multiple: true
        });
    };
    /**
     *
     * @param input
     */
    FPReconcileHome.prototype.initCodingStatusCombo = function (input) {
        var rcodingStatus = [
            { id: "0", text: HtmlLang.Write(LangModule.Common, "Uncreated", "?????????") },
            { id: "1", text: HtmlLang.Write(LangModule.Common, "Created", "?????????") },
            { id: "2", text: HtmlLang.Write(LangModule.Common, "UnnecessaryCreateVoucher", "????????????") }
        ];
        input.combobox({
            data: rcodingStatus,
            valueField: "id",
            textField: "text"
        });
    };
    /**
     * ??????????????????????????????
     * @param status
     */
    FPReconcileHome.prototype.getReconcileStatusName = function (status, withClass) {
        var text = "";
        var cls = "";
        switch (status) {
            case ReconcileStatusEnum.None:
                text = HtmlLang.Write(LangModule.FP, "NotReconciled", "?????????");
                cls = "fp-rc-noreconciled";
                break;
            case ReconcileStatusEnum.Reconciled:
                text = HtmlLang.Write(LangModule.FP, "Reconciled", "?????????");
                cls = "fp-rc-reconciled";
                break;
            case ReconcileStatusEnum.NoReconcile:
                text = HtmlLang.Write(LangModule.FP, "NoReconcile", "????????????");
                cls = "fp-rc-noreconcile";
                break;
            default:
                text = "";
        }
        if (withClass) {
            text = "<div class='" + cls + "' >" + text + "</div>";
        }
        return text;
    };
    /**
     * ??????????????????????????????
     * @param status
     */
    FPReconcileHome.prototype.getCodingStatusName = function (status, withClass) {
        var text = "";
        var cls = "";
        switch (status) {
            case ReconcileStatusEnum.NoReconcile:
                text = HtmlLang.Write(LangModule.FP, "UnnecessaryCreateVoucher", "????????????");
                cls = "fp-rc-noreconcile";
                break;
            default:
                text = "";
        }
        if (withClass) {
            text = "<div class='" + cls + "' >" + text + "</div>";
        }
        return text;
    };
    /**
     * ??????????????????????????????
     * @param status
     */
    FPReconcileHome.prototype.getVerifyTypeName = function (type, withClass) {
        var text = "";
        var cls = "";
        switch (type) {
            case VerifyTypeEnum.NoVerify:
                text = HtmlLang.Write(LangModule.FP, "NotCertified", "?????????");
                cls = "fp-rc-noverify";
                break;
            case VerifyTypeEnum.ScanVerify:
                text = HtmlLang.Write(LangModule.FP, "ScanAuth", "????????????");
                cls = "fp-rc-scanverify";
                break;
            case VerifyTypeEnum.CheckVerify:
                text = HtmlLang.Write(LangModule.FP, "CheckAuth", "????????????");
                cls = "fp-rc-checkedverify";
                break;
            case VerifyTypeEnum.NoNeedVerify:
                text = HtmlLang.Write(LangModule.FP, "NoNeedVerify", "????????????");
                cls = "fp-rc-noneedverify";
                break;
            default:
                text = "";
        }
        if (withClass) {
            text = "<div class='" + cls + "' >" + text + "</div>";
        }
        return text;
    };
    /**
     * ????????????
     * @param fapiaoID
     * @param callback
     */
    FPReconcileHome.prototype.viewFapiao = function (fapiaoID, callback) {
        new FPHome().viewFapiao(fapiaoID, function () {
            $.isFunction(callback) && callback();
        });
    };
    /**
     * ??????????????????????????????
     * @param status
     */
    FPReconcileHome.prototype.getFapiaoStatusName = function (status, withClass) {
        var text = "";
        var cls = "";
        switch (status) {
            case FPFapiaoStatusEnum.Obsolete:
                text = HtmlLang.Write(LangModule.FP, "Obsolete", "??????");
                cls = "fp-status-obsolete";
                break;
            case FPFapiaoStatusEnum.Normal:
                text = HtmlLang.Write(LangModule.FP, "Normal", "??????");
                cls = "fp-status-normal";
                break;
            case FPFapiaoStatusEnum.OutOfControl:
                text = HtmlLang.Write(LangModule.FP, "OutOfControl", "??????");
                cls = "fp-status-outofcontrol";
                break;
            case FPFapiaoStatusEnum.Unnormal:
                text = HtmlLang.Write(LangModule.FP, "Abnormal", "??????");
                cls = "fp-status-unnormal";
                break;
            case FPFapiaoStatusEnum.Credit:
                text = HtmlLang.Write(LangModule.FP, "RedFlush", "??????");
                cls = "fp-status-credit";
                break;
            default:
                text = "";
        }
        if (withClass) {
            text = "<div class='" + cls + "' >" + text + "</div>";
        }
        return text;
    };
    //??????????????????????????? ???????????????
    FPReconcileHome.prototype.GetFullTableNumber = function (number, type) {
        if (!number) {
            return "";
        }
        var prefix = (+type) == 0 ? "SFP" : "PFP";
        if (number instanceof Array) {
            var result = "";
            for (var i = 0; i < number.length; i++) {
                result += prefix + ("0000".substring(0, 4 - number[i].toString().length) + number[i].toString()) + ",";
            }
            return result.trimEnd(",");
        }
        return prefix + ("0000".substring(0, 4 - number.toString().length) + number.toString());
    };
    /**
     * ???????????????????????????
     * @param source
     */
    FPReconcileHome.prototype.getFapiaoSourceName = function (source) {
        switch (source) {
            case FapiaoSourceEnum.Input:
                return HtmlLang.Write(LangModule.FP, "ManualInput", "????????????");
            case FapiaoSourceEnum.Import:
                return HtmlLang.Write(LangModule.FP, "Import", "????????????");
            case FapiaoSourceEnum.Excel:
                return HtmlLang.Write(LangModule.FP, "ExcelInput", "Excel??????");
            default:
                return "";
        }
    };
    /**
    * ????????????coding??????
    */
    FPReconcileHome.prototype.getCodingPageList = function (filter, callback) {
        mAjax.post(this.getCodingPageListUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        }, null, true);
    };
    /**
    * ????????????coding??????
    */
    FPReconcileHome.prototype.saveCodingStatus = function (filter, callback) {
        mAjax.submit(this.saveCodingStatusUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ????????????coding??????
    */
    FPReconcileHome.prototype.saveCoding = function (data, callback) {
        mAjax.submit(this.saveCodingUrl, { filter: data }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ????????????coding??????
    */
    FPReconcileHome.prototype.saveCodingRow = function (data, callback) {
        (data.length >= 50 ? mAjax.submit : mAjax.post)(this.saveCodingRowUrl, { data: data }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ????????????coding??????
    */
    FPReconcileHome.prototype.deleteCodingRow = function (data, callback) {
        mAjax.post(this.deleteCodingRowUrl, { row: data }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ????????????coding??????
    */
    FPReconcileHome.prototype.getCodingSetting = function (callback) {
        mAjax.post(this.getCodingSettingUrl, {}, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ????????????coding??????
    */
    FPReconcileHome.prototype.saveCodingSetting = function (setting, callback) {
        mAjax.submit(this.saveCodingSettingUrl, { model: setting }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
     *
     * @param filter
     * @param callback
     */
    FPReconcileHome.prototype.resetCodingData = function (filter, callback) {
        mAjax.submit(this.resetCodingDataUrl, { filter: filter }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
    * ???????????????????????????
    */
    FPReconcileHome.prototype.saveFastCode = function (model, callback) {
        mAjax.submit(this.saveFastCodeUrl, { model: model }, function (data) {
            $.isFunction(callback) && callback(data);
        });
    };
    /**
     * ???????????????????????????
     */
    FPReconcileHome.prototype.getBaseData = function () {
        var result = null;
        mAjax.post(this.getBaseDataUrl, {}, function (data) {
            result = data;
        }, null, true, false);
        //?????????????????????????????????
        if (!!result.MContact && result.MContact.length > 0 && !!result.MTrackLink && result.MTrackLink.length > 0) {
            for (var i = 0; i < result.MContact.length; i++) {
                result.MContact[i].MTrackItem1 = !result.MTrackItem1 ? null : this.getTrackItem(result.MContact[i].MItemID, result.MTrackItem1.MCheckTypeGroupID, result.MTrackLink);
                result.MContact[i].MTrackItem2 = !result.MTrackItem2 ? null : this.getTrackItem(result.MContact[i].MItemID, result.MTrackItem2.MCheckTypeGroupID, result.MTrackLink);
                result.MContact[i].MTrackItem3 = !result.MTrackItem3 ? null : this.getTrackItem(result.MContact[i].MItemID, result.MTrackItem3.MCheckTypeGroupID, result.MTrackLink);
                result.MContact[i].MTrackItem4 = !result.MTrackItem4 ? null : this.getTrackItem(result.MContact[i].MItemID, result.MTrackItem4.MCheckTypeGroupID, result.MTrackLink);
                result.MContact[i].MTrackItem5 = !result.MTrackItem5 ? null : this.getTrackItem(result.MContact[i].MItemID, result.MTrackItem5.MCheckTypeGroupID, result.MTrackLink);
            }
        }
        return result;
    };
    /**
     * ??????????????????????????????????????????
     * @param contactId
     * @param trackId
     */
    FPReconcileHome.prototype.getTrackItem = function (contactId, trackId, trackLinks) {
        for (var i = 0; i < trackLinks.length; i++) {
            if (trackLinks[i].MContactID == contactId && trackLinks[i].MTrackID == trackId)
                return this.getType() == FPEnum.Sales ? trackLinks[i].MSalTrackId : trackLinks[i].MPurTrackId;
        }
        return null;
    };
    /**
     * ??????????????????
     * @param evt
     */
    FPReconcileHome.prototype.stopPropagation = function (evt) {
        if (evt.stopPropagation)
            evt.stopPropagation();
        else
            evt.cancelBubble = true;
        if (evt.preventDefault)
            evt.preventDefault();
        else
            evt.returnValue = false;
    };
    /**
     * ?????????
     * @param $grid
     * @param rowFildName
     */
    FPReconcileHome.prototype.mergeGridColCells = function ($grid, rowFildName) {
        var rows = $grid.datagrid('getRows');
        var startIndex = 0;
        var endIndex = 0;
        if (rows.length < 1) {
            return;
        }
        $.each(rows, function (i, row) {
            if (row[rowFildName] == rows[startIndex][rowFildName]) {
                endIndex = i;
            }
            else {
                $grid.datagrid('mergeCells', {
                    index: startIndex,
                    field: rowFildName,
                    rowspan: endIndex - startIndex + 1
                });
                startIndex = i;
                endIndex = i;
            }
        });
        $grid.datagrid('mergeCells', {
            index: startIndex,
            field: rowFildName,
            rowspan: endIndex - startIndex + 1
        });
    };
    return FPReconcileHome;
}());
//# sourceMappingURL=FPReconcileHome.js.map