using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDBankTypeBusiness
	{
		List<BDBankTypeViewModel> GetBDBankTypeList(MContext ctx);

		BDBankTypeEditModel GetBDBankTypeEditModel(MContext ctx, BDBankTypeEditModel model);

		OperationResult SaveBankType(MContext ctx, BDBankTypeEditModel banktype);

		OperationResult DeleteBankType(MContext ctx, BDBankTypeModel banktype);
	}
}
