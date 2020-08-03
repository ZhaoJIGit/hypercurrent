var OrgAdd = {
    init: function () {
        $("#aStartTrial").click(function () {
            var toUrl = $(this).attr("href");
            $("body").mFormSubmit({
                url: "/Organisation/OrgRegister", param: { model: {} }, callback: function (msg) {
                    if (msg.Success) {
                        window.location = toUrl + "/" + msg.ObjectID;
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });
    }
}
$(document).ready(function () {
    OrgAdd.init();
});