using JieNor.Megi.Core.DataModel;
using System;

namespace JieNor.Megi.Core.Attribute
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ApiEnumAttribute : System.Attribute
	{
		private bool _IsRequired = true;

		public EnumMappingType Type
		{
			get;
			set;
		}

		public bool IsRequired
		{
			get
			{
				return _IsRequired;
			}
			set
			{
				_IsRequired = value;
			}
		}

		public ApiEnumAttribute(EnumMappingType type)
		{
			Type = type;
		}
	}
}
