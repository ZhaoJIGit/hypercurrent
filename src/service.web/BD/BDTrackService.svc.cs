using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDTrackService : ServiceT<BDTrackModel>, IBDTrack
	{
		private readonly IBDTrackBusiness biz = new BDTrackBusiness();

		public MActionResult<List<BDTrackModel>> GetList(string where, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.GetList, where, accessToken);
		}

		public MActionResult<List<string>> GetTrackNameList(string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.GetTrackNameList, accessToken);
		}

		public MActionResult<List<NameValueModel>> GetTrackBasicInfo(string orgIds = null, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.GetTrackBasicInfo, orgIds, accessToken);
		}

		public MActionResult<OperationResult> Save(BDTrackModel trcModel, List<BDTrackEntryModel> optionsModels, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.SaveTrack, trcModel, optionsModels, accessToken);
		}

		public MActionResult<OperationResult> updateEdit(BDTrackModel info, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.updateEdit, info, accessToken);
		}

		public MActionResult<OperationResult> updateOptEdit(BDTrackModel info, List<BDTrackEntryModel> entryList, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.updateOptEdit, info, entryList, accessToken);
		}

		public MActionResult<OperationResult> IsCanDeleteOrInactive(BDTrackModel info, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.IsCanDeleteOrInactive, info, accessToken);
		}

		public MActionResult<OperationResult> trackDel(BDTrackModel info, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.trackDel, info, accessToken);
		}

		public MActionResult<OperationResult> trackOptDel(BDTrackModel info, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.trackOptDel, info, accessToken);
		}

		public MActionResult<OperationResult> ArchiveTrackEntry(string entryId, int status, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.ArchiveTrackEntry, entryId, status, accessToken);
		}

		public MActionResult<BDTrackModel> GetTrackModelIncludeEntry(string trackId, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.RunFunc(iBDTrackBusiness.GetTrackModelIncludeEntry, trackId, accessToken);
		}

		public MActionResult<BDTrackModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IBDTrackBusiness iBDTrackBusiness = biz;
			return base.GetDataModel(iBDTrackBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}
	}
}
