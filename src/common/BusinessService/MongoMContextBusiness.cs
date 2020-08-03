using JieNor.Megi.Common.Mongo.BusinessContract;
using JieNor.Megi.Common.Mongo.Factory;
using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.Context;
using System.Collections.Generic;

namespace JieNor.Megi.Common.Mongo.BusinessService
{
	public class MongoMContextBusiness : IMongoMContextBusiness, IMContextProvider
	{
		private IMContextProvider _provicer;

		private IMContextProvider MContextProvider = DataQueryingCacheFactory.BuilderMContextProvider();

		private IMContextProvider ExcelMContextProvider = DataQueryingCacheFactory.BuilderExcelMContextProvider();

		private IMContextProvider ApiMContextProvider = DataQueryingCacheFactory.BuilderApiMContextProvider();

		public MongoMContextBusiness(string app = "System")
		{
			_provicer = GetProvider(app);
		}

		private IMContextProvider GetProvider(string app = "System")
		{
			switch (app)
			{
			case "System":
				return MContextProvider;
			case "Excel":
				return ExcelMContextProvider;
			case "Api":
				return ApiMContextProvider;
			default:
				return MContextProvider;
			}
		}

		public void SaveMContext(MContext context, bool refresh = true)
		{
			_provicer.SaveMContext(context, refresh);
		}

		public MContext GetMContextByAccessToken(string AccessToken)
		{
			return _provicer.GetMContextByAccessToken(AccessToken);
		}

		public MContext GetMContextByEmail(string email)
		{
			return _provicer.GetMContextByEmail(email);
		}

		public string GetMEmailByUserIp(string userIp)
		{
			return _provicer.GetMEmailByUserIp(userIp);
		}

		public IList<MContext> GetMContext()
		{
			return _provicer.GetMContext();
		}

		public MContext GetMContextByUserId(string userId)
		{
			return _provicer.GetMContextByUserId(userId);
		}

		public void RemoveMContextByUserId(string userId)
		{
			_provicer.RemoveMContextByUserId(userId);
		}

		public void RemoveMContextByAccessToken(string accessToken)
		{
			_provicer.RemoveMContextByAccessToken(accessToken);
		}

		public void ExpireMContextByUserId(string userId)
		{
			_provicer.ExpireMContextByUserId(userId);
		}

		public void ExpireMContextByAccessToken(string accessToken)
		{
			_provicer.ExpireMContextByAccessToken(accessToken);
		}

		public MContext GetMContextByAccessCode(string accessCode)
		{
			return _provicer.GetMContextByAccessCode(accessCode);
		}

		public void RefreshMContextByAccessToken(string accessCode)
		{
			_provicer.RemoveMContextByAccessToken(accessCode);
		}

		public void RefreshMContextByUserId(string userId)
		{
			_provicer.RefreshMContextByUserId(userId);
		}

		public void UpdateMContextByKeyField(string keyName, string keyValue, string fieldName, object fieldValue, bool syn)
		{
			if (syn)
			{
				MContextProvider.UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn);
				ExcelMContextProvider.UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn);
				ApiMContextProvider.UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn);
			}
			else
			{
				_provicer.UpdateMContextByKeyField(keyName, keyValue, fieldName, fieldValue, syn);
			}
		}

		public void RemoveMContext(Dictionary<string, string> queryDictionary)
		{
			_provicer.RemoveMContext(queryDictionary);
		}
	}
}
