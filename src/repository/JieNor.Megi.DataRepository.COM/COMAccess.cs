using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.COM
{
	public static class COMAccess
	{
		public static MAccessResponseModel GetAccessResult(MContext ctx, List<MAccessRequestModel> requestList = null)
		{
			requestList = ((requestList == null || requestList.Count == 0) ? GetAllAccessRequest(ctx) : requestList);
			MAccessResponseModel mAccessResponseModel = new MAccessResponseModel
			{
				Access = new Dictionary<string, bool>(),
				ctx = ctx
			};
			List<string> bizObjectList = GetBizObjectList(ctx, false);
			for (int i = 0; i < requestList.Count; i++)
			{
				MAccessRequestModel request = requestList[i];
				bool flag = true;
				if (request.RequestType == 0 || request.RequestType == 2)
				{
					flag = (flag && bizObjectList.Exists((string x) => x.ToLower() == request.BizModule.ToLower()));
				}
				if (flag && !string.IsNullOrWhiteSpace(request.BizAccess) && (request.RequestType == 1 || request.RequestType == 2))
				{
					flag = (flag && SECPermissionRepository.HavePermission(ctx, request.BizModule, request.BizAccess, ""));
				}
				request.Name = (string.IsNullOrWhiteSpace(request.Name) ? (request.BizModule + request.BizAccess) : request.Name);
				mAccessResponseModel.Access[request.Name] = flag;
			}
			return mAccessResponseModel;
		}

		public static MAccessResponseModel GetAccessResult(MContext ctx, bool isGetCache, List<MAccessRequestModel> requestList = null)
		{
			requestList = ((requestList == null || requestList.Count == 0) ? GetAllAccessRequest(ctx) : requestList);
			MAccessResponseModel mAccessResponseModel = new MAccessResponseModel
			{
				Access = new Dictionary<string, bool>(),
				ctx = ctx
			};
			List<string> bizObjectList = GetBizObjectList(ctx, false);
			List<SECUserPermissionModel> userPermissionNoByCache = SECPermissionRepository.GetUserPermissionNoByCache(ctx, "");
			for (int i = 0; i < requestList.Count; i++)
			{
				MAccessRequestModel request = requestList[i];
				bool flag = true;
				if (request.RequestType == 0 || request.RequestType == 2)
				{
					flag = (flag && bizObjectList.Exists((string x) => x.ToLower() == request.BizModule.ToLower()));
				}
				if (flag && !string.IsNullOrWhiteSpace(request.BizAccess) && (request.RequestType == 1 || request.RequestType == 2))
				{
					flag = (flag && SECPermissionRepository.HavePermissionNoByCache(ctx, request.BizModule, request.BizAccess, userPermissionNoByCache, ""));
				}
				request.Name = (string.IsNullOrWhiteSpace(request.Name) ? (request.BizModule + request.BizAccess) : request.Name);
				mAccessResponseModel.Access[request.Name] = flag;
			}
			return mAccessResponseModel;
		}

		private static List<MAccessRequestModel> GetAllAccessRequest(MContext ctx)
		{
			List<MAccessRequestModel> list = new List<MAccessRequestModel>();
			List<string> bizObjectList = GetBizObjectList(ctx, true);
			for (int i = 0; i < bizObjectList.Count; i++)
			{
				list.Add(new MAccessRequestModel
				{
					BizModule = bizObjectList[i],
					BizAccess = "Change",
					RequestType = 2
				});
				list.Add(new MAccessRequestModel
				{
					BizModule = bizObjectList[i],
					BizAccess = "Approve",
					RequestType = 2
				});
				list.Add(new MAccessRequestModel
				{
					BizModule = bizObjectList[i],
					BizAccess = "View",
					RequestType = 2
				});
				list.Add(new MAccessRequestModel
				{
					BizModule = bizObjectList[i],
					BizAccess = "Export",
					RequestType = 2
				});
				list.Add(new MAccessRequestModel
				{
					BizModule = bizObjectList[i],
					RequestType = 0
				});
			}
			return list;
		}

		public static bool GetAccessResult(MContext ctx, MAccessRequestModel request)
		{
			return GetAccessResult(ctx, new List<MAccessRequestModel>
			{
				request
			}).Access[request.Name];
		}

		public static bool GetAccessResult(MContext ctx, string obj, string item, int type = 2)
		{
			return GetAccessResult(ctx, new List<MAccessRequestModel>
			{
				new MAccessRequestModel
				{
					Name = obj + item,
					BizAccess = item,
					BizModule = obj,
					RequestType = type
				}
			}).Access[obj + item];
		}

		private static List<string> GetBizObjectList(MContext ctx, bool all = false)
		{
			if (ctx.MOrgVersionID == 0 | all)
			{
				return new List<string>
				{
					"Account",
					"AccountEdit",
					"Adviser",
					"ALL",
					"Attachment",
					"Attachment_Category",
					"Balances",
					"Bank",
					"BankAccount",
					"Bank_Reconciliation",
					"Bank_Reports",
					"Bill",
					"Contact",
					"Currency",
					"Department",
					"Employees",
					"Excel_Plus_Download",
					"Migration_Tool_Download",
					"ExchangeRate",
					"Expense",
					"ExpenseItem",
					"Expense_Reports",
					"Financial_Reports",
					"Fixed_Assets",
					"Fixed_Assets_Reports",
					"General_Ledger",
					"Other_Reports",
					"Invoice",
					"Invoice_Purchases",
					"Invoice_Sales",
					"Item",
					"MegiFapiao",
					"Org",
					"Payment",
					"PayRun",
					"PayRun_Reports",
					"Pay_Salary",
					"Purchases_Fapiao",
					"Purchase_Reports",
					"Receive",
					"Report",
					"Role",
					"Sales_Fapiao",
					"Sale_Reports",
					"Setting",
					"TaxRate",
					"Track",
					"TrackEntry",
					"Transfer",
					"Transferbill",
					"User",
					"Voucher"
				};
			}
			if (ctx.MOrgVersionID == 1)
			{
				return new List<string>
				{
					"Account",
					"AccountEdit",
					"Adviser",
					"ALL",
					"Attachment",
					"Attachment_Category",
					"Balances",
					"Bank",
					"BankAccount",
					"Bank_Reconciliation",
					"Contact",
					"Currency",
					"Employees",
					"Excel_Plus_Download",
					"Migration_Tool_Download",
					"ExchangeRate",
					"Financial_Reports",
					"Fixed_Assets",
					"Fixed_Assets_Reports",
					"General_Ledger",
					"Other_Reports",
					"Item",
					"MegiFapiao",
					"Org",
					"Purchases_Fapiao",
					"Report",
					"Role",
					"Sales_Fapiao",
					"Setting",
					"TaxRate",
					"Track",
					"TrackEntry",
					"User",
					"Voucher"
				};
			}
			return new List<string>();
		}
	}
}
