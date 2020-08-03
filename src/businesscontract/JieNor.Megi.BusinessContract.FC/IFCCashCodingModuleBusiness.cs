using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.FC
{
	public interface IFCCashCodingModuleBusiness : IDataContract<FCCashCodingModuleModel>
	{
		List<FCCashCodingModuleListModel> GetListByCode(MContext ctx, string code);

		OperationResult UpdateCashCodingModuleModel(MContext ctx, FCCashCodingModuleModel model);

		DataGridJson<FCCashCodingModuleListModel> GetCashCodingByPageList(MContext ctx, FCCashCodingModuleListFilter model);

		List<FCCashCodingModuleModel> GetCashCodingModuleListWithNoEntry(MContext ctx);
	}
}
