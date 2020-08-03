using JieNor.Megi.Core;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.DataRepository.RPT.Biz
{
	public class RPTAssetChangeRepository : RPTBaseREpository
	{
		private static Dictionary<string, List<string>> DataColumns;

		public static BizReportModel GetAssetChangeData(MContext context, RPTAssetChangeFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel
			{
				Type = Convert.ToInt32(BizReportType.AssetChange)
			};
			SetTitle(context, bizReportModel, filter);
			switch (filter.MReportType)
			{
			case 1:
				FixAssetChangeSituation(context, bizReportModel, filter);
				break;
			case 2:
				FixAssetAddSummary(context, bizReportModel, filter);
				break;
			case 3:
				FixAssetDealSummary(context, bizReportModel, filter);
				break;
			case 4:
				FixAssetChangeSummary(context, bizReportModel, filter);
				break;
			}
			return bizReportModel;
		}

		private static void FixAssetChangeSituation(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			SetChangeSituationDataColumns(context, filter);
			SetRowHead(model, filter, context);
			SetFixAssetChangeDatas(context, model, filter);
		}

		private static void SetFixAssetChangeDatas(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			List<RPTAssetChangeModel> list = new List<RPTAssetChangeModel>();
			DateTime dateTime = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime2 = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			dateTime2 = dateTime2.AddMonths(1);
			DateTime dateTime3 = dateTime2.AddSeconds(-1.0);
			List<RPTAssetChangeModel> fixAssetList = GetFixAssetList(context, filter);
			fixAssetList = (from m in fixAssetList
			orderby m.MFixAssetsNumber descending
			select m).ToList();
			List<RPTAssetChangeModel> fAChangeList = GetFAChangeList(context, filter);
			List<RPTAssetChangeModel> fADepreciationList = GetFADepreciationList(context, filter);
			int num = (dateTime3.Year - dateTime.Year) * 12 + (dateTime3.Month - dateTime.Month) + 1;
			DateTime dateTime4 = dateTime;
			for (int i = 0; i < num; i++)
			{
				int tempStartPeroid = dateTime4.Year * 100 + dateTime4.Month;
				DateTime dateTime5 = dateTime4;
				dateTime2 = dateTime5.AddMonths(1);
				DateTime endTempDate = dateTime2.AddDays(-1.0);
				List<RPTAssetChangeModel> list2 = (from m in fixAssetList
				where m.MPurchaseDate <= endTempDate
				select m).ToList();
				foreach (RPTAssetChangeModel item in list2)
				{
					if (item.MPurchaseDate >= dateTime5 && item.MPurchaseDate <= endTempDate)
					{
						if (item.MChanged != 0)
						{
							GetValueChangeItems(fAChangeList, dateTime5, endTempDate, item.MitemId, item);
						}
						item.MOriginalBeginPeriod = decimal.Zero;
						item.MOriginalAdd = item.MOriginalAmount;
						item.MOriginalTurnOut = decimal.Zero;
						if (item.MStatus != 0)
						{
							item.MOriginalTurnOut = item.MOriginalAmount;
						}
						item.MOriginalEndBlance = item.MOriginalAmount;
						if (context.MFABeginDate > endTempDate)
						{
							item.MDepreciationBeginPeriod = decimal.Zero;
							item.MDepreciationAdd = decimal.Zero;
							item.MDepreciationTurnOut = decimal.Zero;
						}
						else
						{
							item.MDepreciationBeginPeriod = item.MDepreciatedAmount;
							item.MDepreciationAdd = ((item.MDepreciationFromPeriod > dateTime5) ? decimal.Zero : item.MPeriodDepreciatedAmount);
							item.MDepreciationTurnOut = decimal.Zero;
						}
						item.MDepreciationEndBlance = item.MDepreciationBeginPeriod + item.MDepreciationAdd - item.MDepreciationTurnOut;
					}
					else
					{
						if (item.MHandledDate >= dateTime5 && item.MHandledDate <= endTempDate)
						{
							GetValueChangeItems(fAChangeList, dateTime5, endTempDate, item.MitemId, item);
							item.MOriginalTurnOut = item.MOriginalAmount;
							item.MOriginalEndBlance = decimal.Zero;
						}
						else if (item.MChanged == 0)
						{
							item.MOriginalBeginPeriod = item.MOriginalAmount;
							item.MOriginalAdd = decimal.Zero;
							item.MOriginalTurnOut = decimal.Zero;
							item.MOriginalEndBlance = item.MOriginalAmount;
						}
						else
						{
							GetValueChangeItems(fAChangeList, dateTime5, endTempDate, item.MitemId, item);
						}
						item.MDepreciationBeginPeriod = item.MDepreciatedAmount;
						GetValueDepreciationItems(context, fADepreciationList, dateTime5, endTempDate, item.MitemId, item);
						if (item.MHandledDate >= dateTime5 && item.MHandledDate <= endTempDate)
						{
							item.MDepreciationAdd = decimal.Zero;
							item.MDepreciationTurnOut = item.MDepreciationBeginPeriod;
							item.MDepreciationEndBlance = decimal.Zero;
						}
					}
					int num2;
					if (item.MStatus != 0)
					{
						dateTime2 = item.MHandledDate;
						dateTime2 = dateTime2.AddMonths(1);
						num2 = ((dateTime2.AddSeconds(-1.0) < dateTime3) ? 1 : 0);
					}
					else
					{
						num2 = 0;
					}
					if (num2 != 0)
					{
						item.MOriginalBeginPeriod = decimal.Zero;
						item.MOriginalEndBlance = decimal.Zero;
						item.MDepreciationBeginPeriod = decimal.Zero;
						item.MDepreciationEndBlance = decimal.Zero;
						item.MPrepareForDecreaseAmount = decimal.Zero;
					}
					int num3;
					if (item.MStatus != 0)
					{
						dateTime2 = item.MHandledDate;
						dateTime2 = dateTime2.AddMonths(1);
						num3 = ((dateTime2.AddSeconds(-1.0) == dateTime3) ? 1 : 0);
					}
					else
					{
						num3 = 0;
					}
					if (num3 != 0)
					{
						item.MPrepareForDecreaseAmount = decimal.Zero;
					}
					SetFixAssetChangeRowData(context, model, filter, item);
					RPTAssetChangeModel rPTAssetChangeModel = new RPTAssetChangeModel();
					rPTAssetChangeModel.MOriginalBeginPeriod = item.MOriginalBeginPeriod;
					rPTAssetChangeModel.MOriginalAdd = item.MOriginalAdd;
					rPTAssetChangeModel.MOriginalTurnOut = item.MOriginalTurnOut;
					rPTAssetChangeModel.MOriginalEndBlance = item.MOriginalEndBlance;
					rPTAssetChangeModel.MDepreciationBeginPeriod = item.MDepreciationBeginPeriod;
					rPTAssetChangeModel.MDepreciationAdd = item.MDepreciationAdd;
					rPTAssetChangeModel.MDepreciationTurnOut = item.MDepreciationTurnOut;
					rPTAssetChangeModel.MDepreciationEndBlance = item.MDepreciationEndBlance;
					rPTAssetChangeModel.MNetAmount = item.MNetAmount;
					rPTAssetChangeModel.MPrepareForDecreaseAmount = item.MPrepareForDecreaseAmount;
					rPTAssetChangeModel.MYearPeriod = tempStartPeroid;
					list.Add(rPTAssetChangeModel);
				}
				List<RPTAssetChangeModel> source = (from m in list
				where m.MYearPeriod == tempStartPeroid
				select m).ToList();
				RPTAssetChangeModel rPTAssetChangeModel2 = new RPTAssetChangeModel();
				rPTAssetChangeModel2.MFATypeIDName = tempStartPeroid + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "PeriodSummary", "本期合计");
				rPTAssetChangeModel2.MFixAssetsNumber = "";
				rPTAssetChangeModel2.MFixAssetsName = "";
				rPTAssetChangeModel2.MOriginalBeginPeriod = source.Sum((RPTAssetChangeModel m) => m.MOriginalBeginPeriod);
				rPTAssetChangeModel2.MOriginalAdd = source.Sum((RPTAssetChangeModel m) => m.MOriginalAdd);
				rPTAssetChangeModel2.MOriginalTurnOut = source.Sum((RPTAssetChangeModel m) => m.MOriginalTurnOut);
				rPTAssetChangeModel2.MOriginalEndBlance = source.Sum((RPTAssetChangeModel m) => m.MOriginalEndBlance);
				rPTAssetChangeModel2.MDepreciationBeginPeriod = source.Sum((RPTAssetChangeModel m) => m.MDepreciationBeginPeriod);
				rPTAssetChangeModel2.MDepreciationAdd = source.Sum((RPTAssetChangeModel m) => m.MDepreciationAdd);
				rPTAssetChangeModel2.MDepreciationTurnOut = source.Sum((RPTAssetChangeModel m) => m.MDepreciationTurnOut);
				rPTAssetChangeModel2.MDepreciationEndBlance = source.Sum((RPTAssetChangeModel m) => m.MDepreciationEndBlance);
				rPTAssetChangeModel2.MNetAmount = source.Sum((RPTAssetChangeModel m) => m.MNetAmount);
				rPTAssetChangeModel2.MPrepareForDecreaseAmount = source.Sum((RPTAssetChangeModel m) => m.MPrepareForDecreaseAmount);
				SetFixAssetChangeRowData(context, model, filter, rPTAssetChangeModel2);
				dateTime4 = dateTime4.AddMonths(1);
			}
		}

		private static List<RPTAssetChangeModel> GetFixAssetList(MContext context, RPTAssetChangeFilterModel filter)
		{
			string fixAssetSql = GetFixAssetSql(filter);
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MPurchaseDate", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			array[2].Value = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture).AddMonths(1);
			return ModelInfoManager.GetDataModelBySql<RPTAssetChangeModel>(context, fixAssetSql, array);
		}

		private static string GetFixAssetSql(RPTAssetChangeFilterModel filter)
		{
			string str = " SELECT t1.*,t3.MName AS MFixAssetsName,t4.MName AS MFATypeIDName ";
			if (filter.IncludeCheckType)
			{
				str += " ,convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactNameExp,\r\n                                    F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeNameExp,\r\n                                    concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemNameExp,\r\n                                    t9.MName AS MExpItemNameExp,t10.MName AS MPaItemNameExp,\r\n                                    t10_0.MGroupID AS MPaItemGroupIDExp,t10_1.MName AS MPaItemGroupNameExp, t11.MName AS MTrackItem1NameExp,\r\n                                    t11_2.MName AS MTrackItem1GroupNameExp,t12.MName AS MTrackItem2NameExp,t12_2.MName AS MTrackItem2GroupNameExp,\r\n                                    t13.MName AS MTrackItem3NameExp,t13_2.MName AS MTrackItem3GroupNameExp,t14.MName AS MTrackItem4NameExp,\r\n                                    t14_2.MName AS MTrackItem4GroupNameExp,t15.MName AS MTrackItem5NameExp,t15_2.MName AS MTrackItem5GroupNameExp,\r\n\r\n                                    convert(AES_DECRYPT(t16.MName,'{0}') using utf8) as MContactNameDep,\r\n                                    F_GETUSERNAME(t17.MFirstName, t17.MLastName) AS MEmployeeNameDep,\r\n                                    concat(t18_0.MNumber,':',t18.MDesc) AS MMerItemNameDep,\r\n                                    t19.MName AS MExpItemNameDep,t20.MName AS MPaItemNameDep,\r\n                                    t20_0.MGroupID AS MPaItemGroupIDDep,t20_1.MName as MPaItemGroupNameDep,\r\n                                    t21.MName AS MTrackItem1NameDep,t21_2.MName AS MTrackItem1GroupNameDep,t22.MName AS MTrackItem2NameDep,\r\n                                    t22_2.MName AS MTrackItem2GroupNameDep,t23.MName AS MTrackItem3NameDep,t23_2.MName AS MTrackItem3GroupNameDep,\r\n                                    t24.MName AS MTrackItem4NameDep,t24_2.MName AS MTrackItem4GroupNameDep,t25.MName AS MTrackItem5NameDep,\r\n                                    t25_2.MName AS MTrackItem5GroupNameDep ";
			}
			str += " FROM (SELECT * FROM  t_fa_fixassets WHERE MOrgID = @MOrgID AND MIsDelete = 0 AND MPurchaseDate < @MPurchaseDate  ) t1 \r\n                            LEFT JOIN t_fa_fixassets_l t3 ON t3.MParentID = t1.MitemId\r\n                            AND t3.MOrgID = t1.MOrgID AND t3.MLocaleID = @MLCID AND t3.MIsDelete = 0\r\n                            LEFT JOIN t_fa_fixassetstype_l t4 ON t4.MParentID = t1.MFATypeID AND t4.MLocaleID = @MLCID AND t4.MIsDelete = 0 ";
			if (filter.IncludeCheckType)
			{
				str += " LEFT JOIN t_gl_checkgroupvalue t5 ON t5.MItemID = t1.MExpCheckGroupValueID\r\n                                        AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t6 ON t6.MParentID = t5.MContactID \r\n                                        AND t6.MLocaleID = @MLCID AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_employees_l t7 ON t7.MParentID = t5.MEmployeeID \r\n                                        AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                                        AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                                     LEFT JOIN t_bd_item_l t8 ON t8.MParentID = t5.MMerItemID \r\n                                        AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_expenseitem_l t9 ON t9.MParentID = t5.MExpItemID \r\n                                        AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem_l t10 ON t10.MParentID = t5.MPaItemID \r\n                                        AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem t10_0 ON t10_0.MItemID = t5.MPaItemID \r\n                                        AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                                     LEFT JOIN t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t5.MPaItemID \r\n                                        AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_trackentry_l t11 ON t11.MParentID = t5.MTrackItem1 \r\n                                        AND t11.MLocaleID = @MLCID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t11_1 ON t11_1.MEntryID = t5.MTrackItem1 \r\n                                        AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t11_2 ON t11_2.MParentID = t11_1.MItemID \r\n                                        AND t11_2.MLocaleID = @MLCID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t12 ON t12.MParentID = t5.MTrackItem2 \r\n                                        AND t12.MLocaleID = @MLCID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t12_1 ON t12_1.MEntryID = t5.MTrackItem2 \r\n                                        AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t12_2 ON t12_2.MParentID = t12_1.MItemID \r\n                                        AND t12_2.MLocaleID = @MLCID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t13 ON t13.MParentID = t5.MTrackItem3 \r\n                                        AND t13.MLocaleID = @MLCID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t13_1 ON t13_1.MEntryID = t5.MTrackItem3 \r\n                                        AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t13_2 ON t13_2.MParentID = t13_1.MItemID \r\n                                        AND t13_2.MLocaleID = @MLCID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t14 ON t14.MParentID = t5.MTrackItem4\r\n                                        AND t14.MLocaleID = @MLCID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t14_1 ON t14_1.MEntryID = t5.MTrackItem4 \r\n                                        AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t14_2 ON t14_2.MParentID = t14_1.MItemID\r\n                                        AND t14_2.MLocaleID = @MLCID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t15 ON t15.MParentID = t5.MTrackItem5\r\n                                        AND t15.MLocaleID = @MLCID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t15_1 ON t15_1.MEntryID = t5.MTrackItem5 \r\n                                        AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t15_2 ON t15_2.MParentID = t15_1.MItemID \r\n                                        AND t15_2.MLocaleID = @MLCID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0\r\n\r\n                                     LEFT JOIN t_gl_checkgroupvalue t26 ON t26.MItemID = t1.MDepCheckGroupValueID\r\n                                        AND t26.MOrgID = t1.MOrgID AND t26.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t16 ON t16.MParentID = t26.MContactID \r\n                                        AND t16.MLocaleID = @MLCID AND t16.MOrgID = t1.MOrgID AND t16.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_employees_l t17 ON t17.MParentID = t26.MEmployeeID \r\n                                        AND t17.MOrgID = t1.MOrgId AND t17.MIsDelete = t1.MIsDelete AND t17.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_item t18_0 ON t18_0.MItemID = t26.MMerItemID\r\n                                        AND t18_0.MOrgID = t1.MOrgId AND t18_0.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_item_l t18 ON t18.MParentID = t26.MMerItemID \r\n                                        AND t18.MOrgID = t1.MOrgId AND t18.MIsDelete = t1.MIsDelete AND t18.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_expenseitem_l t19 ON t19.MParentID = t26.MExpItemID \r\n                                        AND t19.MOrgID = t1.MOrgId AND t19.MIsDelete = t1.MIsDelete AND t19.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem_l t20 ON t20.MParentID = t26.MPaItemID \r\n                                        AND t20.MOrgID = t1.MOrgId AND t20.MIsDelete = t1.MIsDelete AND t20.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem t20_0 ON t20_0.MItemID = t26.MPaItemID \r\n                                        AND t20_0.MOrgID = t1.MOrgId AND t20_0.MIsDelete = 0               \r\n                                    LEFT JOIN t_pa_payitemgroup_l t20_1 ON t20_1.MParentID = t26.MPaItemID\r\n                                        AND t20_1.MOrgID = t1.MOrgId AND t20_1.MIsDelete = 0 AND t20_1.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_trackentry_l t21 ON t21.MParentID = t26.MTrackItem1 \r\n                                        AND t21.MLocaleID = @MLCID AND t21.MOrgID = t1.MOrgID AND t21.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t21_1 ON t21_1.MEntryID = t26.MTrackItem1 \r\n                                        AND t21_1.MOrgID = t1.MOrgID AND t21_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t21_2 ON t21_2.MParentID = t21_1.MItemID \r\n                                        AND t21_2.MLocaleID = @MLCID AND t21_2.MOrgID = t1.MOrgID AND t21_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t22 ON t22.MParentID = t26.MTrackItem2 \r\n                                        AND t22.MLocaleID = @MLCID AND t22.MOrgID = t1.MOrgID AND t22.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t22_1 ON t22_1.MEntryID = t26.MTrackItem2 \r\n                                        AND t22_1.MOrgID = t1.MOrgID AND t22_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t22_2 ON t22_2.MParentID = t22_1.MItemID \r\n                                        AND t22_2.MLocaleID = @MLCID AND t22_2.MOrgID = t1.MOrgID AND t22_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t23 ON t23.MParentID = t26.MTrackItem3 \r\n                                        AND t23.MLocaleID = @MLCID AND t23.MOrgID = t1.MOrgID AND t23.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t23_1 ON t23_1.MEntryID = t26.MTrackItem3 \r\n                                        AND t23_1.MOrgID = t1.MOrgID AND t23_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t23_2 ON t23_2.MParentID = t23_1.MItemID \r\n                                        AND t23_2.MLocaleID = @MLCID AND t23_2.MOrgID = t1.MOrgID AND t23_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t24 ON t24.MParentID = t26.MTrackItem4\r\n                                        AND t24.MLocaleID = @MLCID AND t24.MOrgID = t1.MOrgID AND t24.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t24_1 ON t24_1.MEntryID = t26.MTrackItem4 \r\n                                        AND t24_1.MOrgID = t1.MOrgID AND t24_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t24_2 ON t24_2.MParentID = t24_1.MItemID \r\n                                        AND t24_2.MLocaleID = @MLCID AND t24_2.MOrgID = t1.MOrgID AND t24_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t25 ON t25.MParentID = t26.MTrackItem5 \r\n                                        AND t25.MLocaleID = @MLCID AND t25.MOrgID = t1.MOrgID AND t25.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t25_1 ON t25_1.MEntryID = t26.MTrackItem5 \r\n                                        AND t25_1.MOrgID = t1.MOrgID AND t25_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t25_2 ON t25_2.MParentID = t25_1.MItemID \r\n                                        AND t25_2.MLocaleID = @MLCID AND t25_2.MOrgID = t1.MOrgID AND t25_2.MIsDelete = 0 ";
			}
			return string.Format(str, "JieNor-001");
		}

		private static List<RPTAssetChangeModel> GetFAChangeList(MContext context, RPTAssetChangeFilterModel filter)
		{
			string fAChangeSql = GetFAChangeSql(filter);
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			return ModelInfoManager.GetDataModelBySql<RPTAssetChangeModel>(context, fAChangeSql, array);
		}

		private static string GetFAChangeSql(RPTAssetChangeFilterModel filter)
		{
			return " SELECT * FROM  t_fa_fixassetschange WHERE MOrgID = @MOrgID AND MIsDelete = 0 ";
		}

		private static List<RPTAssetChangeModel> GetFADepreciationList(MContext context, RPTAssetChangeFilterModel filter)
		{
			string fADepreciationSql = GetFADepreciationSql(filter);
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			return ModelInfoManager.GetDataModelBySql<RPTAssetChangeModel>(context, fADepreciationSql, array);
		}

		private static string GetFADepreciationSql(RPTAssetChangeFilterModel filter)
		{
			return " SELECT * FROM  t_fa_depreciation WHERE MOrgID = @MOrgID AND MIsDelete = 0  ";
		}

		private static void SetFixAssetChangeRowData(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter, RPTAssetChangeModel item)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Item;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MFATypeIDName),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MFixAssetsNumber),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MFixAssetsName),
				CellType = BizReportCellType.Text
			});
			if (filter.IncludeCheckType)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(RPTBaseREpository.GetCheckGroupValue(context, item)),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MOriginalBeginPeriod),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MOriginalAdd),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MOriginalTurnOut),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MOriginalEndBlance),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MDepreciationBeginPeriod),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MDepreciationAdd),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MDepreciationTurnOut),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MDepreciationEndBlance),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MOriginalEndBlance - item.MDepreciationEndBlance),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MPrepareForDecreaseAmount),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(item.MOriginalEndBlance - item.MDepreciationEndBlance - item.MPrepareForDecreaseAmount),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static void GetValueChangeItems(List<RPTAssetChangeModel> list, DateTime startDate, DateTime endDate, string mId, RPTAssetChangeModel model)
		{
			List<RPTAssetChangeModel> list2 = (from m in list
			where m.MId == mId
			orderby m.MIndex descending
			select m).ToList();
			if (list2.Count > 0)
			{
				List<RPTAssetChangeModel> list3 = (from m in list2
				where m.MChangeFromPeriod >= startDate && m.MChangeFromPeriod <= endDate
				select m).ToList();
				if (list3.Count > 0)
				{
					RPTAssetChangeModel rPTAssetChangeModel = list3.FirstOrDefault();
					int num = list2.LastIndexOf(rPTAssetChangeModel);
					int num2 = num + 1;
					RPTAssetChangeModel rPTAssetChangeModel2 = (num2 <= list2.Count() - 1) ? list2[num2] : list2[list2.Count() - 1];
					if (rPTAssetChangeModel.MOriginalAmount != rPTAssetChangeModel2.MOriginalAmount)
					{
						model.MOriginalBeginPeriod = rPTAssetChangeModel2.MOriginalAmount;
						model.MOriginalAmount = rPTAssetChangeModel.MOriginalAmount;
						model.MOriginalAmountChange = rPTAssetChangeModel.MOriginalAmount - rPTAssetChangeModel2.MOriginalAmount;
						model.MOriginalAdd = ((model.MOriginalAmountChange > decimal.Zero) ? model.MOriginalAmountChange : decimal.Zero);
						model.MOriginalTurnOut = ((model.MOriginalAmountChange < decimal.Zero) ? model.MOriginalAmountChange : decimal.Zero);
					}
					else
					{
						model.MOriginalBeginPeriod = rPTAssetChangeModel2.MOriginalAmount;
						model.MOriginalAdd = decimal.Zero;
						model.MOriginalTurnOut = decimal.Zero;
						model.MOriginalAmount = rPTAssetChangeModel2.MOriginalAmount;
					}
					model.MOriginalEndBlance = model.MOriginalBeginPeriod + model.MOriginalAdd - model.MOriginalTurnOut;
					if (rPTAssetChangeModel.MPrepareForDecreaseAmount != rPTAssetChangeModel2.MPrepareForDecreaseAmount)
					{
						model.MPrepareForDecreaseAmount = rPTAssetChangeModel.MPrepareForDecreaseAmount;
					}
				}
				else
				{
					List<RPTAssetChangeModel> list4 = list2.Where(delegate(RPTAssetChangeModel m)
					{
						int result2;
						if (m.MChangeFromPeriod < startDate)
						{
							DateTime dateTime2 = m.MChangeFromPeriod;
							string a2 = dateTime2.ToString("yyyy-MM-dd");
							dateTime2 = DateTime.MinValue;
							result2 = ((a2 != dateTime2.ToString("yyyy-MM-dd")) ? 1 : 0);
						}
						else
						{
							result2 = 0;
						}
						return (byte)result2 != 0;
					}).ToList();
					if (list4.Count > 0)
					{
						RPTAssetChangeModel rPTAssetChangeModel3 = list4.FirstOrDefault();
						model.MOriginalBeginPeriod = rPTAssetChangeModel3.MOriginalAmount;
						model.MOriginalAdd = decimal.Zero;
						model.MOriginalTurnOut = decimal.Zero;
						model.MOriginalEndBlance = rPTAssetChangeModel3.MOriginalAmount;
						model.MOriginalAmount = rPTAssetChangeModel3.MOriginalAmount;
						model.MPrepareForDecreaseAmount = rPTAssetChangeModel3.MPrepareForDecreaseAmount;
					}
					else
					{
						List<RPTAssetChangeModel> list5 = list2.Where(delegate(RPTAssetChangeModel m)
						{
							int result;
							if (m.MChangeFromPeriod > endDate)
							{
								DateTime dateTime = m.MChangeFromPeriod;
								string a = dateTime.ToString("yyyy-MM-dd");
								dateTime = DateTime.MinValue;
								result = ((a != dateTime.ToString("yyyy-MM-dd")) ? 1 : 0);
							}
							else
							{
								result = 0;
							}
							return (byte)result != 0;
						}).ToList();
						if (list5.Count > 0)
						{
							RPTAssetChangeModel rPTAssetChangeModel4 = list5.FirstOrDefault();
							int changeIndex = rPTAssetChangeModel4.MIndex;
							RPTAssetChangeModel rPTAssetChangeModel5 = list2.FirstOrDefault((RPTAssetChangeModel m) => m.MIndex == changeIndex - 1);
							model.MOriginalBeginPeriod = rPTAssetChangeModel5.MOriginalAmount;
							model.MOriginalAdd = decimal.Zero;
							model.MOriginalTurnOut = decimal.Zero;
							model.MOriginalEndBlance = rPTAssetChangeModel5.MOriginalAmount;
							model.MOriginalAmount = rPTAssetChangeModel5.MOriginalAmount;
							model.MPrepareForDecreaseAmount = rPTAssetChangeModel5.MPrepareForDecreaseAmount;
						}
						else
						{
							model.MOriginalBeginPeriod = model.MOriginalAmount;
							model.MOriginalAdd = decimal.Zero;
							model.MOriginalTurnOut = decimal.Zero;
							model.MOriginalEndBlance = model.MOriginalAmount;
						}
					}
				}
			}
			else
			{
				model.MOriginalBeginPeriod = model.MOriginalAmount;
				model.MOriginalAdd = decimal.Zero;
				model.MOriginalTurnOut = decimal.Zero;
				model.MOriginalEndBlance = model.MOriginalAmount;
			}
		}

		private static void GetValueDepreciationItems(MContext context, List<RPTAssetChangeModel> list, DateTime startDate, DateTime endDate, string mId, RPTAssetChangeModel model)
		{
			if (context.MFABeginDate > endDate)
			{
				model.MDepreciationBeginPeriod = decimal.Zero;
				model.MDepreciationAdd = decimal.Zero;
				model.MDepreciationTurnOut = decimal.Zero;
			}
			else
			{
				List<RPTAssetChangeModel> list2 = (from m in list
				where m.MId == mId && m.MYearPeriod <= endDate.Year * 100 + endDate.Month && m.MYearPeriod >= startDate.Year * 100 + startDate.Month && !string.IsNullOrWhiteSpace(m.MVoucherID)
				select m).ToList();
				List<RPTAssetChangeModel> list3 = (from m in list
				where m.MId == mId && m.MYearPeriod < startDate.Year * 100 + startDate.Month && !string.IsNullOrWhiteSpace(m.MVoucherID)
				orderby m.MYearPeriod descending
				select m).ToList();
				if (list2.Count > 0)
				{
					if (list3.Count > 0)
					{
						RPTAssetChangeModel firstPreDepreciation = list3.FirstOrDefault();
						model.MDepreciationBeginPeriod = (from m in list3
						where m.MYearPeriod == firstPreDepreciation.MYearPeriod
						select m).Sum((RPTAssetChangeModel m) => m.MDepreciatedAmount);
					}
					else
					{
						model.MDepreciationBeginPeriod = list2.Sum((RPTAssetChangeModel m) => m.MDepreciatedAmount) - list2.Sum((RPTAssetChangeModel m) => m.MPeriodDepreciatedAmount);
					}
					model.MDepreciationAdd = list2.Sum((RPTAssetChangeModel m) => m.MPeriodDepreciatedAmount);
					model.MDepreciationTurnOut = decimal.Zero;
				}
				else if (list3.Count > 0)
				{
					RPTAssetChangeModel firstPreDepreciation2 = list3.FirstOrDefault();
					model.MDepreciationBeginPeriod = (from m in list3
					where m.MYearPeriod == firstPreDepreciation2.MYearPeriod
					select m).Sum((RPTAssetChangeModel m) => m.MDepreciatedAmount);
					model.MDepreciationAdd = decimal.Zero;
					model.MDepreciationTurnOut = decimal.Zero;
				}
			}
			model.MDepreciationEndBlance = model.MDepreciationBeginPeriod + model.MDepreciationAdd - model.MDepreciationTurnOut;
		}

		private static void SetFixAssetChangeData(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			List<RPTAssetChangeModel> list = new List<RPTAssetChangeModel>();
			List<RPTAssetChangeModel> fixAssetChangeList = GetFixAssetChangeList(context, model, filter);
			DateTime dateTime = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime2 = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			dateTime2 = dateTime2.AddMonths(1);
			DateTime dateTime3 = dateTime2.AddSeconds(-1.0);
			int num = (dateTime3.Year - dateTime.Year) * 12 + (dateTime3.Month - dateTime.Month) + 1;
			List<RPTAssetChangeModel> source = (from m in fixAssetChangeList
			where m.MChanged == 0
			select m).ToList();
			List<RPTAssetChangeModel> source2 = (from m in fixAssetChangeList
			where m.MChanged != 0
			select m).ToList();
			DateTime tempDate = dateTime;
			for (int i = 0; i < num; i++)
			{
				tempDate = tempDate.AddMonths(i);
				int tempStartPeroid = tempDate.Year * 100 + tempDate.Month;
				List<RPTAssetChangeModel> list2 = (from m in source
				where m.MCreateDate >= tempDate && m.MCreateDate <= tempDate.AddMonths(1).AddDays(-1.0)
				select m).ToList();
				foreach (RPTAssetChangeModel item2 in list2)
				{
					item2.MOriginalBeginPeriod = decimal.Zero;
					item2.MOriginalAdd = item2.MOriginalAmount;
					item2.MOriginalTurnOut = decimal.Zero;
					if (item2.MStatus != 0)
					{
						item2.MOriginalTurnOut = item2.MOriginalAmount;
					}
					item2.MOriginalEndBlance = item2.MOriginalAmount;
					item2.MDepreciationBeginPeriod = item2.MDepreciatedAmount;
					item2.MDepreciationAdd = item2.MPeriodDepreciatedAmount * (decimal)num;
					item2.MDepreciationTurnOut = decimal.Zero;
					item2.MDepreciationEndBlance = item2.MDepreciationBeginPeriod + item2.MDepreciationAdd - item2.MDepreciationTurnOut;
					SetFixAssetChangeRowData(context, model, filter, item2);
					item2.MYearPeriod = tempStartPeroid;
					list.Add(item2);
				}
				List<RPTAssetChangeModel> source3 = (from m in source2
				where m.MYear * 100 + m.MPeriod == tempStartPeroid
				select m).ToList();
				List<RPTAssetChangeModel> list3 = (from p in source3
				group p by new
				{
					p.MFATypeID
				} into g
				select new RPTAssetChangeModel
				{
					MFATypeID = g.Key.MFATypeID
				}).ToList();
				foreach (RPTAssetChangeModel item3 in list3)
				{
					List<RPTAssetChangeModel> source4 = (from m in source3
					where m.MFATypeID == item3.MFATypeID
					orderby m.MIndex descending
					select m).ToList();
					RPTAssetChangeModel rPTAssetChangeModel = source4.FirstOrDefault();
					RPTAssetChangeModel rPTAssetChangeModel2 = (from m in source4
					orderby m.MIndex
					select m).FirstOrDefault();
					rPTAssetChangeModel.MOriginalBeginPeriod = rPTAssetChangeModel2.MOriginalAmountChange;
					decimal num2 = rPTAssetChangeModel.MOriginalAmountChange - rPTAssetChangeModel2.MOriginalAmountChange;
					rPTAssetChangeModel.MOriginalAdd = ((num2 > decimal.Zero) ? num2 : decimal.Zero);
					RPTAssetChangeModel rPTAssetChangeModel3 = source4.FirstOrDefault((RPTAssetChangeModel m) => m.MStatus == 1 || m.MStatus == 2);
					rPTAssetChangeModel.MOriginalEndBlance = rPTAssetChangeModel.MOriginalAmountChange;
					rPTAssetChangeModel.MDepreciationBeginPeriod = rPTAssetChangeModel2.MDepreciatedAmountChange;
					rPTAssetChangeModel.MDepreciationAdd = rPTAssetChangeModel.MDepreciatedAmountChange - rPTAssetChangeModel2.MDepreciatedAmountChange;
					if (rPTAssetChangeModel3 != null)
					{
						rPTAssetChangeModel.MOriginalTurnOut = rPTAssetChangeModel.MOriginalBeginPeriod;
						rPTAssetChangeModel.MDepreciationTurnOut = rPTAssetChangeModel.MDepreciatedAmountChange;
						rPTAssetChangeModel.MDepreciationEndBlance = decimal.Zero;
					}
					else
					{
						rPTAssetChangeModel.MOriginalTurnOut = ((num2 < decimal.Zero) ? Math.Abs(num2) : decimal.Zero);
						rPTAssetChangeModel.MDepreciationTurnOut = decimal.Zero;
						rPTAssetChangeModel.MDepreciationEndBlance = rPTAssetChangeModel.MDepreciatedAmountChange;
					}
					SetFixAssetChangeRowData(context, model, filter, rPTAssetChangeModel);
					rPTAssetChangeModel.MYearPeriod = tempStartPeroid;
					list.Add(rPTAssetChangeModel);
				}
				List<RPTAssetChangeModel> source5 = (from m in list
				where m.MYearPeriod == tempStartPeroid
				select m).ToList();
				RPTAssetChangeModel rPTAssetChangeModel4 = new RPTAssetChangeModel();
				rPTAssetChangeModel4.MFATypeIDName = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "PeriodSummary", "本期合计");
				rPTAssetChangeModel4.MFixAssetsNumber = "";
				rPTAssetChangeModel4.MFixAssetsName = "";
				rPTAssetChangeModel4.MOriginalBeginPeriod = source5.Sum((RPTAssetChangeModel m) => m.MOriginalBeginPeriod);
				rPTAssetChangeModel4.MOriginalAdd = source5.Sum((RPTAssetChangeModel m) => m.MOriginalAdd);
				rPTAssetChangeModel4.MOriginalTurnOut = source5.Sum((RPTAssetChangeModel m) => m.MOriginalTurnOut);
				rPTAssetChangeModel4.MOriginalEndBlance = source5.Sum((RPTAssetChangeModel m) => m.MOriginalEndBlance);
				rPTAssetChangeModel4.MDepreciationBeginPeriod = source5.Sum((RPTAssetChangeModel m) => m.MDepreciationBeginPeriod);
				rPTAssetChangeModel4.MDepreciationAdd = source5.Sum((RPTAssetChangeModel m) => m.MDepreciationAdd);
				rPTAssetChangeModel4.MDepreciationTurnOut = source5.Sum((RPTAssetChangeModel m) => m.MDepreciationTurnOut);
				rPTAssetChangeModel4.MDepreciationEndBlance = source5.Sum((RPTAssetChangeModel m) => m.MDepreciationEndBlance);
				rPTAssetChangeModel4.MNetAmount = source5.Sum((RPTAssetChangeModel m) => m.MNetAmount);
				rPTAssetChangeModel4.MPrepareForDecreaseAmount = source5.Sum((RPTAssetChangeModel m) => m.MPrepareForDecreaseAmount);
				SetFixAssetChangeRowData(context, model, filter, rPTAssetChangeModel4);
			}
		}

		private static List<RPTAssetChangeModel> GetFixAssetChangeList(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			string fixAssetChangeSql = GetFixAssetChangeSql(filter);
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			return ModelInfoManager.GetDataModelBySql<RPTAssetChangeModel>(context, fixAssetChangeSql, array);
		}

		private static string GetFixAssetChangeSql(RPTAssetChangeFilterModel filter)
		{
			return " SELECT t1.MItemId,t1.MOrgID,t1.MChanged,t1.MPrefix,t1.MNumber,t1.MPurchaseDate,t1.MFATypeID,t1.MCreateDate,t1.MPeriodDepreciatedAmount,\r\n                               t1.MStatus,t1.MOriginalAmount,t1.MDepreciatedAmount,t1.MHandledDate,t1.MNetAmount,t1.MPrepareForDecreaseAmount,\r\n                               t2.MOriginalAmount AS MOriginalAmountChange,t2.MDepreciatedAmount AS MDepreciatedAmountChange,t2.MNetAmount AS MNetAmountChange,t2.MIndex,\r\n                               t2.MYear,t2.MPeriod,t3.MName AS MFixAssetsName,t4.MName AS MFATypeIDName\r\n                               FROM ( SELECT * FROM t_fa_fixassets WHERE MOrgID = @MOrgID AND MIsDelete = 0 ) t1\r\n                               LEFT JOIN t_fa_fixassetschange t2 ON t2.mid = t1.mitemid\r\n                                AND t2.MIsDelete = 0\r\n                               LEFT JOIN t_fa_fixassets_l t3 ON t3.MParentID = t1.MitemId\r\n                                AND t3.MOrgID = t1.MOrgID AND t3.MLocaleID = @MLCID AND t3.MIsDelete = 0\r\n                               LEFT JOIN t_fa_fixassetstype_l t4 ON t4.MParentID = t1.MFATypeID AND t4.MLocaleID = @MLCID AND t4.MIsDelete = 0 ";
		}

		private static void SetChangeSituationDataColumns(MContext context, RPTAssetChangeFilterModel filter)
		{
			DataColumns = new Dictionary<string, List<string>>();
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Category", "资产类别"), null);
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Encoding", "资产编码"), null);
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FAName", "资产名称"), null);
			if (filter.IncludeCheckType)
			{
				DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AccountDimension", "核算维度"), null);
			}
			string text = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "OriginalAmount", "原值");
			List<string> childrenColumnList = GetChildrenColumnList(context, text, true);
			DataColumns.Add(text, childrenColumnList);
			string text2 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AccumulateDepreciation", "累计折旧");
			DataColumns.Add(text2, GetChildrenColumnList(context, text2, false));
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "NetWorth", "净值"), null);
			string text3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "DepreciationReserves", "减值准备");
			DataColumns.Add(text3, null);
			DataColumns.Add(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "EndBalance", "净额"), null);
		}

		private static List<string> GetChildrenColumnList(MContext ctx, string baseColumnName, bool isBegin = true)
		{
			List<string> list = new List<string>();
			string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "BeginPeriod", "期初");
			string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Add", "新增");
			string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "TurnOut", "转出");
			list.Add(text);
			list.Add(text2);
			list.Add(text3);
			if (isBegin)
			{
				string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "OriginalEndBlance", "期末原值");
				list.Add(text4);
			}
			else
			{
				string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "DepreciationEndBlance", "期末累计折旧");
				list.Add(text5);
			}
			return list;
		}

		private static void SetRowHead(BizReportModel reportModel, RPTAssetChangeFilterModel filter, MContext ctx)
		{
			for (int i = 0; i < 2; i++)
			{
				if (i == 0)
				{
					BizReportRowModel bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Header;
					foreach (KeyValuePair<string, List<string>> dataColumn in DataColumns)
					{
						bool flag = dataColumn.Value != null && dataColumn.Value.Count() > 0;
						BizReportCellModel bizReportCellModel = new BizReportCellModel
						{
							Value = dataColumn.Key,
							CellType = BizReportCellType.Text
						};
						bizReportCellModel.ColumnSpan = ((!flag) ? 1 : dataColumn.Value.Count());
						bizReportCellModel.RowSpan = (flag ? 1 : 2);
						bizReportRowModel.AddCell(bizReportCellModel);
					}
					reportModel.AddRow(bizReportRowModel);
				}
				else
				{
					BizReportRowModel bizReportRowModel2 = new BizReportRowModel();
					bizReportRowModel2.RowType = BizReportRowType.Header;
					foreach (KeyValuePair<string, List<string>> dataColumn2 in DataColumns)
					{
						if (dataColumn2.Value != null && dataColumn2.Value.Count() > 0)
						{
							foreach (string item in dataColumn2.Value)
							{
								BizReportCellModel model = new BizReportCellModel
								{
									Value = item,
									CellType = BizReportCellType.Text
								};
								bizReportRowModel2.AddCell(model);
							}
						}
					}
					reportModel.AddRow(bizReportRowModel2);
				}
			}
		}

		public static void FixAssetAddSummary(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			SetFixAssetAddSummaryRowHead(context, model, filter);
			SetFixAssetAddSummaryRowData(context, model, filter);
		}

		private static void SetFixAssetAddSummaryRowHead(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Category", "资产类别"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Encoding", "资产编码"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FAName", "资产名称"),
				CellType = BizReportCellType.Text
			});
			if (filter.IncludeCheckType)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AccountDimension", "核算维度"),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "PurchaseDate", "采购时间"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "OriginalAmount", "原值"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "PurchaseDepreciation", "购入已折旧"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "PurchaseDepreciationReserves", "购入减值准备"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "NetWorth", "净值"),
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetFixAssetAddSummaryRowData(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			List<RPTAssetChangeModel> list = (from m in GetFixAssetAddSummaryList(context, model, filter)
			orderby m.MFATypeID, m.MNumber
			select m).ToList();
			List<RPTAssetChangeModel> fAChangeList = GetFAChangeList(context, filter);
			string tempFATypeID = string.Empty;
			int num = list.Count();
			if (list.Count >= 1)
			{
				foreach (RPTAssetChangeModel item in list)
				{
					RPTAssetChangeModel rPTAssetChangeModel = (from m in fAChangeList
					orderby m.MIndex
					select m).FirstOrDefault((RPTAssetChangeModel m) => m.MId == item.MitemId);
					if (rPTAssetChangeModel != null)
					{
						item.MOriginalAmount = rPTAssetChangeModel.MOriginalAmount;
						item.MPrepareForDecreaseAmount = rPTAssetChangeModel.MPrepareForDecreaseAmount;
					}
					tempFATypeID = item.MFATypeID;
					BizReportRowModel bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MFATypeIDName),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MFixAssetsNumber),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MFixAssetsName),
						CellType = BizReportCellType.Text
					});
					if (filter.IncludeCheckType)
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(RPTBaseREpository.GetCheckGroupValue(context, item)),
							CellType = BizReportCellType.Text
						});
					}
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MPurchaseDate.ToString("yyyy-MM-dd")),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MOriginalAmount),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MDepreciatedAmount),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MPrepareForDecreaseAmount),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MNetAmount),
						CellType = BizReportCellType.Money
					});
					model.AddRow(bizReportRowModel);
					int num2 = list.IndexOf(item);
					if (num2 == num - 1)
					{
						List<RPTAssetChangeModel> lists = (from m in list
						where m.MFATypeID == tempFATypeID
						select m).ToList();
						GetFixAssetAddSummaryTotal(context, model, filter, BizReportRowType.SubTotal, lists);
					}
					else if (num2 < num - 1 && tempFATypeID != list[num2 + 1].MFATypeID)
					{
						List<RPTAssetChangeModel> lists2 = (from m in list
						where m.MFATypeID == tempFATypeID
						select m).ToList();
						GetFixAssetAddSummaryTotal(context, model, filter, BizReportRowType.SubTotal, lists2);
					}
				}
				GetFixAssetAddSummaryTotal(context, model, filter, BizReportRowType.Total, list);
			}
		}

		private static void GetFixAssetAddSummaryTotal(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter, BizReportRowType rowType, List<RPTAssetChangeModel> lists)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = rowType;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + ((rowType == BizReportRowType.SubTotal) ? lists.FirstOrDefault().MFATypeIDName : "")),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			if (filter.IncludeCheckType)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(""),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(lists.Sum((RPTAssetChangeModel m) => m.MOriginalAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(lists.Sum((RPTAssetChangeModel m) => m.MDepreciatedAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(lists.Sum((RPTAssetChangeModel m) => m.MPrepareForDecreaseAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(lists.Sum((RPTAssetChangeModel m) => m.MNetAmount)),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		private static List<RPTAssetChangeModel> GetFixAssetAddSummaryList(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			string fixAssetAddSummarysSql = GetFixAssetAddSummarysSql(filter);
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartPeroid", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndPeroid", MySqlDbType.VarChar, 36)
			};
			DateTime dateTime = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime2 = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			dateTime2 = dateTime2.AddMonths(1);
			DateTime dateTime3 = dateTime2.AddSeconds(-1.0);
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			array[2].Value = dateTime;
			array[3].Value = dateTime3;
			return ModelInfoManager.GetDataModelBySql<RPTAssetChangeModel>(context, fixAssetAddSummarysSql, array);
		}

		private static string GetFixAssetAddSummarysSql(RPTAssetChangeFilterModel filter)
		{
			string str = " SELECT t1.MItemId,t1.MPurchaseDate,t1.MFATypeID,t1.MPrefix,t1.MNumber,t1.MOriginalAmount,t1.MCreateDate,\r\n                              t1.MDepreciatedAmount,t1.MPrepareForDecreaseAmount,t1.MNetAmount,t2.MName AS MFixAssetsName,t3.MName AS MFATypeIDName ";
			if (filter.IncludeCheckType)
			{
				str += " ,convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactNameExp,\r\n                                    F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeNameExp,\r\n                                    concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemNameExp,\r\n                                    t9.MName AS MExpItemNameExp,t10.MName AS MPaItemNameExp,\r\n                                    t10_0.MGroupID AS MPaItemGroupIDExp,t10_1.MName AS MPaItemGroupNameExp, t11.MName AS MTrackItem1NameExp,\r\n                                    t11_2.MName AS MTrackItem1GroupNameExp,t12.MName AS MTrackItem2NameExp,t12_2.MName AS MTrackItem2GroupNameExp,\r\n                                    t13.MName AS MTrackItem3NameExp,t13_2.MName AS MTrackItem3GroupNameExp,t14.MName AS MTrackItem4NameExp,\r\n                                    t14_2.MName AS MTrackItem4GroupNameExp,t15.MName AS MTrackItem5NameExp,t15_2.MName AS MTrackItem5GroupNameExp,\r\n\r\n                                    convert(AES_DECRYPT(t16.MName,'{0}') using utf8) as MContactNameDep,\r\n                                    F_GETUSERNAME(t17.MFirstName, t17.MLastName) AS MEmployeeNameDep,\r\n                                    concat(t18_0.MNumber,':',t18.MDesc) AS MMerItemNameDep,\r\n                                    t19.MName AS MExpItemNameDep,t20.MName AS MPaItemNameDep,\r\n                                    t20_0.MGroupID AS MPaItemGroupIDDep,t20_1.MName as MPaItemGroupNameDep,\r\n                                    t21.MName AS MTrackItem1NameDep,t21_2.MName AS MTrackItem1GroupNameDep,t22.MName AS MTrackItem2NameDep,\r\n                                    t22_2.MName AS MTrackItem2GroupNameDep,t23.MName AS MTrackItem3NameDep,t23_2.MName AS MTrackItem3GroupNameDep,\r\n                                    t24.MName AS MTrackItem4NameDep,t24_2.MName AS MTrackItem4GroupNameDep,t25.MName AS MTrackItem5NameDep,\r\n                                    t25_2.MName AS MTrackItem5GroupNameDep ";
			}
			str += " FROM ( SELECT * FROM t_fa_fixassets WHERE MIsDelete = 0 \r\n                                AND MPurchaseDate BETWEEN @MStartPeroid AND @MEndPeroid AND MOrgID = @MOrgID ) t1\r\n                              LEFT JOIN t_fa_fixassets_l t2 ON t1.MItemID = t2.MParentID\r\n                                AND t2.MIsDelete = 0 AND t2.MLocaleID = @MLCID\r\n                              LEFT JOIN t_fa_fixassetstype_l t3 ON t1.MFATypeID = t3.MParentID\r\n                                AND t3.MIsDelete = 0 AND t3.MLocaleID = @MLCID  ";
			if (filter.IncludeCheckType)
			{
				str += " LEFT JOIN t_gl_checkgroupvalue t5 ON t5.MItemID = t1.MExpCheckGroupValueID\r\n                                        AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t6 ON t6.MParentID = t5.MContactID \r\n                                        AND t6.MLocaleID = @MLCID AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_employees_l t7 ON t7.MParentID = t5.MEmployeeID \r\n                                        AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                                        AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                                     LEFT JOIN t_bd_item_l t8 ON t8.MParentID = t5.MMerItemID \r\n                                        AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_expenseitem_l t9 ON t9.MParentID = t5.MExpItemID \r\n                                        AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem_l t10 ON t10.MParentID = t5.MPaItemID \r\n                                        AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem t10_0 ON t10_0.MItemID = t5.MPaItemID \r\n                                        AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                                     LEFT JOIN t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t5.MPaItemID \r\n                                        AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_trackentry_l t11 ON t11.MParentID = t5.MTrackItem1 \r\n                                        AND t11.MLocaleID = @MLCID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t11_1 ON t11_1.MEntryID = t5.MTrackItem1 \r\n                                        AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t11_2 ON t11_2.MParentID = t11_1.MItemID \r\n                                        AND t11_2.MLocaleID = @MLCID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t12 ON t12.MParentID = t5.MTrackItem2 \r\n                                        AND t12.MLocaleID = @MLCID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t12_1 ON t12_1.MEntryID = t5.MTrackItem2 \r\n                                        AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t12_2 ON t12_2.MParentID = t12_1.MItemID \r\n                                        AND t12_2.MLocaleID = @MLCID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t13 ON t13.MParentID = t5.MTrackItem3 \r\n                                        AND t13.MLocaleID = @MLCID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t13_1 ON t13_1.MEntryID = t5.MTrackItem3 \r\n                                        AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t13_2 ON t13_2.MParentID = t13_1.MItemID \r\n                                        AND t13_2.MLocaleID = @MLCID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t14 ON t14.MParentID = t5.MTrackItem4\r\n                                        AND t14.MLocaleID = @MLCID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t14_1 ON t14_1.MEntryID = t5.MTrackItem4 \r\n                                        AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t14_2 ON t14_2.MParentID = t14_1.MItemID\r\n                                        AND t14_2.MLocaleID = @MLCID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t15 ON t15.MParentID = t5.MTrackItem5\r\n                                        AND t15.MLocaleID = @MLCID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t15_1 ON t15_1.MEntryID = t5.MTrackItem5 \r\n                                        AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t15_2 ON t15_2.MParentID = t15_1.MItemID \r\n                                        AND t15_2.MLocaleID = @MLCID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0\r\n\r\n                                     LEFT JOIN t_gl_checkgroupvalue t26 ON t26.MItemID = t1.MDepCheckGroupValueID\r\n                                        AND t26.MOrgID = t1.MOrgID AND t26.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t16 ON t16.MParentID = t26.MContactID \r\n                                        AND t16.MLocaleID = @MLCID AND t16.MOrgID = t1.MOrgID AND t16.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_employees_l t17 ON t17.MParentID = t26.MEmployeeID \r\n                                        AND t17.MOrgID = t1.MOrgId AND t17.MIsDelete = t1.MIsDelete AND t17.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_item t18_0 ON t18_0.MItemID = t26.MMerItemID\r\n                                        AND t18_0.MOrgID = t1.MOrgId AND t18_0.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_item_l t18 ON t18.MParentID = t26.MMerItemID \r\n                                        AND t18.MOrgID = t1.MOrgId AND t18.MIsDelete = t1.MIsDelete AND t18.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_expenseitem_l t19 ON t19.MParentID = t26.MExpItemID \r\n                                        AND t19.MOrgID = t1.MOrgId AND t19.MIsDelete = t1.MIsDelete AND t19.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem_l t20 ON t20.MParentID = t26.MPaItemID \r\n                                        AND t20.MOrgID = t1.MOrgId AND t20.MIsDelete = t1.MIsDelete AND t20.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem t20_0 ON t20_0.MItemID = t26.MPaItemID \r\n                                        AND t20_0.MOrgID = t1.MOrgId AND t20_0.MIsDelete = 0               \r\n                                    LEFT JOIN t_pa_payitemgroup_l t20_1 ON t20_1.MParentID = t26.MPaItemID\r\n                                        AND t20_1.MOrgID = t1.MOrgId AND t20_1.MIsDelete = 0 AND t20_1.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_trackentry_l t21 ON t21.MParentID = t26.MTrackItem1 \r\n                                        AND t21.MLocaleID = @MLCID AND t21.MOrgID = t1.MOrgID AND t21.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t21_1 ON t21_1.MEntryID = t26.MTrackItem1 \r\n                                        AND t21_1.MOrgID = t1.MOrgID AND t21_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t21_2 ON t21_2.MParentID = t21_1.MItemID \r\n                                        AND t21_2.MLocaleID = @MLCID AND t21_2.MOrgID = t1.MOrgID AND t21_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t22 ON t22.MParentID = t26.MTrackItem2 \r\n                                        AND t22.MLocaleID = @MLCID AND t22.MOrgID = t1.MOrgID AND t22.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t22_1 ON t22_1.MEntryID = t26.MTrackItem2 \r\n                                        AND t22_1.MOrgID = t1.MOrgID AND t22_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t22_2 ON t22_2.MParentID = t22_1.MItemID \r\n                                        AND t22_2.MLocaleID = @MLCID AND t22_2.MOrgID = t1.MOrgID AND t22_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t23 ON t23.MParentID = t26.MTrackItem3 \r\n                                        AND t23.MLocaleID = @MLCID AND t23.MOrgID = t1.MOrgID AND t23.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t23_1 ON t23_1.MEntryID = t26.MTrackItem3 \r\n                                        AND t23_1.MOrgID = t1.MOrgID AND t23_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t23_2 ON t23_2.MParentID = t23_1.MItemID \r\n                                        AND t23_2.MLocaleID = @MLCID AND t23_2.MOrgID = t1.MOrgID AND t23_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t24 ON t24.MParentID = t26.MTrackItem4\r\n                                        AND t24.MLocaleID = @MLCID AND t24.MOrgID = t1.MOrgID AND t24.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t24_1 ON t24_1.MEntryID = t26.MTrackItem4 \r\n                                        AND t24_1.MOrgID = t1.MOrgID AND t24_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t24_2 ON t24_2.MParentID = t24_1.MItemID \r\n                                        AND t24_2.MLocaleID = @MLCID AND t24_2.MOrgID = t1.MOrgID AND t24_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t25 ON t25.MParentID = t26.MTrackItem5 \r\n                                        AND t25.MLocaleID = @MLCID AND t25.MOrgID = t1.MOrgID AND t25.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t25_1 ON t25_1.MEntryID = t26.MTrackItem5 \r\n                                        AND t25_1.MOrgID = t1.MOrgID AND t25_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t25_2 ON t25_2.MParentID = t25_1.MItemID \r\n                                        AND t25_2.MLocaleID = @MLCID AND t25_2.MOrgID = t1.MOrgID AND t25_2.MIsDelete = 0 ";
			}
			return string.Format(str, "JieNor-001");
		}

		public static void FixAssetDealSummary(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			SetAssetDealSummaryRowHead(context, model, filter);
			SetAssetDealSummaryRowData(context, model, filter);
		}

		private static void SetAssetDealSummaryRowHead(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Category", "资产类别"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Encoding", "资产编码"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FAName", "资产名称"),
				CellType = BizReportCellType.Text
			});
			if (filter.IncludeCheckType)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "AccountDimension", "核算维度"),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "DealDate", "处理时间"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "OutOriginalAmount", "转出原值"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "OutAccumulateDepreciation", "转出累计折旧"),
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetAssetDealSummaryRowData(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			List<RPTAssetChangeModel> list = (from m in GetAssetDealSummaryList(context, model, filter)
			orderby m.MFATypeID, m.MNumber
			select m).ToList();
			List<RPTAssetChangeModel> fADepreciationList = GetFADepreciationList(context, filter);
			string tempFATypeID = string.Empty;
			int num = list.Count();
			if (list.Count >= 1)
			{
				foreach (RPTAssetChangeModel item in list)
				{
					tempFATypeID = item.MFATypeID;
					List<RPTAssetChangeModel> source = (from m in fADepreciationList
					where m.MYear == item.MLastDepreciatedYear && m.MPeriod == item.MLastDepreciatedPeriod && m.MId == item.MitemId && !string.IsNullOrEmpty(m.MVoucherID)
					select m).ToList();
					item.MDepreciatedAmountChange = (source.Any() ? source.Sum((RPTAssetChangeModel m) => m.MDepreciatedAmount) : item.MDepreciatedAmount);
					BizReportRowModel bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MFATypeIDName),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MFixAssetsNumber),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MFixAssetsName),
						CellType = BizReportCellType.Text
					});
					if (filter.IncludeCheckType)
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(RPTBaseREpository.GetCheckGroupValue(context, item)),
							CellType = BizReportCellType.Text
						});
					}
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MHandledDate.ToString("yyyy-MM-dd")),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MOriginalAmount),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(item.MDepreciatedAmountChange),
						CellType = BizReportCellType.Money
					});
					model.AddRow(bizReportRowModel);
					int num2 = list.IndexOf(item);
					if (num2 == num - 1)
					{
						List<RPTAssetChangeModel> lists = (from m in list
						where m.MFATypeID == tempFATypeID
						select m).ToList();
						GetAssetDealSummaryTotal(context, model, filter, BizReportRowType.SubTotal, lists);
					}
					else if (num2 < num - 1 && tempFATypeID != list[num2 + 1].MFATypeID)
					{
						List<RPTAssetChangeModel> lists2 = (from m in list
						where m.MFATypeID == tempFATypeID
						select m).ToList();
						GetAssetDealSummaryTotal(context, model, filter, BizReportRowType.SubTotal, lists2);
					}
				}
				GetAssetDealSummaryTotal(context, model, filter, BizReportRowType.Total, list);
			}
		}

		private static List<RPTAssetChangeModel> GetAssetDealSummaryList(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			string assetDealSummarysSql = GetAssetDealSummarysSql(filter);
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartPeroid", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndPeroid", MySqlDbType.VarChar, 36)
			};
			DateTime dateTime = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime2 = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			dateTime2 = dateTime2.AddMonths(1);
			DateTime dateTime3 = dateTime2.AddSeconds(-1.0);
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			array[2].Value = dateTime;
			array[3].Value = dateTime3;
			return ModelInfoManager.GetDataModelBySql<RPTAssetChangeModel>(context, assetDealSummarysSql, array);
		}

		private static string GetAssetDealSummarysSql(RPTAssetChangeFilterModel filter)
		{
			string str = " SELECT t1.MItemId,t1.MPurchaseDate,t1.MFATypeID,t1.MPrefix,t1.MNumber,t1.MOriginalAmount,t1.MHandledDate,t1.MStatus,\r\n                              t1.MDepreciatedAmount,t1.MPrepareForDecreaseAmount,t1.MNetAmount,t2.MName AS MFixAssetsName,t3.MName AS MFATypeIDName,\r\n                              t1.MLastDepreciatedYear, t1.MLastDepreciatedPeriod   ";
			if (filter.IncludeCheckType)
			{
				str += " ,convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactNameExp,\r\n                                    F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeNameExp,\r\n                                    concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemNameExp,\r\n                                    t9.MName AS MExpItemNameExp,t10.MName AS MPaItemNameExp,\r\n                                    t10_0.MGroupID AS MPaItemGroupIDExp,t10_1.MName AS MPaItemGroupNameExp, t11.MName AS MTrackItem1NameExp,\r\n                                    t11_2.MName AS MTrackItem1GroupNameExp,t12.MName AS MTrackItem2NameExp,t12_2.MName AS MTrackItem2GroupNameExp,\r\n                                    t13.MName AS MTrackItem3NameExp,t13_2.MName AS MTrackItem3GroupNameExp,t14.MName AS MTrackItem4NameExp,\r\n                                    t14_2.MName AS MTrackItem4GroupNameExp,t15.MName AS MTrackItem5NameExp,t15_2.MName AS MTrackItem5GroupNameExp,\r\n\r\n                                    convert(AES_DECRYPT(t16.MName,'{0}') using utf8) as MContactNameDep,\r\n                                    F_GETUSERNAME(t17.MFirstName, t17.MLastName) AS MEmployeeNameDep,\r\n                                    concat(t18_0.MNumber,':',t18.MDesc) AS MMerItemNameDep,\r\n                                    t19.MName AS MExpItemNameDep,t20.MName AS MPaItemNameDep,\r\n                                    t20_0.MGroupID AS MPaItemGroupIDDep,t20_1.MName as MPaItemGroupNameDep,\r\n                                    t21.MName AS MTrackItem1NameDep,t21_2.MName AS MTrackItem1GroupNameDep,t22.MName AS MTrackItem2NameDep,\r\n                                    t22_2.MName AS MTrackItem2GroupNameDep,t23.MName AS MTrackItem3NameDep,t23_2.MName AS MTrackItem3GroupNameDep,\r\n                                    t24.MName AS MTrackItem4NameDep,t24_2.MName AS MTrackItem4GroupNameDep,t25.MName AS MTrackItem5NameDep,\r\n                                    t25_2.MName AS MTrackItem5GroupNameDep ";
			}
			str += " FROM ( SELECT * FROM t_fa_fixassets WHERE (MStatus = 1 OR MStatus = 2)  AND MIsDelete = 0 \r\n                                AND MHandledDate BETWEEN @MStartPeroid AND @MEndPeroid  AND MOrgID=@MOrgID ) t1\r\n                              LEFT JOIN t_fa_fixassets_l t2 ON t1.MItemID = t2.MParentID\r\n                                AND t2.MIsDelete = 0 AND t2.MLocaleID = @MLCID\r\n                              LEFT JOIN t_fa_fixassetstype_l t3 ON t1.MFATypeID = t3.MParentID\r\n                                AND t3.MIsDelete = 0 AND t3.MLocaleID = @MLCID ";
			if (filter.IncludeCheckType)
			{
				str += " LEFT JOIN t_gl_checkgroupvalue t5 ON t5.MItemID = t1.MExpCheckGroupValueID\r\n                                        AND t5.MOrgID = t1.MOrgID AND t5.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t6 ON t6.MParentID = t5.MContactID \r\n                                        AND t6.MLocaleID = @MLCID AND t6.MOrgID = t1.MOrgID AND t6.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_employees_l t7 ON t7.MParentID = t5.MEmployeeID \r\n                                        AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_item t8_0 ON t8_0.MItemID = t5.MMerItemID\r\n                                        AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                                     LEFT JOIN t_bd_item_l t8 ON t8.MParentID = t5.MMerItemID \r\n                                        AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_expenseitem_l t9 ON t9.MParentID = t5.MExpItemID \r\n                                        AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem_l t10 ON t10.MParentID = t5.MPaItemID \r\n                                        AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem t10_0 ON t10_0.MItemID = t5.MPaItemID \r\n                                        AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                                     LEFT JOIN t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t5.MPaItemID \r\n                                        AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_trackentry_l t11 ON t11.MParentID = t5.MTrackItem1 \r\n                                        AND t11.MLocaleID = @MLCID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t11_1 ON t11_1.MEntryID = t5.MTrackItem1 \r\n                                        AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t11_2 ON t11_2.MParentID = t11_1.MItemID \r\n                                        AND t11_2.MLocaleID = @MLCID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t12 ON t12.MParentID = t5.MTrackItem2 \r\n                                        AND t12.MLocaleID = @MLCID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t12_1 ON t12_1.MEntryID = t5.MTrackItem2 \r\n                                        AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t12_2 ON t12_2.MParentID = t12_1.MItemID \r\n                                        AND t12_2.MLocaleID = @MLCID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t13 ON t13.MParentID = t5.MTrackItem3 \r\n                                        AND t13.MLocaleID = @MLCID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t13_1 ON t13_1.MEntryID = t5.MTrackItem3 \r\n                                        AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t13_2 ON t13_2.MParentID = t13_1.MItemID \r\n                                        AND t13_2.MLocaleID = @MLCID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t14 ON t14.MParentID = t5.MTrackItem4\r\n                                        AND t14.MLocaleID = @MLCID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t14_1 ON t14_1.MEntryID = t5.MTrackItem4 \r\n                                        AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t14_2 ON t14_2.MParentID = t14_1.MItemID\r\n                                        AND t14_2.MLocaleID = @MLCID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t15 ON t15.MParentID = t5.MTrackItem5\r\n                                        AND t15.MLocaleID = @MLCID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t15_1 ON t15_1.MEntryID = t5.MTrackItem5 \r\n                                        AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t15_2 ON t15_2.MParentID = t15_1.MItemID \r\n                                        AND t15_2.MLocaleID = @MLCID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0\r\n\r\n                                     LEFT JOIN t_gl_checkgroupvalue t26 ON t26.MItemID = t1.MDepCheckGroupValueID\r\n                                        AND t26.MOrgID = t1.MOrgID AND t26.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t16 ON t16.MParentID = t26.MContactID \r\n                                        AND t16.MLocaleID = @MLCID AND t16.MOrgID = t1.MOrgID AND t16.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_employees_l t17 ON t17.MParentID = t26.MEmployeeID \r\n                                        AND t17.MOrgID = t1.MOrgId AND t17.MIsDelete = t1.MIsDelete AND t17.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_item t18_0 ON t18_0.MItemID = t26.MMerItemID\r\n                                        AND t18_0.MOrgID = t1.MOrgId AND t18_0.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_item_l t18 ON t18.MParentID = t26.MMerItemID \r\n                                        AND t18.MOrgID = t1.MOrgId AND t18.MIsDelete = t1.MIsDelete AND t18.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_expenseitem_l t19 ON t19.MParentID = t26.MExpItemID \r\n                                        AND t19.MOrgID = t1.MOrgId AND t19.MIsDelete = t1.MIsDelete AND t19.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem_l t20 ON t20.MParentID = t26.MPaItemID \r\n                                        AND t20.MOrgID = t1.MOrgId AND t20.MIsDelete = t1.MIsDelete AND t20.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem t20_0 ON t20_0.MItemID = t26.MPaItemID \r\n                                        AND t20_0.MOrgID = t1.MOrgId AND t20_0.MIsDelete = 0               \r\n                                    LEFT JOIN t_pa_payitemgroup_l t20_1 ON t20_1.MParentID = t26.MPaItemID\r\n                                        AND t20_1.MOrgID = t1.MOrgId AND t20_1.MIsDelete = 0 AND t20_1.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_trackentry_l t21 ON t21.MParentID = t26.MTrackItem1 \r\n                                        AND t21.MLocaleID = @MLCID AND t21.MOrgID = t1.MOrgID AND t21.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t21_1 ON t21_1.MEntryID = t26.MTrackItem1 \r\n                                        AND t21_1.MOrgID = t1.MOrgID AND t21_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t21_2 ON t21_2.MParentID = t21_1.MItemID \r\n                                        AND t21_2.MLocaleID = @MLCID AND t21_2.MOrgID = t1.MOrgID AND t21_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t22 ON t22.MParentID = t26.MTrackItem2 \r\n                                        AND t22.MLocaleID = @MLCID AND t22.MOrgID = t1.MOrgID AND t22.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t22_1 ON t22_1.MEntryID = t26.MTrackItem2 \r\n                                        AND t22_1.MOrgID = t1.MOrgID AND t22_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t22_2 ON t22_2.MParentID = t22_1.MItemID \r\n                                        AND t22_2.MLocaleID = @MLCID AND t22_2.MOrgID = t1.MOrgID AND t22_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t23 ON t23.MParentID = t26.MTrackItem3 \r\n                                        AND t23.MLocaleID = @MLCID AND t23.MOrgID = t1.MOrgID AND t23.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t23_1 ON t23_1.MEntryID = t26.MTrackItem3 \r\n                                        AND t23_1.MOrgID = t1.MOrgID AND t23_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t23_2 ON t23_2.MParentID = t23_1.MItemID \r\n                                        AND t23_2.MLocaleID = @MLCID AND t23_2.MOrgID = t1.MOrgID AND t23_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t24 ON t24.MParentID = t26.MTrackItem4\r\n                                        AND t24.MLocaleID = @MLCID AND t24.MOrgID = t1.MOrgID AND t24.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t24_1 ON t24_1.MEntryID = t26.MTrackItem4 \r\n                                        AND t24_1.MOrgID = t1.MOrgID AND t24_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t24_2 ON t24_2.MParentID = t24_1.MItemID \r\n                                        AND t24_2.MLocaleID = @MLCID AND t24_2.MOrgID = t1.MOrgID AND t24_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t25 ON t25.MParentID = t26.MTrackItem5 \r\n                                        AND t25.MLocaleID = @MLCID AND t25.MOrgID = t1.MOrgID AND t25.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t25_1 ON t25_1.MEntryID = t26.MTrackItem5 \r\n                                        AND t25_1.MOrgID = t1.MOrgID AND t25_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t25_2 ON t25_2.MParentID = t25_1.MItemID \r\n                                        AND t25_2.MLocaleID = @MLCID AND t25_2.MOrgID = t1.MOrgID AND t25_2.MIsDelete = 0 ";
			}
			return string.Format(str, "JieNor-001");
		}

		private static void GetAssetDealSummaryTotal(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter, BizReportRowType rowType, List<RPTAssetChangeModel> lists)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = rowType;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total") + " " + ((rowType == BizReportRowType.SubTotal) ? lists.FirstOrDefault().MFATypeIDName : "")),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			if (filter.IncludeCheckType)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(""),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(lists.Sum((RPTAssetChangeModel m) => m.MOriginalAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(lists.Sum((RPTAssetChangeModel m) => m.MDepreciatedAmountChange)),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel);
		}

		public static void FixAssetChangeSummary(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
		}

		private static void SetTitle(MContext context, BizReportModel model, RPTAssetChangeFilterModel filter)
		{
			string title = "";
			switch (filter.MReportType)
			{
			case 1:
				title = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FixAssetChangeSituation", "固定资产变动情况表");
				break;
			case 2:
				title = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FixAssetAddSummary", "固定资产新增统计");
				break;
			case 3:
				title = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FixAssetDealSummary", "固定资产处理统计");
				break;
			case 4:
				title = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FixAssetChangeSummary", "固定资产变更统计");
				break;
			}
			model.Title1 = title;
			model.Title2 = context.MOrgName;
			model.Title3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + filter.MStartPeroid + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to") + " " + filter.MEndPeroid;
		}
	}
}
