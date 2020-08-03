using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.BD.InitDocument;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDInitDocumentBusiness : IDataContract<BDInitDocumentModel>
	{
		BDInitDocumentViewModel GetInitDocumentList(MContext ctx, BDInitDocumentFilterModel query);

		List<NameValueModel> GetInitDocumentData(MContext ctx, int type = 0);

		OperationResult SaveInitDocumentModel(MContext ctx, BDInitDocumentViewModel model);

		OperationResult UpdateDocCurrentAccountCode(MContext ctx, string docType, string docId, string accountCode);

		OperationResult CheckIsExistInitBill(MContext ctx);
	}
}
