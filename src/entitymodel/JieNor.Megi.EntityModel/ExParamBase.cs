using System.Collections.Generic;

namespace JieNor.Megi.EntityModel
{
	public class ExParamBase
	{
		public string ElementID
		{
			get;
			set;
		}

		public string MOrgID
		{
			get;
			set;
		}

		public List<string> MOrgIDs
		{
			get;
			set;
		}

		public string MUserID
		{
			get;
			set;
		}

		public string MGroupName
		{
			get;
			set;
		}

		public string MGroupID
		{
			get;
			set;
		}

		public string MCreateBy
		{
			get;
			set;
		}

		public bool IgnoreModifiedSince
		{
			get;
			set;
		}
	}
}
