var IVSetting = {
    isSmartVersion: $("#hidIsSmartVersion").val() === "True",
    init: function () {
        IVSetting.initSort();
        IVSetting.initUI();
        IVSetting.initAction();
        IVSetting.loadLogo();
    },
    initUI: function () {
        var totalW = $(".m-imain-content").width();
        var centerMaxW = totalW - $(".settings").width() - $(".logo-wrapper").width() - 45;
        $(".center-wrapper").css("max-width", centerMaxW);
        if (IVSetting.isSmartVersion) {
            $(".print-tmpl ul.show-list").width(300);
        }
    },
    loadLogo: function () {
        var psList = $(".print-tmpl");
        $.each(psList, function (idx, item) {
            var psLogo = $(item).find(".ps-logo");
            if (psLogo.length == 1) {
                $.mAjax.Post("/BD/PrintSetting/ShowImage", { id: psLogo.attr("id") }, function (img) {
                    if (img) {
                        psLogo.html(img);
                    }
                })
            }
        });
    },
    initSort: function () {
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
                    "/BD/PrintSetting/SortSetting",
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
    getDialogOption: function () {
        var opt = {};
        opt.w = undefined;
        opt.h = undefined;
        if (IVSetting.isSmartVersion) {
            opt.w = 785;
            opt.h = 310;
        }
        return opt;
    },
    initAction: function () {
        $("#aNewTheme, #divNewTheme").off("click").on("click", function () {
            var opt = IVSetting.getDialogOption();
            IVSetting.openDialog(this, HtmlLang.Write(LangModule.Common, "NewPrintSetting", "New Print Setting"), "/BD/PrintSetting/InvoiceSettingEdit", opt.w, opt.h, true);
        });
        $(".edit").off("click").on("click", function () {
            var opt = IVSetting.getDialogOption();
            IVSetting.openDialog(this, HtmlLang.Write(LangModule.Common, "EditPrintSetting", "Edit Print Setting"), "/BD/PrintSetting/InvoiceSettingEdit", opt.w, opt.h);
        });
        $(".copy").off("click").on("click", function () {
            IVSetting.openDialog(this, HtmlLang.Write(LangModule.Common, "CopyPrintSetting", "Copy Print Setting"), "/BD/PrintSetting/InvoiceSettingCopy", 400, 345);
        });
        $("div[id^=divFolderOptions] .upload-logo, .upload-logo a").off("click").on("click", function () {
            IVSetting.uploadOrChangeLogo(this, HtmlLang.Write(LangModule.Common, "UploadLogo", "Upload Logo"));
        });
        $(".upload-logo img, .change-logo").off("click").live("click", function () {
            IVSetting.uploadOrChangeLogo(this, HtmlLang.Write(LangModule.Common, "ChangeLogo", "Change Logo"));
        });
        $(".remove-logo").off("click").on("click", function () {
            IVSetting.removeLogo(this);
        });
        $(".delete").off("click").on("click", function () {
            IVSetting.deleteSetting(this);
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
        IVSetting.openDialog(sender, title, "/BD/PrintSetting/InvoiceLogoUpload", 410, 270);
    },
    openDialog: function (sender, title, url, w, h, isNew) {
        if (!isNew) {
            url = url + '/' + IVSetting.getEditId(sender)
        }
        var param = {
            title: title,
            href: url
        };
        if (w && h) {
            param.width = w;
            param.height = h;
        }

        Megi.dialog(param);
    },
    removeLogo: function (sender) {
        mAjax.submit(
            "/BD/PrintSetting/RemoveLogo",
            { id: IVSetting.getEditId(sender) },
            function (msg) {
                if (msg.Success) {
                    IVSetting.reload();
                }
                else {
                    $.mDialog.alert(msg.Message);
                }
            });
    },
    reload: function () {
        mWindow.reload("/BD/PrintSetting/InvoiceSetting");
    },
    deleteSetting: function (sender) {
        $.mDialog.confirm(LangKey.AreYouSureToDelete,
        {
            callback: function () {
                var id = IVSetting.getEditId(sender);
                mAjax.submit(
                    "/BD/PrintSetting/DeleteSetting",
                    { id: id },
                    function (msg) {
                        if (msg.Success) {
                            $.mMsg(HtmlLang.Write(LangModule.IV, "DeleteSuccessfully", "Delete Successfully!"));
                            IVSetting.reload();
                        } else {
                            $.mDialog.alert(msg.Message);
                        }
                    });
            }
        });
    }
}
$(document).ready(function () {
    IVSetting.init();
});