var InspectSetting = {
    //设置期间的巡检项ID
    setPeriodItemIdList: new Array("4120", "4220", "4320", "4420", "4520", "4620"),
    defalutPeriodList: new Array({ id: "3", value: "3" }, { id: "6", value: "6" }, { id: "9", value: "9" }, { id: "12", value: "12" }),
    init: function () {

        InspectSetting.initAction();

        InspectSetting.loadInspectTreeList();
    },
    initAction: function () {
        $("#btnSave").on("click", function () {
            InspectSetting.requestSave();
        });
    },
    loadInspectTreeList: function () {
        var treeGrid = $("#tbInspect");
        treeGrid.treegrid({
            url: "/BD/RoutineInspect/GetInspectItemTreeList",
            //queryParams: "",
            idField: "id",
            treeField: 'text',
            checkbox: true,
            fitColumns: false,
            singleSelect: false,
            scrollY: true,
            height: $(".m-imain").height() - 20,
            lines: true,
            region: "center",
            columns: [[{
                field: 'id', width: 40, checkbox: false, hidden: true
            },
             {
                 title: HtmlLang.Write(LangModule.BD, "CheckPointItem", "检查项"), field: 'text', width: 500, align: 'left', sortable: false, formatter: function (value, rec, rowIndex) {
                     return mText.encode(value);
                 }
             },
             {
                 field: 'MSettingID', width: 40, checkbox: false, hidden: true
             },
             {
                 field: 'MSettingParamID', width: 40, hidden: true
             },
             {
                 title: HtmlLang.Write(LangModule.BD, "CheckPoint", "检查"), field: 'MEnable', width: 100, align: 'center', sortable: false, formatter: function (value, rec, rowIndex) {

                     var checkedHtml = "<input type=\"checkbox\" class=\"row-key-checkbox\" checked value=\"" + rec.id + "\" >";
                     var unCheckedHtml = "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.id + "\" >";
                     var disableHtml = "<input type=\"checkbox\" class=\"row-key-checkbox\" disabled  value=\"" + rec.id + "\" >";
                     var disableCheckHtml = "<input type=\"checkbox\" class=\"row-key-checkbox\" disabled checked value=\"" + rec.id + "\" >";
                     var isParent = rec.children != null && rec.children.length > 0;

                     if (rec.MRequirePass || rec.id == "6000") {
                         return disableCheckHtml;
                     }

                     if (value == false || rec.MRequirePass) {
                         return unCheckedHtml;
                     } else {
                         //如果是父级，查找所有子级是否启用
                         if (isParent) {
                             var isAllChildChecked = InspectSetting.isAllChildEnable(rec);
                             if (!isAllChildChecked) {
                                 return unCheckedHtml;
                             }
                         }
                         return checkedHtml;
                     }
                 }
             },
             {
                 title: HtmlLang.Write(LangModule.BD, "InspectItemSetParam", "条件"), field: 'Param', align: 'left', width: 180, sortable: false, formatter: function (value, rec, rowIndex) {
                     var isSetPeriodItem = InspectSetting.isSetPeriodItem(rec.id);
                     if (isSetPeriodItem) {

                         rec.SettingParam = rec.SettingParam ? rec.SettingParam : {};
                         if (rec.MParameter && rec.MParameter.MCompareValue) {
                             rec.SettingParam.compareValue = rec.MParameter.MCompareValue;
                         } else {
                             rec.SettingParam.compareValue = 6;
                         }

                         var html = '<div><label style="padding-right:10px;vertical-align:middle">' + HtmlLang.Write(LangModule.BD, "OverPeriod", "超过期间") + ':</label><input inspectitemid="' + rec.id + '" class="numberspiner-period" required="false"  value="' + rec.SettingParam.compareValue + '" style="width:60px;" /></div>';
                         return html;


                     }
                 }
             }
            ]],
            onLoadSuccess: function (row, data) {

                //初始化numberspiner控件信息
                InspectSetting.initNumberSpinner();

                //绑定事件
                //1.checkbox事件
                InspectSetting.bindCheckBoxCheckEvent();

                InspectSetting.bindEditItemEvent();
            },
            onClickRow: function (row) {

            },
            onClickCell: function (field, index, value) {

            }
        });
    },
    initNumberSpinner: function () {
        $(".numberspiner-period").each(function () {
            var period = $(this).val();
            var nodeId = $(this).attr("inspectitemid");
            period = period ? period : 6;

            $(this).numberspinner({
                min: 2,
                max: 40,
                onSpinUp: function () {
                    var value = $(this).numberspinner("getValue");
                    InspectSetting.setInspectParamValue(nodeId, value);
                },
                onSpinDown: function () {
                    var value = $(this).numberspinner("getValue");
                    InspectSetting.setInspectParamValue(nodeId, value);
                },
                onChange: function (newValue, oldValue) {
                    InspectSetting.setInspectParamValue(nodeId, newValue);
                }
            });

            $(this).numberspinner("setValue", period);

            var node = $("#tbInspect").treegrid("find", nodeId);

            if (!node.MEnable) {
                $(this).numberspinner("disable");
            } else {
                $(this).numberspinner("enable");
            }

        });
    },
    setInspectParamValue: function (nodeId, value) {
        var node = $("#tbInspect").treegrid("find", nodeId);

        node.SettingParam = {};

        node.SettingParam.compareValue = value;

    },
    initPeriodCombobox: function (node) {

        var period = node && node.MParameter && node.MParameter.MCompareValue ? node.MParameter.MCompareValue : 6;

        //var isComboboxValue = false;

        //for (var i = 0 ; i < InspectSetting.defalutPeriodList.length; i++) {
        //    var periodObj = InspectSetting.defalutPeriodList[i];

        //    if (periodObj.value == period) {
        //        isComboboxValue = true;
        //        break;
        //    }
        //}

        //$("#ckbOverPeriod").combobox({
        //    valueField: "id",
        //    textField: "value",
        //    data: InspectSetting.defalutPeriodList,
        //    onLoadSuccess: function () {
        //        if (isComboboxValue) {
        //            $(this).combobox("setValue", period);
        //        } else {
        //            $(this).combobox("setValue", "");
        //        }
        //    }
        //});

        //if (!isComboboxValue) {
        //    $("#rdCustomPeriodType").trigger("click");

        //    $("#tbxCustomPeriod").val(period);
        //}

        $("#ckbOverPeriod").numberspinner({
            min: 1,
            max: 100
        });

        $("#ckbOverPeriod").numberspinner("setValue", period);
    },
    bindCheckBoxCheckEvent: function () {
        var treeGrid = $("#tbInspect");
        $(".row-key-checkbox").off("click").on("click", function () {
            var id = $(this).val();
            var isCheck = $(this).is(":checked");

            InspectSetting.cascadeChildrenCheckBox(id, isCheck);
            InspectSetting.cascadeParentCheckBox(id, isCheck);

            InspectSetting.disableConditon(id, !isCheck);
        })
    },

    disableConditon: function (nodeId, disabled) {
        var numberSpinnerDom = $(".datagrid-row[node-id='" + nodeId + "']").find("td[field='Param']").find(".numberspiner-period");

        if (numberSpinnerDom && numberSpinnerDom.length > 0) {
            if (!disabled) {
                numberSpinnerDom.numberspinner("enable");
            } else {
                numberSpinnerDom.numberspinner("disable");
            }
        }
    },

    bindEditItemEvent: function () {
        var treeGrid = $("#tbInspect");
        $(".list-item-edit").on("click", function () {
            var inspectItemId = $(this).attr("inspectitem");

            var node = treeGrid.treegrid("find", inspectItemId);

            if (!node) {
                return;
            }

            var paramType = $(this).attr("inspectparamtype");

            if (paramType == 1) {
                InspectSetting.loadParamSetPeriodWindow(node);
            }

        });
    },
    cascadeChildrenCheckBox: function (parentId, isCheck) {
        var treeGrid = $("#tbInspect");

        var checkedNode = treeGrid.treegrid("find", parentId);

        checkedNode.MEnable = isCheck;

        var childrenNode = treeGrid.treegrid("getChildren", parentId);

        if (childrenNode.length > 0) {
            //选中子级
            for (var i = 0 ; i < childrenNode.length; i++) {
                var childNode = childrenNode[i];
                var nodeId = childNode.id;
                var childrenCheckBox = $(".datagrid-row[node-id='" + nodeId + "']").find(".row-key-checkbox");

                if (!isCheck) {
                    childrenCheckBox.removeAttr("checked", isCheck);

                    childrenCheckBox.attr("disabled", "disabled");
                } else {
                    childrenCheckBox.attr("checked", isCheck);
                    childrenCheckBox.removeAttr("disabled", "disabled");
                }

                childNode.MEnable = isCheck;
                //设置条件
                InspectSetting.disableConditon(nodeId, !isCheck);
            }
        }
    },
    cascadeParentCheckBox: function (childId, isCheck) {
        var treeGrid = $("#tbInspect");
        var parentNode = treeGrid.treegrid("getParent", childId);

        if (!parentNode) return;

        //找到的所有子级，进行勾选和半勾选判断
        var childrenNodes = treeGrid.treegrid("getChildren", parentNode.id);
        var parentNodeCheckBox = $(".datagrid-row[node-id='" + parentNode.id + "']").find(".row-key-checkbox");
        //如果等于1个话，就按照childId的状态进行选择
        if (childrenNodes.length == 1) {
            parentNodeCheckBox.attr("checked", isCheck);
        } else {
            //如果是选中的话，要查找所有子级科目，看是否全选择
            if (isCheck) {
                var isAllCheck = InspectSetting.isAllChildNodeCheck(parentNode, childId);

                if (isAllCheck) {
                    parentNodeCheckBox.attr("checked", true);

                } else {
                    //parentNodeCheckBox[0].indeterminate = true;
                    //parentNodeCheckBox.attr("indeterminate", true);
                }
            } else {
                //点击的子级不选中，查找所有的子级是否存在有选中的
                var isAllNoChecked = true;
                for (var i = 0; i < childrenNodes.length; i++) {
                    var childNode = childrenNodes[i];

                    if (childNode.id == childId) {
                        continue;
                    }

                    var checkBox = $(".datagrid-row[node-id='" + childNode.id + "']").find(".row-key-checkbox");

                    if (checkBox.is(":checked")) {
                        isAllNoChecked = false;
                        break;
                    }
                }
                parentNodeCheckBox.removeAttr("checked");
            }
        }
    },
    isAllChildNodeCheck: function (node, childId) {
        var isAllCheck = true;
        var childrenNodes = node.children;

        for (var i = 0; i < childrenNodes.length; i++) {
            var childNode = childrenNodes[i];

            if (childId && childNode.id == childId) {
                continue;
            }

            var checkBox = $(".datagrid-row[node-id='" + childNode.id + "']").find(".row-key-checkbox");

            if (!checkBox.is(":checked")) {
                isAllCheck = false;
                break;
            }
        }

        return isAllCheck;
    },
    isAllChildEnable: function (node) {
        var isAllEnable = true;
        var childrenNodes = node.children;

        for (var i = 0; i < childrenNodes.length; i++) {
            var childNode = childrenNodes[i];

            if (!childNode.MEnable) {
                isAllCheck = false;
                break;
            }
        }

        return isAllEnable;
    },
    requestSave: function () {
        var treeGrid = $("#tbInspect");

        var data = treeGrid.treegrid("getData");

        if (data.length == 0) {
            var tips = HtmlLang.Write(LangModule.BD, "NoDataNeedSave", "没有数据需要保存！");
            $.mDialog.alert(tips);
            return;
        }

        var inspectSettingArray = new Array();
        for (var i = 0 ; i < data.length; i++) {
            var inspectSetting = {};

            inspectSetting.MItemID = data[i].MSettingID;
            inspectSetting.MEnable = data[i].MEnable;

            inspectSettingArray.push(inspectSetting);

            var childData = data[i].children;

            if (childData && childData.length > 0) {
                for (var j = 0; j < childData.length; j++) {
                    var child = childData[j];

                    var childInspectSetting = {};
                    childInspectSetting.MItemID = child.MSettingID;
                    childInspectSetting.MEnable = child.MEnable;

                    //如果节点存在属性设置节点
                    if (child.hasOwnProperty("SettingParam")) {

                        var settingParam = {};
                        settingParam.MOperator = child.SettingParam.operator;
                        settingParam.MCompareValue = child.SettingParam.compareValue;
                        settingParam.MID = child.id;
                        settingParam.MItemID = child.MSettingParamID;

                        childInspectSetting.MSettingParam = settingParam;
                    }


                    inspectSettingArray.push(childInspectSetting);
                }
            }
        }

        //进行保存
        var url = "/BD/RoutineInspect/SaveRoutineInspectSetting";
        var params = { list: inspectSettingArray };
        mAjax.submit(url, params, function (msg) {
            if (msg && msg.Success) {
                var tips = HtmlLang.Write(LangModule.BD, "InspectSaveSuccess", "巡检项设置保存成功！");
                $.mDialog.message(tips);

                InspectSetting.loadInspectTreeList();

            } else {
                var tips = msg == null || msg.Message == null ? HtmlLang.Write(LangModule.BD, "InspectSaveFail", "巡检项设置保存失败！") : msg.Message;

                $.mDialog.alert(tips);
            }
        });
    },
    isSetPeriodItem: function (id) {
        var setPerriodItemIdList = InspectSetting.setPeriodItemIdList;
        for (var i = 0; i < setPerriodItemIdList.length; i++) {
            if (id == setPerriodItemIdList[i]) {
                return true;
            }
        }

        return false;
    },
    loadParamSetPeriodWindow: function (node) {
        //设置期间
        $(".paramset-period").show();
        $("#tbxCustomPeriod").val("");
        //定位
        var bodyDom = $("body");
        var divParamDom = $(".paramset-period");

        var left = (bodyDom.width() - divParamDom.width()) / 2;

        var top = (bodyDom.height() - divParamDom.height()) / 2;

        divParamDom.css("left", left);

        divParamDom.css("top", top - 20);

        var maskDiv = '<div id="sa_popup_overlay" style="position: absolute; z-index: 99998; top: 0px; left: 0px; width: 100%; height: 100%; background: rgb(0, 0, 0); opacity: 0.5;"></div>';

        $("body").append(maskDiv);

        $("#btnOK", ".paramset-period").on("click", function () {

            //var period = null;
            //if ($("#rdStandardPeriod").is(":checked")) {
            //    period = $("#ckbOverPeriod").combobox("getValue");
            //} else {
            //    period = $("#tbxCustomPeriod").val();
            //}

            var period = $("#ckbOverPeriod").numberspinner("getValue");

            if (!period) {
                return;
            }

            var settingParam = {};
            //大于
            settingParam.operator = 0;
            settingParam.compareValue = period;

            node.SettingParam = settingParam;

            node.MParameter = node.MParameter ? node.MParameter : {};

            node.MParameter.MCompareValue = period;

            var paramCell = $("tr[node-id='" + node.id + "']").find("td[field='Param']").find("label");

            paramCell.text(HtmlLang.Write(LangModule.BD, "OverPeriod", "超过期间") + ':' + settingParam.compareValue);


            $("#sa_popup_overlay").remove();
            $(".paramset-period").hide();
        });

        $("#btnCancel", ".paramset-period").off("click").on("click", function () {
            $("#sa_popup_overlay").remove();
            $(".paramset-period").hide();

            $("#ckbOverPeriod").remove();
        });

        $("#rdStandardPeriod").off("click").on("click", function () {
            $("#ckbOverPeriod").combobox("enable");
            $("#tbxCustomPeriod").val("").attr("disabled", true);

            $("#tbxCustomPeriod").removeClass("validatebox-invalid");
        });

        $("#rdCustomPeriodType").off("click").on("click", function () {
            $("#ckbOverPeriod").combobox("setValue", "");
            $("#ckbOverPeriod").combobox("disable");
            $("#tbxCustomPeriod").removeAttr("disabled");

            $(".validatebox-text", "#divStandardPeriod").removeClass("validatebox-invalid");
        });

        InspectSetting.initPeriodCombobox(node);
    }
}