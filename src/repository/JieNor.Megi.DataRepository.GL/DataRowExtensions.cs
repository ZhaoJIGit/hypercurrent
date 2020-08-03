using System;
using System.Data;

namespace JieNor.Megi.DataRepository.GL
{
	public static class DataRowExtensions
	{
		public static T MField<T>(this DataRow row, string columnName)
		{
			if ((typeof(T) == typeof(decimal) || typeof(T) == typeof(int) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong) || typeof(T) == typeof(uint)) && (row.IsNull(columnName) || string.IsNullOrWhiteSpace(row[columnName].ToString())))
			{
				return default(T);
			}
			if (typeof(T) == typeof(DateTime))
			{
				return (row[columnName] == null || string.IsNullOrWhiteSpace(row[columnName].ToString())) ? default(T) : ((T)(object)Convert.ToDateTime(row[columnName]));
			}
			if (typeof(T) == typeof(bool))
			{
				return (row[columnName] == null || string.IsNullOrWhiteSpace(row[columnName].ToString())) ? default(T) : ((T)(object)(int.Parse(row[columnName].ToString()) == 1));
			}
			return row.Field<T>(columnName);
		}

		public static string MField(this DataRow row, string columnName)
		{
			return row.MField<string>(columnName);
		}
	}
}
