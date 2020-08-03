
var Print = {
    //打印预览，isCheck不传默认需要校验打印数据
    previewPrint: function (title, paramStr, isCheck) {
        var jsonParam = JSON.parse(paramStr);
        jsonParam.width = $(".m-imain").width() - 17;

        //是否需要校验打印数据
        if (isCheck == undefined || isCheck) {
            top.$.mAjax.Post($("#aGoServer", top.document).val() + "/BD/Print/PreviewCheck", jsonParam, function (msg) {
                if (msg.Success == false) {
                    $.mDialog.alert(msg.Message);
                    return;
                }

                Print.openPrintDialog(title, jsonParam);
                $("body", parent.document).unmask();
            });
        }
        else {
            Print.openPrintDialog(title, jsonParam);
        }
    },
    //打开打印弹出框
    openPrintDialog: function (title, jsonParam) {
        var topWrapper = $(".m-wrapper", top.document);
        var popupW = topWrapper.width() - 63;
        var popupH = topWrapper.height() - 75 - 50 - 6;
        top.$.mDialog.show({
            mTitle: title,
            mWidth: popupW,
            mHeight: popupH,
            mOffsets: { right: "0px", bottom: "4px" },//'right-bottom',
            mMax: false,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "",//"mBoxTitle",
            mContent: "iframe:" + $("#aGoServer", top.document).val() + "/BD/Print/Preview",
            mPostData: jsonParam,
            mCloseCallback: function () {
                var dialogBg = $("#XYTipsWindowBg");
                if (Megi.isEdge() && dialogBg.length > 0) {
                    dialogBg.animate({ opacity: "0" }, 500, function () { $(this).remove(); });
                }
            }
        });
    },
    //预加载打印的数据
    preloadPrintData: function (paramStr) {
        mAjax.post("/BD/PrintSetting/CreateReportModel?r=" + new Date().getTime(),JSON.parse(paramStr));
    },
    //选择套打设置
    selectPrintSetting: function (paramStr) {
        $.mDialog.show({
            mTitle: HtmlLang.Write(LangModule.IV, "SelectPrintSetting", "Select Print Setting"),
            mWidth: 410,
            mHeight: 270,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle",
            mContent: "iframe:" + '/IV/Invoice/SelectPrintSetting',
            mPostData: JSON.parse(paramStr)
        });
    },
    //发邮件
    openSendEmailDialog: function (title, param) {
        var postData = {};
        var arrParam = param.split('&');
        for (var i = 0; i < arrParam.length; i++) {
            var arrKeyVal = arrParam[i].split('=');
            postData[arrKeyVal[0]] = arrKeyVal[1];
        }
        $.mDialog.show({
            mTitle: title,
            mShowbg: true,
            mShowTitle: true,
            mDrag: "mBoxTitle",
            mContent: "iframe:" + '/IV/Invoice/SendInvoice',
            mPostData: postData
        });
    }
};