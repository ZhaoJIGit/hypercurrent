
var TaxFillingBase = {
    post: function (url,postParam,callback) {
        $("#aPostReport").click(function () {
            mAjax.submit(url, postParam,
                function (msg) {
                    if (msg.Success) {
                        TaxFillingBase.checkTask(msg.ObjectID,callback);
                    } else {
                        $.mDialog.alert(msg.Message, function () {
                            mWindow.reload();
                        });
                    }
                });
        });
    },
    checkTask: function (etaxtaskId,callback) {
        if (typeof (EventSource) !== "undefined") {
            var es = new EventSource("/Report/ETax/CheckTask/" + etaxtaskId);
            es.onopen = function (e) {
            }
            es.onmessage = function (e) {
                var obj = eval('(' + e.data + ')');
                if (obj.MStatusCode == "COMPLETED"){
                    es.close();
                    callback(obj);
                }
                if (obj.MIsExpired) {
                    es.close();
                }
            };
            es.onerror = function (event) {
                es.close();
            }
        }
    }
}
