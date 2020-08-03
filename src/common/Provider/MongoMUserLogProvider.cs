using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.Context;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Common.Mongo.Provider
{
	public class MongoMUserLogProvider : MongoDataProviderBase<MUserLog>, IMUserLogProvider
	{
		public MongoMUserLogProvider(string connectionString)
			: base(connectionString)
		{
		}

		protected override void InitMapping(BsonClassMap<MUserLog> classMap)
		{
			classMap.AutoMap();
			classMap.SetIdMember(classMap.GetMemberMap((MUserLog c) => c.Id));
			classMap.GetMemberMap((MUserLog c) => c.MDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.SetIgnoreExtraElements(true);
		}

		public IList<MUserLog> GetMUserLog()
		{
			return base.Collection.FindAll().ToList();
		}

		public List<MUserLog> GetMUserLogByFieldValue(string fieldName, string fieldValue)
		{
			return base.Collection.Find(Query.EQ(fieldName, fieldValue)).ToList();
		}

		public void SaveMUserLog(List<MUserLog> logs)
		{
			if (logs != null && logs.Count > 0)
			{
				base.Collection.InsertBatch(logs);
			}
		}
	}
}
