CheckTypeDetail = {
    checkTypeList: null,
    checkTypeValueList: $("#hideCheckTypeValueList").val(),
    currencyList: null,
    contactList: null,
    employeeList: null,
    merchandiseItemList: null,
    expenseItemList: null,
    paList: null, //工资项
    trackList: null,
    trackItem1Option: null,
    trackItem2Option: null,
    trackItem3Option: null,
    trackItem4Option: null,
    trackItem5Option: null,
    //当前选中的核算维度
    currentSelectedCheckType: null,
    accountList:null,
    init: function () {
        CheckTypeDetail.initUI();
        CheckTypeDetail.initAction();
        CheckTypeDetail.initBaseData();
    },
    initUI: function (callback) {
        //加载核算维度类型
        CheckTypeDetail.loadCheckTypeList();
        
    },
    loadCheckTypeList:function(){
        var url = "/BD/BDAccount/GetCheckTypeList";
        mAjax.post(url, {}, function (data) {
            if (data && data.length > 0) {
                CheckTypeDetail.checkTypeList = data;
                for (var i = 0; i < data.length; i++) {
                    
                    var li = "<li value='" + data[i].MItemID + "' class='gl-report-checktype-li'>" + mText.encode(data[i].MName) + "</li>";
                    $("#ulCheckTypeList").append(li);
                }

                CheckTypeDetail.bindLiClickEvent();

                //加载页面初始值
                CheckTypeDetail.initCheckTypeValueView();
            }
        }, false, false, false);
    },
    initCheckTypeValueView: function () {
        
        if (CheckTypeDetail.checkTypeValueList && CheckTypeDetail.checkTypeValueList!="null") {
            CheckTypeDetail.checkTypeValueList = JSON.parse(decodeURIComponent(CheckTypeDetail.checkTypeValueList));

            for (var i = 0; i < CheckTypeDetail.checkTypeValueList.length; i++) {
                var checkTypeValueOj = CheckTypeDetail.checkTypeValueList[i];
                if (checkTypeValueOj == null) {
                    continue;
                }
                var checkType = checkTypeValueOj.MName;
                var checkTypeValue = checkTypeValueOj.MValue;
                var checkTypeValueName = mText.encode(checkTypeValueOj.MValue1);

                if (checkTypeValueName) {
                    var nameArray = checkTypeValueName.split('_');

                    var checkTypeDom = $("#view" + checkType);
                    var selectedItemSpan = "<span style='padding-left:5px' class='gl-report-checktype-viewli' checktype='" + checkType + "' keyvalue='" + checkTypeValue + "'>" + mText.encode(nameArray[1]) + "</span>";

                    if (checkTypeDom && checkTypeDom.length > 0) {
                        checkTypeDom.append(selectedItemSpan);
                    } else {
                        checkTypeDom = "<div id='view" + checkType + "'><span style='color:#39a4f8'  class='title'>" + mText.encode(nameArray[0]) + "</span></div>";
                        $("#ulChecktypeView").append(checkTypeDom);
                        $("#view" + checkType).append(selectedItemSpan);
                    }
                }
            }
        }
    },
    initAction: function () {
        $("#btnAddCheckTypeValue").click(function () {
            $("body").mask("");
            var selectedItemList = CheckTypeDetail.getSelectItemList();
            var checkTypeName = CheckTypeDetail.getCheckTypeName();
            var isTree = CheckTypeDetail.checkPlanSelectItemIsTree();
            //已经选择的id
            var selectedIds = CheckTypeDetail.getSelectedCheckTypeIdList();

            if (selectedItemList && selectedItemList.length > 0) {
                var length = selectedItemList.length;
                
                for (var i = 0; i < length; i++) {
                    var li = "";
                    var dom = selectedItemList[i];
                    //父级不选
                    if (isTree && dom.children && dom.children.length > 0) {
                        continue;
                    }

                    //隐藏的不选中
                    if (isTree && $(dom.target).css("display") == "none") {
                        continue;
                    }

                    var id = isTree ? dom.id : $(dom).val();

                    if ($.inArray(id, selectedIds) != -1) {
                        continue;
                    } else {
                        selectedIds.push(id);
                    }

                    var name = isTree ? dom.text : $(dom).parent().parent().find(".checktype-name").text();
                    li += "<li><span><input type='checkbox' value='" + id + "'/></span><span>" + mText.encode(name) + "</span></li>";
                    //在value中插入一条值
                    var checkTypeDom = $("#view" + CheckTypeDetail.currentSelectedCheckType);
                    var selectedItemSpan = "<span style='padding-left:5px' class='gl-report-checktype-viewli' checktype='" + CheckTypeDetail.currentSelectedCheckType + "' keyvalue='" + id + "'>" + mText.encode(name) + "</span>";

                    if (checkTypeDom && checkTypeDom.length > 0) {
                        checkTypeDom.append(selectedItemSpan);
                    } else {
                        checkTypeDom = "<div id='view" + CheckTypeDetail.currentSelectedCheckType + "'><span class='title' style='color:#39a4f8'>" + mText.encode(checkTypeName) + "</span></div>";
                        $("#ulChecktypeView").append(checkTypeDom);
                        $("#view" + CheckTypeDetail.currentSelectedCheckType).append(selectedItemSpan);
                    }
                    $("#checktype-value-selected").append(li);
                   
                }
            }
            $("body").unmask();
        });
        $("#btnDeleteCheckTypeValue").click(function () {
            $("input[type='checkbox']:checked", "#checktype-value-selected").each(function () {
                var checkTypeValue = $(this).val();

                $("span[keyvalue='" + checkTypeValue + "']", "#ulChecktypeView").remove();

                $(this).parent().parent().remove();
            });

            $(".gl-report-selected-allcheckbox").removeAttr("checked");
        });

        $("#btnSave").click(function () {
            var checkTypeValueList = CheckTypeDetail.getSelectChecktypeValue();

            $.mDialog.setParam(0, checkTypeValueList);

            $.mDialog.close();
        });

        $("#btnCancel").click(function () {
            $.mDialog.close();
        });

        $(".gl-report-allcheckbox").click(function () {
            var isChecked = $(this).is(":checked");
            var dom = $("#checktype-value");
            var isTree = dom.hasClass("tree");

            if (isChecked) {
                if (isTree) {
                    $(dom).tree("checkAll");
                } else {
                    $(this).parent().next().find("input[type='checkbox']").attr("checked", "checked");
                }
            } else {
                if (isTree) {
                    $(dom).tree("unCheckAll");
                } else {
                    $(this).parent().next().find("input[type='checkbox']").removeAttr("checked");
                }
                
            }
        });

        $(".gl-report-selected-allcheckbox").click(function () {
            var isChecked = $(this).is(":checked");

            if (isChecked) {
                $(this).parent().next().find("input[type='checkbox']").attr("checked", "checked");
            } else {
                $(this).parent().next().find("input[type='checkbox']").removeAttr("checked");
            }
        });

        $("#tbxSearch").on("keyup", function (e) {
            CheckTypeDetail.searchEvent(e);
        });

        $("#btnClearReview").off("click").on("click", function () {

            CheckTypeDetail.checkTypeValueList = null;
            //清空预览
            $("#ulChecktypeView").empty();
            //清空已选项
            $("#checktype-value-selected").empty();

            $(".gl-report-allcheckbox").removeAttr("checked");
            $(".gl-report-allcheckbox").trigger("click");

            $(".gl-report-allcheckbox").removeAttr("checked");

            $(".gl-report-selected-allcheckbox").removeAttr("checked");
        });
    },
    getSelectItemList:function(){
        var dom = $("#checktype-value");
        var isTree = dom.hasClass("tree");

        if (isTree) {
            return $(dom).tree("getChecked");
        } else {
            return $("input[type='checkbox']:checked", "#checktype-value");
        }
    },
    checkPlanSelectItemIsTree:function(){
        var dom = $("#checktype-value");
        var isTree = dom.hasClass("tree");
        return isTree;
    },
    searchEvent:function(e){
        var dom = $("#checktype-value");
        var isTree = dom.hasClass("tree");
        var keyword = $("#tbxSearch").val();
        if (isTree) {
            $(dom).tree("doFilter", keyword);
        } else {
            $(dom).find("li").each(function () {
                var text = $(this).text();

                var isMatch = text.indexOf(keyword) >= 0 || keyword=="";

                if (isMatch) {
                    $(this).show();
                } else {
                    $(this).hide();
                }

            });
        }
    },
    checkTypeValueIsSelected: function (checkTypeId) {
        var result = false;
        var doms = $("li", "#checktype-value-selected");
        if (doms && doms.length > 0) {
            var length = doms.length;
            for (var i = 0 ; i < length; i++) {
                var id = $("input[type='checkbox']" ,$(doms[i])).val();
                if (checkTypeId == id) {
                    result = true;
                    break;
                }
            }  
        }
        return result;
    },
    getSelectedCheckTypeIdList:function(){
        var list = new Array();
        var doms = $("li", "#checktype-value-selected");
        if (doms && doms.length > 0) {
            var length = doms.length;
            for (var i = 0 ; i < length; i++) {
                var id = $("input[type='checkbox']", $(doms[i])).val();
                list.push(id);
            }
        }
        return list;
    },
    getSelectChecktypeValue: function () {
        var array = new Array();

        $(".gl-report-checktype-viewli").each(function () {
            var checkType = $(this).attr("checktype");
            var checkValue = $(this).attr("keyvalue");
            var checkTypeName = $("#view" + checkType).find(".title").text();
            var name = mText.encode(checkTypeName + "_" + $(this).text());
            var data = { key: checkType, value: checkValue, name: name };
            array.push(data);
        });
        return array;
    },
    initBaseData:function(){
        if (CheckTypeDetail.trackList == null) {
            var url = "/BD/Tracking/GetTrackBasicInfo";
            CheckTypeDetail.getBaseDataAysn(url, function (data) {
                CheckTypeDetail.trackList = data;
                //进行拆分
                if (data && data.length > 0) {
                    for (var i = 0; i < data.length; i++) {
                        var tempTrack = data[i];
                        var tempTrackOption = tempTrack.MChildren;

                        switch (i) {
                            case 0:
                                CheckTypeDetail.trackItem1Option = tempTrackOption;
                                break;
                            case 1:
                                CheckTypeDetail.trackItem2Option = tempTrackOption;
                                break;
                            case 2:
                                CheckTypeDetail.trackItem3Option = tempTrackOption;
                                break;
                            case 3:
                                CheckTypeDetail.trackItem4Option = tempTrackOption;
                                break;
                            case 4:
                                CheckTypeDetail.trackItem5Option = tempTrackOption;
                                break;
                        }
                    }
                }

            });
        }
    },
    getBaseDataAysn: function (url, callback) {
        $.mAjax.post(url, null, function (data) {
            if (data && callback && $.isFunction(callback)) {
                callback(data);
            }
        }, false, true, false);
    },
    bindLiClickEvent: function (selector) {
        $(".gl-report-checktype-li").on("click", function () {
            $(".gl-report-checktyp-liselected").removeClass("gl-report-checktyp-liselected");
            $(this).addClass("gl-report-checktyp-liselected");
            var checkType = $(this).attr("value");

            CheckTypeDetail.currentSelectedCheckType = checkType;

            CheckTypeDetail.loadCheckCheckTypeValue(checkType);
            //还原数据
            CheckTypeDetail.loadSelectedCheckCheckTypeValue(checkType);

            $(".gl-report-allcheckbox").removeAttr("checked");
        });
        
    },
    loadCheckCheckTypeValue: function (checkType) {
        $("#checktype-value").empty();
        var data = CheckTypeDetail.getBaseDataByCheckTypeEnum(checkType);
        if (checkType == "0") {
            CheckTypeDetail.setPlanSelectTree(data, "id", "text");
        } else if (checkType == "1") {
            CheckTypeDetail.setPlanSelectLi(data, "MItemID", "MName");
        } else if (checkType == "2") {
            CheckTypeDetail.setPlanSelectLi(data, "MItemID", "MNumber", "MDesc");
        } else if (checkType == "3") {
            CheckTypeDetail.setPlanSelectTree(data, "id", "text");
        } else if (checkType == "4") {
            CheckTypeDetail.setPlanSelectTree(data, "id", "text");
        } else if (checkType == "5" || checkType == "6" || checkType == "7" || checkType == "8" || checkType == "9") {
            CheckTypeDetail.setPlanSelectLi(data, "MValue", "MName");
        }
    },
    //设置备选的li
    setPlanSelectLi: function (data, key, value,extendKey) {
        $("#checktype-value").removeClass("tree").addClass("backgroup");
        if (data && data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                var text = data[i][value];
                if (extendKey != undefined) {
                    text += ":" + data[i][extendKey];
                }
                text = mText.encode(text);
                var li = "<li><span><input type='checkbox' value='" + data[i][key] + "'/></span><span class='checktype-name' style='padding-left:5px'>" + text+ "</span></li>";
                $("#checktype-value").append(li);
            }
        }
    },
    setPlanSelectTree: function (data, key, value) {
        $("#checktype-value").addClass("tree").removeClass("backgroup");
        $("#checktype-value").tree({
            valueField: key,
            textField: value,
            data: data,
            height: 230,
            showDisabled:true,
            checkbox:function(node){
                if (node.children > 0) {
                    return false;
                }
                return true;
            },
            onBeforeSelect: function (node) {
                //不是叶子节点不允许选择
                if (!$(this).tree('isLeaf', node.target)) {
                    return false;
                }

                //联系人标题不允许选择
                if (node.id == "Customer" || node.id == "Supplier" || node.id == "Other") {
                    return false;
                }

            },
            onSelect: function (data) {
               

            },
            onLoadSuccess: function () {
                
            }
        });
    },
    loadSelectedCheckCheckTypeValue: function (checkTypeEnum) {
        $("#checktype-value-selected").empty();
        $("#ulChecktypeView").find("span").each(function(){
            var checkType = $(this).attr("checktype");
            if (checkType == checkTypeEnum) {
                var id = $(this).attr("keyvalue");
                var name = $(this).text();
                var li = "<li><span><input type='checkbox' value='" + id + "'/></span><span>" + mText.encode(name) + "</span></li>";
                $("#checktype-value-selected").append(li);
            }
        });
         
    },
    //加载基础数据
    getBaseDataByCheckTypeEnum: function (checkTypeEnum) {
        if (checkTypeEnum == "0") {

            if (CheckTypeDetail.contactList == null) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=0";
                CheckTypeDetail.getBaseDataAysn(url, function (data) {
                    if (data) {
                        CheckTypeDetail.contactList = data.MDataList;
                    }

                });
            }
            return CheckTypeDetail.contactList;
        } else if (checkTypeEnum == "1") {
            if (CheckTypeDetail.employeeList == null) {
                var url = "/BD/Employees/GetBDEmployeesList";
                CheckTypeDetail.getBaseDataAysn(url, function (data) {
                    CheckTypeDetail.employeeList = data;
                });
            }
            return CheckTypeDetail.employeeList;

        } else if (checkTypeEnum == "2") {
            if (CheckTypeDetail.merchandiseItemList == null) {
                var url = "/BD/Item/GetItemList";
                CheckTypeDetail.getBaseDataAysn(url, function (data) {
                    CheckTypeDetail.merchandiseItemList = data;
                });
            }

            return CheckTypeDetail.merchandiseItemList;

        } else if (checkTypeEnum == "3") {
            if (CheckTypeDetail.expenseItemList == null) {
                var url = "/GL/GLCheckType/GetCheckTypeDataByType?type=3";
                CheckTypeDetail.getBaseDataAysn(url, function (data) {
                    if (data) {
                        CheckTypeDetail.expenseItemList = data.MDataList;
                    }

                });
            }
            return CheckTypeDetail.expenseItemList;

        } else if (checkTypeEnum == "4") {

            if (CheckTypeDetail.paList == null) {
                var url = "/PA/PayItem/GetSalaryItemTreeList";
                CheckTypeDetail.getBaseDataAysn(url, function (data) {
                    CheckTypeDetail.paList = data;
                });
            }
            return CheckTypeDetail.paList;
        } else if (checkTypeEnum == "5") {
            return CheckTypeDetail.trackItem1Option;
        } else if (checkTypeEnum == "6") {
            return CheckTypeDetail.trackItem2Option;
        } else if (checkTypeEnum == "7") {
            return CheckTypeDetail.trackItem3Option;
        } else if (checkTypeEnum == "8") {
            return CheckTypeDetail.trackItem4Option;
        } else if (checkTypeEnum == "9") {
            return CheckTypeDetail.trackItem5Option;
        }
    },
    getCheckTypeName: function (checkTypeEnum) {
        if (!checkTypeEnum) {
            checkTypeEnum = CheckTypeDetail.currentSelectedCheckType;
        }
        var checkTypeName = "";
        for (var i = 0; i < CheckTypeDetail.checkTypeList.length; i++) {
            var checkType = CheckTypeDetail.checkTypeList[i];

            if (checkType.MItemID == checkTypeEnum) {
                checkTypeName = checkType.MName;
                break;
            }
        }

        return checkTypeName;
    }
}