using JieNor.Megi.Common.Mongo.BusinessContract;
using JieNor.Megi.Common.Mongo.Factory;
using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.LOG;
using System.Collections.Generic;

namespace JieNor.Megi.Common.Mongo.BusinessService
{
	public class MongoMLogBusiness : IMongoMLogBusiness, IMLogProvider
	{
		public void SaveMLog(List<MLog> logs)
		{
			DataQueryingCacheFactory.BuilderMLogProvider().SaveMLog(logs);
		}

		public List<MLog> GetMLogByFieldValue(string fieldName, string fieldValue)
		{
			return DataQueryingCacheFactory.BuilderMLogProvider().GetMLogByFieldValue(fieldName, fieldValue);
		}

		public IList<MLog> GetMLog()
		{
			return DataQueryingCacheFactory.BuilderMLogProvider().GetMLog();
		}
	}
}
