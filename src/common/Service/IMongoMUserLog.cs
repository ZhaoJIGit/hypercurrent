using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Common.Mongo.Service
{
	[ServiceContract]
	public interface IMongoMUserLog
	{
		[OperationContract]
		IList<MUserLog> GetMUserLog();

		[OperationContract]
		List<MUserLog> GetMUserLogByFieldValue(string fieldName, string feildValue);

		[OperationContract]
		void SaveMUserLog(List<MUserLog> logs);
	}
}
