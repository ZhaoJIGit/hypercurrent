using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.PA;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.PA.Controllers
{
	public class PayrollBasicController : GoControllerBase
	{
		private IPAPayrollBasic _payrollBasic = null;

		private IBDAttachment _attachment = null;

		public PayrollBasicController(IPAPayrollBasic payrollBasic, IBDAttachment attachment)
		{
			_payrollBasic = payrollBasic;
			_attachment = attachment;
		}

		public ActionResult Index()
		{
			PAPaySettingModel resultData = _payrollBasic.GetPaySetting(null).ResultData;
			base.ViewData["PAPaySettingModel"] = resultData;
			return base.View();
		}

		public ActionResult GetPaySettingModel()
		{
			MActionResult<PAPaySettingModel> paySetting = _payrollBasic.GetPaySetting(null);
			return base.Json(paySetting);
		}

		public ActionResult UpdatePaySetting(PAPaySettingModel model)
		{
			return base.Json(_payrollBasic.UpdatePaySetting(model, null));
		}
	}
}
