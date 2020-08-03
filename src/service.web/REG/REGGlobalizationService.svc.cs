using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.REG;

namespace JieNor.Megi.Service.Web.REG
{
	public class REGGlobalizationService : ServiceT<REGGlobalizationModel>, IREGGlobalization
	{
		private readonly IREGGlobalizationBusiness biz = new REGGlobalizationBusiness();

		public MActionResult<OperationResult> GlobalizationUpdate(REGGlobalizationModel model, string accessToken = null)
		{
			IREGGlobalizationBusiness iREGGlobalizationBusiness = biz;
			return base.RunFunc(iREGGlobalizationBusiness.GlobalizationUpdate, model, accessToken);
		}

		public MActionResult<REGGlobalizationModel> GetOrgGlobalizationDetail(string orgid, string accessToken = null)
		{
			IREGGlobalizationBusiness iREGGlobalizationBusiness = biz;
			return base.RunFunc(iREGGlobalizationBusiness.GetOrgGlobalizationDetail, orgid, accessToken);
		}
	}
}
