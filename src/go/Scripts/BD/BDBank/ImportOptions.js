/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ImportOptions = {
    isNewSolution: false,
    isDefault: false,
    excelRowCount: 0,
    excelStatementList: null,
    previewTop: 0,
    headerColumnIdx: 1,
    dataColumnIdx: 2,
    oriSysColumnMapping: [],
    solutionModel: {},
    modifiedSolutionModel: {},
    bocModifiedSolutionModel: {},
    arrExcelHeader: null,
    sysStatementColumn: null,
    bocInitSettingFields: ["MTransAcctNo", "MTransAcctNo2", "MTransAcctName", "MTransAcctName2"],
    requiredSolutionFields: ['MDate', 'MSpentAmt', 'MReceivedAmt', 'MBalance'],//, 'MTransAcctNo', 'MTransAcctName'
    moneyFields: ['MSpentAmt', 'MReceivedAmt', 'MBalance'],
    centerFields: ['MDate', 'MTime'],
    sysColumnInitCnt: 0,
    bankType: $("#hidBankTypeId").val(),
    multiHeaderBanks: ["CCB"],
    multiHeaderMappings: { CCB: ["MSpentAmt", "MReceivedAmt"] },//第二行标题包含的字段
    ccbSecondHeaderValues: ['借方', '贷方'],
    likeBOCBanks: ["BOC"],
    multiSelectMapping: ["MTransAcctNo", "MTransAcctName"],//可以多选的字段
    secondHeaderObj: null,//针对建设银行有两个标题行的情况
    defaultTimeout: 3000,
    successMsgTimeout: 4500,//方案保存成功信息的持续时间
    isFirstLoad: true,
    //收/付款账户字段是否分开
    isRecPayFieldSplit: false,
    init: function () {
        $.mDialog.max();
        ImportOptions.adjustPreviewWidth();
        ImportOptions.isNewSolution = $("#hidIsNewSolution").val() == 'true';
        ImportOptions.previewTop = parseInt($("#hidPreviewTop").val());
        ImportOptions.arrExcelHeader = $("#hidExcelHeader").val().split(',');
        var excelStatement = $("#hidExcelStatement")[0].value;
        ImportOptions.solutionModel = eval('(' + $("#hidJsonSolution")[0].value + ')');
        ImportOptions.setName();
        ImportOptions.oriSolutionName = $("#txtSolution").val();
        ImportOptions.sysStatementColumn = eval('(' + $("#hidStatementColumn").val() + ')');
        ImportOptions.modifiedSolutionModel = ImportOptions.solutionModel;
        if (excelStatement != '') {
            ImportOptions.excelStatementList = eval('(' + excelStatement + ')');
            ImportOptions.excelRowCount = ImportOptions.excelStatementList.length;
        }
        ImportOptions.bindAction();
        if (ImportOptions.isNewSolution) {
            $('#txtHeaderRowStart').numberspinner('setValue', 1);
            $('#txtDataRowStart').numberspinner('setValue', 2);
            var list = ImportOptions.getSelectObjs();
            $.each(list, function (index, obj) {
                $(obj).combobox("setValue", '');
            });
        }
        ImportOptions.previewExcelRowData(ImportOptions.headerRowStart(), ImportOptions.headerColumnIdx);
        ImportOptions.previewExcelRowData(ImportOptions.dataRowStart(), ImportOptions.dataColumnIdx);

        ImportOptions.isRecPayFieldSplit = ImportOptions.isStRecPayFieldSplit();
        if (!ImportOptions.isNewSolution) {
            ImportOptions.initSysColumn();
            ImportOptions.previewMappingData();
        }
        if (localStorage.Message) {
            ImportOptions.saveBill(ImportOptions.modifiedSolutionModel, ImportOptions.successMsgTimeout);
            localStorage.clear();
        }
        ImportOptions.isFirstLoad = false;
    },
    setName: function (multiLang) {
        $("#divTmplName").mFormSetForm(multiLang || ImportOptions.solutionModel);
        $("#spSolution").text($("#txtSolution").val());
    },
    adjustPreviewWidth: function () {
        $('.mg-statement-import .preview').css('width', '100%').css('width', '-=450px');
    },
    getSelectObjs: function () {
        return $("#tbColumnMapping tbody td:nth-child(3) input[class*='multi-select']");
    },
    isMultiHeader: function () {
        var isMulti = $.inArray(ImportOptions.bankType, ImportOptions.multiHeaderBanks) != -1 || (ImportOptions.dataRowStart() - ImportOptions.headerRowStart()) == 2;
        if (isMulti) {
            ImportOptions.secondHeaderObj = ImportOptions.excelStatementList[ImportOptions.headerRowStart() + 1];
        }
        else {
            ImportOptions.secondHeaderObj = null;
        }
        return isMulti;
    },
    initSysColumn: function () {
        if (ImportOptions.excelStatementList != null) {
            var columnTds = $("#tbColumnMapping tbody td:nth-child(1)");
            var selectObjs = ImportOptions.getSelectObjs();
            var assignedList = [];
            var megiFieldList = $(selectObjs[0]).combobox("getData");
            for (var prop in ImportOptions.solutionModel) {
                if (ImportOptions.solutionModel.hasOwnProperty(prop)) {
                    $.each(columnTds, function (index, tdObj) {
                        //方案字段值等于Excel标题列，并且字段必须在匹配列表里面(导入Cash,Excel标头和MParent都是Cash，导致匹配加载错)
                        if (ImportOptions.solutionModel[prop] != '' && ImportOptions.solutionModel[prop] != null
                            && ((ImportOptions.solutionModel[prop] === $(tdObj).text() && megiFieldList.filter(function (item) { return item.Key == prop }).length > 0) ||
                            (ImportOptions.isMultiHeader() && $.inArray(prop, ImportOptions.multiHeaderMappings.CCB) != -1
                            && ImportOptions.solutionModel[prop] == ImportOptions.secondHeaderObj[ImportOptions.arrExcelHeader[index]]))) {

                            var selectedValue = $(selectObjs[index]).combobox("getValues");
                            if (selectedValue == '' || $.inArray(prop, selectedValue) != -1) {
                                $(selectObjs[index]).combobox("setValue", prop);
                            } else {
                                $(selectObjs[index]).combobox("setValues", selectedValue.concat([prop]));
                            }
                            assignedList.push(index);
                            ImportOptions.sysColumnInitCnt++;
                            return false;
                        }
                    });
                }
            }
            $.each(selectObjs, function (index, selectObj) {
                if ($.inArray(index, assignedList) == -1) {
                    $(selectObj).combobox("setValue", "");
                }
            });
            //判断收/付款账户字段是否分开
            if (ImportOptions.isRecPayFieldSplit) {
                $.each(columnTds, function (index, tdObj) {
                    ImportOptions.initRecPaySplitFieldMapping(selectObjs[index], $(tdObj).text());
                });
                //记录中行预设匹配的字段
                $.each(ImportOptions.bocInitSettingFields, function (i, val) {
                    ImportOptions.bocModifiedSolutionModel[val] = ImportOptions.modifiedSolutionModel[val];
                });
            }
            ImportOptions.saveMapping();
        }
    },
    //初始化收、付款分开字段的映射
    initRecPaySplitFieldMapping: function (selectObj, tdTxt, isClear) {
        if (!tdTxt) {
            return;
        }

        var isPayerAcctNo = tdTxt.indexOf("付款人账号") != -1 || tdTxt.indexOf("Debit Account No") != -1;
        var isPayeeAcctNo = tdTxt.indexOf("收款人账号") != -1 || tdTxt.indexOf("Payee's Account Number") != -1;
        var isPayerAcctName = tdTxt.indexOf("付款人名称") != -1 || tdTxt.indexOf("Payer's Name") != -1;
        var isPayeeAcctName = tdTxt.indexOf("收款人名称") != -1 || tdTxt.indexOf("Payee's Name") != -1;

        //非名义账户
        if (tdTxt.indexOf("名义") == -1) {
            if (isPayerAcctNo || isPayeeAcctNo) {
                if (isClear == undefined) {
                    $(selectObj).combobox("setValue", "MTransAcctNo");
                }
                if (isPayerAcctNo) {
                    ImportOptions.modifiedSolutionModel["MTransAcctNo"] = isClear ? "" : tdTxt;
                }
                else {
                    ImportOptions.modifiedSolutionModel["MTransAcctNo2"] = isClear ? "" : tdTxt;
                }
            }
            else if (isPayerAcctName || isPayeeAcctName) {
                if (isClear == undefined) {
                    $(selectObj).combobox("setValue", "MTransAcctName");
                }
                if (isPayerAcctName) {
                    ImportOptions.modifiedSolutionModel["MTransAcctName"] = isClear ? "" : tdTxt;
                }
                else {
                    ImportOptions.modifiedSolutionModel["MTransAcctName2"] = isClear ? "" : tdTxt;
                }
            }
        }
    },
    saveMapping: function () {
        var columnTds = $("#tbColumnMapping tbody td:nth-child(1)");
        var selectObjs = ImportOptions.getSelectObjs();
        $.each(columnTds, function (index, tdObj) {
            ImportOptions.oriSysColumnMapping.push($(selectObjs[index]).combobox("getValues"));
        });
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
    //开始行上下箭头点击事件
    onSpinClick: function (sender, isDownClick) {
        var txtRowStart = $(sender).closest("span.spinner").find("input:text");
        var rowStart = parseInt(txtRowStart.val()) - 1;
        var isHeader = txtRowStart.attr("id") == "txtHeaderRowStart";
        var columnIdx = isHeader ? ImportOptions.headerColumnIdx : ImportOptions.dataColumnIdx;
        ImportOptions.previewExcelRowData(rowStart, columnIdx);

        //修改的是标题开始行
        if (isHeader) {
            ImportOptions.onHeaderRowStartChanged();
        }
        //是否为递减操作
        else if (isDownClick) {
            ImportOptions.onDataRowStartChanged();
        }
        ImportOptions.switchTmplNameEdit();
        ImportOptions.previewMappingData();
    },
    bindAction: function () {
        //绑定开始行
        $('#txtHeaderRowStart,#txtDataRowStart').numberspinner({
            min: 1,
            max: ImportOptions.excelRowCount,
            editable: true,
            width: 43,
            onSpinUp: function () {
                ImportOptions.onSpinClick(this);
            },
            onSpinDown: function () {
                ImportOptions.onSpinClick(this, true);
            }
        });
        $('#txtHeaderRowStart').off("keyup").on("keyup", function () {
            if(this.value) {
                ImportOptions.previewExcelRowData(ImportOptions.headerRowStart(), ImportOptions.headerColumnIdx);
                ImportOptions.onHeaderRowStartChanged();
                ImportOptions.switchTmplNameEdit();
            }
        });
        $('#txtDataRowStart').off("keyup").on("keyup", function () {
            if(this.value) {
                ImportOptions.previewExcelRowData(ImportOptions.dataRowStart(), ImportOptions.dataColumnIdx);
                ImportOptions.switchTmplNameEdit();
                ImportOptions.previewMappingData();
            }
        });
        var prevValue = [];
        var selectHeader = null;
        ImportOptions.getSelectObjs().combobox({
            //添加必录标志
            formatter: function (item) {
                var result = [];
                if ($.inArray(item.Key, ImportOptions.requiredSolutionFields) != -1) {
                    result.push("<span style='color:red;'>*</span>");
                }
                result.push("<span class='item-text'>" + item.Value + "</span>");
                return result.join('');
            },
            onShowPanel: function () {
                prevValue = $(this).combobox("getValues");
                selectHeader = this;
            },
            onSelect: function (item) {
                if (item.Key != "") {
                    //unassign->分配一个字段
                    if (prevValue.length == 1 && prevValue[0] == "") {
                        $(this).combobox("setValue", item.Key);
                    }
                    ImportOptions.sysColumnInitCnt++;
                }
                else {//分配了一个或多个字段->unassign
                    $(this).combobox("setValue", "");
                }
                var isMappingExist = false;
                var isAllowDuplicate = false;
                //如果收/付款账户字段，并且是分开的，则不校验重复
                if (ImportOptions.isRecPayFieldSplit && $.inArray(item.Key, ImportOptions.multiSelectMapping) != -1) {
                    isAllowDuplicate = true;
                }
                if (!isAllowDuplicate) {
                    if (item.Key != '') {
                        var currentSelect = this;
                        $.each(ImportOptions.getSelectObjs(), function (index, selectObj) {
                            var selectedValues = $(selectObj).combobox("getValues");
                            if ($.inArray(item.Key, selectedValues) != -1 && currentSelect.id != selectObj.id) {
                                isMappingExist = true;
                                var confirmMsg = HtmlLang.Write(LangModule.Bank, "reassignMappingConfirmMsg", "{0} is already assigned to {1}, Would you like to continue to reassign the {0}?");
                                confirmMsg = confirmMsg.replace(/\{0\}/g, item.Value).replace('{1}', $(selectObj).closest("tr").find("td.excel-column").text());
                                $.mDialog.confirm(confirmMsg,
                                {
                                    height: 180,
                                    callback: function () {
                                        var curSelectValue = $(selectObj).combobox("getValues");
                                        curSelectValue = $.grep(curSelectValue, function (key) {
                                            return key != item.Key;
                                        });
                                        $(selectObj).combobox("setValue", curSelectValue);
                                        ImportOptions.onMappingChanged(prevValue, currentSelect);
                                    },
                                    cancelCallback: function () {
                                        $(currentSelect).combobox("setValues", prevValue.length == 0 ? [""] : prevValue);
                                    }
                                });

                                $("div[class*=confirm]").find("input[id*=cancel]").click(function () {
                                    $(currentSelect).combobox("setValues", prevValue);
                                });
                                return false;
                            }
                        });
                    }
                }
                if (!isMappingExist) {
                    var solutionHeader = $(selectHeader).closest("tr").find("td.excel-column").text();
                    ImportOptions.onMappingChanged(prevValue, this, solutionHeader);
                }
                $(this).combo("hidePanel");
            },
            onUnselect: function (item) {
                //分配了一个字段，反选变成unassign
                if (prevValue.length == 1) {
                    $(this).combobox("setValue", "");
                }
                else {
                    ImportOptions.modifiedSolutionModel[item.Key] = "";
                }
                ImportOptions.onMappingChanged(prevValue, this);
                //ImportOptions.previewMappingData();
                $(this).combo("hidePanel");
            },
            //使用Delete或Backspace清出所选匹配
            onChange: function (newValue, oldValue) {
                var oriLen = newValue.length;
                newValue = newValue.filter(Boolean);
                //如果包含空格，把空格去掉重新设值
                if (newValue.length < oriLen) {
                    $(this).combobox("setValues", newValue);
                    return;
                }

                if (oldValue.length > 1) {
                    oldValue = oldValue.filter(Boolean);
                }
                if (!ImportOptions.isFirstLoad) {
                    var isMappingChanged = false;
                    //清空手动删除的匹配
                    if (oldValue.length > newValue.length) {
                        $.each(oldValue, function (i, val) {
                            //处理所有删除，或者删除一部分匹配（需要校验文本是否存在）
                            if ($.inArray(val, newValue) == -1 && !ImportOptions.isComboTextExist(val, newValue)) {
                                ImportOptions.modifiedSolutionModel[val] = '';
                                isMappingChanged = true;
                            }
                        });
                    }
                    if (isMappingChanged) {
                        ImportOptions.onMappingChanged(oldValue, this);
                    }
                }
            }
        });
        $("#aSave").click(function () {
            ImportOptions.saveSolution();
        });
        $("#aBack").click(function () {
            var bankId = location.href.substring(location.href.lastIndexOf('/') + 1).split('?')[0];
            mWindow.reload("/BD/BDBank/Import?type=" + ImportOptions.bankType + "&id=" + bankId);
        });

        $("#aPreview").click(function () {
            ImportOptions.modifiedSolutionModel.MName = $("#txtSolution").is(":visible") > 0 ? $.trim($("#txtSolution").val()) : $("#spSolution").text();
            if (!$(".setting").mFormValidate()) {
                return;
            }
            if (!ImportOptions.validateMapping()) {
                return;
            }
            //设置页面隐藏
            $(".mg-statement-import").hide();
            //导入库数据预览
            $("#divDBDataPreview").show();

            $("#btnSave").show();
            $("#btnPreview").hide();
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
    isComboTextExist: function (oldVal, newValues) {
        var result = false;
        $.each(newValues, function (i, val) {
            if (val.indexOf(ImportOptions.getSelectedText([oldVal])) != -1) {
                result = true;
                return false;
            }
        });
        return result;
    },
    onDataRowStartChanged: function () {
        var headerStart = ImportOptions.headerRowStart() + 1;
        var dataStart = ImportOptions.dataRowStart() + 1;
        var multiHeaderInterval = 0;
        if (headerStart + multiHeaderInterval == dataStart) {
            $('#txtHeaderRowStart').numberspinner('setValue', headerStart - 1);
            ImportOptions.previewExcelRowData(ImportOptions.headerRowStart(), ImportOptions.headerColumnIdx);
            ImportOptions.onHeaderRowStartChanged();
            ImportOptions.switchTmplNameEdit();
        }
    },
    onHeaderRowStartChanged: function () {
        //解决先匹配后修改开始行导致方案匹配没更新问题
        ImportOptions.changeSolution();

        var headerStart = ImportOptions.headerRowStart();
        var dataStart = ImportOptions.dataRowStart();
        var multiHeaderInterval = 0;
        ImportOptions.isRecPayFieldSplit = ImportOptions.isStRecPayFieldSplit();

        //在导入标题行及数据行与预设方案不一致的对账单，在调整完标题行的时候，自动加载系统字段
        if (!ImportOptions.isNewSolution && ImportOptions.sysColumnInitCnt < ImportOptions.requiredSolutionFields.length) {
            ImportOptions.initSysColumn();
            ImportOptions.previewMappingData();
        }

        //自动调整数据开始行
        if (headerStart + multiHeaderInterval >= dataStart) {
            var newDataStart = headerStart + 2 + multiHeaderInterval;
            $('#txtDataRowStart').numberspinner('setValue', newDataStart);
            ImportOptions.previewExcelRowData(ImportOptions.dataRowStart(), ImportOptions.dataColumnIdx);
            ImportOptions.previewMappingData();
        }
        //给有空标题设置图标
        $.each(ImportOptions.getSelectObjs(), function (index, selectObj) {
            ImportOptions.IsEmptyColumn(selectObj);
        });
    },
    //如果为空标题，返回Megi字段ID，否则返回空
    IsEmptyColumn: function (selectObj) {
        var selectedVal = $(selectObj).combobox("getValues");
        var excelTd = $(selectObj).closest("tr").find(".excel-column");
        var colIdx = $("#tbColumnMapping tbody tr").index($(selectObj).closest("tr"));
        //已选择匹配且标题为空，并且第二行标题为空或非第二行标题字段对应标题为空
        if ($.trim(selectedVal) != '' && $.trim(excelTd.text()) == ''
            && (!ImportOptions.isMultiHeader() || $.inArray(selectedVal[0], ImportOptions.multiHeaderMappings.CCB) == -1 || $.trim(ImportOptions.secondHeaderObj[ImportOptions.arrExcelHeader[colIdx]]) == '')) {
            excelTd.text('');
            $("<span class='reg-error-image'></span>").show().appendTo(excelTd);
            return selectObj.id;
        }
        return '';
    },
    getDatetime: function () {
        var curDate = new Date();
        return curDate.getFullYear() + "/" + curDate.getMonth() + "/" + curDate.getDay() + " " + curDate.getHours() + ":" + curDate.getMinutes() + ":" + curDate.getSeconds();
    },
    switchTmplNameEdit: function () {
        if (ImportOptions.solutionModel.MIsDefault) {
            var lbl = $("#spSolution");
            var div = $("#divSolution");
            var txt = $("#txtSolution");
            if (ImportOptions.isSolutionChanged()) {
                if (txt.is(":hidden")) {
                    lbl.hide();
                    $.getJSON("/BD/BDBank/GetCurrentTime?id=" + (new Date()).toString(), function (curDt) {
                        var newSolution = {};
                        var multiLang = jQuery.extend(true, [], ImportOptions.solutionModel.MultiLanguage);
                        curDt = curDt.Data;
                        for (var i = 0; i < multiLang.length; i++) {
                            if (multiLang[i].MFieldName == "MName") {
                                multiLang[i].MMultiLanguageValue += ' ' + curDt;
                                for (var j = 0; j < multiLang[i].MMultiLanguageField.length; j++) {
                                    multiLang[i].MMultiLanguageField[j].MValue += ' ' + curDt;
                                }
                            }
                        }
                        newSolution.MultiLanguage = multiLang;
                        ImportOptions.setName(newSolution);
                    });
                    div.show();
                    txt.focus();
                }
            }
            else {
                ImportOptions.setName();
                lbl.show();
                txt.val(lbl.text());
                div.hide();
            }
        }
    },
    //触发方案修改
    changeSolution: function () {
        var trList = $("#tbColumnMapping tbody tr");
        //遍历映射列表行
        $.each(trList, function (i, tr) {
            var sysColumn = $(tr).find(".sys-column input");
            //匹配是否有值
            if (sysColumn.combobox("getValues").length > 0) {
                ImportOptions.onMappingChanged([], sysColumn[0]);
            }
        });
    },
    onMappingChanged: function (prevValue, currentSelect, header) {
        if (prevValue.length == 1) {
            if (prevValue[0] != "") {
                ImportOptions.modifiedSolutionModel[prevValue[0]] = '';
                ImportOptions.initRecPaySplitFieldMapping(currentSelect, header, true)
            }
        }
        var curSelectVal = $(currentSelect).combobox("getValues");
        if (curSelectVal.length > 0) {
            $.each(curSelectVal, function (idx, key) {
                var solutionHeader;
                //建行的收入支出金额从第二个标题行读取
                var isCCBMultiRowField = ImportOptions.isMultiHeader() && $.inArray(key, ImportOptions.multiHeaderMappings.CCB) != -1;
                if (isCCBMultiRowField) {
                    var colIdx = $("#tbColumnMapping tbody tr").index($(currentSelect).closest("tr"));
                    solutionHeader = ImportOptions.secondHeaderObj[ImportOptions.arrExcelHeader[colIdx]];
                }
                if (!isCCBMultiRowField || !solutionHeader || $.inArray($.trim(solutionHeader), ImportOptions.ccbSecondHeaderValues) == -1) {
                    solutionHeader = $(currentSelect).closest("tr").find("td.excel-column").text();
                }
                if (key) {
                    if (ImportOptions.isRecPayFieldSplit) {
                        ImportOptions.initRecPaySplitFieldMapping(currentSelect, solutionHeader);
                    }
                    ImportOptions.modifiedSolutionModel[key] = solutionHeader;
                }
            });
        }
        ImportOptions.previewMappingData();
        ImportOptions.switchTmplNameEdit();
    },
    isSolutionChanged: function () {
        var isChanged = false;
        if ($("#txtHeaderRowStart").val() != ImportOptions.solutionModel.MHeaderRowStart || $("#txtDataRowStart").val() != ImportOptions.solutionModel.MDataRowStart) {
            return true;
        }
        var selectObjs = ImportOptions.getSelectObjs();
        var oriMapping = ImportOptions.oriSysColumnMapping;
        $.each(selectObjs, function (index, selectObj) {
            var currentMapping = $(selectObj).combobox("getValues");
            //长度不同或值不同
            if (currentMapping.length != oriMapping[index].length
                || currentMapping.length == 1 && currentMapping.toString() != oriMapping[index].toString()) {
                isChanged = true;
            } else if (currentMapping.length > 1) {
                var sameCount = 0;
                $.each(currentMapping, function (idx, val) {
                    if ($.inArray(val, oriMapping[index]) != -1) {
                        sameCount++;
                    }
                });
                if (sameCount != oriMapping[index].length) {
                    isChanged = true;
                }
            }
        });
        return isChanged;
    },
    validateMapping: function () {
        //银行ID为空时禁止导入
        var bankId = $("#hidBankId").val();
        if (!bankId) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "bankIdIsEmpty", "Not find bank account,please select a bank account"));
            return false;
        }

        if (ImportOptions.dataRowStart() <= ImportOptions.headerRowStart()) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Bank, "TransactionStartInvolid", "Actual transactions starting line must be greater than the start line of the title!"));
            return false;
        }

        var emptyExcelHeaderList = [];
        var arrHeader = [];
        var arrDuplicateHeader = [];
        var arrRequiredField = ImportOptions.requiredSolutionFields.concat();
        $.each(ImportOptions.requiredSolutionFields, function (idx, field) {
            $.each(ImportOptions.getSelectObjs(), function (index, selectObj) {
                var emptySelId = ImportOptions.IsEmptyColumn(selectObj);
                if (emptySelId) {
                    emptyExcelHeaderList.push(emptySelId);
                }
                //从必填项中排除已填的
                if ($.inArray(field, $(selectObj).combobox("getValues")) != -1) {
                    arrRequiredField.splice($.inArray(field, arrRequiredField), 1);
                    return false;
                }
            });
        });
        //空标题
        if (emptyExcelHeaderList.length > 0) {
            $.mAlert(HtmlLang.Write(LangModule.Docs, "ExcelHeaderColumnNull", "导入文件的列标题不能为空, 请检查标题开始行是否正确!"));
            return false;
        }

        //找出重复的标题列
        $.each($("#tbColumnMapping .excel-column"), function (i, td) {
            var tdVal = $.trim($(td).text());
            if (tdVal) {
                if ($.inArray(tdVal, arrHeader) != -1) {
                    arrDuplicateHeader.push(tdVal);
                }
                arrHeader.push(tdVal);
            }
        });
        if (arrDuplicateHeader.length > 0) {
            $.each($("#tbColumnMapping .excel-column"), function (i, td) {
                if ($.inArray($.trim($(td).text()), arrDuplicateHeader) != -1) {
                    $(td).find(".reg-error-image").remove();
                    $("<span class='reg-error-image'></span>").show().appendTo($(td));
                }
            });
            $.mAlert(HtmlLang.Write(LangModule.Bank, "ExcelColumnHeadingDuplicate", "导入文件有重复的标题：{0}，请检查标题开始行是否正确！").replace('{0}', arrDuplicateHeader.toString()));
            return false;
        }
        if (arrRequiredField.length > 0) {
            var requiredFields = ImportOptions.getSelectedText(arrRequiredField);
            $.mAlert(HtmlLang.Write(LangModule.Docs, "ValidateSolutionRequired", "The required fields:{0} is unassigned.").replace('{0}', requiredFields));
            return false;
        }
        return true;
    },
    //判断收/付款账户是否为分开的字段（目前只有中行是分开的）
    isStRecPayFieldSplit: function () {
        var result = false;
        var isBOC = $.inArray(ImportOptions.bankType, ImportOptions.likeBOCBanks) != -1;
        if (isBOC) {
            var hasPayerAcctNo = false;
            var hasPayeeAcctNo = false;
            $.each($("#tbColumnMapping .excel-column"), function (i, td) {
                var tdTxt = $(td).text();
                //非名义账户
                if (tdTxt.indexOf("名义") == -1) {
                    if (tdTxt.indexOf("付款人账号") != -1 || tdTxt.indexOf("Debit Account No") != -1) {
                        hasPayerAcctNo = true;
                    }
                    else if (tdTxt.indexOf("收款人账号") != -1 || tdTxt.indexOf("Payee's Account Number") != -1) {
                        hasPayeeAcctNo = true;
                    }
                }
            });
            result = hasPayeeAcctNo && hasPayerAcctNo;
        }

        return result;
    },
    saveSolution: function () {
        var isChanged = false;
        ImportOptions.solutionModel = eval('(' + $("#hidJsonSolution")[0].value + ')');
        var isParentIDEmpty = ImportOptions.solutionModel.MParentID == '' || ImportOptions.solutionModel.MParentID == null;
        if (ImportOptions.isNewSolution
            || isParentIDEmpty
            || ImportOptions.isSolutionChanged()
            || ImportOptions.modifiedSolutionModel.MName != ImportOptions.oriSolutionName) {
            isChanged = true;
        }

        //如果收/付款账户是分开的字段，不管是否修改过匹配，都按默认匹配来处理
        if (ImportOptions.isRecPayFieldSplit) {
            var bocInitSettingFields = ["MTransAcctNo", "MTransAcctNo2", "MTransAcctName", "MTransAcctName2"];
            $.each(bocInitSettingFields, function (i, field) {
                ImportOptions.solutionModel[field] = ImportOptions.bocModifiedSolutionModel[field];
                ImportOptions.modifiedSolutionModel[field] = ImportOptions.bocModifiedSolutionModel[field];
            });
        }

        if (!isChanged) {
            ImportOptions.saveBill(ImportOptions.solutionModel);
            return;
        }

        ImportOptions.modifiedSolutionModel.MIsDefault = ImportOptions.solutionModel.MIsDefault;
        if (ImportOptions.modifiedSolutionModel.MIsDefault) {
            ImportOptions.modifiedSolutionModel.MItemID = '';
            ImportOptions.modifiedSolutionModel.MIsDefault = false;
        }
        ImportOptions.modifiedSolutionModel.MParentID = ImportOptions.solutionModel.MParentID;
        ImportOptions.modifiedSolutionModel.MHeaderRowStart = $("#txtHeaderRowStart").val();
        ImportOptions.modifiedSolutionModel.MDataRowStart = $("#txtDataRowStart").val();
        if (ImportOptions.isNewSolution || isParentIDEmpty) {
            ImportOptions.modifiedSolutionModel.MParentID = ImportOptions.bankType;
            ImportOptions.solutionModel.MParentID = ImportOptions.bankType;
        }
        if (!ImportOptions.isNewSolution && isParentIDEmpty) {
            ImportOptions.modifiedSolutionModel.IsUpdate = true;
        }
        if (ImportOptions.bankType == "HSBC" && !ImportOptions.modifiedSolutionModel.MDateFormat) {
            ImportOptions.modifiedSolutionModel.MDateFormat = "dd/MM/yyyy,dd/M/yyyy,d/M/yyyy,d/MM/yyyy,dd/MM/yy,dd/M/yy,d/M/yy,d/MM/yy";
        }

        //如果方案名有改动，则设置多语言对象
        if (ImportOptions.modifiedSolutionModel.MName != ImportOptions.oriSolutionName) {
            var solutionLang = $("#txtSolution").getLangEditorData();
            var langArray = new Array();
            langArray.push(solutionLang);
            ImportOptions.modifiedSolutionModel.MultiLanguage = langArray;
        }

        $("#divSolution").mFormSubmit({
            url: "/BD/BDBank/SaveImportSolution", param: { model: ImportOptions.modifiedSolutionModel }, callback: function (response) {
                if (response.Success == true) {
                    var msg = HtmlLang.Write(LangModule.Bank, "SolutionSaveSuccess", "The solution:{0} save Success!").replace("{0}", response.Tag);
                    var url = "/BD/BDBank/ImportOptions/" + $("#hidBankId").val() + "?solutionId=" + response.ObjectID + "&type=" + ImportOptions.bankType + "&fileName=" + Megi.getUrlParam("fileName");
                    localStorage.Message = msg;
                    var timeout = isChanged ? ImportOptions.successMsgTimeout : ImportOptions.defaultTimeout;
                    $.mMsg(msg, timeout);
                    mWindow.reload(url);
                } else {
                    if (response.Tag == "Exist") {
                        $("#aBackAgain").trigger("click");
                        $("#txtSolution").css({ "border": "1px solid red" });
                    }
                    $.mDialog.alert(response.Message);
                }
            }
        });
    },
    saveBill: function (solutionObj, timeout) {
        $("body").mask("");
        var obj = {};
        obj.MBankTypeID = ImportOptions.bankType;
        obj.MBankID = $("#hidBankId").val();
        obj.MFileName = $("#hidFileName").val();
        obj.ImportSolutionModel = solutionObj;
        mAjax.submit("/BD/BDBank/SaveImportBankBill", { model: obj }, function (response) {
            if (response.Success == true) {
                var responseTag = eval('(' + response.Tag + ')');
                var msg = HtmlLang.Write(LangModule.Bank, "ImportStatementSuccessMsg", "{0} statements were added.").replace("{0}", responseTag.Total);
                $.mMsg(msg, timeout);
                if (parent.location.href.indexOf("BDBankReconcileHome") != -1) {
                    new parent.BDBankReconcileHome().Reload("2", [responseTag.StartDate, responseTag.EndDate], function () {
                        $.mTab.refresh(HtmlLang.Write(LangModule.Common, "bankaccounts", "Bank Accounts"), '/BD/BDBank/BDBankHome', false, true);
                    });
                }
                else {
                    parent.location.reload();
                }
                $.mDialog.close();
            } else {
                $.mDialog.alert(response.VerificationInfor[0].Message);
                $("body").unmask();
            }
        });
    },
    addOrUpdateUrlParam: function (url, key, value) {
        var result = [];
        var arrSec = url.split('?');
        var addParam = key + '=' + value;
        if (arrSec.length == 2) {
            var found = false;
            var arrKV = arrSec[1].split('&');
            for (j = 0; j < arrKV.length; j++) {
                if (arrKV[j].split('=')[0] == key) {
                    arrKV[j] = key + "=" + value;
                    found = true;
                }
                result.push(arrKV[j]);
            }
            if (!found) {
                return url + '&' + addParam
            }
            return arrSec[0] + '?' + result.join('&');
        }
        return url + '?' + addParam
    },
    previewExcelRowData: function (rowIdx, previewColumnIdx) {
        if (ImportOptions.excelStatementList != null) {
            var index = 0;
            var columnTd = $("#tbColumnMapping tbody td:nth-child(" + previewColumnIdx + ")");
            if (rowIdx >= 0 && rowIdx < ImportOptions.excelStatementList.length) {
                var headerRow = ImportOptions.excelStatementList[rowIdx];
                $.each(headerRow, function (xlsHeader, data) {
                    var ellipStr = ImportOptions.ellipsisStr(data, columnTd[index]);
                    $(columnTd[index]).text(ellipStr);
                    index++;
                });
            }
        }
    },
    getSelectedText: function (selectedValuesOrText) {
        var selectedTexts = [];
        $.each(ImportOptions.sysStatementColumn, function (index, item) {
            //[{Key:"MSpentAmt", Value:"支出金额"},{Key:"MTime", Value:"交易时间"}] || [0]:支付金额,交易时
            if ($.inArray(item.Key, selectedValuesOrText) != -1 || selectedValuesOrText.length == 1 && selectedValuesOrText[0].indexOf(item.Value) != -1) {
                selectedTexts.push(item.Value);
            }
        });
        return selectedTexts;
    },
    previewMappingData: function () {
        if (ImportOptions.excelStatementList != null) {
            var tbPreviewMapping = $("#tbPreviewMapping");
            tbPreviewMapping.find("thead").text('');
            tbPreviewMapping.find("tbody").text('');
            var selectList = $("#tbColumnMapping tbody tr td:nth-child(3) input[class*='multi-select']");
            var header = '';
            var assignedColumnIndex = [];
            var moneyFieldIndex = [];
            var centerFieldIndex = [];
            var width = '';
            $.each(selectList, function (idx, selectObj) {
                var selectedValues = $(selectObj).combobox("getValues");
                if (selectedValues.length > 0 && selectedValues[0] != "") {
                    var selectedTexts = ImportOptions.getSelectedText(selectedValues);
                    if ($.inArray(selectedValues[0], ImportOptions.moneyFields) != -1) {
                        moneyFieldIndex.push(idx);
                    }
                    else if ($.inArray(selectedValues[0], ImportOptions.centerFields) != -1) {
                        centerFieldIndex.push(idx);
                    }
                    if ($.inArray(selectedValues[0], ['MDate', 'MTime', 'MSpentAmt', 'MReceivedAmt', 'MBalance', 'MTransType']) != -1) {
                        width = 'width:10%;';
                    }
                    else if ($.inArray(selectedValues[0], ['MTransNo', 'MTransAcctNo', 'MTransAcctName']) != -1) {
                        width = 'width:15%;';
                    }
                    else if ($.inArray(selectedValues[0], ['MDesc']) != -1) {
                        width = 'width:20%;';
                    }
                    var align = " style='text-align: center;" + width + "'";
                    header += "<td class='preview-mapping-header'" + align + ">" + selectedTexts.toString() + "<input type='hidden' value='" + selectedValues.toString() + "'/></td>";
                    assignedColumnIndex.push(idx);
                }
            });
            tbPreviewMapping.find("thead").append("<tr>" + header + "</tr>");
            for (var rowIdx = ImportOptions.dataRowStart() ; rowIdx < ImportOptions.excelStatementList.length; rowIdx++) {
                var columnIdx = 0;
                var tdTmp = '';
                var align = '';
                var dataRow = ImportOptions.excelStatementList[rowIdx];
                var emptyColumn = 0;
                $.each(dataRow, function (xlsHeader, data) {
                    if ($.inArray(columnIdx, assignedColumnIndex) != -1) {
                        //金额右对齐
                        if ($.inArray(columnIdx, moneyFieldIndex) != -1) {
                            align = " style='text-align: right;'";
                        } else if ($.inArray(columnIdx, centerFieldIndex) != -1) {
                            align = " style='text-align: center;'";
                        }
                        else {
                            align = "";
                        }
                        tdTmp += "<td" + align + "><div>" + (data == null ? '' : data) + "</div></td>";
                    }
                    if (!data) {
                        emptyColumn++;
                    }
                    columnIdx++;
                });
                //过滤掉所有数据为空的行
                if (tdTmp != '' && emptyColumn != columnIdx) {
                    var cssHide = rowIdx > (ImportOptions.previewTop - 1) ? " style='display:none;'" : "";
                    tbPreviewMapping.find("tbody").append("<tr" + cssHide + ">" + tdTmp + "</tr>");
                }
            }
        }
    },
    ellipsisStr: function (str, obj) {
        //var re = /[^\u4e00-\u9fa5]/;
        //var isChinese = !re.test(str);
        //var limitLen = isChinese ? 10 : 19;
        //if (str != null && str != '' && str.length > limitLen) {
        //    $(obj).attr("title", str);
        //    return str.substring(0, limitLen) + "...";
        //}
        return str == null ? '' : str;
    }
}

$(document).ready(function () {
    $(window).resize(function () {
        ImportOptions.adjustPreviewWidth();
    });
    ImportOptions.init();
    if ($.browser.mozilla) {
        ImportOptions.adjustPreviewWidth();
    }
});