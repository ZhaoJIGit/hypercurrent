using JieNor.Megi.Common.Mongo.BusinessContract;
using JieNor.Megi.Common.Mongo.BusinessService;
using JieNor.Megi.Common.Mongo.Service;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Mongo.Service
{
	public class MongoMUserLogService : IMongoMUserLog
	{
		public IMongoMUserLogBusiness biz = new MongoMUserLogBusiness();

		public IList<MUserLog> GetMUserLog()
		{
			return biz.GetMUserLog();
		}

		public List<MUserLog> GetMUserLogByFieldValue(string fieldName, string feildValue)
		{
			return biz.GetMUserLogByFieldValue(fieldName, feildValue);
		}

		public void SaveMUserLog(List<MUserLog> logs)
		{
			biz.SaveMUserLog(logs);
		}
	}
}
