using System;
using System.Data;

namespace JieNor.Megi.Core.Repository
{
	public class ModelToMySqlInfor
	{
		public string MainTableName
		{
			get;
			set;
		}

		public string MainTablePKFieldName
		{
			get;
			set;
		}

		public string MainTableSelectSql
		{
			get;
			set;
		}

		public string MainTableInsertSql
		{
			get;
			set;
		}

		public string MainTableUpdateSql
		{
			get;
			set;
		}

		[Obsolete]
		public string MainTableDeleteSql
		{
			get;
			set;
		}

		public string MainDeleteFlagSql
		{
			get;
			set;
		}

		public string MainDeleteFlagByFKSql
		{
			get;
			set;
		}

		public string MainArchiveFlagSql
		{
			get;
			set;
		}

		public bool HaveMultiLangTable
		{
			get;
			set;
		}

		public DataColumnCollection MultiCols
		{
			get;
			set;
		}

		public string MultiLangTableInsertSql
		{
			get;
			set;
		}

		public string MultiLangTableUpdateSql
		{
			get;
			set;
		}

		[Obsolete]
		public string MultiLangTableDeleteSql
		{
			get;
			set;
		}

		public string MultiLangDeleteFlagSql
		{
			get;
			set;
		}

		public string MultiLangTableSelectSql
		{
			get;
			set;
		}
	}
}
