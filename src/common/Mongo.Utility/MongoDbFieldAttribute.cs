using System;

namespace JieNor.Megi.Common.Mongo.Utility
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class MongoDbFieldAttribute : Attribute
	{
		public bool IsIndex
		{
			get;
			set;
		}

		public bool Unique
		{
			get;
			set;
		}

		public bool Ascending
		{
			get;
			set;
		}

		public MongoDbFieldAttribute(bool _isIndex)
		{
			IsIndex = _isIndex;
			Unique = false;
			Ascending = true;
		}
	}
}
