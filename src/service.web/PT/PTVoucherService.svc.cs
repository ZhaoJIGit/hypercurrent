using JieNor.Megi.BusinessContract.PT;
using JieNor.Megi.BusinessService.PT;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.PT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.PT;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.PT
{
	public class PTVoucherService : ServiceT<PTVoucherModel>, IPTVoucher
	{
		private readonly IPTBizBusiness ptBiz = new PTBizBusiness();

		private readonly IPTVoucherBusiness ptVoucher = new PTVoucherBusiness();

		public MActionResult<List<PTVoucherModel>> GetList(string accessToken = null)
		{
			IPTVoucherBusiness iPTVoucherBusiness = ptVoucher;
			return base.RunFunc(iPTVoucherBusiness.GetList, accessToken);
		}

		public MActionResult<PTVoucherModel> GetModel(string itemID, bool isFromPrint = false, string accessToken = null)
		{
			IPTVoucherBusiness iPTVoucherBusiness = ptVoucher;
			return base.RunFunc(iPTVoucherBusiness.GetModel, itemID, isFromPrint, accessToken);
		}

		public MActionResult<OperationResult> Sort(string ids, string accessToken = null)
		{
			IPTVoucherBusiness iPTVoucherBusiness = ptVoucher;
			return base.RunFunc(iPTVoucherBusiness.Sort, ids, accessToken);
		}

		public MActionResult<Dictionary<string, string>> GetKeyValueList(string accessToken = null)
		{
			IPTBizBusiness iPTBizBusiness = ptBiz;
			return base.RunFunc(iPTBizBusiness.GetKeyValueList, "Voucher", accessToken);
		}

		public MActionResult<PTVoucherModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null)
		{
			IPTVoucherBusiness iPTVoucherBusiness = ptVoucher;
			return base.GetDataModel(iPTVoucherBusiness.GetDataModel, pkID, includeDelete, accessToken);
		}

		public MActionResult<OperationResult> InsertOrUpdate(PTVoucherModel modelData, string fields = null, string accessToken = null)
		{
			IPTVoucherBusiness iPTVoucherBusiness = ptVoucher;
			return base.InsertOrUpdate(iPTVoucherBusiness.InsertOrUpdate, modelData, fields, accessToken);
		}

		public MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null)
		{
			IPTVoucherBusiness iPTVoucherBusiness = ptVoucher;
			return base.DeleteModels(iPTVoucherBusiness.DeleteModels, pkID, accessToken);
		}
	}
}
