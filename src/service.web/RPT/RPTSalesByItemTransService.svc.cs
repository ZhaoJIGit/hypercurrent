using JieNor.Megi.BusinessContract.RPT;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.RPT;

namespace JieNor.Megi.Service.Web.RPT
{
	public class RPTSalesByItemTransService : ServiceT<BaseModel>, IRPTSalesByItemTrans, IRPTBizReport<RPTSalesByItemTransFilterModel>
	{
		private readonly IRPTSalesByItemTransBusiness biz = new RPTSalesByItemTransBusiness();

		public MActionResult<string> GetBizReportJson(RPTSalesByItemTransFilterModel filter, string accessToken = null)
		{
			IRPTSalesByItemTransBusiness iRPTSalesByItemTransBusiness = biz;
			return base.RunFunc(iRPTSalesByItemTransBusiness.GetBizReportJson, filter, accessToken);
		}
	}
}
