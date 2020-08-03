var FPSetting = {
    type:1,
    init: function() {
        FPSetting.bindAction();
        FPSetting.initTab();
    },
    initTab:function() {
        $('#tt').tabs({
            onSelect: function (title,index) {
                if (index==1) {
                    FPSetting.type = 2;
                } else {
                    FPSetting.type = 1;
                }
                FPSetting.bindData();
            }
        });
    },
    bindData:function() {
        
    },
    bindAction:function() {
        
    }
}

$(document).ready(function () {
    FPSetting.init();
});
