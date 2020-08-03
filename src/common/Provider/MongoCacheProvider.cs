using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.MResource;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace JieNor.Megi.Common.Mongo.Provider
{
	public class MongoCacheProvider : MongoDataProviderBase<MongoCache>, ICacheProvider
	{
		public MongoCacheProvider(string connectionString)
			: base(connectionString)
		{
		}

		public T GetData<T>(string key)
		{
			MongoCache data = base.Collection.FindOne(Query.EQ("_id", key));
			if (data != null && !string.IsNullOrEmpty(data.Data))
			{
				return JsonConvert.DeserializeObject<T>(data.Data);
			}
			return default(T);
		}

		protected override void InitMapping(BsonClassMap<MongoCache> classMap)
		{
			classMap.AutoMap();
			classMap.SetIdMember(classMap.GetMemberMap((MongoCache c) => c.MCacheKey));
			classMap.SetIgnoreExtraElements(true);
		}

		public void SaveData<T>(string key, T data)
		{
			base.Collection.Save(new MongoCache
			{
				MCacheKey = key,
				Data = JsonConvert.SerializeObject(data)
			});
		}

		public void RemoveData(string key)
		{
			base.Collection.Remove(Query.EQ("_id", key));
		}

		public void RemoveByPrefix(string prefix)
		{
			base.Collection.Remove(Query.Matches("_id", new BsonRegularExpression(new Regex(prefix + "*"))));
		}
	}
}
