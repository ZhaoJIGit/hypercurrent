using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IBASCurrency
	{
		[OperationContract]
		MActionResult<List<BASCurrencyModel>> GetList(string accessToken = null);

		[OperationContract]
		MActionResult<List<BASCurrencyViewModel>> GetViewList(bool flag = true, string accessToken = null);

		[OperationContract]
		MActionResult<BASCurrencyModel> GetModel(BASCurrencyModel model, string accessToken = null);

		[OperationContract]
		MActionResult<BASCurrencyViewModel> GetViewModel(BASCurrencyModel model, string accessToken = null);
	}
}
