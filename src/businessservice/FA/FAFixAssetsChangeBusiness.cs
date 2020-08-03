using JieNor.Megi.BusinessContract.FA;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessService.FA
{
	public class FAFixAssetsChangeBusiness : IFAFixAssetsChangeBusiness, IDataContract<FAFixAssetsChangeModel>
	{
		private readonly FAFixAssetsChangeRepository dal = new FAFixAssetsChangeRepository();

		private readonly FAFixAssetsRepository faFixAssets = new FAFixAssetsRepository();

		private static GLUtility utility = new GLUtility();

		public List<FAFixAssetsChangeModel> GetFixAssetsChangeLog(MContext ctx, FAFixAssetsChangeFilterModel filter = null)
		{
			List<FAFixAssetsChangeModel> list = new List<FAFixAssetsChangeModel>();
			List<FAFixAssetsChangeModel> fixAssetsChangeLogNotPage = dal.GetFixAssetsChangeLogNotPage(ctx, filter);
			if (fixAssetsChangeLogNotPage.Count > 0)
			{
				foreach (FAFixAssetsChangeModel item in fixAssetsChangeLogNotPage)
				{
					if (item.MIndex == 0)
					{
						item.MType = -1;
						list.Add(item);
					}
					else if (item.MType == 16)
					{
						if (item.MStatus == 0)
						{
							item.MNote = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "CancelHandle", "撤销处置");
						}
						else
						{
							item.MNote = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "Handle", "处置") + " : ";
							item.MNote += ((item.MStatus == 1) ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "Sale", "出售") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "Scrap", "报废"));
						}
						list.Add(item);
					}
					else
					{
						string changeValue = GetChangeValue(ctx, fixAssetsChangeLogNotPage, item);
						if (!string.IsNullOrWhiteSpace(changeValue))
						{
							item.MNote = changeValue;
							list.Add(item);
						}
					}
				}
			}
			else
			{
				FAFixAssetsChangeModel fAFixAssetsChangeModel = new FAFixAssetsChangeModel();
				FAFixAssetsChangeModel fixAssetsModel = faFixAssets.GetFixAssetsModel(ctx, filter.MID);
				fAFixAssetsChangeModel.MType = -1;
				fAFixAssetsChangeModel.MUserName = fixAssetsModel.MUserName;
				fAFixAssetsChangeModel.MCreateDate = fixAssetsModel.MCreateDate;
				list.Add(fAFixAssetsChangeModel);
			}
			return list;
		}

		private string GetChangeValue(MContext ctx, List<FAFixAssetsChangeModel> models, FAFixAssetsChangeModel model)
		{
			string result = string.Empty;
			int num = models.FindIndex((FAFixAssetsChangeModel m) => m.MItemID == model.MItemID) + 1;
			if (num < models.Count)
			{
				FAFixAssetsChangeModel nextItem = models[num];
				result = CompareItems(ctx, nextItem, model);
			}
			return result;
		}

		private string CompareItems(MContext ctx, FAFixAssetsChangeModel nextItem, FAFixAssetsChangeModel currentItem)
		{
			string text = string.Empty;
			if (nextItem.MNumber != currentItem.MNumber || nextItem.MPrefix != currentItem.MPrefix)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "FANumber", "编号") + " : " + nextItem.MPrefix + nextItem.MNumber + "->" + currentItem.MPrefix + currentItem.MNumber + ";";
			}
			if (nextItem.MName != currentItem.MName)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "Name", "名称") + " : " + nextItem.MName + "->" + currentItem.MName + ";";
			}
			if (nextItem.MQuantity != currentItem.MQuantity)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "Quantity", "数量") + " : " + nextItem.MQuantity + "->" + currentItem.MQuantity + ";";
			}
			if (nextItem.MDepreciationTypeID != currentItem.MDepreciationTypeID)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "DepreciationTypeName", "折旧方式") + " : " + nextItem.MDepreciationTypeName + "->" + currentItem.MDepreciationTypeName + ";";
			}
			DateTime dateTime;
			if (nextItem.MDepreciationFromPeriod != currentItem.MDepreciationFromPeriod)
			{
				string[] obj = new string[8]
				{
					text,
					" ",
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "DepreciationBeginPeriod", "折旧开始期间"),
					" : ",
					null,
					null,
					null,
					null
				};
				dateTime = nextItem.MDepreciationFromPeriod;
				obj[4] = dateTime.ToShortDateString();
				obj[5] = "->";
				dateTime = currentItem.MDepreciationFromPeriod;
				obj[6] = dateTime.ToShortDateString();
				obj[7] = ";";
				text = string.Concat(obj);
			}
			if (nextItem.MUsefulPeriods != currentItem.MUsefulPeriods)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "UsefulPeriods", "预计使用期数") + " : " + nextItem.MUsefulPeriods + "->" + currentItem.MUsefulPeriods + ";";
			}
			if (nextItem.MRateOfSalvage != currentItem.MRateOfSalvage)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "RateOfSalvage", "残值率") + " %  : " + nextItem.MRateOfSalvage + "->" + currentItem.MRateOfSalvage + ";";
			}
			if (nextItem.MDepreciatedAmount != currentItem.MDepreciatedAmount)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "DepreciatedAmount", "累计折旧") + " : " + nextItem.MDepreciatedAmount.ToMoneyFormat() + "->" + currentItem.MDepreciatedAmount.ToMoneyFormat() + ";";
			}
			if (nextItem.MPrepareForDecreaseAmount != currentItem.MPrepareForDecreaseAmount)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "PrepareForDecreaseAmount", "减值准备") + " : " + nextItem.MPrepareForDecreaseAmount.ToMoneyFormat() + "->" + currentItem.MPrepareForDecreaseAmount.ToMoneyFormat() + ";";
			}
			if (nextItem.MDepAccountCode != currentItem.MDepAccountCode)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "DepAccountCode", "折旧科目") + " : " + nextItem.MDepAccountName + "->" + currentItem.MDepAccountName + ";";
			}
			if (nextItem.MFixAccountCode != currentItem.MFixAccountCode)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "FixAccountCode", "固定资产科目") + " : " + nextItem.MFixAccountName + "->" + currentItem.MFixAccountName + ";";
			}
			if (nextItem.MExpAccountCode != currentItem.MExpAccountCode)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "ExpAccountCode", "费用科目") + " : " + nextItem.MExpAccountName + "->" + currentItem.MExpAccountName + ";";
			}
			if (nextItem.MDepCheckGroupValueID != currentItem.MDepCheckGroupValueID)
			{
				nextItem.MCheckGroupValueModel = utility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
				{
					nextItem.MDepCheckGroupValueID
				});
				currentItem.MCheckGroupValueModel = utility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
				{
					currentItem.MDepCheckGroupValueID
				});
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "DepCheckGroupValueID", "折旧科目核算维度") + " : " + GetCheckGroupValue(nextItem.MCheckGroupValueModel) + "->" + GetCheckGroupValue(currentItem.MCheckGroupValueModel) + ";";
			}
			if (nextItem.MFixCheckGroupValueID != currentItem.MFixCheckGroupValueID)
			{
				nextItem.MCheckGroupValueModel = utility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
				{
					nextItem.MFixCheckGroupValueID
				});
				currentItem.MCheckGroupValueModel = utility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
				{
					currentItem.MFixCheckGroupValueID
				});
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "FixCheckGroupValueID", "固定资产科目核算维度") + " : " + GetCheckGroupValue(nextItem.MCheckGroupValueModel) + "->" + GetCheckGroupValue(currentItem.MCheckGroupValueModel) + ";";
			}
			if (nextItem.MExpCheckGroupValueID != currentItem.MExpCheckGroupValueID)
			{
				nextItem.MCheckGroupValueModel = utility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
				{
					nextItem.MExpCheckGroupValueID
				});
				currentItem.MCheckGroupValueModel = utility.MergeCheckGroupValueModelByIDs(ctx, new List<string>
				{
					currentItem.MExpCheckGroupValueID
				});
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "ExpCheckGroupValueID", "固定资产科目核算维度") + " : " + GetCheckGroupValue(nextItem.MCheckGroupValueModel) + "->" + GetCheckGroupValue(currentItem.MCheckGroupValueModel) + ";";
			}
			if (nextItem.MPurchaseDate != currentItem.MPurchaseDate)
			{
				string[] obj2 = new string[8]
				{
					text,
					" ",
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "PurchaseDate", "采购日期"),
					" : ",
					null,
					null,
					null,
					null
				};
				dateTime = nextItem.MPurchaseDate;
				obj2[4] = dateTime.ToString("yyyy-MM-dd");
				obj2[5] = "->";
				dateTime = currentItem.MPurchaseDate;
				obj2[6] = dateTime.ToString("yyyy-MM-dd");
				obj2[7] = ";";
				text = string.Concat(obj2);
			}
			if (nextItem.MOriginalAmount != currentItem.MOriginalAmount)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "OriginalAmount", "原值") + " : " + nextItem.MOriginalAmount.ToMoneyFormat() + "->" + currentItem.MOriginalAmount.ToMoneyFormat() + ";";
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "BackAdjust", "追溯调整") + " : " + (currentItem.MBackAdjust ? COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "Back", "追溯") : COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "NotBack", "不追溯")) + ";";
			}
			else
			{
				dateTime = currentItem.MChangeFromPeriod;
				if (dateTime.Year >= 1900 && !currentItem.MBackAdjust)
				{
					text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "BackAdjust", "追溯调整") + " : " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "NotBack", "不追溯") + ";";
				}
			}
			if (nextItem.MPeriodDepreciatedAmount != currentItem.MPeriodDepreciatedAmount)
			{
				text = text + " " + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "PeriodDepreciatedAmount", "月折旧额") + " : " + nextItem.MPeriodDepreciatedAmount.ToMoneyFormat() + "->" + currentItem.MPeriodDepreciatedAmount.ToMoneyFormat() + ";";
			}
			dateTime = currentItem.MChangeFromPeriod;
			if (dateTime.Year > 1900)
			{
				string[] obj3 = new string[6]
				{
					text,
					" ",
					COMMultiLangRepository.GetText(ctx.MLCID, LangModule.FA, "ChangeEffectFromtPeriod", "变更开始影响期间"),
					" : ",
					null,
					null
				};
				dateTime = currentItem.MChangeFromPeriod;
				obj3[4] = dateTime.ToString("yyyy-MM");
				obj3[5] = ";";
				text = string.Concat(obj3);
			}
			if (text != string.Empty)
			{
				text = text.Trim().TrimEnd(';');
				text += ".";
			}
			return text;
		}

		private string GetCheckGroupValue(GLCheckGroupValueModel model)
		{
			string text = "";
			if (!string.IsNullOrEmpty(model.MContactID))
			{
				text = text + " " + model.MContactIDTitle + ":" + model.MContactName + ",";
			}
			if (!string.IsNullOrEmpty(model.MEmployeeID))
			{
				text = text + " " + model.MEmployeeIDTitle + ":" + model.MEmployeeName + ",";
			}
			if (!string.IsNullOrEmpty(model.MMerItemID))
			{
				text = text + " " + model.MMerItemIDTitle + ":" + model.MMerItemName + ",";
			}
			if (!string.IsNullOrEmpty(model.MExpItemID))
			{
				text = text + " " + model.MExpItemIDTitle + ":" + model.MExpItemName + ",";
			}
			if (!string.IsNullOrEmpty(model.MPaItemID))
			{
				text = text + " " + model.MPaItemIDTitle + ":" + model.MPaItemName + ",";
			}
			if (!string.IsNullOrEmpty(model.MTrackItem1))
			{
				text = text + " " + model.MTrackItem1Title + ":" + model.MTrackItem1Name + ",";
			}
			if (!string.IsNullOrEmpty(model.MTrackItem2))
			{
				text = text + " " + model.MTrackItem2Title + ":" + model.MTrackItem2Name + ",";
			}
			if (!string.IsNullOrEmpty(model.MTrackItem3))
			{
				text = text + " " + model.MTrackItem3Title + ":" + model.MTrackItem3Name + ",";
			}
			if (!string.IsNullOrEmpty(model.MTrackItem4))
			{
				text = text + " " + model.MTrackItem4Title + ":" + model.MTrackItem4Name + ",";
			}
			if (!string.IsNullOrEmpty(model.MTrackItem5))
			{
				text = text + " " + model.MTrackItem5Title + ":" + model.MTrackItem5Name + ",";
			}
			return text.Trim().TrimEnd(',');
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, FAFixAssetsChangeModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<FAFixAssetsChangeModel> modelData, string fields = null)
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

		public FAFixAssetsChangeModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public FAFixAssetsChangeModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<FAFixAssetsChangeModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<FAFixAssetsChangeModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
