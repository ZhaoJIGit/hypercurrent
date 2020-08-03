using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JieNor.Megi.DataModel.GL
{
	public class GLCommonDataModel
	{
		public List<BDAccountModel> accountList
		{
			get;
			set;
		}

		public List<BDAccountModel> accountWithParentList
		{
			get;
			set;
		}

		public List<BDAccountModel> accountIncludeDisable
		{
			get;
			set;
		}

		public List<BDAccountModel> accountListWithCheckType
		{
			get;
			set;
		}

		public List<BDAccountModel> accountWithParentDisable
		{
			get;
			set;
		}

		public List<BDBankAccountEditModel> bankAccountList
		{
			get;
			set;
		}

		public DateTime lastAccountModifyDate
		{
			get;
			set;
		}

		public List<REGCurrencyViewModel> currencyList
		{
			get;
			set;
		}

		public List<BDItemModel> itemList
		{
			get;
			set;
		}

		public List<BDItemModel> itemNumberList
		{
			get;
			set;
		}

		public DateTime lastItemModifyDate
		{
			get;
			set;
		}

		public List<BDExpenseItemModel> expeneseItemList
		{
			get;
			set;
		}

		public List<BDExpenseItemModel> expenseItemNameList
		{
			get;
			set;
		}

		public DateTime lastExpenseModifyDate
		{
			get;
			set;
		}

		public List<REGTaxRateModel> taxRateList
		{
			get;
			set;
		}

		public List<REGTaxRateModel> TaxRateTaxAccountList
		{
			get;
			set;
		}

		public DateTime lastTaxRateModifyDate
		{
			get;
			set;
		}

		public List<BDContactsModel> contactList
		{
			get;
			set;
		}

		public List<BDContactsTrackLinkModel> MContactsTrackLink
		{
			get;
			set;
		}

		public DateTime lastContactModifyDate
		{
			get;
			set;
		}

		public List<BDEmployeesModel> employeeList
		{
			get;
			set;
		}

		public DateTime lastEmployeeModifyDate
		{
			get;
			set;
		}

		public List<PAPayItemModel> payitemList
		{
			get;
			set;
		}

		public List<BDTrackModel> trackList
		{
			get;
			set;
		}

		public List<BDTrackModel> trackListWithEntry
		{
			get;
			set;
		}

		public DateTime lastTrackModifyDate
		{
			get;
			set;
		}

		public List<KeyValuePair<string, List<BDExchangeRateViewModel>>> exchangeRateList
		{
			get;
			set;
		}

		public DateTime lastExchangRateModifyDate
		{
			get;
			set;
		}

		public List<GLVoucherModel> voucherList
		{
			get;
			set;
		}

		public List<GLBalanceModel> balanceList
		{
			get;
			set;
		}

		public List<GLBalanceModel> leftAccountBalanceList
		{
			get;
			set;
		}

		public List<GLPeriodTransferModel> periodTransferList
		{
			get;
			set;
		}

		public List<KeyValuePair<int, List<GLBalanceModel>>> lastBalanceList
		{
			get;
			set;
		}

		public List<BDVoucherSettingCategoryModel> VoucherSettingCategoryList
		{
			get;
			set;
		}

		public Hashtable checkGroupValue
		{
			get;
			set;
		}

		public List<BDCheckInactiveModel> BDInactiveList
		{
			get;
			set;
		}

		public List<GLSettlementModel> ClosedPeriods
		{
			get;
			set;
		}

		public List<GLSimpleVoucherModel> SimpleVouchers
		{
			get;
			set;
		}

		public List<GLCheckGroupValueModel> CheckGroupValueList
		{
			get;
			set;
		}
	}
}
