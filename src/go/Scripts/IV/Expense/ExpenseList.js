/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="../IVBase.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ExpenseList = {
    //Tab选项卡的索引号
    CurrentType: 0,
    //列表地址
    url_List: "/IV/Expense/ExpenseList/",
    //编辑地址
    url_Edit: "/IV/Expense/ExpenseEdit",
    //查看地址
    url_View: "/IV/Expense/ExpenseView",
    //是否有权限进行 编辑 和 删除
    HasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    //状态
    Status: 0,
    //初始化页面
    init: function () {
        ExpenseList.initSearchControl();
        ExpenseList.initTab();
        ExpenseList.initAction();
        //初始化父类的绑定事件
        IVBase.bindEvent = function () {
            ExpenseList.bindGrid(ExpenseList.CurrentType);
        }
    },
    //初始化查询控件
    initSearchControl: function () {
        var filter = Megi.request("filter");
        if (filter != "") {
            $(".m-adv-search-btn").hide();
            $(".m-adv-search").show();
            filter = eval("(" + filter + ")");
            $("body").mFormSetForm(filter);
        }
    },
    //初始化 Tab 选项卡
    initTab: function () {
        var typeId = Number($("#hidExpenseType").val());
        $(".m-extend-tabs").tabsExtend({
            initTabIndex: typeId,
            onSelect: function (index) {
                ExpenseList.bindGrid(index);
                ExpenseList.CurrentType = index;
                //刷新Tab合计数据
                IVBase.tabSelRefreshTitle(3);
                $(window).resize();
            }
        });
    },
    //初始化内页工具栏
    initAction: function () {
        //批量删除
        $("#btnDelete").click(function () {
            Megi.grid("#gridExpense", "deleteSelected", {
                url: "/IV/Expense/DeleteExpenseList", callback: function (msg) {
                    if (msg.Success == false) {
                        $.mDialog.alert(msg.Message);
                        return;
                    }
                    $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                    mWindow.reload(ExpenseList.url_List + ExpenseList.CurrentType);
                }
            });
        });
        //批量提交并审核
        $("#btnSbmForAppr").click(function () {
            Megi.grid("#gridExpense", "optSelected", {
                url: "/IV/Expense/UpdateExpenseStatus", msg: HtmlLang.Write(LangModule.IV, "AreYouSureToSubmitForApproval", "Are you sure to submit for approval?"), param: { MOperationID: IVBase.Status.WaitingApproval },
                callback: function (msg) {
                    if (msg.Success == false) {
                        $.mDialog.alert(msg.Message);
                        return;
                    }
                    $.mMsg(HtmlLang.Write(LangModule.IV, "SubmitForApprovalSuccessfully", "Submit for approval Successfully!"));
                    mWindow.reload(ExpenseList.url_List + ExpenseList.CurrentType);
                }
            });
        });
        //批量审核
        $("#btnApproval").click(function () {
            Megi.grid("#gridExpense", "optSelected", {
                url: "/IV/Expense/ApproveExpense", msg: HtmlLang.Write(LangModule.IV, "AreYouSureToApproval", "Are you sure to approval?"), param: { MOperationID: IVBase.Status.AwaitingPayment },
                callback: function (msg) {
                    if (msg.Success == false) {
                        var arrMsg = [msg.Message];
                        //错误明细
                        if (msg.Tag) {
                            arrMsg.push(msg.Tag);
                        }
                        $.mDialog.alert("<div style='margin-left:10px;'>" + arrMsg.join("").replace(/\n|\r\n/g, "<br>") + "</div>", undefined, 0, true, true);
                        return;
                    }
                    $.mMsg(HtmlLang.Write(LangModule.IV, "ApproveSuccessfully", "Approve Successfully!"));
                    mWindow.reload(ExpenseList.url_List + ExpenseList.CurrentType);
                }
            });
        });
        //查询
        $("#aSearchExpense").click(function () {
            ExpenseList.bindGrid(ExpenseList.CurrentType);
        });

        $("#btnMergePay").click(function () {
            Megi.grid("#gridExpense", "optSelected", {
                callback: function (ids) {
                    mAjax.submit(
                        "/IV/Expense/IsSuccessMerge",
                        { expids: ids },
                        function (res) {
                            if (res.Success) {
                                $.mDialog.show({
                                    mTitle: HtmlLang.Write(LangModule.IV, "NewBatchPayment", "New Batch Payment"),
                                    mWidth: 700,
                                    mHeight: 400,
                                    mShowbg: true,
                                    mShowTitle: true,
                                    mDrag: "mBoxTitle",
                                    mContent: "iframe:/IV/UC/BatchPayment",
                                    mPostData: { selectIds: ids, obj: "Expense" }
                                });
                            }
                            else {
                                $.mDialog.alert(res.Message);
                            }
                        });
                }
            });
            return false;
        });

        //导出
        $("#aExport").click(function () {
            location.href = '/IV/Expense/Export?jsonParam=' + escape($.toJSON(ExpenseList.getQueryParam()));
            $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        });
        //打印
        $("#aPrint").click(function () {
            Megi.grid("#gridExpense", "optSelected", {
                callback: function (ids) {
                    IVBase.OpenPrintDialog(HtmlLang.Write(LangModule.IV, "ExpenseClaims", "Expense Claims"), $.toJSON(ExpenseList.getQueryParam(ids)), "ExpenseClaimsPrint");
                }
            });
        });
    },
    //显示附件
    viewFileInfo: function (fileId, fileIds) {
        Megi.openDialog('/BD/Docs/FileView', '', 'curFileId=' + fileId + '&fileIds=' + fileIds, 560, 460);
    },
    getQueryParam: function (selectedIds) {
        var queryParam = {}
        if ($(".m-adv-search").css("display") == "block") {
            queryParam = $("body").mFormGetForm();
        }
        queryParam.BizObject = "Expense";
        queryParam.MStatus = ExpenseList.Status;
        if (selectedIds) {
            queryParam.SelectedIds = selectedIds;
        }
        return queryParam;
    },
    //绑定数据列表
    bindGrid: function (type) {
        //状态（每个Tab选项卡对应一个状态值）
        var status = 0;
        //列表总列数
        var columnList = null;
        //隐藏内页工具栏中的所有按钮（出查询按钮外）
        $('.m-tab-toolbar .left .m-tool-bar-btn').hide();
        switch (type) {
            case 0://所有
                columnList = Megi.getDataGridColumnList(ExpenseList.columns, [1, 2, 3, 4, 5, 6, 7, 8, 11]);
                break;
            case 1://草稿
                columnList = Megi.getDataGridColumnList(ExpenseList.columns, [0, 1, 2, 3, 4, 5, 7, 10, 11]);
                status = IVBase.Status.Draft;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnMergePay').hide();
                break;
            case 2://等待审核
                columnList = Megi.getDataGridColumnList(ExpenseList.columns, [0, 1, 2, 3, 4, 5, 7, 10, 11]);
                status = IVBase.Status.WaitingApproval;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnMergePay').hide();
                break;
            case 3://等待支付
                columnList = Megi.getDataGridColumnList(ExpenseList.columns, [0, 1, 2, 3, 4, 5, 6, 7, 10, 11]);
                status = IVBase.Status.AwaitingPayment;
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnApproval').hide();
                $('#btnDelete').hide();
                $('#btnMergePay').show();
                break;
            case 4://已支付
                columnList = Megi.getDataGridColumnList(ExpenseList.columns, [0, 1, 2, 3, 4, 5, 6, 7, 10, 11]);
                status = IVBase.Status.Paid;
                //根据状态的不同来显示不同的操作按钮
                $('.m-tab-toolbar .left .m-tool-bar-btn').show();
                $('#btnSbmForAppr').hide();
                $('#btnApproval').hide();
                $('#btnDelete').hide();
                $('#btnMergePay').hide();
                break;
        }
        ExpenseList.Status = status;
        //查询参数列表
        var queryParam = {}
        //获取查询参数
        if ($(".m-adv-search").css("display") == "block") {
            queryParam = $("body").mFormGetForm();
        }
        //状态
        queryParam.MStatus = status;
        //发票类型
        queryParam.MType = IVBase.InvoiceType.Sale;
        //开始查询数据并绑定数据列表
        Megi.grid("#gridExpense", {
            url: "/IV/Expense/GetExpenseList",
            pagination: true,
            queryParams: queryParam,
            columns: columnList,
            onLoadSuccess: function () {
                $("#gridExpense").datagrid('resizeColumn');
            }
        });
    },
    //数据列表总列数
    columns: [
        //复选框
        {
            title: '<input type=\"checkbox\" >', field: 'MID', width: 25, align: 'center', formatter: function (value, rec, rowIndex) {
                return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" >";
            }
        },
        //员工
        {
            title: HtmlLang.Write(LangModule.IV, "Employee", "Employee"), field: 'MContactName', width: 120, sortable: true, formatter: function (value, rec, rowIndex) {
                if (value) {
                    return '<span class="icon-invoice-add">&nbsp;</span>' + value;
                } else {
                    return "";
                }
            }
        },
        //备注
        {
            title: HtmlLang.Write(LangModule.IV, "Ref", "Ref"), field: 'MReference', width: 150, align: 'left', sortable: true, formatter: function (value, rec, rowIndex) {
                if (value) {
                    return value;
                } else {
                    return "";
                }
            }
        },
        //录入日期
        { title: HtmlLang.Write(LangModule.IV, "Date", "Date"), field: 'MBizDate', width: 90, align: 'center', sortable: true, formatter: $.mDate.formatter },
        //到期日期
        {
            title: HtmlLang.Write(LangModule.IV, "DueDate", "Due Date"), field: 'MDueDate', width: 90, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                var dueDate = $.mDate.format(value);
                if (dueDate == "") {
                    return "&nbsp;";
                }
                var now = new Date();
                var tempDate = new Date(dueDate);
                tempDate.setDate(tempDate.getDate() + 1);
                if (tempDate < now) {
                    return "<span class='red'>" + dueDate + "</span>";
                }
                return dueDate;
            }
        },
        //预计付款日期
        {
            title: HtmlLang.Write(LangModule.IV, "ExpectedDate", "Expected Date"), field: 'MExpectedDate', width: 90, align: 'center', sortable: true, formatter: function (value, rec, rowIndex) {
                var expDate = $.mDate.format(value);
                return expDate;
            }
        },
        //已核销金额
        {
            title: HtmlLang.Write(LangModule.IV, "Paid", "Paid"), field: 'MVerificationAmt', width: 90, align: "right", sortable: true, formatter: function (value, rec, rowIndex) {
                value = Math.abs(value);
                if (rec.MOrgCyID == rec.MCyID) {
                    return Megi.Math.toDecimal(value, 2);
                } else {
                    return "<span class='iv-cy'  onmouseover=\"ExpenseList.showLocalCurrency(this," + rec.MVerifyAmt + ",'" + rec.MOrgCyID + "');\">" + rec.MCyID + "</span>" + Megi.Math.toDecimal(value, 2);
                }
            }
        },
        //发票金额（含税、原币）
        {
            title: HtmlLang.Write(LangModule.IV, "Unpaid", "未付"), field: 'MTaxTotalAmtFor', width: 90, align: "right", sortable: true, formatter: function (value, rec, rowIndex) {
                var result = Math.abs(value) - Math.abs(rec.MVerificationAmt);
                if (rec.MOrgCyID == rec.MCyID) {
                    return Megi.Math.toDecimal(result, 2);
                } else {
                    return "<span class='iv-cy'   onmouseover=\"ExpenseList.showLocalCurrency(this," + (Math.abs(rec.MTaxTotalAmt) - Math.abs(rec.MVerifyAmt)) + ",'" + rec.MOrgCyID + "');\">" + rec.MCyID + "</span>" + Megi.Math.toDecimal(result, 2);
                }
            }
        },
        //状态
        {
            title: HtmlLang.Write(LangModule.IV, "Status", "Status"), field: 'MStatus', width: 120, fixed: true, sortable: true, formatter: function (value, rec, rowIndex) {
                var status = Number(value);
                switch (status) {
                    case IVBase.Status.Draft:
                        return HtmlLang.Write(LangModule.IV, "Draft", "Draft");
                    case IVBase.Status.WaitingApproval:
                        return HtmlLang.Write(LangModule.IV, "AwaitingApproval", "Awaiting Approval");
                    case IVBase.Status.AwaitingPayment:
                        return HtmlLang.Write(LangModule.IV, "AwaitingPayment", "Awaiting Payment");
                    case IVBase.Status.Paid:
                        return HtmlLang.Write(LangModule.IV, "Paid", "Paid");
                }
            }
        },
        //是否已发送过电子邮件
        {
            title: HtmlLang.Write(LangModule.IV, "Sent", "Sent"), field: 'Sent', width: 30, sortable: true, align: 'center', formatter: function (value, rec, rowIndex) {
                return value == true ? "<span class=\"icon-flag-done\"></span>" : "";
            }
        },
        //附件
        {
            title: HtmlLang.Write(LangModule.Common, "Attachment", "Attachment"), field: 'Attach', align: 'center', width: 100, sortable: false, formatter: function (value, rec, rowIndex) {
                var hasAttach = rec.MAttachIDs != null && rec.MAttachIDs != '';
                var curAttachId = hasAttach ? rec.MAttachIDs.split(',')[0] : '';
                var attachCount = hasAttach ? rec.MAttachIDs.split(',').length : '';
                var attachIconClass = hasAttach ? "m-list-attachment" : "";
                return "<a href='javascript:void(0);' onclick=\"ExpenseList.viewFileInfo('" + curAttachId + "', '" + rec.MAttachIDs + "', '');\" class='" + attachIconClass + "'><span>" + attachCount + "</span></a>";
            }
        },
        //编辑 和 删除
        {
            title: HtmlLang.Write(LangModule.IV, "Operation", "Operation"), field: 'Action', align: 'center', width: 60, sortable: false, formatter: function (value, rec, rowIndex) {
                //验证是否有权限进行 编辑 和 删除（这个验证只是为了方便用户操作，其实后台服务器做了双重权限验证）
                if (IVBase.HasChangeAuth) {
                    //将 双引号 和 单引号 替换一下，避免在方法中作为参数传递出现错误
                    if (rec.MReference) {
                        rec.MReference = rec.MReference.replace(/"/g, "").replace(/'/g, "");
                    }
                    //1.有权限，则显示 编辑 和 删除
                    //2.为了和批量删除按钮的显示隐藏保持一致，这里需要判断一下当前列表行是否需要显示删除操作，列表编号为 1 和 2（Draft，Awaiting Approval）的需要显示删除操作
                    if (ExpenseList.CurrentType == 1 || ExpenseList.CurrentType == 2) {
                        return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"ExpenseList.editItem(" + rec.MStatus + ", '" + rec.MID + "');\" class='list-item-edit'></a><a href=\"javascript:void(0);\" onclick=\"ExpenseList.deleteItem('" + rec.MID + "');\" class='list-item-del'></a></div>";
                    } else {
                        //其他列表行只显示编辑操作
                        return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"ExpenseList.editItem(" + rec.MStatus + ", '" + rec.MID + "');\" class='list-item-edit'></a></div>";
                    }
                } else {
                    //没权限，则显示 编辑，因为编辑页面会对该操作进行权限验证，如果没有权限，则显示为只读
                    return "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"ExpenseList.editItem(" + rec.MStatus + ", '" + rec.MID + "');\" class='list-item-edit'></a></div>";
                }
            }
        }
    ],
    showLocalCurrency: function (selector, totalTaxAmt, localCyId) {
        $(selector).mLocalCyTooltip(totalTaxAmt, localCyId);
        $(selector).tooltip("show")
    },
    //删除
    deleteItem: function (id) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete, {
            callback: function () {
                var param = {};
                param.KeyIDs = id;
                mAjax.submit(
                    "/IV/Expense/DeleteExpenseList",
                    { param: param },
                    function (msg) {
                        if (msg.Success) {
                            mWindow.reload(ExpenseList.url_List + ExpenseList.CurrentType);
                        } else {
                            $.mDialog.alert(HtmlLang.Write(LangModule.IV, "Deletefailed", "Delete failed"));
                        }
                    });
            }
        });
    },
    //编辑
    editItem: function (MStatus, MID) {
        var type = typeof InitBills;
        if (type == "undefined") {
            if (!ExpenseList.HasChangeAuth || MStatus == IVBase.Status.AwaitingPayment || MStatus == IVBase.Status.Paid) {
                var tabTitle = HtmlLang.Write(LangModule.IV, "ViewExpense", "View Expense");
                //等待付款，还可以改预计付款日期，所以标题是编辑：Edit Expense
                if (MStatus == IVBase.Status.AwaitingPayment) {
                    tabTitle = HtmlLang.Write(LangModule.IV, "EditExpense", "Edit Expense");
                }
                $.mTab.addOrUpdate(tabTitle, ExpenseList.url_View + "/" + MID + "?tabIndex=" + ExpenseList.CurrentType + "&sv=1");
            } else {
                var tabTitle = HtmlLang.Write(LangModule.IV, "EditExpense", "Edit Expense");
                $.mTab.addOrUpdate(tabTitle, ExpenseList.url_Edit + "/" + MID + "?tabIndex=" + ExpenseList.CurrentType + "&sv=1");
            }
        }
        else {
            InitBills.AddExpenseDialog(MID);
        }

    },
    //刷新列表
    reloadData: function () {
        mWindow.reload(ExpenseList.url_List + ExpenseList.CurrentType);
    },
    reload: function (type) {
        ExpenseList.bindGrid(type);
        IVBase.tabSelRefreshTitle(type);
    }
}
//初始化页面
$(document).ready(function () {
    ExpenseList.init();
});