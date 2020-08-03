using System.Collections.Generic;

namespace JieNor.Megi.EntityModel
{
	public class PostParam<T> : ExParamBase where T : new()
	{
		public List<T> DataList
		{
			get;
			set;
		}

		public bool IsPut
		{
			get;
			set;
		}

		public PostParam()
		{
			DataList = new List<T>();
		}
	}
}
