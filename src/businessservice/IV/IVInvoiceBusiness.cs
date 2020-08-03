using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.BusinessService.BD;
using JieNor.Megi.BusinessService.REG;
using JieNor.Megi.Common.Utility;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Helper;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.Core.Utility;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.COM;
using JieNor.Megi.DataModel.IO;
using JieNor.Megi.DataModel.IO.Import;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Invoice;
using JieNor.Megi.DataModel.REG;
using JieNor.Megi.DataRepository.API;
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
using System.Text;
using System.Web.Script.Serialization;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVInvoiceBusiness : BusinessServiceBase, IIVInvoiceBusiness, IDataContract<IVInvoiceModel>
	{
		private BDContactsRepository _bdContacts = new BDContactsRepository();

		private REGCurrencyRepository _bdCurrency = new REGCurrencyRepository();

		private BDItemRepository _item = new BDItemRepository();

		private REGTaxRateRepository _taxRate = new REGTaxRateRepository();

		private BDTrackRepository _track = new BDTrackRepository();

		private BDItemBusiness _itemBiz = new BDItemBusiness();

		private BDContactsBusiness _contactBiz = new BDContactsBusiness();

		private REGTaxRateBusiness _taxRateBiz = new REGTaxRateBusiness();

		private BDTrackBusiness _trackBiz = new BDTrackBusiness();

		private REGCurrencyBusiness _currencyBiz = new REGCurrencyBusiness();

		private BASDataDictionaryBusiness _dictBiz = new BASDataDictionaryBusiness();

		private BDAccountBusiness _acctBiz = new BDAccountBusiness();

		public IVInvoiceRepository dal = new IVInvoiceRepository();

		public OperationResult UpdateRepeatInvoiceMessage(MContext ctx, IVInvoiceEmailSendModel model)
		{
			return IVInvoiceRepository.UpdateRepeatInvoiceMessage(ctx, model);
		}

		public IVRepeatInvoiceModel GetRepeatInvoiceEditModel(MContext ctx, string pkID)
		{
			return IVInvoiceRepository.GetRepeatInvoiceEditModel(ctx, pkID);
		}

		public IVRepeatInvoiceModel GetRepeatInvoiceCopyModel(MContext ctx, string pkID)
		{
			return IVInvoiceRepository.GetRepeatInvoiceCopyModel(ctx, pkID);
		}

		public OperationResult UpdateRepeatInvoice(MContext ctx, IVRepeatInvoiceModel model)
		{
			BDTrackRepository bDTrackRepository = new BDTrackRepository();
			List<NameValueModel> trackBasicInfo = bDTrackRepository.GetTrackBasicInfo(ctx, null, false, true);
			if (trackBasicInfo != null && trackBasicInfo.Count > 0 && model.RepeatInvoiceEntry != null)
			{
				foreach (IVRepeatInvoiceEntryModel item in model.RepeatInvoiceEntry)
				{
					item.MTrackItem1 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem1, 1);
					item.MTrackItem2 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem2, 2);
					item.MTrackItem3 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem3, 3);
					item.MTrackItem4 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem4, 4);
					item.MTrackItem5 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem5, 5);
				}
			}
			return IVInvoiceRepository.UpdateRepeatInvoice(ctx, model);
		}

		public OperationResult UpdateRepeatInvoiceStatus(MContext ctx, ParamBase param)
		{
			IVInvoiceRepository.UpdateRepeatInvoiceStatus(ctx, param);
			return new OperationResult
			{
				Success = true
			};
		}

		public OperationResult DeleteRepeatInvoiceList(MContext ctx, ParamBase param)
		{
			return IVInvoiceRepository.DeleteRepeatInvoiceList(ctx, param);
		}

		public OperationResult AddRepeatInvoiceNoteLog(MContext ctx, IVRepeatInvoiceModel model)
		{
			return IVInvoiceRepository.AddRepeatInvoiceNoteLog(ctx, model);
		}

		public OperationResult UpdateRepeatInvoiceExpectedInfo(MContext ctx, IVRepeatInvoiceModel model)
		{
			IVInvoiceRepository.UpdateRepeatInvoiceExpectedInfo(ctx, model);
			return new OperationResult
			{
				Success = true
			};
		}

		public DataGridJson<IVRepeatInvoiceListModel> GetRepeatInvoiceList(MContext ctx, IVInvoiceListFilterModel filter)
		{
			DataGridJson<IVRepeatInvoiceListModel> dataGridJson = new DataGridJson<IVRepeatInvoiceListModel>();
			dataGridJson.rows = IVInvoiceRepository.GetRepeatInvoiceList(ctx, filter);
			dataGridJson.total = IVInvoiceRepository.GetRepeatInvoiceTotalCount(ctx, filter);
			return dataGridJson;
		}

		public IVGoldenTaxInvoiceModel GetGoldenTaxInvoiceEditModel(MContext ctx, string pkID)
		{
			return IVInvoiceRepository.GetGoldenTaxInvoiceEditModel(ctx, pkID);
		}

		public OperationResult UpdateGoldenTaxInvoice(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			return IVInvoiceRepository.UpdateGoldenTaxInvoice(ctx, model);
		}

		public OperationResult UpdateGoldenTaxCourierInfo(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			return IVInvoiceRepository.UpdateGoldenTaxCourierInfo(ctx, model);
		}

		public OperationResult DeleteGoldenTaxInvoiceList(MContext ctx, ParamBase param)
		{
			return IVInvoiceRepository.DeleteGoldenTaxInvoiceList(ctx, param);
		}

		public OperationResult ArchiveGoldenTaxInvoiceList(MContext ctx, ParamBase param)
		{
			return IVInvoiceRepository.ArchiveGoldenTaxInvoiceList(ctx, param);
		}

		public OperationResult UpdateGoldenTaxInvoicePrintStatusList(MContext ctx, ParamBase param, bool isPrint)
		{
			return IVInvoiceRepository.UpdateGoldenTaxInvoicePrintStatusList(ctx, param, isPrint);
		}

		public OperationResult AddGoldenTaxInvoiceNoteLog(MContext ctx, IVGoldenTaxInvoiceModel model)
		{
			return IVInvoiceRepository.AddGoldenTaxInvoiceNoteLog(ctx, model);
		}

		public DataGridJson<IVGoldenTaxInvoiceListModel> GetGoldenTaxInvoiceList(MContext ctx, IVGoldenTaxInvoiceListFilterModel filter)
		{
			DataGridJson<IVGoldenTaxInvoiceListModel> dataGridJson = new DataGridJson<IVGoldenTaxInvoiceListModel>();
			dataGridJson.rows = IVInvoiceRepository.GetGoldenTaxInvoiceList(ctx, filter);
			dataGridJson.total = IVInvoiceRepository.GetGoldenTaxInvoiceTotalCount(ctx, filter);
			return dataGridJson;
		}

		public OperationResult AddInvoiceNoteLog(MContext ctx, IVInvoiceModel model)
		{
			return IVInvoiceRepository.AddInvoiceNoteLog(ctx, model);
		}

		public IVInvoiceSummaryModel GetInvoiceSummaryModel(MContext ctx, string type, DateTime startDate, DateTime endDate)
		{
			return IVInvoiceRepository.GetInvoiceSummaryModel(ctx, type, startDate, endDate);
		}

		public IVContactInvoiceSummaryModel GetInvoiceSummaryModelByContact(MContext ctx, string contactId)
		{
			return IVInvoiceRepository.GetInvoiceSummaryModelByContact(ctx, contactId);
		}

		public DataGridJson<IVInvoiceListModel> GetInvoiceList(MContext ctx, IVInvoiceListFilterModel filter)
		{
			filter.Keyword = filter.Keyword;
			DataGridJson<IVInvoiceListModel> dataGridJson = new DataGridJson<IVInvoiceListModel>();
			dataGridJson.rows = IVInvoiceRepository.GetInvoiceList(ctx, filter);
			dataGridJson.total = IVInvoiceRepository.GetInvoiceTotalCount(ctx, filter);
			return dataGridJson;
		}

		public List<NameValueModel> GetFPInvoiceSummary(MContext ctx, IVInvoiceListFilterModel filter)
		{
			return IVInvoiceRepository.GetFPInvoiceSummary(ctx, filter);
		}

		public List<IVInvoiceListModel> GetInvoiceListByFilter(MContext ctx, IVInvoiceListFilterModel filter)
		{
			return IVInvoiceRepository.GetInvoiceList(ctx, filter);
		}

		public List<IVInvoiceModel> GetInvoiceListForPrint(MContext ctx, IVInvoiceListFilterModel filter)
		{
			List<IVInvoiceModel> list = new List<IVInvoiceModel>();
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			List<BDItemModel> dataModelList = ModelInfoManager.GetDataModelList<BDItemModel>(ctx, new SqlWhere(), false, true);
			if (!string.IsNullOrWhiteSpace(filter.SelectedIds))
			{
				SqlWhere filter2 = new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MID", filter.SelectedIds.Split(',').ToList());
				list = IVInvoiceRepository.GetInvoiceListIncludeEntry(ctx, filter2);
				IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
				iVVerificationListFilterModel.MBillID = filter.SelectedIds;
				iVVerificationListFilterModel.MBizBillType = "Invoice";
				iVVerificationListFilterModel.MViewVerif = true;
				List<IVVerificationListModel> historyVerifData = IVVerificationRepository.GetHistoryVerifData(ctx, iVVerificationListFilterModel);
				foreach (IVInvoiceModel item in list)
				{
					item.Verification = (from f in historyVerifData
					where f.MSourceBillID == item.MID
					select f).ToList();
					foreach (IVInvoiceEntryModel item2 in item.InvoiceEntry)
					{
						BDItemModel bDItemModel = dataModelList.FirstOrDefault((BDItemModel f) => f.MItemID == item2.MItemID);
						if (bDItemModel != null)
						{
							List<MultiLanguageField> mMultiLanguageField = bDItemModel.MultiLanguage.FirstOrDefault((MultiLanguageFieldList f) => f.MFieldName == "MDesc").MMultiLanguageField;
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

		public List<IVInvoiceModel> GetInvoiceListForExport(MContext ctx, IVInvoiceListFilterModel filter)
		{
			List<IVInvoiceListModel> invoiceList = IVInvoiceRepository.GetInvoiceList(ctx, filter);
			if (!invoiceList.Any())
			{
				return new List<IVInvoiceModel>();
			}
			SqlWhere filter2 = new SqlWhere().Equal("MOrgID", ctx.MOrgID).In("MID", (from f in invoiceList
			select f.MID).ToList());
			List<IVInvoiceModel> invoiceListIncludeEntry = IVInvoiceRepository.GetInvoiceListIncludeEntry(ctx, filter2);
			foreach (IVInvoiceModel item in invoiceListIncludeEntry)
			{
				item.IsCalculateMainAmount = false;
				IVInvoiceListModel iVInvoiceListModel = invoiceList.SingleOrDefault((IVInvoiceListModel f) => f.MID == item.MID);
				item.MVerifyAmt = Convert.ToDecimal(iVInvoiceListModel.MVerifyAmt);
			}
			return invoiceListIncludeEntry;
		}

		public List<IVInvoiceModel> GetInvoiceListIncludeEntry(MContext ctx, IVInvoiceListFilterModel filter)
		{
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.Equal("MIsDelete", 0);
			SqlWhere sqlWhere2 = sqlWhere;
			DateTime dateTime = ctx.MBeginDate;
			sqlWhere2.LessThen("MBizDate", dateTime.ToString("yyyy-MM-dd"));
			if (filter.MEndDate.HasValue)
			{
				SqlWhere sqlWhere3 = sqlWhere;
				dateTime = filter.MEndDate.Value;
				sqlWhere3.LessOrEqual("MBizDate", dateTime.ToString("yyyy-MM-dd"));
			}
			if (filter.MStartDate.HasValue)
			{
				SqlWhere sqlWhere4 = sqlWhere;
				dateTime = filter.MStartDate.Value;
				sqlWhere4.GreaterOrEqual("MBizDate", dateTime.ToString("yyyy-MM-dd"));
			}
			sqlWhere.Equal("MStatus", filter.MStatus);
			if (!string.IsNullOrWhiteSpace(filter.MContactID))
			{
				List<string> values = filter.MContactID.Split(',').ToList();
				sqlWhere.In("MContactID", values);
			}
			List<IVInvoiceModel> invoiceListIncludeEntry = IVInvoiceRepository.GetInvoiceListIncludeEntry(ctx, filter);
			if (invoiceListIncludeEntry != null && invoiceListIncludeEntry.Count() > 0 && string.IsNullOrWhiteSpace(filter.MTrackItem1) && string.IsNullOrWhiteSpace(filter.MTrackItem2) && string.IsNullOrWhiteSpace(filter.MTrackItem3) && string.IsNullOrWhiteSpace(filter.MTrackItem4) && string.IsNullOrWhiteSpace(filter.MTrackItem5))
			{
				return invoiceListIncludeEntry;
			}
			return invoiceListIncludeEntry;
		}

		public OperationResult UpdateInvoice(MContext ctx, IVInvoiceModel model)
		{
			OperationResult operationResult = new OperationResult();
			if (model.InvoiceEntry != null && model.InvoiceEntry.Any((IVInvoiceEntryModel invoice) => invoice.MTaxAmtFor > invoice.MAmountFor))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "CannotGreaterAmount", "The tax amount cannot be greater than the total amount");
				return operationResult;
			}
			IVInvoiceModel iVInvoiceModel = null;
			if (!string.IsNullOrEmpty(model.MID))
			{
				iVInvoiceModel = IVInvoiceRepository.GetInvoiceModel(ctx, model.MID);
				if ((iVInvoiceModel == null || iVInvoiceModel.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment)) && iVInvoiceModel.MBizDate >= ctx.MBeginDate)
				{
					return new OperationResult
					{
						Success = false,
						Code = "1000",
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataHasBeenChanged", "Data has benn changed！")
					};
				}
			}
			if (string.IsNullOrEmpty(model.MCyID))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "CnyCannotEmpty", "Currency can't be empty!");
				return operationResult;
			}
			if (iVInvoiceModel == null || iVInvoiceModel.MCyID != model.MCyID)
			{
				operationResult = REGCurrencyRepository.CheckCurrency(ctx, model.MCyID, model.MBizDate);
				if (!operationResult.Success)
				{
					return operationResult;
				}
			}
			if (!IVVerificationRepository.CheckIsCanEditOrDelete(ctx, "Invoice", model.MID))
			{
				return new OperationResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "IVCannotEdit", "Approval data can't be edit.")
				};
			}
			if (IVInvoiceRepository.CheckInvoiceNumberIsExist(ctx, model.MType, ctx.MOrgID, model.MID, model.MNumber))
			{
				operationResult.Success = false;
				operationResult.Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "IVNumberIsExist", "单据编号已经存在，请使用其他编号");
				return operationResult;
			}
			operationResult = GLInterfaceRepository.IsPeriodUnclosed(ctx, model.MBizDate);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			BDTrackRepository bDTrackRepository = new BDTrackRepository();
			List<NameValueModel> trackBasicInfo = bDTrackRepository.GetTrackBasicInfo(ctx, null, false, true);
			if (trackBasicInfo != null && trackBasicInfo.Count > 0)
			{
				foreach (IVInvoiceEntryModel item in model.InvoiceEntry)
				{
					item.MTrackItem1 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem1, 1);
					item.MTrackItem2 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem2, 2);
					item.MTrackItem3 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem3, 3);
					item.MTrackItem4 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem4, 4);
					item.MTrackItem5 = bDTrackRepository.GetBillTrackItemID(trackBasicInfo, item.MTrackItem5, 5);
				}
			}
			return IVInvoiceRepository.UpdateInvoice(ctx, model);
		}

		public OperationResult UpdateInvoiceStatus(MContext ctx, ParamBase param)
		{
			return IVInvoiceRepository.UpdateInvoiceStatus(ctx, param);
		}

		public OperationResult UnApproveInvoice(MContext ctx, string invoiceId)
		{
			IVInvoiceModel invoiceModel = IVInvoiceRepository.GetInvoiceModel(ctx, invoiceId);
			if (invoiceModel == null || string.IsNullOrEmpty(invoiceModel.MID) || Math.Abs(invoiceModel.MVerifyAmtFor) > decimal.Zero)
			{
				return new OperationResult
				{
					Success = false,
					Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DataHadBeenUpdate", "Data had been updated.")
				};
			}
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, invoiceId);
			if (invoiceModel.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) && !operationResult.Success)
			{
				return operationResult;
			}
			return IVInvoiceRepository.UnApproveInvoice(ctx, invoiceModel);
		}

		public OperationResult ApproveInvoice(MContext ctx, ParamBase param)
		{
			return IVInvoiceRepository.ApproveInvoice(ctx, param);
		}

		public OperationResult UpdateInvoiceExpectedInfo(MContext ctx, IVInvoiceModel model)
		{
			IVInvoiceRepository.UpdateInvoiceExpectedInfo(ctx, model);
			return new OperationResult
			{
				Success = true
			};
		}

		public IVInvoiceModel GetInvoiceEditModel(MContext ctx, string pkID, string bizType)
		{
			string bizObjKey = "Invoice_Sales";
			if (bizType.ToLower() == "Invoice_Purchase".ToLower() || bizType.ToLower() == "Invoice_Purchase_Red".ToLower())
			{
				bizObjKey = "Invoice_Purchases";
			}
			IVInvoiceModel invoiceEditModel = IVInvoiceRepository.GetInvoiceEditModel(ctx, pkID, bizType);
			if (!string.IsNullOrEmpty(invoiceEditModel.MID))
			{
				IVVerificationListFilterModel iVVerificationListFilterModel = new IVVerificationListFilterModel();
				iVVerificationListFilterModel.MBillID = invoiceEditModel.MID;
				iVVerificationListFilterModel.MBizBillType = "Invoice";
				iVVerificationListFilterModel.MBizType = invoiceEditModel.MType;
				iVVerificationListFilterModel.MContactID = invoiceEditModel.MContactID;
				iVVerificationListFilterModel.MCurrencyID = invoiceEditModel.MCyID;
				iVVerificationListFilterModel.MViewVerif = true;
				invoiceEditModel.Verification = IVVerificationRepository.GetHistoryVerifData(ctx, iVVerificationListFilterModel);
			}
			invoiceEditModel.MActionPermission = GetActionPermissionModel(ctx, invoiceEditModel, bizObjKey);
			if (invoiceEditModel.MActionPermission.MIsCanApprove)
			{
				IVInvoiceModel nextUnApproveInvoiceID = IVInvoiceRepository.GetNextUnApproveInvoiceID(ctx, invoiceEditModel.MCreateDate, bizType);
				if (nextUnApproveInvoiceID != null)
				{
					invoiceEditModel.MNextID = nextUnApproveInvoiceID.MID;
					invoiceEditModel.MNextType = nextUnApproveInvoiceID.MType;
				}
			}
			return invoiceEditModel;
		}

		private IVInvoicePermissionModel GetActionPermissionModel(MContext ctx, IVInvoiceModel model, string bizObjKey)
		{
			IVInvoicePermissionModel iVInvoicePermissionModel = new IVInvoicePermissionModel();
			IVVerificationBusiness iVVerificationBusiness = new IVVerificationBusiness();
			List<IVVerificationInforModel> customerWaitForVerificationInfor = iVVerificationBusiness.GetCustomerWaitForVerificationInfor(ctx, model.MID, "Invoice");
			if (!base.HavePermission(ctx, bizObjKey, "Change"))
			{
				iVInvoicePermissionModel.MHaveAction = false;
				if (!string.IsNullOrEmpty(model.MID) && model.MStatus == Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) && Math.Abs(model.MTaxTotalAmtFor) > Math.Abs(model.MVerificationAmt) && base.HavePermission(ctx, "BankAccount", "Change"))
				{
					iVInvoicePermissionModel.MIsCanPay = true;
					if (customerWaitForVerificationInfor != null && customerWaitForVerificationInfor.Count > 0)
					{
						iVInvoicePermissionModel.MIsCanVerification = true;
					}
				}
				return iVInvoicePermissionModel;
			}
			if (string.IsNullOrEmpty(model.MID))
			{
				iVInvoicePermissionModel.MHaveAction = false;
				iVInvoicePermissionModel.MIsCanEdit = true;
				iVInvoicePermissionModel.MIsInitCanEdit = true;
				if (base.HavePermission(ctx, bizObjKey, "Approve"))
				{
					iVInvoicePermissionModel.MIsCanApprove = true;
				}
				return iVInvoicePermissionModel;
			}
			iVInvoicePermissionModel.MHaveAction = true;
			if (model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MTaxTotalAmtFor) == Math.Abs(model.MVerificationAmt) || !base.HavePermission(ctx, "BankAccount", "Change"))
			{
				iVInvoicePermissionModel.MIsCanPay = false;
			}
			if (model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MTaxTotalAmtFor) == Math.Abs(model.MVerificationAmt) || !base.HavePermission(ctx, "BankAccount", "Change") || customerWaitForVerificationInfor == null || customerWaitForVerificationInfor.Count == 0)
			{
				iVInvoicePermissionModel.MIsCanVerification = false;
			}
			if (model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MTaxTotalAmtFor) == Math.Abs(model.MVerificationAmt) || !base.HavePermission(ctx, bizObjKey, "Approve"))
			{
				iVInvoicePermissionModel.MIsCanCreateCreditNote = false;
			}
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MVerificationAmt) > decimal.Zero || model.MBizDate < ctx.MBeginDate || !base.HavePermission(ctx, bizObjKey, "Approve") || !operationResult.Success)
			{
				iVInvoicePermissionModel.MIsCanApprove = false;
			}
			if (model.MStatus != Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MVerificationAmt) > decimal.Zero || model.MBizDate < ctx.MBeginDate || !base.HavePermission(ctx, bizObjKey, "Approve") || !operationResult.Success)
			{
				iVInvoicePermissionModel.MIsCanUnApprove = false;
			}
			if (!base.HavePermission(ctx, "General_Ledger", "View") || !GLInterfaceRepository.IsBillCreatedVoucher(ctx, model.MID))
			{
				iVInvoicePermissionModel.MIsCanViewVoucherCreateDetail = false;
			}
			if (Math.Abs(model.MVerificationAmt) > decimal.Zero || model.MBizDate < ctx.MBeginDate || model.MStatus >= 3)
			{
				iVInvoicePermissionModel.MIsCanVoid = false;
			}
			if (model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment) || Math.Abs(model.MVerificationAmt) > decimal.Zero || model.MBizDate < ctx.MBeginDate)
			{
				iVInvoicePermissionModel.MIsCanDelete = false;
			}
			if (model.MStatus >= Convert.ToInt32(IVInvoiceStatusEnum.WaitingPayment))
			{
				iVInvoicePermissionModel.MIsCanEdit = false;
			}
			if (ctx.MInitBalanceOver || Math.Abs(model.MVerificationAmt) > decimal.Zero || !operationResult.Success)
			{
				iVInvoicePermissionModel.MIsInitCanEdit = false;
			}
			return iVInvoicePermissionModel;
		}

		public IVInvoiceModel GetInvoiceCopyModel(MContext ctx, string pkID, bool isCopyCredit)
		{
			IVInvoiceModel invoiceCopyModel = IVInvoiceRepository.GetInvoiceCopyModel(ctx, pkID, isCopyCredit);
			string bizObjKey = "Invoice_Sales";
			if (invoiceCopyModel.MType.ToLower() == "Invoice_Purchase".ToLower() || invoiceCopyModel.MType.ToLower() == "Invoice_Purchase_Red".ToLower())
			{
				bizObjKey = "Invoice_Purchases";
			}
			invoiceCopyModel.MActionPermission = GetActionPermissionModel(ctx, invoiceCopyModel, bizObjKey);
			return invoiceCopyModel;
		}

		public OperationResult DeleteInvoiceList(MContext ctx, ParamBase param)
		{
			string[] array = param.KeyIDSWithNoSingleQuote.Split(',');
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = IVVerificationRepository.CheckIsCanEditOrDelete(ctx, "Invoice", array.ToList());
			num = array.Length - list.Count;
			OperationResult operationResult = new OperationResult();
			string text = string.Join(",", list);
			if (text != "")
			{
				param.KeyIDs = text;
				operationResult = IVInvoiceRepository.DeleteInvoiceList(ctx, param);
				if (!operationResult.Success)
				{
					return operationResult;
				}
			}
			else
			{
				operationResult.Success = false;
				operationResult.Message = num.ToString();
			}
			return operationResult;
		}

		public OperationResult PayToInvoice(MContext ctx, IVMakePaymentModel model)
		{
			model.MObjectType = "Invoice";
			IVInvoiceMakePaymentRepository iVInvoiceMakePaymentRepository = new IVInvoiceMakePaymentRepository();
			return iVInvoiceMakePaymentRepository.ToPay(ctx, model);
		}

		public string GetChartStackedDictionary(MContext ctx, string Type, DateTime startDate, DateTime endDate, string contactId = null)
		{
			return IVInvoiceRepository.GetChartStackedDictionary(ctx, Type, startDate, endDate, contactId);
		}

		public List<ChartPie2DModel> GetChartPieDictionary(MContext ctx, string Type, DateTime startDate, DateTime endDate)
		{
			return IVInvoiceRepository.GetChartPieDictionary(ctx, Type, startDate, endDate);
		}

		public List<IVStatementsModel> GetStatementData(MContext ctx, IVStatementListFilterModel param)
		{
			return IVInvoiceRepository.GetStatementData(ctx, param);
		}

		public List<IVViewStatementModel> GetViewStatementData(MContext ctx, string contactID, string type, DateTime BeginDate, DateTime EndDate)
		{
			return IVInvoiceRepository.GetViewStatementData(ctx, contactID, type, BeginDate, EndDate);
		}

		public List<IVViewStatementModel> GetViewStatementOpeningBalanceDate(MContext ctx, string statementType, string contactId, DateTime beginDate)
		{
			return IVInvoiceRepository.GetViewStatementOpeningBalanceDate(ctx, statementType, contactId, beginDate);
		}

		public string GetOverPastDictionary(MContext ctx, string contactID)
		{
			return IVInvoiceRepository.GetOverPastDictionary(ctx, contactID);
		}

		public void CheckInvoiceNumberExist<T>(MContext ctx, List<T> list, List<IOValidationResultModel> validationResult)
		{
			List<string> numberList = (from f in list
			where !string.IsNullOrWhiteSpace(ModelHelper.GetModelValue<T>(f, "MNumber"))
			select ModelHelper.GetModelValue<T>(f, "MNumber")).ToList();
			List<string> existInvoiceNumberList = IVInvoiceRepository.GetExistInvoiceNumberList(ctx, numberList);
			if (existInvoiceNumberList.Any())
			{
				foreach (string item in existInvoiceNumberList)
				{
					IEnumerable<T> enumerable = from f in list
					where ModelHelper.GetModelValue<T>(f, "MNumber") == item
					select f;
					foreach (T item2 in enumerable)
					{
						int rowIndex = 0;
						int.TryParse(ModelHelper.TryGetModelValue(item2, "MRowIndex"), out rowIndex);
						validationResult.Add(new IOValidationResultModel
						{
							FieldType = IOValidationTypeEnum.InvoiceNumber,
							FieldValue = item,
							RowIndex = rowIndex,
							Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DuplicateInvoiceNo", "导入的销售单号:{0}已存在!")
						});
					}
				}
			}
		}

		public void CheckInvoicesRequiredField(MContext ctx, List<IVInvoiceModel> list)
		{
			GLDataPool instance = GLDataPool.GetInstance(ctx, false, 0, 0, 0);
			if (ctx.IsSys)
			{
				ctx.IsSys = false;
			}
			List<BDTrackModel> trackList = (from f in ModelInfoManager.GetDataModelList<BDTrackModel>(ctx, new SqlWhere(), false, true)
			orderby f.MCreateDate
			select f).ToList();
			List<BDContactsModel> contactList = instance.ContactList;
			List<REGTaxRateModel> taxRateList = instance.TaxRateList;
			List<REGCurrencyViewModel> currencyListByName = _currencyBiz.GetCurrencyListByName(ctx, true, false);
			List<REGCurrencyViewModel> allCurrencyList = _currencyBiz.GetAllCurrencyList(ctx, false, false);
			List<BDItemModel> itemListIgnoreLocale = _item.GetItemListIgnoreLocale(ctx, true);
			List<CommandInfo> list2 = APIDataRepository.FillBaseData(ctx, list, true, null);
			foreach (IVInvoiceModel item in list)
			{
				List<ValidationError> list3 = item.ValidationErrors ?? new List<ValidationError>();
				bool flag = false;
				BDContactsInfoModel contactModel = item.MContactInfo;
				if (contactModel == null)
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ContactIsNull", "联系人为空!")));
				}
				else
				{
					bool flag2 = true;
					if (string.IsNullOrWhiteSpace(contactModel.MItemID) || contactModel.IsNew)
					{
						if (!_contactBiz.ValidateContactRequiredFields(ctx, contactModel, list3))
						{
							flag2 = false;
							contactModel.MItemID = null;
						}
					}
					else
					{
						BDContactsModel model = contactList.FirstOrDefault((BDContactsModel f) => f.MItemID == contactModel.MItemID);
						if (COMValidateHelper.CheckBasicData(ctx, model, list3, true))
						{
							item.MContactID = contactModel.MItemID;
						}
					}
					if (flag2)
					{
						switch (item.MType)
						{
						case "Invoice_Sale":
						case "Invoice_Sale_Red":
							if (!contactModel.MIsCustomer)
							{
								list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "SaleContactNotCustomer", "销售单的联系人必须是客户!")));
							}
							break;
						case "Invoice_Purchase":
						case "Invoice_Purchase_Red":
							if (!contactModel.MIsSupplier)
							{
								list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "BillContactNotSupplier", "采购单的联系人必须是供应商!")));
							}
							break;
						}
					}
				}
				if (string.IsNullOrWhiteSpace(item.MType))
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceTypeIsNull", "业务单据类型不能为空!")));
				}
				else if (item.MType != "Invoice_Sale" && item.MType != "Invoice_Purchase" && item.MType != "Invoice_Sale_Red" && item.MType != "Invoice_Purchase_Red")
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceTypeIsError", "业务单据类型错误!")));
				}
				if (item.MBizDate == DateTime.MinValue)
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "BizDateIsNull", "业务单据日期为空!")));
				}
				if (item.MDueDate == DateTime.MinValue && (item.MType == "Invoice_Sale" || item.MType == "Invoice_Purchase"))
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DueDateIsNull", "到期日为空!")));
				}
				if (item.MDueDate < item.MBizDate && (item.MType == "Invoice_Sale" || item.MType == "Invoice_Purchase"))
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DueDateSmallBizDate", "到期日小于业务单据日期!")));
				}
				if (item.MExpectedDate != DateTime.MinValue && item.MExpectedDate < item.MBizDate && item.MType == "Invoice_Sale")
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "ReceiptDateGreaterThanDate", "预计收款日期必须大于或等于单据日期!")));
				}
				if (string.IsNullOrWhiteSpace(item.MCyID))
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "CyIDIsNull", "币别不能为空!")));
				}
				else
				{
					REGCurrencyViewModel model2 = currencyListByName.FirstOrDefault((REGCurrencyViewModel f) => f.MCurrencyID.EqualsIgnoreCase(item.MCyID));
					if (COMValidateHelper.CheckBasicData(ctx, model2, list3, true))
					{
						string text = base.CheckCurrencyExchangeRate(ctx, allCurrencyList, item.MCyID, item.MBizDate, true);
						if (!string.IsNullOrWhiteSpace(text))
						{
							list3.Add(new ValidationError(text));
						}
					}
				}
				if (string.IsNullOrWhiteSpace(item.MTaxType))
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxTypeIsNull", "价税类型不能为空!")));
				}
				else if (item.MTaxType != "Tax_Inclusive" && item.MTaxType != "Tax_Exclusive" && item.MTaxType != "No_Tax")
				{
					string message = string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataNotExist", "{0}不存在!"), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxType", "Tax Type"));
					list3.Add(new ValidationError(message));
				}
				if (!string.IsNullOrWhiteSpace(item.MID) && (item.MStatus == 4 || item.MStatus == 3))
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceStatusIsNotDraftUpdateFail", "单据状态不为草稿状态，不可以修改!")));
				}
				List<IVInvoiceEntryModel> invoiceEntry = item.InvoiceEntry;
				if (invoiceEntry == null || !invoiceEntry.Any())
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceEntryIsNull", "单据分录为空!")));
				}
				else
				{
					if (item.MTotalAmt == decimal.Zero)
					{
						list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceAmountIsZero", "单据总价不能为零!")));
					}
					string text2 = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "FieldCannotBeNegativeNumber", "{0}不能为负数!");
					foreach (IVInvoiceEntryModel item2 in invoiceEntry)
					{
						List<ValidationError> list4 = item2.ValidationErrors ?? new List<ValidationError>();
						if (!string.IsNullOrWhiteSpace(item2.MItemID))
						{
							COMValidateHelper.CheckBasicData(ctx, itemListIgnoreLocale.FirstOrDefault((BDItemModel f) => f.MItemID == item2.MItemID), list4, true);
						}
						if (string.IsNullOrWhiteSpace(item2.MDesc))
						{
							list4.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceEntryDescHasNull", "分录的描述存在为空的值!")));
						}
						if (item2.MQty < decimal.Zero)
						{
							list4.Add(new ValidationError(string.Format(text2, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "Quantity", "Quantity"))));
						}
						if (item2.MDiscount < decimal.Zero || item2.MDiscount > 100m)
						{
							list4.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DiscountInvoid", "折扣率必须大于等于零且小于等于100。")));
						}
						if (item2.MPrice < decimal.Zero)
						{
							list4.Add(new ValidationError(string.Format(text2, COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "UnitPrice", "Unit Price"))));
						}
						if (item2.MAmountFor != item2.MLineAmountFor)
						{
							list4.Add(new ValidationError(COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.IV, "LineTotalAmountForNotMatch", "The line TotalAmountFor {0} does not match the expected line TotalAmountFor {1}", item2.MLineAmountFor.To2Decimal(), item2.MAmountFor.To2Decimal())));
						}
						if (string.IsNullOrWhiteSpace(item2.MTaxID))
						{
							if (item.MTaxID != "No_Tax")
							{
								list4.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "InvoiceEntryTaxIdHasNull", "分录的税率存在为空的值!")));
							}
						}
						else if (!taxRateList.Exists((REGTaxRateModel f) => f.MItemID == item2.MTaxID))
						{
							list4.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "TaxRateNotExist", "税率不存在!")));
						}
						COMValidateHelper.CheckTrackValue(ctx, item2, trackList, list4);
						if (list4.Count > 0)
						{
							flag = true;
						}
						item2.ValidationErrors = list4;
					}
				}
				if (flag && list3.All((ValidationError m) => m.Type != 1))
				{
					list3.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EntryHaveError", "分录有错误!"), 1));
				}
				item.ValidationErrors = list3;
			}
			if (!list.Exists((IVInvoiceModel f) => f.ValidationErrors.Any()) && list2.Any())
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				dynamicDbHelperMySQL.ExecuteSqlTran(list2);
			}
		}

		public OperationResult ImportInvoiceList(MContext ctx, List<IVInvoiceModel> list, bool isFromApi = false)
		{
			OperationResult operationResult = new OperationResult();
			List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();
			string empty = string.Empty;
			string empty2 = string.Empty;
			List<CommandInfo> list2 = new List<CommandInfo>();
			List<IVInvoiceEntryModel> entryList = new List<IVInvoiceEntryModel>();
			list.ForEach(delegate(IVInvoiceModel f)
			{
				entryList.AddRange(f.InvoiceEntry);
			});
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			base.ValidateBizDate(ctx, list, validationResult, false, "MBizDate");
			List<CommandInfo> list3 = _contactBiz.CheckContactExist(ctx, list, ref empty, validationResult, true, "MItemID", -1);
			_currencyBiz.CheckCurrencyExist(ctx, list, "MCyID", validationResult);
			List<REGCurrencyViewModel> allCurrencyList = _currencyBiz.GetAllCurrencyList(ctx, false, false);
			base.CheckCurrencyExchangeRateExist(ctx, list, validationResult, allCurrencyList, "MCyID", "MBizDate");
			_dictBiz.CheckTaxTypeExist(ctx, list, validationResult, null);
			_itemBiz.CheckItemExist(ctx, entryList, validationResult, "MItemID", "MItemID");
			base.ValidateDiscount<IVInvoiceModel, IVInvoiceEntryModel>(ctx, list, validationResult);
			base.ValidateTotalAmount<IVInvoiceModel, IVInvoiceEntryModel>(ctx, list, validationResult);
			base.CheckTaxRateSelect<IVInvoiceModel, IVInvoiceEntryModel>(ctx, list, validationResult, true);
			_taxRateBiz.CheckTaxRateExist(ctx, entryList, validationResult);
			_trackBiz.CheckTrackExist(ctx, list[0].TrackItemNameList, entryList, validationResult);
			CheckInvoiceNumberExist(ctx, list, validationResult);
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
			JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
			operationResult.Tag = javaScriptSerializer.Serialize(new
			{
				NewContact = empty
			});
			list2.AddRange(IVInvoiceRepository.GetImportInvoiceCmdList(ctx, list));
			OperationResult operationResult2 = ResultHelper.ToOperationResult(list);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list2) > 0);
			return operationResult;
		}

		private void SetDefaultValue(MContext ctx, IVInvoiceModel model)
		{
			if (model.IsNew && !model.IsUpdateFieldExists("MBizDate"))
			{
				model.MBizDate = ctx.DateNow.Date;
				model.UpdateFieldList.Add("MBizDate");
			}
		}

		private bool CheckData(IVInvoiceModel model)
		{
			return true;
		}

		private bool ProcessContact(MContext ctx, IVInvoiceModel model)
		{
			BDContactsInfoModel mContactInfo = model.MContactInfo;
			if (!model.IsNew && !model.IsUpdateFieldExists("MContactInfo"))
			{
				return true;
			}
			if (mContactInfo == null || string.IsNullOrEmpty(mContactInfo.MContactID))
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ContactIsNull", "联系人不能为空!")));
				return false;
			}
			if (string.IsNullOrEmpty(mContactInfo.MName))
			{
				model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "ContactNotExists", "联系人不存在!")));
				return false;
			}
			if (!mContactInfo.MIsActive)
			{
				model.ValidationErrors.Add(new ValidationError(string.Format(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "DataDisabled", "{0}已禁用!"), COMMultiLangRepository.GetText(ctx.MLCID, LangModule.Common, "Contact", "Contact"))));
			}
			if (mContactInfo.IsNew)
			{
				switch (model.MType)
				{
				case "Invoice_Sale":
				case "Invoice_Sale_Red":
					if (mContactInfo.MIsCustomer)
					{
						break;
					}
					model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "SaleContactNotCustomer", "销售单的联系人必须是客户!")));
					return false;
				case "Invoice_Purchase":
				case "Invoice_Purchase_Red":
					if (mContactInfo.MIsSupplier)
					{
						break;
					}
					model.ValidationErrors.Add(new ValidationError(COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "BillContactNotSupplier", "采购单的联系人必须是供应商!")));
					return false;
				}
			}
			return true;
		}

		public void ImportInvoiceFromApi(MContext ctx, List<IVInvoiceModel> list)
		{
			OperationResult operationResult = new OperationResult();
			List<IOValidationResultModel> validationResult = new List<IOValidationResultModel>();
			List<CommandInfo> list2 = new List<CommandInfo>();
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			List<CommandInfo> list3 = APIDataRepository.FillBaseData(ctx, list, true, null);
			if (list3 != null)
			{
				list2.AddRange(list3);
			}
			foreach (IVInvoiceModel item in list)
			{
				if (ProcessContact(ctx, item))
				{
					continue;
				}
			}
			CheckInvoicesRequiredField(ctx, list);
			base.ValidateBizDate(ctx, list, validationResult, false, "MBizDate");
			base.CheckTaxRateSelect<IVInvoiceModel, IVInvoiceEntryModel>(ctx, list, validationResult, false);
			base.SetValidationError(ctx, list, validationResult);
			List<IVInvoiceModel> list4 = new List<IVInvoiceModel>();
			foreach (IVInvoiceModel item2 in list)
			{
				try
				{
					if (item2.ValidationErrors == null || item2.ValidationErrors.Count == 0)
					{
						List<IVInvoiceModel> list5 = new List<IVInvoiceModel>();
						list5.Add(item2);
						List<CommandInfo> importInvoiceCmdList = IVInvoiceRepository.GetImportInvoiceCmdList(ctx, list5);
						list2.AddRange(importInvoiceCmdList);
						if (item2.MType == "Invoice_Sale_Red" || item2.MType == "Invoice_Purchase_Red")
						{
							foreach (IVInvoiceEntryModel mEntry in item2.MEntryList)
							{
								mEntry.MQty = Math.Abs(mEntry.MQty);
								mEntry.MTaxAmount = Math.Abs(mEntry.MTaxAmount);
								mEntry.MTaxAmountFor = Math.Abs(mEntry.MTaxAmountFor);
								mEntry.MAmountFor = Math.Abs(mEntry.MAmountFor);
								mEntry.MAmount = Math.Abs(mEntry.MAmount);
								mEntry.MTaxAmtFor = Math.Abs(mEntry.MTaxAmtFor);
								mEntry.MTaxAmt = Math.Abs(mEntry.MTaxAmt);
							}
						}
						list4.Add(item2);
					}
				}
				catch (MActionException ex)
				{
					List<MActionResultCodeEnum> codes = ex.Codes;
					List<ValidationError> list6 = item2.ValidationErrors ?? new List<ValidationError>();
					foreach (MActionResultCodeEnum item3 in codes)
					{
						list6.Add(new ValidationError
						{
							Message = GLInterfaceRepository.GetActionExceptionMessageByCode(ctx, item3)
						});
					}
					item2.ValidationErrors = list6;
				}
			}
			operationResult.Success = (list2.Any() && dynamicDbHelperMySQL.ExecuteSqlTran(list2) > 0);
		}

		public List<IOTemplateConfigModel> GetTemplateConfig(MContext ctx, string invoiceType, bool isFromExcel = false)
		{
			List<IOTemplateConfigModel> list = new List<IOTemplateConfigModel>();
			string[] megiLangTypes = ServerHelper.MegiLangTypes;
			string empty = string.Empty;
			bool flag = invoiceType == "Invoice_Sale" || invoiceType == "Invoice_Sale_Red";
			Dictionary<string, string> langList = flag ? COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.Contact, "Customer", "Customer", null)
			}) : COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
			{
				new COMLangInfoModel(LangModule.Contact, "Supplier", "Supplier", null)
			});
			list.Add(new IOTemplateConfigModel("MContactID", langList, true, true, 2, null));
			if (flag)
			{
				list.Add(new IOTemplateConfigModel("MNumber", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "Invoice", "Invoice #", null)
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
			bool flag2 = invoiceType == "Invoice_Sale" || invoiceType == "Invoice_Purchase";
			bool isRequired = flag2 && true;
			if (flag2)
			{
				list.Add(new IOTemplateConfigModel("MDueDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.Common, "DueDate", "Due Date", empty)
				}), IOTemplateFieldType.Date, isRequired, true, 2, null));
			}
			if (invoiceType == "Invoice_Sale")
			{
				list.Add(new IOTemplateConfigModel("MExpectedDate", COMMultiLangRepository.GetAllText(megiLangTypes, new COMLangInfoModel[1]
				{
					new COMLangInfoModel(LangModule.IV, "InvoiceExpectedPaymentDate", "Expected Payment date", empty)
				}), IOTemplateFieldType.Date, false, true, 2, null));
			}
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
				}), false, false, 2, null),
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
			if (isFromExcel && ctx.MEnabledModules.Contains(ModuleEnum.GL))
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

		public List<ImportTemplateDataSource> GetTemplateBasicData(MContext ctx, string invoiceType, bool isFromExcel = false, Dictionary<string, string[]> exampleDataList = null)
		{
			bool flag = invoiceType == "Invoice_Sale" || invoiceType == "Invoice_Sale_Red";
			int value = flag ? 1 : 2;
			List<BDContactsInfoModel> contactsInfo = _bdContacts.GetContactsInfo(ctx, Convert.ToString(value), string.Empty);
			List<BASDataDictionaryModel> dictList = BASDataDictionary.GetDictList("TaxType", ctx.MLCID);
			BASCurrencyViewModel @base = _bdCurrency.GetBase(ctx, false, null, null);
			List<REGCurrencyViewModel> viewList = _bdCurrency.GetViewList(ctx, null, null, false, null);
			List<BDItemModel> listByWhere = _item.GetListByWhere(ctx, string.Empty);
			List<REGTaxRateModel> list = _taxRate.GetList(ctx, null, false);
			List<NameValueModel> trackBasicInfo = _track.GetTrackBasicInfo(ctx, null, false, false);
			List<ImportTemplateDataSource> list2 = new List<ImportTemplateDataSource>();
			List<ImportDataSourceInfo> list3 = new List<ImportDataSourceInfo>();
			foreach (BDContactsInfoModel item3 in contactsInfo)
			{
				if (!string.IsNullOrWhiteSpace(item3.MName))
				{
					list3.Add(new ImportDataSourceInfo
					{
						Key = item3.MItemID,
						Value = item3.MName
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
					Value = $"{@base.MCurrencyID} {@base.MLocalName}",
					Info = "1"
				});
			}
			foreach (REGCurrencyViewModel item4 in viewList)
			{
				if (!list4.Any((ImportDataSourceInfo f) => f.Key == item4.MCurrencyID))
				{
					list4.Add(new ImportDataSourceInfo
					{
						Key = item4.MCurrencyID,
						Value = $"{item4.MCurrencyID} {item4.MName}"
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
			foreach (BDItemModel item5 in listByWhere)
			{
				list5.Add(new ImportDataSourceInfo
				{
					Key = item5.MItemID,
					Value = $"{((item5.MNumber != null) ? item5.MNumber.Trim() : string.Empty)}:{item5.MDesc.RemoveLineBreaks()}"
				});
				decimal num;
				object text;
				if (!flag)
				{
					num = Math.Round(item5.MPurPrice, 4);
					text = num.ToString();
				}
				else
				{
					num = Math.Round(item5.MSalPrice, 4);
					text = num.ToString();
				}
				string arg = (string)text;
				ImportDataSourceInfo importDataSourceInfo = list5.SingleOrDefault((ImportDataSourceInfo f) => f.Key == item5.MItemID);
				importDataSourceInfo.Value += $"{'┇'}{arg}";
			}
			list2.Add(new ImportTemplateDataSource(true)
			{
				FieldType = ImportTemplateColumnType.InventoryItem,
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
			if (isFromExcel && ctx.MEnabledModules.Contains(ModuleEnum.GL))
			{
				List<BDAccountModel> bDAccountList = _acctBiz.GetBDAccountList(ctx, new BDAccountListFilterModel(), false, false);
				List<ImportDataSourceInfo> list7 = new List<ImportDataSourceInfo>();
				foreach (BDAccountModel item6 in bDAccountList)
				{
					if (!string.IsNullOrWhiteSpace(item6.MCode))
					{
						list7.Add(new ImportDataSourceInfo
						{
							Key = item6.MCode,
							Value = item6.MFullName
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
					dataSourceList2.FirstOrDefault((ImportDataSourceInfo f) => f.Key == ctx.MBasCurrencyID).Value
				});
			}
			return list2;
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx, string invoiceType)
		{
			return GetImportTemplateModel(ctx, invoiceType, false);
		}

		public ImportTemplateModel GetImportTemplateModel(MContext ctx, string invoiceType, bool isFromExcel = false)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("MTaxTypeID", "MTaxID");
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			List<IOTemplateConfigModel> templateConfig = GetTemplateConfig(ctx, invoiceType, isFromExcel);
			foreach (IOTemplateConfigModel item in templateConfig)
			{
				dictionary2.Add(item.MFieldName, item.MLangList.FirstOrDefault((KeyValuePair<string, string> f) => f.Key == ctx.MLCID).Value);
			}
			Dictionary<string, string[]> dictionary3 = new Dictionary<string, string[]>();
			List<ImportTemplateDataSource> templateBasicData = GetTemplateBasicData(ctx, invoiceType, isFromExcel, dictionary3);
			DateTime dateNow = ctx.DateNow;
			dictionary3.Add("MNumber", new string[1]
			{
				(invoiceType == "Invoice_Sale") ? "INV-0001" : "CN-0001"
			});
			dictionary3.Add("MBizDate", new string[1]
			{
				dateNow.ToString("yyyy-MM-dd")
			});
			Dictionary<string, string[]> dictionary4 = dictionary3;
			string[] obj = new string[1];
			DateTime dateTime = dateNow.AddDays(20.0);
			obj[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary4.Add("MDueDate", obj);
			Dictionary<string, string[]> dictionary5 = dictionary3;
			string[] obj2 = new string[1];
			dateTime = dateNow.AddDays(10.0);
			obj2[0] = dateTime.ToString("yyyy-MM-dd");
			dictionary5.Add("MExpectedDate", obj2);
			dictionary3.Add("MQty", new string[2]
			{
				"10.0000",
				"15.0000"
			});
			dictionary3.Add("MPrice", new string[2]
			{
				"12.00000000",
				"20.00000000"
			});
			List<string> alignRightFieldList = "MBizDate,MDueDate,MExpectedDate,MQty,MPrice".Split(',').ToList();
			return new ImportTemplateModel
			{
				TemplateType = invoiceType,
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

		public List<IVInvoiceSendModel> GetSendInvoiceList(MContext ctx, IVInvoiceSendParam param)
		{
			return IVInvoiceRepository.GetSendInvoiceList(ctx, param);
		}

		public OperationResult SendInvoiceList(MContext ctx, IVInvoiceEmailSendModel model)
		{
			return IVInvoiceRepository.SendInvoiceList(ctx, model);
		}

		public List<IVEmailTmplPreviewModel> PreviewEmailTmpl(MContext ctx, IVInvoiceEmailSendModel model)
		{
			List<IVEmailTmplPreviewModel> list = new List<IVEmailTmplPreviewModel>();
			foreach (IVInvoiceSendModel sendEntry in model.SendEntryList)
			{
				list.Add(new IVEmailTmplPreviewModel
				{
					MSubject = model.MSubject,
					MContent = model.MContent,
					MSubjectPreview = IVInvoiceRepository.PreviewEmailTmpl(ctx, model, sendEntry, model.MSubject, EmailPreviewTypeEnum.Subject, false),
					MContentPreview = IVInvoiceRepository.PreviewEmailTmpl(ctx, model, sendEntry, model.MContent, EmailPreviewTypeEnum.Content, false)
				});
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

		public OperationResult InsertOrUpdate(MContext ctx, IVInvoiceModel modelData, string fields = null)
		{
			return dal.InsertOrUpdate(ctx, modelData, fields);
		}

		public OperationResult InsertOrUpdateModels(MContext ctx, List<IVInvoiceModel> modelData, string fields = null)
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

		public IVInvoiceModel GetDataModel(MContext ctx, string pkID, bool includeDelete = false)
		{
			return dal.GetDataModel(ctx, pkID, includeDelete);
		}

		public IVInvoiceModel GetDataModelByFilter(MContext ctx, SqlWhere filter)
		{
			return dal.GetDataModelByFilter(ctx, filter);
		}

		public List<IVInvoiceModel> GetModelList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelList(ctx, filter, includeDelete);
		}

		public DataGridJson<IVInvoiceModel> GetModelPageList(MContext ctx, SqlWhere filter, bool includeDelete = false)
		{
			return dal.GetModelPageList(ctx, filter, includeDelete);
		}

		public List<BDCheckInactiveModel> GetInactiveList(MContext ctx)
		{
			return GLDataPool.GetInstance(ctx, false, 0, 0, 0).BDInactiveList;
		}

		public IVInvoiceModel Delete(MContext ctx, DeleteParam param)
		{
			GetParam getParam = new GetParam();
			getParam.ElementID = param.ElementID;
			IVInvoiceModel iVInvoiceModel = Get(ctx, getParam).rows.FirstOrDefault();
			if (iVInvoiceModel != null)
			{
				if (iVInvoiceModel.ValidationErrors == null)
				{
					iVInvoiceModel.ValidationErrors = new List<ValidationError>();
				}
				if (iVInvoiceModel.MStatus == 3)
				{
					iVInvoiceModel.ValidationErrors.Add(new ValidationError
					{
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteApprovedInvoiceFail", "已审核的单据不能删除！")
					});
					return iVInvoiceModel;
				}
				if (iVInvoiceModel.MStatus == 4)
				{
					iVInvoiceModel.ValidationErrors.Add(new ValidationError
					{
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteReceivedInvoiceFail", "已收款的单据不允许删除！")
					});
					return iVInvoiceModel;
				}
				ParamBase paramBase = new ParamBase();
				paramBase.KeyIDs = param.ElementID;
				OperationResult operationResult = IVInvoiceRepository.DeleteInvoiceList(ctx, paramBase);
				if (!operationResult.Success)
				{
					iVInvoiceModel.ValidationErrors.Add(new ValidationError
					{
						Message = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "DeleteInvoiceFail", " 删除单据失败！")
					});
				}
			}
			else
			{
				COMValidateHelper.AddRecordNotExistError(ctx, ref iVInvoiceModel);
			}
			return iVInvoiceModel;
		}

		public DataGridJson<IVInvoiceModel> Get(MContext ctx, GetParam param)
		{
			SqlWhere sqlWhere = new SqlWhere();
			if (param.ModifiedSince > DateTime.MinValue)
			{
				sqlWhere.GreaterThen("MModifyDate", param.ModifiedSince);
			}
			if (!string.IsNullOrWhiteSpace(param.ElementID))
			{
				sqlWhere.Equal("MID", param.ElementID);
			}
			if (!string.IsNullOrWhiteSpace(param.Type))
			{
				sqlWhere.In("MType", param.Type.Split(','));
			}
			sqlWhere.PageSize = param.PageSize;
			sqlWhere.PageIndex = param.PageIndex;
			sqlWhere.WhereSqlString = param.Where;
			List<IVInvoiceModel> dataModelList = ModelInfoManager.GetDataModelList<IVInvoiceModel>(ctx, sqlWhere, true, true);
			APIDataPool instance = APIDataPool.GetInstance(ctx);
			List<BDContactsInfoModel> contacts = instance.Contacts;
			if (dataModelList != null && dataModelList.Count > 0)
			{
				foreach (IVInvoiceModel item in dataModelList)
				{
					item.MContactInfo = contacts.FirstOrDefault((BDContactsInfoModel f) => f.MItemID == item.MContactID);
					if (item.MEntryList != null && item.MEntryList.Count != 0 && (item.MType == "Invoice_Sale_Red" || item.MType == "Invoice_Purchase_Red"))
					{
						foreach (IVInvoiceEntryModel mEntry in item.MEntryList)
						{
							mEntry.MQty = Math.Abs(mEntry.MQty);
							mEntry.MTaxAmount = Math.Abs(mEntry.MTaxAmount);
							mEntry.MTaxAmountFor = Math.Abs(mEntry.MTaxAmountFor);
							mEntry.MAmountFor = Math.Abs(mEntry.MAmountFor);
							mEntry.MAmount = Math.Abs(mEntry.MAmount);
							mEntry.MTaxAmtFor = Math.Abs(mEntry.MTaxAmtFor);
							mEntry.MTaxAmt = Math.Abs(mEntry.MTaxAmt);
						}
					}
				}
			}
			DataGridJson<IVInvoiceModel> dataGridJson = new DataGridJson<IVInvoiceModel>();
			dataGridJson.rows = dataModelList;
			dataGridJson.total = ModelInfoManager.GetDataModelListCount(ctx, typeof(IVInvoiceModel), sqlWhere);
			return dataGridJson;
		}

		public List<IVInvoiceModel> Post(MContext ctx, PostParam<IVInvoiceModel> param)
		{
			List<IVInvoiceModel> dataList = param.DataList;
			if (dataList.Count > 0)
			{
				ImportInvoiceFromApi(ctx, dataList);
			}
			return dataList;
		}
	}
}
