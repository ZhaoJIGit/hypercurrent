using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class FileController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage GetExcelPlusApp()
		{
			string text = AppDomain.CurrentDomain.BaseDirectory + "ExcelPlus\\hypercurrent.exe";
			FileStream content = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.Read);
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
			httpResponseMessage.Content = new StreamContent(content);
			httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
			httpResponseMessage.Content.Headers.ContentDisposition.FileName = "hypercurrent.exe";
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
			httpResponseMessage.Content.Headers.ContentLength = new FileInfo(text).Length;
			return httpResponseMessage;
		}

		[HttpGet]
		public HttpResponseMessage GetApp(string appName)
		{
			string text = AppDomain.CurrentDomain.BaseDirectory + $"Apps\\{appName}.exe";
			FileStream content = new FileStream(text, FileMode.Open, FileAccess.Read, FileShare.Read);
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
			httpResponseMessage.Content = new StreamContent(content);
			httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
			httpResponseMessage.Content.Headers.ContentDisposition.FileName = Path.GetFileName(text);
			httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
			httpResponseMessage.Content.Headers.ContentLength = new FileInfo(text).Length;
			return httpResponseMessage;
		}
	}
}
