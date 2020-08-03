using System.Configuration;
using System.Web.Caching;

namespace JieNor.Megi.Common.Cache
{
	public class TableCacheDependency : ICacheDependency
	{
		private static readonly string _DataBaseName = ConfigurationManager.AppSettings["CacheDatabaseName"];

		private char[] configurationSeparator = new char[1]
		{
			','
		};

		private AggregateCacheDependency _Dependency = new AggregateCacheDependency();

		public TableCacheDependency(string tableConfig)
		{
			string[] tables = tableConfig.Split(configurationSeparator);
			string[] array = tables;
			foreach (string table in array)
			{
				_Dependency.Add(new SqlCacheDependency(_DataBaseName, table));
			}
		}

		public CacheDependency GetDependency()
		{
			return _Dependency;
		}
	}
}
