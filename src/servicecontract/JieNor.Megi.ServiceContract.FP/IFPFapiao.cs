using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FP
{
	[ServiceContract]
	public interface IFPFapiao
	{
		[OperationContract]
		MActionResult<OperationResult> SetReconcileStatus(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<FPFapiaoModel> GetFapiaoModel(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<FPFapiaoModel>> GetFapiaoByIds(List<string> fapiaoIds, bool setDefault = true, string contactID = null, string accessToken = null);

		[OperationContract]
		MActionResult<List<FPFapiaoModel>> GetFapiaoListByFilter(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<FPFapiaoModel> SaveFapiao(FPFapiaoModel fapiao, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteFapiaoByFapiaoIds(FPFapiaoFilterModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteFPImportByIds(FPFapiaoFilterModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> BatchUpdateFPStatusByIds(FPFapiaoFilterModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> BatchUpdateFPVerifyType(List<FPFapiaoModel> modelList, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FPFapiaoReconcileModel>> GetReconcileList(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FPImpportModel>> GetStatementList(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FPFapiaoModel>> GetTransactionList(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveReconcile(FPFapiaoReconcileModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> RemoveReconcile(FPFapiaoReconcileModel model, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FPLogModel>> GetFapiaoLogList(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<FPCodingPageModel> GetCodingPageList(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveCodingStatus(FPFapiaoFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveCoding(FPFapiaoFilterModel fitler, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ResetCodingData(FPFapiaoFilterModel fitler, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveCodingRow(List<FPCodingModel> rows, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteCodingRow(FPCodingModel row, string accessToken = null);

		[OperationContract]
		MActionResult<FPCodingSettingModel> GetCodingSetting(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveCodingSetting(FPCodingSettingModel model, string accessToken = null);

		[OperationContract]
		MActionResult<FPBaseDataModel> GetBaseData(string accessToken = null);

		[OperationContract]
		MActionResult<List<FPFapiaoModel>> GetFapiaoListIncludeEntry(FPFapiaoFilterModel filter, string accessToken = null);
	}
}
