using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;
using System.ServiceModel;

namespace JieNor.Megi.Common.Mongo.IProvider
{
	[ServiceContract]
	public interface IMUserLogProvider
	{
		[OperationContract]
		IList<MUserLog> GetMUserLog();

		[OperationContract]
		List<MUserLog> GetMUserLogByFieldValue(string fieldName, string value);

		[OperationContract]
		void SaveMUserLog(List<MUserLog> logs);
	}
}
