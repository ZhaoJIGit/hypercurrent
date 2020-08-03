/// <reference path="../../IV/IVBase.js" />
/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var ContactView = {
    CurrentTabID: 1,
    IsClick: true,
    TabID: { ContactInfo: "liContactInfo", SaleInfo: "liSale", PurchaseInfo: "liPurchase", HistoryInfo: "liHistory" },
    init: function () {
        ContactView.initTab();
        ContactView.bindHistory();
        ContactView.clickAction();
        IVBase.bindEvent = function () {
            ContactView.bindInvoice(ContactView.CurrentTabID);
        }
    },
    clickAction: function () {
        $("#person").click(function () {
            if (!$(this).hasClass("hide")) { return false; }
            $(this).removeClass("hide");
            $("#person-cont").removeClass("contHide");
            $("#finances").addClass("hide");
            $("#finances-cont").addClass("contHide");
        });
        $("#finances").click(function () {
            if (!$(this).hasClass("hide")) { return false; }
            $(this).removeClass("hide");
            $("#finances-cont").removeClass("contHide");
            $("#person").addClass("hide");
            $("#person-cont").addClass("contHide");
        });
        $('a[class*="edit-arrow"]').click(function () {
            //var href = $(this).attr('href');
            //mWindow.reload(href);
        });
        $("#aSaveNote").click(function () {
            ContactView.saveHistoryNote();
        });
    },
    initTab: function () {
        $("#divInvoice").hide();
        $("#divHistoryNote").hide();
        $(".m-extend-tabs").tabsExtend({
            onSelect: function (index) {
                var curTab = $(".m-extend-tabs li:eq(" + index + ")");
                var curTabId = curTab.attr("id");
                switch (curTabId) {
                    case ContactView.TabID.ContactInfo:
                        $("#divContact").show();
                        $("#divInvoice").hide();
                        $("#divHistoryNote").hide();
                        break;
                    case ContactView.TabID.SaleInfo:
                    case ContactView.TabID.PurchaseInfo:
                        var type = curTabId == ContactView.TabID.SaleInfo ? "'Invoice_Sale','Invoice_Sale_Red'" : "'Invoice_Purchase','Invoice_Purchase_Red'";
                        ContactView.InitIncomingChart(type);
                        $("#divContact").hide();
                        $("#divInvoice").show();
                        $("#divHistoryNote").hide();
                        if (index == 1) {
                            $("#divSales").show();
                            $("#divPurchases").hide();
                        }
                        else {
                            $("#divSales").hide();
                            $("#divPurchases").show();
                        }
                        ContactView.bindInvoice(curTabId);
                        ContactView.CurrentTabID = curTabId;
                        $(window).resize();
                        break;
                    case ContactView.TabID.HistoryInfo:
                        ContactView.bindHistory();
                        $("#divContact").hide();
                        $("#divInvoice").hide();
                        $("#divHistoryNote").show();
                        $(window).resize();
                        break;
                }
            }
        });
    },
    reload: function () {
        location.reload();
    },
    InitOverPastChart: function () {
        var ContactID = $("#ContactID").val();
        mAjax.post(
            "/BD/Contacts/GetOverPastData",
            { contactID: ContactID },
            function (jsonData) {
                if (jsonData != null) {
                    ContactView.fillOverPastData(JSON.parse(jsonData));
                }
            });
    },
    fillOverPastData: function (jsonData) {
        if (jsonData.data.length == 0) {
            return;
        }
        var data = jsonData.data;
        var labels = jsonData.labels;
        var scalSpace = jsonData.scalSpace;
        new iChart.ColumnStacked2D({
            render: 'divOverPastChart',
            data: data,
            sub_option: {
                label: false
            },
            labels: labels,
            border: 0,
            decimalsnum: 2,
            width: 540,
            height: 150,
            label: { color: '#7c8490', font: 'Arial', fontsize: 12 },
            default_mouseover_css: false,
            background_color: null,
            align: 'left',
            tip: {
                enable: true,
                listeners: {
                    //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                    parseText: function (tip, name, value, text, i) {
                        for (var j = 0; j < data.length; j++) {
                            if (data[j].name == name) {
                                return "<span style=\"color:" + data[j].color + ";font-size:12px;display:inline;\">" + mText.encode(name) + ": </span>" +
                                       "<span style=\"display:inline;color:#7c8490;\">" + value + "</span>";
                            }
                        }
                    }
                }
            },
            legend: {
                enable: true,
                background_color: "rgba(254,254,254,0.2)",
                color: "#c1cdde",
                fontsize: 10,
                sign: 'round',
                border: {
                    width: 0
                },
                column: 2,
                align: "left",
                valign: "top",
                offsetx: -10,
                offsety: -20
            },
            coordinate: {
                background_color: null,
                scale: [{
                    position: 'right',
                    start_scale: 0,
                    scale_space: scalSpace,
                    end_scale: scalSpace * 3
                }]
            }
        }).draw();
    },
    bindInvoice: function (curTabId) {
        var columnList = null;
        var queryParam = {}
        var hasChangeAuth = false;
        switch (curTabId) {
            case ContactView.TabID.SaleInfo:
                columnList = Megi.getDataGridColumnList(IVBase.columns, [1, 2, 3, 4, 5, 7, 8, 9, 10, 11, 12]);
                queryParam.MType = IVBase.InvoiceType.Sale;
                $("#hidType").val('Invoice');
                hasChangeAuth = $("#hidChangeInvoiceAuth").val() == "1";
                break;
            case ContactView.TabID.PurchaseInfo:
                columnList = Megi.getDataGridColumnList(IVBase.columns, [2, 3, 4, 5, 7, 8, 9, 11, 12]);
                queryParam.MType = IVBase.InvoiceType.Purchase;
                $("#hidType").val('Bill');
                hasChangeAuth = $("#hidChangeBillAuth").val() == "1";
                break;
        }
        if ($(".m-adv-search").css("display") == "block") {
            queryParam = $("body").mFormGetForm();
        }
        queryParam.MStatus = 0;
        queryParam.MContactID = $("#ContactID").val();

        Megi.grid("#gridInvoice", {
            url: IVBase.url_getlist,
            pagination: true,
            queryParams: queryParam,
            columns: columnList
        });
        ContactView.resize();
    },
    resize: function () {
        var w = $(".m-imain-content").width();
        $("#divList").css({ "width": w + "px" });
        try {
            $("#gridInvoice").datagrid('resize', {
                width: $("#divList").width()
            });
        } catch (exc) { }
    },
    editContactInfo: function (id, tabIndex) {
        $.mDialog.show({
            mTitle: id == "" ? HtmlLang.Write(LangModule.Contact, "AddContact", "Add Contact") : HtmlLang.Write(LangModule.BD, "EditContact", "Edit Contact"),
            mContent: "iframe:" + '/BD/Contacts/ContactsEdit/' + id + "?tabIndex=" + tabIndex,
            mWidth: 1080,
            mHeight: 450,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle",
            mCloseCallback: function () {
                location.reload(true);
            }
        });
        //$.mDialog.show({
        //    mTitle: HtmlLang.Write(LangModule.BD, "EditContact", "Edit Contact"),
        //    mContent: "iframe:" + '/BD/Contacts/ContactsEdit/' + id + "?tabIndex=1&tabTitle=",
        //    mWidth: 1100,
        //    mHeight: 450,
        //    mShowbg: true,
        //    mShowTitle: true,
        //    mDrag: "mBoxTitle"
        //});
    },
    InitIncomingChart: function (type) {
        //动态数据：data,labels,tip,coordinate
        mAjax.post(
            "/IV/Invoice/GetInComingChartData",
            { Type: type, contactId: $("#ContactID").val() },
            function (jsonData) {
                if (jsonData != null) {
                    ContactView.fillStackedData(JSON.parse(jsonData));
                }
            });
    },
    /*用来做渐变的颜色*/
    chartColors: ['#0000FF', '#0033FF', '#0066FF', '#0099FF', '#00CCFF'],
    /*获取渐变色的数组*/
    getGradientColors: function (dataCount) {
        //声称渐变色
        return mColor.Gradient(ContactView.chartColors[0], ContactView.chartColors[ContactView.chartColors.length - 1], dataCount);
    },
    /*给每个data的color赋值*/
    setDataColor: function (data) {
        //
        var colors = ContactView.getGradientColors(data.length);
        //每个data都赋值
        for (var i = 0; i < data.length ; i++) {
            //赋值
            data[i].color = colors[i];
        }
        return data;
    },
    fillStackedData: function (jsonData) {
        if (jsonData.data.length == 0) {
            var defaultData = {};
            defaultData.value = [0, 0, 0, 0, 0, 0];
            jsonData.data.push(defaultData);

        }
        var data = jsonData.data;
        var labels = jsonData.labels;
        var scalSpace = jsonData.scalSpace;
        //等待支付的多语言
        var owingL = HtmlLang.Write(LangModule.IV, "Owing", "owing");
        //逾期的多语言
        var dueL = HtmlLang.Write(LangModule.IV, "Due", "due");
        new iChart.ColumnStacked2D({
            render: 'divOverPastChart',
            data: ContactView.setDataColor(data),
            sub_option: {
                label: false,
                listeners: {
                    click: function (r, e, m) {
                        $.mTab.addOrUpdate(HtmlLang.Write(LangModule.IV, "ViewStatement", "View Statement"), "/IV/Invoice/ViewStatement?statementType=Outstanding&statementContactID=" + $("#ContactID").val());
                    }
                }
            },
            //labels: ['Older', 'Oct', 'Nov', 'Dec', 'Jan', 'Feb'],
            labels: labels,
            border: 0,
            decimalsnum: 2,
            width: 480,
            height: 168,
            //offsetx: 35,
            label: { color: '#7c8490', font: 'Arial', fontsize: 12 },
            default_mouseover_css: false,
            background_color: null,
            tip: {
                enable: true,
                listeners: {
                    //tip:提示框对象、name:数据名称、value:数据值、text:当前文本、i:数据点的索引
                    parseText: function (tip, name, value, text, i, j) {
                        var nameOj = $.parseJSON(name);
                        var firstName = nameOj.MChartFirstName;
                        var lastName = nameOj.MChartLastName;
                        var money = Megi.Math.toMoneyFormat(value);

                        var dueOrOwing = nameOj.MChartDueOrOwing.split(',')[i];

                        dueOrOwing = dueOrOwing == "1" ? owingL : (dueOrOwing == "-1" ? dueL : "");
                        return "<span class=\"m-chart-font-blue14\">" + nameOj.name + "</span><br />" +
                                  "<span class=\"m-chart-font-gray\">" + firstName + " " + lastName + "</span><br />" +
                                  "<span class=\"m-chart-font-blueBold16\">" + money + " " + dueOrOwing + "</span>";
                    }
                }
            },
            coordinate: {
                width: "100%",
                background_color: null,
                axis: {
                    width: [0, 0, 1, 0],
                    color: '#666666'
                },
                scale: {
                    scale_enable: false,
                    scale_share: 0,
                },
                height: '85%'
            },

        }).draw();
    },
    //绑定列表
    bindHistory: function () {
        var param = { MStatus: 0, MContactID: $("#ContactID").val(), PageSize: 100000000 };
        mAjax.post(
            IVBase.url_getlist,
            { param: param },
            function (response) {
                if (response) {
                    var InvoiceIds = [];
                    $.each(response.rows, function (i, item) {
                        InvoiceIds.push(item.MID);
                    });

                    var gridOpts = {
                        resizable: true,
                        auto: true,
                        pagination: true,
                        queryParams: { MPKID: InvoiceIds.toString() },
                        columns: [[
                            //操作类型
                            { title: LangKey.Operation, field: 'MAction', width: 80, sortable: false },
                            //操作日期
                            {
                                title: LangKey.Date, field: 'MCreateDate', width: 100, align: 'center', sortable: false, formatter: function (value, row, index) {
                                    return $.mDate.format(row.MCreateDate);
                                }
                            },
                            //操作用户
                            { title: LangKey.User, field: 'MUserName', width: 100, sortable: false },
                            //操作明细
                            {
                                title: LangKey.Details, field: 'MNote', width: 320, sortable: false, formatter: function (value, row, index) {
                                    if (!value) {
                                        return "";
                                    }
                                    return value.format(row.MValue1, row.MValue2, row.MValue3, row.MValue4, row.MValue5, row.MValue6, row.MValue7, row.MValue8, row.MValue9, row.MValue10);
                                }
                            }
                        ]],
                        onLoadSuccess: function (data) {
                            if (data) {
                                $("#liHistory .title").text(data.total + " " + HtmlLang.Write(LangModule.Common, "items", "items"));
                            }
                        }
                    };

                    if (InvoiceIds.length > 0) {
                        gridOpts.url = "/Log/Log/GetBussinessLogList";
                    }
                    else {
                        gridOpts.data = new Array();
                    }

                    Megi.grid('#gridBusLog', gridOpts);
                }
            });
    },
    saveHistoryNote: function () {
        //备注
        var txtNote = $("#txtNote");
        var note = $.trim(txtNote.val());
        if (note == "") {
            txtNote.focus();
            return;
        }
        //提交
        $("#divNotes").mFormSubmit({
            url: "/BD/Contacts/AddContactNoteLog",
            param: { model: { MItemID: $("#ContactID").val() }, MDesc: note },
            callback: function (msg) {
                if (msg.Success == true) {
                    //提示信息
                    $.mMsg(LangKey.SaveSuccessfully);
                    //保存成功，刷新列表
                    ContactView.bindHistory();
                    //清空备注
                    txtNote.val("");
                } else {
                    //保存失败，提示错误
                    $.mDialog.alert(msg.Message);
                }
            }
        });
    },
    getSaleReportUrl: function (contactName, title, reportTypeId, parentReportTypeId) {
        var date = new Date();
        var monthLastDay = $.mDate.format(new Date(date.getFullYear(), date.getMonth() + 1, 0));
        var filter = {
            AgedByField: "2",
            AgedType: "2",
            AsAt: monthLastDay,
            DateFrom: "1900-01-01",
            DateTo: monthLastDay,
            MContactID: $("#ContactID").val(),
            MContactName: contactName,
            IsFromContactView: true
        };
        var url = "/Report/Report2/" + reportTypeId + "/0/" + parentReportTypeId + "?filter=" + encodeURI($.toJSON(filter));
        $.mTab.addOrUpdate(title, url);
    }
}

$(document).ready(function () {
    ContactView.init();
});