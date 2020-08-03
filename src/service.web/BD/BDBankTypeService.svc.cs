using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDBankTypeService : ServiceT<BDBankTypeModel>, IBDBankType
	{
		private readonly IBDBankTypeBusiness biz = new BDBankTypeBusiness();

		public MActionResult<List<BDBankTypeViewModel>> GetBDBankTypeList(string accessToken = null)
		{
			IBDBankTypeBusiness iBDBankTypeBusiness = biz;
			return base.RunFunc(iBDBankTypeBusiness.GetBDBankTypeList, accessToken);
		}

		public MActionResult<OperationResult> SaveBankType(BDBankTypeEditModel banktype, string accessToken = null)
		{
			IBDBankTypeBusiness iBDBankTypeBusiness = biz;
			return base.RunFunc(iBDBankTypeBusiness.SaveBankType, banktype, accessToken);
		}

		public MActionResult<OperationResult> DeleteBankType(BDBankTypeModel banktype, string accessToken = null)
		{
			IBDBankTypeBusiness iBDBankTypeBusiness = biz;
			return base.RunFunc(iBDBankTypeBusiness.DeleteBankType, banktype, accessToken);
		}

		public MActionResult<BDBankTypeEditModel> GetBDBankTypeEditModel(BDBankTypeEditModel banktype, string accessToken = null)
		{
			IBDBankTypeBusiness iBDBankTypeBusiness = biz;
			return base.RunFunc(iBDBankTypeBusiness.GetBDBankTypeEditModel, banktype, accessToken);
		}
	}
}
