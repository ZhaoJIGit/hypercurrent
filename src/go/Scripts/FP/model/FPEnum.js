var FPTableIssueStatusEnum;
(function (FPTableIssueStatusEnum) {
    FPTableIssueStatusEnum[FPTableIssueStatusEnum["NotIssued"] = 0] = "NotIssued";
    FPTableIssueStatusEnum[FPTableIssueStatusEnum["PartlyIssued"] = 1] = "PartlyIssued";
    FPTableIssueStatusEnum[FPTableIssueStatusEnum["Issued"] = 2] = "Issued";
})(FPTableIssueStatusEnum || (FPTableIssueStatusEnum = {}));
var FPFapiaoStatusEnum;
(function (FPFapiaoStatusEnum) {
    FPFapiaoStatusEnum[FPFapiaoStatusEnum["Obsolete"] = 0] = "Obsolete";
    FPFapiaoStatusEnum[FPFapiaoStatusEnum["Normal"] = 1] = "Normal";
    FPFapiaoStatusEnum[FPFapiaoStatusEnum["OutOfControl"] = 2] = "OutOfControl";
    FPFapiaoStatusEnum[FPFapiaoStatusEnum["Unnormal"] = 3] = "Unnormal";
    FPFapiaoStatusEnum[FPFapiaoStatusEnum["Credit"] = 4] = "Credit";
})(FPFapiaoStatusEnum || (FPFapiaoStatusEnum = {}));
var FPEnum;
(function (FPEnum) {
    FPEnum[FPEnum["Sales"] = 0] = "Sales";
    FPEnum[FPEnum["Purchase"] = 1] = "Purchase";
})(FPEnum || (FPEnum = {}));
var FPFapiaoTypeEnum;
(function (FPFapiaoTypeEnum) {
    /// <summary>
    /// 增值税普通发票
    /// </summary>
    FPFapiaoTypeEnum[FPFapiaoTypeEnum["Common"] = 0] = "Common";
    /// <summary>
    /// 增值税专用发票
    /// </summary>
    FPFapiaoTypeEnum[FPFapiaoTypeEnum["Special"] = 1] = "Special";
})(FPFapiaoTypeEnum || (FPFapiaoTypeEnum = {}));
var ReconcileTabEnum;
(function (ReconcileTabEnum) {
    ReconcileTabEnum[ReconcileTabEnum["Reconcile"] = 0] = "Reconcile";
    ReconcileTabEnum[ReconcileTabEnum["Statement"] = 1] = "Statement";
    ReconcileTabEnum[ReconcileTabEnum["Transaction"] = 2] = "Transaction";
    ReconcileTabEnum[ReconcileTabEnum["Coding"] = 3] = "Coding";
    ReconcileTabEnum[ReconcileTabEnum["ImportLog"] = 4] = "ImportLog";
})(ReconcileTabEnum || (ReconcileTabEnum = {}));
var ReconcileStatusEnum;
(function (ReconcileStatusEnum) {
    ReconcileStatusEnum[ReconcileStatusEnum["None"] = 0] = "None";
    ReconcileStatusEnum[ReconcileStatusEnum["Reconciled"] = 1] = "Reconciled";
    ReconcileStatusEnum[ReconcileStatusEnum["NoReconcile"] = 2] = "NoReconcile";
})(ReconcileStatusEnum || (ReconcileStatusEnum = {}));
var VerifyTypeEnum;
(function (VerifyTypeEnum) {
    VerifyTypeEnum[VerifyTypeEnum["NoVerify"] = 0] = "NoVerify";
    VerifyTypeEnum[VerifyTypeEnum["ScanVerify"] = 1] = "ScanVerify";
    VerifyTypeEnum[VerifyTypeEnum["CheckVerify"] = 2] = "CheckVerify";
    VerifyTypeEnum[VerifyTypeEnum["NoNeedVerify"] = 3] = "NoNeedVerify";
})(VerifyTypeEnum || (VerifyTypeEnum = {}));
var FapiaoSourceEnum;
(function (FapiaoSourceEnum) {
    FapiaoSourceEnum[FapiaoSourceEnum["Input"] = 0] = "Input";
    FapiaoSourceEnum[FapiaoSourceEnum["Import"] = 1] = "Import";
    FapiaoSourceEnum[FapiaoSourceEnum["Excel"] = 2] = "Excel";
})(FapiaoSourceEnum || (FapiaoSourceEnum = {}));
//# sourceMappingURL=FPEnum.js.map