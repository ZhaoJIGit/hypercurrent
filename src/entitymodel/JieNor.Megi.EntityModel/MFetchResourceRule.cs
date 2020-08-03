using System;

namespace JieNor.Megi.EntityModel
{
	public class MFetchResourceRule
	{
		public Type Type = typeof(int);

		public int MaxLength = 3;

		public int StartWith = 1;

		public char FilledWith = '0';

		public bool StartAfterMax = true;

		public string FieldName
		{
			get;
			set;
		}
	}
}
