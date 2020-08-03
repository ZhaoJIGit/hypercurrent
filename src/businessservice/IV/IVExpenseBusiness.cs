using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.DataRepository.RPT.Biz;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVExpenseBusiness : BusinessServiceBase, IIVExpenseBusiness, IBasicBusiness<IVExpenseModel>
	{
		private BDTrackRepository _track = new BDTrackRepository();

		private REGCurrencyRepository _bdCurrency = new REGCurrencyRepository();

		private REGTaxRateRepository _taxRate = new REGTaxRateRepository();

		private BDAccountBusiness _acctBiz = new BDAccountBusiness();

		private BDExpenseItemBusiness _expenseItemBiz = new BDExpenseItemBusiness();

		private BDBankAccountBusiness _bankAcctBiz = new BDBankAccountBusiness();

		private BDExpenseItemRepository _expItem = new BDExpenseItemRepository();

		private BDEmployeesBusiness _empBiz = new BDEmployeesBusiness();

		public IVExpenseModel GetExpenseEditModel(MContext ctx, string pkID)
		{
			IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, pkID);
			ResetExpenseModel(ctx, expenseModel);
			return expenseModel;
		}

		private void ResetExpenseModel(MContext ctx, IVExpenseModel model)
		{
			if (model != null && !string.IsNullOrEmpty(model.MID))
			{
				IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
				iVVerificationListFilterModel.MBillID = model.MID;
				iVVerificationListFilterModel.MBizBillType = "Expense";
				iVVerificationListFilterModel.MBizType = model.MType;
				iVVerificationListFilterModel.MContactID = model.MContactID;
				iVVerificationListFilterModel.MCurrencyID = model.MCyID;
				iVVerificationListFilterModel.MViewVerif = true;
				model.Verification = IVExpenseRepository.GetHistoryVerifData(ctx, iVVerificationListFilterModel);
			}
			model.MActionPermission = GetActionPermissionModel(ctx, model);
			if (model.MActionPermission.MIsCanApprove)
			{
				IVExpenseModel nextUnApproveExpenseID = IVExpenseRepository.GetNextUnApproveExpenseID(ctx, model.MCreateDate);
				if (nextUnApproveExpenseID != null)
				{
					model.MNextID = nextUnApproveExpenseID.MID;
				}
			}
			if (model.MStatus == Convert.ToInt32(IVExpenseStatusEnum.Paid) || model.MVerifyAmtFor > decimal.Zero)
			{
				model.MBankList = IVVerificationRepository.GetPaymentBankList(ctx, model.MID);
			}
			model.MIsInitBill = (model.MBizDate < ctx.MBeginDate);
		}

		private IVExpensePermissionModel GetActionPermissionModel(MContext ctx, IVExpenseModel model)
		{
			IVExpensePermissionModel iVExpensePermissionModel = new IVExpensePermissionModel();
			IVVerificationBusiness iVVerificationBusiness = new IVVerificationBusiness();
			List<IVVerificationInforModel> customerWaitForVerificationInfor = iVVerificationBusiness.GetCustomerWaitForVerificationInfor(ctx, model.MID, "Expense");
			if (!base.HavePermission(ctx, "Expense", "Change"))
			{
				iVExpensePermissionModel.MHaveAction = false;
				if (!string.IsNullOrEmpty(model.MID) && model.MStatus == Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment) && Math.Abs(model.MTaxTotalAmtFor) > Math.Abs(model.MVerificationAmt) && base.HavePermission(ctx, "BankAccount", "Change"))
				{
					iVExpensePermissionModel.MIsCanPay = true;
					if (customerWaitForVerificationInfor != null && customerWaitForVerificationInfor.Count > 0)
					{
						iVExpensePermissionModel.MIsCanVerification = true;
					}
				}
				return iVExpensePermissionModel;
			}
			if (string.IsNullOrEmpty(model.MID))
			{
				iVExpensePermissionModel.MHaveAction = false;
				if (base.HavePermission(ctx, "Expense", "Approve"))
				{
					iVExpensePermissionModel.MIsCanApprove = true;
				}
				return iVExpensePermissionModel;
			}
			iVExpensePermissionModel.MHaveAction = true;
			if (model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MTaxTotalAmtFor) == Math.Abs(model.MVerificationAmt) || !base.HavePermission(ctx, "BankAccount", "Change"))
			{
				iVExpensePermissionModel.MIsCanPay = false;
			}
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (model.MStatus >= Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment) || Math.Abs(model.MVerificationAmt) > decimal.Zero || model.MBizDate < ctx.MBeginDate || !base.HavePermission(ctx, "Expense", "Approve") || !operationResult.Success)
			{
				iVExpensePermissionModel.MIsCanApprove = false;
			}
			if (model.MStatus != Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment) || Math.Abs(Math.Abs(model.MVerificationAmt)) > decimal.Zero || model.MBizDate < ctx.MBeginDate || !base.HavePermission(ctx, "Expense", "Approve") || !operationResult.Success)
			{
				iVExpensePermissionModel.MIsCanUnApprove = false;
			}
			if (!base.HavePermission(ctx, "General_Ledger", "View") || !GLInterfaceRepository.IsBillCreatedVoucher(ctx, model.MID))
			{
				iVExpensePermissionModel.MIsCanViewVoucherCreateDetail = false;
			}
			if (Math.Abs(model.MVerificationAmt) > decimal.Zero || model.MStatus >= 3)
			{
				iVExpensePermissionModel.MIsCanVoid = false;
			}
			if (model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MVerificationAmt) > decimal.Zero)
			{
				iVExpensePermissionModel.MIsCanDelete = false;
			}
			if (Math.Abs(model.MTaxTotalAmtFor) == Math.Abs(model.MVerificationAmt))
			{
				iVExpensePermissionModel.MIsCanVerification = false;
			}
			else if (customerWaitForVerificationInfor == null || customerWaitForVerificationInfor.Count == 0 || model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || !base.HavePermission(ctx, "BankAccount", "Change"))
			{
				iVExpensePermissionModel.MIsCanVerification = false;
			}
			return iVExpensePermissionModel;
		}

		public OperationResult AddExpenseNoteLog(MContext ctx, IVExpenseModel model)
		{
			return IVExpenseRepository.AddExpenseNoteLog(ctx, model);
		}

		public IVExpenseModel GetExpenseCopyModel(MContext ctx, string pkID)
		{
			IVExpenseModel expenseCopyModel = IVExpenseRepository.GetExpenseCopyModel(ctx, pkID);
			ResetExpenseModel(ctx, expenseCopyModel);
			return expenseCopyModel;
		}

		public DataGridJson<IVExpenseListModel> GetExpenseList(MContext ctx, IVExpenseListFilterModel filter)
		{
			DataGridJson<IVExpenseListModel> expenseList = IVExpenseRepository.GetExpenseList(ctx, filter, null);
			if (expenseList != null)
			{
				foreach (IVExpenseListModel row in expenseList.rows)
				{
					row.MContactName = GlobalFormat.GetUserName(row.MFirstName, row.MLastName, null);
				}
			}
			return expenseList;
		}

		public List<IVExpenseModel> GetExpenseListIncludeEntry(MContext ctx, IVExpenseListFilterModel filter)
		{
			return GetExpenseListIncludeEntry(ctx, filter, null);
		}

		public List<IVExpenseModel> GetExpenseListIncludeEntry(MContext ctx, IVExpenseListFilterModel filter, GetParam param)
		{
			DataGridJson<IVExpenseListModel> expenseList = IVExpenseRepository.GetExpenseList(ctx, filter, param);
			if (expenseList == null || expenseList.rows == null || expenseList.rows.Count() == 0)
			{
				return new List<IVExpenseModel>();
			}
			List<IVExpenseListModel> rows = expenseList.rows;
			string[] values = (from x in rows
			select x.MID).ToArray();
			filter.In("MID", values);
			if (!string.IsNullOrEmpty(filter.Sort))
			{
				filter.OrderBy(filter.Sort + " desc");
			}
			return IVExpenseRepository.GetExpenseListIncludeEntry(ctx, filter);
		}

		public List<IVExpenseModel> GetExpenseListForPrint(MContext ctx, IVExpenseListFilterModel filter)
		{
			List<IVExpenseModel> list = new List<IVExpenseModel>();
			if (!string.IsNullOrWhiteSpace(filter.SelectedIds))
			{
				SqlWhere filter2 = new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MID", filter.SelectedIds.Split(',').ToList());
				list = ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, filter2, false, true);
				List<BDExpenseItemModel> dataModelList = ModelInfoManager.GetDataModelList<BDExpenseItemModel>(ctx, new SqlWhere(), false, true);
				foreach (IVExpenseModel item in list)
				{
					foreach (IVExpenseEntryModel item2 in item.ExpenseEntry)
					{
						BDExpenseItemModel bDExpenseItemModel = dataModelList.FirstOrDefault((BDExpenseItemModel f) => f.MItemID == item2.MItemID);
						if (bDExpenseItemModel != null)
						{
							List<MultiLanguageField> mMultiLanguageField = bDExpenseItemModel.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MDesc").MMultiLanguageField;
							if (mMultiLanguageField.Any((MultiLanguageField f) => f.MValue == item2.MDesc))
							{
								string mValue = mMultiLanguageField.FirstOrDefault((MultiLanguageField f) => f.MLocaleID == ctx.MLCID).MValue;
								if (!string.IsNullOrWhiteSpace(mValue))
								{
									item2.MDesc = mValue;
								}
							}
						}
					}
				}
			}
			return list;
		}

		public List<IVExpenseModel> GetExpenseListForExport(MContext ctx, IVExpenseListFilterModel filter)
		{
			List<IVExpenseModel> list = new List<IVExpenseModel>();
			List<IVExpenseListModel> rows = IVExpenseRepository.GetExpenseList(ctx, filter, null).rows;
			if (!rows.Any())
			{
				return new List<IVExpenseModel>();
			}
			SqlWhere filter2 = new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MID", (from f in rows
			select f.MID).ToList());
			List<IVExpenseModel> dataModelList = ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, filter2, false, true);
			foreach (IVExpenseModel item in dataModelList)
			{
				item.IsCalculateMainAmount = false;
				IVExpenseListModel iVExpenseListModel = rows.SingleOrDefault((IVExpenseListModel f) => f.MID == item.MID);
				item.MDepartment = iVExpenseListModel.DepartmentName;
				item.MVerifyAmt = iVExpenseListModel.MVerifyAmt;
				list.Add(item);
			}
			return list;
		}

		public OperationResult UpdateExpense(MContext ctx, IVExpenseModel model)
		{
			OperationResult operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, model.MBizDate);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (string.IsNullOrEmpty(model.MCyID))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CnyCannotEmpty", "Currency can't be empty!");
				return operationResult;
			}
			if (model.ExpenseEntry != null && model.ExpenseEntry.Exists((IVExpenseEntryModel a) => a.MDesc.Length > 500))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DescTooLong", "描述长度不能大于500字符");
				return operationResult;
			}
			if (!string.IsNullOrEmpty(model.MID))
			{
				IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, model.MID);
				if ((expenseModel == null || expenseModel.MStatus >= Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment)) && expenseModel.MBizDate >= ctx.MBeginDate)
				{
					return new OperationResult
					{
						Success = false,
						Code = "1000",
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！")
					};
				}
			}
			return IVExpenseRepository.UpdateExpense(ctx, model);
		}

		public OperationResult UnApproveExpense(MContext ctx, string expenseId)
		{
			IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, expenseId);
			if (expenseModel == null || string.IsNullOrEmpty(expenseModel.MID) || Math.Abs(expenseModel.MVerifyAmtFor) > decimal.Zero)
			{
				return new OperationResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DataHadBeenUpdate", "Data had been updated.")
				};
			}
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, expenseId);
			if (expenseModel.MStatus >= Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment) && !operationResult.Success)
			{
				return operationResult;
			}
			return IVExpenseRepository.UnApproveExpense(ctx, expenseId);
		}

		public OperationResult ApproveExpense(MContext ctx, ParamBase param)
		{
			return IVExpenseRepository.ApproveExpense(ctx, param);
		}

		public OperationResult UpdateExpenseStatus(MContext ctx, ParamBase param)
		{
			return IVExpenseRepository.UpdateExpenseStatus(ctx, param);
		}

		public OperationResult DeleteExpenseList(MContext ctx, ParamBase param)
		{
			return IVExpenseRepository.DeleteExpenseList(ctx, param);
		}

		public OperationResult AddExpensePayment(MContext ctx, List<IVMakePaymentModel> makePaymentList)
		{
			if (makePaymentList.Count != 1)
			{
				for (int i = 0; i < makePaymentList.Count; i++)
				{
					makePaymentList[i].MIsPayAll = true;
				}
			}
			IVExpenseMakePaymentRepository iVExpenseMakePaymentRepository = new IVExpenseMakePaymentRepository();
			return iVExpenseMakePaymentRepository.ToPay(ctx, makePaymentList);
		}

		public List<IVExpenseModel> GetModelList(MContext ctx, ParamBase param)
		{
			return IVExpenseRepository.GetModelList(ctx, param);
		}

		public IVExpenseSummaryModel GetExpenseSummaryModel(MContext ctx, DateTime startDate, DateTime endDate)
		{
			return IVExpenseRepository.GetExpenseSummaryModel(ctx, startDate, endDate);
		}

		public string GetChartStackedDictionary(MContext ctx, string statisticType, DateTime startDate, DateTime endDate)
		{
			string baseCurrencyID = GetBaseCurrencyID(ctx);
			RPTExpenseClaimRepository rPTExpenseClaimRepository = new RPTExpenseClaimRepository(baseCurrencyID);
			return rPTExpenseClaimRepository.GetChartStackedDictionary(ctx, startDate, endDate, statisticType, 6);
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string empty = string.Empty;
			bool flag = ctx.MOrgVersionID == 1;
			bool flag2 = flag;
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MEmployee", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Contact, "Employee", "Employee", null)
				}), true, true, 2, null),
				new IOTemplateConfigModel("MReference", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Reference", "Reference", null)
				}), false, true, 2, null),
				new IOTemplateConfigModel("MBizDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Date", "Date", empty)
				}), IOTemplateFieldType.Date, true, true, 2, null)
			});
			if (!flag)
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MDueDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "DueDate", "Due Date", empty)
					}), IOTemplateFieldType.Date, true, true, 2, null),
					new IOTemplateConfigModel("MExpectedDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.IV, "ExpectedPaymentDate", "Expected Payment date", empty)
					}), IOTemplateFieldType.Date, false, true, 2, null)
				});
			}
			bool isRequired = !flag;
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MCyID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Currency", "Currency", null)
				}), isRequired, true, 2, null)
			});
			if (!flag)
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MPaymentDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.IV, "PaymentDate", "Payment Date", empty)
					}), false, true, 2, null),
					new IOTemplateConfigModel("MPaidAmount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.IV, "PaidAmount", "Paid Amount", null)
					}), false, true, 2, null),
					new IOTemplateConfigModel("MBankAccount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.IV, "BankAccount", "Bank Account", null)
					}), false, true, 2, null)
				});
			}
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MItemID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "ExpenseItem", "Expense Item", null)
				}), true, false, 2, null),
				new IOTemplateConfigModel("MDesc", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Description", "Description", null)
				}), true, false, 2, null),
				new IOTemplateConfigModel("MQty", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "Quantity", "Quantity", null)
				}), IOTemplateFieldType.Decimal, true, false, 4, null),
				new IOTemplateConfigModel("MPrice", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "UnitPrice", "Unit Price", null)
				}), IOTemplateFieldType.Decimal, true, false, 4, null),
				new IOTemplateConfigModel("MTaxAmtFor", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "TaxAmountFor", "税额", null)
				}), IOTemplateFieldType.Decimal, false, false, 2, null)
			});
			List<BDTrackModel> trackNameList = _track.GetTrackNameList(ctx, false);
			IEnumerable<IGrouping<string, BDTrackModel>> enumerable = from f in trackNameList
			group f by f.MItemID;
			Dictionary<string, string> allText = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", ":")
			});
			int num = 0;
			foreach (IGrouping<string, BDTrackModel> item in enumerable)
			{
				List<BDTrackModel> list2 = item.ToList();
				string fieldName = "MTrackItem" + (num + 1);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (BDTrackModel item2 in list2)
				{
					dictionary.Add(item2.MLocaleID, allText[item2.MLocaleID] + item2.MName);
				}
				list.Add(new IOTemplateConfigModel(fieldName, dictionary, false, false, 2, null));
				num++;
			}
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				list.AddRange(new List<IOTemplateConfigModel>
				{
					new IOTemplateConfigModel("MDebitAccount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "DebitAccount", "Debit Account", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MCreditAccount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "CreditAccount", "Credit Account", null)
					}), false, false, 2, null),
					new IOTemplateConfigModel("MTaxAccount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
					{
						new COMLangInfoModel(LangModule.Common, "TaxAccount", "Tax Account", null)
					}), false, false, 2, null)
				});
			}
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, Dictionary<string, string[]> exampleDataList = null)
		{
			List<BDEmployeesModel> employeeList = _empBiz.GetEmployeeList(ctx, false);
			List<BASDataDictionaryModel> dictList = BASDataDictionary.GetDictList("TaxType", ctx.MLCID);
			BASCurrencyViewModel @base = _bdCurrency.GetBase(ctx, false, null, null);
			List<REGCurrencyViewModel> viewList = _bdCurrency.GetViewList(ctx, null, null, false, null);
			List<BDExpenseItemModel> listByTier = _expenseItemBiz.GetListByTier(ctx, false, false);
			List<REGTaxRateModel> list = _taxRate.GetList(ctx, null, false);
			List<NameValueModel> trackBasicInfo = _track.GetTrackBasicInfo(ctx, null, false, false);
			List<BDBankAccountEditModel> bankAccountList = _bankAcctBiz.GetBankAccountList(ctx);
			List<ImportTemplateDataSource> list2 = new List<ImportTemplateDataSource>();
			List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
			foreach (BDEmployeesModel item2 in employeeList)
			{
				if (!string.IsNullOrWhiteSpace(item2.MFullName))
				{
					list3.Add(new ImportDataSourceInfo
					{
						Key = item2.MItemID,
						Value = item2.MFullName
					});
				}
			}
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Employee,
				FieldList = new List<string>
				{
					"MEmployee"
				},
				DataSourceList = list3
			});
			List<ImportDataSourceInfo> dsTaxTypeList = new List<ImportDataSourceInfo>();
			dictList.ForEach(delegate(BASDataDictionaryModel f)
			{
				dsTaxTypeList.Add(new ImportDataSourceInfo
				{
					Key = f.DictValue,
					Value = f.DictName
				});
			});
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.TaxType,
				FieldList = new List<string>
				{
					"MTaxTypeID"
				},
				DataSourceList = dsTaxTypeList
			});
			List<ImportDataSourceInfo> list4 = new List<ImportDataSourceInfo>();
			if (@base != null)
			{
				list4.Add(new ImportDataSourceInfo
				{
					Key = @base.MCurrencyID,
					Value = $"{@base.MCurrencyID} {@base.MLocalName}"
				});
			}
			foreach (REGCurrencyViewModel item3 in viewList)
			{
				if (!list4.Any((ImportDataSourceInfo f) => f.Key == item3.MCurrencyID))
				{
					list4.Add(new ImportDataSourceInfo
					{
						Key = item3.MCurrencyID,
						Value = $"{item3.MCurrencyID} {item3.MName}"
					});
				}
			}
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Currency,
				FieldList = new List<string>
				{
					"MCyID"
				},
				DataSourceList = list4
			});
			List<ImportDataSourceInfo> list5 = new List<ImportDataSourceInfo>();
			foreach (BDExpenseItemModel item4 in listByTier)
			{
				list5.Add(new ImportDataSourceInfo
				{
					Key = item4.MItemID,
					Value = item4.MName
				});
			}
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.ExpenseItem,
				FieldList = new List<string>
				{
					"MItemID"
				},
				DataSourceList = list5
			});
			List<ImportDataSourceInfo> dsTaxRateList = new List<ImportDataSourceInfo>();
			list.ForEach(delegate(REGTaxRateModel f)
			{
				dsTaxRateList.Add(new ImportDataSourceInfo
				{
					Key = f.MItemID,
					Value = $"{f.MName}({f.MTaxRate.ToOrgDigitalFormat(ctx)}%)"
				});
			});
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.TaxRate,
				FieldList = new List<string>
				{
					"MTaxID"
				},
				DataSourceList = dsTaxRateList
			});
			for (int i = 0; i < trackBasicInfo.Count; i++)
			{
				if (trackBasicInfo[i].MChildren != null)
				{
					List<ImportDataSourceInfo> list6 = new List<ImportDataSourceInfo>();
					foreach (NameValueModel mChild in trackBasicInfo[i].MChildren)
					{
						if (!string.IsNullOrWhiteSpace(mChild.MValue))
						{
							list6.Add(new ImportDataSourceInfo
							{
								Key = mChild.MValue,
								Value = mChild.MName
							});
						}
					}
					ImportTemplateColumnType fieldType = (ImportTemplateColumnType)Enum.Parse(typeof(ImportTemplateColumnType), "TrackItem" + (i + 1));
					list2.Add(new ImportTemplateDataSource(true)
					{
						FieldType = fieldType,
						FieldList = new List<string>
						{
							"MTrackItem" + (i + 1)
						},
						DataSourceList = list6
					});
				}
			}
			bool flag = ctx.MEnabledModules.Contains(ModuleEnum.GL);
			if (flag)
			{
				List<BDAccountModel> bDAccountList = _acctBiz.GetBDAccountList(ctx, new BDAccountListFilterModel(), false, false);
				List<ImportDataSourceInfo> list7 = new List<ImportDataSourceInfo>();
				foreach (BDAccountModel item5 in bDAccountList)
				{
					if (!string.IsNullOrWhiteSpace(item5.MCode))
					{
						list7.Add(new ImportDataSourceInfo
						{
							Key = item5.MCode,
							Value = item5.MFullName
						});
					}
				}
				list2.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Account,
					FieldList = new List<string>
					{
						"MDebitAccount",
						"MCreditAccount",
						"MTaxAccount"
					},
					DataSourceList = list7
				});
			}
			List<ImportDataSourceInfo> list8 = new List<ImportDataSourceInfo>();
			foreach (BDBankAccountEditModel item6 in bankAccountList)
			{
				list8.Add(new ImportDataSourceInfo
				{
					Key = item6.MItemID,
					Value = item6.MBankName
				});
			}
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.BankAccount,
				FieldList = new List<string>
				{
					"MBankAccount"
				},
				DataSourceList = list8
			});
			if (exampleDataList != null)
			{
				exampleDataList.Add("MEmployee", new string[1]
				{
					employeeList.Any() ? employeeList[0].MFullName : string.Empty
				});
				string[] array = new string[2];
				string[] array2 = new string[2];
				if (listByTier.Any())
				{
					for (int j = 0; j < 2; j++)
					{
						if (j < listByTier.Count)
						{
							array[j] = listByTier[j].MName;
							array2[j] = listByTier[j].MDesc;
							if (string.IsNullOrWhiteSpace(array2[j]))
							{
								array2[j] = array[j];
							}
						}
					}
				}
				exampleDataList.Add("MItemID", array);
				exampleDataList.Add("MDesc", array2);
				exampleDataList.Add("MBankAccount", new string[1]
				{
					bankAccountList.Any() ? bankAccountList[bankAccountList.Count() - 1].MBankName : string.Empty
				});
				List<ImportDataSourceInfo> dataSourceList = list2.FirstOrDefault((ImportTemplateDataSource f) => f.FieldType == ImportTemplateColumnType.Currency).DataSourceList;
				exampleDataList.Add("MCyID", new string[1]
				{
					dataSourceList.FirstOrDefault((ImportDataSourceInfo f) => f.Key == ctx.MBasCurrencyID).Value
				});
				if (flag)
				{
					List<ImportDataSourceInfo> dataSourceList2 = list2.FirstOrDefault((ImportTemplateDataSource f) => f.FieldType == ImportTemplateColumnType.Account).DataSourceList;
					ImportDataSourceInfo importDataSourceInfo = dataSourceList2.FirstOrDefault((ImportDataSourceInfo f) => f.Key.StartsWith("660103") || f.Key.StartsWith("560103"));
					ImportDataSourceInfo importDataSourceInfo2 = dataSourceList2.FirstOrDefault((ImportDataSourceInfo f) => f.Key.StartsWith("2241"));
					ImportDataSourceInfo importDataSourceInfo3 = dataSourceList2.FirstOrDefault((ImportDataSourceInfo f) => f.Key.StartsWith("22210101"));
					string text = (importDataSourceInfo != null) ? importDataSourceInfo.Value : string.Empty;
					string text2 = (importDataSourceInfo2 != null) ? importDataSourceInfo2.Value : string.Empty;
					string text3 = (importDataSourceInfo3 != null) ? importDataSourceInfo3.Value : string.Empty;
					exampleDataList.Add("MDebitAccount", new string[2]
					{
						text,
						text
					});
					exampleDataList.Add("MCreditAccount", new string[2]
					{
						text2,
						text2
					});
					exampleDataList.Add("MTaxAccount", new string[2]
					{
						text3,
						text3
					});
				}
			}
			return list2;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary2);
			DateTime dateTime = ctx.DateNow;
			DateTime dateTime2 = dateTime.AddMonths(-1);
			dictionary2.Add("MBizDate", new string[1]
			{
				dateTime2.ToString("yyyy-MM-dd")
			});
			Dictionary<string, string[]> dictionary3 = dictionary2;
			string[] obj = new string[1];
			dateTime = dateTime2.AddDays(20.0);
			obj[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary3.Add("MDueDate", obj);
			Dictionary<string, string[]> dictionary4 = dictionary2;
			string[] obj2 = new string[1];
			dateTime = dateTime2.AddDays(10.0);
			obj2[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary4.Add("MExpectedDate", obj2);
			dictionary2.Add("MQty", new string[2]
			{
				"1.0000",
				"1.0000"
			});
			dictionary2.Add("MPrice", new string[2]
			{
				"500.0000",
				"350.0000"
			});
			dictionary2.Add("MTaxAmtFor", new string[2]
			{
				"5.00",
				"3.50"
			});
			Dictionary<string, string[]> dictionary5 = dictionary2;
			string[] obj3 = new string[1];
			dateTime = dateTime2.AddDays(7.0);
			obj3[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary5.Add("MPaymentDate", obj3);
			dictionary2.Add("MPaidAmount", new string[1]
			{
				"850.00"
			});
			return new ImportTemplateModel
			{
				ColumnList = dictionary,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				FieldConfigList = templateConfig,
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2
			};
		}

		public ImportTemplateModel GetImportTemplateModelForSmartOrg(MContext ctx)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, dictionary2);
			DateTime dateTime = ctx.DateNow;
			DateTime dateTime2 = dateTime.AddMonths(-1);
			dictionary2.Add("MBizDate", new string[1]
			{
				dateTime2.ToString("yyyy-MM-dd")
			});
			Dictionary<string, string[]> dictionary3 = dictionary2;
			string[] obj = new string[1];
			dateTime = dateTime2.AddDays(20.0);
			obj[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary3.Add("MDueDate", obj);
			Dictionary<string, string[]> dictionary4 = dictionary2;
			string[] obj2 = new string[1];
			dateTime = dateTime2.AddDays(10.0);
			obj2[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary4.Add("MExpectedDate", obj2);
			dictionary2.Add("MQty", new string[2]
			{
				"1.0000",
				"1.0000"
			});
			dictionary2.Add("MPrice", new string[2]
			{
				"500.0000",
				"350.0000"
			});
			dictionary2.Add("MTaxAmtFor", new string[2]
			{
				"5.00",
				"3.50"
			});
			Dictionary<string, string[]> dictionary5 = dictionary2;
			string[] obj3 = new string[1];
			dateTime = dateTime2.AddDays(7.0);
			obj3[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary5.Add("MPaymentDate", obj3);
			dictionary2.Add("MPaidAmount", new string[1]
			{
				"850.00"
			});
			return new ImportTemplateModel
			{
				ColumnList = dictionary,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				FieldConfigList = templateConfig,
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary2
			};
		}

		private string GetBaseCurrencyID(MContext ctx)
		{
			REGCurrencyRepository rEGCurrencyRepository = new REGCurrencyRepository();
			BASCurrencyViewModel @base = rEGCurrencyRepository.GetBase(ctx, false, null, null);
			return @base.MCurrencyID;
		}

		public OperationResult ImportExpenseInvoiceList(MContext ctx, List<IVExpenseModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<BizVerificationInfor> list2 = ValidateExpense(ctx, list);
			if (list2.Count > 0)
			{
				operationResult.Success = false;
			}
			else
			{
				foreach (IVExpenseModel item in list)
				{
					GetExpenseModel(ctx, item);
				}
				List<CommandInfo> list3 = new List<CommandInfo>();
				list3.AddRange(IVExpenseRepository.GetImportExpenseCmdList(ctx, list));
				OperationResult operationResult2 = ResultHelper.ToOperationResult(list);
				if (!operationResult2.Success)
				{
					return operationResult2;
				}
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list3) > 0);
				if (!operationResult.Success)
				{
					string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ImportExpenseItemFail", "导入费用报销单失败!");
					list2.Add(new BizVerificationInfor
					{
						Message = text
					});
				}
			}
			operationResult.VerificationInfor = list2;
			return operationResult;
		}

		public List<BizVerificationInfor> ValidateExpense(MContext ctx, List<IVExpenseModel> expenseList)
		{
			REGCurrencyBusiness rEGCurrencyBusiness = new REGCurrencyBusiness();
			BDEmployeesBusiness bDEmployeesBusiness = new BDEmployeesBusiness();
			BDAccountBusiness bDAccountBusiness = new BDAccountBusiness();
			BDTrackBusiness bDTrackBusiness = new BDTrackBusiness();
			List<BizVerificationInfor> list = new List<BizVerificationInfor>();
			List<IOValidationResultModel> list2 = new List<IOValidationResultModel>();
			List<IVExpenseModel> list3 = (from x in expenseList
			where x.MBizDate != DateTime.MinValue
			select x).ToList();
			base.ValidateBizDate(ctx, list3, list2, true, "MBizDate");
			List<REGCurrencyViewModel> allCurrencyList = rEGCurrencyBusiness.GetAllCurrencyList(ctx, false, false);
			list3 = (from x in expenseList
			where x.MCyID != ctx.MBasCurrencyID
			select x).ToList();
			rEGCurrencyBusiness.CheckCurrencyExist(ctx, list3, "MCyID", list2, allCurrencyList);
			CheckCurrencyRateIsExist(ctx, list3, list2, allCurrencyList);
			List<IVExpenseEntryModel> expenseEntryList = new List<IVExpenseEntryModel>();
			expenseList.ForEach(delegate(IVExpenseModel x)
			{
				if (x.MEntryList != null && x.MEntryList.Count > 0)
				{
					expenseEntryList.AddRange(x.MEntryList);
				}
			});
			if (ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				List<BDAccountModel> bDAccountList = bDAccountBusiness.GetBDAccountList(ctx, new BDAccountListFilterModel
				{
					IncludeDisable = true
				}, true, false);
				bDAccountBusiness.CheckImportAccountExist(ctx, expenseEntryList, "MDebitAccount", list2, "MItemID", bDAccountList);
				bDAccountBusiness.CheckImportAccountExist(ctx, expenseEntryList, "MCreditAccount", list2, "MItemID", bDAccountList);
				bDAccountBusiness.CheckImportAccountExist(ctx, expenseEntryList, "MTaxAccount", list2, "MItemID", bDAccountList);
			}
			_expenseItemBiz.CheckExpenseItemExist(ctx, expenseEntryList, list2, "MItemID");
			string empty = string.Empty;
			bDEmployeesBusiness.CheckEmployeeExist(ctx, expenseList, "MEmployee", list2, ref empty, false, false);
			bDTrackBusiness.CheckTrackExist(ctx, expenseList.First().TrackItemNameList, expenseEntryList, list2);
			foreach (IOValidationResultModel item in list2)
			{
				string message = string.IsNullOrWhiteSpace(item.FieldValue) ? item.Message : string.Format(item.Message, item.FieldValue);
				list.Add(new BizVerificationInfor
				{
					Id = item.RowIndex.ToString(),
					Message = message,
					CheckItem = "RowIndex",
					RowIndex = item.RowIndex
				});
			}
			foreach (IVExpenseModel expense in expenseList)
			{
				int rowIndex = (expense.MRowIndex == 0) ? expenseList.IndexOf(expense) : expense.MRowIndex;
				if (expense.MBizDate == DateTime.MinValue)
				{
					string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "BizDateIsNull", "业务日期不能为空!");
					list.Add(new BizVerificationInfor
					{
						Id = rowIndex.ToString(),
						Message = text,
						CheckItem = "RowIndex",
						RowIndex = rowIndex
					});
				}
				if (expense.MDueDate < expense.MBizDate)
				{
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DueDateMustGreaterOrEqualDate", "Due date must be greater than or equal to date!");
					list.Add(new BizVerificationInfor
					{
						Id = rowIndex.ToString(),
						Message = text2,
						CheckItem = "RowIndex",
						RowIndex = rowIndex
					});
				}
				if (expense.MExpectedDate != DateTime.MinValue && expense.MExpectedDate < expense.MBizDate)
				{
					string text3 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ExpectedDateIsGreaterThanTheDate", "Expected Payment date must be greater than or equal to date!");
					list.Add(new BizVerificationInfor
					{
						Id = rowIndex.ToString(),
						Message = text3,
						CheckItem = "RowIndex",
						RowIndex = rowIndex
					});
				}
				foreach (IVExpenseEntryModel item2 in expense.ExpenseEntry)
				{
					List<BizVerificationInfor> list4 = new List<BizVerificationInfor>();
					int mRowIndex = item2.MRowIndex;
					if (string.IsNullOrWhiteSpace(item2.MDesc))
					{
						string text4 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceEntryDescHasNull", "描述不能为空!");
						list.Add(new BizVerificationInfor
						{
							Id = mRowIndex.ToString(),
							Message = text4,
							CheckItem = "RowIndex",
							RowIndex = mRowIndex
						});
					}
					if (item2.MQty <= decimal.Zero)
					{
						string text5 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "QuantityIsRequire", "数量必须为正数!");
						list.Add(new BizVerificationInfor
						{
							Id = mRowIndex.ToString(),
							Message = text5,
							CheckItem = "RowIndex",
							RowIndex = mRowIndex
						});
					}
					if (item2.MPrice <= decimal.Zero)
					{
						string text6 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "PriceIsRequire", "单价必须为正数!");
						list.Add(new BizVerificationInfor
						{
							Id = mRowIndex.ToString(),
							Message = text6,
							CheckItem = "RowIndex",
							RowIndex = mRowIndex
						});
					}
					if (item2.MTaxAmtFor < decimal.Zero)
					{
						string text7 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxAmountIsRequire", "税额必须大于0!");
						list.Add(new BizVerificationInfor
						{
							Id = mRowIndex.ToString(),
							Message = text7,
							CheckItem = "RowIndex",
							RowIndex = mRowIndex
						});
					}
					if (item2.MTaxAmtFor > item2.MQty * item2.MPrice)
					{
						string text8 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxAmtCanNotLargerAmt", "税额不能大于金额！");
						list.Add(new BizVerificationInfor
						{
							Id = mRowIndex.ToString(),
							Message = text8,
							CheckItem = "RowIndex",
							RowIndex = mRowIndex
						});
					}
				}
			}
			return list;
		}

		private void CheckCurrencyRateIsExist(MContext ctx, List<IVExpenseModel> expenseList, List<IOValidationResultModel> validationResult, List<REGCurrencyViewModel> foreignCurrencyList)
		{
			base.CheckCurrencyExchangeRateExist(ctx, expenseList, validationResult, foreignCurrencyList, "MCyID", "MBizDate");
			if (validationResult == null || validationResult.Count <= 0)
			{
				foreach (IVExpenseModel expense in expenseList)
				{
					string currencyId = expense.MCyID;
					if (expense.MCyID != ctx.MBasCurrencyID)
					{
						IEnumerable<REGCurrencyViewModel> source = from f in foreignCurrencyList
						where f.MCurrencyID == currencyId && f.MRateDate <= expense.MBizDate
						select f;
						if (source.Any())
						{
							string mRate = (from x in source
							orderby x.MRateDate
							select x).Last().MRate;
							decimal one = decimal.One;
							decimal.TryParse(mRate, out one);
							expense.MExchangeRate = one;
						}
					}
				}
			}
		}

		private void GetExpenseModel(MContext ctx, IVExpenseModel expModel)
		{
			expModel.IsNew = string.IsNullOrEmpty(expModel.MID);
			expModel.MID = (string.IsNullOrEmpty(expModel.MID) ? UUIDHelper.GetGuid() : expModel.MID);
			expModel.MStatus = 1;
			expModel.MRowIndex = expModel.MRowIndex;
			expModel.MContactID = expModel.MEmployee;
			GetExpenseEntryList(ctx, expModel, expModel.ExpenseEntry);
			expModel.MTotalAmtFor = expModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MAmountFor);
			expModel.MTotalAmt = expModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MAmount);
			expModel.MTaxTotalAmtFor = expModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmountFor);
			expModel.MTaxTotalAmt = expModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmount);
			expModel.MTaxAmtFor = expModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmtFor);
			expModel.MTaxAmt = expModel.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MTaxAmount);
		}

		private void GetExpenseEntryList(MContext ctx, IVExpenseModel expModel, List<IVExpenseEntryModel> entryList)
		{
			int num = 1;
			foreach (IVExpenseEntryModel entry in entryList)
			{
				entry.IsNew = string.IsNullOrEmpty(entry.MEntryID);
				entry.MEntryID = (string.IsNullOrEmpty(entry.MEntryID) ? UUIDHelper.GetGuid() : entry.MEntryID);
				entry.MID = expModel.MID;
				entry.MQty = Math.Round(Math.Abs(entry.MQty), 4);
				entry.MPrice = Math.Round(Math.Abs(entry.MPrice), 4);
				entry.MSeq = num;
				entry.MTaxAmtFor = Math.Round(Math.Abs(entry.MTaxAmtFor), 2);
				entry.MTaxAmountFor = Math.Round(Math.Abs(entry.MQty * entry.MPrice), 2);
				entry.MTaxAmount = Math.Round(entry.MTaxAmountFor * expModel.MExchangeRate, 2);
				entry.MTaxAmt = Math.Round(entry.MTaxAmtFor * expModel.MExchangeRate, 2);
				entry.MAmountFor = entry.MTaxAmountFor;
				entry.MAmount = entry.MTaxAmount;
				entry.MApproveAmt = entry.MTaxAmount;
				entry.MApproveAmtFor = entry.MTaxAmountFor;
				num++;
			}
		}

		public DataGridJson<IVExpenseModel> Get(MContext ctx, GetParam param)
		{
			DataGridJson<IVExpenseModel> dataGridJson = new DataGridJson<IVExpenseModel>();
			List<IVExpenseModel> list = dataGridJson.rows = GetExpenseListIncludeEntry(ctx, new IVExpenseListFilterModel(), param);
			dataGridJson.total = ((list != null) ? list.Count : 0);
			return dataGridJson;
		}

		public List<IVExpenseModel> Post(MContext ctx, PostParam<IVExpenseModel> param)
		{
			List<IVExpenseModel> dataList = param.DataList;
			OperationResult result = ImportExpenseInvoiceList(ctx, dataList);
			List<IVExpenseModel> list;
			List<IVExpenseModel> list2 = BusinessServiceBase.DealApiErrorMessageList(dataList, result, out list);
			if (list.Any())
			{
				if (list2.Any())
				{
					ImportExpenseInvoiceList(ctx, list2);
				}
				list2.AddRange(list);
			}
			return list2;
		}

		public IVExpenseModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}
	}
}
