using JieNor.Megi.Core;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.FA;
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
	public class RPTDepreciationSummaryRepository : RPTFABaseRepository
	{
		public static BizReportModel GetDepreciationSummaryData(MContext context, RPTDepreciationSummaryFilterModel filter)
		{
			BizReportModel bizReportModel = new BizReportModel
			{
				Type = Convert.ToInt32(BizReportType.DepreciationSummary)
			};
			SetTitle(context, bizReportModel, filter);
			SetRowHead(context, bizReportModel, filter);
			SetRowData(context, bizReportModel, filter);
			return bizReportModel;
		}

		private static void SetTitle(MContext context, BizReportModel model, RPTDepreciationSummaryFilterModel filter)
		{
			model.Title1 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "DereciationSummary", "折旧汇总表");
			model.Title2 = context.MOrgName;
			model.Title3 = COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "ForThePeriod", "For the period ") + " " + filter.MStartPeroid + " " + COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "To", "to") + " " + filter.MEndPeroid;
		}

		private static void SetRowHead(MContext context, BizReportModel model, RPTDepreciationSummaryFilterModel filter)
		{
			BizReportRowModel bizReportRowModel = new BizReportRowModel();
			bizReportRowModel.RowType = BizReportRowType.Header;
			bizReportRowModel.AddCell(new BizReportCellModel
			{
				Value = COMMultiLangRepository.GetText(context.MLCID, LangModule.FA, "FixAssetsTypeOptions", "资产类别"),
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

		private static void PrehandleDepreciationList(MContext ctx, List<RPTDepreciationSummaryModel> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				RPTDepreciationSummaryModel rPTDepreciationSummaryModel = list[i];
				if (string.IsNullOrWhiteSpace(rPTDepreciationSummaryModel.MDepreciationID))
				{
					rPTDepreciationSummaryModel.MPeriodDepreciatedAmount = decimal.Zero;
					rPTDepreciationSummaryModel.MDepreciatedAmountOfYear = rPTDepreciationSummaryModel.MOrgDepreciatedAmountOfYear;
					rPTDepreciationSummaryModel.MDepreciatedAmount = rPTDepreciationSummaryModel.MOrgDepreciatedAmount;
					RPTDepreciationSummaryModel rPTDepreciationSummaryModel2 = rPTDepreciationSummaryModel;
					DateTime mFABeginDate = ctx.MFABeginDate;
					int num = mFABeginDate.Year * 100;
					mFABeginDate = ctx.MFABeginDate;
					rPTDepreciationSummaryModel2.MYearPeriod = num + mFABeginDate.Month;
				}
			}
		}

		private static void SetRowData(MContext context, BizReportModel model, RPTDepreciationSummaryFilterModel filter)
		{
			List<RPTDepreciationSummaryModel> list = GetDepreciationDetail(context, filter);
			PrehandleDepreciationList(context, list);
			List<FAFixAssetsChangeModel> fAChangeList = RPTFABaseRepository.GetFAChangeList(context);
			if (filter.IncludeCheckType && filter.CheckTypeValueList != null && filter.CheckTypeValueList.Count > 0)
			{
				foreach (NameValueModel checkTypeValue in filter.CheckTypeValueList)
				{
					switch (checkTypeValue.MName)
					{
					case "0":
						list = (from m in list
						where m.MContactIDDep == checkTypeValue.MValue || m.MContactIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "1":
						list = (from m in list
						where m.MEmployeeIDDep == checkTypeValue.MValue || m.MEmployeeIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "2":
						list = (from m in list
						where m.MMerItemIDDep == checkTypeValue.MValue || m.MMerItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "3":
						list = (from m in list
						where m.MExpItemIDDep == checkTypeValue.MValue || m.MExpItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "4":
						list = (from m in list
						where m.MPaItemIDDep == checkTypeValue.MValue || m.MPaItemIDExp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "5":
						list = (from m in list
						where m.MTrackItem1Dep == checkTypeValue.MValue || m.MTrackItem1Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "6":
						list = (from m in list
						where m.MTrackItem2Dep == checkTypeValue.MValue || m.MTrackItem2Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "7":
						list = (from m in list
						where m.MTrackItem3Dep == checkTypeValue.MValue || m.MTrackItem3Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "8":
						list = (from m in list
						where m.MTrackItem4Dep == checkTypeValue.MValue || m.MTrackItem4Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					case "9":
						list = (from m in list
						where m.MTrackItem5Dep == checkTypeValue.MValue || m.MTrackItem5Exp == checkTypeValue.MValue
						select m).ToList();
						break;
					}
				}
			}
			DateTime dateTime = DateTime.ParseExact(filter.MStartPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			DateTime t = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
			List<RPTDepreciationSummaryModel> list2 = new List<RPTDepreciationSummaryModel>();
			list2 = ((!filter.IncludeCheckType) ? (from p in list
			group p by new
			{
				p.MFATypeID
			} into g
			select new RPTDepreciationSummaryModel
			{
				MFATypeID = g.Key.MFATypeID
			}).ToList() : (from p in list
			group p by new
			{
				p.MFATypeID,
				p.MDepCheckGroupValueID,
				p.MExpCheckGroupValueID
			} into g
			select new RPTDepreciationSummaryModel
			{
				MFATypeID = g.Key.MFATypeID,
				MDepCheckGroupValueID = g.Key.MDepCheckGroupValueID,
				MExpCheckGroupValueID = g.Key.MExpCheckGroupValueID
			}).ToList());
			List<RPTDepreciationSummaryModel> list3 = new List<RPTDepreciationSummaryModel>();
			Dictionary<int, List<decimal>> dictionary = new Dictionary<int, List<decimal>>();
			foreach (RPTDepreciationSummaryModel item3 in list2)
			{
				List<RPTDepreciationSummaryModel> list4 = new List<RPTDepreciationSummaryModel>();
				list4 = ((!filter.IncludeCheckType) ? (from m in list
				where m.MFATypeID == item3.MFATypeID
				orderby m.MYearPeriod descending
				select m).ToList() : (from m in list
				where m.MFATypeID == item3.MFATypeID && m.MDepCheckGroupValueID == item3.MDepCheckGroupValueID && m.MExpCheckGroupValueID == item3.MExpCheckGroupValueID
				orderby m.MYearPeriod descending
				select m).ToList());
				RPTDepreciationSummaryModel rPTDepreciationSummaryModel = new RPTDepreciationSummaryModel();
				List<RPTDepreciationSummaryModel> list5 = (from p in list4
				group p by new
				{
					p.MitemId
				} into g
				select new RPTDepreciationSummaryModel
				{
					MitemId = g.Key.MitemId
				}).ToList();
				DateTime startDate = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture);
				DateTime dateTime2 = DateTime.ParseExact(filter.MEndPeroid + "01", "yyyyMMdd", CultureInfo.CurrentCulture).AddMonths(1);
				foreach (RPTDepreciationSummaryModel item4 in list4)
				{
					GetValueChangeItems(fAChangeList, item4, dateTime2, startDate);
				}
				List<RPTDepreciationSummaryModel> list6 = new List<RPTDepreciationSummaryModel>();
				foreach (RPTDepreciationSummaryModel item5 in list5)
				{
					List<RPTDepreciationSummaryModel> source = (from m in list4
					where m.MitemId == item5.MitemId && m.MYearPeriod <= Convert.ToInt32(filter.MEndPeroid) && m.MYearPeriod > 0
					select m).ToList();
					RPTDepreciationSummaryModel rPTDepreciationSummaryModel2 = list4.FirstOrDefault((RPTDepreciationSummaryModel m) => m.MitemId == item5.MitemId);
					RPTDepreciationSummaryModel firstItem = source.FirstOrDefault();
					if (firstItem == null)
					{
						if (context.MFABeginDate > t)
						{
							rPTDepreciationSummaryModel2.MDepreciatedAmount = decimal.Zero;
							rPTDepreciationSummaryModel2.MDepreciatedAmountOfYear = decimal.Zero;
							rPTDepreciationSummaryModel2.MOrgPrepareForDecreaseAmount = decimal.Zero;
						}
						else
						{
							rPTDepreciationSummaryModel2.MDepreciatedAmount = decimal.Zero;
							rPTDepreciationSummaryModel2.MDepreciatedAmount = rPTDepreciationSummaryModel2.MOrgDepreciatedAmount;
							rPTDepreciationSummaryModel2.MDepreciatedAmountOfYear = rPTDepreciationSummaryModel2.MOrgDepreciatedAmountOfYear;
							rPTDepreciationSummaryModel2.MPrepareForDecreaseAmount = rPTDepreciationSummaryModel2.MOrgPrepareForDecreaseAmount;
						}
					}
					else
					{
						List<RPTDepreciationSummaryModel> source2 = (from m in source
						where m.MYearPeriod == firstItem.MYearPeriod
						select m).ToList();
						rPTDepreciationSummaryModel2.MDepreciatedAmount = source2.Sum((RPTDepreciationSummaryModel m) => m.MDepreciatedAmount);
						int year = t.Year;
						int num = firstItem.MYearPeriod / 100;
						rPTDepreciationSummaryModel2.MDepreciatedAmountOfYear = ((year != num) ? decimal.Zero : source2.Sum((RPTDepreciationSummaryModel m) => m.MDepreciatedAmountOfYear));
					}
					if (rPTDepreciationSummaryModel2.MStatus != 0 && rPTDepreciationSummaryModel2.MHandledDate < dateTime2)
					{
						rPTDepreciationSummaryModel2.MOriginalAmount = decimal.Zero;
						rPTDepreciationSummaryModel2.MDepreciatedAmount = decimal.Zero;
						rPTDepreciationSummaryModel2.MOrgPrepareForDecreaseAmount = decimal.Zero;
					}
					list6.Add(rPTDepreciationSummaryModel2);
				}
				RPTDepreciationSummaryModel rPTDepreciationSummaryModel3 = list6.FirstOrDefault();
				decimal num2 = list6.Sum((RPTDepreciationSummaryModel m) => m.MOriginalAmount);
				decimal num3 = list6.Sum((RPTDepreciationSummaryModel m) => m.MDepreciatedAmount);
				decimal num4 = list6.Sum((RPTDepreciationSummaryModel m) => m.MDepreciatedAmountOfYear);
				decimal num5 = list6.Sum((RPTDepreciationSummaryModel m) => m.MOrgPrepareForDecreaseAmount);
				decimal num6 = default(decimal);
				BizReportRowModel bizReportRowModel = new BizReportRowModel();
				bizReportRowModel.RowType = BizReportRowType.Item;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(rPTDepreciationSummaryModel3.MFATypeIDName),
					CellType = BizReportCellType.Text
				});
				if (filter.IncludeCheckType)
				{
					bizReportRowModel.AddCell(new BizReportCellModel
					{
						Value = Convert.ToString(RPTBaseREpository.GetCheckGroupValue(context, rPTDepreciationSummaryModel3)),
						CellType = BizReportCellType.Text
					});
				}
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(num2),
					CellType = BizReportCellType.Money
				});
				int num7 = (t.Year - dateTime.Year) * 12 + (t.Month - dateTime.Month);
				DateTime dateTime3 = dateTime;
				for (int i = 0; i <= num7; i++)
				{
					DateTime peroid = dateTime3;
					IEnumerable<RPTDepreciationSummaryModel> source3 = from m in list4
					where m.MYearPeriod == peroid.Year * 100 + peroid.Month && !string.IsNullOrWhiteSpace(m.MVoucherID)
					select m;
					decimal item2 = source3.Sum((RPTDepreciationSummaryModel m) => m.MPeriodDepreciatedAmount);
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
						List<decimal> list7 = dictionary[i];
						list7.Add(item2);
					}
					dateTime3 = dateTime3.AddMonths(1);
				}
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
				num6 = num2 - num3 - num5;
				bizReportRowModel.AddCell(new BizReportCellModel
				{
					Value = Convert.ToString(num6),
					CellType = BizReportCellType.Money
				});
				rPTDepreciationSummaryModel.MOriginalAmount = num2;
				rPTDepreciationSummaryModel.MDepreciatedAmount = num3;
				rPTDepreciationSummaryModel.MDepreciatedAmountOfYear = num4;
				rPTDepreciationSummaryModel.MPrepareForDecreaseAmount = num5;
				rPTDepreciationSummaryModel.MNetAmount = num6;
				list3.Add(rPTDepreciationSummaryModel);
				model.AddRow(bizReportRowModel);
			}
			BizReportRowModel bizReportRowModel2 = new BizReportRowModel();
			bizReportRowModel2.RowType = BizReportRowType.Item;
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(COMMultiLangRepository.GetText(context.MLCID, LangModule.Report, "Total", "Total")),
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
				Value = Convert.ToString(list3.Sum((RPTDepreciationSummaryModel m) => m.MOriginalAmount)),
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
				Value = Convert.ToString(list3.Sum((RPTDepreciationSummaryModel m) => m.MDepreciatedAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list3.Sum((RPTDepreciationSummaryModel m) => m.MDepreciatedAmountOfYear)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list3.Sum((RPTDepreciationSummaryModel m) => m.MPrepareForDecreaseAmount)),
				CellType = BizReportCellType.Money
			});
			bizReportRowModel2.AddCell(new BizReportCellModel
			{
				Value = Convert.ToString(list3.Sum((RPTDepreciationSummaryModel m) => m.MNetAmount)),
				CellType = BizReportCellType.Money
			});
			model.AddRow(bizReportRowModel2);
		}

		private static void GetValueChangeItems(List<FAFixAssetsChangeModel> list, RPTDepreciationSummaryModel model, DateTime endDate, DateTime startDate)
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

		private static List<RPTDepreciationSummaryModel> GetDepreciationDetail(MContext context, RPTDepreciationSummaryFilterModel filter)
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
			return ModelInfoManager.GetDataModelBySql<RPTDepreciationSummaryModel>(context, sql, array);
		}

		private static string GetSql(RPTDepreciationSummaryFilterModel filter)
		{
			string str = " SELECT \n                                t1.MFATypeID,\n                                t1.MItemId,\n                                t1.MPurchaseDate,\n                                t1.MNumber,\n                                t3.MYearPeriod,\n                                t4.MName AS MFATypeIDName,\n                                CONCAT(t1.MPrefix, t1.MNumber) AS MFixAssetsNumber,\n                                t1.MPrepareForDecreaseAmount,\n                                t1.MOriginalAmount,\n                                t1.MRateOfSalvage,\n                                t1.MUsefulPeriods,\n                                t3.MItemID as MDepreciationID,\n                                t3.MDepreciatedAmount,\n                                t1.MStatus,\n                                t1.MHandledDate,\n                                t3.MNetAmount,\n                                t2.MName AS MFixAssetsName,\n                                t1.MDepCheckGroupValueID,\n                                t1.MExpCheckGroupValueID,\n                                t3.MPeriodDepreciatedAmount,\n                                t1.MDepreciatedAmount as MOrgMDepreciatedAmount,\n                                t1.MDepreciatedAmountOfYear AS MOrgDepreciatedAmountOfYear,\n                                t3.MDepreciatedAmountOfYear,\n                                t3.MVoucherID,\n                                t3.MIsAdjust,\n                                t1.MPrepareForDecreaseAmount AS MOrgPrepareForDecreaseAmount,\n                                t1.MDepreciatedAmount AS MOrgDepreciatedAmount,\n                                t1.MDepreciationFromPeriod";
			if (filter.IncludeCheckType)
			{
				str += " ,convert(AES_DECRYPT(t6.MName,'{0}') using utf8) as MContactNameExp,\r\n                                    F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeNameExp,\r\n                                    concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemNameExp,\r\n                                    t9.MName AS MExpItemNameExp,t10.MName AS MPaItemNameExp,\r\n                                    t10_0.MGroupID AS MPaItemGroupIDExp,t10_1.MName AS MPaItemGroupNameExp, t11.MName AS MTrackItem1NameExp,\r\n                                    t11_2.MName AS MTrackItem1GroupNameExp,t12.MName AS MTrackItem2NameExp,t12_2.MName AS MTrackItem2GroupNameExp,\r\n                                    t13.MName AS MTrackItem3NameExp,t13_2.MName AS MTrackItem3GroupNameExp,t14.MName AS MTrackItem4NameExp,\r\n                                    t14_2.MName AS MTrackItem4GroupNameExp,t15.MName AS MTrackItem5NameExp,t15_2.MName AS MTrackItem5GroupNameExp,\r\n\r\n                                    t100.MContactID AS MContactIDExp,t100.MEmployeeID AS MEmployeeIDExp,t100.MMerItemID AS MMerItemIDExp,\r\n                                    t100.MExpItemID AS MExpItemIDExp,t100.MPaItemID AS MPaItemIDExp,t100.MTrackItem1 AS MTrackItem1Exp,\r\n                                    t100.MTrackItem2 AS MTrackItem2Exp,t100.MTrackItem3 AS MTrackItem3Exp,t100.MTrackItem4 AS MTrackItem4Exp,\r\n                                    t100.MTrackItem5 AS MTrackItem5Exp,\r\n\r\n                                    t101.MContactID AS MContactIDDep,t101.MEmployeeID AS MEmployeeIDDep,t101.MMerItemID AS MMerItemIDDep,\r\n                                    t101.MExpItemID AS MExpItemIDDep,t101.MPaItemID AS MPaItemIDDep,t101.MTrackItem1 AS MTrackItem1Dep,\r\n                                    t101.MTrackItem2 AS MTrackItem2Dep,t101.MTrackItem3 AS MTrackItem3Dep,t101.MTrackItem4 AS MTrackItem4Dep,\r\n                                    t101.MTrackItem5 AS MTrackItem5Dep,\r\n\r\n\r\n                                    convert(AES_DECRYPT(t16.MName,'{0}') using utf8) as MContactNameDep,\r\n                                    F_GETUSERNAME(t17.MFirstName, t17.MLastName) AS MEmployeeNameDep,\r\n                                    concat(t18_0.MNumber,':',t18.MDesc) AS MMerItemNameDep,\r\n                                    t19.MName AS MExpItemNameDep,t20.MName AS MPaItemNameDep,\r\n                                    t20_0.MGroupID AS MPaItemGroupIDDep,t20_1.MName as MPaItemGroupNameDep,\r\n                                    t21.MName AS MTrackItem1NameDep,t21_2.MName AS MTrackItem1GroupNameDep,t22.MName AS MTrackItem2NameDep,\r\n                                    t22_2.MName AS MTrackItem2GroupNameDep,t23.MName AS MTrackItem3NameDep,t23_2.MName AS MTrackItem3GroupNameDep,\r\n                                    t24.MName AS MTrackItem4NameDep,t24_2.MName AS MTrackItem4GroupNameDep,t25.MName AS MTrackItem5NameDep,\r\n                                    t25_2.MName AS MTrackItem5GroupNameDep ";
			}
			str += "  FROM( SELECT * FROM t_fa_fixassets WHERE MOrgID = @MOrgID AND MPurchaseDate < @MPurchaseDate AND MIsDelete = 0 ) t1\r\n                              LEFT JOIN t_fa_depreciation t3 ON t3.MId = t1.MItemID\r\n                                AND t3.MOrgID = t1.MOrgID  AND t3.MIsDelete = 0\r\n                              LEFT JOIN t_fa_fixassets_l t2 ON t1.MItemID = t2.MParentID \r\n                                 AND t2.MIsDelete = 0 AND t2.MLocaleID = @MLCID\r\n                              LEFT JOIN t_fa_fixassetstype_l t4 ON t1.MFATypeID = t4.MParentID \r\n                                AND t4.MIsDelete = 0 AND t4.MLocaleID = @MLCID ";
			if (filter.IncludeCheckType)
			{
				str += " LEFT JOIN t_gl_checkgroupvalue t100 ON t100.MItemID = t1.MExpCheckGroupValueID\r\n                                        AND t100.MOrgID = t1.MOrgID AND t100.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t6 ON t6.MParentID = t100.MContactID \r\n                                        AND t6.MLocaleID = @MLCID AND t6.MOrgID = t1.MOrgID  AND t6.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_employees_l t7 ON t7.MParentID = t100.MEmployeeID \r\n                                        AND t7.MOrgID = t1.MOrgId AND t7.MIsDelete = t1.MIsDelete AND t7.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_item t8_0 ON t8_0.MItemID = t100.MMerItemID\r\n                                        AND t8_0.MOrgID = t1.MOrgId AND t8_0.MIsDelete = t1.MIsDelete\r\n                                     LEFT JOIN t_bd_item_l t8 ON t8.MParentID = t100.MMerItemID \r\n                                        AND t8.MOrgID = t1.MOrgId AND t8.MIsDelete = t1.MIsDelete AND t8.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_expenseitem_l t9 ON t9.MParentID = t100.MExpItemID \r\n                                        AND t9.MOrgID = t1.MOrgId AND t9.MIsDelete = t1.MIsDelete AND t9.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem_l t10 ON t10.MParentID = t100.MPaItemID \r\n                                        AND t10.MOrgID = t1.MOrgId AND t10.MIsDelete = t1.MIsDelete AND t10.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_pa_payitem t10_0 ON t10_0.MItemID = t100.MPaItemID \r\n                                        AND t10_0.MOrgID = t1.MOrgId AND t10_0.MIsDelete = t1.MIsDelete \r\n                                     LEFT JOIN t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t100.MPaItemID \r\n                                        AND t10_1.MOrgID = t1.MOrgId AND t10_1.MIsDelete = t1.MIsDelete AND t10_1.MLocaleID = @MLCID\r\n                                     LEFT JOIN t_bd_trackentry_l t11 ON t11.MParentID = t100.MTrackItem1 \r\n                                        AND t11.MLocaleID = @MLCID AND t11.MOrgID = t1.MOrgID AND t11.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t11_1 ON t11_1.MEntryID = t100.MTrackItem1 \r\n                                        AND t11_1.MOrgID = t1.MOrgID AND t11_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t11_2 ON t11_2.MParentID = t11_1.MItemID \r\n                                        AND t11_2.MLocaleID = @MLCID AND t11_2.MOrgID = t1.MOrgID AND t11_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t12 ON t12.MParentID = t100.MTrackItem2 \r\n                                        AND t12.MLocaleID = @MLCID AND t12.MOrgID = t1.MOrgID AND t12.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t12_1 ON t12_1.MEntryID = t100.MTrackItem2 \r\n                                        AND t12_1.MOrgID = t1.MOrgID AND t12_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t12_2 ON t12_2.MParentID = t12_1.MItemID \r\n                                        AND t12_2.MLocaleID = @MLCID AND t12_2.MOrgID = t1.MOrgID AND t12_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t13 ON t13.MParentID = t100.MTrackItem3 \r\n                                        AND t13.MLocaleID = @MLCID AND t13.MOrgID = t1.MOrgID AND t13.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t13_1 ON t13_1.MEntryID = t100.MTrackItem3 \r\n                                        AND t13_1.MOrgID = t1.MOrgID AND t13_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t13_2 ON t13_2.MParentID = t13_1.MItemID \r\n                                        AND t13_2.MLocaleID = @MLCID AND t13_2.MOrgID = t1.MOrgID AND t13_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t14 ON t14.MParentID = t100.MTrackItem4\r\n                                        AND t14.MLocaleID = @MLCID AND t14.MOrgID = t1.MOrgID AND t14.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t14_1 ON t14_1.MEntryID = t100.MTrackItem4 \r\n                                        AND t14_1.MOrgID = t1.MOrgID AND t14_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t14_2 ON t14_2.MParentID = t14_1.MItemID\r\n                                        AND t14_2.MLocaleID = @MLCID AND t14_2.MOrgID = t1.MOrgID AND t14_2.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry_l t15 ON t15.MParentID = t100.MTrackItem5\r\n                                        AND t15.MLocaleID = @MLCID AND t15.MOrgID = t1.MOrgID AND t15.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_trackentry t15_1 ON t15_1.MEntryID = t100.MTrackItem5 \r\n                                        AND t15_1.MOrgID = t1.MOrgID AND t15_1.MIsDelete = 0\r\n                                     LEFT JOIN t_bd_track_l t15_2 ON t15_2.MParentID = t15_1.MItemID \r\n                                        AND t15_2.MLocaleID = @MLCID AND t15_2.MOrgID = t1.MOrgID AND t15_2.MIsDelete = 0\r\n\r\n                                     LEFT JOIN t_gl_checkgroupvalue t101 ON t101.MItemID = t1.MDepCheckGroupValueID\r\n                                        AND t101.MOrgID = t1.MOrgID AND t101.MIsDelete = 0 \r\n                                     LEFT JOIN t_bd_contacts_l t16 ON t16.MParentID = t101.MContactID \r\n                                        AND t16.MLocaleID = @MLCID AND t16.MOrgID = t1.MOrgID AND t16.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_employees_l t17 ON t17.MParentID = t101.MEmployeeID \r\n                                        AND t17.MOrgID = t1.MOrgId AND t17.MIsDelete = t1.MIsDelete AND t17.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_item t18_0 ON t18_0.MItemID = t101.MMerItemID\r\n                                        AND t18_0.MOrgID = t1.MOrgId AND t18_0.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_item_l t18 ON t18.MParentID = t101.MMerItemID \r\n                                        AND t18.MOrgID = t1.MOrgId AND t18.MIsDelete = t1.MIsDelete AND t18.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_expenseitem_l t19 ON t19.MParentID = t101.MExpItemID \r\n                                        AND t19.MOrgID = t1.MOrgId AND t19.MIsDelete = t1.MIsDelete AND t19.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem_l t20 ON t20.MParentID = t101.MPaItemID \r\n                                        AND t20.MOrgID = t1.MOrgId AND t20.MIsDelete = t1.MIsDelete AND t20.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_pa_payitem t20_0 ON t20_0.MItemID = t101.MPaItemID \r\n                                        AND t20_0.MOrgID = t1.MOrgId AND t20_0.MIsDelete = 0               \r\n                                    LEFT JOIN t_pa_payitemgroup_l t20_1 ON t20_1.MParentID = t101.MPaItemID\r\n                                        AND t20_1.MOrgID = t1.MOrgId AND t20_1.MIsDelete = 0 AND t20_1.MLocaleID = @MLCID\r\n                                    LEFT JOIN t_bd_trackentry_l t21 ON t21.MParentID = t101.MTrackItem1 \r\n                                        AND t21.MLocaleID = @MLCID AND t21.MOrgID = t1.MOrgID AND t21.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t21_1 ON t21_1.MEntryID = t101.MTrackItem1 \r\n                                        AND t21_1.MOrgID = t1.MOrgID AND t21_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t21_2 ON t21_2.MParentID = t21_1.MItemID \r\n                                        AND t21_2.MLocaleID = @MLCID AND t21_2.MOrgID = t1.MOrgID AND t21_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t22 ON t22.MParentID = t101.MTrackItem2 \r\n                                        AND t22.MLocaleID = @MLCID AND t22.MOrgID = t1.MOrgID AND t22.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t22_1 ON t22_1.MEntryID = t101.MTrackItem2 \r\n                                        AND t22_1.MOrgID = t1.MOrgID AND t22_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t22_2 ON t22_2.MParentID = t22_1.MItemID \r\n                                        AND t22_2.MLocaleID = @MLCID AND t22_2.MOrgID = t1.MOrgID AND t22_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t23 ON t23.MParentID = t101.MTrackItem3 \r\n                                        AND t23.MLocaleID = @MLCID AND t23.MOrgID = t1.MOrgID AND t23.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t23_1 ON t23_1.MEntryID = t101.MTrackItem3 \r\n                                        AND t23_1.MOrgID = t1.MOrgID AND t23_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t23_2 ON t23_2.MParentID = t23_1.MItemID \r\n                                        AND t23_2.MLocaleID = @MLCID AND t23_2.MOrgID = t1.MOrgID AND t23_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t24 ON t24.MParentID = t101.MTrackItem4\r\n                                        AND t24.MLocaleID = @MLCID AND t24.MOrgID = t1.MOrgID AND t24.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t24_1 ON t24_1.MEntryID = t101.MTrackItem4 \r\n                                        AND t24_1.MOrgID = t1.MOrgID AND t24_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t24_2 ON t24_2.MParentID = t24_1.MItemID \r\n                                        AND t24_2.MLocaleID = @MLCID AND t24_2.MOrgID = t1.MOrgID AND t24_2.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry_l t25 ON t25.MParentID = t101.MTrackItem5 \r\n                                        AND t25.MLocaleID = @MLCID AND t25.MOrgID = t1.MOrgID AND t25.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_trackentry t25_1 ON t25_1.MEntryID = t101.MTrackItem5 \r\n                                        AND t25_1.MOrgID = t1.MOrgID AND t25_1.MIsDelete = 0\r\n                                    LEFT JOIN t_bd_track_l t25_2 ON t25_2.MParentID = t25_1.MItemID \r\n                                        AND t25_2.MLocaleID = @MLCID AND t25_2.MOrgID = t1.MOrgID AND t25_2.MIsDelete = 0 ";
			}
			return string.Format(str, "JieNor-001");
		}
	}
}
