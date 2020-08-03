var Report = {
    IsDraftClick: false,
    IsPublishedClick: false,
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    accountStandard: $("#hideAccountStandard").val(),
    init: function () {
        $("#tabReport").tabs({
            onSelect: function (title, index) {
                switch (index) {
                    case 1:
                        Report.bindDraftReport();
                        break;
                    case 2:
                        Report.bindPublishedReport();
                        break;
                    case 3:
                        Report.bindArchiveReport();
                        break;
                }
                $(window).resize();
            }
        });
        var tabType = $("#hidTabType").val();
        switch (tabType) {
            case "1":
                $('#tabReport').tabs('select', 1);
                break;
            case "2":
                $('#tabReport').tabs('select', 2);
                break;
            case "3":
                $('#tabReport').tabs('select', 3);
                break;
        }
    },
    bindDraftReport: function () {
        Megi.grid("#gridDraftReport", {
            url: "/Report/RptManager/GetDraftReportList",
            resizable: true,
            auto: true,
            pagination: true,
            columns: [[{ title: HtmlLang.Write(LangModule.Report, "Title", "Title"), field: 'MTitle', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "Subtitle", "Subtitle"), field: 'MSubtitle', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "ReportDate", "Report date"), field: 'MReportDate', width: 100, formatter: $.mDate.formatter },
                       { title: HtmlLang.Write(LangModule.Report, "Author", "Author"), field: 'MAuthor', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "CreatedDate", "Created date"), field: 'MCreateDate', width: 100, formatter: $.mDate.formatter },
                       {
                           title: HtmlLang.Write(LangModule.Report, "Operation", "Operation"), field: 'Action', align: 'center', width: 50, sortable: false, formatter: function (value, rec, rowIndex) {
                               var editFuc = "$.mTab.addOrUpdate($(this).attr('title'), '/Report/Report2/" + rec.MType + '/' + rec.MID + "')";
                               var html = "<div class='list-item-action'>";
                               html += "<a href=\"javascript:void(0);\" title='" + mText.encode(rec.MTitle) + "' onclick=\"" + editFuc + "\" class='list-item-edit'></a>";
                               html += "</div>";
                               return html;
                           }
                       }
            ]]
        });
    },
    bindPublishedReport: function () {
        Megi.grid("#gridPublishedReport", {
            url: "/Report/RptManager/GetPublishedReportList",
            resizable: true,
            auto: true,
            pagination: true,
            columns: [[
                       {
                           title: HtmlLang.Write(LangModule.Report, "Title", "Title"), field: 'MTitle', width: 100, formatter: function (value, row, index) {
                               return "<a href='javascript:void(0);' title = '" + mText.encode(row.MTitle) + "' onclick=\"Report.viewReport($(this).attr('title'), '" + row.MType + "', '" + row.MID + "');" + "\" >" + value + "</a>";
                           }
                       },
                       { title: HtmlLang.Write(LangModule.Report, "Subtitle", "Subtitle"), field: 'MSubtitle', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "ReportDate", "ReportDate"), field: 'MReportDate', width: 100, formatter: $.mDate.formatter },
                       { title: HtmlLang.Write(LangModule.Report, "Author", "Author"), field: 'MAuthor', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "PublishedDate", "Published date"), field: 'MPublishDate', width: 100, formatter: $.mDate.formatter },
                        {
                            title: HtmlLang.Write(LangModule.Common, "Export", "Export"), field: 'MID', width: 40, align: 'center', formatter: function (value, row, index) {
                                var html = '';
                                html += '<a href="/Report/RptManager/Export/' + value + "?type=Pdf&reportTypeId=" + row.MType + '" onclick="Report.exportMsg();">pdf</a>&nbsp;&nbsp;';
                                html += '<a href="/Report/RptManager/Export/' + value + "?type=Xls&reportTypeId=" + row.MType + '" onclick="Report.exportMsg();">xls</a>';
                                return html;
                            }
                        }]]
        });
    },
    exportMsg: function () {
        $.mMsg(HtmlLang.Write(LangModule.Common, "Exporting", "Exporting..."));
    },
    viewReport: function (MTitle, MType, MID) {
        $.mTab.addOrUpdate(MTitle, "/Report/Report2/View/" + MType + "/" + MID, false);
    },
    bindArchiveReport: function () {
        Megi.grid("#gridArchivedReport", {
            url: "",
            resizable: true,
            auto: true,
            pagination: true,
            columns: [[{ title: HtmlLang.Write(LangModule.Report, "Title", "Title"), field: 'a', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "Subtitle", "Subtitle"), field: 'b', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "ReportDate", "Report date"), field: 'c', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "Author", "Author"), field: 'd', width: 100 },
                       { title: HtmlLang.Write(LangModule.Report, "ArchivedDate", "ArchivedDate"), field: 'e', width: 100 }]],
            onDblClickCell: function (rowIndex, field, value) {
                Report.IsDraftClick = true;
                if (field == "MID") {
                    Report.IsDraftClick = false;
                }
            },
            onDblClickRow: function (rowIndex, rowData) {
                if (Report.IsClick) {
                }
            }
        });
    }
}

$(document).ready(function () {
    Report.init();
});