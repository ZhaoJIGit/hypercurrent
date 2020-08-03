using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.REG;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVPaymentBusiness : BusinessServiceBase, IIVPaymentBusiness, IBasicBusiness<IVPaymentModel>
	{
		private BDItemBusiness _itemBiz = new BDItemBusiness();

		private REGTaxRateBusiness _taxRateBiz = new REGTaxRateBusiness();

		private BDTrackBusiness _trackBiz = new BDTrackBusiness();

		private BDContactsBusiness _contactBiz = new BDContactsBusiness();

		private REGCurrencyBusiness _currencyBiz = new REGCurrencyBusiness();

		private BASDataDictionaryBusiness _dictBiz = new BASDataDictionaryBusiness();

		private BDEmployeesBusiness _empBiz = new BDEmployeesBusiness();

		private BDExpenseItemBusiness _expenseItemBiz = new BDExpenseItemBusiness();

		private BDExpenseItemRepository expenseItem = new BDExpenseItemRepository();

		private BDItemRepository item = new BDItemRepository();

		public List<IVPaymentListModel> GetPaymentList(MContext ctx, string filterString)
		{
			return IVPaymentRepository.GetPaymentList(ctx, filterString);
		}

		public List<IVPaymentModel> GetPaymentListByFilter(MContext ctx, IVPaymentListFilterModel filter)
		{
			return IVPaymentRepository.GetPaymentListByFilter(ctx, filter);
		}

		public List<IVPaymentModel> GetPaymentListIncludeEntry(MContext ctx, IVPaymentListFilterModel filter)
		{
			DateTime dateTime = filter.MStartDate;
			filter.GreaterOrEqual("MBizDate", dateTime.ToString("yyyy-MM-dd"));
			dateTime = filter.MEndDate;
			filter.LessOrEqual("MBizDate", dateTime.ToString("yyyy-MM-dd"));
			filter.Equal("MIsDelete", 0);
			List<IVPaymentModel> list = IVPaymentRepository.GetPaymentModelIncludeEntry(ctx, filter);
			if (filter.IsContainReturn)
			{
				if (list == null)
				{
					list = new List<IVPaymentModel>();
				}
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.GreaterOrEqual("MBizDate", filter.MStartDate);
				sqlWhere.LessOrEqual("MBizDate", filter.MEndDate);
				sqlWhere.In("MType", new string[2]
				{
					"Receive_SaleReturn",
					"Receive_OtherReturn"
				});
				List<IVReceiveModel> receiveModelIncludeEntry = IVReceiveRepository.GetReceiveModelIncludeEntry(ctx, sqlWhere);
				if (receiveModelIncludeEntry != null && receiveModelIncludeEntry.Count() > 0)
				{
					list.AddRange(ConvertReceiveToPayment(receiveModelIncludeEntry));
				}
			}
			return list;
		}

		private List<IVPaymentModel> ConvertReceiveToPayment(List<IVReceiveModel> receiveList)
		{
			List<IVPaymentModel> list = new List<IVPaymentModel>();
			if (receiveList == null || receiveList.Count() == 0)
			{
				return list;
			}
			foreach (IVReceiveModel receive in receiveList)
			{
				IVPaymentModel iVPaymentModel = new IVPaymentModel();
				iVPaymentModel.MBankID = receive.MBankID;
				iVPaymentModel.MContactID = receive.MContactID;
				iVPaymentModel.MContactName = receive.MContactID;
				iVPaymentModel.MContactType = receive.MContactType;
				iVPaymentModel.MBizDate = receive.MBizDate;
				iVPaymentModel.MTaxTotalAmt = receive.MTaxTotalAmt * decimal.MinusOne;
				iVPaymentModel.MTaxTotalAmtFor = receive.MTaxTotalAmtFor * decimal.MinusOne;
				iVPaymentModel.MTotalAmt = receive.MTotalAmt * decimal.MinusOne;
				iVPaymentModel.MTotalAmtFor = receive.MTotalAmtFor * decimal.MinusOne;
				List<IVPaymentEntryModel> list2 = new List<IVPaymentEntryModel>();
				List<IVReceiveEntryModel> receiveEntry = receive.ReceiveEntry;
				if (receiveEntry != null && receiveEntry.Count() > 0)
				{
					foreach (IVReceiveEntryModel item2 in receiveEntry)
					{
						IVPaymentEntryModel iVPaymentEntryModel = new IVPaymentEntryModel();
						iVPaymentEntryModel.MTaxAmount = item2.MTaxAmount * decimal.MinusOne;
						iVPaymentEntryModel.MTaxAmountFor = item2.MTaxAmountFor * decimal.MinusOne;
						iVPaymentEntryModel.MAmount = item2.MTaxAmount * decimal.MinusOne;
						iVPaymentEntryModel.MAmountFor = item2.MTaxAmountFor * decimal.MinusOne;
						iVPaymentEntryModel.MTaxAmt = item2.MTaxAmt * decimal.MinusOne;
						iVPaymentEntryModel.MTaxAmtFor = item2.MTaxAmtFor * decimal.MinusOne;
						iVPaymentEntryModel.MItemID = item2.MItemID;
						iVPaymentEntryModel.MTrackItem1 = item2.MTrackItem1;
						iVPaymentEntryModel.MTrackItem2 = item2.MTrackItem2;
						iVPaymentEntryModel.MTrackItem3 = item2.MTrackItem3;
						iVPaymentEntryModel.MTrackItem4 = item2.MTrackItem4;
						iVPaymentEntryModel.MTrackItem5 = item2.MTrackItem5;
						list2.Add(iVPaymentEntryModel);
					}
				}
				iVPaymentModel.PaymentEntry = list2;
				list.Add(iVPaymentModel);
			}
			return list;
		}

		public OperationResult UpdatePayment(MContext ctx, IVPaymentModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (model.PaymentEntry != null && model.PaymentEntry.Any((IVPaymentEntryModel payment) => payment.MTaxAmtFor > payment.MAmountFor))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "CannotGreaterAmount", "The tax amount cannot be greater than the total amount");
				return operationResult;
			}
			if (string.IsNullOrEmpty(model.MCyID))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CnyCannotEmpty", "Currency can't be empty!");
				return operationResult;
			}
			operationResult = REGCurrencyRepository.CheckCurrency(ctx, model.MCyID, model.MBizDate);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (model.MBizDate < ctx.MBeginDate && ctx.MInitBalanceOver)
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InitBalanceIsOver", "The initial balance has been completed and is not allowed to initialize the document operation!");
				return operationResult;
			}
			operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, model.MBizDate);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			model.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			if (!string.IsNullOrEmpty(model.MID))
			{
				IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, model.MID);
				if (!string.IsNullOrEmpty(paymentEditModel.MID))
				{
					model.MSource = paymentEditModel.MSource;
					model.MReconcileStatu = paymentEditModel.MReconcileStatu;
					model.MBankID = (string.IsNullOrEmpty(model.MBankID) ? paymentEditModel.MBankID : model.MBankID);
				}
				OperationResult operationResult2 = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
				if (!operationResult2.Success || Math.Abs(paymentEditModel.MVerificationAmt) > decimal.Zero || Math.Abs(paymentEditModel.MReconcileAmtFor) > decimal.Zero)
				{
					return new OperationResult
					{
						Success = false,
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！")
					};
				}
			}
			BDTrackRepository bDTrackRepository = new BDTrackRepository();
			List<NameValueModel> trackBasicInfo = bDTrackRepository.GetTrackBasicInfo(ctx, null, false, true);
			if (trackBasicInfo != null && trackBasicInfo.Count > 0)
			{
				foreach (IVPaymentEntryModel item2 in model.PaymentEntry)
				{
					item2.MTrackItem1 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item2.MTrackItem1, 1);
					item2.MTrackItem2 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item2.MTrackItem2, 2);
					item2.MTrackItem3 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item2.MTrackItem3, 3);
					item2.MTrackItem4 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item2.MTrackItem4, 4);
					item2.MTrackItem5 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item2.MTrackItem5, 5);
				}
			}
			return IVPaymentRepository.UpdatePayment(ctx, model);
		}

		public IVPaymentModel GetPaymentEditModel(MContext ctx, string pkID)
		{
			IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, pkID);
			if (!string.IsNullOrEmpty(paymentEditModel.MID))
			{
				paymentEditModel.Verification = GetVerification(ctx, paymentEditModel);
			}
			paymentEditModel.MActionPermission = GetActionPermissionModel(ctx, paymentEditModel);
			return paymentEditModel;
		}

		private IVPaymentPermissionModel GetActionPermissionModel(MContext ctx, IVPaymentModel model)
		{
			IVPaymentPermissionModel iVPaymentPermissionModel = new IVPaymentPermissionModel();
			BDBankAccountBusiness bDBankAccountBusiness = new BDBankAccountBusiness();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountBusiness.GetBDBankAccountEditModel(ctx, model.MBankID);
			if (!base.HavePermission(ctx, "Bank", "Change"))
			{
				iVPaymentPermissionModel.MHaveAction = false;
				return iVPaymentPermissionModel;
			}
			bool flag = base.HavePermission(ctx, "BankAccount", "Change");
			if (string.IsNullOrEmpty(model.MID))
			{
				iVPaymentPermissionModel.MHaveAction = false;
				iVPaymentPermissionModel.MIsCanEdit = true;
				iVPaymentPermissionModel.MIsInitCanEdit = true;
				return iVPaymentPermissionModel;
			}
			iVPaymentPermissionModel.MHaveAction = true;
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (!flag || !base.HavePermission(ctx, "General_Ledger", "View") || model.MBizDate < ctx.MBeginDate || !GLInterfaceRepository.IsBillCreatedVoucher(ctx, model.MID))
			{
				iVPaymentPermissionModel.MIsCanViewVoucherCreateDetail = false;
			}
			if (!flag || !operationResult.Success || Math.Abs(model.MVerificationAmt) > decimal.Zero || Math.Abs(model.MReconcileAmtFor) > decimal.Zero || model.MBizDate < ctx.MBeginDate)
			{
				iVPaymentPermissionModel.MIsCanEdit = false;
				iVPaymentPermissionModel.MIsCanVoid = false;
				iVPaymentPermissionModel.MIsCanDelete = false;
			}
			if (!flag || Math.Abs(model.MTaxTotalAmtFor) == Math.Abs(model.MVerificationAmt))
			{
				iVPaymentPermissionModel.MIsCanVerification = false;
			}
			else
			{
				IVVerificationBusiness iVVerificationBusiness = new IVVerificationBusiness();
				List<IVVerificationInforModel> customerWaitForVerificationInfor = iVVerificationBusiness.GetCustomerWaitForVerificationInfor(ctx, model.MID, "Payment");
				if (customerWaitForVerificationInfor == null || customerWaitForVerificationInfor.Count == 0)
				{
					iVPaymentPermissionModel.MIsCanVerification = false;
				}
			}
			if (!flag || !operationResult.Success || model.MBizDate < ctx.MBeginDate || Math.Abs(model.MVerificationAmt) > decimal.Zero || Math.Abs(model.MReconcileAmtFor) > decimal.Zero)
			{
				iVPaymentPermissionModel.MIsCanEdit = false;
			}
			if (!flag || !operationResult.Success || ctx.MInitBalanceOver || Math.Abs(model.MVerificationAmt) > decimal.Zero || Math.Abs(model.MReconcileAmtFor) > decimal.Zero)
			{
				iVPaymentPermissionModel.MIsInitCanEdit = false;
			}
			if (model.MBizDate < ctx.MBeginDate)
			{
				iVPaymentPermissionModel.MIsCanViewReconcile = false;
				iVPaymentPermissionModel.MIsCanDeleteReconcile = false;
				iVPaymentPermissionModel.MMarkAsUnReconciled = false;
				iVPaymentPermissionModel.MMarkAsReconciled = false;
			}
			else
			{
				switch (model.MReconcileStatu)
				{
				case 201:
					iVPaymentPermissionModel.MIsCanViewReconcile = false;
					iVPaymentPermissionModel.MIsCanDeleteReconcile = false;
					iVPaymentPermissionModel.MMarkAsUnReconciled = false;
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "Change") || !bDBankAccountEditModel.MIsNeedReconcile)
					{
						iVPaymentPermissionModel.MMarkAsReconciled = false;
					}
					break;
				case 202:
				case 203:
					iVPaymentPermissionModel.MMarkAsReconciled = false;
					iVPaymentPermissionModel.MMarkAsUnReconciled = false;
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "View"))
					{
						iVPaymentPermissionModel.MIsCanViewReconcile = false;
					}
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "Change"))
					{
						iVPaymentPermissionModel.MIsCanDeleteReconcile = false;
					}
					break;
				case 204:
					iVPaymentPermissionModel.MIsCanViewReconcile = false;
					iVPaymentPermissionModel.MIsCanDeleteReconcile = false;
					iVPaymentPermissionModel.MMarkAsReconciled = false;
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "Change") || !bDBankAccountEditModel.MIsNeedReconcile)
					{
						iVPaymentPermissionModel.MMarkAsUnReconciled = false;
					}
					break;
				}
			}
			return iVPaymentPermissionModel;
		}

		public IVPaymentViewModel GetPaymentViewModel(MContext ctx, string pkID)
		{
			IVPaymentViewModel paymentViewModel = IVPaymentRepository.GetPaymentViewModel(ctx, pkID);
			if (!string.IsNullOrEmpty(paymentViewModel.MID))
			{
				paymentViewModel.Verification = GetVerification(ctx, paymentViewModel);
				paymentViewModel.MActionPermission = GetActionPermissionModel(ctx, paymentViewModel);
			}
			return paymentViewModel;
		}

		private List<IVVerificationListModel> GetVerification(MContext ctx, IVPaymentModel model)
		{
			List<IVVerificationListModel> list = new List<IVVerificationListModel>();
			IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
			iVVerificationListFilterModel.MBillID = model.MID;
			iVVerificationListFilterModel.MBizBillType = "Payment";
			iVVerificationListFilterModel.MBizType = model.MType;
			iVVerificationListFilterModel.MContactID = model.MContactID;
			iVVerificationListFilterModel.MCurrencyID = model.MCyID;
			iVVerificationListFilterModel.MViewVerif = true;
			if (model.MType == "Pay_Other" && model.MContactType == "Employees" && string.IsNullOrEmpty(model.MContactID))
			{
				IVVerificationModel salaryMergePaymentVerifData = IVVerificationRepository.GetSalaryMergePaymentVerifData(ctx, iVVerificationListFilterModel);
				if (!string.IsNullOrEmpty(salaryMergePaymentVerifData.MID))
				{
					IVVerificationListModel iVVerificationListModel = new IVVerificationListModel();
					iVVerificationListModel.MSourceBillID = model.MID;
					iVVerificationListModel.IsMergePay = true;
					iVVerificationListModel.MSourceBillType = "Payment";
					iVVerificationListModel.MBizBillType = "PayRun";
					iVVerificationListModel.MBillID = salaryMergePaymentVerifData.MSourceBillID;
					iVVerificationListModel.MBizType = "Pay_Salary";
					iVVerificationListModel.VerificationID = salaryMergePaymentVerifData.MID;
					iVVerificationListModel.MHaveVerificationAmt = salaryMergePaymentVerifData.MAmount;
					iVVerificationListModel.MHaveVerificationAmtFor = salaryMergePaymentVerifData.MAmount;
					iVVerificationListModel.MBillNo = salaryMergePaymentVerifData.MNumber;
					list.Add(iVVerificationListModel);
				}
			}
			if (!list.Any())
			{
				list = IVVerificationRepository.GetHistoryVerifData(ctx, iVVerificationListFilterModel);
			}
			return list;
		}

		public OperationResult UpdateReconcileStatu(MContext ctx, string paymentId, IVReconcileStatus statu)
		{
			OperationResult operationResult = new OperationResult();
			IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, paymentId);
			if (string.IsNullOrEmpty(paymentEditModel.MID))
			{
				operationResult.Success = false;
				return operationResult;
			}
			if (paymentEditModel.MReconcileStatu == 203 || paymentEditModel.MReconcileStatu == 202)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！");
				operationResult.Success = false;
				return operationResult;
			}
			return IVPaymentRepository.UpdateReconcileStatu(ctx, paymentId, statu);
		}

		public OperationResult UpdateReconcileStatu(MContext ctx, ParamBase param, IVReconcileStatus statu)
		{
			return new OperationResult();
		}

		[Obsolete]
		public OperationResult DeletePayment(MContext ctx, IVPaymentModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (IVVerificationRepository.CheckIsCanEditOrDelete(ctx, "Payment", model.MID))
			{
				operationResult = IVPaymentRepository.DeletePayment(ctx, model);
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = "1";
			}
			return operationResult;
		}

		public List<IVPaymentModel> GetInitList(MContext ctx)
		{
			return IVPaymentRepository.GetInitList(ctx);
		}

		public DataGridJson<IVPaymentModel> GetInitPaymentListByPage(MContext ctx, IVPaymentListFilterModel filter)
		{
			return IVPaymentRepository.GetInitPaymentListByPage(ctx, filter);
		}

		public OperationResult DeletePaymentList(MContext ctx, ParamBase param)
		{
			return IVPaymentRepository.DeletePaymentList(ctx, param);
		}

		public OperationResult ImportPaymentList(MContext ctx, List<IVPaymentModel> list)
		{
			List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();
			OperationResult operationResult = base.ValidateImportCurrency(ctx, list[0].MBankID, (from f in list
			select f.MCyID).ToList());
			if (!operationResult.Success)
			{
				return operationResult;
			}
			string empty = string.Empty;
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<IVPaymentEntryModel> list3 = new List<IVPaymentEntryModel>();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			foreach (IVPaymentModel item2 in list)
			{
				bool flag = false;
				switch (item2.MContactType)
				{
				case "Supplier":
					item2.MType = "Pay_Purchase";
					break;
				case "Employees":
					item2.MType = "Pay_Other";
					flag = true;
					break;
				case "Other":
					item2.MType = "Pay_Other";
					break;
				}
				int num = 1;
				foreach (IVPaymentEntryModel item3 in item2.PaymentEntry)
				{
					if (flag)
					{
						item3.MItemID = item3.MExpItemID;
						item3.MTaxID = string.Empty;
						item3.MTaxAmt = decimal.Zero;
						item3.MTaxAmtFor = decimal.Zero;
					}
					item3.MSeq = num;
					num++;
				}
				if (flag)
				{
					item2.MContactID = item2.MEmployeeID;
					item2.MTaxID = "No_Tax";
					item2.MTaxAmt = decimal.Zero;
					item2.MTaxAmtFor = decimal.Zero;
				}
				list3.AddRange(item2.PaymentEntry);
			}
			List<CommandInfo> list4 = new List<CommandInfo>();
			base.ValidateBizDate(ctx, list, validationResult, false, "MBizDate");
			List<IVPaymentModel> list5 = (from f in list
			where f.MContactType != "Employees"
			select f).ToList();
			if (list5.Any())
			{
				list4.AddRange(_contactBiz.CheckContactExist(ctx, list5, ref empty, validationResult, true, "MItemID", -1));
			}
			string empty2 = string.Empty;
			List<IVPaymentModel> list6 = (from f in list
			where f.MContactType == "Employees"
			select f).ToList();
			if (list6.Any())
			{
				list4.AddRange(_empBiz.CheckEmployeeExist(ctx, list6, "MContactID", validationResult, ref empty2, true, false));
			}
			list = list5.Concat(list6).ToList();
			_currencyBiz.CheckCurrencyExist(ctx, list, "MCyID", validationResult);
			List<REGCurrencyViewModel> allCurrencyList = _currencyBiz.GetAllCurrencyList(ctx, false, false);
			base.CheckCurrencyExchangeRateExist(ctx, list, validationResult, allCurrencyList, "MCyID", "MBizDate");
			_dictBiz.CheckTaxTypeExist(ctx, list, validationResult, null);
			List<BDExpenseItemModel> childrenExpenseItemList = expenseItem.GetChildrenExpenseItemList(ctx, true, true);
			List<BDItemModel> itemListIgnoreLocale = item.GetItemListIgnoreLocale(ctx, true);
			foreach (IVPaymentModel item4 in list)
			{
				foreach (IVPaymentEntryModel item5 in item4.PaymentEntry)
				{
					if (item4.MContactType == "Employees")
					{
						_expenseItemBiz.CheckExpenseItemExist(ctx, item5, ref childrenExpenseItemList, validationResult, "MItemID", -1);
					}
					else
					{
						_itemBiz.CheckItemExist(ctx, item5, ref itemListIgnoreLocale, validationResult, "MItemID", "MItemID", -1);
					}
				}
			}
			base.ValidateDiscount<IVPaymentModel, IVPaymentEntryModel>(ctx, list, validationResult);
			base.ValidateTotalAmount<IVPaymentModel, IVPaymentEntryModel>(ctx, list, validationResult);
			base.CheckTaxRateSelect<IVPaymentModel, IVPaymentEntryModel>(ctx, list, validationResult, true);
			_taxRateBiz.CheckTaxRateExist(ctx, list3, validationResult);
			_trackBiz.CheckTrackExist(ctx, list[0].TrackItemNameList, list3, validationResult);
			base.SetValidationResult(ctx, operationResult, validationResult, false);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (list4.Any())
			{
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list4) > 0);
				if (!operationResult.Success)
				{
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "NewContactSaveFailed", "The new contact:{0} save failed!"), empty)
					});
					operationResult.VerificationInfor.Add(new BizVerificationInfor
					{
						Level = AlertEnum.Error,
						Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "NewEmpSaveFailed", "The new Employee:{0} save failed!"), empty2)
					});
					return operationResult;
				}
			}
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			string empty5 = string.Empty;
			list2.AddRange(IVPaymentRepository.ImportPaymentList(ctx, list, ref empty3, ref empty4, ref empty5));
			OperationResult operationResult2 = ResultHelper.ToOperationResult(list);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			if (!string.IsNullOrWhiteSpace(empty3))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = empty3
				});
				return operationResult;
			}
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			operationResult.Tag = javaScriptSerializer.Serialize(new
			{
				NewContact = empty,
				NewEmp = empty2,
				StartDate = empty4,
				EndDate = empty5
			});
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list2) > 0);
			return operationResult;
		}

		public DataGridJson<IVPaymentModel> Get(MContext ctx, GetParam param)
		{
			throw new NotImplementedException();
		}

		public List<IVPaymentModel> Post(MContext ctx, PostParam<IVPaymentModel> param)
		{
			throw new NotImplementedException();
		}

		public IVPaymentModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}
	}
}
