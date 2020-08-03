using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.IV;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.IV
{
	public class IVReceiveService : ServiceT<IVReceiveModel>, IIVReceive
	{
		private readonly IIVReceiveBusiness biz = new IVReceiveBusiness();

		public MActionResult<List<IVReceiveListModel>> GetReceiveList(string filterString, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.GetReceiveList, filterString, accessToken);
		}

		public MActionResult<OperationResult> UpdateReceive(IVReceiveModel model, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.UpdateReceive, model, accessToken);
		}

		public MActionResult<IVReceiveModel> GetReceiveEditModel(string pkID, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.GetReceiveEditModel, pkID, accessToken);
		}

		public MActionResult<IVReceiveViewModel> GetReceiveViewModel(string pkID, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.GetReceiveViewModel, pkID, accessToken);
		}

		public MActionResult<OperationResult> DeleteReceive(IVReceiveModel model, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.DeleteReceive, model, accessToken);
		}

		public MActionResult<List<IVReceiveModel>> GetInitList(string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.GetInitList, accessToken);
		}

		public MActionResult<DataGridJson<IVReceiveModel>> GetInitReceiveListByPage(IVReceiveListFilterModel filter, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.GetInitReceiveListByPage, filter, accessToken);
		}

		public MActionResult<OperationResult> DeleteReceiveList(ParamBase param, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.DeleteReceiveList, param, accessToken);
		}

		public MActionResult<OperationResult> ImportReceiveList(List<IVReceiveModel> list, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.ImportReceiveList, list, accessToken);
		}

		public MActionResult<OperationResult> UpdateReconcileStatu(string receiveId, IVReconcileStatus statu, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.UpdateReconcileStatu, receiveId, statu, accessToken);
		}

		public MActionResult<OperationResult> BatchUpdateReconcileStatu(ParamBase param, IVReconcileStatus statu, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.UpdateReconcileStatu, param, statu, accessToken);
		}

		public MActionResult<ImportTemplateModel> GetImportTemplateModel(IVImportTransactionFilterModel param, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.GetImportTemplateModel, param, accessToken);
		}

		public MActionResult<List<IVReceiveModel>> GetReceiveListByFilter(IVReceiveListFilterModel filter, string accessToken = null)
		{
			IIVReceiveBusiness iIVReceiveBusiness = biz;
			return base.RunFunc(iIVReceiveBusiness.GetReceiveListIncludeEnetry, filter, accessToken);
		}
	}
}
