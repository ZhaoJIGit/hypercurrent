using JieNor.Megi.Common.Context;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Controllers;
using JieNor.Megi.My.Web.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using System.Web.Mvc;

namespace JieNor.Megi.My.Web.Areas.Org.Controllers
{
	public class OrganisationController : MyControllerBase
	{
		private IBASMyHome _org;

		private IBASOrganisation _organisation;

		public OrganisationController(IBASMyHome org, IBASOrganisation organisation)
		{
			_org = org;
			_organisation = organisation;
		}

		public ActionResult OrganisationAdd()
		{
			base.SetTitle(LangHelper.GetText(LangModule.My, "NewOrganisation", "New Organisation"));
			return new MegiActionResult(base.ViewData, base.TempData);
		}

		public ActionResult Guide()
		{
			base.SetTitle(LangHelper.GetText(LangModule.My, "NewOrganisation", "New Organisation"));
			return base.View();
		}

		[HttpPost]
		public ActionResult OrgRegister(BASOrganisationModel model)
		{
			MContext mContext = ContextHelper.MContext;
			mContext.MMaster = mContext.MUserID;
			mContext.MUsedStatusID = "00000001-0004-0001-0000-000000000000";
			mContext.MAppID = "1";
			MActionResult<OperationResult> data = _org.OrgRegisterForTry(model, null);
			ContextHelper.MContext = mContext;
			return base.Json(data);
		}

		public JsonResult CreateDemoCompany()
		{
			MActionResult<OperationResult> data = _organisation.CreateDemoCompany(null);
			return base.Json(data);
		}
	}
}
