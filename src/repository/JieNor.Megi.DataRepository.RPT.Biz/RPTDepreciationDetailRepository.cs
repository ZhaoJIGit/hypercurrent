using JieNor.Megi.Core;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.RPT;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.DataRepository.RPT.Biz
{
	public class RPTDepreciationDetailRepository : RPTFABaseRepository
	{
		private static GLUtility utility = new GLUtility();

		public static BizReportModel GetDepreciationDetailData(MContext context, RPTDepreciationDetailFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel
			{
				Type = Convert.ToInt32(BizReportType.DepreciationDetail)
			};
			SetTitle(context, bizReportModel, filter);
			SetRowHead(context, bizReportModel, filter);
			SetRowData(context, bizReportModel, filter);
			return bizReportModel;
		}

		private static void SetTitle(MContext context, BizReportModel model, RPTDepreciationDetailFilterModel filter)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "DereciationDetail", "折旧明细表");
			model.Title2 = context.MOrgName;
			model.Title3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + filter.MStartPeroid + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to") + " " + filter.MEndPeroid;
		}

		private static void SetRowHead(MContext context, BizReportModel model, RPTDepreciationDetailFilterModel filter)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.FA, "FixAssetsTypeOptions", "资产类别"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.FA, "FixAssetsNumber", "资产编码"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.FA, "FixAssetsName", "资产名称"),
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
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "OriginalAmount", "原值"),
				CellType = BizReportCellType.Text
			});
			DateTime dateTime = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime2 = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			int num = (dateTime2.Year - dateTime.Year) * 12 + (dateTime2.Month - dateTime.Month);
			DateTime dateTime3 = dateTime;
			for (int i = 0; i <= num; i++)
			{
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.FA, "PeriodDepreciateAmount", "月折旧") + "[" + dateTime3.Year + "-" + ((dateTime3.Month < 10) ? ("0" + dateTime3.Month) : dateTime3.Month.ToString()) + "]",
					CellType = BizReportCellType.Text
				});
				dateTime3 = dateTime3.AddMonths(1);
			}
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FinalAccumulatedDepreciation", "期末累计折旧"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "YTDDeprection", "本年累计折旧"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "DepreciationReserves", "减值准备"),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "FinalNetWorth", "期末净额"),
				CellType = BizReportCellType.Text
			});
			model.AddRow(bizReportRowModel);
		}

		private static void SetRowData(MContext ctx, BizReportModel model, RPTDepreciationDetailFilterModel filter)
		{
			List<RPTDepreciationDetailModel> source = (from m in GetDepreciationDetail(ctx, filter)
			orderby m.MFixAssetsNumber descending
			select m).ToList();
			List<FAFixAssetsChangeModel> fAChangeList = RPTFABaseRepository.GetFAChangeList(ctx);
			if (filter.IncludeCheckType && filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count > 0)
			{
				foreach (NameValueModel checkTypeValue in filter.CheckTypeValueList)
				{
					switch (checkTypeValue.MName)
					{
					case "0":
						source = (from m in source
						where m.MContactIDDep == checkTypeValue.MValue || m.MContactIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "1":
						source = (from m in source
						where m.MEmployeeIDDep == checkTypeValue.MValue || m.MEmployeeIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "2":
						source = (from m in source
						where m.MMerItemIDDep == checkTypeValue.MValue || m.MMerItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "3":
						source = (from m in source
						where m.MExpItemIDDep == checkTypeValue.MValue || m.MExpItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "4":
						source = (from m in source
						where m.MPaItemIDDep == checkTypeValue.MValue || m.MPaItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "5":
						source = (from m in source
						where m.MTrackItem1Dep == checkTypeValue.MValue || m.MTrackItem1Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "6":
						source = (from m in source
						where m.MTrackItem2Dep == checkTypeValue.MValue || m.MTrackItem2Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "7":
						source = (from m in source
						where m.MTrackItem3Dep == checkTypeValue.MValue || m.MTrackItem3Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "8":
						source = (from m in source
						where m.MTrackItem4Dep == checkTypeValue.MValue || m.MTrackItem4Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "9":
						source = (from m in source
						where m.MTrackItem5Dep == checkTypeValue.MValue || m.MTrackItem5Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					}
				}
			}
			List<RPTDepreciationDetailModel> list = (from p in source
			group p by new
			{
				p.MitemId
			} into g
			select new RPTDepreciationDetailModel
			{
				MitemId = g.Key.MitemId
			}).ToList();
			List<RPTDepreciationDetailModel> list2 = new List<RPTDepreciationDetailModel>();
			Dictionary<int, List<decimal>> dictionary = new Dictionary<int, List<decimal>>();
			DateTime startDate = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime dateTime2 = dateTime.AddMonths(1);
			foreach (RPTDepreciationDetailModel item3 in list)
			{
				RPTDepreciationDetailModel rPTDepreciationDetailModel = source.FirstOrDefault((RPTDepreciationDetailModel m) => m.MitemId == item3.MitemId);
				if (rPTDepreciationDetailModel != null)
				{
					RPTDepreciationDetailModel rPTDepreciationDetailModel2 = new RPTDepreciationDetailModel();
					GetValueChangeItems(fAChangeList, rPTDepreciationDetailModel, dateTime2, startDate);
					decimal num = (rPTDepreciationDetailModel.MStatus != 0 && rPTDepreciationDetailModel.MHandledDate < dateTime2) ? decimal.Zero : rPTDepreciationDetailModel.MOriginalAmount;
					decimal num2 = default(decimal);
					decimal num3 = default(decimal);
					decimal num4 = default(decimal);
					decimal num5 = default(decimal);
					BizReportRowModel bizReportRowModel = new BizReportRowModel();
					bizReportRowModel.RowType = BizReportRowType.Item;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(rPTDepreciationDetailModel.MFATypeIDName),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(rPTDepreciationDetailModel.MFixAssetsNumber),
						CellType = BizReportCellType.Text
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(rPTDepreciationDetailModel.MFixAssetsName),
						CellType = BizReportCellType.Text
					});
					if (filter.IncludeCheckType)
					{
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = Convert.ToString(RPTBaseREpository.GetCheckGroupValue(ctx, rPTDepreciationDetailModel)),
							CellType = BizReportCellType.Text
						});
					}
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(num),
						CellType = BizReportCellType.Money
					});
					DateTime dateTime3 = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
					DateTime t = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
					int num6 = (t.Year - dateTime3.Year) * 12 + (t.Month - dateTime3.Month);
					DateTime dateTime4 = dateTime3;
					for (int i = 0; i <= num6; i++)
					{
						DateTime peroid = dateTime4;
						List<RPTDepreciationDetailModel> source2 = (from m in source
						where m.MYearPeriod == peroid.Year * 100 + peroid.Month && !string.IsNullOrWhiteSpace(m.MVoucherID) && m.MitemId == item3.MitemId
						select m).ToList();
						decimal item2 = source2.Sum((RPTDepreciationDetailModel m) => m.MPeriodDepreciatedAmount);
						bizReportRowModel.AddCell(new BizReportCellModel
						{
							Value = item2.ToString(),
							CellType = BizReportCellType.Money
						});
						if (!dictionary.ContainsKey(i))
						{
							dictionary.Add(i, new List<decimal>
							{
								item2
							});
						}
						else
						{
							List<decimal> list3 = dictionary[i];
							list3.Add(item2);
						}
						dateTime4 = dateTime4.AddMonths(1);
					}
					List<RPTDepreciationDetailModel> list4 = (from m in source
					orderby m.MYearPeriod descending
					where m.MitemId == item3.MitemId && m.MYearPeriod <= Convert.ToInt32(filter.MEndPeroid) && m.MYearPeriod > 0 && !string.IsNullOrWhiteSpace(m.MVoucherID)
					select m).ToList();
					List<RPTDepreciationDetailModel> notDeplastItemModel = (from m in source
					orderby m.MYearPeriod descending
					where m.MitemId == item3.MitemId && m.MYearPeriod <= Convert.ToInt32(filter.MEndPeroid) && m.MYearPeriod > 0
					select m).ToList();
					if (list4.Count <= 0)
					{
						if (ctx.MFABeginDate > t)
						{
							num2 = default(decimal);
							num4 = default(decimal);
							num3 = default(decimal);
						}
						else if (notDeplastItemModel.Count > 0)
						{
							List<RPTDepreciationDetailModel> source3 = (from m in notDeplastItemModel
							where m.MYearPeriod == notDeplastItemModel.FirstOrDefault().MYearPeriod
							select m).ToList();
							num2 = source3.Sum((RPTDepreciationDetailModel m) => m.MOrgDepreciatedAmount);
							num3 = source3.FirstOrDefault().MOrgDepreciatedAmountOfYear;
							num4 = rPTDepreciationDetailModel.MOrgPrepareForDecreaseAmount;
						}
						else
						{
							int num7;
							if (dateTime3 <= rPTDepreciationDetailModel.MDepreciationFromPeriod && rPTDepreciationDetailModel.MDepreciationFromPeriod <= t)
							{
								dateTime = rPTDepreciationDetailModel.MDepreciationFromPeriod;
								num7 = ((dateTime.Month == 1) ? 1 : 0);
							}
							else
							{
								num7 = 0;
							}
							if (num7 != 0)
							{
								num2 = rPTDepreciationDetailModel.MOrgDepreciatedAmount;
								num4 = rPTDepreciationDetailModel.MOrgPrepareForDecreaseAmount;
								num3 = default(decimal);
							}
							else
							{
								num2 = rPTDepreciationDetailModel.MOrgDepreciatedAmount;
								num4 = rPTDepreciationDetailModel.MOrgPrepareForDecreaseAmount;
								num3 = rPTDepreciationDetailModel.MOrgDepreciatedAmount;
							}
						}
					}
					else
					{
						RPTDepreciationDetailModel rPTDepreciationDetailModel3 = list4.FirstOrDefault();
						int yearPeriod = rPTDepreciationDetailModel3.MYearPeriod;
						List<RPTDepreciationDetailModel> source4 = (from m in list4
						where m.MYearPeriod == yearPeriod
						select m).ToList();
						num2 = source4.Sum((RPTDepreciationDetailModel m) => m.MDepreciatedAmount);
						int year = t.Year;
						int num8 = yearPeriod / 100;
						num3 = ((year != num8) ? decimal.Zero : source4.Sum((RPTDepreciationDetailModel m) => m.MDepreciatedAmountOfYear));
						num4 = rPTDepreciationDetailModel.MOrgPrepareForDecreaseAmount;
					}
					if (rPTDepreciationDetailModel.MStatus != 0 && rPTDepreciationDetailModel.MHandledDate < dateTime2)
					{
						num = default(decimal);
						num2 = default(decimal);
						num4 = default(decimal);
					}
					num5 = num - num2 - num4;
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(num2),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(num3),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(num4),
						CellType = BizReportCellType.Money
					});
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(num5),
						CellType = BizReportCellType.Money
					});
					rPTDepreciationDetailModel2.MOriginalAmount = num;
					rPTDepreciationDetailModel2.MDepreciatedAmount = num2;
					rPTDepreciationDetailModel2.MDepreciatedAmountOfYear = num3;
					rPTDepreciationDetailModel2.MPrepareForDecreaseAmount = num4;
					rPTDepreciationDetailModel2.MNetAmount = num5;
					list2.Add(rPTDepreciationDetailModel2);
					model.AddRow(bizReportRowModel);
				}
			}
			BizReportRowModel bizReportRowModel2 = new BizReportRowModel();
			bizReportRowModel2.RowType = BizReportRowType.Item;
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(""),
				CellType = BizReportCellType.Text
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Report, "Total", "Total")),
				CellType = BizReportCellType.Text
			});
			if (filter.IncludeCheckType)
			{
				bizReportRowModel2.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(""),
					CellType = BizReportCellType.Text
				});
			}
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list2.Sum((RPTDepreciationDetailModel m) => m.MOriginalAmount)),
				CellType = BizReportCellType.Money
			});
			for (int j = 0; j < dictionary.Count; j++)
			{
				bizReportRowModel2.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(dictionary[j].Sum()),
					CellType = BizReportCellType.Money
				});
			}
			if (dictionary.Count == 0)
			{
				bizReportRowModel2.AddCell(new BizReportCellModel
				{
					Value = "0",
					CellType = BizReportCellType.Money
				});
			}
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list2.Sum((RPTDepreciationDetailModel m) => m.MDepreciatedAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list2.Sum((RPTDepreciationDetailModel m) => m.MDepreciatedAmountOfYear)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list2.Sum((RPTDepreciationDetailModel m) => m.MPrepareForDecreaseAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list2.Sum((RPTDepreciationDetailModel m) => m.MNetAmount)),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel2);
		}

		public static void GetValueChangeItems(List<FAFixAssetsChangeModel> list, RPTDepreciationDetailModel model, DateTime endDate, DateTime startDate)
		{
			List<FAFixAssetsChangeModel> list2 = (from m in list
			where m.MID == model.MitemId
			orderby m.MIndex descending
			select m).ToList();
			if (list2.Count > 0)
			{
				List<FAFixAssetsChangeModel> list3 = list2.Where(delegate(FAFixAssetsChangeModel m)
				{
					int result;
					if (m.MChangeFromPeriod < endDate)
					{
						DateTime dateTime2 = m.MChangeFromPeriod;
						string a2 = dateTime2.ToString("yyyy-MM-dd");
						dateTime2 = DateTime.MinValue;
						dateTime2 = dateTime2.Date;
						result = ((a2 != dateTime2.ToString("yyyy-MM-dd")) ? 1 : 0);
					}
					else
					{
						result = 0;
					}
					return (byte)result != 0;
				}).ToList();
				if (list3.Count > 0)
				{
					FAFixAssetsChangeModel fAFixAssetsChangeModel = list3.FirstOrDefault();
					model.MOriginalAmount = fAFixAssetsChangeModel.MOriginalAmount;
					model.MOrgPrepareForDecreaseAmount = fAFixAssetsChangeModel.MPrepareForDecreaseAmount;
				}
				else
				{
					List<FAFixAssetsChangeModel> source = (from m in list2.Where(delegate(FAFixAssetsChangeModel m)
					{
						DateTime dateTime = m.MChangeFromPeriod;
						string a = dateTime.ToString("yyyy-MM-dd");
						dateTime = DateTime.MinValue;
						return a != dateTime.ToString("yyyy-MM-dd") && m.MChangeFromPeriod > startDate;
					})
					orderby m.MIndex descending
					select m).ToList();
					if (source.Any())
					{
						FAFixAssetsChangeModel fAFixAssetsChangeModel2 = source.FirstOrDefault();
						int changeIndex = fAFixAssetsChangeModel2.MIndex;
						FAFixAssetsChangeModel fAFixAssetsChangeModel3 = list2.FirstOrDefault((FAFixAssetsChangeModel m) => m.MIndex == changeIndex - 1);
						model.MOriginalAmount = fAFixAssetsChangeModel3.MOriginalAmount;
						model.MOrgPrepareForDecreaseAmount = fAFixAssetsChangeModel3.MPrepareForDecreaseAmount;
					}
					else
					{
						FAFixAssetsChangeModel fAFixAssetsChangeModel4 = (from m in list2
						orderby m.MIndex descending
						select m).FirstOrDefault();
						model.MOriginalAmount = fAFixAssetsChangeModel4.MOriginalAmount;
						model.MOrgPrepareForDecreaseAmount = fAFixAssetsChangeModel4.MPrepareForDecreaseAmount;
					}
				}
			}
		}

		public static List<RPTDepreciationDetailModel> GetDepreciationDetail(MContext context, RPTDepreciationDetailFilterModel filter)
		{
			string sql = GetSql(filter);
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLCID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStartPeroid", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MEndPeroid", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MPurchaseDate", MySqlDbType.VarChar, 36)
			};
			array[0].Value = context.MOrgID;
			array[1].Value = context.MLCID;
			array[2].Value = filter.MStartPeroid;
			array[3].Value = filter.MEndPeroid;
			array[4].Value = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture).AddMonths(1);
			return ModelInfoManager.GetDataModelBySql<RPTDepreciationDetailModel>(context, sql, array);
		}

		private static string GetSql(RPTDepreciationDetailFilterModel filter)
		{
			string str = " SELECT t1.MItemId,t1.MPurchaseDate,t3.MYearPeriod,t4.MName AS MFATypeIDName,concat(t1.MPrefix,t1.MNumber) as MFixAssetsNumber,\r\n                                      t1.MPrepareForDecreaseAmount,t1.MOriginalAmount,t1.MRateOfSalvage,t1.MUsefulPeriods,t3.MDepreciatedAmount,t1.MStatus,t1.MHandledDate,\r\n                                      t3.MNetAmount,t2.MName AS MFixAssetsName,t1.MDepCheckGroupValueID,t3.MPeriodDepreciatedAmount,t1.MDepreciatedAmountOfYear AS MOrgDepreciatedAmountOfYear,\r\n                                      t3.MDepreciatedAmountOfYear,t3.MVoucherID,t3.MIsAdjust,t1.MPrepareForDecreaseAmount AS MOrgPrepareForDecreaseAmount,\r\n                                      t1.MDepreciatedAmount AS MOrgDepreciatedAmount,t1.MDepreciationFromPeriod ";
			if (filter.IncludeCheckType)
			{
				str += " ,convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactNameExp,\r\n                                    F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeNameExp,\r\n                                    concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemNameExp,\r\n                                    t9.MName AS MExpItemNameExp,t10.MName AS MPaItemNameExp,\r\n                                    t10_0.MGroupID AS MPaItemGroupIDExp,t10_1.MName AS MPaItemGroupNameExp, t11.MName AS MTrackItem1NameExp,\r\n                                    t11_2.MName AS MTrackItem1GroupNameExp,t12.MName AS MTrackItem2NameExp,t12_2.MName AS MTrackItem2GroupNameExp,\r\n                                    t13.MName AS MTrackItem3NameExp,t13_2.MName AS MTrackItem3GroupNameExp,t14.MName AS MTrackItem4NameExp,\r\n                                    t14_2.MName AS MTrackItem4GroupNameExp,t15.MName AS MTrackItem5NameExp,t15_2.MName AS MTrackItem5GroupNameExp,\r\n\r\n                                    t100.MContactID AS MContactIDExp,t100.MEmployeeID AS MEmployeeIDExp,t100.MMerItemID AS MMerItemIDExp,\r\n                                    t100.MExpItemID AS MExpItemIDExp,t100.MPaItemID AS MPaItemIDExp,t100.MTrackItem1 AS MTrackItem1Exp,\r\n                                    t100.MTrackItem2 AS MTrackItem2Exp,t100.MTrackItem3 AS MTrackItem3Exp,t100.MTrackItem4 AS MTrackItem4Exp,\r\n                                    t100.MTrackItem5 AS MTrackItem5Exp,\r\n\r\n                                    t101.MContactID AS MContactIDDep,t101.MEmployeeID AS MEmployeeIDDep,t101.MMerItemID AS MMerItemIDDep,\r\n                                    t101.MExpItemID AS MExpItemIDDep,t101.MPaItemID AS MPaItemIDDep,t101.MTrackItem1 AS MTrackItem1Dep,\r\n                                    t101.MTrackItem2 AS MTrackItem2Dep,t101.MTrackItem3 AS MTrackItem3Dep,t101.MTrackItem4 AS MTrackItem4Dep,\r\n                                    t101.MTrackItem5 AS MTrackItem5Dep,\r\n\r\n\r\n                                    convert(AES_DECRYPT(t16.MName,'{0}') using utf8) as MContactNameDep,\r\n                                    F_GETUSERNAME(t17.MFirstName, t17.MLastName) AS MEmployeeNameDep,\r\n                                    concat(t18_0.MNumber,':',t18.MDesc) AS MMerItemNameDep,\r\n                                    t19.MName AS MExpItemNameDep,t20.MName AS MPaItemNameDep,\r\n                                    t20_0.MGroupID AS MPaItemGroupIDDep,t20_1.MName as MPaItemGroupNameDep,\r\n                                    t21.MName AS MTrackItem1NameDep,t21_2.MName AS MTrackItem1GroupNameDep,t22.MName AS MTrackItem2NameDep,\r\n                                    t22_2.MName AS MTrackItem2GroupNameDep,t23.MName AS MTrackItem3NameDep,t23_2.MName AS MTrackItem3GroupNameDep,\r\n                                    t24.MName AS MTrackItem4NameDep,t24_2.MName AS MTrackItem4GroupNameDep,t25.MName AS MTrackItem5NameDep,\r\n                                    t25_2.MName AS MTrackItem5GroupNameDep ";
			}
			str += "  FROM( SELECT * FROM t_fa_fixassets WHERE MOrgID = @MOrgID AND MPurchaseDate < @MPurchaseDate AND MIsDelete = 0 ) t1\r\n                              LEFT JOIN t_fa_depreciation t3 ON t3.MId = t1.MItemID\r\n                                AND t3.MOrgID = t1.MOrgID AND t3.MIsDelete = 0 \r\n                              LEFT JOIN t_fa_fixassets_l t2 ON t1.MItemID = t2.MParentID \r\n                                 AND t2.MIsDelete = 0 AND t2.MLocaleID = @MLCID\r\n                              LEFT JOIN t_fa_fixassetstype_l t4 ON t1.MFATypeID = t4.MParentID \r\n                                AND t4.MIsDelete = 0 AND t4.MLocaleID = @MLCID ";
			if (filter.IncludeCheckType)
			{
				str += " LEFT JOIN t_gl_checkgroupvalue t100 ON t100.MItemID = t1.MExpCheckGroupValueID\r\n                                        AND t100.MOrgID = t1.MOrgID AND t100.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t6 ON t6.MParentID = t100.MContactID \r\n                                        AND t6.MLocaleID = @MLCID AND t6.MOrgID = t1.MOrgID  AND t6.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_employees_l t7 ON t7.MParentID = t100.MEmployeeID \r\n                                        AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_item t8_0 ON t8_0.MItemID = t100.MMerItemID\r\n                                        AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                                     LEFT JOIN t_bd_item_l t8 ON t8.MParentID = t100.MMerItemID \r\n                                        AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_expenseitem_l t9 ON t9.MParentID = t100.MExpItemID \r\n                                        AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem_l t10 ON t10.MParentID = t100.MPaItemID \r\n                                        AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem t10_0 ON t10_0.MItemID = t100.MPaItemID \r\n                                        AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                                     LEFT JOIN t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t100.MPaItemID \r\n                                        AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_trackentry_l t11 ON t11.MParentID = t100.MTrackItem1 \r\n                                        AND t11.MLocaleID = @MLCID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t11_1 ON t11_1.MEntryID = t100.MTrackItem1 \r\n                                        AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t11_2 ON t11_2.MParentID = t11_1.MItemID \r\n                                        AND t11_2.MLocaleID = @MLCID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t12 ON t12.MParentID = t100.MTrackItem2 \r\n                                        AND t12.MLocaleID = @MLCID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t12_1 ON t12_1.MEntryID = t100.MTrackItem2 \r\n                                        AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t12_2 ON t12_2.MParentID = t12_1.MItemID \r\n                                        AND t12_2.MLocaleID = @MLCID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t13 ON t13.MParentID = t100.MTrackItem3 \r\n                                        AND t13.MLocaleID = @MLCID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t13_1 ON t13_1.MEntryID = t100.MTrackItem3 \r\n                                        AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t13_2 ON t13_2.MParentID = t13_1.MItemID \r\n                                        AND t13_2.MLocaleID = @MLCID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t14 ON t14.MParentID = t100.MTrackItem4\r\n                                        AND t14.MLocaleID = @MLCID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t14_1 ON t14_1.MEntryID = t100.MTrackItem4 \r\n                                        AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t14_2 ON t14_2.MParentID = t14_1.MItemID\r\n                                        AND t14_2.MLocaleID = @MLCID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t15 ON t15.MParentID = t100.MTrackItem5\r\n                                        AND t15.MLocaleID = @MLCID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t15_1 ON t15_1.MEntryID = t100.MTrackItem5 \r\n                                        AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t15_2 ON t15_2.MParentID = t15_1.MItemID \r\n                                        AND t15_2.MLocaleID = @MLCID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0\r\n\r\n                                     LEFT JOIN t_gl_checkgroupvalue t101 ON t101.MItemID = t1.MDepCheckGroupValueID\r\n                                        AND t101.MOrgID = t1.MOrgID AND t101.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t16 ON t16.MParentID = t101.MContactID \r\n                                        AND t16.MLocaleID = @MLCID AND t16.MOrgID = t1.MOrgID AND t16.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_employees_l t17 ON t17.MParentID = t101.MEmployeeID \r\n                                        AND t17.MOrgID = t1.MOrgId AND t17.MIsDelete = t1.MIsDelete AND t17.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_item t18_0 ON t18_0.MItemID = t101.MMerItemID\r\n                                        AND t18_0.MOrgID = t1.MOrgId AND t18_0.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_item_l t18 ON t18.MParentID = t101.MMerItemID \r\n                                        AND t18.MOrgID = t1.MOrgId AND t18.MIsDelete = t1.MIsDelete AND t18.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_expenseitem_l t19 ON t19.MParentID = t101.MExpItemID \r\n                                        AND t19.MOrgID = t1.MOrgId AND t19.MIsDelete = t1.MIsDelete AND t19.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem_l t20 ON t20.MParentID = t101.MPaItemID \r\n                                        AND t20.MOrgID = t1.MOrgId AND t20.MIsDelete = t1.MIsDelete AND t20.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem t20_0 ON t20_0.MItemID = t101.MPaItemID \r\n                                        AND t20_0.MOrgID = t1.MOrgId AND t20_0.MIsDelete = 0               \r\n                                    LEFT JOIN t_pa_payitemgroup_l t20_1 ON t20_1.MParentID = t101.MPaItemID\r\n                                        AND t20_1.MOrgID = t1.MOrgId AND t20_1.MIsDelete = 0 AND t20_1.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_trackentry_l t21 ON t21.MParentID = t101.MTrackItem1 \r\n                                        AND t21.MLocaleID = @MLCID AND t21.MOrgID = t1.MOrgID AND t21.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t21_1 ON t21_1.MEntryID = t101.MTrackItem1 \r\n                                        AND t21_1.MOrgID = t1.MOrgID AND t21_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t21_2 ON t21_2.MParentID = t21_1.MItemID \r\n                                        AND t21_2.MLocaleID = @MLCID AND t21_2.MOrgID = t1.MOrgID AND t21_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t22 ON t22.MParentID = t101.MTrackItem2 \r\n                                        AND t22.MLocaleID = @MLCID AND t22.MOrgID = t1.MOrgID AND t22.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t22_1 ON t22_1.MEntryID = t101.MTrackItem2 \r\n                                        AND t22_1.MOrgID = t1.MOrgID AND t22_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t22_2 ON t22_2.MParentID = t22_1.MItemID \r\n                                        AND t22_2.MLocaleID = @MLCID AND t22_2.MOrgID = t1.MOrgID AND t22_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t23 ON t23.MParentID = t101.MTrackItem3 \r\n                                        AND t23.MLocaleID = @MLCID AND t23.MOrgID = t1.MOrgID AND t23.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t23_1 ON t23_1.MEntryID = t101.MTrackItem3 \r\n                                        AND t23_1.MOrgID = t1.MOrgID AND t23_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t23_2 ON t23_2.MParentID = t23_1.MItemID \r\n                                        AND t23_2.MLocaleID = @MLCID AND t23_2.MOrgID = t1.MOrgID AND t23_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t24 ON t24.MParentID = t101.MTrackItem4\r\n                                        AND t24.MLocaleID = @MLCID AND t24.MOrgID = t1.MOrgID AND t24.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t24_1 ON t24_1.MEntryID = t101.MTrackItem4 \r\n                                        AND t24_1.MOrgID = t1.MOrgID AND t24_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t24_2 ON t24_2.MParentID = t24_1.MItemID \r\n                                        AND t24_2.MLocaleID = @MLCID AND t24_2.MOrgID = t1.MOrgID AND t24_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t25 ON t25.MParentID = t101.MTrackItem5 \r\n                                        AND t25.MLocaleID = @MLCID AND t25.MOrgID = t1.MOrgID AND t25.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t25_1 ON t25_1.MEntryID = t101.MTrackItem5 \r\n                                        AND t25_1.MOrgID = t1.MOrgID AND t25_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t25_2 ON t25_2.MParentID = t25_1.MItemID \r\n                                        AND t25_2.MLocaleID = @MLCID AND t25_2.MOrgID = t1.MOrgID AND t25_2.MIsDelete = 0 ";
			}
			return string.Format(str, "JieNor-001");
		}
	}
}
