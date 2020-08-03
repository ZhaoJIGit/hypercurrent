using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.GL
{
	[ServiceContract]
	public interface IGLVoucherReference
	{
		[OperationContract]
		MActionResult<OperationResult> InsertReference(GLVoucherReferenceModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<GLVoucherReferenceModel>> GetReferenceList(int size, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<GLVoucherReferenceModel>> GetModelPageList(SqlWhere filter, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Delete(string pkID, string accessToken = null);
	}
}
