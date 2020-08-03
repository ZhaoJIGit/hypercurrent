using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLDocVoucher
	{
		[OperationContract]
		MActionResult<DataGridJson<GLDocEntryVoucherModel>> GetDocVoucherModelList(GLDocVoucherFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> CreateDocVoucher(List<GLDocEntryVoucherModel> list, bool create = true, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteDocVoucher(List<GLDocEntryVoucherModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<List<string>> GetUpdatedDocTable(DateTime lastQueryTime, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ResetDocVoucher(List<string> docIDs, int docType, string accessToken = null);
	}
}
