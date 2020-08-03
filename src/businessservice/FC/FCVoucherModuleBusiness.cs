using JieNor.Megi.BusinessContract.FC;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.FC;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.FC
{
	public class FCVoucherModuleBusiness : IFCVoucherModuleBusiness, IDataContract<FCVoucherModuleModel>
	{
		private readonly FCVoucherModuleRepository dal = new FCVoucherModuleRepository();

		private readonly GLUtility utility = new GLUtility();

		public List<FCVoucherModuleModel> GetVoucherModuleListWithNoEntry(MContext ctx)
		{
			return dal.GetVoucherModuleListWithNoEntry(ctx);
		}

		public FCVoucherModuleModel GetVoucherModelWithEntry(MContext ctx, string PKID)
		{
			return dal.GetVoucherModelWithEntry(ctx, PKID);
		}

		public FCVoucherModuleModel UpdateVoucherModuleModel(MContext ctx, FCVoucherModuleModel model)
		{
			PreHanldeVoucherModules(ctx, new List<FCVoucherModuleModel>
			{
				model
			});
			ValidateVoucherModules(ctx, new List<FCVoucherModuleModel>
			{
				model
			});
			return dal.UpdateVoucherModuleModel(ctx, model);
		}

		public List<FCVoucherModuleModel> GetVoucherModuleList(MContext ctx, List<string> pkIDS)
		{
			return dal.GetVoucherModuleModelList(ctx, pkIDS);
		}

		public DataGridJson<FCVoucherModuleModel> GetVoucherModuleModelPageList(MContext ctx, GLVoucherListFilterModel filter)
		{
			DataGridJson<FCVoucherModuleModel> dataGridJson = new DataGridJson<FCVoucherModuleModel>();
			dataGridJson.rows = dal.GetVoucherModulePageList(ctx, filter, false);
			dataGridJson.total = dal.GetVoucherModulePageListCount(ctx, filter);
			return dataGridJson;
		}

		public FCVoucherModuleModel GetVoucherModule(MContext ctx, string pkID = null)
		{
			return dal.GetVoucherModule(ctx, pkID);
		}

		public void PreHanldeVoucherModules(MContext ctx, List<FCVoucherModuleModel> vouchers)
		{
			vouchers.ForEach(delegate(FCVoucherModuleModel x)
			{
				x.MOrgID = ctx.MOrgID;
				x.MVoucherModuleEntrys = (x.MVoucherModuleEntrys ?? new List<FCVoucherModuleEntryModel>());
				decimal debitTotal = default(decimal);
				decimal creditTotal = default(decimal);
				x.MVoucherModuleEntrys.ForEach(delegate(FCVoucherModuleEntryModel y)
				{
					y.MOrgID = ctx.MOrgID;
					y.MCurrencyID = (string.IsNullOrWhiteSpace(y.MCurrencyID) ? ctx.MBasCurrencyID : y.MCurrencyID);
					y.MExchangeRate = ((y.MCurrencyID == ctx.MBasCurrencyID) ? 1.0m : y.MExchangeRate);
					y.IsNew = string.IsNullOrWhiteSpace(y.MEntryID);
					y.MCheckGroupValueModel = (y.MCheckGroupValueModel ?? new GLCheckGroupValueModel());
					debitTotal += y.MDebit;
					creditTotal += y.MCredit;
					if (y.MCheckGroupValueModel != null)
					{
						y.MCheckGroupValueModel.MOrgID = ctx.MOrgID;
					}
				});
				x.MDebitTotal = debitTotal;
				x.MCreditTotal = creditTotal;
			});
		}

		public List<MActionResultCodeEnum> ValidateVoucherModules(MContext ctx, List<FCVoucherModuleModel> vouchers)
		{
			if (vouchers == null || vouchers.Count == 0)
			{
				return null;
			}
			ValidateQueryModel validateVoucherModuleFastCodeSql = utility.GetValidateVoucherModuleFastCodeSql(vouchers);
			List<FCVoucherModuleEntryModel> source = utility.Union((from x in vouchers
			select x.MVoucherModuleEntrys).ToList());
			List<string> ids = (from x in source
			select x.MAccountID).ToList();
			ValidateQueryModel validateCommonModelSql = utility.GetValidateCommonModelSql<BDAccountModel>(MActionResultCodeEnum.MAccountInvalid, ids, null, null);
			ValidateQueryModel validateAccountHasSubSql = utility.GetValidateAccountHasSubSql(ids, false);
			List<string> ids2 = (from x in source
			select x.MCheckGroupValueModel.MContactID).ToList();
			ValidateQueryModel validateCommonModelSql2 = utility.GetValidateCommonModelSql<BDContactsModel>(MActionResultCodeEnum.MContactInvalid, ids2, null, null);
			List<string> ids3 = (from x in source
			select x.MCheckGroupValueModel.MEmployeeID).ToList();
			ValidateQueryModel validateCommonModelSql3 = utility.GetValidateCommonModelSql<BDEmployeesModel>(MActionResultCodeEnum.MEmployeeInvalid, ids3, null, null);
			List<string> ids4 = (from x in source
			select x.MCheckGroupValueModel.MMerItemID).ToList();
			ValidateQueryModel validateCommonModelSql4 = utility.GetValidateCommonModelSql<BDItemModel>(MActionResultCodeEnum.MMerItemInvalid, ids4, null, null);
			List<string> ids5 = (from x in source
			select x.MCheckGroupValueModel.MExpItemID).ToList();
			ValidateQueryModel validateCommonModelSql5 = utility.GetValidateCommonModelSql<BDExpenseItemModel>(MActionResultCodeEnum.MExpItemInvalid, ids5, null, null);
			ValidateQueryModel validatExpItemHasSubSql = utility.GetValidatExpItemHasSubSql(ids5);
			List<string> ids6 = (from x in source
			select x.MCheckGroupValueModel into x
			where x.MPaItemGroupID != x.MPaItemID && !string.IsNullOrWhiteSpace(x.MPaItemID)
			select x.MPaItemGroupID).ToList();
			ValidateQueryModel validateCommonModelSql6 = utility.GetValidateCommonModelSql<PAPayItemGroupModel>(MActionResultCodeEnum.MPaItemInvalid, ids6, null, null);
			List<string> ids7 = (from x in source
			select x.MCheckGroupValueModel.MPaItemGroupID).ToList();
			ValidateQueryModel validateCommonModelSql7 = utility.GetValidateCommonModelSql<PAPayItemGroupModel>(MActionResultCodeEnum.MPaItemInvalid, ids7, null, null);
			List<List<string>> list = (from x in source
			select new List<string>
			{
				x.MCheckGroupValueModel.MTrackItem1,
				x.MCheckGroupValueModel.MTrackItem2,
				x.MCheckGroupValueModel.MTrackItem3,
				x.MCheckGroupValueModel.MTrackItem4,
				x.MCheckGroupValueModel.MTrackItem5
			}).ToList();
			List<string> ids8 = utility.Union(list);
			ValidateQueryModel validateCommonModelSql8 = utility.GetValidateCommonModelSql<BDTrackEntryModel>(MActionResultCodeEnum.MTrackItemInvalid, ids8, null, null);
			List<List<string>> list2 = (from x in source
			select new List<string>
			{
				x.MCheckGroupValueModel.MTrackItem1GroupID,
				x.MCheckGroupValueModel.MTrackItem2GroupID,
				x.MCheckGroupValueModel.MTrackItem3GroupID,
				x.MCheckGroupValueModel.MTrackItem4GroupID,
				x.MCheckGroupValueModel.MTrackItem5GroupID
			}).ToList();
			List<string> ids9 = utility.Union(list2);
			ValidateQueryModel validateCommonModelSql9 = utility.GetValidateCommonModelSql<BDTrackModel>(MActionResultCodeEnum.MTrackGroupInvalid, ids9, null, null);
			List<MActionResultCodeEnum> result = utility.QueryValidateSql(ctx, true, validateVoucherModuleFastCodeSql, validateCommonModelSql, validateAccountHasSubSql, validateCommonModelSql2, validateCommonModelSql3, validateCommonModelSql4, validateCommonModelSql5, validateCommonModelSql6, validateCommonModelSql8);
			if (!utility.CheckVoucherModuleCheckGroupValueMatchCheckGroup(ctx, vouchers))
			{
				throw new MActionException
				{
					Codes = new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MCheckGroupValueNotMatchWithAccount
					}
				};
			}
			return result;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FCVoucherModuleModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FCVoucherModuleModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public FCVoucherModuleModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FCVoucherModuleModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FCVoucherModuleModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FCVoucherModuleModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
