;
/*
    整个系统需要用到的常量定义
    这个js最好在最后加载，这样就可以覆盖同名变量
    1.多语言信息
    2.下拉框可选择，可编辑需要用到的常量
*/
//
var dataLangAttrName = "data-lang";
//
var mLangClassName = "m-lang";
//
var MPKID = "MPKID";
//
var txtLangId = "txtLang";
//
var langIdAttrName = "langId";
//
var langNameAttrName = "langName";
//
var fieldAttrName = "field";
//
var pkIdAttrName = "pkID";
//
var oldValueAttrName = "oldValue";
//
var MLocaleID = "MLocaleID";
//
var MValue = "MValue";
//
var MFieldName = "MFieldName";
//
var MMultiLanguageValue = "MMultiLanguageValue";
//
var MMultiLanguageField = "MMultiLanguageField";
//
var configLanguageTitle = "config language";
//
var LangButtonId = "LangBtn";
//
var mLanguageClassName = "m-language";
//
var mLanguageDivClassName = "m-language-div";
//
var mLangBtnClassName = "m-lang-btn";
//
var langConfigId = "LangConfig";
//
var LangItemContainerId = "LangItemContainer";
//
var langCancelClass = "lang-cancel";
//
var langOkClass = "lang-ok";
//
var OKKeyName = "ok";
//
var CancelKeyName = "cancel";
//数据字段对应的class名称
var dataFieldSelector = ".mg-data";
//多语言字段的类名
var langFieldClassName = "m-lang";
//多语言输入框的添加样式
var langInputClassName = "m-lang-input";
//多语言ok按钮
var langOkClassName = "m-lang-ok";
//多语言div的id
var langDivId = "langDivId";
//多语言数据的名称
var langDataName = "langDataName";
//多语言取消按钮
var langCancelClassName = "m-lang-cancel";
//多语言字段对应的数据属性名称
var langFieldAttrName = "data-lang";
//主见关键字段保存在form中的获取方式
var keyFieldSelector = ".m-form-key";
//
var multiLanguageName = "MultiLanguage";

var dashedLineClassName = "mg-dashed-line";
/*银行可选可修改---------------------------*/
//
//银行/信用卡/现金
var bank = "bank", credit = "credit", cash = "cash";

/*页面布局的常量*/
//可滚动区域的样式
var mainScroll = ".m-imain";
//顶部toolbar的样式
var topToolbar = ".m-toolbar";
//底部操作按钮的样式
var footToolbar = ".m-toolbar-footer";

//发票标题前缀
var mTitle_Pre_Invoice = "IV";
var mTitle_Pre_Invoice_Red = "CN";
var mTitle_Pre_Bill = "Bill";
var mTitle_Pre_Bill_Red = "BCN";
var mTitle_Pre_Expense = "EC";
var mTitle_Pre_Payment = "Pay";
var mTitle_Pre_Receive = "Rec";
var mTitle_Pre_Transfer = "Trans";

var mEmailReg = /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i;

//银行账户类型
window.BankTypeEnum = [
    //其他银行
    {
        type: 0,
        height:380,
        name: HtmlLang.Write(LangModule.Bank, "other", "Other"),
        baseName: "other"
    },
    //银行
    {
        type: 1,
        height: 480,
        name: HtmlLang.Write(LangModule.Bank, "bank", "Bank"),
        baseName: "Bank"
    },
    //信用卡
    {
        type: 2,
        height: 440,
        name: HtmlLang.Write(LangModule.Bank, "credit", "Credit"),
        baseName: "Credit Card"
    },
    //现金 
    {
        type: 3,
        height: 400,
        name: HtmlLang.Write(LangModule.Bank, "cash", "Cash"),
        baseName: "Cash"
    },
    //贝宝
    {
        type: 4,
        height: 380,
        name: HtmlLang.Write(LangModule.Bank, "Paypal", "PayPal"),
        baseName: "Paypal"
    },
    //支付宝
    {
        type: 5,
        height: 380,
        name: HtmlLang.Write(LangModule.Bank, "alipay", "Alipay"),
        baseName: "Alipay"
    }
]
