using JieNor.Megi.EntityModel.LOG;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Common.Mongo.Service
{
	[ServiceContract]
	public interface IMongoMLog
	{
		[OperationContract]
		IList<MLog> GetMLog();

		[OperationContract]
		MLog GetMLogByFieldValue(string fieldName, string feildValue);

		[OperationContract]
		void SaveMLog(List<MLog> logs);
	}
}
