using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.GL;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JieNor.Megi.BusinessService.FA
{
	public class FAFixAssetsBusiness : IFAFixAssetsBusiness, IDataContract<FAFixAssetsModel>
	{
		private readonly FAFixAssetsRepository dal = new FAFixAssetsRepository();

		private readonly BASOrgPrefixSettingBusiness prefix = new BASOrgPrefixSettingBusiness();

		public List<NameValueModel> GetFixAssetsTabInfo(MContext ctx)
		{
			return dal.GetFixAssetsTabInfo(ctx);
		}

		public GLCheckGroupValueModel GetMergeCheckGroupValueModel(MContext ctx, List<string> checkGroupValueIDs)
		{
			return new GLUtility().MergeCheckGroupValueModelByIDs(ctx, checkGroupValueIDs);
		}

		public FAFixAssetsModel GetFixAssetsModelByID(MContext ctx, string itemID)
		{
			List<FAFixAssetsModel> fixAssetsList = GetFixAssetsList(ctx, new FAFixAssetsFilterModel
			{
				MItemID = itemID,
				Status = -1
			});
			FAFixAssetsModel fAFixAssetsModel = fixAssetsList.FirstOrDefault();
			if (fAFixAssetsModel != null)
			{
				fAFixAssetsModel.MultiLanguage = GetDataModel(ctx, itemID, false).MultiLanguage;
			}
			return fAFixAssetsModel;
		}

		public FAFixAssetsModel GetFixAssetsModel(MContext ctx, string itemID = null, bool isCopy = false)
		{
			BASOrgPrefixSettingModel nextFixAssetsNumber = GetNextFixAssetsNumber(ctx);
			int mValue;
			if (string.IsNullOrWhiteSpace(itemID))
			{
				FAFixAssetsModel obj = new FAFixAssetsModel
				{
					MPrefix = nextFixAssetsNumber.MPrefix,
					MFullNumber = nextFixAssetsNumber.ToMString()
				};
				mValue = nextFixAssetsNumber.MValue;
				obj.MNumber = mValue.ToString();
				obj.MPrefixModel = nextFixAssetsNumber;
				return obj;
			}
			GLUtility gLUtility = new GLUtility();
			FAFixAssetsModel fixAssetsModelByID = GetFixAssetsModelByID(ctx, itemID);
			fixAssetsModelByID.MCheckGroupValueModel = gLUtility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
			{
				fixAssetsModelByID.MFixCheckGroupValueID,
				fixAssetsModelByID.MDepCheckGroupValueID,
				fixAssetsModelByID.MExpCheckGroupValueID
			});
			fixAssetsModelByID.MPrefixModel = nextFixAssetsNumber;
			if (isCopy)
			{
				FAFixAssetsModel fAFixAssetsModel = fixAssetsModelByID;
				mValue = nextFixAssetsNumber.MValue;
				fAFixAssetsModel.MNumber = mValue.ToString();
				fixAssetsModelByID.MPrefix = nextFixAssetsNumber.MPrefix;
				fixAssetsModelByID.MItemID = null;
				fixAssetsModelByID.MLastDepreciatedDate = DateTime.MinValue;
				fixAssetsModelByID.MultiLanguage.ForEach(delegate(MultiLanguageFieldList x)
				{
					x.MParentID = null;
					x.MMultiLanguageField.ForEach(delegate(MultiLanguageField y)
					{
						y.MPKID = null;
					});
				});
			}
			else
			{
				fixAssetsModelByID.MSrcModel = GetDataModel(ctx, itemID, false);
			}
			return fixAssetsModelByID;
		}

		private BASOrgPrefixSettingModel GetNextFixAssetsNumber(MContext ctx)
		{
			return dal.GetNextFixAssetsNumber(ctx);
		}

		public DataGridJson<FAFixAssetsModel> GetFixAssetsPageList(MContext ctx, FAFixAssetsFilterModel filter = null)
		{
			int fixAssetsPageListCount = dal.GetFixAssetsPageListCount(ctx, filter);
			List<FAFixAssetsModel> fixAssetsPageList = dal.GetFixAssetsPageList(ctx, filter);
			return new DataGridJson<FAFixAssetsModel>
			{
				total = fixAssetsPageListCount,
				rows = fixAssetsPageList
			};
		}

		public List<FAFixAssetsModel> GetFixAssetsList(MContext ctx, FAFixAssetsFilterModel filter)
		{
			List<FAFixAssetsModel> fixAssetsList = dal.GetFixAssetsList(ctx, filter);
			return ProcessFixAssetExtendData(ctx, fixAssetsList, filter);
		}

		private List<FAFixAssetsModel> ProcessFixAssetExtendData(MContext ctx, List<FAFixAssetsModel> fixAssetList, FAFixAssetsFilterModel filter)
		{
			if (filter == null || fixAssetList == null || fixAssetList.Count() == 0)
			{
				return fixAssetList;
			}
			List<BDAccountModel> accountList = null;
			if (filter.NeedAccountFullName && fixAssetList != null && fixAssetList.Count() > 0)
			{
				SqlWhere sqlWhere = new SqlWhere();
				List<string> accountCodeList = new List<string>();
				fixAssetList.ForEach(delegate(FAFixAssetsModel x)
				{
					if (!string.IsNullOrWhiteSpace(x.MExpAccountCode))
					{
						accountCodeList.Add(x.MExpAccountCode);
					}
					if (!string.IsNullOrWhiteSpace(x.MFixAccountCode))
					{
						accountCodeList.Add(x.MFixAccountCode);
					}
					if (!string.IsNullOrWhiteSpace(x.MDepAccountCode))
					{
						accountCodeList.Add(x.MDepAccountCode);
					}
				});
				if (accountCodeList.Count() > 0)
				{
					sqlWhere.In("MCode", accountCodeList);
				}
				accountList = new BDAccountRepository().GetBaseBDAccountList(ctx, sqlWhere, true, null);
			}
			if (filter.NeedMultiLang)
			{
				List<string> assetIdList = (from x in fixAssetList
				select x.MItemID).ToList();
				List<MultiLanguageFieldList> multiLanguageList = dal.GetMultiLanguageList(ctx, assetIdList);
				string[] megiLangTypes = ServerHelper.MegiLangTypes;
				if (multiLanguageList != null && multiLanguageList.Count() > 0)
				{
					foreach (FAFixAssetsModel fixAsset in fixAssetList)
					{
						List<MultiLanguageFieldList> list = (from x in multiLanguageList
						where x.MParentID == fixAsset.MItemID
						select x).ToList();
						if (list == null || list.Count() == 0)
						{
							throw new NullReferenceException("not find multilanguage info");
						}
						fixAsset.MultiLanguage = list;
						if (megiLangTypes != null && megiLangTypes.Count() > 0)
						{
							string[] array = megiLangTypes;
							foreach (string text in array)
							{
								string multiLanguageValue = fixAsset.GetMultiLanguageValue(text, "MName");
								if (text == LangCodeEnum.EN_US)
								{
									fixAsset.MName0x0009 = multiLanguageValue;
								}
								else if (text == LangCodeEnum.ZH_CN)
								{
									fixAsset.MName0x7804 = multiLanguageValue;
								}
								else if (text == LangCodeEnum.ZH_CN)
								{
									fixAsset.MName0x7C04 = multiLanguageValue;
								}
							}
						}
					}
				}
			}
			fixAssetList.ForEach(delegate(FAFixAssetsModel x)
			{
				x.MDepreciationTypeName = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "CompositeLifeMethod", "???????????????");
				if (accountList != null && accountList.Count() > 0)
				{
					if (!string.IsNullOrWhiteSpace(x.MExpAccountCode))
					{
						BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel y) => y.MCode == x.MExpAccountCode);
						if (bDAccountModel != null)
						{
							x.MExpAccountName = bDAccountModel.MFullName;
						}
					}
					if (!string.IsNullOrWhiteSpace(x.MFixAccountCode))
					{
						BDAccountModel bDAccountModel2 = accountList.FirstOrDefault((BDAccountModel y) => y.MCode == x.MFixAccountCode);
						if (bDAccountModel2 != null)
						{
							x.MFixAccountName = bDAccountModel2.MFullName;
						}
					}
					if (!string.IsNullOrWhiteSpace(x.MDepAccountCode))
					{
						BDAccountModel bDAccountModel3 = accountList.FirstOrDefault((BDAccountModel y) => y.MCode == x.MDepAccountCode);
						if (bDAccountModel3 != null)
						{
							x.MDepAccountName = bDAccountModel3.MFullName;
						}
					}
				}
				x.MDepreciatedPeriod = x.MDepreciatedPeriods;
				x.MSalvageRate = x.MRateOfSalvage;
				x.MHandleDateString = ((x.MHandledDate != DateTime.MinValue) ? x.MHandledDate.ToString("yyyy-MM") : "");
			});
			return fixAssetList;
		}

		public OperationResult DeleteFixAssets(MContext ctx, List<string> itemIDs)
		{
			return dal.DeleteFixAssets(ctx, itemIDs);
		}

		public OperationResult SaveFixAssets(MContext ctx, FAFixAssetsModel model)
		{
			if (model.MCheckGroupValueModel != null)
			{
				GLUtility gLUtility = new GLUtility();
				model.MFixCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, model.MFixAccountCode, model.MCheckGroupValueModel);
				model.MDepCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, model.MDepAccountCode, model.MCheckGroupValueModel);
				model.MExpCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, model.MExpAccountCode, model.MCheckGroupValueModel);
			}
			return dal.SaveFixAssets(ctx, model);
		}

		public OperationResult ImportAssetCardList(MContext ctx, List<FAFixAssetsModel> assetCardList)
		{
			OperationResult operationResult = new OperationResult();
			if (assetCardList == null || assetCardList.Count() == 0)
			{
				return operationResult;
			}
			List<CommandInfo> list = new List<CommandInfo>();
			List<StringBuilder> list2 = new List<StringBuilder>();
			FAFixAssetsTypeBusiness fAFixAssetsTypeBusiness = new FAFixAssetsTypeBusiness();
			List<FAFixAssetsTypeModel> fixAssetsTypeList = fAFixAssetsTypeBusiness.GetFixAssetsTypeList(ctx, null);
			BASOrgPrefixSettingBusiness bASOrgPrefixSettingBusiness = new BASOrgPrefixSettingBusiness();
			BASOrgPrefixSettingModel orgPrefixSettingModel = bASOrgPrefixSettingBusiness.GetOrgPrefixSettingModel(ctx, "FixAssets");
			List<BDAccountModel> baseBDAccountList = new BDAccountRepository().GetBaseBDAccountList(ctx, null, true, null);
			foreach (FAFixAssetsModel assetCard in assetCardList)
			{
				StringBuilder errorBulider = new StringBuilder();
				List<ValidationError> list3 = new List<ValidationError>();
				try
				{
					List<ValidationError> list4 = ValidateAssetCard(ctx, assetCard, fixAssetsTypeList, orgPrefixSettingModel, assetCardList, baseBDAccountList);
					assetCard.ValidationErrors.AddRange(list4);
					list3.AddRange(list4);
					SetAssetCardDefaultValue(ctx, assetCard, fixAssetsTypeList, orgPrefixSettingModel);
					List<CommandInfo> insertOrUpdateCmdList = dal.GetInsertOrUpdateCmdList(ctx, assetCard);
					if (list4.Count() == 0 && insertOrUpdateCmdList != null)
					{
						list.AddRange(insertOrUpdateCmdList);
					}
				}
				catch (MActionException ex)
				{
					List<MActionResultCodeEnum> codes = ex.Codes;
					foreach (MActionResultCodeEnum item2 in codes)
					{
						string text = item2.ToString();
						text = text.Substring(1, text.Length - 1);
						string errrorString = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, text, "????????????????????????");
						if (!list3.Exists((ValidationError x) => x.Message == errrorString))
						{
							ValidationError item = new ValidationError(errrorString);
							list3.Add(item);
						}
					}
					assetCard.ValidationErrors.AddRange(list3);
				}
				if (list3.Count() > 0)
				{
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "ErrorInRowIndex", "??????{0}????????????????????????");
					text2 = string.Format(text2, assetCard.MRowIndex.ToString());
					errorBulider.Append(text2);
					list3.ForEach(delegate(ValidationError x)
					{
						errorBulider.AppendLine(x.Message);
					});
					list2.Add(errorBulider);
				}
			}
			if (list2.Count() > 0)
			{
				list.Clear();
				StringBuilder resultBuilder = new StringBuilder();
				list2.ForEach(delegate(StringBuilder x)
				{
					resultBuilder.AppendLine(x.ToString());
				});
				operationResult.Success = false;
				operationResult.Message = resultBuilder.ToString();
			}
			if (list.Count() > 0)
			{
				operationResult.Success = (BDRepository.ExecuteSqlTran(ctx, list) > 0);
			}
			return operationResult;
		}

		private List<ValidationError> ValidateAssetCard(MContext ctx, FAFixAssetsModel assetCard, List<FAFixAssetsTypeModel> assetTypeList, BASOrgPrefixSettingModel fixAssetInitData, List<FAFixAssetsModel> assetCardList, List<BDAccountModel> accountList)
		{
			List<ValidationError> list = new List<ValidationError>();
			if (!string.IsNullOrWhiteSpace(assetCard.MPrefix) && assetCard.MPrefix.Length > 30)
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetPrefixTooLength", "??????????????????????????????30??????");
				ValidationError item = new ValidationError(text);
				list.Add(item);
			}
			if (string.IsNullOrWhiteSpace(assetCard.MNumber))
			{
				string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetNumberIsRequire", "??????????????????????????????");
				ValidationError item2 = new ValidationError(text2);
				list.Add(item2);
			}
			else if (assetCard.MNumber.Length > 200)
			{
				string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetNumberTooLength", "??????????????????????????????200??????");
				ValidationError item3 = new ValidationError(text3);
				list.Add(item3);
			}
			if (assetCard.MNumber.Length < 4)
			{
				assetCard.MNumber = "0000".Substring(0, 4 - assetCard.MNumber.Length) + assetCard.MNumber;
			}
			bool flag = false;
			foreach (FAFixAssetsModel assetCard2 in assetCardList)
			{
				string a = ((assetCard2.MPrefix == null) ? "" : assetCard2.MPrefix) + "-" + assetCard2.MNumber;
				string b = ((assetCard.MPrefix == null) ? "" : assetCard.MPrefix) + "-" + assetCard.MNumber;
				if (assetCard2 != assetCard && a == b)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "FixAssetsNumberDuplicated", "??????????????????!");
				ValidationError item4 = new ValidationError(text4);
				list.Add(item4);
			}
			int num = 0;
			if (!int.TryParse(assetCard.MNumber, out num) || num < 0)
			{
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetNumberMustNumber", "????????????????????????????????????");
				ValidationError item5 = new ValidationError(text5);
				list.Add(item5);
			}
			foreach (string mActiveLocaleID in ctx.MActiveLocaleIDS)
			{
				string multiLanguageValue = assetCard.GetMultiLanguageValue(mActiveLocaleID, "MName");
				if (string.IsNullOrWhiteSpace(multiLanguageValue))
				{
					string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetNameIsRequire", "??????????????????????????????");
					ValidationError item6 = new ValidationError(text6);
					list.Add(item6);
				}
				else if (multiLanguageValue.Length > 200)
				{
					string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetNameTooLength", "??????????????????????????????200??????");
					ValidationError item7 = new ValidationError(text7);
					list.Add(item7);
				}
			}
			if (assetCard.MPurchaseDate == DateTime.MinValue)
			{
				string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetPurchaseDateIsRequire", "????????????????????????");
				ValidationError item8 = new ValidationError(text8);
				list.Add(item8);
			}
			if (string.IsNullOrWhiteSpace(assetCard.MFATypeIDName))
			{
				string text9 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetTypeIsRequire", "????????????????????????");
				ValidationError item9 = new ValidationError(text9);
				list.Add(item9);
			}
			else
			{
				FAFixAssetsTypeModel fAFixAssetsTypeModel = (from x in assetTypeList
				where x.MNumber + "-" + x.MName == assetCard.MFATypeIDName || x.MItemID == assetCard.MFATypeIDName || x.MNumber == assetCard.MFATypeIDName
				select x).FirstOrDefault();
				if (fAFixAssetsTypeModel == null)
				{
					string text10 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetTypeNotExist", "????????????:{0}?????????");
					text10 = string.Format(text10, assetCard.MFATypeIDName);
					ValidationError item10 = new ValidationError(text10);
					list.Add(item10);
				}
				else
				{
					assetCard.MFATypeID = fAFixAssetsTypeModel.MItemID;
				}
			}
			if (assetCard.MQuantity <= decimal.Zero)
			{
				string text11 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetQuantityIsRequire", "????????????????????????");
				ValidationError item11 = new ValidationError(text11);
				list.Add(item11);
			}
			if (string.IsNullOrWhiteSpace(assetCard.MDepreciationTypeName))
			{
				string text12 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciationTypeIsRequire", "????????????????????????");
				ValidationError item12 = new ValidationError(text12);
				list.Add(item12);
			}
			DateTime dateTime;
			if (assetCard.MDepreciationFromPeriod == DateTime.MinValue)
			{
				string text13 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciationFromPeriodIsRequire", "??????????????????????????????");
				ValidationError item13 = new ValidationError(text13);
				list.Add(item13);
			}
			else if (assetCard.MDepreciationFromPeriod < fixAssetInitData.MFABeginDate)
			{
				string text14 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciationFromPeriodMoreThanBeginDate", "????????????????????????????????????????????????????????????");
				ValidationError item14 = new ValidationError(text14);
				list.Add(item14);
			}
			else
			{
				dateTime = assetCard.MDepreciationFromPeriod;
				int num2 = dateTime.Year * 100;
				dateTime = assetCard.MDepreciationFromPeriod;
				int num3 = num2 + dateTime.Month;
				dateTime = assetCard.MPurchaseDate;
				int num4 = dateTime.Year * 100;
				dateTime = assetCard.MPurchaseDate;
				if (num3 < num4 + dateTime.Month)
				{
					string text15 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciationFromPeriodMoreThanMPurchaseDate", "????????????????????????????????????????????????");
					ValidationError item15 = new ValidationError(text15);
					list.Add(item15);
				}
				else
				{
					GLPeriodTransferBusiness gLPeriodTransferBusiness = new GLPeriodTransferBusiness();
					DateTime lastTransferPeriod = gLPeriodTransferBusiness.GetLastTransferPeriod(ctx, 2);
					if (assetCard.MDepreciationFromPeriod <= lastTransferPeriod)
					{
						string text16 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciationFromPeriodError", "???????????????????????????????????????????????????");
						ValidationError item16 = new ValidationError(text16);
						list.Add(item16);
					}
				}
			}
			if (assetCard.MUsefulPeriods <= 0)
			{
				string text17 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetUsefulPeriodIsRequire", "?????????????????????????????????0?????????");
				ValidationError item17 = new ValidationError(text17);
				list.Add(item17);
			}
			if (accountList != null && accountList.Count() > 0)
			{
				if (!string.IsNullOrWhiteSpace(assetCard.MDepAccountName))
				{
					assetCard.MDepAccountName = assetCard.MDepAccountName.Trim();
					BDAccountModel bDAccountModel = (from x in accountList
					where x.MFullName == assetCard.MDepAccountName || x.MNumber.Trim() == assetCard.MDepAccountName.Trim() || x.MCode == assetCard.MDepAccountName.Trim()
					select x).FirstOrDefault();
					if (bDAccountModel == null)
					{
						string text18 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepAccountNotExist", "??????????????????:{0}?????????");
						text18 = string.Format(text18, assetCard.MDepAccountName);
						ValidationError item18 = new ValidationError(text18);
						list.Add(item18);
					}
					else if (!bDAccountModel.MIsActive)
					{
						string text19 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepAccountIsDisable", "??????????????????:{0}?????????");
						text19 = string.Format(text19, bDAccountModel.MFullName);
						ValidationError item19 = new ValidationError(text19);
						list.Add(item19);
					}
					else
					{
						List<BDAccountModel> childrenAccountByRecursion = BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel, accountList, false);
						if (childrenAccountByRecursion != null && childrenAccountByRecursion.Count() > 0)
						{
							string text20 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetAccountIsParentAccount", "????????????{0}???????????????");
							text20 = string.Format(text20, bDAccountModel.MFullName);
							ValidationError item20 = new ValidationError(text20);
							list.Add(item20);
						}
						else
						{
							assetCard.MDepAccountCode = bDAccountModel.MCode;
						}
					}
				}
				if (!string.IsNullOrWhiteSpace(assetCard.MExpAccountName))
				{
					assetCard.MExpAccountName = assetCard.MExpAccountName.Trim();
					BDAccountModel bDAccountModel2 = (from x in accountList
					where x.MFullName == assetCard.MExpAccountName || x.MNumber.Trim() == assetCard.MExpAccountName.Trim() || x.MCode == assetCard.MExpAccountName.Trim()
					select x).FirstOrDefault();
					if (bDAccountModel2 == null)
					{
						string text21 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetExpAccountNotExist", "??????????????????:{0}?????????");
						text21 = string.Format(text21, assetCard.MExpAccountName);
						ValidationError item21 = new ValidationError(text21);
						list.Add(item21);
					}
					else if (!bDAccountModel2.MIsActive)
					{
						string text22 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetExpAccountIsDisable", "??????????????????:{0}?????????");
						text22 = string.Format(text22, bDAccountModel2.MFullName);
						ValidationError item22 = new ValidationError(text22);
						list.Add(item22);
					}
					else
					{
						List<BDAccountModel> childrenAccountByRecursion2 = BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel2, accountList, false);
						if (childrenAccountByRecursion2 != null && childrenAccountByRecursion2.Count() > 0)
						{
							string text23 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetAccountIsParentAccount", "????????????{0}???????????????");
							text23 = string.Format(text23, bDAccountModel2.MFullName);
							ValidationError item23 = new ValidationError(text23);
							list.Add(item23);
						}
						else
						{
							assetCard.MExpAccountCode = bDAccountModel2.MCode;
						}
					}
				}
				if (!string.IsNullOrWhiteSpace(assetCard.MFixAccountName))
				{
					assetCard.MFixAccountName = assetCard.MFixAccountName.Trim();
					BDAccountModel bDAccountModel3 = (from x in accountList
					where x.MFullName == assetCard.MFixAccountName || x.MNumber.Trim() == assetCard.MFixAccountName.Trim() || x.MCode == assetCard.MFixAccountName.Trim()
					select x).FirstOrDefault();
					if (bDAccountModel3 == null)
					{
						string text24 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetFixAccountNotExist", "??????????????????:{0}?????????");
						text24 = string.Format(text24, assetCard.MFixAccountName);
						ValidationError item24 = new ValidationError(text24);
						list.Add(item24);
					}
					else if (!bDAccountModel3.MIsActive)
					{
						string text25 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetFixAccountIsDisable", "??????????????????:{0}?????????");
						text25 = string.Format(text25, bDAccountModel3.MFullName);
						ValidationError item25 = new ValidationError(text25);
						list.Add(item25);
					}
					else
					{
						List<BDAccountModel> childrenAccountByRecursion3 = BDAccountHelper.GetChildrenAccountByRecursion(bDAccountModel3, accountList, false);
						if (childrenAccountByRecursion3 != null && childrenAccountByRecursion3.Count() > 0)
						{
							string text26 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetAccountIsParentAccount", "????????????{0}???????????????");
							text26 = string.Format(text26, bDAccountModel3.MFullName);
							ValidationError item26 = new ValidationError(text26);
							list.Add(item26);
						}
						else
						{
							assetCard.MFixAccountCode = bDAccountModel3.MCode;
						}
					}
				}
			}
			if (assetCard.MOriginalAmount < decimal.Zero)
			{
				string text27 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetOriginalAmountIsRequire", "???????????????????????????0?????????");
				ValidationError item27 = new ValidationError(text27);
				list.Add(item27);
			}
			else
			{
				assetCard.MOriginalAmount = decimal.Round(assetCard.MOriginalAmount, 2);
			}
			if (assetCard.MDepreciatedPeriod.HasValue && assetCard.MDepreciatedPeriod.Value < 0)
			{
				string text28 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciatedPeriodIsRequire", "???????????????????????????0");
				ValidationError item28 = new ValidationError(text28);
				list.Add(item28);
			}
			int? mDepreciatedPeriod = assetCard.MDepreciatedPeriod;
			int num5 = 0;
			if (mDepreciatedPeriod.GetValueOrDefault() > num5 & mDepreciatedPeriod.HasValue)
			{
				FAFixAssetsTypeModel fAFixAssetsTypeModel2 = (from x in assetTypeList
				where x.MItemID == assetCard.MFATypeID
				select x).FirstOrDefault();
				if (fAFixAssetsTypeModel2 != null)
				{
					dateTime = ctx.DateNow;
					int year = dateTime.Year;
					dateTime = assetCard.MPurchaseDate;
					int num6 = (year - dateTime.Year) * 12;
					dateTime = ctx.DateNow;
					int month = dateTime.Month;
					dateTime = assetCard.MPurchaseDate;
					int num7 = num6 + (month - dateTime.Month);
					if (!fAFixAssetsTypeModel2.MDepreciationFromCurrentPeriod)
					{
						num7 += -1;
					}
					mDepreciatedPeriod = assetCard.MDepreciatedPeriod;
					num5 = num7;
					if (mDepreciatedPeriod.GetValueOrDefault() > num5 & mDepreciatedPeriod.HasValue)
					{
						string message = fAFixAssetsTypeModel2.MDepreciationFromCurrentPeriod ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciatedPeriodThanError1", "?????????????????????????????????????????????????????????") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciatedPeriodThanError2", "???????????????????????????????????????????????????????????????");
						ValidationError item29 = new ValidationError(message);
						list.Add(item29);
					}
				}
			}
			mDepreciatedPeriod = assetCard.MDepreciatedPeriod;
			num5 = assetCard.MUsefulPeriods;
			if (mDepreciatedPeriod.GetValueOrDefault() > num5 & mDepreciatedPeriod.HasValue)
			{
				string text29 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetDepreciatedPeriodThanUserPeriod", "?????????????????????????????????????????????");
				ValidationError item30 = new ValidationError(text29);
				list.Add(item30);
			}
			int num8;
			if (assetCard.MSalvageRate.HasValue)
			{
				decimal? mSalvageRate = assetCard.MSalvageRate;
				if (!(mSalvageRate.GetValueOrDefault() < default(decimal) & mSalvageRate.HasValue))
				{
					mSalvageRate = assetCard.MSalvageRate;
					decimal d = 100;
					num8 = ((mSalvageRate.GetValueOrDefault() >= d & mSalvageRate.HasValue) ? 1 : 0);
				}
				else
				{
					num8 = 1;
				}
			}
			else
			{
				num8 = 0;
			}
			if (num8 != 0)
			{
				string text30 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetRateOfSalvageError", "??????????????????0(??????)-100?????????");
				ValidationError item31 = new ValidationError(text30);
				list.Add(item31);
			}
			if (assetCard.MPeriodDepreciatedAmount < decimal.Zero)
			{
				string text31 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetPeriodDepreciatedAmountLessZoer", "????????????????????????0");
				ValidationError item32 = new ValidationError(text31);
				list.Add(item32);
			}
			else
			{
				FAFixAssetsTypeModel fAFixAssetsTypeModel3 = (from x in assetTypeList
				where x.MItemID == assetCard.MFATypeID
				select x).FirstOrDefault();
				assetCard.MRateOfSalvage = (assetCard.MSalvageRate.HasValue ? assetCard.MSalvageRate.Value : (fAFixAssetsTypeModel3?.MRateOfSalvage ?? decimal.Zero));
				assetCard.MSalvageAmount = assetCard.MOriginalAmount * assetCard.MRateOfSalvage / 100m;
				assetCard.MSalvageAmount = decimal.Round(assetCard.MSalvageAmount, 2);
				if (assetCard.MOriginalAmount - assetCard.MDepreciatedAmount - assetCard.MPrepareForDecreaseAmount - assetCard.MSalvageAmount >= decimal.Zero && assetCard.MPeriodDepreciatedAmount > assetCard.MOriginalAmount - assetCard.MDepreciatedAmount - assetCard.MPrepareForDecreaseAmount - assetCard.MSalvageAmount)
				{
					string text32 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetPeriodDepreciatedAmountError", "??????????????????????????????-????????????-????????????-??????");
					ValidationError item33 = new ValidationError(text32);
					list.Add(item33);
				}
			}
			if (assetCard.MDepreciatedAmount < decimal.Zero)
			{
				string text33 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetMDepreciatedAmountLessZero", "?????????????????????????????????0");
				ValidationError item34 = new ValidationError(text33);
				list.Add(item34);
			}
			else if (assetCard.MDepreciatedAmount > assetCard.MOriginalAmount - assetCard.MPrepareForDecreaseAmount - assetCard.MSalvageAmount)
			{
				string text34 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetMDepreciatedAmountError", "?????????????????????????????????-????????????-????????????");
				ValidationError item35 = new ValidationError(text34);
				list.Add(item35);
			}
			if (assetCard.MPrepareForDecreaseAmount < decimal.Zero)
			{
				string text35 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetPrepareForDecreaseAmountError", "??????????????????????????????0???");
				ValidationError item36 = new ValidationError(text35);
				list.Add(item36);
			}
			if (!string.IsNullOrWhiteSpace(assetCard.MRemark) && assetCard.MRemark.Length > 200)
			{
				string text36 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "AssetRemarkTooLength", "??????????????????200??????");
				ValidationError item37 = new ValidationError(text36);
				list.Add(item37);
			}
			return list;
		}

		private void SetAssetCardDefaultValue(MContext ctx, FAFixAssetsModel assetCard, List<FAFixAssetsTypeModel> assetTypeList, BASOrgPrefixSettingModel fixAssetBaseInit)
		{
			FAFixAssetsTypeModel fAFixAssetsTypeModel = (from x in assetTypeList
			where x.MItemID == assetCard.MFATypeID
			select x).FirstOrDefault();
			assetCard.MHandledDate = DateTime.MinValue;
			if (fAFixAssetsTypeModel != null)
			{
				if (string.IsNullOrWhiteSpace(assetCard.MPrefix))
				{
					assetCard.MPrefix = fixAssetBaseInit.MPrefixName;
				}
				if (assetCard.MNumber.Length < 4)
				{
					assetCard.MNumber = "0000".Substring(0, 4 - assetCard.MNumber.Length) + assetCard.MNumber;
				}
				if (!assetCard.MDepreciatedPeriod.HasValue)
				{
					FAFixAssetsModel fAFixAssetsModel = assetCard;
					DateTime dateTime = assetCard.MDepreciationFromPeriod;
					int year = dateTime.Year;
					dateTime = assetCard.MPurchaseDate;
					int num = (year - dateTime.Year) * 12;
					dateTime = assetCard.MDepreciationFromPeriod;
					int month = dateTime.Month;
					dateTime = assetCard.MPurchaseDate;
					fAFixAssetsModel.MDepreciatedPeriods = num + (month - dateTime.Month);
					assetCard.MDepreciatedPeriods += ((!fAFixAssetsTypeModel.MDepreciationFromCurrentPeriod) ? (-1) : 0);
					assetCard.MDepreciatedPeriods = ((assetCard.MDepreciatedPeriods >= 0) ? assetCard.MDepreciatedPeriods : 0);
				}
				else
				{
					assetCard.MDepreciatedPeriods = assetCard.MDepreciatedPeriod.Value;
				}
				assetCard.MUsefulPeriods = ((assetCard.MUsefulPeriods == 0) ? fAFixAssetsTypeModel.MUsefulPeriods : assetCard.MUsefulPeriods);
				if (assetCard.MPeriodDepreciatedAmount == decimal.Zero)
				{
					assetCard.MPeriodDepreciatedAmount = ((assetCard.MUsefulPeriods == assetCard.MDepreciatedPeriods) ? decimal.Zero : ((assetCard.MOriginalAmount - assetCard.MDepreciatedAmount - assetCard.MSalvageAmount - assetCard.MPrepareForDecreaseAmount) / (decimal)(assetCard.MUsefulPeriods - assetCard.MDepreciatedPeriods)));
					assetCard.MPeriodDepreciatedAmount = decimal.Round(assetCard.MPeriodDepreciatedAmount, 2);
				}
				assetCard.MNetAmount = assetCard.MOriginalAmount - assetCard.MDepreciatedAmount - assetCard.MPrepareForDecreaseAmount;
				assetCard.MDepAccountCode = ((string.IsNullOrWhiteSpace(assetCard.MDepAccountCode) && !string.IsNullOrWhiteSpace(fAFixAssetsTypeModel.MDepAccountCode)) ? fAFixAssetsTypeModel.MDepAccountCode : assetCard.MDepAccountCode);
				assetCard.MExpAccountCode = ((string.IsNullOrWhiteSpace(assetCard.MExpAccountCode) && !string.IsNullOrWhiteSpace(fAFixAssetsTypeModel.MExpAccountCode)) ? fAFixAssetsTypeModel.MExpAccountCode : assetCard.MExpAccountCode);
				assetCard.MFixAccountCode = ((string.IsNullOrWhiteSpace(assetCard.MFixAccountCode) && !string.IsNullOrWhiteSpace(fAFixAssetsTypeModel.MFixAccountCode)) ? fAFixAssetsTypeModel.MFixAccountCode : assetCard.MFixAccountCode);
				GLUtility gLUtility = new GLUtility();
				assetCard.MFixCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, assetCard.MFixAccountCode, new GLCheckGroupValueModel());
				assetCard.MDepCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, assetCard.MDepAccountCode, new GLCheckGroupValueModel());
				assetCard.MExpCheckGroupValueID = gLUtility.GetCheckGroupValueID(ctx, assetCard.MExpAccountCode, new GLCheckGroupValueModel());
				assetCard.MDepreciationTypeID = "0";
			}
		}

		public OperationResult HandleFixAssets(MContext ctx, List<string> itemIDs, int type)
		{
			return dal.HandleFixAssets(ctx, itemIDs, type);
		}

		public OperationResult SetExpenseAccountDefault(MContext ctx, bool check, string accountCode)
		{
			return dal.SetExpenseAccountDefault(ctx, check, accountCode);
		}

		public List<DateTime> GetFAPeriodFromBeginDate(MContext ctx, bool includeCurrentPeriod = false)
		{
			return dal.GetFAPeriodFromBeginDate(ctx, includeCurrentPeriod);
		}

		public string GetLastFinishedPeriod(MContext ctx)
		{
			DateTime mFABeginDate = ctx.MFABeginDate;
			SqlWhere filter = new SqlWhere().Equal("MTransferTypeID", 2);
			List<GLPeriodTransferModel> modelList = new GLPeriodTransferRepository().GetModelList(ctx, filter, false);
			DateTime maxValue = DateTime.MaxValue;
			DateTime dateTime = (modelList == null || modelList.Count == 0) ? mFABeginDate : (from x in modelList
			select new DateTime(x.MYear, x.MPeriod, 1)).Max().AddMonths(1);
			dateTime = ((dateTime < mFABeginDate) ? mFABeginDate : dateTime);
			return mFABeginDate.ToString("yyyy-MM-dd") + "," + maxValue.ToString("yyyy-MM-dd") + "," + dateTime.ToString("yyyy-MM-dd");
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, true, null);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> exampleDataList = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary, null, exampleDataList);
			InitExampleData(ctx, exampleDataList);
			Dictionary<string, int> columnWidthList = InitColumnWidth();
			return new ImportTemplateModel
			{
				TemplateType = "Fixed_Assets",
				LocaleID = ctx.MLCID,
				ColumnList = dictionary,
				FieldConfigList = templateConfig,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = exampleDataList,
				ColumnWidthList = columnWidthList
			};
		}

		private void InitExampleData(MContext ctx, Dictionary<string, string[]> exampleDataList)
		{
			exampleDataList.Add("MPrefix", new string[1]
			{
				"FAN"
			});
			exampleDataList.Add("MNumber", new string[1]
			{
				"0001"
			});
			exampleDataList.Add("MName", new string[1]
			{
				"IPhone 8"
			});
			exampleDataList.Add("MPurchaseDate", new string[1]
			{
				"2017-01-01"
			});
			exampleDataList.Add("MFATypeIDName", new string[1]
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "ElectronicEquipment", "001_????????????")
			});
			exampleDataList.Add("MQuantity", new string[1]
			{
				"1"
			});
			exampleDataList.Add("MDepreciationTypeID", new string[1]
			{
				COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "StraightLineMethod", "???????????????")
			});
			exampleDataList.Add("MDepreciationFromPeriod", new string[1]
			{
				"2017-01"
			});
			exampleDataList.Add("MOriginalAmount", new string[1]
			{
				"9999.99"
			});
		}

		private Dictionary<string, int> InitColumnWidth()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("MPrefix", 10);
			dictionary.Add("MNumber", 10);
			dictionary.Add("MName", 20);
			dictionary.Add("MPurchaseDate", 10);
			dictionary.Add("MFATypeIDName", 20);
			dictionary.Add("MQuantity", 10);
			dictionary.Add("MDepreciationTypeID", 10);
			dictionary.Add("MDepreciationFromPeriod", 10);
			dictionary.Add("MOriginalAmount", 10);
			return dictionary;
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, bool isMainSheet, GLCheckGroupModel checkGroupModel)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ManualInput", "????????????");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Require", "??????");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "UnRequire", "?????????");
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MPrefix", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "FixAssetNumberPrefix", "??????????????????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "FixAssetNumberPrefixTips", "?????????????????????????????????????????????????????????????????????????????????????????????"), false),
				new IOTemplateConfigModel("MNumber", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "FixAssetNumber", "????????????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "FixAssetNumberTips", "??????;???????????????????????????0001?????????????????????"), true),
				new IOTemplateConfigModel("MName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "FixAssetName", "??????????????????", null)
				}), text2, true),
				new IOTemplateConfigModel("MPurchaseDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "PurchaseDate", "????????????", null)
				}), IOTemplateFieldType.Date, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PurchaseDateTips", "??????;?????????YYYY-MM-DD"), true),
				new IOTemplateConfigModel("MFATypeIDName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "FixAssetTypeName", "????????????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "FixAssetTypeNameTips", "??????;???????????????"), true),
				new IOTemplateConfigModel("MHandleDateString", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "DisposePeriod", "????????????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "DisposePeriodTips", "????????????"), false),
				new IOTemplateConfigModel("MQuantity", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "FixAssetsQuantity", "??????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "FixAssetsQuantityTips", "??????;??????0?????????"), true),
				new IOTemplateConfigModel("MDepreciationTypeName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "DepreciationType", "????????????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "DepreciationTypeTips", "??????;???????????????"), true),
				new IOTemplateConfigModel("MDepreciationFromPeriod", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "DepreciationFromPeriod", "??????????????????", null)
				}), IOTemplateFieldType.Text, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "DepreciationFromPeriodTips", "??????;????????????YYYY-MM;????????????????????????????????????????????????"), true),
				new IOTemplateConfigModel("MUsefulPeriods", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "ExpectedUsefulPeriods", "???????????????????????????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "ExpectedUsefulPeriodsTips", "???????????????0?????????"), true),
				new IOTemplateConfigModel("MFixAccountName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "FixAssetsAccount", "??????????????????", null)
				}), text3, false),
				new IOTemplateConfigModel("MDepAccountName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "DepreciationAccount", "??????????????????", null)
				}), text3, false),
				new IOTemplateConfigModel("MExpAccountName", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "DepreciationExpenseAccount", "??????????????????", null)
				}), text3, false),
				new IOTemplateConfigModel("MOriginalAmount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "OriginalValue", "??????", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "OriginalAmountTips", "??????;??????????????????0;??????????????????????????????"), true),
				new IOTemplateConfigModel("MDepreciatedPeriod", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "DepreciatedPeriodsCount", "???????????????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "DepreciatedPeriodsCountTips", "?????????"), false),
				new IOTemplateConfigModel("MDepreciatedAmount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "DepreciatedValue", "????????????", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "DepreciatedValueTips", "??????????????????0;??????????????????????????????"), false),
				new IOTemplateConfigModel("MPrepareForDecreaseAmount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "PrepareDecreaseValue", "????????????", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PrepareDecreaseValueTips", "?????????????????????????????????"), false),
				new IOTemplateConfigModel("MSalvageRate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "RateOfSalvage", "?????????(%)", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "RateOfSalvageTips", "?????????;0-100?????????;??????????????????????????????"), false),
				new IOTemplateConfigModel("MSalvageAmount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "ExpectedSalvageValue", "????????????", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "ExpectedSalvageTips", "?????????????????????????????????"), false),
				new IOTemplateConfigModel("MPeriodDepreciatedAmount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "PeriodDepreciationValue", "????????????", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PeriodDepreciationValueTips", "???????????????????????????????????????????????????"), false),
				new IOTemplateConfigModel("MNetAmount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "NetValue", "??????", null)
				}), IOTemplateFieldType.Decimal, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "NetValueTips", "???????????????????????????????????????????????????"), false),
				new IOTemplateConfigModel("MRemark", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.PA, "Remark", "??????", null)
				}), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "RemarkTips", "????????????;??????????????????200"), false)
			});
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string> columnList, List<BDAccountModel> accountList, Dictionary<string, string[]> exampleDataList = null)
		{
			List<ImportTemplateDataSource> list = new List<ImportTemplateDataSource>();
			FAFixAssetsModel emptyDataEditModel = ModelInfoManager.GetEmptyDataEditModel<FAFixAssetsModel>(ctx);
			List<string> fieldList = (from f in emptyDataEditModel.MultiLanguage
			where columnList.Keys.Contains(f.MFieldName)
			select f.MFieldName).ToList();
			BASLangBusiness bASLangBusiness = new BASLangBusiness();
			List<BASLangModel> orgLangList = bASLangBusiness.GetOrgLangList(ctx);
			List<ImportDataSourceInfo> dsLangList = new List<ImportDataSourceInfo>();
			orgLangList.ForEach(delegate(BASLangModel f)
			{
				dsLangList.Add(new ImportDataSourceInfo
				{
					Key = f.LangID,
					Value = f.LangName
				});
			});
			list.Add(new ImportTemplateDataSource(false)
			{
				FieldType = ImportTemplateColumnType.MultiLanguage,
				FieldList = fieldList,
				DataSourceList = dsLangList
			});
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			accountList = bDAccountRepository.GetBaseBDAccountList(ctx, new SqlWhere(), false, null);
			if (accountList != null)
			{
				accountList = BDAccountHelper.GetChildrenList(accountList);
				List<ImportDataSourceInfo> list2 = new List<ImportDataSourceInfo>();
				foreach (BDAccountModel account in accountList)
				{
					if (!string.IsNullOrWhiteSpace(account.MCode))
					{
						list2.Add(new ImportDataSourceInfo
						{
							Key = account.MCode,
							Value = account.MFullName
						});
					}
				}
				ImportTemplateDataSource item = new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Account,
					FieldList = new List<string>
					{
						"MFixAccountName",
						"MDepAccountName",
						"MExpAccountName"
					},
					DataSourceList = list2
				};
				list.Add(item);
			}
			ImportTemplateDataSource item2 = new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Method,
				FieldList = new List<string>
				{
					"MDepreciationTypeName"
				},
				DataSourceList = new List<ImportDataSourceInfo>
				{
					new ImportDataSourceInfo
					{
						Key = "0",
						Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "StraightLineMethod", "???????????????")
					}
				}
			};
			list.Add(item2);
			FAFixAssetsTypeBusiness fAFixAssetsTypeBusiness = new FAFixAssetsTypeBusiness();
			List<FAFixAssetsTypeModel> fixAssetsTypeList = fAFixAssetsTypeBusiness.GetFixAssetsTypeList(ctx, null);
			if (fixAssetsTypeList != null && fixAssetsTypeList.Count() > 0)
			{
				List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
				foreach (FAFixAssetsTypeModel item4 in fixAssetsTypeList)
				{
					list3.Add(new ImportDataSourceInfo
					{
						Key = item4.MItemID,
						Value = item4.MNumber + "-" + item4.MName
					});
				}
				ImportTemplateDataSource item3 = new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.None,
					FieldList = new List<string>
					{
						"MFATypeIDName"
					},
					DataSourceList = list3
				};
				list.Add(item3);
			}
			return list;
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FAFixAssetsModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FAFixAssetsModel> modelData, string fields = null)
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

		public FAFixAssetsModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FAFixAssetsModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FAFixAssetsModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FAFixAssetsModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
