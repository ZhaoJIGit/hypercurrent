var PTTabType = { Biz: "Biz", Voucher: "Voucher", SalaryList: "SalaryList" };
var PTBase = {
    tabId: "#tabInit",
    isSmartVersion: $("#hidIsSmartVersion").val() === "True",
    initSort: function (parentId, saveUrl) {
        Sortable.create(eval(parentId), {
            draggable: ".print-tmpl",
            group: "sorting",
            sort: true,
            onSort: function (evt) {
                var arrResult = [];
                $(".print-tmpl").each(function () {
                    arrResult.push(this.id);
                });

                mAjax.post(
                    saveUrl,
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
    reload: function (tabType) {
        var tmpType = tabType;
        if (tabType == PTTabType.SalaryList) {
            tmpType = "Salary";
        }
        var url = "/PT/PTBase/PT{0}ListPartial?reload=1".replace(/\{0\}/g, tmpType);
        //$("#div" + tabType).load(url);

        var curTab = $(PTBase.tabId).tabs('getSelected');
        var curIdx = $(PTBase.tabId).tabs('getTabIndex', curTab);
        var curPanel = $("#div" + tabType).closest(".panel");
        var tgtIdx = $("#tabInit .panel").index(curPanel);

        if (curIdx == tgtIdx || tgtIdx == -1) {
            top.window.accessRequest(function () {
                curTab.panel('refresh', url);
            });
        }
        else {
            PTIndex.CurrentType = tgtIdx;
            $(PTBase.tabId).tabs('select', tgtIdx);
        }
    },
    getEditId: function (sender) {
        var id = $(sender).closest(".print-tmpl").attr("id");
        if (!id) {
            var idx = parseInt($(sender).parent().attr("id").replace("divFolderOptions", ""));
            id = $("#divOptions" + idx).closest(".print-tmpl").attr("id");
        }
        return id;
    },
    openDialog: function (sender, title, url, isNew, w, h) {
        if (!isNew) {
            url = url + '/' + PTBase.getEditId(sender)
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
    }
}