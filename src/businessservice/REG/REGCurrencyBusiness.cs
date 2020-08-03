using JieNor.Megi.BusinessContract.REG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.REG
{
	public class REGCurrencyBusiness : APIBusinessBase<REGCurrencyViewModel>, IREGCurrencyBusiness, IDataContract<REGCurrencyModel>
	{
		private List<BASCurrencyModel> _currencyDataPool;

		private readonly REGCurrencyRepository dal = new REGCurrencyRepository();

		private REGCurrencyRepository repository = new REGCurrencyRepository();

		private BASCurrencyRepository basDal = new BASCurrencyRepository();

		protected override void OnGetBefore(MContext ctx, GetParam param)
		{
			param.ModifiedSince = DateTime.MinValue;
			param.IgnoreModifiedSince = true;
		}

		protected override DataGridJson<REGCurrencyViewModel> OnGet(MContext ctx, GetParam param)
		{
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			_currencyDataPool = instance.BASCurrencies;
			return dal.Get(ctx, param);
		}

		protected override void OnGetAfter(MContext ctx, GetParam param, APIDataPool dataPool, REGCurrencyViewModel model)
		{
		}

		public List<REGCurrencyModel> GetCurrencyList(MContext context)
		{
			return new REGCurrencyRepository().GetList(context);
		}

		public List<REGCurrencyViewModel> GetCurrencyViewList(MContext context, DateTime? endDate, bool isIncludeBase = false)
		{
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			return rEGCurrencyRepository.GetCurrencyViewList(context, endDate, isIncludeBase, null);
		}

		public List<REGCurrencyViewModel> GetCurrencyViewList(MContext context, DateTime? endDate, bool isIncludeBase = false, GetParam param = null)
		{
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			return rEGCurrencyRepository.GetCurrencyViewList(context, endDate, isIncludeBase, param);
		}

		public List<REGCurrencyViewModel> GetBillCurrencyViewList(MContext ctx, DateTime? endTime, bool isIncludeBase = false)
		{
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			List<REGCurrencyViewModel> billViewList = rEGCurrencyRepository.GetBillViewList(ctx, endTime, null, false);
			if (isIncludeBase)
			{
				BASCurrencyViewModel @base = rEGCurrencyRepository.GetBase(ctx, false, null, null);
				if (@base != null)
				{
					REGCurrencyViewModel rEGCurrencyViewModel = new REGCurrencyViewModel();
					rEGCurrencyViewModel.MCurrencyID = @base.MCurrencyID;
					rEGCurrencyViewModel.MName = @base.MLocalName;
					rEGCurrencyViewModel.MUserRate = "1";
					billViewList.Insert(0, rEGCurrencyViewModel);
				}
			}
			return billViewList;
		}

		public List<REGCurrencyViewModel> GetAllCurrencyList(MContext context, bool isIncludeBase = false, bool ignoreLocale = false)
		{
			List<REGCurrencyViewModel> allCurrencyList = dal.GetAllCurrencyList(context, ignoreLocale);
			if (isIncludeBase)
			{
				List<BASCurrencyViewModel> list = new List<BASCurrencyViewModel>();
				GetBaseCurrency(context, ignoreLocale, list);
				foreach (BASCurrencyViewModel item in list)
				{
					REGCurrencyViewModel rEGCurrencyViewModel = new REGCurrencyViewModel();
					rEGCurrencyViewModel.MCurrencyID = item.MCurrencyID;
					rEGCurrencyViewModel.MName = item.MLocalName;
					rEGCurrencyViewModel.MUserRate = "1";
					allCurrencyList.Insert(0, rEGCurrencyViewModel);
				}
			}
			return allCurrencyList;
		}

		public List<REGCurrencyViewModel> GetCurrencyListByName(MContext context, bool isIncludeBase = false, bool ignoreLocale = false)
		{
			List<REGCurrencyViewModel> allCurrencyList = dal.GetAllCurrencyList(context, ignoreLocale);
			if (isIncludeBase)
			{
				List<BASCurrencyViewModel> list = new List<BASCurrencyViewModel>();
				GetBaseCurrency(context, ignoreLocale, list);
				foreach (BASCurrencyViewModel item in list)
				{
					REGCurrencyViewModel rEGCurrencyViewModel = new REGCurrencyViewModel();
					rEGCurrencyViewModel.MCurrencyID = item.MCurrencyID;
					rEGCurrencyViewModel.MName = item.MLocalName;
					rEGCurrencyViewModel.MUserRate = "1";
					allCurrencyList.Insert(0, rEGCurrencyViewModel);
				}
			}
			return allCurrencyList;
		}

		public OperationResult InsertCurrency(MContext ctx, REGCurrencyModel model)
		{
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			return rEGCurrencyRepository.InsertCurrency(ctx, model);
		}

		public OperationResult RemoveModel(MContext ctx, REGCurrencyModel model)
		{
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Currency", model.MItemID);
			if (operationResult.Success)
			{
				if (repository.ExistsByCurrencyID(model.MCurrencyID, model.MOrgID, ctx))
				{
					operationResult = repository.DeleteCurrency(ctx, model);
				}
				else
				{
					operationResult.Success = false;
				}
			}
			return operationResult;
		}

		public BASCurrencyViewModel GetBaseCurrency(MContext context)
		{
			return GetBaseCurrency(context, false, null);
		}

		public BASCurrencyViewModel GetBaseCurrency(MContext context, bool ignoreLocale = false, List<BASCurrencyViewModel> baseList = null)
		{
			return new REGCurrencyRepository().GetBase(context, ignoreLocale, baseList, null);
		}

		public void CheckCurrencyExist<T>(MContext ctx, T model, string currencyField, List<IOValidationResultModel> validationResult, List<REGCurrencyViewModel> currencyList, int rowIndex = -1)
		{
			string currencyName = ModelHelper.GetModelValue(model, currencyField);
			if (!string.IsNullOrWhiteSpace(currencyName))
			{
				REGCurrencyViewModel rEGCurrencyViewModel = currencyList.FirstOrDefault((REGCurrencyViewModel f) => currencyName.ToUpper().Contains(f.MCurrencyID) || currencyName.ToUpper().Contains(f.MName.ToUpper()) || f.MItemID == currencyName);
				if (rEGCurrencyViewModel != null)
				{
					ModelHelper.SetModelValue(model, currencyField, rEGCurrencyViewModel.MCurrencyID, null);
				}
				else
				{
					int rowIndex2 = 0;
					if (rowIndex != -1)
					{
						rowIndex2 = rowIndex;
					}
					else
					{
						int.TryParse(ModelHelper.TryGetModelValue(model, "MRowIndex"), out rowIndex2);
					}
					validationResult.Add(new IOValidationResultModel
					{
						FieldType = IOValidationTypeEnum.Currency,
						FieldValue = currencyName,
						Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CurrencyNotFound", "The currency:{0} can't be found!"), currencyName),
						RowIndex = rowIndex2
					});
				}
			}
		}

		public void CheckCurrencyExist<T>(MContext ctx, List<T> modelList, string currencyField, List<IOValidationResultModel> validationResult)
		{
			List<REGCurrencyViewModel> currencyListByName = GetCurrencyListByName(ctx, true, true);
			foreach (T model in modelList)
			{
				CheckCurrencyExist(ctx, model, currencyField, validationResult, currencyListByName, -1);
			}
		}

		public void CheckCurrencyExist<T>(MContext ctx, List<T> modelList, string currencyField, List<IOValidationResultModel> validationResult, List<REGCurrencyViewModel> currencyList)
		{
			foreach (T model in modelList)
			{
				CheckCurrencyExist(ctx, model, currencyField, validationResult, currencyList, -1);
			}
		}

		public bool Exists(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.Exists(ctx, pkID, includeDelete);
		}

		public bool ExistsByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.ExistsByFilter(ctx, filter);
		}

		public OperationResult InsertOrUpdate(MContext ctx, REGCurrencyModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<REGCurrencyModel> modelData, string fields = null)
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

		public REGCurrencyModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public REGCurrencyModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<REGCurrencyModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<REGCurrencyModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public OperationResult RemoveCurrency(MContext ctx, REGCurrencyModel model)
		{
			OperationResult operationResult = BDRepository.IsCanDelete(ctx, "Currency", model.MItemID);
			if (operationResult.Success)
			{
				operationResult = repository.DeleteCurrency(ctx, model);
			}
			return operationResult;
		}

		public REGCurrencyModel GetSingleCurrency(MContext ctx, REGCurrencyModel model)
		{
			throw new NotImplementedException();
		}

		public List<GlobalCurrencyModel> GetOrgCurrencyModel(MContext ctx)
		{
			return REGCurrencyRepository.GetOrgCurrencyModel(ctx);
		}
	}
}
