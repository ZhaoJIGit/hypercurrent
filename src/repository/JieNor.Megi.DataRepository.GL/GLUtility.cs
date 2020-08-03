using Fasterflect;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataMode;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.BD.InitDocument;
using JieNor.Megi.DataModel.Enum;
using JieNor.Megi.DataModel.FA;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.DataModel.FP;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.FA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLUtility
	{
		public const char SplitChar = '·';

		public GLCheckTypeDataModel GetContactCheckTypeData(MContext ctx)
		{
			string format = "SELECT\r\n                            t1.MItemID,\r\n                            CONVERT( AES_DECRYPT(t2.MName, '{0}') USING UTF8) AS MName,\r\n                            MIsCustomer,\r\n                            MIsSupplier,\r\n                            MIsOther,\r\n                            t1.MIsActive\r\n                        FROM\r\n                            T_BD_Contacts t1\r\n                                INNER JOIN\r\n                            T_BD_Contacts_l t2 ON t1.MOrgID = t2.MOrgID\r\n                                AND t1.MItemID = t2.MParentID\r\n                                AND t2.MLocaleID = @MLocaleID\r\n                                AND t2.MIsDelete = t1.MIsDelete\r\n                        WHERE\r\n                            t1.MOrgID = @MorgID AND t1.MIsDelete = 0\r\n                        ORDER BY MName";
			format = string.Format(format, "JieNor-001");
			List<BDContactsModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDContactsModel>(ctx, format, ctx.GetParameters((MySqlParameter)null));
			GLTreeModel item = new GLTreeModel
			{
				id = "Customer",
				text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Customer", "客户"),
				MIsActive = true,
				children = (from x in dataModelBySql
				where x.MIsCustomer
				select new GLTreeModel
				{
					id = x.MItemID,
					parentId = "Customer",
					text = x.MName,
					MIsActive = x.MIsActive
				}).ToList()
			};
			GLTreeModel item2 = new GLTreeModel
			{
				id = "Supplier",
				text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Supplier", "供应商"),
				MIsActive = true,
				children = (from x in dataModelBySql
				where x.MIsSupplier
				select new GLTreeModel
				{
					id = x.MItemID,
					parentId = "Supplier",
					text = x.MName,
					MIsActive = x.MIsActive
				}).ToList()
			};
			GLTreeModel item3 = new GLTreeModel
			{
				id = "Other",
				text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Other", "Other"),
				MIsActive = true,
				children = (from x in dataModelBySql
				where x.MIsOther
				select new GLTreeModel
				{
					id = x.MItemID,
					parentId = "Other",
					text = x.MName,
					MIsActive = x.MIsActive
				}).ToList()
			};
			List<GLTreeModel> mDataList = new List<GLTreeModel>
			{
				item,
				item2,
				item3
			};
			return new GLCheckTypeDataModel
			{
				MCheckType = 0,
				MCheckTypeColumnName = GetCheckTypeColumnName(0),
				MCheckTypeName = GetCheckTypeName(ctx, 0),
				MDataList = mDataList,
				MShowType = 2
			};
		}

		public GLCheckTypeDataModel GetEmployeeCheckTypeData(MContext ctx)
		{
			string sql = "SELECT\r\n                        t1.MItemID,\r\n                         F_GetUserName( t2.MFirstName, t2.MLastName) AS MFullName,\r\n                        t1.MIsActive\r\n                    FROM\r\n                        t_bd_employees t1\r\n                            INNER JOIN\r\n                        t_bd_employees_l t2 ON\r\n\t\t                    t1.MOrgID = t2.MOrgID\r\n                            AND t1.MItemID = t2.MParentID\r\n                            AND t2.MLocaleID = @MLocaleID\r\n                            AND t2.MIsDelete = t1.MIsDelete\r\n                    WHERE\r\n                        t1.MOrgID = @MorgID AND t1.MIsDelete = 0\r\n                    ORDER BY MFullName";
			List<BDEmployeesModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDEmployeesModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
			List<GLTreeModel> mDataList = (from x in dataModelBySql
			select new GLTreeModel
			{
				id = x.MItemID,
				parentId = "0",
				text = x.MFullName,
				MIsActive = x.MIsActive
			}).ToList();
			return new GLCheckTypeDataModel
			{
				MCheckType = 1,
				MCheckTypeColumnName = GetCheckTypeColumnName(1),
				MCheckTypeName = GetCheckTypeName(ctx, 1),
				MDataList = mDataList,
				MShowType = 0
			};
		}

		public GLCheckTypeDataModel GetAllEmployeeCheckTypeData(MContext ctx)
		{
			string sql = "SELECT\r\n                        t1.MItemID,\r\n                         F_GetUserName( t2.MFirstName, t2.MLastName) AS MFullName,\r\n                        t1.MIsActive\r\n                    FROM\r\n                        t_bd_employees t1\r\n                            INNER JOIN\r\n                        t_bd_employees_l t2 ON\r\n\t\t                    t1.MOrgID = t2.MOrgID\r\n                            AND t1.MItemID = t2.MParentID\r\n                            AND t2.MIsDelete = t1.MIsDelete\r\n                    WHERE\r\n                        t1.MOrgID = @MorgID AND t1.MIsDelete = 0\r\n                    ORDER BY MFullName";
			List<BDEmployeesModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDEmployeesModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
			List<GLTreeModel> mDataList = (from x in dataModelBySql
			select new GLTreeModel
			{
				id = x.MItemID,
				parentId = "0",
				text = x.MFullName,
				MIsActive = x.MIsActive
			}).ToList();
			return new GLCheckTypeDataModel
			{
				MCheckType = 1,
				MCheckTypeColumnName = GetCheckTypeColumnName(1),
				MCheckTypeName = GetCheckTypeName(ctx, 1),
				MDataList = mDataList,
				MShowType = 0
			};
		}

		public GLCheckTypeDataModel GetMerItemCheckTypeData(MContext ctx)
		{
			string sql = "SELECT\r\n                t1.MItemID,\r\n                t2.MDesc as MName,\r\n                t1.MNumber,\r\n                t1.MIsActive\r\n                FROM\r\n                t_bd_item t1\r\n\t                INNER JOIN\r\n                t_bd_item_l t2 ON\r\n\t                t1.MOrgID = t2.MOrgID\r\n\t                AND t1.MItemID = t2.MParentID\r\n\t                AND t2.MIsDelete = t1.MIsDelete\r\n                WHERE\r\n                t1.MOrgID = @MOrgID AND t1.MIsDelete = 0  AND t2.MLocaleID = @MLocaleID\r\n                ORDER BY MName";
			List<BDItemModel> dataModelBySql = ModelInfoManager.GetDataModelBySql<BDItemModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
			List<GLTreeModel> mDataList = (from x in dataModelBySql
			select new GLTreeModel
			{
				id = x.MItemID,
				parentId = "0",
				text = $"{x.MNumber}:{x.MName}",
				MIsActive = x.MIsActive
			}).ToList();
			return new GLCheckTypeDataModel
			{
				MCheckType = 2,
				MCheckTypeColumnName = GetCheckTypeColumnName(2),
				MCheckTypeName = GetCheckTypeName(ctx, 2),
				MDataList = mDataList,
				MShowType = 0
			};
		}

		public GLCheckTypeDataModel GetExpItemCheckTypeData(MContext ctx)
		{
			string sql = "SELECT\r\n                               t1.MItemID , t3.MName , t2.MItemID MEntryID, ltrim(t4.MName) MEntryName, t1.MIsActive , t2.MIsActive as EntryIsActive\r\n                            FROM\r\n                                t_bd_expenseitem t1\r\n                                    INNER JOIN\r\n                                t_bd_expenseitem_l t3 ON t1.MOrgId = t3.MOrgID\r\n                                    AND t3.MParentId = t1.MItemID\r\n                                    AND t1.MisDelete = t3.MisDelete\r\n                                    LEFT JOIN\r\n                                t_bd_expenseitem t2 ON t1.MItemID = t2.MParentItemID\r\n                                    AND t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MisDelete = t2.MisDelete\r\n                                    LEFT JOIN\r\n                                t_bd_expenseitem_l t4 ON t4.MOrgId = t2.MOrgID\r\n                                    AND t4.MParentId = t2.MItemID\r\n                                    AND t3.MisDelete = t4.MisDelete\r\n                                    AND t4.MLocaleID = t3.MLocaleID\r\n                            WHERE\r\n                                t1.MOrgID = @MOrgID\r\n                                    AND t1.MIsDelete = 0\r\n                                    and t1.MParentItemID = '0'\r\n                                    and t3.MLocaleID = @MLocaleID\r\n                                    order by t3.MName";
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, ctx.GetParameters((MySqlParameter)null));
			List<GLTreeModel> mDataList = BindDataTable2List(ds);
			return new GLCheckTypeDataModel
			{
				MCheckType = 3,
				MCheckTypeColumnName = GetCheckTypeColumnName(3),
				MCheckTypeName = GetCheckTypeName(ctx, 3),
				MDataList = mDataList,
				MShowType = 2
			};
		}

		public GLCheckTypeDataModel GetAllExpItemCheckTypeData(MContext ctx)
		{
			string sql = "SELECT\r\n                               t1.MItemID , t3.MName , t2.MItemID MEntryID, ltrim(t4.MName) MEntryName, t1.MIsActive , t2.MIsActive as EntryIsActive\r\n                            FROM\r\n                                t_bd_expenseitem t1\r\n                                    INNER JOIN\r\n                                t_bd_expenseitem_l t3 ON t1.MOrgId = t3.MOrgID\r\n                                    AND t3.MParentId = t1.MItemID\r\n                                    AND t1.MisDelete = t3.MisDelete\r\n                                    LEFT JOIN\r\n                                t_bd_expenseitem t2 ON t1.MItemID = t2.MParentItemID\r\n                                    AND t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MisDelete = t2.MisDelete\r\n                                    LEFT JOIN\r\n                                t_bd_expenseitem_l t4 ON t4.MOrgId = t2.MOrgID\r\n                                    AND t4.MParentId = t2.MItemID\r\n                                    AND t3.MisDelete = t4.MisDelete\r\n                                    AND t4.MLocaleID = t3.MLocaleID\r\n                            WHERE\r\n                                t1.MOrgID = @MOrgID\r\n                                    AND t1.MIsDelete = 0\r\n                                    and t1.MParentItemID = '0'\r\n                                    order by t3.MName";
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, ctx.GetParameters((MySqlParameter)null));
			List<GLTreeModel> mDataList = ExpenseItemTableToList(ds);
			return new GLCheckTypeDataModel
			{
				MCheckType = 3,
				MCheckTypeColumnName = GetCheckTypeColumnName(3),
				MCheckTypeName = GetCheckTypeName(ctx, 3),
				MDataList = mDataList,
				MShowType = 2
			};
		}

		public GLCheckTypeDataModel GetPaItemCheckTypeData(MContext ctx)
		{
			string sql = "SELECT\r\n                            t1.MItemID , t3.MItemID as MEntryID, t2.MName , t4.MName as MEntryName, t1.MItemType, t3.MItemType, t1.MIsActive , t3.MIsActive as EntryIsActive\r\n                        FROM\r\n                            t_pa_payitemgroup t1\r\n                                LEFT JOIN\r\n                            t_pa_payitemgroup_l t2 ON t1.MItemID = t2.MParentID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t1.MIsDelete = t2.MIsDelete\r\n                                LEFT JOIN\r\n                            t_pa_payitem t3 ON t1.MOrgID = t3.MOrgID\r\n                                AND t3.MGroupID = t1.MItemID\r\n                                AND t1.MIsDelete = t3.MIsDelete\r\n                                LEFT JOIN\r\n                            t_pa_payitem_l t4 ON t3.MItemID = t4.MParentID\r\n                                AND t4.MLocaleID = t2.MLocaleID\r\n                                AND t3.MIsDelete = t4.MIsDelete\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID\r\n                                AND t2.MLocaleID = @MLocaleID\r\n                                AND t1.MIsDelete = 0\r\n                        ORDER BY t1.MItemType, t1.MCreateDate";
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, ctx.GetParameters((MySqlParameter)null));
			List<GLTreeModel> mDataList = BindDataTable2List(ds);
			return new GLCheckTypeDataModel
			{
				MCheckType = 4,
				MCheckTypeColumnName = GetCheckTypeColumnName(4),
				MCheckTypeName = GetCheckTypeName(ctx, 4),
				MDataList = mDataList,
				MShowType = 2
			};
		}

		public GLCheckTypeDataModel GetTrackCheckTypeData(MContext ctx, int type, bool includeDisabled = false)
		{
			List<GLTreeModel> trackItemList = GetTrackItemList(ctx, includeDisabled);
			if (trackItemList.Count <= type - 5)
			{
				return null;
			}
			GLTreeModel gLTreeModel = trackItemList[type - 5];
			List<GLTreeModel> children = gLTreeModel.children;
			return new GLCheckTypeDataModel
			{
				MCheckType = type,
				MCheckTypeGroupID = gLTreeModel.id,
				MCheckTypeName = gLTreeModel.text,
				MCheckTypeColumnName = GetCheckTypeColumnName(type),
				MDataList = children,
				MShowType = 0
			};
		}

		public List<GLTreeModel> GetTrackItemList(MContext ctx, bool includeDisabled = false)
		{
			string format = "SELECT\r\n                           t1.MItemID, t3.MEntryID, t2.MName, t4.MName as MEntryName , t3.MIsActive\r\n                        FROM\r\n                            t_bd_track t1\r\n                                INNER JOIN\r\n                            t_bd_track_l t2 ON t1.MItemID = t2.MParentID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t1.MIsDelete = t2.MIsDelete\r\n                                LEFT JOIN\r\n                            t_bd_trackentry t3 ON\r\n                                t1.MItemID = t3.MItemID\r\n                                AND t3.MOrgID = t1.MOrgID\r\n                                AND t3.MIsDelete = t1.MIsDelete\r\n                                {0}\r\n                                LEFT JOIN\r\n                            t_bd_trackentry_l t4 ON t3.MEntryID = t4.MParentID\r\n                                AND t4.MIsDelete = t3.MIsDelete\r\n                                AND t4.MLocaleID = t2.MLocaleID\r\n\r\n                        WHERE\r\n                                t1.MOrgID = @MOrgID\r\n                                AND t2.MLocaleID = @MLocaleID\r\n                                AND t1.MIsDelete = 0  ";
			format = string.Format(format, (!includeDisabled) ? " AND t3.MIsActive = 1 " : "");
			format += "   ORDER BY t1.MCreateDate, t2.MName, IF(TRIM(convert(t4.MName using gbk)) RLIKE '^[0-9]', 1, 2), TRIM(convert(t4.MName using gbk)) ";
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(format, ctx.GetParameters((MySqlParameter)null));
			return BindDataTable2List(ds);
		}

		public List<GLTreeModel> GetAllTrackItemList(MContext ctx, bool includeDisabled = false)
		{
			string format = "SELECT\r\n                           t1.MItemID, t3.MEntryID, t2.MName, t4.MName as MEntryName , t3.MIsActive\r\n                        FROM\r\n                            t_bd_track t1\r\n                                INNER JOIN\r\n                            t_bd_track_l t2 ON t1.MItemID = t2.MParentID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t1.MIsDelete = t2.MIsDelete\r\n                                LEFT JOIN\r\n                            t_bd_trackentry t3 ON\r\n                                t1.MItemID = t3.MItemID\r\n                                AND t3.MOrgID = t1.MOrgID\r\n                                AND t3.MIsDelete = t1.MIsDelete\r\n                                {0}\r\n                                LEFT JOIN\r\n                            t_bd_trackentry_l t4 ON t3.MEntryID = t4.MParentID\r\n                                AND t4.MIsDelete = t3.MIsDelete\r\n                                AND t4.MLocaleID = t2.MLocaleID\r\n\r\n                        WHERE\r\n                                t1.MOrgID = @MOrgID\r\n                                AND t1.MIsDelete = 0  ";
			format = string.Format(format, (!includeDisabled) ? " AND t3.MIsActive = 1 " : "");
			format += "   ORDER BY t1.MCreateDate, t2.MName, IF(TRIM(convert(t4.MName using gbk)) RLIKE '^[0-9]', 1, 2), TRIM(convert(t4.MName using gbk)) ";
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(format, ctx.GetParameters((MySqlParameter)null));
			return BindDataTable2List(ds);
		}

		public int GetBalanceDirByType(string mType)
		{
			switch (mType)
			{
			case "Invoice_Sale":
			case "Invoice_Sale_Red":
			case "Pay_Adjustment":
			case "Pay_BankFee":
			case "Pay_BankInterest":
			case "Pay_Other":
			case "Pay_Prepare":
			case "Pay_PurReturn":
			case "Pay_OtherReturn":
			case "Pay_Salary":
			case "Pay_Purchase":
				return 1;
			default:
				return -1;
			}
		}

		private bool IsCheckTypeActive(List<BDAccountModel> accountList, string accountCode, string columnName)
		{
			BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MCode == accountCode);
			if (bDAccountModel != null)
			{
				object obj = bDAccountModel.MCheckGroupModel.TryGetPropertyValue(columnName);
				int num = (obj == null || string.IsNullOrWhiteSpace(obj.ToString())) ? CheckTypeStatusEnum.Disabled : int.Parse(obj.ToString());
				return num == CheckTypeStatusEnum.Required || num == CheckTypeStatusEnum.Optional;
			}
			return false;
		}

		public List<GLCheckGroupValueModel> GetCheckGroupValueModelFromBill(MContext ctx, List<BDInitBillViewModel> list)
		{
			List<BDAccountModel> currentAccountList = new BDAccountRepository().GetCurrentAccountBaseData(ctx, true);
			List<BDInitBillEntryViewModel> source = Union((from x in list
			select x.MInitBillEntryList).ToList());
			List<GLCheckGroupValueModel> source2 = (from x in source
			group x by new
			{
				x.MCurrentAccountCode,
				x.MType,
				x.MCyID,
				x.MContactID,
				x.MEmployeeID,
				x.MMerItemID,
				x.MExpItemID,
				x.MPaItemID,
				x.MTrackItem1,
				x.MTrackItem2,
				x.MTrackItem3,
				x.MTrackItem4,
				x.MTrackItem5
			} into y
			select new GLCheckGroupValueModel
			{
				MAccountID = currentAccountList.FirstOrDefault((BDAccountModel s) => s.MCode == y.Key.MCurrentAccountCode).MItemID,
				MContactID = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MContactID") ? y.Key.MContactID : null),
				MEmployeeID = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MEmployeeID") ? y.Key.MEmployeeID : null),
				MMerItemID = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MMerItemID") ? y.Key.MMerItemID : null),
				MExpItemID = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MExpItemID") ? y.Key.MExpItemID : null),
				MPaItemID = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MPaItemID") ? y.Key.MPaItemID : null),
				MTrackItem1 = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MTrackItem1") ? y.Key.MTrackItem1 : null),
				MTrackItem2 = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MTrackItem2") ? y.Key.MTrackItem2 : null),
				MTrackItem3 = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MTrackItem3") ? y.Key.MTrackItem3 : null),
				MTrackItem4 = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MTrackItem4") ? y.Key.MTrackItem4 : null),
				MTrackItem5 = (IsCheckTypeActive(currentAccountList, y.Key.MCurrentAccountCode, "MTrackItem5") ? y.Key.MTrackItem5 : null),
				MCurrencyID = y.Key.MCyID,
				MAmount = y.Sum((BDInitBillEntryViewModel z) => (decimal)GetBalanceDirByType(y.Key.MType) * z.MTaxAmount),
				MAmountFor = y.Sum((BDInitBillEntryViewModel z) => (decimal)GetBalanceDirByType(y.Key.MType) * z.MTaxAmountFor),
				MBillAmountInfo = new GLCheckGroupValueModel.GLBillAmountInfo
				{
					MCurrencyID = y.Key.MCyID,
					MAmount = y.Sum((BDInitBillEntryViewModel z) => (decimal)GetBalanceDirByType(y.Key.MType) * z.MTaxAmount),
					MAmountFor = y.Sum((BDInitBillEntryViewModel z) => (decimal)GetBalanceDirByType(y.Key.MType) * z.MTaxAmountFor)
				}
			}).ToList();
			source2 = (from x in source2
			group x by new
			{
				x.MAccountID,
				x.MBillAmountInfo.MCurrencyID,
				x.MContactID,
				x.MEmployeeID,
				x.MMerItemID,
				x.MExpItemID,
				x.MPaItemID,
				x.MTrackItem1,
				x.MTrackItem2,
				x.MTrackItem3,
				x.MTrackItem4,
				x.MTrackItem5
			} into y
			select new GLCheckGroupValueModel
			{
				MAccountID = y.Key.MAccountID,
				MContactID = y.Key.MContactID,
				MEmployeeID = y.Key.MEmployeeID,
				MMerItemID = y.Key.MMerItemID,
				MExpItemID = y.Key.MExpItemID,
				MPaItemID = y.Key.MPaItemID,
				MTrackItem1 = y.Key.MTrackItem1,
				MTrackItem2 = y.Key.MTrackItem2,
				MTrackItem3 = y.Key.MTrackItem3,
				MTrackItem4 = y.Key.MTrackItem4,
				MTrackItem5 = y.Key.MTrackItem5,
				MBillAmountInfo = new GLCheckGroupValueModel.GLBillAmountInfo
				{
					MCurrencyID = y.Key.MCurrencyID,
					MAmount = y.Sum((GLCheckGroupValueModel z) => z.MAmount),
					MAmountFor = y.Sum((GLCheckGroupValueModel z) => z.MAmountFor)
				}
			}).ToList();
			for (int i = 0; i < source2.Count; i++)
			{
				source2[i] = GetCheckGroupValueModel(ctx, source2[i]);
			}
			return source2;
		}

		public List<KeyValuePair<string, string>> GetCheckTypeList(MContext ctx)
		{
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			for (int i = 0; i < 10; i++)
			{
				list.Add(new KeyValuePair<string, string>(GetCheckTypeColumnName(i), GetCheckTypeName(ctx, i)));
			}
			return list;
		}

		public string GetCheckTypeName(MContext ctx, int type = 0)
		{
			switch (type)
			{
			case 0:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Contact", "联系人");
			case 1:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Employee", "员工");
			case 2:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "MerItem", "商品项目");
			case 3:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseItem", "费用项目");
			case 4:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.PA, "PaSalaryItem", "工资项目");
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
			{
				GLCheckTypeDataModel trackCheckTypeData = GetTrackCheckTypeData(ctx, type, true);
				return trackCheckTypeData?.MCheckTypeName;
			}
			default:
				return string.Empty;
			}
		}

		public string GetCheckTypeColumnName(int type = 0)
		{
			switch (type)
			{
			case 0:
				return "MContactID";
			case 1:
				return "MEmployeeID";
			case 2:
				return "MMerItemID";
			case 3:
				return "MExpItemID";
			case 4:
				return "MPaItemID";
			case 5:
				return "MTrackItem1";
			case 6:
				return "MTrackItem2";
			case 7:
				return "MTrackItem3";
			case 8:
				return "MTrackItem4";
			case 9:
				return "MTrackItem5";
			default:
				return string.Empty;
			}
		}

		private List<GLTreeModel> BindDataTable2List(DataSet ds)
		{
			List<GLTreeModel> list = new List<GLTreeModel>();
			if (ds == null || ds.Tables == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			GLTreeModel gLTreeModel = null;
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				DataRow row = ds.Tables[0].Rows[i];
				GLTreeModel gLTreeModel2 = new GLTreeModel
				{
					id = row.MField<string>("MItemID"),
					text = row.MField<string>("MName"),
					MIsActive = row.MField<bool>("MIsActive")
				};
				string columnName = ds.Tables[0].Columns.Contains("EntryIsActive") ? "EntryIsActive" : "MIsActive";
				GLTreeModel gLTreeModel3 = new GLTreeModel
				{
					id = row.MField<string>("MEntryID"),
					parentId = gLTreeModel2.id,
					text = row.MField<string>("MEntryName"),
					MIsActive = row.MField<bool>(columnName)
				};
				if (gLTreeModel == null || gLTreeModel.id != gLTreeModel2.id)
				{
					gLTreeModel = gLTreeModel2;
					gLTreeModel2.children = (string.IsNullOrWhiteSpace(gLTreeModel3.id) ? null : new List<GLTreeModel>
					{
						gLTreeModel3
					});
					list.Add(gLTreeModel2);
				}
				else if (!string.IsNullOrWhiteSpace(gLTreeModel3.id))
				{
					gLTreeModel.children.Add(gLTreeModel3);
				}
			}
			return list;
		}

		private List<GLTreeModel> ExpenseItemTableToList(DataSet ds)
		{
			List<GLTreeModel> list = new List<GLTreeModel>();
			if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
			{
				return list;
			}
			GLTreeModel gLTreeModel = null;
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				DataRow row = ds.Tables[0].Rows[i];
				GLTreeModel gLTreeModel2 = new GLTreeModel
				{
					id = row.MField<string>("MItemID"),
					text = row.MField<string>("MName"),
					MIsActive = row.MField<bool>("MIsActive")
				};
				string columnName = ds.Tables[0].Columns.Contains("EntryIsActive") ? "EntryIsActive" : "MIsActive";
				GLTreeModel gLTreeModel3 = new GLTreeModel
				{
					id = row.MField<string>("MEntryID"),
					parentId = gLTreeModel2.id,
					text = row.MField<string>("MEntryName"),
					MIsActive = row.MField<bool>(columnName)
				};
				bool flag = gLTreeModel == null || gLTreeModel.id != gLTreeModel2.id;
				bool flag2 = gLTreeModel != null && gLTreeModel.id == gLTreeModel2.id && gLTreeModel.text != gLTreeModel2.text;
				if (flag | flag2)
				{
					gLTreeModel = gLTreeModel2;
					gLTreeModel2.children = (string.IsNullOrWhiteSpace(gLTreeModel3.id) ? null : new List<GLTreeModel>
					{
						gLTreeModel3
					});
					list.Add(gLTreeModel2);
				}
				else if (!string.IsNullOrWhiteSpace(gLTreeModel3.id))
				{
					gLTreeModel.children.Add(gLTreeModel3);
				}
			}
			return list;
		}

		public ValidateQueryModel GetValidateTrackItemGroupQuery(MContext ctx, GLCheckGroupModel checkgroupModel)
		{
			if (checkgroupModel == null)
			{
				return new ValidateQueryModel();
			}
			List<GLTreeModel> trackItemList = GetTrackItemList(ctx, false);
			if (checkgroupModel.MTrackItem1 != CheckTypeStatusEnum.Disabled && trackItemList.Count < 1)
			{
				goto IL_0092;
			}
			if (checkgroupModel.MTrackItem2 != CheckTypeStatusEnum.Disabled && trackItemList.Count < 2)
			{
				goto IL_0092;
			}
			if (checkgroupModel.MTrackItem3 != CheckTypeStatusEnum.Disabled && trackItemList.Count < 3)
			{
				goto IL_0092;
			}
			if (checkgroupModel.MTrackItem4 != CheckTypeStatusEnum.Disabled && trackItemList.Count < 4)
			{
				goto IL_0092;
			}
			int num = (checkgroupModel.MTrackItem5 != CheckTypeStatusEnum.Disabled && trackItemList.Count < 5) ? 1 : 0;
			goto IL_0093;
			IL_0092:
			num = 1;
			goto IL_0093;
			IL_0093:
			bool flag = (byte)num != 0;
			string querySQL = " select (case when 0 = " + (flag ? "0" : "1") + " then " + 915 + " else " + 999 + " end) as MCode ";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateVoucherNumberOutOfRange(MContext ctx, int maxCount)
		{
			string querySQL = " SELECT\r\n                                    (CASE\n                                        WHEN LENGTH(CAST(MAX(CAST(MNumber AS SIGNED)) AS CHAR)) > @MaxCount THEN " + 981 + "\r\n                                        ELSE " + 999 + "\r\n                                    END) AS MCode\n                                FROM\n                                    t_gl_voucher\n                                WHERE\n                                    MOrgID = @MOrgID AND MIsDelete = 0\r\n                                        AND LENGTH(IFNULL(MNumber, '')) > 0 ";
			MySqlParameter[] parameter = new MySqlParameter[1]
			{
				new MySqlParameter("@MaxCount", maxCount)
			};
			return new ValidateQueryModel(querySQL, parameter);
		}

		public ValidateQueryModel GetValidateAccountSql(MContext ctx, BDAccountModel account)
		{
			bool flag = !string.IsNullOrWhiteSpace(account.MParentID) && !string.IsNullOrWhiteSpace(account.MNumber) && !string.IsNullOrWhiteSpace(account.MultiLanguage.FirstOrDefault((MultiLanguageFieldList x) => x.MFieldName == "MName").MMultiLanguageField.FirstOrDefault((MultiLanguageField x) => x.MLocaleID == ctx.MLCID).MValue);
			string querySQL = " select (case when '0' = @MAccountValid then " + 931 + " else " + 999 + " end) as MCode ";
			MySqlParameter[] parameter = new MySqlParameter[1]
			{
				new MySqlParameter("@MAccountValid", flag ? "1" : "0")
			};
			return new ValidateQueryModel(querySQL, parameter);
		}

		public ValidateQueryModel GetValidateAccountNumberSql(MContext ctx, BDAccountModel account)
		{
			if (account == null || string.IsNullOrWhiteSpace(account.MParentID) || string.IsNullOrWhiteSpace(account.MNumber))
			{
				return new ValidateQueryModel();
			}
			string text = " select " + 927 + " as MCode from t_bd_account where MNumber in('" + account.MNumber + "')  and MOrgID = @MOrgID and MParentID = @MAccountParentID and MIsDelete = 0 ";
			if (!string.IsNullOrWhiteSpace(account.MItemID) && !account.IsNew)
			{
				text = text + " and MItemID not in('" + account.MItemID + "') ";
			}
			MySqlParameter[] parameter = new MySqlParameter[1]
			{
				new MySqlParameter("@MAccountParentID", account.MParentID)
			};
			return new ValidateQueryModel(text, parameter);
		}

		public ValidateQueryModel GetValidateFixAssetsTypeNumberSql(MContext ctx, FAFixAssetsTypeModel type)
		{
			if (type == null || string.IsNullOrWhiteSpace(type.MNumber))
			{
				return new ValidateQueryModel();
			}
			MySqlParameter[] parameter = new MySqlParameter[1]
			{
				new MySqlParameter
				{
					ParameterName = "@MFixAssetsTypeNumber",
					Value = type.MName
				}
			};
			string text = " select " + 937 + " as MCode from t_fa_fixassetstype where MNumber = @MFixAssetsTypeNumber  and MOrgID = @MOrgID and MIsDelete = 0 ";
			if (!string.IsNullOrWhiteSpace(type.MItemID) && !type.IsNew)
			{
				text = text + " and MItemID not in('" + type.MItemID + "') ";
			}
			return new ValidateQueryModel(text, parameter);
		}

		public ValidateQueryModel GetValidateFixAssetsTypeRelatedSql(MContext ctx, List<string> itemIDs)
		{
			if (itemIDs == null || itemIDs.Count == 0)
			{
				return new ValidateQueryModel();
			}
			string querySQL = " select " + 941 + " as MCode from t_fa_fixassets where MFATypeID in('" + string.Join("','", itemIDs) + "')  and MOrgID = @MOrgID and MIsDelete = 0 ";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateFixAssetsTypeNameSql(MContext ctx, FAFixAssetsTypeModel type)
		{
			if (string.IsNullOrWhiteSpace(type?.MName))
			{
				return new ValidateQueryModel();
			}
			List<MultiLanguageFieldList> source = (from t in type.MultiLanguage
			where t.MFieldName == "MName"
			select t).ToList();
			MultiLanguageFieldList multiLanguageFieldList = source.FirstOrDefault((MultiLanguageFieldList t) => t.MFieldName == "MName");
			if (multiLanguageFieldList == null)
			{
				return new ValidateQueryModel();
			}
			List<MySqlParameter> list = new List<MySqlParameter>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(" select " + 938 + " as MCode from t_fa_fixassetstype_L where MOrgID = @MOrgID  and MIsDelete = 0 ");
			stringBuilder.Append(" AND (");
			int num = 0;
			foreach (MultiLanguageField item in multiLanguageFieldList.MMultiLanguageField)
			{
				if (!string.IsNullOrEmpty(item.MValue))
				{
					stringBuilder.AppendFormat(" (TRIM(MName)=@ColumnValue{0} and MLocaleID = @MLCID{0}) OR", num);
					list.Add(new MySqlParameter($"@MLCID{num}", item.MLocaleID));
					list.Add(new MySqlParameter($"@ColumnValue{num}", item.MValue.Trim()));
					num++;
				}
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			stringBuilder.Append(")");
			if (!string.IsNullOrWhiteSpace(type.MItemID) && !type.IsNew)
			{
				stringBuilder.Append(" and MParentID not in('" + type.MItemID + "')");
			}
			return new ValidateQueryModel(stringBuilder.ToString(), list.ToArray());
		}

		public ValidateQueryModel GetValidateFixAssetsNumberSql(MContext ctx, FAFixAssetsModel type)
		{
			if (type == null || string.IsNullOrWhiteSpace(type.MNumber))
			{
				return new ValidateQueryModel();
			}
			string text = " select " + 939 + " as MCode from t_fa_fixassets where MNumber = @FixAssetsNumber and MPrefix = @FixAssetsPrefix  and MOrgID = @MOrgID and MIsDelete = 0 ";
			if (!string.IsNullOrWhiteSpace(type.MItemID) && !type.IsNew)
			{
				text = text + " and MItemID not in('" + type.MItemID + "') ";
			}
			return new ValidateQueryModel(text, new List<MySqlParameter>
			{
				new MySqlParameter
				{
					ParameterName = "@FixAssetsPrefix",
					Value = type.MPrefix
				},
				new MySqlParameter
				{
					ParameterName = "@FixAssetsNumber",
					Value = type.MNumber
				}
			}.ToArray());
		}

		public ValidateQueryModel GetValidateExitsCreatedDepreciationVoucher(MContext ctx, List<string> voucherIDs)
		{
			if (voucherIDs == null || voucherIDs.Count == 0)
			{
				return new ValidateQueryModel();
			}
			DateTime mFABeginDate = ctx.MFABeginDate;
			int num = mFABeginDate.Year * 12;
			mFABeginDate = ctx.MFABeginDate;
			int num2 = num + mFABeginDate.Month;
			string querySQL = "\r\n                        SELECT\r\n                            (CASE\r\n                                WHEN COUNT(t1.MItemID) > 0 THEN " + 949 + "\r\n                                ELSE " + 999 + "\r\n                            END) AS MCode\r\n                        FROM\r\n                            t_gl_periodtransfer t1\r\n                                INNER JOIN\r\n                            t_gl_voucher t2 ON t1.MVoucherID = t2.MItemID\r\n                                AND t1.MOrgID = t2.MOrgID\r\n                                AND t1.MIsdelete = t2.MIsDelete\r\n                                AND t1.MYear = t2.MYear\r\n                                AND t1.MPeriod = t2.MPeriod\r\n                        WHERE\r\n                            t1.MOrgID = @MOrgID\r\n                                AND t1.MTransferTypeID = 2\r\n                                AND t1.MVoucherID in  ('" + string.Join("','", voucherIDs) + "')\r\n                                AND (t1.MYear * 12 + t1.MPeriod) >= " + num2 + "\r\n                                AND t1.MIsDelete = 0\r\n                                AND EXISTS( SELECT\r\n                                    1\r\n                                FROM\r\n                                    t_gl_periodtransfer t3\r\n                                WHERE\r\n                                    (t3.MYear * 12 + t3.MPeriod) > (t1.MYear * 12 + t1.MPeriod)\r\n                                        AND t3.MOrgID = t1.MOrgID\r\n                                        AND t3.MTransferTypeID = t1.MTransferTypeID\r\n                                        AND t3.MIsDelete = t1.MIsDelete )\r\n                        ";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateExitsChange(MContext ctx, List<string> voucherIDs)
		{
			if (voucherIDs == null || voucherIDs.Count == 0)
			{
				return new ValidateQueryModel();
			}
			string querySQL = "SELECT\r\n                        (CASE\r\n                                WHEN COUNT(t1.MItemID) > 0 THEN " + 952 + "\r\n                                ELSE " + 999 + "\r\n                            END) AS MCode\r\n                    FROM\r\n                        t_fa_fixassetschange t1\r\n                            INNER JOIN\r\n                        (SELECT\r\n                            t.*\r\n                        FROM\r\n                            t_gl_voucher t\r\n                         INNER JOIN t_gl_periodtransfer t4 ON t.MItemID = t4.MVoucherID\r\n                            AND t.MIsDelete = t4.MIsDelete\r\n                            AND t4.MTransferTypeID = 2\r\n                        WHERE\r\n                            t.MItemID IN  ('" + string.Join("','", voucherIDs) + "')\r\n                                AND t.MOrgID = @MOrgID\r\n                                AND t.MIsDelete = 0 ) t2 ON YEAR(t1.MChangeFromPeriod) * 12 + MONTH(t1.MChangeFromPeriod) > ((t2.MYear * 12 + t2.MPeriod))\r\n                    WHERE\r\n                        t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\r\n                            AND t1.MType >= 4\r\n                            AND t1.MType != 16";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateFixAssetsNameSql(MContext ctx, FAFixAssetsModel type)
		{
			if (type == null || string.IsNullOrWhiteSpace(type.MName))
			{
				return new ValidateQueryModel();
			}
			string text = " select " + 940 + " as MCode from t_fa_fixassets_L where MName in('" + type.MName + "')  and MOrgID = @MOrgID and MLocaleID = @MLocaleID and MIsDelete = 0 ";
			if (!string.IsNullOrWhiteSpace(type.MItemID) && !type.IsNew)
			{
				text = text + " and MParentID not in('" + type.MItemID + "') ";
			}
			return new ValidateQueryModel(text, null);
		}

		public ValidateQueryModel GetValidateFixAssetsStatusSql(MContext ctx, List<string> itemIDs, int type)
		{
			if (itemIDs == null || itemIDs.Count == 0)
			{
				return new ValidateQueryModel();
			}
			int num = (type == 0) ? 944 : 943;
			string querySQL = " select " + num + " as MCode from t_fa_fixassets where MItemID in('" + string.Join("','", itemIDs) + "')  and MOrgID = @MOrgID " + ((type == 0) ? " and MStatus = 0" : " and MStatus > 0") + " and MIsDelete = 0 ";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidteAccountNameSql(MContext ctx, BDAccountModel account)
		{
			ValidateQueryModel validateQueryModel = new ValidateQueryModel();
			List<MultiLanguageField> mMultiLanguageField = account.MultiLanguage.FirstOrDefault((MultiLanguageFieldList x) => x.MFieldName == "MName").MMultiLanguageField;
			List<MySqlParameter> list = new List<MySqlParameter>();
			list.Add(new MySqlParameter("@MAccountNameParentID", account.MParentID));
			for (int i = 0; i < mMultiLanguageField.Count; i++)
			{
				string text = "@AccountName" + i;
				list.Add(new MySqlParameter(text, mMultiLanguageField[i].MValue));
				string text2 = " select " + 928 + " as MCode from t_bd_account_l t1 inner join t_bd_account t2 on t1.MParentID = t2.MItemID and t1.MOrgId = t2.MOrgID and t1.MIsDelete = t2.MIsDelete where MName in('" + text + "') and t1.MParentID = @MAccountNameParentID and t1.MOrgID = @MOrgID and t1.MIsDelete = 0  and t1.MLocaleID = '" + mMultiLanguageField[i].MLocaleID + "' ";
				if (!string.IsNullOrWhiteSpace(account.MItemID) && !account.IsNew)
				{
					text2 = text2 + " and t2.MItemID not in('" + account.MItemID + "') ";
				}
				validateQueryModel.Join(new ValidateQueryModel(text2, list.ToArray()));
			}
			return validateQueryModel;
		}

		public ValidateQueryModel GetValidateAccountCheckGroupUpdateSql(MContext ctx, string accountId, GLCheckGroupModel newCheckGroup)
		{
			if (string.IsNullOrWhiteSpace(accountId) || newCheckGroup == null)
			{
				return new ValidateQueryModel();
			}
			string text = "SELECT\r\n                                (case when count(*) = 0 then " + 999 + " else " + 930 + " end) as MCode\r\n                            FROM\r\n                                t_gl_checkgroupvalue x\r\n                                    INNER JOIN\r\n                                (SELECT\r\n                                    MCheckGroupValueID\r\n                                FROM\r\n                                    t_gl_initbalance\r\n                                WHERE\r\n                                    maccountid = @MCheckGroupAccountID\r\n                                        AND MOrgID = @MOrgID\r\n                                        AND MIsDelete = 0  UNION SELECT\r\n                                    MCheckGroupValueID\r\n                                FROM\r\n                                    t_gl_balance\r\n                                WHERE\r\n                                    maccountid = @MCheckGroupAccountID\r\n                                        AND MOrgID = @MOrgID\r\n                                        AND MIsDelete = 0  UNION SELECT\r\n                                    MCheckGroupValueID\r\n                                FROM\r\n                                    t_gl_voucherentry t1\r\n                                INNER JOIN t_gl_voucher t2 ON t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MID = t2.MItemID\r\n                                    AND t1.MisDelete = t2.MIsDelete\r\n                                    AND (t2.MStatus = 0 OR t2.MStatus = 1)\r\n                                    AND (length(ifnull(t2.MNumber,'')) > 0)\r\n                                WHERE\r\n                                    t1.MOrgID = @MOrgID AND t1.MIsDelete = 0\r\n                                        AND t1.MAccountID = @MCheckGroupAccountID) y ON x.MItemID = y.MCheckGroupValueID\r\n                            WHERE\r\n                                x.MOrgID = @MOrgID AND x.MIsDelete = 0  ";
			List<string> list = new List<string>();
			list.Add(GetQueryFilter(newCheckGroup.MContactID, "MContactID"));
			list.Add(GetQueryFilter(newCheckGroup.MEmployeeID, "MEmployeeID"));
			list.Add(GetQueryFilter(newCheckGroup.MMerItemID, "MMerItemID"));
			list.Add(GetQueryFilter(newCheckGroup.MExpItemID, "MExpItemID"));
			list.Add(GetQueryFilter(newCheckGroup.MPaItemID, "MPaItemID"));
			list.Add(GetQueryFilter(newCheckGroup.MTrackItem1, "MTrackItem1"));
			list.Add(GetQueryFilter(newCheckGroup.MTrackItem2, "MTrackItem2"));
			list.Add(GetQueryFilter(newCheckGroup.MTrackItem3, "MTrackItem3"));
			list.Add(GetQueryFilter(newCheckGroup.MTrackItem4, "MTrackItem4"));
			list.Add(GetQueryFilter(newCheckGroup.MTrackItem5, "MTrackItem5"));
			list = (from x in list
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			if (list.Count > 0)
			{
				string str = string.Join(" OR ", list);
				text = text + " and (" + str + ")";
			}
			MySqlParameter[] parameter = new MySqlParameter[1]
			{
				new MySqlParameter("@MCheckGroupAccountID", accountId)
			};
			return new ValidateQueryModel(text, parameter);
		}

		private string GetQueryFilter(int type, string columName)
		{
			if (type == CheckTypeStatusEnum.Required)
			{
				return " LENGTH(IFNULL(x." + columName + ", '')) = 0 ";
			}
			if (type == CheckTypeStatusEnum.Disabled || type == CheckTypeStatusEnum.DisabledOptional || type == CheckTypeStatusEnum.DisabledRequired)
			{
				return " LENGTH(IFNULL(x." + columName + ", '')) > 0 ";
			}
			return string.Empty;
		}

		public ValidateQueryModel GetValidteVoucherReferenceSql(MContext ctx, GLVoucherReferenceModel model)
		{
			ValidateQueryModel validateQueryModel = new ValidateQueryModel();
			if (model == null || string.IsNullOrWhiteSpace(model.MContent))
			{
				return new ValidateQueryModel(MActionResultCodeEnum.MVoucherExplanationInvalid);
			}
			MySqlParameter[] parameter = new MySqlParameter[2]
			{
				new MySqlParameter("@MReferenceItemID", model.MItemID),
				new MySqlParameter("@MReferenceContent", model.MContent)
			};
			string text = " select " + 932 + " as MCode from t_gl_voucherreference where MContent = @MReferenceContent and MOrgID = @MOrgID and MIsDelete = 0  and MLocaleID = @MLocaleID ";
			if (!string.IsNullOrWhiteSpace(model.MItemID))
			{
				text += " and MItemID != @MReferenceItemID ";
			}
			return new ValidateQueryModel(text, parameter);
		}

		public ValidateQueryModel GetValidatePeriodBeforeStartSql(MContext ctx, int year, int period)
		{
			string querySQL = " select (case when @MYearPeriod < @MStartYearPeriod then " + 910 + " else " + 999 + " end) as MCode ";
			MySqlParameter[] obj = new MySqlParameter[2]
			{
				new MySqlParameter("@MYearPeriod", year * 12 + period),
				null
			};
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int num = mGLBeginDate.Year * 12;
			mGLBeginDate = ctx.MGLBeginDate;
			obj[1] = new MySqlParameter("@MStartYearPeriod", num + mGLBeginDate.Month);
			MySqlParameter[] parameter = obj;
			return new ValidateQueryModel(querySQL, parameter);
		}

		public ValidateQueryModel GetValidateHasNotSettledPeriod(MContext ctx, int year, int period)
		{
			int num = year * 12 + period;
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			int num2 = num - mGLBeginDate.Year * 12;
			mGLBeginDate = ctx.MGLBeginDate;
			int num3 = num2 - mGLBeginDate.Month;
			string querySQL = " select (case when count(*) < " + num3 + " then  " + 925 + " else " + 999 + " end) as MCode from(\r\n                    SELECT DISTINCT\r\n                        myear, mperiod\r\n                    FROM\r\n                        t_gl_settlement t1\r\n                    WHERE\r\n                        morgid = @MOrgID\r\n                            AND MStatus = 1\r\n                            AND MIsDelete = 0 ) tt";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateHasClosedPeriodSql(MContext ctx, List<DateTime> dates)
		{
			if (dates == null || dates.Count == 0)
			{
				return new ValidateQueryModel();
			}
			List<int> paramValues = (from x in dates
			select x.Year * 100 + x.Month).Distinct().ToList();
			List<MySqlParameter> list = new List<MySqlParameter>();
			string text = " select (case when count(*) > 0  then  " + 967 + " else " + 999 + " end) as MCode from(\r\n                    SELECT DISTINCT\r\n                        myear, mperiod\r\n                    FROM\r\n                        t_gl_settlement t1\r\n                    WHERE\r\n                        morgid = @MOrgID\r\n                            AND MStatus = 1\r\n                            AND MIsDelete = 0\r\n                            and (MYear * 100 + MPeriod) " + GetInFilterQuery(paramValues, ref list, "M_ID") + ") tt ";
			if (dates.Any((DateTime t) => t.CompareTo(ctx.MGLBeginDate) < 0))
			{
				text = text + " union all select " + 968 + " as MCode ";
			}
			return new ValidateQueryModel(text, list.ToArray());
		}

		public ValidateQueryModel GetValidateInitBalanceOverSql(MContext ctx, bool init = true)
		{
			string querySQL = " select (case when @MInitBalanceOver < 1 then " + 914 + " else " + 999 + " end) as MCode ";
			MySqlParameter[] parameter = new MySqlParameter[1]
			{
				new MySqlParameter("@MInitBalanceOver", (ctx.MInitBalanceOver || !init) ? 1 : 0)
			};
			return new ValidateQueryModel(querySQL, parameter);
		}

		public ValidateQueryModel GetValidatePeriodClosedSql(int year, int period)
		{
			string querySQL = " select " + 909 + " as MCode from t_gl_settlement where MOrgID = @MOrgID and MStatus = @MClosedStatus and MYear = @MClosedYear and MPeriod = @MClosedPeriod and MIsDelete = 0  ";
			MySqlParameter[] parameter = new MySqlParameter[3]
			{
				new MySqlParameter("@MClosedYear", year),
				new MySqlParameter("@MClosedPeriod", period),
				new MySqlParameter("@MClosedStatus", 1)
			};
			return new ValidateQueryModel(querySQL, parameter);
		}

		public ValidateQueryModel GetValidatePeriodClosedSql(List<int> yearPeriods)
		{
			List<MySqlParameter> list = new List<MySqlParameter>
			{
				new MySqlParameter("@MClosedStatus", 1)
			};
			string inFilterQuery = GetInFilterQuery(yearPeriods, ref list, "M_ID");
			string querySQL = " select " + 909 + " as MCode from \r\n                            t_gl_settlement where MOrgID = @MOrgID and MStatus = @MClosedStatus and MIsDelete = 0\r\n                            and MYear*100+MPeriod " + inFilterQuery + " ";
			return new ValidateQueryModel(querySQL, list.ToArray());
		}

		public ValidateQueryModel GetValidateVoucherApproveSql(List<GLVoucherModel> vouchers, int nowStatus = -2, int oldStatus = -2)
		{
			if (vouchers == null || vouchers.Count == 0)
			{
				return new ValidateQueryModel(null, null);
			}
			List<string> list = (from x in vouchers
			select x.MItemID).ToList();
			if (list == null || list.Count == 0)
			{
				return new ValidateQueryModel(null, null);
			}
			string querySQL = null;
			int num;
			switch (nowStatus)
			{
			case 1:
				if (oldStatus != 0)
				{
					goto default;
				}
				goto case -1;
			default:
				num = ((nowStatus == 0 && oldStatus != 1) ? 1 : 0);
				break;
			case -1:
				num = 1;
				break;
			}
			if (num != 0)
			{
				querySQL = " select " + 911 + " as MCode from t_gl_voucher where MOrgID = @MOrgID and MStatus = 1 and MItemID in('" + string.Join("','", list) + "') and MIsDelete = 0 ";
			}
			else if (nowStatus == 0 && oldStatus == 1)
			{
				querySQL = " select " + 912 + " as MCode from t_gl_voucher where MOrgID = @MOrgID and MStatus = 0 and MItemID in('" + string.Join("','", list) + "') and MIsDelete = 0 ";
			}
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateVoucherNumberSql(List<GLVoucherModel> vouchers)
		{
			if (vouchers == null || vouchers.Count == 0)
			{
				return new ValidateQueryModel();
			}
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string format = " select " + 906 + " as MCode from t_gl_voucher\r\n                where MNumber in('{0}') and MOrgID = @MOrgID and MYear = @MNumberYear{1} and MPeriod = @MNumberPeriod{1} and MIsDelete = 0 ";
			var enumerable = vouchers.GroupBy(delegate(GLVoucherModel f)
			{
				DateTime mDate = f.MDate;
				int year = mDate.Year;
				mDate = f.MDate;
				return new
				{
					Year = year,
					Month = mDate.Month
				};
			});
			List<MySqlParameter> list = new List<MySqlParameter>();
			foreach (var item in enumerable)
			{
				List<GLVoucherModel> source = item.ToList();
				List<string> list2 = (from t in source
				where !string.IsNullOrEmpty(t.MNumber)
				select t into x
				select x.MNumber).ToList();
				if (list2 != null && list2.Count != 0)
				{
					if (num > 0)
					{
						stringBuilder.AppendLine(" union ");
					}
					stringBuilder.AppendFormat(format, string.Join("','", list2), num);
					List<string> list3 = (from t in source
					where !string.IsNullOrEmpty(t.MItemID)
					select t into x
					select x.MItemID).ToList();
					if (list3 != null && list3.Count > 0)
					{
						stringBuilder.Append(" and MItemID not in('" + string.Join("','", list3) + "') ");
					}
					list.Add(new MySqlParameter($"@MNumberYear{num}", item.Key.Year));
					list.Add(new MySqlParameter($"@MNumberPeriod{num}", item.Key.Month));
					num++;
				}
			}
			if (string.IsNullOrWhiteSpace(stringBuilder.ToString()))
			{
				return new ValidateQueryModel();
			}
			return new ValidateQueryModel(stringBuilder.ToString(), list.ToArray());
		}

		public ValidateQueryModel GetValidateVoucherModuleFastCodeSql(List<FCVoucherModuleModel> vouchers)
		{
			if (vouchers == null || vouchers.Count == 0)
			{
				return new ValidateQueryModel();
			}
			List<string> list = (from x in vouchers
			select x.MFastCode).ToList();
			List<string> list2 = (from x in vouchers
			select x.MItemID).ToList();
			List<MySqlParameter> list3 = new List<MySqlParameter>();
			if (list == null || list.Count == 0)
			{
				return new ValidateQueryModel();
			}
			string text = " select " + 922 + " as MCode from t_fc_vouchermodule where " + JoinParameters("MFastCode", list, ref list3) + " and MOrgID = @MOrgID and MIsDelete = 0  and MLCID = @MLocaleID ";
			if (list2 != null && list2.Count > 0)
			{
				text = text + " and MItemID not in('" + string.Join("','", list2) + "') ";
			}
			return new ValidateQueryModel(text, list3.ToArray());
		}

		public ValidateQueryModel GetValidateFapiaoModuleFastCodeSql(List<FCFapiaoModuleModel> vouchers)
		{
			if (vouchers == null || vouchers.Count == 0)
			{
				return new ValidateQueryModel();
			}
			List<string> list = (from x in vouchers
			select x.MFastCode).ToList();
			List<string> list2 = (from x in vouchers
			select x.MID).ToList();
			List<MySqlParameter> list3 = new List<MySqlParameter>();
			if (list == null || list.Count == 0)
			{
				return new ValidateQueryModel();
			}
			string text = " select " + 966 + " as MCode from t_fc_fapiaomodule where " + JoinParameters("MFastCode", list, ref list3) + " and MOrgID = @MOrgID and MIsDelete = 0  and MLCID = @MLocaleID ";
			if (list2 != null && list2.Count > 0)
			{
				text = text + " and MID not in('" + string.Join("','", list2) + "') ";
			}
			return new ValidateQueryModel(text, list3.ToArray());
		}

		public ValidateQueryModel GetValidateCreatedDepreciationVoucher(int year, int period)
		{
			string querySQL = " SELECT\r\n                                (CASE\r\n                                    WHEN COUNT(t1.MItemID) > 0 THEN " + 945 + "\r\n                                    ELSE " + 999 + "\r\n                                END) as MCode\r\n                            FROM\r\n                                t_gl_periodtransfer t1\r\n                                inner join\r\n                                t_gl_voucher t2\r\n                                on t2.MOrgID = t1.MOrgID\r\n                                and t2.MIsDelete = t1.MisDelete\r\n                            WHERE\r\n                                t1.morgid = @MOrgID\r\n                                    AND t1.MTransferTypeID = " + 2 + "\r\n                                    AND t1.MYear = @DepYear\r\n                                    AND t1.MPeriod = @DepPeriod\r\n                                    AND t1.MIsDelete = 0 ";
			MySqlParameter[] parameter = new MySqlParameter[2]
			{
				new MySqlParameter("@DepYear", year),
				new MySqlParameter("@DepPeriod", period)
			};
			return new ValidateQueryModel(querySQL, parameter);
		}

		public ValidateQueryModel GetValidateDepreciatedBeforePeriod(MContext ctx, int year, int period)
		{
			int num = year * 12 + period;
			DateTime mFABeginDate = ctx.MFABeginDate;
			int num2 = num - mFABeginDate.Year * 12;
			mFABeginDate = ctx.MFABeginDate;
			int num3 = num2 - mFABeginDate.Month;
			int num4 = year * 12 + period;
			mFABeginDate = ctx.MFABeginDate;
			int num5 = mFABeginDate.Year * 12;
			mFABeginDate = ctx.MFABeginDate;
			int num6 = num5 + mFABeginDate.Month;
			if (num3 == 0)
			{
				return new ValidateQueryModel();
			}
			GLPeriodTransferRepository gLPeriodTransferRepository = new GLPeriodTransferRepository();
			mFABeginDate = ctx.MFABeginDate;
			int num7 = mFABeginDate.Year * 12;
			mFABeginDate = ctx.MFABeginDate;
			List<GLPeriodTransferModel> modelByPeriodAndType = gLPeriodTransferRepository.GetModelByPeriodAndType(ctx, num7 + mFABeginDate.Month, year * 12 + period - 1, 2, null);
			int num8;
			if (modelByPeriodAndType != null && modelByPeriodAndType.Count != 0)
			{
				num8 = modelByPeriodAndType.Max((GLPeriodTransferModel x) => x.MYear * 12 + x.MPeriod) + 1;
			}
			else
			{
				mFABeginDate = ctx.MFABeginDate;
				int num9 = mFABeginDate.Year * 12;
				mFABeginDate = ctx.MFABeginDate;
				num8 = num9 + mFABeginDate.Month;
			}
			int num10 = num8;
			int num11 = year * 12 + period - 1;
			for (int i = num10; i <= num11; i++)
			{
				int year2 = (i - 1) / 12;
				int period2 = (i - 1) % 12 + 1;
				List<FADepreciationModel> detailDepreciationList = new FADepreciationRepository().GetDetailDepreciationList(ctx, new FAFixAssetsFilterModel
				{
					Year = year2,
					Period = period2
				});
				if (detailDepreciationList != null && detailDepreciationList.Count > 0)
				{
					return new ValidateQueryModel(MActionResultCodeEnum.MExistsUnDepreciatedPeriodBefore);
				}
			}
			return new ValidateQueryModel();
		}

		public ValidateQueryModel GetCheckDepreciationVoucherApproved(int year, int period)
		{
			string querySQL = "SELECT\r\n                                CASE\r\n                                    WHEN COUNT(*) > 0 THEN " + 951 + "\r\n                                    ELSE " + 999 + "\r\n                                END AS MCode\r\n                            FROM\r\n                                t_gl_voucher t1\r\n                                    INNER JOIN\r\n                                t_gl_periodtransfer t2 ON t1.MItemID = t2.MVoucherID\r\n                                    AND t1.MYear = t2.MYear\r\n                                    AND t1.MPeriod = t2.MPeriod\r\n                                    AND t2.MTransferTypeID = " + 2 + "\r\n                                    AND t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MIsDelete = t2.MIsDelete\r\n                            WHERE\r\n                                t1.MOrgID = @MOrgID AND t1.MYear = " + year + "\r\n                                    AND t1.MPeriod = " + period + "\r\n                                    AND t1.MIsDelete = 0\r\n                                    AND t1.MStatus =  " + 1 + " ";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateCreatedDepreciatedVoucherInHandledPeriod(List<string> itemIDs)
		{
			string querySQL = " SELECT\r\n                                (CASE\r\n                                    WHEN COUNT(t1.MItemID) > 0 THEN " + 948 + "\r\n                                    ELSE " + 999 + "\r\n                                END) as MCode\r\n                            FROM\r\n                                t_gl_periodtransfer t1\r\n                                inner join\r\n                                t_gl_voucher t2\r\n                                on t2.MOrgID = t1.MOrgID\r\n                                and t2.MIsDelete = t1.MisDelete\r\n                                inner join t_fa_fixassets t3\r\n                                on t3.MOrgID = t1.MOrgID\r\n                                and t3.MIsDelete = t1.MIsDelete\r\n                            WHERE\r\n                                t1.morgid = @MOrgID\r\n                                    AND t1.MTransferTypeID = " + 2 + "\r\n                                    AND t3.MItemID in ('" + string.Join("','", itemIDs) + "')\r\n                                    AND t1.MYear = year(t3.MHandledDate)\r\n                                    AND t1.MPeriod = month(t3.MHandledDate)\r\n                                    AND t1.MIsDelete = 0 ";
			return new ValidateQueryModel(querySQL, null);
		}

		private string JoinParameters(string columnName, List<string> paramValues, ref List<MySqlParameter> parameters)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < paramValues.Count; i++)
			{
				string text = "@" + columnName + i;
				list.Add(" " + columnName + " = " + text + " ");
				parameters.Add(new MySqlParameter(text, paramValues[i]));
			}
			return " ( " + string.Join(" OR ", list) + " ) ";
		}

		public ValidateQueryModel GetValidateInvoiceSaleNumberSql(List<IVInvoiceModel> invoices)
		{
			string text = string.Empty;
			if (invoices == null)
			{
				return new ValidateQueryModel();
			}
			invoices = (from x in invoices
			where x.MType == "Invoice_Sale" || x.MType == "Invoice_Sale_Red"
			select x).ToList();
			if (invoices.Count == 0)
			{
				return new ValidateQueryModel();
			}
			List<string> list = (from x in invoices
			select x.MNumber).ToList();
			if (list.Count < list.Distinct().Count())
			{
				text = " select " + 906 + " as MCode ";
			}
			else
			{
				List<string> oldNumbers = new List<string>();
				List<string> newNumbers = new List<string>();
				List<string> oldIds = new List<string>();
				invoices.ForEach(delegate(IVInvoiceModel x)
				{
					if (string.IsNullOrWhiteSpace(x.MID) || x.IsNew)
					{
						newNumbers.Add(x.MNumber);
					}
					else
					{
						oldNumbers.Add(x.MNumber);
						oldIds.Add(x.MID);
					}
				});
				if (newNumbers.Count > 0)
				{
					text = text + " select (case when  count(MID) > 0 then  " + 906 + " else " + 999 + " end) as MCode from t_iv_invoice where MOrgID = @MOrgID and ( MType = 'Invoice_Sale_Red' || MType = 'Invoice_Sale') and MIsDelete = 0 and MNumber in('" + string.Join("','", newNumbers) + "')";
				}
				if (oldNumbers.Count > 0)
				{
					text += ((newNumbers.Count > 0) ? " union all " : "");
					text = text + " select (case when count(MID) > 0 then  " + 906 + " else " + 999 + " end) as MCode from t_iv_invoice where MOrgID = @MOrgID and MIsDelete = 0  and MNumber in(' " + string.Join("','", newNumbers) + "') and MID not in ('" + string.Join("','", oldIds) + "')";
				}
			}
			return new ValidateQueryModel(text, new MySqlParameter[0]);
		}

		public ValidateQueryModel GetValidatePeriodTransferSql(int year, int period, List<int> transferTypeIDs)
		{
			if (transferTypeIDs == null || transferTypeIDs.Count == 0)
			{
				return new ValidateQueryModel(null, null);
			}
			string querySQL = " select " + 913 + " as MCode from t_gl_periodtransfer where MTransferTypeID in('" + string.Join("','", transferTypeIDs) + "') and MOrgID = @MOrgID and MYear = @MTransferYear and MPeriod = @MTransferPeriod and MIsDelete = 0  ";
			MySqlParameter[] parameter = new MySqlParameter[2]
			{
				new MySqlParameter("@MTransferYear", year),
				new MySqlParameter("@MTransferPeriod", period)
			};
			return new ValidateQueryModel(querySQL, parameter);
		}

		public ValidateQueryModel GetValidateAccountHasSubSql(List<string> ids, bool isCode = false)
		{
			if (ids == null || ids.Count == 0)
			{
				return new ValidateQueryModel();
			}
			string text = null;
			text = (isCode ? (" select " + 916 + " as MCode from t_bd_account where MParentID in ( select MItemID from t_bd_account where MCode in ('" + string.Join("','", ids) + "') and MOrgID = @MOrgID and MIsDelete = 0  ) and MOrgID = @MOrgID and MIsDelete = 0  ") : (" select " + 916 + " as MCode from t_bd_account where MParentID in ('" + string.Join("','", ids) + "') and MOrgID = @MOrgID and MIsDelete = 0  "));
			return new ValidateQueryModel(text, null);
		}

		public ValidateQueryModel GetValidatExpItemHasSubSql(List<string> ids)
		{
			if (ids == null || ids.Count == 0)
			{
				return new ValidateQueryModel();
			}
			string querySQL = " select " + 917 + " as MCode from t_bd_expenseitem where MParentItemID in ('" + string.Join("','", ids) + "') and MOrgID = @MOrgID and MIsDelete = 0  ";
			return new ValidateQueryModel(querySQL, null);
		}

		public ValidateQueryModel GetValidateCommonModelSql<T>(MActionResultCodeEnum errorCode, List<string> ids, string fieldName = null, string otherFilter = null) where T : BaseModel
		{
			ids = (from x in ids ?? new List<string>()
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToList();
			if (ids.Count == 0)
			{
				return new ValidateQueryModel();
			}
			object obj = Activator.CreateInstance(typeof(T));
			object propertyValue = obj.GetPropertyValue("TableName");
			object obj2 = fieldName ?? obj.GetPropertyValue("PKFieldName");
			string querySQL = " select (case when count(" + obj2 + ") < " + ids.Count + " then " + (int)errorCode + " else " + 999 + " end ) as MCode from " + propertyValue + " where MOrgID = @MOrgID and  " + obj2 + " in ('" + string.Join("','", ids) + "') and MIsDelete = 0 " + otherFilter;
			return new ValidateQueryModel(querySQL, null);
		}

		public void CheckInitDocumentCheckGroupValueMatchWithAccount(MContext ctx, List<BDInitBillViewModel> billList)
		{
			List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
			int i;
			for (i = 0; i < billList.Count; i++)
			{
				if (!string.IsNullOrWhiteSpace(billList[i].MCurrentAccountCode))
				{
					BDAccountModel account = accountWithParentList.FirstOrDefault((BDAccountModel x) => x.MCode == billList[i].MCurrentAccountCode);
					if (CheckGroupValueMathWithCheckGroupModel(ctx, billList[i].MCheckGroupValueModel, billList[i].MCheckGroupModel, account, true, false))
					{
						billList[i].MCheckGroupValueModel.MItemID = string.Empty;
						billList[i].MCheckGroupValueModel = GetCheckGroupValueModel(ctx, billList[i].MCheckGroupValueModel);
					}
				}
			}
		}

		public List<MActionResultCodeEnum> QueryValidateSql(MContext ctx, bool throwException = false, params ValidateQueryModel[] query)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			for (int i = 0; i < query.Count(); i++)
			{
				if (query[i] != null)
				{
					list.Add(query[i]);
				}
			}
			return QueryValidateSql(ctx, list, throwException);
		}

		public List<MActionResultCodeEnum> QueryValidateSql(MContext ctx, List<ValidateQueryModel> queryList, bool throwException = false)
		{
			ValidateQueryModel validateQueryModel = new ValidateQueryModel();
			for (int i = 0; i < queryList.Count; i++)
			{
				validateQueryModel = validateQueryModel.Join(queryList[i]);
			}
			DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(validateQueryModel.QuerySQL, ctx.GetParameters((MySqlParameter)null).Concat(validateQueryModel.Parameter).ToArray());
			List<MActionResultCodeEnum> list = new List<MActionResultCodeEnum>();
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				list = (from x in dataSet.Tables[0].AsEnumerable()
				where (int)x.Field<long>("MCode") != 999
				select (MActionResultCodeEnum)x.Field<long>("MCode")).Distinct().ToList();
			}
			if (list.Count > 0 & throwException)
			{
				throw new MActionException
				{
					Codes = list
				};
			}
			return list;
		}

		public List<T> Union<T>(List<List<T>> list)
		{
			List<T> list2 = new List<T>();
			for (int i = 0; i < list.Count; i++)
			{
				list2.AddRange((IEnumerable<T>)list[i]);
			}
			return list2;
		}

		public List<ValidateQueryModel> GetValidateCheckGroupValueModel(List<GLCheckGroupValueModel> models)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			List<string> ids = (from x in models
			select x.MContactID).ToList();
			ValidateQueryModel validateCommonModelSql = GetValidateCommonModelSql<BDContactsModel>(MActionResultCodeEnum.MContactInvalid, ids, null, null);
			List<string> ids2 = (from x in models
			select x.MEmployeeID).ToList();
			ValidateQueryModel validateCommonModelSql2 = GetValidateCommonModelSql<BDEmployeesModel>(MActionResultCodeEnum.MEmployeeInvalid, ids2, null, null);
			List<string> ids3 = (from x in models
			select x.MMerItemID).ToList();
			ValidateQueryModel validateCommonModelSql3 = GetValidateCommonModelSql<BDItemModel>(MActionResultCodeEnum.MMerItemInvalid, ids3, null, null);
			List<string> ids4 = (from x in models
			select x.MExpItemID).ToList();
			ValidateQueryModel validateCommonModelSql4 = GetValidateCommonModelSql<BDExpenseItemModel>(MActionResultCodeEnum.MExpItemInvalid, ids4, null, null);
			ValidateQueryModel validatExpItemHasSubSql = GetValidatExpItemHasSubSql(ids4);
			List<string> ids5 = (from x in models
			where x.MPaItemGroupID != x.MPaItemID && !string.IsNullOrWhiteSpace(x.MPaItemID)
			select x.MPaItemGroupID).ToList();
			ValidateQueryModel validateCommonModelSql5 = GetValidateCommonModelSql<PAPayItemGroupModel>(MActionResultCodeEnum.MPaItemInvalid, ids5, null, null);
			List<string> ids6 = (from x in models
			select x.MPaItemGroupID).ToList();
			ValidateQueryModel validateCommonModelSql6 = GetValidateCommonModelSql<PAPayItemGroupModel>(MActionResultCodeEnum.MPaItemInvalid, ids6, null, null);
			List<List<string>> list2 = (from x in models
			select new List<string>
			{
				x.MTrackItem1,
				x.MTrackItem2,
				x.MTrackItem3,
				x.MTrackItem4,
				x.MTrackItem5
			}).ToList();
			List<string> ids7 = Union(list2);
			ValidateQueryModel validateCommonModelSql7 = GetValidateCommonModelSql<BDTrackEntryModel>(MActionResultCodeEnum.MTrackItemInvalid, ids7, null, null);
			List<List<string>> list3 = (from x in models
			select new List<string>
			{
				x.MTrackItem1GroupID,
				x.MTrackItem2GroupID,
				x.MTrackItem3GroupID,
				x.MTrackItem4GroupID,
				x.MTrackItem5GroupID
			}).ToList();
			List<string> ids8 = Union(list3);
			ValidateQueryModel validateCommonModelSql8 = GetValidateCommonModelSql<BDTrackModel>(MActionResultCodeEnum.MTrackGroupInvalid, ids8, null, null);
			return new List<ValidateQueryModel>
			{
				validateCommonModelSql,
				validateCommonModelSql2,
				validateCommonModelSql3,
				validateCommonModelSql4,
				validatExpItemHasSubSql,
				validateCommonModelSql5,
				validateCommonModelSql6,
				validateCommonModelSql7,
				validateCommonModelSql8
			};
		}

		public GLCheckGroupValueModel GetCheckGroupValueModel(MContext ctx, GLCheckGroupValueModel model)
		{
			model = (model ?? new GLCheckGroupValueModel());
			try
			{
				string str = "SELECT\r\n                                    MItemID\r\n                                FROM\r\n                                    t_gl_checkgroupvalue\r\n                                WHERE\r\n                                    MOrgID = @MOrgID AND MIsDelete = 0\r\n                                        AND ifnull(MContactID,'') = @MContactID\r\n                                        AND ifnull(MEmployeeID,'') = @MEmployeeID\r\n                                        AND ifnull(MMerItemID,'') = @MMerItemID\r\n                                        AND ifnull(MExpItemID,'') = @MExpItemID\r\n                                        AND ifnull(MPaItemID,'') = @MPaItemID\r\n                                        AND ifnull(MTrackItem1,'') = @MTrackItem1\r\n                                        AND ifnull(MTrackItem2,'') = @MTrackItem2\r\n                                        AND ifnull(MTrackItem3,'') = @MTrackItem3\r\n                                        AND ifnull(MTrackItem4,'') = @MTrackItem4\r\n                                        AND ifnull(MTrackItem5,'') = @MTrackItem5 ";
				str += " limit 0,1 ";
				MySqlParameter[] cmdParms = ctx.GetParameters((MySqlParameter)null).Concat(new MySqlParameter[11]
				{
					new MySqlParameter("@MContactID", Convert2Empty(model.MContactID)),
					new MySqlParameter("@MEmployeeID", Convert2Empty(model.MEmployeeID)),
					new MySqlParameter("@MMerItemID", Convert2Empty(model.MMerItemID)),
					new MySqlParameter("@MExpItemID", Convert2Empty(model.MExpItemID)),
					new MySqlParameter("@MPaItemID", Convert2Empty(model.MPaItemID)),
					new MySqlParameter("@MTrackItem1", Convert2Empty(model.MTrackItem1)),
					new MySqlParameter("@MTrackItem2", Convert2Empty(model.MTrackItem2)),
					new MySqlParameter("@MTrackItem3", Convert2Empty(model.MTrackItem3)),
					new MySqlParameter("@MTrackItem4", Convert2Empty(model.MTrackItem4)),
					new MySqlParameter("@MTrackItem5", Convert2Empty(model.MTrackItem5)),
					new MySqlParameter("@MItemID", model.MItemID)
				}).ToArray();
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				DataSet dataSet = dynamicDbHelperMySQL.Query(str, cmdParms);
				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					model.MItemID = dataSet.Tables[0].Rows[0].Field<string>("MItemID");
				}
				else
				{
					if (!string.IsNullOrWhiteSpace(model.MItemID))
					{
						model.MItemID = null;
					}
					List<CommandInfo> insertOrUpdateCmd = ModelInfoManager.GetInsertOrUpdateCmd<GLCheckGroupValueModel>(ctx, model, null, true);
					dynamicDbHelperMySQL.ExecuteSqlTran(insertOrUpdateCmd);
				}
				return model;
			}
			catch (Exception innerException)
			{
				throw new Exception("科目核算维度值组合插入失败", innerException);
			}
		}

		public List<GLCheckGroupValueModel> GetCheckGroupValueModelByID(MContext ctx, List<string> itemIDs)
		{
			string text = "SELECT\r\n                t1.MOrgID,\r\n                t1.MItemID,\r\n                t1.MContactID,\r\n                t1.MEmployeeID,\r\n                t1.MMerItemID,\r\n                t1.MExpItemID,\r\n                t1.MPaItemID,\r\n                t1.MTrackItem1,\r\n                t1.MTrackItem2,\r\n                t1.MTrackItem3,\r\n                t1.MTrackItem4,\r\n                t1.MTrackItem5,\r\n                CONVERT( AES_DECRYPT(t6.MName, '{0}') USING UTF8) AS MContactName,\r\n                F_GETUSERNAME(t7.MFirstName, t7.MLastName) AS MEmployeeName,\r\n                concat(t8_0.MNumber,':',t8.MDesc) AS MMerItemName,\r\n                t9.MName AS MExpItemName,\r\n                t10.MName AS MPaItemName,\r\n                t10_0.MGroupID AS MPaItemGroupID,\r\n                t10_1.MName AS MPaItemGroupName,\r\n                t11.MItemID AS MTrackItem1GroupID,\r\n                t12.MName AS MTrackItem1GroupName,\r\n                t13.MName AS MTrackItem1Name,\r\n                t14.MItemID AS MTrackItem2GroupID,\r\n                t15.MName AS MTrackItem2GroupName,\r\n                t16.MName AS MTrackItem2Name,\r\n                t17.MItemID AS MTrackItem3GroupID,\r\n                t18.MName AS MTrackItem3GroupName,\r\n                t19.MName AS MTrackItem3Name,\r\n                t20.MItemID AS MTrackItem4GroupID,\r\n                t21.MName AS MTrackItem4GroupName,\r\n                t22.MName AS MTrackItem4Name,\r\n                t23.MItemID AS MTrackItem5GroupID,\r\n                t24.MName AS MTrackItem5GroupName,\r\n                t25.MName AS MTrackItem5Name\r\n            FROM\r\n                t_gl_checkgroupvalue t1\r\n                    LEFT JOIN\r\n                t_bd_contacts_l t6 ON t6.MParentID = t1.MContactID\r\n                    AND t6.MOrgID = t1.MOrgId\r\n                    AND t6.MIsDelete = t1.MIsDelete\r\n                    AND t6.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_employees_l t7 ON t7.MParentID = t1.MEmployeeID\r\n                    AND t7.MOrgID = t1.MOrgId\r\n                    AND t7.MIsDelete = t1.MIsDelete\r\n                    AND t7.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_item t8_0 ON t8_0.MItemID = t1.MMerItemID\r\n                    AND t8_0.MOrgID = t1.MOrgId\r\n                    AND t8_0.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_item_l t8 ON t8.MParentID = t1.MMerItemID\r\n                    AND t8.MOrgID = t1.MOrgId\r\n                    AND t8.MIsDelete = t1.MIsDelete\r\n                    AND t8.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_expenseitem_l t9 ON t9.MParentID = t1.MExpItemID\r\n                    AND t9.MOrgID = t1.MOrgId\r\n                    AND t9.MIsDelete = t1.MIsDelete\r\n                    AND t9.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem_l t10 ON t10.MParentID = t1.MPaItemID\r\n                    AND t10.MOrgID = t1.MOrgId\r\n                    AND t10.MIsDelete = t1.MIsDelete\r\n                    AND t10.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_pa_payitem t10_0 ON t10_0.MItemID = t1.MPaItemID\r\n                    AND t10_0.MOrgID = t1.MOrgId\r\n                    AND t10_0.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_pa_payitemgroup_l t10_1 ON t10_1.MParentID = t1.MPaItemID\r\n                    AND t10_1.MOrgID = t1.MOrgId\r\n                    AND t10_1.MIsDelete = t1.MIsDelete\r\n                    AND t10_1.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t11 ON t11.MEntryID = t1.MTrackItem1\r\n                    AND t11.MOrgID = t1.MOrgID\r\n                    AND t11.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t12 ON t12.MParentID = t11.MItemID\r\n                    AND t12.MOrgID = t1.MOrgID\r\n                    AND t12.MIsDelete = t1.MIsDelete\r\n                    AND t12.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t13 ON t13.MParentID = t1.MTrackItem1\r\n                    AND t13.MOrgID = t1.MOrgId\r\n                    AND t13.MIsDelete = t1.MIsDelete\r\n                    AND t13.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t14 ON t14.MEntryID = t1.MTrackItem2\r\n                    AND t14.MOrgID = t1.MOrgID\r\n                    AND t14.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t15 ON t15.MParentID = t14.MItemID\r\n                    AND t15.MOrgID = t1.MOrgID\r\n                    AND t15.MIsDelete = t1.MIsDelete\r\n                    AND t15.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t16 ON t16.MParentID = t1.MTrackItem2\r\n                    AND t16.MOrgID = t1.MOrgId\r\n                    AND t16.MIsDelete = t1.MIsDelete\r\n                    AND t16.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t17 ON t17.MEntryID = t1.MTrackItem3\r\n                    AND t17.MOrgID = t1.MOrgID\r\n                    AND t17.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t18 ON t18.MParentID = t17.MItemID\r\n                    AND t18.MOrgID = t1.MOrgID\r\n                    AND t18.MIsDelete = t1.MIsDelete\r\n                    AND t18.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t19 ON t19.MParentID = t1.MTrackItem3\r\n                    AND t19.MOrgID = t1.MOrgId\r\n                    AND t19.MIsDelete = t1.MIsDelete\r\n                    AND t19.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t20 ON t20.MEntryID = t1.MTrackItem4\r\n                    AND t20.MOrgID = t1.MOrgID\r\n                    AND t20.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t21 ON t21.MParentID = t20.MItemID\r\n                    AND t21.MOrgID = t1.MOrgID\r\n                    AND t21.MIsDelete = t1.MIsDelete\r\n                    AND t21.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t22 ON t22.MParentID = t1.MTrackItem4\r\n                    AND t22.MOrgID = t1.MOrgID\r\n                    AND t22.MIsDelete = t1.MIsDelete\r\n                    AND t22.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry t23 ON t23.MEntryID = t1.MTrackItem5\r\n                    AND t23.MOrgID = t1.MOrgID\r\n                    AND t23.MIsDelete = t1.MIsDelete\r\n                    LEFT JOIN\r\n                t_bd_track_l t24 ON t24.MParentID = t23.MItemID\r\n                    AND t24.MOrgID = t1.MOrgID\r\n                    AND t24.MIsDelete = t1.MIsDelete\r\n                    AND t24.MLocaleID = @MLocaleID\r\n                    LEFT JOIN\r\n                t_bd_trackentry_l t25 ON t25.MParentID = t1.MTrackItem5\r\n                    AND t25.MOrgID = t1.MOrgId\r\n                    AND t25.MIsDelete = t1.MIsDelete\r\n                    AND t25.MLocaleID = @MLocaleID\r\n            WHERE\r\n                t1.MOrgID = @MOrgID\r\n                    AND t1.MIsDelete = 0 ";
			if (itemIDs != null && itemIDs.Count > 0)
			{
				text = ((itemIDs.Count != 1) ? (text + " and t1.MItemID in ('" + string.Join("','", itemIDs) + "') ") : (text + " and t1.MItemID = '" + itemIDs[0] + "'"));
			}
			text = string.Format(text, "JieNor-001");
			List<GLCheckGroupValueModel> list = ModelInfoManager.GetDataModelBySql<GLCheckGroupValueModel>(ctx, text, ctx.GetParameters((MySqlParameter)null)) ?? new List<GLCheckGroupValueModel>();
			list.ForEach(delegate(GLCheckGroupValueModel x)
			{
				if (string.IsNullOrWhiteSpace(x.MPaItemIDName) && !string.IsNullOrWhiteSpace(x.MPaItemGroupName))
				{
					x.MPaItemName = x.MPaItemGroupName;
				}
			});
			return list;
		}

		public GLCheckGroupValueModel GetCheckGroupValueModelByID(MContext ctx, string itemID)
		{
			List<GLCheckGroupValueModel> checkGroupValueModelByID = GetCheckGroupValueModelByID(ctx, new List<string>
			{
				itemID
			});
			return (checkGroupValueModelByID.Count > 0) ? checkGroupValueModelByID[0] : new GLCheckGroupValueModel();
		}

		public GLCheckGroupModel GetCheckGroupModelByID(MContext ctx, string itemID)
		{
			string sql = "select * from t_gl_checkgroup where MItemID = @MItemID";
			MySqlParameter[] cmdParms = new MySqlParameter[1]
			{
				new MySqlParameter("@MItemID", itemID)
			};
			return ModelInfoManager.GetDataModel<GLCheckGroupModel>(ctx, sql, cmdParms);
		}

		public void SetVoucherCheckGroupValueID(MContext ctx, List<GLVoucherModel> list)
		{
			List<GLVoucherEntryModel> list2 = Union((from x in list
			select x.MVoucherEntrys).ToList());
			list2.ForEach(delegate(GLVoucherEntryModel x)
			{
				x.MCheckGroupValueID = GetCheckGroupValueModel(ctx, x.MCheckGroupValueModel).MItemID;
			});
		}

		public void CheckVoucherCheckGroupValueMatchCheckGroup(MContext ctx, List<GLVoucherModel> list, bool clearCheckGroupValue = false)
		{
			list = (from x in list
			where !x.IsDelete
			select x).ToList();
			List<GLVoucherEntryModel> entryList = Union((from x in list
			select (from y in x.MVoucherEntrys
			where !string.IsNullOrWhiteSpace(y.MAccountID)
			select y).ToList()).ToList());
			List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
			int i;
			for (i = 0; i < entryList.Count; i++)
			{
				if (!string.IsNullOrWhiteSpace(entryList[i].MAccountID))
				{
					BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == entryList[i].MAccountID);
					if (CheckGroupValueMathWithCheckGroupModel(ctx, entryList[i].MCheckGroupValueModel, bDAccountModel.MCheckGroupModel, bDAccountModel, clearCheckGroupValue, false))
					{
						entryList[i].MCheckGroupValueModel.MItemID = null;
						entryList[i].MCheckGroupValueModel = GetCheckGroupValueModel(ctx, entryList[i].MCheckGroupValueModel);
						entryList[i].MCheckGroupValueID = entryList[i].MCheckGroupValueModel.MItemID;
					}
				}
			}
		}

		public List<BizVerificationInfor> CheckVoucherCheckGroupValueMatchCheckGroup(MContext ctx, GLVoucherModel voucher, bool clearCheckGroupValue = false)
		{
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			if (voucher.IsDelete || voucher.MVoucherEntrys == null || voucher.MVoucherEntrys.Count == 0)
			{
				return list;
			}
			List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
			int i;
			for (i = 0; i < voucher.MVoucherEntrys.Count; i++)
			{
				try
				{
					if (!string.IsNullOrWhiteSpace(voucher.MVoucherEntrys[i].MAccountID))
					{
						BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == voucher.MVoucherEntrys[i].MAccountID);
						if (CheckGroupValueMathWithCheckGroupModel(ctx, voucher.MVoucherEntrys[i].MCheckGroupValueModel, bDAccountModel.MCheckGroupModel, bDAccountModel, clearCheckGroupValue, false))
						{
							voucher.MVoucherEntrys[i].MCheckGroupValueModel.MItemID = null;
							voucher.MVoucherEntrys[i].MCheckGroupValueModel = GetCheckGroupValueModel(ctx, voucher.MVoucherEntrys[i].MCheckGroupValueModel);
							voucher.MVoucherEntrys[i].MCheckGroupValueID = voucher.MVoucherEntrys[i].MCheckGroupValueModel.MItemID;
						}
					}
				}
				catch (MActionException ex)
				{
					BizVerificationInfor item = new BizVerificationInfor
					{
						RowIndex = voucher.MRowIndex,
						Message = string.Join(" ", ex.Messages).Replace("<div class='m-error-title'>", "").Replace("</div>", " ")
							.Replace("<div class='m-error-body'>", "")
					};
					list.Add(item);
				}
			}
			return list;
		}

		public void CheckVoucherEntryCheckGroupValueMatchCheckGroup(MContext ctx, GLVoucherEntryModel entry, bool clearCheckGroupValue = false)
		{
			if (!string.IsNullOrWhiteSpace(entry.MAccountID))
			{
				List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
				BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == entry.MAccountID);
				if (CheckGroupValueMathWithCheckGroupModel(ctx, entry.MCheckGroupValueModel, bDAccountModel.MCheckGroupModel, bDAccountModel, clearCheckGroupValue, false))
				{
					entry.MCheckGroupValueModel.MItemID = null;
					entry.MCheckGroupValueModel = GetCheckGroupValueModel(ctx, entry.MCheckGroupValueModel);
					entry.MCheckGroupValueID = entry.MCheckGroupValueModel.MItemID;
				}
			}
		}

		public bool CheckVoucheCurrencyMatchAccount(MContext ctx, List<GLVoucherModel> list, bool clearCurrencyValue = false)
		{
			List<GLVoucherEntryModel> entryList = Union((from x in list
			select (from y in x.MVoucherEntrys
			where !string.IsNullOrWhiteSpace(y.MAccountID)
			select y).ToList()).ToList());
			List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
			int i;
			for (i = 0; i < entryList.Count; i++)
			{
				if (!string.IsNullOrWhiteSpace(entryList[i].MAccountID))
				{
					BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == entryList[i].MAccountID);
					if (!bDAccountModel.MIsCheckForCurrency && entryList[i].MCurrencyID != ctx.MBasCurrencyID)
					{
						if (!clearCurrencyValue)
						{
							return false;
						}
						entryList[i].MCurrencyID = ctx.MBasCurrencyID;
						entryList[i].MAmountFor = entryList[i].MAmount;
						entryList[i].MExchangeRate = 1.0m;
					}
				}
			}
			return true;
		}

		public bool CheckVoucherModuleCheckGroupValueMatchCheckGroup(MContext ctx, List<FCVoucherModuleModel> list)
		{
			List<FCVoucherModuleEntryModel> entryList = Union((from x in list
			select (from y in x.MVoucherModuleEntrys
			where !string.IsNullOrWhiteSpace(y.MAccountID)
			select y).ToList()).ToList());
			List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
			int i;
			for (i = 0; i < entryList.Count; i++)
			{
				if (!string.IsNullOrWhiteSpace(entryList[i].MAccountID))
				{
					BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MItemID == entryList[i].MAccountID);
					if (CheckGroupValueMathWithCheckGroupModel(ctx, entryList[i].MCheckGroupValueModel, bDAccountModel.MCheckGroupModel, bDAccountModel, true, false))
					{
						entryList[i].MCheckGroupValueModel.MItemID = null;
						entryList[i].MCheckGroupValueModel = GetCheckGroupValueModel(ctx, entryList[i].MCheckGroupValueModel);
						entryList[i].MCheckGroupValueID = entryList[i].MCheckGroupValueModel.MItemID;
					}
				}
			}
			return true;
		}

		public bool CheckGroupValueMathWithCheckGroupModel(MContext ctx, GLCheckGroupValueModel value, GLCheckGroupModel group, BDAccountModel account, bool clearCheckGroupValue = false, bool ignoreRequired = false)
		{
			bool result = false;
			if (value != null && group != null)
			{
				List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
				int num = IsCheckTypeValueValid(value.MContactID, group.MContactID, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(0, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MContactID = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(0, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MEmployeeID, group.MEmployeeID, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(1, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MEmployeeID = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(1, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MMerItemID, group.MMerItemID, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(2, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MMerItemID = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(2, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MExpItemID, group.MExpItemID, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(3, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MExpItemID = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(3, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MPaItemID, group.MPaItemID, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(4, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MPaItemID = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(4, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MTrackItem1, group.MTrackItem1, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(5, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MTrackItem1 = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(5, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MTrackItem2, group.MTrackItem2, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(6, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MTrackItem2 = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(6, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MTrackItem3, group.MTrackItem3, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(7, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MTrackItem3 = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(7, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MTrackItem4, group.MTrackItem4, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(8, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MTrackItem4 = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(8, num));
						result = false;
					}
					break;
				}
				num = IsCheckTypeValueValid(value.MTrackItem5, group.MTrackItem5, ignoreRequired);
				switch (num)
				{
				case -1:
					list.Add(new KeyValuePair<int, int>(9, num));
					break;
				case 0:
					if (clearCheckGroupValue)
					{
						value.MTrackItem5 = null;
						result = true;
					}
					else
					{
						list.Add(new KeyValuePair<int, int>(9, num));
						result = false;
					}
					break;
				}
				if (list.Count > 0)
				{
					MActionException ex = new MActionException(new List<MActionResultCodeEnum>
					{
						MActionResultCodeEnum.MCheckGroupValueNotMatchWithAccount
					});
					ex.Messages = new List<string>
					{
						"<div class='m-error-title'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountName", "科目名称:") + MText.Encode(account.MFullName) + "</div>"
					};
					ex.Messages.AddRange(AssembleErrorMessage(ctx, list));
					ex.Messages.Add("<div class='m-error-title'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "PleaseModifyYourBill", "请在单据补充相应信息或者修改科目信息!") + "</div>");
					throw ex;
				}
			}
			return result;
		}

		public GLCheckGroupValueModel FilterCheckGroupValueModelByCheckGroup(MContext ctx, GLCheckGroupValueModel value, BDAccountModel account, bool throwException = false)
		{
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>();
			GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel();
			GLCheckGroupModel mCheckGroupModel = account.MCheckGroupModel;
			gLCheckGroupValueModel.MContactID = GetValueByGroupEnableStatus(value.MContactID, mCheckGroupModel.MContactID, CheckTypeEnum.MContactID, ref list);
			gLCheckGroupValueModel.MEmployeeID = GetValueByGroupEnableStatus(value.MEmployeeID, mCheckGroupModel.MEmployeeID, CheckTypeEnum.MEmployeeID, ref list);
			gLCheckGroupValueModel.MMerItemID = GetValueByGroupEnableStatus(value.MMerItemID, mCheckGroupModel.MMerItemID, CheckTypeEnum.MMerItemID, ref list);
			gLCheckGroupValueModel.MExpItemID = GetValueByGroupEnableStatus(value.MExpItemID, mCheckGroupModel.MExpItemID, CheckTypeEnum.MExpItemID, ref list);
			gLCheckGroupValueModel.MPaItemID = GetValueByGroupEnableStatus(value.MPaItemID, mCheckGroupModel.MPaItemID, CheckTypeEnum.MPaItemID, ref list);
			gLCheckGroupValueModel.MTrackItem1 = GetValueByGroupEnableStatus(value.MTrackItem1, mCheckGroupModel.MTrackItem1, CheckTypeEnum.MTrackItem1, ref list);
			gLCheckGroupValueModel.MTrackItem2 = GetValueByGroupEnableStatus(value.MTrackItem2, mCheckGroupModel.MTrackItem2, CheckTypeEnum.MTrackItem2, ref list);
			gLCheckGroupValueModel.MTrackItem3 = GetValueByGroupEnableStatus(value.MTrackItem3, mCheckGroupModel.MTrackItem3, CheckTypeEnum.MTrackItem3, ref list);
			gLCheckGroupValueModel.MTrackItem4 = GetValueByGroupEnableStatus(value.MTrackItem4, mCheckGroupModel.MTrackItem4, CheckTypeEnum.MTrackItem4, ref list);
			gLCheckGroupValueModel.MTrackItem5 = GetValueByGroupEnableStatus(value.MTrackItem5, mCheckGroupModel.MTrackItem5, CheckTypeEnum.MTrackItem5, ref list);
			if (list.Count > 0 & throwException)
			{
				MActionException ex = new MActionException(new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MCheckGroupValueNotMatchWithAccount
				});
				ex.Messages = new List<string>
				{
					"<div class='m-error-title'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "AccountName", "科目名称:") + MText.Encode(account.MFullName) + "</div>"
				};
				ex.Messages.AddRange(AssembleErrorMessage(ctx, list));
				ex.Messages.Add("<div class='m-error-title'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "PleaseModifyYourBill", "请在单据补充相应信息或者修改科目信息!") + "</div>");
				throw ex;
			}
			return GetCheckGroupValueModel(ctx, gLCheckGroupValueModel);
		}

		public string GetValueByGroupEnableStatus(string value, int status, CheckTypeEnum type, ref List<KeyValuePair<int, int>> columnList)
		{
			if (status == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(value))
			{
				columnList.Add(new KeyValuePair<int, int>((int)type, -1));
			}
			return (status != CheckTypeStatusEnum.Optional && status != CheckTypeStatusEnum.Required) ? null : value;
		}

		public List<string> AssembleErrorMessage(MContext ctx, List<KeyValuePair<int, int>> columnList)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < columnList.Count; i++)
			{
				string empty = string.Empty;
				KeyValuePair<int, int> keyValuePair = columnList[i];
				if (keyValuePair.Value == -1)
				{
					List<string> list2 = list;
					string format = "<div class='m-error-body'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CheckValueRequiredError", "[{0}] 核算维度启用状态为必录，但单据却没有录入相应的值;");
					keyValuePair = columnList[i];
					list2.Add(string.Format(format, MText.Encode(GetCheckTypeName(ctx, keyValuePair.Key))) + "</div>");
				}
				else
				{
					List<string> list3 = list;
					string format2 = "<div class='m-error-body'>" + COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "CheckValueNotRequiredErroe", "[{0}] 核算维度启用状态为禁用，但单据却录入了相应的值;");
					keyValuePair = columnList[i];
					list3.Add(string.Format(format2, MText.Encode(GetCheckTypeName(ctx, keyValuePair.Key))) + "</div>");
				}
			}
			return list;
		}

		public bool ValidateUpdateAccount(MContext ctx, BDAccountModel model)
		{
			if (model != null)
			{
				List<BDAccountModel> accountWithParentList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountWithParentList;
				if (string.IsNullOrWhiteSpace(model.MItemID) || model.IsNew)
				{
					List<BDAccountModel> list = (from x in accountWithParentList
					where x.MParentID == model.MParentID
					select x).ToList();
					if (list != null && list.Count >= 1)
					{
						return true;
					}
				}
				List<GLVoucherEntryModel> list2 = Union((from x in new GLVoucherRepository().GetVoucherModelList(ctx, null, false, 0, 0)
				select (from y in x.MVoucherEntrys
				where y.MAccountID == model.MParentID
				select y).ToList()).ToList());
				for (int i = 0; i < list2.Count; i++)
				{
					CheckGroupValueMathWithCheckGroupModel(ctx, list2[i].MCheckGroupValueModel, list2[i].MAccountModel.MCheckGroupModel, list2[i].MAccountModel, false, false);
				}
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.AddFilter("MAccountID", SqlOperators.Equal, model.MParentID);
				sqlWhere.AddFilter("MCheckGroupValueID", SqlOperators.NotEqual, "0");
				List<GLInitBalanceModel> initBalanceListIncludeCheckGroupValue = new GLInitBalanceRepository().GetInitBalanceListIncludeCheckGroupValue(ctx, sqlWhere);
				for (int j = 0; j < initBalanceListIncludeCheckGroupValue.Count; j++)
				{
					CheckGroupValueMathWithCheckGroupModel(ctx, initBalanceListIncludeCheckGroupValue[j].MCheckGroupValueModel, initBalanceListIncludeCheckGroupValue[j].MAccountModel.MCheckGroupModel, initBalanceListIncludeCheckGroupValue[j].MAccountModel, false, false);
				}
				List<GLBalanceModel> balanceListIncludeCheckGroupValue = new GLBalanceRepository().GetBalanceListIncludeCheckGroupValue(ctx, sqlWhere, false);
				for (int k = 0; k < balanceListIncludeCheckGroupValue.Count; k++)
				{
					CheckGroupValueMathWithCheckGroupModel(ctx, balanceListIncludeCheckGroupValue[k].MCheckGroupValueModel, balanceListIncludeCheckGroupValue[k].MAccountModel.MCheckGroupModel, balanceListIncludeCheckGroupValue[k].MAccountModel, false, false);
				}
			}
			return true;
		}

		private int IsCheckTypeValueValid(string checkGroupValue, int status, bool ignoreRequired = false)
		{
			if (status == CheckTypeStatusEnum.Required && string.IsNullOrWhiteSpace(checkGroupValue))
			{
				return (!ignoreRequired) ? (-1) : 0;
			}
			if (!string.IsNullOrWhiteSpace(checkGroupValue) && status != CheckTypeStatusEnum.Optional && status != CheckTypeStatusEnum.Required)
			{
				return 0;
			}
			return 1;
		}

		public string Convert2Empty(string value)
		{
			return string.IsNullOrWhiteSpace(value) ? "" : value;
		}

		public GLCheckGroupValueModel GetCheckGroupValueModelFromObject(MContext ctx, object obj)
		{
			GLCheckGroupValueModel model = (obj == null) ? new GLCheckGroupValueModel() : new GLCheckGroupValueModel
			{
				MContactID = ToString(obj.TryGetPropertyValue("MContactID")),
				MEmployeeID = ToString(obj.TryGetPropertyValue("MEmployeeID")),
				MMerItemID = ToString(obj.TryGetPropertyValue("MMerItemID")),
				MExpItemID = ToString(obj.TryGetPropertyValue("MExpItemID")),
				MPaItemID = ToString(obj.TryGetPropertyValue("MPaItemID")),
				MTrackItem1 = ToString(obj.TryGetPropertyValue("MTrackItem1")),
				MTrackItem2 = ToString(obj.TryGetPropertyValue("MTrackItem2")),
				MTrackItem3 = ToString(obj.TryGetPropertyValue("MTrackItem3")),
				MTrackItem4 = ToString(obj.TryGetPropertyValue("MTrackItem4")),
				MTrackItem5 = ToString(obj.TryGetPropertyValue("MTrackItem5"))
			};
			return GetCheckGroupValueModel(ctx, model);
		}

		public GLCheckGroupValueModel CopyCheckGroupValueModel(GLCheckGroupValueModel src)
		{
			return new GLCheckGroupValueModel
			{
				MOrgID = src.MOrgID,
				MItemID = src.MItemID,
				MContactID = src.MContactID,
				MEmployeeID = src.MEmployeeID,
				MMerItemID = src.MMerItemID,
				MExpItemID = src.MExpItemID,
				MPaItemID = src.MPaItemID,
				MTrackItem1 = src.MTrackItem1,
				MTrackItem2 = src.MTrackItem2,
				MTrackItem3 = src.MTrackItem3,
				MTrackItem4 = src.MTrackItem4,
				MTrackItem5 = src.MTrackItem5
			};
		}

		public GLCheckGroupValueModel MergeCheckGroupValueModel(GLCheckGroupValueModel a, GLCheckGroupValueModel b)
		{
			if (a == null)
			{
				return b;
			}
			if (b == null)
			{
				return a;
			}
			return new GLCheckGroupValueModel
			{
				MOrgID = a.MOrgID,
				MContactID = (string.IsNullOrWhiteSpace(a.MContactID) ? b.MContactID : a.MContactID),
				MEmployeeID = (string.IsNullOrWhiteSpace(a.MEmployeeID) ? b.MEmployeeID : a.MEmployeeID),
				MMerItemID = (string.IsNullOrWhiteSpace(a.MMerItemID) ? b.MMerItemID : a.MMerItemID),
				MExpItemID = (string.IsNullOrWhiteSpace(a.MExpItemID) ? b.MExpItemID : a.MExpItemID),
				MPaItemID = (string.IsNullOrWhiteSpace(a.MPaItemID) ? b.MPaItemID : a.MPaItemID),
				MTrackItem1 = (string.IsNullOrWhiteSpace(a.MTrackItem1) ? b.MTrackItem1 : a.MTrackItem1),
				MTrackItem2 = (string.IsNullOrWhiteSpace(a.MTrackItem2) ? b.MTrackItem2 : a.MTrackItem2),
				MTrackItem3 = (string.IsNullOrWhiteSpace(a.MTrackItem3) ? b.MTrackItem3 : a.MTrackItem3),
				MTrackItem4 = (string.IsNullOrWhiteSpace(a.MTrackItem4) ? b.MTrackItem4 : a.MTrackItem4),
				MTrackItem5 = (string.IsNullOrWhiteSpace(a.MTrackItem5) ? b.MTrackItem5 : a.MTrackItem5)
			};
		}

		private string ToString(object obj)
		{
			return obj?.ToString();
		}

		public static string GetMerItemDesc(MContext ctx, string pkID)
		{
			string result = string.Empty;
			if (!string.IsNullOrWhiteSpace(pkID))
			{
				string sql = "select MDesc from t_bd_item_l where morgid = @MOrgID and MLocaleID= @MLocaleID and MParentID = @MItemID and MIsDelete = 0";
				MySqlParameter[] parameters = ctx.GetParameters("@MItemID", pkID);
				DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, parameters);
				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					result = dataSet.Tables[0].Rows[0].MField<string>("MDesc");
				}
			}
			return result;
		}

		public static string GetExpItemDesc(MContext ctx, string pkID)
		{
			string result = string.Empty;
			if (!string.IsNullOrWhiteSpace(pkID))
			{
				string sql = "select MDesc from t_bd_expenseitem_l where morgid = @MOrgID and MLocaleID= @MLocaleID and MParentID = @MItemID and MIsDelete = 0";
				MySqlParameter[] parameters = ctx.GetParameters("@MItemID", pkID);
				DataSet dataSet = new DynamicDbHelperMySQL(ctx).Query(sql, parameters);
				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
				{
					result = dataSet.Tables[0].Rows[0].MField<string>("MDesc");
				}
			}
			return result;
		}

		public List<ValidateQueryModel> GetValidateQueryModel(MContext ctx, BDCheckValidteListModel checkDataList)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			if (checkDataList.AccountIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql = GetValidateCommonModelSql<BDAccountModel>(MActionResultCodeEnum.MAccountInvalid, checkDataList.AccountIdList, null, null);
				ValidateQueryModel validateAccountHasSubSql = GetValidateAccountHasSubSql(checkDataList.AccountIdList, false);
				list.Add(validateCommonModelSql);
				list.Add(validateAccountHasSubSql);
			}
			if (checkDataList.ContactIdList.Count() > 0)
			{
				string otherFilter = " AND MIsActive=1";
				ValidateQueryModel validateCommonModelSql2 = GetValidateCommonModelSql<BDContactsModel>(MActionResultCodeEnum.MContactInvalid, checkDataList.ContactIdList, null, otherFilter);
				list.Add(validateCommonModelSql2);
			}
			if (checkDataList.EmployeeIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql3 = GetValidateCommonModelSql<BDEmployeesModel>(MActionResultCodeEnum.MEmployeeInvalid, checkDataList.EmployeeIdList, null, null);
				list.Add(validateCommonModelSql3);
			}
			if (checkDataList.MerchandiseIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql4 = GetValidateCommonModelSql<BDItemModel>(MActionResultCodeEnum.MMerItemInvalid, checkDataList.MerchandiseIdList, null, null);
				list.Add(validateCommonModelSql4);
			}
			if (checkDataList.ExpenseIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql5 = GetValidateCommonModelSql<BDExpenseItemModel>(MActionResultCodeEnum.MExpItemInvalid, checkDataList.ExpenseIdList, null, null);
				list.Add(validateCommonModelSql5);
			}
			if (checkDataList.PaIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql6 = GetValidateCommonModelSql<PAPayItemModel>(MActionResultCodeEnum.MPaItemInvalid, checkDataList.PaIdList, null, null);
				list.Add(validateCommonModelSql6);
			}
			if (checkDataList.PaGroupIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql7 = GetValidateCommonModelSql<PAPayItemGroupModel>(MActionResultCodeEnum.MPaItemInvalid, checkDataList.PaGroupIdList, null, null);
				list.Add(validateCommonModelSql7);
			}
			if (checkDataList.TrackEntryIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql8 = GetValidateCommonModelSql<BDTrackEntryModel>(MActionResultCodeEnum.MTrackItemInvalid, checkDataList.TrackEntryIdList, null, null);
				list.Add(validateCommonModelSql8);
			}
			if (checkDataList.BankIdList.Count() > 0)
			{
				ValidateQueryModel validateCommonModelSql9 = GetValidateCommonModelSql<BDBankAccountModel>(MActionResultCodeEnum.MBankAccountInvalid, checkDataList.BankIdList, null, null);
				list.Add(validateCommonModelSql9);
			}
			return list;
		}

		public void ValidateDocHasCreateVoucher(MContext ctx, string billID = null)
		{
			if (string.IsNullOrWhiteSpace(billID))
			{
				return;
			}
			GLVoucherModel createdVoucherByDocID = new GLDocVoucherRepository().GetCreatedVoucherByDocID(ctx, billID);
			if (createdVoucherByDocID == null || string.IsNullOrWhiteSpace(createdVoucherByDocID.MNumber))
			{
				return;
			}
			throw new MActionException
			{
				Codes = new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MBillHasAlreadyCreatedVoucher
				}
			};
		}

		public void ValidateFapiaoReconcile(MContext ctx, FPTableModel table, List<FPFapiaoModel> fapiaos)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			ValidateQueryModel validateCommonModelSql = GetValidateCommonModelSql<FPTableModel>(MActionResultCodeEnum.MTableNotExists, new List<string>
			{
				table.MItemID
			}, null, null);
			list.Add(validateCommonModelSql);
			ValidateQueryModel validateCommonModelSql2 = GetValidateCommonModelSql<FPFapiaoModel>(MActionResultCodeEnum.MFapiaoDeletedOrObsolete, (from x in fapiaos
			select x.MID).ToList(), null, " AND MStatus != 0 ");
			list.Add(validateCommonModelSql2);
			ValidateQueryModel validateCommonModelSql3 = GetValidateCommonModelSql<FPFapiaoModel>(MActionResultCodeEnum.MExistsFapiaoInNoReconciledStatus, (from x in fapiaos
			select x.MID).ToList(), null, " AND MReconcileStatus != 2 ");
			list.Add(validateCommonModelSql3);
			if (table.MTotalAmount < fapiaos.Sum((FPFapiaoModel x) => x.MTotalAmount) + table.MAjustAmount)
			{
				list.Add(new ValidateQueryModel(MActionResultCodeEnum.MFapiaoAmountCannotLargerThanTableAmount));
			}
			QueryValidateSql(ctx, true, list.ToArray());
		}

		public void ValidateFapiaoTableCanDelete(MContext ctx, List<string> tableIds)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			ValidateQueryModel validateCommonModelSql = GetValidateCommonModelSql<FPTableModel>(MActionResultCodeEnum.MExistsReconciledFapiao, tableIds, null, " and MRTotalAmount = 0 ");
			list.Add(validateCommonModelSql);
			QueryValidateSql(ctx, true, list.ToArray());
		}

		public void ValidateFapiaoCoding(MContext ctx, List<FPCodingModel> codings, int category)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			if (category == 0)
			{
				List<DateTime> dates = (from x in codings
				select x.MBizDate).ToList();
				list.Add(GetValidateHasClosedPeriodSql(ctx, dates));
				QueryValidateSql(ctx, true, list.ToArray());
			}
			int num = 0;
			while (true)
			{
				if (num < codings.Count)
				{
					if (string.IsNullOrWhiteSpace(codings[num].MContactID))
					{
						throw new MActionException
						{
							Codes = new List<MActionResultCodeEnum>
							{
								MActionResultCodeEnum.MFapiaoCreateVoucherWithNoContact
							}
						};
					}
					if (string.IsNullOrWhiteSpace(codings[num].MDebitAccount))
					{
						throw new MActionException
						{
							Codes = new List<MActionResultCodeEnum>
							{
								MActionResultCodeEnum.MFapiaoCreateVoucherWithNoDebitAccount
							}
						};
					}
					if (string.IsNullOrWhiteSpace(codings[num].MCreditAccount))
					{
						throw new MActionException
						{
							Codes = new List<MActionResultCodeEnum>
							{
								MActionResultCodeEnum.MFapiaoCreateVoucherWithNoCreditAccount
							}
						};
					}
					if (codings[num].MTaxAmount != decimal.Zero && string.IsNullOrWhiteSpace(codings[num].MTaxAccount) && (codings[num].MInvoiceType != 1 || codings[num].MType != 0))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			throw new MActionException
			{
				Codes = new List<MActionResultCodeEnum>
				{
					MActionResultCodeEnum.MFapiaoCreateVoucherWithNoTaxAccount
				}
			};
		}

		public void ValidateSetFapiaoNoReconcieStatus(MContext ctx, List<string> fapiaoIDs)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			ValidateQueryModel validateCommonModelSql = GetValidateCommonModelSql<FPFapiaoModel>(MActionResultCodeEnum.MFapiaoDeletedOrObsolete, fapiaoIDs, null, " and MStatus != 0 ");
			ValidateQueryModel validateCommonModelSql2 = GetValidateCommonModelSql<FPFapiaoModel>(MActionResultCodeEnum.MExistsFapiaoInReconciledStatus, fapiaoIDs.ToList(), null, " and MReconcileStatus != 1 ");
			list.Add(validateCommonModelSql);
			list.Add(validateCommonModelSql2);
			QueryValidateSql(ctx, true, list.ToArray());
		}

		public void ValidateSetFapiaoNoCreateVoucherStatus(MContext ctx, List<string> fapiaoIDs)
		{
			List<ValidateQueryModel> list = new List<ValidateQueryModel>();
			ValidateQueryModel validateCommonModelSql = GetValidateCommonModelSql<FPFapiaoModel>(MActionResultCodeEnum.MFapiaoDeletedOrObsolete, fapiaoIDs, null, " and MStatus != 0 ");
			ValidateQueryModel validateCommonModelSql2 = GetValidateCommonModelSql<FPFapiaoModel>(MActionResultCodeEnum.MExistsFapiaoInNoCodingStatus, fapiaoIDs.ToList(), null, " and MCodingStatus != 1 ");
			list.Add(validateCommonModelSql);
			list.Add(validateCommonModelSql2);
			QueryValidateSql(ctx, true, list.ToArray());
		}

		public GLCheckGroupValueModel MergeCheckGroupValueModelByIDs(MContext ctx, List<string> idList)
		{
			List<GLCheckGroupValueModel> checkGroupValueModelByID = GetCheckGroupValueModelByID(ctx, idList);
			GLCheckGroupValueModel gLCheckGroupValueModel = new GLCheckGroupValueModel
			{
				MContactIDTitle = GetCheckTypeName(ctx, 0),
				MEmployeeIDTitle = GetCheckTypeName(ctx, 1),
				MMerItemIDTitle = GetCheckTypeName(ctx, 2),
				MExpItemIDTitle = GetCheckTypeName(ctx, 3),
				MPaItemGroupName = GetCheckTypeName(ctx, 4),
				MTrackItem1GroupName = GetCheckTypeName(ctx, 5),
				MTrackItem2GroupName = GetCheckTypeName(ctx, 6),
				MTrackItem3GroupName = GetCheckTypeName(ctx, 7),
				MTrackItem4GroupName = GetCheckTypeName(ctx, 8),
				MTrackItem5GroupName = GetCheckTypeName(ctx, 9)
			};
			for (int i = 0; i < checkGroupValueModelByID.Count; i++)
			{
				gLCheckGroupValueModel = MergeCheckGroupValueModel(ctx, gLCheckGroupValueModel, checkGroupValueModelByID[i]);
			}
			return gLCheckGroupValueModel;
		}

		public string GetCheckGroupValueID(MContext ctx, BDAccountModel account, GLCheckGroupValueModel value, string checkGroupValueID)
		{
			if (value == null && string.IsNullOrWhiteSpace(checkGroupValueID))
			{
				return null;
			}
			GLCheckGroupValueModel gLCheckGroupValueModel = (value != null) ? CopyCheckGroupValueModel(value) : GetCheckGroupValueModelByID(ctx, checkGroupValueID);
			if (CheckGroupValueMathWithCheckGroupModel(ctx, gLCheckGroupValueModel, account.MCheckGroupModel, account, true, false) || string.IsNullOrWhiteSpace(gLCheckGroupValueModel.MItemID))
			{
				return GetCheckGroupValueModel(ctx, gLCheckGroupValueModel).MItemID;
			}
			return gLCheckGroupValueModel.MItemID;
		}

		public string GetCheckGroupValueID(MContext ctx, string accountCode, GLCheckGroupValueModel value)
		{
			if (!string.IsNullOrWhiteSpace(accountCode))
			{
				List<BDAccountModel> accountList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).AccountList;
				BDAccountModel bDAccountModel = accountList.FirstOrDefault((BDAccountModel x) => x.MCode == accountCode);
				if (bDAccountModel != null)
				{
					GLCheckGroupValueModel gLCheckGroupValueModel = value.Clone();
					CheckGroupValueMathWithCheckGroupModel(ctx, gLCheckGroupValueModel, bDAccountModel.MCheckGroupModel, bDAccountModel, true, true);
					return GetCheckGroupValueModel(ctx, gLCheckGroupValueModel).MItemID;
				}
			}
			return null;
		}

		private GLCheckGroupValueModel MergeCheckGroupValueModel(MContext ctx, GLCheckGroupValueModel a, GLCheckGroupValueModel b)
		{
			return new GLCheckGroupValueModel
			{
				MContactIDTitle = a.MContactIDTitle,
				MEmployeeIDTitle = a.MEmployeeIDTitle,
				MMerItemIDTitle = a.MMerItemIDTitle,
				MExpItemIDTitle = a.MExpItemIDTitle,
				MPaItemGroupName = a.MPaItemGroupName,
				MTrackItem1GroupName = a.MTrackItem1GroupName,
				MTrackItem2GroupName = a.MTrackItem2GroupName,
				MTrackItem3GroupName = a.MTrackItem3GroupName,
				MTrackItem4GroupName = a.MTrackItem4GroupName,
				MTrackItem5GroupName = a.MTrackItem5GroupName,
				MContactID = MergeString(a.MContactID, b.MContactID),
				MEmployeeID = MergeString(a.MEmployeeID, b.MEmployeeID),
				MMerItemID = MergeString(a.MMerItemID, b.MMerItemID),
				MExpItemID = MergeString(a.MExpItemID, b.MExpItemID),
				MPaItemID = MergeString(a.MPaItemID, b.MPaItemID),
				MTrackItem1 = MergeString(a.MTrackItem1, b.MTrackItem1),
				MTrackItem2 = MergeString(a.MTrackItem2, b.MTrackItem2),
				MTrackItem3 = MergeString(a.MTrackItem3, b.MTrackItem3),
				MTrackItem4 = MergeString(a.MTrackItem4, b.MTrackItem4),
				MTrackItem5 = MergeString(a.MTrackItem5, b.MTrackItem5),
				MContactName = MergeString(a.MContactName, b.MContactName),
				MEmployeeName = MergeString(a.MEmployeeName, b.MEmployeeName),
				MMerItemName = MergeString(a.MMerItemName, b.MMerItemName),
				MExpItemName = MergeString(a.MExpItemName, b.MExpItemName),
				MPaItemName = MergeString(a.MPaItemName, b.MPaItemName),
				MTrackItem1Name = MergeString(a.MTrackItem1Name, b.MTrackItem1Name),
				MTrackItem2Name = MergeString(a.MTrackItem2Name, b.MTrackItem2Name),
				MTrackItem3Name = MergeString(a.MTrackItem3Name, b.MTrackItem3Name),
				MTrackItem4Name = MergeString(a.MTrackItem4Name, b.MTrackItem4Name),
				MTrackItem5Name = MergeString(a.MTrackItem5Name, b.MTrackItem5Name)
			};
		}

		private string MergeString(string a, string b)
		{
			return string.IsNullOrWhiteSpace(a) ? b : a;
		}

		public int GetModuleIDByDocType(int type)
		{
			switch (type)
			{
			case 0:
				return 0;
			case 1:
				return 1;
			case 2:
				return 2;
			case 3:
				return 6;
			case 4:
				return 7;
			case 5:
				return 8;
			case 6:
				return 4;
			case 7:
				return 5;
			default:
				throw new Exception("单据类型异常");
			}
		}

		public string GetModuleNameByModuleID(MContext ctx, int type)
		{
			switch (type)
			{
			case 0:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Sales", "销售");
			case 1:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "Purchases", "采购");
			case 2:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ExpenseClaim", "费用报销");
			case 6:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Receive", "收款");
			case 7:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Payment", "付款");
			case 8:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Transfer", "转账");
			case 4:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "OutputInoivce", "销项发票");
			case 5:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "InputInvoice", "进项发票");
			default:
				throw new Exception("模块类型异常");
			}
		}

		public string GetDocTypeNameByTypeID(MContext ctx, int type)
		{
			switch (type)
			{
			case 0:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "SalesDoc", "销售单");
			case 1:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "PurchaseDoc", "采购单");
			case 2:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ExpenseDoc", "费用报销单");
			case 3:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "ReceiveDoc", "收款单");
			case 4:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "PaymentDoc", "付款单");
			case 5:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "TransferDoc", "转账单");
			case 7:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "OutputInoivce", "销项发票");
			case 8:
				return COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "InputInvoice", "进项发票");
			default:
				throw new Exception("单据类型异常");
			}
		}

		public List<FCFapiaoModuleModel> GetFapiaoModuleList(MContext ctx)
		{
			string sql = "SELECT\r\n                                t1.MID,\n                                t1.MFastCode,\n                                t1.MDescription,\n                                t1.MEXPLANATION,\n                                t1.MDebitAccount,\n                                t1.MCreditAccount,\n                                t1.MTaxAccount,\n                                t1.MMerItemID,\n                                t1.MTrackItem1,\n                                t1.MTrackItem2,\n                                t1.MTrackItem3,\n                                t1.MTrackItem4,\n                                t1.MTrackItem5\n                            FROM\n                                t_fc_fapiaomodule t1\n                            WHERE\n                                t1.MOrgID = @MOrgID\n                                    AND t1.MIsDelete = 0\n                                    AND t1.MLCID = @MLocaleID\r\n                                order by t1.MFastCode desc ";
			return ModelInfoManager.GetDataModelBySql<FCFapiaoModuleModel>(ctx, sql, ctx.GetParameters((MySqlParameter)null));
		}

		public string GetVoucherEntryExplanation(MContext ctx, int docType, int dir, string DocNumber, string Contact, string Employee, string Reference, string MerItemNumber, string ExpenseItem, string Desc)
		{
			string docTypeNameByTypeID = GetDocTypeNameByTypeID(ctx, docType);
			int moduleID = GetModuleIDByDocType(docType);
			string empty = string.Empty;
			List<string> list = new List<string>();
			BDVoucherSettingCategoryModel bDVoucherSettingCategoryModel = GLDataPool.GetInstance(ctx, false, 0, 0, 0).VoucherSettingCategoryList.FirstOrDefault((BDVoucherSettingCategoryModel x) => x.MModuleID == moduleID);
			if (bDVoucherSettingCategoryModel == null)
			{
				return "";
			}
			List<BDVoucherSettingModel> mSettingList = bDVoucherSettingCategoryModel.MSettingList;
			bool flag = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.DocType && x.MStatus && x.MDC == dir);
			bool flag2 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.DocNumber && x.MStatus && x.MDC == dir);
			bool flag3 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.Contact && x.MStatus && x.MDC == dir);
			bool flag4 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.Employee && x.MStatus && x.MDC == dir);
			bool flag5 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.Reference && x.MStatus && x.MDC == dir);
			bool flag6 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.MerItemNumber && x.MStatus && x.MDC == dir);
			bool flag7 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.ExpenseItem && x.MStatus && x.MDC == dir);
			bool flag8 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.Desc && x.MStatus && x.MDC == dir);
			bool flag9 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.ExpenseItemMerItemNumber && x.MStatus && x.MDC == dir);
			bool flag10 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.ContactEmployee && x.MStatus && x.MDC == dir);
			list = new List<string>
			{
				flag ? docTypeNameByTypeID : string.Empty,
				flag2 ? DocNumber : string.Empty,
				flag3 ? Contact : string.Empty,
				flag4 ? Employee : string.Empty,
				flag10 ? (string.IsNullOrWhiteSpace(Contact) ? Employee : Contact) : string.Empty,
				flag5 ? Reference : string.Empty,
				flag6 ? MerItemNumber : string.Empty,
				flag7 ? ExpenseItem : string.Empty,
				flag9 ? (string.IsNullOrWhiteSpace(MerItemNumber) ? ExpenseItem : MerItemNumber) : string.Empty,
				flag8 ? Desc : string.Empty
			};
			list = (from x in list
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			return (list.Count == 0) ? string.Empty : string.Join("·", list);
		}

		public string GetCodingVoucherEntryExplanation(MContext ctx, int category, int dir, string Contact, string FapiaoNumber, string Explanation)
		{
			string docTypeNameByTypeID = GetDocTypeNameByTypeID(ctx, category + 7);
			int moduleID = GetModuleIDByDocType(category + 6);
			string empty = string.Empty;
			List<string> list = new List<string>();
			List<BDVoucherSettingModel> mSettingList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).VoucherSettingCategoryList.FirstOrDefault((BDVoucherSettingCategoryModel x) => x.MModuleID == moduleID).MSettingList;
			bool flag = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.FapiaoType && x.MStatus && x.MDC == dir);
			bool flag2 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.Contact && x.MStatus && x.MDC == dir);
			bool flag3 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.FapiaoNumber && x.MStatus && x.MDC == dir);
			bool flag4 = mSettingList.Exists((BDVoucherSettingModel x) => x.MTypeID == BDVoucherSettingEumn.Explanation && x.MStatus && x.MDC == dir);
			list = new List<string>
			{
				flag ? docTypeNameByTypeID : string.Empty,
				flag2 ? Contact : string.Empty,
				flag3 ? FapiaoNumber : string.Empty,
				flag4 ? Explanation : string.Empty
			};
			list = (from x in list
			where !string.IsNullOrWhiteSpace(x)
			select x).ToList();
			return (list.Count == 0) ? string.Empty : string.Join("·", list);
		}

		public static string GetInFilterQuery<T>(List<T> paramValues, ref List<MySqlParameter> paramList, string prefix = "M_ID")
		{
			string text = " in (";
			for (int i = 0; i < paramValues.Count; i++)
			{
				string name = "@" + prefix + i + ",";
				text += name;
				if (paramList != null && !paramList.Exists((Predicate<MySqlParameter>)((MySqlParameter x) => x.ParameterName == name.TrimEnd(','))))
				{
					paramList.Add(new MySqlParameter(name.TrimEnd(','), paramValues[i]));
				}
			}
			return text.TrimEnd(',') + " ) ";
		}

		public static void ReorderVoucherEntry(List<GLVoucherModel> vouchers)
		{
			for (int i = 0; i < vouchers.Count; i++)
			{
				if (vouchers[i] != null && vouchers[i].MVoucherEntrys != null && vouchers[i].MVoucherEntrys.Count > 0)
				{
					vouchers[i].MVoucherEntrys = (from x in vouchers[i].MVoucherEntrys
					orderby x.MDC descending
					select x).ToList();
					for (int j = 0; j < vouchers[i].MVoucherEntrys.Count; j++)
					{
						vouchers[i].MVoucherEntrys[j].MEntrySeq = j;
					}
				}
			}
		}
	}
}
