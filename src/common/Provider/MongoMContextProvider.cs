using JieNor.Megi.Common.Mongo.IProvider;
using JieNor.Megi.EntityModel.Context;
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
	public class MongoMContextProvider : MongoDataProviderBase<MContext>, IMContextProvider
	{
		public MongoMContextProvider(string connectionString)
			: base(connectionString)
		{
		}

		protected override void InitMapping(BsonClassMap<MContext> classMap)
		{
			classMap.AutoMap();
			classMap.SetIdMember(classMap.GetMemberMap((MContext c) => c.MUserID));
			classMap.GetMemberMap((MContext c) => c.MExpireDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.GetMemberMap((MContext c) => c.DateNow).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.GetMemberMap((MContext c) => c.MLastAccessTime).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.GetMemberMap((MContext c) => c.MCurrentLoginTime).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.GetMemberMap((MContext c) => c.MBeginDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.GetMemberMap((MContext c) => c.MGLBeginDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.GetMemberMap((MContext c) => c.MFABeginDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.GetMemberMap((MContext c) => c.MRegDate).SetSerializationOptions(new DateTimeSerializationOptions(DateTimeKind.Local));
			classMap.SetIgnoreExtraElements(true);
		}

		public IList<MContext> GetMContext()
		{
			return base.Collection.FindAll().ToList();
		}

		public MContext GetMContextByUserId(string userId)
		{
			MContext mContext = base.Collection.FindOne(Query.EQ("_id", userId));
			if (mContext != null && !mContext.IsExpired)
			{
				mContext.MLastAccessTime = mContext.DateNow;
				SaveMContext(mContext, true);
			}
			return mContext;
		}

		public string GetMEmailByUserIp(string userIp)
		{
			MContext mContext = (from x in base.Collection.Find(Query.EQ("MUserIP", userIp))
			orderby x.MLastAccessTime
			select x).LastOrDefault();
			return (mContext == null) ? string.Empty : mContext.MEmail;
		}

		public MContext GetMContextByAccessToken(string accessToken)
		{
			MContext mContext = base.Collection.FindOne(Query.EQ("MAccessToken", accessToken));
			if (mContext != null && !mContext.IsExpired)
			{
				RefreshMContextByAccessToken(mContext.MAccessToken, mContext.DateNow);
			}
			return mContext;
		}

		public void RefreshMContextByAccessToken(string accessToken, DateTime dt)
		{
			base.Collection.Update(Query.EQ("MAccessToken", accessToken), Update.Set("MLastAccessTime", dt));
		}

		public List<MContext> GetMContextByOrgID(string orgID)
		{
			return base.Collection.Find(Query.EQ("MOrgID", orgID)).ToList();
		}

		public void UpdateMContextByKeyField(string keyName, string keyValue, string fieldName, object fieldValue, bool syn)
		{
			try
			{
				IMongoQuery query = Query.EQ(keyName, keyValue);
				Type t = fieldValue.GetType();
				UpdateBuilder update2 = null;
				if (fieldValue is bool)
				{
					update2 = Update.Set(fieldName, (bool)fieldValue);
				}
				else if (fieldValue is Array)
				{
					BsonArray array = new BsonArray();
					array.AddRange(fieldValue as Array);
					update2 = Update.Set(fieldName, array);
				}
				else
				{
					update2 = ((!(fieldValue is int)) ? Update.Set(fieldName, (string)fieldValue) : Update.Set(fieldName, (int)fieldValue));
				}
				base.Collection.Update(query, update2, UpdateFlags.Multi);
			}
			catch (Exception)
			{
			}
		}

		public MContext GetMContextByEmail(string email)
		{
			MContext mContext = base.Collection.FindOne(Query.EQ("MEmail", email));
			if (mContext != null && !mContext.IsExpired)
			{
				RefreshMContextByAccessToken(mContext.MAccessToken, mContext.DateNow);
			}
			return mContext;
		}

		public void SaveMContext(MContext context, bool refresh = true)
		{
			if (refresh)
			{
				context.MLastAccessTime = context.DateNow;
			}
			base.Collection.Save(context);
		}

		public void RemoveMContextByUserId(string userId)
		{
			base.Collection.Remove(Query.EQ("_id", userId));
		}

		public void RemoveMContextByAccessToken(string accessToken)
		{
			base.Collection.Remove(Query.EQ("MAccessToken", accessToken));
		}

		public void RefreshMContextByUserId(string userId)
		{
			MContext mContext = GetMContextByUserId(userId);
			if (mContext.IsExpired)
			{
				RefreshMContextByAccessToken(mContext.MAccessToken, mContext.DateNow);
			}
		}

		public void RefreshMContextByAccessToken(string accessToken)
		{
			MContext mContext = GetMContextByAccessToken(accessToken);
			if (mContext.IsExpired)
			{
				RefreshMContextByAccessToken(mContext.MAccessToken, mContext.DateNow);
			}
		}

		public void ExpireMContextByUserId(string userId)
		{
			MContext context = GetMContextByUserId(userId);
			context.MLastAccessTime = context.DateNow.AddMinutes((double)(-1 * context.MAccessTokenValidMinute));
			SaveMContext(context, true);
		}

		public void ExpireMContextByAccessToken(string accessToken)
		{
			MContext context = GetMContextByAccessToken(accessToken);
			context.MLastAccessTime = context.DateNow.AddMinutes((double)(-1 * context.MAccessTokenValidMinute));
			SaveMContext(context, true);
		}

		public MContext GetMContextByAccessCode(string accessCode)
		{
			MContext mContext = base.Collection.FindOne(Query.EQ("MAccessCode", accessCode));
			return mContext.IsExpired ? null : mContext;
		}

		public void RemoveMContext(Dictionary<string, string> queryDictionary)
		{
			List<IMongoQuery> queryList = new List<IMongoQuery>();
			foreach (string key in queryDictionary.Keys)
			{
				string value = queryDictionary[key];
				if (!string.IsNullOrWhiteSpace(value))
				{
					queryList.Add(Query.EQ(key, value));
				}
			}
			if (queryList.Count() > 0)
			{
				IMongoQuery query = Query.And(queryList);
				base.Collection.Remove(query);
			}
		}
	}
}
