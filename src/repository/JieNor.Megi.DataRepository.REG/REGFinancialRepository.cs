using JieNor.Megi.Common.Context;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.REG
{
	public class REGFinancialRepository : DataServiceT<REGFinancialModel>
	{
		public OperationResult UpdateByOrgID(MContext ctx, REGFinancialModel model)
		{
			OperationResult operationResult = new OperationResult();
			model.MAppID = ctx.MAppID;
			model.MOrgID = ctx.MOrgID;
			REGFinancialModel byOrgID = GetByOrgID(ctx, model);
			if (byOrgID != null)
			{
				model.MItemID = byOrgID.MItemID;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<REGFinancialModel>(ctx, model, null, true));
			if (ctx.MRegProgress < 15)
			{
				List<REGTaxRateModel> list2 = new List<REGTaxRateModel>();
				List<REGTaxRateModel> modelList = new REGTaxRateRepository().GetModelList(ctx, null, false);
				switch (Convert.ToInt32(model.MTaxPayer))
				{
				case 1:
					AddTaxRateModel(list2, ctx, "basic tax rate", 17m, modelList);
					AddTaxRateModel(list2, ctx, "basic tax rate(2018)", 16m, modelList);
					AddTaxRateModel(list2, ctx, "low tax rate", 13m, modelList);
					AddTaxRateModel(list2, ctx, "transport tax rate", 11m, modelList);
					AddTaxRateModel(list2, ctx, "transport tax rate(2018)", 10m, modelList);
					AddTaxRateModel(list2, ctx, "modernservice tax rate", 6m, modelList);
					AddTaxRateModel(list2, ctx, "zero tax rate", decimal.Zero, modelList);
					break;
				case 2:
					AddTaxRateModel(list2, ctx, "smallscale tax rate", 3m, modelList);
					AddTaxRateModel(list2, ctx, "zero tax rate", decimal.Zero, modelList);
					break;
				}
				if (list2.Any())
				{
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmds(ctx, list2, null, true));
				}
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			if (operationResult.Success)
			{
				ctx.MTaxCode = model.MTaxNo;
				ContextHelper.MContext = ctx;
			}
			return operationResult;
		}

		public REGFinancialModel GetByOrgID(MContext ctx, REGFinancialModel model)
		{
			List<REGFinancialModel> modelList = GetModelList(ctx, new SqlWhere().Equal("MOrgID", model.MOrgID), false);
			if (modelList.Count > 0)
			{
				REGFinancialModel rEGFinancialModel = modelList[0];
				ctx.IsSys = true;
				BASOrgInitSettingModel dataModelByFilter = new BASOrgInitSettingRepository().GetDataModelByFilter(ctx, new SqlWhere().Equal("MOrgID", ctx.MOrgID));
				DateTime mConversionDate;
				if (dataModelByFilter != null)
				{
					REGFinancialModel rEGFinancialModel2 = rEGFinancialModel;
					mConversionDate = dataModelByFilter.MConversionDate;
					rEGFinancialModel2.MConversionDate = mConversionDate.ToString("yyyy/MM/dd");
					rEGFinancialModel.MAccountingStandard = dataModelByFilter.MAccountingStandard;
				}
				if (string.IsNullOrWhiteSpace(rEGFinancialModel.MConversionDate))
				{
					BASOrganisationModel dataEditModel = ModelInfoManager.GetDataEditModel<BASOrganisationModel>(ctx, ctx.MOrgID, false, true);
					if (dataEditModel.MConversionDate != DateTime.MinValue)
					{
						REGFinancialModel rEGFinancialModel3 = rEGFinancialModel;
						mConversionDate = dataEditModel.MConversionDate;
						rEGFinancialModel3.MConversionDate = mConversionDate.ToString("yyyy/MM/dd");
					}
				}
				ctx.IsSys = false;
				return rEGFinancialModel;
			}
			return null;
		}

		private void AddTaxRateModel(List<REGTaxRateModel> list, MContext ctx, string trName, decimal taxRate, List<REGTaxRateModel> taxRateInDB)
		{
			REGTaxRateModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<REGTaxRateModel>(ctx);
			emptyDataEditModel.MTaxRate = taxRate;
			setMultiField(emptyDataEditModel, "MName", trName.Replace(" ", "-"));
			if (!TaxRateIsExist(taxRateInDB, emptyDataEditModel))
			{
				emptyDataEditModel.MOrgID = ctx.MOrgID;
				emptyDataEditModel.MAppID = "1";
				emptyDataEditModel.MIsSysData = true;
				emptyDataEditModel.MTaxRate = taxRate;
				emptyDataEditModel.MEffectiveTaxRate = taxRate;
				emptyDataEditModel.TaxRateDetail = new List<REGTaxRateEntryModel>();
				REGTaxRateEntryModel rEGTaxRateEntryModel = new REGTaxRateEntryModel();
				rEGTaxRateEntryModel.MOrgID = ctx.MOrgID;
				rEGTaxRateEntryModel.MName = trName;
				rEGTaxRateEntryModel.MTaxRate = taxRate;
				emptyDataEditModel.TaxRateDetail.Add(rEGTaxRateEntryModel);
				list.Add(emptyDataEditModel);
			}
		}

		private bool TaxRateIsExist(List<REGTaxRateModel> taxRateListInDB, REGTaxRateModel taxRate)
		{
			bool result = false;
			foreach (REGTaxRateModel item in taxRateListInDB)
			{
				if (!(taxRate.MTaxRate != taxRate.MTaxRate))
				{
					foreach (MultiLanguageField item2 in taxRate.MultiLanguage[0].MMultiLanguageField)
					{
						if (item.MultiLanguage[0].MMultiLanguageField.Any((MultiLanguageField x) => x.MValue.ToLower() == item2.MValue.ToLower()))
						{
							return true;
						}
					}
				}
			}
			return result;
		}

		private void setMultiField<T>(T trModel, string multiKey, string multiName) where T : BaseModel
		{
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = megiLangTypes;
			foreach (string text in array)
			{
				dictionary.Add(text, COMMultiLangRepository.GetText(LangModule.Bank, text, multiName));
			}
			MultiLanguageFieldList multiLanguageFieldList = trModel.MultiLanguage.Find((Predicate<MultiLanguageFieldList>)((MultiLanguageFieldList p) => p.MFieldName.Equals(multiKey)));
			if (multiLanguageFieldList != null)
			{
				MultiLanguageField multiLanguageField = null;
				foreach (string key in dictionary.Keys)
				{
					multiLanguageField = multiLanguageFieldList.MMultiLanguageField.SingleOrDefault((MultiLanguageField p) => p.MLocaleID.Equals(key, StringComparison.OrdinalIgnoreCase));
					if (multiLanguageField == null)
					{
						multiLanguageField = new MultiLanguageField();
						multiLanguageField.MLocaleID = key;
					}
					multiLanguageField.MValue = dictionary[key];
				}
			}
		}
	}
}
