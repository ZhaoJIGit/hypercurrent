using System;
using System.Runtime.Serialization;

namespace JieNor.Megi.Core
{
	[DataContract]
	public class ApiEndpoitUnavaliableException : Exception
	{
		public ApiEndpoitUnavaliableException(string message)
			: base(message)
		{
		}
	}
}
