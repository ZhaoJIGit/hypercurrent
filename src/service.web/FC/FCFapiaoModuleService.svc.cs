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
	public class FCFapiaoModuleService : ServiceT<FCFapiaoModuleModel>, IFCFapiaoModule
	{
		private readonly IFCFapiaoModuleBusiness biz = new FCFapiaoModuleBusiness();

		public MActionResult<OperationResult> SaveFapiaoModule(FCFapiaoModuleModel model, string accessToken = null)
		{
			IFCFapiaoModuleBusiness iFCFapiaoModuleBusiness = biz;
			return base.RunFunc(iFCFapiaoModuleBusiness.SaveFapiaoModule, model, accessToken);
		}

		public MActionResult<OperationResult> DeleteFapiaoModules(List<string> ids, string accessToken = null)
		{
			IFCFapiaoModuleBusiness iFCFapiaoModuleBusiness = biz;
			return base.RunFunc(iFCFapiaoModuleBusiness.DeleteFapiaoModules, ids, accessToken);
		}

		public MActionResult<DataGridJson<FCFapiaoModuleModel>> GetFapiaoModulePageList(FCFastCodeFilterModel filter, string accessToken = null)
		{
			IFCFapiaoModuleBusiness iFCFapiaoModuleBusiness = biz;
			return base.RunFunc(iFCFapiaoModuleBusiness.GetFapiaoModulePageList, filter, accessToken);
		}

		public MActionResult<List<FCFapiaoModuleModel>> GetFapiaoModuleList(FCFastCodeFilterModel filter, string accessToken = null)
		{
			IFCFapiaoModuleBusiness iFCFapiaoModuleBusiness = biz;
			return base.RunFunc(iFCFapiaoModuleBusiness.GetFapiaoModuleList, filter, accessToken);
		}

		public MActionResult<int> GetFapiaoModulePageListCount(string accessToken = null)
		{
			IFCFapiaoModuleBusiness iFCFapiaoModuleBusiness = biz;
			return base.RunFunc(iFCFapiaoModuleBusiness.GetFapiaoModulePageListCount, accessToken);
		}
	}
}
