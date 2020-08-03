using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.Mongo;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.API
{
	public class APIDataPool
	{
		private MContext ctx;

		private bool IsCacheData = true;

		public List<BDAccountEditModel> Accounts => GetBaseData<BDAccountEditModel>(ctx, null);

		public List<GLCheckGroupModel> CheckGroups => GetBaseData<GLCheckGroupModel>(ctx, null);

		public List<GLCheckGroupValueModel> CheckGroupValues => GetBaseData<GLCheckGroupValueModel>(ctx, null);

		public List<BDBankAccountModel> Banks => GetBaseData<BDBankAccountModel>(ctx, null);

		public List<BDBankTypeModel> BankTypes => GetBaseData<BDBankTypeModel>(ctx, null);

		public List<BDContactsTypeModel> ContactGroups => GetBaseData<BDContactsTypeModel>(ctx, null);

		public List<BDContactsTypeLinkModel> ContactTypeLinks => GetBaseData<BDContactsTypeLinkModel>(ctx, null);

		public List<BDContactsInfoModel> Contacts => GetBaseData<BDContactsInfoModel>(ctx, null);

		public List<BDEmployeesModel> Employees => GetBaseData<BDEmployeesModel>(ctx, null);

		public List<BDItemModel> Items => GetBaseData<BDItemModel>(ctx, null);

		public List<BDExpenseItemModel> ExpenseItems => GetBaseData<BDExpenseItemModel>(ctx, null);

		public List<REGCurrencyViewModel> Currencies => GetBaseData<REGCurrencyViewModel>(ctx, null);

		public List<REGTaxRateModel> TaxRates => GetBaseData<REGTaxRateModel>(ctx, null);

		public List<BDTrackModel> TrackingCategories => (from f in GetBaseData<BDTrackModel>(ctx, null)
		orderby f.MCreateDate
		select f).ToList();

		public List<BDContactsTrackLinkModel> ContactTrackLinks => GetBaseData<BDContactsTrackLinkModel>(ctx, null);

		public List<BASCurrencyModel> BASCurrencies => GetBaseData<BASCurrencyModel>(ctx, null);

		public List<PAPayItemModel> PayItems => GetBaseData<PAPayItemModel>(ctx, null);

		public List<PAPayItemGroupModel> PayItemGroups => GetBaseData<PAPayItemGroupModel>(ctx, null);

		private APIDataPool(MContext context)
		{
			ctx = context;
		}

		public static APIDataPool GetInstance(MContext ctx)
		{
			return new APIDataPool(ctx);
		}

		public dynamic GetBaseDataByModel(MContext ctx, Type type)
		{
			if (type == typeof(BDAccountEditModel))
			{
				return GetBaseData<BDAccountEditModel>(ctx, null);
			}
			if (type == typeof(BDContactsTrackLinkModel))
			{
				return GetBaseData<BDContactsTrackLinkModel>(ctx, null);
			}
			if (type == typeof(BDOrganisationModel))
			{
				return GetBaseData<BDOrganisationModel>(ctx, null);
			}
			if (type == typeof(BDBankAccountModel))
			{
				return GetBaseData<BDBankAccountModel>(ctx, null);
			}
			if (type == typeof(BDBankTypeModel))
			{
				return GetBaseData<BDBankTypeModel>(ctx, null);
			}
			if (type == typeof(BDContactsInfoModel))
			{
				return GetBaseData<BDContactsInfoModel>(ctx, null);
			}
			if (type == typeof(BDContactsTypeModel))
			{
				return GetBaseData<BDContactsTypeModel>(ctx, null);
			}
			if (type == typeof(BDEmployeesModel))
			{
				return GetBaseData<BDEmployeesModel>(ctx, null);
			}
			if (type == typeof(BDItemModel))
			{
				return GetBaseData<BDItemModel>(ctx, null);
			}
			if (type == typeof(BDTrackModel))
			{
				return GetBaseData<BDTrackModel>(ctx, null);
			}
			if (type == typeof(REGCurrencyViewModel))
			{
				return GetBaseData<REGCurrencyViewModel>(ctx, null);
			}
			if (type == typeof(REGTaxRateModel))
			{
				return GetBaseData<REGTaxRateModel>(ctx, null);
			}
			if (type == typeof(GLCheckGroupModel))
			{
				return GetBaseData<GLCheckGroupModel>(ctx, null);
			}
			if (type == typeof(GLCheckGroupValueModel))
			{
				return GetBaseData<GLCheckGroupValueModel>(ctx, null);
			}
			return null;
		}

		public List<T> GetBaseData<T>(MContext ctx, T model = null) where T : BaseModel
		{
			List<T> list = MongoBDHelper.FindAll<T>(ctx);
			if (list == null || list.Count == 0 || !IsCacheData)
			{
				list = GetCommonBaseDataInDatabase<T>(ctx);
				SetCommonBaseData(ctx, list, true);
			}
			return list;
		}

		private List<BDTrackModel> GetTrackCategoriesInDatabase(MContext ctx, bool multiLang = true)
		{
			List<BDTrackModel> tracks = APIDataRepository.GetBaseDataInDatabase<BDTrackModel>(ctx, null);
			List<BDTrackEntryModel> baseDataInDatabase = APIDataRepository.GetBaseDataInDatabase<BDTrackEntryModel>(ctx, null);
			int i;
			for (i = 0; i < tracks.Count; i++)
			{
				tracks[i].MEntryList = (from x in baseDataInDatabase
				where x.MItemID == tracks[i].MItemID
				select x).ToList();
			}
			return tracks;
		}

		private List<T> GetCommonBaseDataInDatabase<T>(MContext ctx) where T : BaseModel
		{
			if (typeof(T) == typeof(BDTrackModel))
			{
				return GetTrackCategoriesInDatabase(ctx, true) as List<T>;
			}
			return APIDataRepository.GetBaseDataInDatabase<T>(ctx, null);
		}

		private void SetCommonBaseData<T>(MContext ctx, List<T> data, bool multiLang = true) where T : BaseModel
		{
			if (data != null && data.Any() && typeof(T) == typeof(BDContactsInfoModel))
			{
				data.ForEach((Action<T>)delegate(T x)
				{
					(x as BDContactsInfoModel).SetPaymentTerms();
				});
			}
			MongoBDHelper.Insert(ctx, data);
		}
	}
}
