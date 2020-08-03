using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.IV
{
	public interface IIVReceiveBusiness
	{
		List<IVReceiveListModel> GetReceiveList(MContext ctx, string filterString);

		DataGridJson<IVReceiveModel> GetInitReceiveListByPage(MContext ctx, IVReceiveListFilterModel filter);

		OperationResult UpdateReceive(MContext ctx, IVReceiveModel model);

		IVReceiveModel GetReceiveEditModel(MContext ctx, string pkID);

		IVReceiveViewModel GetReceiveViewModel(MContext ctx, string pkID);

		OperationResult DeleteReceive(MContext ctx, IVReceiveModel model);

		List<IVReceiveModel> GetInitList(MContext ctx);

		OperationResult DeleteReceiveList(MContext ctx, ParamBase param);

		OperationResult ImportReceiveList(MContext ctx, List<IVReceiveModel> list);

		OperationResult UpdateReconcileStatu(MContext ctx, string receiveId, IVReconcileStatus statu);

		OperationResult UpdateReconcileStatu(MContext ctx, ParamBase param, IVReconcileStatus statu);

		ImportTemplateModel GetImportTemplateModel(MContext ctx, IVImportTransactionFilterModel param);

		List<IVReceiveModel> GetReceiveListByFilter(MContext ctx, IVReceiveListFilterModel filter);

		List<IVReceiveModel> GetReceiveListIncludeEnetry(MContext ctx, IVReceiveListFilterModel filter);
	}
}
