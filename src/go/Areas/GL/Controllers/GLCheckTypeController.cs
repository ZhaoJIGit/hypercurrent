using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.GL.Controllers
{
	public class GLCheckTypeController : GoControllerBase
	{
		private IGLCheckType checkType = null;

		public GLCheckTypeController(IGLCheckType _checkType)
		{
			checkType = _checkType;
		}

		public ActionResult GetCheckTypeDataByType(int type)
		{
			MActionResult<GLCheckTypeDataModel> checkTypeDataByType = checkType.GetCheckTypeDataByType(type, true, null);
			return base.Json(checkTypeDataByType);
		}

		[Permission("Setting", "View", "")]
		public ActionResult GetCheckTreeTypeDataByType(int type, bool includeDisable = true)
		{
			MActionResult<GLCheckTypeDataModel> checkTypeDataByType = checkType.GetCheckTypeDataByType(type, includeDisable, null);
			List<GLTreeModel> data = new List<GLTreeModel>();
			if (checkTypeDataByType != null && checkTypeDataByType.ResultData != null && checkTypeDataByType.ResultData.MDataList != null)
			{
				data = checkTypeDataByType.ResultData.MDataList;
			}
			return base.Json(data);
		}
	}
}
