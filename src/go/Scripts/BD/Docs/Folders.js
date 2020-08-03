/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var Folders = {
    uploadDoneCount: 0,
    arrUploadErrorRowIdx: [],
    selectedFileCount: 0,
    folderAction: { add: 'add', del: 'del', rename: 'rename', hide: 'hide' },
    bizObject: '',
    init: function () {
        var folderId;
        if ($("#tbFolders tr").length > 0) {
            if (window.location.hash) {
                folderId = window.location.hash.substring(1);
            }
            if (!folderId) {
                folderId = $("#tbFolders tr:first").attr("data-id");
            }
        }
        if ($.browser.msie && parseInt($.browser.version) === 8 || $.browser.safari) {
            $('.m-box-folder-files').css('width', '100%').css('width', '-=180px');
        }

        Folders.bindAction();
        Folders.bindFileUpload();
        Folders.loadFolderInfo(folderId);
        $("#tbFolders tr:last").find("td").addClass("cell-last");
        Folders.initActionStatus();
    },
    loadFolderInfo: function (folderId) {
        window.location.hash = folderId;
        if (typeof (folderId) != 'undefined') {
            var currentTr = $("#tbFolders").find("tr[data-id=" + folderId + "]");
            Folders.bizObject = currentTr.find("input[name='hdnBizObject']").val();
            Folders.switchOptionVisible();
            Folders.reloadMoveToList(Folders.folderAction.hide, folderId);
            $("#tbFolders tr td").removeClass("current");
            currentTr.find("td").addClass("current");
            $("#divCurrentFolder").text(currentTr.find("input[name='hdnOriCategoryName']").val());
        } else {
            $("#divCurrentFolder").text("");
        }

        if (Folders.getFileCount(folderId) == 0 || typeof (folderId) == 'undefined') {
            //$("#divUploadPanel").show();
            //$('#divFileList').hide();
        } else {
            $("#divUploadPanel").hide();
            $('#divFileList').show();
        }
        Folders.loadFileList(folderId);
    },
    bindAction: function () {
        $("#aSave").click(function () {
            if (!$("#divFolderInput").mFormValidate()) {
                return;
            }
            $(".m-imain").mask("");
            Folders.updateFolderInfo();
            return false;
        });
        $("#aAddFolder").click(function () {
            $(".m-imain").mask("");
            Folders.addFolder();
            return false;
        });
        $("#aCancel").click(function () {
            $("#divCurrentFolder").show();
            $("#divFolderInput").hide();
        });
        $("#divRename").click(function () {
            if (!$(this).hasClass("l-btn-disabled")) {
                $("#divCurrentFolder").hide();
                $("#divFolderInput").show();
                $("#txtFolder").val($("#divCurrentFolder").text()).focus();
            };
        });
        $("#divDelete").click(function () {
            $.mDialog.confirm(LangKey.AreYouSureToDelete,
            {
                callback: function () {
                    $(".m-imain").mask("");
                    Folders.updateFolderInfo(true);
                }
            });
        });
        $('#divUploadPanel a, #aUploadFile').click(function () {
            if (!$(this).hasClass("l-btn-disabled")) {
                $('#fileInput').click();
            }
        });
        $("#tbFolders tr").live("click", function () {
            if ($("#divFolderInput").is(":visible")) {
                $("#divFolderInput").hide();
                $("#divCurrentFolder").show();
            }
            $("#tbFolders td").removeClass("current");
            $(this).find("td").addClass("current");
            var itemId = $(this).attr("data-id");
            Folders.loadFolderInfo(itemId);
            return false;
        });
        $('#divFileList input:checkbox').live("change", function () {
            var isAnyCheck = $('#divFileList input:checked').length > 0 && $("#divFileList .datagrid-view2 .datagrid-body input:checkbox").length > 0;
            if (!$(this).hasClass("row-key-checkbox") && !this.checked) {
                isAnyCheck = false;
            }
            Folders.enableFileAction(isAnyCheck);
        });
        $("#aDelFile").click(function () {
            if (!$(this).hasClass("l-btn-disabled")) {
                $.mDialog.confirm(LangKey.AreYouSureToDelete,
                {
                    callback: function () {
                        Folders.updateFileInfo(true);
                    }
                });
            }
        });
        $("#divMoveTo .menu-item").live('click', function () {
            var folderId = this.tagName == "a" ? $(this).attr("data-id") : $(this).find("a").attr("data-id");
            if (folderId != "") {
                Folders.moveFiles(folderId);
            }
        });
        $("#fileInput").change(function () {
            //Folders.selectedFileCount = ImportBase.selectFileCount();
            Folders.bindFileUpload();
        });
        $(document).on('drop dragover', function (e) {
            e.preventDefault();
        });
        $("#divMoveTo div[class*='menu-item']").live('mouseover', function () {
            $("#divMoveTo div[class*='menu-item']").removeClass("menu-active");
            $(this).toggleClass("menu-active");
        });
    },
    bindFileUpload: function () {
        if ($.browser.safari) {
            $('#fileInput').removeAttr("multiple");
        }
        $('#fileInput').fileupload({
            //dropZone: $('#divUploadPanel'),
            autoUpload: true,
            url: '/BD/Docs/UploadFile',
            iframe: ImportBase.isIE9Previous,
            formData: { id: Folders.getCurrentFolderId() },
            dataType: 'json',
            limitConcurrentUploads: 1,
            sequentialUploads: true,
            progressInterval: 100,
            add: function (e, data) {
                if (data.originalFiles && data.files && data.files[0].name == data.originalFiles[0].name) {
                    Folders.selectedFileCount = data.originalFiles.length;
                    $('#fileInput').fileupload({
                        formData: { id: Folders.getCurrentFolderId() }
                    });
                }
                var validateResult = FileBase.validateFile(data.files[0].name, data.files[0].size);
                if (validateResult) {
                    Folders.selectedFileCount--;
                    $.mDialog.alert(validateResult);
                    return;
                }

                if ($("#divFileList").is(":hidden")) {
                    $("#divUploadPanel").hide();
                    $('#divFileList').show();
                    Folders.bindFileList();
                }
                var newFileObj = {};
                newFileObj.MName = data.files[0].name;
                newFileObj.MSize = data.files[0].size;
                newFileObj.MCreatorID = $("#hdnCurUserID").val();
                Megi.grid("#tbFileList", "insertRow", { row: newFileObj });

                var currentRow = $("#divFileList .datagrid-view2 .datagrid-body table tr:last");
                data.context = currentRow.find("td[field='MCreateDate']");
                data.context.find('.progress-wrapper').show();
                data.context.find("span").hide();

                Folders.enableFileAction(false);
                data.submit().success(function (data, textStatus, jqXHR) {
                    data = data.Data || data;
                    Folders.uploadDoneCount++;
                    if (data != undefined && data.isSuccess != true) {
                        Folders.arrUploadErrorRowIdx.push(currentRow.attr("datagrid-row-index"));
                        if (Folders.uploadDoneCount == Folders.selectedFileCount) {
                            for (var i = Folders.arrUploadErrorRowIdx.length - 1; i >= 0; i--) {
                                Megi.grid("#tbFileList", "deleteRow", Folders.arrUploadErrorRowIdx[i]);
                            }
                        }
                        $.mDialog.alert(data.Message);
                    }
                    if (Folders.uploadDoneCount == Folders.selectedFileCount) {
                        if (Folders.arrUploadErrorRowIdx.length < Folders.selectedFileCount) {
                            var folderId = Folders.getCurrentFolderId();
                            var totle = Folders.getFileCount(folderId) + Folders.uploadDoneCount - Folders.arrUploadErrorRowIdx.length;
                            Folders.updateFileCount(folderId, totle);
                            Folders.loadFolderInfo(folderId);
                        }
                        Folders.uploadDoneCount = 0;
                        Folders.arrUploadErrorRowIdx = [];
                    }
                })
                .error(function (data, textStatus, errorThrown) {
                    if (typeof (data) != 'undefined' || typeof (textStatus) != 'undefined' || typeof (errorThrown) != 'undefined') {
                        $.mDialog.alert(textStatus + ":" + errorThrown + "(" + data.status + ")");
                    }
                });
            },
            progress: function (e, data) {
                var progress = parseInt(data.loaded / data.total * 100, 10);
                if (($.browser.safari || ImportBase.isIE9Previous) && progress == 100) {
                    progress = 99;
                }
                data.context.find('.progress').width(progress + "%");
            },
            done: function (e, data) {
            },
            fail: function (event, data) {
                if (data.files[0].error) {
                    $.mDialog.alert(data.files[0].error);
                }
            }
        });
    },
    initActionStatus: function () {
        if ($("#tbFolders tr").length == 0) {
            Folders.enableFileAction(false);
            $("#aUploadFile").linkbutton("disable");
            $("#aOptions").splitbutton("disable");
        } else {
            //Folders.enableFileAction(true);
            $("#aUploadFile").linkbutton("enable");
            Folders.switchOptionVisible();
        }
    },
    switchOptionVisible: function () {
        if (Folders.bizObject) {
            $("#aOptions").splitbutton("disable");
        }
        else {
            $("#aOptions").splitbutton();
        }
    },
    getFileCount: function (folderId) {
        var currentTr = $("#tbFolders").find("tr[data-id=" + folderId + "]");
        return parseInt(currentTr.find("span").text());
    },
    updateFileCount: function (folderId, count) {
        var spAttachCount = $("#tbFolders").find("tr[data-id=" + folderId + "]").find("span");
        spAttachCount.text(count);
    },
    addFolder: function () {
        mAjax.submit("/BD/Docs/FolderAdd", null, function (response) {
            if (response.Success == true) {
                Folders.reLoadFolderList(response.ObjectID, function () {
                    Folders.loadFolderInfo(response.ObjectID);
                    $("#divCurrentFolder").hide();
                    $("#divFolderInput").show();
                    $("#txtFolder").val($("#divCurrentFolder").text()).select();
                    Folders.initActionStatus();
                    Folders.reloadMoveToList(Folders.folderAction.add, response.ObjectID, $("#divCurrentFolder").text()); $(".m-imain").unmask();
                    $(".m-imain").unmask();
                });
            } else {
                $(".m-imain").unmask();
                $.mDialog.alert(response.Message);
            }
        });
    },
    updateFolderInfo: function (isDel) {
        var obj = {};
        obj.MItemID = Folders.getCurrentFolderId();
        obj.MCategoryName = isDel ? $("#divCurrentFolder").text() : $("#txtFolder").val();
        obj.IsDelete = isDel;
        mAjax.submit("/BD/Docs/FolderEdit", { model: obj }, function (response) {
            if (response.Success == true) {
                var currentTr = $("#tbFolders").find("tr[data-id=" + obj.MItemID + "]");
                if (!isDel) {
                    currentTr.find("label").text(response.MCategoryNameEllipsis);
                    currentTr.find("input[name='hdnOriCategoryName']").val(obj.MCategoryName);
                    if ($("#divCurrentFolder").text() != $("#txtFolder").val()) {
                        Folders.reLoadFolderList(obj.MItemID);
                    }
                    $("#divCurrentFolder").text(obj.MCategoryName);
                    Folders.reloadMoveToList(Folders.folderAction.rename, obj.MItemID, obj.MCategoryName);

                    $("#hideFolderCount").val(+$("#hideFolderCount").val() + 1);

                } else {
                    currentTr.remove();
                    var firFolderId;
                    if ($("#tbFolders tr").length > 0) {
                        firFolderId = $("#tbFolders tr:first").attr("data-id");
                    }
                    Folders.reloadMoveToList(Folders.folderAction.del, obj.MItemID);
                    Folders.loadFolderInfo(firFolderId);

                    $("#hideFolderCount").val(+$("#hideFolderCount").val() - 1);
                }
                $("#divCurrentFolder").show();
                $("#divFolderInput").hide();
                Folders.initActionStatus();
                $(".m-imain").unmask();
            } else {
                $(".m-imain").unmask();
                $.mDialog.alert(response.Message, function () {
                    $("#txtFolder").focus();
                });
            }
        });
    },
    reLoadFolderList: function (folderId, callback) {
        $("#divFolderList").load("/BD/Docs/FoldersPartial?reload=1", function () {
            Folders.loadFolderInfo(folderId);
            if (typeof (callback) != 'undefined') {
                callback();
            }
        });
    },
    reloadMoveToList: function (action, id, name) {
        var divMoveTo = $("#divMoveTo");
        var height = divMoveTo.height();
        var unitHeight = 32;
        switch (action) {
            case Folders.folderAction.add:
                var lastItem = $("#divMoveTo div[class*='menu-item']:last");
                var newItem = lastItem.clone();
                newItem.find("a").attr('data-id', id).text(name);
                if (lastItem.find("a").attr("data-id") == "") {
                    lastItem.remove();
                } else {
                    height += unitHeight;
                }
                divMoveTo.append(newItem).height(height);
                break;
            case Folders.folderAction.del:
                var count = divMoveTo.find("div[class*='menu-item']").length;
                var currentLink = divMoveTo.find("a[data-id='" + id + "']");
                var currentItem = currentLink.closest("div[class*='menu-item']");
                if (count == 1) {
                    currentLink.attr("data-id", "").text("");
                } else {
                    currentItem.remove();
                    divMoveTo.height(height - unitHeight);
                }
                break;
            case Folders.folderAction.rename:
                divMoveTo.find("a[data-id='" + id + "']").text(name);
                break;
            case Folders.folderAction.hide:
                divMoveTo.find("div[class*='menu-item']").show();
                divMoveTo.find("a[data-id='" + id + "']").closest("div[class*='menu-item']").hide();
                divMoveTo.height(32 * (divMoveTo.find("div[class*='menu-item']").length - 1));
                break;
        }
    },
    getSelectedFileIds: function () {
        var rows = $("#tbFileList").datagrid("getSelections");
        var fileIds = [];
        for (var i = 0; i < rows.length; i++) {
            fileIds.push(rows[i].MItemID);
        }
        return fileIds;
    },
    loadFileList: function (folderId) {
        var queryParam = {};
        queryParam.categoryId = folderId;
        Folders.bindFileList("/BD/Docs/GetFileList", queryParam);
        Folders.enableFileAction(false);
    },
    bindFileList: function (url, queryParam) {
        Megi.grid('#tbFileList', {
            resizable: true,
            auto: true,
            pagination: true,
            url: url,
            queryParams: queryParam,
            columns: [[
                {
                    title: "<input type='checkbox' >", field: 'IsSelect', formatter: function (value, row, rowIndex) {
                        return "<input type='checkbox' class='row-key-checkbox'  value='" + row.MItemID + "' >";
                    }, width: 20, align: 'center'
                },
                {
                    title: HtmlLang.Write(LangModule.Docs, "File", "File"), field: 'MName', width: 210, align: 'left', sortable: true, formatter: function (value, row, index) {
                        return "<a href='javascript:void(0)' onclick=\"Folders.viewFileInfo('" + row.MItemID + "', '" + row.MName + "');return false;\">" + value + "</a>";
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Docs, "Size", "Size"), field: 'MSize', width: 80, sortable: true, formatter: function (value, row, index) {
                        return FileBase.formatFileSize(value);
                    }
                },
                {
                    title: HtmlLang.Write(LangModule.Docs, "Uploaded", "Uploaded"), field: 'MCreateDate', width: 100, sortable: true, formatter: function (value, row, index) {
                        return "<div class='progress-wrapper hide'><div class='progress'></div></div><span>" + (value ? $.mDate.formatDateTime(value) : "") + "</span>";
                    }
                },
                { title: HtmlLang.Write(LangModule.Docs, "UploadedBy", "Uploaded by"), field: 'MCreatorName', width: 80, sortable: true }]],
            onCheck: function (rowIndex, rowData) {
                Folders.enableFileAction(true);
            },
            onUncheck: function (rowIndex, rowData) {
                var isAnyCheck = ($('#divFileList input:checked').length - 1) > 0;
                Folders.enableFileAction(isAnyCheck);
            },
            onLoadSuccess: function (data) {
                try {
                    $("#tbFileList").datagrid('resize', {
                        width: $("#divFileList").width()
                    });
                } catch (exc) { }
            }
        });
    },
    moveFiles: function (folderId) {
        var obj = {};
        var arrFileId = Folders.getSelectedFileIds();
        obj.KeyIDs = arrFileId.toString();
        obj.MOperationID = folderId;
        mAjax.submit("/BD/Docs/MoveFilesTo", { param: obj }, function (msg) {
            if (msg == true) {
                var currentFolderId = Folders.getCurrentFolderId();
                Folders.updateFileCount(currentFolderId, Folders.getFileCount(currentFolderId) - arrFileId.length);
                Folders.updateFileCount(folderId, Folders.getFileCount(folderId) + arrFileId.length);
                Folders.loadFolderInfo(currentFolderId);
            } else {
                $.mDialog.alert(msg.Message);
            }
        });
    },
    viewFileInfo: function (fileId, fileName) {
        if (fileId && fileId != 'undefined') {
            var rows = $("#tbFileList").datagrid("getRows");
            var fileIds = [];
            for (var i = 0; i < rows.length; i++) {
                fileIds.push(rows[i].MItemID);
            }

            Megi.openDialog('/BD/Docs/FileView', fileName, 'curFileId=' + fileId + '&fileIds=' + fileIds.toString(), 560, 460);
        }
        else {
            $.mAlert(HtmlLang.Write(LangModule.Docs, "WaitingUploadMsg", "The attachment is uploading , please wait a moment!"));
        }
    },
    enableFileAction: function (isEnable) {
        if (isEnable) {
            $("#aMoveTo").splitbutton("enable");
            $("#aDelFile").linkbutton("enable");
        } else {
            $("#aMoveTo").splitbutton("disable");
            $("#aDelFile").linkbutton("disable");
        }

        var folderCount = $("#hideFolderCount").val();

        if (folderCount <= 1) {
            $("#aMoveTo").splitbutton("disable");
        }

    },
    updateFileInfo: function (isDel) {
        var obj = {};
        var arrFileId = Folders.getSelectedFileIds();
        obj.KeyIDs = arrFileId.toString();

        //为空 不提交
        if (!obj.KeyIDs || obj.KeyIDs.length < 1) {
            return;
        }

        mAjax.submit("/BD/Docs/DeleteFiles", { param: obj }, function (response) {
            if (response.Success == true) {
                var folderId = Folders.getCurrentFolderId();
                Folders.updateFileCount(folderId, Folders.getFileCount(folderId) - arrFileId.length);

                for (var i = arrFileId.length - 1; i >= 0; i--) {
                    var chkObj = $("#divFileList .datagrid-view2 .datagrid-body table input:checkbox[value='" + arrFileId[i] + "']");
                    var rowIndex = chkObj.closest("tr").attr("datagrid-row-index");
                    Megi.grid("#tbFileList", "deleteRow", rowIndex);
                }
                Megi.grid("#tbFileList", 'reload');
                Folders.loadFolderInfo(folderId);
            } else {
                $.mDialog.alert(response.Message);
            }
        });
    },
    getCurrentFolderId: function () {
        var currentId = $("#tbFolders tr td[class*=current]").parent().attr("data-id");
        return currentId;
    }
}

$(document).ready(function () {
    Folders.init();
});