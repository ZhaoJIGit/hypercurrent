using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;

namespace JieNor.Megi.BusinessContract.SYS
{
	public interface ISYSCouponBusiness : IDataContract<SYSCouponModel>
	{
		OperationResult ApplyCoupon(MContext ctx, string code, SYSCouponType type);
	}
}
