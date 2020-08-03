document.ready = function (callback) {
    ///兼容FF,Google
    if (document.addEventListener) {
        document.addEventListener('DOMContentLoaded', function () {
            document.removeEventListener('DOMContentLoaded', arguments.callee, false);
            callback();
        }, false)
    }
        //兼容IE
    else if (document.attachEvent) {
        document.attachEvent('onreadystatechange', function () {
            if (document.readyState == "complete") {
                document.detachEvent("onreadystatechange", arguments.callee);
                callback();
            }
        })
    }
    else if (document.lastChild == document.body) {
        callback();
    }
}

document.ready(function () {
    var idName = "sessisonID";

    //创建一个SessionID
    function generateSessionID() {

        var guid = new GUID();
        var id = guid.newGUID();

        sessionStorage.setItem(idName, id);
    }

    //先从sessionStorage里面获取
    var sessinoId = sessionStorage.getItem(idName);

    //如果没有创建，则创建一个
    if (!sessinoId) generateSessionID();

});