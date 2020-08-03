namespace JieNor.Megi.Core
{
	public static class SqlFilterExtensions
	{
		public static SqlFilter And(this SqlFilter filter1, SqlFilter filter2)
		{
			filter1.AppendFilter(filter2, " And ");
			return filter1;
		}

		public static SqlFilter And(this SqlFilter filter1, FilterList filter2)
		{
			filter1.AppendFilter(filter2, " And ");
			return filter1;
		}

		public static SqlFilter Or(this SqlFilter filter1, SqlFilter filter2)
		{
			filter1.AppendFilter(filter2, " Or ");
			return filter1;
		}

		public static SqlFilter Or(this SqlFilter filter1, FilterList filter2)
		{
			filter1.AppendFilter(filter2, " And ");
			return filter1;
		}
	}
}
