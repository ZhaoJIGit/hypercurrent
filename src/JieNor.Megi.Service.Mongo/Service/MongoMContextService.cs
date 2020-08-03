using JieNor.Megi.Common.Mongo.BusinessService;
using JieNor.Megi.Common.Mongo.Factory;
using JieNor.Megi.Common.Mongo.Service;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.Service.Mongo.Service
{
	public class MongoMContextService : IMongoMContext
	{
		public void SaveMContext(MContext context, bool refresh = true, string app = "System")
		{
			new MongoMContextBusiness(app).SaveMContext(context, refresh);
		}

		public MContext GetMContextByAccessToken(string accessToken, string app = "System")
		{
			return new MongoMContextBusiness(app).GetMContextByAccessToken(accessToken);
		}

		public IList<MContext> GetMContext(string app = "System")
		{
			return new MongoMContextBusiness(app).GetMContext();
		}

		public MContext GetMContextByUserId(string userId, string app = "System")
		{
			return new MongoMContextBusiness(app).GetMContextByUserId(userId);
		}

		public string GetMEmailByUserIp(string userIp, string app = "System")
		{
			return new MongoMContextBusiness(app).GetMEmailByUserIp(userIp);
		}

		public void RemoveMContextByUserId(string userId, string app = "System")
		{
			new MongoMContextBusiness(app).RemoveMContextByUserId(userId);
		}

		public void RemoveMContextByAccessToken(string accessToken, string app = "System")
		{
			new MongoMContextBusiness(app).RemoveMContextByAccessToken(accessToken);
		}

		public void ExpireMContextByUserId(string userId, string app = "System")
		{
			new MongoMContextBusiness(app).ExpireMContextByUserId(userId);
		}

		public void ExpireMContextByAccessToken(string accessToken, string app = "System")
		{
			new MongoMContextBusiness(app).ExpireMContextByAccessToken(accessToken);
		}

		public MContext GetMContextByAccessCode(string accessCode, string app = "System")
		{
			return DataQueryingCacheFactory.BuilderMContextProvider().GetMContextByAccessCode(accessCode);
		}

		public void RefreshMContextByAccessToken(string accessCode, string app = "System")
		{
			new MongoMContextBusiness(app).RemoveMContextByAccessToken(accessCode);
		}

		public void RefreshMContextByUserId(string userId, string app = "System")
		{
			new MongoMContextBusiness(app).RefreshMContextByUserId(userId);
		}

		public MContext GetMContextByEmail(string email, string app = "System")
		{
			return new MongoMContextBusiness(app).GetMContextByEmail(email);
		}

		public void UpdateMContextByKeyField(string keyName, string keyValue, string fieldName, object fieldValue, bool syn = true, string app = "System")
		{
			new MongoMContextBusiness(app).UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn);
		}

		public void RemoveMContext(Dictionary<string, string> queryDictionary, string app = "System")
		{
			new MongoMContextBusiness(app).RemoveMContext(queryDictionary);
		}
	}
}
