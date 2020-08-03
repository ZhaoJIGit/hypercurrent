using JieNor.Megi.Core;
using JieNor.Megi.Core.Log;
using JieNor.Megi.EntityModel.Context;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.BAS
{
	[ServiceContract]
	public interface IOptLogList
	{
		[OperationContract]
		MActionResult<DataGridJson<OptLogListModel>> GetOptLogList(OptLogListFilter filter, string accessToken = null);
	}
}
