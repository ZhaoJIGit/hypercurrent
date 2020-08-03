(function () {
    var GLCheckType = (function () {
        //
        var GLCheckType = function () {
            //
            var that = this;
            //
            var getCheckTypeDataUrl = "/GL/GLCheckType/GetCheckTypeDataByType?type=";
            //
            var requiredText = '<span style="color: red;font-weight: bold;">*</span>';

            //
            var checkGroupUlClass = "vc-checkgroup-ul";
            //
            var checkGroupLiClass = "vc-checkgroup-li";

            var checkGroupInvalidClass = "v-c-i-l";

            //根据核算维度类型获取下拉值 返回的类型是 {Name:name,Type:typeId,Value:[ModelList]}的形式
            this.getCheckTypeData = function (type, func, force, aysnc) {
                //如果没有回调函数的话，肯定是同步获取的
                var aysnc = $.isFunction(func) || aysnc === true;
                //如果强制重新获取，或者本来就没有数据，则重新增加一个
                if (force || !GLCheckType.CheckTypeList[type].data) {
                    //异步获取
                    mAjax.post(getCheckTypeDataUrl + type, {}, function (data) {
                        //
                        var data = data || [];
                        //
                        GLCheckType.CheckTypeList[type].data = data;
                        //
                        data && $.isFunction(func) && func(data);
                    }, "", "", aysnc);
                }
                //只有在同步的情况下才返回数据
                if (!aysnc) {
                    //
                    return GLCheckType.CheckTypeList[type].data;
                }
            };

            this.getCheckTypeColumnByType = function (type) {
                //联系人
                for (var i = 0; i < GLCheckType.CheckTypeList.length; i++) {
                    //
                    if (GLCheckType.CheckTypeList[i].type == type) {
                        return GLCheckType.CheckTypeList[i].column;
                    }
                }
            }

            //
            this.getCheckTypeByColumnName = function (column) {
                //联系人
                for (var i = 0; i < GLCheckType.CheckTypeList.length; i++) {
                    //
                    if (GLCheckType.CheckTypeList[i].column == column) {
                        return GLCheckType.CheckTypeList[i];
                    }
                }
            }
            //
            this.isCheckGroupEnbale = function (checkGroupValue) {
                //1.选录， 2，必录
                return checkGroupValue == 1 || checkGroupValue == 2;
            };
            //
            this.filterTreeDataClearID = function (treeData, filter) {
                //
                var fond = false;
                //
                for (var i = 0 ; i < treeData.length ; i++) {
                    //如果本身具有特性
                    if (filter(treeData[i])) {
                        //
                        fond = true;
                    }
                        //如果子集有符合条件的也行
                    else if (treeData[i].children && treeData[i].children.length > 0) {
                        //
                        if (that.filterTreeDataClearID(treeData[i].children, filter)) {
                            //
                            fond = true;
                        }
                        else {
                            //清空ID
                            treeData[i].id = null; treeData[i].text = null;
                        }
                    }
                    else {

                        //清空ID
                        treeData[i].id = null; treeData[i].text = null;
                    }
                }
                //
                return fond;
            }
            //
            this.filterTreeDataWithNoID = function (dataList) {
                //
                var newList = [];
                //
                for (var i = 0; i < dataList.length ; i++) {
                    //
                    if (dataList[i].id) {
                        //
                        if (dataList[i].children && dataList[i].children.length > 0) {
                            //
                            dataList[i].children = that.filterTreeDataWithNoID(dataList[i].children);
                        }
                        //
                        newList.push(dataList[i]);
                    }
                }
                //
                return newList;
            };
            //过滤树里面的内容
            this.filterTreeData = function (data, keyword) {
                //
                var data = $.extend(true, [], data);
                //
                var filter = function (node) {
                    //
                    return node && keyword && node.text.toLowerCase().indexOf(keyword.toLowerCase()) >= 0;
                }
                //
                if (data && data.length > 0) {
                    //
                    that.filterTreeDataClearID(data, filter);
                    //
                    data = that.filterTreeDataWithNoID(data);
                }
                return data;
            }
            this.getCheckTypeAddComboboxName = function (name, data) {
                switch (name) {
                    case "item":
                        return ["inventory", undefined];
                    case "track":
                        return ["trackOption", "/BD/Tracking/CategoryOptionEdit?trackId=" + data.MCheckTypeGroupID];
                    default: return [name, undefined];

                }
            }
            //
            this.bindCheckType2Dom = function (typeValue, type, titleDom, valueDom, index, options, value, noRequired) {
                //
                if (that.isCheckGroupEnbale(typeValue)) {
                    //
                    var required = that.isCheckTypeRequired(typeValue);
                    //
                    var data = that.getCheckTypeData(type);
                    //
                    titleDom.html((required ? requiredText : "") + (mText.encode(data.MCheckTypeName) || ""));
                    //
                    valueDom.data("data", data);
                    //正常的combobox显示
                    if (data.MShowType == 0 || data.MShowType == 1) {
                        //
                        valueDom.mAddCombobox(that.getCheckTypeAddComboboxName(GLCheckType.CheckTypeList[type].name, data)[0], {
                            data: data.MDataList,
                            hasDownArrow: true,
                            required: required && !noRequired,
                            textField: "text",
                            valueField: "id",
                            hideItemKey: "MIsActive",
                            hideItemValue: false,
                            width: options.width,
                            heght: options.heght,
                            groupField: data.MShowType == 0 ? null : "parentId",
                            onSelect: function (row) {

                                valueDom.data("selectedvalue", row);
                                //
                                return options.onSelect(row, valueDom, titleDom);
                            },
                            onLoadSuccess: function () {

                                (value != null && value != undefined) ? valueDom.combobox("setValue", value) : "";
                            }
                        },
                        {
                            hasPermission: 1,
                            hideWhenClose: options.hideWhenClose,
                            url: that.getCheckTypeAddComboboxName(GLCheckType.CheckTypeList[type].name, data)[1],
                            //关闭后需要重新加载摘要
                            callback: function () {
                                //重新获取摘要
                                that.getCheckTypeData(type, function (dt) {
                                    //
                                    titleDom.html(mText.encode(dt.MCheckTypeName));

                                    valueDom.combobox("options").data = dt.MDataList;
                                    //重新加载
                                    valueDom.combobox("loadData", dt.MDataList);
                                }, true);
                            }
                        });
                    }
                    else {
                        //工资项目
                        if (GLCheckType.CheckTypeList[type].name == "payitem") {
                            //工资项目按照树显示
                            valueDom.mAddCombotree(GLCheckType.CheckTypeList[type].name, {
                                data: data.MDataList,
                                width: options.width,
                                heght: options.heght,
                                required: required && !noRequired,
                                onBeforeSelect: function (node) {
                                    if (!$(this).tree('isLeaf', node.target)) {
                                        return false;
                                    }
                                },
                                onSelect: function (row) {
                                    valueDom.data("selectedvalue", row);
                                    //
                                    return options.onSelect(row, valueDom, titleDom);
                                },
                                onLoadSuccess: function () {
                                    (value != null && value != undefined) ? valueDom.combotree("setValue", value) : "";
                                }
                            }, {
                                hasPermission: true,
                                hideWhenClose: options.hideWhenClose,
                                itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                                dialogTitle: HtmlLang.Write(LangModule.BD, "NewSalaryItem", "New Salary Item"),
                                url: "/PA/PayItem/PayItemEdit?type=0",
                                width: 420,
                                height: 365,
                                callback: function () {
                                    //
                                    var data = that.getCheckTypeData(type, null, true, false);
                                    //
                                    valueDom.data("data", data);
                                    //
                                    $(valueDom).combotree("loadData", data.MDataList);
                                }
                            });
                        }
                        else if (GLCheckType.CheckTypeList[type].name == "contact") {
                            //联系人按照树显示
                            valueDom.mAddCombotree(GLCheckType.CheckTypeList[type].name, {
                                data: data.MDataList,
                                width: options.width,
                                heght: options.heght,
                                required: required && !noRequired,
                                onBeforeSelect: function (node) {

                                    if (node.id == "Supplier" || node.id == "Customer" || node.id == "Other") return false;

                                    if (!$(this).tree('isLeaf', node.target)) {
                                        return false;
                                    }
                                },
                                onSelect: function (row) {
                                    valueDom.data("selectedvalue", row);
                                    //
                                    return options.onSelect(row, valueDom, titleDom);
                                },
                                onLoadSuccess: function () {
                                    (value != null && value != undefined) ? valueDom.combotree("setValue", value) : "";
                                }
                            }, {
                                hasPermission: true,
                                hideWhenClose: options.hideWhenClose,
                                itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                                dialogTitle: HtmlLang.Write(LangModule.Common, "NewContact", "New Contact"),
                                url: "/BD/Contacts/ContactsEdit/",
                                width: 1080,
                                height: 450,
                                callback: function () {
                                    //
                                    var data = that.getCheckTypeData(type, null, true, false);
                                    //
                                    valueDom.data("data", data);
                                    //
                                    $(valueDom).combotree("loadData", data.MDataList);
                                }
                            });
                        }
                        else if (GLCheckType.CheckTypeList[type].name == "expense") {
                            //费用报销按照树显示
                            valueDom.mAddCombotree(GLCheckType.CheckTypeList[type].name, {
                                data: data.MDataList,
                                width: options.width,
                                heght: options.heght,
                                required: required && !noRequired,
                                onBeforeSelect: function (node) {
                                    if (!$(this).tree('isLeaf', node.target)) {
                                        return false;
                                    }
                                },
                                onSelect: function (row) {
                                    valueDom.data("selectedvalue", row);
                                    //
                                    return options.onSelect(row, valueDom, titleDom);
                                },
                                onLoadSuccess: function () {
                                    (value != null && value != undefined) ? valueDom.combotree("setValue", value) : "";
                                }
                            }, {
                                hasPermission: true,
                                hideWhenClose: options.hideWhenClose,
                                itemTitle: HtmlLang.Write(LangModule.Common, "New", "New"),
                                dialogTitle: HtmlLang.Write(LangModule.Common, "NewExpenseItem", "New Expense Item"),
                                url: "/BD/ExpenseItem/ExpenseItemEdit/",
                                width: 520,
                                height: 400,
                                callback: function () {
                                    //
                                    var data = that.getCheckTypeData(type, null, true, false);
                                    //
                                    valueDom.data("data", data);
                                    //
                                    $(valueDom).combotree("loadData", data.MDataList);
                                }
                            });
                        }

                    }
                    //
                    ++index;
                }
                //
                return index;
            };
            //组织冒泡事件
            this.stopPropagation = function (e) {
                //组织冒泡事件
                if (e.stopPropagation) {
                    // this code is for Mozilla and Opera 
                    e.stopPropagation();
                } else if (window.event) {
                    // this code is for IE 
                    window.event.cancelBubble = true;
                }
                //
                return false;
            };
            //把一个model的核算维度的值，组装成一个核算维度model
            this.setCheckGroupValueModel = function (data, fieldName) {
                //
                fieldName = fieldName || "MCheckGroupValueModel";
                //
                var data = ($.isArray(data) ? data : [data]) || 0;
                //
                for (var i = 0; i < data.length ; i++) {
                    //
                    var model = {};
                    //
                    for (var j = 0; j < GLCheckType.CheckTypeList.length; j++) {
                        //
                        if (data[i][GLCheckType.CheckTypeList[j].column]) {
                            //
                            model[GLCheckType.CheckTypeList[j].column] = data[i][GLCheckType.CheckTypeList[j].column];
                        }
                    }
                    //额外的一个字段，也要进行传值
                    model.MContactType = that.getContactType(data[i].MContactType);
                    //
                    data[i][fieldName] = model;
                }
                //
                return data;
            };
            //将字符串转成int类型
            this.getContactType = function (type) {

                switch (type) {
                    case "Customer":
                        return 0;
                    case "Supplier":
                        return 1;
                    case "Other":
                        return 2;
                }
                return 2;
            }

            //从一棵树的节点中找到对应的id
            this.filterCheckTypeData = function (type, id, text, data) {
                //
                data = data || that.getCheckTypeData(type).MDataList;
                //
                for (var i = 0; i < data.length ; i++) {
                    //
                    if ((id && data[i].id === id) || (text && data[i].text === text)) {
                        //
                        return data[i];
                    }
                    //
                    if (data[i].children && data[i].children.length > 0) {
                        //
                        var find = that.filterCheckTypeData(type, id, text, data[i].children);
                        //
                        if (find) {
                            //
                            return find;
                        }
                    }
                }
                //
                return false;
            };
            //
            this.isCheckTypeRequired = function (checkTypeID) {
                //
                return checkTypeID === 2;
            }
            //
            this.isCheckTypeEnable = function (checkTypeID) {
                return checkTypeID == 1 || checkTypeID == 2;
            }
            //
            this.bindCheckTypes2Doms = function (checkGroupModel, titleDoms, valueDoms, options) {
                //
                var index = 0;
                //联系人
                for (var i = 0; i < GLCheckType.CheckTypeList.length; i++) {
                    //
                    var type = GLCheckType.CheckTypeList[i];
                    //
                    index = that.bindCheckType2Dom(checkGroupModel[type.column], type.type, titleDoms.eq(index), valueDoms.eq(index), index, options);
                }
            }
            //
            this.getCurrencyHtml = function (currency, noEncode, isCheckForCurrency) {
                //
                if ((currency && currency.MCurrencyID && currency.MCurrencyID != GLVoucherHome.baseCurrencyID)) {
                    //
                    var html = "<ul class='vc-currency-ul'>";
                    //
                    html += "<li >" + mMath.toMoneyFormat(currency.MAmountFor) + "[" + currency.MCurrencyID + "]</li>";
                    //
                    html += "<li >" + "1 : " + (+(currency.MExchangeRate)).toFixed(6) + "[" + GLVoucherHome.baseCurrencyID + "]</li>";
                    //
                    html += "</ul>";
                    //
                    return noEncode ? html : mText.encode(html);
                }
                //
                return "";
            }
            //把一个核算维度model组装成一个div
            this.getCheckGroupValueHtml = function (checkGroupValue, checkGroup, ulClass, liClass) {
                //
                ulClass = ulClass || checkGroupUlClass;
                //
                liClass = liClass || checkGroupLiClass;
                //
                if (!checkGroupValue) {
                    return "";
                }
                //
                checkGroup = checkGroup || {};
                //
                var html = "<ul class='" + ulClass + "'>";
                //
                if (checkGroupValue.MContactID && checkGroupValue.MContactName) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MContactID) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(that.getCheckTypeName(CheckTypeEnum.MContactID) + ":" + checkGroupValue.MContactName) + "</li>";
                }
                //
                if (checkGroupValue.MEmployeeID && checkGroupValue.MEmployeeName) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MEmployeeID) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(that.getCheckTypeName(CheckTypeEnum.MEmployeeID) + ":" + checkGroupValue.MEmployeeName) + "</li>";
                }
                //
                if (checkGroupValue.MMerItemID && checkGroupValue.MMerItemName) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MMerItemID) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(that.getCheckTypeName(CheckTypeEnum.MMerItemID) + ":" + checkGroupValue.MMerItemName) + "</li>";
                }
                //
                if (checkGroupValue.MExpItemID && checkGroupValue.MExpItemName) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MExpItemID) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(that.getCheckTypeName(CheckTypeEnum.MExpItemID) + ":" + checkGroupValue.MExpItemName) + "</li>";
                }
                //
                if (checkGroupValue.MPaItemID && (checkGroupValue.MPaItemName || checkGroupValue.MPaItemGroupName)) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MPaItemID) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(that.getCheckTypeName(CheckTypeEnum.MPaItemID) + ":" + (checkGroupValue.MPaItemName ? checkGroupValue.MPaItemName : checkGroupValue.MPaItemGroupName)) + "</li>";
                }
                //
                if (checkGroupValue.MTrackItem1 && checkGroupValue.MTrackItem1Name) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MTrackItem1) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(checkGroupValue.MTrackItem1GroupName + ":" + checkGroupValue.MTrackItem1Name) + "</li>";
                }
                //
                if (checkGroupValue.MTrackItem2 && checkGroupValue.MTrackItem2Name) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MTrackItem2) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(checkGroupValue.MTrackItem2GroupName + ":" + checkGroupValue.MTrackItem2Name) + "</li>";
                }
                //
                if (checkGroupValue.MTrackItem3 && checkGroupValue.MTrackItem3Name) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MTrackItem3) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(checkGroupValue.MTrackItem3GroupName + ":" + checkGroupValue.MTrackItem3Name) + "</li>";
                }
                //
                if (checkGroupValue.MTrackItem4 && checkGroupValue.MTrackItem4Name) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MTrackItem4) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(checkGroupValue.MTrackItem4GroupName + ":" + checkGroupValue.MTrackItem4Name) + "</li>";
                }
                //
                if (checkGroupValue.MTrackItem5 && checkGroupValue.MTrackItem5Name) {

                    var t = checkGroup.MItemID && !that.isCheckTypeEnable(checkGroup.MTrackItem5) ? checkGroupInvalidClass : " ";
                    //联系人
                    html += "<li class='" + liClass + " " + t + "'>" + mText.encode(checkGroupValue.MTrackItem5GroupName + ":" + checkGroupValue.MTrackItem5Name) + "</li>";
                }
                //
                html += "</ul>";
                //
                return html;
            };
            //根据核算维度获取核算维度的名字
            this.getCheckTypeName = function (checkType) {
                //
                switch (checkType) {
                    case CheckTypeEnum.MContactID:
                        return HtmlLang.Write(LangModule.BD, "Contact", "联系人");
                    case CheckTypeEnum.MEmployeeID:
                        return HtmlLang.Write(LangModule.BD, "Employee", "员工");
                    case CheckTypeEnum.MMerItemID:
                        return HtmlLang.Write(LangModule.BD, "MerItem", "商品项目");
                    case CheckTypeEnum.MExpItemID:
                        return HtmlLang.Write(LangModule.BD, "ExpenseItem", "费用项目");
                    case CheckTypeEnum.MPaItemID:
                        return HtmlLang.Write(LangModule.BD, "PayItem", "工资项目");
                    default:
                        return "";
                }
            };
            //
            this.validateCheckGroup = function (checkGroup, checkGroupValue, clear) {
                //
                var nameList = [];
                //联系人
                for (var i = 0; i < GLCheckType.CheckTypeList.length; i++) {
                    //
                    var type = GLCheckType.CheckTypeList[i];
                    //
                    var typeValue = checkGroup[type.column];
                    //
                    var typeGroupValue = checkGroupValue[type.column];
                    //
                    var data = that.getCheckTypeData(type.type);
                    //
                    if (typeValue == 2 && !typeGroupValue) {
                        //
                        nameList.push(data.MCheckTypeName);
                    }
                    //如果核算维度为禁用，但是却有值，并且需要清除的话，就直接清除了
                    if ((typeValue != 1 && typeValue != 2) && typeGroupValue && clear) {
                        //
                        checkGroupValue[type.column] = null;
                    }
                }
                //
                return nameList.length > 0 ? nameList : checkGroupValue;
            }

            //填充 工资项目 跟踪项的 MPaGroupItemID 
            this.fillPaItemGroupID = function (checkGroupValueModel) {
                //
                if (checkGroupValueModel.MPaItemID) {
                    //
                    var data = GLCheckType.CheckTypeList[4].data.MDataList;
                    //
                    if (data && data.length > 0) {
                        //去找到这个值
                        var paItem = that.filterCheckTypeData(null, checkGroupValueModel.MPaItemID, null, data);
                        //如果是group的话，
                        checkGroupValueModel.MPaItemGroupID = paItem.parentId ? paItem.parentId : paItem.id;
                    }
                }
                //
                return checkGroupValueModel;
            }
        }
        //
        return GLCheckType;
    })();
    //
    window.GLCheckType = GLCheckType;
    //核算维度类型
    $.extend(window.GLCheckType, {
        CheckTypeList: [
            {
                type: 0,
                data: null,
                column: "MContactID",
                name: "contact"
            },
            {
                type: 1,
                data: null,
                column: "MEmployeeID",
                name: "employee"
            },
            {
                type: 2,
                data: null,
                column: "MMerItemID",
                name: "item"
            },
            {
                type: 3,
                data: null,
                column: "MExpItemID",
                name: "expense"
            },
            {
                type: 4,
                data: null,
                column: "MPaItemID",
                name: "payitem"
            },
            {
                type: 5,
                data: null,
                column: "MTrackItem1",
                name: "track"
            },
            {
                type: 6,
                data: null,
                column: "MTrackItem2",
                name: "track"
            },
            {
                type: 7,
                data: null,
                column: "MTrackItem3",
                name: "track"
            },
            {
                type: 8,
                data: null,
                column: "MTrackItem4",
                name: "track"
            },
            {
                type: 9,
                data: null,
                column: "MTrackItem5",
                name: "track"
            },
        ]
    });
})()