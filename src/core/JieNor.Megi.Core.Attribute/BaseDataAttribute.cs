using System;
using System.Collections.Generic;
using System.Linq;

namespace JieNor.Megi.Core.Attribute
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class BaseDataAttribute : System.Attribute
	{
		public List<string> MatchFields = new List<string>();

		public string IDField
		{
			get;
			set;
		}

		public BaseDataAttribute(string idField, params string[] fields)
		{
			IDField = idField;
			MatchFields = fields.ToList();
		}
	}
}
