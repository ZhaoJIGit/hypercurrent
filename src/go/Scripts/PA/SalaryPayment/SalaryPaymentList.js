var SalaryPaymentList = {
    payRunUrl: "/PA/SalaryPayment/PayRunList",
    currentUrl: "/PA/SalaryPayment/SalaryPaymentList/" + $("#hidRunId").val(),
    hasChangeAuth: $("#hidChangeAuth").val() == "True" ? true : false,
    interval: null,
    isDblClick: false,
    rundId: $("#hidRunId").val(),
    //Tab选项卡的索引号
    CurrentStatus: 0,
    init: function () {
        PayRunBase.dgId = "#salaryPaymentList";
        SalaryPaymentList.initTab();
        SalaryPaymentList.bindAction();
    },
    //初始化 Tab 选项卡
    initTab: function () {
        var status = Number($("#hidStatus").val());
        $(".m-extend-tabs").tabsExtend({
            initTabIndex: status,
            onSelect: function (index) {
                SalaryPaymentList.CurrentStatus = index;
                SalaryPaymentList.initToolBar();
                SalaryPaymentList.bindGrid();
                $(window).resize();
            }
        });
    },
    initToolBar: function () {
        switch (SalaryPaymentList.CurrentStatus) {
            case PayRunBase.Status.All:
                $("#aAddEmp,#aDelete,#aEmail,#aBatchPay,#divBatchPay,#divBatchPayBy,#divMergeBatchPay,#aUnApprove,#aApprove").hide();
                break;
            case PayRunBase.Status.Draft:
                $("#aAddEmp,#aDelete,#aApprove").show();
                $("#aEmail,#aBatchPay,#divBatchPay,#divBatchPayBy,#divMergeBatchPay,#aUnApprove").hide();
                break;
            case PayRunBase.Status.AwaitingPayment:
                $("#aEmail,#aBatchPay,#divBatchPay,#divBatchPayBy,#divMergeBatchPay,#aUnApprove").show();
                $("#divBatchPay").css("display", "none");
                $("#aAddEmp,#aDelete,#aApprove").hide();
                break;
            case PayRunBase.Status.Paid:
                $("#aAddEmp,#aDelete,#aBatchPay,#divBatchPay,#divBatchPayBy,#divMergeBatchPay,#aApprove,#aUnApprove").hide();
                $("#aEmail").show();
                break;

        }
    },
    bindAction: function () {
        $("#aCancel").off('click').on('click', function () {
            SalaryPaymentList.closeTab();
        });
        $("#aApprove").off('click').on('click', function () {
            Megi.grid("#salaryPaymentList", "optSelected", {
                callback: function (ids) {
                    SalaryPaymentList.saveModel(ids, PayRunBase.Status.AwaitingPayment);
                }
            });
        });
        $("#aUnApprove").off('click').on('click', function () {
            Megi.grid("#salaryPaymentList", "optSelected", {
                callback: function (ids) {
                    SalaryPaymentList.saveModel(ids, PayRunBase.Status.Draft);
                }
            });
        });
        $("#aEmail").off('click').on('click', function () {
            SalaryPaymentList.execCallBackAfterSelect(function (ids, names) {
                SalaryPaymentList.emailPayslip(ids, names);
            });
        });
        $("#aPreview").off('click').on('click', function () {
            SalaryPaymentList.execCallBackAfterSelect(function (ids, names) {
                var title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Preview", "Preview"), HtmlLang.Write(LangModule.PA, "SalaryList", "Salary List")]);
                var param = $.toJSON({ reportType: "SalaryPrint", jsonParam: escape($.toJSON(SalaryPaymentList.getParam(ids, names))) });
                Print.previewPrint(title, param);
            });
        });
        $("#aDelete").off('click').on('click', function () {
            Megi.grid("#salaryPaymentList", "optSelected", {
                callback: function (ids) {
                    $.mDialog.confirm(LangKey.AreYouSureToDelete,
                    {
                        callback: function () {
                            var param = {};
                            param.KeyIDs = ids;
                            mAjax.submit(
                                "/PA/SalaryPayment/SalaryPaymentDelete",
                                 { param: param },
                                 function (msg) {
                                     if (msg.Success) {
                                         SalaryPaymentList.reload(true);
                                     } else {
                                         $.mDialog.alert(msg.Message);
                                     }
                                 });
                        }
                    });
                }
            });
        });
        $("#aAddEmp").off('click').on('click', function () {
            $.mDialog.show({
                mTitle: HtmlLang.Write(LangModule.PA, "AddEmployee", "Add Employee"),
                mContent: "iframe:" + '/PA/SalaryPayment/EmployeeAdd/' + $("#hidRunId").val(),
                mWidth: 550,
                mHeight: 380,
                mShowbg: true,
                mShowTitle: true,
                mDrag: "mBoxTitle"
            });
        });
        //批量支付
        $("#aBatchPay,#divBatchPay").click(function () {
            Megi.grid("#salaryPaymentList", "optSelected", {
                callback: function (ids) {
                    PayRunBase.batchPayment(ids, false, false, SalaryPaymentList.rundId);
                }
            });
            return false;
        });
        //合并生成付款单
        $("#divMergeBatchPay").click(function () {
            var rows = $('#salaryPaymentList').datagrid('getSelections');
            if (rows.length < 2) {
                $.mDialog.alert(HtmlLang.Write(LangModule.PA, "SelectMultipleEmployees", "请选择多个员工！"));
                return false;
            }
            Megi.grid("#salaryPaymentList", "optSelected", {
                callback: function (ids) {
                    PayRunBase.batchPayment(ids, false, true, SalaryPaymentList.rundId);
                }
            });
            return false;
        });
    },
    refreshSummary: function () {
        //刷新Tab合计数据
        IVBase.tabSelRefreshTitle(4, $("#hidRunId").val());
    },
    execCallBackAfterSelect: function (callBack) {
        Megi.grid("#salaryPaymentList", "optSelected", {
            callback: function (ids) {
                var rows = $('#salaryPaymentList').datagrid('getRows');
                var names = [];
                var arrId = ids.split(',');
                for (var i = 0; i < rows.length; i++) {
                    if ($.inArray(rows[i].MID, arrId) != -1) {
                        names.push(rows[i].MEmployeeName);
                    }
                }
                if (callBack != undefined) {
                    callBack(ids, names.join(','));
                }
            }
        });
    },
    getParam: function (selectIds, empNames) {
        var jsonParam = {};
        jsonParam.ObjectIds = selectIds;
        jsonParam.MRunID = $("#hidRunId").val();
        jsonParam.SalaryMonth = $("#hidSalaryDate").val();
        jsonParam.ObjectNames = empNames;
        return jsonParam;
    },
    emailPayslip: function (selectIds, empNames) {
        var paramStr = "selectIds=" + selectIds + "&sendType=4&type=SalaryPrint&period=" + $("#hidSalaryMonth").val() + "&rptJsonParam=" + escape($.toJSON(SalaryPaymentList.getParam(selectIds, empNames)));
        var param = $.toJSON({ type: "SalaryPrint", param: paramStr });
        Print.selectPrintSetting(param);
    },
    closeTab: function () {
        var dataIndex = $(this).closest("iframe", parent.document).parent().attr("data-index");
        $.mTab.remove(dataIndex);
    },
    autoWidth: function (isShow, height) {
        var w = $(".m-imain-content").width();
        $(".m-pa-sp-list").css({ "width": w + "px" });
        try {
            $("#salaryPaymentList").datagrid('resize', {
                width: $(".m-pa-sp-list").width(),
                height: SalaryPaymentList.getAutoHeight()
            });
        } catch (exc) { }
        SalaryPaymentList.interval = setInterval(SalaryPaymentList.adjustListScroll, 100);
    },
    adjustListScroll: function () {
        //解决最后一列宽度不够的问题
        if ($(".m-pa-sp-list .datagrid-view").width() == $(".m-pa-sp-list").width()) {
            clearInterval(SalaryPaymentList.interval);
            return;
        }
        var totalW = $(".m-pa-sp-list").width();
        var frozenW = $(".m-pa-sp-list .datagrid-view1").width()
        var scrollW = totalW - frozenW;
        $(".m-pa-sp-list .datagrid-view").width(totalW);
        $(".m-pa-sp-list .datagrid-view2").width(scrollW);
        $(".m-pa-sp-list .datagrid-header").width(scrollW);
        $(".m-pa-sp-list .datagrid-body").width(scrollW);
        $(".m-pa-sp-list .datagrid-footer").width(scrollW);

        //解决只有一条记录时，水平滚动条盖住数据
        var scrollContainer = $(".m-pa-sp-list .datagrid-view2 .datagrid-body");
        var hScrollH = 18;
        if (scrollContainer.height() == 35) {
            scrollContainer.height(scrollContainer.height() + hScrollH);
            var dv = $(".m-pa-sp-list .datagrid-view");
            dv.height(dv.height() + hScrollH);

        }
    },
    saveModel: function (ids, status, callBack) {
        var obj = {};
        obj.MID = $("#hidRunId").val();
        obj.MStatus = status;
        obj.MSelectIds = ids;
        mAjax.submit("/PA/SalaryPayment/SavePayRun", { model: obj }, function (msg) {
            if (msg.Success) {
                if (callBack != undefined) {
                    callBack();
                }
                else {
                    if (status === PayRunBase.Status.AwaitingPayment) {
                        $.mMsg(HtmlLang.Write(LangModule.Common, "ApproveSuccessfully", "Approve Successfully!"));
                    }
                    else {
                        $.mMsg(HtmlLang.Write(LangModule.Common, "UnapproveSuccessfully", "Unpprove Successfully!"));
                    }
                    SalaryPaymentList.reloadPayRun(function () {
                        SalaryPaymentList.reload();
                        //mWindow.reload(SalaryPaymentList.currentUrl);
                    });
                }
            } else {
                $.mDialog.alert(msg.Message);
            }
        });
    },
    getEmployeeName: function (empid, callback) {
        $.mAjax.post("/BD/Employees/GetEmployeesEditData", { model: { MItemID: empid } }, function (data) {
            callback(data);
        })
    },
    editSalaryPayment: function (mid, empid) {
        //var title = mid == "" ? HtmlLang.Write(LangModule.PA, "AddSalaryPayment", "Add Salary Payment") : HtmlLang.Write(LangModule.PA, "EditSalaryPayment", "Edit Salary Payment");
        //$.mTab.addOrUpdate(title, "/PA/SalaryPayment/SalaryPaymentEdit/" + mid);

        SalaryPaymentList.getEmployeeName(empid, function (data) {
            $.mDialog.show({
                mTitle: mid == "" ? HtmlLang.Write(LangModule.PA, "AddSalaryPayment", "Add Salary Payment") :
                     data == null ? HtmlLang.Write(LangModule.PA, "EditSalaryPayment", "Edit Salary Payment") :
                     HtmlLang.Write(LangModule.PA, "PayslipFor", "Payslip for {0}").replace('{0}', mText.encode(data.Name)),
                mContent: "iframe:" + '/PA/SalaryPayment/SalaryPaymentEdit/' + mid,
                mShowbg: true,
                mShowTitle: true,
                mDrag: "mBoxTitle"
            });
        });
    },
    bindGrid: function (callBack) {
        var columnList = [
            {
                title: '<input type=\"checkbox\" >', field: 'MID', width: 25, align: 'center', formatter: function (value, rec, rowIndex) {
                    return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" />";
                }
            },
            {
                title: HtmlLang.Write(LangModule.Common, "RowNo", "No."), field: 'MRowNo', width: 50, sortable: true, align: 'center'
            },
            {
                title: HtmlLang.Write(LangModule.PA, "Name", "Name"), field: 'MEmployeeName', width: 136, align: 'left', sortable: true, formatter: function (value, row, index) {
                    return "<a href=\"javascript:void(0);\" onclick=\"SalaryPaymentList.editSalaryPayment('" + row.MID + "'" + ",'" + row.MEmployeeID + "')" + "\" >" + value + "</a>"
                }
            }];
        var isApproved = SalaryPaymentList.CurrentStatus >= PayRunBase.Status.AwaitingPayment;
        var salaryItemColumns = PayRunBase.salaryItemColumnList().slice();
        if (isApproved || SalaryPaymentList.CurrentStatus == PayRunBase.Status.All) {
            columnList.push({
                title: HtmlLang.Write(LangModule.IV, "Sent", "Sent"), field: 'MIsSent', width: 50, fixed: true, sortable: true, align: 'center', formatter: function (value, rec, rowIndex) {
                    return value == true ? "<span class=\"icon-flag-done\"></span>" : "";
                }
            });
            salaryItemColumns.splice(1, 0, {
                title: HtmlLang.Write(LangModule.IV, "Paid", "Paid"), field: 'MVerificationAmt', width: 100, align: 'right', sortable: true, formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(value);
                }
            });
        }
        columnList = columnList.concat(salaryItemColumns);
        if (!isApproved) {
            columnList.push({
                title: HtmlLang.Write(LangModule.PA, "Operation", "Operation"), field: 'Action', align: 'center', width: 80, sortable: false, formatter: function (value, rec, index) {
                    var actionHtml = "<div class='list-item-action'><a href=\"javascript:void(0);\" onclick=\"SalaryPaymentList.editSalaryPayment('" + rec.MID + "'" + ",'" + rec.MEmployeeID + "');\" class='list-item-edit'></a>";
                    if (SalaryPaymentList.hasChangeAuth && parseInt($("#hidStatus").val()) < 2) {
                        actionHtml += "<a href=\"javascript:void(0);\" onclick=\"SalaryPaymentList.delSalaryPayment('" + rec.MID + "'" + "," + index + ");\" class='list-item-del'></a></div>";
                    } else {
                        actionHtml += "</div>";
                    }

                    return actionHtml;
                }
            });
        }
        
        //是否有未分配的宽度
        var haveUnAssignWidth = PayRunBase.haveUnAssignWidth(columnList);

        //横向滚动条左边固定列
        var leftFixedFields = "MRowNo,MID,MEmployeeName,MIsSent,BaseSalary,MVerificationAmt".split(',');//,SalaryAfterPIT,TotalSalary
        var leftFrozenColumns = [];
        var removeIdx = [];

        //设置左边固定列
        var allFields = [];
        $.each(columnList, function (i, column) {
            allFields.push(column.field);
            if ($.inArray(column.field, leftFixedFields) != -1) {
                leftFrozenColumns.push(column);
                removeIdx.push(i);
            }
        });
        //设置右边滚动列
        removeIdx = removeIdx.reverse();
        $.each(removeIdx, function (i, idx) {
            columnList.splice(idx, 1);
        });

        var queryParam = {};
        queryParam.PayRunID = $("#hidRunId").val();
        queryParam.MStatus = SalaryPaymentList.CurrentStatus;
        Megi.grid("#salaryPaymentList", {
            resizable: true,
            //显示纵向滚动条
            scrollY: true,
            lines: true,
            //如果有未分配宽度，则自动分配列宽
            fitColumns: haveUnAssignWidth,
            auto: haveUnAssignWidth,
            url: "/PA/SalaryPayment/GetSalaryPaymentList",
            queryParams: queryParam,
            frozenColumns: [leftFrozenColumns],
            columns: [columnList],
            pagination: true,
            onLoadSuccess: function (data) {
                PayRunBase.hideDisableColumn("salaryPaymentList", data, allFields);
                SalaryPaymentList.autoWidth();
                if (callBack != undefined) {
                    callBack(data);
                }
            }
        });
    },
    reload: function (isFromDelete) {
        SalaryPaymentList.bindGrid(function (data) {
            if (data && (data.rows == null || data.rows.length === 0) && isFromDelete) {
                SalaryPaymentList.closeTab();
                $.mTab.addOrUpdate(HtmlLang.Write(LangModule.PA, "PayRun", "Pay Run"), SalaryPaymentList.payRunUrl, true, true);
            }
            else {
                SalaryPaymentList.refreshSummary();
                SalaryPaymentList.reloadPayRun();
            }
        });
    },
    reloadPayRun: function (callBack) {
        //刷新Pay Run
        var title = HtmlLang.Write(LangModule.PA, "PayRun", "Pay Run");
        $.mTab.refresh(title, SalaryPaymentList.payRunUrl + "?refresh=true", false);
        if (callBack != undefined) {
            callBack();
        }
    },
    getAutoHeight: function () {
        //var oriHeight = $(".m-pa-sp-list .datagrid-view").height();
        var selectorName = ".m-imain";
        var gridWrapDivHegith = $(selectorName).height() - $(".m-extend-tabs").outerHeight() - $(".m-tab-toolbar").outerHeight() - 40;
        //if (oriHeight > gridWrapDivHegith) {
        $(".datagrid-wrap", selectorName).height(gridWrapDivHegith);

        var gridViewDiv = $(".datagrid-view", selectorName);
        gridViewDiv.height(gridWrapDivHegith);

        var gridBody = $(".datagrid-body", selectorName);
        //数据显示的高度需要减去表头占用高度
        var gridBodyHeight = gridViewDiv.height() - 10;// - $(".datagrid-header", selectorName).height();
        //gridBody.height(gridBodyHeight);
        return gridBodyHeight;
        //}
        //return oriHeight;
    },
    delSalaryPayment: function (id, rowIndex) {
        //先选中该行
        Megi.grid("#salaryPaymentList", "selectRow", rowIndex);
        Megi.grid("#salaryPaymentList", "deleteSelected", {
            url: "/PA/SalaryPayment/SalaryPaymentDelete",
            callback: function (response) {
                if (response.Success) {
                    $.mMsg(HtmlLang.Write(LangModule.PA, "DeleteSalaryPaymentSuccess", "Delete Salary Payment Successfully"));
                    SalaryPaymentList.reload(true);
                } else {
                    $.mDialog.alert(response.Message);
                }
            }
        })
    }
}

$(document).ready(function () {
    SalaryPaymentList.init();

    $(window).resize(function () {
        SalaryPaymentList.autoWidth();
    });
});