using System.Web.Mvc;

namespace JieNor.Megi.Common.Framework
{
	public class MvcApplicationStart
	{
		public void Start()
		{
			RebuildJsonValueProviderFactory();
		}

		private void RebuildJsonValueProviderFactory()
		{
			ValueProviderFactory jsonFactory = null;
			foreach (ValueProviderFactory factory in ValueProviderFactories.Factories)
			{
				if (factory.GetType().FullName == "System.Web.Mvc.JsonValueProviderFactory")
				{
					jsonFactory = factory;
					break;
				}
			}
			if (jsonFactory != null)
			{
				ValueProviderFactories.Factories.Remove(jsonFactory);
			}
			JsonValueProviderFactory largeJsonValueProviderFactory = new JsonValueProviderFactory();
			ValueProviderFactories.Factories.Add(largeJsonValueProviderFactory);
		}
	}
}
