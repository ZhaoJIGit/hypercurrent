using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDTrack
	{
		[OperationContract]
		MActionResult<List<BDTrackModel>> GetList(string where, string accessToken = null);

		[OperationContract]
		MActionResult<List<string>> GetTrackNameList(string accessToken = null);

		[OperationContract]
		MActionResult<List<NameValueModel>> GetTrackBasicInfo(string orgIds = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Save(BDTrackModel trcModel, List<BDTrackEntryModel> optionsModels, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> updateEdit(BDTrackModel info, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> updateOptEdit(BDTrackModel info, List<BDTrackEntryModel> entryList, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> IsCanDeleteOrInactive(BDTrackModel info, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> trackDel(BDTrackModel info, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> trackOptDel(BDTrackModel info, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ArchiveTrackEntry(string entryId, int status, string accessToken = null);

		[OperationContract]
		MActionResult<BDTrackModel> GetTrackModelIncludeEntry(string trackId, string accessToken = null);

		[OperationContract]
		MActionResult<BDTrackModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);
	}
}
