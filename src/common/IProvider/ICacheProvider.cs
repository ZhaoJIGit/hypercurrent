namespace JieNor.Megi.Common.Mongo.IProvider
{
	public interface ICacheProvider
	{
		T GetData<T>(string key);

		void SaveData<T>(string key, T data);

		void RemoveData(string key);

		void RemoveByPrefix(string prefix);
	}
}
