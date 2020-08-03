using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.GL
{
	public interface IGLVoucherReferenceBusiness : IDataContract<GLVoucherReferenceModel>
	{
		OperationResult InsertReference(MContext ctx, GLVoucherReferenceModel model);

		List<GLVoucherReferenceModel> GetReferenceList(MContext ctx, int size);
	}
}
