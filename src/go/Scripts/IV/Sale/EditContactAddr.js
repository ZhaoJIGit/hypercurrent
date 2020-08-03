var EditContactAddr = {
    init: function () {
        EditContactAddr.saveClick();
        EditContactAddr.cancelClick();
        mAjax.post("/IV/Sale/GetContactAddr", { model: { MItemID: $("#ContactID").val() } }, function (data) {
                $("body").mFormSetForm(data);
                //对国家赋值
                $("#selMPCountryID").combobox("setValue", data.MPCountryID);
        });
    },
    saveClick: function () {
        $("#aSave").click(function () {
            var contactID = $("#ContactID").val();
            var viewFrom = $("#viewFrom").val();
            $("#divStateEditAddr").mFormSubmit({
                url: "/IV/Sale/UpdateStateContactAddr", param: { model: {} }, callback: function (msg) {
                    if (msg == true) {
                        Megi.closeDialog();
                        if (viewFrom == "Statements") {
                            parent.Statements.reloadData();
                        }
                        else if (viewFrom == "ViewStatement") {
                            parent.ViewStatement.reload(contactID);
                        }
                    } else {
                        $.mDialog.alert(msg.Message);
                    }
                }
            });
            return false;
        });
    },
    cancelClick: function () {
        $("#aCancel").click(function () {
            var contactID = $("#ContactID").val();
            var viewFrom = $("#viewFrom").val();
            Megi.closeDialog();
            if (viewFrom == "Statements") {
                parent.Statements.reloadData();
            }
            else if (viewFrom == "ViewStatement") {
                parent.ViewStatement.reload(contactID);
            }
            return false;
        });
    }
}

$(document).ready(function () {
    EditContactAddr.init();
});