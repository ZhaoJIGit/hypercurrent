using JieNor.Megi.Service.Webapi.Helper;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class SystemController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage GetExcelPlusVersion()
		{
			string excelVersion = ConfigurationManager.AppSettings["ExcelPlusVersion"];
			string text = ConfigurationManager.AppSettings["ExcelPlusUpdateLog"];
			StringBuilder stringBuilder = new StringBuilder();
			if (text != null)
			{
				string[] array = text.Split('|');
				foreach (string value in array)
				{
					stringBuilder.AppendLine(value);
				}
			}
			return ResponseHelper.toJson(new
			{
				ExcelVersion = excelVersion,
				UpdateLog = stringBuilder.ToString()
			}, true, null, false);
		}

		[HttpGet]
		public HttpResponseMessage GetAppVersion(string appName)
		{
			string mVersion = ConfigurationManager.AppSettings[$"{appName}Version"];
			string text = ConfigurationManager.AppSettings[$"{appName}UpdateLog"];
			StringBuilder stringBuilder = new StringBuilder();
			if (text != null)
			{
				string[] array = text.Split('|');
				foreach (string value in array)
				{
					stringBuilder.AppendLine(value);
				}
			}
			string content = new JavaScriptSerializer().Serialize(new
			{
				MVersion = mVersion,
				MUpdateLog = stringBuilder.ToString()
			});
			return new HttpResponseMessage
			{
				Content = new StringContent(content)
			};
		}
	}
}
