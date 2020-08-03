using System.Collections.Generic;

namespace JieNor.Megi.Core
{
	public class ApiResult<T> where T : new()
	{
		public bool Success
		{
			get;
			set;
		}

		public string Message
		{
			get;
			set;
		}

		public string Code
		{
			get;
			set;
		}

		public List<T> DataList
		{
			get;
			set;
		}

		public ApiResult()
		{
			Success = false;
			Message = "";
			Code = "";
		}
	}
}
