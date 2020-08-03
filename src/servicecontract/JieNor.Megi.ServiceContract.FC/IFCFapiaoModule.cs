using JieNor.Megi.Core;
using JieNor.Megi.Core.Context;
using JieNor.Megi.DataModel.FC;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.FC
{
	[ServiceContract]
	public interface IFCFapiaoModule
	{
		[OperationContract]
		MActionResult<OperationResult> SaveFapiaoModule(FCFapiaoModuleModel model, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteFapiaoModules(List<string> ids, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<FCFapiaoModuleModel>> GetFapiaoModulePageList(FCFastCodeFilterModel filter, string accessToken = null);

		[OperationContract]
		MActionResult<List<FCFapiaoModuleModel>> GetFapiaoModuleList(FCFastCodeFilterModel fitler, string accessToken = null);

		[OperationContract]
		MActionResult<int> GetFapiaoModulePageListCount(string accessToken = null);
	}
}
