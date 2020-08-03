var Preview = {                                                               
    previewUrl: "/BD/Print/Preview",
    init: function () {
        PreviewBase.regHidMaskEvt();
        Preview.loadPDF();
        $("#documentViewer1_Splitter_Toolbar_Menu").css({ "margin-left": "8px" });
        Preview.hidTmplCombo();
        LoadingPanel.Hide();
        Preview.reloadPreview();

        /* 重写SaveToWindow事件，解决新开窗口保存不了问题:
         * 1. Edge浏览器新开窗口保存报Permission Denied错误
         * 2. Safari浏览器新开窗口保存跳到登录页面
         */
        if (Megi.isEdge() || Megi.isSafari()) {
            ASPxClientReportViewer.prototype.SaveToWindow = function (format) {
                var url = "/BD/Print/ExportDocument?reportType=" + $("#hidReportType").val() + "&jsonParam=" + $("#hidJsonParam").val() + "&dxArgument=" + escape("saveToWindow=format:" + format);
                var h = Math.min(645, window.screen.height);
                var w = 1180;
                var top = (window.screen.height / 2) - (h / 2 + 50);
                var left = (window.screen.width / 2) - (w / 2 + 10);
                var title = $("#hidReportName").val();
                var w = window.open(url, title, 'toolbars=no, resizable=yes, scrollbars=yes, width=' + w + ', height=' + h + ', top=' + top + ',left=' + left);
            }
        }
        
        //重写SaveToDisk事件，解决Safari浏览器保存没反应问题
        if (Megi.isSafari()) {
            ASPxClientReportViewer.prototype.SaveToDisk = function (format) {
                var url = "/BD/Print/ExportDocument";
                var data = { reportType: $("#hidReportType").val(), jsonParam: $("#hidJsonParam").val(), dxArgument: "saveToDisk=format:" + format };
                Megi.openNewTab(url, data, false);
            }
        }
    },
    onBtnClick: function (sender, e, layoutId) {
        top.accessRequest(function () {

        }, false);

        //加载设置、打开设计器、还原按钮事件
        var psIDParam = Preview.getPrtSettingIDParam();
        switch (e.item.name)
        {
            case "loadSetting":
                LoadingPanel.Show();
                PreviewBase.postUrl(Preview.previewUrl, psIDParam + "&source=Preview");
        	    break;
            case "reportDesigner":
                LoadingPanel.Show();
                PreviewBase.postUrl('/BD/ReportDesigner', psIDParam + "&source=Designer");
                break;
            case "restoreToDefault":
                if (confirm(LangKey.AreYouSureToRestore)) {
                    LoadingPanel.Show();
                    PreviewBase.postUrl(Preview.previewUrl, psIDParam + "&source=Restore&layoutId=" + layoutId);
                }
                break;
            case "printPDF":
                window.open($("#hidPdfUrl").val());
                break;
        }
    },
    onComboValueChanged: function (s, e) {
        if (e.item.name == 'SaveFormat') {
            $("#documentViewer1_Splitter_Toolbar_Menu_ITCNT2_SaveFormat_DDD_PW-1").hide();
        }
        if (e.item.name == 'cbTmpl') {
            $("#documentViewer1_Splitter_Toolbar_Menu_ITCNT3_cbTmpl_DDD_PW-1").hide();

            var layoutId = e.editor.lastSuccessValue;//lastChangedValue
            var psIDParam = Preview.getPrtSettingIDParam(layoutId);
            LoadingPanel.Show();
            PreviewBase.postUrl(Preview.previewUrl, psIDParam + "&source=Preview&layoutId=" + layoutId);
        };
    },
    documentViewer_Init: function (sender, e, layoutId) {
        //设置模板下拉框选中项
        var cbTmpl = sender.GetToolbar().GetItemTemplateControl("cbTmpl");
        cbTmpl.SetValue(layoutId);

        //自适应屏幕宽度
        var dvWrapper = $("#documentViewer1_Splitter");
        dvWrapper.width($(".mBoxWrap", parent.document).width());
        $(".mBoxContent", parent.document).width(dvWrapper.width());

        //解决IE/Safari弹框被PDF遮挡问题
        if (Megi.isIE() || Megi.isSafari()) {
            $(document).click(function () {
                Preview.showHideBlankIfr("ITCNT2_SaveFormat", $("#documentViewer1_Splitter_Toolbar_Menu_ITCNT2_SaveFormat_DDD_PW-1"), $("#ifrBlank1"));
                Preview.showHideBlankIfr("ITCNT3_cbTmpl", $("#documentViewer1_Splitter_Toolbar_Menu_ITCNT3_cbTmpl_DDD_PW-1"), $("#ifrBlank2"));
            });
        }
    },
    showHideBlankIfr: function (id, trigerObj, ifrBlank){
        if (trigerObj.is(":visible")) {
            if (Megi.isSafari()) {
                if ($("#divPdf embed").length == 1) {
                    $("#documentViewer1_Splitter_Toolbar_Menu_{0}_DDD_L_D".replace("{0}", id)).height(38);
                }
            }
            else {
                trigerObj.height(38);
                ifrBlank.css({ "z-index": 20, "opacity": 0.5 });
                ifrBlank.show();
                ifrBlank.width(trigerObj.width()).height(trigerObj.height());
            }
        }
        else if(Megi.isIE()) {
            ifrBlank.css({ "z-index": 0, "opacity": 0 });
            ifrBlank.hide();
        }
    },
    insertBlankIfr: function (ifrId, tgtId) {
        $("<iframe id='" + ifrId + "' src='about:blank' frameborder='0' style='position:absolute;z-index:0;display:none;position: absolute;'></iframe>").insertAfter(tgtId);
    },
    loadPDF: function () {
        //隐藏Dev预览
        $("#documentViewer1_Splitter_1").hide();

        //IE浏览器插入空白IFrame解决PDF遮挡下拉框问题
        if (Megi.isIE()) {
            Preview.insertBlankIfr("ifrBlank1", "#documentViewer1_Splitter_Toolbar_Menu_ITCNT2_SaveFormat_DDD_PW-1");
            Preview.insertBlankIfr("ifrBlank2", "#documentViewer1_Splitter_Toolbar_Menu_ITCNT3_cbTmpl_DDD_PW-1");
        }

        var dialogW = $(".mBoxWrap", parent.document).width();
        $("#documentViewer1_Splitter_1").closest("tbody").append("<tr><td><div id='divPdf' style='z-index:0;width:" + dialogW + "px;'></div></td></tr>");
        var dialogH = $(".mBoxWrap", parent.document).height();
        var headerH = $(".mBoxTitle", parent.document).height();
        var pdfH = dialogH - headerH - 33;
        //if ($("#divPdf embed").length == 0) {
        //    pdfH -= 20;
        //}
        
        var isSafari = Megi.isSafari();
        var cssObj = {};
        //Safaril浏览器增加下拉框占位空间
        if (isSafari) {
            pdfH -= 35;
            cssObj["background-color"] = "#525659";
            cssObj["padding-top"] = "35px";
        }

        cssObj["height"] = pdfH;
        $("#divPdf").css(cssObj);
        //加载PDF
        PDFObject.embed($("#hidPdfUrl").val(), "#divPdf");

        //如果pdf嵌入失败，则还原Safari浏览器的样式设置
        if ($("#divPdf embed").length == 0 && isSafari) {
            $("#divPdf").css({ "background-color": "#FFFFFF", "padding-top": "0px" });
            var cbSaveFormat = $("#documentViewer1_Splitter_Toolbar_Menu_ITCNT2_SaveFormat_DDD_L_D");
            var cbTmpl = $("#documentViewer1_Splitter_Toolbar_Menu_ITCNT3_cbTmpl_DDD_L_D");
            cbSaveFormat.height(cbSaveFormat.closest("div").height());
            cbTmpl.height(cbTmpl.closest("div").height());
        }
    },
    getPrtSettingIDParam: function (ptId) {
        if (!ptId) {            
            ptId = window['documentViewer1_Splitter_Toolbar_Menu_ITCNT3_cbTmpl'].lastSuccessValue;
        }
        return "printSettingID=" + ptId;
    },
    reloadPreview: function () {
        var tabIndex = $("iframe:visible", parent.parent.document).parent().attr("data-index");
        $(".m-tab li:eq(" + tabIndex + ") .m-tab-inner", top.window.document).off("click.prt").on("click.prt", function () {
            if (navigator.userAgent.toLowerCase().indexOf('firefox') > -1) {
                LoadingPanel.Show();
                PreviewBase.postUrl(Preview.previewUrl, Preview.getPrtSettingIDParam() + "&source=Preview");
            }
        });
    },
    hidTmplCombo: function () {
        //隐藏模板下拉框
        if (window['documentViewer1_Splitter_Toolbar_Menu_ITCNT3_cbTmpl'].lastSuccessValue == null) {
            $("#documentViewer1_Splitter_Toolbar_Menu_ITCNT3_cbTmpl").hide();
            $("#documentViewer1_Splitter_Toolbar_Menu_DXI12_").hide();
            $("#documentViewer1_Splitter_Toolbar_Menu_DXI11_IS").hide();
        }
    },
    getUrlParam: function (name) {
        var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
        if (results == null || results[1] == undefined) {
            return null;
        }
        else {
            return results[1];
        }
    }
}

$(document).ready(function () {
    Preview.init();
    //$.ajax({
    //    type: "POST",
    //    url: "/BD/Print/WriteLog?r=" + new Date().getTime()
    //});
});