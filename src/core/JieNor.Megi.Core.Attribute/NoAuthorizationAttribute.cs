using System;

namespace JieNor.Megi.Core.Attribute
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class NoAuthorizationAttribute : System.Attribute
	{
	}
}
