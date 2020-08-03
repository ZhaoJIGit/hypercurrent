using JieNor.Megi.BusinessContract.PT;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.DataRepository.PT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.PT
{
	public class PTSalaryListBusiness : PTBaseBusiness, IPTSalaryListBusiness, IDataContract<PAPrintSettingModel>
	{
		private readonly PTSalaryListRepository dal = new PTSalaryListRepository();

		private readonly PAPayItemGroupRepository payItemGroupDal = new PAPayItemGroupRepository();

		public List<PAPrintSettingModel> GetList(MContext ctx)
		{
			List<PAPrintSettingModel> list = dal.GetList(ctx);
			if (list.Count == 0)
			{
				InsertStandardSetting(ctx, ref list);
			}
			List<PAPayItemGroupModel> salaryItemGroupList = PAPayItemGroupRepository.GetSalaryItemGroupList(ctx, PAPayItemGroupTypeEnum.NoChildLevel);
			foreach (PAPrintSettingModel item2 in list)
			{
				List<PAPrintSettingEntryModel> list2 = new List<PAPrintSettingEntryModel>();
				foreach (PAPayItemGroupModel item3 in salaryItemGroupList)
				{
					PAPrintSettingEntryModel pAPrintSettingEntryModel = item2.MEntryList.FirstOrDefault((PAPrintSettingEntryModel f) => f.MPayItemID == item3.MItemID);
					if (pAPrintSettingEntryModel != null)
					{
						pAPrintSettingEntryModel.MPayItemName = item3.MName;
						pAPrintSettingEntryModel.MPayItemType = item3.MItemType;
						list2.Add(pAPrintSettingEntryModel);
					}
				}
				item2.MEntryList = list2;
			}
			return list;
		}

		public OperationResult CheckNameExist(MContext ctx, PAPrintSettingModel model, ref string name)
		{
			OperationResult result = new OperationResult();
			if (model.MultiLanguage.Count > 0)
			{
				name = model.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
				result = dal.CheckNameExist(ctx, string.Empty, name);
			}
			return result;
		}

		public OperationResult Copy(MContext ctx, PAPrintSettingModel model, bool isCopyTmpl = false)
		{
			string empty = string.Empty;
			OperationResult operationResult = CheckNameExist(ctx, model, ref empty);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			PAPrintSettingModel pAPrintSettingModel = CreateCopyModel(ctx, model);
			if (isCopyTmpl)
			{
				operationResult = dal.CopySettingWithTemplate(ctx, pAPrintSettingModel, model.MItemID);
				new RPTReportBusiness().UpdateReportLayoutCache(ctx);
			}
			else
			{
				operationResult = dal.InsertOrUpdate(ctx, pAPrintSettingModel, null);
			}
			base.GetKeyValueList(ctx, "PayRun", true);
			operationResult.Tag = empty;
			return operationResult;
		}

		public OperationResult Sort(MContext ctx, string ids)
		{
			OperationResult result = dal.Sort(ctx, ids);
			base.GetKeyValueList(ctx, "PayRun", true);
			return result;
		}

		public OperationResult InsertStandardSetting(MContext ctx)
		{
			List<PAPrintSettingModel> list = null;
			return InsertStandardSetting(ctx, ref list);
		}

		public OperationResult InsertStandardSetting(MContext ctx, ref List<PAPrintSettingModel> psList)
		{
			OperationResult result = new OperationResult();
			if (psList == null)
			{
				List<PAPrintSettingModel> list = dal.GetList(ctx);
				if (list.Any())
				{
					return result;
				}
			}
			PAPrintSettingModel standardSetting = GetStandardSetting(ctx);
			if (standardSetting != null)
			{
				standardSetting.MItemID = UUIDHelper.GetGuid();
				standardSetting.MOrgID = ctx.MOrgID;
				standardSetting.IsNew = true;
				standardSetting.MIsSys = false;
				foreach (MultiLanguageFieldList item in standardSetting.MultiLanguage)
				{
					item.MParentID = standardSetting.MItemID;
					foreach (MultiLanguageField item2 in item.MMultiLanguageField)
					{
						item2.MPKID = UUIDHelper.GetGuid();
					}
				}
				SetEntryData(ctx, standardSetting);
				result = dal.InsertOrUpdate(ctx, standardSetting, null);
			}
			if (psList != null)
			{
				psList = dal.GetList(ctx);
			}
			return result;
		}

		public PAPrintSettingModel GetPrintSetting(MContext ctx, string itemID, bool isFromPrint = false)
		{
			PAPrintSettingModel pAPrintSettingModel = null;
			string key = string.Format("{0}_PrtSetting_{1}_{2}", ctx.MOrgID, "PayRun", ctx.MLCID);
			if (isFromPrint && ((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).ContainsKey(key))
			{
				List<PAPrintSettingModel> source = ((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList)[key] as List<PAPrintSettingModel>;
				pAPrintSettingModel = source.SingleOrDefault((PAPrintSettingModel f) => f.MItemID == itemID);
			}
			if (pAPrintSettingModel == null)
			{
				pAPrintSettingModel = GetDataModel(ctx, itemID, false);
			}
			return pAPrintSettingModel;
		}

		private PAPrintSettingModel GetStandardSetting(MContext ctx)
		{
			return ModelInfoManager.GetDataModelList<PAPrintSettingModel>(ctx, new SqlWhere().Equal("MOrgID", "0").Equal("MIsSys", 1), false, false).FirstOrDefault();
		}

		private PAPrintSettingModel CreateCopyModel(MContext ctx, PAPrintSettingModel model)
		{
			PAPrintSettingModel dataModel = GetDataModel(ctx, model.MItemID, false);
			dataModel.MItemID = UUIDHelper.GetGuid();
			dataModel.IsNew = true;
			dataModel.MSeq = 999;
			List<MultiLanguageField> mMultiLanguageField = model.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField;
			foreach (MultiLanguageFieldList item in dataModel.MultiLanguage)
			{
				item.MParentID = dataModel.MItemID;
				foreach (MultiLanguageField item2 in item.MMultiLanguageField)
				{
					if (item.MFieldName == "MName")
					{
						MultiLanguageField multiLanguageField = mMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == item2.MLocaleID);
						if (multiLanguageField != null)
						{
							item2.MValue = multiLanguageField.MValue;
						}
						else
						{
							item2.MValue = string.Empty;
						}
					}
					item2.MPKID = UUIDHelper.GetGuid();
				}
			}
			DateTime dateTime2 = dataModel.MModifyDate = (dataModel.MCreateDate = ctx.DateNow);
			dataModel.MCreatorID = ctx.MUserID;
			dataModel.MModifierID = ctx.MUserID;
			foreach (PAPrintSettingEntryModel mEntry in dataModel.MEntryList)
			{
				mEntry.IsNew = true;
				mEntry.MItemID = dataModel.MItemID;
				mEntry.MEntryID = UUIDHelper.GetGuid();
				mEntry.MCreateDate = dateTime2;
				mEntry.MModifyDate = dateTime2;
			}
			return dataModel;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, PAPrintSettingModel modelData, string fields = null)
		{
			OperationResult operationResult = new OperationResult();
			string text = string.Empty;
			bool flag = string.IsNullOrWhiteSpace(modelData.MItemID);
			if (modelData.MultiLanguage.Count > 0)
			{
				text = modelData.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
				operationResult = dal.CheckNameExist(ctx, modelData.MItemID, text);
				if (!operationResult.Success)
				{
					return operationResult;
				}
			}
			if (flag)
			{
				modelData.MSeq = 999;
			}
			List<string> list = string.IsNullOrWhiteSpace(fields) ? null : fields.Split(',').ToList();
			if (list == null)
			{
				SetEntryData(ctx, modelData);
			}
			List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<PAPrintSettingModel>(ctx, modelData, list, true);
			operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, insertOrUpdateCmd) > 0);
			if (modelData.MultiLanguage.Count > 0)
			{
				operationResult.Tag = text;
			}
			base.GetKeyValueList(ctx, "PayRun", true);
			return operationResult;
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<PAPrintSettingModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			OperationResult result = dal.DeletePrintSetting(ctx, pkID);
			base.GetKeyValueList(ctx, "PayRun", true);
			return result;
		}

		public PAPrintSettingModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			PAPrintSettingModel pAPrintSettingModel = null;
			if (string.IsNullOrWhiteSpace(pkID))
			{
				pAPrintSettingModel = GetStandardSetting(ctx);
				MultiLanguageFieldList multiLanguageFieldList = pAPrintSettingModel.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
				{
					item.MValue = string.Empty;
				}
				foreach (MultiLanguageFieldList item2 in pAPrintSettingModel.MultiLanguage)
				{
					item2.MParentID = pAPrintSettingModel.MItemID;
					foreach (MultiLanguageField item3 in item2.MMultiLanguageField)
					{
						item3.MPKID = UUIDHelper.GetGuid();
					}
				}
				SetEntryData(ctx, pAPrintSettingModel);
				pAPrintSettingModel.MItemID = string.Empty;
				pAPrintSettingModel.MIsSys = false;
			}
			else
			{
				List<PAPrintSettingModel> list = GetList(ctx);
				pAPrintSettingModel = list.FirstOrDefault((PAPrintSettingModel f) => f.MItemID == pkID);
			}
			return pAPrintSettingModel;
		}

		private void SetEntryData(MContext ctx, PAPrintSettingModel model)
		{
			List<PAPayItemGroupModel> salaryItemGroupList = PAPayItemGroupRepository.GetSalaryItemGroupList(ctx, PAPayItemGroupTypeEnum.NoChildLevel);
			List<PAPrintSettingEntryModel> list = new List<PAPrintSettingEntryModel>();
			foreach (PAPayItemGroupModel item in salaryItemGroupList)
			{
				PAPrintSettingEntryModel pAPrintSettingEntryModel = model.MEntryList?.FirstOrDefault((PAPrintSettingEntryModel f) => f.MPayItemID == item.MItemID);
				if (pAPrintSettingEntryModel == null)
				{
					pAPrintSettingEntryModel = new PAPrintSettingEntryModel();
					pAPrintSettingEntryModel.MIsShow = true;
					pAPrintSettingEntryModel.MPayItemID = item.MItemID;
					pAPrintSettingEntryModel.MPayItemName = item.MName;
				}
				pAPrintSettingEntryModel.MItemID = model.MItemID;
				pAPrintSettingEntryModel.MOrgID = ctx.MOrgID;
				list.Add(pAPrintSettingEntryModel);
			}
			model.MEntryList = list;
		}

		private static string GetOrgRegAddress(MContext ctx)
		{
			BASOrganisationModel orgBasicInfo = new BASOrganisationBusiness().GetOrgBasicInfo(ctx);
			return (orgBasicInfo != null) ? orgBasicInfo.MRegAddress : string.Empty;
		}

		public PAPrintSettingModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<PAPrintSettingModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<PAPrintSettingModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
