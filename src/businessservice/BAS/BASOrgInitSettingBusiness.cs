using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.Common.Context;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASOrgInitSettingBusiness : IBASOrgInitSettingBusiness, IDataContract<BASOrgInitSettingModel>
	{
		private readonly BASOrgInitSettingRepository dal = new BASOrgInitSettingRepository();

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			ctx.IsSys = true;
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			ctx.IsSys = true;
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BASOrgInitSettingModel modelData, string fields = null)
		{
			OperationResult operationResult = dal.UpdateInitSettingModel(ctx, modelData, fields);
			if (operationResult.Success && modelData.MAccountingStandard != "3")
			{
				operationResult = BankAccountSynchronous(ctx);
				ctx.IsSys = false;
				GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
				gLInitBalanceRepository.DeleteInitBalance(ctx);
			}
			return operationResult;
		}

		private OperationResult BankAccountSynchronous(MContext ctx)
		{
			BDBankAccountRepository bDBankAccountRepository = new BDBankAccountRepository();
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MIsDelete", 0);
			sqlWhere.Equal("MIsActive", 1);
			List<BDBankAccountEditModel> modelList = bDBankAccountRepository.GetModelList(ctx, sqlWhere, false);
			List<string> list = null;
			List<BDAccountModel> source = new List<BDAccountModel>();
			if (modelList?.Any() ?? false)
			{
				list = (from f in modelList
				select f.MItemID).ToList();
				source = ModelInfoManager.GetDataModelList<BDAccountModel>(ctx, new SqlWhere().In("MItemID", list).Equal("MIsDelete", 1), false, false);
			}
			List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, null, false, list);
			BDAccountModel bDAccountModel = (from x in baseBDAccountList
			where x.MNumber == "1001" && x.MIsSys
			select x).FirstOrDefault();
			BDAccountModel bDAccountModel2 = (from x in baseBDAccountList
			where x.MNumber == "1002" && x.MIsSys
			select x).FirstOrDefault();
			List<CommandInfo> list2 = new List<CommandInfo>();
			if (modelList != null && bDAccountModel != null && bDAccountModel2 != null)
			{
				modelList = (from f in modelList
				orderby f.MCreateDate
				select f).ToList();
				modelList = (from x in modelList
				where string.IsNullOrEmpty(x.MAccountID)
				select x).ToList();
				List<string> list3 = new List<string>();
				list3.Add("MAccountID");
				List<string> list4 = new List<string>();
				list4.Add("MParentID");
				list4.Add("MNumber");
				list4.Add("MDC");
				list4.Add("MAccountTypeID");
				list4.Add("MAccountGroupID");
				list4.Add("MAccountTableID");
				list4.Add("MCode");
				list4.Add("MCheckGroupID");
				int num = 1;
				int num2 = 1;
				BDAccountBusiness bDAccountBusiness = new BDAccountBusiness();
				BDAccountModel dataModelByFilter = bDAccountRepository.GetDataModelByFilter(ctx, new SqlWhere().Equal("MCode", "1001"));
				BDAccountModel dataModelByFilter2 = bDAccountRepository.GetDataModelByFilter(ctx, new SqlWhere().Equal("MCode", "1002"));
				foreach (BDBankAccountEditModel item in modelList)
				{
					BDAccountModel bDAccountModel3 = (from x in baseBDAccountList
					where x.MItemID == item.MItemID
					select x).FirstOrDefault();
					if (bDAccountModel3 != null)
					{
						item.MAccountID = bDAccountModel3.MItemID;
						list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, item, list3, true));
						if (bDAccountModel3.MIsDelete)
						{
							BDAccountModel bDAccountModel4 = source.FirstOrDefault((BDAccountModel f) => f.MItemID == item.MItemID);
							if (bDAccountModel4 != null && bDAccountModel4.MultiLanguage != null)
							{
								bDAccountModel3.MultiLanguage = bDAccountModel4.MultiLanguage;
								foreach (MultiLanguageFieldList item2 in bDAccountModel3.MultiLanguage)
								{
									foreach (MultiLanguageField item3 in item2.MMultiLanguageField)
									{
										item3.MIsDelete = false;
									}
								}
							}
						}
					}
					else
					{
						item.MAccountID = item.MItemID;
						list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDBankAccountEditModel>(ctx, item, list3, true));
						bDAccountModel3 = new BDAccountModel();
						bDAccountModel3.MItemID = item.MItemID;
						bDAccountModel3.MIsActive = true;
						bDAccountModel3.MIsDelete = false;
						bDAccountModel3.MOrgID = ctx.MOrgID;
						bDAccountModel3.MultiLanguage = BDBankAccountBusiness.GetAccountMulitLang(item.MultiLanguage, bDAccountModel3.MultiLanguage);
						bDAccountModel3.IsNew = true;
					}
					bDAccountModel3.MCheckGroupID = "0";
					if (item.MBankAccountType == 3)
					{
						string text = (num2 < 10) ? ("0" + Convert.ToString(num2)) : Convert.ToString(num2);
						bDAccountModel3.MParentID = bDAccountModel.MItemID;
						bDAccountModel3.MDC = bDAccountModel.MDC;
						bDAccountModel3.MAccountTypeID = bDAccountModel.MAccountTypeID;
						bDAccountModel3.MAccountGroupID = bDAccountModel.MAccountGroupID;
						bDAccountModel3.MAccountTableID = bDAccountModel.MAccountTableID;
						bDAccountModel3.MNumber = bDAccountModel.MNumber + "." + text;
						bDAccountModel3.MIsCheckForCurrency = (item.MCyID != ctx.MBasCurrencyID);
						bDAccountModel3.MCode = bDAccountModel.MCode + text;
						num2++;
					}
					else
					{
						string text2 = (num < 10) ? ("0" + Convert.ToString(num)) : Convert.ToString(num);
						bDAccountModel3.MParentID = bDAccountModel2.MItemID;
						bDAccountModel3.MDC = bDAccountModel2.MDC;
						bDAccountModel3.MAccountTypeID = bDAccountModel2.MAccountTypeID;
						bDAccountModel3.MAccountGroupID = bDAccountModel2.MAccountGroupID;
						bDAccountModel3.MAccountTableID = bDAccountModel2.MAccountTableID;
						bDAccountModel3.MNumber = bDAccountModel2.MNumber + "." + text2;
						bDAccountModel3.MIsCheckForCurrency = (item.MCyID != ctx.MBasCurrencyID);
						bDAccountModel3.MCode = bDAccountModel2.MCode + text2;
						num++;
					}
					bDAccountModel3.MIsDelete = false;
					bDAccountModel3.MIsActive = true;
					list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDAccountModel>(ctx, bDAccountModel3, null, true));
					BDAccountModel parentModel = (item.MBankAccountType == 3) ? dataModelByFilter : dataModelByFilter2;
					list2.AddRange(bDAccountBusiness.GetUpdateAccountFullNameCmds(ctx, bDAccountModel3, parentModel, null));
				}
				if (source.Any())
				{
					list2.AddRange(bDAccountRepository.GetRestoreBankAccountCmds(ctx, list));
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num3 = dynamicDbHelperMySQL.ExecuteSqlTran(list2);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = (list2.Count() == 0 || num3 > 0);
			return operationResult;
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BASOrgInitSettingModel> modelData, string fields = null)
		{
			ctx.IsSys = true;
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			ctx.IsSys = true;
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			ctx.IsSys = true;
			return dal.DeleteModels(ctx, pkID);
		}

		public BASOrgInitSettingModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			ctx.IsSys = true;
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BASOrgInitSettingModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			ctx.IsSys = true;
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BASOrgInitSettingModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			ctx.IsSys = true;
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BASOrgInitSettingModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			ctx.IsSys = true;
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public OperationResult GLSetupSuccess(MContext ctx)
		{
			OperationResult operationResult = new OperationResult();
			if (!SECPermissionRepository.HavePermission(ctx, "General_Ledger", "View", ""))
			{
				operationResult = dal.GLSetupSuccess(ctx);
				if (operationResult.Success)
				{
					ContextHelper.UpdateMContextByKeyField("MOrgID", ctx.MOrgID, "MEnabledModules", new int[2]
					{
						ModuleEnum.Sales.ToMInt32(),
						ModuleEnum.GL.ToMInt32()
					}, true);
				}
			}
			return operationResult;
		}
	}
}
