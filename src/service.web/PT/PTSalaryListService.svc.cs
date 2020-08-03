using JieNor.Megi.BusinessContract.PT;
using JieNor.Megi.BusinessService.PT;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.PT;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.PT
{
	public class PTSalaryListService : ServiceT<PAPrintSettingModel>, IPTSalaryList
	{
		private readonly IPTSalaryListBusiness biz = new PTSalaryListBusiness();

		public MActionResult<List<PAPrintSettingModel>> GetList(string accessToken = null)
		{
			IPTSalaryListBusiness iPTSalaryListBusiness = biz;
			return base.RunFunc(iPTSalaryListBusiness.GetList, accessToken);
		}

		public MActionResult<OperationResult> Copy(PAPrintSettingModel model, bool isCopyTmpl = false, string accessToken = null)
		{
			IPTSalaryListBusiness iPTSalaryListBusiness = biz;
			return base.RunFunc(iPTSalaryListBusiness.Copy, model, isCopyTmpl, accessToken);
		}

		public MActionResult<PAPrintSettingModel> GetPrintSetting(string itemID, bool isFromPrint = false, string accessToken = null)
		{
			IPTSalaryListBusiness iPTSalaryListBusiness = biz;
			return base.RunFunc(iPTSalaryListBusiness.GetPrintSetting, itemID, isFromPrint, accessToken);
		}

		public MActionResult<OperationResult> Sort(string ids, string accessToken = null)
		{
			IPTSalaryListBusiness iPTSalaryListBusiness = biz;
			return base.RunFunc(iPTSalaryListBusiness.Sort, ids, accessToken);
		}

		public MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null)
		{
			IPTSalaryListBusiness iPTSalaryListBusiness = biz;
			return base.DeleteModels(iPTSalaryListBusiness.DeleteModels, pkID, accessToken);
		}

		public MActionResult<OperationResult> InsertOrUpdate(PAPrintSettingModel modelData, string fields = null, string accessToken = null)
		{
			IPTSalaryListBusiness iPTSalaryListBusiness = biz;
			return base.InsertOrUpdate(iPTSalaryListBusiness.InsertOrUpdate, modelData, fields, accessToken);
		}

		public MActionResult<PAPrintSettingModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IPTSalaryListBusiness iPTSalaryListBusiness = biz;
			return base.GetDataModel(iPTSalaryListBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}
	}
}
