using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.PT;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.PT
{
	public interface IPTVoucherBusiness : IDataContract<PTVoucherModel>
	{
		List<PTVoucherModel> GetList(MContext ctx);

		PTVoucherModel GetModel(MContext ctx, string itemID, bool isFromPrint = false);

		OperationResult Sort(MContext ctx, string ids);
	}
}
