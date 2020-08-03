using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Core.Attribute
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class AppSourceAttribute : System.Attribute
	{
		public List<string> AppSourceList;

		public AppSourceAttribute(params string[] apps)
		{
			AppSourceList = ((apps != null) ? apps.ToList() : null);
		}
	}
}
