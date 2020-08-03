using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace JieNor.Megi.EntityModel.Context
{
	public class CustomJson : JsonResult
	{
		public static int GZipLength
		{
			get
			{
				string text = ConfigurationManager.AppSettings["GZipLength"];
				int num;
				return (string.IsNullOrWhiteSpace(text) || !int.TryParse(text, out num)) ? 102400 : int.Parse(text);
			}
			set
			{
			}
		}

		public override void ExecuteResult(ControllerContext context)
		{
			HttpResponseBase response = context.HttpContext.Response;
			response.ContentType = ((!string.IsNullOrEmpty(base.ContentType)) ? base.ContentType : "application/json");
			if (base.ContentEncoding != null)
			{
				response.ContentEncoding = base.ContentEncoding;
			}
			if (base.Data != null)
			{
				IsoDateTimeConverter isoDateTimeConverter = new IsoDateTimeConverter
				{
					DateTimeFormat = "yyyy-MM-dd HH:mm:ss"
				};
				string text = JsonConvert.SerializeObject(base.Data, Formatting.None, isoDateTimeConverter);
				response.Write(text);
				if (text.Length > GZipLength)
				{
					string text2 = context.HttpContext.Request.Headers["Accept-Encoding"];
					if (!string.IsNullOrEmpty(text2))
					{
						text2 = text2.ToUpperInvariant();
						if (text2.Contains("GZIP"))
						{
							response.AppendHeader("Content-Encoding", "gzip");
							response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
						}
						else if (text2.Contains("DEFLATE"))
						{
							response.AppendHeader("Content-Encoding", "deflate");
							response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
						}
					}
				}
			}
			else
			{
				base.ExecuteResult(context);
			}
		}
	}
}
