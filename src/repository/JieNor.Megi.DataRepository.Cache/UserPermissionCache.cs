using JieNor.Megi.Common.Cache;
using JieNor.Megi.Core.CacheManager;
using JieNor.Megi.DataModel.SEC;
using System;
using System.Collections.Generic;

namespace JieNor.Megi.DataRepository.Cache
{
	[Obsolete]
	public class UserPermissionCache : CacheMangerBase<List<SECUserPermissionModel>>
	{
		private static Dictionary<string, List<SECUserPermissionModel>> userPermissions = new Dictionary<string, List<SECUserPermissionModel>>();

		private static string cacheKey = "UP";

		public static UserPermissionCache _obj;

		public static UserPermissionCache Instance
		{
			get
			{
				if (_obj == null)
				{
					_obj = new UserPermissionCache();
				}
				return _obj;
			}
		}

		public UserPermissionCache()
			: base((ICacheProvider)new MongoProvider())
		{
			Subscribe();
		}

		protected override string GetTypePrefix()
		{
			return cacheKey;
		}

		private void Subscribe()
		{
			RedisProvider.Subscribe(cacheKey, delegate(string t)
			{
				userPermissions.Remove(t);
			});
		}

		public override void RemoveData(string key)
		{
			base.RemoveData(key);
			RedisProvider.Publish(cacheKey, key);
		}

		public override List<SECUserPermissionModel> GetData(string key)
		{
			userPermissions.TryGetValue(key, out List<SECUserPermissionModel> data);
			if (data == null)
			{
				data = base.GetData(key);
				if (data == null)
				{
					return null;
				}
				try
				{
					userPermissions.Add(key, data);
				}
				catch
				{
					userPermissions.TryGetValue(key, out data);
				}
			}
			return data;
		}
	}
}
