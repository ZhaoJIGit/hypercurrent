using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.BAS;
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
	public class IVReceiveBusiness : BusinessServiceBase, IIVReceiveBusiness, IBasicBusiness<IVReceiveModel>
	{
		private BDContactsRepository _bdContacts = new BDContactsRepository();

		private REGCurrencyRepository _bdCurrency = new REGCurrencyRepository();

		private BDItemRepository _item = new BDItemRepository();

		private REGTaxRateRepository _taxRate = new REGTaxRateRepository();

		private BDTrackRepository _track = new BDTrackRepository();

		private BDAccountRepository _account = new BDAccountRepository();

		private BDItemBusiness _itemBiz = new BDItemBusiness();

		private REGTaxRateBusiness _taxRateBiz = new REGTaxRateBusiness();

		private BDTrackBusiness _trackBiz = new BDTrackBusiness();

		private BDContactsBusiness _contactBiz = new BDContactsBusiness();

		private REGCurrencyBusiness _currencyBiz = new REGCurrencyBusiness();

		private BASDataDictionaryBusiness _dictBiz = new BASDataDictionaryBusiness();

		private BDExpenseItemRepository _expenseItem = new BDExpenseItemRepository();

		private BDEmployeesBusiness _empBiz = new BDEmployeesBusiness();

		public List<IVReceiveListModel> GetReceiveList(MContext ctx, string filterString)
		{
			return IVReceiveRepository.GetReceiveList(ctx, filterString);
		}

		public OperationResult UpdateReceive(MContext ctx, IVReceiveModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (model.ReceiveEntry != null && model.ReceiveEntry.Any((IVReceiveEntryModel receive) => receive.MTaxAmtFor > receive.MAmountFor))
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
			model.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
			if (!string.IsNullOrEmpty(model.MID))
			{
				IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, model.MID);
				if (!string.IsNullOrEmpty(receiveEditModel.MID))
				{
					model.MSource = receiveEditModel.MSource;
					model.MReconcileStatu = receiveEditModel.MReconcileStatu;
					model.MBankID = (string.IsNullOrEmpty(model.MBankID) ? receiveEditModel.MBankID : model.MBankID);
				}
				OperationResult operationResult2 = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
				if (!operationResult2.Success || Math.Abs(receiveEditModel.MVerificationAmt) > decimal.Zero || Math.Abs(receiveEditModel.MReconcileAmtFor) > decimal.Zero)
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
				foreach (IVReceiveEntryModel item in model.ReceiveEntry)
				{
					item.MTrackItem1 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem1, 1);
					item.MTrackItem2 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem2, 2);
					item.MTrackItem3 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem3, 3);
					item.MTrackItem4 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem4, 4);
					item.MTrackItem5 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem5, 5);
				}
			}
			return IVReceiveRepository.UpdateReceive(ctx, model);
		}

		public IVReceiveModel GetReceiveEditModel(MContext ctx, string pkID)
		{
			IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, pkID);
			if (!string.IsNullOrEmpty(receiveEditModel.MID))
			{
				receiveEditModel.Verification = GetVerification(ctx, receiveEditModel);
			}
			receiveEditModel.MActionPermission = GetActionPermissionModel(ctx, receiveEditModel);
			return receiveEditModel;
		}

		public IVReceiveViewModel GetReceiveViewModel(MContext ctx, string pkID)
		{
			IVReceiveViewModel receiveViewModel = IVReceiveRepository.GetReceiveViewModel(ctx, pkID);
			if (!string.IsNullOrEmpty(receiveViewModel.MID))
			{
				receiveViewModel.Verification = GetVerification(ctx, receiveViewModel);
			}
			receiveViewModel.MActionPermission = GetActionPermissionModel(ctx, receiveViewModel);
			return receiveViewModel;
		}

		private IVReceivePermissionModel GetActionPermissionModel(MContext ctx, IVReceiveModel model)
		{
			IVReceivePermissionModel iVReceivePermissionModel = new IVReceivePermissionModel();
			BDBankAccountBusiness bDBankAccountBusiness = new BDBankAccountBusiness();
			BDBankAccountEditModel bDBankAccountEditModel = bDBankAccountBusiness.GetBDBankAccountEditModel(ctx, model.MBankID);
			if (!base.HavePermission(ctx, "Bank", "Change"))
			{
				iVReceivePermissionModel.MHaveAction = false;
				return iVReceivePermissionModel;
			}
			bool flag = base.HavePermission(ctx, "BankAccount", "Change");
			if (string.IsNullOrEmpty(model.MID))
			{
				iVReceivePermissionModel.MHaveAction = false;
				iVReceivePermissionModel.MIsCanEdit = true;
				iVReceivePermissionModel.MIsInitCanEdit = true;
				return iVReceivePermissionModel;
			}
			iVReceivePermissionModel.MHaveAction = true;
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (!flag || !base.HavePermission(ctx, "General_Ledger", "View") || !GLInterfaceRepository.IsBillCreatedVoucher(ctx, model.MID) || model.MBizDate < ctx.MBeginDate)
			{
				iVReceivePermissionModel.MIsCanViewVoucherCreateDetail = false;
			}
			if (!flag || !operationResult.Success || Math.Abs(model.MVerificationAmt) > decimal.Zero || Math.Abs(model.MReconcileAmtFor) > decimal.Zero || model.MBizDate < ctx.MBeginDate)
			{
				iVReceivePermissionModel.MIsCanEdit = false;
				iVReceivePermissionModel.MIsCanVoid = false;
				iVReceivePermissionModel.MIsCanDelete = false;
			}
			if (Math.Abs(model.MTaxTotalAmtFor) == Math.Abs(model.MVerificationAmt))
			{
				iVReceivePermissionModel.MIsCanVerification = false;
			}
			else
			{
				IVVerificationBusiness iVVerificationBusiness = new IVVerificationBusiness();
				List<IVVerificationInforModel> customerWaitForVerificationInfor = iVVerificationBusiness.GetCustomerWaitForVerificationInfor(ctx, model.MID, "Receive");
				if (!flag || customerWaitForVerificationInfor == null || customerWaitForVerificationInfor.Count == 0)
				{
					iVReceivePermissionModel.MIsCanVerification = false;
				}
			}
			if (!flag || !operationResult.Success || model.MBizDate < ctx.MBeginDate || Math.Abs(model.MVerificationAmt) > decimal.Zero || Math.Abs(model.MReconcileAmtFor) > decimal.Zero)
			{
				iVReceivePermissionModel.MIsCanEdit = false;
			}
			if (!flag || !operationResult.Success || ctx.MInitBalanceOver || Math.Abs(model.MVerificationAmt) > decimal.Zero || Math.Abs(model.MReconcileAmtFor) > decimal.Zero)
			{
				iVReceivePermissionModel.MIsInitCanEdit = false;
			}
			if (model.MBizDate < ctx.MBeginDate)
			{
				iVReceivePermissionModel.MIsCanViewReconcile = false;
				iVReceivePermissionModel.MIsCanDeleteReconcile = false;
				iVReceivePermissionModel.MMarkAsUnReconciled = false;
				iVReceivePermissionModel.MMarkAsReconciled = false;
			}
			else
			{
				switch (model.MReconcileStatu)
				{
				case 201:
					iVReceivePermissionModel.MIsCanViewReconcile = false;
					iVReceivePermissionModel.MIsCanDeleteReconcile = false;
					iVReceivePermissionModel.MMarkAsUnReconciled = false;
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "Change") || !bDBankAccountEditModel.MIsNeedReconcile)
					{
						iVReceivePermissionModel.MMarkAsReconciled = false;
					}
					break;
				case 202:
				case 203:
					iVReceivePermissionModel.MMarkAsReconciled = false;
					iVReceivePermissionModel.MMarkAsUnReconciled = false;
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "View"))
					{
						iVReceivePermissionModel.MIsCanViewReconcile = false;
					}
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "Change"))
					{
						iVReceivePermissionModel.MIsCanDeleteReconcile = false;
					}
					break;
				case 204:
					iVReceivePermissionModel.MIsCanViewReconcile = false;
					iVReceivePermissionModel.MIsCanDeleteReconcile = false;
					iVReceivePermissionModel.MMarkAsReconciled = false;
					if (!base.HavePermission(ctx, "Bank_Reconciliation", "Change") || !bDBankAccountEditModel.MIsNeedReconcile)
					{
						iVReceivePermissionModel.MMarkAsUnReconciled = false;
					}
					break;
				}
			}
			return iVReceivePermissionModel;
		}

		private List<IVVerificationListModel> GetVerification(MContext ctx, IVReceiveModel model)
		{
			IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
			iVVerificationListFilterModel.MBillID = model.MID;
			iVVerificationListFilterModel.MBizBillType = "Receive";
			iVVerificationListFilterModel.MBizType = model.MType;
			iVVerificationListFilterModel.MContactID = model.MContactID;
			iVVerificationListFilterModel.MCurrencyID = model.MCyID;
			iVVerificationListFilterModel.MViewVerif = true;
			return IVVerificationRepository.GetHistoryVerifData(ctx, iVVerificationListFilterModel);
		}

		public OperationResult UpdateReconcileStatu(MContext ctx, string receiveId, IVReconcileStatus statu)
		{
			OperationResult operationResult = new OperationResult();
			IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, receiveId);
			if (string.IsNullOrEmpty(receiveEditModel.MID))
			{
				operationResult.Success = false;
				return operationResult;
			}
			if (receiveEditModel.MReconcileStatu == 203 || receiveEditModel.MReconcileStatu == 202)
			{
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！");
				operationResult.Success = false;
				return operationResult;
			}
			return IVReceiveRepository.UpdateReconcileStatu(ctx, receiveId, statu);
		}

		public OperationResult UpdateReconcileStatu(MContext ctx, ParamBase param, IVReconcileStatus statu)
		{
			return new OperationResult();
		}

		public OperationResult DeleteReceive(MContext ctx, IVReceiveModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (IVVerificationRepository.CheckIsCanEditOrDelete(ctx, "Receive", model.MID))
			{
				operationResult = IVReceiveRepository.DeleteReceive(ctx, model);
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = "1";
			}
			return operationResult;
		}

		public List<IVReceiveModel> GetInitList(MContext ctx)
		{
			return IVReceiveRepository.GetInitList(ctx);
		}

		public DataGridJson<IVReceiveModel> GetInitReceiveListByPage(MContext ctx, IVReceiveListFilterModel filter)
		{
			return IVReceiveRepository.GetInitReceiveListByPage(ctx, filter);
		}

		public List<IVReceiveModel> GetReceiveListByFilter(MContext ctx, IVReceiveListFilterModel filter)
		{
			return IVReceiveRepository.GetReceiveListByFilter(ctx, filter);
		}

		public List<IVReceiveModel> GetReceiveListIncludeEnetry(MContext ctx, IVReceiveListFilterModel filter)
		{
			filter.GreaterOrEqual("MBizDate", filter.MStartDate);
			filter.LessOrEqual("MBizDate", filter.MEndDate);
			filter.Equal("MIsDelete", 0);
			List<IVReceiveModel> list = IVReceiveRepository.GetReceiveModelIncludeEntry(ctx, filter);
			if (filter.IsContainReturn)
			{
				if (list == null)
				{
					list = new List<IVReceiveModel>();
				}
				SqlWhere sqlWhere = new SqlWhere();
				sqlWhere.GreaterOrEqual("MBizDate", filter.MStartDate);
				sqlWhere.LessOrEqual("MBizDate", filter.MEndDate);
				sqlWhere.In("MType", new string[2]
				{
					"Pay_OtherReturn",
					"Pay_PurReturn"
				});
				List<IVPaymentModel> paymentModelIncludeEntry = IVPaymentRepository.GetPaymentModelIncludeEntry(ctx, sqlWhere);
				if (paymentModelIncludeEntry != null && paymentModelIncludeEntry.Count() > 0)
				{
					list.AddRange(ConvertPaymentToReceive(paymentModelIncludeEntry));
				}
			}
			return list;
		}

		private List<IVReceiveModel> ConvertPaymentToReceive(List<IVPaymentModel> paymentList)
		{
			List<IVReceiveModel> list = new List<IVReceiveModel>();
			if (paymentList == null || paymentList.Count() == 0)
			{
				return list;
			}
			foreach (IVPaymentModel payment in paymentList)
			{
				IVReceiveModel iVReceiveModel = new IVReceiveModel();
				iVReceiveModel.MBankID = payment.MBankID;
				iVReceiveModel.MContactID = payment.MContactID;
				iVReceiveModel.MContactName = payment.MContactID;
				iVReceiveModel.MContactType = payment.MContactType;
				iVReceiveModel.MBizDate = payment.MBizDate;
				iVReceiveModel.MTaxTotalAmt = payment.MTaxTotalAmt * decimal.MinusOne;
				iVReceiveModel.MTaxTotalAmtFor = payment.MTaxTotalAmtFor * decimal.MinusOne;
				iVReceiveModel.MTotalAmt = payment.MTotalAmt * decimal.MinusOne;
				iVReceiveModel.MTotalAmtFor = payment.MTotalAmtFor * decimal.MinusOne;
				List<IVReceiveEntryModel> list2 = new List<IVReceiveEntryModel>();
				List<IVPaymentEntryModel> paymentEntry = payment.PaymentEntry;
				if (paymentEntry != null && paymentEntry.Count() > 0)
				{
					foreach (IVPaymentEntryModel item in paymentEntry)
					{
						IVReceiveEntryModel iVReceiveEntryModel = new IVReceiveEntryModel();
						iVReceiveEntryModel.MTaxAmount = item.MTaxAmount * decimal.MinusOne;
						iVReceiveEntryModel.MTaxAmountFor = item.MTaxAmountFor * decimal.MinusOne;
						iVReceiveEntryModel.MAmount = item.MTaxAmount * decimal.MinusOne;
						iVReceiveEntryModel.MAmountFor = item.MTaxAmountFor * decimal.MinusOne;
						iVReceiveEntryModel.MTaxAmt = item.MTaxAmt * decimal.MinusOne;
						iVReceiveEntryModel.MTaxAmtFor = item.MTaxAmtFor * decimal.MinusOne;
						iVReceiveEntryModel.MItemID = item.MItemID;
						iVReceiveEntryModel.MTrackItem1 = item.MTrackItem1;
						iVReceiveEntryModel.MTrackItem2 = item.MTrackItem2;
						iVReceiveEntryModel.MTrackItem3 = item.MTrackItem3;
						iVReceiveEntryModel.MTrackItem4 = item.MTrackItem4;
						iVReceiveEntryModel.MTrackItem5 = item.MTrackItem5;
						list2.Add(iVReceiveEntryModel);
					}
				}
				iVReceiveModel.ReceiveEntry = list2;
				list.Add(iVReceiveModel);
			}
			return list;
		}

		public OperationResult DeleteReceiveList(MContext ctx, ParamBase param)
		{
			return IVReceiveRepository.DeleteReceiveList(ctx, param);
		}

		public OperationResult ImportReceiveList(MContext ctx, List<IVReceiveModel> list)
		{
			List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();
			SetReceiveType(list);
			OperationResult operationResult = base.ValidateImportCurrency(ctx, list[0].MBankID, (from f in list
			select f.MCyID).ToList());
			if (!operationResult.Success)
			{
				return operationResult;
			}
			string empty = string.Empty;
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<IVReceiveEntryModel> entryList = new List<IVReceiveEntryModel>();
			list.ForEach(delegate(IVReceiveModel f)
			{
				entryList.AddRange(f.ReceiveEntry);
			});
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			base.ValidateBizDate(ctx, list, validationResult, false, "MBizDate");
			List<CommandInfo> list3 = _contactBiz.CheckContactExist(ctx, list, ref empty, validationResult, true, "MItemID", -1);
			_currencyBiz.CheckCurrencyExist(ctx, list, "MCyID", validationResult);
			List<REGCurrencyViewModel> allCurrencyList = _currencyBiz.GetAllCurrencyList(ctx, false, false);
			base.CheckCurrencyExchangeRateExist(ctx, list, validationResult, allCurrencyList, "MCyID", "MBizDate");
			_dictBiz.CheckTaxTypeExist(ctx, list, validationResult, null);
			_itemBiz.CheckItemExist(ctx, entryList, validationResult, "MItemID", "MItemID");
			base.ValidateDiscount<IVReceiveModel, IVReceiveEntryModel>(ctx, list, validationResult);
			base.ValidateTotalAmount<IVReceiveModel, IVReceiveEntryModel>(ctx, list, validationResult);
			base.CheckTaxRateSelect<IVReceiveModel, IVReceiveEntryModel>(ctx, list, validationResult, true);
			_taxRateBiz.CheckTaxRateExist(ctx, entryList, validationResult);
			_trackBiz.CheckTrackExist(ctx, list[0].TrackItemNameList, entryList, validationResult);
			base.SetValidationResult(ctx, operationResult, validationResult, false);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			if (list3.Any())
			{
				operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list3) > 0);
				if (!operationResult.Success)
				{
					operationResult.Message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.BD, "NewContactSaveFailed", "The new contact:{0} save failed!"), empty);
					return operationResult;
				}
			}
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			list2.AddRange(IVReceiveRepository.ImportReceiveList(ctx, list, ref empty2, ref empty3, ref empty4));
			OperationResult operationResult2 = ResultHelper.ToOperationResult(list);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			if (!string.IsNullOrWhiteSpace(empty2))
			{
				operationResult.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = empty2
				});
				return operationResult;
			}
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			operationResult.Tag = javaScriptSerializer.Serialize(new
			{
				NewContact = empty,
				StartDate = empty3,
				EndDate = empty4
			});
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list2) > 0);
			return operationResult;
		}

		private void SetReceiveType(List<IVReceiveModel> list)
		{
			foreach (IVReceiveModel item in list)
			{
				string mContactType = item.MContactType;
				if (!(mContactType == "Customer"))
				{
					if (mContactType == "Other")
					{
						item.MType = "Receive_Other";
					}
				}
				else
				{
					item.MType = "Receive_Sale";
				}
			}
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, string bizType, bool isFromExcel = false)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string empty = string.Empty;
			bool flag = bizType == "Pay_Purchase";
			list.Add(new IOTemplateConfigModel("MContactType", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.IV, "ContactType", "Contact Type", null)
			}), true, true, 2, null));
			list.Add(new IOTemplateConfigModel("MContactID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.Common, "Contact", "联系人", null)
			}), false, true, 2, null));
			if (flag)
			{
				list.Add(new IOTemplateConfigModel("MEmployeeID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "employee", "Employee", null)
				}), false, true, 2, null));
			}
			list.Add(new IOTemplateConfigModel("MReference", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.Common, "Reference", "Reference", null)
			}), false, true, 2, null));
			list.Add(new IOTemplateConfigModel("MBizDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.Common, "Date", "Date", empty)
			}), IOTemplateFieldType.Date, true, true, 2, null));
			list.AddRange(new List<IOTemplateConfigModel>
			{
				new IOTemplateConfigModel("MCyID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "Currency", "Currency", null)
				}), true, true, 2, null),
				new IOTemplateConfigModel("MTaxTypeID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "TaxType", "Tax Type", null)
				}), true, true, 2, null),
				new IOTemplateConfigModel("MItemID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "Item", "Item", null)
				}), false, false, 2, null)
			});
			if (flag)
			{
				list.Add(new IOTemplateConfigModel("MExpItemID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "ExpenseItem", "Expense Item", null)
				}), false, false, 2, null));
			}
			list.AddRange(new List<IOTemplateConfigModel>
			{
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
				}), IOTemplateFieldType.Decimal, true, false, 8, null),
				new IOTemplateConfigModel("MDiscount", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "Discount", "Discount", "(%)")
				}), IOTemplateFieldType.Decimal, false, false, 6, null),
				new IOTemplateConfigModel("MTaxID", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "TaxRate", "Tax Rate", null)
				}), false, false, 2, null)
			});
			List<BDTrackModel> trackNameList = _track.GetTrackNameList(ctx, false);
			IEnumerable<IGrouping<string, BDTrackModel>> enumerable = from f in trackNameList
			group f by f.MItemID;
			int num = 0;
			Dictionary<string, string> allText = COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.BD, "Tracking", "Tracking", ":")
			});
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
			return list;
		}

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, IVImportTransactionFilterModel param, bool isFromExcel = false, Dictionary<string, string[]> exampleDataList = null)
		{
			bool flag = param.BizType == "Pay_Purchase";
			List<BDContactsInfoModel> contactsInfo = _bdContacts.GetContactsInfo(ctx, string.Empty, string.Empty);
			contactsInfo = (flag ? (from f in contactsInfo
			where f.MIsSupplier || f.MIsOther
			select f).ToList() : (from f in contactsInfo
			where f.MIsCustomer || f.MIsOther
			select f).ToList());
			List<BASDataDictionaryModel> dictList = BASDataDictionary.GetDictList("TaxType", ctx.MLCID);
			BASCurrencyViewModel @base = _bdCurrency.GetBase(ctx, false, null, null);
			List<REGCurrencyViewModel> viewList = _bdCurrency.GetViewList(ctx, null, null, false, null);
			List<BDItemModel> listByWhere = _item.GetListByWhere(ctx, string.Empty);
			List<REGTaxRateModel> list = _taxRate.GetList(ctx, null, false);
			List<NameValueModel> trackBasicInfo = _track.GetTrackBasicInfo(ctx, null, false, false);
			BDBankAccountEditModel accountModel = _account.GetBDBankAccountEditModel(ctx, param.AccountId);
			List<ImportTemplateDataSource> list2 = new List<ImportTemplateDataSource>();
			List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
			if (flag)
			{
				list3.Add(new ImportDataSourceInfo
				{
					Key = "Supplier",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Supplier", "Supplier")
				});
				list3.Add(new ImportDataSourceInfo
				{
					Key = "Employees",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Employees", "Employees")
				});
			}
			else
			{
				list3.Add(new ImportDataSourceInfo
				{
					Key = "Customer",
					Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Customer", "Customer")
				});
			}
			list3.Add(new ImportDataSourceInfo
			{
				Key = "Other",
				Value = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Contact, "Other", "Other")
			});
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.ContactType,
				FieldList = new List<string>
				{
					"MContactType"
				},
				DataSourceList = list3
			});
			List<ImportDataSourceInfo> list4 = new List<ImportDataSourceInfo>();
			foreach (BDContactsInfoModel item2 in contactsInfo)
			{
				if (!string.IsNullOrWhiteSpace(item2.MName))
				{
					list4.Add(new ImportDataSourceInfo
					{
						Key = item2.MItemID,
						Value = item2.MName
					});
				}
			}
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.Contact,
				FieldList = new List<string>
				{
					"MContactID"
				},
				DataSourceList = list4
			});
			if (flag)
			{
				List<BDEmployeesModel> employeeList = _empBiz.GetEmployeeList(ctx, false);
				List<ImportDataSourceInfo> list5 = new List<ImportDataSourceInfo>();
				foreach (BDEmployeesModel item3 in employeeList)
				{
					if (!string.IsNullOrWhiteSpace(item3.MFullName))
					{
						list5.Add(new ImportDataSourceInfo
						{
							Key = item3.MItemID,
							Value = item3.MFullName
						});
					}
				}
				list2.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.Employee,
					FieldList = new List<string>
					{
						"MEmployeeID"
					},
					DataSourceList = list5
				});
			}
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
			List<ImportDataSourceInfo> list6 = new List<ImportDataSourceInfo>();
			if (@base != null && @base.MCurrencyID == accountModel.MCyID)
			{
				list6.Add(new ImportDataSourceInfo
				{
					Key = @base.MCurrencyID,
					Value = $"{@base.MCurrencyID} {@base.MLocalName}",
					Info = "1"
				});
			}
			else
			{
				REGCurrencyViewModel rEGCurrencyViewModel = viewList.SingleOrDefault((REGCurrencyViewModel f) => f.MCurrencyID == accountModel.MCyID);
				if (rEGCurrencyViewModel != null)
				{
					list6.Add(new ImportDataSourceInfo
					{
						Key = rEGCurrencyViewModel.MCurrencyID,
						Value = $"{rEGCurrencyViewModel.MCurrencyID} {rEGCurrencyViewModel.MName}"
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
				DataSourceList = list6
			});
			List<ImportDataSourceInfo> list7 = new List<ImportDataSourceInfo>();
			foreach (BDItemModel item4 in listByWhere)
			{
				list7.Add(new ImportDataSourceInfo
				{
					Key = item4.MItemID,
					Value = $"{((item4.MNumber != null) ? item4.MNumber.Trim() : string.Empty)}:{item4.MDesc}".RemoveLineBreaks()
				});
				string arg = (item4.MSalPrice == decimal.Zero) ? string.Empty : Math.Round(item4.MSalPrice, 4).ToString();
				ImportDataSourceInfo importDataSourceInfo = list7.SingleOrDefault((ImportDataSourceInfo f) => f.Key == item4.MItemID);
				importDataSourceInfo.Value += $"{'┇'}{arg}";
			}
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.InventoryItem,
				FieldList = new List<string>
				{
					"MItemID"
				},
				DataSourceList = list7
			});
			if (flag)
			{
				List<BDExpenseItemModel> childrenExpenseItemList = _expenseItem.GetChildrenExpenseItemList(ctx, false, false);
				List<ImportDataSourceInfo> list8 = new List<ImportDataSourceInfo>();
				foreach (BDExpenseItemModel item5 in childrenExpenseItemList)
				{
					list8.Add(new ImportDataSourceInfo
					{
						Key = item5.MItemID,
						Value = item5.MName
					});
				}
				list2.Add(new ImportTemplateDataSource(true)
				{
					FieldType = ImportTemplateColumnType.ExpenseItem,
					FieldList = new List<string>
					{
						"MExpItemID"
					},
					DataSourceList = list8
				});
			}
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
					string text = (i + 1).ToString();
					List<ImportDataSourceInfo> list9 = new List<ImportDataSourceInfo>();
					foreach (NameValueModel mChild in trackBasicInfo[i].MChildren)
					{
						if (!string.IsNullOrWhiteSpace(mChild.MValue))
						{
							list9.Add(new ImportDataSourceInfo
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
						DataSourceList = list9
					});
				}
			}
			if (exampleDataList != null)
			{
				exampleDataList.Add("MContactID", new string[1]
				{
					contactsInfo.Any() ? contactsInfo[0].MName : string.Empty
				});
				exampleDataList.Add("MTaxTypeID", new string[1]
				{
					dictList.Any() ? dictList[0].DictName : string.Empty
				});
				exampleDataList.Add("MItemID", new string[2]
				{
					listByWhere.Any() ? listByWhere[0].MNumber : string.Empty,
					(listByWhere.Count() > 1) ? listByWhere[1].MNumber : string.Empty
				});
				exampleDataList.Add("MDesc", new string[2]
				{
					listByWhere.Any() ? listByWhere[0].MDesc : string.Empty,
					(listByWhere.Count() > 1) ? listByWhere[1].MDesc : string.Empty
				});
				List<ImportDataSourceInfo> dataSourceList = list2.FirstOrDefault((ImportTemplateDataSource f) => f.FieldType == ImportTemplateColumnType.TaxRate).DataSourceList;
				string text2 = (dataSourceList != null) ? dataSourceList.Last().Value : string.Empty;
				exampleDataList.Add("MTaxID", new string[2]
				{
					text2,
					text2
				});
				List<ImportDataSourceInfo> dataSourceList2 = list2.FirstOrDefault((ImportTemplateDataSource f) => f.FieldType == ImportTemplateColumnType.Currency).DataSourceList;
				exampleDataList.Add("MCyID", new string[1]
				{
					dataSourceList2.Any() ? dataSourceList2.FirstOrDefault().Value : string.Empty
				});
			}
			return list2;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx, IVImportTransactionFilterModel param)
		{
			return GetImportTemplateModel(ctx, param, false);
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx, IVImportTransactionFilterModel param, bool isFromExcel = false)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("MTaxTypeID", "MTaxID");
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, param.BizType, false);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary2.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary3 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, param, isFromExcel, dictionary3);
			dictionary3.Add("MBizDate", new string[1]
			{
				ctx.DateNow.ToString("yyyy-MM-dd")
			});
			dictionary3.Add("MQty", new string[2]
			{
				"1.0000",
				"2.0000"
			});
			dictionary3.Add("MPrice", new string[2]
			{
				"300.00000000",
				"500.00000000"
			});
			List<string> alignRightFieldList = "MBizDate,MQty,MPrice".Split(',').ToList();
			return new ImportTemplateModel
			{
				TemplateType = param.BizType,
				ColumnList = dictionary2,
				FieldConfigList = templateConfig,
				RequiredColumnList = (from f in templateConfig
				where f.MIsRequired
				select f.MFieldName).ToList(),
				AliasColumnList = dictionary,
				TemplateDictionaryList = templateBasicData,
				ExampleDataList = dictionary3,
				LocaleID = ctx.MLCID,
				AlignRightFieldList = alignRightFieldList
			};
		}

		public DataGridJson<IVReceiveModel> Get(MContext ctx, GetParam param)
		{
			throw new NotImplementedException();
		}

		public List<IVReceiveModel> Post(MContext ctx, PostParam<IVReceiveModel> param)
		{
			throw new NotImplementedException();
		}

		public IVReceiveModel Delete(MContext ctx, DeleteParam param)
		{
			throw new NotImplementedException();
		}
	}
}
