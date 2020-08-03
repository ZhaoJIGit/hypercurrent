using System.Collections.Generic;

namespace JieNor.Megi.Core
{
	public class DataGridJson<T>
	{
		public int total
		{
			get;
			set;
		}

		public List<T> rows
		{
			get;
			set;
		}

		public object obj
		{
			get;
			set;
		}
	}
}
