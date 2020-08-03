/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
/// <reference path="ImportBase.js" />
var ImportOptions = {
    excelRowCount: 0,
    excelDataList: null,
    previewTop: 0,
    headerColumnIdx: 1,
    dataColumnIdx: 2,
    solution: {},
    config: {},
    configColumn: null,
    mapping: {},
    excelHeaderMapping: {},
    excelSelectData: [],
    arrExcelHeader: null,
    requiredFields: [],
    rightFields: [],
    centerFields: [],
    type: Megi.getUrlParam("type"),
    isHangTian: $("#isHangTian").val(),
    multiSelectFields: [["MContact"], [], [], [], []],//Voucher: 1, Sale: 2, Purchase: 3, Payment: 4, Receive: 5
    init: function () {
        ImportOptions.setTitle();
        $.mDialog.max();
        ImportOptions.adjustPreviewWidth();
        ImportOptions.loadExcelData();
        ImportOptions.initSolution();
        ImportOptions.bindAction();
        ImportOptions.loadData();
    },
    setTitle: function () {
        var title = '';
        switch (parseInt(ImportOptions.type)) {
            case ImportTypeExt.Voucher:
                title = HtmlLang.Write(LangModule.GL, "ImportVoucher", "Import Voucher");
                break;
            case ImportTypeExt.Sale:
                title = HtmlLang.Write(LangModule.IV, "ImportInvoice", "Import Invoice");
                break;
            case ImportTypeExt.Purchase:
                title = HtmlLang.Write(LangModule.IV, "ImportBill", "Import Bill");
                break;
            case ImportTypeExt.Payment:
                title = HtmlLang.Write(LangModule.Bank, "ImportSpendMoney", "Import Spend Money");
                break;
            case ImportTypeExt.Receive:
                title = HtmlLang.Write(LangModule.Bank, "ImportReceiveMoney", "Import Receive Money");
                break;
            case ImportTypeExt.InFaPiao:
            case ImportTypeExt.OutFaPiao:
                title = HtmlLang.Write(LangModule.FP, "MatchFaPiao", "匹配发票项");
                break;
        }
        if (title) {
            $("div.boxTitle>h3", parent.document).text(title);
        }
    },
    loadExcelData: function () {
        ImportOptions.arrExcelHeader = $("#hidExcelHeader").val().split(',');
        var excelData = $("#hidExcelData").val();
        if (excelData != '') {
            ImportOptions.excelDataList = eval('(' + excelData.replace(/&apos;/g, '\'') + ')');
            ImportOptions.excelRowCount = ImportOptions.excelDataList.length;
        }
    },
    initSolution: function () {
        ImportOptions.solution = eval('(' + $("#hidJsonSolution")[0].value + ')');
        ImportOptions.config = ImportOptions.solution.MConfig;

        $.each(ImportOptions.config, function (i, item) {
            if (item.MIsRequired) {
                ImportOptions.requiredFields.push(item.MConfigID);
            }
            ImportOptions.mapping[item.MConfigID] = item.MColumnName;

            switch (item.MDataType) {
                case "datetime":
                    ImportOptions.centerFields.push(item.MConfigID);
                    break;
                case "decimal":
                    ImportOptions.rightFields.push(item.MConfigID);
                    break;
            }
        });
    },
    loadData: function () {
        ImportOptions.previewTop = parseInt($("#hidPreviewTop").val());
        ImportOptions.previewExcelSelect(ImportOptions.headerRowStart(), ImportOptions.headerColumnIdx);
        ImportOptions.initExcelSelect();
        if (ImportOptions.isHangTian) {
            ImportOptions.previewMappingData(1);
            ImportOptions.previewMappingData(2);
            $("#txtDataRowStart").numberspinner('disable');
            $("#txtHeaderRowStart").numberspinner('disable');
        } else {
            ImportOptions.previewMappingData();
        }

    },
    adjustPreviewWidth: function () {
        $('.mg-statement-import .preview').css('width', '100%').css('width', '-=450px');
    },
    isMultiSelField: function (configName) {
        return $.inArray(configName, ImportOptions.multiSelectFields[parseInt(ImportOptions.type) - 1]) != -1;
    },
    initExcelSelect: function () {
        if (ImportOptions.excelDataList != null) {
            $.each(ImportOptions.config, function (i, config) {
                var sel = $(".excel-select[id=" + config.MConfigID + "]");
                //设置多选
                if (ImportOptions.isMultiSelField(config.MConfigStandardName)) {
                    sel.combobox({ multiple: true });
                    sel.combobox("setValue", "");
                }
                if (config.MColumnName == null || config.MColumnName == "") {
                    sel.combobox("setValue", "");
                }
                else {
                    var arrColumnName = config.MColumnName.split(',');
                    //特殊字符解码
                    //$.each(tmpArrColumnName, function (j, item) {
                    //    arrColumnName.push(_.htmlDecode(item));
                    //});
                    var inited = 0;
                    $.each(ImportOptions.arrExcelHeader, function (k, excelHeader) {
                        if (ImportOptions.excelHeaderMapping[excelHeader] == config.MColumnName
                            || $.inArray(ImportOptions.excelHeaderMapping[excelHeader], arrColumnName) != -1) {
                            var selectedValue = sel.combobox("getValues");
                            if (selectedValue == '') {
                                sel.combobox("setValue", excelHeader);
                            } else {
                                sel.combobox("setValues", [selectedValue, excelHeader]);
                            }
                            inited++;
                            //不是多选字段，或者是多选字段并且找到所有字段，则退出循环
                            if (!ImportOptions.isMultiSelField(config.MConfigStandardName) || arrColumnName.length == inited) {
                                return false;
                            }
                        }
                    });
                }
                if (ImportOptions.isHangTian) {
                    sel.combobox('disable');
                }
            });
        }
    },
    headerRowStart: function () {
        var start = $("#txtHeaderRowStart").val();
        if (!start) {
            start = 1;
        }
        return parseInt(start) - 1;
    },
    dataRowStart: function () {
        var start = $("#txtDataRowStart").val();
        if (!start) {
            start = 1;
        }
        return parseInt(start) - 1;
    },
    bindAction: function () {
        $('#txtHeaderRowStart,#txtDataRowStart').numberspinner({
            min: 1,
            max: ImportOptions.excelRowCount,
            editable: true,
            width: 43
        });
        $("span.spinner-arrow").click(function () {
            var txtRowStart = $(this).closest("span.spinner").find("input:text");
            var rowStart = parseInt(txtRowStart.val()) - 1;
            var isHeader = txtRowStart.attr("id") == "txtHeaderRowStart";
            if (isHeader) {
                ImportOptions.previewExcelSelect(rowStart, ImportOptions.headerColumnIdx);
                ImportOptions.onHeaderRowStartChanged();
            }
            else {
                ImportOptions.previewMappingData();
            }
        });
        Megi.attachPropChangeEvent(document.getElementById("txtHeaderRowStart"), function () {
            ImportOptions.previewExcelSelect(ImportOptions.headerRowStart(), ImportOptions.headerColumnIdx);
            ImportOptions.onHeaderRowStartChanged();
        });
        Megi.attachPropChangeEvent(document.getElementById("txtDataRowStart"), function () {
            ImportOptions.previewMappingData();
        });
        $.each($(".excel-column2 input:text"), function (i, selTxt) {
            Megi.attachPropChangeEvent(selTxt, function () {
                if ($.trim(selTxt.value) == '') {
                    var sel = $(selTxt).parent().prev();
                    ImportOptions.clearSelect(sel.get(0));
                    ImportOptions.previewMappingData();
                    sel.combo("hidePanel");
                }
            });
        });
        $("#aSave").click(function () {
            ImportOptions.save();
        });
        $("#aBack").click(function () {
            var id = location.href.substring(location.href.lastIndexOf('/') + 1).split('?')[0];
            if (parseInt(ImportOptions.type) == ImportTypeExt.InFaPiao || parseInt(ImportOptions.type) == ImportTypeExt.OutFaPiao) {
                var inOrOuttype = $("#inOrOuttype").val();
                var fpType = $("#fpType").val();
                mWindow.reload("/IO/ImportBySolution/ImportFaPiao?inOrOuttype=" + inOrOuttype + "&fpType=" + fpType);
            } else {
                mWindow.reload("/IO/ImportBySolution/Import?id=" + id + "&type=" + ImportOptions.type);
            }
        });
        $("#aPreview").click(function () {
            if (!ImportOptions.validateMapping()) {
                return;
            }
            //设置页面隐藏
            $(".mg-statement-import").hide();
            //导入库数据预览
            $("#divDBDataPreview").show();

            $("#btnSave").show();
            $("#btnPreview").hide();


            $(".main-header").length && $(".main-header").tabs({
                selected: 0,
                onSelect: function (title, index) {
                    if (index == 0) {
                        $("#specialGrid").show();
                        $("#ptGrid").hide();
                    } else {
                        $("#specialGrid").hide();
                        $("#ptGrid").show();
                    }
                }
            });


        });

        $("#aBackAgain").click(function () {
            //设置页面隐藏
            $(".mg-statement-import").show();
            //导入库数据预览
            $("#divDBDataPreview").hide();

            $("#btnSave").hide();
            $("#btnPreview").show();
            ImportOptions.adjustPreviewWidth();
        });
    },
    validateSelect: function (sel) {
        var selVal = $(sel).combobox("getValue");
        var combo = $(sel).next();
        if ($.inArray(sel.id, ImportOptions.requiredFields) != -1) {
            if (selVal) {
                combo.css({ "border": "" });
            }
            else {
                combo.css({ "border": "1px solid red" });
            }
        }
        else {
            if (selVal && combo.css('border') != '') {
                combo.css({ "border": "" });
            }
        }
    },
    clearSelect: function (sender) {
        $(sender).combobox("setValue", "");
        ImportOptions.validateSelect(sender);
        ImportOptions.mapping[sender.id] = '';
    },
    getSelectedText: function (excelHeaders) {
        var selectedTexts = [];
        $.each(excelHeaders, function (index, excelHeader) {
            selectedTexts.push(ImportOptions.excelHeaderMapping[excelHeader]);
        });
        return selectedTexts;
    },
    bindExcelSelect: function () {
        var prevValue = [];
        var prevText = [];
        var configId;
        var configName;
        $(".excel-select").combobox({
            data: ImportOptions.excelSelectData,
            valueField: 'Key',
            textField: 'Value',
            panelHeight: '140',
            //multiple:true,//不允许多对一
            onShowPanel: function () {
                var tdConfigColumn = $(this).closest("tr").find(".config-column");
                configId = tdConfigColumn.find("input[name='hidConfigID']").val();
                configName = tdConfigColumn.find("input[name='hidConfigStandardName']").val();
                prevValue = $(this).combobox("getValues");
                prevText = ImportOptions.getSelectedText(prevValue);
            },
            onSelect: function (item) {
                if (item.Key != "") {
                    if (ImportOptions.isMultiSelField(configName)) {
                        if (prevValue.length == 0 || prevValue.length == 1 && prevValue[0] == "") {
                            $(this).combobox("setValue", item.Key);
                        }
                        ImportOptions.validateSelect(this);
                        ImportOptions.mapping[configId] = $(this).combobox("getText");
                    }
                    else {
                        ImportOptions.validateSelect(this);
                        ImportOptions.mapping[configId] = item.Value;
                    }
                }
                else {//分配了一个或多个字段->unassign
                    ImportOptions.clearSelect(this);
                }
                ImportOptions.previewMappingData();
                $(this).combo("hidePanel");
            },
            onUnselect: function (item) {
                if (ImportOptions.isMultiSelField(configName)) {
                    var newKey = prevValue.filter(function (i) {
                        return i != item.Key;
                    });
                    var newText = prevText.filter(function (i) {
                        return i != item.Value;
                    });
                    $(this).combobox("setValue", newKey);
                    ImportOptions.validateSelect(this);
                    ImportOptions.mapping[configId] = newText;
                }
                else {
                    //分配了一个字段，反选变成unassign
                    $(this).combobox("setValue", "");
                    ImportOptions.validateSelect(this);
                    ImportOptions.mapping[configId] = '';
                }
                ImportOptions.previewMappingData();
                $(this).combo("hidePanel");
            }
        });
    },
    onHeaderRowStartChanged: function () {
        var headerStart = ImportOptions.headerRowStart();
        var dataStart = ImportOptions.dataRowStart();
        //自动调整数据开始行
        if (headerStart >= dataStart) {
            var newDataStart = headerStart + 2;
            $('#txtDataRowStart').numberspinner('setValue', newDataStart);
            ImportOptions.previewExcelRowData(newDataStart, ImportOptions.dataColumnIdx);
            ImportOptions.previewMappingData();
        }
    },
    validateMapping: function () {
        if (ImportOptions.config.length == 0) {
            return;
        }
       
        if ($("#txtDataRowStart").val() === "" || $("#txtHeaderRowStart").val() === "") {
            return;
        }
        //IE9,10环境下mFormValidate此方法被调用导致页面数据丢失,先改为上面方式处理 by hxf
        //if (!$(".setting").mFormValidate()) {
        //    return;
        //}

        if (ImportOptions.dataRowStart() <= ImportOptions.headerRowStart()) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "DataStartInvolid", "Valid data starting row must be greater than the start row of the header!"));
            return false;
        }

        var unAssignRequiredField = [];
        var sameColumnField = [];
        var unassignCurrency = false;
        var currencyConfigId;
        $.each(ImportOptions.config, function (i, config) {
            var selVal = $("#" + config.MConfigID).combobox("getValue");
            if ($.inArray(config.MConfigID, ImportOptions.requiredFields) != -1 && selVal == '') {
                //航天方案,发票类型不作校验
                if (!(ImportOptions.isHangTian && config.MConfigStandardName === "MType")) {
                    unAssignRequiredField.push(config.MConfigName);
                    $("#" + config.MConfigID).next().css({ "border": "1px solid red" });
                }
            }
            //检查发票是否重复匹配了列
            if (parseInt(ImportOptions.type) == ImportTypeExt.OutFaPiao || parseInt(ImportOptions.type) == ImportTypeExt.InFaPiao) {
                $.each(ImportOptions.config, function (j, config2) {
                    var selJVal = $("#" + config2.MConfigID).combobox("getValue");
                    if (selVal == selJVal && i != j && selVal != '') {
                        sameColumnField.push(config.MConfigName);
                        $("#" + config.MConfigID).next().css({ "border": "1px solid red" });
                    }
                });
            }
            if (config.MConfigStandardName === "MCurrencyID") {
                currencyConfigId = config.MConfigID;
            }
            if (config.MConfigStandardName === "MExchangeRate" && ImportOptions.mapping[config.MConfigID]) {
                $.each(ImportOptions.config, function (j, config2) {
                    if (config2.MConfigStandardName === "MCurrencyID" && !ImportOptions.mapping[config2.MConfigID]) {
                        unassignCurrency = true;
                        return false;
                    }
                });
            }
        });
        //汇率字段已分配，但币别未分配
        if (unassignCurrency) {
            if (currencyConfigId) {
                $("#" + currencyConfigId).next().css({ "border": "1px solid red" });
            }
            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "CurrencyUnassigned", "The Currency field is unassigned!（When exchange rate is not empty, the Currency field is required!）"));
            return false;
        }
        else {
            $("#" + currencyConfigId).next().css({ "border": "" });
        }
        if (sameColumnField.length > 0) {
            $.mAlert(HtmlLang.Write(LangModule.Docs, "ValidateSameColumn", "重复匹配了列！"));
            return false;
        }
        if (unAssignRequiredField.length > 0) {
            $.mAlert(HtmlLang.Write(LangModule.Docs, "ValidateSolutionRequired", "The required fields:{0} is unassigned.").replace('{0}', unAssignRequiredField.toString()));
            return false;
        }
        return true;
    },
    save: function () {
        var model = {};
        model.SolutionID = ImportOptions.solution.MItemID;
        model.TemplateType = Megi.getUrlParam("type");
        model.MSolutionName = $("#spSolution").text();
        model.MHeaderRowIndex = $("#txtHeaderRowStart").val();
        model.MDataRowIndex = $("#txtDataRowStart").val();
        model.FileName = $("#hidFileName").val();
        model.MConfig = [];
        model.FaPiaoType = $("#inOrOuttype").val();
        model.Type = $("#fpType").val();

        $.each(ImportOptions.mapping, function (id, name) {
            var obj = {};
            obj.MConfigID = id;
            obj.MColumnName = name;
            model.MConfig.push(obj);
        });
        mAjax.submit(
            "/IO/ImportBySolution/Save",
            { model: model },
            function (response) {
                if (response.Success) {
                    if (ImportOptions.type == ImportTypeExt.InFaPiao || ImportOptions.type == ImportTypeExt.OutFaPiao) {
                        if (response.MessageList && response.MessageList.length > 0) {
                            var retMsg = '<div style="text-align:left;margin-left:20px;">';

                            var title = HtmlLang.Write(LangModule.Bank, "FaPiaoErrorUnImport", "以下数据未导入:");
                            retMsg = retMsg + title + "</br>";
                            for (var i = 0; i < response.MessageList.length; i++) {
                                retMsg = retMsg + " - " + response.MessageList[i] + "</br>";
                            }
                            $.mDialog.alert(retMsg, function () {
                                $.mDialog.close(1, "");
                            });
                            $("#popup_message").css("max-width", "500px");
                            $("#popup_message").css("max-height", "500px");
                        } else {
                            var url = "/IO/ImportBySolution/ConfirmImport?type=" + ImportOptions.type + "&filename=" + encodeURIComponent(model.FileName);
                            url = url + "&normalFPCount=" + response.NormalFPCount + "&specialFPCount=" + response.SpecialFPCount;
                            url = url + "&hasglstartData=" + response.HasGLStartData + "&solutionID=" + model.SolutionID + "&solutionID=" + model.SolutionID;
                            mWindow.reload(url);
                        }
                    } else {
                        ImportBase.showSuccessMsg(ImportOptions.type, response.Tag);
                        $.mDialog.close();
                    }

                } else {
                    if (ImportOptions.type == ImportTypeExt.InFaPiao || ImportOptions.type == ImportTypeExt.OutFaPiao) {
                        if (response.MessageList && response.MessageList.length > 0) {
                            if (response.Tag && response.Tag == "1") {
                                $.mDialog.alert(response.Message, undefined, 0, true);
                            } else {
                                var retMsg = '<div style="text-align:left;margin-left:20px;">';
                                var title = HtmlLang.Write(LangModule.Bank, "FaPiaoErrorUnImport", "以下数据未导入:");
                                retMsg = retMsg + title + "</br>";
                                for (var i = 0; i < response.MessageList.length; i++) {
                                    retMsg = retMsg + " - " + response.MessageList[i] + "</br>";
                                }
                                $.mDialog.alert(retMsg, undefined, 0, true);
                           }
                        } else {
                            $.mDialog.alert(response.Message, undefined, 0, true);
                        }
                    } else {
                        $.mDialog.alert("<div>" + response.Message.replace(/\n|\r\n/g, "<br>") + "</div>", undefined, 0, true, true);
                    }

                }
            });
    },
    previewExcelSelect: function (rowIdx) {
        if (ImportOptions.excelDataList != null) {
            if (rowIdx >= 0 && rowIdx < ImportOptions.excelDataList.length) {
                var headerRow = ImportOptions.excelDataList[rowIdx];
                ImportOptions.excelSelectData = [];
                ImportOptions.excelSelectData.push({ Key: "", Value: HtmlLang.Write(LangModule.Bank, "Unassigned", "Unassigned") });
                $.each(headerRow, function (xlsHeader, data) {
                    if (data != null && data != '') {
                        ImportOptions.excelSelectData.push({ Key: xlsHeader, Value: data });
                        ImportOptions.excelHeaderMapping[xlsHeader] = data;
                    }
                });
            }
            ImportOptions.bindExcelSelect();
        }
    },
    getExcelHeader: function (columnName) {
        var result = [];
        var arrColumnName = columnName.toString().split(',');
        $.each(ImportOptions.excelHeaderMapping, function (xlsHeader, columnHeader) {
            if ($.inArray(columnHeader, arrColumnName) != -1) {
                result.push(xlsHeader);
                if (arrColumnName.length === 1) {
                    return false;
                }
            }
        });
        return result;
    },
    getConfigNameByIds: function (configIds) {
        var result = [];
        var arrIds = configIds.split(',');
        for (var i = 0; i < arrIds.length; i++) {
            result.push($("#spConfig" + arrIds[i]).text());
        }
        return result.toString();
    },
    previewMappingData: function (type) {
        if (ImportOptions.excelDataList != null) {
            var tbPreviewMapping = type == 1 ? $("#tbPreviewSpecialMapping") : type == 2 ? $("#tbPreviewptMapping") : $("#tbPreviewMapping");
            tbPreviewMapping.find("thead").text('');
            tbPreviewMapping.find("tbody").text('');
            var assignedExcelHeader = [];
            var moneyFieldIndex = [];
            var centerFieldIndex = [];
            var filterMapping = [];
            var dtHeader = '';

            $.each(ImportOptions.mapping, function (id, columName) {
                var hasDuplicate = false;
                //var configColumn = $("#spConfig" + id).text();
                $.each(filterMapping, function (idx, filter) {
                    if (filter.ColumnName == columName) {
                        filter.ConfigID += "," + id;
                        hasDuplicate = true;
                        return false;
                    }
                });
                if (!hasDuplicate && columName != '' && columName != null) {
                    filterMapping.push({ ColumnName: columName, ConfigID: id });
                }
            });

            $.each(filterMapping, function (i, filter) {
                if (filter.ColumnName) {
                    var xlsHeader = ImportOptions.getExcelHeader(filter.ColumnName);
                    var align = " style='text-align: center;'";
                    var configName = ImportOptions.getConfigNameByIds(filter.ConfigID);
                    dtHeader += "<td class='preview-mapping-header'" + align + ">" + configName + "<input type='hidden' value='" + filter.ConfigID + "'/></td>";
                    if ($.inArray(filter.ConfigID, ImportOptions.rightFields) != -1) {
                        moneyFieldIndex.push(xlsHeader);
                    }
                    else if ($.inArray(filter.ConfigID, ImportOptions.centerFields) != -1) {
                        centerFieldIndex.push(xlsHeader);
                    }
                    assignedExcelHeader.push(xlsHeader); 0
                }
            });
            tbPreviewMapping.find("thead").append("<tr>" + dtHeader + "</tr>");
            if (type == 1 || type == 2) {
                var dataJson = type == 1 ? $("#hidSpecivalFaPiao").val() : $("#hidGeneralFaPiao").val();
                var data = eval("(" + dataJson.replace(/&apos;/g, '\'') + ")");
                ImportOptions.pingPreviewTable(1, data, assignedExcelHeader, tbPreviewMapping, moneyFieldIndex, centerFieldIndex);
            } else {
                ImportOptions.pingPreviewTable(ImportOptions.dataRowStart(), ImportOptions.excelDataList, assignedExcelHeader, tbPreviewMapping, moneyFieldIndex, centerFieldIndex);
            }
        }
    },
    pingPreviewTable: function (rowStart, data, assignedExcelHeader, tbPreviewMapping, moneyFieldIndex, centerFieldIndex) {
        for (var rowIdx = rowStart ; rowIdx < data.length; rowIdx++) {
            var tdTmp = '';
            var dataRow = data[rowIdx];

            $.each(assignedExcelHeader, function (i, xlsHeader) {
                var align = '';
                if ($.inArray(xlsHeader, moneyFieldIndex) != -1) {
                    align = " style='text-align: right;'";
                } else if ($.inArray(xlsHeader, centerFieldIndex) != -1) {
                    align = " style='text-align: center;'";
                }
                var val = [];
                $.each(xlsHeader, function (i, header) {
                    val.push(dataRow[header] == "" ? "" : mText.encode(dataRow[header]));
                });
                val = val.filter(Boolean);
                tdTmp += "<td" + align + " nowrap>" + (val == null ? '' : val) + "</td>";
            });
            if (tdTmp != '') {
                var cssHide = rowIdx > (ImportOptions.previewTop - 1) ? " style='display:none;'" : " style='height:30px;'";
                tbPreviewMapping.find("tbody").append("<tr index='" + rowIdx + "'" + cssHide + ">" + tdTmp + "</tr>");
            }
        }
    }
}

$(document).ready(function () {
    $(window).resize(function () {
        ImportOptions.adjustPreviewWidth();
    });
    ImportOptions.init();
});