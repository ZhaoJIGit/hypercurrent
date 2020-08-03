using JieNor.Megi.BusinessContract.BAS;
using JieNor.Megi.BusinessService.BAS;
using JieNor.Megi.Core;
using JieNor.Megi.Core.Log;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.BAS;

namespace JieNor.Megi.Service.Web.BAS
{
	public class OptLogListService : ServiceT<BASCodeValueModel>, IOptLogList
	{
		private readonly IOptLogListBusiness biz = new OptLogListBusiness();

		public MActionResult<DataGridJson<OptLogListModel>> GetOptLogList(OptLogListFilter filter, string accessToken = null)
		{
			IOptLogListBusiness optLogListBusiness = biz;
			return base.RunFunc(optLogListBusiness.GetOptLogList, filter, accessToken);
		}
	}
}
