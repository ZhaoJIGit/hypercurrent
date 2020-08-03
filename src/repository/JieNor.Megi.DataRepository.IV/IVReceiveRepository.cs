using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Log;
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
	public class IVReceiveRepository : IVBaseRepository<IVReceiveModel>
	{
		public static List<IVReceiveListModel> GetReceiveList(MContext ctx, string filterString)
		{
			return null;
		}

		public static OperationResult UpdateReceive(MContext ctx, IVReceiveModel model)
		{
			model.MNumber = ModelInfoManager.GetAutoNumber<IVReceiveModel>(ctx, model.MID, "RCV-", "");
			OperationResult operationResult = IVBaseRepository<IVReceiveModel>.UpdateBill(ctx, model, null);
			if (operationResult.Success)
			{
				OptLogTemplate updateReceiveLogTemplate = GetUpdateReceiveLogTemplate(model);
				OptLog.AddLog(updateReceiveLogTemplate, ctx, model.MID, model.MContactID, model.MBizDate, model.MTaxTotalAmtFor);
			}
			return operationResult;
		}

		public static List<CommandInfo> ImportReceiveList(MContext ctx, List<IVReceiveModel> list, ref string message, ref string startDate, ref string endDate)
		{
			List<CommandInfo> list2 = new List<CommandInfo>();
			int num = 0;
			foreach (IVReceiveModel item in list)
			{
				item.MID = UUIDHelper.GetGuid();
				item.IsNew = true;
				item.TableName = new IVReceiveModel().TableName;
				ResetReceiveAmt(item);
				string[] array = ModelInfoManager.GetAutoNumber<IVReceiveModel>(ctx, item.MID, "RCV-", "").Split('-');
				if (array.Length == 2)
				{
					item.MNumber = $"{array[0]}-{(Convert.ToInt32(array[1]) + num).ToString().PadLeft(4, '0')}";
				}
				item.MReconcileStatu = Convert.ToInt32(IVReconcileStatus.None);
				item.MSource = 102;
				int num2 = 1;
				foreach (IVReceiveEntryModel item2 in item.ReceiveEntry)
				{
					item2.MEntryID = UUIDHelper.GetGuid();
					item2.MSeq = num2;
					num2++;
				}
				num++;
			}
			OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBills(ctx, list, null);
			if (operationResult.Success)
			{
				list2.AddRange(operationResult.OperationCommands);
			}
			list2.AddRange(GetNewReceiveCmds(ctx, list));
			List<DateTime> source = (from f in list
			select f.MBizDate).ToList();
			DateTime dateTime = source.Min();
			startDate = dateTime.ToString("yyyy-MM-dd");
			dateTime = source.Max();
			endDate = dateTime.ToString("yyyy-MM-dd");
			return list2;
		}

		public static List<CommandInfo> GetNewReceiveCmds(MContext ctx, List<IVReceiveModel> modelList)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			string value = "INSERT INTO t_iv_receive(MID,MOrgID,MBankID,MType,MContactID,MNumber,MBizDate,MBranding,MAttachCount,MTaxID,MCyID,MExchangeRate,MOToLRate,MTotalAmtFor,MLToORate,MTotalAmt,MTaxTotalAmtFor,MTaxTotalAmt,MVerificationAmt,MReconcileAmt,MReconcileAmtFor,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate,MReference,MDesc,MContactType,MSource,MReconcileStatu)";
			string format = " SELECT @MID{0},@MOrgID{0},@MBankID{0},@MType{0},@MContactID{0},@MNumber{0},@MBizDate{0},@MBranding{0},@MAttachCount{0},@MTaxID{0},@MCyID{0},@MExchangeRate{0},@MOToLRate{0},@MLToORate{0},@MTotalAmtFor{0},@MTotalAmt{0},@MTaxTotalAmtFor{0},@MTaxTotalAmt{0},@MVerificationAmt{0},@MReconcileAmt{0},@MReconcileAmtFor{0},0,@MCreatorID{0},@MCreateDate{0},@MModifierID{0},@MModifyDate{0},@MReference{0},@MDesc{0},@MContactType{0},@MSource{0},@MReconcileStatu{0}";
			string value2 = "INSERT INTO t_iv_receiveentry(MEntryID,MID,MSeq,MItemID,MAcctID,MTaxID,MTrackItem1,MTrackItem2,MTrackItem3,MTrackItem4,MTrackItem5,MQty,MPrice,MDiscount,MAmountFor,MAmount,MTaxAmountFor,MTaxAmount,MTaxAmtFor,MTaxAmt,MDesc,MOrgID,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate)";
			string format2 = "SELECT @MEntryID{0},@MID{0},@MSeq{0},@MItemID{0},@MAcctID{0},@MTaxID{0},@MTrackItem1{0},@MTrackItem2{0},@MTrackItem3{0},@MTrackItem4{0},@MTrackItem5{0},@MQty{0},@MPrice{0},@MDiscount{0},@MAmountFor{0},@MAmount{0},@MTaxAmountFor{0},@MTaxAmount{0},@MTaxAmtFor{0},@MTaxAmt{0},@MDesc{0},@MOrgID{0},0,@MCreatorID{0},@MCreateDate{0},@MModifierID{0},@MModifyDate{0}";
			StringBuilder stringBuilder = new StringBuilder(1000);
			StringBuilder stringBuilder2 = new StringBuilder(1000);
			stringBuilder.Append(value);
			stringBuilder2.Append(value2);
			int num = 0;
			int num2 = 0;
			List<MySqlParameter> list2 = new List<MySqlParameter>();
			List<MySqlParameter> list3 = new List<MySqlParameter>();
			foreach (IVReceiveModel model in modelList)
			{
				if (num > 0)
				{
					stringBuilder.Append(" UNION ALL ");
				}
				list2.AddRange(GetReceiveParams(ctx, num, model));
				stringBuilder.AppendFormat(format, num);
				foreach (IVReceiveEntryModel item in model.ReceiveEntry)
				{
					if (num2 > 0)
					{
						stringBuilder2.Append(" UNION ALL ");
					}
					list3.AddRange(GetReceiveEntryParams(ctx, num2, item, model.MID));
					stringBuilder2.AppendFormat(format2, num2);
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

		private static List<MySqlParameter> GetReceiveParams(MContext ctx, int i, IVReceiveModel model)
		{
			return new List<MySqlParameter>
			{
				new MySqlParameter($"@MID{i}", model.MID),
				new MySqlParameter($"@MOrgID{i}", ctx.MOrgID),
				new MySqlParameter($"@MBankID{i}", model.MBankID),
				new MySqlParameter($"@MType{i}", model.MType),
				new MySqlParameter($"@MContactID{i}", model.MContactID),
				new MySqlParameter($"@MNumber{i}", model.MNumber),
				new MySqlParameter($"@MBizDate{i}", model.MBizDate),
				new MySqlParameter($"@MBranding{i}", model.MBranding),
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
				new MySqlParameter($"@MReconcileAmt{i}", model.MReconcileAmt),
				new MySqlParameter($"@MReconcileAmtFor{i}", model.MReconcileAmtFor),
				new MySqlParameter($"@MReference{i}", model.MReference),
				new MySqlParameter($"@MDesc{i}", model.MDesc),
				new MySqlParameter($"@MContactType{i}", model.MContactType),
				new MySqlParameter($"@MSource{i}", model.MSource),
				new MySqlParameter($"@MReconcileStatu{i}", model.MReconcileStatu),
				new MySqlParameter($"@MVerifyAmt{i}", model.MVerifyAmt),
				new MySqlParameter($"@MVerifyAmtFor{i}", model.MVerifyAmtFor),
				new MySqlParameter($"@MCurrentAccountCode{i}", model.MCurrentAccountCode),
				new MySqlParameter($"@MCreatorID{i}", ctx.MUserID),
				new MySqlParameter($"@MCreateDate{i}", ctx.DateNow),
				new MySqlParameter($"@MModifierID{i}", ctx.MUserID),
				new MySqlParameter($"@MModifyDate{i}", ctx.DateNow)
			};
		}

		private static List<MySqlParameter> GetReceiveEntryParams(MContext ctx, int i, IVReceiveEntryModel entry, string mid)
		{
			return new List<MySqlParameter>
			{
				new MySqlParameter($"@MEntryID{i}", entry.MEntryID),
				new MySqlParameter($"@MID{i}", mid),
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
				new MySqlParameter($"@MOrgID{i}", ctx.MOrgID),
				new MySqlParameter($"@MCreatorID{i}", ctx.MUserID),
				new MySqlParameter($"@MCreateDate{i}", ctx.DateNow),
				new MySqlParameter($"@MModifierID{i}", ctx.MUserID),
				new MySqlParameter($"@MModifyDate{i}", ctx.DateNow)
			};
		}

		public static List<CommandInfo> GetUpdateReceiveCommand(MContext ctx, IVReceiveModel model)
		{
			List<CommandInfo> list = new List<CommandInfo>();
			OptLogTemplate updateReceiveLogTemplate = GetUpdateReceiveLogTemplate(model);
			ResetReceiveAmt(model);
			model.MNumber = ModelInfoManager.GetAutoNumber<IVReceiveModel>(ctx, model.MID, "RCV-", "");
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVReceiveModel>(ctx, model, null, true));
			list.Add(OptLog.GetAddLogCommand(updateReceiveLogTemplate, ctx, model.MID, model.MContactID, model.MBizDate, model.MTaxTotalAmtFor));
			return list;
		}

		private static OptLogTemplate GetUpdateReceiveLogTemplate(IVReceiveModel model)
		{
			if (string.IsNullOrWhiteSpace(model.MID) && !model.IsDelete)
			{
				return OptLogTemplate.Receive_Created;
			}
			return OptLogTemplate.Receive_Edited;
		}

		public static IVReceiveModel GetReceiveEditModel(MContext ctx, string pkID)
		{
			IVReceiveModel iVReceiveModel = ModelInfoManager.GetDataEditModel<IVReceiveModel>(ctx, pkID, false, true);
			if (iVReceiveModel == null)
			{
				iVReceiveModel = new IVReceiveModel();
			}
			return iVReceiveModel;
		}

		public static IVReceiveViewModel GetReceiveViewModel(MContext ctx, string pkID)
		{
			string sql = string.Format("SELECT a.*, convert(AES_DECRYPT(b.MName,'{0}') using utf8) as MContactName from T_IV_Receive a\r\n                            left JOIN T_BD_Contacts_L b ON a.MOrgID = b.MOrgID and a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID AND  b.MIsDelete = 0 \r\n                            WHERE a.MID=@MID AND a.MOrgID=@MOrgID and a.MIsDelete = 0 ", "JieNor-001");
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MLocaleID", ctx.MLCID),
				new MySqlParameter("@MID", pkID),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			DataSet ds = dynamicDbHelperMySQL.Query(sql, cmdParms);
			IVReceiveViewModel iVReceiveViewModel = ModelInfoManager.GetFirstOrDefaultModel<IVReceiveViewModel>(ds);
			if (iVReceiveViewModel == null)
			{
				iVReceiveViewModel = new IVReceiveViewModel();
			}
			List<IVReceiveEntryModel> list = iVReceiveViewModel.ReceiveEntry = ModelInfoManager.GetBizEntryDataEditModel<IVReceiveEntryModel>(ctx, pkID);
			iVReceiveViewModel.ReceiveAttachment = ModelInfoManager.GetBizAttachmentRelationModel<IVReceiveAttachmentModel>(ctx, pkID);
			return iVReceiveViewModel;
		}

		public static OperationResult DeleteReceive(MContext ctx, IVReceiveModel model)
		{
			return IVBaseRepository<IVReceiveModel>.DeleteBill<IVReceiveModel>(ctx, model.MID);
		}

		public static List<IVReceiveModel> GetInitList(MContext ctx)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*, c.MCurrencyID AS MOrgCyID, convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MContactName,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from T_IV_Receive a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID  AND  b.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON a.MOrgID=c.MOrgID AND  c.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_IV_ReceiveAttachment d ON a.MID=d.MParentID AND d.MIsDelete = 0 ");
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
			return ModelInfoManager.DataTableToList<IVReceiveModel>(dt);
		}

		public static List<IVReceiveModel> GetReceiveModelIncludeEntry(MContext ctx, SqlWhere filter)
		{
			if (filter == null)
			{
				throw new NullReferenceException("filter can not be null");
			}
			return ModelInfoManager.GetDataModelList<IVReceiveModel>(ctx, filter, false, true);
		}

		public static List<IVReceiveModel> GetModelList(MContext ctx, List<string> pkIds)
		{
			return ModelInfoManager.GetDataModelList<IVReceiveModel>(ctx, pkIds);
		}

		public static List<IVReceiveModel> GetReceiveList(MContext ctx, ParamBase param)
		{
			return ModelInfoManager.GetDataModelList<IVReceiveModel>(ctx, param.KeyIDs);
		}

		public static List<IVReceiveModel> GetReceiveListByFilter(MContext ctx, IVReceiveListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*, c.MCurrencyID AS MOrgCyID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MContactName,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from T_IV_Receive a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID  AND  b.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON a.MOrgID=c.MOrgID AND  c.MIsDelete = 0  ");
			stringBuilder.Append(" LEFT JOIN T_IV_ReceiveAttachment d ON a.MID=d.MParentID AND d.MIsDelete = 0  ");
			stringBuilder.Append(" WHERE a.MIsDelete = 0  AND a.MOrgID=@MOrgID ");
			stringBuilder.Append(" AND  a.MBizDate >= @MStartDate AND a.MBizDate <= @MEndDate");
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
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			DataSet ds = new DynamicDbHelperMySQL(ctx).Query(sqlQuery.Sql, array);
			return ModelInfoManager.DataTableToList<IVReceiveModel>(ds);
		}

		public static DataGridJson<IVReceiveModel> GetInitReceiveListByPage(MContext ctx, IVReceiveListFilterModel filter)
		{
			SqlQuery sqlQuery = new SqlQuery();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select a.*, c.MCurrencyID AS MOrgCyID,convert(AES_DECRYPT(b.MName,'{0}') using utf8) AS MContactName,group_concat(DISTINCT d.MAttachID) AS MAttachIDs from T_IV_Receive a ", "JieNor-001");
			stringBuilder.Append(" LEFT JOIN T_BD_Contacts_l b ON a.MContactID=b.MParentID AND b.MLocaleID=@MLocaleID  AND  b.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_REG_Financial c ON a.MOrgID=c.MOrgID AND  c.MIsDelete = 0 ");
			stringBuilder.Append(" LEFT JOIN T_IV_ReceiveAttachment d ON a.MID=d.MParentID AND  d.MIsDelete = 0 ");
			stringBuilder.Append(" WHERE a.MIsDelete = 0  AND a.MOrgID=@MOrgID ");
			stringBuilder.Append(" AND a.MBizDate < @ConversionDate");
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
			sqlQuery.SqlWhere = filter;
			sqlQuery.SelectString = stringBuilder.ToString();
			MySqlParameter[] array2 = array;
			foreach (MySqlParameter para in array2)
			{
				sqlQuery.AddParameter(para);
			}
			return ModelInfoManager.GetPageDataModelListBySql<IVReceiveModel>(ctx, sqlQuery);
		}

		public static OperationResult DeleteReceiveList(MContext ctx, ParamBase param)
		{
			return IVBaseRepository<IVReceiveModel>.DeleteBill<IVReceiveModel>(ctx, param);
		}

		public static OperationResult UpdateReconcileStatu(MContext ctx, string receiveId, IVReconcileStatus statu)
		{
			CommandInfo updateReconcileStatuSql = GetUpdateReconcileStatuSql(ctx, receiveId, statu);
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

		public static CommandInfo GetUpdateReconcileStatuSql(MContext ctx, string receiveId, IVReconcileStatus statu)
		{
			string commandText = "UPDATE T_IV_Receive SET MReconcileStatu=@MReconcileStatu WHERE MID=@MID AND MOrgID=@MOrgID ";
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MID", receiveId),
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

		private static void ResetReceiveAmt(IVReceiveModel model)
		{
			if (model != null)
			{
				model.MTotalAmtFor = Math.Round(model.MTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MTotalAmt = Math.Round(model.MTotalAmt, 2, MidpointRounding.AwayFromZero);
				model.MTaxTotalAmtFor = Math.Round(model.MTaxTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MTaxTotalAmt = Math.Round(model.MTaxTotalAmt, 2, MidpointRounding.AwayFromZero);
				if (model.ReceiveEntry != null && model.ReceiveEntry.Count != 0)
				{
					foreach (IVReceiveEntryModel item in model.ReceiveEntry)
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
			commandInfo.CommandText = "update t_iv_receive set MCurrentAccountCode=@newCode where MCurrentAccountCode=@oldCode and MOrgID=@MOrgID";
			DbParameter[] array = commandInfo.Parameters = parameters;
			list.Add(commandInfo);
			return list;
		}
	}
}
