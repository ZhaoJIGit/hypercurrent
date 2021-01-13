using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text;

namespace JieNor.Megi.DataRepository.FA
{
	public class FAFixAssetsRepository : DataServiceT<FAFixAssetsModel>
	{
		public GLUtility utility = new GLUtility();

		public List<NameValueModel> GetFixAssetsTabInfo(MContext ctx)
		{
			string sql = "select MStatus, count(1) as MCount from t_fa_fixAssets where MOrgID = @MOrgID and MisDelete = 0 group by MStatus ";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, ctx.GetParameters((MySqlParameter)null));
			if (dataSet != null && dataSet.Tables.Count != 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				List<NameValueModel> list = new List<NameValueModel>();
				for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
				{
					list.Add(new NameValueModel
					{
						MName = dataSet.Tables[0].Rows[i].MField<int>("MStatus").ToString(),
						MValue = dataSet.Tables[0].Rows[i].MField<long>("MCount").ToString()
					});
				}
				return list;
			}
			return new List<NameValueModel>();
		}

		public BASOrgPrefixSettingModel GetNextFixAssetsNumber(MContext ctx)
		{
			BASOrgPrefixSettingModel orgPrefixSettingModel = new BASOrgPrefixSettingRepository().GetOrgPrefixSettingModel(ctx, "FixAssets");
			string sql = "select (ifnull(max(CAST(MNumber AS SIGNED)),0))  as NextNumber from t_fa_fixassets where MOrgID = @MOrgID and MIsDelete = 0 and MPrefix = @MPrefix ";
			MySqlParameter[] parameters = ctx.GetParameters("@MPrefix", orgPrefixSettingModel.MPrefix);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, parameters);
			orgPrefixSettingModel.MValue = int.Parse(dataSet.Tables[0].Rows[0].MField<long>("NextNumber").ToString());
			orgPrefixSettingModel.MValue = ((orgPrefixSettingModel.MValue == 0) ? orgPrefixSettingModel.MStartIndex : (orgPrefixSettingModel.MValue + 1));
			return orgPrefixSettingModel;
		}

		public string getQueryFixAssetsSql(MContext ctx, FAFixAssetsFilterModel filter, bool count = false)
		{
			string arg = (filter != null && !string.IsNullOrWhiteSpace(filter.CheckGroup) && !string.IsNullOrWhiteSpace(filter.CheckGroupValue)) ? "LEFT JOIN\n                    t_gl_checkgroupvalue t4 ON t1.MFixCheckGroupValueID = t4.MItemID\n                        AND t4.MOrgID = t1.MOrgID\n                        AND t4.MisDelete = t1.MisDelete\n                        LEFT JOIN\n                    t_gl_checkgroupvalue t5 ON t1.MDepCheckGroupValueID = t5.MItemID\n                        AND t5.MOrgID = t1.MOrgID\n                        AND t5.MisDelete = t1.MisDelete\n                        LEFT JOIN\n                    t_gl_checkgroupvalue t6 ON t1.MExpCheckGroupValueID = t6.MItemID\n                        AND t6.MOrgID = t1.MOrgID\n                        AND t6.MisDelete = t1.MisDelete " : "";
			string arg2 = (filter != null && !string.IsNullOrWhiteSpace(filter.CheckGroup) && !string.IsNullOrWhiteSpace(filter.CheckGroupValue)) ? " AND (t4.{0} = @MCheckGroupValueID OR t5.{0} = @MCheckGroupValueID\n                                OR t6.{0} = @MCheckGroupValueID) " : "";
			string text = "SELECT \n                                t1.MItemID " + (count ? "" : ",\n                                t1.MOrgID,\n                                t1.MChanged,\n                                t1.MPrefix,\n                                t1.MNumber,\n                                t1.MPurchaseDate,\n                                t1.MFATypeID,\n                                t1.MHandledDate,\n                                t1.MStatus,\n                                t1.MQuantity,\n                                t1.MDepreciationTypeID,\n                                t1.MUsefulPeriods,\n                                t1.MDepreciationFromPeriod,\n                                t1.MFixAccountCode,\n                                t1.MDepAccountCode,\n                                t1.MExpAccountCode,\n                                t1.MFixCheckGroupValueID,\n                                t1.MDepCheckGroupValueID,\n                                t1.MExpCheckGroupValueID,\n                                t1.MOriginalAmount,\n                                t1.MPeriodDepreciatedAmount as MSrcPeriodDepreciatedAmount,\n                                t1.MDepreciatedAmountOfYear as MSrcDepreciatedAmountOfYear,\n                                t1.MDepreciatedPeriods as MSrcDepreciatedPeriods,\n                                t1.MDepreciatedAmount as MSrcDepreciatedAmount,\n                                t1.MNetAmount as MSrcNetAmount,\n                                IFNULL(t10.MPeriodDepreciatedAmount,\n                                        t1.MPeriodDepreciatedAmount) as MPeriodDepreciatedAmount,\n                                IFNULL(t10.MDepreciatedAmountOfYear,\n                                        t1.MDepreciatedAmountOfYear) as MDepreciatedAmountOfYear,\n                                t1.MRateOfSalvage,\n                                t1.MSalvageAmount,\n                                t1.MPrepareForDecreaseAmount,\n                                IFNULL(t10.MDepreciatedPeriods,\n                                        t1.MDepreciatedPeriods) as MDepreciatedPeriods,\n                                IFNULL(t10.MDepreciatedAmount,\n                                        t1.MDepreciatedAmount) as MDepreciatedAmount,\n                                t1.MLastDepreciatedYear,\n                                t1.MLastDepreciatedPeriod,\n                                IFNULL(t10.MNetAmount, t1.MNetAmount) as MNetAmount,\n                                t1.MRemark,\n                                t1.MIsDelete,\n                                t1.MCreatorID,\n                                t1.MCreateDate,\n                                t1.MModifierID,\n                                t1.MModifyDate,\n                                t2.MName,\n                                t3.MDepreciationFromCurrentPeriod,\n                                CONCAT(t3.MNumber, '-', t3_L.MName) AS MFATypeIDName ") + "\n                            FROM\n                                t_fa_fixassets t1\n                                    INNER JOIN\n                                t_fa_fixassets_l t2 ON t1.MItemID = t2.MParentId\n                                    AND t2.MLocaleID = @MLocaleID\n                                    AND t2.MIsDelete = t1.MIsDelete\n                                    AND t2.MOrgID = t1.MOrgID\n                                    INNER JOIN\n                                t_fa_fixassetstype t3 ON t1.MOrgID = t3.MOrgID\n                                    AND t3.MItemID = t1.MFATypeID\n                                    AND t1.MIsDelete = t3.MIsDelete\n                                    INNER JOIN\n                                t_fa_fixassetstype_l t3_L ON t1.MOrgID = t3_L.MOrgID\n                                    AND t1.MIsDelete = t3_L.MIsDelete\n                                    AND t3_L.MLocaleID = @MLocaleID\n                                    AND t1.MFATypeID = t3_L.MParentID " + (count ? "" : "\n                                    LEFT JOIN\n                               (SELECT \n                                    MID,\n                                    MOrgID,\n                                    MVoucherID,\n                                    MYear,\n                                    MPeriod,\n                                    MIsDelete,\n                                    SUM(MNetAmount) MNetAmount,\n                                    SUM(MPeriodDepreciatedAmount) AS MPeriodDepreciatedAmount,\n                                    SUM(MDepreciatedAmount) AS MDepreciatedAmount,\n                                    SUM(MDepreciatedAmountOfYear) AS MDepreciatedAmountOfYear,\n                                    SUM(MDepreciatedPeriods) as MDepreciatedPeriods,\n                                    (SUM(MPeriodDepreciatedAmount) - sum(MAdjustAmount)) as MTempPeriodDepreciatedAmount,\n                                    SUM(MAdjustAmount) as MLastAdjustAmount\n                                FROM\n                                    t_fa_depreciation t20\n                                WHERE\n                                    t20.MOrgID = @MOrgID\n                                        AND t20.MIsDelete = 0\n                                GROUP BY MID , MOrgID , MVoucherID, MYear , MPeriod , MIsDelete ) t10 ON t1.MOrgID = t10.MOrgID\n                                    AND t1.MLastDepreciatedYear = t10.MYear\n                                    AND t1.MLastDepreciatedPeriod = t10.MPeriod\n                                    AND t10.MID = t1.MItemID\n                                    AND t1.MIsDelete = t10.MIsDelete ") + "\n                                    {0}\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\r\n                                    {1}";
			if (filter != null && !string.IsNullOrWhiteSpace(filter.MItemID))
			{
				text += " and t1.MItemID = @MItemID ";
			}
			if (filter != null && !string.IsNullOrWhiteSpace(filter.Number))
			{
				text += " and concat(t1.MPrefix,t1.MNumber) = @MNumber";
			}
			if (filter.ItemIDs != null && filter.ItemIDs.Count > 0)
			{
				text = text + " and t1.MItemID in ('" + string.Join("','", filter.ItemIDs) + "') ";
			}
			if (filter != null && !string.IsNullOrWhiteSpace(filter.Keyword))
			{
				string str = (filter != null && filter.MKeyword.HasValue) ? " OR \r\n                        t1.MPeriodDepreciatedAmount = @MKeyword OR\r\n                        t1.MOriginalAmount = @MKeyword OR\r\n                        t1.MDepreciatedAmount = @MKeyword\r\n                    " : "";
				text = text + " and (concat(t1.MPrefix, t1.MNumber) like concat('%',@Keyword,'%')\r\n                       OR t1.MPurchaseDate like binary concat('%',@Keyword,'%')\r\n                       OR t2.MName like concat('%',@Keyword,'%')\r\n                       OR t1.MDepreciationFromPeriod like binary concat('%',@Keyword,'%')\r\n                       OR CONCAT(t3.MNumber, '-', t3_L.MName) like concat('%',@Keyword,'%')\r\n                       " + str + ")";
			}
			if (filter != null && filter.Status == 0)
			{
				text += " and t1.MStatus = 0";
			}
			else if (filter != null && filter.Status == 1)
			{
				text += " and ( t1.MStatus = 1 or t1.MStatus = 2)";
			}
			text = ((!filter.NotNeedPage) ? (count ? (" select count(*) as MCount from (" + text + ") t") : ("select * from (" + text + " order by concat( t1.MPrefix, t1.MNumber) desc limit " + (filter.page - 1) * filter.rows + "," + filter.rows + ") t ")) : ("select * from (" + text + ") t order by concat( MPrefix, MNumber) desc "));
			return string.Format(string.Format(text, arg, arg2), filter.CheckGroup);
		}

		public int GetFixAssetsPageListCount(MContext ctx, FAFixAssetsFilterModel filter = null)
		{
			string queryFixAssetsSql = getQueryFixAssetsSql(ctx, filter, true);
			MySqlParameter[] parameters = GetParameters(ctx, filter);
			object single = new DynamicDbHelperMySQL(ctx).GetSingle(queryFixAssetsSql, parameters);
			int result = 0;
			int.TryParse(single.ToString(), out result);
			return result;
		}

		private MySqlParameter[] GetParameters(MContext ctx, FAFixAssetsFilterModel filter)
		{
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.Add(new MySqlParameter("@MNumber", filter.Number));
			list.Add(new MySqlParameter("@Keyword", filter.Keyword));
			list.Add(new MySqlParameter("@MItemID", filter.MItemID));
			list.Add(new MySqlParameter("@MKeyword", filter.MKeyword.HasValue ? filter.MKeyword.Value : decimal.Zero));
			list.Add(new MySqlParameter("@MCheckGroupValueID", filter.CheckGroupValue));
			return list.ToArray();
		}

		public List<FAFixAssetsModel> GetFixAssetsPageList(MContext ctx, FAFixAssetsFilterModel filter = null)
		{
			string queryFixAssetsSql = getQueryFixAssetsSql(ctx, filter, false);
			MySqlParameter[] parameters = GetParameters(ctx, filter);
			List<FAFixAssetsModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<FAFixAssetsModel>(ctx, queryFixAssetsSql, parameters);
			return dataModelBySql.ToList();
		}
		private void Log(string text)
		{
			string logFilePath = @"D:\MegiIIS\go.megichina.com\Log\";
			try
			{
				if (!Directory.Exists(logFilePath))
				{
					Directory.CreateDirectory(logFilePath);
				}
				string arg = logFilePath;
				DateTime now = DateTime.Now;
				string fileName = string.Format("{0}{1}.txt", arg, now.ToString("yyyy-MM-dd"));
				StringBuilder builder = new StringBuilder();
				StringBuilder stringBuilder = builder;
				now = DateTime.Now;
				stringBuilder.AppendLine(now.ToString("yyyy-MM-dd hh:mm:ss") + ":" + text);
				File.AppendAllText(fileName, builder.ToString());
			}
			catch (Exception)
			{
			}
		}
		public List<FAFixAssetsModel> GetFixAssetsList(MContext ctx, FAFixAssetsFilterModel filter)
		{
			filter = (filter ?? new FAFixAssetsFilterModel());
			filter.NotNeedPage = true;
			string queryFixAssetsSql = getQueryFixAssetsSql(ctx, filter, false);
			MySqlParameter[] parameters = GetParameters(ctx, filter);
			List<FAFixAssetsModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<FAFixAssetsModel>(ctx, queryFixAssetsSql, parameters);
			return (from x in dataModelBySql
			where !x.MIsAdjust
			select x).ToList();
		}

		public OperationResult DeleteFixAssets(MContext ctx, List<string> itemIDs)
		{
			List<NameValueModel> list = CheckFixAssetsAreDepreciated(ctx, itemIDs);
			if (list != null && list.Count > 0)
			{
				return new OperationResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(LangModule.FA, "FixAssetsAsFollowAreDrepciated", "以下资产已经开始计提折旧，无法删除:") + string.Join(",", list)
				};
			}
			return DeleteModels(ctx, itemIDs);
		}

		private List<NameValueModel> CheckFixAssetsAreDepreciated(MContext ctx, List<string> itemIDs)
		{
			string sql = "SELECT \n                          t1.MNumber as MName\n                        FROM\n                            t_fa_fixassets t1 \n                        WHERE\n                            t1.MItemID in ('" + string.Join("','", itemIDs) + "') and t1.MIsDelete = 0 and t1.MOrgID = @MOrgID and t1.MLastDepreciatedYear> 2000 ";
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, ctx.GetParameters((MySqlParameter)null));
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				return ModelInfoManager.DataTableToList<NameValueModel>(dataSet);
			}
			return null;
		}

		public List<CommandInfo> GetSaveFixAssetsCmds(MContext ctx, FAFixAssetsModel model, FAFixAssetsModel srcModel = null)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			ValidateFixAssets(ctx, model);
			List<string> fields = null;
			int num = model.MLastDepreciatedYear * 12 + model.MLastDepreciatedPeriod;
			DateTime dateTime = ctx.MFABeginDate;
			int num2 = dateTime.Year * 12;
			dateTime = ctx.MFABeginDate;
			if (num < num2 + dateTime.Month)
			{
				model.MDepreciatedAmountOfYear = model.MDepreciatedAmount;
			}
			if (model.MIsChange && !string.IsNullOrWhiteSpace(model.MItemID))
			{
				srcModel = (srcModel ?? GetDataModel(ctx, model.MItemID, false));
				int maxChangeIndex = GetMaxChangeIndex(ctx, model.MItemID);
				if (maxChangeIndex == 0)
				{
					FAFixAssetsChangeModel modelData = CopyFromModel2ChangeModel(ctx, srcModel);
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FAFixAssetsChangeModel>(ctx, modelData, null, true));
				}
				FAFixAssetsChangeModel fAFixAssetsChangeModel = CopyFromModel2ChangeModel(ctx, model);
				fAFixAssetsChangeModel.MIndex = maxChangeIndex + 1;
				fAFixAssetsChangeModel.MChanged = true;
				fAFixAssetsChangeModel.MType = GetChangeType(ctx, model, GetDataModel(ctx, model.MItemID, false));
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FAFixAssetsChangeModel>(ctx, fAFixAssetsChangeModel, null, true));
				model.MChanged = true;
				model.MLastDepreciatedPeriod = srcModel.MLastDepreciatedPeriod;
				model.MLastDepreciatedYear = srcModel.MLastDepreciatedYear;
				if (srcModel.MLastDepreciatedYear > 1900)
				{
					dateTime = srcModel.MPurchaseDate;
					int num3 = dateTime.Year * 12;
					dateTime = srcModel.MPurchaseDate;
					int num4 = num3 + dateTime.Month;
					dateTime = model.MPurchaseDate;
					int num5 = num4 - dateTime.Year * 12;
					dateTime = model.MPurchaseDate;
					if (num5 - dateTime.Month != 0)
					{
						List<CommandInfo> list2 = list;
						CommandInfo obj = new CommandInfo
						{
							CommandText = "update t_fa_depreciation t set t.MDepreciatedPeriods = @NewPeriods where t.MOrgID = @MFixAssetsOrgID and  t.MID = @MFixAssetsID and t.MYear = @MDeprecationYear and t.MPeriod = @MDepreciationPeriod and t.MIsDelete = 0 "
						};
						DbParameter[] array = obj.Parameters = new List<MySqlParameter>
						{
							new MySqlParameter("@NewPeriods", model.MDepreciatedPeriods),
							new MySqlParameter("@MFixAssetsID", srcModel.MItemID),
							new MySqlParameter("@MDeprecationYear", srcModel.MLastDepreciatedYear),
							new MySqlParameter("@MDepreciationPeriod", srcModel.MLastDepreciatedPeriod),
							new MySqlParameter("@MFixAssetsOrgID", ctx.MOrgID)
						}.ToArray();
						list2.Add(obj);
					}
					if (fAFixAssetsChangeModel.MType >= FixAssetsChangeTypeEnum.Strategy)
					{
						List<CommandInfo> list3 = list;
						CommandInfo obj2 = new CommandInfo
						{
							CommandText = "update t_fa_depreciation t set MIsDelete = 0 where (MYear * 12 + MPeriod) >= @MYearPeriod and MOrgID =  @MFixAssetsOrgID and MID = @MFixAssetsID and MIsDelete = 0 "
						};
						List<MySqlParameter> obj3 = new List<MySqlParameter>
						{
							new MySqlParameter("@MFixAssetsID", srcModel.MItemID)
						};
						dateTime = srcModel.MChangeFromPeriod;
						int num6 = dateTime.Year * 12;
						dateTime = srcModel.MChangeFromPeriod;
						obj3.Add(new MySqlParameter("@MYearPeriod", num6 + dateTime.Month));
						obj3.Add(new MySqlParameter("@MFixAssetsOrgID", ctx.MOrgID));
						DbParameter[] array = obj2.Parameters = obj3.ToArray();
						list3.Add(obj2);
						fields = new List<string>
						{
							"MChanged",
							"MPrefix",
							"MNumber",
							"MPurchaseDate",
							"MFATypeID",
							"MHandledDate",
							"MStatus",
							"MQuantity",
							"MDepreciationTypeID",
							"MUsefulPeriods",
							"MFixAccountCode",
							"MDepAccountCode",
							"MExpAccountCode",
							"MFixCheckGroupValueID",
							"MDepCheckGroupValueID",
							"MExpCheckGroupValueID",
							"MOriginalAmount",
							"MNetAmount",
							"MSalvageAmount",
							"MRateOfSalvage",
							"MPrepareForDecreaseAmount",
							"MRemark"
						};
					}
				}
			}
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<FAFixAssetsModel>(ctx, model, fields, true));
			return list;
		}

		public OperationResult SaveFixAssets(MContext ctx, FAFixAssetsModel model)
		{
			OperationResult operationResult = new OperationResult();
			List<CommandInfo> saveFixAssetsCmds = GetSaveFixAssetsCmds(ctx, model, null);
			operationResult.ObjectID = model.MItemID;
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(saveFixAssetsCmds) > 0);
			return operationResult;
		}

		public List<CommandInfo> GetInsertOrUpdateCmdList(MContext ctx, FAFixAssetsModel model)
		{
			return GetSaveFixAssetsCmds(ctx, model, null);
		}

		private int GetChangeType(MContext ctx, FAFixAssetsModel changeModel, FAFixAssetsModel srcModel)
		{
			int num = 0;
			num += ((changeModel.MName != srcModel.MName || changeModel.MNumber != srcModel.MNumber || changeModel.MPrefix != srcModel.MPrefix || changeModel.MQuantity != srcModel.MQuantity) ? FixAssetsChangeTypeEnum.Basic : 0);
			num += ((changeModel.MDepAccountCode != srcModel.MDepAccountCode || changeModel.MFixAccountCode != srcModel.MFixAccountCode || changeModel.MExpAccountCode != srcModel.MExpAccountCode || changeModel.MDepreciatedPeriods != srcModel.MDepreciatedPeriods || changeModel.MDepreciatedAmount != srcModel.MDepreciatedAmount || changeModel.MPeriodDepreciatedAmount != srcModel.MPeriodDepreciatedAmount) ? FixAssetsChangeTypeEnum.Other : 0);
			num += ((changeModel.MDepreciationTypeID != srcModel.MDepreciationTypeID || changeModel.MUsefulPeriods != srcModel.MUsefulPeriods || changeModel.MRateOfSalvage != srcModel.MRateOfSalvage) ? FixAssetsChangeTypeEnum.Strategy : 0);
			int num2 = num;
			int num6;
			if (!(changeModel.MOriginalAmount != srcModel.MOriginalAmount))
			{
				DateTime mPurchaseDate = changeModel.MPurchaseDate;
				int num3 = mPurchaseDate.Year * 12;
				mPurchaseDate = changeModel.MPurchaseDate;
				int num4 = num3 + mPurchaseDate.Month;
				mPurchaseDate = srcModel.MPurchaseDate;
				int num5 = mPurchaseDate.Year * 12;
				mPurchaseDate = srcModel.MPurchaseDate;
				if (num4 == num5 + mPurchaseDate.Month && !(changeModel.MPrepareForDecreaseAmount != srcModel.MPrepareForDecreaseAmount))
				{
					num6 = 0;
					goto IL_0182;
				}
			}
			num6 = FixAssetsChangeTypeEnum.Original;
			goto IL_0182;
			IL_0182:
			num = num2 + num6;
			num += ((changeModel.MStatus != srcModel.MStatus) ? FixAssetsChangeTypeEnum.Handle : 0);
			return (num == 0) ? FixAssetsChangeTypeEnum.Other : num;
		}

		private FAFixAssetsChangeModel CopyFromModel2ChangeModel(MContext ctx, FAFixAssetsModel model)
		{
			FAFixAssetsChangeModel obj = new FAFixAssetsChangeModel
			{
				MOrgID = ctx.MOrgID,
				MID = model.MItemID,
				MItemID = UUIDHelper.GetGuid(),
				IsNew = true
			};
			DateTime now = DateTime.Now;
			obj.MYear = now.Year;
			now = DateTime.Now;
			obj.MPeriod = now.Month;
			obj.MNumber = model.MNumber;
			obj.MPrefix = model.MPrefix;
			obj.MName = model.MName;
			obj.MPurchaseDate = model.MPurchaseDate;
			obj.MFATypeID = model.MFATypeID;
			obj.MStatus = model.MStatus;
			obj.MHandledDate = model.MHandledDate;
			obj.MQuantity = model.MQuantity;
			obj.MDepreciationTypeID = model.MDepreciationTypeID;
			obj.MUsefulPeriods = model.MUsefulPeriods;
			obj.MDepreciationFromPeriod = model.MDepreciationFromPeriod;
			obj.MFixAccountCode = model.MFixAccountCode;
			obj.MDepAccountCode = model.MDepAccountCode;
			obj.MExpAccountCode = model.MExpAccountCode;
			obj.MFixCheckGroupValueID = model.MFixCheckGroupValueID;
			obj.MDepCheckGroupValueID = model.MDepCheckGroupValueID;
			obj.MExpCheckGroupValueID = model.MExpCheckGroupValueID;
			obj.MOriginalAmount = model.MOriginalAmount;
			obj.MPrepareForDecreaseAmount = model.MPrepareForDecreaseAmount;
			obj.MRateOfSalvage = model.MRateOfSalvage;
			obj.MSalvageAmount = model.MSalvageAmount;
			obj.MLastDepreciatedPeriod = model.MLastDepreciatedPeriod;
			obj.MLastDepreciatedYear = model.MLastDepreciatedYear;
			obj.MPeriodDepreciatedAmount = model.MPeriodDepreciatedAmount;
			obj.MDepreciatedPeriods = model.MDepreciatedPeriods;
			obj.MDepreciatedAmount = model.MDepreciatedAmount;
			obj.MRemark = model.MRemark;
			obj.MChangeFromPeriod = model.MChangeFromPeriod;
			obj.MBackAdjust = model.MBackAdjust;
			FAFixAssetsChangeModel fAFixAssetsChangeModel = obj;
			fAFixAssetsChangeModel.MultiLanguage = CopyMultiLanguage(model.MultiLanguage, fAFixAssetsChangeModel.MItemID);
			return fAFixAssetsChangeModel;
		}

		private FAFixAssetsModel CopyFromModel(MContext ctx, FAFixAssetsModel model)
		{
			FAFixAssetsModel fAFixAssetsModel = new FAFixAssetsModel
			{
				MOrgID = ctx.MOrgID,
				MItemID = model.MItemID,
				MNumber = model.MNumber,
				MName = model.MName,
				MPrefix = model.MPrefix,
				MPurchaseDate = model.MPurchaseDate,
				MFATypeID = model.MFATypeID,
				MStatus = model.MStatus,
				MHandledDate = model.MHandledDate,
				MQuantity = model.MQuantity,
				MDepreciationTypeID = model.MDepreciationTypeID,
				MUsefulPeriods = model.MUsefulPeriods,
				MDepreciationFromPeriod = model.MDepreciationFromPeriod,
				MFixAccountCode = model.MFixAccountCode,
				MDepAccountCode = model.MDepAccountCode,
				MExpAccountCode = model.MExpAccountCode,
				MFixCheckGroupValueID = model.MFixCheckGroupValueID,
				MDepCheckGroupValueID = model.MDepCheckGroupValueID,
				MExpCheckGroupValueID = model.MExpCheckGroupValueID,
				MOriginalAmount = model.MOriginalAmount,
				MPrepareForDecreaseAmount = model.MPrepareForDecreaseAmount,
				MRateOfSalvage = model.MRateOfSalvage,
				MSalvageAmount = model.MSalvageAmount,
				MLastDepreciatedPeriod = model.MLastDepreciatedPeriod,
				MPeriodDepreciatedAmount = model.MPeriodDepreciatedAmount,
				MDepreciatedPeriods = model.MDepreciatedPeriods,
				MDepreciatedAmount = model.MDepreciatedAmount,
				MRemark = model.MRemark,
				MDepreciatedAmountOfYear = model.MDepreciatedAmountOfYear,
				MChangeFromPeriod = model.MChangeFromPeriod,
				MBackAdjust = model.MBackAdjust
			};
			fAFixAssetsModel.MultiLanguage = CopyMultiLanguage(model.MultiLanguage, fAFixAssetsModel.MItemID);
			return fAFixAssetsModel;
		}

		private List<MultiLanguageFieldList> CopyMultiLanguage(List<MultiLanguageFieldList> src, string parentID)
		{
			List<MultiLanguageFieldList> list = new List<MultiLanguageFieldList>();
			for (int i = 0; i < src.Count; i++)
			{
				List<MultiLanguageField> list2 = new List<MultiLanguageField>();
				string mMultiLanguageValue = src[i].MMultiLanguageValue;
				MultiLanguageFieldList item = new MultiLanguageFieldList
				{
					MFieldName = src[i].MFieldName,
					MMultiLanguageField = list2,
					MMultiLanguageValue = mMultiLanguageValue,
					MParentID = parentID
				};
				for (int j = 0; j < src[i].MMultiLanguageField.Count; j++)
				{
					list2.Add(new MultiLanguageField
					{
						MLocaleID = src[i].MMultiLanguageField[j].MLocaleID,
						MValue = src[i].MMultiLanguageField[j].MValue,
						MOrgID = src[i].MMultiLanguageField[j].MOrgID
					});
				}
				list.Add(item);
			}
			return list;
		}

		private int GetMaxChangeIndex(MContext ctx, string itemID)
		{
			string sql = "select (ifnull(max(CAST(MIndex AS SIGNED)),0))  as MaxIndex from t_fa_fixassetschange where MOrgID = @MOrgID and MIsDelete = 0 and MID = @MItemID ";
			MySqlParameter[] parameters = ctx.GetParameters("@MItemID", itemID);
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, parameters);
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return 0;
			}
			return int.Parse(dataSet.Tables[0].Rows[0].MField<long>("MaxIndex").ToString());
		}

		private void ValidateFixAssets(MContext ctx, FAFixAssetsModel model)
		{
			ValidateQueryModel validateFixAssetsNumberSql = utility.GetValidateFixAssetsNumberSql(ctx, model);
			ValidateQueryModel item = new ValidateQueryModel();
			if (!model.MIsChange)
			{
				DateTime t = model.MDepreciationFromPeriod;
				if (t < ctx.MFABeginDate)
				{
					t = ctx.MFABeginDate;
				}
				item = utility.GetValidateCreatedDepreciationVoucher(t.Year, t.Month);
			}
			List<ValidateQueryModel> second = (model.MCheckGroupValueModel != null) ? utility.GetValidateCheckGroupValueModel(new List<GLCheckGroupValueModel>
			{
				model.MCheckGroupValueModel
			}) : new List<ValidateQueryModel>();
			utility.QueryValidateSql(ctx, true, new List<ValidateQueryModel>
			{
				validateFixAssetsNumberSql,
				item
			}.Concat(second).ToArray());
		}

		public OperationResult HandleFixAssets(MContext ctx, List<string> itemIDs, int type)
		{
			ValidateFixAssetsStatus(ctx, itemIDs, type);
			List<FAFixAssetsModel> modelList = GetModelList(ctx, new SqlWhere().In("MItemID", itemIDs), false);
			List<CommandInfo> list = new List<CommandInfo>();
			for (int i = 0; i < modelList.Count; i++)
			{
				FAFixAssetsModel fAFixAssetsModel = modelList[i];
				FAFixAssetsModel fAFixAssetsModel2 = CopyFromModel(ctx, fAFixAssetsModel);
				fAFixAssetsModel2.MIsChange = true;
				fAFixAssetsModel2.MStatus = type;
				fAFixAssetsModel2.MItemID = fAFixAssetsModel.MItemID;
				fAFixAssetsModel2.IsNew = false;
				fAFixAssetsModel2.MHandledDate = ((type >= 1) ? ctx.DateNow : DateTime.MinValue);
				FAFixAssetsModel fAFixAssetsModel3 = fAFixAssetsModel2;
				DateTime mHandledDate = fAFixAssetsModel2.MHandledDate;
				int year = mHandledDate.Year;
				mHandledDate = fAFixAssetsModel2.MHandledDate;
				fAFixAssetsModel3.MHandledDate = new DateTime(year, mHandledDate.Month, 1);
				fAFixAssetsModel2.MultiLanguage = null;
				fAFixAssetsModel.MHandledDate = fAFixAssetsModel2.MHandledDate;
				list.AddRange(GetSaveFixAssetsCmds(ctx, fAFixAssetsModel2, fAFixAssetsModel));
			}
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) >= 1)
			};
		}

		public OperationResult SetExpenseAccountDefault(MContext ctx, bool check, string accountCode)
		{
			string commandText = "update t_fa_fixAssetsType set MExpAccountCode = @MExpAccountCode where MOrgID = @MOrgID and MIsDelete = 0 ";
			OperationResult operationResult = new OperationResult();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = ctx.GetParameters("@MExpAccountCode", check ? accountCode : "");
			list.Add(obj);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) >= 1);
			return operationResult;
		}

		private void ValidateFixAssetsStatus(MContext ctx, List<string> itemIDs, int status)
		{
			ValidateQueryModel validateFixAssetsStatusSql = utility.GetValidateFixAssetsStatusSql(ctx, itemIDs, status);
			ValidateQueryModel validateQueryModel = (status == 0) ? utility.GetValidateCreatedDepreciatedVoucherInHandledPeriod(itemIDs) : new ValidateQueryModel();
			utility.QueryValidateSql(ctx, true, validateFixAssetsStatusSql, validateQueryModel);
		}

		public static IEnumerable<CommandInfo> UpdateAccountCode<T>(MContext ctx, string oldCode, string newCode) where T : BaseModel
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@oldCode", MySqlDbType.VarChar, 36)
				{
					Value = oldCode
				},
				new MySqlParameter("@newCode", MySqlDbType.VarChar, 36)
				{
					Value = newCode
				}
			};
			string tableName = (Activator.CreateInstance(typeof(T)) as T).TableName;
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update " + tableName + " set MFixAccountCode=@newCode where MFixAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete=0 ";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			CommandInfo commandInfo2 = new CommandInfo();
			commandInfo2.CommandText = "update " + tableName + " set MDepAccountCode=@newCode where MDepAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete=0 ";
			array = (commandInfo2.Parameters = parameters);
			list.Add(commandInfo2);
			CommandInfo commandInfo3 = new CommandInfo();
			commandInfo3.CommandText = "update " + tableName + " set MExpAccountCode=@newCode where MExpAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete=0";
			array = (commandInfo3.Parameters = parameters);
			list.Add(commandInfo3);
			return list;
		}

		public FAFixAssetsChangeModel GetFixAssetsModel(MContext ctx, string mid)
		{
			string sql = " SELECT t1.*, F_GETUSERNAME (t2.MFristName, t2.MLastName) AS MUserName FROM t_fa_fixassets t1\r\n                            LEFT JOIN t_sec_user_l t2 ON t1.MCreatorID = t2.MParentID\r\n                                AND t2.MLocaleID = @MLCID AND t2.MIsDelete = 0\r\n                            WHERE\r\n                            t1.MOrgID = @MOrgID AND t1.MItemId = @MID AND t1.MIsDelete = 0 ";
			List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
			list.AddRange(new List<MySqlParameter>
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
				{
					Value = mid
				}
			});
			return ModelInfoManager.GetDataModel<FAFixAssetsChangeModel>(ctx, sql, list.ToArray());
		}

		public List<DateTime> GetFAPeriodFromBeginDate(MContext ctx, bool isFixedAssetsChange = false)
		{
			List<DateTime> list = new List<DateTime>();
			DateTime dateTime = DateTime.Now;
			int year = dateTime.Year;
			dateTime = DateTime.Now;
			DateTime dateTime2 = new DateTime(year, dateTime.Month, 1);
			string sql = " SELECT MPurchaseDate FROM t_fa_fixassets\r\n                    WHERE\r\n                    MOrgID = @MOrgID AND MIsDelete = 0  ORDER BY MPurchaseDate";
			List<MySqlParameter> list2 = ctx.GetParameters((MySqlParameter)null).ToList();
			List<FAFixAssetsModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<FAFixAssetsModel>(ctx, sql, list2.ToArray());
			if (dataModelBySql != null && dataModelBySql.Count > 0)
			{
				DateTime mPurchaseDate = dataModelBySql.FirstOrDefault().MPurchaseDate;
				DateTime t = (from m in dataModelBySql
				orderby m.MPurchaseDate descending
				select m).FirstOrDefault().MPurchaseDate;
				if (dateTime2 > t)
				{
					t = dateTime2;
				}
				int num = t.Year * 12 + t.Month - (mPurchaseDate.Year * 12 + mPurchaseDate.Month);
				DateTime item = mPurchaseDate;
				list.Add(new DateTime(item.Year, item.Month, 1));
				for (int i = 0; i < num; i++)
				{
					item = item.AddMonths(1);
					list.Add(item);
				}
			}
			if (list.Any())
			{
				dateTime = list.LastOrDefault();
				DateTime date = dateTime.Date;
				if (!isFixedAssetsChange)
				{
					string sql2 = " SELECT MYear,MPeriod,MYearPeriod FROM t_fa_depreciation\r\n                        WHERE MOrgID = @MOrgID  AND MIsDelete = 0  ORDER BY MYearPeriod";
					List<FADepreciationModel> dataModelBySql2 = ModelInfoManager.GetDataModelBySql<FADepreciationModel>(ctx, sql2, list2.ToArray());
					if (dataModelBySql2 != null && dataModelBySql2.Count > 0)
					{
						string str = (from m in dataModelBySql2
						orderby m.MYearPeriod descending
						select m).FirstOrDefault().MYearPeriod.ToString();
						DateTime t2 = DateTime.ParseExact(str + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
						if (t2 > date)
						{
							int num2 = t2.Year * 12 + t2.Month - (date.Year * 12 + date.Month);
							DateTime item2 = date;
							for (int j = 0; j < num2; j++)
							{
								item2 = item2.AddMonths(1);
								list.Add(item2);
							}
						}
					}
				}
			}
			if (!list.Any())
			{
				list.Add(dateTime2);
			}
			return list;
		}

		public List<MultiLanguageFieldList> GetMultiLanguageList(MContext ctx, List<string> assetIdList)
		{
			List<MySqlParameter> list = new List<MySqlParameter>();
			List<MultiLanguageFieldList> list2 = new List<MultiLanguageFieldList>();
			string str = "SELECT * from t_fa_fixassets_l where MOrgID=@MorgID and MIsDelete=0 ";
			if (assetIdList != null)
			{
				str = str + " and MParentID in " + BDRepository.GetInSql(ctx, assetIdList, out list);
			}
			str += " order by MParentID ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			list.Add(new MySqlParameter
			{
				ParameterName = "@MOrgID",
				Value = ctx.MOrgID
			});
			DataSet dataSet = dynamicDbHelperMySQL.Query(str, list.ToArray());
			if (dataSet == null || dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
			{
				return list2;
			}
			DataTable dataTable = dataSet.Tables[0];
			List<MultiLanguageField> list3 = new List<MultiLanguageField>();
			DataRowCollection rows = dataSet.Tables[0].Rows;
			string b = null;
			foreach (DataRow row in dataSet.Tables[0].Rows)
			{
				string parentId = Convert.ToString(row["MParentID"]);
				if (parentId != b)
				{
					MultiLanguageFieldList multiLanguageFieldList = new MultiLanguageFieldList();
					multiLanguageFieldList.MParentID = row.Field<string>("MParentID");
					multiLanguageFieldList.MMultiLanguageField = new List<MultiLanguageField>();
					multiLanguageFieldList.MFieldName = "MName";
					list2.Add(multiLanguageFieldList);
					b = parentId;
				}
				MultiLanguageField multiLanguageField = new MultiLanguageField();
				multiLanguageField.MPKID = row.Field<string>("MPKID");
				multiLanguageField.MOrgID = row.Field<string>("MOrgID");
				multiLanguageField.MValue = row.Field<string>("MName");
				multiLanguageField.MLocaleID = row.Field<string>("MLocaleID");
				MultiLanguageFieldList multiLanguageFieldList2 = (from x in list2
				where x.MParentID == parentId && x.MFieldName == "MName"
				select x).First();
				multiLanguageFieldList2.MMultiLanguageField.Add(multiLanguageField);
			}
			return list2;
		}
	}
}
