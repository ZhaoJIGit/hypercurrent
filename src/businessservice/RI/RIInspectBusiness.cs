using JieNor.Megi.BusinessContract.RI;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.RI;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace JieNor.Megi.BusinessService.RI
{
	public class RIInspectBusiness : IRIInspectBusiness, IDataContract<RICategoryModel>
	{
		private RICategoryRepository dal = new RICategoryRepository();

		private static Hashtable CategoryTableCache = Hashtable.Synchronized(new Hashtable());

		private static List<Func<MContext, RICategoryModel, int, int, RIInspectionResult>> funcs = new RICurrentInspector().enginers.Concat(new RIFixAssetsInspector().enginers).Concat(new RIMonthlyInspector().enginers).Concat(new RIExpenseTaxInspector().enginers)
			.Concat(new RIOtherInspector().enginers)
			.Concat(new RIBusinessInspector().enginers)
			.ToList();

		private List<RICategoryModel> FilterCategoryListByConfig(List<RICategoryModel> list)
		{
			string text = ConfigurationManager.AppSettings["InspectFilterStart"];
			if (!string.IsNullOrWhiteSpace(text))
			{
				List<string> filters = text.Trim().Split(',').ToList();
				if (filters != null || filters.Count > 0)
				{
					return list.Where(delegate(RICategoryModel x)
					{
						for (int i = 0; i < filters.Count; i++)
						{
							if (x.MItemID.StartsWith(filters[i]))
							{
								return false;
							}
						}
						return true;
					}).ToList();
				}
			}
			return list;
		}

		public List<RICategoryModel> GetCategoryList(MContext ctx, bool includeSettingDisable = true, int year = 0, int period = 0)
		{
			List<RICategoryModel> list = GetCategoryListFromCache(ctx);
			if (!includeSettingDisable)
			{
				list = (from x in list
				where x.MSetting.MEnable
				select x).ToList();
			}
			list = FilterCategoryListByConfig(list);
			if (year != 0 && period != 0)
			{
				GLDataPool.RemovePool(ctx, true);
				RICategoryModel rICategoryModel = list.FirstOrDefault((RICategoryModel x) => x.MItemID == "4120");
				RICategoryModel rICategoryModel2 = list.FirstOrDefault((RICategoryModel x) => x.MItemID == "4220");
				RICategoryModel rICategoryModel3 = list.FirstOrDefault((RICategoryModel x) => x.MItemID == "4320");
				int num = (rICategoryModel != null && rICategoryModel.MSetting.MSettingParam != null && !string.IsNullOrWhiteSpace(rICategoryModel.MSetting.MSettingParam.MCompareValue)) ? int.Parse(rICategoryModel.MSetting.MSettingParam.MCompareValue) : 0;
				int num2 = (rICategoryModel2 != null && rICategoryModel2.MSetting.MSettingParam != null && !string.IsNullOrWhiteSpace(rICategoryModel2.MSetting.MSettingParam.MCompareValue)) ? int.Parse(rICategoryModel2.MSetting.MSettingParam.MCompareValue) : 0;
				int num3 = (rICategoryModel3 != null && rICategoryModel3.MSetting.MSettingParam != null && !string.IsNullOrWhiteSpace(rICategoryModel3.MSetting.MSettingParam.MCompareValue)) ? int.Parse(rICategoryModel3.MSetting.MSettingParam.MCompareValue) : 0;
				int num4 = (num > num2) ? num : num2;
				num4 = ((num4 > num3) ? num4 : num3);
				num4 = ((num4 > 6) ? num4 : 6);
				GLDataPool.GetInstance(ctx, true, year, period, num4);
			}
			return list;
		}

		private List<RICategoryModel> GetCategoryListFromCache(MContext ctx)
		{
			List<RICategoryModel> list = null;
			string key = ctx.MOrgID + "_" + ctx.MLCID;
			if (!CategoryTableCache.ContainsKey(key))
			{
				list = dal.GetCategoryList(ctx, ctx.MLCID);
				if (list == null || list.Count == 0)
				{
					new BDInspectRepository().InitInspectItem(ctx);
					list = dal.GetCategoryList(ctx, ctx.MLCID);
				}
				AddCategory(ctx, list, null, false);
			}
			return CategoryTableCache[key] as List<RICategoryModel>;
		}

		public void UpdateCategory(MContext ctx)
		{
			List<RICategoryModel> categoryList = dal.GetCategoryList(ctx, "0x0009");
			List<RICategoryModel> categoryList2 = dal.GetCategoryList(ctx, "0x7804");
			List<RICategoryModel> categoryList3 = dal.GetCategoryList(ctx, "0x7C04");
			AddCategory(ctx, categoryList, "0x0009", true);
			AddCategory(ctx, categoryList2, "0x7804", true);
			AddCategory(ctx, categoryList3, "0x7C04", true);
		}

		private void AddCategory(MContext ctx, List<RICategoryModel> list, string localeID = null, bool force = false)
		{
			string key = ctx.MOrgID + "_" + (localeID ?? ctx.MLCID);
			lock (CategoryTableCache.SyncRoot)
			{
				if (!CategoryTableCache.ContainsKey(key) | force)
				{
					CategoryTableCache[key] = list;
				}
			}
		}

		public RIInspectionResult Inspect(MContext ctx, RICategoryModel category, int year, int period)
		{
			Func<MContext, RICategoryModel, int, int, RIInspectionResult> func = GetFunc(ctx, category);
			return func(ctx, category, year, period);
		}

		public RIInspectionResult Inspect(MContext ctx, string settingId, int year, int period)
		{
			List<RICategoryModel> categoryListFromCache = GetCategoryListFromCache(ctx);
			RICategoryModel category = categoryListFromCache.FirstOrDefault((RICategoryModel x) => x.MSetting.MItemID == settingId);
			RIInspectionResult result = Inspect(ctx, category, year, period);
			return AssembleResult(ctx, category, result);
		}

		public RIInspectionResult AssembleResult(MContext ctx, RICategoryModel category, RIInspectionResult result)
		{
			result.MRequirePass = category.MSetting.MRequirePass;
			result.MID = category.MSetting.MItemID;
			RICategoryModel rICategoryModel = GetCategoryList(ctx, true, 0, 0).FirstOrDefault((RICategoryModel x) => x.MItemID == category.MParentID);
			result.MTopFailedMessage = rICategoryModel.MFailTextString;
			result.MTopPassedMessage = rICategoryModel.MPassTextString;
			result.MItemID = category.MItemID;
			result.MParentID = rICategoryModel.MItemID;
			if (result.children != null && result.children.Count > 0)
			{
				result.children.ForEach(delegate(RIInspectionResult x)
				{
					string format2 = x.MPassed ? category.MPassTextString : category.MFailTextString;
					object[] mMessageParam2 = x.MMessageParam;
					x.MMessage = string.Format(format2, mMessageParam2);
					object mLinkUrl4;
					if (!result.MPassed || !result.MNoLinkUrlIfPassed)
					{
						if (!string.IsNullOrWhiteSpace(category.MLinkUrl))
						{
							string mLinkUrl3 = category.MLinkUrl;
							mMessageParam2 = x.MUrlParam;
							mLinkUrl4 = string.Format(mLinkUrl3, mMessageParam2);
						}
						else
						{
							mLinkUrl4 = "";
						}
					}
					else
					{
						mLinkUrl4 = string.Empty;
					}
					x.MLinkUrl = (string)mLinkUrl4;
				});
				result.MMessage = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "itemsFailded", "{0}项检查不通过"), result.children.Count);
				result.MLinkUrl = string.Empty;
			}
			else
			{
				RIInspectionResult rIInspectionResult = result;
				string format = result.MPassed ? category.MPassTextString : category.MFailTextString;
				object[] mMessageParam = result.MMessageParam;
				rIInspectionResult.MMessage = string.Format(format, mMessageParam);
				RIInspectionResult rIInspectionResult2 = result;
				object mLinkUrl2;
				if (!result.MNoLinkUrlIfPassed || !result.MPassed)
				{
					if (!string.IsNullOrWhiteSpace(category.MLinkUrl))
					{
						string mLinkUrl = category.MLinkUrl;
						mMessageParam = result.MUrlParam;
						mLinkUrl2 = string.Format(mLinkUrl, mMessageParam);
					}
					else
					{
						mLinkUrl2 = "";
					}
				}
				else
				{
					mLinkUrl2 = string.Empty;
				}
				rIInspectionResult2.MLinkUrl = (string)mLinkUrl2;
			}
			return result;
		}

		private Func<MContext, RICategoryModel, int, int, RIInspectionResult> GetFunc(MContext ctx, RICategoryModel category)
		{
			for (int i = 0; i < funcs.Count; i++)
			{
				if (funcs[i].Method.Name.Equals(category.MFuncName, StringComparison.OrdinalIgnoreCase))
				{
					return funcs[i];
				}
			}
			return null;
		}

		public OperationResult ClearDataPool(MContext ctx)
		{
			GLDataPool.RemovePool(ctx, true);
			return new OperationResult();
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, RICategoryModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<RICategoryModel> modelData, string fields = null)
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

		public RICategoryModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public RICategoryModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<RICategoryModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<RICategoryModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
