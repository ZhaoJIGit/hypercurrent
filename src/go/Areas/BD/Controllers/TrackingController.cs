using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using JieNor.Megi.Go.Web.Controllers;
using JieNor.Megi.Identity.Attribute;
using JieNor.Megi.ServiceContract.BD;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

namespace JieNor.Megi.Go.Web.Areas.BD.Controllers
{
	public class TrackingController : GoControllerBase
	{
		private IBDTrack _track = null;

		private IBDTrackEntry trackEntryService = null;

		public TrackingController()
		{
		}

		public TrackingController(IBDTrack track, IBDTrackEntry trackEntry)
		{
			_track = track;
			trackEntryService = trackEntry;
		}

		[Permission("Setting", "View", "")]
		public ActionResult Index(string id = "0")
		{
			List<BDTrackModel> resultData = _track.GetList("", null).ResultData;
			ViewBag.list = resultData;
			ViewBag.tabKeySelected = id;

			return View();
		}

		[Permission("Setting", "View", "")]
		public ActionResult GetTrackList()
		{
			MActionResult<List<BDTrackModel>> list = _track.GetList("", null);
			return base.Json(list);
		}

		[Permission("Setting", "Change", "")]
		public ActionResult CategoryEdit(string pkId, string tabIndex = "0")
		{
			ViewBag.pkId = pkId;
			ViewBag.tabIndex = tabIndex;

			return base.View();
		}

		[Permission("Setting", "Change", "")]
		public ActionResult CategoryOptionEdit(string trackId, string pkId, string tabIndex = "0")
		{
			ViewBag.trackId = trackId;

			BDTrackModel resultData = _track.GetTrackModelIncludeEntry(trackId, null).ResultData;
			ViewBag.curName = "";
			ViewBag.tabTitle = "";

			if (resultData != null)
			{
				List<MultiLanguageFieldList> multiLanguage = resultData.MultiLanguage;
				if (multiLanguage != null)
				{
					MultiLanguageFieldList multiLanguageFieldList = (from x in multiLanguage
					where x.MFieldName == "MName"
					select x).FirstOrDefault();
					if (multiLanguageFieldList != null)
					{
						ViewBag.tabTitle = multiLanguageFieldList.MMultiLanguageValue;
					}
				}
				BDTrackEntryModel bDTrackEntryModel = (from x in resultData.MEntryList
				where x.MEntryID == pkId
				select x).FirstOrDefault();
				if (bDTrackEntryModel != null)
				{
					List<MultiLanguageFieldList> multiLanguage2 = bDTrackEntryModel.MultiLanguage;
					if (multiLanguage2 != null)
					{
						MultiLanguageFieldList multiLanguageFieldList2 = (from x in multiLanguage2
						where x.MFieldName == "MName"
						select x).FirstOrDefault();
						if (multiLanguageFieldList2 != null)
						{
							ViewBag.curName = multiLanguageFieldList2.MMultiLanguageValue;
						}
					}
				}
			}

			ViewBag.pkId = pkId;
			ViewBag.tabIndex = tabIndex;

			return View();
		}

		[Permission("Setting", "Change", "")]
		public JsonResult SaveTrackingInfo(BDTrackModel model, List<BDTrackEntryModel> optionsModels)
		{
			MActionResult<OperationResult> data = _track.Save(model, optionsModels, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult TrackingUpd(BDTrackModel info)
		{
			MActionResult<OperationResult> data = _track.updateEdit(info, null);
			return base.Json(data);
		}

		public JsonResult GetTrackOptionById(string optionId)
		{
			BDTrackEntryModel bDTrackEntryModel = trackEntryService.GetDataModel(optionId, false, null).ResultData;
			if (bDTrackEntryModel == null)
			{
				bDTrackEntryModel = new BDTrackEntryModel();
			}
			return base.Json(bDTrackEntryModel);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult TrackingOptUpd(BDTrackModel track, List<BDTrackEntryModel> entryList)
		{
			MActionResult<OperationResult> data = _track.updateOptEdit(track, entryList, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult IsCanDeleteOrInactive(BDTrackModel info)
		{
			MActionResult<OperationResult> data = _track.IsCanDeleteOrInactive(info, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult TrackingDelete(BDTrackModel info)
		{
			MActionResult<OperationResult> data = _track.trackDel(info, null);
			return base.Json(data);
		}

		[Permission("Setting", "Change", "")]
		public JsonResult TrackingOptDelete(BDTrackModel info)
		{
			MActionResult<OperationResult> data = _track.trackOptDel(info, null);
			return base.Json(data);
		}

		[Permission("Setting", "View", "")]
		public JsonResult GetTrackById(string trackId, string optionId = null)
		{
			BDTrackModel bDTrackModel = new BDTrackModel();
			if (string.IsNullOrEmpty(optionId))
			{
				bDTrackModel = _track.GetDataModel(trackId, false, null).ResultData;
			}
			else
			{
				List<BDTrackModel> resultData = _track.GetList("", null).ResultData;
				bDTrackModel = (from x in resultData
				where x.MEntryID == optionId
				select x).FirstOrDefault();
			}
			return base.Json(bDTrackModel);
		}

		public JsonResult GetTrackOptionsById(string trackId)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(" MItemID ", trackId);
			List<BDTrackEntryModel> resultData = trackEntryService.GetModelList(sqlWhere, false, null).ResultData;
			List<NameValueModel> list = new List<NameValueModel>();
			if (resultData != null)
			{
				foreach (BDTrackEntryModel item in resultData)
				{
					NameValueModel nameValueModel = new NameValueModel();
					nameValueModel.MValue = item.MEntryID;
					nameValueModel.MValue1 = (item.MIsActive ? "1" : "0");
					List<MultiLanguageFieldList> multiLanguage = item.MultiLanguage;
					if (multiLanguage != null)
					{
						MultiLanguageFieldList multiLanguageFieldList = (from x in multiLanguage
						where x.MFieldName == "MName"
						select x).FirstOrDefault();
						if (multiLanguageFieldList != null)
						{
							nameValueModel.MName = multiLanguageFieldList.MMultiLanguageValue;
						}
					}
					list.Add(nameValueModel);
				}
			}
			list = (from x in list
			orderby x.MName
			select x).ToList();
			return base.Json(list);
		}

		public ActionResult ArchiveTrackEntry(string entryId, bool status)
		{
			int status2 = status ? 1 : 0;
			MActionResult<OperationResult> data = _track.ArchiveTrackEntry(entryId, status2, null);
			return base.Json(data);
		}

		public ActionResult GetTrackBasicInfo()
		{
			MActionResult<List<NameValueModel>> trackBasicInfo = _track.GetTrackBasicInfo(null, null);
			return base.Json(trackBasicInfo);
		}
	}
}
