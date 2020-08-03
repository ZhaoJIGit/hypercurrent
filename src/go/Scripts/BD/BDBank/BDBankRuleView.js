var BDBankRuleView = {
    init: function () {
        BDBankRuleView.initForm();
    },
    initForm: function () {
        $("body").mFormGet({
            url: "/BD/BDBank/GetBDBankRuleEditModel"
        });
    }
}
$(document).ready(function () {
    BDBankRuleView.init();
});