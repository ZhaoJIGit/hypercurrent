using JieNor.Megi.BusinessContract.FC;
using JieNor.Megi.BusinessService.FC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FC;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FC
{
	public class FCCashCodingModuleService : ServiceT<FCCashCodingModuleModel>, IFCCashCodingModule
	{
		private readonly IFCCashCodingModuleBusiness biz = new FCCashCodingModuleBusiness();

		public MActionResult<List<FCCashCodingModuleListModel>> GetListByCode(string code, string accessToken = null)
		{
			IFCCashCodingModuleBusiness iFCCashCodingModuleBusiness = biz;
			return base.RunFunc(iFCCashCodingModuleBusiness.GetListByCode, code, accessToken);
		}

		public MActionResult<OperationResult> UpdateCashCodingModuleModel(FCCashCodingModuleModel model, string accessToken = null)
		{
			IFCCashCodingModuleBusiness iFCCashCodingModuleBusiness = biz;
			return base.RunFunc(iFCCashCodingModuleBusiness.UpdateCashCodingModuleModel, model, accessToken);
		}

		public MActionResult<DataGridJson<FCCashCodingModuleListModel>> GetCashCodingByPageList(FCCashCodingModuleListFilter model, string accessToken = null)
		{
			IFCCashCodingModuleBusiness iFCCashCodingModuleBusiness = biz;
			return base.RunFunc(iFCCashCodingModuleBusiness.GetCashCodingByPageList, model, accessToken);
		}

		public MActionResult<List<FCCashCodingModuleModel>> GetCashCodingModuleListWithNoEntry(string accessToken = null)
		{
			IFCCashCodingModuleBusiness iFCCashCodingModuleBusiness = biz;
			return base.RunFunc(iFCCashCodingModuleBusiness.GetCashCodingModuleListWithNoEntry, accessToken);
		}

		public MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null)
		{
			IFCCashCodingModuleBusiness iFCCashCodingModuleBusiness = biz;
			return base.DeleteModels(iFCCashCodingModuleBusiness.DeleteModels, pkID, accessToken);
		}

		public MActionResult<FCCashCodingModuleModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IFCCashCodingModuleBusiness iFCCashCodingModuleBusiness = biz;
			return base.GetDataModel(iFCCashCodingModuleBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}
	}
}
