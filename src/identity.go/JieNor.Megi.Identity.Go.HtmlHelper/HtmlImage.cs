using JieNor.Megi.Core.FileUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.Identity.Go.AutoManager;
using System;
using System.IO;
using System.Web.Mvc;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public class HtmlImage
	{
		public static MvcHtmlString Show(string id, string attr = null)
		{
			Stream stream = null;
			string empty = string.Empty;
			string format = "data:image/jpg;base64,{0}";
			BDAttachmentModel attachmentModel = BDAttachmentManager.GetAttachmentModel(id);
			try
			{
				stream = FtpHelper.GetDownloadStream(attachmentModel.MPath, attachmentModel.MUploadName);
			}
			catch (Exception)
			{
			}
			if (stream != null)
			{
				MemoryStream memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				empty = string.Format(format, Convert.ToBase64String(memoryStream.ToArray()));
			}
			else
			{
				empty = string.Format(format, string.Empty);
			}
			return new MvcHtmlString($"<img {attr} src='{empty}' />");
		}
	}
}
