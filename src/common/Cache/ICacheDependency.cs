using System.Web.Caching;

namespace JieNor.Megi.Common.Cache
{
	public interface ICacheDependency
	{
		CacheDependency GetDependency();
	}
}
