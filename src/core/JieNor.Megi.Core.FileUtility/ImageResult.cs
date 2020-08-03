using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.Core.FileUtility
{
	public class ImageResult : ActionResult
	{
		public ImageResult(Stream imageStream, string contentType)
		{
			bool flag = imageStream == null;
			if (flag)
			{
				throw new ArgumentNullException("imageStream");
			}
			bool flag2 = contentType == null;
			if (flag2)
			{
				throw new ArgumentNullException("contentType");
			}
			this.ImageStream = imageStream;
			this.ContentType = contentType;
		}

		public Stream ImageStream { get; private set; }

		public string ContentType { get; private set; }

		public override void ExecuteResult(ControllerContext context)
		{
			bool flag = context == null;
			if (flag)
			{
				throw new ArgumentNullException("context");
			}
			HttpResponseBase response = context.HttpContext.Response;
			response.ContentType = this.ContentType;
			byte[] array = new byte[4096];
			for (; ; )
			{
				int num = this.ImageStream.Read(array, 0, array.Length);
				bool flag2 = num == 0;
				if (flag2)
				{
					break;
				}
				response.OutputStream.Write(array, 0, num);
			}
			response.End();
		}
	}
}
