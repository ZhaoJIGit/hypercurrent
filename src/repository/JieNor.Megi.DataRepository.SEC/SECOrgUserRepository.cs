using JieNor.Megi.Core;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.SEC
{
	public class SECOrgUserRepository : DataServiceT<SECOrgUserModel>
	{
		public static SECOrgUserModel GetOrgUserModel(MContext ctx)
		{
			if (string.IsNullOrEmpty(ctx.MOrgID))
			{
				return null;
			}
			SqlWhere filter = new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MUserID", ctx.MUserID).Equal("MIsDelete", 0);
			List<SECOrgUserModel> dataModelList = ModelInfoManager.GetDataModelList<SECOrgUserModel>(ctx, filter, false, false);
			if (dataModelList == null || dataModelList.Count == 0)
			{
				return null;
			}
			return dataModelList[0];
		}
	}
}
