using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.MultiLanguage;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace JieNor.Megi.DataRepository.IV
{
	public class IVPaymentRepository : IVBaseRepository<IVPaymentModel>
	{
		public static List<IVPaymentListModel> GetPaymentList(MContext ctx, string filterString)
		{
			return null;
		}

		public static OperationResult UpdatePayment(MContext ctx, IVPaymentModel model)
		{
			model.MNumber = ModelInfoManager.GetAutoNumber<IVPaymentModel>(ctx, model.MID, "PAY-", "");
			OperationResult operationResult = IVBaseRepository<IVPaymentModel>.UpdateBill(ctx, model, null);
			if (operationResult.Success)
			{
				OptLogTemplate updatePaymentLogTemplate = GetUpdatePaymentLogTemplate(model);
				OptLog.AddLog(updatePaymentLogTemplate, ctx, model.MID, model.MContactID, model.MBizDate, model.MTaxTotalAmtFor);
			}
			return operationResult;
		}

		public static List<CommandInfo> ImportPaymentList(MContext ctx, List<IVPaymentModel> list, ref string message, ref string startDate, ref string endDate)
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			int num = 0;
			foreach (IVPaymentModel item in list)
			{
				item.TableName = new IVPaymentModel().TableName;
				ResetPaymentAmt(item);
				string[] array = ModelInfoManager.GetAutoNumber<IVPaymentModel>(ctx, item.MID, "PAY-", "").Split('-');
				if (array.Length == 2)
				{
					item.MNumber = $"{array[0]}-{(Convert.ToInt32(array[1]) + num).ToString().PadLeft(4, '0')}";
				}
				item.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
				item.MSource = 102;
				list2.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, item, null, true));
				num++;
			}
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBills(ctx, list, null);
			if (operationResult.Success)
			{
				list2.AddRange(operationResult.OperationCommands);
			}
			List<DateTime> source = (from f in list
			select f.MBizDate).ToList();
			DateTime dateTime = source.Min();
			startDate = dateTime.ToString("yyyy-MM-dd");
			dateTime = source.Max();
			endDate = dateTime.ToString("yyyy-MM-dd");
			return list2;
		}

		public static List<CommandInfo> GetUpdatePaymentCommand(MContext ctx, IVPaymentModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			OptLogTemplate updatePaymentLogTemplate = GetUpdatePaymentLogTemplate(model);
			ResetPaymentAmt(model);
			model.MNumber = ModelInfoManager.GetAutoNumber<IVPaymentModel>(ctx, model.MID, "PAY-", "");
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVPaymentModel>(ctx, model, null, true));
			list.Add(OptLog.GetAddLogCommand(updatePaymentLogTemplate, ctx, model.MID, model.MContactID, model.MBizDate, model.MTaxTotalAmtFor));
			return list;
		}

		private static OptLogTemplate GetUpdatePaymentLogTemplate(IVPaymentModel model)
		{
			if (string.IsNullOrWhiteSpace(model.MID) && !model.IsDelete)
			{
				return OptLogTemplate.Payment_Created;
			}
			return OptLogTemplate.Payment_Edited;
		}

		public static IVPaymentModel GetPaymentEditModel(MContext ctx, string pkID)
		{
			IVPaymentModel iVPaymentModel = ModelInfoManager.GetDataEditModel<IVPaymentModel>(ctx, pkID, false, true);
			if (iVPaymentModel == null)
			{
				iVPaymentModel = new IVPaymentModel();
			}
			return iVPaymentModel;
		}

		public static IVPaymentViewModel GetPaymentViewModel(MContext ctx, string pkID)
		{
			string sql = string.Format("SELECT a.*, convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName from T_IV_Payment a\r\n                            left JOIN T_BD_Contacts_L b ON a.MOrgID = b.MOrgID and  a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete=0\r\n                            WHERE a.MID=@MID AND a.MIsDelete=0 AND a.MOrgID=@MOrgID ", "JieNor-001");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MID", pkID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			IVPaymentViewModel iVPaymentViewModel = ModelInfoManager.GetFirstOrDefaultModel<IVPaymentViewModel>(ds);
			if (iVPaymentViewModel == null)
			{
				iVPaymentViewModel = new IVPaymentViewModel();
			}
			List<IVPaymentEntryModel> list = iVPaymentViewModel.PaymentEntry = ModelInfoManager.GetBizEntryDataEditModel<IVPaymentEntryModel>(ctx, pkID);
			iVPaymentViewModel.PaymentAttachment = ModelInfoManager.GetBizAttachmentRelationModel<IVPaymentAttachmentModel>(ctx, pkID);
			return iVPaymentViewModel;
		}

		public static OperationResult DeletePayment(MContext ctx, IVPaymentModel model)
		{
			return IVBaseRepository<IVPaymentModel>.DeleteBill<IVPaymentModel>(ctx, model.MID);
		}

		public static OperationResult UpdateReconcileStatu(MContext ctx, string paymentId, IVReconcileStatus statu)
		{
			CommandInfo updateReconcileStatuSql = GetUpdateReconcileStatuSql(ctx, paymentId, statu);
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			dynamicDbHelperMySQL.ExecuteSqlTran(new List<CommandInfo>
			{
				updateReconcileStatuSql
			});
			return new OperationResult
			{
				Success = true
			};
		}

		public static CommandInfo GetUpdateReconcileStatuSql(MContext ctx, string paymentId, IVReconcileStatus statu)
		{
			string commandText = "UPDATE T_IV_Payment SET MReconcileStatu=@MReconcileStatu WHERE MID=@MID AND MOrgID=@MOrgID and MIsDelete = 0 ";
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MID", paymentId),
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MReconcileStatu", Convert.ToInt32(statu))
			};
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = parameters;
			return obj;
		}

		public static List<IVPaymentModel> GetInitList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*, c.MCurrencyID AS MOrgCyID, ifnull(convert(AES_DECRYPT(b.MName,'{0}') using utf8),concat(be.MFirstName,be.MLastName)) AS MContactName,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from T_IV_Payment a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MOrgID = b.MOrgID and  a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN t_bd_employees_l be ON a.MOrgID = be.MOrgID and  a.MContactID=be.MParentID AND be.MLocaleID=@MLocaleID  AND be.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON a.MOrgID=c.MOrgID AND c.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_IV_PaymentAttachment d ON a.MOrgID = d.MOrgID and a.MID=d.MParentID  AND d.MIsDelete = 0 ");
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
			return ModelInfoManager.DataTableToList<IVPaymentModel>(dt);
		}

		public static List<IVPaymentModel> GetPaymentModelIncludeEntry(MContext ctx, SqlWhere filter)
		{
			if (filter == null)
			{
				filter = new SqlWhere();
				filter.LessThen("MBizDate", ctx.MBeginDate);
			}
			return ModelInfoManager.GetDataModelList<IVPaymentModel>(ctx, filter, false, true);
		}

		public static List<IVPaymentModel> GetModelList(MContext ctx, List<string> pkIds)
		{
			string text = "";
			foreach (string pkId in pkIds)
			{
				text += $"'{pkId}',";
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
			}
			string sql = $"select * from T_IV_Payment where MOrgID='{ctx.MOrgID}' and MID in ({text}) and MIsDelete = 0 ";
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataTable dt = dynamicDbHelperMySQL.Query(sql).Tables[0];
			return ModelInfoManager.DataTableToList<IVPaymentModel>(dt);
		}

		public static List<IVPaymentModel> GetPaymentList(MContext ctx, ParamBase param)
		{
			return ModelInfoManager.GetDataModelList<IVPaymentModel>(ctx, param.KeyIDs);
		}

		public static List<IVPaymentModel> GetPaymentListByFilter(MContext ctx, IVPaymentListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*, c.MCurrencyID AS MOrgCyID, ifnull(convert(AES_DECRYPT(b.MName,'{0}') using utf8),concat(be.MFirstName,be.MLastName)) AS MContactName,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from T_IV_Payment a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MOrgID = b.MOrgID and a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND b.MIsDelete = 0  ");
			stringBuilder.Append(" LEFT JOIN t_bd_employees_l be ON a.MOrgID = be.MOrgID and a.MContactID=be.MParentID AND be.MLocaleID=@MLocaleID AND be.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON a.MOrgID=c.MOrgID AND c.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_IV_PaymentAttachment d ON a.MOrgID = d.MOrgID and  a.MID=d.MParentID  AND d.MIsDelete = 0 ");
			stringBuilder.Append(" WHERE a.MIsDelete = 0 AND a.MOrgID=@MOrgID ");
			stringBuilder.Append(" AND a.MBizDate >= @MStartDate AND a.MBizDate<= @MEndDate");
			stringBuilder.Append(" GROUP BY a.MID");
			MySqlParameter[] array = new MySqlParameter[4]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@MStartDate", MySqlDbType.DateTime),
				new MySqlParameter("@MEndDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = filter.MStartDate;
			array[3].Value = filter.MEndDate;
			filter.AddOrderBy("a.MBizDate", SqlOrderDir.Asc);
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.DataTableToList<IVPaymentModel>(new DynamicDbHelperMySQL(ctx).Query(sqlQuery.Sql, array));
		}

		public static DataGridJson<IVPaymentModel> GetInitPaymentListByPage(MContext ctx, IVPaymentListFilterModel filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*, c.MCurrencyID AS MOrgCyID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName,be.MFirstName,be.MLastName,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from T_IV_Payment a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON b.MOrgID = a.MOrgID and  a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID  AND b.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN t_bd_employees_l be ON be.MOrgID = a.MOrgID and a.MContactID=be.MParentID AND be.MLocaleID=@MLocaleID  AND be.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON a.MOrgID=c.MOrgID  AND c.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_IV_PaymentAttachment d ON d.MOrgID = a.MOrgID  and  a.MID=d.MParentID  AND d.MIsDelete = 0 ");
			stringBuilder.Append(" WHERE a.MIsDelete = 0 AND a.MOrgID=@MOrgID ");
			stringBuilder.Append(" AND a.MBizDate<@ConversionDate ");
			stringBuilder.Append(" GROUP BY a.MID");
			MySqlParameter[] array = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MLocaleID", MySqlDbType.VarChar, 6),
				new MySqlParameter("@ConversionDate", MySqlDbType.DateTime)
			};
			array[0].Value = ctx.MOrgID;
			array[1].Value = ctx.MLCID;
			array[2].Value = filter.MConversionDate;
			if (string.IsNullOrWhiteSpace(filter.Order))
			{
				filter.AddOrderBy("MModifyDate,MCreateDate", SqlOrderDir.Asc);
			}
			else if (filter.Sort == "MContactName")
			{
				filter.OrderBy($" convert(MContactName using gbk) {filter.Order}");
			}
			else
			{
				if (filter.Sort.Equals("MTaxTotalAmtFor2"))
				{
					filter.Sort = "MTaxTotalAmtFor";
				}
				filter.OrderBy($" {filter.Sort} {filter.Order}");
			}
			SqlQuery sqlQuery = new SqlQuery();
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			DataGridJson<IVPaymentModel> pageDataModelListBySql = ModelInfoManager.GetPageDataModelListBySql<IVPaymentModel>(ctx, sqlQuery);
			foreach (IVPaymentModel row in pageDataModelListBySql.rows)
			{
				if (string.IsNullOrEmpty(row.MContactName))
				{
					row.MContactName = GlobalFormat.GetUserName(row.MFirstName, row.MLastName, ctx);
				}
			}
			return pageDataModelListBySql;
		}

		public static OperationResult DeletePaymentList(MContext ctx, ParamBase param)
		{
			return IVBaseRepository<IVPaymentModel>.DeleteBill<IVPaymentModel>(ctx, param);
		}

		private static void ResetPaymentAmt(IVPaymentModel model)
		{
			if (model != null)
			{
				model.MTotalAmtFor = Math.Round(model.MTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MTotalAmt = Math.Round(model.MTotalAmt, 2, MidpointRounding.AwayFromZero);
				model.MTaxTotalAmtFor = Math.Round(model.MTaxTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MTaxTotalAmt = Math.Round(model.MTaxTotalAmt, 2, MidpointRounding.AwayFromZero);
				if (model.PaymentEntry != null && model.PaymentEntry.Count != 0)
				{
					foreach (IVPaymentEntryModel item in model.PaymentEntry)
					{
						item.MTaxAmount = Math.Round(item.MTaxAmount, 2, MidpointRounding.AwayFromZero);
						item.MTaxAmountFor = Math.Round(item.MTaxAmountFor, 2, MidpointRounding.AwayFromZero);
						item.MAmount = Math.Round(item.MAmount, 2, MidpointRounding.AwayFromZero);
						item.MAmountFor = Math.Round(item.MAmountFor, 2, MidpointRounding.AwayFromZero);
						item.MTaxAmtFor = Math.Round(item.MTaxAmtFor, 2, MidpointRounding.AwayFromZero);
						item.MTaxAmt = Math.Round(item.MTaxAmt, 2, MidpointRounding.AwayFromZero);
					}
				}
			}
		}

		public static IEnumerable<CommandInfo> UpdateCurrentAccount(MContext ctx, string oldCode, string newCode)
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
			commandInfo.CommandText = "update t_iv_payment set MCurrentAccountCode=@newCode where MCurrentAccountCode=@oldCode and MOrgID=@MOrgID and MIsDelete=0";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}
	}
}
