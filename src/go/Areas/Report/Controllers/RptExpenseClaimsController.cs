using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BD;
using JieNor.Megi.ServiceContract.RPT;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.Report.Controllers
{
	public class RptExpenseClaimsController : GoControllerBase
	{
		private IBDTrack TrackService = null;

		private IRPTExpenseClaim reportService = null;

		public RptExpenseClaimsController(IBDTrack track, IRPTExpenseClaim report)
		{
			TrackService = track;
			reportService = report;
		}

		public ActionResult Index()
		{
			return base.View();
		}

		public ActionResult GetTrack(int count = 5)
		{
			List<object> data = new List<object>();
			List<BDTrackModel> resultData = TrackService.GetList("", null).ResultData;
			if (resultData != null && resultData.Count > 0)
			{
				resultData = (from x in resultData
				orderby x.MCreateDate
				select x).ToList();
				data = GetTrackListJson(resultData);
			}
			return base.Json(data);
		}

		private List<object> GetTrackListJson(List<BDTrackModel> trackList)
		{
			List<object> list = new List<object>();
			string empty = string.Empty;
			object obj = null;
			List<string> list2 = new List<string>();
			foreach (BDTrackModel track in trackList)
			{
				empty = track.MPKID;
				if (!list2.Contains(track.MPKID) && list2.Count < 5)
				{
					List<BDTrackModel> options = (from x in trackList
					where x.MPKID == track.MPKID
					select x).ToList();
					obj = new
					{
						MPKID = track.MPKID,
						MName = track.MName,
						Options = options
					};
					list2.Add(track.MPKID);
					list.Add(obj);
				}
			}
			return list;
		}

		[Permission("Report", "View", "")]
		public string GetReportData(RPTExpenseClaimFilterModel filter)
		{
			return reportService.GetBizReportJson(filter, null).ResultData;
		}
	}
}
