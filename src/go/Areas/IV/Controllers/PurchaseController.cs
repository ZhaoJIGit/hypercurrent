using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Identity;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.IV;
using System.Collections.Generic;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class PurchaseController : IVInvoiceBaseController
	{
		public PurchaseController(IIVInvoice invoice)
			: base(invoice)
		{
		}

		[Permission("Invoice_Purchases", "View", "")]
		public ActionResult Index(string pieListType)
		{
			base.SetTitle(LangHelper.GetText(LangModule.IV, "Purchases", "Purchases"));
			base.ViewData["TypeName"] = "Bills";
			base.ViewData["ListLink"] = "/IV/Bill/BillList";
			base.SetVDSummary("Invoice_Purchase");
			List<ChartPie2DModel> resultData = base._invoice.GetChartPieDictionary("'Invoice_Purchase','Invoice_Purchase_Red'", base.StartDate, base.EndDate, null).ResultData;
			base.ViewData["PieList"] = resultData;
			if (string.IsNullOrWhiteSpace(pieListType))
			{
				pieListType = "limit";
			}
			base.ViewData["pieListType"] = pieListType;
			return base.View();
		}
	}
}
