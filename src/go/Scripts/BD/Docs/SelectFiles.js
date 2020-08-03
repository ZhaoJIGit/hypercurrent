/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />
var SelectFiles = {
    init: function () {
        var folderId = $("#hdnCurFolderID").val();
        if ($.browser.msie && parseInt($.browser.version) === 8 || $.browser.safari) {
            $('.m-box-select-files').css('width', '100%').css('width', '-=170px');
        }
        SelectFiles.loadFolderInfo(folderId);
        SelectFiles.bindAction();
    },
    loadFolderInfo: function (folderId) {
        $("#tbFolders tr td").removeClass("current");
        var currentTr = $("#tbFolders").find("tr[data-id=" + folderId + "]");
        currentTr.find("td").addClass("current");
        SelectFiles.loadFileList(folderId);
    },
    bindAction: function () {
        $("#tbFolders tr").live("click", function () {
            $("#tbFolders td").removeClass("current");
            $(this).find("td").addClass("current");
            var itemId = $(this).attr("data-id");
            SelectFiles.loadFolderInfo(itemId);
            return false;
        });
        $("#aAddFiles").click(function () {
            Megi.grid("#tbFileList", "optSelected", {
                callback: function (ids) {
                    parent.AssociateFiles.createAssociation(ids);
                    Megi.closeDialog();
                }
            });
        });
        $("#chkAll").off("click.bx").on("click.bx", function () {
            var selectedCount = 0;
            if (this.checked) {
                $("#tbFileList").parent().find(".row-key-checkbox").each(function () {
                    if ($(this).attr("checked") == "checked") {
                        selectedCount++;
                    }
                });
                if (selectedCount == 0) {
                    this.checked = false;
                    $("#tbFileList").datagrid("clearSelections");
                }
            }
            SelectFiles.changeAddFilesText(selectedCount);
        });
    },
    getSelectedFileIds:function() {
        var rows = $("#tbFileList").datagrid("getSelections");
        var fileIds = [];
        for (var i = 0; i < rows.length; i++) {
            fileIds.push(rows[i].MItemID);
        }
        return fileIds.toString();
    },
    changeAddFilesText: function (selectedCount) {
        var btnAddFilesText = HtmlLang.Write(LangModule.Docs, "AddFiles", "Add Files");
        if (selectedCount > 0) {
            if (selectedCount > 1) {
                btnAddFilesText = HtmlLang.Write(LangModule.Docs, "AddMultiFiles", "Add {0} Files").replace("{0}", selectedCount);
            } else {
                btnAddFilesText = HtmlLang.Write(LangModule.Docs, "AddOneFile", "Add 1 File");
            }
        }
        $("#aAddFiles .l-btn-text").text(btnAddFilesText);
    },
    loadFileList: function (folderId) {
        var queryParam = {};
        queryParam.categoryId = folderId;
        var associateFileIds = parent.AssociateFiles.getAssociatedFileIds();
        Megi.grid('#tbFileList', {
            resizable: true,
            auto: true,
            url: "/BD/Docs/GetFileList",
            queryParams: queryParam,
            pagination: true,
            columns: [[
                {
                    title: '<input id="chkAll" type=\"checkbox\" >', formatter: function (value, row, rowIndex) {
                        return "<input type=\"checkbox\" class=\"row-key-checkbox\" " + (associateFileIds.indexOf(row.MItemID) > -1 ? "disabled=\"disabled\"" : "") + " value=\"" + row.MItemID + "\" >";
                    }, width: 20, align: 'center', sortable: true
                },
                { title: HtmlLang.Write(LangModule.Docs, "File", "File"), field: 'MName', width: 210, align: 'left', sortable: true },
                {
                    title: HtmlLang.Write(LangModule.Docs, "Size", "Size"), field: 'MSize', width: 80, sortable: true, formatter: function (value, row, index) {
                        return FileBase.formatFileSize(value);
                    }
                },
                { title: HtmlLang.Write(LangModule.Docs, "Uploaded", "Uploaded"), field: 'MCreateDateFormated', width: 100, sortable: true },
                { title: HtmlLang.Write(LangModule.Docs, "UploadedBy", "Uploaded by"), field: 'MCreatorName', width: 80, sortable: true }]],
                onCheck: function (rowIndex, rowData) {
                    var rows = $("#tbFileList").datagrid("getSelections");
                    SelectFiles.changeAddFilesText(rows.length + 1);
                },
                onUncheck: function (rowIndex, rowData) {
                    var rows = $("#tbFileList").datagrid("getSelections");
                    SelectFiles.changeAddFilesText(rows.length - 1);
                },
                onClickRow: function (rowIndex, rowData) {
                    if ($("#divFileList .datagrid-view2 .datagrid-body tr:eq(" + rowIndex + ")").find("input:checkbox").length == 0) {
                        $(this).datagrid('unselectRow', rowIndex);
                    }
                },
                onLoadSuccess:function() {
                    var rowsWithCheckbox = $("#divFileList").find("input:checkbox");
                    var rowsData = $("#tbFileList").datagrid("getRows");
                    if (rowsWithCheckbox.length == 1 || rowsWithCheckbox.length - 1 < rowsData.length) {
                        $("#chkAll").attr("disabled", "disabled");
                    }
                }
        });
    },
    getCurrentFolderId: function() {
        return $("#tbFolders tr td[class*=current]").parent().attr("data-id");
    }
}

$(document).ready(function () {
    SelectFiles.init();
});