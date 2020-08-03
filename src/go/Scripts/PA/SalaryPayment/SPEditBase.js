var SPEditBase = {
    EditorHeight: "26px",
    SalaryPayID: "",
    Selector: "#tbPaySalaryDetail",
    Disabled: false,
    DataSourceEntry: [],
    PITThresholdAmount: 0,
    PITTaxRateList: [],
    SalaryTaxAmt: 0,
    SalaryNetAmt: 0,
    RowIndex: -1,//当前编辑行索引
    ManualTaxAmt: 0,//手动修改的个税
    Verification: null,
    IsCanBankAccountViewPermission: false,
    IsCanBankAccountChangePermission: false,
    Model: null,
    IsAmountChanged: false,
    //是否计算个税
    IsCalculatePIT: false,
    init: function (salaryPayID, taxAmt) {
        SPEditBase.SalaryPayID = salaryPayID;
        SPEditBase.IsCanBankAccountViewPermission = $("#hidIsCanBankAccountViewPermission").val() == "1" ? true : false;
        SPEditBase.IsCanBankAccountChangePermission = $("#hidIsCanBankAccountChangePermission").val() == "1" ? true : false;

        SPEditBase.Model = eval('(' + $("#hidModel").val() + ')');
        SPEditBase.PITThresholdAmount = SPEditBase.Model.MPITThresholdAmount;
        SPEditBase.PITTaxRateList = SPEditBase.Model.MPITTaxRateList;
        SPEditBase.Verification = SPEditBase.Model.Verification;
        SPEditBase.IsCalculatePIT = SPEditBase.Model.IsCalculatePIT;
        SPEditBase.ManualTaxAmt = taxAmt;

        if ($("body", parent.document).find(".mBoxTitle").length == 0) {
            var title = HtmlLang.Write(LangModule.PA, "PeriodPayslipFor", "{0} Payslip for {1}").replace("{0}", SPEditBase.Model.MDateFormat).replace("{1}", SPEditBase.Model.MEmployeeName);
            $("#pageTitle").text(title).show();
            $("#dialogTitle").css({ "padding-top": 5 });
        }
        else {
            $.mDialog.max();
            if (Megi.isIE()) {
                FWInner.initFW();
            }
        }
        //是否可编辑
        SPEditBase.Disabled = $("#hidIsEdit").val() === "False";
        //绑定明细列表
        SPEditBase.bindTreeGrid(salaryPayID);
        SPEditBase.initAction();
    },
    //初始化事件
    initAction: function () {
        if ($("#hidIsEdit").val() === "True") {
            $("#spTax").numberbox({
                onChange: function (newVal, oldVal) {
                    SPEditBase.ManualTaxAmt = newVal;
                    SPEditBase.updateSummaryInfo();
                }
            });
            $("#spTax").keyup(function (e) {
                //Enter事件
                if (e.which == 13) {
                    $("#aSave").focus();
                }
            }).keydown(function (e) {
                //Tab事件
                if (e.which == 9) {
                    SPEditBase.PreventDefaultEvent(e);
                    $("#aSave").focus();
                }
            });
        }
    },
    //验证信息
    valideInfo: function () {
        var result = true;
        for (var i = 0; i < SPEditBase.DataSourceEntry.length; i++) {
            var item = SPEditBase.DataSourceEntry[i];
            if (item.MDesc == "" && item.MAmount == "") {
                continue;
            }
            if (item.MAmount == "") {
                $(SPEditBase.Selector).parent().find("tr[datagrid-row-index=" + i + "]>td[field='MAmount']").addClass("row-error");
                result = false;
            }
        }
        return result;
    },
    //获取明细实体列表
    getViewInfo: function () {
        SPEditBase.endEditGrid();
        SPEditBase.updateDataSourceEntry();
        var obj = {};
        var arr = new Array();
        for (var i = 0; i < SPEditBase.DataSourceEntry.length; i++) {
            var item = SPEditBase.DataSourceEntry[i];
            if (item.MItemID == "" && item.MDesc == "" && item.MAmount == "") {
                continue;
            }
            arr.push(item);
        }
        obj.SalaryPaymentEntry = arr;
        obj.MTaxSalary = SPEditBase.SalaryTaxAmt;
        obj.MNetSalary = SPEditBase.SalaryNetAmt;
        return obj;
    },
    //更新数据源
    updateDataSourceEntry: function (id, keyValue) {
        if (id != undefined) {
            for (var i = 0; i < SPEditBase.DataSourceEntry.length; i++) {
                if (SPEditBase.DataSourceEntry[i].RowIndex == id) {
                    if (keyValue != undefined) {
                        eval("SPEditBase.DataSourceEntry[i]." + keyValue);
                        //更新父级数据
                        SPEditBase.SetParentNode(SPEditBase.DataSourceEntry[i].RowIndex);
                    }
                }
            }
        }
        SPEditBase.updateSummaryInfo();
    },
    //更新合计信息
    updateSummaryInfo: function () {
        var amt = 0;
        var arrTax = new Array();
        for (var i = 0; i < SPEditBase.DataSourceEntry.length; i++) {
            var item = SPEditBase.DataSourceEntry[i];
            if (item.MAmount != "") {
                if (SPEditBase.isExitsChildNode(item.RowIndex)) { continue; }
                amt += parseFloat(SPEditBase.EmptyToIntZero(item.MAmount * item.MCoefficient));
            }
        }
        var taxAmt = 0;
        if (SPEditBase.Disabled) {
            taxAmt = SPEditBase.SalaryTaxAmt;
        }
        else {
            //修改工资项金额
            if (SPEditBase.IsAmountChanged || !SPEditBase.ManualTaxAmt) {
                taxAmt = SPEditBase.CalSalaryTaxAmt(amt);
            }
            else {//修改个税金额
                taxAmt = SPEditBase.ManualTaxAmt;
            }
        }
        var netAmt = (amt - taxAmt) == 0 ? "0.00" : amt - taxAmt;
        if (amt == 0) { amt = "0.00"; }
        //税前总金额
        $("#spTotal").html(Megi.Math.toMoneyFormat(amt, 2));
        //计算个人所得税
        if (SPEditBase.Disabled) {
            $("#spTax").html(Megi.Math.toMoneyFormat(taxAmt, 2));
        } else {
            $("#spTax").numberbox('setValue', Megi.Math.toDecimal(taxAmt, 2));
            SPEditBase.SalaryTaxAmt = taxAmt;
            SPEditBase.SalaryNetAmt = netAmt;
        }
        //净发金额
        $("#spNet").html(Megi.Math.toMoneyFormat(netAmt, 2));
        //核销信息
        SPEditBase.updateVerification(netAmt);
    },
    //重新设置数据源
    setDataSourceEntry: function (recordData) {
        var entryDataSource = new Array();
        for (var i = 0; i < recordData.length; i++) {
            if (i == 0) {
                var qzdAmt = SPEditBase.EmptyToIntZero(recordData[0].MPITThresholdAmount);
                if (qzdAmt > 0) { SPEditBase.PITThresholdAmount = qzdAmt };
                if (SPEditBase.Disabled) { SPEditBase.SalaryTaxAmt = recordData[0].MSalTaxAmount }
            }
            //树状2级
            var childAllAmt = 0;
            var childNode = $(SPEditBase.Selector).treegrid("getChildren", recordData[i].id);
            if (childNode.length > 0) {
                for (var j = 0; j < childNode.length; j++) {
                    var obj2 = SPEditBase.getOneEntry(childNode[j]);
                    entryDataSource.push(obj2);
                    childAllAmt += parseFloat(SPEditBase.EmptyToIntZero(childNode[j].MAmount));
                }
                //修改树状1级金额
                recordData[i].MAmount = childAllAmt;
            }
            //树状1级
            var obj = SPEditBase.getOneEntry(recordData[i]);
            entryDataSource.push(obj);
        }
        SPEditBase.DataSourceEntry = entryDataSource;
    },
    //删除数据源中指定的记录（根据索引删除）
    deleteDataSourceEntry: function (rowIndex) {
        if (SPEditBase.DataSourceEntry.length <= 1) {
            //$.mDialog.alert("You must have at least 1 line item.", null, LangModule.IV, "AtLeastOneLineItem");
            return false;
        }
        var arr = new Array();
        for (var i = 0; i < SPEditBase.DataSourceEntry.length; i++) {
            if (i != rowIndex) {
                var obj = SPEditBase.DataSourceEntry[i];
                obj.RowIndex = i;
                arr.push(obj);
            }
        }
        SPEditBase.DataSourceEntry = arr;
        return true;
    },
    //插入一条记录到数据源中（根据索引插入）
    insertDataSourceEntry: function (rowIndex) {
        var newRow = null;
        var arr = new Array();
        var index = 0;
        for (var i = 0; i < SPEditBase.DataSourceEntry.length; i++) {
            if (SPEditBase.DataSourceEntry[i].RowIndex == rowIndex) {
                newRow = SPEditBase.getEmptyEntry();
                newRow.RowIndex = index;
                arr.push(newRow);
                index += 1;
            }
            var obj = SPEditBase.DataSourceEntry[i];
            obj.RowIndex = index;
            arr.push(obj);
            index += 1;
        }
        SPEditBase.DataSourceEntry = arr;
        return newRow;
    },
    //绑定明细列表
    bindTreeGrid: function (salaryPayID) {
        var columns = SPEditBase.Columns.getArray();
        SPEditBase.adjustListWidth();
        $(SPEditBase.Selector).megitreegrid({
            columns: columns,
            resizable: true,
            fitColumns: true,
            auto: true,
            width: $("#divSalaryDetail").width(),
            url: "/PA/SalaryPayment/GetSalaryPaymentPersonDetails?salaryPayId=" + salaryPayID,
            valueField: "MPayItemID",
            textField: "MPayItemName",
            parentField: "MParentPayItemID",
            idField: 'id',
            treeField: "MPayItemName",
            // 单击行时，设置行可编辑状态
            onClickRow: function (rowData) {
                if (SPEditBase.Disabled) { return; }
                if (SPEditBase.isExitsChildNode(rowData.id)) { return; }
                SPEditBase.bindGridEditorEvent(rowData);
                SPEditBase.bindGridEditorTabEvent(rowData);
                Megi.regClickToSelectAllEvt();
            },
            onClickCell: function (field, rowData) {
                if (SPEditBase.Disabled) { return; }
                SPEditBase.endEditGrid();
                if (SPEditBase.isExitsChildNode(rowData.id)) { return; }
                $(SPEditBase.Selector).treegrid("beginEdit", rowData.id);
                var editor = $(SPEditBase.Selector).treegrid("getEditor", { id: rowData.id, field: field });
                SPEditBase.SetEditorFocus(field);
                Megi.regClickToSelectAllEvt();
            },
            onAfterEdit: function () {
                SPEditBase.SetNewCss();
            },
            onLoadSuccess: function (row, data) {
                //设置所有1级的数据
                SPEditBase.SetAllParentNode();
                //设置表格数据
                SPEditBase.setDataSourceEntry(data);
                //更新表格数据
                SPEditBase.updateDataSourceEntry();
                //加上是增加项还是扣除项的icon
                SPEditBase.SetNewCss();
            }
        });
    },
    //自适应宽度
    adjustListWidth: function () {
        if ($(".m-iv").length > 0) {
            var totalWidth = $(".m-imain-content").width();
            var w = totalWidth - 285;
            $("#divSalaryDetail").width(w);
        }
    },
    //给明细列表控件绑定 KeyUp 事件
    bindGridEditorEvent: function (rowData) {
        var ed = $(SPEditBase.Selector).treegrid('getEditors', rowData.id);
        for (var i = 0; i < ed.length; i++) {
            var e = ed[i];
            switch (e.field) {
                case "MDesc":
                    $(e.target).bind("keyup", function (event) {
                        var value = SPEditBase.getGridEditorValue(rowData.id, "MDesc");
                        SPEditBase.updateDataSourceEntry(rowData.id, "MDesc='" + value + "'");
                    });
                    break;
                case "MAmount":
                    $(e.target).bind("keyup", function (event) {
                        //Enter按下时，不触发金额计算
                        if (event.which == 13) {
                            return;
                        }
                        SPEditBase.onAmountChanging(rowData);
                    });
                    break;
            }
        }
    },
    //金额改变时触发
    onAmountChanging: function (rowData) {
        SPEditBase.IsAmountChanged = true;
        var value = Megi.Math.toDecimal(SPEditBase.getGridEditorValue(rowData.id, "MAmount"), 2);
        if (!value) {
            value = 0;
        }
        if (value < 0) { value = 0; }
        if (value > 100000000) { value = 100000000; }
        SPEditBase.updateDataSourceEntry(rowData.id, "MAmount='" + Megi.Math.toDecimal(value, 2) + "'");
        SPEditBase.IsAmountChanged = false;
    },
    bindGridEditorTabEvent: function (rowData) {
        $(".datagrid-row-editing").find(".datagrid-editable-input").each(function () {
            $(this).keyup(function (event) {
                //Enter事件
                if (event.which == 13) {
                    var field = $(this).closest("table").closest("td").attr("field");
                    //当前编辑行的所有控件
                    var editing_controls = $(".datagrid-row-editing").find(".datagrid-editable-input");
                    //当前正在编辑的控件的索引
                    var editing_controls_index = editing_controls.index(this);
                    //如果当前正在编辑的控件是该编辑行的最后一个控件
                    if (editing_controls_index == editing_controls.length - 1) {
                        //则结束当前编辑行，开始进行下一行编辑

                        var nextId = SPEditBase.GetNextEditRowID(rowData.id);
                        if (nextId == rowData.id) {
                            SPEditBase.endEditGrid();
                            $("#spTax").focus().select();
                        }
                        else if (nextId != '') {
                            SPEditBase.beginNextRowEdit(nextId);
                            SPEditBase.SetEditorFocus();
                        }
                    } else {
                        //将焦点移动到下一个控件
                        var nextCtrl = editing_controls[editing_controls_index + 1];
                        nextCtrl.focus();
                        if (field == "MDesc") {
                            nextCtrl.select();
                        }
                    }
                }
            }).keydown(function (e) {
                var field = $(this).closest("table").closest("td").attr("field");
                //Tab事件
                if (field == "MAmount" && e.which == 9) {
                    SPEditBase.PreventDefaultEvent(e);
                    var nextId = SPEditBase.GetNextEditRowID(rowData.id);
                    if (nextId == rowData.id) {
                        SPEditBase.endEditGrid();
                        $("#spTax").focus().select();
                    }
                    else if (nextId != '') {
                        SPEditBase.beginNextRowEdit(nextId);
                        SPEditBase.SetEditorFocus();
                    }
                }
            });
        });
    },
    //设置编辑行焦点（默认聚焦第一个控件）
    SetEditorFocus: function (field) {
        var editing_controls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
        if (editing_controls.length > 0) {
            var isAmountField = field == "MAmount";
            var ctrl = editing_controls[isAmountField ? 1 : 0];
            ctrl.focus();
            if (isAmountField) {
                ctrl.select();
            }
        }
    },
    //阻止默认的事件
    PreventDefaultEvent: function (e) {
        if (e && e.preventDefault) {
            e.preventDefault();
        }
        else {
            window.event.returnValue = false;
        }
    },

    GetNextEditRowID: function (currentId) {
        var currentIndex = -1;
        $(".datagrid-btable").find("tr").each(function (i) {
            var nodeId = $(this).attr("node-id");
            var hasChildNode = SPEditBase.isExitsChildNode(nodeId);
            if (hasChildNode) {
                $(SPEditBase.Selector).treegrid("expand", nodeId);
            }
            if (currentIndex > -1 && !hasChildNode) {
                id = nodeId;
                return false;
            }
            if (nodeId == currentId) {
                currentIndex = i;
            }
        });
        return id;
    },


    //开始下一行编辑
    beginNextRowEdit: function (id) {
        SPEditBase.endEditGrid();
        $(SPEditBase.Selector).treegrid("selectRow", id);
        $(SPEditBase.Selector).treegrid("beginEdit", id);
        var newRowData = $(SPEditBase.Selector).treegrid("getSelected");

        //var newEditor = $(SPEditBase.Selector).datagrid('getEditor', { index: newRowIndex, field: "MItemID" });

        SPEditBase.bindGridEditorTabEvent(newRowData);
        SPEditBase.bindGridEditorEvent(newRowData);
        Megi.regClickToSelectAllEvt();
    },

    //获取明细列表编辑状态下控件的值
    getGridEditorValue: function (rowIndex, field) {
        var rowObj = $(SPEditBase.Selector).parent().find("tr[node-id=" + rowIndex + "]");
        var value = $(rowObj).find("td[field='" + field + "']").find("input").val();
        return value;//value == undefined ? '' : 
    },
    //获取一个空的明细对象
    getEmptyEntry: function () {
        var obj = {};
        obj.MEntryID = "";
        obj.MID = SPEditBase.SalaryPayID;
        obj.MPayItemID = "";
        obj.MParentPayItemID = "";
        obj.MIsNew = true;
        obj.MDesc = "";
        obj.MAmount = "";
        obj.MCoefficient = 1;
        return obj;
    },
    getOneEntry: function (data) {
        var obj = SPEditBase.getEmptyEntry();
        obj.RowIndex = data.id;
        obj.MPayItemID = data.id;
        obj.MParentPayItemID = data.MParentPayItemID;
        obj.MDesc = data.MDesc;
        obj.MAmount = data.MAmount;
        obj.MCoefficient = data.MCoefficient;
        return obj;
    },
    SetNewCss: function () {
        //第一列的表格数据
        var node = $("tr[class^='datagrid-row'] td[field='MPayItemName']");
        node.each(function (index) {
            //找到隐藏的addType标识
            var preNode = $(this).prev().find("div:first");
            preNode.each(function () {
                //增加项或者扣除项的第一行
                if (index == 0 || node.eq(index - 1).prev().find("div:first").html() != $(this).html()) {
                    if (node.eq(index).find("span:first").html() == "+" || node.eq(index).find("span:first").html() == "- ") {
                        node.eq(index).find("span:first").remove();
                    }
                    node.eq(index).find("span").css({ "margin-top": "5px" });
                    //增加项
                    if ($(this).html() == 1) {
                        node.eq(index).find("span:first").before("<span style='width: 10px; height: 17px; display: inline-block;font-size:21px;'>+</span>");
                    } else {
                        node.eq(index).find("span:first").before("<span style='width: 10px; height: 17px; display: inline-block;font-size:21px;'>- </span>");
                        //设置第一个负工资项的上边框
                        node.eq(index).closest("tr").find("td").css({ "border-top": "1px solid #e9e9e9" });
                    }
                }
                    //其他行右移
                else {
                    node.eq(index).find("span:first").css("margin-left", "10px");
                }
            });
        });
    },
    SetAllParentNode: function () {
        var recordData = $(SPEditBase.Selector).treegrid("getData");
        for (var i = 0; i < recordData.length; i++) {
            SPEditBase.SetNodeFromChild(recordData[i].id);
        }
    },
    SetParentNode: function (childId) {
        var parentNode = $(SPEditBase.Selector).treegrid("getParent", childId);
        if (parentNode == null) { return; }
        var amount = SPEditBase.getGridEditorValue(childId, "MAmount");
        //不统计负数金额
        if (amount < 0) {
            amount = 0;
        }
        var childAllAmt = parseFloat(SPEditBase.EmptyToIntZero(amount));
        SPEditBase.SetNodeFromChild(parentNode.id, childId, childAllAmt);
    },
    SetNodeFromChild: function (nodeId, childId, initAmount) {
        var childAllAmt = initAmount == undefined ? 0 : initAmount;
        var childNode = $(SPEditBase.Selector).treegrid("getChildren", nodeId);
        if (childNode.length > 0) {
            for (var j = 0; j < childNode.length; j++) {
                if (childId != undefined && childNode[j].id == childId) { continue; }
                childAllAmt += parseFloat(SPEditBase.EmptyToIntZero(childNode[j].MAmount));
            }
            if (childAllAmt == 0) {
                childAllAmt = "0.00";
            }
            //更新树状1级金额
            $(SPEditBase.Selector).treegrid("update", { id: nodeId, row: { MAmount: Megi.Math.toDecimal(childAllAmt, 2) } });
            //更新树状1级数据源
            SPEditBase.updateDataSourceEntry(nodeId, "MAmount='" + Megi.Math.toDecimal(childAllAmt, 2) + "'");
        }
    },
    EmptyToIntZero: function (value) {
        if (value == "") { value = 0; }
        return value;
    },
    //明细列表的列数组
    Columns: {
        getArray: function () {
            var arr = new Array();
            arr.push({ title: '', field: 'MCoefficient', width: 0, hidden: true });
            arr.push({ title: HtmlLang.Write(LangModule.PA, "Category", "Category"), field: 'MPayItemName', width: 100, align: 'left', sortable: false });
            arr.push({
                title: HtmlLang.Write(LangModule.PA, "Description", "Description"), field: 'MDesc', width: 120, align: 'center', sortable: false, editor: { type: 'text' }, formatter: function (value, rowData, rowIndex) {
                    return (value == null || value == "") ? "" : value;
                }
            });
            arr.push({
                title: HtmlLang.Write(LangModule.PA, "Amount", "Amount"), field: 'MAmount', width: 60, align: 'center', sortable: false, editor: { type: 'numberbox', options: { precision: 2, minPrecision: 2, min: 0, max: 100000000 } }, formatter: function (value, rowData, rowIndex) {
                    return (value == null || value == 0) ? "" : Megi.Math.toMoneyFormat(Math.abs(value), 2);
                }
            });
            var columns = new Array();
            columns.push(arr);
            return columns;
        }
    },
    //删除一条明细（根据索引）
    DeleteItem: function (btnObj) {
        var rowIndex = $(btnObj).closest(".datagrid-row").attr("datagrid-row-index");
        var result = SPEditBase.deleteDataSourceEntry(rowIndex);
        if (!result) {
            return;
        }
        Megi.grid(SPEditBase.Selector, "deleteRow", rowIndex);
        SPEditBase.endEditGrid();
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
            var tr_id = $(this).attr("id");
            $(this).attr("id", tr_id.substr(0, tr_id.lastIndexOf('-') - 1) + i);
        });
        SPEditBase.updateDataSourceEntry();
    },
    //添加一条明细
    AddItem: function (btnObj) {
        var rowIndex = Number($(btnObj).closest(".datagrid-row").attr("datagrid-row-index"));
        SPEditBase.AddItemByIndex(rowIndex);
    },
    //插入一条明细（根据索引）
    AddItemByIndex: function (rowIndex) {
        SPEditBase.endEditGrid();
        var newRow = SPEditBase.insertDataSourceEntry(rowIndex);
        if (newRow != null) {
            Megi.grid(SPEditBase.Selector, "insertRow", { index: rowIndex, row: newRow });
            SPEditBase.endEditGrid();
        }
        $(".datagrid-btable").find(".datagrid-row").each(function (i) {
            $(this).attr("datagrid-row-index", i);
        });
        Megi.grid(SPEditBase.Selector, "beginEdit", rowIndex);
        Megi.grid(SPEditBase.Selector, "selectRow", rowIndex);
        SPEditBase.RowIndex = rowIndex;

        var rowData = Megi.grid(SPEditBase.Selector, "getSelected");
        SPEditBase.bindGridEditorEvent(rowData, rowIndex);
    },
    //结束编辑列表
    endEditGrid: function () {
        var recordData = $(SPEditBase.Selector).treegrid("getData");
        for (var i = 0; i < recordData.length; i++) {
            $(SPEditBase.Selector).treegrid("endEdit", recordData[i].id);
            var childNode = $(SPEditBase.Selector).treegrid("getChildren", recordData[i].id);
            for (var j = 0; j < childNode.length; j++) {
                $(SPEditBase.Selector).treegrid("endEdit", childNode[j].id);
            }
        }
    },
    isExitsChildNode: function (parId) {
        var childNode = $(SPEditBase.Selector).treegrid("getChildren", parId);
        return childNode.length > 0;
    },
    CalSalaryTaxAmt: function (preTaxAmt) {
        var result = "0.00";
        //如果不计算个税，则返回0
        if (!SPEditBase.IsCalculatePIT) {
            return result;
        }

        var restAmt = preTaxAmt - SPEditBase.PITThresholdAmount;

        //遍历当前月份的个税税率列表，找到纳税区间
        $.each(SPEditBase.PITTaxRateList, function (i, item) {
            if (restAmt > item.MBeginAmount && restAmt <= item.MEndAmount) {
                result = restAmt * item.MTaxRate / 100 - item.MDeductionAmount;
                return false;
            }
        });

        return result;
    },
    updateVerification: function (amtFor) {
        if (SPEditBase.Verification == null || SPEditBase.Verification.length == 0) {
            $("#divCredit").html("");
            return;
        }
        var html = "";
        var totalVerificationAmt = 0;
        for (var i = 0; i < SPEditBase.Verification.length; i++) {
            var lastStyle = "style='border-top:1px dotted #777;'";
            if (i == 0) {
                lastStyle = "style='border-top: 1px solid #000;'";
            }
            var item = SPEditBase.Verification[i];
            var verifiText = HtmlLang.Write(LangModule.Common, item.MBizBillType, item.MBizBillType);
            var less = "<label style='color:#888;'>" + HtmlLang.Write(LangModule.IV, "Less", "Less") + "</label> ";
            verifiText = less + verifiText;// + " " + item.MBillNo;

            totalVerificationAmt += Math.abs(item.MHaveVerificationAmtFor);
            //是否有银行查看权限
            if (SPEditBase.IsCanBankAccountViewPermission) {
                //是否有银行编辑权限
                if (SPEditBase.IsCanBankAccountChangePermission) {
                    html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" onclick="SPEditBase.ViewVerificationDetail(\'' + item.MBillID + '\',\'' + item.MBizType + '\')"><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><span class="mg-total-action"><a href="javascript:void(0)" onclick="SPEditBase.deleteVerification(\'' + item.VerificationID + '\',\'' + item.IsMergePay + '\')" class="m-icon-delete" style="padding-left:10px;">&nbsp;</a></span><div class="clear"></div></div>';
                } else {
                    html += '<div class="credit" ' + lastStyle + '><a href="javascript:void(0)" onclick="SPEditBase.ViewVerificationDetail(\'' + item.MBillID + '\',\'' + item.MBizType + '\')"><span class="mg-total-text">' + verifiText + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span></a><span class="mg-total-action"></span><div class="clear"></div></div>';
                }
            } else {
                if (SPEditBase.IsCanBankAccountChangePermission) {
                    html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + HtmlLang.Write(LangModule.Common, item.MBizType, item.MBizType) + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span><a href="javascript:void(0)" onclick="SPEditBase.deleteVerification(\'' + item.VerificationID + '\',\'' + item.IsMergePay + '\')" class="m-icon-delete">&nbsp;</a><span class="mg-total-action"></span><div class="clear"></div></div>';
                } else {
                    html += '<div class="credit" ' + lastStyle + '><span class="mg-total-text">' + HtmlLang.Write(LangModule.Common, item.MBizType, item.MBizType) + '<label class="bizdate">' + $.mDate.format(item.MBizDate) + '</label></span><span class="mg-total-value">' + Megi.Math.toMoneyFormat(Math.abs(item.MHaveVerificationAmtFor)) + '</span><span class="mg-total-action"></span><div class="clear"></div></div>';
                }
            }
        }
        var amountDue = amtFor - totalVerificationAmt;
        html += ' <div class="due-total1" style="border-top:1px dotted #777;font-weight: bold;height: 40px;line-height: 40px;border-bottom: 3px double #000;color: #000;"><span class="mg-total-text">' + HtmlLang.Write(LangModule.IV, "AmountDue", "Amount Due") + '</span><span class="mg-total-value" id="spTotal">' + Megi.Math.toMoneyFormat(amountDue) + '</span><span class="mg-total-currency"></span><span class="mg-total-action"></span></div>';

        $("#divCredit").html(html);
    },
    //查看核销的详细信息
    //ViewVerificationDetail: function (billId, bizType, ref) {
    //    BillUrl.open({ BillID: billId, BillType: bizType, Ref: ref });
    //},
    ViewVerificationDetail: function (billId, bizType) {
        BillUrl.open({ BillID: billId, BillType: bizType });
    },
    deleteVerification: function (verificationId, isMergePay) {
        var content = HtmlLang.Write(LangModule.IV, "DeleteReconcileRelationshipConfirm", "Are you sure to delete the reconcile relationship.");
        $.mDialog.confirm(content, null, function () {
            mAjax.submit(
                "/IV/Verification/DeleteVerification/",
                { verificationId: verificationId,isMergePay:isMergePay},
                function (msg) {
                    $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteReconcileRelationshipSuccessfully", "Delete reconcile relationship successfully!"));
                    SalaryPaymentEdit.reload();
                }
            );
        });
    }
}