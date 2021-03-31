using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.COM
{
	[ServiceContract]
	public interface ICOMAccessBusiness
	{
		MAccessResponseModel GetAccessResult(MContext ctx, List<MAccessRequestModel> requestList = null);

		MAccessResponseModel GetAccessResultByListNoByCacche(MContext ctx, bool isGetCache, List<MAccessRequestModel> requestList = null);

		List<PlanModel> GetPlan(MContext ctx);
		List<PlanModel> GetPlanByEmail(MContext ctx,string email);

	}
}
