using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.BusinessService.FA;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.FA;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.FA
{
	public class FAFixAssetsTypeService : ServiceT<FAFixAssetsTypeModel>, IFAFixAssetsType
	{
		private readonly IFAFixAssetsTypeBusiness biz = new FAFixAssetsTypeBusiness();

		public MActionResult<List<FAFixAssetsTypeModel>> GetFixAssetsTypeList(string itemID, string accessToken = null)
		{
			IFAFixAssetsTypeBusiness iFAFixAssetsTypeBusiness = biz;
			return base.RunFunc(iFAFixAssetsTypeBusiness.GetFixAssetsTypeList, itemID, accessToken);
		}

		public MActionResult<FAFixAssetsTypeModel> GetFixAssetsTypeModel(string itemID = null, string accessToken = null)
		{
			IFAFixAssetsTypeBusiness iFAFixAssetsTypeBusiness = biz;
			return base.RunFunc(iFAFixAssetsTypeBusiness.GetFixAssetsTypeModel, itemID, accessToken);
		}

		public MActionResult<OperationResult> DeleteFixAssetsType(List<string> itemIDs, string accessToken = null)
		{
			IFAFixAssetsTypeBusiness iFAFixAssetsTypeBusiness = biz;
			return base.RunFunc(iFAFixAssetsTypeBusiness.DeleteFixAssetsType, itemIDs, accessToken);
		}

		public MActionResult<OperationResult> SaveFixAssetsType(FAFixAssetsTypeModel model, string accessToken = null)
		{
			IFAFixAssetsTypeBusiness iFAFixAssetsTypeBusiness = biz;
			return base.RunFunc(iFAFixAssetsTypeBusiness.SaveFixAssetsType, model, accessToken);
		}
	}
}
