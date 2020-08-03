var FPEditVerifyStatus = /** @class */ (function () {
    function FPEditVerifyStatus() {
        this.selectIds = $("#hidIds").val();
        this.langId = $("#langId").val();
        this.verifyStatusTable = "#batchGrid";
        this.getFaPiaoVerifyListUrl = "/FP/FPHome/GetFaPiaoVerifyList";
        this.saveInRealTime = true;
        this.verifyTypeSelector = "tr[datagrid-row-index='0'] .combo-text";
        this.verifyDateSelector = "tr[datagrid-row-index='0'] .Wdate";
        this.btnSave = "#aSave";
        this.hint = HtmlLang.Write(LangModule.FP, "ButtonsBatchSetting", "批量设置");
        this.editIndex = null;
        this.viewVoucherUrl = "/GL/GLVoucher/GLVoucherEdit";
    }
    FPEditVerifyStatus.prototype.init = function () {
        this.home = new FPReconcileHome();
        this.loadData();
    };
    /**
    * 展示数据到面板
    */
    FPEditVerifyStatus.prototype.showData = function (data) {
    };
    ;
    /**
     * 获取数据
     */
    FPEditVerifyStatus.prototype.loadData = function () {
        var _this = this;
        //获取过滤
        var filter = {
            MFapiaoIDs: this.selectIds.split(',')
        };
        $(this.verifyStatusTable).datagrid({
            url: this.getFaPiaoVerifyListUrl,
            queryParams: filter,
            pagination: false,
            resizable: true,
            auto: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $(this.verifyStatusTable).width() - 20,
            height: $("body").height() - 40,
            columns: [[
                    {
                        field: 'MNumber', title: HtmlLang.Write(LangModule.FP, "FapiaoNumber", "发票号"), width: 100, align: 'left', formatter: function (value, record) {
                            return record.MID === "" ? "" : value;
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
                        field: 'MTotalAmount', title: HtmlLang.Write(LangModule.FP, "TotalAmount", "总额"), width: 100, align: 'right', formatter: function (value, record) {
                            return record.MID === "" ? "" : mMath.toMoneyFormat(value);
                        }
                    },
                    {
                        field: 'MVerifyType', hidden: this.home.getType() == FPEnum.Sales, title: HtmlLang.Write(LangModule.FP, "VerifyType", "认证状态"), width: 100, align: 'left',
                        editor: this.getVerifyStatusEditor('MVerifyType'),
                        formatter: function (value, record) {
                            return record.MID === "" ? "" : _this.home.getVerifyTypeName(value);
                        }
                    },
                    {
                        field: 'MVerifyDate', hidden: this.home.getType() == FPEnum.Sales, title: HtmlLang.Write(LangModule.FP, "VerifyDate", "认证月份"), width: 100, align: 'left',
                        editor: this.getVerifyDateEditor('MVerifyDate'),
                        formatter: function (value, record) {
                            var date = mDate.parse(value);
                            if (record.MVerifyType == 1 || record.MVerifyType == 2) {
                                return date.getFullYear() <= 1901 ? "" : record.MID === "" ? "" : mDate.parse(value).format("yyyy-MM");
                            }
                            else {
                                return "";
                            }
                        }
                    },
                    {
                        field: 'MStatus', title: HtmlLang.Write(LangModule.FP, "FapiaoStatus", "状态"), width: 100, align: 'center', formatter: function (value, record) {
                            return record.MID === "" ? "" : _this.home.getFapiaoStatusName(value, true);
                        }
                    },
                    //标准版显示勾兑状态
                    {
                        field: 'MReconcileStatus', title: HtmlLang.Write(LangModule.FP, "ReconcileStatus", "勾兑状态"), hidden: this.home.isSmartVersion(), width: 100, align: 'center', formatter: function (value, record) {
                            return record.MID === "" ? "" : _this.home.getReconcileStatusName(value, true);
                        }
                    },
                    //记账版显示凭证状态
                    {
                        field: 'MCodingStatus', title: HtmlLang.Write(LangModule.FP, "FPVoucherStatus", "凭证状态"), hidden: !this.home.isSmartVersion(), width: 100, align: 'center', formatter: function (value, record) {
                            if (record.MVoucherID) {
                                var voucherNumber = "GL-" + record.MVoucherNumber;
                                return "<a href='###' onclick='mTab.addOrUpdate(\"" + voucherNumber + "\",\"" + _this.viewVoucherUrl + "?MItemID=" + record.MVoucherID + "\", false, true, true, true)'>" + voucherNumber + "</a>";
                            }
                            return record.MStatus == FPFapiaoStatusEnum.Obsolete ? "" : _this.home.getCodingStatusName(value, true);
                        }
                    }
                ]],
            onClickCell: function (rowIndex, field, value, evt) {
                _this.beginEdit(rowIndex);
                var editor = $(_this.verifyStatusTable).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    editor.target.focus();
                }
            },
            onLoadSuccess: function () {
                _this.initEditor();
                _this.initDateCss();
                $(_this.btnSave).off("click").on("click", function (evt) {
                    var data = _this.getDataSourceEntry();
                    _this.changeFaPiaoVerifyStatus(data);
                });
                var editor = $(_this.verifyStatusTable).datagrid('getEditor', { index: 0, field: "MVerifyType" });
                $(editor.target).combobox("setValue", "");
                var dateeditor = $(_this.verifyStatusTable).datagrid('getEditor', { index: 0, field: "MVerifyDate" });
                $(dateeditor.target).val("");
                $(_this.verifyTypeSelector).attr("hint", _this.hint).initHint();
                $(_this.verifyDateSelector).attr("hint", _this.hint).initHint();
                //对第一行认证状态绑定onselect事件
                var self = _this;
                $(editor.target).combobox('options').onSelect = function (record) {
                    if (record != undefined) {
                        self.bindGridComboboxEditorEvent("MVerifyType", record.id);
                    }
                    ;
                };
                //对第一行认证期间绑定onpicked事件bindGridEditorEvent
                $(_this.verifyDateSelector).attr("onfocus", "WdatePicker({ dateFmt: 'yyyy-MM',readOnly:true, lang: '" + _this.langId + "', skin:'" + _this.langId + "',onpicked:function(){new FPEditVerifyStatus().bindGridEditorEvent($(this).val())}})");
            }
        });
    };
    //结束编辑列表
    FPEditVerifyStatus.prototype.endEditGrid = function () {
        var rows = $(this.verifyStatusTable).datagrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            $(this.verifyStatusTable).datagrid('endEdit', i);
        }
    };
    /**
     * 初始化日期控件样式及值
     */
    FPEditVerifyStatus.prototype.initDateCss = function () {
        var rows = $(this.verifyStatusTable).datagrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            $("tr[datagrid-row-index='" + i + "'] .Wdate").css("width", "173px");
            var editor = $(this.verifyStatusTable).datagrid('getEditor', { index: i, field: "MVerifyType" });
            var verifyType = $(editor.target).combobox("getValue");
            if (verifyType === VerifyTypeEnum.NoVerify.toString() && i > 0) {
                $("tr[datagrid-row-index='" + i + "'] .Wdate").val("");
                $("tr[datagrid-row-index='" + i + "'] .Wdate").attr("disabled", "disabled");
            }
        }
    };
    /**
    * 开始bianji
    * @param index
    */
    FPEditVerifyStatus.prototype.beginEdit = function (rowIndex) {
        $(this.verifyStatusTable).datagrid("beginEdit", rowIndex);
        this.editIndex = rowIndex;
    };
    /**
     * 获取认证状态编辑框
     */
    FPEditVerifyStatus.prototype.getVerifyStatusEditor = function (name) {
        var data = [
            { id: "2", text: HtmlLang.Write(LangModule.FP, "CheckAuth", "勾选认证") },
            { id: "1", text: HtmlLang.Write(LangModule.FP, "ScanAuth", "扫描认证") },
            { id: "0", text: HtmlLang.Write(LangModule.FP, "NotCertified", "未认证") }
        ];
        var editor = {
            type: 'combobox',
            options: {
                height: 32,
                valueField: "id",
                textField: "text",
                autoSizePanel: false,
                data: data,
                onSelect: function (record, data) {
                    if (record.id == 0) {
                        $($(data).parents("td")[1]).next("td").find(".Wdate").val("");
                        $($(data).parents("td")[1]).next("td").find(".Wdate").attr("disabled", "disabled");
                    }
                    else {
                        $($(data).parents("td")[1]).next("td").find(".Wdate").removeAttr("disabled");
                    }
                }
            }
        };
        return editor;
    };
    /**
     * 绑定combobox编辑事件
     * @param index
     * @param field
     * @param value
     */
    FPEditVerifyStatus.prototype.bindGridComboboxEditorEvent = function (field, value) {
        var rows = $(this.verifyStatusTable).datagrid('getRows');
        for (var rowIndex = 1; rowIndex < rows.length; rowIndex++) {
            var editor = $(this.verifyStatusTable).datagrid('getEditor', { index: rowIndex, field: "MVerifyType" });
            $(editor.target).combobox("setValue", value);
            if (value === VerifyTypeEnum.NoVerify.toString()) {
                $("tr[datagrid-row-index='" + rowIndex + "'] .Wdate").val("");
                $("tr[datagrid-row-index='" + rowIndex + "'] .Wdate").attr("disabled", "disabled");
            }
            else {
                $("tr[datagrid-row-index='" + rowIndex + "'] .Wdate").removeAttr("disabled");
            }
        }
    };
    /**
     * 绑定时间编辑事件
     * @param rowIndex
     */
    FPEditVerifyStatus.prototype.bindGridEditorEvent = function (value) {
        var rows = $(this.verifyStatusTable).datagrid('getRows');
        for (var rowIndex = 1; rowIndex < rows.length; rowIndex++) {
            var editor = $(this.verifyStatusTable).datagrid('getEditor', { index: rowIndex, field: "MVerifyDate" });
            var verifyTypeEditor = $(this.verifyStatusTable).datagrid('getEditor', { index: rowIndex, field: "MVerifyType" });
            if ($(verifyTypeEditor.target).combobox("getValue") === VerifyTypeEnum.NoVerify.toString())
                continue;
            $(editor.target).val(value);
        }
    };
    /**
     * 初始化编辑状态
     */
    FPEditVerifyStatus.prototype.initEditor = function () {
        var rows = $(this.verifyStatusTable).datagrid('getRows');
        for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
            $(this.verifyStatusTable).datagrid("beginEdit", rowIndex);
        }
    };
    /**
     * 获取认证期间编辑框
     */
    FPEditVerifyStatus.prototype.getVerifyDateEditor = function (name) {
        var editor = {
            type: 'monthpicker',
            options: {
                dateFmt: 'yyyy-MM',
                readOnly: true,
                lang: this.langId,
                skin: this.langId,
                height: 28
            }
        };
        return editor;
    };
    //重新设置数据源
    FPEditVerifyStatus.prototype.getDataSourceEntry = function () {
        var entryDataSource = new Array();
        var rows = $(this.verifyStatusTable).datagrid('getRows');
        for (var i = 1; i < rows.length; i++) {
            var row = rows[i];
            var editor = $(this.verifyStatusTable).datagrid('getEditor', { index: i, field: "MVerifyType" });
            row.MVerifyType = $(editor.target).combobox("getValue");
            var dateEditor = $(this.verifyStatusTable).datagrid('getEditor', { index: i, field: "MVerifyDate" });
            row.MVerifyDate = $(dateEditor.target).val();
            entryDataSource.push(row);
        }
        return entryDataSource;
    };
    ;
    /**
     * 初始化面板事件
     */
    FPEditVerifyStatus.prototype.initEvent = function () {
    };
    /**
    * 初始化元素  高度 宽度 滚动等
    */
    FPEditVerifyStatus.prototype.initDom = function () {
    };
    /**
    * 初始化分页
    */
    FPEditVerifyStatus.prototype.initPage = function (total) {
    };
    /**
     * 修改发票认证状态
     * @param filter
     */
    FPEditVerifyStatus.prototype.changeFaPiaoVerifyStatus = function (data) {
        var _this = this;
        mDialog.confirm(HtmlLang.Write(LangModule.FP, "ModifyFBRZStatusTitle", "对认证状态的修改，系统不会做任何总账的处理，如有需要，请手动进行操作。"), function () {
            _this.home.setFaPiaoVerifyStatus(data, function (data) {
                if (data.Success) {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "OperationSuccessfully", "操作成功!"));
                    window.parent.$(".mCloseBox").trigger("click");
                }
                else {
                    mDialog.message(data.Message);
                    return;
                }
            });
        });
    };
    return FPEditVerifyStatus;
}());
//# sourceMappingURL=FPEditVerifyStatus.js.map