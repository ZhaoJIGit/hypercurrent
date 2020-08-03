using System.Collections.Generic;

namespace JieNor.Megi.Core
{
	public class ApiPostParam<T> : ApiParam where T : new()
	{
		public List<T> DataList
		{
			get;
			set;
		}

		public ApiPostParam()
		{
			DataList = new List<T>();
		}
	}
}
