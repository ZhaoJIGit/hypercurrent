using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.SYS;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Sys.Controllers
{
	public class CouponController : GoControllerBase
	{
		private ISYSCoupon _coupon = null;

		public CouponController(ISYSCoupon coupon)
		{
			_coupon = coupon;
		}

		public JsonResult ApplyCoupon(string code)
		{
			MActionResult<OperationResult> data = _coupon.ApplyCoupon(code, SYSCouponType.Trial, null);
			return base.Json(data);
		}
	}
}
