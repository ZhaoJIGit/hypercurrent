using MongoDB.Driver;

namespace JieNor.Megi.Common.Mongo.Utility
{
	public class MongoDb
	{
		private string ConnectionString;

		private string DataBase;

		public MongoDb(string conectionString, string dataBase)
		{
			ConnectionString = conectionString;
			DataBase = dataBase;
		}

		public MongoDatabase GetDataBase()
		{
			MongoClient client = new MongoClient(ConnectionString);
			return client.GetServer().GetDatabase(DataBase);
		}
	}
}
