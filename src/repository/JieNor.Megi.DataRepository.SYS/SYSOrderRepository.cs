using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.Core.Repository;
using JieNor.Megi.Core.DBUtility;
using MySql.Data.MySqlClient;
using JieNor.Megi.Core;

namespace JieNor.Megi.DataRepository.SYS
{
	public class SYSOrderRepository : DataServiceT<SYSOrderModel>
	{
		public void SaveOrder(MContext ctx, SYSOrderModel orderModel, List<SYSOrderEntryModel> orderEntryModels)
		{
			MLogger.Log($"保存订单");

			InsertOrder(ctx, orderModel);
			InsertOrderItems(ctx, orderEntryModels);
		}

		private void InsertOrderItems(MContext ctx, List<SYSOrderEntryModel> orderItems)
		{
			foreach (var item in orderItems)
			{

				var commandInfo = new CommandInfo
				{
					CommandText = @"
INSERT INTO t_sys_orderentry
(MItemID,
MEntryID,
MSeq,
MQty,
MPrice,
MAmount,
MDiscountAmount,
MDesc,
MCreatorID,
MCreateDate,
MIsDelete)
VALUES
(@MItemID,
@MEntryID,
@MSeq,
@MQty,
@MPrice,
@MAmount,
@MDiscountAmount,
@MDesc,
@MCreatorID,
@MCreateDate,
0);
"
				};
				var now = DateTime.Now;
				commandInfo.Parameters = new MySqlParameter[]
				{
					new MySqlParameter("@MItemID", item.MItemID),
					new MySqlParameter("@MEntryID", item.MEntryID),
					new MySqlParameter("@MSeq", 1),
					new MySqlParameter("@MQty", item.MQty),
					new MySqlParameter("@MPrice", item.MPrice),
					//new MySqlParameter("@MPayTime", MySql.Data.MySqlClient.),
					new MySqlParameter("@MAmount", item.MAmount),
					new MySqlParameter("@MDiscountAmount", item.MDiscountAmount),
					new MySqlParameter("@MDesc", item.MDesc ?? string.Empty),

					new MySqlParameter("@MCreatorID", ctx.MUserID),
					new MySqlParameter("@MCreateDate", now),
				};

				DbHelperMySQL.ExecuteSqlTran(ctx, new List<CommandInfo>()
			{
				commandInfo
			});
			}

		}

		private static void InsertOrder(MContext ctx, SYSOrderModel orderModel)
		{
			var commandInfo = new CommandInfo
			{
				CommandText = @"INSERT INTO t_sys_order
(MItemID,
MNumber,
MOrgID,
MAmount,
MDesc,
MSubmitTime,
MBizType,
MStatus,
MActualAmount,
MPayType,
MCreatorID,
MCreateDate,
MIsDelete)
VALUES
(@MItemID,
@MNumber,
@MOrgID,
@MAmount,
@MDesc,
@MSubmitTime,
@MBizType,
@MStatus,
@MActualAmount,
@MPayType,
@MCreatorID,
@MCreateDate,
0); "
			};
			var now = DateTime.Now;
			commandInfo.Parameters = new MySqlParameter[]
			{
				new MySqlParameter("@MItemID", orderModel.MItemID),
				new MySqlParameter("@MNumber", orderModel.MNumber),
				new MySqlParameter("@MOrgID", orderModel.MOrgID),
				new MySqlParameter("@MAmount", orderModel.MAmount),
				new MySqlParameter("@MDesc", orderModel.MDesc ?? string.Empty),
				new MySqlParameter("@MSubmitTime", now),
				//new MySqlParameter("@MPayTime", MySql.Data.MySqlClient.),
				new MySqlParameter("@MBizType", 1),
				new MySqlParameter("@MStatus", orderModel.MStatus),
				new MySqlParameter("@MActualAmount", orderModel.MAmount),
				new MySqlParameter("@MPayType", orderModel.MPayType),
				new MySqlParameter("@MCreatorID", ctx.MUserID),
				new MySqlParameter("@MCreateDate", now),
			};

			DbHelperMySQL.ExecuteSqlTran(ctx, new List<CommandInfo>()
			{
				commandInfo
			});
		}

		public void SaveLog(MContext ctx, SYSOrderLogModel log)
		{
			MLogger.Log($"保存订单日志");
			ModelInfoManager.InsertOrUpdate<SYSOrderLogModel>(ctx, log);
		}

		public (SYSOrderModel, List<SYSOrderEntryModel>) GetOrderInfo(MContext ctx, string orderId)
		{
			if (string.IsNullOrEmpty(orderId))
			{
				return (null, null);
			}

			var where = new SqlWhere().Equal("MItemID", orderId).NotEqual("MOrgID", DBNull.Value.ToString());
			return (base.GetDataModelByFilter(new MContext
			{
				IsSys = true
			}, where), new SYSOrderEntryRepository().GetOrderItems(orderId));
		}

		public SYSOrderModel GetOrder(MContext ctx, string orderId)
		{
			if (string.IsNullOrEmpty(orderId))
			{
				return null;
			}
			
			var where = new SqlWhere().Equal("MItemID", orderId).NotEqual("MOrgID", DBNull.Value.ToString());
			return base.GetDataModelByFilter(new MContext
			{
				IsSys = true
			}, where);
		}

		public void Update(MContext ctx, SYSOrderModel order)
		{
			//MLogger.Log($"更新定单,order.MActualAmount=>{order.MActualAmount}");

			CommandInfo commandInfo = new CommandInfo
			{
				CommandText = @"UPDATE t_sys_order
SET
MPayTime = @MPayTime,
MStatus = @MStatus,
MCompleteTime = @MCompleteTime,
MActualAmount = @MActualAmount
WHERE MItemID = @MItemID;"
			};
			var now = DateTime.Now;
			commandInfo.Parameters = new MySqlParameter[]
			{
				new MySqlParameter("@MItemID", order.MItemID),
				new MySqlParameter("@MPayTime",  now),
				new MySqlParameter("@MCompleteTime", now),
				new MySqlParameter("@MStatus", order.MStatus),
				new MySqlParameter("@MActualAmount", order.MActualAmount)
			};

			DbHelperMySQL.ExecuteSqlTran(ctx, new List<CommandInfo>()
			{
				commandInfo
			});
		}
	}


	public class SYSOrderEntryRepository : DataServiceT<SYSOrderEntryModel>
	{
		public List<SYSOrderEntryModel> GetOrderItems(string orderId)
		{
			var where = new SqlWhere().Equal("MEntryID", orderId);
			return GetModelList(new MContext
			{
				IsSys = true
			}, where);
		}
	}

}
