using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FC
{
	public interface IFCFapiaoModuleBusiness : IDataContract<FCFapiaoModuleModel>
	{
		OperationResult SaveFapiaoModule(MContext ctx, FCFapiaoModuleModel model);

		OperationResult DeleteFapiaoModules(MContext ctx, List<string> ids);

		DataGridJson<FCFapiaoModuleModel> GetFapiaoModulePageList(MContext ctx, FCFastCodeFilterModel filter);

		List<FCFapiaoModuleModel> GetFapiaoModuleList(MContext ctx, FCFastCodeFilterModel fitler);

		int GetFapiaoModulePageListCount(MContext ctx);
	}
}
