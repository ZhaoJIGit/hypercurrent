using Newtonsoft.Json;

namespace JieNor.Megi.Core.Utility
{
	public static class JsonUtility
	{
		public static string Serializer(this object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}

		public static T Deserialize<T>(this string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}
	}
}
