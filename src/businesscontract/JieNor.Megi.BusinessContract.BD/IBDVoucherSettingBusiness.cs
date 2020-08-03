using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BD
{
	public interface IBDVoucherSettingBusiness : IDataContract<BDVoucherSettingModel>
	{
		List<BDVoucherSettingCategoryModel> GetVoucherSettingCategoryList(MContext ctx);

		OperationResult SaveVoucherSetting(MContext ctx, List<BDVoucherSettingModel> list);
	}
}
