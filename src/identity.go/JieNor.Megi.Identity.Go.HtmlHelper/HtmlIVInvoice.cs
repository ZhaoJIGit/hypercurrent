using JieNor.Megi.DataModel.IV;
using JieNor.Megi.Identity.Go.AutoManager;

namespace JieNor.Megi.Identity.Go.HtmlHelper
{
	public static class HtmlIVInvoice
	{
		public static IVInvoiceSummaryModel GetSummaryModel(string type)
		{
			return IVInvoiceManager.GetInvoiceSummaryModel(type);
		}
	}
}
