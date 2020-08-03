
var AccountEdit = {
    isCombox: $("#isCombox").val(),
    accountStandard: $("#hideAccountStandard").val(),
    account: null,
    init: function () {
        AccountCheckType.getCheckTypeList(function (data) {
            AccountEdit.initAction();
            if (!AccountEdit.isCombox) {
                AccountEdit.initForm(function (accountData) {
                    AccountEdit.initCheckType(accountData);
                });
            } else if (AccountEdit.accountStandard == 3) {
                $("#sltAccountType").removeAttr("disabled");
                $("#sltAccountType").combobox("enable");
                $("input[name='MDC']").removeAttr("disabled");
                $("#tbxNumber").width(196);
            } else {
                $("#tbxNumber").width(196);
            }

        });
    },
    initAction: function () {
        $("#btnSave").click(function () {
            AccountEdit.saveAccount();
        });


        $(".m-lang-btn").click(function () {
            if (AccountEdit.account) {
                $("#0x0009").removeAttr("disabled");
            }

            if (!AccountEdit.account.MIsSys) {
                $("#0x7804").removeAttr("disabled");
            }

        });

        if (AccountEdit.isCombox) {
            $("#tbxParentName").combotree({
                onSelect: function (node) {
                    $("#hidParentID").val(node.id);

                    if (!node.children || node.children.length == 0) {
                        $("#isLeaf").val(true);
                    } else {
                        $("#isLeaf").val(false);
                    }

                    AccountEdit.initForm(function (accountData) {
                        AccountEdit.initCheckType(accountData);
                    });

                    $("#sltAccountType").combobox("disable");
                },
                onChange: function (newValue, oldValue) {
                    if (newValue == "" && AccountEdit.accountStandard == 3) {
                        $("#sltAccountType").combobox("enable");
                    }
                },
            });

            $("#tabsAccountEdit").tabs({
                onSelect: function (title, index) {
                    if (index == 1) {
                        var t = $('#tbxParentName').combotree('tree');
                        var n = t.tree('getSelected');
                        if (!n || !n.text) {
                            //如果没有选择项,提示用选择
                            var tips = HtmlLang.Write(LangModule.Acct, "NoSelectParentAccount", "没有选择父级科目，请先选择");
                            $.mDialog.alert(tips);

                            $(this).tabs("select", 0);
                        }

                    }
                }
            });
        }

        AccountCheckType.initAction();
    },
    initForm: function (callback) {
        var id = $("#hidAccountID").val();
        var parentId = $("#hidParentID").val();
        $("body").mFormGet({
            url: "/BD/BDAccount/GetAccount?MItemID=" + id + "&MParentID=" + parentId,
            callback: function (data) {
                var code = data ? data.MNumber : "";
                var mdc = data && data.MDC != 0 ? data.MDC : $("#hideMDC").val();
                var id = data.MItemID;
                AccountEdit.account = data;
                //去父节点的编码
                var array = code.split(".");
                if (array.length > 0) {
                    var firstName = "";
                    for (var i = 0; i < array.length - 1; i++) {
                        firstName += array[i] + ".";
                    }
                    var secondName = array[array.length - 1];
                    $("#lblParentId").text(firstName);
                    //计算tbxNumber长度
                    var domNumber = $("#tbxNumber");

                    var offsetValue = 3;
                    var maginBottomValue = 4;

                    if (Megi.isSafari()) {
                        offsetValue = 3;
                        maginBottomValue = 2;
                    } else if (Megi.isIE()) {
                        offsetValue = 4;
                        maginBottomValue = 3;
                    }
                    var width = domNumber.parent().width() - $("#lblParentId").width() - offsetValue;
                    domNumber.width(width);
                    domNumber.val(secondName);
                    domNumber.css("margin-bottom", maginBottomValue + "px");
                }

                $("#sltAccountType").combobox("setValue", data.MAccountTypeID);
                $("#selIsCheckForCurrency").combobox("setValue", data.MIsCheckForCurrency + "");

                if (mdc >= 0) {
                    $("input[name='MDC']:eq(0)").attr("checked", 'checked');
                } else if (mdc < 0) {
                    $("input[name='MDC']:eq(1)").attr("checked", 'checked');
                }
                //新增放开科目方向的选择或者非预设科目并且允许编辑
                if (!id || (!data.MIsSys && data.MIsAllowEdit)) {
                    $("input[name='MDC']").removeAttr("disabled");
                }

                $("input[name='MIsSys']").val(data.MIsSys);
                $("input[name='MAccountGroupID']").val(data.MAccountGroupID);
                $("input[name='MAccountTableID']").val(data.MAccountTableID);
                $("input[name='MCode']").val(data.MCode);

                if (data.MParentName) {
                    $("#tbxParentName").val(data.MParentName);
                }
                //自定义科目的编辑逻辑
                if (AccountEdit.accountStandard == 3) {

                    $("#sltAccountType").removeAttr("disabled");
                    if ((data.MItemID && data.MIsCheckForCurrency) || !data.MIsAllowEdit) {
                        $("#selIsCheckForCurrency").combobox("disable", "true");
                        $("input[name='MDC']").attr("disabled", "disabled");
                    } else {
                        $("input[name='MDC']").removeAttr("disabled");
                    }

                } else {
                    //如果是系統预设项，屏蔽一些按钮
                    if (data.MIsSys) {
                        $("#sltAccountType").combobox("disable");
                        $("#tbxNumber").attr("disabled", "disabled");
                        $("#txtName").attr("disabled", "disabled");

                        $("#txtName").data("disable", true);
                    } else {
                        $("#txtName").data("disable", false);
                    }

                    if (!data.MIsAllowEdit) {
                        $("#tbxNumber").attr("disabled", "disabled");

                        if (data.MIsSys) {
                            $("#txtName").attr("disabled", "disabled");
                        }
                    } else {
                        $("#txtName").data("disable", false);
                        $("#selIsCheckForCurrency").combobox("enable");
                    }

                    //外币核算能够更改的逻辑 如果科目有过引用并且已经是外币核算了，不能进行更改；现金和银行账号的核算维度不能够进行更改
                    if ((data.MItemID && data.MIsCheckForCurrency && !data.MIsAllowEdit)
                        || (data.MCode && (data.MCode.indexOf("1001") == 0 || data.MCode.indexOf("1002") == 0))) {
                        $("#selIsCheckForCurrency").combobox("disable", "true");
                    }
                }

                //在加载核算维度信息
                if (callback && $.isFunction(callback)) {
                    callback(data);
                }
            }
        });
        var name = LangKey.Name;
    },
    initCheckType: function (data) {
        //如果是新增，也是可以修改核算维度
        var isLeaf = $("#isLeaf").val().toLowerCase() == "true" || AccountEdit.isCombox;

        //如果是父节点，不显示核算维度
        if (!isLeaf) {
            $("#tabsAccountEdit").tabs("disableTab", 1);
        } else {
            $("#tabsAccountEdit").tabs("enableTab", 1);
        }

        var isCurrentAccount = AccountEdit.isCurrentAccount(data);

        $(".accout-checktype").each(function () {
            AccountCheckType.initAccountCheckCombobox($(this), isCurrentAccount);
        })

        //选录必录类型
        $(".account-checktype-enterstatus", ".account-checktype-item").each(function () {
            AccountCheckType.initAccountCheckEnterTypeCombobox($(this));
        });

        //加载核算维度信息
        AccountCheckType.loadCheckGroupData(data.MCheckGroupModel);

        //四个往来科目必须设置联系人作为核算维度
        var firstCheckTypeDiv = $(".account-checktype-item")[0];
        if (isCurrentAccount) {
            $(".enable-stauts", firstCheckTypeDiv).hide();

            $(".check-type-enabledtext", firstCheckTypeDiv).text(HtmlLang.Write(LangModule.Acct, "Enabled", "启用"));
            $(".check-type-delete", firstCheckTypeDiv).hide();
            //禁用掉所有的操作
            $(".accout-checktype", firstCheckTypeDiv).combobox("setValue", "0").combobox("disable");
            $(".account-checktype-enterstatus", firstCheckTypeDiv).combobox("setValue", "1").combobox("disable");

        } else if (AccountEdit.isCombox) {
            $(".enable-stauts", firstCheckTypeDiv).show();
            $(".check-type-delete", firstCheckTypeDiv).show();
            $(".check-type-enabledtext", firstCheckTypeDiv).text(HtmlLang.Write(LangModule.Acct, "Enabled", "启用"));

            $(".account-checktype-enterstatus", firstCheckTypeDiv).combobox("enable");
        }
    },
    //是否往来科目
    isCurrentAccount: function (data) {
        var code = data.MCode ? data.MCode : data.MParentCode;

        if (code.indexOf("1122") == 0 || code.indexOf("1123") == 0 ||
           code.indexOf("2202") == 0 || code.indexOf("2203") == 0) {
            return true;
        }

        return false;
    },
    initCurrency: function (value, cyIds) {
        if (value == 1) {
            //进行外币核算，加载外币
            $("#divFieldCy").show();
            AccountEdit.loadAccountCurrency(cyIds);
        } else {
            $("#divCy").empty();
            $("#divFieldCy").hide();
        }
    },
    saveAccount: function () {
        if ($(".m-form-key").val() == "") {
            $("input[name='MIsActive']").val("true");
        }

        //保存的时候如果是叶子节点，给出提示
        var isLeaf = $("#isLeaf").val();

        if (AccountEdit.isCombox && AccountEdit.accountStandard != 3) {
            var tree = $('#tbxParentName').combotree('tree');
            var node = tree.tree('getSelected');

            if (!node) {
                var tips = HtmlLang.Write(LangModule.Acct, "NotSelectParent", "请先选择上级科目！");
                $.mDialog.message(tips);
                return;
            }

            var children = tree.tree("getChildren", node.target);

            if (children.length == 0) {
                isLeaf = true;
            } else {
                isLeaf = false;
            }
        }

        var accountNumber = $("#tbxNumber").val();

        if (!$("#tbxNumber").validatebox("isValid")) {
            $("#tbxNumber").focus();
            $("#tbxNumber").validatebox("validate");
            return;
        }

        var re = /^[0-9]+.?[0-9]*$/;

        if (!re.test(accountNumber)) {
            var tips = HtmlLang.Write(LangModule.Acct, "AccountNumberOnlyNumber", "科目代码只能填写数字!");
            $.mDialog.alert(tips);
            return false;
        }

        if (accountNumber.length > 4) {
            var tips = HtmlLang.Write(LangModule.Acct, "AccountNumberTooLength", "科目代码最大4位!");
            $.mDialog.alert(tips);
            return false;
        }


        var fullName = $("#lblParentId").text() + $("#tbxNumber").val();

        var obj = {};
        obj.MItemID = $("#hidAccountID").val();
        obj.MParentID = $("#hidParentID").val() ? $("#hidParentID").val() : "0";
        obj.MDC = $("input[name='MDC']:checked").val();
        obj.MAccountGroupID = $("input[name='MAccountGroupID']").val();
        obj.MAccountTableID = $("input[name='MAccountTableID']").val();
        obj.IsCanRelateContact = $("#hideIsCanRelateContact").val();
        obj.MAccountTypeID = $("#sltAccountType").combobox("getValue");

        obj.MCode = $("input[name='MCode']").val();
        obj.MNumber = fullName;
        obj.MName = $("#txtName").val();

        if (!obj.MName) {
            $("#tabsAccountEdit").tabs("select", 0);
            $("#txtName").focus();
            $("#txtName").validatebox("validate");
            return;
        }

        obj.MDesc = $("#txtDescription").val();
        obj.MIsCheckForCurrency = $("#selIsCheckForCurrency").combobox("getValue");
        obj.MIsSys = $("input[name='MIsSys']").val();
        obj.MCyID = "";


        $("input[name='MCyID']").each(function () {
            obj.MCyID += $(this).val() + ",";
        });

        if (obj.MCyID) {
            obj.MCyID = obj.MCyID.substring(0, obj.MCyID.length - 1);
        }

        obj.MCheckGroupModel = AccountCheckType.getCheckGroupValueObject();

        if (isLeaf && !$("#hidAccountID").val()) {
            mAjax.post("/BD/BDAccount/AddAcountCheck", { model: obj }, function (msg) {
                if (msg && msg.Success) {

                    //如果有消息，就提示
                    if (msg.Message) {
                        $.mDialog.alert(msg.Message);
                        return;
                    }

                    var message = HtmlLang.Write(LangModule.Acct,"confirmAddAccount", "You are adding first lower level account,system will put all data transferred to the new account.Are you continue?");
                    $.mDialog.confirm(message, {
                        callback: function () {
                            AccountEdit.requestSaveAccount(obj);
                        }
                    })
                } else {
                    AccountEdit.requestSaveAccount(obj);
                }
            });

        } else {
            //直接保存
            AccountEdit.requestSaveAccount(obj);
        }

    },
    requestSaveAccount: function (obj) {
        $("body").mFormSubmit({
            url: "/BD/BDAccount/SaveAccount", param: { model: obj }, callback: function (msg) {
                var successMsg = $("#hidAccountID").val().length > 0 ? HtmlLang.Write(LangModule.Acct, "AccountUpdated", "Account updated") + ": " + $("#txtName").val() :
                    HtmlLang.Write(LangModule.Acct, "AccountAdded", "Account added") + ": " + $("#txtName").val();
                if (msg.Success) {
                    $.mDialog.message(successMsg);

                    var obj = JSON.parse(msg.ObjectID);

                    $.mDialog.close(obj);
                } else {
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    },
    //加载科目选择的外币
    loadAccountCurrency: function (cyIds) {
        var accountId = $("#hidAccountID").val();
        var url = "/BD/Currency/GetBDCurrencyList";
        mAjax.post(url, { isIncludeBase: true }, function (msg) {
            if (msg) {
                var cyList = msg;
                var checkBox = "";
                for (var i = 0; i < cyList.length; i++) {
                    //4个换行
                    if (i != 0 && i % 4 == 0) {
                        checkBox += "</br>";
                    }
                    if (AccountEdit.isSelectCurrency(cyList[i].MCurrencyID, cyIds)) {
                        checkBox += '<input style="padding-right:10px" name="MCyID" type="checkbox" value="' + cyList[i].MCurrencyID + '" checked />' + cyList[i].MCurrencyID;
                    } else {
                        checkBox += '<input name="MCyID" type="checkbox" value="' + cyList[i].MCurrencyID + '" />' + cyList[i].MCurrencyID;
                    }
                }
                $("#divCy").append(checkBox);
            }
        });
    },
    //cyId_1是否需要选择
    isSelectCurrency: function (cyId_1, cyIds) {
        var result = false;
        if (cyIds) {
            var checkedCys = cyIds.split(',');
            for (var j = 0; j < checkedCys.length; j++) {
                if (cyId_1 == checkedCys[j]) {
                    result = true;
                    break;
                }
            }
        }
        return result;
    },
    //获取光标位置
    getCursorPos: function (selector) {
        if (navigator.userAgent.indexOf("MSIE") > -1) { // IE
            var range = document.selection.createRange();
            range.text = '';
            range.setEndPoint('StartToStart', selector.createTextRange());
            return range.text.length;
        } else {
            return selector.selectionStart;
        }
    },
    //设置光标位置
    setCursorPos: function (selector, pos) {
        if (navigator.userAgent.indexOf("MSIE") > -1) {
            var range = document.selection.createRange();
            var textRange = selector.createTextRange();
            textRange.moveStart('character', pos);
            textRange.collapse();
            textRange.select();
        } else {
            selector.setSelectionRange(n, n);
        }
    },
    getSelectedNode: function () {
        if (AccountEdit.isCombox) {
            var tree = $('#tbxParentName').combotree('tree');
            var node = tree.tree('getSelected');
            return node;
        }

        var node = {};
        node.MItemID = $("#hidParentID").val();
        node.text = $('#tbxParentName').text();

        return node;
    }
}
$(document).ready(function () {
    AccountEdit.init();
});
