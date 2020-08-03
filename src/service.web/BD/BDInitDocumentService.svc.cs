using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.BD.InitDocument;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BD;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.BD
{
	public class BDInitDocumentService : ServiceT<BDInitDocumentModel>, IBDInitDocument
	{
		private readonly IBDInitDocumentBusiness biz = new BDInitDocumentBusiness();

		public MActionResult<BDInitDocumentViewModel> GetInitDocumentModel(BDInitDocumentFilterModel query, string accessToken = null)
		{
			IBDInitDocumentBusiness iBDInitDocumentBusiness = biz;
			return base.RunFunc(iBDInitDocumentBusiness.GetInitDocumentList, query, accessToken);
		}

		public MActionResult<OperationResult> SaveInitDocumentModel(BDInitDocumentViewModel model, string accessToken = null)
		{
			IBDInitDocumentBusiness iBDInitDocumentBusiness = biz;
			return base.RunFunc(iBDInitDocumentBusiness.SaveInitDocumentModel, model, accessToken);
		}

		public MActionResult<OperationResult> UpdateDocCurrentAccountCode(string docType, string docId, string accountCode, string accessToken = null)
		{
			IBDInitDocumentBusiness iBDInitDocumentBusiness = biz;
			return base.RunFunc(iBDInitDocumentBusiness.UpdateDocCurrentAccountCode, docType, docId, accountCode, accessToken);
		}

		public MActionResult<OperationResult> CheckIsExistInitBill(string accessToken = null)
		{
			IBDInitDocumentBusiness iBDInitDocumentBusiness = biz;
			return base.RunFunc(iBDInitDocumentBusiness.CheckIsExistInitBill, null);
		}

		public MActionResult<List<NameValueModel>> GetInitDocumentData(int type = 0, string accessToken = null)
		{
			IBDInitDocumentBusiness iBDInitDocumentBusiness = biz;
			return base.RunFunc(iBDInitDocumentBusiness.GetInitDocumentData, type, accessToken);
		}
	}
}
