using JieNor.Megi.BusinessContract.SYS;
using JieNor.Megi.BusinessService.SYS;
using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.EntityModel.SYS;
using JieNor.Megi.ServiceContract.SYS;

namespace JieNor.Megi.Service.Web.SYS
{
	public class SYSOrderService : ServiceT<SYSOrderModel>, ISYSOrder
	{
		private ISYSOrderBusiness _orderBusiness=new SYSOrderBusiness();


		public MActionResult<OperationResult> CreateOrder(SysCreateOrderModel createOrderModel, string accessToken = null)
		{
			var rtn= base.RunFunc(_orderBusiness.CreateOrder, createOrderModel, accessToken);
			return rtn;
		}


		public MActionResult<SYSOrderEntry> GetOrder(string orderId, string accessToken = null)
		{
			return RunFunc(_orderBusiness.GetOrder, orderId, accessToken);
		}

		public MActionResult<OperationResult> PayForOrder(string orderId,decimal amount, string accessToken = null)
		{
			return RunFunc(_orderBusiness.PayForOrder, orderId,amount, accessToken);
		}
	}
}
