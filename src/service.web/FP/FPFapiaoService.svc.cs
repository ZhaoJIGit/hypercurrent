using JieNor.Megi.BusinessContract.FP;
using JieNor.Megi.BusinessService.FP;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FP;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FP
{
	public class FPFapiaoService : ServiceT<FPFapiaoModel>, IFPFapiao
	{
		private readonly IFPFapiaoBusiness biz = new FPFapiaoBusiness();

		private readonly FPLogBusiness bizLog = new FPLogBusiness();

		public MActionResult<FPFapiaoModel> GetFapiaoModel(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetFapiaoModel, filter, accessToken);
		}

		public MActionResult<List<FPFapiaoModel>> GetFapiaoByIds(List<string> fapiaoIds, bool setDefault = true, string contactID = null, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetFapiaoByIds, fapiaoIds, setDefault, contactID, accessToken);
		}

		public MActionResult<List<FPFapiaoModel>> GetFapiaoListByFilter(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetFapiaoListByFilter, filter, accessToken);
		}

		public MActionResult<FPFapiaoModel> SaveFapiao(FPFapiaoModel fapiao, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.SaveFapiao, fapiao, null);
		}

		public MActionResult<OperationResult> DeleteFapiaoByFapiaoIds(FPFapiaoFilterModel model, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.DeleteFapiaoByFapiaoIds, model, accessToken);
		}

		public MActionResult<OperationResult> DeleteFPImportByIds(FPFapiaoFilterModel model, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.DeleteFPImportByIds, model, accessToken);
		}

		public MActionResult<OperationResult> BatchUpdateFPStatusByIds(FPFapiaoFilterModel model, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.BatchUpdateFPStatusByIds, model, accessToken);
		}

		public MActionResult<OperationResult> BatchUpdateFPVerifyType(List<FPFapiaoModel> modelList, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.BatchUpdateFPVerifyType, modelList, accessToken);
		}

		public MActionResult<DataGridJson<FPFapiaoReconcileModel>> GetReconcileList(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetReconcileList, filter, accessToken);
		}

		public MActionResult<DataGridJson<FPImpportModel>> GetStatementList(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetStatementList, filter, accessToken);
		}

		public MActionResult<DataGridJson<FPFapiaoModel>> GetTransactionList(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetTransactionList, filter, accessToken);
		}

		public MActionResult<OperationResult> SaveReconcile(FPFapiaoReconcileModel model, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.SaveReconcile, model, accessToken);
		}

		public MActionResult<OperationResult> SetReconcileStatus(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.SetReconcileStatus, filter, accessToken);
		}

		public MActionResult<OperationResult> RemoveReconcile(FPFapiaoReconcileModel model, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.RemoveReconcile, model, accessToken);
		}

		public MActionResult<DataGridJson<FPLogModel>> GetFapiaoLogList(FPFapiaoFilterModel filter, string accessToken = null)
		{
			return base.RunFunc(bizLog.GetFapiaoLogList, filter, accessToken);
		}

		public MActionResult<FPCodingPageModel> GetCodingPageList(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetCodingPageList, filter, accessToken);
		}

		public MActionResult<OperationResult> SaveCodingStatus(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.SaveCodingStatus, filter, accessToken);
		}

		public MActionResult<OperationResult> SaveCoding(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.SaveCoding, filter, accessToken);
		}

		public MActionResult<OperationResult> ResetCodingData(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.ResetCodingData, filter, accessToken);
		}

		public MActionResult<OperationResult> SaveCodingRow(List<FPCodingModel> rows, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.SaveCodingRow, rows, accessToken);
		}

		public MActionResult<OperationResult> DeleteCodingRow(FPCodingModel row, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.DeleteCodingRow, row, accessToken);
		}

		public MActionResult<FPCodingSettingModel> GetCodingSetting(string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetCodingSetting, accessToken);
		}

		public MActionResult<OperationResult> SaveCodingSetting(FPCodingSettingModel model, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.SaveCodingSetting, model, accessToken);
		}

		public MActionResult<FPBaseDataModel> GetBaseData(string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetBaseData, accessToken);
		}

		public MActionResult<List<FPFapiaoModel>> GetFapiaoListIncludeEntry(FPFapiaoFilterModel filter, string accessToken = null)
		{
			IFPFapiaoBusiness iFPFapiaoBusiness = biz;
			return base.RunFunc(iFPFapiaoBusiness.GetFapiaoListIncludeEntry, filter, accessToken);
		}
	}
}
