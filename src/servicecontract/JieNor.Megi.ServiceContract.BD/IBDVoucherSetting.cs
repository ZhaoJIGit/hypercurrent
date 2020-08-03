using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BD
{
	[ServiceContract]
	public interface IBDVoucherSetting
	{
		[OperationContract]
		MActionResult<List<BDVoucherSettingCategoryModel>> GetVoucherSettingCategoryList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveVoucherSetting(List<BDVoucherSettingModel> list, string accessToken = null);
	}
}
