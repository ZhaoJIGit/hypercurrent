using JieNor.Megi.BusinessContract.SYS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.DataRepository.SYS;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using JieNor.Megi.Common.Logger;
using JieNor.Megi.EntityModel.SYS;
using JieNor.Megi.Core.Attribute;
using Newtonsoft.Json;
using System.Text;
using JieNor.Megi.DataRepository.COM;

namespace JieNor.Megi.BusinessService.SYS
{
	public class SYSOrderBusiness : ISYSOrderBusiness
	{

		SYSOrderRepository _dal = new SYSOrderRepository();

		void LogOrder(MContext ctx, string orderId, string message)
		{
			var log = new SYSOrderLogModel()
			{
				MItemID = Guid.NewGuid().ToString("N"),
				MOrderID = orderId,
				MDesc = message, //todo: multi language
			};

			_dal.SaveLog(ctx, log);
		}
		public OperationResult CreateOrder(MContext ctx, SysCreateOrderModel createOrderModel)
		{
			//MLogger.Log($"创建订单:{createOrderModel.OrgId}");
			var rtn = new OperationResult();

			var order = new SYSOrderModel
			{
				MNumber = GetNumber(),
				MItemID = Guid.NewGuid().ToString("N"),
				MPayType = (SYSPayType)createOrderModel.MPayType
			};
			var plan= COMAccess.GetPlan(ctx).Select(x => x.Code); ;


			var items = createOrderModel.Items.Select(x => GetOrderEntry(order.MItemID, x,plan.ToList())).ToList();
			order.MOrgID = createOrderModel.OrgId;
			order.MAmount = items.Sum(x => x.MAmount);
			order.MStatus = SYSOrderStatus.WatiPay;

			_dal.SaveOrder(ctx, order, items);

			//LogOrder(ctx, order.MItemID,"Create Order");	
			//rtn.SuccessModel=new List<BaseModel>(){order};
			rtn.ObjectID = order.MItemID;
			rtn.Success = true;

			return rtn;
		}

		public string GetNumber()
		{
			//生成订单号
			//订单号生成原则：年（4位）+月（2位）+日（2位）+时（2位）+分（2位）+秒（2位）+商家编号（5位，不够左补0）+5位随机数，2018 10 10 21 30 2 1      00001 43261

			// 商家编号（5位，不够左补0）
			//string merchant = MerchantID.ToString();
			//merchant = merchant.PadLeft(5, '0');     // 共5位，/*之前用0补齐*/

			string num = GetRandomString(5);//自动生成一个5位随机数

			string ordernum = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd") +
			DateTime.Now.ToString("hh") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss") + num;

			return ordernum;
		}

		public static string GetRandomString(int iLength)
		{
			string buffer = "0123456789";// 随机字符中也可以为汉字（任何）  
			StringBuilder sb = new StringBuilder();
			Random r = new Random();
			int range = buffer.Length;
			for (int i = 0; i < iLength; i++)
			{
				sb.Append(buffer.Substring(r.Next(range), 1));
			}
			return sb.ToString();
		}

		//		Quarterly:
		//Fee: 687
		//Discount: -34.35
		//Total: 652.65

		//Bi-annually: 
		//Fee: 1374
		//Discount: -137.40
		//Total: 1236.60

		//Annually: 
		//Fee: 2748.00
		//Discount: 549.60
		//Total: 2198.40

		(decimal, decimal) GetSkuPrice(string skuId, List<string> plans)
		{
			if(string.IsNullOrEmpty(skuId))
				throw new Exception("不支持的订阅");
			var sku = skuId.ToLower();

			//标准版
			bool flag_normal = plans.Contains("NORMAL");
			//销售
			bool flag_sales = plans.Contains("SALES");
			//发票
			bool flag_invoice = plans.Contains("INVOICE");

			if (flag_normal || (flag_sales && flag_invoice))
			{
				switch (sku)
				{
					case "quarterly":
						return (1197, 1197);
					case "biannually":
						return (2394, (decimal)2274.3);
					case "annually":
						return (4788, (decimal)4309.2);
					default:
						throw new Exception("不支持的订阅");
				}
			}
			else if (flag_sales)
			{
				switch (sku)
				{
					case "quarterly":
						return (597, 597);
					case "biannually":
						return (1194, (decimal)1134.3);
					case "annually":
						return (2388, (decimal)2149.2);
					default:
						throw new Exception("不支持的订阅");
				}

			}
			else if (flag_invoice)
			{
				switch (sku)
				{
					case "quarterly":
						return (897, 897);
					case "biannually":
						return (1794, (decimal)1704.3);
					case "annually":
						return (3588, (decimal)3229.2);
					default:
						throw new Exception("不支持的订阅");
				}

			}
			else {
				throw new Exception("不支持的订阅");
			}
		}

		SYSOrderEntryModel GetOrderEntry(string orderId, SysCreateOrderItemModel item,List<string> plans)
		{
			var (price, amount) = GetSkuPrice(item.SkuId,plans);
			var entry = new SYSOrderEntryModel()
			{
				MEntryID = orderId,
				MItemID = Guid.NewGuid().ToString("N"),
				MQty = item.Qty,
				MPrice = price,
				MAmount = amount
			};
			entry.MDiscountAmount = entry.MPrice - entry.MAmount;
			return entry;
		}

		[NoAuthorization]
		public OperationResult PayForOrder(MContext ctx, string orderId, decimal amount)
		{
			var rtn = new OperationResult();
			var (order,items) = _dal.GetOrderInfo(ctx, orderId);

			order.MActualAmount = amount;
			order.MStatus = SYSOrderStatus.Paid;
			_dal.Update(ctx, order);

			// 新增订阅
			var months = items.Sum(x => x.MQty);
			var organisationBusiness = new BASOrganisationBusiness();
			var org = organisationBusiness.GetDataModel(ctx, order.MOrgID);
			organisationBusiness.Subscribe(ctx, org, months, order.MCreatorID);

			
			//var newExpiredDate = org.MExpiredDate.AddMonths(month);
			//org.MExpiredDate = newExpiredDate;

			//var resut = organisationBusiness.Update(ctx, org);
			//MLogger.Log(JsonConvert.SerializeObject(resut));
			//todo: update org expiredDate

			//LogOrder(ctx, order.MItemID,"Pay for order");

			rtn.Success = true;
			return rtn;
		}

		public SYSOrderEntry GetOrder(MContext ctx, string orderId)
		{
			var order = _dal.GetOrder(ctx, orderId);
			if (null == order)
				return null;

			return new SYSOrderEntry
			{
				MItemID = order.MItemID,
				MNumber = order.MNumber,
				MOrgID = order.MOrgID,
				MAmount = order.MAmount,
				//MDesc = order.MDesc,
				MSubmitTime = order.MSubmitTime,
				MPayTime = order.MPayTime,
				//MBizType = order.MBizType,
				MStatus = order.MStatus,
				MCancelTime = order.MCancelTime,
				MCompleteTime = order.MCompleteTime,
				//MOutOrderId = order.MOutOrderId,
				//MOutFee = order.MOutFee,
				MActualAmount = order.MActualAmount,
				MPayType = order.MPayType,
				//MPayAccountType = order.MPayAccountType
			};
		}
	}
}
