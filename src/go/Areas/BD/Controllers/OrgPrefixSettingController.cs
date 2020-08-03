using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class OrgPrefixSettingController : GoControllerBase
	{
		private IBASOrgPrefixSetting orgPrefixSetting = null;

		public OrgPrefixSettingController(IBASOrgPrefixSetting _orgPrefixSetting)
		{
			orgPrefixSetting = _orgPrefixSetting;
		}

		public ActionResult GetPrefixSetting(string module)
		{
			return base.Json(orgPrefixSetting.GetOrgPrefixSettingModel(module, null));
		}

		public ActionResult SavePrefixSetting(BASOrgPrefixSettingModel model)
		{
			return base.Json(orgPrefixSetting.UpdateOrgPrefixSettingModel(model, null));
		}
	}
}
