using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Report
{
	public class ReportAreaRegistration : AreaRegistration
	{
		public override string AreaName => "Report";

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute("Report_ReportHome", "Report/ReportHome/{action}/{reportTypeId}/{reportId}/{pReportTypeId}/{pReportId}", new
			{
				controller = "ReportHome",
				reportTypeId = UrlParameter.Optional,
				reportId = UrlParameter.Optional,
				pReportTypeId = UrlParameter.Optional,
				pReportId = UrlParameter.Optional
			});
			context.MapRoute("Report_View", "Report/Report2/View/{reportTypeId}/{reportId}", new
			{
				controller = "RPTHome",
				action = "View",
				reportTypeId = UrlParameter.Optional,
				reportId = UrlParameter.Optional
			});
			context.MapRoute("Report_Report2", "Report/Report2/{reportTypeId}/{reportId}/{pReportTypeId}/{pReportId}", new
			{
				controller = "RPTHome",
				action = "Report2",
				reportTypeId = UrlParameter.Optional,
				reportId = UrlParameter.Optional,
				pReportTypeId = UrlParameter.Optional,
				pReportId = UrlParameter.Optional
			});
			context.MapRoute("Report_Publish", "Report/Publish/{reportTypeId}/{reportId}", new
			{
				controller = "RPTHome",
				action = "Publish",
				reportTypeId = UrlParameter.Optional,
				reportId = UrlParameter.Optional
			});
			context.MapRoute("Report_Home", "Report/{id}", new
			{
				controller = "RPTHome",
				action = "Index",
				id = UrlParameter.Optional
			});
			context.MapRoute("Report_default", "Report/{controller}/{action}/{id}", new
			{
				controller = "RPTHome",
				action = "Index",
				id = UrlParameter.Optional
			});
		}
	}
}
