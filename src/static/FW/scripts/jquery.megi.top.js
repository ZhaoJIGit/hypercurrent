/// <reference path="jquery-1.8.2.min.js" />
/// <reference path="jquery.megi.extend.js" />

//多语言对应的模块
var LangModule = { Common: '', Org: 'Org', BD: 'BD', IV: 'IV', Acct: 'Act', Bank: 'Bnk', User: 'Usr', Contact: 'Ctc', Report: 'Rpt', My: 'My', Login: 'Lg', Docs: 'Doc', PA: 'PA', GL: 'GL', FP: 'FP', FA: 'FA' };
/*多语言输入框的常量*/

var HtmlLang = {
    Write: function (module, key, defaultValue) {
        var moduleId = 1;
        switch (module) { case LangModule.Common: moduleId = 1; break; case LangModule.Org: moduleId = 2; break; case LangModule.BD: moduleId = 3; break; case LangModule.IV: moduleId = 4; break; case LangModule.Acct: moduleId = 5; break; case LangModule.Bank: moduleId = 6; break; case LangModule.User: moduleId = 7; break; case LangModule.Contact: moduleId = 8; break; case LangModule.Report: moduleId = 9; break; case LangModule.My: moduleId = 10; break; case LangModule.Login: moduleId = 11; break; case LangModule.Docs: moduleId = 12; break; case LangModule.PA: moduleId = 13; break; case LangModule.GL: moduleId = 14; break; case LangModule.FP: moduleId = 15; break; case LangModule.FA: moduleId = 16; break; }
        var value = LangKey[module + key];

        if (!value) {
            //$.ajax({
            //    type: "POST",
            //    url: "/Framework/UpdateLang",
            //    data: { module: moduleId, key: key, defaultValue: defaultValue },
            //    success: function (msg) {

            //    }
            //});

            return defaultValue;
        }
        return value;
    }
}

  