using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Core.Log
{
	public class OptLog
	{
		public static void AddLog(OptLogTemplate template, MContext ctx, string pkId, params object[] formatValues)
		{
			OptLogRepository.AddLog(template, ctx, pkId, false, formatValues);
		}

		public static CommandInfo GetAddLogCommand(OptLogTemplate template, MContext ctx, string pkId, bool createDateAdd, params object[] formatValues)
		{
			return OptLogRepository.GetAddLogCommand(template, ctx, pkId, createDateAdd, formatValues);
		}

		public static CommandInfo GetAddLogCommand(OptLogTemplate template, MContext ctx, List<string> pkIdList, bool createDateAdd, Dictionary<string, List<object>> formatValues = null)
		{
			return OptLogRepository.GetAddLogCommand(template, ctx, pkIdList, createDateAdd, formatValues);
		}

		public static CommandInfo GetAddLogCommand(OptLogTemplate template, MContext ctx, string pkId, params object[] formatValues)
		{
			return OptLogRepository.GetAddLogCommand(template, ctx, pkId, false, formatValues);
		}

		public static DataGridJson<OptLogListModel> GetLogList(OptLogListFilter filter)
		{
			List<NameValueModel> orgContactList = OptLogRepository.GetOrgContactList(filter);
			List<NameValueModel> orgEmployeesList = OptLogRepository.GetOrgEmployeesList(filter);
			DataGridJson<OptLogListModel> logList = OptLogRepository.GetLogList(filter);
			List<OptLogListModel> rows = logList.rows;
			if (rows.Count > 0)
			{
				for (int i = 0; i < rows.Count; i++)
				{
					OptLogTemplate optLogTemplate = (OptLogTemplate)Convert.ToInt32(rows[i].MTemplateID);
					rows[i].MUserName = GlobalFormat.GetUserName(rows[i].MFirstName, rows[i].MLastName, filter.MContext);
					switch (optLogTemplate)
					{
					case OptLogTemplate.Sale_Invoice_Created:
					case OptLogTemplate.Sale_Invoice_Edited:
					case OptLogTemplate.Sale_Invoice_Approved:
					case OptLogTemplate.Sale_Credit_Note_Created:
					case OptLogTemplate.Sale_Credit_Note_Edited:
					case OptLogTemplate.Sale_Credit_Note_Approved:
					case OptLogTemplate.Bill_Created:
					case OptLogTemplate.Bill_Edited:
					case OptLogTemplate.Bill_Approved:
					case OptLogTemplate.Bill_Credit_Note_Created:
					case OptLogTemplate.Bill_Credit_Note_Edited:
					case OptLogTemplate.Bill_Credit_Note_Approved:
					case OptLogTemplate.Sale_Invoice_Reverse_Approved:
					case OptLogTemplate.Credit_Note_Reverse_Approved:
					case OptLogTemplate.Bill_Reverse_Approved:
					case OptLogTemplate.Bill_Credit_Note_Reverse_Approved:
					case OptLogTemplate.Sale_Invoice_Submit_For_Approval:
					case OptLogTemplate.Sale_Credit_Note_Submit_For_Approval:
					case OptLogTemplate.Bill_Submit_For_Approval:
					case OptLogTemplate.Bill_Credit_Note_Submit_For_Approval:
					case OptLogTemplate.Sale_Invoice_Edit_Account:
					case OptLogTemplate.Sale_Credit_Note_Edit_Account:
					case OptLogTemplate.Bill_Edit_Account:
					case OptLogTemplate.Bill_Credit_Note_Edit_Account:
					case OptLogTemplate.Sale_Invoice_Generate_Voucher:
					case OptLogTemplate.Sale_Credit_Note_Generate_Voucher:
					case OptLogTemplate.Bill_Generate_Voucher:
					case OptLogTemplate.Bill_Credit_Note_Generate_Voucher:
						rows[i].MValue2 = GetContactName(rows[i].MValue2, orgContactList);
						break;
					case OptLogTemplate.ExpenseClaims_Created:
					case OptLogTemplate.ExpenseClaims_Edited:
					case OptLogTemplate.ExpenseClaims_Approved:
					case OptLogTemplate.ExpenseClaims_Credit:
					case OptLogTemplate.ExpenseClaims_Reverse_Approved:
					case OptLogTemplate.ExpenseClaims_Submit_For_Approval:
					case OptLogTemplate.ExpenseClaims_Edit_Account:
					case OptLogTemplate.ExpenseClaims_Generate_Voucher:
						rows[i].MValue2 = GetEmployeesName(rows[i].MValue2, orgEmployeesList);
						break;
					case OptLogTemplate.Sale_Invoice_PartiallyPaid:
					case OptLogTemplate.Sale_Invoice_Paid:
					case OptLogTemplate.Sale_Credit_Note_PartiallyPaid:
					case OptLogTemplate.Sale_Credit_Note_Paid:
					case OptLogTemplate.Bill_PartiallyPaid:
					case OptLogTemplate.Bill_Paid:
					case OptLogTemplate.Bill_Credit_Note_PartiallyPaid:
					case OptLogTemplate.Bill_Credit_Note_Paid:
					case OptLogTemplate.Contact_Created:
					case OptLogTemplate.Contact_Edited:
					case OptLogTemplate.Payment_Created:
					case OptLogTemplate.Payment_Edited:
					case OptLogTemplate.Receive_Created:
					case OptLogTemplate.Receive_Edited:
						rows[i].MValue1 = GetContactName(rows[i].MValue1, orgContactList);
						break;
					case OptLogTemplate.ExpenseClaims_PartiallyPaid:
					case OptLogTemplate.ExpenseClaims_Paid:
						rows[i].MValue1 = GetEmployeesName(rows[i].MValue1, orgEmployeesList);
						break;
					}
				}
			}
			logList.rows = rows;
			return logList;
		}

		private static string GetContactName(string contactId, List<NameValueModel> listContact)
		{
			string text = (from t in listContact
			where t.MValue == contactId
			select t.MName).FirstOrDefault();
			return string.IsNullOrEmpty(text) ? "" : text;
		}

		private static string GetEmployeesName(string employeesId, List<NameValueModel> listEmployees)
		{
			string text = (from t in listEmployees
			where t.MValue == employeesId
			select t.MName).FirstOrDefault();
			return string.IsNullOrEmpty(text) ? "" : text;
		}
	}
}
