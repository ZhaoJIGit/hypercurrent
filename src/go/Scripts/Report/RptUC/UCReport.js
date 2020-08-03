/// <reference path="UCReportFooter.js" />
/// <reference path="UCReportHeader.js" />
var UCReport = {
    options: null,
    dataSource: null,
    isReadonly: false,
    ReportID: $("#hidReportID").val(),
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    reportTypeId: '',
    isActive: false,
    getReportJson: function () {
        var headerInfo = UCReportHeader.getValue();
        UCReport.dataSource.HeaderTitle = headerInfo.title;
        UCReport.dataSource.HeaderContent = headerInfo.content;
        UCReport.dataSource.IsNew = headerInfo.isNew;
        UCReport.dataSource.IsActive = true;
        UCReport.dataSource.ReportID = UCReport.ReportID;
        return UCReport.dataSource;
    },
    init: function (opts) {
        UCReport.options = opts;
        UCReport.initFilter();
        UCReport.isReadonly = opts.isReadonly == undefined ? false : opts.isReadonly;
        opts.IsReload = false;
        UCReport.initAction();
        UCReport.load();
        UCReport.scrollEvent();
    },
    scrollEvent: function () {
        $(".m-imain").scroll(function () {
            $("#divReportDetail").find(".report-opt-container").each(function () {
                if ($(this).css("display") == "block") {
                    var optObj = $(this).parent().find(".opt");
                    var left = $(optObj).offset().left;
                    var top = $(optObj).offset().top;
                    var w = $(optObj).width();
                    var h = $(optObj).height();
                    var cWidth = $(this).width();
                    var cLeft = left - cWidth + w;
                    var cTop = top + h;
                    if (cLeft <= 5) {
                        cLeft = 5;
                    }
                    $(this).css({ left: cLeft, top: cTop });
                }
            });
            $(".opt").hide();
        });
        $(".m-imain").click(function (e) {
            if (e.target == "javascript:void(0)") {
                return;
            }
            $("#divReportDetail").find(".report-opt-container").hide();
        });
    },
    initFilter: function () {
        var filter = UCReport.getFilter();
        if (filter != null) {
            $("body").mFormSetForm(filter);
            //扩展过滤条件的初始化 chenpan
            if (UCReport.options.initFilter != undefined) {
                UCReport.options.initFilter(filter);
            }
        }
    },
    getFilter: function () {
        var filter = $("#hidReportFilter").val();
        if (filter != null && filter.length > 0) {

            filter = _.urlDecode(filter);

            return eval("(" + filter + ")");
        }
        return null;
    },
    reload: function () {
        UCReport.options.IsReload = true;
        UCReport.dataSource.Filter = UCReport.getParam();

        UCReport.getData(function (msg) {
            $("#divReportTitle1").html(mText.encode(msg.Title1));
            $("#divReportTitle2").html(mText.encode(msg.Title2));
            $("#divReportTitle3").html(mText.encode(msg.Title3));
            $("#divReportTitle4").html(mText.encode(msg.Title4));
            UCReport.reportTypeId = msg.Type;
            UCReport.isActive = msg.IsActive;
            UCReport.dataSource = msg;
            UCReport.createReport(msg.Rows);
            UCReport.initGridReport();

            if (UCReport.options.callback != undefined) {
                UCReport.options.callback(msg);
            }

            var noteDataSource = UCReport.getNoteDataSource();
            UCReportFooter.init({ Items: noteDataSource, hasChangeAuth: UCReport.hasChangeAuth });
        });
    },
    load: function () {
        UCReport.getData(function (msg) {
            //进行数据组装的时候，也给个遮罩

            if (msg == null) {
                return;
            }

            $("body").mask("");
            $("#divReportTitle1").html(mText.encode(msg.Title1));
            $("#divReportTitle2").html(mText.encode(msg.Title2));
            $("#divReportTitle3").html(mText.encode(msg.Title3));
            $("#divReportTitle4").html(mText.encode(msg.Title4));
            UCReport.reportTypeId = msg.Type;
            UCReport.isActive = msg.IsActive;
            UCReport.dataSource = msg;
            UCReportHeader.init({ title: HtmlLang.Write(LangModule.Report, msg.HeaderTitle, msg.HeaderTitle), content: msg.HeaderContent, isNew: msg.IsNew, isReload: UCReport.options.IsReload, isReadonly: UCReport.isReadonly, hasChangeAuth: UCReport.hasChangeAuth });
            UCReport.createReport(msg.Rows);
            UCReport.initGridReport();
            if (msg.Filter != null) {
                $("body").mFormSetForm(msg.Filter);
            }
            if (UCReport.isActive) {
                $("#aBackToReport").hide();
            } else {
                $("#aBackToReport").show();
            }
            var noteDataSource = UCReport.getNoteDataSource();
            UCReportFooter.init({ Items: noteDataSource, hasChangeAuth: UCReport.hasChangeAuth });

            if (UCReport.options.callback != undefined) {
                UCReport.options.callback(msg);
            }
            $("body").unmask("");
        })
    },
    //跳转到报表首页位置
    locateReport: function (index) {
        //报表的title
        var title = HtmlLang.Write(LangModule.Common, "allreports", "All Reports");
        //报表首页的url,调到第三个页签去
        var reportUrl = "/Report/?id=" + index;
        $.mTab.refresh(title, reportUrl, false, true);
    },
    initAction: function () {
        $("#aDeleteDraft").off("click").on("click", function () {
            UCReport.deleteReport(function (msg) {
                $.mMsg(LangKey.DeleteSuccessfully);
                if (msg.ObjectID == "" || msg.ObjectID == $("#hidReportID").val()) {

                    if (window.parent.$(".mCloseBox").length > 0) {
                        $.mDialog.close();
                    }
                    else {
                        UCReport.locateReport(0);
                        $.mTab.remove();
                    }
                } else {
                    var pRptTypeId = $("#hidPTypeID").val();
                    var url = "/Report/Report2/" + pRptTypeId + "/" + msg.ObjectID;
                    window.location = url;
                }
            });
        });
        $("#aSaveAsDraft").off("click").on("click", function () {
            UCReport.saveDraft(function (msg) {
                msg = msg.ResultData || msg;
                $("#hidReportID").val(msg.ObjectID);
                $.mMsg(LangKey.SaveSuccessfully);
                UCReport.locateReport(1);
                window.location = window.location;
            });
        });
        $("#aAddToReport").off("click").on("click", function () {
            var pRptTypeId = $(this).attr("prpttypeid");
            var pRptId = $(this).attr("prptid");
            var rptId = $(this).attr("rptid");
            UCReport.saveDraft(function () {
                mWindow.reload("/Report/Report2/" + pRptTypeId + "/" + pRptId);
            });
        });
        $("#divExportTypes a").off("click").on("click", function () {
            UCReport.exportData(this);
        });
        $("#aExport").off("click").on("click", function () {
            UCReport.exportData(this, "Xls");
        });
        $("#aPublish").off("click").on("click", function () {
            var sender = this;
            UCReport.doCallBackAfterSaveDraft(function () {
                var rptId = $(sender).attr("rptid");
                var rptTypeId = $(sender).attr("rpttypeid");
                var url = "/Report/Publish/" + rptTypeId + "/" + rptId;

                //用弹出框的形式来做
                $.mDialog.show({
                    mTitle: $("#aPublish").find(".l-btn-text").html(),
                    mWidth: 450,
                    mHeight: 380,
                    mDrag: "mBoxTitle",
                    mShowbg: true,
                    mContent: "iframe:" + url,
                    mCloseCallback: function (msg) {
                        if (msg == undefined || msg == "0") {
                            return;
                        }
                        UCReport.locateReport(2);
                        url = "/Report/Report2/View/" + rptTypeId + "/" + rptId;
                        mWindow.reload(url);
                    }
                });
            });
        });
        $("#aPrint").off("click").on("click", function () {
            UCReport.printReportAfterSaveDraft(this);
        });
        $("#aBatchPrint").off("click").on("click", function () {
            UCReport.printReport(this, true);
        });
    },
    printReportAfterSaveDraft: function (sender) {
        UCReport.doCallBackAfterSaveDraft(function () {
            UCReport.printReport(sender);
        });
    },
    printReport: function (sender, isBatchPrint) {
        var rptId = $(sender).attr("rptid");
        var json = {};
        json.ReportId = rptId;
        if (isBatchPrint && SubsidiaryLedger != undefined) {
            json.ReportFilter = SubsidiaryLedger.getFilter();
            var dicResult = SubsidiaryLedger.getAccountIdList();
            json.ReportFilter.MAccountIDs = Object.keys(dicResult).toString();
            json.ReportFilter.AccountLevel = +json.ReportFilter.AccountLevel;
            json.ReportFilter = $.toJSON(json.ReportFilter);
            json.IsBatchPrint = true;
        }

        var rptType = $(sender).attr("rpttype");
        var title = $(sender).attr("rpttitle");

        title = Megi.getCombineTitle([HtmlLang.Write(LangModule.Common, "Print", "Print"), title]);
        var param = $.toJSON({ reportType: rptType, jsonParam: escape($.toJSON(json)) });
        Print.previewPrint(title, param);
    },
    exportData: function (sender, exportType) {
        if (exportType == undefined) {
            exportType = $(sender).text();
        }
        $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
        var url = "/Report/RptManager/Export/" + UCReport.ReportID;

        //明细分类账导出Excel
        if (exportType == "Xls" && typeof (SubsidiaryLedger) != "undefined") {
            var data = { type: exportType, reportTypeId: $("#aExport").attr("rpttypeid") };
            var json = {};
            json.ReportFilter = SubsidiaryLedger.getFilter();
            json.ReportFilter.AccountIDNameList = SubsidiaryLedger.getAccountIdList();
            json.ReportFilter.AccountLevel = +json.ReportFilter.AccountLevel;
            json.ReportFilter = $.toJSON(json.ReportFilter);
            data.jsonParam = $.toJSON(json);
            Megi.openNewTab(url, data, true);
        }
        else {
            UCReport.doCallBackAfterSaveDraft(function () {
                location.href = url + "?type=" + exportType + "&reportTypeId=" + $("#aExport").attr("rpttypeid");
            });
        }
    },
    doCallBackAfterSaveDraft: function (callBack) {
        if (!UCReport.isActive) {
            UCReport.saveDraft(function () {
                callBack();
            });
        }
        else {
            callBack();
        }
    },
    getData: function (callback) {
        var obj = UCReport.getParam();
        mAjax.post(UCReport.options.url,
            { filter: obj }, function (msg) {
                msg = typeof msg == "string" ? $.parseJSON(msg) : msg;
                callback(msg);
            }, "", true);
    },
    deleteReport: function (callback) {
        //确认
        var title = LangKey.AreYouSureToDelete;
        //
        $.mDialog.confirm(title, function () {
            mAjax.submit(
                "/Report/RptManager/DeleteReportByReportID",
                { reportId: UCReport.ReportID },
                function (msg) {
                    msg = typeof msg == "string" ? $.parseJSON(msg) : msg;
                    callback(msg);
                });
        });
    },
    saveDraft: function (callback) {
        var reportData = UCReport.getReportJson();
        var content = $.toJSON(reportData);

        $("body").mask("");
        $("form").ajaxForm({
            url: "/Report/RptManager/UpdateReportByBizReport",
            data: { reportId: UCReport.ReportID, content: encodeURIComponent(content) },
            dataType: ImportBase.isIE9Previous ? null : "json",
            type: "POST",
            success: function (msg) {
                $("body").unmask();
                //解决IE9下无法进入成功回调，直接提示下载问题
                if (ImportBase.isIE9Previous) {
                    //另外获取post提交结果
                    $.getJSON("/Report/RptManager/GetUploadResult?id=" + (new Date()).toString(), function (response) {
                        callback(response);
                    });
                }
                else {
                    callback(msg);
                }
            },
            fail: function (event, data) {
                $("body").unmask();
                $.mDialog.alert(data);
            }
        });
        $("#rptContent").submit();
    },
    getParam: function () {
        var urlFilter = UCReport.getFilter();
        var obj = $("body").mFormGetForm();
        obj.MReportID = UCReport.ReportID;

        if (UCReport.options.getFilter != undefined) {
            var filter = UCReport.options.getFilter();
            if (filter != null) {
                $.extend(obj, filter);
            }
        }
        //主要传一些固定的参数
        if (UCReport.options.extParam != undefined) {
            $.extend(obj, UCReport.options.extParam);
        }

        if (UCReport.options.param) {
            //如果有参数传入，进行合并（处理一些特殊的easyui控件） chenpan
            $.extend(obj, UCReport.options.param);
        }
        obj.IsReload = UCReport.options.IsReload;
        //if (UCReport.options.getFilter != undefined) {
        //    var filter = UCReport.options.getFilter();
        //    if (filter != null) {
        //        $.extend(obj, filter);
        //    }
        //}
        if (UCReport.options.param) {
            //如果有参数传入，进行合并（处理一些特殊的easyui控件） chenpan
            $.extend(obj, UCReport.options.param);
        }
        return obj;
    },
    initGridReport: function () {
        $(".m-report").find("td").find(".value").mouseover(function () {
            var tdL = $(this).closest("td").offset().left;
            var tdW = $(this).closest("td").width();
            var vL = $(this).offset().left;
            var vW = $(this).width();
            var top = $(this).offset().top + 2;
            var l = vL + vW + 2;
            if (l - 5 > tdL + tdW) {
                l = tdL + tdW - 5;
            }
            $(this).parent().find(".opt").css({ "left": l + "px", top: top + "px", "position": "absolute", "display": "block" }).addClass("opt-focus");
        });
        $(".m-report").find("td").find(".value").mouseout(function () {
            $(this).parent().find(".opt").removeClass("opt-focus");
        });
    },
    //创建报表
    createReport: function (rows) {
        var reportDom = document.getElementById("divReportDetail");
        reportDom.innerHTML = "";
        if (rows == null || rows.length == 0 || rows[0].Cells == null || rows[0].Cells.length == 0) {
            return;
        }
        var columnCount = rows[0].Cells.length;
        var tbW = columnCount > 12 ? 'style="width:' + ($("#divReportDetail").width() + (columnCount - 12) * 150) + 'px"' : "";
        if (tbW.length > 0) {
            $("#divReportDetail").css("overflow", "auto");
        }
        var html = '<table cellpadding="0" cellspacing="0" ' + tbW + '>';
        var rowLength = rows.length;
        for (var i = 0; i < rowLength; i++) {
            var row = rows[i];

            if (UCReport.options.autoFillCell != undefined && UCReport.options.autoFillCell == false) {
                columnCount = row.Cells.length
            }

            html += UCReport.getRowHtml(row, i, columnCount);
        }
        html += '</table>';
        reportDom.innerHTML = html;

        UCReport.bindEvent();
    },
    //获取行html
    getRowHtml: function (rowObj, rowIndex, columnCount, hasChild) {
        var html = '';
        if (!rowObj || rowObj.Cells.length == 0) {
            return html;
        }
        var rowType = "item";
        var rowCls = "tb-item";
        switch (rowObj.RowType) {
            case 1:
                rowType = "header";
                break;
            case 2:
                rowType = "group";
                break;
            case 3:
                rowType = "item";
                break;
            case 31:
                rowType = "subitem";
                break;
            case 4:
                rowType = "subtotal";
                break;
            case 5:
                rowType = "total";
                break;
        }
        rowCls = "tb-" + rowType.toLocaleLowerCase();

        var hasChild = false;
        if (rowObj.SubRows && rowObj.SubRows.length > 0) {
            hasChild = true;
        }

        html += '<tr class="' + rowCls + '" rowIndex="' + rowIndex + '">';
        html += UCReport.getCellArrayHtml(rowObj.Cells, rowIndex, rowType, columnCount, hasChild);
        html += '</tr>';

        if (hasChild > 0) {
            var subRowLenght = rowObj.SubRows.length;
            for (var j = 0; j < subRowLenght; j++) {
                html += UCReport.getRowHtml(rowObj.SubRows[j], rowIndex + "_" + j, columnCount);
            }
        }

        return html;
    },
    //获取行表格html：
    getCellArrayHtml: function (cellArray, rowIndex, rowType, columnCount, hasChild) {
        var html = '';
        var cellLength = cellArray.length;
        for (var i = 0; i < cellLength; i++) {
            var cellObj = cellArray[i];
            if (i == 0 && hasChild) {
                html += UCReport.getCellHtml(cellObj, rowIndex, i, rowType, true);
            } else {
                html += UCReport.getCellHtml(cellObj, rowIndex, i, rowType, false);
            }
        }
        if (cellLength < columnCount) {

            var length = columnCount - cellLength;
            for (var i = 0; i < length; i++) {
                html += '<td rowIndex="' + rowIndex + '" cellIndex="' + (cellLength + i) + '" >&nbsp;</td>';
            }
        }
        return html;
    },
    //获取单个表格html
    getCellHtml: function (cellObj, rowIndex, cellIndex, rowType, hasChild) {
        var cls = UCReport.getCellCls(cellObj);
        var statuTypeCls = UCReport.getCellStatuCls(cellObj);
        var value = (cellObj.Value == null || cellObj.Value == "") ? "&nbsp;" : cellObj.Value;
        if (cls == "item-money") {
            value = mMath.toMoneyFormat(value);
        }
        else if (cls == "item-price") {
            //统一使用money css样式
            cls = "item-price item-money";
            value = mMath.toMoneyFormat(value, 4, 2);
        }
        else if (cls == "item-date" || cls == "item-datetime") {
            value = value.replace(/\"/g, "").replace(/\\/g, "");
            value = $.mDate.formatter(value);
        }
        var html = '';
        if (rowType != "header") {
            value = mText.encode(value);
        }
        switch (rowType) {
            case "item":
            case "subitem":
                html = UCReport.getItemCellHtml(cellObj, rowIndex, cellIndex, cls, statuTypeCls, value, hasChild);
                break;
            default:
                html = UCReport.getTotalCellHtml(cellObj, rowIndex, cellIndex, cls, statuTypeCls, value, hasChild);
                break;
        }
        return html;
    },
    //获取数据行表格html
    getItemCellHtml: function (cellObj, rowIndex, cellIndex, cls, statuTypeCls, value, hasChild) {
        var subReportUrl = "javascript:void(0)";
        var cellLinkClickEvent = "";
        var html = '';
        var optHtml = '';
        var valMargin = 0;
        if (cellIndex == 0) {
            valMargin = UCReport.getIndentMargin(rowIndex);
        }


        if (cellObj.CellLink != undefined && cellObj.CellLink != null) {
            cellObj.CellLink.Title = mText.htmlDecode(cellObj.CellLink.Title);
            cellObj.CellLink.Title = _.urlEncode(cellObj.CellLink.Title);
            cellLinkClickEvent = "$.mTab.addOrUpdate('" + cellObj.CellLink.Title + "','" + cellObj.CellLink.Url + "',false,true,false,true); return false;";
        }



        if (cellObj.SubReport != undefined && cellObj.SubReport != null) {
            var url = UCReport.getSubReportUrl(cellObj.SubReport);
            optHtml += '<a href="' + url + '" >' + cellObj.SubReport.Text + '</a>';
        }
        if (cellLinkClickEvent != "") {
            if (cellObj.CellLink.DisabledEvent) {
                //禁用了事件
                optHtml += '<a href="javascript:void(0)" class="click-a" reportname="' + cellObj.CellLink.Text + '"  url="' + cellObj.CellLink.Url + '">' + cellObj.CellLink.Text + '</a>';
            } else {
                optHtml += '<a href="javascript:void(0)"  onclick="' + cellLinkClickEvent + '">' + cellObj.CellLink.Text + '</a>';
            }

        }

        if (UCReport.hasChangeAuth) {
            optHtml += '<a href="javascript:void(0)" class="footnote-add"  rowIndex="' + rowIndex + '" cellIndex="' + cellIndex + '">' + HtmlLang.Write(LangModule.Report, "AddFootnote", "Add footnote") + '</a>';
        }
        var style = cellIndex == 0 ? "" : " style='padding-left:10px;' ";
        var htmlSpan = "";
        htmlSpan = cellObj.RowSpan == 0 ? htmlSpan : ' rowspan="' + cellObj.RowSpan + '" ';



        html += '<td class="' + cls + '" rowIndex="' + rowIndex + '" cellIndex="' + cellIndex + '" ' + style + (rowIndex == 0 ? '' : '') + htmlSpan + " >";

        if (cls == "item-text" || cls == "item-date" || cls == "item-datetime" || cls == "item-tree-filed") {

            var newMargin = valMargin;
            if (hasChild) {
                html += '<a href="javascript:void(0)" class="exp-col expand"  style="margin-left:' + valMargin + 'px">&nbsp;</a>';
                newMargin = 0;
            } else if (cellIndex == 0 && cls == "item-tree-filed") {
                html += '<a href="javascript:void(0)"  style="display: inline-block;float:left;width:14px">&nbsp;</a>';
            }
            if (cellObj.SubReport != undefined && cellObj.SubReport != null) {
                subReportUrl = UCReport.getSubReportUrl(cellObj.SubReport);
                html += '<a href="' + subReportUrl + '" class="value' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '">' + value + '</a>';
            } else if (cellLinkClickEvent != "") {
                if (cellObj.CellLink.DisabledEvent) {
                    html += '<a href="javascript:void(0)" class="click-a value' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '" url="' + cellObj.CellLink.Url + '" reportname="' + cellObj.CellLink.Text + '">' + value + '</a>';
                } else {
                    html += '<a href="javascript:void(0)" class="value' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '" onclick="' + cellLinkClickEvent + '">' + value + '</a>';
                }

            } else if (!cellObj.CellLink || !cellObj.CellLink.Url) {
                html += '<span  class="value' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '">' + value + '</span>';
            } else {
                html += '<a href="' + subReportUrl + '" class="value' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '">' + value + '</a>';
            }
            html += UCReport.getCellNoteHtml(cellObj.Notes);
            if (optHtml != '') {
                html += '<a href="javascript:void(0)" class="opt" style="position:absolute;">&nbsp;</a>';
            }
        }
        else {
            if (optHtml != '') {
                html += '<a href="javascript:void(0)" class="opt" style="position:absolute;">&nbsp;</a>';
            }
            var newMargin = valMargin;
            if (hasChild) {
                newMargin = 0;
            }
            html += UCReport.getCellNoteHtml(cellObj.Notes);
            if (cellObj.SubReport != undefined && cellObj.SubReport != null) {
                subReportUrl = UCReport.getSubReportUrl(cellObj.SubReport);
                html += '<a href="' + subReportUrl + '" class="value ' + statuTypeCls + '" style="margin-left:' + newMargin + 'px" title="' + value + '" >' + value + '</a>';
            } else if (cellLinkClickEvent != "") {
                html += '<a href="javascript:void(0)" class="value ' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '" onclick="' + cellLinkClickEvent + '">' + value + '</a>';
            } else if (!cellObj.CellLink || !cellObj.CellLink.Url) {
                html += '<span href="' + subReportUrl + '" class="value' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '">' + value + '</span>';
            } else {
                html += '<a href="' + subReportUrl + '" class="value ' + statuTypeCls + '"  style="margin-left:' + newMargin + 'px" title="' + value + '">' + value + '</a>';
            }
            if (hasChild) {
                html += '<a href="javascript:void(0)" class="exp-col expand"  style="margin-left:' + valMargin + 'px">&nbsp;</a>';
            }
        }
        if (optHtml != '') {
            html += '<div class="report-opt-container">';
            html += optHtml;
            html += '</div>';
        }
        html += '</td>';
        return html;
    },
    //获取树的缩进html
    getIndentMargin: function (rowIndex) {
        rowIndex = String(rowIndex);
        var k = 0;
        var sum = 0;
        k = rowIndex.indexOf("_");
        while (k > -1) {
            sum += 1;
            k = rowIndex.indexOf("_", k + 1);
        }
        if (sum == 0) {
            return "";
        }
        var w = 20;
        return w * sum;
    },
    setTreeIndent: function () {
        var treeFiled = $(".item-tree-filed");
        if ($(".item-tree-filed").length > 0) {
            var header = $(".item-tree-filed").eq(0).closest("table").find(".tb-header>td[cellindex='0']");
            $(header).html($(header).html() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
        }
    },
    getSubReportUrl: function (subReport) {
        var url = "/Report/Report2/" + subReport.ReportType;
        if (subReport.ReportFilter != null && subReport.ReportFilter != undefined) {
            var filter = {};
            for (var r in subReport.ReportFilter) {
                if (r == "MReportID" || r == "IsReload") {
                    continue;
                }
                var value = eval("subReport.ReportFilter." + r);
                if (value == undefined || value == "") {
                    continue;
                }
                if (value == "") {
                    continue;
                }

                if (typeof value == "string") {
                    value = _.urlEncode(value)
                }

                eval("filter." + r + "='" + value + "'");
            }
            var pRptTypeID = $("#hidReportTypeID").val();
            url = url + "/0/" + pRptTypeID + "/" + UCReport.ReportID + "?filter=" + encodeURI($.toJSON(filter));
        }
        return url;
    },
    //获取表格备注html
    getCellNoteHtml: function (notes) {
        if (notes == null || notes.length == 0) {
            return "";
        }
        var html = '';
        for (var i = 0; i < notes.length; i++) {
            var note = notes[i];
            html += "<a class='note-no' number='" + note.No + "' href='#reportnote" + note.No + "' >" + note.No + "</a>";
        }
        return html;
    },
    getTotalCellHtml: function (cellObj, rowIndex, cellIndex, cls, statuTypeCls, value) {
        var html = '';
        var style = cellIndex == 0 ? "" : " style='padding-left:10px;' ";
        var htmlSpan = "";
        htmlSpan = cellObj.RowSpan == 0 ? htmlSpan : " rowspan='" + cellObj.RowSpan + "'";
        htmlSpan += cellObj.ColumnSpan == 0 ? htmlSpan : " colspan='" + cellObj.ColumnSpan + "'";

        var extendClass = cellObj.RowSpan == 2 ? " multi-row" : "";

        html += '<td  class="' + cls + extendClass + '" rowIndex="' + rowIndex + '" cellIndex="' + cellIndex + '" ' + style + (rowIndex == 0 ? '' : '') + htmlSpan + ' >';
        html += '<span class="' + statuTypeCls + '">' + value + '</span>';
        html += '</td>';
        return html;
    },
    //获取表格class
    getCellCls: function (cellObj) {
        var type = "text";
        switch (cellObj.CellType) {
            case 1:
                type = "text";
                break;
            case 2:
                type = "money";
                break;
            case 3:
                type = "date";
                break;
            case 4:
                type = "datetime";
                break;
            case 5:
                type = "price";
                break;
            case 6:
                type = "text-right";
                break;
            case 7:
                type = "tree-filed";
                break;
            default:
                type = "text";
                break;
        }
        type = type.toLowerCase();
        return "item-" + type;
    },
    //获取表格状态class
    getCellStatuCls: function (cellObj) {
        var statuType = "";
        switch (cellObj.CellStatuType) {
            case 1:
                statuType = "UpRed";
                break;
            case 2:
                statuType = "UpGreen";
                break;
            case 3:
                statuType = "DownRed";
                break;
            case 4:
                statuType = "DownGreen";
                break;
        }
        return statuType.toLowerCase();
    },
    //创建事件
    bindEvent: function () {
        $("#divReportDetail").find(".tb-item,.tb-subitem").each(function () {
            $(this).find(".exp-col").click(function () {
                var rowIndex = $(this).closest("td").attr("rowindex");
                var cellIndex = $(this).closest("td").attr("cellindex");
                if ($(this).hasClass("expand")) {
                    $(this).removeClass("expand").addClass("collapse");
                    $("#divReportDetail").find(".tb-item,.tb-subitem").each(function () {
                        var trRowIndex = $(this).attr("rowindex");
                        if (trRowIndex.indexOf(rowIndex + "_") == 0) {
                            $(this).find(".exp-col").removeClass("expand").addClass("collapse");
                            $(this).show();
                            var td = $(this).find("td")[cellIndex];

                            var childrenDom = $(td).children();

                            if (childrenDom && childrenDom.length > 0) {
                                var width = $(td).width();

                                var tempWidth = 0;
                                var childLength = childrenDom.length;
                                for (var i = 0; i < childLength; i++) {
                                    tempWidth += $(childrenDom[i]).width();
                                }

                                if (tempWidth - 40 > width) {
                                    //有padding-right属性
                                    $($(".tb-header").find("td")[cellIndex]).width(tempWidth - 40);
                                }
                            }
                        }
                    });
                } else {
                    $(this).removeClass("collapse").addClass("expand");
                    $("#divReportDetail").find(".tb-item,.tb-subitem").each(function () {
                        var trRowIndex = $(this).attr("rowindex");
                        if (trRowIndex.indexOf(rowIndex + "_") == 0) {
                            $(this).find(".exp-col").removeClass("collapse").addClass("expand");
                            $(this).hide();

                        }
                    });
                }
            });
        });
        if (UCReport.isReadonly) {
            return;
        }
        $("#divReportDetail").find(".tb-item,.tb-subitem").find(".opt").click(function () {
            if ($(this).parent().find(".report-opt-container").css("display") == "none") {
                $(".report-opt-container").hide();
                var left = $(this).offset().left;
                var top = $(this).offset().top;
                var w = $(this).width();
                var h = $(this).height();
                var cWidth = $(this).parent().find(".report-opt-container").width();
                var cLeft = left - cWidth + w;
                var cTop = top + h;
                if (cLeft <= 5) {
                    cLeft = 5;
                }
                $(this).parent().find(".report-opt-container").css({ left: cLeft, top: cTop }).show();
            } else {
                $(".report-opt-container").hide();
            }
        });
        $("#divReportDetail").find(".footnote-add").click(function () {
            var rowIndex = $(this).attr("rowIndex");
            var cellIndex = $(this).attr("cellIndex");
            var number = UCReportFooter.addItem(rowIndex, cellIndex);
            var tdObj = $(this).closest(".report-opt-container").parent();
            if ($(tdObj).hasClass("item-money")) {
                $(tdObj).find(".opt").after("<a class='note-no' number='" + number + "' href='#reportnote" + number + "' >" + number + "</a>");
            } else {
                $(tdObj).find(".opt").before("<a class='note-no' number='" + number + "' href='#reportnote" + number + "'>" + number + "</a>");
            }
            $(this).closest(".report-opt-container").hide();

            rowIndex = rowIndex;
            cellIndex = Number(cellIndex);
            UCReport.addNote(rowIndex, cellIndex, number);
        });

        //统计项子集查看时间
        $("#divReportDetail").find(".subitem-view").click(function () {
            //如果第一个同级项class='tb-subitem'则显示
            var paretnRowIndex = $(this).attr('rowIndex');
            var dom = $(this);

            $("#divReportDetail").find(".tb-subitem").each(function () {
                var subitemParentIndex = $(this).attr("parentRowIndex");
                if (subitemParentIndex == paretnRowIndex) {
                    if ($(this).is(':hidden')) {
                        $(dom).text(HtmlLang.Write(LangModule.Report, "hideSubitem", "Hidden subitem"))
                        $(this).show();
                    } else {
                        $(dom).text(HtmlLang.Write(LangModule.Report, "ViewSubitem", "View subitem"));
                        $(this).hide();
                    }
                }
            });

            $(this).closest(".report-opt-container").hide();

        });

    },
    //新增note(更新数据源)
    addNote: function (rowIndex, cellIndex, number) {
        var row = UCReport.getRow(rowIndex);
        if (row.Cells[cellIndex].Notes == undefined) {
            row.Cells[cellIndex].Notes = new Array();
        }
        var note = {};
        note.No = number;
        note.Value = "";
        row.Cells[cellIndex].Notes.push(note);
    },
    //删除note(更新数据源)
    deleteNote: function (rowIndex, cellIndex, number) {
        var row = UCReport.getRow(rowIndex);
        var footNotes = row.Cells[cellIndex].Notes;
        if (footNotes == undefined || footNotes.length == 0) {
            return;
        }
        var arr = new Array();
        for (var i = 0; i < footNotes.length; i++) {
            if (footNotes[i].No != number) {
                arr.push(footNotes[i]);
            }
        }
        row.Cells[cellIndex].Notes = arr;
    },
    //更新note(更新数据源)
    updateNote: function (rowIndex, cellIndex, number, value) {
        var row = UCReport.getRow(rowIndex);
        var footNotes = row.Cells[cellIndex].Notes;
        if (footNotes == undefined || footNotes.length == 0) {
            return;
        }
        var arr = new Array();
        for (var i = 0; i < footNotes.length; i++) {
            if (footNotes[i].No == number) {
                footNotes[i].No = number;
                footNotes[i].Value = value;
            }
        }
    },
    //根据行索引获取行对象
    getRow: function (rowIndex) {
        var arrRowIndex = rowIndex.split('_');
        var row = null;
        var arrRowLength = arrRowIndex.length;
        for (var i = 0; i < arrRowLength; i++) {
            //第一级
            if (i == 0) {
                row = UCReport.dataSource.Rows[parseInt(arrRowIndex[i])];
            }
            else {//子项
                row = row.SubRows[parseInt(arrRowIndex[i])];
            }
        }
        return row;
    },
    //获取note数据源
    getNoteDataSource: function () {
        var rows = UCReport.dataSource.Rows;
        if (rows == null || rows.length == 0) {
            return null;
        }
        var arr = new Array();
        for (var i = 0; i < rows.length; i++) {
            UCReport.getRowNotes(arr, i, rows[i]);
        }
        return UCReport.orderNote(arr);
    },
    //获取所有批注
    getRowNotes: function (arr, rowIndex, row) {
        if (row == null) {
            return;
        }
        var cells = row.Cells;
        for (var k = 0; k < cells.length; k++) {
            var cell = cells[k];
            if (cell.Notes != null && cell.Notes.length > 0) {
                for (var m = 0; m < cell.Notes.length; m++) {
                    var note = {};
                    note.No = cell.Notes[m].No;
                    note.Value = cell.Notes[m].Value;
                    note.rowIndex = rowIndex;
                    note.cellIndex = k;
                    arr.push(note);
                }
            }
        }
        if (row.SubRows != null && row.SubRows.length > 0) {
            for (var j = 0; j < row.SubRows.length; j++) {
                UCReport.getRowNotes(arr, rowIndex, row.SubRows[j]);
            }
        }
    },
    //Notes按从小到大排序
    orderNote: function (notes) {
        if (notes.length == 0) {
            return notes;
        }
        for (var i = 0; i < notes.length; i++) {
            for (var k = i + 1; k < notes.length; k++) {
                var note1 = notes[i];
                var notes2 = notes[k];
                var number1 = Number(notes[i].No);
                var number2 = Number(notes[k].No);
                if (number1 > number2) {
                    notes[i] = notes2;
                    notes[k] = note1;

                }
            }
        }
        return notes;
    },
    backToParentReport: function () {
        mWindow.reload("/Report/Report2/" + $("#hidPTypeID").val() + "/" + $("#hidPID").val());
    },
    //展开所有的子集科目
    ExpandAll: function () {
        $(".tb-subitem").css("display", "table-row");
        $(".exp-col").removeClass("expand").addClass("collapse");
    }
}
