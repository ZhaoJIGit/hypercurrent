using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.MResource;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Common.Mongo.Provider
{
	public class MongoMResourceProvider : MongoDataProviderBase<JieNor.Megi.EntityModel.MResource.MResource>, IMResourceProvider
	{
		public MongoMResourceProvider(string connectionString)
			: base(connectionString)
		{
		}

		protected override void InitMapping(BsonClassMap<JieNor.Megi.EntityModel.MResource.MResource> classMap)
		{
			classMap.AutoMap();
			classMap.SetIdMember(classMap.GetMemberMap((JieNor.Megi.EntityModel.MResource.MResource c) => c.MID));
			classMap.GetMemberMap((JieNor.Megi.EntityModel.MResource.MResource c) => c.MCreateDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.SetIgnoreExtraElements(true);
		}

		public List<JieNor.Megi.EntityModel.MResource.MResource> Get(string accessToken = null, string userId = null, string orgId = null)
		{
			IMongoQuery[] querys = new IMongoQuery[0];
			List<IMongoQuery> queryList = querys.ToList();
			if (!string.IsNullOrWhiteSpace(accessToken))
			{
				queryList.Add(Query.EQ("MAccessToken", accessToken));
			}
			if (!string.IsNullOrWhiteSpace(userId))
			{
				queryList.Add(Query.EQ("MUserID", userId));
			}
			if (!string.IsNullOrWhiteSpace(orgId))
			{
				queryList.Add(Query.EQ("MOrgID", orgId));
			}
			queryList.Add(Query.EQ("MIsDelete", false));
			return base.Collection.Find(Query.And(queryList.ToArray())).ToList();
		}

		public void Save(List<JieNor.Megi.EntityModel.MResource.MResource> resources)
		{
			base.Collection.InsertBatch(resources);
		}

		public void Remove(string accessToken = null, string userId = null, string orgId = null)
		{
			IMongoQuery[] querys = new IMongoQuery[0];
			List<IMongoQuery> queryList = querys.ToList();
			if (!string.IsNullOrWhiteSpace(accessToken))
			{
				queryList.Add(Query.EQ("MAccessToken", accessToken));
			}
			if (!string.IsNullOrWhiteSpace(userId))
			{
				queryList.Add(Query.EQ("MUserID", userId));
			}
			if (!string.IsNullOrWhiteSpace(orgId))
			{
				queryList.Add(Query.EQ("MOrgID", orgId));
			}
			base.Collection.Remove(Query.And(queryList.ToArray()));
		}

		public void RemoveWithIDs(IEnumerable<string> ids)
		{
			if (ids != null && ids.Count() != 0)
			{
				List<BsonValue> bsonIds = new List<BsonValue>();
				for (int i = 0; i < ids.Count(); i++)
				{
					bsonIds.Add(BsonValue.Create(ids.ElementAt(i)));
				}
				base.Collection.Remove(Query.In("_id", bsonIds));
			}
		}
	}
}
