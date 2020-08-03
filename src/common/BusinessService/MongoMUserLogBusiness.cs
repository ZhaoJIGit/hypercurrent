using JieNor.Megi.Common.Mongo.BusinessContract;
using JieNor.Megi.Common.Mongo.Factory;
using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.Common.Mongo.BusinessService
{
	public class MongoMUserLogBusiness : IMongoMUserLogBusiness, IMUserLogProvider
	{
		public void SaveMUserLog(List<MUserLog> logs)
		{
			DataQueryingCacheFactory.BuilderMUserLogProvider().SaveMUserLog(logs);
		}

		public List<MUserLog> GetMUserLogByFieldValue(string fieldName, string fieldValue)
		{
			return DataQueryingCacheFactory.BuilderMUserLogProvider().GetMUserLogByFieldValue(fieldName, fieldValue);
		}

		public IList<MUserLog> GetMUserLog()
		{
			return DataQueryingCacheFactory.BuilderMUserLogProvider().GetMUserLog();
		}
	}
}
