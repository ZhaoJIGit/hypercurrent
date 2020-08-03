using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDTrackEntryService : ServiceT<BDTrackEntryModel>, IBDTrackEntry
	{
		private readonly IBDTrackEntryBusiness biz = new BDTrackEntryBusiness();

		public MActionResult<BDTrackEntryModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IBDTrackEntryBusiness iBDTrackEntryBusiness = biz;
			return base.GetDataModel(iBDTrackEntryBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}

		public MActionResult<List<BDTrackEntryModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			IBDTrackEntryBusiness iBDTrackEntryBusiness = biz;
			return base.GetModelList(iBDTrackEntryBusiness.GetModelList, filter, includeDelete, accessToken);
		}
	}
}
