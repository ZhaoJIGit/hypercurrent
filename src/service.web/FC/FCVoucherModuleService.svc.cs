using JieNor.Megi.BusinessContract.FC;
using JieNor.Megi.BusinessService.FC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FC;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FC
{
	public class FCVoucherModuleService : ServiceT<FCVoucherModuleModel>, IFCVoucherModule
	{
		private readonly IFCVoucherModuleBusiness biz = new FCVoucherModuleBusiness();

		public MActionResult<List<FCVoucherModuleModel>> GetVoucherModuleListWithNoEntry(string accessToken = null)
		{
			IFCVoucherModuleBusiness iFCVoucherModuleBusiness = biz;
			return base.RunFunc(iFCVoucherModuleBusiness.GetVoucherModuleListWithNoEntry, accessToken);
		}

		public MActionResult<FCVoucherModuleModel> GetVoucherModelWithEntry(string PKID, string accessToken = null)
		{
			IFCVoucherModuleBusiness iFCVoucherModuleBusiness = biz;
			return base.RunFunc(iFCVoucherModuleBusiness.GetVoucherModelWithEntry, PKID, accessToken);
		}

		public MActionResult<List<FCVoucherModuleModel>> GetVoucherModuleList(List<string> pkIDS, string accessToken = null)
		{
			IFCVoucherModuleBusiness iFCVoucherModuleBusiness = biz;
			return base.RunFunc(iFCVoucherModuleBusiness.GetVoucherModuleList, pkIDS, accessToken);
		}

		public MActionResult<DataGridJson<FCVoucherModuleModel>> GetVoucherModuleModelPageList(GLVoucherListFilterModel filter, string accessToken = null)
		{
			IFCVoucherModuleBusiness iFCVoucherModuleBusiness = biz;
			return base.RunFunc(iFCVoucherModuleBusiness.GetVoucherModuleModelPageList, filter, accessToken);
		}

		public MActionResult<FCVoucherModuleModel> UpdateVoucherModuleModel(FCVoucherModuleModel model, string accessToken = null)
		{
			IFCVoucherModuleBusiness iFCVoucherModuleBusiness = biz;
			return base.RunFunc(iFCVoucherModuleBusiness.UpdateVoucherModuleModel, model, accessToken);
		}

		public MActionResult<FCVoucherModuleModel> GetVoucherModule(string pkID = null, string accessToken = null)
		{
			IFCVoucherModuleBusiness iFCVoucherModuleBusiness = biz;
			return base.RunFunc(iFCVoucherModuleBusiness.GetVoucherModule, pkID, accessToken);
		}

		public MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null)
		{
			IFCVoucherModuleBusiness iFCVoucherModuleBusiness = biz;
			return base.DeleteModels(iFCVoucherModuleBusiness.DeleteModels, pkID, accessToken);
		}
	}
}
