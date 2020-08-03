if (mBrowserVerify == undefined) {
    var mBrowserVerify = {};
}

mBrowserVerify.run = function () {
    var BrowserErrorCode = { VersionTooLow: "401", VersionNotSupport: "402" }
    var bErrorCode = "";
    var brow = mBrowser.info;
    var bInfo = "";
    var version = 0;
    if (brow && brow.version != undefined && brow.version != null && String(brow.version).length > 0) {
        version = parseInt(String(brow.version).split('.')[0]);
    }
    if (brow.msie) {
        if (parseInt(brow.version) < 9) {
            bErrorCode = BrowserErrorCode.VersionTooLow;
        }
    }
    if (brow.chrome) {
        //if (version > 56) {
        //    bErrorCode = BrowserErrorCode.VersionNotSupport;
        //}
    }
    if (bErrorCode != "") {
        window.location = "http://info.megichina.com/browser/" + bErrorCode;
    }
}
mBrowserVerify.run();

