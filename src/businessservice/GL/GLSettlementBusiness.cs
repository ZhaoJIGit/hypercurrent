using JieNor.Megi.BusinessContract.GL;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace JieNor.Megi.BusinessService.GL
{
	public class GLSettlementBusiness : IGLSettlementBusiness, IDataContract<GLSettlementModel>
	{
		private readonly GLSettlementRepository dal = new GLSettlementRepository();

		private readonly BASLangBusiness lang = new BASLangBusiness();

		public GLSettlementModel GetSettlementModel(MContext ctx, GLSettlementModel model)
		{
			return dal.GetSettlementModel(ctx, model);
		}

		public DateTime GetCurrentPeriod(MContext ctx)
		{
			return dal.GetCurrentPeriod(ctx);
		}

		public OperationResult PreSettle(MContext ctx, DateTime date, bool isCalculate = false)
		{
			GLDashboardModel dashboardData = new GLVoucherBusiness().GetDashboardData(ctx, date.Year, date.Month, 0);
			bool flag = true;
			List<string> list = new List<string>();
			string objectID = "";
			if (!ctx.MInitBalanceOver)
			{
				flag = false;
				list.Add(lang.GetText(ctx, LangModule.Common, "BalanceNotInited", "系统科目未初始化期初余额"));
			}
			List<GLVoucherModel> voucherModelList = new GLVoucherRepository().GetVoucherModelList(ctx, null, false, date.Year, date.Month);
			if (voucherModelList.Count > 0 && (from x in voucherModelList
			where x.MStatus == 0
			select x).ToList().Count > 0)
			{
				flag = false;
				list.Add(lang.GetText(ctx, LangModule.Common, "ExistsUnApprovedVouchers", "存在未审核的凭证，请先审核"));
			}
			if ((voucherModelList.Count > 0 && voucherModelList.Max((GLVoucherModel x) => int.Parse(x.MNumber)) != voucherModelList.Count) & isCalculate)
			{
				List<GLVoucherModel> list2 = (from x in voucherModelList
				orderby int.Parse(x.MNumber)
				select x).ToList();
				int num = voucherModelList.Max((GLVoucherModel x) => int.Parse(x.MNumber));
				List<string> list3 = new List<string>();
				int num2 = 0;
				for (int i = 1; i <= num; i++)
				{
					if (int.Parse(list2[num2].MNumber) != i)
					{
						list3.Add(COMResourceHelper.ToVoucherNumber(ctx, null, i));
					}
					else
					{
						num2++;
					}
				}
				flag = false;
				list.Add(string.Format(lang.GetText(ctx, LangModule.Common, "VoucherNumberIsNotContinuous", "凭证号不连续，空缺编号为[{0}],需要重新编号 "), string.Join(",", list3)));
				objectID = "1";
			}
			List<GLUnsettlementModel> unsettledPeriod = dal.GetUnsettledPeriod(ctx, new GLSettlementModel
			{
				MYear = date.Year,
				MPeriod = date.Month,
				MStatus = 1
			});
			if (unsettledPeriod != null && unsettledPeriod.Count > 0)
			{
				flag = false;
				list.Add(lang.GetText(ctx, LangModule.Common, "ExistsUnsettledPeriodBefore", "本期前有未结帐的期，请先将其结帐"));
			}
			if ((flag & isCalculate) && list.Count > 0)
			{
				list.Add(lang.GetText(ctx, LangModule.Common, "ConfirmCalculateWithWarning", "以上原因可能导致测算结果不准确，您确定要测算吗?"));
			}
			string text = string.Empty;
			for (int j = 0; j < list.Count; j++)
			{
				text = text + (j + 1) + "." + list[j] + "<br>";
			}
			return new OperationResult
			{
				Success = flag,
				Message = text,
				ObjectID = objectID
			};
		}


		/// <summary>
		/// 获取稳定时期
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="includeCurrentPeriod"></param>
		/// <returns></returns>
		public List<DateTime> GetSettledPeriodFromBeginDate(MContext ctx, bool includeCurrentPeriod = false)
		{
			return dal.GetSettledPeriodFromBeginDate(ctx, includeCurrentPeriod, true);
		}

		public DateTime GetAvaliableVoucherDate(MContext ctx)
		{
			return dal.GetAvaliableVoucherDate(ctx);
		}

		public OperationResult Settle(MContext ctx, GLSettlementModel model)
		{
			return dal.Settle(ctx, model);
		}

		public bool IsPeriodValid(MContext ctx, DateTime date)
		{
			bool result = true;
			if (ctx.MRegProgress == 15 && date >= ctx.MGLBeginDate)
			{
				GLSettlementModel dataModelByFilter = GetDataModelByFilter(ctx, new SqlWhere().AddFilter("MOrgID", SqlOperators.Equal, ctx.MOrgID).AddFilter("MYear", SqlOperators.Equal, date.Year).AddFilter("MPeriod", SqlOperators.Equal, date.Month));
				result = (dataModelByFilter == null || dataModelByFilter.MStatus != 1);
			}
			return result;
		}

		public string GetLastFinishedPeriod(MContext ctx)
		{
			return dal.GetLastFinishedPeriod(ctx);
		}

		public List<string> GetSettledPeriod(MContext ctx)
		{
			List<string> list = new List<string>();
			List<DateTime> settledPeriodList = dal.GetSettledPeriodList(ctx);
			if (settledPeriodList != null && settledPeriodList.Count > 0)
			{
				foreach (DateTime item in settledPeriodList)
				{
					list.Add(item.Year + "-" + item.Month);
				}
			}
			return list;
		}

		public List<DateTime> GetSettledDateTime(MContext ctx)
		{
			return dal.GetSettledPeriodList(ctx);
		}

		public List<DateTime> GetFullPeriod(MContext ctx)
		{
			MLogger.Log("GetFullPeriod::" +Newtonsoft.Json.JsonConvert.SerializeObject(ctx));
			List<DateTime> list = new List<DateTime>();
			DataSet maxHadVoucherPeriod = dal.GetMaxHadVoucherPeriod(ctx);
			DateTime mGLBeginDate = ctx.MBeginDate;
			DateTime dateTime = ctx.MBeginDate;
			if (maxHadVoucherPeriod != null && maxHadVoucherPeriod.Tables.Count > 0 && maxHadVoucherPeriod.Tables[0].Rows.Count > 0)
			{
				DataTable dataTable = maxHadVoucherPeriod.Tables[0];
				DataRow dataRow = dataTable.Rows[0];
				string s = Convert.ToString(dataRow[0]) + "01";
				if (!DateTime.TryParseExact(s, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime))
				{
					dateTime = ctx.MBeginDate;
				}
			}
			DateTime dateTime2 = mGLBeginDate.AddMonths(-1);
			List<DateTime> nextSettledPeriod = dal.GetNextSettledPeriod(ctx, new GLSettlementModel
			{
				MOrgID = ctx.MOrgID,
				MYear = dateTime2.Year,
				MPeriod = dateTime2.Month
			});
			if (nextSettledPeriod != null && nextSettledPeriod.Count > 0)
			{
				DateTime dateTime3 = nextSettledPeriod[nextSettledPeriod.Count - 1].AddMonths(1);
				dateTime = ((dateTime < dateTime3) ? dateTime3 : dateTime);
			}
			DateTime item = mGLBeginDate;
			while (item.Year * 100 + item.Month <= dateTime.Year * 100 + dateTime.Month)
			{
				list.Add(item);
				item = item.AddMonths(1);
			}
			
			

			MLogger.Log("list::" + Newtonsoft.Json.JsonConvert.SerializeObject(list));

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

		public OperationResult InsertOrUpdate(MContext ctx, GLSettlementModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<GLSettlementModel> modelData, string fields = null)
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

		public GLSettlementModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public GLSettlementModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<GLSettlementModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<GLSettlementModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
