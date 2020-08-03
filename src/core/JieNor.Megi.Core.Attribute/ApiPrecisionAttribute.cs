using System;

namespace JieNor.Megi.Core.Attribute
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ApiPrecisionAttribute : System.Attribute
	{
		public int Precision
		{
			get;
			set;
		}

		public ApiPrecisionAttribute(int precision)
		{
			Precision = precision;
		}
	}
}
