var MegiChange = {
    isValueChange: function (index) {
        var result = false;
        var easyUIControls = $("body", iframe.contentDocument).mGetEasyUIControls();
        for (var i = 0; i < easyUIControls.length ; i++) {
            var control = easyUIControls[i];
            for (var j = 0; j < control.controls.length ; j++) {
                var $item = control.type.createInstance(control.controls[j]);
                if ($item.isValueChange != undefined) {
                    result = $item.isValueChange();
                    alert(result);
                    if (result == true) {
                        break;
                    }
                }
            }
            if (result == true) {
                break;
            }
        }
        return result;
    }
}