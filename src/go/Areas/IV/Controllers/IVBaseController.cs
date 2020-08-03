using JieNor.Megi.Common.ImportAndExport;
using JieNor.Megi.Common.ImportAndExport.DataModel;
using JieNor.Megi.Common.ImportAndExport.Export;
using JieNor.Megi.Common.ImportAndExport.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.IO.Export;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.HtmlHelper;
using JieNor.Megi.ServiceContract.BAS;
using JieNor.Megi.ServiceContract.GL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.IV.Controllers
{
	public class IVBaseController : GoControllerBase
	{
		protected IBASOrganisation _org = null;

		protected IGLSettlement _settle = null;

		public IVBaseController(IBASOrganisation org, IGLSettlement settle)
		{
			_org = org;
			_settle = settle;
		}

		public IVBaseController()
		{
		}

		public FileResult Export(string jsonParam)
		{
			dynamic obj = null;
			string empty = string.Empty;
			BizReportType bizReportType = BizReportType.None;
			IVListFilterBaseModel iVListFilterBaseModel = ReportParameterHelper.DeserializeObject<IVListFilterBaseModel>(jsonParam);
			if (iVListFilterBaseModel.BizObject == "Expense")
			{
				obj = ReportParameterHelper.DeserializeObject<IVExpenseListFilterModel>(jsonParam);
				empty = HtmlLang.GetText(LangModule.IV, "ExpenseClaims", "Expense Claims");
				bizReportType = BizReportType.ExpenseList;
			}
			else
			{
				obj = ReportParameterHelper.DeserializeObject<IVInvoiceListFilterModel>(jsonParam);
				//if (_003C_003Eo__4._003C_003Ep__2 == null)
				//{
				//	_003C_003Eo__4._003C_003Ep__2 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof(IVBaseController), new CSharpArgumentInfo[1]
				//	{
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				//	}));
				//}
				//Func<CallSite, object, bool> target = _003C_003Eo__4._003C_003Ep__2.Target;
				//CallSite<Func<CallSite, object, bool>> _003C_003Ep__ = _003C_003Eo__4._003C_003Ep__2;
				//if (_003C_003Eo__4._003C_003Ep__1 == null)
				//{
				//	_003C_003Eo__4._003C_003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof(IVBaseController), new CSharpArgumentInfo[2]
				//	{
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, null)
				//	}));
				//}
				//Func<CallSite, object, string, object> target2 = _003C_003Eo__4._003C_003Ep__1.Target;
				//CallSite<Func<CallSite, object, string, object>> _003C_003Ep__2 = _003C_003Eo__4._003C_003Ep__1;
				//if (_003C_003Eo__4._003C_003Ep__0 == null)
				//{
				//	_003C_003Eo__4._003C_003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "MType", typeof(IVBaseController), new CSharpArgumentInfo[1]
				//	{
				//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
				//	}));
				//}

				if (obj.MType == "Invoice_Sale")
					//if (target(_003C_003Ep__, target2(_003C_003Ep__2, _003C_003Eo__4._003C_003Ep__0.Target(_003C_003Eo__4._003C_003Ep__0, obj), "Invoice_Sale")))
				{
					empty = HtmlLang.GetText(LangModule.IV, "Invoices", "Invoices");
					bizReportType = BizReportType.InvoiceList;
				}
				else
				{
					empty = HtmlLang.GetText(LangModule.IV, "Bill", "Bill");
					bizReportType = BizReportType.PurchaseList;
				}
			}
			ReportModel reportModel = ReportStorageHelper.CreateReportModel(bizReportType, jsonParam, CreateReportModelSource.Export, null, null, null);
			Stream stream = ExportHelper.CreateRptExportFile(reportModel, ExportFileType.Xls);
			string exportName = $"{reportModel.OrgName}-{empty}.xls";
			return base.ExportReport(stream, exportName);
		}

		protected DateTime SetDefaultBizDate(bool minConversionDate = false)
		{
			BASOrganisationModel resultData = _org.GetModel(null).ResultData;
			if (resultData == null)
			{
				string defaultBizDate = GetDefaultBizDate();
				base.ViewData["DefaultBizDate"] = defaultBizDate;
				base.ViewData["MaxBizDate"] = defaultBizDate;
				base.ViewData["MConversionDate"] = defaultBizDate;
				return DateTime.Now;
			}
			if (resultData.MConversionDate < new DateTime(1900, 1, 1))
			{
				resultData.MConversionDate = new DateTime(1911, 1, 2);
			}
			if (minConversionDate)
			{
				base.ViewData["DefaultBizDate"] = resultData.MConversionDate.AddDays(-1.0).ToDateFormat();
				base.ViewData["MaxBizDate"] = resultData.MConversionDate.ToDateFormat();
				base.ViewData["MConversionDate"] = resultData.MConversionDate;
				return resultData.MConversionDate;
			}
			base.ViewData["DefaultBizDate"] = GetDefaultBizDate();
			base.ViewData["MaxBizDate"] = resultData.MConversionDate.ToDateFormat();
			base.ViewData["MConversionDate"] = resultData.MConversionDate;
			return resultData.MConversionDate;
		}

		protected string GetDefaultBizDate()
		{
			List<string> resultData = _settle.GetSettledPeriod(null).ResultData;
			if (resultData == null || resultData.Count == 0)
			{
				return HtmlLang.DateNowString();
			}
			List<DateTime> list = new List<DateTime>();
			foreach (string item in resultData)
			{
				string[] array = item.Split('-');
				list.Add(new DateTime(Convert.ToInt32(array[0]), Convert.ToInt32(array[1]), 1));
			}
			DateTime dateTime = (from t in list
			orderby t descending
			select t).First();
			if (dateTime.AddMonths(1).AddSeconds(-1.0) < DateTime.Now)
			{
				return DateTime.Now.ToLangDate();
			}
			return dateTime.AddMonths(1).ToLangDate();
		}
	}
}
