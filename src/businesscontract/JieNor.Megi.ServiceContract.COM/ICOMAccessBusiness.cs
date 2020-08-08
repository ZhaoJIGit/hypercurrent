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

		PlanModel GetPlan(MContext ctx);
	}
}
