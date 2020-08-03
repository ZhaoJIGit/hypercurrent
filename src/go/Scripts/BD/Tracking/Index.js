/// <reference path="/intellisense/jquery.megi.common.js" />
/// <reference path="/intellisense/JieNor.Megi.Common.0x0009.js" />;
var Tracking = {
    hasChangeAuth: $("#hidChangeAuth").val() == "1" ? true : false,
    init: function () {
        Tracking.InitUI();
        Tracking.tabSelected();         //默认选中选项卡
        Tracking.AddLabel();            //添加选项卡
        Tracking.AddOneOption();        //增加选项         
        Tracking.saveClick();           //保存
        Tracking.cancelClick();         //取消
        Tracking.trackRename();         //重命名
        Tracking.trackDelete();         //删除
        Tracking.archiveTrackOption();  //归档
        Tracking.bindAction();
    },
    bindAction: function () {
        $('#mtabs').tabs({
            overflowTabs:false,
            onSelect: function () {
                var tabTitle = $('#mtabs').tabs('getSelected').panel('options').title;
                if (tabTitle) {
                    $('a[id^="aCancel"]').trigger("click");
                }
                
            }
        });
    },
    InitUI: function () {
        //如果用户没有权限，隐藏一些按钮
        if (!Tracking.hasChangeAuth) {
            $("#AddTrackingCategory").hide();
            $(".mg-track-closeIcon").hide();
            $(".mg-track-Rename").hide();

            $("a[flag='add']").hide();
        }
        
    },
    tabSelected: function () {
        var tabkey = $("#tabKeySel").val();
        tabkey = mText.htmlDecode(tabkey)
        tabkey = isNaN(parseInt(tabkey)) ? tabkey : +tabkey;

        $('#mtabs').tabs('select', tabkey);
    },
    AddLabel: function () {
        $("#AddTrackingCategory").click(function () {
            $(this).parent().hide();
            $('#mtabs').tabs('add', {
                title: '',
                content: $("#AddTbs").html()
            });

            //所有的多语言框重新渲染
            $("#TabContent-NEW").find(".easyui-validatebox").each(function () {
                $(this).initLangEditor();
            });
            //重新渲染字符屏蔽
            $(".easyui-filterchar", "#TabContent-NEW").each(function () {
                $(this).megifilterchar({
                    filterChar: ' '
                });
            });
            $('a[id^="AddAnotherOption"]').unbind("click");
            Tracking.AddOneOption();
            $('a[id^="aSave"]').unbind("click");
            Tracking.saveClick();
            $('a[id^="aCancel"]').unbind("click");
            Tracking.cancelClick();

            Megi.regClickToSelectAllEvt();
        });

        $("#btnRefresh").click(function () {
            mWindow.reload()
        });
    },
    AddOneOption: function () {
        $("#AddAnotherOption").click(function () {
            var length = $(this).prev().find("div").length;

            $(this).prev().find('div:last-child').after('<div><input class="textbox m-lang" name="MName-' + length + '" data-options="required:true,filterChar:\' \',validType:\'isBlank\'" id="item-' + length + '"/></div>');

            //对多语言框进行重新渲染
            $("#item-" + length).initLangEditor();
            $("#item-" + length).validatebox({
                required: false,
                valiType: 'isBlank'
            });
            Megi.regClickToSelectAllEvt();
        });

        $('a[id^="AddAnotherOption"][id*="Exist"]').click(function () {
            //第一增加输入框
            var length = $(this).prev().find("div").length;
            $(this).prev().find('div[class!="track-option"]').last().after('<div><input class="textbox  m-lang easyui-validatebox" data-options="required:true,filterChar:\' \',validType:\'isBlank\'" name="MName-Exist' + length + '" id="item-Exist' + length + '"/></div>');

            //增加保存和取消按钮
            var addClickId = $(this).attr('id');
            var saveId = "aSave" + addClickId[addClickId.length - 1];
            var cancelId = "aCancel" + addClickId[addClickId.length - 1];

            //是否引进增加了保存按钮
            var domButton = $("#existSaveBtn");
            var isExistButton = domButton != null && domButton.length > 0 ? true : false;

            if (!isExistButton) {

                $(this).parent().after('<div class="mg-track-bottom-buttons" id="existSaveBtn">' +
                       '<a href="'+$("#hideGoServer").val()+'/BD/Tracking/Index" class="easyui-linkbutton easyui-linkbutton-gray l-btn l-btn-small" id="' + cancelId + '" style="margin-right:20px"><span class="l-btn-left"><span class="l-btn-text"><span class="l-btn-left"><span class="l-btn-text">' + LangKey.Cancel + '</span></span></span></span></a>' +
                       '<a href="javascript:void(0);"  url="' + $("#hideGoServer").val() + '/BD/Tracking/Index" class="easyui-linkbutton easyui-linkbutton-yellow l-btn l-btn-small" id="' + saveId + '"><span class="l-btn-left"><span class="l-btn-text"><span class="l-btn-left"><span class="l-btn-text">' + LangKey.Save + '</span></span></span></span></a></div>');
                //保存事件从新绑定
                $('a[id^="aSave"]').unbind("click");
                Tracking.saveClick();
                $('a[id^="aCancel"]').unbind("click");
                Tracking.cancelClick();
            }


            //多语言输入框进行渲染
            $("#item-Exist" + length).initLangEditor();
            $("#item-Exist" + length).validatebox({
                required: false,
                valiType: 'isBlank'
            });
            Megi.regClickToSelectAllEvt();
        });
    },
    saveClick: function () {
        $('a[id^="aSave"]').click(function () {

            var tabPanelId = "#TabContent" + $(this).attr('id').substring(5);
            //修改更新后需要定位的panel
            var tabPanelTitle = $(".mg-track-content-item", tabPanelId).eq(0).text();
            if (!tabPanelTitle) {
                tabPanelTitle = $("#MName").val();
            }
            var toUrl = $(this).attr("url");
            var input = $(tabPanelId).find('input[class*="mg-track-items-input"]');
            var array = new Array();
            var j = 0;

            //是否是完全新增，还是在原有的跟踪项下增加一个子项
            var isNew = $(this).attr('id').indexOf("NEW") >= 0 ? true : false;

            if (isNew && !$("#TabContent-NEW").mFormValidate()) {
                return false;
            } else if (!$(".mg-track-content-items").mFormValidate()) {
                return false;
            }
            //跟踪项
            var model = {};

            //跟踪项的子项
            var optionModels = new Array();

            //完全新增
            if (isNew) {
                //第一步取跟踪项的实体
                //多语言
                var dataLang = $("#MName").getLangEditorData();

                var langArray = new Array();

                langArray.push(dataLang);

                model.MultiLanguage = langArray;

                $(".m-lang", ".tracking-options").each(function () {
                    if ($(this).val()) {

                        var optionLang = $(this).getLangEditorData();

                        //因为name的格式是：MName-0,需要去除
                        if (optionLang && optionLang.MFieldName) {
                            optionLang.MFieldName = optionLang.MFieldName.split("-")[0];
                        }

                        var optionlangArray = new Array();
                        optionlangArray.push(optionLang);

                        var optionModel = {};

                        optionModel.MultiLanguage = optionlangArray;
                        optionModel.MIsActive = true;

                        optionModels.push(optionModel);
                    }
                });
            } else {
                //在原有跟踪项上增加一个子项
                model.MItemID = $("#MItemID", tabPanelId).val();

                $(".easyui-validatebox", tabPanelId).each(function () {
                    if ($(this).val()) {

                        var optionLang = $(this).getLangEditorData();

                        //因为name的格式是：MName-Exist0,需要去除
                        if (optionLang && optionLang.MFieldName) {
                            optionLang.MFieldName = optionLang.MFieldName.split("-")[0];
                        }

                        var optionlangArray = new Array();
                        optionlangArray.push(optionLang);

                        var optionModel = {};
                        //父项的ID
                        optionModel.MItemID = $("#MItemID", tabPanelId).val();
                        optionModel.MultiLanguage = optionlangArray;
                        optionModel.MIsActive = true;
                        optionModels.push(optionModel);
                    }
                });

            }

            mAjax.submit("/BD/Tracking/SaveTrackingInfo", { model: model, optionsModels: optionModels }, function (msg) {
                if (msg.Success == true) {
                    //为了兼容Safari的跳转
                    var tips = isNew ? HtmlLang.Write(LangModule.BD, "TrackAddSuccess", "跟踪项新增成功") :
                        HtmlLang.Write(LangModule.Common, "SaveSuccessful", "保存成功！");
                    $.mDialog.message(tips);

                    window.setTimeout(function () { Tracking.go(toUrl, tabPanelTitle) }, 500);

                } else {
                    $.mDialog.alert(msg.Message);
                }
            });
            return false;
        });

    },
    go: function (toUrl, tabPanelTitle) {
        tabPanelTitle = mText.htmlEncode(tabPanelTitle);
        mWindow.reload(toUrl + "?id=" + tabPanelTitle + "&cache=" + Math.random());
    },
    cancelClick: function () {
        $('a[id^="aCancel"]').click(function () {
            var tabPanelId = "#TabContent" + $(this).attr('id').substring(7);
            var tabSelectIndex = Tracking.getSelectTabIndex();
            var toUrl = $(this).attr("href");
            var input = $(tabPanelId).find('input[class*="mg-track-items-input"]');
            if ($(tabPanelId).find('input[id="MItemID"]').length > 0) {  //判断是否是已经存在的数据
                tabPanelTitle = $(tabPanelId).find('span[class="mg-track-content-item"]').eq(0).text();
            }
            else {
                if (input.length > 0 && input.eq(0).val() != "") { tabPanelTitle = input.eq(0).val(); }
            }
            mWindow.reload(toUrl + "?id=" + tabSelectIndex);
            //window.location = toUrl + "?id=" + tabSelectIndex;
            return false;
        });
    },
    trackRename: function () {
        $('a[class="mg-track-Rename"]').click(function () {
            var pkId;
            var curName;
            var tabSelectIndex = Tracking.getSelectTabIndex();
            var tabId = "#TabContent" + (tabSelectIndex+1);

            if ($(this).parent().find('input[id="MPKID"]').val() != undefined) {
                //pkId = $(this).parent().find('input[id="MPKID"]').val();                        //表头数据PKID
                pkId = $(tabId).find('input[id="MItemID"]').val();                        //表头数据ID
                curName = $(this).parent().find('span[class="mg-track-content-item"]').text();  //当前编辑名称
                Megi.dialog({
                    title: HtmlLang.Write(LangModule.BD, "EditTracking", "Edit Tracking"),
                    width: 400,
                    height: 400,
                    href: '/BD/Tracking/CategoryEdit?tabIndex='+tabSelectIndex+'&pkId=' + pkId + "&curName=" + curName
                });
            }
            else {
                //pkId = $(this).parent().find('input[id="MEntryPKID"]').val();                   //表体数据PKID
                //跟踪项ID
                var trackId =  $(tabId).find('input[id="MItemID"]').val();
                pkId = $(this).parent().find('input[id="MEntryID"]').val();                     //表体数据PKID
                curName = $(this).parent().find('span[class="mg-track-content-item"]').text();  //当前编辑名称
                var tabTitle = $('#mtabs').tabs('getSelected').panel('options').title;

                curName = mText.htmlEncode(curName);
                tabTitle = mText.htmlEncode(tabTitle);

                Megi.dialog({
                    title: HtmlLang.Write(LangModule.BD, "EditTracking", "Edit Tracking"),
                    width: 400,
                    height: 400,
                    href: '/BD/Tracking/CategoryOptionEdit?tabIndex=' + tabSelectIndex + '&pkId=' + pkId + "&trackId=" + trackId
                });
            }
        });
    },
    trackDelete: function () {
        $('a[class="mg-track-delete"]').click(function () {
            var Id;
            var tabIndex = Tracking.getSelectTabIndex();
            if ($(this).parent().find('input[id="MItemID"]').val() != undefined) {
                Id = $(this).parent().find('input[id="MItemID"]').val();                        //表头数据ItemID
                //tabTitle = $('#mtabs').tabs('getSelected').panel('options').title;              //当前标题
                $.mDialog.confirm(HtmlLang.Write(LangModule.BD, "WillBePermanentlyDeleted", "this tracking will be permanently deleted."), {
                    callback: function () {
                        var params = { MItemID: Id };
                        mAjax.submit("/BD/Tracking/TrackingDelete", { info: params }, function (msg) {
                            if (msg.Success) {
                                Tracking.reload(tabIndex);
                            } else {
                                $.mDialog.alert(msg.Message);
                            }
                        }, "", false);
                    }
                });
            } else {
                Id = $(this).parent().find('input[id="MEntryID"]').val();                       //表体数据EntryID
                $.mDialog.confirm(HtmlLang.Write(LangModule.BD, "OptionWilBeDeleted", "This option will be permanently deleted."), {
                    callback: function () {
                        var params = { MEntryID: Id };
                        mAjax.submit("/BD/Tracking/TrackingOptDelete", { info: params }, function (msg) {
                            if (msg && msg.Success) {
                                Tracking.reload(tabIndex);
                            } else {
                                $.mDialog.alert(msg.Message);
                            }
                        }, "", false);
                    }
                });
            }
        });
    },
    archiveTrackOption: function () {
        $(".mg-track-status").each(function () {
            $(this).click(function () {
                var optionId = $(this).parent().find('input[id="MEntryID"]').val()
                if (!optionId) {
                    return;
                }
                var statusStringDom = $(this).prev(".track-option-name")
                var statusDom = $(this).parent().find(".mg-track-status");

                var status = statusDom.attr("value") == "1" ? true : false;

                //如果是禁用的话 就隐藏编辑和删除按钮
                var deleteid = "#mg-track-delete" + optionId;
                var renameid = "#mg-track-Rename" + optionId;
                if (status) {
                    var confirmMsg = HtmlLang.Write(LangModule.Common, "AreYouSureToArchive", "你确定要继续禁用吗？");
                    mAjax.submit("/BD/Tracking/IsCanDeleteOrInactive", { info: { MEntryID: optionId } }, function(response) {
                        //可删除弹出提示框是否继续删除
                        $.mDialog.confirm('<div style="text-align:left;margin-left:20px;">' + mText.encode(response.Message) + confirmMsg + '</div>', {
                            callback: function() {
                                mAjax.submit("/BD/Tracking/ArchiveTrackEntry", { entryId: optionId, status: !status }, function(msg) {
                                    if (msg.Success) {
                                        if (status) {
                                            statusDom.removeClass("mg-track-enable");
                                            statusDom.addClass("mg-track-disable");
                                            statusDom.attr("value", "0");

                                            var disable = HtmlLang.Write(LangModule.BD, "Disable", "禁用");
                                            statusStringDom.text(disable);
                                            //隐藏
                                            $(deleteid).css("visibility", "hidden");
                                            $(renameid).css("visibility", "hidden");

                                        } else {
                                            statusDom.removeClass("mg-track-disable");
                                            statusDom.addClass("mg-track-enable");
                                            statusDom.attr("value", "1");
                                            var enable = HtmlLang.Write(LangModule.BD, "Enable", "启用");
                                            statusStringDom.text(enable);
                                            //显示visible
                                            $(deleteid).css("visibility", "visible");
                                            $(renameid).css("visibility", "visible");
                                        }
                                    }
                                }, "", true);
                            }
                        });
                    });
                } else {
                    mAjax.submit("/BD/Tracking/ArchiveTrackEntry", { entryId: optionId, status: !status }, function (msg) {
                        if (msg.Success) {
                            if (status) {
                                statusDom.removeClass("mg-track-enable");
                                statusDom.addClass("mg-track-disable");
                                statusDom.attr("value", "0");

                                var disable = HtmlLang.Write(LangModule.BD, "Disable", "禁用");
                                statusStringDom.text(disable);
                                //隐藏
                                $(deleteid).css("visibility", "hidden");
                                $(renameid).css("visibility", "hidden");

                            } else {
                                statusDom.removeClass("mg-track-disable");
                                statusDom.addClass("mg-track-enable");
                                statusDom.attr("value", "1");
                                var enable = HtmlLang.Write(LangModule.BD, "Enable", "启用");
                                statusStringDom.text(enable);
                                //显示visible
                                $(deleteid).css("visibility", "visible");
                                $(renameid).css("visibility", "visible");
                            }
                        }
                    }, "", true);
                }
  
            });
        });
    },
    reload: function (tab) {
        var url = $("#hideGoServer").val() + "/BD/Tracking/Index/?id=" + tab + "&i=0";
        mWindow.reload(url);
    },
    getSelectTabIndex: function () {
        var selectedTab = $("#mtabs").tabs("getSelected");
        var selectTabIndex = $("#mtabs").tabs("getTabIndex", selectedTab);

        return selectTabIndex;
    }
}

$(document).ready(function () {
    Tracking.init();
});