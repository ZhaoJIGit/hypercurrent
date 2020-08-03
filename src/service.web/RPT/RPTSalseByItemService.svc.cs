using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTSalseByItemService : ServiceT<BaseModel>, IRPTSalseByItem, IRPTBizReport<RPTSalseByItemFilterModel>
	{
		private readonly IRPTSalseByItemBusiness biz = new RPTSalseByItemBusiness();

		public MActionResult<string> GetBizReportJson(RPTSalseByItemFilterModel filter, string accessToken = null)
		{
			IRPTSalseByItemBusiness iRPTSalseByItemBusiness = biz;
			return base.RunFunc(iRPTSalseByItemBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
