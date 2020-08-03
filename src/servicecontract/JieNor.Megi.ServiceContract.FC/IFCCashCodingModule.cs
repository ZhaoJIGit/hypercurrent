using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FC
{
	[ServiceContract]
	public interface IFCCashCodingModule
	{
		[OperationContract]
		MActionResult<List<FCCashCodingModuleListModel>> GetListByCode(string code, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateCashCodingModuleModel(FCCashCodingModuleModel model, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FCCashCodingModuleListModel>> GetCashCodingByPageList(FCCashCodingModuleListFilter model, string accessToken = null);

		[OperationContract]
		MActionResult<List<FCCashCodingModuleModel>> GetCashCodingModuleListWithNoEntry(string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null);

		[OperationContract]
		MActionResult<FCCashCodingModuleModel> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);
	}
}
