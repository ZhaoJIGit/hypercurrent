
var PayRunBase = {
    dgId: null,
    internal: null,
    //用户增加项类型名称
    USER_ADD_ITEM: "UserAddItem",
    //用户扣除项类型名称
    USER_SUBTRACT_ITEM: "UserSubtractItem",
    userItemCount: 0,
    Status: {
        All: 0,
        Draft: 1,
        AwaitingPayment: 3,
        Paid: 4
    },
    groupList: [],
    salaryItemColumnList: function () {
        PayRunBase.getPayItemGroupList();

        var columns = [{
            title: PayRunBase.getDisplayName("BaseSalary"), field: 'BaseSalary', width: 100, align: 'right', sortable: true, formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        },
        {
            title: PayRunBase.getDisplayName("Allowance"), field: 'Allowance', width: 90, align: 'right', sortable: true, hidden: PayRunBase.isHidden("Allowance"), formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        },
        {
            title: PayRunBase.getDisplayName("Bonus"), field: 'Bonus', width: 90, align: 'right', sortable: true, hidden: PayRunBase.isHidden("Bonus"), formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        },
        {
            title: PayRunBase.getDisplayName("Commission"), field: 'Commission', width: 90, align: 'right', sortable: true, hidden: PayRunBase.isHidden("Commission"), formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        },
        {
            title: PayRunBase.getDisplayName("OverTime"), field: 'OverTime', width: 80, align: 'right', sortable: true, hidden: PayRunBase.isHidden("OverTime"), formatter: function (value, row, index) {
                return Megi.Math.toMoneyFormat(value);
            }
        }];

        //用户新增项
        columns = columns.concat(PayRunBase.getUserColumnList(PayRunBase.USER_ADD_ITEM));

        columns = columns.concat([
            {
                title: PayRunBase.getDisplayName("TaxAdjustment"), field: 'TaxAdjustment', width: 110, align: 'right', sortable: true, hidden: PayRunBase.isHidden("TaxAdjustment"), formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            {
                title: PayRunBase.getDisplayName("Attendance"), field: 'Attendance', width: 80, align: 'right', sortable: true, hidden: PayRunBase.isHidden("Attendance"), formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(Math.abs(value));
                }
            }]);


        //用户扣除项
        columns = columns.concat(PayRunBase.getUserColumnList(PayRunBase.USER_SUBTRACT_ITEM));

        columns = columns.concat([
            {
                title: PayRunBase.getDisplayName("Other"), field: 'Other', width: 70, align: 'right', sortable: true, hidden: PayRunBase.isHidden("Other"), formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(Math.abs(value));
                }
            },
            {
                title: HtmlLang.Write(LangModule.PA, "SSHFEmployee", "SS&HF(Employee)"), field: 'SSWithHFEmployee', width: 155, align: 'right', sortable: true, formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(Math.abs(value));
                }
            },
            {
                title: HtmlLang.Write(LangModule.PA, "SSHFEmployer", "SS&HF(Employer)"), field: 'SSWithHFEmployer', width: 155, align: 'right', sortable: true, formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            {
                title: HtmlLang.Write(LangModule.PA, "PretaxTotal", "Pre-tax Total"), field: 'SalaryBeforePIT', width: 100, align: 'right', sortable: true, formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            {
                title: HtmlLang.Write(LangModule.PA, "PIT", "PIT"), field: 'PIT', width: 70, align: 'right', sortable: true, hidden: PayRunBase.isHidden("PIT"), formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            {
                title: HtmlLang.Write(LangModule.PA, "NetSalary", "Net Salary"), field: 'SalaryAfterPIT', width: 100, align: 'right', sortable: true, formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(value);
                }
            },
            {
                title: HtmlLang.Write(LangModule.PA, "TotalSalary", "Total Salary"), field: 'TotalSalary', width: 100, align: 'right', sortable: true, formatter: function (value, row, index) {
                    return Megi.Math.toMoneyFormat(value);
                }
            }]);

        return columns;
    },
    //获取用户新增的工资列
    getUserColumnList: function (typeName) {
        var userColumns = [];
        $.each(PayRunBase.groupList, function (i, item) {
            if (item.MItemTypeName.indexOf(typeName) != -1) {
                PayRunBase.userItemCount++;
                var userField = typeName + item.MItemID;
                userColumns.push({
                    title: PayRunBase.getDisplayName(userField), field: userField, width: 80, align: 'right', sortable: true, formatter: function (value, row, index) {
                        return PayRunBase.getUserPayItemAmt(row, this.field);
                    }
                });
            }
        });

        return userColumns;
    },
    //获取用户新增工资项的金额
    getUserPayItemAmt: function (row, field) {
        var amt = 0;
        if (row.UserPayItemList) {
            $.each(row.UserPayItemList, function (j, userItem) {
                if (userItem.ItemTypeName == field) {
                    amt = userItem.Amount;
                    return false;
                }
            });
        }

        return Megi.Math.toMoneyFormat(Math.abs(amt));
    },
    hideDisableColumn: function (gridId, data, fields) {        
        if (data && data.rows.length > 0) {
            var disableItemList = data.rows[0].DisableItemList;
            var disabledItemTitle = HtmlLang.Write(LangModule.PA, "DisabledPayItemToolTips", "<b>该工资项目已禁用</b><br/>如需重新启用，请至系统设置->工资项目中进行设置");
            $.each(disableItemList, function (i, item) {
                if ($.inArray(item.MItemTypeName, fields) != -1) {
                    var dg = $('#' + gridId);

                    //设置数据行样式
                    var col = dg.datagrid('getColumnOption', item.MItemTypeName);
                    col.styler = function () {
                        return 'background-color:#F1F1F1';
                    };

                    for (var rowIdx = 0; rowIdx < data.rows.length; rowIdx++)
                    {
                        dg.datagrid('refreshRow', rowIdx);
                    }

                    //设置表头样式
                    var td = dg.datagrid('getPanel').find('div.datagrid-header td[field=' + item.MItemTypeName + ']');
                    td.css('background-color', '#F1F1F1');
                    td.tooltip({
                        "content": disabledItemTitle,
                        "delay": 0,
                        "track": true,
                        "fade": 250
                    });
                }
            });

            //设置禁用列的class
            var tdList = $(PayRunBase.dgId).treegrid('getPanel').find("div.datagrid-body td");
            $.each(tdList, function () {
                var color = $(this).css("background-color").toLowerCase();
                if (color === "#f1f1f1" || color === "rgb(241, 241, 241)") {
                    $(this).addClass("disable-field");
                }
            });
        }
    },
    batchPayment: function (ids, isFromEdit, isMergePay, rundId) {
        var urlParam = '';
        if (isFromEdit) {
            urlParam = '?IsFromEdit=' + isFromEdit;
        } else {
            urlParam = '?rundId=' + rundId;
        }

        if (isMergePay) {
            urlParam = '?isMergePay=' + isMergePay + "&rundId=" + rundId;
        }

        $.mDialog.show({
            mTitle: isFromEdit ? HtmlLang.Write(LangModule.IV, "NewPayment", "New Payment") : HtmlLang.Write(LangModule.IV, "NewBatchPayment", "New Batch Payment"),
            //mTop: window.pageYOffset || document.documentElement.scrollTop,
            mWidth: 700,
            mHeight: 400,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle",
            mContent: "iframe:" + '/IV/UC/BatchPayment' + urlParam,
            mPostData: { selectIds: ids, obj: "PayRun" }
        });
    },
    //获取所有可修改名称或可禁用的一级工资项目
    getPayItemGroupList: function () {
        mAjax.post("/PA/PayItem/GetSalaryGroupItemList", {}, function (list) {
            PayRunBase.groupList = list.filter(function (item) {
                return item.MItemType < 2000 || item.MItemType == 3010;
            });
        }, "", true, false);
    },
    //获取列名
    getDisplayName: function (fieldName) {
        //一级工资项目取数据多语言
        var payItem = PayRunBase.groupList.filter(function (item) {
            return item.MItemTypeName == fieldName;
        });
        if (payItem.length == 1) {
            return payItem[0].MName;
        }

        return HtmlLang.Write(LangModule.PA, fieldName, fieldName);
    },
    //如果预设工资项已删除，则隐藏
    isHidden: function (fieldName) {
        //一级工资项目取数据多语言
        var payItem = PayRunBase.groupList.filter(function (item) {
            return item.MItemTypeName == fieldName;
        });

        //如果找不到，表示已删除
        return payItem.length == 0;
    },
    //判断列表是否有未分配的宽度
    haveUnAssignWidth: function (columns) {
        //列表宽度
        var listWidth = $(".m-imain-content").width();
        var unAssignWidth = listWidth;
        var columnWidth = 0;
        var columnCnt = 0;
        //过滤隐藏列
        var visibleColumns = columns.filter(function (item) {
            return !item.hidden;
        });
        //计算未分配宽度
        $.each(visibleColumns, function (i, item) {
            unAssignWidth -= item.width;
        });

        //未分配宽度大于0，表示需要自适应宽度
        return unAssignWidth > 0;
    }
}