using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.PA
{
	public interface IPAPayItemGroupBussiness : IDataContract<PAPayItemGroupModel>
	{
		List<PAPayItemGroupModel> GetSalaryItemList(MContext ctx);

		PAPayItemGroupModel GetSalaryGroupItemById(MContext ctx, string id);

		OperationResult UpdateModel(MContext ctx, PAPayItemGroupModel model);

		OperationResult Delete(MContext ctx, ParamBase param);
	}
}
