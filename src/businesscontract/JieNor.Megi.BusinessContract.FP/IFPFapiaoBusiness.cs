using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FP
{
	public interface IFPFapiaoBusiness : IDataContract<FPFapiaoModel>
	{
		OperationResult SetReconcileStatus(MContext ctx, FPFapiaoFilterModel filter);

		FPFapiaoModel GetFapiaoModel(MContext ctx, FPFapiaoFilterModel filter);

		List<FPFapiaoModel> GetFapiaoByIds(MContext ctx, List<string> fapiaoIds, bool setDefault = true, string contactID = null);

		List<FPFapiaoModel> GetFapiaoListByFilter(MContext ctx, FPFapiaoFilterModel filter);

		FPFapiaoModel SaveFapiao(MContext ctx, FPFapiaoModel fapiao);

		OperationResult DeleteFapiaoByFapiaoIds(MContext ctx, FPFapiaoFilterModel model);

		OperationResult DeleteFPImportByIds(MContext ctx, FPFapiaoFilterModel model);

		OperationResult BatchUpdateFPStatusByIds(MContext ctx, FPFapiaoFilterModel model);

		OperationResult BatchUpdateFPVerifyType(MContext ctx, List<FPFapiaoModel> modelList);

		DataGridJson<FPFapiaoReconcileModel> GetReconcileList(MContext ctx, FPFapiaoFilterModel filter);

		DataGridJson<FPImpportModel> GetStatementList(MContext ctx, FPFapiaoFilterModel filter);

		DataGridJson<FPFapiaoModel> GetTransactionList(MContext ctx, FPFapiaoFilterModel filter);

		OperationResult SaveReconcile(MContext ctx, FPFapiaoReconcileModel model);

		OperationResult RemoveReconcile(MContext ctx, FPFapiaoReconcileModel model);

		DataGridJson<FPLogModel> GetFapiaoLogList(MContext ctx, FPFapiaoFilterModel filter);

		FPCodingPageModel GetCodingPageList(MContext ctx, FPFapiaoFilterModel filter);

		OperationResult SaveCodingStatus(MContext ctx, FPFapiaoFilterModel filter);

		OperationResult SaveCoding(MContext ctx, FPFapiaoFilterModel fitler);

		OperationResult ResetCodingData(MContext ctx, FPFapiaoFilterModel fitler);

		OperationResult SaveCodingRow(MContext ctx, List<FPCodingModel> rows);

		OperationResult DeleteCodingRow(MContext ctx, FPCodingModel row);

		FPCodingSettingModel GetCodingSetting(MContext ctx);

		OperationResult SaveCodingSetting(MContext ctx, FPCodingSettingModel model);

		FPBaseDataModel GetBaseData(MContext ctx);

		List<FPFapiaoModel> GetFapiaoListIncludeEntry(MContext ctx, FPFapiaoFilterModel filter);
	}
}
