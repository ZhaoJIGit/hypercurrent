using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO.Import.Account;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDAccountMatchLogBusiness : IBDAccountMatchLogBusiness, IDataContract<BDAccountMatchLogModel>
	{
		private readonly BDAccountMatchLogRepository dal = new BDAccountMatchLogRepository();

		public List<IOAccountModel> GetMatchLogList(MContext ctx)
		{
			List<IOAccountModel> list = new List<IOAccountModel>();
			List<BDAccountMatchLogModel> modelList = GetModelList(ctx, new SqlWhere().Equal("MIsDelete", 0).Equal("MMatchResult", 3), false);
			modelList = (from f in modelList
			orderby f.MNumber
			select f).ToList();
			foreach (BDAccountMatchLogModel item in modelList)
			{
				if (!string.IsNullOrWhiteSpace(item.MParentNumber) && !modelList.Any((BDAccountMatchLogModel f) => f.MNumber != item.MNumber && f.MNumber == item.MParentNumber))
				{
					item.MParentNumber = string.Empty;
				}
			}
			ConvertToLogTreeModelList(list, modelList, null, null);
			return list;
		}

		public OperationResult SaveMatchLog(MContext ctx, List<BDAccountEditModel> acctList)
		{
			return SaveMatchLog(ctx, acctList, false);
		}

		public OperationResult SaveMatchLog(MContext ctx, List<BDAccountEditModel> acctList, bool isFromMigration = false)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> list = new List<CommandInfo>();
			List<BDAccountEditModel> list2 = ModelInfoManager.GetDataModelList<BDAccountEditModel>(ctx, new SqlWhere(), false, false);
			if (!isFromMigration)
			{
				list2 = (from f in list2
				where !f.MNumber.StartsWith("1001") && !f.MNumber.StartsWith("1002")
				select f).ToList();
			}
			GetUpdateMatchLogCmds(ctx, acctList, list, list2, false);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			return operationResult;
		}

		public void GetUpdateMatchLogCmds(MContext ctx, List<BDAccountEditModel> acctList, List<CommandInfo> commandList, List<BDAccountEditModel> sysAcctList, bool isFromMigration = false)
		{
			List<BDAccountMatchLogModel> list = new List<BDAccountMatchLogModel>();
			List<BDAccountMatchLogModel> source = ModelInfoManager.GetDataModelList<BDAccountMatchLogModel>(ctx, new SqlWhere().Equal("MIsDelete", 0), false, false);
			List<string> list2 = new List<string>();
			DateTime dateNow = ctx.DateNow;
			if (isFromMigration)
			{
				list2 = (from f in source
				select f.MItemID).ToList();
				source = new List<BDAccountMatchLogModel>();
			}
			foreach (BDAccountEditModel acct in acctList)
			{
				if (acct.MatchResult == IOAccountMatchResultEnum.ManualMatch)
				{
					if (string.IsNullOrWhiteSpace(acct.MMatchNumber) && string.IsNullOrWhiteSpace(acct.MNewNumber))
					{
						BDAccountMatchLogModel bDAccountMatchLogModel = source.FirstOrDefault((BDAccountMatchLogModel f) => f.MNumber == acct.MNumber && f.MName.ToUpper() == acct.MName.ToUpper());
						if (bDAccountMatchLogModel != null)
						{
							list2.Add(bDAccountMatchLogModel.MItemID);
						}
					}
					else
					{
						BDAccountMatchLogModel log = GetMatchLogModel(ctx, acct, sysAcctList, isFromMigration);
						BDAccountMatchLogModel bDAccountMatchLogModel2 = source.FirstOrDefault((BDAccountMatchLogModel f) => f.MNumber == log.MNumber && f.MName.ToUpper() == log.MName.ToUpper());
						log.MDate = dateNow;
						if (bDAccountMatchLogModel2 != null)
						{
							log.MItemID = bDAccountMatchLogModel2.MItemID;
							log.IsUpdate = true;
						}
						list.Add(log);
					}
				}
				else if (isFromMigration)
				{
					BDAccountMatchLogModel matchLogModel = GetMatchLogModel(ctx, acct, sysAcctList, isFromMigration);
					matchLogModel.MDate = dateNow;
					list.Add(matchLogModel);
				}
			}
			if (list.Any())
			{
				commandList.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list, null, true));
			}
			if (list2.Any())
			{
				commandList.AddRange(ModelInfoManager.GetDeleteFlagCmd<BDAccountMatchLogModel>(ctx, list2.Distinct().ToList()));
			}
		}

		private BDAccountMatchLogModel GetMatchLogModel(MContext ctx, BDAccountEditModel model, List<BDAccountEditModel> sysAcctList, bool isFromMigration = false)
		{
			BDAccountMatchLogModel bDAccountMatchLogModel = new BDAccountMatchLogModel();
			bDAccountMatchLogModel.MOrgID = ctx.MOrgID;
			bDAccountMatchLogModel.MAccountTableID = ctx.MAccountTableID;
			bDAccountMatchLogModel.MAccountTypeId = model.MAccountTypeID;
			bDAccountMatchLogModel.MIsCheckForCurrency = model.MIsCheckForCurrency;
			bDAccountMatchLogModel.MDC = model.MDC;
			bDAccountMatchLogModel.MMatchResult = (int)model.MatchResult;
			bDAccountMatchLogModel.MNumber = ((!string.IsNullOrWhiteSpace(model.MOriNumber)) ? model.MOriNumber : model.MNumber);
			bDAccountMatchLogModel.MName = ((!string.IsNullOrWhiteSpace(model.MOriName)) ? model.MOriName : model.MName);
			bDAccountMatchLogModel.MNewNumber = model.MNewNumber;
			bDAccountMatchLogModel.MParentNumber = model.MParentNumber;
			if (isFromMigration)
			{
				bDAccountMatchLogModel.MMigrationID = model.MMigrationID;
				bDAccountMatchLogModel.MSourceCheckType = model.MSourceCheckType;
				bDAccountMatchLogModel.MSourceCurrency = model.MSourceCurrency;
				bDAccountMatchLogModel.MSourceBillKey = 2;
				bDAccountMatchLogModel.MMegiID = model.MItemID;
				bDAccountMatchLogModel.MCheckType = model.MCheckGroupNames;
			}
			if (!string.IsNullOrWhiteSpace(model.MMatchNumber) && model.MMatchNumber != "-1")
			{
				string matchNumber = model.MMatchNumber.Split(' ')[0];
				bDAccountMatchLogModel.MSysNumber = matchNumber;
				BDAccountEditModel bDAccountEditModel = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MNumber == matchNumber);
				if (bDAccountEditModel != null)
				{
					bDAccountMatchLogModel.MSysDC = bDAccountEditModel.MDC;
					bDAccountMatchLogModel.MSysAccountTypeId = bDAccountEditModel.MAccountTypeID;
					List<string> list = new List<string>();
					SetFullName(list, bDAccountEditModel, sysAcctList);
					bDAccountMatchLogModel.MMatchNumber = string.Format("{0} {1}", matchNumber, string.Join("-", list));
				}
			}
			return bDAccountMatchLogModel;
		}

		private void SetFullName(List<string> fullNameList, BDAccountEditModel acct, List<BDAccountEditModel> sysAcctList)
		{
			fullNameList.Add(acct.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageValue);
			BDAccountEditModel bDAccountEditModel = sysAcctList.FirstOrDefault((BDAccountEditModel f) => f.MItemID == acct.MParentID);
			if (bDAccountEditModel != null)
			{
				SetFullName(fullNameList, bDAccountEditModel, sysAcctList);
			}
			else
			{
				fullNameList.Reverse();
			}
		}

		private IOAccountModel GetLogTreeModel(BDAccountMatchLogModel log)
		{
			return new IOAccountModel
			{
				id = log.MItemID,
				text = log.MName,
				MNumber = log.MNumber,
				MMatchNumber = log.MMatchNumber,
				MNewNumber = log.MNewNumber
			};
		}

		private void ConvertToLogTreeModelList(List<IOAccountModel> resultList, List<BDAccountMatchLogModel> logList, List<BDAccountMatchLogModel> parentLogList = null, IOAccountModel parentLog = null)
		{
			parentLogList = (parentLogList ?? (from f in logList
			where string.IsNullOrWhiteSpace(f.MParentNumber)
			select f).ToList());
			foreach (BDAccountMatchLogModel parentLog2 in parentLogList)
			{
				IOAccountModel logTreeModel = GetLogTreeModel(parentLog2);
				if (parentLog == null)
				{
					resultList.Add(logTreeModel);
				}
				else
				{
					if (parentLog.children == null)
					{
						parentLog.children = new List<IOAccountModel>();
					}
					parentLog.children.Add(logTreeModel);
				}
				List<BDAccountMatchLogModel> list = (from f in logList
				where f.MParentNumber == parentLog2.MNumber
				select f).ToList();
				if (list != null && list.Any())
				{
					ConvertToLogTreeModelList(resultList, logList, list, logTreeModel);
				}
			}
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDAccountMatchLogModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDAccountMatchLogModel> modelData, string fields = null)
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

		public BDAccountMatchLogModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BDAccountMatchLogModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDAccountMatchLogModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDAccountMatchLogModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
