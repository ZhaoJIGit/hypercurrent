using MySql.Data.MySqlClient;
using System;

namespace JieNor.Megi.Core.MResource
{
	public class MResourceFilter
	{
		public IResourceAdapter Adapter = new CommonResourceAdpater();

		private Type _type;

		private string _tableName;

		private object _maxLength;

		private object _startWith;

		private string _filledChar;

		private object _startAfterMax;

		private string _fieldName;

		private string _pkFieldName;

		private string _prefix;

		public int Count
		{
			get;
			set;
		}

		public Type Type
		{
			get
			{
				return _type ?? Adapter.Type;
			}
			set
			{
				_type = value;
			}
		}

		public string TableName
		{
			get
			{
				return _tableName ?? Adapter.TableName;
			}
			set
			{
				_tableName = value;
			}
		}

		public int MaxLength
		{
			get
			{
				return (_maxLength == null) ? Adapter.MaxLength : int.Parse(_maxLength.ToString());
			}
			set
			{
				_maxLength = value;
			}
		}

		public int StartWith
		{
			get
			{
				return (_startWith == null) ? Adapter.StartWith : int.Parse(_startWith.ToString());
			}
			set
			{
				_startWith = value;
			}
		}

		public string FilledChar
		{
			get
			{
				return string.IsNullOrWhiteSpace(_filledChar) ? Adapter.FilledChar : _filledChar;
			}
			set
			{
				_filledChar = value;
			}
		}

		public bool StartAfterMax
		{
			get
			{
				return (_startAfterMax == null) ? Adapter.StartAfterMax : Convert.ToBoolean(_startAfterMax);
			}
			set
			{
				_startAfterMax = value;
			}
		}

		public string SqlFilter
		{
			get;
			set;
		}

		public MySqlParameter[] SqlFitlerParams
		{
			get;
			set;
		}

		public string FieldName
		{
			get
			{
				return _fieldName ?? Adapter.FieldName;
			}
			set
			{
				_fieldName = value;
			}
		}

		public string PKFieldName
		{
			get
			{
				return _pkFieldName ?? Adapter.PKFieldName;
			}
			set
			{
				_pkFieldName = value;
			}
		}

		public bool FillIfInvalid
		{
			get;
			set;
		}

		public string Prefix
		{
			get
			{
				return _prefix ?? Adapter.Prefix;
			}
			set
			{
				_prefix = value;
			}
		}

		public string ResourcePrefix
		{
			get;
			set;
		}

		public bool NoHandle
		{
			get;
			set;
		}

		public MResourceFilter(Type type)
		{
			Type = type;
		}
	}
}
