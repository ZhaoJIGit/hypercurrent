using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDEmailTemplateBusiness : IBDEmailTemplateBusiness, IDataContract<BDEmailTemplateModel>
	{
		private readonly BDEmailTemplateRepository dal = new BDEmailTemplateRepository();

		public List<BDEmailTemplateModel> GetList(MContext ctx, EmailSendTypeEnum sendType)
		{
			if (sendType == EmailSendTypeEnum.Payslip && !ModelInfoManager.ExistsByFilter<BDEmailTemplateModel>(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID).Equal("MIsSys", 0).Equal("MType", sendType.ToString())))
			{
				InsertStandardEmailTmpl(ctx, sendType);
			}
			return dal.GetList(ctx, sendType);
		}

		public OperationResult InsertStandardEmailTmpl(MContext ctx, EmailSendTypeEnum sendType)
		{
			OperationResult result = new OperationResult();
			BDEmailTemplateModel dataModelByFilter = dal.GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", "0").Equal("MIsSys", 1).Equal("MType", sendType.ToString()));
			if (dataModelByFilter != null)
			{
				dataModelByFilter.MItemID = UUIDHelper.GetGuid();
				dataModelByFilter.MOrgID = ctx.MOrgID;
				dataModelByFilter.IsNew = true;
				dataModelByFilter.MIsSys = false;
				foreach (MultiLanguageFieldList item in dataModelByFilter.MultiLanguage)
				{
					item.MParentID = dataModelByFilter.MItemID;
					foreach (MultiLanguageField item2 in item.MMultiLanguageField)
					{
						item2.MPKID = UUIDHelper.GetGuid();
					}
				}
				result = dal.InsertOrUpdate(ctx, dataModelByFilter, null);
			}
			return result;
		}

		public OperationResult CheckNameExist(MContext ctx, BDEmailTemplateModel model, ref string name)
		{
			OperationResult result = new OperationResult();
			if (model.MultiLanguage.Count > 0)
			{
				name = model.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MName").MMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
				result = dal.CheckNameExist(ctx, model.MType, string.Empty, name);
			}
			return result;
		}

		public OperationResult Copy(MContext ctx, BDEmailTemplateModel model)
		{
			string empty = string.Empty;
			OperationResult operationResult = CheckNameExist(ctx, model, ref empty);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			BDEmailTemplateModel modelData = CreateCopyModel(ctx, model);
			operationResult = dal.InsertOrUpdate(ctx, modelData, null);
			operationResult.Tag = empty;
			return operationResult;
		}

		public BDEmailTemplateModel GetModel(MContext ctx, string itemID)
		{
			return dal.GetModel(ctx, itemID);
		}

		private BDEmailTemplateModel CreateCopyModel(MContext ctx, BDEmailTemplateModel model)
		{
			BDEmailTemplateModel dataModel = GetDataModel(ctx, model.MItemID, false);
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
						item2.MValue = mMultiLanguageField.SingleOrDefault((MultiLanguageField f) => f.MLocaleID == item2.MLocaleID).MValue;
					}
					item2.MPKID = UUIDHelper.GetGuid();
				}
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

		public OperationResult InsertOrUpdate(MContext ctx, BDEmailTemplateModel modelData, string fields = null)
		{
			OperationResult operationResult = dal.CheckNameExist(ctx, modelData.MType, modelData.MItemID, modelData.MName);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			BDEmailTemplateModel bDEmailTemplateModel = modelData;
			if (!string.IsNullOrWhiteSpace(modelData.MItemID))
			{
				bDEmailTemplateModel = ModelInfoManager.GetDataEditModel<BDEmailTemplateModel>(ctx, modelData.MItemID, false, true);
				if (bDEmailTemplateModel.MIsSys)
				{
					return operationResult;
				}
			}
			else
			{
				bDEmailTemplateModel.MOrgID = ctx.MOrgID;
				bDEmailTemplateModel.MSeq = 999;
				bDEmailTemplateModel.MIsSys = false;
			}
			setMultiField(bDEmailTemplateModel, "MName", modelData.MName);
			setMultiField(bDEmailTemplateModel, "MSubject", modelData.MSubject);
			setMultiField(bDEmailTemplateModel, "MContent", modelData.MContent);
			operationResult = dal.InsertOrUpdate(ctx, bDEmailTemplateModel, fields);
			operationResult.Tag = modelData.MName;
			return operationResult;
		}

		private void setMultiField(BDEmailTemplateModel model, string field, string value)
		{
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			if (model.MultiLanguage == null)
			{
				model.MultiLanguage = new List<MultiLanguageFieldList>();
			}
			if (!string.IsNullOrWhiteSpace(model.MItemID))
			{
				MultiLanguageFieldList multiLanguageFieldList = model.MultiLanguage.SingleOrDefault((MultiLanguageFieldList f) => f.MFieldName == field);
				if (multiLanguageFieldList != null)
				{
					foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
					{
						item.MValue = value;
					}
				}
			}
			else
			{
				MultiLanguageFieldList multiLanguageFieldList2 = new MultiLanguageFieldList();
				multiLanguageFieldList2.MFieldName = field;
				multiLanguageFieldList2.MMultiLanguageField = new List<MultiLanguageField>();
				string[] array = megiLangTypes;
				foreach (string mLocaleID in array)
				{
					multiLanguageFieldList2.MMultiLanguageField.Add(new MultiLanguageField
					{
						MLocaleID = mLocaleID,
						MValue = value
					});
				}
				model.MultiLanguage.Add(multiLanguageFieldList2);
			}
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDEmailTemplateModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			return dal.Delete(ctx, pkID);
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return new OperationResult();
		}

		public BDEmailTemplateModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		private static string GetOrgRegAddress(MContext ctx)
		{
			BASOrganisationModel orgBasicInfo = new BASOrganisationBusiness().GetOrgBasicInfo(ctx);
			return (orgBasicInfo != null) ? orgBasicInfo.MRegAddress : string.Empty;
		}

		public BDEmailTemplateModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDEmailTemplateModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDEmailTemplateModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
