using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.SEC;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class InitSettingController : GoControllerBase
	{
		private IBASOrgInitSetting _initSetting = null;

		private ISECPermission _perm = null;

		public InitSettingController(IBASOrgInitSetting initSetting, ISECPermission perm)
		{
			_initSetting = initSetting;
			_perm = perm;
		}

		public JsonResult GLSetupSuccess()
		{
			return base.Json(_initSetting.GLSetupSuccess(null));
		}
	}
}
