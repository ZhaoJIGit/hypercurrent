using JieNor.Megi.BusinessContract.COM;
using JieNor.Megi.BusinessService.COM;
using JieNor.Megi.Core.ServiceModel;
using JieNor.Megi.DataModel.BAS;
using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using JieNor.Megi.ServiceContract.COM;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Web.COM
{
	public class COMTableInfoService : ServiceT<BASLangModel>, ICOMTableInfo
	{
		private ICOMTableInfoBusiness biz = new COMTableInfoBusiness();

		public MActionResult<List<List<MTableColumnModel>>> GetTableColumnModels(string tableName, string accessToken = null)
		{
			ICOMTableInfoBusiness iCOMTableInfoBusiness = biz;
			return base.RunFunc(iCOMTableInfoBusiness.GetTableColumnModels, tableName, accessToken);
		}
	}
}
