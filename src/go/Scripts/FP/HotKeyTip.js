var hotKeyTip = (function() {
    function hotKeyTip() {
        this.tipHotKeyDiv = ".tip-hotkey-div";
        this.tipHotKeyTableDiv = ".tip-hotkey-table-div";
        this.hotKeyBtn = "#tabHotKey";
        this.hotHandleBtn = "#tabHotHandle";
        this.hotKeyDiv = "#hotKeyDiv";
        this.hothandleDiv = "#hotHandleDiv";
        this.tipHotKeyTabShow = "tip-hotkey-tab-show";
        this.hotKeyClick = false;
    }
    hotKeyTip.prototype.init = function() {
        this.initEvent();
    };
    hotKeyTip.prototype.initEvent = function () {
        var _this = this;
        $(_this.tipHotKeyDiv).mTip({
            target: $(_this.tipHotKeyTableDiv),
            width: 400,
            parent: $(_this.tipHotKeyTableDiv).parent()
        });
        $(_this.hotKeyBtn).off("click").on("click", function () {
            _this.showHotKeyDiv();
        });
        $(_this.hotHandleBtn).off("click").on("click", function() {
            _this.showHotHandleDiv();
        });
    };
    hotKeyTip.prototype.showHotKeyDiv = function() {
        if (!$(this.hotKeyBtn).hasClass(this.tipHotKeyTabShow)) {
            $(this.hotKeyBtn).addClass(this.tipHotKeyTabShow);
            $(this.hotHandleBtn).removeClass(this.tipHotKeyTabShow);
            $(this.hotKeyDiv).show();
            $(this.hothandleDiv).hide();
        }
    };
    hotKeyTip.prototype.showHotHandleDiv = function() {
        if (!$(this.hotHandleBtn).hasClass(this.tipHotKeyTabShow)) {
            $(this.hotHandleBtn).addClass(this.tipHotKeyTabShow);
            $(this.hotKeyBtn).removeClass(this.tipHotKeyTabShow);
            $(this.hotKeyDiv).hide();
            $(this.hothandleDiv).show();
        }
    };
    return hotKeyTip;
}());
$(document).ready(function () {
    var hotkeytip = new hotKeyTip();
    hotkeytip.init();
});