using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.SYS;

namespace JieNor.Megi.BusinessContract.SYS
{
	public interface ISYSOrderBusiness
	{
		OperationResult CreateOrder(MContext ctx, SysCreateOrderModel createOrderModel);


		OperationResult PayForOrder(MContext ctx, string orderId, decimal amount);

		SYSOrderEntry GetOrder(MContext ctx, string orderId);
		
		DataGridJson<SYSOrderEntry> GetOrderList(MContext ctx, string orgId);

	}
}
