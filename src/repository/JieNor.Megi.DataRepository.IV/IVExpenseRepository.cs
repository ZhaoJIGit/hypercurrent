using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.BD;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.IV.Expense;
using JieNor.Megi.DataRepository.BD;
using JieNor.Megi.DataRepository.COM;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.DataRepository.Log;
using JieNor.Megi.EntityModel;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.Enum;
using JieNor.Megi.EntityModel.MultiLanguage;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVExpenseRepository : IVBaseRepository<IVExpenseModel>
	{
		public static IVExpenseModel GetExpenseModel(MContext ctx, string pkID)
		{
			IVExpenseModel iVExpenseModel = ModelInfoManager.GetDataEditModel<IVExpenseModel>(ctx, pkID, false, true);
			if (iVExpenseModel == null)
			{
				iVExpenseModel = new IVExpenseModel();
			}
			return iVExpenseModel;
		}

		public static List<IVExpenseModel> GetExpenseModelIncludeEntry(MContext ctx, SqlWhere filter)
		{
			if (filter == null)
			{
				throw new NullReferenceException("filter can not be null");
			}
			return ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, filter, false, true);
		}

		public static OperationResult AddExpenseNoteLog(MContext ctx, IVExpenseModel model)
		{
			IVExpenseLogRepository.AddExpenseNoteLog(ctx, model);
			return new OperationResult
			{
				Success = true
			};
		}

		public static IVExpenseModel GetExpenseCopyModel(MContext ctx, string pkID)
		{
			IVExpenseModel model = ModelInfoManager.GetDataEditModel<IVExpenseModel>(ctx, pkID, false, true);
			if (model == null)
			{
				model = new IVExpenseModel();
			}
			model.MID = string.Empty;
			model.MStatus = Convert.ToInt32(IVInvoiceStatusEnum.Draft);
			List<BDCheckInactiveModel> bDInactiveList = GLDataPool.GetInstance(ctx, false, 0, 0, 0).BDInactiveList;
			if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == model.MEmployee && m.ObjectType == "Employees"))
			{
				model.MEmployee = "";
			}
			if (model.ExpenseEntry != null && model.ExpenseEntry.Count > 0)
			{
				foreach (IVExpenseEntryModel item in model.ExpenseEntry)
				{
					item.MEntryID = "";
					item.MID = "";
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MItemID && m.ObjectType == "ExpenseItem"))
					{
						item.MItemID = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem1 && m.ObjectType == "Track"))
					{
						item.MTrackItem1 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem2 && m.ObjectType == "Track"))
					{
						item.MTrackItem2 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem3 && m.ObjectType == "Track"))
					{
						item.MTrackItem3 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem4 && m.ObjectType == "Track"))
					{
						item.MTrackItem4 = "";
					}
					if (bDInactiveList.Any((BDCheckInactiveModel m) => m.MItemID == item.MTrackItem5 && m.ObjectType == "Track"))
					{
						item.MTrackItem5 = "";
					}
				}
			}
			return model;
		}

		public static List<IVExpenseModel> GetExpenseList(MContext ctx, List<string> keyIdList)
		{
			return ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, keyIdList);
		}

		public static List<IVExpenseModel> GetInitList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.*, c.MCurrencyID as MOrgCyID,b.MFirstName,b.MLastName,concat(b.MFirstName,b.MLastName) as MContactName,group_concat(DISTINCT d.MAttachID) as MAttachIDs from T_IV_Expense a ");
			stringBuilder.Append("left join T_IV_ExpenseEntry ae on a.MID=ae.MID AND ae.MIsDelete=0 and ae.MOrgID = a.MOrgID ");
			stringBuilder.Append("left join T_BD_Employees_L b on a.MEmployee=b.MParentID and b.MLocaleID=@MLocaleID  AND b.MIsDelete=0  and b.MOrgID = a.MOrgID ");
			stringBuilder.Append("left join T_REG_Financial c on a.MOrgID=c.MOrgID  AND c.MIsDelete=0   and c.MOrgID = a.MOrgID ");
			stringBuilder.Append("left join T_IV_ExpenseAttachment d on a.MID=d.MParentID  AND d.MIsDelete=0  and d.MOrgID = a.MOrgID  ");
			stringBuilder.Append(" WHERE a.MIsDelete = 0  AND a.MOrgID=@MOrgID ");
			stringBuilder.Append(" AND a.MBizDate<(select MConversionDate from t_bas_organisation where MItemID=@MOrgID) ");
			stringBuilder.Append(" GROUP BY a.MID");
			MySqlParameter[] array = new MySqlParameter[2]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			return ModelInfoManager.DataTableToList<IVExpenseModel>(dt);
		}

		public static DataGridJson<IVExpenseListModel> GetExpenseList(MContext ctx, IVExpenseListFilterModel filter, GetParam param = null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("select a.*, c.MCurrencyID as MOrgCyID,b.MFirstName,b.MLastName,concat(b.MFirstName,b.MLastName) as MContactName,group_concat(DISTINCT d.MAttachID) as MAttachIDs, f.MName as DepartmentName from T_IV_Expense a ");
			stringBuilder.Append("left join T_IV_ExpenseEntry ae on a.MID=ae.MID AND ae.MIsDelete=0 and ae.MOrgID = a.MOrgID ");
			stringBuilder.Append("left join T_BD_Employees_L b on a.MEmployee=b.MParentID and b.MLocaleID=@MLocaleID  AND b.MIsDelete=0  and b.MOrgID = a.MOrgID ");
			stringBuilder.Append("left join T_REG_Financial c on a.MOrgID=c.MOrgID  AND c.MIsDelete=0   and c.MOrgID = a.MOrgID ");
			stringBuilder.Append("left join T_IV_ExpenseAttachment d on a.MID=d.MParentID  AND d.MIsDelete=0  and d.MOrgID = a.MOrgID  ");
			stringBuilder.Append("left join T_BD_Department e on a.MDepartment=e.MItemID  AND e.MIsDelete=0  and e.MOrgID = a.MOrgID ");
			stringBuilder.Append("left join T_BD_Department_l f on e.MItemID=f.MParentID and f.MLocaleID=@MLocaleID  AND f.MIsDelete=0   and f.MOrgID = a.MOrgID ");
			stringBuilder.Append("and not exists(select 1 from T_IV_BankBillReconcile c where c.MBankBillEntryID=ae.MEntryID  AND c.MIsDelete=0  and c.MOrgID = a.MOrgID and c.MIsDelete = 0 ) ");
			stringBuilder.Append("where a.MIsDelete=0  and a.MOrgID=@MOrgID ");
			if (param != null)
			{
				if (param.ModifiedSince > DateTime.MinValue)
				{
					stringBuilder.Append(" and a.MModifyDate > @MModifyDate ");
				}
				if (!string.IsNullOrWhiteSpace(param.ElementID))
				{
					stringBuilder.Append(" and a.MID = @MID ");
				}
			}
			stringBuilder.Append(GetExpenseListSearchWhere(ctx, filter));
			stringBuilder.Append(" GROUP BY a.MID ");
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			sqlQuery.AddParameter(new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MOrgID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6)
			{
				Value = ctx.MLCID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36)
			{
				Value = ctx.MUserID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MType", MySqlDbType.VarChar, 36)
			{
				Value = filter.MType
			});
			sqlQuery.AddParameter(new MySqlParameter("@MStatus", MySqlDbType.Int32, 2)
			{
				Value = (object)filter.MStatus
			});
			sqlQuery.AddParameter(new MySqlParameter("@MKeyword", MySqlDbType.VarChar, 500)
			{
				Value = (string.IsNullOrWhiteSpace(filter.Keyword) ? filter.Keyword : filter.Keyword.Replace("\\", "\\\\"))
			});
			sqlQuery.AddParameter(new MySqlParameter("@MContactID", MySqlDbType.VarChar, 200)
			{
				Value = filter.MContactID
			});
			sqlQuery.AddParameter(new MySqlParameter("@MStartDate", MySqlDbType.DateTime)
			{
				Value = (object)filter.MStartDate
			});
			sqlQuery.AddParameter(new MySqlParameter("@MEndDate", MySqlDbType.DateTime)
			{
				Value = (object)filter.MEndDate
			});
			sqlQuery.AddParameter(new MySqlParameter("@MTrackItem1", MySqlDbType.VarChar, 36)
			{
				Value = filter.MTrackItem1
			});
			sqlQuery.AddParameter(new MySqlParameter("@MTrackItem2", MySqlDbType.VarChar, 36)
			{
				Value = filter.MTrackItem2
			});
			sqlQuery.AddParameter(new MySqlParameter("@MTrackItem3", MySqlDbType.VarChar, 36)
			{
				Value = filter.MTrackItem3
			});
			sqlQuery.AddParameter(new MySqlParameter("@MTrackItem4", MySqlDbType.VarChar, 36)
			{
				Value = filter.MTrackItem4
			});
			sqlQuery.AddParameter(new MySqlParameter("@MTrackItem5", MySqlDbType.VarChar, 36)
			{
				Value = filter.MTrackItem5
			});
			if (param != null)
			{
				sqlQuery.AddParameter(new MySqlParameter("@MModifyDate", MySqlDbType.DateTime)
				{
					Value = (object)param.ModifiedSince
				});
				sqlQuery.AddParameter(new MySqlParameter("@MID", MySqlDbType.VarChar, 36)
				{
					Value = param.ElementID
				});
			}
			if (string.IsNullOrEmpty(filter.Sort))
			{
				sqlQuery.OrderBy(" MBizDate DESC");
			}
			else if (filter.Sort == "MContactName")
			{
				sqlQuery.OrderBy($" CONVERT(MContactName USING gbk) {filter.Order}");
			}
			else
			{
				sqlQuery.OrderBy($" {filter.Sort} {filter.Order}");
			}
			return ModelInfoManager.GetPageDataModelListBySql<IVExpenseListModel>(ctx, sqlQuery);
		}

		public static IVExpenseModel GetNextUnApproveExpenseID(MContext ctx, DateTime dt)
		{
			if (dt.Year <= 1900)
			{
				dt = DateTime.Now;
			}
			string sql = "SELECT * FROM T_IV_Expense \r\n                           WHERE MOrgID=@MOrgID AND MIsDelete=0 AND MCreateDate < @CreateDate AND (MStatus=@Draft or MStatus=@WaitingApproval)\r\n                           order by MCreateDate desc limit 0,1 ";
			MySqlParameter[] cmdParms = new MySqlParameter[4]
			{
				new MySqlParameter("@CreateDate", dt),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@Draft", Convert.ToInt32(IVExpenseStatusEnum.Draft)),
				new MySqlParameter("@WaitingApproval", Convert.ToInt32(IVExpenseStatusEnum.WaitingApproval))
			};
			return ModelInfoManager.GetDataModel<IVExpenseModel>(ctx, sql, cmdParms);
		}

		public static OperationResult UpdateExpense(MContext ctx, IVExpenseModel model)
		{
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, model.MID);
			if (model.MStatus >= Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment) && !operationResult.Success)
			{
				return operationResult;
			}
			UpdateExpenseEntryAccount(ctx, model);
			model.MContactID = model.MEmployee;
			AdjustTailDifferentAmount(model);
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVExpenseModel>(ctx, model, null, true));
			OperationResult operationResult2 = GLInterfaceRepository.GenerateVouchersByBill(ctx, model, null);
			if (operationResult2.Success)
			{
				list.AddRange(operationResult2.OperationCommands);
				list.AddRange(IVExpenseLogHelper.GetSaveLogCmd(ctx, model));
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
				return new OperationResult
				{
					Success = (num > 0),
					ObjectID = model.MID
				};
			}
			return operationResult2;
		}

		private static void UpdateExpenseEntryAccount(MContext ctx, IVExpenseModel model)
		{
			if (model.ExpenseEntry != null && model.ExpenseEntry.Count != 0)
			{
				List<string> list = (from t in model.ExpenseEntry
				select t.MItemID).Distinct().ToList();
				if (list != null && list.Count != 0)
				{
					BDExpenseItemRepository bDExpenseItemRepository = new BDExpenseItemRepository();
					List<BDExpenseItemModel> expenseItemList = bDExpenseItemRepository.GetExpenseItemList(ctx, list);
					if (expenseItemList != null && expenseItemList.Count != 0)
					{
						int i;
						for (i = 0; i < model.ExpenseEntry.Count; i++)
						{
							string mAcctID = "";
							string mItemID = model.ExpenseEntry[i].MItemID;
							if (!string.IsNullOrEmpty(mItemID))
							{
								BDExpenseItemModel bDExpenseItemModel = (from t in expenseItemList
								where t.MItemID == model.ExpenseEntry[i].MItemID
								select t).FirstOrDefault();
								mAcctID = ((bDExpenseItemModel == null) ? "" : bDExpenseItemModel.MAccountId);
							}
							model.ExpenseEntry[i].MAcctID = mAcctID;
						}
					}
				}
			}
		}

		public static OperationResult UnApproveExpense(MContext ctx, string expenseId)
		{
			OperationResult operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, expenseId);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			IVExpenseModel dataEditModel = ModelInfoManager.GetDataEditModel<IVExpenseModel>(ctx, expenseId, false, true);
			List<CommandInfo> list = new List<CommandInfo>();
			List<CommandInfo> list2 = list;
			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = "UPDATE T_IV_Expense SET MStatus = @MStatus WHERE MID = @MID AND MOrgID=@MOrgID and MIsDelete = 0 "
			};
			DbParameter[] array = commandInfo.Parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MStatus", Convert.ToInt32(IVExpenseStatusEnum.Draft)),
				new MySqlParameter("@MID", expenseId),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			list2.Add(commandInfo);
			RecordStatus status = RecordStatus.Draft;
			OperationResult operationResult2 = GLInterfaceRepository.TransferBillCreatedVouchersByStatus(ctx, expenseId, status);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			list.AddRange(operationResult2.OperationCommands);
			list.AddRange(IVExpenseLogHelper.GetUnApproveLogCmd(ctx, dataEditModel));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			int num = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			return new OperationResult
			{
				Success = (num > 0),
				ObjectID = expenseId
			};
		}

		public static OperationResult ApproveExpense(MContext ctx, ParamBase param)
		{
			string[] source = param.KeyIDSWithNoSingleQuote.Split(',');
			List<string> list = source.ToList();
			List<CommandInfo> list2 = new List<CommandInfo>();
			string empty = string.Empty;
			bool success = true;
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, list);
			if (!operationResult.Success)
			{
				return operationResult;
			}
			List<IVExpenseModel> modelList = GetModelList(ctx, list);
			if (!ValidateEmployeeEmpty(ctx, modelList, operationResult))
			{
				return operationResult;
			}
			List<MySqlParameter> list3 = new List<MySqlParameter>
			{
				new MySqlParameter("@MStatus", Convert.ToInt32(IVExpenseStatusEnum.WaitingPayment)),
				new MySqlParameter("@Draft", Convert.ToInt32(IVExpenseStatusEnum.Draft)),
				new MySqlParameter("@WaitingApproval", Convert.ToInt32(IVExpenseStatusEnum.WaitingApproval)),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			string inFilterQuery = GLUtility.GetInFilterQuery(list, ref list3, "M_ID");
			List<CommandInfo> list4 = list2;
			CommandInfo obj = new CommandInfo
			{
				CommandText = "UPDATE T_IV_Expense SET MStatus = @MStatus WHERE MID " + inFilterQuery + "  AND MOrgID=@MOrgID and MIsDelete = 0 AND (MStatus =@Draft OR MStatus=@WaitingApproval) "
			};
			DbParameter[] array = obj.Parameters = list3.ToArray();
			list4.Add(obj);
			foreach (IVExpenseModel item in modelList)
			{
				list2.AddRange(IVExpenseLogHelper.GetApproveLogCmd(ctx, item));
			}
			RecordStatus status = RecordStatus.Saved;
			OperationResult operationResult2 = GLInterfaceRepository.TransferBillsCreatedVouchersByStatus(ctx, (from x in modelList
			select x.MID).Distinct().ToList(), status);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			list2.AddRange(operationResult2.OperationCommands);
			if (list2.Count == 0)
			{
				return new OperationResult
				{
					Success = success,
					Message = empty
				};
			}
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			operationResult.Success = (dynamicDbHelperMySQL.ExecuteSqlTran(list2) > 0);
			return operationResult;
		}

		private static bool ValidateEmployeeEmpty(MContext ctx, List<IVExpenseModel> modelList, OperationResult result)
		{
			IEnumerable<IVExpenseModel> enumerable = from f in modelList
			where string.IsNullOrWhiteSpace(f.MEmployee)
			select f;
			if (enumerable.Any())
			{
				string text = COMMultiLangRepository.GetText(ctx.MLCID, LangModule.IV, "EmpNotExistCantApprovExpense", "以下费用报销单的员工不存在，不能进行审核：");
				result.VerificationInfor.Add(new BizVerificationInfor
				{
					Level = AlertEnum.Error,
					Message = text
				});
				List<string> list = new List<string>();
				foreach (IVExpenseModel item in enumerable)
				{
					string str = string.Join(" ", item.MReference, item.MBizDate.ToOrgZoneDateFormat(null)).Trim();
					list.Add("- " + str);
				}
				result.Tag = string.Join(Environment.NewLine, list);
				result.Success = false;
			}
			return result.Success;
		}

		public static OperationResult UpdateExpenseStatus(MContext ctx, ParamBase param)
		{
			string[] array = param.KeyIDSWithSingleQuote.Replace("'", "").Split(',');
			List<CommandInfo> list = new List<CommandInfo>();
			OperationResult operationResult = new OperationResult
			{
				Success = true
			};
			bool flag = true;
			List<IVExpenseModel> list2 = new List<IVExpenseModel>();
			string[] array2 = array;
			int num;
			foreach (string text in array2)
			{
				operationResult = GLInterfaceRepository.IsDocCanOperate(ctx, text);
				if (!operationResult.Success)
				{
					return operationResult;
				}
				List<CommandInfo> list3 = list;
				CommandInfo commandInfo = new CommandInfo
				{
					CommandText = "UPDATE T_IV_Expense SET MStatus = @MStatus WHERE MID = @MID AND MOrgID=@MOrgID and MIsDelete = 0 "
				};
				DbParameter[] array3 = commandInfo.Parameters = new MySqlParameter[3]
				{
					new MySqlParameter("@MStatus", param.MOperationID),
					new MySqlParameter("@MID", text),
					new MySqlParameter("@MOrgID", ctx.MOrgID)
				};
				list3.Add(commandInfo);
				IVExpenseModel expenseModel = GetExpenseModel(ctx, text);
				if (param.MOperationID == "1")
				{
					list.AddRange(IVExpenseLogHelper.GetUnApproveLogCmd(ctx, expenseModel));
				}
				else if (param.MOperationID == "3")
				{
					list.AddRange(IVExpenseLogHelper.GetApproveLogCmd(ctx, expenseModel));
				}
				list2.Add(expenseModel);
				string mOperationID = param.MOperationID;
				num = Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval);
				if (mOperationID == num.ToString())
				{
					list.AddRange(IVExpenseLogHelper.GetSubmitForApprovalLogCmd(ctx, expenseModel));
				}
			}
			string mOperationID2 = param.MOperationID;
			num = Convert.ToInt32(IVInvoiceStatusEnum.Draft);
			int num2;
			if (!(mOperationID2 == num.ToString()))
			{
				string mOperationID3 = param.MOperationID;
				num = Convert.ToInt32(IVInvoiceStatusEnum.WaitingApproval);
				if (!(mOperationID3 == num.ToString()))
				{
					num2 = 0;
					goto IL_01cd;
				}
			}
			num2 = -1;
			goto IL_01cd;
			IL_01cd:
			RecordStatus status = (RecordStatus)num2;
			OperationResult operationResult2 = GLInterfaceRepository.TransferBillsCreatedVouchersByStatus(ctx, (from x in list2
			select x.MID).Distinct().ToList(), status);
			if (!operationResult2.Success)
			{
				return operationResult2;
			}
			list.AddRange(operationResult2.OperationCommands);
			if (list.Count != 0 & flag)
			{
				DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
				int num3 = dynamicDbHelperMySQL.ExecuteSqlTran(list);
			}
			return operationResult;
		}

		public static void UpdateExpenseStatus(MContext context, string ExpenseId, IVInvoiceStatusEnum status)
		{
			string sql = "UPDATE T_IV_Expense SET MStatus = @MStatus WHERE MID =@MID AND MOrgID=@MOrgID and MIsDelete = 0 ";
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MStatus", MySqlDbType.Int32, 2),
				new MySqlParameter("@MOrgID", context.MOrgID)
			};
			array[0].Value = ExpenseId;
			array[1].Value = Convert.ToInt32(status);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(context);
			dynamicDbHelperMySQL.ExecuteSql(sql, array);
		}

		public static OperationResult DeleteExpenseList(MContext ctx, ParamBase param)
		{
			OperationResult operationResult = IVBaseRepository<IVExpenseModel>.DeleteBill<IVExpenseModel>(ctx, param);
			if (operationResult.Success)
			{
				IVExpenseLogRepository.AddExpenseDeleteLog(ctx, param);
			}
			return operationResult;
		}

		public static List<IVExpenseModel> GetModelList(MContext ctx, ParamBase param)
		{
			string[] values = param.KeyIDs.Split(',');
			SqlWhere sqlWhere = new SqlWhere();
			sqlWhere.AddFilter("MID", SqlOperators.In, values);
			return ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, sqlWhere, false, false);
		}

		public static List<IVExpenseModel> GetModelList(MContext ctx, List<string> pkIDs)
		{
			return ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, pkIDs);
		}

		public static List<IVExpenseModel> GetExpenseListIncludeEntry(MContext ctx, SqlWhere filter)
		{
			return ModelInfoManager.GetDataModelList<IVExpenseModel>(ctx, filter, false, true);
		}

		public static IVExpenseSummaryModel GetExpenseSummaryModel(MContext ctx, DateTime startDate, DateTime endDate)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT SUM(1) as AllCount, SUM(MTaxTotalAmt) as AllAmount,\r\n\t                    SUM(CASE WHEN MStatus = 1 THEN 1 ELSE 0 END) AS DraftCount,SUM(CASE WHEN MStatus = 1 THEN MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS DraftAmount,\r\n\t                    SUM(CASE WHEN MStatus = 2 THEN 1 ELSE 0 END) AS WaitingApprovalCount,SUM(CASE WHEN MStatus = 2 THEN  MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS WaitingApprovalAmount,\r\n\t                    SUM(CASE WHEN MStatus = 3 THEN 1 ELSE 0 END) AS WaitingPaymentCount,SUM(CASE WHEN MStatus = 3 THEN  MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS WaitingPaymentAmount,\r\n                        SUM(CASE WHEN (MStatus = 4  OR (MStatus=3 AND ifnull(MVerifyAmtFor,0)<>0)) THEN 1 ELSE 0 END) AS PaidCount,\r\n                        SUM(CASE WHEN (MStatus = 4  OR (MStatus=3 AND ifnull(MVerifyAmtFor,0)<>0)) THEN MVerifyAmt ELSE 0 END) AS PaidAmount,\r\n                        SUM(CASE WHEN MStatus = 3 AND MDueDate <= @MNow then  1 ELSE 0 END) AS DueCount,\r\n                        SUM(CASE WHEN MStatus = 3 AND MDueDate <= @MNow then  MTaxTotalAmt-MVerifyAmt ELSE 0 END) AS DueAmount\r\n                        FROM T_IV_Expense \r\n                        WHERE MIsDelete = 0 AND MOrgID=@MOrgID and MBizDate >=@StartDate AND MBizDate <= @EndDate ");
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND MCreatorID=@MCreatorID ");
			}
			MySqlParameter[] array = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@StartDate", MySqlDbType.DateTime),
				new MySqlParameter("@EndDate", MySqlDbType.DateTime),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MNow", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = startDate;
			array[2].Value = endDate;
			array[3].Value = ctx.MUserID;
			MySqlParameter obj = array[4];
			DateTime dateTime = ctx.DateNow;
			dateTime = dateTime.Date;
			dateTime = dateTime.AddDays(1.0);
			obj.Value = dateTime.AddSeconds(-1.0);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dataTable = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			IVExpenseSummaryModel iVExpenseSummaryModel = new IVExpenseSummaryModel();
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				return iVExpenseSummaryModel;
			}
			DataRow dataRow = dataTable.Rows[0];
			iVExpenseSummaryModel.AllCount = dataRow["AllCount"].ToMInt32();
			iVExpenseSummaryModel.DraftCount = dataRow["DraftCount"].ToMInt32();
			iVExpenseSummaryModel.DraftAmount = dataRow["DraftAmount"].ToMDecimal();
			iVExpenseSummaryModel.WaitingApprovalCount = dataRow["WaitingApprovalCount"].ToMInt32();
			iVExpenseSummaryModel.WaitingApprovalAmount = dataRow["WaitingApprovalAmount"].ToMDecimal();
			iVExpenseSummaryModel.WaitingPaymentCount = dataRow["WaitingPaymentCount"].ToMInt32();
			iVExpenseSummaryModel.WaitingPaymentAmount = dataRow["WaitingPaymentAmount"].ToMDecimal();
			iVExpenseSummaryModel.PaidCount = dataRow["PaidCount"].ToMInt32();
			iVExpenseSummaryModel.PaidAmount = dataRow["PaidAmount"].ToMDecimal();
			iVExpenseSummaryModel.AllAmount = iVExpenseSummaryModel.DraftAmount + iVExpenseSummaryModel.WaitingApprovalAmount + iVExpenseSummaryModel.WaitingPaymentAmount + iVExpenseSummaryModel.PaidAmount;
			iVExpenseSummaryModel.DueCount = dataRow["DueCount"].ToMInt32();
			iVExpenseSummaryModel.DueAmount = dataRow["DueAmount"].ToMDecimal();
			return iVExpenseSummaryModel;
		}

		public static List<CommandInfo> GetImportExpenseCmdList(MContext ctx, List<IVExpenseModel> list)
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			int num = 0;
			DateTime dateNow = ctx.DateNow;
			foreach (IVExpenseModel item in list)
			{
				item.MCreateDate = dateNow.AddSeconds((double)(-num));
				AdjustTailDifferentAmount(item);
				list2.AddRange(IVExpenseLogHelper.GetSaveLogCmd(ctx, item));
				num++;
			}
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBills(ctx, list, null);
			if (operationResult.Success)
			{
				list2.AddRange(operationResult.OperationCommands);
			}
			list2.AddRange(GetNewAndUpdateExpenseCmdList(ctx, list));
			return list2;
		}

		private static void AdjustTailDifferentAmount(IVExpenseModel model)
		{
			if (model.MExchangeRate != decimal.One)
			{
				foreach (IVExpenseEntryModel item in model.ExpenseEntry)
				{
					decimal num = item.MTaxAmount - item.MAmount - item.MTaxAmt;
					if (Math.Abs(num) > decimal.Zero)
					{
						IVExpenseEntryModel iVExpenseEntryModel = item;
						iVExpenseEntryModel.MAmount += num;
						IVExpenseEntryModel iVExpenseEntryModel2 = item;
						iVExpenseEntryModel2.MApproveAmt += num;
					}
				}
				model.MTotalAmt = model.ExpenseEntry.Sum((IVExpenseEntryModel f) => f.MAmount);
			}
		}

		public static List<CommandInfo> GetNewAndUpdateExpenseCmdList(MContext ctx, List<IVExpenseModel> modelList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			List<IVExpenseModel> list2 = (from m in modelList
			where m.IsNew
			select m).ToList();
			if (list2.Count > 0)
			{
				list.AddRange(GetNewExpenseCmdList(ctx, list2));
			}
			List<IVExpenseModel> list3 = (from m in modelList
			where !m.IsNew
			select m).ToList();
			if (list3.Count > 0)
			{
				list.AddRange(GetUpdateExpenseCmdList(ctx, list3, list2.Count));
			}
			return list;
		}

		public static List<CommandInfo> GetUpdateExpenseCmdList(MContext ctx, List<IVExpenseModel> modelList, int beginIndex = 0)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = " UPDATE t_iv_expense ";
			string format = " SET MID = @MID{0},MOrgID= @MOrgID{0},MType=@MType{0},MEmployee=@MEmployee{0},MDepartment=@MDepartment{0},MContactID=@MContactID{0},\r\n                                MNumber=@MNumber{0},MBizDate=@MBizDate{0},MDueDate=@MDueDate{0},MExpectedDate=@MExpectedDate{0},MAttachCount=@MAttachCount{0},MTaxID=@MTaxID{0},MCyID=@MCyID{0},\r\n                                MExchangeRate=@MExchangeRate{0},MOToLRate=@MOToLRate{0},MLToORate=@MLToORate{0},MTotalAmtFor=@MTotalAmtFor{0},MTotalAmt=@MTotalAmt{0},MTaxTotalAmtFor=@MTaxTotalAmtFor{0},\r\n                                MTaxTotalAmt=@MTaxTotalAmt{0},MVerificationAmt=@MVerificationAmt{0},MStatus=@MStatus{0},MReference=@MReference{0},MDesc=@MDesc{0},MIsDelete=0,MCreatorID=@MCreatorID{0},\r\n                                MCreateDate=@MCreateDate{0},MModifierID=@MModifierID{0},MModifyDate=@MModifyDate{0}   WHERE MID = @MID{0} ";
			string value2 = " UPDATE t_iv_expenseentry ";
			string format2 = " SET MEntryID=@MEntryID{0},MID=@MID{0},MSeq=@MSeq{0},MItemID=@MItemID{0},MAcctID=@MAcctID{0},MTaxID=@MTaxID{0},\r\n                                MTrackItem1=@MTrackItem1{0},MTrackItem2=@MTrackItem2{0},MTrackItem3=@MTrackItem3{0},MTrackItem4=@MTrackItem4{0},MTrackItem5=@MTrackItem5{0},\r\n                                MQty=@MQty{0},MPrice=@MPrice{0},MDiscount=@MDiscount{0},MAmountFor=@MAmountFor{0},MAmount=@MAmount{0},MTaxAmountFor=@MTaxAmountFor{0},\r\n                                MTaxAmount=@MTaxAmount{0},MTaxAmtFor=@MTaxAmtFor{0},MTaxAmt=@MTaxAmt{0},MDesc=@MDesc{0},MApproveAmtFor=@MApproveAmtFor{0},MApproveAmt=@MApproveAmt{0},MOrgID=@MOrgID{0},\r\n                                MIsDelete=0,MCreatorID=@MCreatorID{0},MCreateDate=@MCreateDate{0},MModifierID=@MModifierID{0},MModifyDate=@MModifyDate{0} WHERE MEntryID=@MEntryID{0} ";
			StringBuilder stringBuilder = new StringBuilder(1000);
			StringBuilder stringBuilder2 = new StringBuilder(1000);
			stringBuilder.Append(value);
			stringBuilder2.Append(value2);
			int num = beginIndex;
			int num2 = beginIndex;
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			List<MySqlParameter> list3 = new List<MySqlParameter>();
			foreach (IVExpenseModel model in modelList)
			{
				if (num > beginIndex)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				stringBuilder.AppendFormat(format, num);
				list2.AddRange(GetNewExpenseParams(ctx, num, model));
				foreach (IVExpenseEntryModel item in model.ExpenseEntry)
				{
					if (num2 > beginIndex)
					{
						stringBuilder2.Append(" UNION ALL ");
					}
					stringBuilder2.AppendFormat(format2, num2);
					list3.AddRange(GetNewExpenseEntryParams(ctx, num2, model, item));
					num2++;
				}
				num++;
			}
			List<CommandInfo> list4 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list4.Add(obj);
			List<CommandInfo> list5 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = stringBuilder2.ToString()
			};
			array = (obj2.Parameters = list3.ToArray());
			list5.Add(obj2);
			return list;
		}

		public static List<CommandInfo> GetNewExpenseCmdList(MContext ctx, List<IVExpenseModel> modelList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = "INSERT INTO t_iv_expense(MID,MOrgID,MType,MEmployee,MDepartment,MContactID,MNumber,MBizDate,MDueDate,MExpectedDate,MAttachCount,MTaxID,MCyID,MExchangeRate,MOToLRate,MLToORate,MTotalAmtFor,MTotalAmt,MTaxTotalAmtFor,MTaxTotalAmt,MVerificationAmt,MStatus,MReference,MDesc,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate)";
			string format = " SELECT @MID{0},@MOrgID{0},@MType{0},@MEmployee{0},@MDepartment{0},@MContactID{0},@MNumber{0},@MBizDate{0},@MDueDate{0},@MExpectedDate{0},@MAttachCount{0},@MTaxID{0},@MCyID{0},@MExchangeRate{0},@MOToLRate{0},@MLToORate{0},@MTotalAmtFor{0},@MTotalAmt{0},@MTaxTotalAmtFor{0},@MTaxTotalAmt{0},@MVerificationAmt{0},@MStatus{0},@MReference{0},@MDesc{0},0,@MCreatorID{0},@MCreateDate{0},@MModifierID{0},@MModifyDate{0}";
			string value2 = "INSERT INTO t_iv_expenseentry(MEntryID,MID,MSeq,MItemID,MAcctID,MTaxID,MTrackItem1,MTrackItem2,MTrackItem3,MTrackItem4,MTrackItem5,MQty,MPrice,MDiscount,MAmountFor,MAmount,MTaxAmountFor,MTaxAmount,MTaxAmtFor,MTaxAmt,MDesc,MApproveAmtFor,MApproveAmt,MOrgID,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate)";
			string format2 = "SELECT @MEntryID{0},@MID{0},@MSeq{0},@MItemID{0},@MAcctID{0},@MTaxID{0},@MTrackItem1{0},@MTrackItem2{0},@MTrackItem3{0},@MTrackItem4{0},@MTrackItem5{0},@MQty{0},@MPrice{0},@MDiscount{0},@MAmountFor{0},@MAmount{0},@MTaxAmountFor{0},@MTaxAmount{0},@MTaxAmtFor{0},@MTaxAmt{0},@MDesc{0},@MApproveAmtFor{0},@MApproveAmt{0},@MOrgID{0},0,@MCreatorID{0},@MCreateDate{0},@MModifierID{0},@MModifyDate{0}";
			StringBuilder stringBuilder = new StringBuilder(1000);
			StringBuilder stringBuilder2 = new StringBuilder(1000);
			stringBuilder.Append(value);
			stringBuilder2.Append(value2);
			int num = 0;
			int num2 = 0;
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			List<MySqlParameter> list3 = new List<MySqlParameter>();
			foreach (IVExpenseModel model in modelList)
			{
				if (num > 0)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				stringBuilder.AppendFormat(format, num);
				list2.AddRange(GetNewExpenseParams(ctx, num, model));
				foreach (IVExpenseEntryModel item in model.ExpenseEntry)
				{
					if (num2 > 0)
					{
						stringBuilder2.Append(" UNION ALL ");
					}
					stringBuilder2.AppendFormat(format2, num2);
					list3.AddRange(GetNewExpenseEntryParams(ctx, num2, model, item));
					num2++;
				}
				num++;
			}
			List<CommandInfo> list4 = list;
			CommandInfo obj = new CommandInfo
			{
				CommandText = stringBuilder.ToString()
			};
			DbParameter[] array = obj.Parameters = list2.ToArray();
			list4.Add(obj);
			List<CommandInfo> list5 = list;
			CommandInfo obj2 = new CommandInfo
			{
				CommandText = stringBuilder2.ToString()
			};
			array = (obj2.Parameters = list3.ToArray());
			list5.Add(obj2);
			return list;
		}

		private static List<MySqlParameter> GetNewExpenseParams(MContext ctx, int i, IVExpenseModel model)
		{
			return new List<MySqlParameter>
			{
				new MySqlParameter($"@MID{i}", model.MID),
				new MySqlParameter($"@MOrgID{i}", ctx.MOrgID),
				new MySqlParameter($"@MType{i}", "Expense_Claims"),
				new MySqlParameter($"@MEmployee{i}", model.MEmployee),
				new MySqlParameter($"@MDepartment{i}", model.MDepartment),
				new MySqlParameter($"@MContactID{i}", model.MContactID),
				new MySqlParameter($"@MNumber{i}", model.MNumber),
				new MySqlParameter($"@MBizDate{i}", model.MBizDate),
				new MySqlParameter($"@MDueDate{i}", model.MDueDate),
				new MySqlParameter($"@MExpectedDate{i}", model.MExpectedDate),
				new MySqlParameter($"@MAttachCount{i}", model.MAttachCount),
				new MySqlParameter($"@MTaxID{i}", model.MTaxID),
				new MySqlParameter($"@MCyID{i}", model.MCyID),
				new MySqlParameter($"@MExchangeRate{i}", model.MExchangeRate),
				new MySqlParameter($"@MOToLRate{i}", model.MOToLRate),
				new MySqlParameter($"@MLToORate{i}", model.MLToORate),
				new MySqlParameter($"@MTotalAmtFor{i}", model.MTotalAmtFor),
				new MySqlParameter($"@MTotalAmt{i}", model.MTotalAmt),
				new MySqlParameter($"@MTaxTotalAmtFor{i}", model.MTaxTotalAmtFor),
				new MySqlParameter($"@MTaxTotalAmt{i}", model.MTaxTotalAmt),
				new MySqlParameter($"@MVerificationAmt{i}", model.MVerificationAmt),
				new MySqlParameter($"@MStatus{i}", model.MStatus),
				new MySqlParameter($"@MReference{i}", model.MReference),
				new MySqlParameter($"@MDesc{i}", model.MDesc),
				new MySqlParameter($"@MIsDelete{i}", MySqlDbType.Decimal),
				new MySqlParameter($"@MCreatorID{i}", ctx.MUserID),
				new MySqlParameter($"@MCreateDate{i}", model.MCreateDate),
				new MySqlParameter($"@MModifierID{i}", ctx.MUserID),
				new MySqlParameter($"@MModifyDate{i}", ctx.DateNow)
			};
		}

		private static List<MySqlParameter> GetNewExpenseEntryParams(MContext ctx, int i, IVExpenseModel model, IVExpenseEntryModel entry)
		{
			return new List<MySqlParameter>
			{
				new MySqlParameter($"@MEntryID{i}", entry.MEntryID),
				new MySqlParameter($"@MID{i}", model.MID),
				new MySqlParameter($"@MSeq{i}", entry.MSeq),
				new MySqlParameter($"@MItemID{i}", entry.MItemID),
				new MySqlParameter($"@MAcctID{i}", entry.MAcctID),
				new MySqlParameter($"@MTaxID{i}", entry.MTaxID),
				new MySqlParameter($"@MTrackItem1{i}", entry.MTrackItem1),
				new MySqlParameter($"@MTrackItem2{i}", entry.MTrackItem2),
				new MySqlParameter($"@MTrackItem3{i}", entry.MTrackItem3),
				new MySqlParameter($"@MTrackItem4{i}", entry.MTrackItem4),
				new MySqlParameter($"@MTrackItem5{i}", entry.MTrackItem5),
				new MySqlParameter($"@MQty{i}", entry.MQty),
				new MySqlParameter($"@MPrice{i}", entry.MPrice),
				new MySqlParameter($"@MDiscount{i}", entry.MDiscount),
				new MySqlParameter($"@MAmountFor{i}", entry.MAmountFor),
				new MySqlParameter($"@MAmount{i}", entry.MAmount),
				new MySqlParameter($"@MTaxAmountFor{i}", entry.MTaxAmountFor),
				new MySqlParameter($"@MTaxAmount{i}", entry.MTaxAmount),
				new MySqlParameter($"@MTaxAmtFor{i}", entry.MTaxAmtFor),
				new MySqlParameter($"@MTaxAmt{i}", entry.MTaxAmt),
				new MySqlParameter($"@MDesc{i}", entry.MDesc),
				new MySqlParameter($"@MApproveAmtFor{i}", entry.MApproveAmtFor),
				new MySqlParameter($"@MApproveAmt{i}", entry.MApproveAmt),
				new MySqlParameter($"@MOrgID{i}", ctx.MOrgID),
				new MySqlParameter($"@MIsDelete{i}", MySqlDbType.Decimal),
				new MySqlParameter($"@MCreatorID{i}", ctx.MUserID),
				new MySqlParameter($"@MCreateDate{i}", model.MCreateDate),
				new MySqlParameter($"@MModifierID{i}", ctx.MUserID),
				new MySqlParameter($"@MModifyDate{i}", ctx.DateNow)
			};
		}

		private static string GetExpenseListSearchWhere(MContext ctx, IVExpenseListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (filter.MStatus > 0)
			{
				if (filter.MStatus == 4)
				{
					stringBuilder.Append(" AND (MStatus = 4  OR (MStatus=3 AND ifnull(MVerificationAmt,0)<>0)) ");
				}
				else
				{
					stringBuilder.Append(" AND a.MStatus = @MStatus ");
				}
			}
			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				stringBuilder.Append(" AND (a.MReference LIKE concat('%',@MKeyword,'%') OR a.MTaxTotalAmtFor LIKE concat('%',@MKeyword,'%') OR F_GetUserName(b.MFirstName,b.MLastName) LIKE concat('%',@MKeyword,'%')) ");
			}
			if (!string.IsNullOrEmpty(filter.MContactID))
			{
				stringBuilder.Append(" AND b.MParentID=@MContactID ");
			}
			DateTime dateTime = new DateTime(1900, 1, 1);
			DateTime value;
			if (filter.MEndDate.HasValue && filter.MEndDate.Value > dateTime)
			{
				value = filter.MEndDate.Value;
				filter.MEndDate = value.AddDays(1.0);
			}
			if (ctx.IsSelfData)
			{
				stringBuilder.Append(" AND a.MCreatorID=@MCreatorID ");
			}
			switch (filter.MSearchWithin)
			{
			case IVInvoiceSearchWithinEnum.AnyDate:
			{
				DateTime? mStartDate = filter.MStartDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND (a.MBizDate>=@MStartDate OR a.MDueDate>=@MStartDate OR a.MExpectedDate>=@MStartDate) ");
				}
				mStartDate = filter.MEndDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND (a.MBizDate< @MEndDate OR a.MDueDate<@MEndDate OR a.MExpectedDate<@MStartDate) ");
				}
				break;
			}
			case IVInvoiceSearchWithinEnum.DueDate:
			{
				DateTime? mStartDate = filter.MStartDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MDueDate>=@MStartDate ");
				}
				mStartDate = filter.MEndDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MDueDate<@MEndDate ");
				}
				break;
			}
			case IVInvoiceSearchWithinEnum.TransactionDate:
			{
				DateTime? mStartDate = filter.MStartDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MBizDate>=@MStartDate  ");
				}
				mStartDate = filter.MEndDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MBizDate< @MEndDate ");
				}
				break;
			}
			case IVInvoiceSearchWithinEnum.ExpectedDate:
			{
				DateTime? mStartDate = filter.MStartDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MExpectedDate>=@MStartDate  ");
				}
				mStartDate = filter.MEndDate;
				value = dateTime;
				if (mStartDate > (DateTime?)value)
				{
					stringBuilder.Append(" AND a.MExpectedDate< @MEndDate ");
				}
				break;
			}
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem1))
			{
				stringBuilder.Append(" AND EXISTS(select * from T_IV_ExpenseEntry ae where a.MID=ae.MID AND ae.MTrackItem1=@MTrackItem1 AND ae.MIsDelete=0 )");
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem2))
			{
				stringBuilder.Append(" AND EXISTS(select * from T_IV_ExpenseEntry ae where a.MID=ae.MID AND ae.MTrackItem2=@MTrackItem2 AND ae.MIsDelete=0 )");
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem3))
			{
				stringBuilder.Append(" AND EXISTS(select * from T_IV_ExpenseEntry ae where a.MID=ae.MID AND ae.MTrackItem3=@MTrackItem3 AND ae.MIsDelete=0 )");
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem4))
			{
				stringBuilder.Append(" AND EXISTS(select * from T_IV_ExpenseEntry ae where a.MID=ae.MID AND ae.MTrackItem4=@MTrackItem4 AND ae.MIsDelete=0 )");
			}
			if (!string.IsNullOrEmpty(filter.MTrackItem5))
			{
				stringBuilder.Append(" AND EXISTS(select * from T_IV_ExpenseEntry ae where a.MID=ae.MID AND ae.MTrackItem5=@MTrackItem5 AND ae.MIsDelete=0 )");
			}
			return stringBuilder.ToString();
		}

		private static void SetVerificationModel(MContext ctx, IVExpensePaymentModel model, OperationResult opResult)
		{
			decimal num = (model.MPayment.MTaxTotalAmtFor - (model.MExpense.MTaxTotalAmtFor - model.MExpense.MVerificationAmt) > decimal.Zero) ? (model.MExpense.MTaxTotalAmtFor - model.MExpense.MVerificationAmt) : model.MPayment.MTaxTotalAmtFor;
			IVVerificationModel iVVerificationModel = new IVVerificationModel();
			iVVerificationModel.MSourceBillID = opResult.ObjectID;
			iVVerificationModel.MSourceBillType = "Payment";
			iVVerificationModel.MSourceBillEntryID = "";
			iVVerificationModel.MTargetBillID = model.MExpense.MID;
			iVVerificationModel.MTargetBillType = "Expense";
			iVVerificationModel.MTargetBillEntryID = "";
			iVVerificationModel.MAmount = num;
			iVVerificationModel.MDirection = 1;
			iVVerificationModel.MIsDelete = false;
			iVVerificationModel.MCreatorID = ctx.MUserID;
			iVVerificationModel.MCreateDate = DateTime.Now;
			IVVerificationRepository.UpdateVerification(ctx, iVVerificationModel);
			if (model.MPayment.MTaxTotalAmtFor - (model.MExpense.MTaxTotalAmtFor - model.MExpense.MVerificationAmt) >= decimal.Zero)
			{
				UpdateExpenseStatus(ctx, model.MExpense.MID, IVInvoiceStatusEnum.Paid);
			}
			IVExpenseLogRepository.AddExpensePaidLog(ctx, model.MExpense.MID, num);
		}

		public static List<IVVerificationListModel> GetHistoryVerifData(MContext ctx, IVVerificationListFilterModel filter)
		{
			string haveVerifDataView = GetHaveVerifDataView(filter);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(GetHistoryVerifDataSql(filter, haveVerifDataView, "Payment"));
			stringBuilder.AppendLine(" Union All");
			stringBuilder.AppendLine(GetHistoryVerifDataSql(filter, haveVerifDataView, "Receive"));
			MySqlParameter[] array = new MySqlParameter[8]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizType", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MContactID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCurrencyID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MDueDate", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBillID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizBillType", MySqlDbType.VarChar, 36)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = filter.MBizType;
			array[2].Value = filter.MContactID;
			array[3].Value = filter.MCurrencyID;
			array[4].Value = ctx.MLCID;
			array[5].Value = DateTime.Now.ToString("yyyy-MM-dd 23:59:59");
			array[6].Value = filter.MBillID;
			array[7].Value = filter.MBizBillType;
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet dataSet = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array);
			return ModelInfoManager.DataTableToList<IVVerificationListModel>(dataSet.Tables[0]);
		}

		private static string GetHaveVerifDataView(IVVerificationListFilterModel filter)
		{
			string str = "T_IV_Expense";
			if (filter.MBizBillType == "Payment")
			{
				str = "T_IV_Payment";
			}
			else if (filter.MBizBillType == "Receive")
			{
				str = "T_IV_Receive";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Select v.MID AS VerificationID, v.MSourceBillType,v.MSourceBillID, v.MTargetBillType,v.MTargetBillID,v.MAmount,v.MAmt ");
			stringBuilder.AppendLine("From " + str + " a ");
			stringBuilder.AppendLine("inner Join T_IV_Verification v On v.MSourceBillType=@MBizBillType And a.MID=v.MSourceBillID  AND v.MIsDelete=0 ");
			stringBuilder.AppendLine("Where a.MOrgID=@MOrgID AND a.MIsDelete=0");
			GetFilterString(filter, stringBuilder);
			stringBuilder.AppendLine("UNION ALL ");
			stringBuilder.AppendLine("Select v.MID AS VerificationID, v.MTargetBillType as MSourceBillType,v.MTargetBillID as MSourceBillID,v.MSourceBillType as MTargetBillType,v.MSourceBillID as MTargetBillID,v.MAmount,v.MAmt ");
			stringBuilder.AppendLine("From " + str + " a ");
			stringBuilder.AppendLine("inner Join T_IV_Verification v On v.MTargetBillType=@MBizBillType And a.MID=v.MTargetBillID  AND v.MIsDelete=0  ");
			stringBuilder.AppendLine("Where a.MOrgID=@MOrgID AND a.MIsDelete=0 ");
			GetFilterString(filter, stringBuilder);
			return stringBuilder.ToString();
		}

		private static void GetFilterString(IVVerificationListFilterModel filter, StringBuilder sqlVerifi)
		{
			if (!string.IsNullOrWhiteSpace(filter.MBizType))
			{
				sqlVerifi.AppendLine(" And a.MType=@MBizType ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MContactID))
			{
				sqlVerifi.AppendLine(" And a.MContactID=@MContactID ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MCurrencyID))
			{
				sqlVerifi.AppendLine(" And a.MCyID=@MCurrencyID ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MBillID))
			{
				sqlVerifi.AppendLine(" And a.MID=@MBillID ");
			}
			string verifScope = GetVerifScope(filter.MViewDataType, " a");
			if (!string.IsNullOrWhiteSpace(verifScope))
			{
				sqlVerifi.AppendLine(" And " + verifScope);
			}
			if (!string.IsNullOrEmpty(filter.MKeyword))
			{
				sqlVerifi.AppendLine(" AND (a.MReference=@MKeyword OR a.MNumber=@MKeyword)");
			}
			if (filter.MAmount > decimal.Zero)
			{
				sqlVerifi.AppendLine(" AND ABS(a.MTaxTotalAmtFor) - ABS(a.MVerificationAmt)=@MAmount");
			}
		}

		private static string GetVerifScope(IVVerificationInforViewScopeEnum verificationInforViewScope, string tableAs)
		{
			switch (verificationInforViewScope)
			{
			case IVVerificationInforViewScopeEnum.All:
				return "";
			case IVVerificationInforViewScopeEnum.DueAndUnFinished:
				return string.Format(" {0}.MVerificationAmt < {0}.MTaxTotalAmtFor And {0}.MDueDate <=@MDueDate", tableAs);
			case IVVerificationInforViewScopeEnum.Finish:
				return string.Format(" {0}.MVerificationAmt >= {0}.MTaxTotalAmtFor ", tableAs);
			case IVVerificationInforViewScopeEnum.UnFinished:
				return string.Format(" {0}.MVerificationAmt < {0}.MTaxTotalAmtFor ", tableAs);
			default:
				return "";
			}
		}

		private static string GetHistoryVerifDataSql(IVVerificationListFilterModel filter, string sqlVerifi, string bizBillType)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			if (bizBillType == "Payment")
			{
				text = "T_IV_Payment";
				stringBuilder.AppendLine("Select v.VerificationID, v.MSourceBillType,v.MSourceBillID,'Expense' as MBizBillType,a.MType as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,a.MNumber as MBillNo,");
				stringBuilder.AppendLine("a.MBizDate,a.MContactID,ifnull(convert(AES_DECRYPT(b.MName,'{0}') using utf8),concat(e.MFirstName,e.MLastName)) as MContactName,a.MCyID as MCurrencyID,");
				stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
				stringBuilder.AppendLine("abs(v.MAmt) as MHaveVerificationAmt,abs(v.MAmount)  as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(v.MAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(v.MAmount) as MNoVerificationAmtFor");
				stringBuilder.AppendLine("From " + text + " a ");
				stringBuilder.AppendLine("Left Join T_BD_Contacts_l b On a.MContactID=b.MParentID and b.MOrgID = a.MOrgID and b.MLocaleID=@MLocaleID  AND b.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_BD_Employees_L e On a.MContactID=e.MParentID and e.MOrgID = a.MOrgID and e.MLocaleID=@MLocaleID  AND e.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID AND c.MIsDelete=0 ");
				stringBuilder.AppendLine("inner Join (" + sqlVerifi + ") v On a.MID=v.MTargetBillID and v.MTargetBillType= '" + bizBillType + "' ");
				stringBuilder.AppendLine("Where a.MOrgID=@MOrgID  AND a.MIsDelete=0");
			}
			else if (bizBillType == "Receive")
			{
				text = "T_IV_Receive";
				stringBuilder.AppendLine("Select v.VerificationID, v.MSourceBillType,v.MSourceBillID,'Expense' as MBizBillType,a.MType as MBizType,");
				stringBuilder.AppendLine("a.MID as MBillID,a.MNumber as MBillNo,");
				stringBuilder.AppendLine("a.MBizDate,a.MContactID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,a.MCyID as MCurrencyID,");
				stringBuilder.AppendLine("c.MName as MCurrencyName,a.MExchangeRate as MRate,a.MReference,");
				stringBuilder.AppendLine("a.MTaxTotalAmt as MAmountTotal,a.MTaxTotalAmtFor as MAmountTotalFor,");
				stringBuilder.AppendLine("abs(v.MAmt) as MHaveVerificationAmt,abs(v.MAmount)  as MHaveVerificationAmtFor,");
				stringBuilder.AppendLine("ABS(a.MTaxTotalAmt)-abs(v.MAmt) as MNoVerificationAmt,ABS(a.MTaxTotalAmtFor)-abs(v.MAmount) as MNoVerificationAmtFor");
				stringBuilder.AppendLine("From " + text + " a ");
				stringBuilder.AppendLine("Left Join T_BD_Contacts_l b On a.MContactID=b.MParentID and b.MOrgID = a.MOrgID and b.MLocaleID=@MLocaleID  AND b.MIsDelete=0 ");
				stringBuilder.AppendLine("Left Join T_Bas_Currency_L c On a.MCyID=c.MParentID and c.MLocaleID=@MLocaleID  AND c.MIsDelete=0 ");
				stringBuilder.AppendLine("inner Join (" + sqlVerifi + ") v On a.MID=v.MTargetBillID and v.MTargetBillType= '" + bizBillType + "' ");
				stringBuilder.AppendLine("Where a.MOrgID=@MOrgID  AND a.MIsDelete=0");
			}
			else if (bizBillType == "Expense")
			{
				text = "T_IV_Expense";
			}
			if (!string.IsNullOrWhiteSpace(filter.MContactID))
			{
				stringBuilder.AppendLine(" And a.MContactID=@MContactID ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MCurrencyID))
			{
				stringBuilder.AppendLine(" And a.MCyID=@MCurrencyID ");
			}
			if (!string.IsNullOrWhiteSpace(filter.MBillID))
			{
				stringBuilder.AppendLine(" And v.MSourceBillID=@MBillID ");
			}
			return stringBuilder.ToString();
		}

		public static List<CommandInfo> UpdateCurrentAccount(MContext ctx, string oldCode, string newCode)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
				{
					Value = ctx.MOrgID
				},
				new MySqlParameter("@oldCode", MySqlDbType.VarChar, 36)
				{
					Value = oldCode
				},
				new MySqlParameter("@newCode", MySqlDbType.VarChar, 36)
				{
					Value = newCode
				}
			};
			CommandInfo commandInfo = new CommandInfo();
			commandInfo.CommandText = "update t_iv_expense set MCurrentAccountCode=@newCode where MCurrentAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete=0";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}
	}
}
