using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.LOG;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Common.Mongo.Provider
{
	public class MongoMLogProvider : MongoDataProviderBase<MLog>, IMLogProvider
	{
		public MongoMLogProvider(string connectionString)
			: base(connectionString)
		{
		}

		protected override void InitMapping(BsonClassMap<MLog> classMap)
		{
			classMap.AutoMap();
			classMap.SetIdMember(classMap.GetMemberMap((MLog c) => c.Id));
			classMap.GetMemberMap((MLog c) => c.MDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.SetIgnoreExtraElements(true);
		}

		public IList<MLog> GetMLog()
		{
			return base.Collection.FindAll().ToList();
		}

		public List<MLog> GetMLogByFieldValue(string fieldName, string fieldValue)
		{
			return base.Collection.Find(Query.EQ(fieldName, fieldValue)).ToList();
		}

		public void SaveMLog(List<MLog> logs)
		{
			if (logs != null && logs.Count > 0)
			{
				base.Collection.InsertBatch(logs);
			}
		}
	}
}
