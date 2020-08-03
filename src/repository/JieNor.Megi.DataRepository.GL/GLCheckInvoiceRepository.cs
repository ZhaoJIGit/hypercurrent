using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.DataModel.IV;
using JieNor.Megi.DataModel.RI;
using JieNor.Megi.EntityModel.Context;
using MySql.Data.MySqlClient;
using System;

namespace JieNor.Megi.DataRepository.GL
{
	public class GLCheckInvoiceRepository
	{
		public int GetInvoice(MContext ctx, DateTime beginTime, DateTime endTime, string invoiceType)
		{
			string sql = "SELECT Count(*) AS RowCounts FROM T_IV_Invoice \r\n                           WHERE MOrgID=@MOrgID AND (MType = @MType OR MType=@MTypeRed) AND MIsDelete=0 \r\n                           AND MBizDate  BETWEEN @BeginTime AND @EndTime AND MStatus < 3 ";
			MySqlParameter[] cmdParms = new MySqlParameter[5]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BeginTime", beginTime),
				new MySqlParameter("@EndTime", endTime),
				new MySqlParameter("@MType", invoiceType),
				new MySqlParameter("@MTypeRed", invoiceType + "_Red")
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			return (single != null) ? Convert.ToInt32(single) : 0;
		}

		public int GetExpense(MContext ctx, DateTime beginTime, DateTime endTime)
		{
			string sql = "SELECT COUNT(*) AS RowCount\r\n                           FROM T_IV_Expense WHERE MOrgID=@MOrgID AND MIsDelete=0 \r\n                           AND MBizDate  BETWEEN @BeginTime AND @EndTime AND MStatus < 3 ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BeginTime", beginTime),
				new MySqlParameter("@EndTime", endTime)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			return (single != null) ? Convert.ToInt32(single) : 0;
		}

		public RIInvoiceModel GetPayment(MContext ctx, DateTime beginTime, DateTime endTime, bool isPayMent = true)
		{
			string str = " SELECT COUNT(*) AS RowCounts,t2.MBankTypeID,t2.MItemID AS MBankID ";
			str += (isPayMent ? " FROM T_IV_Payment t1 " : " FROM T_IV_Receive t1 ");
			str += $" INNER JOIN T_BD_BankAccount t2 \r\n                           ON t1.MBankID=t2.MItemID AND t2.MOrgID=t1.MOrgID  AND t2.MIsDelete=0 AND t2.MIsNeedReconcile=1\r\n                           WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0 AND t1.MBizDate  BETWEEN @BeginTime AND @EndTime \r\n                           AND ((t1.MReconcileStatu = {Convert.ToInt32(IVReconcileStatus.None)} OR t1.MReconcileStatu={Convert.ToInt32(IVReconcileStatus.Partly)})  ) ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BeginTime", beginTime),
				new MySqlParameter("@EndTime", endTime)
			};
			return ModelInfoManager.GetDataModel<RIInvoiceModel>(ctx, str, cmdParms);
		}

		public RIInvoiceModel GetTransfer(MContext ctx, DateTime beginTime, DateTime endTime)
		{
			string sql = string.Format(" SELECT t9.RowCounts,t10.MItemID AS MBankID,t10.MBankTypeID,t11.MItemID AS MBankIDFrom ,t11.MBankTypeID AS MBankTypeIDFrom FROM (\n                            SELECT COUNT(*) AS RowCounts,t1.* FROM T_IV_Transfer t1\n                            WHERE t1.MOrgID=@MOrgID AND t1.MIsDelete=0\n                            AND MBizDate BETWEEN @BeginTime AND @EndTime\n                            AND ((EXISTS (SELECT 1 FROM T_BD_BankAccount t2 WHERE t2.MItemID=t1.MFromAcctID AND t2.MIsDelete=0 AND t2.MIsNeedReconcile=1) \n                            AND (t1.MFromReconcileStatu={0} or t1.MFromReconcileStatu={1}) ) \n                            OR (EXISTS (SELECT 1 FROM T_BD_BankAccount t3 WHERE t3.MItemID=t1.MToAcctID AND t3.MIsDelete=0 AND t3.MIsNeedReconcile=1) \n                            AND (t1.MToReconcileStatu={0} or t1.MToReconcileStatu={1}) )) \r\n                            )t9 \r\n                            LEFT JOIN T_BD_BankAccount t10\r\n                            ON t10.MItemID = t9.MToAcctID AND t10.MIsDelete = 0 AND t10.MIsNeedReconcile = 1\r\n                            LEFT JOIN T_BD_BankAccount t11\r\n                            ON t11.MItemID = t9.MFromAcctID AND t11.MIsDelete = 0 AND t11.MIsNeedReconcile = 1 \r\n                            ORDER BY  t9.MBizDate DESC ", Convert.ToInt32(IVReconcileStatus.None), Convert.ToInt32(IVReconcileStatus.Partly));
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BeginTime", beginTime),
				new MySqlParameter("@EndTime", endTime)
			};
			return ModelInfoManager.GetDataModel<RIInvoiceModel>(ctx, sql, cmdParms);
		}

		public RIInvoiceModel GetBankBill(MContext ctx, DateTime beginTime, DateTime endTime)
		{
			string sql = " SELECT COUNT(*) AS RowCounts,a.MBankID,a.MBankTypeID FROM T_IV_BankBill  a\n                            INNER JOIN T_IV_BankBillEntry b  ON a.MID=b.MID and b.MOrgID = a.MOrgID and b.MIsDelete = 0 \n                            WHERE a.MOrgID=@MOrgID AND b.MIsDelete=0 AND IFNULL(b.MParentID,'')=''\n                            AND b.MDate BETWEEN @BeginTime AND @EndTime AND b.MCheckState <> 2\n                            AND NOT EXISTS(SELECT 1 FROM t_iv_bankbillreconcile c WHERE c.MOrgID=@MOrgID AND b.MEntryID=c.MBankBillEntryID and a.MBankID is not null AND c.MIsDelete=0 ) ";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@BeginTime", beginTime),
				new MySqlParameter("@EndTime", endTime)
			};
			return ModelInfoManager.GetDataModel<RIInvoiceModel>(ctx, sql, cmdParms);
		}

		public int GetIVToVoucherBill(MContext ctx, int year, int month)
		{
			string sql = " SELECT COUNT(DISTINCT t2.MDocID ) AS RowCounts\r\n                            FROM\r\n                                t_gl_voucher t1\r\n                                    INNER JOIN\r\n                                t_gl_doc_voucher t2 ON t1.MitemID = t2.MVoucherID\r\n                                    AND t1.MOrgID = t2.MOrgID\r\n                                    AND t1.MIsdelete = t2.MIsDelete\r\n                            WHERE\r\n                                t1.MOrgID = @MOrgID\r\n                                    AND t1.MYear = @MYear\r\n                                    AND t1.mperiod = @MPeriod\r\n                                    AND t1.MIsDelete = 0 \r\n                                    AND LENGTH(IFNULL(t1.MNumber, '')) = 0\r\n                                    AND t1.MStatus = - 1\r\n                                    and (t2.MergeStatus = 0 Or t2.MergeStatus = 1)";
			MySqlParameter[] cmdParms = new MySqlParameter[3]
			{
				new MySqlParameter("@MOrgID", ctx.MOrgID),
				new MySqlParameter("@MYear", year),
				new MySqlParameter("@MPeriod", month)
			};
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);
			object single = dynamicDbHelperMySQL.GetSingle(sql, cmdParms);
			return (single != null) ? Convert.ToInt32(single) : 0;
		}
	}
}
