using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataRepository.GL;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace JieNor.Megi.DataRepository.IV
{
	public static class IVTransferRepository
	{
		private static readonly GLDocVoucherRepository docVoucherDal = new GLDocVoucherRepository();

		public static List<IVTransferListModel> GetTransferList(MContext ctx, string filterString)
		{
			return null;
		}

		public static OperationResult UpdateTransfer(MContext ctx, IVTransferModel model)
		{
			model.MOrgID = ctx.MOrgID;
			ResetTransferAmt(model);
			PutInAmountAndRate(ctx, model);
			bool flag = string.IsNullOrWhiteSpace(model.MID);
			List<CommandInfo> list = new List<CommandInfo>();
			list.AddRange(ModelInfoManager.GetInsertOrUpdateCmd<IVTransferModel>(ctx, model, null, true));
			if (flag || GLInterfaceRepository.IsDocCanOperate(ctx, model.MID).Success)
			{
				OperationResult operationResult = GLInterfaceRepository.GenerateVouchersByBill(ctx, model, null);
				if (!operationResult.Success)
				{
					return operationResult;
				}
				list.AddRange(operationResult.OperationCommands);
			}
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(list) > 0),
				SuccessModelID = new List<string>
				{
					model.MID
				}
			};
		}

		private static void ResetTransferAmt(IVTransferModel model)
		{
			if (model != null)
			{
				model.MFromTotalAmt = Math.Round(model.MFromTotalAmt, 2, MidpointRounding.AwayFromZero);
				model.MFromTotalAmtFor = Math.Round(model.MFromTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MToTotalAmt = Math.Round(model.MToTotalAmt, 2, MidpointRounding.AwayFromZero);
				model.MToTotalAmtFor = Math.Round(model.MToTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MDiffFromTotalAmtFor = Math.Round(model.MDiffFromTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MDiffToTotalAmtFor = Math.Round(model.MDiffToTotalAmtFor, 2, MidpointRounding.AwayFromZero);
				model.MBeginExchangeRate = Math.Round(model.MBeginExchangeRate, 6, MidpointRounding.AwayFromZero);
				model.MExchangeRate = Math.Round(model.MExchangeRate, 6, MidpointRounding.AwayFromZero);
			}
		}

		public static IVTransferModel GetTransferEditModel(MContext ctx, string pkID)
		{
			IVTransferModel iVTransferModel = ModelInfoManager.GetDataEditModel<IVTransferModel>(ctx, pkID, false, true);
			if (iVTransferModel == null)
			{
				iVTransferModel = new IVTransferModel();
			}
			iVTransferModel.MSameTotalAmtFor = iVTransferModel.MFromTotalAmtFor;
			iVTransferModel.MDiffFromTotalAmtFor = iVTransferModel.MFromTotalAmtFor;
			iVTransferModel.MDiffToTotalAmtFor = iVTransferModel.MToTotalAmtFor;
			return iVTransferModel;
		}

		public static List<IVTransferModel> GetTransferList(MContext ctx, ParamBase param)
		{
			return ModelInfoManager.GetDataModelList<IVTransferModel>(ctx, param.KeyIDs);
		}

		public static OperationResult DeleteTransfer(MContext ctx, IVTransferModel model)
		{
			List<CommandInfo> deleteVoucherByDocIDCmds = GLInterfaceRepository.GetDeleteVoucherByDocIDCmds(ctx, model.MID);
			deleteVoucherByDocIDCmds.AddRange(ModelInfoManager.GetDeleteFlagCmd<IVTransferModel>(ctx, model.MID));
			return new OperationResult
			{
				Success = (new DynamicDbHelperMySQL(ctx).ExecuteSqlTran(deleteVoucherByDocIDCmds) > 0)
			};
		}

		public static OperationResult UpdateReconcileStatu(MContext ctx, string transferId, IVReconcileStatus statu)
		{
			CommandInfo updateReconcileStatuSql = GetUpdateReconcileStatuSql(ctx, transferId, statu);
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

		public static CommandInfo GetUpdateReconcileStatuSql(MContext ctx, string transferId, IVReconcileStatus statu)
		{
			string commandText = "";
			switch (statu)
			{
			case IVReconcileStatus.None:
				commandText = string.Format("UPDATE T_IV_Transfer \n                        SET MFromReconcileStatu = (CASE WHEN MFromReconcileStatu={0} THEN  {1} ELSE MFromReconcileStatu END),\n                        MToReconcileStatu = (CASE WHEN MToReconcileStatu={0} THEN  {1} ELSE MToReconcileStatu END) \r\n                        WHERE MID=@MID and MOrgID = @MOrgID and MIsDelete = 0 ", Convert.ToInt32(IVReconcileStatus.Marked), Convert.ToInt32(IVReconcileStatus.None));
				break;
			case IVReconcileStatus.Marked:
				commandText = string.Format("UPDATE T_IV_Transfer \n                        SET MFromReconcileStatu = (CASE WHEN MFromReconcileStatu={0} THEN  {1} ELSE MFromReconcileStatu END),\n                        MToReconcileStatu = (CASE WHEN MToReconcileStatu={0} THEN  {1} ELSE MToReconcileStatu END) \r\n                        WHERE MID=@MID and MOrgID = @MOrgID and MIsDelete = 0 ", Convert.ToInt32(IVReconcileStatus.None), Convert.ToInt32(IVReconcileStatus.Marked));
				break;
			}
			MySqlParameter[] parameters = new MySqlParameter[3]
			{
				new MySqlParameter("@MID", transferId),
				new MySqlParameter("@MReconcileStatu", Convert.ToInt32(statu)),
				new MySqlParameter("@MOrgID", ctx.MOrgID)
			};
			CommandInfo obj = new CommandInfo
			{
				CommandText = commandText
			};
			DbParameter[] array = obj.Parameters = parameters;
			return obj;
		}

		private static void PutInAmountAndRate(MContext ctx, IVTransferModel model)
		{
			if (model.MFromCyID == model.MToCyID && model.MFromCyID == ctx.MBasCurrencyID)
			{
				model.MBeginExchangeRate = 1.0m;
				model.MExchangeRate = 1.0m;
				model.MFromTotalAmtFor = model.MFromTotalAmt;
				model.MToTotalAmtFor = model.MToTotalAmt;
				model.MExchangeLoss = decimal.Zero;
			}
			else if (model.MFromCyID == ctx.MBasCurrencyID && model.MToCyID != ctx.MBasCurrencyID)
			{
				model.MBeginExchangeRate = decimal.One;
				model.MFromTotalAmtFor = model.MFromTotalAmt;
				model.MToTotalAmt = ((model.MToTotalAmt == decimal.Zero) ? (model.MToTotalAmtFor * model.MExchangeRate) : model.MToTotalAmt);
				model.MExchangeLoss = model.MFromTotalAmt - model.MToTotalAmt;
			}
			else if (model.MFromCyID != ctx.MBasCurrencyID && model.MToCyID == ctx.MBasCurrencyID)
			{
				model.MFromTotalAmt = ((model.MFromTotalAmt == decimal.Zero) ? (model.MFromTotalAmtFor * model.MBeginExchangeRate) : model.MFromTotalAmt);
				model.MToTotalAmt = ((model.MToTotalAmt == decimal.Zero) ? (model.MFromTotalAmtFor * model.MExchangeRate) : model.MToTotalAmt);
				model.MToTotalAmtFor = model.MToTotalAmt;
				model.MExchangeLoss = model.MFromTotalAmt - model.MToTotalAmt;
			}
			model.MFromTotalAmt = Math.Round(model.MFromTotalAmt, 2, MidpointRounding.AwayFromZero);
			model.MToTotalAmt = Math.Round(model.MToTotalAmt, 2, MidpointRounding.AwayFromZero);
			model.MToTotalAmtFor = Math.Round(model.MToTotalAmtFor, 2, MidpointRounding.AwayFromZero);
			model.MFromTotalAmtFor = Math.Round(model.MFromTotalAmtFor, 2, MidpointRounding.AwayFromZero);
		}

		private static CommandInfo GetTransferUpdateSql(MContext ctx, IVTransferModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("update T_IV_Transfer set ");
			stringBuilder.Append(" MID = IFNULL(@MID,MID) , ");
			stringBuilder.Append(" MFromTotalAmtFor = IFNULL(@MFromTotalAmtFor,MFromTotalAmtFor) , ");
			stringBuilder.Append(" MFromTotalAmt = IFNULL(@MFromTotalAmt,MFromTotalAmt) , ");
			stringBuilder.Append(" MToTotalAmtFor = IFNULL(@MToTotalAmtFor,MToTotalAmtFor) , ");
			stringBuilder.Append(" MToTotalAmt = IFNULL(@MToTotalAmt,MToTotalAmt) , ");
			stringBuilder.Append(" MIsDelete = IFNULL(@MIsDelete,MIsDelete) , ");
			stringBuilder.Append(" MModifierID = IFNULL(@MModifierID,MModifierID) , ");
			stringBuilder.Append(" MModifyDate = IFNULL(@MModifyDate,MModifyDate) , ");
			stringBuilder.Append(" MOrgID = IFNULL(@MOrgID,MOrgID) , ");
			stringBuilder.Append(" MNumber = IFNULL(@MNumber,MNumber) , ");
			stringBuilder.Append(" MFromAcctID = IFNULL(@MFromAcctID,MFromAcctID) , ");
			stringBuilder.Append(" MToAcctID = IFNULL(@MToAcctID,MToAcctID) , ");
			stringBuilder.Append(" MBizDate = IFNULL(@MBizDate,MBizDate) , ");
			stringBuilder.Append(" MFromCyID = IFNULL(@MFromCyID,MFromCyID) , ");
			stringBuilder.Append(" MToCyID = IFNULL(@MToCyID,MToCyID) , ");
			stringBuilder.Append(" MExchangeRate = IFNULL(@MExchangeRate,MExchangeRate),  ");
			stringBuilder.Append(" MReference = IFNULL(@MReference,MReference)  ");
			stringBuilder.Append(" where MID=@MID AND MOrgID=@MOrgID and MIsDelete = 0 ");
			MySqlParameter[] array = new MySqlParameter[18]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MFromTotalAmtFor", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MFromTotalAmt", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MToTotalAmtFor", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MToTotalAmt", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MIsDelete", MySqlDbType.Bit),
				new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MModifyDate", MySqlDbType.DateTime),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MNumber", MySqlDbType.VarChar, 100),
				new MySqlParameter("@MFromAcctID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MToAcctID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizDate", MySqlDbType.DateTime),
				new MySqlParameter("@MFromCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MToCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MExchangeRate", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MReference", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MID;
			array[1].Value = model.MFromTotalAmtFor;
			array[2].Value = model.MFromTotalAmt;
			array[3].Value = model.MToTotalAmtFor;
			array[4].Value = model.MToTotalAmt;
			array[5].Value = model.MIsDelete;
			array[6].Value = ctx.MUserID;
			array[7].Value = DateTime.Now;
			array[8].Value = model.MOrgID;
			array[9].Value = model.MNumber;
			array[10].Value = model.MFromAcctID;
			array[11].Value = model.MToAcctID;
			array[12].Value = model.MBizDate;
			array[13].Value = model.MFromCyID;
			array[14].Value = model.MToCyID;
			array[15].Value = model.MExchangeRate;
			array[16].Value = model.MReference;
			array[17].Value = ctx.MOrgID;
			return new CommandInfo(stringBuilder.ToString(), array);
		}

		private static CommandInfo GetTransferInsertSql(MContext ctx, IVTransferModel model)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("insert into T_IV_Transfer(");
			stringBuilder.Append("MID,MFromTotalAmtFor,MFromTotalAmt,MToTotalAmtFor,MToTotalAmt,MIsDelete,MCreatorID,MCreateDate,MModifierID,MModifyDate,MOrgID,MNumber,MFromAcctID,MToAcctID,MBizDate,MFromCyID,MToCyID,MExchangeRate,MReference");
			stringBuilder.Append(") values (");
			stringBuilder.Append("@MID,@MFromTotalAmtFor,@MFromTotalAmt,@MToTotalAmtFor,@MToTotalAmt,@MIsDelete,@MCreatorID,@MCreateDate,@MModifierID,@MModifyDate,@MOrgID,@MNumber,@MFromAcctID,@MToAcctID,@MBizDate,@MFromCyID,@MToCyID,@MExchangeRate,@MReference");
			stringBuilder.Append(") ");
			MySqlParameter[] array = new MySqlParameter[19]
			{
				new MySqlParameter("@MID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MFromTotalAmtFor", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MFromTotalAmt", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MToTotalAmtFor", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MToTotalAmt", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MIsDelete", MySqlDbType.Bit),
				new MySqlParameter("@MCreatorID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MCreateDate", MySqlDbType.DateTime),
				new MySqlParameter("@MModifierID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MModifyDate", MySqlDbType.DateTime),
				new MySqlParameter("@MOrgID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MNumber", MySqlDbType.VarChar, 100),
				new MySqlParameter("@MFromAcctID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MToAcctID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MBizDate", MySqlDbType.DateTime),
				new MySqlParameter("@MFromCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MToCyID", MySqlDbType.VarChar, 36),
				new MySqlParameter("@MExchangeRate", MySqlDbType.Decimal, 23),
				new MySqlParameter("@MReference", MySqlDbType.VarChar, 36)
			};
			array[0].Value = model.MID;
			array[1].Value = model.MFromTotalAmtFor;
			array[2].Value = model.MFromTotalAmt;
			array[3].Value = model.MToTotalAmtFor;
			array[4].Value = model.MToTotalAmt;
			array[5].Value = model.MIsDelete;
			array[6].Value = ctx.MUserID;
			array[7].Value = DateTime.Now;
			array[8].Value = ctx.MUserID;
			array[9].Value = DateTime.Now;
			array[10].Value = ctx.MOrgID;
			array[11].Value = model.MNumber;
			array[12].Value = model.MFromAcctID;
			array[13].Value = model.MToAcctID;
			array[14].Value = DateTime.Now;
			array[15].Value = model.MFromCyID;
			array[16].Value = model.MToCyID;
			array[17].Value = model.MExchangeRate;
			array[18].Value = model.MReference;
			return new CommandInfo(stringBuilder.ToString(), array);
		}
	}
}
