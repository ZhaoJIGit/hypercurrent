using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.PT
{
	public interface IPTBizBusiness : IDataContract<BDPrintSettingModel>
	{
		List<BDPrintSettingModel> GetList(MContext ctx);

		OperationResult Copy(MContext ctx, BDPrintSettingModel model, bool isCopyTmpl = false);

		BDPrintSettingModel GetPrintSetting(MContext ctx, string itemID, bool isFromPrint = false);

		OperationResult Sort(MContext ctx, string ids);

		Dictionary<string, string> GetKeyValueList(MContext ctx, string bizObject);
	}
}
