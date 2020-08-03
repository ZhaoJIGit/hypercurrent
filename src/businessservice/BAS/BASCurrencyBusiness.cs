using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.BAS
{
	public class BASCurrencyBusiness : IBASCurrencyBusiness, IDataContract<BASCurrencyModel>
	{
		private readonly BASCurrencyRepository dal = new BASCurrencyRepository();

		public List<BASCurrencyModel> GetList(MContext context)
		{
			return dal.GetList(context);
		}

		public BASCurrencyModel GetModel(MContext ctx, BASCurrencyModel model)
		{
			return dal.GetModel(model);
		}

		public BASCurrencyViewModel GetViewModel(MContext ctx, BASCurrencyModel model)
		{
			return dal.GetViewModel(ctx, model);
		}

		public List<BASCurrencyViewModel> GetViewList(MContext context, bool flag)
		{
			List<BASCurrencyViewModel> list = dal.GetViewList(context);
			if (list != null && !flag)
			{
				BASCurrencyViewModel model = new REGCurrencyRepository().GetBase(context, false, null, null);
				IEnumerable<BASCurrencyViewModel> enumerable = from x in list
				where x.MCurrencyID != model.MCurrencyID
				select x;
				list = ((enumerable != null) ? enumerable.ToList() : new List<BASCurrencyViewModel>());
			}
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

		public OperationResult InsertOrUpdate(MContext ctx, BASCurrencyModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<BASCurrencyModel> modelData, string fields = null)
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

		public BASCurrencyModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public BASCurrencyModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<BASCurrencyModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<BASCurrencyModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}
	}
}
