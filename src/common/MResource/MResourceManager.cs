using JieNor.Megi.Common.Mongo.BusinessService;
using JieNor.Megi.Common.Mongo.Service;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.MResource;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace JieNor.Megi.Common.MResource
{
	public class MResourceManager
	{
		public static string GetMongoServieType = ConfigurationManager.AppSettings["GetMongoServieType"];

		private static bool GetMongoServiceFromWCF => string.IsNullOrEmpty(GetMongoServieType) || GetMongoServieType.Equals("1");

		public static List<JieNor.Megi.EntityModel.MResource.MResource> Get(string accessToken = null, string userId = null, string orgId = null)
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMResource mContextService = ServiceHostManager.GetMongoService<IMongoMResource>();
				using (mContextService as IDisposable)
				{
					return mContextService.Get(accessToken, userId, orgId);
				}
			}
			return new MongoMResourceBusiness().Get(accessToken, userId, orgId);
		}

		public static void Save(List<JieNor.Megi.EntityModel.MResource.MResource> resources)
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMResource mContextService = ServiceHostManager.GetMongoService<IMongoMResource>();
				using (mContextService as IDisposable)
				{
					mContextService.Save(resources);
				}
			}
			else
			{
				new MongoMResourceBusiness().Save(resources);
			}
		}

		public static void Remove(string accessToken = null, string userId = null, string orgId = null)
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMResource mContextService = ServiceHostManager.GetMongoService<IMongoMResource>();
				using (mContextService as IDisposable)
				{
					mContextService.Remove(accessToken, userId, orgId);
				}
			}
			else
			{
				new MongoMResourceBusiness().Remove(accessToken, userId, orgId);
			}
		}

		public static void RemoveWithIDs(IEnumerable<string> ids)
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMResource mContextService = ServiceHostManager.GetMongoService<IMongoMResource>();
				using (mContextService as IDisposable)
				{
					mContextService.RemoveWithIDs(ids);
				}
			}
			else
			{
				new MongoMResourceBusiness().RemoveWithIDs(ids);
			}
		}
	}
}
