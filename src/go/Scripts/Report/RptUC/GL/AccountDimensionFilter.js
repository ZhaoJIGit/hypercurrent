var AccountDimensionFilter = {
    itemId: $("#hideItemId").val(),
    rptType: $("#hideRptType").val(),
    data: $("#hideJsonData").val(),
    checkTypeList: null,
    compareList: null,
    logicList: null,
    fieldList: null,
    combobxDefaultHeight: 33,
    editorRowIndex: null,
    init: function () {
        AccountDimensionFilter.initCheckTypeList();
        AccountDimensionFilter.initConditionGrid();
        AccountDimensionFilter.initAction();

        AccountDimensionFilter.loadData();
    },
    initAction: function () {
        $("#btnUp").off("click").on("click", function () {
            AccountDimensionFilter.checkTypeMove(true);
        })

        $("#btnDown").off("click").on("click", function () {
            AccountDimensionFilter.checkTypeMove(false);
        });

        $("#ulCheckTypeList").find("li").each(function () {
            $(this).off("click").on("click", function () {

                $(".selected", "#ulCheckTypeList").removeClass("selected");

                $(this).addClass("selected");
            });
        });

        $("#btnSave").click(function () {
            AccountDimensionFilter.saveFilter();
        });

        $("#btnCancel").click(function () {
            $.mDialog.close();
        });


        $("body").click(function (e) {

            if (!$(e.srcElement).hasClass("datagrid-body")) {
                return;
            }

            if (AccountDimensionFilter.editorRowIndex != null) {
                $("#gridCondtion").datagrid("endEdit", AccountDimensionFilter.editorRowIndex);
                AccountDimensionFilter.editorRowIndex = null;
            }
        });

    },
    checkTypeMove: function (isMoveUp) {
        var ulDom = $("#ulCheckTypeList");
        var selectedDom = ulDom.find(".selected");
        if (!selectedDom || selectedDom.length == 0) {
            return;
        }
        //如果已经是第一个了或者最后一个,不进行操作
        var currentIndex = parseInt($(selectedDom).find("input").attr("index"));
        if ((isMoveUp && currentIndex == 0) || (!isMoveUp && currentIndex == ulDom.find("li").length - 1)) {
            return;
        }

        var switchTargetIndex = isMoveUp ? currentIndex - 1 : currentIndex + 1;
        var switchTarget = ulDom.find("input[index='" + switchTargetIndex + "']").parent();


        //交换index
        $(switchTarget).find("input").attr("index", currentIndex);
        $(selectedDom).find("input").attr("index", switchTargetIndex);

        isMoveUp ? $(switchTarget).before(selectedDom) : $(switchTarget).after(selectedDom);

        //滚动条的操作，如果选择的li的位置和ul的位置的差大于ul的高度，说明有滚动
        var selectedDomY = $(selectedDom).offset().top;
        //ul的开始位置
        var ulY = $(ulDom).parent().offset().top;

        $(ulDom).animate({ scrollTop: selectedDomY - ulY }, 150);
    },
    //初始加载核算项目
    initCheckTypeList: function () {
        var url = "/BD/BDAccount/GetCheckTypeList";

        AccountDimensionFilter.getCheckTypeDataList(function (data) {
            var ulDom = $("#ulCheckTypeList");

            for (var i = 0; i < data.length; i++) {
                var checkType = data[i];

                var liDom = '<li><input type="checkbox" value="' + checkType.MType + '" index="' + i + '"/><span style="padding-left:5px">' + checkType.MName + '</span></li>';

                $(ulDom).append(liDom);
            }
        });
    },
    getCheckTypeDataList: function (callback) {
        var url = "/BD/BDAccount/GetCheckTypeList";

        if (AccountDimensionFilter.checkTypeList != null) {
            callback(AccountDimensionFilter.checkTypeList);
            return;
        }

        mAjax.post(url, {}, function (data) {
            if (data && data.length > 0) {
                AccountDimensionFilter.checkTypeList = data;
                if (callback && $.isFunction(callback)) {
                    callback(data);
                }
            }
        }, false, false, false);
    },
    initConditionGrid: function () {
        $("#gridCondtion").datagrid({
            columns: [[
               {
                   field: 'MItemID', title: "", width: 40, formatter: function (val, rec, rowIndex) {
                       return '<a href="javascript:void(0)" class="m-icon-add-row" rowindex="' + rowIndex + '">&nbsp;</a>';
                   }
               },
              {

                  field: 'MField', title: HtmlLang.Write(LangModule.Report, "Field", "字段"), width: 120, editor: {
                      type: "combobox",
                      options: {
                          valueField: "MType",
                          textField: "MName",
                          required: false,
                          height: AccountDimensionFilter.combobxDefaultHeight,
                          data: AccountDimensionFilter.getGridEditorData("MField"),
                          onSelect: function (data) {
                              //if (changeEvent) { changeEvent(); }

                          },
                          onLoadSuccess: function () {
                              //if (loadSuccessEvent) { loadSuccessEvent() };
                          }
                      }
                  },
                  formatter: function (value, row, index) {
                      if (value == "" || value == undefined) {
                          return "";
                      }

                      var value = AccountDimensionFilter.getFieldValueById(value, "MField");

                      return value;

                  }
              },
              {
                  field: 'MCompare', title: HtmlLang.Write(LangModule.Report, "Compare", "比较"), width: 96, editor: {
                      type: "combobox",
                      options: {
                          valueField: "MType",
                          textField: "MName",
                          required: true,
                          height: AccountDimensionFilter.combobxDefaultHeight,
                          data: AccountDimensionFilter.getGridEditorData("MCompare"),
                          onSelect: function (data) {
                              //if (changeEvent) { changeEvent(); }

                          },
                          onLoadSuccess: function () {
                              //if (loadSuccessEvent) { loadSuccessEvent() };
                          }
                      }
                  },
                  formatter: function (value, row, index) {
                      if (value == "" || value == undefined) {
                          return "";
                      }

                      var value = AccountDimensionFilter.getFieldValueById(value, "MCompare");

                      return value;

                  }
              },
              {
                  field: 'MCompareValue', title: HtmlLang.Write(LangModule.Report, "CompareValue", "比较值"), width: 100, editor: {
                      type: "validatebox"
                  }
              },
              {
                  field: 'MLogic', title: HtmlLang.Write(LangModule.Report, "Logic", "逻辑"), width: 100, editor: {
                      type: "combobox",
                      options: {
                          valueField: "MType",
                          textField: "MName",
                          required: true,
                          height: AccountDimensionFilter.combobxDefaultHeight,
                          data: AccountDimensionFilter.getGridEditorData("MLogic"),
                          onSelect: function (data) {
                              //if (changeEvent) { changeEvent(); }

                          },
                          onLoadSuccess: function () {
                              $(this).combobox("setValue", "");
                          }
                      }
                  },
                  formatter: function (value, row, index) {
                      if (value == "" || value == undefined) {
                          return "";
                      }

                      var value = AccountDimensionFilter.getFieldValueById(value, "MLogic");

                      return value;

                  }
              },
              {
                  title: HtmlLang.Write(LangModule.Report, "Operation", "操作"), align: 'center', width: 40, field: "Op", formatter: function (val, rec, rowIndex) {
                      return "<div class='list-item-action'><a href='javascript:void(0);' row-index='" + rowIndex + "' class='list-item-del'></a></div>";
                  }
              }
            ]],
            onClickCell: function (rowIndex, field, value) {

                if (field == "MItemID") {
                    AccountDimensionFilter.insertEmptyRow(rowIndex + 1);
                    return;
                }

                if (field == "Op") {
                    AccountDimensionFilter.deleteRow(rowIndex);
                    var rows = $(this).datagrid("getRows");

                    if (rows == 0) {
                        AccountDimensionFilter.initConditionGrid();
                    }

                    return;
                }

                $(this).datagrid("beginEdit", rowIndex);

                AccountDimensionFilter.endEditRow();

                AccountDimensionFilter.editorRowIndex = rowIndex;
            },
            onLoadSuccess: function () {
                AccountDimensionFilter.initDataGridData();
            }
        });

        $("#gridCondtion").datagrid("loadData", []);
    },
    endEditRow: function () {
        if (AccountDimensionFilter.editorRowIndex != null) {
            $("#gridCondtion").datagrid("endEdit", AccountDimensionFilter.editorRowIndex);
        }
    },
    initDataGridData: function () {

        var rows = $("#gridCondtion").datagrid("getRows");

        if (!rows || rows.length == 0) {
            var emptyRow = AccountDimensionFilter.getDataGridEmptyRow();

            $("#gridCondtion").datagrid("insertRow", { index: 0, row: emptyRow });
        }

    },
    insertEmptyRow: function (index) {
        var emptyRow = AccountDimensionFilter.getDataGridEmptyRow();

        $("#gridCondtion").datagrid("insertRow", { index: index, row: emptyRow });
    },
    deleteRow: function (index) {
        $("#gridCondtion").datagrid("deleteRow", index);
    },
    getGridEditorData: function (field) {
        var result = new Array();

        if (field == "MField") {

            if (AccountDimensionFilter.fieldList != null) {
                result = AccountDimensionFilter.fieldList;
            } else {
                var amountArray = new Array();

                var initBalance = { "MType": "110", "MName": HtmlLang.Write(LangModule.Report, "InitBalance", "期初余额") };
                amountArray.push(initBalance);

                var debitBalance = { "MType": "120", "MName": HtmlLang.Write(LangModule.Report, "DebitBalance", "借方发生额") };
                amountArray.push(debitBalance);

                var creditBalance = { "MType": "130", "MName": HtmlLang.Write(LangModule.Report, "CreditBalance", "贷方发生额") };
                amountArray.push(creditBalance);

                var ytdDebitBalance = { "MType": "140", "MName": HtmlLang.Write(LangModule.Report, "YTDDebitBalance", "本年累计借方发生额") };
                amountArray.push(ytdDebitBalance);

                var ytdCreditBalance = { "MType": "150", "MName": HtmlLang.Write(LangModule.Report, "YTDCreditBalance", "本年累计贷方发生额") };
                amountArray.push(ytdCreditBalance);


                var endBalance = { "MType": "160", "MName": HtmlLang.Write(LangModule.Report, "EndBalance", "期末余额") };
                amountArray.push(endBalance);

                var checkTypeData = AccountDimensionFilter.getCheckTypeDataList(function (checkTypeList) {
                    if (checkTypeList) {
                        result = amountArray.concat(checkTypeList);
                    }
                });

                AccountDimensionFilter.fieldList = result;
            }
        } else if (field == "MCompare") {

            if (AccountDimensionFilter.compareList != null) {
                result = AccountDimensionFilter.compareList;
            } else {
                var equal = { "MType": 1, "MName": HtmlLang.Write(LangModule.Report, "Equal", "等于") };
                result.push(equal);

                var notEqual = { "MType": 2, "MName": HtmlLang.Write(LangModule.Report, "NotEqual", "不等于") };
                result.push(notEqual);

                var greaterThan = { "MType": 3, "MName": HtmlLang.Write(LangModule.Report, "GreaterThan", "大于") };
                result.push(greaterThan);

                var greaterThanOrEqual = { "MType": 4, "MName": HtmlLang.Write(LangModule.Report, "GreaterThanOrEqual", "大于等于") };
                result.push(greaterThanOrEqual);

                var lessThan = { "MType": 5, "MName": HtmlLang.Write(LangModule.Report, "LessThan", "小于") };
                result.push(lessThan);

                var lessThanOrEqual = { "MType": 6, "MName": HtmlLang.Write(LangModule.Report, "LessThanOrEqual", "小于等于") };
                result.push(lessThanOrEqual);

                var include = { "MType": 7, "MName": HtmlLang.Write(LangModule.Report, "Include", "包含") };
                result.push(include);

                var notInclude = { "MType": 8, "MName": HtmlLang.Write(LangModule.Report, "NotInclude", "不包含") };
                result.push(notInclude);

                var isNull = { "MType": 9, "MName": HtmlLang.Write(LangModule.Report, "IsNull", "为空") };
                result.push(isNull);

                var isNotNull = { "MType": 10, "MName": HtmlLang.Write(LangModule.Report, "IsNotNull", "不为空") };
                result.push(isNotNull);

                AccountDimensionFilter.compareList = result;
            }


        } else if (field == "MLogic") {

            if (AccountDimensionFilter.logicList != null) {
                result = AccountDimensionFilter.logicList;
            } else {
                var and = { "MType": 1, "MName": HtmlLang.Write(LangModule.Report, "And", "且") };
                var or = { "MType": 2, "MName": HtmlLang.Write(LangModule.Report, "Or", "或") };
                result.push(and);
                result.push(or);

                AccountDimensionFilter.logicList = result;
            }
        }

        return result;
    },
    getFieldValueById: function (value, field) {
        var result = "";
        if (value == "") {
            return result;
        }
        if (field == "MField") {
            if (AccountDimensionFilter.fieldList) {
                for (var i = 0 ; i < AccountDimensionFilter.fieldList.length; i++) {
                    var fieldName = AccountDimensionFilter.fieldList[i].MType;
                    if (fieldName == value) {
                        result = AccountDimensionFilter.fieldList[i].MName;
                        break;
                    }
                }
            }
        } else if (field == "MCompare") {
            if (AccountDimensionFilter.compareList) {
                for (var i = 0 ; i < AccountDimensionFilter.compareList.length; i++) {
                    var fieldName = AccountDimensionFilter.compareList[i].MType;
                    if (fieldName == value) {
                        result = AccountDimensionFilter.compareList[i].MName;
                        break;
                    }
                }
            }
        } else if (field == "MLogic") {
            if (AccountDimensionFilter.logicList) {
                for (var i = 0 ; i < AccountDimensionFilter.logicList.length; i++) {
                    var fieldName = AccountDimensionFilter.logicList[i].MType;
                    if (fieldName == value) {
                        result = AccountDimensionFilter.logicList[i].MName;
                        break;
                    }
                }
            }
        }

        return result;
    },
    getDataGridEmptyRow: function () {
        var row = {};
        row.MField = "";
        row.MCompare = "";
        row.MCompareValue = "";
        row.MLogic = "";

        return row;
    },
    isValidateRow: function (row) {

        if (AccountDimensionFilter.isEmptyRow(row)) {
            if (AccountDimensionFilter.editorRowIndex != null) {
                var editors = $("#gridCondtion").datagrid("getEditors", AccountDimensionFilter.editorRowIndex);

                for (var i = 0 ; i < editors.length; i++) {
                    var validateDom = $(editors[i].target).next().find(".validatebox-invalid");
                    if (validateDom) {
                        $(validateDom).removeClass("validatebox-invalid").removeClass("validatebox-text");
                    }
                }
            }

            return true;
        }

        if (!row.MCompare || !row.MLogic) {
            return false;
        }

        return true;
    },
    isEmptyRow: function (row) {
        if (row.MCompare == "" && row.MLogic == "" && row.MField == "" && row.MCompareValue == "") {
            return true;
        }

        return false;
    },
    saveFilter: function () {

        var conditionRows = $("#gridCondtion").datagrid("getRows");

        if (conditionRows && conditionRows.length > 0) {
            for (var i = 0; i < conditionRows.length; i++) {
                var row = conditionRows[i];

                if (!AccountDimensionFilter.isValidateRow(row)) {
                    $("#tt").tabs("select", 1);
                    return false;
                }
            }
        }

        //如果有编辑行，先结束编辑
        AccountDimensionFilter.endEditRow();


        if (!$("body").mFormValidate()) {
            //切换到条件选择页面
            $("#tt").tabs("select", 1);
            return false;
        }


        var params = {};

        params.MItemID = $("#hideItemId").val();
        params.MName = $("#tbxFilterSchemeName").val();
        params.MReportType = AccountDimensionFilter.rptType;
        params.MContent = {};

        var content = {};
        content.CheckGroup = AccountDimensionFilter.getCheckGroupArray();

        if (!content.CheckGroup || content.CheckGroup.length == 0) {
            var tips = HtmlLang.Write(LangModule.Report, "NoSelectCheckType", "没有选择核算维度！");
            $.mDialog.alert(tips);
            return;
        }

        content.Condition = AccountDimensionFilter.getConditionArray();

        //验证一下所选核算维度条件是否选择
        var isNotValidateCheckTypeName = new Array();
        if (content.Condition && content.Condition.length > 0) {
            for (var i = 0; i < content.Condition.length; i++) {
                var condition = content.Condition[i];

                if (condition.MField < 100) {
                    var isValidate = false;
                    for (var j = 0 ; j < content.CheckGroup.length; j++) {
                        var checkType = content.CheckGroup[j];

                        if (checkType.FieldType == condition.MField) {
                            isValidate = true;
                            break;
                        }
                    }

                    if (!isValidate) {
                        var name = AccountDimensionFilter.getCheckTypeName(condition.MField);
                        isNotValidateCheckTypeName.push(name);
                    }
                }
            }
            var length = isNotValidateCheckTypeName.length
            if (length > 0) {
                //提示错误
                var tips = HtmlLang.Write(LangModule.Report, "NotLegalConditionField", "以下条件字段在维度组合中没有选择，不能作为查询条件:") + "\\n";

                for (var i = 0; i < length; i++) {
                    tips += isNotValidateCheckTypeName[i] + ",";
                }

                $.mDialog.alert(tips);
                return;
            }
        }

        var contentJsonString = JSON.stringify(content);

        params.MContent = contentJsonString;

        $("body").mFormSubmit({
            url: "/Report/RptAccountDimensionSummary/SaveFilterScheme", param: { schemeModel: params }, callback: function (response) {
                if (response.Success) {
                    var successMsg = AccountDimensionFilter.itemId ? HtmlLang.Write(LangModule.Report, "FilterSchemeUpdated", "过滤方案修改成功！")
                        : HtmlLang.Write(LangModule.Report, "FilterSchemeSaved", "过滤方案新增成功");
                    $.mDialog.message(successMsg);

                    $.mDialog.close();
                } else {
                    var message = response.Message ? response.Message : HtmlLang.Write(LangModule.Report, "FilterSchemeSaveFail", "过滤方案保存失败！");
                    $.mDialog.alert(message);
                }
            }
        });
    },
    getContectJsonString: function () {
        var content = {};
        content.CheckGroup = AccountDimensionFilter.getCheckGroupArray();
        content.Condition = AccountDimensionFilter.getConditionArray();

        var result = JSON.stringify(content);

        return result;
    },
    getCheckGroupArray: function () {
        var checkList = $("#ulCheckTypeList").find("input:checked");

        if (!checkList || checkList.length == 0) {
            return new Array();
        }
        var checkItemList = new Array();
        for (var i = 0; i < checkList.length; i++) {
            var checkbox = checkList[i];

            var item = {};

            item.FieldType = $(checkbox).attr("value");
            item.Index = $(checkbox).attr("index");
            checkItemList.push(item);
        }

        return checkItemList;
    },
    //获取条件组合
    getConditionArray: function () {
        var rows = $("#gridCondtion").datagrid("getRows");

        if (!rows || rows.length == 0) {
            return new Array();
        }

        var conditionList = new Array();
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];

            if (row.MField == "" || row.MCompare == "") {
                continue
            }

            if (row.MLogic == "") {
                row.MLogic = 0;
            }

            conditionList.push(row);
        }
        return conditionList;
    },
    loadData: function () {
        if (!AccountDimensionFilter.itemId) {
            return;
        }

        var url = "/Report/RptAccountDimensionSummary/GetFilterScheme?id=" + AccountDimensionFilter.itemId;

        $("body").mFormGet({
            url: url, callback: function (data) {
                if (data) {
                    //维度数据
                    //如果没有设置方案数据，下面不做处理
                    if (!data.MContent) {
                        return;
                    }

                    var content = JSON.parse(data.MContent);

                    var checktypeList = content.CheckGroup;
                    var condition = content.Condition;

                    if (checktypeList && checktypeList.length > 0) {

                        var targetLiDom = $("#ulCheckTypeList").find("input[index='" + 0 + "']").parent();
                        var targetLiIndex = $(targetLiDom).find("input").attr("index");
                        for (var i = 0; i < checktypeList.length; i++) {
                            var fieldType = checktypeList[i].FieldType;
                            var index = checktypeList[i].Index;

                            var checkTypeInput = $("#ulCheckTypeList").find("input[value='" + fieldType + "']");

                            $(checkTypeInput).attr("checked", "checked");
                            var checkTypeIndex = $(checkTypeInput).attr("index");
                            //如果是第一个了
                            if (checkTypeIndex == targetLiIndex) {
                                targetLiDom = $("#ulCheckTypeList").find("input[index='" + (checkTypeIndex + 1) + "']").parent();
                                targetLiIndex = $(targetLiDom).find("input").attr("index");
                                continue;
                            }

                            var currentLiDom = $(checkTypeInput).parent();
                            $(targetLiDom).before(currentLiDom);
                        }
                    }

                    if (condition && condition.length > 0) {
                        $("#gridCondtion").datagrid("loadData", condition);
                    }
                }
            }
        });
    },
    getCheckTypeName: function (checkTypeId) {
        var result = "";
        var checkTypeList = AccountDimensionFilter.checkTypeList;
        if (checkTypeList && checkTypeList.length > 0) {
            for (var i = 0; i < checkTypeList.length; i++) {
                var checkType = checkTypeList[i];

                if (checkType.MType == checkTypeId) {
                    result = checkType.MName;

                    break;
                }
            }
        }

        return result;
    }
}