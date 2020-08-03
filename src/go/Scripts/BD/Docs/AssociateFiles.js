/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var AssociateFiles = {
    bizObject: '',
    bizObjectID: '',
    uploadDoneCount: 0,
    arrUploadingRowIdx: [],
    selectedFileCount: 0,
    folderId: '',
    removeRelationText: '',
    arrUploadedFileId: [],
    arrUploadingTr: [],
    isPopupTop: false,
    submitingFileList: [],//上传中的附件
    initTarget: function (bizObject, bizObjectID) {
        AssociateFiles.removeRelationText = HtmlLang.Write(LangModule.Docs, "removeFileFrom" + bizObject, "Remove from ");
        AssociateFiles.bizObject = bizObject;
        AssociateFiles.bizObjectID = bizObjectID;
        mAjax.post("/BD/Docs/GetBizObjectCategoryId/", { id: escape(bizObject) }, function (data) {
            AssociateFiles.folderId = data;
        });
    },
    init: function () {
        //if (!AssociateFiles.isEdit()) {
        //    $("#divRelatedAttach .footer").hide();
        //}
        var isNoPermission = $("#divRelatedAttach").find(".footer").length == 0;
        if (isNoPermission || !$("#divRelatedAttach .footer").is(":visible")) {
            $(".m-form-related-attach").css("min-height", 0);
            AssociateFiles.autoAdjustPopupTop();
        }

        $(".m-form-related-attach .l-btn-text").width(197);
        AssociateFiles.bindAction();
        AssociateFiles.bindFileUpload();
        var aRelatedAttachOffset = $("#aRelatedAttach").offset();
        var popupPos = "left-top";//aRelatedAttachOffset.top > 500 ? "left-top" : "left-bottom";
        var iframeWidth = parent.$("body").find("iframe").width();
        var left = -30;
        var popupWidth = $("#divRelatedAttach").outerWidth();
        if ((iframeWidth - aRelatedAttachOffset.left) < popupWidth) {
            popupPos = popupPos.replace('left', 'right');
            left = -popupWidth + 50;
        }
        AssociateFiles.isPopupTop = popupPos.lastIndexOf("top") != -1;
        Megi.popup("#aRelatedAttach", {
            selector: "#divRelatedAttach", paddingBottom: -10, left: left, position: popupPos, hideCallBack: function () {
                $("#tbFileList .file-delete-options").hide();
            }
        });
        $("#divRelatedAttach").find(".l-btn-text").css("padding", "0px 0px");
    },
    isEdit: function () {
        return $("#hidHaveAttachChangePermission").val() === "True";
    },
    bindAction: function () {
        $("#aAddFromFilesLib").click(function () {
            Megi.dialog({
                title: HtmlLang.Write(LangModule.Docs, "ChooseFiles", "Choose unattached files from your library"),
                width: 897,
                height: 500,
                href: '/BD/Docs/SelectFiles'
            });
        });
        $('#aUploadFiles').click(function () {
            $('#fileInput').click();
        });
        $("#fileInput").change(function () {
            //AssociateFiles.selectedFileCount = ImportBase.selectFileCount();
            AssociateFiles.bindFileUpload();
        });
        /*$(document).off("click.assoFile").on("click.assoFile", function (e) {
			if($("#tbFileList").is(":hidden") && e.){
				$("#tbFileList .file-delete-options").hide();
			}
        });*/
    },
    bindFileUpload: function () {
        if ($.browser.safari) {
            $('#fileInput').removeAttr("multiple");
        }
        $('#fileInput').fileupload({
            dropZone: AssociateFiles.isEdit() ? $("body") : null,
            autoUpload: true,
            url: '/BD/Docs/UploadFile',
            formData: { id: AssociateFiles.folderId },
            iframe: ImportBase.isIE9Previous,
            dataType: 'json',
            limitConcurrentUploads: 1,
            sequentialUploads: true,
            progressInterval: 100,
            add: function (e, data) {
                if (data.originalFiles && data.files && data.files[0].name == data.originalFiles[0].name) {
                    AssociateFiles.selectedFileCount = data.originalFiles.length;
                    $('#fileInput').fileupload({
                        formData: { id: AssociateFiles.folderId }
                    });
                    //拖入上传时自动弹出
                    //if ($("#divRelatedAttach").is(":hidden")) {
                    //    $("#aRelatedAttach").trigger("click.popup");
                    //}
                }
                var validateResult = FileBase.validateFile(data.files[0].name, data.files[0].size);
                if (validateResult) {
                    AssociateFiles.selectedFileCount--;
                    $.mDialog.alert(validateResult);
                    return;
                }

                var fileArray = [];
                var newFile = {};
                newFile.MName = data.files[0].name;
                newFile.MSize = data.files[0].size;
                newFile.MItemID = '';
                newFile.RelationID = '';
                newFile.MCreateDateFormated = '';
                newFile.MCreatorName = '';
                fileArray.push(newFile);

                var currentRowIdx = $("#tbFileList tr").length;
                AssociateFiles.arrUploadingRowIdx.push(currentRowIdx);
                AssociateFiles.bindFileList(fileArray);
                data.context = $("#tbFileList tr:last .uploaded-on").css("padding-bottom", 6).html("<div class='progress-wrapper' style='width:328px;'><div class='progress'></div></div>");
                //待提交的附件
                var submitJqxhr = data.submit();
                AssociateFiles.submitingFileList.push({
                    fileName: data.files[0].name,
                    jqxhr: submitJqxhr
                });
                submitJqxhr.success(function (data, textStatus, jqXHR) {
                    data = data.Data || data;
                    AssociateFiles.uploadDoneCount++;
                    if (data != undefined && data.isSuccess != true) {
                        $.mDialog.alert(data.Message);
                        if (AssociateFiles.uploadDoneCount == AssociateFiles.selectedFileCount) {
                            for (var i = AssociateFiles.arrUploadingRowIdx.length - 1; i >= 0; i--) {
                                $("#tbFileList tr:eq(" + AssociateFiles.arrUploadingRowIdx[i] + ")").remove();
                            }
                            AssociateFiles.autoAdjustPopupTop();
                            AssociateFiles.bindFileList();
                            AssociateFiles.uploadDoneCount = 0;
                        }
                    }
                    else if (data == undefined) {
                        $.getJSON("/BD/Docs/GetUploadResult?id=" + (new Date()).toString(), function (response) {
                            AssociateFiles.finishUpload(response);
                        });
                    }
                    else {
                        AssociateFiles.finishUpload(data);
                    }
                })
          .error(function (data, textStatus, errorThrown) {
              if (textStatus != "abort") {
                  if (typeof (data) != 'undefined' || typeof (textStatus) != 'undefined' || typeof (errorThrown) != 'undefined') {
                      $.mDialog.alert(textStatus + errorThrown + data.toString());
                  }
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
            done: function (e, data) {//data.result.MItemID
                var currentRow = data.context.closest("tr");
                AssociateFiles.arrUploadingTr.push(currentRow);
            },
            fail: function (event, data) {
                if (data.files[0].error) {
                    $.mDialog.alert(data.files[0].error);
                }
            }
        });
    },
    finishUpload: function (response) {
        if (response != undefined && response.isSuccess != true) {
            AssociateFiles.updateRelatedFileCount();
            $.mDialog.alert(response.Message);
            return;
        }
        var itemId = response.MItemID;
        AssociateFiles.arrUploadedFileId.push(itemId);
        if (AssociateFiles.uploadDoneCount == AssociateFiles.selectedFileCount) {
            //上传完后清空submitingFileList
            AssociateFiles.submitingFileList = [];
            AssociateFiles.createAssociation(AssociateFiles.arrUploadedFileId.join(","), function () {
                $.each(AssociateFiles.arrUploadingTr, function (index, tr) {
                    $(tr).remove();
                });
                AssociateFiles.arrUploadingTr = [];
            });
            AssociateFiles.arrUploadedFileId = [];
            AssociateFiles.uploadDoneCount = 0;
        }
    },
    removeRelation: function (sender, isDelFile) {
        var currentTr = $(sender).closest("tr");
        var fileId = currentTr.attr("data-id");
        var fileName = currentTr.attr("data-name");
        var relationId = currentTr.attr("rid");
        AssociateFiles.removeFileRelation(sender, relationId, fileId, fileName, isDelFile);
    },
    showDeleteOption: function (sender) {
        var offset = $(sender).offset();
        var optionPanels = $("#tbFileList .file-delete-options");
        optionPanels.removeClass("current");
        var divDelOption = $(sender).closest(".file-list-action").find("div[class='file-delete-options']").addClass("current");
        optionPanels.not("[class*=current]").hide();
        if (divDelOption.is(":visible")) {
            divDelOption.hide();
        }
        else {
            optionPanels.find(".popup-arrow").removeAttr("style").css("right", 17);
            divDelOption.css({ top: offset.top + 20 }).show();
        }

        $("#divFiles").scroll(function () {
            if (divDelOption.is(":visible")) {
                divDelOption.hide();
            }
        });
    },
    //删除文件
    removeFileRelation: function (sender, relationId, fileId, fileName, isDelFile) {
        var keyIDs = AssociateFiles.bizObjectID == '' ? '' : relationId;
        //如果文件未上传或点击从销售单删除则直接删除文件，不用请求服务器进行删除
        if ((keyIDs == null || keyIDs == '') || (AssociateFiles.bizObjectID == '' && !isDelFile)) {
            AssociateFiles.deleteItem(sender);
            AssociateFiles.autoAdjustPopupTop();

            ///取消上传中的附件
            $.each(AssociateFiles.submitingFileList, function (index, item) {
                if (item.fileName == fileName) {
                    AssociateFiles.selectedFileCount--;
                    item.jqxhr.abort();
                }
            });

        } else {
            var obj = {};
            obj.KeyIDs = keyIDs;
            obj.MType = "Remove";
            obj.MOperationID = fileId;
            if (isDelFile) {
                obj.MType = "Delete";
                $.mDialog.confirm(LangKey.AreYouSureToDelete,
                {
                    callback: function () {
                        AssociateFiles.execRemoveRelation(sender, obj);
                    }
                });
            }
            else {
                AssociateFiles.execRemoveRelation(sender, obj);
            }
        }
    },
    execRemoveRelation: function (sender, obj) {
        $("#divFiles").mask("");
        mAjax.submit("/BD/Docs/RemoveFileRelation?bizObject=" + AssociateFiles.bizObject, { param: obj }, function (msg) {
            AssociateFiles.deleteItem(sender);
            AssociateFiles.autoAdjustPopupTop();
            $("#divFiles").unmask();
            if (!msg.Success) {
                $.mDialog.alert(msg.Message);
            }
        });
    },
    loadFileList: function (fileIds) {
        //if (!fileIds) {
        //    AssociateFiles.bindFileList(null);
        //    return;
        //}
        var param = { bizObject: AssociateFiles.bizObject, bizObjectID: AssociateFiles.bizObjectID, fileIds: fileIds };

        $.mAjax.Post("/BD/Docs/GetRelatedFileList", param, function (data) {
            AssociateFiles.bindFileList(data);
        });

        //$("body").mFormGet({
        //    url: "/BD/Docs/GetRelatedFileList", param: param, fill: false, callback: function (data) {
        //        AssociateFiles.bindFileList(data);
        //    }
        //});
    },
    updateRelatedFileCount: function () {
        var n = $("#tbFileList tr").length - 1;
        if (n > 0) {
            $("#spAttachCount").text(n);
        } else {
            $("#spAttachCount").text("");
        }
    },
    bindFileList: function (fileList) {
        var fileListHtml = '';
        var isTbExist = $("#tbFileList").length > 0;
        if (!isTbExist) {
            fileListHtml = "<table id='tbFileList' class='file-list' border='0' cellspacing='0' cellpadding='0' border='0' style='table-layout: auto;'>";
            var title = HtmlLang.Write(LangModule.Docs, "UploadFilesTitle", "Upload your files to store them alongside all of your financial documents");
            var state = HtmlLang.Write(LangModule.Docs, "AddFilesState", "Added files will only be visible to users with access to your company");
            fileListHtml += "<tr class='nofile-row'><td colspan='2'><div class='nofile-msg'><div class='file-list-empty-left'></div><div class='file-list-empty-right'><div>" + title + "</div><p>" + state + "</p></div></div></td></tr>";
        }
        if ($("#tbFileList tr").length > 0) {
            $("#tbFileList tr:last").addClass("row-splitter");
        }
        if (fileList != null) {
            var uploadedLabel = HtmlLang.Write(LangModule.Docs, "UploadedAtWhenByWho", "File uploaded at {0} by {1}");
            $.each(fileList, function (index, obj) {
                var rowSplitterCss = '';
                if (index < fileList.length - 1) {
                    rowSplitterCss = "row-splitter";
                }
                fileListHtml += "<tr class='" + rowSplitterCss + " file-item' data-name='" + obj.MName + "' data-id='" + obj.MItemID + "' rid='" + obj.RelationID + "'><td class='file-list-detail'><em unselectable='on'><div>";
                fileListHtml += "<a class='file-name' href='javascript:void(0)' onclick=\"AssociateFiles.viewFileInfo('" + obj.MItemID + "');return false;\" title='" + obj.MName + "'>" + obj.MName + "</a>";
                fileListHtml += "<div class='uploaded-on'>" + uploadedLabel.replace("{0}", (obj.MCreateDate ? $.mDate.formatDateTime(obj.MCreateDate) : "")).replace("{1}", mText.encode(obj.MCreatorName)) + "</div>";

                fileListHtml += "</div></em></td>";
                if (AssociateFiles.isEdit()) {
                    fileListHtml += "<td class='file-list-action'><a href='javascript:void(0)' class='m-file-options' onclick=\"AssociateFiles.showDeleteOption(this);\">&nbsp;</a>";
                    fileListHtml += "<div class='file-delete-options'><b class='popup-arrow popup-arrow-notch'></b><b class='popup-arrow'></b><a href='javascript:void(0)' class='remove-relation file-option' onclick=\"AssociateFiles.removeRelation(this);\">" + AssociateFiles.removeRelationText + "</a>";
                    fileListHtml += "<a href='javascript:void(0)' class='delete file-option' id='aRemoveRelationWithFile' onclick=\"AssociateFiles.removeRelation(this, true);\">" + HtmlLang.Write(LangModule.Docs, "Delete", "Delete") + "</a></div>";
                    fileListHtml += "</td></tr>";
                }
            });
        }
        if ($("#tbFileList tr").length == 1 && fileList && fileList.length > 0) {
            $("#tbFileList tr:first").hide();
        }
        if (!isTbExist) {
            fileListHtml += "</table>";
            $("#divFileList").html(fileListHtml);
        } else {
            $("#divFileList tr:last").after(fileListHtml);
        }
        if ($("#tbFileList tr").length == 1) {//(!fileList || fileList.length == 0) && 
            $("#tbFileList tr:first").show();
        }
        AssociateFiles.updateRelatedFileCount();
        AssociateFiles.autoAdjustPopupTop();
    },
    autoAdjustPopupTop: function () {
        if (AssociateFiles.isPopupTop) {
            var popupHeight = $("#divRelatedAttach").outerHeight();

            var toolbar = $(".m-toolbar-footer:visible");

            toolbar = toolbar.length > 0 ? toolbar : $(".m-toolbar-footer");

            var newTop = toolbar.offset().top - popupHeight;
            $("#divRelatedAttach").css({ top: newTop });
        }
    },
    viewFileInfo: function (fileId, fileIds) {
        if (fileId && fileId != 'undefined') {
            Megi.openDialog('/BD/Docs/FileView', '', 'curFileId=' + fileId + '&fileIds=' + (typeof (fileIds) == 'undefined' ? AssociateFiles.getAssociatedFileIds() : fileIds), 560, 460);
        }
        else {
            $.mAlert(HtmlLang.Write(LangModule.Docs, "WaitingUploadMsg", "The attachment is uploading , please wait a moment!"));
        }
    },
    deleteItem: function (btnObj) {
        var row = $(btnObj).closest("tr");
        row.remove();
        var rowCount = $("#tbFileList tr").length;
        if (rowCount == 1) {
            $("#tbFileList tr:first").show();
        }
        $("#tbFileList tr:last").removeClass("row-splitter");
        AssociateFiles.updateRelatedFileCount();
    },
    getAssociatedFileIds: function () {
        var fileIds = [];
        $("#tbFileList tr").each(function () {
            if ($(this).attr("data-id")) {
                fileIds.push($(this).attr("data-id"));
            }
        });
        return fileIds.toString();
    },
    createAssociation: function (fileIds, callBack) {
        //要添加的附件ID字符串
        var fileId_str = "";
        //获取当前已经添加的附件ID列表
        var fsids = AssociateFiles.getAssociatedFileIds();
        //如果当前已经存在添加的附件，则需要排除重复的附件
        if (fsids) {
            if (fileIds) {
                var fileIdss = fileIds.split(',');
                for (var i = 0; i < fileIdss.length; i++) {
                    //如果当前附件还没有添加过，则可以添加
                    if (fsids.indexOf(fileIdss[i]) == -1) {
                        fileId_str += fileIdss[i] + ",";
                    }
                }
            }
        } else {
            //否则不需要排除，直接添加即可
            fileId_str = fileIds;
        }
        //如果存在附件ID字符串，则添加
        if (fileId_str) {
            if (AssociateFiles.bizObjectID != null && AssociateFiles.bizObjectID != "") {
                AssociateFiles.associateFilesTo(AssociateFiles.bizObject, AssociateFiles.bizObjectID, fileId_str, callBack);
            } else {
                AssociateFiles.loadFileList(fileId_str);
                if (callBack != undefined) {
                    callBack();
                }
            }
        }
    },
    associateFilesTo: function (bizObject, bizObjectID, fileIds, callBack) {
        var obj = {};
        obj.KeyIDs = typeof (fileIds) == 'undefined' ? AssociateFiles.getAssociatedFileIds() : fileIds;
        obj.MOperationID = bizObjectID;
        mAjax.submit("/BD/Docs/CreateFilesAssociation?bizObject=" + bizObject, { param: obj }, function (msg) {
            if (msg == true) {
                if (AssociateFiles.bizObjectID != "") {
                    AssociateFiles.loadFileList(obj.KeyIDs);
                }
                if (callBack != undefined) {
                    callBack();
                }
            } else {
                $.mDialog.alert(msg.Message);
            }
        });
    }
}

$(document).ready(function () {
    AssociateFiles.init();
});