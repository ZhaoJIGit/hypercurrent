using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.DataModel.SEC;
using JieNor.Megi.DataRepository.SEC;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace JieNor.Megi.DataRepository.COM
{
    public static class COMAccess
    {
        public static MAccessResponseModel GetAccessResult(MContext ctx, List<MAccessRequestModel> requestList = null)
        {
            requestList = (requestList == null || requestList.Count == 0) ? GetAllAccessRequest(ctx) : requestList;
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
            List<string> list_result = new List<string>();
            //标准版
            List<string> list_basic = new List<string>
            {
                "Account",
                "AccountEdit",
                "Adviser",
                "Balances",
                "BankAccount",
                "Bank_Reconciliation",
                "Bank_Reports",
                "Bill",
                "Department",
                "Employees",
                "Excel_Plus_Download",
                "Migration_Tool_Download",
                "Expense",
                "ExpenseItem",
                "Expense_Reports",
                "Financial_Reports",
                "Fixed_Assets",
                "Fixed_Assets_Reports",
                "Other_Reports",
                "Invoice",
                "Invoice_Purchases",
                "Invoice_Sales",
                "MegiFapiao",
                "Payment",
                "PayRun",
                "PayRun_Reports",
                "Pay_Salary",
                "Purchases_Fapiao",
                "Purchase_Reports",
                "Receive",
                "Role",
                "Sale_Reports",
                "Track",
                "TrackEntry",
                "Transfer",
                "Transferbill",
                "Voucher",
                "ALL",
                "Attachment",
                "Attachment_Category",
                "General_Ledger",
                "Bank",
                "Contact",
                "Currency",
                "ExchangeRate",
                "Item",
                "Org",
                "Report",
                "Sales_Fapiao",
                "Setting",
                "TaxRate",
                "User"
            };
            //记账版
            List<string> list_smart = new List<string>
            {
                "Account",
                "AccountEdit",
                "Adviser",
                "Balances",
                "BankAccount",
                "Bank_Reconciliation",
                "Employees",
                "Excel_Plus_Download",
                "Migration_Tool_Download",
                "Financial_Reports",
                "Fixed_Assets",
                "Fixed_Assets_Reports",
                "Other_Reports",
                "MegiFapiao",
                "Purchases_Fapiao",
                "Role",
                "Track",
                "TrackEntry",
                "Voucher",
                "ALL",
                "Attachment",
                "Attachment_Category",
                "General_Ledger",
                "Bank",
                "Contact",
                "Currency",
                "ExchangeRate",
                "Item",
                "Org",
                "Report",
                "Sales_Fapiao",
                "Setting",
                "TaxRate",
                "User"
            };
            //公用
            List<string> list_common = new List<string>
            {
                "ALL",
                "Attachment",
                "Attachment_Category",
                "General_Ledger",
                "Bank",
                  "BankAccount",
                "Bank_Reconciliation",
                "Bank_Reports",
                "Contact",
                "Currency",
                "ExchangeRate",
                "Item",
                "Org",
                "Report",
                "Sales_Fapiao",
                "Setting",
                "TaxRate",
                "User"
            };
            //对应当前用户 方案模块
            List<PlanModel> list_pm = GetPlan(ctx);
            foreach (PlanModel pm in list_pm)
            {
                if (all || pm.Code == "NORMAL")
                {
                    list_result = new List<string>();
                    break;
                }
                if (pm.Code.ToUpper() == "SALES")
                {
                    list_result.AddRange(new List<string>
                    {
                        "Account",
                        "AccountEdit",
                        "Bill",
                        "Invoice",
                        "Invoice_Sales",
                        "Payment",
                        "Receive",
                        "Sale_Reports"
                    });
                }
                if (pm.Code.ToUpper() == "INVOICE")
                {
                    list_result.AddRange(new List<string>
                    {
                        "Account",
                        "AccountEdit",
                        "Bill",
                        "Invoice",
                        "Invoice_Sales",
                        "Payment",
                        "Receive",
                        "Sale_Reports"
                    });
                }
            }
            if (list_result.Count == 0)
            {
                //区分版本
                if (ctx.MOrgVersionID == 0) { list_result = list_basic; }
                else if (ctx.MOrgVersionID == 1) { list_result = list_smart; }
            }
            else
            {
                list_result.AddRange(list_common);
            }
            return list_result.Distinct().ToList();
        }
        /// <summary>
        /// 获取当前用户的方案
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static List<PlanModel> GetPlan(MContext ctx)
        {
            string str_sql = "select p.id,p.code,p.remark,pu.useremail from t_bas_plan p " +
                "join t_bas_planuser pu on p.id=pu.planid where pu.useremail='" + ctx.MEmail + "'";

            //DynamicDbHelperMySQL dbhelper = new DynamicDbHelperMySQL(ctx);
            //DataSet ds = dbhelper.Query(str_sql);
            DataSet ds = DbHelperMySQL.Query(str_sql);

            List<PlanModel> list_pm = new List<PlanModel>();
            DataTable dt = ds.Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                PlanModel pm = new PlanModel
                {
                    Id = Convert.ToInt32(item["id"]),
                    Code = Convert.ToString(item["code"]).ToUpper(),
                    Remark = Convert.ToString(item["remark"]),
                    UserEmail = Convert.ToString(item["useremail"])
                };
                list_pm.Add(pm);
            }
              
            if (list_pm.Count==0) {
                PlanModel pm = new PlanModel
                {
                    Id = 0,
                    Code = "NORMAL",
                    UserEmail = ctx.MEmail
                };
                list_pm.Add(pm);
            }

            return list_pm;
        }
        public static List<PlanModel> GetPlan(MContext ctx,string email)
        {
            string str_sql = "select p.id,p.code,p.remark,pu.useremail from t_bas_plan p " +
                "join t_bas_planuser pu on p.id=pu.planid where pu.useremail='" + email + "'";

            //DynamicDbHelperMySQL dbhelper = new DynamicDbHelperMySQL(ctx);
            //DataSet ds = dbhelper.Query(str_sql);
            DataSet ds = DbHelperMySQL.Query(str_sql);

            List<PlanModel> list_pm = new List<PlanModel>();
            DataTable dt = ds.Tables[0];
            foreach (DataRow item in dt.Rows)
            {
                PlanModel pm = new PlanModel
                {
                    Id = Convert.ToInt32(item["id"]),
                    Code = Convert.ToString(item["code"]).ToUpper(),
                    Remark = Convert.ToString(item["remark"]),
                    UserEmail = Convert.ToString(item["useremail"])
                };
                list_pm.Add(pm);
            }

            if (list_pm.Count == 0)
            {
                PlanModel pm = new PlanModel
                {
                    Id = 0,
                    Code = "NORMAL",
                    UserEmail = ctx.MEmail
                };
                list_pm.Add(pm);
            }

            return list_pm;
        }
    }
}
