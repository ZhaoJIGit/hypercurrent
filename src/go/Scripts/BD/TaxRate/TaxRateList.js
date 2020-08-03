/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="../../../intellisense/jquery.megi.vsdoc.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;    
var TaxRateSetting = {
    IsListActionClick: false,
    IsClick: true,
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    isActive: true,   //是否可用
    tabswitch: new BrowserTabSwitch(),
    init: function () {
        TaxRateSetting.tabswitch.initSessionStorage();
        TaxRateSetting.bindAction();
        TaxRateSetting.bindGrid();
    },
    //加载未被禁用的列表  全部
    loadActiveData: function (callBack) {
        //获取工具顶边距和高度
        var toolbar = $(".m-tab-toolbar:visible");
        var toolbarTop = toolbar.length > 0 ? toolbar.offset().top : 0;
        var toolbarHeight = toolbar.length > 0 ? toolbar.height() : 0;

        Megi.grid('#tbTax', {
            resizable: true,
            auto: true,
            url: "/BD/TaxRate/GetTaxRateListByPage",
            sortName: 'MName',
            sortOrder: 'desc',
            scrollY: true,
            height: $("body").height() - toolbarTop - toolbarHeight - 20,
            queryParams: { SearchFilter: "0", rows: 100 },
            columns: [[{
                title: '<input type=\"checkbox\" >', field: 'MItemID', formatter: function (value, rec, rowIndex) {
                    return value == true ? "<span class=\"mg-icon-lock\">&nbsp;</span>" : "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                }, width: 40, align: 'center',
            },
                { title: LangKey.Name, field: 'MName', width: 200, sortable: true },
                { title: "MIsSysData", field: "MIsSysData", hidden: true },
                { title: HtmlLang.Write(LangModule.Bank, "TaxRate", "Tax Rate"), field: 'MTaxRate', width: 150, align: 'right', sortable: true, formatter: function (value, rec, rowIndex) { return value + "%"; } },
                {
                    title: HtmlLang.Write(LangModule.Bank, "Operation", "Operation"), field: 'Action', align: 'center', width: 100, sortable: false, formatter: function (value, rec, rowIndex) {

                        var actionDiv = "<div class='list-item-action'>";
                        var isSetup = $("#hidIsSetup").val() === 'true';

                        actionDiv += "<a href=\"javascript:void(0);\" onclick=\"TaxRateSetting.IsListActionClick = true;TaxRateSetting.editTaxRate('" + rec.MItemID + "');" + "\" class='list-item-edit'></a>";

                        //初始化 或者 有权限下 都可以删除
                        if (isSetup || TaxRateSetting.hasChangeAuth) {
                            actionDiv += "<a href=\"javascript:void(0);\" onclick=\"TaxRateSetting.IsListActionClick = true;TaxRateSetting.deleteItem('" + rec.MItemID + "');\" class='list-item-del'></a></div>";
                        }

                        return actionDiv;
                    }
                }
            ]],
            onLoadSuccess: function (data) {
                if (callBack != undefined) {
                    callBack(data);
                }
            },
        });
    },
    //加载已被禁用的列表 
    loadNoActiveData: function(){
        Megi.grid('#tbNoActiveTax', {
            resizable: true,
            auto: true,
            url: "/BD/TaxRate/GetTaxRateListByPage?MIsActive=" + TaxRateSetting.isActive,
            sortName: 'MName',
            sortOrder: 'desc',
            queryParams: { SearchFilter: "0", rows: 100 },
            columns: [[{
                title: '<input type=\"checkbox\" >', field: 'MItemID', formatter: function (value, rec, rowIndex) {
                    return value == true ? "<span class=\"mg-icon-lock\">&nbsp;</span>" : "<input type=\"checkbox\" class=\"row-key-checkbox\"  value=\"" + rec.MItemID + "\" >";
                }, width: 40, align: 'center',
            },
                { title: LangKey.Name, field: 'MName', width: 200, sortable: true },
                { title: "MIsSysData", field: "MIsSysData", hidden: true },
                { title: HtmlLang.Write(LangModule.Bank, "TaxRate", "Tax Rate"), field: 'MTaxRate', width: 150, align: 'right', sortable: true, formatter: function (value, rec, rowIndex) { return value + "%"; } },
            ]],
        });
    },
    bindGrid: function (callBack) {
        if (TaxRateSetting.isActive) {
            TaxRateSetting.loadActiveData(callBack);
        }
        else
        {
            TaxRateSetting.loadNoActiveData();
        }
    },
    bindAction: function () {
        $("#aNewTaxRate").click(function () {
            TaxRateSetting.editTaxRate("");
        });
        $("#aDeleteTaxRate").click(function () {
            //多选删除
            TaxRateSetting.deleteClick();
        });
        $('#tt').tabs({
            onSelect: function (title, index) {
                if (index == 0) {
                    TaxRateSetting.isActive = true;
                }
                else {
                    TaxRateSetting.isActive = false;
                }
                TaxRateSetting.bindGrid();
            }
        });

        //禁用按钮
        $("#aArchiveTaxRate").click(function () {
            TaxRateSetting.archiveItem(false);
        });

        //恢复按钮
        $("#btnRestore").click(function () {
            TaxRateSetting.archiveItem(true);
        });
    },
    editTaxRate: function (id) {
        var url = '/BD/TaxRate/TaxRateEdit/' + id;
        if ($("#hidIsSetup").val() === 'true') {
            url += '?isSetup=true';
        }
        Megi.dialog({
            title: id == "" ? HtmlLang.Write(LangModule.Bank, "NewTaxRate", "New Tax Rate") : HtmlLang.Write(LangModule.Bank, "EditTaxRate", "Edit Tax Rate"),
            width: 520,
            height: 330,
            href: url
        });
    },
    afterEdit: function (msg) {
        Megi.mMsg(msg);
        TaxRateSetting.bindGrid();
    },
    //禁用/恢复  项目
    archiveItem: function (isRestore, isSeeAll) {
        var selector = isRestore ? '#tbNoActiveTax' : '#tbTax';
        if (isRestore) {
            var tips = HtmlLang.Write(LangModule.BD, "ConfirmRestoreItem", "您确定要启用这些项目吗？");
            Megi.grid(selector, "optSelected", {
                callback: function (ids) {
                    $.mDialog.confirm(tips,
                    {
                        callback: function () {
                            mAjax.submit(
                                "/BD/TaxRate/ArchiveTaxRate",
                                { KeyIDs: ids, isRestore: isRestore },
                                function (response) {
                                    if (response.Success == true) {
                                        var tips = HtmlLang.Write(LangModule.BD, "RestoreTaxRateSuccess", "Restore tax rate successfully");
                                        $.mDialog.message(tips);
                                    } else {
                                        var tips = HtmlLang.Write(LangModule.BD, "RestoreTaxRateFail", "Restore tax rate fail");
                                        $.mDialog.alert(tips);
                                    }
                                    TaxRateSetting.bindGrid();
                                });
                        }
                    });
                }
            });
        } else {
            //先去检查是否可禁用
            Megi.grid(selector, "dbSelected", {
                url: "/BD/TaxRate/IsCanDeleteOrInactive", callback: function (data) {
                    var alertMsg = BDQuote.GetQuoteMsg(data, isSeeAll);
                    if (data.Success) {
                        //可禁用弹出提示框是否继续禁用
                        $.mDialog.confirm(alertMsg, {
                            callback: function () {
                                //执行禁用操作
                                Megi.grid(selector, "dbSelected", {
                                    url: "/BD/TaxRate/ArchiveTaxRate",
                                    callback: function (retdata) {
                                        if (retdata.Success == true) {
                                            $.mDialog.message(HtmlLang.Write(LangModule.BD, "ArchiveTaxRateSuccess", "Archive tax rate successfully"));
                                        }
                                        else {
                                            $.mDialog.alert(HtmlLang.Write(LangModule.BD, "ArchiveTaxRateFail", "Archive tax rate fail"));
                                        }
                                        TaxRateSetting.bindGrid();
                                    }
                                });
                            }
                        });
                        if (isSeeAll) {
                            $("#popup_message").css("max-height", "300px");
                        }
                        else {
                            $("#popup_message").css("max-height", "200px");
                        }
                    } else {
                        //不可禁用弹出提示。
                        $.mDialog.alert(alertMsg);
                        if (isSeeAll) {
                            $("#popup_message").css("max-height", "300px");
                        }
                        else {
                            $("#popup_message").css("max-height", "200px");
                        }
                    }
                }
            });
        }
    },
    deleteClick: function (isSeeAll) {
        var obj = {};
        obj.IsDelete = true;
        //先去检查是否可删除
        Megi.grid("#tbTax", "dbSelected", {
            url: "/BD/TaxRate/IsCanDeleteOrInactive", param: obj, callback: function (response) {
                var alertMsg = BDQuote.GetQuoteMsg(response, isSeeAll);
                if (response.Success == true) {
                    //可删除弹出提示框是否继续删除
                    $.mDialog.confirm(alertMsg, {
                        callback: function () {
                            Megi.grid("#tbTax", "dbSelected", {
                                url: "/BD/TaxRate/DeleteTaxRateList", callback: function (delResponse) {
                                    $.mDialog.message(HtmlLang.Write(LangModule.BD, "DeleteTaxRate", "Delete TaxRate Successfully"));
                                    TaxRateSetting.bindGrid();
                                }
                            });
                        }
                    });
                    if (isSeeAll) {
                        $("#popup_message").css("max-height", "300px");
                    }
                    else {
                        $("#popup_message").css("max-height", "200px");
                    }
                
                } else {
                    //不可删除弹出提示。
                    $.mDialog.alert(alertMsg);
                    if (isSeeAll) {
                        $("#popup_message").css("max-height", "300px");
                    }
                    else {
                        $("#popup_message").css("max-height", "200px");
                    }
                }
            }
        });
    },
    //删除项目
    deleteItem: function (ids) {
        var obj = {};
        obj.KeyIDs = ids;
        obj.IsDelete = true;
        mAjax.submit("/BD/TaxRate/IsCanDeleteOrInactive", { param: obj }, function (response) {
            var alertMsg = BDQuote.GetQuoteMsg(response);
            if (response.Success == true) {
                //可删除弹出提示框是否继续删除
                $.mDialog.confirm(alertMsg, {
                    callback: function () {
                        mAjax.submit("/BD/TaxRate/DeleteTaxRateList", { param: obj }, function (delResponse) {
                            //if (delResponse.Success == true) {
                            $.mDialog.message(HtmlLang.Write(LangModule.BD, "DeleteTaxRate", "Delete TaxRate Successfully"));
                            TaxRateSetting.bindGrid();
                        });
                    }
                });
                $("#popup_message").css("max-height", "200px");
                
            } else {
                //不可删除弹出提示。
                $.mDialog.alert(alertMsg);
                $("#popup_message").css("max-height", "200px");
            }
        })
    },
    reload: function () {
        TaxRateSetting.bindGrid(function () {
            if ($("#hidIsSetup").val() == "true") {
                try {
                    $("#tbTax").datagrid('resize', {
                        width: 710
                    });
                } catch (exc) { }
            }
        });
    }
}

$(document).ready(function () {
    TaxRateSetting.init();
});