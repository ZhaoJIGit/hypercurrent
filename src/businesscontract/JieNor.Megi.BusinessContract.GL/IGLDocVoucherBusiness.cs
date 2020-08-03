using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLDocVoucherBusiness : IDataContract<GLDocVoucherModel>
	{
		DataGridJson<GLDocEntryVoucherModel> GetDocVoucherModelList(MContext ctx, GLDocVoucherFilterModel filter);

		OperationResult CreateDocVoucher(MContext ctx, List<GLDocEntryVoucherModel> list, bool create);

		OperationResult DeleteDocVoucher(MContext ctx, List<GLDocEntryVoucherModel> list);

		List<string> GetUpdatedDocTable(MContext ctx, DateTime lastQueryTime);

		OperationResult ResetDocVoucher(MContext ctx, List<string> list, int docType);
	}
}
