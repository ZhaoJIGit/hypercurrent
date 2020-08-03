var Status;
(function (Status) {
    Status[Status["draft"] = -1] = "draft";
    Status[Status["saved"] = 0] = "saved";
    Status[Status["approved"] = 1] = "approved";
})(Status || (Status = {}));
var FixAssetsChangeTypeEnum;
(function (FixAssetsChangeTypeEnum) {
    /// <summary>
    /// 0001 基本信息变更（编号、名称、清理期间、数量）
    /// </summary>
    FixAssetsChangeTypeEnum[FixAssetsChangeTypeEnum["Basic"] = 1] = "Basic";
    /// <summary>
    /// 0010 其他变更（科目、核算维度、已折旧期间、累计折扣、减值准备）
    /// </summary>
    FixAssetsChangeTypeEnum[FixAssetsChangeTypeEnum["Other"] = 2] = "Other";
    /// <summary>
    /// 0100 折旧策略变更（折旧方式、预计使用期限、开始折旧期间、残值率）
    /// </summary>
    FixAssetsChangeTypeEnum[FixAssetsChangeTypeEnum["Strategy"] = 4] = "Strategy";
    /// <summary>
    /// 1000 原值变更（原值，采购日期）
    /// </summary>
    FixAssetsChangeTypeEnum[FixAssetsChangeTypeEnum["Original"] = 8] = "Original";
    /// <summary>
    /// 10000处置变更
    /// </summary>
    FixAssetsChangeTypeEnum[FixAssetsChangeTypeEnum["Handle"] = 16] = "Handle";
})(FixAssetsChangeTypeEnum || (FixAssetsChangeTypeEnum = {}));
//# sourceMappingURL=FAEnums.js.map