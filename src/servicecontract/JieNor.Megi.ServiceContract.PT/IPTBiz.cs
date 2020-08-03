using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.PT
{
	[ServiceContract]
	public interface IPTBiz
	{
		[OperationContract]
		MActionResult<List<BDPrintSettingModel>> GetList(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Copy(BDPrintSettingModel model, bool isCopyTmpl = false, string accessToken = null);

		[OperationContract]
		MActionResult<BDPrintSettingModel> GetPrintSetting(string itemID, bool isFromPrint = false, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Sort(string ids, string accessToken = null);

		[OperationContract]
		MActionResult<Dictionary<string, string>> GetKeyValueList(string bizObject, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdate(BDPrintSettingModel modelData, string fields = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null);

		[OperationContract]
		MActionResult<BDPrintSettingModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);
	}
}
