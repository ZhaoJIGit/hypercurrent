using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.FA
{
    public class FADepreciationRepository : DataServiceT<FADepreciationModel>
    {
        public int GetSummaryDepreciationPageCount(MContext ctx, FAFixAssetsFilterModel filter)
        {
            string summaryQuerySql = GetSummaryQuerySql(filter, true);
            MySqlParameter[] parameter = GetParameter(ctx, filter);
            object single = new DynamicDbHelperMySQL(ctx).GetSingle(summaryQuerySql, parameter);
            int result = 0;
            int.TryParse(single.ToString(), out result);
            return result;
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

        public List<FADepreciationModel> GetDetailDepreciationList(MContext ctx, FAFixAssetsFilterModel filter)
        {
            string detailQuerySql = GetDetailQuerySql(filter);
            MySqlParameter[] parameter = GetParameter(ctx, filter);
            List<FADepreciationModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<FADepreciationModel>(ctx, detailQuerySql, parameter);
            if (dataModelBySql.Count == 0)
            {
                return dataModelBySql;
            }
            if (filter.Period == 1 && dataModelBySql.Exists((FADepreciationModel x) => !string.IsNullOrWhiteSpace(x.MItemID)))
            {
                dataModelBySql.ForEach(delegate (FADepreciationModel x)
                {
                    if (string.IsNullOrWhiteSpace(x.MItemID) || string.IsNullOrWhiteSpace(x.MVoucherID))
                    {
                        x.MDepreciatedAmountOfYear = decimal.Zero;
                    }
                });
            }

            List<string> fixIDList = (from x in dataModelBySql
                                      select x.MID).Distinct().ToList();
            List<FAFixAssetsChangeModel> fixAssetsChangeList = new FAFixAssetsChangeRepository().GetFixAssetsChangeList(ctx, fixIDList);
            for (int i = 0; i < fixIDList.Count; i++)
            {
                List<FAFixAssetsChangeModel> list = (from x in fixAssetsChangeList
                                                     where x.MID == fixIDList[i]
                                                     select x).ToList();
                if (list != null && list.Count != 0)
                {
                    int maxIndex = list.Max((FAFixAssetsChangeModel x) => x.MIndex);
                    if (maxIndex != 0)
                    {
                        FAFixAssetsChangeModel fAFixAssetsChangeModel = list.FirstOrDefault((FAFixAssetsChangeModel x) => x.MIndex == maxIndex);
                        DateTime mChangeFromPeriod = fAFixAssetsChangeModel.MChangeFromPeriod;
                        int num = mChangeFromPeriod.Year * 12;
                        mChangeFromPeriod = fAFixAssetsChangeModel.MChangeFromPeriod;
                        if (num + mChangeFromPeriod.Month > filter.Year * 12 + filter.Period)
                        {
                            list = (from x in list.Where(delegate (FAFixAssetsChangeModel x)
                            {
                                DateTime mChangeFromPeriod2 = x.MChangeFromPeriod;
                                int num2 = mChangeFromPeriod2.Year * 12;
                                mChangeFromPeriod2 = x.MChangeFromPeriod;
                                return num2 + mChangeFromPeriod2.Month <= filter.Year * 12 + filter.Period;
                            })
                                    orderby x.MIndex descending
                                    select x).ToList();
                            FAFixAssetsChangeModel first = list.First();
                            (from x in dataModelBySql
                             where x.MID == first.MID
                             select x).ToList().ForEach(delegate (FADepreciationModel x)
                             {
                                 x.MOriginalAmount = first.MOriginalAmount;
                                 x.MPrepareForDecreaseAmount = first.MPrepareForDecreaseAmount;
                             });
                        }
                    }
                }
            }

            if (dataModelBySql.Exists((FADepreciationModel x) => !string.IsNullOrWhiteSpace(x.MVoucherID)))
            {
                return dataModelBySql;
            }
            return Calculate(ctx, filter, dataModelBySql);
        }

        public List<FADepreciationModel> GetSummaryDepreciationList(MContext ctx, FAFixAssetsFilterModel filter)
        {
            string summaryQuerySql = GetSummaryQuerySql(filter, false);
            MySqlParameter[] parameter = GetParameter(ctx, filter);
            List<FADepreciationModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<FADepreciationModel>(ctx, summaryQuerySql, parameter);
            if (dataModelBySql.Count == 0)
            {
                return dataModelBySql;
            }
            if (filter.Period == 1 && dataModelBySql.Exists((FADepreciationModel x) => !string.IsNullOrWhiteSpace(x.MItemID)))
            {
                dataModelBySql.ForEach(delegate (FADepreciationModel x)
                {
                    if (string.IsNullOrWhiteSpace(x.MItemID) || string.IsNullOrWhiteSpace(x.MVoucherID))
                    {
                        x.MDepreciatedAmountOfYear = decimal.Zero;
                    }
                });
            }
            List<string> fixIDList = (from x in dataModelBySql
                                      select x.MID).Distinct().ToList();
            List<FAFixAssetsChangeModel> fixAssetsChangeList = new FAFixAssetsChangeRepository().GetFixAssetsChangeList(ctx, fixIDList);
            int i;
            for (i = 0; i < fixIDList.Count; i++)
            {
                List<FAFixAssetsChangeModel> list = (from x in fixAssetsChangeList
                                                     where x.MID == fixIDList[i]
                                                     select x).ToList();
                if (list != null && list.Count != 0)
                {
                    int maxIndex = list.Max((FAFixAssetsChangeModel x) => x.MIndex);
                    if (maxIndex != 0)
                    {
                        FAFixAssetsChangeModel fAFixAssetsChangeModel = list.FirstOrDefault((FAFixAssetsChangeModel x) => x.MIndex == maxIndex);
                        DateTime mChangeFromPeriod = fAFixAssetsChangeModel.MChangeFromPeriod;
                        int num = mChangeFromPeriod.Year * 12;
                        mChangeFromPeriod = fAFixAssetsChangeModel.MChangeFromPeriod;
                        if (num + mChangeFromPeriod.Month > filter.Year * 12 + filter.Period)
                        {
                            list = (from x in list.Where(delegate (FAFixAssetsChangeModel x)
                            {
                                DateTime mChangeFromPeriod2 = x.MChangeFromPeriod;
                                int num2 = mChangeFromPeriod2.Year * 12;
                                mChangeFromPeriod2 = x.MChangeFromPeriod;
                                return num2 + mChangeFromPeriod2.Month <= filter.Year * 12 + filter.Period;
                            })
                                    orderby x.MIndex descending
                                    select x).ToList();
                            FAFixAssetsChangeModel first = list.First();
                            (from x in dataModelBySql
                             where x.MID == first.MID
                             select x).ToList().ForEach(delegate (FADepreciationModel x)
                             {
                                 x.MOriginalAmount = first.MOriginalAmount;
                                 x.MPrepareForDecreaseAmount = first.MPrepareForDecreaseAmount;
                             });
                        }
                    }
                }
            }
            return dataModelBySql;
        }

        public FAFixAssetsChangeModel GetAvaliableDepreciation(MContext ctx, FAFixAssetsModel fixAsset, List<FAFixAssetsChangeModel> changeList, int year, int period)
        {
            if (!fixAsset.MChanged || changeList == null || changeList.Count < 1)
            {
                return null;
            }
            FAFixAssetsChangeModel result = null;
            for (int i = 0; i < changeList.Count; i++)
            {
                DateTime mChangeFromPeriod = changeList[i].MChangeFromPeriod;
                int num = mChangeFromPeriod.Year * 12;
                mChangeFromPeriod = changeList[i].MChangeFromPeriod;
                if (num + mChangeFromPeriod.Month == year * 12 + period)
                {
                    result = changeList[i];
                    break;
                }
            }
            return result;
        }

        public List<FADepreciationModel> Calculate(MContext ctx, FAFixAssetsFilterModel filter, List<FADepreciationModel> list)
        {
            List<FAFixAssetsModel> fixAssetsList = new FAFixAssetsRepository().GetFixAssetsList(ctx, new FAFixAssetsFilterModel
            {
                Status = -1
            });
            List<FAFixAssetsChangeModel> changes = new FAFixAssetsChangeRepository().GetFixAssetsChangeListForDepreciate(ctx, filter);
            List<FADepreciationModel> list2 = (from x in list
                                               where x.MLastDepreciatedDate < ctx.MFABeginDate
                                               select x).ToList();
            if (list2.Count > 0)
            {
                for (int j = 0; j < list2.Count; j++)
                {
                    FADepreciationModel depreciation = list2[j];
                    FAFixAssetsModel fixAsset = fixAssetsList.FirstOrDefault((FAFixAssetsModel x) => x.MItemID == depreciation.MID);
                    FAFixAssetsChangeModel avaliableDepreciation = GetAvaliableDepreciation(ctx, fixAsset, (from x in changes
                                                                                                            where x.MID == fixAsset.MItemID
                                                                                                            select x).ToList(), filter.Year, filter.Period);
                    if (avaliableDepreciation != null)
                    {
                        if (avaliableDepreciation.MDepreciatedPeriods + 1 == avaliableDepreciation.MUsefulPeriods)
                        {
                            depreciation.MTempPeriodDepreciatedAmount = depreciation.MNetAmount - avaliableDepreciation.MSalvageAmount;
                            depreciation.MPeriodDepreciatedAmount = depreciation.MTempPeriodDepreciatedAmount;
                        }
                        else if (avaliableDepreciation.MPeriodDepreciatedAmount > depreciation.MNetAmount - avaliableDepreciation.MSalvageAmount)
                        {
                            depreciation.MTempPeriodDepreciatedAmount = ((avaliableDepreciation.MPeriodDepreciatedAmount > depreciation.MNetAmount - avaliableDepreciation.MSalvageAmount) ? (depreciation.MNetAmount - avaliableDepreciation.MSalvageAmount) : avaliableDepreciation.MPeriodDepreciatedAmount);
                            depreciation.MPeriodDepreciatedAmount = depreciation.MTempPeriodDepreciatedAmount;
                        }
                        else
                        {
                            depreciation.MTempPeriodDepreciatedAmount = avaliableDepreciation.MPeriodDepreciatedAmount;
                        }
                    }
                    else if (fixAsset.MDepreciatedPeriods + 1 == depreciation.MUsefulPeriods)
                    {
                        depreciation.MTempPeriodDepreciatedAmount = depreciation.MNetAmount - fixAsset.MSalvageAmount;
                        depreciation.MPeriodDepreciatedAmount = depreciation.MTempPeriodDepreciatedAmount;
                    }
                    else if (fixAsset.MPeriodDepreciatedAmount > depreciation.MNetAmount - fixAsset.MSalvageAmount)
                    {
                        depreciation.MTempPeriodDepreciatedAmount = ((fixAsset.MPeriodDepreciatedAmount > depreciation.MNetAmount - fixAsset.MSalvageAmount) ? (depreciation.MNetAmount - fixAsset.MSalvageAmount) : fixAsset.MPeriodDepreciatedAmount);
                        depreciation.MPeriodDepreciatedAmount = depreciation.MTempPeriodDepreciatedAmount;
                    }
                    else
                    {
                        depreciation.MTempPeriodDepreciatedAmount = fixAsset.MPeriodDepreciatedAmount;
                    }
                    depreciation.MTempPeriodDepreciatedAmount = ((depreciation.MTempPeriodDepreciatedAmount < decimal.Zero) ? decimal.Zero : depreciation.MTempPeriodDepreciatedAmount);
                }
            }
            List<FADepreciationModel> list3 = (from x in list
                                               where x.MLastDepreciatedDate >= ctx.MFABeginDate
                                               select x).ToList();
            if (list3.Count > 0)
            {
                MContext ctx2 = ctx;
                FAFixAssetsFilterModel fAFixAssetsFilterModel = new FAFixAssetsFilterModel();
                DateTime dateTime = new DateTime(filter.Year, filter.Period, 1);
                dateTime = dateTime.AddMonths(-1);
                fAFixAssetsFilterModel.Year = dateTime.Year;
                dateTime = new DateTime(filter.Year, filter.Period, 1);
                dateTime = dateTime.AddMonths(-1);
                fAFixAssetsFilterModel.Period = dateTime.Month;
                List<FADepreciationModel> detailDepreciationList = GetDetailDepreciationList(ctx2, fAFixAssetsFilterModel);
                List<FADepreciationModel> list4 = new List<FADepreciationModel>();
       
                int i;
                for (i = 0; i < changes.Count; i++)
                {
                    FADepreciationModel fADepreciationModel = list3.FirstOrDefault((FADepreciationModel x) => x.MID == changes[i].MID);
                    if (!(fADepreciationModel?.MIsChanged ?? true))
                    {
                        FADepreciationModel fADepreciationModel2 = HandleChangeDepreciate(ctx, detailDepreciationList, changes, fADepreciationModel, filter);
                        if (fADepreciationModel2 != null)
                        {
                            list4.Add(fADepreciationModel2);
                        }
                    }
                }
                list.AddRange(list4);
                for (int k = 0; k < list3.Count && !list3[k].MIsChanged; k++)
                {
                    FADepreciationModel depreciation2 = list3[k];
                    FAFixAssetsModel fixAsset2 = fixAssetsList.FirstOrDefault((FAFixAssetsModel x) => x.MItemID == depreciation2.MID);
          
                    FADepreciationModel fADepreciationModel3 = detailDepreciationList.FirstOrDefault((FADepreciationModel x) => x.MID == depreciation2.MID && !x.MIsAdjust);
               
                    FAFixAssetsChangeModel avaliableDepreciation2 = GetAvaliableDepreciation(ctx, fixAsset2, (from x in changes
                                                                                                              where x.MID == fixAsset2.MItemID
                                                                                                              select x).ToList(), filter.Year, filter.Period);
                    Log(Newtonsoft.Json.JsonConvert.SerializeObject(avaliableDepreciation2));
                    if (avaliableDepreciation2 != null)
                    {
                        if (fADepreciationModel3.MDepreciatedPeriods + 1 == avaliableDepreciation2.MUsefulPeriods)
                        {
                            depreciation2.MTempPeriodDepreciatedAmount = depreciation2.MNetAmount - avaliableDepreciation2.MSalvageAmount;
                            depreciation2.MPeriodDepreciatedAmount = depreciation2.MTempPeriodDepreciatedAmount;
                        }
                        else if (fADepreciationModel3.MPeriodDepreciatedAmount > depreciation2.MNetAmount - fADepreciationModel3.MSalvageAmount)
                        {
                            depreciation2.MTempPeriodDepreciatedAmount = ((fADepreciationModel3.MPeriodDepreciatedAmount > depreciation2.MNetAmount - avaliableDepreciation2.MSalvageAmount) ? (depreciation2.MNetAmount - avaliableDepreciation2.MSalvageAmount) : fADepreciationModel3.MPeriodDepreciatedAmount);
                            depreciation2.MPeriodDepreciatedAmount = depreciation2.MTempPeriodDepreciatedAmount;
                        }
                        else
                        {
                            depreciation2.MTempPeriodDepreciatedAmount = fADepreciationModel3.MPeriodDepreciatedAmount;
                        }
                    }
                    else if (fADepreciationModel3.MDepreciatedPeriods + 1 == depreciation2.MUsefulPeriods)
                    {
                        depreciation2.MTempPeriodDepreciatedAmount = depreciation2.MNetAmount - fADepreciationModel3.MSalvageAmount;
                        depreciation2.MPeriodDepreciatedAmount = depreciation2.MTempPeriodDepreciatedAmount;
                    }
                    else if (fADepreciationModel3.MPeriodDepreciatedAmount > depreciation2.MNetAmount - fADepreciationModel3.MSalvageAmount)
                    {
                        depreciation2.MTempPeriodDepreciatedAmount = ((fADepreciationModel3.MPeriodDepreciatedAmount > depreciation2.MNetAmount - fADepreciationModel3.MSalvageAmount) ? (depreciation2.MNetAmount - fADepreciationModel3.MSalvageAmount) : fADepreciationModel3.MPeriodDepreciatedAmount);
                        depreciation2.MPeriodDepreciatedAmount = depreciation2.MTempPeriodDepreciatedAmount;
                    }
                    else
                    {
                        depreciation2.MTempPeriodDepreciatedAmount = fADepreciationModel3.MPeriodDepreciatedAmount;
                        depreciation2.MPeriodDepreciatedAmount = fADepreciationModel3.MPeriodDepreciatedAmount;
                    }
                    depreciation2.MTempPeriodDepreciatedAmount = ((depreciation2.MTempPeriodDepreciatedAmount < decimal.Zero) ? decimal.Zero : depreciation2.MTempPeriodDepreciatedAmount);
                }
            }
            return (from x in list
                    where x.MTempPeriodDepreciatedAmount != decimal.Zero || x.MAdjustAmount != decimal.Zero
                    orderby x.MFixAssetsNumber descending
                    select x).ToList();
        }

        private FADepreciationModel HandleChangeDepreciate(MContext ctx, List<FADepreciationModel> lastDepreciations, List<FAFixAssetsChangeModel> changes, FADepreciationModel current, FAFixAssetsFilterModel filter)
        {
            List<FAFixAssetsChangeModel> list = changes.Where(delegate (FAFixAssetsChangeModel x)
            {
                int result2;
                if (x.MID == current.MID)
                {
                    DateTime mChangeFromPeriod2 = x.MChangeFromPeriod;
                    int num9 = mChangeFromPeriod2.Year * 12;
                    mChangeFromPeriod2 = x.MChangeFromPeriod;
                    result2 = ((num9 + mChangeFromPeriod2.Month == filter.Year * 12 + filter.Period || x.MIndex == 0) ? 1 : 0);
                }
                else
                {
                    result2 = 0;
                }
                return (byte)result2 != 0;
            }).ToList();
            if (list == null || list.Count == 0 || current == null || !lastDepreciations.Exists((FADepreciationModel x) => x.MID == current.MID))
            {
                return null;
            }
            List<FADepreciationModel> source = (from x in lastDepreciations
                                                where x.MID == current.MID
                                                select x).ToList();
            decimal num = source.Sum((FADepreciationModel x) => x.MPeriodDepreciatedAmount);
            decimal num2 = source.Sum((FADepreciationModel x) => x.MDepreciatedAmount);
            FAFixAssetsChangeModel fAFixAssetsChangeModel = list.FirstOrDefault((FAFixAssetsChangeModel x) => x.MIndex == 0);
            int maxIndex = (from x in list
                            select x.MIndex).Max();
            if (maxIndex == 0)
            {
                return null;
            }
            FAFixAssetsChangeModel fAFixAssetsChangeModel2 = list.FirstOrDefault((FAFixAssetsChangeModel x) => x.MIndex == maxIndex);
            List<FAFixAssetsChangeModel> lastMonthChange = changes.Where(delegate (FAFixAssetsChangeModel x)
            {
                int result;
                if (x.MID == current.MID)
                {
                    DateTime mChangeFromPeriod = x.MChangeFromPeriod;
                    int num8 = mChangeFromPeriod.Year * 12;
                    mChangeFromPeriod = x.MChangeFromPeriod;
                    if (num8 + mChangeFromPeriod.Month < filter.Year * 12 + filter.Period)
                    {
                        result = ((x.MIndex != 0) ? 1 : 0);
                        goto IL_0060;
                    }
                }
                result = 0;
                goto IL_0060;
                IL_0060:
                return (byte)result != 0;
            }).ToList();
            if (lastMonthChange != null && lastMonthChange.Count > 0)
            {
                fAFixAssetsChangeModel = lastMonthChange.FirstOrDefault((FAFixAssetsChangeModel x) => x.MIndex == lastMonthChange.Max((FAFixAssetsChangeModel y) => x.MIndex));
            }
            bool flag = fAFixAssetsChangeModel.MOriginalAmount != fAFixAssetsChangeModel2.MOriginalAmount;
            bool flag2 = fAFixAssetsChangeModel.MPrepareForDecreaseAmount != fAFixAssetsChangeModel2.MPrepareForDecreaseAmount;
            bool mBackAdjust = fAFixAssetsChangeModel2.MBackAdjust;
            decimal num3 = default(decimal);
            decimal num4 = default(decimal);
            decimal num5 = default(decimal);
            decimal num6 = default(decimal);
            int diffOfPeriods = GetDiffOfPeriods(fAFixAssetsChangeModel.MPurchaseDate, fAFixAssetsChangeModel2.MPurchaseDate);
            if (flag & mBackAdjust)
            {
                num4 = fAFixAssetsChangeModel2.MOriginalAmount * (decimal.One - fAFixAssetsChangeModel.MRateOfSalvage / 100m) / (decimal)fAFixAssetsChangeModel.MUsefulPeriods * (decimal)current.MDepreciatedPeriods;
                num3 = num4 - current.MDepreciatedAmount;
                num6 = fAFixAssetsChangeModel2.MOriginalAmount * fAFixAssetsChangeModel2.MRateOfSalvage / 100m;
                num5 = (fAFixAssetsChangeModel2.MOriginalAmount - fAFixAssetsChangeModel2.MPrepareForDecreaseAmount - current.MDepreciatedAmount - num3 - num6) / (decimal)(fAFixAssetsChangeModel2.MUsefulPeriods - current.MDepreciatedPeriods);
            }
            else if ((flag && !mBackAdjust) || diffOfPeriods != 0)
            {
                num3 = fAFixAssetsChangeModel.MPeriodDepreciatedAmount * (decimal)diffOfPeriods;
                num6 = fAFixAssetsChangeModel2.MOriginalAmount * fAFixAssetsChangeModel2.MRateOfSalvage / 100m;
                num5 = (fAFixAssetsChangeModel2.MOriginalAmount - fAFixAssetsChangeModel2.MPrepareForDecreaseAmount - current.MDepreciatedAmount - num3 - num6) / (decimal)(fAFixAssetsChangeModel2.MUsefulPeriods - current.MDepreciatedPeriods);
            }
            else
            {
                num3 = default(decimal);
                num6 = fAFixAssetsChangeModel2.MOriginalAmount * fAFixAssetsChangeModel2.MRateOfSalvage / 100m;
                num5 = (fAFixAssetsChangeModel2.MOriginalAmount - fAFixAssetsChangeModel2.MPrepareForDecreaseAmount - current.MDepreciatedAmount - num6) / (decimal)(fAFixAssetsChangeModel2.MUsefulPeriods - current.MDepreciatedPeriods);
            }
            decimal num7 = fAFixAssetsChangeModel2.MOriginalAmount - current.MDepreciatedAmount - fAFixAssetsChangeModel2.MPrepareForDecreaseAmount;
            current.MTempPeriodDepreciatedAmount = Math.Round((num5 > num7 - num6) ? (num7 - num6) : num5, 2, MidpointRounding.AwayFromZero);
            current.MPeriodDepreciatedAmount = current.MTempPeriodDepreciatedAmount;
            current.MNetAmount = num7;
            current.MIsChanged = true;
            num3 = Math.Round(num3, 2, MidpointRounding.AwayFromZero);
            if (num3 != decimal.Zero)
            {
                return new FADepreciationModel
                {
                    MOrgID = ctx.MOrgID,
                    MItemID = null,
                    MID = current.MID,
                    MIsAdjust = true,
                    MYear = current.MYear,
                    MPeriod = current.MPeriod,
                    MFATypeIDName = current.MFATypeIDName,
                    MNumber = current.MNumber,
                    MFixAssetsNumber = current.MFixAssetsNumber,
                    MFixAssetsName = current.MFixAssetsName,
                    MDepAccountFullName = current.MDepAccountFullName,
                    MExpAccountFullName = current.MExpAccountFullName,
                    MExpAccountCode = current.MExpAccountCode,
                    MDepAccountCode = current.MDepAccountCode,
                    MExpCheckGroupValueID = current.MExpCheckGroupValueID,
                    MDepCheckGroupValueID = current.MDepCheckGroupValueID,
                    MAdjustAmount = num3,
                    MPeriodDepreciatedAmount = num3
                };
            }
            return null;
        }

        private int GetDiffOfPeriods(DateTime src, DateTime dest)
        {
            return src.Year * 12 + src.Month - dest.Year * 12 - dest.Month;
        }

        private MySqlParameter[] GetParameter(MContext ctx, FAFixAssetsFilterModel filter)
        {
            List<MySqlParameter> list = ctx.GetParameters((MySqlParameter)null).ToList();
            list.AddRange(new List<MySqlParameter>
            {
                new MySqlParameter("@MYear", filter.Year),
                new MySqlParameter("@MPeriod", filter.Period),
                new MySqlParameter("@MDate", new DateTime(filter.Year, filter.Period, 1)),
                new MySqlParameter("@MNextDate", new DateTime(filter.Year, filter.Period, 1).AddMonths(1)),
                new MySqlParameter("@Keyword", filter.Keyword),
                new MySqlParameter("@MNumber", filter.Number),
                new MySqlParameter("@MKeyword", filter.MKeyword.HasValue ? filter.MKeyword.Value : decimal.Zero)
            });
            return list.ToArray();
        }

        private string GetSummaryQuerySql(FAFixAssetsFilterModel filter, bool count = false)
        {
            string text = "\n                            SELECT \n                                t1.MPrefix,\n                                CONCAT(t1.MPrefix, t1.MNumber) AS MFixAssetsNumber,\n                                t1.MItemID AS MID,\n                                t9.MItemID,\n                                t2.MName AS MFixAssetsName,\n                                concat(t11.MNumber,'-',t5.MName) AS MFATypeIDName,\n                                t1.MOriginalAmount,\n                                t1.MLastDepreciatedPeriod,\n                                t1.MLastDepreciatedYear,\n                                t1.MPrepareForDecreaseAmount,\n                                t1.MRateOfSalvage,\n                                t1.MSalvageAmount,\n                                t1.MDepreciationFromPeriod,\n                                t1.MPurchaseDate,\n                                -- //--折旧期数\n                                (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MDepreciatedPeriods IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MDepreciatedPeriods IS NULL)\n                                            THEN\n                                                t1.MDepreciatedPeriods\n                                            ELSE t10.MDepreciatedPeriods\n                                        END)\n                                    ELSE t9.MDepreciatedPeriods\n                                END) AS MDepreciatedPeriods,\n                                -- //--净值\n                                (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MNetAmount IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MNetAmount IS NULL)\n                                            THEN\n                                                t1.MNetAmount\n                                            ELSE t10.MNetAmount\n                                        END)\n                                    ELSE t9.MNetAmount\n                                END) AS MNetAmount,\n                                -- //--用户设置折旧额\n                                (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MTempPeriodDepreciatedAmount IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MTempPeriodDepreciatedAmount IS NULL)\n                                            THEN\n                                                t1.MPeriodDepreciatedAmount\n                                            ELSE t10.MTempPeriodDepreciatedAmount\n                                        END)\n                                    ELSE t9.MTempPeriodDepreciatedAmount\n                                END) AS MTempPeriodDepreciatedAmount,\n\n                                -- //--月折旧额\n                                ((CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MPeriodDepreciatedAmount IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MPeriodDepreciatedAmount IS NULL)\n                                            THEN\n                                                t1.MPeriodDepreciatedAmount\n                                            ELSE t10.MPeriodDepreciatedAmount\n                                        END)\n                                    ELSE t9.MPeriodDepreciatedAmount\n                                END) - (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MLastAdjustAmount IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MLastAdjustAmount IS NULL)\n                                            THEN\n                                                0\n                                            ELSE t10.MLastAdjustAmount\n                                        END)\n                                    ELSE t9.MLastAdjustAmount\n                                END)) AS MPeriodDepreciatedAmount,\n\n                                -- //--累计折旧\n                                (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MDepreciatedAmount IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MDepreciatedAmount IS NULL)\n                                            THEN\n                                                t1.MDepreciatedAmount\n                                            ELSE t10.MDepreciatedAmount\n                                        END)\n                                    ELSE t9.MDepreciatedAmount\n                                END) AS MDepreciatedAmount,\n\n                                -- //本年累计折旧\n                                (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MDepreciatedAmountOfYear IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MDepreciatedAmountOfYear IS NULL)\n                                            THEN\n                                                t1.MDepreciatedAmountOfYear\n                                            ELSE t10.MDepreciatedAmountOfYear\n                                        END)\n                                    ELSE t9.MDepreciatedAmountOfYear\n                                END) AS MDepreciatedAmountOfYear,\n\n                                -- //--年\n                                (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MYear IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MYear IS NULL)\n                                            THEN\n                                                @MYear\n                                            ELSE t10.MYear\n                                        END)\n                                    ELSE t9.MYear\n                                END) AS MYear,\n\n                                -- //--期\n                                (CASE\n                                    WHEN\n                                        t9.MOrgID IS NULL\n                                            OR t9.MPeriod IS NULL\n                                    THEN\n                                        (CASE\n                                            WHEN\n                                                (t10.MOrgID IS NULL\n                                                    OR t10.MPeriod IS NULL)\n                                            THEN\n                                                @MPeriod\n                                            ELSE t10.MPeriod\n                                        END)\n                                    ELSE t9.MPeriod\n                                END) AS MPeriod,\n\n                                t9.MLastAdjustAmount,\n                                t9.MVoucherID,\n                                0 as MIsAdjust,\n                                t1.MCreateDate,\n                                null as MDepAccountCode,\n                                null as MExpAccountCode,\n                                null as MDepCheckGroupValueID,\n                                null as MExpCheckGroupValueID,\n                                t4.MNumber AS MVoucherNumber" + (filter.NeedAccountFullName ? ", t7.MFullName as MDepAccountFullName, t9.MFullName as MExpAccountFullName " : "") + "\n                            FROM\n                                t_fa_fixassets t1\n                                    INNER JOIN\n                                t_fa_fixassets_l t2 ON t1.MOrgID = t2.MOrgID\n                                    AND t2.MParentId = t1.MitemID\n                                    AND t2.MLocaleID = @MLocaleID\n                                    AND t2.MIsdelete = t1.MIsDelete\n                                    LEFT JOIN\n                                (SELECT \n                                    MID,\n                                    group_concat(MItemID, ',') as MItemID,\n                                    MOrgID,\n                                    MVoucherID,\n                                    MYear,\n                                    MPeriod,\n                                    MIsDelete,\n                                    SUM(MNetAmount) MNetAmount,\n                                    SUM(MPeriodDepreciatedAmount) AS MPeriodDepreciatedAmount,\n                                    SUM(MDepreciatedAmount) AS MDepreciatedAmount,\n                                    SUM(MDepreciatedAmountOfYear) AS MDepreciatedAmountOfYear,\n                                    SUM(MDepreciatedPeriods) as MDepreciatedPeriods,\n                                    (SUM(MPeriodDepreciatedAmount) - sum(MAdjustAmount)) as MTempPeriodDepreciatedAmount,\n                                    SUM(MAdjustAmount) as MLastAdjustAmount\n                                FROM\n                                    t_fa_depreciation t30\n                                WHERE\n                                    t30.MOrgID = @MOrgID\n                                        AND t30.MIsDelete = 0\n                                GROUP BY MID , MOrgID , MVoucherID, MYear , MPeriod , MIsDelete) t9 ON t1.MItemID = t9.MID\n                                    AND t1.MOrgID = t9.MOrgID\n                                    AND t9.MYear = @MYear\n                                    AND t9.MPeriod = @MPeriod\n\n                                    LEFT JOIN\n                                (SELECT \n                                    MID,\n                                    MOrgID,\n                                    MVoucherID,\n                                    MYear,\n                                    MPeriod,\n                                    MIsDelete,\n                                    SUM(MNetAmount) MNetAmount,\n                                    SUM(MPeriodDepreciatedAmount) AS MPeriodDepreciatedAmount,\n                                    SUM(MDepreciatedAmount) AS MDepreciatedAmount,\n                                    SUM(MDepreciatedAmountOfYear) AS MDepreciatedAmountOfYear,\n                                    SUM(MDepreciatedPeriods) as MDepreciatedPeriods,\n                                    (SUM(MPeriodDepreciatedAmount) - sum(MAdjustAmount)) as MTempPeriodDepreciatedAmount,\n                                    SUM(MAdjustAmount) as MLastAdjustAmount\n                                FROM\n                                    t_fa_depreciation t20\n                                WHERE\n                                    t20.MOrgID = @MOrgID\n                                        AND t20.MIsDelete = 0\n                                GROUP BY MID , MOrgID , MVoucherID, MYear , MPeriod , MIsDelete) t10 ON t1.MItemID = t10.MID\n                                    AND t1.MOrgID = t10.MOrgID\n                                    AND t1.MLastDepreciatedYear = t10.MYear\n                                    AND t1.MLastDepreciatedPeriod = t10.MPeriod \n\n                                    LEFT JOIN\n                                t_gl_voucher t4 ON t4.MItemID = t9.MVoucherID\n                                    AND t4.MisDelete = t1.MisDelete\n                                    AND t4.MorgID = t1.MOrgID\n                                    LEFT JOIN\n                                t_fa_fixassetstype t11 ON t1.MOrgID = t11.MOrgID\n                                    AND t11.MItemID = t1.MFATypeID\n                                    AND t11.MIsdelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_fa_fixassetstype_l t5 ON t1.MOrgID = t5.MOrgID\n                                    AND t5.MParentId = t11.MItemID\n                                    AND t5.MLocaleID = @MLocaleID\n                                    AND t5.MIsdelete = t1.MIsDelete\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\n                                    AND(LENGTH(IFNULL(t4.MItemID, '')) > 0\n                                    OR(( t1.MUsefulPeriods > ifnull(t10.MDepreciatedPeriods, t1.MDepreciatedPeriods))\n                                    AND t1.MDepreciationFromPeriod <= @MDate ))\n                                   AND (t1.MStatus = 0\n                                        OR (t1.MStatus > 0\n                                        AND ((t11.MDepreciationFromCurrentPeriod = 0 and  DATE_ADD(t1.MHandledDate,\n                                        INTERVAL 1 MONTH) > @MDate) or (t11.MDepreciationFromCurrentPeriod = 1 and DATE_ADD(t1.MHandledDate,\n                                        INTERVAL 0 MONTH) > @MDate))))\r\n                                    AND(LENGTH(IFNULL(t4.MItemID, '')) > 0 || (IFNULL(t10.MDepreciatedAmount, t1.MDepreciatedAmount) + t1.MSalvageAmount + t1.MPrepareForDecreaseAmount) < t1.MOriginalAmount)  ";
            if (filter.ItemIDs != null && filter.ItemIDs.Count > 0)
            {
                text = text + " and t1.MItemID in ('" + string.Join("','", filter.ItemIDs) + "')";
            }
            if (!string.IsNullOrWhiteSpace(filter.Number))
            {
                text += " and concat(t1.MPrefix, t1.MNumber) = @MNumber ";
            }
            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                string str = filter.MKeyword.HasValue ? " OR\r\n                    t1.MOriginalAmount = @MKeyword OR\r\n                    IFNULL(t10.MPeriodDepreciatedAmount, t1.MPeriodDepreciatedAmount) = @MKeyword OR\n                    IFNULL(t10.MDepreciatedAmount, t1.MDepreciatedAmount) = @MKeyword OR\n                    IFNULL(t10.MDepreciatedAmountOfYear, t1.MDepreciatedAmountOfYear) = @MKeyword OR\n                    t1.MPrepareForDecreaseAmount = @MKeyword OR\n                    IFNULL(t10.MNetAmount, t1.MNetAmount) = @MKeyword\r\n                " : "";
                text = text + " and ( \r\n                    concat(t1.MPrefix, t1.MNumber) like concat('%',@Keyword,'%') Or \r\n                    concat(t11.MNumber,'-',t5.MName) like concat('%',@Keyword,'%') Or \r\n                    t2.MName like concat('%',@Keyword,'%') Or \r\n                    t1.MPurchaseDate like binary concat('%',@Keyword,'%') Or \r\n                    t1.MDepreciationFromPeriod like binary concat('%',@Keyword,'%') OR \r\n                    t5.MName like concat('%',@Keyword,'%')" + str + ")";
            }
            return count ? ("select count(*) as MCount from (" + text + ")t") : ("select * from (" + text + " order by MFixAssetsNumber desc  limit " + (filter.page - 1) * filter.rows + "," + filter.rows + ")t ");
        }

        public OperationResult SaveDepreciationList(MContext ctx, FAFixAssetsFilterModel filter)
        {
            List<CommandInfo> insertOrUpdateCmds = ModelInfoManager.GetInsertOrUpdateCmds(ctx, filter.DepreciationModels, null, true);
            return new OperationResult
            {
                Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(insertOrUpdateCmds) > 0)
            };
        }

        public List<MActionResultCodeEnum> GetValidateVoucherDeleteResult(MContext ctx, List<string> voucherIDs)
        {
            GLUtility gLUtility = new GLUtility();
            ValidateQueryModel validateExitsCreatedDepreciationVoucher = gLUtility.GetValidateExitsCreatedDepreciationVoucher(ctx, voucherIDs);
            ValidateQueryModel validateExitsChange = gLUtility.GetValidateExitsChange(ctx, voucherIDs);
            return gLUtility.QueryValidateSql(ctx, false, new List<ValidateQueryModel>
            {
                validateExitsCreatedDepreciationVoucher,
                validateExitsChange
            }.ToArray());
        }

        public List<CommandInfo> GetDeleteDepreciatedVoucherCmds(MContext ctx, List<string> voucherIDs)
        {
            SqlWhere filter = new SqlWhere().In("MVoucherID", voucherIDs);
            List<FADepreciationModel> dataModelList = ModelInfoManager.GetDataModelList<FADepreciationModel>(ctx, filter, false, false);
            List<CommandInfo> list = new List<CommandInfo>();
            List<CommandInfo> list2 = new List<CommandInfo>();
            List<string> list3 = (from x in dataModelList
                                  select x.MID).Distinct().ToList();
            if (dataModelList == null || dataModelList.Count == 0)
            {
                return list;
            }
            List<FAFixAssetsModel> dataModelList2 = ModelInfoManager.GetDataModelList<FAFixAssetsModel>(ctx, new SqlWhere(), false, false);
            for (int i = 0; i < list3.Count; i++)
            {
                string id = list3[i];
                FAFixAssetsModel fAFixAssetsModel = dataModelList2.FirstOrDefault((FAFixAssetsModel x) => x.MItemID == id);
                DateTime dateTime = fAFixAssetsModel.MLastDepreciatedDate;
                int num = dateTime.Year * 12;
                dateTime = fAFixAssetsModel.MLastDepreciatedDate;
                int num2 = num + dateTime.Month;
                dateTime = ctx.MFABeginDate;
                int num3 = dateTime.Year * 12;
                dateTime = ctx.MFABeginDate;
                if (num2 >= num3 + dateTime.Month)
                {
                    dateTime = fAFixAssetsModel.MLastDepreciatedDate;
                    DateTime dateTime3 = fAFixAssetsModel.MLastDepreciatedDate = dateTime.AddMonths(-1);
                    dateTime = fAFixAssetsModel.MLastDepreciatedDate;
                    int num4 = dateTime.Year * 12;
                    dateTime = fAFixAssetsModel.MLastDepreciatedDate;
                    int num5 = num4 + dateTime.Month;
                    dateTime = fAFixAssetsModel.MDepreciationFromPeriod;
                    int num6 = dateTime.Year * 12;
                    dateTime = fAFixAssetsModel.MDepreciationFromPeriod;
                    if (num5 < num6 + dateTime.Month)
                    {
                        fAFixAssetsModel.MLastDepreciatedYear = 0;
                        fAFixAssetsModel.MLastDepreciatedPeriod = 0;
                    }
                    list.AddRange(list2 = ModelInfoManager.GetInsertOrUpdateCmd<FAFixAssetsModel>(ctx, fAFixAssetsModel, new List<string>
                    {
                        "MLastDepreciatedYear",
                        "MLastDepreciatedPeriod"
                    }, true));
                }
            }
            List<MySqlParameter> list4 = ctx.GetParameters((MySqlParameter)null).ToList();
            List<CommandInfo> list5 = list;
            CommandInfo obj = new CommandInfo
            {
                CommandText = "update t_fa_depreciation t set t.MIsDelete = 1  where MOrgID = @MOrgID and MIsDelete = 0 and  MIsAdjust = 1 and  MVoucherID " + GLUtility.GetInFilterQuery(voucherIDs, ref list4, "M_ID")
            };
            DbParameter[] array = obj.Parameters = list4.ToArray();
            list5.Add(obj);
            List<MySqlParameter> list6 = ctx.GetParameters((MySqlParameter)null).ToList();
            List<CommandInfo> list7 = list;
            CommandInfo obj2 = new CommandInfo
            {
                CommandText = "update t_fa_depreciation t set t.MVoucherID = null, t.MDepreciatedAmount = null, MDepreciatedPeriods = null, t.MDepreciatedAmountOfYear = null, t.MNetAmount = null where MOrgID = @MOrgID and MIsDelete = 0 and MVoucherID  " + GLUtility.GetInFilterQuery(voucherIDs, ref list6, "M_ID")
            };
            array = (obj2.Parameters = list6.ToArray());
            list7.Add(obj2);
            return list;
        }

        private string GetDetailQuerySql(FAFixAssetsFilterModel filter)
        {
            string text = "\n                            SELECT \n                                t1.MPrefix,\n                                CONCAT(t1.MPrefix, t1.MNumber) AS MFixAssetsNumber,\n                                t1.MItemID AS MID,\n                                t3.MItemID,\n                                t2.MName AS MFixAssetsName,\n                                concat(t11.MNumber,'-',t5.MName) AS MFATypeIDName,\n                                t1.MOriginalAmount,\n                                t1.MLastDepreciatedPeriod,\n                                t1.MLastDepreciatedYear,\n                                t1.MPrepareForDecreaseAmount,\n                                t1.MRateOfSalvage,\n                                t1.MSalvageAmount,\n                                t1.MDepreciationFromPeriod,\n                                ifnull(t3.MDepreciatedPeriods, ifnull(t10.MDepreciatedPeriods, t1.MDepreciatedPeriods)) MDepreciatedPeriods,\n                                t1.MPurchaseDate,\n                                IFNULL(t3.MNetAmount, IFNULL(t10.MNetAmount, t1.MNetAmount)) MNetAmount,\n                                IFNULL(t3.MTempPeriodDepreciatedAmount, IFNULL(t10.MTempPeriodDepreciatedAmount, t1.MPeriodDepreciatedAmount)) MTempPeriodDepreciatedAmount,\n                                (IFNULL(t3.MPeriodDepreciatedAmount, IFNULL(t10.MPeriodDepreciatedAmount, t1.MPeriodDepreciatedAmount) - ifnull(t10.MLastAdjustAmount, 0)) ) AS MPeriodDepreciatedAmount,\n                                IFNULL(t3.MDepreciatedAmount, IFNULL(t10.MDepreciatedAmount, t1.MDepreciatedAmount)) AS MDepreciatedAmount,\n                                IFNULL(t3.MDepreciatedAmountOfYear, IFNULL(t10.MDepreciatedAmountOfYear, t1.MDepreciatedAmountOfYear)) AS MDepreciatedAmountOfYear,\n                                t10.MLastAdjustAmount,\n                                t3.MVoucherID,\n                                IFNULL(t3.MYear, @MYear) MYear,\n                                IFNULL(t3.MPeriod, @MPeriod) MPeriod,\n                                t3.MIsAdjust,\n                                t1.MCreateDate,\n                                IFNULL(t3.MDepAccountCode, t1.MDepAccountCode) AS MDepAccountCode,\n                                IFNULL(t3.MExpAccountCode, t1.MExpAccountCode) AS MExpAccountCode,\n                                IFNULL(t3.MDepCheckGroupValueID,\n                                        t1.MDepCheckGroupValueID) AS MDepCheckGroupValueID,\n                                IFNULL(t3.MExpCheckGroupValueID,\n                                        t1.MExpCheckGroupValueID) AS MExpCheckGroupValueID,\n                                t4.MNumber AS MVoucherNumber" + (filter.NeedAccountFullName ? ", t7.MFullName as MDepAccountFullName, t9.MFullName as MExpAccountFullName " : "") + "\n                            FROM\n                                t_fa_fixassets t1\n                                    INNER JOIN\n                                t_fa_fixassets_l t2 ON t1.MOrgID = t2.MOrgID\n                                    AND t2.MParentId = t1.MitemID\n                                    AND t2.MLocaleID = @MLocaleID\n                                    AND t2.MIsdelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_fa_depreciation t3 ON t1.MItemId = t3.MID\n                                    AND t3.MOrgID = t1.MOrgID\n                                    AND t1.MIsDelete = t3.MIsDelete\n                                    AND t3.MYear = @MYear\n                                    AND t3.MPeriod = @MPeriod \n                                    LEFT JOIN\n                                (SELECT \n                                    MID,\n                                    MOrgID,\n                                    MYear,\n                                    MPeriod,\n                                    MIsDelete,\n                                    SUM(MNetAmount) MNetAmount,\n                                    SUM(MPeriodDepreciatedAmount) AS MPeriodDepreciatedAmount,\n                                    SUM(MDepreciatedAmount) AS MDepreciatedAmount,\n                                    SUM(MDepreciatedAmountOfYear) AS MDepreciatedAmountOfYear,\n                                    SUM(MDepreciatedPeriods) as MDepreciatedPeriods,\n                                    (SUM(MPeriodDepreciatedAmount) - sum(MAdjustAmount)) as MTempPeriodDepreciatedAmount,\n                                    SUM(MAdjustAmount) as MLastAdjustAmount\n                                FROM\n                                    t_fa_depreciation t20\n                                WHERE\n                                    t20.MOrgID = @MOrgID\n                                        AND t20.MIsDelete = 0\n                                GROUP BY MID , MOrgID , MYear , MPeriod , MIsDelete) t10 ON t1.MItemID = t10.MID\n                                    AND t1.MOrgID = t10.MOrgID\n                                    AND t1.MLastDepreciatedYear = t10.MYear\n                                    AND t1.MLastDepreciatedPeriod = t10.MPeriod \n                                    LEFT JOIN\n                                t_gl_voucher t4 ON t4.MItemID = t3.MVoucherID\n                                    AND t4.MisDelete = t1.MisDelete\n                                    AND t4.MorgID = t1.MOrgID\n                                    LEFT JOIN\n                                t_fa_fixassetstype t11 ON t1.MOrgID = t11.MOrgID\n                                    AND t11.MItemID = t1.MFATypeID\n                                    AND t11.MIsdelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_fa_fixassetstype_l t5 ON t1.MOrgID = t5.MOrgID\n                                    AND t5.MParentId = t11.MItemID\n                                    AND t5.MLocaleID = @MLocaleID\n                                    AND t5.MIsdelete = t1.MIsDelete" + (filter.NeedAccountFullName ? "  LEFT JOIN\n                                t_bd_account t6 ON t1.MOrgID = t6.MOrgID\n                                    AND t6.MCode = IFNULL(t3.MDepAccountCode, t1.MDepAccountCode)\n                                    AND t6.MIsdelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_account_l t7 ON t1.MOrgID = t7.MOrgID\n                                    AND t7.MParentId = t6.MItemID\n                                    AND t7.MLocaleID = @MLocaleID\n                                    AND t7.MIsdelete = t1.MIsDelete\n                                    LEFT JOIN\n                                 t_bd_account t8 ON t1.MOrgID = t8.MOrgID\n                                    AND t8.MCode = IFNULL(t3.MExpAccountCode, t1.MExpAccountCode)\n                                    AND t8.MIsdelete = t1.MIsDelete\n                                    LEFT JOIN\n                                t_bd_account_l t9 ON t1.MOrgID = t9.MOrgID\n                                    AND t9.MParentId = t8.MItemID\n                                    AND t9.MLocaleID = @MLocaleID\n                                    AND t9.MIsdelete = t1.MIsDelete" : "") + "\n                            WHERE\n                                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\n                                    AND(LENGTH(IFNULL(t4.MItemID, '')) > 0\n                                    OR(( t1.MUsefulPeriods > (ifnull(t3.MDepreciatedPeriods, ifnull(t10.MDepreciatedPeriods, t1.MDepreciatedPeriods)))\n                                    AND t1.MDepreciationFromPeriod <= @MDate ))\n                                    AND(t1.MStatus = 0\n                                    OR(t1.MStatus > 0\n                                    AND date_add(t1.MHandledDate,interval 1 month) > @MDate ))\n                                    AND(IFNULL(t3.MDepreciatedAmount, IFNULL(t10.MDepreciatedAmount, t1.MDepreciatedAmount)) + t1.MSalvageAmount + t1.MPrepareForDecreaseAmount) < t1.MOriginalAmount) ";
            if (filter.ItemIDs != null && filter.ItemIDs.Count > 0)
            {
                text = text + " and t1.MItemID in ('" + string.Join("','", filter.ItemIDs) + "')";
            }
            return "select * from(" + text + ") x order by x.MFixAssetsNumber desc ";
        }
    }
}
