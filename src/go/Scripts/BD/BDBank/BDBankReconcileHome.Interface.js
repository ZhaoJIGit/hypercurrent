if (BDBank == undefined) {
    var BDBank = {};
}
BDBank.refreshTransPage = function (bankId, bankName, ready, normalCallback, popCallback) {
    var url = "/BD/BDBank/BDBankReconcileHome?acctid=" + bankId + "&index=3&type=1";
    var title = bankName;
    if (window.parent.$(".mCloseBox").length == 0) {
            $.mTab.refresh(title, url, false, true);
        //if (ready) {
        //    //如果是refresh的话，就不能改变其原有的title
        //} else {
        //    $.mTab.addOrUpdate(title, url);
        //}
        if (normalCallback) {
            normalCallback();
        }
    } else {
        parent.$.mTab.refresh(title, url, false, true);
        if (popCallback) {
            popCallback();
        }
    }
}