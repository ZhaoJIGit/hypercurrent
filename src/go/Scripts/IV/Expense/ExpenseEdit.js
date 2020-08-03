/// <reference path="/Scripts/IV/IVBase.js" />
/// <reference path="/Scripts/IV/Expense/ExpenseEditBase.js" />
var ExpenseEdit = {
    //费用报销单ID
    ExpenseID: $("#hidExpenseID").val(),
    //费用报销单模型
    ExpenseModel: null,
    //当前编辑的费用报销单状态，默认为草稿状态
    CurrentStatus: 1,
    //费用报销单列表地址
    ListUrl: "/IV/Expense/ExpenseList/",
    //费用报销单编辑地址
    EditUrl: "/IV/Expense/ExpenseEdit",
    //费用报销单编辑地址
    InitEditUrl: "/IV/Expense/ExpenseInitEdit",
    //费用报销单查看地址
    ViewUrl: "/IV/Expense/ExpenseView",
    IsFirstLoad: false,
    EmployeesInfo: null,
    //初始化
    init: function () {
        ExpenseEdit.IsFirstLoad = true;
        //解决手动修改日期，到期日没跟着变问题
        $("#txtDate").datebox({
            onChange: function (newValue, oldValue) {
                if ($("#txtDate").datebox("panel").is(":visible")) {
                    return;
                }
                if (mDate.isDateString(newValue)) {
                    ExpenseEdit.changeDate();
                    
                }
            },
            onSelect: function (date) {
                ExpenseEdit.changeDate();
            }
        });
        ExpenseEdit.initExpenseModel();
        ExpenseEdit.initCombobox();
        ExpenseEdit.initOperate();
        //将当前页面设置为稳定状态
        $.mTab.setStable();
        //ExpenseEdit.IsFirstLoad = false;
    },
    //初始化下拉框
    initCombobox: function () {
        //员工下拉框
        $('#selEmployee').mAddCombobox("employee",
            {
                //数据加载成功后更新数据源
                onLoadSuccess: function (msg) {
                    //提示文字
                    $("#selEmployee").initHint();
                    //选中员工
                    if (ExpenseEdit.ExpenseModel && ExpenseEdit.ExpenseModel.MEmployee) {
                        $('#selEmployee').combobox('setValue', ExpenseEdit.ExpenseModel.MEmployee);
                    }
                    //将当前页面设置为稳定状态
                    $.mTab.setStable();
                },
                onSelect: function (rec) {
                    ExpenseEditBase.changeContact(rec.MItemID, function () {
                        //ExpenseEditBase.setDefaultDueDate();
                    });
                },
                onChange: function (value, text) {
                    if (!ExpenseEdit.IsFirstLoad) {

                        $.mAjax.post("/BD/Employees/GetEmployeesInfo", { model: { MItemID: value } }, function (data) {
                            ExpenseEdit.EmployeesInfo = data;
                            var date = new Date();
                            var dateVal = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate(); //默认当前日期
                            var isDate = mDate.isValidDate($('#txtDate').datebox('getValue')); //检查费用报销日是否输入正确的日期
                            var txtdate = null;
                            if (isDate) {
                                txtdate = new Date($('#txtDate').datebox('getValue')); //费用报销日
                            }
                            if (data.MPurDueDate && data.MPurDueCondition) {
                                switch (data.MPurDueCondition) {
                                    case "item0": //下一个月
                                        dateVal = date.getFullYear() + "-" + (date.getMonth() + 2) + "-" + data.MPurDueDate;
                                        break;
                                    case "item1": //费用报销单日期后的天数
                                        if (txtdate) {
                                            var purDueDate = txtdate.addDays(data.MPurDueDate);
                                            dateVal = purDueDate.getFullYear() + "-" + (purDueDate.getMonth() + 1) + "-" + purDueDate.getDate();
                                        }
                                        break;
                                    case "item2": //费用报销单月末后的天数
                                        if (txtdate) {
                                            dateVal = txtdate.getFullYear() + "-" + (txtdate.getMonth() + 2) + "-" + data.MPurDueDate;
                                        }
                                        break;
                                    case "item3": //当前月份
                                        if (date.getDate() > data.MPurDueDate) {
                                            dateVal = date.getFullYear() + "-" + (date.getMonth() + 2) + "-" + data.MPurDueDate;
                                        } else {
                                            dateVal = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + data.MPurDueDate;
                                        }
                                        break;
                                }
                            }
                            $('#txtDueDate').datebox('setValue', dateVal);
                        }, null, false, false);
                    }
                    else {
                        ExpenseEdit.IsFirstLoad = false;
                    }
                }
            },
            {
                //是否有联系人编辑权限
                hasPermission: ExpenseEditBase.IsCanSettingChangePermission,
                width: 1100,
                //弹出框关闭后的回调函数
                callback: function (param) {
                    if (param) {
                        $('#selEmployee').combobox('setValue', param);
                    }
                }
            }
        );

        //部门下拉框
        //var orgId = $("#hidOrgId").val();
        //$('#selDepartment').mAddCombobox("department",
        //    {
        //        //数据加载成功后更新数据源
        //        onLoadSuccess: function (msg) {
        //            //提示文字
        //            $("#selDepartment").initHint();
        //            //选中部门
        //            if (ExpenseEdit.ExpenseModel && ExpenseEdit.ExpenseModel.MDepartment) {
        //                $('#selDepartment').combobox('setValue', ExpenseEdit.ExpenseModel.MDepartment);
        //            }
        //            //将当前页面设置为稳定状态
        //            $.mTab.setStable();
        //        },
        //        onSelect: function (rec) {

        //        }
        //    },
        //    {
        //        //是否有联系人编辑权限
        //        hasPermission: ExpenseEditBase.IsCanSettingChangePermission,
        //        //弹出框关闭后的回调函数
        //        callback: null,
        //        url: "/BD/Dept/DeptEdit?orgid=" + orgId + "&parentid=" + orgId + "&op=add"
        //    }
        //);
    },
    //初始化相关操作
    initOperate: function () {
        //保存
        $("#btnSave").click(function () {
            ExpenseEdit.saveExpense(ExpenseEdit.CurrentStatus, function (msg) {
                //提示信息
                $.mMsg(LangKey.SaveSuccessfully);
                //修改当前tab选项卡标题
                $.mTab.rename(ExpenseEditBase.getTabTitle());
                //列表tab选项卡标题
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                //刷新的Url地址
                var status = ExpenseEdit.CurrentStatus == 0 ? IVBase.Status.Draft : ExpenseEdit.CurrentStatus;
                var url = ExpenseEdit.ListUrl + "?id=" + status;
                //在指定的tab选项卡中刷新
                $.mTab.refresh(title, url, false, true);
                if (ExpenseEdit.CurrentStatus == IVBase.Status.AwaitingPayment || ExpenseEdit.CurrentStatus == IVBase.Status.Paid) {
                    //如果状态为：等待支付 或 已支付，则跳转至查看页面
                    mWindow.reload(ExpenseEdit.ViewUrl + "/" + msg.ObjectID);
                } else {
                    //否则跳转至编辑页面
                    mWindow.reload(ExpenseEdit.EditUrl + "/" + msg.ObjectID);
                }
            });
        });
        //保存为草稿
        $("#btnSaveAsDraft").click(function () {
            ExpenseEdit.saveExpense(IVBase.Status.Draft, function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(ExpenseEditBase.getTabTitle());
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.Draft;
                $.mTab.refresh(title, url, false, true);
                mWindow.reload(ExpenseEdit.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后继续编辑
        $("#btnSaveAndContinue").click(function () {
            ExpenseEdit.saveExpense(ExpenseEdit.CurrentStatus, function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(ExpenseEditBase.getTabTitle());
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.Draft;
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面
                mWindow.reload(ExpenseEdit.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后提交审核
        $("#btnSaveAndSubmitForApproval").click(function () {
            ExpenseEdit.saveExpense(IVBase.Status.WaitingApproval, function (msg) {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(ExpenseEditBase.getTabTitle());
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.WaitingApproval;
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面
                mWindow.reload(ExpenseEdit.EditUrl + "/" + msg.ObjectID);
            });
        });
        //保存后重新添加一个费用报销
        $("#btnSaveAndAddAnother").click(function () {
            ExpenseEdit.saveExpense(ExpenseEdit.CurrentStatus, function () {
                $.mMsg(LangKey.SaveSuccessfully);
                $.mTab.rename(ExpenseEditBase.getTabTitle("NewExpense"));
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.Draft;
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(ExpenseEdit.EditUrl);
            });
        });
        //审核费用报销
        $("#aApproveExpense").click(function () {
            ExpenseEdit.saveExpense(IVBase.Status.AwaitingPayment, function (msg) {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                $.mTab.rename(ExpenseEditBase.getTabTitle());
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.AwaitingPayment;
                $.mTab.refresh(title, url, false, true);
                //跳转至查看页面
                mWindow.reload(ExpenseEdit.ViewUrl + "/" + msg.ObjectID + "?sv=1");
            });
        });
        //审核后重新添加一个费用报销
        $("#aApproveAndAddAnother").click(function () {
            ExpenseEdit.saveExpense(IVBase.Status.AwaitingPayment, function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                $.mTab.rename(ExpenseEditBase.getTabTitle("NewExpense"));
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.AwaitingPayment;
                $.mTab.refresh(title, url, false, true);
                //跳转至编辑页面（新增不带ID）
                mWindow.reload(ExpenseEdit.EditUrl);
            });
        });
        //审核后查看下一个
        $("#aApproveAndViewNext").click(function () {
            var expId = $(this).attr("expId");
            ExpenseEdit.saveExpense(IVBase.Status.AwaitingPayment, function () {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                var title = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var url = ExpenseEdit.ListUrl + "?id=" + IVBase.Status.AwaitingPayment;
                $.mTab.refresh(title, url, false, true);
                //跳转至查看页面
                mWindow.reload(ExpenseEdit.EditUrl + "/" + expId + "?sv=1");
            });
        });
        //审核后打印
        $("#aApproveAndPrint").click(function () {
            ExpenseEdit.saveExpense(IVBase.Status.AwaitingPayment, function (response) {
                $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));

                var listTitle = HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims");
                var listUrl = ExpenseEditBase.url_ExpenseList + "?id=" + IVBase.Status.AwaitingPayment;

                //打印
                var printParam = {};
                var expenseId = $.trim($("#hidExpenseID").val());
                expenseId = expenseId == "" ? response.ObjectID : expenseId;
                printParam.SelectedIds = expenseId;
                IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims"), $.toJSON(printParam), "ExpenseClaimsPrint");

                $.mTab.refresh(listTitle, listUrl, false, true);
                mWindow.reload(ExpenseEdit.ViewUrl + "/" + expenseId + "?sv=1");
            });
        });
        //操作记录
        $("#aHistory").click(function () {
            ExpenseEditBase.OpenHistoryDialog(ExpenseEdit.ExpenseID, "History");
        });
        //打印
        $("#aPrint").click(function () {
            IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims"), $.toJSON({ SelectedIds: $("#hidExpenseID").val() }), "ExpenseClaimsPrint");
        });
    },
    afterAddContact: function (param) {
        if (param) {
            $('#selContact').combobox('setValue', param);
        }
    },
    //设置默认的到期日期
    setDefaultDueDate: function () {
        var billDate = $('#txtDate').datebox('getValue');
        var value = ExpenseEditBase.getDefaultDueDate(billDate);
        $('#txtDueDate').datebox('setValue', value);
    },
    //录入日期改变时，到期日期自动改变
    changeDate: function () {
        ExpenseEditBase.changeDate();
        var data = ExpenseEdit.EmployeesInfo;
        if (data) {
            if (data.MPurDueDate && data.MPurDueCondition) {
                var txtdate = new Date(newValue);//费用报销日
                switch (data.MPurDueCondition) {
                    case "item1"://费用报销单日期后的天数
                        var purDueDate = txtdate.addDays(data.MPurDueDate);
                        $('#txtDueDate').datebox('setValue', purDueDate.getFullYear() + "-" + (purDueDate.getMonth() + 1) + "-" + purDueDate.getDate());
                        break;
                    case "item2"://费用报销单月末后的天数
                        $('#txtDueDate').datebox('setValue', txtdate.getFullYear() + "-" + (txtdate.getMonth() + 2) + "-" + data.MPurDueDate);
                        break;
                }
            }
        }
    },
    //初始化费用报销单
    initExpenseModel: function () {
        //费用报销单模型
        var msg = mText.getObject("#hidExpenseModel");
        ExpenseEdit.ExpenseModel = msg;
        //摘要
        $("#txtRef").val(msg.MReference);
        //录入日期（如果费用报销单ID不为空，则设置指定的日期，否则使用页面上的默认日期）
        if (ExpenseEdit.ExpenseID != "") {
            $('#txtDate').datebox('setValue', $.mDate.format(msg.MBizDate));
        }
        //到期日期
        $('#txtDueDate').datebox('setValue', $.mDate.format(msg.MDueDate));
        //预计付款日期
        $('#txtExpectedDate').datebox('setValue', $.mDate.format(msg.MExpectedDate));
        //状态
        ExpenseEdit.CurrentStatus = msg.MStatus;

        //初始化父类信息
        ExpenseEditBase.init(ExpenseEditBase.IVType.Sale, ExpenseEdit.ExpenseID, msg.MOToLRate, msg.MLToORate, msg.Verification, msg.ExpenseEntry, msg.MCyID, ExpenseEdit.CurrentStatus, false, false, $('#txtDate').datebox('getValue'));
    },
    //保存费用报销单
    saveExpense: function (status, callback) {
        var bizDate = $('#txtDate').datebox('getValue');
        var dueDate = $('#txtDueDate').datebox('getValue');
        //状态如果小于1，则默认为1（状态 1 为草稿）
        if (status < 1) {
            status = 1;
        }
        if (!ExpenseEditBase.verifyExchangeRate(bizDate)) {
            return;
        }
        //获取费用报销单明细列表
        var entryInfo = ExpenseEditBase.getViewInfo();
        //验证费用报销单
        var result = $(".m-form-icon").mFormValidate();
        //验证费用报销单明细
        entryResult = ExpenseEditBase.valideInfo();
        if (!result || !entryResult) {
            //验证不通过，则终止该操作
            return;
        }

        var employeeId = $("#selEmployee").combobox("getValue");
        var employeeName = $("#selEmployee").combobox("getText")
        if (!employeeId || employeeId == employeeName) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "EmployeeNotExist", "员工不存在，请重新选择员工"));
            return;
        }

        //到期日期必须大于单据录入日期
        if (dueDate < bizDate) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "DueDateIsGreaterThanTheDate", "Due date must be greater than the date."));
            return;
        }
        //预计付款日期必须大于等于单据录入日期
        var expectedDate = $('#txtExpectedDate').datebox('getValue');
        if (expectedDate) {
            //如果选择预计付款日期才需要验证
            if (expectedDate < bizDate) {
                $.mDialog.alert(HtmlLang.Write(LangModule.IV, "ExpectedDateIsGreaterThanTheDate", "Expected Payment date must be greater than or equal to date."));
                return;
            }
        }
        //至少要有一条费用报销单明细
        if (entryInfo.ExpenseEntry == null || entryInfo.ExpenseEntry.length < 1) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "AtLeastOneLineItem", "You must have at least 1 line item."));
            return;
        }
        //每一行分录的税金额必须小于总金额
        if (entryInfo.ExpenseEntry.where("+x.MTaxAmtFor > +x.MTaxAmountFor").length > 0) {
            //提醒用户
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "TaxAmountMustLessThanTotalAmount", "分录税金额必须小于总金额."));
            return;
        }
        //汇总金额必须大于0
        var total = parseFloat($("#spTotal").text());
        if (!total || total <= 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "TotalAmountNotToZero", "Aggregate amount must be greater than zero."));
            return;
        }

        //要保存的费用报销单模型
        var obj = {};
        //费用报销单ID
        obj.MID = $("#hidExpenseID").val();
        //摘要
        obj.MReference = $("#txtRef").val();
        //录入日期
        obj.MBizDate = bizDate;
        //到期日期
        obj.MDueDate = dueDate;
        //预计付款日期
        obj.MExpectedDate = expectedDate;
        //员工
        obj.MEmployee = $("#selEmployee").combobox('getValue');
        //部门
        //obj.MDepartment = $("#selDepartment").combobox('getValue');
        //状态
        obj.MStatus = status;
        //发票类型（日志需要）
        obj.MType = "Expense_Claims";
        //发票金额（不含税、原币）
        obj.MTotalAmtFor = entryInfo.MTotalAmtFor;
        //发票金额（不含税、本位币）
        obj.MTotalAmt = entryInfo.MTotalAmt;
        //发票金额（含税、原币）
        obj.MTaxTotalAmtFor = entryInfo.MTaxTotalAmtFor;
        //发票金额（含税、本位币）
        obj.MTaxTotalAmt = entryInfo.MTaxTotalAmt;
        //币别ID
        obj.MCyID = entryInfo.MCyID;
        //汇率
        obj.MExchangeRate = entryInfo.MExchangeRate;
        obj.MOToLRate = entryInfo.MOToLRate;
        obj.MLToORate = entryInfo.MLToORate;
        //费用报销单明细列表
        obj.ExpenseEntry = entryInfo.ExpenseEntry;

        var dbModel = mText.getObject("#hidExpenseModel");
        obj.MCurrentAccountCode = dbModel.MCurrentAccountCode;

        //提交保存
        $("body").mFormSubmit({
            url: "/IV/Expense/UpdateExpense", param: { model: obj }, extValidate: ExpenseEditBase.valideInfo, validate: true, callback: function (msg) {
                if (msg.Success == false) {
                    if (msg.Code == "1000") {
                        $.mDialog.alert(msg.Message, {
                            callback: function () {
                                mWindow.reload();
                            }
                        });
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                    return;
                }
                //保存费用报销单附件                
                if (!ExpenseEdit.ExpenseID && typeof (AssociateFiles) != undefined && typeof (AssociateFiles) != 'undefined') {
                    AssociateFiles.associateFilesTo($("#hidBizObject").val(), msg.ObjectID, undefined, function () {
                        if (callback != undefined) {
                            callback(msg);
                        }
                    });
                }
                else {
                    if (callback != undefined) {
                        callback(msg);
                    }
                }
            }
        });
    }
}
//初始化页面
$(document).ready(function () {
    ExpenseEdit.init();
});