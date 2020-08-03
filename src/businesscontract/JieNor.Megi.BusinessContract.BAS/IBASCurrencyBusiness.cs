using JieNor.Megi.Core.DataModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.BusinessContract.BAS
{
	public interface IBASCurrencyBusiness : IDataContract<BASCurrencyModel>
	{
		List<BASCurrencyModel> GetList(MContext context);

		List<BASCurrencyViewModel> GetViewList(MContext context, bool flag);

		BASCurrencyModel GetModel(MContext ctx, BASCurrencyModel model);

		BASCurrencyViewModel GetViewModel(MContext ctx, BASCurrencyModel model);
	}
}
