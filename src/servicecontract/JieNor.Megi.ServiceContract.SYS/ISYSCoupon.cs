using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.SYS
{
	[ServiceContract]
	public interface ISYSCoupon
	{
		[OperationContract]
		MActionResult<OperationResult> ApplyCoupon(string code, SYSCouponType type, string accessToken = null);
	}
}
