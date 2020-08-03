var BDBankCashCoding = {
    mouseDown: false,
    isSelect: false,
    LastCheckedBox: null,
    lastSelectedRow: null,
    scrollIntervalId: null,
    scrollSpeed: 1,
    IsClick: false,
    TrackDataSource: null,
    ContactDataSource: null,
    TaxRateDataSource: null,
    FastCodeDataSource: null,
    AccountDataSource: [],
    BankID: "",
    IsGL: $("#hidIsGL").val(),
    isSmartVersion: $("#isSmartVersion").val(),
    InitCashCodingList: function (bankID) {
        BDBankCashCoding.BankID = bankID;
        // felson 2016.7.12
        //var track = $("#hidTrack").val();
        BDBankCashCoding.TrackDataSource = mData.getNameValueTrackDataList();
        BDBankCashCoding.FastCodeDataSource = BDBankCashCodingFastCode.GetDataList();
        var contact = mText.getObject("#hidContacts");
        BDBankCashCoding.ContactDataSource = eval(contact);
        //felson 2016.7.12
        //var taxRate = $("#hidOrgTaxRateList").val();
        BDBankCashCoding.TaxRateDataSource = mData.getTaxRateList();

        BDBankCashCoding.getAccountData(function (msg) {
            BDBankCashCoding.bindGrid(bankID);

            $(window).resize(function () {
                $("#gridCashCoding").datagrid("resize", { height: BDBankCashCoding.getGridHeight() });
            });
        });

        $("#btnSaveAndRec").off("click").click(function () {
            BDBankCashCoding.saveAndRec();
        });

        $("#aDeleteCashCoding").off("click").click(function () {
            Megi.grid("#gridCashCoding", "deleteSelected", {
                url: "/BD/BDBank/DeleteCashCoding", callback: function () {
                    BDBankCashCoding.bindGrid(bankID);
                }
            });
        });

        $("#aCashCodingSearch").off("click").click(function () {
            BDBankCashCoding.endEditGrid()
            BDBankCashCoding.bindGrid(bankID);
            return false;
        });

        $("#aCashCodingClear").off("click").click(function () {
            $("#txtKeyword").val("");
            BDBankCashCoding.bindGrid(bankID);
            return false;
        });

        $("#aCCMarkAsNonGenerateVoucher").off("click").on("click", function () {
            BDBankCashCoding.endEditGrid();
            var selectRecs = Megi.grid("#gridCashCoding", "getSelections");
            //判断有没有拆分的数据，如果存在拆分的数据，就不执行
            for (var i = 0; i < selectRecs.length; i++) {
                var idCount = 0;
                for (var j = 0; j < selectRecs.length; j++) {
                    if (selectRecs[i].MRootID == selectRecs[j].MRootID) {
                        idCount += 1;
                    }
                }
                if (idCount > 1) {
                    var message = HtmlLang.Write(LangModule.Bank, "CannotMarkNotToGenerateEntry", "The data has been split, it can not unmark as non-gerenate entry");
                    $.mAlert(message);
                    return;
                }
            }

            var statu = $(this).attr("status");
            Megi.grid("#gridCashCoding", "optSelected", {
                url: "/BD/BDBank/UpdateBankBillVoucherStatus", msg: "", param: { status: statu }, callback: function (msg) {
                    if (msg.Success) {
                        var message = HtmlLang.Write(LangModule.Bank, "MarkSuccess", "Mark successful!");
                        $.mMsg(message);
                        BDBankCashCoding.bindGrid(bankID);
                    } else {
                        $.mAlert("<div>" + msg.Message + "</div>", function () {
                            BDBankStatementDetail.bindGrid(bankID);
                        }, 1, true);
                    }
                }
            });
        });
    },
    getGridHeight: function () {
        if (BDBankCashCoding.isSmartVersion)
            return $("body").height() - $("#gridCashCodingDiv").offset().top - 11;
        return $("body").height() - $("#gridCashCodingDiv").offset().top - 40;
    },
    getAccountData: function (callback) {
        mAjax.post("/BD/BDAccount/GetAccountListWithCheckType", {
            isIncludeParent: false,
            needFullName: true
        },
        function (data) {
            BDBankCashCoding.AccountDataSource = data || [];
            callback && $.isFunction(callback) && callback(BDBankCashCoding.AccountDataSource);
        });
    },
    getContactData: function (callback) {
        mAjax.post("/BD/Contacts/GetContactItemList", {},
        function (data) {
            BDBankCashCoding.ContactDataSource = data;
            $("#hidContacts").val(mText.toJson(data))
            if (callback != undefined) {
                callback();
            }
        });
    },
    getAccountCheckGroupModel: function (accountId) {
        if (!accountId || BDBankCashCoding.AccountDataSource == null || BDBankCashCoding.AccountDataSource.length == 0) {
            return null;
        }
        for (var i = 0; i < BDBankCashCoding.AccountDataSource.length; i++) {
            var acctModel = BDBankCashCoding.AccountDataSource[i];
            if (acctModel.MItemID == accountId) {
                return acctModel.MCheckGroupModel;
            }
        }
        return null;
    },
    validateGrid: function (rows, selectRec) {
        var index = -1;
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].MEntryID == selectRec.MEntryID) {
                index = i;
                break;
            }
        }
        var hasError = false;
        //分录的描述为必录
        if (!selectRec.MDesc) {
            BDBankCashCoding.setErrorInfo(index, "MDesc");
            hasError = true;
        }
        //启用了总账，科目为必录，根据核算项做验证（必录的返回值为2）
        if (BDBankCashCoding.IsGL == "1") {
            var acctId = selectRec.MAccountID;
            var requiredValue = "2";
            //验证科目
            if (!acctId) {
                BDBankCashCoding.setErrorInfo(index, "MAccountID");
                hasError = true;
            }
            var checkGroupModel = BDBankCashCoding.getAccountCheckGroupModel(acctId);
            if (checkGroupModel != null) {
                //验证联系, 联系人不能为空，并且联系人不是员工
                if (checkGroupModel.MContactID == requiredValue && (!selectRec.MContactID || selectRec.MContactID.indexOf("_4") > 0)) {
                    BDBankCashCoding.setErrorInfo(index, "MContactID");
                    hasError = true;
                }
                //验证员工，联系人不能为空，并且联系人是员工
                if (checkGroupModel.MEmployeeID == requiredValue && (!selectRec.MContactID || selectRec.MContactID.indexOf("_4") == -1)) {
                    BDBankCashCoding.setErrorInfo(index, "MContactID");
                    hasError = true;
                }

                //验证跟踪项
                if (BDBankCashCoding.TrackDataSource != null && BDBankCashCoding.TrackDataSource.length > 0) {
                    for (var i = 1; i <= BDBankCashCoding.TrackDataSource.length; i++) {
                        var chkItem = eval("checkGroupModel.MTrackItem" + i);
                        if (chkItem == requiredValue && !eval("selectRec.MTrackItem" + i)) {
                            BDBankCashCoding.setErrorInfo(index, "MTrackItem" + i);
                            hasError = true;
                        }
                    }
                }
            }
        }
        return hasError;
    },
    setErrorInfo: function (rowIndex, field) {
        $("#gridCashCoding").parent().find("tr[datagrid-row-index=" + rowIndex + "]>td[field='" + field + "']").addClass("row-error");
    },
    //刷新账面余额
    reloadBalance: function () {
        var dates = $('.daterangepicker-span').mDaterangepicker("getRangeDate");
        var id = Megi.request("acctid");
        var txtKeyword=$(".m-keyword-search #txtKeyword");
        var txtKeywordVal = $(".m-keyword-search #txtKeyword").val();
        //异步获取
        $("body").mFormGet({
            url: "/BD/BDBank/GetBDBankAccountInfoByDate",
            param: { acctid: id, beginDate: dates[0], endDate: dates[1] },
            callback: function () {
                txtKeyword.val(txtKeywordVal);
            }
        });
    },
    saveAndRec: function () {
        BDBankCashCoding.endEditGrid();
        var rows = Megi.grid("#gridCashCoding", "getRows");
        var selectRecs = Megi.grid("#gridCashCoding", "getSelections");
        if (selectRecs.length == 0) {
            $.mDialog.alert(HtmlLang.Write(LangModule.Common, "PleaseSelectOneOrMoreItems", "Please select one or more items!"));
            return;
        }

        var hasError = false;

        for (var i = 0; i < selectRecs.length; i++) {
            selectRecs[i].MTaxRate = BDBankCashCoding.getTaxRate(selectRecs[i].MTaxID);
            selectRecs[i].MDate = $.mDate.format(selectRecs[i].MDate);

            var itemError = BDBankCashCoding.validateGrid(rows, selectRecs[i]);
            if (itemError) {
                hasError = true;
            }
        }

        if (hasError) {
            return;
        }

        var obj = {};
        obj.MBankID = Megi.request("acctid");
        obj.MBankBillEntryList = selectRecs;
        mAjax.submit(
            "/BD/BDBank/UpdateCashCodingList",
            { model: obj },
            function (msg) {
                if (msg.Success) {
                    $.mMsg(LangKey.SaveSuccessfully);
                    BDBankCashCoding.bindGrid(BDBankCashCoding.BankID);
                    //刷新账面余额
                    BDBankCashCoding.reloadBalance();
                    // new BDBankReconcileHome().resetSingleTabInit(2);
                } else {
                    $.mDialog.alert("<div>" + msg.Message + "</div>", function () {
                        BDBankCashCoding.bindGrid(BDBankCashCoding.BankID);
                    }, 1, true);
                }
            });
        setTimeout($(window).resize(), 1000);
    },
    AddGridEditor: function (rowIndex) {
        BDBankCashCoding.SetAmtEditor(false, false);
        var rows = $('#gridCashCoding').datagrid('getRows');
        var entryId = "";
        for (var i = 0; i < rows.length; i++) {
            if (rowIndex == i) {
                entryId = rows[i].MEntryID;
                break;
            }
        }
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MEntryID != row.MRootID && row.MEntryID == entryId) {
                var isSpent = row.MSpentAmt > 0 ? true : false;
                BDBankCashCoding.SetAmtEditor(true, isSpent);
                break;
            }
        }
    },
    bindGrid: function (bankID) {
        //获取选择的日期
        var dates = new BDBankReconcileHome().getUserSelectedDate();
        var bankId = bankID;
        var columns = BDBankCashCoding.getColumns();
        var keyword = $("#txtKeyword").val();
        Megi.grid("#gridCashCoding", {
            resizable: true,
            auto: true,
            checkOnSelect: false,
            pagination: true,
            fitColumns: true,
            collapsible: true,
            scrollY: true,
            width: $("#gridCashCodingDiv").width() - 5,
            height: BDBankCashCoding.getGridHeight(),
            url: "/BD/BDBank/GetCashCodingList",
            queryParams: { MBankID: bankId, MKeyword: keyword, MAmount: keyword, StartDate: dates[0], EndDate: dates[1] },
            columns: columns,
            isSelectAll: true,
            chkField: "MEntryID",
            onClickCell: function (rowIndex, field, value) {
                $(".m-tip-top-div").hide();
                if (field == "MDate" || field == "MTransAcctName") {
                    return;
                }
                if (field != "MEntryID" && field != "MRootID" && field != "Action") {
                    BDBankCashCoding.beginEditRow(rowIndex, field);
                    var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowIndex, field: field });
                    if (editor != null) {
                        if (field == "MRef" || field == "MDesc") {
                            editor.target.focus();
                        }
                        else if (field == "MFastCodeID") {
                            $(editor.target).combogrid("textbox").focus();
                        }
                        else if (field == "MSpentAmt" || field == "MReceivedAmt") {
                            editor.target.focus();
                            editor.target.select();
                        }
                        else if (field == "MContactID" || field == "MTaxID" || field == "MAccountID" || field == "MTrackItem1" || field == "MTrackItem2" || field == "MTrackItem3" || field == "MTrackItem4" || field == "MTrackItem5") {
                            $(editor.target).combobox("textbox").focus();
                        }
                    }
                }
            },
            onLoadSuccess: function () {
                BDBankCashCoding.initGridEvent();
                BDBankCashCoding.bindGridSelectEvent();
                //点击结束编辑事件
                $(".bank-reconcile-main").off("click.endEdit").on("click.endEdit", function (e) {
                    BDBankCashCoding.endEditGrid();
                });
                $("#gridCashCoding").datagrid("resize");
                $("#gridCashCoding").datagrid("resizeColumn");

                BDBankCashCoding.initTitleTooltip();
            }
        });
    },
    initGridEvent: function () {
        //增加鼠标拖拽选中行
        BDBankCashCoding.mouseDrag2SelectRow();
    },
    //update by 锦友 性能优化
    //结束编辑列表
    endEditGrid: function () {
        var recs = Megi.grid("#gridCashCoding", "getRows");
        for (var i = 0; i < recs.length; i++) {
            var $tr = $("#gridCashCoding").closest(".datagrid").find("tr[datagrid-row-index='" + i + "']");
            var rootId = "";
            if ($tr.hasClass("datagrid-row-editing")) {
                rootId = recs[i].MRootID;
                var isselect = $tr.find(".row-key-checkbox").attr("checked") == "checked";
                Megi.grid("#gridCashCoding", "endEdit", i);
                //这里解决的是结束编辑会把之前选中状态去掉了。
                if (isselect) {
                    Megi.grid("#gridCashCoding", "selectRow", i);
                }
                BDBankCashCoding.save(rootId);
                break;
            }
            /*
                这是原来代码，循环所有行，没有做当前行是否是编辑状态状态，所以性能慢，耗时，45S，优化后用时5S
                //这里解决的是结束编辑会把之前选中状态去掉了。
                var isselect = $tr.find(".row-key-checkbox").attr("checked") == "checked";
                Megi.grid("#gridCashCoding", "endEdit", i);
                if (isselect) {
                    Megi.grid("#gridCashCoding", "selectRow", i);
                }
            */

        }
      },
    //保存数据
    save: function (rootId) {
        if (rootId == "") {
            return "";
        }
        var rows = Megi.grid("#gridCashCoding", "getRows");
        var items = new Array();
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MRootID == rootId) {
                row.MTaxRate = BDBankCashCoding.getTaxRate(row.MTaxID);
                row.MDate = $.mDate.format(row.MDate);
                row.MDesc = "dd";
                if (row.MSpentAmt > 0) {
                    row.MSpentAmt=mMath.toDecimal(row.MSpentAmt, 2);
                    }
                if (row.MReceivedAmt > 0) {
                    row.MReceivedAmt = mMath.toDecimal(row.MReceivedAmt, 2);
                    }
                
                items.push(row);
            }
        }
        if (items.length == 0) {
            return;
        }
        //实时保存数据
        mAjax.post("/BD/BDBank/UpdateBankBillEntryList", { entryList: items },
        function (data) {
        });
    },
    getCurrentRow: function () {
        var recs = Megi.grid("#gridCashCoding", "getRows");
        for (var i = 0; i < recs.length; i++) {
            if (i == BDBankCashCoding.RowIndex) {
                return recs[i];
            }
        }
        return null;
    },
    copyRow: function (splitObj, entryId) {
        BDBankCashCoding.endEditGrid();
        var rowIndex = -1;
        var obj = {};
        var rows = $('#gridCashCoding').datagrid('getRows');
        //找到当前行，并赋值给obj
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MEntryID == entryId) {
                obj = jQuery.extend(true, {}, row);
                obj.MSpentAmt = "";
                obj.MReceivedAmt = "";
                obj.MEntryID = Math.random() * 10;
                obj.MSrcEntryID = "";
                rowIndex = i;
                break;
            }
        }
        var newRowIndex = rowIndex + 1;
        $('#gridCashCoding').datagrid('insertRow', {
            index: newRowIndex,
            row: obj
        });
        if ($("#gridCashCoding").closest(".datagrid").find("tr[datagrid-row-index='" + rowIndex + "']").find(".row-key-checkbox").attr("checked") == "checked") {
            Megi.grid("#gridCashCoding", "selectRow", newRowIndex);
        } else {
            Megi.grid("#gridCashCoding", "unselectRow", newRowIndex);
        }
        BDBankCashCoding.beginEditRow(newRowIndex);
        //$(window).resize();
        //最后一行定位到滚动条末。
        if (newRowIndex == rows.length - 1) {
            $('.datagrid-body').animate({ scrollTop: $('.datagrid-body').height() });
        }
    },
    deleteRow: function (rootId, entryId) {
        var rowIndex = -1;
        BDBankCashCoding.endEditGrid();
        var rows = $('#gridCashCoding').datagrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MEntryID == entryId) {
                rowIndex = i;
            }
        }

        BDBankCashCoding.updateRootRowData(rootId, entryId);
        Megi.grid("#gridCashCoding", "deleteRow", rowIndex);

        BDBankCashCoding.save(rootId);
    },
    updateRootRowData: function (rootId, entryId) {
        var rows = $('#gridCashCoding').datagrid('getRows');
        var obj = null;
        var rootRowIndex = 0;
        var entryTotalSpentAmt = 0;
        var entryTotalReceiveAmt = 0;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MEntryID == rootId) {
                rootRowIndex = i;
                obj = row;
            }
            else if (row.MRootID == rootId && row.MEntryID != entryId) {
                entryTotalSpentAmt = Number(entryTotalSpentAmt) + Number(row.MSpentAmt);
                entryTotalReceiveAmt = Number(entryTotalReceiveAmt) + Number(row.MReceivedAmt);
            }
        }

        obj.MSpentAmt = Number(obj.MSrcSpentAmt) - entryTotalSpentAmt;
        obj.MReceivedAmt = Number(obj.MSrcReceivedAmt) - entryTotalReceiveAmt;
        if (obj.MSpentAmt > 0) {
            $("#gridCashCoding").closest(".datagrid-view").find("tr[datagrid-row-index='" + rootRowIndex + "']").find("td[field='MSpentAmt']>.datagrid-cell").html(Megi.Math.toMoneyFormat(obj.MSpentAmt));
        }
        if (obj.MReceivedAmt > 0) {
            $("#gridCashCoding").closest(".datagrid-view").find("tr[datagrid-row-index='" + rootRowIndex + "']").find("td[field='MReceivedAmt']>.datagrid-cell").html(Megi.Math.toMoneyFormat(obj.MReceivedAmt));
        }
    },
    //设置金额Editor
    SetAmtEditor: function (showEditor, isSpent) {
        $("#gridCashCoding").datagrid('removeEditor', 'MSpentAmt');
        $("#gridCashCoding").datagrid('removeEditor', 'MReceivedAmt');
        if (!showEditor) {
            return;
        }
        var editor = { type: 'numberbox', options: { precision: 4, minPrecision: 2, min: 0, max: 999999999999 } };
        if (isSpent) {
            $("#gridCashCoding").datagrid('addEditor', [{ field: 'MSpentAmt', editor: editor }]);
        } else {
            $("#gridCashCoding").datagrid('addEditor', [{ field: 'MReceivedAmt', editor: editor }]);
        }
    },
    //获取
    getEditingRowInfo: function () {
        var info = {};
        info.RowIndex = -1;
        info.RootID = "";
        info.EntryID = "";
        var editingRow = $(".datagrid-row-editing");
        if ($(editingRow).length == 0) {
            return info;
        }
        info.RowIndex = Number($(editingRow).attr("datagrid-row-index"));
        var chkBtn = $(editingRow).find(".row-key-checkbox");
        if (chkBtn.length > 0) {
            info.RootID = $(chkBtn).attr("rootId");
            info.EntryID = $(chkBtn).attr("entryId");
        }
        return info;
    },
    getContactDataSource: function (rootId) {
        var arr = new Array();
        var contactList = BDBankCashCoding.ContactDataSource;
        if (contactList == null || contactList.length == 0) {
            return arr;
        }
        return contactList;
    },
    reloadFastCodeData: function (fastCodeId) {
        var rowInfo = BDBankCashCoding.getEditingRowInfo();
        if (rowInfo.RowIndex == -1) {
            return;
        }
        BDBankCashCoding.FastCodeDataSource = BDBankCashCodingFastCode.GetDataList();
        var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MFastCodeID" });
        if (editor == null) {
            return;
        }
        $(editor.target).combogrid("grid").datagrid("loadData", BDBankCashCoding.FastCodeDataSource);
        $(editor.target).combogrid("setValue", fastCodeId);
        var fcText = "";
        for (var i = 0; i < BDBankCashCoding.FastCodeDataSource.length; i++) {
            if (BDBankCashCoding.FastCodeDataSource[i].MID == fastCodeId) {
                fcText = BDBankCashCoding.FastCodeDataSource[i].MText;
                break;
            }
        }
        $(editor.target).combogrid("textbox").val(fcText).focus();
    },
    reloadAccountData: function (acctId) {
        var rowInfo = BDBankCashCoding.getEditingRowInfo();
        if (rowInfo.RowIndex == -1) {
            return;
        }
        var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MAccountID" });
        if (editor == null) {
            return;
        }
        $(editor.target).combobox("loadData", BDBankCashCoding.AccountDataSource);
        if (acctId) {
            $(editor.target).combobox("select", acctId);
            $(editor.target).combobox("textbox").focus();
        }
    },
    loadContactData: function (target, rootId) {
        var ds = BDBankCashCoding.getContactDataSource(rootId);
        $(target).combobox("loadData", ds);
    },
    reloadContactData: function (contactModel) {
        var contactId = contactModel.MItemID;
        var mIsCustomer = contactModel.MIsCustomer;
        var mIsOther = contactModel.MIsOther;
        var mIsSupplier = contactModel.MIsSupplier;
        //这个地方比较特殊，其它类型的联系人，类型值是4，但是接口返回的3（返回的类型有1 供应商，2 客户，3 其它，4 员工），这里是个坑啊
        var mtype = "3";
        var rowInfo = BDBankCashCoding.getEditingRowInfo();
        if (rowInfo.RowIndex == -1) {
            return;
        }
        BDBankCashCoding.beginEditRow(rowInfo.RowIndex);
        var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MContactID" });
        if (editor == null) {
            return;
        }
        var target = editor.target;
        var rootId = rowInfo.RootID;
        BDBankCashCoding.loadContactData(target, rootId);
        var srcRow = BDBankCashCoding.getRowByEntryID(rootId);
        if (srcRow == null) {
            return;
        }
        $(target).combobox("textbox").click().focus();

        if (srcRow.MSpentAmt != '' && Number(srcRow.MSpentAmt) > 0) {
            if (mIsSupplier) {
                mtype = "1";
            } else if (mIsCustomer) {
                mtype = "2";
            }
             $(target).combobox("select", contactId + "_" + mtype);
            return;
        }
        if (srcRow.MReceivedAmt != '' && Number(srcRow.MReceivedAmt) > 0) {
            if (mIsCustomer) {
                mtype = "2";
            } else if (mIsSupplier) {
                mtype = "1";
            }
              $(target).combobox("select", contactId + "_" + mtype);
        }
    },
    setContactUrl: function (target, rootId) {
        var contactType = 1;
        var srcRow = BDBankCashCoding.getRowByEntryID(rootId);
        var contactType = 1;
        if (srcRow.MContactType == "Customer") {
            contactType = 1;
        } else if (srcRow.MContactType == "Supplier") {
            contactType = 2;
        } else {
            contactType = 3;
        }
        var $panel = $(target).combobox("panel");
        if ($(".add-combobox-item", $panel.parent()).length > 0) {
            $(".add-combobox-item", $panel.parent()).attr("url", "/BD/Contacts/ContactsEdit?contactType=" + contactType + "&name=" + encodeURI(srcRow.MTransAcctName));
        }
    },
    setRowDataByFastCode: function (fastCodeData) {
        var fcList = BDBankCashCoding.FastCodeDataSource;
        if (fcList == null || fcList.length == 0) {
            return;
        }
        var rowInfo = BDBankCashCoding.getEditingRowInfo();

        var chkBtn = $("#gridCashCoding").closest(".datagrid").find("tr[datagrid-row-index='" + rowInfo.RowIndex + "']").find(".row-key-checkbox");
        var rootId = $(chkBtn).attr("rootId");
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MContactID", fastCodeData.MContactID);
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MTaxID", fastCodeData.MTaxID);
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MAccountID", fastCodeData.MAccountID);
        if (fastCodeData.MRef != null && $.trim(fastCodeData.MRef) != "") {
            BDBankCashCoding.setEditorValue("textbox", rowInfo.RowIndex, "MRef", fastCodeData.MRef);
            BDBankCashCoding.BatchUpdateRowData("MRef", rootId, fastCodeData.MRef);
        }
        if (fastCodeData.MDesc != null && $.trim(fastCodeData.MDesc) != "") {
            BDBankCashCoding.setEditorValue("textbox", rowInfo.RowIndex, "MDesc", fastCodeData.MDesc);
            BDBankCashCoding.BatchUpdateRowData("MDesc", rootId, fastCodeData.MDesc);
        }
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MTrackItem1", fastCodeData.MTrackItem1);
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MTrackItem2", fastCodeData.MTrackItem2);
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MTrackItem3", fastCodeData.MTrackItem3);
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MTrackItem4", fastCodeData.MTrackItem4);
        BDBankCashCoding.setEditorValue("combobox", rowInfo.RowIndex, "MTrackItem5", fastCodeData.MTrackItem5);

        if (fastCodeData.MContactID && !fastCodeData.MAccountID) {
            BDBankCashCoding.setDefaultAccount(fastCodeData.MContactID);
        }

        //var contactEditor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MContactID" });
        //var fastCodeEditor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MFastCodeID" });
        //var refEditor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MRef" });

        ////联系人为空，焦点定位到联系人
        //if ($(contactEditor.target).combobox("getValue") == "") {
        //    //$(fastCodeEditor.target).combobox("showPanel").combobox("textbox").focus();
        //    $(contactEditor.target).combobox("showPanel").combobox("textbox").focus();
        //    //备注为空，焦点定位到备注
        //} else if ($(refEditor.target).val() == "") {
        //    $(refEditor.target).focus();
        //}
        //else {
        //    var descEditor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MDesc" });
        //    $(descEditor.target).focus();
        //}
    },
    setEditorValue: function (type, rowIndex, field, value) {
        var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowIndex, field: field });
        if (!editor) {
            return;
        }
        if (type == "combobox") {
            $(editor.target).combobox("setValue", value);
        } else {
            $(editor.target).val(value);
        }
    },
    getRows: function () {
        var rows = $('#gridCashCoding').datagrid('getRows');
        return (rows == null || rows == undefined) ? [] : rows;
    },
    getRowByEntryID: function (entryId) {
        var srcRow = {};
        var rows = BDBankCashCoding.getRows();
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MEntryID == entryId) {
                srcRow = row;
                break;
            }
        }
        return srcRow;
    },
    getEditingRowData: function () {
        var editingRow = $(".datagrid-row-editing");
        if ($(editingRow).length == 0) {
            return {};
        }
        editingRow = editingRow.eq(0);
        var chkBtn = $(editingRow).find(".row-key-checkbox");
        var entryId = $(chkBtn).attr("entryId");
        return BDBankCashCoding.getRowByEntryID(entryId);
    },
    //开始编辑行, 开始编辑单元格
    beginEditRow: function (rowIndex, field) {
        var trRow = $("#gridCashCoding").closest(".datagrid").find("tr[datagrid-row-index='" + rowIndex + "']");
        if (!$(trRow).hasClass("m-select")) {
            $("#gridCashCoding").closest(".datagrid-view").find("tr").removeClass("m-select");
        }

        BDBankCashCoding.endEditGrid();
        var chkBtn = $("#gridCashCoding").closest(".datagrid").find("tr[datagrid-row-index='" + rowIndex + "']").find(".row-key-checkbox");
        var entryId = $(chkBtn).attr("entryId");
        var rootId = $(chkBtn).attr("rootId");
        BDBankCashCoding.SetAmtEditor(false, false);

        if (rootId != entryId) {
            var rows = $('#gridCashCoding').datagrid('getRows');
            for (var i = 0; i < rows.length; i++) {
                var row = rows[i];
                if (row.MEntryID == rootId) {
                    if (Number(row.MSpentAmt) > 0) {
                        BDBankCashCoding.SetAmtEditor(true, true);
                    } else {
                        BDBankCashCoding.SetAmtEditor(true, false);
                    }
                    break;
                }
            }
        }
        Megi.grid("#gridCashCoding", "beginEdit", rowIndex);

        var contactEditor = $("#gridCashCoding").datagrid('getEditor', { index: rowIndex, field: "MContactID" });
        if (contactEditor == null) {
            return;
        }
        //绑定联系人
        BDBankCashCoding.loadContactData(contactEditor.target, rootId);
        //设置新增联系人的url
        BDBankCashCoding.setContactUrl(contactEditor.target, rootId);

        //绑定科目
        BDBankCashCoding.reloadAccountData();

        var fastCodeEditor = $("#gridCashCoding").datagrid('getEditor', { index: rowIndex, field: "MFastCodeID" });

        $(fastCodeEditor.target).combogrid("grid").datagrid("loadData", BDBankCashCoding.FastCodeDataSource);

        //$(fastCodeEditor.target).combogrid("textbox").focus();

        BDBankCashCoding.bindGridEditorEvent(rowIndex);
        BDBankCashCoding.bindGridSelectEvent();
    },
    //编辑下一行
    editNextRow: function (rowIndex) {
        var newRowIndex = rowIndex + 1;
        BDBankCashCoding.beginEditRow(newRowIndex);

        var editor = $("#gridCashCoding").datagrid('getEditor', { index: newRowIndex, field: "MFastCodeID" });
        if (editor != null) {
            $(editor.target).combogrid("textbox").focus();
        }
    },
    //根据联系人带出默认科目
    setDefaultAccount: function (contactId) {
        var contactList = BDBankCashCoding.ContactDataSource;
        if (contactList == null || contactList.length == 0) {
            return;
        }
        var rowInfo = BDBankCashCoding.getEditingRowInfo();
        var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowInfo.RowIndex, field: "MAccountID" });
        if (editor == null) {
            return;
        }
        var accountId = '';
        for (var i = 0; i < contactList.length; i++) {
            var contact = contactList[i];
            if (contact.MItemID == contactId) {
                accountId = contact.MAccountID;
            }
        }
        if (!accountId) {
            return;
        }
        $(editor.target).combobox("select", accountId);
    },
    bindGridSelectEvent: function () {
        //将原来的change事件改为click事件为了监听键盘按键操作
        $("#gridCashCoding").closest(".datagrid-view").find(".row-key-checkbox").off("click").on("click", function (event) {
            var rootId = $(this).attr("rootId");
            var chked = $(this).attr("checked") == "checked" ? true : false;
            $("#gridCashCoding").closest(".datagrid-view").find(".row-key-checkbox").each(function (i) {
                if ($(this).attr("rootId") == rootId) {
                    if (chked) {
                        $("#gridCashCoding").datagrid("selectRow", i);
                    } else {
                        $("#gridCashCoding").datagrid("unselectRow", i);
                    }
                    var $elem = $(event.target || event.srcElement);
                    if (event.shiftKey && BDBankCashCoding.LastCheckedBox != null && BDBankCashCoding.LastCheckedBox[0] != $elem[0]) {
                        BDBankCashCoding.shiftCheckRow($elem);
                    } else {
                        $(this).attr("checked", chked);
                    }
                    BDBankCashCoding.LastCheckedBox = $elem;
                    BDBankCashCoding.needCheckAll();
                }
            });
        });
    },
    bindGridEditorEvent: function (rowIndex) {
        $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input").each(function () {
            //将文本框的keyup事件改为失去焦点后触发批量填充
            $(this).off("blur.fc").on("blur.fc", function () {
                var curField = $(this).closest(".datagrid-editable").parent().attr("field");
                if (curField != "MSpentAmt" && curField != "MReceivedAmt") {
                    var rootId = $(this).closest(".datagrid-row-editing").find(".m-icon-split").attr("rootId");
                    //$(this).val()不支持cobobox获取值,combobox字段单独获取值
                    var value = $(this).val();
                    if (curField != "MRef" && curField != "MDesc") {
                        var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowIndex, field: curField });
                        if (editor != null) {
                            value = $(editor.target).combobox("getValue");
                        }
                    }

                    BDBankCashCoding.BatchUpdateRowData(curField, rootId, value);
                }
            });
            $(this).keyup(function (event) {
                var editingCtrls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                var editingCtrlsIndex = editingCtrls.index(this);
                var isLastEditor = editingCtrlsIndex == editingCtrls.length - 1 ? true : false;
                //回车键，如果是最一列，则编辑下一行，否则焦点移到下一输入框
                if (event.which == 13) {
                    if (isLastEditor) {
                        BDBankCashCoding.editNextRow(rowIndex);
                    } else {
                        $(editingCtrls[editingCtrlsIndex + 1]).focus();
                    }
                }
            }).keydown(function (event) {
                var editingCtrls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
                var editingCtrlsIndex = editingCtrls.index(this);
                var isLastEditor = editingCtrlsIndex == editingCtrls.length - 1 ? true : false;
                var newRowIndex = rowIndex + 1;
                if (event.which == 9) {
                    if (isLastEditor) {
                        var curField = $(this).closest(".datagrid-editable").closest("td").next().attr("field");
                        if (curField == "MSpentAmt" || curField == "MReceivedAmt" || curField == "Action") {
                            BDBankCashCoding.beginEditRow(newRowIndex);
                            $("#gridCashCoding").closest(".datagrid-view").find("tr[datagrid-row-index='" + newRowIndex + "']").find(".m-icon-split").focus();
                        }
                    }
                }
            });
        });
        $(".datagrid-row-editing").each(function () {
            var splitBtn = $(this).find("td[field='MRootID']").find(".m-icon-split");
            var entryId = $(splitBtn).attr("entryId");
            var rootId = $(splitBtn).attr("rootId");
            $(this).find("td[field='MSpentAmt']").find(".datagrid-editable-input").off("keyup").on("keyup", function (event) {
                setTimeout(function () {
                    var money = BDBankCashCoding.getEditorAmt($(event.target || event.srcElement).val());
                    BDBankCashCoding.updateRootSpentRowData(rootId, entryId, money, (event.target || event.srcElement));
                }, 100);
            });
            $(this).find("td[field='MReceivedAmt']").find(".datagrid-editable-input").off("keyup").on("keyup", function (event) {
                setTimeout(function () {
                    var money = BDBankCashCoding.getEditorAmt($(event.target || event.srcElement).val());
                    BDBankCashCoding.updateRootReceiveRowData(rootId, entryId, money, (event.target || event.srcElement));
                }, 100);
            });
        });
    },
    //shift键选中checkbox连续行,用于批量提交
    shiftCheckRow: function (from) {
        if (BDBankCashCoding.LastCheckedBox == null)
            return;
        var checked = BDBankCashCoding.LastCheckedBox.is(":checked");
       // var checkboxs = $(".row-key-checkbox");
        var checkboxs = $(".row-key-checkbox", $("#gridCashCoding").closest(".datagrid"));
        var start = false;
        var stop = false;
        var startElem = null;
        checkboxs.each(function (index, elem) {
            if (!start && !stop) {
                if ($(elem)[0] == $(from)[0]) {
                    start = true;
                    startElem = $(from);
                }
                else if ($(elem)[0] == BDBankCashCoding.LastCheckedBox[0]) {
                    start = true;
                    startElem = BDBankCashCoding.LastCheckedBox;
                }
                if (start) {
                    checked ? $(elem).attr("checked", "checked") : $(elem).removeAttr("checked");
                }
            }
            else if (start && !stop) {
                if ($(elem)[0] == $(from)[0] && startElem[0] == BDBankCashCoding.LastCheckedBox[0]) {
                    start = false;
                    stop = true;
                }
                if ($(elem)[0] == BDBankCashCoding.LastCheckedBox[0] && startElem[0] == $(from)[0]) {
                    start = false;
                    stop = true;
                }
                if (checked) {
                    $(elem).attr("checked", "checked");
                    Megi.grid("#gridCashCoding", "selectRow", index);
                }else{
                    $(elem).removeAttr("checked");
                }
                
            }
        });
    },
    //是否需要勾选全选
    needCheckAll: function () {
        var checkboxs = $(".row-key-checkbox");
        if (checkboxs.filter(function (index, element) {
            return !$(element).is(":checked");
        }).length == 0) {
            $(".cash-coding .datagrid-htable input[type='checkbox']").attr("checked", "checked");
        }
        else {
            $(".cash-coding .datagrid-htable input[type='checkbox']").removeAttr("checked");
        }
    },
    getEditorAmt: function (value) {
        if (value == null || value == "") {
            return 0;
        }
        value = Number(value);
        var reg = new RegExp("^(0|[1-9][0-9]*)+(.[0-9]+)?$");
        if (!reg.test(value)) {
            return 0;
        }
        return value;
    },
    updateRootSpentRowData: function (rootId, entryId, spentAmt, inputObj) {
        var rows = $('#gridCashCoding').datagrid('getRows');
        var obj = null;
        var rootRowIndex = 0;
        var entryTotalSpentAmt = 0;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MEntryID == rootId) {
                rootRowIndex = i;
                obj = row;
            }
            else if (row.MRootID == rootId && row.MEntryID != entryId) {
                entryTotalSpentAmt = Number(entryTotalSpentAmt) + Number(row.MSpentAmt);
            }
        }
        if (obj == null) {
            return;
        }
        var money = Number(obj.MSrcSpentAmt) - entryTotalSpentAmt - spentAmt;
        if (money <= 0) {
            if (inputObj != undefined) {
                $(inputObj).val("");
            }
            money = Number(obj.MSrcSpentAmt) - entryTotalSpentAmt;
        }
        obj.MSpentAmt = mMath.toDecimal(money, 2);
        BDBankCashCoding.updateRow(rootRowIndex, obj);
    },
    updateRow: function (index, row) {
        var rowTr = $("#gridCashCoding").parent().find("tr[datagrid-row-index=" + index + "]");
        var srcChked = $(rowTr).find(".row-key-checkbox").attr("checked");
        $('#gridCashCoding').datagrid('updateRow', {
            index: index,
            row: row
        });
        if (srcChked == "checked") {
            $(rowTr).find(".row-key-checkbox").attr("checked", true);
            $(rowTr).attr("isSelected", true);
        } else {
            $(rowTr).find(".row-key-checkbox").attr("checked", false);
            $(rowTr).attr("isSelected", false);
        }

        BDBankCashCoding.bindGridSelectEvent();
    },
    updateRootReceiveRowData: function (rootId, entryId, receiveAmt, inputObj) {
        var rows = $('#gridCashCoding').datagrid('getRows');
        var obj = null;
        var rootRowIndex = 0;
        var entryTotalReceiveAmt = 0;
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            if (row.MEntryID == rootId) {
                rootRowIndex = i;
                obj = row;
            }
            else if (row.MRootID == rootId && row.MEntryID != entryId) {
                entryTotalReceiveAmt = Number(entryTotalReceiveAmt) + Number(row.MReceivedAmt);
            }
        }
        if (obj == null) {
            return;
        }
        var money = Number(obj.MSrcReceivedAmt) - entryTotalReceiveAmt - receiveAmt;
        if (money <= 0) {
            if (inputObj != undefined) {
                $(inputObj).val("");
            }
            money = Number(obj.MSrcReceivedAmt) - entryTotalReceiveAmt;
        }
        obj.MReceivedAmt = mMath.toDecimal(money, 2);
        BDBankCashCoding.updateRow(rootRowIndex, obj);
    },
    MutilSelect: function (objColumn, event) {
        var trRow = $(objColumn).closest("tr");
        if (!event.shiftKey && !event.ctrlKey) {
            if ($(trRow).hasClass("m-select")) {
                $("#gridCashCoding").closest(".datagrid-view").find("tr").removeClass("m-select");
                BDBankCashCoding.lastSelectedRow = null;
            } else {
                $("#gridCashCoding").closest(".datagrid-view").find("tr").removeClass("m-select");
                $(trRow).addClass("m-select");
                BDBankCashCoding.lastSelectedRow = trRow;
            }
        }
        if (event.ctrlKey) {
            if ($(trRow).hasClass("m-select")) {
                $(trRow).removeClass("m-select");
                BDBankCashCoding.lastSelectedRow = null;
            }
            else {
                $(trRow).addClass("m-select");
                BDBankCashCoding.lastSelectedRow = trRow;
            }
        }
        else if (event.shiftKey) {
            if (BDBankCashCoding.lastSelectedRow == null)
                return;
            var selectRows = $("#gridCashCoding").closest(".datagrid-view").find(".m-select");
            var beginRowIndex = -1;
            if (selectRows.length > 0) {
                beginRowIndex = Number($(BDBankCashCoding.lastSelectedRow).attr("datagrid-row-index"));
            }
            var rowIndex = Number($(trRow).attr("datagrid-row-index"));

            var start;
            var stop;
            if (beginRowIndex > rowIndex) {
                start = rowIndex;
                stop = beginRowIndex;
            } else {
                start = beginRowIndex;
                stop = rowIndex;
            }
            for (var i = start; i < stop + 1; i++) {
                $("#gridCashCoding").closest(".datagrid-view").find("tr[datagrid-row-index='" + i + "']").addClass("m-select");
            }
        }

        mWindow.clearSelectedText();
    },
    bindGridComboxEditorEvent: function (field, value) {
        var editingRow = $(".datagrid-row-editing");
        if (editingRow.length == 0) {
            return;
        }
        var rootId = $(editingRow).find(".m-icon-split").attr("rootId");
        BDBankCashCoding.BatchUpdateRowData(field, rootId, value);
        //var editingCtrls = $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input");
        //for (var i = 0; i < editingCtrls.length; i++) {
        //    if ($(editingCtrls[i]).closest("td[field='" + field + "']").length > 0) {
        //        if (i == editingCtrls.length - 1) {
        //            BDBankCashCoding.editNextRow(rowIndex);
        //        }
        //        else {
        //            $(editingCtrls[i + 1]).focus();
        //        }
        //    }
        //}
    },
    //获取当前编辑行号
    GetEditRowIndex: function () {
        var editingRow = $(".datagrid-row-editing");
        var editRowIndex = Number($(editingRow).attr("datagrid-row-index"));
        return editRowIndex;
    },
    //更新model的属性值
    UpdateModelFiledValue(rowData, field, value) {
        rowData[field] = value;
    },
    BatchUpdateRowData: function (field, rootId, value) {
        //跟高勇已确认  删除判断父级没有被选中不批量操作的逻辑代码
        var srcRow = null;
        var rows = $('#gridCashCoding').datagrid('getRows');
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].MEntryID == rootId) {
                srcRow = rows[i];
                break;
            }
        }
        if (srcRow == null) {
            return;
        }
        var selectRows = $("#gridCashCoding").closest(".datagrid-view").find(".m-select");
        for (var i = 0; i < rows.length; i++) {
            var row = rows[i];
            var rowIndex;
            if (field != "MContactID" || srcRow.MContactType == row.MContactType) {
                for (var j = 0; j < selectRows.length; j++) {
                    rowIndex = $(selectRows[j]).attr("datagrid-row-index");
                    //updateRow事件触发会把当前行结束编辑状态,现根据不等于当前编辑行则更新
                    if (i == rowIndex && i != BDBankCashCoding.GetEditRowIndex()) {
                        //  eval("row." + field + "='" + value + "'");
                        BDBankCashCoding.UpdateModelFiledValue(row, field, value);
                        BDBankCashCoding.updateRow(i, row);
                        break;
                    }
                }
            }
            //如果联系人选择的是其他，则收付款都填充
            if (field === "MContactID") {
                var mtype = BDBankCashCoding.getContactSelectType(value);
                if (mtype === 3) {
                    for (var j = 0; j < selectRows.length; j++) {
                        rowIndex = $(selectRows[j]).attr("datagrid-row-index");
                        //updateRow事件触发会把当前行结束编辑状态,现根据不等于当前编辑行则更新
                        if (i == rowIndex && i != BDBankCashCoding.GetEditRowIndex()) {
                            eval("row." + field + "='" + value + "'");
                            BDBankCashCoding.updateRow(i, row);
                            break;
                        }
                    }
                }
            }
        }
    },
    getContactSelectType: function (contactId) {
        var contactList = BDBankCashCoding.ContactDataSource;
        if (contactList == null || contactList.length === 0) {
            return;
        }
        var contactType = '';
        for (var i = 0; i < contactList.length; i++) {
            var contact = contactList[i];
            if (contact.MItemID == contactId) {
                contactType = contact.MType;
            }
        }
        return contactType;
    },
    initTitleTooltip: function () {
        $(".tooltip-top").remove();
        $(".datagrid-body").scroll(function () {
            $(".tooltip:visible").remove();
        });
        $(".br-sort-column").tooltip({
            content: HtmlLang.Write(LangModule.FP, "Click2SortColumn", "点击进行排序"),
            position: "top"
        });
    },
    getSortColumnTitle: function (name) {
        return "<span class='br-sort-column'>" + name + "</span>";
    },
    getColumns: function () {
        var arr = new Array();
        arr.push({
            title: '<input type=\"checkbox\" >', field: 'MEntryID', width: 25, align: 'center', formatter: function (value, rec, rowIndex) {
                if (rec.MEntryID != rec.MRootID) {
                    return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\"  entryId='" + rec.MEntryID + "' rootId='" + rec.MRootID + "' disabled='disabled' >";
                }
                return "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + value + "\"  entryId='" + rec.MEntryID + "' rootId='" + rec.MRootID + "' >";
            }
        });

        arr.push({
            title: '', field: 'MRootID', width: 50, align: 'center', formatter: function (value, rec, rowIndex) {
                return '<a href="javascript:void(0)" class="m-icon-split" entryId="' + rec.MEntryID + '" rootId="' + rec.MRootID + '" onclick="BDBankCashCoding.copyRow(this,\'' + rec.MEntryID + '\')">&nbsp;</a>';
            }
        });

        arr.push({
            title: BDBankCashCoding.getSortColumnTitle(HtmlLang.Write(LangModule.Bank, "Date", "Date")), field: 'MDate', sortable: true, width: 100, align: 'left', formatter: function (value, rec, rowIndex) {
                value = $.mDate.formatter(value);
                if (rec.MRootID != rec.MEntryID) {
                    return "<div onclick='BDBankCashCoding.MutilSelect(this,event)'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + value + "</div>";
                }
                return "<div onclick='BDBankCashCoding.MutilSelect(this,event)'>" + value + "</div>";
            }
        });
        arr.push({
            title: BDBankCashCoding.getSortColumnTitle(HtmlLang.Write(LangModule.Bank, "Payee", "Receipt/payee")), field: 'MTransAcctName', sortable: true, width: 140, align: 'left', formatter: function (value, rec, rowIndex) {
                if (value == null || value == "") {
                    value = "&nbsp;";
                }
                return "<div onclick='BDBankCashCoding.MutilSelect(this,event)'>" + value + "</div>";
            }
        });

        var fastCodePanelWidth = 0;
        var fastCodeGridCols = [{ title: HtmlLang.Write(LangModule.Common, "FastCode", "Fast Code"), field: 'MText', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "Reference", "Reference"), field: 'MRef', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "Contact", "Contact"), field: 'MContactName', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "TaxRate", "Tax Rate"), field: 'MTaxName', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "GLAccount", "Account"), field: 'MAccountName', width: 185 }];
        if (BDBankCashCoding.TrackDataSource != null && BDBankCashCoding.TrackDataSource.length > 0) {
            for (var i = 0; i < BDBankCashCoding.TrackDataSource.length; i++) {
                fastCodeGridCols.push({ title: BDBankCashCoding.TrackDataSource[i].MName, field: "MTrackItem" + (i + 1) + "Name", width: 80 });
            }
        }
        for (var i = 0; i < fastCodeGridCols.length; i++) {
            fastCodePanelWidth += fastCodeGridCols[i].width;
        }

        arr.push({
            title: HtmlLang.Write(LangModule.Common, "FastCode", "Fast Code"), field: 'MFastCodeID', width: 100, align: 'left', formatter: function (value, rec, rowIndex) {
                return rec.MCode;
            }, editor: {
                type: 'addCombogrid',
                options: {
                    addOptions: {
                        url: "/FC/FCCashCodingModule/FCCashCodingEdit",
                        height: 330 + 50 * (BDBankCashCoding.TrackDataSource.length / 2),
                        itemTitle: HtmlLang.Write(LangModule.Common, "NewFastCode", "New Fast Code"),
                        hasPermission: true,
                        callback: function (fastCodeId) {
                            if (!fastCodeId) {
                                return;
                            }
                            BDBankCashCoding.reloadFastCodeData(fastCodeId);
                        }
                    },
                    dataOptions: {
                        panelWidth: fastCodePanelWidth + 30,
                        scrollY: true,
                        idField: "MID",
                        textField: "MText",
                        columns: [fastCodeGridCols],
                        data: BDBankCashCoding.FastCodeDataSource,
                        onSelect: function (selector, record) {
                            if (record != undefined) {
                                BDBankCashCoding.bindGridComboxEditorEvent("MFastCodeID", record.MID);
                                BDBankCashCoding.setRowDataByFastCode(record);
                                $(".datagrid-row-editing").find(".combo-text,.datagrid-editable-input").keyup();

                                var rowInfo = BDBankCashCoding.getEditingRowInfo();
                                BDBankCashCoding.reselectEditor(rowInfo.RowIndex, "MContactID");
                                BDBankCashCoding.reselectEditor(rowInfo.RowIndex, "MTaxID");

                                if (BDBankCashCoding.IsGL == "1") {
                                    BDBankCashCoding.reselectEditor(rowInfo.RowIndex, "MAccountID");
                                }

                                if (BDBankCashCoding.TrackDataSource != null) {
                                    for (var i = 0; i < BDBankCashCoding.TrackDataSource.length; i++) {
                                        var trackItemField = "MTrackItem" + (i + 1);
                                        BDBankCashCoding.reselectEditor(rowInfo.RowIndex, trackItemField);
                                    }
                                }
                            }
                        }
                    }
                }
            }, formatter: function (value, rec, rowIndex) {
                if (value == null || value.length == 0) {
                    return "";
                }
                if (BDBankCashCoding.FastCodeDataSource == null || BDBankCashCoding.FastCodeDataSource.length == 0) {
                    return "";
                }
                for (var i = 0; i < BDBankCashCoding.FastCodeDataSource.length; i++) {
                    if (BDBankCashCoding.FastCodeDataSource[i].MID == value) {
                        return BDBankCashCoding.FastCodeDataSource[i].MCode;
                    }
                }
                return "";
            }
        });

        arr.push({
            title: HtmlLang.Write(LangModule.Bank, "Contact", "Contact"), field: 'MContactID', width: 100, align: 'left', editor: {
                type: 'addCombobox',
                options: {
                    type: 'contact',
                    addOptions: {
                        hasPermission: true,
                        callback: function (contactModel) {
                            BDBankCashCoding.getContactData(function () {
                                BDBankCashCoding.reloadContactData(contactModel);
                            });
                        }
                    },
                    dataOptions: {
                        height: "28px",
                        valueField: "MItemID",
                        textField: "MName",
                        groupField: "MGroupName",
                        autoSizePanel: true,
                        mode: 'local',
                        onSelect: function (record) {
                            if (record != undefined) {
                                BDBankCashCoding.bindGridComboxEditorEvent("MContactID", record.MItemID);
                                BDBankCashCoding.setDefaultAccount(record.MItemID);
                            }
                        }
                    }
                }
            }, formatter: function (value, rec, rowIndex) {
                if (value == null || value.length == 0) {
                    return "";
                }
                return BDBankCashCoding.getContactName(value);
            }
        });

        arr.push({ title: HtmlLang.Write(LangModule.Bank, "Description", "Description"), field: 'MDesc', width: 100, align: 'left', editor: { type: 'text' } });
        arr.push({ title: HtmlLang.Write(LangModule.Bank, "Reference", "Reference"), field: 'MRef', width: 100, align: 'left', editor: { type: 'text' } });

        arr.push({
            title: HtmlLang.Write(LangModule.Bank, "TaxRate", "Tax Rate"), field: 'MTaxID', width: 100, align: 'left', editor: {
                type: 'combobox',
                options: {
                    height: "28px",
                    valueField: "MItemID",
                    textField: "MName",
                    autoSizePanel: true,
                    data: BDBankCashCoding.TaxRateDataSource,
                    onSelect: function (record) {
                        if (record != undefined) {
                            BDBankCashCoding.bindGridComboxEditorEvent("MTaxID", record.MItemID);
                        }
                    }
                }
            }, formatter: function (value, rec, rowIndex) {
                return BDBankCashCoding.getTaxRateName(value);
            }
        });
        if (BDBankCashCoding.IsGL == "1") {
            arr.push({
                title: HtmlLang.Write(LangModule.Bank, "GLAccount", "Account"), field: 'MAccountID', width: 100, align: 'left', editor: {
                    type: 'addCombobox',
                    options: {
                        type: 'account',
                        addOptions: {
                            hasPermission: true,
                            addClass: 'm-account-combobox',
                            callback: function (mAccountModel) {
                                BDBankCashCoding.getAccountData(function () {
                                    if (!mAccountModel) {
                                        return;
                                    }
                                    BDBankCashCoding.reloadAccountData(mAccountModel.MAccountID);
                                });
                            }
                        },
                        dataOptions: {
                            data: BDBankCashCoding.AccountDataSource,
                            height: "28px",
                            valueField: "MItemID",
                            textField: "MFullName",
                            autoSizePanel: true,
                            onSelect: function (record) {
                                if (record != undefined) {
                                    BDBankCashCoding.bindGridComboxEditorEvent("MAccountID", record.MItemID);
                                }
                            }
                        }
                    }
                }, formatter: function (value) {
                    if (value == undefined || value == "") {
                        return "";
                    }
                    for (var i = 0; i < BDBankCashCoding.AccountDataSource.length; i++) {
                        var account = BDBankCashCoding.AccountDataSource[i];
                        if (account.MItemID == value) {
                            return account.MFullName;
                        }
                    }
                    return "";
                }
            });
        }

        if (BDBankCashCoding.TrackDataSource != null) {
            for (var i = 0; i < BDBankCashCoding.TrackDataSource.length; i++) {
                var obj = BDBankCashCoding.getTrackColumn(i);
                arr.push(obj);
            }
        }

        arr.push({
            title: HtmlLang.Write(LangModule.Bank, "Spent", "Spent"), field: 'MSpentAmt', width: 100, align: 'right', formatter: function (value, rec, rowIndex) {
                return value == 0 ? "" : Megi.Math.toMoneyFormat(value);
            }
        });
        arr.push({
            title: HtmlLang.Write(LangModule.Bank, "Received", "Received"), field: 'MReceivedAmt', width: 100, align: 'right', formatter: function (value, rec, rowIndex) {
                return value == 0 ? "" : Megi.Math.toMoneyFormat(value);
            }
        });
        arr.push({
            title: "", field: 'Action', align: 'center', width: 30, formatter: function (value, rec, rowIndex) {
                if (rec.MEntryID == rec.MRootID) {
                    return "";
                }
                return "<div class='list-item-action'><a href=\"javascript:void(0);\"  onclick=\"BDBankCashCoding.deleteRow('" + rec.MRootID + "','" + rec.MEntryID + "');\" class='list-item-del'></a></div>";
            }
        });
        var columns = new Array();
        columns.push(arr);
        return columns;
    },
    //重新选择Combobox
    reselectEditor: function (rowIndex, field) {
        var editor = $("#gridCashCoding").datagrid('getEditor', { index: rowIndex, field: field });
        if (editor != null) {
            var value = $(editor.target).combobox("getValue");
            if (!value) {
                return;
            }
            $(editor.target).combobox("select", value);
        }
    },
    getContactName: function (itemId) {
        if (BDBankCashCoding.ContactDataSource == null || BDBankCashCoding.ContactDataSource.length == 0) {
            return "";
        }
        for (var i = 0; i < BDBankCashCoding.ContactDataSource.length; i++) {
            if (BDBankCashCoding.ContactDataSource[i].MItemID == itemId) {
                return BDBankCashCoding.ContactDataSource[i].MName;
            }
        }
        return "";
    },
    //获取税率名称
    getTaxRateName: function (taxRateId) {
        for (var i = 0; i < BDBankCashCoding.TaxRateDataSource.length; i++) {
            if (BDBankCashCoding.TaxRateDataSource[i].MItemID == taxRateId) {
                return BDBankCashCoding.TaxRateDataSource[i].MName;
            }
        }
        return "";
    },
    //获取税率
    getTaxRate: function (taxRateId) {
        if (taxRateId == null || taxRateId == "") {
            return 0;
        }
        for (var i = 0; i < BDBankCashCoding.TaxRateDataSource.length; i++) {
            if (BDBankCashCoding.TaxRateDataSource[i].MItemID == taxRateId) {
                return parseFloat(BDBankCashCoding.TaxRateDataSource[i].MEffectiveTaxRate);
            }
        }
        return 0;
    },
    getTrackColumn: function (index) {
        var obj = {};
        obj.title = BDBankCashCoding.TrackDataSource[index].MName;
        obj.field = "MTrackItem" + (index + 1);
        obj.width = 80;
        obj.editor = {
            type: 'combobox',
            options: {
                height: "28px",
                valueField: "MValue",
                textField: "MName",
                hideItemKey: 'MValue1',
                hideItemValue: '0',
                autoSizePanel: true,
                data: BDBankCashCoding.TrackDataSource[index].MChildren,
                onSelect: function (record) {
                    BDBankCashCoding.bindGridComboxEditorEvent(obj.field, record.MValue, record);
                }
            }
        };
        obj.formatter = function (value, rowData, rowIndex) {
            return BDBankCashCoding.getTrackName(value);
        }
        return obj;
    },
    getFastCodeColumns: function () {
        var arr = [{ title: HtmlLang.Write(LangModule.Common, "FastCode", "Fast Code"), field: 'MText', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "Reference", "Reference"), field: 'MRef', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "Contact", "Contact"), field: 'MContactName', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "TaxRate", "Tax Rate"), field: 'MTaxRateName', width: 85 },
                    { title: HtmlLang.Write(LangModule.Bank, "GLAccount", "Account"), field: 'MAccountName', width: 185 }];
        if (BDBankCashCoding.TrackDataSource != null && BDBankCashCoding.TrackDataSource.length > 0) {
            for (var i = 1; i <= BDBankCashCoding.TrackDataSource.length; i++) {
                arr.push({ title: BDBankCashCoding.TrackDataSource[0].MName, field: "MTrackItem" + i + "Name", width: 80 });
            }
        }
        return arr;
    },
    getTrackName: function (value) {
        for (var i = 0; i < BDBankCashCoding.TrackDataSource.length; i++) {
            if (BDBankCashCoding.TrackDataSource[i].MChildren != null) {
                for (var k = 0; k < BDBankCashCoding.TrackDataSource[i].MChildren.length; k++) {
                    if (value == BDBankCashCoding.TrackDataSource[i].MChildren[k].MValue) {
                        return BDBankCashCoding.TrackDataSource[i].MChildren[k].MName;
                    }
                }
            }
        }
        return "";
    },
    //清除选中的文本
    clearSelectedText: function () {
        if (mObject.getPropertyValue(document, "selection")) {
            mObject.getPropertyValue(document, "selection").empty();
        }
        else if (window.getSelection) {
            window.getSelection().removeAllRanges();
        }
    },
    //获取关联的行
    getRelatedTrs: function (tr) {
        var rowspan = tr.find("td[field='MEntryID']").attr("rowspan");
        if (!!rowspan && +(rowspan) > 0) {
            var num = $(this.splitBtn, tr).attr("mnumber");
            return tr.add(tr.nextAll(":lt(" + ((+rowspan - 1)) + ")"));
        }
        return tr;
    },
    //选中行
    selectRow: function (row, selected) {
        if (selected) {
            $(row).addClass("m-select");
        }
        else {
            $(row).removeClass("m-select");
        }
    },
    //鼠标拖拽
    mouseDrag2SelectRow: function () {
        $("#gridCashCodingDiv").off("mousedown.m2").on("mousedown.m2", function (event) {
            var tableHeaderHeight = $("#gridCashCodingDiv .datagrid-htable:visible").height();
            var checkall = ".cash-coding .datagrid-htable input[type='checkbox']";
            if ($(event.srcElement || event.target)[0] == $(checkall)[0])
                return true;
            if ($(event.srcElement || event.target).hasClass("row-key-checkbox"))
                return true;
            if (!$("#gridCashCodingDiv").is(":visible"))
                return true;
            var selList = $("tr.datagrid-row td[field='MEntryID']:visible");
            var checkX = $(checkall).closest("td[field='MEntryID']:visible").offset().left + $(checkall).closest("td[field='MEntryID']:visible").width();
            var startX = (event.clientX);
            var startY = (event.clientY);
            var bodyX = $("#gridCashCodingDiv").offset().left;
            //Y轴的高度取决于表头,而不能写死
            var bodyY = $("#gridCashCodingDiv").offset().top + tableHeaderHeight;
            //拆分列不允许拖拽
            var splitLeft = $("td[field='MRootID']:visible").offset().left;
            var splitRight = $("td[field='MDate']:visible").offset().left;
            if (startX >= splitLeft && startX < splitRight)
                return true;
            //收付款人后面的列不允许拖拽
            var receiveLeft = $("td[field='MFastCodeID']:visible").offset().left;
            if (startX > receiveLeft)
                return true;
            //超出边界不允许拖拽
            if (startX < bodyX || startY < bodyY)
                return true;
            BDBankCashCoding.mouseDown = true;
            BDBankCashCoding.isSelect = true;
            var _x = null;
            var _y = null;
            var selectFunc = function (event, x, y) {
                if (BDBankCashCoding.isSelect && BDBankCashCoding.mouseDown)
                    BDBankCashCoding.clearSelectedText();
                _x = x || (event.clientX);
                _y = y || (event.clientY);
                if (Math.abs(_x - startX) <= 2 && Math.abs(_y - startY) <= 2)
                    return;
                if (BDBankCashCoding.isSelect && BDBankCashCoding.mouseDown && $(".select-div").length == 0) {
                    var bgDiv1 = document.createElement("div");
                    bgDiv1.style.cssText = "position:absolute;width:0px;height:0px;font-size:0px;margin:0px;padding:0px;background-color:#C3D5ED;z-index:999;opacity:0.0;left:0;top:0;display:none;";
                    bgDiv1.className = "select-bg-div";
                    $(bgDiv1).width($("body").width());
                    $(bgDiv1).height($("body").width());
                    $("#gridCashCodingDiv").append(bgDiv1);
                    var selDiv1 = document.createElement("div");
                    selDiv1.style.cssText = "position:absolute;width:0px;height:0px;font-size:0px;margin:0px;padding:0px;border:1px dashed #0099FF;background-color:#C3D5ED;z-index:1000;filter:alpha(opacity:60);opacity:0.6;display:none;";
                    selDiv1.id = "selectDiv";
                    selDiv1.className = "select-div";
                    $("#gridCashCodingDiv").append(selDiv1);
                    selDiv1.style.left = startX + "px";
                    selDiv1.style.top = startY + "px";
                }
                var selDiv = $(".select-div");
                var bgDiv = $(".select-bg-div");
                if (BDBankCashCoding.isSelect && BDBankCashCoding.mouseDown) {
                    if (selDiv.is(":hidden")) {
                        selDiv.show();
                    }
                    if (bgDiv.is(":hidden")) {
                        bgDiv.show();
                    }
                    selDiv[0].style.left = Math.min(_x, startX) + "px";
                    selDiv[0].style.top = Math.min(_y, startY) + "px";
                    selDiv[0].style.width = Math.abs(_x - startX) + "px";
                    selDiv[0].style.height = Math.abs(_y - startY) + "px";
                    var _l = selDiv[0].offsetLeft, _t = selDiv[0].offsetTop;
                    var _w = selDiv[0].offsetWidth, _h = selDiv[0].offsetHeight;
                    for (var i = 0; i < selList.length; i++) {
                        var scrollTop = $(selList[i]).closest(".datagrid-body").scrollTop();
                        var sl = selList[i].offsetWidth + selList[i].offsetLeft + 20;
                        var st = selList[i].offsetHeight + selList[i].offsetTop + 170 - scrollTop;
                        if (startX < checkX) {
                            if ((sl > startX && 20 < _x && st + tableHeaderHeight > startY && st < _y) || (sl > _x && sl < startX && st < startY && st > _y)) {
                                selList.eq(i).find("input[type='checkbox']:visible").attr("checked", "checked");
                            }
                            BDBankCashCoding.needCheckAll();
                        }
                        else {
                            if ((sl < startX && sl < _x && st + tableHeaderHeight > startY && st < _y) || (sl < _x && sl < startX && st < startY && st > _y)) {
                                var trs = BDBankCashCoding.getRelatedTrs(selList.eq(i).closest("tr.datagrid-row"));
                                trs.each(function (index, elem) {
                                    BDBankCashCoding.selectRow($(elem), true);
                                });
                            }
                        }
                    }
                }
                else {
                    selDiv.remove();
                    bgDiv.remove();
                }
            };
            var scrollInterval = function (event) {
                var div = $("#gridCashCoding").prev("div").find(".datagrid-body");
                if (div.length == 0)
                    return;
                var scrollHeight = div[0].scrollHeight;
                var height = div.height();
                if (scrollHeight > height) {
                    var cx = event.clientX, cy = event.clientY;
                    if (cx > div.offset().left && cy > (div.offset().top + height) && BDBankCashCoding.scrollIntervalId == null) {
                        BDBankCashCoding.scrollIntervalId = window.setInterval(function () {
                            if (div[0].scrollTop + height >= scrollHeight) {
                                window.clearInterval(BDBankCashCoding.scrollIntervalId);
                                BDBankCashCoding.scrollIntervalId = null;
                                return;
                            }
                            ;
                            div[0].scrollTop = div[0].scrollTop + BDBankCashCoding.scrollSpeed * 5;
                            BDBankCashCoding.scrollSpeed++;
                            selectFunc(event);
                        }, 100);
                    }
                }
            };
            $("#gridCashCodingDiv").off("mousemove.m2").on("mousemove.m2", function (event) {
                selectFunc(event);
                BDBankCashCoding.isSelect && BDBankCashCoding.mouseDown && scrollInterval(event);
            });
            $(document).off("keyup.m2").on("keyup.m2", function (event) {
                if (event.keyCode == 38 || event.keyCode == 40 || event.keyCode == 33 || event.keyCode == 34) {
                    selectFunc(event, _x, _y);
                }
            });
            $(document).off("mouseup.m2").on("mouseup.m2", function (event) {
                BDBankCashCoding.isSelect = false;
                BDBankCashCoding.mouseDown = false;
                if (BDBankCashCoding.scrollIntervalId != null) {
                    window.clearInterval(BDBankCashCoding.scrollIntervalId);
                    BDBankCashCoding.scrollIntervalId = null;
                }
                var selectDiv = $(".select-div");
                var bgDiv = $(".select-bg-div");
                selectDiv.remove();
                bgDiv.remove();
                selList = null, _x = null, _y = null, startX = null, startY = null;
            });
            return;
        });
    }
}