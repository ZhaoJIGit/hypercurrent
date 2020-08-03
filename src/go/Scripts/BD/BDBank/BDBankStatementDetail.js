var BDBankStatementDetail = {
    HasChangeAuth: false,
    //查找相关
    functoolbar: ".statement",
    btnSearchdiv: ".btnSearchDiv",
    btnreset: '.search-reset',
    btnSearchclose: ".m-adv-search-close",
    divSearch: ".m-adv-search-statementdetail",
    txtAmountFrom: ".search-amount-from",
    txtAmountTo: ".search-amount-to",
    txtPayer: ".search-payer",
    txtDate: ".search-date",
    txtRef: ".search-ref",
    txtFrom: ".search-from",
    toFlag: ".search-toflag",
    exactAmount: ".search-exact",
    btnSearch: ".search-do",
    init: function (bankID) {
        BDBankStatementDetail.initDom();
        BDBankStatementDetail.HasChangeAuth = $("#hidChangeAuth").val() === "True";
        BDBankStatementDetail.bindGrid(bankID);


        $("#aMarkAsNonGenerateVoucher,#aMarkAsUnGenerateVoucher").off("click").on("click", function () {
            var statu = $(this).attr("status");
            Megi.grid("#gridBankStatementDetail", "optSelected", {
                url: "/BD/BDBank/UpdateBankBillVoucherStatus", msg: "", param: { status: statu }, callback: function (msg) {
                    if (msg.Success) {
                        var message = '';
                        if (statu == 2) {
                            message = HtmlLang.Write(LangModule.Bank, "MarkSuccess", "Mark successful!");
                        } else {
                            message = HtmlLang.Write(LangModule.Bank, "UnmarkSuccess", "Unmark successful!");
                        }
                        $.mMsg(message);
                        BDBankStatementDetail.bindGrid(bankID);
                    } else {
                        $.mAlert("<div>" + msg.Message + "</div>", function () {
                            BDBankStatementDetail.bindGrid(bankID);
                        }, 1, true);
                    }
                }
            });
        });

        //显示搜索div
        $(BDBankStatementDetail.btnSearchdiv, BDBankStatementDetail.functoolbar).off("click").on("click", function () {
            $(BDBankStatementDetail.btnSearchdiv, BDBankStatementDetail.functoolbar).hide();
            $(BDBankStatementDetail.divSearch).show();
            BDBankStatementDetail.resize();
        });

        //隐藏搜索div
        $(BDBankStatementDetail.btnSearchclose, BDBankStatementDetail.divSearch).off("click").on("click", function () {
            $(BDBankStatementDetail.btnSearchdiv, BDBankStatementDetail.functoolbar).show();
            $(BDBankStatementDetail.divSearch).hide();
            BDBankStatementDetail.resize();
        });

        $(BDBankStatementDetail.exactAmount, BDBankStatementDetail.divSearch).off("change").on("change", BDBankStatementDetail.exactAmountShow);

        //添加搜索的功能
        $(BDBankStatementDetail.btnSearch, BDBankStatementDetail.divSearch).off("click").on("click", function () {
            BDBankStatementDetail.bindGrid(bankID);
        });

        $(BDBankStatementDetail.btnreset, BDBankStatementDetail.divSearch).off("click").on("click", function () {
            $(BDBankStatementDetail.txtDate, BDBankStatementDetail.divSearch).datebox('setValue', '');
            $(BDBankStatementDetail.txtPayer, BDBankStatementDetail.divSearch).val('');
            $(BDBankStatementDetail.txtRef, BDBankStatementDetail.divSearch).val('');
            $(BDBankStatementDetail.txtFrom, BDBankStatementDetail.divSearch).combobox("setValue", '');
            $(BDBankStatementDetail.txtAmountFrom, BDBankStatementDetail.divSearch).numberbox("setValue", "");
            $(BDBankStatementDetail.txtAmountTo, BDBankStatementDetail.divSearch).numberbox("setValue", "");
            $(BDBankStatementDetail.exactAmount, BDBankStatementDetail.divSearch).prop("checked", false);
            $(BDBankStatementDetail.exactAmount, BDBankStatementDetail.divSearch).trigger("change");
        });

    },
    //初始化界面元素
    initDom: function () {
        $(BDBankStatementDetail.txtPayer, BDBankStatementDetail.divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Bank, "Payee", "收/付款人")).initHint();
        //支出\收入
        $(BDBankStatementDetail.txtFrom, BDBankStatementDetail.divSearch).combobox({
            width: 110,
            textField: 'text',
            valueField: 'value',
            data: [
                {
                    text: HtmlLang.Write(LangModule.Common, 'Received', "收入"),
                    value: "1"
                },
                {
                    text: HtmlLang.Write(LangModule.Common, 'Spent', "支出"),
                    value: "2"
                }
            ]
        });
    },
    //显示&隐藏 金额过滤项
    exactAmountShow: function () {
        if ($(BDBankStatementDetail.exactAmount, BDBankStatementDetail.divSearch).is(":checked")) {
            $(BDBankStatementDetail.txtAmountTo, BDBankStatementDetail.divSearch).hide();
            $(BDBankStatementDetail.toFlag, BDBankStatementDetail.divSearch).hide();
            $(BDBankStatementDetail.txtAmountFrom, BDBankStatementDetail.divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Common, "Amount", "Amount")).removeClass("has-hint").initHint();
        }
        else {
            $(BDBankStatementDetail.txtAmountTo, BDBankStatementDetail.divSearch).show();
            $(BDBankStatementDetail.toFlag, BDBankStatementDetail.divSearch).show();
            $(BDBankStatementDetail.txtAmountFrom, BDBankStatementDetail.divSearch).removeAttr("hint").attr("hint", HtmlLang.Write(LangModule.Common, "MinAmount", "最小金额")).removeClass("has-hint").initHint();
        }
    },
    getFiter: function () {
        var dates = new BDBankReconcileHome().getUserSelectedDate();
        var filter = {
            ExactDate: $(BDBankStatementDetail.txtDate, BDBankStatementDetail.divSearch).datebox("getValue"),
            TransAcctName: $(BDBankStatementDetail.txtPayer, BDBankStatementDetail.divSearch).val(),
            MDesc: $(BDBankStatementDetail.txtRef, BDBankStatementDetail.divSearch).val(),
            SrcFrom: $(BDBankStatementDetail.txtFrom, BDBankStatementDetail.divSearch).combobox("getValue"),
            AmountFrom: $(BDBankStatementDetail.txtAmountFrom, BDBankStatementDetail.divSearch).val(),
            AmountTo: $(BDBankStatementDetail.txtAmountTo, BDBankStatementDetail.divSearch).val(),
            IsExactAmount: $(BDBankStatementDetail.exactAmount, BDBankStatementDetail.divSearch).is(":checked"),
            StartDate: dates[0],
            EndDate: dates[1]
        };
        return filter;
    },
    resize: function () {
        Megi.grid("#gridBankStatementDetail", "resize", {
            height: ($("body").height() - $("#gridBankStatementDetailDiv").offset().top - 10)
        });
    },
    bindGrid: function (bankID) {
        var filter = BDBankStatementDetail.getFiter();
        filter.MBankID = bankID;
        //
        Megi.grid("#gridBankStatementDetail", {
            resizable: true,
            auto: true,
            checkOnSelect: false,
            pagination: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $("#gridBankStatementDetailDiv").width(),
            height: ($("body").height() - $("#gridBankStatementDetailDiv").offset().top - 10),
            url: "/BD/BDBank/GetBankStatementDetailList",
            queryParams: filter,
            columns: [[
                //复选框
                {
                    title: '<input type=\"checkbox\" class="row-key-checkbox-all">', field: 'MEntryID', width: 10, align: 'center', formatter: function (value, rec, rowIndex) {
                        if (rec.MVoucherStatus == 3 || !rec.MIsCanMark || (rec.MParentID != null && rec.MParentID != "")) {
                            return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MEntryID + "\" disabled='disabled' >";
                        } else {
                            return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MEntryID + "\" >";
                        }
                    }
                },
                //业务日期
                {
                    title: BDBankCashCoding.getSortColumnTitle(LangKey.Date), field: 'MDate', align: 'left', width: 30, sortable: true, formatter: function (value, rec, rowIndex) {
                        value = $.mDate.formatter(value);
                        if (rec.MParentID != null && rec.MParentID != "") {
                            return "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + value;
                        }
                        return value;
                    }
                }, {
                    title: BDBankCashCoding.getSortColumnTitle(HtmlLang.Write(LangModule.Bank, "Payee", "Receipt/payee")), sortable: true, field: 'MTransAcctName', width: 80, align: 'left', formatter: function (value, rec, rowIndex) {
                        if (value == null || value == "") {
                            value = "&nbsp;";
                        }
                        return value;
                    }
                },
                //备注
                {
                    title: LangKey.Reference, field: 'MUserRef', width: 80, formatter: function (value, rec, rowIndex) {
                        return value;
                    }
                },
                //付款金额
                {
                    title: LangKey.Spent, field: 'MSpentAmt', align: 'right', width: 40, formatter: function (value, rec, rowIndex) {
                        if (value == 0) {
                            return "";
                        }
                        return mMath.toMoneyFormat(rec.MSpentAmt)
                    }
                },
                //收款金额
                {
                    title: LangKey.Received, field: 'MReceivedAmt', align: 'right', width: 40, formatter: function (value, rec, rowIndex) {
                        if (value == 0) {
                            return "";
                        }
                        return mMath.toMoneyFormat(rec.MReceivedAmt)
                    }
                },
                //凭证状态
                {
                    title: HtmlLang.Write(LangModule.Bank, "VoucherStatus", "Voucher Status"), field: 'MVoucherStatus', align: 'center', sortable: true, width: 40, formatter: function (value, rec, rowIndex) {
                        if (value == 1) {
                            return "<span class='m-yellow'>" + HtmlLang.Write(LangModule.Bank, "Ungenerated", "Ungenerated") + "</span>"
                        } else if (value == 2) {
                            return "<span class='m-black'>" + HtmlLang.Write(LangModule.Bank, "Notgenerated", "Not generated") + "</span>"
                        } else {
                            var glNumber = "GL-" + rec.MVoucherNumber;
                            return "<a href='javascript:void(0)' onclick=\"$.mTab.addOrUpdate('" + glNumber + "', '/GL/GLVoucher/GLVoucherEdit?MItemID=" + rec.MVoucherID + "', true);\"><span class='m-green'>" + glNumber + "</span></a>"
                        }
                    }
                }
            ]],
            onLoadSuccess: function () {

                BDBankCashCoding.initTitleTooltip();

                BDBankStatementDetail.bindClick();
            }
        });

        Megi.grid("#gridBankStatementDetail", "resize");
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