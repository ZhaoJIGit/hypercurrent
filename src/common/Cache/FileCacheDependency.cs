using System;
using System.Web.Caching;

namespace JieNor.Megi.Common.Cache
{
	public class FileCacheDependency : ICacheDependency
	{
		private CacheDependency _Dependency = null;

		public FileCacheDependency(string fileName)
		{
			_Dependency = new CacheDependency(fileName);
		}

		public FileCacheDependency(string fileName, DateTime start)
		{
			_Dependency = new CacheDependency(fileName, start);
		}

		public CacheDependency GetDependency()
		{
			return _Dependency;
		}
	}
}
