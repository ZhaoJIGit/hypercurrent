using JieNor.Megi.EntityModel.LOG;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Common.Mongo.IProvider
{
	[ServiceContract]
	public interface IMLogProvider
	{
		[OperationContract]
		IList<MLog> GetMLog();

		[OperationContract]
		List<MLog> GetMLogByFieldValue(string fieldName, string value);

		[OperationContract]
		void SaveMLog(List<MLog> logs);
	}
}
