using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.IV;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLDocVoucherBusiness : IGLDocVoucherBusiness, IDataContract<GLDocVoucherModel>
	{
		private readonly GLDocVoucherRepository dal = new GLDocVoucherRepository();

		private readonly IVInvoiceBusiness invoiceBiz = new IVInvoiceBusiness();

		private readonly IVExpenseBusiness expenseBiz = new IVExpenseBusiness();

		private readonly IVPaymentBusiness paymentBiz = new IVPaymentBusiness();

		private readonly IVReceiveBusiness receiveBiz = new IVReceiveBusiness();

		private readonly GLVoucherBusiness voucherBiz = new GLVoucherBusiness();

		private readonly IVTransferBusiness transferBiz = new IVTransferBusiness();

		public DataGridJson<GLDocEntryVoucherModel> GetDocVoucherModelList(MContext ctx, GLDocVoucherFilterModel filter)
		{
			if (filter.MDocIDs != null && filter.MDocIDs.Count > 0)
			{
				filter.rows = filter.MDocIDs.Count;
			}
			DataGridJson<GLDocEntryVoucherModel> dataGridJson = new DataGridJson<GLDocEntryVoucherModel>();
			dataGridJson.rows = dal.GetDocEntryVoucherModelListByFilter(ctx, filter);
			dataGridJson.total = dal.GetDocEntryVoucherModelListCountByFilter(ctx, filter);
			return dataGridJson;
		}

		public List<GLDocVoucherModel> GetDocVoucherList(MContext ctx, int? year, int? period, int? type, int? status)
		{
			return dal.GetDocVoucherList(ctx, year, period, type, status);
		}

		public List<GLDocVoucherModel> GetDocVoucherByVoucherID(MContext ctx, string voucherID)
		{
			return dal.GetDocVoucherByVoucherID(ctx, voucherID);
		}

		public List<GLDocVoucherModel> GetDocVoucherByDocID(MContext ctx, string docID)
		{
			return dal.GetModelList(ctx, new SqlWhere().Equal("MDocID", docID), false);
		}

		public OperationResult DeleteDocVoucher(MContext ctx, List<GLDocEntryVoucherModel> list)
		{
			List<string> pkIDS = (from x in list
			select x.MVoucherID).ToList();
			return voucherBiz.DeleteVoucherModels(ctx, pkIDS);
		}

		public OperationResult ResetDocVoucher(MContext ctx, List<string> docIDs, int docType)
		{
			OperationResult result = new OperationResult
			{
				Success = true
			};
			if (docIDs == null || docIDs.Count == 0)
			{
				return result;
			}
			docIDs = docIDs.Distinct().ToList();
			return dal.ResetDocVoucher(ctx, docIDs, docType);
		}

		public void PrehandleDocVoucherAccount(MContext ctx, List<GLDocEntryVoucherModel> list)
		{
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			List<BDAccountModel> accountIncludeDisable = instance.AccountIncludeDisable;
			if (list != null && list.Count != 0)
			{
				int i;
				for (i = 0; i < list.Count; i++)
				{
					if (!string.IsNullOrWhiteSpace(list[i].MDebitAccountID) && !accountIncludeDisable.Exists((BDAccountModel x) => x.MItemID == list[i].MDebitAccountID))
					{
						list[i].MDebitAccountID = null;
					}
					if (!string.IsNullOrWhiteSpace(list[i].MCreditAccountID) && !accountIncludeDisable.Exists((BDAccountModel x) => x.MItemID == list[i].MCreditAccountID))
					{
						list[i].MCreditAccountID = null;
					}
					if (!string.IsNullOrWhiteSpace(list[i].MTaxAccountID) && !accountIncludeDisable.Exists((BDAccountModel x) => x.MItemID == list[i].MTaxAccountID))
					{
						list[i].MTaxAccountID = null;
					}
				}
			}
		}

		public OperationResult CreateDocVoucher(MContext ctx, List<GLDocEntryVoucherModel> list, bool create)
		{
			OperationResult operationResult = new OperationResult();
			PrehandleDocVoucherAccount(ctx, list);
			return dal.CreateDocVoucher(ctx, list, create);
		}

		public List<string> GetUpdatedDocTable(MContext ctx, DateTime lastQueryTime)
		{
			return dal.GetUpdatedDocTable(ctx, lastQueryTime);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, GLDocVoucherModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLDocVoucherModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public GLDocVoucherModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLDocVoucherModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLDocVoucherModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<GLDocVoucherModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
