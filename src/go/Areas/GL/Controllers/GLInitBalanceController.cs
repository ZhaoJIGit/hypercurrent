using JieNor.Megi.Core.Context;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.GL;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.GL.Controllers
{
	public class GLInitBalanceController : GoControllerBase
	{
		private IGLInitBalance GLInitBalanceService = null;

		public GLInitBalanceController(IGLInitBalance glInitBalance)
		{
			GLInitBalanceService = glInitBalance;
		}

		public ActionResult ClearInitBalance()
		{
			MActionResult<OperationResult> data = GLInitBalanceService.ClearInitBalance(null, null);
			return base.Json(data);
		}

		public ActionResult CheckAutoCreateBillHadVerifiyRecord()
		{
			MActionResult<OperationResult> data = GLInitBalanceService.CheckAutoCreateBillHadVerifiyRecord(null);
			return base.Json(data);
		}
	}
}
