$(document).ready(function () {
    //设置tabIndex
    var easyUIControls = $("body").mGetEasyUIControls();
    for (var i = 0; i < easyUIControls.length ; i++) {
        var control = easyUIControls[i];
        for (var j = 0; j < control.controls.length ; j++) {
            var $item = control.type.createInstance(control.controls[j]);
            if ($item.setTabIndex != undefined) {
                $item.setTabIndex();
            }
        }
    }

    ////获取所有的可见的input
    var $inputs = $("input:visible");
    var beginIndex = 9900;
    $inputs.each(function () {
        if ($(this).attr("tabIndex") == undefined) {
            $(this).attr("tabIndex", beginIndex);
            beginIndex += 1;
        }
    });
    $inputs.sort(function (a, b) {
        var aIndex = Number($(a).attr("tabIndex"));
        var bIndex = Number($(b).attr("tabIndex"));
        return aIndex > bIndex;
    });

    //每个都绑定keyup事件
    $inputs.each(function (index) {
        //$(this).attr("tabIndex", index);
        //即使出现异常，也不要处理，不影响应用
        try {
            var tabIndex = $(this).attr("tabIndex");
            if (tabIndex == undefined) {
                $(this).attr("tabIndex", index);
            }
            //事件绑定
            $(this).on("keydown", function (event) {
                //如果是Tab键
                if (event.which == 9) {
                    //下一个input获取焦点
                    $inputs.eq($(this).attr("tabIndex") + 1).focus();
                }
                //不影响其他功能
                return true;
            });
            //不影响其他功能
            return true;
        } catch (e) {
            //不影响其他功能
            return true;
        }
    });
});