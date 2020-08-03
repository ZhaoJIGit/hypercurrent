var PreviewBase = {
    postUrl: function (url, param) {
        var form = $("#frmSubmit");
        form.attr('action', url);

        if (param) {
            var arrParam = param.split('&');
            for (var i = 0; i < arrParam.length; i++) {
                var arrKeyVal = arrParam[i].split('=');
                var input = form.find("input:hidden[name=" + arrKeyVal[0] + "]");
                if (input.length == 0) {
                    input = $("<input type='hidden' name='" + arrKeyVal[0] + "'/>");
                }
                input.attr('value', arrKeyVal[1]);
                form.append(input);
            }
        }
        form.submit();
    },
    regHidMaskEvt: function () {
        $(".mCloseBox", parent.document).click(function () {
            var bg = $("#XYTipsWindowBg", parent.document);
            if (bg.length > 0) {
                bg.remove();
            }
        });
    }
}