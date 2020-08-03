using JieNor.Megi.EntityModel.COM;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.ServiceContract.COM
{
	[ServiceContract]
	public interface ICOMTableInfo
	{
		[OperationContract]
		MActionResult<List<List<MTableColumnModel>>> GetTableColumnModels(string tableName, string accessToken = null);
	}
}
