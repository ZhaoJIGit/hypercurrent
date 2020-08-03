using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JieNor.Megi.Common.Mongo.Utility
{
	public class MongoDbHelper
	{
		private MongoDatabase _db;

		private readonly string OBJECTID_KEY = "_id";

		public MongoDbHelper(string connectionString, string dataBase)
		{
			_db = new MongoDb(connectionString, dataBase).GetDataBase();
		}

		public MongoDbHelper(MongoDatabase db)
		{
			_db = db;
		}

		public T GetNextSequence<T>(IMongoQuery query, SortByDocument sortBy, UpdateDocument update, string collectionName, string indexName)
		{
			if (_db == null)
			{
				return default(T);
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				query = InitQuery(query);
				sortBy = InitSortBy(sortBy, OBJECTID_KEY);
				update = InitUpdateDocument(update, indexName);
				FindAndModifyResult ido = mc.FindAndModify(query, sortBy, update, true, false);
				return ido.GetModifiedDocumentAs<T>();
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public bool CollectionExists(string collectionName)
		{
			return _db.CollectionExists(collectionName);
		}

		public IEnumerable<string> GetAllCollections()
		{
			return _db.GetCollectionNames();
		}

		public MongoCollection<BsonDocument> GetCollection(string collectionName)
		{
			if (!_db.CollectionExists(collectionName))
			{
				_db.CreateCollection(collectionName);
			}
			return _db.GetCollection<BsonDocument>(collectionName);
		}

		public MongoCollection<T> GetCollection<T>(string collectionName)
		{
			if (!_db.CollectionExists(collectionName))
			{
				_db.CreateCollection(collectionName);
			}
			return _db.GetCollection<T>(collectionName);
		}

		public bool Insert<T>(T t)
		{
			string collectionName = typeof(T).Name;
			return Insert(t, collectionName);
		}

		public bool Insert<T>(T t, string collectionName)
		{
			if (_db == null)
			{
				return false;
			}
			try
			{
				MongoCollection<BsonDocument> mc = GetCollection(collectionName);
				BsonDocument bd = t.ToBsonDocument();
				WriteConcernResult result = mc.Insert(bd);
				if (!string.IsNullOrEmpty(result.ErrorMessage))
				{
					return false;
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool Insert<T>(List<T> list)
		{
			string collectionName = typeof(T).Name;
			return Insert(list, collectionName);
		}

		public bool Insert<T>(List<T> list, string collectionName)
		{
			if (_db == null)
			{
				return false;
			}
			try
			{
				MongoCollection<BsonDocument> mc = GetCollection(collectionName);
				List<BsonDocument> bsonList = new List<BsonDocument>();
				list.ForEach((Action<T>)delegate(T t)
				{
					bsonList.Add(BsonExtensionMethods.ToBsonDocument<T>(t));
				});
				mc.InsertBatch((IEnumerable<BsonDocument>)bsonList);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public List<T> FindAll<T>(string collectionName)
		{
			if (_db == null)
			{
				return null;
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				MongoCursor<T> mongoCursor = mc.FindAll();
				return mongoCursor.ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public List<T> FindAll<T>()
		{
			string collectionName = typeof(T).Name;
			return FindAll<T>(collectionName);
		}

		public T FindOneToIndexMax<T>(string collectionName, string[] sort)
		{
			return FindOneToIndexMax<T>(null, collectionName, sort);
		}

		public T FindOneToIndexMax<T>(IMongoQuery query, string collectionName, string[] sort)
		{
			if (_db == null)
			{
				return default(T);
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				query = InitQuery(query);
				return mc.Find(query).SetSortOrder((IMongoSortBy)SortBy.Descending(sort)).FirstOrDefault();
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public T FindOne<T>(IMongoQuery query, string collectionName)
		{
			if (_db == null)
			{
				return default(T);
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				query = InitQuery(query);
				return mc.FindOne(query);
			}
			catch (Exception)
			{
				return default(T);
			}
		}

		public T FindOne<T>(string collectionName)
		{
			return FindOne<T>(null, collectionName);
		}

		public T FindOne<T>()
		{
			string collectionName = typeof(T).Name;
			return FindOne<T>(null, collectionName);
		}

		public T FindOne<T>(IMongoQuery query)
		{
			string collectionName = typeof(T).Name;
			return FindOne<T>(query, collectionName);
		}

		public List<T> Find<T>(IMongoQuery query, string collectionName)
		{
			if (_db == null)
			{
				return null;
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				query = InitQuery(query);
				MongoCursor<T> mongoCursor = mc.Find(query);
				return mongoCursor.ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public List<T> Find<T>(IMongoQuery query)
		{
			string collectionName = typeof(T).Name;
			return Find<T>(query, collectionName);
		}

		public List<T> Find<T>(IMongoQuery query, int pageIndex, int pageSize, SortByDocument sortBy, string collectionName)
		{
			if (_db == null)
			{
				return null;
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				MongoCursor<T> mongoCursor2 = null;
				query = InitQuery(query);
				sortBy = InitSortBy(sortBy, OBJECTID_KEY);
				pageIndex = ((pageIndex == 0) ? 1 : pageIndex);
				mongoCursor2 = mc.Find(query).SetSortOrder((IMongoSortBy)sortBy).SetSkip((pageIndex - 1) * pageSize)
					.SetLimit(pageSize);
				return mongoCursor2.ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public List<T> Find<T>(IMongoQuery query, int pageIndex, int pageSize, SortByDocument sortBy)
		{
			string collectionName = typeof(T).Name;
			return Find<T>(query, pageIndex, pageSize, sortBy, collectionName);
		}

		public List<T> Find<T>(IMongoQuery query, string indexName, object lastKeyValue, int pageSize, int sortType, string collectionName)
		{
			if (_db == null)
			{
				return null;
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				MongoCursor<T> mongoCursor2 = null;
				query = InitQuery(query);
				if (sortType > 0)
				{
					if (lastKeyValue != null)
					{
						query = Query.And(query, Query.GT(indexName, BsonValue.Create(lastKeyValue)));
					}
					mongoCursor2 = mc.Find(query).SetSortOrder((IMongoSortBy)new SortByDocument(indexName, 1)).SetLimit(pageSize);
				}
				else
				{
					if (lastKeyValue != null)
					{
						query = Query.And(query, Query.LT(indexName, BsonValue.Create(lastKeyValue)));
					}
					mongoCursor2 = mc.Find(query).SetSortOrder((IMongoSortBy)new SortByDocument(indexName, -1)).SetLimit(pageSize);
				}
				return mongoCursor2.ToList();
			}
			catch (Exception)
			{
				return null;
			}
		}

		public List<T> Find<T>(IMongoQuery query, string indexName, object lastKeyValue, int pageSize, int sortType)
		{
			string collectionName = typeof(T).Name;
			return Find<T>(query, indexName, lastKeyValue, pageSize, sortType, collectionName);
		}

		public List<T> Find<T>(IMongoQuery query, string lastObjectId, int pageSize, int sortType, string collectionName)
		{
			return Find<T>(query, OBJECTID_KEY, new ObjectId(lastObjectId), pageSize, sortType, collectionName);
		}

		public List<T> Find<T>(IMongoQuery query, string lastObjectId, int pageSize, int sortType)
		{
			string collectionName = typeof(T).Name;
			return Find<T>(query, lastObjectId, pageSize, sortType, collectionName);
		}

		public bool Update<T>(IMongoQuery query, IMongoUpdate update, string collectionName)
		{
			if (_db == null)
			{
				return false;
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				query = InitQuery(query);
				WriteConcernResult result = mc.Update(query, update, UpdateFlags.Multi);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool Update<T>(IMongoQuery query, IMongoUpdate update)
		{
			string collectionName = typeof(T).Name;
			return Update<T>(query, update, collectionName);
		}

		public bool Remove<T>(IMongoQuery query, string collectionName)
		{
			if (_db == null)
			{
				return false;
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				query = InitQuery(query);
				mc.Remove(query);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool Remove<T>(IMongoQuery query)
		{
			string collectionName = typeof(T).Name;
			return Remove<T>(query, collectionName);
		}

		public bool RemoveAll<T>()
		{
			string collectionName = typeof(T).Name;
			return Remove<T>(null, collectionName);
		}

		public bool Drop(string collectionName)
		{
			if (_db == null)
			{
				return false;
			}
			try
			{
				MongoCollection<BsonDocument> mc = GetCollection(collectionName);
				mc.Drop();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool RemoveAll(string collectionName)
		{
			if (_db == null)
			{
				return false;
			}
			try
			{
				MongoCollection<BsonDocument> mc = GetCollection(collectionName);
				mc.RemoveAll();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool RemoveAll<T>(string collectionName)
		{
			return Remove<T>(null, collectionName);
		}

		public bool CreateIndex<T>()
		{
			if (_db == null)
			{
				return false;
			}
			try
			{
				string collectionName = typeof(T).Name;
				MongoCollection<BsonDocument> mc = GetCollection(collectionName);
				PropertyInfo[] propertys = typeof(T).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
				PropertyInfo[] array = propertys;
				foreach (PropertyInfo property in array)
				{
					object[] customAttributes = property.GetCustomAttributes(true);
					foreach (object obj in customAttributes)
					{
						MongoDbFieldAttribute mongoField = obj as MongoDbFieldAttribute;
						if (mongoField != null)
						{
							IndexKeysBuilder indexKey = (!mongoField.Ascending) ? IndexKeys.Descending(property.Name) : IndexKeys.Ascending(property.Name);
							mc.CreateIndex(indexKey, IndexOptions.SetUnique(mongoField.Unique));
						}
					}
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public long GetCount<T>(IMongoQuery query, string collectionName)
		{
			if (_db == null)
			{
				return 0L;
			}
			try
			{
				MongoCollection<T> mc = this.GetCollection<T>(collectionName);
				if (query == null)
				{
					return mc.Count();
				}
				return mc.Count(query);
			}
			catch (Exception)
			{
				return 0L;
			}
		}

		public long GetCount<T>(IMongoQuery query)
		{
			string collectionName = typeof(T).Name;
			return GetCount<T>(query, collectionName);
		}

		public long GetDataSize<T>()
		{
			string collectionName = typeof(T).Name;
			return GetDataSize(collectionName);
		}

		public long GetDataSize(string collectionName)
		{
			if (_db == null)
			{
				return 0L;
			}
			try
			{
				MongoCollection<BsonDocument> mc = GetCollection(collectionName);
				return mc.GetTotalStorageSize();
			}
			catch (Exception)
			{
				return 0L;
			}
		}

		private IMongoQuery InitQuery(IMongoQuery query)
		{
			if (query == null)
			{
				query = Query.Exists(OBJECTID_KEY);
			}
			return query;
		}

		private SortByDocument InitSortBy(SortByDocument sortBy, string sortByName)
		{
			if ((BsonDocument)sortBy == (BsonDocument)null)
			{
				sortBy = new SortByDocument(sortByName, -1);
			}
			return sortBy;
		}

		private UpdateDocument InitUpdateDocument(UpdateDocument update, string indexName)
		{
			if ((BsonDocument)update == (BsonDocument)null)
			{
				update = new UpdateDocument("$inc", new QueryDocument(indexName, 0));
			}
			return update;
		}
	}
}
