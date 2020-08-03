using JieNor.Megi.BusinessContract.PT;
using JieNor.Megi.BusinessService.PT;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.PT;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.PT
{
	public class PTBizService : ServiceT<BDPrintSettingModel>, IPTBiz
	{
		private readonly IPTBizBusiness biz = new PTBizBusiness();

		public MActionResult<List<BDPrintSettingModel>> GetList(string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.GetList, accessToken);
		}

		public MActionResult<OperationResult> Copy(BDPrintSettingModel model, bool isCopyTmpl = false, string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.Copy, model, isCopyTmpl, accessToken);
		}

		public MActionResult<BDPrintSettingModel> GetPrintSetting(string itemID, bool isFromPrint = false, string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.GetPrintSetting, itemID, isFromPrint, accessToken);
		}

		public MActionResult<OperationResult> Sort(string ids, string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.Sort, ids, accessToken);
		}

		public MActionResult<Dictionary<string, string>> GetKeyValueList(string bizObject, string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.GetKeyValueList, bizObject, accessToken);
		}

		public MActionResult<OperationResult> InsertOrUpdate(BDPrintSettingModel modelData, string fields = null, string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.InsertOrUpdate, modelData, fields, accessToken);
		}

		public MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.DeleteModels, pkID, accessToken);
		}

		public MActionResult<BDPrintSettingModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = biz;
			return base.RunFunc(iPTBizBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}
	}
}
