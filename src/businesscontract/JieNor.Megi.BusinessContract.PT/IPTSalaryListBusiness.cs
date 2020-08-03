using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.PT
{
	public interface IPTSalaryListBusiness : IDataContract<PAPrintSettingModel>
	{
		List<PAPrintSettingModel> GetList(MContext ctx);

		OperationResult Copy(MContext ctx, PAPrintSettingModel model, bool isCopyTmpl = false);

		PAPrintSettingModel GetPrintSetting(MContext ctx, string itemID, bool isFromPrint = false);

		OperationResult Sort(MContext ctx, string ids);
	}
}
