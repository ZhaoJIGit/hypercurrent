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
using System.Text;
using System.Collections;

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
MIsDelete,
HBFQNum,
HbFqSellerPercent)
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
0,
@HBFQNum,
@HbFqSellerPercent); "
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
				new MySqlParameter("@HBFQNum", orderModel.HBFQNum),
				new MySqlParameter("@HbFqSellerPercent", orderModel.HbFqSellerPercent),

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
		public List<SYSOrderEntry> GetOrderList(MContext ctx, SqlWhere filter)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("SELECT * FROM t_sys_order");
			if (filter != null && !string.IsNullOrEmpty(filter.WhereSqlString))
			{
				stringBuilder.AppendLine(filter.WhereSqlString);
			}
			stringBuilder.AppendLine("order by MCreateDate desc");

			ArrayList arrayList = new ArrayList();
			if (filter != null && filter.Parameters.Length != 0)
			{
				MySqlParameter[] parameters = filter.Parameters;
				foreach (MySqlParameter value in parameters)
				{
					arrayList.Add(value);
				}
			}
			MySqlParameter[] array = (MySqlParameter[])arrayList.ToArray(typeof(MySqlParameter));
			DynamicDbHelperMySQL dynamicDbHelperMySQL = new DynamicDbHelperMySQL(ctx);

			var dt = dynamicDbHelperMySQL.Query(stringBuilder.ToString(), array).Tables[0];
			List<SYSOrderEntry> list = new List<SYSOrderEntry>();
			for (int i = 0; i < dt.Rows.Count; i++)
            {
				var row = dt.Rows[i];
				SYSOrderEntry entry = new SYSOrderEntry();
				entry.MPayTime = row["MPayTime"].ToMDateTime();
				entry.MItemID = row["MItemID"].ToString();
				entry.MSubmitTime = row["MSubmitTime"].ToMDateTime();
				string name = Enum.GetName(typeof(SYSOrderStatus), row["MStatus"].ToMInt32());
				entry.MStatus = (SYSOrderStatus)Enum.Parse(typeof(SYSOrderStatus), name) ;
				string type = Enum.GetName(typeof(SYSPayType), row["MPayType"].ToMInt32());
				entry.MPayType = (SYSPayType)Enum.Parse(typeof(SYSPayType), type);

				entry.MCompleteTime = row["MCompleteTime"].ToMDateTime();
				entry.MNumber = row["MNumber"].ToString();
				entry.MAmount = row["MAmount"].ToMDecimal();
				entry.MActualAmount = row["MActualAmount"].ToMDecimal();
				entry.MOrgID = row["MOrgID"].ToString();
				list.Add(entry);
			}
			return list;

		}
	}

}
