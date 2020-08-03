using System;

namespace JieNor.Megi.Core.Attribute
{
	public class DBFieldValidateAttribute : System.Attribute
	{
		public string TableName
		{
			get;
			set;
		}

		public string ColName
		{
			get;
			set;
		}

		public bool IgnoreLengthValidate
		{
			get;
			set;
		}

		public DBFieldValidateAttribute()
		{
		}

		public DBFieldValidateAttribute(string tableName, string colName)
		{
			TableName = tableName;
			ColName = colName;
		}
	}
}
