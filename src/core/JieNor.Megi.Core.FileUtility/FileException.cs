using System;

namespace JieNor.Megi.Core.FileUtility
{
	[Serializable]
	public class FileException : Exception
	{
		public FileExceptionType ExceptionType
		{
			get;
			set;
		}

		public FileException(FileExceptionType exceptionType)
		{
			ExceptionType = exceptionType;
		}
	}
}
