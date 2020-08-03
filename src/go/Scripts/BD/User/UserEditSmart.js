var UserEdit = {
    IsLoad: false,
    IsStrikePosition: true,
    lastEventTime: null,
    //原有邮箱
    oldEmail: null,
    init: function () {
        //初始化按钮操作
        UserEdit.clickAction();
        //初始化用户权限表格
        UserEdit.showUserRole();
        UserEdit.initUserRoleTable();
    },
    //获取岗位_角色对应的权限ID列表，每个权限ID以逗号（“,”）分隔，每个权限ID是由（权限分组ID_权限）组成，比如：销售查看权限ID为：Sales_MView ，每个权限ID对应一个复选框ID
    //现在暂时只有这四种权限（MView：查看， MChange：修改， MApprove：审核， MExport：导出）
    getPositionRole: function (position, role) {
        var pr = position + "_" + role;
        switch (pr) {
            case "Finance_Admin":
            case "SysManage_Admin":
            case "Other_Admin":
            case "All_Admin":
                //岗位为：系统管理员(拥有所有权限)
            case "SysManage_Basic":
            case "SysManage_DataEntry":
            case "SysManage_Standard":
            case "SysManage_Advance":
            case "SysManage_Admin":
                return "Bank_MView, Bank_MChange, Bank_MApprove, Bank_MExport,\
                Bank_Reconciliation_MView, Bank_Reconciliation_MChange, Bank_Reconciliation_MApprove, Bank_Reconciliation_MExport,\
                Bank_ExceptReconciliation_MView, Bank_ExceptReconciliation_MChange, Bank_ExceptReconciliation_MApprove, Bank_ExceptReconciliation_MExport,\
                Contacts_MView, Contacts_MChange, Contacts_MApprove, Contacts_MExport,\
                Attachment_MView, Attachment_MChange, Attachment_MApprove, Attachment_MExport,\
                General_Ledger_MView, General_Ledger_MChange, General_Ledger_MApprove, General_Ledger_MExport,\
                Fapiao_MView, Fapiao_MChange, Fapiao_MApprove, Fapiao_MExport,\
                Sales_Fapiao_MView, Sales_Fapiao_MChange, Sales_Fapiao_MApprove, Sales_Fapiao_MExport,\
                Purchases_Fapiao_MView, Purchases_Fapiao_MChange, Purchases_Fapiao_MApprove, Purchases_Fapiao_MExport,\
                Reports_MView, Reports_MChange, Reports_MApprove, Reports_MExport,\
                Other_Reports_MView, Other_Reports_MChange, Other_Reports_MApprove, Other_Reports_MExport,\
                Edit_Settings_MView, Edit_Settings_MChange, Edit_Settings_MApprove, Edit_Settings_MExport,\
                System_Settings_MView, System_Settings_MChange, System_Settings_MApprove, System_Settings_MExport,\
                Excel_Plus_Download_MExport,\
                Migration_Tool_Download_MExport,Fixed_Assets_MView,Fixed_Assets_MChange,Fixed_Assets_MApprove,Fixed_Assets_MExport,\
                Fixed_Assets_Reports_MView,Fixed_Assets_Reports_MChange,Fixed_Assets_Reports_MApprove,Fixed_Assets_Reports_MExport";
            case "Finance_Basic":
                return "Bank_ExceptReconciliation_MView, Contacts_MView, Attachment_MView, General_Ledger_MView, Reports_MView, Other_Reports_MView,Fixed_Assets_MView,Fixed_Assets_Reports_MView";
            case "Finance_DataEntry":
                return "Bank_MView, Bank_MChange,\
                Bank_Reconciliation_MView, Bank_Reconciliation_MChange,\
                Bank_ExceptReconciliation_MView, Bank_ExceptReconciliation_MChange,\
                Contacts_MView, Contacts_MChange,\
                Attachment_MView, Attachment_MChange,\
                General_Ledger_MView, General_Ledger_MChange,\
                Fapiao_MView, Fapiao_MChange,\
                Sales_Fapiao_MView, Sales_Fapiao_MChange,\
                Purchases_Fapiao_MView, Purchases_Fapiao_MChange,\
                Reports_MView, Reports_MChange,\
                Fapiao_MView, Fapiao_MChange,\
                Other_Reports_MView, Other_Reports_MChange,\
                Edit_Settings_MView, Edit_Settings_MChange,Fixed_Assets_MView,Fixed_Assets_MChange,\
                Fixed_Assets_Reports_MView,Fixed_Assets_Reports_MChange";
            case "Finance_Standard":
                //财务 + 高级用户
            case "Finance_Advance":
                //所有岗位 + 高级用户
            case "All_Advance":
                return "Bank_MView, Bank_MChange, Bank_MApprove, Bank_MExport,\
                Bank_Reconciliation_MView, Bank_Reconciliation_MChange, Bank_Reconciliation_MApprove, Bank_Reconciliation_MExport,\
                Bank_ExceptReconciliation_MView, Bank_ExceptReconciliation_MChange, Bank_ExceptReconciliation_MApprove, Bank_ExceptReconciliation_MExport,\
                Contacts_MView, Contacts_MChange, Contacts_MApprove, Contacts_MExport,\
                Attachment_MView, Attachment_MChange, Attachment_MApprove, Attachment_MExport,\
                General_Ledger_MView, General_Ledger_MChange, General_Ledger_MApprove, General_Ledger_MExport,\
                Fapiao_MView, Fapiao_MChange, Fapiao_MApprove, Fapiao_MExport,\
                Sales_Fapiao_MView, Sales_Fapiao_MChange, Sales_Fapiao_MApprove, Sales_Fapiao_MExport,\
                Purchases_Fapiao_MView, Purchases_Fapiao_MChange, Purchases_Fapiao_MApprove, Purchases_Fapiao_MExport,\
                Reports_MView, Reports_MChange, Reports_MApprove, Reports_MExport,\
                Other_Reports_MView, Other_Reports_MChange, Other_Reports_MApprove, Other_Reports_MExport,\
                Edit_Settings_MView, Edit_Settings_MChange, Edit_Settings_MApprove, Edit_Settings_MExport,Fixed_Assets_MView,Fixed_Assets_MChange,Fixed_Assets_MApprove,Fixed_Assets_MExport,\
                Fixed_Assets_Reports_MView,Fixed_Assets_Reports_MChange,Fixed_Assets_Reports_MApprove,Fixed_Assets_Reports_MExport";
                //其他
            case "Other_Basic":
                return "Contacts_MView, Attachment_MView";
            case "Other_DataEntry":
            case "Other_Standard":
            case "Other_Advance":
                return "Attachment_MView, Contacts_MChange, Attachment_MChange";
                //所有岗位
            case "All_Basic":
                return "Bank_MView, Bank_Reconciliation_MView, Bank_ExceptReconciliation_MView, Contacts_MView, Attachment_MView, General_Ledger_MView, Reports_MView, Edit_Settings_MView,Fixed_Assets_MView,Other_Reports_MView,Fixed_Assets_Reports_MView";
            case "All_DataEntry":
                return "Bank_MView, Bank_MChange,\
                Bank_Reconciliation_MView, Bank_Reconciliation_MChange,\
                Bank_ExceptReconciliation_MView, Bank_ExceptReconciliation_MChange,\
                Contacts_MView, Contacts_MChange,\
                Attachment_MView, Attachment_MChange,\
                General_Ledger_MView, General_Ledger_MChange,\
		        Fapiao_MView, Fapiao_MChange,\
		        Sales_Fapiao_MView, Sales_Fapiao_MChange,\
                Purchases_Fapiao_MView, Purchases_Fapiao_MChange,\
                Edit_Settings_MView, Edit_Settings_MChange,\
		        Fixed_Assets_MView,Fixed_Assets_MChange,\
		        Fixed_Assets_Reports_MView,Fixed_Assets_Reports_MChange";
            case "All_Standard":
                return "Bank_MView, Bank_MChange, Bank_MApprove,\
                Bank_Reconciliation_MView, Bank_Reconciliation_MChange, Bank_Reconciliation_MApprove,\
                Bank_ExceptReconciliation_MView, Bank_ExceptReconciliation_MChange, Bank_ExceptReconciliation_MApprove,\
                Contacts_MView, Contacts_MChange, Contacts_MApprove,\
                Attachment_MView, Attachment_MChange, Attachment_MApprove,\
                General_Ledger_MView, General_Ledger_MChange, General_Ledger_MApprove,\
		        Fapiao_MView, Fapiao_MChange, Fapiao_MApprove,\
                Sales_Fapiao_MView, Sales_Fapiao_MChange, Sales_Fapiao_MApprove,\
                Purchases_Fapiao_MView, Purchases_Fapiao_MChange, Purchases_Fapiao_MApprove,\
                Edit_Settings_MView, Edit_Settings_MChange, Edit_Settings_MApprove,Fixed_Assets_MView,Fixed_Assets_MChange,Fixed_Assets_MApprove,\
                Fixed_Assets_Reports_MView,Fixed_Assets_Reports_MChange,Fixed_Assets_Reports_MApprove";

            default:
                break;
        }
    },
    //显示用户权限列表
    showUserRole: function () {
        //选择岗位时加载对应的权限
        $("#ddlPosition").combotree({
            data: [
                { id: "Finance", text: HtmlLang.Write(LangModule.User, "Finance", "Finance") },
                { id: "SysManage", text: HtmlLang.Write(LangModule.User, "SysManage", "SysManage") },
                { id: "Other", text: HtmlLang.Write(LangModule.User, "Other", "Other") },
                { id: "All", text: HtmlLang.Write(LangModule.User, "All", "All") }
            ],
            onChange: function (record) {
                var position_strs = $.trim($("#ddlPosition").combotree("getValues"));


                //如果岗位 不是管理员 或者 是所有岗位，则角色默认为：基本用户角色
                if (position_strs.indexOf("SysManage") == -1 || position_strs.indexOf("All") > -1) {
                    if (UserEdit.IsStrikePosition) {
                        $("#ddlRole").combobox("setValue", "Basic");
                    }
                }
                var role = $("#ddlRole").combobox("getValue");

                if (UserEdit.IsLoad) {
                    UserEdit.loadUserRoleList();
                }
            },
            onBeforeLoad: function () {
                //禁用验证
                $("#ddlPosition").combotree("disableValidation");
            },
            onShowPanel: function () {
                //启用验证
                $("#ddlPosition").combotree("enableValidation");
            }
        });

        //选择角色时加载对应的权限
        $("#ddlRole").combobox({
            valueField: 'id', textField: 'text',
            data: [
                { id: "Basic", text: HtmlLang.Write(LangModule.User, "Basic", "Basic(Read Only)") },
                { id: "DataEntry", text: HtmlLang.Write(LangModule.User, "DataEntry", "Data Entry") },
                { id: "Standard", text: HtmlLang.Write(LangModule.User, "Standard", "Standard") },
                { id: "Advance", text: HtmlLang.Write(LangModule.User, "Advance", "Advance(Including Advisor)") },
                { id: "Admin", text: HtmlLang.Write(LangModule.User, "Admin", "Admin") }
            ],
            onSelect: function (record) {
                //如果角色选择 管理员 则岗位自动选择所有岗位
                if (record.id == "Admin") {
                    UserEdit.IsStrikePosition = false;
                    $("#ddlPosition").combotree("setValue", "All");
                    UserEdit.IsStrikePosition = true;
                }
                var role = record.id;
                UserEdit.loadUserRoleList();
            },
            onBeforeLoad: function () {
                //禁用验证
                $("#ddlRole").combobox("disableValidation");
            },
            onShowPanel: function () {
                //启用验证
                $("#ddlRole").combobox("enableValidation");
            }
        });
    },
    //加载对应的权限
    loadUserRoleList: function () {
        //如果当前选择的岗位为空，则取消禁用所有复选框
        var position_strs = $.trim($("#ddlPosition").combotree("getValues"));
        if (!position_strs) {
            UserEdit.cancelAllSelect();
            return;
        }
        //岗位保值
        $("#hidMPosition").val(position_strs);

        //系统管理
        if (position_strs.indexOf("SysManage") > -1) {
            //自动选择管理员角色
            $("#ddlRole").combobox("setValue", "Admin");
        }
        //角色
        var role = $.trim($("#ddlRole").combobox("getValue"));

        //管理员
        if (role == "Admin" && position_strs.indexOf("SysManage") == -1) {
            //自动选择所有岗位
            $("#ddlPosition").combotree("setValue", "All");
        }

        //先取消所有选中
        UserEdit.cancelAllSelect();
        //循环选中的岗位
        var positions = position_strs.split(',');
        for (var i = 0; i < positions.length; i++) {
            //根据（岗位_角色）勾选相应的权限
            UserEdit.selectRole(UserEdit.getPositionRole(positions[i], role));
        }
        //财务
        if (position_strs.indexOf("Finance") > -1) {
            if (role == "Standard" || role == "Advance") {
                //锁定指定的复选框
                UserEdit.lockRole("System_Settings_MView, System_Settings_MChange, System_Settings_MApprove, System_Settings_MExport");
            }
        }
        //禁用Excel Plus下载权限项
        UserEdit.lockDownLoadPermission("Excel_Plus_Download");
        UserEdit.lockDownLoadPermission("Migration_Tool_Download");
        UserEdit.lockAttachmentPermission();
    },
    //Excel工具下载,禁用查询、修改、审核权限
    lockDownLoadPermission: function (id) {
        var lockPermissionItem = "{0}_MView, {0}_MChange, {0}_MApprove".replace(/\{0\}/g, id);
        UserEdit.lockRole(lockPermissionItem, true);
    },
    //附件：禁用审核、导出权限
    lockAttachmentPermission: function () {
        var lockPermissionItem = "Attachment_MApprove, Attachment_MExport";
        UserEdit.lockRole(lockPermissionItem, true);
    },
    //选中指定的复选框
    selectRole: function (indexs) {
        //再设置需要选中的
        var indexss = indexs.split(',');
        for (var i = 0; i < indexss.length; i++) {
            $("#" + $.trim(indexss[i])).attr("checked", "true").attr("disabled", false);
        }
    },
    //取消所有复选框的选中
    cancelAllSelect: function () {
        array = $("#divUserRole input[type='checkbox']");
        array.each(function (i) {
            array.eq(i).removeAttr("checked");
            if (!UserEdit.isExportItem(this.id)) {
                array.eq(i).attr("disabled", true);
            }
        });
    },
    //是否为导出项
    isExportItem: function (str) {
        return str.lastIndexOf("_MExport") != -1;
    },
    //取消指定的复选框的选中
    cancelSelect: function (indexs) {
        var indexss = indexs.split(',');
        for (var i = 0; i < indexss.length; i++) {
            $("#" + $.trim(indexss[i])).removeAttr("checked");
        }
    },
    //锁定指定的复选框
    lockRole: function (indexs, cancelSelect) {
        var indexss = indexs.split(',');
        for (var i = 0; i < indexss.length; i++) {
            $("#" + $.trim(indexss[i])).attr("disabled", true);
            if (cancelSelect) {
                $("#" + $.trim(indexss[i])).removeAttr("checked");
            }
        }
    },
    //解锁指定的复选框
    deblocking: function (indexs) {
        var indexss = indexs.split(',');
        for (var i = 0; i < indexss.length; i++) {
            $("#" + $.trim(indexss[i])).attr("disabled", false);
        }
    },
    //获取用户ID
    getItemId: function () {
        return $.trim($("#ItemId").val());
    },
    //获取用户权限组列表
    getGroups: function () {
        var groups = new Array();
        //groups.push(UserEdit.getGroup("Bank"));//银行
        groups.push(UserEdit.getGroup("Bank_Reconciliation"));//银行勾对
        groups.push(UserEdit.getGroup("Bank_ExceptReconciliation"));//银行(不包括银行勾对）
        groups.push(UserEdit.getGroup("Contacts"));//联系人
        groups.push(UserEdit.getGroup("Attachment"));//附件
        groups.push(UserEdit.getGroup("Fixed_Assets"));//固定资产
        groups.push(UserEdit.getGroup("General_Ledger"));//总账
        groups.push(UserEdit.getGroup("Sales_Fapiao"));//销售发票
        groups.push(UserEdit.getGroup("Purchases_Fapiao"));//采购发票
        //groups.push(UserEdit.getGroup("Reports"));//报表
        groups.push(UserEdit.getGroup("Other_Reports"));//财务报表
        groups.push(UserEdit.getGroup("Fixed_Assets_Reports"));//固定资产报表
        groups.push(UserEdit.getGroup("Edit_Settings"));//基础设置
        groups.push(UserEdit.getGroup("System_Settings"));//系统设置
        groups.push(UserEdit.getGroup("Excel_Plus_Download"));//Excel工具
        groups.push(UserEdit.getGroup("Migration_Tool_Download"));//迁移工具
        return groups;
    },
    //从数组中获取值封装成权限组
    getGroup: function (groupId) {
        var obj = {
            GroupID: groupId,
            View: $("#" + groupId + "_MView").attr("checked") == undefined ? false : true,
            Change: $("#" + groupId + "_MChange").attr("checked") == undefined ? false : true,
            Approve: $("#" + groupId + "_MApprove").attr("checked") == undefined ? false : true,
            Export: $("#" + groupId + "_MExport").attr("checked") == undefined ? false : true
        };
        return obj;
    },
    //选择复选框时联动选择
    clickCheckbox: function () {
        $("#divUserRole input[type='checkbox']").click(function () {
            var id = $(this).attr("id");
            var checked = $(this).attr("checked") == undefined ? false : true;
            //银行复选框联动选中
            UserEdit.linkageBank(id, checked, "MView");
            UserEdit.linkageBank(id, checked, "MChange");
            UserEdit.linkageBank(id, checked, "MApprove");
            UserEdit.linkageBank(id, checked, "MExport");
            //报表复选框联动选中
            UserEdit.linkageReports(id, checked, "MView");
            UserEdit.linkageReports(id, checked, "MChange");
            UserEdit.linkageReports(id, checked, "MApprove");
            UserEdit.linkageReports(id, checked, "MExport");
            //发票复选框联动选中
            UserEdit.linkageFapiao(id, checked, "MView");
            UserEdit.linkageFapiao(id, checked, "MChange");
            UserEdit.linkageFapiao(id, checked, "MApprove");
            UserEdit.linkageFapiao(id, checked, "MExport");
        });
    },
    //银行复选框联动选中
    linkageBank: function (id, ckd, role) {
        if (id == "Bank_" + role) {
            $("#Bank_Reconciliation_" + role).attr("checked", ckd);
            $("#Bank_ExceptReconciliation_" + role).attr("checked", ckd);
        }
        if (id == "Bank_Reconciliation_" + role || id == "Bank_ExceptReconciliation_" + role) {
            var brv = $("#Bank_Reconciliation_" + role).attr("checked") == undefined ? false : true;
            var bev = $("#Bank_ExceptReconciliation_" + role).attr("checked") == undefined ? false : true;
            if (brv && bev) {
                $("#Bank_" + role).attr("checked", true);
            } else {
                $("#Bank_" + role).removeAttr("checked");
            }
        }
    },
    //报表复选框联动选中
    linkageReports: function (id, ckd, role) {
        if (id == "Reports_" + role) {
            $("#Other_Reports_" + role).attr("checked", ckd);
            $("#Fixed_Assets_Reports_" + role).attr("checked", ckd);
        }
        if (id == "Other_Reports_" + role || id == "Fixed_Assets_Reports_" + role) {
            var fr = $("#Other_Reports_" + role).attr("checked") == undefined ? false : true;
            var fa = $("#Fixed_Assets_Reports_" + role).attr("checked") == undefined ? false : true;
            if (fr && fa) {
                $("#Reports_" + role).attr("checked", true);
            } else {
                $("#Reports_" + role).removeAttr("checked");
            }
        }
    },
    //发票复选框联动选中
    linkageFapiao: function (id, ckd, role) {
        if (id == "Fapiao_" + role) {
            $("#Sales_Fapiao_" + role).attr("checked", ckd);
            $("#Purchases_Fapiao_" + role).attr("checked", ckd);
        }
        if (id == "Sales_Fapiao_" + role || id == "Purchases_Fapiao_" + role) {
            var sfv = $("#Sales_Fapiao_" + role).attr("checked") == undefined ? false : true;
            var pfv = $("#Purchases_Fapiao_" + role).attr("checked") == undefined ? false : true;
            if (sfv && pfv) {
                $("#Fapiao_" + role).attr("checked", true);
            } else {
                $("#Fapiao_" + role).removeAttr("checked");
            }
        }
    },
    //用户编辑时银行复选框自动联动
    selectBankParent: function (role) {
        var brv = $("#Bank_Reconciliation_" + role).attr("checked") == undefined ? false : true;
        var bev = $("#Bank_ExceptReconciliation_" + role).attr("checked") == undefined ? false : true;
        if (brv && bev) {
            $("#Bank_" + role).attr("checked", true);
        } else {
            $("#Bank_" + role).removeAttr("checked");
        }
    },
    //用户编辑时报表复选框自动联动
    selectReportsParent: function (role) {
        var grv = $("#Other_Reports_" + role).attr("checked") == undefined ? false : true;
        if (grv) {
            $("#Reports_" + role).attr("checked", true);
        } else {
            $("#Reports_" + role).removeAttr("checked");
        }
    },
    //用户编辑时银行复选框自动联动
    selectFapiaoParent: function (role) {
        var sfv = $("#Sales_Fapiao_" + role).attr("checked") == undefined ? false : true;
        var pfv = $("#Purchases_Fapiao_" + role).attr("checked") == undefined ? false : true;
        if (sfv && pfv) {
            $("#Fapiao_" + role).attr("checked", true);
        } else {
            $("#Fapiao_" + role).removeAttr("checked");
        }
    },
    //用户编辑时复选框自动联动
    selectParentCheckbox: function () {
        //设置银行父 复选框选中
        UserEdit.selectBankParent("MView");
        UserEdit.selectBankParent("MChange");
        UserEdit.selectBankParent("MApprove");
        UserEdit.selectBankParent("MExport");
        //设置报表父 复选框选中
        UserEdit.selectReportsParent("MView");
        UserEdit.selectReportsParent("MChange");
        UserEdit.selectReportsParent("MApprove");
        UserEdit.selectReportsParent("MExport");
        //设置发票父 复选框选中
        UserEdit.selectFapiaoParent("MView");
        UserEdit.selectFapiaoParent("MChange");
        UserEdit.selectFapiaoParent("MApprove");
        UserEdit.selectFapiaoParent("MExport");
    },
    //用户编辑时复选框自动锁定
    autoLockRole: function () {
        //获取当前选择的（岗位_角色）拥有的权限ID列表
        var position_strs = $.trim($("#ddlPosition").combotree("getValues"));
        var role = $.trim($("#ddlRole").combobox("getValue"));
        var pr_ss_str = "";
        //循环选中的岗位
        var positions = position_strs.split(',');
        for (var i = 0; i < positions.length; i++) {
            var pr_s = UserEdit.getPositionRole(positions[i], role);
            if (pr_s) {
                pr_ss_str += pr_s + ",";
            }
        }
        var pr_ss = null;
        if (pr_ss_str) {
            var pr_ss_str1 = pr_ss_str.split(",");
            if (pr_ss_str1.length > 0) {
                pr_ss = new Array();
                for (var i = 0; i < pr_ss_str1.length; i++) {
                    if (pr_ss_str1[i]) {
                        if ($.inArray(pr_ss, pr_ss_str1[i])) {
                            pr_ss.push(pr_ss_str1[i]);
                        }
                    }
                }
            }
        }
        //获取所有权限ID列表
        roleIds = UserEdit.getPositionRole("SysManage", "Admin");
        roleIdss = roleIds.split(',');
        //遍历所有权限Id（所有复选框）
        for (var i = 0; i < roleIdss.length; i++) {
            //当前遍历的复选框Id
            var roleId = $.trim(roleIdss[i]);
            //当前 岗位 和 角色 是否有权限（因为当点击“邀请用户”进来的时候，岗位 和 角色 下拉框默认是为空的，这样就导致根据 岗位 和 角色 获取不到相应的权限）
            if (pr_ss) {
                //获取当前遍历的复选框的 checked 属性值
                var ckbs = $("#" + roleId).attr("checked") == undefined ? false : true;
                //如果当前遍历的复习没有被选中，则需要根据 岗位 和 角色 来区别当前复选框是否可用
                if (!ckbs) {
                    //虽然我没选择某个权限，但是我有某个权限，只是我没有选而已，必须给我留着，不能锁定
                    if (!UserEdit.checkUserRole(pr_ss, roleId) && !UserEdit.isExportItem(roleId)) {
                        //锁定当前复选框
                        $("#" + roleId).attr("disabled", true);
                    }
                }
            } else {
                //锁定当前复选框
                $("#" + roleId).attr("disabled", true);
            }
        }
    },
    //检查（岗位_角色）是否拥有某个权限
    checkUserRole: function (pr_ss, roleId) {
        var isOk = false;
        for (var i = 0; i < pr_ss.length; i++) {
            if ($.trim(pr_ss[i]) == roleId) {
                isOk = true;
                break;
            }
        }
        return isOk;
    },
    //初始化用户权限表格
    initUserRoleTable: function () {
        $('#tbUserRole').treegrid({
            //width: 877,
            nowrap: true,
            animate: true,
            collapsible: false,
            scrollY: true,
            height: $("body").height() - $("#divUserRole").offset().top - 55,
            url: '/BD/User/GetUserRoleList/',
            idField: 'Id',
            treeField: 'MPosition',
            frozenColumns: [[
                {
                    title: '', field: 'MPosition', width: 250, resizable: false,
                    formatter: function (value) {
                        return value;
                    }
                }
            ]],
            columns: [[
                {
                    field: 'MView', title: HtmlLang.Write(LangModule.User, "MView", "ReadOnly / Preview"), width: 155, resizable: false, align: 'center',
                    formatter: function (value, rowData, rowIndex) {
                        return "<input type='checkbox' id='" + rowData.Id + "_MView' " + (rowData.MView ? 'checked' : '') + "/>";
                    }
                },
                {
                    field: 'MChange', title: HtmlLang.Write(LangModule.User, "MChange", "New / Edit"), width: 155, resizable: false, align: 'center',
                    formatter: function (value, rowData, rowIndex) {
                        return "<input type='checkbox' id='" + rowData.Id + "_MChange' " + (rowData.MChange ? 'checked' : '') + "/>";
                    }
                },
                {
                    field: 'MApprove', title: HtmlLang.Write(LangModule.User, "MApprove", "Approve / Publish"), width: 155, resizable: false, align: 'center',
                    formatter: function (value, rowData, rowIndex) {
                        return "<input type='checkbox' id='" + rowData.Id + "_MApprove' " + (rowData.MApprove ? 'checked' : '') + "/>";
                    }
                },
                {
                    field: 'MExport', title: HtmlLang.Write(LangModule.User, "MExport", "Export"), width: 155, resizable: false, align: 'center',
                    formatter: function (value, rowData, rowIndex) {
                        return "<input type='checkbox' id='" + rowData.Id + "_MExport' " + (rowData.MExport ? 'checked' : '') + "/>";
                    }
                }
            ]],
            onLoadSuccess: function (row, data) {
                //获取编辑数据
                UserEdit.initGetList();
                //复选框点击操作
                UserEdit.clickCheckbox();
            }
        });
    },
    //删除，发送邀请，保存，取消
    clickAction: function () {
        //删除
        $("#aDelete").click(function () {
            var itemId = UserEdit.getItemId();
            $.mDialog.confirm(LangKey.AreYouSureToDelete,
            {
                callback: function () {
                    var params = { MItemID: itemId };
                    mAjax.submit("/BD/User/UserLinkInfoDelete/", { model: params }, function (msg) {
                        if (msg == true) {
                            //关闭当前选项卡
                            $.mTab.remove();
                        } else {
                            $.mDialog.alert(HtmlLang.Write(LangModule.User, "AtLeastOneSystemManager", "every organisation must have at least one system manager."));
                        }
                    });
                }
            });
        });
        //发送邀请
        $("#aInvite").click(function () {
            //验证信息
            if (!UserEdit.validateInfo()) {
                return;
            }
            var itemId = UserEdit.getItemId();

            var newEmail = $("#txtMEmail").val();

            //如果当前输入的邮箱不等于原来页面的邮箱，清空mitemid
            if ($.trim(newEmail) != $.trim(UserEdit.oldEmail)) {
                $("#ItemId").val("");
            }

            //用户权限组列表
            var groups = UserEdit.getGroups();
            $("#DivUserEdit").mFormSubmit({
                url: "/BD/User/UserPermissionUpd", param: { model: { MGrpOperateList: groups } }, callback: function (data) {
                    if (data.Success) {
                        //打开发送邀请对话框
                        $("#txtMEmail").attr("disabled", "disabled");
                        UserEdit.openSendInviteDialog(itemId);
                    } else {
                        var msg = data.Message;
                        if (!data.Message) {
                            msg = data.ErrorMessageDetail;
                        }
                        $.mDialog.alert(msg);
                    }
                }
            });
            return false;
        });
        //保存用户信息(新增 或 修改)
        $("#aSave").click(function () {
            if (UserEdit.isCurrentUser()) {
                var title = HtmlLang.Write(LangModule.Common, "AreYouSureToUpdateUserInfo", "After the information saved, the whole website will be refreshed, Please make sure all data is saved.\n Are you sure you want to save?")
                $.mDialog.confirm(title, function () {
                    UserEdit.saveUser();
                });
            }
            else {
                UserEdit.saveUser();
            }
            return false;
        });

        $("#txtMEmail").bind("keyup", function (e) {
            if (e.keyCode == 32) {
                $("#txtMEmail").val($.trim($("#txtMEmail").val()));
            } else {
                UserEdit.lastEventTime = e.timeStamp;
                setTimeout(function () {
                    if (UserEdit.lastEventTime - e.timeStamp == 0) {
                        UserEdit.findUserInfo();
                    }

                }, 1000);
            }
        });
    },
    //根据用户邮箱找用户信息
    findUserInfo: function () {
        var email = $("#txtMEmail").val();
        //没填，不处理
        if (!email) {
            return;
        }
        var url = "/BD/User/GetUserInfo";
        mAjax.post(url, { email: $.trim(email) }, function (data) {
            if (data) {
                $("#txtMFirstName").val(data.MFirstName);
                $("#txtMLastName").val(data.MLastName);

                $("#txtMFirstName").attr("disabled", "disabled");
                $("#txtMLastName").attr("disabled", "disabled");
            } else {
                //可以填写
                $("#txtMFirstName").val("");
                $("#txtMFirstName").removeAttr("disabled");

                $("#txtMLastName").val("");
                $("#txtMLastName").removeAttr("disabled");
            }
        }, null, true);
    },
    isCurrentUser: function () {
        return $("#hidIsCurrentUser").val() == "1" ? true : false;
    },
    validateInfo: function () {
        //启用验证
        $("#ddlPosition").combotree("enableValidation");
        $("#ddlRole").combobox("enableValidation");
        //表单验证
        $('input.easyui-validatebox').validatebox('enableValidation');
        if (!$("#DivUserEdit").mFormValidate()) {
            return false;
        }
        return true;
    },
    saveUser: function () {

        var firstName = $("#txtMFirstName").val();
        var lastName = $("#txtMLastName").val();

        $("#txtMFirstName").val($.trim(firstName));
        $("#txtMLastName").val($.trim(lastName));

        //验证信息
        if (!UserEdit.validateInfo()) {
            return;
        }
        //用户权限组列表
        var groups = UserEdit.getGroups();
        $("#DivUserEdit").mFormSubmit({
            url: "/BD/User/UserPermissionUpd", param: { model: { MGrpOperateList: groups } }, callback: function (data) {
                if (data.Success) {
                    var userId = UserEdit.getItemId();
                    if (!userId && data.ObjectID) {
                        //打开发送邀请对话框
                        UserEdit.openSendInviteDialog(data.ObjectID);
                    } else {
                        //保存成功，提示信息
                        $.mMsg(LangKey.ModifySuccess);
                        if (UserEdit.isCurrentUser()) {
                            top.window.location = top.mWindow.getOrigin();
                        }
                    }
                    //$.mTab.refresh(HtmlLang.Write(LangModule.User, "Users", "Users"), "/BD/User/UserList", false);
                } else {
                    var msg = data.Message;
                    if (!data.Message) {
                        msg = data.ErrorMessageDetail;
                    }
                    $.mDialog.alert(msg, function () {
                        var userId = UserEdit.getItemId();
                        //不为空
                        if (userId) {
                            //打开一个新的tab页
                            //$.mTab.addOrUpdate(HtmlLang.Write(LangModule.User, "Users", "Users"), '/BD/User/UserList', true);
                            //移除当前页
                            $.mTab.remove();

                        }

                    });
                }
            }
        });
    },
    //打开发送邀请对话框
    openSendInviteDialog: function (itemId) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.User, "ResendInvite", "Resend Invite"),
            top: window.pageYOffset || document.documentElement.scrollTop,
            width: 570,
            height: 350,
            href: "/BD/User/UserInvite/" + itemId
        });
    },
    //加载用户权限列表(点击修改进来的)
    initGetList: function () {
        $("body").mFormGet({
            url: "/BD/User/GetUserEditInfo", fill: true, callback: function (data) {
                if (data && data.MItemID != null) {

                    UserEdit.oldEmail = data.MEmail;
                    //过滤掉固定资产（还没做）
                    var grpOperateList = $.grep(data.MGrpOperateList, function (obj, i) {
                        return (
                            (obj.GroupID.toLowerCase().indexOf("fapiao") >= 0) || (
                            obj.GroupID.toLowerCase().indexOf("sale") == -1
                            && obj.GroupID.toLowerCase().indexOf("purchase") == -1
                            && obj.GroupID.toLowerCase().indexOf("expense") == -1
                            && obj.GroupID.toLowerCase().indexOf("pay") == -1
                            && obj.GroupID.toLowerCase().indexOf("sales") == -1
                            && obj.GroupID.toLowerCase().indexOf("bank_reports") == -1
                            && obj.GroupID.toLowerCase().indexOf("financial_reports") == -1)
                            );
                    });

                    $(grpOperateList).each(function (index) {
                        var val = grpOperateList[index];
                        if ($("#divUserRole tr[node-id='" + val.GroupID + "']").length > 0) {
                            $('#tbUserRole').treegrid('update', {
                                id: val.GroupID,
                                row: {
                                    MView: val.View,
                                    MChange: val.Change,
                                    MApprove: val.Approve,
                                    MExport: val.Export
                                }
                            });
                        }
                    });
                    //加载岗位
                    if (data.MPosition) {
                        $("#ddlPosition").combotree("setValues", data.MPosition.split(','));
                    }
                    //加载角色
                    if (data.MRole) {
                        $("#ddlRole").combobox("setValue", data.MRole);
                    }

                    //此变量赋值的顺序不可变，必须在加载岗位 和 角色 之后赋值
                    UserEdit.IsLoad = true;
                    //选中父级复选框
                    UserEdit.selectParentCheckbox();
                    //复选框点击操作
                    UserEdit.clickCheckbox();
                    //禁用Excel Plus下载权限项
                    UserEdit.lockDownLoadPermission("Excel_Plus_Download");
                    UserEdit.lockDownLoadPermission("Migration_Tool_Download");
                    UserEdit.lockAttachmentPermission();

                } else {
                    UserEdit.IsLoad = true;
                }
                //初始化按钮
                UserEdit.initButton();
                //用户编辑时复选框自动锁定
                UserEdit.autoLockRole();
            }
        });
    },
    //初始化操作按钮
    initButton: function () {
        //用户ID
        var itemId = UserEdit.getItemId();
        if (itemId) {
            //PermStatus包含的值：Active 或 Pending
            var PermStatus = $("#PermStatus").val();
            var spanPermStatus = $("#spanPermStatus");
            if (PermStatus == "Active") {
                //隐藏邀请按钮
                $("#aInvite").hide();
                //锁定姓名和电子邮箱
                $("#txtMFirstName").attr("disabled", true);
                $("#txtMLastName").attr("disabled", true);
                $("#txtMEmail").attr("disabled", true);
                //多语言
                var isArchive = $("#hidIsArchive").val();
                if (isArchive == 1) {
                    PermStatus = HtmlLang.Write(LangModule.User, "Archive", "Archive");
                    spanPermStatus.css("color", "#777");
                } else {
                    PermStatus = HtmlLang.Write(LangModule.User, "Active", "Active");
                    spanPermStatus.css("color", "#57a400");
                }
            } else {
                //设置状态颜色
                spanPermStatus.css("color", "#777");
                //多语言
                PermStatus = HtmlLang.Write(LangModule.User, "Pending", "Pending");

                $("#aInvite").show();
                $("#txtMEmail").attr("disabled", true);
            }
            //显示状态
            spanPermStatus.text(PermStatus);
            //按钮文字改为：保存
            $("#ConOrSave").html(HtmlLang.Write(LangModule.User, "Edit", "修改"));
            $("#divPermStatus").show();
            $("#aDelete").show();

        } else {
            $("#divPermStatus").hide();
            $("#aDelete").hide();
            $("#aInvite").hide();
        }
        $("#aSave").show();
    }
}
//初始化页面数据
$(document).ready(function () {
    UserEdit.init();
});