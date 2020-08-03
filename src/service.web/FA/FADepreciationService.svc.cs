using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.BusinessService.FA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FA;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FA
{
	public class FADepreciationService : ServiceT<FADepreciationModel>, IFADepreciation
	{
		private readonly IFADepreciationBusiness biz = new FADepreciationBusiness();

		public MActionResult<DataGridJson<FADepreciationModel>> GetSummaryDepreciationPageList(FAFixAssetsFilterModel filter, string accessToken = null)
		{
			IFADepreciationBusiness iFADepreciationBusiness = biz;
			return base.RunFunc(iFADepreciationBusiness.GetSummaryDepreciationPageList, filter, accessToken);
		}

		public MActionResult<OperationResult> DepreciatePeriod(FAFixAssetsFilterModel filter, string accessToken = null)
		{
			IFADepreciationBusiness iFADepreciationBusiness = biz;
			return base.RunFunc(iFADepreciationBusiness.DepreciatePeriod, filter, accessToken);
		}

		public MActionResult<List<FADepreciationModel>> GetSummaryDepreciationList(FAFixAssetsFilterModel filter, string accessToken = null)
		{
			IFADepreciationBusiness iFADepreciationBusiness = biz;
			return base.RunFunc(iFADepreciationBusiness.GetSummaryDepreciationList, filter, accessToken);
		}

		public MActionResult<List<FADepreciationModel>> GetDetailDepreciationList(FAFixAssetsFilterModel filter, string accessToken = null)
		{
			IFADepreciationBusiness iFADepreciationBusiness = biz;
			return base.RunFunc(iFADepreciationBusiness.GetDetailDepreciationList, filter, accessToken);
		}

		public MActionResult<OperationResult> SaveDepreciationList(FAFixAssetsFilterModel filter, string accessToken = null)
		{
			IFADepreciationBusiness iFADepreciationBusiness = biz;
			return base.RunFunc(iFADepreciationBusiness.SaveDepreciationList, filter, accessToken);
		}
	}
}
