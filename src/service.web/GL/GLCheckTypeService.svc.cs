using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Core;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.GL;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.GL
{
	public class GLCheckTypeService : ServiceT<GLCheckTypeModel>, IGLCheckType
	{
		private readonly IGLCheckTypeBusiness biz = new GLCheckTypeBusiness();

		public MActionResult<GLCheckTypeDataModel> GetCheckTypeDataByType(int type, bool includeDisabled = false, string accessToken = null)
		{
			IGLCheckTypeBusiness iGLCheckTypeBusiness = biz;
			return base.RunFunc(iGLCheckTypeBusiness.GetCheckTypeDataByType, type, includeDisabled, accessToken);
		}

		public MActionResult<List<GLCheckTypeModel>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null)
		{
			IGLCheckTypeBusiness iGLCheckTypeBusiness = biz;
			return base.GetModelList(iGLCheckTypeBusiness.GetModelList, filter, includeDelete, accessToken);
		}
	}
}
