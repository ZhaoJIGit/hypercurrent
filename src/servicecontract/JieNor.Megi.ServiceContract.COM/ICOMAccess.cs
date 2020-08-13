using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.COM
{
	[ServiceContract]
	public interface ICOMAccess
	{
		[OperationContract]
		MActionResult<MAccessResponseModel> GetAccessResultByList(List<MAccessRequestModel> requestList = null, string accessToken = null);

		[OperationContract]
		MActionResult<MAccessResponseModel> GetAccessResultByListNoByCacche(bool isGetCache, List<MAccessRequestModel> requestList = null, string accessToken = null);

		[OperationContract]
		MActionResult<List<PlanModel>> GetPlan();
	}
}
