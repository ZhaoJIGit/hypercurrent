using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.BD.InitDocument;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDInitDocument
	{
		[OperationContract]
		MActionResult<BDInitDocumentViewModel> GetInitDocumentModel(BDInitDocumentFilterModel query, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveInitDocumentModel(BDInitDocumentViewModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<NameValueModel>> GetInitDocumentData(int type = 0, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateDocCurrentAccountCode(string docType, string docId, string accountCode, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CheckIsExistInitBill(string accessToken = null);
	}
}
