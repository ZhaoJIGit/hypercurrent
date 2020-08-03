var VoucherSettingStatusEnum;
(function (VoucherSettingStatusEnum) {
    //可选，可取消
    VoucherSettingStatusEnum[VoucherSettingStatusEnum["normalEnable"] = 0] = "normalEnable";
    //正常 不可选，不可取消
    VoucherSettingStatusEnum[VoucherSettingStatusEnum["normarlDisable"] = 1] = "normarlDisable";
})(VoucherSettingStatusEnum || (VoucherSettingStatusEnum = {}));
/*
    模块的枚举
*/
var ModuleEnum;
(function (ModuleEnum) {
    ModuleEnum[ModuleEnum["Invoice"] = 0] = "Invoice";
    ModuleEnum[ModuleEnum["Purchase"] = 1] = "Purchase";
    ModuleEnum[ModuleEnum["Expense"] = 2] = "Expense";
    ModuleEnum[ModuleEnum["Bank"] = 3] = "Bank";
})(ModuleEnum || (ModuleEnum = {}));
var VoucherSettingEnum;
(function (VoucherSettingEnum) {
    VoucherSettingEnum[VoucherSettingEnum["EntryMergeSet"] = 0] = "EntryMergeSet";
    VoucherSettingEnum[VoucherSettingEnum["ExplanationSet"] = 1] = "ExplanationSet";
})(VoucherSettingEnum || (VoucherSettingEnum = {}));
//# sourceMappingURL=GLEnum.js.map