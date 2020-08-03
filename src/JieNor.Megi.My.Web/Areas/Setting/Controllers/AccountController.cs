using JieNor.Megi.Common.Context;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.My.Web.Controllers;
using JieNor.Megi.ServiceContract.SEC;
using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.Setting.Controllers
{
	public class AccountController : MyControllerBase
	{
		private ISECAccount _acct;

		public AccountController(ISECAccount acct)
		{
			_acct = acct;
		}

		public ActionResult AccountEdit()
		{
			base.ViewData["secUserModel"] = _acct.GetUserModelByKey(ContextHelper.MContext.MUserID, null).ResultData;
			return base.View();
		}

		public JsonResult AcctUpdateEmail(SECUserModel info)
		{
			info.MItemID = ContextHelper.MContext.MUserID;
			return base.Json(_acct.UpdateAccountData(info, 1, null));
		}

		public JsonResult AcctUpdatePass(SECUserModel info)
		{
			info.MItemID = ContextHelper.MContext.MUserID;
			return base.Json(_acct.UpdateAccountData(info, 2, null));
		}

		public JsonResult AcctUpdateOther(SECUserModel info)
		{
			info.MItemID = ContextHelper.MContext.MUserID;
			return base.Json(_acct.UpdateAccountData(info, 3, null));
		}
	}
}
