using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.PT
{
	[ServiceContract]
	public interface IPTSalaryList
	{
		[OperationContract]
		MActionResult<List<PAPrintSettingModel>> GetList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Copy(PAPrintSettingModel model, bool isCopyTmpl = false, string accessToken = null);

		[OperationContract]
		MActionResult<PAPrintSettingModel> GetPrintSetting(string itemID, bool isFromPrint = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Sort(string ids, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdate(PAPrintSettingModel modelData, string fields = null, string accessToken = null);

		[OperationContract]
		MActionResult<PAPrintSettingModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);
	}
}
