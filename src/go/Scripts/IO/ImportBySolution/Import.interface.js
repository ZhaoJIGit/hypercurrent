if (IOImport == undefined) {
    var IOImport = {};
}
IOImport.open = function (importTypeId) {
    var title = HtmlLang.Write(LangModule.Common, "Import", "Import");
    var url = "/IO/ImportBySolution/Import?type=" + importTypeId;
    Megi.dialog({
        title: title,
        width: 990,
        height: 550,
        href: url
    });
}

IOImport.fpOpen = function (importTypeId, callback) {
    mAjax.post(
    "/FP/FPSetting/GetImportTypeConfigModel",
    { FPType: 0, type: importTypeId },
    function (msg) {
        var isImport = false;
        if (importTypeId === 1) {
            isImport = msg[0].MImportType === 1 || msg[1].MImportType === 1;
        } else {
            isImport = msg[0].MImportType === 1;
        }
        if (isImport) {
            mDialog.show({
                mTitle: "",
                mDrag: "mBoxTitle",
                mShowbg: true,
                mWidth: 800,
                mHeight: 450,
                mContent: "iframe:" + "/IO/ImportBySolution/ImportFaPiao?inOrOuttype=" + importTypeId + "&fpType=0",
                mCloseCallback: [function () {
                }, callback || function () {
                    mDialog.message(HtmlLang.Write(LangModule.FP, "ImportSuccess", "导入成功！"));
                    var title = importTypeId == 0 ? HtmlLang.Write(LangModule.FP, "VATFapiao", "销项发票") : HtmlLang.Write(LangModule.FP, "PurchaseFapiaoHome", "进项发票");
                    var url = "/FP/FPHome/FPReconcileHome?type=" + importTypeId + "&index=2";
                    $.mTab.addOrUpdate(title, url, true);
                }]
            });
        }
        else {
            $.mDialog.alert(HtmlLang.Write(LangModule.FP, "FPTypeIsNotLocal", "发票设置中没有选择本地上传，请修改发票设置"));
        }
    });
}