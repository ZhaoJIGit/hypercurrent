/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var isSetup = $("#hidIsSetup").val();
if (isSetup === "true") {
    document.domain = $("#hidDomain").val();
}
var FileView = {
    init: function () {
        $("div.boxTitle>h3", parent.document).text($("#hidAttachName").val());
        FileView.bindAction();
        $("body").unmask();
    },
    bindAction: function () {
        $("#aDownloadFile").off('click').on('click', function () {
            //mWindow.reload这个有点问题，点了下载之后，下一页、上一页点了没反应
            window.location.href = '/BD/Docs/DownloadFile/' + $(this).attr("data-id");
        });
        $("#aPrev").off('click').on('click', function () {
            FileView.loadFileInfo(false);
        });
        $("#aNext").off('click').on('click', function () {
            FileView.loadFileInfo(true);
        });
    },
    loadFileInfo: function (goNext) {
        var curFileId = $("#hidCurFileId").val();
        var arrFileId = $("#hidFileIds").val().split(',');
        var nextFileId;
        $.each(arrFileId, function (index, fileId) {
            if (fileId == curFileId) {
                if (goNext) {
                    if (index < arrFileId.length - 1) {
                        nextFileId = arrFileId[index + 1];
                    } else {
                        nextFileId = arrFileId[0];
                    }
                } else {
                    if (index > 0) {
                        nextFileId = arrFileId[index - 1];
                    } else {
                        nextFileId = arrFileId[arrFileId.length - 1];
                    }
                }
                return false;
            }
        });

        Megi.postUrl("/BD/Docs/FileView", "curFileId=" + nextFileId + "&fileIds=" + arrFileId.toString() + "&isSetup=" + $("#hidIsSetup").val());
    }
}

$(document).ready(function () {
    FileView.init();
});