
var Index = {
    intervalId: null,
    oriOnbeforeunload: null,
    init: function () {
        $("#divExportAll", top.window.document).unmask();
        Index.startDownload();
        Index.monitorProgress();
    },
    monitorProgress: function () {
        Index.intervalId = setInterval(function () {
            $.post("/BD/Export/Progress", function (response) {
                Index.updateProgress(response.Data);
            });
        }, 500);
    },
    updateProgress: function (response) {
        var divExportAll = $("#divExportAll", top.window.document);
        divExportAll.unmask();
        var divMsg = $("#divProgress").find(".message");
        divMsg.html(response.Message);
        var progress = $("#divProgress").find(".progress");
        if (response.Progress > 0) {
            progress.width(response.Progress + "%");
        }
        if (response.Progress == 90) {
            if (Megi.isIE() && top.window.onbeforeunload != null) {
                Index.oriOnbeforeunload = top.window.onbeforeunload;
                $(top.window).unbind('beforeunload');
                top.window.onbeforeunload = null;
            }
            setTimeout(function () {
                clearInterval(Index.intervalId);
                divExportAll.hide();
                
                if (Megi.isIE()) {
                    setTimeout(function () {
                        if (!top.window.onbeforeunload) {
                            top.window.onbeforeunload = Index.oriOnbeforeunload;
                        }
                    }, 1000);
                }
            }, 500);
        }
        else if (response.Progress == -1) {
            top.$.mDialog.alert(response.Message);
            var divWrapper = $("#divExportAll", top.window.document);
            divWrapper.height(53);
            clearInterval(Index.intervalId);
            setTimeout(function () {
                divWrapper.remove();
            }, 5000);
        }
    },
    startDownload: function () {
        $.fileDownload("/BD/Export/ExportAll", {
            successCallback: function (url) {
                clearInterval(Index.intervalId);
                $("#divExportAll", top.window.document).remove();
            },
            failCallback: function (html, url) {
                clearInterval(Index.intervalId);
                if (html) {
                    $.mDialog.alert(msg);
                }
            }
        });
    }
}

$(document).ready(function () {
    Index.init();
});