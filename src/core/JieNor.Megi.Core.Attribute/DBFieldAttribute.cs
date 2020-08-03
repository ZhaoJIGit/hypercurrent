using System;

namespace JieNor.Megi.Core.Attribute
{
	public class DBFieldAttribute : System.Attribute
	{
		public string FieldName
		{
			get;
			set;
		}

		public DBFieldAttribute(string fieldName)
		{
			FieldName = fieldName;
		}
	}
}
