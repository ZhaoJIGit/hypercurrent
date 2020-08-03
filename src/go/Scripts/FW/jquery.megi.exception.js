(function () {

    var mException = (function () {

        var mException = function () {

            var logUrl = "/FW/FWHome/LogException";
            var that = this;

            this.init = function () {
                window.onerror = that.handleErrorMessage;
            }

            this.handleErrorMessage = function (msg, url, line, col, error) {

                //没有URL不上报！上报也不知道错误
                if (msg != "Script error." && !url) {
                    return;
                }

                //目前只记录GO站点的信息
                if (!(window.location.host.indexOf("go.megi") >= 0)) {
                    return;
                }

                //采用异步的方式
                //我遇到过在window.onunload进行ajax的堵塞上报
                //由于客户端强制关闭webview导致这次堵塞上报有Network Error
                //我猜测这里window.onerror的执行流在关闭前是必然执行的
                //而离开文章之后的上报对于业务来说是可丢失的
                //所以我把这里的执行流放到异步事件去执行
                //脚本的异常数降低了10倍
                setTimeout(function () {
                    var data = {};
                    //不一定所有浏览器都支持col参数
                    col = col || (window.event && window.event.errorCharacter) || 0;

                    data.url = url;
                    data.line = line;
                    data.col = col;
                    if (!!error && !!error.stack) {
                        //如果浏览器有堆栈信息
                        //直接使用
                        data.msg = error.stack.toString();
                    } else if (!!arguments.callee) {
                        //尝试通过callee拿堆栈信息
                        var ext = [];
                        var f = arguments.callee.caller, c = 3;
                        //这里只拿三层堆栈信息
                        while (f && (--c > 0)) {
                            ext.push(f.toString());
                            if (f === f.caller) {
                                break;//如果有环
                            }
                            f = f.caller;
                        }
                        data.msg = ext.join(",");
                    }

                    //把data上报到后台！
                    that.sendErrorMessage(data);
                    //控制台保持输出，但是不适用浏览器的报错的形式
                    console && console.log("MEGI error:" + JSON.stringify(data));
                }, 0);

                return;
            }

            this.sendErrorMessage = function (message) {

                try {
                    mAjax && mAjax.post(logUrl, { message: JSON.stringify(message) }, function () {
                    });
                } catch (e) {
                }
            }
        }

        return mException;
    })()

    window.mException = mException;

})()

window.onload = function () {

    new mException().init();
};