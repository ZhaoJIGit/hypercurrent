AccountDimensionSummary = {
    currentSelectedFilter: null,
    accountList: null,
    currencyList: null,
    isClick: null,
    lastFilter: null,
    filterSchemeList: null,
    init: function () {
        AccountDimensionSummary.initUI();
        AccountDimensionSummary.initAction();
        AccountDimensionSummary.initBaseData();
        AccountDimensionSummary.loadAccount();
        AccountDimensionSummary.loadCurrency();
        AccountDimensionSummary.initFilterData();

        AccountDimensionSummary.loadReportData();

    },
    initUI: function () {
        $("#divReportDetail").removeClass("report-content-gl").addClass("report-content-gl");
    },
    initAction: function () {
        $("#btnMoreFilter").click(function () {
            if (!AccountDimensionSummary.isMouseover) {
                AccountDimensionSummary.showMoreFilter();
            }
            AccountDimensionSummary.isClick = true;

        })

        $(".gl-rpt-search").click(function (event) {
            event = event || window.event;
            event.stopPropagation();
        });

        $("body").click(function (e) {

            if ($(e.srcElement).attr("id") == "btnMoreFilter") {
                return;
            }

            $(".gl-rpt-search").hide();
        });

        $("#aReportUpdate").click(function () {
            $("#btnOK").trigger("click");
        });


        $("#ckAccountDemesionSubTotal").off("click").on("click", function () {
            if ($(this).is(":checked")) {
                //禁用按照科目核算维度
                $("#ckbAccountSubTotal").removeAttr("checked").attr("disabled", "disabled");
            } else {
                $("#ckbAccountSubTotal").removeAttr("checked").removeAttr("disabled");
            }
            AccountDimensionSummary.setCkbCrossPeriodSubTotalStatus();
        });

        $("#ckbAccountSubTotal").off("click").on("click", function () {
            if ($(this).is(":checked")) {
                //禁用按照科目核算维度
                $("#ckAccountDemesionSubTotal").removeAttr("checked").attr("disabled", "disabled");
            } else {
                $("#ckAccountDemesionSubTotal").removeAttr("checked").removeAttr("disabled");
            }
            AccountDimensionSummary.setCkbCrossPeriodSubTotalStatus();
        });

        //条件重置
        $("#btnReset").off("click").on("click", function () {
            $("#cbxAccount").combotree("setValue", "");
            $("#cbxMCurrencyID").combobox("setValue", "");
            $("#ckAccountDemesionSubTotal").removeAttr("disabled").removeAttr("checked");
            $("#ckbAccountSubTotal").removeAttr("disabled").removeAttr("checked");
            $("#ckbCrossPeriodSubTotal").removeAttr("checked").attr("disabled", "disabled");
            $("#ckbMDisplayNoAccurrenceAmount").attr("checked", "checked");
            $("#ckbMDisplayZeorEndBalance").attr("checked", "checked");
            $("#ckbIncludeDisabledAccount").removeAttr("checked");
            $("#ckbIncludeUnapprovedVoucher").removeAttr("checked");
        });

        $("#btnOK").off("click").on("click", function () {
            var filterSchemeId = $("#cbxFilterScheme").combobox("getValue");
            if (!filterSchemeId) {
                var tips = HtmlLang.Write(LangModule.Report, "NoSelectFilterScheme", "请选择一个过滤方案！");
                $.mDialog.alert(tips);
                return;
            }

            if (!$("#cbxStartPeriod").combobox("getValue")) {
                var tips = HtmlLang.Write(LangModule.Report, "NoSelectStartPeriod", "没有选择开始期间！");
                $.mDialog.alert(tips);
                return;
            }

            if (!$("#cbxEndPeriod").combobox("getValue")) {

                var tips = HtmlLang.Write(LangModule.Report, "NoSelectEndPeriod", "没有选择结束期间！");
                $.mDialog.alert(tips);
                return;
            }
            UCReport.reload();
        });
    },
    showMoreFilter: function (isHide) {
        var dom = $(".gl-rpt-search");
        var display = dom.css("display") == "block";
        if (display || isHide) {
            dom.hide();
        } else {
            dom.show();
        }
    },
    initBaseData: function () {
        if (AccountDimensionSummary.accountList == null) {
            var url = "/BD/BDAccount/GetAccountList";
            $.mAjax.post(url, { ShowNumber: true, IsAll: true }, function (data) {
                if (data) {
                    AccountDimensionSummary.accountList = data;
                }
            }, false, true, false);
        }

        if (AccountDimensionSummary.currencyList == null) {

            var integratedStandardCurrency = HtmlLang.Write(LangModule.Report, "IntegratedStandardCurrency", "Integrated standard currency");

            var integratedStandardCurrencyOj = {
                MName: integratedStandardCurrency,
                MCurrencyID: "0"
            }

            //新增综合本位币
            AccountDimensionSummary.currencyList = new Array();
            AccountDimensionSummary.currencyList.push(integratedStandardCurrencyOj);


            url = "/BD/Currency/GetBDCurrencyList";
            $.mAjax.post(url, { isIncludeBase: true }, function (data) {
                if (data) {
                    AccountDimensionSummary.currencyList = AccountDimensionSummary.currencyList.concat(data);
                }
            }, false, true, false);
        }
    },
    loadAccount: function () {
        $("#cbxAccount").combotree({
            valueField: 'id',
            textField: 'text',
            data: AccountDimensionSummary.accountList
        });
    },
    loadCurrency: function () {
        $("#cbxMCurrencyID").combobox({
            valueField: 'MCurrencyID',
            textField: 'MName',
            data: AccountDimensionSummary.currencyList
        });
    },
    setCkbCrossPeriodSubTotalStatus: function () {
        var isCanCheck = $("#ckAccountDemesionSubTotal").is(":checked") || $("#ckbAccountSubTotal").is(":checked");

        if (isCanCheck) {
            $("#ckbCrossPeriodSubTotal").removeAttr("disabled");
        } else {
            $("#ckbCrossPeriodSubTotal").removeAttr("checked").attr("disabled", "disabled");
        }
    },
    initFilterData: function () {

        $("#cbxFilterScheme").mAddCombobox("filterScheme", {
            required: true,
            valueField: 'MItemID',
            textField: 'MName',
            url: '/Report/RptAccountDimensionSummary/GetFilterSchemeList?MReportType=43',
            onLoadSuccess: function (data) {
                AccountDimensionSummary.filterSchemeList = data;
            }
        }, {
            hasPermission: true,
            url: "/Report/RptAccountDimensionSummary/EditFilterScheme",
            ButtonList: [{
                buttonClass: "report-button-editblue",
                buttonName: HtmlLang.Write(LangModule.Report, "EditFilterScheme", "编辑过滤方案"),
                buttonEvent: function () {

                    $("#cbxFilterScheme").combobox("hidePanel");

                    //显示当前所有的过滤方案
                    var rows = $("#cbxFilterScheme").combobox("getData");

                    //如果没有，则跳转到新增
                    if (!rows || rows.length == 0) {
                        $.mDialog.show({
                            mTitle: HtmlLang.Write(LangModule.Report, "FilterScheme", "过滤方案"),
                            mDrag: "mBoxTitle",
                            mShowbg: true,
                            mWidth: 600,
                            mHeight: 400,
                            mContent: "iframe:/Report/RptAccountDimensionSummary/EditFilterScheme?id=" + AccountDimensionSummary.currentSelectedFilter,
                            mCloseCallback: [function () {
                                //去掉遮罩
                                //$("#su_popup_overlay").empty();
                                AccountDimensionSummary.initFilterData();
                            }]
                        });
                    } else {
                        //显示编辑窗口
                        var table = $("table", "#panelScheme");

                        $(table).empty();
                        var editLang = HtmlLang.Write(LangModule.Report, "Edit", "编辑");
                        var deleteLang = HtmlLang.Write(LangModule.Report, "Delete", "删除");

                        for (var i = 0; i < rows.length; i++) {
                            var row = rows[i];
                            var tr = "<tr filterid='" + row.MItemID + "'>" +
                                        "<td class='mg-data'>" + row.MName + "</td>" +
                                        "<td  class='mg-data edit-column'><a href='javascript:void(0);' class='mg-track-Rename'>&nbsp;</a><a href='javascript:void(0);'>" + editLang + "</a></td>" +
                                        "<td  class='mg-data delete-column'><a href='javascript:void(0);' class='mg-track-delete'>&nbsp;</a><a href='javascript:void(0);'>" + deleteLang + "</a></td>" +
                                     "</tr>";

                            $(table).append(tr);
                        }

                        $.mDialog.show({
                            mTitle: "",
                            mWidth: 600,
                            mHeight: 400,
                            mDrag: "mBoxTitle",
                            mShowbg: true,
                            mShowTitle: false,
                            mContent: "id:panelScheme",
                            mCloseCallback: [function () {
                                AccountDimensionSummary.initFilterData();
                            }]

                        });

                        //进行事件绑定
                        AccountDimensionSummary.bindEditClickEvent();
                    }

                }
            }]
        });
    },
    addOrEditFilterScheme: function (id) {
        var url = "iframe:/Report/RptAccountDimensionSummary/EditFilterScheme";
        if (id) {
            url += "?id=" + id;
        }
        $.mDialog.show({
            mTitle: HtmlLang.Write(LangModule.Report, "FilterScheme", "过滤方案"),
            mDrag: "mBoxTitle",
            mShowbg: true,
            mWidth: 560,
            mHeight: 450,
            mContent: url,
            mCloseCallback: [function () {
                AccountDimensionSummary.initFilterData();
            }]
        });
    },
    deleteFilterScheme: function (id) {
        var url = "/Report/RptAccountDimensionSummary/DeleteFilterScheme";

        $.mDialog.confirm(LangKey.AreYouSureToDelete, {
            callback: function () {
                mAjax.submit(url, { id: id }, function (data) {
                    if (data && data.Success > 0) {
                        var filterSchemeDom = $("#filterSchemeList").find("tr[filterid='" + id + "']");
                        filterSchemeDom.empty();
                        //重新加载下拉框数据
                        AccountDimensionSummary.reloadFilterSchemeCombobox();
                        var tips = HtmlLang.Write(LangModule.Report, "DeleteFilterSchemeSuccess", "删除过滤方案成功！");
                        $.mDialog.message(tips);
                    } else {
                        var tips = HtmlLang.Write(LangModule.Report, "DeleteFilterSchemeFail", "删除过滤方案失败！");
                        $.mDialog.alert(tips);
                    }
                });
            }
        });

    },
    bindEditClickEvent: function () {
        //绑定事件
        var dom = $("table", "#panelScheme");
        $(dom).find(".edit-column").off("click").on("click", function () {
            var filterSchemeId = $(this).parent().attr("filterid");

            $(".mCloseBox").trigger("click");

            AccountDimensionSummary.addOrEditFilterScheme(filterSchemeId);
        });

        $(dom).find(".delete-column").off("click").on("click", function () {
            var filterSchemeId = $(this).parent().attr("filterid");

            AccountDimensionSummary.deleteFilterScheme(filterSchemeId);
        });

        $("#btnAddFilterScheme").off("click").on("click", function () {
            AccountDimensionSummary.addOrEditFilterScheme();
        });

    },
    reloadFilterSchemeCombobox: function () {
        //重新加载下拉框数据
        $("#cbxFilterScheme").combobox("reload");
    },
    getFilter: function () {
        var filterSchemeId = $("#cbxFilterScheme").combobox("getValue");

        var filterScheme = {};

        filterScheme.FilterSchemeId = filterSchemeId;
        filterScheme.MAccountId = $("#cbxAccount").combotree("getValue");
        filterScheme.IsSubtotalByAccountDemension = $("#ckAccountDemesionSubTotal").is(":checked");
        filterScheme.IsSubtotalByAccount = $("#ckbAccountSubTotal").is(":checked");
        return filterScheme;
    },
    loadReportData: function (filterScheme) {
        var opts = {};
        opts.url = "/Report/RptAccountDimensionSummary/GetReportData";;
        opts.getFilter = function () {

            var filter = AccountDimensionSummary.getFilter();
            AccountDimensionSummary.lastFilter = filter;

            return filter;
        };
        opts.autoFillCell = false;

        opts.callback = function (msg) {
            $("table", "#divReportDetail").mTableResize({
                forceFit: false
            });

            var filter = msg == null || msg.Filter == null ? AccountDimensionSummary.lastFilter : msg.Filter;

            if (filter) {
                var startPeriod = filter.MStartPeroid;
                var endPeriod = filter.MEndPeroid;

                $("#cbxStartPeriod").combobox("setValue", startPeriod);
                $("#cbxEndPeriod").combobox("setValue", endPeriod);

                $("#cbxMCurrencyID").combobox("setValue", filter.MCurrencyID);

                if (filter.IncludeUnapprovedVoucher) {
                    $("#ckbIncludeUnapprovedVoucher").attr("checked", "checked");
                }

                if (filter.MDisplayNoAccurrenceAmount) {
                    $("#ckbMDisplayNoAccurrenceAmount").attr("checked", "checked");
                }

                if (filter.MDisplayZeorEndBalance) {
                    $("#ckbMDisplayZeorEndBalance").attr("checked", "checked");
                }

                if (filter.FilterSchemeId) {
                    $("#cbxFilterScheme").combobox("setValue", filter.FilterSchemeId);
                }
            }

            //隐藏条件
            AccountDimensionSummary.showMoreFilter(true);

        };
        UCReport.init(opts);

    }
}