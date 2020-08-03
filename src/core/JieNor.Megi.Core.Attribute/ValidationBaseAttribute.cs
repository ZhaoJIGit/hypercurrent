using System;

namespace JieNor.Megi.Core.Attribute
{
	public class ValidationBaseAttribute : System.Attribute
	{
		public string LangKey
		{
			get;
			set;
		}

		public virtual bool IsValid(string value)
		{
			return true;
		}
	}
}
