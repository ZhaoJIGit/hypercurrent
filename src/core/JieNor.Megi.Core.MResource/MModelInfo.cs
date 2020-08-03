using Fasterflect;
using JieNor.Megi.Core.DBUtility;
using JieNor.Megi.EntityModel.COM;
using System;
using System.Reflection;

namespace JieNor.Megi.Core.MResource
{
	public class MModelInfo
	{
		private Type _type;

		private string _tableName;

		private PropertyInfo[] _properties;

		public static object _instance;

		public Type type
		{
			get
			{
				return _type;
			}
			private set
			{
			}
		}

		public string TableName
		{
			get
			{
				if (_tableName == null)
				{
					_tableName = Instance.GetPropertyValue("TableName").ToString();
				}
				return _tableName;
			}
			private set
			{
			}
		}

		public PropertyInfo[] Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = type.GetProperties();
				}
				return _properties;
			}
			private set
			{
			}
		}

		private MTableColumnModel[] _columns
		{
			get;
			set;
		}

		public MTableColumnModel[] Columns
		{
			get
			{
				if (_columns == null)
				{
					_columns = TableColumnHelper.GetTableColumnModels(TableName).ToArray();
				}
				return _columns;
			}
			private set
			{
			}
		}

		public object Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Activator.CreateInstance(type);
				}
				return _instance;
			}
			private set
			{
			}
		}

		public MModelInfo(Type type)
		{
			_type = type;
		}
	}
}
