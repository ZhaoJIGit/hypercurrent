using JieNor.Megi.Common.Mongo.BusinessService;
using JieNor.Megi.Common.Mongo.Service;
using JieNor.Megi.Common.ServiceManager;
using JieNor.Megi.EntityModel.Context;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace JieNor.Megi.Common.Context
{
	public class MContextManager
	{
		public static string GetMongoServieType = ConfigurationManager.AppSettings["GetMongoServieType"];

		private static bool GetMongoServiceFromWCF => string.IsNullOrEmpty(GetMongoServieType) || GetMongoServieType.Equals("1");

		public static IList<MContext> GetMContext(string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					return mContextService.GetMContext(app);
				}
			}
			return new MongoMContextBusiness(app).GetMContext();
		}

		public static void UpdateMContextByKeyField(string keyName, string keyValue, string fieldName, object fieldValue, bool syn, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn);
			}
		}

		public static void RemoveMContext(Dictionary<string, string> queryDictionary, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.RemoveMContext(queryDictionary, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).RemoveMContext(queryDictionary);
			}
		}

		public static MContext GetMContextByUserId(string userId, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					return mContextService.GetMContextByUserId(userId, app);
				}
			}
			return new MongoMContextBusiness(app).GetMContextByUserId(userId);
		}

		public static string GetMEmailByUserIp(string userIp, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					return mContextService.GetMEmailByUserIp(userIp, app);
				}
			}
			return new MongoMContextBusiness(app).GetMEmailByUserIp(userIp);
		}

		public static MContext GetMContextByAccessToken(string accessToken, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					return mContextService.GetMContextByAccessToken(accessToken, app);
				}
			}
			return new MongoMContextBusiness(app).GetMContextByAccessToken(accessToken);
		}

		public static MContext GetMContextByEmail(string email, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					return mContextService.GetMContextByEmail(email, app);
				}
			}
			return new MongoMContextBusiness(app).GetMContextByEmail(email);
		}

		public static void ExpireMContextByUserId(string userId, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.ExpireMContextByUserId(userId, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).ExpireMContextByUserId(userId);
			}
		}

		public static void ExpireMContextByAccessToken(string accessToken, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.ExpireMContextByAccessToken(accessToken, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).ExpireMContextByAccessToken(accessToken);
			}
		}

		public static void SaveMContext(MContext context, bool refresh = true, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.SaveMContext(context, refresh, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).SaveMContext(context, refresh);
			}
		}

		public static void RemoveMContextByUserId(string userId, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.RemoveMContextByUserId(userId, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).RemoveMContextByUserId(userId);
			}
		}

		public static void RemoveMContextByAccessToken(string accessToken, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.GetMContextByUserId(accessToken, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).GetMContextByUserId(accessToken);
			}
		}

		public static void RefreshMContextByUserId(string userId, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.GetMContextByUserId(userId, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).GetMContextByUserId(userId);
			}
		}

		public static void RefreshMContextByAccessToken(string userId, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					mContextService.GetMContextByUserId(userId, app);
				}
			}
			else
			{
				new MongoMContextBusiness(app).GetMContextByUserId(userId);
			}
		}

		public static MContext GetMContextByAccessCode(string accessCode, string app = "System")
		{
			if (GetMongoServiceFromWCF)
			{
				IMongoMContext mContextService = ServiceHostManager.GetMongoService<IMongoMContext>();
				using (mContextService as IDisposable)
				{
					return mContextService.GetMContextByAccessCode(accessCode, app);
				}
			}
			return new MongoMContextBusiness(app).GetMContextByUserId(accessCode);
		}
	}
}
