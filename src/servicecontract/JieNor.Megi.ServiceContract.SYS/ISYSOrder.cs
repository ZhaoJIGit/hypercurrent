using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.SYS;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;
using JieNor.Megi.EntityModel.SYS;

namespace JieNor.Megi.ServiceContract.SYS
{
	[ServiceContract]
	public interface ISYSOrder
	{
		[OperationContract]
		MActionResult<OperationResult> CreateOrder(SysCreateOrderModel createOrderModel, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> PayForOrder(string orderId, decimal amount, string accessToken = null);

		[OperationContract]
		MActionResult<SYSOrderEntry> GetOrder(string orderId, string accessToken = null);
	}
}
