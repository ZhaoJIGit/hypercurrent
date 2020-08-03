using JieNor.Megi.Common.Mongo.BusinessContract;
using JieNor.Megi.Common.Mongo.Factory;
using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.MResource;
using System.Collections.Generic;

namespace JieNor.Megi.Common.Mongo.BusinessService
{
	public class MongoMResourceBusiness : IMongoMResourceBusiness, IMResourceProvider
	{
		public List<JieNor.Megi.EntityModel.MResource.MResource> Get(string accessToken = null, string userId = null, string orgId = null)
		{
			return DataQueryingCacheFactory.BuilderMResourceProvider().Get(accessToken, userId, orgId);
		}

		public void Save(List<JieNor.Megi.EntityModel.MResource.MResource> resources)
		{
			DataQueryingCacheFactory.BuilderMResourceProvider().Save(resources);
		}

		public void Remove(string accessToken = null, string userId = null, string orgId = null)
		{
			DataQueryingCacheFactory.BuilderMResourceProvider().Remove(accessToken, userId, orgId);
		}

		public void RemoveWithIDs(IEnumerable<string> ids)
		{
			DataQueryingCacheFactory.BuilderMResourceProvider().RemoveWithIDs(ids);
		}
	}
}
