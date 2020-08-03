var SPEditBase1 = {
    EditorHeight: "26px",
    SalaryPayID: "",
    Selector: "#tbPaySalaryDetail",
    Disabled: false,
    DataSourcePayItem: [],
    DataSourceEntry: [],
    RowIndex: -1,//当前编辑行索引
    ExchangeRate: 1,//当前汇率
    init: function (salaryPayID, entryDataSource, disabled) {

        if (disabled == undefined) {
            disabled = false;
        }

        SPEditBase1.SalaryPayID = salaryPayID;

        SPEditBase1.Disabled = disabled;

        //初始化工资项目
        SPEditBase1.initPayItem(function () {

            //设置明细列表数据源
            SPEditBase1.setDataSourceEntry(entryDataSource, true);

            //绑定明细列表
            SPEditBase1.bindGrid();

            //更新明细列表数据源
            SPEditBase1.updateDataSourceEntry();

            //将当前页面设置为稳定状态
            $.mTab.setStable();
        });
    },
    //验证信息
    valideInfo: function () {
        var result = true;
        for (var i = 0; i < SPEditBase1.DataSourceEntry.length; i++) {
            var item = SPEditBase1.DataSourceEntry[i];
            if (item.MItemID == "" && item.MDesc == "" && item.MAmountFor == "") {
                continue;
            }
            if (item.MItemID == "") {
                $(SPEditBase1.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MItemID']").addClass("row-error");
                $(".datagrid-editable-input").css("height", "28px");
                result = false;
            }
            if (item.MPrice == "") {
                $(SPEditBase1.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MPrice']").addClass("row-warning");
            }
            if (item.MQty == "") {
                $(SPEditBase1.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MQty']").addClass("row-warning");
            }
            if (item.MAmountFor == "") {
                $(SPEditBase1.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MAmountFor']").addClass("row-warning");
                result = false;
            }
        }
        return result;
    },
    //获取明细实体列表
    getViewInfo: function (withId) {
        SPEditBase1.endEditGrid();
        SPEditBase1.updateDataSourceEntry();
        var currencyId = $('#selCurrency').combobox('getValue');
        var totalAmtFor = 0;
        var obj = {};
        var arr = new Array();
        for (var i = 0; i < SPEditBase1.DataSourceEntry.length; i++) {
            var item = SPEditBase1.DataSourceEntry[i];
            if (item.MItemID == "" && item.MDesc == "" && item.MAmount == "") {
                continue;
            }
            if (withId != undefined && withId == false) {
                item.MEntryID = "";
            }
            arr.push(item);
            totalAmtFor += parseFloat(item.MAmountFor);
        }
        obj.InvoiceEntry = arr;
        obj.MCyID = currencyId;
        obj.MTotalAmountFor = totalAmtFor;
        obj.MTotalAmount = parseFloat(totalAmtFor) / parseFloat(SPEditBase1.ExchangeRate);
        obj.MExchangeRate = SPEditBase1.ExchangeRate;
        return obj;
    },
    //初始化工资项目
    initPayItem: function (callback) {
        Megi.Ajax.post("/PA/PayItem/GetSalaryItemList", null, function (msg) {
            SPEditBase1.DataSourcePayItem = msg;
            callback();
        })
    },
    //更新数据源
    updateDataSourceEntry: function (keyValue) {
        for (var i = 0; i < SPEditBase1.DataSourceEntry.length; i++) {
            if (i == SPEditBase1.RowIndex) {
                if (keyValue != undefined) {
                    eval("SPEditBase1.DataSourceEntry[i]." + keyValue);
                }
            }
        }
        SPEditBase1.updateSummaryInfo();
    },
    //获取行数据的汇总
    getAmount: function (rowIndex) {
        var vQty = SPEditBase1.DataSourceEntry[rowIndex].MQty;
        var vPrice = SPEditBase1.DataSourceEntry[rowIndex].MPrice;
        var amount = "";
        if (vQty != undefined && vQty != "" && vPrice != undefined && vPrice != "") {
            amount = parseFloat(vQty) * parseFloat(vPrice);
        }
        return amount;
    },
    //更新合计信息
    updateSummaryInfo: function () {
        var amtFor = 0;
        var arrTax = new Array();
        for (var i = 0; i < SPEditBase1.DataSourceEntry.length; i++) {
            var item = SPEditBase1.DataSourceEntry[i];
            if (item.MAmountFor != "") {
                amtFor += parseFloat(item.MAmountFor);   
            }
        }
        if (amtFor == 0) {
            amtFor = "0.00";
        }

        $("#spTotal").html(Megi.Math.toDecimal(amtFor, 2));
        //计算个人所得税
        $("#spNet").html(Megi.Math.toDecimal(amtFor, 2));

    },
    //获取工资项目名称
    getPayItemCode: function (itemId) {
        if (SPEditBase1.DataSourcePayItem) {
            for (var i = 0; i < SPEditBase1.DataSourcePayItem.length; i++) {
                if (SPEditBase1.DataSourcePayItem[i].MItemID == itemId) {
                    return SPEditBase1.DataSourcePayItem[i].MName;
                }
            }
        }
        return itemId;
    },
    //重新设置数据源
    setDataSourceEntry: function (entryDataSource, addDefaultRow) {
        var defaultRow = 5;
        if (entryDataSource == null) {
            entryDataSource = new Array();
        }
        var rowIndex = 0;
        for (var i = 0; i < entryDataSource.length; i++) {
            entryDataSource[i].RowIndex = rowIndex;
            rowIndex += 1;
        }
        if (addDefaultRow == undefined || addDefaultRow == true) {
            var dataLength = entryDataSource.length;
            var emptyRow = 1;
            if (dataLength < defaultRow) {
                emptyRow = defaultRow - 1 - dataLength + emptyRow;
            }
            for (var i = 0; i < emptyRow; i++) {
                var obj = SPEditBase1.getEmptyEntry();
                obj.RowIndex = rowIndex;
                entryDataSource.push(obj);
                rowIndex += 1;
            }
        }
        SPEditBase1.DataSourceEntry = entryDataSource;
    },
    //删除数据源中指定的记录（根据索引删除）
    deleteDataSourceEntry: function (rowIndex) {
        if (SPEditBase1.DataSourceEntry.length <= 1) {
            //$.mDialog.alert("You must have at least 1 line item.", null, LangModule.IV, "AtLeastOneLineItem");
            return false;
        }
        var arr = new Array();
        for (var i = 0; i < SPEditBase1.DataSourceEntry.length; i++) {
            if (i != rowIndex) {
                var obj = SPEditBase1.DataSourceEntry[i];
                obj.RowIndex = i;
                arr.push(obj);
            }
        }
        SPEditBase1.DataSourceEntry = arr;
        return true;
    },
    //插入一条记录到数据源中（根据索引插入）
    insertDataSourceEntry: function (rowIndex) {
        var newRow = null;
        var arr = new Array();
        var index = 0;
        for (var i = 0; i < SPEditBase1.DataSourceEntry.length; i++) {
            if (SPEditBase1.DataSourceEntry[i].RowIndex == rowIndex) {
                newRow = SPEditBase1.getEmptyEntry();
                newRow.RowIndex = index;
                arr.push(newRow);
                index += 1;
            }
            var obj = SPEditBase1.DataSourceEntry[i];
            obj.RowIndex = index;
            arr.push(obj);
            index += 1;
        }
        SPEditBase1.DataSourceEntry = arr;
        return newRow;
    },
    //绑定明细列表
    bindGrid: function () {
        var columns = SPEditBase1.Columns.getArray();
        Megi.grid(SPEditBase1.Selector, {
            columns: columns,
            resizable: true,
            auto: true,
            data: SPEditBase1.DataSourceEntry,
            singleSelect: true,
            //单据单元格时，设置当前双击的行为可编辑状态
            onClickCell: function (rowIndex, field, value) {
                if (SPEditBase1.Disabled || field == "MEntryID" || field == "MAddID") {
                    return;
                }
                SPEditBase1.endEditGrid();
                SPEditBase1.RowIndex = rowIndex;
                Megi.grid(SPEditBase1.Selector, "beginEdit", rowIndex);
                var editor = $(SPEditBase1.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    if (field == "MItemID") {
                        $(editor.target).combobox("showPanel").combobox("textbox").focus();
                    }
                    else {
                        editor.target.select();
                    }
                    $(".datagrid-body").find(".numberbox,.datagrid-editable-input,.validatebox-text").click(function () {
                        $(this).select();
                    });
                }
            },
            //单击列表行时，设置列表为不可编辑状态
            onClickRow: function (rowIndex, rowData) {
                if (SPEditBase1.Disabled) {
                    return;
                }
                SPEditBase1.bindGridEditorEvent(rowData, rowIndex);
                SPEditBase1.bindGridEditorTabEvent(rowData, rowIndex);
            },
            onAfterEdit: function () {
                SPEditBase1.updateDataSourceEntry();
            }
        });
        Megi.grid(SPEditBase1.Selector, "selectRow", 0);
    },
    //给明细列表控件绑定 Tab 事件
    bindGridEditorTabEvent: function (rowData, rowIndex) {
        $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input").each(function () {
            $(this).keyup(function (event) {
                if (event.which == 13) {
                    //当前编辑行的所有控件
                    var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                    //当前正在编辑的控件的索引
                    var editing_controls_index = editing_controls.index(this);
                    //如果当前正在编辑的控件是该编辑行的最后一个控件
                    if (editing_controls_index == editing_controls.length - 1) {
                        //则结束当前编辑行，开始进行下一行编辑
                        SPEditBase1.beginNextRowEdit(rowIndex);
                        editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                        if (editing_controls.length > 0) {
                            //设置下一编辑行的第一个控件焦点
                            editing_controls[0].focus();
                        }
                    } else {
                        //将焦点移动到下一个控件
                        editing_controls[editing_controls_index + 1].focus();
                    }
                }
            }).keydown(function (event) {
                if (event.which == 9) {
                    if ($(this).closest(".datagrid-editable").closest("td").next().attr("field") == "MAmountFor") {
                        SPEditBase1.beginNextRowEdit(rowIndex);
                    }
                }
            }).focus(function (event) {
                var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                var editing_controls_index = editing_controls.index(this);
                var field = $(editing_controls[editing_controls_index]).closest("td[field]").attr("field");
                var editor = $(SPEditBase1.Selector).datagrid('getEditor', { index: rowIndex, field: field });
                if (editor != null) {
                    if (field == "MItemID") {
                        $(editor.target).combobox("showPanel");
                    } else {
                        editor.target.select();
                    }
                }
            });
        });
    },
    //开始下一行编辑
    beginNextRowEdit: function (rowIndex) {
        SPEditBase1.endEditGrid();
        var newRowIndex = rowIndex + 1;
        SPEditBase1.RowIndex = newRowIndex;
        Megi.grid(SPEditBase1.Selector, "selectRow", newRowIndex);
        Megi.grid(SPEditBase1.Selector, "beginEdit", newRowIndex);
        var newRowData = Megi.grid(SPEditBase1.Selector, "getSelected");

        var newEditor = $(SPEditBase1.Selector).datagrid('getEditor', { index: newRowIndex, field: "MItemID" });
        if (newEditor != null) {
            $(newEditor.target).combobox("showPanel");
            //设置前面的+为tab的焦点，这样combox才能获取焦点
            $(newEditor.target).closest("td[field='MItemID']").prev().find('.m-icon-add-row').focus();
        }
        SPEditBase1.bindGridEditorTabEvent(newRowData, newRowIndex);
        SPEditBase1.bindGridEditorEvent(newRowData, newRowIndex);
    },
    //给明细列表控件绑定 KeyUp 事件
    bindGridEditorEvent: function (rowData, rowIndex) {
        var taxType = SPEditBase1.getTaxType();
        var ed = $(SPEditBase1.Selector).datagrid('getEditors', rowIndex);
        for (var i = 0; i < ed.length; i++) {
            var e = ed[i];
            switch (e.field) {
                case "MQty":
                    $(e.target).bind("keyup", function (event) {
                        var value = SPEditBase1.getGridEditorValue(SPEditBase1.RowIndex, "MQty");
                        SPEditBase1.updateDataSourceEntry("MQty='" + value + "'");
                        rowData.MAmountFor = SPEditBase1.DataSourceEntry[SPEditBase1.RowIndex].MAmountFor;
                    });
                    break;
                case "MPrice":
                    $(e.target).bind("keyup", function (event) {
                        var value = SPEditBase1.getGridEditorValue(SPEditBase1.RowIndex, "MPrice");
                        SPEditBase1.updateDataSourceEntry("MPrice='" + value + "'");
                        rowData.MAmountFor = SPEditBase1.DataSourceEntry[SPEditBase1.RowIndex].MAmountFor;
                    });
                    break;
                case "MAmountFor":
                    $(e.target).bind("keyup", function (event) {
                        var value = SPEditBase1.getGridEditorValue(SPEditBase1.RowIndex, "MAmountFor");
                        SPEditBase1.updateDataSourceEntry("MAmountFor='" + value + "'");
                    });
                    break;
            }
        }
    },
    //获取明细列表编辑状态下控件的值
    getGridEditorValue: function (rowIndex, field) {
        var rowObj = $(SPEditBase1.Selector).parent().find("tr[datagrid-row-index=" + rowIndex + "]");
        var value = $(rowObj).find("td[field='" + field + "']").find("input").val();
        return value;
    },
    //获取一个空的明细对象
    getEmptyEntry: function () {
        var obj = {};
        obj.MEntryID = "";
        obj.SalaryPayID = SPEditBase1.SalaryPayID;
        obj.MItemID = "";
        obj.MIsNew = true;
        obj.MDesc = "";
        obj.MAmountFor = "";
        obj.MAmount = "";
        return obj;
    },
    //明细列表的列数组
    Columns: {
        getArray: function () {
            var arr = new Array();
            //新增行
            if (!SPEditBase1.Disabled) {
                arr.push({
                    title: '', field: 'MAddID', width: 20, height: SPEditBase1.EditorHeight, align: 'center', formatter: function (value, rowData, rowIndex) {
                        return "<a href='javascript:void(0)' class='m-icon-add-row' onclick='SPEditBase1.AddItem(this)'>&nbsp;</a>";
                    }
                });
            }
            //工资项目
            var itemColumn = SPEditBase1.Columns.getItemColumn();
            arr.push(itemColumn);
            arr.push({ title: HtmlLang.Write(LangModule.IV, "Description", "Description"), field: 'MDesc', width: 120, align: 'center', sortable: false, editor: { type: 'text' } });
            arr.push({
                title: HtmlLang.Write(LangModule.IV, "Amount", "Amount"), field: 'MAmountFor', width: 80, align: 'center', sortable: false, editor: { type: 'numberbox' }, formatter: function (value, rowData, rowIndex) {
                    return (value == null || value == "") ? "" : Megi.Math.toMoneyFormat(Math.abs(value));
                }
            });
            //删除行
            if (!SPEditBase1.Disabled) {
                arr.push({
                    title: '', field: 'MEntryID', width: 20, height: SPEditBase1.EditorHeight, align: 'center', sortable: false, formatter: function (value, rowData, rowIndex) {
                        return "<div class='list-item-action'><a href='javascript:void(0)' class='m-icon-delete-row' onclick='SPEditBase1.DeleteItem(this)'>&nbsp;</a></div>";
                    }
                });
            }
            var columns = new Array();
            columns.push(arr);
            return columns;
        },
        updateEditorValue: function (cbox, field, value) {
            var rowObj = $(cbox).closest("td[field='MItemID']").parent();
            $(rowObj).find("td[field='" + field + "']").find("input").val(value);
        },
        updateComboEditorValue: function (cbox, field, text, value) {
            var rowObj = $(cbox).closest("td[field='MItemID']").parent();
            $(rowObj).find("td[field='" + field + "']").find(".combo-text").val(text);
            $(rowObj).find("td[field='" + field + "']").find(".combo-value").val(value);
        },
        //获取工资项目
        getItemColumn: function () {
            var obj = {};
            obj.title = HtmlLang.Write(LangModule.PA, "PayItem", "Pay Item");
            obj.field = 'MItemID';
            obj.width = 100;
            obj.align = 'center';
            obj.sortable = false;
            obj.editor = {
                type: 'addCombobox',
                options: {
                    type: "pa",
                    addOptions: {
                        //弹出框关闭后的回调函数
                        callback: SPEditBase1.initPayItem
                    },
                    dataOptions: {
                        height: SPEditBase1.EditorHeight,
                        //数据加载成功后更新数据源
                        onLoadSuccess: function (msg) {
                            SPEditBase1.DataSourcePayItem = msg;
                        },
                        onSelect: function (rec) {

                        }
                    }
                }
            };
            obj.formatter = function (value, rowData, rowIndex) {
                return SPEditBase1.getPayItemCode(value);
            }
            return obj;
        }
    },
    //删除一条明细（根据索引）
    DeleteItem: function (btnObj) {
        var rowIndex = $(btnObj).closest(".datagrid-row").attr("datagrid-row-index");
        var result = SPEditBase1.deleteDataSourceEntry(rowIndex);
        if (!result) {
            return;
        }
        Megi.grid(SPEditBase1.Selector, "deleteRow", rowIndex);
        SPEditBase1.endEditGrid();
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
            var tr_id = $(this).attr("id");
            $(this).attr("id", tr_id.substr(0, tr_id.lastIndexOf('-') - 1) + i);
        });
        SPEditBase1.updateDataSourceEntry();
    },
    //添加一条明细
    AddItem: function (btnObj) {
        var rowIndex = Number($(btnObj).closest(".datagrid-row").attr("datagrid-row-index"));
        SPEditBase1.AddItemByIndex(rowIndex);
    },
    //插入一条明细（根据索引）
    AddItemByIndex: function (rowIndex) {
        SPEditBase1.endEditGrid();
        var newRow = SPEditBase1.insertDataSourceEntry(rowIndex);
        if (newRow != null) {
            Megi.grid(SPEditBase1.Selector, "insertRow", { index: rowIndex, row: newRow });
            SPEditBase1.endEditGrid();
        }
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
        });
        Megi.grid(SPEditBase1.Selector, "beginEdit", rowIndex);
        Megi.grid(SPEditBase1.Selector, "selectRow", rowIndex);
        SPEditBase1.RowIndex = rowIndex;

        var rowData = Megi.grid(SPEditBase1.Selector, "getSelected");
        SPEditBase1.bindGridEditorTabEvent(rowData, rowIndex);
        SPEditBase1.bindGridEditorEvent(rowData, rowIndex);
    },
    //获取本位币ID
    getOrgCurrencyID: function () {
        var obj = eval("(" + "{" + $("#selCurrency").attr("data-options") + "}" + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return "";
        }
        return obj.data[0].SrcCurrencyID;
    },
    //获取当前币别信息
    getCurrency: function () {
        var obj = eval("(" + "{" + $("#selCurrency").attr("data-options") + "}" + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return "";
        }
        var selectedValue = $("#selCurrency").combobox("getValue");
        for (var i = 0; i < obj.data.length; i++) {
            if (obj.data[i].CurrencyID == selectedValue) {
                return obj.data[i];
            }
        }
        return null;
    },
    //设置默认币别
    setDefaultCurrency: function (defaultCyId) {
        var obj = $("#selCurrency").attr("data-options");
        obj = "{" + obj + "}";
        obj = eval("(" + obj + ")");
        if (obj == null || obj.data == null || obj.data.length == 0) {
            return;
        }
        for (var i = 0; i < obj.data.length; i++) {
            if (obj.data[i].CurrencyID == defaultCyId) {
                $("#selCurrency").combobox("select", defaultCyId);
                return;
            }
        }
        $("#selCurrency").combobox("select", obj.data[0].CurrencyID);
    },
    //结束编辑列表
    endEditGrid: function () {
        var recordLength = Megi.grid(SPEditBase1.Selector, "getRows").length;
        for (var i = 0; i < recordLength; i++) {
            Megi.grid(SPEditBase1.Selector, "endEdit", i);
        }
    }
}