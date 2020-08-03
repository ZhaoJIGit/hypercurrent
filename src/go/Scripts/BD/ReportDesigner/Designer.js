(function (reportDesigner) {
    var suppressCloseConfirmation = false;
    PreviewBase.regHidMaskEvt();
    $(window).on('beforeunload', function (e) {
        if (!suppressCloseConfirmation && reportDesigner.GetDesignerModel().isDirty()) {            
            return HtmlLang.Write(LangModule.Common, "UnSavedChangeAlarm", "You have unsaved changes on the page");
        }
    });

    reportDesigner.CustomizeMenuActions.AddHandler(function (_, e) {
        var saveAction = e.Actions.filter(function (x) { return x.text === 'Save' })[0];
        saveAction.text = HtmlLang.Write(LangModule.Common, "SaveAndClose", "Save and Close");
        
        //隐藏预览按钮
        var previewAction = e.Actions.filter(function (x) { return x.text === 'Preview' })[0];
        previewAction.visible = false;

        //隐藏Run Wizard
        var runWizardAction = e.Actions.filter(function (x) { return x.text === 'Run Wizard' })[0];
        runWizardAction.visible = false;
    });
    reportDesigner.SaveCommandExecuted.AddHandler(function (_, e) {
        suppressCloseConfirmation = true;
        var result = JSON.parse(e.Result);
        var url = result.Data.redirectUrl.split("?")[0];
        var param = result.Data.redirectUrl.split("?")[1];
        PreviewBase.postUrl(url, param);
    });
    if ($("#hidCurrentLangID", top.document).val() == "0x7804") {
        //, "Parameters": "参数", "Select...": "选择..."
        var langVals = { "Actions": "操作", "Field list": "字段列表", "Solid": "细线", "Dot": "点线", "To create an item click the Add button.": "点击添加按钮来创建排序字段", "Sort By:": "排序方式：", "Fields": "字段", "Properties": "属性", "Collapse": "隐藏", "Open": "显示", "": "", "": "" };
        var oriGetLocalization = DevExpress.Designer.getLocalization;
        DevExpress.Designer.getLocalization = function (key) {
            if (langVals[key]) {
                return langVals[key];
            }
            else {
                return oriGetLocalization(key);
            }
        };
    }
})(reportDesigner);