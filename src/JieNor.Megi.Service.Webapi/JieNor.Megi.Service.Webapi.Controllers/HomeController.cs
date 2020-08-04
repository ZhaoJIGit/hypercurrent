using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.SEC;
using JieNor.Megi.ServiceContract.SEC;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace JieNor.Megi.Service.Webapi.Controllers
{
	public class HomeController : ApiController
	{
		private ISECUserAccount appUI;

		public HomeController()
		{
			appUI = ServiceHostManager.GetSysService<ISECUserAccount>();
		}

		public HomeController(ISECUserAccount appUI)
		{
			this.appUI = appUI;
		}

		public HttpResponseMessage GetDataModel(string accessToken = "123456")
		{
			SECLoginModel sECLoginModel = new SECLoginModel();
			sECLoginModel.Email = "591387160@qq.com";
			SECLoginResultModel resultDatum = appUI.Login(sECLoginModel, null).ResultData;
			return new HttpResponseMessage
			{
				Content = new StringContent("hahha", Encoding.GetEncoding("UTF-8"), "application/json")
			};
		}
	}
}
