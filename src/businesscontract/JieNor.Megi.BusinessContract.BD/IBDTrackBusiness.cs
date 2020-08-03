using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDTrackBusiness : IDataContract<BDTrackModel>
	{
		List<BDTrackModel> GetList(MContext ctx, string where);

		List<string> GetTrackNameList(MContext ctx);

		List<NameValueModel> GetTrackBasicInfo(MContext ctx, string orgIds = null);

		OperationResult SaveList(MContext ctx, string[] array);

		OperationResult SaveTrack(MContext ctx, BDTrackModel trcModel, List<BDTrackEntryModel> optionsModels);

		OperationResult updateEdit(MContext ctx, BDTrackModel info);

		OperationResult updateOptEdit(MContext ctx, BDTrackModel info, List<BDTrackEntryModel> entryList);

		OperationResult IsCanDeleteOrInactive(MContext ctx, BDTrackModel info);

		OperationResult trackDel(MContext ctx, BDTrackModel info);

		OperationResult trackOptDel(MContext ctx, BDTrackModel info);

		OperationResult ArchiveTrackEntry(MContext ctx, string entryId, int status);

		BDTrackModel GetTrackModelIncludeEntry(MContext ctx, string trackId);
	}
}
