using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessContract.RI;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.RI;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD.RI;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RI;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.RI
{
	public class RIInspectService : ServiceT<RICategoryModel>, IRIInspect
	{
		private readonly IRIInspectBusiness biz = new RIInspectBusiness();

		private readonly IBDInspect bdBiz = new BDInspectBusiness();

		public MActionResult<RIInspectionResult> Inspect(string settingID, int year, int period, string accessToken = null)
		{
			IRIInspectBusiness iRIInspectBusiness = biz;
			return base.RunFunc(iRIInspectBusiness.Inspect, settingID, year, period, accessToken);
		}

		public MActionResult<List<RICategoryModel>> GetCategoryList(bool includeSettingDisable = true, int year = 0, int period = 0, string accessToken = null)
		{
			IRIInspectBusiness iRIInspectBusiness = biz;
			return base.RunFunc(iRIInspectBusiness.GetCategoryList, includeSettingDisable, year, period, accessToken);
		}

		public MActionResult<OperationResult> ClearDataPool(string accessToken = null)
		{
			IRIInspectBusiness iRIInspectBusiness = biz;
			return base.RunFunc(iRIInspectBusiness.ClearDataPool, accessToken);
		}

		public MActionResult<List<BDInspectItemTreeModel>> GetInspectItemTreeList(BDInspectItemFilterModel filter, string accessToken = null)
		{
			IBDInspect iBDInspect = bdBiz;
			return base.RunFunc(iBDInspect.GetInspectItemTreeList, filter, accessToken);
		}

		public MActionResult<OperationResult> InitInspectItem(string accessToken = null)
		{
			IBDInspect iBDInspect = bdBiz;
			return base.RunFunc(iBDInspect.InitInspectItem, accessToken);
		}

		public MActionResult<OperationResult> SaveInspectSetting(List<RICategorySettingModel> inspectSetingItemList, string accessToken = null)
		{
			IBDInspect iBDInspect = bdBiz;
			return base.RunFunc(iBDInspect.SaveInspectSetting, inspectSetingItemList, accessToken);
		}
	}
}
