using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD.RI;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.RI
{
	[ServiceContract]
	public interface IRIInspect
	{
		[OperationContract]
		MActionResult<RIInspectionResult> Inspect(string settingId, int year, int period, string accessToken = null);

		[OperationContract]
		MActionResult<List<RICategoryModel>> GetCategoryList(bool includeSettingDisable = true, int year = 0, int period = 0, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ClearDataPool(string accessToken = null);

		[OperationContract]
		MActionResult<List<BDInspectItemTreeModel>> GetInspectItemTreeList(BDInspectItemFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InitInspectItem(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> SaveInspectSetting(List<RICategorySettingModel> inspectSetingItemList, string accessToken = null);
	}
}
