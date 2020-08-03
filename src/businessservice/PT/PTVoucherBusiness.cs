using JieNor.Megi.BusinessContract.PT;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.PT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.PT;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.PT
{
	public class PTVoucherBusiness : PTBaseBusiness, IPTVoucherBusiness, IDataContract<PTVoucherModel>
	{
		private readonly PTVoucherRepository dal = new PTVoucherRepository();

		public List<PTVoucherModel> GetList(MContext ctx)
		{
			List<PTVoucherModel> list = dal.GetList(ctx);
			if (list.Count((PTVoucherModel f) => f.MIsSys) == 0)
			{
				InsertDefaultData(ctx, ref list);
			}
			return list;
		}

		public OperationResult CheckNameExist(MContext ctx, PTVoucherModel model, ref string name)
		{
			OperationResult result = new OperationResult();
			if (model.MultiLanguage.Count > 0)
			{
				name = model.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
				result = dal.CheckNameExist(ctx, string.Empty, name);
			}
			return result;
		}

		public OperationResult Sort(MContext ctx, string ids)
		{
			OperationResult result = dal.Sort(ctx, ids);
			base.GetKeyValueList(ctx, "Voucher", true);
			return result;
		}

		public OperationResult InsertDefaultData(MContext ctx)
		{
			List<PTVoucherModel> list = null;
			return InsertDefaultData(ctx, ref list);
		}

		public OperationResult InsertDefaultData(MContext ctx, ref List<PTVoucherModel> psList)
		{
			OperationResult result = new OperationResult();
			if (psList == null)
			{
				List<PTVoucherModel> list = dal.GetList(ctx);
				if (list.Any())
				{
					return result;
				}
			}
			List<PTVoucherModel> presetModelList = GetPresetModelList(ctx);
			if (presetModelList != null)
			{
				foreach (PTVoucherModel item in presetModelList)
				{
					item.MItemID = UUIDHelper.GetGuid();
					item.MOrgID = ctx.MOrgID;
					item.IsNew = true;
					foreach (MultiLanguageFieldList item2 in item.MultiLanguage)
					{
						item2.MParentID = item.MItemID;
						foreach (MultiLanguageField item3 in item2.MMultiLanguageField)
						{
							item3.MPKID = UUIDHelper.GetGuid();
						}
					}
				}
				result = ModelInfoManager.InsertOrUpdate(ctx, presetModelList, null);
			}
			if (psList != null)
			{
				psList = dal.GetList(ctx);
			}
			return result;
		}

		public PTVoucherModel GetModel(MContext ctx, string itemID, bool isFromPrint = false)
		{
			PTVoucherModel pTVoucherModel = null;
			string key = string.Format("{0}_PrtSetting_{1}_{2}", ctx.MOrgID, "Voucher", ctx.MLCID);
			if (isFromPrint && ((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList).ContainsKey(key))
			{
				List<PTVoucherModel> source = ((ConcurrentDictionary<string, object>)PTBaseBusiness.cacheList)[key] as List<PTVoucherModel>;
				pTVoucherModel = source.SingleOrDefault((PTVoucherModel f) => f.MItemID == itemID);
			}
			if (pTVoucherModel == null)
			{
				pTVoucherModel = dal.GetModel(ctx, itemID);
			}
			return pTVoucherModel;
		}

		private List<PTVoucherModel> GetPresetModelList(MContext ctx)
		{
			return dal.GetModelList(ctx, new SqlWhere().Equal("MOrgID", "0").Equal("MIsSys", 1), false);
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, PTVoucherModel modelData, string fields = null)
		{
			OperationResult operationResult = new OperationResult();
			string text = string.Empty;
			if (modelData.MultiLanguage.Count > 0)
			{
				text = modelData.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
				operationResult = dal.CheckNameExist(ctx, modelData.MItemID, text);
				if (!operationResult.Success)
				{
					return operationResult;
				}
			}
			if (string.IsNullOrWhiteSpace(modelData.MItemID))
			{
				modelData.MSeq = 999;
			}
			ChangePTInfoByDirection(ctx, modelData);
			List<CommandInfo> list = new List<CommandInfo>();
			if (modelData.MIsDefault)
			{
				PTVoucherModel dataModelByFilter = dal.GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MIsDefault", 1));
				if (dataModelByFilter != null && dataModelByFilter.MItemID != modelData.MItemID)
				{
					dataModelByFilter.MIsDefault = false;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PTVoucherModel>(ctx, dataModelByFilter, new List<string>
					{
						"MIsDefault"
					}, true));
				}
			}
			List<string> fields2 = fields?.Split(',').ToList();
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<PTVoucherModel>(ctx, modelData, fields2, true));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			if (modelData.MultiLanguage.Count > 0)
			{
				operationResult.Tag = text;
			}
			base.GetKeyValueList(ctx, "Voucher", true);
			return operationResult;
		}

		private void ChangePTInfoByDirection(MContext ctx, PTVoucherModel model)
		{
			if (model.MIsAllowCustomize && model.MTemplateType == "A4_1_V_F")
			{
				PTVoucherModel pTVoucherModel = null;
				if (!string.IsNullOrWhiteSpace(model.MItemID))
				{
					pTVoucherModel = dal.GetModel(ctx, model.MItemID);
				}
				if (model.MPaperDirection == 1)
				{
					model.MPreviewImage = "pt-voucher-v1";
					if (pTVoucherModel != null && pTVoucherModel.MPaperDirection == 2 && pTVoucherModel.MUpOffset == -30m && model.MUpOffset == -30m)
					{
						model.MUpOffset = decimal.Zero;
					}
				}
				else
				{
					model.MPreviewImage = "pt-voucher-h1";
					if (model.MUpOffset == decimal.Zero && (pTVoucherModel == null || (pTVoucherModel != null && pTVoucherModel.MPaperDirection == 1 && pTVoucherModel.MUpOffset == decimal.Zero)))
					{
						model.MUpOffset = -30m;
					}
				}
			}
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<PTVoucherModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			OperationResult result = dal.DeleteModels(ctx, pkID);
			base.GetKeyValueList(ctx, "Voucher", true);
			return result;
		}

		public PTVoucherModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return GetDataModel(ctx, pkID, includeDelete, false);
		}

		public PTVoucherModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false, bool isFromBackup = false)
		{
			PTVoucherModel pTVoucherModel = null;
			if (string.IsNullOrWhiteSpace(pkID))
			{
				pTVoucherModel = dal.GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", "0").Equal("MIsSys", 1).Equal("MTemplateType", "A4_1_V_F"));
				MultiLanguageFieldList multiLanguageFieldList = pTVoucherModel.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName");
				if (!isFromBackup)
				{
					foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
					{
						item.MValue = string.Empty;
					}
				}
				foreach (MultiLanguageFieldList item2 in pTVoucherModel.MultiLanguage)
				{
					item2.MParentID = pTVoucherModel.MItemID;
					foreach (MultiLanguageField item3 in item2.MMultiLanguageField)
					{
						item3.MPKID = UUIDHelper.GetGuid();
					}
				}
				pTVoucherModel.MItemID = string.Empty;
				pTVoucherModel.MIsDefault = false;
				pTVoucherModel.MIsSys = false;
			}
			else
			{
				pTVoucherModel = dal.GetDataModel(ctx, pkID, includeDelete);
			}
			pTVoucherModel.TemplateTypeList = new List<NameValueModel>
			{
				new NameValueModel
				{
					MTag = pTVoucherModel.MTemplateType,
					MValue = COMMultiLangRepository.GetText(LangModule.GL, ctx.MLCID, pTVoucherModel.MTemplateType)
				}
			};
			pTVoucherModel.PaperTypeList = new List<NameValueModel>
			{
				new NameValueModel
				{
					MTag = pTVoucherModel.MPaperType.ToString(),
					MValue = COMMultiLangRepository.GetText(LangModule.Common, ctx.MLCID, pTVoucherModel.MPaperType.ToString())
				}
			};
			return pTVoucherModel;
		}

		private static string GetOrgRegAddress(MContext ctx)
		{
			BASOrganisationModel orgBasicInfo = new BASOrganisationBusiness().GetOrgBasicInfo(ctx);
			return (orgBasicInfo != null) ? orgBasicInfo.MRegAddress : string.Empty;
		}

		public PTVoucherModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<PTVoucherModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<PTVoucherModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
