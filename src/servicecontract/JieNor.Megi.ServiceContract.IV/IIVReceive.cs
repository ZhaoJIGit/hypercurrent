using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.IV
{
	[ServiceContract]
	public interface IIVReceive
	{
		[OperationContract]
		MActionResult<List<IVReceiveListModel>> GetReceiveList(string filterString, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReceive(IVReceiveModel model, string accessToken = null);

		[OperationContract]
		MActionResult<IVReceiveModel> GetReceiveEditModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<IVReceiveViewModel> GetReceiveViewModel(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteReceive(IVReceiveModel model, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVReceiveModel>> GetInitList(string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<IVReceiveModel>> GetInitReceiveListByPage(IVReceiveListFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteReceiveList(ParamBase param, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> UpdateReconcileStatu(string receiveId, IVReconcileStatus statu, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> BatchUpdateReconcileStatu(ParamBase param, IVReconcileStatus statu, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> ImportReceiveList(List<IVReceiveModel> list, string accessToken = null);

		[OperationContract]
		MActionResult<ImportTemplateModel> GetImportTemplateModel(IVImportTransactionFilterModel param, string accessToken = null);

		[OperationContract]
		MActionResult<List<IVReceiveModel>> GetReceiveListByFilter(IVReceiveListFilterModel filter, string accessToken = null);
	}
}
