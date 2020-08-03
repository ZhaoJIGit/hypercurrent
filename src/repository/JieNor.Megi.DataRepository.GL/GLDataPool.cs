using JieNor.Megi.Core;
using JieNor.Megi.Core.MResource;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLDataPool : MResouceBase
	{
		private GLCommonDataModel data = null;

		private MContext ctx;

		private int year = 0;

		private int period = 0;

		private int periodDiff = 0;

		private string _emptyCheckGroupValueID;

		public List<BDAccountModel> AccountList
		{
			get
			{
				if (data.accountList == null)
				{
					List<BDAccountModel> accountListWithCheckType = new BDAccountRepository().GetAccountListWithCheckType(ctx, null, false, false);
					data.accountList = SetListValue(data.accountList, accountListWithCheckType);
				}
				return data.accountList;
			}
		}

		public List<BDAccountModel> AccountListWitchCheckType
		{
			get
			{
				if (data.accountListWithCheckType == null)
				{
					List<BDAccountModel> accountListWithCheckType = new BDAccountRepository().GetAccountListWithCheckType(ctx, null, true, false);
					data.accountListWithCheckType = SetListValue(data.accountListWithCheckType, accountListWithCheckType);
				}
				return data.accountListWithCheckType;
			}
		}

		public List<BDBankAccountEditModel> BankAccountList
		{
			get
			{
				if (data.bankAccountList == null)
				{
					data.bankAccountList = new BDBankAccountRepository().GetModelList(ctx, new SqlWhere(), false);
				}
				return data.bankAccountList;
			}
		}

		public List<REGCurrencyViewModel> CurrencyList
		{
			get
			{
				if (data.currencyList == null)
				{
					REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
					data.currencyList = rEGCurrencyRepository.GetAllCurrencyList(ctx, false);
					BASCurrencyViewModel @base = rEGCurrencyRepository.GetBase(ctx, false, null, null);
					if (@base != null)
					{
						REGCurrencyViewModel rEGCurrencyViewModel = new REGCurrencyViewModel();
						rEGCurrencyViewModel.MCurrencyID = @base.MCurrencyID;
						rEGCurrencyViewModel.MName = @base.MLocalName;
						rEGCurrencyViewModel.MNumber = @base.MCurrencyID;
						rEGCurrencyViewModel.MUserRate = "1";
						rEGCurrencyViewModel.MRate = "1";
						data.currencyList.Insert(0, rEGCurrencyViewModel);
					}
				}
				return data.currencyList;
			}
		}

		public List<GLVoucherModel> VoucherList
		{
			get
			{
				if (data.voucherList == null)
				{
					List<GLVoucherModel> voucherList = new GLVoucherRepository().GetVoucherList(ctx, new GLVoucherListFilterModel
					{
						Year = year,
						Period = period
					});
					data.voucherList = SetListValue(data.voucherList, voucherList);
				}
				return data.voucherList;
			}
		}

		public List<GLBalanceModel> BalanceList
		{
			get
			{
				if (data.balanceList == null)
				{
					List<GLBalanceModel> balanceListIncludeCheckGroupValue = new GLBalanceRepository().GetBalanceListIncludeCheckGroupValue(ctx, new SqlWhere().Equal("MYear", year).Equal("MPeriod", period), true);
					data.balanceList = SetListValue(data.balanceList, balanceListIncludeCheckGroupValue);
				}
				return data.balanceList;
			}
		}

		public List<GLBalanceModel> LeafAccountBalanceList
		{
			get
			{
				if (data.leftAccountBalanceList == null)
				{
					List<GLBalanceModel> list = new List<GLBalanceModel>();
					int i;
					for (i = 0; i < BalanceList.Count; i++)
					{
						if (AccountList.Exists((BDAccountModel x) => x.MItemID == BalanceList[i].MAccountID))
						{
							list.Add(BalanceList[i]);
						}
					}
					data.leftAccountBalanceList = SetListValue(data.leftAccountBalanceList, list);
				}
				return data.leftAccountBalanceList;
			}
		}

		public List<GLPeriodTransferModel> PeriodTransferList
		{
			get
			{
				if (data.periodTransferList == null)
				{
					data.periodTransferList = new GLPeriodTransferRepository().GetModelList(ctx, new SqlWhere().Equal("MYear", year).Equal("MPeriod", period), false);
				}
				return data.periodTransferList;
			}
		}

		public List<BDAccountModel> AccountWithParentList
		{
			get
			{
				if (data.accountWithParentList == null)
				{
					data.accountWithParentList = SetListValue(data.accountWithParentList, new BDAccountRepository().GetAccountListWithCheckType(ctx, null, true, false));
				}
				return data.accountWithParentList;
			}
		}

		public List<BDAccountModel> AccountIncludeDisable
		{
			get
			{
				if (data.accountIncludeDisable == null)
				{
					data.accountIncludeDisable = SetListValue(data.accountIncludeDisable, new BDAccountRepository().GetBaseBDAccountList(ctx, null, true, null));
				}
				return data.accountIncludeDisable;
			}
		}

		public List<BDAccountModel> AccountIncludeParentDisable
		{
			get
			{
				if (data.accountWithParentDisable == null)
				{
					data.accountWithParentDisable = new BDAccountRepository().GetAccountListWithCheckType(ctx, null, true, true);
				}
				return data.accountWithParentDisable;
			}
		}

		public List<BDItemModel> MerItemList
		{
			get
			{
				if (data.itemList == null || data.itemList.Count == 0)
				{
					string format = "SELECT \r\n                        t1.*,\r\n                        t2.MDesc\r\n                        FROM\r\n                        t_bd_item t1\r\n\t                        INNER JOIN\r\n                        t_bd_item_l t2 ON \r\n\t                        t1.MOrgID = t2.MOrgID\r\n\t                        AND t1.MItemID = t2.MParentID\r\n\t                        AND t2.MIsDelete = t1.MIsDelete\r\n                        WHERE\r\n                        t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 AND t2.MLocaleID = @MLocaleID\r\n                        ORDER BY MName";
					format = string.Format(format, "JieNor-001");
					data.itemList = SetListValue(data.itemList, ModelInfoManager.GetDataModelBySql<BDItemModel>(ctx, format, ctx.GetParameters((MySqlParameter)null)));
				}
				return data.itemList;
			}
		}

		public List<BDItemModel> MerItemNumberList
		{
			get
			{
				if (data.itemNumberList == null)
				{
					string format = "SELECT \r\n                        t1.MItemID,\r\n                        t1.MNumber\r\n                        FROM\r\n                        t_bd_item t1\r\n\t                        INNER JOIN\r\n                        t_bd_item_l t2 ON \r\n\t                        t1.MOrgID = t2.MOrgID\r\n\t                        AND t1.MItemID = t2.MParentID\r\n\t                        AND t2.MIsDelete = t1.MIsDelete\r\n                        WHERE\r\n                        t1.MOrgID = @MOrgID AND t1.MIsDelete = 0 AND t2.MLocaleID = @MLocaleID";
					format = string.Format(format, "JieNor-001");
					data.itemNumberList = SetListValue(data.itemNumberList, ModelInfoManager.GetDataModelBySql<BDItemModel>(ctx, format, ctx.GetParameters((MySqlParameter)null)));
				}
				return data.itemNumberList;
			}
		}

		public List<BDExpenseItemModel> ExpenseItemList
		{
			get
			{
				if (data.expeneseItemList == null)
				{
					data.expeneseItemList = SetListValue(data.expeneseItemList, new BDExpenseItemRepository().GetModelList(ctx, new SqlWhere(), false));
				}
				return data.expeneseItemList;
			}
		}

		public List<BDExpenseItemModel> ExpenseItemNameList
		{
			get
			{
				if (data.expenseItemNameList == null)
				{
					string sql = "SELECT \n                        p.MItemID, l.MName\n                    FROM\n                        T_BD_ExpenseItem p\n                            INNER JOIN\n                        T_BD_ExpenseItem_l l ON p.MOrgID = l.MOrgID\n                            AND p.MItemID = l.mparentid\n                            AND l.MLocaleID = @MLocaleID\n                            AND p.MIsDelete = l.MIsDelete\n                    WHERE\n                        p.morgid = @MOrgID\n                            AND p.MIsDelete = 0;";
					data.expenseItemNameList = SetListValue(data.expenseItemNameList, ModelInfoManager.GetDataModelBySql<BDExpenseItemModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null)));
				}
				return data.expenseItemNameList;
			}
		}

		public List<REGTaxRateModel> TaxRateList
		{
			get
			{
				if (data.taxRateList == null)
				{
					data.taxRateList = SetListValue(data.taxRateList, new REGTaxRateRepository().GetModelList(ctx, new SqlWhere(), false));
				}
				return data.taxRateList;
			}
		}

		public List<REGTaxRateModel> TaxRateTaxAccountList
		{
			get
			{
				if (data.TaxRateTaxAccountList == null)
				{
					string sql = "SELECT \r\n                        t.MItemID,\r\n                        t.MSaleTaxAccountCode\r\n                        FROM\r\n                        T_REG_TaxRate t\r\n                        WHERE\r\n                        t.MOrgID = @MOrgID AND t.MIsDelete = 0";
					data.TaxRateTaxAccountList = SetListValue(data.TaxRateTaxAccountList, ModelInfoManager.GetDataModelBySql<REGTaxRateModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null)));
				}
				return data.TaxRateTaxAccountList;
			}
		}

		public List<BDContactsModel> ContactList
		{
			get
			{
				if (data.contactList == null)
				{
					string format = "SELECT \r\n                            t1.MItemID,\r\n                            CONVERT( AES_DECRYPT(t2.MName, '{0}') USING UTF8) AS MName,\r\n                            t1.MCCurrentAccountCode,\r\n                            MIsCustomer,\r\n                            MIsSupplier,\r\n                            MIsOther,\r\n                            t1.MCreateDate,\r\n                            t1.MModifyDate,\r\n                            t1.MIsActive\r\n                        FROM\r\n                            T_BD_Contacts t1\r\n                                INNER JOIN\r\n                            T_BD_Contacts_l t2 ON t1.MOrgID = t2.MOrgID\r\n                                AND t1.MItemID = t2.MParentID\r\n                                AND t2.MLocaleID = @MLocaleID\r\n                                AND t2.MIsDelete = t1.MIsDelete\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\r\n                        ORDER BY MName";
					format = string.Format(format, "JieNor-001");
					data.contactList = SetListValue(data.contactList, ModelInfoManager.GetDataModelBySql<BDContactsModel>(ctx, format, ctx.GetParameters((MySqlParameter)null)));
				}
				return data.contactList;
			}
		}

		public Hashtable MCheckGroupValue
		{
			get
			{
				if (data.checkGroupValue == null)
				{
					Hashtable hashtable = new Hashtable();
					string sql = " SELECT \n                                    MItemID,\n                                    MOrgID,\n                                    MContactID,\n                                    MEmployeeID,\n                                    MMerItemID,\n                                    MExpItemID,\n                                    MPaItemID,\n                                    MTrackItem1,\n                                    MTrackItem2,\n                                    MTrackItem3,\n                                    MTrackItem4,\n                                    MTrackItem5\n                                FROM\n                                    t_gl_checkgroupvalue\n                                WHERE MOrgID = @MOrgID and MIsDelete = 0";
					List<GLCheckGroupValueModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<GLCheckGroupValueModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
					if (dataModelBySql?.Any() ?? false)
					{
						for (int i = 0; i < dataModelBySql.Count; i++)
						{
							int hashCode = dataModelBySql[i].GetHashCode();
							if (!hashtable.ContainsKey(hashCode))
							{
								hashtable[hashCode] = dataModelBySql[i];
							}
						}
					}
					if (data.checkGroupValue == null)
					{
						data.checkGroupValue = hashtable;
					}
				}
				return data.checkGroupValue;
			}
			private set
			{
			}
		}

		public List<BDContactsTrackLinkModel> MContactsTrackLink
		{
			get
			{
				if (data.MContactsTrackLink == null)
				{
					string sql = "SELECT \n                                    t1.*\n                                FROM\n                                    t_bd_contactstracklink t1\n                                        INNER JOIN\n                                    t_bd_contacts t2 ON t1.MContactID = t2.MItemID\n                                        AND t1.MOrgID = t2.MOrgID\n                                        AND t1.MIsDelete = t2.MIsDelete\n                                        INNER JOIN\n                                    t_bd_track t3 ON t3.MOrgID = t1.MOrgID\n                                        AND t3.MItemID = t1.MTrackID\n                                        AND t3.MIsDelete = t1.MIsDelete\n                                WHERE\n                                    t1.MOrgID = @MOrgID AND t1.MIsDelete = 0";
					data.MContactsTrackLink = SetListValue(data.MContactsTrackLink, ModelInfoManager.GetDataModelBySql<BDContactsTrackLinkModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null)));
				}
				return data.MContactsTrackLink;
			}
		}

		public List<BDEmployeesModel> EmployeeList
		{
			get
			{
				if (data.employeeList == null)
				{
					string format = "SELECT \r\n                        t1.*,\r\n                         F_GetUserName( t2.MFirstName, t2.MLastName) AS MFullName\r\n                    FROM\r\n                        t_bd_employees t1\r\n                            INNER JOIN\r\n                        t_bd_employees_l t2 ON \r\n\t\t                    t1.MOrgID = t2.MOrgID\r\n                            AND t1.MItemID = t2.MParentID\r\n                            AND t2.MLocaleID = @MLocaleID\r\n                            AND t2.MIsDelete = t1.MIsDelete\r\n                    WHERE\r\n                        t1.MOrgID = @MorgID AND t1.MIsDelete = 0\r\n                    ORDER BY MFullName";
					format = string.Format(format, "JieNor-001");
					data.employeeList = SetListValue(data.employeeList, ModelInfoManager.GetDataModelBySql<BDEmployeesModel>(ctx, format, ctx.GetParameters((MySqlParameter)null)));
				}
				return data.employeeList;
			}
		}

		public List<BDTrackModel> TrackList
		{
			get
			{
				if (data.trackList == null)
				{
					data.trackList = SetListValue(data.trackList, new BDTrackRepository().GetTrackNameList(ctx, false));
				}
				return data.trackList;
			}
		}

		public List<PAPayItemModel> PayitemList
		{
			get
			{
				if (data.payitemList == null)
				{
					data.payitemList = SetListValue(data.payitemList, new PAPayItemRepository().GetModelList(ctx, new SqlWhere(), false));
				}
				return data.payitemList;
			}
		}

		public string EmptyCheckGroupValueID
		{
			get
			{
				if (string.IsNullOrWhiteSpace(_emptyCheckGroupValueID))
				{
					_emptyCheckGroupValueID = new GLUtility().GetCheckGroupValueModel(ctx, new GLCheckGroupValueModel()).MItemID;
				}
				return _emptyCheckGroupValueID;
			}
		}

		public List<BDVoucherSettingCategoryModel> VoucherSettingCategoryList
		{
			get
			{
				if (data.VoucherSettingCategoryList == null)
				{
					data.VoucherSettingCategoryList = SetListValue(data.VoucherSettingCategoryList, new BDVoucherSettingRepository().GetVoucherSettingCategoryList(ctx));
				}
				return data.VoucherSettingCategoryList;
			}
		}

		public List<BDCheckInactiveModel> BDInactiveList
		{
			get
			{
				if (data.BDInactiveList == null)
				{
					data.BDInactiveList = SetListValue(data.BDInactiveList, BDRepository.GetBDInactiveLists(ctx));
				}
				return data.BDInactiveList;
			}
		}

		public List<GLSettlementModel> ClosedPeriods
		{
			get
			{
				if (data.ClosedPeriods == null)
				{
					data.ClosedPeriods = new GLSettlementRepository().GetClosedPeriodList(ctx);
				}
				return data.ClosedPeriods;
			}
		}

		public List<GLCheckGroupValueModel> CheckGroupValueList
		{
			get
			{
				if (data.CheckGroupValueList == null)
				{
					data.CheckGroupValueList = new GLVoucherRepository().GetCheckGroupValueList(ctx);
				}
				return data.CheckGroupValueList;
			}
		}

		public List<BDTrackModel> TrackListWithEntry
		{
			get
			{
				if (data.trackListWithEntry == null)
				{
					List<GLTreeModel> trackItemList = new GLUtility().GetTrackItemList(ctx, true);
					List<BDTrackModel> list = new List<BDTrackModel>();
					data.trackListWithEntry = (data.trackListWithEntry ?? new List<BDTrackModel>());
					int num = 0;
					while (true)
					{
						int num2 = num;
						int? nullable = (trackItemList != null) ? new int?(trackItemList.Count) : null;
						if (num2 < nullable.GetValueOrDefault() & nullable.HasValue)
						{
							BDTrackModel track = new BDTrackModel
							{
								MItemID = trackItemList[num].id,
								MName = trackItemList[num].text,
								MEntryList = new List<BDTrackEntryModel>()
							};
							trackItemList[num]?.children?.ForEach(delegate(GLTreeModel x)
							{
								track.MEntryList.Add(new BDTrackEntryModel
								{
									MEntryID = x.id,
									MName = x.text,
									MItemID = track.MItemID
								});
							});
							data.trackListWithEntry.Add(track);
							num++;
							continue;
						}
						break;
					}
				}
				return data.trackListWithEntry;
			}
		}

		private GLDataPool(MContext c, int y = 0, int p = 0, int d = 0)
		{
			ctx = c;
			year = y;
			period = p;
			periodDiff = ((d == 0) ? 6 : d);
			data = new GLCommonDataModel();
		}

		public static GLDataPool GetInstance(MContext ctx, bool tokenEqual = false, int year = 0, int period = 0, int periodDiff = 0)
		{
			if (!tokenEqual)
			{
				object obj = MResourceHelper.GLDataPoolCache[ctx];
				if (obj != null)
				{
					return obj as GLDataPool;
				}
			}
			else
			{
				object obj2 = MResourceHelper.GLDataPoolCacheByToken[ctx.MAccessToken];
				if (obj2 != null)
				{
					GLDataPool gLDataPool = obj2 as GLDataPool;
					gLDataPool.ModifyTime = DateTime.Now;
					MResourceHelper.GLDataPoolCacheByToken[ctx.MAccessToken] = gLDataPool;
					return gLDataPool;
				}
			}
			GLDataPool gLDataPool2 = new GLDataPool(ctx, year, period, periodDiff);
			if (tokenEqual)
			{
				MResourceHelper.GLDataPoolCacheByToken[ctx.MAccessToken] = gLDataPool2;
			}
			else
			{
				MResourceHelper.GLDataPoolCache[ctx] = gLDataPool2;
			}
			return gLDataPool2;
		}

		public static void RemovePool(MContext ctx, bool equalToken = false)
		{
			if (ctx != null)
			{
				MResourceHelper.GLDataPoolCache.Remove(ctx);
				if (!string.IsNullOrEmpty(ctx.MAccessToken))
				{
					MResourceHelper.GLDataPoolCacheByToken.Remove(ctx.MAccessToken);
				}
			}
		}

		public void InitLastBalanceList(int periodDiff)
		{
			if (periodDiff > 0)
			{
				data.lastBalanceList = new List<KeyValuePair<int, List<GLBalanceModel>>>();
				for (int num = periodDiff - 1; num > 0; num--)
				{
					List<GLBalanceModel> balanceListIncludeCheckGroupValue = new GLBalanceRepository().GetBalanceListIncludeCheckGroupValue(ctx, new SqlWhere().Equal("MYearPeriod", year * 100 + period - num), true);
					data.lastBalanceList.Add(new KeyValuePair<int, List<GLBalanceModel>>(year * 12 + periodDiff - num, balanceListIncludeCheckGroupValue));
				}
			}
		}

		public List<KeyValuePair<int, List<GLBalanceModel>>> GetLastBalanceList(int periodDiff)
		{
			if ((data.lastBalanceList == null || data.lastBalanceList.Count == 0) && periodDiff != 0)
			{
				InitLastBalanceList(periodDiff);
			}
			List<KeyValuePair<int, List<GLBalanceModel>>> list = new List<KeyValuePair<int, List<GLBalanceModel>>>();
			int i;
			for (i = periodDiff - 1; i > 0; i--)
			{
				if (data.lastBalanceList.Exists((KeyValuePair<int, List<GLBalanceModel>> x) => x.Key == year * 12 + periodDiff - i))
				{
					list.Add(data.lastBalanceList.FirstOrDefault((KeyValuePair<int, List<GLBalanceModel>> x) => x.Key == year * 12 + periodDiff - i));
				}
			}
			return list;
		}

		public List<BDExchangeRateViewModel> GetExchanageRateList(string currencyID)
		{
			data.exchangeRateList = (data.exchangeRateList ?? new List<KeyValuePair<string, List<BDExchangeRateViewModel>>>());
			KeyValuePair<string, List<BDExchangeRateViewModel>> item = data.exchangeRateList.FirstOrDefault((KeyValuePair<string, List<BDExchangeRateViewModel>> x) => x.Key == currencyID);
			if (string.IsNullOrWhiteSpace(item.Key))
			{
				item = new KeyValuePair<string, List<BDExchangeRateViewModel>>(currencyID, new BDExchangeRateRepository().GetExchangeRateViewList(new BDExchangeRateFilterModel
				{
					MSourceCurrencyID = ctx.MBasCurrencyID,
					MTargetCurrencyID = currencyID
				}, ctx).rows);
				data.exchangeRateList.Add(item);
			}
			return item.Value;
		}

		private static List<T> SetListValue<T>(List<T> src, List<T> dest)
		{
			if (src == null || src.Count == 0)
			{
				src = dest;
			}
			return src;
		}

		public List<GLSimpleVoucherModel> GetSimpleVouchersByPeriods(List<int> periods)
		{
			if (data.SimpleVouchers == null)
			{
				data.SimpleVouchers = new GLVoucherRepository().GetSimpleVouchersByPeriods(ctx, periods);
			}
			return data.SimpleVouchers;
		}

		public List<GLSimpleVoucherModel> GetSimpleVouchersByDocIDs(List<string> ids)
		{
			if (data.SimpleVouchers == null)
			{
				data.SimpleVouchers = new GLVoucherRepository().GetSimpleVouchersByDocIDs(ctx, ids);
			}
			return data.SimpleVouchers;
		}

		public GLCheckGroupValueModel GetCheckGroupValueModel(GLCheckGroupValueModel value)
		{
			value = (value ?? new GLCheckGroupValueModel());
			value.MContactID = ConvertEmpty2Null(value.MContactID);
			value.MEmployeeID = ConvertEmpty2Null(value.MEmployeeID);
			value.MMerItemID = ConvertEmpty2Null(value.MMerItemID);
			value.MExpItemID = ConvertEmpty2Null(value.MExpItemID);
			value.MPaItemID = ConvertEmpty2Null(value.MPaItemID);
			value.MTrackItem1 = ConvertEmpty2Null(value.MTrackItem1);
			value.MTrackItem2 = ConvertEmpty2Null(value.MTrackItem2);
			value.MTrackItem3 = ConvertEmpty2Null(value.MTrackItem3);
			value.MTrackItem4 = ConvertEmpty2Null(value.MTrackItem4);
			value.MTrackItem5 = ConvertEmpty2Null(value.MTrackItem5);
			GLCheckGroupValueModel gLCheckGroupValueModel = CheckGroupValueList.FirstOrDefault((GLCheckGroupValueModel x) => ConvertEmpty2Null(x.MContactID) == ConvertEmpty2Null(value.MContactID) && ConvertEmpty2Null(x.MEmployeeID) == ConvertEmpty2Null(value.MEmployeeID) && ConvertEmpty2Null(x.MMerItemID) == ConvertEmpty2Null(value.MMerItemID) && ConvertEmpty2Null(x.MExpItemID) == ConvertEmpty2Null(value.MExpItemID) && ConvertEmpty2Null(x.MPaItemID) == ConvertEmpty2Null(value.MPaItemID) && ConvertEmpty2Null(x.MTrackItem1) == ConvertEmpty2Null(value.MTrackItem1) && ConvertEmpty2Null(x.MTrackItem2) == ConvertEmpty2Null(value.MTrackItem2) && ConvertEmpty2Null(x.MTrackItem3) == ConvertEmpty2Null(value.MTrackItem3) && ConvertEmpty2Null(x.MTrackItem4) == ConvertEmpty2Null(value.MTrackItem4) && ConvertEmpty2Null(x.MTrackItem5) == ConvertEmpty2Null(value.MTrackItem5));
			if (gLCheckGroupValueModel == null)
			{
				value.IsNew = true;
				value.MItemID = UUIDHelper.GetGuid();
				data.CheckGroupValueList = (data.CheckGroupValueList ?? new List<GLCheckGroupValueModel>());
				data.CheckGroupValueList.Add(value);
				return value;
			}
			return gLCheckGroupValueModel;
		}

		private static string ConvertEmpty2Null(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? null : value;
		}
	}
}
