using System.Collections.Generic;

namespace JieNor.Megi.DataModel.MSG
{
	public class MSGMessageOrgUserModel
	{
		public string MOrgID
		{
			get;
			set;
		}

		public string MOrgName
		{
			get;
			set;
		}

		public List<MSGMessageUserModel> MUserList
		{
			get;
			set;
		}
	}
}
