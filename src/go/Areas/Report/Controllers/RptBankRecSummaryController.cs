using JieNor.Megi.Core;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.RPT;
using System;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptBankRecSummaryController : GoControllerBase
	{
		private IRPTBankBillRecSummary _rptBankBillRec;

		public RptBankRecSummaryController(IRPTBankBillRecSummary rptBankBillRec)
		{
			_rptBankBillRec = rptBankBillRec;
		}

		public ActionResult BankRec()
		{
			string resultData = _rptBankBillRec.AddReport(null).ResultData;
			return Redirect($"/Report/Report2/{Convert.ToInt32(BizReportType.BankReconciliationSummary)}/{resultData}");
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTBankBillRecSummaryFilterModel filter)
		{
			return _rptBankBillRec.GetBizReportJson(filter, null).ResultData;
		}
	}
}
