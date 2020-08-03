using JieNor.Megi.Core.Context;
using JieNor.Megi.Core.DataModel;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Core.ServiceModel
{
	[ServiceContract]
	public interface IServiceContract<T> where T : BaseModel
	{
		[OperationContract]
		MActionResult<bool> Exists(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<bool> ExistsByFilter(SqlWhere filter, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdate(T modelData, string fields = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> InsertOrUpdateModels(List<T> modelData, string fields = null, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> Delete(string pkID, string accessToken = null);

		[OperationContract]
		MActionResult<OperationResult> DeleteModels(List<string> pkID, string accessToken = null);

		[OperationContract]
		MActionResult<T> GetDataModelByFilter(SqlWhere filter, string accessToken = null);

		[OperationContract]
		MActionResult<T> GetDataModel(string pkID, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<List<T>> GetModelList(SqlWhere filter, bool includeDelete = false, string accessToken = null);

		[OperationContract]
		MActionResult<DataGridJson<T>> GetModelPageList(SqlWhere filter, bool includeDelete = false, string accessToken = null);
	}
}
