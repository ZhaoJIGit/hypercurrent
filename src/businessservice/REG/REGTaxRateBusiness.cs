using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JieNor.Megi.BusinessService.REG
{
	public class REGTaxRateBusiness : APIBusinessBase<REGTaxRateModel>, IREGTaxRateBusiness, IDataContract<REGTaxRateModel>
	{
		private readonly REGTaxRateRepository dal = new REGTaxRateRepository();

		private readonly BDAccountRepository accountRepository = new BDAccountRepository();

		protected override DataGridJson<REGTaxRateModel> OnGet(MContext ctx, GetParam param)
		{
			return dal.Get(ctx, param);
		}

		public void CheckTaxRateExist<T>(MContext ctx, T model, List<IOValidationResultModel> validationResult, List<REGTaxRateModel> taxRateList, string taxRateField = "MTaxID", int rowIndex = -1)
		{
			string text = ModelHelper.GetModelValue(model, taxRateField).Trim();
			if (!string.IsNullOrWhiteSpace(text))
			{
				string text2 = string.Empty;
				int num = text.LastIndexOf('(');
				string taxRateName = (num == -1) ? text : text.Substring(0, num);
				if (taxRateList != null)
				{
					REGTaxRateModel rEGTaxRateModel = taxRateList.FirstOrDefault((REGTaxRateModel f) => f.MItemID == taxRateName || (!string.IsNullOrWhiteSpace(f.MName) && HttpUtility.HtmlDecode(f.MName.Trim()) == taxRateName));
					if (!string.IsNullOrWhiteSpace(taxRateName) && rEGTaxRateModel != null)
					{
						if (!rEGTaxRateModel.MIsActive)
						{
							text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TaxRateHasDisabled", "税率：{0}已禁用！");
						}
						else
						{
							ModelHelper.SetModelValue(model, taxRateField, rEGTaxRateModel.MItemID, null);
						}
					}
					else
					{
						text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxRateNotFound", "The Tax Rate:{0} can't be found!");
					}
					if (!string.IsNullOrWhiteSpace(text2))
					{
						int rowIndex2 = 0;
						if (rowIndex != -1)
						{
							rowIndex2 = rowIndex;
						}
						else
						{
							int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex2);
						}
						validationResult.Add(new IOValidationResultModel
						{
							FieldType = IOValidationTypeEnum.TaxRate,
							FieldValue = text,
							Message = text2,
							RowIndex = rowIndex2
						});
					}
				}
			}
		}

		public void CheckTaxRateExist<T>(MContext ctx, List<T> entryList, List<IOValidationResultModel> validationResult)
		{
			string taxRateField = "MTaxID";
			List<string> source = (from f in entryList
			where !string.IsNullOrWhiteSpace(ModelHelper.GetModelValue<T>(f, taxRateField))
			select ModelHelper.GetModelValue<T>(f, taxRateField).Trim()).Distinct().ToList();
			if (source.Any())
			{
				List<REGTaxRateModel> listIgnoreLocale = dal.GetListIgnoreLocale(ctx, true);
				int num = 0;
				foreach (T entry in entryList)
				{
					CheckTaxRateExist(ctx, entry, validationResult, listIgnoreLocale, taxRateField, num);
					num++;
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

		public OperationResult InsertOrUpdate(MContext ctx, REGTaxRateModel modelData, string fields = null)
		{
			OperationResult operationResult = new OperationResult();
			if (modelData == null || modelData.TaxRateDetail == null || modelData.TaxRateDetail.Count == 0)
			{
				return operationResult;
			}
			decimal d = modelData.TaxRateDetail.Sum((REGTaxRateEntryModel x) => x.MTaxRate);
			if (d > 100m)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TaxRateGreateThan100", "税率不能大于100%");
				return operationResult;
			}
			if (d < decimal.Zero)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TaxRateLessThan0", "税率不能小于0%");
				return operationResult;
			}
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<REGTaxRateModel> modelData, string fields = null)
		{
			return dal.InsertOrUpdateModels(ctx, modelData, fields);
		}

		public OperationResult Delete(MContext ctx, string pkID)
		{
			OperationResult operationResult = new OperationResult();
			if (pkID.IndexOf(',') >= 0)
			{
				List<string> list = new List<string>();
				operationResult = BDRepository.IsCanDelete(ctx, "TaxRate", pkID, out list);
				if (list.Count != 0)
				{
					ModelInfoManager.DeleteFlag<REGTaxRateModel>(ctx, list);
				}
			}
			else
			{
				operationResult = BDRepository.IsCanDelete(ctx, "TaxRate", pkID);
				if (operationResult.Success)
				{
					operationResult = dal.Delete(ctx, pkID);
				}
			}
			return operationResult;
		}

		public OperationResult DeleteModels(MContext ctx, List<string> pkID)
		{
			return dal.DeleteModels(ctx, pkID);
		}

		public REGTaxRateModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			REGTaxRateModel dataModel = dal.GetDataModel(ctx, pkID, includeDelete);
			if (dataModel != null && ctx.MRegProgress == 15)
			{
				string saleTaxAccountCode = (dataModel.MSaleTaxAccountCode == null) ? "22210105" : dataModel.MSaleTaxAccountCode;
				string purchaseAccountCode = (dataModel.MPurchaseAccountCode == null) ? "22210101" : dataModel.MPurchaseAccountCode;
				string payDebitAccountCode = (dataModel.MPayDebitAccountCode == null) ? "22210102" : dataModel.MPayDebitAccountCode;
				BDAccountRepository bDAccountRepository = new BDAccountRepository();
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.Equal("a.MIsDelete", "0");
				sqlWhere.Equal("a.MIsActive", "1");
				sqlWhere.Equal("a.MAccountTableID", ctx.MAccountTableID);
				sqlWhere.In("MCode", new string[3]
				{
					saleTaxAccountCode,
					purchaseAccountCode,
					payDebitAccountCode
				});
				List<BDAccountModel> baseBDAccountList = bDAccountRepository.GetBaseBDAccountList(ctx, sqlWhere, false, null);
				if (baseBDAccountList != null && baseBDAccountList.Count() == 3)
				{
					BDAccountModel bDAccountModel = (from x in baseBDAccountList
					where x.MCode == saleTaxAccountCode
					select x).FirstOrDefault();
					dataModel.MSaleTaxAccountId = ((bDAccountModel != null) ? bDAccountModel.MItemID : "");
					BDAccountModel bDAccountModel2 = (from x in baseBDAccountList
					where x.MCode == purchaseAccountCode
					select x).FirstOrDefault();
					dataModel.MPurchaseAccountId = ((bDAccountModel2 != null) ? bDAccountModel2.MItemID : "");
					BDAccountModel bDAccountModel3 = (from x in baseBDAccountList
					where x.MCode == payDebitAccountCode
					select x).FirstOrDefault();
					dataModel.MPayDebitAccountId = ((bDAccountModel3 != null) ? bDAccountModel3.MItemID : "");
				}
			}
			return dataModel;
		}

		public REGTaxRateModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<REGTaxRateModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<REGTaxRateModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public DataGridJson<REGTaxRateModel> GetTaxTateListByPage(MContext ctx, REGTaxTateListFilterModel filter)
		{
			return dal.GetTaxRateListByPage(ctx, filter);
		}

		public BDIsCanDeleteModel IsCanDeleteOrInactive(MContext ctx, ParamBase param)
		{
			return BDRepository.IsCanDeleteOrInactive(ctx, "TaxRate", param.KeyIDs.Split(',').ToList(), param.IsDelete);
		}

		public OperationResult ArchiveTaxRate(MContext ctx, string keyIDs, bool isActive)
		{
			List<string> list = keyIDs.Split(',').ToList();
			if (list == null || list.Count <= 0)
			{
				return new OperationResult
				{
					Success = false
				};
			}
			return dal.ArchiveTaxRate(ctx, keyIDs.Split(',').ToList(), isActive);
		}

		public List<REGTaxRateModel> GetTaxRateList(MContext ctx, bool ignoreLocale = false)
		{
			REGTaxRateRepository rEGTaxRateRepository = new REGTaxRateRepository();
			return rEGTaxRateRepository.GetList(ctx, null, ignoreLocale);
		}

		public REGTaxRateModel GetNewTaxRate(MContext ctx, string name, decimal percent)
		{
			return new REGTaxRateModel
			{
				MOrgID = ctx.MOrgID,
				MAppID = ctx.MAppID,
				MTaxRate = percent,
				MEffectiveTaxRate = percent,
				MultiLanguage = new List<MultiLanguageFieldList>
				{
					new MultiLanguageFieldList
					{
						MFieldName = "MName",
						MMultiLanguageField = new List<MultiLanguageField>
						{
							new MultiLanguageField
							{
								MLocaleID = "0x0009",
								MValue = name
							},
							new MultiLanguageField
							{
								MLocaleID = "0x7804",
								MValue = name
							},
							new MultiLanguageField
							{
								MLocaleID = "0x7C04",
								MValue = name
							}
						}
					}
				},
				TaxRateDetail = new List<REGTaxRateEntryModel>
				{
					new REGTaxRateEntryModel
					{
						MOrgID = ctx.MOrgID,
						MTaxRate = percent
					}
				}
			};
		}

		public OperationResult GetUpdateTaxInfo(MContext ctx, int changeTaxType)
		{
			OperationResult operationResult = new OperationResult();
			if (ctx.MRegProgress < 15)
			{
				operationResult.Success = false;
				return operationResult;
			}
			REGFinancialModel rEGFinancialModel = new REGFinancialModel
			{
				MOrgID = ctx.MOrgID
			};
			REGFinancialModel byOrgID = new REGFinancialRepository().GetByOrgID(ctx, rEGFinancialModel);
			if (rEGFinancialModel == null)
			{
				throw new Exception("无法找到组织财务信息");
			}
			if (byOrgID.MTaxPayer == changeTaxType.ToString())
			{
				operationResult.Success = false;
				return operationResult;
			}
			List<REGTaxRateModel> modelList = new REGTaxRateRepository().GetModelList(ctx, null, false);
			List<REGTaxRateModel> addTaxListByUpgrade = GetAddTaxListByUpgrade(ctx, changeTaxType);
			List<REGTaxRateModel> list = new List<REGTaxRateModel>();
			modelList = (from x in modelList
			where !string.IsNullOrWhiteSpace(x.MName)
			select x).ToList();
			foreach (REGTaxRateModel item in addTaxListByUpgrade)
			{
				if (!TaxRateIsExist(modelList, item))
				{
					BizVerificationInfor bizVerificationInfor = new BizVerificationInfor();
					bizVerificationInfor.Id = "1";
					bizVerificationInfor.Message = item.MName;
					bizVerificationInfor.ExtendField = item.MTaxText;
					operationResult.VerificationInfor.Add(bizVerificationInfor);
				}
				else
				{
					BizVerificationInfor bizVerificationInfor2 = new BizVerificationInfor();
					bizVerificationInfor2.Id = "0";
					bizVerificationInfor2.Message = item.MName;
					bizVerificationInfor2.ExtendField = item.MTaxText;
					operationResult.VerificationInfor.Add(bizVerificationInfor2);
				}
			}
			operationResult.Success = (operationResult.VerificationInfor != null && operationResult.VerificationInfor.Any((BizVerificationInfor x) => x.Id == "1") && operationResult.VerificationInfor.Any((BizVerificationInfor x) => x.Id == "0"));
			return operationResult;
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

		private List<REGTaxRateModel> GetAddTaxListByUpgrade(MContext ctx, int changeTaxType)
		{
			List<REGTaxRateModel> list = new List<REGTaxRateModel>();
			List<BASLangModel> sysLangList = BASLangRepository.GetSysLangList(ctx);
			switch (changeTaxType)
			{
			case 1:
			{
				REGTaxRateModel rEGTaxRateModel2 = new REGTaxRateModel();
				rEGTaxRateModel2.MName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "modernservice-tax-rate", "现代服务业税率");
				rEGTaxRateModel2.MTaxRate = 6m;
				rEGTaxRateModel2.MultiLanguage = GetMulitLangeuageFieldList(ctx, "modernservice-tax-rate", "现代服务业税率", sysLangList);
				list.Add(rEGTaxRateModel2);
				REGTaxRateModel rEGTaxRateModel3 = new REGTaxRateModel();
				rEGTaxRateModel3.MName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "transport-tax-rate(2018)", "运输业税率(2018)");
				rEGTaxRateModel3.MTaxRate = 10m;
				rEGTaxRateModel3.MultiLanguage = GetMulitLangeuageFieldList(ctx, "transport-tax-rate(2018)", "运输业税率(2018)", sysLangList);
				list.Add(rEGTaxRateModel3);
				REGTaxRateModel rEGTaxRateModel4 = new REGTaxRateModel();
				rEGTaxRateModel4.MName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "transport-tax-rate", "运输业税率");
				rEGTaxRateModel4.MTaxRate = 11m;
				rEGTaxRateModel4.MultiLanguage = GetMulitLangeuageFieldList(ctx, "transport-tax-rate", "运输业税率", sysLangList);
				list.Add(rEGTaxRateModel4);
				REGTaxRateModel rEGTaxRateModel5 = new REGTaxRateModel();
				rEGTaxRateModel5.MName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "low-tax-rate", "低税率");
				rEGTaxRateModel5.MTaxRate = 13m;
				rEGTaxRateModel5.MultiLanguage = GetMulitLangeuageFieldList(ctx, "low-tax-rate", "低税率", sysLangList);
				list.Add(rEGTaxRateModel5);
				REGTaxRateModel rEGTaxRateModel6 = new REGTaxRateModel();
				rEGTaxRateModel6.MName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "basic-tax-rate", "基本税率");
				rEGTaxRateModel6.MTaxRate = 17m;
				rEGTaxRateModel6.MultiLanguage = GetMulitLangeuageFieldList(ctx, "basic-tax-rate", "基本税率", sysLangList);
				list.Add(rEGTaxRateModel6);
				REGTaxRateModel rEGTaxRateModel7 = new REGTaxRateModel();
				rEGTaxRateModel7.MName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "basic-tax-rate(2018)", "基本税率(2018)");
				rEGTaxRateModel7.MultiLanguage = GetMulitLangeuageFieldList(ctx, "basic-tax-rate(2018)", "基本税率(2018)", sysLangList);
				rEGTaxRateModel7.MTaxRate = 16m;
				list.Add(rEGTaxRateModel7);
				break;
			}
			case 2:
			{
				REGTaxRateModel rEGTaxRateModel = new REGTaxRateModel();
				rEGTaxRateModel.MName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Bank, "smallscale-tax-rate", "小额税率");
				rEGTaxRateModel.MultiLanguage = GetMulitLangeuageFieldList(ctx, "smallscale-tax-rate", "小额税率", sysLangList);
				rEGTaxRateModel.MTaxRate = 3m;
				list.Add(rEGTaxRateModel);
				break;
			}
			}
			return list;
		}

		private List<MultiLanguageFieldList> GetMulitLangeuageFieldList(MContext ctx, string taxRateKey, string defaultValue, List<BASLangModel> sysLanguageList = null)
		{
			sysLanguageList = (sysLanguageList ?? BASLangRepository.GetSysLangList(ctx));
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>
			{
				new MultiLanguageFieldList
				{
					MFieldName = "MName"
				}
			};
			List<MultiLanguageField> list2 = new List<MultiLanguageField>();
			foreach (BASLangModel sysLanguage in sysLanguageList)
			{
				MultiLanguageField item = new MultiLanguageField
				{
					MLocaleID = sysLanguage.LangID,
					MValue = COMMultiLangRepository.GetText(sysLanguage.LangID, LangModule.Bank, taxRateKey, defaultValue)
				};
				list2.Add(item);
			}
			list[0].MMultiLanguageField = list2;
			return list;
		}
	}
}
