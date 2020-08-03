using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.ServiceContract.IV;
using System;

namespace JieNor.Megi.Identity.Go.AutoManager
{
	public class IVInvoiceManager
	{
		public static IVInvoiceSummaryModel GetInvoiceSummaryModel(string type)
		{
			DateTime startDate = DateTime.Now.AddYears(-15);
			DateTime now = DateTime.Now;
			IIVInvoice sysService = ServiceHostManager.GetSysService<IIVInvoice>();
			using (sysService as IDisposable)
			{
				return sysService.GetInvoiceSummaryModel(type, startDate, now, null).ResultData;
			}
		}
	}
}
