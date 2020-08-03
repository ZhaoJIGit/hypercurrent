

/*

*/
var HistoryView = {
    init: function () {
    },
    openDialog: function (invoiceId, billType) {
        Megi.dialog({
            title: HtmlLang.Write(LangModule.Common, "History", "History"),
            top: window.pageYOffset || document.documentElement.scrollTop,
            width: 800,
            height: 455,
            href: "/Log/Log/BusLogList?invoiceId=" + invoiceId + "&billType=" + billType
        });

    }

}

