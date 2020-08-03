using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.DataRepository.API
{
	public class APIBaseData
	{
		private List<KeyValuePair<Type, object>> Data = new List<KeyValuePair<Type, object>>();

		public APIBaseData AddOrUpdate<T>(List<T> data)
		{
			if (Data.Exists((Predicate<KeyValuePair<Type, object>>)((KeyValuePair<Type, object> x) => x.Key == typeof(T))))
			{
				Data.RemoveAt(Data.FindIndex((Predicate<KeyValuePair<Type, object>>)((KeyValuePair<Type, object> x) => x.Key == typeof(T))));
			}
			Data.Add(new KeyValuePair<Type, object>(typeof(T), (object)data));
			return this;
		}

		public List<T> Get<T>()
		{
			if (!Data.Exists((Predicate<KeyValuePair<Type, object>>)((KeyValuePair<Type, object> x) => x.Key == typeof(T))))
			{
				return null;
			}
			return Data.FirstOrDefault((KeyValuePair<Type, object> x) => x.Key == typeof(T)).Value as List<T>;
		}

		public APIBaseData Clear(Type type = null)
		{
			if (Data == null || Data.Count == 0)
			{
				return this;
			}
			if (type == (Type)null)
			{
				Data = new List<KeyValuePair<Type, object>>();
			}
			else
			{
				Data.RemoveAt(Data.FindIndex((KeyValuePair<Type, object> x) => x.Key == type));
			}
			return this;
		}
	}
}
