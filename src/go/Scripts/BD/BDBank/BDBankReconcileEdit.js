/// <reference path="../../IV/BillUrlHelper.js" />
/// <reference path="../../IV/BillUrlHelper.js" />
var BDBankReconcileEdit = {
    MathBillIDList: '',
    init: function () {
        $.mDialog.max();
        BDBankReconcileEdit.setTitle();
        BDBankReconcileEdit.MathBillIDList = Megi.request("mathList");
        BDBankReconcileEdit.initTab();
        BDBankReconcileEdit.bindTransList(true);
        BDBankReconcileEdit.bindAction();
        BDBankReconcileEdit.bindMathInfo();
        BDBankReconcileEdit.initCreatePR();
    },
    setTitle: function () {
        $("div.boxTitle>h3", parent.document).text(HtmlLang.Write(LangModule.Bank, "ReconcileEdit", "Reconcile Edit"));
    },
    bindAction: function () {
        //移除后添加
        $("#txtKeyword,#txtAmount").off("keyup.ty").on("keyup.ty", function (event) {
            if (event.keyCode == 13) {
                $("#aSearch").click();

                //$("#aSearch").trigger("keyup.ty");
            }
        });

        $("#aSearch").click(function () {
            BDBankReconcileEdit.MathBillIDList = null;
            BDBankReconcileEdit.bindTransList();
        });
        $(".bankinfo").find(".more-info").click(function () {
            $(".statement-detail",".bankinfo").hide();
            var left = $(this).offset().left;
            var top = $(this).offset().top;
            var height = $(this).closest(".bankinfo").find(".statement-detail").height();
            Megi.popup("#aMore", { selector: "#adivMoreContent" });
        });

        //隐藏弹出的Div
        $(".statement-detail", ".bankinfo").find(".m-icon-close").click(function () {
            $(this).closest(".statement-detail").hide();
        });

        $("#txtAdjustment,#txtBankFeeAmount,#txtInterestAmount").keyup(function () {
            BDBankReconcileEdit.calAmount(null, false);
        }).blur(function () {
            BDBankReconcileEdit.calAmount(null, false);
        });
        $("#aClear").click(function () {
            $("#txtKeyword").val("");
            $("#txtAmount").numberbox("setValue", "");
            BDBankReconcileEdit.bindTransList();
        });
        $("#aNewSpendMoney").click(function () {
            BDBankReconcileEdit.newPayment("", "", "", "");
        });
        $("#aNewReceiveMoney").click(function () {
            BDBankReconcileEdit.newReceive("", "", "", "");
        });
        $("#aBankFee").click(function () {
            $("#divBankFee").show();
            $('.m-imain').scrollTop($('.m-imain')[0].scrollHeight);
        });
        $("#aHidBankFee").click(function () {
            $("#divBankFee").hide();
            $("#txtBankFeeAmount").numberbox("setValue", "");
            BDBankReconcileEdit.calAmount(null, false);
        });
        $("#aInterest").click(function () {
            $("#divInterest").show();
            $('.m-imain').scrollTop($('.m-imain')[0].scrollHeight);
        });
        $("#aHidInterest").click(function () {
            $("#divInterest").hide();
            $("#txtInterestAmount").numberbox("setValue", "");
            BDBankReconcileEdit.calAmount(null, false);
        });
        $("#aMinorAdjustment").click(function () {
            $("#divAdjustment").show();
            $('.m-imain').scrollTop($('.m-imain')[0].scrollHeight);
        });
        $("#hidAdjustment").click(function () {
            $("#divAdjustment").hide();
            $("#txtAdjustment").numberbox("setValue", "");
            BDBankReconcileEdit.calAmount(null, false);
        });

        $("#aNewTransferMoney").click(function () {
            //弹窗
            $.mDialog.show({
                mTitle: HtmlLang.Write(LangModule.Bank, "NewTransfer", "New Transfer"),
                mWidth: 1000,
                mHeight: 450,
                mDrag: "mBoxTitle",
                mShowbg: true,
                mContent: "iframe:" + "/IV/IVTransfer/IVTransferHome?acctid=" + Megi.request("bankId") + "&dialog=1",
                mCloseCallback: [function (param) {
                    parent.$.mDialog.min();
                    BDBankReconcileEdit.afterNewPR(param);
                }]
            });
        });

        //新建匹配规则按钮点击
        $("#createRuleBtn").click(function () {
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Bank, "NewMapRule", "New Rule"),
                width: 480,
                height: 350,
                href: "/BD/BDBank/BDBankRuleEdit?type=new"
            });
        });


        $(".bankinfo>.info>.flag>.m-icon-delete").click(function () {
            var bankBillEntryId = $(this).attr("billId");
            var obj = {};
            obj.KeyIDs = bankBillEntryId;
            Megi.confirm(LangKey.AreYouSureToDelete, function () {
                mAjax.submit(
                    "/BD/BDBank/BDBankCashCoding/DeleteCashCoding",
                    { param: obj },
                    function () {
                        parent.Megi.displaySuccess("#divMessage", HtmlLang.Write(LangModule.Bank, "DeleteSuccessfully", "Delete Successfully!"));
                        parent.BankRecList.bindList();
                        parent.Megi.closeDialog();
                    });
            })
        });
    },
    initCreatePR: function () {
        if (Megi.request("pr") != "1") {
            return;
        }
        var recModel = eval("(" + $("#hidRecModel").val() + ")");
        if (recModel.MSpentAmt > 0) {
            BDBankReconcileEdit.newPayment();
        } else if (recModel.MReceivedAmt > 0) {
            BDBankReconcileEdit.newReceive();
        }
    },
    newPayment: function () {

        var contactId = BDBankReconcileEdit.getContactId();
        var desc = Megi.request("desc");
        var recModel = eval("(" + $("#hidRecModel").val() + ")");
        if (desc.length == 0) {
            desc = recModel.MDesc;
        }

        $.mDialog.show({
            title: HtmlLang.Write(LangModule.IV, "SpendMoney", "Spend Money"),
            width: 850,
            height: 350,
            href: "/IV/Payment/PaymentCreateByBankBill?acctid=" + Megi.request("bankId") + "&amt=" + recModel.MSpentAmt + "&cttId=" + contactId + "&date=" + recModel.MDate + "&desc=" + escape(encodeURIComponent(desc))
        });
    },
    //获取匹配的联系人ID
    getContactId: function () {
        //优先取url传过来的，没传则取自动匹配到的
        var contactId = Megi.request("cttId") || $("#hidContactID").val();
        if (contactId) {
            var arrSplit = contactId.split('_');
            contactId = arrSplit[0];
        }

        return contactId;
    },
    newReceive: function () {

        var contactId = BDBankReconcileEdit.getContactId();
        var desc = Megi.request("desc");
        var recModel = eval("(" + $("#hidRecModel").val() + ")");
        if (desc.length == 0) {
            desc = recModel.MDesc;
        }
        Megi.dialog({
            title: HtmlLang.Write(LangModule.IV, "ReceiveMoney", "Receive Money"),
            width: 850,
            height: 350,
            href: "/IV/Receipt/ReceiptCreateByBankBill?acctid=" + Megi.request("bankId") + "&amt=" + recModel.MReceivedAmt + "&cttId=" + contactId + "&date=" + recModel.MDate + "&desc=" + escape(encodeURIComponent(desc))
        });
    },
    initTransSelectedList: function () {
        var mathBillIds = BDBankReconcileEdit.MathBillIDList;
        if (mathBillIds == undefined || mathBillIds == null || mathBillIds.length == 0) {
            return;
        }
        var billIds = mathBillIds.split(',');
        var rows = Megi.grid("#tbTranList", "getRows");
        for (var i = 0; i < rows.length; i++) {
            for (var k = 0; k < billIds.length; k++) {
                if (rows[i].MBillID == billIds[k]) {
                    Megi.grid("#tbTranList", "selectRow", i);
                    break;
                }
            }
        }
    },
    afterNewPR: function (mathBillIds) {
        BDBankReconcileEdit.MathBillIDList = mathBillIds;
        BDBankReconcileEdit.bindTransList(mathBillIds);
    },
    bindTransList: function (isInit) {
        Megi.grid("#tbTranList", {
            resizable: true,
            auto: true,
            scrollY: true,
            url: "/BD/BDBank/GetBDBankReconcileTranstionList",
            queryParams: {
                BizObject: $("#hidBizOjbect").val(),
                Keyword: $("#txtKeyword").val(),
                MAmount: $("#txtAmount").val(),
                MBankID: Megi.request("bankId"),
                MBankBillDate: $("#hidBankBillDate").val()
            },
            height: 240,
            columns: BDBankReconcileEdit.getColumns(true),
            onClickCell: function (rowIndex, field, value) {
                if (field == "MBillID") {
                    BDBankReconcileEdit.MathBillIDList = null;
                    BDBankReconcileEdit.bindSelectedTransList(value);
                }
            },
            onLoadSuccess: function () {
                if (isInit) {
                    BDBankReconcileEdit.initTransSelectedList();
                    BDBankReconcileEdit.bindSelectedTransList();
                    BDBankReconcileEdit.bindTransList();
                }
                $(window).resize();
                var selectRows = new Array();
                var rows = Megi.grid("#tbTranList", "getRows");
                if ($("#tbSelectedTranList").parent().hasClass("datagrid-view")) {
                    selectRows = Megi.grid("#tbSelectedTranList", "getSelections");
                }
                rows = rows == null ? new Array() : rows;
                selectRows = selectRows == null ? new Array() : selectRows;
                if (selectRows.length > 0) {
                    for (var i = 0; i < rows.length; i++) {
                        for (var j = 0; j < selectRows.length; j++) {
                            if (rows[i].MBillID == selectRows[j].MBillID) {
                                //勾对的查找界面，先拆分再新增收/付款单后，拆分失效
                                //依据已经选中的行更新数据
                                Megi.grid("#tbTranList", "updateRow", {
                                    index: i,
                                    row: {
                                        MSplitReceiveAmtFor: selectRows[j].MSplitReceiveAmtFor,
                                        MSplitReceivedAmt: selectRows[j].MSplitReceivedAmt,
                                        MSplitSpentAmt: selectRows[j].MSplitSpentAmt,
                                        MSplitSpentAmtFor: selectRows[j].MSplitSpentAmtFor,
                                    }
                                });
                                Megi.grid("#tbTranList", "selectRow", i);
                            }
                        }
                    }
                }
                BDBankReconcileEdit.bindSelectedTransList();
            }
        });
    },
    bindSelectedTransList: function (billId) {
        var data = new Array();
        var srcRows = new Array();
        var rows = Megi.grid("#tbTranList", "getSelections");
        if ($("#tbSelectedTranList").parent().hasClass("datagrid-view")) {
            srcRows = Megi.grid("#tbSelectedTranList", "getSelections");
        }
        rows = rows == null ? new Array() : rows;
        srcRows = srcRows == null ? new Array() : srcRows;
        //删除当前Bill
        if (billId != undefined && billId != "") {
            var arr = new Array();
            for (var i = 0; i < srcRows.length; i++) {
                if (srcRows[i].MBillID != billId) {
                    arr.push(srcRows[i]);
                }
            }
            srcRows = arr;
        }
        for (var i = 0; i < srcRows.length; i++) {
            var row = srcRows[i];
            for (var j = 0; j < rows.length; j++) {
                if (row.MBillID == rows[j].MBillID) {
                    row.MSplitReceiveAmtFor = rows[j].MSplitReceiveAmtFor;
                    row.MSplitReceivedAmt = rows[j].MSplitReceivedAmt;
                    row.MSplitSpentAmt = rows[j].MSplitSpentAmt;
                    row.MSplitSpentAmtFor = rows[j].MSplitSpentAmtFor;
                    break;
                }
            }
            data.push(row);
        }
        for (var i = 0; i < rows.length; i++) {
            var isExists = false;
            for (var j = 0; j < srcRows.length; j++) {
                if (rows[i].MBillID == srcRows[j].MBillID) {
                    isExists = true;
                }
            }
            if (!isExists) {
                data.push(rows[i]);
            }
        }
        Megi.grid("#tbSelectedTranList", {
            resizable: true,
            auto: true,
            scrollY: true,
            height: 192,
            data: data,
            columns: BDBankReconcileEdit.getColumns(false),
            onClickRow: function (rowIndex, rowData) {
                BDBankReconcileEdit.MathBillIDList = null;
                Megi.grid("#tbSelectedTranList", "deleteRow", rowIndex);
                BDBankReconcileEdit.deleteSelectedTrans(rowData.MBillID);
            },
            onLoadSuccess: function () {
                Megi.grid("#tbSelectedTranList", "selectAll");
            }
        });

        var mathBillIds = BDBankReconcileEdit.MathBillIDList;
        if (mathBillIds != undefined && mathBillIds != null && mathBillIds.length > 0) {
            Megi.grid("#tbSelectedTranList", "selectAll");
        }
        BDBankReconcileEdit.calAmount(data, true);
        $(window).resize();
    },
    deleteSelectedTrans: function (billId) {
        var rows = Megi.grid("#tbTranList", "getRows");
        if (rows.length > 0) {
            for (var i = 0; i < rows.length; i++) {
                if (rows[i].MBillID == billId) {
                    var rowData = rows[i];
                    rowData.MSplitSpentAmtFor = rowData.MSpentAmtFor;
                    rowData.MSplitReceiveAmtFor = rowData.MReceiveAmtFor;;

                    Megi.grid("#tbTranList", "updateRow", { index: i, row: rowData });
                    Megi.grid("#tbTranList", "unselectRow", i);
                    BDBankReconcileEdit.calAmount(null, true);
                    break;
                }
            }
        } else {
            BDBankReconcileEdit.calAmount(null, true);
        }
    },
    calAmount: function (rows, isCalAdjustAmt) {
        if (rows == undefined || rows == null) {
            rows = Megi.grid("#tbSelectedTranList", "getSelections");
        }
        $("#divAdjustAmt").hide();
        $("#spTotal").html("0.00");
        $("#spRecTotal").html("0.00");
        $("#aReconcile").unbind().removeClass("easyui-linkbutton-yellow").addClass("easyui-linkbutton-gray");
        if (rows == null || rows.length == 0) {
            BDBankReconcileEdit.bindMathInfo();
        }
        var totalAmt = 0;
        for (var i = 0; i < rows.length; i++) {
            totalAmt += Number(rows[i].MSplitSpentAmtFor) > 0 ? Number(rows[i].MSplitSpentAmtFor) : Number(rows[i].MSplitReceiveAmtFor);
        }
        var ajustAmt = $("#txtAdjustment").val();
        if (ajustAmt == "" || isNaN(ajustAmt)) {
            ajustAmt = 0;
        } else {
            ajustAmt = Number(ajustAmt);
        }
        if ($("#divAdjustment").is(':hidden')) {
            ajustAmt = 0;
        }
        var bankFeeAmt = $("#txtBankFeeAmount").val();
        if (bankFeeAmt == "" || isNaN(bankFeeAmt)) {
            bankFeeAmt = 0;
        } else {
            bankFeeAmt = Number(bankFeeAmt);
        }
        if ($("#divBankFee").is(':hidden')) {
            bankFeeAmt = 0;
        }

        var interestAmt = $("#txtInterestAmount").val();
        if (interestAmt == "" || isNaN(interestAmt)) {
            interestAmt = 0;
        } else {
            interestAmt = Number(interestAmt);
        }
        if ($("#divInterestAmt").is(':hidden')) {
            interestAmt = 0;
        }

        var recTotalAmt = Megi.Math.toDecimal((totalAmt + ajustAmt + bankFeeAmt + interestAmt), 2);
        totalAmt = Megi.Math.toDecimal(totalAmt, 2);

        $("#spTotal").html(Megi.Math.toMoneyFormat(totalAmt));
        $("#spRecTotal").html(Megi.Math.toMoneyFormat(recTotalAmt));

        var actualTotalAmt = Number($("#spActualTotal").html().replace(/,/g, ""));
        var lessAmt = actualTotalAmt - Number(recTotalAmt);
        if (lessAmt == 0) {
            $("#divAdjustAmt").hide();
            $("#aReconcile").addClass("easyui-linkbutton-yellow").removeClass("easyui-linkbutton-gray").click(function () {
                BDBankReconcileEdit.addRec();
            });
        } else {
            $("#spAdjustAmt").html(Megi.Math.toMoneyFormat(lessAmt, 2));
            $("#divAdjustAmt").show();
        }
        BDBankReconcileEdit.bindMathInfo();
    },
    addRec: function () {
        var recModel = eval("(" + $("#hidRecModel").val() + ")");
        var obj = {};
        obj.MBankBillEntryID = recModel.MEntryID;
        obj.MSpentAmtFor = recModel.MSplitSpentAmt;
        obj.MReceiveAmtFor = recModel.MSplitReceivedAmt;
        obj.MBankID = Megi.request("bankId");
        obj.MAdjustAmtFor = $("#txtAdjustment").numberbox("getValue");
        obj.MBankFeeAmtFor = $("#txtBankFeeAmount").numberbox("getValue");
        obj.MInterestAmtFor = $("#txtInterestAmount").numberbox("getValue")

        var bankBillDate = $("#hidBankBillDate").val();
        var ref = $("#hidBankRef").val().length == 0 ? $("#hidBankRef").val() : $("#hidBankDesc").val()

        var rows = Megi.grid("#tbSelectedTranList", "getRows");
        var arr = new Array();
        for (var i = 0; i < rows.length; i++) {
            var item = rows[i];
            var entry = {};
            entry.MTargetBillID = item.MBillID;
            entry.MSpentAmtFor = item.MSplitSpentAmtFor;
            entry.MReceiveAmtFor = item.MSplitReceiveAmtFor;
            entry.MTargetBillType = item.MTargetBillType;
            entry.MDate = bankBillDate;
            arr.push(entry);
        }
        if (obj.MAdjustAmtFor != 0) {
            var entry = {};
            if (recModel.MReceivedAmt > 0) {
                entry.MSpentAmtFor = 0;
                entry.MReceiveAmtFor = obj.MAdjustAmtFor;
            } else {
                entry.MSpentAmtFor = obj.MAdjustAmtFor;
                entry.MReceiveAmtFor = 0;
            }
            entry.MIsAdjustAmt = true;
            entry.MDesc = $("#hidRecDesc").val();
            entry.MRef = $("#txtAdjustmentDesc").val();
            entry.MDate = bankBillDate;
            arr.push(entry);
        }
        if (obj.MInterestAmtFor != 0) {
            var entry = {};
            if (recModel.MReceivedAmt > 0) {
                entry.MSpentAmtFor = 0;
                entry.MReceiveAmtFor = obj.MInterestAmtFor;
            } else {
                entry.MSpentAmtFor = obj.MInterestAmtFor;
                entry.MReceiveAmtFor = 0;
            }
            entry.MIsInterestAmt = true;
            entry.MDesc = $("#hidRecDesc").val();
            entry.MRef = $("#txtInterestDesc").val();
            entry.MDate = bankBillDate;
            arr.push(entry);
        }

        if ($("#divBankFee").is(':visible')) {
            var result = $("#divBankFee").mFormValidate();
            if (!result) {
                return;
            }

            if (obj.MBankFeeAmtFor != 0) {
                var entry = {};
                if (recModel.MReceivedAmt > 0) {
                    entry.MSpentAmtFor = 0;
                    entry.MReceiveAmtFor = obj.MBankFeeAmtFor;
                } else {
                    entry.MSpentAmtFor = obj.MBankFeeAmtFor;
                    entry.MReceiveAmtFor = 0;
                }
                entry.MIsBankFeeAmt = true;
                entry.MDesc = $("#hidRecDesc").val();
                entry.MRef = $("#txtBankFeeDesc").val();
                entry.MDate = bankBillDate;
                //entry.MContactID = $("#selContact").combobox("getValue");
                arr.push(entry);
            }
        }

        obj.RecEntryList = arr;
        mAjax.submit(
            "/BD/BDBank/UpdateBDBankBillReconcile",
            { model: [obj] },
            function (msg) {
                if (msg.Success) {
                    mMsg(HtmlLang.Write(LangModule.Bank, "Reconciled", "Reconciled"));
                    $.mDialog.close(true);
                } else {
                    $.mAlert(msg.Message, function () {
                        BDBankReconcileEdit.MathBillIDList = null;
                        BDBankReconcileEdit.bindTransList();
                    });
                }
            });
    },
    getColumns: function (isAllData) {
        var spentField = isAllData ? "MSpentAmtFor" : "MSplitSpentAmtFor";
        var receiveField = isAllData ? "MReceiveAmtFor" : "MSplitReceiveAmtFor";
        var attrCheck = isAllData ? "" : " checked='checked' ";
        var arr = [[{
            title: '&nbsp;', field: 'MBillID', formatter: function (value, rec, rowIndex) {
                return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\" " + attrCheck + " >";
            }, width: 20, align: 'center', sortable: false
        },
            { title: LangKey.Date, field: 'MBizDate', width: 50, sortable: true, formatter: $.mDate.formatter },
            {
                title: LangKey.Description, field: 'MBankAccountName', width: 80, sortable: true, formatter: function (value, rec, rowIndex) {
                    value = value == null ? "" : value;
                    if (value != "") {
                        value = rec.MDescription + "(" + value + ")";
                    } else {
                        value = rec.MDescription;
                    }
                    value = value.replace("Receive:", HtmlLang.Write(LangModule.Common, "Receive_Sale", "Receive") + " : ");
                    value = value.replace("Payment:", HtmlLang.Write(LangModule.Common, "Pay_Purchase", "Payment") + " : ");
                    value = value.replace("Interest:", HtmlLang.Write(LangModule.Bank, "Interest", "Interest") + " : ");
                    value = value.replace("Bank fee:", HtmlLang.Write(LangModule.Bank, "BankFees", "Bank fee") + " : ");
                    value = value.replace("Reconciliation adjustment:", HtmlLang.Write(LangModule.Bank, "ReconciliationAdjustment", "Reconciliation adjustment") + " : ");
                    return value;
                }
            },
            {
                title: HtmlLang.Write(LangModule.Bank, "MReference", "Ref/Number"), field: 'MReference', width: 80, sortable: true, formatter: function (value, rec, rowIndex) {
                    value = value == null ? "" : value;
                    //没有备注就全部不显示 为空
                    //if (rec.MReference != null && rec.MReference != "") {
                    var number = rec.MNumber;
                    if (rec.MType != "Invoice_Sale" && rec.MType != "Invoice_Sale_Red") {
                        number = "";
                    }
                    if (number == null || number == "") {
                        return value;
                    }
                    return "[ " + number + " ] " + value;
                    //}
                }
            },
            {
                title: HtmlLang.Write(LangModule.IV, "Type", "Type"), field: 'MTargetBillType', width: 50, sortable: true, formatter: function (value, rec, rowIndex) {
                    if (rec.MType == null || rec.MType == "") {
                        return eval("LangKey." + rec.MTargetBillType);
                    } else {
                        return eval("LangKey." + rec.MType);
                    }
                }
            },
            {
                title: HtmlLang.Write(LangModule.Bank, "Spent", "Spent"), align: 'right', field: spentField, width: 60, sortable: true, formatter: function (value, rec, rowIndex) {
                    return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 2);
                }
            },
            {
                title: HtmlLang.Write(LangModule.Bank, "Received", "Received"), align: 'right', field: receiveField, width: 60, sortable: true, formatter: function (value, rec, rowIndex) {
                    return value == 0 ? "" : Megi.Math.toMoneyFormat(value, 2);
                }
            }]];
        if (isAllData) {
            arr[0].push({
                title: HtmlLang.Write(LangModule.Bank, "SplitAmount", "Split Amount"), field: 'MSplitAmount', width: 40, align: 'right', sortable: false, formatter: function (value, rec, rowIndex) {
                    if (rec.MSpentAmtFor > 0) {
                        if (rec.MSpentAmtFor == rec.MSplitSpentAmtFor) {
                            return "";
                        } else {
                            return rec.MSplitSpentAmtFor;
                        }
                    } else {
                        if (rec.MReceiveAmtFor == rec.MSplitReceiveAmtFor) {
                            return "";
                        } else {
                            return rec.MSplitReceiveAmtFor;
                        }
                    }
                }
            });

            arr[0].push({
                title: "", field: 'MSplitTrans', width: 40, sortable: true, formatter: function (value, rec, rowIndex) {
                    var amtFor = rec.MSpentAmtFor > 0 ? rec.MSpentAmtFor : rec.MReceiveAmtFor;
                    var splitAmtFor = rec.MSpentAmtFor > 0 ? rec.MSplitSpentAmtFor : rec.MSplitReceiveAmtFor;
                    if (splitAmtFor == undefined || splitAmtFor == "") {
                        splitAmtFor = 0;
                    }
                    var html = '';
                    html += "<a href='javascript:void(0)' onclick='BDBankReconcileEdit.SplitTrans(" + amtFor + "," + splitAmtFor + "," + rowIndex + ")'>" + HtmlLang.Write(LangModule.Bank, "Split", "Split") + "</a>";
                    html += "<a href='javascript:void(0)' onclick='BDBankReconcileEdit.ViewTargetBillDetail(\"" + rec.MBillID + "\",\"" + rec.MType + "\")'>" + HtmlLang.Write(LangModule.Bank, "View", "View") + "</a>";
                    return html;
                }
            });
        }
        return arr;
    },
    SplitTrans: function (amtFor, splitAmtFor, rowIndex) {
        $.mDialog.show({
            title: HtmlLang.Write(LangModule.Bank, "SplitTransaction", "Split transaction"),
            width: 450,
            height: 250,
            href: "/BD/BDBank/BDBankReconcileSplitEdit?totalAmtFor=" + amtFor + "&splitAmtFor=" + splitAmtFor + "&rowIndex=" + rowIndex
        });
    },
    //查看单据的详细信息
    ViewTargetBillDetail: function (billId, bizType, ref) {
        BillUrl.open({ BillID: billId, BillType: bizType, Ref: ref });
    },
    AfterSplitTran: function (param) {
        var rows = Megi.grid("#tbTranList", "getRows");
        var newRow = rows[param.RowIndex];
        if (newRow.MSpentAmtFor > 0) {
            newRow.MSplitSpentAmtFor = param.SplitAmtFor;
        } else {
            newRow.MSplitReceiveAmtFor = param.SplitAmtFor;
        }
        var rowData = Megi.grid("#tbTranList", "updateRow", { index: param.RowIndex, row: newRow });
        if (param.SplitAmtFor == 0) {
            return;
        }
        Megi.grid("#tbTranList", "selectRow", param.RowIndex);
        BDBankReconcileEdit.bindSelectedTransList();
    },
    initTab: function () {
        $(".m-match-tab").each(function () {
            var index = 0;
            $(this).find(".m-match-tab-header>ul>li").each(function (i) {
                $(this).attr("index", i);
                if ($(this).hasClass("current")) {
                    index = i;
                }
                $(this).removeClass("current");
            });
            $(this).find(".m-match-tab-header>ul>li").eq(index).addClass("current");
            $(this).find(".m-match-tab-content>.content").hide();
            $(this).find(".m-match-tab-content>.content").eq(index).show();
            $(this).find(".m-match-tab-header>ul>li>a").unbind();
            $(this).find(".m-match-tab-header>ul>li>a").click(function () {
                $(this).closest(".m-match-tab").find(".m-match-tab-header>ul>li").removeAttr("class").css({ "border-bottom": "1px solid #BAE58C" });
                var currentIndex = Number($(this).parent().attr("index"));
                var extCls = $(this).parent().attr("data-extcls");
                if (extCls != undefined && extCls != undefined && extCls != "") {
                    $(this).parent().addClass(extCls).addClass("current");
                } else {
                    $(this).parent().addClass("current");
                }
                $(this).closest(".m-match-tab").find(".m-match-tab-content>.content").hide();
                $(this).closest(".m-match-tab").find(".m-match-tab-content>.content").eq(currentIndex).show();
            });
        });
    },
    bindMathInfo: function () {
        $(".m-match-tab-header").find("li").eq(0).removeClass("match").css({ "border-bottom": "1px solid #ECF5FF" });
        $("#divMathInfo").addClass("match-tip").html("<span>" + HtmlLang.Write(LangModule.Bank, "FindAndSelectMatchingTransactions", "Find & select matching transactions below") + "</span>");
        if ($("#spRecTotal").html() != $("#spActualTotal").html()) {
            return;
        }
        rows = Megi.grid("#tbTranList", "getSelections");
        if (rows == null || rows.length == 0) {
            return;
        }
        var html = '';
        $("#divMathInfo").removeClass("match-tip")
        $(".m-match-tab-header").find("li").eq(0).addClass("match").css({ "border-bottom": "1px solid #BAE58C" });
        if (rows.length > 1) {
            html += '<div class="content match">';
            html += '<div class="info">';
            html += '<div class="detail">';
            html += '<span>' + HtmlLang.Write(LangModule.Bank, "TransactionsSelected", "{0} transactions selected").format(rows.length) + '</span>';
            html += '</div>';
            html += '</div>';
            html += '</div>';
            $("#divMathInfo").html(html);
            return;
        }
        var mathItem = rows[0];

        var amt = Number(mathItem.MSplitReceiveAmtFor) > 0 ? Megi.Math.toMoneyFormat(mathItem.MSplitReceiveAmtFor)
            : Megi.Math.toMoneyFormat(mathItem.MSplitSpentAmtFor);

        html += '<div class="content match">';
        html += '<div class="info">';
        html += '<div class="detail">';
        html += '<span><label>' + $.mDate.format(mathItem.MBizDate) + '</label></span>';
        html += '</div>';

        html += '<div class="desc">';
        var ref = mathItem.MReference;
        if (mathItem.MTargetBillType == "Invoice") {
            ref = "[ " + mathItem.MNumber + " ] " + ref;
        }
        html += '<span class="m-ellipsis">' + (mathItem.MBankAccountName == null ? "" : mathItem.MBankAccountName) + '</span>';
        html += '<a href="javascript:void(0)" class="more-info" id="bMore">' + $('.bankinfo .more-info').html() + '</a>';
        html += '</div>';

        html += '<div class="amount">' + amt + '</div>';
        html += '<div class="clear"></div>';

        html += '</div>';
        html += '</div>';
        $("#divMathInfo").html(html);

        //设置弹窗值
        $('#bdivMoreContent').find('.statement-MDate').text($.mDate.format(mathItem.MBizDate));
        $('#bdivMoreContent').find('.statement-MTransAcctName').text((mathItem.MBankAccountName == null ? "" : mathItem.MBankAccountName));
        $('#bdivMoreContent').find('.statement-MDesc').text(ref);
        $('#bdivMoreContent').find('.statement-Amt').text(amt);

        //重新绑定事件
        $(".mginfo").find(".more-info").off("click").on("click",function () {
            $(".statement-detail", ".mginfo").hide();
            var left = $(this).offset().left;
            var top = $(this).offset().top;
            var height = $(this).closest(".mginfo").find(".statement-detail").height();
            Megi.popup("#bMore", { selector: "#bdivMoreContent" });
        });

        //隐藏弹出的Div
        $(".statement-detail", ".mginfo").find(".m-icon-close").off("click").on("click",function () {
            $(this).closest(".statement-detail").hide();
        });
    }
}

$(document).ready(function () {
    BDBankReconcileEdit.init();
});