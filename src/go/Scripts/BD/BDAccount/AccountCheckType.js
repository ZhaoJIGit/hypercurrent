var AccountCheckType = {
    checkTypeList: null,
    //获取系统核算维度
    getCheckTypeList: function (callback) {
        var url = "/BD/BDAccount/GetCheckTypeList";
        mAjax.post(url, {}, function (data) {
            if (data) {
                if (callback && $.isFunction(callback)) {
                    AccountCheckType.checkTypeList = data;
                    callback(data);
                }
            }
        }, false, false, true);
    },
    getCheckGroupValueObject: function () {

        var checkGroupValue = AccountCheckType.getNullAccountCheckGroup();

        $(".account-checktype-item").each(function (index) {
            var checkTypeItemDom = $(this);
            var checkTypeCombobox = $(checkTypeItemDom).find(".accout-checktype");
            //核算维度类型
            var checkType = checkTypeCombobox && checkTypeCombobox.length > 0 ? $(checkTypeCombobox).combobox("getValue") : "";

            if (checkType && (checkType != "" || checkType != "-1")) {
                //必录和选录
                var enterTypeCombobox = $(checkTypeItemDom).find(".account-checktype-enterstatus");

                var enterTypeValue = $(enterTypeCombobox).combobox("getValue");

                var required = enterTypeValue == "1" ? true : false;

                //启用和禁用
                var enableStatusDom = $(checkTypeItemDom).find(".enable-stauts");
                var enabled = false;
                if (enableStatusDom.attr("value") === "1") {
                    enabled = true;
                }

                AccountCheckType.setCheckGroupValue(checkGroupValue, checkType, required, enabled);
            }
        })

        return checkGroupValue;
    },
    //checkgroup的值
    setCheckGroupValue: function (checkGroupOj, checkType, required, enabled) {
        var result = 0;
        if (required && enabled) {
            result = 2;   //必录 启用
        } else if (required && !enabled) {
            result = 4;  // 必录 禁用
        } else if (!required && enabled) {
            result = 1;   //选填 启用
        } else if (!required && !enabled) {
            result = 3;   //选填 禁用
        }

        switch (checkType) {
            case "0":
                checkGroupOj.MContactID = result;
                break;
            case "1":
                checkGroupOj.MEmployeeID = result;
                break;
            case "2":
                checkGroupOj.MMerItemID = result;
                break;
            case "3":
                checkGroupOj.MExpItemID = result;
                break;
            case "4":
                checkGroupOj.MPaItemID = result;
                break;
            case "5":
                checkGroupOj.MTrackItem1 = result;
                break;
            case "6":
                checkGroupOj.MTrackItem2 = result;
                break;
            case "7":
                checkGroupOj.MTrackItem3 = result;
                break;
            case "8":
                checkGroupOj.MTrackItem4 = result;
                break;
            case "9":
                checkGroupOj.MTrackItem5 = result;
                break;
        }
    },
    initAction: function () {
        $(".enable-stauts").each(function () {
            AccountCheckType.bindEnabledEvent($(this));
        });

        $(".input-status").each(function () {
            AccountCheckType.bindInputStatus($(this));
        });

        $("#btnAddNewCheckItem").click(function () {
            var templateDom = $(".account-check-template");
            var insertDom = AccountCheckType.getCheckTypeDom();

            $(".m-list-accountcheck").append(insertDom);

            var comboboxDom = $(insertDom).find(".accout-checktype");

            var isCurrentAccount = AccountEdit.isCurrentAccount(AccountEdit.account);;

            AccountCheckType.initAccountCheckCombobox(comboboxDom, isCurrentAccount);

            //初始化类型选择下拉框
            var checkTypeEnterTypeCombobox = $(insertDom).find(".account-checktype-enterstatus");

            AccountCheckType.initAccountCheckEnterTypeCombobox(checkTypeEnterTypeCombobox);
            //绑定删除事件
            var deleteDom = $(insertDom).find(".check-type-delete");
            AccountCheckType.bindDeleteEvent(deleteDom);

            $(insertDom).addClass("new-checktype");
            
        });

        $(".check-type-delete").each(function () {
            AccountCheckType.bindDeleteEvent($(this));
        })
    },
    //初始化核算维度类型下拉框
    initAccountCheckCombobox: function (dom, isCurrentAccount) {
        if (!dom) {
            return;
        }
        var bindData = AccountCheckType.getAccountCheckTypeList(isCurrentAccount);

        $(dom).combobox({
            textField: "MName",
            valueField: "MItemID",
            data: bindData,
            onChange: function (newValue, oldValue) {

                if (newValue == "-1") {
                    return;
                }

                var isSelected = AccountCheckType.checkTypeIsSelected(newValue, $(this));
                if (isSelected) {
                    var tips = HtmlLang.Write(LangModule.Acct, "CheckTypeSelected", "核算维度已经选择，请使用其他维度!");
                    $.mDialog.alert(tips);

                    setTimeout(function () {
                        $(dom).combobox("setValue", "");
                    }, 200);

                    return;
                }
            },
            onLoadSuccess: function () {
                $(this).combobox("setValue", "-1");
            }
        });
    },
    initAccountCheckEnterTypeCombobox:function(dom){
        var data = AccountCheckType.getCheckEnterType();
        $(dom).combobox({
            valueField: "id",
            textField: "text",
            data: data,
            onChange: function (newValue, oldValue) {
                var topDom = $(this).parent().parent();
                var oldEnterType = $(topDom).find(".oldinputstatus").val();
                //必录改成非必录，需要提示用户确认
                if (oldEnterType === "true" && newValue == "0") {
                    var url = "/BD/BDAccount/CheckTypeIsUsed";
                    var param = {};
                    if ($("#isLeaf").val() && !$("#hidAccountID").val()) {
                        param.accountId = AccountEdit.getSelectedNode().MItemID;
                    } else {
                        param.accountId = $("#hidAccountID").val();
                    }

                    param.checkType = $(topDom).find(".accout-checktype").combobox("getValue");

                    $.mAjax.post(url, param, function (data) {
                        if (data && !data.Success) {
                            var confirmMsg = HtmlLang.Write(LangModule.BD, "CheckTypeIsUsedContiunChange", "维度已经在以下数据中使用，是否更改为选录?</br>");

                            $.mDialog.confirm("<div>"+confirmMsg+"<br>"+ data.Message +"</div>", function () {
                                //
                            }, function () {
                                $(topDom).find(".account-checktype-enterstatus").combobox("setValue", "1");
                            }, false, false, true);
                        }
                    }, true, true);
                }
            }
        });
    },
    getAccountCheckTypeList: function (isCurrentAccount) {
        var bindData = new Array();

        var tipsItem = {
            "MItemID":"-1",
            "MName": HtmlLang.Write(LangModule.Acct, "SelectCheckType", "请选择核算维度"),
        }

        bindData.push(tipsItem);

        bindData = bindData.concat(AccountCheckType.checkTypeList);

        if (isCurrentAccount) {
            var tempBindData = new Array();

            for (var i = 0; i < bindData.length; i++) {
                var checkType = bindData[i];
                if (checkType.MItemID != "1" && checkType.MItemID != "3" && checkType.MItemID != "4") {
                    tempBindData.push(checkType);
                }
            }

            return tempBindData;
        }
        return bindData;
    },
    //判断核算维度是否已经选择
    checkTypeIsSelected: function (checkType, selfDom) {
        var isSelected = false;

        var checkTypeDomList = $(".accout-checktype");

        if (!checkTypeDomList || checkTypeDomList.length == 0) {
            return isSelected;
        }

        for (var i = 0; i < checkTypeDomList.length; i++) {
            var checkTypeDom = checkTypeDomList[i];
            if ($(checkTypeDom).is(selfDom)) {
                continue;
            }
            var currentCheckType = $(checkTypeDom).combobox("getValue");
            if (currentCheckType && checkType == currentCheckType) {
                isSelected = true;
                return isSelected;
            }
        }

        return isSelected;

    },
    //启用禁用事件绑定
    bindEnabledEvent: function (dom) {
        if (!dom) { return; }
        $(dom).unbind("click").bind("click", function () {
            var topDom = $(dom).parent().parent();

            var oldValue = $(dom).attr("value");
            if (oldValue === "1") {

                //判断是否有引用，有不允许禁用
                var url = "/BD/BDAccount/CheckTypeIsUsed";
                var param = {};
                if ($("#isLeaf").val() && !$("#hidAccountID").val()) {
                    param.accountId = AccountEdit.getSelectedNode().MItemID;
                } else {
                    param.accountId = $("#hidAccountID").val();
                }

                param.checkType = $(topDom).find(".accout-checktype").combobox("getValue");
                if (!param.checkType || param.checkType=="-1") {
                    var tips = HtmlLang.Write(LangModule.Acct, "NeedSelectAChecktype", "请先选择一个核算维度");
                    $.mDialog.alert(tips);
                    return;
                }


                $.mAjax.post(url, param, function (data) {
                    if (data && data.Success) {
                        $(dom).removeClass("check-type-enabled");
                        $(dom).addClass("check-type-disabled");
                        $(dom).attr("value", "0");
                        $(dom).parent().prev().text(HtmlLang.Write(LangModule.Acct, "Disabled", "禁用"));

                        //禁用掉所有的操作
                        $(topDom).find(".accout-checktype").combobox("disable");
                        $(topDom).find(".check-type-delete").removeClass("delete-enable").addClass("delete-disable")
                        .attr("title", HtmlLang.Write(LangModule.Acct, "AccountDeleteDisableTips", "请先启用核算维度再删除！"));
                        $(topDom).find(".account-checktype-enterstatus").combobox("disable");
                    } else {
                        //提示语句修改成多语言
                        //var msg = HtmlLang.Write(LangModule.Acct, "Theaccountexists", "该科目核算维度存在历史数据，不允许\r\n") + data.Message;
                        var msg = HtmlLang.Write(LangModule.Acct, "CheckTypeIsUsedCanNotInactive", "核算维度已被使用,不允许禁用!");
                        $.mDialog.alert(msg);
                    }
                }, true, true);
            } else {
                $(dom).removeClass("check-type-disabled");
                $(dom).addClass("check-type-enabled");
                $(dom).attr("value", "1");
                $(dom).parent().prev().text(HtmlLang.Write(LangModule.Acct, "Enabled", "启用"));

                if ($(topDom).hasClass("new-checktype")) {
                    $(topDom).find(".accout-checktype").combobox("enable");
                }
                $(topDom).find(".account-checktype-enterstatus").combobox("enable");

                $(topDom).find(".check-type-delete")
                    .removeClass("delete-disable")
                    .addClass("delete-enable")
                    .attr("title", HtmlLang.Write(LangModule.Acct, "AccountTypeDeleteTips", "有数据的核算维度不能删除！"));
            }
        });
    },
    bindInputStatus: function (dom) {
        $(dom).eq(0).unbind("click").bind("click", function () {
            var topDom = dom.parent().parent();
            var oldValue = $(topDom).find(".oldinputstatus").val();

            //必录改成非必录，需要提示用户确认
            if (oldValue === "true") {
                var url = "/BD/BDAccount/CheckTypeIsUsed";
                var param = {};
                if ($("#isLeaf").val() && !$("#hidAccountID").val()) {
                    param.accountId = AccountEdit.getSelectedNode().MItemID;
                } else {
                    param.accountId = $("#hidAccountID").val();
                }

                param.checkType = $(topDom).find(".accout-checktype").combobox("getValue");

                $.mAjax.post(url, param, function (data) {
                    if (data && !data.Success) {
                        var confirmMsg = HtmlLang.Write(LangModule.BD, "CheckTypeIsUsedContiunChange", "维度已经在以下数据中使用，是否更改为非选录?")+"\\n" + data.Message;

                        $.mDialog.confirm(confirmMsg, function () {
                            $(topDom).find(".input-status").eq(0).attr("checked", "checked");
                            $(topDom).find(".oldinputstatus").val("false");
                        }, function () {
                            $(topDom).find(".input-status").eq(1).attr("checked", "checked");
                        }, false, false, true);
                    }
                }, true, true);
            }
        });
    },
    bindDeleteEvent:function(dom){
        $(dom).off("click").on("click", function () {
            var topDom = dom.parent().parent();
            //如果新增，就不去后台校验了
            if ($(topDom).hasClass("new-checktype")) {
                $(topDom).remove();
                AccountCheckType.resetcheckTypeName();
            } else {
                var url = "/BD/BDAccount/CheckTypeIsUsed";
                var param = {};
                if ($("#isLeaf").val() && !$("#hidAccountID").val()) {
                    if (AccountEdit.isCombox) {
                        param.accountId = AccountEdit.getSelectedNode().MItemID;
                    } else {
                        param.accountId = $("#hidParentID").val();
                    }
                    
                } else {
                    param.accountId = $("#hidAccountID").val();
                }

                param.checkType = $(topDom).find(".accout-checktype").combobox("getValue");

                $.mAjax.post(url, param, function (data) {
                    if (data && !data.Success) {
                        //var tips = HtmlLang.Write(LangModule.Acct, "CheckTypeIsUsedNotAllowToDelete", "维度已经在以下数据中使用，不允许删除!\r\n") + data.Message;
                        var tips = HtmlLang.Write(LangModule.Acct, "CheckTypeIsUsedCanNotDelete", "核算维度已被使用,不允许删除!");
                        $.mDialog.alert(tips);
                    } else if (data && data.Success) {
                        $(topDom).remove();
                        AccountCheckType.resetcheckTypeName();
                    }
                }, true, true);
            }
        });
    },
    //维度重新排序
    resetcheckTypeName:function(){
        var checkTypeNameDomList = $(".account-checktype-name:visible");
        var prefix = HtmlLang.Write(LangModule.Acct, "CheckType", "核算维度");
        if (checkTypeNameDomList && checkTypeNameDomList.length > 0) {
            var length = checkTypeNameDomList.length;
            for (var i = 0; i < length; i++) {
                var checkTypeNameDom = checkTypeNameDomList[i];
                $(checkTypeNameDom).text(prefix + " " + (i + 1));
            }
        }
    },
    loadCheckGroupData: function (checkGroupData) {
        //加载核算维度值
        if (!checkGroupData) {
            return;
        }

        var checkTypeCount = 10;

        var checkTypeDomList = $(".account-checktype-item");
        var checkTypeDomIndex = 0;
        for (var i = 0; i < checkTypeCount; i++) {
            //获取维度值
            var checkTypeValue = AccountCheckType.getCheckTypeValueFromGroupData(checkGroupData, i);

            //如果为0，表示从来没有使用过
            if (checkTypeValue == 0 || checkTypeValue == null) {
                continue;
            }
            //如果页面有多余的维度，则直接复制
            if (checkTypeDomIndex < checkTypeDomList.length) {
                var checkTypeDom = checkTypeDomList[checkTypeDomIndex];
                AccountCheckType.setCheckGroupDomValue(checkTypeDom, i, checkTypeValue);
                checkTypeDomIndex++;
            } else {
                //新增一个
                var copyCheckTypeDom = AccountCheckType.getCheckTypeDom();
                $(".m-list-accountcheck").append(copyCheckTypeDom);
                //对Dom里面的内容进行一些处理
                var comboboxDom = $(copyCheckTypeDom).find(".accout-checktype");

                var isCurrentAccount = AccountEdit.isCurrentAccount(AccountEdit.account);;

                AccountCheckType.initAccountCheckCombobox(comboboxDom, isCurrentAccount);

                var checkTypeEnterTypeCombobox = $(copyCheckTypeDom).find(".account-checktype-enterstatus");
                AccountCheckType.initAccountCheckEnterTypeCombobox(checkTypeEnterTypeCombobox);

                var deleteDom = $(copyCheckTypeDom).find(".check-type-delete");
                AccountCheckType.bindDeleteEvent(deleteDom);

                AccountCheckType.setCheckGroupDomValue(copyCheckTypeDom, i, checkTypeValue);
            }
        }

    },
    //创建一个核算维度编辑dom
    getCheckTypeDom: function (checkTypeId, checkTypeValue) {
        var domIndex = AccountCheckType.getCheckTypeDomIndex();

        if (domIndex > 10) {
            var tips = HtmlLang.Write(LangModule.Acct, "TooManyCheckType", "最多允许10个核算维度！");
            $.mDialog.alert(tips);
            return;
        }

        var templateDom = $(".account-check-template");

        var copyDom = templateDom.clone();
        $(copyDom).css("display", "block").removeClass("account-check-template").addClass("account-checktype-item");



        //对Dom里面的内容进行一些处理
        var comboboxDom = $(copyDom).find(".accout-checktype-template");
        comboboxDom.removeClass("accout-checktype-template").addClass("accout-checktype");

        //选录，必录下拉框
        var requiredDom = $(copyDom).find(".account-checktype-enterstatus");
        AccountCheckType.bindInputStatus(requiredDom);

        //绑定启用禁用事件
        var enabledDom = $(copyDom).find(".enable-stauts");

        AccountCheckType.bindEnabledEvent(enabledDom);


        $(copyDom).find(".account-checktype-name").text(HtmlLang.Write(LangModule.Acct, "CheckType", "核算维度") + " " + domIndex);

        if (domIndex == 10) {
            $(copyDom).find(".account-checktype-name").css("margin-right", "9px");
        }

        //复制值
        if (checkTypeId) {
            comboboxDom.combobox("setValue", checkTypeId);
        }

        if (checkTypeValue) {
            AccountCheckType.setCheckGroupDomValue(copyDom, checkTypeValue);
        }

        return copyDom;
    },
    setCheckGroupDomValue: function (copyDom, checkType, checkTypeValue) {

        var required = checkTypeValue == 2 || checkTypeValue == 4;
        var enabled = checkTypeValue == 1 || checkTypeValue == 2;

        var comboboxDom = $(copyDom).find(".accout-checktype");

        $(comboboxDom).combobox("setValue", checkType);
        //已经增加的维度不能删除
        

        $(copyDom).find(".oldinputstatus").val(required);

        var requiredDom = $(copyDom).find(".account-checktype-enterstatus");
        if (required) {
            $(requiredDom).combobox("setValue","1");
        } else {
            $(requiredDom).combobox("setValue", "0");
        }

        var enabledDom = $(copyDom).find(".enable-stauts");
        if (!enabled) {
            $(enabledDom).removeClass("check-type-enabled");
            $(enabledDom).addClass("check-type-disabled");
            $(enabledDom).attr("value", "0");
            $(copyDom).find(".check-type-enabledtext").text(HtmlLang.Write(LangModule.Acct, "Disabled", "禁用"));

            //$(copyDom).find(".accout-checktype").combobox("disable");

            $(copyDom).find("input[type='radio']").attr("disabled", "disabled");
        } else {
            $(enabledDom).removeClass("check-type-disabled");
            $(enabledDom).addClass("check-type-enabled");
            $(enabledDom).attr("value", "1");
            $(copyDom).find(".check-type-enabledtext").text(HtmlLang.Write(LangModule.Acct, "Enabled", "启用"));
            $(copyDom).find("input[type='radio']").removeAttr("disabled");
        }

        //如果是系统原有的，不允许编辑核算维度类型
        if (checkTypeValue) {
            $(comboboxDom).combobox("disable");

            $(copyDom).removeClass("new-checktype");
        }

    },
    //获取当前页面框维度的长度
    getCheckTypeDomIndex: function () {
        var domLength = $(".account-checktype-item").length;

        return domLength + 1;

    },
    //获取核算维度类型值
    getCheckTypeValueFromGroupData: function (checkTypeData, matchIndex, matchKey) {
        var loopCount = -1;
        for (var p in checkTypeData) {
            loopCount++;

            if (matchIndex && loopCount != matchIndex) {
                continue;
            }

            if (matchKey && matchKey != p) {
                continue
            }

            if (typeof (checkTypeData[p]) != "function") {
                return checkTypeData[p]
            }

        }
        return null;
    },
    //获取一个null的checkgroupmodel对象
    getNullAccountCheckGroup: function () {
        var obj = {};

        obj.MContactID = 0;
        obj.MEmployeeID = 0;
        obj.MMerItemID = 0;
        obj.MExpItemID = 0;
        obj.MPaItemID = 0;
        obj.MTrackItem1 = 0;
        obj.MTrackItem2 = 0;
        obj.MTrackItem3 = 0;
        obj.MTrackItem4 = 0;
        obj.MTrackItem5 = 0;

        return obj;
    },
    getCheckEnterType: function () {
        var checkEnterTypeList = new Array();

        var tips = { id: "", text: HtmlLang.Write(LangModule.Acct, "EmptyEnterType", "请选择录入类型") };
        var optional = { id: "0", text: HtmlLang.Write(LangModule.Acct, "AccountCheckOptional", "可选") };
        var required = { id: "1", text: HtmlLang.Write(LangModule.Acct, "AccountCheckRequired", "必录") };

        checkEnterTypeList.push(tips);
        checkEnterTypeList.push(optional);
        checkEnterTypeList.push(required);

        return checkEnterTypeList;

    }
}