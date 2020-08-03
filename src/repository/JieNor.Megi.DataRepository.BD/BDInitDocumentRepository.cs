using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.BD.InitDocument;
using JieNor.Megi.DataModel.GL;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace JieNor.Megi.DataRepository.BD
{
	public class BDInitDocumentRepository : DataServiceT<BDInitDocumentModel>
	{
		private readonly REGCurrencyRepository currencyBiz = new REGCurrencyRepository();

		private readonly BDAccountRepository accountDal = new BDAccountRepository();

		private readonly GLInitBalanceRepository initBalanceDal = new GLInitBalanceRepository();

		private readonly List<string> CurrentAccountList = new List<string>
		{
			"1122",
			"2202",
			"1123",
			"2203",
			"1221",
			"2241"
		};

		private GLUtility utility = new GLUtility();

		public List<GLInitBalanceModel> GetInitBalanceFromBill(MContext ctx)
		{
			BDAccountRepository bDAccountRepository = new BDAccountRepository();
			GLInitBalanceRepository gLInitBalanceRepository = new GLInitBalanceRepository();
			DateTime mGLBeginDate = ctx.MGLBeginDate;
			List<BDInitBillViewModel> list = (from x in GetInitBillViewModelList(ctx, new BDInitDocumentFilterModel
			{
				MTypeList = new List<int>
				{
					0,
					1,
					2,
					3,
					4
				}
			})
			where !string.IsNullOrWhiteSpace(x.MCurrentAccountCode)
			select x).ToList();
			List<GLCheckGroupValueModel> checkGroupValueModelFromBill = utility.GetCheckGroupValueModelFromBill(ctx, list);
			return (from x in checkGroupValueModelFromBill
			select new GLInitBalanceModel
			{
				MAccountID = x.MAccountID,
				MCheckGroupValueID = x.MItemID,
				MInitBalance = x.MBillAmountInfo.MAmount,
				MInitBalanceFor = x.MBillAmountInfo.MAmountFor,
				MCurrencyID = x.MBillAmountInfo.MCurrencyID
			}).ToList();
		}

		public List<GLInitBalanceModel> FilterZeorRecord(List<GLInitBalanceModel> list)
		{
			list = (list ?? new List<GLInitBalanceModel>());
			return (from x in list
			where x.MInitBalance != decimal.Zero || x.MInitBalanceFor != decimal.Zero
			select x).ToList();
		}

		public OperationResult CheckInitBalanceEqualWithBill(MContext ctx)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			if (ctx.MOrgVersionID == 1)
			{
				return operationResult;
			}
			List<string> list = new List<string>();
			List<GLInitBalanceModel> list2 = FilterZeorRecord(GetInitBalanceFromBill(ctx));
			List<BDAccountModel> currentAccountBaseData = accountDal.GetCurrentAccountBaseData(ctx, false);
			List<GLInitBalanceModel> list3 = (from x in currentAccountBaseData
			where x.MCreateInitBill && (x.MInitBalanceModel.MInitBalance != decimal.Zero || x.MInitBalanceModel.MInitBalanceFor != decimal.Zero)
			select new GLInitBalanceModel
			{
				MAccountID = x.MItemID,
				MCurrencyID = x.MInitBalanceModel.MCurrencyID,
				MCheckGroupValueID = x.MInitBalanceModel.MCheckGroupValueID,
				MInitBalance = x.MInitBalanceModel.MInitBalance * (decimal)x.MDC,
				MInitBalanceFor = x.MInitBalanceModel.MInitBalanceFor * (decimal)x.MDC
			}).ToList();
			for (int i = 0; i < list3.Count; i++)
			{
				GLInitBalanceModel balance = list3[i];
				GLInitBalanceModel gLInitBalanceModel = list2.FirstOrDefault((GLInitBalanceModel x) => x.MAccountID == balance.MAccountID && x.MCheckGroupValueID == balance.MCheckGroupValueID && x.MCurrencyID == balance.MCurrencyID && x.MInitBalance == balance.MInitBalance && x.MInitBalanceFor == balance.MInitBalanceFor && x.MMatched == 0);
				if (gLInitBalanceModel == null)
				{
					BDAccountModel bDAccountModel = currentAccountBaseData.FirstOrDefault((BDAccountModel y) => y.MItemID == balance.MAccountID);
					string empty = string.Empty;
					if (bDAccountModel.MCheckGroupID != "0")
					{
						list.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "Account_Currency_CheckGroup_InitBalanceNotMatchBills", "科目[{0}] 币别[{1}] 相应核算维度的初始化金额与业务单据统计金额不匹配"), bDAccountModel.MFullName, balance.MCurrencyID));
					}
					else
					{
						list.Add(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "Account_Currency_InitBalanceNotMatchBills", "科目[{0}] 币别[{1}] 初始化金额与业务单据统计金额不匹配"), bDAccountModel.MFullName, balance.MCurrencyID));
					}
				}
				else
				{
					gLInitBalanceModel.MMatched = 1;
					balance.MMatched = 1;
				}
			}
			if (list.Count > 0)
			{
				operationResult.Success = false;
				operationResult.Message = string.Join(";", list.Distinct().ToList());
			}
			else if (list3.Exists((GLInitBalanceModel x) => x.MMatched == 0 && (x.MInitBalance != decimal.Zero || x.MInitBalanceFor != decimal.Zero)) || list2.Exists((GLInitBalanceModel x) => x.MMatched == 0 && (x.MInitBalance != decimal.Zero || x.MInitBalanceFor != decimal.Zero)))
			{
				operationResult.Success = false;
				operationResult.Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.GL, "AccountInitBalanceNotBalanceWithBills", "科目初始化金额与业务单据统计金额不相等"));
			}
			return operationResult;
		}

		private string GetInitInvoiceSaleSql(MContext ctx, string mType)
		{
			return "SELECT \n                    t1.MID,\n                    t1.MCurrentAccountCode,\n                    t1.MType,\n                    '' AS MBankID,\n                    t1.MNumber,\n                    t1.MReference,\n                    t1.MBizDate,\n                    t1.MVerifyAmt,\r\n                    t1.MVerifyAmtFor,'Invoice' as MBizBillType, \n                    (CASE\n                        WHEN t1.MBizDate < @MBeginDate THEN '1'\n                        ELSE '0'\n                    END) AS MIsInitBill,\n                    t1.MDueDate,\n                    t1.MCyID,\n                    t1.MExchangeRate,\n                    t1.MTaxTotalAmt,\n                    t1.MTaxTotalAmtFor,\n                    t2.MEntryID,\n                    (CASE\n                        WHEN t1.MTaxTotalAmt = t2.MTaxAmount THEN '1'\n                        ELSE '0'\n                    END) AS MSingleRow,\n                    t1.MContactID,\n                    '' AS MEmployeeID,\n                    t2.MItemID AS MMerItemID,\n                    '' AS MExpItemID,\n                    '' AS MPaItemID,\n                    t2.MTrackItem1,\n                    t2.MTrackItem2,\n                    t2.MTrackItem3,\n                    t2.MTrackItem4,\n                    t2.MTrackItem5,\n                    t2.MTaxAmount,\n                    t2.MTaxAmountFor,\n                    t2.MSeq\n                FROM\n                    t_iv_invoice t1\n                        INNER JOIN\n                    t_iv_invoiceentry t2 ON t1.MID = t2.MID\n                        AND t1.MOrgId = t2.MOrgID\n                        AND t2.MIsDelete = t1.MIsDelete\n                WHERE\n                    t1.morgid = @MOrgID\n                        AND t1.MIsDelete = 0\n                        AND  LOCATE('" + mType + "', MType) = 1\n                        AND t1.MBizDate < @MGLBeginDate\r\n                        {0} ";
		}

		private string GetInitExpenseSql(MContext ctx)
		{
			return "SELECT \n                    t1.MID,\n                    t1.MCurrentAccountCode,\n                    t1.MType,\n                    '' AS MBankID,\n                    '' as MNumber,\n                    t1.MReference,\n                    t1.MBizDate,\n                    t1.MVerifyAmt,\r\n                    t1.MVerifyAmtFor,'Expense' as MBizBillType, \n                    (CASE\n                        WHEN t1.MBizDate < @MBeginDate THEN '1'\n                        ELSE '0'\n                    END) AS MIsInitBill,\n                    t1.MDueDate,\n                    t1.MCyID,\n                    t1.MExchangeRate,\n                    t1.MTaxTotalAmt,\n                    t1.MTaxTotalAmtFor,\n                    t2.MEntryID,\n                    (CASE\n                        WHEN t1.MTaxTotalAmt = t2.MTaxAmount THEN '1'\n                        ELSE '0'\n                    END) AS MSingleRow,\n                    '' as MContactID,\n                    t1.MContactID AS MEmployeeID,\n                    '' AS MMerItemID,\n                    t2.MItemID AS MExpItemID,\n                    '' AS MPaItemID,\n                    t2.MTrackItem1,\n                    t2.MTrackItem2,\n                    t2.MTrackItem3,\n                    t2.MTrackItem4,\n                    t2.MTrackItem5,\n                    t2.MTaxAmount,\n                    t2.MTaxAmountFor,\n                    t2.MSeq\n                FROM\n                    t_iv_expense t1\n                        INNER JOIN\n                    t_iv_expenseentry t2 ON t1.MID = t2.MID\n                        AND t1.MOrgId = t2.MOrgID\n                        AND t2.MIsDelete = t1.MIsDelete\n                WHERE\n                    t1.morgid = @MOrgID\n                        AND t1.MIsDelete = 0\n                        AND t1.MBizDate < @MGLBeginDate\r\n                        {0} ";
		}

		private string GetInitReceiveSql(MContext ctx)
		{
			return "SELECT \n                    t1.MID,\n                    t1.MCurrentAccountCode,\n                    t1.MType,\n                    t1.MBankID AS MBankID,\n                    '' as MNumber,\n                    t1.MReference,\n                    t1.MBizDate,\n                    t1.MVerifyAmt,\r\n                    t1.MVerifyAmtFor,'Receive' as MBizBillType, \n                    (CASE\n                        WHEN t1.MBizDate < @MBeginDate THEN '1'\n                        ELSE '0'\n                    END) AS MIsInitBill,\n                    '' as MDueDate,\n                    t1.MCyID,\n                    t1.MExchangeRate,\n                    t1.MTaxTotalAmt,\n                    t1.MTaxTotalAmtFor,\n                    t2.MEntryID,\n                    (CASE\n                        WHEN t1.MTaxTotalAmt = t2.MTaxAmount THEN '1'\n                        ELSE '0'\n                    END) AS MSingleRow,\n                    t1.MContactID as MContactID,\n                    '' AS MEmployeeID,\n                    t2.MItemID AS MMerItemID,\n                    '' AS MExpItemID,\n                    '' AS MPaItemID,\n                    t2.MTrackItem1,\n                    t2.MTrackItem2,\n                    t2.MTrackItem3,\n                    t2.MTrackItem4,\n                    t2.MTrackItem5,\n                    t2.MTaxAmount,\n                    t2.MTaxAmountFor,\n                    t2.MSeq\n                FROM\n                    t_iv_receive t1\n                        INNER JOIN\n                    t_iv_receiveentry t2 ON t1.MID = t2.MID\n                        AND t1.MOrgId = t2.MOrgID\n                        AND t2.MIsDelete = t1.MIsDelete\n                WHERE\n                    t1.morgid = @MOrgID\n                        AND t1.MIsDelete = 0\n                        AND t1.MBizDate < @MGLBeginDate\r\n                        {0} ";
		}

		private string GetInitPaymentSql(MContext ctx)
		{
			return "SELECT \n                    t1.MID,\n                    t1.MCurrentAccountCode,\n                    t1.MType,\n                    t1.MBankID AS MBankID,\n                    '' as MNumber,\n                    t1.MReference,\n                    t1.MBizDate,\n                    t1.MVerifyAmt,\r\n                    t1.MVerifyAmtFor,'Payment' as MBizBillType, \n                    (CASE\n                        WHEN t1.MBizDate < @MBeginDate THEN '1'\n                        ELSE '0'\n                    END) AS MIsInitBill,\n                    '' as MDueDate,\n                    t1.MCyID,\n                    t1.MExchangeRate,\n                    t1.MTaxTotalAmt,\n                    t1.MTaxTotalAmtFor,\n                    t2.MEntryID,\n                    (CASE\n                        WHEN t1.MTaxTotalAmt = t2.MTaxAmount THEN '1'\n                        ELSE '0'\n                    END) AS MSingleRow,\n                    (CASE\n                        WHEN t1.MContactType != 'Employees' THEN t1.MContactID\n                        ELSE ''\n                    END) AS MContactID,\n                    (CASE\n                        WHEN t1.MContactType = 'Employees' THEN t1.MContactID\n                        ELSE ''\n                    END) AS MEmployeeID,\n                   (CASE\n                        WHEN t1.MContactType != 'Employees' THEN t2.MItemID\n                        ELSE ''\n                    END) AS MMerItemID,\n                    (CASE\n                        WHEN t1.MContactType = 'Employees' THEN t2.MItemID\n                        ELSE ''\n                    END) AS MExpItemID,\n                    '' AS MPaItemID,\n                    t2.MTrackItem1,\n                    t2.MTrackItem2,\n                    t2.MTrackItem3,\n                    t2.MTrackItem4,\n                    t2.MTrackItem5,\n                    t2.MTaxAmount,\n                    t2.MTaxAmountFor,\n                    t2.MSeq\n                FROM\n                    t_iv_payment t1\n                        INNER JOIN\n                    t_iv_paymententry t2 ON t1.MID = t2.MID\n                        AND t1.MOrgId = t2.MOrgID\n                        AND t2.MIsDelete = t1.MIsDelete\n                WHERE\n                    t1.morgid = @MOrgID\n                        AND t1.MIsDelete = 0\n                        AND t1.MBizDate < @MGLBeginDate\r\n                        {0}";
		}

		public List<BDInitBillViewModel> GetInitBillViewModelList(MContext ctx, BDInitDocumentFilterModel filter)
		{
			List<string> list = new List<string>();
			if (filter.MTypeList.Contains(0))
			{
				list.Add(GetInitInvoiceSaleSql(ctx, "Invoice_Sale"));
			}
			if (filter.MTypeList.Contains(1))
			{
				list.Add(GetInitInvoiceSaleSql(ctx, "Invoice_Purchase"));
			}
			if (filter.MTypeList.Contains(2))
			{
				list.Add(GetInitReceiveSql(ctx));
			}
			if (filter.MTypeList.Contains(3))
			{
				list.Add(GetInitPaymentSql(ctx));
			}
			if (filter.MTypeList.Contains(4))
			{
				list.Add(GetInitExpenseSql(ctx));
			}
			string str = "";
			string text = "";
			List<MySqlParameter> list2 = new List<MySqlParameter>
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MGLBeginDate", ctx.MGLBeginDate),
				new MySqlParameter("@MBeginDate", ctx.MBeginDate)
			};
			if (!string.IsNullOrWhiteSpace(filter.MCurrentAccountCode))
			{
				str = " AND MCurrentAccountCode =@MCurrentAccountCode";
				list2.Add(new MySqlParameter("@MCurrentAccountCode", filter.MCurrentAccountCode));
			}
			if (filter.MCheckGroupValueModel != null)
			{
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MContactID))
				{
					text += " AND MContactID = @MContactID";
					list2.Add(new MySqlParameter("@MContactID", filter.MCheckGroupValueModel.MContactID));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MEmployeeID))
				{
					text += " AND MContactID = @MEmployeeID";
					list2.Add(new MySqlParameter("@MEmployeeID", filter.MCheckGroupValueModel.MEmployeeID));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MExpItemID))
				{
					text += " AND MItemID = @MExpItemID";
					list2.Add(new MySqlParameter("@MExpItemID", filter.MCheckGroupValueModel.MExpItemID));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MMerItemID))
				{
					text += " AND MItemID = @MMerItemID";
					list2.Add(new MySqlParameter("@MMerItemID", filter.MCheckGroupValueModel.MMerItemID));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MTrackItem1))
				{
					text += " AND MTrackItem1 = @MTrackItem1";
					list2.Add(new MySqlParameter("@MTrackItem1", filter.MCheckGroupValueModel.MTrackItem1));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MTrackItem2))
				{
					text += " AND MTrackItem2 = @MTrackItem2";
					list2.Add(new MySqlParameter("@MTrackItem2", filter.MCheckGroupValueModel.MTrackItem2));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MTrackItem3))
				{
					text += " AND MTrackItem3 = @MTrackItem3";
					list2.Add(new MySqlParameter("@MTrackItem3", filter.MCheckGroupValueModel.MTrackItem3));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MTrackItem4))
				{
					text += " AND MTrackItem4 = @MTrackItem4";
					list2.Add(new MySqlParameter("@MTrackItem4", filter.MCheckGroupValueModel.MTrackItem4));
				}
				if (!string.IsNullOrWhiteSpace(filter.MCheckGroupValueModel.MTrackItem5))
				{
					text += " AND MTrackItem5 = @MTrackItem5";
					list2.Add(new MySqlParameter("@MTrackItem5", filter.MCheckGroupValueModel.MTrackItem5));
				}
			}
			string sql = string.Format(string.Join(" Union ALL ", list) + " order by MID", str + " " + text);
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sql, list2.ToArray());
			List<BDInitBillViewModel> list3 = BindDatatable2List(ds);
			return FilterVerification(ctx, list3);
		}

		private List<BDInitBillViewModel> FilterVerification(MContext ctx, List<BDInitBillViewModel> list)
		{
			List<BDInitBillViewModel> list2 = new List<BDInitBillViewModel>();
			for (int i = 0; i < list.Count; i++)
			{
				BDInitBillViewModel model = list[i];
				if (Math.Abs(model.MVerifyAmt) > decimal.Zero)
				{
					IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
					iVVerificationListFilterModel.MBillID = model.MID;
					iVVerificationListFilterModel.MBizBillType = model.MBizBillType;
					iVVerificationListFilterModel.MViewVerif = true;
					List<IVVerificationListModel> historyVerifData = IVVerificationRepository.GetHistoryVerifData(ctx, iVVerificationListFilterModel);
					decimal num = (from x in historyVerifData
					where x.MBizDate < ctx.MGLBeginDate
					select x).Sum((IVVerificationListModel x) => (model.MType == "Invoice_Sale_Red" || model.MType == "Invoice_Purchase_Red") ? (decimal.MinusOne * x.MHaveVerificationAmtFor) : Math.Abs(x.MHaveVerificationAmtFor));
					model.MVerifyAmtFor = num;
					if (!(Math.Abs(model.MTaxTotalAmtFor) <= Math.Abs(num)))
					{
						goto IL_0107;
					}
					continue;
				}
				goto IL_0107;
				IL_0107:
				list2.Add(model);
			}
			return list2;
		}

		private List<BDInitBillViewModel> BindDatatable2List(DataSet ds)
		{
			List<BDInitBillViewModel> list = new List<BDInitBillViewModel>();
			if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
			{
				BDInitBillViewModel bDInitBillViewModel = new BDInitBillViewModel();
				DataTable dataTable = ds.Tables[0];
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dataTable.Rows[i];
					string a = dataRow["MID"].ToString();
					if (a != bDInitBillViewModel.MID)
					{
						if (!string.IsNullOrWhiteSpace(bDInitBillViewModel.MID))
						{
							list.Add(bDInitBillViewModel);
						}
						bDInitBillViewModel = new BDInitBillViewModel
						{
							MID = dataRow.MField<string>("MID"),
							MCurrentAccountCode = dataRow.MField<string>("MCurrentAccountCode"),
							MType = dataRow.MField<string>("MType"),
							MBankID = dataRow.MField<string>("MBankID"),
							MNumber = dataRow.MField<string>("MNumber"),
							MReference = dataRow.MField<string>("MReference"),
							MBizDate = dataRow.MField<DateTime>("MBizDate"),
							MVerifyAmt = dataRow.MField<decimal>("MVerifyAmtFor"),
							MVerifyAmtFor = dataRow.MField<decimal>("MVerifyAmtFor"),
							MBizBillType = dataRow.MField<string>("MBizBillType"),
							MIsInitBill = (dataRow.MField<string>("MIsInitBill") == "1"),
							MDueDate = dataRow.MField<DateTime>("MDueDate"),
							MCyID = dataRow.MField<string>("MCyID"),
							MTaxTotalAmt = dataRow.MField<decimal>("MTaxTotalAmt"),
							MTaxTotalAmtFor = dataRow.MField<decimal>("MTaxTotalAmtFor"),
							MSingleRow = (dataRow.MField<string>("MSingleRow") == "1"),
							MInitBillEntryList = new List<BDInitBillEntryViewModel>(),
							MCheckGroupValueModel = new GLCheckGroupValueModel
							{
								MContactID = dataRow.MField<string>("MContactID"),
								MEmployeeID = dataRow.MField<string>("MEmployeeID"),
								MMerItemID = dataRow.MField<string>("MMerItemID"),
								MExpItemID = dataRow.MField<string>("MExpItemID"),
								MPaItemID = dataRow.MField<string>("MPaItemID"),
								MTrackItem1 = dataRow.MField<string>("MTrackItem1"),
								MTrackItem2 = dataRow.MField<string>("MTrackItem2"),
								MTrackItem3 = dataRow.MField<string>("MTrackItem3"),
								MTrackItem4 = dataRow.MField<string>("MTrackItem4"),
								MTrackItem5 = dataRow.MField<string>("MTrackItem5")
							}
						};
					}
					BDInitBillEntryViewModel item = new BDInitBillEntryViewModel
					{
						MEntryID = dataRow.MField<string>("MEntryID"),
						MCurrentAccountCode = dataRow.MField<string>("MCurrentAccountCode"),
						MType = dataRow.MField<string>("MType"),
						MCyID = dataRow.MField<string>("MCyID"),
						MTaxAmount = dataRow.MField<decimal>("MTaxAmount"),
						MTaxAmountFor = dataRow.MField<decimal>("MTaxAmountFor"),
						MContactID = dataRow.MField<string>("MContactID"),
						MEmployeeID = dataRow.MField<string>("MEmployeeID"),
						MMerItemID = dataRow.MField<string>("MMerItemID"),
						MExpItemID = dataRow.MField<string>("MExpItemID"),
						MPaItemID = dataRow.MField<string>("MPaItemID"),
						MTrackItem1 = dataRow.MField<string>("MTrackItem1"),
						MTrackItem2 = dataRow.MField<string>("MTrackItem2"),
						MTrackItem3 = dataRow.MField<string>("MTrackItem3"),
						MTrackItem4 = dataRow.MField<string>("MTrackItem4"),
						MTrackItem5 = dataRow.MField<string>("MTrackItem5")
					};
					bDInitBillViewModel.MInitBillEntryList.Add(item);
				}
				list.Add(bDInitBillViewModel);
			}
			return list;
		}

		public OperationResult UpdateDocCurrentAccountCode(MContext ctx, string docType, string docId, string accountCode)
		{
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			string empty = string.Empty;
			string arg = string.Empty;
			empty = "update {0} set MCurrentAccountCode = @MCurrentAccountCode where MOrgID = @MOrgID and MID = @MItemID and MIsDelete = 0 ";
			if (docType.ToLower().IndexOf("invoice") >= 0)
			{
				arg = "t_iv_invoice";
			}
			else if (docType.ToLower().IndexOf("pay") >= 0)
			{
				arg = "t_iv_payment";
			}
			else if (docType.ToLower().IndexOf("receive") >= 0)
			{
				arg = "t_iv_receive";
			}
			else if (docType.ToLower().IndexOf("expense") >= 0)
			{
				arg = "t_iv_expense";
			}
			empty = string.Format(empty, arg);
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter
				{
					ParameterName = "@MOrgID",
					Value = ctx.MOrgID
				},
				new MySqlParameter
				{
					ParameterName = "@MItemID",
					Value = docId
				},
				new MySqlParameter
				{
					ParameterName = "@MCurrentAccountCode",
					Value = accountCode
				}
			};
			OperationResult operationResult2 = operationResult;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> list = new List<CommandInfo>();
			CommandInfo obj = new CommandInfo
			{
				CommandText = empty
			};
			DbParameter[] array = obj.Parameters = parameters;
			list.Add(obj);
			operationResult2.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list) > 0);
			return operationResult;
		}

		public DataSet GetBillIdByInitBalance(MContext ctx, GLInitBalanceModel initBalance)
		{
			string sql = "";
			if (initBalance.MBillType == "Invoice_Sale" || initBalance.MBillType == "Invoice_Purchase")
			{
				sql = "SELECT a.MID from t_iv_invoice a\r\n                                  INNER JOIN t_iv_invoiceentry b on a.MID=b.MID and a.MOrgID=b.MOrgID and a.MIsDelete=0 and a.MStatus=3\r\n                                WHERE a.MOrgID=@MOrgID and a.MContactID=@MContactID and  a.MBizDate < @MBizDate and ifnull(b.MItemID , '')=@MMerItemID and b.MTaxAmount=@MAmount and b.MTaxAmountFor=@MAmountFor and \r\n                                ifnull(b.MTrackItem1,'')=@MTrackItem1 and ifnull(b.MTrackItem2,'')=@MTrackItem2 and ifnull(b.MTrackItem3,'')=@MTrackItem3 and ifnull(b.MTrackItem4,'')=@MTrackItem4 and  ifnull(b.MTrackItem5,'')=@MTrackItem5 and\r\n                                a.MType=@BillType";
			}
			else if (initBalance.MBillType == "Receive_Sale")
			{
				sql = "SELECT a.MID from t_iv_receive a\r\n                                  INNER JOIN t_iv_receiveentry b on a.MID=b.MID and a.MOrgID=b.MOrgID and a.MIsDelete=0 and b.MIsDelete=0\r\n                                where a.MOrgID=@MOrgID and a.MContactID=@MContactID and a.MBizDate <'@MBizDate' and ifnull(b.MItemID , '')=@MMerItemID and a.MBankID=@MBankID and a.MTaxTotalAmt=@MAmount and a.MTaxTotalAmtFor=@MAmountFor and \r\n                                ifnull(b.MTrackItem1,'')=@MTrackItem1 and ifnull(b.MTrackItem2,'')=@MTrackItem2 and ifnull(b.MTrackItem3,'')=@MTrackItem3 and ifnull(b.MTrackItem4,'')=@MTrackItem4 and  ifnull(b.MTrackItem5,'')=@MTrackItem5 \r\n                               and a.MType=@BillType and MContactType=@MContactType";
			}
			else if (initBalance.MBillType == "Pay_Purchase")
			{
				sql = "SELECT a.MID from t_iv_payment a\r\n                                  INNER JOIN t_iv_paymententry b on a.MID=b.MID and a.MOrgID=b.MOrgID and a.MIsDelete=0 and b.MIsDelete=0\r\n                                where a.MOrgID=@MOrgID and a.MContactID=@MContactID and a.MBizDate <'@MBizDate' and (ifnull(b.MItemID , '')=@MMerItemID or ifnull(b.MItemID , '')=@MExpenseID) and a.MBankID=@MBankID and a.MTaxTotalAmt=@MAmount and a.MTaxTotalAmtFor=@MAmountFor and \r\n                                ifnull(b.MTrackItem1,'')=@MTrackItem1 and ifnull(b.MTrackItem2,'')=@MTrackItem2 and ifnull(b.MTrackItem3,'')=@MTrackItem3 and ifnull(b.MTrackItem4,'')=@MTrackItem4 and  ifnull(b.MTrackItem5,'')=@MTrackItem5\r\n                                and a.MType=@BillType and MContactType=@MContactType";
			}
			else if (initBalance.MBillType == "Expense_Claims")
			{
				sql = " SELECT a.MID from t_iv_expense a\r\n                                  INNER JOIN t_iv_expenseentry b on a.MID=b.MID and a.MOrgID=b.MOrgID and a.MIsDelete=0 and b.MIsDelete=0 and a.MStatus=3\r\n                                where a.MOrgID=@MOrgID and a.MEmployee=@MEmpoyeeID and a.MBizDate <'@MBizDate'  and ifnull(b.MItemID , '')=@MExpenseID and a.MTaxTotalAmt=@MAmount and a.MTaxTotalAmtFor=@MAmountFor and \r\n                                ifnull(b.MTrackItem1,'')=@MTrackItem1 and ifnull(b.MTrackItem2,'')=@MTrackItem2 and ifnull(b.MTrackItem3,'')=@MTrackItem3 and ifnull(b.MTrackItem4,'')=@MTrackItem4 and  ifnull(b.MTrackItem5,'')=@MTrackItem5  \r\n                              and a.MType=@BillType";
			}
			MySqlParameter[] cmdParms = new MySqlParameter[16]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MContactID", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MContactID) ? "" : initBalance.MCheckGroupValueModel.MContactID),
				new MySqlParameter("@MEmpoyeeID", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MEmployeeID) ? "" : initBalance.MCheckGroupValueModel.MEmployeeID),
				new MySqlParameter("@MMerItemID", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MMerItemID) ? "" : initBalance.MCheckGroupValueModel.MMerItemID),
				new MySqlParameter("@MExpenseID", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MExpItemID) ? "" : initBalance.MCheckGroupValueModel.MExpItemID),
				new MySqlParameter("@MAmount", initBalance.MInitBalance),
				new MySqlParameter("@MAmountFor", initBalance.MInitBalanceFor),
				new MySqlParameter("@MTrackItem1", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MTrackItem1) ? "" : initBalance.MCheckGroupValueModel.MTrackItem1),
				new MySqlParameter("@MTrackItem2", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MTrackItem2) ? "" : initBalance.MCheckGroupValueModel.MTrackItem2),
				new MySqlParameter("@MTrackItem3", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MTrackItem3) ? "" : initBalance.MCheckGroupValueModel.MTrackItem3),
				new MySqlParameter("@MTrackItem4", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MTrackItem4) ? "" : initBalance.MCheckGroupValueModel.MTrackItem4),
				new MySqlParameter("@MTrackItem5", string.IsNullOrWhiteSpace(initBalance.MCheckGroupValueModel.MTrackItem5) ? "" : initBalance.MCheckGroupValueModel.MTrackItem5),
				new MySqlParameter("@BillType", initBalance.MBillType),
				new MySqlParameter("@MContactType", initBalance.MContactTypeFromBill),
				new MySqlParameter("@MBankID", initBalance.MBankID),
				new MySqlParameter("@MBizDate", ctx.MGLBeginDate.ToString("yyyy-MM-dd"))
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			return dynamicDbHelperMySQL.Query(sql, cmdParms);
		}

		public OperationResult CheckIsExistInitBill(MContext ctx)
		{
			string strSql = "SELECT\r\n\t                            *\r\n                            FROM\r\n\t                            (\r\n\t\t                            SELECT CASE WHEN (EXISTS (\r\n\t\t\t\t                            SELECT\r\n\t\t\t\t\t                            *\r\n\t\t\t\t                            FROM\r\n\t\t\t\t\t                            t_iv_invoice\r\n\t\t\t\t                            WHERE\r\n\t\t\t\t\t                            MOrgID = @MOrgID\r\n\t\t\t\t                            AND MBizDate < @MBeginDate\r\n\t\t\t                            )\r\n\t\t\t                            OR EXISTS (\r\n\t\t\t\t                            SELECT\r\n\t\t\t\t\t                            *\r\n\t\t\t\t                            FROM\r\n\t\t\t\t\t                            t_iv_receive\r\n\t\t\t\t                            WHERE\r\n\t\t\t\t\t                            MOrgID = @MOrgID\r\n\t\t\t\t                            AND MBizDate < @MBeginDate\r\n\t\t\t                            )\r\n\t\t\t                            OR EXISTS (\r\n\t\t\t\t                            SELECT\r\n\t\t\t\t\t                            *\r\n\t\t\t\t                            FROM\r\n\t\t\t\t\t                            t_iv_payment\r\n\t\t\t\t                            WHERE\r\n\t\t\t\t\t                            MOrgID = @MOrgID\r\n\t\t\t\t                            AND MBizDate < @MBeginDate\r\n\t\t\t                            )\r\n\t\t                            ) THEN\r\n\t\t\t                            1\r\n\t\t                            ELSE\r\n\t\t\t                            0\r\n\t\t                            END AS Result\r\n\t                            ) t\r\n                            WHERE\r\n\t                            t.Result = 1";
			MySqlParameter[] cmdParms = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MBeginDate", ctx.MGLBeginDate.ToString("yyyy-MM-dd"))
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			OperationResult operationResult = new OperationResult();
			operationResult.Success = dynamicDbHelperMySQL.Exists(strSql, cmdParms);
			return operationResult;
		}
	}
}
