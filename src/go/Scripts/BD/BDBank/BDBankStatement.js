var BDBankStatement = {
    HasChangeAuth: false,
    InitStatementList: function (bankID) {
        BDBankStatement.HasChangeAuth = $("#hidChangeAuth").val() === "True";
        BDBankStatement.bindGrid(bankID);
        BDBankStatement.bindAction(bankID);
        new BDBankReconcileHome().getForm();
    },
    bindAction: function (bankID) {
        //删除记录
        $("#btnDeleteStatement").off("click").on("click", function () {
            //
            var array = [];
            //获取参数
            $(".row-key-checkbox[checked=checked]", $("#gridBankStatements").parent()).each(function () {
                array.push($(this).val());
            });
            if (array.length > 0) {
                //提醒内容
                var content = HtmlLang.Write(LangModule.Bank, "AreYouSureToDeleteStatementWithReconcileRecord", "Are you sure to delete selected statements with their reconciled records?");
                //弹出提醒
                $.mDialog.confirm(content, function () {
                    //删除
                    BDBankStatement.delteStatement(bankID);
                });
            } else {
                //如果没有股勾选的情况
                $.mDialog.alert(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
            }
        });
    },
    delteStatement: function (bankID) {
        //url
        var deleteUrl = "/BD/BDBank/DeleteBankbill";
        //
        var array = [];
        //获取参数
        $(".row-key-checkbox[checked=checked]", $("#gridBankStatements").parent()).each(function () {
            array.push($(this).val());
        });
        //获取参数
        mAjax.submit(deleteUrl, { mids: array }, function (msg) {
            //
            if (msg.Success || msg.Result) {
                //提醒删除成功
                mMsg(msg.Message);
                //刷新当前页面
                BDBankStatement.InitStatementList(bankID);
            }
            else {
                $.mDialog.error(msg.Message);
            }
        });
    },
    bindGrid: function (bankID) {
        var acctId = bankID;
        //获取选择的日期
        var dates = new BDBankReconcileHome().getUserSelectedDate();
        //
        Megi.grid("#gridBankStatements", {
            resizable: true,
            auto: true,
            checkOnSelect: false,
            pagination: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $("#gridBankStatementsDiv").width() - 5,
            height: ($("body").height() - $("#gridBankStatementsDiv").offset().top - 10),
            url: "/BD/BDBank/GetBDBankStatementsList",
            queryParams: {
                MBankID: acctId,
                StartDate: dates[0],
                EndDate: dates[1]
            },
            columns: [[
                //复选框
                {
                    title: '<input type=\"checkbox\" class="row-key-checkbox-all">', field: 'MID', width: 10, align: 'center', formatter: function (value, rowData, rowIndex) { return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rowData.MID + "\" >"; }
                },
                {
                    title: HtmlLang.Write(LangModule.Bank, "ImportDate", "Import Date"), field: 'MImportDate', width: 50, formatter: function (value, rowData, rowIndex) {
                        var title = HtmlLang.Write(LangModule.Bank, "Statement", "Statement");
                        switch (rowData.MBankTypeID) {
                            case "Cash":
                                title = HtmlLang.Write(LangModule.Bank, "CashStatement", "Cash Statement");
                                break;
                            case "Paypal":
                                title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Bank, "paypal", "PayPal"), title]);
                                break;
                            case "Alipay":
                                title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Bank, "alipay", "Alipay"), title]);
                                break;
                            default:
                                title = HtmlLang.Write(LangModule.Bank, "BankStatement", "Bank Statement");
                                break;
                        }
                        var url = "/BD/BDBank/BDBankStatementView?MID=" + rowData.MID + "&MBankID=" + acctId + "&StartDate=" + mDate.format(rowData.MStartDate) + "&EndDate=" + mDate.format(rowData.MEndDate);
                        return "<a href=\"javascript:void(0);\" onclick=\"$.mTab.addOrUpdate('" + title + "', '" + url + "', true);" + "\" >" + mDate.format(rowData.MImportDate) + "</a>";
                    }
                },
                { title: HtmlLang.Write(LangModule.Bank, "StartDate", "Start Date"), field: 'MStartDate', width: 50, formatter: mDate.formatter },
                { title: HtmlLang.Write(LangModule.Bank, "EndDate", "End Date"), field: 'MEndDate', width: 50, formatter: mDate.formatter },
                { title: HtmlLang.Write(LangModule.Bank, "StartBalance", "Start Balance"), field: 'MStartBalance', width: 50, align: "right", formatter: function (value, rec, rowIndex) { return mMath.toMoneyFormat(value) } },
                { title: HtmlLang.Write(LangModule.Bank, "EndBalance", "End Balance"), field: 'MEndBalance', width: 50, align: "right", formatter: function (value, rec, rowIndex) { return mMath.toMoneyFormat(value) } },
                { title: HtmlLang.Write(LangModule.Bank, "User", "User"), field: 'MUser', width: 50 },
                { title: HtmlLang.Write(LangModule.Bank, "ImportedFile", "Imported File"), field: 'MFileName', width: 50 },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Status", "Status"), field: 'MStatus', width: 50, align: "center", formatter: function (value, rec, rowIndex) {
                        return value;
                    }
                },
            ]],
            onLoadSuccess: function () {
                var isSmartVersion = $("#hideVerison").val();
                if (isSmartVersion=="True" || isSmartVersion==true) {
                    $("#gridBankStatements").datagrid("hideColumn", "MStatus");
                }

                BDBankStatement.bindClick();
            }
        });

    },
    //绑定Click事件
    bindClick: function(){
        $(".row-key-checkbox:visible").off("click").on("click", function (evt) {
            var $elem = $(evt.srcElement || evt.target);
            if ($elem.is(":checked")) {
                if ($(".row-key-checkbox:visible:not([disabled]):checked").length
                    == $(".row-key-checkbox:visible:not([disabled])").length) {
                    $(".row-key-checkbox-all:visible").attr("checked", "checked");
                }
            }
            else {
                $(".row-key-checkbox-all:visible").removeAttr("checked");
            }
        });
    }
}