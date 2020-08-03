var PSList = {
    init: function () {
        PSList.initSort();
        PSList.initUI();
        PSList.initAction();
		PSList.loadLogo();
    },
    initUI: function () {
        var totalW = $("#divAccordion").width() - 34 - 15;
        var logoW = 210 + 35;
        $(".ps-list .settings").css({ "width": totalW - logoW });
    },
    loadLogo: function () {
        var psList = $(".print-tmpl");
        $.each(psList, function (idx, item) {
            var psLogo = $(item).find(".ps-logo");
            if (psLogo.length == 1) {
                mAjax.post("/PA/PayrollBasic/ShowImage", { id: psLogo.attr("id") }, function (img) {
                    if (img) {
                        psLogo.html(img);
                        PSList.bindLogoClick();
                    }
                })
            }
        });
    },
    initSort: function(){
        Sortable.create(divSettings, {
            draggable: ".print-tmpl",
            group: "sorting",
            sort: true,
            onSort: function (evt) {
                var arrResult = [];
                $(".print-tmpl").each(function () {
                    arrResult.push(this.id);
                });

                mAjax.post(
                    "/PA/PayrollBasic/SortPrintSetting",
                    { ids: arrResult.toString() },
                    function (msg) {
                        if (msg.Success) {
                            //$.mMsg(HtmlLang.Write(LangModule.Common, "SortSuccess", "Sort successfully!"));
                        }
                        else {
                            $.mDialog.alert(msg.Message);
                        }
                    });
            }
        });
    },
    bindLogoClick: function () {
        $(".upload-logo img, .change-logo").off("click").on("click", function () {
            PSList.uploadOrChangeLogo(this, HtmlLang.Write(LangModule.Common, "ChangeLogo", "Change Logo"));
        });
    },
    initAction: function () {
        $("#aNewTheme, #divNewTheme").off("click").on("click", function () {
            PSList.openDialog(this, HtmlLang.Write(LangModule.Common, "NewPrintSetting", "New Print Setting"), "/PA/PayrollBasic/PrintSettingEdit", 950, 510, true);
        });
        $(".edit").off("click").on("click", function () {
            PSList.openDialog(this, HtmlLang.Write(LangModule.Common, "EditPrintSetting", "Edit Print Setting"), "/PA/PayrollBasic/PrintSettingEdit", 950, 510);
        });
        $(".copy").off("click").on("click", function () {
            PSList.openDialog(this, HtmlLang.Write(LangModule.Common, "CopyPrintSetting", "Copy Print Setting"), "/PA/PayrollBasic/PrintSettingCopy", 400, 345);
        });
        $("div[id^=divFolderOptions] .upload-logo, .upload-logo a").off("click").on("click", function () {
            PSList.uploadOrChangeLogo(this, HtmlLang.Write(LangModule.Common, "UploadLogo", "Upload Logo"));
        });
        $(".remove-logo").off("click").on("click", function () {
            PSList.removeLogo(this);
        });
        $(".delete").off("click").on("click", function () {
            PSList.deleteSetting(this);
        });
    },
    getEditId: function (sender) {
        var id = $(sender).closest(".print-tmpl").attr("id");
        if (!id) {
            var idx = parseInt($(sender).parent().attr("id").replace("divFolderOptions", ""));
            id = $("#divOptions" + idx).closest(".print-tmpl").attr("id");
        }
        return id;
    },
    uploadOrChangeLogo: function (sender, title) {
        PSList.openDialog(sender, title, "/PA/PayrollBasic/PrintSettingLogoUpload", 410, 270);
    },
    openDialog: function (sender, title, url, w, h, isNew) {
        if (!isNew) {
            url = url + '/' + PSList.getEditId(sender)
        }
        Megi.dialog({
            title: title,
            width: w,
            height: h,
            href: url
        });
    },
    removeLogo: function (sender) {
        mAjax.submit(
            "/PA/PayrollBasic/RemovePrintSettingLogo",
            { id: PSList.getEditId(sender) },
            function (msg) {
                if (msg.Success) {
                    PSList.reload();
                }
                else {
                    $.mDialog.alert(msg.Message);
                }
            });
    },
    reload: function () {
        //location.href = "/PA/PayrollBasic/Index";
        var curPanel = $('#divAccordion').accordion('getSelected');    // get the selected panel
        if (curPanel) {
            curPanel.panel('refresh', '/PA/PayrollBasic/PrintSettingListPartial?reload=1');    // call 'refresh' method to load new content
        }
    },
    deleteSetting: function (sender) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete,
        {
            callback: function () {
                var id = PSList.getEditId(sender);
                mAjax.submit(
                    "/PA/PayrollBasic/DeletePrintSetting",
                    { id: id },
                    function (msg) {
                        if (msg.Success) {
                            $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                            PSList.reload();
                            //$(".print-tmpl[id='" + id + "']").remove();
                        } else {
                            $.mDialog.alert(msg.Message);
                        }
                    });
            }
        });
    }
}
$(document).ready(function () {
    PSList.init();
});