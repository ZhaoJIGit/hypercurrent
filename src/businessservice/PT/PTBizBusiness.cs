using JieNor.Megi.BusinessContract.PT;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.RPT;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.PT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace JieNor.Megi.BusinessService.PT
{
	public class PTBizBusiness : PTBaseBusiness, IPTBizBusiness, IDataContract<BDPrintSettingModel>
	{
		private readonly PTBizRepository dal = new PTBizRepository();

		public List<BDPrintSettingModel> GetList(MContext ctx)
		{
			List<BDPrintSettingModel> list = dal.GetList(ctx);
			if (list.Count == 0)
			{
				InsertStandardSetting(ctx, ref list);
			}
			return list;
		}

		public dynamic GetPTList(MContext ctx, bool isUpdate = false)
		{
			string text = string.Format("{0}_PrtSetting_{1}_{2}", ctx.MOrgID, "Voucher", ctx.MLCID);
			object obj = null;
			if (((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).ContainsKey(text) && !isUpdate)
			{
				obj = ((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList)[text];
			}
			else
			{
				obj = GetList(ctx);
				if (((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).ContainsKey(text))
				{
					((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList)[text] = obj;
				}
				else
				{
					//if (_003C_003Eo__2._003C_003Ep__0 == null)
					//{
					//	_003C_003Eo__2._003C_003Ep__0 = CallSite<Action<CallSite, ConcurrentDictionary<string, object>, string, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "TryAdd", null, typeof(PTBizBusiness), new CSharpArgumentInfo[3]
					//	{
					//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
					//		CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
					//	}));
					//}
					//_003C_003Eo__2._003C_003Ep__0.Target(_003C_003Eo__2._003C_003Ep__0, PTBaseBusiness.cacheList, text, obj);

					PTBaseBusiness.cacheList.TryAdd(text, obj);
				}
			}
			return obj;
		}

		public OperationResult CheckNameExist(MContext ctx, BDPrintSettingModel model, ref string name)
		{
			OperationResult operationResult = new OperationResult();
			List<MultiLanguageFieldList> langList = (from a in model.MultiLanguage
			where a.MFieldName == "MName"
			select a).ToList();
			if (ModelInfoManager.IsLangColumnValueExists<BDPrintSettingModel>(ctx, "MName", langList, model.MItemID, "", "", false))
			{
				operationResult.Success = false;
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PrintSettingExist", "A Print Setting with the same name already exists.");
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text
				});
				return operationResult;
			}
			return operationResult;
		}

		public OperationResult Copy(MContext ctx, BDPrintSettingModel model, bool isCopyTmpl = false)
		{
			string empty = string.Empty;
			OperationResult operationResult = CheckNameExist(ctx, model, ref empty);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			BDPrintSettingModel bDPrintSettingModel = CreateCopyModel(ctx, model, isCopyTmpl);
			if (isCopyTmpl)
			{
				operationResult = dal.CopySettingWithTemplate(ctx, bDPrintSettingModel, model.MItemID);
				new RPTReportBusiness().UpdateReportLayoutCache(ctx);
			}
			else
			{
				operationResult = dal.InsertOrUpdate(ctx, bDPrintSettingModel, null);
			}
			base.GetKeyValueList(ctx, string.Empty, true);
			operationResult.Tag = empty;
			return operationResult;
		}

		public OperationResult Sort(MContext ctx, string ids)
		{
			OperationResult result = dal.Sort(ctx, ids);
			base.GetKeyValueList(ctx, string.Empty, true);
			return result;
		}

		public OperationResult InsertStandardSetting(MContext ctx)
		{
			List<BDPrintSettingModel> list = null;
			return InsertStandardSetting(ctx, ref list);
		}

		public OperationResult InsertStandardSetting(MContext ctx, ref List<BDPrintSettingModel> psList)
		{
			OperationResult result = new OperationResult();
			if (psList == null)
			{
				List<BDPrintSettingModel> list = dal.GetList(ctx);
				if (list.Any())
				{
					return result;
				}
			}
			BDPrintSettingModel standardSetting = GetStandardSetting(ctx);
			if (standardSetting != null)
			{
				standardSetting.MItemID = UUIDHelper.GetGuid();
				standardSetting.MOrgID = ctx.MOrgID;
				standardSetting.IsNew = true;
				standardSetting.MIsSys = false;
				standardSetting.MContactDetails = GetOrgRegAddress(ctx);
				foreach (MultiLanguageFieldList item in standardSetting.MultiLanguage)
				{
					item.MParentID = standardSetting.MItemID;
					foreach (MultiLanguageField item2 in item.MMultiLanguageField)
					{
						item2.MPKID = UUIDHelper.GetGuid();
					}
				}
				result = dal.InsertOrUpdate(ctx, standardSetting, null);
			}
			if (psList != null)
			{
				psList = dal.GetList(ctx);
			}
			return result;
		}

		public BDPrintSettingModel GetPrintSetting(MContext ctx, string itemID, bool isFromPrint = false)
		{
			BDPrintSettingModel bDPrintSettingModel = null;
			string key = $"{ctx.MOrgID}_PrtSetting_{string.Empty}_{ctx.MLCID}";
			if (isFromPrint && ((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).ContainsKey(key))
			{
				List<BDPrintSettingModel> source = ((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList)[key] as List<BDPrintSettingModel>;
				bDPrintSettingModel = source.SingleOrDefault((BDPrintSettingModel f) => f.MItemID == itemID);
			}
			if (bDPrintSettingModel == null)
			{
				bDPrintSettingModel = dal.GetPrintSetting(ctx, itemID);
			}
			return bDPrintSettingModel;
		}

		private BDPrintSettingModel GetStandardSetting(MContext ctx)
		{
			return dal.GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", "0").Equal("MIsSys", 1));
		}

		private BDPrintSettingModel CreateCopyModel(MContext ctx, BDPrintSettingModel model, bool isCopyTmpl = false)
		{
			List<MultiLanguageField> source = new List<MultiLanguageField>();
			if (model.MultiLanguage != null && model.MultiLanguage.Any())
			{
				source = model.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField;
			}
			bool flag = isCopyTmpl && !source.Any();
			BDPrintSettingModel dataModel = GetDataModel(ctx, model.MItemID, false, flag);
			dataModel.MItemID = UUIDHelper.GetGuid();
			dataModel.IsNew = true;
			dataModel.MSeq = 999;
			foreach (MultiLanguageFieldList item in dataModel.MultiLanguage)
			{
				item.MParentID = dataModel.MItemID;
				foreach (MultiLanguageField item2 in item.MMultiLanguageField)
				{
					if (item.MFieldName == "MName")
					{
						MultiLanguageField multiLanguageField = source.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == item2.MLocaleID);
						if (multiLanguageField != null)
						{
							item2.MValue = multiLanguageField.MValue;
						}
						else if (flag)
						{
							MultiLanguageField multiLanguageField2 = item2;
							multiLanguageField2.MValue += ctx.DateNow.ToString(" yyyy-MM-dd HH:mm:ss");
						}
						else
						{
							item2.MValue = string.Empty;
						}
					}
					item2.MPKID = UUIDHelper.GetGuid();
				}
			}
			dataModel.MCreateDate = ctx.DateNow;
			dataModel.MModifyDate = ctx.DateNow;
			dataModel.MCreatorID = ctx.MUserID;
			dataModel.MModifierID = ctx.MUserID;
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

		public OperationResult InsertOrUpdate(MContext ctx, BDPrintSettingModel modelData, string fields = null)
		{
			OperationResult operationResult = new OperationResult();
			string empty = string.Empty;
			if (modelData.MultiLanguage.Count > 0)
			{
				List<MultiLanguageFieldList> langList = (from a in modelData.MultiLanguage
				where a.MFieldName == "MName"
				select a).ToList();
				if (ModelInfoManager.IsLangColumnValueExists<BDPrintSettingModel>(ctx, "MName", langList, modelData.MItemID, "", "", false))
				{
					operationResult.Success = false;
					string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "PrintSettingExist", "A Print Setting with the same name already exists.");
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = text
					});
					return operationResult;
				}
			}
			if (string.IsNullOrWhiteSpace(fields) && string.IsNullOrWhiteSpace(modelData.MContactDetails))
			{
				modelData.MContactDetails = GetOrgRegAddress(ctx);
			}
			if (string.IsNullOrWhiteSpace(modelData.MItemID))
			{
				modelData.MSeq = 999;
			}
			operationResult = dal.InsertOrUpdate(ctx, modelData, fields);
			if (modelData.MultiLanguage.Count > 0)
			{
				operationResult.Tag = empty;
			}
			base.GetKeyValueList(ctx, string.Empty, true);
			return operationResult;
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDPrintSettingModel> modelData, string fields = null)
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
			base.GetKeyValueList(ctx, string.Empty, true);
			return result;
		}

		public BDPrintSettingModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return GetDataModel(ctx, pkID, includeDelete, false);
		}

		public BDPrintSettingModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false, bool isFromBackup = false)
		{
			if (string.IsNullOrWhiteSpace(pkID))
			{
				BDPrintSettingModel standardSetting = GetStandardSetting(ctx);
				MultiLanguageFieldList multiLanguageFieldList = standardSetting.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				if (!isFromBackup)
				{
					foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
					{
						item.MValue = string.Empty;
					}
				}
				foreach (MultiLanguageFieldList item2 in standardSetting.MultiLanguage)
				{
					item2.MParentID = standardSetting.MItemID;
					foreach (MultiLanguageField item3 in item2.MMultiLanguageField)
					{
						item3.MPKID = UUIDHelper.GetGuid();
					}
				}
				standardSetting.MItemID = string.Empty;
				standardSetting.MIsSys = false;
				standardSetting.MContactDetails = GetOrgRegAddress(ctx);
				return standardSetting;
			}
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		private static string GetOrgRegAddress(MContext ctx)
		{
			BASOrganisationModel orgBasicInfo = new BASOrganisationBusiness().GetOrgBasicInfo(ctx);
			return (orgBasicInfo != null) ? orgBasicInfo.MRegAddress : string.Empty;
		}

		public BDPrintSettingModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDPrintSettingModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDPrintSettingModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
