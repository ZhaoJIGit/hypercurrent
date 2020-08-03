using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FA
{
	public interface IFAFixAssetsTypeBusiness : IDataContract<FAFixAssetsTypeModel>
	{
		List<FAFixAssetsTypeModel> GetFixAssetsTypeList(MContext ctx, string itemID = null);

		FAFixAssetsTypeModel GetFixAssetsTypeModel(MContext ctx, string itemID = null);

		OperationResult DeleteFixAssetsType(MContext ctx, List<string> itemIDs);

		OperationResult SaveFixAssetsType(MContext ctx, FAFixAssetsTypeModel model);
	}
}
