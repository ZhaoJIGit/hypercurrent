using JieNor.Megi.BusinessContract.SYS;
using JieNor.Megi.BusinessService.SYS;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.SYS;

namespace JieNor.Megi.Service.Web.SYS
{
	public class SYSCouponService : ServiceT<SYSCouponModel>, ISYSCoupon
	{
		private readonly ISYSCouponBusiness _coupon = new SYSCouponBusiness();

		public MActionResult<OperationResult> ApplyCoupon(string code, SYSCouponType type, string accessToken = null)
		{
			ISYSCouponBusiness coupon = _coupon;
			return base.RunFunc(coupon.ApplyCoupon, code, type, accessToken);
		}
	}
}
