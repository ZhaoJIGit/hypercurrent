using JieNor.Megi.Common.Context;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Sys.Controllers
{
	public class FileDownloadController : GoControllerBase
	{
		public ActionResult SystemUpdateInfo(string fileName, string fileType)
		{
			MContext mContext = ContextHelper.MContext;
			string text = string.IsNullOrEmpty(mContext.MLCID) ? "0x0009" : mContext.MLCID;
			fileName = (string.IsNullOrWhiteSpace(fileName) ? "systemupdateinfo.pdf" : fileName);
			string fileName2 = base.Server.MapPath("~/Template/Sys") + "\\" + text + "\\" + fileName;
			string contentType = string.IsNullOrWhiteSpace(fileType) ? "application/pdf" : ("application/" + fileType);
			return File(fileName2, contentType, fileName);
		}
	}
}
