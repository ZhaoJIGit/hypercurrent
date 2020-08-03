var BDPrefixSetting = /** @class */ (function () {
    function BDPrefixSetting() {
        this.getPrefixSettingUrl = "/BD/OrgPrefixSetting/GetPrefixSetting";
        this.savePrefixSettingUrl = "/BD/OrgPrefixSetting/SavePrefixSetting";
    }
    /**
    * 加载凭证数据
    */
    BDPrefixSetting.prototype.loadPrefixSettingData = function (module, callback) {
        mAjax.post(this.getPrefixSettingUrl, { module: module }, function (data) {
            callback(data);
        }, null, true);
    };
    /**
    * 保存数据
    */
    BDPrefixSetting.prototype.savePrefixSettingData = function (model, callback) {
        mAjax.submit(this.savePrefixSettingUrl, { model: model }, function (result) {
            callback(result);
        });
    };
    return BDPrefixSetting;
}());
//# sourceMappingURL=BDPrefixSetting.js.map