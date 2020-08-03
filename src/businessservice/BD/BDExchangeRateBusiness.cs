using JieNor.Megi.BusinessContract.BD;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BD
{
	public class BDExchangeRateBusiness : IBDExchangeRateBusiness, IDataContract<BDExchangeRateModel>
	{
		private readonly BDExchangeRateRepository dal = new BDExchangeRateRepository();

		public List<BDExchangeRateModel> GetExchangeRateList(MContext ctx, BDExchangeRateModel model)
		{
			return new BDExchangeRateRepository().GetExchangeRateList(model);
		}

		public DataGridJson<BDExchangeRateViewModel> GetExchangeRateViewList(MContext context, BDExchangeRateFilterModel filter)
		{
			return new BDExchangeRateRepository().GetExchangeRateViewList(filter, context);
		}

		public OperationResult InsertExchangeRate(MContext ctx, BDExchangeRateModel model)
		{
			return new BDExchangeRateRepository().InsertOrUpdate(ctx, model, null);
		}

		public List<BDExchangeRateModel> GetMonthlyExchangeRateList(MContext ctx, DateTime date)
		{
			return dal.GetMonthlyExchangeRateList(ctx, date);
		}

		public OperationResult UpdateExchangeRate(MContext ctx, BDExchangeRateModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (!string.IsNullOrEmpty(model.MItemID))
			{
				BDExchangeRateFilterModel bDExchangeRateFilterModel = new BDExchangeRateFilterModel();
				bDExchangeRateFilterModel.Equal("MSourceCurrencyID", model.MSourceCurrencyID);
				bDExchangeRateFilterModel.Equal("MTargetCurrencyID", model.MTargetCurrencyID);
				BDExchangeRateFilterModel bDExchangeRateFilterModel2 = bDExchangeRateFilterModel;
				DateTime dateTime = ctx.DateNow;
				bDExchangeRateFilterModel2.LessOrEqual("MRateDate", dateTime.ToString("yyyy-MM-dd"));
				List<BDExchangeRateModel> modelList = dal.GetModelList(ctx, bDExchangeRateFilterModel, false);
				if (modelList == null || modelList.Count() == 0 || (modelList.Count() == 1 && modelList[0].MItemID == model.MItemID && model.MRateDate > ctx.DateNow))
				{
					operationResult.Success = false;
					operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "NeedLessOrEqualTodayExchangeRate", "Need a less than or log in today's exchange rate");
					return operationResult;
				}
				if (modelList.Exists((BDExchangeRateModel x) => x.MRateDate == model.MRateDate && x.MItemID != model.MItemID))
				{
					operationResult.Success = false;
					string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExistExhangeRateInDate", "日期：{0}已存在汇率。同一天只能存在一条汇率记录。");
					OperationResult operationResult2 = operationResult;
					string format = text;
					dateTime = model.MRateDate;
					operationResult2.Message = string.Format(format, dateTime.ToString("yyyy-MM-dd"));
					return operationResult;
				}
			}
			List<CommandInfo> updateExchangeRateCmdList = GetUpdateExchangeRateCmdList(ctx, model, ref operationResult);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(updateExchangeRateCmdList) > 0);
			return operationResult;
		}

		public OperationResult UpdateExchangeRateList(MContext ctx, List<BDExchangeRateModel> list)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			if (list != null && list.Count > 0)
			{
				List<CommandInfo> list2 = new List<CommandInfo>();
				for (int i = 0; i < list.Count; i++)
				{
					if (!operationResult.Success)
					{
						break;
					}
					list[i].MOrgID = ctx.MOrgID;
					list2.AddRange(GetUpdateExchangeRateCmdList(ctx, list[i], ref operationResult));
				}
				if (operationResult.Success)
				{
					operationResult.Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list2) > 0);
				}
			}
			return operationResult;
		}

		public List<CommandInfo> GetUpdateExchangeRateCmdList(MContext ctx, BDExchangeRateModel model, ref OperationResult result)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			BDExchangeRateRepository bDExchangeRateRepository = new BDExchangeRateRepository();
			if (bDExchangeRateRepository.Exists(ctx, model.MItemID, true))
			{
				result = BDRepository.IsCanDelete(ctx, "ExchangeRate", model.MItemID);
				if (result.Success)
				{
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDExchangeRateModel>(ctx, model, null, true));
				}
				return list;
			}
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal(" MOrgID ", ctx.MOrgID);
			sqlWhere.Equal(" MTargetCurrencyID ", model.MTargetCurrencyID);
			sqlWhere.Equal(" MSourceCurrencyID ", model.MSourceCurrencyID);
			sqlWhere.Equal(" DATE_FORMAT(MRateDate , '%Y-%m-%d') ", model.MRateDate.ToString("yyyy-MM-dd"));
			BDExchangeRateModel dataModelByFilter = bDExchangeRateRepository.GetDataModelByFilter(ctx, sqlWhere);
			if (dataModelByFilter != null && !string.IsNullOrEmpty(dataModelByFilter.MItemID))
			{
				result = BDRepository.IsCanDelete(ctx, "ExchangeRate", dataModelByFilter.MItemID);
				if (result.Success)
				{
					dataModelByFilter.MRate = model.MRate;
					dataModelByFilter.MRateDate = model.MRateDate;
					dataModelByFilter.MUserRate = model.MUserRate;
					dataModelByFilter.MRate = model.MRate;
					dataModelByFilter.MIsDelete = false;
					list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDExchangeRateModel>(ctx, dataModelByFilter, null, true));
				}
			}
			else
			{
				list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<BDExchangeRateModel>(ctx, model, null, true));
			}
			return list;
		}

		public decimal GetExchangeRate(MContext ctx, string from, DateTime date, string to = null)
		{
			return BDExchangeRateRepository.GetExchangeRate(ctx, from, date, to);
		}

		public OperationResult RemoveExchangeRate(MContext ctx, BDExchangeRateModel model)
		{
			BDExchangeRateRepository bDExchangeRateRepository = new BDExchangeRateRepository();
			if (bDExchangeRateRepository.Exists(ctx, model.MItemID, false))
			{
				return bDExchangeRateRepository.DeleteCurrency(ctx, model);
			}
			return new OperationResult
			{
				Success = false
			};
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, BDExchangeRateModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BDExchangeRateModel> modelData, string fields = null)
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

		public BDExchangeRateModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BDExchangeRateModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BDExchangeRateModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BDExchangeRateModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
