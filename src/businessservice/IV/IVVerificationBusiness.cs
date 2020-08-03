using JieNor.Megi.BusinessContract.IV;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataModel.IV.Verification;
using JieNor.Megi.DataModel.PA;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.IV;
using JieNor.Megi.DataRepository.PA;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.MultiLanguage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.BusinessService.IV
{
	public class IVVerificationBusiness : IIVVerificationBusiness
	{
		public List<IVVerificationListModel> GetVerificationList(MContext ctx, IVVerificationListFilterModel filter)
		{
			List<IVVerificationInforModel> customerWaitForVerificationInfor = GetCustomerWaitForVerificationInfor(ctx, filter.MBillID, filter.MBizBillType);
			List<IVVerificationListModel> list = new List<IVVerificationListModel>();
			if (customerWaitForVerificationInfor == null || customerWaitForVerificationInfor.Count == 0)
			{
				return list;
			}
			foreach (IVVerificationInforModel item in customerWaitForVerificationInfor)
			{
				filter.MBizBillType = item.MBizBillType;
				filter.MBizType = item.MBizType;
				filter.MContactType = item.MContactType;
				list.AddRange(IVVerificationRepository.GetVerificationList(ctx, filter));
			}
			return list;
		}

		public void UpdateVerification(MContext ctx, IVVerificationModel model)
		{
			IVVerificationRepository.UpdateVerification(ctx, model);
		}

		public OperationResult UpdateVerificationList(MContext ctx, List<IVVerificationModel> list)
		{
			return IVVerificationRepository.UpdateVerificationList(ctx, list);
		}

		public OperationResult DeleteVerification(MContext ctx, IVVerificationModel model)
		{
			return IVVerificationRepository.DeleteVerification(ctx, model);
		}

		public OperationResult DeleteVerificationByPKID(MContext ctx, string pkID, bool isMergePay = false)
		{
			return IVVerificationRepository.DeleteVerification(ctx, pkID, isMergePay);
		}

		public IVVerificationModel GetVerificationEditModel(MContext ctx, string pkID)
		{
			return IVVerificationRepository.GetVerificationEditModel(ctx, pkID);
		}

		public List<IVVerificationInforModel> GetCustomerWaitForVerificationInfor(MContext ctx, string contactID, string currencyID, string bizBillType, string bizType)
		{
			return IVVerificationRepository.GetCustomerWaitForVerificationInfor(ctx, contactID, currencyID, bizBillType, bizType, string.Empty);
		}

		public List<IVVerificationInforModel> GetCustomerWaitForVerificationInfor(MContext ctx, string billId, string bizBillType)
		{
			string contactID = string.Empty;
			string currencyID = string.Empty;
			string bizType = string.Empty;
			string contactType = "";
			switch (bizBillType)
			{
			case "Invoice":
			{
				IVInvoiceModel invoiceModel = IVInvoiceRepository.GetInvoiceModel(ctx, billId);
				if (invoiceModel == null || invoiceModel.MTaxTotalAmtFor - invoiceModel.MVerificationAmt == decimal.Zero)
				{
					return null;
				}
				contactID = invoiceModel.MContactID;
				currencyID = invoiceModel.MCyID;
				bizType = invoiceModel.MType;
				goto default;
			}
			case "Payment":
			{
				IVPaymentModel paymentEditModel = IVPaymentRepository.GetPaymentEditModel(ctx, billId);
				if (paymentEditModel == null || paymentEditModel.MTaxTotalAmtFor - paymentEditModel.MVerificationAmt == decimal.Zero)
				{
					return null;
				}
				contactID = paymentEditModel.MContactID;
				contactType = paymentEditModel.MContactType;
				currencyID = paymentEditModel.MCyID;
				bizType = paymentEditModel.MType;
				goto default;
			}
			case "Receive":
			{
				IVReceiveModel receiveEditModel = IVReceiveRepository.GetReceiveEditModel(ctx, billId);
				if (receiveEditModel == null || receiveEditModel.MTaxTotalAmtFor - receiveEditModel.MVerificationAmt == decimal.Zero)
				{
					return null;
				}
				contactID = receiveEditModel.MContactID;
				contactType = receiveEditModel.MContactType;
				currencyID = receiveEditModel.MCyID;
				bizType = receiveEditModel.MType;
				goto default;
			}
			case "Expense":
			{
				IVExpenseModel expenseModel = IVExpenseRepository.GetExpenseModel(ctx, billId);
				if (expenseModel == null || expenseModel.MTaxTotalAmtFor - expenseModel.MVerificationAmt == decimal.Zero)
				{
					return null;
				}
				contactID = expenseModel.MContactID;
				currencyID = expenseModel.MCyID;
				bizType = expenseModel.MType;
				goto default;
			}
			case "PayRun":
			{
				PASalaryPaymentModel salaryPaymentEditModel = PASalaryPaymentRepository.GetSalaryPaymentEditModel(ctx, billId);
				if (salaryPaymentEditModel == null || (salaryPaymentEditModel.MStatus == 4 && salaryPaymentEditModel.MNetSalary - salaryPaymentEditModel.MVerificationAmt == decimal.Zero))
				{
					return null;
				}
				contactID = salaryPaymentEditModel.MEmployeeID;
				currencyID = ctx.MBasCurrencyID;
				bizType = "Pay_Salary";
				goto default;
			}
			default:
				return IVVerificationRepository.GetCustomerWaitForVerificationInfor(ctx, contactID, currencyID, bizBillType, bizType, contactType);
			}
		}

		public bool CheckIsCanEditOrVoidOrDelete(MContext ctx, string billType, string pkID)
		{
			return IVVerificationRepository.CheckIsCanEditOrDelete(ctx, billType, pkID);
		}

		public List<IVBatchPaymentModel> GetBatchPaymentList(MContext ctx, ParamBase para, string selectObj, bool isMergePay)
		{
			List<IVBatchPaymentModel> batchPaymentList = IVVerificationRepository.GetBatchPaymentList(ctx, para, selectObj);
			if (batchPaymentList != null && batchPaymentList.Count > 0 && (selectObj == "Expense" || selectObj == "PayRun"))
			{
				for (int i = 0; i < batchPaymentList.Count; i++)
				{
					batchPaymentList[i].MContactName = GlobalFormat.GetUserName(batchPaymentList[i].MFirstName, batchPaymentList[i].MLastName, null);
					if (selectObj == "PayRun")
					{
						batchPaymentList[i].MReference = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.PA, "SalaryPaymentReference", "{0}工资", batchPaymentList[i].MNumber);
					}
				}
				if (isMergePay)
				{
					List<IVBatchPaymentModel> list = new List<IVBatchPaymentModel>();
					IVBatchPaymentModel iVBatchPaymentModel = batchPaymentList.FirstOrDefault();
					iVBatchPaymentModel.MContactName = "";
					iVBatchPaymentModel.MNoVerifyAmtFor = batchPaymentList.Sum((IVBatchPaymentModel m) => m.MNoVerifyAmtFor);
					iVBatchPaymentModel.MPayAmount = batchPaymentList.Sum((IVBatchPaymentModel m) => m.MPayAmount);
					iVBatchPaymentModel.MReference = COMMultiLangRepository.GetTextFormat(ctx.MLCID, LangModule.PA, "SalaryPaymentReference", "{0}工资", iVBatchPaymentModel.MNumber);
					list.Add(iVBatchPaymentModel);
					return list;
				}
			}
			return batchPaymentList;
		}

		public OperationResult BatchPaymentUpdate(MContext ctx, IVBatchPayHeadModel headModel)
		{
			return IVVerificationRepository.BatchPaymentUpdate(ctx, headModel);
		}

		public OperationResult IsSuccessBatch(MContext ctx, ParamBase para, string selectObj)
		{
			return IVVerificationRepository.IsSuccessBatch(ctx, para, selectObj);
		}

		public List<IVVerifiactionDocMapModel> GetVerificationList(MContext ctx, IVVerificationFilterModel filter)
		{
			int searchMethod = GetSearchMethod(filter);
			List<IVVerifiactionDocMapModel> list = new List<IVVerifiactionDocMapModel>();
			switch (searchMethod)
			{
			case 1:
			case 3:
				if (filter.EndDate != DateTime.MinValue)
				{
					filter.EndDate = filter.EndDate.AddDays(1.0);
				}
				list = IVVerificationRepository.GetVerificationList(ctx, filter);
				break;
			case 2:
				list = GetVerificationDocList(ctx, filter);
				break;
			}
			if (list == null || list.Count() == 0)
			{
				return list;
			}
			Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
			foreach (IVVerifiactionDocMapModel item in list)
			{
				if (dictionary.Keys.Contains(item.MSourceBillType))
				{
					dictionary[item.MSourceBillType].Add(item.MSourceBillID);
				}
				else
				{
					List<string> list2 = new List<string>();
					list2.Add(item.MSourceBillID);
					dictionary.Add(item.MSourceBillType, list2);
				}
				if (dictionary.Keys.Contains(item.MTargetBillType))
				{
					dictionary[item.MTargetBillType].Add(item.MTargetBillID);
				}
				else
				{
					List<string> list3 = new List<string>();
					list3.Add(item.MTargetBillID);
					dictionary.Add(item.MTargetBillType, list3);
				}
			}
			foreach (string key in dictionary.Keys)
			{
				List<string> values = dictionary[key];
				switch (key)
				{
				case "Invoice":
				{
					IVInvoiceListFilterModel iVInvoiceListFilterModel = new IVInvoiceListFilterModel();
					iVInvoiceListFilterModel.In("MItemID", values);
					iVInvoiceListFilterModel.page = 1;
					iVInvoiceListFilterModel.PageSize = 2147483647;
					List<IVInvoiceListModel> invoiceList = IVInvoiceRepository.GetInvoiceList(ctx, iVInvoiceListFilterModel);
					SetVerificationBillModel(list, invoiceList);
					break;
				}
				case "Receive":
				{
					ParamBase paramBase2 = new ParamBase();
					paramBase2.KeyIDs = string.Join(",", values);
					List<IVReceiveModel> receiveList = IVReceiveRepository.GetReceiveList(ctx, paramBase2);
					SetVerificationBillModel(list, receiveList);
					break;
				}
				case "Payment":
				{
					ParamBase paramBase = new ParamBase();
					paramBase.KeyIDs = string.Join(",", values);
					List<IVPaymentModel> paymentList = IVPaymentRepository.GetPaymentList(ctx, paramBase);
					SetVerificationBillModel(list, paymentList);
					break;
				}
				}
			}
			return list;
		}

		private int GetSearchMethod(IVVerificationFilterModel filter)
		{
			int result = 1;
			if (filter.StartDate != DateTime.MinValue && filter.EndDate != DateTime.MinValue && (filter.ReceiveStartDate == DateTime.MinValue || filter.ReceiveEndDate == DateTime.MinValue))
			{
				result = 1;
			}
			else if (filter.ReceiveStartDate != DateTime.MinValue && filter.ReceiveEndDate != DateTime.MinValue && (filter.StartDate == DateTime.MinValue || filter.EndDate == DateTime.MinValue))
			{
				result = 2;
			}
			else if (filter.ReceiveStartDate != DateTime.MinValue && filter.ReceiveEndDate != DateTime.MinValue && filter.StartDate != DateTime.MinValue && filter.EndDate != DateTime.MinValue)
			{
				result = 3;
			}
			return result;
		}

		private List<IVVerifiactionDocMapModel> GetVerificationDocList(MContext ctx, IVVerificationFilterModel filter)
		{
			IVReceiveListFilterModel iVReceiveListFilterModel = new IVReceiveListFilterModel();
			iVReceiveListFilterModel.MStartDate = filter.ReceiveStartDate;
			IVReceiveListFilterModel iVReceiveListFilterModel2 = iVReceiveListFilterModel;
			DateTime mEndDate;
			DateTime dateTime;
			if (!(filter.ReceiveEndDate != DateTime.MinValue))
			{
				mEndDate = filter.ReceiveEndDate;
			}
			else
			{
				dateTime = filter.ReceiveEndDate;
				dateTime = dateTime.AddDays(1.0);
				mEndDate = dateTime.AddSeconds(-1.0);
			}
			iVReceiveListFilterModel2.MEndDate = mEndDate;
			iVReceiveListFilterModel.page = 1;
			iVReceiveListFilterModel.PageSize = 2147483647;
			IVPaymentListFilterModel iVPaymentListFilterModel = new IVPaymentListFilterModel();
			iVPaymentListFilterModel.MStartDate = filter.ReceiveStartDate;
			IVPaymentListFilterModel iVPaymentListFilterModel2 = iVPaymentListFilterModel;
			DateTime mEndDate2;
			if (!(filter.ReceiveEndDate != DateTime.MinValue))
			{
				mEndDate2 = filter.ReceiveEndDate;
			}
			else
			{
				dateTime = filter.ReceiveEndDate;
				dateTime = dateTime.AddDays(1.0);
				mEndDate2 = dateTime.AddSeconds(-1.0);
			}
			iVPaymentListFilterModel2.MEndDate = mEndDate2;
			iVPaymentListFilterModel.page = 1;
			iVPaymentListFilterModel.PageSize = 2147483647;
			List<IVReceiveModel> receiveListByFilter = IVReceiveRepository.GetReceiveListByFilter(ctx, iVReceiveListFilterModel);
			List<IVPaymentModel> paymentListByFilter = IVPaymentRepository.GetPaymentListByFilter(ctx, iVPaymentListFilterModel);
			List<string> list = new List<string>();
			if (receiveListByFilter != null && receiveListByFilter.Count() > 0)
			{
				List<string> collection = (from x in receiveListByFilter
				select x.MID).ToList();
				list.AddRange(collection);
			}
			if (paymentListByFilter != null && paymentListByFilter.Count() > 0)
			{
				List<string> collection2 = (from x in paymentListByFilter
				select x.MID).ToList();
				list.AddRange(collection2);
			}
			filter = new IVVerificationFilterModel();
			filter.BillIdList = list;
			return IVVerificationRepository.GetVerificationList(ctx, filter);
		}

		private void SetVerificationBillModel<T>(List<IVVerifiactionDocMapModel> verificationList, T billList)
		{
			if (((object)billList) is List<IVInvoiceListModel>)
			{
				List<IVInvoiceListModel> source = billList as List<IVInvoiceListModel>;
				foreach (IVVerifiactionDocMapModel verification in verificationList)
				{
					List<IVInvoiceListModel> invoiceList = (from x in source
					where x.MID == verification.MSourceBillID || x.MID == verification.MTargetBillID
					select x).ToList();
					verification.InvoiceList = invoiceList;
				}
			}
			else if (((object)billList) is List<IVReceiveModel>)
			{
				List<IVReceiveModel> source2 = billList as List<IVReceiveModel>;
				foreach (IVVerifiactionDocMapModel verification2 in verificationList)
				{
					List<IVReceiveModel> receiveList = (from x in source2
					where x.MID == verification2.MSourceBillID || x.MID == verification2.MTargetBillID
					select x).ToList();
					verification2.ReceiveList = receiveList;
				}
			}
			else if (((object)billList) is List<IVPaymentModel>)
			{
				List<IVPaymentModel> source3 = billList as List<IVPaymentModel>;
				foreach (IVVerifiactionDocMapModel verification3 in verificationList)
				{
					List<IVPaymentModel> paymentList = (from x in source3
					where x.MID == verification3.MSourceBillID || x.MID == verification3.MTargetBillID
					select x).ToList();
					verification3.PaymentList = paymentList;
				}
			}
		}
	}
}
