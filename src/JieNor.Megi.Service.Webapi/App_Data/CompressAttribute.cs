using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Web.Http.Filters;

namespace JieNor.Megi.Service.Webapi
{
	public class CompressAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			HttpContent content = actionExecutedContext.Response.Content;
			byte[] array = (content == null) ? null : content.ReadAsByteArrayAsync().Result;
			byte[] content2 = (array == null) ? new byte[0] : GetGZIPByte(array);
			actionExecutedContext.Response.Content = new ByteArrayContent(content2);
			actionExecutedContext.Response.Content.Headers.Add("Content-encoding", "gzip");
			actionExecutedContext.Response.Content.Headers.Add("Content-Type", "application/json");
			base.OnActionExecuted(actionExecutedContext);
		}

		public byte[] GetGZIPByte(byte[] unZipBytes)
		{
			if (unZipBytes == null)
			{
				return null;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gZipStream.Write(unZipBytes, 0, unZipBytes.Length);
				}
				return memoryStream.ToArray();
			}
		}
	}
}
