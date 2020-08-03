using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;

namespace JieNor.Megi.Common.Mongo.Provider
{
	public class MongoDataProviderBase<T>
	{
		private MongoServer server;

		private MongoDatabase database;

		protected MongoCollection<T> Collection;

		private string GetLogConnectionName(string prefix)
		{
			object[] obj = new object[8]
			{
				prefix,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			DateTime now = DateTime.Now;
			obj[1] = now.Year;
			obj[2] = "_";
			now = DateTime.Now;
			obj[3] = ((now.Month < 10) ? "0" : "");
			now = DateTime.Now;
			obj[4] = now.Month;
			obj[5] = "_";
			now = DateTime.Now;
			obj[6] = ((now.Day < 10) ? "0" : "");
			now = DateTime.Now;
			obj[7] = now.Day;
			return string.Concat(obj);
		}

		public MongoDataProviderBase(string connectionString)
		{
			MongoClient client = new MongoClient(connectionString);
			server = client.GetServer();
			string databaseName = MongoUrl.Create(connectionString).DatabaseName;
			string type = typeof(T).Name;
			if (type == "MUserLog" || type == "MLog")
			{
				string dbName = "LOG_" + DateTime.Now.ToString("yyyy_MM");
				databaseName = dbName;
			}
			database = server.GetDatabase(databaseName);
			if (type == "MUserLog" || type == "MLog")
			{
				string collectionName = GetLogConnectionName((type == "MLog") ? "d_" : "u_");
				if (!database.CollectionExists(collectionName))
				{
					database.CreateCollection(collectionName);
				}
				Collection = database.GetCollection<T>(collectionName);
				lock (base.GetType())
				{
					if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
					{
						BsonClassMap<T> classMap2 = new BsonClassMap<T>();
						InitMapping(classMap2);
						BsonClassMap.RegisterClassMap<T>((Action<BsonClassMap<T>>)InitMapping);
					}
				}
			}
			else
			{
				Collection = database.GetCollection<T>(typeof(T).Name);
				lock (base.GetType())
				{
					if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
					{
						BsonClassMap<T> classMap = new BsonClassMap<T>();
						InitMapping(classMap);
						BsonClassMap.RegisterClassMap<T>((Action<BsonClassMap<T>>)InitMapping);
					}
				}
			}
		}

		protected virtual void InitMapping(BsonClassMap<T> classMap)
		{
			classMap.AutoMap();
			classMap.SetIgnoreExtraElements(true);
		}
	}
}
